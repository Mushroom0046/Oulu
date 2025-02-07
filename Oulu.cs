using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Oulu
{
    public static class OuluLanguage
    {
        public const string EN = "en";
    }

    public class WordBook
    {
        public string Id { get; set; }
        public string Language { get; set; }
        public string Name { get; set; }
    }

    public static class OuluClient
    {
        const string GET_WORD_BOOK = "https://api.frdic.com/api/open/v1/studylist/category";
        const string POST_WORD_BOOK = "https://api.frdic.com/api/open/v1/studylist/category";
        const string DELETE_WORD_BOOK = "https://api.frdic.com/api/open/v1/studylist/category";
        const string POST_WORD = "https://api.frdic.com/api/open/v1/studylist/words";

        public static HttpClient Client = new HttpClient();
        public static string Authorization { get; set; } = "";

        public static async Task<List<WordBook>> GetAllWordBookAsync(string language)
        {
            // 发送GET请求并等待响应
            string url = $"{GET_WORD_BOOK}?language={language}&Authorization={Authorization}";
            HttpResponseMessage response = await Client.GetAsync(url);

            // 确保请求成功
            response.EnsureSuccessStatusCode();

            // 读取响应内容
            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseBody);

            // 获取 Study 数组
            JObject jsonObj = JObject.Parse(responseBody);
            JArray dataArray = (JArray)jsonObj["data"];
            return dataArray.ToObject<List<WordBook>>();
        }

        public static async Task<string> AddWordBookAsync(string language, string name)
        {
            // 构建请求体的匿名对象
            var requestData = new
            {
                language = language,
                name = name,
                Authorization = Authorization
            };

            // 将请求对象序列化为 JSON 字符串
            string jsonRequest = JsonConvert.SerializeObject(requestData);
            // 创建请求内容，设置字符编码和内容类型
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            // 发送 POST 请求
            HttpResponseMessage response = await Client.PostAsync(POST_WORD_BOOK, content);

            // 检查响应状态码，如果不是成功状态码则会抛出异常
            response.EnsureSuccessStatusCode();

            // 读取响应内容
            string responseBody = await response.Content.ReadAsStringAsync();

            // 使用 Newtonsoft.Json 解析响应内容，提取 data 下的 id
            dynamic responseObj = JsonConvert.DeserializeObject(responseBody);
            string id = responseObj.data.id;

            return id;
        }

        public static async Task<string> AddWordsToWordBook(string id, string language, List<string> words)
        {
            var requestData = new
            {
                id = id,
                language = language,
                words = words,
                Authorization = Authorization
            };

            string jsonRequest = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await Client.PostAsync(POST_WORD, content);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            dynamic responseObj = JsonConvert.DeserializeObject(responseBody);
            return responseObj.message;
        }

        public static async Task DeleteWordBook(string id, string language, string name)
        {
            var requestData = new
            {
                id = id,
                language = language,
                name = name,
                Authorization = Authorization
            };

            string jsonRequest = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Delete, DELETE_WORD_BOOK)
            {
                Content = content
            };

            HttpResponseMessage response = await Client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            await response.Content.ReadAsStringAsync();
        }
    }
}

