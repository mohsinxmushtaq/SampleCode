using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Scanner.Test.Models
{
    public class ReceiptLine
    {
        public List<int> boundingBox { get; set; }
        public string text { get; set; }
        public List<Word> words { get; set; }
    }

    public class Word
    {
        public List<int> boundingBox { get; set; }
        public string text { get; set; }
        public double confidence { get; set; }
    }

}
