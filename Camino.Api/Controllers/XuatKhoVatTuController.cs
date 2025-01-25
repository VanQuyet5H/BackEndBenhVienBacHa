using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.Error;
using Camino.Api.Models.NhapKhoVatTuChiTiets;
using Camino.Api.Models.NhapKhoVatTus;
using Camino.Api.Models.XuatKhos;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.XuatKhos;
using Camino.Core.Domain.ValueObject.YeuCauHoanTra;
using Camino.Core.Helpers;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.KhoDuocPhams;
using Camino.Services.Localization;
using Camino.Services.NhapKhoVatTus;
using Camino.Services.VatTu;
using Camino.Services.XuatKhos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.Controllers
{
    public class XuatKhoVatTuController : CaminoBaseController
    {
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;
        private readonly IXuatKhoVatTuService _xuatKhoVatTuService;
        private readonly IVatTuService _vatTuService;

        private readonly INhapKhoVatTuService _nhapKhoVatTuService;

        private readonly IKhoDuocPhamService _khoDuocPhamService;
        private readonly IXuatKhoService _xuatKhoService;
        private readonly IUserAgentHelper _userAgentHelper;

        public XuatKhoVatTuController(ILocalizationService localizationService
            , IVatTuService vatTuService, IKhoDuocPhamService khoDuocPhamService
            , INhapKhoVatTuService nhapKhoVatTuService, IXuatKhoService xuatKhoService
            , IExcelService excelService, IXuatKhoVatTuService xuatKhoVatTuService, IUserAgentHelper userAgentHelper)
        {
            _localizationService = localizationService;
            _excelService = excelService;
            _xuatKhoVatTuService = xuatKhoVatTuService;
            _vatTuService = vatTuService;
            _khoDuocPhamService = khoDuocPhamService;
            _nhapKhoVatTuService = nhapKhoVatTuService;
            _xuatKhoService = xuatKhoService;
            _userAgentHelper = userAgentHelper;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XuatKhoVatTu)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoVatTuService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XuatKhoVatTu)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoVatTuService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XuatKhoVatTu)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridChildAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoVatTuService.GetDataForGridChildAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XuatKhoVatTu)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridChildAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoVatTuService.GetTotalPageForGridChildAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("IsValidateUpdateOrDelete")]
        public async Task<ActionResult> IsValidateUpdateOrDelete(long id)
        {
            var isValidate = await _xuatKhoVatTuService.IsValidateUpdateOrRemove(id);
            return Ok(isValidate);
        }

        //Delete
        [HttpDelete("DeleteItem")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.XuatKhoDuocPham)]
        public async Task<ActionResult> DeleteItem(long id)
        {
            var entity = await _xuatKhoVatTuService.GetByIdAsync(id, p => p.Include(s => s.XuatKhoVatTuChiTiets).ThenInclude(s => s.XuatKhoVatTuChiTietViTris)
                                                                      .Include(s => s.NhapKhoVatTus).ThenInclude(s => s.NhapKhoVatTuChiTiets));
            if (entity == null)
            {
                return NotFound();
            }

            await _xuatKhoVatTuService.DeleteXuatKho(entity);
            return NoContent();
        }

        [HttpPost("GetNguoiXuat")]
        public async Task<ActionResult> GetNguoiXuat(DropDownListRequestModel model)
        {
            var result = await _xuatKhoService.GetNguoiXuat(model);
            return Ok(result);
        }

        [HttpPost("GetNguoiNhan")]
        public async Task<ActionResult> GetNguoiNhan(DropDownListRequestModel model)
        {
            var result = await _xuatKhoService.GetNguoiNhan(model);
            return Ok(result);
        }

        [HttpPost("GetVatTu")]
        public async Task<ActionResult> GetVatTu([FromBody] ThemVatTu model)
        {
            var xuatKhoVatTuChiTiet = await _xuatKhoVatTuService.GetVatTu(model);

            if (xuatKhoVatTuChiTiet == null)
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.ThemVatTu.SoLuongValidate"));
            }

            var result = new XuatKhoVatTuChiTietViewModel();
            result.TenVatTu =
                (await _vatTuService.GetByIdAsync(xuatKhoVatTuChiTiet.VatTuBenhVien.VatTus.Id)).Ten;
            result.ChatLuong = model.ChatLuong == 1 ? "Đạt" : "Không đạt";
            result.VatTuId = model.VatTuBenhVienId ?? 0;
            result.TongSoLuongXuat = model.SoLuongXuat ?? 0;
            result.DatChatLuong = model.ChatLuong == 1 ? true : false;

            //update 12/3/2020
            result.DonGiaBan = model.DonGia ?? 0;
            result.VAT = model.VAT ?? 0;
            result.ChietKhau = model.ChietKhau ?? 0;
            //

            if (!xuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.Any())
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.ThemVatTu.Required"));
            }
            foreach (var item in xuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris)
            {
                var nhapKhoVatTuChiTiet =
                    await _xuatKhoVatTuService.GetNhapKhoVatTuChiTietById(item.NhapKhoVatTuChiTietId);
                var child = new XuatKhoVatTuChiTietViTriViewModel();
                child.ViTri = nhapKhoVatTuChiTiet?.KhoViTri?.Ten;
                child.SoLo = nhapKhoVatTuChiTiet?.Solo;
                child.HanSuDungDisplay = nhapKhoVatTuChiTiet?.HanSuDung.ApplyFormatDate();
                child.SoLuongXuat = item.SoLuongXuat;
                child.NhapKhoVatTuChiTietId = item.NhapKhoVatTuChiTietId;

                result.XuatKhoVatTuChiTietViTris.Add(child);
            }
            return Ok(result);
        }

        [HttpPost("GetLoaiKhoVatTu")]
        public async Task<ActionResult> GetLoaiKhoVatTu(long id)
        {
            var entity = await _khoDuocPhamService.GetByIdAsync(id);
            return Ok(Convert.ToInt32(entity.LoaiKho));
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetAllVatTuData")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XuatKhoVatTu)]
        public async Task<ActionResult<GridDataSource>> GetAllVatTuData([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoVatTuService.GetAllVatTuData(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetAllVatTuTotal")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XuatKhoVatTu)]
        public async Task<ActionResult<GridDataSource>> GetAllVatTuTotal([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoVatTuService.GetAllVatTuTotal(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetVatTuOnGroup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> GetVatTuOnGroup([FromBody] GetVatTuOnGroupModel model)
        {
            var result = await _xuatKhoVatTuService.GetVatTuOnGroup(model.Id, model.KhoXuatId, model.SearchString, model.ListDaChon);
            return Ok(result);
        }

        [HttpPost("GetKhoVatTu")]
        public async Task<ActionResult> GetKhoVatTu(DropDownListRequestModel model)
        {
            var result = await _xuatKhoVatTuService.GetKhoVatTu(model);
            return Ok(result);
        }


        [HttpPost("GetKhoVatTuNhap")]
        public async Task<ActionResult> GetKhoVatTuNhap(DropDownListRequestModel model)
        {
            var result = await _xuatKhoVatTuService.GetKhoVatTuNhap(model);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XuatKhoVatTu)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> Get(long id)
        {
            var entity = await _xuatKhoVatTuService.GetByIdAsync(id,
                                        p => p.Include(x => x.XuatKhoVatTuChiTiets).ThenInclude(x => x.XuatKhoVatTuChiTietViTris)
                                                                                      .ThenInclude(x => x.NhapKhoVatTuChiTiet).ThenInclude(x => x.KhoViTri)
                                            .Include(x => x.XuatKhoVatTuChiTiets).ThenInclude(x => x.XuatKhoVatTu).ThenInclude(x => x.NhapKhoVatTus).ThenInclude(x => x.NhapKhoVatTuChiTiets)
                                            .Include(x => x.KhoVatTuNhap)
                                            .Include(x => x.KhoVatTuXuat)
                                            .Include(x => x.NguoiXuat).ThenInclude(x => x.User)
                                            .Include(x => x.NguoiNhan).ThenInclude(x => x.User)
                                            .Include(x => x.XuatKhoVatTuChiTiets).ThenInclude(x => x.VatTuBenhVien).ThenInclude(x => x.VatTus)
                                            );
            var result = entity.ToModel<XuatKhoVatTuViewModel>();
            result.KhoNhapDisplay = entity.KhoVatTuNhap.Ten;
            result.KhoXuatDisplay = entity.KhoVatTuXuat.Ten;
            return Ok(result);
        }

        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.XuatKhoVatTu)]
        public async Task<ActionResult> Post
            ([FromBody] XuatKhoVatTuViewModel model)
        {
            if (!model.XuatKhoVatTuChiTiets.Any())
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.KhoVatTuChiTiet.Required"));

            }

            if (!await _xuatKhoService.IsKhoManagerment(model.KhoXuatId ?? 0) || !await _xuatKhoService.IsKhoManagerment(model.KhoNhapId ?? 0))
            {
                throw new ApiException(_localizationService.GetResource("Kho.KhoNhapXuat.NotManagerment"));
            }

            if ((model.KhoXuatId != null && !await _xuatKhoVatTuService.IsKhoExists(model.KhoXuatId ?? 0))
                 || (model.KhoNhapId != null && !await _xuatKhoVatTuService.IsKhoExists(model.KhoNhapId ?? 0)))
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.KhoVatTu.NotExists"));
            }

            if (model.XuatKhoVatTuChiTiets.Any(p => p.SoLuongXuat > p.SoLuongTon))
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.KhoVatTuChiTiet.SoLuongTonMoreThanSoLuongXuat"));
            }

            if (model.XuatKhoVatTuChiTiets.Any(p => p.SoLuongXuat == 0 || p.SoLuongXuat == null))
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.KhoVatTuChiTiet.SoLuongXuatMoreThan0"));
            }

            var thongTinXuatKhoVatTuVo = new ThongTinXuatKhoVatTuVo
            {
                KhoXuatId = model.KhoXuatId.Value,
                KhoNhapId = model.KhoNhapId.Value,
                NguoiXuatId = model.NguoiXuatId.Value,
                NguoiNhanId = model.NguoiNhanId,
                LoaiNguoiNhan = model.LoaiNguoiNhan.Value,
                TenNguoiNhan = model.TenNguoiNhan,
                NgayXuat = model.NgayXuat.Value,
                LyDoXuatKho = model.LyDoXuatKho,
            };
            foreach (var item in model.XuatKhoVatTuChiTiets)
            {
                thongTinXuatKhoVatTuVo.ThongTinXuatKhoVatTuChiTietVos.Add(new ThongTinXuatKhoVatTuChiTietVo { Id = item.Id, SoLuongXuat = item.SoLuongXuat.Value });
            }

            var xuatKhoId = await _xuatKhoVatTuService.XuatKhoVatTu(thongTinXuatKhoVatTuVo);

            var phieuXuat = await _xuatKhoVatTuService.InPhieuXuat(xuatKhoId, model.HostingName);
            return Ok(phieuXuat);
        }
        // update 14/5/2021 --- can review lai
        [HttpPost("InPhieuXuat")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public Task<string> InPhieuXuat(long xuatKhoVatTuId, string hostingName)
        {
            var phieuXuat = _xuatKhoVatTuService.InPhieuXuat(xuatKhoVatTuId, hostingName);
            return phieuXuat;
        }
        [HttpPost("GetSoLuongTon")]
        public async Task<ActionResult> GetSoLuongTon([FromBody] ThemVatTuNoValidate model)
        {
            if (model.VatTuId == null || model.VatTuId == 0) return Ok(0);
            var total = await _xuatKhoVatTuService.GetSoLuongTon(model.VatTuId ?? 0, model.ChatLuong == 1 ? true : false, model.KhoId ?? 0);
            return Ok(total);
        }


        #region export excel
        [HttpPost("ExportData")]
        public async Task<ActionResult> ExportData(QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoVatTuService.GetDataForGridAsync(queryInfo, true);
            var data = gridData.Data.Select(p => (XuatKhoVatTuGridVo)p).ToList();
            var dataExcel = data.Map<List<XuatKhoVatTuExportExcel>>();

            foreach (var item in dataExcel)
            {
                var gridChildData = await _xuatKhoVatTuService.GetDataForGridChildAsync(queryInfo, item.Id, true);
                var dataChild = gridChildData.Data.Select(p => (XuatKhoVatTuChildrenGridVo)p).ToList();
                var dataChildExcel = dataChild.Map<List<XuatKhoVatTuExportExcelChild>>();
                item.XuatKhoVatTuExportExcelChild.AddRange(dataChildExcel);
            }

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(XuatKhoVatTuExportExcel.SoPhieu), "Số Phiếu"));
            lstValueObject.Add((nameof(XuatKhoVatTuExportExcel.KhoXuat), "Nơi Xuất"));
            lstValueObject.Add((nameof(XuatKhoVatTuExportExcel.KhoNhap), "Nơi Nhập"));
            lstValueObject.Add((nameof(XuatKhoVatTuExportExcel.LyDoXuatKho), "Lý Do Xuất"));
            lstValueObject.Add((nameof(XuatKhoVatTuExportExcel.NguoiNhan), "Người Nhận"));
            lstValueObject.Add((nameof(XuatKhoVatTuExportExcel.NguoiXuat), "Người Xuất"));
            lstValueObject.Add((nameof(XuatKhoVatTuExportExcel.NgayXuatDisplay), "Ngày Xuất"));
            lstValueObject.Add((nameof(XuatKhoVatTuExportExcel.XuatKhoVatTuExportExcelChild), ""));


            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Xuất kho vật tư");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=XuatKhoVatTu" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion export excel
    }
}
