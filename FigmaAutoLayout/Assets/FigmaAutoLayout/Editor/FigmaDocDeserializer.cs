using Blobler.Objects;
using Newtonsoft.Json; 

namespace Blobler
{
    public static class FigmaDocDeserializer
    {
        public static FigmaFile Parse(string jsonString)
        {
            var file = JsonConvert.DeserializeObject<FigmaFile>(jsonString);
            return file; 
        }
    }
}
