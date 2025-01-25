using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;

namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        #region Grid
        Task<GridDataSource> GetDataForGridTiepNhanNoiTruAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridTiepNhanNoiTruAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridTiepNhanNoiTruAsyncVer2(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridTiepNhanNoiTruAsyncVer2(QueryInfo queryInfo);

        #region Cập nhật 11/07/2022: fix grid load chậm
        Task<GridDataSource> GetDataForGridTiepNhanNoiTruAsyncVer3(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridTiepNhanNoiTruAsyncVer3(QueryInfo queryInfo);
        #endregion
        Task<GridDataSource> GetDataForGridTiepNhanNoiTruAsyncVer4(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridSoDoGiuongTiepNhanNoiTruAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridSoDoGiuongTiepNhanNoiTruAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataLichSuChuyenDoiTuongForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageLichSuChuyenDoiTuongForGridAsync(QueryInfo queryInfo);
        #endregion

        #region get data
        List<LookupItemVo> GetListLoaiBenhAn(DropDownListRequestModel queryInfo);

        #endregion

        #region thêm/xóa/sửa

        Task XuLyTaoBenhAnAsync(Core.Domain.Entities.DieuTriNoiTrus.NoiTruBenhAn noiTruBenhAn);
        Task XuLyCapNhatBenhAnAsync(YeuCauTiepNhan yeuCauTiepNhan, Enums.LoaiBenhAn loaiBenhAnTruocCapNhat);
        Task XuLyChiDinhEkipVaDichVuGiuongNoiTruAsync(ChiDinhEkipVaDichVuGiuongNoiTruTiepNhanVo yeuCauVo, YeuCauTiepNhan yeuCauTiepNhanChiTiet);

        Task KiemTraPhongChiDinhTiepNhanNoiTru(GiuongBenhTrongVo giuongBenhTrong);
        Task<List<LookupItemNoiTruCongTyBHTNVo>> GetCongTyBaoHiemTuNhanAsync(DropDownListRequestModel model);

        #endregion

        #region bệnh án sơ sinh
        Task<bool> KiemTraBenhAnMeCoConTrungTen(long? yeuCauTiepNhanMeId, string hoTen);
        Task<bool> KiemTraTaoBenhAnSoSinhAsync(long yeuCauTiepNhanId);
        Task GetThongTinTiepNhanBenhAnMeAsync(YeuCauTiepNhan yeuCauTiepNhan);
        Task XuLyTaoBenhAnSoSinhAsync(YeuCauTiepNhan yeuCauTiepNhanCon, long yeuCauTiepNhanMeId, long khoaChuyenBenhAnSoSinhVeId, DateTime lucDeSoSinh, long? yeuCauGoiDichVuId = null);
        Task<bool> KiemTraYeuCauGoiDichVuDaSuDungAsync(long? yeuCauGoiDichVuId, long? benhNhanId = null, bool isCheckBenhNhanHienTai = false, long? yeuCauTiepNhanMeId = null);
        Task<bool> KiemTraYeuCauGoiDichVuDaChiDinhChoConKhacAsync(long? yeuCauGoiDichVuId);
        Task<List<LookupItemVo>> GetYeuCauGoiDichVuSoSinhCuaMeAsync(DropDownListRequestModel model);
        Task<bool> KiemTraNgaySinhConVaThoiGianNhapVienMe(long yeuCauTiepNhanMeId, DateTime ngaySinhCon);
        #endregion


        #region Cập nhật chỉ định dịch vụ giường trong gói
        void GetDichVuGiuongTrongGoiTheoBenhNhan(BenhNhan benhNhan, DichVuGiuongTrongGoiVo dichVuInfo, YeuCauDichVuGiuongBenhVien yeuCauDichVuGiuong);


        #endregion

        #region kiểm tra sử dụng dịch vụ giường trong gói
        Task<List<ChiTietSuDungDichVuGiuongTrongGoiTheoBenhNhanVo>> GetThongTinSuDungDichVuGiuongTrongGoiAsync(long benhNhanId);


        #endregion
    }
}
