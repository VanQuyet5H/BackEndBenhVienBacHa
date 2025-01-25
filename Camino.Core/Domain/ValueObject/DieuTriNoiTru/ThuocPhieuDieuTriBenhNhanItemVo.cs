using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.DieuTriNoiTru
{
    public class ThuocPhieuDieuTriBenhNhanItemVo
    {
        public string KeyId { get; set; }
        public string DisplayName { get; set; }
        public long? DuocPhamBenhVienId { get; set; }
        public string Ten { get; set; }
        public string HoatChat { get; set; }
        public string DVT { get; set; }
        public long? DuocPhamBenhVienPhanNhomId { get; set; }
        public string MaDuocPhamBenhVien { get; set; }
        public bool? SuDungTaiBenhVien { get; set; }
        public string DuongDung { get; set; }
        public int? Rank { get; set; }
        public string HamLuong { get; set; }
        public string NhaSanXuat { get; set; }
        public bool CheckThuoc { get; set; }
    }
    public class BienBanHoiChanSuDungThuocCoDauGidVo : GridItem
    {
        public BienBanHoiChanSuDungThuocCoDauGidVo()
        {
            ThuocDaDieuTris = new List<string>();
            ThuocCoDaus = new List<string>();
            ListThemThuocCoDau = new List<ThuocCoDauVo>();
            ThuocDaDieuTriVos = new List<ThuocPhieuDieuTriBenhNhanItemVo>();
        }
        public string ChanDoanBenh { get; set; }
        public string TomTatTinhTrangBenhNhanKhiHoiChan { get; set; }
        public string ChanDoanBenhSauHoiChan { get; set; }
        public List<string> ThuocDaDieuTris { get; set; }
        public List<ThuocPhieuDieuTriBenhNhanItemVo> ThuocDaDieuTriVos { get; set; }
        public List<string> ThuocCoDaus { get; set; }
        public List<ThuocCoDauVo> ListThemThuocCoDau { get; set; }
        public long? NoiThucHienId { get; set; }
        public DateTime? HoiChanLuc { get; set; }
        public long? LanhDao { get; set; }
        public long? BsDieuTri { get; set; }

    }
    public class ThuocCoDauVo
    {
        public string Id { get; set; }
        public string LyDo { get; set; }
        public string TenThuoc { get; set; }
        public string HamLuong { get; set; }
        public string DuongDung { get; set; }

    }
}
