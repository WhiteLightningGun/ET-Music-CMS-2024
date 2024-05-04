using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using UpLoader_For_ET.Models;
using Microsoft.AspNetCore.Authorization;
using UpLoader_For_ET.Data;
using UpLoader_For_ET.DBModels;
using static System.IO.Path;
using UpLoader_For_ET.Configuration;
using Microsoft.AspNetCore.Identity.UI.Services;
using UpLoader_For_ET.Services;
using System.Text;
using System.Text.Encodings.Web;
using UpLoader_For_ET.Tools;
using Microsoft.Extensions.Options;
using UpLoader_For_ET.StaticClasses;
using Microsoft.AspNetCore.Identity;

namespace UpLoader_For_ET.Controllers;
public class UserController : Controller
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext ApplicationDB;
    public UserSpaceLimitSetting userSpaceLimit;
    private MessageLimitSetting messageSettings;
    private readonly UserManager<IdentityUser> userManager;
    private ContactFormVisibility contactFormControl;
    private readonly IEmailSender emailSender;
    private static string MainDirectory = Environment.CurrentDirectory;
    public readonly ControllerTools ct;
    
    public UserController(ILogger<HomeController> logger, 
    ApplicationDbContext injectedADB, 
    IWebHostEnvironment env,
    IOptions<UserSpaceLimitSetting> _userSpaceLimit,
    IEmailSender _emailSender,
    ContactFormVisibility contactFormVisibility,
    IOptions<MessageLimitSetting> _messageSettings,
    ControllerTools _controllerTools,
    UserManager<IdentityUser> userManager)
    {
        _logger = logger;
        ApplicationDB = injectedADB;
        _env = env;
        userSpaceLimit = _userSpaceLimit.Value;
        emailSender = _emailSender;
        contactFormControl = contactFormVisibility;
        messageSettings = _messageSettings.Value;
        ct = _controllerTools;
        this.userManager = userManager;
    }

    [Authorize]
    public IActionResult UserSummary()
    {
        string userName = User.Identity!.Name!;
        string userIDHash = ct.GetUserIDHash(userName);
        decimal spaceUsed = ct.GetSpaceForUser(userName);

        UserSummaryModel userSummaryModel = new()
        {
            userName = userName,
            uploadDBEntries = ApplicationDB.UploadDBEntries?.Select(x => x).Where(x => x.userHash == userIDHash).ToList(),
            percentageUsed = ct.PercentageSpaceUsed(userName),
            spaceUsedMB = spaceUsed,
            spaceRemaining = userSpaceLimit.userLimitMB - spaceUsed
        };

        return View(userSummaryModel);
    }

    [Authorize]
    [Authorize(Roles="Plebian,User,Administrators")]
    public IActionResult UserDownloads()
    {
        string userIDHash = ct.GetUserIDHash(User.Identity!.Name!);
        UserDownloadModel userdownloadmodel = new()
        {
            uploadDBEntries = ApplicationDB.UploadDBEntries?.Select(x => x).Where(x => x.userHash == userIDHash).ToList()
        };
        return View(userdownloadmodel);
    }

    [Authorize(Roles="Plebian,User,Administrators")]
    [HttpPost]
    public async Task<IActionResult> UserDownloads(string argFileHash, string argFileName)
    {
        Console.WriteLine("User/UserDownloads Called");
        string userIDHash = ct.GetUserIDHash(User.Identity!.Name!);
        string fileNameHash = argFileHash;
        string FileBayPathForUser = Combine(MainDirectory, $"FileBay/{userIDHash}/{fileNameHash}");

        //get file as bytes, save with user-friendly name parameter
        byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(FileBayPathForUser);
        return File(fileBytes, "application/force-download", argFileName); 
    }

    [Authorize(Roles = "Administrators")]
    [HttpPost]
    public async Task<IActionResult> DeleteUser(string userEmailSubmission, string selectedRole, string currentRole)
    {
        Console.WriteLine($"DeleteUser method was called with argument {userEmailSubmission} selectedRole {selectedRole} currentRole {currentRole}.");

        var user = await userManager.FindByEmailAsync(userEmailSubmission);

        if(user == null)
        {
            TempData["Alert"] = "User not found!!!";
            return View();
        }
        else
        {
            // deleteall files in filebay associated with this user
            var userIDHash = await userManager.GetUserIdAsync(user);
            string FileBayPathForUser = Combine(MainDirectory, $"FileBay/{userIDHash}");

            //what if directory doesn't exist?
            if (Directory.Exists(FileBayPathForUser))
            {
                string[] files = Directory.GetFiles(FileBayPathForUser, "*", SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    System.IO.File.Delete(file);
                }

                Directory.Delete(FileBayPathForUser); //Deletes empty directory

                // deleting any entries in Uploads DB
                var uploadEntries = ApplicationDB.UploadDBEntries?.Select(x => x).Where(x => x.userHash == userIDHash).ToList();

                if (uploadEntries != null)
                {
                    foreach (var entry in uploadEntries)
                    {
                        ApplicationDB.Remove(entry);
                    }
                    await ApplicationDB.SaveChangesAsync();
                }
            }

            var result = await userManager.DeleteAsync(user);
            var userId = await userManager.GetUserIdAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Unexpected error occurred deleting user.");
            }

            TempData["Message"] = $"Success, {userEmailSubmission} was deleted";
            return View();
        }
    }

    [Authorize(Roles="Administrators")]
    [HttpPost]
    public async Task<IActionResult> SpecificUserDownload(string argFileHash, string argFileName, string argUserName)
    {
        string userIDHash = ct.GetUserIDHash(argUserName);
        string fileNameHash = argFileHash;
        string FileBayPathForUser = Combine(MainDirectory, $"FileBay/{userIDHash}/{fileNameHash}");

        //get file as bytes, save with user-friendly name parameter
        byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(FileBayPathForUser);
        return File(fileBytes, "application/force-download", argFileName); 
    }
}