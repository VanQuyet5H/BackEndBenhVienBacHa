using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.BaoCao.BaoCaoKetQuaKhamChuaBenh
{
    public class BaoCaoKetQuaKhamChuaBenhVo
    {
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public string Hosting { get; set; }
    }
    public class ListDatePhieuBaoCao : GridItem
    {
        public List<DateTime> ItemColumns { get; set; }
    }
    public class LoaiHinhThuc
    {
        public decimal SoDuDauKy { get; set; }
        public string TenLoaiHinhThuc { get; set; }
        public string MaLoaiHinhThuc { get; set; }
        public DateTime NgayThangNam { get; set; }
    }
    public class KetQuaKCB
    {
        public KetQuaKCB()
        {
            LoaiHinhThucList = new List<LoaiHinhThuc>();
            DoiTuongBNList = new List<DoiTuongBN>();
        }
        public string MaHinhThucDen { get; set; }
        public string TenHinhThucDen { get; set; }
        public List<LoaiHinhThuc> LoaiHinhThucList { get; set; }
        public List<DoiTuongBN> DoiTuongBNList { get; set; }
        public string MaDoiTuongBenhNhan { get; set; }
        public string TenDoiTuongBenhNhan { get; set; }
        public DateTime NgayThangNam { get; set; }
    }
    public class DoanhThuNhaThuoc
    {
        public int SoNguoi { get; set; }
        public decimal DoanhThu { get; set; }
        public DateTime NgayThangNam { get; set; }
        public string TenDoanhThuNhaThuoc { get; set; }
    }
    public class KetQuaKCBVaDoanhThuNhaThuoc
    {
        public KetQuaKCBVaDoanhThuNhaThuoc()
        {
            KetQuaKCBList = new List<KetQuaKCB>();
            DoanhThuNhaThuocList = new List<DoanhThuNhaThuoc>();
        }
        public List<KetQuaKCB> KetQuaKCBList { get; set; }
        public List<DoanhThuNhaThuoc> DoanhThuNhaThuocList { get; set; }
        public decimal SoDuDauKy { get; set; }
        public string Ten { get; set; }
        public DateTime NgayThangNam { get; set; }
    }
    public class TongTienMat
    {
        public DateTime? NgayThangNam { get; set; }
        public string MaTongTienMat { get; set; }
        public decimal? SoDuDauKy { get; set; }
        public int? SoNguoi { get; set; }
        public decimal? DoanhThu { get; set; }
    }
    public class LoaiPhieuThu
    {
        public LoaiPhieuThu()
        {
            TongTienMatList = new List<TongTienMat>();
            NgayThangNamList = new List<NgayThangNamTongTien>();
        }
        public List<NgayThangNamTongTien> NgayThangNamList { get; set; }
        public string MaTongTienMat { get; set; }
        public string TenLoaiPhieuThu { get; set; }
        public List<TongTienMat> TongTienMatList { get; set; }
    }
    public class NgayThangNamTongTien
    {
        public DateTime NgayThangNam { get; set; }
    }
    public class DoiTuongBN
    {
        public DateTime NgayThangNam { get; set; }
        public string MaDoiTuongBN { get; set; }
        public string TenDoiTuongBN { get; set; }
        public decimal SoDuDauKy { get; set; }
        public int SoNguoi { get; set; }
        public decimal DoanhThu { get; set; }
    }
    public class CongNoPhaiThu
    {
         public CongNoPhaiThu()
        {
            ListCongNo = new List<CongNo>();
        }
        public string TenCongNo { get; set; }
        public List<CongNo> ListCongNo { get; set; }
    }
    public class CongNo{
        public decimal? SoDuDauKy { get; set; }
        public int? SoNguoi { get; set; }
        public decimal? DoanhThu { get; set; }
        public DateTime? NgayThangNam { get; set; }
    }

    public class DataTongHopBaoCaoKetQuaKhamChuaBenhVo
    {
        public DataTongHopBaoCaoKetQuaKhamChuaBenhVo()
        {
            Doc = new List<DataDocBaoCaoKetQuaKhamChuaBenhVo>();
            Data = new List<DataBaoCaoKetQuaKhamChuaBenhVo>();
        }
        public List<DataDocBaoCaoKetQuaKhamChuaBenhVo> Doc { get; set; }
        public List<DataBaoCaoKetQuaKhamChuaBenhVo> Data { get; set; }
        public decimal DoanhThuTrongThang { get; set; }
        public decimal DoanhThuLuyKeTrongNam { get; set; }
        public decimal SoLuotTiepNhan { get; set; }
    }

    public class DataDocBaoCaoKetQuaKhamChuaBenhVo
    {
        public string KeyDoc { get; set; }
        public string TT { get; set; }
        public string NoiDung { get; set; }
    }

    public class DataBaoCaoKetQuaKhamChuaBenhVo
    {
        public string KeyNgang { get; set; }
        public string KeyDoc { get; set; }
        public decimal? Value { get; set; }
    }

    public class ColumnExcelInfoVo
    {
        public string ColumnName { get; set; }
        public string ColumnKey { get; set; }
    }
}
