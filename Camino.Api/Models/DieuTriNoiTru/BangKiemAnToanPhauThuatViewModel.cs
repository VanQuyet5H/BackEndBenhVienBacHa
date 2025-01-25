using System;
using System.Collections.Generic;

namespace Camino.Api.Models.DieuTriNoiTru
{
    public class BangKiemAnToanPhauThuatViewModel
    {
        public BangKiemAnToanPhauThuatViewModel()
        {
            FileChuKy = new List<FileChuKyViewModel>();
        }

        public bool? BsCheckHoTen { get; set; }

        public bool? DdCheckHoTen { get; set; }

        public int? XnKhangSinhDuPhong { get; set; }

        public bool? XnVtRachDa { get; set; }

        public bool? XnChamSocSauMo { get; set; }

        public bool? XnNhanDan { get; set; }

        public bool? CheckGioiThieuEkip { get; set; }

        public bool? DdDemDungCu { get; set; }

        public bool? XnMo { get; set; }

        public bool? XNLaiThongTinNguoibenh { get; set; }

        public int? DanhDauVungMo { get; set; }

        public bool? XnDu { get; set; }

        public bool? XnMonitor { get; set; }

        public bool? XnTienSuDiUng { get; set; }

        public bool? XnKhoTho { get; set; }

        public bool? XnMatMau { get; set; }

        public bool? XnDoiChieuHa { get; set; }

        public bool? PtvXnChuY { get; set; }

        public bool? BsXnChuY { get; set; }

        public bool? DdXnChuY { get; set; }

        public string DdChayNgoai { get; set; }

        public string DdDungCu { get; set; }

        public string DdGayMe { get; set; }

        public string BsGayMe { get; set; }

        public string PhauThuatVien { get; set; }

        public string NguoiThucHienReadonly { get; set; }

        public string NgayThucHienReadonly { get; set; }

        public string ChanDoan { get; set; }

        public string PhongMo { get; set; }

        public DateTime? NgayKiem { get; set; }
        public string NgayKiemString { get; set; }

        public long? IdNoiTruHoSo { get; set; }

        public string ChuThich { get; set; }

        public List<FileChuKyViewModel> FileChuKy { get; set; }

        public int? DanNhanBenhPham { get; set; }
        public bool? ThuocvaThietBiGayMeCoDayDuKhong { get; set; }
        public bool? DanLuu { get; set; }
        public bool? XacNhanDieuCanChuYTrongPT { get; set; }
        public bool? XacNhanDieuCanChuYVeHoiTinhVaChamSocSauMo { get; set; }
        public bool? NguoiBenhCoTienSuDiUng { get; set; }
        public bool? BSGayMeCanChuYTrongGayMe { get; set; }
        public bool? KimGacDungCu { get; set; }
        public int? DatPlaQueDaoDien { get; set; }
        public string PhuongPhapPhauThuat { get; set; }
        public string PhuongPhapVoCam { get; set; }
    }
}
