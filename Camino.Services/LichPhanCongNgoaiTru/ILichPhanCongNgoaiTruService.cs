using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.KhoaPhong;
using Camino.Core.Domain.ValueObject.LichPhanCongNgoaiTru;

namespace Camino.Services.LichPhanCongNgoaiTru
{
    public interface ILichPhanCongNgoaiTruService
        : IMasterFileService<Core.Domain.Entities.LichPhanCongNgoaiTrus.LichPhanCongNgoaiTru>
    {
        Task<List<Core.Domain.Entities.LichPhanCongNgoaiTrus.LichPhanCongNgoaiTru>> GetListLichPhanCong(List<long> listIds, DateTime fromDate, DateTime toDate);

        List<LichTuanGridVo> GetDataForTuanAsync(DateTime date);

        Task<List<LichPhanCongNgoaiTruGridVo>> XepLich(DateTime date, int khoaId);

        Task<List<KhoaPhongTemplateVo>> GetListKhoaPhong(DropDownListRequestModel model);

        Task<List<Core.Domain.Entities.LichPhanCongNgoaiTrus.LichPhanCongNgoaiTru>> GetListLichLastWeek(List<long> listIds);

        Task<bool> IsNhanVienIdExists(List<long> listIdValidator, long id);

    }
}