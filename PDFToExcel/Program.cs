using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
//using iTextSharp.text.pdf.parser;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Xml;
using System.Net;
using System.Text.RegularExpressions;

namespace PDFToExcel
{
    class Program
    {
        static void Main(string[] args)
        {
            X:
            int each = 0;
            int contador = 0;
            string container = "";
            string cosa;
            int flag = 0;
            string[] stringArray = new string[5];
            //String path = "C:/xampp/htdocs/PdfParser";
            string baseURL = "https://eplserver.net/erp/tools/BoxPalletID/Mayur/";
            WebClient clientin = new WebClient();
            string content = clientin.DownloadString(baseURL);
            string regex = "<a href=.*?(.*?)>";
            MatchCollection matches = Regex.Matches(content, regex);
            if (matches.Count > 0)
            {
                foreach (Match m in matches)
                {
                    if (m.ToString().Contains("INFD"))
                    {
                        string naem = m.Groups[1].ToString().Replace("\"", "");
                        cosa = naem;
                        //Console.WriteLine("Inner DIV: {0}", naem);
                        string URL = baseURL + naem;
                        Console.WriteLine("PDF: " + URL);
                        using (WebClient clienton = new WebClient())
                        {
                            clienton.DownloadFile(URL, "C:/Mayur/process.pdf");
                            string first;
                            string second;
                            string third;
                            string fourth;
                            string fifth;
                            ////////////////////////
                            string pathToPdf = @"C:/Mayur/process.pdf";
                            System.Threading.Thread.Sleep(3000);
                            //string pathToXml = Path.ChangeExtension(pathToPdf, ".xml");
                            //SautinSoft.PdfFocus f = new SautinSoft.PdfFocus();
                            //f.XmlOptions.ConvertNonTabularDataToSpreadsheet = true;
                            SautinSoft.PdfFocus f = new SautinSoft.PdfFocus();
                            f.OpenPdf("C:/Mayur/process.pdf");
                            f.ToXml("C:/Mayur/process.xml");
                            f.XmlOptions.ConvertNonTabularDataToSpreadsheet = true;
                            //f.OpenPdf(pathToPdf);
                            if (f.PageCount > 0)
                            {
                                int result = f.ToXml("C:/Mayur/process.xml");
                                //Show HTML document in browser 
                                if (result == 0)
                                {
                                    //System.Diagnostics.Process.Start("C:/Users/luisr/Documents/process.xml");
                                }
                            }
                            f.ClosePdf();
                            ///////////////////////////////
                            ///////////////////////////////
                            XmlTextReader reader = new XmlTextReader("C:/Mayur/process.xml");
                            while (reader.Read())
                            {
                                switch (reader.NodeType)
                                {
                                    case XmlNodeType.Element: // The node is an element.
                                        if (XmlNodeType.Element.ToString() == "Cell")
                                        {
                                            //Console.Write("<" + reader.Name);
                                            //while (reader.MoveToNextAttribute()) // Read the attributes.
                                            //Console.Write(" " + reader.Name + "='" + reader.Value + "'");
                                            //Console.WriteLine(">");
                                        }
                                        break;
                                    case XmlNodeType.Text: //Display the text in each element.
                                        if (!reader.Value.Contains("sautinsoft") && !reader.Value.Contains("Click") && !reader.Value.Contains("(Licensed") && !reader.Value.Contains("CODE") && !reader.Value.Contains("Converted") && !reader.Value.Contains("trial") && !reader.Value.Contains("PRODUCT CODE") && !reader.Value.Contains("ROLL") && !reader.Value.Contains("QTY") && !reader.Value.Contains("LOT") && !reader.Value.Contains("PART") && !reader.Value.Contains("INFD"))
                                        {
                                            //Console.WriteLine(reader.Value);
                                            if (reader.Value != null && reader.Value != "" && reader.Value != "NIL")
                                            {
                                                Console.Write(reader.Value + " ");
                                                stringArray[each] = reader.Value;
                                                each++;
                                                if (each == 5)
                                                {
                                                    contador++;
                                                    Console.Write(" " + contador + "\n");
                                                    each = 0;
                                                    //////////////////////
                                                    first = stringArray[0];
                                                    second = stringArray[1];
                                                    third = stringArray[2];
                                                    fourth = stringArray[3];
                                                    fifth = stringArray[4];
                                                    var client = new HttpClient();
                                                    var URI = "https://eplserver.net/erp/tools/BoxPalletID/Mayur/Mayur.php?jej=1&Product=" + first + "&Roll=" + second + "&Qty=" + third + "&Lot=" + fourth + "&Part=" + fifth + "&Container=Process";
                                                    using (var response = client.GetAsync(URI).Result)
                                                    {
                                                        string responseData = response.Content.ReadAsStringAsync().Result;
                                                        if (responseData == "success")
                                                        {
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else if (reader.Value.Contains("'INFD") && !reader.Value.Contains("CONTAINER") && flag == 0)
                                        {
                                            //Console.Write(reader.Vlue);
                                            string s = reader.Value;
                                            Char charRange = '-';
                                            int endIndex = 0;
                                            int startIndex = s.IndexOf("'");
                                            if (reader.Value.Contains("'INFD-"))
                                            {
                                                endIndex = s.LastIndexOf(charRange);
                                            }
                                            else
                                            {
                                                endIndex = s.IndexOf(charRange);
                                            }
                                            int length = endIndex - startIndex - 1;
                                            startIndex++;
                                            container = s.Substring(startIndex, length);
                                            flag = 1;
                                        }
                                        break;
                                }
                            }
                            reader.Close();
                            var cliente = new HttpClient();
                            var URO = "https://eplserver.net/erp/tools/BoxPalletID/Mayur/Mayur.php?jij=1&Container=" + container + "&naem=" + cosa;
                            using (var response = cliente.GetAsync(URO).Result)
                            {
                                string responseData = response.Content.ReadAsStringAsync().Result;
                                if (responseData == "success")
                                {
                                    Console.Write("\nFinished Loading the File for Order " + container);
                                    each = 0;
                                    contador = 0;
                                    first = "";
                                    second = "";
                                    third = "";
                                    fourth = "";
                                    fifth = "";
                                    container = "";
                                    flag = 0;
                                    System.Threading.Thread.Sleep(3000);
                                    
                                    File.Delete("C:/Mayur/process.xml");
                                    File.Delete("C:/Mayur/process.pdf");
                                    Console.Clear();
                                }
                            }
                            ///////////////////////////////
                            ///////////////////////////////
                        }
                    }
                }
            }
            else
            {
                Console.Write("No Pdf Files Found\n");
            }
            System.Threading.Thread.Sleep(60000);
            goto X;
            if (Console.ReadKey(true).Key == ConsoleKey.Escape)
            {
                Environment.Exit(0);
            }
        }
    }
}