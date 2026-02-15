# LinkHub Project - Getting Started

LinkHub a platform to allow user to create client contacts, and link contacts to multiple clients

##  Tools to get started

- **.NET 8.0 SDK or Later** 
- **SQL Server or LocalDb**
- **Visual Studio 2022+** or **VS Code**


## 1. Clone the Repository

```
git clone https://your-repo-url/LinkHub.git
cd LinkHub
```

## 2. Database Setup

- Update the connection string in `appsettings.json` (in both API and UI projects) to point to your SQL Server instance.
- Run EF Core migrations to create the database:

```
dotnet ef database update --project LinkHub.Infrastructure --startup-project LinkHub.API
```

## 3. Build the Solution

```
dotnet Clean
```

```
dotnet Restore
```

```
dotnet build
```

## 4. Run the API

```
dotnet run --project LinkHub.API/LinkHub.API.csproj
```
- The API will start on `https://localhost:5001`

## 5. Run the UI

```
dotnet run --project LinkHub.UI/LinkHub.UI.csproj
```
- The UI will start on `https://localhost:5097`






