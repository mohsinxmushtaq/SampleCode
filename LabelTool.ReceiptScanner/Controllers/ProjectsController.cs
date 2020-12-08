using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LabelTool.ReceiptScanner.Models;
using LabelTool.ReceiptScanner.Services;
using LabelTool.ReceiptScanner.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OrchardCore.Admin;
using OrchardCore.DisplayManagement.Notify;
using OrchardCore.FileStorage;
using OrchardCore.Media;
using OrchardCore.Security;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LabelTool.ReceiptScanner.Controllers
{
    [Admin]
    public class ProjectsController : Controller
    {
        private readonly ILabelToolServices _lableToolService;
        private readonly IAuthorizationService _authorizationService;
        private readonly INotifier _notifier;
        private readonly IHtmlLocalizer H;
        private readonly IMediaFileStore _mediaFileStore;

        public ProjectsController(
            ILabelToolServices lableToolService, 
            IAuthorizationService authorizationService, 
            INotifier notifier, 
            IHtmlLocalizer<ProjectsController> htmlLocalizer,
            IMediaFileStore mediaFileStore)
        {
            _lableToolService = lableToolService;
            _authorizationService = authorizationService;
            _notifier = notifier;
            H = htmlLocalizer;
            _mediaFileStore = mediaFileStore;
        }
        // GET: /<controller>/
        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Editor()
        {
            return View();
        }


        public async Task<List<ImageAssets>> GetImages(int id)
        {
            var imageList = new List<ImageAssets>();
            if (id > 0)
            {
                var result = await _lableToolService.GetImageAssets(id);
                imageList = new List<ImageAssets>(result);
                return imageList;
            }
            return imageList;
        }

        public async Task<object> GetImageOcr(string imagePath)
        {
            try
            {
                using (StreamReader r = new StreamReader(@"/Media/" + imagePath + ".ocr.json"))
                {
                    string json = r.ReadToEnd();
                    object ro = JsonConvert.DeserializeObject<object>(json);
                    return ro;
                }
            }
            catch(Exception ex)
            {
                return new object();
            }
        }


        [HttpPost]
        public async Task<IActionResult> Create(CreateLabelProjectViewModel project)
        {
            if (User.IsInRole("Administrator"))
            {
                var result = await _lableToolService.Add(project, User);

                _notifier.Success(H["Project created successfully"]);
                return View(project);
            }
            else
            {
                return StatusCode(401);
            }
            
        }


    }
}
