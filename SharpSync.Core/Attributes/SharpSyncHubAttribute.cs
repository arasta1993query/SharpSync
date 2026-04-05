using System;

namespace SharpSync.Core.Attributes
{
    /// <summary>
    /// Explicitly marks a SignalR Hub for synchronization into a strongly-typed TypeScript client.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SharpSyncHubAttribute : Attribute
    {
        /// <summary>
        /// Optional namespace or module segregation for the generated TypeScript files.
        /// </summary>
        public string? ModuleName { get; set; }
    }
}
