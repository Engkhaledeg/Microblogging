using System;
using Xunit;
using FluentAssertions;
using MicroBlog.Core.Entities;

namespace MicroBlog.Tests.Unit.Entities
{
    public class PostEntityTests
    {
        [Fact]
        public void CreatingPost_WithTextLongerThan140_ShouldThrow()
        {
            var longText = new string('x', 141);
            Action act = () => new Post { UserName = "alice", Text = longText, Latitude = 0, Longitude = 0 };
            act.Should().Throw<ArgumentException>().WithMessage("*maximum 140 characters*");
        }

        [Fact]
        public void CreatedAt_DefaultsToUtcNow()
        {
            var before = DateTime.UtcNow.AddSeconds(-1);
            var post = new Post { UserName = "bob", Text = "Hello", Latitude = 45, Longitude = 10 };
            var after = DateTime.UtcNow.AddSeconds(1);
            post.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        }

        [Theory]
        [InlineData(91, 0)]
        [InlineData(-91, 0)]
        [InlineData(0, 181)]
        [InlineData(0, -181)]
        public void InvalidCoords_ShouldThrow(double lat, double lng)
        {
            var post = new Post { UserName = "carol", Text = "Geo test", Latitude = 0, Longitude = 0 };
            Action actLat = () => post.Latitude = lat;
            Action actLng = () => post.Longitude = lng;
            actLat.Should().Throw<ArgumentOutOfRangeException>();
            actLng.Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}
