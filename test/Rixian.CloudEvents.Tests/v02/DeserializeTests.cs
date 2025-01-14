using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;

namespace Rixian.CloudEvents.Tests.v02
{
    public class DeserializeTests
    {
        [Theory]
        [InlineData("json1.json")]
        [InlineData("json2.json")]
        public void TestJsonFiles(string fileName)
        {
            var json = File.ReadAllText($@".\v02\samples\json\{fileName}");
            var evnt = CloudEventV0_2.Deserialize(json);

            evnt.Should().BeOfType<JsonCloudEventV0_2>();
        }

        [Theory]
        [InlineData("string1.json")]
        public void TestStringFilesV02(string fileName)
        {
            var json = File.ReadAllText($@".\v02\samples\string\{fileName}");
            var evnt = CloudEventV0_2.Deserialize(json);

            evnt.Should().BeOfType<StringCloudEventV0_2>();
        }

        [Theory]
        [InlineData("none1.json")]
        public void TestNoDataFiles(string fileName)
        {
            var json = File.ReadAllText($@".\v02\samples\none\{fileName}");
            var evnt = CloudEventV0_2.Deserialize(json);

            evnt.Should().BeOfType<CloudEventV0_2>();
            evnt.Should().NotBeOfType<JsonCloudEventV0_2>();
            evnt.Should().NotBeOfType<BinaryCloudEventV0_2>();
            evnt.Should().NotBeOfType<StringCloudEventV0_2>();
        }

        [Theory]
        [InlineData("custom1.json")]
        public void CustomEvent(string fileName)
        {
            var json = File.ReadAllText($@".\v02\samples\custom\{fileName}");
            var evnt = JsonConvert.DeserializeObject<TestCloudEvent>(json);

            evnt.Should().NotBeNull();
            evnt.Should().BeOfType<TestCloudEvent>();
        }

        [Theory]
        [InlineData("AAAAAA")]
        public void BinaryEvent_ContainsData_Success(string data)
        {
            var evnt = CloudEventV0_2.CreateCloudEvent("test", new Uri("/", UriKind.RelativeOrAbsolute), Encoding.UTF8.GetBytes(data));

            evnt.Should().NotBeNull();
            evnt.Should().BeOfType<BinaryCloudEventV0_2>();

            var jobj = JObject.FromObject(evnt);

            //
            // Can explicitly deserialize to binary
            var evnt2 = jobj.ToObject<BinaryCloudEventV0_2>();
            evnt2.Should().NotBeNull();
            evnt2.Data.Should().NotBeNull();

            //
            // Without a type provided this should deserialize to a binary event
            var evnt3 = CloudEventV0_2.Deserialize(jobj.ToString());
            evnt3.Should().NotBeNull();
            evnt3.Should().BeOfType<BinaryCloudEventV0_2>();
        }

        [Fact]
        public void BinaryEvent_NoData_Success()
        {
            var evnt = CloudEventV0_2.CreateCloudEvent("test", new Uri("/", UriKind.RelativeOrAbsolute), (byte[])null);

            evnt.Should().NotBeNull();
            evnt.Should().BeOfType<BinaryCloudEventV0_2>();

            var jobj = JObject.FromObject(evnt);

            //
            // Can explicitly deserialize to binary even without data present
            var evnt2 = jobj.ToObject<BinaryCloudEventV0_2>();
            evnt2.Should().NotBeNull();
            evnt2.Data.Should().BeNull();

            //
            // Without a type provided this should deserialize to a generic event
            var evnt3 = CloudEventV0_2.Deserialize(jobj.ToString());
            evnt3.Should().NotBeNull();
            evnt3.Should().BeOfType<CloudEventV0_2>();
        }

        [Theory]
        [InlineData("custombinary1.json")]
        public void CustomBinaryEvent_Success(string fileName)
        {
            var json = File.ReadAllText($@".\v02\samples\custom\{fileName}");
            var evnt = JsonConvert.DeserializeObject<TestBinaryEvent>(json);

            evnt.Should().NotBeNull();
            evnt.Should().BeOfType<TestBinaryEvent>();

            var dataString = Encoding.UTF8.GetString(evnt.Data);
            dataString.Should().Be("This is a test!");

            evnt.SampleField1.Should().Be("ABCDEFG");
        }

        [Theory]
        [InlineData("pdf1.pdf", "application/pdf")]
        public void BinaryEvent_LargeData_Success(string fileName, string contentType)
        {
            var data = File.ReadAllBytes($@".\v02\samples\binary\{fileName}");
            var evnt = CloudEventV0_2.CreateCloudEvent("test", new Uri("/", UriKind.RelativeOrAbsolute), data, contentType, null, null);

            evnt.Should().NotBeNull();
            evnt.Should().BeOfType<BinaryCloudEventV0_2>();
            evnt.ContentType.Should().Be(contentType);

            evnt.Data.Length.Should().Be(data.Length);

            var json = JsonConvert.SerializeObject(evnt, Formatting.Indented);
        }
    }
}
