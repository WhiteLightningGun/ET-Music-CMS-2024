using UpLoader_For_ET.DBModels;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
namespace UpLoader_For_ET.Models;


public class ManagerPageModel
{   
    [BindProperty]
    public List<string>? interestingRoles { get; set; }
    public List<ManagerPageEntry>? managerEntries { get; set; }
    public string? RoleSelectionGet { get; set; }
    public bool contactFormVisibility { get; set; }
}