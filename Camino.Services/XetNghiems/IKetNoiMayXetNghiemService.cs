using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.MayXetNghiems;

namespace Camino.Services.XetNghiems
{
    public interface IKetNoiMayXetNghiemService : IPhienXetNghiemBaseService
    {
        Task<List<MayXetNghiem>> GetDanhSachMayXetNghiem();
        Task<List<string>> GetDanhSachChiSoXetNghiem(string barCodeNumber, long mauMayXetNghiemId);
    }
}
