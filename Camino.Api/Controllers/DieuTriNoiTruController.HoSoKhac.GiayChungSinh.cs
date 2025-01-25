using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.DieuTriNoiTru;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Camino.Api.Controllers
{
    public partial class DieuTriNoiTruController
    {
        [HttpPost("CheckValidationForChungSinh")]
        public ActionResult CheckValidationForChungSinh([FromBody]GiayChungSinhNewVo validationCheck)
        {
            return Ok(true);
        }
        [HttpPost("CheckValidationSoChungSinhExit")]
        public ActionResult CheckValidationSoChungSinhExit([FromBody]GiayChungSinhNewVo validationCheck)
        {
            var result = _dieuTriNoiTruService.kiemTrungSoChungSinh(validationCheck.So, validationCheck.YeuCauTiepNhanId, validationCheck.NoiTruHoSoKhacGiayChungSinhId);
            return Ok(result);
        }
        [HttpPost("ThemHoacCapNhatGiayChungSinh")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> ThemHoacCapNhatGiayChungSinh(GiayChungSinhNewViewModel hoSoKhacViewModel)
        {
            await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(hoSoKhacViewModel.YeuCauTiepNhanId);
            if (hoSoKhacViewModel.Id == 0)
            {
                var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
                var noiThucHien = _userAgentHelper.GetCurrentNoiLLamViecId();
                var trichBienBanNoiTru = new GiayChungSinhNewViewModel()
                {
                    YeuCauTiepNhanId = hoSoKhacViewModel.YeuCauTiepNhanId,
                    LoaiHoSoDieuTriNoiTru = hoSoKhacViewModel.LoaiHoSoDieuTriNoiTru,
                    ThongTinHoSo = hoSoKhacViewModel.ThongTinHoSo,
                    NhanVienThucHienId = nguoiDangLogin,
                    NoiThucHienId = noiThucHien,
                    ThoiDiemThucHien = DateTime.Now
                };
                var files = new List<NoiTruHoSoKhacFileDinhKem>();
                var user = trichBienBanNoiTru.ToEntity<NoiTruHoSoKhac>();

                if (hoSoKhacViewModel.FileDinhKems.Any())
                {
                    foreach (var itemfileDinhKem in hoSoKhacViewModel.FileDinhKems)
                    {
                        var noiTruHoSoKhacFileDinhKem = new NoiTruHoSoKhacFileDinhKem()
                        {
                            //NoiTruHoSoKhacId = user.Id,
                            Ma = itemfileDinhKem.Uid,
                            Ten = itemfileDinhKem.Ten,
                            TenGuid = itemfileDinhKem.TenGuid,
                            DuongDan = itemfileDinhKem.DuongDan,
                            LoaiTapTin = itemfileDinhKem.LoaiTapTin,
                            MoTa = itemfileDinhKem.MoTa,
                            KichThuoc = itemfileDinhKem.KichThuoc
                        };
                        _taiLieuDinhKemService.LuuTaiLieuAsync(noiTruHoSoKhacFileDinhKem.DuongDan, noiTruHoSoKhacFileDinhKem.TenGuid);
                        user.NoiTruHoSoKhacFileDinhKems.Add(noiTruHoSoKhacFileDinhKem);
                    }
                }
                await _noiDuTruHoSoKhacService.AddAsync(user);
                return CreatedAtAction(nameof(Get), new { id = user.Id }, user.ToModel<GiayChungSinhNewViewModel>());
            }
            else
            {
                var update = await _noiDuTruHoSoKhacService.GetByIdAsync(hoSoKhacViewModel.Id ,s => s.Include(d => d.NoiTruHoSoKhacFileDinhKems));
                var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
                var noiThucHien = _userAgentHelper.GetCurrentNoiLLamViecId();
                update.YeuCauTiepNhanId = hoSoKhacViewModel.YeuCauTiepNhanId;
                update.LoaiHoSoDieuTriNoiTru = hoSoKhacViewModel.LoaiHoSoDieuTriNoiTru;
                update.ThongTinHoSo = hoSoKhacViewModel.ThongTinHoSo;
                update.NhanVienThucHienId = nguoiDangLogin;
                update.NoiThucHienId = noiThucHien;
                update.ThoiDiemThucHien = DateTime.Now;
                if (update == null)
                {
                    return NotFound();
                }
                hoSoKhacViewModel.ToEntity<NoiTruHoSoKhac>();
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
                if (hoSoKhacViewModel.Id != 0 )
                {
                    if(hoSoKhacViewModel.FileDinhKems != null && hoSoKhacViewModel.FileDinhKems.Count() != 0)
                    {
                        foreach (var itemfileDinhKem in hoSoKhacViewModel.FileDinhKems.ToList())
                        {
                            var noiTruHoSoKhacFileDinhKem = new NoiTruHoSoKhacFileDinhKem()
                            {
                                Ma = itemfileDinhKem.Uid,
                                Ten = itemfileDinhKem.Ten,
                                TenGuid = itemfileDinhKem.TenGuid,
                                DuongDan = itemfileDinhKem.DuongDan,
                                LoaiTapTin = itemfileDinhKem.LoaiTapTin,
                                MoTa = itemfileDinhKem.MoTa,
                                KichThuoc = itemfileDinhKem.KichThuoc
                            };
                            _taiLieuDinhKemService.LuuTaiLieuAsync(noiTruHoSoKhacFileDinhKem.DuongDan, noiTruHoSoKhacFileDinhKem.TenGuid);
                            update.NoiTruHoSoKhacFileDinhKems.Add(noiTruHoSoKhacFileDinhKem);
                        }
                    }
                }
                await _noiDuTruHoSoKhacService.UpdateAsync(update);
                return Ok(update.Id);
            }
        }

        [HttpGet("GetHoSoKhacGiayChungSinh")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<GiayChungSinhNewVo>> GetHoSoKhacGiayChungSinh(long id, long? noiTruHoSoKhacId)
        {
            var ycTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(id, s => s.Include(p => p.NoiTruHoSoKhacs).ThenInclude(d=>d.NoiTruHoSoKhacFileDinhKems));
            var hoSoChungSinh= ycTiepNhan.NoiTruHoSoKhacs.Where(p => p.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayChungSinh);
            
            if (hoSoChungSinh == null)
            {
                return NoContent();
            }
            // view 
            if (noiTruHoSoKhacId != null && noiTruHoSoKhacId != 0)
            {
                var noiTruHoSoKhacIdInfo = hoSoChungSinh.Where(d => d.Id == noiTruHoSoKhacId).Select(d => d.ThongTinHoSo).FirstOrDefault();

                if (!string.IsNullOrEmpty(noiTruHoSoKhacIdInfo))
                {
                    var json = JsonConvert.DeserializeObject<GiayChungSinhNewVo>(noiTruHoSoKhacIdInfo);
                    var chungSinhConTheoYeuCauTiepNhan = new GiayChungSinhNewVo();
                    chungSinhConTheoYeuCauTiepNhan = json;

                    var gt = string.Empty;
                    switch (json.GioiTinh)
                    {
                        case "Trai":
                            gt = "Nam";
                            break;
                        case "Gái":
                            gt = "Nữ";
                            break;
                        case "Nam":
                            gt = "Nam";
                            break;
                        case "Nữ":
                            gt = "Nữ";
                            break;

                        default:
                            gt = "Không xác định";
                            break;
                    }
                    chungSinhConTheoYeuCauTiepNhan.GioiTinh = gt;
                    chungSinhConTheoYeuCauTiepNhan.NoiTruHoSoKhacId = (long)noiTruHoSoKhacId;
                    chungSinhConTheoYeuCauTiepNhan.NoiTruHoSoKhacGiayChungSinhId = (long)noiTruHoSoKhacId;
                    if (hoSoChungSinh.Select(d => d.NoiTruHoSoKhacFileDinhKems).Any())
                    {
                        chungSinhConTheoYeuCauTiepNhan.FileDinhKems = hoSoChungSinh.Where(d=>d.Id == noiTruHoSoKhacId).SelectMany(d => d.NoiTruHoSoKhacFileDinhKems)
                                                                                    .Select(d=> new FileDinhKemViewModel() {
                                                                                        Ma = d.Ma,
                                                                                        Ten = d.Ten,
                                                                                        TenGuid = d.TenGuid,
                                                                                        DuongDan = d.DuongDan,
                                                                                        LoaiTapTin = d.LoaiTapTin,
                                                                                        MoTa = d.MoTa,
                                                                                        KichThuoc = d.KichThuoc,
                                                                                        Id = d.Id
                                                                                    }).ToList();
                        //gắn upload file nhớ gắn Id cho file đính kèm
                    }
                    #region dành data đã kết thúc bệnh án , k có save TenGiamDocChuyenMon,TenNhanVienDoDe,TenNhanVienGhiPhieu
                    if (string.IsNullOrEmpty(chungSinhConTheoYeuCauTiepNhan.TenGiamDocChuyenMon) && chungSinhConTheoYeuCauTiepNhan.GiamDocChuyenMonId != null)
                    {
                        chungSinhConTheoYeuCauTiepNhan.TenGiamDocChuyenMon = _dieuTriNoiTruService.GetNameBacSi(chungSinhConTheoYeuCauTiepNhan.GiamDocChuyenMonId.GetValueOrDefault());
                    }
                    if (string.IsNullOrEmpty(chungSinhConTheoYeuCauTiepNhan.TenNhanVienDoDe) && chungSinhConTheoYeuCauTiepNhan.NhanVienDoDeId != null)
                    {
                        chungSinhConTheoYeuCauTiepNhan.TenNhanVienDoDe = _dieuTriNoiTruService.GetNameBacSi(chungSinhConTheoYeuCauTiepNhan.NhanVienDoDeId.GetValueOrDefault());
                    }
                    if (string.IsNullOrEmpty(chungSinhConTheoYeuCauTiepNhan.TenNhanVienGhiPhieu) && chungSinhConTheoYeuCauTiepNhan.NhanVienGhiPhieuId != null)
                    {
                        chungSinhConTheoYeuCauTiepNhan.TenNhanVienGhiPhieu = _dieuTriNoiTruService.GetNameBacSi(chungSinhConTheoYeuCauTiepNhan.NhanVienGhiPhieuId.GetValueOrDefault());
                    }
                    #endregion
                    return chungSinhConTheoYeuCauTiepNhan;
                }

            }
            // get thông tin mặc định của mẹ
            var lstChungSinhConTheoYeuCauTiepNhans = new GiayChungSinhNewVo()
            {
                CMND = ycTiepNhan.SoChungMinhThu,
                NgayCapGiayChungSinh = DateTime.Now,
                TrangThaiLuu = 1 // mặc định create
                //NgayCap = ycTiepNhan. //k có
            };
            return Ok(lstChungSinhConTheoYeuCauTiepNhans);
        }

        [HttpGet("GetDSGiayChungSinhCon")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<GiayChungSinhNewVo>> GetDSGiayChungSinhCon(long id)
        {
            var ycTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(id, s => s.Include(p => p.NoiTruHoSoKhacs).ThenInclude(d => d.NoiTruHoSoKhacFileDinhKems));
            var hoSoChungSinh = ycTiepNhan.NoiTruHoSoKhacs.Where(p => p.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayChungSinh);

            if (hoSoChungSinh == null)
            {
                return NoContent();
            }

            var lstChungSinhConTheoYeuCauTiepNhans = new List<DSGiayChungSinhNewConVo>();
            if (hoSoChungSinh.Any())
            {
                var data = hoSoChungSinh.Select(d => new {
                    Id = d.Id,
                    ThongTinHoSo = d.ThongTinHoSo
                }).ToList();
                foreach (var item in data.ToList())
                {
                    var json = JsonConvert.DeserializeObject<GiayChungSinhNewVo>(item.ThongTinHoSo);

                    var gt = string.Empty;
                    switch (json.GioiTinh)
                    {
                        case "Trai":
                            gt = "Nam";
                            break;
                        case "Gái":
                            gt = "Nữ";
                            break;
                        case "Nam":
                            gt = "Nam";
                            break;
                        case "Nữ":
                            gt = "Nữ";
                            break;
                        default:
                            gt = "Không xác định";
                            break;
                    }

                    var shungSinhConTheoYeuCauTiepNhan = new DSGiayChungSinhNewConVo() {
                        NoiTruHoSoKhacId = item.Id,
                        DuDinhDatTenCon = json.DuDinhDatTenCon,
                        GioiTinh = gt,
                        CanNang = json.CanNang,
                        TrangThaiLuu = json.TrangThaiLuu,
                        So = json.So,
                        QuyenSo = json.QuyenSo
                    };
                    lstChungSinhConTheoYeuCauTiepNhans.Add(shungSinhConTheoYeuCauTiepNhan);
                }
            }

            return Ok(lstChungSinhConTheoYeuCauTiepNhans);
        }


        [HttpPost("ThongTinNhanVienDangNhapIdTreSoSinh")]
        public async Task<ActionResult> ThongTinNhanVienDangNhapIdTreSoSinh(long id)
        {
            var result = await _dieuTriNoiTruService.ThongTinNhanVienDangNhapIdTreSoSinh(id);
            return Ok(result);
        }


        [HttpPost("InGiayChungSinh")]
        public ActionResult InGiayChungSinh([FromBody]InGiayChungSinhQueryInfo info)
        {
            var result = _dieuTriNoiTruService.InGiayChungSinh(info.NoiTruHoSoKhacId, info.Hosting);
            return Ok(result);
        }

        [HttpPost("xoaChungSinh")]
        public async Task<ActionResult> xoaChungSinh(long id)
        {
            var soket = await _noiDuTruHoSoKhacService.GetByIdAsync(id, s => s.Include(k => k.NoiTruHoSoKhacFileDinhKems));
            if (soket == null)
            {
                return NotFound();
            }
            await _noiDuTruHoSoKhacService.DeleteByIdAsync(id);
            return NoContent();
        }
        [HttpPost("InGiayChungSinhTatCa")]
        public ActionResult InGiayChungSinhTatCa([FromBody]InGiayChungSinhQueryInfo info)
        {
            var result = _dieuTriNoiTruService.InGiayChungSinhTatCa(info.NoiTruHoSoKhacId, info.Hosting);
            return Ok(result);
        }

        #region get lis thon tin BA con
        [HttpPost("GetDataInfoBACon")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<ICollection<InfoBAConGridVo>>> GetDataInfoBACon([FromBody]DropDownListRequestModel queryInfo, long yeuCauTiepNhanId)
        {
            var lookup = await _dieuTriNoiTruService.GetDataInfoBACon(queryInfo, yeuCauTiepNhanId);
            return Ok(lookup);
        }
        #endregion
    }
}
