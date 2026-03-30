using System;
using System.Collections.Generic;

namespace SharpSync.Core.Plugins
{
    /// <summary>
    /// Defines a service responsible for locating and yielding types 
    /// from an assembly that should be processed by SharpSync.
    /// </summary>
    public interface ITypeScanner
    {
        /// <summary>
        /// Scans the given assembly path for relevant types.
        /// </summary>
        /// <param name="assemblyPath">The physical path to the compiled assembly (.dll)</param>
        /// <returns>A collection of types marked for synchronization.</returns>
        IEnumerable<Type> Scan(string assemblyPath);
    }
}
