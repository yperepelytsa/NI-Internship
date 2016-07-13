using System.Text.RegularExpressions;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;

namespace LibraryBot
{
    public class Program
    {
        //delay was set to 500 to make it work faster, may not work properly for slow internet, default is 5000
        static int delay = 500;// delay in ms
        public static void Main(string[] args)
        {
             Console.WriteLine("Bot started...");
             List<string> pages=getPageList(getPageContent("http://nz.ukma.edu.ua/index.php?option=com_content&task=category&sectionid=10&id=60&Itemid=47"));
             Console.WriteLine("Found "+pages.Count+" pages...");
             int count = 0;
             foreach (string page in pages)
                 {              
                     Console.WriteLine("Processing page: " + page);
                     List<string> pdfs = getPdfFiles(page);
                     Console.WriteLine("Found " + pdfs.Count + " files...");
                     foreach (string pdf in pdfs)
                         {
                         count++;                            
                         Console.WriteLine("Downloading file: " + pdf);
                         downloadPdf(pdf);
                         }
                 }
            Console.WriteLine("Files processed: " + count);
            

        }
        public static List<string> getPageList(string input)
        {
            Regex r2 = new Regex(@"http.*task=view.*Itemid=47");
            List<string> reslist = new List<string>();
            foreach (Match m in r2.Matches(input))
            {
                reslist.Add(m.ToString());
            }
            for (int i=0;i<reslist.Count;i++)
            {
                reslist[i]= reslist[i].Replace("&amp;", "&");
            }
            return reslist;
        }
        public static string getPageContent(string uri)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();           
            HttpClient c = new HttpClient();

            Task<byte[]> bt = c.GetByteArrayAsync(uri);
            Task<string> t = c.GetStringAsync(uri);
            byte[] s = bt.Result;
            string data = Encoding.UTF8.GetString(s);
            while (stopWatch.ElapsedMilliseconds < delay) { 
            Thread.Sleep(100);
            }
            return data;
        }
        public static List<string> getPdfFiles(string uri)
        {
            string s=getPageContent(uri);
            List<string> reslist = new List<string>();
            Regex r2 = new Regex(@"(www|elib)(\w|_|/|\.|-|%)*pdf");
            foreach (Match m in r2.Matches(s))
            {            
                reslist.Add(m.ToString());
            }
            for (int i = 0; i < reslist.Count; i++)
            {
                reslist[i]=reslist[i].Replace("www.", "http://");
                if (!reslist[i].StartsWith("http"))
                {
                    reslist[i] = "http://" + reslist[i];
                }
            }
            return reslist;
        }

        public static void downloadPdf(string uristr)
        {
            
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            if (!Directory.Exists("files"))
            {
                Directory.CreateDirectory("files");
            }
            Uri uri = new Uri(uristr);
            HttpClient client = new HttpClient();

            client.GetAsync(uri).ContinueWith(
                (requestTask) =>
                {
                    HttpResponseMessage response = requestTask.Result;
                    response.EnsureSuccessStatusCode();
                response.Content.ReadAsFileAsync("files/" + Path.GetFileName(uri.LocalPath), false).ContinueWith(
                        (readTask) =>
                        {
                            Process process = new Process();
                            process.StartInfo.FileName = "files/" + Path.GetFileName(uri.LocalPath);
                            process.Start();
                        });
                });
            while (stopWatch.ElapsedMilliseconds < delay)
            {
                Thread.Sleep(100);
            }
        }
     
    }
    public static class HttpContentExtensions
    {
        public static Task ReadAsFileAsync(this HttpContent content, string filename, bool overwrite)
        {
            string clearfilename = filename.Replace(".pdf", "");
            if (!overwrite && File.Exists(filename))
            {
                int i = 1;
                while (File.Exists(filename))
                {
                    filename = String.Format("{0}{1}.pdf", clearfilename,i);
                    i++;
                }
            }
            string pathname = Path.GetFullPath(filename);
            FileStream fileStream = null;
            try
            {
                fileStream = new FileStream(pathname, FileMode.Create, FileAccess.Write, FileShare.None);
                return content.CopyToAsync(fileStream).ContinueWith(
                    (copyTask) =>
                    {
                        fileStream.Flush();
                    });
            }
            catch
            {
                if (fileStream != null)
                {
                    fileStream.Flush();
                }

                throw;
            }

        }
    }
}
