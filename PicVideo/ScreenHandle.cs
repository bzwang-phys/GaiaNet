using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System;
using System.IO;

namespace GaiaNet.PicVideo
{
    public class ScreenHandle
    {
        public void ScreenCaptureWin(){
            // int width = 128;
            // int height = 128;
            // using(FileStream pngStream = new FileStream("hah",FileMode.Open, FileAccess.Read));
            // using(var image = new Bitmap(pngStream))
            // {
            //     var resized = new Bitmap(width, height);
            //     using (var graphics = Graphics.FromImage(resized))
            //     {
            //         graphics.CompositingQuality = CompositingQuality.HighSpeed;
            //         graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //         graphics.CompositingMode = CompositingMode.SourceCopy;
            //         graphics.DrawImage(image, 0, 0, width, height);
            //         resized.Save($"resized-{file}", ImageFormat.Png);
            //         Console.WriteLine($"Saving resized-{file} thumbnail");
            //     }       
            // }     
        }

        public void ScreenCaptureLinux(){
        }
    }
}