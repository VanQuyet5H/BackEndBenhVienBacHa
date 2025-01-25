using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Core.Domain.ValueObject.PhieuNghiDuongThai;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;

namespace Camino.Services.YeuCauKhamBenh
{
    public partial interface IYeuCauKhamBenhService : IMasterFileService<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh>
    {
        string GetBodyByName(string ten);
        string GetResourceValueByResourceName(string ten);
        Task<List<ICDTemplateVo>> GetListICDBaoGomItemDaChonAsync(DropDownListRequestModel queryInfo);
        Task KiemTraDatayeuCauKhamBenhAsync(long yeuCauKhamBenhId, long phongBenhVienHangDoiId = 0, Enums.EnumTrangThaiHangDoi trangThaiHangDoi = Enums.EnumTrangThaiHangDoi.DangKham);
        void KiemTraChanDoanSoBoKhiThemDichVu(YeuCauTiepNhan yeuCauTiepNhan, long yeuCauKhamBenhId);
        Task<List<NoiDungMauKhamBenhLookupItemVo>> GetListNoiDungMauKhamBenhTheoBacSiAsync(DropDownListRequestModel queryInfo);
        void SaveCapNhatNgayDuongThai(long yeuCauKhamBenhId, DateTime tuNgay, DateTime denNgay);
        Task<string> PhieuNghiDuongThai(long yeuCauKhamBenhId);
        NgayDuongThaiYeuCauKhamBenhVo GetNgayDuongThaiYeuCauKhamBenh(long yeuCauKhamBenhId);
    }
}
