using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Smartphonev3.Models;

namespace Smartphonev3.Controllers
{
    public class ThongBaosController : Controller
    {
        private SmartphoneEntitiesv3_1 db = new SmartphoneEntitiesv3_1();

        // GET: ThongBaos
        public ActionResult Index()
        {
            var thongBaos = db.ThongBaos.Include(t => t.NguoiDung);
            return View(thongBaos.ToList());
        }

        // GET: ThongBaos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ThongBao thongBao = db.ThongBaos.Find(id);
            if (thongBao == null)
            {
                return HttpNotFound();
            }
            return View(thongBao);
        }

        // GET: ThongBaos/Create
        public ActionResult Create()
        {
            ViewBag.nguoi_dung_id = new SelectList(db.NguoiDungs, "nguoi_dung_id", "ten_dang_nhap");
            return View();
        }

        // POST: ThongBaos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "thong_bao_id,nguoi_dung_id,noi_dung,ngay_gui,trang_thai")] ThongBao thongBao)
        {
            if (ModelState.IsValid)
            {
                db.ThongBaos.Add(thongBao);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.nguoi_dung_id = new SelectList(db.NguoiDungs, "nguoi_dung_id", "ten_dang_nhap", thongBao.nguoi_dung_id);
            return View(thongBao);
        }

        // GET: ThongBaos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ThongBao thongBao = db.ThongBaos.Find(id);
            if (thongBao == null)
            {
                return HttpNotFound();
            }
            ViewBag.nguoi_dung_id = new SelectList(db.NguoiDungs, "nguoi_dung_id", "ten_dang_nhap", thongBao.nguoi_dung_id);
            return View(thongBao);
        }

        // POST: ThongBaos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "thong_bao_id,nguoi_dung_id,noi_dung,ngay_gui,trang_thai")] ThongBao thongBao)
        {
            if (ModelState.IsValid)
            {
                db.Entry(thongBao).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.nguoi_dung_id = new SelectList(db.NguoiDungs, "nguoi_dung_id", "ten_dang_nhap", thongBao.nguoi_dung_id);
            return View(thongBao);
        }

        // GET: ThongBaos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ThongBao thongBao = db.ThongBaos.Find(id);
            if (thongBao == null)
            {
                return HttpNotFound();
            }
            return View(thongBao);
        }

        // POST: ThongBaos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ThongBao thongBao = db.ThongBaos.Find(id);
            db.ThongBaos.Remove(thongBao);
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
