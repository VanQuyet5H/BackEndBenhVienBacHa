using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.Error;
using Camino.Api.Models.NhapKhoDuocPhamChiTiets;
using Camino.Api.Models.NhapKhoDuocPhams;
using Camino.Api.Models.XuatKhos;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Camino.Core.Domain.Entities.NhapKhoDuocPhams;
using Camino.Core.Domain.Entities.XuatKhos;
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
using Camino.Services.NhapKhoDuocPhams;
using Camino.Services.Thuocs;
using Camino.Services.XuatKhos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCaching.Internal;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.Controllers
{
    public class XuatKhoDuocPhamController : CaminoBaseController
    {
        private readonly IKhoDuocPhamService _khoDuocPhamService;
        private readonly IXuatKhoService _xuatKhoService;
        private readonly IDuocPhamService _duocPhamService;
        private readonly INhapKhoDuocPhamService _nhapKhoDuocPhamService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;
        private readonly IUserAgentHelper _userAgentHelper;
        public XuatKhoDuocPhamController(IXuatKhoService xuatKhoService, IDuocPhamService duocPhamService
        , INhapKhoDuocPhamService nhapKhoDuocPhamService, ILocalizationService localizationService
            , IKhoDuocPhamService khoDuocPhamService, IExcelService excelService, IUserAgentHelper userAgentHelper)
        {
            _xuatKhoService = xuatKhoService;
            _duocPhamService = duocPhamService;
            _nhapKhoDuocPhamService = nhapKhoDuocPhamService;
            _localizationService = localizationService;
            _khoDuocPhamService = khoDuocPhamService;
            _excelService = excelService;
            _userAgentHelper = userAgentHelper;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XuatKhoDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XuatKhoDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XuatKhoDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridChildAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoService.GetDataForGridChildAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XuatKhoDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridChildAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoService.GetTotalPageForGridChildAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetKhoDuocPham")]
        public async Task<ActionResult> GetKhoDuocPham(DropDownListRequestModel model)
        {
            var result = await _xuatKhoService.GetKhoDuocPham(model);
            return Ok(result);
        }
        [HttpPost("GetKhoTheoLoaiDuocPham")]
        public async Task<ActionResult> GetKhoTheoLoaiDuocPham(DropDownListRequestModel model)
        {
            var result = await _xuatKhoService.GetKhoTheoLoaiDuocPham(model);
            return Ok(result);
        }
        [HttpPost("GetLoaiKhoDuocPham")]
        public async Task<ActionResult> GetLoaiKhoDuocPham(long id)
        {
            var entity = await _khoDuocPhamService.GetByIdAsync(id);
            return Ok(Convert.ToInt32(entity.LoaiKho));
        }

        [HttpPost("GetKhoDuocPhamNhap")]
        public async Task<ActionResult> GetKhoDuocPhamNhap(DropDownListRequestModel model)
        {
            var result = await _xuatKhoService.GetKhoDuocPhamNhap(model);
            return Ok(result);
        }


        [HttpPost("GetKhoLoaiDuocPhamNhap")]
        public async Task<ActionResult> GetKhoLoaiDuocPhamNhap(DropDownListRequestModel model)
        {
            var result = await _xuatKhoService.GetKhoLoaiDuocPhamNhap(model);
            return Ok(result);
        }

        [HttpPost("GetLoaiXuatKho")]
        public async Task<ActionResult> GetLoaiXuatKho(DropDownListRequestModel model)
        {
            var result = await _xuatKhoService.GetLoaiXuatKho(model);

            return Ok(result);
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

        [HttpPost("GetListDuocPham")]
        public async Task<ActionResult> GetListDuocPham(DropDownListRequestModel model)
        {
            var result = await _xuatKhoService.GetListDuocPham(model);
            return Ok(result);
        }

        [HttpPost("GetDuocPham")]
        public async Task<ActionResult> GetDuocPham([FromBody]ThemDuocPham model)
        {
            var xuatKhoDuocPhamChiTiet = await _xuatKhoService.GetDuocPham(model);

            if (xuatKhoDuocPhamChiTiet == null)
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.ThemDuocPham.SoLuongValidate"));
            }

            var result = new XuatKhoDuocPhamChiTietViewModel();
            result.TenDuocPham =
                (await _duocPhamService.GetByIdAsync(xuatKhoDuocPhamChiTiet.DuocPhamBenhVien.DuocPham.Id)).Ten;
            result.ChatLuong = model.ChatLuong == 1 ? "Đạt" : "Không đạt";
            result.DuocPhamId = model.DuocPhamBenhVienId ?? 0;
            result.TongSoLuongXuat = model.SoLuongXuat ?? 0;
            result.DatChatLuong = model.ChatLuong == 1 ? true : false;

            //update 12/3/2020
            result.DonGiaBan = model.DonGia ?? 0;
            result.VAT = model.VAT ?? 0;
            result.ChietKhau = model.ChietKhau ?? 0;
            //

            if (!xuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.Any())
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.ThemDuocPham.Required"));
            }
            foreach (var item in xuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris)
            {
                var nhapKhoDuocPhamChiTiet =
                    await _xuatKhoService.GetNhapKhoDuocPhamChiTietById(item.NhapKhoDuocPhamChiTietId);
                var child = new XuatKhoDuocPhamChiTietViTriViewModel();
                child.ViTri = nhapKhoDuocPhamChiTiet?.KhoDuocPhamViTri?.Ten;
                child.SoLo = nhapKhoDuocPhamChiTiet?.Solo;
                child.HanSuDungDisplay = nhapKhoDuocPhamChiTiet?.HanSuDung.ApplyFormatDate();
                child.SoLuongXuat = item.SoLuongXuat;
                child.NhapKhoDuocPhamChiTietId = item.NhapKhoDuocPhamChiTietId;

                result.XuatKhoDuocPhamChiTietViTris.Add(child);
            }
            return Ok(result);
        }

        [HttpPost("GetSoLuongTon")]
        public async Task<ActionResult> GetSoLuongTon([FromBody]ThemDuocPhamNoValidate model)
        {
            if (model.DuocPhamId == null || model.DuocPhamId == 0) return Ok(0);
            var total = await _xuatKhoService.GetSoLuongTon(model.DuocPhamId ?? 0, model.ChatLuong == 1 ? true : false, model.KhoId ?? 0);
            return Ok(total);
        }

        [HttpPost("GetDonGiaBan")]
        public async Task<ActionResult> GetDonGiaBan(long duocPhamId)
        {
            var donGia = await _xuatKhoService.GetDonGiaBan(duocPhamId);
            return Ok(donGia);
        }

        [HttpPost("IsValidateUpdateOrDelete")]
        public async Task<ActionResult> IsValidateUpdateOrDelete(long id)
        {
            var isValidate = await _xuatKhoService.IsValidateUpdateOrRemove(id);
            return Ok(isValidate);
        }


        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.XuatKhoDuocPham)]
        public async Task<ActionResult> Post
            ([FromBody]XuatKhoDuocPhamViewModel model)
        {
            if (!model.XuatKhoDuocPhamChiTiets.Any())
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.KhoDuocPhamChiTiet.Required"));

            }

            if (!await _xuatKhoService.IsKhoManagerment(model.KhoDuocPhamXuatId ?? 0) || !await _xuatKhoService.IsKhoManagerment(model.KhoDuocPhamNhapId ?? 0))
            {
                throw new ApiException(_localizationService.GetResource("Kho.KhoNhapXuat.NotManagerment"));
            }

            if ((model.KhoDuocPhamXuatId != null && !await _xuatKhoService.IsKhoExists(model.KhoDuocPhamXuatId ?? 0))
                 || (model.KhoDuocPhamNhapId != null && !await _xuatKhoService.IsKhoExists(model.KhoDuocPhamNhapId ?? 0)))
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.KhoDuocPham.NotExists"));
            }

            if (model.XuatKhoDuocPhamChiTiets.Any(p => p.SoLuongXuat > p.SoLuongTon))
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.KhoDuocPhamChiTiet.SoLuongTonMoreThanSoLuongXuat"));
            }

            if (model.XuatKhoDuocPhamChiTiets.Any(p => p.SoLuongXuat == 0 || p.SoLuongXuat == null))
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.KhoDuocPhamChiTiet.SoLuongXuatMoreThan0"));
            }



            var thongTinXuatKhoDuocPhamVo = new ThongTinXuatKhoDuocPhamVo
            {
                KhoXuatId = model.KhoDuocPhamXuatId.Value,
                KhoNhapId = model.KhoDuocPhamNhapId.Value,
                NguoiXuatId = model.NguoiXuatId.Value,
                NguoiNhanId = model.NguoiNhanId,
                LoaiNguoiNhan = model.LoaiNguoiNhan.Value,
                TenNguoiNhan = model.TenNguoiNhan,
                NgayXuat = model.NgayXuat.Value,
                LyDoXuatKho = model.LyDoXuatKho,
            };
            foreach (var item in model.XuatKhoDuocPhamChiTiets)
            {
                thongTinXuatKhoDuocPhamVo.ThongTinXuatKhoDuocPhamChiTietVos.Add(new ThongTinXuatKhoDuocPhamChiTietVo { Id = item.Id, SoLuongXuat = item.SoLuongXuat.Value });
            }

            var xuatKhoId = await _xuatKhoService.XuatKhoDuocPham(thongTinXuatKhoDuocPhamVo);

            var phieuXuat = await _xuatKhoService.InPhieuXuat(xuatKhoId, model.HostingName);
            return Ok(phieuXuat);
        }
        // update 14/52021 ---- review lại 
        [HttpPost("InPhieuXuat")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public Task<string> InPhieuXuat(long xuatKhoDuocPhamId, string hostingName)
        {
            var phieuXuatLinhBu = _xuatKhoService.InPhieuXuat(xuatKhoDuocPhamId, hostingName);
            return phieuXuatLinhBu;
        }


        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XuatKhoDuocPham)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> Get(long id)
        {
            var entity = await _xuatKhoService.GetByIdAsync(id,
                                        p => p.Include(x => x.XuatKhoDuocPhamChiTiets).ThenInclude(x => x.XuatKhoDuocPhamChiTietViTris)
                                                                                      .ThenInclude(x => x.NhapKhoDuocPhamChiTiet).ThenInclude(x => x.KhoDuocPhamViTri)
                                            .Include(x => x.XuatKhoDuocPhamChiTiets).ThenInclude(x => x.XuatKhoDuocPham).ThenInclude(x => x.NhapKhoDuocPhams).ThenInclude(x => x.NhapKhoDuocPhamChiTiets)
                                            .Include(x => x.KhoDuocPhamNhap)
                                            .Include(x => x.KhoDuocPhamXuat)
                                            .Include(x => x.NguoiXuat).ThenInclude(x => x.User)
                                            .Include(x => x.NguoiNhan).ThenInclude(x => x.User)
                                            .Include(x => x.XuatKhoDuocPhamChiTiets).ThenInclude(x => x.DuocPhamBenhVien).ThenInclude(x => x.DuocPham)
                                            .Include(x => x.XuatKhoDuocPhamChiTiets).ThenInclude(x => x.DuocPhamBenhVien).ThenInclude(x => x.DuocPhamBenhVienPhanNhom)

                                            );
            var result = entity.ToModel<XuatKhoDuocPhamViewModel>();
            return Ok(result);
        }

        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.XuatKhoDuocPham)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> Update([FromBody] XuatKhoDuocPhamViewModel model)
        {
            if (!model.XuatKhoDuocPhamChiTiets.Any())
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.KhoDuocPhamChiTiet.Required"));

            }

            if ((model.KhoDuocPhamXuatId != null && !await _xuatKhoService.IsKhoExists(model.KhoDuocPhamXuatId ?? 0))
                || (model.KhoDuocPhamNhapId != null && !await _xuatKhoService.IsKhoExists(model.KhoDuocPhamNhapId ?? 0)))
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.KhoDuocPham.NotExists"));
            }

            var entity = model.ToEntity<XuatKhoDuocPham>();

            foreach (var item in entity.XuatKhoDuocPhamChiTiets)
            {
                item.NgayXuat = entity.NgayXuat;
                foreach (var itemChild in item.XuatKhoDuocPhamChiTietViTris)
                {
                    itemChild.NgayXuat = entity.NgayXuat;
                }
            }

            var result = await _xuatKhoService.UpdateXuatKho(entity);
            return NoContent();
        }
        //Delete
        [HttpDelete("DeleteItem")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.XuatKhoDuocPham)]
        public async Task<ActionResult> DeleteItem(long id)
        {
            var entity = await _xuatKhoService.GetByIdAsync(id, p => p.Include(s => s.XuatKhoDuocPhamChiTiets).ThenInclude(s => s.XuatKhoDuocPhamChiTietViTris)
                                                                      .Include(s => s.NhapKhoDuocPhams).ThenInclude(s => s.NhapKhoDuocPhamChiTiets));
            if (entity == null)
            {
                return NotFound();
            }

            await _xuatKhoService.DeleteXuatKho(entity);
            return NoContent();
        }

        [HttpPost("ExportData")]
        public async Task<ActionResult> ExportData(QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoService.GetDataForGridAsync(queryInfo, true);
            var data = gridData.Data.Select(p => (XuatKhoDuocPhamGridVo)p).ToList();
            var dataExcel = data.Map<List<XuatKhoDuocPhanExportExcel>>();

            foreach (var item in dataExcel)
            {
                var gridChildData = await _xuatKhoService.GetDataForGridChildAsync(queryInfo, item.Id, true);
                var dataChild = gridChildData.Data.Select(p => (XuatKhoDuocPhamChildrenGridVo)p).ToList();
                var dataChildExcel = dataChild.Map<List<XuatKhoDuocPhanExportExcelChild>>();
                item.XuatKhoDuocPhanExportExcelChild.AddRange(dataChildExcel);
            }

            var lstValueObject = new List<(string, string)>
            {
                (nameof(XuatKhoDuocPhanExportExcel.SoPhieu), "Số Phiếu"),
                (nameof(XuatKhoDuocPhanExportExcel.KhoDuocPhamXuat), "Nơi Xuất"),
                (nameof(XuatKhoDuocPhanExportExcel.KhoDuocPhamNhap), "Nơi Nhập"),
                (nameof(XuatKhoDuocPhanExportExcel.LyDoXuatKho), "Lý Do Xuất"),
                (nameof(XuatKhoDuocPhanExportExcel.NguoiNhan), "Người Nhận"),
                (nameof(XuatKhoDuocPhanExportExcel.NguoiXuat), "Người Xuất"),
                (nameof(XuatKhoDuocPhanExportExcel.NgayXuatDisplay), "Ngày Xuất"),
                (nameof(XuatKhoDuocPhanExportExcel.XuatKhoDuocPhanExportExcelChild), "ANC")
            };


            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Xuất kho dược phẩm");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=XuatKho" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetAllDuocPhamData")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XuatKhoDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetAllDuocPhamData([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoService.GetAllDuocPhamData(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetAllDuocPhamTotal")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XuatKhoDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetAllDuocPhamTotal([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoService.GetAllDuocPhamTotal(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetDuocPhamOnGroup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> GetDuocPhamOnGroup([FromBody] GetDuocPhamOnGroupModel model)
        {
            var result = await _xuatKhoService.GetDuocPhamOnGroup(model.Id, model.KhoXuatId, model.SearchString, model.ListDaChon);
            return Ok(result);
        }
    }
}