# csharp.examples

The examples all use MSSQL Server so that 
```powershell
dotnet tool install --global dotnet-ef
// make sure db exists -- i.e. Blog
dotnet ef database update
```

To create the migration and generate the sql
```powershell
cd src/EntityFrameworkCore
dotnet ef migrations add "Initial"
dotnet ef database update --project .\Relationships\Relationships.csproj
dotnet ef migrations script --no-build --idempotent --project .\Relationships\Relationships.csproj --output sql/table/EFCore.migration.sql
```

## Relationships

Shows.....
