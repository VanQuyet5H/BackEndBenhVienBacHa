using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.LinhDuocPham;
using Camino.Services.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Controllers
{
    public partial class YeuCauLinhDuocPhamController
    {
        #region Ds yeu cau linh duoc pham
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataDSLinhDuocPhamForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachYeuCauLinhDuocPham)] // to do
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhDuocPhamService.GetDataForGridAsync(queryInfo,false);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageFDSLinhDuocPhamForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachYeuCauLinhDuocPham)] // to do
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhDuocPhamService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion
        #region Ds yeu cau linh duoc pham child
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataDSYeuCauLinhDuocPhamChildForGridAsync")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucChuyenKhoa)] // to do
        public async Task<ActionResult<GridDataSource>> GetDataDSYeuCauLinhDuocPhamChildForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhDuocPhamService.GetDataDSYeuCauLinhDuocPhamChildForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageFDSYeuCauLinhDuocPhamChildForGridAsync")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucChuyenKhoa)] // to do
        public async Task<ActionResult<GridDataSource>> GetTotalPageFDSYeuCauLinhDuocPhamChildForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhDuocPhamService.GetTotalPageFDSYeuCauLinhDuocPhamChildForGridAsync(queryInfo);
            return Ok(gridData);
        }
        // child child
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataDSYeuCauLinhDuocPhamChildChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachYeuCauLinhDuocPham, Enums.DocumentType.DuyetYeuCauLinhDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetDataDSYeuCauLinhDuocPhamChildChildForGridAsync
          ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhDuocPhamService.GetDataDSYeuCauLinhDuocPhamChildChildForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageFDSYeuCauLinhDuocPhamChildChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachYeuCauLinhDuocPham, Enums.DocumentType.DuyetYeuCauLinhDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageFDSYeuCauLinhDuocPhamChildChildForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhDuocPhamService.GetTotalPageFDSYeuCauLinhDuocPhamChildChildForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion

        #region Ds duyệt linh duoc pham child
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataDSDuyetLinhDuocPhamChildForGridAsync")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucChuyenKhoa)] // to do
        public async Task<ActionResult<GridDataSource>> GetDataDSDuyetLinhDuocPhamChildForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhDuocPhamService.GetDataDSDuyetLinhDuocPhamChildForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageFDSLinhDuocPhamChildForGridAsync")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucChuyenKhoa)] // to do
        public async Task<ActionResult<GridDataSource>> GetTotalPageFDSLinhDuocPhamChildForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhDuocPhamService.GetTotalPageFDSLinhDuocPhamChildForGridAsync(queryInfo);
            return Ok(gridData);
        }
        // child child
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataDSDuyetLinhDuocPhamChildChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachYeuCauLinhDuocPham, Enums.DocumentType.DuyetYeuCauLinhDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetDataDSDuyetLinhDuocPhamChildChildForGridAsync
          ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhDuocPhamService.GetDataDSDuyetLinhDuocPhamChildChildForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageFDSLinhDuocPhamChildChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachYeuCauLinhDuocPham, Enums.DocumentType.DuyetYeuCauLinhDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageFDSLinhDuocPhamChildChildForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhDuocPhamService.GetTotalPageFDSLinhDuocPhamChildChildForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion

        #region In lĩnh duoc pham
        [HttpPost("InLinhDuocPham")]
        public async Task<ActionResult<string>> InLinhDuocPham([FromBody]XacNhanInLinhDuocPham xacNhanInLinhDuocPham)
        {
            var htmlLinhDuocPham = await _yeuCauLinhDuocPhamService.InLinhDuocPham(xacNhanInLinhDuocPham);
            return htmlLinhDuocPham;
        }
        #endregion
        #region Ds duyet linh duoc pham
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataDSDuyetLinhDuocPhamForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauLinhDuocPham)] 
        public async Task<ActionResult<GridDataSource>> GetDataDSDuyetLinhDuocPhamForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhDuocPhamService.GetDataDSDuyetDuocPhamForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageDSLinhDuocPhamForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauLinhDuocPham)]
        public async Task<ActionResult<GridDataSource>>  GetDSDuyetLinhDuocPhamForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhDuocPhamService.GetDSDuyetDuocPhamTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion

        #region huy ds yeu cau linh   // to do 
        [HttpPost("HuyItemYeuCauLinhThuoc")]
        public async Task<ActionResult> Delete(long id)
        {
            var listHuy = await _yeuCauLinhDuocPhamService.GetByIdAsync(id,s=>s.Include(x=>x.YeuCauDuocPhamBenhViens).Include(k=>k.YeuCauLinhDuocPhamChiTiets));
            if (listHuy == null)
            {
                return NotFound();
            }
            await _yeuCauLinhDuocPhamService.DeleteByIdAsync(id);
            foreach (var item in listHuy.YeuCauDuocPhamBenhViens)
            {
                var entity = _yeuCauDuocPhamBenhVienService.GetById(item.Id);
                entity.YeuCauLinhDuocPhamId = null;
                _yeuCauDuocPhamBenhVienService.Update(entity);
            }
            return NoContent();
        }
        #endregion
        [HttpPost("ExportDanhSachLinhDuocPham")]
        public async Task<ActionResult> ExportDanhSachLinhDuocPham(QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhDuocPhamService.GetDataForGridAsync(queryInfo, true);
            queryInfo.Sort[0].Dir = "asc";
            queryInfo.Sort[0].Field = "Id";
            if (gridData == null || gridData.Data.Count == 0)
            {
                return NoContent();
            }
            var datas = gridData.Data.Cast<DsLinhDuocPhamGridVo>().ToList();
            foreach (var dataDuTruLinh in datas)
            {
                if (dataDuTruLinh.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan) // linh tt
                {
                    queryInfo.AdditionalSearchString = dataDuTruLinh.Id.ToString() + '-' + (int)dataDuTruLinh.LoaiPhieuLinh + '-' + dataDuTruLinh.LinhVeKhoId + '-' + dataDuTruLinh.DuocDuyet + '-' + dataDuTruLinh.DaGui;
                    var gridDataLinhBenhNhan = await _yeuCauLinhDuocPhamService.GetDataDSDuyetLinhDuocPhamChildForGridAsync(queryInfo);
                    dataDuTruLinh.ListChildLinhBenhNhan = gridDataLinhBenhNhan.Data.Cast<DSLinhDuocPhamChildTuGridVo>().ToList();
                    if (dataDuTruLinh.ListChildLinhBenhNhan.Count() > 0)
                    {
                        queryInfo.AdditionalSearchString = "";
                        foreach (var dataDuTruLinhChild in dataDuTruLinh.ListChildLinhBenhNhan)
                        {
                            queryInfo.AdditionalSearchString = dataDuTruLinhChild.Id.ToString() + '-' +
                                                               (int)dataDuTruLinhChild.LoaiPhieuLinh + '-' +
                                                               dataDuTruLinhChild.DuocPhamBenhVienId + '-' +
                                                               dataDuTruLinhChild.LaBHYT + '-' +
                                                               dataDuTruLinhChild.LinhVeKhoId + '-' +
                                                               dataDuTruLinhChild.YeuCauTiepNhanId + '-' +
                                                               dataDuTruLinh.DuocDuyet + '-' +
                                                               dataDuTruLinhChild.KhoLinhId;
                            if (dataDuTruLinh.DuocDuyet != false)
                            {
                                var gridDataChildLinhBenhNhan = await _yeuCauLinhDuocPhamService.GetDataDSYeuCauLinhDuocPhamChildChildForGridAsync(queryInfo);
                                dataDuTruLinhChild.ListChildChildLinhBenhNhan = gridDataChildLinhBenhNhan.Data.Cast<DSLinhDuocPhamChildTuGridVo>().ToList();
                            }
                        }
                    }
                }
                if (dataDuTruLinh.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhBu) // linh bù
                {
                    queryInfo.AdditionalSearchString = dataDuTruLinh.Id.ToString() + '-' + (int)dataDuTruLinh.LoaiPhieuLinh + '-' + dataDuTruLinh.LinhVeKhoId + '-' + dataDuTruLinh.DuocDuyet;
                    var gridDataLinhBu = await _yeuCauLinhDuocPhamService.GetDataDSDuyetLinhDuocPhamChildForGridAsync(queryInfo);
                    dataDuTruLinh.ListChildLinhBu = gridDataLinhBu.Data.Cast<YeuCauLinhDuocPhamBuGridVo>().ToList();
                    if (dataDuTruLinh.ListChildLinhBu.Count() > 0)
                    {
                        queryInfo.AdditionalSearchString = "";
                        foreach (var dataDuTruLinhChild in dataDuTruLinh.ListChildLinhBu)
                        {
                            queryInfo.AdditionalSearchString = dataDuTruLinhChild.Id.ToString() + '-' +
                                                               (int)dataDuTruLinhChild.LoaiPhieuLinh + '-' +
                                                               dataDuTruLinhChild.DuocPhamBenhVienId + '-' +
                                                               dataDuTruLinhChild.LaBHYT + '-' +
                                                               dataDuTruLinhChild.LinhVeKhoId + '-' +
                                                               dataDuTruLinh.DuocDuyet;

                                var gridDataChildLinhBenhNhan = await _yeuCauLinhDuocPhamService.GetDataDSYeuCauLinhDuocPhamChildChildForGridAsync(queryInfo);
                                dataDuTruLinhChild.ListChildChildLinhBu = gridDataChildLinhBenhNhan.Data.Cast<DuocPhamLinhBuCuaBNGridVos>().ToList();
                        }
                    }
                }
                if (dataDuTruLinh.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhDuTru) // linh du tru
                {
                    queryInfo.AdditionalSearchString = dataDuTruLinh.Id.ToString() + '-' + (int)dataDuTruLinh.LoaiPhieuLinh + '-' + dataDuTruLinh.LinhVeKhoId + '-' + dataDuTruLinh.DuocDuyet;
                    var gridDataLinhBenhNhan = await _yeuCauLinhDuocPhamService.GetDataDSDuyetLinhDuocPhamChildForGridAsync(queryInfo);
                    dataDuTruLinh.ListChildLinhDuTru = gridDataLinhBenhNhan.Data.Cast<DSLinhDuocPhamChildTuGridVo>().ToList();
                }
            }
            var bytes = _yeuCauLinhDuocPhamService.ExportDanhSachLayDuTruLinh(datas);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DanhSachYeuCauLinhDuocPham" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        [HttpPost("ExportDanhSachDuyetLinhDuocPham")]
        public async Task<ActionResult> ExportDanhSachDuyetLinhDuocPham(QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhDuocPhamService.GetDataForGridAsync(queryInfo, true);
            queryInfo.Sort[0].Dir = "asc";
            queryInfo.Sort[0].Field = "Id";
            if (gridData == null || gridData.Data.Count == 0)
            {
                return NoContent();
            }
            var datas = gridData.Data.Cast<DsLinhDuocPhamGridVo>().ToList();
            foreach (var dataDuTruLinh in datas)
            {
                if (dataDuTruLinh.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan) // linh tt
                {
                    queryInfo.AdditionalSearchString = dataDuTruLinh.Id.ToString() + '-' + (int)dataDuTruLinh.LoaiPhieuLinh + '-' + dataDuTruLinh.LinhVeKhoId + '-' + dataDuTruLinh.DuocDuyet + '-' + dataDuTruLinh.DaGui;
                    var gridDataLinhBenhNhan = await _yeuCauLinhDuocPhamService.GetDataDSDuyetLinhDuocPhamChildForGridAsync(queryInfo);
                    dataDuTruLinh.ListChildLinhBenhNhan = gridDataLinhBenhNhan.Data.Cast<DSLinhDuocPhamChildTuGridVo>().ToList();
                    if (dataDuTruLinh.ListChildLinhBenhNhan.Count() > 0)
                    {
                        queryInfo.AdditionalSearchString = "";
                        foreach (var dataDuTruLinhChild in dataDuTruLinh.ListChildLinhBenhNhan)
                        {
                            queryInfo.AdditionalSearchString = dataDuTruLinhChild.Id.ToString() + '-' +
                                                               (int)dataDuTruLinhChild.LoaiPhieuLinh + '-' +
                                                               dataDuTruLinhChild.DuocPhamBenhVienId + '-' +
                                                               dataDuTruLinhChild.LaBHYT + '-' +
                                                               dataDuTruLinhChild.LinhVeKhoId + '-' +
                                                               dataDuTruLinhChild.YeuCauTiepNhanId + '-' +
                                                               dataDuTruLinh.DuocDuyet + '-' +
                                                               dataDuTruLinhChild.KhoLinhId;
                            if(dataDuTruLinh.DuocDuyet != false)
                            {
                                var gridDataChildLinhBenhNhan = await _yeuCauLinhDuocPhamService.GetDataDSDuyetLinhDuocPhamChildChildForGridAsync(queryInfo);
                                dataDuTruLinhChild.ListChildChildLinhBenhNhan = gridDataChildLinhBenhNhan.Data.Cast<DSLinhDuocPhamChildTuGridVo>().ToList();
                            }
                        }
                    }
                }
                if (dataDuTruLinh.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhBu) // linh bù
                {
                    queryInfo.AdditionalSearchString = dataDuTruLinh.Id.ToString() + '-' + (int)dataDuTruLinh.LoaiPhieuLinh + '-' + dataDuTruLinh.LinhVeKhoId + '-' + dataDuTruLinh.DuocDuyet;
                    var gridDataLinhBu = await _yeuCauLinhDuocPhamService.GetDataDSDuyetLinhDuocPhamChildForGridAsync(queryInfo);
                    dataDuTruLinh.ListChildLinhBu = gridDataLinhBu.Data.Cast<YeuCauLinhDuocPhamBuGridVo>().ToList();
                    if (dataDuTruLinh.ListChildLinhBu.Count() > 0)
                    {
                        queryInfo.AdditionalSearchString = "";
                        foreach (var dataDuTruLinhChild in dataDuTruLinh.ListChildLinhBu)
                        {
                            queryInfo.AdditionalSearchString = dataDuTruLinhChild.Id.ToString() + '-' +
                                                               (int)dataDuTruLinhChild.LoaiPhieuLinh + '-' +
                                                               dataDuTruLinhChild.DuocPhamBenhVienId + '-' +
                                                               dataDuTruLinhChild.LaBHYT + '-' +
                                                               dataDuTruLinhChild.LinhVeKhoId + '-' +
                                                               dataDuTruLinh.DuocDuyet;
                            var gridDataChildLinhBenhNhan = await _yeuCauLinhDuocPhamService.GetDataDSDuyetLinhDuocPhamChildChildForGridAsync(queryInfo);
                            dataDuTruLinhChild.ListChildChildLinhBu = gridDataChildLinhBenhNhan.Data.Cast<DuocPhamLinhBuCuaBNGridVos>().ToList();
                        }
                    }
                }
                if (dataDuTruLinh.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhDuTru) // linh du tru
                {
                    queryInfo.AdditionalSearchString = dataDuTruLinh.Id.ToString() + '-' + (int)dataDuTruLinh.LoaiPhieuLinh + '-' + dataDuTruLinh.LinhVeKhoId + '-' + dataDuTruLinh.DuocDuyet;
                    var gridDataLinhBenhNhan = await _yeuCauLinhDuocPhamService.GetDataDSDuyetLinhDuocPhamChildForGridAsync(queryInfo);
                    dataDuTruLinh.ListChildLinhDuTru = gridDataLinhBenhNhan.Data.Cast<DSLinhDuocPhamChildTuGridVo>().ToList();
                }
            }
            var bytes = _yeuCauLinhDuocPhamService.ExportDanhSachDuyetLayDuTruLinh(datas);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DanhSachDuyetLinhDuocPham" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}
