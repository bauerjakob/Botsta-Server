# Botsta Server

## How to migrate database?
1. Install ef migration tools `dotnet tool install --global dotnet-ef`
2. Open ./Botsta.DataSource in terminal
3. Execute command `dotnet ef migrations add Initial --startup-project ../Botsta.Server/ --context BotstaDbContext`
4. Update database `dotnet ef database update --context BotstaDbContext --startup-project ../Botsta.Server/`
