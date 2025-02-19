param(# Migration Name
    [string]
    $MigrationName, 
    [switch]
    $verbose)

Write-Host "Adding migrations"
if ($verbose) {
    dotnet ef migrations add $MigrationName -p .\MReminders.API.Infrastructure\ -s .\MReminders.API.Server\ -c AppDbContext -v 
}
else {
    dotnet ef migrations add $MigrationName -p .\MReminders.API.Infrastructure\ -s .\MReminders.API.Server\ -c AppDbContext 
}
Write-Host "Done with adding migrations"


