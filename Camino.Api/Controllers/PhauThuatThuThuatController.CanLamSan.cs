using Camino.Api.Auth;
using Camino.Api.Models.Error;
using Camino.Api.Models.KhamBenh;
using Camino.Api.Models.PhauThuatThuThuat;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DichVuKyThuat;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Core.Domain.ValueObject.PhauThuatThuThuat;
using Camino.Services.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.ValueObject.Grid;
using Newtonsoft.Json;
using static Camino.Core.Domain.Enums;
using Camino.Core.Domain.ValueObject.PhieuInXetNghiem;
using Microsoft.EntityFrameworkCore;
using Camino.Api.Models.DieuTriNoiTru;
using System;
using System.Globalization;
using Camino.Core.Helpers;

namespace Camino.Api.Controllers
{
    public partial class PhauThuatThuThuatController
    {
        [HttpPost("GetListNhomDichVuCLSPTTT")]
        public async Task<ActionResult<ICollection<NhomDichVuBenhVienTreeViewVo>>> GetListNhomDichVuCLSPTTT([FromBody] DropDownListRequestModel model)
        {
            var lookup = await _phauThuatThuThuatService.GetListNhomDichVuCLSPTTT(model);
            return Ok(lookup);
        }

        [HttpPost("ThemYeuCauDichVuKyThuatMultiSelect")]
        [ClaimRequirement(SecurityOperation.Add, DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult<ChiDinhDichVuResultVo>> ThemYeuCauDichVuKyThuatMultiselectAsync([FromBody] PhauThuatThuThuatChiDinhDichVuKyThuatMultiSelectViewModel yeuCauViewModel)
        {
            // kiểm tra yêu cầu khám bệnh trước khi thêm
            //await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhAsync(yeuCauViewModel.YeuCauKhamBenhId);

            var yeuCauVo = yeuCauViewModel.Map<ChiDinhDichVuKyThuatMultiselectVo>();


            // get thông tin yêu cầu tiếp nhận hiện tại
            //var yeuCauTiepNhanChiTiet = await _khamBenhService.GetYeuCauTiepNhanByIdAsync(yeuCauViewModel.YeuCauTiepNhanId);
            //var yeuCauTiepNhanChiTiet = await _yeuCauTiepNhanService.GetByIdAsync(yeuCauViewModel.YeuCauTiepNhanId, o => o.Include(p => p.NoiTruBenhAn));
            var yeuCauTiepNhanChiTiet = await _phauThuatThuThuatService.GetYeuCauTiepNhanKhiThemDichVuNgoaiTruByIdAsync(yeuCauViewModel.YeuCauTiepNhanId);

            // kiểm tra nếu là dịch vụ thêm từ gói
            if (yeuCauViewModel.DichVuKyThuatTuGois != null && yeuCauViewModel.DichVuKyThuatTuGois.Any())
            {
                var dichVuChiDinhTheoGoiVo = new ChiDinhGoiDichVuTheoBenhNhanVo()
                {
                    YeuCauTiepNhanId = yeuCauViewModel.YeuCauTiepNhanId,
                    ISPTTT = true
                };
                dichVuChiDinhTheoGoiVo.DichVus.AddRange(yeuCauViewModel.DichVuKyThuatTuGois
                    .Select(item => new ChiTietGoiDichVuChiDinhTheoBenhNhanVo()
                    {
                        Id = item.Id,
                        YeuCauGoiDichVuId = item.YeuCauGoiDichVuId ?? 0,
                        TenDichVu = item.TenDichVu,
                        ChuongTrinhGoiDichVuId = item.ChuongTrinhGoiDichVuId ?? 0,
                        ChuongTrinhGoiDichVuChiTietId = item.ChuongTrinhGoiDichVuChiTietId ?? 0,
                        DichVuBenhVienId = item.DichVuBenhVienId ?? 0,
                        NhomGoiDichVu = (int)Enums.EnumNhomGoiDichVu.DichVuKyThuat,
                        SoLuongSuDung = 1
                    }));

                var lstDichVuKyThuatDangChon = yeuCauViewModel.DichVuKyThuatBenhVienChiDinhs.Select(x => x).ToList();
                foreach (var strId in lstDichVuKyThuatDangChon)
                {
                    var itemObj = JsonConvert.DeserializeObject<ItemChiDinhDichVuKyThuatVo>(strId);
                    if (dichVuChiDinhTheoGoiVo.DichVus.Any(x => x.DichVuBenhVienId == itemObj.DichVuId))
                    {
                        var dichVuLoaiBo = yeuCauViewModel.DichVuKyThuatBenhVienChiDinhs.FirstOrDefault(x => x == strId);
                        yeuCauVo.DichVuKyThuatBenhVienChiDinhs.Remove(dichVuLoaiBo);
                    }
                }

                await _khamBenhService.XuLyThemChiDinhGoiDichVuTheoBenhNhanAsync(yeuCauTiepNhanChiTiet, dichVuChiDinhTheoGoiVo);
            }

            if (yeuCauVo.DichVuKyThuatBenhVienChiDinhs.Any())
            {
                await _phauThuatThuThuatService.XuLyThemYeuCauDichVuKyThuatMultiselectAsync(yeuCauVo, yeuCauTiepNhanChiTiet);
            }

            await _tiepNhanBenhNhanService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);

            // kiểm tra chỉ định dich vụ vượt quá số dư tài khoản
            var chiDinhDichVuResultVo = new ChiDinhDichVuResultVo();
            //chiDinhDichVuResultVo.SoDuTaiKhoan = await _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhanChiTiet.Id);
            //chiDinhDichVuResultVo.SoDuTaiKhoanConLai = await _taiKhoanBenhNhanService.GetSoTienUocLuongConLai(yeuCauTiepNhanChiTiet.Id);

            decimal soDuTk = 0;
            decimal soDuUocLuongConLai = 0;

            if (yeuCauTiepNhanChiTiet.NoiTruBenhAn != null || yeuCauTiepNhanChiTiet.QuyetToanTheoNoiTru == true)
            {
                //var chiPhiKhamChuaBenh = _thuNganNoiTruService.GetDanhSachChiPhiKhamChuaBenhChuaThu(yeuCauTiepNhanChiTiet.Id).Result.Select(o => o.BNConPhaiThanhToan).DefaultIfEmpty(0).Sum();
                var chiPhiKhamChuaBenh = _thuNganNoiTruService.GetSoTienBNConPhaiThanhToan(yeuCauTiepNhanChiTiet.Id).Result;

                soDuTk = await _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhanChiTiet.Id);
                soDuUocLuongConLai = soDuTk - chiPhiKhamChuaBenh;
            }
            else
            {
                soDuTk = await _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhanChiTiet.Id);
                soDuUocLuongConLai = await _taiKhoanBenhNhanService.GetSoTienUocLuongConLai(yeuCauTiepNhanChiTiet.Id);
            }

            chiDinhDichVuResultVo.SoDuTaiKhoan = soDuTk;
            chiDinhDichVuResultVo.SoDuTaiKhoanConLai = soDuUocLuongConLai;

            if (yeuCauVo.DichVuKyThuatBenhVienChiDinhs.Any())
            {
                var yeuCauVuaThem = yeuCauTiepNhanChiTiet.YeuCauDichVuKyThuats.Where(x => x.YeuCauTiepNhanId == yeuCauViewModel.YeuCauTiepNhanId &&
                                                                                          x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy &&
                                                                                          (yeuCauVo.YeuCauDichVuKyThuatCuoiCungId == 0 || (x.Id > yeuCauVo.YeuCauDichVuKyThuatCuoiCungId)))
                                                                              .ToList();

                chiDinhDichVuResultVo.IsVuotQuaSoDuTaiKhoan = yeuCauVuaThem.Any(x => x.TrangThaiThanhToan != TrangThaiThanhToan.BaoLanhThanhToan);
            }

            // nếu loại dvkt vừa thêm thuộc 4 nhóm này, thì chuyển yêu cầu khám bệnh hiện tại sang hàng chờ làm chỉ định
            //if (yeuCauVo.ChuyenHangDoiSangLamChiDinh)
            //{
            //    chiDinhDichVuResultVo.ChuyenHangDoiSangLamChiDinh = true;
            //    await _khamBenhService.CapNhatHangChoKhiChiDinhDichVuKyThuatAsync(yeuCauVo.YeuCauTiepNhanId, yeuCauVo.YeuCauKhamBenhId, _userAgentHelper.GetCurrentNoiLLamViecId());
            //}
            #region BVHD-3761
            if (!string.IsNullOrEmpty(yeuCauViewModel.BieuHienLamSang) || !string.IsNullOrEmpty(yeuCauViewModel.DichTeSarsCoV2))
            {
                _khamBenhService.UpdateDichVuKyThuatSarsCoVTheoYeuCauTiepNhan(yeuCauViewModel.YeuCauTiepNhanId, yeuCauViewModel.BieuHienLamSang, yeuCauViewModel.DichTeSarsCoV2);
            }
            #endregion BVHD-3761

            return chiDinhDichVuResultVo;
        }

        [HttpGet("GetDichVuKyThuatChuaHoanThanhTrenHoanThanh")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<DichVuKyThuatChuaHoanThanhTrenHoanThanh> GetDichVuKyThuatChuaHoanThanhTrenHoanThanh(long yeuCauTiepNhanId)
        {
            DichVuKyThuatChuaHoanThanhTrenHoanThanh dichVuKyThuatChuaHoanThanhTrenHoanThanh = new DichVuKyThuatChuaHoanThanhTrenHoanThanh();
            //dichVuKyThuatChuaHoanThanhTrenHoanThanh.DichVuKyThuatDaHoanThanh = await _phauThuatThuThuatService.GetSoLuongDichVuKyThuatDaHoanThanh(yeuCauTiepNhanId);
            //dichVuKyThuatChuaHoanThanhTrenHoanThanh.TongDichVuKyThuat = await _phauThuatThuThuatService.GetSoLuongDichVuKyThuat(yeuCauTiepNhanId);

            //Cập nhật 12/12/2022
            var result = await _phauThuatThuThuatService.GetTienTrinhHoanThanhDichVuKyThuat(yeuCauTiepNhanId);
            dichVuKyThuatChuaHoanThanhTrenHoanThanh.DichVuKyThuatDaHoanThanh = result.Item1;
            dichVuKyThuatChuaHoanThanhTrenHoanThanh.TongDichVuKyThuat = result.Item2;

            return dichVuKyThuatChuaHoanThanhTrenHoanThanh;
        }

        [HttpGet("GetDichVuKyThuatChuaHoanThanhTrenHoanThanhCanLamSang")]     
        public async Task<DichVuKyThuatChuaHoanThanhTrenHoanThanh> GetDichVuKyThuatChuaHoanThanhTrenHoanThanhCanLamSang(long yeuCauTiepNhanId)
        {
            DichVuKyThuatChuaHoanThanhTrenHoanThanh dichVuKyThuatChuaHoanThanhTrenHoanThanh = new DichVuKyThuatChuaHoanThanhTrenHoanThanh();
            dichVuKyThuatChuaHoanThanhTrenHoanThanh.DichVuKyThuatDaHoanThanh = await _phauThuatThuThuatService.GetSoLuongDichVuKyThuatDaHoanThanhCanLamSang(yeuCauTiepNhanId);
            dichVuKyThuatChuaHoanThanhTrenHoanThanh.TongDichVuKyThuat = await _phauThuatThuThuatService.GetSoLuongDichVuKyThuatCanLamSang(yeuCauTiepNhanId);

            return dichVuKyThuatChuaHoanThanhTrenHoanThanh;
        }

        [HttpGet("GetDataDefaultDichVuKyThuat")]
        //[ClaimRequirement(SecurityOperation.View, DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<List<PhauThuatThuThuatCanLamSanGridVo>> GetDataDefaultDichVuKyThuat(long yeuCauTiepNhanId, long? phieuDieuTriId)
        {
            //var gridData = await _phauThuatThuThuatService.GetDichVuKyThuatsByYeuCauTiepNhan(yeuCauTiepNhanId, phieuDieuTriId);
            var gridData = await _phauThuatThuThuatService.GetDichVuKyThuatsByYeuCauTiepNhanVer2(yeuCauTiepNhanId, phieuDieuTriId);
            return gridData;
        }

        [HttpPost("ApDungThoiGianDienBienChoYeuCauDichVus")]
        public async Task<ActionResult> ApDungThoiGianDienBienChoYeuCauDichVus([FromBody] ApDungThoiGianDienBienVo apDungThoiGianDienBienVo)
        {
            var yeuCauKhamBenhIds = apDungThoiGianDienBienVo.DataGridDichVuChons.Where(o => o.ChecBoxItem && o.NhomId == (int)EnumNhomGoiDichVu.DichVuKhamBenh).Select(o => o.Id).ToList();
            var yeuCauDichVuKyThuatIds = apDungThoiGianDienBienVo.DataGridDichVuChons.Where(o => o.ChecBoxItem && o.NhomId == (int)EnumNhomGoiDichVu.DichVuKyThuat).Select(o => o.Id).ToList();
            if (!yeuCauKhamBenhIds.Any() && !yeuCauDichVuKyThuatIds.Any())
            {
                throw new ApiException("Vui lòng chọn dịch vụ để áp dụng");
            }
            _phauThuatThuThuatService.ApDungThoiGianDienBienDichVuKhamVaKyThuat(yeuCauKhamBenhIds, yeuCauDichVuKyThuatIds, apDungThoiGianDienBienVo.ThoiGianDienBien);
            return Ok(true);
        }

        [HttpPost("CapNhatGridItemDichVuKyThuat")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult> CapNhatGridItemDichVuKyThuatAsync(GridItemYeuCauDichVuKyThuatViewModel viewModel)
        {
            //await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhAsync(viewModel.YeuCauKhamBenhId);

            //var yeuCauTiepNhanChiTiet = await _khamBenhService.GetYeuCauTiepNhanByIdAsync(viewModel.YeuCauTiepNhanId);
            var yeuCauTiepNhanChiTiet = await _khamBenhService.GetYeuCauTiepNhanKhiSuaXoaDVKyThuatNgoaiTruByIdAsync(viewModel.YeuCauTiepNhanId);
            //var yeuCauKhamBenhDangKham = yeuCauTiepNhanChiTiet.YeuCauKhamBenhs.FirstOrDefault(x => x.Id == viewModel.YeuCauKhamBenhId);

            var flagUpdate = false;
            foreach (var item in yeuCauTiepNhanChiTiet.YeuCauDichVuKyThuats)
            {
                if (item.Id == viewModel.YeuCauDichVuKyThuatId)
                {
                    flagUpdate = true;

                    long? yeuCauGoiDichVuKhuyenMaiId = null;
                    if (viewModel.IsUpdateLoaiGia)
                    {
                        if (viewModel.NhomGiaDichVuKyThuatBenhVienId != null && item.NhomGiaDichVuKyThuatBenhVienId != viewModel.NhomGiaDichVuKyThuatBenhVienId)
                        {
                            if (item.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                            {
                                throw new ApiException(_localizationService.GetResource("ChiDinh.LoaiGia.DaThanhToan"));
                            }

                            item.NhomGiaDichVuKyThuatBenhVienId = viewModel.NhomGiaDichVuKyThuatBenhVienId.Value;
                            var donGiaBenhVien = await _khamBenhService.GetDonGiaBenhVienDichVuKyThuatAsync(item.DichVuKyThuatBenhVienId, viewModel.NhomGiaDichVuKyThuatBenhVienId.Value);
                            if (donGiaBenhVien == 0)
                            {
                                throw new ApiException(_localizationService.GetResource("ChiDinh.LoaiGia.NotExists"));
                            }

                            //BVHD-3825
                            // kiểm tra nếu là dịch vụ khuyến mãi từ gói marketing
                            if (item.MienGiamChiPhis.Any(x => x.DaHuy != true && x.YeuCauGoiDichVuId != null && (x.TaiKhoanBenhNhanThuId == null || x.TaiKhoanBenhNhanThu.DaHuy != true)))
                            {
                                viewModel.IsSwapDichVuKhuyenMai = true;
                                viewModel.LaDichVuKhuyenMai = true;
                                yeuCauGoiDichVuKhuyenMaiId = item.MienGiamChiPhis
                                    .First(x => x.DaHuy != true && x.YeuCauGoiDichVuId != null && (x.TaiKhoanBenhNhanThuId == null || x.TaiKhoanBenhNhanThu.DaHuy != true))
                                    .YeuCauGoiDichVuId.Value;
                            }

                            item.Gia = donGiaBenhVien;
                        }
                    }

                    if (viewModel.IsUpdateSoLan)
                    {
                        if (viewModel.SoLan == null || viewModel.SoLan == 0)
                        {
                            throw new ApiException(_localizationService.GetResource("DichVuKyThuat.SoLan.Range"));
                        }
                        // kiểm tra nếu là dịch vụ chỉ đinh từ gói marketing
                        if (item.YeuCauGoiDichVuId != null)
                        {
                            var soLuongConLai = await _khamBenhService.GetSoLuongConLaiDichVuKyThuatTrongGoiMarketingBenhNhanAsync(item.YeuCauGoiDichVuId.Value, item.DichVuKyThuatBenhVienId);
                            var soLuongKhaDung = soLuongConLai + item.SoLan;
                            if (soLuongKhaDung < viewModel.SoLan)
                            {
                                throw new ApiException(string.Format(_localizationService.GetResource("DichVuKyThuat.SoLanConLaiTrongGoi.Range"), item.TenDichVu, soLuongKhaDung));
                            }
                        }

                        // kiểm tra nếu là dịch vụ khuyến mãi từ gói marketing
                        if (item.MienGiamChiPhis.Any(x => x.DaHuy != true && x.YeuCauGoiDichVuId != null && (x.TaiKhoanBenhNhanThuId == null || x.TaiKhoanBenhNhanThu.DaHuy != true)))
                        {
                            var yeuCauGoiId = item.MienGiamChiPhis
                                .First(x => x.DaHuy != true && x.YeuCauGoiDichVuId != null && (x.TaiKhoanBenhNhanThuId == null || x.TaiKhoanBenhNhanThu.DaHuy != true))
                                .YeuCauGoiDichVuId.Value;
                            var soLuongConLai = await _khamBenhService.GetSoLuongConLaiDichVuKyThuatKhuyenMaiTrongGoiMarketingBenhNhanAsync(yeuCauGoiId, item.DichVuKyThuatBenhVienId);
                            var soLuongKhaDung = soLuongConLai + item.SoLan;
                            if (soLuongKhaDung < viewModel.SoLan)
                            {
                                throw new ApiException(string.Format(_localizationService.GetResource("DichVuKyThuat.SoLanConLaiTrongGoi.Range"), item.TenDichVu, soLuongKhaDung));
                            }

                            viewModel.IsSwapDichVuKhuyenMai = true;
                            viewModel.LaDichVuKhuyenMai = true;
                            yeuCauGoiDichVuKhuyenMaiId = yeuCauGoiId;
                        }
                        item.SoLan = viewModel.SoLan != null ? viewModel.SoLan.Value : item.SoLan;
                    }

                    if (viewModel.IsUpdateNoiThucHien)
                    {
                        if (viewModel.NoiThucHienId == null || viewModel.NoiThucHienId == 0)
                        {
                            throw new ApiException(_localizationService.GetResource("ycdvcls.NoiThucHienId.Required"));
                        }

                        var queryInfo = new DropDownListRequestModel()
                        {
                            //Id = viewModel.NoiThucHienId.Value,
                            ParameterDependencies = "{NoiThucHienId:" + viewModel.NoiThucHienId.Value + "}",
                            Take = 50
                        };

                        var lstBacSiKhams = await _yeuCauKhamBenhService.GetBacSiKhams(queryInfo);
                        //if (!lstBacSiKhams.Any())
                        //{
                        //    throw new ApiException(_localizationService.GetResource("ycdvcls.NoiThucHienId.BaSiNotExists"));
                        //}

                        item.NoiThucHienId = viewModel.NoiThucHienId;

                        // nếu nơi thực hiện mới ko có bác sĩ đang chọn, thì tự dộng chọn bác sĩ đầu tiên
                        if (lstBacSiKhams.Any() && !lstBacSiKhams.Any(x => x.KeyId == viewModel.NguoiThucHienId))
                        {
                            item.NhanVienThucHienId = lstBacSiKhams.First().KeyId;
                        }
                        else
                        {
                            item.NhanVienThucHienId = null;
                        }
                    }

                    if (viewModel.IsUpdateNguoiThucHien)
                    {
                        //if (viewModel.NguoiThucHienId == null || viewModel.NguoiThucHienId == 0)
                        //{
                        //    throw new ApiException(_localizationService.GetResource("ycdvcls.NhanVienThucHienId.Required"));
                        //}
                        item.NhanVienThucHienId = viewModel.NguoiThucHienId;
                    }
                    //item.NoiThucHienId = viewModel.NoiThucHienId != null ? viewModel.NoiThucHienId.Value : item.NoiThucHienId;

                    if (viewModel.IsUpdateBenhPhamXetNghiem)
                    {
                        item.BenhPhamXetNghiem = viewModel.BenhPhamXetNghiem;
                    }

                    if (viewModel.IsSwapDichVuGoi)
                    {
                        if (viewModel.LaDichVuTrongGoi == true)
                        {
                            var thongTin = new ThongTinDichVuTrongGoi()
                            {
                                BenhNhanId = yeuCauTiepNhanChiTiet.BenhNhanId.Value,
                                DichVuId = item.DichVuKyThuatBenhVienId,
                                NhomGoiDichVu = EnumNhomGoiDichVu.DichVuKyThuat,
                                SoLuong = item.SoLan
                            };
                            await _khamBenhService.GetYeuCauGoiDichVuTheoDichVuChiDinhAsync(thongTin);
                            item.Gia = thongTin.DonGia;
                            item.DonGiaTruocChietKhau = thongTin.DonGiaTruocChietKhau;
                            item.DonGiaSauChietKhau = thongTin.DonGiaSauChietKhau;
                            item.YeuCauGoiDichVuId = thongTin.YeuCauGoiDichVuId;

                            if (item.MienGiamChiPhis.Any(x => x.DaHuy != true))
                            {
                                item.SoTienBaoHiemTuNhanChiTra = null;
                                item.SoTienMienGiam = null;
                                foreach (var mienGiam in item.MienGiamChiPhis.Where(a => a.DaHuy != true))
                                {
                                    mienGiam.WillDelete = true;
                                }

                                foreach (var congNo in item.CongTyBaoHiemTuNhanCongNos.Where(a => a.DaHuy != true))
                                {
                                    congNo.WillDelete = true;
                                }
                            }
                        }
                        else
                        {
                            item.DonGiaTruocChietKhau = null;
                            item.DonGiaSauChietKhau = null;
                            item.YeuCauGoiDichVuId = null;

                            var donGiaBenhVien = await _khamBenhService.GetDonGiaBenhVienDichVuKyThuatAsync(item.DichVuKyThuatBenhVienId, item.NhomGiaDichVuKyThuatBenhVienId);
                            if (donGiaBenhVien == 0)
                            {
                                throw new ApiException(_localizationService.GetResource("ChiDinh.LoaiGia.NotExists"));
                            }

                            item.Gia = donGiaBenhVien;
                        }
                    }

                    //BVHD-3825
                    if (viewModel.IsSwapDichVuKhuyenMai)
                    {
                        if (item.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                        {
                            throw new ApiException(_localizationService.GetResource("DichVuKhuyenMai.TrangThaiYeuCauDichVu.DaThanhToan"));
                        }

                        if (viewModel.LaDichVuKhuyenMai == true)
                        {
                            var thongTin = new ThongTinDichVuTrongGoi()
                            {
                                BenhNhanId = yeuCauTiepNhanChiTiet.BenhNhanId.Value,
                                DichVuId = item.DichVuKyThuatBenhVienId,
                                NhomGoiDichVu = EnumNhomGoiDichVu.DichVuKyThuat,
                                SoLuong = item.SoLan,
                                NhomGiaId = item.NhomGiaDichVuKyThuatBenhVienId
                            };

                            //dùng cho trường hợp cập nhật số lượng hoặc loại giá
                            if (viewModel.IsUpdateLoaiGia || viewModel.IsUpdateSoLan)
                            {
                                thongTin.YeuCauDichVuCapNhatSoLuongLoaiGiaId = item.Id;
                                thongTin.YeucauGoiDichVuKhuyenMaiId = yeuCauGoiDichVuKhuyenMaiId;
                            }

                            await _tiepNhanBenhNhanService.GetYeuCauGoiDichVuKhuyenMaiTheoDichVuChiDinhAsync(thongTin);

                            if (item.MienGiamChiPhis.Any(x => x.DaHuy != true && (x.TaiKhoanBenhNhanThuId == null || x.TaiKhoanBenhNhanThu.DaHuy != true) && x.YeuCauGoiDichVuId != null))
                            {
                                //item.SoTienBaoHiemTuNhanChiTra = null;
                                //item.SoTienMienGiam = null;
                                foreach (var mienGiam in item.MienGiamChiPhis.Where(a => a.DaHuy != true && (a.TaiKhoanBenhNhanThuId == null || a.TaiKhoanBenhNhanThu.DaHuy != true) && a.YeuCauGoiDichVuId != null))
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

                                //foreach (var congNo in item.CongTyBaoHiemTuNhanCongNos.Where(a => a.DaHuy != true))
                                //{
                                //    congNo.WillDelete = true;
                                //}
                            }

                            //trường hợp cập nhật số lượng hoặc loại giá thì giữ nguyên đơn giá chỉ định trước đó
                            if (!viewModel.IsUpdateLoaiGia && !viewModel.IsUpdateSoLan)
                            {
                                item.Gia = thongTin.DonGia;
                            }

                            var thanhTien = item.SoLan * item.Gia;
                            var thanhTienMienGiam = item.SoLan * thongTin.DonGiaKhuyenMai.Value;

                            var tongTienMienGiam = (thanhTien > thanhTienMienGiam) ? (thanhTien - thanhTienMienGiam) : 0;
                            item.SoTienMienGiam = item.SoTienMienGiam.GetValueOrDefault() + tongTienMienGiam;
                            item.MienGiamChiPhis.Add(new MienGiamChiPhi()
                            {
                                YeuCauTiepNhanId = item.YeuCauTiepNhanId,
                                LoaiMienGiam = Enums.LoaiMienGiam.MienGiamThem,
                                LoaiChietKhau = Enums.LoaiChietKhau.ChietKhauTheoSoTien,
                                SoTien = tongTienMienGiam,
                                YeuCauGoiDichVuId = thongTin.YeuCauGoiDichVuId
                            });
                        }
                        else
                        {
                            var donGiaBenhVien = await _khamBenhService.GetDonGiaBenhVienDichVuKyThuatAsync(item.DichVuKyThuatBenhVienId, item.NhomGiaDichVuKyThuatBenhVienId);
                            if (donGiaBenhVien == 0)
                            {
                                throw new ApiException(_localizationService.GetResource("ChiDinh.LoaiGia.NotExists"));
                            }

                            item.Gia = donGiaBenhVien;
                            var mienGiam = item.MienGiamChiPhis.FirstOrDefault(x => x.DaHuy != true && x.YeuCauGoiDichVuId != null && x.YeuCauGoiDichVuId != null && (x.TaiKhoanBenhNhanThuId == null || x.TaiKhoanBenhNhanThu.DaHuy != true));
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
                        }
                    }
                    break;
                }
            }

            if (!flagUpdate)
            {
                throw new ApiException(_localizationService.GetResource("ApiError.EntityNull"));
            }

            //if (viewModel.IsUpdateNguoiThucHien || viewModel.IsUpdateNoiThucHien)
            //{
            //    await _yeuCauTiepNhanService.UpdateAsync(yeuCauTiepNhanChiTiet);
            //}
            //else
            //{
                await _tiepNhanBenhNhanService.PrepareForEditDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);
            //}
            //return yeuCauKhamBenhDangKham;
            return Ok();
        }

        [HttpPost("InPhieuChiDinh")]
        public ActionResult<string> InPhieuChiDinh([FromBody] InPhieuChiDinhCLSPTTT inPhieuChiDinh)
        {
            var hostingName = inPhieuChiDinh.Hosting;
            var htmlBangThuTienThuoc = _phauThuatThuThuatService.InBaoCaoChiDinh(inPhieuChiDinh.YeuCauTiepNhanId
                , hostingName, inPhieuChiDinh.ListDichVuChiDinh, inPhieuChiDinh.InChungChiDinh, inPhieuChiDinh.KieuInChung
                , inPhieuChiDinh.IsFromPhieuDieuTri, inPhieuChiDinh.PhieuDieuTriId, inPhieuChiDinh.ListChonLoaiPhieuIn);
            return htmlBangThuTienThuoc;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("HuyYeuCauChayLaiXetNghiemTheoNhomDichVu")]
        public ActionResult<bool> HuyYeuCauChayLaiXetNghiemTheoNhomDichVu(long phienXetNghiemId, long nhomDichVuBenhVienId)
        {
            var huyPhienXN = _phauThuatThuThuatService.HuyYeuCauChayLaiXetNghiemTheoNhomDichVu(phienXetNghiemId, nhomDichVuBenhVienId);
            return Ok(huyPhienXN);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("LichSuYeuCauChayLaiXetNghiem")]
        public async Task<ActionResult<List<LichSuYeuCauChayLai>>> LichSuYeuCauChayLaiXetNghiem(LichSuChayLaiXetNghiemViewModel lichSuChayLaiXetNghiem)
        {
            var lichSuChayLaiXetNghiemVo = new LichSuChayLaiXetNghiemVo
            {
                NhomDichVuBenhVienId = lichSuChayLaiXetNghiem.NhomDichVuBenhVienId,
                LichSuPhienXetNghiemIds = lichSuChayLaiXetNghiem.LichSuPhienXetNghiemIds
            };

            var lichSuYeuCauChayLais = await _phauThuatThuThuatService.LichSuYeuCauChayLaiXetNghiem(lichSuChayLaiXetNghiemVo);
            return Ok(lichSuYeuCauChayLais);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("HuyDichVuKyThuat")]
        public async Task<ActionResult<ChiDinhDichVuResultVo>> HuyDichVuKyThuat(PhauThuatThuThuatHuyDichVuThanhToanViewModel phauThuatThuThuatHuyDichVuThanhToanViewModel)
        {
            var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(phauThuatThuThuatHuyDichVuThanhToanViewModel.YeuCauTiepNhanId, o => o.Include(p => p.YeuCauDichVuKyThuats)
                                                                                                                                                .Include(p => p.NoiTruBenhAn)
                                                                                                                                                .Include(x => x.MienGiamChiPhis).ThenInclude(x => x.TaiKhoanBenhNhanThu));

            var yeuCauDichVuKyThuat = yeuCauTiepNhan.YeuCauDichVuKyThuats.Where(p => p.Id == phauThuatThuThuatHuyDichVuThanhToanViewModel.DichVuId)
                                                                         .FirstOrDefault();

            if (yeuCauDichVuKyThuat.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
            {
                throw new ApiException(_localizationService.GetResource("ChiDinh.HuyDichDichVu.DaThanhToan"));
            }

            yeuCauDichVuKyThuat.NhanVienHuyDichVuId = _userAgentHelper.GetCurrentUserId();
            yeuCauDichVuKyThuat.LyDoHuyDichVu = phauThuatThuThuatHuyDichVuThanhToanViewModel.LyDoHuyDichVu;
            yeuCauDichVuKyThuat.WillDelete = true;

            //BVHD-3825
            var mienGiam = yeuCauDichVuKyThuat.MienGiamChiPhis.FirstOrDefault(x => x.DaHuy != true && x.YeuCauGoiDichVuId != null && (x.TaiKhoanBenhNhanThuId == null || x.TaiKhoanBenhNhanThu.DaHuy != true));
            if (mienGiam != null)
            {
                mienGiam.DaHuy = true;
                mienGiam.WillDelete = true;

                var giamSoTienMienGiam = yeuCauDichVuKyThuat.SoTienMienGiam.GetValueOrDefault() - mienGiam.SoTien;
                if (giamSoTienMienGiam < 0)
                {
                    giamSoTienMienGiam = 0;
                }
                yeuCauDichVuKyThuat.SoTienMienGiam = giamSoTienMienGiam;
            }

            await _tiepNhanBenhNhanService.PrepareForDeleteDichVuAndUpdateAsync(yeuCauTiepNhan);

            // kiểm tra chỉ định dich vụ vượt quá số dư tài khoản
            var chiDinhDichVuResultVo = new ChiDinhDichVuResultVo();
            //chiDinhDichVuResultVo.SoDuTaiKhoan = await _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhan.Id);
            //chiDinhDichVuResultVo.SoDuTaiKhoanConLai = await _taiKhoanBenhNhanService.GetSoTienUocLuongConLai(yeuCauTiepNhan.Id);

            decimal soDuTk = 0;
            decimal soDuUocLuongConLai = 0;

            if (yeuCauTiepNhan.NoiTruBenhAn != null || yeuCauTiepNhan.QuyetToanTheoNoiTru == true)
            {
                //var chiPhiKhamChuaBenh = _thuNganNoiTruService.GetDanhSachChiPhiKhamChuaBenhChuaThu(yeuCauTiepNhan.Id).Result.Select(o => o.BNConPhaiThanhToan).DefaultIfEmpty(0).Sum();
                var chiPhiKhamChuaBenh = _thuNganNoiTruService.GetSoTienBNConPhaiThanhToan(yeuCauTiepNhan.Id).Result;

                soDuTk = await _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhan.Id);
                soDuUocLuongConLai = soDuTk - chiPhiKhamChuaBenh;
            }
            else
            {
                soDuTk = await _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhan.Id);
                soDuUocLuongConLai = await _taiKhoanBenhNhanService.GetSoTienUocLuongConLai(yeuCauTiepNhan.Id);
            }

            chiDinhDichVuResultVo.SoDuTaiKhoan = soDuTk;
            chiDinhDichVuResultVo.SoDuTaiKhoanConLai = soDuUocLuongConLai;

            return chiDinhDichVuResultVo;
        }

        #region chỉ định gói dv marketing
        #region grid
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetGoiDichVuCuaBenhNhanDataForGrid")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult<GridDataSource>> GetGoiDichVuCuaBenhNhanDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _khamBenhService.GetGoiDichVuCuaBenhNhanDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetGoiDichVuCuaBenhNhanTotalPageForGrid")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult<GridDataSource>> GetGoiDichVuCuaBenhNhanTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _khamBenhService.GetGoiDichVuCuaBenhNhanTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetChiTietGoiDichVuCuaBenhNhanDataForGrid")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult<GridDataSourceChiTietGoiDichVuTheoBenhNhan>> GetChiTietGoiDichVuCuaBenhNhanDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var searchObj = queryInfo.AdditionalSearchString.Split(';');
            var yeuCauGoiDichVuId = long.Parse(searchObj[0]);
            var yeuCauGoiDichVu = _yeuCauGoiDichVuService.GetById(yeuCauGoiDichVuId);
            var benhNhanId = yeuCauGoiDichVu.GoiSoSinh == true ? yeuCauGoiDichVu.BenhNhanSoSinhId : yeuCauGoiDichVu.BenhNhanId;
            var dichVuGiuongDaChiDinhs = await _dieuTriNoiTruService.GetThongTinSuDungDichVuGiuongTrongGoiAsync(benhNhanId ?? 0);

            var gridData = await _khamBenhService.GetChiTietGoiDichVuCuaBenhNhanDataForGridAsync(queryInfo, false, dichVuGiuongDaChiDinhs);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetChiTietGoiDichVuCuaBenhNhanTotalPageForGrid")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult<GridDataSourceChiTietGoiDichVuTheoBenhNhan>> GetChiTietGoiDichVuCuaBenhNhanTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _khamBenhService.GetChiTietGoiDichVuCuaBenhNhanTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }


        #endregion

        [HttpGet("KiemTraDangKyGoiDichVuTheoBenhNhan")]
        public async Task<ActionResult<bool>> KiemTraDangKyGoiDichVuTheoBenhNhanAsync(long benhNhanId)
        {
            var kiemTra = await _khamBenhService.KiemTraDangKyGoiDichVuTheoBenhNhanAsync(benhNhanId);
            return kiemTra;
        }

        [HttpPost("KiemTraValidationChiDinhDichVuTrongGoiMarketing")]
        public async Task<ActionResult> KiemTraValidationChiDinhGoiDichVuTheoBenhNhanAsync([FromBody] ChiDinhGoiDichVuTheoBenhNhanViewModel chiDinhViewModel)
        {
            if (!chiDinhViewModel.DichVus.Any())
            {
                throw new ApiException(_localizationService.GetResource("ChiDinhGoiDichVuMarketing.DichVu.Required"));
            }
            if (chiDinhViewModel.DichVus.Any(x => x.SoLuongSuDung == 0))
            {
                throw new ApiException(_localizationService.GetResource("ChiDinhGoiDichVuMarketing.SoLuongChiDinh.Required"));
            }

            var yeuCauVo = chiDinhViewModel.Map<ChiDinhGoiDichVuTheoBenhNhanVo>();
            await _khamBenhService.KiemTraSoLuongConLaiCuaDichVuTrongGoiAsync(yeuCauVo);
            return Ok();
        }

        [HttpPost("KiemTraDichVuTrongGoiMarketingDaCoTheoYeuCauTiepNhan")]
        public async Task<ActionResult<List<ChiDinhGoiDichVuTheoBenhNhanDichVuLoiVo>>> KiemTraDichVuTrongGoiMarketingDaCoTheoYeuCauTiepNhanAsync([FromBody] ChiDinhGoiDichVuTheoBenhNhanViewModel chiDinhViewModel)
        {
            var dichVuTrung = await _khamBenhService.KiemTraValidationChiDinhGoiDichVuTheoBenhNhanAsync(chiDinhViewModel.YeuCauTiepNhanId, chiDinhViewModel.DichVus.Select(a => a.Id).ToList());
            return dichVuTrung;
        }

        [HttpPost("ThemChiDinhGoiDichVuTheoBenhNhan")]
        [ClaimRequirement(SecurityOperation.Add, DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult<ChiDinhDichVuResultVo>> ThemChiDinhGoiDichVuTheoBenhNhanAsync([FromBody] ChiDinhGoiDichVuTheoBenhNhanViewModel yeuCauViewModel)
        {
            // get thông tin yêu cầu tiếp nhận hiện tại
            //var yeuCauTiepNhanChiTiet =
            //    await _khamBenhService.GetYeuCauTiepNhanByIdAsync(yeuCauViewModel.YeuCauTiepNhanId);
            var yeuCauTiepNhanChiTiet =
                await _khamBenhService.GetYeuCauTiepNhanKhiThemDVTrongGoiNgoaiTruByIdAsync(yeuCauViewModel.YeuCauTiepNhanId);
            var yeuCauVo = yeuCauViewModel.Map<ChiDinhGoiDichVuTheoBenhNhanVo>();
            await _khamBenhService.XuLyThemChiDinhGoiDichVuTheoBenhNhanAsync(yeuCauTiepNhanChiTiet, yeuCauVo);
            await _tiepNhanBenhNhanService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);

            // kiểm tra chỉ định dich vụ vượt quá số dư tài khoản
            var chiDinhDichVuResultVo = new ChiDinhDichVuResultVo();
            //chiDinhDichVuResultVo.SoDuTaiKhoan = await _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhanChiTiet.Id);
            //chiDinhDichVuResultVo.SoDuTaiKhoanConLai = await _taiKhoanBenhNhanService.GetSoTienUocLuongConLai(yeuCauTiepNhanChiTiet.Id);

            decimal soDuTk = 0;
            decimal soDuUocLuongConLai = 0;

            if (yeuCauTiepNhanChiTiet.NoiTruBenhAn != null || yeuCauTiepNhanChiTiet.QuyetToanTheoNoiTru == true)
            {
                //var chiPhiKhamChuaBenh = _thuNganNoiTruService.GetDanhSachChiPhiKhamChuaBenhChuaThu(yeuCauTiepNhanChiTiet.Id).Result.Select(o => o.BNConPhaiThanhToan).DefaultIfEmpty(0).Sum();
                var chiPhiKhamChuaBenh = _thuNganNoiTruService.GetSoTienBNConPhaiThanhToan(yeuCauTiepNhanChiTiet.Id).Result;

                soDuTk = await _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhanChiTiet.Id);
                soDuUocLuongConLai = soDuTk - chiPhiKhamChuaBenh;
            }
            else
            {
                soDuTk = await _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhanChiTiet.Id);
                soDuUocLuongConLai = await _taiKhoanBenhNhanService.GetSoTienUocLuongConLai(yeuCauTiepNhanChiTiet.Id);
            }

            chiDinhDichVuResultVo.SoDuTaiKhoan = soDuTk;
            chiDinhDichVuResultVo.SoDuTaiKhoanConLai = soDuUocLuongConLai;

            // dịch vụ chỉ định từ gói marketing ko cần kiểm tra trạng thái thanh toán
            //chiDinhDichVuResultVo.IsVuotQuaSoDuTaiKhoan = yeuCauVo.YeuCauKhamBenhNews.Any(x => x.TrangThaiThanhToan != TrangThaiThanhToan.BaoLanhThanhToan)
            //                                               || yeuCauVo.YeuCauDichVuKyThuatNews.Any(x => x.TrangThaiThanhToan != TrangThaiThanhToan.BaoLanhThanhToan)
            //                                               || yeuCauVo.YeuCauDichVuGiuongBenhVienNews.Any(x => x.TrangThaiThanhToan != TrangThaiThanhToan.BaoLanhThanhToan);

            return chiDinhDichVuResultVo;
        }

        [HttpPost("ThemYeuGoiDichVuThuongDung")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult<ChiDinhDichVuResultVo>> ThemYeuGoiDichVuThuongDungAsync([FromBody] ChiDinhNhomGoiDichVuThuongDungViewModel yeuCauViewModel)
        {
            // get thông tin yêu cầu tiếp nhận hiện tại
            //var yeuCauTiepNhanChiTiet =
            //    await _khamBenhService.GetYeuCauTiepNhanByIdAsync(yeuCauViewModel.YeuCauTiepNhanId);
            var yeuCauTiepNhanChiTiet =
                await _khamBenhService.GetYeuCauTiepNhanKhiThemDvThuongDungNgoaiTruByIdAsync(yeuCauViewModel.YeuCauTiepNhanId);
            var yeuCauVo = yeuCauViewModel.Map<YeuCauThemGoiDichVuThuongDungVo>();
            await _khamBenhService.XuLyThemGoiDichVuThuongDungAsync(yeuCauTiepNhanChiTiet, yeuCauVo);
            await _tiepNhanBenhNhanService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);

            // kiểm tra chỉ định dich vụ vượt quá số dư tài khoản
            var chiDinhDichVuResultVo = new ChiDinhDichVuResultVo();
            //chiDinhDichVuResultVo.SoDuTaiKhoan = await _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhanChiTiet.Id);
            //chiDinhDichVuResultVo.SoDuTaiKhoanConLai = await _taiKhoanBenhNhanService.GetSoTienUocLuongConLai(yeuCauTiepNhanChiTiet.Id);

            decimal soDuTk = 0;
            decimal soDuUocLuongConLai = 0;

            if (yeuCauTiepNhanChiTiet.NoiTruBenhAn != null || yeuCauTiepNhanChiTiet.QuyetToanTheoNoiTru == true)
            {
                //var chiPhiKhamChuaBenh = _thuNganNoiTruService.GetDanhSachChiPhiKhamChuaBenhChuaThu(yeuCauTiepNhanChiTiet.Id).Result.Select(o => o.BNConPhaiThanhToan).DefaultIfEmpty(0).Sum();
                var chiPhiKhamChuaBenh = _thuNganNoiTruService.GetSoTienBNConPhaiThanhToan(yeuCauTiepNhanChiTiet.Id).Result;

                soDuTk = await _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhanChiTiet.Id);
                soDuUocLuongConLai = soDuTk - chiPhiKhamChuaBenh;
            }
            else
            {
                soDuTk = await _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhanChiTiet.Id);
                soDuUocLuongConLai = await _taiKhoanBenhNhanService.GetSoTienUocLuongConLai(yeuCauTiepNhanChiTiet.Id);
            }

            chiDinhDichVuResultVo.SoDuTaiKhoan = soDuTk;
            chiDinhDichVuResultVo.SoDuTaiKhoanConLai = soDuUocLuongConLai;

            chiDinhDichVuResultVo.IsVuotQuaSoDuTaiKhoan = yeuCauVo.YeuCauKhamBenhNews.Any(x => x.TrangThaiThanhToan != TrangThaiThanhToan.BaoLanhThanhToan)
                                                           || yeuCauVo.YeuCauDichVuKyThuatNews.Any(x => x.TrangThaiThanhToan != TrangThaiThanhToan.BaoLanhThanhToan)
                                                           || yeuCauVo.YeuCauDichVuGiuongBenhVienNews.Any(x => x.TrangThaiThanhToan != TrangThaiThanhToan.BaoLanhThanhToan);

            return chiDinhDichVuResultVo;
        }
        #endregion

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetSoDuTaiKhoan")]
        public async Task<ActionResult<ChiDinhDichVuResultVo>> GetSoDuTaiKhoanPTTT(long yeuCauTiepNhanId)
        {
            var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(yeuCauTiepNhanId, o => o.Include(p => p.NoiTruBenhAn));

            var chiDinhDichVuResultVo = new ChiDinhDichVuResultVo();

            decimal soDuTk = 0;
            decimal soDuUocLuongConLai = 0;

            if (yeuCauTiepNhan.NoiTruBenhAn != null || yeuCauTiepNhan.QuyetToanTheoNoiTru == true)
            {
                //var chiPhiKhamChuaBenh = _thuNganNoiTruService.GetDanhSachChiPhiKhamChuaBenhChuaThu(yeuCauTiepNhan.Id).Result.Select(o => o.BNConPhaiThanhToan).DefaultIfEmpty(0).Sum();
                var chiPhiKhamChuaBenh = _thuNganNoiTruService.GetSoTienBNConPhaiThanhToan(yeuCauTiepNhan.Id).Result;

                soDuTk = await _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhan.Id);
                soDuUocLuongConLai = soDuTk - chiPhiKhamChuaBenh;
            }
            else
            {
                soDuTk = await _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhan.Id);
                soDuUocLuongConLai = await _taiKhoanBenhNhanService.GetSoTienUocLuongConLai(yeuCauTiepNhan.Id);
            }

            chiDinhDichVuResultVo.SoDuTaiKhoan = soDuTk;
            chiDinhDichVuResultVo.SoDuTaiKhoanConLai = soDuUocLuongConLai;

            return chiDinhDichVuResultVo;
        }
        #region BVHD-3959
        [HttpPost("GetListThoiGianDienBiens")]
        public async Task<ActionResult<ICollection<LookupItemDienBienVo>>> GetListThoiGianDienBiens([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookups = new List<LookupItemDienBienVo>();
            if(!string.IsNullOrEmpty(queryInfo.ParameterDependencies))
            {
                var queryString = JsonConvert.DeserializeObject<DienBienQueryInfo>(queryInfo.ParameterDependencies);

                var yctn = await _dieuTriNoiTruService.GetByIdAsync(queryString.YeuCauTiepNhanId, s =>
                s.Include(p => p.NoiTruBenhAn).ThenInclude(p => p.NoiTruPhieuDieuTris));

                var phieuDieuTri = yctn.NoiTruBenhAn.NoiTruPhieuDieuTris.First(p => p.Id == queryString.PhieuDieuTriId);

                if (!string.IsNullOrEmpty(phieuDieuTri.DienBien))
                {
                    var dienBiens =
                            JsonConvert.DeserializeObject<List<PhieuThamKhamDienBienViewModel>>(phieuDieuTri.DienBien);

                    if(dienBiens != null)
                    {
                        foreach (var item in dienBiens)
                        {
                            var newLookup = new LookupItemDienBienVo()
                            {
                                KeyId = item.IdView,
                                DisplayName = item.ThoiGian?.ApplyFormatDateTime(),
                                ThoiGian = (DateTime)item.ThoiGian // bởi vì date time có required != null
                            };
                            lookups.Add(newLookup);
                        }
                    }
                }
                return Ok(lookups);
            }
            return Ok();
        }
        
        #endregion
    }
}