# Rolling Hills Safaris — .NET MAUI mobile app

C# / .NET MAUI starter for the Rolling Hills Safaris Namibia mobile experience. This project is intended to complement the existing company website, not replace it.

## Current state
- Professional green-and-sand visual direction and mobile-first landing screen.
- Native Android app target configured for Windows development.
- Enquiry buttons open the existing company website: https://www.rollinghillssafarisnam.com/.
- Company-approved photography, verified safari details, and final contact/booking flow still need to be added before release.

## Build Android
From this directory, run:

```powershell
dotnet restore RollingHillsSafaris.csproj -p:TargetFrameworks=net10.0-android
dotnet build RollingHillsSafaris.csproj -f net10.0-android -p:TargetFrameworks=net10.0-android
```

## iOS
Packaging and signing require a supported Mac build host and Apple signing credentials. Windows-only Android setup does not produce an iPhone build.

## Release checklist
- Add company-approved, licensed safari images and official logo.
- Verify safari offerings, rates, phone/email/WhatsApp, and booking policy with the company.
- Test on physical Android and iOS devices.
- Complete privacy policy, accessibility, app-store metadata, and release signing.

This is an early development starter, not yet a production release.
