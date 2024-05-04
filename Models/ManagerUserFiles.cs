using UpLoader_For_ET.DBModels;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
namespace UpLoader_For_ET.Models;


public class ManageUserFiles
{   
    public List<UploadDBEntry>? uploadDBEntries {get; set;}
    public string? selectedUserName { get; set; }

}