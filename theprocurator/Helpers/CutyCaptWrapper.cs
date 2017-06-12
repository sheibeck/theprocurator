using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;

public class CutyCaptWrapper
{
    /// <summary>
    /// 1 - small
    /// 2 - medium
    /// 3 - large
    /// </summary>
    public int ThumbNailSize { get; set; }
    public bool ThumbKeepAspectRatio { get; set; }
    public int ThumbExpiryTimeInHours { get; set; }
    public string ScreenShotPath { get; set; }
    public string CutyCaptPath { get; set; }
    //public string CutyCaptWorkingDirectory { get; set; }
    public string CutyCaptDefaultArguments { get; set; }
    private int ThumbWidth { get; set; }
    private int ThumbHeight { get; set; }

    public CutyCaptWrapper()
    {
        //default values
        ThumbNailSize = 3;
        ThumbKeepAspectRatio = false;
        ThumbExpiryTimeInHours = 168; //1 week
        ScreenShotPath = HttpContext.Current.Server.MapPath("~/Content/CharacterSheet/Thumbnails/"); // must be within the web root
        CutyCaptPath = HttpContext.Current.Server.MapPath("~/tools/CutyCapt.exe"); // must be within the web root

        if (!Directory.Exists(ScreenShotPath))
        {
            Directory.CreateDirectory(ScreenShotPath);
        }

        CutyCaptDefaultArguments = " --max-wait=10000 --out-format=png --javascript=on --java=off --plugins=off --js-can-open-windows=off --js-can-access-clipboard=off --private-browsing=on";

    }

    /// <summary>
    /// Checks if there is a cached screenshot of the website and returns url path to thumbnail of the website in order to use ase html image element source
    /// Usage example: &lt;img src=&quot;&lt;%=CutyCaptWrapper().GetScreenShot(&quot;http://google.com&quot;)%&gt;&quot; alt=&quot;&quot;&gt;
    /// </summary>
    public string GetScreenShot(string url, string fileName)
    {
        if (true) //IsURLValid(url))
        {

            if (!Directory.Exists(ScreenShotPath))
            {
                Directory.CreateDirectory(ScreenShotPath);
            }
            //set thumbnail sizes
            SetThumbnailSize();
            string ScreenShotFileName = ScreenShotPath + fileName + "_full.png"; //GetScreenShotFileName(url);
            string ScreenShotThumbnailFileName = ScreenShotPath + GetScreenShotThumbnailFileName(ScreenShotFileName, ThumbWidth, ThumbHeight);
            string RunArguments = " --url=" + url + " --out=" + ScreenShotFileName + CutyCaptDefaultArguments;

            FileInfo ScreenShotThumbnailFileNameInfo = new FileInfo(ScreenShotThumbnailFileName);

            if (!ScreenShotThumbnailFileNameInfo.Exists || ScreenShotThumbnailFileNameInfo.CreationTime < DateTime.Now.AddMinutes(-5))
            {
          
                    ProcessStartInfo info = new ProcessStartInfo(CutyCaptPath, RunArguments);
                    info.UseShellExecute = false;
                    info.RedirectStandardInput = true;
                    info.RedirectStandardError = true;
                    info.RedirectStandardOutput = true;
                    info.CreateNoWindow = true;
                    //info.WorkingDirectory = CutyCaptWorkingDirectory;
                    using (Process scr = Process.Start(info))
                    {
                        //string output = scr.StandardOutput.ReadToEnd();
                        scr.WaitForExit();
                        ThumbnailCreate(ScreenShotFileName, ScreenShotThumbnailFileName, ThumbWidth, ThumbHeight, ThumbKeepAspectRatio);
                        //delete original file
                        File.Delete(ScreenShotFileName);
                        //return output;
                    }
            }
            return GetRelativeUri(ScreenShotThumbnailFileName);
        }
        else
        {
            return "Wrong URL";
        }
    }

    private void ThumbnailCreate(string sourceFilePath, string outFilePath, int NewWidth, int MaxHeight, bool keepAspectRatio)
    {
        using (Image FullsizeImage = Image.FromFile(sourceFilePath))
        {
            int NewHeight = MaxHeight;
            if (keepAspectRatio)
            {
                NewHeight = FullsizeImage.Height * NewWidth / FullsizeImage.Width;
                if (NewHeight > MaxHeight)
                {
                    NewWidth = FullsizeImage.Width * MaxHeight / FullsizeImage.Height;
                    NewHeight = MaxHeight;
                }
            }
            using (Image NewImage = FullsizeImage.GetThumbnailImage(NewWidth, NewHeight, null, IntPtr.Zero))
            {
                try
                {
                    if (File.Exists(outFilePath))
                        File.Delete(outFilePath);

                    NewImage.Save(outFilePath, ImageFormat.Png);
                }
                catch
                {
                    // windows is holding the file open. Just ignore it for now
                }
            }
        }
    }
    private string GetScreenShotFileName(string url)
    {
        Uri uri = new Uri(url);
        return uri.Host.Replace(".", "_") + uri.LocalPath.Replace("/", "_") + ".png";
    }
    private string GetScreenShotThumbnailFileName(string sourceFilename, int width, int height)
    {
        FileInfo sourceFile = new FileInfo(sourceFilename);
        string shortFilename = sourceFile.Name;      
        string ext = Path.GetExtension(shortFilename);
        //string replacementEnding = String.Format("{0}x{1}", width, height) + ext;
        return shortFilename.Replace("_full", "");
    }
    private string GetRelativeUri(string pathToFile)
    {
        string rootPath = HttpContext.Current.Server.MapPath("~");
        return pathToFile.Replace(rootPath, "").Replace(@"\", "/");
    }
    private bool IsURLValid(string url)
    {
       string strRegex = "^(https?://)"
        + "?(([0-9a-z_!~*'().&=+$%-]+: )?[0-9a-z_!~*'().&=+$%-]+@)?" //user@ 
        + @"(([0-9]{1,3}\.){3}[0-9]{1,3}" // IP- 199.194.52.184 
        + "|" // allows either IP or domain 
        + @"([0-9a-z_!~*'()-]+\.)*" // tertiary domain(s)- www. 
        + @"([0-9a-z][0-9a-z-]{0,61})?[0-9a-z]\." // second level domain 
        + "[a-z]{2,6})" // first level domain- .com or .museum 
        + "(:[0-9]{1,4})?" // port number- :80 
        + "((/?)|" // a slash isn't required if there is no file name 
        + "(/[0-9a-z_!~*'().;?:@&=+$,%#-]+)+/?)$";
        Regex re = new Regex(strRegex, RegexOptions.Compiled | RegexOptions.IgnoreCase);

        if (re.IsMatch(url))
            return (true);
        else
            return (false);
    }
    private void SmallThumbnail()
    {
        ThumbWidth = 200;
        ThumbHeight = 150;
    }
    private void MediumThumbnail()
    {
        ThumbWidth = 240;
        ThumbHeight = 190;
    }
    private void LargeThumbnail()
    {
        ThumbWidth = 320;
        ThumbHeight = 270;
    }
    private void SetThumbnailSize()
    {
        if (ThumbNailSize == 1)
        {
            SmallThumbnail();
        }
        if (ThumbNailSize == 2)
        {
            MediumThumbnail();
        }
        if (ThumbNailSize == 3)
        {
            LargeThumbnail();
        }
        else
        {
            ThumbNailSize = 1;
        }
    }

}