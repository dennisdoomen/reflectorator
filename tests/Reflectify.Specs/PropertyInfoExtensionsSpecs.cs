using System.Globalization;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace Reflectify.Specs;

public class PropertyInfoExtensionsSpecs
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

    private sealed class ClassWithIndexer
    {
        [UsedImplicitly]
        public object Foo { get; set; }

        public string this[int n] => n.ToString(CultureInfo.InvariantCulture);
    }
}
