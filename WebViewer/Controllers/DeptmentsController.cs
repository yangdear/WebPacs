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
    public class DeptmentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Deptments
        public ActionResult Index()
        {
            return View(db.Deptments.ToList());
        }

        // GET: Deptments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Deptment deptment = db.Deptments.Find(id);
            if (deptment == null)
            {
                return HttpNotFound();
            }
            return View(deptment);
        }

        // GET: Deptments/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Deptments/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DeptmentId,DeptmentName")] Deptment deptment)
        {
            if (ModelState.IsValid)
            {
                db.Deptments.Add(deptment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(deptment);
        }

        // GET: Deptments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Deptment deptment = db.Deptments.Find(id);
            if (deptment == null)
            {
                return HttpNotFound();
            }
            return View(deptment);
        }

        // POST: Deptments/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DeptmentId,DeptmentName")] Deptment deptment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(deptment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(deptment);
        }

        // GET: Deptments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Deptment deptment = db.Deptments.Find(id);
            if (deptment == null)
            {
                return HttpNotFound();
            }
            return View(deptment);
        }

        // POST: Deptments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Deptment deptment = db.Deptments.Find(id);
            db.Deptments.Remove(deptment);
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
