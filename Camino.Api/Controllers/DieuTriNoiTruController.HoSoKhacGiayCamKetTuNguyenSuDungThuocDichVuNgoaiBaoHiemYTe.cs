using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.GiayCamKetTuNguyenSuDungThuocDichVuNgoaiBaoHiemYTe;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.ValueObject.GiayCamKetTuNguyenSuDungThuocDichVuNgoaiBaoHiemYTe;
using Camino.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public partial class DieuTriNoiTruController
    {

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GriDaTaSourceAllDichVuDaKhamNoiTru")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public ActionResult<GiayCamKetTuNguyenSuDungThuocDichVuNgoaiBaoHiemYTeGrid> GriDaTaSourceAllDichVuDaKhamNoiTru(long yeuCauTiepNhanId)
        {
            var listData = _dieuTriNoiTruService.GetListGiayCamKetTuNguyenSuDungThuocDichVuNgoaiBaoHiemYTeGridAsync(yeuCauTiepNhanId);
            GiayCamKetTuNguyenSuDungThuocDichVuNgoaiBaoHiemYTeGrid dataObject = new GiayCamKetTuNguyenSuDungThuocDichVuNgoaiBaoHiemYTeGrid();
            dataObject.DanhSachDichVuKyThuatThuocVatTuList = listData;
            long userId = _userAgentHelper.GetCurrentUserId();
            string nguoiLogin = _nhanVienService.GetByIdAsync(userId,s=>s.Include(p=>p.User)).Result.User.HoTen;
            dataObject.TenNhanVien = nguoiLogin;
            var ngayHienTai = new DateTime();
            ngayHienTai = DateTime.Now;
            dataObject.NgayThucHien = ngayHienTai.ApplyFormatDateTimeSACH();
            return dataObject;
        }
        [HttpPost("GetThongTinGiayCamKetTuNguyenSuDungThuocDVNgoaiBHYT")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public GiayCamKetTuNguyenSuDungThuocDichVuNgoaiBaoHiemYTeGridVo GetThongTinGiayCamKetTuNguyenSuDungThuocDVNgoaiBHYT(long yeuCauTiepNhanId)
        {
            var lookup = _dieuTriNoiTruService.GetThongTinGiayCamKetTuNguyenSuDungThuocDVNgoaiBHYT(yeuCauTiepNhanId);
            return lookup;
        }
        #region save / update
        [HttpPost("LuuPhieuGiayCamKetTuNguyenSuDungThuoc")]
        public async Task<ActionResult<GiayCamKetTuNguyenSuDungThuocDichVuNgoaiBaoHiemYTeViewModel>> LuuBangKiemAnToanPhauThuatTuPhongDieuTri([FromBody] GiayCamKetTuNguyenSuDungThuocDichVuNgoaiBaoHiemYTeViewModel giayCamKetTuNguyenSuDungThuocDichVuNgoaiBaoHiemYTeViewModel)
        {
            if (giayCamKetTuNguyenSuDungThuocDichVuNgoaiBaoHiemYTeViewModel.Id == 0)
            {
                var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
                var noiThucHien = _userAgentHelper.GetCurrentNoiLLamViecId();
                var trichBienBanNoiTru = new GiayCamKetTuNguyenSuDungThuocDichVuNgoaiBaoHiemYTeViewModel()
                {
                    YeuCauTiepNhanId = giayCamKetTuNguyenSuDungThuocDichVuNgoaiBaoHiemYTeViewModel.YeuCauTiepNhanId,
                    LoaiHoSoDieuTriNoiTru = giayCamKetTuNguyenSuDungThuocDichVuNgoaiBaoHiemYTeViewModel.LoaiHoSoDieuTriNoiTru,
                    ThongTinHoSo = giayCamKetTuNguyenSuDungThuocDichVuNgoaiBaoHiemYTeViewModel.ThongTinHoSo,
                    NhanVienThucHienId = nguoiDangLogin,
                    NoiThucHienId = noiThucHien,
                    ThoiDiemThucHien = DateTime.Now
                };
                var gcktnsdt = trichBienBanNoiTru.ToEntity<NoiTruHoSoKhac>();
                if (giayCamKetTuNguyenSuDungThuocDichVuNgoaiBaoHiemYTeViewModel.FileChuKy != null)
                {
                    foreach (var itemfileChuKy in giayCamKetTuNguyenSuDungThuocDichVuNgoaiBaoHiemYTeViewModel.FileChuKy)
                    {
                        var noiTruHoSoKhacFileDinhKem = new NoiTruHoSoKhacFileDinhKem()
                        {
                            //NoiTruHoSoKhacId = user.Id,
                            Ma = itemfileChuKy.Uid,
                            Ten = itemfileChuKy.Ten,
                            TenGuid = itemfileChuKy.TenGuid,
                            DuongDan = itemfileChuKy.DuongDan,
                            LoaiTapTin = itemfileChuKy.LoaiTapTin,
                            MoTa = itemfileChuKy.MoTa,
                            KichThuoc = itemfileChuKy.KichThuoc
                        };
                        _taiLieuDinhKemService.LuuTaiLieuAsync(noiTruHoSoKhacFileDinhKem.DuongDan, noiTruHoSoKhacFileDinhKem.TenGuid);
                        gcktnsdt.NoiTruHoSoKhacFileDinhKems.Add(noiTruHoSoKhacFileDinhKem);
                    }
                }
                await _noiDuTruHoSoKhacService.AddAsync(gcktnsdt);
                return CreatedAtAction(nameof(Get), new { id = gcktnsdt.Id }, gcktnsdt.ToModel<GiayCamKetTuNguyenSuDungThuocDichVuNgoaiBaoHiemYTeViewModel>());
            }
            else
            {
                var update = await _noiDuTruHoSoKhacService.GetByIdAsync(giayCamKetTuNguyenSuDungThuocDichVuNgoaiBaoHiemYTeViewModel.Id, s => s.Include(x => x.NoiTruHoSoKhacFileDinhKems));
                var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
                var noiThucHien = _userAgentHelper.GetCurrentNoiLLamViecId();
                update.YeuCauTiepNhanId = giayCamKetTuNguyenSuDungThuocDichVuNgoaiBaoHiemYTeViewModel.YeuCauTiepNhanId;
                update.LoaiHoSoDieuTriNoiTru = giayCamKetTuNguyenSuDungThuocDichVuNgoaiBaoHiemYTeViewModel.LoaiHoSoDieuTriNoiTru;
                update.ThongTinHoSo = giayCamKetTuNguyenSuDungThuocDichVuNgoaiBaoHiemYTeViewModel.ThongTinHoSo;
                update.NhanVienThucHienId = nguoiDangLogin;
                update.NoiThucHienId = noiThucHien;
                update.ThoiDiemThucHien = DateTime.Now;
                if (update == null)
                {
                    return NotFound();
                }
                giayCamKetTuNguyenSuDungThuocDichVuNgoaiBaoHiemYTeViewModel.ToEntity<NoiTruHoSoKhac>();
                // remove list fileChuKy hiện tại
                foreach (var itemNoiTruHoSoKhacFileDinhKem in update.NoiTruHoSoKhacFileDinhKems.ToList())
                {
                    var soket = await _noiDuTruHoSoKhacFileDinhKemService.GetByIdAsync(itemNoiTruHoSoKhacFileDinhKem.Id);
                    if (soket == null)
                    {
                        return NotFound();
                    }
                    await _noiDuTruHoSoKhacFileDinhKemService.DeleteByIdAsync(soket.Id);
                }
                if (giayCamKetTuNguyenSuDungThuocDichVuNgoaiBaoHiemYTeViewModel.Id != 0)
                {
                    if (giayCamKetTuNguyenSuDungThuocDichVuNgoaiBaoHiemYTeViewModel.FileChuKy != null)
                    {
                        foreach (var itemfileChuKy in giayCamKetTuNguyenSuDungThuocDichVuNgoaiBaoHiemYTeViewModel.FileChuKy)
                        {
                            var noiTruHoSoKhacFileDinhKem = new NoiTruHoSoKhacFileDinhKem()
                            {
                                Ma = itemfileChuKy.Uid,
                                Ten = itemfileChuKy.Ten,
                                TenGuid = itemfileChuKy.TenGuid,
                                DuongDan = itemfileChuKy.DuongDan,
                                LoaiTapTin = itemfileChuKy.LoaiTapTin,
                                MoTa = itemfileChuKy.MoTa,
                                KichThuoc = itemfileChuKy.KichThuoc
                            };
                            _taiLieuDinhKemService.LuuTaiLieuAsync(noiTruHoSoKhacFileDinhKem.DuongDan, noiTruHoSoKhacFileDinhKem.TenGuid);
                            update.NoiTruHoSoKhacFileDinhKems.Add(noiTruHoSoKhacFileDinhKem);
                        }
                    }

                }
               await _noiDuTruHoSoKhacService.UpdateAsync(update);
                return Ok(update.Id);
            }
            return null;
        }
        #endregion
        #region In 
        [HttpPost("InPhieuGiayCamKetTuNguyenSuDungThuoc")]
        public async Task<ActionResult<string>> InPhieuGiayCamKetTuNguyenSuDungThuoc([FromBody]XacNhanInPhieuGiayCamKetTuNguyenSuDungThuocDichVuNgoaiBaoHiemYTe xacNhanIn)
        {
            var html = await _dieuTriNoiTruService.InPhieuGiayCamKetTuNguyenSuDungThuoc(xacNhanIn);
            return html;
        }
        #endregion
    }
}
