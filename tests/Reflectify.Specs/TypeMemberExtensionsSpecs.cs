using System;
using System.Globalization;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace Reflectify.Specs;

public class TypeMemberExtensionsSpecs
{
    public class GetPropertiesAndFields
    {
        [Fact]
        public void Can_get_all_public_explicit_and_default_instance_interface_properties()
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
                    Name =
                        $"{typeof(IInterfaceWithSingleProperty).FullName!.Replace("+", ".")}.ExplicitlyImplementedProperty",
                    PropertyType = typeof(string)
                },
#if NETCOREAPP3_0_OR_GREATER
                new { Name = "DefaultProperty", PropertyType = typeof(string) }
#endif
            });
        }

        [Fact]
        public void Can_get_all_public_static_properties()
        {
            // Act
            var properties = typeof(SuperClass).GetProperties(
                MemberKind.Public | MemberKind.Static);

            // Assert
            properties.Should().BeEquivalentTo([
                new { Name = "StaticProperty", PropertyType = typeof(bool) }
            ]);
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
                new { Name = "ExplicitlyImplementedProperty", PropertyType = typeof(int) }
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
        public void Can_get_write_only_properties()
        {
            // Act
            var properties = typeof(ClassImplementingSetterOnlyInterface)
                .GetProperties(MemberKind.Public | MemberKind.DefaultInterfaceProperties | MemberKind.ExplicitlyImplemented);

            // Assert
            properties.Should().BeEquivalentTo([
                new { Name = "WriteOnlyProperty", PropertyType = typeof(string) }
            ]);
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

        [Fact]
        public void Supports_returning_no_properties_if_asked_for()
        {
            // Act
            var properties = typeof(ClassWithIndexer).GetProperties(MemberKind.None);

            // Assert
            properties.Should().BeEmpty();
        }

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

        [Fact]
        public void Supports_returning_no_fields_if_asked_for()
        {
            // Act
            var properties = typeof(ClassWithIndexer).GetFields(MemberKind.None);

            // Assert
            properties.Should().BeEmpty();
        }

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
                new { Name = "NormalField" },
            ]);
        }
    }

    public class FindProperty
    {
        [Fact]
        public void Can_find_a_normal_property()
        {
            // Act
            var property = typeof(SuperClass).FindProperty("NormalProperty", MemberKind.Public);

            // Assert
            property.Should().NotBeNull().And.Return<string>();
        }

        [Fact]
        public void Cannot_find_a_property_if_it_does_not_exist()
        {
            // Act
            var property = typeof(SuperClass).FindProperty("NonExistingProperty", MemberKind.Public);

            // Assert
            property.Should().BeNull();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void A_property_name_is_required(string propertyName)
        {
            // Act
            var act = () => typeof(SuperClass).FindProperty(propertyName, MemberKind.Public);

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("*property name*");
        }

        [Fact]
        public void Can_find_a_property_that_hides_its_base_class_name_sake()
        {
            // Act
            var property = typeof(SuperClass).FindProperty("NewProperty", MemberKind.Public);

            // Assert
            property.Should().NotBeNull().And.Return<int>();
        }

        [Fact]
        public void Can_find_a_property_that_is_hidden_by_a_superclass_provided_you_refer_to_the_baseclass()
        {
            // Act
            var property = typeof(BaseClass).FindProperty("NewProperty", MemberKind.Public);

            // Assert
            property.Should().NotBeNull().And.Return<string>();
        }

        [Fact]
        public void Cannot_find_an_internal_property_if_you_ask_for_public_ones()
        {
            // Act
            var property = typeof(BaseClass).FindProperty("InternalProperty", MemberKind.Public);

            // Assert
            property.Should().BeNull();
        }

        [Fact]
        public void Can_find_an_internal_property_if_you_ask_for_them()
        {
            // Act
            var property = typeof(SuperClass).FindProperty("InternalProperty", MemberKind.Internal);

            // Assert
            property.Should().NotBeNull().And.Return<bool>();
        }

        [Fact]
        public void Can_find_an_internal_protected_property_if_you_ask_for_them()
        {
            // Act
            var property = typeof(SuperClass).FindProperty("InternalProtectedProperty", MemberKind.Internal);

            // Assert
            property.Should().NotBeNull().And.Return<bool>();
        }

        [Fact]
        public void Cannot_find_an_explicitly_implemented_property_if_you_dont_ask_for_that()
        {
            // Act
            var property = typeof(SuperClass).FindProperty("ExplicitlyImplementedProperty", MemberKind.Public);

            // Assert
            property.Should().BeNull();
        }

        [Fact]
        public void Can_find_an_explicitly_implemented_property_if_you_ask_for_it()
        {
            // Act
            var property = typeof(SuperClass).FindProperty("ExplicitlyImplementedProperty", MemberKind.ExplicitlyImplemented);

            // Assert
            property.Should().NotBeNull().And.Return<string>();
        }

        [Fact]
        public void Cannot_find_a_default_interface_property_if_you_dont_ask_for_that()
        {
            // Act
            var property = typeof(SuperClass).FindProperty("DefaultProperty", MemberKind.Public);

            // Assert
            property.Should().BeNull();
        }

#if NETCOREAPP3_0_OR_GREATER
        [Fact]
        public void Can_find_a_default_interface_property_if_you_ask_for_it()
        {
            // Act
            var property = typeof(SuperClass).FindProperty("DefaultProperty", MemberKind.DefaultInterfaceProperties);

            // Assert
            property.Should().NotBeNull().And.Return<string>();
        }
#endif

        [Fact]
        public void Can_find_a_public_indexer()
        {
            // Act
            var indexer = typeof(ClassWithIndexer).FindIndexer(MemberKind.Public, typeof(int));

            // Assert
            indexer.Should().NotBeNull();
            indexer.GetIndexParameters().Should().BeEquivalentTo(new[]
            {
                new { ParameterType = typeof(int) }
            });
        }

        [Fact]
        public void Cannot_find_an_internal_indexer_if_you_ask_for_public_ones()
        {
            // Act
            var indexer = typeof(ClassWithIndexer).FindIndexer(MemberKind.Public, typeof(string), typeof(string));

            // Assert
            indexer.Should().BeNull();
        }

        [Fact]
        public void Can_find_an_internal_indexer_if_you_ask_for_ot()
        {
            // Act
            var indexer = typeof(ClassWithIndexer).FindIndexer(MemberKind.Internal, typeof(string), typeof(string));

            // Assert
            indexer.Should().NotBeNull();
            indexer.GetIndexParameters().Should().BeEquivalentTo(new[]
            {
                new { ParameterType = typeof(string) },
                new { ParameterType = typeof(string) }
            });
        }

        // TODO: add tests for interfaces
    }

    public class FindField
    {
        [Fact]
        public void Can_find_a_public_instance_field()
        {
            // Act
            var field = typeof(SuperClass).FindField("NormalField", MemberKind.Public);

            // Assert
            field.Should().NotBeNull();
            field.Name.Should().Be("NormalField");
            field.FieldType.Should().Be<string>();
        }

        [Fact]
        public void Cannot_find_a_field_if_it_does_not_exist()
        {
            // Act
            var field = typeof(SuperClass).FindField("NonExistingField", MemberKind.Public);

            // Assert
            field.Should().BeNull();
        }

        [Fact]
        public void Cannot_find_a_static_field_if_you_dont_ask_for_it()
        {
            // Act
            var field = typeof(SuperClass).FindField("StaticField", MemberKind.Public);

            // Assert
            field.Should().BeNull();
        }

        [Fact]
        public void Can_find_a_static_field_if_you_ask_for_it()
        {
            // Act
            var field = typeof(SuperClass).FindField("StaticField", MemberKind.Public | MemberKind.Static);

            // Assert
            field.Should().NotBeNull();
            field.Name.Should().Be("StaticField");
            field.FieldType.Should().Be<bool>();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void A_field_name_is_required(string name)
        {
            // Act
            var act = () => typeof(SuperClass).FindField(name, MemberKind.Public);

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("*field name*");
        }

        [Fact]
        public void Cannot_find_an_internal_field_if_you_ask_for_public_ones()
        {
            // Act
            var field = typeof(BaseClass).FindField("InternalField", MemberKind.Public);

            // Assert
            field.Should().BeNull();
        }

        [Fact]
        public void Can_find_an_internal_field_if_you_ask_for_them()
        {
            // Act
            var field = typeof(SuperClass).FindField("InternalField", MemberKind.Internal);

            // Assert
            field.Should().NotBeNull();
            field.Name.Should().Be("InternalField");
            field.FieldType.Should().Be<string>();
        }

        [Fact]
        public void Can_find_an_internal_protected_property_if_you_ask_for_them()
        {
            // Act
            var field = typeof(SuperClass).FindField("ProtectedInternalField", MemberKind.Internal);

            // Assert
            field.Should().NotBeNull();
            field.Name.Should().Be("ProtectedInternalField");
            field.FieldType.Should().Be<string>();
        }

        [Fact]
        public void Cannot_find_an_explicitly_implemented_property_if_you_dont_ask_for_that()
        {
            // Act
            var property = typeof(SuperClass).FindProperty("ExplicitlyImplementedProperty", MemberKind.Public);

            // Assert
            property.Should().BeNull();
        }

        [Fact]
        public void Can_find_an_explicitly_implemented_property_if_you_ask_for_it()
        {
            // Act
            var property = typeof(SuperClass).FindProperty("ExplicitlyImplementedProperty", MemberKind.ExplicitlyImplemented);

            // Assert
            property.Should().NotBeNull().And.Return<string>();
        }

        [Fact]
        public void Cannot_find_a_default_interface_property_if_you_dont_ask_for_that()
        {
            // Act
            var property = typeof(SuperClass).FindProperty("DefaultProperty", MemberKind.Public);

            // Assert
            property.Should().BeNull();
        }
    }

    public class Methods
    {
        [Fact]
        public void Can_find_a_parameterless_method()
        {
            // Act
            var method = typeof(ClassWithMethods).FindMethod("Method", MemberKind.Public);

            // Assert
            method.Should().NotBeNull();
            method.GetParameters().Should().HaveCount(0);
        }

        [Fact]
        public void Can_find_a_parameterless_method_directly()
        {
            // Act
            var method = typeof(ClassWithMethods).FindParameterlessMethod("Method", MemberKind.Public);

            // Assert
            method.Should().NotBeNull();
            method.GetParameters().Should().HaveCount(0);
        }

        [Fact]
        public void Cannot_find_an_internal_method_if_you_ask_for_public_ones()
        {
            // Act
            var method = typeof(ClassWithMethods).FindMethod("InternalMethod", MemberKind.Public);

            // Assert
            method.Should().BeNull();
        }

        [Fact]
        public void Can_find_an_internal_method_if_you_ask_for_it()
        {
            // Act
            var method = typeof(ClassWithMethods).FindMethod("InternalMethod", MemberKind.Internal);

            // Assert
            method.Should().NotBeNull();
        }

        [Fact]
        public void Can_find_a_protected_internal_method_if_you_ask_for_it()
        {
            // Act
            var method = typeof(ClassWithMethods).FindMethod("ProtectedInternalMethod", MemberKind.Internal);

            // Assert
            method.Should().NotBeNull();
        }

        [Fact]
        public void Can_find_a_static_method()
        {
            // Act
            var method = typeof(ClassWithMethods).FindMethod("StaticMethod", MemberKind.Public);

            // Assert
            method.Should().NotBeNull();
        }

        [Fact]
        public void Can_find_a_method_with_any_parameter()
        {
            // Act
            var method = typeof(ClassWithMethods).FindMethod("MethodWithThreeParameters", MemberKind.Public);

            // Assert
            method.Should().NotBeNull();
            method.GetParameters().Should().HaveCount(3);
        }

        [Fact]
        public void Can_find_a_method_with_specific_parameter()
        {
            // Act
            var method = typeof(ClassWithMethods).FindMethod("MethodWithThreeParameters", MemberKind.Public, typeof(string),
                typeof(int), typeof(bool));

            // Assert
            method.Should().NotBeNull();
            method.GetParameters().Should().HaveCount(3);
        }

        [Fact]
        public void Can_detect_a_method_with_specific_parameter()
        {
            // Act / Assert
            typeof(ClassWithMethods).HasMethod("MethodWithThreeParameters", MemberKind.Public, typeof(string),
                typeof(int), typeof(bool)).Should().BeTrue();
        }

        [Fact]
        public void The_name_of_the_method_must_match()
        {
            // Act
            var method = typeof(ClassWithMethods).FindMethod("NonExistingName", MemberKind.Public, typeof(string), typeof(int),
                typeof(bool));

            // Assert
            method.Should().BeNull();
        }

        [Fact]
        public void The_number_of_parameters_must_match()
        {
            // Act
            var method = typeof(ClassWithMethods).FindMethod("MethodWithThreeParameters", MemberKind.Public, typeof(string),
                typeof(int));

            // Assert
            method.Should().BeNull();
        }

        [Fact]
        public void The_type_of_the_parameters_must_match()
        {
            // Act
            var method = typeof(ClassWithMethods).FindMethod("MethodWithThreeParameters", MemberKind.Public, typeof(string),
                typeof(object), typeof(bool));

            // Assert
            method.Should().BeNull();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void The_name_is_required(string methodName)
        {
            // Act
            var act = () => typeof(ClassWithMethods).FindMethod(methodName, MemberKind.Public);

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("*method name*");
        }

        private class ClassWithMethods
        {
            [UsedImplicitly]
            public void Method()
            {
            }

            [UsedImplicitly]
            public void MethodWithThreeParameters(string text, int number, bool flag)
            {
            }

            [UsedImplicitly]
            internal void InternalMethod()
            {
            }

            [UsedImplicitly]
            public static void StaticMethod()
            {
            }

            [UsedImplicitly]
            protected internal void ProtectedInternalMethod()
            {
            }
        }
    }

    public class ConversionOperators
    {
        [Fact]
        public void Can_find_the_explicit_convertor_for_a_specific_source_and_target_type()
        {
            // Act
            var convertor = typeof(ClassWithConversionOperators)
                .FindExplicitConversionOperator(typeof(ClassWithConversionOperators), typeof(int));

            // Assert
            convertor.Should().NotBeNull();
            convertor.ReturnType.Should().Be<int>();
        }

        [Fact]
        public void The_source_type_of_the_explicit_convertor_must_match()
        {
            // Act
            var convertor = typeof(ClassWithConversionOperators)
                .FindExplicitConversionOperator(typeof(ClassWithConversionOperators), typeof(bool));

            // Assert
            convertor.Should().BeNull();
        }

        [Fact]
        public void Can_find_the_implicit_convertor_for_a_specific_source_and_target_type()
        {
            // Act
            var convertor = typeof(ClassWithConversionOperators)
                .FindImplicitConversionOperator(typeof(ClassWithConversionOperators), typeof(string));

            // Assert
            convertor.Should().NotBeNull();
            convertor.ReturnType.Should().Be<string>();
        }

        [Fact]
        public void The_source_type_of_the_implicit_convertor_must_match()
        {
            // Act
            var convertor = typeof(ClassWithConversionOperators)
                .FindImplicitConversionOperator(typeof(ClassWithConversionOperators), typeof(bool));

            // Assert
            convertor.Should().BeNull();
        }

        private class ClassWithConversionOperators
        {
            [UsedImplicitly]
            public static explicit operator int(ClassWithConversionOperators instance) => 42;

            [UsedImplicitly]
            public static implicit operator string(ClassWithConversionOperators instance) => "42";
        }
    }

    private class SuperClass : BaseClass, IInterfaceWithDefaultProperty
    {
        public string NormalProperty { get; set; }

        public new int NewProperty { get; set; }

        public static bool StaticProperty { get; set; }

        internal bool InternalProperty { get; set; }

        protected internal bool InternalProtectedProperty { get; set; }

        string IInterfaceWithSingleProperty.ExplicitlyImplementedProperty { get; set; }

        public string InterfaceProperty { get; set; }

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
        public string NormalField;

        internal string InternalField;

        public static bool StaticField;

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

    private class ClassImplementingSetterOnlyInterface : IWithSetterOnlyProperty
    {
        public string WriteOnlyProperty { private get; set; }
    }

    private interface IWithSetterOnlyProperty
    {
        string WriteOnlyProperty { set; }
    }

    private sealed class ClassWithIndexer
    {
        [UsedImplicitly]
        public object Foo { get; set; }

        public string this[int n] => n.ToString(CultureInfo.InvariantCulture);

        internal string this[string s1, string s2] => s1 + "/" + s2;
    }
}
