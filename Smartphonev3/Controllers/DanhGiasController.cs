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
    public class DanhGiasController : Controller
    {
        private SmartphoneEntitiesv3_1 db = new SmartphoneEntitiesv3_1();

        // GET: DanhGias
        public ActionResult Index()
        {
            var danhGias = db.DanhGias.Include(d => d.NguoiDung).Include(d => d.SanPham);
            return View(danhGias.ToList());
        }

        // GET: DanhGias/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DanhGia danhGia = db.DanhGias.Find(id);
            if (danhGia == null)
            {
                return HttpNotFound();
            }
            return View(danhGia);
        }

        // GET: DanhGias/Create
        public ActionResult Create()
        {
            ViewBag.khach_hang_id = new SelectList(db.NguoiDungs, "nguoi_dung_id", "ten_dang_nhap");
            ViewBag.san_pham_id = new SelectList(db.SanPhams, "san_pham_id", "ten_san_pham");
            return View();
        }

        // POST: DanhGias/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "danh_gia_id,san_pham_id,khach_hang_id,so_sao,binh_luan,ngay_tao_danh_gia")] DanhGia danhGia)
        {
            if (ModelState.IsValid)
            {
                db.DanhGias.Add(danhGia);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.khach_hang_id = new SelectList(db.NguoiDungs, "nguoi_dung_id", "ten_dang_nhap", danhGia.khach_hang_id);
            ViewBag.san_pham_id = new SelectList(db.SanPhams, "san_pham_id", "ten_san_pham", danhGia.san_pham_id);
            return View(danhGia);
        }

        // GET: DanhGias/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DanhGia danhGia = db.DanhGias.Find(id);
            if (danhGia == null)
            {
                return HttpNotFound();
            }
            ViewBag.khach_hang_id = new SelectList(db.NguoiDungs, "nguoi_dung_id", "ten_dang_nhap", danhGia.khach_hang_id);
            ViewBag.san_pham_id = new SelectList(db.SanPhams, "san_pham_id", "ten_san_pham", danhGia.san_pham_id);
            return View(danhGia);
        }

        // POST: DanhGias/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "danh_gia_id,san_pham_id,khach_hang_id,so_sao,binh_luan,ngay_tao_danh_gia")] DanhGia danhGia)
        {
            if (ModelState.IsValid)
            {
                db.Entry(danhGia).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.khach_hang_id = new SelectList(db.NguoiDungs, "nguoi_dung_id", "ten_dang_nhap", danhGia.khach_hang_id);
            ViewBag.san_pham_id = new SelectList(db.SanPhams, "san_pham_id", "ten_san_pham", danhGia.san_pham_id);
            return View(danhGia);
        }

        // GET: DanhGias/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DanhGia danhGia = db.DanhGias.Find(id);
            if (danhGia == null)
            {
                return HttpNotFound();
            }
            return View(danhGia);
        }

        // POST: DanhGias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DanhGia danhGia = db.DanhGias.Find(id);
            db.DanhGias.Remove(danhGia);
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
