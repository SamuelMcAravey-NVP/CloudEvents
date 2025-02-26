using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Text.RegularExpressions;
using Xunit;

namespace Rixian.CloudEvents.Tests.v01
{
    public class DeserializeTests
    {
        [Theory]
        [InlineData("json1.json")]
        [InlineData("json2.json")]
        public void TestJsonFiles(string fileName)
        {
            var json = File.ReadAllText($@".\v01\samples\json\{fileName}");
            var evnt = CloudEventV0_1.Deserialize(json);
            Assert.IsType<JsonCloudEventV0_1>(evnt);
        }

        [Theory]
        [InlineData("binary1.json")]
        public void TestBinaryFiles(string fileName)
        {
            var json = File.ReadAllText($@".\v01\samples\binary\{fileName}");
            var evnt = CloudEventV0_1.Deserialize(json);
            Assert.IsType<BinaryCloudEventV0_1>(evnt);
        }

        [Theory]
        [InlineData("string1.json")]
        public void TestStringFiles(string fileName)
        {
            var json = File.ReadAllText($@".\v01\samples\string\{fileName}");
            var evnt = CloudEventV0_1.Deserialize(json);
            Assert.IsType<StringCloudEventV0_1>(evnt);
        }

        [Theory]
        [InlineData("none1.json")]
        public void TestNoDataFiles(string fileName)
        {
            var json = File.ReadAllText($@".\v01\samples\none\{fileName}");
            var evnt = CloudEventV0_1.Deserialize(json);
            Assert.IsType<CloudEventV0_1>(evnt);
            Assert.IsNotType<JsonCloudEventV0_1>(evnt);
            Assert.IsNotType<BinaryCloudEventV0_1>(evnt);
            Assert.IsNotType<StringCloudEventV0_1>(evnt);
        }
    }
}
