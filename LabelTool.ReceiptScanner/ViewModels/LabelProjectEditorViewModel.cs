using LabelTool.ReceiptScanner.Models;
using System.Collections.Generic;

namespace LabelTool.ReceiptScanner.ViewModels
{
    public class LabelProjectEditorViewModel
    {
        public LabelProject lableProject { get; set; }
        public List<ImageAssets> Assets { get; set; }
        public LabelProjectEditorViewModel()
        {
            this.Assets = new List<ImageAssets>();
        }
    }

    

}
