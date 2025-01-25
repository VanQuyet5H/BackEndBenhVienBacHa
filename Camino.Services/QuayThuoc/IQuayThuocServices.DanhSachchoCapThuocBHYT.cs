using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.QuayThuoc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.ThongTinCongTyBaoHiemTuNhan;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;

namespace Camino.Services.QuayThuoc
{
    public partial interface IQuayThuocService : IMasterFileService<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien>
    {
        GridDataSource GetDanhSachchoCapThuocBHYT(QueryInfo queryInfo, bool isPrint);
        GridDataSource GetTotalDanhSachchoCapThuocBHYT(QueryInfo queryInfo);
        byte[] ExportDanhSachChoCapThuocBHYT(ICollection<DonThuocThanhToanGridVo> datas);        
        GridDataSource GetDanhSachLichSuCapThuocBHYT(QueryInfo queryInfo, bool isPrint);
        GridDataSource GetTotalSachLichSuCapThuocBHYT(QueryInfo queryInfo);
    }
}
