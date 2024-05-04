
using UpLoader_For_ET.DBModels;
using Microsoft.EntityFrameworkCore;

namespace UpLoader_For_ET;

public class UploadsDBContext : DbContext
{
    public UploadsDBContext(DbContextOptions options) :base(options)
    {
    }
    public DbSet<UploadDBEntry>? UploadDBEntries { get; set; }
    public DbSet<FrontPageEntry>? FrontPageEntries { get; set; }
    public DbSet<MessageDBEntry>? MessageDBEntries { get; set; }
}
