//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Smartphonev3.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ThanhToan
    {
        public int thanh_toan_id { get; set; }
        public Nullable<int> don_hang_id { get; set; }
        public string phuong_thuc_thanh_toan { get; set; }
        public string trang_thai_thanh_toan { get; set; }
        public System.DateTime ngay_thanh_toan { get; set; }
    
        public virtual DonHang DonHang { get; set; }
		public decimal TongTien { get; set; }
	}
}
