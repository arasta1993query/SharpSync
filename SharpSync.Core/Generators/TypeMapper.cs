using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace SharpSync.Core.Generators
{
    public static class TypeMapper
    {
        public static string MapCSharpToTypeScript(Type type)
        {
            if (type == typeof(string)) return "string";
            if (type == typeof(int) || type == typeof(long) || type == typeof(short) || type == typeof(byte) ||
                type == typeof(float) || type == typeof(double) || type == typeof(decimal)) return "number";
            if (type == typeof(bool)) return "boolean";
            if (type == typeof(DateTime) || type == typeof(DateTimeOffset) || type == typeof(DateOnly)) return "string";
            if (type == typeof(Guid)) return "string";
            if (type == typeof(void)) return "void";

            if (type.IsEnum) return type.Name;

            if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(IEnumerable<>) || 
                                       type.GetGenericTypeDefinition() == typeof(List<>) ||
                                       type.GetGenericTypeDefinition() == typeof(ICollection<>) ||
                                       type.GetGenericTypeDefinition() == typeof(IList<>)))
            {
                return MapCSharpToTypeScript(type.GetGenericArguments()[0]) + "[]";
            }

            if (type.IsArray)
            {
                return MapCSharpToTypeScript(type.GetElementType()!) + "[]";
            }

            // Handle Task<T> or ActionResult<T>
            if (type.IsGenericType && (type.Name.StartsWith("Task") || type.Name.StartsWith("ActionResult")))
            {
                return MapCSharpToTypeScript(type.GetGenericArguments()[0]);
            }

            Type? underlying = Nullable.GetUnderlyingType(type);
            if (underlying != null)
            {
                return MapCSharpToTypeScript(underlying) + " | null";
            }

            return type.Name == "Object" ? "any" : type.Name;
        }

        public static string MapCSharpToZod(Type type, HashSet<Type> zodEnabledTypes)
        {
            if (type == typeof(string)) return "z.string()";
            if (type == typeof(int) || type == typeof(long) || type == typeof(short) || type == typeof(byte)) return "z.number().int()";
            if (type == typeof(float) || type == typeof(double) || type == typeof(decimal)) return "z.number()";
            if (type == typeof(bool)) return "z.boolean()";
            if (type == typeof(DateTime) || type == typeof(DateTimeOffset) || type == typeof(DateOnly)) return "z.string().datetime()";
            if (type == typeof(Guid)) return "z.string().uuid()";

            if (type.IsEnum) return $"z.nativeEnum({type.Name})";

            if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(IEnumerable<>) || 
                                       type.GetGenericTypeDefinition() == typeof(List<>) ||
                                       type.GetGenericTypeDefinition() == typeof(ICollection<>) ||
                                       type.GetGenericTypeDefinition() == typeof(IList<>)))
            {
                return $"z.array({MapCSharpToZod(type.GetGenericArguments()[0], zodEnabledTypes)})";
            }

            if (type.IsArray)
            {
                return $"z.array({MapCSharpToZod(type.GetElementType()!, zodEnabledTypes)})";
            }

            Type? underlying = Nullable.GetUnderlyingType(type);
            if (underlying != null)
            {
                return MapCSharpToZod(underlying, zodEnabledTypes) + ".nullable()";
            }

            if (zodEnabledTypes.Contains(type))
            {
                return $"z.lazy(() => {type.Name}Schema)";
            }

            return "z.any()";
        }

        public static List<string> GetZodRules(PropertyInfo prop)
        {
            var rules = new List<string>();
            foreach (var attr in prop.GetCustomAttributes(true))
            {
                var attrType = attr.GetType();
                if (attrType.Name == "RequiredAttribute") rules.Add(".min(1)");
                else if (attrType.Name == "StringLengthAttribute")
                {
                    var max = attrType.GetProperty("MaximumLength")?.GetValue(attr);
                    var min = attrType.GetProperty("MinimumLength")?.GetValue(attr);
                    if (max != null) rules.Add($".max({max})");
                    if (min != null && (int)min > 0) rules.Add($".min({min})");
                }
                else if (attrType.Name == "MaxLengthAttribute")
                {
                    var max = attrType.GetProperty("Length")?.GetValue(attr);
                    if (max != null) rules.Add($".max({max})");
                }
                else if (attrType.Name == "MinLengthAttribute")
                {
                    var min = attrType.GetProperty("Length")?.GetValue(attr);
                    if (min != null) rules.Add($".min({min})");
                }
                else if (attrType.Name == "RangeAttribute")
                {
                    var min = attrType.GetProperty("Minimum")?.GetValue(attr);
                    var max = attrType.GetProperty("Maximum")?.GetValue(attr);
                    if (min != null) rules.Add($".min({min})");
                    if (max != null) rules.Add($".max({max})");
                }
                else if (attrType.Name == "EmailAddressAttribute") rules.Add(".email()");
                else if (attrType.Name == "UrlAttribute") rules.Add(".url()");
                else if (attrType.Name == "RegularExpressionAttribute")
                {
                    var pattern = attrType.GetProperty("Pattern")?.GetValue(attr)?.ToString();
                    if (pattern != null) rules.Add($".regex(new RegExp('{pattern.Replace("'", "\\'")}'))");
                }
            }
            return rules;
        }

        public static bool IsSimpleType(Type type)
        {
            Type? underlying = Nullable.GetUnderlyingType(type);
            if (underlying != null) type = underlying;

            return type.IsPrimitive || 
                   type == typeof(string) || 
                   type == typeof(decimal) || 
                   type == typeof(DateTime) || 
                   type == typeof(DateTimeOffset) || 
                   type == typeof(DateOnly) || 
                   type == typeof(TimeOnly) || 
                   type == typeof(Guid) ||
                   type.IsEnum;
        }

        public static bool IsSystemType(Type type)
        {
            return type.Namespace != null && (type.Namespace.StartsWith("System") || type.Namespace.StartsWith("Microsoft"));
        }

        public static Type? GetBaseType(Type type)
        {
            if (type == null) return null;
            if (type.IsArray) return GetBaseType(type.GetElementType()!);
            
            Type? underlying = Nullable.GetUnderlyingType(type);
            if (underlying != null) return GetBaseType(underlying);

            if (type.IsGenericType)
            {
                return GetBaseType(type.GetGenericArguments()[0]);
            }
            return type;
        }

        public static string CamelCase(string s)
        {
            if (string.IsNullOrEmpty(s) || !char.IsUpper(s[0])) return s;
            return char.ToLower(s[0]) + s.Substring(1);
        }
    }
}
