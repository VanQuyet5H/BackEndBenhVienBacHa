using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.Error;
using Camino.Api.Models.YeuCauHoanTraVatTu;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.YeuCauTraVatTus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauHoanTra;
using Camino.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.Controllers
{
    public partial class YeuCauTraVatTuController
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetAllVatTuData")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.YeuCauHoanTraVatTu)]
        public async Task<ActionResult<GridDataSource>> GetAllVatTuData([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _ycHoanTraVatTuService.GetAllVatTuData(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetAllVatTuTotal")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.YeuCauHoanTraVatTu)]
        public async Task<ActionResult<GridDataSource>> GetAllVatTuTotal([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _ycHoanTraVatTuService.GetAllVatTuTotal(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetKhoVatTu")]
        public async Task<ActionResult> GetKhoVatTu(DropDownListRequestModel model)
        {
            var result = await _ycHoanTraVatTuService.GetKhoVatTu(model);
            return Ok(result);
        }

        [HttpPost("GetKhoVatTuHoanTra")]
        public async Task<ActionResult> GetKhoVatTuHoanTra(DropDownListRequestModel model)
        {
            var result = await _ycHoanTraVatTuService.GetKhoVatTuHoanTra(model);
            return Ok(result);
        }

        [HttpPost("GetVatTuOnGroup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> GetVatTuOnGroup([FromBody] GetVatTuOnGroupModel model)
        {
            var result = await _ycHoanTraVatTuService.GetVatTuOnGroup(model.Id, model.KhoXuatId, model.SearchString, model.ListDaChon);
            return Ok(result);
        }

        //[HttpPost]
        [HttpPost("GuiPhieuHoanTraVatTu")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.YeuCauHoanTraVatTu)]
        public async Task<ActionResult> Post
           ([FromBody] YeuCauHoanTraVatTuViewModel model)
        {
            if (!model.YeuCauTraVatTuChiTiets.Any())
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.YeuCauTraVatTuChiTiet.Required"));

            }

            if ((model.KhoXuatId != null && !await _xuatKhoService.IsKhoExists(model.KhoXuatId ?? 0))
                 || (model.KhoNhapId != null && !await _xuatKhoService.IsKhoExists(model.KhoNhapId ?? 0)))
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.KhoVatTu.NotExists"));
            }

            if (model.YeuCauTraVatTuChiTiets.Any(p => p.SoLuongXuat > p.SoLuongTon))
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.KhoVatTuChiTiet.SoLuongTonMoreThanSoLuongXuat"));
            }

            if (model.YeuCauTraVatTuChiTiets.Any(p => p.SoLuongXuat == 0 || p.SoLuongXuat == null))
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.KhoVatTuChiTiet.SoLuongXuatMoreThan0"));
            }

            var entity = model.ToEntity<YeuCauTraVatTu>();

            foreach (var item in model.YeuCauTraVatTuChiTiets)
            {
                var modelThemVatTuHoanTra = new ThemVatTuHoanTra
                {
                    NhapKhoVatTuChiTietId = long.Parse(item.Id.Split(",")[0]),
                    NhomVatTu = item.Id.Split(",")[1] == Enums.LoaiSuDung.VatTuTieuHao.GetDescription() ? Enums.LoaiSuDung.VatTuTieuHao : Enums.LoaiSuDung.VatTuThayThe,
                    SoLuongXuat = item.SoLuongXuat,
                    KhoId = model.KhoXuatId,
                    LaVatTuBHYT = item.LaVatTuBHYT,
                };

                var xuatKhoVatTuChiTiet = await _ycHoanTraVatTuService.GetVatTu(modelThemVatTuHoanTra);

                entity.YeuCauTraVatTuChiTiets.Add(xuatKhoVatTuChiTiet);
            }

            _ycHoanTraVatTuService.Add(entity);

            //update gia cho NhapKhoChiTiet
            foreach (var item in model.YeuCauTraVatTuChiTiets)
            {
                var nhapKhoVatTuChiTietIdUpdate = long.Parse(item.Id.Split(",")[0]);
                await _ycHoanTraVatTuService.UpdateGiaChoNhapKhoChiTiet(item.SoLuongXuat ?? 0, nhapKhoVatTuChiTietIdUpdate);
            }

            //throw new ApiException(_localizationService.GetResource("XuatKho.KhoVatTuChiTiet.SoLuongXuatMoreThan0"));

            return Ok(entity.Id);
        }


        [HttpGet("GetUpdate")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.YeuCauHoanTraVatTu)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> GetUpdate(long id)
        {
            var entity = await _ycHoanTraVatTuService.GetByIdAsync(id,
                                        p => p.Include(x => x.YeuCauTraVatTuChiTiets).ThenInclude(x => x.XuatKhoVatTuChiTietViTri).ThenInclude(x => x.NhapKhoVatTuChiTiet)
                                       .Include(x => x.YeuCauTraVatTuChiTiets).ThenInclude(x => x.VatTuBenhVien).ThenInclude(x => x.VatTus)
                                       .Include(x => x.YeuCauTraVatTuChiTiets).ThenInclude(x => x.HopDongThauVatTu).ThenInclude(x => x.NhaThau)
                                       .Include(x => x.KhoXuat).Include(x => x.KhoNhap)
                                       .Include(x => x.NhanVienDuyet).ThenInclude(x => x.User)
                                       .Include(x => x.NhanVienYeuCau).ThenInclude(x => x.User));
            var result = entity.ToModel<YeuCauHoanTraVatTuViewModel>();
            foreach (var yeuCauTraVatTuChiTiet in entity.YeuCauTraVatTuChiTiets)
            {
                var chiTiet = result.YeuCauTraVatTuChiTiets.FirstOrDefault(p => p.Id == yeuCauTraVatTuChiTiet.Id + "");
                if (chiTiet != null)
                {
                    chiTiet.VatTu = yeuCauTraVatTuChiTiet.VatTuBenhVien.VatTus.Ten;
                    chiTiet.DVT = yeuCauTraVatTuChiTiet.VatTuBenhVien.VatTus.DonViTinh;
                    chiTiet.LaVatTuBHYT = yeuCauTraVatTuChiTiet.LaVatTuBHYT;
                    chiTiet.MaVatTu = yeuCauTraVatTuChiTiet.VatTuBenhVien.Ma;
                    chiTiet.SoLo = yeuCauTraVatTuChiTiet.Solo;
                    chiTiet.SoLuongTon =
                        (yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri?.NhapKhoVatTuChiTiet.SoLuongNhap ?? 0) -
                        (yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri?.NhapKhoVatTuChiTiet.SoLuongDaXuat ?? 0);
                    chiTiet.HanSuDung = yeuCauTraVatTuChiTiet.HanSuDung;
                    chiTiet.NgayNhap = yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri?.NhapKhoVatTuChiTiet
                        .NgayNhap;

                    chiTiet.Nhom = yeuCauTraVatTuChiTiet.VatTuBenhVien.LoaiSuDung.GetValueOrDefault().GetDescription();
                    chiTiet.SoLuongXuat = yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri?.SoLuongXuat ?? 0;
                    chiTiet.TiLeThapGia = yeuCauTraVatTuChiTiet.TiLeTheoThapGia;
                    chiTiet.Vat = yeuCauTraVatTuChiTiet.VAT;
                    chiTiet.Id = yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri?.NhapKhoVatTuChiTietId + "," +
                                 yeuCauTraVatTuChiTiet.VatTuBenhVien.LoaiSuDung.GetValueOrDefault().GetDescription() + "," +
                                 (yeuCauTraVatTuChiTiet.LaVatTuBHYT ? "true" : "false");
                }
            }

            return Ok(result);
        }

        //[HttpPut]
        [HttpPost("GuiLaiPhieuHoanTraVatTu")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.YeuCauHoanTraVatTu)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> Update([FromBody] YeuCauHoanTraVatTuViewModel model)
        {
            if (!model.YeuCauTraVatTuChiTiets.Any())
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.KhoVatTuChiTiet.Required"));

            }

            if (model.KhoXuatId != null && !await _xuatKhoService.IsKhoExists(model.KhoXuatId ?? 0)
                || model.KhoNhapId != null && !await _xuatKhoService.IsKhoExists(model.KhoNhapId ?? 0))
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.KhoVatTu.NotExists"));
            }

            var lstModelThemVatTuHoanTra = new List<ThemVatTuHoanTra>();
            foreach (var item in model.YeuCauTraVatTuChiTiets)
            {
                var nhapKhoVatTuChiTietId = long.Parse(item.Id.Split(",")[0]);
                var modelThemVatTuHoanTra = new ThemVatTuHoanTra
                {
                    NhapKhoVatTuChiTietId = nhapKhoVatTuChiTietId,
                    NhomVatTu = item.Id.Split(",")[1] == Enums.LoaiSuDung.VatTuTieuHao.GetDescription() ? Enums.LoaiSuDung.VatTuTieuHao : Enums.LoaiSuDung.VatTuThayThe,
                    SoLuongXuat = item.SoLuongXuat,
                    KhoId = model.KhoXuatId,
                    LaVatTuBHYT = item.LaVatTuBHYT,
                    SoLuongTon = item.SoLuongTon
                };
                lstModelThemVatTuHoanTra.Add(modelThemVatTuHoanTra);
            }

            if (await _ycHoanTraVatTuService.CheckValidSlTon(lstModelThemVatTuHoanTra, model.Id) == false)
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.KhoVatTuChiTiet.SoLuongTonMoreThanSoLuongXuat"));
            }

            var entity = await _ycHoanTraVatTuService.GetByIdAsync(model.Id,
                u => u.Include(x => x.YeuCauTraVatTuChiTiets).ThenInclude(x => x.XuatKhoVatTuChiTietViTri).ThenInclude(x => x.XuatKhoVatTuChiTiet)
                    .Include(x => x.YeuCauTraVatTuChiTiets).ThenInclude(x => x.XuatKhoVatTuChiTietViTri).ThenInclude(x => x.NhapKhoVatTuChiTiet)
                );

            entity.KhoNhapId = model.KhoNhapId.GetValueOrDefault();
            entity.GhiChu = model.GhiChu;

            foreach (var item in model.YeuCauTraVatTuChiTiets)
            {
                var id = long.Parse(item.Id.Split(",")[0]);

                if (entity.YeuCauTraVatTuChiTiets.All(c => c.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTietId != id))
                {
                    var modelThemVatTuHoanTra = new ThemVatTuHoanTra
                    {
                        NhapKhoVatTuChiTietId = long.Parse(item.Id.Split(",")[0]),
                        NhomVatTu = item.Id.Split(",")[1] == Enums.LoaiSuDung.VatTuTieuHao.GetDescription() ? Enums.LoaiSuDung.VatTuTieuHao : Enums.LoaiSuDung.VatTuThayThe,
                        SoLuongXuat = item.SoLuongXuat,
                        KhoId = model.KhoXuatId,
                        LaVatTuBHYT = item.LaVatTuBHYT
                    };

                    var xuatKhoVatTuChiTiet = await _ycHoanTraVatTuService.GetVatTu(modelThemVatTuHoanTra);

                    entity.YeuCauTraVatTuChiTiets.Add(xuatKhoVatTuChiTiet);
                    await _ycHoanTraVatTuService.UpdateSlXuatNhapKhoChiTiet(item.SoLuongXuat ?? 0, id);
                }
                else
                {
                    if (!entity.YeuCauTraVatTuChiTiets.Any()) continue;
                    foreach (var ycTraDpModify in entity.YeuCauTraVatTuChiTiets.Where(c => c.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTietId == id))
                    {
                        ycTraDpModify.SoLuongTra = item.SoLuongXuat.GetValueOrDefault();
                        ycTraDpModify.XuatKhoVatTuChiTietViTri.NgayXuat = model.NgayYeuCau;
                        ycTraDpModify.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.NgayXuat = model.NgayYeuCau;
                        ycTraDpModify.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.SoLuongDaXuat =
                            ycTraDpModify.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.SoLuongDaXuat -
                            ycTraDpModify.XuatKhoVatTuChiTietViTri.SoLuongXuat +
                            item.SoLuongXuat.GetValueOrDefault();
                        ycTraDpModify.XuatKhoVatTuChiTietViTri.SoLuongXuat = item.SoLuongXuat.GetValueOrDefault();
                    }
                }
            }

            var lstEntityCanXoa = entity.YeuCauTraVatTuChiTiets
                .Select(c => c.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTietId)
                .Where(q => !model.YeuCauTraVatTuChiTiets.Select(c => long.Parse(c.Id.Split(",")[0])).Contains(q) &&
                            q != 0);

            foreach (var entityYeuCauTraVatTuChiTiet in lstEntityCanXoa)
            {
                if (entity.YeuCauTraVatTuChiTiets.Any(c => c.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTietId == entityYeuCauTraVatTuChiTiet))
                {
                    var entityYcTraVtChiTiet = entity.YeuCauTraVatTuChiTiets.First(c =>
                        c.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTietId == entityYeuCauTraVatTuChiTiet);

                    entityYcTraVtChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet
                            .SoLuongDaXuat =
                        entityYcTraVtChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet
                            .SoLuongDaXuat - entityYcTraVtChiTiet.XuatKhoVatTuChiTietViTri.SoLuongXuat;
                    entityYcTraVtChiTiet.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.WillDelete =
                        true;
                    entityYcTraVtChiTiet.XuatKhoVatTuChiTietViTri.WillDelete =
                        true;
                    entityYcTraVtChiTiet.WillDelete =
                        true;
                }
            }

            await _ycHoanTraVatTuService.UpdateAsync(entity);

            return Ok(entity.Id);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("SaveEveryMovingVatTu")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.YeuCauHoanTraDuocPham)]
        public async Task<ActionResult<YeuCauHoanTraVatTuViewModel>> SaveEveryMovingVatTu([FromBody] YeuCauHoanTraVatTuViewModelIgnoreValidate model)
        {
            var entity = await _ycHoanTraVatTuService.GetByIdAsync(model.Id,
                u => u.Include(x => x.YeuCauTraVatTuChiTiets).ThenInclude(x => x.XuatKhoVatTuChiTietViTri).ThenInclude(x => x.XuatKhoVatTuChiTiet)
                    .Include(x => x.YeuCauTraVatTuChiTiets).ThenInclude(x => x.XuatKhoVatTuChiTietViTri).ThenInclude(x => x.NhapKhoVatTuChiTiet)
            );


            foreach (var item in model.YeuCauTraVatTuChiTiets)
            {
                var id = long.Parse(item.Id.Split(",")[0]);

                if (entity.YeuCauTraVatTuChiTiets.All(c => c.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTietId != id))
                {
                    var modelThemVatTuHoanTra = new ThemVatTuHoanTra
                    {
                        NhapKhoVatTuChiTietId = long.Parse(item.Id.Split(",")[0]),
                        NhomVatTu = item.Id.Split(",")[1] == Enums.LoaiSuDung.VatTuTieuHao.GetDescription() ? Enums.LoaiSuDung.VatTuTieuHao : Enums.LoaiSuDung.VatTuThayThe,
                        SoLuongXuat = item.SoLuongXuat,
                        KhoId = model.KhoXuatId,
                        LaVatTuBHYT = item.LaVatTuBHYT
                    };

                    var xuatKhoVatTuChiTiet = await _ycHoanTraVatTuService.GetVatTu(modelThemVatTuHoanTra);

                    entity.YeuCauTraVatTuChiTiets.Add(xuatKhoVatTuChiTiet);
                    await _ycHoanTraVatTuService.UpdateSlXuatNhapKhoChiTiet(item.SoLuongXuat ?? 0, id);
                }
                else
                {
                    if (!entity.YeuCauTraVatTuChiTiets.Any()) continue;
                    foreach (var ycTraDpModify in entity.YeuCauTraVatTuChiTiets.Where(c => c.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTietId == id))
                    {
                        ycTraDpModify.SoLuongTra = item.SoLuongXuat.GetValueOrDefault();
                        ycTraDpModify.XuatKhoVatTuChiTietViTri.NgayXuat = model.NgayYeuCau;
                        ycTraDpModify.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.NgayXuat = model.NgayYeuCau;
                        ycTraDpModify.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.SoLuongDaXuat =
                            ycTraDpModify.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.SoLuongDaXuat -
                            ycTraDpModify.XuatKhoVatTuChiTietViTri.SoLuongXuat +
                            item.SoLuongXuat.GetValueOrDefault();
                        ycTraDpModify.XuatKhoVatTuChiTietViTri.SoLuongXuat = item.SoLuongXuat.GetValueOrDefault();
                    }
                }
            }

            var lstEntityCanXoa = entity.YeuCauTraVatTuChiTiets
                .Select(c => c.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTietId)
                .Where(q => !model.YeuCauTraVatTuChiTiets.Select(c => long.Parse(c.Id.Split(",")[0])).Contains(q) &&
                            q != 0);

            foreach (var entityYeuCauTraVatTuChiTiet in lstEntityCanXoa)
            {
                if (entity.YeuCauTraVatTuChiTiets.Any(c => c.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTietId == entityYeuCauTraVatTuChiTiet))
                {
                    var entityYcTraVtChiTiet = entity.YeuCauTraVatTuChiTiets.First(c =>
                        c.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTietId == entityYeuCauTraVatTuChiTiet);

                    entityYcTraVtChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet
                            .SoLuongDaXuat =
                        entityYcTraVtChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet
                            .SoLuongDaXuat - entityYcTraVtChiTiet.XuatKhoVatTuChiTietViTri.SoLuongXuat;
                    entityYcTraVtChiTiet.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.WillDelete =
                        true;
                    entityYcTraVtChiTiet.XuatKhoVatTuChiTietViTri.WillDelete =
                        true;
                    entityYcTraVtChiTiet.WillDelete =
                        true;
                }
            }

            await _ycHoanTraVatTuService.UpdateAsync(entity);

            var entityResult = await _ycHoanTraVatTuService.GetByIdAsync(model.Id,
                                        p => p.Include(x => x.YeuCauTraVatTuChiTiets).ThenInclude(x => x.XuatKhoVatTuChiTietViTri)
                                            .ThenInclude(x => x.NhapKhoVatTuChiTiet)
                                            .Include(x => x.YeuCauTraVatTuChiTiets).ThenInclude(x => x.HopDongThauVatTu).ThenInclude(x => x.NhaThau)
                                       .Include(x => x.YeuCauTraVatTuChiTiets).ThenInclude(x => x.VatTuBenhVien)
                                            .ThenInclude(x => x.VatTus)
                                            .Include(w => w.KhoXuat).Include(w => w.KhoNhap)
                                            .Include(w => w.NhanVienDuyet).ThenInclude(w => w.User)
                                            .Include(w => w.NhanVienYeuCau).ThenInclude(w => w.User));
            var result = entityResult.ToModel<YeuCauHoanTraVatTuViewModel>();
            foreach (var yeuCauTraVatTuChiTiet in entityResult.YeuCauTraVatTuChiTiets)
            {
                var chiTiet = result.YeuCauTraVatTuChiTiets.FirstOrDefault(p => p.Id == yeuCauTraVatTuChiTiet.Id + "");
                if (chiTiet != null)
                {
                    chiTiet.VatTu = yeuCauTraVatTuChiTiet.VatTuBenhVien.VatTus.Ten;
                    chiTiet.MaVatTu = yeuCauTraVatTuChiTiet.VatTuBenhVien.VatTus.Ma;
                    chiTiet.DVT = yeuCauTraVatTuChiTiet.VatTuBenhVien.VatTus.DonViTinh;
                    chiTiet.LaVatTuBHYT = yeuCauTraVatTuChiTiet.LaVatTuBHYT;
                    chiTiet.SoLo = yeuCauTraVatTuChiTiet.Solo;
                    chiTiet.SoLuongTon =
                        (yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri?.NhapKhoVatTuChiTiet.SoLuongNhap ?? 0) -
                        (yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri?.NhapKhoVatTuChiTiet.SoLuongDaXuat ?? 0);
                    chiTiet.HanSuDung = yeuCauTraVatTuChiTiet.HanSuDung;
                    chiTiet.NgayNhap = yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri?.NhapKhoVatTuChiTiet
                        .NgayNhap;

                    chiTiet.Nhom = yeuCauTraVatTuChiTiet.VatTuBenhVien.LoaiSuDung != null ? yeuCauTraVatTuChiTiet.VatTuBenhVien.LoaiSuDung.GetDescription() : string.Empty;
                    chiTiet.SoLuongXuat = yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri?.SoLuongXuat ?? 0;
                    chiTiet.TiLeThapGia = yeuCauTraVatTuChiTiet.TiLeTheoThapGia;
                    chiTiet.Vat = yeuCauTraVatTuChiTiet.VAT;
                    chiTiet.Id = yeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri?.NhapKhoVatTuChiTietId + "," +
                                 yeuCauTraVatTuChiTiet.VatTuBenhVien.LoaiSuDung.GetDescription() + "," +
                                 (yeuCauTraVatTuChiTiet.LaVatTuBHYT ? "true" : "false");
                }
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.YeuCauHoanTraVatTu)]
        public async Task<ActionResult> Delete(long id)
        {
            var ycHoanTraVatTu = await _ycHoanTraVatTuService.GetByIdAsync(id, s =>
                s.Include(w => w.YeuCauTraVatTuChiTiets)
                    .ThenInclude(w => w.XuatKhoVatTuChiTietViTri).ThenInclude(w => w.NhapKhoVatTuChiTiet)
                .Include(w => w.YeuCauTraVatTuChiTiets)
                    .ThenInclude(w => w.XuatKhoVatTuChiTietViTri).ThenInclude(w => w.XuatKhoVatTuChiTiet));
            if (ycHoanTraVatTu == null || ycHoanTraVatTu.DuocDuyet != null)
            {
                return NotFound();
            }
            foreach (var entityYeuCauTraVatTuChiTiet in ycHoanTraVatTu.YeuCauTraVatTuChiTiets)
            {
                entityYeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet
                        .SoLuongDaXuat =
                    entityYeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet
                        .SoLuongDaXuat - entityYeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.SoLuongXuat;
                entityYeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.WillDelete =
                    true;
                entityYeuCauTraVatTuChiTiet.XuatKhoVatTuChiTietViTri.WillDelete =
                    true;
                entityYeuCauTraVatTuChiTiet.WillDelete =
                    true;
            }
            await _ycHoanTraVatTuService.DeleteByIdAsync(id);
            return NoContent();
        }
    }
}
