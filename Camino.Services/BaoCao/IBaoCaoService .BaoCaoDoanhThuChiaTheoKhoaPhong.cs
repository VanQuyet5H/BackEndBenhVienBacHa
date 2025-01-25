using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.BaoCao
{
    public partial interface IBaoCaoService
    {
        #region DOANH THU CHIA THEO KHOA PHÒNG 
        Task<List<BaoCaoDoanhThuChiaTheoKhoaPhongGridVo>> GetDataDoanhThuChiaTheoKhoaPhongForGridAsync(BaoCaoDoanhThuChiaTheoKhoaPhongQueryInfo queryInfo);
        Task<SumBaoCaoDoanhThuChiaTheoKhoaPhongGridVo> GetTotalBaoCaoDoanhThuChiaTheoKhoaPhongForGridAsync(BaoCaoDoanhThuChiaTheoKhoaPhongQueryInfo queryInfo);
        byte[] ExportDoanhThuChiaTheoKhoaPhong(List<BaoCaoDoanhThuChiaTheoKhoaPhongGridVo> dataSource, BaoCaoDoanhThuChiaTheoKhoaPhongQueryInfo query, SumBaoCaoDoanhThuChiaTheoKhoaPhongGridVo total);
        #endregion

        #region BÁO CÁO TỔNG HỢP DOANH THU THEO NGUỒN BỆNH NHÂN 
        Task<GridDataSource> GetDataTongHopDoanhThuTheoNguonBenhNhanForGridAsync(BaoCaoTongHopDoanhThuTheoNguonBenhNhanQueryInfo queryInfo);
        Task<SumTongHopDoanhThuTheoNguonBenhNhanGridVo> GetTotalBaoCaoDoanhThuongHopDoanhThuTheoNguonBenhNhanForGridAsync(BaoCaoTongHopDoanhThuTheoNguonBenhNhanQueryInfo queryInfo);
        byte[] ExportDoanhThuongHopDoanhThuTheoNguonBenhNhan(List<BaoCaoTongHopDoanhThuTheoNguonBenhNhanGridVo> dataSource, BaoCaoTongHopDoanhThuTheoNguonBenhNhanQueryInfo query);
        #endregion
    }
}
