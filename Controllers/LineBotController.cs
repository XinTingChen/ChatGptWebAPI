using ChatGPTSharp;
using isRock.LineBot;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace ChatGptWebAPI.Controllers
{
    //[ApiController]
    //[Route("[controller]")]
    public class LineBotController : LineWebHookControllerBase
    {
        public const string adminUserId = "Ue27cd4c8df1ef9e6766b5cb6875a0d0f";

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
                ChatGPTClient client = new ChatGPTClient("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", "gpt-3.5-turbo");

                var msg = await client.SendMessage(LineEvent.message.text);

                this.ReplyMessage(LineEvent.replyToken, msg.Response);
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
        //    try
        //    {
        //        var LineEvent = this.ReceivedMessage.events.FirstOrDefault();
        //        ChatGPTClient client = new ChatGPTClient("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", "gpt-3.5-turbo");

        //        var msg = await client.SendMessage(LineEvent.message.text);

        //        this.ReplyMessage(LineEvent.replyToken, msg.Response);
        //    }
        //    catch (Exception ex)
        //    {
        //        this.PushMessage(adminUserId, "發生錯誤:\n" + ex.Message);
        //    }
        //    return Ok();
        //}
    }
}
