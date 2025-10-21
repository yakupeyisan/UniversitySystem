# ========== SOLUTION OLUŞTUR ==========
Write-Host "[STEP 3/3] Solution oluşturuluyor..." -ForegroundColor Yellow

# Eğer solution yoksa oluştur
if (-not (Test-Path "UniversitySystem.sln")) {
    dotnet new sln -n UniversitySystem
    Write-Host "Solution oluşturuldu." -ForegroundColor Green
} else {
    Write-Host "Solution zaten mevcut, atlanıyor." -ForegroundColor Gray
}

# Core projelerini ekle
dotnet sln UniversitySystem.sln add src/Core/Core.Domain/Core.Domain.csproj
dotnet sln UniversitySystem.sln add src/Core/Core.Application/Core.Application.csproj
dotnet sln UniversitySystem.sln add src/Core/Core.Infrastructure/Core.Infrastructure.csproj

# API projesini ekle
dotnet sln UniversitySystem.sln add src/Presentation/API/API.csproj

# Module Domain projelerini ekle
$modules = @(
    "VirtualPOS",
    "Wallet", 
    "Academic",
    "AccessControl",
    "Cafeteria",
    "Parking",
    "Library",
    "EventTicketing",
    "PersonMgmt",
    "Research",
    "Payroll"
)

foreach ($module in $modules) {
    dotnet sln UniversitySystem.sln add "src/Modules/$module/Domain/$module.Domain.csproj"
}

# Test projelerini ekle
dotnet sln UniversitySystem.sln add tests/Unit/Tests.Unit.csproj
dotnet sln UniversitySystem.sln add tests/Integration/Tests.Integration.csproj

Write-Host ""
Write-Host "----------------------------------------" -ForegroundColor Green
Write-Host "  Solution başarıyla oluşturuldu!" -ForegroundColor Green
Write-Host "----------------------------------------" -ForegroundColor Green
