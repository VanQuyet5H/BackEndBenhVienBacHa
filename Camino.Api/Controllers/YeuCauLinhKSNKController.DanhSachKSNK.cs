using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauKSNK;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Controllers
{
    public partial class YeuCauLinhKSNKController
    {
        #region Ds yeu cau linh duoc pham
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataDSLinhKSNKForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachYeuCauLinhKSNK)] // to do
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhKSNKService.GetDataForGridAsync(queryInfo, false);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageFDSLinhKSNKForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachYeuCauLinhKSNK)] // to do
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhKSNKService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion
        #region Ds yeu cau linh duoc pham child
        //[ApiExplorerSettings(IgnoreApi = true)]
        //[HttpPost("GetDataDSLinhKSNKChildForGridAsync")]
        ////[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucChuyenKhoa)] // to do
        //public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncChild
        //    ([FromBody]QueryInfo queryInfo)
        //{
        //    var gridData = await _yeuCauLinhKSNKService.GetDataForGridAsyncChild(queryInfo);
        //    return Ok(gridData);
        //}

        //[HttpPost("GetTotalPageFDSLinhDuocPhamorGridAsync")]
        ////[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucChuyenKhoa)] // to do
        //public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncChild
        //    ([FromBody]QueryInfo queryInfo)
        //{
        //    var gridData = await _yeuCauLinhKSNKService.GetTotalPageForGridAsyncChild(queryInfo);
        //    return Ok(gridData);
        //}
        #endregion

        #region In lĩnh duoc pham
        [HttpPost("InLinhKSNK")]
        public async Task<ActionResult<string>> InLinhKSNK([FromBody]XacNhanInLinhKSNK xacNhanInLinhKSNK)
        {
            var htmlLinhDuocPham = await _yeuCauLinhKSNKService.InLinhKSNK(xacNhanInLinhKSNK);
            return htmlLinhDuocPham;
        }
        #endregion
        #region YeuCau linh KSNK
        // child child
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataDSYeuCauLinhKSNKChildChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauLinhKSNK, Enums.DocumentType.DanhSachYeuCauLinhKSNK)]
        public async Task<ActionResult<GridDataSource>> GetDataDSYeuCauLinhKSNKChildChildForGridAsync
          ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhKSNKService.GetDataDSYeuCauLinhKSNKChildChildForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageFDSYeuCauLinhKSNKChildChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauLinhKSNK, Enums.DocumentType.DanhSachYeuCauLinhKSNK)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageFDSYeuCauLinhKSNKChildChildForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhKSNKService.GetTotalPageFDSYeuCauLinhKSNKChildChildForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion
        #region Ds duyet linh KSNK
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataDSDuyetLinhKSNKForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauLinhKSNK, Enums.DocumentType.DanhSachYeuCauLinhKSNK)]
        public async Task<ActionResult<GridDataSource>> GetDataDSDuyetLinhKSNKForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhKSNKService.GetDataDSDuyetKSNKForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageDSLinhKSNKForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauLinhKSNK, Enums.DocumentType.DanhSachYeuCauLinhKSNK)]
        public async Task<ActionResult<GridDataSource>> GetDSDuyetLinhDuocPhamForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhKSNKService.GetDSDuyetKSNKTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        // child
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataDSDuyetLinhKSNKChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauLinhKSNK, Enums.DocumentType.DanhSachYeuCauLinhKSNK)]
        public async Task<ActionResult<GridDataSource>> GetDataDSDuyetLinhKSNKChildForGridAsync
           ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhKSNKService.GetDataDSDuyetLinhKSNKChildForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageFDSLinhKSNKChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauLinhKSNK, Enums.DocumentType.DanhSachYeuCauLinhKSNK)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageFDSLinhKSNKChildForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhKSNKService.GetTotalPageFDSLinhKSNKChildForGridAsync(queryInfo);
            return Ok(gridData);
        }
        // child child
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataDSDuyetLinhKSNKChildChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauLinhKSNK, Enums.DocumentType.DanhSachYeuCauLinhKSNK)]
        public async Task<ActionResult<GridDataSource>> GetDataDSDuyetLinhKSNKChildChildForGridAsync
          ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhKSNKService.GetDataDSDuyetLinhKSNKChildChildForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageFDSLinhKSNKChildChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauLinhKSNK, Enums.DocumentType.DanhSachYeuCauLinhKSNK)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageFDSLinhKSNKChildChildForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhKSNKService.GetTotalPageFDSLinhKSNKChildChildForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion

        #region huy ds yeu cau linh   // to do 
        [HttpPost("HuyItemYeuCauLinhThuoc")]
        public async Task<ActionResult> Delete(long id)
        {
            var listHuy = await _yeuCauLinhKSNKService.GetByIdAsync(id, s => s.Include(x => x.YeuCauVatTuBenhViens).Include(k => k.YeuCauLinhVatTuChiTiets));
            if (listHuy == null)
            {
                return NotFound();
            }
            await _yeuCauLinhKSNKService.DeleteByIdAsync(id);
            foreach (var item in listHuy.YeuCauVatTuBenhViens)
            {
                var entity = _yeuCauVatTuBenhVienService.GetById(item.Id);
                entity.YeuCauLinhVatTuId = null;
                _yeuCauVatTuBenhVienService.Update(entity);
            }
            return NoContent();
        }
        #endregion
        [HttpPost("ExportDanhSachLinhKSNK")]
        public async Task<ActionResult> ExportDanhSachLinhKSNK(QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhKSNKService.GetDataForGridAsync(queryInfo, true);
            queryInfo.Sort[0].Dir = "asc";
            queryInfo.Sort[0].Field = "Id";
            if (gridData == null || gridData.Data.Count == 0)
            {
                return NoContent();
            }
            var datas = gridData.Data.Cast<DSLinhKSNKGridVo>().ToList();
            foreach (var dataDuTruLinh in datas)
            {
                if (dataDuTruLinh.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan) // linh tt
                {
                    queryInfo.AdditionalSearchString = dataDuTruLinh.Id.ToString() + '-' + (int)dataDuTruLinh.LoaiPhieuLinh + '-' + dataDuTruLinh.LinhVeKhoId + '-' + dataDuTruLinh.DuocDuyet;
                    var gridDataLinhBenhNhan = await _yeuCauLinhKSNKService.GetDataDSDuyetLinhKSNKChildForGridAsync(queryInfo);
                    dataDuTruLinh.ListChildLinhBenhNhan = gridDataLinhBenhNhan.Data.Cast<DSLinhVatChildTuGridVo>().ToList();
                    if (dataDuTruLinh.ListChildLinhBenhNhan.Count() > 0)
                    {
                        queryInfo.AdditionalSearchString = "";
                        foreach (var dataDuTruLinhChild in dataDuTruLinh.ListChildLinhBenhNhan)
                        {
                            queryInfo.AdditionalSearchString = dataDuTruLinhChild.Id.ToString() + '-' +
                                                              (int)dataDuTruLinhChild.LoaiPhieuLinh + '-' +
                                                              dataDuTruLinhChild.KSNKBenhVienId + '-' +
                                                              dataDuTruLinhChild.LaBHYT + '-' +
                                                              dataDuTruLinhChild.LinhveKhoId + '-' +
                                                              dataDuTruLinhChild.YeuCauTiepNhanId + '-' +
                                                              dataDuTruLinh.DuocDuyet + '-' +
                                                              dataDuTruLinhChild.KhoLinhId;
                            if (dataDuTruLinh.DuocDuyet != false)
                            {
                                var gridDataChildLinhBenhNhan = await _yeuCauLinhKSNKService.GetDataDSYeuCauLinhKSNKChildChildForGridAsync(queryInfo);
                                dataDuTruLinhChild.ListChildChildLinhBenhNhan = gridDataChildLinhBenhNhan.Data.Cast<DSLinhVatChildTuGridVo>().ToList();
                            }
                        }
                    }
                }
                if (dataDuTruLinh.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhBu) // linh bù
                {
                    queryInfo.AdditionalSearchString = dataDuTruLinh.Id.ToString() + '-' + (int)dataDuTruLinh.LoaiPhieuLinh + '-' + dataDuTruLinh.LinhVeKhoId + '-' + dataDuTruLinh.DuocDuyet;
                    var gridDataLinhBu = await _yeuCauLinhKSNKService.GetDataDSDuyetLinhKSNKChildForGridAsync(queryInfo);
                    dataDuTruLinh.ListChildLinhBu = gridDataLinhBu.Data.Cast<YeuCauLinhKSNKBuGridVo>().ToList();
                    if (dataDuTruLinh.ListChildLinhBu.Count() > 0)
                    {
                        queryInfo.AdditionalSearchString = "";
                        foreach (var dataDuTruLinhChild in dataDuTruLinh.ListChildLinhBu)
                        {
                            queryInfo.AdditionalSearchString = dataDuTruLinhChild.Id.ToString() + '-' +
                                                               (int)dataDuTruLinhChild.LoaiPhieuLinh + '-' +
                                                               dataDuTruLinhChild.KSNKBenhVienId + '-' +
                                                               dataDuTruLinhChild.LaBHYT + '-' +
                                                               dataDuTruLinhChild.LinhveKhoId + '-' +
                                                               dataDuTruLinh.DuocDuyet;
                            var gridDataChildLinhBenhNhan = await _yeuCauLinhKSNKService.GetDataDSYeuCauLinhKSNKChildChildForGridAsync(queryInfo);
                            dataDuTruLinhChild.ListChildChildLinhBu = gridDataChildLinhBenhNhan.Data.Cast<KSNKLinhBuCuaBNGridVos>().ToList();
                        }
                    }
                }
                if (dataDuTruLinh.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhDuTru) // linh du tru
                {
                    queryInfo.AdditionalSearchString = dataDuTruLinh.Id.ToString() + '-' + (int)dataDuTruLinh.LoaiPhieuLinh + '-' + dataDuTruLinh.LinhVeKhoId + '-' + dataDuTruLinh.DuocDuyet;
                    var gridDataLinhBenhNhan = await _yeuCauLinhKSNKService.GetDataDSDuyetLinhKSNKChildForGridAsync(queryInfo);
                    dataDuTruLinh.ListChildLinhDuTru = gridDataLinhBenhNhan.Data.Cast<DSLinhVatChildTuGridVo>().ToList();
                }
            }
            var bytes = _yeuCauLinhKSNKService.ExportDanhSachLayDuTruLinh(datas);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DanhSachYeuCauLinhKSNK" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        [HttpPost("ExportDanhSachDuyetLinhKSNK")]
        public async Task<ActionResult> ExportDanhSachDuyetLinhKSNK(QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhKSNKService.GetDataForGridAsync(queryInfo, true);
            queryInfo.Sort[0].Dir = "asc";
            queryInfo.Sort[0].Field = "Id";
            if (gridData == null || gridData.Data.Count == 0)
            {
                return NoContent();
            }
            var datas = gridData.Data.Cast<DSLinhKSNKGridVo>().ToList();
            foreach (var dataDuTruLinh in datas)
            {
                if (dataDuTruLinh.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan) // linh tt
                {
                    queryInfo.AdditionalSearchString = dataDuTruLinh.Id.ToString() + '-' + (int)dataDuTruLinh.LoaiPhieuLinh + '-' + dataDuTruLinh.LinhVeKhoId + '-' + dataDuTruLinh.DuocDuyet;
                    var gridDataLinhBenhNhan = await _yeuCauLinhKSNKService.GetDataDSDuyetLinhKSNKChildForGridAsync(queryInfo);
                    dataDuTruLinh.ListChildLinhBenhNhan = gridDataLinhBenhNhan.Data.Cast<DSLinhVatChildTuGridVo>().ToList();
                    if (dataDuTruLinh.ListChildLinhBenhNhan.Count() > 0)
                    {
                        queryInfo.AdditionalSearchString = "";
                        foreach (var dataDuTruLinhChild in dataDuTruLinh.ListChildLinhBenhNhan)
                        {
                            queryInfo.AdditionalSearchString = dataDuTruLinhChild.Id.ToString() + '-' +
                                                               (int)dataDuTruLinhChild.LoaiPhieuLinh + '-' +
                                                               dataDuTruLinhChild.KSNKBenhVienId + '-' +
                                                               dataDuTruLinhChild.LaBHYT + '-' +
                                                               dataDuTruLinhChild.LinhveKhoId + '-' +
                                                               dataDuTruLinhChild.YeuCauTiepNhanId + '-' +
                                                               dataDuTruLinh.DuocDuyet + '-' +
                                                               dataDuTruLinhChild.KhoLinhId;

                            var gridDataChildLinhBenhNhan = await _yeuCauLinhKSNKService.GetDataDSDuyetLinhKSNKChildChildForGridAsync(queryInfo);
                            dataDuTruLinhChild.ListChildChildLinhBenhNhan = gridDataChildLinhBenhNhan.Data.Cast<DSLinhVatChildTuGridVo>().ToList();
                        }
                    }
                }
                if (dataDuTruLinh.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhBu) // linh bù
                {
                    queryInfo.AdditionalSearchString = dataDuTruLinh.Id.ToString() + '-' + (int)dataDuTruLinh.LoaiPhieuLinh + '-' + dataDuTruLinh.LinhVeKhoId + '-' + dataDuTruLinh.DuocDuyet;
                    var gridDataLinhBu = await _yeuCauLinhKSNKService.GetDataDSDuyetLinhKSNKChildForGridAsync(queryInfo);
                    dataDuTruLinh.ListChildLinhBu = gridDataLinhBu.Data.Cast<YeuCauLinhKSNKBuGridVo>().ToList();
                    if (dataDuTruLinh.ListChildLinhBu.Count() > 0)
                    {
                        queryInfo.AdditionalSearchString = "";
                        foreach (var dataDuTruLinhChild in dataDuTruLinh.ListChildLinhBu)
                        {
                            queryInfo.AdditionalSearchString = dataDuTruLinhChild.Id.ToString() + '-' +
                                                               (int)dataDuTruLinhChild.LoaiPhieuLinh + '-' +
                                                               dataDuTruLinhChild.KSNKBenhVienId + '-' +
                                                               dataDuTruLinhChild.LaBHYT + '-' +
                                                               dataDuTruLinhChild.LinhveKhoId + '-' +
                                                               dataDuTruLinh.DuocDuyet;
                            var gridDataChildLinhBenhNhan = await _yeuCauLinhKSNKService.GetDataDSDuyetLinhKSNKChildChildForGridAsync(queryInfo);
                            dataDuTruLinhChild.ListChildChildLinhBu = gridDataChildLinhBenhNhan.Data.Cast<KSNKLinhBuCuaBNGridVos>().ToList();
                        }
                    }
                }
                if (dataDuTruLinh.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhDuTru) // linh du tru
                {
                    queryInfo.AdditionalSearchString = dataDuTruLinh.Id.ToString() + '-' + (int)dataDuTruLinh.LoaiPhieuLinh + '-' + dataDuTruLinh.LinhVeKhoId + '-' + dataDuTruLinh.DuocDuyet;
                    var gridDataLinhBenhNhan = await _yeuCauLinhKSNKService.GetDataDSDuyetLinhKSNKChildForGridAsync(queryInfo);
                    dataDuTruLinh.ListChildLinhDuTru = gridDataLinhBenhNhan.Data.Cast<DSLinhVatChildTuGridVo>().ToList();
                }
            }
            var bytes = _yeuCauLinhKSNKService.ExportDanhSachDuyetLayDuTruLinh(datas);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DanhSachDuyetLinhKSNK" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataDSYeuCauLinhDuocPhamKSNKChildForGridAsync")]
        public async Task<ActionResult<GridDataSource>> GetDataDSYeuCauLinhDuocPhamKSNKChildForGridAsync
           ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhKSNKService.GetDataDSYeuCauLinhDuocPhamKSNKChildForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageFDSYeuCauLinhDuocPhamKSNKChildForGridAsync")]
        public async Task<ActionResult<GridDataSource>> GetTotalPageFDSYeuCauLinhDuocPhamChildForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhKSNKService.GetTotalPageFDSYeuCauLinhDuocPhamKSNKChildForGridAsync(queryInfo);
            return Ok(gridData);
        }
    }
}
