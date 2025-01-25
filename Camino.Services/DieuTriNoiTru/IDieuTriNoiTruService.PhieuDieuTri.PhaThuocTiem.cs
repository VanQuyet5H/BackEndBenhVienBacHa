using Camino.Core.Domain;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        GridDataSource GetDataForGridDanhSachPhaThuocTiem(QueryInfo queryInfo);
        GridDataSource GetTotalPageForGridDanhSachPhaThuocTiem(QueryInfo queryInfo);
        GridDataSource GetDataForGridDanhSachPhaThuocTiemNgoai(QueryInfo queryInfo);
        GridDataSource GetTotalPageForGridDanhSachPhaThuocTiemNgoai(QueryInfo queryInfo);
        Task ThemPhaThuocTiem(PhaThuocTiemBenhVienVo donThuocChiTiet, YeuCauTiepNhan yeuCauTiepNhan);
        Task CapNhatPhaThuocTiem(PhaThuocTiemBenhVienVo donThuocChiTiet, YeuCauTiepNhan yeuCauTiepNhan);

        //Task XoaPhaThuocTiems(PhaThuocTiemBenhVienVo donThuocChiTiet, YeuCauTiepNhan yeuCauTiepNhan);
        //Task<string> XuLySoThuTuPhaThuocTiemHoacTruyen(YeuCauTiepNhan yeuCauTiepNhan, bool LaPhaThuocTiem, long phieuDieuTriId);
        //Task<string> TangHoacGiamSTTDonThuocTiemChiTiet(ThuocBenhVienTangGiamSTTTiemHoacTruyenVo donThuocChiTiet, YeuCauTiepNhan yeuCauTiepNhan);

        Task CapNhatKhongTinhPhiTiem(CapNhatKhongTinhPhiTiem capNhatKhongTinhPhi, YeuCauTiepNhan yeuCauTiepNhan);


    }
}
