namespace UpLoader_For_ET.Models;

public class DeleteAlertModel
{
    public bool success { get; set; }
    public string? fileNameDeleted { get; set; }

    public DeleteAlertModel(string filename)
    {
        fileNameDeleted = filename;
        success = true;
    }


}
