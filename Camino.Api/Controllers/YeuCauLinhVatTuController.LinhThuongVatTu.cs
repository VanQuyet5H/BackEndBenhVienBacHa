using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.Error;
using Camino.Api.Models.LinhThuongVatTu;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.YeuCauLinhVatTu;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.YeuCauLinhVatTus;
using Microsoft.EntityFrameworkCore;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using static Camino.Core.Domain.Enums;
using Camino.Core.Helpers;
using Newtonsoft.Json;

namespace Camino.Api.Controllers
{
    public partial class YeuCauLinhVatTuController
    {
        [HttpPost("GetKhoLinhVatTu")]
        public async Task<ActionResult> GetKhoLinhVatTu([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _yeuCauLinhVatTuService.GetKhoLinh(queryInfo);
            return Ok(lookup);
        }

        [HttpPost("GetKhoCurrentUserVatTu")]
        public async Task<ActionResult> GetKhoCurrentUserVatTu([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _yeuCauLinhVatTuService.GetKhoCurrentUser(queryInfo);
            return Ok(lookup);
        }

        [HttpGet("GetCurrentUserVatTu")]
        public async Task<ActionResult> GetCurrentUserVatTu()
        {
            var lookup = await _yeuCauLinhVatTuService.GetCurrentUser();
            return Ok(lookup);
        }

        [HttpPost("GetVatTuTheoKho")]
        public async Task<ActionResult<ICollection<VatTuLookupVo>>> GetVatTuTheoKhoLookup(DropDownListRequestModel model)
        {
            var lookup = await _yeuCauLinhVatTuService.GetVatTu(model);
            return Ok(lookup);
        }

        [HttpGet("GetTrangThaiPhieuLinhVatTu")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TaoYeuCauLinhThuongVatTu)]
        public async Task<ActionResult<TrangThaiDuyetVo>> GetTrangThaiPhieuLinhVatTu(long phieuLinhId)
        {
            var result = await _yeuCauLinhVatTuService.GetTrangThaiPhieuLinh(phieuLinhId);
            return Ok(result);
        }

        [HttpPost("ThemLinhThuongVatTuGridVo")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.TaoYeuCauLinhThuongVatTu)]
        public async Task<ActionResult> ThemLinhThuongVatTuGridVo(VatTuGridViewModel model)
        {
            var models = new LinhThuongVatTuGridVo
            {
                VatTuBenhVienId = model.VatTuBenhVienId,
                Ten = model.Ten,
                Ma = model.Ma,
                DVT = model.DVT,
                NhaSX = model.NhaSX,
                NuocSX = model.NuocSX,
                SLYeuCau = model.SLYeuCau,
                LoaiVatTu = model.LoaiVatTu.Value,
                KhoXuatId = model.KhoXuatId,
                LaVatTuBHYT = model.LaVatTuBHYT,
                Nhom = model.LoaiVatTu == 1 ? "Vật Tư Không BHYT" : "Vật Tư BHYT"
            };
            var result = _yeuCauLinhVatTuService.LinhThuongVatTuGridVo(models);
            return Ok(result);
        }

        //GET
        [HttpGet("GetPhieuLinhThuongVatTu")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TaoYeuCauLinhThuongVatTu)]
        public async Task<ActionResult<LinhThuongVatTuViewModel>> GetPhieuLinhThuongVatTu(long id)
        {
            var phieuLinhThuong =
                await _yeuCauLinhVatTuService.GetByIdAsync(id, s => s.Include(r => r.YeuCauLinhVatTuChiTiets).ThenInclude(ct => ct.VatTuBenhVien).ThenInclude(dpct => dpct.VatTus)
                                                                     .Include(r => r.KhoNhap).Include(r => r.KhoXuat)
                                                                     .Include(r => r.NhanVienYeuCau).ThenInclude(nv => nv.User)
                                                                     .Include(r => r.NhanVienDuyet).ThenInclude(nv => nv.User));

            if (phieuLinhThuong == null)
            {
                return NotFound();
            }
            var model = phieuLinhThuong.ToModel<LinhThuongVatTuViewModel>();
            foreach (var item in model.YeuCauLinhVatTuChiTiets)
            {
                if (item.LaVatTuBHYT == true)
                {
                    item.Nhom = "Vật Tư BHYT";
                }
                else
                {
                    item.Nhom = "Vật Tư Không BHYT";
                }
                item.SLTon = _yeuCauLinhVatTuService.GetSoLuongTonVatTuGridVo(item.VatTuBenhVienId.Value, item.KhoXuatId.Value, item.LaVatTuBHYT.Value);
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


        [HttpPost("GuiPhieuLinhThuongVatTu")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.TaoYeuCauLinhThuongVatTu)]
        public async Task<ActionResult> GuiPhieuLinhThuongVatTu(LinhThuongVatTuViewModel linhThuongVatTuVM)
        {
            if (!linhThuongVatTuVM.YeuCauLinhVatTuChiTiets.Any())
            {
                throw new ApiException(_localizationService.GetResource("LinhThuongDuocPham.LinhDuocPhamChiTiets.Required"));
            }
            //var checkKhoQuanLyDeleted = await _yeuCauLinhDuocPhamService.CheckKhoNhanVienQuanLy(linhThuongDuocPhamVM.KhoNhapId.Value);
            //if (checkKhoQuanLyDeleted == false)
            //{
            //    throw new ApiException(_localizationService.GetResource("LinhThuongDuocPham.KhoNhanVienQuanLy.NotExists"));
            //}
            foreach (var item in linhThuongVatTuVM.YeuCauLinhVatTuChiTiets)
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
            linhThuongVatTuVM.LoaiPhieuLinh = Enums.EnumLoaiPhieuLinh.LinhDuTru;
            linhThuongVatTuVM.NgayYeuCau = DateTime.Now;
            var linhThuongVatTu = linhThuongVatTuVM.ToEntity<YeuCauLinhVatTu>();
            linhThuongVatTu.NoiYeuCauId = _userAgentHelper.GetCurrentNoiLLamViecId();
            await _yeuCauLinhVatTuService.AddAsync(linhThuongVatTu);
            return Ok(linhThuongVatTu.Id);
        }

        [HttpPost("GuiLaiPhieuLinhThuongVatTu")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.TaoYeuCauLinhThuongVatTu)]
        public async Task<ActionResult> GuiLaiPhieuLinhThuongVatTu(LinhThuongVatTuViewModel linhThuongVatTuVM)
        {
            await _yeuCauLinhVatTuService.CheckPhieuLinhDaDuyetHoacDaHuy(linhThuongVatTuVM.Id);
            if (!linhThuongVatTuVM.YeuCauLinhVatTuChiTiets.Any())
            {
                throw new ApiException(_localizationService.GetResource("LinhThuongDuocPham.LinhDuocPhamChiTiets.Required"));
            }
            var checkKhoQuanLyDeleted = await _yeuCauLinhDuocPhamService.CheckKhoNhanVienQuanLy(linhThuongVatTuVM.KhoNhapId.Value);
            if (checkKhoQuanLyDeleted == false)
            {
                throw new ApiException(_localizationService.GetResource("LinhThuongDuocPham.KhoNhanVienQuanLy.NotExists"));
            }
            if (linhThuongVatTuVM.DuocDuyet == false && !linhThuongVatTuVM.IsLuu) // Từ chối duyệt
            {
                linhThuongVatTuVM.DuocDuyet = null;
                linhThuongVatTuVM.NhanVienDuyetId = null;
                linhThuongVatTuVM.NgayDuyet = null;
            }
            var linhThuongVatTu = await _yeuCauLinhVatTuService.GetByIdAsync(linhThuongVatTuVM.Id, s => s.Include(r => r.YeuCauLinhVatTuChiTiets));
            if (linhThuongVatTu == null)
            {
                return NotFound();
            }
            foreach (var item in linhThuongVatTuVM.YeuCauLinhVatTuChiTiets)
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
            linhThuongVatTuVM.LoaiPhieuLinh = Enums.EnumLoaiPhieuLinh.LinhDuTru;
            if (linhThuongVatTuVM.DaGui == true)
            {
                linhThuongVatTu.NgayYeuCau = DateTime.Now;
            }
            linhThuongVatTuVM.ToEntity(linhThuongVatTu);
            linhThuongVatTu.NoiYeuCauId = _userAgentHelper.GetCurrentNoiLLamViecId();
            await _yeuCauLinhVatTuService.UpdateAsync(linhThuongVatTu);
            EnumTrangThaiPhieuLinh enumTrangThaiPhieuLinh;
            if (linhThuongVatTu.DaGui != true)
            {
                enumTrangThaiPhieuLinh = EnumTrangThaiPhieuLinh.DangChoGui;
            }
            else
            {
                if (linhThuongVatTu.DuocDuyet == true)
                {
                    enumTrangThaiPhieuLinh = EnumTrangThaiPhieuLinh.DaDuyet;
                }
                else if (linhThuongVatTu.DuocDuyet == false)
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
                linhThuongVatTu.Id,
                linhThuongVatTu.LastModified,
                enumTrangThaiPhieuLinh,
                ten
            };
            return Ok(result);
        }

        [HttpPost("InPhieuLinhThuongVatTu")]
        public string InPhieuLinhThuongVatTu(PhieuLinhThuongVatTu phieuLinhThuongVatTu)
        {
            var result = _yeuCauLinhVatTuService.InPhieuLinhThuongVatTu(phieuLinhThuongVatTu);
            return result;
        }
        [HttpPost("InPhieuLinhBuVatTuXemTruoc")]
        public string InPhieuLinhBuVatTuXemTruoc(PhieuLinhThuongVatTuXemTruoc phieuLinhThuongVatTuXemTruoc)
        {
            var yeuCauVatTuBenhVienDateTimes = new List<DateTime>();
            var yeuCauVatTuVienIds = new List<long>();
            if (phieuLinhThuongVatTuXemTruoc.YeuCauVatTuBenhViens != null)
            {
                foreach (var item in phieuLinhThuongVatTuXemTruoc.YeuCauVatTuBenhViens.ToList())
                {
                    var itemIds = _yeuCauLinhVatTuService.GetIdsYeuCauVT(item.KhoLinhTuId, item.KhoLinhVeId, item.VatTuBenhVienId);
                    if (itemIds.Any())
                    {
                        yeuCauVatTuVienIds.AddRange(itemIds);
                    }
                }
               
            }
            if (yeuCauVatTuVienIds.Any())
            {
                foreach (var item in yeuCauVatTuVienIds)
                {
                    var itemDateTiem = _yeuCauLinhVatTuService.GetDateTime(item);
                    yeuCauVatTuBenhVienDateTimes.Add(itemDateTiem);
                }
            }
            if (yeuCauVatTuBenhVienDateTimes.Any())
            {
                var sortDateTimeTungay = yeuCauVatTuBenhVienDateTimes.OrderBy(s => s).ToList();
                //r sortDateTimeDenNgay = yeuCauVatTuBenhVienDateTimes.OrderByDescending(s => s).ToList();
                phieuLinhThuongVatTuXemTruoc.ThoiDiemLinhTongHopTuNgay = sortDateTimeTungay.First();
                phieuLinhThuongVatTuXemTruoc.ThoiDiemLinhTongHopDenNgay = DateTime.Now;
            }
            phieuLinhThuongVatTuXemTruoc.YeuCauVatTuBenhVienIds = yeuCauVatTuVienIds;
            var result = _yeuCauLinhVatTuService.InPhieuLinhBuVatTuXemTruoc(phieuLinhThuongVatTuXemTruoc);
            return result;
        }
    }
}
