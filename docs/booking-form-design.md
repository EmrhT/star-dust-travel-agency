# Booking form design

This slice extends the existing identity-owned booking operation with a real
form. It teaches browser input handling, validation, JSON model binding, and
field-level error reporting without introducing another CRUD subsystem.

## User flow

1. An authenticated user selects a scheduled journey.
2. React opens `/journeys/{journeyId}/book`.
3. The page displays the journey summary and accepts booking details.
4. Client validation gives immediate feedback before submission.
5. `POST /api/bookings` independently validates the submitted JSON.
6. A successful mutation refreshes owned bookings and navigates to
   `/bookings`.

## Input and trust boundaries

| Value | Source | Backend treatment |
| --- | --- | --- |
| `journeyId` | Browser route and request body | Untrusted; parse and verify |
| `numberOfTravelers` | Form | Untrusted; validate range |
| `contactEmail` | Form | Untrusted; validate and normalize |
| `specialRequests` | Form | Untrusted; validate length and normalize |
| `userSubject` | Validated JWT `sub` claim | Trusted identity source |
| `bookedAtUtc` | Backend clock | Trusted server value |

The browser never submits `userSubject` or `bookedAtUtc`. ASP.NET Core derives
them after token validation so a crafted request cannot select another owner or
booking time.

## Form fields

| Field | Required | Validation |
| --- | --- | --- |
| Number of travelers | Yes | Integer from 1 through 6 |
| Contact email | Yes | Nonblank, valid email, at most 254 characters |
| Special requests | No | At most 500 characters after trimming |

Client validation exists for usability. The same constraints remain mandatory
in ASP.NET Core because clients can bypass React and submit HTTP directly.

## HTTP contract

The existing endpoint remains the single booking-creation boundary:

```http
POST /api/bookings
Authorization: Bearer <access-token>
Content-Type: application/json
```

```json
{
  "journeyId": "65c2d80e-0000-0000-0000-000000000000",
  "numberOfTravelers": 2,
  "contactEmail": "traveler@example.com",
  "specialRequests": "Window seats if possible."
}
```

The successful response returns the persisted form values without exposing the
owner subject:

```json
{
  "id": "79bfde80-0000-0000-0000-000000000000",
  "journeyId": "65c2d80e-0000-0000-0000-000000000000",
  "numberOfTravelers": 2,
  "contactEmail": "traveler@example.com",
  "specialRequests": "Window seats if possible.",
  "bookedAtUtc": "2026-10-31T10:00:00Z"
}
```

## Expected outcomes

| Condition | HTTP result |
| --- | --- |
| Valid booking | `201 Created` |
| Invalid form field | `400 ValidationProblemDetails` |
| Missing journey | `404 ProblemDetails` |
| Journey not scheduled | `409 ProblemDetails` |
| Existing user-and-journey booking | `409 ProblemDetails` |
| Missing or invalid token | `401 Unauthorized` |

## Implementation boundaries

- Use controlled Material UI inputs first so React form mechanics stay visible.
- Do not add React Hook Form or a schema-validation library for three fields.
- Keep generated TypeScript DTOs owned by OpenAPI and NSwag.
- Preserve backend ownership derivation from the JWT subject.
- Add one EF Core migration and inspect it before applying it.
- Preserve existing booking rows during the schema migration.

