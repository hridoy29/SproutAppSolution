using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;

namespace Cs2Gen.BusinessObjects
{
    public class Utilities
    {
        public static byte[] ConvertImageTobyte(string path)
        {
            byte[] result = null;
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                result = new byte[fileStream.Length];
                fileStream.Read(result, 0, (int)fileStream.Length);
            }

            return result;
        }
        public static Bitmap ConvertbyteToImage(byte[] imageBuffer)
        {
            Bitmap result = null;
            using (MemoryStream memoryStream = new MemoryStream(imageBuffer, true))
            {
                memoryStream.Write(imageBuffer, 0, imageBuffer.Length);
                result = new Bitmap(memoryStream);
            }
            return result;
        } 
    }
}
