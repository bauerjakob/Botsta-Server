FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY Botsta.Server/*.csproj ./Botsta.Server/
COPY Botsta.DataStorage/*.csproj ./Botsta.DataStorage/
RUN dotnet restore

# copy everything else and build app
COPY Botsta.Server/. ./Botsta.Server/
COPY Botsta.DataStorage/. ./Botsta.DataStorage/
WORKDIR /source/Botsta.Server/
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
EXPOSE 80
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "Botsta.Server.dll"]