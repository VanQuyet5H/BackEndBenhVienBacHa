using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.BaoCaos
{
    public class BaoCaoTongHopCongNoChuaThanhToanTimKiemVo : QueryInfo
    {
        public long? HinhThucDenId { get; set; }
        public long? NoiGioiThieuId { get; set; }
        public string MaYeuCauTiepNhan { get; set; }
        public string SearchString { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public DateTime? TuNgayFormat { get; set; }
        public DateTime? DenNgayFormat { get; set; }

        //BVHD-3882
        public bool? LaNguoiBenhNgoaiTru { get; set; }
    }

    public class BaoCaoTongHopCongNoChuaThanhToanGridVo : GridItem
    {
        public int? STT { get; set; }
        public long YeucauTiepNhanId { get; set; }
        public string MaYeuCauTiepNhan { get; set; }
        public long? NoiGioiThieuId { get; set; }
        public DateTime? ThoiDiemTiepNhan { get; set; }
        public string ThoiDiemTiepNhanDisplay => ThoiDiemTiepNhan?.ApplyFormatDate();
        public Enums.EnumTrangThaiYeuCauTiepNhan TrangThaiYeuCauTiepNhan { get; set; }
        public string NoiGioiThieuDisplay { get; set; }
        public long BenhNhanId { get; set; }
        public string MaBN { get; set; }
        public string HoTen { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public string NgaySinhDisplay => DateHelper.DOBFormat(NgaySinh, ThangSinh, NamSinh);
        public string DiaChi { get; set; }
        public string PhongPhauThuat { get; set; }
        public string NgayPhauThuatDisplay { get; set; }
        public string DichVuKyThuat { get; set; }
        public decimal? ChiPhiCanLamSan { get; set; }
        public decimal? ChiPhiCanLamSanNgoaiTru { get; set; }
        public decimal? ChiPhiCanLamSanNoiTru { get; set; }
        public decimal? ChiPhiGiuong { get; set; }
        public decimal? ChiPhiThuocVTYT { get; set; }
        public decimal? ChiPhiThuoc { get; set; }
        public decimal? ChiPhiVTYT { get; set; }
        public decimal? ChiPhiThuePhongMo { get; set; }
        public decimal? GiamDau { get; set; }
        public decimal? TestCovid { get; set; }
        public decimal? SuatAn { get; set; }
        public decimal? NguoiBenhDaThanhToan { get; set; }
        public decimal? CongNoChuaThanhToan { get; set; }
        public decimal? ChiPhiCaPhauThuat { get; set; }
        public string ThoiGianGayMeDisplay { get; set; }
        public string ThoiGianBanGiaoDisplay { get; set; }
        public string ThoiGianBatDauThuePhongDisplay { get; set; }
        public string ThoiGianKetThucThuePhongDisplay { get; set; }
        public bool RaVien => TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DaHoanTat;

        public string TenHinhThucDen { get; set; }
        public bool? LaGioiThieu { get; set; }
        public string HinhThucDenDisplay => LaGioiThieu == true 
            ? (TenHinhThucDen + ((!string.IsNullOrEmpty(TenHinhThucDen) && !string.IsNullOrEmpty(NoiGioiThieuDisplay)) ? "/ " : "") + NoiGioiThieuDisplay) 
            : TenHinhThucDen;

        public bool? LaDataTheoDieuKienTimKiem { get; set; }

        //BVHD-3917
        public decimal? ChiPhiChuaThucHien { get; set; }

        public decimal? ChiPhiThuocChuaThucHien { get; set; }
        public decimal? ChiPhiVTYTChuaThucHien { get; set; }
        public decimal? DichVuKhac => ChiPhiCaPhauThuat.GetValueOrDefault()
                                      - ChiPhiCanLamSanNoiTru.GetValueOrDefault()
                                      - ChiPhiCanLamSanNgoaiTru.GetValueOrDefault()
                                      - ChiPhiChuaThucHien.GetValueOrDefault()
                                      - ChiPhiGiuong.GetValueOrDefault()
                                      - ChiPhiThuoc.GetValueOrDefault()
                                      - ChiPhiThuocChuaThucHien.GetValueOrDefault()
                                      - ChiPhiVTYT.GetValueOrDefault()
                                      - ChiPhiVTYTChuaThucHien.GetValueOrDefault()
                                      - ChiPhiThuePhongMo.GetValueOrDefault()
                                      - GiamDau.GetValueOrDefault();
    }

    public class BaoCaoTongHopCongNoChuaThanhToanChiPhiTheoTungNhomVo
    {
        public string MaYeuCauTiepNhan { get; set; }
        public decimal? ChiPhi { get; set; }
        public bool LaBenhNhanThu { get; set; }
    }

    public class BaoCaoTongHopCongNoChuaThanhToanChiPhiTungDichVuVo : BaoCaoTongHopCongNoChuaThanhToanChiPhiTheoTungNhomVo
    {
        public BaoCaoTongHopCongNoChuaThanhToanChiPhiTungDichVuVo()
        {
            PhieuChis = new List<ThongTinPhieuChiVo>();
            PhuongPhapTinhGiaTriTonKhos = new List<Enums.PhuongPhapTinhGiaTriTonKho>();
            ChiPhiBHYTDichVuGiuongs = new List<ThongTinChiPhiBHYTDichVuVo>();
        }

        public string TenDichVu { get; set; }
        public Enums.EnumNhomGoiDichVu? NhomDichVu { get; set; }
        public Enums.LoaiDichVuKyThuat? LoaiDichVuKyThuat { get; set; }
        public DateTime? NgayPhauThuat { get; set; }
        public DateTime? ThoiGianGayMe { get; set; }
        public DateTime? ThoiGianBanGiao { get; set; }
        public string NoiThucHien { get; set; }
        public bool DaThucHien { get; set; }
        public bool? LaNoiTru { get; set; }

        //BVHD-3917 : chi phí thuê phòng mổ
        public bool CoThuePhong { get; set; }
        public long? YeuCauDichVuKyThuatChiPhiThuePhongId { get; set; }
        public ChiPhiThuePhongVo ChiPhiThuePhong { get; set; }

        //BVHD-3917
        //tham khảo từ GetDataThongKeCacDVChuaLayLenBienLaiThuTienForGrid
        public double SoLuong { get; set; }
        public bool DuocHuongBHYT { get; set; }
        public bool? BaoHiemChiTra { get; set; }
        public decimal DonGiaBHYT { get; set; }
        public int TiLeBaoHiemThanhToan { get; set; }
        public int MucHuongBaoHiem { get; set; }
        public decimal DonGiaBHYTThanhToan => DonGiaBHYT * TiLeBaoHiemThanhToan / 100 * MucHuongBaoHiem / 100;
        public decimal BHYTThanhToan => (DuocHuongBHYT && BaoHiemChiTra == true) ? (decimal)(SoLuong * (double)DonGiaBHYTThanhToan) : 0;

        //public decimal? ChiPhiThucTe =>
        //    LaThuTheoChiPhi 
        //        ? TongTienMatThuTheoChiPhi 
        //        : (ChiPhi == null 
        //            ? (decimal?)null 
        //            : ((ChiPhi.GetValueOrDefault() - BHYTThanhToan) < 0 ? (decimal?)null : (ChiPhi.GetValueOrDefault() - BHYTThanhToan)));

        public decimal? ChiPhiThucTe
        {
            get
            {
                if (LaThuTheoChiPhi)
                {
                    return TongTienMatThuTheoChiPhi;
                }
                else if (LaTuTinhGiaBan)
                {
                    var giaBanThucTe = (GiaBan == null || KhongTinhPhi.GetValueOrDefault())
                        ? (decimal?)null
                        : (GiaBan.GetValueOrDefault() - SoTienMienGiam.GetValueOrDefault());

                    ChiPhi = giaBanThucTe;
                }

                //else
                {
                    return (ChiPhi == null
                                ? (decimal?)null
                                : ((ChiPhi.GetValueOrDefault() - BHYTThanhToan) < 0 ? (decimal?)null : (ChiPhi.GetValueOrDefault() - BHYTThanhToan)));
                }
            }
        }

        //dùng cho trường hợp tính toán tiền mặt cho loại thu tiền theo chi phí
        //tham khảo từ BaoCaoThuPhiVienPhiGridVo
        public bool LaThuTheoChiPhi { get; set; }
        public decimal? TongChiPhi => PhieuChis.Any() ? PhieuChis.Where(x => x.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi).Sum(a => a.TienChiPhi ?? 0) : (decimal?) null;
        public decimal? CongNo { get; set; }
        public decimal? TienChuyenKhoan { get; set; }
        public decimal? TienPOS { get; set; }
        public decimal TongTienMatThuTheoChiPhi => LaThuTheoChiPhi ? (TongChiPhi.GetValueOrDefault() - CongNo.GetValueOrDefault()) : 0;

        public List<ThongTinPhieuChiVo> PhieuChis { get; set; }

        //BVHD-3882: Cập nhật cách tính giá bán dược phẩm và vật tư
        public bool? KhongTinhPhi { get; set; }
        public long? XuatKhoChiTietId { get; set; }
        public bool LaTuTinhGiaBan { get; set; }
        public decimal? SoTienMienGiam { get; set; }
        public decimal? DonGiaNhap { get; set; }
        public int? VAT { get; set; }
        public int? TiLeTheoThapGia { get; set; }
        public Enums.PhuongPhapTinhGiaTriTonKho PhuongPhapTinhGiaTriTonKho => PhuongPhapTinhGiaTriTonKhos.Any() ? PhuongPhapTinhGiaTriTonKhos.First() : Enums.PhuongPhapTinhGiaTriTonKho.ApVAT;
        public decimal? GiaTonKho => DonGiaNhap.GetValueOrDefault() + (DonGiaNhap.GetValueOrDefault() * (PhuongPhapTinhGiaTriTonKho == Enums.PhuongPhapTinhGiaTriTonKho.ApVAT ? VAT.GetValueOrDefault() : 0) / 100);
        public decimal? DonGiaBan => Math.Round(GiaTonKho.GetValueOrDefault() + (GiaTonKho.GetValueOrDefault() * TiLeTheoThapGia.GetValueOrDefault() / 100), 2, MidpointRounding.AwayFromZero);
        public decimal? GiaBan => Math.Round((decimal)SoLuong * DonGiaBan.GetValueOrDefault(), 2, MidpointRounding.AwayFromZero);
        public List<Enums.PhuongPhapTinhGiaTriTonKho> PhuongPhapTinhGiaTriTonKhos { get; set; }

        // get thông tin chi phí BHYT cho trường hợp dịch vụ giường
        public List<ThongTinChiPhiBHYTDichVuVo> ChiPhiBHYTDichVuGiuongs { get; set; }
    }

    public class ThongTinPhieuChiVo
    {
        public Enums.LoaiChiTienBenhNhan LoaiChiTienBenhNhan { get; set; }
        public decimal? TienChiPhi { get; set; }
    }

    public class PhuongPhapTinhGiaTriTonKhoTheoXuatVo
    {
        public long XuatKhoChiTietId { get; set; }
        public Enums.PhuongPhapTinhGiaTriTonKho PhuongPhapTinhGiaTriTonKho { get; set; }
    }

    public class ChiPhiThuePhongVo
    {
        public long YeuCauDichVuKyThuatThuePhongId { get; set; }
        public long YeuCauDichVuKyThuatChiPhiThuePhongId { get; set; }
        public decimal? GiaThuePhong { get; set; }
        public DateTime? BatDauThuePhong { get; set; }
        public DateTime? KetThucThuePhong { get; set; }
    }

    public class MaTiepNhanTheoHinhThucDenQueryInfoVo
    {
        public long? HinhThucDenId { get; set; }
        public long? NoiGioiThieuId { get; set; }
    }

    public class MaTiepNhanTheoHinhThucDenLookupItemVo : LookupItemTextVo
    {
        public string MaYeuCauTiepNhan { get; set; }
        public string MaNguoiBenh { get; set; }
        public string TenNguoiBenh { get; set; }
    }

    public class ThongTinNguoiBenhChiVo
    {
        public long YeuCauTiepNhanId { get; set; }
        public decimal? ChiPhi { get; set; }
    }

    public class ThongTinChiPhiBHYTDichVuVo
    {
        public bool DuocHuongBaoHiem { get; set; }
        public bool? BaoHiemChiTra { get; set; }
        public decimal? DonGiaBaoHiem { get; set; }
        public int? MucHuongBaoHiem { get; set; }
        public int? TiLeBaoHiemThanhToan { get; set; }
    }
}
