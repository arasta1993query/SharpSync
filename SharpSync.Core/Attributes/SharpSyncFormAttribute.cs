using System;

namespace SharpSync.Core.Attributes
{
    /// <summary>
    /// Mark a controller method to enable Zod schema generation for its associated DTOs.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class SharpSyncFormAttribute : Attribute
    {
    }
}
