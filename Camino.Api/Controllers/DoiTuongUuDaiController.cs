using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Models.DoiTuongUuDais;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Services.DoiTuongUuDais;
using Camino.Services.Localization;
using Microsoft.AspNetCore.Mvc;
using Camino.Core.Domain.Entities.DoiTuongUuDais;
using Camino.Api.Models.Error;
using Camino.Core.Domain.ValueObject.DoiTuongUuDais;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;

namespace Camino.Api.Controllers
{
    public class DoiTuongUuDaiController : CaminoBaseController
    {
        private readonly ILocalizationService  _localizationService;
        private IDoiTuongUuDaiService _doiTuongUuDaiService;
        private readonly IExcelService _excelService;
        public DoiTuongUuDaiController(ILocalizationService localizationService, IDoiTuongUuDaiService doiTuongUuDaiService, IExcelService excelService)
        {
             _localizationService = localizationService;
            _doiTuongUuDaiService = doiTuongUuDaiService;
            _excelService = excelService;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DoiTuongUuDaiDichVuKyThuat)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _doiTuongUuDaiService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DoiTuongUuDaiDichVuKyThuat)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _doiTuongUuDaiService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DoiTuongUuDaiDichVuKyThuat)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridChildAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _doiTuongUuDaiService.GetTotalPageForGridChildAsync(queryInfo);
            return Ok(gridData);
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DoiTuongUuDaiDichVuKyThuat)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridChildAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _doiTuongUuDaiService.GetDataForGridChildAsync(queryInfo);
            return Ok(gridData);
        }
        [HttpPost("GetDichVuKyThuat")]
        public async Task<ActionResult> GetDichVuKyThuat([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _doiTuongUuDaiService.GetDichVuKyThuat(queryInfo);
            return Ok(lookup);
        }
        [HttpPost("GetDichVuKyThuatById")]
        public async Task<ActionResult> GetDichVuKyThuatById([FromBody]DropDownListRequestModel queryInfo, long id)
        {
            var lookup = await _doiTuongUuDaiService.GetDichVuKyThuatById(queryInfo, id);
            return Ok(lookup);
        }

        [HttpPost("GetDoiTuong")]
        public async Task<ActionResult> GetDoiTuong()
        {
            var lookup = await _doiTuongUuDaiService.GetDoiTuong();
            return Ok(lookup);
        }
        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DoiTuongUuDaiDichVuKyThuat)]
        public async Task<ActionResult> Post([FromBody] DoiTuongUuDaiDichVuKyThuatAddViewModel doiTuongViewModel)
        {
            int count = 0;
            for (int i = 0; i < doiTuongViewModel.DoiTuongUuDai.Count; i++)
            {
                for (int j = 0; j < doiTuongViewModel.DoiTuongUuDai.Count; j++)
                {
                    if (doiTuongViewModel.DoiTuongUuDai[i].DoiTuongId == doiTuongViewModel.DoiTuongUuDai[j].DoiTuongId)
                    {
                        count++;
                    }
                }
                if (count > 1) {
                    throw new ApiException(_localizationService.GetResource("DoiTuongUuDai.Denied"));
                }
                count = 0;


            }
            foreach(var dichVuKyThuatBenhVienId in doiTuongViewModel.ListDichVuKyThuatBenhVienId)
            {
                var check = await _doiTuongUuDaiService.CheckDichVuKyThuatExist(Convert.ToInt64(dichVuKyThuatBenhVienId));
                if (check == false)
                {
                    throw new ApiException(_localizationService.GetResource("DoiTuongUuDai.DeleteDichVu"));
                }
                var checkActive = await _doiTuongUuDaiService.CheckDichVuKyThuatActive(Convert.ToInt64(dichVuKyThuatBenhVienId));
                if (checkActive == false)
                {
                    throw new ApiException(_localizationService.GetResource("DoiTuongUuDaiKyThuat.NotAcitve"));
                }
            }
            foreach (var dichVuKyThuatBenhVienId in doiTuongViewModel.ListDichVuKyThuatBenhVienId)
            {
                for (int i = 0; i < doiTuongViewModel.DoiTuongUuDai.Count; i++)
                {
                    var result = new DoiTuongUuDaiDichVuKyThuatBenhVien()
                    {
                        DoiTuongUuDaiId = Convert.ToInt64(doiTuongViewModel.DoiTuongUuDai[i].DoiTuongId),
                        DichVuKyThuatBenhVienId = Convert.ToInt64(dichVuKyThuatBenhVienId),
                        TiLeUuDai = Convert.ToInt32(doiTuongViewModel.DoiTuongUuDai[i].TiLeUuDai)
                    };
                    var CheckDaTao = await _doiTuongUuDaiService.CheckDichVuKyThuatExit(result.DoiTuongUuDaiId, result.DichVuKyThuatBenhVienId);
                    if (CheckDaTao == true)
                    {
                        throw new ApiException(_localizationService.GetResource("DoiTuongUuDai.Exit"));
                    }
                    await _doiTuongUuDaiService.AddDoiTuongEntity(result);
                }
            }
                
            return Ok();
        }
        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DoiTuongUuDaiDichVuKyThuat)]
        public async Task<ActionResult> Delete(long id)
        {
            await _doiTuongUuDaiService.DeleteDoiTuongDichVuKyThuat(id);
            return NoContent();
        }
        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DoiTuongUuDaiDichVuKyThuat)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<DoiTuongUuDaiDichVuKyThuatAddViewModel>> Get(long id)
        {
            var result = new DoiTuongUuDaiDichVuKyThuatAddViewModel();
            var lstdoiTuongUudai = new List<DoiTuongUuDaiViewModel>();
           
            var entity = await _doiTuongUuDaiService.GetData(id);
            if(entity != null) {
                result.DichVuKyThuatBenhVienId =entity.First().DichVuKyThuatBenhVienId;
                result.DichVuKyThuatModelText = await _doiTuongUuDaiService.GetNameDichVuKyThuat(entity.First().DichVuKyThuatBenhVienId);
                for (int i = 0; i < entity.Count; i++)
                {
                    var doiTuongUudai = new DoiTuongUuDaiViewModel();
                    doiTuongUudai.DoiTuongId = entity[i].DoiTuongUuDaiId;
                    doiTuongUudai.Ten = entity[i].DoiTuongUuDai.Ten;
                    doiTuongUudai.TiLeUuDai = entity[i].TiLeUuDai;
                    doiTuongUudai.DoiTuongOld = entity[i].DoiTuongUuDaiId;
                    lstdoiTuongUudai.Add(doiTuongUudai);
                }
                result.DoiTuongUuDai = new List<DoiTuongUuDaiViewModel>();
                result.DoiTuongUuDai.AddRange(lstdoiTuongUudai);
            }
            return Ok(result);
        }

        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DoiTuongUuDaiDichVuKyThuat)]
        public async Task<ActionResult> Put([FromBody] DoiTuongUuDaiDichVuKyThuatAddViewModel viewModel)
        {
            int count = 0;
            for (int i = 0; i < viewModel.DoiTuongUuDai.Count; i++)
            {
                for (int j = 0; j < viewModel.DoiTuongUuDai.Count; j++)
                {
                    if (viewModel.DoiTuongUuDai[i].DoiTuongId == viewModel.DoiTuongUuDai[j].DoiTuongId)
                    {
                        count++;
                    }
                }
                if (count > 1)
                {
                    throw new ApiException(_localizationService.GetResource("DoiTuongUuDai.Denied"));
                }
                count = 0;


            }
            foreach (var dichVuKyThuatBenhVienId in viewModel.ListDichVuKyThuatBenhVienId)
            {
                var check = await _doiTuongUuDaiService.CheckDichVuKyThuatExist(Convert.ToInt64(viewModel.DichVuKyThuatBenhVienId));
                if (check == false)
                {
                    throw new ApiException(_localizationService.GetResource("DoiTuongUuDai.DeleteDichVu"));
                }
                var checkActive = await _doiTuongUuDaiService.CheckDichVuKyThuatActive(Convert.ToInt64(viewModel.DichVuKyThuatBenhVienId));
                if (checkActive == false)
                {
                    throw new ApiException(_localizationService.GetResource("DoiTuongUuDaiKyThuat.NotAcitve"));
                }
                await _doiTuongUuDaiService.DeleteToAdd(Convert.ToInt64(viewModel.DichVuKyThuatBenhVienOld));
            }
            foreach (var dichVuKyThuatBenhVienId in viewModel.ListDichVuKyThuatBenhVienId)
            {
                for (int i = 0; i < viewModel.DoiTuongUuDai.Count; i++)
                {
                    var result = new DoiTuongUuDaiDichVuKyThuatBenhVien()
                    {
                        DoiTuongUuDaiId = Convert.ToInt64(viewModel.DoiTuongUuDai[i].DoiTuongId),
                        DichVuKyThuatBenhVienId = Convert.ToInt64(dichVuKyThuatBenhVienId),
                        TiLeUuDai = Convert.ToInt32(viewModel.DoiTuongUuDai[i].TiLeUuDai)
                    };
                    await _doiTuongUuDaiService.AddDoiTuongEntity(result);
                }
            }
            return NoContent();
        }

        [HttpPost("ExportDoiTuongUuDaiDichVuKyThuat")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DoiTuongUuDaiDichVuKyThuat)]
        public async Task<ActionResult> ExportDoiTuongUuDaiDichVuKyThuat(QueryInfo queryInfo)
        {
            // todo: hardcode max row export excel
            queryInfo.Skip = 0;
            queryInfo.Take = 20000;

            var gridData = await _doiTuongUuDaiService.GetDataForGridAsync(queryInfo);
            var data = gridData.Data.Select(p => (DoiTuongUuDaiGridVo)p).ToList();
            var dataExcel = data.Map<List<DoiTuongUuDaiDichVuKyThuatExportExcel>>();

            queryInfo.Sort = new List<Sort>();
            queryInfo.Sort.Add(new Sort()
            {
                Field = "DoiTuong",
                Dir = "asc"
            });
            foreach (var item in dataExcel)
            {
                var gridChildData = await _doiTuongUuDaiService.GetDataForGridChildAsync(queryInfo, item.DichVuKyThuatId);
                var dataChild = gridChildData.Data.Select(p => (DoiTuongUuDaiChildGridVo)p).ToList();
                var dataChildExcel = dataChild.Map<List<DoiTuongUuDaiDichVuKyThuatExportExcelChild>>();
                item.DoiTuongUuDaiDichVuKyThuatExportExcelChild.AddRange(dataChildExcel);
            }

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(DoiTuongUuDaiDichVuKyThuatExportExcel.Ma), "Mã"));
            lstValueObject.Add((nameof(DoiTuongUuDaiDichVuKyThuatExportExcel.Ma4350), "Mã 4350"));
            lstValueObject.Add((nameof(DoiTuongUuDaiDichVuKyThuatExportExcel.Ten), "Tên"));
            lstValueObject.Add((nameof(DoiTuongUuDaiDichVuKyThuatExportExcel.DoiTuongUuDaiDichVuKyThuatExportExcelChild), ""));


            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Đối tượng ưu đãi dịch vụ kỹ thuật");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DoiTuongUuDaiDichVuKyThuat" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";


            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}