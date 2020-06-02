# Send X

Send X is a distributed messaging app, developped just for fun.

## Overview

##### Api.Messages
Server-side service to store messages. No authorisation.
##### Client.Cli
Client's agent to send messages. Messages are stored in local DB, until they send to server. 
Allow user to encrypt local DB with a password.  
##### Client.Web
Web-client, single-page application, to discover messages.

## Local development environmet

Use user secrets to set connection string to PostgreSQL for **Sendx.Api.Messages**.

```bash
dotnet user-secrets set ConnectionStrings:DefaultConnection = Host=;Port=5432;Database=sendx;Username=;Password=
```
Use **Migrator** to apply API database migrations.


## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

## License
[MIT](https://choosealicense.com/licenses/mit/)