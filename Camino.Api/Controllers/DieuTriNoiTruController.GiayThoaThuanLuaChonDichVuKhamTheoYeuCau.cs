using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Models.DieuTriNoiTru;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Camino.Api.Controllers
{
    public partial class DieuTriNoiTruController
    {
        [HttpGet("GetGiayThoaThuanLuaChonDichVuKhamTheoYeuCau")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<GiayThoaThuanLuaChonDichVuKhamTheoYeuCauViewModel> GiayThoaThuanLuaChonDichVuKhamTheoYeuCau(long yeuCauTiepNhanId)
        {
            var getGiayThoaThuanLuaChonDichVuKhamTheoYeuCauEntity = await _noiTruHoSoKhacService.GetNoiTruHoSoKhac(yeuCauTiepNhanId, Enums.LoaiHoSoDieuTriNoiTru.GiayThoaThuanLuaChonDichVuKhamChuabenhTheoYeuCau); 

            if (getGiayThoaThuanLuaChonDichVuKhamTheoYeuCauEntity != null)
            {
                var listNoiTruHoSoKhacFileDinhKem =
                    await _noiDuTruHoSoKhacFileDinhKemService.GetListFileDinhKem(getGiayThoaThuanLuaChonDichVuKhamTheoYeuCauEntity.Id);
                var giayCamKet = JsonConvert.DeserializeObject<GiayThoaThuanLuaChonDichVuKhamTheoYeuCauViewModel>(getGiayThoaThuanLuaChonDichVuKhamTheoYeuCauEntity.ThongTinHoSo);
                giayCamKet.IdNoiTruHoSo = getGiayThoaThuanLuaChonDichVuKhamTheoYeuCauEntity.Id;

                foreach (var fileChuKyEntity in listNoiTruHoSoKhacFileDinhKem)
                {
                    if (giayCamKet.FileChuKy != null && giayCamKet.FileChuKy.Any(w => w.TenGuid == fileChuKyEntity.TenGuid))
                    {
                        giayCamKet.FileChuKy.First(w => w.TenGuid == fileChuKyEntity.TenGuid).Id =
                            fileChuKyEntity.Id;
                        giayCamKet.FileChuKy.First(w => w.TenGuid == fileChuKyEntity.TenGuid).Uid =
                            fileChuKyEntity.Uid;
                    }
                }

                return giayCamKet;
            }

            var giayCamKetKtMoi = new GiayThoaThuanLuaChonDichVuKhamTheoYeuCauViewModel();
            var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
            giayCamKetKtMoi.KhiCanBaoTin =""; // để tự nhập
            giayCamKetKtMoi.NamTaiBuongLoai = ""; // để tự nhập
            giayCamKetKtMoi.BacSiKham = ""; // để tự nhập
            giayCamKetKtMoi.NgayThucHienReadonly = DateTime.Now.ApplyFormatDateTime();
            giayCamKetKtMoi.NguoiThucHienReadonly = await _noiTruHoSoKhacService.GetNguoiThucHien(nguoiDangLogin);
            return giayCamKetKtMoi;
        }

        [HttpPost("UpdateGiayThoaThuanLuaChonDichVuKhamTheoYeuCau")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> UpdateGiayThoaThuanLuaChonDichVuKhamTheoYeuCau([FromBody] GiayThoaThuanLuaChonDichVuKhamTheoYeuCauViewModel giayThoaThuanLuaChonDichVuKhamTheoYeuCauViewModel, long yeuCauTiepNhanId)
        {
            var nhanVienThucHienId = _userAgentHelper.GetCurrentUserId();
            var noiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            giayThoaThuanLuaChonDichVuKhamTheoYeuCauViewModel.NgayThucHienReadonly = DateTime.Now.ApplyFormatDateTime();
            giayThoaThuanLuaChonDichVuKhamTheoYeuCauViewModel.NguoiThucHienReadonly = await _noiTruHoSoKhacService.GetNguoiThucHien(nhanVienThucHienId);
            var thongTinHoSo = JsonConvert.SerializeObject(giayThoaThuanLuaChonDichVuKhamTheoYeuCauViewModel);
            var yctnEntity = await _dieuTriNoiTruService.GetByIdAsync(yeuCauTiepNhanId);

            if (giayThoaThuanLuaChonDichVuKhamTheoYeuCauViewModel.IdNoiTruHoSo != null)
            {
                var noiTruHoSoKhac = await _noiTruHoSoKhacService.GetByIdAsync(giayThoaThuanLuaChonDichVuKhamTheoYeuCauViewModel.IdNoiTruHoSo.GetValueOrDefault(),
                    w => w.Include(q => q.NoiTruHoSoKhacFileDinhKems));
                noiTruHoSoKhac.ThongTinHoSo = thongTinHoSo;
                noiTruHoSoKhac.LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.GiayThoaThuanLuaChonDichVuKhamChuabenhTheoYeuCau;
                noiTruHoSoKhac.NhanVienThucHienId = nhanVienThucHienId;
                noiTruHoSoKhac.ThoiDiemThucHien = DateTime.Now;
                noiTruHoSoKhac.NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
                if (giayThoaThuanLuaChonDichVuKhamTheoYeuCauViewModel.FileChuKy != null && giayThoaThuanLuaChonDichVuKhamTheoYeuCauViewModel.FileChuKy.Any())
                {
                    foreach (var file in giayThoaThuanLuaChonDichVuKhamTheoYeuCauViewModel.FileChuKy)
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
                        if (giayThoaThuanLuaChonDichVuKhamTheoYeuCauViewModel.FileChuKy != null && giayThoaThuanLuaChonDichVuKhamTheoYeuCauViewModel.FileChuKy.All(w => w.TenGuid != noiTruHoSoKhacKemFileEntity.TenGuid))
                        {
                            noiTruHoSoKhacKemFileEntity.WillDelete = true;
                        }
                    }
                }
                await _tiepNhanBenhNhanService.PrepareForEditYeuCauTiepNhanAndUpdateAsync(yctnEntity);
                await _noiTruHoSoKhacService.UpdateAsync(noiTruHoSoKhac);
                giayThoaThuanLuaChonDichVuKhamTheoYeuCauViewModel.CheckCreateOrCapNhat = true; // true là cập nhat
                return Ok(giayThoaThuanLuaChonDichVuKhamTheoYeuCauViewModel);
            }

            var noiTruHoSoKhacNew = new NoiTruHoSoKhac
            {
                Id = 0,
                LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.GiayThoaThuanLuaChonDichVuKhamChuabenhTheoYeuCau,
                NhanVienThucHienId = nhanVienThucHienId,
                NoiThucHienId = noiThucHienId,
                ThoiDiemThucHien = DateTime.Now,
                YeuCauTiepNhanId = yeuCauTiepNhanId,
                ThongTinHoSo = thongTinHoSo
            };

            if (giayThoaThuanLuaChonDichVuKhamTheoYeuCauViewModel.FileChuKy.Any())
            {
                foreach (var file in giayThoaThuanLuaChonDichVuKhamTheoYeuCauViewModel.FileChuKy)
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
            giayThoaThuanLuaChonDichVuKhamTheoYeuCauViewModel.IdNoiTruHoSo = noiTruHoSoKhacNew.Id;
            giayThoaThuanLuaChonDichVuKhamTheoYeuCauViewModel.CheckCreateOrCapNhat = false; // false là tao moires
            return Ok(giayThoaThuanLuaChonDichVuKhamTheoYeuCauViewModel);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("PhieuInGiayThoaThuanLuaChonDichVuKhamTheoYeuCau")]
        public async Task<ActionResult> PhieuInGiayThoaThuanLuaChonDichVuKhamTheoYeuCau(PhieuDieuTriVaServicesHttpParams dieuTriNoiTruVaServicesHttpParams)
        {
            var phieuIns = await _noiTruHoSoKhacService.PhieuInGiayThoaThuanLuaChonDichVuKhamTheoYeuCau(dieuTriNoiTruVaServicesHttpParams);
            return Ok(phieuIns);
        }
        [HttpPost("GetListNhanViens")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListNhanViens([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _noiTruHoSoKhacService.GetListNhanViens(queryInfo);
            return Ok(lookup);
        }
    }
}
