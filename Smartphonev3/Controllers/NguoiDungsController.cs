using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Smartphonev3.Models;
using static System.Data.Entity.Infrastructure.Design.Executor;
using static System.Net.Mime.MediaTypeNames;

namespace Smartphonev3.Controllers
{
	public class NguoiDungsController : Controller
	{
		private SmartphoneEntitiesv3_1 db = new SmartphoneEntitiesv3_1();

		// GET: NguoiDungs
		public ActionResult Index()
		{
			// Lấy vai trò từ Session
			var vaiTro = Session["Role"]?.ToString();

			// Kiểm tra vai trò
			if (vaiTro == "Admin")
			{
				// Nếu là Admin, hiển thị danh sách tất cả người dùng
				var nguoiDungs = db.NguoiDungs.Include(n => n.ThongTinNguoiDung).ToList();
				return View(nguoiDungs);
			}
			else
			{
				// Nếu không phải Admin, lấy thông tin người dùng theo id từ Session
				var nguoiDungId = Session["NguoiDungId"]?.ToString();
				if (string.IsNullOrEmpty(nguoiDungId))
				{
					// Nếu không có id trong Session, chuyển hướng về trang đăng nhập
					return RedirectToAction("Login", "NguoiDungs");
				}

				// Tìm người dùng theo nguoi_dung_id
				var nguoiDung = db.NguoiDungs.Include(n => n.ThongTinNguoiDung)
											 .FirstOrDefault(n => n.nguoi_dung_id.ToString() == nguoiDungId);

				if (nguoiDung == null)
				{
					// Nếu không tìm thấy người dùng
					return HttpNotFound();
				}

				// Trả về view với người dùng tìm thấy
				return View(new List<NguoiDung> { nguoiDung });
			}
		}
		// GET: NguoiDungs/Details/5
		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			NguoiDung nguoiDung = db.NguoiDungs.Find(id);
			if (nguoiDung == null)
			{
				return HttpNotFound();
			}
			return View(nguoiDung);
		}

		// GET: NguoiDungs/Create
		public ActionResult Create()
		{
			ViewBag.nguoi_dung_id = new SelectList(db.ThongTinNguoiDungs, "nguoi_dung_id", "ten_day_du");
			return View();
		}

		// POST: NguoiDungs/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "nguoi_dung_id,ten_dang_nhap,mat_khau,email_sdt,vai_tro,ngay_tao,ngay_cap_nhat")] NguoiDung nguoiDung)
		{
			if (ModelState.IsValid)
			{
				// Kiểm tra xem tên đăng nhập hoặc email đã tồn tại chưa
				var existingUser = db.NguoiDungs.FirstOrDefault(u => u.ten_dang_nhap == nguoiDung.ten_dang_nhap || u.email_sdt == nguoiDung.email_sdt);
				if (existingUser != null)
				{
					ModelState.AddModelError("", "Tên đăng nhập hoặc email đã tồn tại. Vui lòng chọn tên khác.");
					return View(nguoiDung);
				}
				// Thêm người dùng vào bảng NguoiDung
				db.NguoiDungs.Add(nguoiDung);
				db.SaveChanges();

				// Sau khi lưu NguoiDung, lấy nguoi_dung_id mới thêm
				int newUserId = nguoiDung.nguoi_dung_id;

				// Tạo bản ghi mới trong bảng ThongTinNguoiDung
				ThongTinNguoiDung thongTinNguoiDung = new ThongTinNguoiDung
				{
					nguoi_dung_id = newUserId,
					ten_day_du = nguoiDung.ten_dang_nhap,
					dia_chi = null,
					so_dien_thoai = null,
					ngay_sinh = null
				};

				db.ThongTinNguoiDungs.Add(thongTinNguoiDung);
				db.SaveChanges();

				// Thông báo thành công
				TempData["SuccessMessage"] = "Tài khoản đã được đăng ký thành công!";
				return RedirectToAction("Index");
			}

			ViewBag.nguoi_dung_id = new SelectList(db.ThongTinNguoiDungs, "nguoi_dung_id", "ten_day_du", nguoiDung.nguoi_dung_id);
			return View(nguoiDung);
		}


		// GET: NguoiDungs/Edit/5
		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			NguoiDung nguoiDung = db.NguoiDungs.Find(id);
			if (nguoiDung == null)
			{
				return HttpNotFound();
			}
			ViewBag.nguoi_dung_id = new SelectList(db.ThongTinNguoiDungs, "nguoi_dung_id", "ten_day_du", nguoiDung.nguoi_dung_id);
			return View(nguoiDung);
		}

		// POST: NguoiDungs/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "nguoi_dung_id,ten_dang_nhap,mat_khau,email_sdt,vai_tro,ngay_tao,ngay_cap_nhat")] NguoiDung nguoiDung)
		{
			if (ModelState.IsValid)
			{
				db.Entry(nguoiDung).State = EntityState.Modified;
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			ViewBag.nguoi_dung_id = new SelectList(db.ThongTinNguoiDungs, "nguoi_dung_id", "ten_day_du", nguoiDung.nguoi_dung_id);
			return View(nguoiDung);
		}

		// GET: NguoiDungs/Delete/5
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			NguoiDung nguoiDung = db.NguoiDungs.Find(id);
			if (nguoiDung == null)
			{
				return HttpNotFound();
			}
			return View(nguoiDung);
		}

		// POST: NguoiDungs/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			NguoiDung nguoiDung = db.NguoiDungs.Find(id);
			db.NguoiDungs.Remove(nguoiDung);
			db.SaveChanges();
			return RedirectToAction("Index");
		}
		// Action để hiển thị trang đăng nhập
		public ActionResult Login()
		{
			return View();
		}

		// Action xử lý đăng nhập
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Login(string ten_dang_nhap, string mat_khau)
		{
			if (string.IsNullOrEmpty(ten_dang_nhap) || string.IsNullOrEmpty(mat_khau))
			{
				ViewBag.ErrorMessage = "Vui lòng nhập tên đăng nhập, email, số điện thoại và mật khẩu.";
				return View();
			}

			// Kiểm tra xem tên đăng nhập là email hoặc số điện thoại
			var nguoiDung = db.NguoiDungs.SingleOrDefault(n => (n.ten_dang_nhap == ten_dang_nhap || n.email_sdt == ten_dang_nhap) && n.mat_khau == mat_khau);

			if (nguoiDung != null)
			{
				// Nếu đăng nhập thành công, lưu thông tin vào session và phân quyền
				Session["NguoiDungId"] = nguoiDung.nguoi_dung_id;
				Session["Username"] = nguoiDung.ten_dang_nhap;
				Session["Role"] = nguoiDung.vai_tro;
				Session["email_sdt"] = nguoiDung.email_sdt;

				// Thêm thông báo thành công vào ViewBag
				ViewBag.SuccessMessage = "Đăng nhập thành công!";

				// Chuyển hướng đến trang quản trị hoặc trang người dùng
				if (nguoiDung.vai_tro == "Admin")
				{
					return RedirectToAction("Index", "NguoiDungs"); // Chuyển tới trang quản trị
				}
				else
				{
					return RedirectToAction("Index", "SanPhams"); // Chuyển tới trang người dùng
				}
			}
			else
			{
				ViewBag.ErrorMessage = "Tên đăng nhập, email hoặc số điện thoại, hoặc mật khẩu không đúng.";
				return View();
			}
		}



		// Đăng xuất
		public ActionResult Logout()
		{
			Session.Clear();
			return RedirectToAction("Login");
		}


		public ActionResult ForgotPassword()
		{
			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult ForgotPassword(string email_sdt)
		{
			if (string.IsNullOrEmpty(email_sdt))
			{
				ViewBag.ErrorMessage = "Vui lòng nhập email hoặc số điện thoại.";
				return View();
			}

			// Tìm người dùng theo email hoặc số điện thoại
			var nguoiDung = db.NguoiDungs.SingleOrDefault(n => n.email_sdt == email_sdt);

			if (nguoiDung != null)
			{
				// Nếu tìm thấy người dùng, hiển thị mật khẩu
				ViewBag.Message = "Mật khẩu của bạn là: " + nguoiDung.mat_khau;
			}
			else
			{
				// Nếu không tìm thấy người dùng, hiển thị thông báo lỗi
				ViewBag.ErrorMessage = "Không có tài khoản với email hoặc số điện thoại này.";
			}
			return View();
		}



	}
}

