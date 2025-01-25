using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.BaoCaos
{
    public class BaoCaoDoanhThuChiaTheoKhoaPhongGridVo :GridItem
    {
        public long KhoaPhongId { get; set; }
        public string KhoaPhong { get; set; }
        public decimal? DoanhThuKhachLe { get; set; }
        public decimal? DoanhThuDoan { get; set; }
        public decimal? DoanhThuBaoHiemYTe { get; set; }
        public decimal? DoanhThuCoDinh { get; set; }
        public decimal? DoanhThuThuocVaVTYT { get; set; }
        public decimal? DoanhThuSuDungPhongMo { get; set; }
        public decimal? DoanhThuLuongBacSiPartime { get; set; }
        public decimal? DoanhThuTienDien { get; set; }
        public decimal? DoanhThuSuatAn { get; set; }
        public decimal? DoanhThuTongCong => DoanhThuKhachLe.GetValueOrDefault() + DoanhThuDoan.GetValueOrDefault() + DoanhThuBaoHiemYTe.GetValueOrDefault() + DoanhThuCoDinh.GetValueOrDefault() +
            DoanhThuThuocVaVTYT.GetValueOrDefault() + DoanhThuSuDungPhongMo.GetValueOrDefault() + DoanhThuLuongBacSiPartime.GetValueOrDefault() + DoanhThuTienDien.GetValueOrDefault() + DoanhThuSuatAn.GetValueOrDefault();
    }

    public class BaoCaoDoanhThuKSKChiaTheoKhoaPhongGridVo : GridItem
    {
        public long KhoaPhongId { get; set; }
        public decimal DoanhThuThucTe { get; set; }        
    }

    public class BaoCaoDoanhThuChiTietVATDonThuocDataVo
    {
        public long Id { get; set; }
        public int VAT { get; set; }
    }

    public class BaoCaoDoanhThuChiaTheoKhoaPhongQueryInfo 
    {
        public long? KhoaPhongThucHienDichVuId { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
    }
    public class SumBaoCaoDoanhThuChiaTheoKhoaPhongGridVo : GridItem
    {
        public decimal TotalDoanhThuKhachLe { get; set; }
        public decimal TotalDoanhThuDoan { get; set; }
        public decimal TotalDoanhThuBaoHiemYTe { get; set; }
        public decimal TotalDoanhThuCoDinh { get; set; }
        public decimal TotalDoanhThuThuocVaVTYT { get; set; }
        public decimal TotalDoanhThuSuDungPhongMo { get; set; }
        public decimal TotalDoanhThuLuongBacSiPartime { get; set; }
        public decimal TotalDoanhThuTienDien { get; set; }
        public decimal TotalDoanhThuSuatAn { get; set; }
        public decimal TotalDoanhThuTongCong { get; set; }
    }
    public class BaoCaoKeHoachTongHopQueryInfo
    {
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
    }
    public class BaoCaoKeHoachTongHopGridVo : GridItem
    {
        public string MaYeuCauTiepNhan { get; set; }
        public string TenNguoiBenh { get; set; }
        public DateTime SoPhutChoKhamBenh { get; set; }
        public DateTime SoPhutChoXetNghiem { get; set; }
        public DateTime SoPhutChoChuanDoanHA { get; set; }
        public DateTime SoPhutChoThamDoChucNang { get; set; }
        public float TrungBinh { get; set; }
        public DateTime? ThoiDiemTN { get; set; }
        public string ThoiDiemTNStr =>ThoiDiemTN?.ApplyFormatTime();
        public DateTime? ThoiDiemBSKham { get; set; }
        public string ThoiDiemBSKhamStr => ThoiDiemBSKham?.ApplyFormatTime();
        public DateTime? ThoiDiemRaChiDinh { get; set; }
        public string ThoiDiemRaChiDinhStr => ThoiDiemRaChiDinh?.ApplyFormatTime();
        public DateTime? ThoiDiemLayMauXN { get; set; }
        public string ThoiDiemLayMauXNStr =>ThoiDiemLayMauXN?.ApplyFormatTime();
        public DateTime? ThoiDiemTraKetQuaXN { get; set; }
        public string ThoiDiemTraKQXNStr =>ThoiDiemTraKetQuaXN?.ApplyFormatTime();
        public DateTime? ThoiDiemThucHienCLS { get; set; }
        public string ThoiDiemCDHAStr =>ThoiDiemThucHienCLS?.ApplyFormatTime() ;
        public DateTime? ThoiDiemBacSiKetLuan { get; set; }
        public string ThoiDiemKLStr =>ThoiDiemBacSiKetLuan?.ApplyFormatTime() ;
        public DateTime? ThoiDiemBacSiKeDonThuoc { get; set; }
        public string ThoiDiemKeDonStr => ThoiDiemBacSiKeDonThuoc?.ApplyFormatTime();

        public double SoPhutChoKB { get; set; }

    }
    public class BaoCaoKeHoachTongHopTheoNgayGridVo : GridItem
    {
        public float SoPhutChoKhamBenh { get; set; }
        public float SoPhutChoXetNghiem { get; set; }
        public float SoPhutChoChuanDoanHA { get; set; }
        public float SoPhutChoThamDoChucNang { get; set; }
        public float TrungBinh { get; set; }


    }
}
