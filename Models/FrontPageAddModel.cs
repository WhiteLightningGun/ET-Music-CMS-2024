using UpLoader_For_ET.DBModels;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
namespace UpLoader_For_ET.Models;


public class FrontPageAddPost
{
    [BindProperty]
    [DisplayName("Title: ")]
    [Required(ErrorMessage = "Title must be of at least 1 letter and at most 30 letters in length.")]
    [StringLength(30, MinimumLength = 1)]
    public string Title { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Include comment of minimum 3 letters, max 90 letters..", AllowEmptyStrings = false)]
    [DisplayName("Description: ")]
    [StringLength(90, MinimumLength = 3)]
    public string userDescription { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Include comment of minimum 3 letters..", AllowEmptyStrings = false)]
    [DisplayName("html embed url: ")]
    [StringLength(100, MinimumLength = 5)]
    public string htmlEmbedURL { get; set; }

    //public bool HasErrors { get; set; }
    //public List<string>? ValidationErrors { get; set; }

    public FrontPageEntry? selectedFrontPageVideo { get; set; }
    public FrontPageAddPost( 
    string userdescription = "placeholder", 
    string title = "placeholder Title",
    string htmlembedurl = "placeholder url")
    {
        //HasErrors = hasErrors;
        //ValidationErrors = validationErrors;
        userDescription = userdescription;
        Title = title;
        htmlEmbedURL = htmlembedurl;
    }

}