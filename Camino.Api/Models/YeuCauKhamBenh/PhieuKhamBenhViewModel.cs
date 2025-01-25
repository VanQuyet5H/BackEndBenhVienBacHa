using Camino.Api.Models.DichVuKhamBenh;
using Camino.Api.Models.KhoaPhong;
using Camino.Api.Models.NhanVien;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.BenhNhans;
using System;
using Camino.Core.Helpers;

namespace Camino.Api.Models.YeuCauKhamBenh
{
    public class PhieuKhamBenhViewModel
    {
        public string BarCodeImgBase64 { get; set; }
        public string MaTN { get; set; }
        public string MaBN { get; set; }
        public string Ngay { get; set; }
        public string Thang { get; set; }
        public string Nam { get; set; }
        public string HoTen { get; set; }
        public string NamSinh { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string GioiTinhString
        {
            get { return GioiTinh != null ? GioiTinh.GetDescription() : ""; }
        }
        public string DiaChi { get; set; }
        public string DienThoai { get; set; }
        public string DoiTuong { get; set; }
        public string SoTheBHYT { get; set; }
        public string HanThe { get; set; }
        public string Now { get; set; }
        public string NowTime { get; set; }
        public string NgayIn { get; set; }

        public string NgayHieuLuc { get; set; }
        public string GioKhamDuKien { get; set; }
        public string LogoUrl { get; set; }
        public string GioiTinhDisPlay { get; set; }

        public string DangKyKham { get; set; }

        public string NoiYeuCau { get; set; }
        public string ChuanDoanSoBo { get; set; }
        public string DanhSachDichVu { get; set; }
        public string NguoiChiDinh { get; set; }
        public string NguoiGiamHo { get; set; }
        public string TenQuanHeThanNhan { get; set; }

        //BVHD-3800
        public bool? LaCapCuu { get; set; }
        public string CapCuu => LaCapCuu != true ? "" : "Cấp Cứu";
    }

}
