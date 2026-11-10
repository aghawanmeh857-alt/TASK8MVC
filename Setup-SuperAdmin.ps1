$ErrorActionPreference = 'Stop'
$email = Read-Host 'SuperAdmin email'
$securePassword = Read-Host 'Password (at least 6 characters, uppercase, lowercase, number, symbol)' -AsSecureString
$credential = [System.Net.NetworkCredential]::new('', $securePassword)
$previousEmail = $env:Bootstrap__SuperAdminEmail
$previousPassword = $env:Bootstrap__SuperAdminPassword
try {
    $env:Bootstrap__SuperAdminEmail = $email
    $env:Bootstrap__SuperAdminPassword = $credential.Password
    Write-Host 'Starting the project. The initial SuperAdmin is created only if none exists.'
    Write-Host 'Sign in with the email and password you entered. Press Ctrl+C to stop.'
    dotnet run --project (Join-Path $PSScriptRoot 'TASK8MVC/TASK8MVC.csproj') --launch-profile https
}
finally {
    $env:Bootstrap__SuperAdminEmail = $previousEmail
    $env:Bootstrap__SuperAdminPassword = $previousPassword
    $credential.Password = ''
}
