using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace csharplab07
{
    class Program
    {
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public static void ProcessDirectory(string targetDirectory, string outputDir, int width, int height)
        {
            var allowedExtensions = new[] { ".jpg", ".png", ".bmp", "jpeg", ".tiff", ".img"};
            var files = Directory
                .GetFiles(targetDirectory)
                .Where(file => allowedExtensions.Any(file.ToLower().EndsWith))
                .ToList();

            if (!Directory.Exists(outputDir))
            {
                DirectoryInfo di = Directory.CreateDirectory(outputDir);
            }


            Image imag;
            string path = "";
            foreach (string fileName in files) {
                imag = Bitmap.FromFile(fileName);
                string fname = getNextFileName(Path.Combine(outputDir, Path.GetFileName(fileName)));               
                path = Path.Combine(outputDir, Path.GetFileName(fname));
                ImageFormat format = imag.RawFormat;
                Bitmap bmap = ResizeImage(imag, width, height);
                bmap.Save(path, format);              
            }
        }

        private static string getNextFileName(string fileName)
        {
            string extension = Path.GetExtension(fileName);

            int i = 0;
            while (File.Exists(fileName))
            {
                if (i == 0)
                    fileName = fileName.Replace(extension, "(" + ++i + ")" + extension);
                else
                    fileName = fileName.Replace("(" + i + ")" + extension, "(" + ++i + ")" + extension);
            }

            return fileName;
        }

        static void Main(string[] args)
        {
            int width = 0;
            int height = 0;
            string inputDir = Directory.GetCurrentDirectory();
            string outputDir = Path.Combine(Directory.GetCurrentDirectory(), "copy");
            bool rightArgument = false;
            Regex regexRes = new Regex(@"-res=(\d+)x(\d+)");
            Regex regexInput = new Regex(@"-input=(.+)");
            Regex regexOutput = new Regex(@"-output=(.+)");
            foreach (var argument in args)
            {
                Match match = regexRes.Match(argument);
                if (match.Success)
                {
                    width = Int32.Parse(match.Groups[1].Value);
                    height = Int32.Parse(match.Groups[2].Value);
                    rightArgument = true;
                }
                match = regexInput.Match(argument);
                if (match.Success)
                {
                    inputDir = Path.GetFullPath(match.Groups[1].Value);
                    rightArgument = true;
                }
                match = regexOutput.Match(argument);
                if (match.Success)
                {
                    outputDir = Path.GetFullPath(match.Groups[1].Value);
                    rightArgument = true;
                }
                if (!rightArgument)
                {
                    Console.WriteLine($"Undefined argument \"{argument}\"");
                    Console.ReadLine();
                    Environment.Exit(-1);
                }
                rightArgument = false;
            }

            Console.WriteLine($"width: {width}");
            Console.WriteLine($"height: {height}");
            Console.WriteLine($"inputdir: {inputDir}");
            Console.WriteLine($"outputdir: {outputDir}");
           
            if (Directory.Exists(inputDir))
            {
                ProcessDirectory(inputDir,outputDir, width, height);
            }
            else
            {
                Console.WriteLine("Incorrect inputdir.");
            }
            Console.ReadLine();
        }

    }
}
