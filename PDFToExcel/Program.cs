using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Net.Http;
using System.Net.Http.Headers;

namespace PDFToExcel
{
    class Program
    {
        static void Main(string[] args)
        {

            ////////////////////////////////
            String path =                       "C:/xampp/htdocs/PdfParser/";
            string[] files =                    System.IO.Directory.GetFiles(path, "mayur*.pdf");
            foreach (string jej in files)
            {
                Console.Write(jej + " ");
            }
            ////////////////////////////////
            String pdf =                        "C:/xampp/htdocs/PdfParser/mayurTest5.pdf";
            String Container =                  "";
            int banderola =                     0;
            var text =                          "";
            String resultText =                 "";
            string[] stringArray =              new string[6];
            int contador =                      0;
            string septimo =                    "";
            PdfReader reader;
            try
            {
                reader =                            new PdfReader(pdf);
                PdfReaderContentParser parser =     new PdfReaderContentParser(reader);
                ITextExtractionStrategy strategy;
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    strategy =                          parser.ProcessContent(i, new SimpleTextExtractionStrategy());
                    var s =                             PdfTextExtractor.GetTextFromPage(reader, i, strategy);
                    text +=                             (strategy.GetResultantText());
                }
                resultText =                    resultText.Replace("-\n", "");
                string[] sentences =            text.Split(new string[] { "\n" }, StringSplitOptions.None);
                foreach (string marker in sentences)
                {
                    if ((!marker.Contains("JZ") && marker.Length > 45))// || (!marker.Contains("INFD JZ") && marker.Length > 45))
                    {
                        string[] words =                    marker.Split(new string[] { " " }, StringSplitOptions.None);
                        //foreach (string palabra in words)
                        for (int i = 0; i < words.Length; i++)
                        {
                            string palabra =                    words[i];
                            if ((i+1) < words.Length)
                            {
                                septimo =                           words[i+1];
                            }
                            if(palabra != "NIL")
                            {
                                Console.Write(palabra + " ");
                            }else
                            {
                                contador =                          0;
                                continue;
                            }
                            stringArray[contador] =             palabra;
                            if (contador == 5)
                            {
                                var primis =                        stringArray[0];
                                var seguns =                        stringArray[1];
                                var tercis =                        "";
                                var cuarto =                        "";
                                var quinto =                        "";
                                var sexto =                         "";
                                if (seguns[0] == '/' || (seguns[0] == 'M' && seguns[1] == 'I' && seguns[2] == 'L'))
                                {
                                    primis =                        primis + " " + seguns;
                                    tercis =                        stringArray[2];
                                    cuarto =                        stringArray[3];
                                    quinto =                        stringArray[4];
                                    sexto =                         stringArray[5];
                                }
                                else
                                {
                                    tercis =                        seguns;
                                    cuarto =                        stringArray[2];
                                    quinto =                        stringArray[3];
                                    sexto =                         stringArray[4];
                                }
                                if (primis != "NIL")
                                {
                                    contador =                          -1;
                                    Console.Write("\n");
                                    var client =                        new HttpClient();
                                    try
                                    {
                                        /*var URI =                           "https://eplserver.net/erp/tools/BoxPalletID/Mayur/Mayur.php?jej=1&Product="+primis+"&Roll="+tercis+"&Qty="+cuarto+"&Lot="+quinto+"&Part="+sexto+"&Container=Process";
                                        using (var response = client.GetAsync(URI).Result)
                                        {
                                            string responseData =               response.Content.ReadAsStringAsync().Result;
                                        }*/
                                    }
                                    catch (HttpRequestException e)
                                    {
                                        System.Diagnostics.Debug.WriteLine(e);
                                    }
                                    if (septimo == "EP" || septimo == "SL")
                                    {
                                        i++;
                                        contador =                          -1;
                                    }
                                }
                            }
                            contador++;
                        }
                    }
                    else if(marker.Contains("INFD"))
                    {
                        if (marker.Contains("INFD-JZ"))
                        {
                            string[] words =                            marker.Split(new string[] { " " }, StringSplitOptions.None);
                            foreach (string palabra in words)
                            {
                                if (palabra.Contains("CONTAINER"))
                                {
                                    if (banderola == 0)
                                    {
                                        Container =                                 palabra.Replace("CONTAINER", "");
                                        banderola =                                 1;
                                    }
                                }
                            }
                        }
                        else
                        {
                            string[] words =                            marker.Split(new string[] { " " }, StringSplitOptions.None);
                            for (int i = 0; i < words.Length; i++)
                            {
                                var palabra =                               words[i];
                                if (palabra == "INFD")
                                {
                                    if (words[i+1] == "JZ")
                                    {
                                        if (banderola == 0)
                                        {
                                            string Cont =                               palabra + " " + words[i + 1] + " " + words[i + 2];
                                            Container =                                 Cont.Replace("CONTAINER", "");
                                            banderola =                                 1;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                var cliente =                   new HttpClient();
                var URO =                       "https://eplserver.net/erp/tools/BoxPalletID/Mayur/Mayur.php?jij=1&Container=" + Container;
                using (var response = cliente.GetAsync(URO).Result)
                {
                    string responseData =           response.Content.ReadAsStringAsync().Result;
                    if (responseData == "success")
                    {
                        Console.Write("\nFinished Loading the File for Order "+Container);
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("JEJ " + e);
            }
            Console.ReadKey();
        }
    }
}