using LabelTool.ReceiptScanner.Models;
using LabelTool.ReceiptScanner.ViewModels;
using OrchardCore.FileStorage;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LabelTool.ReceiptScanner.Services
{
    public interface ILabelToolServices
    {
        Task<long> Add(CreateLabelProjectViewModel project, ClaimsPrincipal user);

        Task<LabelProject> Get(int id);

        Task<object> RecognizeImageContent(string imagePath);

        Task<IList<ImageAssets>> GetImageAssets(int id);

    }
}
