@echo off
REM ============================================================
REM Installation Verification Script
REM Checks if all files are correctly installed
REM ============================================================

echo.
echo ============================================================
echo USART HMI Translation System - Verification
echo ============================================================
echo.

set "USART_DIR=C:\Program Files (x86)\USART HMI"
set "DEVEL_DIR=c:\devel"
set "ERROR_COUNT=0"

echo Checking installation...
echo.

REM Check USART HMI directory
echo [1] USART HMI Installation Directory
if exist "%USART_DIR%" (
    echo   [OK] Found: %USART_DIR%
) else (
    echo   [ERROR] Not found: %USART_DIR%
    set /a ERROR_COUNT+=1
)
echo.

REM Check critical DLLs
echo [2] Critical DLL Files
set "CRITICAL_DLLS=hmitype.dll achmiface.dll rsapp.dll HMIFORM.dll TFTEDIT.dll TFTRUN.dll hmioldapp.dll"

for %%D in (%CRITICAL_DLLS%) do (
    if exist "%USART_DIR%\%%D" (
        echo   [OK] %%D
    ) else (
        echo   [ERROR] Missing: %%D
        set /a ERROR_COUNT+=1
    )
)
echo.

REM Check translation directory
echo [3] Translation Directory
if exist "%DEVEL_DIR%" (
    echo   [OK] Found: %DEVEL_DIR%
) else (
    echo   [ERROR] Not found: %DEVEL_DIR%
    set /a ERROR_COUNT+=1
)
echo.

REM Check translation file
echo [4] Translation File
if exist "%DEVEL_DIR%\hmi_translation.txt" (
    echo   [OK] Found: %DEVEL_DIR%\hmi_translation.txt

    REM Count lines in translation file
    for /f %%A in ('type "%DEVEL_DIR%\hmi_translation.txt" ^| find /c /v ""') do set LINE_COUNT=%%A
    echo   Info: Contains %LINE_COUNT% translation entries
) else (
    echo   [WARNING] Not found: %DEVEL_DIR%\hmi_translation.txt
    echo   Info: Translation system will log untranslated text
)
echo.

REM Check log file (optional - will be created on first run)
echo [5] Log File (optional)
if exist "%DEVEL_DIR%\hmi.log" (
    echo   [OK] Found: %DEVEL_DIR%\hmi.log

    REM Count lines in log file
    for /f %%A in ('type "%DEVEL_DIR%\hmi.log" ^| find /c /v ""') do set LOG_COUNT=%%A
    echo   Info: Contains %LOG_COUNT% logged entries
) else (
    echo   [INFO] Not found: %DEVEL_DIR%\hmi.log
    echo   Info: Will be created on first run
)
echo.

REM Check file sizes
echo [6] File Size Verification
if exist "%USART_DIR%\hmitype.dll" (
    for %%F in ("%USART_DIR%\hmitype.dll") do set SIZE=%%~zF

    REM Expected size: around 1,174,016 bytes (1.2 MB)
    if %SIZE% gtr 1000000 (
        echo   [OK] hmitype.dll size: %SIZE% bytes
    ) else (
        echo   [WARNING] hmitype.dll size: %SIZE% bytes (seems small)
        echo   Info: Expected size: ~1,174,016 bytes
    )
) else (
    echo   [ERROR] hmitype.dll not found
    set /a ERROR_COUNT+=1
)
echo.

REM Summary
echo ============================================================
echo Verification Summary
echo ============================================================
echo.

if %ERROR_COUNT% equ 0 (
    echo   [OK] All checks passed!
    echo.
    echo   Your installation is complete and ready to use.
    echo.
    echo   Next steps:
    echo     1. Run: "%USART_DIR%\USART HMI.exe"
    echo     2. Use the application
    echo     3. Check: %DEVEL_DIR%\hmi.log for untranslated text
    echo     4. Add translations to: %DEVEL_DIR%\hmi_translation.txt
    echo.
) else (
    echo   [ERROR] Found %ERROR_COUNT% error(s)
    echo.
    echo   Please run install_dlls.bat to fix the installation.
    echo.
)

pause
