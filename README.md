# PublishingHouseApp

PublishingHouseApp is an ASP.NET Core MVC web application for maintaining a publishing house’s book catalog. Users can browse books, and administrators can add, edit, and delete book records.

## Features

- Browse books in a card view and explore featured titles on the home page.
- Manage books, authors, and genres.
- Search the author list and browse paginated results.
- Register and sign in with ASP.NET Core Identity. Book creation, editing, and deletion are restricted to administrators.

## Implementation

- Model the many-to-many relationship between books and genres with a `BookGenre` join entity.
- Prevent author deletion while books are still linked to that author.
- Use paginated lists for book and author views and reuse Razor partial views for table rendering.

## Tech stack

C#, ASP.NET Core MVC, ASP.NET Core Identity, Entity Framework Core, SQL Server, Razor, and Bootstrap.

## Run locally

You need the .NET 9 SDK, Visual Studio on Windows, and a SQL Server instance. These instructions use SQL Server Express LocalDB as an example.

1. Clone the repository and open the solution in Visual Studio. Set `PublishingHouseApp` as the startup project.
2. In a terminal opened in the folder containing the web project's `.csproj` file, configure the development database connection.

   ```powershell
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\MSSQLLocalDB;Database=PublishingHouseApp;Trusted_Connection=True"
   ```

3. Open **Tools → NuGet Package Manager → Package Manager Console**. Select `PublishingHouseApp` as the default project and apply the EF Core migrations.

   ```powershell
   $env:ASPNETCORE_ENVIRONMENT = 'Development'
   Update-Database
   ```

4. Press **F5** using a Development launch profile. The home page opens in the browser.

The database schema must be in place before the first launch because the app queries the ASP.NET Core Identity tables when it starts. After creating the `Admin` and `User` roles as needed, the app runs the sample-data initializer.
