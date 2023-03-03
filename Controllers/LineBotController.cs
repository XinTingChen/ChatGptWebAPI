using isRock.LineBot;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace ChatGptWebAPI.Controllers
{
    public class LineBotController : LineWebHookControllerBase
    {
        public const string adminUserId = "Ue27cd4c8df1ef9e6766b5cb6875a0d0f";
        public static List<Message> chats = new List<Message>();

        public LineBotController()
        {
            this.ChannelAccessToken = "TNwTq+/3kNrVZN+v7A+T0+9ci3trSdCnsHhdyNTwsElhiJvW1JCH+5o8UWR2aFBsmMola+E1DqX2+35uNRqjVFZDTrj3LrihbdIST2d69QVhe7I70sQOZuTlOgEtkKRix1XD6/yCIkq2SQWsNuvO6gdB04t89/1O/w1cDnyilFU=";
        }

        /// <summary> 
        /// ChatGPT
        /// </summary>
        /// <remarks>
        /// ChatGPT
        /// </remarks> 
        /// <returns></returns> 
        /// <response code="200">回傳成功</response> 
        /// <response code="404">找不到資料</response> 
        /// <response code="500">伺服器錯誤</response> 
        [Route("api/ChatGPT")]
        [HttpPost]
        [Produces("application/json")]
        public async Task<IActionResult> ChatGPT()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            try
            {
                var LineEvent = this.ReceivedMessage.events.FirstOrDefault();

                Console.WriteLine(LineEvent.message.text);

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "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");
                    httpClient.DefaultRequestHeaders.Add("OpenAI-Organization", "org-NcvEcjsvwKLpvuooIKIJBQAx");

                    if (LineEvent.message.text.StartsWith("/img "))
                    {
                        JObject requestData = new JObject();
                        requestData.Add("model", "image-alpha-001");
                        requestData.Add("prompt", LineEvent.message.text.TrimStart("/img ".ToArray()));
                        requestData.Add("num_images", 1);
                        var content = new StringContent(requestData.ToString(), System.Text.Encoding.UTF8, "application/json");

                        var response = await httpClient.PostAsync("https://api.openai.com/v1/images/generations", content);
                        response.EnsureSuccessStatusCode();

                        var responseContent = await response.Content.ReadAsStringAsync();

                        JObject jsonResponse = JObject.Parse(responseContent);

                        Utility.ReplyImageMessage(LineEvent.replyToken, jsonResponse["data"][0]["url"].ToString(), jsonResponse["data"][0]["url"].ToString(), this.ChannelAccessToken);
                    }
                    else 
                    {
                        chats.Add(new Message() { role = "user", content = LineEvent.message.text });
                        parameters requestBody = new parameters
                        {
                            model = "gpt-3.5-turbo",
                            messages = chats
                        };
                        var json = JsonConvert.SerializeObject(requestBody);
                        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                        var response = await httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
                        response.EnsureSuccessStatusCode();

                        var responseJson = await response.Content.ReadAsStringAsync();
                        Root responseData = JsonConvert.DeserializeObject<Root>(responseJson);
                        chats.Add(responseData.choices[0].message);

                        Console.WriteLine(responseData.choices[0].message.content);
                        Utility.ReplyMessage(LineEvent.replyToken, responseData.choices[0].message.content, this.ChannelAccessToken);

                        Console.WriteLine(chats.Count);
                    }
                    
                }

            }
            catch (Exception ex)
            {
                this.PushMessage(adminUserId, "發生錯誤:\n" + ex.Message);
            }
            return Ok();
        }


        //[Route("api/test")]
        //[HttpGet]
        //[Produces("application/json")]
        //public async Task<IActionResult> test()
        //{
           

        //    return Ok();
        //}

        class parameters
        {
            public string model { get; set; }
            public List<Message> messages { get; set; }
        }
        public class Choice
        {
            public Message message { get; set; }
            public string finish_reason { get; set; }
            public int index { get; set; }
        }

        public class Message
        {
            public string role { get; set; }
            public string content { get; set; }
        }

        public class Root
        {
            public string id { get; set; }
            public string @object { get; set; }
            public int created { get; set; }
            public string model { get; set; }
            public Usage usage { get; set; }
            public List<Choice> choices { get; set; }
        }

        public class Usage
        {
            public int prompt_tokens { get; set; }
            public int completion_tokens { get; set; }
            public int total_tokens { get; set; }
        }
    }
}
