using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.Error;

//using Camino.Api.Models.NhapKhoVatTus;
using Camino.Api.Models.NhapKhoKSNKs;

using Camino.Core.Domain;
using Camino.Core.Domain.Entities.YeuCauNhapKhoVatTus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhoKSNKs;
using Camino.Core.Domain.ValueObject.Thuoc;

//using Camino.Core.Domain.ValueObject.YeuCauNhapKhoVatTu;
using Camino.Core.Domain.ValueObject.YeuCauNhapKhoKSNK;

using Camino.Core.Helpers;
using Camino.Services.CauHinh;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Camino.Services.NhapKhoDuocPhams;
using Camino.Services.NhapKhoVatTuNhomKSNK;
using Camino.Services.NhapKhoVatTus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Camino.Api.Controllers
{
    public class NhapKhoVatTuKSNKController : CaminoBaseController
    {
        private readonly INhapKhoVatTuNhomKSNKService _nhapKhoVatTuNhomKSNKService;
        private readonly IExcelService _excelService;
        private readonly ILocalizationService _localizationService;
        private readonly ICauHinhService _cauHinhService;
        private readonly IYeuCauNhapKhoVatTuService _yeuCauNhapKhoVatTuService;

        public NhapKhoVatTuKSNKController(INhapKhoVatTuNhomKSNKService nhapKhoVatTuNhomKSNKService
            , IYeuCauNhapKhoDuocPhamService yeuCauNhapKhoDuocPhamService, ICauHinhService cauHinhService
            , IExcelService excelService, ILocalizationService localizationService,
            IYeuCauNhapKhoVatTuService yeuCauNhapKhoVatTuService)
        {
            _nhapKhoVatTuNhomKSNKService = nhapKhoVatTuNhomKSNKService;
            _excelService = excelService;
            _localizationService = localizationService;
            _cauHinhService = cauHinhService;
            _yeuCauNhapKhoVatTuService = yeuCauNhapKhoVatTuService;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.NhapVatTuThuocNhomKSNK)]
        public ActionResult<GridDataSource> GetDataForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = _nhapKhoVatTuNhomKSNKService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.NhapVatTuThuocNhomKSNK)]
        public ActionResult<GridDataSource> GetTotalPageForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = _nhapKhoVatTuNhomKSNKService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.NhapVatTuThuocNhomKSNK)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridChildAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _nhapKhoVatTuNhomKSNKService.GetDataForGridChildAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.NhapVatTuThuocNhomKSNK)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridChildAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _nhapKhoVatTuNhomKSNKService.GetTotalPageForGridChildAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("ThemVatTuNhapKho")]
        public async Task<ActionResult> ThemVatTuNhapKho([FromBody] YeuCauNhapKhoKSNKChiTietGridVo model)
        {
            var result = await _nhapKhoVatTuNhomKSNKService.GetVatTuGrid(model);
            return Ok(result);
        }

        [HttpPost("GetListLoaiSuDung")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListLoaiSuDung([FromBody] LookupQueryInfo queryInfo)
        {
            var lookup = await _nhapKhoVatTuNhomKSNKService.GetListLoaiSuDung(queryInfo);
            return Ok(lookup);
        }

        [HttpPost("SuggestLoaiSuDung")]
        public async Task<ActionResult> SuggestLoaiSuDung(long id)
        {
            var result = await _nhapKhoVatTuNhomKSNKService.SuggestLoaiSuDung(id);
            return Ok(result);
        }

        [HttpPost("GetListNhaThauNhapKho")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListNhaThauNhapKho([FromBody] LookupQueryInfo queryInfo)
        {
            var lookup = await _nhapKhoVatTuNhomKSNKService.GetListNhaThauNhapKho(queryInfo);
            return Ok(lookup);
        }

        [HttpPost("GetListVatTuNhapKho")]
        public ActionResult<ICollection<LookupItemVo>> GetListVatTuNhapKho([FromBody] DropDownListRequestModel model)
        {
            var nhapKhoInput = JsonConvert.DeserializeObject<NhapKhoDuocPhamVatTuTheoHopDongThau>(model.ParameterDependencies.Replace("undefined", "null"));
            var lookup = _nhapKhoVatTuNhomKSNKService.GetDropDownListVatTu(nhapKhoInput, model);
            return Ok(lookup);

        }

        [HttpPost("GetDropDownListVatTuFromNhaThau")]
        public ActionResult<ICollection<LookupItemVo>> GetDropDownListVatTuFromNhaThau([FromBody]DropDownListRequestModel model)
        {
            var lookup = _nhapKhoVatTuNhomKSNKService.GetDropDownListVatTuFromNhaThau(model);
            return Ok(lookup);
        }

        [HttpPost("ThemYeuCauNhapKhoVatTu")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.NhapVatTuThuocNhomKSNK)]
        public async Task<ActionResult<YeuCauNhapKhoKSNKViewModel>> ThemYeuCauNhapKhoVatTu([FromBody] YeuCauNhapKhoKSNKViewModel viewModel)
        {
            if (viewModel.NhapKhoVatTuChiTiets.Count <= 0)
                throw new ApiException(_localizationService.GetResource("NhapKhoVatTuChiTiet.Required"), (int)HttpStatusCode.BadRequest);

            var yeuCauNhapKhoVatTuChiTietGridVos = new List<YeuCauNhapKhoKSNKChiTietGridVo>();
            foreach (var nhapKhoVatTuChiTiet in viewModel.NhapKhoVatTuChiTiets)
            {

                if (nhapKhoVatTuChiTiet.LoaiNhap == 1) //Loại Hợp đồng thầu
                {
                    var kiemTraNgayHetHanHopDong = _nhapKhoVatTuNhomKSNKService.KiemTraNgayHetHanHopDong(nhapKhoVatTuChiTiet.HopDongThauVatTuId);
                    if (!kiemTraNgayHetHanHopDong)
                    {
                        throw new ApiException($"Hợp đồng đã hết hạn sử dụng");

                    }

                    var hopDongThauVatTu =
                    _nhapKhoVatTuNhomKSNKService.SoLuongHopDongThauVatTu(nhapKhoVatTuChiTiet.HopDongThauVatTuId,
                                                                     nhapKhoVatTuChiTiet.VatTuBenhVienId,
                                                                     nhapKhoVatTuChiTiet.YeuCauNhapKhoVatTuId,
                                                                     nhapKhoVatTuChiTiet.LaVatTuBHYT);
                    if (hopDongThauVatTu != null && hopDongThauVatTu.SoLuongChuaNhap < nhapKhoVatTuChiTiet.SoLuongNhap)
                    {
                        throw new ApiException($"{nhapKhoVatTuChiTiet.VatTuDisplay} số lượng nhập kho {nhapKhoVatTuChiTiet.SoLuongNhap} lớn hơn số lượng cần nhập là {hopDongThauVatTu.SoLuongChuaNhap}");
                    }

                }

                yeuCauNhapKhoVatTuChiTietGridVos.Add(nhapKhoVatTuChiTiet.Map<YeuCauNhapKhoKSNKChiTietGridVo>());
            }
            var lst = await _nhapKhoVatTuNhomKSNKService.YeuCauNhapKhoVatTuChiTiets(viewModel.Id, viewModel.KyHieuHoaDon, viewModel.SoChungTu, yeuCauNhapKhoVatTuChiTietGridVos);
            if (lst.Any())
            {
                return Ok(lst);
            }
            var entity = viewModel.ToEntity<YeuCauNhapKhoVatTu>();

            //set kho tong
            entity.KhoId = await _nhapKhoVatTuNhomKSNKService.GetKhoHanhChinh();

            foreach (var item in entity.YeuCauNhapKhoVatTuChiTiets)
            {
                item.NgayNhap = entity.NgayNhap;
                if (item.LaVatTuBHYT)
                {
                    item.TiLeTheoThapGia = 0;
                    item.VAT = 0;
                }
                else
                {
                    var loaiSuDung = await _nhapKhoVatTuNhomKSNKService.SuggestLoaiSuDung(item.VatTuBenhVienId);
                    if (loaiSuDung == null)
                    {
                        item.TiLeTheoThapGia = 0;
                    }
                    else if (loaiSuDung == Enums.LoaiSuDung.VatTuThayThe)
                    {
                        item.TiLeTheoThapGia = _cauHinhService.GetTiLeTheoThapGia(Enums.LoaiThapGia.VatTuThayThe, item.DonGiaNhap);
                    }
                    else
                    {
                        item.TiLeTheoThapGia = _cauHinhService.GetTiLeTheoThapGia(Enums.LoaiThapGia.VatTuTieuHao, item.DonGiaNhap);
                    }

                    item.TiLeBHYTThanhToan = null;
                }
            }

            await _yeuCauNhapKhoVatTuService.AddAsync(entity);
            return Ok(entity.Id);
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.NhapVatTuThuocNhomKSNK)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<YeuCauNhapKhoKSNKViewModel>> Get(long id)
        {
            var result = await _yeuCauNhapKhoVatTuService.GetByIdAsync(id
                ,
                u => u.Include(x => x.YeuCauNhapKhoVatTuChiTiets).ThenInclude(x => x.HopDongThauVatTu).ThenInclude(x => x.NhaThau)
                .Include(x => x.YeuCauNhapKhoVatTuChiTiets).ThenInclude(x => x.VatTuBenhVien).ThenInclude(x => x.VatTus)
                .Include(x => x.YeuCauNhapKhoVatTuChiTiets).ThenInclude(x => x.KhoViTri)
                );
            if (result == null)
                return NotFound();
            var resultData = result.ToModel<YeuCauNhapKhoKSNKViewModel>();

            return Ok(resultData);
        }

        [HttpGet("GetKhoVatTuYTe")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.NhapVatTuThuocNhomKSNK)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<LookupItemVo> GetKhoVatTuYTe(long id)
        {
            var khoVatTuYte = _nhapKhoVatTuNhomKSNKService.KhoKSNK();
            return Ok(khoVatTuYte);
        }

        [HttpPost("GetGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.NhapVatTuThuocNhomKSNK)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<YeuCauNhapKhoKSNKViewModel>> GetGrid(long id)
        {
            var result = await _yeuCauNhapKhoVatTuService.GetByIdAsync(id,
               u => u.Include(x => x.YeuCauNhapKhoVatTuChiTiets).ThenInclude(x => x.HopDongThauVatTu).ThenInclude(x => x.NhaThau)
               .Include(x => x.YeuCauNhapKhoVatTuChiTiets).ThenInclude(x => x.VatTuBenhVien).ThenInclude(x => x.VatTus)
               .Include(x => x.YeuCauNhapKhoVatTuChiTiets).ThenInclude(x => x.KhoViTri)
                   .Include(x => x.YeuCauNhapKhoVatTuChiTiets).ThenInclude(x => x.KhoNhapSauKhiDuyet)
                   .Include(x => x.YeuCauNhapKhoVatTuChiTiets).ThenInclude(x => x.NguoiNhapSauKhiDuyet)
               );
            if (result == null)
                return NotFound();
            var resultData = result.ToModel<YeuCauNhapKhoKSNKViewModel>();

            foreach (var item in result.YeuCauNhapKhoVatTuChiTiets)
            {
                var modelChild = resultData.NhapKhoVatTuChiTiets.First(p => p.Id == item.Id);
                modelChild.NhaThauDisplay = item.HopDongThauVatTu.NhaThau.Ten;
                modelChild.NhaThauId = item.HopDongThauVatTu.NhaThauId;
                modelChild.HopDongThauDisplay = item.HopDongThauVatTu.HeThongTuPhatSinh != true ? item.HopDongThauVatTu.SoHopDong : string.Empty;
                modelChild.VatTuDisplay = item.VatTuBenhVien.VatTus.Ten;
                modelChild.LoaiDisplay = item.LaVatTuBHYT ? "BHYT" : "Không BHYT";
                modelChild.Solo = item.Solo;
                modelChild.HanSuDungDisplay = item.HanSuDung.ApplyFormatDate();
                modelChild.MaVach = item.MaVach;
                modelChild.MaRef = item.MaRef;
                modelChild.SoLuongNhapDisplay = item.SoLuongNhap.ApplyNumber();
                //modelChild.ViTriDisplay = item.KhoViTri.Ten;
                //modelChild.NhomDisplay = item.DuocPhamBenhVienPhanNhom?.Ten;
                modelChild.LoaiSuDung = await _nhapKhoVatTuNhomKSNKService.SuggestLoaiSuDung(item.VatTuBenhVienId);
                modelChild.LoaiSuDungDisplay = modelChild.LoaiSuDung.GetDescription();
                modelChild.DVT = item.VatTuBenhVien.VatTus.DonViTinh;
                modelChild.LoaiNhap = item.HopDongThauVatTu.HeThongTuPhatSinh != true ? 1 : 2;
                modelChild.KhoNhapSauKhiDuyetId = item.KhoNhapSauKhiDuyetId;
                modelChild.NguoiNhapSauKhiDuyetId = item.NguoiNhapSauKhiDuyetId;
                modelChild.TenKhoNhapSauKhiDuyet = item.KhoNhapSauKhiDuyet?.Ten;
                modelChild.TenNguoiNhapSauKhiDuyet = item.NguoiNhapSauKhiDuyet?.HoTen;
                modelChild.ThanhTienTruocVat = item.ThanhTienTruocVat;
                modelChild.ThanhTienSauVat = item.ThanhTienSauVat;
                modelChild.GhiChu = item.GhiChu;
            }

            return Ok(resultData);
        }

        [HttpPost("CapNhatYeuCauNhapKhoVatTu")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.NhapVatTuThuocNhomKSNK)]
        public async Task<ActionResult> CapNhatYeuCauNhapKhoVatTu([FromBody] YeuCauNhapKhoKSNKViewModel viewModel)
        {
            var entity = await _yeuCauNhapKhoVatTuService.GetByIdAsync(viewModel.Id, u => u.Include(x => x.YeuCauNhapKhoVatTuChiTiets)
                .ThenInclude(e => e.HopDongThauVatTu).ThenInclude(w => w.HopDongThauVatTuChiTiets));
            if (viewModel.NhapKhoVatTuChiTiets.Count <= 0)
                throw new ApiException(_localizationService.GetResource("NhapKhoVatTuChiTiet.Required"), (int)HttpStatusCode.BadRequest);
            if (entity == null)
            {
                return NotFound();
            }

            //đã duyệt hoặc từ chối
            if (entity.DuocKeToanDuyet == true || (entity.DuocKeToanDuyet == false && (viewModel.DuocKeToanDuyet == null || viewModel.DuocKeToanDuyet == true)))
            {
                throw new ApiException(_localizationService.GetResource("NhapKho.StatusHasBeenChanged"), (int)HttpStatusCode.BadRequest);
            }
            var yeuCauNhapKhoVatTuChiTietGridVos = new List<YeuCauNhapKhoKSNKChiTietGridVo>();
            foreach (var nhapKhoVatTuChiTiet in viewModel.NhapKhoVatTuChiTiets)
            {

                if (nhapKhoVatTuChiTiet.LoaiNhap == 1) //Loại Hợp đồng thầu
                {
                    var kiemTraNgayHetHanHopDong = _nhapKhoVatTuNhomKSNKService.KiemTraNgayHetHanHopDong(nhapKhoVatTuChiTiet.HopDongThauVatTuId);
                    if (!kiemTraNgayHetHanHopDong)
                    {
                        throw new ApiException($"Hợp đồng đã hết hạn sử dụng");

                    }

                    var hopDongThauVatTu =
                    _nhapKhoVatTuNhomKSNKService.SoLuongHopDongThauVatTu(nhapKhoVatTuChiTiet.HopDongThauVatTuId,
                                                                     nhapKhoVatTuChiTiet.VatTuBenhVienId,
                                                                     nhapKhoVatTuChiTiet.YeuCauNhapKhoVatTuId,
                                                                     nhapKhoVatTuChiTiet.LaVatTuBHYT);

                    if (hopDongThauVatTu != null && hopDongThauVatTu.SoLuongChuaNhap < nhapKhoVatTuChiTiet.SoLuongNhap)
                    {
                        throw new ApiException($"{nhapKhoVatTuChiTiet.VatTuDisplay} số lượng nhập kho {nhapKhoVatTuChiTiet.SoLuongNhap} lớn hơn số lượng cần nhập là {hopDongThauVatTu.SoLuongChuaNhap}");
                    }
                }

                yeuCauNhapKhoVatTuChiTietGridVos.Add(nhapKhoVatTuChiTiet.Map<YeuCauNhapKhoKSNKChiTietGridVo>());
            }
            var lst = await _nhapKhoVatTuNhomKSNKService.YeuCauNhapKhoVatTuChiTiets(viewModel.Id, viewModel.KyHieuHoaDon, viewModel.SoChungTu, yeuCauNhapKhoVatTuChiTietGridVos);
            if (lst.Any())
            {
                return Ok(lst);
            }

            viewModel.ToEntity(entity);

            foreach (var item in entity.YeuCauNhapKhoVatTuChiTiets.Where(e => e.WillDelete != true))
            {
                item.NgayNhap = entity.NgayNhap;
                if (item.LaVatTuBHYT)
                {
                    item.TiLeTheoThapGia = 0;
                    item.VAT = 0;
                }
                else
                {
                    var loaiSuDung = await _nhapKhoVatTuNhomKSNKService.SuggestLoaiSuDung(item.VatTuBenhVienId);
                    if (loaiSuDung == null)
                    {
                        item.TiLeTheoThapGia = 0;
                    }
                    else if (loaiSuDung == Enums.LoaiSuDung.VatTuThayThe)
                    {
                        item.TiLeTheoThapGia = _cauHinhService.GetTiLeTheoThapGia(Enums.LoaiThapGia.VatTuThayThe, item.DonGiaNhap);
                    }
                    else
                    {
                        item.TiLeTheoThapGia = _cauHinhService.GetTiLeTheoThapGia(Enums.LoaiThapGia.VatTuTieuHao, item.DonGiaNhap);
                    }

                    item.TiLeBHYTThanhToan = null;
                }
            }

            await _yeuCauNhapKhoVatTuService.UpdateAsync(entity);

            return Ok(entity.Id);
        }

        [HttpPost("XoaYeuCauNhapKhoVatTu")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.NhapVatTuThuocNhomKSNK)]
        public async Task<ActionResult> XoaYeuCauNhapKhoVatTu(long id)
        {
            var nhapKho = await _yeuCauNhapKhoVatTuService.GetByIdAsync(id, x => x.Include(p => p.YeuCauNhapKhoVatTuChiTiets));
            if (nhapKho == null)
            {
                return NotFound();
            }
            await _yeuCauNhapKhoVatTuService.DeleteAsync(nhapKho);
            return NoContent();
        }

        [HttpPost("GoiDuyetLai")]
        public async Task<ActionResult> GoiDuyetLai(long id)
        {
            var entity = await _yeuCauNhapKhoVatTuService.GetByIdAsync(id);
            entity.DuocKeToanDuyet = null;
            entity.NhanVienDuyetId = null;
            entity.NgayDuyet = null;
            await _yeuCauNhapKhoVatTuService.UpdateAsync(entity);
            return Ok(entity);
        }

        [HttpGet("GetPriceOnContract")]
        public async Task<ActionResult> GetPriceOnContract(long hopDongThauId, long vatTuId)
        {
            var currentPrice = await _nhapKhoVatTuNhomKSNKService.GetPriceOnContract(hopDongThauId, vatTuId);
            return Ok(currentPrice);
        }

        #region export

        [HttpPost("ExportNhapKhoKSNK")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.NhapVatTuThuocNhomKSNK)]
        public async Task<ActionResult> ExportNhapKhoKSNK(QueryInfo queryInfo)
        {
            // todo: hardcode max row export excel
            queryInfo.Skip = 0;
            queryInfo.Take = 20000;

            var queryStringParent = !string.IsNullOrEmpty(queryInfo.AdditionalSearchString) ? JsonConvert.DeserializeObject<NhapKhoKSNKGridVo>(queryInfo.AdditionalSearchString) : new NhapKhoKSNKGridVo();

            var gridData = _nhapKhoVatTuNhomKSNKService.GetDataForGridAsync(queryInfo);
            var data = gridData.Data.Select(p => (NhapKhoKSNKGridVo)p).ToList();
            var dataExcel = data.Map<List<NhapKhoKSNKExportExcel>>();

            queryInfo.Sort = new List<Sort>();
            queryInfo.Sort.Add(new Sort()
            {
                Field = "TenVatTu",
                Dir = "asc"
            });

            foreach (var item in dataExcel)
            {
                //+ ";" + queryStringParent.ChuaXepViTri
                queryInfo.AdditionalSearchString = item.Id + "";
                var gridChildData = await _nhapKhoVatTuNhomKSNKService.GetDataForGridChildAsync(queryInfo);
                var dataChild = gridChildData.Data.Select(p => (NhapKhoKSNKChiTietGripVo)p).ToList();
                var dataChildExcel = dataChild.Map<List<NhapKhoKSNKExportExcelChild>>();
                item.NhapKhoKSNKExportExcelChild.AddRange(dataChildExcel);
            }

            var lstValueObject = new List<(string, string)>
            {
                (nameof(NhapKhoKSNKExportExcel.SoChungTu), "Số Hóa Đơn"),
                (nameof(NhapKhoKSNKExportExcel.NgayHoaDonDisplay), "Ngày Hóa Đơn"),
                (nameof(NhapKhoKSNKExportExcel.TenKho), "Kho Nhập"),
                (nameof(NhapKhoKSNKExportExcel.TenNguoiNhap), "Người Nhập"),
                (nameof(NhapKhoKSNKExportExcel.NgayNhapDisplay), "Ngày Nhập"),
                (nameof(NhapKhoKSNKExportExcel.TinhTrangDisplay), "Tình Trạng"),
                (nameof(NhapKhoKSNKExportExcel.NguoiDuyet), "Người Duyệt"),
                (nameof(NhapKhoKSNKExportExcel.NgayDuyetDisplay), "Ngày Duyệt"),
                (nameof(NhapKhoKSNKExportExcel.NhapKhoKSNKExportExcelChild), "")
            };


            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Nhập kho kiểm soát nhiễm khuẩn");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=NhapKhoKSNK" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";


            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        #endregion

        [HttpPost("InPhieuYeuCauNhapKhoVatTu")]
        public string InPhieuYeuCauNhapKhoVatTu(InPhieuNhapKhoKSNK inPhieuNhapKhoVatTu)
        {
            var result = _nhapKhoVatTuNhomKSNKService.InPhieuYeuCauNhapKhoVatTu(inPhieuNhapKhoVatTu);
            return result;
        }

    }
}
