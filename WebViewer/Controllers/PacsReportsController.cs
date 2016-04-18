using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebViewer.Models;

namespace WebViewer.Controllers
{
    public class PacsReportsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: PacsReports
        public ActionResult Index(int id)
        {
            return View(db.PacsReports.Where(f=>f.patient.PatientId==id).ToList());
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

        // GET: PacsReports/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PacsReports/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PacsReportId,PacsCode,CheckDate,DicomFileName,PreDiagnose,CheckPoint,ImageDesc,ImagingDiagnosis,ReportDate")] PacsReport pacsReport)
        {
            if (ModelState.IsValid)
            {
                db.PacsReports.Add(pacsReport);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(pacsReport);
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
                return RedirectToAction("Index");
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
            db.SaveChanges();
            return RedirectToAction("Index");
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
