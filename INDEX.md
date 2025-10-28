# USART HMI Translation System - Documentation Index

Quick navigation to all documentation files.

---

## 🚀 Getting Started

**New user? Start here:**

1. **[QUICKSTART.md](QUICKSTART.md)** - 15-minute setup guide
2. **[README.md](README.md)** - Complete documentation

---

## 📁 File Reference

**What's included:**

- **[FILELIST.txt](FILELIST.txt)** - Complete list of all files and their purposes

---

## 🛠️ Tools & Scripts

### Automation Scripts

Located in `scripts/`:

1. **[decrypt_actr.bat](scripts/decrypt_actr.bat)** - Decrypt ACTR.dll automatically
2. **[install_dlls.bat](scripts/install_dlls.bat)** - Install all files (requires admin)
3. **[verify_installation.bat](scripts/verify_installation.bat)** - Check installation

### Decryption Tools

Located in `tools/`:

- **[ACTRDecryptor.exe](tools/ACTRDecryptor.exe)** - Main decryption tool (32-bit)
- **[AppDllPass.dll](tools/AppDllPass.dll)** - Native crypto library
- **[ACTRDecryptor.cs](tools/ACTRDecryptor.cs)** - Source code
- **[README.txt](tools/README.txt)** - Tool documentation

---

## 📖 Detailed Guides

### dnSpy Modification

**[docs/DNSPY_GUIDE.md](docs/DNSPY_GUIDE.md)**

Complete guide for modifying hmitype.dll:
- Opening DLL in dnSpy
- Finding the Language() method
- Editing and compiling
- Saving changes
- Troubleshooting compilation errors

### Language() Method

**[docs/LANGUAGE_METHOD.md](docs/LANGUAGE_METHOD.md)**

Complete source code and explanation:
- Full method implementation
- Line-by-line code explanation
- Dependencies and requirements
- Performance considerations
- Test cases

---

## 🌍 Translation Files

Located in `translation/`:

**[hmi_translation.txt](translation/hmi_translation.txt)**

Sample translation file with 36 common UI strings:
- Format: `[Chinese]<TAB>[English]`
- UTF-8 encoding
- Ready to use

**Format:**
```
指令集	Instruction Set
资料中心	Resource Center
按下事件	Press Event
```

---

## 📝 Quick Reference

### Directory Structure

```
final_release/
├── README.md                   ← Main documentation
├── QUICKSTART.md               ← 15-minute guide
├── FILELIST.txt                ← File listing
├── INDEX.md                    ← This file
│
├── tools/
│   ├── ACTRDecryptor.exe       ← Decryption tool
│   ├── AppDllPass.dll          ← Crypto library
│   ├── ACTRDecryptor.cs        ← Source code
│   ├── ACTRDecryptor.csproj    ← Project file
│   └── README.txt              ← Tool docs
│
├── translation/
│   └── hmi_translation.txt     ← Sample translations
│
├── docs/
│   ├── DNSPY_GUIDE.md          ← dnSpy guide
│   └── LANGUAGE_METHOD.md      ← Method code
│
└── scripts/
    ├── decrypt_actr.bat        ← Decrypt script
    ├── install_dlls.bat        ← Install script
    └── verify_installation.bat ← Verify script
```

---

## 🎯 Common Tasks

### Task 1: First-Time Setup

1. [QUICKSTART.md](QUICKSTART.md) - Follow the 5-step guide
2. [scripts/decrypt_actr.bat](scripts/decrypt_actr.bat) - Decrypt ACTR.dll
3. [docs/DNSPY_GUIDE.md](docs/DNSPY_GUIDE.md) - Modify hmitype.dll
4. [scripts/install_dlls.bat](scripts/install_dlls.bat) - Install files
5. [scripts/verify_installation.bat](scripts/verify_installation.bat) - Verify

### Task 2: Add Translations

1. Run application
2. Check: `c:\devel\hmi.log`
3. Edit: [translation/hmi_translation.txt](translation/hmi_translation.txt)
4. Restart application

### Task 3: Troubleshooting

1. [README.md#troubleshooting](README.md#troubleshooting) - Common issues
2. [docs/DNSPY_GUIDE.md#troubleshooting](docs/DNSPY_GUIDE.md#troubleshooting) - Compilation errors
3. [scripts/verify_installation.bat](scripts/verify_installation.bat) - Check installation

### Task 4: Rebuild Tools

1. [tools/ACTRDecryptor.cs](tools/ACTRDecryptor.cs) - Edit source
2. [tools/README.txt#rebuilding-from-source](tools/README.txt) - Build instructions

---

## 📚 Documentation by Topic

### Decryption

- [README.md#step-1-decrypt-actrdll](README.md#step-1-decrypt-actrdll)
- [tools/README.txt](tools/README.txt)
- [QUICKSTART.md#step-1-decrypt-actrdll](QUICKSTART.md#step-1-decrypt-actrdll)

### Modification

- [README.md#step-2-modify-hmitypedll](README.md#step-2-modify-hmitypedll)
- [docs/DNSPY_GUIDE.md](docs/DNSPY_GUIDE.md)
- [docs/LANGUAGE_METHOD.md](docs/LANGUAGE_METHOD.md)

### Installation

- [README.md#step-3-install-modified-files](README.md#step-3-install-modified-files)
- [QUICKSTART.md#step-3-install-files](QUICKSTART.md#step-3-install-files)
- [scripts/install_dlls.bat](scripts/install_dlls.bat)

### Translation

- [README.md#translation-file-format](README.md#translation-file-format)
- [translation/hmi_translation.txt](translation/hmi_translation.txt)
- [QUICKSTART.md#adding-translations](QUICKSTART.md#adding-translations)

---

## 🔧 Technical Details

### Architecture

**[README.md#overview](README.md#overview)**
- Three-tier component structure
- Two-stage loading mechanism
- Assembly resolution pipeline

### Encryption

**[tools/README.txt#technical-details](tools/README.txt)**
- Custom XOR cipher
- DateTime-based key derivation
- GZIP compression

### Method Implementation

**[docs/LANGUAGE_METHOD.md](docs/LANGUAGE_METHOD.md)**
- Translation lookup algorithm
- Logging system
- Fallback mechanism

---

## ⚡ Quick Links

| What | Where | Purpose |
|------|-------|---------|
| **Start Here** | [QUICKSTART.md](QUICKSTART.md) | 15-minute setup |
| **Full Docs** | [README.md](README.md) | Complete guide |
| **File List** | [FILELIST.txt](FILELIST.txt) | What's included |
| **Decrypt** | [scripts/decrypt_actr.bat](scripts/decrypt_actr.bat) | Run this first |
| **dnSpy Guide** | [docs/DNSPY_GUIDE.md](docs/DNSPY_GUIDE.md) | Modify DLL |
| **Method Code** | [docs/LANGUAGE_METHOD.md](docs/LANGUAGE_METHOD.md) | Paste this code |
| **Install** | [scripts/install_dlls.bat](scripts/install_dlls.bat) | Copy files |
| **Verify** | [scripts/verify_installation.bat](scripts/verify_installation.bat) | Check setup |
| **Translations** | [translation/hmi_translation.txt](translation/hmi_translation.txt) | Add your text |

---

## 📞 Support

### Documentation

All questions are answered in these files:
1. [README.md](README.md) - Complete documentation
2. [QUICKSTART.md](QUICKSTART.md) - Quick setup
3. [docs/DNSPY_GUIDE.md](docs/DNSPY_GUIDE.md) - Modification guide
4. [docs/LANGUAGE_METHOD.md](docs/LANGUAGE_METHOD.md) - Code reference

### Troubleshooting

Check these sections:
1. [README.md#troubleshooting](README.md#troubleshooting)
2. [docs/DNSPY_GUIDE.md#troubleshooting](docs/DNSPY_GUIDE.md#troubleshooting)
3. [QUICKSTART.md#troubleshooting](QUICKSTART.md#troubleshooting)

### Verification

Run these scripts:
1. [scripts/verify_installation.bat](scripts/verify_installation.bat) - Check files
2. [tools/README.txt](tools/README.txt) - Tool help

---

## 📦 Package Information

**Version:** 1.0
**Release Date:** October 2025
**Target Application:** USART HMI 1.67.5.3034
**Platform:** Windows
**Framework:** .NET Framework 3.5

**Package Contents:**
- 4 documentation files (MD/TXT)
- 2 tool executables
- 2 source code files
- 3 automation scripts
- 1 translation file

**Total Size:** ~150 KB (documentation + tools)
**Decrypted Output:** ~23 MB (21 DLLs)

---

## 🗺️ Navigation Tips

### For New Users

1. Start → [QUICKSTART.md](QUICKSTART.md)
2. Need details? → [README.md](README.md)
3. Stuck on dnSpy? → [docs/DNSPY_GUIDE.md](docs/DNSPY_GUIDE.md)

### For Advanced Users

1. Source code → [tools/ACTRDecryptor.cs](tools/ACTRDecryptor.cs)
2. Method code → [docs/LANGUAGE_METHOD.md](docs/LANGUAGE_METHOD.md)
3. Automation → [scripts/](scripts/)

### For Translators

1. Sample file → [translation/hmi_translation.txt](translation/hmi_translation.txt)
2. Format guide → [README.md#translation-file-format](README.md#translation-file-format)
3. How to use → [QUICKSTART.md#adding-translations](QUICKSTART.md#adding-translations)

---

## ✅ Checklist

Use this to track your progress:

- [ ] Read [QUICKSTART.md](QUICKSTART.md)
- [ ] Run [scripts/decrypt_actr.bat](scripts/decrypt_actr.bat)
- [ ] Follow [docs/DNSPY_GUIDE.md](docs/DNSPY_GUIDE.md)
- [ ] Use code from [docs/LANGUAGE_METHOD.md](docs/LANGUAGE_METHOD.md)
- [ ] Run [scripts/install_dlls.bat](scripts/install_dlls.bat)
- [ ] Run [scripts/verify_installation.bat](scripts/verify_installation.bat)
- [ ] Copy [translation/hmi_translation.txt](translation/hmi_translation.txt)
- [ ] Test application
- [ ] Check `c:\devel\hmi.log`
- [ ] Add translations to `c:\devel\hmi_translation.txt`

---

## 🎓 Learning Path

### Beginner

1. [QUICKSTART.md](QUICKSTART.md) - Understand the workflow
2. [scripts/decrypt_actr.bat](scripts/decrypt_actr.bat) - Run automation
3. [docs/DNSPY_GUIDE.md](docs/DNSPY_GUIDE.md) - Follow step-by-step

### Intermediate

1. [README.md](README.md) - Full technical details
2. [docs/LANGUAGE_METHOD.md](docs/LANGUAGE_METHOD.md) - Understand the code
3. [FILELIST.txt](FILELIST.txt) - Know what's where

### Advanced

1. [tools/ACTRDecryptor.cs](tools/ACTRDecryptor.cs) - Modify the tool
2. [tools/README.txt](tools/README.txt) - Rebuild from source
3. [docs/LANGUAGE_METHOD.md#performance-considerations](docs/LANGUAGE_METHOD.md#performance-considerations) - Optimize

---

**Last Updated:** October 2025
**Documentation Version:** 1.0
