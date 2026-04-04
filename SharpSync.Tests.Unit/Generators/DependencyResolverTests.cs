using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using SharpSync.Core.Generators;
using Xunit;

namespace SharpSync.Tests.Unit.Generators
{
    public class DependencyResolverTests
    {
        private readonly HashSet<Type> _zodEnabledTypes;
        private readonly DependencyResolver _resolver;

        // Mock Classes for testing
        public class MyMockController { 
            public MyMockDto GetDto() => new MyMockDto();
        }
        public class MyMockDto { 
            public int Id { get; set; }
            public string Name { get; set; }
            public List<RelatedDto> Related { get; set; } = new List<RelatedDto>();
        }
        public class RelatedDto { 
            public string Description { get; set; }
        }

        public DependencyResolverTests()
        {
            _zodEnabledTypes = new HashSet<Type>();
            _resolver = new DependencyResolver(_zodEnabledTypes);
        }

        [Fact]
        public void CollectTypesRecursive_Controller_IdentifiesDtoDependencies()
        {
            // Arrange
            var initialTypes = new List<Type> { typeof(MyMockController) };

            // Act
            var result = _resolver.CollectTypesRecursive(initialTypes);

            // Assert
            result.Should().Contain(typeof(MyMockDto));
            result.Should().Contain(typeof(RelatedDto));
            result.Should().HaveCount(3); // Controller + MyMockDto + RelatedDto
        }

        [Fact]
        public void MarkTypeAndDependenciesForZod_Dto_MarksRecursively()
        {
            // Act
            _resolver.MarkTypeAndDependenciesForZod(typeof(MyMockDto));

            // Assert
            _zodEnabledTypes.Should().Contain(typeof(MyMockDto));
            _zodEnabledTypes.Should().Contain(typeof(RelatedDto));
        }

        [Theory]
        [InlineData(typeof(List<string>), typeof(string))]
        [InlineData(typeof(int[]), typeof(int))]
        [InlineData(typeof(int?), typeof(int))]
        [InlineData(typeof(int), typeof(int))]
        public void GetBaseType_ReturnsUnwrappedType(Type input, Type expected)
        {
            // Act
            var result = TypeMapper.GetBaseType(input);

            // Assert
            result.Should().Be(expected);
        }
    }
}
