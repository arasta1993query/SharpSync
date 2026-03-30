using Microsoft.Extensions.DependencyInjection;

namespace SharpSync.Core.Plugins
{
    /// <summary>
    /// Represents a plugin for the SharpSync generator pipeline.
    /// Used to register custom type scanners, generators, and other services.
    /// </summary>
    public interface ISharpSyncPlugin
    {
        void ConfigureServices(IServiceCollection services);
    }
}
