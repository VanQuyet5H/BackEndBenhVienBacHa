using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.PhieuCongKhaiDichVuKyThuat
{
    public class PhieuCongKhaiDichVuKyThuat
    {
        public long YeuCauTiepNhanId { get; set; }
        public DateTime NgayVaoVien { get; set; }
        public DateTime NgayRaVien { get; set; }
    }
    public class PhieuCongKhaiDichVuKyThuatObject : GridItem
    {
        public long YeuCauTiepNhanId { get; set; }
        public DateTime ThoiDiemThucHien { get; set; }
        public Enums.LoaiHoSoDieuTriNoiTru LoaiHoSoDieuTriNoiTru { get; set; }
        public string ThongTinHoSo { get; set; }
        public long NhanVienThucHienId { get; set; }
        public long NoiThucHienId { get; set; }
        public List<FileChuKyPhieuCongKhaiDichVuKyThuatGridVo> ListFile { get; set; }
    }
    public class FileChuKyPhieuCongKhaiDichVuKyThuatGridVo : GridItem
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string TenGuid { get; set; }
        public long KichThuoc { get; set; }
        public string DuongDan { get; set; }
        public Enums.LoaiTapTin LoaiTapTin { get; set; }
        public string MoTa { get; set; }
    }
    public class InPhieuCongKhaiDichVuKyThuat
    {
        public string ChanDoan { get; set; }
        public DateTime? NgayVaoVien { get; set; }
        public DateTime? NgayRaVien { get; set; }
        public List<Table> TableString { get; set; }
    }
    public class Table
    {
        public string Html { get; set; }
    }

    #region Thuốc vật tư

    public class PhieuCongKhaiThuocVatTu
    {
        public long YeuCauTiepNhanId { get; set; }
        public DateTime NgayVaoVien { get; set; }
        public DateTime NgayRaVien { get; set; }
    }
    public class PhieuCongKhaiThuocVatTuObject : GridItem
    {
        public long YeuCauTiepNhanId { get; set; }
        public DateTime ThoiDiemThucHien { get; set; }
        public Enums.LoaiHoSoDieuTriNoiTru LoaiHoSoDieuTriNoiTru { get; set; }
        public string ThongTinHoSo { get; set; }
        public long NhanVienThucHienId { get; set; }
        public long NoiThucHienId { get; set; }
        public List<FileChuKyPhieuCongKhaiThuocVatTuThuatGridVo> ListFile { get; set; }
    }
    public class FileChuKyPhieuCongKhaiThuocVatTuThuatGridVo : GridItem
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string TenGuid { get; set; }
        public long KichThuoc { get; set; }
        public string DuongDan { get; set; }
        public Enums.LoaiTapTin LoaiTapTin { get; set; }
        public string MoTa { get; set; }
    }
    public class InPhieuCongKhaiThuocVatTu
    {
        public string ChanDoan { get; set; }
        public DateTime? NgayVaoVien { get; set; }
        public DateTime? NgayRaVien { get; set; }
        public List<Table> TableString { get; set; }
    }
   

    #endregion

}
