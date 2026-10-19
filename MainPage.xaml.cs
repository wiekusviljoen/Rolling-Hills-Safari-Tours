using System.Net.Mail;

namespace RollingHillsSafaris;

public partial class MainPage : ContentPage
{
    private const string CompanyWebsite = "https://www.rollinghillssafarisnam.com/";
    private const string CompanyEmail = "rollinghillssafaris@gmail.com";
    private const string CompanyPhoneDigits = "264812694550";

    public MainPage()
    {
        InitializeComponent();
        TripDatePicker.MinimumDate = DateTime.Today;
        TripDatePicker.Date = DateTime.Today.AddDays(1);
        AdultsPicker.SelectedIndex = 1;
        ChildrenPicker.SelectedIndex = 0;
        BookingTypePicker.SelectedIndex = 2;
        PaymentPlanPicker.SelectedIndex = 0;
        CurrencyPicker.SelectedIndex = 5;
    }

    private async Task ScrollToSectionAsync(VisualElement section)
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await PageScrollView.ScrollToAsync(section, ScrollToPosition.Start, true);
        });
    }

    private async void OnGoToBooking(object sender, EventArgs e)
    {
        await ScrollToSectionAsync(BookingSection);
    }

    private async void OnGoToTours(object sender, EventArgs e)
    {
        await ScrollToSectionAsync(ToursSection);
    }

    private async void OnSandwichClicked(object sender, EventArgs e)
    {
        TourPicker.SelectedIndex = 0;
        await ScrollToSectionAsync(BookingSection);
    }

    private async void OnBirdingClicked(object sender, EventArgs e)
    {
        TourPicker.SelectedIndex = 2;
        await ScrollToSectionAsync(BookingSection);
    }

    private async void OnCustomClicked(object sender, EventArgs e)
    {
        TourPicker.SelectedIndex = 4;
        await ScrollToSectionAsync(BookingSection);
    }

    private async void OnCheckoutClicked(object sender, EventArgs e)
    {
        if (TourPicker.SelectedIndex < 0 || string.IsNullOrWhiteSpace(NameEntry.Text) || string.IsNullOrWhiteSpace(EmailEntry.Text))
        {
            await DisplayAlert("Complete your booking details", "Please select a tour and enter your name and email address before continuing to checkout.", "OK");
            return;
        }
        try { _ = new MailAddress(EmailEntry.Text.Trim()); }
        catch (FormatException) { await DisplayAlert("Check your email", "Please enter a valid email address before continuing.", "OK"); return; }
        var selectedTour = TourPicker.SelectedItem?.ToString() ?? string.Empty;
        var adults = AdultsPicker.SelectedItem?.ToString() ?? "Not selected";
        if (selectedTour.StartsWith("Sandwich Harbour", StringComparison.OrdinalIgnoreCase) && adults == "1")
        {
            await DisplayAlert("Minimum group size", "The published Sandwich Harbour rates require at least 2 adults. Contact us if you are a solo traveller and would like to join an existing group.", "OK");
            return;
        }
        var checkout = new CheckoutPage(selectedTour, TripDatePicker.Date.GetValueOrDefault(), adults, ChildrenPicker.SelectedItem?.ToString() ?? "0", BookingTypePicker.SelectedItem?.ToString() ?? "Please advise me", PaymentPlanPicker.SelectedItem?.ToString() ?? "Pay in full", CurrencyPicker.SelectedItem?.ToString() ?? "Let me choose at checkout (if available)", NameEntry.Text.Trim(), EmailEntry.Text.Trim(), PhoneEntry.Text?.Trim() ?? string.Empty, NotesEditor.Text?.Trim() ?? string.Empty);
        await Navigation.PushAsync(checkout);
    }
    private string BuildEnquiry()
    {
        var adults = AdultsPicker.SelectedItem?.ToString() ?? "Not selected";
        var children = ChildrenPicker.SelectedItem?.ToString() ?? "Not selected";
        var tour = TourPicker.SelectedItem?.ToString() ?? "Not selected";
        var bookingType = BookingTypePicker.SelectedItem?.ToString() ?? "Please advise me";
        var paymentPlan = PaymentPlanPicker.SelectedItem?.ToString() ?? "Not selected";
        var currency = CurrencyPicker.SelectedItem?.ToString() ?? "Let me choose at checkout (if available)";
        return $"Hello Rolling Hills Safaris,\n\nI'd like to enquire about a safari booking.\n\n" +
               $"Experience: {tour}\nPreferred date: {TripDatePicker.Date:ddd, d MMM yyyy}\n" +
               $"Adults: {adults}\nChildren: {children}\nPreference: {bookingType}\nPayment preference: {paymentPlan}\nPreferred checkout currency: {currency}\n\n" +
               $"Name: {NameEntry.Text?.Trim()}\nEmail: {EmailEntry.Text?.Trim()}\n" +
               $"Phone / WhatsApp: {PhoneEntry.Text?.Trim()}\n\n" +
               $"Additional notes: {NotesEditor.Text?.Trim()}\n\n" +
               "Please confirm availability, final pricing and the next steps. Thank you.";
    }

    private async void OnSubmitBooking(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameEntry.Text) || string.IsNullOrWhiteSpace(EmailEntry.Text))
        {
            await DisplayAlert("A little more information", "Please enter your name and email address so the team can reply.", "OK");
            return;
        }

        try
        {
            _ = new MailAddress(EmailEntry.Text.Trim());
        }
        catch (FormatException)
        {
            await DisplayAlert("Check your email", "Please enter a valid email address.", "OK");
            return;
        }

        if (TripDatePicker.Date.GetValueOrDefault().Date < DateTime.Today)
        {
            await DisplayAlert("Check your travel date", "Please choose today or a future date.", "OK");
            return;
        }

        var selectedTour = TourPicker.SelectedItem?.ToString() ?? string.Empty;
        if (selectedTour.StartsWith("Sandwich Harbour", StringComparison.OrdinalIgnoreCase) &&
            AdultsPicker.SelectedItem?.ToString() == "1")
        {
            await DisplayAlert("Minimum group size", "The published Sandwich Harbour rates require at least 2 adults. Contact us if you are a solo traveller and would like to join an existing group.", "OK");
            return;
        }

        var subject = Uri.EscapeDataString("Safari booking enquiry - Rolling Hills Safaris");
        var body = Uri.EscapeDataString(BuildEnquiry());
        var mailto = new Uri($"mailto:{CompanyEmail}?subject={subject}&body={body}");
        try
        {
            await Launcher.Default.OpenAsync(mailto);
            await DisplayAlert("Almost there", "Your email app has opened with the enquiry prepared. Please review it and press Send. This is not yet a confirmed booking.", "OK");
        }
        catch
        {
            await DisplayAlert("Contact the team", $"Please email {CompanyEmail} with your preferred tour and date.", "OK");
        }
    }

    private async void OnWhatsAppClicked(object sender, EventArgs e)
    {
        var message = Uri.EscapeDataString(BuildEnquiry());
        var uri = new Uri($"https://wa.me/{CompanyPhoneDigits}?text={message}");
        try { await Launcher.Default.OpenAsync(uri); }
        catch { await DisplayAlert("WhatsApp", $"Please contact us on +264 81 269 4550 or {CompanyEmail}.", "OK"); }
    }

    private async void OnWebsiteClicked(object sender, EventArgs e)
    {
        try { await Launcher.Default.OpenAsync(new Uri(CompanyWebsite)); }
        catch { await DisplayAlert("Website", CompanyWebsite, "OK"); }
    }
}