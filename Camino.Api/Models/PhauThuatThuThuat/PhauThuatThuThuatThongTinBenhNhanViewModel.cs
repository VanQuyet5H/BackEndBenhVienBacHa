using Camino.Api.Models.PhongBenhVien;
using Camino.Core.Domain;
using Camino.Api.Models.KhamBenh;
using iText.Layout.Element;
using Camino.Core.Domain.Entities.PhongBenhViens;
using System.Collections.Generic;
using Camino.Core.Domain.Entities.BenhNhans;
using static Camino.Core.Domain.Enums;
using Camino.Api.Models.KetQuaSinhHieu;
using Camino.Core.Helpers;
using System;
using Camino.Core.Domain.ValueObject.KhamBenhs;

namespace Camino.Api.Models.PhauThuatThuThuat
{
    public class PhauThuatThuThuatThongTinBenhNhanViewModel : BaseViewModel
    {
        public PhauThuatThuThuatThongTinBenhNhanViewModel()
        {
            GoiDichVus = new List<GoiDichVuTheoBenhNhanGridVo>();
        }

        public string MaTN { get; set; }
        public long BenhNhanId { get; set; }
        public string MaBN { get; set; }
        public string HoTen { get; set; }
        public string GioiTinh { get; set; }
        public string Tuoi { get; set; }
        public string SDT { get; set; }
        public string DanToc { get; set; }
        public string DiaChi { get; set; }
        public string NgheNghiep { get; set; }
        public string TuyenKham { get; set; }
        public string MucHuong { get; set; }
        public string LyDoTN { get; set; }
        public string NgayTN { get; set; }
        public decimal SoDuTaiKhoan { get; set; }
        public string SoDuTaiKhoanDisplay
        {
            get { return SoDuTaiKhoan.ApplyFormatMoneyVND(); }
        }
        public decimal SoDuTaiKhoanUocLuongConLai { get; set; }
        public string SoDuTaiKhoanUocLuongConLaiDisplay
        {
            get { return SoDuTaiKhoanUocLuongConLai.ApplyFormatMoneyVND(); }
        }
        public long YeuCauTiepNhanId { get; set; }
        public bool CoBHYT { get; set; }
        public string BHYTMaSoThe { get; set; }
        public bool? CoDichVuKhuyenMai { get; set; }
        public DateTime? BHYTNgayHieuLuc { get; set; }
        public DateTime? BHYTNgayHetHan { get; set; }
        public string BHYTThoiGianHieuLucDisplay => BHYTNgayHieuLuc?.ApplyFormatDate() + (BHYTNgayHieuLuc != null && BHYTNgayHetHan != null ? " - " : "") + BHYTNgayHetHan?.ApplyFormatDate();
        public bool IsBHYTHetHan => CoBHYT == true && BHYTNgayHieuLuc != null && BHYTNgayHetHan != null && (DateTime.Now.Date < BHYTNgayHieuLuc.Value.Date || DateTime.Now.Date > BHYTNgayHetHan.Value.Date);

        public EnumTrangThaiPhauThuatThuThuat TrangThaiPhauThuatThuThuat { get; set; }
        public List<KetQuaSinhHieuViewModel> KetQuaSinhHieus { get; set; }

        //public string TenDichVu { get; set; }
        //public int? SoThuTu { get; set; }

        //public string MaBenhNhan { get; set; }
        //public EnumTrangThaiPhauThuatThuThuat TrangThaiPhauThuatThuThuat { get; set; }
        //public EnumTrangThaiHangDoi TrangThai { get; set; }
        //public EnumTrangThaiYeuCauDichVuKyThuat TrangThaiDichVuKyThuat { get; set; }
        //public EnumTrangThaiHangDoi TrangThaiHangDoi { get; set; }
        public virtual KhamBenhYeuCauTiepNhanViewModel YeuCauTiepNhan { get; set; }

        public bool IsTuongTrinhLai { get; set; }
        public bool IsTuongTrinhLaiCoTheoDoi { get; set; }
        public bool IsTuongTrinhLaiCoKetLuan { get; set; }

        //BVHD-3800
        public bool? LaCapCuu { get; set; }

        //BVHD-3882
        public bool? LaHinhThucDenGioiThieu { get; set; }

        //BVHD-3941
        public string TenCongTyBaoHiemTuNhan { get; set; }

        public List<GoiDichVuTheoBenhNhanGridVo> GoiDichVus { get; set; }
    }
}
