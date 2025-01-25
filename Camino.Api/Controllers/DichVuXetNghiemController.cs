using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.DichVuXetNghiem;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DichVuXetNghiems;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DichVuXetNghiem;
using Camino.Core.Helpers;
using Camino.Services.DichVuKyThuatBenhVien;
using Camino.Services.DichVuXetNghiem;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Controllers
{

    public class DichVuXetNghiemController : CaminoBaseController
    {
        private readonly IDichVuXetNghiemService _dichVuXetNghiemService;
        private readonly IDichVuKyThuatBenhVienService _dichVuKyThuatBenhVienService;

        public DichVuXetNghiemController(IDichVuXetNghiemService dichVuXetNghiemService, IDichVuKyThuatBenhVienService dichVuKyThuatBenhVienService)
        {
            _dichVuXetNghiemService = dichVuXetNghiemService;
            _dichVuKyThuatBenhVienService = dichVuKyThuatBenhVienService;
        }
        #region Ds TreeView And Search

        [HttpPost("GetDataTreeView")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ChiSoXetNghiem)]
        public List<DichVuXetNghiemGridVo> GetDataTreeView([FromBody] QueryInfo queryInfo)
        {
            var gridData = _dichVuXetNghiemService.GetDataTreeView(queryInfo);
            return gridData.ToList();
        }
        [HttpPost("SearchDichVuXetNghiem")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ChiSoXetNghiem)]
        public List<DichVuXetNghiemGridVo> SearchDichVuXetNghiem([FromBody] QueryInfo queryInfo)
        {
            var gridData = _dichVuXetNghiemService.SearchDichVuXetNghiem(queryInfo);
            return gridData.ToList();
        }
        #endregion      


        [HttpPost("GetLoaiMau")]
        public ActionResult<ICollection<LookupItemVo>> GetLoaiMau(DropDownListRequestModel model)
        {
            var lookup = _dichVuXetNghiemService.GetLoaiMau(model);
            return Ok(lookup);
        }


        [HttpPost("MauMayXetNghiems")]
        public async Task<ActionResult<ICollection<MauMayXetNghiemLookup>>> MauMayXetNghiem(DropDownListRequestModel queryInfo)
        {
            var lookup = await _dichVuXetNghiemService.MauMayXetNghiemLookUp(queryInfo);
            return Ok(lookup);
        }
        #region XuLy

        #region Add
        [HttpPost("ThemChiSoXetNghiem")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.ChiSoXetNghiem)]
        public async Task<ActionResult> ThemChiSoXetNghiem(DichVuXetNghiemViewModel dichVuXetNghiemVM)
        {
            var dichVuXetNghiemId = dichVuXetNghiemVM.Id;
            dichVuXetNghiemVM.Id = 0;

            var dichVuXetNghiemKetNoiChiSos = new List<DichVuXetNghiemKetNoiChiSo>();
            if (dichVuXetNghiemVM.KetNoiChiSoXetNghiems != null && dichVuXetNghiemVM.KetNoiChiSoXetNghiems.Count > 0)
            {
                foreach (var item in dichVuXetNghiemVM.KetNoiChiSoXetNghiems
                                                    .Where(o => o.Id == 0 && !string.IsNullOrEmpty(o.MaChiSo) && !string.IsNullOrEmpty(o.TenKetNoi) && o.MauMayXetNghiemId != null))
                {

                    var dichVuXetNghiemKetNoiChiSoVM = new DichVuXetNghiemKetNoiChiSoViewModel
                    {
                        MaKetNoi = item.MaChiSo,
                        TenKetNoi = item.TenKetNoi,
                        MaChiSo = item.MaChiSo,
                        MauMayXetNghiemId = item.MauMayXetNghiemId,
                        HieuLuc = true,
                        TiLe = 1,
                        NotSendOrder = item.NotSendOrder
                    };
                    dichVuXetNghiemKetNoiChiSos.Add(dichVuXetNghiemKetNoiChiSoVM.ToEntity<DichVuXetNghiemKetNoiChiSo>());
                }
            }

            if (dichVuXetNghiemVM.CapDichVu == 1)
            {
                if (dichVuXetNghiemVM.CoChiSoXetNghiem == false && dichVuXetNghiemVM.IsCreateChild)
                {
                    var dichVuXetNghiemCha = new DichVuXetNghiem
                    {
                        NhomDichVuBenhVienId = dichVuXetNghiemVM.NhomDichVuBenhVienId.Value,
                        Ma = dichVuXetNghiemVM.Ma,
                        Ten = dichVuXetNghiemVM.TenDichVuKyThuat,
                        CapDichVu = dichVuXetNghiemVM.CapDichVu.Value,
                        HieuLuc = true,
                    };
                    var dichVuXetNghiemCon = dichVuXetNghiemVM.ToEntity<DichVuXetNghiem>();
                    var dichVuKyThuatBenhVienEntity = await _dichVuKyThuatBenhVienService.GetByIdAsync(dichVuXetNghiemVM.DichVuKyThuatBenhVienId);
                    if (dichVuXetNghiemKetNoiChiSos.Any())
                    {
                        foreach (var dichVuXetNghiemKetNoiChiSo in dichVuXetNghiemKetNoiChiSos)
                        {
                            dichVuXetNghiemCon.DichVuXetNghiemKetNoiChiSos.Add(dichVuXetNghiemKetNoiChiSo);
                        }
                    }
                    dichVuKyThuatBenhVienEntity.LoaiMauXetNghiem = dichVuXetNghiemVM.LoaiMauXetNghiem;
                    dichVuXetNghiemCon.CapDichVu = dichVuXetNghiemCha.CapDichVu + 1;
                    dichVuXetNghiemCha.DichVuXetNghiems.Add(dichVuXetNghiemCon);
                    dichVuKyThuatBenhVienEntity.DichVuXetNghiem = dichVuXetNghiemCha;
                    await _dichVuKyThuatBenhVienService.UpdateAsync(dichVuKyThuatBenhVienEntity);
                    return NoContent();
                }
                if (dichVuXetNghiemVM.IsCreateChild)
                {
                    dichVuXetNghiemVM.CapDichVu += 1;
                    var dichVuXetNghiem = dichVuXetNghiemVM.ToEntity<DichVuXetNghiem>();
                    if (dichVuXetNghiemKetNoiChiSos.Any())
                    {
                        foreach (var dichVuXetNghiemKetNoiChiSo in dichVuXetNghiemKetNoiChiSos)
                        {
                            dichVuXetNghiem.DichVuXetNghiemKetNoiChiSos.Add(dichVuXetNghiemKetNoiChiSo);
                        }
                    }
                    var dichVuKyThuatBenhVienEntity = await _dichVuKyThuatBenhVienService.GetByIdAsync(dichVuXetNghiemVM.DichVuKyThuatBenhVienId, s => s.Include(p => p.DichVuXetNghiem).ThenInclude(p => p.DichVuXetNghiems));
                    dichVuKyThuatBenhVienEntity.LoaiMauXetNghiem = dichVuXetNghiemVM.LoaiMauXetNghiem;
                    dichVuKyThuatBenhVienEntity.DichVuXetNghiem.DichVuXetNghiems.Add(dichVuXetNghiem);
                    await _dichVuKyThuatBenhVienService.UpdateAsync(dichVuKyThuatBenhVienEntity);
                }

                else
                {
                    var dichVuXetNghiem = dichVuXetNghiemVM.ToEntity<DichVuXetNghiem>();
                    var dichVuKyThuatBenhVienEntity = await _dichVuKyThuatBenhVienService.GetByIdAsync(dichVuXetNghiemVM.DichVuKyThuatBenhVienId,
                                                     s => s.Include(p => p.DichVuXetNghiem).ThenInclude(p => p.DichVuXetNghiemKetNoiChiSos));
                    dichVuKyThuatBenhVienEntity.LoaiMauXetNghiem = dichVuXetNghiemVM.LoaiMauXetNghiem;
                    dichVuKyThuatBenhVienEntity.DichVuXetNghiem = dichVuXetNghiem;

                    if (dichVuXetNghiemKetNoiChiSos.Any())
                    {
                        foreach (var dichVuXetNghiemKetNoiChiSo in dichVuXetNghiemKetNoiChiSos)
                        {
                            dichVuKyThuatBenhVienEntity.DichVuXetNghiem.DichVuXetNghiemKetNoiChiSos.Add(dichVuXetNghiemKetNoiChiSo);
                        }
                    }
                    await _dichVuKyThuatBenhVienService.UpdateAsync(dichVuKyThuatBenhVienEntity);
                }
            }
            else
            {
                dichVuXetNghiemVM.CapDichVu += 1;
                var dichVuXetNghiemCon = dichVuXetNghiemVM.ToEntity<DichVuXetNghiem>();

                if (dichVuXetNghiemKetNoiChiSos.Any())
                {
                    foreach (var dichVuXetNghiemKetNoiChiSo in dichVuXetNghiemKetNoiChiSos)
                    {
                        dichVuXetNghiemCon.DichVuXetNghiemKetNoiChiSos.Add(dichVuXetNghiemKetNoiChiSo);
                    }
                }
                var dichVuKyThuatBenhVienEntity = _dichVuXetNghiemService.DichVuKyThuatBenhVienEntity(dichVuXetNghiemVM.DichVuXetNghiemId.Value, dichVuXetNghiemVM.NhomDichVuBenhVienId.Value);
                dichVuKyThuatBenhVienEntity.LoaiMauXetNghiem = dichVuXetNghiemVM.LoaiMauXetNghiem;
                dichVuXetNghiemCon.DichVuXetNghiemChaId = dichVuXetNghiemId;
                var dichVuXetNghiem = dichVuKyThuatBenhVienEntity.DichVuXetNghiem.DichVuXetNghiems.Where(p => p.Id == dichVuXetNghiemId).FirstOrDefault();
                dichVuXetNghiem.DichVuXetNghiems.Add(dichVuXetNghiemCon);
                await _dichVuKyThuatBenhVienService.UpdateAsync(dichVuKyThuatBenhVienEntity);
            }
            return NoContent();
        }
        #endregion

        #region Get
        [HttpGet("GetChiSoXetNghiem")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ChiSoXetNghiem)]
        public async Task<ActionResult<DichVuXetNghiemViewModel>> Get(long id, long dichVuXetNghiemId, string chiSoCha)
        {
            var dichVuXetNghiem = await _dichVuXetNghiemService.GetByIdAsync(id, s => s.Include(p => p.DichVuXetNghiemKetNoiChiSos).Include(p=>p.HDPP));
            var mauMayXetNghiems = _dichVuXetNghiemService.MauMayXetNghiemGridVo(id);
            var model = dichVuXetNghiem.ToModel<DichVuXetNghiemViewModel>();
            var entity = _dichVuXetNghiemService.DichVuKyThuatBenhVienEntity(dichVuXetNghiemId, dichVuXetNghiem.NhomDichVuBenhVienId);
            model.TenLoaiMauXetNghiem = entity.LoaiMauXetNghiem?.GetDescription();
            model.LoaiMauXetNghiem = entity.LoaiMauXetNghiem;
            model.TenCha = entity.NhomDichVuBenhVien.Ten;
            model.ChiSoCha = chiSoCha;
            model.TenDichVuKyThuat = entity.Ten;
            model.HdppName = dichVuXetNghiem.HDPP?.Ten;

            model.KetNoiChiSoXetNghiems = mauMayXetNghiems.Select(o => new KetNoiChiSoXetNghiemViewModel
            {
                Id = o.Id,
                TenKetNoi = o?.TenKetNoi,
                MaChiSo = o?.MaChiSo,
                TenMauMayXetNghiem = o?.TenMauMayXN,
                MauMayXetNghiemId = o?.MauMayXetNghiemId,
                NotSendOrder = o?.NotSendOrder

            }).ToList();
            model.CoChiSoXetNghiem = true;
            return Ok(model);
        }

        [HttpPost("GetChuaChiSoXetNghiem")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ChiSoXetNghiem)]
        public ActionResult GetChuaChiSoXetNghiem(ChuaCoDichVuXetNghiemViewModel viewModel)
        {

            return Ok(viewModel);
        }

        #endregion

        #region Update
        [HttpPost("CapNhatChiSoXetNghiem")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.ChiSoXetNghiem)]
        public async Task<ActionResult> CapNhatChiSoXetNghiem(DichVuXetNghiemViewModel dichVuXetNghiemVM)
        {
            var dichVuXetNghiemKetNoiChiSos = new List<DichVuXetNghiemKetNoiChiSo>();
            if (dichVuXetNghiemVM.KetNoiChiSoXetNghiems != null && dichVuXetNghiemVM.KetNoiChiSoXetNghiems.Count > 0)
            {
                foreach (var item in dichVuXetNghiemVM.KetNoiChiSoXetNghiems
                                                    .Where(o => !string.IsNullOrEmpty(o.MaChiSo) && !string.IsNullOrEmpty(o.TenKetNoi) && o.MauMayXetNghiemId != null))
                {
                    var dichVuXetNghiemKetNoiChiSoVM = new DichVuXetNghiemKetNoiChiSoViewModel
                    {
                        MaKetNoi = item.MaChiSo,
                        TenKetNoi = item.TenKetNoi,
                        MaChiSo = item.MaChiSo,
                        MauMayXetNghiemId = item.MauMayXetNghiemId,
                        HieuLuc = true,
                        TiLe = 1,
                        Id = item.Id,
                        NotSendOrder = item.NotSendOrder
                    };
                    dichVuXetNghiemKetNoiChiSos.Add(dichVuXetNghiemKetNoiChiSoVM.ToEntity<DichVuXetNghiemKetNoiChiSo>());
                }
            }
            if (dichVuXetNghiemVM.CapDichVu == 1)
            {
                var dichVuKyThuatBenhVien = _dichVuXetNghiemService.DichVuKyThuatBenhVienEntity(dichVuXetNghiemVM.Id, dichVuXetNghiemVM.NhomDichVuBenhVienId.Value);
                var dichVuXetNghiem = await _dichVuXetNghiemService.GetByIdAsync(dichVuXetNghiemVM.Id, s => s.Include(p => p.DichVuXetNghiemKetNoiChiSos));
                var dichVuXNKetNoiChiSos = dichVuXetNghiem.DichVuXetNghiemKetNoiChiSos.Where(p => p.HieuLuc == true);
                if (dichVuXNKetNoiChiSos.Any())
                {
                    //Kiểm tra chỉ số bị xóa
                    var chiSoXoa = dichVuXNKetNoiChiSos.Where(o => dichVuXetNghiemKetNoiChiSos.All(x => x.Id != o.Id));
                    //Kiểm tra chỉ số cập nhật
                    var chiSoCapNhat = dichVuXNKetNoiChiSos.Where(o => dichVuXetNghiemKetNoiChiSos.Any(x => x.Id == o.Id));
                    //Kiểm tra chỉ số them mới
                    var chiSoMoi = dichVuXetNghiemKetNoiChiSos.Where(o => dichVuXNKetNoiChiSos.All(x => x.Id != o.Id));
                    foreach (var itemXoa in chiSoXoa)
                    {
                        itemXoa.WillDelete = true;
                    }
                    foreach (var itemCapNhat in chiSoCapNhat)
                    {
                        var data = dichVuXetNghiemKetNoiChiSos.FirstOrDefault(o => o.Id == itemCapNhat.Id);
                        if (data != null)
                        {
                            itemCapNhat.MaKetNoi = data.MaChiSo;
                            itemCapNhat.TenKetNoi = data.TenKetNoi;
                            itemCapNhat.MaChiSo = data.MaChiSo;
                            itemCapNhat.MauMayXetNghiemId = data.MauMayXetNghiemId;
                            itemCapNhat.HieuLuc = true;
                            itemCapNhat.TiLe = 1;
                            itemCapNhat.NotSendOrder = data.NotSendOrder;
                        }
                    }
                    foreach (var itemMoi in chiSoMoi)
                    {
                        dichVuXetNghiem.DichVuXetNghiemKetNoiChiSos.Add(itemMoi);
                    }
                }
                else
                {

                    foreach (var itemMoi in dichVuXetNghiemKetNoiChiSos)
                    {
                        dichVuXetNghiem.DichVuXetNghiemKetNoiChiSos.Add(itemMoi);
                    }
                }
                dichVuKyThuatBenhVien.DichVuXetNghiem = dichVuXetNghiemVM.ToEntity(dichVuXetNghiem);
                dichVuKyThuatBenhVien.LoaiMauXetNghiem = dichVuXetNghiemVM.LoaiMauXetNghiem;
                await _dichVuKyThuatBenhVienService.UpdateAsync(dichVuKyThuatBenhVien);
            }
            else if (dichVuXetNghiemVM.CapDichVu == 2)
            {
                dichVuXetNghiemVM.Ten = dichVuXetNghiemVM.TenChiSo;
                var dichVuXetNghiems = await _dichVuXetNghiemService.DichVuXetNghiems(dichVuXetNghiemVM.DichVuXetNghiemChaId.Value, dichVuXetNghiemVM.Id, dichVuXetNghiemVM.NhomDichVuBenhVienId.Value);
                foreach (var item in dichVuXetNghiems)
                {
                    if (item.DichVuXetNghiemChaId == null)
                    {
                        item.DichVuKyThuatBenhViens.First().LoaiMauXetNghiem = dichVuXetNghiemVM.LoaiMauXetNghiem;
                    }
                    else
                    {
                        var dichVuXNKetNoiChiSos = item.DichVuXetNghiemKetNoiChiSos.Where(p => p.HieuLuc == true);
                        if (dichVuXNKetNoiChiSos.Any())
                        {
                            //Kiểm tra chỉ số bị xóa
                            var chiSoXoa = dichVuXNKetNoiChiSos.Where(o => dichVuXetNghiemKetNoiChiSos.All(x => x.Id != o.Id));
                            //Kiểm tra chỉ số cập nhật
                            var chiSoCapNhat = dichVuXNKetNoiChiSos.Where(o => dichVuXetNghiemKetNoiChiSos.Any(x => x.Id == o.Id));
                            //Kiểm tra chỉ số them mới
                            var chiSoMoi = dichVuXetNghiemKetNoiChiSos.Where(o => dichVuXNKetNoiChiSos.All(x => x.Id != o.Id));
                            foreach (var itemXoa in chiSoXoa)
                            {
                                itemXoa.WillDelete = true;
                            }
                            foreach (var itemCapNhat in chiSoCapNhat)
                            {
                                var data = dichVuXetNghiemKetNoiChiSos.FirstOrDefault(o => o.Id == itemCapNhat.Id);
                                if (data != null)
                                {
                                    itemCapNhat.MaKetNoi = data.MaChiSo;
                                    itemCapNhat.TenKetNoi = data.TenKetNoi;
                                    itemCapNhat.MaChiSo = data.MaChiSo;
                                    itemCapNhat.MauMayXetNghiemId = data.MauMayXetNghiemId;
                                    itemCapNhat.HieuLuc = true;
                                    itemCapNhat.TiLe = 1;
                                    itemCapNhat.NotSendOrder = data.NotSendOrder;
                                }
                            }
                            foreach (var itemMoi in chiSoMoi)
                            {
                                item.DichVuXetNghiemKetNoiChiSos.Add(itemMoi);
                            }
                        }
                        else
                        {

                            foreach (var itemMoi in dichVuXetNghiemKetNoiChiSos)
                            {
                                item.DichVuXetNghiemKetNoiChiSos.Add(itemMoi);
                            }
                        }
                        dichVuXetNghiemVM.ToEntity(item);
                    }
                }
                await _dichVuXetNghiemService.UpdateAsync(dichVuXetNghiems);
            }
            else
            {
                dichVuXetNghiemVM.Ten = dichVuXetNghiemVM.TenChiSo;
                var dichVuKyThuatBenhVien = _dichVuXetNghiemService.DichVuKyThuatBenhVienEntity(dichVuXetNghiemVM.DichVuXetNghiemId.Value, dichVuXetNghiemVM.NhomDichVuBenhVienId.Value);
                var dichVuXetNghiem = await _dichVuXetNghiemService.GetByIdAsync(dichVuXetNghiemVM.Id, s => s.Include(p => p.DichVuXetNghiemKetNoiChiSos));
                var dichVuXNKetNoiChiSos = dichVuXetNghiem.DichVuXetNghiemKetNoiChiSos.Where(p => p.HieuLuc == true);
                if (dichVuXNKetNoiChiSos.Any())
                {
                    //Kiểm tra chỉ số bị xóa
                    var chiSoXoa = dichVuXNKetNoiChiSos.Where(o => dichVuXetNghiemKetNoiChiSos.All(x => x.Id != o.Id));
                    //Kiểm tra chỉ số cập nhật
                    var chiSoCapNhat = dichVuXNKetNoiChiSos.Where(o => dichVuXetNghiemKetNoiChiSos.Any(x => x.Id == o.Id));
                    //Kiểm tra chỉ số them mới
                    var chiSoMoi = dichVuXetNghiemKetNoiChiSos.Where(o => dichVuXNKetNoiChiSos.All(x => x.Id != o.Id));
                    foreach (var itemXoa in chiSoXoa)
                    {
                        itemXoa.WillDelete = true;
                    }
                    foreach (var itemCapNhat in chiSoCapNhat)
                    {
                        var data = dichVuXetNghiemKetNoiChiSos.FirstOrDefault(o => o.Id == itemCapNhat.Id);
                        if (data != null)
                        {
                            itemCapNhat.MaKetNoi = data.MaChiSo;
                            itemCapNhat.TenKetNoi = data.TenKetNoi;
                            itemCapNhat.MaChiSo = data.MaChiSo;
                            itemCapNhat.MauMayXetNghiemId = data.MauMayXetNghiemId;
                            itemCapNhat.HieuLuc = true;
                            itemCapNhat.TiLe = 1;
                            itemCapNhat.NotSendOrder = data.NotSendOrder;
                        }
                    }
                    foreach (var itemMoi in chiSoMoi)
                    {
                        dichVuXetNghiem.DichVuXetNghiemKetNoiChiSos.Add(itemMoi);
                    }
                }
                else
                {

                    foreach (var itemMoi in dichVuXetNghiemKetNoiChiSos)
                    {
                        dichVuXetNghiem.DichVuXetNghiemKetNoiChiSos.Add(itemMoi);
                    }
                }
                dichVuXetNghiemVM.ToEntity(dichVuXetNghiem);
                dichVuKyThuatBenhVien.LoaiMauXetNghiem = dichVuXetNghiemVM.LoaiMauXetNghiem;
                await _dichVuKyThuatBenhVienService.UpdateAsync(dichVuKyThuatBenhVien);
            }

            return NoContent();
        }
        #endregion

        #region Delete
        [HttpPost("XoaChiSoXetNghiem")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.ChiSoXetNghiem)]
        public async Task<ActionResult> Delete(DichVuXetNghiemViewModel dichVuXetNghiemVM)
        {
            if (dichVuXetNghiemVM.CapDichVu == 3)
            {
                var entity = await _dichVuXetNghiemService.GetByIdAsync(dichVuXetNghiemVM.Id, s => s.Include(p => p.DichVuXetNghiemKetNoiChiSos));
                await _dichVuXetNghiemService.DeleteByIdAsync(dichVuXetNghiemVM.Id);
            }
            else
            {
                await _dichVuXetNghiemService.XoaDichVuXetNghiems(dichVuXetNghiemVM.Id);
            }
            return NoContent();
        }
        #endregion


        #region Export excel
        [HttpPost("ExportDataTreeView")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.KhamDoanHopDongKham)]
        public async Task<ActionResult> ExportDataTreeView()
        {
            var nhomDVXNs = await _dichVuXetNghiemService.NhomDichVuXetNghiems();
            var bytes = _dichVuXetNghiemService.ExportExportDataTreeViews(nhomDVXNs);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DSDichVuXetNghiem" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion
        #endregion
    }
}