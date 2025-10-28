@echo off
REM ============================================================
REM ACTR.dll Decryption Script
REM Decrypts ACTR.dll from USART HMI installation
REM ============================================================

echo.
echo ============================================================
echo ACTR.dll Decryption Script
echo ============================================================
echo.

REM Check if USART HMI is installed
set "USART_DIR=C:\Program Files (x86)\USART HMI"
set "ACTR_FILE=%USART_DIR%\ACTR.dll"

if not exist "%USART_DIR%" (
    echo ERROR: USART HMI not found at: %USART_DIR%
    echo.
    echo Please install USART HMI first or update the path in this script.
    echo.
    pause
    exit /b 1
)

if not exist "%ACTR_FILE%" (
    echo ERROR: ACTR.dll not found at: %ACTR_FILE%
    echo.
    pause
    exit /b 1
)

echo Found USART HMI installation: %USART_DIR%
echo Found ACTR.dll: %ACTR_FILE%
echo.

REM Check if ACTRDecryptor.exe exists
set "SCRIPT_DIR=%~dp0"
set "TOOLS_DIR=%SCRIPT_DIR%..\tools"
set "DECRYPTOR=%TOOLS_DIR%\ACTRDecryptor.exe"
set "APPDLL=%TOOLS_DIR%\AppDllPass.dll"

if not exist "%DECRYPTOR%" (
    echo ERROR: ACTRDecryptor.exe not found at: %DECRYPTOR%
    echo.
    echo Please ensure the tools directory contains:
    echo   - ACTRDecryptor.exe
    echo   - AppDllPass.dll
    echo.
    pause
    exit /b 1
)

if not exist "%APPDLL%" (
    echo ERROR: AppDllPass.dll not found at: %APPDLL%
    echo.
    pause
    exit /b 1
)

echo Found decryption tools:
echo   - ACTRDecryptor.exe
echo   - AppDllPass.dll
echo.

REM Run decryptor
echo Starting decryption...
echo.
echo Command: "%DECRYPTOR%" decode "%ACTR_FILE%"
echo.

cd /d "%TOOLS_DIR%"
"%DECRYPTOR%" decode "%ACTR_FILE%"

if errorlevel 1 (
    echo.
    echo ERROR: Decryption failed!
    echo.
    pause
    exit /b 1
)

echo.
echo ============================================================
echo Decryption Complete!
echo ============================================================
echo.
echo Decrypted files are located in: E:\decrypted\
echo.
echo Next steps:
echo   1. Open hmitype.dll in dnSpy
echo   2. Modify the Language() method
echo   3. Save the modified hmitype.dll
echo   4. Run install_dlls.bat to install
echo.

pause
