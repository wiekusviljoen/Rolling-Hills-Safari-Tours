# Rolling Hills Safaris — booking and payments integration plan

## Current safe stage

The MAUI app now captures the guest's payment preference (full payment or deposit) and preferred checkout currency (NAD, ZAR, USD, EUR, GBP, or choose at checkout). These are enquiry preferences only. The app does not collect card details, create a live payment, claim a booking is confirmed, or claim availability has been checked.

## Recommended payment provider to evaluate first

DPO Pay by Network publicly describes Namibia online payments settled in NAD and a travel/tourism offering with deposits, full payments, and customer currency choice. Merchant eligibility, supported currencies for this specific Namibian account, conversion/markup, fees, settlement schedule, refunds, and the nominated Namibian bank account must be confirmed in writing with DPO before implementation. Official references:

- Namibia payments: https://dpogroup.com/online-payments/namibia/
- Travel and tourism: https://dpogroup.com/industries/travel-tourism/
- API quick start: https://docs.dpopay.com/dpo-pay-by-network/reference/quick-start-guide
- Merchant onboarding: https://docs.dpopay.com/dpo-pay-by-network/docs/get-a-merchant-account
- Test credentials: https://docs.dpopay.com/dpo-pay-by-network/docs/sandbox-test-credentials

DPO's current documentation says sandbox credentials are issued on request rather than through a public self-service sandbox. Do not hard-code credentials or put them in the MAUI client.

## Architecture required before live checkout

1. Identify the current booking engine used by the official website (provider, account owner, API/webhooks, documentation, and access). This repository contains only the MAUI client and does not reveal an existing booking-system integration. Do not assume a booking provider or scrape the website as a substitute for an API.
2. Implement a server-side booking adapter for that system. Availability and capacity must come from the source of truth, with a temporary hold/lock while checkout is underway. The server must re-check availability before final confirmation and handle expiry, cancellation, and duplicate requests idempotently.
3. Implement a server-side DPO adapter. The client sends a booking reference and payment choice; the server validates the amount and currency from trusted booking/pricing data, creates a DPO hosted-checkout token, and returns only the checkout URL/token needed for redirection.
4. Return customers to a neutral app/web status page. Treat a browser redirect as informational only. The backend must validate DPO callback/webhook and verify the transaction against DPO before recording payment as successful.
5. Persist booking/payment states, audit events, currency, quoted exchange rate and expiry, amount due, deposit amount, remaining balance, provider references, refunds and reconciliation. Never store card PAN/CVV.
6. Send booking and payment confirmation only after the backend verifies the transaction and the booking engine confirms the reservation. Handle failed/abandoned checkout and release expired holds.
7. Add secure configuration/secrets management, HTTPS, authentication for admin operations, backups, privacy/retention policy, and production monitoring before public launch.

## Open dependencies

- Official booking system/vendor and its API credentials/documentation.
- DPO merchant account approval, nominated Namibian bank account confirmation, integration guide/version, test credentials, accepted currencies, fees, settlement terms and refund/chargeback process.
- Business-approved tour prices, deposit rules, cancellation policy, age rules, tax treatment and whether exchange-rate quotes are fixed for a checkout session.
- A hosted backend/API and database. The MAUI app alone is not a secure place for payment secrets or authoritative availability.

## Release gate

No live charges until DPO merchant credentials are issued and verified, booking-system synchronization is tested, callback verification and idempotency are tested, refunds and failed payments are handled, and the business approves the terms. Until then, the app remains an enquiry/preference capture flow.
