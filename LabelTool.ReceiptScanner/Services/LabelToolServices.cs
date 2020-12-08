using LabelTool.ReceiptScanner.Models;
using Microsoft.Data.SqlClient.DataClassification;
using Microsoft.Extensions.Logging;
using OrchardCore.Data;
using OrchardCore.Environment.Shell.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YesSql;
using Dapper;
using Dapper.Contrib.Extensions;
using LabelTool.ReceiptScanner.ViewModels;
using Org.BouncyCastle.Bcpg;
using System.Security.Claims;
using Azure;
using Azure.AI.FormRecognizer;
using Azure.AI.FormRecognizer.Models;
using OrchardCore.Media.Azure;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Threading;
using Newtonsoft.Json;
using System.Linq;
using OrchardCore.Media;
using System.IO;
using OrchardCore.FileStorage;

namespace LabelTool.ReceiptScanner.Services
{
    public class LabelToolServices : ILabelToolServices
    {
        private readonly IShellConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly IDbConnectionAccessor _dbAccessor;
        private readonly IMediaFileStore _mediaFileStore;

        public LabelToolServices(
            ILogger<LabelToolServices> logger,
            IShellConfiguration configuration,
            IDbConnectionAccessor dbAccessor,
            IMediaFileStore mediaFileStore
            )
        {
            _logger = logger;
            _configuration = configuration;
            _dbAccessor = dbAccessor;
            _mediaFileStore = mediaFileStore;
        }

        public async Task<long> Add(CreateLabelProjectViewModel project, ClaimsPrincipal user)
        {
            try
            {
                using (var connection = _dbAccessor.CreateConnection())
                {
                    connection.Open();

                    var dialect = SqlDialectFactory.For(connection);

                    var insertModel = new LabelProject();
                    insertModel.DisplayName = project.DisplayName;
                    insertModel.Owner = user.Identity.Name;
                    insertModel.AzureConnection = project.AzureConnection;
                    insertModel.FolderPath = project.FolderPath;

                    var identity = connection.Insert(insertModel);

                    return identity;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while inserting Tools Project details: " + ex.Message);
                return 0;
            }
        }

        public async Task<LabelProject> Get(int id)
        {
            using (var connection = _dbAccessor.CreateConnection())
            {
                connection.Open();

                var dialect = SqlDialectFactory.For(connection);

                var recognizeResultList = await connection.QueryFirstAsync<LabelProject>("SELECT id,DisplayName,AzureConnection,FolderPath,CreatedUtc,Owner from LabelProjects where Id = @Id", new { Id = id });

                return recognizeResultList;
            }
        }

        public async Task<IList<ImageAssets>> GetImageAssets(int id)
        {
            var imageList = new List<ImageAssets>();

            using (var connection = _dbAccessor.CreateConnection())
            {
                connection.Open();

                var dialect = SqlDialectFactory.For(connection);

                var recognizeResultList = await connection.QueryFirstAsync<LabelProject>("SELECT id,DisplayName,AzureConnection,FolderPath,CreatedUtc,Owner from LabelProjects where Id = @Id", new { Id = id });

                var dirContent = await _mediaFileStore.GetDirectoryContentAsync(recognizeResultList.FolderPath);

                string[] imgEnds = new string[] { ".jpg", ".jpeg" };

                var imgContentList = dirContent.Where(x => imgEnds.Any(x.Path.EndsWith));

                foreach (var item in imgContentList)
                {
                    bool exisits = OcrExisits(dirContent, item.Path);
                    imageList.Add(new ImageAssets { ImageName = item.Name, ImagePath = item.Path, OcrExisits = exisits });
                }
                return imageList;
            }
        }

        private bool OcrExisits(IEnumerable<IFileStoreEntry> dirContent, string path)
        {
            return dirContent.Any(x => x.Path == path + ".ocr.json");
        }

        public async Task<object> RecognizeImageContent(string imagePath)
        {
            var azureFormsKey = _configuration["FormsKey"];
            var azureLayoutUrl = _configuration["AnalyzeLayout"];


            //Azure blob storage
            var azureBlobEndPoint = _configuration["BlobEndPoint"];
            var azureSasQuery = _configuration["SASQuery"];
            var azureBlobContainer = _configuration[$"OrchardCore_Media_Azure:{nameof(MediaBlobStorageOptions.ContainerName)}"];

            //create a public image URL with sas query to access image
            var imagePublicUrl = azureBlobEndPoint + azureBlobContainer + "/" + imagePath + azureSasQuery;
            Uri imageUri = new Uri(imagePublicUrl);
            var test = imageUri.AbsoluteUri;

            //Creating the HTTP Client to begin post request
            var client = new HttpClient();

            var objData = new JObject();
            objData["source"] = imageUri.AbsoluteUri;
            var serialized = JsonConvert.SerializeObject(objData);
            var data = new StringContent(serialized, Encoding.UTF8, "application/json");

            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", azureFormsKey);
            var response = await client.PostAsync(azureLayoutUrl, data);
            string result = response.Headers.GetValues("Operation-Location").FirstOrDefault();

            //start to check if the result from the recognizer is availble 
            var n_tries = 10;
            var n_try = 0;
            var wait_sec = 5000;
            string getFormResult = "";

            while (n_try < n_tries)
            {
                var resultRespose = await client.GetAsync(result);
                getFormResult = resultRespose.Content.ReadAsStringAsync().Result;

                dynamic obj = JsonConvert.DeserializeObject(getFormResult);
                var status = obj.status;

                if (status != "succeeded")
                {
                    Thread.Sleep(wait_sec);
                    n_try++;
                }
                else
                {
                    break;
                }

            }

            return getFormResult;
        }
    }
}
