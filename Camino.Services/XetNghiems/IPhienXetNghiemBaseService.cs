using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.DichVuXetNghiems;
using Camino.Core.Domain.Entities.XetNghiems;
using Camino.Core.Domain.ValueObject.XetNghiems;

namespace Camino.Services.XetNghiems
{
    public interface IPhienXetNghiemBaseService : IMasterFileService<PhienXetNghiem>
    {
        Task DuyetPhieuGuiMauXetNghiem(long phieuGuiMauXetNghiemId, long nhanVienNhanMauId);
        Task DuyetYeuCauChayLaiXetNghiem(long yeuCauChayLaiXetNghiemId, long nhanVienDuyetId);
        List<KetQuaXetNghiemTruocVo> GetKetQuaXetNghiemTruocCuaBenhNhan(long benhNhanId);
        void AddKetQuaXetNghiemChiTiet(PhienXetNghiemChiTiet phienXetNghiemChiTiet, Core.Domain.Entities.DichVuXetNghiems.DichVuXetNghiem dichVuXetNghiem, BenhNhan benhNhan, List<Core.Domain.Entities.DichVuXetNghiems.DichVuXetNghiem> dichVuXetNghiems, List<DichVuXetNghiemKetNoiChiSo> dichVuXetNghiemKetNoiChiSos, List<KetQuaXetNghiemTruocVo> ketQuaXetNghiemLanTruocs = null);
    }
}
