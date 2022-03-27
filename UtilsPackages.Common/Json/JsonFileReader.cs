using Newtonsoft.Json;

namespace UtilsPackages.Common.Json
{
    public interface IJsonFileReader
    {
        T? Read<T>(string file);
    }

    public class JsonFileReader : IJsonFileReader
    {
        public T? Read<T>(string file)
        {
            var content = File.ReadAllText(file, System.Text.Encoding.UTF8);
            if (string.IsNullOrEmpty(content))
            {
                return default;
            }

            return JsonConvert.DeserializeObject<T>(content);
        }
    }
}
