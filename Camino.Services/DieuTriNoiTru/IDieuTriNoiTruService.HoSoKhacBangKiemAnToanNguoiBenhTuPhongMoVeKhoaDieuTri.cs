using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BangKiemAnToanNguoiBenhPTTuPhongDieuTri;
using Camino.Core.Domain.ValueObject.DichVuKyThuat;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.TrichBienBanHoiChan;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        Task<List<string>> GetDanhSachPhauThuatVien();
        Task<List<DichVuKyThuatChoBenhVienTemplateVo>> GetDanhSachXeNghiemCanLam(DropDownListRequestModel model);
        Task<List<LookupItemVo>> GetListThuocDaDung(DropDownListRequestModel model, long yeuCauTiepNhanId);
        Task<List<LookupItemVo>> GetListThuocDangDung(DropDownListRequestModel model, long yeuCauTiepNhanId);
        Task<List<LookupItemVo>> GetListThuocBanGiao(DropDownListRequestModel model);
        Task<GridDataSource> GetDanhSachNguoiBenhAnToanPTTuPhongDieuTri(QueryInfo queryInfo);
        BangKiemAnToanNguoiBenhPTTuPhongDieuTriGrid GetThongTinBangKiemAnToanNguoiBenhPTTuPhongDieuTri(long yeuCauTiepNhanId);
        BangKiemAnToanNguoiBenhPTTuPhongDieuTriGrid GetThongTinBenhNhanPtVephongDieuTriViewDS(long noiTruHoSoKhacId);
        Task<string> InBangKiemAnToanNguoiBenhPTVeKhoaDieuTri(XacNhanInTrichBienBanHoiChan xacNhanInTrichBienBanHoiChan);
    }

}
