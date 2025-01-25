using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;

namespace Camino.Core.Domain.ValueObject.YeuCauKhamBenh
{
    public class KetLuanGridVo : GridItem
    {
        public string TenDichVu { get; set; }
        public string ChuanDoanICDChinh { get; set; }
        public List<Entities.YeuCauKhamBenhs.YeuCauKhamBenhICDKhac> ChuanDoanICDPhu { get; set; }
        public string HuongDieuTri { get; set; }
        public string KetQuaDieuTri { get; set; }
        public string TinhTrangRaVien { get; set; }
        public bool? TaiKham { get; set; }
        public DateTime? NgayTaiKham { get; set; }
        public string NgayHenKhamDisplay { get; set; }
        public string GhiChu { get; set; }
        public bool? KhamChuyenKhoaTiepTheo { get; set; }
        public string MaDichVu { get; set; }
        public string LoaiGia { get; set; }
        public string DuocHuongBaoHiem { get; set; }
        public string NoiThucHien { get; set; }
        public int? SoLuong { get; set; }
        public decimal? DonGia { get; set; }
        public decimal? ThanhTien { get; set; }

        // 
        public bool? CoKeToa { get; set; }
        public string TomTatKetQuaCLS { get; set; }
        //public 
        public string SuDung { get; set; }
        public string BacSyKeToa { get; set; }
        // update 16/6/2020
        public string ChuThichChanDoanICDChinh { get; set; }
        public string ChuThichChanDoanICDPhu { get; set; }
        public string ICDPhu { get; set; }
       

        public string CachGiaiQuyet { get; set; }

        public bool? NhapVien { get; set; }
        public string Khoa { get; set; }
        public string LyDoNhapVien { get; set; }
       
        public bool? ChuyenVien { get; set; }
        public string BenhVienChuyenDen { get; set; }
        public string LyDoChuyenVien { get; set; }
        public bool? CoTuVong { get; set; }
        // update 16/6/2020
        public bool? DieuTriNgoaiTru { get; set; }
        public string TenDichVuKyThuat { get; set; }
        public int? SoLanDieuTri { get; set; }
        public string ThoiGianBatDauDieuTri { get; set; }
        public string DiUngThuocDisplay { get; set; }
        // update 27/8/2020
        public string TinhTrangChuyenVien { get; set; }
        public string ThoiDiemChuyenVien { get; set; }
        public string NhanVienHoTong { get; set; }
        public string PhuongTienDiChuyen { get; set; }
        public string HuongDieuTriChuyenVien { get; set; }
    }
}
