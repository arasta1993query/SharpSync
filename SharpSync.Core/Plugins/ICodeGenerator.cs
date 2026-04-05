using System;
using System.Collections.Generic;

namespace SharpSync.Core.Plugins
{
    /// <summary>
    /// Defines a service responsible for translating discovered C# types 
    /// into TypeScript definitions or TanStack Query hooks.
    /// </summary>
    public interface ICodeGenerator
    {
        /// <summary>
        /// Generates the output source code for the given types using the specified framework.
        /// </summary>
        /// <param name="types">The C# types discovered by the scanner.</param>
        /// <param name="framework">The target frontend framework.</param>
        /// <returns>A dictionary containing relative file paths and their generated content.</returns>
        IDictionary<string, string> Generate(IEnumerable<Type> types, Generators.FrameworkType framework = Generators.FrameworkType.React);
    }
}
