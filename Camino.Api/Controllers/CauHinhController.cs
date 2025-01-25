using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.Cauhinh;
using Camino.Api.Models.Error;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Services.CauHinh;
using Camino.Services.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Camino.Api.Controllers
{
    public class CauHinhController : CaminoBaseController
    {
        private readonly ICauHinhService _cauHinhService;
        private readonly ICauHinhTheoThoiGianService _cauHinhTheoThoiGianService;
        private readonly ILocalizationService _localizationService;

        public CauHinhController(ICauHinhService cauHinhService, ICauHinhTheoThoiGianService cauHinhTheoThoiGianService, ILocalizationService localizationService)
        {
            _cauHinhService = cauHinhService;
            _localizationService = localizationService;
            _cauHinhTheoThoiGianService = cauHinhTheoThoiGianService;
        }

        [HttpGet("GetById")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuanLyCacCauHinh)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<CauhinhViewModel>> GetById(long id, int loaiCauHinh)
        {
            if (loaiCauHinh == 1)
            {
                var cauhinh = await _cauHinhService.GetByIdAsync(id);
                if (cauhinh == null)
                {
                    return NotFound();
                }
                return Ok(cauhinh.ToModel<CauhinhViewModel>());
            }
            else
            {
                var cauhinh = await _cauHinhTheoThoiGianService.GetByIdAsync(id,
                    s => s.Include(k => k.CauHinhTheoThoiGianChiTiets));

                if (cauhinh == null)
                {
                    return NotFound();
                }

                return Ok(cauhinh.ToModel<CauhinhViewModel>());
            }
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuanLyCacCauHinh)]
        public GridDataSource GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = _cauHinhService.GetTotalPageForGridAsync(queryInfo);
            return gridData;
        }

        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.QuanLyCacCauHinh)]
        public async Task<ActionResult> Put([FromBody] CauhinhViewModel cauHinhViewModel)
        {
            cauHinhViewModel.Name = cauHinhViewModel.Name.TrimEnd().TrimStart();
            cauHinhViewModel.Value = cauHinhViewModel.Value.TrimEnd().TrimStart();

            if (cauHinhViewModel.LoaiCauHinh == 1)
            {
                var cauHinh = await _cauHinhService.GetByIdAsync(cauHinhViewModel.Id);

                if (cauHinh == null)
                {
                    return NotFound();
                }

                if (cauHinhViewModel.DataType == (int) Enums.DataType.List)
                {
                    if (!cauHinhViewModel.CauHinhDanhSachChiTiets.Any())
                    {
                        throw new ApiException(_localizationService.GetResource("CauHinh.CauHinhDanhSachChiTiets.Required"));
                    }

                    if (cauHinhViewModel.DataTypeLoaiCauHinh == Enums.LoaiCauHinh.CauHinhGachNo)
                    {
                        var slTenHienThi = cauHinhViewModel.CauHinhDanhSachChiTiets.Select(x => x.DisplayName.Trim().ToLower()).Distinct().Count();
                        if (slTenHienThi < cauHinhViewModel.CauHinhDanhSachChiTiets.Count)
                        {
                            throw new ApiException(_localizationService.GetResource("CauHinh.CauHinhDanhSachChiTiets.DisplayNameIsExists"));
                        }
                    }
                }

                cauHinhViewModel.ToEntity(cauHinh);
                await _cauHinhService.UpdateAsync(cauHinh);
            }

            if (cauHinhViewModel.LoaiCauHinh == 2)
            {
                var cauHinhTheoThoiGian = await _cauHinhTheoThoiGianService.GetByIdAsync(cauHinhViewModel.Id,
                    s => s.Include(k => k.CauHinhTheoThoiGianChiTiets));

                if (cauHinhTheoThoiGian == null)
                {
                    return NotFound();
                }

                // update thời gian
                var index = 0;
                var cauHinhChiTietPassedBy = new CauHinhTheoThoiGianChiTietViewModel();
                foreach (var cauHinhChiTiet in cauHinhViewModel.CauHinhTheoThoiGianChiTiets)
                {
                    index++;
                    if (index >= 2)
                    {
                        cauHinhChiTietPassedBy.ToDate = cauHinhChiTiet.FromDate.GetValueOrDefault().AddDays(-1);
                    }
                    cauHinhChiTietPassedBy = cauHinhChiTiet;

                    if (index == cauHinhViewModel.CauHinhTheoThoiGianChiTiets.Count)
                    {
                        if (cauHinhChiTietPassedBy.ToDate != null)
                        {
                            cauHinhChiTietPassedBy.ToDate = null;
                        }
                    }
                }

                var indexValidate = 0;
                var cauHinhPassBy = new CauHinhTheoThoiGianChiTietViewModel();

                foreach (var cauHinhItem in cauHinhViewModel.CauHinhTheoThoiGianChiTiets)
                {
                    if (cauHinhItem.ToDate != null)
                    {
                        if (cauHinhItem.FromDate > cauHinhItem.ToDate)
                        {
                            throw new ArgumentException(_localizationService.GetResource("DichVuKyThuatBenhVien.TuNgay.Required"));
                        }
                    }

                    if (indexValidate == 1)
                    {
                        if (indexValidate >= 1)
                        {
                            if (cauHinhItem.FromDate < cauHinhPassBy.FromDate)
                            {
                                throw new ArgumentException(_localizationService.GetResource("DichVuKyThuatBenhVien.TuNgay.Required"));
                            }
                        }
                    }

                    indexValidate++;
                    cauHinhPassBy = cauHinhItem;
                }

                cauHinhViewModel.ToEntity(cauHinhTheoThoiGian);

                await _cauHinhTheoThoiGianService.UpdateAsync(cauHinhTheoThoiGian);
            }

            return NoContent();
        }

        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuanLyCacCauHinh)]
        public GridDataSource GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = _cauHinhService.GetDataForGridAsync(queryInfo);
            return gridData;
        }

        [HttpPost("GetLoaiCauHinh")]
        public ActionResult<ICollection<Enums.LoaiCauHinh>> GetListLoaiCauHinh()
        {
            var listEnum = _cauHinhService.getListLoaiCauHinh();
            return Ok(listEnum);
        }

        [HttpGet("GetByName")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<CauhinhViewModel> GetByName(string name)
        {
            var cauhinh = _cauHinhService.GetByName(name);
            if (cauhinh == null)
            {
                return NotFound();
            }
            return Ok(cauhinh.ToModel<CauhinhViewModel>());
        }
    }
}