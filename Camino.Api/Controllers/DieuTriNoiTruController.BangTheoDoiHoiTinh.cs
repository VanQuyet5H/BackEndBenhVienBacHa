using System;
using System.Collections.Generic;
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
        [HttpPost("UpdateBangTheoDoiHoiTinh")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> UpdateBangTheoDoiHoiTinh([FromBody] BangTheoDoiHoiTinhViewModel bangTheoDoiHoiTinhViewModel, long yeuCauTiepNhanId)
        {
            var nhanVienThucHienId = _userAgentHelper.GetCurrentUserId();
            var noiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            bangTheoDoiHoiTinhViewModel.NgayThucHienReadonly = DateTime.Now.ApplyFormatDateTime();
            bangTheoDoiHoiTinhViewModel.NguoiThucHienReadonly = await _noiTruHoSoKhacService.GetNguoiThucHien(nhanVienThucHienId);
            var thongTinHoSo = JsonConvert.SerializeObject(bangTheoDoiHoiTinhViewModel);
            var yctnEntity = await _dieuTriNoiTruService.GetByIdAsync(yeuCauTiepNhanId);

            if (bangTheoDoiHoiTinhViewModel.IdNoiTruHoSo != null)
            {
                var noiTruHoSoKhac = await _noiTruHoSoKhacService.GetByIdAsync(bangTheoDoiHoiTinhViewModel.IdNoiTruHoSo.GetValueOrDefault(),
                    w => w.Include(q => q.NoiTruHoSoKhacFileDinhKems));
                noiTruHoSoKhac.ThongTinHoSo = thongTinHoSo;
                noiTruHoSoKhac.LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.BangTheoDoiHoiTinh;
                noiTruHoSoKhac.NhanVienThucHienId = nhanVienThucHienId;
                noiTruHoSoKhac.ThoiDiemThucHien = DateTime.Now;
                noiTruHoSoKhac.NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
                if (bangTheoDoiHoiTinhViewModel.FileChuKy != null && bangTheoDoiHoiTinhViewModel.FileChuKy.Any())
                {
                    foreach (var file in bangTheoDoiHoiTinhViewModel.FileChuKy)
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
                        if (bangTheoDoiHoiTinhViewModel.FileChuKy != null && bangTheoDoiHoiTinhViewModel.FileChuKy.All(w => w.TenGuid != noiTruHoSoKhacKemFileEntity.TenGuid))
                        {
                            noiTruHoSoKhacKemFileEntity.WillDelete = true;
                        }
                    }
                }
                await _tiepNhanBenhNhanService.PrepareForEditYeuCauTiepNhanAndUpdateAsync(yctnEntity);
                await _noiTruHoSoKhacService.UpdateAsync(noiTruHoSoKhac);
                bangTheoDoiHoiTinhViewModel.IsSave = false;
                return Ok(bangTheoDoiHoiTinhViewModel);
            }

            var noiTruHoSoKhacNew = new NoiTruHoSoKhac
            {
                Id = 0,
                LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.BangTheoDoiHoiTinh,
                NhanVienThucHienId = nhanVienThucHienId,
                NoiThucHienId = noiThucHienId,
                ThoiDiemThucHien = DateTime.Now,
                YeuCauTiepNhanId = yeuCauTiepNhanId,
                ThongTinHoSo = thongTinHoSo
            };

            if (bangTheoDoiHoiTinhViewModel.FileChuKy.Any())
            {
                foreach (var file in bangTheoDoiHoiTinhViewModel.FileChuKy)
                {
                    var noiTruHoSoKhacFileChuKy = new NoiTruHoSoKhacFileDinhKem
                    {
                        Id = 0,
                        Ten = file.Ten,
                        DuongDan = file.DuongDan,
                        Ma = file.Uid,
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
            bangTheoDoiHoiTinhViewModel.IdNoiTruHoSo = noiTruHoSoKhacNew.Id;
            bangTheoDoiHoiTinhViewModel.IsSave = true;
            return Ok(bangTheoDoiHoiTinhViewModel);
        }

        [HttpGet("GetBangTheoDoiHoiTinh")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<BangTheoDoiHoiTinhViewModel> GetBangTheoDoiHoiTinh(long yeuCauTiepNhanId)
        {
            var noiTruHoSoKhac = await _noiTruHoSoKhacService.GetNoiTruHoSoKhac(yeuCauTiepNhanId, Enums.LoaiHoSoDieuTriNoiTru.BangTheoDoiHoiTinh);

            if (noiTruHoSoKhac != null)
            {
                var listNoiTruHoSoKhacFileDinhKem =
                    await _noiDuTruHoSoKhacFileDinhKemService.GetListFileDinhKem(noiTruHoSoKhac.Id);
                var bangTheoDoi = JsonConvert.DeserializeObject<BangTheoDoiHoiTinhViewModel>(noiTruHoSoKhac.ThongTinHoSo);
                bangTheoDoi.IdNoiTruHoSo = noiTruHoSoKhac.Id;
                var noiTruHoSoConLai = await _noiTruHoSoKhacService.GetListNoiTruHoSoKhacBangTheoDoi(yeuCauTiepNhanId, Enums.LoaiHoSoDieuTriNoiTru.BangTheoDoiHoiTinh);

                bangTheoDoi.ListPhieu = new List<PhieuHoiTinhViewModel>();

                foreach (var fileChuKyEntity in listNoiTruHoSoKhacFileDinhKem)
                {
                    if (bangTheoDoi.FileChuKy != null && bangTheoDoi.FileChuKy.Any(w => w.TenGuid == fileChuKyEntity.TenGuid))
                    {
                        bangTheoDoi.FileChuKy.First(w => w.TenGuid == fileChuKyEntity.TenGuid).Id =
                            fileChuKyEntity.Id;
                        bangTheoDoi.FileChuKy.First(w => w.TenGuid == fileChuKyEntity.TenGuid).Uid =
                            fileChuKyEntity.Uid;
                        bangTheoDoi.FileChuKy.First(w => w.TenGuid == fileChuKyEntity.TenGuid).DuongDan = fileChuKyEntity.DuongDanTmp;
                    }
                }

                foreach (var noiTruHoSo in noiTruHoSoConLai)
                {
                    var bienBanTruHoSo = JsonConvert.DeserializeObject<BangTheoDoiHoiTinhViewModel>(noiTruHoSo.ThongTinHoSo);

                    var phieuHoiTinhViewModel = new PhieuHoiTinhViewModel
                    {
                        Id = noiTruHoSo.Id,
                        NgayThucHienDisplay = bienBanTruHoSo.NgayThucHien?.ApplyFormatDate(),
                        CachMo = bienBanTruHoSo.CachMo,
                        DdNhan = bienBanTruHoSo.DdNhan
                    };
                    bangTheoDoi.ListPhieu.Add(phieuHoiTinhViewModel);
                }

                bangTheoDoi.ThongTinHoSo = noiTruHoSoKhac.ThongTinHoSo;

                return bangTheoDoi;
            }

            var bangTheoDoiHoiTinh = new BangTheoDoiHoiTinhViewModel();
            var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
            bangTheoDoiHoiTinh.NgayThucHienReadonly = DateTime.Now.ApplyFormatDateTime();
            bangTheoDoiHoiTinh.NguoiThucHienReadonly = await _noiTruHoSoKhacService.GetNguoiThucHien(nguoiDangLogin);
            return bangTheoDoiHoiTinh;
        }

        [HttpGet("GetBangTheoDoiCuThe")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<BangTheoDoiHoiTinhViewModel> GetBangTheoDoiCuThe(long phieuId, long yctnId)
        {
            var bangTheoDoiCuThe = await _noiTruHoSoKhacService.GetByIdAsync(phieuId);

            if (bangTheoDoiCuThe != null)
            {
                var listNoiTruHoSoKhacFileDinhKem =
                    await _noiDuTruHoSoKhacFileDinhKemService.GetListFileDinhKem(bangTheoDoiCuThe.Id);
                var bangTheoDoiObj = JsonConvert.DeserializeObject<BangTheoDoiHoiTinhViewModel>(bangTheoDoiCuThe.ThongTinHoSo);
                bangTheoDoiObj.IdNoiTruHoSo = bangTheoDoiCuThe.Id;
                var noiTruHoSoConLai = await _noiTruHoSoKhacService.GetListNoiTruHoSoKhacBangTheoDoi(yctnId, Enums.LoaiHoSoDieuTriNoiTru.BangTheoDoiHoiTinh);

                bangTheoDoiObj.ListPhieu = new List<PhieuHoiTinhViewModel>();

                foreach (var fileChuKyEntity in listNoiTruHoSoKhacFileDinhKem)
                {
                    if (bangTheoDoiObj.FileChuKy != null && bangTheoDoiObj.FileChuKy.Any(w => w.TenGuid == fileChuKyEntity.TenGuid))
                    {
                        bangTheoDoiObj.FileChuKy.First(w => w.TenGuid == fileChuKyEntity.TenGuid).Id =
                            fileChuKyEntity.Id;
                        bangTheoDoiObj.FileChuKy.First(w => w.TenGuid == fileChuKyEntity.TenGuid).Uid =
                            fileChuKyEntity.Uid;
                        bangTheoDoiObj.FileChuKy.First(w => w.TenGuid == fileChuKyEntity.TenGuid).DuongDan = fileChuKyEntity.DuongDanTmp;
                    }
                }

                foreach (var noiTruHoSo in noiTruHoSoConLai)
                {
                    var bienBanTruHoSo = JsonConvert.DeserializeObject<BangTheoDoiHoiTinhViewModel>(noiTruHoSo.ThongTinHoSo);

                    var phieuHoiTinhViewModel = new PhieuHoiTinhViewModel
                    {
                        Id = noiTruHoSo.Id,
                        NgayThucHienDisplay = noiTruHoSo.NgayHoiChan.ApplyFormatDate(),
                        CachMo = bienBanTruHoSo.CachMo,
                        DdNhan = bienBanTruHoSo.DdNhan
                    };
                    bangTheoDoiObj.ListPhieu.Add(phieuHoiTinhViewModel);
                }

                bangTheoDoiObj.ThongTinHoSo = bangTheoDoiCuThe.ThongTinHoSo;

                return bangTheoDoiObj;
            }

            return new BangTheoDoiHoiTinhViewModel();
        }

        [HttpGet("DeleteBangTheoDoiHoiTinh")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> DeleteBangTheoDoiHoiTinh(long phieuId, long yctnId)
        {
            var bienBanHoiChan = await _noiTruHoSoKhacService.GetByIdAsync(phieuId);
            if (bienBanHoiChan.YeuCauTiepNhanId == yctnId)
            {
                await _noiTruHoSoKhacService.DeleteByIdAsync(phieuId);
            }
            return Ok();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("PhieuInBangTheoDoiHoiTinh")]
        public async Task<ActionResult> PhieuInBangTheoDoiHoiTinh(BangTheoDoiHoiTinhHttpParamsRequest bangTheoDoiHoiTinhHttpParams)
        {
            var phieuIns = await _noiTruHoSoKhacService.PhieuInBangTheoDoiHoiTinh(bangTheoDoiHoiTinhHttpParams);
            return Ok(phieuIns);
        }
    }
}
