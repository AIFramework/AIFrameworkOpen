# Run all MathCalculatorTool tests
Write-Host "`n================================================" -ForegroundColor Cyan
Write-Host "  ALL TESTS FOR MATHCALCULATORTOOL" -ForegroundColor Cyan
Write-Host "================================================`n" -ForegroundColor Cyan

$totalTests = 10
$passedTests = 0
$testResults = @()

function Run-Test {
    param(
        [string]$Name,
        [string]$Path,
        [int]$Num,
        [string]$ProjectFile = $null
    )
    
    Write-Host "================================================" -ForegroundColor Yellow
    Write-Host "  TEST $Num/$totalTests : $Name" -ForegroundColor Yellow
    Write-Host "================================================`n" -ForegroundColor Yellow
    
    $success = $false
    
    try {
        Push-Location $Path
        
        if ($ProjectFile) {
            $output = dotnet run --project $ProjectFile 2>&1 | Out-String
        } else {
            $output = dotnet run 2>&1 | Out-String
        }
        
        # Проверяем успех - exit code 0 и нет критических ошибок
        if ($LASTEXITCODE -eq 0) {
            $success = $true
            $script:passedTests++
        }
        
        Write-Host $output
        
    } catch {
        Write-Host "ERROR: $($_.Exception.Message)" -ForegroundColor Red
    } finally {
        Pop-Location
    }
    
    return @{
        Name = $Name
        Number = $Num
        Success = $success
    }
}

# Get Tests path
$testsPath = $PSScriptRoot
if (-not $testsPath) {
    $testsPath = Get-Location
}

# TEST 1: Main test suite (125 tests)
$testResults += Run-Test -Name "Main suite (125 tests)" `
                         -Path "$testsPath\ScientificNotationTest" `
                         -Num 1 `
                         -ProjectFile "TestNewFeatures.csproj"

# TEST 2: Newton method
$testResults += Run-Test -Name "Newton method" `
                         -Path "$testsPath\OriginalScriptTest" `
                         -Num 2

# TEST 3: Limitations
$testResults += Run-Test -Name "Limitations (8 tests)" `
                         -Path "$testsPath\LimitationsTest" `
                         -Num 3

# TEST 4: Comprehensive
$testResults += Run-Test -Name "Comprehensive (37 tests)" `
                         -Path "$testsPath\ComprehensiveTest" `
                         -Num 4

# TEST 5: Edge Cases
$testResults += Run-Test -Name "Edge Cases (Tricky)" `
                         -Path "$testsPath\EdgeCaseTests" `
                         -Num 5 `
                         -ProjectFile "EdgeCaseTests.csproj"

# TEST 6: Extreme Cases
$testResults += Run-Test -Name "Extreme Cases (71 tests)" `
                         -Path "$testsPath\EdgeCaseTests" `
                         -Num 6 `
                         -ProjectFile "ExtremeCases.csproj"

# TEST 7: Comments Support (Python # style) - Basic tests
$testResults += Run-Test -Name "Comments Support (38 tests)" `
                         -Path "$testsPath\EdgeCaseTests" `
                         -Num 7 `
                         -ProjectFile "TestComments.csproj"

# TEST 8: Comments Support - HONEST validation tests
$testResults += Run-Test -Name "HONEST Comment Tests (29 tests)" `
                         -Path "$testsPath\EdgeCaseTests" `
                         -Num 8 `
                         -ProjectFile "HonestCommentTests.csproj"

# TEST 9: String Arrays - join, concat, и все операции
$testResults += Run-Test -Name "String Array Tests (32 tests)" `
                         -Path "$testsPath\EdgeCaseTests" `
                         -Num 9 `
                         -ProjectFile "StringArrayTests.csproj"

# TEST 10: Tokenization - зарезервированные слова, операторы в строках
$testResults += Run-Test -Name "Tokenization Tests (60 tests)" `
                         -Path "$testsPath\EdgeCaseTests" `
                         -Num 10 `
                         -ProjectFile "TokenizationTests.csproj"

# SUMMARY
Write-Host "`n`n================================================" -ForegroundColor Cyan
Write-Host "  SUMMARY" -ForegroundColor Cyan
Write-Host "================================================`n" -ForegroundColor Cyan

foreach ($result in $testResults) {
    $status = if ($result.Success) { "[PASS]" } else { "[FAIL]" }
    $color = if ($result.Success) { "Green" } else { "Red" }
    Write-Host "  $status Test $($result.Number): $($result.Name)" -ForegroundColor $color
}

Write-Host "`n================================================" -ForegroundColor Cyan
$percentage = [math]::Round(($passedTests / $totalTests) * 100, 1)
if ($passedTests -eq $totalTests) {
    Write-Host "  ALL TESTS PASSED! $passedTests/$totalTests (100%)" -ForegroundColor Green
} else {
    Write-Host "  PASSED: $passedTests/$totalTests ($percentage%)" -ForegroundColor Yellow
}
Write-Host "================================================`n" -ForegroundColor Cyan

# Exit code
exit $(if ($passedTests -eq $totalTests) { 0 } else { 1 })
