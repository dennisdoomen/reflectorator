using System;
using FluentAssertions;
using Xunit;

namespace Reflectify.Specs;

public class MemberInfoExtensionsSpecs
{
    [Fact]
    public void Can_determine_a_method_has_an_attribute()
    {
        // Arrange
        var member = typeof(ClassWithAttributedMember).GetMethod("Method");

        // Act / Assert
        member.HasAttribute<ObsoleteAttribute>().Should().BeTrue();
    }

    [Fact]
    public void Can_determine_a_method_has_an_attribute_using_a_specific_predicate()
    {
        // Arrange
        var member = typeof(ClassWithAttributedMember).GetMethod("Method");

        // Act / Assert
        member.HasAttribute<ObsoleteAttribute>(attribute =>
            attribute.Message!.StartsWith("Specific")).Should().BeTrue();
    }

    [Fact]
    public void The_predicate_must_not_be_null()
    {
        // Arrange
        var member = typeof(ClassWithAttributedMember).GetMethod("Method");

        // Act
        var act = () => member.HasAttribute<ObsoleteAttribute>(null).Should().BeTrue();

        // Assert
        act.Should().Throw<ArgumentNullException>().WithMessage("*predicate*");
    }

    [Fact]
    public void Can_determine_a_method_has_an_attribute_that_does_not_meet_a_predicate()
    {
        // Arrange
        var member = typeof(ClassWithAttributedMember).GetMethod("Method");

        // Act / Assert
        member.HasAttribute<ObsoleteAttribute>(predicate =>
            predicate.Message.Contains("*Other*")).Should().BeFalse();
    }

    private class ClassWithAttributedMember
    {
        [Obsolete("Specific reason")]
        public void Method()
        {
        }
    }
}
