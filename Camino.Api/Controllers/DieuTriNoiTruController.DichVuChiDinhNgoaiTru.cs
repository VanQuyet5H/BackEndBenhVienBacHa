using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.DieuTriNoiTru;
using Camino.Api.Models.Error;
using Camino.Api.Models.KhamBenh;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public partial class DieuTriNoiTruController
    {
        #region Get data
        [HttpPost("GetDataDefaulDichVuChiDinhNgoaiTru")]
        public async Task<List<KhamBenhGoiDichVuGridVo>> GetDataDefaulDichVuChiDinhNgoaiTru(GridChiDinhDichVuNgoaiTruQueryInfoVo queryInfo)
        {
            var gridData = await _dieuTriNoiTruService.GetDataDefaulDichVuChiDinhNgoaiTru(queryInfo);
            return gridData;
        }

        [HttpPost("GetGridDataGhiNhanVTTHThuocChiDinhNgoaiTru")]
        public async Task<List<GhiNhanVatTuTieuHaoThuocGroupParentGridVo>> GetGridDataGhiNhanVTTHThuocChiDinhNgoaiTruAsync(GridChiDinhDichVuNgoaiTruQueryInfoVo queryInfo)
        {
            //var gridData = await _dieuTriNoiTruService.GetGridDataGhiNhanVTTHThuocChiDinhNgoaiTruAsync(queryInfo);
            var gridData = await _dieuTriNoiTruService.GetGridDataGhiNhanVTTHThuocChiDinhNgoaiTruAsyncVer2(queryInfo);
            return gridData;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridNhomDichVuTheoThoiGianNhapVien")]
        public async Task<ActionResult<GridDataSource>> GetDataForGridNhomDichVuTheoThoiGianNhapVienAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = new GridDataSource();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                gridData = await _dieuTriNoiTruService.GetDataForGridNhomDichVuTheoThoiGianNhapVienAsync(queryInfo);
            }
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridNhomDichVuTheoThoiGianNhapVien")]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridNhomDichVuTheoThoiGianNhapVienAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = new GridDataSource();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                gridData = await _dieuTriNoiTruService.GetTotalPageForGridNhomDichVuTheoThoiGianNhapVienAsync(queryInfo);
            }
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridDichVuTheoThoiGianNhapVien")]
        public async Task<ActionResult<GridDataSource>> GetDataForGridDichVuTheoThoiGianNhapVienAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = new GridDataSource();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                gridData = await _dieuTriNoiTruService.GetDataForGridDichVuTheoThoiGianNhapVienAsync(queryInfo);
            }
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridDichVuTheoThoiGianNhapVien")]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridDichVuTheoThoiGianNhapVienAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = new GridDataSource();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                gridData = await _dieuTriNoiTruService.GetTotalPageForGridDichVuTheoThoiGianNhapVienAsync(queryInfo);
            }
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridNhomDichVuCLSTheoThoiGianNhapVien")]
        public async Task<ActionResult<GridDataSource>> GetDataForGridNhomDichVuCLSTheoThoiGianNhapVienAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = new GridDataSource();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                gridData = await _dieuTriNoiTruService.GetDataForGridNhomDichVuCLSTheoThoiGianNhapVienAsync(queryInfo);
            }
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridNhomDichVuCLSTheoThoiGianNhapVien")]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridNhomDichVuCLSTheoThoiGianNhapVienAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = new GridDataSource();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                gridData = await _dieuTriNoiTruService.GetTotalPageForGridNhomDichVuCLSTheoThoiGianNhapVienAsync(queryInfo);
            }
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridKetQuaCDHATheoThoiGianNhapVien")]
        public async Task<ActionResult<GridDataSource>> GetDataForGridKetQuaCDHATheoThoiGianNhapVienAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = new GridDataSource();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                gridData = await _dieuTriNoiTruService.GetDataForGridKetQuaCDHATheoThoiGianNhapVienAsync(queryInfo);
            }
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridKetQuaCDHATheoThoiGianNhapVien")]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridKetQuaCDHATheoThoiGianNhapVienAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = new GridDataSource();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                gridData = await _dieuTriNoiTruService.GetTotalPageForGridKetQuaCDHATheoThoiGianNhapVienAsync(queryInfo);
            }
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridKetQuaXetNghiemTheoThoiGianNhapVien")]
        public async Task<ActionResult<GridDataSource>> GetDataForGridKetQuaXetNghiemTheoThoiGianNhapVienAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = new GridDataSource();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                gridData = await _dieuTriNoiTruService.GetDataForGridKetQuaXetNghiemTheoThoiGianNhapVienAsync(queryInfo);
            }
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridKetQuaXetNghiemTheoThoiGianNhapVien")]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridKetQuaXetNghiemTheoThoiGianNhapVienAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = new GridDataSource();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                gridData = await _dieuTriNoiTruService.GetTotalPageForGridKetQuaXetNghiemTheoThoiGianNhapVienAsync(queryInfo);
            }
            return Ok(gridData);
        }
        #endregion

        #region tab dv khám
        [HttpPut("XoaDichVuKhamBenh")]
        [ClaimRequirement(SecurityOperation.Delete, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> XoaDichVuKhamBenh(ChiDinhNgoaiTruCanXoaViewModel xoaDichVuViewModel)
        {
            var entity = await _yeuCauKhamBenhService.GetByIdAsync(xoaDichVuViewModel.DichVuId, x => x.Include(y => y.YeuCauKhamBenhTruoc));
            if (entity.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham && entity.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
            {
                throw new ApiException(_localizationService.GetResource("ChiDinh.DichVuKham.DaThucHien"));
            }
            ////todo: cần cân nhắc lại điều kiện chỗ này
            //else if (entity.YeuCauKhamBenhTruoc != null)
            //{
            //    if (entity.YeuCauKhamBenhTruoc.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
            //    {
            //        throw new ApiException(_localizationService.GetResource("KhamBenh.YeuCauKhamBenh.DaHuy"));
            //    }
            //    else if (entity.YeuCauKhamBenhTruoc.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
            //    {
            //        throw new ApiException(_localizationService.GetResource("KhamBenh.YeuCauKhamBenh.DaHoanThanhKham"));
            //    }
            //}

            //var yeuCauTiepNhanChiTiet = await _khamBenhService.GetYeuCauTiepNhanByIdAsync(entity.YeuCauTiepNhanId);
            var yeuCauTiepNhanChiTiet = await _khamBenhService.GetYeuCauTiepNhanKhiXoaDichVuNgoaiTruByIdAsync(entity.YeuCauTiepNhanId);
            //if (yeuCauTiepNhanChiTiet.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy ||
            //    yeuCauTiepNhanChiTiet.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DaHoanTat)
            //{
            //    throw new ApiException(_localizationService.GetResource("KhamBenh.YeuCauTiepNhan.DaHoanTat"));
            //}
            foreach (var item in yeuCauTiepNhanChiTiet.YeuCauKhamBenhs)
            {
                if (item.Id == xoaDichVuViewModel.DichVuId)
                {
                    if (yeuCauTiepNhanChiTiet.YeuCauDuocPhamBenhViens.Any(x => x.YeuCauKhamBenhId == item.Id && x.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy)
                        || yeuCauTiepNhanChiTiet.YeuCauVatTuBenhViens.Any(x => x.YeuCauKhamBenhId == item.Id && x.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy))
                    {
                        throw new ApiException(_localizationService.GetResource("ChiDinh.HuyDichDichVu.DaCoGhiNhanVTTHThuoc"));
                    }

                    item.WillDelete = true;
                    if (!string.IsNullOrEmpty(xoaDichVuViewModel.LyDoHuyDichVu))
                    {
                        if (item.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                        {
                            throw new ApiException(_localizationService.GetResource("ChiDinh.HuyDichDichVu.DaThanhToan"));
                        }
                        item.NhanVienHuyDichVuId = _userAgentHelper.GetCurrentUserId();
                        item.LyDoHuyDichVu = xoaDichVuViewModel.LyDoHuyDichVu;
                    }
                    else
                    {
                        if (item.TrangThaiThanhToan != Enums.TrangThaiThanhToan.DaThanhToan && item.TaiKhoanBenhNhanChis.Any())
                        {
                            throw new ApiException(_localizationService.GetResource("ChiDinh.XoaDichDichVu.DaHuyThanhToan"));
                        }
                    }

                    //BVHD-3825
                    if (item.MienGiamChiPhis.Any())
                    {
                        foreach (var mienGiam in item.MienGiamChiPhis.Where(x => x.DaHuy != true && x.YeuCauGoiDichVuId != null))
                        {
                            mienGiam.DaHuy = true;
                            item.SoTienMienGiam = item.SoTienMienGiam.GetValueOrDefault() == 0 ? item.SoTienMienGiam : (item.SoTienMienGiam - mienGiam.SoTien);
                        }
                    }

                    if (item.LaChiDinhTuNoiTru != null && item.LaChiDinhTuNoiTru == true)
                    {
                        // trường hợp dịch vụ nội trú
                        await _tiepNhanBenhNhanService.XuLyXoaYLenhVaCapNhatDienBienKhiXoaDichVuAsync(EnumNhomGoiDichVu.DichVuKhamBenh, item.Id);
                    }
                    break;
                }
            }

            await _dieuTriNoiTruService.PrepareForDeleteDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);
            return Ok();
        }


        #endregion

        #region tab dv kỹ thuật
        [HttpPut("XoaDichVuKyThuat")]
        [ClaimRequirement(SecurityOperation.Delete, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<ChiDinhDichVuResultVo>> XoaDichVuKyThuat(ChiDinhNgoaiTruCanXoaViewModel xoaDichVuViewModel)
        {
            var entity = await _yeuCauDichVuKyThuatService.GetByIdAsync(xoaDichVuViewModel.DichVuId);
            if (entity != null && (entity.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien || entity.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien))
            {
                throw new ApiException(_localizationService.GetResource("KhamBenhChiDinh.DichVuKyThuat.DaThucHien"));
            }

            //var yeuCauTiepNhanChiTiet = await _khamBenhService.GetYeuCauTiepNhanByIdAsync(entity.YeuCauTiepNhanId);
            var yeuCauTiepNhanChiTiet = await _khamBenhService.GetYeuCauTiepNhanKhiXoaDichVuNgoaiTruByIdAsync(entity.YeuCauTiepNhanId);
            foreach (var item in yeuCauTiepNhanChiTiet.YeuCauDichVuKyThuats)
            {
                if (item.Id == xoaDichVuViewModel.DichVuId)
                {
                    if (yeuCauTiepNhanChiTiet.YeuCauDuocPhamBenhViens.Any(x => x.YeuCauDichVuKyThuatId == item.Id && x.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy)
                        || yeuCauTiepNhanChiTiet.YeuCauVatTuBenhViens.Any(x => x.YeuCauDichVuKyThuatId == item.Id && x.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy))
                    {
                        throw new ApiException(_localizationService.GetResource("ChiDinh.HuyDichDichVu.DaCoGhiNhanVTTHThuoc"));
                    }
                    item.WillDelete = true;
                    if (!string.IsNullOrEmpty(xoaDichVuViewModel.LyDoHuyDichVu))
                    {
                        if (item.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                        {
                            throw new ApiException(_localizationService.GetResource("ChiDinh.HuyDichDichVu.DaThanhToan"));
                        }
                        item.NhanVienHuyDichVuId = _userAgentHelper.GetCurrentUserId();
                        item.LyDoHuyDichVu = xoaDichVuViewModel.LyDoHuyDichVu;
                    }
                    else
                    {
                        if (item.TrangThaiThanhToan != Enums.TrangThaiThanhToan.DaThanhToan && item.TaiKhoanBenhNhanChis.Any())
                        {
                            throw new ApiException(_localizationService.GetResource("ChiDinh.XoaDichDichVu.DaHuyThanhToan"));
                        }
                    }

                    //BVHD-3825
                    var mienGiam = item.MienGiamChiPhis.FirstOrDefault(x => x.DaHuy != true && x.YeuCauGoiDichVuId != null && (x.TaiKhoanBenhNhanThuId == null || x.TaiKhoanBenhNhanThu.DaHuy != true));
                    if (mienGiam != null)
                    {
                        mienGiam.DaHuy = true;
                        mienGiam.WillDelete = true;

                        var giamSoTienMienGiam = item.SoTienMienGiam.GetValueOrDefault() - mienGiam.SoTien;
                        if (giamSoTienMienGiam < 0)
                        {
                            giamSoTienMienGiam = 0;
                        }
                        item.SoTienMienGiam = giamSoTienMienGiam;
                    }
                    break;
                }
            }

            await _dieuTriNoiTruService.PrepareForDeleteDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);
            return Ok();
        }

        [HttpGet("GetYeuCauDichVuKyThuatById")]
        public async Task<ActionResult<TrangThaiThucHienYeuCauDichVuKyThuatViewModel>> GetYeuCauDichVuKyThuatByIdAsync(long yeuCauDichVuKyThuatId)
        {
            var yeuCauDichVuKyThuat = _yeuCauDichVuKyThuatService.GetById(yeuCauDichVuKyThuatId);
            var result = yeuCauDichVuKyThuat.ToModel<TrangThaiThucHienYeuCauDichVuKyThuatViewModel>();

            result.NhanVienThucHienId = result.NhanVienThucHienId == null ? _userAgentHelper.GetCurrentUserId() : result.NhanVienThucHienId;
            result.ThoiDiemThucHien = result.ThoiDiemThucHien == null ? DateTime.Now : result.ThoiDiemThucHien;

            return result;
        }

        [HttpPut("XuLyCapNhatThucHienYeuCauDichVuKyThuat")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> XuLyCapNhatThucHienYeuCauDichVuKyThuatAsync([FromBody] TrangThaiThucHienYeuCauDichVuKyThuatViewModel viewModel)
        {
            var yeuCauDichVuKyThuat = _yeuCauDichVuKyThuatService.GetById(viewModel.Id);
            if (yeuCauDichVuKyThuat.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
            {
                throw new ApiException(_localizationService.GetResource("CapNhatThucHienDichVuKyThuat.TrangThaiYeuCauDichVuKyThuat.DaHuy"));
            }
            viewModel.ToEntity(yeuCauDichVuKyThuat);
            if (viewModel.LaThucHienDichVu)
            {
                if (yeuCauDichVuKyThuat.TrangThaiThanhToan != Enums.TrangThaiThanhToan.BaoLanhThanhToan
                    && yeuCauDichVuKyThuat.TrangThaiThanhToan != Enums.TrangThaiThanhToan.DaThanhToan
                    && yeuCauDichVuKyThuat.TrangThaiThanhToan != TrangThaiThanhToan.CapNhatThanhToan)
                {
                    throw new ApiException(_localizationService.GetResource("CapNhatThucHienDichVuKyThuat.TrangThaiThanhToanYeuCauDichVuKyThuat.ChuaThanhToan"));
                }

                //yeuCauDichVuKyThuat.LyDoHuyTrangThaiDaThucHien = null;
                //yeuCauDichVuKyThuat.NhanVienHuyTrangThaiDaThucHienId = null;
                yeuCauDichVuKyThuat.TrangThai = Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien;
            }
            else
            {
                yeuCauDichVuKyThuat.NhanVienHuyTrangThaiDaThucHienId = _userAgentHelper.GetCurrentUserId();
                yeuCauDichVuKyThuat.NhanVienThucHienId = yeuCauDichVuKyThuat.NhanVienKetLuanId = null;
                yeuCauDichVuKyThuat.ThoiDiemThucHien = yeuCauDichVuKyThuat.ThoiDiemHoanThanh = null;
                yeuCauDichVuKyThuat.TrangThai = Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien;
            }

            _yeuCauDichVuKyThuatService.Update(yeuCauDichVuKyThuat);
            return Ok();
        }
        #endregion

        #region tab thuốc/vật tư
        [HttpPut("XuLyXoaYeuCauGhiNhanVTTHThuoc")]
        [ClaimRequirement(SecurityOperation.Delete, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<ChiDinhDichVuResultVo>> XuLyXoaYeuCauGhiNhanVTTHThuocAsync(GhiNhatThuocVatTuNgoaiTruCanXoaViewModel xoaYeuCauViewModel)
        {
            //var yeuCauTiepNhanChiTiet = await _khamBenhService.GetYeuCauTiepNhanByIdAsync(xoaYeuCauViewModel.YeuCauTiepNhanId);
            var yeuCauTiepNhanChiTiet = await _khamBenhService.GetYeuCauTiepNhanKhiXoaVTTHThuocNgoaiTruByIdAsync(xoaYeuCauViewModel.YeuCauTiepNhanId);

            // xử lý xóa yeu cau duoc pham/ vat tu
            await _khamBenhService.XuLyXoaYeuCauGhiNhanVTTHThuocAsync(yeuCauTiepNhanChiTiet, xoaYeuCauViewModel.YeuCauGhiNhanId);
            await _dieuTriNoiTruService.PrepareForDeleteDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);

            return Ok();
        }


        #endregion
    }
}
