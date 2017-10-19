using Xunit;
using Musiq.Handlers;

namespace MusiqTests.Handlers
{
    public class TagGeneratorTests
    {
        [Fact]
        public void TestMethod1()
        {
            var filename = @"C:\Users\Grandma\bomb tracks\x gon' give it to ya - dmx.mp3";
            var tag = TrackMetaGenerator.Read(filename);

            Assert.Equal("x gon' give it to ya - dmx.mp3", tag.FileName);
            Assert.Equal("mp3", tag.FileType);
            Assert.Equal("dmx", tag.Artist);
            Assert.Equal("x gon' give it to ya", tag.Title);
            Assert.Equal(1, tag.Score);
        }
    }
}
