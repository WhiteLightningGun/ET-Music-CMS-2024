namespace UpLoader_For_ET.Services;

public interface IContactFormVisibility
{
    public bool IsVisible { get; set; }
    public void Toggle(); 
}