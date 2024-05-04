using System.ComponentModel.DataAnnotations;

namespace UpLoader_For_ET.DBModels;


public class MessageDBEntry
{
    [Required]
    public int id { get; set; } // internal DB id, required by default, increments automatically

    [Required]
    [EmailAddress]
    public string? Email { get; set; } 

    [Required]
    [StringLength(200)]
    public string? Message { get; set; }

    [Required]
    public DateTime TimeArrived { get; set; }
}