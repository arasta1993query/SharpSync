using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SharpSync.Core.Attributes;
using SharpSync.Core.Plugins;

namespace SharpSync.Core.Services
{
    public class AssemblyScanner : ITypeScanner
    {
        public IEnumerable<Type> Scan(string assemblyPath)
        {
            if (string.IsNullOrWhiteSpace(assemblyPath))
                throw new ArgumentException("Assembly path cannot be null or empty.", nameof(assemblyPath));

            try
            {
                // Note: In an MSBuild context, using MetadataLoadContext might be necessary to avoid locking.
                // For direct tooling, LoadFrom is often acceptable, but we handle typical load exceptions gracefully.
                var assembly = Assembly.LoadFrom(assemblyPath);

                var types = GetLoadableTypes(assembly)
                    .Where(t => t.GetCustomAttribute<SharpSyncAttribute>() != null || 
                                t.GetCustomAttributes(true).Any(a => a.GetType().Name == "SharpSyncHubAttribute"))
                    .ToList();

                return types;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to scan assembly '{assemblyPath}' for SharpSync artifacts: {ex.Message}", ex);
            }
        }

        private IEnumerable<Type> GetLoadableTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetExportedTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                // When some dependencies are missing, return the types that did load successfully.
                return e.Types.Where(t => t != null)!;
            }
        }
    }
}
