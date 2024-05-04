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


using Microsoft.Extensions.Options;

using UpLoader_For_ET.StaticClasses;

namespace UpLoader_For_ET.Controllers;

public partial class HomeController : Controller
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext ApplicationDB;
    public UserSpaceLimitSetting userSpaceLimit;
    private readonly string MainDirectory = Environment.CurrentDirectory;
    
    public HomeController(ILogger<HomeController> logger, 
    ApplicationDbContext injectedADB, 
    IWebHostEnvironment env,
    IOptions<UserSpaceLimitSetting> _userSpaceLimit)
    {
        _logger = logger;
        ApplicationDB = injectedADB;
        _env = env;
        userSpaceLimit = _userSpaceLimit.Value;
    }

    public IActionResult Index()
    {
        List<FrontPageEntry>? entries = ApplicationDB.FrontPageEntries?.Select(x => x).ToList();

        if(entries is not null)
        {
            IndexPageModel indexpagemodel = new(entries);

            return View(indexpagemodel);
        }

        return View();
    }
    
    public IActionResult Privacy()
    {

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

}
