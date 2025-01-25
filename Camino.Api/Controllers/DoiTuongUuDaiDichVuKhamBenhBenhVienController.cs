using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Models.DoiTuongUuDais;
using Camino.Api.Models.Error;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DoiTuongUuDais;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DoiTuongUuDais;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Services.DoiTuongUuDais;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{
    public class DoiTuongUuDaiDichVuKhamBenhBenhVienController : CaminoBaseController
    {
        private readonly ILocalizationService _localizationService;
        private IDoiTuongUuDaiService _doiTuongUuDaiService;
        private IExcelService _excelService;
        public DoiTuongUuDaiDichVuKhamBenhBenhVienController(ILocalizationService localizationService, IDoiTuongUuDaiService doiTuongUuDaiService, IExcelService excelService)
        {
            _localizationService = localizationService;
            _doiTuongUuDaiService = doiTuongUuDaiService;
            _excelService = excelService;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DoiTuongUuDaiDichVuKhamBenh)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _doiTuongUuDaiService.GetDataForGridBenhVienAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DoiTuongUuDaiDichVuKhamBenh)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _doiTuongUuDaiService.GetTotalPageForGridBenhVienAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DoiTuongUuDaiDichVuKhamBenh)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridChildAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _doiTuongUuDaiService.GetTotalPageForGridBenhVienChildAsync(queryInfo);
            return Ok(gridData);
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DoiTuongUuDaiDichVuKhamBenh)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridChildAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _doiTuongUuDaiService.GetDataForGridBenhVienChildAsync(queryInfo);
            return Ok(gridData);
        }
        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DoiTuongUuDaiDichVuKhamBenh)]
        public async Task<ActionResult> Delete(long id)
        {
            await _doiTuongUuDaiService.DeleteDoiTuongDichVuKhamBenh(id);
            return NoContent();
        }
        [HttpPost("GetDichVuKhamBenh")]
        public async Task<ActionResult> GetDichVuKyThuat([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _doiTuongUuDaiService.GetDichVuKhamBenh(queryInfo);
            return Ok(lookup);
        }
        [HttpPost("GetDichVuKhamBenhById")]
        public async Task<ActionResult> GetDichVuKhamBenhById([FromBody]DropDownListRequestModel queryInfo, long id)
        {
            var lookup = await _doiTuongUuDaiService.GetDichVuKhamBenhById(queryInfo, id);
            return Ok(lookup);
        }

        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DoiTuongUuDaiDichVuKhamBenh)]
        public async Task<ActionResult> Post([FromBody] DoiTuongUuDaiDichVuKhamBenhBenhVienViewModel doiTuongViewModel)
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
                if (count > 1)
                {
                    throw new ApiException(_localizationService.GetResource("DoiTuongUuDai.Denied"));
                }
                count = 0;
            }
            foreach (var dichVuKhamBenhBenhVienId in doiTuongViewModel.ListDichVuKhamBenhBenhVienId)
            {
                var check = await _doiTuongUuDaiService.CheckDichVuKhamBenhExist(Convert.ToInt64(dichVuKhamBenhBenhVienId));
                if (check == false)
                {
                    throw new ApiException(_localizationService.GetResource("DoiTuongUuDaiKhamBenh.DeleteDichVu"));
                }
                var checkActive = await _doiTuongUuDaiService.CheckDichVuKhamBenhActive(Convert.ToInt64(dichVuKhamBenhBenhVienId));
                if (checkActive == false)
                {
                    throw new ApiException(_localizationService.GetResource("DoiTuongUuDaiKhamBenh.NotAcitve"));
                }
            }
            foreach (var dichVuKhamBenhBenhVienId in doiTuongViewModel.ListDichVuKhamBenhBenhVienId)
            {
                for (int i = 0; i < doiTuongViewModel.DoiTuongUuDai.Count; i++)
                {
                    var result = new DoiTuongUuDaiDichVuKhamBenhBenhVien()
                    {
                        DoiTuongUuDaiId = Convert.ToInt64(doiTuongViewModel.DoiTuongUuDai[i].DoiTuongId),
                        DichVuKhamBenhBenhVienId = Convert.ToInt64(dichVuKhamBenhBenhVienId),
                        TiLeUuDai = Convert.ToInt32(doiTuongViewModel.DoiTuongUuDai[i].TiLeUuDai)
                    };
                    var CheckDaTao = await _doiTuongUuDaiService.CheckDichVuKhamBenhExit(result.DoiTuongUuDaiId, result.DichVuKhamBenhBenhVienId);
                    if (CheckDaTao == true)
                    {
                        throw new ApiException(_localizationService.GetResource("DoiTuongUuDai.Exit"));
                    }
                    await _doiTuongUuDaiService.AddDoiTuongUuDaiKhamBenhEntity(result);
                }
            }
            return Ok();
        }
        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DoiTuongUuDaiDichVuKhamBenh)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<DoiTuongUuDaiDichVuKhamBenhBenhVienViewModel>> Get(long id)
        {
            var result = new DoiTuongUuDaiDichVuKhamBenhBenhVienViewModel();
            var lstdoiTuongUudai = new List<DoiTuongUuDaiViewModel>();

            var entity = await _doiTuongUuDaiService.GetDataDoiTuongUuDaiKhamBenh(id);
            if (entity != null)
            {
                result.DichVuKhamBenhBenhVienId = entity.First().DichVuKhamBenhBenhVienId;
                result.DichVuKhamBenhModelText = await _doiTuongUuDaiService.GetNameDichVuKhamBenh(entity.First().DichVuKhamBenhBenhVienId);
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
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DoiTuongUuDaiDichVuKhamBenh)]
        public async Task<ActionResult> Put([FromBody] DoiTuongUuDaiDichVuKhamBenhBenhVienViewModel viewModel)
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
            foreach (var dichVuKhamBenhBenhVienId in viewModel.ListDichVuKhamBenhBenhVienId)
            {
                var check = await _doiTuongUuDaiService.CheckDichVuKhamBenhExist(Convert.ToInt64(viewModel.DichVuKhamBenhBenhVienId));
                if (check == false)
                {
                    throw new ApiException(_localizationService.GetResource("DoiTuongUuDaiKhamBenh.DeleteDichVu"));
                }
                var checkActive = await _doiTuongUuDaiService.CheckDichVuKhamBenhActive(Convert.ToInt64(viewModel.DichVuKhamBenhBenhVienId));
                if (checkActive == false)
                {
                    throw new ApiException(_localizationService.GetResource("DoiTuongUuDaiKhamBenh.NotAcitve"));
                }
                await _doiTuongUuDaiService.DeleteToAddDoiTuongUuDaiKhamBenh(Convert.ToInt64(viewModel.DichVuKhamBenhBenhVienOld));
            }
            foreach (var dichVuKhamBenhBenhVienId in viewModel.ListDichVuKhamBenhBenhVienId)
            {
                for (int i = 0; i < viewModel.DoiTuongUuDai.Count; i++)
                {
                    var result = new DoiTuongUuDaiDichVuKhamBenhBenhVien()
                    {
                        DoiTuongUuDaiId = Convert.ToInt64(viewModel.DoiTuongUuDai[i].DoiTuongId),
                        DichVuKhamBenhBenhVienId = Convert.ToInt64(dichVuKhamBenhBenhVienId),
                        TiLeUuDai = Convert.ToInt32(viewModel.DoiTuongUuDai[i].TiLeUuDai)
                    };
                    await _doiTuongUuDaiService.AddDoiTuongUuDaiKhamBenhEntity(result);
                }
            }
            return NoContent();
        }

        [HttpPost("ExportDoiTuongUuDaiDichVuKhamBenh")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DoiTuongUuDaiDichVuKhamBenh)]
        public async Task<ActionResult> ExportDoiTuongUuDaiDichVuKhamBenh(QueryInfo queryInfo)
        {
            // todo: hardcode max row export excel
            queryInfo.Skip = 0;
            queryInfo.Take = 20000;

            var gridData = await _doiTuongUuDaiService.GetDataForGridBenhVienAsync(queryInfo);
            var data = gridData.Data.Select(p => (DoiTuongUuDaiGridVo)p).ToList();
            var dataExcel = data.Map<List<DoiTuongUuDaiDichVuKhamBenhExportExcel>>();

            queryInfo.Sort = new List<Sort>();
            queryInfo.Sort.Add(new Sort()
            {
                Field = "DoiTuong",
                Dir = "asc"
            });
            foreach (var item in dataExcel)
            {
                var gridChildData = await _doiTuongUuDaiService.GetDataForGridBenhVienChildAsync(queryInfo, item.DichVuKyThuatId);
                var dataChild = gridChildData.Data.Select(p => (DoiTuongUuDaiChildGridVo)p).ToList();
                var dataChildExcel = dataChild.Map<List<DoiTuongUuDaiDichVuKhamBenhExportExcelChild>>();
                item.DoiTuongUuDaiDichVuKhamBenhExportExcelChild.AddRange(dataChildExcel);
            }

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(DoiTuongUuDaiDichVuKhamBenhExportExcel.Ma), "Mã"));
            lstValueObject.Add((nameof(DoiTuongUuDaiDichVuKhamBenhExportExcel.Ma4350), "Mã TT37"));
            lstValueObject.Add((nameof(DoiTuongUuDaiDichVuKhamBenhExportExcel.Ten), "Tên"));
            lstValueObject.Add((nameof(DoiTuongUuDaiDichVuKhamBenhExportExcel.DoiTuongUuDaiDichVuKhamBenhExportExcelChild), ""));


            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Đối tượng ưu đãi dịch vụ khám bệnh");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DoiTuongUuDaiDichVuKhamBenh" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";


            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}