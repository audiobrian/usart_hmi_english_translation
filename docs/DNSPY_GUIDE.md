# dnSpy Modification Guide

Complete step-by-step guide for modifying `hmitype.dll` using dnSpy.

---

## Prerequisites

1. **dnSpy installed** (x86 or x64 version)
   - Download: https://github.com/dnSpy/dnSpy/releases
   - Extract to a folder (e.g., `C:\Tools\dnSpy\`)

2. **Decrypted hmitype.dll**
   - Location: `E:\decrypted\hmitype.dll`
   - Must be decrypted using ACTRDecryptor first

---

## Step 1: Open hmitype.dll in dnSpy

1. Launch **dnSpy.exe** (32-bit recommended)
2. Click **File → Open**
3. Navigate to `E:\decrypted\`
4. Select **hmitype.dll**
5. Click **Open**

The assembly will appear in the left panel.

---

## Step 2: Find the Language() Method

### Option A: Using Search (Recommended)

1. Press **Ctrl+Shift+K** (or **Edit → Search Assemblies**)
2. In the search window:
   - **Search for:** `Language`
   - **Search in:** Select **Member (U)**
   - **Match:** Any
3. Click **Search**
4. Look for: `string Language(this string text)`
   - Type: Method
   - Declaring Type: (obfuscated class name)
   - Token: `060005FB`

### Option B: Manual Navigation

1. Expand **hmitype** in the left panel
2. Expand the **first namespace** (will have Unicode characters)
3. Look for a **class with obfuscated name** (Unicode control characters)
4. Expand **Methods**
5. Find **Language (String) : String**

---

## Step 3: Edit the Method

1. **Right-click** on the `Language` method
2. Select **Edit Method (C#)...**

A code editor window will open showing the current method implementation.

---

## Step 4: Replace Method Code

### Original Code

You'll see something like:

```csharp
public unsafe static string Language(this string text)
{
    string result;
    if (!AppServer.Appiniiten)
    {
        result = text;
    }
    else if (AppData.ExeLanguage != 1)
    {
        result = text;
    }
    else if (text == null || text == "")
    {
        result = "";
    }
    else
    {
        byte[] bytes = Encoding.UTF8.GetBytes(text + "\0");
        // ... more code
    }
    return result;
}
```

### New Code

**Delete everything** and replace with:

```csharp
public unsafe static string Language(this string text)
{
    string result;
    if (!AppServer.Appiniiten)
    {
        result = text;
    }
    else if (text == null || text == "")
    {
        result = "";
    }
    else
    {
        // CUSTOM TRANSLATION SYSTEM
        try
        {
            string translationFile = @"c:\devel\hmi_translation.txt";
            if (System.IO.File.Exists(translationFile))
            {
                string[] lines = System.IO.File.ReadAllLines(translationFile, Encoding.UTF8);
                bool found = false;

                foreach (string line in lines)
                {
                    string[] parts = line.Split('\t');
                    if (parts.Length >= 2 && parts[0] == text)
                    {
                        found = true;
                        return parts[1]; // Return English translation
                    }
                }

                // File exists but translation not found - log it
                if (!found)
                {
                    System.IO.File.AppendAllText(@"c:\devel\hmi.log", text + "\r\n");
                }
            }
        }
        catch { }

        // Original flow if translation system fails
        byte[] bytes = Encoding.UTF8.GetBytes(text + "\0");
        if (bytes.Length < 2)
        {
            result = "";
        }
        else
        {
            string text2;
            fixed (void* ptr = bytes)
            {
                text2 = AppServer.GetptrtoString(ACLanguage.ACLang((byte*)ptr), Encoding.UTF8);
            }
            result = text2;
        }
    }
    return result;
}
```

### Key Changes

1. **Removed:** `else if (AppData.ExeLanguage != 1)` check
   - Now works regardless of language setting

2. **Added:** Translation file lookup
   - Reads `c:\devel\hmi_translation.txt`
   - Searches for Chinese text → English mapping

3. **Added:** Logging for untranslated text
   - Writes to `c:\devel\hmi.log`
   - Only logs if translation file exists but text not found

---

## Step 5: Compile the Method

1. Click the **Compile** button (bottom right)
2. Wait for compilation

### Success

You should see:
```
Compilation successful
```

The editor window will close automatically.

### Common Errors

#### Error: "The name 'Encoding' does not exist in the current context"

**Solution:** The `using System.Text;` directive is missing. dnSpy should add it automatically, but if not:

1. Click **Cancel**
2. Scroll to the top of the file
3. Check that these lines exist:
   ```csharp
   using System;
   using System.Text;
   ```
4. If missing, add `using System.Text;` after `using System;`
5. Try compiling again

#### Error: "Cannot use unsafe code"

**Solution:** The assembly is marked with `[module: UnverifiableCode]`. This should already be present, but if not:

1. This is unusual - the assembly should support unsafe code
2. Verify you're editing the correct method
3. Try restarting dnSpy and opening the file again

#### Error: Line number or syntax errors

**Solution:**
1. Make sure you copied the ENTIRE method including the closing brace `}`
2. Check for any missing semicolons or brackets
3. Compare line-by-line with the code in `LANGUAGE_METHOD.md`

---

## Step 6: Verify Changes

After successful compilation:

1. The method name should appear **bold** in the left panel (indicates modification)
2. Double-click the method to view the decompiled code
3. Verify your changes are present

---

## Step 7: Save Modified DLL

1. Click **File → Save Module**
   - Or press **Ctrl+S**
2. A save dialog appears
3. **Important:** Keep the filename as `hmitype.dll`
4. Choose to **overwrite** the existing file
5. Click **Save**

### Confirmation

You should see a brief progress indicator, then the file is saved.

---

## Step 8: Backup (Optional but Recommended)

Before copying to the installation directory:

```cmd
copy "E:\decrypted\hmitype.dll" "E:\decrypted\hmitype.dll.modified"
```

This allows you to compare original vs. modified later.

---

## Step 9: Install Modified DLL

Copy the modified DLL to the USART HMI installation:

```cmd
copy "E:\decrypted\hmitype.dll" "C:\Program Files (x86)\USART HMI\hmitype.dll"
```

**Important:** You must copy ALL 21 DLLs from `E:\decrypted\`, not just `hmitype.dll`.

---

## Verification Checklist

After modification:

- [ ] Method compiled successfully
- [ ] Method name appears bold in dnSpy
- [ ] File saved (Ctrl+S)
- [ ] File size changed (should be larger than original)
- [ ] Backed up modified file (optional)
- [ ] Copied to installation directory
- [ ] Created `c:\devel\` folder
- [ ] Copied `hmi_translation.txt` to `c:\devel\`

---

## Testing

1. Create the translation directory:
   ```cmd
   mkdir c:\devel
   ```

2. Copy the sample translation file:
   ```cmd
   copy final_release\translation\hmi_translation.txt c:\devel\
   ```

3. Run USART HMI:
   ```cmd
   "C:\Program Files (x86)\USART HMI\USART HMI.exe"
   ```

4. Check for log file:
   ```cmd
   type c:\devel\hmi.log
   ```

If the log file exists and contains Chinese text, the modification is working!

---

## Troubleshooting

### dnSpy Won't Open the DLL

**Problem:** "This file is not a valid assembly"

**Solutions:**
1. Verify the file was decrypted properly (check file size)
2. Check that the file has the `MZ` header:
   ```cmd
   xxd hmitype.dll | head -1
   ```
   Should show: `4d5a 9000...` (MZ header)

3. Make sure you're using the decrypted version, not the compressed one

### Method Won't Compile

**Problem:** Errors about missing types or namespaces

**Solution:**
1. Make sure you're editing the correct `Language` method
2. Check that all `using` statements are present
3. Verify the class contains `AppServer`, `ACLanguage`, etc.
4. If the class structure is different, you may be editing the wrong method

### Modified DLL Doesn't Work

**Problem:** Application crashes or translations don't apply

**Solutions:**

1. **Check all DLLs are copied:**
   ```cmd
   dir "C:\Program Files (x86)\USART HMI\hmitype.dll"
   dir "C:\Program Files (x86)\USART HMI\hmioldapp.dll"
   ```

2. **Check translation file exists:**
   ```cmd
   dir c:\devel\hmi_translation.txt
   ```

3. **Check file encoding:**
   - Open `hmi_translation.txt` in Notepad++
   - **Encoding** menu → Should be **UTF-8**

4. **Check for syntax errors:**
   - Re-open `hmitype.dll` in dnSpy
   - Check the `Language` method for compilation errors

### Original Behavior Restored

**Problem:** The application works but uses original Chinese translations

**Solution:**

This means the modified method isn't being called. Possible causes:

1. **Wrong file copied:** Check that the modified `hmitype.dll` was copied, not the original
2. **DLL not loaded:** The application may be loading from ACTR.dll instead of filesystem
   - Verify ALL 21 DLLs are in the installation directory
3. **Cache issue:** Try deleting files from `%APPDATA%\USART HMI\`

---

## Advanced: Viewing IL Code

To see the compiled IL (Intermediate Language):

1. Right-click the `Language` method
2. Select **Edit IL Instructions...**
3. You'll see low-level IL opcodes

This is useful for:
- Verifying the method was actually modified
- Debugging compilation issues
- Understanding what the C# code compiles to

---

## Alternative: Hex Editing (Not Recommended)

While you can modify DLLs using a hex editor, this is:
- **Error-prone:** Easy to corrupt the file
- **Difficult:** Requires understanding PE format and IL opcodes
- **Unnecessary:** dnSpy provides a much safer method

Stick with dnSpy unless you have specific advanced needs.

---

## Notes

### Why Modify This Method?

The `Language()` method is an extension method called by nearly every UI string in the application:

```csharp
string displayText = "打开".Language();
```

By intercepting this method, we can:
1. Replace Chinese text with English from our translation file
2. Log untranslated text for later translation
3. Bypass the built-in language system entirely

### Original Flow

Without modification:
```
Chinese Text → Language() → ACLanguage.ACLang() → Chinese/English (limited)
```

### Modified Flow

With modification:
```
Chinese Text → Language() → Check hmi_translation.txt → English (custom)
                          ↓ (if not found)
                          → Log to hmi.log
                          → Original flow (fallback)
```

---

## Summary

1. Open `hmitype.dll` in dnSpy
2. Find `Language(this string text)` method
3. Replace method code with custom translation logic
4. Compile
5. Save module
6. Copy to installation directory
7. Create `c:\devel\hmi_translation.txt`
8. Run and test

**Time required:** 5-10 minutes
**Difficulty:** Beginner (no programming required - just copy/paste)

---

## Next Steps

- See `LANGUAGE_METHOD.md` for the complete method source code
- See `README.md` for installation instructions
- See `../translation/hmi_translation.txt` for translation examples
