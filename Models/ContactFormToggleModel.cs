namespace UpLoader_For_ET.Models;


public class ContactFormToggleView
{

    public bool contactFormVisibility { get; set; }

    public ContactFormToggleView(bool currentSetting)
    {
        contactFormVisibility = currentSetting;
    }

}
