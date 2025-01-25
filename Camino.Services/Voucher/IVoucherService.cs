using Camino.Core.Domain.Entities.Vouchers;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Core.Domain.ValueObject.Voucher;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.Voucher
{
    public interface IVoucherService : IMasterFileService<Core.Domain.Entities.Vouchers.Voucher>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        bool CompareTuNgayDenNgay(DateTime? tuNgay, DateTime? denNgay);
        Task<List<VoucherDichVuVo>> GetListDichVuChoVoucher(DropDownListRequestModel queryInfo);
        Task<List<VoucherLoaiGiaVo>> GetListLoaiGiaChoDichVu(long dichVuId, EnumDichVuTongHop loaiDichVu);
        Task<List<VoucherLoaiGiaVo>> GetListTatCaLoaiGiaChoDichVu(EnumDichVuTongHop loaiDichVu);
        Task<decimal> GetDonGiaChoDichVu(long dichVuId, long loaiGiaId, EnumDichVuTongHop loaiDichVu);
        Task<GridDataSource> GetListDichVuForGridAsync(long voucherId);
        Task<GridDataSource> GetPagesListDichVuForGridAsync(long voucherId);
        Task<List<NhomDichVuBenhVienTreeViewVo>> GetListNhomDichVuChoVoucher(DropDownListRequestModel model);
        Task<GridDataSource> GetListNhomDichVuForGridAsync(long voucherId);
        Task<GridDataSource> GetPagesListNhomDichVuForGridAsync(long voucherId);
        Task<bool> KiemTraDichVuDaTonTaiTrongNhomDichVu(long voucherId, long dichVuId, EnumDichVuTongHop loaiDichVuBenhVien);
        Task<bool> KiemTraNhomDichVuDaBaoGomDichVu(long voucherId, long nhomDichVuId);
        Task<bool> KiemTraDichVuDaTonTaiTrongNhomDichVuTheoDanhSach(long dichVuId, EnumDichVuTongHop loaiDichVuBenhVien, List<VoucherChiTietMienGiam> lstVoucherChiTietMienGiam);
        Task<bool> KiemTraNhomDichVuDaBaoGomDichVuTheoDanhSach(long nhomDichVuId, List<VoucherChiTietMienGiam> lstVoucherChiTietMienGiam);
        string GetBarcodeBasedOnMa(string ma);
        string GetHtmlVoucher(string hostingName, string ten, string ma, int soLuong, int maSoTu, int soLuongPhatHanh);
        Task<GridDataSource> GetListChiTietBenhNhanDaSuDungForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetPagesListChiTietBenhNhanDaSuDungForGridAsync(QueryInfo queryInfo);
        Task<int> GetTongSoBenhNhanSuDungDichVu(long voucherId);
        Task<bool> IsMaExists(string ma, long id);
        Task<SoLuongPhatHanhVoucher> GetSoLuongPhatHanhVoucher(long voucherId);
        bool IsNhomDichVuKhamBenh(long nhomDichVuId);
        long GetNhomDichVuBenhVienId();
    }
}