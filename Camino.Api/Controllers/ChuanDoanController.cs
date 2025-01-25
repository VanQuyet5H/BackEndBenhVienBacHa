using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.Error;
using Camino.Api.Models.ICDs;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.ICDs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.ICDs;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.ICDs;
using Camino.Services.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.Controllers
{
    public class ChuanDoanController : CaminoBaseController
    {
        private readonly IChuanDoanService _chuanDoanService;
        private IChuanDoanLienKetICDService _chuanDoanLienKetICDService;
        private readonly IExcelService _excelService;
        private readonly ILocalizationService _localizationService;
        public ChuanDoanController(ILocalizationService localizationService, IChuanDoanService chuanDoanService, IChuanDoanLienKetICDService chuanDoanLienKetIcdService, IExcelService excelService)
        {
            _chuanDoanService = chuanDoanService;
            _excelService = excelService;
            _chuanDoanLienKetICDService = chuanDoanLienKetIcdService;
            _localizationService = localizationService;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucChuanDoan)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _chuanDoanService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucChuanDoan)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _chuanDoanService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucChuanDoan)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridChildAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _chuanDoanService.GetDataForGridChildAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucChuanDoan)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridChildAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _chuanDoanService.GetTotalPageForGridChildAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucChuanDoan)]
        public async Task<ActionResult> Delete(long id)
        {
            var chucdanh = await _chuanDoanService.GetByIdAsync(id,p=>p.Include(x=>x.ChuanDoanLienKetICDs));
            if (chucdanh == null)
            {
                return NotFound();
            }
            if (chucdanh.ChuanDoanLienKetICDs.Count > 0)
            {
                for (int i = 0; i < chucdanh.ChuanDoanLienKetICDs.Count; i++)
                {
                    await _chuanDoanLienKetICDService.DeleteByIdAsync(chucdanh.ChuanDoanLienKetICDs.ToList()[i].Id);
                    i = -1;
                }
                //foreach(var item in chucdanh.ChuanDoanLienKetICDs) {
                //    await _chuanDoanLienKetICDService.DeleteByIdAsync(item.Id);
                //}
            }
            await _chuanDoanService.DeleteByIdAsync(id);

            return NoContent();
        }
        [HttpPost("GetDanhMucChuanDoan")]
        public async Task<ActionResult> GetDanhMucChuanDoan([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _chuanDoanService.GetDichVuKhamBenh(queryInfo);
            return Ok(lookup);
        }
        [HttpPost("GetChuanDoanLienKetICD")]
        public async Task<ActionResult> GetChuanDoanLienKetIcd([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _chuanDoanService.GetListChuanDoanIcd(model);
            return Ok(lookup);
        }

        [HttpPost("GetListChuanDoanTheoMaBenh")]
        public async Task<ActionResult> GetListChuanDoanTheoMaBenh([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _chuanDoanService.GetListChuanDoanTheoMaBenh(model);
            return Ok(lookup);
        }


        [HttpPost("GetListChuanDoanTheoTenBenh")]
        public async Task<ActionResult> GetListChuanDoanTheoTenBenh([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _chuanDoanService.GetListChuanDoanTheoTenBenh(model);
            return Ok(lookup);
        }

        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucChuanDoan)]
        public async Task<ActionResult<ChuanDoanViewModel>> Post
           ([FromBody]ChuanDoanViewModel chuanDoanViewModel)
        {
            var danhMucChuanDoan =  _chuanDoanService.GetDanhMucChuanDoan(Convert.ToInt64(chuanDoanViewModel.DanhMucChuanDoanId));
            if (danhMucChuanDoan.Count == 0) {
                throw new ApiException(_localizationService.GetResource("danhMucChuanDoan.CannotAccess"));
            }
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = chuanDoanViewModel.TenTiengAnh.Normalize(NormalizationForm.FormD);
            chuanDoanViewModel.TenTiengAnh = regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
            var chuanDoan = chuanDoanViewModel.ToEntity<ChuanDoan>();
            await _chuanDoanService.AddAsync(chuanDoan);
            var last = _chuanDoanService.GetChuanDoanLast();
            foreach(var item in chuanDoanViewModel.ChuanDoanLienKetICDIds){
                await _chuanDoanLienKetICDService.AddAsync(new ChuanDoanLienKetICD {ChuanDoanId=last.Id,ICDId=item });
            }
           
            return NoContent();
        }
        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucChuanDoan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<ChuanDoanViewModel>> Get(long id)
        {
            var chuanDoan = await _chuanDoanService.GetByIdAsync(id, s => s.Include(r => r.ChuanDoanLienKetICDs).ThenInclude(p => p.ICD).Include(y => y.DanhMucChuanDoan));

            if (chuanDoan == null)
            {
                return NotFound();
            }
            var result = chuanDoan.ToModel<ChuanDoanViewModel>();
            //result.DanhMucChuanDoanName = _chuanDoanService.GetTenDanhMuc(chuanDoan.DanhMucChuanDoanId);
            return Ok(result);
        }
        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucChuanDoan)]
        public async Task<ActionResult> Put([FromBody] ChuanDoanViewModel viewModel)
        {
            int count = 0;
            var entity = await _chuanDoanService.GetByIdAsync(viewModel.Id, s => s.Include(r => r.ChuanDoanLienKetICDs));
            if (entity == null)
                return NotFound();

            viewModel.ToEntity(entity);
            await _chuanDoanService.UpdateAsync(entity);

            if (viewModel.ChuanDoanLienKetICDIds.Count == 0) {
                if (entity.ChuanDoanLienKetICDs.Count > 0) {
                    for (int i = 0; i < entity.ChuanDoanLienKetICDs.Count; i++)
                    {
                        await _chuanDoanLienKetICDService.DeleteByIdAsync(entity.ChuanDoanLienKetICDs.ToList()[i].Id);
                        i = -1;
                    }
                }
                
            }
            else
            {
                foreach(var item in viewModel.ChuanDoanLienKetICDIds) {

                    if (count == 0)
                    {
                        if (entity.ChuanDoanLienKetICDs.Count > 0)
                        {
                            for (int i = 0; i < entity.ChuanDoanLienKetICDs.Count; i++)
                            {
                                await _chuanDoanLienKetICDService.DeleteByIdAsync(entity.ChuanDoanLienKetICDs.ToList()[i].Id);
                                i = -1;
                            }

                            await _chuanDoanLienKetICDService.AddAsync(new ChuanDoanLienKetICD { ChuanDoanId = entity.Id, ICDId = item });

                        }
                        else
                        {
                            await _chuanDoanLienKetICDService.AddAsync(new ChuanDoanLienKetICD { ChuanDoanId = entity.Id, ICDId = item });
                        }
                    }
                    else
                    {
                        await _chuanDoanLienKetICDService.AddAsync(new ChuanDoanLienKetICD { ChuanDoanId = entity.Id, ICDId = item });
                    }
                   
                    count++;
                }
            }
            

            return NoContent();
        }

        [HttpPost("ExportChanDoan")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucChuanDoan)]
        public async Task<ActionResult> ExportChanDoan(QueryInfo queryInfo)
        {
            var gridData = await _chuanDoanService.GetDataForGridAsync(queryInfo, true);
            var loaiBenhVienData = gridData.Data.Select(p => (ChuanDoanGridVo)p).ToList();
            var dataExcel = loaiBenhVienData.Map<List<ChuanDoanExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(ChuanDoanExportExcel.Ma), "Mã"));
            lstValueObject.Add((nameof(ChuanDoanExportExcel.TenTiengViet), "Tên Tiếng Việt"));
            lstValueObject.Add((nameof(ChuanDoanExportExcel.TenTiengAnh), "Tên Tiếng Anh"));

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Chẩn Đoán");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=ChanDoan" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}