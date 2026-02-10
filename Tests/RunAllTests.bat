@echo off
setlocal enabledelayedexpansion
chcp 65001 >nul
echo.
echo ╔═══════════════════════════════════════════════════════════════╗
echo ║  ЗАПУСК ВСЕХ ТЕСТОВ MATHCALCULATORTOOL                       ║
echo ╚═══════════════════════════════════════════════════════════════╝
echo.

set passed=0
set total=11

echo ═══════════════════════════════════════════════════════════════
echo   ТЕСТ 1/11: Основной набор (141 тест)
echo ═══════════════════════════════════════════════════════════════
dotnet run --project ScientificNotationTest\TestNewFeatures.csproj >nul 2>&1
set ec=!errorlevel!
if !ec!==0 (
    echo ✅ УСПЕШНО
    set /a passed+=1
) else (
    echo ❌ ПРОВАЛЕН (exit code: !ec!^)
)

echo.
echo ═══════════════════════════════════════════════════════════════
echo   ТЕСТ 2/11: Метод Ньютона
echo ═══════════════════════════════════════════════════════════════
dotnet run --project OriginalScriptTest >nul 2>&1
set ec=!errorlevel!
if !ec!==0 (
    echo ✅ УСПЕШНО
    set /a passed+=1
) else (
    echo ❌ ПРОВАЛЕН (exit code: !ec!^)
)

echo.
echo ═══════════════════════════════════════════════════════════════
echo   ТЕСТ 3/11: Тестирование ограничений
echo ═══════════════════════════════════════════════════════════════
dotnet run --project LimitationsTest >nul 2>&1
set ec=!errorlevel!
if !ec!==0 (
    echo ✅ УСПЕШНО
    set /a passed+=1
) else (
    echo ❌ ПРОВАЛЕН (exit code: !ec!^)
)

echo.
echo ═══════════════════════════════════════════════════════════════
echo   ТЕСТ 4/11: Комплексный набор (37 тестов)
echo ═══════════════════════════════════════════════════════════════
dotnet run --project ComprehensiveTest >nul 2>&1
set ec=!errorlevel!
if !ec!==0 (
    echo ✅ УСПЕШНО
    set /a passed+=1
) else (
    echo ❌ ПРОВАЛЕН (exit code: !ec!^)
)

echo.
echo ═══════════════════════════════════════════════════════════════
echo   ТЕСТ 5/11: Каверзные тесты (Edge Cases)
echo ═══════════════════════════════════════════════════════════════
dotnet run --project EdgeCaseTests\EdgeCaseTests.csproj >nul 2>&1
set ec=!errorlevel!
if !ec!==0 (
    echo ✅ УСПЕШНО
    set /a passed+=1
) else (
    echo ❌ ПРОВАЛЕН (exit code: !ec!^)
)

echo.
echo ═══════════════════════════════════════════════════════════════
echo   ТЕСТ 6/11: Экстремальные тесты (71 тест)
echo ═══════════════════════════════════════════════════════════════
dotnet run --project EdgeCaseTests\ExtremeCases.csproj >nul 2>&1
set ec=!errorlevel!
if !ec!==0 (
    echo ✅ УСПЕШНО
    set /a passed+=1
) else (
    echo ❌ ПРОВАЛЕН (exit code: !ec!^)
)

echo.
echo ═══════════════════════════════════════════════════════════════
echo   ТЕСТ 7/11: Поддержка комментариев (38 тестов)
echo ═══════════════════════════════════════════════════════════════
dotnet run --project EdgeCaseTests\TestComments.csproj >nul 2>&1
set ec=!errorlevel!
if !ec!==0 (
    echo ✅ УСПЕШНО
    set /a passed+=1
) else (
    echo ❌ ПРОВАЛЕН (exit code: !ec!^)
)

echo.
echo ═══════════════════════════════════════════════════════════════
echo   ТЕСТ 8/11: ЧЕСТНЫЕ тесты комментариев (29 тестов)
echo ═══════════════════════════════════════════════════════════════
dotnet run --project EdgeCaseTests\HonestCommentTests.csproj >nul 2>&1
set ec=!errorlevel!
if !ec!==0 (
    echo ✅ УСПЕШНО
    set /a passed+=1
) else (
    echo ❌ ПРОВАЛЕН (exit code: !ec!^)
)

echo.
echo ═══════════════════════════════════════════════════════════════
echo   ТЕСТ 9/11: Массивы строк (43 теста)
echo ═══════════════════════════════════════════════════════════════
dotnet run --project EdgeCaseTests\StringArrayTests.csproj >nul 2>&1
set ec=!errorlevel!
if !ec!==0 (
    echo ✅ УСПЕШНО
    set /a passed+=1
) else (
    echo ❌ ПРОВАЛЕН (exit code: !ec!^)
)

echo.
echo ═══════════════════════════════════════════════════════════════
echo   ТЕСТ 10/11: Токенизация (56 тестов)
echo ═══════════════════════════════════════════════════════════════
dotnet run --project EdgeCaseTests\TokenizationTests.csproj >nul 2>&1
set ec=!errorlevel!
if !ec!==0 (
    echo ✅ УСПЕШНО
    set /a passed+=1
) else (
    echo ❌ ПРОВАЛЕН (exit code: !ec!^)
)

echo.
echo ═══════════════════════════════════════════════════════════════
echo   ТЕСТ 11/11: Complex Comment Tests (24 теста)
echo ═══════════════════════════════════════════════════════════════
dotnet run --project EdgeCaseTests\ComplexCommentTests.csproj >nul 2>&1
set ec=!errorlevel!
if !ec!==0 (
    echo ✅ УСПЕШНО
    set /a passed+=1
) else (
    echo ❌ ПРОВАЛЕН (exit code: !ec!^)
)

echo.
echo ╔═══════════════════════════════════════════════════════════════╗
if !passed!==!total! (
    echo ║  🎉 ВСЕ ТЕСТЫ ПРОЙДЕНЫ! !passed!/!total!
) else (
    echo ║  ⚠️  ПРОЙДЕНО: !passed!/!total!
)
echo ╚═══════════════════════════════════════════════════════════════╝
echo.

endlocal
pause
