using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.YeuCauTiepNhans
{
    public class BenhNhanChoKhamGridVo : GridItem
    {
        public long YeuCauTiepNhanId { get; set; }
        public long? YeuCauKhamBenhId { get; set; }
        public int SoThuTu { get; set; }
        public string MaYeuCauTiepNhan { get; set; }
        public string MaBN { get; set; }
        public string HoTen { get; set; }
        public Enums.LoaiGioiTinh GioiTinh { get; set; }
        public string TenGioiTinh { get; set; }
        public int Tuoi { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public string NgaySinhDisplay => DateHelper.DOBFormat(NgaySinh, ThangSinh, NamSinh);
        public string DiaChi { get; set; }
        public string LyDoKham { get; set; }
        public string TenNhomMau { get; set; }
        public Enums.EnumNhomMau? NhomMau { get; set; }
        public int? Mach { get; set; }
        public int? NhipTho { get; set; }
        public double? CanNang { get; set; }
        public int? HuyetAp { get; set; }
        public string HuyetApDisplay { get; set; }
        public double? NhietDo { get; set; }
        public double? ChieuCao { get; set; }
        public double? BMI { get; set; }
        public string TinhTrang { get; set; }
        public bool? ProgessChiSoSinhTon { get; set; }
        public byte[] YeuCauKhamBenhLastModified { get; set; }

        public string HighLightClass { get; set; }
        public bool CoBaoHiem { get; set; }

        public long PhongBenhVienId { get; set; }
        public Enums.EnumTrangThaiHangDoi TrangThai { get; set; }
        public Enums.EnumTrangThaiYeuCauTiepNhan TrangThaiYeuCauTiepNhan { get; set; }
        public DateTime ThoiDiemTiepNhan { get; set; }
        public Enums.EnumLoaiHangDoi LoaiHangDoi { get; set; }
        public ICollection<YeuCauDichVuKyThuat> YeuCauDichVuKyThuats { get; set; }
        public DateTime NgayKhamBenh { get; set; }

        // khám đoàn
        public int DichVuKhamDaThucHien { get; set; }
        public int TongDichVuKham { get; set; }
        public string TienTrinhKhamSucKhoe => DichVuKhamDaThucHien + "/" + TongDichVuKham;
        public bool IsKhamDuDichVuKhamSucKhoe => DichVuKhamDaThucHien == TongDichVuKham;
        public long? HopDongKhamSucKhoeNhanVienId { get; set; }

        //BVHD-3751
        public long? NoiDangKyId { get; set; }
    }

    public class BenhNhanDoiKetLuanGridVo : GridItem
    {
        public string MaTN { get; set; }
        public string HoTen { get; set; }
        public Enums.LoaiGioiTinh GioiTinh { get; set; }
        public string TenGioiTinh { get; set; }
        public int Tuoi { get; set; }
        public string LyoKham { get; set; }
    }

    public class SoLuongYeuCauHienTaiVo
    {
        public int ChuanBiKham { get; set; }
        public int DangLamChiDinh { get; set; }
        public int DangDoiKetLuan { get; set; }
    }

    public class ThongTinChungCuaBenhNhan : GridItem
    {
        public string MaTN { get; set; }
        public string MaBN { get; set; }
        public string HoTenBenhNhan { get; set; }
        public string SinhNgay { get; set; }
        public string Tuoi { get; set; }
        public string GioiTinh { get; set; }
        public string NgheNghiep { get; set; }
        public string DanToc { get; set; }
        public string QuocTich { get; set; }
        public string SoNha { get; set; }
        public string DiaChiDayDu { get; set; }
        public string ThonPho { get; set; }
        public string XaPhuong { get; set; }
        public string Huyen { get; set; }
        public string TinhThanhPho { get; set; }
        public string NoiLamViec { get; set; }
        public string DoiTuong { get; set; }
        public string BHYTNgayHetHan { get; set; }
        public string BHYTMaSoThe { get; set; }
        public string BHYTMaSoThe2 { get; set; }
        public string NguoiLienHeQuanHeThanNhan { get; set; }
        public string NguoiLienHeDiaChiDayDu { get; set; }
        public string SoDienThoai { get; set; }
        public string NguoiLienHeQuanSoDienThoai { get; set; }
        public string ThoiDiemTiepNhan { get; set; }
        public string ThoiDiemTiepNhanFormat { get; set; }
        public string ChanDoanNoiGioiThieu { get; set; }
        public string Ngay { get; set; }
        public string Thang { get; set; }
        public string Nam { get; set; }
        public string HoTenBacSi { get; set; }
        public string BenhSu { get; set; }


        //Cập nhật 22/06/2022 -> yêu cầu phiếu khám và phiếu vào viện hiển thị số điện thoại từ 2 nguồn trong YCTN: SDT và SDT người liên hệ
        public string SoDienThoaiNguoiBenh { get; set; }
        public string SoDienThoaiNguoiBenhDisplay
        {
            get
            {
                var lstSoDienThoai = new List<string>();
                lstSoDienThoai.Add(SoDienThoaiNguoiBenh);
                lstSoDienThoai.Add(NguoiLienHeQuanSoDienThoai);
                return string.Join(", ", lstSoDienThoai.Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList());
                //return (SoDienThoaiNguoiBenh ?? "") + ((!string.IsNullOrEmpty(SoDienThoaiNguoiBenh) && !string.IsNullOrEmpty(NguoiLienHeQuanSoDienThoai)) ? ", " : "") + (NguoiLienHeQuanSoDienThoai ?? "");
            }
        }
    }

    //Khám mắt
    public class ThongTinBenhMatChiTietList
    {
        public List<ThongTinBenhMatChiTietValue> DataKhamTheoTemplate { get; set; }
    }
    public class ThongTinBenhMatChiTietValue
    {
        public string Id { get; set; }
        public string Value { get; set; }
    }
    public class ThongTinBenhMatChiTiet
    {
        public string ThiLucKhongKinh { get; set; }
        public string NhanAp { get; set; }
        public string ThiLucCoKinh { get; set; }
        public string MPKK { get; set; }
        public string MPNA { get; set; }
        public string MTKK { get; set; }
        public string MTNA { get; set; }
        public string MPCK { get; set; }
        public string MTCK { get; set; }
        public string SoiHienVi { get; set; }
        public string SoiDayMat { get; set; }
        public string KhamMatNoiDung { get; set; }


    }
    public class ThongTinBenhRHMChiTiet
    {
        public string Rang { get; set; }
        public string Ham { get; set; }
        public string Mat { get; set; }
    }
    //Khám tai mũi họng
    public class ThongTinBenhNhanTaiMuiHongList
    {
        public List<ThongTinBenhNhanTaiMuiHongValue> DataKhamTheoTemplate { get; set; }
    }
    public class ThongTinBenhNhanTaiMuiHongValue
    {
        public string Id { get; set; }
        public string Value { get; set; }
    }
    public class ThongTinBenhNhanTaiMuiHongChiTiet
    {
        public string Tai { get; set; }
        public string Mui { get; set; }
        public string Hong { get; set; }
    }

    //Khám khác
    public class ThongTinBenhNhanKhamKhacList
    {
        public List<ThongTinBenhNhanKhamKhacValue> DataKhamTheoTemplate { get; set; }
    }
    public class ThongTinBenhNhanKhamKhacValue
    {
        public string Id { get; set; }
        public string Value { get; set; }
        public string fxFlex { get; set; }
    }
    public class ThongTinViecLamBenNhan
    {
        public string CongViec { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
    }
    public class ThongTinBenhNhanKhamKhacTemplateList
    {
        public List<ThongTinBenhNhanKhamKhacTemplate> ComponentDynamics { get; set; }
    }
    public class ThongTinBenhNhanKhamKhacTemplate
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public int Type { get; set; }
        public List<ThongTinBenhNhanKhamKhacTemplateGroupItems> groupItems { get; set; }
    }

    public class ThongTinBenhNhanKhamKhacTemplateGroupItems
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public int Type { get; set; }
    }

    public class ThongTinBenhNhanKhamKhacChiTiet
    {
        public string TuanHoan { get; set; }
        public string HoHap { get; set; }
        public string TieuHoa { get; set; }
        public string ThanTietNieuSinhDuc { get; set; }
        public string ThanKinh { get; set; }
        public string CoXuongKhop { get; set; }
        public string RangHamMat { get; set; }
        public string NoiTietDinhDuong { get; set; }
        public string SanPhuKhoa { get; set; }
        public string DaLieu { get; set; }
    }

    public class ThongTinBenhNhanKhamTMHLabel
    {
        public string Tai { get; set; }
        public string Mui { get; set; }
        public string Hong { get; set; }
    }

    public class ThongTinBenhMat : GridItem
    {
        public string HeaderMat { get; set; }
        public string MaBN { get; set; }
        public string HoTenBenhNhan { get; set; }
        public string SinhNgay { get; set; }
        public string GioiTinh { get; set; }
        public string NgheNghiep { get; set; }
        public string DanToc { get; set; }
        public string QuocTich { get; set; }
        public string SoNha { get; set; }
        public string DiaChiDayDu { get; set; }
        public string XaPhuong { get; set; }
        public string Huyen { get; set; }
        public string TinhThanhPho { get; set; }
        public string NoiLamViec { get; set; }
        public string DoiTuong { get; set; }
        public string BHYTNgayHetHan { get; set; }
        public string BHYTMaSoThe { get; set; }
        public string NguoiLienHeQuanHeThanNhan { get; set; }
        public string SoDienThoaiQHTN { get; set; }
        public string ThoiDiemTiepNhan { get; set; }
        public string ChanDoanNoiGioiThieu { get; set; }
        public string TienSuDiUng { get; set; }
        public string TienSuBenh { get; set; }
        public string BarCodeImgBase64 { get; set; }
        public string PhongKham { get; set; }
        public string HoTenBacSi { get; set; }
        public string LyDoVaoKham { get; set; }
        public string LyDoVaoVien { get; set; }
        public string QuaTrinhBenhLy { get; set; }
        public string HuongXuLi { get; set; }
        public string ToanThan
        {
            get
            {
                var result = string.Empty;
                if (!string.IsNullOrEmpty(NoiDungKhamBenh))
                {
                    result += NoiDungKhamBenh.Replace("\n", "<br>") + "; ";
                }
                if (!string.IsNullOrEmpty(KhamToanThan))
                {
                    result += KhamToanThan.Replace("\n", "<br>") + "; ";
                }
                return result;
            }
        }
        public string NoiDungKhamBenh { get; set; }
        public string KhamToanThan { get; set; }
        public string MPKK { get; set; }
        public string MPNA { get; set; }
        public string MTKK { get; set; }
        public string MTNA { get; set; }
        public string MPCK { get; set; }
        public string MTCK { get; set; }
        public string TomTatCLS { get; set; }
        public string ChanDoanVaoVien { get; set; }
        public string DaXuLi { get; set; }
        public string KhamMat { get; set; }
        public string NgayThangNam { get; set; }
        public string NgayGioIn { get; set; }

    }

    public class ThongTinBenhTaiMuiHong : GridItem
    {
        public string HeaderTMH { get; set; }
        public string MaBN { get; set; }
        public string HoTenBenhNhan { get; set; }
        public string SinhNgay { get; set; }
        public string GioiTinh { get; set; }
        public string NgheNghiep { get; set; }
        public string DanToc { get; set; }
        public string QuocTich { get; set; }
        public string SoNha { get; set; }
        public string DiaChiDayDu { get; set; }
        public string XaPhuong { get; set; }
        public string Huyen { get; set; }
        public string TinhThanhPho { get; set; }
        public string NoiLamViec { get; set; }
        public string DoiTuong { get; set; }
        public string BHYTNgayHetHan { get; set; }
        public string BHYTMaSoThe { get; set; }
        public string NguoiLienHeQuanHeThanNhan { get; set; }
        public string SoDienThoaiQHTN { get; set; }
        public string ThoiDiemTiepNhan { get; set; }
        public string ChanDoanNoiGioiThieu { get; set; }
        public string TienSuDiUng { get; set; }
        public string TienSuBenh { get; set; }
        public string BarCodeImgBase64 { get; set; }
        public string PhongKham { get; set; }
        public string HoTenBacSi { get; set; }
        public string LyDoVaoKham { get; set; }
        public string LyDoVaoVien { get; set; }

        public string QuaTrinhBenhLy { get; set; }
        public string HuongXuLi { get; set; }
        public string ToanThan
        {
            get
            {
                var result = string.Empty;
                if (!string.IsNullOrEmpty(NoiDungKhamBenh))
                {
                    result += NoiDungKhamBenh.Replace("\n", "<br>") + "; ";
                }
                if (!string.IsNullOrEmpty(KhamToanThan))
                {
                    result += KhamToanThan.Replace("\n", "<br>") + "; ";
                }
                return result;
            }
        }
        public string NoiDungKhamBenh { get; set; }
        public string KhamToanThan { get; set; }
        public string Tai { get; set; }
        public string Mui { get; set; }
        public string Hong { get; set; }
        public string NgayThangNam { get; set; }
        public string TomTatCLS { get; set; }
        public string DaXuLi { get; set; }
        public string ChanDoanVaoVien { get; set; }
        public string NgayGioIn { get; set; }
    }

    public class ThongTinBenhRangHamMat : GridItem
    {
        public string HeaderRHM { get; set; }
        public string MaBN { get; set; }
        public string HoTenBenhNhan { get; set; }
        public string SinhNgay { get; set; }
        public string GioiTinh { get; set; }
        public string NgheNghiep { get; set; }
        public string DanToc { get; set; }
        public string QuocTich { get; set; }
        public string SoNha { get; set; }
        public string DiaChiDayDu { get; set; }
        public string XaPhuong { get; set; }
        public string Huyen { get; set; }
        public string TinhThanhPho { get; set; }
        public string NoiLamViec { get; set; }
        public string DoiTuong { get; set; }
        public string BHYTNgayHetHan { get; set; }
        public string BHYTMaSoThe { get; set; }
        public string NguoiLienHeQuanHeThanNhan { get; set; }
        public string SoDienThoaiQHTN { get; set; }
        public string ThoiDiemTiepNhan { get; set; }
        public string ChanDoanNoiGioiThieu { get; set; }
        public string TienSuDiUng { get; set; }
        public string TienSuBenh { get; set; }
        public string BarCodeImgBase64 { get; set; }
        public string PhongKham { get; set; }
        public string HoTenBacSi { get; set; }
        public string LyDoVaoVien { get; set; }
        public string LyDoVaoKham { get; set; }
        public string QuaTrinhBenhLy { get; set; }
        public string HuongXuLi { get; set; }
        public string ToanThan
        {
            get
            {
                var result = string.Empty;
                if (!string.IsNullOrEmpty(NoiDungKhamBenh))
                {
                    result += NoiDungKhamBenh.Replace("\n", "<br>") + "; ";
                }
                if (!string.IsNullOrEmpty(KhamToanThan))
                {
                    result += KhamToanThan.Replace("\n", "<br>") + "; ";
                }
                return result;
            }
        }
        public string NoiDungKhamBenh { get; set; }
        public string KhamToanThan { get; set; }
        public string Rang { get; set; }
        public string Ham { get; set; }
        public string Mat { get; set; }
        public string NgayThangNam { get; set; }
        public string TomTatCLS { get; set; }
        public string DaXuLi { get; set; }
        public string ChanDoanVaoVien { get; set; }
        public string NgayGioIn { get; set; }
    }

    public class ThongTinBenhKhac
    {
        public string Header { get; set; }
        public string So { get; set; }
        public string HeaderKhac { get; set; }
        public string MaTN { get; set; }
        public string MaBN { get; set; }
        public string HoTenBenhNhan { get; set; }
        public string SinhNgay { get; set; }
        public string Tuoi { get; set; }
        public string GioiTinh { get; set; }
        public string NgheNghiep { get; set; }
        public string DanToc { get; set; }
        public string QuocTich { get; set; }
        public string SoNha { get; set; }
        public string DiaChiDayDu { get; set; }
        public string DiaChi { get; set; }
        public string BenhSu { get; set; }
        public string ThonPho { get; set; }
        public string XaPhuong { get; set; }
        public string Huyen { get; set; }
        public string TinhThanhPho { get; set; }
        public string NoiLamViec { get; set; }
        public string DoiTuong { get; set; }
        public string BHYTNgayHetHan { get; set; }
        public string BHYTMaSoThe { get; set; }
        public string NguoiLienHeQuanHeThanNhan { get; set; }
        public string NguoiLienHeQuanHeSoDienThoai { get; set; }

        public string SoDienThoai { get; set; }
        public string ThoiDiemTiepNhan { get; set; }
        public string ChanDoanNoiGioiThieu { get; set; }
        public string Ngay { get; set; }
        public string Thang { get; set; }
        public string Nam { get; set; }
        public string PhongKham { get; set; }
        public string HoTenBacSi { get; set; }
        public string LyDoVaoVien { get; set; }
        public string QuaTrinhBenhLy { get; set; }
        public string BenhChuyenKhoa { get; set; }
        public string TomTatCLS { get; set; }
        public string ChanDoanVaoVien { get; set; }
        public string ChoDieuTriTaiKhoa { get; set; }
        public string DaXuLi { get; set; }
        public string Mach { get; set; }
        public string NhietDo { get; set; }
        public string HuyetAp { get; set; }
        public string NhipTho { get; set; }
        public string CanNang { get; set; }
        public string ChieuCao { get; set; }
        public string BMI { get; set; }
        public string SpO2 { get; set; }
        public string ChuY { get; set; }
        public string BanThanTenBenh { get; set; }
        public string GiaDinhTenBenh { get; set; }
        public string SanPhuKhoa { get; set; }
        public string TuanHoan { get; set; }
        public string HoHap { get; set; }
        public string TieuHoa { get; set; }
        public string ThanTietNieuSinhDuc { get; set; }
        public string ThanKinh { get; set; }
        public string CoXuongKhop { get; set; }
        public string RangHamMat { get; set; }
        public string NoiTietDinhDuong { get; set; }
        public string DaLieu { get; set; }
        public string CacBoPhanKhac { get; set; }
        public string CacBoPhan { get; set; }
        public string TienSuBenh { get; set; }
        public string TienSuDiUng { get; set; }
        public string HuongXuLy { get; set; }
        public string HuongDieuTri { get; set; }

        public string ChanDoan { get; set; }

        public string NgayThangNam { get; set; }
        public string XetNghiemDaLam { get; set; }
        public string KhamBenh { get; set; }
        public string GioKham { get; set; }
        public string NgayKham { get; set; }
        public string ThangKham { get; set; }
        public string NamKham { get; set; }
        public string LyDoVaoKham { get; set; }
        public string SDTThanNhan { get; set; }
        public string XetNghiemDaCoVaLamMoi { get; set; }
        public string KetQuaCanLamSang { get; set; }
        public string Khoa { get; set; }
        public string ToanThan
        {
            get
            {
                var result = string.Empty;
                if (!string.IsNullOrEmpty(NoiDungKhamBenh))
                {
                    result += NoiDungKhamBenh.Replace("\n", "<br>") + "; ";
                }
                if (!string.IsNullOrEmpty(KhamToanThan))
                {
                    result += KhamToanThan.Replace("\n", "<br>") + "; ";
                }
                return result;
            }
        }
        public string NoiDungKhamBenh { get; set; }
        public string KhamToanThan { get; set; }
    }

    public class ThongTinGiayChuyenVien
    {
        public string STT { get; set; }
        public string MaTN { get; set; }
        public string BenhVienChuyenDen { get; set; }
        public string HoTenBenhNhan { get; set; }
        public string SinhNgay { get; set; }
        public string Tuoi { get; set; }
        public string GioiTinh { get; set; }
        public string NgheNghiep { get; set; }
        public string DanToc { get; set; }
        public string QuocTich { get; set; }
        public string DiaChi { get; set; }
        public string NoiLamViec { get; set; }
        public string DoiTuong { get; set; }
        public string DaKhamBenhDieuTri { get; set; }
        public string BHYTNgayHetHan { get; set; }
        public string BHYTMaSoThe { get; set; }
        public string ChanDoan { get; set; }
        public string DauHieuLamSang { get; set; }
        public string TenLoaiLyDoChuyenVien { get; set; }
        public string KetQuaCLS { get; set; }
        public string ThoiDiemTiepNhan { get; set; }
        public string PhuongPhap { get; set; }
        public string TinhTrang { get; set; }
        public string HuongDieuTri { get; set; }
        public string ThoiGianChuyenTuyen { get; set; }
        public string PhuongTienVanChuyen { get; set; }
        public string NguoiHoTong { get; set; }
        public string Ngay { get; set; }
        public string Thang { get; set; }
        public string Nam { get; set; }
        public string BacSiThucHien { get; set; }
        public string KetQuaXetNghiemCLS { get; set; }
        public string PhuongPhapTrongDieuTri { get; set; }
        public string BenhVienHienTai { get; set; }
        public string NgayThangNam { get; set; }

    }

    public class ThongTinTheBenhNhan : GridItem
    {
        public string MaBN { get; set; }
        public string HoTenBenhNhan { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string GioiTinhDisplay { get; set; }
        public string SoDienThoai { get; set; }
        public string NgheNghiep { get; set; }
        public string QuocTich { get; set; }
        public string Tinh { get; set; }
        public string Huyen { get; set; }
        public string Phuong { get; set; }
        public string SoNha { get; set; }
        public string SoCMND { get; set; }
        public string Email { get; set; }
        public string DanToc { get; set; }
        public string DiaChiDayDu { get; set; }

    }

    public class TheBenhNhan
    {
        public string HostingName { get; set; }
        public string BarCodeImgBase64 { get; set; }
        public string MaBN { get; set; }
        public string HoTenBenhNhan { get; set; }

    }
    public class VongDeoTay
    {
        public string HostingName { get; set; }
        public string BarCodeByMaBN { get; set; }
        public string BarCodeImgBase64 { get; set; }
        public string MaBN { get; set; }
        public string HoTenBenhNhan { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public string GioiTinh { get; set; }
        public string SoDienThoai { get; set; }
        public int? Tuoi { get; set; }
        public string Khoa { get; set; }

    }
    public class TimKiemThongTin
    {
        public string TimKiemMaBNVaMaTN { get; set; }
    }

    public class ThongTinNgayNghiHuongBHYT
    {
        public long YeuCauKhamBenhId { get; set; }
        public DateTime? ThoiDiemTiepNhan { get; set; }
        public DateTime? DenNgay { get; set; }

        public long? ICDChinhNghiHuongBHYT { get; set; }
        public string TenICDChinhNghiHuongBHYT { get; set; }
        public string PhuongPhapDieuTriNghiHuongBHYT { get; set; }
    }

    public class NghiHuongBHYTJson
    {
        public long ICDId { get; set; }        
        public string Ten { get; set; }
    }


    public class PhieuKhamBenhVo
    {
        public long YeuCauKhamBenhId { get; set; }
        public bool CoKhamBenh { get; set; }
        public bool CoHeader { get; set; }
        public bool CoKhamBenhVaoVien { get; set; }
    }

    public class PhieuKhamBenhVoHtml
    {

        public PhieuKhamBenhVoHtml()
        {
            Htmls = new List<string>();
        }
        public string Html { get; set; }
        public string TenFile { get; set; }
        public bool? LaPhieuKhamBenh { get; set; }
        public List<string> Htmls { get; set; }
    }

    public class ThongTinNgayNghiHuongBHYTDetail
    {
        public string Header { get; set; }
        public long YeuCauKhamBenhId { get; set; }
        public string SoNgayNghi { get; set; }
        public string ThoiDiemTiepNhan { get; set; }
        public string DenNgay { get; set; }
        public string MaTN { get; set; }
        public string HoTenBenhNhan { get; set; }
        public string SinhNgay { get; set; }
        public string BHYTMaSoThe { get; set; }
        public string GioiTinh { get; set; }
        public string NoiLamViec { get; set; }
        public string LienSo { get; set; }
        public string ChanDoanPhuongPhap { get; set; }
        public string HoTenCha { get; set; }
        public string HoTenMe { get; set; }
        public string HoTenBacSi { get; set; }
        public string Ngay { get; set; }
        public string Thang { get; set; }
        public string Nam { get; set; }
        public string NgayThangNam { get; set; }
    }

    public class ThongTinNgayNghiHuongBHYTTiepNhan
    {
        public long YeuCauTiepNhanId { get; set; }
        public DateTime? ThoiDiemTiepNhan { get; set; }
        public DateTime? DenNgay { get; set; }
        public long? BacSiKetLuanId { get; set; }

        public long? ICDChinhNghiHuongBHYT { get; set; }
        public string TenICDChinhNghiHuongBHYT { get; set; }
        public string PhuongPhapDieuTriNghiHuongBHYT { get; set; }
    }

    public class NgayThangNamSinhVo
    {
        public string Days { get; set; }
        public string Months { get; set; }
        public string Years { get; set; }
    }

    public class NgayThangNamSinhTiepNhanBenhNhan
    {
        public int Days { get; set; }
        public int Months { get; set; }
        public int Years { get; set; }
    }

    public class YeuCauDichVuKTPTTTVo
    {
        public long YeuCauTiepNhanId { get; set; }
        public int? LanThucHien { get; set; }
    }

    public class YeuCauDichVuKhamTheoTiepNhanVo
    {
        public long YeuCauId { get; set; }
        public string TenDichVu { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public Enums.EnumTrangThaiYeuCauKhamBenh TrangThai { get; set; }
    }
}
