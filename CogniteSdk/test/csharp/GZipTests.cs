using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using static Oryx.Cognite.GZip;

namespace Test.CSharp
{
    public class GZipTests
    {
        private class TestData
        {
            public string Name { get; set; }
            public int Value { get; set; }
            public string[] Items { get; set; }
        }

        [Fact]
        public async Task TestGZipJsonStreamContent()
        {
            var largeData = new TestData
            {
                Name = new string('A', 1000),
                Value = 42,
                Items = Enumerable.Repeat("item", 100).ToArray()
            };

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var uncompressedJson = JsonSerializer.Serialize(largeData, options);
            var uncompressedSize = Encoding.UTF8.GetByteCount(uncompressedJson);

            var content = new GZipJsonStreamContent<TestData>(largeData, CompressionLevel.Optimal, options);

            var compressedBytes = await content.ReadAsByteArrayAsync();
            var compressedSize = compressedBytes.Length;

            Assert.Equal("application/json", content.Headers.ContentType.MediaType);
            Assert.Contains("gzip", content.Headers.ContentEncoding);

            Assert.True(compressedSize < uncompressedSize);
        }

    }
}

