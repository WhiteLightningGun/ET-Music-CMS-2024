using System.ComponentModel.DataAnnotations;

namespace UpLoader_For_ET.DBModels;


public class FrontPageEntry
{
    [Required]
    public int id { get; set; } // internal DB id, required by default, increments automatically
    [Required]
    public string? title { get; set; } 
    [Required]
    public string? description { get; set; }
    [Required]
    public string? htmlEmbedLink { get; set; }
}