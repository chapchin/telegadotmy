using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace ConsoleApp1
{
    class Program
    {
        static void Main()
        {
            //Botstart();
            //string res = GetWeather("/su");
            Console.WriteLine(GetWeather("/su"));
            Botstart();
            //Console.ReadKey();

            // https://api.telegram.org/bot615580371:AAFSJ7GZ5F3AiEdkO8pyDb5U9oNTRpcJCZ8/getMe
            // https://api.telegram.org/bot615580371:AAFSJ7GZ5F3AiEdkO8pyDb5U9oNTRpcJCZ8/getUpdates
            // https://api.telegram.org/bot615580371:AAFSJ7GZ5F3AiEdkO8pyDb5U9oNTRpcJCZ8/sendMessage?chat_id=405644586&text= And i see you

        }

        private static string GetWeather(string City)
        {
            string url = String.Empty;
            if (City == "/su") url = @"https://xml.meteoservice.ru/export/gismeteo/point/133.xml";
                            
            else return "такого города нет";
            string data = new WebClient().DownloadString(url);
            var weatherCollection = XDocument.Parse(data)
                                            .Descendants("MMWEATHER")
                                            .Descendants("REPORT")
                                            .Descendants("TOWN")
                                            .Descendants("FORECAST").ToArray();
            //Console.WriteLine(weatherCollection);
            string textWeather = string.Empty;
            foreach (var item in weatherCollection)
            {
                //Console.WriteLine(textWeather);
                textWeather += String.Format(
                "Дата {0}-{1}-{2} {3}:00 - {4} мм.рт.ст. {5}CC \r\n",
                item.Attribute("day").Value,
                item.Attribute("month").Value,
                item.Attribute("year").Value,
                item.Attribute("hour").Value,
                item.Element("PRESSURE").Attribute("max").Value,
                item.Element("TEMPERATURE").Attribute("max").Value
                );
                //Console.WriteLine(textWeather);
            }
            return textWeather;
        }


            //string xmlData = File.ReadAllText(@"C:\folder\catalog.xml");//var xmarr = XDocument.Parse(xmlData)//                        .Descendants("catalog")//                        .Descendants("book")//                        .ToArray;//string text = string.Empty;//foreach (var item in xmarr)//{//    text += String.Format(//        "Id Товара: {0} /r/n Автор: {1}\r\n Наименование: {2} /r/n Стоимость:{3}/r/n/r/n",//        item.Attribute("id").value,//        item.Element("autor").Value.Replace(", ", " "),//        item.Element("title").Value,//        item.Element("price").Vlaue//        );//}//    Console.WriteLine(text);//    File.WriteAllText(@"C:\folder\catalog.txt",text);//#endregion// 615580371:AAFSJ7GZ5F3AiEdkO8pyDb5U9oNTRpcJCZ8// https://api.telegram.org/bot615580371:AAFSJ7GZ5F3AiEdkO8pyDb5U9oNTRpcJCZ8/getMe

            private static void Botstart()
            {
                //throw new NotImplementedException();
                Console.WriteLine("Бот запущен...");
                int update_id = 0;
                string messageFromId = "";
                string messageText = "";
                //string messageName = "";
                string firstName = "";
                string token = "615580371:AAFSJ7GZ5F3AiEdkO8pyDb5U9oNTRpcJCZ8";
                WebClient WebClient = new WebClient();
                string startUrl = $"https://api.telegram.org/bot{token}";
                while (true)
                {
                    string url = $"{startUrl}/getUpdates?offset={update_id + 1}";
                ////////////////// https://api.telegram.org/bot615580371:AAFSJ7GZ5F3AiEdkO8pyDb5U9oNTRpcJCZ8/getUpdates?offset=1
                // ОШИБКА тут закрывает канал сервер телеграмм
                string response = WebClient.DownloadString(url);
                    var Message = JObject.Parse(response)["result"].ToArray();
                    foreach (var currentMessage in Message)
                    {
                        update_id = Convert.ToInt32(currentMessage["update_id"]);
                        try
                        {
                            firstName = currentMessage["message"]["from"]["first_name"].ToString();
                            messageFromId = currentMessage["message"]["from"]["id"].ToString();
                            messageText = currentMessage["message"]["text"].ToString();
                            Console.WriteLine($"{firstName} {messageFromId} {messageText}");

                        //messageText = "Вы прислали: " + messageText;
                        messageText = GetWeather(messageText);
                        Console.WriteLine(url);
                        url = $"{startUrl}/sendMessage?chat_id={messageFromId}&text={messageText}";
                        
                        WebClient.DownloadString(url);

                        }
                        catch { }

                    }

                    Thread.Sleep(5000);

                }
            }

        
    }
}
