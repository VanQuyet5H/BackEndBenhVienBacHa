using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Services.NhapKhoDuocPhams;
using Camino.Services.Localization;
using Microsoft.AspNetCore.Mvc;
using Camino.Core.Domain;
using Camino.Api.Auth;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Camino.Api.Models.NhapKhoDuocPhams;
using Camino.Api.Extensions;
using Camino.Api.Models.Error;
using System.Net;
using Camino.Api.Models.General;
using Camino.Services.HopDongThauDuocPhamService;
using Newtonsoft.Json;
using Camino.Api.Models.NhapKhoDuocPhamChiTiets;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.KhoDuocPhams;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Core.Domain.ValueObject.YeuCauNhapKhoDuocPham;
using Camino.Core.Domain.Entities.YeuCauNhapKhoDuocPhams;
using Camino.Services.CauHinh;
using Camino.Core.Helpers;
using Camino.Services.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject.Thuoc;

namespace Camino.Api.Controllers
{
    public class NhapKhoDuocPhamController : CaminoBaseController
    {
        private readonly INhapKhoDuocPhamService _nhapKhoDuocPhamService;
        private readonly ILocalizationService _localizationService;
        private readonly IHopDongThauDuocPhamService _hopDongThauDuocPhamService;
        private readonly IHopDongThauDuocPhamChiTietService _hopDongThauDuocPhamChiTietService;
        private readonly IExcelService _excelService;
        private readonly ICauHinhService _cauHinhService;
        private readonly IYeuCauNhapKhoDuocPhamService _yeuCauNhapKhoDuocPhamService;

        public NhapKhoDuocPhamController(
            INhapKhoDuocPhamService nhapKhoDuocPhamService,
            IHopDongThauDuocPhamService hopDongThauDuocPhamService,
            IHopDongThauDuocPhamChiTietService hopDongThauDuocPhamChiTietService,
            ILocalizationService localizationService,
            IExcelService excelService, ICauHinhService cauHinhService
            , IYeuCauNhapKhoDuocPhamService yeuCauNhapKhoDuocPhamService)
        {
            _hopDongThauDuocPhamService = hopDongThauDuocPhamService;
            _nhapKhoDuocPhamService = nhapKhoDuocPhamService;
            _hopDongThauDuocPhamChiTietService = hopDongThauDuocPhamChiTietService;
            _localizationService = localizationService;
            _excelService = excelService;
            _cauHinhService = cauHinhService;
            _yeuCauNhapKhoDuocPhamService = yeuCauNhapKhoDuocPhamService;
        }

        #region GetDataForGrid

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.NhapKhoDuocPham)]
        public ActionResult<GridDataSource> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = _nhapKhoDuocPhamService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.NhapKhoDuocPham)]
        public ActionResult<GridDataSource> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            //remove lazy load
            var gridData = _nhapKhoDuocPhamService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.NhapKhoDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridChildAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _nhapKhoDuocPhamService.GetDataForGridChildAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.NhapKhoDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridChildAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _nhapKhoDuocPhamService.GetTotalPageForGridChildAsync(queryInfo);
            return Ok(gridData);
        }

        #endregion

        #region CRUD


        //[HttpPost]
        [HttpPost("ThemYeuCauNhapKhoDuocPham")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.NhapKhoDuocPham)]
        public async Task<ActionResult<YeuCauNhapKhoDuocPhamViewModel>> ThemYeuCauNhapKhoDuocPham([FromBody] YeuCauNhapKhoDuocPhamViewModel viewModel)//YeuCauNhapKhoDuocPhamViewModelValidator
        {
            if (viewModel.NhapKhoDuocPhamChiTiets.Count <= 0)
                throw new ApiException(_localizationService.GetResource("NhapKhoDuocPhamChiTiet.Required"), (int)HttpStatusCode.BadRequest);
            var yeuCauNhapKhoDuocPhamChiTietGridVos = new List<YeuCauNhapKhoDuocPhamChiTietGridVo>();

            foreach (var nhapKhoDuocPhamChiTiet in viewModel.NhapKhoDuocPhamChiTiets)
            {
                if (nhapKhoDuocPhamChiTiet.LoaiNhap == 1) //Loại Hợp đồng thầu
                {
                    var kiemTraNgayHetHanHopDong = _nhapKhoDuocPhamService.KiemTraNgayHetHanHopDong(nhapKhoDuocPhamChiTiet.HopDongThauDuocPhamId);
                    if (!kiemTraNgayHetHanHopDong)
                    {
                        throw new ApiException($"Hợp đồng đã hết hạn sử dụng");

                    }

                    var hopDongThauDuocPham =
                    _nhapKhoDuocPhamService.SoLuongHopDongThauDuocPham(nhapKhoDuocPhamChiTiet.HopDongThauDuocPhamId,
                                                                     nhapKhoDuocPhamChiTiet.DuocPhamBenhVienId,
                                                                     nhapKhoDuocPhamChiTiet.YeuCauNhapKhoDuocPhamId,
                                                                     nhapKhoDuocPhamChiTiet.LaDuocPhamBHYT);
                    if (hopDongThauDuocPham != null && hopDongThauDuocPham.SoLuongChuaNhap < nhapKhoDuocPhamChiTiet.SoLuongNhap)
                    {
                        throw new ApiException($"{nhapKhoDuocPhamChiTiet.DuocPhamDisplay} số lượng nhập kho {nhapKhoDuocPhamChiTiet.SoLuongNhap} lớn hơn số lượng cần nhập là {hopDongThauDuocPham.SoLuongChuaNhap}");
                    }

                }


                yeuCauNhapKhoDuocPhamChiTietGridVos.Add(nhapKhoDuocPhamChiTiet.Map<YeuCauNhapKhoDuocPhamChiTietGridVo>());
            }
            var lst = await _nhapKhoDuocPhamService.YeuCauNhapKhoDuocPhamChiTiets(viewModel.Id, viewModel.KyHieuHoaDon, viewModel.SoChungTu, yeuCauNhapKhoDuocPhamChiTietGridVos);
            if (lst.Any())
            {
                return Ok(lst);
            }
            var entity = viewModel.ToEntity<YeuCauNhapKhoDuocPham>();

            //set kho tong
            entity.KhoId = await _yeuCauNhapKhoDuocPhamService.GetKhoTong1Id();

            foreach (var item in entity.YeuCauNhapKhoDuocPhamChiTiets)
            {
                item.NgayNhap = entity.NgayNhap;
                if (item.LaDuocPhamBHYT)
                {
                    item.TiLeTheoThapGia = 0;
                    item.VAT = 0;
                }
                else
                {
                    item.TiLeTheoThapGia = _cauHinhService.GetTiLeTheoThapGia(Enums.LoaiThapGia.ThuocKhongBaoHiem, item.DonGiaNhap, item.VAT, item.KhoNhapSauKhiDuyetId);

                    item.TiLeBHYTThanhToan = null;
                }
            }

            await _yeuCauNhapKhoDuocPhamService.AddAsync(entity);
            return Ok(entity.Id);
        }
        //[HttpPut]ThemYeuCauNhapKhoDuocPham
        [HttpPost("CapNhatYeuCauNhapKhoDuocPham")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.NhapKhoDuocPham)]
        public async Task<ActionResult<NhapKhoDuocPhamViewModel>> CapNhatYeuCauNhapKhoDuocPham([FromBody] YeuCauNhapKhoDuocPhamViewModel viewModel)
        {
            var entity = await _yeuCauNhapKhoDuocPhamService.GetByIdAsync(viewModel.Id, u => u.Include(x => x.YeuCauNhapKhoDuocPhamChiTiets)
                .ThenInclude(e => e.HopDongThauDuocPham).ThenInclude(w => w.HopDongThauDuocPhamChiTiets));
            if (viewModel.NhapKhoDuocPhamChiTiets.Count <= 0)
                throw new ApiException(_localizationService.GetResource("NhapKhoDuocPhamChiTiet.Required"), (int)HttpStatusCode.BadRequest);
            if (entity == null)
            {
                return NotFound();
            }

            //đã duyệt hoặc từ chối
            if (entity.DuocKeToanDuyet == true || (entity.DuocKeToanDuyet == false && (viewModel.DuocKeToanDuyet == null || viewModel.DuocKeToanDuyet == true)))
            {
                throw new ApiException(_localizationService.GetResource("NhapKho.StatusHasBeenChanged"), (int)HttpStatusCode.BadRequest);
            }

            var yeuCauNhapKhoDuocPhamChiTietGridVos = new List<YeuCauNhapKhoDuocPhamChiTietGridVo>();
            foreach (var nhapKhoDuocPhamChiTiet in viewModel.NhapKhoDuocPhamChiTiets)
            {
                if (nhapKhoDuocPhamChiTiet.LoaiNhap == 1) //Loại Hợp đồng thầu
                {
                    var kiemTraNgayHetHanHopDong = _nhapKhoDuocPhamService.KiemTraNgayHetHanHopDong(nhapKhoDuocPhamChiTiet.HopDongThauDuocPhamId);
                    if (!kiemTraNgayHetHanHopDong)
                    {
                        throw new ApiException($"Hợp đồng đã hết hạn sử dụng");

                    }

                    var hopDongThauDuocPham =
                    _nhapKhoDuocPhamService.SoLuongHopDongThauDuocPham(nhapKhoDuocPhamChiTiet.HopDongThauDuocPhamId,
                                                                     nhapKhoDuocPhamChiTiet.DuocPhamBenhVienId,
                                                                     nhapKhoDuocPhamChiTiet.YeuCauNhapKhoDuocPhamId,
                                                                     nhapKhoDuocPhamChiTiet.LaDuocPhamBHYT);
                    if (hopDongThauDuocPham != null && hopDongThauDuocPham.SoLuongChuaNhap < nhapKhoDuocPhamChiTiet.SoLuongNhap)
                    {
                        throw new ApiException($"{nhapKhoDuocPhamChiTiet.DuocPhamDisplay} số lượng nhập kho {nhapKhoDuocPhamChiTiet.SoLuongNhap} lớn hơn số lượng cần nhập là {hopDongThauDuocPham.SoLuongChuaNhap}");
                    }
                }              

                yeuCauNhapKhoDuocPhamChiTietGridVos.Add(nhapKhoDuocPhamChiTiet.Map<YeuCauNhapKhoDuocPhamChiTietGridVo>());
            }

            var lst = await _nhapKhoDuocPhamService.YeuCauNhapKhoDuocPhamChiTiets(viewModel.Id, viewModel.KyHieuHoaDon, viewModel.SoChungTu, yeuCauNhapKhoDuocPhamChiTietGridVos);
            if (lst.Any())
            {
                return Ok(lst);
            }

            viewModel.ToEntity(entity);

            foreach (var item in entity.YeuCauNhapKhoDuocPhamChiTiets.Where(e => e.WillDelete != true))
            {
                item.NgayNhap = entity.NgayNhap;
                if (item.LaDuocPhamBHYT)
                {
                    item.TiLeTheoThapGia = 0;
                    item.VAT = 0;
                }
                else
                {
                    item.TiLeTheoThapGia = _cauHinhService.GetTiLeTheoThapGia(Enums.LoaiThapGia.ThuocKhongBaoHiem, item.DonGiaNhap, item.VAT, item.KhoNhapSauKhiDuyetId);
                    item.TiLeBHYTThanhToan = null;
                }
            }

            await _yeuCauNhapKhoDuocPhamService.UpdateAsync(entity);

            return Ok(entity.Id);
        }
        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.NhapKhoDuocPham)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<YeuCauNhapKhoDuocPhamViewModel>> Get(long id)
        {
            var result = await _yeuCauNhapKhoDuocPhamService.GetByIdAsync(id
                ,
                u => u.Include(x => x.YeuCauNhapKhoDuocPhamChiTiets).ThenInclude(x => x.HopDongThauDuocPham).ThenInclude(x => x.NhaThau)
                .Include(x => x.YeuCauNhapKhoDuocPhamChiTiets).ThenInclude(x => x.DuocPhamBenhVien).ThenInclude(x => x.DuocPham)
                .Include(x => x.YeuCauNhapKhoDuocPhamChiTiets).ThenInclude(x => x.KhoViTri)
                .Include(x => x.YeuCauNhapKhoDuocPhamChiTiets).ThenInclude(x => x.DuocPhamBenhVienPhanNhom)
                );
            if (result == null)
                return NotFound();
            var resultData = result.ToModel<YeuCauNhapKhoDuocPhamViewModel>();
            return Ok(resultData);
        }

        [HttpPost("XoaYeuCauNhapKhoDuocPham")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.NhapKhoDuocPham)]
        public async Task<ActionResult> XoaYeuCauNhapKhoDuocPham(long id)
        {
            var nhapKho = await _yeuCauNhapKhoDuocPhamService.GetByIdAsync(id, x => x.Include(p => p.YeuCauNhapKhoDuocPhamChiTiets));
            if (nhapKho == null)
            {
                return NotFound();
            }
            await _yeuCauNhapKhoDuocPhamService.DeleteAsync(nhapKho);
            return NoContent();
        }

        [HttpPost("GetGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.NhapKhoDuocPham)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<YeuCauNhapKhoDuocPhamViewModel>> GetGrid(long id)
        {
            var result = await _yeuCauNhapKhoDuocPhamService.GetByIdAsync(id
                ,
                u => u.Include(x => x.YeuCauNhapKhoDuocPhamChiTiets).ThenInclude(x => x.HopDongThauDuocPham).ThenInclude(x => x.NhaThau)
                .Include(x => x.YeuCauNhapKhoDuocPhamChiTiets).ThenInclude(x => x.DuocPhamBenhVien).ThenInclude(x => x.DuocPham).ThenInclude(x => x.DonViTinh)
                .Include(x => x.YeuCauNhapKhoDuocPhamChiTiets).ThenInclude(x => x.DuocPhamBenhVien).ThenInclude(x => x.DuocPhamBenhVienPhanNhom)

                .Include(x => x.YeuCauNhapKhoDuocPhamChiTiets).ThenInclude(x => x.KhoViTri)
                .Include(x => x.YeuCauNhapKhoDuocPhamChiTiets).ThenInclude(x => x.DuocPhamBenhVienPhanNhom)
                .Include(x => x.YeuCauNhapKhoDuocPhamChiTiets).ThenInclude(x => x.DuocPhamBenhVien).ThenInclude(x => x.DuocPhamBenhVienPhanNhom)
                    .Include(x => x.YeuCauNhapKhoDuocPhamChiTiets).ThenInclude(x => x.KhoNhapSauKhiDuyet)
                    .Include(x => x.YeuCauNhapKhoDuocPhamChiTiets).ThenInclude(x => x.NguoiNhapSauKhiDuyet)
                );
            if (result == null)
                return NotFound();
            var resultData = result.ToModel<YeuCauNhapKhoDuocPhamViewModel>();

            foreach (var item in result.YeuCauNhapKhoDuocPhamChiTiets)
            {
                var modelChild = resultData.NhapKhoDuocPhamChiTiets.First(p => p.Id == item.Id);
                modelChild.NhaThauDisplay = item.HopDongThauDuocPham.NhaThau.Ten;
                modelChild.NhaThauId = item.HopDongThauDuocPham.NhaThauId;
                modelChild.HopDongThauDisplay = item.HopDongThauDuocPham.HeThongTuPhatSinh != true ? item.HopDongThauDuocPham.SoHopDong : string.Empty;
                modelChild.DuocPhamDisplay = item.DuocPhamBenhVien.DuocPham.Ten;
                modelChild.LoaiDisplay = item.LaDuocPhamBHYT ? "BHYT" : "Không BHYT";
                modelChild.Solo = item.Solo;
                modelChild.HanSuDungDisplay = item.HanSuDung.ApplyFormatDate();
                modelChild.MaVach = item.MaVach;
                modelChild.MaRef = item.MaRef;
                modelChild.SoLuongNhapDisplay = item.SoLuongNhap.ApplyNumber();
                //modelChild.ViTriDisplay = item.KhoViTri.Ten;
                modelChild.DVT = item.DuocPhamBenhVien.DuocPham.DonViTinh.Ten;
                modelChild.HamLuong = item.DuocPhamBenhVien.DuocPham.HamLuong;
                modelChild.NhomDisplay = item.DuocPhamBenhVienPhanNhom?.Ten ?? item.DuocPhamBenhVien?.DuocPhamBenhVienPhanNhom?.Ten ?? "CHƯA PHÂN NHÓM";
                modelChild.LoaiNhap = item.HopDongThauDuocPham.HeThongTuPhatSinh != true ? 1 : 2;
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

        [HttpPost("ThongTinChiTietValidation")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.NhapKhoDuocPham)]
        public ActionResult ThongTinChiTietValidation([FromBody] NhapKhoDuocPhamChiTietViewModel viewModel)
        {
            return NoContent();
        }

        #endregion

        [HttpPost("ChangePostion")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.NhapKhoDuocPham)]
        public async Task<ActionResult> ChangePostion(string stringId)
        {
            await _nhapKhoDuocPhamService.NhapKhoDuocPhamChiTietUpdateViTri(long.Parse(stringId.Split(";")[0]), long.Parse(stringId.Split(";")[1]));
            return NoContent();
        }

        [HttpPost("GetListNhaThauNhapKho")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListNhaThauNhapKho([FromBody]LookupQueryInfo queryInfo)
        {
            var lookup = await _nhapKhoDuocPhamService.GetListNhaThauNhapKho(queryInfo);
            return Ok(lookup);
        }

        [HttpPost("GetListDuocPhamNhapKho")]
        public ActionResult<ICollection<LookupItemVo>> GetListDuocPhamNhapKho([FromBody]DropDownListRequestModel model)
        {
            var nhapKhoInput = JsonConvert.DeserializeObject<NhapKhoDuocPhamVatTuTheoHopDongThau>(model.ParameterDependencies.Replace("undefined", "null"));
            var lookup = _nhapKhoDuocPhamService.GetDropDownListDuocPham(nhapKhoInput, model);
            return Ok(lookup);
        }

        [HttpPost("GetDropDownListDuocPhamFromNhaThau")]
        public ActionResult<ICollection<LookupItemVo>> GetDropDownListDuocPhamFromNhaThau([FromBody]DropDownListRequestModel model)
        {
            var lookup = _nhapKhoDuocPhamService.GetDropDownListDuocPhamFromNhaThau(model);
            return Ok(lookup);
        }

        [HttpPost("GetListViTriKhoDuocPhamTheoKho")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListViTriKhoDuocPhamTheoKho(LookupQueryInfo model)
        {
            long khoDuocPhamId = 0;
            var parameter = model != null ? model.ParameterDependencies : "";
            if (!string.IsNullOrEmpty(parameter) && !parameter.ToLower().Contains("undefined"))
            {
                var getValue = JsonConvert.DeserializeObject<Dictionary<string, long>>(parameter);
                khoDuocPhamId = getValue.Values.First();
            }

            var lookup = await _nhapKhoDuocPhamService.GetListViTriKhoDuocPhamTheoKho(khoDuocPhamId, model);
            return Ok(lookup);
        }

        #region kiểm tra thông tin

        [HttpGet("KiemTraThongTinXuatKhoTuNhapKhoHienTai")]
        public async Task<ActionResult<bool>> KiemTraThongTinXuatKhoTuNhapKhoHienTai(long nhapKhoId)
        {
            var checkXuatKho = await _nhapKhoDuocPhamService.KiemTraNhapKhoDaCoChiTietXuatKhoAsync(nhapKhoId);
            return checkXuatKho;
        }

        [HttpGet("KiemTraHieuLucHopDongThau")]
        public async Task<ActionResult<bool>> KiemTraHieuLucHopDongThau(long hopDongThauId)
        {
            var kiemTraHopDongThau = await _hopDongThauDuocPhamService.KiemTraHieuLucHopDongThau(hopDongThauId);
            if (!kiemTraHopDongThau)
            {
                throw new ApiException(_localizationService.GetResource("NhapKhoDuocPhamChiTiet.HopDongThau.OutOfDate"), (int)HttpStatusCode.BadRequest);
            }
            return kiemTraHopDongThau;
        }

        [HttpGet("GetPriceOnContract")]
        public async Task<ActionResult> GetPriceOnContract(long hopDongThauId, long duocPhamId)
        {
            var currentPrice = await _nhapKhoDuocPhamService.GetPriceOnContract(hopDongThauId, duocPhamId);
            return Ok(currentPrice);
        }
        #endregion

        #region export
        [HttpPost("ExportNhapKhoDuocPham")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.NhapKhoDuocPham)]
        public async Task<ActionResult> ExportNhapKhoDuocPham(QueryInfo queryInfo)
        {
            // todo: hardcode max row export excel
            queryInfo.Skip = 0;
            queryInfo.Take = 20000;

            var queryStringParent = !string.IsNullOrEmpty(queryInfo.AdditionalSearchString) ? JsonConvert.DeserializeObject<NhapKhoDuocPhamGripVo>(queryInfo.AdditionalSearchString) : new NhapKhoDuocPhamGripVo();

            var gridData = _nhapKhoDuocPhamService.GetDataForGridAsync(queryInfo);
            var data = gridData.Data.Select(p => (NhapKhoDuocPhamGripVo)p).ToList();
            var dataExcel = data.Map<List<NhapKhoDuocPhamExportExcel>>();

            queryInfo.Sort = new List<Sort>();
            queryInfo.Sort.Add(new Sort()
            {
                Field = "TenDuocPham",
                Dir = "asc"
            });

            foreach (var item in dataExcel)
            {
                //+ ";" + queryStringParent.ChuaXepViTri
                queryInfo.AdditionalSearchString = item.Id + "";
                var gridChildData = await _nhapKhoDuocPhamService.GetDataForGridChildAsync(queryInfo);
                var dataChild = gridChildData.Data.Select(p => (NhapKhoDuocPhamChiTietGripVo)p).ToList();
                var dataChildExcel = dataChild.Map<List<NhapKhoDuocPhamExportExcelChild>>();
                item.NhapKhoDuocPhamExportExcelChild.AddRange(dataChildExcel);
            }

            var lstValueObject = new List<(string, string)>
            {
                (nameof(NhapKhoDuocPhamExportExcel.SoPhieu), "Số Chứng Từ"),
                (nameof(NhapKhoDuocPhamExportExcel.SoChungTu), "Số Hóa Đơn"),
                (nameof(NhapKhoDuocPhamExportExcel.NgayHoaDonDisplay), "Ngày Hóa Đơn"),
                (nameof(NhapKhoDuocPhamExportExcel.TenKho), "Kho Nhập"),
                (nameof(NhapKhoDuocPhamExportExcel.TenNguoiNhap), "Người Nhập"),
                //(nameof(NhapKhoDuocPhamExportExcel.LoaiNguoiGiaoDisplay), "Loại Người Giao"),
                //(nameof(NhapKhoDuocPhamExportExcel.TenNguoiGiao), "Tên Người Giao"),
                (nameof(NhapKhoDuocPhamExportExcel.NgayNhapDisplay), "Ngày Nhập"),
                (nameof(NhapKhoDuocPhamExportExcel.TinhTrangDisplay), "Tình Trạng"),
                (nameof(NhapKhoDuocPhamExportExcel.NguoiDuyet), "Người Duyệt"),
                (nameof(NhapKhoDuocPhamExportExcel.NgayDuyetDisplay), "Ngày Duyệt"),
                (nameof(NhapKhoDuocPhamExportExcel.NhapKhoDuocPhamExportExcelChild), "")
            };


            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Nhập kho dược phẩm");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=NhapKhoDuocPham" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";


            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion

        #region vu le
        [HttpPost("GetListNhomThuoc")]
        public async Task<ActionResult<ICollection<NhomThuocTreeViewVo>>> GetListNhomThuoc([FromBody] DropDownListRequestModel model)
        {
            var lookup = await _nhapKhoDuocPhamService.GetListNhomThuocAsync(model);
            return Ok(lookup);
        }

        [HttpPost("GetListViTriKhoTong1")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListViTriKhoTong1(LookupQueryInfo model)
        {
            var lookup = await _nhapKhoDuocPhamService.GetListViTriKhoTong1(model);
            return Ok(lookup);
        }

        [HttpPost("ThemDuocPhamNhapKho")]
        public async Task<ActionResult> ThemDuocPhamNhapKho([FromBody] YeuCauNhapKhoDuocPhamChiTietGridVo model) //AddOrUpdateThongTinDuocPhamNhapKhoValidator
        {
            var result = await _nhapKhoDuocPhamService.GetDuocPhamGrid(model);
            return Ok(result);
        }
        [HttpPost("GetKhoLoaiVatTus")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetKhoLoaiVatTus(DropDownListRequestModel model)
        {
            var lookup = await _nhapKhoDuocPhamService.GetKhoLoaiVatTus(model);
            return Ok(lookup);
        }

        [HttpPost("GetKhoTheoLoaiDuocPham")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetKhoTheoLoaiDuocPham(DropDownListRequestModel model)
        {
            var lookup = await _nhapKhoDuocPhamService.GetKhoTheoLoaiDuocPham(model);
            return Ok(lookup);
        }
        [HttpPost("GoiDuyetLai")]
        public async Task<ActionResult> GoiDuyetLai(long id)
        {
            var entity = await _yeuCauNhapKhoDuocPhamService.GetByIdAsync(id);
            entity.DuocKeToanDuyet = null;
            entity.NhanVienDuyetId = null;
            entity.NgayDuyet = null;
            await _yeuCauNhapKhoDuocPhamService.UpdateAsync(entity);
            return Ok(entity);
        }

        [HttpPost("SuggestNhomDuocPham")]
        public async Task<ActionResult> SuggestNhomDuocPham(long id)
        {
            var result = await _yeuCauNhapKhoDuocPhamService.SuggestNhomDuocPham(id);
            return Ok(result);
        }
        #endregion vu le

        [HttpPost("InPhieuYeuCauNhapKhoDuocPham")]
        public string InPhieuYeuCauNhapKhoDuocPham(long yeuCauNhapKhoDuocPhamId)
        {
            var result = _nhapKhoDuocPhamService.InPhieuYeuCauNhapKhoDuocPham(yeuCauNhapKhoDuocPhamId);
            return result;
        }
    }
}