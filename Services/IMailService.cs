using UpLoader_For_ET.Models;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace UpLoader_For_ET.Services
{
    /// <summary>
    /// Uses the IEmailSender interface to create another interface called IMailService
    /// </summary>
    public interface IMailService : IEmailSender
    {
        Task<bool> SendAsync(MailData mailData, CancellationToken ct);
    }
}