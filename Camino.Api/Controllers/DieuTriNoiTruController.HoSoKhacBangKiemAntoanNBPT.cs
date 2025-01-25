using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.BangKiemAnToanNBPT;
using Camino.Api.Models.Error;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.Entities.KetQuaSinhHieus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BangKiemAnToanNBPT;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.TrichBienBanHoiChan;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Camino.Api.Controllers
{
    public partial class DieuTriNoiTruController
    {
        [HttpPost("GetThuocTienMeVaNhanVienVaChiSoSinhTon")]
        public ThuocTienMeVaNhanVienGrid GetThuocTienMeVaNhanVienVaChiSoSinhTon([FromBody]QueryInfo queryInfo)
        {
            var lookup = _dieuTriNoiTruService.GetThuocTienMeVaNhanVienVaChiSoSinhTon(queryInfo);
            return lookup;
        }
        [HttpPost("GetThongTinBangKiemATNBPT")]
        public BangKiemAnToanNBPTGridVo GetThongTinBangKiemATNBPT(long yeuCauTiepNhanId)
        {
            var lookup = _dieuTriNoiTruService.GetThongTinBangKiemATNBPT(yeuCauTiepNhanId);
            return lookup;
        }
        [HttpPost("GetDanhSachBangKiemAnToanNBPT")]
        public async Task<ActionResult<GridDataSource>> GetDanhSachBangKiemAnToanNBPT([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _dieuTriNoiTruService.GetDanhSachBangKiemAnToanNBPT(queryInfo);
            return Ok(gridData);
        }
        [HttpPost("GetDanhSachBangKiemAnToanNBPTSave")]
        public BangKiemAnToanNBPTGridVo GetDanhSachBangKiemAnToanNBPTSave(long yeuCauTiepNhanId)
        {
            var gridData =  _dieuTriNoiTruService.GetDanhSachBangKiemAnToanNBPTSave(yeuCauTiepNhanId);
            return gridData;
        }
        [HttpPost("LuuBangKiemAnToanNBPT")]
        public async Task<ActionResult<BangKiemAnToanNBPTViewModel>> LuuBangKiemAnToanNBPT([FromBody] BangKiemAnToanNBPTViewModel bangKiemAnToanNBPTViewModel)
        {
            if (bangKiemAnToanNBPTViewModel.Id == 0)
            {
                if (bangKiemAnToanNBPTViewModel.NgayGioDuDinhGayMe < bangKiemAnToanNBPTViewModel.NgayGioDuaBNDiPT)
                {
                    throw new ApiException(_localizationService.GetResource("DieuTriNoiTru.ThoiGianGayMe.notValue"), (int)HttpStatusCode.BadRequest);
                }
                var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
                var noiThucHien = _userAgentHelper.GetCurrentNoiLLamViecId();
                var yeuCauTiepNhanEntiTy = _yeuCauTiepNhanService.GetByIdAsync(bangKiemAnToanNBPTViewModel.YeuCauTiepNhanId, s => s.Include(x => x.KetQuaSinhHieus));
                var kiemAntoanNguoiBenhPTModel = new BangKiemAnToanNBPTViewModel()
                {
                    
                    YeuCauTiepNhanId = bangKiemAnToanNBPTViewModel.YeuCauTiepNhanId,
                    LoaiHoSoDieuTriNoiTru = bangKiemAnToanNBPTViewModel.LoaiHoSoDieuTriNoiTru,
                    ThongTinHoSo = bangKiemAnToanNBPTViewModel.ThongTinHoSo,
                    NhanVienThucHienId = nguoiDangLogin,
                    NoiThucHienId = noiThucHien,
                    ThoiDiemThucHien = DateTime.Now
                };
                var kiemAntoanNguoiBenhPT = kiemAntoanNguoiBenhPTModel.ToEntity<NoiTruHoSoKhac>();
                    kiemAntoanNguoiBenhPT.YeuCauTiepNhan = yeuCauTiepNhanEntiTy.Result;

                CreatedAtAction(nameof(Get), new { id = kiemAntoanNguoiBenhPT.Id }, kiemAntoanNguoiBenhPT.ToModel<BangKiemAnToanNBPTViewModel>());
                if (bangKiemAnToanNBPTViewModel.FileChuKy != null)
                {
                    foreach (var itemfileChuKy in bangKiemAnToanNBPTViewModel.FileChuKy)
                    {
                        var noiTruHoSoKhacFileDinhKem = new NoiTruHoSoKhacFileDinhKem()
                        {
                            //NoiTruHoSoKhacId = user.Id,
                            Ma = itemfileChuKy.Ma,
                            Ten = itemfileChuKy.Ten,
                            TenGuid = itemfileChuKy.TenGuid,
                            DuongDan = itemfileChuKy.DuongDan,
                            LoaiTapTin = itemfileChuKy.LoaiTapTin,
                            MoTa = itemfileChuKy.MoTa,
                            KichThuoc = itemfileChuKy.size
                        };
                        _taiLieuDinhKemService.LuuTaiLieuAsync(noiTruHoSoKhacFileDinhKem.DuongDan, noiTruHoSoKhacFileDinhKem.TenGuid);
                        kiemAntoanNguoiBenhPT.NoiTruHoSoKhacFileDinhKems.Add(noiTruHoSoKhacFileDinhKem);
                    }
                }
                if (bangKiemAnToanNBPTViewModel.ListChiSoSinhTon.Count() > 0)
                {
                    var queryIdUpdate = bangKiemAnToanNBPTViewModel.ListChiSoSinhTon.Where(x => x.Id != 0).Select(x=>x.Id).ToList(); // sau khi sửa
                    var queryChiSoSinhHieu = _dieuTriNoiTruService.GetListChiSoSinhHieu(bangKiemAnToanNBPTViewModel.YeuCauTiepNhanId); // list id hiện có
                    var listId = queryChiSoSinhHieu.Where(s => !queryIdUpdate.Any(a => a == s)).ToList(); // là list id khoog tồn tại queryIdUpdate ==> xoas no
                    foreach (var deleteCSSH in listId)
                    {
                        var s = await _ketQuaSinhHieuService.GetByIdAsync(deleteCSSH);
                        if (s == null)
                        {
                            return NotFound();
                        }
                        await _ketQuaSinhHieuService.DeleteByIdAsync(deleteCSSH);
                    }
                   
                    foreach (var itemSinhTon in bangKiemAnToanNBPTViewModel.ListChiSoSinhTon)
                    {
                        if(itemSinhTon.Id == 0)
                        {
                            var chiSoSinhHieu = new KetQuaSinhHieu()
                            {
                                //YeuCauTiepNhanId = bangKiemAnToanNBPTViewModel.YeuCauTiepNhanId,
                                NhipTim =  itemSinhTon.NhipTim,
                                NhipTho = itemSinhTon.NhipTho,
                                ThanNhiet = itemSinhTon.ThanNhiet,
                                HuyetApTamThu = itemSinhTon.HuyetApTamThu,
                                HuyetApTamTruong = itemSinhTon.HuyetApTamTruong,
                                ChieuCao = itemSinhTon.ChieuCao,
                                CanNang = itemSinhTon.CanNang,
                                Bmi = itemSinhTon.BMI,
                                Glassgow = itemSinhTon.Glassgow,
                                SpO2 = itemSinhTon.SpO2,
                                ThoiDiemThucHien = DateTime.Now,
                                NoiThucHienId = noiThucHien,
                                NhanVienThucHienId = nguoiDangLogin,
                            };
                            kiemAntoanNguoiBenhPT.YeuCauTiepNhan.KetQuaSinhHieus.Add(chiSoSinhHieu);
                        }
                       
                    }
                }
                await _noiDuTruHoSoKhacService.AddAsync(kiemAntoanNguoiBenhPT);
                return Ok(kiemAntoanNguoiBenhPT.Id);
            }
            else
            {
                var update = await _noiDuTruHoSoKhacService.GetByIdAsync(bangKiemAnToanNBPTViewModel.Id, s => s.Include(k => k.NoiTruHoSoKhacFileDinhKems).Include(j=>j.YeuCauTiepNhan).ThenInclude(p=>p.KetQuaSinhHieus));
                var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
                var noiThucHien = _userAgentHelper.GetCurrentNoiLLamViecId();
                if (bangKiemAnToanNBPTViewModel.NgayGioDuDinhGayMe < bangKiemAnToanNBPTViewModel.NgayGioDuaBNDiPT)
                {
                    throw new ApiException(_localizationService.GetResource("DieuTriNoiTru.ThoiGianGayMe.notValue"), (int)HttpStatusCode.BadRequest);
                }
                update.YeuCauTiepNhanId = bangKiemAnToanNBPTViewModel.YeuCauTiepNhanId;
                update.LoaiHoSoDieuTriNoiTru = bangKiemAnToanNBPTViewModel.LoaiHoSoDieuTriNoiTru;
                update.ThongTinHoSo = bangKiemAnToanNBPTViewModel.ThongTinHoSo;
                update.NhanVienThucHienId = nguoiDangLogin;
                update.NoiThucHienId = noiThucHien;
                if(update.ThoiDiemThucHien == null)
                {
                    update.ThoiDiemThucHien = DateTime.Now;
                }
                
                if (update == null)
                {
                    return NotFound();
                }
                bangKiemAnToanNBPTViewModel.ToEntity<NoiTruHoSoKhac>();

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
                if (bangKiemAnToanNBPTViewModel.Id != 0)
                {
                    if (bangKiemAnToanNBPTViewModel.FileChuKy.Count() > 0)
                    {
                        foreach (var itemfileChuKy in bangKiemAnToanNBPTViewModel.FileChuKy)
                        {
                            var noiTruHoSoKhacFileDinhKem = new NoiTruHoSoKhacFileDinhKem()
                            {
                                Ma = itemfileChuKy.Ma,
                                Ten = itemfileChuKy.Ten,
                                TenGuid = itemfileChuKy.TenGuid,
                                DuongDan = itemfileChuKy.DuongDan,
                                LoaiTapTin = itemfileChuKy.LoaiTapTin,
                                MoTa = itemfileChuKy.MoTa,
                                KichThuoc = itemfileChuKy.size
                            };

                            _taiLieuDinhKemService.LuuTaiLieuAsync(noiTruHoSoKhacFileDinhKem.DuongDan, noiTruHoSoKhacFileDinhKem.TenGuid);
                            update.NoiTruHoSoKhacFileDinhKems.Add(noiTruHoSoKhacFileDinhKem);
                        }
                    }

                }
                if (bangKiemAnToanNBPTViewModel.ListChiSoSinhTon.Count() > 0)
                {
                    var queryIdUpdate = bangKiemAnToanNBPTViewModel.ListChiSoSinhTon.Where(x => x.Id != 0).Select(x => x.Id).ToList(); // sau khi sửa
                    var queryChiSoSinhHieu = _dieuTriNoiTruService.GetListChiSoSinhHieu(bangKiemAnToanNBPTViewModel.YeuCauTiepNhanId); // list id hiện có
                    var listId = queryChiSoSinhHieu.Where(s => !queryIdUpdate.Any(a => a == s)).ToList(); // là list id khoog tồn tại queryIdUpdate ==> xoas no
                    foreach (var deleteCSSH in listId)
                    {
                        var s = await _ketQuaSinhHieuService.GetByIdAsync(deleteCSSH);
                        if (s == null)
                        {
                            return NotFound();
                        }
                        await _ketQuaSinhHieuService.DeleteByIdAsync(deleteCSSH);
                    }

                    foreach (var itemSinhTon in bangKiemAnToanNBPTViewModel.ListChiSoSinhTon)
                    {
                        if (itemSinhTon.Id == 0)
                        {
                            var chiSoSinhHieu = new KetQuaSinhHieu()
                            {
                                YeuCauTiepNhanId = bangKiemAnToanNBPTViewModel.YeuCauTiepNhanId,
                                NhipTim = itemSinhTon.NhipTim,
                                NhipTho = itemSinhTon.NhipTho,
                                ThanNhiet = itemSinhTon.ThanNhiet,
                                HuyetApTamThu = itemSinhTon.HuyetApTamThu,
                                HuyetApTamTruong = itemSinhTon.HuyetApTamTruong,
                                ChieuCao = itemSinhTon.ChieuCao,
                                CanNang = itemSinhTon.CanNang,
                                Bmi = itemSinhTon.BMI,
                                Glassgow = itemSinhTon.Glassgow,
                                SpO2 = itemSinhTon.SpO2,
                                ThoiDiemThucHien = DateTime.Now,
                                NoiThucHienId = noiThucHien,
                                NhanVienThucHienId = nguoiDangLogin,
                            };
                            update.YeuCauTiepNhan.KetQuaSinhHieus.Add(chiSoSinhHieu);
                        }

                    }
                }
                _noiDuTruHoSoKhacService.Update(update);
                return Ok(update.Id);
            }
            return null;
        }
        [HttpPost("GetThongTinBangKiemAnToanNBPTViewDS")]
        public BangKiemAnToanNBPTGridVo GetThongTinBangKiemAnToanNBPTViewDS(long noiTruHoSoKhacId)
        {
            var lookup = _dieuTriNoiTruService.GetThongTinBangKiemAnToanNBPTViewDS(noiTruHoSoKhacId);
            return lookup;
        }
        #region xoaBangKiemAnToanNguoiBenh 
        [HttpPost("xoaBangKiemAnToanNguoiBenh")]
        public async Task<ActionResult> xoaBangKiemAnToanNguoiBenh(long id)
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
        [HttpPost("GetThongTinCreate")]
        public NhanVienNgayThucHien GetThongTinCreate(long idNguoiLogin, long yeuCauTiepNhanId)
        {
            var lookup = _dieuTriNoiTruService.GetThongTinCreate(idNguoiLogin, yeuCauTiepNhanId);
            return lookup;
        }
        #region In 
        [HttpPost("InBangKiemAnToanNBPT")]
        public async Task<ActionResult<string>> InBangKiemAnToanNBPT([FromBody]XacNhanInTrichBienBanHoiChan xacNhanInTrichBienBanHoiChan)
        {
            var html = await _dieuTriNoiTruService.BangKiemAnToanNBPT(xacNhanInTrichBienBanHoiChan);
            return html;
        }
        #endregion
    }
}
