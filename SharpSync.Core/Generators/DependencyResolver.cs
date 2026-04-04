using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SharpSync.Core.Generators
{
    public class DependencyResolver
    {
        private readonly HashSet<Type> _zodEnabledTypes;

        public DependencyResolver(HashSet<Type> zodEnabledTypes)
        {
            _zodEnabledTypes = zodEnabledTypes;
        }

        public HashSet<Type> CollectTypesRecursive(IEnumerable<Type> initialTypes)
        {
            var discovered = new HashSet<Type>();
            var queue = new Queue<Type>(initialTypes);

            while (queue.Count > 0)
            {
                var type = queue.Dequeue();
                if (type == null || discovered.Contains(type) || TypeMapper.IsSystemType(type)) continue;

                discovered.Add(type);

                if (type.Name.EndsWith("Controller"))
                {
                    var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                    foreach (var m in methods)
                    {
                        var retType = GetUnwrappedReturnType(m.ReturnType);
                        EnqueueType(queue, retType, discovered);

                        foreach (var p in m.GetParameters())
                        {
                            EnqueueType(queue, p.ParameterType, discovered);
                        }
                    }
                }
                else
                {
                    foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        EnqueueType(queue, prop.PropertyType, discovered);
                    }
                }
            }

            return discovered;
        }

        public void MarkTypeAndDependenciesForZod(Type type)
        {
            if (type == null) return;

            var baseType = TypeMapper.GetBaseType(type);
            if (baseType == null || TypeMapper.IsSystemType(baseType) || _zodEnabledTypes.Contains(baseType)) return;

            _zodEnabledTypes.Add(baseType);

            if (!baseType.IsEnum)
            {
                foreach (var prop in baseType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    MarkTypeAndDependenciesForZod(prop.PropertyType);
                }
            }
        }

        public HashSet<Type> GetTypeDependencies(Type type)
        {
            var deps = new HashSet<Type>();
            if (type.IsEnum) return deps;

            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var propType = TypeMapper.GetBaseType(prop.PropertyType);
                if (propType != null && !TypeMapper.IsSystemType(propType) && propType != type)
                {
                    deps.Add(propType);
                }
            }
            return deps;
        }

        public HashSet<Type> GetControllerDependencies(Type controllerType)
        {
            var deps = new HashSet<Type>();
            var methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (var method in methods)
            {
                foreach (var param in method.GetParameters())
                {
                    var pType = TypeMapper.GetBaseType(param.ParameterType);
                    if (pType != null && !TypeMapper.IsSystemType(pType)) deps.Add(pType);
                }
                var retType = TypeMapper.GetBaseType(GetUnwrappedReturnType(method.ReturnType));
                if (retType != null && !TypeMapper.IsSystemType(retType)) deps.Add(retType);
            }
            return deps;
        }

        public Type GetUnwrappedReturnType(Type returnType)
        {
            if (returnType.IsGenericType && (returnType.Name.StartsWith("Task") || returnType.Name.StartsWith("ValueTask")))
            {
                return returnType.GetGenericArguments().FirstOrDefault() ?? typeof(void);
            }
            if (returnType.IsGenericType && returnType.Name.StartsWith("ActionResult"))
            {
                return returnType.GetGenericArguments().FirstOrDefault() ?? typeof(void);
            }
            return returnType;
        }

        private void EnqueueType(Queue<Type> queue, Type type, HashSet<Type> discovered)
        {
            if (type.IsGenericType)
            {
                foreach (var arg in type.GetGenericArguments())
                    EnqueueType(queue, arg, discovered);
                return;
            }

            if (type.IsArray)
            {
                EnqueueType(queue, type.GetElementType()!, discovered);
                return;
            }

            if (!TypeMapper.IsSystemType(type) && !discovered.Contains(type))
            {
                queue.Enqueue(type);
            }
        }
    }
}
