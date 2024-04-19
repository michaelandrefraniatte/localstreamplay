using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using HelperClass;
using System.Xml.Linq;
namespace BitmapArraysToBitmapTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Bitmap img = new Bitmap(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height);
            Graphics graphics = Graphics.FromImage(img as System.Drawing.Image);
            graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
            graphics.SmoothingMode = SmoothingMode.HighSpeed;
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
            graphics.CompositingMode = CompositingMode.SourceCopy;
            graphics.CompositingQuality = CompositingQuality.HighSpeed;
            graphics.CopyFromScreen(0, 0, 0, 0, img.Size);
            ImageCompress imgCompress = ImageCompress.GetImageCompressObject;
            imgCompress.GetImage = img;
            imgCompress.Width = img.Width;
            imgCompress.Height = img.Height;
            img = imgCompress.Save();
            pictureBox1.Image = img;
            byte[] bmpdata = BitmapToByteArray(img);
            MessageBox.Show((bmpdata.Length / img.Height / 4).ToString());
            Bitmap bmp = ByteArrayToBitmap(bmpdata, img.Width, img.Height, PixelFormat.Format32bppRgb);
            pictureBox2.Image = bmp;
        }
        public byte[] BitmapToByteArray(Bitmap img)
        {
            BitmapData bmpData = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, img.PixelFormat);
            IntPtr ptr = bmpData.Scan0;
            Int32 psize = bmpData.Stride * bmpData.Height;
            byte[] byOut = new byte[psize]; 
            System.Runtime.InteropServices.Marshal.Copy(ptr, byOut, 0, psize);
            img.UnlockBits(bmpData);
            return byOut;
        }
        public Bitmap ByteArrayToBitmap(byte[] byteIn, int imwidth, int imheight, PixelFormat pxformat)
        {
            Bitmap picOut = new Bitmap(imwidth, imheight, pxformat);
            BitmapData bmpData = picOut.LockBits(new Rectangle(0, 0, imwidth, imheight), ImageLockMode.WriteOnly, pxformat);
            IntPtr ptr = bmpData.Scan0;
            Int32 psize = bmpData.Stride * imheight;
            System.Runtime.InteropServices.Marshal.Copy(byteIn, 0, ptr, psize);
            picOut.UnlockBits(bmpData);
            return picOut;
        }
    }
}
namespace HelperClass
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>  
    /// This class is used to get the constants  
    /// </summary>  
    public class CommonConstant
    {
        public const string JPEG = ".jpeg";
        public const string PNG = ".png";
        public const string JPG = ".jpg";
        public const string BTM = ".btm";
    }
}
namespace HelperClass
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;

    /// <summary>  
    /// This class is used to compress the image to  
    /// provided size  
    /// </summary>  
    public class ImageCompress
    {
        private static volatile ImageCompress imageCompress;
        private Bitmap bitmap;
        private int width;
        private int height;
        private Image img;
        /// <summary>  
        /// It is used to restrict to create the instance of the ImageCompress  
        /// </summary>  
        private ImageCompress()
        {
        }
        /// <summary>  
        /// Gets ImageCompress object  
        /// </summary>  
        public static ImageCompress GetImageCompressObject
        {
            get
            {
                if (imageCompress == null)
                {
                    imageCompress = new ImageCompress();
                }
                return imageCompress;
            }
        }

        /// <summary>  
        /// Gets or sets Width  
        /// </summary>  
        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        /// <summary>  
        /// Gets or sets Width  
        /// </summary>  
        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        /// <summary>  
        /// Gets or sets Image  
        /// </summary>  
        public Bitmap GetImage
        {
            get { return bitmap; }
            set { bitmap = value; }
        }
        /// <summary>  
        /// This function is used to save the image  
        /// </summary>  
        /// <param name="fileName"></param>  
        /// <param name="path"></param>  
        public Bitmap Save()
        {
            img = CompressImage();
            return (Bitmap)img;
        }
        /// <summary>  
        /// This function is use to compress the image to  
        /// predefine size  
        /// </summary>  
        /// <returns>return bitmap in compress size</returns>  
        private Image CompressImage()
        {
            if (GetImage != null)
            {
                Width = (Width == 0) ? GetImage.Width : Width;
                Height = (Height == 0) ? GetImage.Height : Height;
                Bitmap newBitmap = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
                newBitmap = bitmap;
                newBitmap.SetResolution(Width, Height);
                return newBitmap.GetThumbnailImage(Width, Height, null, IntPtr.Zero);
            }
            else
            {
                throw new Exception("Please provide bitmap");
            }
        }

        /// <summary>  
        /// This function is used to check the file Type  
        /// </summary>  
        /// <param name="fileName">String data type:contain the file name</param>  
        /// <returns>true or false on the file extention</returns>  
        private bool ISValidFileType(string fileName)
        {
            bool isValidExt = false;
            string fileExt = Path.GetExtension(fileName);
            switch (fileExt.ToLower())
            {
                case CommonConstant.JPEG:
                case CommonConstant.BTM:
                case CommonConstant.JPG:
                case CommonConstant.PNG:
                    isValidExt = true;
                    break;
            }
            return isValidExt;
        }

        /// <summary>  
        /// This function is used to get the imageCode info  
        /// on the basis of mimeType  
        /// </summary>  
        /// <param name="mimeType">string data type</param>  
        /// <returns>ImageCodecInfo data type</returns>  
        private ImageCodecInfo GetImageCoeInfo(string mimeType)
        {
            ImageCodecInfo[] codes = ImageCodecInfo.GetImageEncoders();
            for (int i = 0; i < codes.Length; i++)
            {
                if (codes[i].MimeType == mimeType)
                {
                    return codes[i];
                }
            }
            return null;
        }
    }
}