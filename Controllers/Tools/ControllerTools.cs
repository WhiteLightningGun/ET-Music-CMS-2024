using static System.IO.Path;
using UpLoader_For_ET.Data;
using UpLoader_For_ET.Configuration;
using Microsoft.Extensions.Options;

namespace UpLoader_For_ET.Tools;

/// <summary>
/// Performs routine data retrieval shared across controllers for user ID hash, space for user, and percentage space used.
/// </summary>
public class ControllerTools
{
    private readonly ApplicationDbContext _context;
    private readonly string MainDirectory = Environment.CurrentDirectory;
    public UserSpaceLimitSetting userSpaceLimit;

    public ControllerTools(ApplicationDbContext context, IOptions<UserSpaceLimitSetting> _userSpaceLimit)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        userSpaceLimit =_userSpaceLimit.Value;
    }

    /// <summary>
    /// Queries the database and returns the GUID associated with a username.
    /// </summary>
    /// <param name="userName"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public string GetUserIDHash(string userName)
    {
        var user = _context.Users.FirstOrDefault(c => c.UserName == userName);

        if (user == null)
        {
            throw new ArgumentException($"No user found with the username: {userName}", nameof(userName));
        }

        return user.Id;
    }

    public decimal GetSpaceForUser(string userName)
    {
        var userIDHash = GetUserIDHash(userName);
        string FileBayPathForUser = Combine(MainDirectory, $"FileBay/{userIDHash}");

        if (!Directory.Exists(FileBayPathForUser))
        {
            Directory.CreateDirectory(FileBayPathForUser);
        }

        //Calculate the amount of space used by user
        DirectoryInfo userDirectoryInfo = new(FileBayPathForUser);
        FileInfo[] userFileInfo = userDirectoryInfo.GetFiles();

        decimal totalBytes = 0;

        foreach (var file in userFileInfo)
        {
            totalBytes += file.Length;
        }

        decimal totalMegabytes = totalBytes / (1000 * 1000);
        return totalMegabytes;
    }

    /// <summary>
    /// Checks the user file folder and returns false if it exceeeds alloted max space.
    /// </summary>
    /// <param name="userName"></param>
    /// <returns></returns>
    public bool CheckSpaceForUser(string? userName, long uploadedFileLength)
    {
        var userIDHash = GetUserIDHash(userName!);
        string FileBayPathForUser = Combine(MainDirectory, $"FileBay/{userIDHash}");

        Directory.CreateDirectory(FileBayPathForUser);

        //Calculate the amount of space used by user
        DirectoryInfo userDirectoryInfo = new(FileBayPathForUser);
        FileInfo[] userFileInfo = userDirectoryInfo.GetFiles();
        decimal totalBytes = uploadedFileLength + userFileInfo.Sum(fileInfo => fileInfo.Length);

        decimal totalMegabytes = totalBytes / (1000*1000);

        return totalMegabytes <= userSpaceLimit.userLimitMB; 
    }

    /// <summary>
    /// Returns a percentage value describing how much space a user has used.
    /// </summary>
    /// <param name="userName"></param>
    /// <returns></returns>
    public decimal? PercentageSpaceUsed(string userName)
    {
        decimal spaceUsedMB = GetSpaceForUser(userName);
        int? spacePermitted = userSpaceLimit.userLimitMB;
        return spacePermitted != null ? spaceUsedMB * 100 / spacePermitted : 0;
    }
}