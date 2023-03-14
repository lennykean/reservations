# Reservations API

A simple API for managing reservations and availability windows for service providers.

## Features

- Create and update availability windows by provider
- Create reservations and validate reservation times against availability windows
- Confirm reservations
- List available timeslots  by provider

## Additional Considerations

- **Timezones**: To productionalize, everything should be handled in UTC.
- **Timeslots**: We may want to consider additional restrictions on timeslots, such as minimum and maximum reservation length, and minimum and maximum availability window size.
- **Database**: SQLite is not a suitable production database. This should be swapped out for an external datastore.
  - It would be a good idea to use computed columes for expiration, and created dates, but sqlite doesn't support that for non-deterministic data
- **Security**: It's assumed that authorization and SSL will be provided by a reverse proxy or API gateway.
- **API**: It would be a good enhancement to create bespoke input models for creating availability windows and reservations
