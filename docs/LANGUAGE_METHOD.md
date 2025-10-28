# Language() Method - Complete Source Code

This file contains the complete modified `Language()` method for `hmitype.dll`.

---

## Method Location

**Assembly:** `hmitype.dll`
**Class:** (obfuscated Unicode name)
**Method:** `Language(this string text)`
**Token:** `0x060005FB`
**Return Type:** `string`
**Parameters:** `this string text`
**Access:** `public static`
**Attributes:** `unsafe`

---

## Complete Method Code

Copy this entire code block and paste it into dnSpy when editing the method:

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

---

## Code Explanation

### Guard Clauses

```csharp
if (!AppServer.Appiniiten)
{
    result = text;
}
```
- **Purpose:** Skip translation if app not initialized
- **Behavior:** Returns original text unchanged

```csharp
else if (text == null || text == "")
{
    result = "";
}
```
- **Purpose:** Handle null/empty strings
- **Behavior:** Returns empty string

---

### Custom Translation System

```csharp
try
{
    string translationFile = @"c:\devel\hmi_translation.txt";
    if (System.IO.File.Exists(translationFile))
    {
        // Translation logic
    }
}
catch { }
```

**Features:**
1. **Safe:** Uses try-catch to prevent crashes
2. **Optional:** Only runs if file exists
3. **Dynamic:** Reads file on every call (no restart needed)

---

### Translation Lookup

```csharp
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
```

**How it works:**
1. Read all lines from translation file
2. Split each line by TAB character
3. If left side matches input text → return right side
4. If match found, return immediately (fast)

**Performance:**
- File I/O on every call (simple but slower)
- Linear search through all lines
- For large translation files (>1000 entries), consider caching

---

### Logging Untranslated Text

```csharp
if (!found)
{
    System.IO.File.AppendAllText(@"c:\devel\hmi.log", text + "\r\n");
}
```

**Behavior:**
- Only logs if translation file EXISTS but text NOT FOUND
- Appends to log file (doesn't overwrite)
- One line per text entry
- Uses Windows line endings (`\r\n`)

**Use case:**
1. Run application
2. Use features
3. Check `hmi.log` for Chinese text
4. Add translations to `hmi_translation.txt`

---

### Fallback to Original System

```csharp
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
```

**Purpose:**
- Runs if translation file doesn't exist
- Runs if translation not found in file
- Uses original USART HMI language system

**Original system:**
1. Converts string to UTF-8 byte array
2. Adds null terminator (`\0`)
3. Calls native `ACLanguage.ACLang()` function
4. Converts result back to string

---

## Removed Code

### Original Version Had:

```csharp
else if (AppData.ExeLanguage != 1)
{
    result = text;
}
```

**Why removed:**
- This check prevented translation when language setting was not Chinese
- We want translation to work regardless of language setting
- Removing this allows the custom translation system to always run

---

## Required Using Statements

The method requires these namespaces:

```csharp
using System;
using System.Text;
```

dnSpy should add these automatically, but if you see compilation errors about `Encoding`, verify they exist at the top of the file.

---

## Dependencies

The method uses these types from the same assembly:

- **`AppServer.Appiniiten`** - Boolean flag for initialization status
- **`AppServer.GetptrtoString()`** - Converts byte pointer to string
- **`ACLanguage.ACLang()`** - Native language translation function

These are part of `hmitype.dll` and don't need to be imported.

---

## Translation File Format

### Location

```
c:\devel\hmi_translation.txt
```

### Format

```
[Chinese]<TAB>[English]
```

### Example

```
指令集	Instruction Set
资料中心	Resource Center
按下事件	Press Event
```

### Rules

1. **Encoding:** Must be UTF-8
2. **Separator:** TAB character (`\t`), not spaces
3. **No header:** First line is data, not column names
4. **Case-sensitive:** Exact match required
5. **No duplicates:** First match wins

---

## Log File Format

### Location

```
c:\devel\hmi.log
```

### Format

```
[Untranslated Chinese text 1]
[Untranslated Chinese text 2]
[Untranslated Chinese text 3]
```

### Example

```
新建文件
删除选项
确认操作
```

### Behavior

- Appends on every call
- May contain duplicates (same text logged multiple times)
- Plain text, no timestamps (for easy copy/paste to translation file)
- Uses Windows line endings

---

## Performance Considerations

### Current Implementation

**Pros:**
- Simple and easy to understand
- No caching = always reads latest file
- No restart needed after editing translations

**Cons:**
- Reads file on every call (slow for frequently-called UI)
- Linear search (O(n) time complexity)
- No duplicate checking in log file

### Potential Optimizations

If you notice performance issues:

1. **Cache translation file:**
   ```csharp
   private static Dictionary<string, string> translationCache = null;

   if (translationCache == null)
   {
       translationCache = new Dictionary<string, string>();
       // Load file into cache
   }
   ```

2. **Use FileSystemWatcher to reload cache:**
   ```csharp
   FileSystemWatcher watcher = new FileSystemWatcher(@"c:\devel\");
   watcher.Filter = "hmi_translation.txt";
   watcher.Changed += (s, e) => translationCache = null;
   ```

3. **Check log file before appending:**
   ```csharp
   string logContent = System.IO.File.ReadAllText(@"c:\devel\hmi.log");
   if (!logContent.Contains(text))
   {
       System.IO.File.AppendAllText(@"c:\devel\hmi.log", text + "\r\n");
   }
   ```

For now, the simple implementation works fine for most use cases.

---

## Testing

### Test Case 1: Translation Exists

**Setup:**
```
hmi_translation.txt:
打开	Open
```

**Input:** `"打开".Language()`
**Expected Output:** `"Open"`

---

### Test Case 2: Translation Not Found (File Exists)

**Setup:**
```
hmi_translation.txt:
打开	Open
```

**Input:** `"关闭".Language()`
**Expected Output:** `"关闭"` (original Chinese)
**Side Effect:** `"关闭"` appended to `hmi.log`

---

### Test Case 3: Translation File Doesn't Exist

**Setup:** No `hmi_translation.txt` file

**Input:** `"打开".Language()`
**Expected Output:** Result from original `ACLanguage.ACLang()`
**Side Effect:** No log file created

---

### Test Case 4: Empty String

**Input:** `"".Language()`
**Expected Output:** `""`

---

### Test Case 5: Null String

**Input:** `((string)null).Language()`
**Expected Output:** `""`

---

## Troubleshooting

### Translations Not Applied

**Check:**
1. File exists: `dir c:\devel\hmi_translation.txt`
2. Encoding is UTF-8 (not ANSI)
3. Using TAB separator (not spaces)
4. Text matches exactly (case-sensitive)

---

### Log File Not Created

**Check:**
1. Folder exists: `mkdir c:\devel`
2. Translation file exists (log only writes if file exists but text not found)
3. Application has write permissions

---

### Application Crashes

**Check:**
1. Method compiled successfully in dnSpy
2. No syntax errors
3. All brackets and braces match
4. Method signature unchanged

---

## Summary

- **Purpose:** Intercept all UI text and apply custom translations
- **Input:** Chinese text from UI
- **Output:** English text from translation file (or original if not found)
- **Side Effect:** Log untranslated text to `hmi.log`
- **Fallback:** Original language system if translation fails

---

## Version History

**v1.0** (October 2025)
- Initial implementation
- File-based translation system
- Logging for untranslated text
- Removed language setting check
