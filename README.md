# Rolling Hills Safaris — Native .NET MAUI app

A native C#/.NET MAUI Android app for Rolling Hills Safaris Namibia. It complements the existing company website and does not modify or replace it.

## Implemented in this build

- Native MAUI interface with Rolling Hills green-and-sand branding.
- Safari discovery cards for Sandwich Harbour, birding tours and custom/breakaway safaris.
- Sandwich Harbour rates shown from the company's published page: half day N$2,800 per adult / N$1,400 per child under 13; full day N$3,800 per adult / N$1,900 per child under 13. Published page states minimum two adults and children aged 0–6 free. Always reconfirm rates before confirming a trip.
- Booking enquiry form with preferred date, tour, adult/child counts, shared/private preference, guest contact details and notes.
- Enquiry handoff to the user's email app or WhatsApp, with the request pre-filled for the guest to review and send.
- Company phone, email and official website links.

## Build Android

From this directory:

```powershell
dotnet restore RollingHillsSafaris.csproj -p:TargetFrameworks=net10.0-android
dotnet build RollingHillsSafaris.csproj -f net10.0-android -p:TargetFrameworks=net10.0-android
```

APK output is under `bin\Debug\net10.0-android\`.

## Important: booking-system integrations still required

The current app sends a **booking enquiry**, not a confirmed reservation. It does not currently have:
- live availability synchronized with the company's booking system or logic to block dates that are closed/full;
- a server/backend for storing bookings or sending automatic customer/company confirmations;
- secure payment processing or server-side payment verification.

These require access to the company's booking/availability system or API, a secure backend, and an agreed payment provider. Do not collect card details in the app. Do not represent an enquiry as a confirmed booking.

## iPhone

iOS packaging, signing and physical-device testing require a supported Mac build host and Apple signing credentials. This Windows build only verifies the Android target.

## Before public release

- Add company-approved, licensed safari photography and official logo assets.
- Confirm all tour names, prices, age brackets, included items and terms with the company.
- Connect live availability, booking storage, automated notifications and secure payment through approved services.
- Add or link the approved privacy policy and complete accessibility, device and release-signing tests.
