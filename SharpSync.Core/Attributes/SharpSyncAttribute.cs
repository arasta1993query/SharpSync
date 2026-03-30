using System;

namespace SharpSync.Core.Attributes
{
    /// <summary>
    /// Mark classes, structs, or enums to be dynamically synchronized into TypeScript.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
    public class SharpSyncAttribute : Attribute
    {
        /// <summary>
        /// Optional namespace or module segregation for the generated TypeScript files.
        /// </summary>
        public string? ModuleName { get; set; }
    }
}
