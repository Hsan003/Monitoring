# Monitoring

To set up the project, create a `.env` file with the following content: 
```
DB_HOST=localhost
DB_PORT=3306
DB_NAME=UptimeMonitoring
DB_USER=root
DB_PASSWORD=

SMTP_SERVER=localhost
SMTP_PORT=1025
SENDER_EMAIL=test@example.com
SENDER_PASSWORD=`.
```
Then, install the dependencies by running `dotnet restore`.

To update the migrations and apply them to the database, run `dotnet ef database update`.
