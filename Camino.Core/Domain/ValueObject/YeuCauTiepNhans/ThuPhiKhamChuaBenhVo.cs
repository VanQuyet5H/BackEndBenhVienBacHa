using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.YeuCauTiepNhans
{
    public class QuyetToanDichVuTrongGoiVo
    {
        public QuyetToanDichVuTrongGoiVo()
        {
            DanhSachChiPhiKhamChuaBenhTrongGoiDichVus = new List<ChiPhiKhamChuaBenhTrongGoiDichVuVo>();
        }
        public long Id { get; set; }
        public decimal? TienMat { get; set; }
        public DateTime NgayThu { get; set; }
        public string NoiDungThu { get; set; }
        public List<ChiPhiKhamChuaBenhTrongGoiDichVuVo> DanhSachChiPhiKhamChuaBenhTrongGoiDichVus { get; set; }
        //public List<ChiPhiKhamChuaBenhTrongGoiDichVuVo> DanhSachGoiCoBHYTVos { get; set; }
    }


    public class ThuPhiKhamChuaBenhVo
    {
        public ThuPhiKhamChuaBenhVo()
        {
            DanhSachChiPhiKhamChuaBenhDaChons = new List<ChiPhiKhamChuaBenhVo>();
        }
        public long Id { get; set; }
        public decimal? TienMat { get; set; }
        public decimal? ChuyenKhoan { get; set; }
        public decimal? POS { get; set; }
        public decimal? SoTienCongNo { get; set; }
        public decimal? SoTienHoanLaiThanhToan { get; set; }
        public DateTime NgayThu { get; set; }
        public string NoiDungThu { get; set; }
        public string NoiDungCongNo { get; set; }

        public List<ChiPhiKhamChuaBenhVo> DanhSachChiPhiKhamChuaBenhDaChons { get; set; }
    }

    public class DanhSachChiPhiThuTamUng
    {
        public DanhSachChiPhiThuTamUng()
        {
            DanhSachChiPhiKhamChuaBenhDaChons = new List<ChiPhiKhamChuaBenhVo>();
        }
        public List<ChiPhiKhamChuaBenhVo> DanhSachChiPhiKhamChuaBenhDaChons { get; set; }
    }

    public class DoiLoaiGiaDanhSachChiPhiKhamChuaBenhNoiTru
    {
        public DoiLoaiGiaDanhSachChiPhiKhamChuaBenhNoiTru()
        {
            ChiPhiKhamChuaBenhNoiTruVos = new List<ChiPhiKhamChuaBenhNoiTruVo>();
        }
        public long YeuCauTiepNhanId { get; set; }
        public long? LoaiGiaDichVuKhamBenhBenhVienId { get; set; }
        public long? LoaiGiaDichVuKyThuatBenhVienId { get; set; }
        public long? LoaiGiaDichVuGiuongBenhVienId { get; set; }
        public List<ChiPhiKhamChuaBenhNoiTruVo> ChiPhiKhamChuaBenhNoiTruVos { get; set; }
    }

    public class ThuPhiKhamChuaBenhNoiTruVo
    {
        public ThuPhiKhamChuaBenhNoiTruVo()
        {
            DanhSachChiPhiKhamChuaBenhDaChons = new List<ChiPhiKhamChuaBenhNoiTruVo>();
        }
        public long Id { get; set; }
        public decimal? TienMat { get; set; }
        public decimal? ChuyenKhoan { get; set; }
        public decimal? POS { get; set; }
        public decimal? SoTienCongNo { get; set; }
        public decimal? SoTienHoanLaiThanhToan { get; set; }
        public DateTime NgayThu { get; set; }
        public string NoiDungThu { get; set; }
        public string NoiDungCongNo { get; set; }
        public List<ChiPhiKhamChuaBenhNoiTruVo> DanhSachChiPhiKhamChuaBenhDaChons { get; set; }
    }

    public class KetQuaThuPhiKhamChuaBenhNoiTruVo
    {
        public long PhieuThuId { get; set; }
        public bool LaPhieuChi { get; set; }
        public long PhieuHoanUngId { get; set; }
        public string Error { get; set; }
    }

    public class KetQuaThuPhiKhamChuaBenhNoiTruVaQuyetToanDichVuTrongGoiVo
    {
        public List<long> PhieuThuIds { get; set; }
        public bool LaPhieuChi { get; set; }
        public List<long> PhieuHoanUngIds { get; set; }
        public string Error { get; set; }
    }
    public class KetQuaThuPhiKhamChuaBenhNgoaiTruVo
    {
        public long PhieuThuId { get; set; }
        public bool LaPhieuChi { get; set; }
        public long PhieuHoanUngId { get; set; }
        public string Error { get; set; }
    }
    public class KetQuaQuyetToanDichVuTrongGoiCoBHYT
    {
        public long PhieuQuyetToanId { get; set; }
        public List<long> PhieuHoanUngIds { get; set; }
        public string Error { get; set; }
    }
    public class KetQuaThuPhiKhamChuaBenhNgoaiTruVaQuyetToanDichVuTrongGoiVo
    {
        public List<long> PhieuThuIds { get; set; }
        public bool LaPhieuChi { get; set; }
        public List<long> PhieuHoanUngIds { get; set; }
        public string Error { get; set; }
    }

    public class CongNoVo
    {
        public long CongTyCongNoId { get; set; }
        public decimal SoTienCongNo { get; set; }
    }

    public class ThongTinHuyPhieuVo
    {
        public long SoPhieu { get; set; }
        public LoaiPhieuThuChiThuNgan LoaiPhieuThuChiThuNgan { get; set; }
        public bool? ThuHoiPhieu { get; set; }
        public long? NguoiThuHoiId { get; set; }
        public string TenNguoiThuHoi { get; set; }
        public DateTime? ThoiGianThuHoi { get; set; }
        public string LyDo { get; set; }

        public bool? HuyPhieuHoanUng { get; set; }
        public bool? ChuyenTienQuaTamUng { get; set; }        
    }

    public class CapNhatDonGiaMoi
    {
        public long Id { get; set; }
        public NhomChiPhiKhamChuaBenh LoaiNhom { get; set; }
        public decimal DonGia { get; set; }
        public decimal? DonGiaCapNhat { get; set; }
    }
}
