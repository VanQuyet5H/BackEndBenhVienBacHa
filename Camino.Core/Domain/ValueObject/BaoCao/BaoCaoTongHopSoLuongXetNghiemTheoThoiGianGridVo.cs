using System;
using System.Collections.Generic;
using Camino.Core.Domain.Entities.XetNghiems;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.BaoCao
{
    public class BaoCaoTongHopSoLuongXetNghiemTheoThoiGianChiTietVo : GridItem
    {
        //public long NhomDichVuBenhVienId { get; set; }
        public long DichVuKyThuatBenhVienId { get; set; }
        public Enums.EnumLoaiYeuCauTiepNhan LoaiYeuCauTiepNhan { get; set; }
        public bool DuocHuongBaoHiem { get; set; }
        public bool? BaoHiemChiTra { get; set; }
        public long? NoiTruPhieuDieuTriId { get; set; }
        public long? YeuCauKhamBenhId { get; set; }
        public List<KetQuaPhienXetNghiemChiTietVo> KetQuaPhienXetNghiemChiTietVos { get; set; }

        public bool MauNoiTru => LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && !DuocHuongBaoHiem;
        public bool MauNgoaiTru => LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru && YeuCauKhamBenhId != null && !DuocHuongBaoHiem;
        public bool MauBHYTNoiTru => LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && DuocHuongBaoHiem;
        public bool MauBHYTNgoaiTru => LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru && YeuCauKhamBenhId != null && DuocHuongBaoHiem;
        public bool DichVu => LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru && YeuCauKhamBenhId == null;
        public bool KhamSucKhoe => LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe;

    }

    public class KetQuaPhienXetNghiemChiTietVo : GridItem
    {
        public string MaBarcode { get; set; }
        public DateTime? ThoiDiemLayMau { get; set; }
        public DateTime? ThoiDiemKetLuan { get; set; }
        public DateTime? ThoiDiemCoKetQua { get; set; }
        //public IEnumerable<Tuple<string,string>> GiaTriTuMayVaNhapTays { get; set; }
        public List<KetQuaChiSoXetNghiemChiTietVo> KetQuaChiSoXetNghiemChiTietVos { get; set; }
    }

    public class KetQuaChiSoXetNghiemChiTietVo : GridItem
    {
        public long DichVuXetNghiemId { get; set; }
        public string DichVuXetNghiemMa { get; set; }
        public string DichVuXetNghiemTen { get; set; }
        public DateTime? ThoiDiemDuyetKetQua { get; set; }
        public int? SoThuTu { get; set; }
        public int CapDichVu { get; set; }
        public string MaChiSo { get; set; }
        public long? MauMayXetNghiemId { get; set; }
        public long? MayXetNghiemId { get; set; }
        public string GiaTriTuMay { get; set; }
        public string GiaTriNhapTay { get; set; }
        public string GiaTriDuyet { get; set; }
        public string GiaTriCu { get; set; }
    }

    public class BaoCaoTongHopSoLuongXetNghiemTheoThoiGianGridVo : GridItem
    {
        public long NhomDichVuBenhVienId { get; set; }
        public long DichVuKyThuatBenhVienId { get; set; }
        public bool ToDam { get; set; }
        public int STT { get; set; }
        public int SoLanThucHienXetNghiem { get; set; }
        public string TenDichVu { get; set; }
        public int SoLuongMauNoiTru { get; set; }
        public int SoLuongMauNgoaiTru { get; set; }
        public int SoLuongMauBHYTNoiTru { get; set; }
        public int SoLuongMauBHYTNgoaiTru { get; set; }
        public int SoLuongDichVu { get; set; }
        public int SoLuongKhamSucKhoe { get; set; }

        public int SoLuongTongHop => SoLuongMauNoiTru + SoLuongMauNgoaiTru + SoLuongMauBHYTNoiTru +
                                     SoLuongMauBHYTNgoaiTru + SoLuongDichVu + SoLuongKhamSucKhoe;
    }
}