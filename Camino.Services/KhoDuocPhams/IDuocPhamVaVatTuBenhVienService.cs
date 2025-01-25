using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.KhoDuocPhams;

namespace Camino.Services.KhoDuocPhams
{
    public interface IDuocPhamVaVatTuBenhVienService
    {
        Task<List<DuocPhamVaVatTuTrongKhoVo>> GetDuocPhamVaVatTuTrongKho(bool laDuocPhamVatTuBHYT, string searchString, long khoId,
            int take);

        Task<List<DuocPhamVaVatTuTrongKhoVo>> GetDuocPhamVaVatTuTrongNhieuKho(bool laDuocPhamVatTuBHYT,
            string searchString, int take, params long[] khoIds);
        Task<List<DuocPhamVaVatTuTrongKhoVo>> GetDuocPhamVaVatTuTrongKhoTuTrucNhanVien(bool laDuocPhamVatTuBHYT, string searchString, long khoId, int take);
    }
}
