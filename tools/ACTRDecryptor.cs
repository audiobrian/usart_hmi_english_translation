using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ACTRDecryptor
{
    class Program
    {
        // Importar las funciones de AppDllPass.dll
        [DllImport("AppDllPass.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe void AppDllPass_Decode(byte* data, uint length, uint assemblyIndex, byte* keyBuffer);

        [DllImport("AppDllPass.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe void AppDllPass_Encode(byte* data, uint length);

        static unsafe void Main(string[] args)
        {
            Console.WriteLine("ACTR.dll Decryptor/Encryptor");
            Console.WriteLine("=============================\n");

            if (args.Length < 2)
            {
                Console.WriteLine("Uso:");
                Console.WriteLine("  Desencriptar: ACTRDecryptor.exe decode <archivo_ACTR.dll>");
                Console.WriteLine("  Encriptar:    ACTRDecryptor.exe encode <directorio_assemblies>");
                Console.WriteLine("\nEjemplos:");
                Console.WriteLine("  ACTRDecryptor.exe decode \"C:\\Program Files (x86)\\USART HMI\\ACTR.dll\"");
                Console.WriteLine("  ACTRDecryptor.exe encode decrypted\\");
                return;
            }

            string mode = args[0].ToLower();
            string path = args[1];

            if (mode == "decode")
            {
                if (!File.Exists(path))
                {
                    Console.WriteLine($"Error: No se encuentra el archivo {path}");
                    return;
                }
                DecodeACTR(path);
            }
            else if (mode == "encode")
            {
                if (!Directory.Exists(path))
                {
                    Console.WriteLine($"Error: No se encuentra el directorio {path}");
                    return;
                }
                EncodeACTR(path);
            }
            else
            {
                Console.WriteLine("Error: Modo debe ser 'decode' o 'encode'");
            }
        }

        static unsafe void DecodeACTR(string actrPath)
        {
            Console.WriteLine($"Leyendo {actrPath}...");
            byte[] actrData = File.ReadAllBytes(actrPath);

            Console.WriteLine($"Tamaño: {actrData.Length} bytes");
            Console.WriteLine("\nDesencriptando contenedor ACTR...");

            // El archivo ACTR tiene múltiples assemblies encriptados
            // Formato: [Header 64 bytes XOR 0x09][Payload]...

            string outputDir = Path.GetDirectoryName(actrPath) + "\\decrypted";
            Directory.CreateDirectory(outputDir);

            using (MemoryStream ms = new MemoryStream(actrData))
            using (BinaryReader br = new BinaryReader(ms))
            {
                int assemblyCount = 0;

                while (ms.Position + 64 < ms.Length)
                {
                    // Leer header (64 bytes)
                    byte[] header = br.ReadBytes(64);

                    // XOR con 0x09 para obtener el header real
                    for (int i = 0; i < header.Length; i++)
                    {
                        header[i] ^= 9;
                    }

                    string headerStr = System.Text.Encoding.ASCII.GetString(header).Replace("\0", "").Trim();
                    string[] parts = headerStr.Split('#');

                    if (parts.Length < 2)
                        break;

                    string dllName = parts[0];
                    int size = int.Parse(parts[1]);
                    string compressed = parts.Length > 2 ? parts[2] : "0";

                    Console.WriteLine($"\n[{assemblyCount}] {dllName} - {size} bytes (compressed: {compressed})");

                    // Leer payload encriptado
                    Console.WriteLine("  Leyendo payload...");
                    byte[] encryptedData = br.ReadBytes(size);

                    // Determinar el índice del assembly
                    Console.WriteLine("  Determinando índice...");
                    uint assemblyIndex = GetAssemblyIndex(dllName);

                    // Inicializar buffer de clave (1024 bytes)
                    Console.WriteLine("  Inicializando buffer de clave...");
                    IntPtr originalBuffer;
                    byte* keyBuffer = InitializeKeyBuffer(out originalBuffer);

                    // Desencriptar
                    Console.WriteLine("  Desencriptando...");
                    fixed (byte* dataPtr = encryptedData)
                    {
                        AppDllPass_Decode(dataPtr, (uint)encryptedData.Length, assemblyIndex, keyBuffer);
                    }

                    // Si está comprimido (flag "1"), descomprimir
                    if (compressed == "1")
                    {
                        Console.WriteLine("  Descomprimiendo GZIP...");
                        encryptedData = Decompress(encryptedData);
                        Console.WriteLine($"  Descomprimido: {encryptedData.Length} bytes");
                    }

                    // Guardar
                    Console.WriteLine("  Guardando archivo...");
                    string outputPath = Path.Combine(outputDir, dllName);
                    File.WriteAllBytes(outputPath, encryptedData);
                    Console.WriteLine($"  Guardado: {outputPath}");

                    // Liberar buffer usando el puntero original
                    Console.WriteLine("  Liberando buffer...");
                    Marshal.FreeHGlobal(originalBuffer);

                    Console.WriteLine($"  Completado #{assemblyCount}");
                    assemblyCount++;
                }

                Console.WriteLine($"\n✓ {assemblyCount} assemblies desencriptados en: {outputDir}");
            }
        }

        static unsafe void EncodeACTR(string inputDir)
        {
            if (!Directory.Exists(inputDir))
            {
                Console.WriteLine($"Error: Directorio no encontrado: {inputDir}");
                return;
            }

            string outputPath = Path.Combine(Path.GetDirectoryName(inputDir) ?? ".", "ACTR_translated.dll");

            Console.WriteLine($"Encriptando assemblies desde {inputDir}...");

            // Lista de assemblies en orden
            string[] assemblies = { "hmitype.dll", "achmiface.dll", "rsapp.dll", "HMIFORM.dll", "TFTEDIT.dll", "TFTRUN.dll" };

            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                foreach (string dllName in assemblies)
                {
                    string dllPath = Path.Combine(inputDir, dllName);
                    if (!File.Exists(dllPath))
                    {
                        Console.WriteLine($"  Advertencia: {dllName} no encontrado, saltando...");
                        continue;
                    }

                    Console.WriteLine($"  Procesando {dllName}...");

                    // Leer DLL
                    byte[] dllData = File.ReadAllBytes(dllPath);

                    // TODO: Comprimir si es necesario
                    // Por ahora, sin comprimir
                    byte[] finalData = dllData;
                    string compressFlag = "0";

                    // Obtener índice del assembly
                    uint assemblyIndex = GetAssemblyIndex(dllName);

                    // Inicializar buffer de clave
                    IntPtr originalBuffer;
                    byte* keyBuffer = InitializeKeyBuffer(out originalBuffer);

                    // Encriptar usando AppDllPass_Encode
                    fixed (byte* dataPtr = finalData)
                    {
                        AppDllPass_Encode(dataPtr, (uint)finalData.Length);
                    }

                    // Liberar buffer
                    Marshal.FreeHGlobal(originalBuffer);

                    // Crear header: "nombre.dll#tamaño#comprimido"
                    string headerStr = $"{dllName}#{finalData.Length}#{compressFlag}";
                    byte[] header = new byte[64];
                    byte[] headerBytes = System.Text.Encoding.ASCII.GetBytes(headerStr);
                    Array.Copy(headerBytes, header, Math.Min(headerBytes.Length, 64));

                    // XOR header con 0x09
                    for (int i = 0; i < header.Length; i++)
                    {
                        header[i] ^= 9;
                    }

                    // Escribir al stream
                    bw.Write(header);
                    bw.Write(finalData);

                    Console.WriteLine($"    ✓ {dllName} encriptado ({finalData.Length} bytes)");
                }

                // Guardar ACTR.dll
                File.WriteAllBytes(outputPath, ms.ToArray());
                Console.WriteLine($"\n✓ ACTR.dll creado: {outputPath}");
                Console.WriteLine($"\nPara usar:");
                Console.WriteLine($"  1. Backup: copy \"C:\\Program Files (x86)\\USART HMI\\ACTR.dll\" ACTR.dll.backup");
                Console.WriteLine($"  2. Reemplazar: copy /Y \"{outputPath}\" \"C:\\Program Files (x86)\\USART HMI\\ACTR.dll\"");
                Console.WriteLine($"  3. Probar: \"C:\\Program Files (x86)\\USART HMI\\USART HMI.exe\"");
            }
        }

        static uint GetAssemblyIndex(string dllName)
        {
            // Lista de assemblies en orden (del código ApplicationRunMain.cs)
            string[] assemblies = { "hmitype", "achmiface", "rsapp", "HMIFORM", "TFTEDIT", "TFTRUN" };

            string name = Path.GetFileNameWithoutExtension(dllName);
            for (uint i = 0; i < assemblies.Length; i++)
            {
                if (assemblies[i].Equals(name, StringComparison.OrdinalIgnoreCase))
                    return i;
            }

            return 255; // Índice por defecto si no se encuentra
        }

        static unsafe byte* InitializeKeyBuffer(out IntPtr originalPointer)
        {
            // Asignar 1029 bytes (luego se alinea a múltiplo de 3)
            IntPtr hglobal = Marshal.AllocHGlobal(1029);
            originalPointer = hglobal; // Guardar el puntero original para FreeHGlobal
            byte* buffer = (byte*)hglobal;

            // Alinear a múltiplo de 3
            while ((int)buffer % 3 != 0)
                buffer++;

            // Constantes hardcoded del código ApplicationRunMain.cs:81-88
            uint[] constants = new uint[5]
            {
                334666603u,   // 0x13F3E5EB
                2954676568u,  // 0xB0292B58
                1723434224u,  // 0x66BC2CF0
                1368908345u,  // 0x51990B39
                3374053736u   // 0xC90D4CE8
            };

            // Inicializar con DateTime
            DateTime now = DateTime.Now;
            int seed = 1248987653 ^ now.Day ^ now.Hour ^ now.Minute ^ now.Second ^ now.Millisecond;
            Marshal.WriteInt32((IntPtr)buffer, 0, seed);

            long monthYear = now.Year * 12 + now.Month;
            Marshal.WriteInt64((IntPtr)buffer, 4, monthYear);
            Marshal.WriteInt32((IntPtr)buffer, 12, now.Year);
            Marshal.WriteInt32((IntPtr)buffer, 16, now.Month);
            Marshal.WriteInt32((IntPtr)buffer, 20, now.Day);
            Marshal.WriteInt32((IntPtr)buffer, 24, now.Hour);
            Marshal.WriteInt32((IntPtr)buffer, 28, now.Minute);
            Marshal.WriteInt32((IntPtr)buffer, 32, now.Second);

            // Llenar el resto con 0xFF
            for (int i = 36; i < 1024; i++)
            {
                buffer[i] = 0xFF;
            }

            // Aplicar XOR con constantes
            uint seed32 = *(uint*)buffer ^ 2541269850u;
            for (int i = 0; i < 5; i++)
            {
                constants[i] ^= seed32;
            }

            // Transformar primeros 32 bytes (algoritmo complejo)
            fixed (uint* constPtr = constants)
            {
                TransformKeyBuffer(buffer, constPtr);
            }

            return buffer;
        }

        static unsafe void TransformKeyBuffer(byte* buffer, uint* constants)
        {
            // Este es el algoritmo de las líneas 130-177 de ApplicationRunMain.cs
            // Es muy complejo, por ahora usamos una versión simplificada
            // En producción, necesitarías replicar exactamente el algoritmo

            byte* bufPtr = buffer + 4;
            int keySize = 32;
            int cycleSize = 8;

            uint* c0 = constants;
            uint* c1 = constants + 1;
            uint* c2 = constants + 2;
            byte* c3 = (byte*)(constants + 3);

            int adjustedSize = keySize - (keySize & 3);
            int cycleIndex = 0;
            int offset = 0;

            for (; offset < adjustedSize; offset += 4)
            {
                uint val = *(uint*)(bufPtr + offset);
                uint t1 = (uint)((int)*c0 + (int)*c1);
                uint t2 = (uint)((int)*c2 << ((int)c3[cycleIndex] + cycleSize & 3));
                uint combined = t1 + t2;
                uint result = (val ^ combined) + (combined + *c0);

                *c0 = *c0 << 1;
                byte* modPtr = (byte*)c0 + (offset + c3[cycleIndex] + cycleSize & 3);
                *modPtr = (byte)(*modPtr + (c3[cycleIndex] ^ 199));

                *(uint*)(bufPtr + offset) = result;

                cycleIndex++;
                if (cycleIndex == cycleSize)
                    cycleIndex = 0;
            }
        }

        static byte[] Decompress(byte[] data)
        {
            // ACTR usa algún tipo de compresión
            // Probablemente GZip o Deflate
            // Necesitarás identificar el formato específico
            try
            {
                using (var input = new MemoryStream(data))
                using (var output = new MemoryStream())
                using (var gzip = new System.IO.Compression.GZipStream(input, System.IO.Compression.CompressionMode.Decompress))
                {
                    gzip.CopyTo(output);
                    return output.ToArray();
                }
            }
            catch
            {
                // Si no es GZip, intentar Deflate
                try
                {
                    using (var input = new MemoryStream(data))
                    using (var output = new MemoryStream())
                    using (var deflate = new System.IO.Compression.DeflateStream(input, System.IO.Compression.CompressionMode.Decompress))
                    {
                        deflate.CopyTo(output);
                        return output.ToArray();
                    }
                }
                catch
                {
                    // No está comprimido o formato desconocido
                    return data;
                }
            }
        }
    }
}
