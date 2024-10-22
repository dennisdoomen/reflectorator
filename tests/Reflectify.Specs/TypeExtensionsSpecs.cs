using System.Globalization;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace Reflectify.Specs;

public class TypeExtensionsSpecs
{
    public class GetProperties
    {
        [Fact]
        public void Can_get_all_public_explicit_and_default_interface_properties()
        {
            // Act
            var properties = typeof(SuperClass).GetProperties(
                MemberKind.Public | MemberKind.ExplicitlyImplemented | MemberKind.DefaultInterfaceProperties);

            // Assert
            properties.Should().BeEquivalentTo(new[]
            {
                new { Name = "NormalProperty", PropertyType = typeof(string) },
                new { Name = "NewProperty", PropertyType = typeof(int) },
                new { Name = "InterfaceProperty", PropertyType = typeof(string) },
                new
                {
                    Name = $"{typeof(IInterfaceWithSingleProperty).FullName!.Replace("+", ".")}.ExplicitlyImplementedProperty",
                    PropertyType = typeof(string)
                },
#if NETCOREAPP3_0_OR_GREATER
                new { Name = "DefaultProperty", PropertyType = typeof(string) }
#endif
            });
        }

        [Fact]
        public void Can_get_all_properties_from_an_interface()
        {
            // Act
            var properties = typeof(IInterfaceWithDefaultProperty).GetProperties(MemberKind.Public);

            // Assert
            properties.Should().BeEquivalentTo(new[]
            {
                new { Name = "InterfaceProperty", PropertyType = typeof(string) },
                new { Name = "ExplicitlyImplementedProperty", PropertyType = typeof(string) },
#if NETCOREAPP3_0_OR_GREATER
                new { Name = "DefaultProperty", PropertyType = typeof(string) },
#endif
            });
        }

        [Fact]
        public void Can_get_normal_public_properties()
        {
            // Act
            var properties = typeof(SuperClass).GetProperties(MemberKind.Public);

            // Assert
            properties.Should().BeEquivalentTo(new[]
            {
                new { Name = "NormalProperty", PropertyType = typeof(string) },
                new { Name = "NewProperty", PropertyType = typeof(int) },
                new { Name = "InterfaceProperty", PropertyType = typeof(string) }
            });
        }

        [Fact]
        public void Can_get_explicit_properties_only()
        {
            // Act
            var properties = typeof(SuperClass).GetProperties(MemberKind.ExplicitlyImplemented);

            // Assert
            properties.Should().BeEquivalentTo(new[]
            {
                new
                {
                    Name =
                        $"{typeof(IInterfaceWithSingleProperty).FullName!.Replace("+", ".")}.ExplicitlyImplementedProperty",
                    PropertyType = typeof(string)
                }
            });
        }

        [Fact]
        public void Prefers_normal_property_over_explicitly_implemented_one()
        {
            // Act
            var properties = typeof(ClassWithExplicitAndNormalProperty).GetProperties(
                MemberKind.Public | MemberKind.ExplicitlyImplemented);

            // Assert
            properties.Should().BeEquivalentTo(new[]
            {
                new
                {
                    Name = "ExplicitlyImplementedProperty",
                    PropertyType = typeof(int)
                }
            });
        }

#if NETCOREAPP3_0_OR_GREATER
        [Fact]
        public void Can_get_default_interface_properties_only()
        {
            // Act
            var properties = typeof(SuperClass).GetProperties(MemberKind.DefaultInterfaceProperties);

            // Assert
            properties.Should().BeEquivalentTo(new[]
            {
                new { Name = "DefaultProperty", PropertyType = typeof(string) }
            });
        }
#endif

        [Fact]
        public void Can_get_internal_properties()
        {
            // Act
            var properties = typeof(SuperClass).GetProperties(MemberKind.Internal);

            // Assert
            properties.Should().BeEquivalentTo(new[]
            {
                new { Name = "InternalProperty", PropertyType = typeof(bool) },
                new { Name = "InternalProtectedProperty", PropertyType = typeof(bool) }
            });
        }

        [Fact]
        public void Will_ignore_indexers()
        {
            // Act
            var properties = typeof(ClassWithIndexer).GetProperties(MemberKind.Public);

            // Assert
            properties.Should().BeEquivalentTo(new[]
            {
                new { Name = "Foo", PropertyType = typeof(object) }
            });
        }
    }

    public class GetFields
    {
        [Fact]
        public void Can_find_public_fields()
        {
            // Act
            var fields = typeof(SuperClass).GetFields(MemberKind.Public);

            // Assert
            fields.Should().BeEquivalentTo(new[]
            {
                new { Name = "NormalField", FieldType = typeof(string) }
            });
        }

        [Fact]
        public void Can_find_internal_fields()
        {
            // Act
            var fields = typeof(SuperClass).GetFields(MemberKind.Internal);

            // Assert
            fields.Should().BeEquivalentTo(new[]
            {
                new { Name = "InternalField", FieldType = typeof(string) },
                new { Name = "ProtectedInternalField", FieldType = typeof(string) }
            });
        }

        [Fact]
        public void Can_find_all_fields()
        {
            // Act
            var fields = typeof(SuperClass).GetFields(
                MemberKind.Internal | MemberKind.Public);

            // Assert
            fields.Should().BeEquivalentTo(new[]
            {
                new { Name = "NormalField", FieldType = typeof(string) },
                new { Name = "InternalField", FieldType = typeof(string) },
                new { Name = "ProtectedInternalField", FieldType = typeof(string) }
            });
        }
    }

    public class GetMembers
    {
        [Fact]
        public void Can_find_all_members()
        {
            // Act
            var members = typeof(SuperClass).GetMembers(MemberKind.Public);

            // Assert
            members.Should().BeEquivalentTo([
                new { Name = "NormalProperty" },
                new { Name = "NewProperty" },
                new { Name = "InterfaceProperty" },
                new { Name = "NormalField" }
            ]);
        }
    }

    public class PropertyInfoExtensions
    {
        [Fact]
        public void Can_determine_a_property_is_an_indexer()
        {
            // Act
            var indexer = typeof(ClassWithIndexer).GetProperty("Item");

            // Assert
            indexer.IsIndexer().Should().BeTrue();
        }

        [Fact]
        public void Can_determine_a_property_is_not_an_indexer()
        {
            // Act
            var indexer = typeof(ClassWithIndexer).GetProperty("Foo");

            // Assert
            indexer.IsIndexer().Should().BeFalse();
        }
    }

    private class SuperClass : BaseClass, IInterfaceWithDefaultProperty
    {
        public string NormalProperty { get; set; }

        public new int NewProperty { get; set; }

        internal bool InternalProperty { get; set; }

        protected internal bool InternalProtectedProperty { get; set; }

        string IInterfaceWithSingleProperty.ExplicitlyImplementedProperty { get; set; }

        public string InterfaceProperty { get; set; }

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
        public string NormalField;

        internal string InternalField;

        protected internal string ProtectedInternalField;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value
    }

    private sealed class ClassWithExplicitAndNormalProperty : IInterfaceWithSingleProperty
    {
        string IInterfaceWithSingleProperty.ExplicitlyImplementedProperty { get; set; }

        [UsedImplicitly]
        public int ExplicitlyImplementedProperty { get; set; }
    }

    private class BaseClass
    {
        [UsedImplicitly]
        public string NewProperty { get; set; }
    }

    private interface IInterfaceWithDefaultProperty : IInterfaceWithSingleProperty
    {
        [UsedImplicitly]
        string InterfaceProperty { get; set; }

#if NETCOREAPP3_0_OR_GREATER
        [UsedImplicitly]
        string DefaultProperty => "Default";
#endif
    }

    private interface IInterfaceWithSingleProperty
    {
        [UsedImplicitly]
        string ExplicitlyImplementedProperty { get; set; }
    }

    private sealed class ClassWithIndexer
    {
        [UsedImplicitly]
        public object Foo { get; set; }

        public string this[int n] => n.ToString(CultureInfo.InvariantCulture);
    }
}
