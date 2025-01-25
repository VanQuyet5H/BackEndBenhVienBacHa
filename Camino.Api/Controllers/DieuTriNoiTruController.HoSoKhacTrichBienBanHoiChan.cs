using Camino.Api.Extensions;
using Camino.Api.Models.TrichBienBanHoiChan;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.ValueObject;
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

        [HttpPost("LuuTrichBienBanHoiChan")]
        //[ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.TrichBienBanHoiChan)]
        public async Task<ActionResult<TrichBienBanHoiChanViewModel>> LuuTrichBienBanHoiChan([FromBody] TrichBienBanHoiChanViewModel trichBienBanHoiChanViewModel)
        {
            if(trichBienBanHoiChanViewModel.Id == 0)
            {
                    var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
                    var noiThucHien = _userAgentHelper.GetCurrentNoiLLamViecId();
                    var trichBienBanNoiTru = new TrichBienBanHoiChanViewModel()
                    {
                        YeuCauTiepNhanId = trichBienBanHoiChanViewModel.YeuCauTiepNhanId,
                        LoaiHoSoDieuTriNoiTru = trichBienBanHoiChanViewModel.LoaiHoSoDieuTriNoiTru,
                        ThongTinHoSo = trichBienBanHoiChanViewModel.ThongTinHoSo,
                        NhanVienThucHienId = nguoiDangLogin,
                        NoiThucHienId = noiThucHien,
                        ThoiDiemThucHien = DateTime.Now
                    };
                    var user = trichBienBanNoiTru.ToEntity<NoiTruHoSoKhac>();
                if (trichBienBanHoiChanViewModel.FileChuKy.Any())
                {
                    foreach (var itemfileChuKy in trichBienBanHoiChanViewModel.FileChuKy)
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
                await _noiDuTruHoSoKhacService.AddAsync(user);
                return CreatedAtAction(nameof(Get), new { id = user.Id }, user.ToModel<TrichBienBanHoiChanViewModel>());
            }
            else
            {
                var update = await _noiDuTruHoSoKhacService.GetByIdAsync(trichBienBanHoiChanViewModel.Id);
                var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
                var noiThucHien = _userAgentHelper.GetCurrentNoiLLamViecId();
                update.YeuCauTiepNhanId = trichBienBanHoiChanViewModel.YeuCauTiepNhanId;
                update.LoaiHoSoDieuTriNoiTru = trichBienBanHoiChanViewModel.LoaiHoSoDieuTriNoiTru;
                update.ThongTinHoSo = trichBienBanHoiChanViewModel.ThongTinHoSo;
                update.NhanVienThucHienId = nguoiDangLogin;
                update.NoiThucHienId = noiThucHien;
                update.ThoiDiemThucHien = DateTime.Now;
                if (update == null)
                {
                    return NotFound();
                }
                trichBienBanHoiChanViewModel.ToEntity<NoiTruHoSoKhac>();
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
                if (trichBienBanHoiChanViewModel.Id != 0)
                {
                    if (trichBienBanHoiChanViewModel.FileChuKy.Any())
                    {
                        foreach (var itemfileChuKy in trichBienBanHoiChanViewModel.FileChuKy)
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
                await _noiDuTruHoSoKhacService.UpdateAsync(update);
                return Ok(update.Id);
            }
            return null;
        }
        [HttpPost("GetChuToa")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetChuToa([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _dieuTriNoiTruService.GetListTenNhanVienmAsync();
            return Ok(lookup);
        }
        [HttpPost("GetThuKy")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetThuKy([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _dieuTriNoiTruService.GetThuKy(queryInfo);
            return Ok(lookup);
        }
        [HttpPost("GetThanhVienThamGia")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetThanhVienThamGia([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _dieuTriNoiTruService.GetThanhVienThamGia(queryInfo);
            return Ok(lookup);
        }
        [HttpPost("GetThongTinTrichBienBanHoiChan")]
        public TrichBienBanHoiChanGridVo GetThongTinTrichBienBanHoiChan(long yeuCauTiepNhanId)
        {
            var lookup =  _dieuTriNoiTruService.GetThongTinTrichBienBanHoiChan(yeuCauTiepNhanId);
            return lookup;
        }
        [HttpPost("GetTenNguoiThucHien")]
        public NhanVienNgayThucHien GetTenNguoiThucHien(long idNguoiLogin,long yeuCauTiepNhanId)
        {
            var lookup = _dieuTriNoiTruService.GetTenNguoiThucHien(idNguoiLogin, yeuCauTiepNhanId);
            return lookup;
        }
        [HttpPost("GetDanhSachTrichBienBanHoiChan")]
        public async Task<ActionResult<GridDataSource>> GetDanhSachTrichBienBanHoiChan([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _dieuTriNoiTruService.GetDanhSachBienBanHoiChan(queryInfo);
            return Ok(gridData);
        }
        [HttpPost("GetThongTinTrichBienBanHoiChanViewDS")]
        public TrichBienBanHoiChanGridVo GetThongTinTrichBienBanHoiChanViewDS(long noiTruHoSoKhacId)
        {
            var lookup = _dieuTriNoiTruService.GetThongTinTrichBienBanHoiChanViewDS(noiTruHoSoKhacId);
            return lookup;
        }
        #region xóa sơ kết
        [HttpPost("xoaBienBan")]
        public async Task<ActionResult> DeleteBienBan(long id)
        {
            var soket = await _noiDuTruHoSoKhacService.GetByIdAsync(id,s=>s.Include(k=>k.NoiTruHoSoKhacFileDinhKems));
            if (soket == null)
            {
                return NotFound();
            }
            await _noiDuTruHoSoKhacService.DeleteByIdAsync(id);
            return NoContent();
        }
        #endregion
        #region In 
        [HttpPost("InTrichBienBanHoiChan")]
        public async Task<ActionResult<string>> InTrichBienBanHoiChan([FromBody]XacNhanInTrichBienBanHoiChan xacNhanInTrichBienBanHoiChan)
        {
            var html = await _dieuTriNoiTruService.BienBanHoiChan(xacNhanInTrichBienBanHoiChan);
            return html;
        }
        #endregion
    }
}
