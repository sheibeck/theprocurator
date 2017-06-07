using HiQPdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace theprocurator.Helpers
{
    public static class PrintHelper
    {
        public static FileStreamResult PrintToPdf(this Controller controller, string id, string fileName)
        {
            // create the HTML to PDF converter                
            HtmlToPdf htmlToPdfConverter = new HtmlToPdf();

            // set browser width
            htmlToPdfConverter.BrowserWidth = 1024;
            htmlToPdfConverter.BrowserHeight = 768;

            // set PDF page size and orientation
            htmlToPdfConverter.Document.PageSize = PdfPageSize.Letter;
            htmlToPdfConverter.Document.PageOrientation = PdfPageOrientation.Landscape;

            // set PDF page margins
            htmlToPdfConverter.Document.Margins = new PdfMargins(5);

            htmlToPdfConverter.WaitBeforeConvert = 2;

            var request = HttpContext.Current.Request;
            var address = string.Format("{0}://{1}", request.Url.Scheme, request.Url.Authority);

            string url = string.Format("{0}/{1}/Print/{2}", address, controller.RouteData.Values["controller"].ToString(), id);

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
    }
}