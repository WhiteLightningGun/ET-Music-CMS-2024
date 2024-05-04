using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UpLoader_For_ET.DBModels;

namespace UpLoader_For_ET.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<UploadDBEntry>? UploadDBEntries { get; set; }
    public DbSet<FrontPageEntry>? FrontPageEntries { get; set; }
    public DbSet<MessageDBEntry>? MessageDBEntries { get; set; }
}
