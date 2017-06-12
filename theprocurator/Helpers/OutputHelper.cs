using HiQPdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

        public static void ToThumbnail(this Controller controller, string id)
        {
           
            var filePath = HttpContext.Current.Server.MapPath(String.Format("~/Content/CharacterSheet/Thumbnails/{0}.png", id));


#if DEBUG
            string url = String.Format("http://api.screenshotmachine.com/?key=f9b7da&dimension=640x480&format=png&cacheLimit=1&url=null)", GetBaseUrl(), id);
#endif

#if !DEBUG
            string url = String.Format("http://api.screenshotmachine.com/?key=f9b7da&dimension=640x480&format=png&cacheLimit=1&url=http://{0}/CharacterSheets/Print/{1})", GetBaseUrl(), id);
#endif


            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;
            
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())           
            using (BinaryReader reader = new BinaryReader(response.GetResponseStream()))
            {
                Byte[] lnByte = reader.ReadBytes(1 * 1024 * 1024 * 10);
                using (FileStream file = new FileStream(filePath, FileMode.Create))
                {
                    file.Write(lnByte, 0, lnByte.Length);
                }
            }
                    
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
    }
}