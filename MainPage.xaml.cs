using System.Diagnostics;

namespace RollingHillsSafaris;

public partial class MainPage : ContentPage
{
    private const string CompanyWebsite = "https://www.rollinghillssafarisnam.com/";

    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnEnquireClicked(object sender, EventArgs e)
    {
        try
        {
            await Launcher.Default.OpenAsync(CompanyWebsite);
        }
        catch (Exception)
        {
            await DisplayAlert("Contact Rolling Hills Safaris",
                "Please visit rollinghillssafarisnam.com to contact the team.", "OK");
        }
    }
}