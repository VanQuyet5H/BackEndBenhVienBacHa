using System.Collections.Generic;
using Camino.Api.Auth;
using Camino.Core.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Camino.Api.Models.KhamBenh;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.Controllers
{
    public partial class KhamBenhController
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetThongTinTuongTrinh")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, Enums.DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult> GetThongTinTuongTrinh(long phongBenhVienId, long yeuCauKhamBenhId)
        {
            var khoaPhong = await _yeuCauDichVuKyThuatService.GetThongTinTuongTrinh(phongBenhVienId, yeuCauKhamBenhId);
            return Ok(khoaPhong);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetListPhauThuatThuThuat")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, Enums.DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<List<KhamBenhPhauThuatThuThuatGridVo>> GetListPhauThuatThuThuat(long phongBenhVienId, long nhanVienId, long yeuCauKhamBenhId)
        {
            var listPhauThuatThuThuat = await _yeuCauDichVuKyThuatService.GetListPhauThuatThuThuat(phongBenhVienId, nhanVienId, yeuCauKhamBenhId);
            return listPhauThuatThuThuat;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("WillShowTabPhauThuat")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, Enums.DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult> WillShowTabPhauThuat(long phongBenhVienId, long nhanVienId, long yeuCauKhamBenhId)
        {
            var listPhauThuatThuThuat = await _yeuCauDichVuKyThuatService.WillShowTabPhauThuat(phongBenhVienId, nhanVienId, yeuCauKhamBenhId);
            return Ok(listPhauThuatThuThuat);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetListICD")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, Enums.DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult> GetListIcd([FromBody]DropDownListRequestModel model)
        {
            var listIcd = await _yeuCauDichVuKyThuatService.GetListICD(model);
            return Ok(listIcd);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetListPhuongPhapPTTT")]
        public async Task<ActionResult> GetListPhuongPhapPttt([FromBody]DropDownListRequestModel model)
        {
            var listPhuongPhapPttt = await _yeuCauDichVuKyThuatService.GetListPhuongPhapPTTT(model);
            return Ok(listPhuongPhapPttt);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetListPhuongPhapPtttAutoComplete")]
        public async Task<ActionResult<string>> GetListPhuongPhapPtttAutoComplete([FromBody]DropDownListRequestModel model)
        {
            var listPhuongPhapPttt = await _yeuCauDichVuKyThuatService.GetListPhuongPhapPtttAutoComplete(model);
            return Ok(listPhuongPhapPttt);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetLoaiPtttDisplay")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, Enums.DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult> GetLoaiPtttDisplay(string ma)
        {
            var loaiPttt = await _yeuCauDichVuKyThuatService.GetLoaiPtttDisplay(ma);
            return Ok(loaiPttt);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetListPhuongPhapVoCam")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, Enums.DocumentType.PhauThuatThuThuatTheoNgay)]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> GetListPhuongPhapVoCam([FromBody]DropDownListRequestModel model)
        {
            var listIcd = await _yeuCauDichVuKyThuatService.GetListPhuongPhapVoCam(model);
            return Ok(listIcd);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTinhHinhPttt")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, Enums.DocumentType.PhauThuatThuThuatTheoNgay)]
        public ActionResult GetTinhHinhPttt([FromBody]LookupQueryInfo model)
        {
            var listIcd = _yeuCauDichVuKyThuatService.GetTinhHinhPttt(model);
            return Ok(listIcd);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetLoaiPttt")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, Enums.DocumentType.PhauThuatThuThuatTheoNgay)]
        public ActionResult GetLoaiPttt([FromBody]LookupQueryInfo model)
        {
            var listIcd = _yeuCauDichVuKyThuatService.GetLoaiPttt(model);
            return Ok(listIcd);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetListBoPhanCoThe")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, Enums.DocumentType.PhauThuatThuThuatTheoNgay)]
        public ActionResult GetListBoPhanCoThe([FromBody]LookupQueryInfo model)
        {
            var listIcd = _yeuCauDichVuKyThuatService.GetListBoPhanCoThe(model);
            return Ok(listIcd);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTaiBienPttt")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, Enums.DocumentType.PhauThuatThuThuatTheoNgay)]
        public ActionResult GetTaiBienPttt([FromBody]LookupQueryInfo model)
        {
            var listIcd = _yeuCauDichVuKyThuatService.GetTaiBienPttt(model);
            return Ok(listIcd);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetHinhPhauThuatDuaTrenBoPhan")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, Enums.DocumentType.PhauThuatThuThuatTheoNgay)]
        public ActionResult GetHinhPhauThuatDuaTrenBoPhan(string boPhan)
        {
            var listIcd = _yeuCauDichVuKyThuatService.GetHinhPhauThuatDuaTrenBoPhan(boPhan);
            return Ok(listIcd);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTuVongPttt")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, Enums.DocumentType.PhauThuatThuThuatTheoNgay)]
        public ActionResult GetTuVongPttt([FromBody]LookupQueryInfo model)
        {
            var listIcd = _yeuCauDichVuKyThuatService.GetTuVongPttt(model);
            return Ok(listIcd);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("SaveTuongTrinh")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.KhamBenh, Enums.DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult> SaveTuongTrinh([FromBody]KhamBenhPhauThuatThuThuatViewModel model)
        {
            await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhAsync(model.YeuCauKhamBenhId);

            var phauThuatThuThuatEntities = new List<YeuCauDichVuKyThuatTuongTrinhPTTT>();

            foreach (var phauThuatThuThuat in model.ListPttt)
            {
                var yeuCauDichVuKyThuat = await _yeuCauDichVuKyThuatService.GetByIdAsync(phauThuatThuThuat.Id, x => x.Include(w => w.YeuCauDichVuKyThuatTuongTrinhPTTT)
                    .ThenInclude(z => z.YeuCauDichVuKyThuatLuocDoPhauThuats));

                if (yeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT == null)
                {
                    yeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT = new YeuCauDichVuKyThuatTuongTrinhPTTT
                    {
                        Id = phauThuatThuThuat.Id,
                        GhiChuICDSauPhauThuat = phauThuatThuThuat.GhiChuICDSauPhauThuat,
                        GhiChuICDTruocPhauThuat = phauThuatThuThuat.GhiChuICDTruocPhauThuat,
                        ICDSauPhauThuatId = phauThuatThuThuat.ICDSauPhauThuatId,
                        ICDTruocPhauThuatId = phauThuatThuThuat.ICDTruocPhauThuatId,
                        LoaiPhauThuatThuThuat = phauThuatThuThuat.LoaiPTTTEnum,
                        MaPhuongPhapPTTT = phauThuatThuThuat.PhuongPhapPhauThuatThuThuatKey,
                        PhuongPhapVoCamId = phauThuatThuThuat.PhuongPhapVoCamId,
                        TrinhTuPhauThuat = phauThuatThuThuat.TrinhTuPttt,
                        TaiBienPTTT = phauThuatThuThuat.TaiBienPttt,
                        TinhHinhPTTT = phauThuatThuThuat.TinhHinhPttt,
                        TuVongTrongPTTT = phauThuatThuThuat.TuVongPttt
                    };
                }
                else
                {
                    yeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.GhiChuICDSauPhauThuat =
                        phauThuatThuThuat.GhiChuICDSauPhauThuat;
                    yeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.GhiChuICDTruocPhauThuat =
                        phauThuatThuThuat.GhiChuICDTruocPhauThuat;
                    yeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.ICDSauPhauThuatId =
                        phauThuatThuThuat.ICDSauPhauThuatId;
                    yeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.ICDTruocPhauThuatId =
                        phauThuatThuThuat.ICDTruocPhauThuatId;
                    yeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.LoaiPhauThuatThuThuat =
                        phauThuatThuThuat.LoaiPTTTEnum;
                    yeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.MaPhuongPhapPTTT =
                        phauThuatThuThuat.PhuongPhapPhauThuatThuThuatKey;
                    yeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.PhuongPhapVoCamId =
                        phauThuatThuThuat.PhuongPhapVoCamId;
                    yeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.TrinhTuPhauThuat =
                        phauThuatThuThuat.TrinhTuPttt;
                    yeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.TaiBienPTTT =
                        phauThuatThuThuat.TaiBienPttt;
                    yeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.TinhHinhPTTT =
                        phauThuatThuThuat.TinhHinhPttt;
                    yeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.TuVongTrongPTTT =
                        phauThuatThuThuat.TuVongPttt;
                }

                if (yeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.YeuCauDichVuKyThuatLuocDoPhauThuats == null ||
                    yeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.YeuCauDichVuKyThuatLuocDoPhauThuats.Count == 0)
                {
                    if (phauThuatThuThuat.LuocDoPttts.Count != 0)
                    {
                        foreach (var luocDoItem in phauThuatThuThuat.LuocDoPttts)
                        {
                            yeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.YeuCauDichVuKyThuatLuocDoPhauThuats?.Add(new YeuCauDichVuKyThuatLuocDoPhauThuat
                            {
                                Id = luocDoItem.Id,
                                YeuCauDichVuKyThuatTuongTrinhPTTTId = luocDoItem.IdYeuCauDichVuKyThuat,
                                LuocDo = luocDoItem.LuocDoPhauThuat,
                                MoTa = luocDoItem.MoTa
                            });
                        }
                    }
                }
                else
                {
                    if (phauThuatThuThuat.LuocDoPttts.Count != 0)
                    {
                        foreach (var luocDoEntityItem in yeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.YeuCauDichVuKyThuatLuocDoPhauThuats)
                        {
                            luocDoEntityItem.WillDelete = true;
                        }

                        foreach (var luocDoEntityItem in yeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.YeuCauDichVuKyThuatLuocDoPhauThuats)
                        {
                            foreach (var phauThuatViewModelItem in phauThuatThuThuat.LuocDoPttts)
                            {
                                if (luocDoEntityItem.Id ==
                                    phauThuatViewModelItem.Id)
                                {
                                    luocDoEntityItem.WillDelete = false;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var luocDoEntityItem in yeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.YeuCauDichVuKyThuatLuocDoPhauThuats)
                        {
                            luocDoEntityItem.WillDelete = true;
                        }
                    }
                }

                var modifyTenPhauThuatThuThuatEntity =
                    await _yeuCauDichVuKyThuatService.ModifyTenPhauThuatThuThuatEntity(yeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT
                        .MaPhuongPhapPTTT);

                yeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.TenPhuongPhapPTTT = modifyTenPhauThuatThuThuatEntity;
                phauThuatThuThuatEntities.Add(yeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT);
            }

            var updateOk = await _yeuCauDichVuKyThuatService.UpdateForThisPhauThuat(phauThuatThuThuatEntities, model.YeuCauKhamBenhId);

            return Ok(updateOk);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetListLuocDoPhauThuat")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, Enums.DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult> GetListLuocDoPhauThuat([FromBody]ListDichVuKyThuatParameterGridVo listIdDichVuKyThuatModel)
        {
            var listPhauThuatThuThuat = await _yeuCauDichVuKyThuatService.GetListLuocDoPhauThuat(listIdDichVuKyThuatModel);
            return Ok(listPhauThuatThuThuat);
        }
    }
}
