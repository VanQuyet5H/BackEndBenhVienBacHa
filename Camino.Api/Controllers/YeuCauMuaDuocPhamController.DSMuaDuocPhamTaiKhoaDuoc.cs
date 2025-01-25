using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.DuTruMuaDuocPhamTaiKhoaDuoc;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DuTruMuaDuocPhamKhoDuocs;
using Camino.Core.Domain.Entities.DuTruMuaDuocPhams;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DuTruMuaDuocPhamTaiKhoa;
using Camino.Core.Domain.ValueObject.DuTruMuaDuocPhamTaiKhoaDuoc;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Services.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Controllers
{
    public partial class YeuCauMuaDuocPhamController
    {
        #region cho xu  lý
        #region DS chờ xử lý
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachDuyetMuaDuTruTaiKhoaDuocForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachDuyetMuaDuTruTaiKhoaDuocForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var catKyTu = queryInfo.Sort[0].Field.Split('/');
            if (catKyTu.Count() > 1)
            {
                queryInfo.Sort[0].Field = catKyTu[0] + catKyTu[1];
            }
            var gridData = await _yeuCauMuaDuTruDuocPhamService.GetDataDuTruMuaDuocPhamTaiKhoaDuocForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDanhSachDuyetMuaDuTruTaiKhoaDuocForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> GetTotalDanhSachDuyetMuaDuTruTaiKhoaDuocForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruDuocPhamService.GetDataDuTruMuaDuocPhamTaiKhoaDuocToTalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion

        #region child chờ xử lý
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataDuTruMuaDuocPhamTaiKhoaDuocChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> GetDataDuTruMuaDuocPhamTaiKhoaDuocChildForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruDuocPhamService.GetDataDuTruMuaDuocPhamTaiKhoaDuocChildForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDanhSachDuyetMuaDuTruTaiKhoaDuocChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> GetTotalDanhSachDuyetMuaDuTruTaiKhoaDuocChildForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruDuocPhamService.GetDataDuTruMuaDuocPhamTaiKhoaDuocToTalPageChildForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion
        #region child child chờ xử lý
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataDuTruMuaDuocPhamTaiKhoaDuocChildChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> GetDataDuTruMuaDuocPhamTaiKhoaDuocChildChildForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruDuocPhamService.GetDataDuTruMuaDuocPhamTaiKhoaDuocChildChildForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDanhSachDuyetMuaDuTruTaiKhoaDuocChildChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> GetTotalDanhSachDuyetMuaDuTruTaiKhoaDuocChildChildForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruDuocPhamService.GetDataDuTruMuaDuocPhamTaiKhoaDuocToTalPageChildChildForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion
        #region duyet -tu chối -hủy duyet ngoài danh sách
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetListDuTruMuaChiTiet")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> GetListDuTruMuaChiTiet(long muaDuTruDuocPhamId)
        {
            var gridData =  _yeuCauMuaDuTruDuocPhamService.GetDuTruMuaDuocPhamChiTiet(muaDuTruDuocPhamId);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetListDuTruMuaKhoaChiTiet")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> GetListDuTruMuaKhoaChiTiet(long muaDuTruDuocPhamId)
        {
            var gridData = _yeuCauMuaDuTruDuocPhamService.GetDuTruMuaDuocPhamKhoaChiTiet(muaDuTruDuocPhamId);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("Duyet")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoaDuoc)]
        public async Task<ActionResult> Duyet(DuTruMuaDuocPhamTaiKhoaDuocDuyet duTruMuaDuocPhamTaiKhoaDuocDuyet)
        {
            if(duTruMuaDuocPhamTaiKhoaDuocDuyet.LoaiDuyet == true) // khoa
            {
                var duTruMuaDuocPhamTheoKhoaEntity = await _yeuCauMuaDuTruDuocPhamService.GetDuTruDuocPhamTheoKhoaByIdAsync(duTruMuaDuocPhamTaiKhoaDuocDuyet.Id);
                duTruMuaDuocPhamTaiKhoaDuocDuyet.ToEntity(duTruMuaDuocPhamTheoKhoaEntity);
                duTruMuaDuocPhamTheoKhoaEntity.KhoDuocDuyet = true;
                duTruMuaDuocPhamTheoKhoaEntity.NhanVienKhoDuocId = _userAgentHelper.GetCurrentUserId();
                duTruMuaDuocPhamTheoKhoaEntity.NgayKhoDuocDuyet = DateTime.Now;
                await _yeuCauMuaDuTruDuocPhamTheoKhoaService.UpdateAsync(duTruMuaDuocPhamTheoKhoaEntity);
                return Ok(duTruMuaDuocPhamTheoKhoaEntity.Id);
            }
            if (duTruMuaDuocPhamTaiKhoaDuocDuyet.LoaiDuyet == false) // kho
            {
                var duTruMuaDuocPhamEntity = await _yeuCauMuaDuTruDuocPhamService.GetByIdAsync(duTruMuaDuocPhamTaiKhoaDuocDuyet.Id, s => s.Include(p => p.DuTruMuaDuocPhamChiTiets));
                duTruMuaDuocPhamTaiKhoaDuocDuyet.ToEntity(duTruMuaDuocPhamEntity);
                duTruMuaDuocPhamEntity.TruongKhoaDuyet = true;
                duTruMuaDuocPhamEntity.TruongKhoaId = _userAgentHelper.GetCurrentUserId();
                duTruMuaDuocPhamEntity.NgayTruongKhoaDuyet = DateTime.Now;
                foreach (var duTruMuaDuocPhamChiTiet in duTruMuaDuocPhamEntity.DuTruMuaDuocPhamChiTiets)
                {
                    if (duTruMuaDuocPhamChiTiet.SoLuongDuTruTruongKhoaDuyet == null)
                    {
                        duTruMuaDuocPhamChiTiet.SoLuongDuTruTruongKhoaDuyet = duTruMuaDuocPhamChiTiet.SoLuongDuTru;
                    }
                }
                await _yeuCauMuaDuTruDuocPhamService.UpdateAsync(duTruMuaDuocPhamEntity);
                return Ok(duTruMuaDuocPhamEntity.Id);
            }
            return NotFound();
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("TuChoi")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoaDuoc)]
        public async Task<ActionResult> TuChoi(DuTruMuaDuocPhamTaiKhoaDuocDuyet duTruMuaDuocPhamTaiKhoaDuocDuyet)
        {
            if (duTruMuaDuocPhamTaiKhoaDuocDuyet.LoaiDuyet == true) // khoa
            {
                var duTruMuaDuocPhamTheoKhoaEntity = await _yeuCauMuaDuTruDuocPhamService.GetDuTruDuocPhamTheoKhoaByIdAsync(duTruMuaDuocPhamTaiKhoaDuocDuyet.Id);
                duTruMuaDuocPhamTaiKhoaDuocDuyet.ToEntity(duTruMuaDuocPhamTheoKhoaEntity);
                duTruMuaDuocPhamTheoKhoaEntity.KhoDuocDuyet = false;
                duTruMuaDuocPhamTheoKhoaEntity.LyDoKhoDuocTuChoi = duTruMuaDuocPhamTaiKhoaDuocDuyet.LyDoTruongKhoaTuChoi;
                duTruMuaDuocPhamTheoKhoaEntity. NhanVienKhoDuocId = _userAgentHelper.GetCurrentUserId();
                duTruMuaDuocPhamTheoKhoaEntity.NgayKhoDuocDuyet = DateTime.Now;
                await _yeuCauMuaDuTruDuocPhamTheoKhoaService.UpdateAsync(duTruMuaDuocPhamTheoKhoaEntity);
                return Ok(duTruMuaDuocPhamTheoKhoaEntity.Id);
            }
            if(duTruMuaDuocPhamTaiKhoaDuocDuyet.LoaiDuyet == false)
            {
                var duTruMuaDuocPhamEntity = await _yeuCauMuaDuTruDuocPhamService.GetByIdAsync(duTruMuaDuocPhamTaiKhoaDuocDuyet.Id, s => s.Include(p => p.DuTruMuaDuocPhamChiTiets));
                duTruMuaDuocPhamTaiKhoaDuocDuyet.ToEntity(duTruMuaDuocPhamEntity);
                duTruMuaDuocPhamEntity.TruongKhoaDuyet = false;
                duTruMuaDuocPhamEntity.LyDoTruongKhoaTuChoi = duTruMuaDuocPhamTaiKhoaDuocDuyet.LyDoTruongKhoaTuChoi;
                duTruMuaDuocPhamEntity.TruongKhoaId = _userAgentHelper.GetCurrentUserId();
                duTruMuaDuocPhamEntity.NgayTruongKhoaDuyet = DateTime.Now;
                await _yeuCauMuaDuTruDuocPhamService.UpdateAsync(duTruMuaDuocPhamEntity);
                return Ok(duTruMuaDuocPhamEntity.Id);
            }
            return NotFound();
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("HuyDuyet")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoaDuoc)]
        public async Task<ActionResult> HuyDuyet(HuyDuyetQueryInfo huyDuyetQueryInfo)
        {
            if (huyDuyetQueryInfo.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoLe)
            {
                var duTruMuaDuocPhamTheoKhoaEntity = await _yeuCauMuaDuTruDuocPhamService.GetDuTruDuocPhamTheoKhoaByIdAsync(huyDuyetQueryInfo.Id);
                if (duTruMuaDuocPhamTheoKhoaEntity != null)
                {
                    duTruMuaDuocPhamTheoKhoaEntity.KhoDuocDuyet = null;
                    duTruMuaDuocPhamTheoKhoaEntity.NgayKhoDuocDuyet = null;
                    foreach (var chiTiet in duTruMuaDuocPhamTheoKhoaEntity.DuTruMuaDuocPhamTheoKhoaChiTiets)
                    {
                        chiTiet.SoLuongDuTruKhoDuocDuyet = null;
                    }
                    await _yeuCauMuaDuTruDuocPhamTheoKhoaService.UpdateAsync(duTruMuaDuocPhamTheoKhoaEntity);
                    return Ok(duTruMuaDuocPhamTheoKhoaEntity.Id);
                }
            }
            else
            {
                var duTruMuaDuocPhamEntity = await _yeuCauMuaDuTruDuocPhamService.GetByIdAsync(huyDuyetQueryInfo.Id, s => s.Include(p => p.DuTruMuaDuocPhamChiTiets));
                if (duTruMuaDuocPhamEntity != null)
                {
                    duTruMuaDuocPhamEntity.TruongKhoaDuyet = null;
                    duTruMuaDuocPhamEntity.NgayTruongKhoaDuyet = null;
                    foreach (var chiTiet in duTruMuaDuocPhamEntity.DuTruMuaDuocPhamChiTiets)
                    {
                        chiTiet.SoLuongDuTruTruongKhoaDuyet = null;
                    }
                    await _yeuCauMuaDuTruDuocPhamService.UpdateAsync(duTruMuaDuocPhamEntity);
                    return Ok(duTruMuaDuocPhamEntity.Id);
                }

            }
//            if (duTruMuaDuocPhamTaiKhoaDuocDuyet.LoaiDuyet == true) // khoa
//            {
//                var duTruMuaDuocPhamTheoKhoaEntity = await _yeuCauMuaDuTruDuocPhamService.GetDuTruDuocPhamTheoKhoaByIdAsync(duTruMuaDuocPhamTaiKhoaDuocDuyet.Id);
//                duTruMuaDuocPhamTaiKhoaDuocDuyet.ToEntity(duTruMuaDuocPhamTheoKhoaEntity);
//                duTruMuaDuocPhamTheoKhoaEntity.KhoDuocDuyet = null;
//                duTruMuaDuocPhamTheoKhoaEntity.LyDoKhoDuocTuChoi = null;
//                duTruMuaDuocPhamTheoKhoaEntity.NhanVienKhoDuocId = null;
//                duTruMuaDuocPhamTheoKhoaEntity.NgayKhoDuocDuyet = null;
//                await _yeuCauMuaDuTruDuocPhamTheoKhoaService.UpdateAsync(duTruMuaDuocPhamTheoKhoaEntity);
//                return Ok(duTruMuaDuocPhamTheoKhoaEntity.Id);
//            }
//            if(duTruMuaDuocPhamTaiKhoaDuocDuyet.LoaiDuyet == false)
//            {
//                var duTruMuaDuocPhamEntity = await _yeuCauMuaDuTruDuocPhamService.GetByIdAsync(duTruMuaDuocPhamTaiKhoaDuocDuyet.Id, s => s.Include(p => p.DuTruMuaDuocPhamChiTiets));
//                duTruMuaDuocPhamTaiKhoaDuocDuyet.ToEntity(duTruMuaDuocPhamEntity);
//                duTruMuaDuocPhamEntity.TruongKhoaDuyet = null;
//                duTruMuaDuocPhamEntity.TruongKhoaId = null;
//                duTruMuaDuocPhamEntity.NgayTruongKhoaDuyet = null;
//                foreach (var item in duTruMuaDuocPhamTaiKhoaDuocDuyet.ListDuTruMuaDuocPhamKhoDuocChiTiet)
//                {
//                    item.SoLuongDuTruTruongKhoaDuyet = null;
//                }
//                await _yeuCauMuaDuTruDuocPhamService.UpdateAsync(duTruMuaDuocPhamEntity);
//                return Ok(duTruMuaDuocPhamEntity.Id);
//            }
            return NotFound();
        }
        #endregion
        #region duyet -tu chối -hủy duyet trong danh sách
        [HttpPost("GetDataUpdate")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoaDuoc)]
        public async Task<ActionResult<DuTruMuaDuocPhamChiTietViewGridVo>> GetDataUpdate(long id, bool typeLoaiKho)
        {
            var result = _yeuCauMuaDuTruDuocPhamService.GetDataUpdate(id,typeLoaiKho);
            return Ok(result);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("DuyetDanhSach")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoaDuoc)]
        public async Task<ActionResult> DuyetDanhSach(DuTruMuaDuocPhamTaiKhoaDuocDuyet duTruMuaDuocPhamTaiKhoaDuocDuyet)
        {
            if (duTruMuaDuocPhamTaiKhoaDuocDuyet.LoaiDuyet == true) // khoa
            {
                var duTruMuaDuocPhamTheoKhoaEntity = await _yeuCauMuaDuTruDuocPhamService.GetDuTruDuocPhamTheoKhoaByIdAsync(duTruMuaDuocPhamTaiKhoaDuocDuyet.Id);
                duTruMuaDuocPhamTaiKhoaDuocDuyet.ToEntity(duTruMuaDuocPhamTheoKhoaEntity);
                duTruMuaDuocPhamTheoKhoaEntity.KhoDuocDuyet = true;
                duTruMuaDuocPhamTheoKhoaEntity.NhanVienKhoDuocId = _userAgentHelper.GetCurrentUserId();
                duTruMuaDuocPhamTheoKhoaEntity.NgayKhoDuocDuyet = DateTime.Now;
                await _yeuCauMuaDuTruDuocPhamTheoKhoaService.UpdateAsync(duTruMuaDuocPhamTheoKhoaEntity);
                return Ok(duTruMuaDuocPhamTheoKhoaEntity.Id);
            }
            if (duTruMuaDuocPhamTaiKhoaDuocDuyet.LoaiDuyet == false)
            {
                var duTruMuaDuocPhamEntity = await _yeuCauMuaDuTruDuocPhamService.GetByIdAsync(duTruMuaDuocPhamTaiKhoaDuocDuyet.Id, s => s.Include(p => p.DuTruMuaDuocPhamChiTiets));
                duTruMuaDuocPhamTaiKhoaDuocDuyet.ToEntity(duTruMuaDuocPhamEntity);
                duTruMuaDuocPhamEntity.TruongKhoaDuyet = true;
                duTruMuaDuocPhamEntity.TruongKhoaId = _userAgentHelper.GetCurrentUserId();
                duTruMuaDuocPhamEntity.NgayTruongKhoaDuyet = DateTime.Now;
                foreach (var duTruMuaDuocPhamChiTiet in duTruMuaDuocPhamEntity.DuTruMuaDuocPhamChiTiets)
                {
                    if (duTruMuaDuocPhamChiTiet.SoLuongDuTruTruongKhoaDuyet == null)
                    {
                        duTruMuaDuocPhamChiTiet.SoLuongDuTruTruongKhoaDuyet = duTruMuaDuocPhamChiTiet.SoLuongDuTru;
                    }
                }
                await _yeuCauMuaDuTruDuocPhamService.UpdateAsync(duTruMuaDuocPhamEntity);
                return Ok(duTruMuaDuocPhamEntity.Id);
            }
            return NotFound();
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("TuChoiDanhSach")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> TuChoiDanhSach(TuChoiQueryInfo TuChoiQueryInfo)
        {
            if (TuChoiQueryInfo.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoLe)
            {
                var duTruMuaDuocPhamTheoKhoaEntity = await _yeuCauMuaDuTruDuocPhamService.GetDuTruDuocPhamTheoKhoaByIdAsync(TuChoiQueryInfo.Id);
                if (duTruMuaDuocPhamTheoKhoaEntity != null)
                {
                    duTruMuaDuocPhamTheoKhoaEntity.KhoDuocDuyet = false;
                    duTruMuaDuocPhamTheoKhoaEntity.LyDoKhoDuocTuChoi = TuChoiQueryInfo.LyDoTuChoi;
                    duTruMuaDuocPhamTheoKhoaEntity.NhanVienKhoDuocId = _userAgentHelper.GetCurrentUserId();
                    duTruMuaDuocPhamTheoKhoaEntity.NgayKhoDuocDuyet = DateTime.Now;
                    foreach (var chiTiet in duTruMuaDuocPhamTheoKhoaEntity.DuTruMuaDuocPhamTheoKhoaChiTiets)
                    {
                        chiTiet.SoLuongDuTruKhoDuocDuyet = null;
                    }
                    await _yeuCauMuaDuTruDuocPhamTheoKhoaService.UpdateAsync(duTruMuaDuocPhamTheoKhoaEntity);
                    return Ok(duTruMuaDuocPhamTheoKhoaEntity.Id);
                }
            }
            else
            {
                var duTruMuaDuocPhamEntity = await _yeuCauMuaDuTruDuocPhamService.GetByIdAsync(TuChoiQueryInfo.Id, s => s.Include(p => p.DuTruMuaDuocPhamChiTiets));
                if (duTruMuaDuocPhamEntity != null)
                {
                    duTruMuaDuocPhamEntity.TruongKhoaDuyet = false;
                    duTruMuaDuocPhamEntity.LyDoTruongKhoaTuChoi = TuChoiQueryInfo.LyDoTuChoi;
                    duTruMuaDuocPhamEntity.TruongKhoaId = _userAgentHelper.GetCurrentUserId();
                    duTruMuaDuocPhamEntity.NgayTruongKhoaDuyet = DateTime.Now;
                    foreach (var chiTiet in duTruMuaDuocPhamEntity.DuTruMuaDuocPhamChiTiets)
                    {
                        chiTiet.SoLuongDuTruTruongKhoaDuyet = null;
                    }
                    await _yeuCauMuaDuTruDuocPhamService.UpdateAsync(duTruMuaDuocPhamEntity);
                    return Ok(duTruMuaDuocPhamEntity.Id);
                }

            }
            return NotFound();
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> GetDataChildForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruDuocPhamService.GetDataChildForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataChildKhoaForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> GetDataChildKhoaForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruDuocPhamService.GetDataChildKhoaForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion
        #region Goi
        #endregion
        #endregion
        #region đã xử lý 
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachDuyetMuaDuTruTaiKhoaDuocDaXuLyForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachDuyetMuaDuTruTaiKhoaDuocDaXuLyForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var catKyTu = queryInfo.Sort[0].Field.Split('/');
            if (catKyTu.Count() > 1)
            {
                queryInfo.Sort[0].Field = catKyTu[0] + catKyTu[1];
            }
            var gridData = await _yeuCauMuaDuTruDuocPhamService.GetDataDuTruMuaDuocPhamTaiKhoaDuocDaXuLyForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDanhSachDuyetMuaDuTruTaiKhoaDuocDaXuLyForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> GetTotalDanhSachDuyetMuaDuTruTaiKhoaDuocDaXuLyForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruDuocPhamService.GetDataDuTruMuaDuocPhamTaiKhoaDuocDaXuLyToTalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachDuyetMuaDuTruTaiKhoaDuocDaXuLyChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachDuyetMuaDuTruTaiKhoaDuocDaXuLyChildForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruDuocPhamService.GetDataDuTruMuaDuocPhamTaiKhoaDuocChildDaXuLyForGridAsync(queryInfo);
            return Ok(gridData);
        }
        //  Child 
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDanhSachDuyetMuaDuTruTaiKhoaDuocDaXuLyChildTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> GetTotalDanhSachDuyetMuaDuTruTaiKhoaDuocDaXuLyChildTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruDuocPhamService.GetDataDuTruMuaDuocPhamTaiKhoaDuocChildDaXuLyTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachDuyetMuaDuTruTaiKhoaDuocDaXuLyChildChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachDuyetMuaDuTruTaiKhoaDuocDaXuLyChildChildForGridAsync([FromBody]QueryInfo queryInfo)
        {
            
            var gridData = await _yeuCauMuaDuTruDuocPhamService.GetDataDuTruMuaDuocPhamTaiKhoaDuocChildDaXuLyChildChildForGridAsync(queryInfo);
            return Ok(gridData);
        }
        //  Child  Child  
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDanhSachDuyetMuaDuTruTaiKhoaDuocDaXuLyChildChildTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> GetTotalDanhSachDuyetMuaDuTruTaiKhoaDuocDaXuLyChildChildTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruDuocPhamService.GetDataDuTruMuaDuocPhamTaiKhoaDuocChildDaXuLyChildChildTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataDuTruMuaDuocPhamTaiKhoaDuocChildDaXuLyChildChildChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> GetDataDuTruMuaDuocPhamTaiKhoaDuocChildDaXuLyChildChildChildForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruDuocPhamService.GetDataDuTruMuaDuocPhamTaiKhoaDuocChildDaXuLyChildChildChildForGridAsync(queryInfo);
            return Ok(gridData);
        }
        //  Child  Child  Child
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataDuTruMuaDuocPhamTaiKhoaDuocChildDaXuLyChildChildChildTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> GetDataDuTruMuaDuocPhamTaiKhoaDuocChildDaXuLyChildChildChildTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruDuocPhamService.GetDataDuTruMuaDuocPhamTaiKhoaDuocChildDaXuLyChildChildChildTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        // view  [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataGoiDuyetDuTruMuaDuocPhamView")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoaDuoc)]
        public async Task<ActionResult<DuTruMuaDuocPhamChiTietGoiViewGridVo>> GetDataGoiDuyetDuTruMuaDuocPhamView(long idKyDuTru,long tinhTrang)
        {
            var gridData = _yeuCauMuaDuTruDuocPhamService.GetDuTruMuaDuocPhamChiTietGoiView(idKyDuTru, tinhTrang);
            return Ok(gridData);
        }
        // view  [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDuTruMuaDuocPhamChiTietGoiViewChild")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> GetDuTruMuaDuocPhamChiTietGoiViewChild([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruDuocPhamService.GetDuTruMuaDuocPhamChiTietGoiViewChild(queryInfo);
            return Ok(gridData);
        }
        #endregion
        #region từ chối
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachDuyetMuaDuTruTaiKhoaDuocTuChoiForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachDuyetMuaDuTruTaiKhoaDuocTuChoiForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruDuocPhamService.GetDataDuTruMuaDuocPhamTaiKhoaDuocTuChoiForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDanhSachDuyetMuaDuTruTaiKhoaDuocTuChoiForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> GetTotalDanhSachDuyetMuaDuTruTaiKhoaDuoTuChoicForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruDuocPhamService.GetDataDuTruMuaDuocPhamTaiKhoaDuocTuChoiToTalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataDuTruMuaDuocPhamTaiKhoaDuocTuChoiChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> GetDataDuTruMuaDuocPhamTaiKhoaDuocTuChoiChildForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruDuocPhamService.GetDataDuTruMuaDuocPhamTaiKhoaDuocTuChoiChildForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataDuTruMuaDuocPhamTaiKhoaDuocTuChoiToTalPageChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> GetDataDuTruMuaDuocPhamTaiKhoaDuocTuChoiToTalPageChildForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruDuocPhamService.GetDataDuTruMuaDuocPhamTaiKhoaDuocTuChoiToTalPageChildForGridAsync(queryInfo);
            return Ok(gridData);
        }
        // child child
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataDuTruMuaDuocPhamTaiKhoaDuocTuChoiChildChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> GetDataDuTruMuaDuocPhamTaiKhoaDuocTuChoiChildChildForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruDuocPhamService.GetDataDuTruMuaDuocPhamTaiKhoaDuocTuChoiChildChildForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataDuTruMuaDuocPhamTaiKhoaDuocTuChoiToTalPageChildChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoaDuoc)]
        public async Task<ActionResult<GridDataSource>> GetDataDuTruMuaDuocPhamTaiKhoaDuocTuChoiToTalPageChildChildForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruDuocPhamService.GetDataDuTruMuaDuocPhamTaiKhoaDuocTuChoiToTalPageChildChildForGridAsync(queryInfo);
            return Ok(gridData);
        }

        #endregion
        #region gởi
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataGoiDuyetDuTruMuaDuocPham")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoaDuoc)]
        public async Task<ActionResult<DuTruMuaDuocPhamChiTietGoiViewGridVo>> GetDataGoiDuyetDuTruMuaDuocPham(long idKyDuTru)
        {
            var gridData =  _yeuCauMuaDuTruDuocPhamService.GetDuTruMuaDuocPhamChiTietGoi(idKyDuTru);
            return Ok(gridData);
        }
        // create
        [HttpPost("GuiDuTruMuaDuocPhamTaiKhoaDuoc")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoaDuoc)]
        public async Task<ActionResult> GuiDuTruMuaDuocPhamTaiKhoaDuoc(DuTruMuaDuocPhamChiTietGoiViewGridVo duTruMuaDuocPhamTaiKhoaDuoc)
        {
            var result = _yeuCauMuaDuocPhamDuTruTaiKhoaDuocService.GuiDuTruMuaDuocPhamTaiKhoaDuoc(duTruMuaDuocPhamTaiKhoaDuoc);
            return Ok(result.Result);
        }
        #endregion
        #region In
        [HttpPost("InPhieuDuTruMuaTaiKhoaDuoc")]
        public string InPhieuDuTruMuaTaiKhoaDuoc(PhieuInDuTruMuaTaiKhoa phieuInDuTruMuaTaiKhoa)
        {
            var result = _yeuCauMuaDuTruDuocPhamService.InPhieuDuTruMuaTaiKhoaDuoc(phieuInDuTruMuaTaiKhoa);
            return result;
        }
        #endregion
    }
}
