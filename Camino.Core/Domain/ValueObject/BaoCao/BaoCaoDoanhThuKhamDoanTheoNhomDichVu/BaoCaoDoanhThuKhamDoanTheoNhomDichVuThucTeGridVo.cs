using Camino.Core.Domain.ValueObject.Grid;
using System;

namespace Camino.Core.Domain.ValueObject.BaoCao.BaoCaoDoanhThuKhamDoanTheoNhomDichVu
{
    public class BaoCaoDoanhThuKhamDoanTheoNhomDichVuThucTeQueryInfo : QueryInfo
    {
        public long HopDongId { get; set; }
        public long CongTyId { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime FromDate { get; set; }
    }
    public class BaoCaoDoanhThuKhamDoanTheoNhomDichVuThucTeDataVo
    {
        public long YeucauTiepNhanId { get; set; }
        public long? YeucauKhamBenhId { get; set; }
        public long? YeucauDichVuKyThuatId { get; set; }
        public long? NhomDichVuBenhVienId { get; set; }
        public Enums.LoaiDichVuKyThuat? LoaiDichVuKyThuat { get; set; }
        public string TenDichVu { get; set; }
        public long? GoiKhamSucKhoeId { get; set; }
        public decimal Gia { get; set; }
        public int SoLan { get; set; }
        //public decimal? DonGiaUuDai { get; set; }
        public decimal? SoTienMienGiam { get; set; }
        public long? DichVuKhamBenhBenhVienId { get; set; }
        public long? NhomGiaDichVuKhamBenhBenhVienId { get; set; }
        public long? DichVuKyThuatBenhVienId { get; set; }
        public long? NhomGiaDichVuKyThuatBenhVienId { get; set; }
        public long? NoiThucHienId { get; set; }
        public long? NoiChiDinhId { get; set; }
    }
    public class BaoCaoDoanhThuKhamDoanTheoNhomDichVuThucTeDataXetNghiemVo
    {
        public long Id { get; set; }
        public DateTime? ThoiDiemNhanMau { get; set; }
    }
    public class BaoCaoDoanhThuKhamDoanTheoNhomDichVuThucTeYCTNDataVo
    {
        public long YeucauTiepNhanId { get; set; }
        public long HopDongId { get; set; }
        public long CongTyId { get; set; }
        public string MaNB { get; set; }
        public string MaTN { get; set; }
        public string HoVaTen { get; set; }
        public int? NamSinh { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string TenCongTy { get; set; }
        public string SoHopDong { get; set; }
    }
    public class BaoCaoDoanhThuKhamDoanTheoNhomDichVuThucTeGridVo : GridItem
    {
        public int STT { get; set; }
        public long HopDongId { get; set; }
        public long CongTyId { get; set; }
        public string MaNB { get; set; }
        public string MaTN { get; set; }
        public string HoVaTen { get; set; }
        public string NamSinh { get; set; }
        public string GioiTinh { get; set; }
        public string TenCongTy { get; set; }

        #region DOANH THU DỊCH VỤ TRONG GÓI
        //Nhóm Dv khám bệnh
        public decimal? SoTienDVKhamBenh { get; set; }
        //Nhóm DV xét nghiệm
        public decimal? SoTienDVXetNghiem { get; set; }
        //Nhóm Thăm dò chức năng - Nội soi
        public decimal? SoTienDVThamDoChucNangNoiSoi { get; set; }
        //Nhóm TDCN - Nội soi TMH
        public decimal? SoTienDVTDCNNoiSoiTMH { get; set; }
        //Nhóm CĐHA - Siêu âm
        public decimal? SoTienDVCDHASieuAm { get; set; }
        //Nhóm CĐHA - X quang thường + X quang số hóa
        public decimal? SoTienDVCDHAXQuangThuongXQuangSoHoa { get; set; }
        //Nhóm CĐHA - CTScanner
        public decimal? SoTienDVCTScan { get; set; }
        //MRI
        public decimal? SoTienDVMRI { get; set; }
        //ĐiệnTim + Điện Não
        public decimal? SoTienDVDienTimDienNao { get; set; }
        //TDCN + Đo loãng xương
        public decimal? SoTienDVTDCNDoLoangXuong { get; set; }
        //DV khác
        public decimal? SoTienDVKhac { get; set; }
        //Tổng cộng
        public decimal? TongCong { get; set; }
        #endregion
        #region DOANH THU DỊCH VỤ PHÁT SINH NGOÀI GÓI
        //Nhóm Dv khám bệnh
        public decimal? SoTienDVKhamBenhNG { get; set; }
        //Nhóm DV xét nghiệm
        public decimal? SoTienDVXetNghiemNG { get; set; }
        //Nhóm Thăm dò chức năng - Nội soi
        public decimal? SoTienDVThamDoChucNangNoiSoiNG { get; set; }
        //Nhóm TDCN - Nội soi TMH
        public decimal? SoTienDVTDCNNoiSoiTMHNG { get; set; }
        //Nhóm CĐHA - Siêu âm
        public decimal? SoTienDVCDHASieuAmNG { get; set; }
        //Nhóm CĐHA - X quang thường + X quang số hóa
        public decimal? SoTienDVCDHAXQuangThuongXQuangSoHoaNG { get; set; }
        //Nhóm CĐHA - CTScanner
        public decimal? SoTienDVCTScanNG { get; set; }
        //MRI
        public decimal? SoTienDVMRING { get; set; }
        //ĐiệnTim + Điện Não
        public decimal? SoTienDVDienTimDienNaoNG { get; set; }
        //TDCN + Đo loãng xương
        public decimal? SoTienDVTDCNDoLoangXuongNG { get; set; }
        //DV Thủ thuật
        public decimal? SoTienDVThuThuatNG { get; set; }
        //DV phẩu thuật
        public decimal? SoTienDVPhauThuatNG { get; set; }
        //DV khác
        public decimal? SoTienDVKhacNG { get; set; }
        //Tổng cộng
        public decimal? TongCongNG { get; set; }
        #endregion

        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
    }
    public class TotalTienDichVuThucTeKhamDoanGridVo : GridItem
    {
        public string TenCongTy { get; set; }
        #region DOANH THU DỊCH VỤ TRONG GÓI
        //Nhóm Dv khám bệnh
        public decimal? SoTienDVKhamBenhs { get; set; }
        //Nhóm DV xét nghiệm
        public decimal? SoTienDVXetNghiems { get; set; }
        //Nhóm Thăm dò chức năng - Nội soi
        public decimal? SoTienDVThamDoChucNangNoiSois { get; set; }
        //Nhóm TDCN - Nội soi TMH
        public decimal? SoTienDVTDCNNoiSoiTMHs { get; set; }
        //Nhóm CĐHA - Siêu âm
        public decimal? SoTienDVCDHASieuAms { get; set; }
        //Nhóm CĐHA - X quang thường + X quang số hóa
        public decimal? SoTienDVCDHAXQuangThuongXQuangSoHoas { get; set; }
        //Nhóm CĐHA - CTScanner
        public decimal? SoTienDVCTScans { get; set; }
        //MRI
        public decimal? SoTienDVMRIs { get; set; }
        //ĐiệnTim + Điện Não
        public decimal? SoTienDVDienTimDienNaos { get; set; }
        //TDCN + Đo loãng xương
        public decimal? SoTienDVTDCNDoLoangXuongs { get; set; }
        //DV khác
        public decimal? SoTienDVKhacs { get; set; }
        //Tổng cộng
        public decimal? TongCongs { get; set; }
        #endregion
        #region DOANH THU DỊCH VỤ PHÁT SINH NGOÀI GÓI
        //Nhóm Dv khám bệnh
        public decimal? SoTienDVKhamBenhNGs { get; set; }
        //Nhóm DV xét nghiệm
        public decimal? SoTienDVXetNghiemNGs { get; set; }
        //Nhóm Thăm dò chức năng - Nội soi
        public decimal? SoTienDVThamDoChucNangNoiSoiNGs { get; set; }
        //Nhóm TDCN - Nội soi TMH
        public decimal? SoTienDVTDCNNoiSoiTMHNGs { get; set; }
        //Nhóm CĐHA - Siêu âm
        public decimal? SoTienDVCDHASieuAmNGs { get; set; }
        //Nhóm CĐHA - X quang thường + X quang số hóa
        public decimal? SoTienDVCDHAXQuangThuongXQuangSoHoaNGs { get; set; }
        //Nhóm CĐHA - CTScanner
        public decimal? SoTienDVCTScanNGs { get; set; }
        //MRI
        public decimal? SoTienDVMRINGs { get; set; }
        //ĐiệnTim + Điện Não
        public decimal? SoTienDVDienTimDienNaoNGs { get; set; }
        //TDCN + Đo loãng xương
        public decimal? SoTienDVTDCNDoLoangXuongNGs { get; set; }
        //DV Thủ thuật
        public decimal? SoTienDVThuThuatNGs { get; set; }
        //DV phẩu thuật
        public decimal? SoTienDVPhauThuatNGs { get; set; }
        //DV khác
        public decimal? SoTienDVKhacNGs { get; set; }
        //Tổng cộng
        public decimal? TongCongNGs { get; set; }
        #endregion
        public long HopDongId { get; set; }
        public long CongTyId { get; set; }
    }
}
