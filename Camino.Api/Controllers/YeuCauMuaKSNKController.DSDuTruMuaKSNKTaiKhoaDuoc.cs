using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DuTruMuaDuocPhamTaiKhoa;
using Camino.Core.Domain.ValueObject.DuTruMuaKSNKTaiKhoaDuoc;
using Camino.Core.Domain.ValueObject.Grid;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Api.Models.DutruMuaVatTuTaiKhoaDuoc.DutruMuaVatTuTaiKhoaDuoc;

namespace Camino.Api.Controllers
{
    public partial class YeuCauMuaKSNKController
    {
        #region DS chờ xử lý
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachDuyetMuaDuTruTaiKhoaDuocForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiHanhChinh)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachDuyetMuaDuTruTaiKhoaDuocForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var catKyTu = queryInfo.Sort[0].Field.Split('/');
            if (catKyTu.Count() > 1)
            {
                queryInfo.Sort[0].Field = catKyTu[0] + catKyTu[1];
            }
            var gridData = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetDataDuTruMuaKSNKTaiKhoaDuocForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDanhSachDuyetMuaDuTruTaiKhoaDuocForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiHanhChinh)]
        public async Task<ActionResult<GridDataSource>> GetTotalDanhSachDuyetMuaDuTruTaiKhoaDuocForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetDataDuTruMuaKSNKTaiKhoaDuocToTalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion

        #region child chờ xử lý

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataDuTruMuaKSNKTaiKhoaDuocChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiHanhChinh)]
        public async Task<ActionResult<GridDataSource>> GetDataDuTruMuaKSNKTaiKhoaDuocChildForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetDataDuTruMuaKSNKTaiKhoaDuocChildForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDanhSachDuyetMuaDuTruTaiKhoaDuocChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiHanhChinh)]
        public async Task<ActionResult<GridDataSource>> GetTotalDanhSachDuyetMuaDuTruTaiKhoaDuocChildForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetDataDuTruMuaKSNKTaiKhoaDuocToTalPageChildForGridAsync(queryInfo);
            return Ok(gridData);
        }

        #endregion

        #region child child chờ xử lý

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataDuTruMuaKSNKTaiKhoaDuocChildChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiHanhChinh)]
        public async Task<ActionResult<GridDataSource>> GetDataDuTruMuaVatTuTaiKhoaDuocChildChildForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetDataDuTruMuaKSNKTaiKhoaDuocChildChildForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDanhSachDuyetMuaDuTruTaiKhoaDuocChildChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiHanhChinh)]
        public async Task<ActionResult<GridDataSource>> GetTotalDanhSachDuyetMuaDuTruTaiKhoaDuocChildChildForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetDataDuTruMuaKSNKTaiKhoaDuocToTalPageChildChildForGridAsync(queryInfo);
            return Ok(gridData);
        }

        #endregion

        #region duyet -tu chối -hủy duyet ngoài danh sách

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetListDuTruMuaChiTiet")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.THDTMuaTaiHanhChinh)]
        public async Task<ActionResult<GridDataSource>> GetListDuTruMuaChiTiet(long muaDuTruVatTuId)
        {
            var gridData = _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetDuTruMuaKSNKChiTiet(muaDuTruVatTuId);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetListDuTruMuaKhoaChiTiet")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.THDTMuaTaiHanhChinh)]
        public async Task<ActionResult<GridDataSource>> GetListDuTruMuaKhoaChiTiet(long muaDuTruVatTuId)
        {
            var gridData = _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetDuTruMuaKSNKKhoaChiTiet(muaDuTruVatTuId);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("Duyet")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.THDTMuaTaiHanhChinh)]
        public async Task<ActionResult> Duyet(DuTruMuaVatTuTaiKhoaDuocDuyet duTruMuaVatTuTaiKhoaDuocDuyet)
        {
            if (duTruMuaVatTuTaiKhoaDuocDuyet.LoaiDuyet == true) // khoa
            {
                var duTruMuaVatTuTheoKhoaEntity = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetDuTruKSNKTheoKhoaByIdAsync(duTruMuaVatTuTaiKhoaDuocDuyet.Id);
                duTruMuaVatTuTaiKhoaDuocDuyet.ToEntity(duTruMuaVatTuTheoKhoaEntity);
                duTruMuaVatTuTheoKhoaEntity.KhoDuocDuyet = true;
                duTruMuaVatTuTheoKhoaEntity.NhanVienKhoDuocId = _userAgentHelper.GetCurrentUserId();
                duTruMuaVatTuTheoKhoaEntity.NgayKhoDuocDuyet = DateTime.Now;
                await _yeuCauMuaDuTruVatTuTheoKhoaService.UpdateAsync(duTruMuaVatTuTheoKhoaEntity);
                return Ok(duTruMuaVatTuTheoKhoaEntity.Id);
            }
            if (duTruMuaVatTuTaiKhoaDuocDuyet.LoaiDuyet == false) // kho
            {
                var duTruMuaVatTuEntity = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetByIdAsync(duTruMuaVatTuTaiKhoaDuocDuyet.Id, s => s.Include(p => p.DuTruMuaVatTuChiTiets));
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
                await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.UpdateAsync(duTruMuaVatTuEntity);
                return Ok(duTruMuaVatTuEntity.Id);
            }
            return NotFound();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("TuChoi")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.THDTMuaTaiHanhChinh)]
        public async Task<ActionResult> TuChoi(DuTruMuaVatTuTaiKhoaDuocDuyet duTruMuaVatTuTaiKhoaDuocDuyet)
        {
            if (duTruMuaVatTuTaiKhoaDuocDuyet.LoaiDuyet == true) // khoa
            {
                var duTruMuaVatTuTheoKhoaEntity = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetDuTruKSNKTheoKhoaByIdAsync(duTruMuaVatTuTaiKhoaDuocDuyet.Id);
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
                var duTruMuaVatTuEntity = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetByIdAsync(duTruMuaVatTuTaiKhoaDuocDuyet.Id, s => s.Include(p => p.DuTruMuaVatTuChiTiets));
                duTruMuaVatTuTaiKhoaDuocDuyet.ToEntity(duTruMuaVatTuEntity);
                duTruMuaVatTuEntity.TruongKhoaDuyet = false;
                duTruMuaVatTuEntity.LyDoTruongKhoaTuChoi = duTruMuaVatTuTaiKhoaDuocDuyet.LyDoTruongKhoaTuChoi;
                duTruMuaVatTuEntity.TruongKhoaId = _userAgentHelper.GetCurrentUserId();
                duTruMuaVatTuEntity.NgayTruongKhoaDuyet = DateTime.Now;
                await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.UpdateAsync(duTruMuaVatTuEntity);
                return Ok(duTruMuaVatTuEntity.Id);
            }
            return NotFound();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("HuyDuyet")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.THDTMuaTaiHanhChinh)]
        public async Task<ActionResult> HuyDuyet(HuyDuyetQueryInfo huyDuyetQueryInfo)
        {
            if (huyDuyetQueryInfo.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoLe)
            {
                var duTruMuaVatTuTheoKhoaEntity = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetDuTruKSNKTheoKhoaByIdAsync(huyDuyetQueryInfo.Id);
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
                var duTruMuaVatTuEntity = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetByIdAsync(huyDuyetQueryInfo.Id, s => s.Include(p => p.DuTruMuaVatTuChiTiets));
                if (duTruMuaVatTuEntity != null)
                {
                    duTruMuaVatTuEntity.TruongKhoaDuyet = null;
                    duTruMuaVatTuEntity.NgayTruongKhoaDuyet = null;
                    foreach (var chiTiet in duTruMuaVatTuEntity.DuTruMuaVatTuChiTiets)
                    {
                        chiTiet.SoLuongDuTruTruongKhoaDuyet = null;
                    }
                    await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.UpdateAsync(duTruMuaVatTuEntity);
                    return Ok(duTruMuaVatTuEntity.Id);
                }

            }
            //            if (duTruMuaVatTuTaiKhoaDuocDuyet.LoaiDuyet == true) // khoa
            //            {
            //                var duTruMuaVatTuTheoKhoaEntity = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetDuTruVatTuTheoKhoaByIdAsync(duTruMuaVatTuTaiKhoaDuocDuyet.Id);
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
            //                var duTruMuaVatTuEntity = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetByIdAsync(duTruMuaVatTuTaiKhoaDuocDuyet.Id, s => s.Include(p => p.DuTruMuaVatTuChiTiets));
            //                duTruMuaVatTuTaiKhoaDuocDuyet.ToEntity(duTruMuaVatTuEntity);
            //                duTruMuaVatTuEntity.TruongKhoaDuyet = null;
            //                duTruMuaVatTuEntity.TruongKhoaId = null;
            //                duTruMuaVatTuEntity.NgayTruongKhoaDuyet = null;
            //                foreach (var item in duTruMuaVatTuTaiKhoaDuocDuyet.ListDuTruMuaVatTuKhoDuocChiTiet)
            //                {
            //                    item.SoLuongDuTruTruongKhoaDuyet = null;
            //                }
            //                await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.UpdateAsync(duTruMuaVatTuEntity);
            //                return Ok(duTruMuaVatTuEntity.Id);
            //            }
            return NotFound();
        }

        #endregion

        #region duyet -tu chối -hủy duyet trong danh sách

        [HttpPost("GetDataUpdate")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiHanhChinh)]
        public async Task<ActionResult<DuTruMuaKSNKChiTietViewGridVo>> GetDataUpdate(long id, bool typeLoaiKho)
        {
            var result = _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetDataUpdate(id, typeLoaiKho);
            return Ok(result);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("DuyetDanhSach")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.THDTMuaTaiHanhChinh)]
        public async Task<ActionResult> DuyetDanhSach(DuTruMuaVatTuTaiKhoaDuocDuyet duTruMuaVatTuTaiKhoaDuocDuyet)
        {
            if (duTruMuaVatTuTaiKhoaDuocDuyet.LoaiDuyet == true) // khoa
            {
                var duTruMuaVatTuTheoKhoaEntity = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetDuTruKSNKTheoKhoaByIdAsync(duTruMuaVatTuTaiKhoaDuocDuyet.Id);
                duTruMuaVatTuTaiKhoaDuocDuyet.ToEntity(duTruMuaVatTuTheoKhoaEntity);
                duTruMuaVatTuTheoKhoaEntity.KhoDuocDuyet = true;
                duTruMuaVatTuTheoKhoaEntity.NhanVienKhoDuocId = _userAgentHelper.GetCurrentUserId();
                duTruMuaVatTuTheoKhoaEntity.NgayKhoDuocDuyet = DateTime.Now;
                await _yeuCauMuaDuTruVatTuTheoKhoaService.UpdateAsync(duTruMuaVatTuTheoKhoaEntity);
                return Ok(duTruMuaVatTuTheoKhoaEntity.Id);
            }
            if (duTruMuaVatTuTaiKhoaDuocDuyet.LoaiDuyet == false)
            {
                var duTruMuaVatTuEntity = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetByIdAsync(duTruMuaVatTuTaiKhoaDuocDuyet.Id, s => s.Include(p => p.DuTruMuaVatTuChiTiets));
                duTruMuaVatTuTaiKhoaDuocDuyet.ToEntity(duTruMuaVatTuEntity);
                duTruMuaVatTuEntity.TruongKhoaDuyet = true;
                duTruMuaVatTuEntity.TruongKhoaId = _userAgentHelper.GetCurrentUserId();
                duTruMuaVatTuEntity.NgayTruongKhoaDuyet = DateTime.Now;
                await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.UpdateAsync(duTruMuaVatTuEntity);
                return Ok(duTruMuaVatTuEntity.Id);
            }
            return NotFound();
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("TuChoiDanhSach")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.THDTMuaTaiHanhChinh)]
        public async Task<ActionResult<GridDataSource>> TuChoiDanhSach(TuChoiQueryInfo TuChoiQueryInfo)
        {
            if (TuChoiQueryInfo.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoLe)
            {
                var duTruMuaVatTuTheoKhoaEntity = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetDuTruKSNKTheoKhoaByIdAsync(TuChoiQueryInfo.Id);
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
                var duTruMuaVatTuEntity = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetByIdAsync(TuChoiQueryInfo.Id, s => s.Include(p => p.DuTruMuaVatTuChiTiets));
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
                    await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.UpdateAsync(duTruMuaVatTuEntity);
                    return Ok(duTruMuaVatTuEntity.Id);
                }

            }
            return NotFound();
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiHanhChinh)]
        public async Task<ActionResult<GridDataSource>> GetDataChildForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetDataChildForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataChildKhoaForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiHanhChinh)]
        public async Task<ActionResult<GridDataSource>> GetDataChildKhoaForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetDataChildKhoaForGridAsync(queryInfo);
            return Ok(gridData);
        }

        #endregion

        #region Đã xử lý 

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachDuyetMuaDuTruTaiKhoaDuocDaXuLyForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiHanhChinh)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachDuyetMuaDuTruTaiKhoaDuocDaXuLyForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var catKyTu = queryInfo.Sort[0].Field.Split('/');
            if (catKyTu.Count() > 1)
            {
                queryInfo.Sort[0].Field = catKyTu[0] + catKyTu[1];
            }
            var gridData = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetDataDuTruMuaKSNKTaiKhoaDuocDaXuLyForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDanhSachDuyetMuaDuTruTaiKhoaDuocDaXuLyForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiHanhChinh)]
        public async Task<ActionResult<GridDataSource>> GetTotalDanhSachDuyetMuaDuTruTaiKhoaDuocDaXuLyForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetDataDuTruMuaKSNKTaiKhoaDuocDaXuLyToTalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachDuyetMuaDuTruTaiKhoaDuocDaXuLyChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiHanhChinh)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachDuyetMuaDuTruTaiKhoaDuocDaXuLyChildForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetDataDuTruMuaKSNKTaiKhoaDuocChildDaXuLyForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDanhSachDuyetMuaDuTruTaiKhoaDuocDaXuLyChildTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiHanhChinh)]
        public async Task<ActionResult<GridDataSource>> GetTotalDanhSachDuyetMuaDuTruTaiKhoaDuocDaXuLyChildTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetDataDuTruMuaKSNKTaiKhoaDuocChildDaXuLyTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachDuyetMuaDuTruTaiKhoaDuocDaXuLyChildChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiHanhChinh)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachDuyetMuaDuTruTaiKhoaDuocDaXuLyChildChildForGridAsync([FromBody]QueryInfo queryInfo)
        {

            var gridData = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetDataDuTruMuaKSNKTaiKhoaDuocChildDaXuLyChildChildForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDanhSachDuyetMuaDuTruTaiKhoaDuocDaXuLyChildChildTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiHanhChinh)]
        public async Task<ActionResult<GridDataSource>> GetTotalDanhSachDuyetMuaDuTruTaiKhoaDuocDaXuLyChildChildTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetDataDuTruMuaKSNKTaiKhoaDuocChildDaXuLyChildChildTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataDuTruMuaKSNKTaiKhoaDuocChildDaXuLyChildChildChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiHanhChinh)]
        public async Task<ActionResult<GridDataSource>> GetDataDuTruMuaKSNKTaiKhoaDuocChildDaXuLyChildChildChildForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetDataDuTruMuaKSNKTaiKhoaDuocChildDaXuLyChildChildChildForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataDuTruMuaKSNKTaiKhoaDuocChildDaXuLyChildChildChildTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiHanhChinh)]
        public async Task<ActionResult<GridDataSource>> GetDataDuTruMuaKSNKTaiKhoaDuocChildDaXuLyChildChildChildTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetDataDuTruMuaKSNKTaiKhoaDuocChildDaXuLyChildChildChildTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetDataGoiDuyetDuTruMuaKSNKView")]
        public async Task<ActionResult<DuTruMuaKSNKChiTietGoiViewGridVo>> GetDataGoiDuyetDuTruMuaKSNKView(long idKyDuTru, long tinhTrang)
        {
            var gridData = _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetDuTruMuaKSNKChiTietGoiView(idKyDuTru, tinhTrang);
            return Ok(gridData);
        }

        [HttpPost("GetDuTruMuaKSNKChiTietGoiViewChild")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiHanhChinh)]
        public async Task<ActionResult<GridDataSource>> GetDuTruMuaKSNKChiTietGoiViewChild([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetDuTruMuaKSNKChiTietGoiViewChild(queryInfo);
            return Ok(gridData);
        }

        #endregion

        #region Từ chối

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachDuyetMuaDuTruTaiKhoaDuocTuChoiForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiHanhChinh)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachDuyetMuaDuTruTaiKhoaDuocTuChoiForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetDataDuTruMuaKSNKTaiKhoaDuocTuChoiForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDanhSachDuyetMuaDuTruTaiKhoaDuocTuChoiForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiHanhChinh)]
        public async Task<ActionResult<GridDataSource>> GetTotalDanhSachDuyetMuaDuTruTaiKhoaDuoTuChoicForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetDataDuTruMuaKSNKTaiKhoaDuocTuChoiToTalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataDuTruMuaKSNKTaiKhoaDuocTuChoiChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiHanhChinh)]
        public async Task<ActionResult<GridDataSource>> GetDataDuTruMuaKSNKTaiKhoaDuocTuChoiChildForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetDataDuTruMuaKSNKTaiKhoaDuocTuChoiChildForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataDuTruMuaKSNKTaiKhoaDuocTuChoiToTalPageChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiHanhChinh)]
        public async Task<ActionResult<GridDataSource>> GetDataDuTruMuaKSNKTaiKhoaDuocTuChoiToTalPageChildForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetDataDuTruMuaKSNKTaiKhoaDuocTuChoiToTalPageChildForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataDuTruMuaKSNKTaiKhoaDuocTuChoiChildChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiHanhChinh)]
        public async Task<ActionResult<GridDataSource>> GetDataDuTruMuaKSNKTaiKhoaDuocTuChoiChildChildForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetDataDuTruMuaKSNKTaiKhoaDuocTuChoiChildChildForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataDuTruMuaKSNKTaiKhoaDuocTuChoiToTalPageChildChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiHanhChinh)]
        public async Task<ActionResult<GridDataSource>> GetDataDuTruMuaKSNKTaiKhoaDuocTuChoiToTalPageChildChildForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetDataDuTruMuaKSNKTaiKhoaDuocTuChoiToTalPageChildChildForGridAsync(queryInfo);
            return Ok(gridData);
        }

        #endregion

        #region Gởi

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataGoiDuyetDuTruMuaKSNK")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiHanhChinh)]
        public async Task<ActionResult<DuTruMuaKSNKChiTietGoiViewGridVo>> GetDataGoiDuyetDuTruMuaKSNK(long idKyDuTru)
        {
            var gridData = _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetDuTruMuaKSNKChiTietGoi(idKyDuTru);
            return Ok(gridData);
        }

        [HttpPost("GuiDuTruMuaKSNKTaiKhoaDuoc")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.THDTMuaTaiHanhChinh)]
        public async Task<ActionResult> GuiDuTruMuaKSNKTaiKhoaDuoc(DuTruMuaKSNKChiTietGoiViewGridVo duTruMuaVatTuTaiKhoaDuoc)
        {
            var result = _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GuiDuTruMuaKSNKTaiKhoaDuoc(duTruMuaVatTuTaiKhoaDuoc);
            return Ok(result.Result);
        }

        #endregion

        #region In

        [HttpPost("InPhieuDuTruMuaTaiKhoaDuoc")]
        public string InPhieuDuTruMuaTaiKhoaDuoc(PhieuInDuTruMuaTaiKhoa phieuInDuTruMuaTaiKhoa)
        {
            var result = _yeuCauMuaDuTruKiemSoatNhiemKhuanService.InPhieuDuTruMuaTaiKhoaDuoc(phieuInDuTruMuaTaiKhoa);
            return result;
        }

        [HttpPost("InPhieuDuTruMuaDuocPhamTaiKhoaDuoc")]
        public string InPhieuDuTruMuaDuocPhamTaiKhoaDuoc(PhieuInDuTruMuaTaiKhoa phieuInDuTruMuaTaiKhoa)
        {
            var result = _yeuCauMuaDuTruKiemSoatNhiemKhuanService.InPhieuDuTruMuaDuocPhamTaiKhoaDuoc(phieuInDuTruMuaTaiKhoa);
            return result;
        }

        [HttpPost("InPhieuVatTuVaDuocPhamTaiKhoaDuoc")]
        public string InPhieuVatTuVaDuocPhamTaiKhoaDuoc(PhieuInDuTruMuaTaiKhoa phieuInDuTruMuaTaiKhoa)
        {
            var htmlAll = string.Empty;

            if (phieuInDuTruMuaTaiKhoa != null && phieuInDuTruMuaTaiKhoa.DuTruMuaVatTuTheoKhoaId != null)
            {
                htmlAll = _yeuCauMuaDuTruKiemSoatNhiemKhuanService.InPhieuDuTruMuaTaiKhoaDuoc(phieuInDuTruMuaTaiKhoa) + "<div class=\"pagebreak\"> </div>";
            }
            if (phieuInDuTruMuaTaiKhoa != null && phieuInDuTruMuaTaiKhoa.DuTruMuaDuocPhamTheoKhoaId != null)
            {
                htmlAll += _yeuCauMuaDuTruKiemSoatNhiemKhuanService.InPhieuDuTruMuaDuocPhamTaiKhoaDuoc(phieuInDuTruMuaTaiKhoa);
            }
            return htmlAll;
        }


        #endregion
    }
}
