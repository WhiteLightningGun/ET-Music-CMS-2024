using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

using Microsoft.AspNetCore.Mvc.RazorPages;
namespace UpLoader_For_ET.Models;


public class ContactForm
{
    [BindProperty]
    [Required(ErrorMessage = "Please include an email address.", AllowEmptyStrings = false)]
    [EmailAddress]
    [Display(Name = "Your email")]
    public string userEmailSubmission { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Message must be of minimum 5 letters.", AllowEmptyStrings = false)]
    [DisplayName("Comment: ")]
    [StringLength(200, MinimumLength = 5)]
    public string userMessageSubmission { get; set; }

    [BindProperty, Required(ErrorMessage = "Please select yes to allow submission.")]
    public bool Consent { get; set; }

    public ContactForm(string emailsubmission, string messagesubmission)
    {
        userEmailSubmission = emailsubmission;
        userMessageSubmission = messagesubmission;
    }

}
