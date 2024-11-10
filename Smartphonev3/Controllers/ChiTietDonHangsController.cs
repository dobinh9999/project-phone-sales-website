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
    public class ChiTietDonHangsController : Controller
    {
        private SmartphoneEntitiesv3_1 db = new SmartphoneEntitiesv3_1();

        // GET: ChiTietDonHangs
        public ActionResult Index()
        {
            var chiTietDonHangs = db.ChiTietDonHangs.Include(c => c.DonHang).Include(c => c.SanPham);
            return View(chiTietDonHangs.ToList());
        }

        // GET: ChiTietDonHangs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChiTietDonHang chiTietDonHang = db.ChiTietDonHangs.Find(id);
            if (chiTietDonHang == null)
            {
                return HttpNotFound();
            }
            return View(chiTietDonHang);
        }

        // GET: ChiTietDonHangs/Create
        public ActionResult Create()
        {
            ViewBag.don_hang_id = new SelectList(db.DonHangs, "don_hang_id", "trang_thai_don_hang");
            ViewBag.san_pham_id = new SelectList(db.SanPhams, "san_pham_id", "ten_san_pham");
            return View();
        }

        // POST: ChiTietDonHangs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "chi_tiet_don_hang_id,don_hang_id,san_pham_id,so_luong,gia")] ChiTietDonHang chiTietDonHang)
        {
            if (ModelState.IsValid)
            {
                db.ChiTietDonHangs.Add(chiTietDonHang);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.don_hang_id = new SelectList(db.DonHangs, "don_hang_id", "trang_thai_don_hang", chiTietDonHang.don_hang_id);
            ViewBag.san_pham_id = new SelectList(db.SanPhams, "san_pham_id", "ten_san_pham", chiTietDonHang.san_pham_id);
            return View(chiTietDonHang);
        }

        // GET: ChiTietDonHangs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChiTietDonHang chiTietDonHang = db.ChiTietDonHangs.Find(id);
            if (chiTietDonHang == null)
            {
                return HttpNotFound();
            }
            ViewBag.don_hang_id = new SelectList(db.DonHangs, "don_hang_id", "trang_thai_don_hang", chiTietDonHang.don_hang_id);
            ViewBag.san_pham_id = new SelectList(db.SanPhams, "san_pham_id", "ten_san_pham", chiTietDonHang.san_pham_id);
            return View(chiTietDonHang);
        }

        // POST: ChiTietDonHangs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "chi_tiet_don_hang_id,don_hang_id,san_pham_id,so_luong,gia")] ChiTietDonHang chiTietDonHang)
        {
            if (ModelState.IsValid)
            {
                db.Entry(chiTietDonHang).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.don_hang_id = new SelectList(db.DonHangs, "don_hang_id", "trang_thai_don_hang", chiTietDonHang.don_hang_id);
            ViewBag.san_pham_id = new SelectList(db.SanPhams, "san_pham_id", "ten_san_pham", chiTietDonHang.san_pham_id);
            return View(chiTietDonHang);
        }

        // GET: ChiTietDonHangs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChiTietDonHang chiTietDonHang = db.ChiTietDonHangs.Find(id);
            if (chiTietDonHang == null)
            {
                return HttpNotFound();
            }
            return View(chiTietDonHang);
        }

        // POST: ChiTietDonHangs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ChiTietDonHang chiTietDonHang = db.ChiTietDonHangs.Find(id);
            db.ChiTietDonHangs.Remove(chiTietDonHang);
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
