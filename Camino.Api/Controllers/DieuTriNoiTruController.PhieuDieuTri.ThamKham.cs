using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.DieuTriNoiTru;
using Camino.Api.Models.Error;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Newtonsoft.Json;
using static Camino.Core.Domain.Enums;
using Camino.Services.Helpers;

namespace Camino.Api.Controllers
{
    public partial class DieuTriNoiTruController
    {
        [HttpPost("GetNgayDieuTri")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> GetNgayDieuTri(long yeuCauTiepNhanId)
        {
            //var entity = await _dieuTriNoiTruService.GetByIdAsync(yeuCauTiepNhanId, s =>
            //        s.Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruPhieuDieuTris)
            //        );

            var lstDate = await _dieuTriNoiTruService.GetNgayDieuTri(yeuCauTiepNhanId);

            return Ok(lstDate);
        }

        [HttpPost("GetThongTinPhieuKham")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> GetThongTinPhieuKham([FromBody] ParamsThongTinPhieuKhamModel model)
        {
            var yctn = await _dieuTriNoiTruService.GetByIdAsync(model.yeuCauTiepNhanId, s =>
                s.Include(p => p.NoiTruBenhAn).ThenInclude(p => p.NoiTruPhieuDieuTris).ThenInclude(p => p.NoiTruThamKhamChanDoanKemTheos).ThenInclude(p => p.ICD)
                 .Include(p => p.NoiTruBenhAn).ThenInclude(p => p.NoiTruPhieuDieuTris).ThenInclude(p => p.KetQuaSinhHieus)
                  .Include(p => p.NoiTruBenhAn).ThenInclude(p => p.NoiTruPhieuDieuTris).ThenInclude(p => p.NoiTruThoiGianDieuTriBenhAnSoSinhs)
                 .Include(p => p.NoiTruBenhAn).ThenInclude(p => p.NoiTruPhieuDieuTris).ThenInclude(p => p.KhoaPhongDieuTri)
                 .Include(p => p.NoiTruBenhAn).ThenInclude(p => p.NoiTruEkipDieuTris).ThenInclude(p => p.BacSi).ThenInclude(p => p.User)
                 .Include(p => p.NoiTruBenhAn).ThenInclude(p => p.NoiTruEkipDieuTris).ThenInclude(p => p.DieuDuong).ThenInclude(p => p.User)
                 .Include(p => p.NoiTruBenhAn).ThenInclude(p => p.NoiTruKhoaPhongDieuTris).ThenInclude(p => p.ChanDoanVaoKhoaICD)
                 .Include(p => p.NoiTruBenhAn).ThenInclude(p => p.NoiTruPhieuDieuTris).ThenInclude(p => p.ChanDoanChinhICD)
                 .Include(p => p.NoiTruBenhAn).ThenInclude(p => p.NoiTruPhieuDieuTris).ThenInclude(p => p.CheDoAn)
                 .Include(p => p.NoiTruBenhAn).ThenInclude(p => p.YeuCauTiepNhan).ThenInclude(p => p.YeuCauNhapVien).ThenInclude(p => p.ChanDoanNhapVienICD)

                .Include(p => p.YeuCauDichVuGiuongBenhViens).ThenInclude(p => p.GiuongBenh).ThenInclude(p => p.PhongBenhVien)
                );

            var result = MapEntityToPhieuThamKhamViewModel(yctn, model.phieuDieuTriId);

            return Ok(result);
        }

        private PhieuKhamThamKhamViewModel MapEntityToPhieuThamKhamViewModel(YeuCauTiepNhan entity, long phieuDieuTriId)
        {
            var phieuDieuTri = entity.NoiTruBenhAn.NoiTruPhieuDieuTris.First(p => p.Id == phieuDieuTriId);

            var ngayDieuTri = phieuDieuTri.NgayDieuTri;
            var result = phieuDieuTri.ToModel<PhieuKhamThamKhamViewModel>();
            foreach (var item in result.NoiTruThamKhamChanDoanKemTheos)
            {
                item.NoiTruPhieuDieuId = phieuDieuTriId;
            }
            //result = new PhieuKhamThamKhamViewModel
            //{
            result.KhoaChiDinh = phieuDieuTri.KhoaPhongDieuTri.Ten;
            result.KhoaChiDinhId = phieuDieuTri.KhoaPhongDieuTriId;
            result.Phong = entity.YeuCauDichVuGiuongBenhViens.Any(p => p.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan)
            ? entity.YeuCauDichVuGiuongBenhViens.Last(p => p.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan).GiuongBenh.PhongBenhVien.Ten
            : "";
            result.Giuong = entity.YeuCauDichVuGiuongBenhViens.Any(p => p.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan)
                ? entity.YeuCauDichVuGiuongBenhViens.Last(p => p.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan).GiuongBenh.Ten
                : "";

            result.BSDieuTri = entity.NoiTruBenhAn.NoiTruEkipDieuTris.Any(s => s.TuNgay.Date <= ngayDieuTri && (s.DenNgay == null || ngayDieuTri <= s.DenNgay.Value.Date))
                                                                                ? entity.NoiTruBenhAn.NoiTruEkipDieuTris
                                                                                .Where(s => s.TuNgay.Date <= ngayDieuTri && (s.DenNgay == null || ngayDieuTri <= s.DenNgay.Value.Date))
                                                                                .Select(s => s.BacSi.User.HoTen).Distinct().Join(", ") : "";

            result.DieuDuong = entity.NoiTruBenhAn.NoiTruEkipDieuTris.Any(s => s.TuNgay.Date <= ngayDieuTri && (s.DenNgay == null || ngayDieuTri <= s.DenNgay.Value.Date))
                                                                                ? entity.NoiTruBenhAn.NoiTruEkipDieuTris
                                                                                .Where(s => s.TuNgay.Date <= ngayDieuTri && (s.DenNgay == null || ngayDieuTri <= s.DenNgay.Value.Date))
                                                                                .Select(s => s.DieuDuong.User.HoTen).Distinct().Join(", ") : "";
            result.NgayYLenh = phieuDieuTri.NgayDieuTri.ApplyFormatDate();
            result.ChuanDoanNhapVien = entity.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.Any(p => p.ChanDoanVaoKhoaICDId != null)
                ? entity.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.Where(p => p.ChanDoanVaoKhoaICDId != null)
                    //.Select(p => p.ChanDoanVaoKhoaICD.Ma + " - " + p.ChanDoanVaoKhoaICD.TenTiengViet)
                    .Select(p => p.ChanDoanVaoKhoaGhiChu)
                    .Join(", ")
                : "";
            result.ThoiDiemNhapVien = entity.NoiTruBenhAn.YeuCauTiepNhan?.YeuCauNhapVien?.ThoiDiemChiDinh;
            result.LaCapCuu = entity.NoiTruBenhAn.LaCapCuu;
            result.DienBien = phieuDieuTri.DienBien;
            result.ChanDoanChinhICDId = phieuDieuTri.ChanDoanChinhICDId;
            result.ChanDoanChinhICDModelText = phieuDieuTri.ChanDoanChinhICD != null ? phieuDieuTri.ChanDoanChinhICD.Ma + " - " + phieuDieuTri.ChanDoanChinhICD.TenTiengViet : "";
            result.ChanDoanChinhGhiChu = phieuDieuTri.ChanDoanChinhGhiChu;
            result.NgayDieuTri = phieuDieuTri.NgayDieuTri.Date;
            result.CheDoAnId = phieuDieuTri.CheDoAnId;
            result.TenCheDoAn = phieuDieuTri.CheDoAn?.Ten;
            if (!string.IsNullOrEmpty(result.DienBien))
            {
                var dienBiens =
                        JsonConvert.DeserializeObject<List<PhieuThamKhamDienBienViewModel>>(result.DienBien);

                foreach (var item in dienBiens)
                {
                    item.IsUpdate = false;
                }
                result.DienBiens.AddRange(dienBiens);
            }
            if (entity.NoiTruBenhAn.LoaiBenhAn == LoaiBenhAn.TreSoSinh)
            {
                var noiTruThoiGianDieuTriBenhAnSoSinhs = phieuDieuTri.NoiTruThoiGianDieuTriBenhAnSoSinhs.ToList();
                result.SoNgayDieuTriBenhAnSoSinh = noiTruThoiGianDieuTriBenhAnSoSinhs.Any() ?
                    NoiTruBenhAnHelper.SoNgayDieuTriBenhAnSoSinh(noiTruThoiGianDieuTriBenhAnSoSinhs) : 0;
            }

            return result;
        }

        [HttpPost("GetICD")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetICD([FromBody] DropDownListRequestModel model, bool coHienThiMa = false)
        {
            var lookup = await _dieuTriNoiTruService.GetICD(model, coHienThiMa);
            return Ok(lookup);
        }

        [HttpPost("GetCheDoChamSoc")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetCheDoChamSoc([FromBody] DropDownListRequestModel model)
        {
            var lookup = await _dieuTriNoiTruService.GetCheDoChamSoc(model);
            return Ok(lookup);
        }

        private void KiemTraThoiGianDieuTriSoSinh(ThoiGianDieuTriSoSinhViewModel item, List<ThoiGianDieuTriSoSinhViewModel> thoiGianDieuTriSoSinhViewModels)
        {
            if (thoiGianDieuTriSoSinhViewModels.Any())
            {
                if (item.GioBatDau >= item.GioKetThuc)
                {
                    throw new ApiException(_localizationService.GetResource("DieuTriNoiTru.GioBatDau.MoreThanGoiKetThuc"));
                }
                foreach (var itemThoiGianDieuTriSoSinhViewModels in thoiGianDieuTriSoSinhViewModels)
                {
                    if (item.GioBatDau != itemThoiGianDieuTriSoSinhViewModels.GioBatDau.Value)
                    {
                        if (itemThoiGianDieuTriSoSinhViewModels.GioBatDau.Value < item.GioBatDau &&
                            itemThoiGianDieuTriSoSinhViewModels.GioKetThuc.Value > item.GioBatDau
                            )
                        {
                            throw new ApiException(_localizationService.GetResource("DieuTriNoiTru.GioBatDau.IsValid"));
                        }
                    }
                    if (item.GioKetThuc != itemThoiGianDieuTriSoSinhViewModels.GioKetThuc.Value)
                    {
                        if (itemThoiGianDieuTriSoSinhViewModels.GioBatDau.Value < item.GioKetThuc &&
                            itemThoiGianDieuTriSoSinhViewModels.GioKetThuc.Value > item.GioKetThuc)
                        {
                            throw new ApiException(_localizationService.GetResource("DieuTriNoiTru.GioBatDau.IsValid"));
                        }
                    }
                }
            }
        }

        [HttpPost("UpdatePhieuDieuTriThamKham")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> UpdatePhieuDieuTriThamKham([FromBody] PhieuKhamThamKhamViewModel viewModel)
        {
            //_logger.LogTrace("UpdatePhieuDieuTriThamKham", viewModel);
            //var entity = await _dieuTriNoiTruService.GetYeuCauTiepNhanWithIncludeUpdate(viewModel.Id);

            var noiTruBenhAn = await _dieuTriNoiTruService.GetNoiTruBenhAnById(viewModel.Id, x => x.Include(o=>o.NoiTruPhieuDieuTris).ThenInclude(o => o.NoiTruThamKhamChanDoanKemTheos)
                                                                                                            .Include(o => o.NoiTruPhieuDieuTris).ThenInclude(o => o.KetQuaSinhHieus));

            if (noiTruBenhAn != null && noiTruBenhAn.ThoiDiemRaVien != null)
            {
                throw new ApiException(_localizationService.GetResource("ApiError.ConcurrencyError"));
            }

            //thời gian diều trị sơ sinh ko trung nhau
            //_logger.LogTrace("UpdatePhieuDieuTriThamKham", "KiemTraThoiGianDieuTriSoSinh");
            if (viewModel.ThoiGianDieuTriSoSinhViewModels.Any())
            {
                foreach (var itemThoiGianDieuTriSoSinhViewModel in viewModel.ThoiGianDieuTriSoSinhViewModels)
                {
                    KiemTraThoiGianDieuTriSoSinh(itemThoiGianDieuTriSoSinhViewModel, viewModel.ThoiGianDieuTriSoSinhViewModels);
                }
            }
            var phieuDieuTriUpdate = noiTruBenhAn.NoiTruPhieuDieuTris.First(p => p.Id == viewModel.PhieuDieuTriId);
            var dienBienCus = new List<PhieuThamKhamDienBienViewModel>();
            if (!string.IsNullOrEmpty(phieuDieuTriUpdate.DienBien))
            {
                dienBienCus = JsonConvert.DeserializeObject<List<PhieuThamKhamDienBienViewModel>>(phieuDieuTriUpdate.DienBien);
            }

            //_logger.LogTrace("UpdatePhieuDieuTriThamKham", "DienBiens");
            if (viewModel.DienBiens.Any())
            {
                var idView = 1;
                foreach (var item in viewModel.DienBiens)
                {
                    item.IdView = idView;
                    idView++;
                    item.CheDoAn = await _dieuTriNoiTruService.GetCheDoAn(item.CheDoAnId);
                    item.CheDoChamSoc = await _dieuTriNoiTruService.GetCheDoChamSoc(item.CheDoChamSocId);

                    //BVHD-3959: Tên BS hiển thị trên phiếu điều trị được lấy theo tên tài khoản chỉnh sửa cuối cùng tại MH thăm khám
                    var dienBienCu = dienBienCus.Where(o => o.ThoiGian == item.ThoiGian).FirstOrDefault();
                    if(dienBienCu != null)
                    {
                        if(dienBienCu.DienBienLastUserId != null && item.DienBien == dienBienCu.DienBien)
                        {
                            item.DienBienLastUserId = dienBienCu.DienBienLastUserId;
                        }
                        else
                        {
                            item.DienBienLastUserId = _userAgentHelper.GetCurrentUserId();
                        }
                        if (dienBienCu.YLenhLastUserId != null && item.YLenh == dienBienCu.YLenh && item.CheDoAn == dienBienCu.CheDoAn && item.CheDoChamSoc == dienBienCu.CheDoChamSoc)
                        {
                            item.YLenhLastUserId = dienBienCu.YLenhLastUserId;
                        }
                        else
                        {
                            item.YLenhLastUserId = _userAgentHelper.GetCurrentUserId();
                        }
                    }
                    else
                    {
                        item.DienBienLastUserId = _userAgentHelper.GetCurrentUserId();
                        item.YLenhLastUserId = _userAgentHelper.GetCurrentUserId();
                    }
                }
                viewModel.DienBien = JsonConvert.SerializeObject(viewModel.DienBiens);
            }

            //if (entity == null)
            //{
            //    return NotFound();
            //}
            //_logger.LogTrace("UpdatePhieuDieuTriThamKham", "NoiTruPhieuDieuTris");
            var noiTruPhieuDieuTris = noiTruBenhAn.NoiTruPhieuDieuTris.Where(p => p.NgayDieuTri.Date >= viewModel.NgayDieuTri.Date).ToList();
            foreach (var item in noiTruPhieuDieuTris)
            {
                item.ChanDoanChinhICDId = viewModel.ChanDoanChinhICDId;
                item.ChanDoanChinhGhiChu = viewModel.ChanDoanChinhGhiChu;
            }

            //if (entity.IsDefault && (entity.Ten != viewModel.Ten || entity.LoaiKho != viewModel.LoaiKho || entity.KhoaPhongId != viewModel.KhoaPhongId || entity.PhongBenhVienId != viewModel.PhongBenhVienId))
            //{
            //    throw new ApiException(_localizationService.GetResource("Kho.KhoDuocPhamVTYT.Update.NhanVien.Only"), (int)HttpStatusCode.BadRequest);
            //}

            //_logger.LogTrace("UpdatePhieuDieuTriThamKham", "ToEntity");
            viewModel.ToEntity(phieuDieuTriUpdate);

            foreach (var chuanDoanKemTheo in phieuDieuTriUpdate.KetQuaSinhHieus)
            {
                chuanDoanKemTheo.NhanVienThucHienId = _userAgentHelper.GetCurrentUserId();
                chuanDoanKemTheo.YeuCauTiepNhanId = viewModel.Id;
                //chuanDoanKemTheo.NoiTruPhieuDieuTriId = phieuDieuTriUpdate.Id;
            }
            //_logger.LogTrace("UpdatePhieuDieuTriThamKham", "PrepareForEditYeuCauTiepNhanAndUpdateAsync");
            //await _tiepNhanBenhNhanService.PrepareForEditYeuCauTiepNhanAndUpdateAsync(entity);
            _dieuTriNoiTruService.SaveChanges();

            //var lstDate = await _dieuTriNoiTruService.GetNgayDieuTri(viewModel.Id);
            //_logger.LogTrace("UpdatePhieuDieuTriThamKham", "ToModel");
            var result = phieuDieuTriUpdate.ToModel<PhieuKhamThamKhamViewModel>();
            foreach (var item in result.NoiTruThamKhamChanDoanKemTheos)
            {
                var tenICD = await _yeuCauKhamBenhService.GetTenICD(item.ICDId.Value);
                item.NoiTruPhieuDieuId = viewModel.PhieuDieuTriId;
                item.TenICD = tenICD;
            }
            //_logger.LogTrace("UpdatePhieuDieuTriThamKham", "ToModelDienBien");
            if (!string.IsNullOrEmpty(result.DienBien))
            {
                var dienBiens =
                        JsonConvert.DeserializeObject<List<PhieuThamKhamDienBienViewModel>>(result.DienBien);
                result.DienBiens.AddRange(dienBiens);
            }
            //_logger.LogTrace("UpdatePhieuDieuTriThamKham", "return");
            return Ok(result);
        }

        // Tạo  1 danh sách tạm
        [HttpPost("CreateNewDateTemp")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> CreateNewDateTemp([FromBody] CreateNewDateModel model)
        {
            //check exist date
            //if (await _dieuTriNoiTruService.IsExistsDateTamThoi(model.Dates, model.DateAdds))
            //{
            //    throw new ApiException(_localizationService.GetResource("DieuTriNoiTru.NewDate.Exists"));
            //}
            ////
            await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(model.yeuCauTiepNhanId);
            var lstDate = await _dieuTriNoiTruService.GetNgayDieuTriTamThoi(model);
            return Ok(lstDate);
        }

        [HttpPost("CreateNewDate")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> CreateNewDate([FromBody] CreateNewDateModel model)
        {
            //check exist date
            if (await _dieuTriNoiTruService.IsExistsDate(model.yeuCauTiepNhanId, model.Dates, model.KhoaId))
            {
                throw new ApiException(_localizationService.GetResource("DieuTriNoiTru.NewDate.Exists"));
            }
            //
            await _dieuTriNoiTruService.CreateNewDate(model.yeuCauTiepNhanId, model.KhoaId, model.Dates.OrderBy(x => x.Date).ToList());
            //await _tiepNhanBenhNhanService.PrepareForEditYeuCauTiepNhanAndUpdateAsync(entity);

            var lstDate = await _dieuTriNoiTruService.GetNgayDieuTri(model.yeuCauTiepNhanId);

            return Ok(lstDate);
        }

        [HttpPost("RemoveDateTemp")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> RemoveDateTemp([FromBody] CreateNewDateModel model)
        {
            ////check
            //if (!await _dieuTriNoiTruService.IsValidateRemoveDate(model.yeuCauTiepNhanId, model.phieuDieuTriId))
            //{
            //    throw new ApiException(_localizationService.GetResource("DieuTriNoiTru.RemoveDate.IsValid"));
            //}
            var lstDate = await _dieuTriNoiTruService.GetNgayDieuTriTamThoi(model);
            return Ok(lstDate);
        }

        [HttpPost("RemoveDate")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> RemoveDate([FromBody] CreateNewDateModel model)
        {
            //check
            if (!await _dieuTriNoiTruService.IsValidateRemoveDate(model.yeuCauTiepNhanId, model.phieuDieuTriId))
            {
                throw new ApiException(_localizationService.GetResource("DieuTriNoiTru.RemoveDate.IsValid"));
            }
            //
            var entity = await _dieuTriNoiTruService.GetYeuCauTiepNhanWithIncludeUpdate(model.yeuCauTiepNhanId);

            var phieuDieuTri = entity.NoiTruBenhAn.NoiTruPhieuDieuTris.First(p => p.Id == model.phieuDieuTriId);

            phieuDieuTri.WillDelete = true;

            //foreach (var item in phieuDieuTri.NoiTruPhieuDieuTriChiTietDienBiens)
            //{
            //    item.WillDelete = true;
            //}

            foreach (var item in phieuDieuTri.NoiTruThamKhamChanDoanKemTheos)
            {
                item.WillDelete = true;
            }

            //foreach (var item in phieuDieuTri.NoiTruPhieuDieuTriChiTietYLenhs)
            //{
            //    item.WillDelete = true;
            //}

            //
            foreach (var item in phieuDieuTri.YeuCauDichVuKyThuats)
            {
                item.WillDelete = true;

                foreach (var itemChild in item.PhongBenhVienHangDois)
                {
                    itemChild.WillDelete = true;
                }
            }

            foreach (var item in phieuDieuTri.YeuCauTruyenMaus)
            {
                item.WillDelete = true;
            }

            foreach (var item in phieuDieuTri.YeuCauVatTuBenhViens)
            {
                item.WillDelete = true;
            }

            foreach (var item in phieuDieuTri.YeuCauDuocPhamBenhViens)
            {
                item.WillDelete = true;

                if (item.NoiTruChiDinhDuocPham != null)
                {
                    item.NoiTruChiDinhDuocPham.WillDelete = true;
                }
            }
            //

            await _tiepNhanBenhNhanService.PrepareForEditYeuCauTiepNhanAndUpdateAsync(entity);

            var lstDate = await _dieuTriNoiTruService.GetNgayDieuTri(model.yeuCauTiepNhanId);

            return Ok(lstDate);
        }

        #region in phieu

        private async Task<string> getContent(YeuCauTiepNhan entity, NoiTruPhieuDieuTri phieuDieuTri, long yeuCauTiepNhanId)
        {

            var content = "";
            var html = _danhSachChoKhamService.GetBodyByName("ToDieuTri");
            var soThuTu = entity.NoiTruBenhAn.NoiTruPhieuDieuTris.OrderBy(o => o.NgayDieuTri).IndexOf(phieuDieuTri) + 1;
            var cdkt = phieuDieuTri.NoiTruThamKhamChanDoanKemTheos.Select(o => o.GhiChu).ToArray();
            var data = new PhieuDieuTriViewModel
            {
                So = soThuTu + "",
                SoVaoVien = entity.NoiTruBenhAn?.SoBenhAn ?? "",
                HoTen = entity.HoTen,
                Tuoi = DateTime.Now.Year - entity.NamSinh + "",
                GioiTinh = entity.GioiTinh == LoaiGioiTinh.GioiTinhNam ? "Nam" : "Nữ",

                Khoa = entity.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.Any()
                    ? entity.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.Last().KhoaPhongChuyenDen.Ten.Replace("Khoa", string.Empty) : "",

                Buong = entity.YeuCauDichVuGiuongBenhViens.Any(p => p.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan)
                    ? entity.YeuCauDichVuGiuongBenhViens.Last(p => p.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan).GiuongBenh?.PhongBenhVien?.Ten
                        : "",
                Giuong = entity.YeuCauDichVuGiuongBenhViens.Any(p => p.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan)
                    ? entity.YeuCauDichVuGiuongBenhViens.Last(p => p.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan).GiuongBenh.Ten
                        : "",
                ChanDoan = "<b>"+ phieuDieuTri.ChanDoanChinhGhiChu + (cdkt.Length > 0 ? "; " + string.Join("; ", cdkt) : "")+"</b>"
            };

            var lstDVKT = await _phauThuatThuThuatService.GetDichVuKyThuatsByYeuCauTiepNhan(yeuCauTiepNhanId, phieuDieuTri.Id);
            var lstThuoc = await _dieuTriNoiTruService.GetDanhSachThuoc(yeuCauTiepNhanId, phieuDieuTri.Id);
            //var lstTruyenDich = await _dieuTriNoiTruService.GetDanhSachTruyenDich(yeuCauTiepNhanId, phieuDieuTri.Id);

            if (!string.IsNullOrEmpty(phieuDieuTri.DienBien))
            {
                var dienBiens = JsonConvert.DeserializeObject<List<NoiTruPhieuDieuTriDienBien>>(phieuDieuTri.DienBien);
                var i = 0;
                dienBiens.OrderBy(o => o.ThoiGian).ToList().ForEach(dienBien =>
                {

                    var bacSiThoiGianDienBienNull = entity.NoiTruBenhAn.NoiTruEkipDieuTris.Any(s => s.TuNgay.Date <= phieuDieuTri.NgayDieuTri && (s.DenNgay == null || phieuDieuTri.NgayDieuTri <= s.DenNgay.Value.Date))
                        ? entity.NoiTruBenhAn.NoiTruEkipDieuTris
                            .Where(s => s.TuNgay.Date <= phieuDieuTri.NgayDieuTri && (s.DenNgay == null || phieuDieuTri.NgayDieuTri <= s.DenNgay.Value.Date))
                            .Select(s => (s.BacSi.HocHamHocViId != null ? s.BacSi.HocHamHocVi.Ma + ". ":"") + s.BacSi.User.HoTen).Distinct().Join(", ") : "";


                    var bacSiThoiGianDienBienKhacNull = entity.NoiTruBenhAn.NoiTruEkipDieuTris.Any(s =>
                                                                              (s.TuNgay <= dienBien.ThoiGian &&
                                                                              (s.DenNgay == null || dienBien.ThoiGian <= s.DenNgay.Value)))
                       ? entity.NoiTruBenhAn.NoiTruEkipDieuTris
                           .Where(s =>
                                       (s.TuNgay <= dienBien.ThoiGian && (s.DenNgay == null || dienBien.ThoiGian <= s.DenNgay.Value)))
                           .Select(s => (s.BacSi.HocHamHocViId != null ? s.BacSi.HocHamHocVi.Ma + ". " : "") + s.BacSi.User.HoTen).Distinct().Join(", ") : "";


                    var ngayGio = string.Empty;
                    var dienBienBenh = string.Empty;
                    var yLenh = string.Empty;

                    if (i == 0 && (lstThuoc.Any(o => o.ThoiDiemChiDinh != null && o.ThoiDiemChiDinh < dienBien.ThoiGian) ||
                        //lstTruyenDich.Any(o => o.ThoiDiemChiDinh != null && o.ThoiDiemChiDinh < dienBien.ThoiGian) ||
                        lstDVKT.Any(o => o.ThoiDiemChiDinh != null && o.ThoiDiemChiDinh < dienBien.ThoiGian)))
                    {
                        ngayGio += "Trước " + dienBien.ThoiGian.Hour + " giờ " + dienBien.ThoiGian.Minute + " phút, " + dienBien.ThoiGian.ApplyFormatDate();
                        ////DIỄN BIẾN
                        //dienBienBenh += "<p><b>DIỄN BIẾN</b>: </p><br/>";
                        ////DVKT
                        //dienBienBenh += "<p> <b>DVKT</b>: </p>";
                        if (lstDVKT.Any(o => o.ThoiDiemChiDinh != null && o.ThoiDiemChiDinh < dienBien.ThoiGian))
                        {
                            foreach (var item in lstDVKT.Where(o => o.ThoiDiemChiDinh != null && o.ThoiDiemChiDinh < dienBien.ThoiGian))
                            {
                                dienBienBenh += "<div style='padding:1px;'> - " + item.TenDichVu + (item.SoLuong > 1 ? "<b> x " + item.SoLuong + " lần</b>" : "") + " </div>";
                            }
                        }
                        ////Y lệnh
                        //yLenh += "<p><b>Y LỆNH</b>: </p> <br>";
                        ////THUỐC
                        //yLenh += "<p><b>THUỐC</b>:" + " </p>";
                        if (lstThuoc.Any(o => o.ThoiDiemChiDinh != null && o.ThoiDiemChiDinh < dienBien.ThoiGian))
                        {
                            foreach (var item in lstThuoc.Where(o => o.ThoiDiemChiDinh != null && o.ThoiDiemChiDinh < dienBien.ThoiGian))
                            {
                                //DỊCH TRUYỀN
                                if (item.LaDichTruyen == true)
                                {
                                    yLenh += "<div><b>" +
                                             ((item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocPhongxa || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.GayNghien
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.HuongThan
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.KhangSinh
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocDieuTriLao
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocCorticoid)
                                                 ? "(" + _dieuTriNoiTruService.GetSoThuTuThuocGayNghien(
                                                       yeuCauTiepNhanId, phieuDieuTri.Id, item.DuocPhamBenhVienId) +
                                                   ") "
                                                 : "") + (_yeuCauKhamBenhService.FormatTenDuocPham(item.Ten, item.HoatChat, item.HamLuong, item.DuoCPhamBenhVienPhanNhomId)) + " x " +
                                             (_yeuCauKhamBenhService.FormatSoLuong(item.SoLuong, item.LoaiThuocTheoQuanLy)) + " " + item.DVT + "</b></div>";
                                    yLenh += !string.IsNullOrEmpty(item.TocDoTruyen?.ToString()) ||
                                             !string.IsNullOrEmpty(item.DonViTocDoTruyenDisplay) ||
                                             !string.IsNullOrEmpty(item.GhiChu) ?
                                             "<div style='margin-left:15px'>" + item.TocDoTruyen + " " +
                                             item.DonViTocDoTruyenDisplay + " " + item.GhiChu + "</div>" : "";

                                    var thoiGianBatDauTruyen = item.ThoiGianBatDauTruyen;
                                    if (thoiGianBatDauTruyen != null)
                                    {
                                        if (item.SoLanDungTrongNgay != null && item.CachGioTruyenDich != null)
                                        {
                                            for (int j = 0; j < item.SoLanDungTrongNgay; j++)
                                            {
                                                var time = thoiGianBatDauTruyen.Value.ConvertIntSecondsToTime12h();
                                                thoiGianBatDauTruyen += item.SoLanDungTrongNgay * 3600;
                                                item.GioSuDung += time + "; ";
                                            }
                                        }
                                        else
                                        {
                                            item.GioSuDung = thoiGianBatDauTruyen.Value.ConvertIntSecondsToTime12h();
                                        }
                                    }
                                    yLenh += !string.IsNullOrEmpty(item.SoLanDungTrongNgay?.ToString()) ||
                                    !string.IsNullOrEmpty(item.CachGioTruyenDich?.ToString()) ||
                                    !string.IsNullOrEmpty(item.GioSuDung?.ToString()) ?

                                    "<p style='margin-left:15px;margin-bottom:0.1cm'>" +
                                    (item.SoLanDungTrongNgay != null ? item.SoLanDungTrongNgay + " lần/ngày," : "") +
                                    " " + (item.CachGioTruyenDich != null
                                        ? "cách " + item.CachGioTruyenDich + " giờ,"
                                        : "") + " " + (!string.IsNullOrEmpty(item.GioSuDung)
                                        ? "giờ sử dụng: " + item.GioSuDung
                                        : "") + "</p>"

                                    : "";
                                }
                                else
                                {
                                    yLenh += "<div><b>" + ((item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocPhongxa || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.GayNghien
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.HuongThan
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.KhangSinh
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocDieuTriLao
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocCorticoid) ? "(" + _dieuTriNoiTruService.GetSoThuTuThuocGayNghien(yeuCauTiepNhanId, phieuDieuTri.Id, item.DuocPhamBenhVienId) + ") " : "") + (_yeuCauKhamBenhService.FormatTenDuocPham(item.Ten, item.HoatChat, item.HamLuong, item.DuoCPhamBenhVienPhanNhomId))
                                              + " x " +
                                             (_yeuCauKhamBenhService.FormatSoLuong(item.SoLuong, item.LoaiThuocTheoQuanLy)) + " " + item.DVT + "</b></div>";
                                     yLenh += !string.IsNullOrEmpty(item.GhiChu) ?"<p style='margin-left:15px'>" + item.GhiChu + "</p>" :"";;
                                    yLenh += "<div style='margin-left:15px;margin-bottom:0.1cm'>" + (!string.IsNullOrEmpty(item.DungSang) ? ("Sáng " + item.DungSang + " " + item.DVT) : "") +
                                             (!string.IsNullOrEmpty(item.ThoiGianDungSangDisplay)
                                                 ? (" " + item.ThoiGianDungSangDisplay)
                                                 : "") + (!string.IsNullOrEmpty(item.DungSang) && (!string.IsNullOrEmpty(item.DungTrua) ||
                                                                                                  !string.IsNullOrEmpty(item.DungChieu) ||
                                                                                                  !string.IsNullOrEmpty(item.DungToi)) ? ", " : "") +
                                             (!string.IsNullOrEmpty(item.DungTrua) ? ("Trưa " + item.DungTrua + " " + item.DVT) : "") +
                                             (!string.IsNullOrEmpty(item.ThoiGianDungTruaDisplay)
                                                 ? (" " + item.ThoiGianDungTruaDisplay)
                                                 : "") + (!string.IsNullOrEmpty(item.DungTrua) && (!string.IsNullOrEmpty(item.DungChieu) ||
                                                                                                  !string.IsNullOrEmpty(item.DungToi)) ? ", " : "") +
                                             (!string.IsNullOrEmpty(item.DungChieu) ? ("Chiều " + item.DungChieu + " " + item.DVT) : "") +
                                             (!string.IsNullOrEmpty(item.ThoiGianDungChieuDisplay)
                                                 ? (" " + item.ThoiGianDungChieuDisplay)
                                                 : "") + (!string.IsNullOrEmpty(item.DungChieu) && (!string.IsNullOrEmpty(item.DungToi)) ? ", " : "") +
                                             (!string.IsNullOrEmpty(item.DungToi) ? ("Tối " + item.DungToi + " " + item.DVT) : "") +
                                             (!string.IsNullOrEmpty(item.ThoiGianDungToiDisplay)
                                                 ? (" " + item.ThoiGianDungToiDisplay)
                                                 : "") + "</div>";
                                }
                            }
                        }

                        //CHẾ ĐỘ ĂN
                        yLenh += "<div><b>Chế độ ăn: </b> " + dienBien.CheDoAn + "</div>";
                        yLenh += "<br>";
                        //CHẾ ĐỘ CHĂM SÓC
                        yLenh += "<div><b>Chế độ chăm sóc: </b>" + dienBien.CheDoChamSoc + " </div>";
                        //BÁC SĨ
                        yLenh += "<p style='text-align: center;'> <b>BÁC SĨ</b></p>";
                        yLenh += "<p style='text-align: center;height:30px'></p>";

                        if (dienBien.ThoiGian != null)
                        {
                            yLenh += "<p style='text-align: center;'> " + bacSiThoiGianDienBienKhacNull + "</p>";
                        }
                        else
                        {
                            yLenh += "<p style='text-align: center;'> " + bacSiThoiGianDienBienNull + "</p>";
                        }
                        
                        if(i == 0)
                        {
                            data.ToDieuTri += "<tr style='border-bottom:hidden'> <td class=\"contain-grid\" style=\"vertical-align: top;\"> ";
                        }
                      
                        data.ToDieuTri += "<div>" + ngayGio + "</div> </td> <td class=\"contain-grid\" style=\"vertical-align: top;\">";
                        data.ToDieuTri += "<div>" + dienBienBenh + "</div> </td> <td class=\"contain-grid\"  style=\"vertical-align: top;\">";
                        data.ToDieuTri += "<div>" + yLenh + "</div> </td> </tr>";
                    }
                    ngayGio = string.Empty;
                    dienBienBenh = string.Empty;
                    yLenh = string.Empty;
                    ngayGio += dienBien.ThoiGian.Hour + " giờ " + dienBien.ThoiGian.Minute + " phút, " + dienBien.ThoiGian.ApplyFormatDate();
                    //DIỄN BIẾN
                    dienBienBenh += "<div> " + (!string.IsNullOrEmpty(dienBien?.DienBien) ? dienBien?.DienBien.Replace("\n","<br>"):"") + " </div>";
                    dienBienBenh += !string.IsNullOrEmpty(dienBien?.DienBien) ? "<br>" : "";
                    ////DVKT
                    //dienBienBenh += "<p> <b>DVKT</b>: </p>";
                    if (lstDVKT.Any(o => o.ThoiDiemChiDinh != null && o.ThoiDiemChiDinh >= dienBien.ThoiGian && (i == dienBiens.Count - 1 || o.ThoiDiemChiDinh < dienBiens[i + 1].ThoiGian)))
                    {
                        foreach (var item in lstDVKT.Where(o => o.ThoiDiemChiDinh != null && o.ThoiDiemChiDinh >= dienBien.ThoiGian && (i == dienBiens.Count - 1 || o.ThoiDiemChiDinh < dienBiens[i + 1].ThoiGian)))
                        {
                            dienBienBenh += "<div style='padding;1px'> - " + item.TenDichVu + (item.SoLuong > 1 ? "<b> x " + item.SoLuong + " lần</b>" : "") + " </div>";
                        }
                    }
                    //Y lệnh
                    yLenh += "<div>" + (!string.IsNullOrEmpty(dienBien?.YLenh) ? dienBien?.YLenh.Replace("\n", "<br>") : "") + " </div>";
                    yLenh += !string.IsNullOrEmpty(dienBien?.YLenh) ?"<br>" :"";
                    ////THUỐC
                    //yLenh += "<p><b>THUỐC</b>:" + " </p>";
                    if (lstThuoc.Any(o => o.ThoiDiemChiDinh != null && o.ThoiDiemChiDinh >= dienBien.ThoiGian && (i == dienBiens.Count - 1 || o.ThoiDiemChiDinh < dienBiens[i + 1].ThoiGian)))
                    {
                        foreach (var item in lstThuoc.Where(o => o.ThoiDiemChiDinh != null && o.ThoiDiemChiDinh >= dienBien.ThoiGian && (i == dienBiens.Count - 1 || o.ThoiDiemChiDinh < dienBiens[i + 1].ThoiGian)))
                        {
                            //DỊCH TRUYỀN
                            if (item.LaDichTruyen == true)
                            {
                                yLenh += "<div><b>" +
                                         ((item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocPhongxa || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.GayNghien
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.HuongThan
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.KhangSinh
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocDieuTriLao
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocCorticoid)
                                             ? "(" + _dieuTriNoiTruService.GetSoThuTuThuocGayNghien(yeuCauTiepNhanId,
                                                   phieuDieuTri.Id, item.DuocPhamBenhVienId) + ") "
                                             : "") + (_yeuCauKhamBenhService.FormatTenDuocPham(item.Ten, item.HoatChat, item.HamLuong, item.DuoCPhamBenhVienPhanNhomId)) + " x " +
                                        (_yeuCauKhamBenhService.FormatSoLuong(item.SoLuong, item.LoaiThuocTheoQuanLy)) + " " + item.DVT + "</b></div>" + " " + item.DVT + "</b></div>";
                                yLenh += !string.IsNullOrEmpty(item.TocDoTruyen?.ToString()) ||
                                             !string.IsNullOrEmpty(item.DonViTocDoTruyenDisplay) ||
                                             !string.IsNullOrEmpty(item.GhiChu) ?
                                             "<div style='margin-left:15px'>" + item.TocDoTruyen + " " +
                                             item.DonViTocDoTruyenDisplay + " " + item.GhiChu + "</div>" : "";

                                var thoiGianBatDauTruyen = item.ThoiGianBatDauTruyen;
                                if (thoiGianBatDauTruyen != null)
                                {
                                    if (item.SoLanDungTrongNgay != null && item.CachGioTruyenDich != null)
                                    {
                                        for (int j = 0; j < item.SoLanDungTrongNgay; j++)
                                        {
                                            var time = thoiGianBatDauTruyen.Value.ConvertIntSecondsToTime12h();
                                            thoiGianBatDauTruyen += item.SoLanDungTrongNgay * 3600;
                                            item.GioSuDung += time + "; ";
                                        }
                                    }
                                    else
                                    {
                                        item.GioSuDung = thoiGianBatDauTruyen.Value.ConvertIntSecondsToTime12h();
                                    }
                                }
                                yLenh += !string.IsNullOrEmpty(item.SoLanDungTrongNgay?.ToString()) ||
                                     !string.IsNullOrEmpty(item.CachGioTruyenDich?.ToString()) ||
                                     !string.IsNullOrEmpty(item.GioSuDung?.ToString()) ?

                                     "<div style='margin-left:15px;margin-bottom:0.1cm'>" +
                                     (item.SoLanDungTrongNgay != null ? item.SoLanDungTrongNgay + " lần/ngày," : "") +
                                     " " + (item.CachGioTruyenDich != null
                                         ? "cách " + item.CachGioTruyenDich + " giờ,"
                                         : "") + " " + (!string.IsNullOrEmpty(item.GioSuDung)
                                         ? "giờ sử dụng: " + item.GioSuDung
                                         : "") + "</div>"

                                     : "";
                            }
                            else
                            {
                                yLenh += "<div><b>" + ((item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocPhongxa || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.GayNghien
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.HuongThan
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.KhangSinh
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocDieuTriLao
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocCorticoid) ? "(" + _dieuTriNoiTruService.GetSoThuTuThuocGayNghien(yeuCauTiepNhanId, phieuDieuTri.Id, item.DuocPhamBenhVienId) + ") " : "") +
                                          (_yeuCauKhamBenhService.FormatTenDuocPham(item.Ten, item.HoatChat, item.HamLuong, item.DuoCPhamBenhVienPhanNhomId)) + " x " +
                                         (_yeuCauKhamBenhService.FormatSoLuong(item.SoLuong, item.LoaiThuocTheoQuanLy)) + " " + item.DVT + "</b></div>";
                                 yLenh += !string.IsNullOrEmpty(item.GhiChu) ? "<div style='margin-left:15px'>" + item.GhiChu + "</div>" :"";;
                                yLenh += "<div style='margin-left:15px;margin-bottom:0.1cm'>" + (!string.IsNullOrEmpty(item.DungSang) ? ("Sáng " + item.DungSang + " " + item.DVT) : "") +
                                         (!string.IsNullOrEmpty(item.ThoiGianDungSangDisplay)
                                             ? (" " + item.ThoiGianDungSangDisplay)
                                             : "") + (!string.IsNullOrEmpty(item.DungSang) && (!string.IsNullOrEmpty(item.DungTrua) ||
                                                                                              !string.IsNullOrEmpty(item.DungChieu) ||
                                                                                              !string.IsNullOrEmpty(item.DungToi)) ? ", " : "") +
                                         (!string.IsNullOrEmpty(item.DungTrua) ? ("Trưa " + item.DungTrua + " " + item.DVT) : "") +
                                         (!string.IsNullOrEmpty(item.ThoiGianDungTruaDisplay)
                                             ? (" " + item.ThoiGianDungTruaDisplay)
                                             : "") + (!string.IsNullOrEmpty(item.DungTrua) && (!string.IsNullOrEmpty(item.DungChieu) ||
                                                                                              !string.IsNullOrEmpty(item.DungToi)) ? ", " : "") +
                                         (!string.IsNullOrEmpty(item.DungChieu) ? ("Chiều " + item.DungChieu + " " + item.DVT) : "") +
                                         (!string.IsNullOrEmpty(item.ThoiGianDungChieuDisplay)
                                             ? (" " + item.ThoiGianDungChieuDisplay)
                                             : "") + (!string.IsNullOrEmpty(item.DungChieu) && (!string.IsNullOrEmpty(item.DungToi)) ? ", " : "") +
                                         (!string.IsNullOrEmpty(item.DungToi) ? ("Tối " + item.DungToi + " " + item.DVT) : "") +
                                         (!string.IsNullOrEmpty(item.ThoiGianDungToiDisplay)
                                             ? (" " + item.ThoiGianDungToiDisplay)
                                             : "") + "</div>";
                            }
                        }
                    }
                    //CHẾ ĐỘ ĂN
                    yLenh += "<div><b>Chế độ ăn: </b> " + dienBien.CheDoAn + "</div>";
                    yLenh += "<br>";
                    //CHẾ ĐỘ CHĂM SÓC
                    yLenh += "<div><b>Chế độ chăm sóc: </b>" + dienBien.CheDoChamSoc + " </div>";
                    //BÁC SĨ
                    yLenh += "<p style='text-align: center;'> <b>BÁC SĨ</b></p>";
                    yLenh += "<p style='text-align: center;height:30px'></p>";

                    if (dienBien.ThoiGian != null)
                    {
                        yLenh += "<p style='text-align: center;'> " + bacSiThoiGianDienBienKhacNull + "</p>";
                    }
                    else
                    {
                        yLenh += "<p style='text-align: center;'> " + bacSiThoiGianDienBienNull + "</p>";
                    }
                    if(i != 0 && i != dienBiens.OrderBy(o => o.ThoiGian).ToList().Count())
                    {
                        data.ToDieuTri += "<tr style='border-top:hidden'> <td class=\"contain-grid\" style=\"vertical-align: top;\">";
                    }
                    else if (i == (dienBiens.OrderBy(o => o.ThoiGian).ToList().Count()-1))
                    {
                        data.ToDieuTri += "<tr> <td class=\"contain-grid\" style=\"vertical-align: top;\">";
                    }
                    else
                    {
                        data.ToDieuTri += "<tr style='border-bottom:hidden'> <td class=\"contain-grid\" style=\"vertical-align: top;\">";
                    }
                   
                    data.ToDieuTri += "<div>" + ngayGio + "</div> </td> <td class=\"contain-grid\" style=\"vertical-align: top;\">";
                    data.ToDieuTri += "<div>" + dienBienBenh + "</div> </td> <td class=\"contain-grid\"  style=\"vertical-align: top;\">";
                    data.ToDieuTri += "<div>" + yLenh + "</div> </td> </tr>";
                    i++;
                });
            }
            else
            {

                var ngayGio = string.Empty;
                var dienBienBenh = string.Empty;
                var yLenh = string.Empty;

                if (phieuDieuTri.ThoiDiemThamKham != null)
                {
                    ngayGio += phieuDieuTri.ThoiDiemThamKham.Value.Hour + " giờ " + phieuDieuTri.ThoiDiemThamKham.Value.Minute + " phút, " + phieuDieuTri.ThoiDiemThamKham.Value.ApplyFormatDate();

                }
                ////DIỄN BIẾN
                //dienBienBenh += "<p><b>DIỄN BIẾN</b>: </p><br/>";
                ////DVKT
                //dienBienBenh += "<p> <b>DVKT</b>: </p>";
                foreach (var item in lstDVKT)
                {
                    dienBienBenh += "<div style='padding:1px;'> - " + item.TenDichVu + (item.SoLuong > 1 ? "<b> x " + item.SoLuong + " lần</b>" : "") + " </div>";
                }
                //Y lệnh
                //yLenh += "<p><b>Y LỆNH</b>: </p> <br>";
                ////THUỐC
                //yLenh += "<p><b>THUỐC</b>:" + " </p>";
                if (lstThuoc.Any())
                {
                    foreach (var item in lstThuoc)
                    {
                        //DỊCH TRUYỀN
                        if (item.LaDichTruyen == true)
                        {
                            yLenh += "<div><b>" +
                                     ((item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocPhongxa || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.GayNghien
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.HuongThan
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.KhangSinh
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocDieuTriLao
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocCorticoid)
                                         ? "(" + _dieuTriNoiTruService.GetSoThuTuThuocGayNghien(yeuCauTiepNhanId,
                                               phieuDieuTri.Id, item.DuocPhamBenhVienId) + ") "
                                         : "") + (_yeuCauKhamBenhService.FormatTenDuocPham(item.Ten, item.HoatChat, item.HamLuong, item.DuoCPhamBenhVienPhanNhomId)) + " x " +
                                     (_yeuCauKhamBenhService.FormatSoLuong(item.SoLuong, item.LoaiThuocTheoQuanLy)) + " " + item.DVT + "</b></div>";
                            yLenh += !string.IsNullOrEmpty(item.TocDoTruyen?.ToString()) ||
                                             !string.IsNullOrEmpty(item.DonViTocDoTruyenDisplay) ||
                                             !string.IsNullOrEmpty(item.GhiChu) ?
                                             "<div style='margin-left:15px'>" + item.TocDoTruyen + " " +
                                             item.DonViTocDoTruyenDisplay + " " + item.GhiChu + "</div>" : "";

                            var thoiGianBatDauTruyen = item.ThoiGianBatDauTruyen;
                            if (thoiGianBatDauTruyen != null)
                            {
                                if (item.SoLanDungTrongNgay != null && item.CachGioTruyenDich != null)
                                {
                                    for (int j = 0; j < item.SoLanDungTrongNgay; j++)
                                    {
                                        var time = thoiGianBatDauTruyen.Value.ConvertIntSecondsToTime12h();
                                        thoiGianBatDauTruyen += item.SoLanDungTrongNgay * 3600;
                                        item.GioSuDung += time + "; ";
                                    }
                                }
                                else
                                {
                                    item.GioSuDung = thoiGianBatDauTruyen.Value.ConvertIntSecondsToTime12h();
                                }
                            }
                            yLenh += !string.IsNullOrEmpty(item.SoLanDungTrongNgay?.ToString()) ||
                                     !string.IsNullOrEmpty(item.CachGioTruyenDich?.ToString()) ||
                                     !string.IsNullOrEmpty(item.GioSuDung?.ToString()) ?

                                     "<div style='margin-left:15px;margin-bottom:0.1cm'>" +
                                     (item.SoLanDungTrongNgay != null ? item.SoLanDungTrongNgay + " lần/ngày," : "") +
                                     " " + (item.CachGioTruyenDich != null
                                         ? "cách " + item.CachGioTruyenDich + " giờ,"
                                         : "") + " " + (!string.IsNullOrEmpty(item.GioSuDung)
                                         ? "giờ sử dụng: " + item.GioSuDung
                                         : "") + "</div>"

                                     : "";
                        }
                        else
                        {
                            //yLenh += "<p><b>" + (item.LaThuocHuongThanGayNghien == true ? "(" + _dieuTriNoiTruService.GetSoThuTuThuocGayNghien(yeuCauTiepNhanId, phieuDieuTri.Id, item.DuocPhamBenhVienId) + ") " : "") + 
                            //    item.Ten + (!string.IsNullOrEmpty(item.HoatChat) ? " (" + item.HoatChat + ")" : "") + (!string.IsNullOrEmpty(item.HamLuong) ? " " + item.HamLuong : "") + " x " +
                            //         (item.LaThuocHuongThanGayNghien == true
                            //             ? NumberHelper.ChuyenSoRaText(Convert.ToDouble(item.SoLuong), false)
                            //             : item.SoLuong.ToString()) + " " + item.DVT + "</b></p>";

                            yLenh += "<div><b>" + ((item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocPhongxa || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.GayNghien
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.HuongThan
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.KhangSinh
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocDieuTriLao
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocCorticoid) ? "(" + _dieuTriNoiTruService.GetSoThuTuThuocGayNghien(yeuCauTiepNhanId, phieuDieuTri.Id, item.DuocPhamBenhVienId) + ") " : "") +
                                (_yeuCauKhamBenhService.FormatTenDuocPham(item.Ten, item.HoatChat, item.HamLuong, item.DuoCPhamBenhVienPhanNhomId)) + " x " +
                                     (_yeuCauKhamBenhService.FormatSoLuong(item.SoLuong, item.LoaiThuocTheoQuanLy)) + " " + item.DVT + "</b></div>";

                             yLenh += !string.IsNullOrEmpty(item.GhiChu) ? "<div style='margin-left:15px'>" + item.GhiChu + "</div>" : "";;
                            
                            
                            yLenh += "<div style='margin-left:15px;margin-bottom:0.1cm'>" + (!string.IsNullOrEmpty(item.DungSang) ? ("Sáng " + item.DungSang + " " + item.DVT) : "") +
                                     (!string.IsNullOrEmpty(item.ThoiGianDungSangDisplay)
                                         ? (" " + item.ThoiGianDungSangDisplay)
                                         : "") + (!string.IsNullOrEmpty(item.DungSang) && (!string.IsNullOrEmpty(item.DungTrua) ||
                                                                                          !string.IsNullOrEmpty(item.DungChieu) ||
                                                                                          !string.IsNullOrEmpty(item.DungToi)) ? ", " : "") +
                                     (!string.IsNullOrEmpty(item.DungTrua) ? ("Trưa " + item.DungTrua + " " + item.DVT) : "") +
                                     (!string.IsNullOrEmpty(item.ThoiGianDungTruaDisplay)
                                         ? (" " + item.ThoiGianDungTruaDisplay)
                                         : "") + (!string.IsNullOrEmpty(item.DungTrua) && (!string.IsNullOrEmpty(item.DungChieu) ||
                                                                                          !string.IsNullOrEmpty(item.DungToi)) ? ", " : "") +
                                     (!string.IsNullOrEmpty(item.DungChieu) ? ("Chiều " + item.DungChieu + " " + item.DVT) : "") +
                                     (!string.IsNullOrEmpty(item.ThoiGianDungChieuDisplay)
                                         ? (" " + item.ThoiGianDungChieuDisplay)
                                         : "") + (!string.IsNullOrEmpty(item.DungChieu) && (!string.IsNullOrEmpty(item.DungToi)) ? ", " : "") +
                                     (!string.IsNullOrEmpty(item.DungToi) ? ("Tối " + item.DungToi + " " + item.DVT) : "") +
                                     (!string.IsNullOrEmpty(item.ThoiGianDungToiDisplay)
                                         ? (" " + item.ThoiGianDungToiDisplay)
                                         : "") + "</div>";
                        }
                    }
                }
                //CHẾ ĐỘ ĂN
                yLenh += "<div><b>Chế độ ăn: </b> " + phieuDieuTri?.CheDoAn?.Ten + "</div>";
                yLenh += "<br>";
                //CHẾ ĐỘ CHĂM SÓC
                yLenh += "<div><b>Chế độ chăm sóc:</b> " + phieuDieuTri?.CheDoChamSoc?.GetDescription() + "</div>";
                //BÁC SĨ
               
                //BÁC SĨ
                yLenh += "<p style='text-align: center;'> <b>BÁC SĨ</b></p>";
                yLenh += "<p style='text-align: center;height:30px'></p>";

                // ngày thăm khám
                if (string.IsNullOrEmpty(ngayGio))
                {
                    var bacSi = entity.NoiTruBenhAn.NoiTruEkipDieuTris.Any(s => s.TuNgay.Date <= phieuDieuTri.NgayDieuTri && (s.DenNgay == null || phieuDieuTri.NgayDieuTri <= s.DenNgay.Value.Date))
                   ? entity.NoiTruBenhAn.NoiTruEkipDieuTris
                       .Where(s => s.TuNgay.Date <= phieuDieuTri.NgayDieuTri && (s.DenNgay == null || phieuDieuTri.NgayDieuTri <= s.DenNgay.Value.Date))
                       .Select(s => (s.BacSi.HocHamHocViId != null ? s.BacSi.HocHamHocVi.Ma + ". " : "") + s.BacSi.User.HoTen).Distinct().Join(", ") : "";

                    yLenh += "<p style='text-align: center;'> " + bacSi + "</p>";
                }
                else
                {

                    var bacSiTheoNgayGioThamKham = entity.NoiTruBenhAn.NoiTruEkipDieuTris.Any(s => s.TuNgay.Date <= phieuDieuTri.ThoiDiemThamKham && (s.DenNgay == null || phieuDieuTri.ThoiDiemThamKham <= s.DenNgay.Value.Date))
                   ? entity.NoiTruBenhAn.NoiTruEkipDieuTris
                       .Where(s => s.TuNgay.Date <= phieuDieuTri.ThoiDiemThamKham && (s.DenNgay == null || phieuDieuTri.ThoiDiemThamKham <= s.DenNgay.Value.Date))
                       .Select(s => (s.BacSi.HocHamHocViId != null ? s.BacSi.HocHamHocVi.Ma + ". " : "") + s.BacSi.User.HoTen).Distinct().Join(", ") : "";

                    yLenh += "<p style='text-align: center;'> " + bacSiTheoNgayGioThamKham + "</p>";
                }

                data.ToDieuTri += "<tr> <td class=\"contain-grid\" style=\"vertical-align: top;\">";
                data.ToDieuTri += "<div>" + ngayGio + "</div> </td> <td class=\"contain-grid\" style=\"vertical-align: top;\">";
                data.ToDieuTri += "<div>" + dienBienBenh + "</div> </td> <td class=\"contain-grid\"  style=\"vertical-align: top;\">";
                data.ToDieuTri += "<div>" + yLenh + "</div> </td> </tr>";
            }




            content += TemplateHelpper.FormatTemplateWithContentTemplate(html, data);
            return content;
        }

        [HttpPost("InPhieuThamKhamTheoNgay")]
        [ClaimRequirement(SecurityOperation.Process, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> InPhieuThamKhamTheoNgay(InPhieuThamKhamTheoNgayModel inPhieuThamKhamTheoNgayModel)
        {
            //var entity = await _dieuTriNoiTruService.GetByIdAsync(inPhieuThamKhamTheoNgayModel.YeuCauTiepNhanId
            //    , s => s.Include(u => u.NoiTruBenhAn).ThenInclude(p => p.NoiTruPhieuDieuTris).ThenInclude(p => p.ChanDoanChinhICD)
            //        .ThenInclude(p => p.NoiTruPhieuDieuTris).ThenInclude(p => p.NoiTruThamKhamChanDoanKemTheos)
            //    .Include(u => u.BenhNhan).ThenInclude(p => p.NgheNghiep)

            //    .Include(u => u.NgheNghiep)
            //    .Include(u => u.DanToc)
            //    .Include(u => u.QuocTich)
            //    .Include(u => u.PhuongXa)
            //    .Include(u => u.QuanHuyen)
            //    .Include(u => u.TinhThanh)
            //    .Include(u => u.HinhThucDen)

            //    .Include(u => u.YeuCauNhapVien)

            //    .Include(u => u.NguoiLienHePhuongXa)
            //    .Include(u => u.NguoiLienHeQuanHuyen)
            //    .Include(u => u.NguoiLienHeTinhThanh)

            //    .Include(u => u.NoiTruBenhAn).ThenInclude(p => p.NoiTruPhieuDieuTris).ThenInclude(p => p.YeuCauDichVuKyThuats)

            //    .Include(u => u.YeuCauDichVuGiuongBenhViens).ThenInclude(p => p.GiuongBenh).ThenInclude(p => p.PhongBenhVien)

            //    .Include(u => u.YeuCauDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien).ThenInclude(p => p.DichVuKyThuat).ThenInclude(p => p.DichVuKyThuatBenhViens)

            //    .Include(u => u.NoiTruBenhAn).ThenInclude(p => p.ChuyenDenBenhVien)

            //    .Include(u => u.NoiTruBenhAn).ThenInclude(p => p.NoiTruKhoaPhongDieuTris).ThenInclude(p => p.KhoaPhongChuyenDen)
            //    .Include(u => u.NoiTruBenhAn).ThenInclude(p => p.NhanVienTaoBenhAn).ThenInclude(p => p.User)
            //     .Include(u => u.NoiTruBenhAn).ThenInclude(p => p.NoiTruPhieuDieuTris).ThenInclude(p => p.CheDoAn)
            //        .Include(u => u.NoiTruBenhAn).ThenInclude(p => p.NoiTruEkipDieuTris).ThenInclude(p => p.BacSi).ThenInclude(p => p.User)
            //        .Include(u => u.NoiTruBenhAn).ThenInclude(p => p.NoiTruEkipDieuTris).ThenInclude(p => p.BacSi).ThenInclude(p => p.HocHamHocVi)
            //        );

            //var content = "";
            //if (entity != null)
            //{
            //    var phieuDieuTri = entity.NoiTruBenhAn.NoiTruPhieuDieuTris.FirstOrDefault(p => p.Id == inPhieuThamKhamTheoNgayModel.PhieuDieuTriId);
            //    content = await getContent(entity, phieuDieuTri, inPhieuThamKhamTheoNgayModel.YeuCauTiepNhanId);
            //}
            var contents = await _dieuTriNoiTruService.GetContentInPhieuThamKham(inPhieuThamKhamTheoNgayModel.YeuCauTiepNhanId, inPhieuThamKhamTheoNgayModel.PhieuDieuTriId, inPhieuThamKhamTheoNgayModel.DienBienModels.Select(o => o.Id).ToList());

            return Ok(string.Join("<div class=\"pagebreak\"> </div>", contents));
        }

        [HttpPost("InPhieuThamKhamTatCaNgay")]
        [ClaimRequirement(SecurityOperation.Process, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> InPhieuThamKhamTatCaNgay(InPhieuThamKhamTheoNgayModel inPhieuThamKhamTheoNgayModel )
        {
            //In tất cả 
            //inPhieuThamKhamTheoNgayModel.DienBienModels null,
            //inPhieuThamKhamTheoNgayModel.PhieuDieuTriId = 0

            //var entity = await _dieuTriNoiTruService.GetByIdAsync(inPhieuThamKhamTheoNgayModel.YeuCauTiepNhanId
            //    , s => s.Include(u => u.NoiTruBenhAn).ThenInclude(p => p.NoiTruPhieuDieuTris).ThenInclude(p => p.ChanDoanChinhICD)
            //        .ThenInclude(p => p.NoiTruPhieuDieuTris).ThenInclude(p => p.NoiTruThamKhamChanDoanKemTheos)
            //    .Include(u => u.BenhNhan).ThenInclude(p => p.NgheNghiep)

            //    .Include(u => u.NgheNghiep)
            //    .Include(u => u.DanToc)
            //    .Include(u => u.QuocTich)
            //    .Include(u => u.PhuongXa)
            //    .Include(u => u.QuanHuyen)
            //    .Include(u => u.TinhThanh)
            //    .Include(u => u.HinhThucDen)

            //    .Include(u => u.YeuCauNhapVien)

            //    .Include(u => u.NguoiLienHePhuongXa)
            //    .Include(u => u.NguoiLienHeQuanHuyen)
            //    .Include(u => u.NguoiLienHeTinhThanh)

            //    .Include(u => u.NoiTruBenhAn).ThenInclude(p => p.NoiTruPhieuDieuTris).ThenInclude(p => p.YeuCauDichVuKyThuats)

            //    .Include(u => u.YeuCauDichVuGiuongBenhViens).ThenInclude(p => p.GiuongBenh).ThenInclude(p => p.PhongBenhVien)

            //    .Include(u => u.YeuCauDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien).ThenInclude(p => p.DichVuKyThuat).ThenInclude(p => p.DichVuKyThuatBenhViens)

            //    .Include(u => u.NoiTruBenhAn).ThenInclude(p => p.ChuyenDenBenhVien)

            //    .Include(u => u.NoiTruBenhAn).ThenInclude(p => p.NoiTruKhoaPhongDieuTris).ThenInclude(p => p.KhoaPhongChuyenDen)
            //    .Include(u => u.NoiTruBenhAn).ThenInclude(p => p.NhanVienTaoBenhAn).ThenInclude(p => p.User)
            //        .Include(u => u.NoiTruBenhAn).ThenInclude(p => p.NoiTruPhieuDieuTris).ThenInclude(p => p.CheDoAn)
            //        .Include(u => u.NoiTruBenhAn).ThenInclude(p => p.NoiTruEkipDieuTris).ThenInclude(p => p.BacSi).ThenInclude(p => p.User)
            //        .Include(u => u.NoiTruBenhAn).ThenInclude(p => p.NoiTruEkipDieuTris).ThenInclude(p => p.BacSi).ThenInclude(p => p.HocHamHocVi)
            //);

            //var content = "";

            //if (entity != null)
            //{
            //    var lstPhieuDieuTri = entity.NoiTruBenhAn.NoiTruPhieuDieuTris.OrderBy(o => o.NgayDieuTri).ToList();

            //    var countPage = 1;

            //    foreach (var phieuDieuTri in lstPhieuDieuTri)
            //    {
            //        if (countPage != 1)
            //        {
            //            content += "<div class=\"pagebreak\"> </div>";
            //        }

            //        content += await getContent(entity, phieuDieuTri, inPhieuThamKhamTheoNgayModel.YeuCauTiepNhanId);
            //        countPage++;
            //    }
            //}

            //return Ok(content);

            var contents = await _dieuTriNoiTruService.GetContentInPhieuThamKham(inPhieuThamKhamTheoNgayModel.YeuCauTiepNhanId);

            return Ok(string.Join("<div class=\"pagebreak\"> </div>", contents));
        }

        #endregion in phieu

        #region Thời gian sơ sinh điều trị theo phiếu

        [HttpPost("ChangeThoiGianDieuTriSoSinh")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public ActionResult<decimal> ChangeThoiGianDieuTriSoSinh(ThoiGianDieuTriSoSinhRaVienViewModel thoiGianDieuTriSoSinhRaVienViewModel)
        {

            if (thoiGianDieuTriSoSinhRaVienViewModel.ThoiGianDieuTriSoSinhViewModels.Any())
            {
                var thoiGianDTbeSoSinhs = thoiGianDieuTriSoSinhRaVienViewModel.ThoiGianDieuTriSoSinhViewModels
                                                            .Select(c => new Core.Domain.Entities.DieuTriNoiTrus.NoiTruThoiGianDieuTriBenhAnSoSinh
                                                            {
                                                                GioBatDau = c.GioBatDau,
                                                                GioKetThuc = c.GioKetThuc
                                                            }).ToList();
                var soNgayDieuTriBenhAnSoSinh = NoiTruBenhAnHelper.SoNgayDieuTriBenhAnSoSinh(thoiGianDTbeSoSinhs);
                return (soNgayDieuTriBenhAnSoSinh);
            }

            return Ok(0);
        }

        #endregion
    }
}
