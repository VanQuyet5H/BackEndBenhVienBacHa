using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.LinhDuocPham;
using Camino.Core.Domain.ValueObject.LinhVatTu;
using Camino.Services.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Camino.Api.Controllers
{
    public partial class YeuCauLinhVatTuController
    {
        #region Ds yeu cau linh duoc pham
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataDSLinhVatTuForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachYeuCauLinhVatTu)] // to do
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhVatTuService.GetDataForGridAsync(queryInfo,false);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageFDSLinhVatTuForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachYeuCauLinhVatTu)] // to do
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhVatTuService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion
        #region Ds yeu cau linh duoc pham child
        //[ApiExplorerSettings(IgnoreApi = true)]
        //[HttpPost("GetDataDSLinhVatTuChildForGridAsync")]
        ////[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucChuyenKhoa)] // to do
        //public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncChild
        //    ([FromBody]QueryInfo queryInfo)
        //{
        //    var gridData = await _yeuCauLinhVatTuService.GetDataForGridAsyncChild(queryInfo);
        //    return Ok(gridData);
        //}

        //[HttpPost("GetTotalPageFDSLinhDuocPhamorGridAsync")]
        ////[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucChuyenKhoa)] // to do
        //public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncChild
        //    ([FromBody]QueryInfo queryInfo)
        //{
        //    var gridData = await _yeuCauLinhVatTuService.GetTotalPageForGridAsyncChild(queryInfo);
        //    return Ok(gridData);
        //}
        #endregion

        #region In lĩnh duoc pham
        [HttpPost("InLinhVatTu")]
        public async Task<ActionResult<string>> InLinhVatTu([FromBody]XacNhanInLinhVatTu xacNhanInLinhVatTu)
        {
            var htmlLinhDuocPham = await _yeuCauLinhVatTuService.InLinhVatTu(xacNhanInLinhVatTu);
            return htmlLinhDuocPham;
        }
        #endregion
        #region YeuCau linh VatTu
        // child child
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataDSYeuCauLinhVatTuChildChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauLinhVatTu, Enums.DocumentType.DanhSachYeuCauLinhVatTu)]
        public async Task<ActionResult<GridDataSource>> GetDataDSYeuCauLinhVatTuChildChildForGridAsync
          ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhVatTuService.GetDataDSYeuCauLinhVatTuChildChildForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageFDSYeuCauLinhVatTuChildChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauLinhVatTu, Enums.DocumentType.DanhSachYeuCauLinhVatTu)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageFDSYeuCauLinhVatTuChildChildForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhVatTuService.GetTotalPageFDSYeuCauLinhVatTuChildChildForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion
        #region Ds duyet linh VatTu
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataDSDuyetLinhVatTuForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauLinhVatTu)]
        public async Task<ActionResult<GridDataSource>> GetDataDSDuyetLinhVatTuForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhVatTuService.GetDataDSDuyetVatTuForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageDSLinhVatTuForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauLinhVatTu)]
        public async Task<ActionResult<GridDataSource>> GetDSDuyetLinhDuocPhamForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhVatTuService.GetDSDuyetVatTuTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        // child
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataDSDuyetLinhVatTuChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauLinhVatTu, Enums.DocumentType.DanhSachYeuCauLinhVatTu)]
        public async Task<ActionResult<GridDataSource>> GetDataDSDuyetLinhVatTuChildForGridAsync
           ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhVatTuService.GetDataDSDuyetLinhVatTuChildForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageFDSLinhVatTuChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauLinhVatTu, Enums.DocumentType.DanhSachYeuCauLinhVatTu)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageFDSLinhVatTuChildForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhVatTuService.GetTotalPageFDSLinhVatTuChildForGridAsync(queryInfo);
            return Ok(gridData);
        }
        // child child
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataDSDuyetLinhVatTuChildChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauLinhVatTu, Enums.DocumentType.DanhSachYeuCauLinhVatTu)]
        public async Task<ActionResult<GridDataSource>> GetDataDSDuyetLinhVatTuChildChildForGridAsync
          ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhVatTuService.GetDataDSDuyetLinhVatTuChildChildForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageFDSLinhVatTuChildChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauLinhVatTu, Enums.DocumentType.DanhSachYeuCauLinhVatTu)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageFDSLinhVatTuChildChildForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhVatTuService.GetTotalPageFDSLinhVatTuChildChildForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion

        #region huy ds yeu cau linh   // to do 
        [HttpPost("HuyItemYeuCauLinhThuoc")]
        public async Task<ActionResult> Delete(long id)
        {
            var listHuy = await _yeuCauLinhVatTuService.GetByIdAsync(id, s => s.Include(x => x.YeuCauVatTuBenhViens).Include(k=>k.YeuCauLinhVatTuChiTiets));
            if (listHuy == null)
            {
                return NotFound();
            }
            await _yeuCauLinhVatTuService.DeleteByIdAsync(id);
            foreach (var item in listHuy.YeuCauVatTuBenhViens)
            {
                var entity = _yeuCauVatTuBenhVienService.GetById(item.Id);
                entity.YeuCauLinhVatTuId = null;
                _yeuCauVatTuBenhVienService.Update(entity);
            }
            return NoContent();
        }
        #endregion
        [HttpPost("ExportDanhSachLinhVatTu")]
        public async Task<ActionResult> ExportDanhSachLinhVatTu(QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhVatTuService.GetDataForGridAsync(queryInfo, true);
            queryInfo.Sort[0].Dir = "asc";
            queryInfo.Sort[0].Field = "Id";
            if (gridData == null || gridData.Data.Count == 0)
            {
                return NoContent();
            }
            var datas = gridData.Data.Cast<DSLinhVatTuGridVo>().ToList();
            foreach (var dataDuTruLinh in datas)
            {
                if (dataDuTruLinh.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan) // linh tt
                {
                    queryInfo.AdditionalSearchString = dataDuTruLinh.Id.ToString() + '-' + (int)dataDuTruLinh.LoaiPhieuLinh + '-' + dataDuTruLinh.LinhVeKhoId + '-' + dataDuTruLinh.DuocDuyet;
                    var gridDataLinhBenhNhan = await _yeuCauLinhVatTuService.GetDataDSDuyetLinhVatTuChildForGridAsync(queryInfo);
                    dataDuTruLinh.ListChildLinhBenhNhan = gridDataLinhBenhNhan.Data.Cast<DSLinhVatChildTuGridVo>().ToList();
                    if (dataDuTruLinh.ListChildLinhBenhNhan.Count() > 0)
                    {
                        queryInfo.AdditionalSearchString = "";
                        foreach (var dataDuTruLinhChild in dataDuTruLinh.ListChildLinhBenhNhan)
                        {
                            queryInfo.AdditionalSearchString = dataDuTruLinhChild.Id.ToString() + '-' +
                                                              (int)dataDuTruLinhChild.LoaiPhieuLinh + '-' +
                                                              dataDuTruLinhChild.VatTuBenhVienId + '-' +
                                                              dataDuTruLinhChild.LaBHYT + '-' +
                                                              dataDuTruLinhChild.LinhveKhoId + '-' +
                                                              dataDuTruLinhChild.YeuCauTiepNhanId + '-' +
                                                              dataDuTruLinh.DuocDuyet + '-' +
                                                              dataDuTruLinhChild.KhoLinhId;
                            if (dataDuTruLinh.DuocDuyet != false)
                            {
                                var gridDataChildLinhBenhNhan = await _yeuCauLinhVatTuService.GetDataDSYeuCauLinhVatTuChildChildForGridAsync(queryInfo);
                                dataDuTruLinhChild.ListChildChildLinhBenhNhan = gridDataChildLinhBenhNhan.Data.Cast<DSLinhVatChildTuGridVo>().ToList();
                            }
                        }
                    }
                }
                if (dataDuTruLinh.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhBu) // linh bù
                {
                    queryInfo.AdditionalSearchString = dataDuTruLinh.Id.ToString() + '-' + (int)dataDuTruLinh.LoaiPhieuLinh + '-' + dataDuTruLinh.LinhVeKhoId + '-' + dataDuTruLinh.DuocDuyet;
                    var gridDataLinhBu = await _yeuCauLinhVatTuService.GetDataDSDuyetLinhVatTuChildForGridAsync(queryInfo);
                    dataDuTruLinh.ListChildLinhBu = gridDataLinhBu.Data.Cast<YeuCauLinhVatTuBuGridVo>().ToList();
                    if (dataDuTruLinh.ListChildLinhBu.Count() > 0)
                    {
                        queryInfo.AdditionalSearchString = "";
                        foreach (var dataDuTruLinhChild in dataDuTruLinh.ListChildLinhBu)
                        {
                            queryInfo.AdditionalSearchString = dataDuTruLinhChild.Id.ToString() + '-' +
                                                               (int)dataDuTruLinhChild.LoaiPhieuLinh + '-' +
                                                               dataDuTruLinhChild.VatTuBenhVienId + '-' +
                                                               dataDuTruLinhChild.LaBHYT + '-' +
                                                               dataDuTruLinhChild.LinhveKhoId + '-' +
                                                               dataDuTruLinh.DuocDuyet;
                                var gridDataChildLinhBenhNhan = await _yeuCauLinhVatTuService.GetDataDSYeuCauLinhVatTuChildChildForGridAsync(queryInfo);
                                dataDuTruLinhChild.ListChildChildLinhBu = gridDataChildLinhBenhNhan.Data.Cast<VatTuLinhBuCuaBNGridVos>().ToList();
                        }
                    }
                }
                if (dataDuTruLinh.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhDuTru) // linh du tru
                {
                    queryInfo.AdditionalSearchString = dataDuTruLinh.Id.ToString() + '-' + (int)dataDuTruLinh.LoaiPhieuLinh + '-' + dataDuTruLinh.LinhVeKhoId + '-' + dataDuTruLinh.DuocDuyet;
                    var gridDataLinhBenhNhan = await _yeuCauLinhVatTuService.GetDataDSDuyetLinhVatTuChildForGridAsync(queryInfo);
                    dataDuTruLinh.ListChildLinhDuTru = gridDataLinhBenhNhan.Data.Cast<DSLinhVatChildTuGridVo>().ToList();
                }
            }
            var bytes = _yeuCauLinhVatTuService.ExportDanhSachLayDuTruLinh(datas);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DanhSachYeuCauLinhVatTu" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        [HttpPost("ExportDanhSachDuyetLinhVatTu")]
        public async Task<ActionResult> ExportDanhSachDuyetLinhVatTu(QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhVatTuService.GetDataForGridAsync(queryInfo, true);
            queryInfo.Sort[0].Dir = "asc";
            queryInfo.Sort[0].Field = "Id";
            if (gridData == null || gridData.Data.Count == 0)
            {
                return NoContent();
            }
            var datas = gridData.Data.Cast<DSLinhVatTuGridVo>().ToList();
            foreach (var dataDuTruLinh in datas)
            {
                if (dataDuTruLinh.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan) // linh tt
                {
                    queryInfo.AdditionalSearchString = dataDuTruLinh.Id.ToString() + '-' + (int)dataDuTruLinh.LoaiPhieuLinh + '-' + dataDuTruLinh.LinhVeKhoId + '-' + dataDuTruLinh.DuocDuyet;
                    var gridDataLinhBenhNhan = await _yeuCauLinhVatTuService.GetDataDSDuyetLinhVatTuChildForGridAsync(queryInfo);
                    dataDuTruLinh.ListChildLinhBenhNhan = gridDataLinhBenhNhan.Data.Cast<DSLinhVatChildTuGridVo>().ToList();
                    if (dataDuTruLinh.ListChildLinhBenhNhan.Count() > 0)
                    {
                        queryInfo.AdditionalSearchString = "";
                        foreach (var dataDuTruLinhChild in dataDuTruLinh.ListChildLinhBenhNhan)
                        {
                            queryInfo.AdditionalSearchString = dataDuTruLinhChild.Id.ToString() + '-' +
                                                               (int)dataDuTruLinhChild.LoaiPhieuLinh + '-' +
                                                               dataDuTruLinhChild.VatTuBenhVienId + '-' +
                                                               dataDuTruLinhChild.LaBHYT + '-' +
                                                               dataDuTruLinhChild.LinhveKhoId + '-' +
                                                               dataDuTruLinhChild.YeuCauTiepNhanId + '-' +
                                                               dataDuTruLinh.DuocDuyet + '-' +
                                                               dataDuTruLinhChild.KhoLinhId;

                            var gridDataChildLinhBenhNhan = await _yeuCauLinhVatTuService.GetDataDSDuyetLinhVatTuChildChildForGridAsync(queryInfo);
                            dataDuTruLinhChild.ListChildChildLinhBenhNhan = gridDataChildLinhBenhNhan.Data.Cast<DSLinhVatChildTuGridVo>().ToList();
                        }
                    }
                }
                if (dataDuTruLinh.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhBu) // linh bù
                {
                    queryInfo.AdditionalSearchString = dataDuTruLinh.Id.ToString() + '-' + (int)dataDuTruLinh.LoaiPhieuLinh + '-' + dataDuTruLinh.LinhVeKhoId + '-' + dataDuTruLinh.DuocDuyet;
                    var gridDataLinhBu = await _yeuCauLinhVatTuService.GetDataDSDuyetLinhVatTuChildForGridAsync(queryInfo);
                    dataDuTruLinh.ListChildLinhBu = gridDataLinhBu.Data.Cast<YeuCauLinhVatTuBuGridVo>().ToList();
                    if (dataDuTruLinh.ListChildLinhBu.Count() > 0)
                    {
                        queryInfo.AdditionalSearchString = "";
                        foreach (var dataDuTruLinhChild in dataDuTruLinh.ListChildLinhBu)
                        {
                            queryInfo.AdditionalSearchString = dataDuTruLinhChild.Id.ToString() + '-' +
                                                               (int)dataDuTruLinhChild.LoaiPhieuLinh + '-' +
                                                               dataDuTruLinhChild.VatTuBenhVienId + '-' +
                                                               dataDuTruLinhChild.LaBHYT + '-' +
                                                               dataDuTruLinhChild.LinhveKhoId + '-' +
                                                               dataDuTruLinh.DuocDuyet;
                            var gridDataChildLinhBenhNhan = await _yeuCauLinhVatTuService.GetDataDSDuyetLinhVatTuChildChildForGridAsync(queryInfo);
                            dataDuTruLinhChild.ListChildChildLinhBu = gridDataChildLinhBenhNhan.Data.Cast<VatTuLinhBuCuaBNGridVos>().ToList();
                        }
                    }
                }
                if (dataDuTruLinh.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhDuTru) // linh du tru
                {
                    queryInfo.AdditionalSearchString = dataDuTruLinh.Id.ToString() + '-' + (int)dataDuTruLinh.LoaiPhieuLinh + '-' + dataDuTruLinh.LinhVeKhoId + '-' + dataDuTruLinh.DuocDuyet;
                    var gridDataLinhBenhNhan = await _yeuCauLinhVatTuService.GetDataDSDuyetLinhVatTuChildForGridAsync(queryInfo);
                    dataDuTruLinh.ListChildLinhDuTru = gridDataLinhBenhNhan.Data.Cast<DSLinhVatChildTuGridVo>().ToList();
                }
            }
            var bytes = _yeuCauLinhVatTuService.ExportDanhSachDuyetLayDuTruLinh(datas);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DanhSachDuyetLinhVatTu" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}
