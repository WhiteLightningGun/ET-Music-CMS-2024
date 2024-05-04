using UpLoader_For_ET.DBModels;
namespace UpLoader_For_ET.Models;

public class UserSummaryModel
{
    public string? userName { get; set; }
    public decimal? percentageUsed { get; set; }
    public List<UploadDBEntry>? uploadDBEntries {get; set;}
    public decimal spaceUsedMB { get; set; }
    public decimal? spaceRemaining { get; set; }

}
