using UpLoader_For_ET.DBModels;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace UpLoader_For_ET.Models;

public class AssignTrackModel
{
    public List<string?>? listOfUsers { get; set; }

    [BindProperty]
    [DisplayName("Your file:")]
    [Required(ErrorMessage = "Please select a file.")]
    public IFormFile? Upload { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Include comment of minimum 3 letters..", AllowEmptyStrings = false)]
    [DisplayName("Comment: ")]
    [StringLength(100, MinimumLength = 3)]
    public string? userDescription { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Don't forget to select a user.", AllowEmptyStrings = false)]
    [DisplayName("Selected User")]
    public string? selectedUser { get; set; }

    public bool HasErrors { get; set; }
    public List<string>? ValidationErrors { get; set; }

    /* CONSTRUCTOR SWITCHED OFF, FOR THE MOMENT...
    public AssignTrackModel(bool hasErrors, List<string> validationErrors, string userdescription = "")
    {
        HasErrors = hasErrors;
        ValidationErrors = validationErrors;
        userDescription = userdescription;
    }
    */

}
