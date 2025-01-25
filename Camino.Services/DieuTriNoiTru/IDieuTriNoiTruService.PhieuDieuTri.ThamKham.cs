using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        Task<NgayDieuTri> GetNgayDieuTri(long yeuCauTiepNhanId);
        Task<NgayDieuTri> GetNgayDieuTriTamThoi(CreateNewDateModel createNewDateModel);


        Task<bool> IsExistsDate(long yeuCauTiepNhanId, List<DateTime> dates,long? khoaId);
        Task<bool> IsExistsDateTamThoi(List<DateTime> dates, List<DateTime> dateAdds);

        Task<bool> IsValidateRemoveDate(long yeuCauTiepNhanId, long phieuDieuTriId);
        Task CreateNewDate(long yeuCauTiepNhanId,long? khoaId, List<DateTime> dates);

        Task<YeuCauTiepNhan> GetYeuCauTiepNhanWithIncludeUpdate(long yeuCauTiepNhanId);

        Task<List<ICDPhieuDieuTriTemplateVo>> GetICD(DropDownListRequestModel model, bool coHienThiMa = false);
        Task<NoiTruPhieuDieuTri> GetNoiTruPhieuDieuTriById(long noiTruPhieuDieuTriId, Func<IQueryable<NoiTruPhieuDieuTri>, IIncludableQueryable<NoiTruPhieuDieuTri, object>> includes = null);
        Task<Core.Domain.Entities.DieuTriNoiTrus.NoiTruBenhAn> GetNoiTruBenhAnById(long noiTruBenhAnId, Func<IQueryable<Core.Domain.Entities.DieuTriNoiTrus.NoiTruBenhAn>, IIncludableQueryable<Core.Domain.Entities.DieuTriNoiTrus.NoiTruBenhAn, object>> includes = null);
        void SaveChanges();
        Task<List<LookupItemVo>> GetCheDoChamSoc(DropDownListRequestModel model);

        Task<List<PhieuDieuTriThuocGridVo>> GetDanhSachThuoc(long yeuCauTiepNhanId, long phieuDieuTriId);

        Task<List<PhieuDieuTriTruyenDichGridVo>> GetDanhSachTruyenDich(long yeuCauTiepNhanId, long phieuDieuTriId);

        //Task<List<PhieuDieuTriSuatAnGridVo>> GetDanhSachSuatAn(long yeuCauTiepNhanId, long phieuDieuTriId);

        Task<bool> IsMoreThanNow(DateTime? date, DateTime? thoiDiemChiDinh);
        int GetSoThuTuThuocGayNghien(long yeuCauTiepNhanId, long phieuDieuTriId, long duocPhamBenhVienId);

        Task<bool> IsTenICDKemTheoExists(long? idICD, long id, long noiTruPhieuDieuTriId);

        Task<string> GetCheDoAn(long? cheDoAnId);
        Task<string> GetCheDoChamSoc(long? cheCheDoChamSocId);
    }
}
