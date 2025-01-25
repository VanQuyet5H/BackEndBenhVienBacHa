using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.DieuTriNoiTru;
using Camino.Api.Models.General;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public partial class DieuTriNoiTruController
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetThongTinHoSoKhacPhieuSangLocDinhDuong")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<HoSoKhacPhieuSangLocDinhDuongViewModel>> GetThongTinHoSoKhacPhieuSangLocDinhDuong(long yeuCauTiepNhanId)
        {
            var hoSoKhac = _dieuTriNoiTruService.GetThongTinHoSoKhacPhieuSangLocDinhDuong(yeuCauTiepNhanId);

            var hoSoKhacVM = hoSoKhac?.ToModel<HoSoKhacPhieuSangLocDinhDuongViewModel>() ?? new HoSoKhacPhieuSangLocDinhDuongViewModel();

            if (hoSoKhac == null)
            {
                var currentUser = _userAgentHelper.GetCurrentUserId();
                hoSoKhacVM.NhanVienThucHienDisplay = await _noiTruHoSoKhacService.GetNguoiThucHien(currentUser);
                hoSoKhacVM.ThoiDiemThucHien = DateTime.Now;
                hoSoKhacVM.ChanDoan = _dieuTriNoiTruService.GetChanDoanVaoVien(yeuCauTiepNhanId);
            }
            
            return hoSoKhacVM;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetDanhSachNhuCauDinhDuong")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public ActionResult<HoSoKhacPhieuSangLocDinhDuongViewModel> GetDanhSachNhuCauDinhDuong()
        {
            var gridData = _dieuTriNoiTruService.GetDanhSachNhuCauDinhDuong();
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("SuaThongTinHoSoKhacPhieuSangLocDinhDuong")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> SuaThongTinHoSoKhacPhieuSangLocDinhDuong([FromBody] HoSoKhacPhieuSangLocDinhDuongViewModel hoSoKhacPhieuSangLocDinhDuongViewModel)
        {
            var newInFoSaveUpDatePhieuSangLocDinhDuong = new InFoSaveUpDatePhieuSangLocDinhDuong();

            hoSoKhacPhieuSangLocDinhDuongViewModel.NhanVienThucHienId = _userAgentHelper.GetCurrentUserId();
            hoSoKhacPhieuSangLocDinhDuongViewModel.NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            hoSoKhacPhieuSangLocDinhDuongViewModel.ThoiDiemThucHien = DateTime.Now;

            var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(hoSoKhacPhieuSangLocDinhDuongViewModel.YeuCauTiepNhanId, o => o.Include(p => p.NoiTruHoSoKhacs)
                                                                                                                                          .ThenInclude(p => p.NoiTruHoSoKhacFileDinhKems));

            if (hoSoKhacPhieuSangLocDinhDuongViewModel.Id == 0)
            {
                var hoSoKhac = hoSoKhacPhieuSangLocDinhDuongViewModel.ToEntity<NoiTruHoSoKhac>();
                yeuCauTiepNhan.NoiTruHoSoKhacs.Add(hoSoKhac);
                newInFoSaveUpDatePhieuSangLocDinhDuong.SaveOrUpdate = true;
            }
            else
            {
                var hoSoKhac = yeuCauTiepNhan.NoiTruHoSoKhacs.Single(c => c.Id == hoSoKhacPhieuSangLocDinhDuongViewModel.Id);
                hoSoKhac = hoSoKhacPhieuSangLocDinhDuongViewModel.ToEntity(hoSoKhac);

                foreach (var item in hoSoKhac.NoiTruHoSoKhacFileDinhKems)
                {
                    if (item.WillDelete != true)
                    {
                        await _taiLieuDinhKemService.LuuTaiLieuAsync(item.DuongDan, item.TenGuid);
                    }
                }
                newInFoSaveUpDatePhieuSangLocDinhDuong.SaveOrUpdate = false;
            }

            await _yeuCauTiepNhanService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhan);

            if(newInFoSaveUpDatePhieuSangLocDinhDuong.SaveOrUpdate == true)
            {
                newInFoSaveUpDatePhieuSangLocDinhDuong.NoiTruHoSoKhacId = yeuCauTiepNhan.NoiTruHoSoKhacs.Select(d => d.Id).LastOrDefault();
            }
            else
            {
                newInFoSaveUpDatePhieuSangLocDinhDuong.NoiTruHoSoKhacId = hoSoKhacPhieuSangLocDinhDuongViewModel.Id;
            }
           
            return Ok(newInFoSaveUpDatePhieuSangLocDinhDuong);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetListGiamCan")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public List<LookupItemVo> GetListGiamCan([FromBody] DropDownListRequestModel dropDownListRequestModel)
        {
            return _dieuTriNoiTruService.GetListGiamCan(dropDownListRequestModel);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetListSoKgGiam")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public List<LookupItemVo> GetListSoKgGiam([FromBody] DropDownListRequestModel dropDownListRequestModel)
        {
            return _dieuTriNoiTruService.GetListSoKgGiam(dropDownListRequestModel);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetListAnUongKem")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public List<LookupItemVo> GetListAnUongKem([FromBody] DropDownListRequestModel dropDownListRequestModel)
        {
            return _dieuTriNoiTruService.GetListAnUongKem(dropDownListRequestModel);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetListTinhTrangBenhLyNang")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public List<LookupItemVo> GetListTinhTrangBenhLyNang([FromBody] DropDownListRequestModel dropDownListRequestModel)
        {
            return _dieuTriNoiTruService.GetListTinhTrangBenhLyNang(dropDownListRequestModel);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetDefaultTinhTrangBenhLyNang")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public LookupItemVo GetDefaultTinhTrangBenhLyNang()
        {
            return _dieuTriNoiTruService.GetDefaultTinhTrangBenhLyNang();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetListKeHoachDinhDuong")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public List<LookupItemVo> GetListKeHoachDinhDuong([FromBody] DropDownListRequestModel dropDownListRequestModel)
        {
            return _dieuTriNoiTruService.GetListKeHoachDinhDuong(dropDownListRequestModel);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetListTocDoTangCan")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public List<LookupItemVo> GetListTocDoTangCan([FromBody] DropDownListRequestModel dropDownListRequestModel)
        {
            return _dieuTriNoiTruService.GetListTocDoTangCan(dropDownListRequestModel);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetListBenhKemTheo")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public List<LookupItemVo> GetListBenhKemTheo([FromBody] DropDownListRequestModel dropDownListRequestModel)
        {
            return _dieuTriNoiTruService.GetListBenhKemTheo(dropDownListRequestModel);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("InPhieuSangLocDinhDuongPhuSan")]
        [ClaimRequirement(SecurityOperation.Process, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<string>> InPhieuSangLocDinhDuongPhuSan(long yeuCauTiepNhanId, string hosting)
        {
            var html = await _dieuTriNoiTruService.InPhieuSangLocDinhDuongPhuSan(yeuCauTiepNhanId, hosting);

            return html;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("InPhieuSangLocDinhDuong")]
        [ClaimRequirement(SecurityOperation.Process, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<string>> InPhieuSangLocDinhDuong(long yeuCauTiepNhanId, string hosting,long noiTruHoSoKhacId)
        {
            var html = await _dieuTriNoiTruService.InPhieuSangLocDinhDuong(yeuCauTiepNhanId, hosting, noiTruHoSoKhacId);
            return html;
        }

        // BVHD-3886
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetListSutCanMotThangQua")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public List<LookupItemVo> GetListSutCanMotThangQua([FromBody] DropDownListRequestModel dropDownListRequestModel)
        {
            return _dieuTriNoiTruService.GetListSutCanMotThangQua(dropDownListRequestModel);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetListAnKemLonHon5Ngay")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public List<LookupItemVo> GetListAnKemLonHon5Ngay([FromBody] DropDownListRequestModel dropDownListRequestModel)
        {
            return _dieuTriNoiTruService.GetListAnKemLonHon5Ngay(dropDownListRequestModel);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetListTaiDanhGia")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public List<LookupItemVo> GetListTaiDanhGia([FromBody] DropDownListRequestModel dropDownListRequestModel)
        {
            return _dieuTriNoiTruService.GetListTaiDanhGia(dropDownListRequestModel);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetListHoiChanDinhDuong")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public List<LookupItemVo> GetListHoiChanDinhDuong([FromBody] DropDownListRequestModel dropDownListRequestModel)
        {
            return _dieuTriNoiTruService.GetListHoiChanDinhDuong(dropDownListRequestModel);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetListDuongNuoiDuong")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public List<LookupItemVo> GetListDuongNuoiDuong([FromBody] DropDownListRequestModel dropDownListRequestModel)
        {
            return _dieuTriNoiTruService.GetListDuongNuoiDuong(dropDownListRequestModel);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetListCheDoAnUongs")]
        //[ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<List<LookupItemVo>> GetListCheDoAnUongs([FromBody] DropDownListRequestModel dropDownListRequestModel)
        {
            var result = await _dieuTriNoiTruService.GetListTCheDoAnUongs(dropDownListRequestModel);
            return result;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetThongTinHoSoKhacPhieuSangLocDinhDuongNew")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<HoSoKhacPhieuSangLocDinhDuongViewModel>> GetThongTinHoSoKhacPhieuSangLocDinhDuongNew(long yeuCauTiepNhanId)
        {
            var hoSoKhacVM = new HoSoKhacPhieuSangLocDinhDuongViewModel();

            var currentUser = _userAgentHelper.GetCurrentUserId();
            hoSoKhacVM.NhanVienThucHienDisplay = await _noiTruHoSoKhacService.GetNguoiThucHien(currentUser);
            hoSoKhacVM.ThoiDiemThucHien = DateTime.Now;
            hoSoKhacVM.ChanDoan = _dieuTriNoiTruService.GetChanDoanVaoVien(yeuCauTiepNhanId);


            return hoSoKhacVM;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetDSPhieuSangLocDinhDuong")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<List<DSPhieuSangLocDinhDuong>>> GetDSPhieuSangLocDinhDuong(long yeuCauTiepNhanId)
        {
            var dsPhieuSangLocDinhDuongs = new List<DSPhieuSangLocDinhDuong>();
            var hoSoKhacs = _dieuTriNoiTruService.GetThongTinHoSoKhacPhieuSangLocDinhDuongs(yeuCauTiepNhanId);

            foreach (var item in hoSoKhacs.ToList())
            {
                var dsPhieuSangLocDinhDuong = new DSPhieuSangLocDinhDuong();

                var thongTin = JsonConvert.DeserializeObject<DSPhieuSangLocDinhDuong>(item.ThongTinHoSo);

                if (thongTin != null)
                {
                    if(thongTin.DungChoPhuNuMangThai == false)
                    {
                        dsPhieuSangLocDinhDuong = thongTin;

                        dsPhieuSangLocDinhDuong.NoiTruHoSoKhacId = item.Id;
                        dsPhieuSangLocDinhDuong.YeuCauTiepNhanId = item.YeuCauTiepNhanId;

                        if (!string.IsNullOrEmpty(thongTin.NgayDanhGiaString))
                        {
                            DateTime ngay = DateTime.Now;
                            DateTime.TryParseExact(thongTin.NgayDanhGiaString, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out ngay);
                            dsPhieuSangLocDinhDuong.NgayDanhGiaDisplay = ngay.ApplyFormatDate();

                        }
                        dsPhieuSangLocDinhDuongs.Add(dsPhieuSangLocDinhDuong);
                    }
                   
                }
            }


            return dsPhieuSangLocDinhDuongs;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("ViewThongTinHoSoKhacPhieuSangLocDinhDuong")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<HoSoKhacPhieuSangLocDinhDuongViewModel>> ViewThongTinHoSoKhacPhieuSangLocDinhDuong(long noiTruHoSoKhacId)
        {
            var hoSoKhac = _dieuTriNoiTruService.ViewThongTinHoSoKhacPhieuSangLocDinhDuong(noiTruHoSoKhacId);

            var hoSoKhacVM = hoSoKhac?.ToModel<HoSoKhacPhieuSangLocDinhDuongViewModel>() ?? new HoSoKhacPhieuSangLocDinhDuongViewModel();

            return hoSoKhacVM;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetThongTinHoSoKhacPhieuSangLocDinhDuongPhuSan")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<HoSoKhacPhieuSangLocDinhDuongViewModel>> GetThongTinHoSoKhacPhieuSangLocDinhDuongPhuSan(long yeuCauTiepNhanId)
        {
            var hoSoKhacVM =  new HoSoKhacPhieuSangLocDinhDuongViewModel();
            var hoSoKhacs = _dieuTriNoiTruService.GetThongTinHoSoKhacPhieuSangLocDinhDuongs(yeuCauTiepNhanId);

            if(hoSoKhacs.ToList().Count() != 0)
            {
                foreach (var item in hoSoKhacs.ToList())
                {
                    var thongTin = JsonConvert.DeserializeObject<DSPhieuSangLocDinhDuong>(item.ThongTinHoSo);

                    if (thongTin.DungChoPhuNuMangThai == true)
                    {
                        var hoSoKhac = _dieuTriNoiTruService.ViewThongTinHoSoKhacPhieuSangLocDinhDuong(item.Id);

                         hoSoKhacVM = hoSoKhac?.ToModel<HoSoKhacPhieuSangLocDinhDuongViewModel>() ;

                        if (hoSoKhac == null)
                        {
                            var currentUser = _userAgentHelper.GetCurrentUserId();
                            hoSoKhacVM.NhanVienThucHienDisplay = await _noiTruHoSoKhacService.GetNguoiThucHien(currentUser);
                            hoSoKhacVM.ThoiDiemThucHien = DateTime.Now;
                            hoSoKhacVM.ChanDoan = _dieuTriNoiTruService.GetChanDoanVaoVien(yeuCauTiepNhanId);
                        }

                        return hoSoKhacVM;
                    }
                    else
                    {
                        var currentUser = _userAgentHelper.GetCurrentUserId();
                        hoSoKhacVM.NhanVienThucHienDisplay = await _noiTruHoSoKhacService.GetNguoiThucHien(currentUser);
                        hoSoKhacVM.ThoiDiemThucHien = DateTime.Now;
                        hoSoKhacVM.ChanDoan = _dieuTriNoiTruService.GetChanDoanVaoVien(yeuCauTiepNhanId);
                        return hoSoKhacVM;
                    }
                }
            }
            else
            {
                var currentUser = _userAgentHelper.GetCurrentUserId();
                hoSoKhacVM.NhanVienThucHienDisplay = await _noiTruHoSoKhacService.GetNguoiThucHien(currentUser);
                hoSoKhacVM.ThoiDiemThucHien = DateTime.Now;
                hoSoKhacVM.ChanDoan = _dieuTriNoiTruService.GetChanDoanVaoVien(yeuCauTiepNhanId);
            }
            return hoSoKhacVM;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("viewDataDinhDuongFirts")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<HoSoKhacPhieuSangLocDinhDuongViewModel>> viewDataDinhDuongFirts(long yeuCauTiepNhanId)
        {
            var hoSoKhacVM = new HoSoKhacPhieuSangLocDinhDuongViewModel();
            var hoSoKhacs = _dieuTriNoiTruService.GetThongTinHoSoKhacPhieuSangLocDinhDuongs(yeuCauTiepNhanId);

            if (hoSoKhacs.ToList().Count() != 0)
            {
                foreach (var item in hoSoKhacs.ToList())
                {
                    var thongTin = JsonConvert.DeserializeObject<DSPhieuSangLocDinhDuong>(item.ThongTinHoSo);

                    if (thongTin.DungChoPhuNuMangThai == false)
                    {
                        var hoSoKhac = _dieuTriNoiTruService.ViewThongTinHoSoKhacPhieuSangLocDinhDuong(item.Id);

                        hoSoKhacVM = hoSoKhac?.ToModel<HoSoKhacPhieuSangLocDinhDuongViewModel>();

                        if (hoSoKhac == null)
                        {
                            var currentUser = _userAgentHelper.GetCurrentUserId();
                            hoSoKhacVM.NhanVienThucHienDisplay = await _noiTruHoSoKhacService.GetNguoiThucHien(currentUser);
                            hoSoKhacVM.ThoiDiemThucHien = DateTime.Now;
                            hoSoKhacVM.ChanDoan = _dieuTriNoiTruService.GetChanDoanVaoVien(yeuCauTiepNhanId);
                        }

                        return hoSoKhacVM;
                    }
                    else
                    {
                        var currentUser = _userAgentHelper.GetCurrentUserId();
                        hoSoKhacVM.NhanVienThucHienDisplay = await _noiTruHoSoKhacService.GetNguoiThucHien(currentUser);
                        hoSoKhacVM.ThoiDiemThucHien = DateTime.Now;
                        hoSoKhacVM.ChanDoan = _dieuTriNoiTruService.GetChanDoanVaoVien(yeuCauTiepNhanId);

                        return hoSoKhacVM;
                    }
                }
            }
            else
            {
                var currentUser = _userAgentHelper.GetCurrentUserId();
                hoSoKhacVM.NhanVienThucHienDisplay = await _noiTruHoSoKhacService.GetNguoiThucHien(currentUser);
                hoSoKhacVM.ThoiDiemThucHien = DateTime.Now;
                hoSoKhacVM.ChanDoan = _dieuTriNoiTruService.GetChanDoanVaoVien(yeuCauTiepNhanId);
            }
            return hoSoKhacVM;
        }

        [HttpPost("XoaPhieus")]
        public async Task<ActionResult> XoaBienBanHoiChanPhauThuat(ReMoveModel model)
        {
            var idDDs = new List<long>();
            var idPSs = new List<long>();

            var hoSoKhacs = _dieuTriNoiTruService.GetThongTinHoSoKhacPhieuSangLocDinhDuongs(model.YeuCauTiepNhanId);

            if (hoSoKhacs.ToList().Count() != 0)
            {
                
                foreach (var item in hoSoKhacs.ToList())
                {
                    var thongTin = JsonConvert.DeserializeObject<DSPhieuSangLocDinhDuong>(item.ThongTinHoSo);

                    if (thongTin.DungChoPhuNuMangThai == true)
                    {
                        idPSs.Add(item.Id);
                    }
                    else
                    {
                        idDDs.Add(item.Id);
                    }
                }
            }
            var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(model.YeuCauTiepNhanId, o => o.Include(p => p.NoiTruHoSoKhacs)
                                                                                                                                         .ThenInclude(p => p.NoiTruHoSoKhacFileDinhKems));

            if (model.DungChoPhuNuMangThai == true)
            {

                foreach(var item in  yeuCauTiepNhan.NoiTruHoSoKhacs.Where(d=> idDDs.Contains(d.Id)).ToList())
                {
                    item.WillDelete = true;
                }

               
            }
            else
            {
                foreach (var item in yeuCauTiepNhan.NoiTruHoSoKhacs.Where(d => idPSs.Contains(d.Id)).ToList())
                {
                    item.WillDelete = true;
                }
            }
            await _yeuCauTiepNhanService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhan);

            return NoContent();
        }
    }
}