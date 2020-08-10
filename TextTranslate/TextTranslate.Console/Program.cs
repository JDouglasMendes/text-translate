using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;

namespace TextTranslate.Console
{
    public class Program
    {
        private static async System.Threading.Tasks.Task Main()
        {            
            var configuration = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                 .AddJsonFile("appsettings.json", false)
                 .Build();
            string exit = string.Empty;
            while (exit != "exit")
            {
                System.Console.Write("Type the phrase you'd like to translate? ");
                
                string textToTranslate = System.Console.ReadLine();
                
                var translator = Translator.CreateFromKeys(configuration.GetSection("Translator:EndPoint").Value,
                                                            configuration.GetSection("Translator:SubscriptionKey").Value)
                    .From(TextTranslatorExtensions.ToEnglish)
                    .To(TextTranslatorExtensions.ToPortugueseBR,
                        TextTranslatorExtensions.ToGerman,
                        TextTranslatorExtensions.ToItalian);

                var result = await translator.TranslateText(textToTranslate);
                foreach (var item in result)
                {
                    System.Console.WriteLine($"SouceText             : {item.SourceText}");
                    item.Translations.ToList().ForEach(x =>
                    {
                        System.Console.WriteLine($"Test                  : {x.Text}");
                        System.Console.WriteLine($"To                    : {x.To}");
                        System.Console.WriteLine($"Transliteration.Text  : {x.Transliteration?.Text }");
                        System.Console.WriteLine($"Transliteration.Script: {x.Transliteration?.Script }");
                        System.Console.WriteLine("-----------------------------------------------------");
                    });
                }
                System.Console.WriteLine("Press any key to continue OR 'exit'.");
                exit = System.Console.ReadLine()?.ToLower();
            }            
        }
    }
}