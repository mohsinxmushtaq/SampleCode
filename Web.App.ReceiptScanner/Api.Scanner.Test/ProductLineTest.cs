using Api.Scanner.Helper;
using Api.Scanner.Models;
using Api.Scanner.Test.Helper;
using Api.Scanner.Test.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Xunit;

namespace Api.Scanner.Test
{
    public class ProductLineTest
    {
        [Fact]
        public void Product_Input_Return_Product()
        {
            var extractor = new ExtractProductLines();
            var result = extractor.CheckCurrentLine("Zeeba Super Basmati Rice");

            Assert.Equal("product", result);
        }

        [Fact]
        public void Price_Input_Return_Price()
        {
            var extractor = new ExtractProductLines();
            var result = extractor.CheckCurrentLine("10.25");

            Assert.Equal("price", result);
        }

        [Fact]
        public void InvalidPrice_Input_Return_NoMatch()
        {
            var extractor = new ExtractProductLines();
            var result = extractor.CheckCurrentLine("10.2522");

            Assert.Equal("no-match", result);
        }

        [Fact]
        public void Clean_Product_Line()
        {
            var extractor = new ExtractProductLines();
            var result = extractor.CleanLine("*Product Name 1/Ct","product");

            Assert.Equal("Product Name 1/Ct", result);
        }

        [Theory]
        [InlineData("10 00 t", "10.00")]
        [InlineData("10.00 t", "10.00")]
        [InlineData("10.00  ", "10.00")]
        [InlineData("10.000", "10.00")]
        [InlineData("*10.000 T", "10.00")]
        public void Clean_Price_Line(string text, string expected)
        {
            var extractor = new ExtractProductLines();
            var result = extractor.CleanLine(text, "price");

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Check_ResultData_ProductLines()
        {
            var extractor = new ExtractProductLines();
            var fileData = File.ReadAllText("Data/test-data.json");
            var allData = JObject.Parse(fileData);
            var dataList = allData.ToObject<RecognizeResult>();

            var result = extractor.GetProductLines(dataList.ResultData.ToString());

            Assert.Equal(8, result.Count);
        }



    }
}
