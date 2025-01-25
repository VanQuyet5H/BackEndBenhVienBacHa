using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.DieuTriNoiTru;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.Entities.InputStringStoreds;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.NoiTruBenhAn;
using Camino.Core.Domain.ValueObject.ToaThuocMau;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public partial class DieuTriNoiTruController
    {
        [HttpPost("ThemHoacCapNhatGiayRaVien")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> ThemHoacCapNhatGiayRaVien(GiayRaVienViewModel hoSoKhacViewModel)
        {
            await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(hoSoKhacViewModel.YeuCauTiepNhanId);
            var ghiChuExits = _dieuTriNoiTruService.KiemTraGhiChuRaVienTonTai(hoSoKhacViewModel.GhiChu);
            if (hoSoKhacViewModel.Id == 0) // Them
            {
                hoSoKhacViewModel.LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.GiayRaVien;
                hoSoKhacViewModel.NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
                hoSoKhacViewModel.NhanVienThucHienId = _userAgentHelper.GetCurrentUserId();


                if (ghiChuExits == false && hoSoKhacViewModel.GhiChu != null)
                {
                    var gc = new InputStringStored()
                    {
                        Set = Enums.InputStringStoredKey.GhiChuGiayRaVien,
                        Value = hoSoKhacViewModel.GhiChu
                    };
                    _inputStringStoredService.Add(gc);

                    var jsonData = JsonConvert.DeserializeObject<PhuongPhapDieuTriJSON>(hoSoKhacViewModel.ThongTinHoSo);
                    jsonData.GhiChu = gc.Value;
                    jsonData.IdGhiChu = gc.Id;
                    string jsons = JsonConvert.SerializeObject(jsonData);
                    hoSoKhacViewModel.ThongTinHoSo = jsons;

                }

                var noiTruHoSoKhac = hoSoKhacViewModel.ToEntity<NoiTruHoSoKhac>();
                await _noiTruHoSoKhacService.AddAsync(noiTruHoSoKhac);
                var resul = new
                {
                    noiTruHoSoKhac.Id,
                    noiTruHoSoKhac.LastModified,
                };

                return Ok(resul);
            }
            else // Update
            {
                var noiTruHoSoKhac = _noiTruHoSoKhacService.GetById(hoSoKhacViewModel.Id);
                hoSoKhacViewModel.NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
                hoSoKhacViewModel.NhanVienThucHienId = _userAgentHelper.GetCurrentUserId();
                hoSoKhacViewModel.LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.GiayRaVien;
                hoSoKhacViewModel.ToEntity(noiTruHoSoKhac);

                if (ghiChuExits == false && hoSoKhacViewModel.GhiChu != null)
                {
                    var gc = new InputStringStored()
                    {
                        Set = Enums.InputStringStoredKey.GhiChuGiayRaVien,
                        Value = hoSoKhacViewModel.GhiChu
                    };
                    _inputStringStoredService.Add(gc);

                    var jsonData = JsonConvert.DeserializeObject<PhuongPhapDieuTriJSON>(hoSoKhacViewModel.ThongTinHoSo);
                    jsonData.GhiChu = gc.Value;
                    jsonData.IdGhiChu = gc.Id;
                    string jsons = JsonConvert.SerializeObject(jsonData);
                    hoSoKhacViewModel.ThongTinHoSo = jsons;
                }


                await _noiTruHoSoKhacService.UpdateAsync(noiTruHoSoKhac);
                var resul = new
                {
                    noiTruHoSoKhac.Id,
                    noiTruHoSoKhac.LastModified,
                };
                return Ok(resul);
            }
        }

       

        

        [HttpGet("GetHoSoKhacGiayRaVien")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<GiayRaVienViewModel>> GetHoSoKhacGiayRaVien(long id)
        {
            var ycTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(id, s => s.Include(p => p.NoiTruHoSoKhacs));

            var hoSoGayMe = ycTiepNhan.NoiTruHoSoKhacs.FirstOrDefault(p => p.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayRaVien);

            if (hoSoGayMe == null)
            {
                return NoContent();
            }
            var model = hoSoGayMe.ToModel<GiayRaVienViewModel>();

            model.TenTruongKhoa = await _dieuTriNoiTruService.GetTenNhanVien(model.TruongKhoaId);
            model.TenGiamDocChuyenMon = await _dieuTriNoiTruService.GetTenNhanVien(model.GiamDocChuyenMonId);
            return Ok(model);
        }

        [HttpPost("ThongTinNhanVienDangNhapId")]
        public async Task<ActionResult> ThongTinNhanVienDangNhapId(long id)
        {
            var result = await _dieuTriNoiTruService.ThongTinNhanVienDangNhapId(id);
            return Ok(result);
        }

        [HttpPost("InGiayRaVien")]
        public ActionResult InGiayRaVien(long noiTruHoSoKhacId)
        {
            var result = _dieuTriNoiTruService.InGiayRaVien(noiTruHoSoKhacId);
            return Ok(result);
        }
        [HttpPost("GetListGhiChuGiayRaVien")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListNhomChucDanh([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _dieuTriNoiTruService.GetGhiChuGiayRaVien(queryInfo);
            foreach (var item in lookup)
            {
                //var tampKB = "\n";
                //var tmpKB = "<br>";
                //item.DisplayName = item.DisplayName.Replace(tmpKB, tampKB);
                var itemList = item.DisplayName.Split("\n");
                item.DisplayName = itemList.Join(" ");
                var itemBr = item.DisplayName.Split("<br>");
                item.DisplayName = itemBr.Join(" ");
            }
            return Ok(lookup);
        }
        [HttpPost("GetGhiChu")]
        public async Task<ActionResult<string>> GetGhiChu(long id)
        {
            var lookup = await _dieuTriNoiTruService.GetGhiChu(id);
            return Ok(lookup);
        }
        [HttpPost("GiamDocChuyenMon")]
        public async Task<ActionResult<ICollection<NhanVienTemplateVos>>> GiamDocChuyenMon([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _dieuTriNoiTruService.GetGiamDocChuyenMons(queryInfo);
            return Ok(lookup);
        }
    }
}
