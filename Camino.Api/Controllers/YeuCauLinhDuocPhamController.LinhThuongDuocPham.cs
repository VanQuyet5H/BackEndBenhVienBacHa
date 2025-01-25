using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.Error;
using Camino.Api.Models.LinhThuongDuocPham;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.YeuCauLinhDuocPhams;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using Camino.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public partial class YeuCauLinhDuocPhamController
    {
        [HttpPost("GetKhoLinh")]
        public async Task<ActionResult> GetKhoLinh([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _yeuCauLinhDuocPhamService.GetKhoLinh(queryInfo);
            return Ok(lookup);
        }

        [HttpPost("GetKhoCurrentUser")]
        public async Task<ActionResult> GetKhoCurrentUser([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _yeuCauLinhDuocPhamService.GetKhoCurrentUser(queryInfo);
            return Ok(lookup);
        }

        [HttpGet("GetCurrentUser")]
        public async Task<ActionResult> GetCurrentUser()
        {
            var lookup = await _yeuCauLinhDuocPhamService.GetCurrentUser();
            return Ok(lookup);
        }

        [HttpGet("GetNhanVienDuyet")]
        public async Task<ActionResult> GetNhanVienDuyet(long? id)
        {
            var lookup = await _yeuCauLinhDuocPhamService.GetNhanVienDuyet(id.Value);
            return Ok(lookup);
        }

        [HttpPost("GetDuocPhamKho")]
        public async Task<ActionResult<ICollection<DuocPhamLookupVo>>> GetDuocPhamKho(DropDownListRequestModel model)
        {
            var lookup = await _yeuCauLinhDuocPhamService.GetDuocPham(model);            
            return Ok(lookup);
        }

        [HttpPost("ThemLinhThuongDuocPhamGridVo")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.TaoYeuCauLinhThuongDuocPham)]
        public async Task<ActionResult> ThemLinhThuongDuocPhamGridVo(DuocPhamGridViewModel model)
        {
            var models = new LinhThuongDuocPhamVo
            {
                DuocPhamBenhVienId = model.DuocPhamBenhVienId.Value,
                Ten = model.Ten,
                HamLuong = model.HamLuong,
                HoatChat = model.HoatChat,
                DuongDungId = model.DuongDungId,
                DuongDung = model.DuongDung,
                DVTId = model.DVTId,
                DVT = model.DVT,
                NhaSX = model.NhaSX,
                NuocSX = model.NuocSX,
                SLYeuCau = model.SLYeuCau,
                KhoXuatId = model.KhoXuatId,
                LoaiDuocPham = model.LoaiDuocPham,
                LaDuocPhamBHYT = model.LaDuocPhamBHYT,
            };
            var result = _yeuCauLinhDuocPhamService.LinhThuongDuocPhamGridVo(models);
            return Ok(result);
        }

        [HttpPost("GetAllMayXetNghiemYeuCauLinh")]
        public ActionResult GetAllMayXetNghiemYeuCauLinh([FromBody]DropDownListRequestModel model)
        {
            var duocPhamMayXetNghiemJson = JsonConvert.DeserializeObject<DuocPhamBenhVienMayXetNghiemJson>(model.ParameterDependencies.Replace("undefined", "null"));

            return Ok(_yeuCauLinhDuocPhamService.GetAllMayXetNghiemYeuCauLinh(model , duocPhamMayXetNghiemJson));
        }

        [HttpPost("GuiPhieuLinhThuong")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.TaoYeuCauLinhThuongDuocPham)]
        public async Task<ActionResult> GuiPhieuLinhThuong(LinhThuongDuocPhamViewModel linhThuongDuocPhamVM)
        {
            if (!linhThuongDuocPhamVM.YeuCauLinhDuocPhamChiTiets.Any())
            {
                throw new ApiException(_localizationService.GetResource("LinhThuongDuocPham.LinhDuocPhamChiTiets.Required"));
            }
            var checkKhoQuanLyDeleted = await _yeuCauLinhDuocPhamService.CheckKhoNhanVienQuanLy(linhThuongDuocPhamVM.KhoNhapId.Value);
            if (checkKhoQuanLyDeleted == false)
            {
                throw new ApiException(_localizationService.GetResource("LinhThuongDuocPham.KhoNhanVienQuanLy.NotExists"));
            }
            foreach (var item in linhThuongDuocPhamVM.YeuCauLinhDuocPhamChiTiets)
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
            linhThuongDuocPhamVM.LoaiPhieuLinh = Enums.EnumLoaiPhieuLinh.LinhDuTru;
            linhThuongDuocPhamVM.NgayYeuCau = DateTime.Now;
            var linhThuongDuocPham = linhThuongDuocPhamVM.ToEntity<YeuCauLinhDuocPham>();
            linhThuongDuocPham.NoiYeuCauId = _userAgentHelper.GetCurrentNoiLLamViecId();
            await _yeuCauLinhDuocPhamService.AddAsync(linhThuongDuocPham);
            return Ok(linhThuongDuocPham.Id);
        }

        [HttpPost("GuiLaiPhieuLinhThuong")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.TaoYeuCauLinhThuongDuocPham)]
        public async Task<ActionResult> GuiLaiPhieuLinhThuong(LinhThuongDuocPhamViewModel linhThuongDuocPhamVM)
        {
            await _yeuCauLinhDuocPhamService.CheckPhieuLinhDaDuyetHoacDaHuy(linhThuongDuocPhamVM.Id);
            if (!linhThuongDuocPhamVM.YeuCauLinhDuocPhamChiTiets.Any())
            {
                throw new ApiException(_localizationService.GetResource("LinhThuongDuocPham.LinhDuocPhamChiTiets.Required"));
            }
            var checkKhoQuanLyDeleted = await _yeuCauLinhDuocPhamService.CheckKhoNhanVienQuanLy(linhThuongDuocPhamVM.KhoNhapId.Value);
            if (checkKhoQuanLyDeleted == false)
            {
                throw new ApiException(_localizationService.GetResource("LinhThuongDuocPham.KhoNhanVienQuanLy.NotExists"));
            }
            if (linhThuongDuocPhamVM.DuocDuyet == false && !linhThuongDuocPhamVM.IsLuu) // Từ chối duyệt
            {
                linhThuongDuocPhamVM.DuocDuyet = null;
                linhThuongDuocPhamVM.NhanVienDuyetId = null;
                linhThuongDuocPhamVM.NgayDuyet = null;
            }
            var linhThuongDuocPham = await _yeuCauLinhDuocPhamService.GetByIdAsync(linhThuongDuocPhamVM.Id, s => s.Include(r => r.YeuCauLinhDuocPhamChiTiets));
            if (linhThuongDuocPham == null)
            {
                return NotFound();
            }
            foreach (var item in linhThuongDuocPhamVM.YeuCauLinhDuocPhamChiTiets)
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
            linhThuongDuocPhamVM.LoaiPhieuLinh = Enums.EnumLoaiPhieuLinh.LinhDuTru;
            if (linhThuongDuocPhamVM.DaGui == true)
            {
                linhThuongDuocPham.NgayYeuCau = DateTime.Now;
            }

            linhThuongDuocPhamVM.ToEntity(linhThuongDuocPham);
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
            return Ok(result);
        }

        [HttpPost("InPhieuLinhThuongDuocPham")]
        public string InPhieuLinhThuongDuocPham(PhieuLinhThuongDuocPham phieuLinhThuongDuoc)
        {
            var result = _yeuCauLinhDuocPhamService.InPhieuLinhThuongDuocPham(phieuLinhThuongDuoc);
            return result;
        }

        //GET
        [HttpGet("GetPhieuLinhThuongDuocPham")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TaoYeuCauLinhThuongDuocPham)]
        public async Task<ActionResult<LinhThuongDuocPhamViewModel>> Get(long id)
        {
            var phieuLinhThuong = await _yeuCauLinhDuocPhamService
                .GetByIdAsync(id, s =>
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

            //Gắn tên máy XN cho yêu cầu lĩnh thường 
            foreach (var ycLinhDuocPhamChiTiet in model.YeuCauLinhDuocPhamChiTiets)
            {
                ycLinhDuocPhamChiTiet.DanhSachTenMayXetNghiem = _yeuCauLinhDuocPhamService.GetTrangThaiPhieuLinh(ycLinhDuocPhamChiTiet.DanhSachMayXetNghiemId);
            }

            foreach (var item in model.YeuCauLinhDuocPhamChiTiets)
            {

                if (item.LaDuocPhamBHYT == true)
                {
                    item.Nhom = "Thuốc BHYT";
                }
                else
                {
                    item.Nhom = "Thuốc Không BHYT";
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
            return Ok(model);
        }

        [HttpGet("GetTrangThaiPhieuLinh")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TaoYeuCauLinhThuongDuocPham)]
        public async Task<ActionResult<TrangThaiDuyetVo>> GetTrangThaiPhieuLinh(long phieuLinhId)
        {
            var result = await _yeuCauLinhDuocPhamService.GetTrangThaiPhieuLinh(phieuLinhId);
            return Ok(result);
        }
    }
}
