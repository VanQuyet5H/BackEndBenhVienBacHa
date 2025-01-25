using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using System;
using System.Collections.Generic;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.DieuTriNoiTru
{
    public class HoSoKhacGayMeGayTeViewModel : BaseViewModel
    {
        public HoSoKhacGayMeGayTeViewModel()
        {
            ThongTinQuanHeThanNhans = new List<ThongTinQuanHeThanNhanVo>();
            NoiTruHoSoKhacFileDinhKems = new List<NoiTruHoSoKhacFileDinhKemViewModel>();
        }
        public long YeuCauTiepNhanId { get; set; }
        public LoaiHoSoDieuTriNoiTru? LoaiHoSoDieuTriNoiTru { get; set; }
        public string ThongTinHoSo { get; set; }
        public long? NhanVienThucHienId { get; set; }
        public string TenNhanVienThucHien { get; set; }
        public long? NhanVienGiaiThichId { get; set; }
        public string TenNhanVienGiaiThich { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
        public string ThoiDiemThucHienDisplay { get; set; }
        public long? NoiThucHienId { get; set; }
        public string TenNoiThucHien { get; set; }
        public List<ThongTinQuanHeThanNhanVo> ThongTinQuanHeThanNhans { get; set; }
        public List<NoiTruHoSoKhacFileDinhKemViewModel> NoiTruHoSoKhacFileDinhKems { get; set; }

    }

    public class ThongTinQuanHeThanNhanJSON
    {
        public ThongTinQuanHeThanNhanJSON()
        {
            ThongTinQuanHeThanNhans = new List<ThongTinQuanHeThanNhanVo>();
        }
        public List<ThongTinQuanHeThanNhanVo> ThongTinQuanHeThanNhans { get; set; }
    }

    public class ThongTinQuanHeThanNhanViewModel : BaseViewModel
    {
        public string HoTen { get; set; }
        public int? NamSinh { get; set; }
        public string CMND { get; set; }
        public long? QuanHeThanNhanId { get; set; }
        public string TenQuanHeThanNhan { get; set; }
        public string DiaChi { get; set; }
    }

    public class NoiTruHoSoKhacFileDinhKemViewModel : BaseViewModel
    {
        public string Uid { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string TenGuid { get; set; }
        public long KichThuoc { get; set; }
        public string DuongDan { get; set; }
        public string MoTa { get; set; }
        public LoaiTapTin LoaiTapTin { get; set; }
    }
}
