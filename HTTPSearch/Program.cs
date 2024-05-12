using System;
using System.Net.Http;
using HtmlAgilityPack;

class Program
{
    static async System.Threading.Tasks.Task Main(string[] args)
    {
        Console.WriteLine("Введите URL сайта для поиска:");
        string siteUrl = Console.ReadLine().Trim();

        
        if (!Uri.TryCreate(siteUrl, UriKind.Absolute, out Uri validatedUrl))
        {
            Console.WriteLine("Некорректный формат URL.");
            return;
        }

        Console.WriteLine("Введите текст для поиска:");
        string searchTerm = Console.ReadLine().Trim();

        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            Console.WriteLine("Текст для поиска должен быть заполнен.");
            return;
        }

        HttpClient httpClient = new HttpClient();
        try
        {
            string html = await httpClient.GetStringAsync(validatedUrl);

            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var nodesWithSearchTerm = htmlDocument.DocumentNode.SelectNodes($"//*[contains(text(), '{searchTerm}')]");
            if (nodesWithSearchTerm != null)
            {
                foreach (var node in nodesWithSearchTerm)
                {
                    string fragment = node.InnerText;
                    string pageUrl = siteUrl;
                    string fullContent = htmlDocument.DocumentNode.InnerHtml;

                    
                    WriteToLog(pageUrl, fragment, fullContent);
                }
            }
            else
            {
                Console.WriteLine($"Текст '{searchTerm}' не найден на странице {siteUrl}.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла ошибка: {ex.Message}");
        }
    }

    static void WriteToLog(string pageUrl, string fragment, string fullContent)
    {
        string logFilePath = "log.txt";

        using (System.IO.StreamWriter sw = System.IO.File.AppendText(logFilePath))
        {
            sw.WriteLine("Page URL: " + pageUrl);
            sw.WriteLine("Fragment: " + fragment);
            sw.WriteLine("Full Content: " + fullContent);
            sw.WriteLine("----------------------------------");
        }
    }
}
