using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.BaoCaos
{
    public class BaoCaoKetQuaKhamChuaBenhKTQueryInfoVo
    {
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
    }

    public class BaoCaoKetQuaKhamChuaBenhKTVo
    {
        public BaoCaoKetQuaKhamChuaBenhKTVo()
        {
            BaoCaoKetQuaColumnHeader = new List<BaoCaoKetQuaColumnHeaderVo>();
            BaoCaoKetQuaColumnTongKhamSucKhoeDoan = new List<BaoCaoKetQuaColumnKhamSucKhoeDoanVo>();
            BaoCaoKetQuaKhamSucKhoeDoans = new List<BaoCaoKetQuaKhamSucKhoeDoan>();

            BaoCaoKetQuaKhamChuaBenhVaDoanhThuNgay = new List<BaoCaoKetQuaColumnDoanhThuVo>();
            BaoCaoKetQuaBenhNhanTuDenKhachLe = new List<BaoCaoKetQuaColumnDoanhThuVo>();
            BaoCaoKetQuaBenhNhanTuDenNoiTru = new List<BaoCaoKetQuaColumnDoanhThuVo>();
            BaoCaoKetQuaBenhNhanTuDenNhiKcb = new List<BaoCaoKetQuaColumnDoanhThuVo>();
            BaoCaoKetQuaBenhNhanTuDenNhiKcbTiemChung = new List<BaoCaoKetQuaColumnDoanhThuVo>();
            BaoCaoKetQuaBenhNhanTuDenNhiTiemChung = new List<BaoCaoKetQuaColumnDoanhThuVo>();
            BaoCaoKetQuaBenhNhanTuDenNhiNoiTRu = new List<BaoCaoKetQuaColumnDoanhThuVo>();
            BaoCaoKetQuaBenhNhanTuDenTong = new List<BaoCaoKetQuaColumnDoanhThuVo>();
            BaoCaoKetQuaBenhNhanTuDenTong = new List<BaoCaoKetQuaColumnDoanhThuVo>();

            BaoCaoKetQuaSoSinh = new List<BaoCaoKetQuaColumnDoanhThuVo>();
            BaoCaoKetQuaPhatSinhNgoaiGoiKSKDoan = new List<BaoCaoKetQuaColumnDoanhThuVo>();
            BaoCaoKetQuaCBNV = new List<BaoCaoKetQuaColumnDoanhThuVo>();
            BaoCaoKetQuaCTV = new List<BaoCaoKetQuaColumnDoanhThuVo>();


            BaoCaoKetQuaVietDuc = new List<BaoCaoKetQuaColumnDoanhThuVo>();
            BaoCaoKetQuaThamMyBSTu = new List<BaoCaoKetQuaColumnDoanhThuVo>();
            BaoCaoKetQuaThamMyKhac = new List<BaoCaoKetQuaColumnDoanhThuVo>();

            BaoCaoKetQuaThaiSanTrongGoi = new List<BaoCaoKetQuaColumnDoanhThuVo>();
            BaoCaoKetQuaThaiSanNgoaiGoi = new List<BaoCaoKetQuaColumnDoanhThuVo>();
            BaoCaoKetQuaRangHamMat = new List<BaoCaoKetQuaColumnDoanhThuVo>();

            BaoCaoKetQuaDoanhThuBanThuoc = new List<BaoCaoKetQuaColumnDoanhThuVo>();
            BaoCaoKetQuaThucThuTienMatVaThe = new List<BaoCaoKetQuaColumnDoanhThuVo>();
            BaoCaoKetQuaThucThuTienMat = new List<BaoCaoKetQuaColumnDoanhThuVo>();

            BaoCaoKetQuaThucThuChuyenKhoan = new List<BaoCaoKetQuaColumnDoanhThuVo>();
            BaoCaoKetQuaThucThuQuetThe = new List<BaoCaoKetQuaColumnDoanhThuVo>();
            BaoCaoKetQuaCongNoThuDuoc = new List<BaoCaoKetQuaColumnDoanhThuVo>();
        }

        public List<BaoCaoKetQuaColumnHeaderVo> BaoCaoKetQuaColumnHeader { get; set; }
        //1.Khám sức khỏe đoàn
        public List<BaoCaoKetQuaColumnKhamSucKhoeDoanVo> BaoCaoKetQuaColumnTongKhamSucKhoeDoan { get; set; }
        //42	CN Quốc phòng
        //2	Cảng hàng không
        public List<BaoCaoKetQuaKhamSucKhoeDoan> BaoCaoKetQuaKhamSucKhoeDoans { get; set; }
        //2.Kết quả khám chữa bệnh và doanh thu ngày
        public List<BaoCaoKetQuaColumnDoanhThuVo> BaoCaoKetQuaKhamChuaBenhVaDoanhThuNgay { get; set; }
        public List<BaoCaoKetQuaColumnDoanhThuVo> BaoCaoKetQuaBenhNhanTuDenKhachLe { get; set; }
        public List<BaoCaoKetQuaColumnDoanhThuVo> BaoCaoKetQuaBenhNhanTuDenNoiTru { get; set; }
        public List<BaoCaoKetQuaColumnDoanhThuVo> BaoCaoKetQuaBenhNhanTuDenNhiKcb { get; set; }
        public List<BaoCaoKetQuaColumnDoanhThuVo> BaoCaoKetQuaBenhNhanTuDenNhiKcbTiemChung { get; set; }
        public List<BaoCaoKetQuaColumnDoanhThuVo> BaoCaoKetQuaBenhNhanTuDenNhiTiemChung { get; set; }
        public List<BaoCaoKetQuaColumnDoanhThuVo> BaoCaoKetQuaBenhNhanTuDenNhiNoiTRu { get; set; }
        public List<BaoCaoKetQuaColumnDoanhThuVo> BaoCaoKetQuaBenhNhanTuDenTong { get; set; }
        public List<BaoCaoKetQuaColumnDoanhThuVo> BaoCaoKetQuaSoSinh { get; set; }
        public List<BaoCaoKetQuaColumnDoanhThuVo> BaoCaoKetQuaPhatSinhNgoaiGoiKSKDoan { get; set; }
        public List<BaoCaoKetQuaColumnDoanhThuVo> BaoCaoKetQuaCBNV { get; set; }
        public List<BaoCaoKetQuaColumnDoanhThuVo> BaoCaoKetQuaCTV { get; set; }
        public List<BaoCaoKetQuaColumnDoanhThuVo> BaoCaoKetQuaVietDuc { get; set; }
        public List<BaoCaoKetQuaColumnDoanhThuVo> BaoCaoKetQuaThamMyBSTu { get; set; }
        public List<BaoCaoKetQuaColumnDoanhThuVo> BaoCaoKetQuaThamMyKhac { get; set; }
        public List<BaoCaoKetQuaColumnDoanhThuVo> BaoCaoKetQuaThaiSanTrongGoi { get; set; }
        public List<BaoCaoKetQuaColumnDoanhThuVo> BaoCaoKetQuaThaiSanNgoaiGoi { get; set; }
        public List<BaoCaoKetQuaColumnDoanhThuVo> BaoCaoKetQuaRangHamMat { get; set; }
        //3.Doanh thu bán thuốc theo ngày
        public List<BaoCaoKetQuaColumnDoanhThuVo> BaoCaoKetQuaDoanhThuBanThuoc { get; set; }
        //II	Trong đó thực thu tiền mặt và thẻ
        public List<BaoCaoKetQuaColumnDoanhThuVo> BaoCaoKetQuaThucThuTienMatVaThe { get; set; }
        public List<BaoCaoKetQuaColumnDoanhThuVo> BaoCaoKetQuaThucThuTienMat { get; set; }
        public List<BaoCaoKetQuaColumnDoanhThuVo> BaoCaoKetQuaThucThuChuyenKhoan { get; set; }
        public List<BaoCaoKetQuaColumnDoanhThuVo> BaoCaoKetQuaThucThuQuetThe { get; set; }
        public List<BaoCaoKetQuaColumnDoanhThuVo> BaoCaoKetQuaCongNoThuDuoc { get; set; }
    }

    public class BaoCaoKetQuaThucThuTheoNgay
    {
        public DateTime Ngay { get; set; }
        public decimal TienMat { get; set; }
        public decimal ChuyenKhoan { get; set; }
        public decimal QuetThe { get; set; }
        public decimal CongNoThuDuoc { get; set; }
    }

    public class BaoCaoKetQuaNhaThuocTheoNgay
    {
        public DateTime Ngay { get; set; }
        public decimal DoanhThu { get; set; }
        public int SoNguoi { get; set; }
    }

    public class BaoCaoKetQuaDoanhThuTheoNgay
    {
        public DateTime Ngay { get; set; }        
        public decimal DoanhThuBenhNhanTuDenKhachLe { get; set; }
        public decimal DoanhThuBenhNhanTuDenNoiTru { get; set; }
        public decimal DoanhThuBenhNhanTuDenNhiKcb { get; set; }
        public decimal DoanhThuBenhNhanTuDenNhiKcbTiemChung { get; set; }
        public decimal DoanhThuBenhNhanTuDenNhiTiemChung { get; set; }
        public decimal DoanhThuBenhNhanTuDenNhiNoiTRu { get; set; }
        public decimal DoanhThuSoSinh { get; set; }
        public decimal DoanhThuPhatSinhNgoaiGoiKSKDoan { get; set; }
        public decimal DoanhThuCBNV { get; set; }
        public decimal DoanhThuCTV { get; set; }
        public decimal DoanhThuVietDuc { get; set; }
        public decimal DoanhThuThamMyBSTu { get; set; }
        public decimal DoanhThuThamMyKhac { get; set; }
        public decimal DoanhThuThaiSanTrongGoi { get; set; }
        public decimal DoanhThuThaiSanNgoaiGoi { get; set; }
        public decimal DoanhThuRangHamMat { get; set; }

        public int SoNguoiBenhNhanTuDenKhachLe { get; set; }
        public int SoNguoiBenhNhanTuDenNoiTru { get; set; }
        public int SoNguoiBenhNhanTuDenNhiKcb { get; set; }
        public int SoNguoiBenhNhanTuDenNhiKcbTiemChung { get; set; }
        public int SoNguoiBenhNhanTuDenNhiTiemChung { get; set; }
        public int SoNguoiBenhNhanTuDenNhiNoiTRu { get; set; }
        public int SoNguoiSoSinh { get; set; }
        public int SoNguoiPhatSinhNgoaiGoiKSKDoan { get; set; }
        public int SoNguoiCBNV { get; set; }
        public int SoNguoiCTV { get; set; }
        public int SoNguoiVietDuc { get; set; }
        public int SoNguoiThamMyBSTu { get; set; }
        public int SoNguoiThamMyKhac { get; set; }
        public int SoNguoiThaiSanTrongGoi { get; set; }
        public int SoNguoiThaiSanNgoaiGoi { get; set; }
        public int SoNguoiRangHamMat { get; set; }
    }

    public class BaoCaoKetQuaColumnHeaderVo
    {
        public int Index { get; set; }
        public string MergeCellText { get; set; }
        public string Cell1Text { get; set; }
        public string Cell2Text { get; set; }
    }
    public class BaoCaoKetQuaColumnKhamSucKhoeDoanVo
    {
        public int Index { get; set; }
        public int? Cell1Value { get; set; }
        public string Cell2Value { get; set; }
    }
    public class BaoCaoKetQuaColumnDoanhThuVo
    {
        public int Index { get; set; }
        public decimal? Cell1Value { get; set; }
        public decimal? Cell2Value { get; set; }
    }
    public class BaoCaoKetQuaKhamSucKhoeDoan
    {
        public BaoCaoKetQuaKhamSucKhoeDoan()
        {
            BaoCaoKetQuaColumnKhamSucKhoeDoanVos = new List<BaoCaoKetQuaColumnKhamSucKhoeDoanVo>();
        }

        public List<BaoCaoKetQuaColumnKhamSucKhoeDoanVo> BaoCaoKetQuaColumnKhamSucKhoeDoanVos { get; set; }
    }

    public class BaoCaoKetQuaTiepNhanKSKDataVo
    {
        public long YeuCauTiepNhanId { get; set; }
        public long? HopDongKhamSucKhoeNhanVienId { get; set; }
        public long CongTyKhamSucKhoeId { get; set; }
        public string CongTyKhamSucKhoe { get; set; }

        public DateTime? ThoiDiemThucHienChuyenKhoaDauTien => BaoCaoKetQuaYeuCauKhamKSKDataVos?.OrderBy(o => o.ThoiDiemThucHien.GetValueOrDefault()).FirstOrDefault()?.ThoiDiemThucHien;
        public List<BaoCaoKetQuaYeuCauKhamKSKDataVo> BaoCaoKetQuaYeuCauKhamKSKDataVos { get; set; }
    }
    public class BaoCaoKetQuaYeuCauKhamKSKDataVo
    {
        public long Id { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
    }

    public class BaoCaoKetQuaYeuCauTiepNhanNhiNgoaiTruVo
    {
        public long Id { get; set; }
        public int CountYeuCauKhamBenh { get; set; }
        public int CountYeuCauDichVuKyThuatKhamSangLocTiemChung => BaoCaoKetQuaYeuCauTiepNhanNhiDVKTNgoaiTruVos?.Where(k => k.KhamSangLocTiemChungId != null).Count() ?? 0;
        public int CountYeuCauDichVuKyNgoaiTiemChung => BaoCaoKetQuaYeuCauTiepNhanNhiDVKTNgoaiTruVos?.Where(k => k.KhamSangLocTiemChungId == null && k.TiemChungId == null).Count() ?? 0;
        public List<BaoCaoKetQuaYeuCauTiepNhanNhiDVKTNgoaiTruVo> BaoCaoKetQuaYeuCauTiepNhanNhiDVKTNgoaiTruVos { get; set; }
    }
    public class BaoCaoKetQuaYeuCauTiepNhanNhiDVKTNgoaiTruVo
    {
        public long Id { get; set; }
        public long? KhamSangLocTiemChungId { get; set; }
        public long? TiemChungId { get; set; }
    }

    public class BaoCaoKetQuaYeuCauTiepNhanDataVo
    {
        public long Id { get; set; }
        public string MaYeuCauTiepNhan { get; set; }
        public Enums.EnumLoaiYeuCauTiepNhan LoaiYeuCauTiepNhan { get; set; }
        public long? BenhNhanId { get; set; }
        public long? HinhThucDenId { get; set; }
        public long? NoiGioiThieuId { get; set; }
        public DateTime ThoiDiemTiepNhan { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public bool? QuyetToanTheoNoiTru { get; set; }
        public bool BenhNhanNhi => NamSinh != null && ((ThoiDiemTiepNhan.Year - NamSinh.Value) <= 7 ? (CalculateHelper.TinhTuoi(NgaySinh, ThangSinh, NamSinh, ThoiDiemTiepNhan) <= 6) : false);
    }
    public class BaoCaoKetQuaYeuCauTiepNhanBenhNhanNhiDataVo
    {
        public long Id { get; set; }
    }
    public class BaoCaoKetQuaPhieuChiDataVo
    {
        public long Id { get; set; }
        public Enums.LoaiChiTienBenhNhan LoaiChiTienBenhNhan { get; set; }
        public decimal? TienChiPhi { get; set; }
        public decimal? TienMat { get; set; }
        public long? YeuCauKhamBenhId { get; set; }
        public long? DichVuKhamBenhBenhVienId { get; set; }
        public long? YeuCauDichVuKyThuatId { get; set; }
        public long? NhomDichVuBenhVienId { get; set; }
        public long? YeuCauDuocPhamBenhVienId { get; set; }
        public long? YeuCauVatTuBenhVienId { get; set; }
        public long? YeuCauDichVuGiuongBenhVienId { get; set; }
        public long? YeuCauGoiDichVuId { get; set; }
        public long? DonThuocThanhToanChiTietId { get; set; }
        public long? DonVTYTThanhToanChiTietId { get; set; }
        public long? YeuCauDichVuGiuongBenhVienChiPhiBenhVienId { get; set; }
        public long? YeuCauTruyenMauId { get; set; }
        public decimal? Gia { get; set; }
        public double? SoLuong { get; set; }
        public decimal? DonGiaBaoHiem { get; set; }
        public int? MucHuongBaoHiem { get; set; }
        public int? TiLeBaoHiemThanhToan { get; set; }
        public decimal? SoTienBaoHiemTuNhanChiTra { get; set; }
        public decimal? SoTienMienGiam { get; set; }
        public bool? DaHuy { get; set; }
    }
    public class BaoCaoKetQuaPhieuThuDataVo
    {
        public long Id { get; set; }
        public DateTime NgayThu { get; set; }
        public DateTime? NgayHuy { get; set; }
        public string SoBLHD { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public string MaYeuCauTiepNhan { get; set; }
        public Enums.EnumLoaiYeuCauTiepNhan LoaiYeuCauTiepNhan { get; set; }
        public bool? QuyetToanTheoNoiTru { get; set; }
        public long? YeuCauTiepNhanNgoaiTruCanQuyetToanId { get; set; }
        public string MaBenhNhan { get; set; }
        public string TenBenhNhan { get; set; }
        public string MaYTe { get; set; }
        public string SoBenhAn { get; set; }
        public string NamSinh { get; set; }
        public string NguoiGioiThieu { get; set; }

        public string ChiTietCongNo => (ChiTietCongNoTuNhans != null && ChiTietCongNoTuNhans.Any() ? string.Join("; ", ChiTietCongNoTuNhans.Distinct()) : string.Empty)
                                       + (CongNoCaNhan.GetValueOrDefault() > 0 ? $"{(ChiTietCongNoTuNhans != null && ChiTietCongNoTuNhans.Any() ? "; " : string.Empty)}{TenBenhNhan}" : string.Empty);
        public string SoHoaDonChiTiet { get; set; }
        public bool GoiDichVu { get; set; }

        public decimal? TamUng => (DaHuy.GetValueOrDefault() == false && LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTamUng)
            ? (SoTienThuTienMat.GetValueOrDefault() + SoTienThuChuyenKhoan.GetValueOrDefault() + SoTienThuPos.GetValueOrDefault())
            : (decimal?)null;

        public decimal? HoanUng => (DaHuy.GetValueOrDefault() == false && LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.HoanUng)
            ? SoTienThuTienMat.GetValueOrDefault()
            : (decimal?)null;

        public decimal? SoTienThu => (DaHuy.GetValueOrDefault() == false && LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi)
            ? (TongChiPhiBNTT.GetValueOrDefault() + CongNoTuNhan.GetValueOrDefault())
            : (DaHuy.GetValueOrDefault() == false && LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuNo) ? (SoTienThuTienMat.GetValueOrDefault() + SoTienThuChuyenKhoan.GetValueOrDefault() + SoTienThuPos.GetValueOrDefault()) : (decimal?)null;

        public decimal? HuyThu => LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.HoanThu ? (SoTienThuTienMat.GetValueOrDefault() * (DaHuy.GetValueOrDefault() ? -1 : 1))
                : (DaHuy.GetValueOrDefault() == true && LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.HoanUng) ? SoTienThuTienMat.GetValueOrDefault()
                : (DaHuy.GetValueOrDefault() == true && LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTamUng) ? (SoTienThuTienMat.GetValueOrDefault() + SoTienThuChuyenKhoan.GetValueOrDefault() + SoTienThuPos.GetValueOrDefault())
                : (DaHuy.GetValueOrDefault() == true && LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi) ? (TongChiPhiBNTT.GetValueOrDefault() + CongNoTuNhan.GetValueOrDefault())
                : (DaHuy.GetValueOrDefault() == true && LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuNo) ? (SoTienThuTienMat.GetValueOrDefault() + SoTienThuChuyenKhoan.GetValueOrDefault() + SoTienThuPos.GetValueOrDefault())
                : (decimal?)null;
        public decimal? ThucThu => TamUng.GetValueOrDefault() - HoanUng.GetValueOrDefault() + SoTienThu.GetValueOrDefault() - HuyThu.GetValueOrDefault() + ThuNoTienMat.GetValueOrDefault() + ThuNoChuyenKhoan.GetValueOrDefault() + ThuNoPos.GetValueOrDefault();
        public decimal? CongNo => LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi
            ? ((CongNoCaNhan.GetValueOrDefault() + CongNoTuNhan.GetValueOrDefault()) * (DaHuy.GetValueOrDefault() ? -1 : 1))
            : (decimal?)null;

        public decimal? TienMat => LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuNo ? (decimal?)null
            : (LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.HoanUng || LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.HoanThu) ? (SoTienThuTienMat.GetValueOrDefault() * -1 * (DaHuy.GetValueOrDefault() ? -1 : 1))
            : LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTamUng ? (DaHuy.GetValueOrDefault() == false ? SoTienThuTienMat.GetValueOrDefault() : SoTienThuTienMat.GetValueOrDefault() * (-1))
            //: LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTamUng ? (LaPhieuHuy == false ? SoTienThuTienMat.GetValueOrDefault() : (SoTienThuTienMat.GetValueOrDefault() + SoTienThuChuyenKhoan.GetValueOrDefault() + SoTienThuPos.GetValueOrDefault()) * (-1))
            : LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi ? (DaHuy.GetValueOrDefault() == false ? (TongChiPhiBNTT.GetValueOrDefault() - CongNoCaNhan.GetValueOrDefault() - SoTienThuChuyenKhoan.GetValueOrDefault() - SoTienThuPos.GetValueOrDefault()) : (TongChiPhiBNTT.GetValueOrDefault() - CongNoCaNhan.GetValueOrDefault() - SoTienThuChuyenKhoan.GetValueOrDefault() - SoTienThuPos.GetValueOrDefault()) * (-1))
            : (decimal?)null;

        public decimal? ChuyenKhoan => LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuNo ? (decimal?)null
            : LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTamUng ? (DaHuy.GetValueOrDefault() == false ? SoTienThuChuyenKhoan.GetValueOrDefault() : SoTienThuChuyenKhoan.GetValueOrDefault() * (-1))
            : LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi ? (DaHuy.GetValueOrDefault() == false ? SoTienThuChuyenKhoan.GetValueOrDefault() : SoTienThuChuyenKhoan.GetValueOrDefault() * (-1))
            : (decimal?)null;
        public decimal? Pos => LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuNo ? (decimal?)null
            : LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTamUng ? (DaHuy.GetValueOrDefault() == false ? SoTienThuPos.GetValueOrDefault() : SoTienThuPos.GetValueOrDefault() * (-1))
            : LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi ? (DaHuy.GetValueOrDefault() == false ? SoTienThuPos.GetValueOrDefault() : SoTienThuPos.GetValueOrDefault() * (-1))
            : (decimal?)null;
        //public decimal? ChuyenKhoan => LaPhieuHuy == false && LoaiThuTienBenhNhan != Enums.LoaiThuTienBenhNhan.ThuNo
        //    ? SoTienThuChuyenKhoan.GetValueOrDefault()
        //    : (decimal?)null;
        //public decimal? Pos => LaPhieuHuy == false && LoaiThuTienBenhNhan != Enums.LoaiThuTienBenhNhan.ThuNo
        //    ? SoTienThuPos.GetValueOrDefault()
        //    : (decimal?)null;
        //will remove
        public decimal? Voucher { get; set; }
        public string LyDo { get; set; }
        public string NguoiThu { get; set; }
        //public string StartDate { get; set; }
        //public string EndDate { get; set; }

        public Enums.LoaiThuTienBenhNhan LoaiThuTienBenhNhan { get; set; }
        public Enums.LoaiNoiThu LoaiNoiThu { get; set; }
        public Enums.LoaiChiTienBenhNhan LoaiChiTienBenhNhan { get; set; }
        public bool? DaHuy { get; set; }
        public decimal? ThuNoTienMat => LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuNo
            ? DaHuy.GetValueOrDefault() == false ? SoTienThuTienMat.GetValueOrDefault() : SoTienThuTienMat.GetValueOrDefault() * (-1)
            : (decimal?)null;
        public decimal? ThuNoChuyenKhoan => LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuNo
            ? DaHuy.GetValueOrDefault() == false ? SoTienThuChuyenKhoan.GetValueOrDefault() : SoTienThuChuyenKhoan.GetValueOrDefault() * (-1)
            : (decimal?)null;
        //? SoTienThuChuyenKhoan.GetValueOrDefault()
        //: (decimal?)null;
        public decimal? ThuNoPos => LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuNo
            ? DaHuy.GetValueOrDefault() == false ? SoTienThuPos.GetValueOrDefault() : SoTienThuPos.GetValueOrDefault() * (-1)
            : (decimal?)null;
        //? SoTienThuPos.GetValueOrDefault()
        //: (decimal?)null;
        public string SoPhieuThuGhiNo { get; set; }
        public decimal? TongChiPhiBNTT => BaoCaoKetQuaPhieuChiDataVos?.Where(chi => chi.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi).Select(chi => chi.TienChiPhi.GetValueOrDefault()).DefaultIfEmpty().Sum();
        public decimal? CongNoTuNhan { get; set; }
        public decimal? CongNoCaNhan { get; set; }
        public decimal? SoTienThuTamUng { get; set; }
        public decimal? SoTienThuTienMat { get; set; }
        public decimal? SoTienThuChuyenKhoan { get; set; }
        public decimal? SoTienThuPos { get; set; }
        public IEnumerable<string> ChiTietCongNoTuNhans { get; set; }
        public bool CoTiemChung { get; set; }
        public List<ChiTietBHYT> ChiTietBHYTs { get; set; }
        public decimal? BHYT { get; set; }
        public bool BenhAnSoSinh { get; set; }
        public List<BaoCaoKetQuaPhieuChiDataVo> BaoCaoKetQuaPhieuChiDataVos { get; set; }
    }
}
