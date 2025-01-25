using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.DichVuGiuong;
using Camino.Api.Models.DichVuGiuongBenhVien;
using Camino.Api.Models.Error;
using Camino.Api.Models.General;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DichVuGiuongBenhViens;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DichVuGiuong;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Camino.Services.DichVuGiuong;
using Camino.Services.DichVuGiuongBenhVien;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.KhamBenhs;
using Camino.Services.Localization;
using Camino.Services.TaiLieuDinhKem;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.Controllers
{
    public class DichVuGiuongBenhVienController : CaminoBaseController
    {
        private readonly IDichVuGiuongBenhVienService _dichVuGiuongBenhVienService;
        private readonly IDichVuGiuongService _dichVuGiuongService;
        private readonly IExcelService _excelService;
        private readonly ILocalizationService _localizationService;
        private readonly IYeuCauDichVuGiuongBenhService _yeuCauDichVuGiuongBenhService;
        private readonly ITaiLieuDinhKemService _taiLieuDinhKemService;
        public DichVuGiuongBenhVienController(
            IExcelService excelService,
            IDichVuGiuongService dichVuGiuongService,
            IDichVuGiuongBenhVienService dichVuGiuongBenhVienService,
            ILocalizationService localizationService,
            IYeuCauDichVuGiuongBenhService yeuCauDichVuGiuongBenhService,
            ITaiLieuDinhKemService taiLieuDinhKemService
        )
        {
            _dichVuGiuongService = dichVuGiuongService;
            _dichVuGiuongBenhVienService = dichVuGiuongBenhVienService;
            _excelService = excelService;
            _localizationService = localizationService;
            _yeuCauDichVuGiuongBenhService = yeuCauDichVuGiuongBenhService;
            _taiLieuDinhKemService = taiLieuDinhKemService;
        }

        #region GetDataForGrid
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDichVuGiuongTaiBenhVien)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _dichVuGiuongBenhVienService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDichVuGiuongTaiBenhVien)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _dichVuGiuongBenhVienService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDichVuGiuongTaiBenhVien)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridChildAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _dichVuGiuongBenhVienService.GetDataForGridChildAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDichVuGiuongTaiBenhVien)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridChildAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _dichVuGiuongBenhVienService.GetTotalPageForGridChildAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridChildBenhVienAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDichVuGiuongTaiBenhVien)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridChildBenhVienAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _dichVuGiuongBenhVienService.GetDataForGridChildBenhVienAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridChildBenhVienAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDichVuGiuongTaiBenhVien)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridChildBenhVienAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _dichVuGiuongBenhVienService.GetTotalPageForGridChildBenhVienAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion
        [HttpPost("GetDichVuGiuongById")]
        public async Task<ActionResult> GetDichVuKhamBenhById([FromBody]DropDownListRequestModel queryInfo, long id)
        {
            var lookup = await _dichVuGiuongBenhVienService.GetDichVuGiuongById(queryInfo, id);
            return Ok(lookup);
        }
        #region GetListVo

        [HttpPost("GetListDichVuGiuong")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListDichVuGiuong([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _dichVuGiuongBenhVienService.GetDichVuGiuong(model);
            return Ok(lookup);
        }

        [HttpPost("GetKhoaKhamTheoDichVuGiuongBenhVien")]
        public async Task<ActionResult> GetKhoaKhamTheoDichVuGiuongBenhVien([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _dichVuGiuongBenhVienService.GetKhoaKhamTheoDichVuGiuongBenhVien(queryInfo);
            return Ok(lookup);
        }
        [HttpPost("GetPhongKhamTheoDichVuGiuongBenhVien")]
        public async Task<ActionResult> GetPhongKhamTheoDichVuGiuongBenhVien([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _dichVuGiuongBenhVienService.GetPhongKhamTheoDichVuGiuongBenhVien(queryInfo);
            return Ok(lookup);
        }

        [HttpPost("GetListLoaiGiuong")]
        public List<LookupItemVo> GetListLoaiGiuongAsync([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookups = _dichVuGiuongBenhVienService.GetListLoaiGiuongAsync(queryInfo);
            return lookups;
        }
        #endregion

        #region CRUD

        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucDichVuGiuongTaiBenhVien)]
        public async Task<ActionResult<DichVuGiuongBenhVienViewModel>> Post([FromBody]DichVuGiuongBenhVienViewModel dichVuGiuongBenhVienViewModel)
        {

            if (dichVuGiuongBenhVienViewModel.DichVuGiuongBenhVienGiaBaoHiems.Count > 0)
            {
                for (int i = 1; i < dichVuGiuongBenhVienViewModel.DichVuGiuongBenhVienGiaBaoHiems.Count; i++)
                {

                    dichVuGiuongBenhVienViewModel.DichVuGiuongBenhVienGiaBaoHiems[i - 1].DenNgay = Convert.ToDateTime(dichVuGiuongBenhVienViewModel.DichVuGiuongBenhVienGiaBaoHiems[i].TuNgay).Date.AddDays(-1);

                    //if (dichVuGiuongBenhVienViewModel.DichVuGiuongBenhVienGiaBaoHiems[i - 1].DenNgay != null)
                    //{
                    //    if (dichVuGiuongBenhVienViewModel.DichVuGiuongBenhVienGiaBaoHiems[i - 1].TuNgay > dichVuGiuongBenhVienViewModel.DichVuGiuongBenhVienGiaBaoHiems[i - 1].DenNgay)
                    //    {
                    //        throw new ApiException(
                    //                        _localizationService.GetResource("DichVuGiuongBenhVien.DichVuGiuongBenhVienGiaBaoHiems.TuNgay"));
                    //    }
                    //}
                    if //(dichVuGiuongBenhVienViewModel.DichVuGiuongBenhVienGiaBaoHiems[i].TuNgay <= dichVuGiuongBenhVienViewModel.DichVuGiuongBenhVienGiaBaoHiems[i - 1].TuNgay ||
                         (dichVuGiuongBenhVienViewModel.DichVuGiuongBenhVienGiaBaoHiems[i - 1].TuNgay > dichVuGiuongBenhVienViewModel.DichVuGiuongBenhVienGiaBaoHiems[i - 1].DenNgay ||
                         dichVuGiuongBenhVienViewModel.DichVuGiuongBenhVienGiaBaoHiems[i].TuNgay > dichVuGiuongBenhVienViewModel.DichVuGiuongBenhVienGiaBaoHiems[i].DenNgay ||
                         dichVuGiuongBenhVienViewModel.DichVuGiuongBenhVienGiaBaoHiems[i].TuNgay <= Convert.ToDateTime(dichVuGiuongBenhVienViewModel.DichVuGiuongBenhVienGiaBaoHiems[i - 1].TuNgay).AddDays(1))
                    {
                        throw new ApiException(
                                            _localizationService.GetResource("DichVuGiuongBenhVien.DichVuGiuongBenhVienGiaBaoHiems.TuNgay"));
                    }

                }
            }
            if (dichVuGiuongBenhVienViewModel.DichVuGiuongBenhVienGiaBenhViens.Count > 0)
            {
                var nhomdichVu = await _dichVuGiuongBenhVienService.GetNhomGiaDichVuKyThuatBenhVien();
                for (int j = 0; j < nhomdichVu.Count; j++)
                {
                    var benhvien = dichVuGiuongBenhVienViewModel.DichVuGiuongBenhVienGiaBenhViens.Where(x => x.NhomGiaDichVuGiuongBenhVienId == nhomdichVu[j].Id).ToList();
                    for (int i = 1; i < benhvien.Count; i++)
                    {
                        benhvien[i - 1].DenNgay = Convert.ToDateTime(benhvien[i].TuNgay).Date.AddDays(-1);
                        //if (benhvien[i - 1].DenNgay != null)
                        //{
                        //    if (benhvien[i - 1].TuNgay > benhvien[i - 1].DenNgay)
                        //    {
                        //        throw new ApiException(
                        //                        _localizationService.GetResource("DichVuGiuongBenhVien.DichVuGiuongBenhVienGiaBenhViens.TuNgay"));
                        //    }
                        //}
                        if //(benhvien[i].TuNgay <= benhvien[i - 1].TuNgay ||
                             (benhvien[i - 1].TuNgay > benhvien[i - 1].DenNgay ||
                             benhvien[i].TuNgay > benhvien[i].DenNgay ||
                             benhvien[i].TuNgay <= Convert.ToDateTime(benhvien[i - 1].TuNgay).AddDays(1))// || benhvien[i - 1].TuNgay >= Convert.ToDateTime(benhvien[i].TuNgay).AddDays(-1))
                        {
                            throw new ApiException(
                                                _localizationService.GetResource("DichVuGiuongBenhVien.DichVuGiuongBenhVienGiaBenhViens.TuNgay"));
                        }
                    }
                }
            }
            //Hiện tại cho giá bệnh viện nhâp 0 nên không cho nhập giá bhyt
            var kiemTraGiaBenhVien = dichVuGiuongBenhVienViewModel.DichVuGiuongBenhVienGiaBenhViens.Any(c => c.TuNgay.Value.Date >= DateTime.Now.Date && c.Gia != 0);
            var kiemTraGiaBHYT = dichVuGiuongBenhVienViewModel.DichVuGiuongBenhVienGiaBaoHiems.Any();
            if (kiemTraGiaBenhVien && kiemTraGiaBHYT)
            {
                throw new ApiException(_localizationService.GetResource("Common.KhongNhapBHYT"));
            }

            //Hiện tại cho giá bệnh viện không được nhỏ hơn giá BHYT
            var giabenhViens = dichVuGiuongBenhVienViewModel.DichVuGiuongBenhVienGiaBenhViens.Where(c => c.TuNgay.Value.Date >= DateTime.Now.Date && c.Gia != 0);
            var giaBHYTs = dichVuGiuongBenhVienViewModel.DichVuGiuongBenhVienGiaBaoHiems.Where(c => c.TuNgay.Value.Date >= DateTime.Now.Date && c.Gia != 0);

            if (giabenhViens.Any() && giaBHYTs.Any())
            {
                foreach (var giabenhVien in giabenhViens)
                {
                    foreach (var giaBHYT in giaBHYTs)
                    {
                        if (giabenhVien.Gia < giaBHYT.Gia)
                        {
                            throw new ApiException(_localizationService.GetResource("Common.GiaBVLonGiaBHYT"));
                        }
                    }
                }
            }

            var dichVuGiuongBenhVien = dichVuGiuongBenhVienViewModel.ToEntity<DichVuGiuongBenhVien>();

            //for (int i = 1; i < dichVuGiuongBenhVien.DichVuGiuongBenhVienGiaBaoHiems.Count; i++)
            //{
            //    dichVuGiuongBenhVien.DichVuGiuongBenhVienGiaBaoHiems.ToList()[i - 1].DenNgay = dichVuGiuongBenhVien.DichVuGiuongBenhVienGiaBaoHiems.ToList()[i].TuNgay.AddDays(-1);
            //}
            //_dichVuGiuongBenhVienService.UpdateDayGiaBenhVienEntity(dichVuGiuongBenhVien.DichVuGiuongBenhVienGiaBenhViens);

            // xử lý kiểm tra giá BH so với giá BV
            var lstGiaBaoHiem = dichVuGiuongBenhVien.DichVuGiuongBenhVienGiaBaoHiems.Where(x => x.WillDelete != true).ToList();
            var lstGiaBenhVien = dichVuGiuongBenhVien.DichVuGiuongBenhVienGiaBenhViens.Where(x => x.WillDelete != true).ToList();
            CheckValidGiaBaoHiem(lstGiaBaoHiem, lstGiaBenhVien);
            await _dichVuGiuongBenhVienService.AddAsync(dichVuGiuongBenhVien);
            //var result = dichVuGiuongBenhVien.ToModel<DichVuGiuongBenhVienViewModel>();
            //return CreatedAtAction(nameof(Get), new { id = dichVuGiuongBenhVienViewModel.Id }, result);
            return Ok();
        }
        [HttpPost("GetNhomDichVu")]
        public async Task<ActionResult> GetNhomDichVu()
        {
            var lookup = await _dichVuGiuongBenhVienService.GetNhomDichVu();
            return Ok(lookup);
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDichVuGiuongTaiBenhVien)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<DichVuGiuongBenhVienViewModel>> Get(long id)
        {
            var dichVuGiuongBenhVien = await _dichVuGiuongBenhVienService.GetByIdAsync(id, s => s.Include(k => k.DichVuGiuong)
                .Include(x => x.DichVuGiuongBenhVienGiaBaoHiems)
                .Include(x => x.DichVuGiuongBenhVienGiaBenhViens).ThenInclude(x => x.NhomGiaDichVuGiuongBenhVien)
                .Include(x => x.DichVuGiuongBenhVienNoiThucHiens).ThenInclude(y => y.PhongBenhVien));

            if (dichVuGiuongBenhVien == null)
            {
                return NotFound();
            }
            var result = dichVuGiuongBenhVien.ToModel<DichVuGiuongBenhVienViewModel>();
            foreach (var item in result.DichVuGiuongBenhVienGiaBenhViens)
            {
                item.NhomGiaDichVuGiuongBenhVienText = item.NhomGiaDichVuGiuongBenhVien.Ten;
            }

            var listEnum = EnumHelper.GetListEnum<Enums.EnumLoaiGiuong>();
            var lstEnum = listEnum.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            });
            result.LoaiGiuongText = lstEnum.FirstOrDefault(x => x.DisplayName == result.LoaiGiuong.GetDescription())?.DisplayName;
            return Ok(result);
        }

        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucDichVuGiuongTaiBenhVien)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> CapNhatDichVuGiuongBenhVien([FromBody]DichVuGiuongBenhVienViewModel dichVuGiuongBenhVienViewModel)
        {


            if (dichVuGiuongBenhVienViewModel.DichVuGiuongBenhVienGiaBaoHiems.Count > 0)
            {
                for (int i = 1; i < dichVuGiuongBenhVienViewModel.DichVuGiuongBenhVienGiaBaoHiems.Count; i++)
                {
                    //update 20/06/2022: set về cuối ngày của đến ngày
                    var denNgay = Convert.ToDateTime(dichVuGiuongBenhVienViewModel.DichVuGiuongBenhVienGiaBaoHiems.ToList()[i].TuNgay).AddDays(-1);
                    dichVuGiuongBenhVienViewModel.DichVuGiuongBenhVienGiaBaoHiems.ToList()[i - 1].DenNgay = denNgay.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                    
                }
                for (int i = 1; i < dichVuGiuongBenhVienViewModel.DichVuGiuongBenhVienGiaBaoHiems.Count; i++)
                {
                    //update 20/06/2022: set về cuối ngày của đến ngày
                    var denNgay = Convert.ToDateTime(dichVuGiuongBenhVienViewModel.DichVuGiuongBenhVienGiaBaoHiems[i].TuNgay).Date.AddDays(-1);
                    dichVuGiuongBenhVienViewModel.DichVuGiuongBenhVienGiaBaoHiems[i - 1].DenNgay = denNgay.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

                    //if (dichVuGiuongBenhVienViewModel.DichVuGiuongBenhVienGiaBaoHiems[i - 1].DenNgay != null)
                    //{
                    //    if (dichVuGiuongBenhVienViewModel.DichVuGiuongBenhVienGiaBaoHiems[i - 1].TuNgay > dichVuGiuongBenhVienViewModel.DichVuGiuongBenhVienGiaBaoHiems[i - 1].DenNgay)
                    //    {
                    //        throw new ApiException(
                    //                        _localizationService.GetResource("DichVuGiuongBenhVien.DichVuGiuongBenhVienGiaBaoHiems.TuNgay"));
                    //    }
                    //}
                    if //(dichVuGiuongBenhVienViewModel.DichVuGiuongBenhVienGiaBaoHiems[i].TuNgay <= dichVuGiuongBenhVienViewModel.DichVuGiuongBenhVienGiaBaoHiems[i - 1].TuNgay ||
                         (dichVuGiuongBenhVienViewModel.DichVuGiuongBenhVienGiaBaoHiems[i - 1].TuNgay > dichVuGiuongBenhVienViewModel.DichVuGiuongBenhVienGiaBaoHiems[i - 1].DenNgay ||
                         dichVuGiuongBenhVienViewModel.DichVuGiuongBenhVienGiaBaoHiems[i].TuNgay > dichVuGiuongBenhVienViewModel.DichVuGiuongBenhVienGiaBaoHiems[i].DenNgay ||
                         dichVuGiuongBenhVienViewModel.DichVuGiuongBenhVienGiaBaoHiems[i].TuNgay <= Convert.ToDateTime(dichVuGiuongBenhVienViewModel.DichVuGiuongBenhVienGiaBaoHiems[i - 1].TuNgay).AddDays(1))
                    {
                        throw new ApiException(
                                            _localizationService.GetResource("DichVuGiuongBenhVien.DichVuGiuongBenhVienGiaBaoHiems.TuNgay"));
                    }

                }
            }

            if (dichVuGiuongBenhVienViewModel.DichVuGiuongBenhVienGiaBenhViens.Count > 0)
            {
                var nhomdichVu = await _dichVuGiuongBenhVienService.NhomGiaDichVuGiuongBenhVien();
                for (int j = 0; j < nhomdichVu.Count; j++)
                {
                    var benhvien = dichVuGiuongBenhVienViewModel.DichVuGiuongBenhVienGiaBenhViens.Where(x => x.NhomGiaDichVuGiuongBenhVienId == nhomdichVu[j].Id).ToList();
                    for (int i = 1; i < benhvien.Count; i++)
                    {
                        //update 20/06/2022: set về cuối ngày của đến ngày
                        var denNgay = Convert.ToDateTime(benhvien[i].TuNgay).Date.AddDays(-1);
                        benhvien[i - 1].DenNgay = denNgay.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                        //if (benhvien[i - 1].DenNgay != null)
                        //{
                        //    if (benhvien[i - 1].TuNgay > benhvien[i - 1].DenNgay)
                        //    {
                        //        throw new ApiException(
                        //                        _localizationService.GetResource("DichVuGiuongBenhVien.DichVuGiuongBenhVienGiaBenhViens.TuNgay"));
                        //    }
                        //}
                        if //(benhvien[i].TuNgay <= benhvien[i - 1].TuNgay ||
                             (benhvien[i - 1].TuNgay > benhvien[i - 1].DenNgay ||
                             benhvien[i].TuNgay > benhvien[i].DenNgay ||
                             benhvien[i].TuNgay <= Convert.ToDateTime(benhvien[i - 1].TuNgay).AddDays(1))
                        {
                            throw new ApiException(
                                                _localizationService.GetResource("DichVuGiuongBenhVien.DichVuGiuongBenhVienGiaBenhViens.TuNgay"));
                        }

                    }
                }

            }
            //Hiện tại cho giá bệnh viện nhâp 0 nên không cho nhập giá bhyt
            var kiemTraGiaBenhVien = dichVuGiuongBenhVienViewModel.DichVuGiuongBenhVienGiaBenhViens.Any(c => c.DenNgay >= DateTime.Now && c.Gia == 0);
            var kiemTraGiaBHYT = dichVuGiuongBenhVienViewModel.DichVuGiuongBenhVienGiaBaoHiems.Any();
            if (kiemTraGiaBenhVien && kiemTraGiaBHYT)
            {
                throw new ApiException(_localizationService.GetResource("Common.KhongNhapBHYT"));
            }

            //Hiện tại cho giá bệnh viện không được nhỏ hơn giá BHYT
            var giabenhViens = dichVuGiuongBenhVienViewModel.DichVuGiuongBenhVienGiaBenhViens.Where(c => c.TuNgay.Value.Date >= DateTime.Now.Date && c.Gia != 0);
            var giaBHYTs = dichVuGiuongBenhVienViewModel.DichVuGiuongBenhVienGiaBaoHiems.Where(c => c.TuNgay.Value.Date >= DateTime.Now.Date && c.Gia != 0);

            if (giabenhViens.Any() && giaBHYTs.Any())
            {
                foreach (var giabenhVien in giabenhViens)
                {
                    foreach (var giaBHYT in giaBHYTs)
                    {
                        if (giabenhVien.Gia < giaBHYT.Gia)
                        {
                            throw new ApiException(_localizationService.GetResource("Common.GiaBVLonGiaBHYT"));
                        }
                    }
                }
            }

            var dichVuGiuongBenhVien = await _dichVuGiuongBenhVienService.GetByIdAsync(dichVuGiuongBenhVienViewModel.Id, p => p.Include(s => s.DichVuGiuong)
                .Include(x => x.DichVuGiuongBenhVienGiaBaoHiems)
                .Include(x => x.DichVuGiuongBenhVienGiaBenhViens)
                .Include(x => x.DichVuGiuongBenhVienNoiThucHiens).ThenInclude(w => w.PhongBenhVien)
                .Include(k => k.DichVuGiuongBenhVienNoiThucHiens).ThenInclude(w => w.KhoaPhong));

            if (dichVuGiuongBenhVien == null)
                return NotFound();

            dichVuGiuongBenhVienViewModel.ToEntity(dichVuGiuongBenhVien);
            //Kiểm tra những Giá bị xóa có đang dùng hay không
            foreach (var dichVuGiuongBenhVienGiaBenhVienDelete in dichVuGiuongBenhVien.DichVuGiuongBenhVienGiaBenhViens.Where(o => o.WillDelete == true))
            {
                var checkGiaDangSuDung = _yeuCauDichVuGiuongBenhService.KiemTraLoaiGiaDangSuDung(
                    dichVuGiuongBenhVienGiaBenhVienDelete.DichVuGiuongBenhVienId,
                    dichVuGiuongBenhVienGiaBenhVienDelete.NhomGiaDichVuGiuongBenhVienId);
                if (checkGiaDangSuDung)
                {
                    throw new ApiException(
                        _localizationService.GetResource("DichVuGiuongBenhVien.DichVuGiuongBenhVienGiaBenhViens.GiaDangSuDung"));
                }
            }
            //for (int i = 1; i < dichVuGiuongBenhVien.DichVuGiuongBenhVienGiaBaoHiems.Count; i++)
            //{
            //    dichVuGiuongBenhVien.DichVuGiuongBenhVienGiaBaoHiems.ToList()[i - 1].DenNgay = dichVuGiuongBenhVien.DichVuGiuongBenhVienGiaBaoHiems.ToList()[i].TuNgay.AddDays(-1);
            //}
            //_dichVuGiuongBenhVienService.UpdateDayGiaBenhVienEntity(dichVuGiuongBenhVien.DichVuGiuongBenhVienGiaBenhViens);

            // xử lý kiểm tra giá BH so với giá BV
            var lstGiaBaoHiem = dichVuGiuongBenhVien.DichVuGiuongBenhVienGiaBaoHiems.Where(x => x.WillDelete != true).ToList();
            var lstGiaBenhVien = dichVuGiuongBenhVien.DichVuGiuongBenhVienGiaBenhViens.Where(x => x.WillDelete != true).ToList();
            CheckValidGiaBaoHiem(lstGiaBaoHiem, lstGiaBenhVien);
            await _dichVuGiuongBenhVienService.UpdateAsync(dichVuGiuongBenhVien);
            return Ok();
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucDichVuGiuongTaiBenhVien)]
        public async Task<ActionResult> Delete(long id)
        {
            var dichVuGiuongBenhVien = await _dichVuGiuongBenhVienService.GetByIdAsync(id, p => p.Include(x => x.DichVuGiuongBenhVienGiaBaoHiems)
                 .Include(x => x.DichVuGiuongBenhVienGiaBenhViens)
                 .Include(x => x.DichVuGiuongBenhVienNoiThucHiens));
            if (dichVuGiuongBenhVien == null)
            {
                return NotFound();
            }

            await _dichVuGiuongBenhVienService.DeleteByIdAsync(id);
            return NoContent();
        }
        [HttpPost("GetLoaiGiuong")]
        public Task<List<LookupItemVo>> GetEnumMatainan()
        {
            var listEnum = EnumHelper.GetListEnum<Enums.EnumLoaiGiuong>();

            var result = listEnum.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            })
                .ToList();
            return Task.FromResult(result);
        }
        [HttpPost("Deletes")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucDichVuGiuongTaiBenhVien)]
        public async Task<ActionResult> Deletes([FromBody]DeletesViewModel model)
        {
            var dichVuGiuongBenhViens = await _dichVuGiuongBenhVienService.GetByIdsAsync(model.Ids);

            if (dichVuGiuongBenhViens == null)
            {
                return NotFound();
            }

            var dichVuGiuongBenhVienList = dichVuGiuongBenhViens.ToList();
            if (dichVuGiuongBenhVienList.Count != model.Ids.Length)
            {
                throw new ArgumentException(_localizationService
                    .GetResource("Common.WrongLengthMultiDelete"));
            }
            await _dichVuGiuongBenhVienService.DeleteAsync(dichVuGiuongBenhVienList);
            return NoContent();
        }
        #endregion

        [HttpGet("GetThongTinDichVuGiuong")]
        public async Task<ActionResult<DichVuGiuongViewModel>> GetThongTinDichVuGiuongAsync(long dichVuGiuongId)
        {
            var result = await _dichVuGiuongService.GetByIdAsync(dichVuGiuongId);
            if (result == null)
            {
                return NotFound();
            }

            var viewModel = result.ToModel<DichVuGiuongViewModel>();
            return Ok(viewModel);
        }
        
        private void CheckValidGiaBaoHiem(ICollection<DichVuGiuongBenhVienGiaBaoHiem> lstGiaBaoHiem, ICollection<DichVuGiuongBenhVienGiaBenhVien> lstGiaBenhVien)
        {
            if (lstGiaBaoHiem.Any() && lstGiaBenhVien.Any())
            {
                foreach (var giaBaoHiem in lstGiaBaoHiem)
                {
                    var lstGiaBenhVienTheoKhoangThoiGian = lstGiaBenhVien.Where(x =>
                            // trường hợp từ ngày nằm trong khoảng thời gian
                            (giaBaoHiem.TuNgay.Date >= x.TuNgay.Date && (x.DenNgay == null || giaBaoHiem.TuNgay.Date <= x.DenNgay))

                            // trường hợp đến ngày nằm trong khoảng thời gian
                            || (giaBaoHiem.DenNgay != null && giaBaoHiem.DenNgay.Value.Date >= x.TuNgay.Date && (x.DenNgay == null || giaBaoHiem.DenNgay.Value.Date <= x.DenNgay.Value.Date))

                            // trường hợp ko có đến ngày
                            || (giaBaoHiem.DenNgay == null && (giaBaoHiem.TuNgay.Date <= x.TuNgay.Date || (x.DenNgay != null && giaBaoHiem.TuNgay.Date <= x.DenNgay.Value.Date))))
                        .ToList();
                    //                    foreach (var giaBenhVienTheoThoiGian in lstGiaBenhVienTheoKhoangThoiGian)
                    //                    {
                    //                        if (giaBenhVienTheoThoiGian.Gia < giaBaoHiem.Gia)
                    //                        {
                    //                            throw new ApiException(_localizationService.GetResource("DichVuGiuongBenhVien.GiaBaoHiem.RangeValueByTime"));
                    //                        }
                    //                    }
                }
            }
        }

        [HttpPost("ExportDichVuGiuongBenhVien")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucDichVuGiuongTaiBenhVien)]
        public async Task<ActionResult> ExportDichVuGiuongBenhVien(QueryInfo queryInfo)
        {
            var gridData = await _dichVuGiuongBenhVienService.GetDataForGridAsync(queryInfo, true);
            var loaiBenhVienData = gridData.Data.Select(p => (DichVuGiuongGridVo)p).ToList();
            var dataExcel = loaiBenhVienData.Map<List<DichVuGiuongBenhVienExportExcel>>();

            foreach (var item in dataExcel)
            {
                var gridChildData = await _dichVuGiuongBenhVienService.GetDataForGridChildAsync(queryInfo, item.Id, true);
                var gridChildDataGiaBenhVien = await _dichVuGiuongBenhVienService.GetDataForGridChildBenhVienAsync(queryInfo, item.Id, true);
                var dataChild = gridChildData.Data.Select(p => (DichVuKhamBenhBenhVienGiaBaoHiemVO)p).ToList();
                var dataChildExcel = dataChild.Map<List<DichVuGiuongBenhVienExportExcelChild>>();
                var dataChildGiaBenhVien = gridChildDataGiaBenhVien.Data.Select(p => (DichVuKhamBenhBenhVienGiaBenhVienVO)p).ToList();
                var dataChildGiaBenhVienExcel = dataChildGiaBenhVien.Map<List<DichVuGiuongBenhVienExportExcelChild>>();
                item.DichVuGiuongBenhVienExportExcelChild.AddRange(dataChildExcel);
                item.DichVuGiuongBenhVienExportExcelChild.AddRange(dataChildGiaBenhVienExcel);
            }

            var lstValueObject = new List<(string, string)>
            {
                (nameof(DichVuGiuongBenhVienExportExcel.Ma), "Mã"),
                (nameof(DichVuGiuongBenhVienExportExcel.MaTT37), "Mã TT37"),
                (nameof(DichVuGiuongBenhVienExportExcel.Ten), "Tên"),
                (nameof(DichVuGiuongBenhVienExportExcel.TenNoiThucHien), "Nơi Thực Hiện"),
                (nameof(DichVuGiuongBenhVienExportExcel.HangBenhVienDisplay), "Hạng Bệnh Viện"),
                (nameof(DichVuGiuongBenhVienExportExcel.LoaiGiuongDisplay), "Loại giường"),
                (nameof(DichVuGiuongBenhVienExportExcel.MoTa), "Mô Tả"),
                (nameof(DichVuGiuongBenhVienExportExcel.HieuLucHienThi), "Hiệu lực"),
                (nameof(DichVuGiuongBenhVienExportExcel.DichVuGiuongBenhVienExportExcelChild), ""),
            };

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Dịch Vụ Giường Bệnh Viện");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DichVuGiuongBenhVien" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        
        [HttpPost("ExportGiuongBV")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucDichVuGiuongTaiBenhVien)]
        public async Task<ActionResult> ExportGiuongBenhViens(QueryInfo queryInfo)
        {
            var gridData = await _dichVuGiuongBenhVienService.GetDataForGridAsync(queryInfo, true);
            var bytes = _dichVuGiuongBenhVienService.ExportDichVuGiuongBenhVien(gridData);

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=ExportGiuongBenhVien" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        #region BVHD-3937
        [HttpPost("DownloadFileExcelTemplateGiaDichVuBenhVien")]
        public ActionResult DownloadFileExcelTemplateGiaDichVuBenhVien()
        {
            var path = @"Resource\\GiaDichVuBenhVien.xlsx";
            System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(fs);

            long byteLength = new System.IO.FileInfo(path).Length;
            var fileContent = binaryReader.ReadBytes((Int32)byteLength);

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=GiaDichVuBenhVien.xlsx");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(fileContent, "application/vnd.ms-excel");
        }

        [HttpPost("ImportGiaDichVuBenhVien")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucDichVuGiuongTaiBenhVien)]
        public async Task<ActionResult> ImportGiaDichVuBenhVien(GiaDichVuBenhVienFileImportVo model)
        {
            var path = _taiLieuDinhKemService.GetObjectStream(model.DuongDan, model.TenGuid);
            model.Path = path;
            var result = await _dichVuGiuongBenhVienService.XuLyKiemTraDataGiaDichVuBenhVienImportAsync(model);
            return Ok(result);
        }

        [HttpPost("KiemTraGiaDichVuBenhVienLoi")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucDichVuGiuongTaiBenhVien)]
        public async Task<ActionResult> KiemTraGiaDichVuBenhVienLoi(GiaDichVuCanKiemTraVo info)
        {
            var result = new GiaDichVuBenhVieDataImportVo();
            await _dichVuGiuongBenhVienService.KiemTraDataGiaDichVuBenhVienImportAsync(info.datas, result);
            return Ok(result);
        }

        [HttpPost("XuLyLuuGiaDichVuImport")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucDichVuGiuongTaiBenhVien)]
        public async Task<ActionResult> XuLyLuuGiaDichVuImport(GiaDichVuCanKiemTraVo info)
        {
            await _dichVuGiuongBenhVienService.XuLyLuuGiaDichVuImportAsync(info.datas);
            return Ok();
        }


        [HttpPost("GetNhomDichVuTheoDichVuTheoGiuong")]
        public async Task<ActionResult> GetNhomDichVuTheoDichVuTheoGiuong([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _dichVuGiuongBenhVienService.GetNhomDichVuTheoDichVuKyThuat(queryInfo);
            return Ok(lookup);
        }

        #endregion
    }
}