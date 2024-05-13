using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UpLoader_For_ET.Data;

namespace UpLoader_For_ET.Controllers;

public partial class RolesController : Controller
{
    private readonly string AdminRole = "Administrators";
    private readonly string UserRole = "User";
    private readonly string PlebianRole = "Plebian";
    private readonly RoleManager<IdentityRole> roleManager;
    private readonly ApplicationDbContext ApplicationDB;

    //CONSTRUCTOR
    public RolesController(RoleManager<IdentityRole> roleManager,
    ApplicationDbContext injectedADB)
    {
        this.roleManager = roleManager;
        ApplicationDB = injectedADB;
    }

    public async Task<IActionResult> Index()
    {
        if (!(await roleManager.RoleExistsAsync(AdminRole)))
        {
            await roleManager.CreateAsync(new IdentityRole(AdminRole));
        }

        if (!(await roleManager.RoleExistsAsync(UserRole)))
        {
            await roleManager.CreateAsync(new IdentityRole(UserRole));
        }

        if (!(await roleManager.RoleExistsAsync(PlebianRole)))
        {
            await roleManager.CreateAsync(new IdentityRole(PlebianRole));
        }

        return Redirect("/"); // Back to homepage
    }

}