using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Core.Helpers;
using Newtonsoft.Json;

namespace Camino.Core.Domain.ValueObject.DichVuKhuyenMai
{
    public class TemplateStringIdDichVuKhuyenMaiTrongGoiVo
    {
        public long YeuCauGoiDichVuId { get; set; }
        public long ChuongTrinhGoiDichVuId { get; set; }
        public long ChuongTrinhGoiDichVuChiTietId { get; set; }
        public int NhomGoiDichVu { get; set; }
        public string TenDichVu { get; set; }
        public long DichVuBenhVienId { get; set; }
        public string TenGoiDichVu { get; set; }
    }

    public class ChiTietGoiDichVuKhuyenMaiTheoBenhNhanGridVo
    {
        public int STT { get; set; }

        public string Id
        {
            get
            {
                var newObj = new TemplateStringIdDichVuKhuyenMaiTrongGoiVo()
                {
                    YeuCauGoiDichVuId = YeuCauGoiDichVuId,
                    ChuongTrinhGoiDichVuId = ChuongTrinhGoiDichVuId,
                    ChuongTrinhGoiDichVuChiTietId = ChuongTrinhGoiDichVuChiTietId,
                    NhomGoiDichVu = (int) NhomGoiDichVu,
                    TenDichVu = TenDichVu == null ? TenDichVu : TenDichVu.Trim(),
                    DichVuBenhVienId = DichVuBenhVienId,
                    TenGoiDichVu = TenGoiDichVu
                };
                return JsonConvert.SerializeObject(newObj);
            }
        }

        public string TenGoiDichVu { get; set; }
        public long YeuCauGoiDichVuId { get; set; }
        public long ChuongTrinhGoiDichVuId { get; set; }
        public long ChuongTrinhGoiDichVuChiTietId { get; set; }
        public long DichVuBenhVienId { get; set; }
        public string MaDichVu { get; set; }
        public string TenDichVu { get; set; }
        public Enums.EnumNhomGoiDichVu NhomGoiDichVu { get; set; }
        public string TenNhomGoiDichVu => NhomGoiDichVu.GetDescription();
        public int NhomId => (int) NhomGoiDichVu;
        public string TenLoaiGia { get; set; }
        public long NhomGiaId { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal DonGiaKhuyenMai { get; set; }
        public DateTime? HanSuDung { get; set; }
        public string HanSuDungDisplay => HanSuDung?.ApplyFormatDate();

        public decimal ThanhTien => SoLuong * DonGia;
        public double SoLuongDaDung { get; set; }
        public double SoLuongDungLanNay { get; set; }
        public double SoLuongConLai => SoLuong - SoLuongDaDung;
        public bool IsPTTT { get; set; }
        public bool IsDieuTriNoiTru { get; set; }
        public bool IsNhomTiemChung { get; set; }
        public Enums.LoaiDichVuKyThuat LoaiDichVuKyThuat { get; set; }
        public bool IsChecked { get; set; }
        //public bool IsDisabled { get; set; }
        public bool IsDichVuGiuong { get; set; }
        public bool IsHetHanSuDung => HanSuDung != null && HanSuDung.Value.Date < DateTime.Now.Date;
        public bool IsGoiDaQuyetToan { get; set; }
        public bool IsDanhSachVacxin { get; set; }
        public bool IsActive => SoLuongConLai > 0
                                //&& !IsDisabled 
                                && (!IsPTTT || (IsPTTT && NhomGoiDichVu == Enums.EnumNhomGoiDichVu.DichVuKyThuat))
                                && (!IsDieuTriNoiTru || (IsDieuTriNoiTru 
                                                         && ((NhomGoiDichVu == Enums.EnumNhomGoiDichVu.DichVuKyThuat && LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.SuatAn)
                                                            //BVHD-3575: cập nhật cho phép chỉ định dv khám từ nội trú
                                                            || NhomGoiDichVu == Enums.EnumNhomGoiDichVu.DichVuKhamBenh)))
                                && !IsDichVuGiuong
                                && ((IsDanhSachVacxin && IsNhomTiemChung) || (!IsDanhSachVacxin && !IsNhomTiemChung))
                                && !IsHetHanSuDung
                                && !IsGoiDaQuyetToan;
    }

    public class ChiTietGoiDichVuKhuyenMaiChiDinhTheoBenhNhanVo
    {
        public string Id { get; set; }
        public long YeuCauGoiDichVuId { get; set; }
        public string TenGoiDichVu { get; set; }
        public long ChuongTrinhGoiDichVuId { get; set; }
        public long ChuongTrinhGoiDichVuChiTietId { get; set; }
        public long DichVuBenhVienId { get; set; }
        public string TenDichVu { get; set; }
        public int NhomGoiDichVu { get; set; }
        public int SoLuongSuDung { get; set; }
        public bool IsActive { get; set; }

        public Enums.ViTriTiem? ViTriTiem { get; set; }
        public int? MuiSo { get; set; }
        public long? NoiThucHienId { get; set; }
        public string LieuLuong { get; set; }
    }

    public class GridDataSourceDichVuKhuyenMai
    {
        public ICollection<ChiTietGoiDichVuKhuyenMaiTheoBenhNhanGridVo> Data { get; set; }

        public int TotalRowCount { get; set; }
    }

    public class ChiDinhGoiDichVuKhuyenMaiTheoBenhNhanDichVuLoiVo : ChiDinhGoiDichVuThuongDungDichVuLoiVo
    {
        public long YeuCauGoiDichVuId { get; set; }
        public long ChuongTrinhGoiDichVuId { get; set; }
        public long ChuongTrinhGoiDichVuChiTietId { get; set; }
        public int NhomGoiDichVuValue { get; set; }
    }


    public class ChiDinhGoiDichVuKhuyenMaiTheoBenhNhanVo
    {
        public ChiDinhGoiDichVuKhuyenMaiTheoBenhNhanVo()
        {
            DichVus = new List<ChiTietGoiDichVuKhuyenMaiChiDinhTheoBenhNhanVo>();
            DichVuKhongThems = new List<ChiDinhGoiDichVuKhuyenMaiTheoBenhNhanDichVuLoiVo>();
            YeuCauDichVuGiuongBenhVienNews = new List<YeuCauDichVuGiuongBenhVien>();
            YeuCauDichVuKyThuatNews = new List<YeuCauDichVuKyThuat>();
            YeuCauKhamBenhNews = new List<Entities.YeuCauKhamBenhs.YeuCauKhamBenh>();
        }
        public long YeuCauTiepNhanId { get; set; }
        public long? YeuCauKhamBenhId { get; set; }
        public long? NoiTruPhieuDieuTriId { get; set; }
        public bool IsKhamBenhDangKham { get; set; }
        public bool ISPTTT { get; set; }
        public bool? IsVuotQuaBaoLanhGoi { get; set; }
        public List<ChiTietGoiDichVuKhuyenMaiChiDinhTheoBenhNhanVo> DichVus { get; set; }
        public List<ChiDinhGoiDichVuKhuyenMaiTheoBenhNhanDichVuLoiVo> DichVuKhongThems { get; set; }
        public List<Entities.YeuCauKhamBenhs.YeuCauKhamBenh> YeuCauKhamBenhNews { get; set; }
        public List<YeuCauDichVuKyThuat> YeuCauDichVuKyThuatNews { get; set; }
        public List<YeuCauDichVuGiuongBenhVien> YeuCauDichVuGiuongBenhVienNews { get; set; }

        //BVHD-3825
        public bool DichVuKhamKhongHuongBHYT { get; set; }
        public bool DichVuKyThuatKhongHuongBHYT { get; set; }

        // dùng cho YCTN
        public bool LaThemTam { get; set; }
        public bool? DuocHuongBaoHiemTam { get; set; }
        public ChiDinhDichVuTrongNhomThuongDungVo DichVuKhuyenMaiThemTamTuTiepNhan { get; set; }

        // dùng cho TiemVacxin
        public bool IsVacxin { get; set; }
    }

    public class ChiDinhDichVuKhuyenMaiResultVo
    {
        public Enums.EnumTrangThaiYeuCauKhamBenh? TrangThaiYeuCauKhamBenh { get; set; }
        public bool ChuyenHangDoiSangLamChiDinh { get; set; }
        public decimal SoDuTaiKhoan { get; set; }
        public string SoDuTaiKhoanDisplay
        {
            get { return SoDuTaiKhoan.ApplyFormatMoneyVND(); }
        }

        public decimal SoDuTaiKhoanConLai { get; set; }
        public string SoDuTaiKhoanConLaiDisplay
        {
            get { return SoDuTaiKhoanConLai.ApplyFormatMoneyVND(); }
        }
        public bool IsVuotQuaSoDuTaiKhoan { get; set; }
        public bool? IsVuotQuaBaoLanhGoi { get; set; }
        public byte[] LastModified { get; set; }
    }

    public class DichVuBenhVienKhuyenMaiTheoGoiDichVuVo
    {
        public Enums.EnumNhomGoiDichVu NhomGoiDichVu { get; set; }
        public bool LaSuatAn { get; set; }
        public long DichVuBenhVienId { get; set; }
        public long NhomGiaDichVuBenhVienId { get; set; }
        public string TenNhomGiaDichVuBenhVien { get; set; }
        public decimal? DonGiaBenhVien { get; set; }
        public decimal? DonGiaBaoHiem { get; set; }
        public decimal? DonGiaKhuyenMai { get; set; }
        public long NoiThucHienId { get; set; }
        public long? BacSiDangKyId { get; set; }
        public string MaDichVu { get; set; }
        public string TenDichVu { get; set; }
        public string MaGiaDichVu { get; set; }
        public string TenGia { get; set; }
        public string Ma4350 { get; set; }
        public Enums.EnumDanhMucNhomTheoChiPhi NhomChiPhiDichVuKyThuat { get; set; }
        public int SoLuong { get; set; }
        public int NhomChiPhiDichVuKyThuatId { get; set; }
        public long NhomDichVuBenhVienId { get; set; }
        public int? TiLeBaoHiemThanhToan { get; set; }
        public bool CoBHYT { get; set; }
        public bool DuocHuongBaoHiem
        {
            get { return CoBHYT && DonGiaBaoHiem > 0; }
        }
        public int SoLanTheoGoi { get; set; }
        public int SoLanDaSuDung { get; set; }
        public int SoLanConLai => SoLanTheoGoi - SoLanDaSuDung;
        public long YeuCauGoiDichVuId { get; set; }

        public decimal ThanhTien => (decimal)SoLuong * (DonGiaBenhVien ?? 0);
        public decimal ThanhTienKM => (decimal)SoLuong * (DonGiaKhuyenMai ?? 0);
        public decimal SoTienMG => (ThanhTien - ThanhTienKM) > 0 ? (ThanhTien - ThanhTienKM) : 0;

        public DateTime HanSuDung { get; set; }
        public bool ConHanSuDung => HanSuDung.Date >= DateTime.Now.Date;
        public bool DaQuyetToan { get; set; }
        public string TenGoiDichVu { get; set; }

        public Enums.ViTriTiem? ViTriTiem { get; set; }
        public int? MuiSo { get; set; }
        public string LieuLuong { get; set; }
    }
}
