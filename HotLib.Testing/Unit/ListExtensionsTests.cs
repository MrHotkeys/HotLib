using System;
using System.Collections.Generic;

using FluentAssertions;

using HotLib.DotNetExtensions;

using Xunit;

namespace HotLib.Testing.Unit
{
    // TODO: Does not have coverage for all members of ListExtensions
    public static class ListExtensionsTests
    {
        [Fact]
        public static void ToDictionaryOfIndices_NullArray_Throws()
        {
            FluentActions.Invoking(() => ListExtensions.ToDictionaryOfIndices<object>(null!)).Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public static void ToDictionaryOfIndices_DuplicatesArray_Throws()
        {
            new[] { 1, 1 }
                .Invoking(a => a.ToDictionaryOfIndices())
                .Should().Throw<ArgumentException>();
        }

        [Fact]
        public static void ToDictionaryOfIndices_IsNotReadOnly()
        {
            var arr = new[] { 1, 2, 3 };

            // Need to cast to IDictionary to have access to IsReadOnly
            var dict = (IDictionary<int, int>)arr.ToDictionaryOfIndices();

            dict.IsReadOnly.Should().BeFalse();
            dict.Invoking(d => d.Add(4, 3))
                .Should().NotThrow();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(1, 2, 3, 4, 5)]
        [InlineData("one", "two", "three", "four", "five")]
        [InlineData(true, false)]
        [InlineData(1, 1f, "one")]
        public static void ToDictionaryOfIndices_FilledArray_Reversed(params object[] arr)
        {
            var dict = arr.ToDictionaryOfIndices();

            dict.Should().NotBeNull();
            dict.Should().HaveCount(arr.Length);

            for (var i = 0; i < arr.Length; i++)
            {
                var key = arr[i];

                dict.Should().ContainKey(key);
                dict[key].Should().Be(i);
            }
        }
    }
}