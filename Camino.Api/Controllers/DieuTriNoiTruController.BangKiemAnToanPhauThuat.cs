using System;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Models.DieuTriNoiTru;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Camino.Api.Controllers
{
    public partial class DieuTriNoiTruController
    {
        [HttpGet("GetBangKiemAnToanPhauThuat")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<BangKiemAnToanPhauThuatViewModel> GetBangKiemAnToanPhauThuat(long yeuCauTiepNhanId)
        {
            var bangKiemAnToan = await _noiTruHoSoKhacService.GetNoiTruHoSoKhac(yeuCauTiepNhanId, Enums.LoaiHoSoDieuTriNoiTru.BangKiemAnToanPhauThuat);

            if (bangKiemAnToan != null)
            {
                var listNoiTruHoSoKhacFileDinhKem =
                    await _noiDuTruHoSoKhacFileDinhKemService.GetListFileDinhKem(bangKiemAnToan.Id);
                var bangKiem = JsonConvert.DeserializeObject<BangKiemAnToanPhauThuatViewModel>(bangKiemAnToan.ThongTinHoSo);
                bangKiem.IdNoiTruHoSo = bangKiemAnToan.Id;

                foreach (var fileChuKyEntity in listNoiTruHoSoKhacFileDinhKem)
                {
                    if (bangKiem.FileChuKy != null && bangKiem.FileChuKy.Any(w => w.TenGuid == fileChuKyEntity.TenGuid))
                    {
                        bangKiem.FileChuKy.First(w => w.TenGuid == fileChuKyEntity.TenGuid).Id =
                            fileChuKyEntity.Id;
                        bangKiem.FileChuKy.First(w => w.TenGuid == fileChuKyEntity.TenGuid).Uid =
                            fileChuKyEntity.Uid;
                    }
                }

                return bangKiem;
            }

            var bangKiemPhauThuat = new BangKiemAnToanPhauThuatViewModel();
            bangKiemPhauThuat.BsCheckHoTen = true;
            bangKiemPhauThuat.DdCheckHoTen = true;
            bangKiemPhauThuat.XnKhangSinhDuPhong = 1;
            bangKiemPhauThuat.XnVtRachDa = true;
            bangKiemPhauThuat.XnChamSocSauMo = true;
            bangKiemPhauThuat.XnNhanDan = true;
            bangKiemPhauThuat.CheckGioiThieuEkip = true;
            bangKiemPhauThuat.DdDemDungCu = true;
            bangKiemPhauThuat.XnMo = true;
            bangKiemPhauThuat.XnDu = true;
            bangKiemPhauThuat.XNLaiThongTinNguoibenh = true;
            bangKiemPhauThuat.XnMonitor = true; 
            bangKiemPhauThuat.XnTienSuDiUng = true; 
            bangKiemPhauThuat.XnKhoTho = true; 
            bangKiemPhauThuat.XnMatMau = true;
            bangKiemPhauThuat.XnDoiChieuHa = true;
            bangKiemPhauThuat.PtvXnChuY = true;
            bangKiemPhauThuat.BsXnChuY = true;
            bangKiemPhauThuat.DdXnChuY = true;
            bangKiemPhauThuat.DanhDauVungMo = 1;

            // BVHD-3871
            bangKiemPhauThuat.DanNhanBenhPham = 1;
            bangKiemPhauThuat.ThuocvaThietBiGayMeCoDayDuKhong = true;
            bangKiemPhauThuat.DanLuu = true;
            bangKiemPhauThuat.XacNhanDieuCanChuYTrongPT = true;
            bangKiemPhauThuat.XacNhanDieuCanChuYVeHoiTinhVaChamSocSauMo = true;
            bangKiemPhauThuat.NguoiBenhCoTienSuDiUng = true;
            bangKiemPhauThuat.BSGayMeCanChuYTrongGayMe = true;
            bangKiemPhauThuat.KimGacDungCu = true;
            bangKiemPhauThuat.DatPlaQueDaoDien = 1;

            var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
            bangKiemPhauThuat.ChanDoan = await _noiTruHoSoKhacService.GetChanDoan(yeuCauTiepNhanId);
            bangKiemPhauThuat.NgayThucHienReadonly = DateTime.Now.ApplyFormatDateTime();
            bangKiemPhauThuat.NguoiThucHienReadonly = await _noiTruHoSoKhacService.GetNguoiThucHien(nguoiDangLogin);
            return bangKiemPhauThuat;
        }

        [HttpPost("UpdateBangKiemAnToanPhauThuat")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> UpdateBangKiemAnToanPhauThuat([FromBody] BangKiemAnToanPhauThuatViewModel bangKiemAnToanPhauThuat, long yeuCauTiepNhanId)
        {
            var nhanVienThucHienId = _userAgentHelper.GetCurrentUserId();
            var noiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            bangKiemAnToanPhauThuat.NgayThucHienReadonly = DateTime.Now.ApplyFormatDateTime();
            bangKiemAnToanPhauThuat.NguoiThucHienReadonly = await _noiTruHoSoKhacService.GetNguoiThucHien(nhanVienThucHienId);
            var thongTinHoSo = JsonConvert.SerializeObject(bangKiemAnToanPhauThuat);
            var yctnEntity = await _dieuTriNoiTruService.GetByIdAsync(yeuCauTiepNhanId);

            if (bangKiemAnToanPhauThuat.IdNoiTruHoSo != null)
            {
                var noiTruHoSoKhac = await _noiTruHoSoKhacService.GetByIdAsync(bangKiemAnToanPhauThuat.IdNoiTruHoSo.GetValueOrDefault(),
                    w => w.Include(q => q.NoiTruHoSoKhacFileDinhKems));
                noiTruHoSoKhac.ThongTinHoSo = thongTinHoSo;
                noiTruHoSoKhac.LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.BangKiemAnToanPhauThuat;
                noiTruHoSoKhac.NhanVienThucHienId = nhanVienThucHienId;
                noiTruHoSoKhac.ThoiDiemThucHien = DateTime.Now;
                noiTruHoSoKhac.NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
                if (bangKiemAnToanPhauThuat.FileChuKy != null && bangKiemAnToanPhauThuat.FileChuKy.Any())
                {
                    foreach (var file in bangKiemAnToanPhauThuat.FileChuKy)
                    {
                        if (noiTruHoSoKhac.NoiTruHoSoKhacFileDinhKems.All(w => w.TenGuid != file.TenGuid))
                        {
                            var noiTruHoSoKhacFileChuKy = new NoiTruHoSoKhacFileDinhKem
                            {
                                Id = 0,
                                Ten = file.Ten,
                                DuongDan = file.DuongDan,
                                Ma = file.Uid,
                                KichThuoc = file.KichThuoc,
                                TenGuid = file.TenGuid,
                                NoiTruHoSoKhacId = noiTruHoSoKhac.Id,
                                LoaiTapTin = file.LoaiTapTin.GetValueOrDefault()
                            };
                            noiTruHoSoKhac.NoiTruHoSoKhacFileDinhKems.Add(noiTruHoSoKhacFileChuKy);
                            await _taiLieuDinhKemService.LuuTaiLieuAsync(noiTruHoSoKhacFileChuKy.DuongDan, noiTruHoSoKhacFileChuKy.TenGuid);
                        }
                    }
                }
                if (noiTruHoSoKhac.NoiTruHoSoKhacFileDinhKems != null && noiTruHoSoKhac.NoiTruHoSoKhacFileDinhKems.Any())
                {
                    foreach (var noiTruHoSoKhacKemFileEntity in noiTruHoSoKhac.NoiTruHoSoKhacFileDinhKems)
                    {
                        if (bangKiemAnToanPhauThuat.FileChuKy != null && bangKiemAnToanPhauThuat.FileChuKy.All(w => w.TenGuid != noiTruHoSoKhacKemFileEntity.TenGuid))
                        {
                            noiTruHoSoKhacKemFileEntity.WillDelete = true;
                        }
                    }
                }
                await _tiepNhanBenhNhanService.PrepareForEditYeuCauTiepNhanAndUpdateAsync(yctnEntity);
                await _noiTruHoSoKhacService.UpdateAsync(noiTruHoSoKhac);
                return Ok(bangKiemAnToanPhauThuat);
            }

            var noiTruHoSoKhacNew = new NoiTruHoSoKhac
            {
                Id = 0,
                LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.BangKiemAnToanPhauThuat,
                NhanVienThucHienId = nhanVienThucHienId,
                NoiThucHienId = noiThucHienId,
                ThoiDiemThucHien = DateTime.Now,
                YeuCauTiepNhanId = yeuCauTiepNhanId,
                ThongTinHoSo = thongTinHoSo
            };

            if (bangKiemAnToanPhauThuat.FileChuKy.Any())
            {
                foreach (var file in bangKiemAnToanPhauThuat.FileChuKy)
                {
                    var noiTruHoSoKhacFileChuKy = new NoiTruHoSoKhacFileDinhKem
                    {
                        Id = 0,
                        Ma = file.Uid,
                        Ten = file.Ten,
                        DuongDan = file.DuongDan,
                        KichThuoc = file.KichThuoc,
                        TenGuid = file.TenGuid,
                        NoiTruHoSoKhacId = noiTruHoSoKhacNew.Id,
                        LoaiTapTin = file.LoaiTapTin.GetValueOrDefault()
                    };
                    noiTruHoSoKhacNew.NoiTruHoSoKhacFileDinhKems.Add(noiTruHoSoKhacFileChuKy);
                    await _taiLieuDinhKemService.LuuTaiLieuAsync(noiTruHoSoKhacFileChuKy.DuongDan, noiTruHoSoKhacFileChuKy.TenGuid);
                }
            }

            await _tiepNhanBenhNhanService.PrepareForEditYeuCauTiepNhanAndUpdateAsync(yctnEntity);
            await _noiTruHoSoKhacService.AddAsync(noiTruHoSoKhacNew);
            bangKiemAnToanPhauThuat.IdNoiTruHoSo = noiTruHoSoKhacNew.Id;
            return Ok(bangKiemAnToanPhauThuat);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("PhieuInBangKiemAnToanPhauThuat")]
        public async Task<ActionResult> PhieuInBangKiemAnToanPhauThuat(PhieuDieuTriVaServicesHttpParams dieuTriNoiTruVaServicesHttpParams)
        {
            var phieuIns = await _noiTruHoSoKhacService.PhieuInBangKiemAnToanPhauThuat(dieuTriNoiTruVaServicesHttpParams);
            return Ok(phieuIns);
        }
    }
}
