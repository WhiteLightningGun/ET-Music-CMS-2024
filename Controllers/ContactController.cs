using Microsoft.AspNetCore.Mvc;
using UpLoader_For_ET.Configuration;
using UpLoader_For_ET.Data;
using UpLoader_For_ET.Services;
using Microsoft.Extensions.Options;
using UpLoader_For_ET.DBModels;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authorization;
using UpLoader_For_ET.Models;

namespace UpLoader_For_ET.Controllers;

public class ContactController : Controller
{
    private readonly ContactFormVisibility contactFormControl;
    private readonly ApplicationDbContext ApplicationDB;
    private readonly MessageLimitSetting messageSettings;
    private readonly IEmailSender emailSender;

    public ContactController(ContactFormVisibility contactFormVisibility, 
    ApplicationDbContext injectedADB, 
    IOptions<MessageLimitSetting> _messageSettings, 
    IEmailSender _emailSender)
    {
        contactFormControl = contactFormVisibility;
        ApplicationDB = injectedADB;
        messageSettings = _messageSettings.Value;
        emailSender = _emailSender;
    }

    public IActionResult MessageMe()
    {
        if(!contactFormControl.IsVisible)
        {
            return Redirect("~/Contact/ContactFormClosed");
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> MessageMe(string userEmailSubmission, string userMessageSubmission, bool Consent)
    {  
        int MessageCount = ApplicationDB.MessageDBEntries?.Count() ?? 0;       
        
        if(!contactFormControl.IsVisible || MessageCount >= messageSettings.MaxMessages)
        {
            contactFormControl.SetToOff();
            return Redirect("~/Contact/ContactFormClosed");
        }

        if(ModelState.IsValid && Consent == true)
        {
            MessageDBEntry messageDBEntry = new()
            {
                Email = userEmailSubmission,
                Message = userMessageSubmission,
                TimeArrived = DateTime.Now
            };

            ApplicationDB.Add(messageDBEntry);
            await ApplicationDB.SaveChangesAsync();

            string? callbackUrl = Url.ActionLink("About", "Home", null); 

            if(callbackUrl is not null)
            {
                await emailSender.SendEmailAsync("admin@electrictrojan.com", "Alert! New message.", $"You have a new message, read it now: <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>{callbackUrl}</a>");
            }
            
            ModelState.Clear();
            TempData["Alert"] = true;

            return View();
        }
        else
        {
            return View();
        }
    }
    public IActionResult ContactFormClosed()
    {
        return View();
    }

    [Authorize(Roles="Administrators")]
    public IActionResult ContactFormToggle()
    {
        contactFormControl.Toggle();

        ContactFormToggleView toggleView = new(contactFormControl.IsVisible);
        return View(toggleView);
    }

    [Authorize(Roles="Administrators")]
    public IActionResult MessageReviewPage()
    {
        MessageReviewModel messagesReview = new()
        {
            //Messages ordered by most recent
            Messages = ApplicationDB.MessageDBEntries?.Select(x => x).OrderByDescending(x => x.TimeArrived).ToList()
        };
        return View(messagesReview);
    }

    [HttpPost]
    [Authorize(Roles="Administrators")]
    public async Task<IActionResult> MessageReviewPage(int messageID)
    {
        Console.Write($"MessageReviewPage : {messageID}");
        MessageDBEntry? toDelete = ApplicationDB.MessageDBEntries?.Select(x => x).Where(x => x.id == messageID).FirstOrDefault();
        if(toDelete is not null)
        {
            ApplicationDB.Remove(toDelete);
            await ApplicationDB.SaveChangesAsync();
            TempData["Message"] = "Message Deleted, remember to re-check form visibility if required.";
        }
        else
        {
            TempData["Message"] = "Something went wrong.";
        }

        MessageReviewModel messagesReview = new()
        {
            Messages = ApplicationDB.MessageDBEntries?.Select(x => x).OrderByDescending(x => x.TimeArrived).ToList()
        };
        return View(messagesReview);
    }

}
 
