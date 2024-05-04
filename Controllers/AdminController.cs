using Microsoft.AspNetCore.Identity; //Role manager, user manager
using Microsoft.AspNetCore.Mvc; // Controller, IAction Result
using Microsoft.AspNetCore.Authorization; //Authorisations
using UpLoader_For_ET.Models;
using UpLoader_For_ET.Data;
using static System.Console;
using static System.IO.Path;
using UpLoader_For_ET.DBModels;
using UpLoader_For_ET.Services;
using UpLoader_For_ET.Tools;
using UpLoader_For_ET.StaticClasses;

namespace UpLoader_For_ET.Controllers;

public partial class AdminController : Controller
{
    private readonly RoleManager<IdentityRole> roleManager;
    private readonly UserManager<IdentityUser> userManager;
    private readonly ApplicationDbContext ApplicationDB;
    private readonly string MainDirectory = Environment.CurrentDirectory;
    private readonly ControllerTools ct;
    private ContactFormVisibility contactFormControl;

    //CONSTRUCTOR
    public AdminController(RoleManager<IdentityRole> roleManager,
    UserManager<IdentityUser> userManager, ApplicationDbContext injectedADB,
    ContactFormVisibility contactFormVisibility,
    ControllerTools _controllerTools)
    {
        this.roleManager = roleManager;
        this.userManager = userManager;
        ApplicationDB = injectedADB;
        contactFormControl = contactFormVisibility;
        ct = _controllerTools;
    }
    [Authorize(Roles="Administrators")]
    public IActionResult Manager()
    {
        List<string?>? emailList = ApplicationDB.Users.Select(x => x.Email).ToList();
        List<IdentityRole> availableRoles = roleManager.Roles.ToList();
        ManagerPageModel managerPage = new();
        List<ManagerPageEntry> managerpageEntries = new();

        foreach(string? email in emailList)
        {
            ManagerPageEntry newManagerEntry = new()
            {
                userEmail = email
            };
            // Get unique user ID associated with email
            string? uniqueID = ApplicationDB.Users.Where(c => c.UserName == email).Select(c => c.Id).First();

            // user role lookup, if it exists
            string? userRoleID = ApplicationDB.UserRoles.Where(x => x.UserId == uniqueID).Select(x => x.RoleId).FirstOrDefault();


            if(userRoleID != null)
            {
                string? roleName = ApplicationDB.Roles.Where(x => x.Id == userRoleID).Select(x => x.Name).FirstOrDefault();
                newManagerEntry.currentUserRole = roleName;
                //Console.WriteLine($"{email} has role {roleName}");
            }
            else
            {
                newManagerEntry.currentUserRole = "Not Set";
            }

            managerpageEntries.Add(newManagerEntry);
        }

        managerPage.managerEntries = managerpageEntries;
        
        List<string> rolesList = new();

        foreach(IdentityRole role in availableRoles)
        {
            if (role.Name != null)
            {
                rolesList.Add(role.Name);
            }
        }
        managerPage.interestingRoles = rolesList;
        managerPage.contactFormVisibility = contactFormControl.IsVisible;
        return View(managerPage);
    }

    [Authorize(Roles = "Administrators")]
    [HttpPost]
    public async Task<IActionResult> Manager(string userEmailSubmission, string selectedRole, string currentRole)
    {
        List<string?> allRolesList = roleManager.Roles.Select(x => x.Name).ToList();
        IdentityUser? userHere = await userManager.FindByEmailAsync(userEmailSubmission);
        
        if (userHere != null)
        {
            IdentityResult removedFromRole = await userManager.RemoveFromRoleAsync(userHere, currentRole);
            IdentityResult result = await userManager.AddToRoleAsync(userHere, selectedRole);
            await userManager.UpdateSecurityStampAsync(userHere);
        }
        else
        {
            WriteLine($"Failed to change role associated with {userEmailSubmission}");
        }
        return Manager();
    }

    [Authorize(Roles = "Administrators")]
    [HttpPost]
    public IActionResult ManageUserFiles(string userEmailSubmission, string selectedRole, string currentRole)
    {
        var userIDHash = ct.GetUserIDHash(userEmailSubmission);
        ManageUserFiles manageUserFiles = new()
        {
            uploadDBEntries = ApplicationDB.UploadDBEntries?.Select(x => x).Where(x => x.userHash == userIDHash).ToList(),
            selectedUserName = userEmailSubmission
        };
        return View(manageUserFiles);
    }

    [Authorize(Roles = "Administrators")]
    public IActionResult ManageUserFiles()
    {
        return Redirect("~/Admin/Manager");
    }

    [Authorize(Roles="User,Administrators")]
    [HttpPost]
    public async Task<IActionResult> DeleteAlert(string? argFileHashD, string argFileNameD, string argUserName)
    {
        // if argUserName == null infer from http context
        argUserName ??= User.Identity!.Name!;
        string userIDHash = ct.GetUserIDHash(argUserName);

        UploadDBEntry? toDelete = ApplicationDB.UploadDBEntries?.Select(x => x).Where(x => x.fileHash == argFileHashD).FirstOrDefault();

        string FileBayPathForUser = Combine(MainDirectory, $"FileBay/{userIDHash}/{argFileHashD}");

        if(toDelete != null)
        {
            ApplicationDB.UploadDBEntries?.Remove(toDelete);
            await ApplicationDB.SaveChangesAsync();
        }

        if (System.IO.File.Exists(FileBayPathForUser)) 
        { 
            System.IO.File.Delete(FileBayPathForUser);
        }

        DeleteAlertModel deletealertmodel = new (argFileNameD);

        return View(deletealertmodel);
    }

    [Authorize(Roles="Administrators")]
    public IActionResult AssignTrack()
    {
        AssignTrackModel assignTrackModel = new()
        {
            listOfUsers = ApplicationDB.Users.Select(x => x.Email).ToList()
        };

        return View(assignTrackModel);
    }

    [Authorize(Roles="Administrators")]
    [HttpPost]
    public async Task<IActionResult> AssignTrack(IFormFile uploadedFile, string comment, string selecteduser)
    {
        AssignTrackModel assignTrackModel = new()
        {
            listOfUsers = ApplicationDB.Users.Select(x => x.Email).ToList()
        };

        if(!ct.CheckSpaceForUser(selecteduser, uploadedFile.Length))
        {
            TempData["Alert"] = $"Upload blocked! {uploadedFile.FileName} is {(decimal)uploadedFile.Length/(1000*1000):0.0} megabytes - too big! {selecteduser} has used {ct.GetSpaceForUser(selecteduser):0.00} mb. Maximum limit {ct.userSpaceLimit.userLimitMB}";
            return View(assignTrackModel);
        }

        string userIDHash = ct.GetUserIDHash(selecteduser);

        string FileBayPathForUser = Combine(MainDirectory, $"FileBay/{userIDHash}/");
        string? userComment = comment;

        //For monitoring data usage it will be necessary to create a record of the amount of space used by the user and measuring folder usage

        if(ModelState.IsValid)
        {
            Console.WriteLine("Valid ModelState.");

            using var fileStream = uploadedFile.OpenReadStream();
            byte[] eightByteHeader = new byte[8];
            fileStream.Read(eightByteHeader, 0, 8);

            if (UploadCheck.CheckFormat(eightByteHeader) != true)
            {
                //Consider banning user for uploading deliberately misnamed files. 
                List<string> errorList = ModelState.Values.SelectMany(state => state.Errors).Select(error => error.ErrorMessage).ToList();
                errorList.Add("Wrong file format, permitted formats are OGG vorbis and MP3 with ID3v2 tags");

                AssignTrackModel uploadModelWithErrors = new()
                {
                    HasErrors = true,
                    ValidationErrors = errorList,
                    listOfUsers = ApplicationDB.Users.Select(x => x.Email).ToList()
                };

                return View(uploadModelWithErrors);
            }

            // for now, we assume successful virus scan takes place here

            string extension = System.IO.Path.GetExtension(uploadedFile.FileName);
            string fileHashForNewFile = Guid.NewGuid().ToString() + extension;
            string originalFileName = uploadedFile.FileName;
            
            Directory.CreateDirectory(FileBayPathForUser);
            string savePath = Combine(FileBayPathForUser, $"{fileHashForNewFile}"); //needs correct file extension
            
            using (var newFile = System.IO.File.Create(savePath))
            {
                uploadedFile.CopyTo(newFile);
            }

            //Add file name and location information to DB
            UploadDBEntry newDBEntry = new UploadDBEntry
            {
                userHash = userIDHash,
                userDescription = userComment,
                userFileTitle = originalFileName,
                fileHash = fileHashForNewFile
            };
            await ApplicationDB.AddAsync(newDBEntry);
            await ApplicationDB.SaveChangesAsync();
        }
        else
        {
            List<string> errorList = ModelState.Values.SelectMany(state => state.Errors).Select(error => error.ErrorMessage).ToList();
            AssignTrackModel uploadModelWithErrors = new()
            {
                HasErrors = true,
                ValidationErrors = errorList,
                listOfUsers = ApplicationDB.Users.Select(x => x.Email).ToList()
            };
            return View(uploadModelWithErrors);
        }

        TempData["Message"] = $"It worked... {selecteduser} has used {ct.GetSpaceForUser(selecteduser):0.00} mb so far.";
        return View(assignTrackModel);
    }

        [Authorize(Roles="Administrators")]
    public IActionResult FrontPageReview()
    {
        FrontPageReviewModel frontPageEditModel = new();
        List<FrontPageEntry>? frontPageEntries = ApplicationDB.FrontPageEntries?.Select(x => x).ToList();
        frontPageEditModel.FrontPageEntries = frontPageEntries;

        return View(frontPageEditModel);
    }

    [Authorize(Roles = "Administrators")]
    public IActionResult FrontPageAdd(int id)
    {
        if(id <= 0) //The case where no useful argument is given in the url, this can include strings which ASP.Net will resolve to 0 if they do not parse to an int
        {
            return View();
        }

        FrontPageAddPost frontpageAdd = new();
        frontpageAdd.selectedFrontPageVideo = ApplicationDB.FrontPageEntries?.Select(x => x).Where(x => x.id == id).FirstOrDefault();
        
        if(frontpageAdd.selectedFrontPageVideo == null)
        {
            TempData["Message"] = "Video not found";
            return View();
        }

        TempData["Message"] = $"FrontPageAdd(int id = {id}) was used";
    
        return View(frontpageAdd);
    }

    [Authorize(Roles = "Administrators")]
    [HttpPost]
    public async Task<IActionResult> FrontPageAdd(string Title, string userDescription, string htmlEmbedURL, string? id)
    {
        if(ModelState.IsValid && id == null) // the case where we are adding a new video
        {
            //Create new entry in db

            FrontPageEntry latestFrontPageEntry = new()
            {
                description = userDescription,
                title = Title,
                htmlEmbedLink = htmlEmbedURL
            };

            ApplicationDB.FrontPageEntries?.Add(latestFrontPageEntry);
            await ApplicationDB.SaveChangesAsync();

            ModelState.Clear();
            TempData["Message"] = "Post Successful";
            return View();
        }

        if(ModelState.IsValid && id != null) // the edit existing entry case
        {
            int ID = Convert.ToInt32(id);

            FrontPageEntry? result = ApplicationDB.FrontPageEntries?.FirstOrDefault(x => x.id == ID);
            if(result != null)
            {
                result.description = userDescription;
                result.title = Title;
                result.htmlEmbedLink = htmlEmbedURL;
                await ApplicationDB.SaveChangesAsync();
                
                ModelState.Clear();
                TempData["Message"] = "Edit Successful";
                return FrontPageAdd(0); // The case where frontpageadd takes zero sends you to the add entry page
            }
            else
            {
                ModelState.Clear();
                TempData["Message"] = "id arg was not found, something has gone wrong.";
                return View();
            }
        }
        TempData["Message"] = "Front Page POST Used";
        return View();
    }
    
}