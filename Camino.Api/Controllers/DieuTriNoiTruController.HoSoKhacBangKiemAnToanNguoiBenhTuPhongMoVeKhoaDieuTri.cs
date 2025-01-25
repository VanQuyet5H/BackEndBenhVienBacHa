using Camino.Api.Extensions;
using Camino.Api.Models.BangKiemAnToanPhauThuatTuPhongDieuTri;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BangKiemAnToanNguoiBenhPTTuPhongDieuTri;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.TrichBienBanHoiChan;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Controllers
{
    public partial class DieuTriNoiTruController
    {
        [HttpPost("GetDanhSachPhauThuatVien")]
        public async Task<List<string>> GetDanhSachPhauThuatVien()
        {
            var lookup = await _dieuTriNoiTruService.GetDanhSachPhauThuatVien();
            return lookup;
        }
        [HttpPost("GetDanhSachXeNghiemCanLam")]
        public async Task<ActionResult> GetDanhSachXeNghiemCanLam([FromBody]DropDownListRequestModel model)
        {
            var listEnum = await _dieuTriNoiTruService.GetDanhSachXeNghiemCanLam(model);
            return Ok(listEnum);
        }
        [HttpPost("GetListThuocDaDung")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListThuocDaDung([FromBody]DropDownListRequestModel queryInfo, long yeuCauTiepNhanId)
        {
            var lookup = await _dieuTriNoiTruService.GetListThuocDaDung(queryInfo, yeuCauTiepNhanId);
            return Ok(lookup);
        }
        [HttpPost("GetListThuocDangDung")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListThuocDangDung([FromBody]DropDownListRequestModel queryInfo, long yeuCauTiepNhanId)
        {
            var lookup = await _dieuTriNoiTruService.GetListThuocDangDung(queryInfo, yeuCauTiepNhanId);
            return Ok(lookup);
        }
        [HttpPost("GetListThuocBanGiao")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListThuocBanGiao([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _dieuTriNoiTruService.GetListThuocBanGiao(queryInfo);
            return Ok(lookup);
        }
        [HttpPost("GetDanhSachNguoiBenhAnToanPTTuPhongDieuTri")]
        public async Task<ActionResult<GridDataSource>> GetDanhSachNguoiBenhAnToanPTTuPhongDieuTri([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _dieuTriNoiTruService.GetDanhSachNguoiBenhAnToanPTTuPhongDieuTri(queryInfo);
            return Ok(gridData);
        }
        [HttpPost("GetThongTinBenhNhanPtVephongDieuTri")]
        public BangKiemAnToanNguoiBenhPTTuPhongDieuTriGrid GetThongTinBenhNhanPtVephongDieuTri(long yeuCauTiepNhanId)
        {
            var lookup = _dieuTriNoiTruService.GetThongTinBangKiemAnToanNguoiBenhPTTuPhongDieuTri(yeuCauTiepNhanId);
            return lookup;
        }
        #region save / update
        [HttpPost("LuuBangKiemAnToanPhauThuatTuPhongDieuTri")]
        //[ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.TrichBienBanHoiChan)]
        public async Task<ActionResult<BangKiemAnToanPhauThuatTuPhongDieuTriViewModel>> LuuBangKiemAnToanPhauThuatTuPhongDieuTri([FromBody] BangKiemAnToanPhauThuatTuPhongDieuTriViewModel bangKiemAnToanPhauThuatTuPhongDieuTriViewModel)
        {
            if (bangKiemAnToanPhauThuatTuPhongDieuTriViewModel.Id == 0)
            {
                var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
                var noiThucHien = _userAgentHelper.GetCurrentNoiLLamViecId();
                var trichBienBanNoiTru = new BangKiemAnToanPhauThuatTuPhongDieuTriViewModel()
                {
                    YeuCauTiepNhanId = bangKiemAnToanPhauThuatTuPhongDieuTriViewModel.YeuCauTiepNhanId,
                    LoaiHoSoDieuTriNoiTru = bangKiemAnToanPhauThuatTuPhongDieuTriViewModel.LoaiHoSoDieuTriNoiTru,
                    ThongTinHoSo = bangKiemAnToanPhauThuatTuPhongDieuTriViewModel.ThongTinHoSo,
                    NhanVienThucHienId = nguoiDangLogin,
                    NoiThucHienId = noiThucHien,
                    ThoiDiemThucHien = DateTime.Now
                };
                var user = trichBienBanNoiTru.ToEntity<NoiTruHoSoKhac>();
                if (bangKiemAnToanPhauThuatTuPhongDieuTriViewModel.FileChuKy != null)
                {
                    foreach (var itemfileChuKy in bangKiemAnToanPhauThuatTuPhongDieuTriViewModel.FileChuKy)
                    {
                        var noiTruHoSoKhacFileDinhKem = new NoiTruHoSoKhacFileDinhKem()
                        {
                            //NoiTruHoSoKhacId = user.Id,
                            Ma = itemfileChuKy.Uid,
                            Ten = itemfileChuKy.Ten,
                            TenGuid = itemfileChuKy.TenGuid,
                            DuongDan = itemfileChuKy.DuongDan,
                            LoaiTapTin = itemfileChuKy.LoaiTapTin,
                            MoTa = itemfileChuKy.MoTa,
                            KichThuoc = itemfileChuKy.KichThuoc
                        };
                        _taiLieuDinhKemService.LuuTaiLieuAsync(noiTruHoSoKhacFileDinhKem.DuongDan, noiTruHoSoKhacFileDinhKem.TenGuid);
                        user.NoiTruHoSoKhacFileDinhKems.Add(noiTruHoSoKhacFileDinhKem);
                    }
                }
               await  _noiDuTruHoSoKhacService.AddAsync(user);
                return CreatedAtAction(nameof(Get), new { id = user.Id }, user.ToModel<BangKiemAnToanPhauThuatTuPhongDieuTriViewModel>());
            }
            else
            {
                var update = await _noiDuTruHoSoKhacService.GetByIdAsync(bangKiemAnToanPhauThuatTuPhongDieuTriViewModel.Id,s=>s.Include(x=>x.NoiTruHoSoKhacFileDinhKems));
                var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
                var noiThucHien = _userAgentHelper.GetCurrentNoiLLamViecId();
                update.YeuCauTiepNhanId = bangKiemAnToanPhauThuatTuPhongDieuTriViewModel.YeuCauTiepNhanId;
                update.LoaiHoSoDieuTriNoiTru = bangKiemAnToanPhauThuatTuPhongDieuTriViewModel.LoaiHoSoDieuTriNoiTru;
                update.ThongTinHoSo = bangKiemAnToanPhauThuatTuPhongDieuTriViewModel.ThongTinHoSo;
                update.NhanVienThucHienId = nguoiDangLogin;
                update.NoiThucHienId = noiThucHien;
                //update.ThoiDiemThucHien = DateTime.Now;
                if (update == null)
                {
                    return NotFound();
                }
                bangKiemAnToanPhauThuatTuPhongDieuTriViewModel.ToEntity<NoiTruHoSoKhac>();
                // remove list fileChuKy hiện tại
                foreach (var itemNoiTruHoSoKhacFileDinhKem in update.NoiTruHoSoKhacFileDinhKems.ToList())
                {
                    var soket = await _noiDuTruHoSoKhacFileDinhKemService.GetByIdAsync(itemNoiTruHoSoKhacFileDinhKem.Id);
                    if (soket == null)
                    {
                        return NotFound();
                    }
                    await _noiDuTruHoSoKhacFileDinhKemService.DeleteByIdAsync(soket.Id);
                }
                if (bangKiemAnToanPhauThuatTuPhongDieuTriViewModel.Id != 0)
                {
                    if (bangKiemAnToanPhauThuatTuPhongDieuTriViewModel.FileChuKy.Count() > 0)
                    {
                        foreach (var itemfileChuKy in bangKiemAnToanPhauThuatTuPhongDieuTriViewModel.FileChuKy)
                        {
                            var noiTruHoSoKhacFileDinhKem = new NoiTruHoSoKhacFileDinhKem()
                            {
                                Ma = itemfileChuKy.Uid,
                                Ten = itemfileChuKy.Ten,
                                TenGuid = itemfileChuKy.TenGuid,
                                DuongDan = itemfileChuKy.DuongDan,
                                LoaiTapTin = itemfileChuKy.LoaiTapTin,
                                MoTa = itemfileChuKy.MoTa,
                                KichThuoc = itemfileChuKy.KichThuoc
                            };
                            _taiLieuDinhKemService.LuuTaiLieuAsync(noiTruHoSoKhacFileDinhKem.DuongDan, noiTruHoSoKhacFileDinhKem.TenGuid);
                            update.NoiTruHoSoKhacFileDinhKems.Add(noiTruHoSoKhacFileDinhKem);
                        }
                    }

                }
                await  _noiDuTruHoSoKhacService.UpdateAsync(update);
                return Ok(update.Id);
            }
            return null;
        }
        #endregion
        [HttpPost("GetThongTinBenhNhanPtVephongDieuTriViewDS")]
        public BangKiemAnToanNguoiBenhPTTuPhongDieuTriGrid GetThongTinBenhNhanPtVephongDieuTriViewDS(long noiTruHoSoKhacId)
        {
            var lookup = _dieuTriNoiTruService.GetThongTinBenhNhanPtVephongDieuTriViewDS(noiTruHoSoKhacId);
            return lookup;
        }
        #region xóa 
        [HttpPost("xoaBangKiemAnToanPhauThuatTuPhongDieuTri")]
        public async Task<ActionResult> DeleteBangKiemAnToanPhauThuatTuPhongDieuTrin(long id)
        {
            var soket = await _noiDuTruHoSoKhacService.GetByIdAsync(id, s => s.Include(k => k.NoiTruHoSoKhacFileDinhKems));
            if (soket == null)
            {
                return NotFound();
            }
            await _noiDuTruHoSoKhacService.DeleteByIdAsync(id);
            return NoContent();
        }
        #endregion
        #region In 
        [HttpPost("InBangKiemAnToanNguoiBenhPTVeKhoaDieuTri")]
        public async Task<ActionResult<string>> InBangKiemAnToanNguoiBenhPTVeKhoaDieuTri([FromBody]XacNhanInTrichBienBanHoiChan xacNhanInTrichBienBanHoiChan)
        {
            var html = await _dieuTriNoiTruService.InBangKiemAnToanNguoiBenhPTVeKhoaDieuTri(xacNhanInTrichBienBanHoiChan);
            return html;
        }
        #endregion
    }
}
