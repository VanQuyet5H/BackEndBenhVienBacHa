using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Infrastructure.Auth;
using Camino.Api.Models.Error;
using Camino.Api.Models.General;
using Camino.Api.Models.KhoaPhongNhanVien;
using Camino.Api.Models.NhanVien;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.HoSoNhanVienDinhKems;
using Camino.Core.Domain.Entities.KhoNhanVienQuanLys;
using Camino.Core.Domain.Entities.NhanVienChucVus;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ChucVu;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.NhanVien;
using Camino.Core.Helpers;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Camino.Services.Messages;
using Camino.Services.NhanVien;
using Camino.Services.PhongBenhVien;
using Camino.Services.TaiLieuDinhKem;
using Camino.Services.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Camino.Api.Controllers
{
    public class NhanVienController : CaminoBaseController
    {
        readonly INhanVienService _nhanVienService;
        private readonly ISmsService _smsService;
        private readonly ILocalizationService _localizationService;
        private readonly IUserService _userService;
        private readonly IExcelService _excelService;
        private readonly IPhongBenhVienService _phongBenhVienService;
        private readonly ITaiLieuDinhKemService _taiLieuDinhKemService;

        public NhanVienController(IJwtFactory jwtFactory, INhanVienService nhanVienService, IPhongBenhVienService phongBenhVienService,
            ISmsService smsService, ILocalizationService localizationService, IUserService userService, IExcelService excelService,
            ITaiLieuDinhKemService taiLieuDinhKemService)
        {
            _nhanVienService = nhanVienService;
            _smsService = smsService;
            _localizationService = localizationService;
            _userService = userService;
            _excelService = excelService;
            _phongBenhVienService = phongBenhVienService;
            _taiLieuDinhKemService = taiLieuDinhKemService;
        }
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucNhanVien)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _nhanVienService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucNhanVien)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _nhanVienService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }


        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucNhanVien)]
        public async Task<ActionResult<NhanVienViewModel>> Post([FromBody] NhanVienViewModel nhanVienViewModel)
        {

            //thêm kho cho nhân viên 09/10/2020
            if (nhanVienViewModel.PhongBenhVienIds != null && nhanVienViewModel.PhongBenhVienIds.Any())
            {
                var khoaPhongNhanVienViewModels = new List<KhoaPhongNhanVienViewModel>();

                var khoaTheoPhongs = _nhanVienService.KhoaTheoPhong(nhanVienViewModel.PhongBenhVienIds);
                nhanVienViewModel.KhoaPhongIds = khoaTheoPhongs;

                foreach (var khoaPhongId in nhanVienViewModel.KhoaPhongIds)
                {
                    var phongBenhVienTheoTungKhoas = _nhanVienService.kiemTraPhongThuocKhoa(nhanVienViewModel.PhongBenhVienIds, khoaPhongId);

                    if (phongBenhVienTheoTungKhoas.Any())
                    {
                        foreach (var phongBenhVienId in phongBenhVienTheoTungKhoas)
                        {
                            var model = new KhoaPhongNhanVienViewModel
                            {
                                NhanVienId = nhanVienViewModel.Id,
                                KhoaPhongId = khoaPhongId,
                                PhongBenhVienId = phongBenhVienId
                            };
                            khoaPhongNhanVienViewModels.Add(model);
                        }
                    }
                    else
                    {
                        var model = new KhoaPhongNhanVienViewModel
                        {
                            NhanVienId = nhanVienViewModel.Id,
                            KhoaPhongId = khoaPhongId
                        };
                        khoaPhongNhanVienViewModels.Add(model);
                    }
                }
                nhanVienViewModel.KhoaPhongNhanViens.AddRange(khoaPhongNhanVienViewModels);
            }
            else
            {
                nhanVienViewModel.KhoaPhongNhanViens.AddRange(nhanVienViewModel.KhoaPhongIds.Select(itemId => new KhoaPhongNhanVienViewModel
                {
                    NhanVienId = nhanVienViewModel.Id,
                    KhoaPhongId = itemId,
                }).GroupBy(t => t.KhoaPhongId).Select(p => p.First()));
            }


            //Chọn kho nhân viên
            if (nhanVienViewModel.KhoNhanVienQuanLyIds != null && nhanVienViewModel.KhoNhanVienQuanLyIds.Any())
            {
                var khoNhanVienQuanLyModels = new List<KhoNhanVienQuanLyModel>();
                var khoTheoNhanViens = _nhanVienService.KhoTheoNhanVien(nhanVienViewModel.UserId);
                foreach (var khoId in nhanVienViewModel.KhoNhanVienQuanLyIds)
                {
                    if (khoTheoNhanViens.Any())
                    {
                        foreach (var phongBenhVienId in khoTheoNhanViens)
                        {
                            var model = new KhoNhanVienQuanLyModel
                            {
                                NhanVienId = nhanVienViewModel.Id,
                                KhoId = khoId
                            };
                            khoNhanVienQuanLyModels.Add(model);
                        }
                    }
                    else
                    {
                        var model = new KhoNhanVienQuanLyModel
                        {
                            NhanVienId = nhanVienViewModel.Id,
                            KhoId = khoId
                        };
                        khoNhanVienQuanLyModels.Add(model);
                    }
                }
                nhanVienViewModel.KhoNhanVienQuanLys.AddRange(khoNhanVienQuanLyModels);
            }
            else
            {
                nhanVienViewModel.KhoNhanVienQuanLys.AddRange(nhanVienViewModel.KhoNhanVienQuanLyIds.Select(itemId => new KhoNhanVienQuanLyModel
                {
                    NhanVienId = nhanVienViewModel.Id,
                    KhoId = itemId
                }).GroupBy(t => t.KhoId).Select(p => p.First()));
            }

            //Chọn phòng chính cho nhân viên đó
            if (nhanVienViewModel.PhongChinhId != 0)
            {
                foreach (var KhoaPhongNhanVien in nhanVienViewModel.KhoaPhongNhanViens)
                {
                    if (KhoaPhongNhanVien.PhongBenhVienId == nhanVienViewModel.PhongChinhId)
                    {
                        KhoaPhongNhanVien.LaPhongLamViecChinh = true;
                    }
                    else
                    {
                        KhoaPhongNhanVien.LaPhongLamViecChinh = null;
                    }
                }
            }

            if (nhanVienViewModel.VanBangChuyenMonId != null)
                if (!await _nhanVienService.CheckVanBangChuyenMonAsync(nhanVienViewModel.VanBangChuyenMonId ?? 0))
                    throw new ApiException(_localizationService.GetResource("NhanVien.VanBangChuyenMon.NotExists"), (int)HttpStatusCode.BadRequest);
            if (nhanVienViewModel.ChucDanhId != null)
                if (!await _nhanVienService.CheckChucDanhAsync(nhanVienViewModel.ChucDanhId ?? 0))
                    throw new ApiException(_localizationService.GetResource("NhanVien.ChucDanh.NotExists"), (int)HttpStatusCode.BadRequest);

            var nhanvien = nhanVienViewModel.ToEntity<NhanVien>();
            nhanvien.User.IsActive = true;

            // update 18/5/2020
            if (nhanVienViewModel.TaoTaiKhoan)
            {
                nhanvien.User.Password = PasswordHasher.HashPassword(nhanVienViewModel.Password);
                foreach (var roleId in nhanVienViewModel.LstRole)
                {
                    var nhanVienRole = new NhanVienRole
                    {
                        NhanVien = nhanvien,
                        RoleId = roleId,
                    };
                    nhanvien.NhanVienRoles.Add(nhanVienRole);
                }
            }
            //
            if (!string.IsNullOrEmpty(nhanVienViewModel.Avatar))
            {
                nhanvien.User.Avatar = nhanVienViewModel.Avatar;
            }
            #region update 28/12/2020
            if (nhanVienViewModel.ChucVuIds.Count() > 0)
            {
                foreach (var chucVuId in nhanVienViewModel.ChucVuIds.ToList())
                {
                    var nhanVienChucVu = new NhanVienChucVu
                    {
                        NhanVien = nhanvien,
                        ChucVuId = chucVuId,
                    };
                    nhanvien.NhanVienChucVus.Add(nhanVienChucVu);
                }
            }
            if (nhanVienViewModel.HoSoNhanVienFileDinhKems.Count() > 0)
            {
                foreach (var itemFile in nhanVienViewModel.HoSoNhanVienFileDinhKems.ToList())
                {
                    var hoSoNhanVienFileDinhKemInfo = new HoSoNhanVienFileDinhKem
                    {
                        NhanVien = nhanvien,
                        Ma = itemFile.Uid,
                        Ten = itemFile.Ten,
                        TenGuid = itemFile.TenGuid,
                        DuongDan = itemFile.DuongDan,
                        LoaiTapTin = itemFile.LoaiTapTin,
                        MoTa = itemFile.MoTa,
                        KichThuoc = itemFile.KichThuoc
                    };
                    nhanvien.HoSoNhanVienFileDinhKems.Add(hoSoNhanVienFileDinhKemInfo);
                }
            }
            #endregion update 28/12/2020
            await _nhanVienService.AddAsync(nhanvien);
            //_smsService.SendSmsTaoNhanVien(nhanvien.User.SoDienThoai.RemoveFormatPhone());
            var nv = await _nhanVienService.GetByIdAsync(nhanvien.Id, s => s.Include(ur => ur.NhanVienRoles).ThenInclude(r => r.Role));
            var actionName = nameof(Get);
            return CreatedAtAction(actionName, new { id = nhanvien.Id }, nv.ToModel<NhanVienViewModel>());

        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucNhanVien)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<NhanVienViewModel>> Get(long id)
        {
            var nhanVien = await _nhanVienService.GetByIdAsync(id, s => s.Include(u => u.User)
             .Include(o => o.VanBangChuyenMon).Include(o => o.PhamViHanhNghe).Include(o => o.HocHamHocVi).Include(o => o.ChucDanh)
             .Include(o => o.KhoaPhongNhanViens).Include(o => o.KhoNhanVienQuanLys).Include(o => o.NhanVienRoles).ThenInclude(o => o.Role)
             .Include(o => o.NhanVienChucVus)
             .Include(o => o.HoSoNhanVienFileDinhKems));
            if (nhanVien == null)
            {
                return NotFound();
            }

            var modelNhanVien = nhanVien.ToModel<NhanVienViewModel>();
            modelNhanVien.KhoaPhongIds = modelNhanVien.KhoaPhongIds?.Select(cc => cc).Distinct().ToList() ?? new List<long>();
            modelNhanVien.PhongBenhVienIds = nhanVien.KhoaPhongNhanViens.Where(cc => cc.PhongBenhVienId != null).Select(cc => (long)cc.PhongBenhVienId).ToList();

            var khoaPhongIds = nhanVien.KhoaPhongNhanViens.Where(cc => cc.PhongBenhVienId == null).Select(cc => (long)cc.KhoaPhongId).ToList();
            var phongBenhVienIds = await _phongBenhVienService.GetPhongByListKhoa(khoaPhongIds);
            if (phongBenhVienIds.Any())
                modelNhanVien.PhongBenhVienIds.AddRange(phongBenhVienIds);

            modelNhanVien.Avatar = nhanVien.User.Avatar;
            modelNhanVien.TaoTaiKhoan = !string.IsNullOrEmpty(nhanVien.User.Password);

            return Ok(modelNhanVien);
        }

        [HttpGet("GetNhanVien/{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.None)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<NhanVienViewModel>> GetNhanVien(long id)
        {
            var nhanVien = await _nhanVienService.GetByIdAsync(id, s => s.Include(u => u.User)
            .Include(o => o.VanBangChuyenMon).Include(o => o.PhamViHanhNghe).Include(o => o.HocHamHocVi).Include(o => o.ChucDanh)
             .Include(o => o.KhoaPhongNhanViens).Include(o => o.NhanVienRoles).ThenInclude(o => o.Role).Include(o => o.NhanVienChucVus));

            if (nhanVien == null)
            {
                return NotFound();
            }
            var modelNhanVien = nhanVien.ToModel<NhanVienViewModel>();
            modelNhanVien.Avatar = nhanVien.User.Avatar;

            return Ok(modelNhanVien);
        }

        [HttpPost("CapNhatNhanVien")]
        [ClaimRequirement(Enums.SecurityOperation.None, Enums.DocumentType.None)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> CapNhatNhanVien([FromBody] NhanVienViewModel nhanVienViewModel)
        {
            if (nhanVienViewModel.VanBangChuyenMonId != 0)
                if (!await _nhanVienService.CheckVanBangChuyenMonAsync(nhanVienViewModel.VanBangChuyenMonId ?? 0))
                    throw new ApiException(_localizationService.GetResource("NhanVien.VanBangChuyenMon.NotExists"), (int)HttpStatusCode.BadRequest);

            var nhanvien = await _nhanVienService.GetByIdAsync(nhanVienViewModel.Id, s => s.Include(u => u.User)
             .Include(o => o.VanBangChuyenMon).Include(o => o.PhamViHanhNghe).Include(o => o.HocHamHocVi).Include(o => o.ChucDanh)
             .Include(o => o.NhanVienRoles).ThenInclude(o => o.Role).Include(o => o.NhanVienChucVus));
            nhanVienViewModel.ToEntity(nhanvien);

            await _nhanVienService.UpdateAsync(nhanvien);
            return NoContent();
        }


        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucNhanVien)]
        public async Task<ActionResult> Put([FromBody] NhanVienViewModel nhanVienViewModel)
        {

            if (nhanVienViewModel.VanBangChuyenMonId != null)
                if (!await _nhanVienService.CheckVanBangChuyenMonAsync(nhanVienViewModel.VanBangChuyenMonId ?? 0))
                    throw new ApiException(_localizationService.GetResource("NhanVien.VanBangChuyenMon.NotExists"), (int)HttpStatusCode.BadRequest);
            if (nhanVienViewModel.ChucDanhId != null)
                if (!await _nhanVienService.CheckChucDanhAsync(nhanVienViewModel.ChucDanhId ?? 0))
                    throw new ApiException(_localizationService.GetResource("NhanVien.ChucDanh.NotExists"), (int)HttpStatusCode.BadRequest);


            var nhanvien = await _nhanVienService.GetByIdAsync(nhanVienViewModel.Id, s => s.Include(u => u.NhanVienRoles)
                        .Include(o => o.User)
                        .Include(o => o.KhoaPhongNhanViens).Include(o => o.KhoNhanVienQuanLys)
                        .Include(o => o.NhanVienChucVus)
                        .Include(o => o.HoSoNhanVienFileDinhKems));

            if (nhanVienViewModel.PhongBenhVienIds != null && nhanVienViewModel.PhongBenhVienIds.Any())
            {
                var khoaPhongNhanVienViewModels = new List<KhoaPhongNhanVienViewModel>();

                var khoaTheoPhongs = _nhanVienService.KhoaTheoPhong(nhanVienViewModel.PhongBenhVienIds);
                nhanVienViewModel.KhoaPhongIds = khoaTheoPhongs;

                foreach (var khoaPhongId in nhanVienViewModel.KhoaPhongIds)
                {
                    var phongBenhVienTheoTungKhoas = _nhanVienService.kiemTraPhongThuocKhoa(nhanVienViewModel.PhongBenhVienIds, khoaPhongId);

                    if (phongBenhVienTheoTungKhoas.Any())
                    {
                        foreach (var phongBenhVienId in phongBenhVienTheoTungKhoas)
                        {
                            var model = new KhoaPhongNhanVienViewModel
                            {
                                NhanVienId = nhanVienViewModel.Id,
                                KhoaPhongId = khoaPhongId,
                                PhongBenhVienId = phongBenhVienId
                            };
                            khoaPhongNhanVienViewModels.Add(model);
                        }
                    }
                    else
                    {
                        var model = new KhoaPhongNhanVienViewModel
                        {
                            NhanVienId = nhanVienViewModel.Id,
                            KhoaPhongId = khoaPhongId
                        };
                        khoaPhongNhanVienViewModels.Add(model);
                    }
                }
                nhanVienViewModel.KhoaPhongNhanViens.AddRange(khoaPhongNhanVienViewModels);
            }
            else
            {
                nhanVienViewModel.KhoaPhongNhanViens.AddRange(nhanVienViewModel.KhoaPhongIds.Select(itemId => new KhoaPhongNhanVienViewModel
                {
                    NhanVienId = nhanVienViewModel.Id,
                    KhoaPhongId = itemId,
                }).GroupBy(t => t.KhoaPhongId).Select(p => p.First()));
            }

            //Chọn kho nhân viên
            if (nhanVienViewModel.KhoNhanVienQuanLyIds != null && nhanVienViewModel.KhoNhanVienQuanLyIds.Any())
            {
                //xóa tất cả kho củ cua nhân viên đó.
                //_nhanVienService.kiemTraKhoNhanVien(nhanVienViewModel.UserId);

                //add lại thông tin kho mới cho nhân viên.
                var khoNhanVienQuanLyModels = new List<KhoNhanVienQuanLyModel>();
                var khoTheoNhanViens = _nhanVienService.KhoTheoNhanVien(nhanVienViewModel.UserId);
                foreach (var khoId in nhanVienViewModel.KhoNhanVienQuanLyIds)
                {
                    if (khoTheoNhanViens.Any())
                    {
                        var model = new KhoNhanVienQuanLyModel
                        {
                            NhanVienId = nhanVienViewModel.Id,
                            KhoId = khoId
                        };
                        khoNhanVienQuanLyModels.Add(model);
                    }
                    else
                    {
                        var model = new KhoNhanVienQuanLyModel
                        {
                            NhanVienId = nhanVienViewModel.Id,
                            KhoId = khoId
                        };
                        khoNhanVienQuanLyModels.Add(model);
                    }
                }
                nhanVienViewModel.KhoNhanVienQuanLys.AddRange(khoNhanVienQuanLyModels);
            }
            else
            {
                nhanVienViewModel.KhoNhanVienQuanLys.AddRange(nhanVienViewModel.KhoNhanVienQuanLyIds.Select(itemId => new KhoNhanVienQuanLyModel
                {
                    NhanVienId = nhanVienViewModel.Id,
                    KhoId = itemId
                }).GroupBy(t => t.KhoId).Select(p => p.First()));
            }

            //Chọn phòng chính cho nhân viên đó
            if (nhanVienViewModel.PhongChinhId != 0)
            {
                foreach (var KhoaPhongNhanVien in nhanVienViewModel.KhoaPhongNhanViens)
                {
                    if (KhoaPhongNhanVien.PhongBenhVienId == nhanVienViewModel.PhongChinhId)
                    {
                        KhoaPhongNhanVien.LaPhongLamViecChinh = true;
                    }
                    else
                    {
                        KhoaPhongNhanVien.LaPhongLamViecChinh = null;
                    }
                }
            }

            nhanVienViewModel.ToEntity(nhanvien);

            // update 18/5/2020
            if (nhanVienViewModel.TaoTaiKhoan)
            {
                if (!string.IsNullOrEmpty(nhanVienViewModel.PasswordNew))
                {
                    nhanvien.User.Password = PasswordHasher.HashPassword(nhanVienViewModel.PasswordNew);
                }
                foreach (var item in nhanVienViewModel.LstRole)
                {
                    if (!nhanvien.NhanVienRoles.Any(p => p.RoleId == item))
                    {
                        var nhanVienRole = new NhanVienRole
                        {
                            NhanVien = nhanvien,
                            RoleId = item,
                        };
                        nhanvien.NhanVienRoles.Add(nhanVienRole);
                    }
                }

                foreach (var item in nhanvien.NhanVienRoles)
                {
                    if (!nhanVienViewModel.LstRole.Any(p => p == item.RoleId))
                    {
                        item.WillDelete = true;
                    }
                }
            }
            else
            {
                nhanvien.User.Password = null;
            }
            //
            if (!string.IsNullOrEmpty(nhanVienViewModel.Avatar))
            {
                nhanvien.User.Avatar = nhanVienViewModel.Avatar;
            }

            if (nhanVienViewModel.TaoTaiKhoan)
            {
                foreach (var item in nhanVienViewModel.ChucVuIds)
                {
                    if (!nhanvien.NhanVienChucVus.Any(p => p.ChucVuId == item))
                    {
                        var nhanVienChucVu = new NhanVienChucVu
                        {
                            NhanVien = nhanvien,
                            ChucVuId = item,
                        };
                        nhanvien.NhanVienChucVus.Add(nhanVienChucVu);
                    }
                }

                foreach (var item in nhanvien.NhanVienChucVus)
                {
                    if (!nhanVienViewModel.ChucVuIds.Any(p => p == item.ChucVuId))
                    {
                        item.WillDelete = true;
                    }
                }
            }
            if (nhanVienViewModel.TaoTaiKhoan)
            {
                foreach (var item in nhanvien.HoSoNhanVienFileDinhKems)
                {
                    if (!nhanVienViewModel.HoSoNhanVienFileDinhKems.Any(p => p.Id == item.Id))
                    {
                        item.WillDelete = true;
                    }
                }
                foreach (var item in nhanVienViewModel.HoSoNhanVienFileDinhKems)
                {
                    if (!nhanvien.HoSoNhanVienFileDinhKems.Any(p => p.NhanVienId == item.NhanVienId))
                    {
                        var hoSoNhanVienFileDinhKemInfo = new HoSoNhanVienFileDinhKem
                        {
                            NhanVien = nhanvien,
                            NhanVienId = nhanvien.Id,
                            Ma = item.Uid,
                            Ten = item.Ten,
                            TenGuid = item.TenGuid,
                            DuongDan = item.DuongDan,
                            LoaiTapTin = item.LoaiTapTin,
                            MoTa = item.MoTa,
                            KichThuoc = item.KichThuoc
                        };
                        //_taiLieuDinhKemService.LuuTaiLieuAsync(hoSoNhanVienFileDinhKemInfo.DuongDan, hoSoNhanVienFileDinhKemInfo.TenGuid);
                        nhanvien.HoSoNhanVienFileDinhKems.Add(hoSoNhanVienFileDinhKemInfo);
                    }
                }


            }

            await _nhanVienService.UpdateAsync(nhanvien);
            return NoContent();
        }

        [HttpPost("KhoaTaiKhoan")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucNhanVien)]
        public async Task<ActionResult> KhoaTaiKhoan(long id)
        {
            var nhanvien = await _nhanVienService.GetByIdAsync(id, s => s.Include(u => u.User));
            nhanvien.User.IsActive = !nhanvien.User.IsActive;
            //    nhanvien.KichHoat = false;
            await _nhanVienService.UpdateAsync(nhanvien);
            return NoContent();
        }


        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucNhanVien)]
        public async Task<ActionResult> Delete(long id)
        {
            var nhanVien = await _userService.GetByIdAsync(id, s => s.Include(u => u.NhanVien).ThenInclude(k => k.KhoaPhongNhanViens)
            .Include(u => u.NhanVien).ThenInclude(z => z.NhanVienRoles)
            .Include(u => u.NhanVien).ThenInclude(z => z.NhanVienChucVus)
            .Include(l => l.NhanVien).ThenInclude(g => g.HoSoNhanVienFileDinhKems));
            if (nhanVien == null)
            {
                return NotFound();
            }
            await _userService.DeleteAsync(nhanVien);
            return NoContent();
        }


        [HttpPost("Deletes")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucNhanVien)]
        public async Task<ActionResult> Deletes([FromBody] DeletesViewModel model)
        {
            var entitys = await _userService.GetByIdsAsync(model.Ids, s => s.Include(u => u.NhanVien).ThenInclude(k => k.KhoaPhongNhanViens)
            .Include(u => u.NhanVien).ThenInclude(z => z.NhanVienRoles)
            .Include(u => u.NhanVien).ThenInclude(g => g.NhanVienChucVus)
            .Include(u => u.NhanVien).ThenInclude(g => g.HoSoNhanVienFileDinhKems));
            if (entitys == null)
            {
                return NotFound();
            }
            if (entitys.Count() != model.Ids.Length)
            {
                throw new ArgumentException(_localizationService.GetResource("Common.WrongLengthMultiDelete"));
            }
            await _userService.DeleteAsync(entitys);
            return NoContent();
        }

        #region GetListLookupItemVo
        [HttpPost("GetListLookupNhanVien")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListLookupNhanVien(DropDownListRequestModel model)
        {
            var lookup = await _nhanVienService.GetListTenNhanVien(model);
            return Ok(lookup);
        }

        [HttpPost("GetListLookupNhanVienAutoComplete")]
        public ActionResult<ICollection<string>> GetListLookupNhanVienAutoComplete(DropDownListRequestModel model)
        {
            var lookup = _nhanVienService.GetListTenNhanVien(model).Result.Select(cc => cc.DisplayName).ToList();
            return Ok(lookup);
        }

        [HttpPost("GetListLookupChucDanhNhanVien")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListLookupChucDanhNhanVien(DropDownListRequestModel model)
        {
            var lookup = await _nhanVienService.GetListTenChucDanhNhanVien(model);
            return Ok(lookup);
        }


        [HttpPost("GetListLookupChucDanhNhanVienAutoComplete")]
        public ActionResult<ICollection<string>> GetListLookupChucDanhNhanVienAutoComplete(DropDownListRequestModel model)
        {
            var lookup = _nhanVienService.GetListTenChucDanhNhanVien(model).Result.Select(cc => cc.DisplayName).ToList();
            return Ok(lookup);
        }


        [HttpPost("GetListLookupNhanVienIsBacSi")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListLookupNhanVienIsBacSi(LookupQueryInfo model)
        {
            var lookup = await _nhanVienService.GetListLookupNhanVienIsBacSi(model);
            return Ok(lookup);
        }

        //Thấy hàm GetListLookupNhanVienIsBacSi thua mẹ nó luôn model.Id = khoaphongId, hết đường nói
        [HttpPost("GetListLookupNhanVienIsBacSiClone")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListLookupNhanVienIsBacSiClone(LookupQueryInfo model)
        {
            var lookup = await _nhanVienService.GetListLookupNhanVienIsBacSiClone(model);
            return Ok(lookup);
        }

        [HttpPost("GetListLookupNhanVienIsYta")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListLookupNhanVienIsYta(LookupQueryInfo model)
        {
            var lookup = await _nhanVienService.GetListLookupNhanVienIsYta(model);
            return Ok(lookup);
        }
        [HttpPost("GetListKhoaPhongByHoSoNhanVien")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListKhoaPhongByHoSoNhanVien([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _nhanVienService.GetListKhoaPhongByHoSoNhanVien(model);
            return Ok(lookup);
        }

        [HttpGet("GetListPhongByKhoa")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListPhongByKhoa(long nhanVienId, string khoaphongIds)
        {
            DropDownListRequestModel model = new DropDownListRequestModel();
            var phongBenhViens = await _nhanVienService.GetListPhongNhanVienByHoSoNhanVien(model, nhanVienId, khoaphongIds);
            return Ok(phongBenhViens);
        }

        [HttpPost("GetListPhongByKhoas")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListPhongByKhoas(DropDownListRequestModel model, long nhanVienId, string khoaphongIds)
        {
            var phongBenhViens = await _nhanVienService.GetListPhongNhanVienByHoSoNhanVien(model, nhanVienId, khoaphongIds);
            return Ok(phongBenhViens);
        }


        [HttpPost("GetListKhoaPhongDataForGridAsync")]
        public async Task<ActionResult<GridDataSource>> GetListKhoaPhongDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var phongBenhViens = await _nhanVienService.GetListKhoaPhongDataForGridAsync(queryInfo);
            return Ok(phongBenhViens);
        }


        [HttpPost("GetListKhoTheoPhongDataForGridAsync")]
        public async Task<ActionResult<GridDataSource>> GetListKhoTheoPhongDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var khoNhanViens = await _nhanVienService.GetListKhoTheoPhongDataForGridAsync(queryInfo);
            return Ok(khoNhanViens);
        }

        #endregion

        #region Thông tin proflie của nhân viên

        [HttpGet("GetNhanVienProflie/{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.None)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<NhanVienViewModel>> GetNhanVienProflie(long id)
        {
            var nhanVien = await _nhanVienService.GetByIdAsync(id, s => s.Include(u => u.User)
            .Include(o => o.VanBangChuyenMon).Include(o => o.PhamViHanhNghe).Include(o => o.HocHamHocVi).Include(o => o.ChucDanh)
             .Include(o => o.KhoaPhongNhanViens).Include(o => o.NhanVienRoles).ThenInclude(o => o.Role).Include(o => o.NhanVienChucVus)
             .Include(o => o.HoSoNhanVienFileDinhKems));

            if (nhanVien == null)
            {
                return NotFound();
            }
            var modelNhanVien = nhanVien.ToModel<NhanVienViewModel>();

            modelNhanVien.KhoaPhongIds = modelNhanVien.KhoaPhongIds.Select(cc => cc).Distinct().ToList();
            modelNhanVien.PhongBenhVienIds = nhanVien.KhoaPhongNhanViens.Where(cc => cc.PhongBenhVienId != null).Select(cc => (long)cc.PhongBenhVienId).ToList();

            modelNhanVien.Avatar = nhanVien.User.Avatar;

            return Ok(modelNhanVien);
        }

        [HttpPost("CapNhatProfileNhanVien")]
        [ClaimRequirement(Enums.SecurityOperation.None, Enums.DocumentType.None)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> CapNhatProfileNhanVien([FromBody] ProfileNhanVien nhanVienViewModel)
        {
            var nhanvien = await _nhanVienService.GetByIdAsync(nhanVienViewModel.Id, s => s.Include(u => u.User)
             .Include(o => o.KhoaPhongNhanViens)
             .Include(o => o.VanBangChuyenMon).Include(o => o.PhamViHanhNghe).Include(o => o.HocHamHocVi).Include(o => o.ChucDanh)
             .Include(o => o.NhanVienRoles).ThenInclude(o => o.Role).Include(o => o.NhanVienChucVus)
             .Include(o => o.HoSoNhanVienFileDinhKems));
            if (nhanVienViewModel.KhoaPhongIds != null)
            {
                nhanVienViewModel.KhoaPhongNhanViens.AddRange(nhanVienViewModel.KhoaPhongIds.Select(itemId => new KhoaPhongNhanVienViewModel
                {
                    NhanVienId = nhanVienViewModel.Id,
                    KhoaPhongId = itemId
                }).GroupBy(t => t.KhoaPhongId).Select(p => p.First()));
            }
            nhanVienViewModel.ToEntity(nhanvien);
            if (!string.IsNullOrEmpty(nhanVienViewModel.Password))
            {
                nhanvien.User.Password = PasswordHasher.HashPassword(nhanVienViewModel.Password);
            }

            if (!string.IsNullOrEmpty(nhanVienViewModel.Avatar))
            {
                nhanvien.User.Avatar = nhanVienViewModel.Avatar;
            }

            foreach (var item in nhanVienViewModel.LstRole)
            {
                if (!nhanvien.NhanVienRoles.Any(p => p.RoleId == item))
                {
                    var nhanVienRole = new NhanVienRole
                    {
                        NhanVien = nhanvien,
                        RoleId = item,
                    };
                    nhanvien.NhanVienRoles.Add(nhanVienRole);
                }
            }

            foreach (var item in nhanvien.NhanVienRoles)
            {
                if (!nhanVienViewModel.LstRole.Any(p => p == item.RoleId))
                {
                    item.WillDelete = true;
                }
            }

            #region 28122021
            foreach (var item in nhanvien.NhanVienChucVus)
            {
                if (!nhanVienViewModel.ChucVuIds.Any(p => p == item.ChucVuId))
                {
                    item.WillDelete = true;
                }
            }
            foreach (var item in nhanVienViewModel.ChucVuIds)
            {
                if (!nhanvien.NhanVienChucVus.Any(p => p.ChucVuId == item))
                {
                    var nhanVienChucVu = new NhanVienChucVu
                    {
                        NhanVien = nhanvien,
                        ChucVuId = item,
                    };
                    nhanvien.NhanVienChucVus.Add(nhanVienChucVu);
                }
            }




            if (nhanVienViewModel.HoSoNhanVienFileDinhKems != null)
            {
                foreach (var item in nhanVienViewModel.HoSoNhanVienFileDinhKems)
                {
                    if (!nhanvien.HoSoNhanVienFileDinhKems.Any(p => p.NhanVienId == item.NhanVienId))
                    {
                        var hoSoNhanVienFileDinhKemInfo = new HoSoNhanVienFileDinhKem
                        {
                            NhanVien = nhanvien,
                            NhanVienId = nhanvien.Id,
                            Ma = item.Uid,
                            Ten = item.Ten,
                            TenGuid = item.TenGuid,
                            DuongDan = item.DuongDan,
                            LoaiTapTin = item.LoaiTapTin,
                            MoTa = item.MoTa,
                            KichThuoc = item.KichThuoc
                        };
                        nhanvien.HoSoNhanVienFileDinhKems.Add(hoSoNhanVienFileDinhKemInfo);
                    }
                }
                foreach (var item in nhanvien.HoSoNhanVienFileDinhKems)
                {
                    if (!nhanVienViewModel.HoSoNhanVienFileDinhKems.Any(p => p.Id == item.Id))
                    {
                        item.WillDelete = true;
                    }
                }
            }
            #endregion 28122021

            await _nhanVienService.UpdateAsync(nhanvien);
            return NoContent();
        }
        #endregion

        [HttpPost("ExportHoSoNhanVien")]
        public async Task<ActionResult> ExportHoSoNhanVien(QueryInfo queryInfo)
        {
            var gridData = await _nhanVienService.GetDataForGridAsync(queryInfo, true);
            var hoSoNhanVienData = gridData.Data.Select(p => (NhanVienGridVo)p).ToList();
            var excelData = hoSoNhanVienData.Map<List<NhanVienExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(NhanVienExportExcel.HoTen), "Họ tên"));
            lstValueObject.Add((nameof(NhanVienExportExcel.SoChungMinhThu), "Số CMT"));
            lstValueObject.Add((nameof(NhanVienExportExcel.SoDienThoai), "Số điện thoại"));
            lstValueObject.Add((nameof(NhanVienExportExcel.Email), "Email"));
            lstValueObject.Add((nameof(NhanVienExportExcel.DiaChi), "Địa chỉ"));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Hồ sơ nhân viên");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=HoSoNhanVien" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        [HttpPost("GetTatCaPhongCuaNhanVienLogin")]
        public ActionResult<List<LookupItemVo>> GetTatCaPhongCuaNhanVienLogin(LookupQueryInfo model)
        {
            var lookup = _nhanVienService.GetTatCaPhongCuaNhanVienLogin(model);
            return Ok(lookup);
        }
        [HttpPost("GetTatCaKhoLeCuaNhanVienLogin")]
        public ActionResult<List<LookupItemVo>> GetTatCaKhoLeCuaNhanVienLogin(LookupQueryInfo model)
        {
            var lookup = _nhanVienService.GetTatCaKhoLeCuaNhanVienLogin(model);
            return Ok(lookup);
        }
        [HttpPost("GetListChucVu")]
        public async Task<ActionResult<ICollection<LookupItemTemplateVo>>> GetListChucVu(DropDownListRequestModel model)
        {
            var lookup = await _nhanVienService.GetListChucVu(model);
            return Ok(lookup);
        }


        [HttpPost("GetListMaBacSi")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListMaBacSi(LookupQueryInfo model)
        {
            var lookup = await _nhanVienService.GetListMaBacSi(model);
            return Ok(lookup);
        }

    }
}
