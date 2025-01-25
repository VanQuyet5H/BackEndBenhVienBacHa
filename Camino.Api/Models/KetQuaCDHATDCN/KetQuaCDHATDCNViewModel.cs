using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Newtonsoft.Json;

namespace Camino.Api.Models.KetQuaCDHATDCN
{
    public class KetQuaCDHATDCNViewModel : BaseViewModel
    {
        public KetQuaCDHATDCNViewModel()
        {
            FileKetQuaCanLamSangs = new List<FileChuKyKetQuaCDHATDCNViewModel>();
        }
        public string ThongTinHanhChinhMaTiepNhan { get; set; }
        public string ThongTinHanhChinhMaBenhNhan { get; set; }
        public string ThongTinHanhChinhHoTen { get; set; }
        public int? ThongTinHanhChinhNgaySinh { get; set; }
        public int? ThongTinHanhChinhThangSinh { get; set; }
        public int? ThongTinHanhChinhNamSinh { get; set; }
        public int ThongTinHanhChinhTuoi => ThongTinHanhChinhNamSinh == null ? 0 : DateTime.Now.Year - ThongTinHanhChinhNamSinh.Value;
        public string ThongTinHanhChinhTenGioiTinh { get; set; }
        public string ThongTinHanhChinhDiaChi { get; set; }
        public string ThongTinHanhChinhDoiTuong { get; set; }
        public long? ThongTinHanhChinhBacSiChiDinhId { get; set; }
        public string ThongTinHanhChinhBacSiChiDinh { get; set; }
        public string ThongTinHanhChinhNgayChiDinh { get; set; }
        public string ThongTinHanhChinhNoiChiDinh { get; set; }
        public string ThongTinHanhChinhSoBenhAn { get; set; }
        public string ThongTinHanhChinhChanDoan { get; set; }
        public string ThongTinHanhChinhChiDinh { get; set; }


        public long YeuCauTiepNhanId { get; set; }
        public string DataKetQuaCanLamSang { get; set; }
        public ChiTietKetQuaCDHATDCNViewModel ChiTietKetQuaObj { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
        public long? NhanVienThucHienId { get; set; }
        public long? NhanVienKetLuanId { get; set; }
        public long? NhanVienThucHien2Id { get; set; }
        public long? NhanVienKetLuan2Id { get; set; }
        public string GhiChuKetQuaCLS { get; set; }
        public DateTime? ThoiDiemKetLuan { get; set; }
        public long? MayTraKetQuaId { get; set; }
        public bool DichVuCoInKetQuaKemHinhAnh { get; set; }
        public List<FileChuKyKetQuaCDHATDCNViewModel> FileKetQuaCanLamSangs { get; set; }
        public long? DichVuKyThuatBenhVienId { get; set; }

        public long? NguoiLuuId { get; set; }
        public string NguoiLuuTen { get; set; }

        public bool? ChoKetQua { get; set; }  
        
        //BVHD-3822
        public bool? LaCapCuu { get; set; }

        //BVHD-3941
        public string TenCongTyBaoHiemTuNhan { get; set; }
    }

    public class ChiTietKetQuaCDHATDCNViewModel
    {
        public ChiTietKetQuaCDHATDCNViewModel()
        {
            HinhAnhDinhKems = new List<HinhAnhDinhKemKetQua>();
        }
        public string TenKetQuaLabel => "KẾT QUẢ";
        public string TenKetQua { get; set; }
        public string KyThuat { get; set; }
        public string KetQua { get; set; }
        public string KetLuan { get; set; }
        public string CoInKetQuaKemHinhAnh { get; set; }
        public bool InKemAnh { get; set; }
        public List<HinhAnhDinhKemKetQua> HinhAnhDinhKems { get; set; }
    }

    public class HinhAnhDinhKemKetQua
    {
        public string HinhAnh { get; set; }
        public string MoTa { get; set; }
        public bool InKemKetQua { get; set; }
    }

    public class FileChuKyKetQuaCDHATDCNViewModel : BaseViewModel
    {
        public long? YeuCauDichVuKyThuatId { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string TenGuid { get; set; }
        public long? KichThuoc { get; set; }
        public string DuongDan { get; set; }
        public string MoTa { get; set; }
        public Enums.LoaiTapTin? LoaiTapTin { get; set; }
    }
}
