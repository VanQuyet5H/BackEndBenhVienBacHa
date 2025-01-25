using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.CongTyBaoHiemTuNhans;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.Users;

namespace Camino.Core.Domain.Entities.GachNos
{
    public class GachNo : BaseEntity
    {
        public string SoChungTu { get; set; }
        public DateTime NgayChungTu { get; set; }
        public Enums.LoaiChungTu LoaiChungTu { get; set; }
        public string KyKeToan { get; set; }
        public Enums.TrangThaiGachNo TrangThai { get; set; }
        public Enums.LoaiTienTe LoaiTienTe { get; set; }
        public decimal TyGia { get; set; }
        public DateTime NgayThucThu { get; set; }
        public Enums.LoaiDoiTuong LoaiDoiTuong { get; set; }
        public long? CongTyBaoHiemTuNhanId { get; set; }
        public long? BenhNhanId { get; set; }
        public string TaiKhoan { get; set; }
        public string TaiKhoanLoaiTien { get; set; }
        public string NguoiNop { get; set; }
        public string ChungTuGoc { get; set; }
        public string DienGiaiChung { get; set; }
        public string SoTaiKhoanNganHang { get; set; }
        public string NguyenTe { get; set; }
        public string ThueNguyenTe { get; set; }
        public string TongNguyenTe { get; set; }
        public string HachToan { get; set; }
        public string ThueHachToan { get; set; }
        public string TongHachToan { get; set; }
        public string LoaiThuChi { get; set; }
        public int VAT { get; set; }
        public decimal TienHachToan { get; set; }
        public string KhoanMucPhi { get; set; }
        public string SoHoaDon { get; set; }
        public DateTime? NgayHoaDon { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal TienThueHachToan { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal TongTienHachToan { get; set; }

        public bool? SuDungGoi { get; set; }

        public virtual CongTyBaoHiemTuNhan CongTyBaoHiemTuNhan { get; set; }
        public virtual BenhNhan BenhNhan { get; set; }
        public virtual NhanVien NguoiXacNhanNhapLieu { get; set; }
    }
}
