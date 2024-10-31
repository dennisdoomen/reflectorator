using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

#pragma warning disable CS0618 // Type or member is obsolete

namespace Reflectify.Specs;

public class TypeMetaDataExtensionsSpecs
{
    public class IsDerivedFromOpenGeneric
    {
        [Fact]
        public void Can_detect_a_type_derived_from_an_open_generic_type()
        {
            // Act
            bool result = typeof(DerivedFromOpenGeneric).IsDerivedFromOpenGeneric(typeof(OpenGenericClass<>));

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void An_open_generic_type_cannot_derive_from_itself()
        {
            // Act
            bool result = typeof(OpenGenericClass<>).IsDerivedFromOpenGeneric(typeof(OpenGenericClass<>));

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void An_unrelated_class_is_not_going_to_match_an_open_generic_type()
        {
            // Act
            bool result = typeof(SomeOtherClass).IsDerivedFromOpenGeneric(typeof(OpenGenericClass<>));

            // Assert
            result.Should().BeFalse();
        }
    }

    public class GetClosedGenericInterfaces
    {
        [Fact]
        public void Returns_nothing_if_the_class_does_not_implement_the_open_generic_interface()
        {
            // Act
            Type[] results = typeof(DerivedFromOpenGeneric).GetClosedGenericInterfaces(typeof(IOpenGenericInterface<>));

            // Assert
            results.Should().BeEmpty();
        }

        [Fact]
        public void Returns_nothing_if_an_interface_does_not_inherit_any_closed_generic_interface()
        {
            // Act
            Type[] results = typeof(ISomeOtherInterface).GetClosedGenericInterfaces(typeof(IOpenGenericInterface<>));

            // Assert
            results.Should().BeEmpty();
        }

        [Fact]
        public void Returns_nothing_if_a_class_does_not_implement_any_closed_generic_interface()
        {
            // Act
            Type[] results = typeof(SomeOtherClass).GetClosedGenericInterfaces(typeof(IOpenGenericInterface<>));

            // Assert
            results.Should().BeEmpty();
        }

        [Fact]
        public void Can_find_closed_generic_interfaces()
        {
            // Act
            Type[] results =
                typeof(TypeImplementingClosedGenericInterface).GetClosedGenericInterfaces(typeof(IOpenGenericInterface<>));

            // Assert
            results.Should().BeEquivalentTo([typeof(IClosedGenericInterface), typeof(IAnotherClosedGenericInterface)]);
        }
    }

    public class HasAttribute
    {
        [Fact]
        public void Can_determine_an_attribute_exists_on_a_specific_type()
        {
            // Act / Assert
            typeof(ClassWithAttribute).HasAttribute<InheritableAttribute>().Should().BeTrue();
        }

        [Fact]
        public void Can_determine_a_derived_attribute_exists_on_a_specific_type()
        {
            // Act / Assert
            typeof(ClassWithAttribute).HasAttribute<Attribute>().Should().BeTrue();
        }

        [Fact]
        public void The_attribute_must_be_applied_directly_to_the_type()
        {
            // Act / Assert
            typeof(ClassDerivedFromOneWithInheritableAndParameterizedAttribute).HasAttribute<CollectionDefinitionAttribute>()
                .Should().BeFalse();
        }

        [Fact]
        public void Can_determine_that_the_attribute_does_not_exist()
        {
            // Act / Assert
            typeof(ClassWithAttribute).HasAttribute<ObsoleteAttribute>().Should().BeFalse();
        }

        [Fact]
        public void Can_check_that_an_attribute_has_a_specific_property()
        {
            // Act
            bool result = typeof(ClassWithInheritableAndParameterizedAttribute)
                .HasAttribute<InheritableAttribute>(a => a.Message!.Contains("First"));

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Can_check_that_a_derived_attribute_has_a_specific_property()
        {
            // Act
            bool result = typeof(ClassWithInheritableAndParameterizedAttribute)
                .HasAttribute<Attribute>(a => !a.IsDefaultAttribute());

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void An_attribute_with_a_property_must_be_applied_directly_to_the_type()
        {
            // Act
            bool result = typeof(ClassDerivedFromOneWithInheritableAndParameterizedAttribute)
                .HasAttribute<CollectionDefinitionAttribute>(a => a.DisableParallelization);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void Can_check_that_an_attribute_doesnt_have_a_specific_property()
        {
            // Act
            bool result = typeof(ClassWithInheritableAndParameterizedAttribute)
                .HasAttribute<CollectionDefinitionAttribute>(a => !a.DisableParallelization);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void An_attribute_predicate_cannot_be_null()
        {
            // Act
            Action act = () => typeof(ClassWithInheritableAndParameterizedAttribute)
                .HasAttribute<CollectionDefinitionAttribute>(null);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }
    }

    public class HasAttributeInHierarchy
    {
        [Fact]
        public void Can_find_an_attribute_in_a_base_class()
        {
            // Act / Assert
            typeof(ClassDerivedFromOneWithInheritableAndParameterizedAttribute)
                .HasAttributeInHierarchy<InheritableAttribute>().Should().BeTrue();
        }

        [Fact]
        public void Can_find_an_attribute_with_a_specific_property()
        {
            // Act
            var result = typeof(ClassDerivedFromOneWithInheritableAndParameterizedAttribute)
                .HasAttributeInHierarchy<InheritableAttribute>(a => a.Message == "FirstAttribute");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Ignores_the_attribute_if_the_predicate_does_not_match()
        {
            // Act
            var result = typeof(ClassDerivedFromOneWithInheritableAndParameterizedAttribute)
                .HasAttributeInHierarchy<InheritableAttribute>(a => a.Message == "Other Message");

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void Cannot_find_an_attribute_on_a_base_class_if_its_not_inheritable()
        {
            // Act / Assert
            typeof(ClassDerivedFromOneWithNonInheritableAndParameterizedAttribute)
                .HasAttributeInHierarchy<NonInheritableAttribute>().Should().BeFalse();
        }

        [Fact]
        public void Cannot_find_an_attribute_if_none_exist()
        {
            // Act / Assert
            typeof(SomeOtherClass).HasAttributeInHierarchy<InheritableAttribute>().Should().BeFalse();
        }
    }

    public class GetMatchingAttributes
    {
        [Fact]
        public void Can_find_all_attributes_of_a_specific_type()
        {
            // Act
            InheritableAttribute[] results = typeof(ClassWithAttribute).GetMatchingAttributes<InheritableAttribute>();

            // Assert
            results.Should().ContainSingle().Which.Message.Should().Be("SomeMessage");
        }

        [Fact]
        public void Can_find_all_attributes_of_a_specific_type_in_a_derived_class()
        {
            // Act
            var results = typeof(ClassDerivedFromOneWithInheritableAndParameterizedAttribute)
                .GetMatchingAttributes<InheritableAttribute>();

            // Assert
            results.Should().BeEquivalentTo([
                new { Message = "FirstAttribute", },
                new { Message = "SecondAttribute" }
            ]);
        }

        [Fact]
        public void Can_find_all_attributes_of_a_specific_type_with_a_specific_property()
        {
            // Act
            var results = typeof(ClassWithInheritableAndParameterizedAttribute)
                .GetMatchingAttributes<InheritableAttribute>(a => a.Message.Contains("First"));

            // Assert
            results.Should().HaveCount(1);
        }

        [Fact]
        public void Can_find_all_attributes_of_a_specific_type_with_a_specific_property_in_a_derived_class()
        {
            // Act
            Attribute[] results = typeof(ClassDerivedFromOneWithInheritableAndParameterizedAttribute)
                .GetMatchingAttributes<InheritableAttribute>(a => a.Message.Contains("First"));

            // Assert
            results.Should().HaveCount(1);
        }

        [Fact]
        public void Can_find_all_attributes_of_a_specific_type_with_a_specific_property_in_a_derived_class_and_base_class()
        {
            // Act
            Attribute[] results = typeof(ClassDerivedFromOneWithInheritableAndParameterizedAttribute)
                .GetMatchingAttributes<InheritableAttribute>(a => a.Message.Contains("First"));

            // Assert
            results.Should().HaveCount(1);
        }

        [Fact]
        public void Will_apply_the_predicate_when_finding_attributes()
        {
            // Act
            Attribute[] results = typeof(ClassDerivedFromOneWithInheritableAndParameterizedAttribute)
                .GetMatchingAttributes<InheritableAttribute>(a => a.Message.Contains("WrongValue"));

            // Assert
            results.Should().HaveCount(0);
        }

        [Fact]
        public void A_predicate_must_be_valid()
        {
            // Act
            var act = () => typeof(ClassWithAttribute).HasAttributeInHierarchy<InheritableAttribute>(null);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }
    }

    public class OverridesEquals
    {
        [Fact]
        public void Can_detect_if_a_type_implements_equality()
        {
            // Act
            bool result = typeof(string).OverridesEquals();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Can_detect_that_a_type_implements_reference_equality()
        {
            // Act
            bool result = typeof(SomeOtherClass).OverridesEquals();

            // Assert
            result.Should().BeFalse();
        }
    }

    public class IsSameOrInherits
    {
        [Fact]
        public void Can_detect_if_a_type_is_the_same_as_another()
        {
            // Act
            bool result = typeof(string).IsSameOrInherits(typeof(string));

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Can_detect_if_a_type_is_derived_from_another()
        {
            // Act
            bool result = typeof(DerivedFromOpenGeneric).IsSameOrInherits(typeof(OpenGenericClass<>));

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Can_detect_if_a_type_is_not_derived_from_another()
        {
            // Act
            bool result = typeof(SomeOtherClass).IsSameOrInherits(typeof(OpenGenericClass<>));

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void Can_detect_if_a_type_is_the_same_as_another_interface()
        {
            // Act
            bool result = typeof(IOpenGenericInterface<string>).IsSameOrInherits(typeof(IOpenGenericInterface<string>));

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Can_detect_if_a_type_is_derived_from_another_interface()
        {
            // Act
            bool result = typeof(IClosedGenericInterface).IsSameOrInherits(typeof(IOpenGenericInterface<string>));

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Can_detect_if_a_type_is_not_derived_from_another_interface()
        {
            // Act
            bool result = typeof(ISomeOtherInterface).IsSameOrInherits(typeof(IOpenGenericInterface<string>));

            // Assert
            result.Should().BeFalse();
        }
    }

    public class IsCompilerGenerated
    {
        [Fact]
        public void An_anonymous_type_is_compiler_generated()
        {
            // Arrange
            var subject = new { SomeProperty = "SomeValue" };

            // Act
            bool result = subject.GetType().IsCompilerGenerated();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void A_tuple_is_compiler_generated()
        {
            // Arrange
            var subject = (SomeProperty: "SomeValue", SomeOtherProperty: 42);

            // Act
            bool result = subject.GetType().IsCompilerGenerated();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void A_record_is_compiler_generated()
        {
            // Act
            bool result = new SomeRecord("PropertyValue").GetType().IsCompilerGenerated();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void A_custom_type_is_never_compiler_generated()
        {
            // Act
            bool result = typeof(SomeOtherClass).IsCompilerGenerated();

            // Assert
            result.Should().BeFalse();
        }

        private record SomeRecord(string SomeProperty);
    }

    public class HasFriendlyName
    {
        [Fact]
        public void A_normal_class_has_a_friendly_name()
        {
            // Act
            typeof(SomeOtherClass).HasFriendlyName().Should().BeTrue();
        }

        [Fact]
        public void An_anonymous_type_does_not_have_a_friendly_name()
        {
            // Arrange
            var subject = new { SomeProperty = "SomeValue" };

            // Act / Assert
            subject.GetType().HasFriendlyName().Should().BeFalse();
        }

        [Fact]
        public void A_tuple_does_not_have_a_friendly_name()
        {
            // Arrange
            var subject = (SomeProperty: "SomeValue", SomeOtherProperty: 42);

            // Act / Assert
            subject.GetType().HasFriendlyName().Should().BeFalse();
        }
    }

    public class IsTuple
    {
        [Fact]
        public void A_normal_class_is_not_a_tuple()
        {
            // Act
            bool result = typeof(SomeOtherClass).IsTuple();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void An_anonymous_type_is_not_a_tuple()
        {
            // Arrange
            var subject = new { SomeProperty = "SomeValue" };

            // Act
            bool result = subject.GetType().IsTuple();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void A_tuple_is_a_tuple()
        {
            // Arrange
            var subject = (SomeProperty: "SomeValue", SomeOtherProperty: 42);

            // Act
            bool result = subject.GetType().IsTuple();

            // Assert
            result.Should().BeTrue();
        }
    }

    public class IsAnonymous
    {
        [Fact]
        public void A_normal_class_is_not()
        {
            // Act
            bool result = typeof(SomeOtherClass).IsAnonymous();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void An_anonymous_type_is()
        {
            // Arrange
            var subject = new { SomeProperty = "SomeValue" };

            // Act
            bool result = subject.GetType().IsAnonymous();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void A_tuple_is_not()
        {
            // Arrange
            var subject = (SomeProperty: "SomeValue", SomeOtherProperty: 42);

            // Act
            bool result = subject.GetType().IsAnonymous();

            // Assert
            result.Should().BeFalse();
        }
    }

    public class IsRecord
    {
        [Fact]
        public void A_normal_class_is_not()
        {
            // Act
            bool result = typeof(SomeOtherClass).IsRecord();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void A_record_is()
        {
            // Act
            bool result = new SomeRecord("PropertyValue").GetType().IsRecord();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void An_anonymous_type_is_not()
        {
            // Arrange
            var subject = new { SomeProperty = "SomeValue" };

            // Act
            bool result = subject.GetType().IsRecord();

            // Assert
            result.Should().BeFalse();
        }

        private record SomeRecord(string SomeProperty);
    }

    public class IsRecordClass
    {
        [Fact]
        public void A_normal_class_is_not()
        {
            // Act
            bool result = typeof(SomeOtherClass).IsRecordClass();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void A_record_is()
        {
            // Act
            bool result = new SomeRecord("PropertyValue").GetType().IsRecordClass();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void An_anonymous_type_is_not()
        {
            // Arrange
            var subject = new { SomeProperty = "SomeValue" };

            // Act
            bool result = subject.GetType().IsRecordClass();

            // Assert
            result.Should().BeFalse();
        }

        private record SomeRecord(string SomeProperty);
    }

    public class IsRecordStruct
    {
        [Fact]
        public void A_normal_class_is_not()
        {
            // Act
            bool result = typeof(SomeOtherClass).IsRecordStruct();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void A_class_record_is_not()
        {
            // Act
            bool result = new SomeClassRecord("PropertyValue").GetType().IsRecordStruct();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void A_struct_record_is()
        {
            // Act
            bool result = new SomeStructRecord("PropertyValue").GetType().IsRecordStruct();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void An_anonymous_type_is_not()
        {
            // Arrange
            var subject = new { SomeProperty = "SomeValue" };

            // Act
            bool result = subject.GetType().IsRecordStruct();

            // Assert
            result.Should().BeFalse();
        }

        private record struct SomeStructRecord(string SomeProperty);

        private record SomeClassRecord(string SomeProperty);
    }

    public class IsKeyValuePair
    {
        [Fact]
        public void A_normal_class_is_not()
        {
            // Act
            bool result = typeof(SomeOtherClass).IsKeyValuePair();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void A_key_value_pair_is()
        {
            // Act
            bool result = typeof(KeyValuePair<string, int>).IsKeyValuePair();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void A_key_value_pair_of_string_is()
        {
            // Act
            bool result = typeof(KeyValuePair<string, string>).IsKeyValuePair();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void A_key_value_pair_of_int_is()
        {
            // Act
            bool result = typeof(KeyValuePair<int, int>).IsKeyValuePair();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void A_key_value_pair_of_string_and_int_is()
        {
            // Act
            bool result = typeof(KeyValuePair<string, int>).IsKeyValuePair();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void A_key_value_pair_of_int_and_string_is()
        {
            // Act
            bool result = typeof(KeyValuePair<int, string>).IsKeyValuePair();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void An_anonymous_type_is_not()
        {
            // Arrange
            var subject = new { SomeProperty = "SomeValue" };

            // Act
            bool result = subject.GetType().IsKeyValuePair();

            // Assert
            result.Should().BeFalse();
        }
    }

    private class OpenGenericClass<T>
    {
        private T Property { get; set; }
    }

    private class DerivedFromOpenGeneric : OpenGenericClass<string>
    {
    }

    private class SomeOtherClass
    {
    }

    private interface IOpenGenericInterface<T>
    {
    }

    private interface IClosedGenericInterface : IOpenGenericInterface<string>
    {
    }

    private interface IAnotherClosedGenericInterface : IOpenGenericInterface<string>
    {
    }

    private class TypeImplementingClosedGenericInterface : IClosedGenericInterface, IAnotherClosedGenericInterface
    {
    }

    private interface ISomeOtherInterface
    {
    }

    [Inheritable("SomeMessage")]
    private class ClassWithAttribute
    {
    }

    [Inheritable("FirstAttribute")]
    [Inheritable("SecondAttribute")]
    private class ClassWithInheritableAndParameterizedAttribute
    {
    }

    private class ClassDerivedFromOneWithInheritableAndParameterizedAttribute : ClassWithInheritableAndParameterizedAttribute
    {
    }

    [NonInheritable("SomeMessage")]
    private class ClassWithNonInheritableAndParameterizedAttribute
    {
    }

    private class ClassDerivedFromOneWithNonInheritableAndParameterizedAttribute : ClassWithInheritableAndParameterizedAttribute
    {
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    private sealed class InheritableAttribute(string message) : Attribute
    {
        public string Message { get; } = message;
    }

    [AttributeUsage(AttributeTargets.All)]
    private sealed class NonInheritableAttribute(string message) : Attribute
    {
        public string Message { get; } = message;
    }
}
