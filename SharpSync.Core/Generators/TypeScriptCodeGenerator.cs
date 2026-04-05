using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using SharpSync.Core.Plugins;

namespace SharpSync.Core.Generators
{
    public class TypeScriptCodeGenerator : ICodeGenerator
    {
        private readonly HashSet<Type> _zodEnabledTypes = new HashSet<Type>();
        private readonly DependencyResolver _dependencyResolver;
        private readonly ModelGenerator _modelGenerator;
        private readonly HookGenerator _hookGenerator;
        private readonly HubGenerator _hubGenerator;

        public TypeScriptCodeGenerator()
        {
            _dependencyResolver = new DependencyResolver(_zodEnabledTypes);
            _modelGenerator = new ModelGenerator(_zodEnabledTypes);
            _hookGenerator = new HookGenerator(_zodEnabledTypes);
            _hubGenerator = new HubGenerator(_zodEnabledTypes);
        }

        public IDictionary<string, string> Generate(IEnumerable<Type> types, FrameworkType framework = FrameworkType.React)
        {
            var discoveredTypes = _dependencyResolver.CollectTypesRecursive(types);
            var result = new Dictionary<string, string>();

            // 1. Discover Zod-enabled types
            _zodEnabledTypes.Clear();
            var controllers = discoveredTypes.Where(t => t.Name.EndsWith("Controller")).ToList();
            foreach (var controller in controllers)
            {
                var methods = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                foreach (var method in methods)
                {
                    if (method.GetCustomAttributes(true).Any(a => a.GetType().Name == "SharpSyncFormAttribute"))
                    {
                        foreach (var param in method.GetParameters())
                        {
                            _dependencyResolver.MarkTypeAndDependenciesForZod(param.ParameterType);
                        }
                        _dependencyResolver.MarkTypeAndDependenciesForZod(_dependencyResolver.GetUnwrappedReturnType(method.ReturnType));
                    }
                }
            }

            // 2. Generate Model Files
            var models = discoveredTypes.Where(t => !t.Name.EndsWith("Controller") && !t.IsGenericTypeDefinition).ToList();
            // Also include generic definitions that were collected
            var genericDefs = discoveredTypes.Where(t => t.IsGenericTypeDefinition).ToList();
            
            foreach (var model in models.Concat(genericDefs))
            {
                var content = _modelGenerator.GenerateModelFile(model, _dependencyResolver);
                var fileName = model.IsGenericTypeDefinition ? model.Name.Split('`')[0] : model.Name;
                result.Add($"models/{fileName}.ts", content);
            }

            // 3. Generate Controller Files
            foreach (var controller in controllers)
            {
                var content = _hookGenerator.GenerateHookFile(controller, _dependencyResolver, framework);
                string fileName = controller.Name.Replace("Controller", "") + "Hooks.ts";
                result.Add($"hooks/{fileName}", content);
            }

            // 4. Generate Hub Files
            var hubs = discoveredTypes.Where(t => t.GetCustomAttributes(true).Any(a => a.GetType().Name == "SharpSyncHubAttribute")).ToList();
            foreach (var hub in hubs)
            {
                var content = _hubGenerator.GenerateHubFile(hub, _dependencyResolver, framework);
                string fileName = hub.Name.Replace("Hub", "") + "Hub.ts";
                result.Add($"hubs/{fileName}", content);
            }

            return result;
        }
    }
}
