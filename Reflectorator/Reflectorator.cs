using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Reflectorator
{
    public static class TypeExtensions
    {
    }
    
    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum MemberSelection
    {
        Public = 1,
        Internal = 2,
        ExplicitlyImplemented = 4,
        DefaultInterfaceProperties = 8
    }

    /// <summary>
    /// Helper class to get all the public and internal fields and properties from a type.
    /// </summary>
    internal sealed class Reflector
    {
        private readonly HashSet<string> collectedPropertyNames = new HashSet<string>();
        private readonly HashSet<string> collectedFields = new HashSet<string>();
        private readonly List<FieldInfo> fields = new List<FieldInfo>();
        private List<PropertyInfo> properties = new List<PropertyInfo>();

        public Reflector(Type typeToReflect, MemberSelection selection)
        {
            LoadProperties(typeToReflect, selection);
            LoadFields(typeToReflect, selection);

            Members = properties.Concat<MemberInfo>(fields).ToArray();
        }

#pragma warning disable MA0051
        private void LoadProperties(Type typeToReflect, MemberSelection selection)
#pragma warning restore MA0051
        {
            while (typeToReflect != null && typeToReflect != typeof(object))
            {
                var allProperties = typeToReflect.GetProperties(
                    BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic);

                AddNormalProperties(selection, allProperties);

                AddExplicitlyImplementedProperties(selection, allProperties);

                AddInterfaceProperties(typeToReflect, selection);

                // Move to the base type
                typeToReflect = typeToReflect.BaseType;
            }

            properties = properties.Where(x => !x.IsIndexer()).ToList();
        }

        private void AddNormalProperties(MemberSelection selection, PropertyInfo[] allProperties)
        {
            if (selection.HasFlag(MemberSelection.Public) || selection.HasFlag(MemberSelection.Internal) ||
                selection.HasFlag(MemberSelection.ExplicitlyImplemented))
                foreach (var property in allProperties)
                    if (!collectedPropertyNames.Contains(property.Name) && !IsExplicitlyImplemented(property) &&
                        HasVisibility(selection, property))
                    {
                        properties.Add(property);
                        collectedPropertyNames.Add(property.Name);
                    }
        }

        private static bool HasVisibility(MemberSelection selection, PropertyInfo prop)
        {
            return (selection.HasFlag(MemberSelection.Public) && prop.GetMethod?.IsPublic is true) ||
                   (selection.HasFlag(MemberSelection.Internal) &&
                    (prop.GetMethod?.IsAssembly is true || prop.GetMethod?.IsFamilyOrAssembly is true));
        }

        private void AddExplicitlyImplementedProperties(MemberSelection selection, PropertyInfo[] allProperties)
        {
            if (selection.HasFlag(MemberSelection.ExplicitlyImplemented))
                foreach (var p in allProperties)
                    if (IsExplicitlyImplemented(p))
                    {
                        var name = p.Name.Split('.').Last();

                        if (!collectedPropertyNames.Contains(name))
                        {
                            properties.Add(p);
                            collectedPropertyNames.Add(name);
                        }
                    }
        }

        private void AddInterfaceProperties(Type typeToReflect, MemberSelection selection)
        {
            if (selection.HasFlag(MemberSelection.DefaultInterfaceProperties) || typeToReflect.IsInterface)
            {
                // Add explicitly implemented interface properties (not included above)
                var interfaces = typeToReflect.GetInterfaces();

                foreach (var iface in interfaces)
                foreach (var prop in iface.GetProperties())
                    if (!collectedPropertyNames.Contains(prop.Name) &&
                        (!prop.GetMethod!.IsAbstract || typeToReflect.IsInterface))
                    {
                        properties.Add(prop);
                        collectedPropertyNames.Add(prop.Name);
                    }
            }
        }

        private static bool IsExplicitlyImplemented(PropertyInfo prop)
        {
            return prop.Name.Contains('.');
        }

        private void LoadFields(Type typeToReflect, MemberSelection selection)
        {
            while (typeToReflect != null && typeToReflect != typeof(object))
            {
                var files = typeToReflect.GetFields(
                    BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic);

                foreach (var field in files)
                    if (!collectedFields.Contains(field.Name) && HasVisibility(selection, field))
                    {
                        fields.Add(field);
                        collectedFields.Add(field.Name);
                    }

                // Move to the base type
                typeToReflect = typeToReflect.BaseType;
            }
        }

        private static bool HasVisibility(MemberSelection selection, FieldInfo field)
        {
            return (selection.HasFlag(MemberSelection.Public) && field.IsPublic) ||
                   (selection.HasFlag(MemberSelection.Internal) && (field.IsAssembly || field.IsFamilyOrAssembly));
        }

        public MemberInfo[] Members { get; }

        public PropertyInfo[] Properties => properties.ToArray();

        public FieldInfo[] Fields => fields.ToArray();
    }
}