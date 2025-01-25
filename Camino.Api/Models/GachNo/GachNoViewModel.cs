using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Models.BenhNhans;
using Camino.Api.Models.YeuCauKhamBenh;
using Camino.Core.Domain;
using Camino.Core.Helpers;

namespace Camino.Api.Models.GachNo
{
    public class GachNoViewModel : BaseViewModel
    {
        public GachNoViewModel()
        {
            BenhNhan = new GachNoBenhNhanViewModel();
            CongTyBaoHiemTuNhan = new GachNoCongTyBaoHiemTuNhanViewModel();
        }
        public string SoChungTu { get; set; }
        public DateTime? NgayChungTu { get; set; }
        public Enums.LoaiChungTu? LoaiChungTu { get; set; }
        public string TenLoaiChungTu => LoaiChungTu?.GetDescription();
        public string KyKeToan { get; set; }
        public Enums.TrangThaiGachNo? TrangThai { get; set; }
        public string TenTrangThai => TrangThai?.GetDescription();
        public Enums.LoaiTienTe? LoaiTienTe { get; set; }
        public string TenLoaiTienTe => LoaiTienTe?.GetDescription();
        public decimal? TyGia { get; set; }
        public DateTime? NgayThucThu { get; set; }
        public Enums.LoaiDoiTuong? LoaiDoiTuong { get; set; }
        public string TenLoaiDoiTuong => LoaiDoiTuong?.GetDescription();
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
        public int? VAT { get; set; }
        public decimal? TienHachToan { get; set; }
        public string KhoanMucPhi { get; set; }
        public string SoHoaDon { get; set; }
        public DateTime? NgayHoaDon { get; set; }
        public decimal? TienThueHachToan { get; set; }
        public decimal? TongTienHachToan { get; set; }
        public bool IsShowXacNhanNhapLieu { get; set; }
        public bool IsXacNhanNhapLieu { get; set; }
        public bool IsDisabledView { get; set; }
        public bool? SuDungGoi { get; set; }

        public GachNoCongTyBaoHiemTuNhanViewModel CongTyBaoHiemTuNhan { get; set; }
        public GachNoBenhNhanViewModel BenhNhan { get; set; }
    }
}
