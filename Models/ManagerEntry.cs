using UpLoader_For_ET.DBModels;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
namespace UpLoader_For_ET.Models;


public class ManagerPageEntry
{
    public string? userEmail { get; set; }
    public string? currentUserRole { get; set; }
}