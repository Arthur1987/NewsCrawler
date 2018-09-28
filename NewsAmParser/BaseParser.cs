using ConstantDefine.Enums;
using NewsAmParser.DataStructure;
using System.Collections.Async;

namespace NewsAmParser
{
    public abstract class BaseParser
    {
        public abstract IAsyncEnumerable<ResponseArticleModel> ParseAsync(string baseAdress, string requestUri);
    }
}
