using System;
using System.Collections.Generic;
using FluentAssertions;
using SharpSync.Core.Generators;
using Xunit;

namespace SharpSync.Tests.Unit.Generators
{
    public class TypeMapperTests
    {
        [Theory]
        [InlineData(typeof(string), "string")]
        [InlineData(typeof(int), "number")]
        [InlineData(typeof(double), "number")]
        [InlineData(typeof(bool), "boolean")]
        [InlineData(typeof(Guid), "string")]
        [InlineData(typeof(DateTime), "string")]
        [InlineData(typeof(void), "void")]
        public void MapCSharpToTypeScript_SimpleTypes_ReturnsCorrectTsType(Type input, string expected)
        {
            // Act
            var result = TypeMapper.MapCSharpToTypeScript(input);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void MapCSharpToTypeScript_Array_ReturnsArrayType()
        {
            // Act
            var result = TypeMapper.MapCSharpToTypeScript(typeof(string[]));

            // Assert
            result.Should().Be("string[]");
        }

        [Fact]
        public void MapCSharpToTypeScript_List_ReturnsArrayType()
        {
            // Act
            var result = TypeMapper.MapCSharpToTypeScript(typeof(List<int>));

            // Assert
            result.Should().Be("number[]");
        }

        [Fact]
        public void MapCSharpToTypeScript_NullableInt_ReturnsUnionWithNull()
        {
            // Act
            var result = TypeMapper.MapCSharpToTypeScript(typeof(int?));

            // Assert
            result.Should().Be("number | null");
        }

        [Fact]
        public void MapCSharpToZod_String_ReturnsZodString()
        {
            // Arrange
            var zodEnabled = new HashSet<Type>();

            // Act
            var result = TypeMapper.MapCSharpToZod(typeof(string), zodEnabled);

            // Assert
            result.Should().Be("z.string()");
        }

        [Fact]
        public void MapCSharpToZod_Int_ReturnsZodInt()
        {
            // Arrange
            var zodEnabled = new HashSet<Type>();

            // Act
            var result = TypeMapper.MapCSharpToZod(typeof(int), zodEnabled);

            // Assert
            result.Should().Be("z.number().int()");
        }

        [Fact]
        public void MapCSharpToZod_EnabledType_ReturnsLazySchema()
        {
            // Arrange
            var type = typeof(TypeMapperTests);
            var zodEnabled = new HashSet<Type> { type };

            // Act
            var result = TypeMapper.MapCSharpToZod(type, zodEnabled);

            // Assert
            result.Should().Be($"z.lazy(() => {type.Name}Schema)");
        }

        [Theory]
        [InlineData("MyProperty", "myProperty")]
        [InlineData("myProperty", "myProperty")]
        [InlineData("URL", "uRL")]
        public void CamelCase_ReturnsCorrectString(string input, string expected)
        {
            // Act
            var result = TypeMapper.CamelCase(input);

            // Assert
            result.Should().Be(expected);
        }
    }
}
