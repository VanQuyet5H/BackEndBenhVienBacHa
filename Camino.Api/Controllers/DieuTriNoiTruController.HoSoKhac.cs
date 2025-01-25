using System.Linq;
using Camino.Core.Domain.ValueObject;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Camino.Core.Domain.ValueObject.PhieuTheoDoiTruyenMau;

namespace Camino.Api.Controllers
{
    public partial class DieuTriNoiTruController
    {
        [HttpPost("GetListNhanVienAutoComplete")]
        public async Task<ActionResult<string>> GetListNhanVienAutoComplete([FromBody] DropDownListRequestModel model)
        {
            var lstNhanVien = await _dieuTriNoiTruService.GetListNhanVienAutoComplete(model);
            return Ok(lstNhanVien);
        }

        [HttpPost("GetListChucDanhAutoComplete")]
        public async Task<ActionResult<string>> GetListChucDanhAutoComplete([FromBody] DropDownListRequestModel model)
        {
            var lstNhanVien = await _dieuTriNoiTruService.GetListChucDanhAutoComplete(model);
            return Ok(lstNhanVien);
        }

        [HttpPost("GetListVanBangChuyenMonAutoComplete")]
        public async Task<ActionResult<string>> GetListVanBangChuyenMonAutoComplete([FromBody] DropDownListRequestModel model)
        {
            var lstNhanVien = await _dieuTriNoiTruService.GetListVanBangChuyenMonAutoComplete(model);
            return Ok(lstNhanVien);
        }
        [HttpGet("GetListHoSoKhac/{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<dynamic>> GetListHoSoKhac(long id)
        {
            var hoSoKhac = await _noiTruHoSoKhacService.GetListNoiTruHoSoKhac(id, null);
            return Ok(hoSoKhac);
        }
        [HttpPost("GetListLoaiHoSoDieuTriNoiTru")]
        public async Task<ActionResult<string>> GetListLoaiHoSoDieuTriNoiTru([FromBody] DropDownListRequestModel model)
        {
            var lstLoaiHoSoKhac = await _noiTruHoSoKhacService.GetLoaiHoSoDieuTriNoiTru(model);
            return Ok(lstLoaiHoSoKhac);
        }
        [HttpDelete("DeleteHoSoKhac/{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> DeleteHoSoKhac(long id)
        {
            var hoSoKhac = await _noiTruHoSoKhacService.GetByIdAsync(id, p => p.Include(x => x.NoiTruHoSoKhacFileDinhKems).Include(s=>s.YeuCauTiepNhan).ThenInclude(d=>d.YeuCauTruyenMaus).ThenInclude(f=>f.NhapKhoMauChiTiets));
            if (hoSoKhac == null)
            {
                return NotFound();
            }
            #region xóa phiếu truyền màu
            if (hoSoKhac.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.PhieuTheoDoiTruyenMau)
            {
                var queryString = JsonConvert.DeserializeObject<InPhieuTheoDoiTruyenMau>(hoSoKhac.ThongTinHoSo);
                if(hoSoKhac.YeuCauTiepNhanId != null)
                {
                    var noitruHoSoKhacids = _noiTruHoSoKhacService.HoSoKhacIds(hoSoKhac.YeuCauTiepNhanId, hoSoKhac.LoaiHoSoDieuTriNoiTru);
                    foreach (var itemId in noitruHoSoKhacids.Result.ToList())
                    {
                        hoSoKhac = await _noiTruHoSoKhacService.GetByIdAsync(itemId, p => p.Include(x => x.NoiTruHoSoKhacFileDinhKems).Include(s => s.YeuCauTiepNhan).ThenInclude(d => d.YeuCauTruyenMaus).ThenInclude(f => f.NhapKhoMauChiTiets));
                        if (queryString.MaDonViMauTruyenId != null)
                        {
                            var listYeuCauTruyenMauUpdate = hoSoKhac.YeuCauTiepNhan.YeuCauTruyenMaus.Where(s => s.NhapKhoMauChiTiets.Any(d => d.Id == queryString.MaDonViMauTruyenId != null)).ToList();
                            foreach (var item in listYeuCauTruyenMauUpdate)
                            {
                                item.NhanVienThucHienId = null;
                                item.ThoiDiemThucHien = null;
                                item.TrangThai = Enums.EnumTrangThaiYeuCauTruyenMau.ChuaThucHien;
                                item.ThoiDiemHoanThanh = null;
                            }

                            await _yeuCauTiepNhanService.UpdateAsync(hoSoKhac.YeuCauTiepNhan);
                        }
                        if (hoSoKhac.NoiTruHoSoKhacFileDinhKems.Count > 0)
                        {
                            for (int i = 0; i < hoSoKhac.NoiTruHoSoKhacFileDinhKems.Count; i++)
                            {
                                await _noiDuTruHoSoKhacFileDinhKemService.DeleteByIdAsync(hoSoKhac.NoiTruHoSoKhacFileDinhKems.ToList()[i].Id);
                                i = -1;
                            }
                        }
                        await _noiTruHoSoKhacService.DeleteByIdAsync(itemId);
                    }
                }
             
                return NoContent();
            }
            else
            {
                var noitruHoSoKhacids = _noiTruHoSoKhacService.HoSoKhacIds(hoSoKhac.YeuCauTiepNhanId, hoSoKhac.LoaiHoSoDieuTriNoiTru);
                foreach (var itemId in noitruHoSoKhacids.Result.ToList())
                {
                    hoSoKhac = await _noiTruHoSoKhacService.GetByIdAsync(itemId, p => p.Include(x => x.NoiTruHoSoKhacFileDinhKems).Include(s => s.YeuCauTiepNhan).ThenInclude(d => d.YeuCauTruyenMaus).ThenInclude(f => f.NhapKhoMauChiTiets));
                    if (hoSoKhac.NoiTruHoSoKhacFileDinhKems.Count > 0)
                    {
                        for (int i = 0; i < hoSoKhac.NoiTruHoSoKhacFileDinhKems.Count; i++)
                        {
                            await _noiDuTruHoSoKhacFileDinhKemService.DeleteByIdAsync(hoSoKhac.NoiTruHoSoKhacFileDinhKems.ToList()[i].Id);
                            i = -1;
                        }
                    }
                    await _noiTruHoSoKhacService.DeleteByIdAsync(itemId);
                }
                return NoContent();
            }
            #endregion

            //if (hoSoKhac.NoiTruHoSoKhacFileDinhKems.Count > 0)
            //{
            //    for (int i = 0; i < hoSoKhac.NoiTruHoSoKhacFileDinhKems.Count; i++)
            //    {
            //        await _noiDuTruHoSoKhacFileDinhKemService.DeleteByIdAsync(hoSoKhac.NoiTruHoSoKhacFileDinhKems.ToList()[i].Id);
            //        i = -1;
            //    }
            //}
            //await _noiTruHoSoKhacService.DeleteByIdAsync(id);

            //return NoContent();


             
           
        }

    }
}
