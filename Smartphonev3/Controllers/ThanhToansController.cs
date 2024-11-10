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
    public class ThanhToansController : Controller
    {
        private SmartphoneEntitiesv3_1 db = new SmartphoneEntitiesv3_1();

        // GET: ThanhToans
        public ActionResult Index()
        {
			var thanhToans = new List<ThanhToan>();
			var vaiTro = Session["Role"]?.ToString();
			if (vaiTro == "Admin")
			{
				// Nếu là Admin, hiển thị tất cả các thông tin thanh toán
				thanhToans = db.ThanhToans.Include(t => t.DonHang).ToList();
			}
			else
			{
				// Nếu là người dùng bình thường, chỉ hiển thị thanh toán của người dùng hiện tại
				var khachHangId = (int)Session["NguoiDungId"];
				thanhToans = db.ThanhToans.Include(t => t.DonHang)
										   .Where(t => t.DonHang.khach_hang_id == khachHangId)
										   .ToList();
			}

			foreach (var item in thanhToans)
			{
				// Lấy tổng tiền của các sản phẩm trong chi tiết đơn hàng
				var chiTietDonHangs = db.ChiTietDonHangs.Where(c => c.don_hang_id == item.don_hang_id).ToList();
				decimal tongTien = chiTietDonHangs.Sum(c => c.gia);

				// Thêm tổng tiền vào đối tượng thanhToan
				item.TongTien = tongTien;
			}

			return View(thanhToans);
		}

        // GET: ThanhToans/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ThanhToan thanhToan = db.ThanhToans.Find(id);
            if (thanhToan == null)
            {
                return HttpNotFound();
            }
            return View(thanhToan);
        }

		// GET: ThanhToans/Create
		public ActionResult Create(int? don_hang_id)
		{
			if (don_hang_id == null)
			{
				return HttpNotFound();  // Hoặc xử lý lỗi nếu không tìm thấy don_hang_id
			}

			var donHang = db.DonHangs.Include(d => d.ChiTietDonHangs)
									 .FirstOrDefault(d => d.don_hang_id == don_hang_id);

			if (donHang == null)
			{
				return HttpNotFound();  // Nếu không tìm thấy đơn hàng
			}

			decimal tongTien = donHang.ChiTietDonHangs.Sum(c => c.gia);

			ThanhToan thanhToan = new ThanhToan
			{
				don_hang_id = don_hang_id.Value,  // Unwrap Nullable<int> here safely
				TongTien = tongTien
			};

			ViewBag.TongTien = tongTien;

			return View(thanhToan);
		}


		// POST: ThanhToans/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to, for 
		// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "thanh_toan_id,don_hang_id,phuong_thuc_thanh_toan,trang_thai_thanh_toan,ngay_thanh_toan")] ThanhToan thanhToan)
		{
			if (ModelState.IsValid)
			{
				// Kiểm tra phương thức thanh toán và thiết lập trạng thái thanh toán
				if (thanhToan.phuong_thuc_thanh_toan == "Cash on Delivery")
				{
					thanhToan.trang_thai_thanh_toan = "Unpaid"; // Nếu là Tiền Mặt thì trạng thái là "Chưa Thanh Toán"
				}
				else
				{
					thanhToan.trang_thai_thanh_toan = "Payment Completed"; // Các phương thức khác sẽ là "Đã Thanh Toán"
				}

				// Cập nhật ngày thanh toán tự động theo ngày thực
				thanhToan.ngay_thanh_toan = DateTime.Now;

				db.ThanhToans.Add(thanhToan);
				db.SaveChanges();
				return RedirectToAction("Index");
			}

			// Recalculate the total price if the form fails to validate
			if (thanhToan.don_hang_id.HasValue)
			{
				var donHang = db.DonHangs.Include(d => d.ChiTietDonHangs)
										 .FirstOrDefault(d => d.don_hang_id == thanhToan.don_hang_id);

				if (donHang != null)
				{
					decimal tongTien = donHang.ChiTietDonHangs.Sum(c => c.gia);
					ViewBag.TongTien = tongTien;
				}
			}

			ViewBag.don_hang_id = thanhToan.don_hang_id;
			return View(thanhToan);
		}
		
		[HttpPost]
		public ActionResult CreateUnpaidRecord(int donHangId, string phuongThuc)
		{
			// Kiểm tra xem đơn hàng có tồn tại hay không
			var donHang = db.DonHangs.Find(donHangId);
			if (donHang == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			// Tạo bản ghi thanh toán chưa hoàn thành
			var thanhToan = new ThanhToan
			{
				don_hang_id = donHangId,
				phuong_thuc_thanh_toan = phuongThuc,
				trang_thai_thanh_toan = "Unpaid", // Trạng thái chưa thanh toán
				ngay_thanh_toan = DateTime.Now
			};

			db.ThanhToans.Add(thanhToan);
			db.SaveChanges();

			return new HttpStatusCodeResult(200); // Trả về trạng thái thành công
		}




		// GET: ThanhToans/Edit/5
		public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ThanhToan thanhToan = db.ThanhToans.Find(id);
            if (thanhToan == null)
            {
                return HttpNotFound();
            }
            ViewBag.don_hang_id = new SelectList(db.DonHangs, "don_hang_id", "trang_thai_don_hang", thanhToan.don_hang_id);
            return View(thanhToan);
        }

        // POST: ThanhToans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "thanh_toan_id,don_hang_id,phuong_thuc_thanh_toan,trang_thai_thanh_toan,ngay_thanh_toan")] ThanhToan thanhToan)
        {
            if (ModelState.IsValid)
            {
                db.Entry(thanhToan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.don_hang_id = new SelectList(db.DonHangs, "don_hang_id", "trang_thai_don_hang", thanhToan.don_hang_id);
            return View(thanhToan);
        }

        // GET: ThanhToans/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ThanhToan thanhToan = db.ThanhToans.Find(id);
            if (thanhToan == null)
            {
                return HttpNotFound();
            }
            return View(thanhToan);
        }

        // POST: ThanhToans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ThanhToan thanhToan = db.ThanhToans.Find(id);
            db.ThanhToans.Remove(thanhToan);
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
