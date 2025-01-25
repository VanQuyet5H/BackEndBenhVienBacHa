using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.Error;
using Camino.Api.Models.YeuCauHoanTraKSNK;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.YeuCauTraVatTus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.KhoKSNKs;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using Camino.Services.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.YeuCauTraDuocPhams;
using static Camino.Core.Domain.Enums;
using Camino.Core.Helpers;

namespace Camino.Api.Controllers
{
    public partial class YeuCauTraKSNKController
    {
        #region Danh sách yêu cầu hoàn trả

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.YeuCauHoanTraKSNK)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync
           ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _ycHoanTraKSNKService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.YeuCauHoanTraKSNK)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _ycHoanTraKSNKService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.YeuCauHoanTraKSNK)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridChildAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _ycHoanTraKSNKService.GetDataForGridChildAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.YeuCauHoanTraKSNK)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridChildAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _ycHoanTraKSNKService.GetTotalPageForGridChildAsync(queryInfo);
            return Ok(gridData);
        }

        #endregion

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.YeuCauHoanTraKSNK)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<YeuCauHoanTraKSNKViewModel>> Get(long id)
        {
            var ycHoanTraDpEntity = await _ycHoanTraKSNKService.GetByIdAsync(id,
                w => w.Include(e => e.KhoXuat)
                    .Include(e => e.KhoNhap)
                    .Include(e => e.NhanVienDuyet).ThenInclude(e => e.User)
                    .Include(e => e.NhanVienYeuCau).ThenInclude(e => e.User));

            if (ycHoanTraDpEntity == null)
            {
                return NotFound();
            }

            var ycHoanTraVm = ycHoanTraDpEntity.ToModel<YeuCauHoanTraKSNKViewModel>();

            return Ok(ycHoanTraVm);
        }

        [HttpPost("GetDataForGridAsyncKSNKTuTrucDaChon")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauHoanTraKSNK)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncKSNKTuTrucDaChon([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _ycHoanTraKSNKService.GetDataForGridAsyncDpVtKSNKTuTrucDaChon(queryInfo);
            //var gridData = await _ycHoanTraKSNKService.GetDataForGridAsyncKSNKTuTrucDaChon(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsyncKSNKTuTrucDaChon")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauHoanTraKSNK)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncKSNKTuTrucDaChon([FromBody]QueryInfo queryInfo)
        {
            //bo lazy load
            var gridData = await _ycHoanTraKSNKService.GetDataForGridAsyncDpVtKSNKTuTrucDaChon(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetDataForGridChildAsyncDaDuyetKSNK")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauHoanTraKSNK)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridChildAsyncDaDuyetKSNK([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _ycHoanTraKSNKService.GetDataForGridChildAsyncDaDuyetKSNK(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridChildAsyncDaDuyetKSNK")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauHoanTraKSNK)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridChildAsyncDaDuyetKSNK([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _ycHoanTraKSNKService.GetTotalPageForGridChildAsyncDaDuyetKSNK(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetDataForGridChildAsyncDuocPhamDaDuyetKSNK")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauHoanTraKSNK)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridChildAsyncDuocPhamDaDuyetKSNK([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _ycHoanTraKSNKService.GetDataForGridChildDuocPhamAsyncDaDuyet(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridChildAsyncDuocPhamDaDuyetKSNK")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauHoanTraKSNK)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridChildAsyncDuocPhamDaDuyetKSNK([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _ycHoanTraKSNKService.GetTotalPageForGridDuocPhamChildAsyncDaDuyet(queryInfo);
            return Ok(gridData);
        }




        [HttpGet("GetTrangThaiYeuCauHoanTraKSNK")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauHoanTraKSNK)]
        public async Task<ActionResult<TrangThaiDuyetVo>> GetTrangThaiYeuCauHoanTraKSNK(long yeuCauTraVTId)
        {
            var result = await _ycHoanTraKSNKService.GetTrangThaiYeuCauHoanTraKSNK(yeuCauTraVTId);
            return Ok(result);
        }

        [HttpPost("XuatKSNKHoanTraTheoNhom")]
        public async Task<ActionResult> XuatKSNKHoanTraTheoNhom(YeuCauHoanTraKSNKChiTietTheoKhoXuatVos model)
        {
            return Ok(model);
        }

        #region CRUD

        [HttpPost("ThemYeuCauHoanTraKSNKTuTruc")]
        [ClaimRequirement(SecurityOperation.Add, DocumentType.YeuCauHoanTraKSNK)]
        public async Task<ActionResult> ThemYeuCauHoanTraKSNKTuTruc(YeuCauHoanTraKSNKTuTrucViewModel model)
        {
            if (!model.YeuCauHoanTraKSNKChiTiets.Any())
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.KhoDuocPhamChiTiet.Required"));
            }
            if (model.YeuCauHoanTraKSNKChiTiets.All(z => z.SoLuongTra == 0))
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.KhoVatTuChiTiet.SoLuongXuatMoreThan0"));
            }
            var yeuCauhoanTraVatTu = model.ToEntity<YeuCauTraVatTu>();
            var yeuCauhoanTraDuocPham = model.ToEntity<YeuCauTraDuocPham>();
            var result = await _ycHoanTraKSNKService.XuLyThemHoanTraKSNKAsync(yeuCauhoanTraVatTu, yeuCauhoanTraDuocPham, model.YeuCauHoanTraKSNKChiTiets);
            return Ok(result);
        }


        [HttpGet("GetYeuCauHoanTraKSNKTuTruc")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauHoanTraKSNK)]
        public async Task<ActionResult<YeuCauHoanTraKSNKTuTrucViewModel>> GetYeuCauHoanTraKSNKTuTruc(long id)
        {
            var yeuCauhoanTra = await _ycHoanTraKSNKService
                .GetByIdAsync(id, s =>
                            s.Include(r => r.YeuCauTraVatTuChiTiets).ThenInclude(ct => ct.VatTuBenhVien).ThenInclude(dpct => dpct.VatTus)
                             .Include(r => r.YeuCauTraVatTuChiTiets).ThenInclude(ct => ct.XuatKhoVatTuChiTietViTri).ThenInclude(dpct => dpct.XuatKhoVatTuChiTiet).ThenInclude(dp => dp.XuatKhoVatTu)
                             .Include(r => r.KhoNhap)
                             .Include(r => r.KhoXuat)
                             .Include(r => r.NhanVienDuyet).ThenInclude(r => r.User)
                             .Include(r => r.NhanVienYeuCau).ThenInclude(r => r.User)
                             )
                             ;
            if (yeuCauhoanTra == null)
            {
                return NotFound();
            }
            var model = yeuCauhoanTra.ToModel<YeuCauHoanTraKSNKTuTrucViewModel>();
            model.YeuCauHoanTraKSNKChiTietHienThis = await _ycHoanTraKSNKService.YeuCauHoanTraKSNKChiTiets(id);
            return Ok(model);
        }

        [HttpPost("CapNhatYeuCauHoanTraKSNKTuTruc")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.YeuCauHoanTraKSNK)]
        public async Task<ActionResult> CapNhatYeuCauHoanTraKSNKTuTruc(YeuCauHoanTraKSNKTuTrucViewModel yeuCauhoanTraVM)
        {
            if (!yeuCauhoanTraVM.YeuCauHoanTraKSNKChiTiets.Any())
            {
                throw new ApiException(
                    _localizationService.GetResource(
                        "DieuChuyenNoiBoDuocPham.YeuCauDieuChuyenDuocPhamChiTiets.Required"));
            }
            if (yeuCauhoanTraVM.YeuCauHoanTraKSNKChiTiets.All(z => z.SoLuongTra == 0))
            {
                throw new ApiException(
                    _localizationService.GetResource("XuatKho.KhoVatTuChiTiet.SoLuongXuatMoreThan0"));
            }
            if (yeuCauhoanTraVM.LoaiDuocPhamVatTu == LoaiDuocPhamVatTu.LoaiVatTu)
            {
                var yeuCauhoanTra = _ycHoanTraKSNKService
                        .GetById(yeuCauhoanTraVM.Id, s =>
                            s.Include(r => r.YeuCauTraVatTuChiTiets).ThenInclude(ct => ct.VatTuBenhVien)
                                .ThenInclude(dpct => dpct.VatTus)
                                .Include(r => r.YeuCauTraVatTuChiTiets).ThenInclude(ct => ct.XuatKhoVatTuChiTietViTri)
                                .ThenInclude(dpct => dpct.XuatKhoVatTuChiTiet).ThenInclude(dp => dp.XuatKhoVatTu)
                                .Include(r => r.YeuCauTraVatTuChiTiets).ThenInclude(ct => ct.XuatKhoVatTuChiTietViTri)
                                .ThenInclude(ct => ct.NhapKhoVatTuChiTiet)
                                .Include(r => r.KhoNhap)
                                .Include(r => r.KhoXuat)
                                .Include(r => r.NhanVienDuyet).ThenInclude(r => r.User)
                                .Include(r => r.NhanVienYeuCau).ThenInclude(r => r.User)
                        )
                    ;

                if (yeuCauhoanTra == null)
                {
                    throw new ApiException("Yêu cầu hoàn trả này không tồn tại.");
                }
                if (yeuCauhoanTra.DuocDuyet != null)
                {
                    throw new ApiException("Yêu cầu hoàn trả này đã được duyệt.");
                }
                yeuCauhoanTraVM.ToEntity(yeuCauhoanTra);
                await _ycHoanTraKSNKService.XuLyCapNhatHoanTraVatTuKSNKAsync(yeuCauhoanTra,
                    yeuCauhoanTraVM.YeuCauHoanTraKSNKChiTiets);
                //await _ycHoanTraKSNKService.XuLyThemHoacCapNhatHoanTraKSNKAsync(yeuCauhoanTra, yeuCauhoanTraVM.YeuCauHoanTraKSNKChiTiets, false);
                var result = new
                {
                    yeuCauhoanTra.Id,
                    yeuCauhoanTra.LastModified
                };
                return Ok(result);
            }
            else
            {
                var yeuCauhoanTra = _ycHoanTraDuocPhamService
                        .GetById(yeuCauhoanTraVM.Id, s =>
                            s.Include(r => r.YeuCauTraDuocPhamChiTiets).ThenInclude(ct => ct.DuocPhamBenhVien).ThenInclude(dpct => dpct.DuocPham)
                                .Include(r => r.YeuCauTraDuocPhamChiTiets).ThenInclude(ct => ct.XuatKhoDuocPhamChiTietViTri).ThenInclude(dpct => dpct.XuatKhoDuocPhamChiTiet).ThenInclude(dp => dp.XuatKhoDuocPham)
                                .Include(r => r.YeuCauTraDuocPhamChiTiets).ThenInclude(ct => ct.XuatKhoDuocPhamChiTietViTri).ThenInclude(ct => ct.NhapKhoDuocPhamChiTiet)
                                .Include(r => r.KhoNhap)
                                .Include(r => r.KhoXuat)
                                .Include(r => r.NhanVienDuyet).ThenInclude(r => r.User)
                                .Include(r => r.NhanVienYeuCau).ThenInclude(r => r.User)
                        )
                    ;

                if (yeuCauhoanTra == null)
                {
                    throw new ApiException("Yêu cầu hoàn trả này không tồn tại.");
                }
                if (yeuCauhoanTra.DuocDuyet != null)
                {
                    throw new ApiException("Yêu cầu hoàn trả này đã được duyệt.");
                }
                yeuCauhoanTraVM.ToEntity(yeuCauhoanTra);
                await _ycHoanTraKSNKService.XuLyCapNhatHoanTraDuocPhamKSNKAsync(yeuCauhoanTra,yeuCauhoanTraVM.YeuCauHoanTraKSNKChiTiets);
                var result = new
                {
                    yeuCauhoanTra.Id,
                    yeuCauhoanTra.LastModified
                };
                return Ok(result);
            }
        }
        #endregion

        [HttpPost("ExportYeuCauTraKSNK")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.YeuCauHoanTraKSNK)]
        public async Task<ActionResult> ExportYeuCauTraKSNK(QueryInfo queryInfo)
        {
            var gridData = await _ycHoanTraKSNKService.GetDataForGridAsync(queryInfo, true);
            var data = gridData.Data.Select(p => (DanhSachYeuCauHoanTraKSNKGridVo)p).ToList();
            var excelData = data.Map<List<DanhSachYeuCauHoanTraKSNKExportExcel>>();

            queryInfo.Sort = new List<Sort>();
            queryInfo.Sort.Add(new Sort
            {
                Field = "VatTu",
                Dir = "asc"
            });

            foreach (var item in excelData)
            {
                if (item.LoaiDuocPhamVatTu == LoaiDuocPhamVatTu.LoaiVatTu)
                {
                    queryInfo.AdditionalSearchString = item.Id + "";
                    var gridChildData = await _ycHoanTraKSNKService.GetDataForGridChildAsync(queryInfo);
                    var dataChild = gridChildData.Data.Select(p => (DanhSachYCHoanTraKSNKChiTietGridVo)p).ToList();
                    var dataChildExcel = dataChild.Map<List<DanhSachYeuCauHoanTraKSNKChiTietExportExcelChild>>();
                    item.DanhSachYeuCauHoanTraKSNKExportExcelChild.AddRange(dataChildExcel);
                }
                else if (item.LoaiDuocPhamVatTu == LoaiDuocPhamVatTu.LoaiDuocPham)
                {
                    queryInfo.AdditionalSearchString = item.Id + "";
                    var gridChildData = await _ycHoanTraKSNKService.GetDataForGridDuocPhamChildAsync(queryInfo);
                    var dataChild = gridChildData.Data.Select(p => (DanhSachYCHoanTraKSNKChiTietGridVo)p).ToList();
                    var dataChildExcel = dataChild.Map<List<DanhSachYeuCauHoanTraKSNKChiTietExportExcelChild>>();
                    item.DanhSachYeuCauHoanTraKSNKExportExcelChild.AddRange(dataChildExcel);
                }
            }

            var lstValueObject = new List<(string, string)>
            {
                (nameof(DanhSachYeuCauHoanTraKSNKExportExcel.Ma), "Số chứng từ"),
                (nameof(DanhSachYeuCauHoanTraKSNKExportExcel.NguoiYeuCau), "Người yêu cầu"),
                (nameof(DanhSachYeuCauHoanTraKSNKExportExcel.KhoHoanTraTu), "Hoàn trả từ kho"),
                (nameof(DanhSachYeuCauHoanTraKSNKExportExcel.KhoHoanTraVe), "Hoàn trả về kho"),
                (nameof(DanhSachYeuCauHoanTraKSNKExportExcel.NgayYeuCauText), "Ngày yêu cầu"),
                (nameof(DanhSachYeuCauHoanTraKSNKExportExcel.TinhTrangDisplay), "Tình trạng"),
                (nameof(DanhSachYeuCauHoanTraKSNKExportExcel.NguoiDuyet), "Người duyệt"),
                (nameof(DanhSachYeuCauHoanTraKSNKExportExcel.NgayDuyetText), "Ngày duyệt"),
                (nameof(DanhSachYeuCauHoanTraKSNKExportExcel.DanhSachYeuCauHoanTraKSNKExportExcelChild), "")
            };

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Yêu cầu hoàn trả kiểm soát nhiễm khuẩn", 2, "Danh sách yêu cầu hoàn trả kiểm soát nhiễm khuẩn");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=YcHoanTraKSNK" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetAllVatTuData")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.YeuCauHoanTraKSNK)]
        public async Task<ActionResult<GridDataSource>> GetAllVatTuData([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _ycHoanTraKSNKService.GetAllDpVtKsnkData(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetAllVatTuTotal")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.YeuCauHoanTraKSNK)]
        public async Task<ActionResult<GridDataSource>> GetAllVatTuTotal([FromBody] QueryInfo queryInfo)
        {
            //bo lazy load
            var gridData = await _ycHoanTraKSNKService.GetAllDpVtKsnkData(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetKhoDPvaVTKSNK")]
        public async Task<ActionResult> GetKhoDPvaVTKSNK(DropDownListRequestModel model)
        {
            var result = await _ycHoanTraKSNKService.GetKhoDPvaVTKSNK(model);
            return Ok(result);
        }

        [HttpPost("GetKhoVatTuHoanTra")]
        public async Task<ActionResult> GetKhoVatTuHoanTra(DropDownListRequestModel model)
        {
            var result = await _ycHoanTraKSNKService.GetKhoVatTuHoanTra(model);
            return Ok(result);
        }

        [HttpPost("GetVatTuOnGroup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> GetVatTuOnGroup([FromBody] GetKSNKOnGroupModel model)
        {
            var result = await _ycHoanTraKSNKService.GetVatTuOnGroup(model.Id, model.KhoXuatId, model.SearchString, model.ListDaChon);
            return Ok(result);
        }

        [HttpPost("GuiPhieuHoanTraVatTu")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.YeuCauHoanTraKSNK)]
        public async Task<ActionResult> Post
           ([FromBody] YeuCauHoanTraKSNKViewModel model)
        {
            if (!model.YeuCauTraKSNKChiTiets.Any())
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.YeuCauTraVatTuChiTiet.Required"));

            }

            if ((model.KhoXuatId != null && !await _xuatKhoService.IsKhoExists(model.KhoXuatId ?? 0))
                 || (model.KhoNhapId != null && !await _xuatKhoService.IsKhoExists(model.KhoNhapId ?? 0)))
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.KhoVatTu.NotExists"));
            }

            if (model.YeuCauTraKSNKChiTiets.Any(p => p.SoLuongXuat > p.SoLuongTon))
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.KhoVatTuChiTiet.SoLuongTonMoreThanSoLuongXuat"));
            }

            if (model.YeuCauTraKSNKChiTiets.Any(p => p.SoLuongXuat == 0 || p.SoLuongXuat == null))
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.KhoVatTuChiTiet.SoLuongXuatMoreThan0"));
            }

            var entity = model.ToEntity<YeuCauTraVatTu>();

            foreach (var item in model.YeuCauTraKSNKChiTiets)
            {
                var modelThemVatTuHoanTra = new ThemKSNKHoanTra
                {
                    NhapKhoVatTuChiTietId = long.Parse(item.Id.Split(",")[0]),
                    NhomVatTu = item.Id.Split(",")[1] == Enums.LoaiSuDung.VatTuTieuHao.GetDescription() ? Enums.LoaiSuDung.VatTuTieuHao : Enums.LoaiSuDung.VatTuThayThe,
                    SoLuongXuat = item.SoLuongXuat,
                    KhoId = model.KhoXuatId,
                    LaVatTuBHYT = item.LaVatTuBHYT,
                };

                var xuatKhoVatTuChiTiet = await _ycHoanTraKSNKService.GetVatTu(modelThemVatTuHoanTra);

                entity.YeuCauTraVatTuChiTiets.Add(xuatKhoVatTuChiTiet);
            }

            _ycHoanTraKSNKService.Add(entity);
            foreach (var item in model.YeuCauTraKSNKChiTiets)
            {
                var nhapKhoVatTuChiTietIdUpdate = long.Parse(item.Id.Split(",")[0]);
                await _ycHoanTraKSNKService.UpdateGiaChoNhapKhoChiTiet(item.SoLuongXuat ?? 0, nhapKhoVatTuChiTietIdUpdate);
            }


            return Ok(entity.Id);
        }


        [HttpGet("GetUpdate")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.YeuCauHoanTraKSNK)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> GetUpdate(long id)
        {
            var entity = await _ycHoanTraKSNKService.GetByIdAsync(id,
                                        p => p.Include(x => x.YeuCauTraVatTuChiTiets).ThenInclude(x => x.XuatKhoVatTuChiTietViTri).ThenInclude(x => x.NhapKhoVatTuChiTiet)
                                       .Include(x => x.YeuCauTraVatTuChiTiets).ThenInclude(x => x.VatTuBenhVien).ThenInclude(x => x.VatTus)
                                       .Include(x => x.YeuCauTraVatTuChiTiets).ThenInclude(x => x.HopDongThauVatTu).ThenInclude(x => x.NhaThau)
                                       .Include(x => x.KhoXuat).Include(x => x.KhoNhap)
                                       .Include(x => x.NhanVienDuyet).ThenInclude(x => x.User)
                                       .Include(x => x.NhanVienYeuCau).ThenInclude(x => x.User));
            var result = entity.ToModel<YeuCauHoanTraKSNKViewModel>();
            foreach (var yeuCauTraVatTuChiTiet in entity.YeuCauTraVatTuChiTiets)
            {
                var chiTiet = result.YeuCauTraKSNKChiTiets.FirstOrDefault(p => p.Id == yeuCauTraVatTuChiTiet.Id + "");
                if (chiTiet != null)
                {
                    chiTiet.VatTu = yeuCauTraVatTuChiTiet.VatTuBenhVien.VatTus.Ten;
                    chiTiet.DVT = yeuCauTraVatTuChiTiet.VatTuBenhVien.VatTus.DonViTinh;
                    chiTiet.LaVatTuBHYT = yeuCauTraVatTuChiTiet.LaVatTuBHYT;
                    chiTiet.MaVatTu = yeuCauTraVatTuChiTiet.VatTuBenhVien.Ma;
                    chiTiet.SoLo = yeuCauTraVatTuChiTiet.Solo;
                    chiTiet.SoLuongTon =
                        (yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri?.NhapKhoVatTuChiTiet.SoLuongNhap ?? 0) -
                        (yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri?.NhapKhoVatTuChiTiet.SoLuongDaXuat ?? 0);
                    chiTiet.HanSuDung = yeuCauTraVatTuChiTiet.HanSuDung;
                    chiTiet.NgayNhap = yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri?.NhapKhoVatTuChiTiet
                        .NgayNhap;

                    chiTiet.Nhom = yeuCauTraVatTuChiTiet.VatTuBenhVien.LoaiSuDung.GetValueOrDefault().GetDescription();
                    chiTiet.SoLuongXuat = yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri?.SoLuongXuat ?? 0;
                    chiTiet.TiLeThapGia = yeuCauTraVatTuChiTiet.TiLeTheoThapGia;
                    chiTiet.Vat = yeuCauTraVatTuChiTiet.VAT;
                    chiTiet.Id = yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri?.NhapKhoVatTuChiTietId + "," +
                                 yeuCauTraVatTuChiTiet.VatTuBenhVien.LoaiSuDung.GetValueOrDefault().GetDescription() + "," +
                                 (yeuCauTraVatTuChiTiet.LaVatTuBHYT ? "true" : "false");
                }
            }

            return Ok(result);
        }

        [HttpPost("GuiLaiPhieuHoanTraVatTu")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.YeuCauHoanTraKSNK)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> Update([FromBody] YeuCauHoanTraKSNKViewModel model)
        {
            if (!model.YeuCauTraKSNKChiTiets.Any())
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.KhoVatTuChiTiet.Required"));

            }

            if (model.KhoXuatId != null && !await _xuatKhoService.IsKhoExists(model.KhoXuatId ?? 0)
                || model.KhoNhapId != null && !await _xuatKhoService.IsKhoExists(model.KhoNhapId ?? 0))
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.KhoVatTu.NotExists"));
            }

            var lstModelThemVatTuHoanTra = new List<ThemKSNKHoanTra>();
            foreach (var item in model.YeuCauTraKSNKChiTiets)
            {
                var nhapKhoVatTuChiTietId = long.Parse(item.Id.Split(",")[0]);
                var modelThemVatTuHoanTra = new ThemKSNKHoanTra
                {
                    NhapKhoVatTuChiTietId = nhapKhoVatTuChiTietId,
                    NhomVatTu = item.Id.Split(",")[1] == Enums.LoaiSuDung.VatTuTieuHao.GetDescription() ? Enums.LoaiSuDung.VatTuTieuHao : Enums.LoaiSuDung.VatTuThayThe,
                    SoLuongXuat = item.SoLuongXuat,
                    KhoId = model.KhoXuatId,
                    LaVatTuBHYT = item.LaVatTuBHYT,
                    SoLuongTon = item.SoLuongTon
                };
                lstModelThemVatTuHoanTra.Add(modelThemVatTuHoanTra);
            }

            if (await _ycHoanTraKSNKService.CheckValidSlTon(lstModelThemVatTuHoanTra, model.Id) == false)
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.KhoVatTuChiTiet.SoLuongTonMoreThanSoLuongXuat"));
            }

            var entity = await _ycHoanTraKSNKService.GetByIdAsync(model.Id,
                u => u.Include(x => x.YeuCauTraVatTuChiTiets).ThenInclude(x => x.XuatKhoVatTuChiTietViTri).ThenInclude(x => x.XuatKhoVatTuChiTiet)
                    .Include(x => x.YeuCauTraVatTuChiTiets).ThenInclude(x => x.XuatKhoVatTuChiTietViTri).ThenInclude(x => x.NhapKhoVatTuChiTiet)
                );

            entity.KhoNhapId = model.KhoNhapId.GetValueOrDefault();
            entity.GhiChu = model.GhiChu;

            foreach (var item in model.YeuCauTraKSNKChiTiets)
            {
                var id = long.Parse(item.Id.Split(",")[0]);

                if (entity.YeuCauTraVatTuChiTiets.All(c => c.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTietId != id))
                {
                    var modelThemVatTuHoanTra = new ThemKSNKHoanTra
                    {
                        NhapKhoVatTuChiTietId = long.Parse(item.Id.Split(",")[0]),
                        NhomVatTu = item.Id.Split(",")[1] == Enums.LoaiSuDung.VatTuTieuHao.GetDescription() ? Enums.LoaiSuDung.VatTuTieuHao : Enums.LoaiSuDung.VatTuThayThe,
                        SoLuongXuat = item.SoLuongXuat,
                        KhoId = model.KhoXuatId,
                        LaVatTuBHYT = item.LaVatTuBHYT
                    };

                    var xuatKhoVatTuChiTiet = await _ycHoanTraKSNKService.GetVatTu(modelThemVatTuHoanTra);

                    entity.YeuCauTraVatTuChiTiets.Add(xuatKhoVatTuChiTiet);
                    await _ycHoanTraKSNKService.UpdateSlXuatNhapKhoChiTiet(item.SoLuongXuat ?? 0, id);
                }
                else
                {
                    if (!entity.YeuCauTraVatTuChiTiets.Any()) continue;
                    foreach (var ycTraDpModify in entity.YeuCauTraVatTuChiTiets.Where(c => c.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTietId == id))
                    {
                        ycTraDpModify.SoLuongTra = item.SoLuongXuat.GetValueOrDefault();
                        ycTraDpModify.XuatKhoVatTuChiTietViTri.NgayXuat = model.NgayYeuCau;
                        ycTraDpModify.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.NgayXuat = model.NgayYeuCau;
                        ycTraDpModify.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.SoLuongDaXuat =
                            ycTraDpModify.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.SoLuongDaXuat -
                            ycTraDpModify.XuatKhoVatTuChiTietViTri.SoLuongXuat +
                            item.SoLuongXuat.GetValueOrDefault();
                        ycTraDpModify.XuatKhoVatTuChiTietViTri.SoLuongXuat = item.SoLuongXuat.GetValueOrDefault();
                    }
                }
            }

            var lstEntityCanXoa = entity.YeuCauTraVatTuChiTiets
                .Select(c => c.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTietId)
                .Where(q => !model.YeuCauTraKSNKChiTiets.Select(c => long.Parse(c.Id.Split(",")[0])).Contains(q) &&
                            q != 0);

            foreach (var entityYeuCauTraVatTuChiTiet in lstEntityCanXoa)
            {
                if (entity.YeuCauTraVatTuChiTiets.Any(c => c.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTietId == entityYeuCauTraVatTuChiTiet))
                {
                    var entityYcTraVtChiTiet = entity.YeuCauTraVatTuChiTiets.First(c =>
                        c.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTietId == entityYeuCauTraVatTuChiTiet);

                    entityYcTraVtChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet
                            .SoLuongDaXuat =
                        entityYcTraVtChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet
                            .SoLuongDaXuat - entityYcTraVtChiTiet.XuatKhoVatTuChiTietViTri.SoLuongXuat;
                    entityYcTraVtChiTiet.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.WillDelete =
                        true;
                    entityYcTraVtChiTiet.XuatKhoVatTuChiTietViTri.WillDelete =
                        true;
                    entityYcTraVtChiTiet.WillDelete =
                        true;
                }
            }

            await _ycHoanTraKSNKService.UpdateAsync(entity);

            return Ok(entity.Id);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("SaveEveryMovingVatTu")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.YeuCauHoanTraDuocPham)]
        public async Task<ActionResult<YeuCauHoanTraKSNKViewModel>> SaveEveryMovingVatTu([FromBody] YeuCauHoanTraKSNKViewModelIgnoreValidate model)
        {
            var entity = await _ycHoanTraKSNKService.GetByIdAsync(model.Id,
                u => u.Include(x => x.YeuCauTraVatTuChiTiets).ThenInclude(x => x.XuatKhoVatTuChiTietViTri).ThenInclude(x => x.XuatKhoVatTuChiTiet)
                    .Include(x => x.YeuCauTraVatTuChiTiets).ThenInclude(x => x.XuatKhoVatTuChiTietViTri).ThenInclude(x => x.NhapKhoVatTuChiTiet)
            );


            foreach (var item in model.YeuCauTraKSNKChiTiets)
            {
                var id = long.Parse(item.Id.Split(",")[0]);

                if (entity.YeuCauTraVatTuChiTiets.All(c => c.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTietId != id))
                {
                    var modelThemVatTuHoanTra = new ThemKSNKHoanTra
                    {
                        NhapKhoVatTuChiTietId = long.Parse(item.Id.Split(",")[0]),
                        NhomVatTu = item.Id.Split(",")[1] == Enums.LoaiSuDung.VatTuTieuHao.GetDescription() ? Enums.LoaiSuDung.VatTuTieuHao : Enums.LoaiSuDung.VatTuThayThe,
                        SoLuongXuat = item.SoLuongXuat,
                        KhoId = model.KhoXuatId,
                        LaVatTuBHYT = item.LaVatTuBHYT
                    };

                    var xuatKhoVatTuChiTiet = await _ycHoanTraKSNKService.GetVatTu(modelThemVatTuHoanTra);

                    entity.YeuCauTraVatTuChiTiets.Add(xuatKhoVatTuChiTiet);
                    await _ycHoanTraKSNKService.UpdateSlXuatNhapKhoChiTiet(item.SoLuongXuat ?? 0, id);
                }
                else
                {
                    if (!entity.YeuCauTraVatTuChiTiets.Any()) continue;
                    foreach (var ycTraDpModify in entity.YeuCauTraVatTuChiTiets.Where(c => c.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTietId == id))
                    {
                        ycTraDpModify.SoLuongTra = item.SoLuongXuat.GetValueOrDefault();
                        ycTraDpModify.XuatKhoVatTuChiTietViTri.NgayXuat = model.NgayYeuCau;
                        ycTraDpModify.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.NgayXuat = model.NgayYeuCau;
                        ycTraDpModify.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.SoLuongDaXuat =
                            ycTraDpModify.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.SoLuongDaXuat -
                            ycTraDpModify.XuatKhoVatTuChiTietViTri.SoLuongXuat +
                            item.SoLuongXuat.GetValueOrDefault();
                        ycTraDpModify.XuatKhoVatTuChiTietViTri.SoLuongXuat = item.SoLuongXuat.GetValueOrDefault();
                    }
                }
            }

            var lstEntityCanXoa = entity.YeuCauTraVatTuChiTiets
                .Select(c => c.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTietId)
                .Where(q => !model.YeuCauTraKSNKChiTiets.Select(c => long.Parse(c.Id.Split(",")[0])).Contains(q) &&
                            q != 0);

            foreach (var entityYeuCauTraVatTuChiTiet in lstEntityCanXoa)
            {
                if (entity.YeuCauTraVatTuChiTiets.Any(c => c.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTietId == entityYeuCauTraVatTuChiTiet))
                {
                    var entityYcTraVtChiTiet = entity.YeuCauTraVatTuChiTiets.First(c =>
                        c.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTietId == entityYeuCauTraVatTuChiTiet);

                    entityYcTraVtChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet
                            .SoLuongDaXuat =
                        entityYcTraVtChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet
                            .SoLuongDaXuat - entityYcTraVtChiTiet.XuatKhoVatTuChiTietViTri.SoLuongXuat;
                    entityYcTraVtChiTiet.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.WillDelete =
                        true;
                    entityYcTraVtChiTiet.XuatKhoVatTuChiTietViTri.WillDelete =
                        true;
                    entityYcTraVtChiTiet.WillDelete =
                        true;
                }
            }

            await _ycHoanTraKSNKService.UpdateAsync(entity);

            var entityResult = await _ycHoanTraKSNKService.GetByIdAsync(model.Id,
                                        p => p.Include(x => x.YeuCauTraVatTuChiTiets).ThenInclude(x => x.XuatKhoVatTuChiTietViTri)
                                            .ThenInclude(x => x.NhapKhoVatTuChiTiet)
                                            .Include(x => x.YeuCauTraVatTuChiTiets).ThenInclude(x => x.HopDongThauVatTu).ThenInclude(x => x.NhaThau)
                                       .Include(x => x.YeuCauTraVatTuChiTiets).ThenInclude(x => x.VatTuBenhVien)
                                            .ThenInclude(x => x.VatTus)
                                            .Include(w => w.KhoXuat).Include(w => w.KhoNhap)
                                            .Include(w => w.NhanVienDuyet).ThenInclude(w => w.User)
                                            .Include(w => w.NhanVienYeuCau).ThenInclude(w => w.User));
            var result = entityResult.ToModel<YeuCauHoanTraKSNKViewModel>();
            foreach (var yeuCauTraVatTuChiTiet in entityResult.YeuCauTraVatTuChiTiets)
            {
                var chiTiet = result.YeuCauTraKSNKChiTiets.FirstOrDefault(p => p.Id == yeuCauTraVatTuChiTiet.Id + "");
                if (chiTiet != null)
                {
                    chiTiet.VatTu = yeuCauTraVatTuChiTiet.VatTuBenhVien.VatTus.Ten;
                    chiTiet.MaVatTu = yeuCauTraVatTuChiTiet.VatTuBenhVien.VatTus.Ma;
                    chiTiet.DVT = yeuCauTraVatTuChiTiet.VatTuBenhVien.VatTus.DonViTinh;
                    chiTiet.LaVatTuBHYT = yeuCauTraVatTuChiTiet.LaVatTuBHYT;
                    chiTiet.SoLo = yeuCauTraVatTuChiTiet.Solo;
                    chiTiet.SoLuongTon =
                        (yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri?.NhapKhoVatTuChiTiet.SoLuongNhap ?? 0) -
                        (yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri?.NhapKhoVatTuChiTiet.SoLuongDaXuat ?? 0);
                    chiTiet.HanSuDung = yeuCauTraVatTuChiTiet.HanSuDung;
                    chiTiet.NgayNhap = yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri?.NhapKhoVatTuChiTiet
                        .NgayNhap;

                    chiTiet.Nhom = yeuCauTraVatTuChiTiet.VatTuBenhVien.LoaiSuDung != null ? yeuCauTraVatTuChiTiet.VatTuBenhVien.LoaiSuDung.GetDescription() : string.Empty;
                    chiTiet.SoLuongXuat = yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri?.SoLuongXuat ?? 0;
                    chiTiet.TiLeThapGia = yeuCauTraVatTuChiTiet.TiLeTheoThapGia;
                    chiTiet.Vat = yeuCauTraVatTuChiTiet.VAT;
                    chiTiet.Id = yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri?.NhapKhoVatTuChiTietId + "," +
                                 yeuCauTraVatTuChiTiet.VatTuBenhVien.LoaiSuDung.GetDescription() + "," +
                                 (yeuCauTraVatTuChiTiet.LaVatTuBHYT ? "true" : "false");
                }
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.YeuCauHoanTraKSNK)]
        public async Task<ActionResult> Delete(long id)
        {
            await _ycHoanTraKSNKService.XoaYeuCauHoanTraVatTuAsync(id);
            return NoContent();
        }


        [HttpDelete("XoaHoanTraDuocPham/{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.YeuCauHoanTraKSNK)]
        public async Task<ActionResult> XoaHoanTraDuocPham(long id)
        {           
            await _ycHoanTraKSNKService.XoaYeuCauHoanTraDuocPhamAsync(id);
            return Ok();
        }

        [HttpGet("GetYeuCauHoanTraDuocPhamTuTruc")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauHoanTraKSNK)]
        public async Task<ActionResult<YeuCauHoanTraKSNKTuTrucViewModel>> GetYeuCauHoanTraDuocPhamTuTruc(long id)
        {
            var yeuCauhoanTra = await _ycHoanTraDuocPhamService
                .GetByIdAsync(id, s =>
                            s.Include(r => r.YeuCauTraDuocPhamChiTiets).ThenInclude(ct => ct.DuocPhamBenhVien).ThenInclude(dpct => dpct.DuocPham).ThenInclude(dp => dp.DonViTinh)
                             .Include(r => r.YeuCauTraDuocPhamChiTiets).ThenInclude(ct => ct.XuatKhoDuocPhamChiTietViTri).ThenInclude(dpct => dpct.XuatKhoDuocPhamChiTiet).ThenInclude(dp => dp.XuatKhoDuocPham)
                             .Include(r => r.KhoNhap)
                             .Include(r => r.KhoXuat)
                             .Include(r => r.NhanVienDuyet).ThenInclude(r => r.User)
                             .Include(r => r.NhanVienYeuCau).ThenInclude(r => r.User)
                             )
                             ;
            if (yeuCauhoanTra == null)
            {
                return NotFound();
            }

            var model = yeuCauhoanTra.ToModel<YeuCauHoanTraKSNKTuTrucViewModel>();
            model.YeuCauHoanTraKSNKChiTietHienThis = await _ycHoanTraKSNKService.YeuCauTraDuocPhamTuTrucChiTiets(id);

            return Ok(model);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataGridDuocPhamChild")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.YeuCauHoanTraKSNK)]
        public async Task<ActionResult<GridDataSource>> GetDataGridDuocPhamChild([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _ycHoanTraKSNKService.GetDataForGridDuocPhamChildAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageGridDuocPhamChild")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.YeuCauHoanTraKSNK)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageGridDuocPhamChild([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _ycHoanTraKSNKService.GetTotalPageForGridDuocPhamChildAsync(queryInfo);
            return Ok(gridData);
        }

    }
}
