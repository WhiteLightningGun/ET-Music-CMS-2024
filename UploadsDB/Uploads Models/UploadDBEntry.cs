using System.ComponentModel.DataAnnotations;

namespace UpLoader_For_ET.DBModels;

public class UploadDBEntry
{
    [Required]
    public int id { get; set; } // internal DB id, required by default, increments automatically
    [Required]
    public string? userHash { get; set; } // the hash code associated with every user identity (see app.AspNetUsers.id in db), will also name the user folder in FileBay
    [Required]
    [StringLength(28)]
    public string? userFileTitle { get; set; } // a user friendly file title set by the uploader, will be derived from the uploadedFile name i.e. Asteroiders_Hostile_3.ogg = "Asteroiders_Hostile_3"
    [StringLength(100)]
    public string? userDescription { get; set; } // The user has the option of appending a brief comment to their upload
    [Required]
    public string? fileHash { get; set; } // name of the file after it has been appended a hash
}