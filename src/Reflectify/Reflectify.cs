﻿#if !REFLECTIFY_COMPILE
// <autogenerated />
#pragma warning disable
#endif

#nullable disable

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Reflectify;

public static class TypeExtensions
{
    private static readonly ConcurrentDictionary<(Type Type, MemberKind Kind), Reflector> ReflectorCache = new();

    /// <summary>
    /// Gets the public, internal, explicitly implemented and/or default properties of a type hierarchy.
    /// </summary>
    /// <param name="type">The type to reflect.</param>
    /// <param name="kind">The kind of properties to include in the response.</param>
    public static PropertyInfo[] GetProperties(this Type type, MemberKind kind)
    {
        return GetFor(type, kind).Properties;
    }

    /// <summary>
    /// Gets the public and/or internal fieldss of a type hierarchy.
    /// </summary>
    /// <param name="type">The type to reflect.</param>
    /// <param name="kind">The kind of fields to include in the response.</param>
    public static FieldInfo[] GetFields(this Type type, MemberKind kind)
    {
        return GetFor(type, kind).Fields;
    }

    /// <summary>
    /// Gets a combination of <see cref="GetProperties"/> and <see cref="GetFields"/>.
    /// </summary>
    /// <param name="type">The type to reflect.</param>
    /// <param name="kind">The kind of fields and properties to include in the response.</param>
    public static MemberInfo[] GetMembers(this Type type, MemberKind kind)
    {
        return GetFor(type, kind).Members;
    }


    private static Reflector GetFor(Type typeToReflect, MemberKind kind)
    {
        return ReflectorCache.GetOrAdd((typeToReflect, kind),
            static key => new Reflector(key.Type, key.Kind));
    }
}

public static class PropertyInfoExtensions
{
    /// <summary>
    /// Returns <c>true</c> if the property is an indexer, or <c>false</c> otherwise.
    /// </summary>
    public static bool IsIndexer(this PropertyInfo member)
    {
        return member.GetIndexParameters().Length != 0;
    }

    /// <summary>
    /// Returns <c>true</c> if the property is explicitly implemented on the
    /// <see cref="MemberInfo.DeclaringType"/>, or <c>false</c> otherwise.
    /// </summary>
    public static bool IsExplicitlyImplemented(this PropertyInfo prop)
    {
#if NETCOREAPP3_0_OR_GREATER
        return prop.Name.Contains('.', StringComparison.OrdinalIgnoreCase);
#else
        return prop.Name.Contains('.');
#endif
    }
}

/// <summary>
/// Defines the kinds of members you want to get when querying for the fields and properties of a type.
/// </summary>
[Flags]
public enum MemberKind
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
    private readonly HashSet<string> collectedPropertyNames = new();
    private readonly HashSet<string> collectedFields = new();
    private readonly List<FieldInfo> fields = new();
    private List<PropertyInfo> properties = new();

    public Reflector(Type typeToReflect, MemberKind kind)
    {
        LoadProperties(typeToReflect, kind);
        LoadFields(typeToReflect, kind);

        Members = properties.Concat<MemberInfo>(fields).ToArray();
    }

    private void LoadProperties(Type typeToReflect, MemberKind kind)
    {
        while (typeToReflect != null && typeToReflect != typeof(object))
        {
            var allProperties = typeToReflect.GetProperties(
                BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic);

            AddNormalProperties(kind, allProperties);

            AddExplicitlyImplementedProperties(kind, allProperties);

            AddInterfaceProperties(typeToReflect, kind);

            // Move to the base type
            typeToReflect = typeToReflect.BaseType;
        }

        properties = properties.Where(x => !x.IsIndexer()).ToList();
    }

    private void AddNormalProperties(MemberKind kind, PropertyInfo[] allProperties)
    {
        if (kind.HasFlag(MemberKind.Public) || kind.HasFlag(MemberKind.Internal) ||
            kind.HasFlag(MemberKind.ExplicitlyImplemented))
        {
            foreach (var property in allProperties)
            {
                if (!collectedPropertyNames.Contains(property.Name) && !property.IsExplicitlyImplemented() &&
                    HasVisibility(kind, property))
                {
                    properties.Add(property);
                    collectedPropertyNames.Add(property.Name);
                }
            }
        }
    }

    private static bool HasVisibility(MemberKind kind, PropertyInfo prop)
    {
        return (kind.HasFlag(MemberKind.Public) && prop.GetMethod?.IsPublic is true) ||
               (kind.HasFlag(MemberKind.Internal) &&
                (prop.GetMethod?.IsAssembly is true || prop.GetMethod?.IsFamilyOrAssembly is true));
    }

    private void AddExplicitlyImplementedProperties(MemberKind kind, PropertyInfo[] allProperties)
    {
        if (kind.HasFlag(MemberKind.ExplicitlyImplemented))
        {
            foreach (var p in allProperties)
            {
                if (p.IsExplicitlyImplemented())
                {
                    var name = p.Name.Split('.').Last();

                    if (!collectedPropertyNames.Contains(name))
                    {
                        properties.Add(p);
                        collectedPropertyNames.Add(name);
                    }
                }
            }
        }
    }

    private void AddInterfaceProperties(Type typeToReflect, MemberKind kind)
    {
        if (kind.HasFlag(MemberKind.DefaultInterfaceProperties) || typeToReflect.IsInterface)
        {
            // Add explicitly implemented interface properties (not included above)
            var interfaces = typeToReflect.GetInterfaces();

            foreach (var iface in interfaces)
            {
                foreach (var prop in iface.GetProperties())
                {
                    if (!collectedPropertyNames.Contains(prop.Name) &&
                        (!prop.GetMethod.IsAbstract || typeToReflect.IsInterface))
                    {
                        properties.Add(prop);
                        collectedPropertyNames.Add(prop.Name);
                    }
                }
            }
        }
    }

    private void LoadFields(Type typeToReflect, MemberKind kind)
    {
        while (typeToReflect != null && typeToReflect != typeof(object))
        {
            var files = typeToReflect.GetFields(
                BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var field in files)
            {
                if (!collectedFields.Contains(field.Name) && HasVisibility(kind, field))
                {
                    fields.Add(field);
                    collectedFields.Add(field.Name);
                }
            }

            // Move to the base type
            typeToReflect = typeToReflect.BaseType;
        }
    }

    private static bool HasVisibility(MemberKind kind, FieldInfo field)
    {
        return (kind.HasFlag(MemberKind.Public) && field.IsPublic) ||
               (kind.HasFlag(MemberKind.Internal) && (field.IsAssembly || field.IsFamilyOrAssembly));
    }

    public MemberInfo[] Members { get; }

    public PropertyInfo[] Properties => properties.ToArray();

    public FieldInfo[] Fields => fields.ToArray();
}
