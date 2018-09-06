using ConstantDefine.Enums;
using NewsAmParser.DataStructure;
using System.Collections.Async;

namespace NewsAmParser
{
    public abstract class BaseParser
    {
        protected AppSetting AppSetting { get; }

        protected BaseParser(AppSetting appSetting)
        {
            AppSetting = appSetting;
        }

        public abstract IAsyncEnumerable<ResponseArticleModel> ParseAsync(NewsCategoryEnum category);
    }
}
