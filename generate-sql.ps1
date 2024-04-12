dotnet tool install --global dotnet-ef

cd src/EntityFrameworkCore

rm sql -Force -Recurse
rm Relationships\Migrations -Force -Recurse

dotnet ef migrations add "Initial" --project .\Relationships\Relationships.csproj
dotnet ef database update --project .\Relationships\Relationships.csproj
dotnet ef migrations script --no-build --idempotent --project .\Relationships\Relationships.csproj --output sql/table/EFCore.migration.sql

cd ../..
