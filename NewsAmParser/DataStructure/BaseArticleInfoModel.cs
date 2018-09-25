using System;

namespace NewsAmParser.DataStructure
{
    public class BaseArticleInfoModel
    {
        public string Title { get; set; }

        public string BaseAddress { get; set; }

        public string Address { get; set; }

        public DateTime PublishedDateTimeUtc { get; set; }
    }
}
