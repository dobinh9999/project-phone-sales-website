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
    public class ThongTinNguoiDungsController : Controller
    {
        private SmartphoneEntitiesv3_1 db = new SmartphoneEntitiesv3_1();

        // GET: ThongTinNguoiDungs
        public ActionResult Index()
        {
			var nguoiDungId = Session["NguoiDungId"];
			var vaiTro = Session["Role"]?.ToString();

			if (vaiTro == "admin")
			{
				// Admin: Hiển thị tất cả người dùng
				var allUsers = db.ThongTinNguoiDungs.ToList();
				return View(allUsers);
			}
			else if (nguoiDungId != null)
			{
				// Người dùng thông thường: Chỉ hiển thị thông tin của chính họ
				var user = db.ThongTinNguoiDungs.Where(x => x.nguoi_dung_id == (int)nguoiDungId).ToList();
				return View(user);
			}

			// Nếu không có người dùng đăng nhập, chuyển về trang đăng nhập
			return RedirectToAction("Login", "NguoiDungs");
		}
		public ActionResult Index1()
		{
			// Giả sử bạn có một DbContext tên là _context và model là ThongTinNguoiDung
			var users = db.ThongTinNguoiDungs.ToList();

			return View(users);  // Trả về view với danh sách tất cả người dùng
		}

		// GET: ThongTinNguoiDungs/Details/5
		public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ThongTinNguoiDung thongTinNguoiDung = db.ThongTinNguoiDungs.Find(id);
            if (thongTinNguoiDung == null)
            {
                return HttpNotFound();
            }
            return View(thongTinNguoiDung);
        }

        // GET: ThongTinNguoiDungs/Create
        public ActionResult Create()
        {
            ViewBag.nguoi_dung_id = new SelectList(db.NguoiDungs, "nguoi_dung_id", "ten_dang_nhap");
            return View();
        }

        // POST: ThongTinNguoiDungs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "nguoi_dung_id,ten_day_du,dia_chi,so_dien_thoai,ngay_sinh")] ThongTinNguoiDung thongTinNguoiDung)
        {
            if (ModelState.IsValid)
            {
                db.ThongTinNguoiDungs.Add(thongTinNguoiDung);
                db.SaveChanges();
                return RedirectToAction("Index1");
            }

            ViewBag.nguoi_dung_id = new SelectList(db.NguoiDungs, "nguoi_dung_id", "ten_dang_nhap", thongTinNguoiDung.nguoi_dung_id);
            return View(thongTinNguoiDung);
        }

        // GET: ThongTinNguoiDungs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ThongTinNguoiDung thongTinNguoiDung = db.ThongTinNguoiDungs.Find(id);
            if (thongTinNguoiDung == null)
            {
                return HttpNotFound();
            }
            ViewBag.nguoi_dung_id = new SelectList(db.NguoiDungs, "nguoi_dung_id", "ten_dang_nhap", thongTinNguoiDung.nguoi_dung_id);
            return View(thongTinNguoiDung);
        }

        // POST: ThongTinNguoiDungs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "nguoi_dung_id,ten_day_du,dia_chi,so_dien_thoai,ngay_sinh")] ThongTinNguoiDung thongTinNguoiDung)
        {
            if (ModelState.IsValid)
            {
                db.Entry(thongTinNguoiDung).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.nguoi_dung_id = new SelectList(db.NguoiDungs, "nguoi_dung_id", "ten_dang_nhap", thongTinNguoiDung.nguoi_dung_id);
            return View(thongTinNguoiDung);
        }

        // GET: ThongTinNguoiDungs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ThongTinNguoiDung thongTinNguoiDung = db.ThongTinNguoiDungs.Find(id);
            if (thongTinNguoiDung == null)
            {
                return HttpNotFound();
            }
            return View(thongTinNguoiDung);
        }

        // POST: ThongTinNguoiDungs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ThongTinNguoiDung thongTinNguoiDung = db.ThongTinNguoiDungs.Find(id);
            db.ThongTinNguoiDungs.Remove(thongTinNguoiDung);
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
