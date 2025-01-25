using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.Error;
using Camino.Api.Models.XuatKhos;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
//using Camino.Core.Domain.ValueObject.XuatKhos;
using Camino.Core.Domain.ValueObject.XuatKhoKSNK;
using Camino.Core.Domain.ValueObject.YeuCauHoanTra;
using Camino.Core.Helpers;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.KhoDuocPhams;
using Camino.Services.Localization;
using Camino.Services.NhapKhoVatTus;
using Camino.Services.VatTu;
using Camino.Services.XuatKhoKSNKs;
using Camino.Services.XuatKhos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.Controllers
{
    public class XuatKhoKSNKController : CaminoBaseController
    {
        private readonly IXuatKhoKSNKService _xuatKhoKSNKService;
        private readonly IXuatKhoService _xuatKhoService;

        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;
        private readonly IVatTuService _vatTuService;
        private readonly INhapKhoVatTuService _nhapKhoVatTuService;
        private readonly IKhoDuocPhamService _khoDuocPhamService;
        private readonly IUserAgentHelper _userAgentHelper;

        private readonly Camino.Services.XuatKhos.IXuatKhoService _xuatKhoDuocPhamService;

        public XuatKhoKSNKController(ILocalizationService localizationService
            , IVatTuService vatTuService, IKhoDuocPhamService khoDuocPhamService
            , INhapKhoVatTuService nhapKhoVatTuService, IXuatKhoService xuatKhoService
            , IExcelService excelService, IXuatKhoKSNKService xuatKhoKSNKService
            , Camino.Services.XuatKhos.IXuatKhoService xuatKhoDuocPhamService,
            IUserAgentHelper userAgentHelper)
        {
            _localizationService = localizationService;
            _excelService = excelService;
            _xuatKhoKSNKService = xuatKhoKSNKService;
            _vatTuService = vatTuService;
            _khoDuocPhamService = khoDuocPhamService;
            _nhapKhoVatTuService = nhapKhoVatTuService;
            _xuatKhoService = xuatKhoService;
            _userAgentHelper = userAgentHelper;
            _xuatKhoDuocPhamService = xuatKhoDuocPhamService;
        }

        #region Danh sách kiểm soát nhiễm khuẩn

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XuatKhoVatTuThuocNhomKSNK)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoKSNKService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XuatKhoVatTuThuocNhomKSNK)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoKSNKService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        #endregion

        #region Vật tư trong kiểm soát nhiễm khuẩn
        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XuatKhoVatTuThuocNhomKSNK)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> Get(long id)
        {
            var entity = await _xuatKhoKSNKService.GetByIdAsync(id,
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

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XuatKhoVatTuThuocNhomKSNK)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridChildAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoKSNKService.GetDataForGridVatTuChildAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XuatKhoVatTuThuocNhomKSNK)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridVaChildAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoKSNKService.GetTotalPageForGridVatTuChildAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("InPhieuXuatVatTu")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public Task<string> InPhieuXuatVatTu(long xuatKhoVatTuId, string hostingName)
        {
            var phieuXuat = _xuatKhoKSNKService.InPhieuXuatVatTu(xuatKhoVatTuId, hostingName);
            return phieuXuat;
        }

        #endregion

        #region Dược phẩm trong kiểm soát nhiễm khuẩn

        [HttpGet("GetDuocPhamXuatKhoKSNk/{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XuatKhoVatTuThuocNhomKSNK)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> GetDuocPhamXuatKhoKSNk(long id)
        {
            var entity = await _xuatKhoDuocPhamService.GetByIdAsync(id,
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
            var result = entity.ToModel<XuatKhoVatTuViewModel>();
            result.KhoNhapDisplay = entity.KhoDuocPhamNhap.Ten;
            result.KhoXuatDisplay = entity.KhoDuocPhamXuat.Ten;
            return Ok(result);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridDuocPhamChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XuatKhoVatTuThuocNhomKSNK)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridDuocPhamChildAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoKSNKService.GetDataForGridDuocPhamChildAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridDuocPhamChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XuatKhoVatTuThuocNhomKSNK)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridDuocPhamChildAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoKSNKService.GetTotalPageForGridDuocPhamChildAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("InPhieuXuatDuocPham")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public Task<string> InPhieuXuatDuocPham(long xuatKhoDuocPhamId, string hostingName)
        {
            var phieuXuat = _xuatKhoKSNKService.InPhieuXuatDuocPham(xuatKhoDuocPhamId, hostingName);
            return phieuXuat;
        }

        #endregion


        [HttpPost("IsValidateUpdateOrDelete")]
        public async Task<ActionResult> IsValidateUpdateOrDelete(long id)
        {
            var isValidate = await _xuatKhoKSNKService.IsValidateUpdateOrRemove(id);
            return Ok(isValidate);
        }

        [HttpDelete("DeleteItem")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.XuatKhoVatTuThuocNhomKSNK)]
        public async Task<ActionResult> DeleteItem(long id)
        {
            var entity = await _xuatKhoKSNKService.GetByIdAsync(id, p => p.Include(s => s.XuatKhoVatTuChiTiets).ThenInclude(s => s.XuatKhoVatTuChiTietViTris)
                                                                      .Include(s => s.NhapKhoVatTus).ThenInclude(s => s.NhapKhoVatTuChiTiets));
            if (entity == null)
            {
                return NotFound();
            }

            await _xuatKhoKSNKService.DeleteXuatKho(entity);
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

        [HttpPost("GetVatTuKSNK")]
        public async Task<ActionResult> GetVatTuKSNK([FromBody] ThemKSNK model)
        {
            var xuatKhoVatTuChiTiet = await _xuatKhoKSNKService.GetVatTu(model);

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
                    await _xuatKhoKSNKService.GetNhapKhoVatTuChiTietById(item.NhapKhoVatTuChiTietId);
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
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XuatKhoVatTuThuocNhomKSNK)]
        public async Task<ActionResult<GridDataSource>> GetAllVatTuData([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoKSNKService.GetAllDpVtKsnkData(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetAllVatTuTotal")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XuatKhoVatTuThuocNhomKSNK)]
        public async Task<ActionResult<GridDataSource>> GetAllVatTuTotal([FromBody] QueryInfo queryInfo)
        {
            //bỏ lazyLoadPage
            var gridData = await _xuatKhoKSNKService.GetAllDpVtKsnkData(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetVatTuOnGroup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> GetVatTuOnGroup([FromBody] GetDpVtOnGroupModel model)
        {
            var result = await _xuatKhoKSNKService.GetDpVtOnGroup(model.TenNhom, model.KhoXuatId, model.SearchString, model.ListDaChon);
            return Ok(result);
        }

        [HttpPost("GetKhoVatTu")]
        public async Task<ActionResult> GetKhoVatTu(DropDownListRequestModel model)
        {
            var result = await _xuatKhoKSNKService.GetKhoKSNK(model);
            return Ok(result);
        }

        [HttpPost("GetKhoVatTuNhap")]
        public async Task<ActionResult> GetKhoVatTuNhap(DropDownListRequestModel model)
        {
            var result = await _xuatKhoKSNKService.GetKhoKSNKNhap(model);
            return Ok(result);
        }

        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.XuatKhoVatTuThuocNhomKSNK)]
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

            if ((model.KhoXuatId != null && !await _xuatKhoKSNKService.IsKhoExists(model.KhoXuatId ?? 0))
                 || (model.KhoNhapId != null && !await _xuatKhoKSNKService.IsKhoExists(model.KhoNhapId ?? 0)))
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

            var thongTinXuatKhoKsnkVo = new ThongTinXuatKhoKsnkVo
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
                thongTinXuatKhoKsnkVo.ThongTinXuatKhoKsnkChiTietVos.Add(new ThongTinXuatKhoKsnkChiTietVo { Id = item.Id, SoLuongXuat = item.SoLuongXuat.Value });
            }

            var resultVo = await _xuatKhoKSNKService.XuatKhoKsnk(thongTinXuatKhoKsnkVo);

            //todo: update phieuXuat khi resultVo.XuatKhoDuocPhamId != null + resultVo.XuatKhoDuocPhamId != null

            var lstPhieuXuatDPVaVT = new List<string>();

            if (resultVo != null && resultVo.XuatKhoVatTuId != null)
            {
                var phieuXuatVT = await _xuatKhoKSNKService.InPhieuXuatVatTu(resultVo.XuatKhoVatTuId.Value, model.HostingName) + "<div class=\"pagebreak\"> </div>";
                lstPhieuXuatDPVaVT.Add(phieuXuatVT);
            }

            if (resultVo != null && resultVo.XuatKhoDuocPhamId != null)
            {
                var phieuXuatDP = await _xuatKhoKSNKService.InPhieuXuatDuocPham(resultVo.XuatKhoDuocPhamId.Value, model.HostingName);
                lstPhieuXuatDPVaVT.Add(phieuXuatDP);
            }

            return Ok(lstPhieuXuatDPVaVT);
        }


        [HttpPost("GetSoLuongTon")]
        public async Task<ActionResult> GetSoLuongTon([FromBody] ThemKSNKNoValidate model)
        {
            if (model.VatTuId == null || model.VatTuId == 0) return Ok(0);
            var total = await _xuatKhoKSNKService.GetSoLuongTon(model.VatTuId ?? 0, model.ChatLuong == 1 ? true : false, model.KhoId ?? 0);
            return Ok(total);
        }

        [HttpPost("ExportData")]
        public async Task<ActionResult> ExportData(QueryInfo queryInfo)
        {
            var gridData = await _xuatKhoKSNKService.GetDataForGridAsync(queryInfo, true);
            var data = gridData.Data.Select(p => (XuatKhoKSNKGridVo)p).ToList();
        
            var dataExcel = data.Map<List<XuatKhoKSNKExportExcel>>();

            foreach (var item in dataExcel)
            {
                if (item.LoaiDuocPhamVatTu == Enums.LoaiDuocPhamVatTu.LoaiVatTu)
                {
                    var gridChildData = await _xuatKhoKSNKService.GetDataForGridVatTuChildAsync(queryInfo, item.Id, true);
                    var dataChild = gridChildData.Data.Select(p => (XuatKhoKSNKChildrenGridVo)p).ToList();
                    var dataChildExcel = dataChild.Map<List<XuatKhoKSNKVatTuVaDuocPhamExportExcelChild>>();

                    item.XuatKhoKSNKExportExcelChild.AddRange(dataChildExcel);
                }

                if (item.LoaiDuocPhamVatTu == Enums.LoaiDuocPhamVatTu.LoaiDuocPham)
                {
                    var gridChildData = await _xuatKhoKSNKService.GetDataForGridDuocPhamChildAsync(queryInfo, item.Id, true);
                    var dataChild = gridChildData.Data.Select(p => (XuatKhoDuocPhamChildrenGridVo)p).ToList();
                    var dataChildExcel = dataChild.Map<List<XuatKhoKSNKVatTuVaDuocPhamExportExcelChild>>();

                    item.XuatKhoKSNKExportExcelChild.AddRange(dataChildExcel);
                }
            }

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(XuatKhoKSNKExportExcel.SoPhieu), "Số Phiếu"));
            lstValueObject.Add((nameof(XuatKhoKSNKExportExcel.KhoXuat), "Nơi Xuất"));
            lstValueObject.Add((nameof(XuatKhoKSNKExportExcel.KhoNhap), "Nơi Nhập"));
            lstValueObject.Add((nameof(XuatKhoKSNKExportExcel.LyDoXuatKho), "Lý Do Xuất"));
            lstValueObject.Add((nameof(XuatKhoKSNKExportExcel.NguoiNhan), "Người Nhận"));
            lstValueObject.Add((nameof(XuatKhoKSNKExportExcel.NguoiXuat), "Người Xuất"));
            lstValueObject.Add((nameof(XuatKhoKSNKExportExcel.NgayXuatDisplay), "Ngày Xuất"));
            lstValueObject.Add((nameof(XuatKhoKSNKExportExcel.XuatKhoKSNKExportExcelChild), ""));


            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Xuất kho kiểm soát nhiễm khuẩn");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=XuatKhoKSNK" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

    }
}
