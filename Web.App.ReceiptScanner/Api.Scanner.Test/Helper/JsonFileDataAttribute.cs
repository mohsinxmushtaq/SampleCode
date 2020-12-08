using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Api.Scanner.Test.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit.Sdk;

namespace Api.Scanner.Test.Helper
{
    public class JsonFileDataAttribute : DataAttribute
    {
        private readonly string _filePath;

        private readonly string _propertyName;

        private readonly Type _dataType;

        private readonly Type _resultType;

        public JsonFileDataAttribute(string filePath, Type dataType, Type resultType)
        {
            _filePath = filePath;
            _dataType = dataType;
            _resultType = resultType;
        }

        public JsonFileDataAttribute(string filePath, string propertyName, Type dataType, Type resultType)
        {
            _filePath = filePath;
            _propertyName = propertyName;
            _dataType = dataType;
            _resultType = resultType;
        }

        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            if (testMethod == null) { throw new ArgumentNullException(nameof(testMethod)); }

            // Get the absolute path to the JSON file
            var path = Path.IsPathRooted(_filePath)
                ? _filePath
                : Path.GetRelativePath(Directory.GetCurrentDirectory(), _filePath);

            if (!File.Exists(path))
            {
                throw new ArgumentException($"Could not find file at path: {path}");
            }

            // Load the file
            var fileData = File.ReadAllText(_filePath);

            if (string.IsNullOrEmpty(_propertyName))
            {
                //whole file is the data
                return JsonConvert.DeserializeObject<List<object[]>>(fileData);
            }

            // Only use the specified property as the data
            var allData = JObject.Parse(fileData);
            var data = allData[_propertyName];
            var dataList = data.ToObject<List<ReceiptLine>>();

            

            var objectList = new List<ReceiptLine[]>();
            foreach (var d in dataList)
            {
                objectList.Add(new ReceiptLine[] { d });
            }

            return objectList;
        }
        
    }
}
