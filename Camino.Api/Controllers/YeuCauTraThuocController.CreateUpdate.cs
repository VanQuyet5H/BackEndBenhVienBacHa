using System;
using System.Collections.Generic;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.Error;
using Camino.Api.Models.YeuCauHoanTraDuocPham;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.YeuCauTraDuocPhams;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauHoanTra;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Controllers
{
    public partial class YeuCauTraThuocController
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetAllDuocPhamData")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.YeuCauHoanTraDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetAllDuocPhamData([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _ycHoanTraDuocPhamService.GetAllDuocPhamData(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetAllDuocPhamTotal")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.YeuCauHoanTraDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetAllDuocPhamTotal([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _ycHoanTraDuocPhamService.GetAllDuocPhamTotal(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetKhoDuocPham")]
        public async Task<ActionResult> GetKhoDuocPham(DropDownListRequestModel model)
        {
            var result = await _ycHoanTraDuocPhamService.GetKhoDuocPham(model);
            return Ok(result);
        }

        [HttpPost("GetKhoDuocHoanTra")]
        public async Task<ActionResult> GetKhoDuocHoanTra(DropDownListRequestModel model)
        {
            var result = await _ycHoanTraDuocPhamService.GetKhoDuocHoanTra(model);
            return Ok(result);
        }



        [HttpPost("GetKhoLoaiDuocPham")]
        public async Task<ActionResult> GetKhoLoaiDuocPham(DropDownListRequestModel model)
        {
            var result = await _ycHoanTraDuocPhamService.GetKhoLoaiDuocPham(model);
            return Ok(result);
        }

        [HttpPost("GetKhoLoaiDuocHoanTra")]
        public async Task<ActionResult> GetKhoLoaiDuocHoanTra(DropDownListRequestModel model)
        {
            var result = await _ycHoanTraDuocPhamService.GetKhoLoaiDuocHoanTra(model);
            return Ok(result);
        }


        [HttpPost("GetDuocPhamOnGroup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> GetDuocPhamOnGroup([FromBody] GetDuocPhamOnGroupModel model)
        {
            var result = await _ycHoanTraDuocPhamService.GetDuocPhamOnGroup(model.Id, model.KhoXuatId, model.SearchString, model.ListDaChon);
            return Ok(result);
        }

        //[HttpPost]
        [HttpPost("GuiPhieuHoanTraDuocPham")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.YeuCauHoanTraDuocPham)]
        public async Task<ActionResult> Post
           ([FromBody] YeuCauHoanTraDuocPhamViewModel model)
        {
            if (!model.YeuCauTraDuocPhamChiTiets.Any())
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.YeuCauTraDuocPhamChiTiet.Required"));

            }

            if ((model.KhoXuatId != null && !await _xuatKhoService.IsKhoExists(model.KhoXuatId ?? 0))
                 || (model.KhoNhapId != null && !await _xuatKhoService.IsKhoExists(model.KhoNhapId ?? 0)))
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.KhoDuocPham.NotExists"));
            }

            if (model.YeuCauTraDuocPhamChiTiets.Any(p => p.SoLuongXuat > p.SoLuongTon))
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.KhoDuocPhamChiTiet.SoLuongTonMoreThanSoLuongXuat"));
            }

            if (model.YeuCauTraDuocPhamChiTiets.Any(p => p.SoLuongXuat == 0 || p.SoLuongXuat == null))
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.KhoDuocPhamChiTiet.SoLuongXuatMoreThan0"));
            }

            //foreach (var item in model.YeuCauTraDuocPhamChiTiets)
            //{
            //    //item.NgayXuat = model.NgayXuat;
            //    var modelThemDuocPhamHoanTra = new ThemDuocPhamHoanTra
            //    {
            //        //ChatLuong = item.DatChatLuong == true ? 1 : 0,
            //        NhapKhoDuocPhamChiTietId = long.Parse(item.Id.Split(",")[0]),
            //        NhomDuocPhamId = long.Parse(item.Id.Split(",")[1]),
            //        SoLuongXuat = item.SoLuongXuat,
            //        KhoId = model.KhoXuatId,
            //        LaDuocPhamBHYT = item.LaDuocPhamBHYT,
            //    };
            //    //var xuatKhoDuocPhamChiTiet = await _xuatKhoService.GetDuocPham(modelThemDuocPhamHoanTra);

            //    //if (xuatKhoDuocPhamChiTiet == null)
            //    //{
            //    //    throw new ApiException(_localizationService.GetResource("XuatKho.ThemDuocPham.SoLuongValidate"));
            //    //}

            //    //item.XuatKhoDuocPhamChiTietViTris = new System.Collections.Generic.List<XuatKhoDuocPhamChiTietViTriViewModel>();
            //    //foreach (var viTri in xuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris)
            //    //{
            //    //    var viTriAdd = new XuatKhoDuocPhamChiTietViTriViewModel
            //    //    {
            //    //        SoLuongXuat = viTri.SoLuongXuat,
            //    //        NhapKhoDuocPhamChiTietId = viTri.NhapKhoDuocPhamChiTietId,
            //    //        NgayXuat = model.NgayXuat,
            //    //    };
            //    //    item.XuatKhoDuocPhamChiTietViTris.Add(viTriAdd);
            //    //}

            //}

            var entity = model.ToEntity<YeuCauTraDuocPham>();

            foreach (var item in model.YeuCauTraDuocPhamChiTiets)
            {
                var modelThemDuocPhamHoanTra = new ThemDuocPhamHoanTra
                {
                    //ChatLuong = item.DatChatLuong == true ? 1 : 0,
                    NhapKhoDuocPhamChiTietId = long.Parse(item.Id.Split(",")[0]),
                    NhomDuocPhamId = !string.IsNullOrEmpty(item.Id.Split(",")[1]) ? long.Parse(item.Id.Split(",")[1]) : (long?)null,
                    SoLuongXuat = item.SoLuongXuat,
                    KhoId = model.KhoXuatId,
                    LaDuocPhamBHYT = item.LaDuocPhamBHYT,
                };

                var xuatKhoDuocPhamChiTiet = await _ycHoanTraDuocPhamService.GetDuocPham(modelThemDuocPhamHoanTra);

                entity.YeuCauTraDuocPhamChiTiets.Add(xuatKhoDuocPhamChiTiet);
            }

            _ycHoanTraDuocPhamService.Add(entity);

            //update gia cho NhapKhoChiTiet
            foreach (var item in model.YeuCauTraDuocPhamChiTiets)
            {
                var nhapKhoDuocPhamChiTietIdUpdate = long.Parse(item.Id.Split(",")[0]);
                await _ycHoanTraDuocPhamService.UpdateGiaChoNhapKhoChiTiet(item.SoLuongXuat ?? 0, nhapKhoDuocPhamChiTietIdUpdate);
            }

            //throw new ApiException(_localizationService.GetResource("XuatKho.KhoDuocPhamChiTiet.SoLuongXuatMoreThan0"));

            return Ok(entity.Id);
        }


        [HttpGet("GetUpdate")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.YeuCauHoanTraDuocPham)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> GetUpdate(long id)
        {
            var entity = await _ycHoanTraDuocPhamService.GetByIdAsync(id,
                                        p => p.Include(x => x.YeuCauTraDuocPhamChiTiets).ThenInclude(x => x.XuatKhoDuocPhamChiTietViTri).ThenInclude(x => x.NhapKhoDuocPhamChiTiet)
                                       .Include(x => x.YeuCauTraDuocPhamChiTiets).ThenInclude(x => x.DuocPhamBenhVien).ThenInclude(x => x.DuocPham)
                                       .Include(x => x.YeuCauTraDuocPhamChiTiets).ThenInclude(x => x.DuocPhamBenhVien).ThenInclude(x => x.DuocPham).ThenInclude(x => x.DonViTinh)
                                       .Include(x => x.YeuCauTraDuocPhamChiTiets).ThenInclude(x => x.DuocPhamBenhVien).ThenInclude(x => x.DuocPhamBenhVienPhanNhom)
                                       .Include(x => x.YeuCauTraDuocPhamChiTiets).ThenInclude(x => x.DuocPhamBenhVienPhanNhom)
                                       .Include(x => x.KhoXuat)
                                       .Include(x => x.KhoNhap)
                                            );
            var result = entity.ToModel<YeuCauHoanTraDuocPhamViewModel>();
            result.TenKhoCanHoanTra = entity.KhoXuat.Ten;
            result.TenKhoNhanHoanTra = entity.KhoNhap.Ten;

            foreach (var yeuCauTraDuocPhamChiTiet in entity.YeuCauTraDuocPhamChiTiets)
            {
                var chiTiet = result.YeuCauTraDuocPhamChiTiets.FirstOrDefault(p => p.Id == yeuCauTraDuocPhamChiTiet.Id + "");
                if (chiTiet != null)
                {
                    chiTiet.TenDuocPham = yeuCauTraDuocPhamChiTiet.DuocPhamBenhVien.DuocPham.Ten;
                    chiTiet.MaDuocPham = yeuCauTraDuocPhamChiTiet.DuocPhamBenhVien.Ma;
                    chiTiet.SoDangKy = yeuCauTraDuocPhamChiTiet.DuocPhamBenhVien.DuocPham.SoDangKy;
                    chiTiet.DVT = yeuCauTraDuocPhamChiTiet.DuocPhamBenhVien.DuocPham.DonViTinh.Ten;
                    chiTiet.LaDuocPhamBHYT = yeuCauTraDuocPhamChiTiet.LaDuocPhamBHYT;
                    chiTiet.SoLo = yeuCauTraDuocPhamChiTiet.Solo;
                    chiTiet.SoLuongTon =
                        (yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri?.NhapKhoDuocPhamChiTiet.SoLuongNhap ?? 0) -
                        (yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri?.NhapKhoDuocPhamChiTiet.SoLuongDaXuat ?? 0);
                    chiTiet.HanSuDung = yeuCauTraDuocPhamChiTiet.HanSuDung;
                    chiTiet.NgayNhap = yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri?.NhapKhoDuocPhamChiTiet
                        .NgayNhap;

                    chiTiet.TenNhom = yeuCauTraDuocPhamChiTiet.DuocPhamBenhVienPhanNhom?.Ten ??
                                      yeuCauTraDuocPhamChiTiet.DuocPhamBenhVien?.DuocPhamBenhVienPhanNhom?.Ten ?? "CHƯA PHÂN NHÓM";
                    chiTiet.SoLuongXuat = yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri?.SoLuongXuat ?? 0;
                    chiTiet.Id = yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri?.NhapKhoDuocPhamChiTietId + "," +
                                 (yeuCauTraDuocPhamChiTiet.DuocPhamBenhVienPhanNhomId ?? yeuCauTraDuocPhamChiTiet.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId) + "," +
                                 (yeuCauTraDuocPhamChiTiet.LaDuocPhamBHYT ? "true" : "false");
                }
            }

            return Ok(result);
        }

        //[HttpPut]
        [HttpPost("GuiLaiPhieuHoanTraDuocPham")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.YeuCauHoanTraDuocPham)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> Update([FromBody] YeuCauHoanTraDuocPhamViewModel model)
        {
            if (!model.YeuCauTraDuocPhamChiTiets.Any())
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.KhoDuocPhamChiTiet.Required"));

            }

            if (model.KhoXuatId != null && !await _xuatKhoService.IsKhoExists(model.KhoXuatId ?? 0)
                || model.KhoNhapId != null && !await _xuatKhoService.IsKhoExists(model.KhoNhapId ?? 0))
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.KhoDuocPham.NotExists"));
            }

            var lstModelThemDuocPhamHoanTra = new List<ThemDuocPhamHoanTra>();
            foreach (var item in model.YeuCauTraDuocPhamChiTiets)
            {
                var nhapKhoDuocPhamChiTietId = long.Parse(item.Id.Split(",")[0]);
                var modelThemDuocPhamHoanTra = new ThemDuocPhamHoanTra
                {
                    NhapKhoDuocPhamChiTietId = nhapKhoDuocPhamChiTietId,
                    NhomDuocPhamId = !string.IsNullOrEmpty(item.Id.Split(",")[1]) ? long.Parse(item.Id.Split(",")[1]) : (long?)null,
                    SoLuongXuat = item.SoLuongXuat,
                    KhoId = model.KhoXuatId,
                    LaDuocPhamBHYT = item.LaDuocPhamBHYT,
                    SoLuongTon = item.SoLuongTon
                };
                lstModelThemDuocPhamHoanTra.Add(modelThemDuocPhamHoanTra);
            }

            if (await _ycHoanTraDuocPhamService.CheckValidSlTon(lstModelThemDuocPhamHoanTra, model.Id) == false)
            {
                throw new ApiException(_localizationService.GetResource("XuatKho.KhoDuocPhamChiTiet.SoLuongTonMoreThanSoLuongXuat"));
            }

            var entity = await _ycHoanTraDuocPhamService.GetByIdAsync(model.Id,
                u => u.Include(x => x.YeuCauTraDuocPhamChiTiets).ThenInclude(x => x.XuatKhoDuocPhamChiTietViTri).ThenInclude(x => x.XuatKhoDuocPhamChiTiet)
                    .Include(x => x.YeuCauTraDuocPhamChiTiets).ThenInclude(x => x.XuatKhoDuocPhamChiTietViTri).ThenInclude(x => x.NhapKhoDuocPhamChiTiet)
                );

            entity.KhoNhapId = model.KhoNhapId.GetValueOrDefault();
            entity.GhiChu = model.GhiChu;


            foreach (var item in model.YeuCauTraDuocPhamChiTiets)
            {
                var id = long.Parse(item.Id.Split(",")[0]);

                if (entity.YeuCauTraDuocPhamChiTiets.All(c => c.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTietId != id))
                {
                    var modelThemDuocPhamHoanTra = new ThemDuocPhamHoanTra
                    {
                        NhapKhoDuocPhamChiTietId = long.Parse(item.Id.Split(",")[0]),
                        NhomDuocPhamId = !string.IsNullOrEmpty(item.Id.Split(",")[1]) ? long.Parse(item.Id.Split(",")[1]) : (long?)null,
                        SoLuongXuat = item.SoLuongXuat,
                        KhoId = model.KhoXuatId,
                        LaDuocPhamBHYT = item.LaDuocPhamBHYT
                    };

                    var xuatKhoDuocPhamChiTiet = await _ycHoanTraDuocPhamService.GetDuocPham(modelThemDuocPhamHoanTra);

                    entity.YeuCauTraDuocPhamChiTiets.Add(xuatKhoDuocPhamChiTiet);
                    await _ycHoanTraDuocPhamService.UpdateSlXuatNhapKhoChiTiet(item.SoLuongXuat ?? 0, id);
                }
                else
                {
                    if (!entity.YeuCauTraDuocPhamChiTiets.Any()) continue;
                    foreach (var ycTraDpModify in entity.YeuCauTraDuocPhamChiTiets.Where(c => c.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTietId == id))
                    {
                        ycTraDpModify.SoLuongTra = item.SoLuongXuat.GetValueOrDefault();
                        ycTraDpModify.XuatKhoDuocPhamChiTietViTri.NgayXuat = model.NgayYeuCau;
                        ycTraDpModify.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.NgayXuat = model.NgayYeuCau;
                        ycTraDpModify.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat =
                            ycTraDpModify.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat -
                            ycTraDpModify.XuatKhoDuocPhamChiTietViTri.SoLuongXuat +
                            item.SoLuongXuat.GetValueOrDefault();
                        ycTraDpModify.XuatKhoDuocPhamChiTietViTri.SoLuongXuat = item.SoLuongXuat.GetValueOrDefault();
                    }
                }
            }

            var lstEntityCanXoa = entity.YeuCauTraDuocPhamChiTiets
                .Select(c => c.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTietId)
                .Where(q => !model.YeuCauTraDuocPhamChiTiets.Select(c => long.Parse(c.Id.Split(",")[0])).Contains(q) &&
                            q != 0);

            foreach (var entityYeuCauTraDuocPhamChiTiet in lstEntityCanXoa)
            {
                if (entity.YeuCauTraDuocPhamChiTiets.Any(c => c.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTietId == entityYeuCauTraDuocPhamChiTiet))
                {
                    var entityYcTraDpChiTiet = entity.YeuCauTraDuocPhamChiTiets.First(c =>
                        c.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTietId == entityYeuCauTraDuocPhamChiTiet);

                    entityYcTraDpChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet
                            .SoLuongDaXuat =
                        entityYcTraDpChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet
                            .SoLuongDaXuat - entityYcTraDpChiTiet.XuatKhoDuocPhamChiTietViTri.SoLuongXuat;
                    entityYcTraDpChiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.WillDelete =
                        true;
                    entityYcTraDpChiTiet.XuatKhoDuocPhamChiTietViTri.WillDelete =
                        true;
                    entityYcTraDpChiTiet.WillDelete =
                        true;
                }
            }

            await _ycHoanTraDuocPhamService.UpdateAsync(entity);

            return Ok(entity.Id);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("SaveEveryMovingThuoc")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.YeuCauHoanTraDuocPham)]
        public async Task<ActionResult<YeuCauHoanTraDuocPhamViewModel>> SaveEveryMovingThuoc([FromBody] YeuCauHoanTraDuocPhamViewModelIgnoreValidate model)
        {
            var lstModelThemDuocPhamHoanTra = new List<ThemDuocPhamHoanTra>();
            foreach (var item in model.YeuCauTraDuocPhamChiTiets)
            {
                var nhapKhoDuocPhamChiTietId = long.Parse(item.Id.Split(",")[0]);
                var modelThemDuocPhamHoanTra = new ThemDuocPhamHoanTra
                {
                    NhapKhoDuocPhamChiTietId = nhapKhoDuocPhamChiTietId,
                    NhomDuocPhamId = !string.IsNullOrEmpty(item.Id.Split(",")[1]) ? long.Parse(item.Id.Split(",")[1]) : (long?)null,
                    SoLuongXuat = item.SoLuongXuat,
                    KhoId = model.KhoXuatId,
                    LaDuocPhamBHYT = item.LaDuocPhamBHYT,
                    SoLuongTon = item.SoLuongTon
                };
                lstModelThemDuocPhamHoanTra.Add(modelThemDuocPhamHoanTra);
            }

            var entity = await _ycHoanTraDuocPhamService.GetByIdAsync(model.Id,
                u => u.Include(x => x.YeuCauTraDuocPhamChiTiets).ThenInclude(x => x.XuatKhoDuocPhamChiTietViTri).ThenInclude(x => x.XuatKhoDuocPhamChiTiet)
                    .Include(x => x.YeuCauTraDuocPhamChiTiets).ThenInclude(x => x.XuatKhoDuocPhamChiTietViTri).ThenInclude(x => x.NhapKhoDuocPhamChiTiet)
                );


            foreach (var item in model.YeuCauTraDuocPhamChiTiets)
            {
                var id = long.Parse(item.Id.Split(",")[0]);

                if (entity.YeuCauTraDuocPhamChiTiets.All(c => c.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTietId != id))
                {
                    var modelThemDuocPhamHoanTra = new ThemDuocPhamHoanTra
                    {
                        NhapKhoDuocPhamChiTietId = long.Parse(item.Id.Split(",")[0]),
                        NhomDuocPhamId = !string.IsNullOrEmpty(item.Id.Split(",")[1]) ? long.Parse(item.Id.Split(",")[1]) : (long?)null,
                        SoLuongXuat = item.SoLuongXuat,
                        KhoId = model.KhoXuatId,
                        LaDuocPhamBHYT = item.LaDuocPhamBHYT
                    };

                    var xuatKhoDuocPhamChiTiet = await _ycHoanTraDuocPhamService.GetDuocPham(modelThemDuocPhamHoanTra);

                    entity.YeuCauTraDuocPhamChiTiets.Add(xuatKhoDuocPhamChiTiet);
                    await _ycHoanTraDuocPhamService.UpdateSlXuatNhapKhoChiTiet(item.SoLuongXuat ?? 0, id);
                }
                else
                {
                    if (!entity.YeuCauTraDuocPhamChiTiets.Any()) continue;
                    foreach (var ycTraDpModify in entity.YeuCauTraDuocPhamChiTiets.Where(c => c.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTietId == id))
                    {
                        ycTraDpModify.SoLuongTra = item.SoLuongXuat.GetValueOrDefault();
                        ycTraDpModify.XuatKhoDuocPhamChiTietViTri.NgayXuat = model.NgayYeuCau;
                        ycTraDpModify.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.NgayXuat = model.NgayYeuCau;
                        ycTraDpModify.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat =
                            ycTraDpModify.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat -
                            ycTraDpModify.XuatKhoDuocPhamChiTietViTri.SoLuongXuat +
                            item.SoLuongXuat.GetValueOrDefault();
                        ycTraDpModify.XuatKhoDuocPhamChiTietViTri.SoLuongXuat = item.SoLuongXuat.GetValueOrDefault();
                    }
                }
            }

            var lstEntityCanXoa = entity.YeuCauTraDuocPhamChiTiets
                .Select(c => c.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTietId)
                .Where(q => !model.YeuCauTraDuocPhamChiTiets.Select(c => long.Parse(c.Id.Split(",")[0])).Contains(q) &&
                            q != 0);

            foreach (var entityYeuCauTraDuocPhamChiTiet in lstEntityCanXoa)
            {
                if (entity.YeuCauTraDuocPhamChiTiets.Any(c => c.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTietId == entityYeuCauTraDuocPhamChiTiet))
                {
                    var entityYcTraDpChiTiet = entity.YeuCauTraDuocPhamChiTiets.First(c =>
                        c.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTietId == entityYeuCauTraDuocPhamChiTiet);

                    entityYcTraDpChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet
                            .SoLuongDaXuat =
                        entityYcTraDpChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet
                            .SoLuongDaXuat - entityYcTraDpChiTiet.XuatKhoDuocPhamChiTietViTri.SoLuongXuat;
                    entityYcTraDpChiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.WillDelete =
                        true;
                    entityYcTraDpChiTiet.XuatKhoDuocPhamChiTietViTri.WillDelete =
                        true;
                    entityYcTraDpChiTiet.WillDelete =
                        true;
                }
            }

            await _ycHoanTraDuocPhamService.UpdateAsync(entity);

            var entityResult = await _ycHoanTraDuocPhamService.GetByIdAsync(model.Id,
                                        p => p.Include(x => x.YeuCauTraDuocPhamChiTiets).ThenInclude(x => x.XuatKhoDuocPhamChiTietViTri).ThenInclude(x => x.NhapKhoDuocPhamChiTiet)
                                       .Include(x => x.YeuCauTraDuocPhamChiTiets).ThenInclude(x => x.DuocPhamBenhVien).ThenInclude(x => x.DuocPham)
                                       .Include(x => x.YeuCauTraDuocPhamChiTiets).ThenInclude(x => x.DuocPhamBenhVien).ThenInclude(x => x.DuocPham).ThenInclude(x => x.DonViTinh)

                                       .Include(x => x.YeuCauTraDuocPhamChiTiets).ThenInclude(x => x.DuocPhamBenhVienPhanNhom)
                                            );
            var result = entityResult.ToModel<YeuCauHoanTraDuocPhamViewModel>();
            foreach (var yeuCauTraDuocPhamChiTiet in entityResult.YeuCauTraDuocPhamChiTiets)
            {
                var chiTiet = result.YeuCauTraDuocPhamChiTiets.FirstOrDefault(p => p.Id == yeuCauTraDuocPhamChiTiet.Id + "");
                if (chiTiet != null)
                {
                    chiTiet.TenDuocPham = yeuCauTraDuocPhamChiTiet.DuocPhamBenhVien.DuocPham.Ten;
                    chiTiet.MaDuocPham = yeuCauTraDuocPhamChiTiet.DuocPhamBenhVien.Ma;
                    chiTiet.SoDangKy = yeuCauTraDuocPhamChiTiet.DuocPhamBenhVien.DuocPham.SoDangKy;
                    chiTiet.DVT = yeuCauTraDuocPhamChiTiet.DuocPhamBenhVien.DuocPham.DonViTinh.Ten;
                    chiTiet.LaDuocPhamBHYT = yeuCauTraDuocPhamChiTiet.LaDuocPhamBHYT;
                    chiTiet.SoLo = yeuCauTraDuocPhamChiTiet.Solo;
                    chiTiet.SoLuongTon =
                        (yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri?.NhapKhoDuocPhamChiTiet.SoLuongNhap ?? 0) -
                        (yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri?.NhapKhoDuocPhamChiTiet.SoLuongDaXuat ?? 0);
                    chiTiet.HanSuDung = yeuCauTraDuocPhamChiTiet.HanSuDung;
                    chiTiet.NgayNhap = yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri?.NhapKhoDuocPhamChiTiet
                        .NgayNhap;

                    chiTiet.TenNhom = yeuCauTraDuocPhamChiTiet.DuocPhamBenhVienPhanNhom?.Ten;
                    chiTiet.SoLuongXuat = yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri?.SoLuongXuat ?? 0;
                    chiTiet.Id = yeuCauTraDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTri?.NhapKhoDuocPhamChiTietId + "," +
                                 yeuCauTraDuocPhamChiTiet.DuocPhamBenhVienPhanNhomId + "," +
                                 (yeuCauTraDuocPhamChiTiet.LaDuocPhamBHYT ? "true" : "false");
                }
            }

            return Ok(result);
        }
    }
}
