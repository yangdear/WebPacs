using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebViewer.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace WebViewer.Controllers
{
    [Authorize]
    public class PacsReportsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        // GET: PacsReports
        public ActionResult Index()
        {
            return View(db.PacsReports.ToList());
        }

        public ActionResult List(int id)
        {
            ViewBag.PatientId = id;
            ViewBag.Patient = db.Patients.Find(id);
            return View(db.PacsReports.Where(f => f.PatientId == id).ToList());
        }

        // GET: PacsReports/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PacsReport pacsReport = db.PacsReports.Find(id);
            if (pacsReport == null)
            {
                return HttpNotFound();
            }
            return View(pacsReport);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">患者编号</param>
        /// <returns></returns>
        // GET: PacsReports/Create
        public ActionResult Create(int id)
        {
            ViewBag.PatientId = id;
            return View();
        }

        // POST: PacsReports/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int id, [Bind(Include = "PacsReportId,PacsCode,CheckDate,DicomFileName,PreDiagnose,CheckPoint,ImageDesc,ImagingDiagnosis,ReportDate")] PacsReport pacsReport)
        {
            if (ModelState.IsValid)
            {
                var currentUser = db.Users.Find(User.Identity.GetUserId());
                pacsReport.Uploader = currentUser;
                pacsReport.ReportState = ReportStateEnum.rsUploadDicom;
                pacsReport.DicomFileName = "";
                pacsReport.PatientId = id;
                db.PacsReports.Add(pacsReport);
                db.SaveChanges();

                pacsReport.DicomFileName= SaveDicomFile(id);
                db.Entry(pacsReport).State = EntityState.Modified;

                db.SaveChanges();

                return RedirectToAction("List", new { id = id });
            }

            return View(pacsReport);
        }

        [HttpPost]
        public string UploadFile()
        {
            return SaveDicomFile(1);
        }
        private string SaveDicomFile(int Patientid)
        {
            string path = "~/upload/" + DateTime.Now.ToString("yyyyMMdd") + "/patient" + Patientid.ToString();
            if (!Directory.Exists(Server.MapPath(path)))
                Directory.CreateDirectory(Server.MapPath(path));
            path += "/dicom" + Directory.GetDirectories(Server.MapPath(path)).Length.ToString();
            if (!Directory.Exists(Server.MapPath(path)))
                Directory.CreateDirectory(Server.MapPath(path));

            if (this.Request.Files.Count > 0)
            {
                for (int i = 0; i < this.Request.Files.Count; i++)
                {
                    var file = this.Request.Files.Get(i);
                    string ext = (new FileInfo(file.FileName)).Extension;
                    var filename = path + "/dicom" + i.ToString()  + ext;
                    file.SaveAs(Server.MapPath(filename));
                }
            }
            else
                return "";
            return path;
        }

        // GET: PacsReports/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PacsReport pacsReport = db.PacsReports.Find(id);
            if (pacsReport == null)
            {
                return HttpNotFound();
            }
            return View(pacsReport);
        }

        // POST: PacsReports/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PacsReportId,PacsCode,CheckDate,DicomFileName,PreDiagnose,CheckPoint,ImageDesc,ImagingDiagnosis,ReportDate")] PacsReport pacsReport)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pacsReport).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("List", new { id = pacsReport.PatientId });
            }
            return View(pacsReport);
        }

        // GET: PacsReports/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PacsReport pacsReport = db.PacsReports.Find(id);
            if (pacsReport == null)
            {
                return HttpNotFound();
            }
            return View(pacsReport);
        }

        // POST: PacsReports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PacsReport pacsReport = db.PacsReports.Find(id);
            db.PacsReports.Remove(pacsReport);
            int patientId = pacsReport.PatientId.Value;
            db.SaveChanges();
            return RedirectToAction("List", new { id = patientId });
        }

        public ActionResult DicomView(int id)
        {
            PacsReport pacsReport = db.PacsReports.Find(id);

            if (pacsReport == null)
            {
                return HttpNotFound();
            }
            ViewBag.pacsReport = pacsReport;
            DicomParseHelper helper = new DicomParseHelper(pacsReport.DicomFileName);

            ViewData.Model = new DicomViewData()
            {
                DataItemList = helper.GetDataList(),
                ImageItemList = helper.ImageList
            };

            return View();
        }

        public ActionResult FillReport(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PacsReport pacsReport = db.PacsReports.Find(id);
            if (pacsReport == null)
            {
                return HttpNotFound();
            }
            FillReportViewModel report = new FillReportViewModel();
            report.ImageDesc = pacsReport.ImageDesc;
            report.ImagingDiagnosis = pacsReport.ImagingDiagnosis;
            return View(report);
        }
        [HttpPost]
        public string FillReport(int id, FillReportViewModel fillreport)
        {
            if (ModelState.IsValid)
            {
                PacsReport report = db.PacsReports.Find(id);
                var currentUser = db.Users.Find(User.Identity.GetUserId());
                report.ReportUser = currentUser;
                report.ReportDate = DateTime.Now;
                report.ReportState = ReportStateEnum.rsFilledReport;
                //db.Entry(currentUser).State = EntityState.Unchanged;
                db.Entry(report).State = EntityState.Modified;
                db.SaveChanges();
                return "保存成功";
            }
            return "保存失败";
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
