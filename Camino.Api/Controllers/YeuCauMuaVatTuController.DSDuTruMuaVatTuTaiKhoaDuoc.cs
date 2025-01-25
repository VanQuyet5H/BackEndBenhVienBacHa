using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DuTruMuaDuocPhamTaiKhoa;
using Camino.Core.Domain.ValueObject.DuTruMuaVatTuTaiKhoa;
using Camino.Core.Domain.ValueObject.DuTruMuaVatTuTaiKhoaDuoc;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Services.YeuCauMuaVatTuDuTruTaiKhoaDuoc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Api.Models.DutruMuaVatTuTaiKhoaDuoc.DutruMuaVatTuTaiKhoaDuoc;

namespace Camino.Api.Controllers
{
    public partial class YeuCauMuaVatTuController
    {
        #region cho xu  lý
        #region DS chờ xử lý
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachDuyetMuaDuTruTaiKhoaDuocForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachDuyetMuaDuTruTaiKhoaDuocForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var catKyTu = queryInfo.Sort[0].Field.Split('/');
            if (catKyTu.Count() > 1)
            {
                queryInfo.Sort[0].Field = catKyTu[0] + catKyTu[1];
            }
            var gridData = await _yeuCauMuaDuTruVatTuService.GetDataDuTruMuaVatTuTaiKhoaDuocForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDanhSachDuyetMuaDuTruTaiKhoaDuocForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> GetTotalDanhSachDuyetMuaDuTruTaiKhoaDuocForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruVatTuService.GetDataDuTruMuaVatTuTaiKhoaDuocToTalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion

        #region child chờ xử lý
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataDuTruMuaVatTuTaiKhoaDuocChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> GetDataDuTruMuaVatTuTaiKhoaDuocChildForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruVatTuService.GetDataDuTruMuaVatTuTaiKhoaDuocChildForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDanhSachDuyetMuaDuTruTaiKhoaDuocChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> GetTotalDanhSachDuyetMuaDuTruTaiKhoaDuocChildForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruVatTuService.GetDataDuTruMuaVatTuTaiKhoaDuocToTalPageChildForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion
        #region child child chờ xử lý
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataDuTruMuaVatTuTaiKhoaDuocChildChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> GetDataDuTruMuaVatTuTaiKhoaDuocChildChildForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruVatTuService.GetDataDuTruMuaVatTuTaiKhoaDuocChildChildForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDanhSachDuyetMuaDuTruTaiKhoaDuocChildChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> GetTotalDanhSachDuyetMuaDuTruTaiKhoaDuocChildChildForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruVatTuService.GetDataDuTruMuaVatTuTaiKhoaDuocToTalPageChildChildForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion
        #region duyet -tu chối -hủy duyet ngoài danh sách
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetListDuTruMuaChiTiet")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> GetListDuTruMuaChiTiet(long muaDuTruVatTuId)
        {
            var gridData = _yeuCauMuaDuTruVatTuService.GetDuTruMuaVatTuChiTiet(muaDuTruVatTuId);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetListDuTruMuaKhoaChiTiet")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> GetListDuTruMuaKhoaChiTiet(long muaDuTruVatTuId)
        {
            var gridData = _yeuCauMuaDuTruVatTuService.GetDuTruMuaVatTuKhoaChiTiet(muaDuTruVatTuId);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("Duyet")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoaDuoc)]
        public async Task<ActionResult> Duyet(DuTruMuaVatTuTaiKhoaDuocDuyet duTruMuaVatTuTaiKhoaDuocDuyet)
        {
            if (duTruMuaVatTuTaiKhoaDuocDuyet.LoaiDuyet == true) // khoa
            {
                var duTruMuaVatTuTheoKhoaEntity = await _yeuCauMuaDuTruVatTuService.GetDuTruVatTuTheoKhoaByIdAsync(duTruMuaVatTuTaiKhoaDuocDuyet.Id);
                duTruMuaVatTuTaiKhoaDuocDuyet.ToEntity(duTruMuaVatTuTheoKhoaEntity);
                duTruMuaVatTuTheoKhoaEntity.KhoDuocDuyet = true;
                duTruMuaVatTuTheoKhoaEntity.NhanVienKhoDuocId = _userAgentHelper.GetCurrentUserId();
                duTruMuaVatTuTheoKhoaEntity.NgayKhoDuocDuyet = DateTime.Now;
                await _yeuCauMuaDuTruVatTuTheoKhoaService.UpdateAsync(duTruMuaVatTuTheoKhoaEntity);
                return Ok(duTruMuaVatTuTheoKhoaEntity.Id);
            }
            if (duTruMuaVatTuTaiKhoaDuocDuyet.LoaiDuyet == false) // kho
            {
                var duTruMuaVatTuEntity = await _yeuCauMuaDuTruVatTuService.GetByIdAsync(duTruMuaVatTuTaiKhoaDuocDuyet.Id, s => s.Include(p => p.DuTruMuaVatTuChiTiets));
                duTruMuaVatTuTaiKhoaDuocDuyet.ToEntity(duTruMuaVatTuEntity);
                duTruMuaVatTuEntity.TruongKhoaDuyet = true;
                duTruMuaVatTuEntity.TruongKhoaId = _userAgentHelper.GetCurrentUserId();
                duTruMuaVatTuEntity.NgayTruongKhoaDuyet = DateTime.Now;
                foreach (var duTruMuaVatTuChiTiet in duTruMuaVatTuEntity.DuTruMuaVatTuChiTiets)
                {
                    if (duTruMuaVatTuChiTiet.SoLuongDuTruTruongKhoaDuyet == null)
                    {
                        duTruMuaVatTuChiTiet.SoLuongDuTruTruongKhoaDuyet = duTruMuaVatTuChiTiet.SoLuongDuTru;
                    }
                }
                await _yeuCauMuaDuTruVatTuService.UpdateAsync(duTruMuaVatTuEntity);
                return Ok(duTruMuaVatTuEntity.Id);
            }
            return NotFound();
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("TuChoi")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoaDuoc)]
        public async Task<ActionResult> TuChoi(DuTruMuaVatTuTaiKhoaDuocDuyet duTruMuaVatTuTaiKhoaDuocDuyet)
        {
            if (duTruMuaVatTuTaiKhoaDuocDuyet.LoaiDuyet == true) // khoa
            {
                var duTruMuaVatTuTheoKhoaEntity = await _yeuCauMuaDuTruVatTuService.GetDuTruVatTuTheoKhoaByIdAsync(duTruMuaVatTuTaiKhoaDuocDuyet.Id);
                duTruMuaVatTuTaiKhoaDuocDuyet.ToEntity(duTruMuaVatTuTheoKhoaEntity);
                duTruMuaVatTuTheoKhoaEntity.KhoDuocDuyet = false;
                duTruMuaVatTuTheoKhoaEntity.LyDoKhoDuocTuChoi = duTruMuaVatTuTaiKhoaDuocDuyet.LyDoTruongKhoaTuChoi;
                duTruMuaVatTuTheoKhoaEntity.NhanVienKhoDuocId = _userAgentHelper.GetCurrentUserId();
                duTruMuaVatTuTheoKhoaEntity.NgayKhoDuocDuyet = DateTime.Now;
                await _yeuCauMuaDuTruVatTuTheoKhoaService.UpdateAsync(duTruMuaVatTuTheoKhoaEntity);
                return Ok(duTruMuaVatTuTheoKhoaEntity.Id);
            }
            if (duTruMuaVatTuTaiKhoaDuocDuyet.LoaiDuyet == false)
            {
                var duTruMuaVatTuEntity = await _yeuCauMuaDuTruVatTuService.GetByIdAsync(duTruMuaVatTuTaiKhoaDuocDuyet.Id, s => s.Include(p => p.DuTruMuaVatTuChiTiets));
                duTruMuaVatTuTaiKhoaDuocDuyet.ToEntity(duTruMuaVatTuEntity);
                duTruMuaVatTuEntity.TruongKhoaDuyet = false;
                duTruMuaVatTuEntity.LyDoTruongKhoaTuChoi = duTruMuaVatTuTaiKhoaDuocDuyet.LyDoTruongKhoaTuChoi;
                duTruMuaVatTuEntity.TruongKhoaId = _userAgentHelper.GetCurrentUserId();
                duTruMuaVatTuEntity.NgayTruongKhoaDuyet = DateTime.Now;
                await _yeuCauMuaDuTruVatTuService.UpdateAsync(duTruMuaVatTuEntity);
                return Ok(duTruMuaVatTuEntity.Id);
            }
            return NotFound();
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("HuyDuyet")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoaDuoc)]
        public async Task<ActionResult> HuyDuyet(HuyDuyetQueryInfo huyDuyetQueryInfo)
        {
            if (huyDuyetQueryInfo.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoLe)
            {
                var duTruMuaVatTuTheoKhoaEntity = await _yeuCauMuaDuTruVatTuService.GetDuTruVatTuTheoKhoaByIdAsync(huyDuyetQueryInfo.Id);
                if (duTruMuaVatTuTheoKhoaEntity != null)
                {
                    duTruMuaVatTuTheoKhoaEntity.KhoDuocDuyet = null;
                    duTruMuaVatTuTheoKhoaEntity.NgayKhoDuocDuyet = null;
                    foreach (var chiTiet in duTruMuaVatTuTheoKhoaEntity.DuTruMuaVatTuTheoKhoaChiTiets)
                    {
                        chiTiet.SoLuongDuTruKhoDuocDuyet = null;
                    }
                    await _yeuCauMuaDuTruVatTuTheoKhoaService.UpdateAsync(duTruMuaVatTuTheoKhoaEntity);
                    return Ok(duTruMuaVatTuTheoKhoaEntity.Id);
                }
            }
            else
            {
                var duTruMuaVatTuEntity = await _yeuCauMuaDuTruVatTuService.GetByIdAsync(huyDuyetQueryInfo.Id, s => s.Include(p => p.DuTruMuaVatTuChiTiets));
                if (duTruMuaVatTuEntity != null)
                {
                    duTruMuaVatTuEntity.TruongKhoaDuyet = null;
                    duTruMuaVatTuEntity.NgayTruongKhoaDuyet = null;
                    foreach (var chiTiet in duTruMuaVatTuEntity.DuTruMuaVatTuChiTiets)
                    {
                        chiTiet.SoLuongDuTruTruongKhoaDuyet = null;
                    }
                    await _yeuCauMuaDuTruVatTuService.UpdateAsync(duTruMuaVatTuEntity);
                    return Ok(duTruMuaVatTuEntity.Id);
                }

            }
            //            if (duTruMuaVatTuTaiKhoaDuocDuyet.LoaiDuyet == true) // khoa
            //            {
            //                var duTruMuaVatTuTheoKhoaEntity = await _yeuCauMuaDuTruVatTuService.GetDuTruVatTuTheoKhoaByIdAsync(duTruMuaVatTuTaiKhoaDuocDuyet.Id);
            //                duTruMuaVatTuTaiKhoaDuocDuyet.ToEntity(duTruMuaVatTuTheoKhoaEntity);
            //                duTruMuaVatTuTheoKhoaEntity.KhoDuocDuyet = null;
            //                duTruMuaVatTuTheoKhoaEntity.LyDoKhoDuocTuChoi = null;
            //                duTruMuaVatTuTheoKhoaEntity.NhanVienKhoDuocId = null;
            //                duTruMuaVatTuTheoKhoaEntity.NgayKhoDuocDuyet = null;
            //                await _yeuCauMuaDuTruVatTuTheoKhoaService.UpdateAsync(duTruMuaVatTuTheoKhoaEntity);
            //                return Ok(duTruMuaVatTuTheoKhoaEntity.Id);
            //            }
            //            if(duTruMuaVatTuTaiKhoaDuocDuyet.LoaiDuyet == false)
            //            {
            //                var duTruMuaVatTuEntity = await _yeuCauMuaDuTruVatTuService.GetByIdAsync(duTruMuaVatTuTaiKhoaDuocDuyet.Id, s => s.Include(p => p.DuTruMuaVatTuChiTiets));
            //                duTruMuaVatTuTaiKhoaDuocDuyet.ToEntity(duTruMuaVatTuEntity);
            //                duTruMuaVatTuEntity.TruongKhoaDuyet = null;
            //                duTruMuaVatTuEntity.TruongKhoaId = null;
            //                duTruMuaVatTuEntity.NgayTruongKhoaDuyet = null;
            //                foreach (var item in duTruMuaVatTuTaiKhoaDuocDuyet.ListDuTruMuaVatTuKhoDuocChiTiet)
            //                {
            //                    item.SoLuongDuTruTruongKhoaDuyet = null;
            //                }
            //                await _yeuCauMuaDuTruVatTuService.UpdateAsync(duTruMuaVatTuEntity);
            //                return Ok(duTruMuaVatTuEntity.Id);
            //            }
            return NotFound();
        }
        #endregion
        #region duyet -tu chối -hủy duyet trong danh sách
        [HttpPost("GetDataUpdate")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoaDuoc)]
        public async Task<ActionResult<DuTruMuaVatTuChiTietViewGridVo>> GetDataUpdate(long id, bool typeLoaiKho)
        {
            var result = _yeuCauMuaDuTruVatTuService.GetDataUpdate(id, typeLoaiKho);
            return Ok(result);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("DuyetDanhSach")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoaDuoc)]
        public async Task<ActionResult> DuyetDanhSach(DuTruMuaVatTuTaiKhoaDuocDuyet duTruMuaVatTuTaiKhoaDuocDuyet)
        {
            if (duTruMuaVatTuTaiKhoaDuocDuyet.LoaiDuyet == true) // khoa
            {
                var duTruMuaVatTuTheoKhoaEntity = await _yeuCauMuaDuTruVatTuService.GetDuTruVatTuTheoKhoaByIdAsync(duTruMuaVatTuTaiKhoaDuocDuyet.Id);
                duTruMuaVatTuTaiKhoaDuocDuyet.ToEntity(duTruMuaVatTuTheoKhoaEntity);
                duTruMuaVatTuTheoKhoaEntity.KhoDuocDuyet = true;
                duTruMuaVatTuTheoKhoaEntity.NhanVienKhoDuocId = _userAgentHelper.GetCurrentUserId();
                duTruMuaVatTuTheoKhoaEntity.NgayKhoDuocDuyet = DateTime.Now;
                await _yeuCauMuaDuTruVatTuTheoKhoaService.UpdateAsync(duTruMuaVatTuTheoKhoaEntity);
                return Ok(duTruMuaVatTuTheoKhoaEntity.Id);
            }
            if (duTruMuaVatTuTaiKhoaDuocDuyet.LoaiDuyet == false)
            {
                var duTruMuaVatTuEntity = await _yeuCauMuaDuTruVatTuService.GetByIdAsync(duTruMuaVatTuTaiKhoaDuocDuyet.Id, s => s.Include(p => p.DuTruMuaVatTuChiTiets));
                duTruMuaVatTuTaiKhoaDuocDuyet.ToEntity(duTruMuaVatTuEntity);
                duTruMuaVatTuEntity.TruongKhoaDuyet = true;
                duTruMuaVatTuEntity.TruongKhoaId = _userAgentHelper.GetCurrentUserId();
                duTruMuaVatTuEntity.NgayTruongKhoaDuyet = DateTime.Now;
                await _yeuCauMuaDuTruVatTuService.UpdateAsync(duTruMuaVatTuEntity);
                return Ok(duTruMuaVatTuEntity.Id);
            }
            return NotFound();
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("TuChoiDanhSach")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> TuChoiDanhSach(TuChoiQueryInfo TuChoiQueryInfo)
        {
            if (TuChoiQueryInfo.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoLe)
            {
                var duTruMuaVatTuTheoKhoaEntity = await _yeuCauMuaDuTruVatTuService.GetDuTruVatTuTheoKhoaByIdAsync(TuChoiQueryInfo.Id);
                if (duTruMuaVatTuTheoKhoaEntity != null)
                {
                    duTruMuaVatTuTheoKhoaEntity.KhoDuocDuyet = false;
                    duTruMuaVatTuTheoKhoaEntity.LyDoKhoDuocTuChoi = TuChoiQueryInfo.LyDoTuChoi;
                    duTruMuaVatTuTheoKhoaEntity.NhanVienKhoDuocId = _userAgentHelper.GetCurrentUserId();
                    duTruMuaVatTuTheoKhoaEntity.NgayKhoDuocDuyet = DateTime.Now;
                    foreach (var chiTiet in duTruMuaVatTuTheoKhoaEntity.DuTruMuaVatTuTheoKhoaChiTiets)
                    {
                        chiTiet.SoLuongDuTruKhoDuocDuyet = null;
                    }
                    await _yeuCauMuaDuTruVatTuTheoKhoaService.UpdateAsync(duTruMuaVatTuTheoKhoaEntity);
                    return Ok(duTruMuaVatTuTheoKhoaEntity.Id);
                }
            }
            else
            {
                var duTruMuaVatTuEntity = await _yeuCauMuaDuTruVatTuService.GetByIdAsync(TuChoiQueryInfo.Id, s => s.Include(p => p.DuTruMuaVatTuChiTiets));
                if (duTruMuaVatTuEntity != null)
                {
                    duTruMuaVatTuEntity.TruongKhoaDuyet = false;
                    duTruMuaVatTuEntity.LyDoTruongKhoaTuChoi = TuChoiQueryInfo.LyDoTuChoi;
                    duTruMuaVatTuEntity.TruongKhoaId = _userAgentHelper.GetCurrentUserId();
                    duTruMuaVatTuEntity.NgayTruongKhoaDuyet = DateTime.Now;
                    foreach (var chiTiet in duTruMuaVatTuEntity.DuTruMuaVatTuChiTiets)
                    {
                        chiTiet.SoLuongDuTruTruongKhoaDuyet = null;
                    }
                    await _yeuCauMuaDuTruVatTuService.UpdateAsync(duTruMuaVatTuEntity);
                    return Ok(duTruMuaVatTuEntity.Id);
                }

            }
            return NotFound();
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> GetDataChildForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruVatTuService.GetDataChildForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataChildKhoaForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> GetDataChildKhoaForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruVatTuService.GetDataChildKhoaForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion
        #region Goi
        #endregion
        #endregion
        #region đã xử lý 
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachDuyetMuaDuTruTaiKhoaDuocDaXuLyForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachDuyetMuaDuTruTaiKhoaDuocDaXuLyForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var catKyTu = queryInfo.Sort[0].Field.Split('/');
            if (catKyTu.Count() > 1)
            {
                queryInfo.Sort[0].Field = catKyTu[0] + catKyTu[1];
            }
            var gridData = await _yeuCauMuaDuTruVatTuService.GetDataDuTruMuaVatTuTaiKhoaDuocDaXuLyForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDanhSachDuyetMuaDuTruTaiKhoaDuocDaXuLyForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> GetTotalDanhSachDuyetMuaDuTruTaiKhoaDuocDaXuLyForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruVatTuService.GetDataDuTruMuaVatTuTaiKhoaDuocDaXuLyToTalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachDuyetMuaDuTruTaiKhoaDuocDaXuLyChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachDuyetMuaDuTruTaiKhoaDuocDaXuLyChildForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruVatTuService.GetDataDuTruMuaVatTuTaiKhoaDuocChildDaXuLyForGridAsync(queryInfo);
            return Ok(gridData);
        }
        //  Child 
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDanhSachDuyetMuaDuTruTaiKhoaDuocDaXuLyChildTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> GetTotalDanhSachDuyetMuaDuTruTaiKhoaDuocDaXuLyChildTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruVatTuService.GetDataDuTruMuaVatTuTaiKhoaDuocChildDaXuLyTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachDuyetMuaDuTruTaiKhoaDuocDaXuLyChildChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachDuyetMuaDuTruTaiKhoaDuocDaXuLyChildChildForGridAsync([FromBody]QueryInfo queryInfo)
        {

            var gridData = await _yeuCauMuaDuTruVatTuService.GetDataDuTruMuaVatTuTaiKhoaDuocChildDaXuLyChildChildForGridAsync(queryInfo);
            return Ok(gridData);
        }
        //  Child  Child  
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDanhSachDuyetMuaDuTruTaiKhoaDuocDaXuLyChildChildTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> GetTotalDanhSachDuyetMuaDuTruTaiKhoaDuocDaXuLyChildChildTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruVatTuService.GetDataDuTruMuaVatTuTaiKhoaDuocChildDaXuLyChildChildTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataDuTruMuaVatTuTaiKhoaDuocChildDaXuLyChildChildChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> GetDataDuTruMuaVatTuTaiKhoaDuocChildDaXuLyChildChildChildForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruVatTuService.GetDataDuTruMuaVatTuTaiKhoaDuocChildDaXuLyChildChildChildForGridAsync(queryInfo);
            return Ok(gridData);
        }
        //  Child  Child  Child
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataDuTruMuaVatTuTaiKhoaDuocChildDaXuLyChildChildChildTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> GetDataDuTruMuaVatTuTaiKhoaDuocChildDaXuLyChildChildChildTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruVatTuService.GetDataDuTruMuaVatTuTaiKhoaDuocChildDaXuLyChildChildChildTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        // view  [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataGoiDuyetDuTruMuaVatTuView")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoaDuoc)]
        public async Task<ActionResult<DuTruMuaVatTuChiTietGoiViewGridVo>> GetDataGoiDuyetDuTruMuaVatTuView(long idKyDuTru, long tinhTrang)
        {
            var gridData = _yeuCauMuaDuTruVatTuService.GetDuTruMuaVatTuChiTietGoiView(idKyDuTru, tinhTrang);
            return Ok(gridData);
        }
        // view  [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDuTruMuaVatTuChiTietGoiViewChild")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> GetDuTruMuaVatTuChiTietGoiViewChild([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruVatTuService.GetDuTruMuaVatTuChiTietGoiViewChild(queryInfo);
            return Ok(gridData);
        }
        #endregion
        #region từ chối
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachDuyetMuaDuTruTaiKhoaDuocTuChoiForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachDuyetMuaDuTruTaiKhoaDuocTuChoiForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruVatTuService.GetDataDuTruMuaVatTuTaiKhoaDuocTuChoiForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDanhSachDuyetMuaDuTruTaiKhoaDuocTuChoiForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> GetTotalDanhSachDuyetMuaDuTruTaiKhoaDuoTuChoicForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruVatTuService.GetDataDuTruMuaVatTuTaiKhoaDuocTuChoiToTalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataDuTruMuaVatTuTaiKhoaDuocTuChoiChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> GetDataDuTruMuaVatTuTaiKhoaDuocTuChoiChildForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruVatTuService.GetDataDuTruMuaVatTuTaiKhoaDuocTuChoiChildForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataDuTruMuaVatTuTaiKhoaDuocTuChoiToTalPageChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> GetDataDuTruMuaVatTuTaiKhoaDuocTuChoiToTalPageChildForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruVatTuService.GetDataDuTruMuaVatTuTaiKhoaDuocTuChoiToTalPageChildForGridAsync(queryInfo);
            return Ok(gridData);
        }
        // child child
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataDuTruMuaVatTuTaiKhoaDuocTuChoiChildChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> GetDataDuTruMuaVatTuTaiKhoaDuocTuChoiChildChildForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruVatTuService.GetDataDuTruMuaVatTuTaiKhoaDuocTuChoiChildChildForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataDuTruMuaVatTuTaiKhoaDuocTuChoiToTalPageChildChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> GetDataDuTruMuaVatTuTaiKhoaDuocTuChoiToTalPageChildChildForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruVatTuService.GetDataDuTruMuaVatTuTaiKhoaDuocTuChoiToTalPageChildChildForGridAsync(queryInfo);
            return Ok(gridData);
        }

        #endregion
        #region gởi
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataGoiDuyetDuTruMuaVatTu")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoaDuoc)]
        public async Task<ActionResult<DuTruMuaVatTuChiTietGoiViewGridVo>> GetDataGoiDuyetDuTruMuaVatTu(long idKyDuTru)
        {
            var gridData = _yeuCauMuaDuTruVatTuService.GetDuTruMuaVatTuChiTietGoi(idKyDuTru);
            return Ok(gridData);
        }
        // create
        [HttpPost("GuiDuTruMuaVatTuTaiKhoaDuoc")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoaDuoc)]
        public async Task<ActionResult> GuiDuTruMuaVatTuTaiKhoaDuoc(DuTruMuaVatTuChiTietGoiViewGridVo duTruMuaVatTuTaiKhoaDuoc)
        {
            var result = _yeuCauMuaVatTuDuTruTaiKhoaDuocService.GuiDuTruMuaVatTuTaiKhoaDuoc(duTruMuaVatTuTaiKhoaDuoc);
            return Ok(result.Result);
        }
        #endregion
        #region In
        [HttpPost("InPhieuDuTruMuaTaiKhoaDuoc")]
        public string InPhieuDuTruMuaTaiKhoaDuoc(PhieuInDuTruMuaTaiKhoa phieuInDuTruMuaTaiKhoa)
        {
            var result = _yeuCauMuaDuTruVatTuService.InPhieuDuTruMuaTaiKhoaDuoc(phieuInDuTruMuaTaiKhoa);
            return result;
        }
        #endregion
    }
}
