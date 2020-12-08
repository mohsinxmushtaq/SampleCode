using System;
using System.Collections.Generic;
using System.Text;

namespace LabelTool.ReceiptScanner.Models
{
    public class LabelProject
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string AzureConnection { get; set; }
        public string FolderPath { get; set; }
        public DateTime CreatedUtc { get; set; }
        public string Owner { get; set; }
        public LabelProject()
        {
            this.CreatedUtc = DateTime.UtcNow;
        }
    }
}
