using System;
using System.Collections.Generic;
using System.Text;

namespace LabelTool.ReceiptScanner.ViewModels
{
    public class CreateLabelProjectViewModel
    {
        public string DisplayName { get; set; }
        public string AzureConnection { get; set; }
        public string FolderPath { get; set; }
    }
}
