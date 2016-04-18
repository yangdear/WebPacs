using Dicom;
using Dicom.Imaging;
using Dicom.IO.Buffer;
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
        private DicomFile _file; 
        public DicomParseHelper(string filepath)
        {
            string path = HttpContext.Current.Server.MapPath(@"~\\UploadFile\\newfile");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            ImageVPath = "\\UploadFile\\newfile\\" + filepath.Replace(".", "");
            ImagePPath = HttpContext.Current.Server.MapPath(ImageVPath);
            if (!Directory.Exists(ImagePPath))
                Directory.CreateDirectory(ImagePPath);
            DicomFilePath = path+"\\"+ filepath;
            _file = DicomFile.Open(DicomFilePath);

        }
        private class DumpWalker : IDicomDatasetWalker
        {
            private int _level = 0;

            public List<DicomDataItem> List { get; set; }
            public DumpWalker(List<DicomDataItem> list)
            {
                List = list;
                Level = 0;
            }


            public int Level
            {
                get
                {
                    return _level;
                }
                set
                {
                    _level = value;
                    Indent = String.Empty;
                    for (int i = 0; i < _level; i++) Indent += "    ";
                }
            }

            private string Indent { get; set; }

            public void OnBeginWalk()
            {
            }

            public bool OnElement(DicomElement element)
            {
                var tag = String.Format(
                    "{0}{1}  {2}",
                    Indent,
                    element.Tag.ToString().ToUpper(),
                    element.Tag.DictionaryEntry.Name);

                string value = "<large value not displayed>";
                if (element.Length <= 2048) value = String.Join("\\", element.Get<string[]>());

                if (element.ValueRepresentation == DicomVR.UI && element.Count > 0)
                {
                    var uid = element.Get<DicomUID>(0);
                    var name = uid.Name;
                    if (name != "Unknown") value = String.Format("{0} ({1})", value, name);
                }
                List.Add(new DicomDataItem()
                {
                    Tag = tag,
                    Vr = element.ValueRepresentation.Code,
                    Length = element.Length.ToString(),
                    Value = value
                });
                return true;
            }

            public Task<bool> OnElementAsync(DicomElement element)
            {
                return Task.FromResult(this.OnElement(element));
            }

            public bool OnBeginSequence(DicomSequence sequence)
            {
                var tag = String.Format(
                    "{0}{1}  {2}",
                    Indent,
                    sequence.Tag.ToString().ToUpper(),
                    sequence.Tag.DictionaryEntry.Name);

                List.Add(new DicomDataItem()
                {
                    Tag = tag,
                    Vr = "SQ",
                    Length = String.Empty,
                    Value = String.Empty
                });

                Level++;
                return true;
            }

            public bool OnBeginSequenceItem(DicomDataset dataset)
            {
                var tag = String.Format("{0}Sequence Item:", Indent);

                List.Add(new DicomDataItem()
                {
                    Tag = tag,
                    Vr = String.Empty,
                    Length = String.Empty,
                    Value = String.Empty
                });

                Level++;
                return true;
            }

            public bool OnEndSequenceItem()
            {
                Level--;
                return true;
            }

            public bool OnEndSequence()
            {
                Level--;
                return true;
            }

            public bool OnBeginFragment(DicomFragmentSequence fragment)
            {
                var tag = String.Format(
                    "{0}{1}  {2}",
                    Indent,
                    fragment.Tag.ToString().ToUpper(),
                    fragment.Tag.DictionaryEntry.Name);

                List.Add(new DicomDataItem()
                {
                    Tag = tag,
                    Vr = fragment.ValueRepresentation.Code,
                    Length = String.Empty,
                    Value = String.Empty
                });

                Level++;
                return true;
            }

            public bool OnFragmentItem(IByteBuffer item)
            {
                var tag = String.Format("{0}Fragment", Indent);
                List.Add(new DicomDataItem()
                {
                    Tag = tag,
                    Vr = String.Empty,
                    Length = item.Size.ToString(),
                    Value = String.Empty
                });

                return true;
            }

            public Task<bool> OnFragmentItemAsync(IByteBuffer item)
            {
                return Task.FromResult(this.OnFragmentItem(item));
            }

            public bool OnEndFragment()
            {
                Level--;
                return true;
            }

            public void OnEndWalk()
            {
            }


        }
        public List<DicomDataItem> GetDataList()
        {
            List<DicomDataItem> list = new List<DicomDataItem>();
            
            new DicomDatasetWalker(_file.FileMetaInfo).Walk(new DumpWalker(list));
            new DicomDatasetWalker(_file.Dataset).Walk(new DumpWalker(list));
            return list;
        }
        public List<DicomImageItem> GetImageList()
        {

            List<DicomImageItem> list = new List<DicomImageItem>();
            DicomImage _image = new DicomImage(_file.Dataset);
            for (int i = 0; i < _image.NumberOfFrames; i++)
            {
                _image.RenderImage(i).As<Image>().Save(ImagePPath + "\\big" + i.ToString() + ".jpg");
                SaveThumbnail(i,_image.RenderImage(i).As<Image>(), 64, 64);
                list.Add(new DicomImageItem() {  BigImage = ImageVPath + "\\big" + i.ToString() + ".jpg" ,
                     SmallImage = ImageVPath + "\\small" + i.ToString() + ".jpg"
                });
            }
            return list;
        }
        private string SaveThumbnail(int i,Image src, int newWidth, int newHeight)
        {
            System.Drawing.Image thumbnail = new System.Drawing.Bitmap(newWidth, newHeight);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(thumbnail);
            g.DrawImage(src, 0, 0, newWidth, newHeight);
            thumbnail.Save(ImagePPath + "\\small" + i.ToString() + ".jpg");
            return ImagePPath + "\\small" + i.ToString() + ".jpg";
            //imageList1.Images.Add("", thumbnail);
        }


    }
}