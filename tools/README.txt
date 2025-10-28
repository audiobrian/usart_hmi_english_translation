================================================================================
ACTR DECRYPTION TOOLS
================================================================================

This directory contains tools for decrypting the ACTR.dll container.

================================================================================
FILES IN THIS DIRECTORY
================================================================================

ACTRDecryptor.exe               Main decryption tool (32-bit)
AppDllPass.dll                  Native decryption library (required)
ACTRDecryptor.cs                Source code (C#)
ACTRDecryptor.csproj            Project file for rebuilding

================================================================================
USAGE
================================================================================

Basic Usage:
------------

To decrypt ACTR.dll:

    ACTRDecryptor.exe decode "C:\Program Files (x86)\USART HMI\ACTR.dll"

Output will be in: E:\decrypted\


Automated Script:
-----------------

Or use the automated script in ..\scripts\:

    cd ..\scripts
    decrypt_actr.bat

================================================================================
OUTPUT
================================================================================

The decryptor extracts 21 DLL files to E:\decrypted\:

  1. achmiface.dll              (666 KB)
  2. AxInterop.WMPLib.dll       (61 KB)
  3. ColListBox.dll             (23 KB)
  4. Colpanel.dll               (11 KB)
  5. ControlInterFace.dll       (16 KB)
  6. DevComponents.DotNetBar2.dll (5.5 MB)
  7. hmiapp_old.dll             (168 KB)
  8. HMIFORM.dll                (1.5 MB)
  9. hmioldapp.dll              (318 KB)
 10. hmitype.dll                (1.2 MB) ← MODIFY THIS ONE
 11. Interop.WMPLib.dll         (339 KB)
 12. Microsoft.mshtml.dll       (8 MB)
 13. msvcm90d.dll               (311 KB)
 14. msvcp90d.dll               (868 KB)
 15. msvcr90d.dll               (1.2 MB)
 16. Newtonsoft.Json.dll        (475 KB)
 17. PicListBox.dll             (19 KB)
 18. rsapp.dll                  (356 KB)
 19. Tcode.dll                  (273 KB)
 20. TFTEDIT.dll                (184 KB)
 21. TFTRUN.dll                 (22 KB)

TOTAL: ~23 MB

================================================================================
TECHNICAL DETAILS
================================================================================

What It Does:
-------------

1. Reads ACTR.dll (9.8 MB encrypted container)
2. Parses 64-byte headers for each assembly
3. XOR decrypts headers with key 0x09
4. Extracts assembly name, size, compression flag
5. Reads encrypted payload
6. Decrypts using AppDllPass_Decode() with DateTime-based keys
7. Decompresses GZIP data (if compression flag = 1)
8. Writes decrypted DLL to E:\decrypted\

Encryption:
-----------

- Algorithm: Custom XOR cipher with state machine
- Key derivation: DateTime (Year, Month, Day, Hour, Minute, Second, Millisecond)
- Buffer size: 1024 bytes
- Assembly-specific index for additional entropy

Compression:
------------

- Format: GZIP (RFC 1952)
- Signature: 0x1F 0x8B 0x08
- All 21 files are compressed in ACTR.dll

Container Format:
-----------------

[Header 1: 64 bytes XOR 0x09]
  Format: "AssemblyName.dll#Size#CompressionFlag"
[Payload 1: encrypted + compressed]

[Header 2: 64 bytes XOR 0x09]
[Payload 2: encrypted + compressed]

...

[Header 21: 64 bytes XOR 0x09]
[Payload 21: encrypted + compressed]

================================================================================
REQUIREMENTS
================================================================================

- Windows (32-bit or 64-bit)
- .NET 9.0 Runtime (for ACTRDecryptor.exe)
- AppDllPass.dll must be in same directory
- ACTR.dll file (from USART HMI installation)

================================================================================
REBUILDING FROM SOURCE
================================================================================

If you need to modify the decryptor:

1. Edit ACTRDecryptor.cs
2. Open command prompt
3. Run:

   dotnet build ACTRDecryptor.csproj -c Release

4. Output:

   bin\Release\net9.0-windows\win-x86\ACTRDecryptor.exe

5. Copy to this directory

================================================================================
TROUBLESHOOTING
================================================================================

Error: "Could not load AppDllPass.dll"
--------------------------------------
Solution: Ensure AppDllPass.dll is in the same directory as ACTRDecryptor.exe


Error: "BadImageFormatException"
---------------------------------
Solution: Use 32-bit version (this package includes 32-bit)


Error: "ACTR.dll not found"
---------------------------
Solution: Provide correct path to ACTR.dll:

  ACTRDecryptor.exe decode "C:\path\to\ACTR.dll"


Error: "Decompression failed"
-----------------------------
This is normal - the tool now skips decompression by default.
Decompression is handled automatically in the latest version.

================================================================================
COMMAND-LINE OPTIONS
================================================================================

Decode Mode (decrypt ACTR.dll):
-------------------------------

  ACTRDecryptor.exe decode <path_to_ACTR.dll>

  Example:
    ACTRDecryptor.exe decode "C:\Program Files (x86)\USART HMI\ACTR.dll"

  Output:
    E:\decrypted\*.dll (21 files)


Encode Mode (re-encrypt DLLs):
------------------------------

  ACTRDecryptor.exe encode <path_to_directory>

  Example:
    ACTRDecryptor.exe encode E:\decrypted\

  Output:
    ACTR_translated.dll (in parent directory)

  Note: Encoding is NOT recommended. Use filesystem loading instead.

================================================================================
NOTES
================================================================================

- The decryption process is deterministic (same input = same output)
- Decryption keys are derived from current DateTime
- Output files are standard .NET assemblies (can be opened in dnSpy)
- Original ACTR.dll is never modified (read-only operation)
- The tool creates E:\decrypted\ if it doesn't exist

================================================================================
PERFORMANCE
================================================================================

Decryption speed: ~1-2 seconds for all 21 files
Bottleneck: File I/O and GZIP decompression

Largest files (by compressed size):
  - DevComponents.DotNetBar2.dll: 2.6 MB compressed
  - Microsoft.mshtml.dll: 2.5 MB compressed
  - HMIFORM.dll: 1.7 MB compressed

================================================================================
SECURITY
================================================================================

This tool is for analysis and educational purposes only.

The decryption algorithm is custom and specific to USART HMI.
Source code is provided for transparency.

DO NOT use this tool to:
  - Pirate software
  - Circumvent licensing
  - Distribute decrypted files

ONLY use for:
  - Personal translation/localization
  - Bug analysis
  - Educational purposes

================================================================================
END OF README
================================================================================
