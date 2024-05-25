using System.Diagnostics;
using System.Security.Cryptography;

namespace Laboratorium3.Zadanie2.NET
{
    public class Program
    {
        public static void Main()
        {
            var algorithms = new List<SymmetricAlgorithm>
            {
                new AesCryptoServiceProvider { KeySize = 128 },
                new AesCryptoServiceProvider { KeySize = 256 },
                new AesManaged { KeySize = 128 },
                new AesManaged { KeySize = 256 },
                new RijndaelManaged { KeySize = 128 },
                new RijndaelManaged { KeySize = 256 },
                new DESCryptoServiceProvider(),
                new TripleDESCryptoServiceProvider()
            };

            var algorithmNames = new List<string>
            {
                "AES (CSP) 128 bit",
                "AES (CSP) 256 bit",
                "AES Managed 128 bit",
                "AES Managed 256 bit",
                "Rindeal Managed 128 bit",
                "Rindeal Managed 256 bit",
                "DES 56 bit",
                "3DES 168 bit"
            };

            var blockSizeBytes = 1024 * 1024;
            var data = new byte[blockSizeBytes];
            new Random().NextBytes(data);

            Console.WriteLine("Algorytm\t\tCzas na blok (s)\tBajty/sek (RAM)\tBajty/sek (HDD)");

            for (var i = 0; i < algorithms.Count; i++)
            {
                var algorithm = algorithms[i];
                using var encryptor = algorithm.CreateEncryptor();

                var stopwatchBlock = Stopwatch.StartNew();
                encryptor.TransformFinalBlock(data, 0, blockSizeBytes);
                stopwatchBlock.Stop();
                var timePerBlock = stopwatchBlock.Elapsed.TotalSeconds;

                var stopwatchMemory = Stopwatch.StartNew();
                for (var j = 0; j < 100; j++)
                {
                    encryptor.TransformFinalBlock(data, 0, blockSizeBytes);
                }
                stopwatchMemory.Stop();
                var bytesPerSecondRam = 100 * blockSizeBytes / stopwatchMemory.Elapsed.TotalSeconds;
                    
                const string filePath = "temp.dat";
                File.WriteAllBytes(filePath, data);
                var stopwatchDisk = Stopwatch.StartNew();
                for (var j = 0; j < 10; j++)
                {
                    using var fileStream = File.OpenRead(filePath);
                    using var cryptoStream = new CryptoStream(fileStream, encryptor, CryptoStreamMode.Read);
                    using var memoryStream = new MemoryStream();
                    cryptoStream.CopyTo(memoryStream);
                }
                stopwatchDisk.Stop();
                var bytesPerSecondHdd = 10 * blockSizeBytes / stopwatchDisk.Elapsed.TotalSeconds;
                File.Delete(filePath);

                Console.WriteLine($"{algorithmNames[i]}\t{timePerBlock}\t\t{bytesPerSecondRam}\t\t{bytesPerSecondHdd}");
            }
        }
    }
}
