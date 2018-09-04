using ConstantDefine.Enums;
using NewsAmParser.DataStructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewsAmParser
{
    public abstract class BaseParser
    {
        protected AppSetting AppSetting { get; }

        protected BaseParser(AppSetting appSetting)
        {
            AppSetting = appSetting;
        }

        public abstract Task<IEnumerable<ResponseArticleModel>> ParseAsync(NewsCategoryEnum category);
    }
}
