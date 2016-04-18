using Dicom;
using Dicom.Imaging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebViewer.Models;

namespace WebViewer.Controllers
{
   
    public class DicomViewerController : Controller
    {
        // GET: DicomViewer
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Upload()
        {
            return View();
        }
        
      

        [HttpPost]
        public ActionResult Upload(FormCollection form)
        {
            if (Request.Files.Count == 0)
            {
                //Request.Files.Count 文件数为0上传不成功
                return View();
            }

            var file = Request.Files[0];
            if (file.ContentLength == 0)
            {
                //文件大小大（以字节为单位）为0时，做一些操作
                return View();
            }
            else
            {
                string path = Server.MapPath(@"~\\UploadFile\\newfile");
                //保存成自己的文件全路径,newfile就是你上传后保存的文件,
                //服务器上的UpLoadFile文件夹必须有读写权限　　　
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                FileInfo fi = new FileInfo(file.FileName);
                file.SaveAs(path +"\\"+ fi.Name);
                return RedirectToAction("Viewer", new { path = fi.Name });
            }

        }
 
        public ActionResult Viewer(string path)
        {
            DicomParseHelper helper = new DicomParseHelper(path);

            ViewData.Model = new DicomViewData() { DataItemList= helper.GetDataList(),
                ImageItemList = helper.GetImageList() };
            return View();
        }

      
    }
}