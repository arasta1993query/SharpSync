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
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: dotnet sharpsync <AssemblyPath> <OutputFilePath>");
                return;
            }

            string assemblyPath = args[0];
            string outputPath = args[1];

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
                
                string tsCode = generator.Generate(types);

                var dir = Path.GetDirectoryName(outputPath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                File.WriteAllText(outputPath, tsCode);

                Console.WriteLine($"[SharpSync] Successfully generated TypeScript into: {outputPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SharpSync] Error: {ex.Message}");
                Environment.ExitCode = 1;
            }
        }
    }
}
