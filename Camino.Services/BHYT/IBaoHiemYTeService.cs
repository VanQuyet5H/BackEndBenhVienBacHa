using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.BHYT;
using Camino.Core.Domain.ValueObject.HamGuiHoSoWatchings;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.BHYT
{
    public interface IBaoHiemYTeService
    {
        ThongTinBHYTVO GetThongTin(ThongTinBenhNhanXemVO thongTinBenhNhan);
        ThongTinBHYTVO GetTokenAndAutoResendThongTin(ThongTinBenhNhanXemVO thongTinBenhNhan);
        Task<HamGuiHoSoWatchingVO> GoiHoSoGiamDinh(ThongTinBenhNhan thongtin, bool isDownLoad);
        HamGuiHoSoWatchingVO addValueToXml(List<ThongTinBenhNhan> thongTinBenhNhans);

        byte[] pathFileTongHop(string tenFile);
        Task<bool> CheckValidGiaTriTheTu(string giaTri);
        Task<bool> CheckValidSoNgayDieuTri(string ngayVao, string ngayRa, int? soNgayDieuTri);
        Task<bool> CheckValidMaDKBD(string maDKBD);
        Task<bool> CheckValidNgayVaoRa(DateTime? ngay);
        Task<bool> CheckValidMaBenhKhac(string maBenhKhac);
        Task<bool> CheckValidGiaTriTheDen(string giaTriTheTu, string giaTriTheDen);
        Task<bool> CheckSpace(string giaTri);
        Task<bool> CheckValueTyLeThanhToan(int? giatri, int? phamvi);
        Task<bool> CheckValueTienNguonKhac(double? giatri, double? thanhtien);
        Task<bool> CheckMaDichVuHoSoDVKT(string giaTri, Enums.EnumDanhMucNhomTheoChiPhi? maNhom);
        Task<bool> CheckMaDichVuHoSoDVKTRequired(string giaTri, Enums.EnumDanhMucNhomTheoChiPhi? maNhom);
        Task<bool> CheckMaVatTuHoSoDVKTRequired(string giaTri, Enums.EnumDanhMucNhomTheoChiPhi? maNhom);
        Task<bool> CheckValidGoiVatTuXML3(string giaTri);
        Task<bool> CheckThongTinThauValid(string giaTri, string maVattu, Enums.EnumDanhMucNhomTheoChiPhi? maNhom);
    }
}
