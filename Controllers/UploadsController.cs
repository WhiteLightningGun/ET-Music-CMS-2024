using Microsoft.AspNetCore.Mvc;
using UpLoader_For_ET.Models;
using Microsoft.AspNetCore.Authorization;
using UpLoader_For_ET.Data;
using UpLoader_For_ET.DBModels;
using static System.IO.Path;
using UpLoader_For_ET.Configuration;
using UpLoader_For_ET.Tools;


using Microsoft.Extensions.Options;

using UpLoader_For_ET.StaticClasses;

namespace UpLoader_For_ET.Controllers;

public class UploadsController : Controller
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext ApplicationDB;
    public UserSpaceLimitSetting userSpaceLimit;
    private readonly string MainDirectory = Environment.CurrentDirectory;
    public readonly ControllerTools ct;
    
    public UploadsController(ILogger<HomeController> logger, 
    ApplicationDbContext injectedADB, 
    IWebHostEnvironment env,
    IOptions<UserSpaceLimitSetting> _userSpaceLimit,
    ControllerTools _controllerTools)
    {
        _logger = logger;
        ApplicationDB = injectedADB;
        _env = env;
        userSpaceLimit = _userSpaceLimit.Value;
        ct = _controllerTools;
    }

    [Authorize(Roles="User,Administrators")]
    [HttpPost]
    [RequestSizeLimit(134212233)] // ~134 MB
    public async Task<IActionResult> UploadPage(IFormFile uploadedFile, string comment)
    {
        string userIDHash = ct.GetUserIDHash(User.Identity?.Name!);

        string FileBayPathForUser = Combine(MainDirectory, $"FileBay/{userIDHash}/");
        string? userComment = comment; // the original argument (string comment) is non-nullable to enforce mandatoryness

        if(ModelState.IsValid)
        {
            if(!ct.CheckSpaceForUser(User.Identity?.Name, uploadedFile.Length))
            {
                TempData["Alert"] = $"Upload blocked! {uploadedFile.FileName} is {(decimal)uploadedFile.Length/(1000*1000):0.0} megabytes - too big! ";
                return View();
            }
            using var fileStream = uploadedFile.OpenReadStream();
            byte[] eightByteHeader = new byte[8];
            fileStream.Read(eightByteHeader, 0, 8);

            if (UploadCheck.CheckFormat(eightByteHeader) != true)
            {
                List<string> errorList = ModelState.Values.SelectMany(state => state.Errors).Select(error => error.ErrorMessage).ToList();
                errorList.Add("Wrong file format, permitted formats are OGG vorbis and MP3 with ID3v2 tags");
                UploadModel uploadModelWithErrors = new(hasErrors: true, validationErrors: errorList);
                return View(uploadModelWithErrors);
            }
            string extension = Path.GetExtension(uploadedFile.FileName);
            string fileHashForNewFile = Guid.NewGuid().ToString() + extension;
            
            Directory.CreateDirectory(FileBayPathForUser);
            string savePath = Combine(FileBayPathForUser, $"{fileHashForNewFile}");
            
            using (var newFile = System.IO.File.Create(savePath))
            {
                await uploadedFile.CopyToAsync(newFile);
            }

            //Add file name and location information to DB
            UploadDBEntry newDBEntry = new UploadDBEntry
            {
                userHash = userIDHash,
                userDescription = userComment,
                userFileTitle = uploadedFile.FileName,
                fileHash = fileHashForNewFile
            };
            await ApplicationDB.AddAsync(newDBEntry);
            await ApplicationDB.SaveChangesAsync();
        }
        else
        {
            List<string> errorList = ModelState.Values.SelectMany(state => state.Errors).Select(error => error.ErrorMessage).ToList();
            UploadModel uploadModelWithErrors = new(hasErrors: true, validationErrors: errorList);
            TempData["Alert"] = "Upload not successful.";
            return View(uploadModelWithErrors);
        }

        TempData["Message"] = "Upload Successful.";
        return View();
    }

    [Authorize(Roles="User,Administrators")]
    public IActionResult UploadPage()
    {
        TempData["Report"] = $"Hello {User.Identity?.Name}, you have used {ct.GetSpaceForUser(User.Identity?.Name!):0.00} megabytes so far. Maximum {userSpaceLimit.userLimitMB} megabytes.";
        return View();
    }
}