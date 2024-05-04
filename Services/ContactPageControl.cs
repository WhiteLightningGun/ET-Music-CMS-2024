namespace UpLoader_For_ET.Services;

/// <summary>
/// A publicly available bool which can be used to control the functionality of the contact form, injected as Singleton service
/// </summary>
public class ContactFormVisibility : IContactFormVisibility
{
    public bool IsVisible { get; set; }
    public int SwitchCount { get; set; }

    public ContactFormVisibility()
    {
        IsVisible = true;
        SwitchCount = 0;
    }

    /// <summary>
    /// Toggles the ContactFormVisibility.IsVisible bool, which is used to control Contact page visibility
    /// </summary>
    public void Toggle()
    {
        if(IsVisible)
        {
            IsVisible = false;
        }
        else
        {
            IsVisible = true;
        }

        SwitchCount += 1;
    }

    /// <summary>
    /// Ensures visibility is set to off, does not toggle on/off.
    /// </summary>
    public void SetToOff()
    {
        if(IsVisible != false)
        {
            IsVisible = false;
            SwitchCount += 1;
        }

    }

}