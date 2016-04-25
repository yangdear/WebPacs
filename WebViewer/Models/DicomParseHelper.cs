

using DicomObjects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace WebViewer.Models
{
    public class DicomDataItem
    {
        public string Tag { get; set; }
        public string Vr { get; set; }
        public string Length { get; set; }
        public string Value { get; set; }
    }

    public class DicomImageItem
    {
        public string SmallImage { get; set; }
        public string BigImage { get; set; }
    }
    public class DicomViewData
    {
        public List<DicomDataItem> DataItemList { get; set; }
        public List<DicomImageItem> ImageItemList { get; set; }
    }
    public class DicomParseHelper
    {
        private string DicomFilePath;
        private string ImageVPath;
        private string ImagePPath;
        public  List<DicomImageItem> ImageList { get; set; }
        public DicomParseHelper(string filepath)
        {
            ImageList = new List<DicomImageItem>();
            //FileInfo fileInfo = new FileInfo(filepath);

            string path = HttpContext.Current.Server.MapPath(filepath);
          
            ImageVPath = filepath.Replace(".", "").Replace("~", "")+"/images";
            ImagePPath = HttpContext.Current.Server.MapPath(ImageVPath);
            if (!Directory.Exists(ImagePPath))
                Directory.CreateDirectory(ImagePPath);
            DicomFilePath = path;
           
            foreach(string file in Directory.GetFiles(path))
            {
                
                DicomImage dcm = new  DicomImage(file);

                for (int i = 0; i < dcm.FrameCount; i++)
                {
                    dcm.Frame = i;
                    Bitmap bmp = dcm.Bitmap();
                    bmp.Save(ImagePPath + "\\big" + (new FileInfo(file)).Name + i.ToString() + ".jpg");
                    SaveThumbnail((new FileInfo(file)).Name,i, bmp, 64, 64);
                    ImageList.Add(new DicomImageItem()
                    {
                        BigImage = ImageVPath + "/big" + (new FileInfo(file)).Name + i.ToString() + ".jpg",
                        SmallImage = ImageVPath + "/small" + (new FileInfo(file)).Name + i.ToString() + ".jpg"
                    });
                }
            }
        }

        internal List<DicomDataItem> GetDataList()
        {
            return new List<DicomDataItem>();
        }

        private string SaveThumbnail(string filename,int i, Image src, int newWidth, int newHeight)
        {
            System.Drawing.Image thumbnail = new System.Drawing.Bitmap(newWidth, newHeight);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(thumbnail);
            g.DrawImage(src, 0, 0, newWidth, newHeight);
            thumbnail.Save(ImagePPath + "\\small" + filename+ i.ToString() + ".jpg");
            return ImagePPath + "\\small" + filename + i.ToString() + ".jpg";
            //imageList1.Images.Add("", thumbnail);
        }


    }
}