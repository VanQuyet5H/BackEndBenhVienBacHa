using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.BienBanHoiChanSuDungThuocCoDau
{
    public class BienBanHoiChanSuDungThuocCoDauViewModel :BaseViewModel
    {
        public long YeuCauTiepNhanId { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
        public Enums.LoaiHoSoDieuTriNoiTru LoaiHoSoDieuTriNoiTru { get; set; }
        public string ThongTinHoSo { get; set; }
        public long? NhanVienThucHienId { get; set; }
        public long? NoiThucHienId { get; set; }
        public DateTime? HoiChanLuc { get; set; }
        public long? LanhDao { get; set; }
        public long? BsDieuTri { get; set; }

    }
    public class BienBanHoiChanSuDungThuocCoDauVo : BaseViewModel
    {
        public BienBanHoiChanSuDungThuocCoDauVo()
        {
            ThuocDaDieuTris = new List<string>();
            ThuocCoDaus = new List<string>();
            ListThemThuocCoDau = new List<ThuocCoDau>();
            ThuocDaDieuTriVos = new List<ThuocPhieuDieuTriBenhNhanItemVo>();
        }
        public string ChanDoanBenh { get; set; }
        public string TomTatTinhTrangBenhNhanKhiHoiChan { get; set; }
        public string ChanDoanBenhSauHoiChan { get; set; }
        public List<string> ThuocDaDieuTris { get; set; }
        public List<ThuocPhieuDieuTriBenhNhanItemVo> ThuocDaDieuTriVos { get; set; }
        public List<string> ThuocCoDaus { get; set; }
        public List<ThuocCoDau> ListThemThuocCoDau { get; set; }
        public long? NoiThucHienId { get; set; }
        public DateTime? HoiChanLuc { get; set; }
        public long? LanhDao { get; set; }
        public long? BsDieuTri { get; set; }

    }
    public class ThuocCoDau
    {
        public string Id { get; set; }
        public string LyDo { get; set; }
        public string TenThuoc { get; set; }
        public string HamLuong { get; set; }
        public string DuongDung { get; set; }

    }
}
