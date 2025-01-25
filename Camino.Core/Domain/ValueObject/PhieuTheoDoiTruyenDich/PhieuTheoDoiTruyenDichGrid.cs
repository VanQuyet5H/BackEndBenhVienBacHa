using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.PhieuTheoDoiTruyenDich
{
    public class PhieuTheoDoiTruyenDichGridInfo : GridItem
    {
        public long YeuCauTiepNhanId { get; set; }
        public DateTime ThoiDiemThucHien { get; set; }
        public Enums.LoaiHoSoDieuTriNoiTru LoaiHoSoDieuTriNoiTru { get; set; }
        public string ThongTinHoSo { get; set; }
        public long NhanVienThucHienId { get; set; }
        public long NoiThucHienId { get; set; }
        public List<FileChuKyPhieuTheoDoiTruyenDichGridVo> ListFile { get; set; }
    }
    public class FileChuKyPhieuTheoDoiTruyenDichGridVo : GridItem
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string TenGuid { get; set; }
        public long KichThuoc { get; set; }
        public string DuongDan { get; set; }
        public Enums.LoaiTapTin LoaiTapTin { get; set; }
        public string MoTa { get; set; }
    }
    public class XacNhanInPhieuTheoDoiTruyenDich
    {
        public long NoiTruHoSoKhacId { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public Enums.LoaiHoSoDieuTriNoiTru LoaiHoSoDieuTriNoiTru { get; set; }
        public string Hosting { get; set; }
    }
    public class InPhieuTheoDoiTruyenDich
    {
        public string ChanDoan { get; set; }
        public List<PhieuTheoDoiTruyenDichGridVo> DachSachTruyenDichArr { get; set; }
    }

    public class InPhieuTheoDoiTruyenDichVo
    {
        public string ChanDoan { get; set; }
        public List<DachSachTruyenDich> DachSachTruyenDichArr { get; set; }
        public List<DachSachTruyenDich> DachSachTruyenDichArrDefault { get; set; }
        public string NgayThucHien { get; set; }
        public string TaiKhoanDangNhap { get; set; }
    }
   
    public class DachSachTruyenDich
    {
        public long Id { get; set; }
        public string Ngay { get; set; }
        public string TenDichTruyen { get; set; }
        public double? SoLuong { get; set; }
        public string LoSoSX { get; set; }
        public double? TocDo { get; set; }
        public double? BatDau { get; set; }
        public double? KetThuc { get; set; }
        public string BSChiDinh { get; set; }
        public string YTaThucHien { get; set; }
        public string NgayThu { get; set; }
        public long? IdTruyenDich { get; set; }
        public string TenTruyenDich { get; set; }
        public bool NoiTruChiTietYLenhThucHien { get; set; }
        public long? NoiTruPhieuDieuTriId { get; set; }
        public DateTime NgayPhieuDieuTri { get; set; }
    }


    public class ValidatetorTruyenDichVo
    {
        public ValidatetorTruyenDichVo()
        {
            listTruyenDich = new List<ValidateSoLuongTruyenDichVo>();
        }
        public List<ValidateSoLuongTruyenDichVo> listTruyenDich { get; set; }
    }
    public class ValidateSoLuongTruyenDichVo
    {
        public string NgayThu { get; set; }

        public double? SoLuong { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public long? DuocPhamId { get; set; }
        public int BatDau { get; set; }
    }
    public class PhieuTheoDoiTruyenComPare 
    {
        public bool NoiTruChiTietYLenhThucHien { get; set; }
        public long? NoiTruPhieuDieuTriChiTietYLenhId { get; set; }
        public long? YeuCauLinhDuocPhamId { get; set; }
    }
}
