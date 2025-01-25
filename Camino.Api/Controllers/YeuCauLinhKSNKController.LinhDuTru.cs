using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.Error;
using Camino.Api.Models.LinhThuongDuocPham;
using Camino.Api.Models.LinhThuongKSNK;
using Camino.Api.Models.LinhThuongVatTu;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.YeuCauLinhDuocPhams;
using Camino.Core.Domain.Entities.YeuCauLinhVatTus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.YeuCauKSNK;
using Camino.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public partial class YeuCauLinhKSNKController
    {
        [HttpPost("GetKhoLinhKSNK")]
        public async Task<ActionResult> GetKhoLinhKSNK([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _yeuCauLinhKSNKService.GetKhoLinh(queryInfo);
            return Ok(lookup);
        }

        [HttpPost("GetKhoCurrentUserKSNK")]
        public async Task<ActionResult> GetKhoCurrentUserKSNK([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _yeuCauLinhKSNKService.GetKhoCurrentUser(queryInfo);
            return Ok(lookup);
        }

        [HttpGet("GetCurrentUserKSNK")]
        public async Task<ActionResult> GetCurrentUserKSNK()
        {
            var lookup = await _yeuCauLinhKSNKService.GetCurrentUser();
            return Ok(lookup);
        }

        [HttpPost("GetKSNKTheoKho")]
        public async Task<ActionResult<ICollection<KSNKLookupVo>>> GetKSNKTheoKhoLookup(DropDownListRequestModel model)
        {
            var lookup = await _yeuCauLinhKSNKService.GetKSNK(model);
            return Ok(lookup);
        }

        [HttpPost("GetTrangThaiPhieuLinhKSNK")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TaoYeuCauLinhThuongKSNK)]
        public async Task<ActionResult<TrangThaiDuyetVos>> GetTrangThaiPhieuLinhKSNK(GridVoYeuCauLinh  model)
        {
            var result = await _yeuCauLinhKSNKService.GetTrangThaiPhieuLinh(model.YeuCauLinhId, model.LoaiDuocPhamHayVatTu);
            return Ok(result);
        }

        [HttpPost("ThemLinhThuongKSNKGridVo")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.TaoYeuCauLinhThuongKSNK)]
        public async Task<ActionResult> ThemLinhThuongKSNKGridVo(KSNKGridViewModel model)
        {
            var models = new LinhThuongKSNKGridVo
            {
                VatTuBenhVienId = model.VatTuBenhVienId, // hoặc DP
                Ten = model.Ten,
                Ma = model.Ma,
                DVT = model.DVT,
                NhaSX = model.NhaSX,
                NuocSX = model.NuocSX,
                SLYeuCau = model.SLYeuCau,
                LoaiKSNK = model.LoaiVatTu.Value,
                KhoXuatId = model.KhoXuatId,
                LaVatTuBHYT = model.LaVatTuBHYT,
                Nhom = model.LoaiVatTu == 1 ? "Không BHYT" : "BHYT",
                KhoLinhId = model.KhoXuatId,
                LoaiDuocPhamHayVatTu = model.LoaiDuocPhamHayVatTu,
                TenKhoLinh = model.TenKhoLinh
            };
            if (models.KhoLinhId != null)
            {
                models.TenKhoLinh = _yeuCauLinhKSNKService.GetNamKhoLinh(models.KhoLinhId);
            }
            var result = _yeuCauLinhKSNKService.LinhThuongKSNKGridVo(models);
            
            return Ok(result);
        }

        //GET
        [HttpPost("GetPhieuLinhThuongKSNK")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TaoYeuCauLinhThuongKSNK)]
        public async Task<ActionResult<LinhThuongKSNKViewModel>> GetPhieuLinhThuongKSNK(GridVoYeuCauLinh vo)
        {
            if(vo.LoaiDuocPhamHayVatTu == true)
            {
                var phieuLinhThuong = await _yeuCauLinhDuocPhamService
               .GetByIdAsync(vo.YeuCauLinhId, s =>
                           s.Include(r => r.YeuCauLinhDuocPhamChiTiets).ThenInclude(ct => ct.DuocPhamBenhVien).ThenInclude(dpct => dpct.DuocPham).ThenInclude(dp => dp.DuongDung)
                            .Include(r => r.YeuCauLinhDuocPhamChiTiets).ThenInclude(ct => ct.DuocPhamBenhVien).ThenInclude(dpct => dpct.DuocPham).ThenInclude(dp => dp.DonViTinh)
                            .Include(r => r.KhoNhap).Include(r => r.KhoXuat)
                            .Include(r => r.NhanVienYeuCau).ThenInclude(nv => nv.User)
                            .Include(r => r.NhanVienDuyet).ThenInclude(nv => nv.User));

                if (phieuLinhThuong == null)
                {
                    return NotFound();
                }
                var model = phieuLinhThuong.ToModel<LinhThuongDuocPhamViewModel>();
                foreach (var item in model.YeuCauLinhDuocPhamChiTiets)
                {

                    if (item.LaDuocPhamBHYT == true)
                    {
                        item.Nhom = "BHYT";
                    }
                    else
                    {
                        item.Nhom = "Không BHYT";
                    }
                    item.SLTon = _yeuCauLinhDuocPhamService.GetSoLuongTonDuocPhamGridVo(item.DuocPhamBenhVienId, item.KhoXuatId, item.LaDuocPhamBHYT);
                }
                if (phieuLinhThuong.NhanVienYeuCauId == _userAgentHelper.GetCurrentUserId())
                {
                    model.LaNguoiTaoPhieu = true;
                }
                else
                {
                    model.LaNguoiTaoPhieu = false;
                }
                model.YeuCauLinhDuocPhamChiTiets = model.YeuCauLinhDuocPhamChiTiets.OrderByDescending(z => z.LaDuocPhamBHYT).ThenBy(z => z.Ten).ToList();

                // yeu cau theo kho . duy nhat 1 kho 
                var tenNhomKho = string.Empty;
                if (model.KhoXuatId != null)
                {
                    tenNhomKho = _yeuCauLinhKSNKService.GetNamKhoLinh((long)model.KhoXuatId);
                }

                var yeuCaulinh = model.YeuCauLinhDuocPhamChiTiets
                                      .Select(d => new LinhThuongKNSKChiTietViewModel() {
                                          Nhom = d.Nhom,
                                          YeuCauLinhVatTuId = d.YeuCauLinhDuocPhamId,
                                          KhoXuatId = d.KhoXuatId,
                                          VatTuBenhVienId = d.DuocPhamBenhVienId,
                                          Ten = d.Ten,
                                          Ma = d.HoatChat , // to do
                                          LaVatTuBHYT = d.LaDuocPhamBHYT,
                                          DuocDuyet = d.DuocDuyet,
                                          SoLuong = d.SoLuong,
                                          SLYeuCau = d.SoLuong , // to do
                                          SoLuongCoTheXuat = d.SoLuongCoTheXuat,
                                          DVT = d.DVT,
                                          NhaSX = d.NhaSX,
                                          NuocSX = d.NuocSX,
                                          IsValidator = d.IsValidator,
                                          LoaiDuocPhamHayVatTu = true, // set  default
                                          TenKhoLinh = tenNhomKho,
                                          Id = d.Id
                                      }).ToList();

                var mapModelKSNK = new LinhThuongKSNKViewModel()
                {
                    KhoXuatId = model.KhoXuatId,
                    TenKhoXuat = model.TenKhoXuat,
                    KhoNhapId = model.KhoNhapId,
                    TenKhoNhap = model.TenKhoNhap,
                    LoaiPhieuLinh = model.LoaiPhieuLinh,
                    NhanVienYeuCauId = model.NhanVienYeuCauId,
                    HoTenNguoiYeuCau = model.HoTenNguoiYeuCau,
                    NgayYeuCau = model.NgayYeuCau,
                    GhiChu = model.GhiChu,
                    DuocDuyet = model.DuocDuyet,
                    LyDoKhongDuyet = model.LyDoKhongDuyet,
                    NhanVienDuyetId = model.NhanVienDuyetId,
                    HoTenNguoiDuyet = model.HoTenNguoiDuyet,
                    NgayDuyet = model.NgayDuyet,
                    IsLuu = model.IsLuu,
                    LastModified = model.LastModified,
                    LaNguoiTaoPhieu = model.LaNguoiTaoPhieu,
                    DaGui = model.DaGui,
                    YeuCauLinhVatTuChiTiets = yeuCaulinh,
                };
              
                return Ok(mapModelKSNK);
            }
            else
            {
                var phieuLinhThuong =
                await _yeuCauLinhKSNKService.GetByIdAsync(vo.YeuCauLinhId, s => s.Include(r => r.YeuCauLinhVatTuChiTiets).ThenInclude(ct => ct.VatTuBenhVien).ThenInclude(dpct => dpct.VatTus)
                                                                     .Include(r => r.KhoNhap).Include(r => r.KhoXuat)
                                                                     .Include(r => r.NhanVienYeuCau).ThenInclude(nv => nv.User)
                                                                     .Include(r => r.NhanVienDuyet).ThenInclude(nv => nv.User));

                if (phieuLinhThuong == null)
                {
                    return NotFound();
                }
                var model = phieuLinhThuong.ToModel<LinhThuongKSNKViewModel>();
                // yeu cau theo kho . duy nhat 1 kho 
                var tenNhomKho = string.Empty;
                if (model.KhoXuatId != null)
                {
                    tenNhomKho = _yeuCauLinhKSNKService.GetNamKhoLinh((long)model.KhoXuatId);
                }
                foreach (var item in model.YeuCauLinhVatTuChiTiets)
                {
                    item.TenKhoLinh = tenNhomKho;

                    if (item.LaVatTuBHYT == true)
                    {
                        item.Nhom = "BHYT";
                    }
                    else
                    {
                        item.Nhom = "Không BHYT";
                    }
                    item.SLTon = _yeuCauLinhKSNKService.GetSoLuongTonKSNKGridVo(item.VatTuBenhVienId.Value, item.KhoXuatId.Value, item.LaVatTuBHYT.Value);
                }
                if (phieuLinhThuong.NhanVienYeuCauId == _userAgentHelper.GetCurrentUserId())
                {
                    model.LaNguoiTaoPhieu = true;
                }
                else
                {
                    model.LaNguoiTaoPhieu = false;
                }
              

                model.YeuCauLinhVatTuChiTiets = model.YeuCauLinhVatTuChiTiets.OrderByDescending(z => z.LaVatTuBHYT).ThenBy(z => z.Ten).ToList();
                return Ok(model);
            }
        }


        [HttpPost("GuiPhieuLinhThuongKSNK")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.TaoYeuCauLinhThuongKSNK)]
        public async Task<ActionResult> GuiPhieuLinhThuongKSNK(LinhThuongKSNKViewModel linhThuongKSNKVM)
        {
            // ds yeuCauDuocTao 
            var yeuCauLinhDuocPhamVatTuIds = new List<GridVoYeuCauLinhDuocTao>();

            // ds theo kho xuất ( bao gồm cả dược phẩm và vật tư)
            if (linhThuongKSNKVM.YeuCauLinhVatTuChiTiets.Any())
            {
                var dsYeuCauLinhVatTuChiTietTheoKhos = linhThuongKSNKVM.YeuCauLinhVatTuChiTiets
                                                       .GroupBy(d => d.KhoXuatId).ToList();

                // tạo theo kho linh (dược phẩm và vật tư) : dp 1 phiếu , vt 1 phiếu
                if(dsYeuCauLinhVatTuChiTietTheoKhos.Any())
                {
                    foreach (var itemInFoKhoLinhs in dsYeuCauLinhVatTuChiTietTheoKhos)
                    {
                        if(itemInFoKhoLinhs.Any())
                        {
                            // tạo dược phẩm
                            if(itemInFoKhoLinhs.Where(d=>d.LoaiDuocPhamHayVatTu == true).Count() != 0)
                            {
                                var yeuCauLinhDuocPhamChiTiets = itemInFoKhoLinhs.Where(d => d.LoaiDuocPhamHayVatTu == true)
                                    .Select(d => new LinhThuongDuocPhamChiTietViewModel {
                                        Id = d.Id,
                                        Nhom = d.LaVatTuBHYT == true ? "BHYT" : "Không BHYT",
                                        YeuCauLinhDuocPhamId = d.YeuCauLinhVatTuId,
                                        DuocPhamBenhVienId = (long)d.VatTuBenhVienId,
                                        Ten = d.Ten,
                                        LaDuocPhamBHYT = (bool)d.LaVatTuBHYT,
                                        KhoXuatId =(long) itemInFoKhoLinhs.Select(f=>f.KhoXuatId).First(),
                                        DuocDuyet = d.DuocDuyet,
                                        SoLuong =d.SoLuong,
                                        SLYeuCau = d.SLYeuCau,
                                        SLTon = d.SLTon,
                                        SoLuongCoTheXuat = d.SoLuongCoTheXuat,
                                        //HamLuong =""
                                        //HoatChat =
                                        //DuongDungId = 
                                        DVT = d.DVT,
                                        NhaSX = d.NhaSX,
                                        NuocSX = d.NuocSX,
                                        IsValidator = d.IsValidator
                                    }).ToList();
                                
                                if (!itemInFoKhoLinhs.Select(d => d.LoaiDuocPhamHayVatTu == true).Any())
                                {
                                    throw new ApiException(_localizationService.GetResource("LinhThuongDuocPham.LinhDuocPhamChiTiets.Required"));
                                }
                                var checkKhoQuanLyDeleted = await _yeuCauLinhDuocPhamService.CheckKhoNhanVienQuanLy(linhThuongKSNKVM.KhoNhapId.Value);
                                if (checkKhoQuanLyDeleted == false)
                                {
                                    throw new ApiException(_localizationService.GetResource("LinhThuongDuocPham.KhoNhanVienQuanLy.NotExists"));
                                }

                                foreach (var item in yeuCauLinhDuocPhamChiTiets)
                                {
                                    if (item.Nhom == "BHYT")
                                    {
                                        item.LaDuocPhamBHYT = true;
                                    }
                                    else
                                    {
                                        item.LaDuocPhamBHYT = false;
                                    }
                                }
                                linhThuongKSNKVM.LoaiPhieuLinh = Enums.EnumLoaiPhieuLinh.LinhDuTru;
                                linhThuongKSNKVM.NgayYeuCau = DateTime.Now;
                                var linhThuongDuocPhamVM = new LinhThuongDuocPhamViewModel()
                                {
                                    KhoXuatId = itemInFoKhoLinhs.Select(d=>d.KhoXuatId).First(),
                                    TenKhoXuat = linhThuongKSNKVM.TenKhoXuat,
                                    KhoNhapId = linhThuongKSNKVM.KhoNhapId,
                                    TenKhoNhap = linhThuongKSNKVM.TenKhoNhap,
                                    LoaiPhieuLinh = linhThuongKSNKVM.LoaiPhieuLinh,
                                    NhanVienYeuCauId = linhThuongKSNKVM.NhanVienYeuCauId,
                                    NhanVienDuyetId = linhThuongKSNKVM.NhanVienDuyetId,
                                    NgayDuyet = linhThuongKSNKVM.NgayDuyet,
                                    DuocDuyet = linhThuongKSNKVM.DuocDuyet,
                                    HoTenNguoiYeuCau = linhThuongKSNKVM.HoTenNguoiYeuCau,
                                    HoTenNguoiDuyet = linhThuongKSNKVM.HoTenNguoiDuyet,
                                    NgayYeuCau = linhThuongKSNKVM.NgayYeuCau,
                                    GhiChu = linhThuongKSNKVM.GhiChu,
                                    LyDoKhongDuyet = linhThuongKSNKVM.LyDoKhongDuyet,
                                    IsLuu = linhThuongKSNKVM.IsLuu,
                                    LastModified = linhThuongKSNKVM.LastModified,
                                    LaNguoiTaoPhieu = linhThuongKSNKVM.LaNguoiTaoPhieu,
                                    DaGui = linhThuongKSNKVM.DaGui,
                                    YeuCauLinhDuocPhamChiTiets = yeuCauLinhDuocPhamChiTiets
                                };





                                var linhThuongDuocPham = linhThuongDuocPhamVM.ToEntity<YeuCauLinhDuocPham>();
                                linhThuongDuocPham.NoiYeuCauId = _userAgentHelper.GetCurrentNoiLLamViecId();
                                await _yeuCauLinhDuocPhamService.AddAsync(linhThuongDuocPham);

                                var newPhieu = new GridVoYeuCauLinhDuocTao
                                { 
                                    LoaiDuocPhamHayVatTu = true,
                                    YeuCauLinhVatTuId = linhThuongDuocPham.Id
                                };
                                yeuCauLinhDuocPhamVatTuIds.Add(newPhieu);
                            }
                            // tạo vật tư
                            if (itemInFoKhoLinhs.Where(d => d.LoaiDuocPhamHayVatTu == false).Count() != 0)
                            {
                                if (!itemInFoKhoLinhs.Where(d => d.LoaiDuocPhamHayVatTu == false).Any())
                                {
                                    throw new ApiException(_localizationService.GetResource("LinhThuongDuocPham.LinhDuocPhamChiTiets.Required"));
                                }
                                var yeuCauLinhChiTietVatTu = itemInFoKhoLinhs.Where(d => d.LoaiDuocPhamHayVatTu == false).ToList();
                                    //.Select(d => new LinhThuongKNSKChiTietViewModel
                                    //{
                                        
                                    //}).ToList();
                                foreach (var item in yeuCauLinhChiTietVatTu)
                                {
                                    if (item.Nhom == "BHYT")
                                    {
                                        item.LaVatTuBHYT = true;
                                    }
                                    else
                                    {
                                        item.LaVatTuBHYT = false;
                                    }
                                }
                                linhThuongKSNKVM.YeuCauLinhVatTuChiTiets = yeuCauLinhChiTietVatTu;
                                linhThuongKSNKVM.LoaiPhieuLinh = Enums.EnumLoaiPhieuLinh.LinhDuTru;
                                linhThuongKSNKVM.NgayYeuCau = DateTime.Now;
                                linhThuongKSNKVM.KhoXuatId = linhThuongKSNKVM.YeuCauLinhVatTuChiTiets.First().KhoXuatId;
                                var linhThuongKSNK = linhThuongKSNKVM.ToEntity<YeuCauLinhVatTu>();
                                linhThuongKSNK.NoiYeuCauId = _userAgentHelper.GetCurrentNoiLLamViecId();
                                await _yeuCauLinhKSNKService.AddAsync(linhThuongKSNK);

                                var newPhieu = new GridVoYeuCauLinhDuocTao
                                {
                                    LoaiDuocPhamHayVatTu = false,
                                    YeuCauLinhVatTuId = linhThuongKSNK.Id
                                };
                                yeuCauLinhDuocPhamVatTuIds.Add(newPhieu);
                            }
                        }
                    }
                }
            }


            
           if(linhThuongKSNKVM.DaGui == true) // true l gui
           {
                return Ok(yeuCauLinhDuocPhamVatTuIds);
           }
           else
           {
                return Ok(yeuCauLinhDuocPhamVatTuIds.Skip(0).Take(1).ToList());
            }

            
        }

        [HttpPost("GuiLaiPhieuLinhThuongKSNK")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.TaoYeuCauLinhThuongKSNK)]
        public async Task<ActionResult> GuiLaiPhieuLinhThuongKSNK(LinhThuongKSNKViewModel linhThuongKSNKVM)
        {
            // ds yeuCauDuocTao 
            var yeuCauLinhDuocPhamVatTuIds = new List<GridVoYeuCauLinhDuocTao>();

            // ds theo kho xuất ( bao gồm cả dược phẩm và vật tư)
            if (linhThuongKSNKVM.YeuCauLinhVatTuChiTiets.Any())
            {
                var dsYeuCauLinhVatTuChiTietTheoKhos = linhThuongKSNKVM.YeuCauLinhVatTuChiTiets
                                                       .GroupBy(d => d.KhoXuatId).ToList();

                // tạo theo kho linh (dược phẩm và vật tư) : dp 1 phiếu , vt 1 phiếu
                if (dsYeuCauLinhVatTuChiTietTheoKhos.Any())
                {
                    foreach (var itemInFoKhoLinhs in dsYeuCauLinhVatTuChiTietTheoKhos)
                    {
                        if (itemInFoKhoLinhs.Any())
                        {
                            // DP
                            if (itemInFoKhoLinhs.Where(d => d.LoaiDuocPhamHayVatTu == true).Count() != 0)
                            {
                                var dpChiTiet = itemInFoKhoLinhs.Where(d => d.LoaiDuocPhamHayVatTu == true).ToList();

                                // là duoc phẩm cùng kho nên cùng 1 yêu cầu 
                                // yêu cầu lĩnh = 0 , yêu cầu lĩnh chi tiết = 0 => là chưa tạo
                                if(linhThuongKSNKVM.Id == 0 && dpChiTiet.Any(d=>d.YeuCauLinhVatTuId == 0))
                                {
                                    linhThuongKSNKVM.YeuCauLinhVatTuChiTiets = dpChiTiet.Where(d => d.YeuCauLinhVatTuId == 0).ToList();

                                    var result = await AddKSNK(linhThuongKSNKVM);
                                    yeuCauLinhDuocPhamVatTuIds.AddRange(result);
                                }

                                // điều kiện yêu cầu lĩnh chi tiết đã có yêu cầu lĩnh mà id  = 0
                                if(linhThuongKSNKVM.Id == 0 &&  dpChiTiet.Any(d => d.YeuCauLinhVatTuId != 0))
                                {
                                    linhThuongKSNKVM.Id = dpChiTiet.Where(d => d.YeuCauLinhVatTuId != null).Select(d => (long)d.YeuCauLinhVatTuId).FirstOrDefault();
                                    
                                    await _yeuCauLinhDuocPhamService.CheckPhieuLinhDaDuyetHoacDaHuy(linhThuongKSNKVM.Id);


                                    if (!itemInFoKhoLinhs.Where(d => d.LoaiDuocPhamHayVatTu == true).Any())
                                    {
                                        throw new ApiException(_localizationService.GetResource("LinhThuongDuocPham.LinhDuocPhamChiTiets.Required"));
                                    }
                                    var checkKhoQuanLyDeleted = await _yeuCauLinhDuocPhamService.CheckKhoNhanVienQuanLy(linhThuongKSNKVM.KhoNhapId.Value);
                                    if (checkKhoQuanLyDeleted == false)
                                    {
                                        throw new ApiException(_localizationService.GetResource("LinhThuongDuocPham.KhoNhanVienQuanLy.NotExists"));
                                    }
                                    
                                    if (linhThuongKSNKVM.DuocDuyet == false && !linhThuongKSNKVM.IsLuu) // Từ chối duyệt
                                    {
                                        linhThuongKSNKVM.DuocDuyet = null;
                                        linhThuongKSNKVM.NhanVienDuyetId = null;
                                        linhThuongKSNKVM.NgayDuyet = null;
                                    }
                                    var linhThuongDuocPham = await _yeuCauLinhDuocPhamService.GetByIdAsync(linhThuongKSNKVM.Id, s => s.Include(r => r.YeuCauLinhDuocPhamChiTiets));
                                    
                                    if (linhThuongDuocPham == null)
                                    {
                                        return NotFound();
                                    }

                                    var yeuCauLinhDuocPhamChiTiets = itemInFoKhoLinhs.Where(d => d.LoaiDuocPhamHayVatTu == true)
                                        .Select(d => new LinhThuongDuocPhamChiTietViewModel
                                        {
                                            Id = d.Id,
                                            Nhom = d.LaVatTuBHYT == true ? "BHYT" : "Không BHYT",
                                            YeuCauLinhDuocPhamId = d.YeuCauLinhVatTuId,
                                            DuocPhamBenhVienId = (long)d.VatTuBenhVienId,
                                            Ten = d.Ten,
                                            LaDuocPhamBHYT = (bool)d.LaVatTuBHYT,
                                            KhoXuatId = (long)itemInFoKhoLinhs.Select(f => f.KhoXuatId).First(),
                                            DuocDuyet = d.DuocDuyet,
                                            SoLuong = d.SoLuong,
                                            SLYeuCau = d.SLYeuCau,
                                            SLTon = d.SLTon,
                                            SoLuongCoTheXuat = d.SoLuongCoTheXuat,
                                        //HamLuong =""
                                        //HoatChat =
                                        //DuongDungId = 
                                            DVT = d.DVT,
                                            NhaSX = d.NhaSX,
                                            NuocSX = d.NuocSX,
                                            IsValidator = d.IsValidator
                                        }).ToList();

                                    foreach (var item in yeuCauLinhDuocPhamChiTiets)
                                    {
                                        if (item.Nhom == "Thuốc BHYT")
                                        {
                                            item.LaDuocPhamBHYT = true;
                                        }
                                        else
                                        {
                                            item.LaDuocPhamBHYT = false;
                                        }
                                    }
                                    linhThuongKSNKVM.LoaiPhieuLinh = Enums.EnumLoaiPhieuLinh.LinhDuTru;
                                    if (linhThuongKSNKVM.DaGui == true)
                                    {
                                        linhThuongDuocPham.NgayYeuCau = DateTime.Now;
                                    }

                                    var linhThuongDuocPhamVM = new LinhThuongDuocPhamViewModel()
                                    {
                                        KhoXuatId = itemInFoKhoLinhs.Select(d => d.KhoXuatId).First(),
                                        TenKhoXuat = linhThuongKSNKVM.TenKhoXuat,
                                        KhoNhapId = linhThuongKSNKVM.KhoNhapId,
                                        TenKhoNhap = linhThuongKSNKVM.TenKhoNhap,
                                        LoaiPhieuLinh = linhThuongKSNKVM.LoaiPhieuLinh,
                                        NhanVienYeuCauId = linhThuongKSNKVM.NhanVienYeuCauId,
                                        NhanVienDuyetId = linhThuongKSNKVM.NhanVienDuyetId,
                                        NgayDuyet = linhThuongKSNKVM.NgayDuyet,
                                        DuocDuyet = linhThuongKSNKVM.DuocDuyet,
                                        HoTenNguoiYeuCau = linhThuongKSNKVM.HoTenNguoiYeuCau,
                                        HoTenNguoiDuyet = linhThuongKSNKVM.HoTenNguoiDuyet,
                                        NgayYeuCau = linhThuongKSNKVM.NgayYeuCau,
                                        GhiChu = linhThuongKSNKVM.GhiChu,
                                        LyDoKhongDuyet = linhThuongKSNKVM.LyDoKhongDuyet,
                                        IsLuu = linhThuongKSNKVM.IsLuu,
                                        LastModified = linhThuongKSNKVM.LastModified,
                                        LaNguoiTaoPhieu = linhThuongKSNKVM.LaNguoiTaoPhieu,
                                        DaGui = linhThuongKSNKVM.DaGui,
                                        YeuCauLinhDuocPhamChiTiets = yeuCauLinhDuocPhamChiTiets,
                                        Id = linhThuongKSNKVM.Id
                                    };


                                    linhThuongDuocPhamVM.ToEntity(linhThuongDuocPham);
                                    linhThuongDuocPham.NoiYeuCauId = _userAgentHelper.GetCurrentNoiLLamViecId();
                                    await _yeuCauLinhDuocPhamService.UpdateAsync(linhThuongDuocPham);

                                    linhThuongKSNKVM.Id = 0;

                                    EnumTrangThaiPhieuLinh enumTrangThaiPhieuLinh;
                                    if (linhThuongDuocPham.DaGui != true)
                                    {
                                        enumTrangThaiPhieuLinh = EnumTrangThaiPhieuLinh.DangChoGui;
                                    }
                                    else
                                    {
                                        if (linhThuongDuocPham.DuocDuyet == true)
                                        {
                                            enumTrangThaiPhieuLinh = EnumTrangThaiPhieuLinh.DaDuyet;
                                        }
                                        else if (linhThuongDuocPham.DuocDuyet == false)
                                        {
                                            enumTrangThaiPhieuLinh = EnumTrangThaiPhieuLinh.TuChoiDuyet;
                                        }
                                        else
                                        {
                                            enumTrangThaiPhieuLinh = EnumTrangThaiPhieuLinh.DangChoDuyet;
                                        }
                                    }
                                    var ten = enumTrangThaiPhieuLinh.GetDescription();
                                    var result = new
                                    {
                                        linhThuongDuocPham.Id,
                                        linhThuongDuocPham.LastModified,
                                        enumTrangThaiPhieuLinh,
                                        ten
                                    };
                                    var newPhieu = new GridVoYeuCauLinhDuocTao
                                    {
                                        LoaiDuocPhamHayVatTu = true,
                                        YeuCauLinhVatTuId = linhThuongDuocPham.Id
                                    };
                                    yeuCauLinhDuocPhamVatTuIds.Add(newPhieu);
                                }

                                // chi tiết lấy all
                                if (linhThuongKSNKVM.Id != 0 )
                                {
                                    await _yeuCauLinhDuocPhamService.CheckPhieuLinhDaDuyetHoacDaHuy(linhThuongKSNKVM.Id);


                                    if (!itemInFoKhoLinhs.Where(d => d.LoaiDuocPhamHayVatTu == true).Any())
                                    {
                                        throw new ApiException(_localizationService.GetResource("LinhThuongDuocPham.LinhDuocPhamChiTiets.Required"));
                                    }
                                    var checkKhoQuanLyDeleted = await _yeuCauLinhDuocPhamService.CheckKhoNhanVienQuanLy(linhThuongKSNKVM.KhoNhapId.Value);
                                    if (checkKhoQuanLyDeleted == false)
                                    {
                                        throw new ApiException(_localizationService.GetResource("LinhThuongDuocPham.KhoNhanVienQuanLy.NotExists"));
                                    }
                                    if (linhThuongKSNKVM.DuocDuyet == false && !linhThuongKSNKVM.IsLuu) // Từ chối duyệt
                                    {
                                        linhThuongKSNKVM.DuocDuyet = null;
                                        linhThuongKSNKVM.NhanVienDuyetId = null;
                                        linhThuongKSNKVM.NgayDuyet = null;
                                    }
                                    var linhThuongDuocPham = await _yeuCauLinhDuocPhamService.GetByIdAsync(linhThuongKSNKVM.Id, s => s.Include(r => r.YeuCauLinhDuocPhamChiTiets));
                                    if (linhThuongDuocPham == null)
                                    {
                                        return NotFound();
                                    }
                                    var yeuCauLinhDuocPhamChiTiets = itemInFoKhoLinhs.Where(d => d.LoaiDuocPhamHayVatTu == true)
                                        .Select(d => new LinhThuongDuocPhamChiTietViewModel
                                        {
                                            Id = d.Id,
                                            Nhom = d.LaVatTuBHYT == true ? "BHYT" : "Không BHYT",
                                            YeuCauLinhDuocPhamId = d.YeuCauLinhVatTuId,
                                            DuocPhamBenhVienId = (long)d.VatTuBenhVienId,
                                            Ten = d.Ten,
                                            LaDuocPhamBHYT = (bool)d.LaVatTuBHYT,
                                            KhoXuatId = (long)itemInFoKhoLinhs.Select(f => f.KhoXuatId).First(),
                                            DuocDuyet = d.DuocDuyet,
                                            SoLuong = d.SoLuong,
                                            SLYeuCau = d.SLYeuCau,
                                            SLTon = d.SLTon,
                                            SoLuongCoTheXuat = d.SoLuongCoTheXuat,
                                            //HamLuong =""
                                            //HoatChat =
                                            //DuongDungId = 
                                            DVT = d.DVT,
                                            NhaSX = d.NhaSX,
                                            NuocSX = d.NuocSX,
                                            IsValidator = d.IsValidator
                                        }).ToList();

                                    foreach (var item in yeuCauLinhDuocPhamChiTiets)
                                    {
                                        if (item.Nhom == "Thuốc BHYT")
                                        {
                                            item.LaDuocPhamBHYT = true;
                                        }
                                        else
                                        {
                                            item.LaDuocPhamBHYT = false;
                                        }
                                    }
                                    linhThuongKSNKVM.LoaiPhieuLinh = Enums.EnumLoaiPhieuLinh.LinhDuTru;
                                    if (linhThuongKSNKVM.DaGui == true)
                                    {
                                        linhThuongDuocPham.NgayYeuCau = DateTime.Now;
                                    }
                                    linhThuongKSNKVM.ToEntity(linhThuongDuocPham);
                                    linhThuongDuocPham.NoiYeuCauId = _userAgentHelper.GetCurrentNoiLLamViecId();
                                    await _yeuCauLinhDuocPhamService.UpdateAsync(linhThuongDuocPham);
                                    EnumTrangThaiPhieuLinh enumTrangThaiPhieuLinh;
                                    if (linhThuongDuocPham.DaGui != true)
                                    {
                                        enumTrangThaiPhieuLinh = EnumTrangThaiPhieuLinh.DangChoGui;
                                    }
                                    else
                                    {
                                        if (linhThuongDuocPham.DuocDuyet == true)
                                        {
                                            enumTrangThaiPhieuLinh = EnumTrangThaiPhieuLinh.DaDuyet;
                                        }
                                        else if (linhThuongDuocPham.DuocDuyet == false)
                                        {
                                            enumTrangThaiPhieuLinh = EnumTrangThaiPhieuLinh.TuChoiDuyet;
                                        }
                                        else
                                        {
                                            enumTrangThaiPhieuLinh = EnumTrangThaiPhieuLinh.DangChoDuyet;
                                        }
                                    }
                                    var ten = enumTrangThaiPhieuLinh.GetDescription();
                                    var result = new
                                    {
                                        linhThuongDuocPham.Id,
                                        linhThuongDuocPham.LastModified,
                                        enumTrangThaiPhieuLinh,
                                        ten
                                    };
                                    var newPhieu = new GridVoYeuCauLinhDuocTao
                                    {
                                        LoaiDuocPhamHayVatTu = true,
                                        YeuCauLinhVatTuId = linhThuongDuocPham.Id
                                    };
                                    yeuCauLinhDuocPhamVatTuIds.Add(newPhieu);
                                }
                            }
                            // VT
                            if (itemInFoKhoLinhs.Where(d => d.LoaiDuocPhamHayVatTu == false).Count() != 0)
                            {
                                /////////////-------------------------------------------------------------------------------///////////////////////////////
                                var vatTuChiTiet = itemInFoKhoLinhs.Where(d => d.LoaiDuocPhamHayVatTu == false).ToList();
                                if (linhThuongKSNKVM.Id == 0 && vatTuChiTiet.All(d => d.YeuCauLinhVatTuId == 0))
                                {
                                    linhThuongKSNKVM.YeuCauLinhVatTuChiTiets = vatTuChiTiet.Where(d => d.YeuCauLinhVatTuId != 0).ToList();
                                    var result = await AddKSNK(linhThuongKSNKVM);
                                    yeuCauLinhDuocPhamVatTuIds.AddRange(result);
                                }
                                /////////////END-------------------------------------------------------------------------------///////////////////////////////
                                ///

                                /////////////-------------------------------------------------------------------------------///////////////////////////////
                                ///
                                if (linhThuongKSNKVM.Id == 0 && vatTuChiTiet.All(d => d.YeuCauLinhVatTuId != 0))
                                {
                                    linhThuongKSNKVM.Id = vatTuChiTiet.Where(d => d.YeuCauLinhVatTuId != null).Select(d => (long)d.YeuCauLinhVatTuId).FirstOrDefault();

                                    await _yeuCauLinhKSNKService.CheckPhieuLinhDaDuyetHoacDaHuy(linhThuongKSNKVM.Id);
                                    if (!itemInFoKhoLinhs.Where(d => d.LoaiDuocPhamHayVatTu == false).Any())
                                    {
                                        throw new ApiException(_localizationService.GetResource("LinhThuongDuocPham.LinhDuocPhamChiTiets.Required"));
                                    }

                                    var linhThuongKSNK = await _yeuCauLinhKSNKService.GetByIdAsync(linhThuongKSNKVM.Id, s => s.Include(r => r.YeuCauLinhVatTuChiTiets));
                                    if (linhThuongKSNK == null)
                                    {
                                        return NotFound();
                                    }
                                    foreach (var item in itemInFoKhoLinhs.Where(d => d.LoaiDuocPhamHayVatTu == false).ToList())
                                    {
                                        if (item.Nhom == "Vật Tư BHYT")
                                        {
                                            item.LaVatTuBHYT = true;
                                        }
                                        else
                                        {
                                            item.LaVatTuBHYT = false;
                                        }
                                    }
                                    linhThuongKSNKVM.LoaiPhieuLinh = Enums.EnumLoaiPhieuLinh.LinhDuTru;
                                    if (linhThuongKSNKVM.DaGui == true)
                                    {
                                        linhThuongKSNK.NgayYeuCau = DateTime.Now;
                                    }

                                    linhThuongKSNKVM.YeuCauLinhVatTuChiTiets = itemInFoKhoLinhs.Where(d => d.LoaiDuocPhamHayVatTu == false).ToList();

                                    linhThuongKSNKVM.KhoXuatId = linhThuongKSNKVM.YeuCauLinhVatTuChiTiets.Select(d => d.KhoXuatId).First();

                                    linhThuongKSNKVM.ToEntity(linhThuongKSNK);
                                    linhThuongKSNK.NoiYeuCauId = _userAgentHelper.GetCurrentNoiLLamViecId();
                                    await _yeuCauLinhKSNKService.UpdateAsync(linhThuongKSNK);

                                    linhThuongKSNKVM.Id = 0;

                                    EnumTrangThaiPhieuLinh enumTrangThaiPhieuLinh;
                                    if (linhThuongKSNK.DaGui != true)
                                    {
                                        enumTrangThaiPhieuLinh = EnumTrangThaiPhieuLinh.DangChoGui;
                                    }
                                    else
                                    {
                                        if (linhThuongKSNK.DuocDuyet == true)
                                        {
                                            enumTrangThaiPhieuLinh = EnumTrangThaiPhieuLinh.DaDuyet;
                                        }
                                        else if (linhThuongKSNK.DuocDuyet == false)
                                        {
                                            enumTrangThaiPhieuLinh = EnumTrangThaiPhieuLinh.TuChoiDuyet;
                                        }
                                        else
                                        {
                                            enumTrangThaiPhieuLinh = EnumTrangThaiPhieuLinh.DangChoDuyet;
                                        }
                                    }
                                    var ten = enumTrangThaiPhieuLinh.GetDescription();
                                    var result = new
                                    {
                                        linhThuongKSNK.Id,
                                        linhThuongKSNK.LastModified,
                                        enumTrangThaiPhieuLinh,
                                        ten
                                    };
                                    var newPhieu = new GridVoYeuCauLinhDuocTao
                                    {
                                        LoaiDuocPhamHayVatTu = false,
                                        YeuCauLinhVatTuId = linhThuongKSNK.Id
                                    };
                                    yeuCauLinhDuocPhamVatTuIds.Add(newPhieu);
                                }

                                /////////////END-------------------------------------------------------------------------------///////////////////////////////
                                ////


                                /////////////-------------------------------------------------------------------------------///////////////////////////////
                                ///yêu cầu đã dc tạo Id != 0
                                if (linhThuongKSNKVM.Id != 0)
                                {

                                    await _yeuCauLinhKSNKService.CheckPhieuLinhDaDuyetHoacDaHuy(linhThuongKSNKVM.Id);
                                    if (!itemInFoKhoLinhs.Where(d => d.LoaiDuocPhamHayVatTu == false).Any())
                                    {
                                        throw new ApiException(_localizationService.GetResource("LinhThuongDuocPham.LinhDuocPhamChiTiets.Required"));
                                    }

                                    var linhThuongKSNK = await _yeuCauLinhKSNKService.GetByIdAsync(linhThuongKSNKVM.Id, s => s.Include(r => r.YeuCauLinhVatTuChiTiets));
                                    if (linhThuongKSNK == null)
                                    {
                                        return NotFound();
                                    }
                                    foreach (var item in itemInFoKhoLinhs.Where(d => d.LoaiDuocPhamHayVatTu == false).ToList())
                                    {
                                        if (item.Nhom == "Vật Tư BHYT")
                                        {
                                            item.LaVatTuBHYT = true;
                                        }
                                        else
                                        {
                                            item.LaVatTuBHYT = false;
                                        }
                                    }
                                    linhThuongKSNKVM.LoaiPhieuLinh = Enums.EnumLoaiPhieuLinh.LinhDuTru;
                                    if (linhThuongKSNKVM.DaGui == true)
                                    {
                                        linhThuongKSNK.NgayYeuCau = DateTime.Now;
                                    }

                                    linhThuongKSNKVM.YeuCauLinhVatTuChiTiets = itemInFoKhoLinhs.Where(d => d.LoaiDuocPhamHayVatTu == false).ToList();

                                    linhThuongKSNKVM.KhoXuatId = linhThuongKSNKVM.YeuCauLinhVatTuChiTiets.Select(d => d.KhoXuatId).First();

                                    linhThuongKSNKVM.ToEntity(linhThuongKSNK);
                                    linhThuongKSNK.NoiYeuCauId = _userAgentHelper.GetCurrentNoiLLamViecId();
                                    await _yeuCauLinhKSNKService.UpdateAsync(linhThuongKSNK);
                                    EnumTrangThaiPhieuLinh enumTrangThaiPhieuLinh;
                                    if (linhThuongKSNK.DaGui != true)
                                    {
                                        enumTrangThaiPhieuLinh = EnumTrangThaiPhieuLinh.DangChoGui;
                                    }
                                    else
                                    {
                                        if (linhThuongKSNK.DuocDuyet == true)
                                        {
                                            enumTrangThaiPhieuLinh = EnumTrangThaiPhieuLinh.DaDuyet;
                                        }
                                        else if (linhThuongKSNK.DuocDuyet == false)
                                        {
                                            enumTrangThaiPhieuLinh = EnumTrangThaiPhieuLinh.TuChoiDuyet;
                                        }
                                        else
                                        {
                                            enumTrangThaiPhieuLinh = EnumTrangThaiPhieuLinh.DangChoDuyet;
                                        }
                                    }
                                    var ten = enumTrangThaiPhieuLinh.GetDescription();
                                    var result = new
                                    {
                                        linhThuongKSNK.Id,
                                        linhThuongKSNK.LastModified,
                                        enumTrangThaiPhieuLinh,
                                        ten
                                    };
                                    var newPhieu = new GridVoYeuCauLinhDuocTao
                                    {
                                        LoaiDuocPhamHayVatTu = false,
                                        YeuCauLinhVatTuId = linhThuongKSNK.Id
                                    };
                                    yeuCauLinhDuocPhamVatTuIds.Add(newPhieu);
                                }
                                /////////////END-------------------------------------------------------------------------------///////////////////////////////
                                ////
                            }
                        }
                    }
                }
            }
            if (linhThuongKSNKVM.DaGui == true)
            {
                return Ok(yeuCauLinhDuocPhamVatTuIds.ToList());
            }
            else
            {
                return Ok(yeuCauLinhDuocPhamVatTuIds.Skip(0).Take(1).ToList());
            }
            
        }

        [HttpPost("InPhieuLinhThuongKSNK")]
        public string InPhieuLinhThuongKSNK(PhieuLinhThuongDPVTModel phieuLinhThuongKSNK)
        {
            var result = _yeuCauLinhKSNKService.InPhieuLinhThuongKSNK(phieuLinhThuongKSNK);
            return result;
        }
    
        private  async Task<List<GridVoYeuCauLinhDuocTao>> AddKSNK(LinhThuongKSNKViewModel linhThuongKSNKVM)
        {
            // ds yeuCauDuocTao 
            var yeuCauLinhDuocPhamVatTuIds = new List<GridVoYeuCauLinhDuocTao>();

            // ds theo kho xuất ( bao gồm cả dược phẩm và vật tư)
            if (linhThuongKSNKVM.YeuCauLinhVatTuChiTiets.Any())
            {
                var dsYeuCauLinhVatTuChiTietTheoKhos = linhThuongKSNKVM.YeuCauLinhVatTuChiTiets
                                                       .GroupBy(d => d.KhoXuatId).ToList();

                // tạo theo kho linh (dược phẩm và vật tư) : dp 1 phiếu , vt 1 phiếu
                if (dsYeuCauLinhVatTuChiTietTheoKhos.Any())
                {
                    foreach (var itemInFoKhoLinhs in dsYeuCauLinhVatTuChiTietTheoKhos)
                    {
                        if (itemInFoKhoLinhs.Any())
                        {
                            // tạo dược phẩm
                            if (itemInFoKhoLinhs.Where(d => d.LoaiDuocPhamHayVatTu == true).Count() != 0)
                            {
                                var yeuCauLinhDuocPhamChiTiets = itemInFoKhoLinhs.Where(d => d.LoaiDuocPhamHayVatTu == true)
                                    .Select(d => new LinhThuongDuocPhamChiTietViewModel
                                    {
                                        Id = d.Id,
                                        Nhom = d.LaVatTuBHYT == true ? "BHYT" : "Không BHYT",
                                        YeuCauLinhDuocPhamId = d.YeuCauLinhVatTuId,
                                        DuocPhamBenhVienId = (long)d.VatTuBenhVienId,
                                        Ten = d.Ten,
                                        LaDuocPhamBHYT = (bool)d.LaVatTuBHYT,
                                        KhoXuatId = (long)itemInFoKhoLinhs.Select(f => f.KhoXuatId).First(),
                                        DuocDuyet = d.DuocDuyet,
                                        SoLuong = d.SoLuong,
                                        SLYeuCau = d.SLYeuCau,
                                        SLTon = d.SLTon,
                                        SoLuongCoTheXuat = d.SoLuongCoTheXuat,
                                        //HamLuong =""
                                        //HoatChat =
                                        //DuongDungId = 
                                        DVT = d.DVT,
                                        NhaSX = d.NhaSX,
                                        NuocSX = d.NuocSX,
                                        IsValidator = d.IsValidator
                                    }).ToList();

                                if (!itemInFoKhoLinhs.Select(d => d.LoaiDuocPhamHayVatTu == true).Any())
                                {
                                    throw new ApiException(_localizationService.GetResource("LinhThuongDuocPham.LinhDuocPhamChiTiets.Required"));
                                }
                                var checkKhoQuanLyDeleted = await _yeuCauLinhDuocPhamService.CheckKhoNhanVienQuanLy(linhThuongKSNKVM.KhoNhapId.Value);
                                if (checkKhoQuanLyDeleted == false)
                                {
                                    throw new ApiException(_localizationService.GetResource("LinhThuongDuocPham.KhoNhanVienQuanLy.NotExists"));
                                }

                                foreach (var item in yeuCauLinhDuocPhamChiTiets)
                                {
                                    if (item.Nhom == "BHYT")
                                    {
                                        item.LaDuocPhamBHYT = true;
                                    }
                                    else
                                    {
                                        item.LaDuocPhamBHYT = false;
                                    }
                                }
                                linhThuongKSNKVM.LoaiPhieuLinh = Enums.EnumLoaiPhieuLinh.LinhDuTru;
                                linhThuongKSNKVM.NgayYeuCau = DateTime.Now;
                                var linhThuongDuocPhamVM = new LinhThuongDuocPhamViewModel()
                                {
                                    KhoXuatId = itemInFoKhoLinhs.Select(d => d.KhoXuatId).First(),
                                    TenKhoXuat = linhThuongKSNKVM.TenKhoXuat,
                                    KhoNhapId = linhThuongKSNKVM.KhoNhapId,
                                    TenKhoNhap = linhThuongKSNKVM.TenKhoNhap,
                                    LoaiPhieuLinh = linhThuongKSNKVM.LoaiPhieuLinh,
                                    NhanVienYeuCauId = linhThuongKSNKVM.NhanVienYeuCauId,
                                    NhanVienDuyetId = linhThuongKSNKVM.NhanVienDuyetId,
                                    NgayDuyet = linhThuongKSNKVM.NgayDuyet,
                                    DuocDuyet = linhThuongKSNKVM.DuocDuyet,
                                    HoTenNguoiYeuCau = linhThuongKSNKVM.HoTenNguoiYeuCau,
                                    HoTenNguoiDuyet = linhThuongKSNKVM.HoTenNguoiDuyet,
                                    NgayYeuCau = linhThuongKSNKVM.NgayYeuCau,
                                    GhiChu = linhThuongKSNKVM.GhiChu,
                                    LyDoKhongDuyet = linhThuongKSNKVM.LyDoKhongDuyet,
                                    IsLuu = linhThuongKSNKVM.IsLuu,
                                    LastModified = linhThuongKSNKVM.LastModified,
                                    LaNguoiTaoPhieu = linhThuongKSNKVM.LaNguoiTaoPhieu,
                                    DaGui = linhThuongKSNKVM.DaGui,
                                    YeuCauLinhDuocPhamChiTiets = yeuCauLinhDuocPhamChiTiets
                                };





                                var linhThuongDuocPham = linhThuongDuocPhamVM.ToEntity<YeuCauLinhDuocPham>();
                                linhThuongDuocPham.NoiYeuCauId = _userAgentHelper.GetCurrentNoiLLamViecId();
                                await _yeuCauLinhDuocPhamService.AddAsync(linhThuongDuocPham);

                                var newPhieu = new GridVoYeuCauLinhDuocTao
                                {
                                    LoaiDuocPhamHayVatTu = true,
                                    YeuCauLinhVatTuId = linhThuongDuocPham.Id
                                };
                                yeuCauLinhDuocPhamVatTuIds.Add(newPhieu);
                            }
                            // tạo vật tư
                            if (itemInFoKhoLinhs.Where(d => d.LoaiDuocPhamHayVatTu == false).Count() != 0)
                            {
                                if (!itemInFoKhoLinhs.Where(d => d.LoaiDuocPhamHayVatTu == false).Any())
                                {
                                    throw new ApiException(_localizationService.GetResource("LinhThuongDuocPham.LinhDuocPhamChiTiets.Required"));
                                }
                                var yeuCauLinhChiTietVatTu = itemInFoKhoLinhs.Where(d => d.LoaiDuocPhamHayVatTu == false).ToList();
                                //.Select(d => new LinhThuongKNSKChiTietViewModel
                                //{

                                //}).ToList();
                                foreach (var item in yeuCauLinhChiTietVatTu)
                                {
                                    if (item.Nhom == "BHYT")
                                    {
                                        item.LaVatTuBHYT = true;
                                    }
                                    else
                                    {
                                        item.LaVatTuBHYT = false;
                                    }
                                }
                                linhThuongKSNKVM.YeuCauLinhVatTuChiTiets = yeuCauLinhChiTietVatTu;
                                linhThuongKSNKVM.LoaiPhieuLinh = Enums.EnumLoaiPhieuLinh.LinhDuTru;
                                linhThuongKSNKVM.NgayYeuCau = DateTime.Now;
                                linhThuongKSNKVM.KhoXuatId = linhThuongKSNKVM.YeuCauLinhVatTuChiTiets.First().KhoXuatId;
                                var linhThuongKSNK = linhThuongKSNKVM.ToEntity<YeuCauLinhVatTu>();
                                linhThuongKSNK.NoiYeuCauId = _userAgentHelper.GetCurrentNoiLLamViecId();
                                await _yeuCauLinhKSNKService.AddAsync(linhThuongKSNK);

                                var newPhieu = new GridVoYeuCauLinhDuocTao
                                {
                                    LoaiDuocPhamHayVatTu = false,
                                    YeuCauLinhVatTuId = linhThuongKSNK.Id
                                };
                                yeuCauLinhDuocPhamVatTuIds.Add(newPhieu);
                            }
                        }
                    }
                }
            }



            if (linhThuongKSNKVM.DaGui == true) // true l gui
            {
                return yeuCauLinhDuocPhamVatTuIds;
            }
            else
            {
                return yeuCauLinhDuocPhamVatTuIds.Skip(0).Take(1).ToList();
            }
        }
    }
}
