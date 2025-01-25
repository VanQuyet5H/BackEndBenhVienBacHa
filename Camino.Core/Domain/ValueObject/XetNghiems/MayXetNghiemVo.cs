using Camino.Core.Domain.Entities.XetNghiems;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.XetNghiems
{
    public class MayXetNghiemVo
    {
        public long Id { get; set; }

        public string Ma { get; set; }

        public string Ten { get; set; }

        public long MauMayXetNghiemID { get; set; }

        public string TenMauMayXetNghiem { get; set; }

        public string NhaCungCap { get; set; }

        public bool HieuLuc { get; set; }

        public string HostName { get; set; }

        public string PortName { get; set; }

        public int? BaudRate { get; set; }

        public byte? DataBits { get; set; }

        public string StopBits { get; set; }

        public string Parity { get; set; }

        public string Handshake { get; set; }

        public string Encoding { get; set; }

        public int? ReadBufferSize { get; set; }

        public bool RtsEnable { get; set; }

        public bool DtrEnable { get; set; }

        public bool DiscardNull { get; set; }

        public string ConnectionMode { get; set; }

        public string ConnectionProtocol { get; set; }

        public bool AutoOpenPort { get; set; }

        public bool AutoOpenForm { get; set; }

        public DateTime? OpenDateTime { get; set; }

        public DateTime? CloseDateTime { get; set; }

        public int? ConnectionStatus { get; set; }

        public bool LogDataEnabled { get; set; }
    }

    public class PhienXetNghiemDataVo
    {
        public long Id { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public string BarCodeID { get; set; }
        public string MaBN { get; set; }
        public string MaYeuCauTiepNhan { get; set; }
        public string HoTen { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string SoDienThoai { get; set; }
        public string MaSoBHYT { get; set; }
        public Enums.EnumLoaiYeuCauTiepNhan LoaiYeuCauTiepNhan { get; set; }
        public int? BHYTMucHuong { get; set; }
        public bool? CoBHYT { get; set; }
        public bool? CoBHTN { get; set; }
        public string DiaChi { get; set; }
        public string ChanDoan { get; set; }
        public string KhoaChiDinh { get; set; }
        public string Phong { get; set; }
        public string NguoiThucHien { get; set; }
        public long? NguoiThucHienId { get; set; }
        public string GhiChu { get; set; }
        public string KetLuan { get; set; }
        public bool? TrangThai { get; set; }
        public string TenCongTy { get; set; }
        public bool? LaCapCuu { get; set; }
        public string TenCongTyBaoHiemTuNhan { get; set; }
        public List<PhienXetNghiemChiTietDataVo> PhienXetNghiemChiTietDataVos { get; set; } = new List<PhienXetNghiemChiTietDataVo>();
    }
    public class PhienXetNghiemChiTietDataVo
    {
        public long Id { get; set; }
        public bool? DaGoiDuyet { get; set; }
        public long YeuCauDichVuKyThuatId { get; set; }
        public string TenDichVu { get; set; }
        public string LoaiKitThu { get; set; }
        public string TenNhomDichVuBenhVien { get; set; }
        public long NhomDichVuBenhVienId { get; set; }
        public long? NoiChiDinhId { get; set; }
        public long NhanVienChiDinhId { get; set; }

        public EnumLoaiMauXetNghiem? LoaiMauXetNghiem { get; set; }
        public long? YeuCauKhamBenhId { get; set; }
        public long? NoiTruPhieuDieuTriId { get; set; }
        public DateTime? ThoiDiemNhanMau { get; set; }
        public DateTime? ThoiDiemKetLuan { get; set; }
        public DateTime? ThoiDiemLayMau { get; set; }
        public long? NhanVienLayMauId { get; set; }
        public long? NhanVienNhanMauId { get; set; }
        public List<KetQuaXetNghiemChiTiet> KetQuaXetNghiemChiTiets { get; set; } = new List<KetQuaXetNghiemChiTiet>();
    }
}
