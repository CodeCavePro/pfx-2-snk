using CommandLine;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace CodeCave.Utility.Windows.Pfx2Snk
{
    internal class Program
    {
        public static int Main(string[] args)
        {
            // Parse command line options and go out if they are invalid
            var options = new Options();
            var isValid = Parser.Default.ParseArgumentsStrict(args, options);
            if (!isValid)
            {
                return 1;
            }

            // Read private key from .pfx certificate
            byte[] snkContent;
            try
            {
                snkContent = GetPrivateKeyFromPfx(options.InputFile, options.Password);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 1;
            }

            // Create .snk path from .pfx file path if needed
            // ReSharper disable once InvertIf
            if (string.IsNullOrWhiteSpace(options.OutputFile))
            {
                var inputFolder = Directory.GetParent(options.InputFile);
                var inputFilename = Path.GetFileNameWithoutExtension(options.InputFile);
                options.OutputFile = Path.Combine(inputFolder.FullName, $"{inputFilename}.snk");
            }

            // Write out .snk file
            return WriteOutSnkFile(options.OutputFile, snkContent) ? 0 : 1;
        }

        /// <summary>
        /// Gets the private key from PFX.
        /// </summary>
        /// <param name="pfxFilePath">The PFX file path.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        /// <exception cref="InvalidDataException">
        /// The SNK file doesn't exist (please check if its path is correct: '{0}').
        /// or
        /// </exception>
        public static byte[] GetPrivateKeyFromPfx(string pfxFilePath, string password)
        {
            if (!File.Exists(pfxFilePath))
            {
                throw new InvalidDataException($"The SNK file doesn't exist (please check if its path is correct: '{pfxFilePath}').");
            }

            try
            {
                var pfxContent = File.ReadAllBytes(pfxFilePath);
                var pfxCert = new X509Certificate2(pfxContent, password, X509KeyStorageFlags.Exportable);
                var pfxPrivateKey = pfxCert.PrivateKey as RSACryptoServiceProvider;
                return pfxPrivateKey?.ExportCspBlob(true) ?? new byte[0];
            }
            catch (Exception ex)
            {
                throw new InvalidDataException($"Failed to parse PFX file because of the following error: {ex.Message}.", ex);
            }
        }

        /// <summary>
        /// Writes the out SNK file.
        /// </summary>
        /// <param name="snkFilePath">The SNK file path.</param>
        /// <param name="snkContent">Content of the SNK.</param>
        /// <returns></returns>
        public static bool WriteOutSnkFile(string snkFilePath, byte[] snkContent)
        {
            try
            {
                File.WriteAllBytes(snkFilePath, snkContent);
                return File.Exists(snkFilePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to write data to the following SNK file {snkFilePath} because of an error: {ex.Message}.");
                return false;
            }
        }
    }
}
