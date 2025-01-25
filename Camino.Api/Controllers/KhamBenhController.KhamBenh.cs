using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.BenhNhanDiUngThuocs;
using Camino.Api.Models.BenhNhanTienSuBenhs;
using Camino.Api.Models.Error;
using Camino.Api.Models.KetQuaSinhHieu;
using Camino.Api.Models.KhamBenh;
using Camino.Api.Models.KhamBenh.ViewModelCheckValidators;
using Camino.Api.Models.YeuCauTiepNhan;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.KetQuaSinhHieus;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BenhNhans;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace Camino.Api.Controllers
{
    public partial class KhamBenhController
    {
        [HttpPost("GetListThuoc")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh)]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BenhNhan)]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListThuoc([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _khamBenhService.GetListThuoc(model);
            return Ok(lookup);
        }

        //Nam test
        [HttpPost("getListLoaiDiUng")]
        public ActionResult<ICollection<Enums.LoaiDiUng>> getListLoaiDiUng()
        {
            var listEnum = _khamBenhService.getListLoaiDiUng();
            return Ok(listEnum);
        }

        [HttpPost("GetListNhomMau")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BenhNhan)]
        public ActionResult<ICollection<Enums.EnumNhomMau>> GetListNhomMau([FromBody]LookupQueryInfo model)
        {
            var listEnum = _khamBenhService.GetListNhomMau(model);
            return Ok(listEnum);
        }

        [HttpPost("GetListTrieuChung")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh)]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListTrieuChung([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _khamBenhService.GetListTrieuChung(model);
            return Ok(lookup);
        }

        [HttpPost("GetListChuanDoan")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListChuanDoan([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _khamBenhService.GetListChuanDoan(model);
            return Ok(lookup);
        }

        [HttpPost("LuuTienSuBenh")]
        public async Task<BenhNhanTienSuKhamBenhGridVo> LuuTienSuBenh([FromBody]BenhNhanTienSuBenhViewModel tienSuBenh)
        {
            if (string.IsNullOrEmpty(tienSuBenh.TenBenh) && string.IsNullOrEmpty(tienSuBenh.TenTinhTrang))
            {
                throw new ApiException(_localizationService.GetResource("TienSuBenh.GiaDinhHoacBanThan.Required"));
            }

            var tienSuBenhEntity = new BenhNhanTienSuBenh
            {
                Id = tienSuBenh.Id,
                BenhNhanId = tienSuBenh.BenhNhanId,
                TenBenh = tienSuBenh.TenBenh,
            };
            await _benhNhanTienSuBenhService.AddAsync(tienSuBenhEntity);

            return new BenhNhanTienSuKhamBenhGridVo
            {
                Id = tienSuBenhEntity.Id,
                //NgayPhatHien = tienSuBenhEntity.NgayPhatHien?.ApplyFormatDate()
            };
        }

        [HttpPost("GetDataGridTienSuBenh")]
        public async Task<ActionResult<GridDataSource>> GetDataGridTienSuBenh(QueryInfo queryInfo)
        {
            var gridData = await _benhNhanTienSuBenhService.GetDataGridTienSuBenh(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("LuuDiUngThuoc")]
        public async Task<ActionResult> LuuDiUngThuoc([FromBody]BenhNhanDiUngThuocViewModel diUngThuoc)
        {
            if (diUngThuoc.TenDiUng == "null")
            {
                diUngThuoc.TenDiUng = await _khamBenhService.GetTenThuoc(diUngThuoc.ThuocId);
            }

            var loaiDiUng = diUngThuoc.LoaiDiUng.GetDescription();

            var diUngThuocEntitiy = new BenhNhanDiUngThuoc
            {
                Id = diUngThuoc.Id,
                BenhNhanId = diUngThuoc.BenhNhanId,
                LoaiDiUng = diUngThuoc.LoaiDiUng,
                TenDiUng = diUngThuoc.TenDiUng,
                BieuHienDiUng = diUngThuoc.BieuHienDiUng,
                MucDo = diUngThuoc.MucDo.Value
            };
            await _benhNhanDiUngThuocService.AddAsync(diUngThuocEntitiy);

            var diUngThuocUi = new BenhNhanDiUngThuocUiReturnViewModel
            {
                Id = diUngThuocEntitiy.Id,
                LoaiDiUng = loaiDiUng,
                TenDiUng = diUngThuocEntitiy.TenDiUng,
                TenMucDo = diUngThuocEntitiy.MucDo.GetDescription()
            };

            return Ok(diUngThuocUi);
        }

        [HttpPost("XoaTienSuBenh")]
        public async Task<ActionResult> XoaTienSuBenh(long id)
        {
            var tienSuBenh = await _benhNhanTienSuBenhService.GetByIdAsync(id);

            if (tienSuBenh == null)
            {
                return NotFound();
            }

            await _benhNhanTienSuBenhService.DeleteByIdAsync(id);
            return Ok();
        }

        [HttpPost("XoaDiUngThuoc")]
        public async Task<ActionResult> XoaDiUngThuoc(long id)
        {
            var diUngThuoc = await _benhNhanDiUngThuocService.GetByIdAsync(id);

            if (diUngThuoc == null)
            {
                return NotFound();
            }

            await _benhNhanDiUngThuocService.DeleteByIdAsync(id);
            return Ok();
        }

        [HttpPost("GetDataGridBenhNhanDiUngThuoc")]
        public async Task<ActionResult<GridDataSource>> GetDataGridBenhNhanDiUngThuoc(QueryInfo queryInfo)
        {
            var gridData = await _benhNhanDiUngThuocService.GetDataGridBenhNhanDiUngThuoc(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("SaveTrieuChung")]
        public async Task<ActionResult<GridDataSource>> SaveTrieuChung([FromBody]KhamBenhTrieuChungViewModel trieuChungObj)
        {
            if (trieuChungObj.trieuChungChange)
            {
                if (trieuChungObj.idTrieuChungsInsert.Length != 0)
                {
                    foreach (var idTrieuChung in trieuChungObj.idTrieuChungsInsert)
                    {
                        var yeuCauTrieuChung = new YeuCauKhamBenhTrieuChung
                        {
                            YeuCauKhamBenhId = trieuChungObj.idYeuCauKhamBenh,
                            TrieuChungId = idTrieuChung
                        };
                        await _yeuCauKhamBenhTrieuChungService.AddAsync(yeuCauTrieuChung);
                    }
                }

                if (trieuChungObj.idTrieuChungsDelete.Length != 0)
                {
                    foreach (var idTrieuChung in trieuChungObj.idTrieuChungsDelete)
                    {
                        var yeuCauTrieuChung = new YeuCauKhamBenhTrieuChung
                        {
                            YeuCauKhamBenhId = trieuChungObj.idYeuCauKhamBenh,
                            TrieuChungId = idTrieuChung
                        };
                        long id = await _yeuCauKhamBenhTrieuChungService.GetIdTask(yeuCauTrieuChung.YeuCauKhamBenhId, yeuCauTrieuChung.TrieuChungId);
                        await _yeuCauKhamBenhTrieuChungService.DeleteByIdAsync(id);
                    }
                }
            }

            var gridTraVe =
                await _yeuCauKhamBenhTrieuChungService.GetDataGridYeuCauKhamBenhTrieuChung(trieuChungObj
                    .idYeuCauKhamBenh);

            return Ok(gridTraVe);
        }

        [HttpPost("DeleteTrieuChung")]
        public async Task<ActionResult> DeleteTrieuChung(long idTrieuChung, long idYeuCauKhamBenh)
        {
            var id = _khamBenhService.GetIdYeuCauTiepNhan(idTrieuChung, idYeuCauKhamBenh);

            var yeuCauTrieuChung = await _yeuCauKhamBenhTrieuChungService.GetByIdAsync(id);

            if (yeuCauTrieuChung == null)
            {
                return NotFound();
            }

            await _yeuCauKhamBenhTrieuChungService.DeleteByIdAsync(id);
            return Ok();
        }

        [HttpPost("DeleteAllTrieuChungByYeuCauTiepNhan")]
        public async Task<ActionResult> DeleteAllTrieuChungByYeuCauTiepNhan(long idYeuCauKhamBenh)
        {
            var listId = _khamBenhService.DeleteAllTrieuChungByYctn(idYeuCauKhamBenh);

            foreach (var id in listId)
            {
                await _yeuCauKhamBenhTrieuChungService.DeleteByIdAsync(id);
            }

            return Ok();
        }

        [HttpPost("SaveChuanDoan")]
        public async Task<ActionResult> SaveChuanDoan([FromBody]KhamBenhChuanDoanViewModel chuanDoanObj)
        {
            if (chuanDoanObj.chuanDoanChange)
            {
                if (chuanDoanObj.idChuanDoansInsert.Length != 0)
                {
                    foreach (var idChuanDoan in chuanDoanObj.idChuanDoansInsert)
                    {
                        var yeuCauChuanDoan = new YeuCauKhamBenhChuanDoan
                        {
                            YeuCauKhamBenhId = chuanDoanObj.idYeuCauKhamBenh,
                            ChuanDoanId = idChuanDoan
                        };
                        await _yeuCauKhamBenhChuanDoanService.AddAsync(yeuCauChuanDoan);
                    }
                }

                if (chuanDoanObj.idChuanDoansDelete.Length != 0)
                {
                    foreach (var idChuanDoan in chuanDoanObj.idChuanDoansDelete)
                    {
                        var yeuCauChuanDoan = new YeuCauKhamBenhChuanDoan
                        {
                            YeuCauKhamBenhId = chuanDoanObj.idYeuCauKhamBenh,
                            ChuanDoanId = idChuanDoan
                        };
                        long id = await _yeuCauKhamBenhChuanDoanService.GetIdTask(yeuCauChuanDoan.YeuCauKhamBenhId, yeuCauChuanDoan.ChuanDoanId);
                        await _yeuCauKhamBenhChuanDoanService.DeleteByIdAsync(id);
                    }
                }
            }

            var gridTraVe =
                await _yeuCauKhamBenhChuanDoanService.GetDataGridYeuCauKhamBenhChuanDoan(chuanDoanObj.idYeuCauKhamBenh);

            return Ok(gridTraVe);
        }

        [HttpPost("SaveTemplate")]
        public async Task<ActionResult> SaveTemplate([FromBody]DynamicTemplateKhoaPhongViewModel templateObj)
        {
            if (templateObj.templateChange)
            {
                var yeuCauKhamBenh = await _yeuCauKhamBenhService.GetByIdAsync(templateObj.idKhamBenh);
                yeuCauKhamBenh.ThongTinKhamTheoKhoa = templateObj.template;
                await _yeuCauKhamBenhService.UpdateAsync(yeuCauKhamBenh);
            }

            return Ok();
        }

        [HttpPost("DeleteChuanDoan")]
        public async Task<ActionResult> DeleteChuanDoan(long idChuanDoan, long idYeuCauKhamBenh)
        {
            var id = _khamBenhService.GetIdYeuCauChuanDoan(idChuanDoan, idYeuCauKhamBenh);

            var yeuCauChuanDoan = await _yeuCauKhamBenhChuanDoanService.GetByIdAsync(id);

            if (yeuCauChuanDoan == null)
            {
                return NotFound();
            }

            await _yeuCauKhamBenhChuanDoanService.DeleteByIdAsync(id);
            return Ok();
        }

        [HttpPost("DeleteAllChuanDoanByYeuCauTiepNhan")]
        public async Task<ActionResult> DeleteAllChuanDoanByYeuCauTiepNhan(long idYeuCauKhamBenh)
        {
            var listId = _khamBenhService.DeleteAllChuanDoanByYeuCauTiepNhan(idYeuCauKhamBenh);

            foreach (var id in listId)
            {
                await _yeuCauKhamBenhChuanDoanService.DeleteByIdAsync(id);
            }

            return Ok();
        }

        [HttpPost("GetTenNhanVien")]
        public string GetTenNhanVien(long userId)
        {
            var ten = _khamBenhService.GetTenNhanVien(userId);
            return ten;
        }

        [HttpPost("LuuChiSoSinhHieu")]
        public async Task<long> LuuChiSoSinhHieu([FromBody]KetQuaSinhHieuViewModel sinhHieu)
        {
            if (sinhHieu.BMI == null && sinhHieu.CanNang == null && sinhHieu.ChieuCao == null &&
                sinhHieu.HuyetApTamThu == null
                && sinhHieu.HuyetApTamTruong == null && sinhHieu.NhipTho == null && sinhHieu.NhipTim == null &&
                sinhHieu.ThanNhiet == null && sinhHieu.Glassgow == null)
            {
                throw new ApiException(_localizationService.GetResource("KhamBenh.Empty.IsRequired"), (int)HttpStatusCode.BadRequest);
            }

            if (sinhHieu.HuyetApTamThu != null || sinhHieu.HuyetApTamTruong != null)
            {
                if (sinhHieu.HuyetApTamThu != null && sinhHieu.HuyetApTamTruong == null ||
                    sinhHieu.HuyetApTamThu == null && sinhHieu.HuyetApTamTruong != null)
                {
                    throw new ApiException(_localizationService.GetResource("KhamBenh.TamThuTamTruong.Empty.IsRequired"), (int)HttpStatusCode.BadRequest);
                }
            }

            var sinhHieuEntitiy = new KetQuaSinhHieu
            {
                Id = sinhHieu.Id,
                YeuCauTiepNhanId = sinhHieu.YeuCauTiepNhanId,
                NhipTim = sinhHieu.NhipTim,
                NhipTho = sinhHieu.NhipTho,
                ThanNhiet = sinhHieu.ThanNhiet,
                HuyetApTamThu = sinhHieu.HuyetApTamThu,
                HuyetApTamTruong = sinhHieu.HuyetApTamTruong,
                ChieuCao = sinhHieu.ChieuCao,
                CanNang = sinhHieu.CanNang,
                Bmi = sinhHieu.BMI,
                NoiThucHienId = sinhHieu.NoiThucHienId,
                NhanVienThucHienId = sinhHieu.NhanVienThucHienId,
                Glassgow = sinhHieu.Glassgow
            };
            await _ketQuaSinhHieuService.AddAsync(sinhHieuEntitiy);

            return await Task.FromResult(sinhHieuEntitiy.Id);
        }

        [HttpPost("XoaChiSoSinhHieu")]
        public async Task<ActionResult> XoaChiSoSinhHieu(long id)
        {
            var chiSoSinhHieu = await _ketQuaSinhHieuService.GetByIdAsync(id);

            if (chiSoSinhHieu == null)
            {
                return NotFound();
            }

            await _ketQuaSinhHieuService.DeleteByIdAsync(id);
            return Ok();
        }

        [HttpPost("SaveTrieuChungKhamText")]
        public async Task<ActionResult> SaveTrieuChungKhamText([FromBody]GhiChuVaTrieuChungKhamTextViewModel trieuChungTxtObj)
        {
            if (trieuChungTxtObj.FlagChangeTrieuChungKhamText)
            {
                var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(trieuChungTxtObj.YeuCauTiepNhanId);
                yeuCauTiepNhan.TrieuChungTiepNhan = trieuChungTxtObj.TrieuChungKham;
                await _yeuCauTiepNhanService.UpdateAsync(yeuCauTiepNhan);
            }

            if (trieuChungTxtObj.FlagChangeGhiChuTrieuChungKhamText)
            {
                var yeuCauKhamBenh = await _yeuCauKhamBenhService.GetByIdAsync(trieuChungTxtObj.YeuCauKhamBenhId);
                yeuCauKhamBenh.GhiChuTrieuChungLamSang = trieuChungTxtObj.GhiChuTrieuChungKham;
                await _yeuCauKhamBenhService.UpdateAsync(yeuCauKhamBenh);
            }

            return Ok();
        }

        [HttpPost("GetListMucDoDiUng")]
        public List<LookupItemVo> GetListMucDoDiUng()
        {
            var lookup = _benhNhanDiUngThuocService.GetListMucDoDiUng();
            return lookup;
        }

        [HttpPost("LuuThongTinKhamBenh")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamBenhDangKham, Enums.DocumentType.KhamDoanKhamBenh)]
        public async Task<ActionResult<KhamBenhPhongBenhVienHangDoiViewModel>> LuuThongTinKhamBenhAsync([FromBody]PhongBenhVienHangDoiKhamBenhViewModel hangDoiViewModel)
        {
            var benhNhanHienTai = new PhongBenhVienHangDoi();
            // kiểm tra yêu cầu khám bệnh trước khi lưu
            if (hangDoiViewModel.YeuCauKhamBenh.IsKhamBenhDangKham)
            {
                await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhDangKhamAsync(hangDoiViewModel.YeuCauKhamBenhId ?? 0);
                //benhNhanHienTai = await _yeuCauKhamBenhService.GetYeuCauKhamBenhDangKhamAsync(hangDoiViewModel.Id);
                benhNhanHienTai = await _yeuCauKhamBenhService.GetYeuCauKhamBenhDangKhamLuuTabKhamBenhAsync(hangDoiViewModel.Id);
            }
            else
            {
                await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhAsync(hangDoiViewModel.YeuCauKhamBenhId ?? 0, hangDoiViewModel.Id);
                //benhNhanHienTai = await _yeuCauKhamBenhService.GetYeuCauKhamBenhDangKhamTheoPhongKham(hangDoiViewModel.PhongBenhVienId, hangDoiViewModel.Id, hangDoiViewModel.YeuCauKhamBenh.IsKhamDoan);
                benhNhanHienTai = await _yeuCauKhamBenhService.GetYeuCauKhamBenhDangKhamTheoPhongKhamLuuTabKhamBenh(hangDoiViewModel.PhongBenhVienId, hangDoiViewModel.Id, hangDoiViewModel.YeuCauKhamBenh.IsKhamDoan);
            }
            if (benhNhanHienTai == null)
            {
                throw new ApiException(_localizationService.GetResource("ApiError.ConcurrencyError"));
                //return NotFound();
            }

            #region log 28/03/2022
            try
            {
                LogManager.GetCurrentClassLogger().Info(
                    $"LuuThongTinKhamBenh phongBenhVienId{_userAgentHelper.GetCurrentNoiLLamViecId()}, yeuCauTiepNhanId{benhNhanHienTai.YeuCauTiepNhanId}, yeuCauKhamBenhId{benhNhanHienTai.YeuCauKhamBenhId ?? 0}, currentUser{_userAgentHelper.GetCurrentUserId()}");
            }
            catch (Exception e)
            {

            }
            #endregion

            // kiểm tra kết quả sinh hiệu
            var lstKetQuaSinhHieu = hangDoiViewModel.YeuCauTiepNhan.KetQuaSinhHieus;
            if (lstKetQuaSinhHieu.Any())
            {
                lstKetQuaSinhHieu = lstKetQuaSinhHieu.Where(x => x.Id == 0).ToList();
                foreach (var item in lstKetQuaSinhHieu)
                {
                    if (item.BMI == null && item.CanNang == null && item.ChieuCao == null &&
                        item.HuyetApTamThu == null && item.HuyetApTamTruong == null && item.NhipTho == null && item.NhipTim == null &&
                        item.ThanNhiet == null && item.Glassgow == null && item.SpO2 == null)
                    {
                        throw new ApiException(_localizationService.GetResource("KhamBenh.Empty.IsRequired"), (int)HttpStatusCode.BadRequest);
                    }

                    if (item.HuyetApTamThu != null && item.HuyetApTamTruong == null ||
                        item.HuyetApTamThu == null && item.HuyetApTamTruong != null)
                    {
                        throw new ApiException(_localizationService.GetResource("KhamBenh.TamThuTamTruong.Empty.IsRequired"), (int)HttpStatusCode.BadRequest);
                    }
                }

            }

            // kiểm tra tự động lưu chẩn đoán icd chính
            var isAutoFillICDChinh = false;
            if (hangDoiViewModel.YeuCauKhamBenh.IsHoanThanhKham != true && benhNhanHienTai.YeuCauKhamBenh.IcdchinhId == null && hangDoiViewModel.YeuCauKhamBenh.ChanDoanSoBoICDId != null)
            {
                isAutoFillICDChinh = true;
                benhNhanHienTai.YeuCauKhamBenh.IcdchinhId = hangDoiViewModel.YeuCauKhamBenh.ChanDoanSoBoICDId;
            }

            if (isAutoFillICDChinh
                && string.IsNullOrEmpty(benhNhanHienTai.YeuCauKhamBenh.GhiChuICDChinh) 
                && !string.IsNullOrEmpty(hangDoiViewModel.YeuCauKhamBenh.ChanDoanSoBoGhiChu))
            {
                benhNhanHienTai.YeuCauKhamBenh.GhiChuICDChinh = hangDoiViewModel.YeuCauKhamBenh.ChanDoanSoBoGhiChu;
            }

            //kiểm tra chẩn đoán phân biệt
            var lstChanDoanPhanBietId =
                hangDoiViewModel.YeuCauKhamBenh.YeuCauKhamBenhChanDoanPhanBiets.Where(x => x.Id == 0)
                    .Select(x => x.ICDId).ToList();
            if (lstChanDoanPhanBietId.Count() != lstChanDoanPhanBietId.Distinct().Count())
            {
                throw new ApiException(_localizationService.GetResource("KhamBenh.ChanDoanPhanBietICDId.IsExists"));
            }

            hangDoiViewModel.ToEntity(benhNhanHienTai);

            // kiểm tra trạng thái yêu cầu khám bệnh dang khám
            if (benhNhanHienTai.YeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham)
            {
                benhNhanHienTai.YeuCauKhamBenh.TrangThai = Enums.EnumTrangThaiYeuCauKhamBenh.DangKham;
                benhNhanHienTai.YeuCauKhamBenh.NoiThucHienId = benhNhanHienTai.YeuCauKhamBenh.NoiDangKyId; //_userAgentHelper.GetCurrentNoiLLamViecId();
                benhNhanHienTai.YeuCauKhamBenh.BacSiThucHienId = _userAgentHelper.GetCurrentUserId();
                benhNhanHienTai.YeuCauKhamBenh.ThoiDiemThucHien = DateTime.Now;

                // bổ sung khám đoàn
                if (benhNhanHienTai.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe)
                {
                    benhNhanHienTai.YeuCauKhamBenh.BacSiKetLuanId = _userAgentHelper.GetCurrentUserId();
                    benhNhanHienTai.YeuCauKhamBenh.NoiKetLuanId = benhNhanHienTai.YeuCauKhamBenh.NoiDangKyId; //_userAgentHelper.GetCurrentNoiLLamViecId();
                }

                YeuCauKhamBenhLichSuTrangThai trangThaiMoi = new YeuCauKhamBenhLichSuTrangThai
                {
                    TrangThaiYeuCauKhamBenh = benhNhanHienTai.YeuCauKhamBenh.TrangThai,
                    MoTa = benhNhanHienTai.YeuCauKhamBenh.TrangThai.GetDescription()
                };
                benhNhanHienTai.YeuCauKhamBenh.YeuCauKhamBenhLichSuTrangThais.Add(trangThaiMoi);
            }

            await _phongBenhVienHangDoiService.UpdateAsync(benhNhanHienTai);
            
            var yeuCauKhamBenh = benhNhanHienTai.ToModel<KhamBenhPhongBenhVienHangDoiViewModel>();

            foreach (var item in yeuCauKhamBenh.YeuCauKhamBenh.YeuCauKhamBenhChanDoanPhanBiets)
            {
                if (string.IsNullOrEmpty(item.TenICD))
                {
                    var chanDoanPhanBiet = hangDoiViewModel.YeuCauKhamBenh.YeuCauKhamBenhChanDoanPhanBiets.FirstOrDefault(x => x.ICDId == item.ICDId);
                    item.TenICD = chanDoanPhanBiet?.TenICD;
                }
            }
            //Kiem tra neu icd khac null va phan biet item > 0
            if (!yeuCauKhamBenh.YeuCauKhamBenh.YeuCauKhamBenhICDKhacs.Any() && yeuCauKhamBenh.YeuCauKhamBenh.YeuCauKhamBenhChanDoanPhanBiets.Any())
            {
                foreach (var icdKhac in yeuCauKhamBenh.YeuCauKhamBenh.YeuCauKhamBenhICDKhacs)
                {
                    if (string.IsNullOrEmpty(icdKhac.TenICD))
                    {
                        var chanDoanICDKhac = hangDoiViewModel.YeuCauKhamBenh.YeuCauKhamBenhChanDoanPhanBiets.FirstOrDefault(x => x.ICDId == icdKhac.ICDId);
                        icdKhac.TenICD = chanDoanICDKhac?.TenICD;
                    }
                }
            }

            // kiểm tra tự động lưu chẩn đoán icd chính
            if (yeuCauKhamBenh.YeuCauKhamBenh.IcdchinhId != null 
                && yeuCauKhamBenh.YeuCauKhamBenh.IcdchinhId != 0 
                && yeuCauKhamBenh.YeuCauKhamBenh.ChanDoanSoBoICDId == yeuCauKhamBenh.YeuCauKhamBenh.IcdchinhId
                && string.IsNullOrEmpty(yeuCauKhamBenh.YeuCauKhamBenh.TenICDChinh))
            {
                yeuCauKhamBenh.YeuCauKhamBenh.TenICDChinh = hangDoiViewModel.YeuCauKhamBenh.TenChanDoanSoBoICD;
            }

            return yeuCauKhamBenh;
        }

        [HttpPost("GetListICDBaoGomItemDaChon")]
        public async Task<List<ICDTemplateVo>> GetListICDBaoGomItemDaChonAsync(DropDownListRequestModel queryInfo)
        {
            var lookup = await _yeuCauKhamBenhService.GetListICDBaoGomItemDaChonAsync(queryInfo);
            return lookup;
        }

        [HttpPost("getListLoaiTienSuBenh")]
        public ActionResult<ICollection<Enums.LoaiDiUng>> getListLoaiTienSuBenh()
        {
            var listEnum = _khamBenhService.getListLoaiTienSuBenh();
            return Ok(listEnum);
        }

        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamBenhDangKham, Enums.DocumentType.KhamDoanKhamBenh)]
        [HttpPost("KiemTraValidationBoPhanTonThuong")]
        public async Task<ActionResult> KiemTraValidationBoPhanTonThuong([FromBody] YeuCauKhamBenhBoPhanTonThuongViewModel viewModel)
        {
            return NoContent();
        }

        //KiemTraValidationChuyenKham // hiện tại không sử dụng
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamBenhDangKham, Enums.DocumentType.KhamDoanKhamBenh)]
        [HttpPost("KiemTraValidationChuyenKham")]
        public async Task<ActionResult> KiemTraValidationChuyenKhamAsync([FromBody] ChuyenKhamYeuCauKhamBenhViewModel viewModel)
        {
            if (viewModel.IsKhamBenhDangKham == true)
            {
                await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhDangKhamAsync(viewModel.YeuCauKhamBenhTruocId);
            }
            else
            {
                await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhAsync(viewModel.YeuCauKhamBenhTruocId);
            }

            //var dichVuKhamBenh = await _dichVuKhamBenhBenhVienService.GetByIdAsync(viewModel.DichVuKhamBenhBenhVienId ?? 0,
            //    x => x.Include(y => y.DichVuKhamBenhBenhVienGiaBenhViens));

            //if (!dichVuKhamBenh.DichVuKhamBenhBenhVienGiaBenhViens.Any(x => x.TuNgay.Date <= DateTime.Now.Date && (x.DenNgay == null || x.DenNgay.Value.Date >= DateTime.Now.Date)))
            //{
            //    throw new ApiException(_localizationService.GetResource("ChuyenDichVuKham.DichVuKhamBenhGiaBenhVien.NotExists"));
            //}

            var yeuCauTiepNhanChiTiet = await _khamBenhService.GetYeuCauTiepNhanByIdAsync(viewModel.YeuCauTiepNhanId);
            await _yeuCauKhamBenhService.KiemTraDataChuyenKhamAsync(yeuCauTiepNhanChiTiet, viewModel.YeuCauKhamBenhTruocId, viewModel.DichVuKhamBenhBenhVienId.Value);
            return NoContent();
        }

        // hiện tại không sử dụng
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamBenhDangKham, Enums.DocumentType.KhamDoanKhamBenh)]
        [HttpPost("XuLyChuyenDichVuKham")]
        public async Task<ActionResult> XuLyChuyenDichVuKhamAsync([FromBody] ChuyenKhamYeuCauKhamBenhViewModel viewModel)
        {
            // tất cả kiểm tra xử lý trong function KiemTraValidationChuyenKham

            // xử lý hủy dịch vụ khám hiện tại
            var yeuCauTiepNhanChiTiet = await _khamBenhService.GetYeuCauTiepNhanByIdAsync(viewModel.YeuCauTiepNhanId);
            //var yeuCauKhamCanHuy = yeuCauTiepNhanChiTiet.YeuCauKhamBenhs.FirstOrDefault(x => x.Id == viewModel.YeuCauKhamBenhTruocId);
            //if (yeuCauKhamCanHuy == null)
            //{
            //    throw new ApiException(_localizationService.GetResource("ApiError.EntityNull"));
            //}
            //if (yeuCauKhamCanHuy.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham)
            //{
            //    throw new ApiException(_localizationService.GetResource("ChuyenKham.DichVuKham.DaThucHien"));
            //}

            //yeuCauKhamCanHuy.WillDelete = true;

            //////xử lý xóa hàng đợi của yêu cầu khám cần hủy
            ////var hangDoiTheoYeuCauKham = yeuCauKhamCanHuy.PhongBenhVienHangDois.FirstOrDefault(x => x.Id == viewModel.PhongBenhVienHangDoiTruocId);
            ////if (hangDoiTheoYeuCauKham != null)
            ////{
            ////    hangDoiTheoYeuCauKham.WillDelete = true;
            ////}

            //await _tiepNhanBenhNhanServiceService.PrepareForDeleteDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);
            await _tiepNhanBenhNhanServiceService.XuLyHuyTatCaDichVuTruocKhiChuyenKhamAsync(yeuCauTiepNhanChiTiet, viewModel.YeuCauKhamBenhTruocId);


            #region xử lý tạo mới dịch vụ khám chuyển khám
            var yeuCauKhamEntity = viewModel.ToEntity<YeuCauKhamBenh>();
            yeuCauKhamEntity.YeuCauKhamBenhTruocId = null;
            await _yeuCauKhamBenhService.XuLyDataYeuCauKhamBenhChuyenKhamAsync(yeuCauKhamEntity, yeuCauTiepNhanChiTiet.CoBHYT ?? false);

            yeuCauTiepNhanChiTiet.YeuCauKhamBenhs.Add(yeuCauKhamEntity);
            await _tiepNhanBenhNhanServiceService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);
            #endregion

            return Ok();
        }


        [HttpPost("GetListNoiDungMauKhamBenhTheoBacSi")]
        public async Task<List<NoiDungMauKhamBenhLookupItemVo>> GetListNoiDungMauKhamBenhTheoBacSiAsync(DropDownListRequestModel queryInfo)
        {
            var lookup = await _yeuCauKhamBenhService.GetListNoiDungMauKhamBenhTheoBacSiAsync(queryInfo);
            return lookup;
        }

        #region BVHD-3574
        [HttpGet("GetThongTinDoiTuongTiepNhan")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh)]
        public async Task<ActionResult<KhamBenhThongTinDoiTuongViewModel>> GetThongTinDoiTuongTiepNhanAsync(int yeuCauTiepNhanId)
        {
            var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(yeuCauTiepNhanId,
                x => x.Include(z => z.BHYTGiayMienCungChiTra)
                    .Include(y => y.YeuCauTiepNhanCongTyBaoHiemTuNhans).ThenInclude(z => z.CongTyBaoHiemTuNhan));

            var result = yeuCauTiepNhan.ToModel<KhamBenhThongTinDoiTuongViewModel>();
            if (!string.IsNullOrEmpty(result.BHYTMaDKBD))
            {
                var benhVien = await _benhVienService.GetBenhVienWithMaBenhVien(result.BHYTMaDKBD);
                if (benhVien != null)
                {
                    result.NoiDangKyBHYT = benhVien.Ten;
                }
            }
            
            return result;
        }

        [HttpPost("KiemTraThemCongTyBHTN")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.KhamBenh)]
        public async Task<ActionResult> KiemTraThemCongTyBHTN([FromBody] KhamBenhYeuCauTiepNhanCongTyBaoHiemTuNhanViewModel model)
        {
            return NoContent();
        }


        [HttpPut("XuLyCapNhatThongTinDoiTuong")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.TiepNhanNoiTru, Enums.DocumentType.KhamBenh)]
        public async Task<ActionResult> XuLyCapNhatThongTinDoiTuongAsync([FromBody] KhamBenhThongTinDoiTuongViewModel yeuCauViewModel)
        {
            if (yeuCauViewModel.CoBHYT == true)
            {
                var kiemTra = await _tiepNhanBenhNhanService.KiemTraDieuKienTaoMoiYeuCauTiepNhanAsync(yeuCauViewModel.BenhNhanId, false, yeuCauViewModel.Id);
                if (!string.IsNullOrEmpty(kiemTra.ErrorMessage))
                {
                    throw new ApiException(kiemTra.ErrorMessage);
                }

                if (yeuCauViewModel.BHYTNgayHieuLuc.Value.Date > DateTime.Now.Date || yeuCauViewModel.BHYTNgayHetHan.Value.Date < DateTime.Now.Date)
                {
                    throw new ApiException(_localizationService.GetResource("ThongTinDoiTuongTiepNhan.TheKhongCoHieuLuc"));
                }
            }

            if (yeuCauViewModel.CoBHTN == true && !yeuCauViewModel.YeuCauTiepNhanCongTyBaoHiemTuNhans.Any())
            {
                throw new ApiException(_localizationService.GetResource("ThongTinDoiTuongTiepNhan.CongTyBaoHiemTuNhan.Required"));
            }

            if (yeuCauViewModel.BHYTGiayMienCungChiTra != null
                && (string.IsNullOrEmpty(yeuCauViewModel.BHYTGiayMienCungChiTra.DuongDan) || string.IsNullOrEmpty(yeuCauViewModel.BHYTGiayMienCungChiTra.TenGuid)))
            {
                yeuCauViewModel.BHYTGiayMienCungChiTra = null;
            }


            var yeuCauTiepNhan = _yeuCauTiepNhanService.GetById(yeuCauViewModel.Id,
                x => x.Include(y => y.BenhNhan)
                            .Include(z => z.BHYTGiayMienCungChiTra)
                            .Include(y => y.YeuCauTiepNhanCongTyBaoHiemTuNhans).ThenInclude(z => z.CongTyBaoHiemTuNhan));

            if (yeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien)
            {
                throw new ApiException(_localizationService.GetResource("ThongTinDoiTuongTiepNhan.TrangThaiYeuCauTiepNhan.DaThucHienHoacHuy"));
            }

            var theBHYTCuTrongYCTN = yeuCauTiepNhan.BHYTMaSoThe;
            var giayMienCungChiTraGuid = yeuCauTiepNhan.BHYTGiayMienCungChiTra?.TenGuid;
            var giayMienCungChiTraDuongDan = yeuCauTiepNhan.BHYTGiayMienCungChiTra?.DuongDan;

            //sau khi chọn file, thì component file upload lại clear mất id
            //if (yeuCauViewModel.BHYTDuocMienCungChiTra == true 
            //    && yeuCauViewModel.BHYTGiayMienCungChiTraId != null 
            //    && yeuCauViewModel.BHYTGiayMienCungChiTra != null
            //    && yeuCauViewModel.BHYTGiayMienCungChiTra.Id == yeuCauViewModel.BHYTGiayMienCungChiTraId)
            //{
            //    yeuCauViewModel.BHYTGiayMienCungChiTra.Id = yeuCauViewModel.BHYTGiayMienCungChiTraId.Value;
            //}

            // chỉ user có quyền mới đc cập nhật thông tin hành chính người bệnh
            var coQuyenCapNhatHanhChinh = _roleService.IsHavePermissionForUpdateInformationTNBN();
            if (!coQuyenCapNhatHanhChinh)
            {
                yeuCauViewModel.HoTen = yeuCauTiepNhan.HoTen.ToUpper();
                yeuCauViewModel.NgaySinh = yeuCauTiepNhan.NgaySinh;
                yeuCauViewModel.ThangSinh = yeuCauTiepNhan.ThangSinh;
                yeuCauViewModel.NamSinh = yeuCauTiepNhan.NamSinh;
                yeuCauViewModel.PhuongXaId = yeuCauTiepNhan.PhuongXaId;
                yeuCauViewModel.TinhThanhId = yeuCauTiepNhan.TinhThanhId;
                yeuCauViewModel.QuanHuyenId = yeuCauTiepNhan.QuanHuyenId;
                yeuCauViewModel.DiaChi = yeuCauTiepNhan.DiaChi;
                yeuCauViewModel.QuocTichId = yeuCauTiepNhan.QuocTichId;
                yeuCauViewModel.SoDienThoai = yeuCauTiepNhan.SoDienThoai;
                yeuCauViewModel.SoChungMinhThu = yeuCauTiepNhan.SoChungMinhThu;
                yeuCauViewModel.Email = yeuCauTiepNhan.Email;
                yeuCauViewModel.NgheNghiepId = yeuCauTiepNhan.NgheNghiepId;
                yeuCauViewModel.GioiTinh = yeuCauTiepNhan.GioiTinh;
                yeuCauViewModel.NoiLamViec = yeuCauTiepNhan.NoiLamViec;
                yeuCauViewModel.DanTocId = yeuCauTiepNhan.DanTocId;

                yeuCauViewModel.NguoiLienHeHoTen = yeuCauTiepNhan.NguoiLienHeHoTen;
                yeuCauViewModel.NguoiLienHeQuanHeNhanThanId = yeuCauTiepNhan.NguoiLienHeQuanHeNhanThanId;
                yeuCauViewModel.NguoiLienHeSoDienThoai = yeuCauTiepNhan.NguoiLienHeSoDienThoai;
                yeuCauViewModel.NguoiLienHeEmail = yeuCauTiepNhan.NguoiLienHeEmail;
                yeuCauViewModel.NguoiLienHeTinhThanhId = yeuCauTiepNhan.NguoiLienHeTinhThanhId;
                yeuCauViewModel.NguoiLienHeQuanHuyenId = yeuCauTiepNhan.NguoiLienHeQuanHuyenId;
                yeuCauViewModel.NguoiLienHePhuongXaId = yeuCauTiepNhan.NguoiLienHePhuongXaId;
                yeuCauViewModel.NguoiLienHeDiaChi = yeuCauTiepNhan.NguoiLienHeDiaChi;
            }

            var thongTinDoiTuong = yeuCauViewModel.ToEntity(yeuCauTiepNhan);

            if (yeuCauTiepNhan.BenhNhan != null)
            {
                yeuCauTiepNhan.BenhNhan.HoTen = yeuCauTiepNhan.HoTen.ToUpper();
                yeuCauTiepNhan.BenhNhan.NgaySinh = yeuCauTiepNhan.NgaySinh;
                yeuCauTiepNhan.BenhNhan.ThangSinh = yeuCauTiepNhan.ThangSinh;
                yeuCauTiepNhan.BenhNhan.NamSinh = yeuCauTiepNhan.NamSinh;
                yeuCauTiepNhan.BenhNhan.PhuongXaId = yeuCauTiepNhan.PhuongXaId;
                yeuCauTiepNhan.BenhNhan.TinhThanhId = yeuCauTiepNhan.TinhThanhId;
                yeuCauTiepNhan.BenhNhan.QuanHuyenId = yeuCauTiepNhan.QuanHuyenId;
                yeuCauTiepNhan.BenhNhan.DiaChi = yeuCauTiepNhan.DiaChi;
                yeuCauTiepNhan.BenhNhan.QuocTichId = yeuCauTiepNhan.QuocTichId;
                yeuCauTiepNhan.BenhNhan.SoDienThoai = yeuCauTiepNhan.SoDienThoai;
                yeuCauTiepNhan.BenhNhan.SoChungMinhThu = yeuCauTiepNhan.SoChungMinhThu;
                yeuCauTiepNhan.BenhNhan.Email = yeuCauTiepNhan.Email;
                yeuCauTiepNhan.BenhNhan.NgheNghiepId = yeuCauTiepNhan.NgheNghiepId;
                yeuCauTiepNhan.BenhNhan.GioiTinh = yeuCauTiepNhan.GioiTinh;
                yeuCauTiepNhan.BenhNhan.NoiLamViec = yeuCauTiepNhan.NoiLamViec;
                yeuCauTiepNhan.BenhNhan.DanTocId = yeuCauTiepNhan.DanTocId;

                if (!string.IsNullOrEmpty(yeuCauTiepNhan.NguoiLienHeHoTen)
                    || (yeuCauTiepNhan.NguoiLienHeQuanHeNhanThanId != 0 && yeuCauTiepNhan.NguoiLienHeQuanHeNhanThanId != null)
                    || !string.IsNullOrEmpty(yeuCauTiepNhan.NguoiLienHeSoDienThoai)
                    || !string.IsNullOrEmpty(yeuCauTiepNhan.NguoiLienHeEmail)
                    || (yeuCauTiepNhan.NguoiLienHeTinhThanhId != 0 && yeuCauTiepNhan.NguoiLienHeTinhThanhId != null)
                    || (yeuCauTiepNhan.NguoiLienHeQuanHuyenId != 0 && yeuCauTiepNhan.NguoiLienHeQuanHuyenId != null)
                    || (yeuCauTiepNhan.NguoiLienHePhuongXaId != 0 && yeuCauTiepNhan.NguoiLienHePhuongXaId != null)
                    || !string.IsNullOrEmpty(yeuCauTiepNhan.NguoiLienHeDiaChi)
                )
                {
                    yeuCauTiepNhan.BenhNhan.NguoiLienHeHoTen = yeuCauTiepNhan.NguoiLienHeHoTen;
                    yeuCauTiepNhan.BenhNhan.NguoiLienHeQuanHeNhanThanId = yeuCauTiepNhan.NguoiLienHeQuanHeNhanThanId;
                    yeuCauTiepNhan.BenhNhan.NguoiLienHeSoDienThoai = yeuCauTiepNhan.NguoiLienHeSoDienThoai;
                    yeuCauTiepNhan.BenhNhan.NguoiLienHeEmail = yeuCauTiepNhan.NguoiLienHeEmail;
                    yeuCauTiepNhan.BenhNhan.NguoiLienHeTinhThanhId = yeuCauTiepNhan.NguoiLienHeTinhThanhId;
                    yeuCauTiepNhan.BenhNhan.NguoiLienHeQuanHuyenId = yeuCauTiepNhan.NguoiLienHeQuanHuyenId;
                    yeuCauTiepNhan.BenhNhan.NguoiLienHePhuongXaId = yeuCauTiepNhan.NguoiLienHePhuongXaId;
                    yeuCauTiepNhan.BenhNhan.NguoiLienHeDiaChi = yeuCauTiepNhan.NguoiLienHeDiaChi;
                }
            }

            // tham khảo từ function RemoveBHYT bên tiếp nhận người bệnh
            if (yeuCauTiepNhan.CoBHYT != true)
            {
                thongTinDoiTuong.BHYTNgayDuocMienCungChiTra = null;

                thongTinDoiTuong.BHYTMaDKBD = null;
                thongTinDoiTuong.BHYTNgayDu5Nam = null;
                thongTinDoiTuong.BHYTMucHuong = null;
                thongTinDoiTuong.BHYTDiaChi = null;
                thongTinDoiTuong.BHYTMaSoThe = null;
                thongTinDoiTuong.BHYTCoQuanBHXH = null;
                thongTinDoiTuong.BHYTMaKhuVuc = null;
                thongTinDoiTuong.BHYTNgayHieuLuc = null;
                thongTinDoiTuong.BHYTNgayHetHan = null;

                thongTinDoiTuong.LyDoVaoVien = null;

                if (thongTinDoiTuong.BHYTGiayMienCungChiTra != null
                    && !string.IsNullOrEmpty(thongTinDoiTuong.BHYTGiayMienCungChiTra.DuongDan)
                    && !string.IsNullOrEmpty(thongTinDoiTuong.BHYTGiayMienCungChiTra.TenGuid))
                {
                    await _taiLieuDinhKemService.XoaTaiLieuAsync(thongTinDoiTuong.BHYTGiayMienCungChiTra.DuongDan, thongTinDoiTuong.BHYTGiayMienCungChiTra.TenGuid);
                }
            }
            else
            {
                if (thongTinDoiTuong.BHYTDuocMienCungChiTra == false || thongTinDoiTuong.BHYTDuocMienCungChiTra == null)
                {
                    thongTinDoiTuong.BHYTNgayDuocMienCungChiTra = null;
                    if (thongTinDoiTuong.BHYTGiayMienCungChiTra != null)
                    {
                        thongTinDoiTuong.BHYTGiayMienCungChiTra.WillDelete = true;
                    }

                    if (!string.IsNullOrEmpty(giayMienCungChiTraGuid) && !string.IsNullOrEmpty(giayMienCungChiTraDuongDan))
                    {
                        await _taiLieuDinhKemService.XoaTaiLieuAsync(giayMienCungChiTraDuongDan, giayMienCungChiTraGuid);
                    }
                }
                else
                {
                    if ((yeuCauViewModel.BHYTGiayMienCungChiTra != null
                         && yeuCauTiepNhan.BHYTGiayMienCungChiTra != null
                         && !string.IsNullOrEmpty(yeuCauViewModel.BHYTGiayMienCungChiTra.DuongDan)
                         && !string.IsNullOrEmpty(yeuCauViewModel.BHYTGiayMienCungChiTra.TenGuid)
                         && !yeuCauViewModel.BHYTGiayMienCungChiTra.TenGuid.Equals(giayMienCungChiTraGuid)) 
                        || (yeuCauViewModel.BHYTGiayMienCungChiTra != null 
                            && yeuCauTiepNhan.BHYTGiayMienCungChiTra == null
                            && !string.IsNullOrEmpty(yeuCauViewModel.BHYTGiayMienCungChiTra.DuongDan)
                            && !string.IsNullOrEmpty(yeuCauViewModel.BHYTGiayMienCungChiTra.TenGuid)))
                    {
                        thongTinDoiTuong.BHYTGiayMienCungChiTra.Ma = Guid.NewGuid().ToString();
                        await _taiLieuDinhKemService.LuuTaiLieuAsync(yeuCauViewModel.BHYTGiayMienCungChiTra.DuongDan, yeuCauViewModel.BHYTGiayMienCungChiTra.TenGuid);
                    }
                }
            }
            await _yeuCauTiepNhanService.PrepareForEditYeuCauTiepNhanAndUpdateAsync(thongTinDoiTuong);

            //update benh nhan
            if (thongTinDoiTuong.BenhNhanId != null && thongTinDoiTuong.BenhNhanId != 0)
            {
                await _tiepNhanBenhNhanService.UpdateBenhNhanForUpdateView(thongTinDoiTuong.BenhNhanId ?? 0, thongTinDoiTuong);
            }

            //Cập nhật ngược lại yêu cầu nhập viện
            await _yeuCauTiepNhanService.CapNhatThongTinHanhChinhVaoNoiTru(yeuCauTiepNhan, theBHYTCuTrongYCTN);
            return NoContent();
        }
        #endregion
        #region BVHD-3698 thêm phiếu nghỉ dưỡng thai
        [HttpPost("CapNhatNgayDuongThai")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh)]
        public async Task<ActionResult<CapNhatPhieuNghiDuongThai>> CapNhatNgayDuongThai(CapNhatPhieuNghiDuongThai phieuNghiDuongThai)
        {
            _yeuCauKhamBenhService.SaveCapNhatNgayDuongThai(phieuNghiDuongThai.YeuCauKhamBenhId, phieuNghiDuongThai.TuNgay, phieuNghiDuongThai.DenNgay);
            return Ok(phieuNghiDuongThai.YeuCauKhamBenhId);
        }

        #region In  theo yêu cầu khám
        [HttpPost("PhieuNghiDuongThai")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh)]
        public async Task<ActionResult<string>> PhieuNghiDuongThai([FromBody]InPhieuNghiDuongThaiQueryInfo inPhieuNghiDuongThaiQueryInfo)
        {
            var html = await _yeuCauKhamBenhService.PhieuNghiDuongThai(inPhieuNghiDuongThaiQueryInfo.YeuCauKhamBenhId);
            return html;
        }
        #endregion

        [HttpPost("GetNgayDuongThaiYeuCauKhamBenh")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh)]
        public async Task<ActionResult<CapNhatPhieuNghiDuongThai>> GetNgayDuongThaiYeuCauKhamBenh(InPhieuNghiDuongThaiQueryInfo phieuNghiDuongThai)
        {
            var data = _yeuCauKhamBenhService.GetNgayDuongThaiYeuCauKhamBenh(phieuNghiDuongThai.YeuCauKhamBenhId);
            return Ok(data);
        }
        #endregion
    }
}