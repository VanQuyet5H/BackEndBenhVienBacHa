using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Models.PhauThuatThuThuat;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.PhauThuatThuThuat;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Camino.Api.Extensions;
using Camino.Api.Models.Error;
using Camino.Core.Domain.Entities.KetQuaSinhHieus;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.Controllers
{
    public partial class PhauThuatThuThuatController
    {
        #region GetThongTinKhoaPhong
        [HttpGet("GetThongTinKhoa")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ThongTinKhoaPhongVo> GetThongTinKhoa(long phongBenhVienId, long? ycdvktId)
        {
            var khoaPhong = await _phauThuatThuThuatService.GetThongTinKhoa(phongBenhVienId, ycdvktId);
            return khoaPhong;
        }
        #endregion

        #region progress operation
        [HttpPost("StartOperation")]
        // [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult> StartOperation(
            [FromBody] PhauThuatThuThuatTuongTrinhViewModel phauThuatThuThuat)
        {
            var result = await _phauThuatThuThuatService.StartPhauThuat(phauThuatThuThuat.YeuCauDichVuKyThuatId);
            return Ok(result);
        }

        [HttpPost("SaveOperation")]
        // [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.PhauThuatThuThuatTheoNgay)]
        public ActionResult SaveOperation(
            [FromBody] PhauThuatThuThuatTuongTrinhViewModel phauThuatThuThuat)
        {
            //var result = await _phauThuatThuThuatService.SaveOperation(phauThuatThuThuat.YeuCauDichVuKyThuatId);
            return Ok();
        }

        [HttpPost("FinishOperation")]
        // [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult> FinishOperation(
            [FromBody] PhauThuatThuThuatTuongTrinhViewModel phauThuatThuThuat)
        {
            var result = await _phauThuatThuThuatService.FinishOperation(phauThuatThuThuat.YeuCauDichVuKyThuatId);
            return Ok(result);
        }
        #endregion

        #region Get Thông tin dùng chung 
        [HttpGet("GetThongTinChiDinhDichVu")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult<ThongTinChiDinhDichVuVo>> GetThongTinChiDinhDichVu(long yeuCauDichVuKyThuatId)
        {
            var lstThoiGianTuVong = await _phauThuatThuThuatService.GetThongTinChiDinhDichVu(yeuCauDichVuKyThuatId);
            return Ok(lstThoiGianTuVong);
        }

        [HttpPost("GetThongTinBoPhanCoThe")]
        public List<ThongTinBoPhanCoTheModel> GetThongTinBoPhanCoThe()
        {
            //bao gồm mỗi bộ phận sẽ đi theo 1 hinh ảnh cụ thể
            List<ThongTinBoPhanCoTheModel> thongTinBoPhanCoThe = new List<ThongTinBoPhanCoTheModel>()
                {
                    new ThongTinBoPhanCoTheModel { Id=1 , DisplayName="Thông tin về phổi", Url="https://s3.ap-southeast-1.amazonaws.com/hapu.tmp/ed8a5c25-ccdc-43a1-9c95-861d393bc8ad.jpg?X-Amz-Expires=3600&X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=AKIA3IYDN5EFFVUFKEED/20200605/ap-southeast-1/s3/aws4_request&X-Amz-Date=20200605T084440Z&X-Amz-SignedHeaders=host&X-Amz-Signature=4e1e95e043dbedb9fce47e433e9d7073bad1b13a1f33c6c9cb083f4335d7c9ee" },
                    new ThongTinBoPhanCoTheModel { Id=2,DisplayName="Thông tin về thận", Url="https://s3.ap-southeast-1.amazonaws.com/hapu.tmp/cf7232fe-d629-4989-96ce-488a62568db1.jpg?X-Amz-Expires=3600&X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=AKIA3IYDN5EFFVUFKEED/20200605/ap-southeast-1/s3/aws4_request&X-Amz-Date=20200605T084637Z&X-Amz-SignedHeaders=host&X-Amz-Signature=ecfcd44d3202906f797b8ab6bc40394d5d61e660908943039878d8b85bba94fd" },
                    new ThongTinBoPhanCoTheModel { Id=3, DisplayName="Thông tin về gan", Url="https://s3.ap-southeast-1.amazonaws.com/hapu.tmp/54e5aab6-69c9-4c99-bee1-1aae2495802e.jpg?X-Amz-Expires=3600&X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=AKIA3IYDN5EFFVUFKEED/20200605/ap-southeast-1/s3/aws4_request&X-Amz-Date=20200605T084720Z&X-Amz-SignedHeaders=host&X-Amz-Signature=1772b9c1103132e1bdb87cfabd4fa8d61e19560f919347c456152ac91531ac3a" },
                };

            return thongTinBoPhanCoThe;
        }

        [HttpPost("GetListThoiGianTuVongPTTTTrongNgay")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.PhauThuatThuThuatTheoNgay)]
        public ActionResult GetListThoiGianTuVongPtttTrongNgay([FromBody] LookupQueryInfo model)
        {
            var lstThoiGianTuVong = _phauThuatThuThuatService.GetListThoiGianTuVongPTTTTrongNgay(model);
            return Ok(lstThoiGianTuVong);
        }

        [HttpPost("GetListTuVongPTTTTrongNgay")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.PhauThuatThuThuatTheoNgay)]
        public ActionResult GetListTuVongPtttTrongNgay([FromBody] LookupQueryInfo model)
        {
            var lstTuVong = _phauThuatThuThuatService.GetListTuVongPTTTTrongNgay(model);
            return Ok(lstTuVong);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("SaveTuongTrinh")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult> SaveTuongTrinh([FromBody]PhauThuatThuThuatTuongTrinhViewModel model)
        {
            var yeuCauDichVuKyThuat = await _yeuCauDichVuKyThuatService.GetByIdAsync(model.YeuCauDichVuKyThuatId, x => x.Include(w => w.YeuCauDichVuKyThuatTuongTrinhPTTT)
                .ThenInclude(z => z.YeuCauDichVuKyThuatLuocDoPhauThuats));

            if (yeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT == null)
            {
                yeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT = new YeuCauDichVuKyThuatTuongTrinhPTTT
                {
                    Id = model.YeuCauDichVuKyThuatId,
                    GhiChuICDSauPhauThuat = model.MoTaCDSauPhauThuat,
                    GhiChuICDTruocPhauThuat = model.MoTaCDTruocPhauThuat,
                    ICDSauPhauThuatId = model.ICDSauId,
                    ICDTruocPhauThuatId = model.ICDTruocId,
                    LoaiPhauThuatThuThuat = model.LoaiPttt,
                    MaPhuongPhapPTTT = model.MaPttt,
                    PhuongPhapVoCamId = model.PpVoCamId,
                    TrinhTuPhauThuat = model.TrinhTuPttt,
                    TaiBienPTTT = model.TaiBienPttt,
                    TinhHinhPTTT = model.TinhHinhPttt,
                    TenPhuongPhapPTTT = model.PhuongPhapPttt,
                    ThoiDiemPhauThuat = model.ThoiGianPt,
                    ThoiGianBatDauGayMe = model.ThoiGianBatDauGayMe,
                    ThoiDiemKetThucPhauThuat = model.ThoiGianKetThucPt
                };
            }
            else
            {
                yeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.GhiChuICDSauPhauThuat =
                    model.MoTaCDSauPhauThuat;
                yeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.GhiChuICDTruocPhauThuat =
                    model.MoTaCDTruocPhauThuat;
                yeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.ICDSauPhauThuatId =
                    model.ICDSauId;
                yeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.ICDTruocPhauThuatId =
                    model.ICDTruocId;
                yeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.LoaiPhauThuatThuThuat =
                    model.LoaiPttt;
                yeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.MaPhuongPhapPTTT =
                    model.MaPttt;
                yeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.PhuongPhapVoCamId =
                    model.PpVoCamId;
                yeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.TrinhTuPhauThuat =
                    model.TrinhTuPttt;
                yeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.TaiBienPTTT =
                    model.TaiBienPttt;
                yeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.TinhHinhPTTT =
                    model.TinhHinhPttt;
                yeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.TenPhuongPhapPTTT =
                    model.PhuongPhapPttt;
                yeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemPhauThuat =
                    model.ThoiGianPt;
                yeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiGianBatDauGayMe =
                    model.ThoiGianBatDauGayMe;
                yeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemKetThucPhauThuat =
                    model.ThoiGianKetThucPt;
            }

            #region BVHD-3882
            //Cập nhật: bỏ đồng bộ thời gian thuê phòng và thời gian PTTTT
            //if (yeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT != null)
            //{
            //    var thuePhong =
            //        await _phauThuatThuThuatService.GetThongTinThuePhongTheoDichVuKyThuatFoUpdateAsync(
            //            yeuCauDichVuKyThuat.Id);
            //    if (thuePhong != null)
            //    {
            //        if (thuePhong.YeuCauDichVuKyThuatTinhChiPhi.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
            //        {
            //            throw new ApiException(
            //                _localizationService.GetResource("PhauThuatThuThuatThuePhong.DichVu.DaHuy"));
            //        }
            //        if (thuePhong.YeuCauDichVuKyThuatTinhChiPhi.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
            //        {
            //            throw new ApiException(
            //                _localizationService.GetResource("PhauThuatThuThuatThuePhong.ThuePhong.DaThanhToan"));
            //        }

            //        if (model.ThoiGianPt == null || model.ThoiGianKetThucPt == null)
            //        {
            //            throw new ApiException(
            //                _localizationService.GetResource("PhauThuatThuThuatThuePhong.ThoiGianThuePhong.Required"));
            //        }

            //        thuePhong.ThoiDiemBatDau = model.ThoiGianPt.Value;
            //        thuePhong.ThoiDiemKetThuc = model.ThoiGianKetThucPt.Value;
            //        thuePhong.YeuCauDichVuKyThuatTinhChiPhi.Gia = await _cauHinhService.GetDonGiaThuePhongAsync(thuePhong);

            //        thuePhong.YeuCauDichVuKyThuatThuePhong.ThoiDiemChiDinh = model.ThoiGianPt.Value;
            //        thuePhong.YeuCauDichVuKyThuatThuePhong.ThoiDiemDangKy = model.ThoiGianPt.Value;
            //        thuePhong.YeuCauDichVuKyThuatThuePhong.ThoiDiemThucHien = model.ThoiGianPt.Value;
            //        thuePhong.YeuCauDichVuKyThuatThuePhong.ThoiDiemKetLuan = model.ThoiGianKetThucPt.Value;
            //        thuePhong.YeuCauDichVuKyThuatThuePhong.ThoiDiemHoanThanh = model.ThoiGianKetThucPt.Value;
            //    }
            //}
            #endregion

            if (yeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.YeuCauDichVuKyThuatLuocDoPhauThuats == null ||
                    yeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.YeuCauDichVuKyThuatLuocDoPhauThuats.Count == 0)
            {
                if (model.LuocDoPhauThuats.Any())
                {
                    foreach (var luocDoItem in model.LuocDoPhauThuats)
                    {
                        luocDoItem.IdYeuCauDichVuKyThuat = model.YeuCauDichVuKyThuatId;
                        yeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.YeuCauDichVuKyThuatLuocDoPhauThuats?.Add(new YeuCauDichVuKyThuatLuocDoPhauThuat
                        {
                            Id = luocDoItem.Id,
                            YeuCauDichVuKyThuatTuongTrinhPTTTId = luocDoItem.IdYeuCauDichVuKyThuat.GetValueOrDefault(),
                            LuocDo = luocDoItem.LuocDoPhauThuat,
                            MoTa = luocDoItem.MoTa
                        });
                    }
                }
            }
            else
            {
                if (model.LuocDoPhauThuats.Any())
                {
                    foreach (var luocDoEntityItem in yeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.YeuCauDichVuKyThuatLuocDoPhauThuats)
                    {
                        luocDoEntityItem.WillDelete = true;
                    }

                    foreach (var luocDoEntityItem in yeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.YeuCauDichVuKyThuatLuocDoPhauThuats)
                    {
                        foreach (var phauThuatViewModelItem in model.LuocDoPhauThuats)
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

            await _yeuCauDichVuKyThuatService.UpdateAsync(yeuCauDichVuKyThuat);

            return Ok();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("HuyTuongTrinh")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult> HuyTuongTrinh([FromBody]RutTuongTrinhViewModel model)
        {
            var dvkt = await _yeuCauDichVuKyThuatService.GetByIdAsync(model.YcdvktId, w => w
                .Include(q => q.YeuCauDichVuKyThuatTuongTrinhPTTT)
            
                //BVHD-3860
                .Include(a => a.YeuCauDuocPhamBenhViens)
                .Include(a => a.YeuCauVatTuBenhViens));

            #region BVHD-3882
            var thuePhong = await _phauThuatThuThuatService.GetThongTinThuePhongTheoDichVuKyThuatAsync(model.YcdvktId);
            if (thuePhong != null)
            {
                throw new ApiException(_localizationService.GetResource("PhauThuatThuThuatThuePhong.HuyTuongTrinh.CoThuePhong"));
            }
            

            #endregion

            dvkt.TrangThai = Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien;

            var userId = _userAgentHelper.GetCurrentUserId();

            if (dvkt.YeuCauDichVuKyThuatTuongTrinhPTTT == null)
            {
                dvkt.YeuCauDichVuKyThuatTuongTrinhPTTT = new YeuCauDichVuKyThuatTuongTrinhPTTT
                {
                    Id = 0,
                    KhongThucHien = true,
                    LyDoKhongThucHien = model.GhiChu,
                    NhanVienTuongTrinhId = userId,
                    ThoiDiemKetThucTuongTrinh = DateTime.Now
                };
            }
            else
            {
                dvkt.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien = true;
                dvkt.YeuCauDichVuKyThuatTuongTrinhPTTT.LyDoKhongThucHien = model.GhiChu;
                dvkt.YeuCauDichVuKyThuatTuongTrinhPTTT.NhanVienTuongTrinhId = userId;
                dvkt.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemKetThucTuongTrinh = DateTime.Now;
            }

            #region BVHD-3860
            string templateKeyId = "\"Id\": {0}, \"NhomId\": {1}";
            var lstThuocVatTuCanXoa =
                dvkt.YeuCauDuocPhamBenhViens
                    .Where(a => a.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy)
                    .Select(a => "{" + string.Format(templateKeyId, a.Id, (int)Enums.EnumNhomGoiDichVu.DuocPham) + "}")
                    .Union(
                        dvkt.YeuCauVatTuBenhViens
                            .Where(a => a.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy)
                            .Select(a => "{" + string.Format(templateKeyId, a.Id, (int)Enums.EnumNhomGoiDichVu.VatTuTieuHao) + "}")
                    )
                    .Distinct().ToList();
            if (model.XoaThuocVaTu == true && lstThuocVatTuCanXoa.Any())
            {
                var yeuCauTiepNhanChiTiet = _phauThuatThuThuatService.GetYeuCauTiepNhanForGhiNhanVatTuThuoc(dvkt.YeuCauTiepNhanId);

                // xử lý xóa yeu cau duoc pham/ vat tu
                var strYeuCauCanXoa = string.Join(";", lstThuocVatTuCanXoa);
                await _khamBenhService.XuLyXoaYeuCauGhiNhanVTTHThuocAsync(yeuCauTiepNhanChiTiet, strYeuCauCanXoa);

                await _tiepNhanBenhNhanService.PrepareForDeleteDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);
            }
            #endregion

            else
            {
                await _yeuCauDichVuKyThuatService.UpdateAsync(dvkt);
            }
            return Ok();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetListTuongTrinhHuy")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult> GetListTuongTrinhHuy(long? noiThucHienId, long? yctnId)
        {
            var tonTaiHuy = await _yeuCauDichVuKyThuatService.GetListTuongTrinhHuy(noiThucHienId, yctnId);
            return Ok(tonTaiHuy);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("SaveTuongTrinhTuVong")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult> SaveTuongTrinhTuVong([FromBody]TuongTrinhTuVongViewModel model)
        {
            var yeuCauDichVuKyThuatTuongTrinhPttt = await _yeuCauDichVuKyThuatTuongTrinhPtttService.GetByIdAsync(model.IdDvkt.GetValueOrDefault(), e => e
                .Include(q => q.YeuCauDichVuKyThuat).ThenInclude(q => q.YeuCauTiepNhan));
            yeuCauDichVuKyThuatTuongTrinhPttt.KhoangThoiGianTuVong = model.TgTuVong;
            yeuCauDichVuKyThuatTuongTrinhPttt.TuVongTrongPTTT = model.TuVong;
            yeuCauDichVuKyThuatTuongTrinhPttt.ThoiDiemKetThucTuongTrinh = DateTime.Now;
            yeuCauDichVuKyThuatTuongTrinhPttt.YeuCauDichVuKyThuat.YeuCauTiepNhan.TuVongTrongPTTT = model.TuVong;
            yeuCauDichVuKyThuatTuongTrinhPttt.YeuCauDichVuKyThuat.YeuCauTiepNhan.KhoangThoiGianTuVong = model.TgTuVong;
            yeuCauDichVuKyThuatTuongTrinhPttt.YeuCauDichVuKyThuat.YeuCauTiepNhan.ThoiDiemTuVong = DateTime.Now;
            yeuCauDichVuKyThuatTuongTrinhPttt.YeuCauDichVuKyThuat.TrangThai =
                Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien;
            yeuCauDichVuKyThuatTuongTrinhPttt.YeuCauDichVuKyThuat.ThoiDiemHoanThanh = DateTime.Now;
            yeuCauDichVuKyThuatTuongTrinhPttt.YeuCauDichVuKyThuat.NhanVienKetLuanId = _userAgentHelper.GetCurrentUserId();
            //Update LanThucHien 09/16
            //yeuCauDichVuKyThuatTuongTrinhPttt.YeuCauDichVuKyThuat.LanThucHien = yeuCauDichVuKyThuatTuongTrinhPttt.YeuCauDichVuKyThuat.LanThucHien != null ? ++yeuCauDichVuKyThuatTuongTrinhPttt.YeuCauDichVuKyThuat.LanThucHien : 0;

            var noiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var yctn = await _yeuCauTiepNhanService.GetByIdAsync(yeuCauDichVuKyThuatTuongTrinhPttt.YeuCauDichVuKyThuat
                .YeuCauTiepNhan.Id, e => e.Include(q => q.YeuCauDichVuKyThuats).ThenInclude(q => q.YeuCauDichVuKyThuatTuongTrinhPTTT));

            var xuLyChoYcdvktConLai = yctn.YeuCauDichVuKyThuats
                .Where(w => w.NoiThucHienId == noiThucHienId &&
                            (w.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien ||
                             w.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien) &&
                            w.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat &&
                            w.YeuCauDichVuKyThuatTuongTrinhPTTT?.KhongThucHien != true &&
                            w.YeuCauDichVuKyThuatTuongTrinhPTTT?.ThoiDiemKetThucTuongTrinh == null &&
                            w.Id != model.IdDvkt.GetValueOrDefault());
            
            var xuLyDaTuongTrinhChoYcdvktConLai = yctn.YeuCauDichVuKyThuats
                .Where(w => w.NoiThucHienId == noiThucHienId &&
                            (w.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien ||
                             w.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien) &&
                            w.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat &&
                            w.Id != model.IdDvkt.GetValueOrDefault() && 
                            w.YeuCauDichVuKyThuatTuongTrinhPTTT?.KhongThucHien != true &&
                            w.YeuCauDichVuKyThuatTuongTrinhPTTT?.ThoiDiemKetThucTuongTrinh != null);

            foreach (var ycdvkt in xuLyChoYcdvktConLai)
            {
                if (ycdvkt.YeuCauDichVuKyThuatTuongTrinhPTTT == null)
                {
                    ycdvkt.YeuCauDichVuKyThuatTuongTrinhPTTT = new YeuCauDichVuKyThuatTuongTrinhPTTT
                    {
                        Id = 0,
                        ThoiDiemKetThucTuongTrinh = DateTime.Now,
                        KhongThucHien = true,
                        LyDoKhongThucHien = "Tử vong",
                        NhanVienTuongTrinhId = _userAgentHelper.GetCurrentUserId()
                    };
                }
                else
                {
                    ycdvkt.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemKetThucTuongTrinh = DateTime.Now;
                    ycdvkt.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien = true;
                    ycdvkt.YeuCauDichVuKyThuatTuongTrinhPTTT.LyDoKhongThucHien = "Tử vong";
                    ycdvkt.YeuCauDichVuKyThuatTuongTrinhPTTT.NhanVienTuongTrinhId = _userAgentHelper.GetCurrentUserId();
                }

                ycdvkt.TrangThai = Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien;
            }

            foreach (var daTuongTrinh in xuLyDaTuongTrinhChoYcdvktConLai)
            {
                daTuongTrinh.ThoiDiemHoanThanh = DateTime.Now;
                daTuongTrinh.NhanVienKetLuanId = _userAgentHelper.GetCurrentUserId();
                //Update LanThucHien 09/16
                //daTuongTrinh.LanThucHien = daTuongTrinh.LanThucHien != null ? ++daTuongTrinh.LanThucHien : 0;
                daTuongTrinh.TrangThai =
                    Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien;
            }

            await _yeuCauDichVuKyThuatTuongTrinhPtttService.UpdateAsync(yeuCauDichVuKyThuatTuongTrinhPttt);
            return Ok();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("LoadTuVong")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult> LoadTuVong(long ycdvktId)
        {
            var tuVongResult = await _yeuCauDichVuKyThuatTuongTrinhPtttService.GetTuVong(ycdvktId);
            return Ok(tuVongResult);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("KetThucTuongTrinh")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult> KetThucTuongTrinh(long ycdvktId)
        {
            await _yeuCauDichVuKyThuatTuongTrinhPtttService.KetThucTuongTrinh(ycdvktId);
            return Ok();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("SaveChiSoSinhHieu")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult> SaveChiSoSinhHieu([FromBody]ChiSoSinhHieuForRequestViewModel chiSoSinhHieuRequestModel)
        {
            var yctnEntity = await _yctnService.GetByIdAsync(chiSoSinhHieuRequestModel.YctnId,
                w => w.Include(e => e.KetQuaSinhHieus));
            foreach (var chiSoSinhHieuViewModel in chiSoSinhHieuRequestModel.ChiSoSinhHieus.Where(e => e.Id == 0))
            {
                var chiSoSinhHieu = new KetQuaSinhHieu();
                chiSoSinhHieuViewModel.ToEntity(chiSoSinhHieu);
                chiSoSinhHieu.YeuCauTiepNhanId = chiSoSinhHieuRequestModel.YctnId;
                chiSoSinhHieu.NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
                chiSoSinhHieu.NhanVienThucHienId = _userAgentHelper.GetCurrentUserId();
                yctnEntity.KetQuaSinhHieus.Add(chiSoSinhHieu);
            }

            var modifyIds = new List<long>();
            foreach (var chiSoSinhHieuViewModel in chiSoSinhHieuRequestModel.ChiSoSinhHieus.Where(e => e.Id != 0))
            {
                foreach (var chiSoSinhHieu in yctnEntity.KetQuaSinhHieus.Where(w => w.Id != 0 && w.Id == chiSoSinhHieuViewModel.Id))
                {
                    chiSoSinhHieuViewModel.ToEntity(chiSoSinhHieu);
                    chiSoSinhHieu.YeuCauTiepNhanId = chiSoSinhHieuRequestModel.YctnId;
                    chiSoSinhHieu.NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
                    chiSoSinhHieu.NhanVienThucHienId = _userAgentHelper.GetCurrentUserId();
                    modifyIds.Add(chiSoSinhHieu.Id);
                }
            }

            var ketQuaShIdEntities = yctnEntity.KetQuaSinhHieus.Select(e => e.Id);
            var deleteKetQuaSh = ketQuaShIdEntities.Where(q => !modifyIds.Contains(q) && q != 0);
            foreach (var idModifyItem in deleteKetQuaSh)
            {
                var chiSoSinhHieuDelete = yctnEntity.KetQuaSinhHieus.FirstOrDefault(e => e.Id == idModifyItem);
                if (chiSoSinhHieuDelete != null) chiSoSinhHieuDelete.WillDelete = true;
            }

            await _yctnService.UpdateAsync(yctnEntity);

            return Ok();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("LoadChiSoSinhHieu")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult> LoadChiSoSinhHieu(long yctnId)
        {
            var sinhHieu = await _yeuCauDichVuKyThuatTuongTrinhPtttService.LoadChiSoSinhHieu(yctnId);
            return Ok(sinhHieu);
        }
        #endregion
    }
}
