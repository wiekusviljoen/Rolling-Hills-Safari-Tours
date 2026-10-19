using System.Globalization;
using System.Net.Mail;

namespace RollingHillsSafaris;

public partial class CheckoutPage : ContentPage
{
    private const string CompanyEmail = "rollinghillssafaris@gmail.com";
    private readonly string _tour;
    private readonly DateTime _travelDate;
    private readonly string _adults;
    private readonly string _children;
    private readonly string _bookingType;
    private readonly string _name;
    private readonly string _email;
    private readonly string _phone;
    private readonly string _notes;
    private readonly decimal? _knownTotal;

    public CheckoutPage(string tour, DateTime travelDate, string adults, string children,
        string bookingType, string paymentPlan, string currency, string name, string email,
        string phone, string notes)
    {
        InitializeComponent();
        _tour = tour;
        _travelDate = travelDate;
        _adults = adults;
        _children = children;
        _bookingType = bookingType;
        _name = name;
        _email = email;
        _phone = phone;
        _notes = notes;

        TourValueLabel.Text = string.IsNullOrWhiteSpace(tour) ? "Tour to be selected" : tour;
        DateValueLabel.Text = travelDate.ToString("ddd, d MMM yyyy", CultureInfo.InvariantCulture);
        GuestsValueLabel.Text = $"{adults} adult(s), {children} child(ren)";
        BookingTypeValueLabel.Text = bookingType;
        NameValueLabel.Text = name;

        PaymentPlanPicker.SelectedIndex = paymentPlan.Contains("deposit", StringComparison.OrdinalIgnoreCase) ? 1 : 0;
        CurrencyPicker.SelectedIndex = CurrencyIndex(currency);
        _knownTotal = TryCalculatePublishedTotal(tour, adults, children);
        UpdatePriceSummary();
    }

    private static int CurrencyIndex(string currency)
    {
        if (currency.Contains("NAD", StringComparison.OrdinalIgnoreCase)) return 0;
        if (currency.Contains("ZAR", StringComparison.OrdinalIgnoreCase)) return 1;
        if (currency.Contains("USD", StringComparison.OrdinalIgnoreCase)) return 2;
        if (currency.Contains("EUR", StringComparison.OrdinalIgnoreCase)) return 3;
        if (currency.Contains("GBP", StringComparison.OrdinalIgnoreCase)) return 4;
        return 5;
    }

    private static decimal? TryCalculatePublishedTotal(string tour, string adults, string children)
    {
        // Only published Sandwich Harbour rates are known in this app. Never guess other tour prices.
        if (!tour.StartsWith("Sandwich Harbour", StringComparison.OrdinalIgnoreCase)) return null;
        if (!int.TryParse(adults, out var adultCount) || !int.TryParse(children, out var childCount)) return null;
        var isHalfDay = tour.Contains("Half Day", StringComparison.OrdinalIgnoreCase);
        var adultRate = isHalfDay ? 2800m : 3800m;
        var childRate = isHalfDay ? 1400m : 1900m;
        return adultCount * adultRate + childCount * childRate;
    }

    private void UpdatePriceSummary()
    {
        var currency = CurrencyPicker.SelectedItem?.ToString() ?? "Let me choose at checkout (if available)";
        var isNad = currency.Contains("NAD", StringComparison.OrdinalIgnoreCase) || currency.Contains("choose", StringComparison.OrdinalIgnoreCase);
        var currencyCode = currency.Contains("ZAR", StringComparison.OrdinalIgnoreCase) ? "ZAR" :
            currency.Contains("USD", StringComparison.OrdinalIgnoreCase) ? "USD" :
            currency.Contains("EUR", StringComparison.OrdinalIgnoreCase) ? "EUR" :
            currency.Contains("GBP", StringComparison.OrdinalIgnoreCase) ? "GBP" : "NAD";

        if (_knownTotal is null)
        {
            TotalValueLabel.Text = "Quote required";
            DueValueLabel.Text = "To be confirmed";
            BalanceValueLabel.Text = "To be confirmed";
            PriceNoteLabel.Text = "A published price is not available in the app for this selection. The team will confirm your quote before requesting payment.";
            return;
        }

        // Published rates are in Namibian dollars. Do not display them as other currencies without an approved live FX rate.
        TotalValueLabel.Text = _knownTotal.Value.ToString("C0", CultureInfo.GetCultureInfo("en-NA")) + " NAD";
        if (!isNad)
        {
            DueValueLabel.Text = "Quote in selected currency";
            BalanceValueLabel.Text = "To be confirmed";
            PriceNoteLabel.Text = $"Published rate is in NAD. A {currencyCode} amount will be confirmed by the team; no exchange rate is guessed in the app.";
            return;
        }

        var depositSelected = PaymentPlanPicker.SelectedIndex == 1;
        DueValueLabel.Text = depositSelected ? "Deposit amount to be agreed" : _knownTotal.Value.ToString("C0", CultureInfo.GetCultureInfo("en-NA")) + " NAD, subject to confirmation";
        BalanceValueLabel.Text = depositSelected ? "Balance due later, amount to be confirmed" : "N$ 0 if paid in full";
        PriceNoteLabel.Text = "Indicative total uses the published adult/child rates. Child ages, availability, group requirements and final price must be confirmed. No payment is taken here.";
    }

    private void OnPaymentPlanChanged(object? sender, EventArgs e) => UpdatePriceSummary();
    private void OnCurrencyChanged(object? sender, EventArgs e) => UpdatePriceSummary();

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        if (Navigation.NavigationStack.Count > 1) await Navigation.PopAsync();
        else await Shell.Current.GoToAsync("..");
    }

    private async void OnRequestPaymentLinkClicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_name) || string.IsNullOrWhiteSpace(_email))
        {
            await DisplayAlert("Missing contact details", "Go back and enter your name and email address first.", "OK");
            return;
        }
        try { _ = new MailAddress(_email); }
        catch (FormatException)
        {
            await DisplayAlert("Check your email", "Go back and enter a valid email address.", "OK");
            return;
        }

        var paymentPlan = PaymentPlanPicker.SelectedItem?.ToString() ?? "Pay in full";
        var currency = CurrencyPicker.SelectedItem?.ToString() ?? "Currency to be confirmed";
        var total = _knownTotal.HasValue ? _knownTotal.Value.ToString("C0", CultureInfo.GetCultureInfo("en-NA")) + " NAD (indicative only)" : "Quote required";
        var body = $"Hello Rolling Hills Safaris,\n\nPlease confirm availability and send me the final price and secure payment link for this booking.\n\nTour: {_tour}\nTravel date: {_travelDate:ddd, d MMM yyyy}\nAdults: {_adults}\nChildren: {_children}\nTour preference: {_bookingType}\nIndicative total: {total}\nPayment preference: {paymentPlan}\nPreferred currency: {currency}\n\nName: {_name}\nEmail: {_email}\nPhone / WhatsApp: {_phone}\nAdditional notes: {_notes}\n\nI understand this is a request only and is not a confirmed booking.";
        var subject = Uri.EscapeDataString("Booking checkout - price and secure payment link request");
        var encodedBody = Uri.EscapeDataString(body);
        try
        {
            await Launcher.Default.OpenAsync(new Uri($"mailto:{CompanyEmail}?subject={subject}&body={encodedBody}"));
            await DisplayAlert("Request prepared", "Your email app has opened with your booking details. Review the message and press Send. Your booking is not confirmed and no payment has been taken.", "OK");
        }
        catch
        {
            await DisplayAlert("Contact the team", $"Please email {CompanyEmail} to request availability, final pricing and a secure payment link.", "OK");
        }
    }
}
