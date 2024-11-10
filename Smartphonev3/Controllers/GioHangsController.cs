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
    public class GioHangsController : Controller
    {
        private SmartphoneEntitiesv3_1 db = new SmartphoneEntitiesv3_1();

		public ActionResult GetCartItemCount()
		{
			if (Session["NguoiDungId"] != null)
			{
				int nguoiDungId = (int)Session["NguoiDungId"];
				int soLuongSanPham = db.GioHangs.Where(g => g.khach_hang_id == nguoiDungId).Sum(g => g.so_luong_san_pham);
				return Json(soLuongSanPham, JsonRequestBehavior.AllowGet);
			}
			return Json(0, JsonRequestBehavior.AllowGet); // Nếu chưa đăng nhập, trả về 0
		}
		// GET: GioHangs
		public ActionResult Index()
		{

			// Lấy thông tin người dùng từ session
			var khach_hang_id = Session["NguoiDungId"];
			var vai_tro = Session["Role"]; // Lấy vai trò của người dùng từ session (Admin hoặc KhachHang)

			// Nếu không có người dùng đăng nhập, trả về yêu cầu đăng nhập
			if (khach_hang_id == null)
			{
				TempData["Message"] = "Bạn cần đăng nhập để xem giỏ hàng.";
				return RedirectToAction("Login", "NguoiDungs");
			}

			IQueryable<GioHang> gioHangs;

			// Kiểm tra vai trò người dùng
			if (vai_tro != null && vai_tro.ToString() == "Admin")
			{
				// Nếu là Admin, hiển thị tất cả giỏ hàng
				gioHangs = db.GioHangs.Include(g => g.NguoiDung).Include(g => g.SanPham);
			}
			else
			{
				// Nếu là người dùng thông thường, chỉ hiển thị giỏ hàng của chính họ
				int nguoiDungId = (int)khach_hang_id;
				gioHangs = db.GioHangs.Where(g => g.khach_hang_id == nguoiDungId)
									  .Include(g => g.NguoiDung)
									  .Include(g => g.SanPham);
			}
			// Đếm số lượng sản phẩm trong giỏ hàng của người dùng
			int soLuongSanPham = gioHangs.Sum(g => g.so_luong_san_pham);

			// Truyền số lượng sản phẩm vào ViewBag
			ViewBag.SoLuongSanPham = soLuongSanPham;
			return View(gioHangs.ToList());
		}
		
		// POST: GioHangs
		// POST: GioHangs
		[HttpPost]
		public ActionResult Index(int[] selectedItems)
		{

			// Kiểm tra xem có sản phẩm nào được chọn không
			if (selectedItems == null || selectedItems.Length == 0)
			{
				TempData["Message"] = "Bạn chưa chọn sản phẩm nào để mua.";
				return RedirectToAction("Index");
			}

			// Lấy thông tin khách hàng từ session
			var khach_hang_id = Session["NguoiDungId"];
			if (khach_hang_id == null)
			{
				TempData["Message"] = "Bạn cần đăng nhập để mua hàng.";
				return RedirectToAction("Login", "NguoiDungs");
			}

			// Tạo đơn hàng mới
			DonHang donHang = new DonHang
			{
				khach_hang_id = (int)khach_hang_id,
				trang_thai_don_hang = "Preparing",  // Tạm thời cho là đã thanh toán
				ngay_tao_don_hang = DateTime.Now,
				tong_gia = 0 // sẽ cập nhật sau
			};

			db.DonHangs.Add(donHang);
			db.SaveChanges(); // Lưu đơn hàng để có ID

			decimal tongGia = 0;

			// Duyệt qua các sản phẩm đã chọn và thêm vào chi tiết đơn hàng
			foreach (var gioHangId in selectedItems)
			{
				var gioHang = db.GioHangs.Find(gioHangId);

				if (gioHang != null)
				{
					var sanPham = db.SanPhams.Find(gioHang.san_pham_id);
					if (sanPham != null)
					{
						// Tính giá của sản phẩm (giá * số lượng)
						decimal giaSanPham = sanPham.gia * gioHang.so_luong_san_pham;

						// Tạo chi tiết đơn hàng
						ChiTietDonHang chiTiet = new ChiTietDonHang
						{
							don_hang_id = donHang.don_hang_id,
							san_pham_id = gioHang.san_pham_id,
							so_luong = gioHang.so_luong_san_pham,
							gia = giaSanPham
						};

						db.ChiTietDonHangs.Add(chiTiet);

						// Cập nhật tổng giá đơn hàng
						tongGia += giaSanPham;

						// Xóa sản phẩm khỏi giỏ hàng sau khi mua
						db.GioHangs.Remove(gioHang);
					}
				}
			}

			// Cập nhật tổng giá của đơn hàng
			donHang.tong_gia = tongGia;
			db.SaveChanges();

			TempData["Message"] = $"Đơn hàng của bạn đã được tạo thành công. Tổng giá: {tongGia:C}";
			return RedirectToAction("Create", "ThanhToans", new { don_hang_id = donHang.don_hang_id });
		}



		// GET: GioHangs/Details/5
		public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GioHang gioHang = db.GioHangs.Find(id);
            if (gioHang == null)
            {
                return HttpNotFound();
            }
            return View(gioHang);
        }

		// GET: GioHangs/Create
		public ActionResult Create(int san_pham_id, int so_luong_san_pham)
		{
			// Kiểm tra xem người dùng đã đăng nhập chưa
			var khach_hang_id = Session["NguoiDungId"];
			if (khach_hang_id == null)
			{
				// Nếu chưa đăng nhập, trả về yêu cầu đăng nhập
				return Json(new { success = false, requireLogin = true, message = "Bạn cần đăng nhập để thêm sản phẩm vào giỏ hàng." });
			}

			// Tạo giỏ hàng mới
			var gioHang = new GioHang
			{
				khach_hang_id = (int)khach_hang_id,
				san_pham_id = san_pham_id,
				so_luong_san_pham = so_luong_san_pham
			};

			if (ModelState.IsValid)
			{
				db.GioHangs.Add(gioHang);
				db.SaveChanges();
				return Json(new { success = true, message = "Sản phẩm đã được thêm vào giỏ hàng." });
			}

			// Trường hợp có lỗi
			return Json(new { success = false, message = "Không thể thêm sản phẩm vào giỏ hàng. Vui lòng thử lại sau." });
		}

		[HttpPost]
		public ActionResult UpdateSoLuongSanPham(int id, int newQuantity)
		{
			var item = db.GioHangs.FirstOrDefault(g => g.gio_hang_id == id);
			if (item != null)
			{
				// Cập nhật số lượng sản phẩm
				item.so_luong_san_pham = newQuantity;
				db.SaveChanges();
			}

			return Json(new { success = true, newQuantity = item.so_luong_san_pham });
		}

		// GET: GioHangs/Edit/5
		public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GioHang gioHang = db.GioHangs.Find(id);
            if (gioHang == null)
            {
                return HttpNotFound();
            }
            ViewBag.khach_hang_id = new SelectList(db.NguoiDungs, "nguoi_dung_id", "ten_dang_nhap", gioHang.khach_hang_id);
            ViewBag.san_pham_id = new SelectList(db.SanPhams, "san_pham_id", "ten_san_pham", gioHang.san_pham_id);
            return View(gioHang);
        }

        // POST: GioHangs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "gio_hang_id,khach_hang_id,san_pham_id,so_luong_san_pham")] GioHang gioHang)
        {
            if (ModelState.IsValid)
            {
                db.Entry(gioHang).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.khach_hang_id = new SelectList(db.NguoiDungs, "nguoi_dung_id", "ten_dang_nhap", gioHang.khach_hang_id);
            ViewBag.san_pham_id = new SelectList(db.SanPhams, "san_pham_id", "ten_san_pham", gioHang.san_pham_id);
            return View(gioHang);
        }

        // GET: GioHangs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GioHang gioHang = db.GioHangs.Find(id);
            if (gioHang == null)
            {
                return HttpNotFound();
            }
            return View(gioHang);
        }

        // POST: GioHangs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            GioHang gioHang = db.GioHangs.Find(id);
            db.GioHangs.Remove(gioHang);
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
