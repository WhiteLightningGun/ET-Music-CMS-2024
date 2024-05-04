namespace UpLoader_For_ET.Models;
using UpLoader_For_ET.DBModels;

public class IndexPageModel
{
    public List<FrontPageEntry> frontPageVideos { get; set; }

    public IndexPageModel(List<FrontPageEntry> frontpagevideos)
    {
        frontPageVideos = frontpagevideos;
    }

}
