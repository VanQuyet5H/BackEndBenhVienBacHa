using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Newtonsoft.Json;

namespace Camino.Core.Domain.ValueObject.KhamBenhs
{
    public class NhomGoiDichVuThuongDungGridVo : GridItem
    {
        public string TenNhom { get; set; }
        public string MoTa { get; set; }
    }

    public class ChiTietNhomGoiDichVuThuongDungGridVo: GridItem
    {
        public long GoiDichVuId { get; set; }
        public string TenGoiDichVu { get; set; }
        public string TenNhomDichVu { get; set; }
        public Enums.EnumNhomGoiDichVu NhomDichVu { get; set; }
        public bool LaSuatAn { get; set; }
        public string MaDichVu { get; set; }
        public string TenDichVu { get; set; }
        public string LoaiGia { get; set; }
        public decimal? DonGia { get; set; }
        public int SoLan { get; set; }
        public decimal ThanhTien => DonGia * SoLan ?? 0;
    }

    public class ChiTietNhomGoiDichVuThuongDungDangChonVo
    {
        public Enums.EnumNhomGoiDichVu NhomDichVu { get; set; }
        public long DichVuId { get; set; }
        public string TenDichVu { get; set; }
        public int SoLanChon { get; set; }
    }

    public class YeuCauThemGoiDichVuThuongDungVo
    {
        public YeuCauThemGoiDichVuThuongDungVo()
        {
            GoiDichVuIds = new List<long>();
            DichVuKhongThems = new List<ChiDinhGoiDichVuThuongDungDichVuLoiVo>();
            YeuCauKhamBenhNews = new List<Entities.YeuCauKhamBenhs.YeuCauKhamBenh>();
            YeuCauDichVuKyThuatNews = new List<YeuCauDichVuKyThuat>();
            YeuCauDichVuGiuongBenhVienNews = new List<YeuCauDichVuGiuongBenhVien>();
        }
        public long YeuCauTiepNhanId { get; set; }
        public long YeuCauKhamBenhId { get; set; }
        public bool? DuocHuongBaoHiem { get; set; }
        public bool? LaPhauThuatThuThuat { get; set; }
        public long? PhieuDieuTriId { get; set; }
        public List<long> GoiDichVuIds { get; set; }
        public List<ChiDinhGoiDichVuThuongDungDichVuLoiVo> DichVuKhongThems { get; set; }
        public List<Entities.YeuCauKhamBenhs.YeuCauKhamBenh> YeuCauKhamBenhNews { get; set; }
        public List<YeuCauDichVuKyThuat> YeuCauDichVuKyThuatNews { get; set; }
        public List<YeuCauDichVuGiuongBenhVien> YeuCauDichVuGiuongBenhVienNews { get; set; }

        //BVHD-3575
        // dùng cho trường hợp chỉ định dv khám từ nội trú
        public bool? TiepNhanNgoaiTruCoBHYT { get; set; }
    }

    public class DichVuBenhVienTheoGoiDichVuVo
    {
        public Enums.EnumNhomGoiDichVu NhomGoiDichVu { get; set; }
        public bool LaSuatAn { get; set; }
        public long DichVuBenhVienId { get; set; }
        public long NhomGiaDichVuBenhVienId { get; set; }
        public string TenNhomGiaDichVuBenhVien { get; set; }
        public decimal? DonGiaBenhVien { get; set; }
        public decimal? DonGiaBaoHiem { get; set; }
        public decimal? DonGiaTruocChietKhau { get; set; }
        public decimal? DonGiaSauChietKhau { get; set; }
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
        public  int? TiLeBaoHiemThanhToan { get; set; }
        public bool CoBHYT { get; set; }
        public bool DuocHuongBaoHiem
        {
            get { return CoBHYT && DonGiaBaoHiem > 0; }
        }
        public int SoLanTheoGoi { get; set; }
        public int SoLanDaSuDung { get; set; }
        public int SoLanConLai => SoLanTheoGoi - SoLanDaSuDung;
        public long YeuCauGoiDichVuId { get; set; }
        public DateTime HanSuDung { get; set; }
        public bool ConHanSuDung => HanSuDung.Date >= DateTime.Now.Date;

        //BVHD-3575
        public long? NoiDangKyId { get; set; }
    }

    public class GoiDichVuTheoBenhNhanGridVo : GridItem
    {
        public string TenGoiDichVu { get; set; }
        public decimal TongCong { get; set; }
        public double ChietKhau => (double)((TongCong - GiaGoi) / TongCong * 100);
        public decimal GiaGoi { get; set; }
        public decimal ChuaThu => GiaGoi - BenhNhanDaThanhToan;
        public string ChuaThuDisplay => ChuaThu.ApplyFormatMoneyVND();
        public decimal BenhNhanDaThanhToan { get; set; }
        public string BenhNhanDaThanhToanDisplay => BenhNhanDaThanhToan.ApplyFormatMoneyVND();
        public decimal DangDung { get; set; }
        public decimal ConLai => BenhNhanDaThanhToan - DangDung;
        public string ConLaiDisplay => ConLai.ApplyFormatMoneyVND();

        //BVHD-3268
        public decimal? GiaGoiDichVuTiemChung { get; set; }
    }


    public class GridDataSourceChiTietGoiDichVuTheoBenhNhan
    {
        public ICollection<ChiTietGoiDichVuTheoBenhNhanGridVo> Data { get; set; }

        public int TotalRowCount { get; set; }
    }

    public class TemplateStringIdDichVuTrongGoiVo
    {
        public long YeuCauGoiDichVuId { get; set; }
        public long ChuongTrinhGoiDichVuId { get; set; }
        public long ChuongTrinhGoiDichVuChiTietId { get; set; }
        public int NhomGoiDichVu { get; set; }
        public string TenDichVu { get; set; }
        public long DichVuBenhVienId { get; set; }
        public string TenGoiDichVu { get; set; }
    }

    public class ChiTietGoiDichVuTheoBenhNhanGridVo
    {
        public int STT { get; set; }

        public string Id
        {
            get
            {
                //string templateKeyId = "\"YeuCauGoiDichVuId\":{0},\"ChuongTrinhGoiDichVuId\":{1},\"ChuongTrinhGoiDichVuChiTietId\":\"{2}\",\"NhomGoiDichVu\":{3},\"TenDichVu\":\"{4}\",\"DichVuBenhVienId\":\"{5}\",\"TenGoiDichVu\":\"{6}\"";
                //return "{" + string.Format(templateKeyId, YeuCauGoiDichVuId, ChuongTrinhGoiDichVuId, ChuongTrinhGoiDichVuChiTietId, (int)NhomGoiDichVu, TenDichVu == null ? TenDichVu : TenDichVu.Trim(), DichVuBenhVienId, TenGoiDichVu) + "}";

                var newObj = new TemplateStringIdDichVuTrongGoiVo()
                {
                    YeuCauGoiDichVuId = YeuCauGoiDichVuId,
                    ChuongTrinhGoiDichVuId = ChuongTrinhGoiDichVuId,
                    ChuongTrinhGoiDichVuChiTietId = ChuongTrinhGoiDichVuChiTietId,
                    NhomGoiDichVu = (int)NhomGoiDichVu,
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
        public string TenLoaiGia { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
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
        public bool IsActive => SoLuongConLai > 0
                                //&& !IsDisabled 
                                && (!IsPTTT || (IsPTTT && NhomGoiDichVu == Enums.EnumNhomGoiDichVu.DichVuKyThuat)) //(LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem || LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh || LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang)))
                                && (!IsDieuTriNoiTru || (IsDieuTriNoiTru 
                                                         && ((NhomGoiDichVu == Enums.EnumNhomGoiDichVu.DichVuKyThuat && LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.SuatAn)
                                                                //BVHD0-3575: cập nhật cho phép nội trú chỉ định dv khám
                                                                || NhomGoiDichVu == Enums.EnumNhomGoiDichVu.DichVuKhamBenh)))
                                && !IsDichVuGiuong
                                && !IsNhomTiemChung;
    }

    public class ChiDinhGoiDichVuTheoBenhNhanVo
    {
        public ChiDinhGoiDichVuTheoBenhNhanVo()
        {
            DichVus = new List<ChiTietGoiDichVuChiDinhTheoBenhNhanVo>();
            DichVuKhongThems = new List<ChiDinhGoiDichVuTheoBenhNhanDichVuLoiVo>();
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
        public List<ChiTietGoiDichVuChiDinhTheoBenhNhanVo> DichVus { get; set; }
        public List<ChiDinhGoiDichVuTheoBenhNhanDichVuLoiVo> DichVuKhongThems { get; set; }
        public List<Entities.YeuCauKhamBenhs.YeuCauKhamBenh> YeuCauKhamBenhNews { get; set; }
        public List<YeuCauDichVuKyThuat> YeuCauDichVuKyThuatNews { get; set; }
        public List<YeuCauDichVuGiuongBenhVien> YeuCauDichVuGiuongBenhVienNews { get; set; }

        //BVHD-3575
        public DateTime? NgayDieuTri { get; set; }
    }

    public class ChiTietGoiDichVuChiDinhTheoBenhNhanVo
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

        //BVHD-3575
        public long? NoiDangKyId { get; set; }
    }

    public class ChiDinhGoiDichVuThuongDungDichVuLoiVo
    {
        public ChiDinhGoiDichVuThuongDungDichVuLoiVo()
        {
            LoaiLois = new List<string>();
        }
        public long GoiDichVuId { get; set; }
        public Enums.EnumNhomGoiDichVu NhomGoiDichVu { get; set; }
        public string TenGoiDichVu { get; set; }
        public long DichVuId { get; set; }
        public string TenDichVu { get; set; }
        public List<string> LoaiLois { get; set; }
        public string TenLoi => string.Join(", ", LoaiLois.Distinct().ToList());
        public bool IsDisabled { get; set; }
        public bool KhongThem { get; set; }
    }

    public class ChiDinhGoiDichVuTheoBenhNhanDichVuLoiVo : ChiDinhGoiDichVuThuongDungDichVuLoiVo
    {
        public long YeuCauGoiDichVuId { get; set; }
        public long ChuongTrinhGoiDichVuId { get; set; }
        public long ChuongTrinhGoiDichVuChiTietId { get; set; }
        public int NhomGoiDichVuValue { get; set; }
    }

    public class DichVuChiDinhCoTrongGoiCuaBenhNhanVo{
        public DichVuChiDinhCoTrongGoiCuaBenhNhanVo()
        {
            DichVuKhamBenhIds = new List<long>();
            DichVuKyThuatIds = new List<long>();
            DichVuChiDinhCoTrongGois = new List<ChiTietGoiDichVuTheoBenhNhanGridVo>();
        }
        public long BenhNhanId { get; set; }
        public List<long> DichVuKhamBenhIds { get; set; }
        public List<long> DichVuKyThuatIds { get; set; }
        public string Message { get; set; }
        public List<ChiTietGoiDichVuTheoBenhNhanGridVo> DichVuChiDinhCoTrongGois { get; set; }
    }

    public class DichVuDaChonYCTNVo{
        public string Nhom { get; set; }
        public string TenDichVu { get; set; }
        public long MaDichVuId { get; set; }
    }


    public class ChiTietSuDungDichVuGiuongTrongGoiTheoBenhNhanVo
    {
        public long YeuCauGoiDichVuId { get; set; }
        public long DichVuBenhVienId { get; set; }
        public long NhomGiaDichVuBenhVienId { get; set; }
        public double SoLuongDaSuDung { get; set; }
    }

    public class ThongTinSuDungDichVuTronGoiVo
    {
        public long YeuCauGoiDichVuId { get; set; }
        public long DichVuBenhVienId { get; set; }
        public int SoLan { get; set; }
    }

    //BVHD-3575
    public class DichVuTrongGoiDaDungVo
    {
        public long YeuCauGoiDichVuId { get; set; }
        public long DichVuBenhVienId { get; set; }
        public long NhomGiaId { get; set; }
        public int SoLuong { get; set; }
        public decimal? DonGiaSauChietKhau { get; set; }
        public decimal? ThanhTienSauChietKhau => SoLuong * (DonGiaSauChietKhau ?? 0);

        // Cập nhật 15/12/2022
        public long? YeuCauDichVuId { get; set; }
    }
}
