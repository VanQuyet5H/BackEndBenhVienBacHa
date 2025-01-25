using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Services.GoiDichVus;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{
    public class GoiDichVuController : CaminoBaseController
    {
        private readonly IGoiDvService _goiDichVuService;

        public GoiDichVuController(IGoiDvService goiDichVuService)
        {
            _goiDichVuService = goiDichVuService;
        }

        [HttpPost("GetListDichVuKyThuat")]
        public async Task<ActionResult> GetListDichVuKyThuat([FromBody]DropDownListRequestModel model)
        {
            var listEnum = await _goiDichVuService.GetListDichVuKyThuat(model);
            return Ok(listEnum);
        }

        [HttpPost("GetListDichVuGiuong")]
        public async Task<ActionResult> GetListDichVuGiuong([FromBody]DropDownListRequestModel model)
        {
            var listEnum = await _goiDichVuService.GetListDichVuGiuong(model);
            return Ok(listEnum);
        }

        [HttpPost("GetChiPhiHienTaiDichVuKhamBenh")]
        public async Task<long> GetChiPhiHienTaiDichVuKhamBenh(long dichVuKhamBenhBenhVienId, long nhomGiaDichVuKhamBenhBenhVienId)
        {
            var chiPhiHienTai = await _goiDichVuService.GetChiPhiHienTaiDichVuKhamBenh(dichVuKhamBenhBenhVienId, nhomGiaDichVuKhamBenhBenhVienId);
            return await Task.FromResult(chiPhiHienTai);
        }

        [HttpPost("GetChiPhiHienTaiDichVuKyThuat")]
        public async Task<long> GetChiPhiHienTaiDichVuKyThuat(long dichVuKyThuatBenhVienId, long nhomGiaDichVuKyThuatBenhVienId)
        {
            var chiPhiHienTai = await _goiDichVuService.GetChiPhiHienTaiDichVuKyThuat(dichVuKyThuatBenhVienId, nhomGiaDichVuKyThuatBenhVienId);
            return await Task.FromResult(chiPhiHienTai);
        }

        [HttpPost("GetChiPhiChoDichVuGiuong")]
        public async Task<long> GetChiPhiChoDichVuGiuong(long dichVuGiuongBenhVienId, long nhomGiaId)
        {
            var chiPhiHienTai = await _goiDichVuService.GetChiPhiChoDichVuGiuong(dichVuGiuongBenhVienId, nhomGiaId);
            return await Task.FromResult(chiPhiHienTai);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridChiTietAsync")]
        public ActionResult<GridDataSource> GetDataForGridChiTietAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = _goiDichVuService.GetDataForGridChiTietAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridChiTietAsync")]
        public ActionResult<GridDataSource> GetTotalPageForGridChiTietAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = _goiDichVuService.GetTotalPageForGridChiTietAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("KichHoatGoiDichVu")]
        public async Task<ActionResult> KichHoatGoiDichVu(long id)
        {
            var entity = await _goiDichVuService.GetByIdAsync(id);
            entity.IsDisabled = entity.IsDisabled == null ? true : !entity.IsDisabled;
            await _goiDichVuService.UpdateAsync(entity);
            return NoContent();
        }


        [HttpPost("LoaiGiaNhomGiaDichVuKyThuatBenhVien")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> LoaiGiaNhomGiaDichVuKyThuatBenhVien(long? dichVuKyThuatId)
        {
            var lookup = await _goiDichVuService.LoaiGiaNhomGiaDichVuKyThuatBenhVien(dichVuKyThuatId);
            return Ok(lookup);
        }

        [HttpPost("LoaiGiaNhomGiaGiuongBenhVien")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> LoaiGiaNhomGiaGiuongBenhVien(long? dichVuGiuongBenhVienId)
        {
            var lookup = await _goiDichVuService.LoaiGiaNhomGiaGiuongBenhVien(dichVuGiuongBenhVienId);
            return Ok(lookup);
        }
        [HttpPost("LoaiGiaDichVuKhamBenh")]
        public async Task<ActionResult<ICollection<LookupItemVo>>>LoaiGiaDichVuKhamBenh(long? idDichVuKhamBenhId)
        {
            var lookup = await _goiDichVuService.GetLoaiGiaDichVuKhamBenh(idDichVuKhamBenhId);
            return Ok(lookup);
        }



        [HttpPost("LoaiGiaNhomGiaDichVuKyThuatBenhVienGrid")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> LoaiGiaNhomGiaDichVuKyThuatBenhVienGrid(long? dichVuKyThuatId)
        {
            var lookup = await _goiDichVuService.LoaiGiaNhomGiaDichVuKyThuatBenhVienGrid(dichVuKyThuatId);
            return Ok(lookup);
        }

        [HttpPost("LoaiGiaNhomGiaGiuongBenhVienGrid")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> LoaiGiaNhomGiaGiuongBenhVienGrid(long? dichVuGiuongBenhVienId)
        {
            var lookup = await _goiDichVuService.LoaiGiaNhomGiaGiuongBenhVienGrid(dichVuGiuongBenhVienId);
            return Ok(lookup);
        }
        [HttpPost("LoaiGiaDichVuKhamBenhGrid")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> LoaiGiaDichVuKhamBenhGrid(long? idDichVuKhamBenhId)
        {
            var lookup = await _goiDichVuService.GetLoaiGiaDichVuKhamBenhGrid(idDichVuKhamBenhId);
            return Ok(lookup);
        }
    }
}
