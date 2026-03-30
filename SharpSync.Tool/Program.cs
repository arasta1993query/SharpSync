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

            for (int i = 1; i < args.Length; i++)
            {
                if (args[i] == "--output" && i + 1 < args.Length)
                {
                    outputDirectory = args[i + 1];
                    i++;
                }
                else if (args[i] == "--client" && i + 1 < args.Length)
                {
                    clientType = args[i + 1].ToLower();
                    i++;
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
                string tsCode = generator.Generate(types);

                if (!Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                }

                string apiPath = Path.Combine(outputDirectory, "api.ts");
                File.WriteAllText(apiPath, tsCode);
                Console.WriteLine($"[SharpSync] Successfully generated TypeScript into: {apiPath}");

                string apiClientPath = Path.Combine(outputDirectory, "apiClient.ts");
                if (!File.Exists(apiClientPath))
                {
                    if (clientType == "fetch")
                    {
                        File.WriteAllText(apiClientPath, GetFetchScaffold());
                    }
                    else
                    {
                        File.WriteAllText(apiClientPath, GetAxiosScaffold());
                    }
                    Console.WriteLine($"[SharpSync] Scaffolding missing apiClient.ts ({clientType}) at: {apiClientPath}");
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

        private static string GetAxiosScaffold()
        {
            return @"import axios, { AxiosRequestConfig } from 'axios';

// Create a globally configurable axios instance (add auth headers or interceptors here)
const axiosInstance = axios.create({
    baseURL: process.env.NEXT_PUBLIC_API_URL || '',
    headers: {
        'Content-Type': 'application/json',
    },
});

export const apiRequest = async <T>(
    url: string,
    method: 'GET' | 'POST' | 'PUT' | 'DELETE' | 'PATCH',
    body?: any,
    options?: AxiosRequestConfig
): Promise<T> => {
    const response = await axiosInstance.request<T>({
        url,
        method,
        data: body,
        ...options,
    });
    return response.data;
};
";
        }

        private static string GetFetchScaffold()
        {
            return @"// Generic fetch wrapper for React Query
const BASE_URL = process.env.NEXT_PUBLIC_API_URL || '';

export const apiRequest = async <T>(
    url: string,
    method: 'GET' | 'POST' | 'PUT' | 'DELETE' | 'PATCH',
    body?: any,
    options?: RequestInit
): Promise<T> => {
    const response = await fetch(`${BASE_URL}/${url}`, {
        method,
        headers: {
            'Content-Type': 'application/json',
            ...(options?.headers || {}),
        },
        body: body ? JSON.stringify(body) : undefined,
        ...options,
    });

    if (!response.ok) {
        throw new Error(`API Request failed: ${response.statusText}`);
    }

    // Try parsing JSON if content exists
    try {
        const text = await response.text();
        return text ? JSON.parse(text) : ({} as T);
    } catch {
        return {} as T;
    }
};
";
        }
    }
}
