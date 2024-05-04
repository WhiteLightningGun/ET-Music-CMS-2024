using UpLoader_For_ET.DBModels;
using Microsoft.AspNetCore.Mvc;



namespace UpLoader_For_ET.Models;

public class UserDownloadModel
{
    //important fields
    public List<UploadDBEntry>? uploadDBEntries {get; set;}
}
