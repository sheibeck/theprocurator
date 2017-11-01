using HiQPdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace theprocurator.Helpers
{
    public static class OutputHelper
    {
        public static FileStreamResult PrintToPdf(this Controller controller, string id, string fileName)
        {
            // create the HTML to PDF converter                
            HtmlToPdf htmlToPdfConverter = new HtmlToPdf();

            // set browser width
            htmlToPdfConverter.BrowserWidth = 670;
            htmlToPdfConverter.BrowserHeight = 900;

            // set PDF page size and orientation
            htmlToPdfConverter.Document.PageSize = PdfPageSize.Letter;
            htmlToPdfConverter.Document.PageOrientation = PdfPageOrientation.Landscape;

            // set PDF page margins
            htmlToPdfConverter.Document.Margins = new PdfMargins(5);

            htmlToPdfConverter.WaitBeforeConvert = 2;

            var request = HttpContext.Current.Request;
            var address = string.Format("{0}://{1}", request.Url.Scheme, request.Url.Authority);

            string url = string.Format("{0}/{1}/Print/{2}", address, controller.RouteData.Values["controller"], id);

            // convert URL to a PDF file
            var filePath = string.Format("{0}\\{1}.pdf", HttpContext.Current.Server.MapPath("~/App_Data"), fileName);
            //htmlToPdfConverter.ConvertUrlToFile(url, filePath);
            //return filePath;
            
            // convert HTML to PDF
            byte[] pdfBuffer = null;
            pdfBuffer = htmlToPdfConverter.ConvertUrlToMemory(url);

            MemoryStream ms = new MemoryStream(pdfBuffer);

            return new FileStreamResult(ms, "application/pdf");
        }

        public static void ToThumbnail(this Controller controller, HttpRequestBase request, string id)
        {           
            var filePath = System.Web.Hosting.HostingEnvironment.MapPath(String.Format("~/Content/CharacterSheet/Thumbnails/{0}.png", id));

            string url = String.Format("http://api.screenshotmachine.com/?key=f9b7da&dimension=640x480&format=png&cacheLimit=0&timeout=5000&url={0}CharacterSheets/Print/{1}", GetBaseUrl(request), id);

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.AutomaticDecompression = DecompressionMethods.GZip;

            WebClient client = new WebClient();            
            client.DownloadFileAsync(new Uri(url), filePath);
        }
        
        public static string GetBaseUrl()
        {
            var request = HttpContext.Current.Request;
            var appUrl = HttpRuntime.AppDomainAppVirtualPath;

            if (appUrl != "/")
                appUrl = "/" + appUrl;

            var baseUrl = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, appUrl);

            return baseUrl;
        }

        public static string GetBaseUrl(HttpRequestBase request)
        {            
            var appUrl = HttpRuntime.AppDomainAppVirtualPath;

            if (appUrl != "/")
                appUrl = "/" + appUrl;

            var baseUrl = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, appUrl);

            return baseUrl;
        }
    }
}