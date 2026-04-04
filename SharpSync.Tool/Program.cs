using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using SharpSync.Core.Plugins;
using SharpSync.Core.Services;
using SharpSync.Core.Generators;

namespace SharpSync.Tool
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: dotnet sharpsync <AssemblyPath> [--output <OutputDirectory>] [--client <axios|fetch>]");
                return;
            }

            string assemblyPath = args[0];
            string outputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "SharpSyncGenerated");
            string clientType = "axios"; // default

            bool force = false;

            for (int i = 1; i < args.Length; i++)
            {
                if ((args[i] == "--output" || args[i] == "-o") && i + 1 < args.Length)
                {
                    outputDirectory = args[i + 1];
                    i++;
                }
                else if ((args[i] == "--client" || args[i] == "-c") && i + 1 < args.Length)
                {
                    clientType = args[i + 1].ToLower();
                    i++;
                }
                else if (args[i] == "--force" || args[i] == "-f")
                {
                    force = true;
                }
            }

            Console.WriteLine($"[SharpSync] Scanning assembly: {assemblyPath}");

            var services = new ServiceCollection();
            services.AddSingleton<ITypeScanner, AssemblyScanner>();
            services.AddSingleton<ICodeGenerator, TypeScriptCodeGenerator>();
            var provider = services.BuildServiceProvider();

            try
            {
                var scanner = provider.GetRequiredService<ITypeScanner>();
                var generator = provider.GetRequiredService<ICodeGenerator>();

                var types = scanner.Scan(assemblyPath);
                var generatedFiles = generator.Generate(types);

                if (!Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                }

                foreach (var fileEntry in generatedFiles)
                {
                    string fullPath = Path.Combine(outputDirectory, fileEntry.Key);
                    string directoryName = Path.GetDirectoryName(fullPath)!;
                    
                    if (!Directory.Exists(directoryName))
                    {
                        Directory.CreateDirectory(directoryName);
                    }

                    File.WriteAllText(fullPath, fileEntry.Value);
                }

                // Clean up legacy api.ts if it exists
                string oldApiPath = Path.Combine(outputDirectory, "api.ts");
                if (File.Exists(oldApiPath))
                {
                    File.Delete(oldApiPath);
                }

                Console.WriteLine($"[SharpSync] Successfully generated {generatedFiles.Count} TypeScript files into: {outputDirectory}");

                string apiClientPath = Path.Combine(outputDirectory, "apiClient.ts");
                if (!File.Exists(apiClientPath) || force)
                {
                    if (clientType == "fetch")
                    {
                        File.WriteAllText(apiClientPath, Scaffolds.GetFetchScaffold());
                    }
                    else
                    {
                        File.WriteAllText(apiClientPath, Scaffolds.GetAxiosScaffold());
                    }
                    string action = force && File.Exists(apiClientPath) ? "Overwriting" : "Scaffolding";
                    Console.WriteLine($"[SharpSync] {action} apiClient.ts ({clientType}) at: {apiClientPath}");
                }
                else
                {
                    Console.WriteLine($"[SharpSync] Skipping scaffolding of apiClient.ts because it already exists.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SharpSync] Error: {ex.Message}");
                Environment.ExitCode = 1;
            }
        }
    }
}
