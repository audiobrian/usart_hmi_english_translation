@echo off
REM ============================================================
REM USART HMI Translation System - Installation Script
REM Installs decrypted DLLs and translation files
REM ============================================================

echo.
echo ============================================================
echo USART HMI Translation System - Installation
echo ============================================================
echo.

REM Check for admin privileges
net session >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: Administrator privileges required!
    echo.
    echo Right-click this script and select "Run as administrator"
    echo.
    pause
    exit /b 1
)

echo Running with administrator privileges... OK
echo.

REM Set directories
set "USART_DIR=C:\Program Files (x86)\USART HMI"
set "DECRYPTED_DIR=E:\decrypted"
set "DEVEL_DIR=c:\devel"
set "SCRIPT_DIR=%~dp0"
set "TRANSLATION_FILE=%SCRIPT_DIR%..\translation\hmi_translation.txt"

REM Check if USART HMI is installed
if not exist "%USART_DIR%" (
    echo ERROR: USART HMI not found at: %USART_DIR%
    echo.
    echo Please install USART HMI first.
    echo.
    pause
    exit /b 1
)

echo Found USART HMI installation: %USART_DIR%
echo.

REM Check if decrypted DLLs exist
if not exist "%DECRYPTED_DIR%" (
    echo ERROR: Decrypted DLLs directory not found: %DECRYPTED_DIR%
    echo.
    echo Please run decrypt_actr.bat first to decrypt ACTR.dll
    echo.
    pause
    exit /b 1
)

REM Count DLL files
dir /b "%DECRYPTED_DIR%\*.dll" >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: No DLL files found in: %DECRYPTED_DIR%
    echo.
    echo Please run decrypt_actr.bat first.
    echo.
    pause
    exit /b 1
)

echo Found decrypted DLLs in: %DECRYPTED_DIR%
echo.

REM Check if translation file exists
if not exist "%TRANSLATION_FILE%" (
    echo WARNING: Translation file not found: %TRANSLATION_FILE%
    echo.
    echo The translation system will still work, but you need to create
    echo the translation file manually.
    echo.
)

REM Step 1: Create devel directory
echo Step 1: Creating translation directory...
if not exist "%DEVEL_DIR%" (
    mkdir "%DEVEL_DIR%"
    if %errorlevel% neq 0 (
        echo ERROR: Failed to create directory: %DEVEL_DIR%
        echo.
        pause
        exit /b 1
    )
    echo   Created: %DEVEL_DIR%
) else (
    echo   Already exists: %DEVEL_DIR%
)
echo.

REM Step 2: Copy translation file
echo Step 2: Installing translation file...
if exist "%TRANSLATION_FILE%" (
    copy /y "%TRANSLATION_FILE%" "%DEVEL_DIR%\hmi_translation.txt" >nul
    if %errorlevel% neq 0 (
        echo ERROR: Failed to copy translation file
        echo.
        pause
        exit /b 1
    )
    echo   Installed: %DEVEL_DIR%\hmi_translation.txt
) else (
    echo   Skipped: Translation file not found
)
echo.

REM Step 3: Backup original DLLs (optional)
echo Step 3: Backing up original DLLs...
set "BACKUP_DIR=%USART_DIR%\backup_original"
if not exist "%BACKUP_DIR%" (
    mkdir "%BACKUP_DIR%"
    echo   Created backup directory: %BACKUP_DIR%

    REM Only backup if files exist and backup doesn't
    if exist "%USART_DIR%\hmitype.dll" (
        copy /y "%USART_DIR%\*.dll" "%BACKUP_DIR%\" >nul 2>&1
        echo   Backed up original DLLs
    )
) else (
    echo   Backup already exists, skipping...
)
echo.

REM Step 4: Copy decrypted DLLs
echo Step 4: Installing decrypted DLLs...
echo.
echo   Copying all DLL files from: %DECRYPTED_DIR%
echo   To: %USART_DIR%
echo.

set /a count=0
for %%F in ("%DECRYPTED_DIR%\*.dll") do (
    copy /y "%%F" "%USART_DIR%\" >nul
    if %errorlevel% neq 0 (
        echo   ERROR: Failed to copy %%~nxF
    ) else (
        echo   Installed: %%~nxF
        set /a count+=1
    )
)

echo.
echo   Total files installed: %count%
echo.

REM Verification
echo Step 5: Verifying installation...
echo.

set "ERROR_COUNT=0"

REM Check critical DLLs
set "CRITICAL_DLLS=hmitype.dll achmiface.dll rsapp.dll HMIFORM.dll TFTEDIT.dll TFTRUN.dll hmioldapp.dll"

for %%D in (%CRITICAL_DLLS%) do (
    if not exist "%USART_DIR%\%%D" (
        echo   ERROR: Missing critical file: %%D
        set /a ERROR_COUNT+=1
    ) else (
        echo   OK: %%D
    )
)

echo.

if %ERROR_COUNT% gtr 0 (
    echo WARNING: %ERROR_COUNT% critical files are missing!
    echo.
    echo Installation may be incomplete.
    echo.
) else (
    echo All critical files verified successfully!
    echo.
)

REM Check translation file
if exist "%DEVEL_DIR%\hmi_translation.txt" (
    echo   OK: Translation file installed
) else (
    echo   WARNING: Translation file not found
    echo   You need to create it manually at: %DEVEL_DIR%\hmi_translation.txt
)

echo.
echo ============================================================
echo Installation Complete!
echo ============================================================
echo.
echo Summary:
echo   - Decrypted DLLs installed: %count%
echo   - Translation directory: %DEVEL_DIR%
echo   - Log file will be created at: %DEVEL_DIR%\hmi.log
echo   - Original DLLs backed up to: %BACKUP_DIR%
echo.
echo Next steps:
echo   1. Verify hmitype.dll was modified in dnSpy
echo   2. Run: "%USART_DIR%\USART HMI.exe"
echo   3. Check log file for untranslated text: %DEVEL_DIR%\hmi.log
echo   4. Add translations to: %DEVEL_DIR%\hmi_translation.txt
echo.
echo Note: If the application doesn't start, check that ALL 21 DLLs
echo       were copied correctly.
echo.

pause
