using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject.ExcelChungTu;
using Camino.Core.Domain.ValueObject.Grid;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.GoiBaoHiemYTe
{
    partial interface IGoiBaoHiemYTeService : IMasterFileService<YeuCauTiepNhan>
    {
        byte[] ExportGiayRaVien(ExcelChungTuQueryInfo excelChungTuQueryInfo);

        byte[] ExportGiayTomTatBenhAn(ExcelChungTuQueryInfo excelChungTuQueryInfo);

        byte[] ExportGiayNghiHuongBHXH(ExcelChungTuQueryInfo excelChungTuQueryInfo);

        byte[] ExportGiayNghiDuongThai(ExcelChungTuQueryInfo excelChungTuQueryInfo);

        byte[] ExportGiayChungSinh(ExcelChungTuQueryInfo excelChungTuQueryInfo);
    }
}
