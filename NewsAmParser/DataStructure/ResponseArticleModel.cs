using ConstantDefine.Enums;
using System;

namespace NewsAmParser.DataStructure
{
    public class ResponseArticleModel
    {
        public string Title { get; set; }

        public NewsCategoryEnum Category { get; set; }

        public DateTime PublishedDateTime { get; set; }

        public DateTime ParseDateTime { get; set; }

        public string Region { get; set; }

        public string [] Topic { get; set; }

        public string Content { get; set; }
    }
}
