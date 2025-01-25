using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.BenhVien;
using Camino.Api.Models.Error;
using Camino.Api.Models.KhamBenh;
using Camino.Api.Models.KhamBenh.ViewModelCheckValidators;
using Camino.Api.Models.YeuCauKhamBenh;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Core.Domain.ValueObject.KhamDoan;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.Controllers
{
    public partial class KhamBenhController
    {
        #region grid


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridBenhNhanLamChiDinhAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridBenhNhanLamChiDinhAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauKhamBenhService.GetDataForGridBenhNhanLamChiDinhAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGriBenhNhanLamChiDinhdAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGriBenhNhanLamChiDinhdAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauKhamBenhService.GetTotalPageForGriBenhNhanLamChiDinhdAsync(queryInfo);
            return Ok(gridData);
        }

        #endregion


        #region get data
        [HttpGet("GetDanhSachChoKhamHienTaiAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamDoanKhamBenh)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachChoKhamHienTaiAsync(long phongKhamHienTaiId, string searchString = "", bool laKhamDoan = false)
        {
            var ds = await _yeuCauKhamBenhService.GetDanhSachBenhNhanChoKham(phongKhamHienTaiId, searchString, laKhamDoan);
            return Ok(ds);
        }

        [HttpGet("GetDanhSachChoKhamHangDoiChungAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamDoanKhamBenh)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachChoKhamHangDoiChungAsync(long phongKhamHienTaiId, string searchString = "", bool laKhamDoan = false)
        {
            var ds = await _yeuCauKhamBenhService.GetDanhSachChoKhamHangDoiChungAsync(phongKhamHienTaiId, searchString, laKhamDoan);
            return Ok(ds);
        }

        [HttpGet("GetDanhSachChoKetLuanHangDoiChungAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamDoanKhamBenh)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachChoKetLuanHangDoiChungAsync(long phongKhamHienTaiId, string searchString = "")
        {
            var ds = await _yeuCauKhamBenhService.GetDanhSachChoKetLuanHangDoiChungAsync(phongKhamHienTaiId, searchString);
            return Ok(ds);
        }

        [HttpGet("GetDanhSachLamChiDinhHienTaiAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamDoanKhamBenh)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachLamChiDinhHienTaiAsync(long phongKhamHienTaiId, string searchString = "")
        {
            var ds = await _yeuCauKhamBenhService.GetDanhSachLamChiDinhHienTaiAsync(phongKhamHienTaiId, searchString);
            return Ok(ds);
        }

        [HttpGet("GetDanhSachDoiKetLuanHienTaiAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamDoanKhamBenh)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachDoiKetLuanHienTaiAsync(long phongKhamHienTaiId, string searchString = "", bool laKhamDoan = false)
        {
            var ds = await _yeuCauKhamBenhService.GetDanhSachDoiKetLuanHienTaiAsync(phongKhamHienTaiId, searchString, laKhamDoan);
            return Ok(ds);
        }

        [HttpGet("GetSoLuongYeuCauHienTai")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamDoanKhamBenh)]
        public async Task<SoLuongYeuCauHienTaiVo> GetSoLuongYeuCauHienTai(long phongKhamId, bool laKhamDoan = false)
        {
            var slYeuCauHienTai = await _yeuCauKhamBenhService.GetSoLuongYeuCauHienTai(phongKhamId, laKhamDoan);
            return slYeuCauHienTai;
        }

        [HttpGet("TimKiemBenhNhanTrongHangDoi")]
        public async Task<KhamBenhPhongBenhVienHangDoiViewModel> TimKiemBenhNhanTrongHangDoi(string searchString, long phongKhamId)
        {
            var result = await _yeuCauKhamBenhService.TimKiemBenhNhanTrongHangDoi(searchString, phongKhamId);
            if (result == null)
            {
                return null;
            }
            var thongTinBenhNhan = result.ToModel<KhamBenhPhongBenhVienHangDoiViewModel>();
            if (thongTinBenhNhan.YeuCauKhamBenh != null && thongTinBenhNhan.YeuCauKhamBenh.YeuCauDichVuKyThuats.Any())
            {
                thongTinBenhNhan.YeuCauKhamBenh.YeuCauDichVuKyThuats =
                    thongTinBenhNhan.YeuCauKhamBenh.YeuCauDichVuKyThuats.Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).ToList();
            }
            return thongTinBenhNhan;
        }

        [HttpGet("GetYeuCauKhamBenhDangKhamTheoPhongKham")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamDoanKhamBenh)]
        public async Task<KhamBenhPhongBenhVienHangDoiViewModel> GetYeuCauKhamBenhDangKhamTheoPhongKham(long phongKhamId, long? hangDoiId, bool laKhamDoan = false, bool laKetLuanHangDoiChung = false)
        {
            //BVHD-3751: laKetLuanHangDoiChung dùng cho trường hợp cập nhật thông tin kết luận cho người bệnh ở phòng khác
            var result = await _yeuCauKhamBenhService.GetYeuCauKhamBenhDangKhamTheoPhongKham(phongKhamId, hangDoiId, laKhamDoan, (laKetLuanHangDoiChung ? Enums.EnumTrangThaiHangDoi.ChoKham : Enums.EnumTrangThaiHangDoi.DangKham));
            if (result == null)
            {
                return null;
            }

            #region Cập nhật 01/12/2022: get dvkt điều trị ngoại trú cho region "dvkt điều trị ngoại trú"
            var dvktDieuTriNgoaiTru = _yeuCauKhamBenhService.GetDichVuKyThuatDieuTriNgoaiTruTheoYeuCauKhamBenhId(result.YeuCauKhamBenhId ?? 0);
            result.YeuCauKhamBenh.YeuCauDichVuKyThuats.Add(dvktDieuTriNgoaiTru);
            #endregion

            var resultViewModel = result.ToModel<KhamBenhPhongBenhVienHangDoiViewModel>();

            //BVHD-3706
            resultViewModel.YeuCauKhamBenh.TrieuChungTiepNhan = resultViewModel.YeuCauKhamBenh.TrieuChungTiepNhan ?? resultViewModel.YeuCauTiepNhan.TrieuChungTiepNhan;

            #region//dich vu khuyen mai
            //if ((result.YeuCauTiepNhan.BenhNhan.YeuCauGoiDichVus.Any() && result.YeuCauTiepNhan.BenhNhan.YeuCauGoiDichVus.Any(z => z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs.Any() ||
            //                                                                          z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats.Any() ||
            //                                                                          z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs.Any()))
            // || (result.YeuCauTiepNhan.BenhNhan.YeuCauGoiDichVuSoSinhs.Any() && result.YeuCauTiepNhan.BenhNhan.YeuCauGoiDichVuSoSinhs.Any(z => z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs.Any() ||
            //                                                                           z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats.Any() ||
            //                                                                           z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs.Any())))
            //{
            //    resultViewModel.CoDichVuKhuyenMai = true;
            //}

            var kiemTraGoi = _yeuCauKhamBenhService.KiemTraCoGoiVaKhuyenMaiTheoNguoiBenhId(result.YeuCauTiepNhan.BenhNhanId ?? 0);
            var nguoiBenhCoGoi = kiemTraGoi.Item1;
            resultViewModel.CoDichVuKhuyenMai = kiemTraGoi.Item2;
            #endregion

            #region dvkt điều trị ngoại trú
            var yeuCauKTNew = new YeuCauDichVuKyThuatViewModel();
            foreach (var item in resultViewModel.YeuCauKhamBenh.YeuCauDichVuKyThuats.Where(p => p.DieuTriNgoaiTru == true && p.TrangThaiThanhToan != Enums.TrangThaiThanhToan.HuyThanhToan))
            {
                yeuCauKTNew = new YeuCauDichVuKyThuatViewModel
                {
                    Id = item.Id,
                    TenDichVu = item.TenDichVu,
                    MaDichVu = item.MaDichVu,
                    MaGiaDichVu = item.MaGiaDichVu,
                    NhomGiaDichVuKyThuatBenhVienId = item.NhomGiaDichVuKyThuatBenhVienId,
                    Gia = item.Gia,
                    NhomChiPhi = item.NhomChiPhi,
                    LoaiDichVuKyThuat = item.LoaiDichVuKyThuat,
                    NhomDichVuBenhVienId = item.NhomDichVuBenhVienId,
                    DieuTriNgoaiTru = item.DieuTriNgoaiTru,
                    DuocHuongBaoHiem = item.DuocHuongBaoHiem,
                    DichVuKyThuatBenhVienId = item.DichVuKyThuatBenhVienId,
                    SoLan = item.SoLan,
                    ThoiDiemBatDauDieuTri = item.ThoiDiemBatDauDieuTri,
                    TenDichVuHienThi = item.MaDichVu + " - " + item.TenDichVu,
                    YeuCauKhamBenhId = item.YeuCauKhamBenhId,
                    YeuCauTiepNhanId = item.YeuCauTiepNhanId,
                    TenNhomDichVu = item.TenNhomDichVu,
                    TrangThai = item.TrangThai,
                    TrangThaiThanhToan = item.TrangThaiThanhToan,
                    ThoiDiemChiDinh = item.ThoiDiemChiDinh,
                    ThoiDiemDangKy = item.ThoiDiemDangKy,
                    NhanVienChiDinhId = item.NhanVienChiDinhId
                };
            }
            resultViewModel.YeuCauKhamBenh.YeuCauDichVuKyThuat = yeuCauKTNew;
            if (resultViewModel.YeuCauKhamBenh.YeuCauDichVuKyThuat.Id == 0)
            {
                resultViewModel.YeuCauKhamBenh.CoDieuTriNgoaiTru = null;
            }
            #endregion

            #region bệnh viện hiện tại
            var benhVienHienTai = await _yeuCauKhamBenhService.BenhVienHienTai();
            if (benhVienHienTai != null)
            {
                resultViewModel.YeuCauTiepNhan.BenhVienHienTai = benhVienHienTai.ToModel<BienVienViewModel>();
            }
            #endregion

            // get template khám theo dịch vụ khám
            if (string.IsNullOrEmpty(resultViewModel.YeuCauKhamBenh.ThongTinKhamTheoDichVuTemplate))
            {
                resultViewModel.YeuCauKhamBenh.ThongTinKhamTheoDichVuTemplate = await _khamBenhService.GetTemplateKhamBenhTheoDichVuKham(result.YeuCauKhamBenh.DichVuKhamBenhBenhVienId);
            }

            if (resultViewModel.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe)
            {
                //var lstDichVuKhamSucKhoe = await _yeuCauKhamBenhService.GetTemplateCacDichVuKhamSucKhoeAsync(resultViewModel.YeuCauTiepNhanId, resultViewModel.YeuCauKhamBenh.Id);
                var lstDichVuKhamSucKhoe = await _yeuCauKhamBenhService.GetTemplateCacDichVuKhamSucKhoeAsyncVer2(resultViewModel.YeuCauTiepNhanId, resultViewModel.YeuCauKhamBenh.Id);
                resultViewModel.YeuCauKhamBenh.TemplateDichVuKhamSucKhoes.Add(new KhamBenhTemplateDichVuKhamSucKhoeViewModel()
                {
                    YeuCauKhamBenhId = resultViewModel.YeuCauKhamBenhId.Value,
                    ChuyenKhoaKhamSucKhoe = resultViewModel.YeuCauKhamBenh.ChuyenKhoaKhamSucKhoe,
                    //TenChuyenKhoa = resultViewModel.YeuCauKhamBenh.ChuyenKhoaKhamSucKhoe.GetDescription(),
                    ThongTinKhamTheoDichVuTemplate = resultViewModel.YeuCauKhamBenh.ThongTinKhamTheoDichVuTemplate,
                    ThongTinKhamTheoDichVuData = resultViewModel.YeuCauKhamBenh.ThongTinKhamTheoDichVuData,
                    TrangThai = resultViewModel.YeuCauKhamBenh.TrangThai,
                    //IsDaKham = resultViewModel.YeuCauKhamBenh.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham && resultViewModel.YeuCauKhamBenh.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham,
                    IsDungChuyenKhoaLogin = true
                });
                resultViewModel.YeuCauKhamBenh.TemplateDichVuKhamSucKhoes.AddRange(lstDichVuKhamSucKhoe.Select(item => new KhamBenhTemplateDichVuKhamSucKhoeViewModel()
                {
                    YeuCauKhamBenhId = item.Id,
                    ChuyenKhoaKhamSucKhoe = item.ChuyenKhoaKhamSucKhoe,
                    //TenChuyenKhoa = item.ChuyenKhoaKhamSucKhoe.GetDescription(),
                    ThongTinKhamTheoDichVuTemplate = item.ThongTinKhamTheoDichVuTemplate,
                    ThongTinKhamTheoDichVuData = item.ThongTinKhamTheoDichVuData,
                    TrangThai = item.TrangThai,
                    IsDisabled = true
                    //IsDaKham = item.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham && item.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham
                }).ToList());
                resultViewModel.YeuCauKhamBenh.TemplateDichVuKhamSucKhoes = resultViewModel.YeuCauKhamBenh.TemplateDichVuKhamSucKhoes.OrderBy(x => x.TenChuyenKhoa).ToList();
            }
            //BVHD-3574: hiển thị thông tin người bệnh theo dịch vụ khám nhiều
            else
            {
                resultViewModel.LaDichVuKhamNhieu = await _yeuCauKhamBenhService.KiemTraDichVuHienTaiCoNhieuNguoiBenhAsync(resultViewModel.YeuCauKhamBenh.DichVuKhamBenhBenhVienId.Value);
            }


            #region//get thông tin gói marketing của người bệnh
            //if (result.YeuCauTiepNhan.BenhNhan.YeuCauGoiDichVus.Any() || result.YeuCauTiepNhan.BenhNhan.YeuCauGoiDichVuSoSinhs.Any())

            //Cập nhật 01/12/2022
            if (nguoiBenhCoGoi)
            {
                var gridData = await _khamBenhService.GetGoiDichVuCuaBenhNhanDataForGridAsync(new QueryInfo
                {
                    AdditionalSearchString = $"{result.YeuCauTiepNhan.BenhNhanId} ; false",
                    Take = Int32.MaxValue
                });

                var lstGoi = gridData.Data.Select(p => (GoiDichVuTheoBenhNhanGridVo)p).ToList();
                if (lstGoi.Any())
                {
                    resultViewModel.GoiDichVus = lstGoi;
                }
            }
            #endregion

            //#region Cập nhật 01/12/2022: get tên ICD nếu có
            //var lstIcdId = resultViewModel.YeuCauKhamBenh.YeuCauKhamBenhICDKhacs.Select(x => x.ICDId ?? 0)
            //            .Union(resultViewModel.YeuCauKhamBenh.YeuCauKhamBenhChanDoanPhanBiets.Select(x => x.ICDId ?? 0))
            //            .Distinct().ToList();
            //if (lstIcdId.Any())
            //{
            //    var lstTenICD = _yeuCauKhamBenhService.GetListTenICD(lstIcdId);
            //    foreach (var item in resultViewModel.YeuCauKhamBenh.YeuCauKhamBenhICDKhacs)
            //    {
            //        var icd = lstTenICD.FirstOrDefault(x => x.KeyId == item.ICDId);
            //        item.TenICD = icd?.DisplayName;
            //    }
            //    foreach (var item in resultViewModel.YeuCauKhamBenh.YeuCauKhamBenhChanDoanPhanBiets)
            //    {
            //        var icd = lstTenICD.FirstOrDefault(x => x.KeyId == item.ICDId);
            //        item.TenICD = icd?.DisplayName;
            //    }
            //}
            //#endregion


            // get số dư toàn khoản
            resultViewModel.YeuCauKhamBenh.SoDuTaiKhoan = await _taikhoanBenhNhanService.GetSoTienDaTamUngAsync(resultViewModel.YeuCauTiepNhanId);
            resultViewModel.YeuCauKhamBenh.SoDuTaiKhoanConLai = await _taikhoanBenhNhanService.GetSoTienUocLuongConLai(resultViewModel.YeuCauTiepNhanId);
            resultViewModel.YeuCauKhamBenh.MucTranChiPhi = _khamBenhService.GetMucTranChiPhi();


            #region //BVHD-3575
            // hiện tại code bệnh viện chưa yêu cầu
            //if (resultViewModel.YeuCauKhamBenh.LaChiDinhTuNoiTru != null 
            //    && resultViewModel.YeuCauKhamBenh.LaChiDinhTuNoiTru == true)
            //{
            //    var benhAn = await _dieuTriNoiTruService.GetNoiTruBenhAnAsync(resultViewModel.YeuCauTiepNhan.MaYeuCauTiepNhan);
            //    resultViewModel.YeuCauKhamBenh.SoBenhAn = benhAn.SoBenhAn;
            //}
            #endregion

            #region BVHD-3895
            //if (!laKhamDoan)
            //{
            //    resultViewModel.LaDichVuKhamVietTat = await _yeuCauKhamBenhService.KiemTraDichVuKhamHienThiTenVietTatAsync(resultViewModel.YeuCauKhamBenh.DichVuKhamBenhBenhVienId ?? 0);
            //}

            #endregion

            #region BVHD-3941
            if (resultViewModel.YeuCauTiepNhan.CoBaoHiemTuNhan == true)
            {
                resultViewModel.YeuCauTiepNhan.TenCongTyBaoHiemTuNhan = await _tiepNhanBenhNhanService.GetThongTinBaoHiemTuNhanAsync(result.YeuCauTiepNhanId);
            }


            #endregion

            #region BVHD-3960
            if (resultViewModel.YeuCauTiepNhan.HinhThucDenId != null && resultViewModel.YeuCauTiepNhan.NoiGioiThieuId != null)
            {
                var hinhThucDenGioiThieu = _cauHinhService.GetSetting("CauHinhBaoCao.HinhThucDenGioiThieu");
                long.TryParse(hinhThucDenGioiThieu?.Value, out long hinhThucDenGioiThieuId);
                resultViewModel.YeuCauTiepNhan.LaHinhThucDenGioiThieu = hinhThucDenGioiThieuId != 0 && resultViewModel.YeuCauTiepNhan.HinhThucDenId == hinhThucDenGioiThieuId;
            }

            #endregion
            return resultViewModel;
        }
        #endregion

        #region xử lý

        [HttpPost("XuLyBenhNhanVang")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamDoanKhamBenh)]
        public async Task<ActionResult> XuLyBenhNhanVangAsync(HangDoiKhamBenhInputViewModel hangDoiKhamBenhInputView)
        {
            var hangDoiDangKham =
                await _phongBenhVienHangDoiService.GetByIdAsync(hangDoiKhamBenhInputView.HangDoiDangKhamId ?? 0);
            if (hangDoiDangKham == null)
            {
                return NotFound();
            }

            await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhAsync(hangDoiDangKham.YeuCauKhamBenhId ?? 0);
            await _yeuCauKhamBenhService.XuLyCapNhatBenhNhanVangAsync(hangDoiKhamBenhInputView.HangDoiDangKhamId ?? 0, hangDoiKhamBenhInputView.PhongBenhVienId ?? 0);
            return Ok();
        }

        [HttpPost("XuLyThemBenhNhanVaoChoKhamHienTai")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamDoanKhamBenh)]
        public async Task<ActionResult> XuLyThemBenhNhanVaoChoKhamHienTai(long hangDoiId)
        {
            var hangDoi = await _phongBenhVienHangDoiService.GetByIdAsync(hangDoiId, x => x.Include(y => y.YeuCauKhamBenh));
            if (hangDoi == null)
            {
                return NotFound();
            }

            var lstChoKham = await _yeuCauKhamBenhService.GetDanhSachBenhNhanChoKham(hangDoi.PhongBenhVienId);
            if (lstChoKham != null && hangDoi.YeuCauKhamBenh != null
                                   && (hangDoi.YeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham || hangDoi.YeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DangKham)
                                   && lstChoKham.Any(x => x.Id != hangDoiId))
            {
                //hangDoi.SoThuTu = 0;
                await _phongBenhVienHangDoiService.UpdateAsync(hangDoi);
            }
            return Ok();
        }

        [HttpPost("XuLyThemBenhNhanVaoDoiKetLuanHienTai")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamDoanKhamBenh)]
        public async Task<ActionResult> XuLyThemBenhNhanVaoDoiKetLuanHienTai(long hangDoiId)
        {
            var hangDoi = await _phongBenhVienHangDoiService.GetByIdAsync(hangDoiId, x => x.Include(y => y.YeuCauKhamBenh)
                .ThenInclude(z => z.YeuCauKhamBenhLichSuTrangThais));
            if (hangDoi == null)
            {
                return NotFound();
            }

            var lstLamChiDinh = await _yeuCauKhamBenhService.GetDanhSachLamChiDinhHienTaiAsync(hangDoi.PhongBenhVienId);
            if (lstLamChiDinh != null && hangDoi.YeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DangLamChiDinh && lstLamChiDinh.Any(x => x.Id == hangDoiId))
            {
                hangDoi.YeuCauKhamBenh.TrangThai = Enums.EnumTrangThaiYeuCauKhamBenh.DangDoiKetLuan;

                // lưu lịch sử
                var lichSuNew = new YeuCauKhamBenhLichSuTrangThai();
                lichSuNew.TrangThaiYeuCauKhamBenh = Enums.EnumTrangThaiYeuCauKhamBenh.DangDoiKetLuan;
                lichSuNew.MoTa = lichSuNew.TrangThaiYeuCauKhamBenh.GetDescription();
                hangDoi.YeuCauKhamBenh.YeuCauKhamBenhLichSuTrangThais.Add(lichSuNew);

                await _phongBenhVienHangDoiService.UpdateAsync(hangDoi);
            }
            return Ok();
        }

        [HttpPost("XuLyBatDauKhamBenhNhan")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamDoanKhamBenh)]
        public async Task<ActionResult> XuLyBatDauKhamBenhNhan(HangDoiKhamBenhInputViewModel hangDoiKhamBenhInputView)
        {
            //todo: có cập nhật bỏ await
            var hangDoiBatDauKham =
                _phongBenhVienHangDoiService.GetById(hangDoiKhamBenhInputView.HangDoiBatDauKhamId,
                    x => x.Include(y => y.YeuCauKhamBenh)
                    //.ThenInclude(z => z.YeuCauKhamBenhLichSuTrangThais)
                    );
            if (hangDoiBatDauKham == null)
            {
                return NotFound();
            }
            await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhAsync(hangDoiBatDauKham.YeuCauKhamBenhId ?? 0, hangDoiBatDauKham.PhongBenhVienId, Enums.EnumTrangThaiHangDoi.ChoKham);

            var coBenhNhanKhacDangKham =
                await _yeuCauKhamBenhService.KiemTraCoBenhNhanKhacDangKhamTrongPhong(hangDoiKhamBenhInputView.HangDoiDangKhamId, hangDoiBatDauKham.PhongBenhVienId, hangDoiKhamBenhInputView.LaKhamDoan);
            if (coBenhNhanKhacDangKham)
            {
                throw new ApiException(_localizationService.GetResource("KhamBenh.BatDauKham.CoBenhNhanKhacDangKhamTrongPhong"));
            }

            if (hangDoiKhamBenhInputView.HangDoiDangKhamId != null && hangDoiKhamBenhInputView.HangDoiDangKhamId != 0)
            {
                await XuLyHoanThanhCongDoanKhamHienTaiCuaBenhNhanAsync(hangDoiKhamBenhInputView.HangDoiDangKhamId.Value, hangDoiKhamBenhInputView.HoanThanhKham);
            }

            hangDoiBatDauKham.TrangThai = Enums.EnumTrangThaiHangDoi.DangKham;
            //if (hangDoiBatDauKham.YeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham)
            //{
            //    hangDoiBatDauKham.YeuCauKhamBenh.TrangThai = Enums.EnumTrangThaiYeuCauKhamBenh.DangKham;
            //    hangDoiBatDauKham.YeuCauKhamBenh.ThoiDiemThucHien = DateTime.Now;
            //    hangDoiBatDauKham.YeuCauKhamBenh.NoiThucHienId = hangDoiBatDauKham.PhongBenhVienId;
            //    hangDoiBatDauKham.YeuCauKhamBenh.BacSiThucHienId = _userAgentHelper.GetCurrentUserId();

            //    // lưu lịch sử
            //    var lichSuNew = new YeuCauKhamBenhLichSuTrangThai();
            //    lichSuNew.TrangThaiYeuCauKhamBenh = hangDoiBatDauKham.YeuCauKhamBenh.TrangThai;
            //    lichSuNew.MoTa = lichSuNew.TrangThaiYeuCauKhamBenh.GetDescription();
            //    hangDoiBatDauKham.YeuCauKhamBenh.YeuCauKhamBenhLichSuTrangThais.Add(lichSuNew);
            //}

            // lưu lịch sử
            var lichSuNew = new YeuCauKhamBenhLichSuTrangThai();
            lichSuNew.TrangThaiYeuCauKhamBenh = hangDoiBatDauKham.YeuCauKhamBenh.TrangThai;
            lichSuNew.MoTa = "Chuyển người bệnh vào khám. " + lichSuNew.TrangThaiYeuCauKhamBenh.GetDescription();
            hangDoiBatDauKham.YeuCauKhamBenh.YeuCauKhamBenhLichSuTrangThais.Add(lichSuNew);

            await _phongBenhVienHangDoiService.UpdateAsync(hangDoiBatDauKham);
            return Ok();
        }

        [HttpPost("XuLyHoanThanhCongDoanKhamHienTaiCuaBenhNhan")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamBenhDangKham)]
        public async Task<ActionResult> XuLyHoanThanhCongDoanKhamHienTaiCuaBenhNhanAsync(long hangDoiHienTaiId, bool hoanThanhKham = false, bool isKhamBenhDangKham = false)
        {
            //todo: có cập nhật bỏ await
            var hangDoiDangKham = _phongBenhVienHangDoiService.GetById(hangDoiHienTaiId);
            if (isKhamBenhDangKham)
            {
                await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhDangKhamAsync(hangDoiDangKham.YeuCauKhamBenhId ?? 0);
            }
            else
            {
                await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhAsync(hangDoiDangKham.YeuCauKhamBenhId ?? 0);
            }

            await _yeuCauKhamBenhService.XuLyHoanThanhCongDoanKhamHienTaiCuaBenhNhan(hangDoiHienTaiId, hoanThanhKham);
            return Ok();
        }

        [HttpGet("GetThongTinBenhNhanTiepTheoTrongHangDoi")]
        public async Task<KhamBenhPhongBenhVienHangDoiViewModel> GetThongTinBenhNhanTiepTheoTrongHangDoi(long phongKhamId, bool laKhamDoan = false, string searchBenhNhan = null)
        {
            long hangDoiId = 0;
            var hangDoiKetLuan =
                await _yeuCauKhamBenhService.GetDanhSachDoiKetLuanHienTaiAsync(phongKhamId, searchBenhNhan, laKhamDoan); // BVHD-3337
            if (hangDoiKetLuan != null && hangDoiKetLuan.Count > 0)
            {
                hangDoiId = hangDoiKetLuan.First().Id;
            }
            else
            {
                var hangDoiChoKham =
                    await _yeuCauKhamBenhService.GetDanhSachBenhNhanChoKham(phongKhamId, searchBenhNhan, laKhamDoan); // BVHD-3337
                if (hangDoiChoKham != null && hangDoiChoKham.Count > 0)
                {
                    hangDoiId = hangDoiChoKham.First().Id;
                }
            }

            if (hangDoiId != 0)
            {

                var result =
                    //await _yeuCauKhamBenhService.GetYeuCauKhamBenhDangKhamTheoPhongKham(phongKhamId, hangDoiId, laKhamDoan, Enums.EnumTrangThaiHangDoi.ChoKham);
                    //Cập nhật 30/11/2022: tối ưu inlucde cho trường hợp chỉ get thông tin người bệnh tiếp theo
                    await _yeuCauKhamBenhService.GetYeuCauKhamBenhTiepTheoTheoPhongKham(hangDoiId, laKhamDoan);
                if (result == null)
                {
                    return null;
                }

                var thongTinKham = result.ToModel<KhamBenhPhongBenhVienHangDoiViewModel>();
                if (thongTinKham.YeuCauKhamBenh != null && thongTinKham.YeuCauKhamBenh.YeuCauDichVuKyThuats.Any())
                {
                    thongTinKham.YeuCauKhamBenh.YeuCauDichVuKyThuats =
                        thongTinKham.YeuCauKhamBenh.YeuCauDichVuKyThuats.Where(x =>
                            x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).ToList();
                }

                return thongTinKham;
            }
            return null;
        }

        [HttpPost("CapNhatThongTinQuayLaiYeuCauKhamTruoc")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamDoanKhamBenh)]
        public async Task<ActionResult<YeuCauKhamBenh>> CapNhatThongTinQuayLaiYeuCauKhamTruoc(KetLuanKhamBenhViewModel ketLuan)
        {
            //var yeuCauKhamBenh = await _yeuCauKhamBenhService.GetByIdAsync(ketLuan.Id);
            //if (yeuCauKhamBenh == null)
            //{
            //    return NotFound();
            //}

            //yeuCauKhamBenh.QuayLaiYeuCauKhamBenhTruoc = ketLuan.QuayLaiYeuCauKhamBenhTruoc;
            ketLuan.BacSiKetLuanId = _userAgentHelper.GetCurrentUserId();
            ketLuan.NoiKetLuanId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var yeuCauKham = await LuuKetLuan(ketLuan);
            //await _yeuCauKhamBenhService.UpdateAsync(yeuCauKhamBenh);
            //var thongTinYeuCauKhamSauKhiCapNhat = await _yeuCauKhamBenhService.GetByIdAsync(ketLuan.Id);
            return yeuCauKham;
        }


        [HttpPost("XuLyChuyenBenhNhanVaoHangDoiKetLuan")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamDoanKhamBenh)]
        public async Task<ActionResult> XuLyChuyenBenhNhanVaoHangDoiKetLuan(long hangDoiId)
        {
            var hangDoi = await _phongBenhVienHangDoiService.GetByIdAsync(hangDoiId, x => x.Include(y => y.YeuCauKhamBenh).ThenInclude(z => z.YeuCauKhamBenhLichSuTrangThais));
            if (hangDoi == null)
            {
                return NotFound();
            }

            if (hangDoi.YeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DangLamChiDinh)
            {
                hangDoi.TrangThai = Enums.EnumTrangThaiHangDoi.ChoKham;
                hangDoi.YeuCauKhamBenh.TrangThai = Enums.EnumTrangThaiYeuCauKhamBenh.DangDoiKetLuan;

                // lưu lịch sử
                var lichSuNew = new YeuCauKhamBenhLichSuTrangThai();
                lichSuNew.TrangThaiYeuCauKhamBenh = hangDoi.YeuCauKhamBenh.TrangThai;
                lichSuNew.MoTa = lichSuNew.TrangThaiYeuCauKhamBenh.GetDescription();
                hangDoi.YeuCauKhamBenh.YeuCauKhamBenhLichSuTrangThais.Add(lichSuNew);

                await _phongBenhVienHangDoiService.UpdateAsync(hangDoi);
            }

            return Ok();
        }
        #endregion


        #region InPhieuKhamBenh
        [HttpPost("InPhieuKhamBenh")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamBenhDangKham, Enums.DocumentType.KhamDoanKhamBenh, Enums.DocumentType.LichSuKhamBenh)]
        public async Task<ActionResult> InPhieuKhamBenh(PhieuKhamBenhVo phieuKhamBenhVo)//InPhieuKhamBenh
        {
            //await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhAsync(yeuCauKhamBenhId);
            var htmls = _yeuCauKhamBenhService.InPhieuKhamBenh(phieuKhamBenhVo);
            return Ok(htmls);
        }

        [HttpPost("GetFilePDFFromHtmls")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.None)]
        public ActionResult GetFilePDFFromHtmls(PhieuKhamBenhVoHtml htmlContent)
        {
            var footerHtml = string.Empty;
            footerHtml = @"<!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <script charset='utf-8'>
                    function hostfunction() {
                        replaceParams();
                        timeNow();
                    }
                    function replaceParams() {
                      var url = window.location.href
                        .replace(/#$/, '');
                      var params = (url.split('?')[1] || '').split('&');
                      for (var i = 0; i < params.length; i++) {
                          var param = params[i].split('=');
                          var key = param[0];
                          var value = param[1] || '';
                          var regex = new RegExp('{' + key + '}', 'g');
                          document.getElementById('total').innerHTML = document.body.innerText.replace(regex, value);
                      }
                    }
                        function timeNow() {
                                                  var today = new Date();
                                                  var day = today.getDate();
                                                  var month = today.getMonth() + 1;
                                                  var hour = today.getHours();
                                                  var minutes = today.getMinutes();
                                                   if(day < 10){
                                                        day = '0' + day;
                                                    };
                                                  if(month < 10){
                                                        month = '0' + month;
                                                    };
                                                  if(hour < 10){
                                                        hour = '0' + hour;
                                                    };
                                                  if(minutes < 10){
                                                        minutes = '0' + minutes;
                                                    };
                                                  var date = day+'/'+(month)+'/'+today.getFullYear();
                                                  var time = hour + ': ' + minutes;
                                                  document.getElementById('hvn').innerHTML = date + ' ' + time;
                                            }
                    
                    </script>
                </head>
                <body  onload='hostfunction()' >
                        <div id='hvn' style='float: left;  display: inline; width: 50%; '>
                        </div>
                        <div  id='total' style='float: left;display: inline; width: 50%; text-align: right'>
                        Trang {page}/{topage}
                        </div>  
                        <div style='clear: both; '></div>
                 </body>
                </html>";
            var htmlToPdfVos = new List<HtmlToPdfVo>();
            foreach (var item in htmlContent.Htmls)
            {
                var htmlToPdfVo = new HtmlToPdfVo
                {
                    Html = item,
                    FooterHtml = footerHtml
                };
                htmlToPdfVos.Add(htmlToPdfVo);
            }
            var bytes = _pdfService.ExportMultiFilePdfFromHtml(htmlToPdfVos);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=" + htmlContent.TenFile + DateTime.Now.ToString("dd/MM/yyyy") + ".pdf");
            Response.ContentType = "application/pdf";
            return new FileContentResult(bytes, "application/pdf");
        }
        #endregion

        [HttpGet("InGiayChuyenVien")]
        public async Task<ActionResult> InGiayChuyenVien(long yeuCauKhamBenhId)//InGiayChuyenVien
        {
            //await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhAsync(yeuCauKhamBenhId);
            var result = _yeuCauKhamBenhService.InGiayChuyenVien(yeuCauKhamBenhId);
            return Ok(result);
        }

        [HttpGet("thongTinNghiHuongBHYT")]
        public ActionResult thongTinNghiHuongBHYT(long yeuCauKhamBenhId)
        {
            var thongTinNghiHuongBHYT = _yeuCauKhamBenhService.GetThongTinNgayNghiHuongBHYT(yeuCauKhamBenhId);
            return Ok(thongTinNghiHuongBHYT);
        }

        [HttpPost("XemGiayNghiHuongBHYTLien1")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamBenhDangKham, Enums.DocumentType.KhamDoanKhamBenh)]
        public ActionResult XemGiayNghiHuongBHYTLien1(NghiHuongBHXHViewModel thongTinNgayNghi)//XemGiayNghiHuongBHYTLien1
        {
            ThongTinNgayNghiHuongBHYT thongTin = new ThongTinNgayNghiHuongBHYT
            {
                YeuCauKhamBenhId = thongTinNgayNghi.YeuCauKhamBenhId.Value,
                ThoiDiemTiepNhan = thongTinNgayNghi.ThoiDiemTiepNhan,
                DenNgay = thongTinNgayNghi.DenNgay,

                ICDChinhNghiHuongBHYT = thongTinNgayNghi.ICDChinhNghiHuongBHYT,
                TenICDChinhNghiHuongBHYT = thongTinNgayNghi.TenICDChinhNghiHuongBHYT,
                PhuongPhapDieuTriNghiHuongBHYT = thongTinNgayNghi.PhuongPhapDieuTriNghiHuongBHYT
            };

            if (thongTinNgayNghi.ICDChinhNghiHuongBHYT != null)
            {
                _yeuCauKhamBenhService.KTNgayGiayNghiHuongBHYT(thongTin);
            }
            
            var result = _yeuCauKhamBenhService.XemGiayNghiHuongBHYTLien1(thongTin);
            return Ok(result);
        }

        [HttpPost("XemGiayNghiHuongBHYTLien2")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamBenhDangKham, Enums.DocumentType.KhamDoanKhamBenh)]
        public ActionResult XemGiayNghiHuongBHYTLien2(NghiHuongBHXHViewModel thongTinNgayNghi)//XemGiayNghiHuongBHYTLien2
        {
            ThongTinNgayNghiHuongBHYT thongTin = new ThongTinNgayNghiHuongBHYT
            {
                YeuCauKhamBenhId = thongTinNgayNghi.YeuCauKhamBenhId.Value,
                ThoiDiemTiepNhan = thongTinNgayNghi.ThoiDiemTiepNhan,
                DenNgay = thongTinNgayNghi.DenNgay
            };
            var result = _yeuCauKhamBenhService.XemGiayNghiHuongBHYTLien2(thongTin);
            return Ok(result);
        }

        [HttpPost("KiemTraYeuCauKhamBenh")]
        public async Task<ActionResult> KiemTraYeuCauKhamBenh(long yeuCauKhamBenhId)//KiemTraYeuCauKhamBenh
        {
            await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhAsync(yeuCauKhamBenhId);
            return Ok();
        }

        #region Khám đoàn khám bệnh tất cả phòng
        [HttpPost("GetDanhSachHangDoiKhamDoanTatCa")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanKhamBenh, Enums.DocumentType.KhamDoanKhamBenhTatCaPhong)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachHangDoiKhamDoanTatCaAsync(KhamDoanKhamBenhTatCaPhongTimKiemVo timKiemVo)
        {
            var ds = await _yeuCauKhamBenhService.GetDanhSachHangDoiKhamDoanTatCaAsync(timKiemVo);
            return Ok(ds);
        }

        [HttpPost("GetThongTinBenhNhanTiepTheoTrongHangDoiKhamDoanTatCa")]
        public async Task<ActionResult<KhamBenhPhongBenhVienHangDoiViewModel>> GetThongTinBenhNhanTiepTheoTrongHangDoiKhamDoanTatCaAsync(KhamDoanKhamBenhTatCaPhongTimKiemVo timKiemVo)
        {
            var ds = await _yeuCauKhamBenhService.GetDanhSachHangDoiKhamDoanTatCaAsync(timKiemVo);
            if (ds.Any())
            {
                var result = await _yeuCauKhamBenhService.GetYeuCauKhamBenhDangKhamTheoHopDongKhamDoanAsync(ds.First().Id);

                var thongTinKham = result?.ToModel<KhamBenhPhongBenhVienHangDoiViewModel>();
                //if (thongTinKham.YeuCauKhamBenh != null && thongTinKham.YeuCauKhamBenh.YeuCauDichVuKyThuats.Any())
                //{
                //    thongTinKham.YeuCauKhamBenh.YeuCauDichVuKyThuats =
                //        thongTinKham.YeuCauKhamBenh.YeuCauDichVuKyThuats.Where(x =>
                //            x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).ToList();
                //}

                return thongTinKham;
            }
            return null;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridHangDoiKhamDoanTatCa")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanKhamBenh)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridHangDoiKhamDoanTatCaAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauKhamBenhService.GetDataForGridHangDoiKhamDoanTatCaAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGriHangDoiKhamDoanTatCa")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanKhamBenh)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGriHangDoiKhamDoanTatCaAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauKhamBenhService.GetTotalPageForGriHangDoiKhamDoanTatCaAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpGet("GetYeuCauKhamBenhDangKhamTheoHopDongKhamDoan")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamDoanKhamBenh, Enums.DocumentType.KhamDoanKhamBenhTatCaPhong)]
        public async Task<KhamBenhPhongBenhVienHangDoiViewModel> GetYeuCauKhamBenhDangKhamTheoHopDongKhamDoanAsync(long hangDoiId)
        {
            var result = await _yeuCauKhamBenhService.GetYeuCauKhamBenhDangKhamTheoHopDongKhamDoanAsync(hangDoiId);
            if (result == null)
            {
                return null;
            }

            var resultViewModel = result.ToModel<KhamBenhPhongBenhVienHangDoiViewModel>();

            //BVHD-3706
            resultViewModel.YeuCauKhamBenh.TrieuChungTiepNhan = resultViewModel.YeuCauKhamBenh.TrieuChungTiepNhan ?? resultViewModel.YeuCauTiepNhan.TrieuChungTiepNhan;

            // get template khám theo dịch vụ khám
            if (string.IsNullOrEmpty(resultViewModel.YeuCauKhamBenh.ThongTinKhamTheoDichVuTemplate))
            {
                resultViewModel.YeuCauKhamBenh.ThongTinKhamTheoDichVuTemplate = await _khamBenhService.GetTemplateKhamBenhTheoDichVuKham(result.YeuCauKhamBenh.DichVuKhamBenhBenhVienId);
            }

            var dataEmptyByTemplate = "{\"DataKhamTheoTemplate\": []}";
            if (resultViewModel.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe)
            {
                var lstDichVuKhamSucKhoe = await _yeuCauKhamBenhService.GetTemplateCacDichVuKhamSucKhoeAsync(resultViewModel.YeuCauTiepNhanId, resultViewModel.YeuCauKhamBenh.Id);
                resultViewModel.YeuCauKhamBenh.TemplateDichVuKhamSucKhoes.Add(new KhamBenhTemplateDichVuKhamSucKhoeViewModel()
                {
                    YeuCauKhamBenhId = resultViewModel.YeuCauKhamBenhId.Value,
                    ChuyenKhoaKhamSucKhoe = resultViewModel.YeuCauKhamBenh.ChuyenKhoaKhamSucKhoe,
                    //TenChuyenKhoa = resultViewModel.YeuCauKhamBenh.ChuyenKhoaKhamSucKhoe.GetDescription(),
                    ThongTinKhamTheoDichVuTemplate = resultViewModel.YeuCauKhamBenh.ThongTinKhamTheoDichVuTemplate,
                    ThongTinKhamTheoDichVuData = resultViewModel.YeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham ? null : resultViewModel.YeuCauKhamBenh.ThongTinKhamTheoDichVuData,
                    TrangThai = resultViewModel.YeuCauKhamBenh.TrangThai,
                    IsCheckedDichVu = //resultViewModel.YeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham 
                                        (resultViewModel.YeuCauKhamBenh.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && resultViewModel.YeuCauKhamBenh.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham),
                    //|| string.IsNullOrEmpty(resultViewModel.YeuCauKhamBenh.ThongTinKhamTheoDichVuData) 
                    //|| !resultViewModel.YeuCauKhamBenh.ThongTinKhamTheoDichVuData.Contains(dataEmptyByTemplate),
                    ThongTinKhamTheoDichVuDataDefault = _khamDoanService.GetDataDefaultDichVuKhamSucKhoe(resultViewModel.YeuCauKhamBenh.ChuyenKhoaKhamSucKhoe)
                });
                resultViewModel.YeuCauKhamBenh.TemplateDichVuKhamSucKhoes.AddRange(lstDichVuKhamSucKhoe.Select(item => new KhamBenhTemplateDichVuKhamSucKhoeViewModel()
                {
                    YeuCauKhamBenhId = item.Id,
                    ChuyenKhoaKhamSucKhoe = item.ChuyenKhoaKhamSucKhoe,
                    //TenChuyenKhoa = item.ChuyenKhoaKhamSucKhoe.GetDescription(),
                    ThongTinKhamTheoDichVuTemplate = item.ThongTinKhamTheoDichVuTemplate,
                    ThongTinKhamTheoDichVuData = item.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham ? null : item.ThongTinKhamTheoDichVuData,
                    TrangThai = item.TrangThai, // dùng chung biến để enable cho phép edit nội dung khám theo chuyên khoa
                    IsCheckedDichVu =// item.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham 
                                        (item.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && item.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham),
                    //|| string.IsNullOrEmpty(item.ThongTinKhamTheoDichVuData) 
                    //|| !item.ThongTinKhamTheoDichVuData.Contains(dataEmptyByTemplate),
                    ThongTinKhamTheoDichVuDataDefault = _khamDoanService.GetDataDefaultDichVuKhamSucKhoe(item.ChuyenKhoaKhamSucKhoe)
                }));
                resultViewModel.YeuCauKhamBenh.TemplateDichVuKhamSucKhoes = resultViewModel.YeuCauKhamBenh.TemplateDichVuKhamSucKhoes.OrderBy(x => x.TenChuyenKhoa).ToList();
                resultViewModel.YeuCauKhamBenh.TemplateDichVuKhamSucKhoes.First().IsDungChuyenKhoaLogin = true;

                // tạo enum chuyên khoa khám sức khỏe chính dựa vào 7 chuyên khoa khám
                resultViewModel.YeuCauTiepNhan.ChuyenKhoaKhamSucKhoeChinhs = EnumHelper.GetListEnum<Enums.ChuyenKhoaKhamSucKhoeChinh>().Select(item => (Enums.ChuyenKhoaKhamSucKhoe)item).ToList();

                var lstDichVuDangChon = resultViewModel.YeuCauKhamBenh.TemplateDichVuKhamSucKhoes
                    .Where(x => x.ChuyenKhoaKhamSucKhoe != null
                                && x.IsCheckedDichVu)
                    .ToList();
                resultViewModel.YeuCauTiepNhan.IsDuChuyenKhoaKhamSucKhoeChinh =
                    resultViewModel.YeuCauTiepNhan.ChuyenKhoaKhamSucKhoeChinhs.All(x => lstDichVuDangChon.Select(y => y.ChuyenKhoaKhamSucKhoe).Contains(x))
                    && lstDichVuDangChon.Count >= resultViewModel.YeuCauTiepNhan.ChuyenKhoaKhamSucKhoeChinhs.Count;
            }

            // get số dư toàn khoản
            resultViewModel.YeuCauKhamBenh.SoDuTaiKhoan = await _taikhoanBenhNhanService.GetSoTienDaTamUngAsync(resultViewModel.YeuCauTiepNhanId);
            resultViewModel.YeuCauKhamBenh.SoDuTaiKhoanConLai = await _taikhoanBenhNhanService.GetSoTienUocLuongConLai(resultViewModel.YeuCauTiepNhanId);
            resultViewModel.YeuCauKhamBenh.MucTranChiPhi = _khamBenhService.GetMucTranChiPhi();
            return resultViewModel;
        }

        [HttpPost("LuuThongTinKhamBenhKhamDoan")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.KhamDoanKhamBenh, Enums.DocumentType.KhamDoanKhamBenhTatCaPhong)]
        public async Task<ActionResult<KhamBenhPhongBenhVienHangDoiViewModel>> LuuThongTinKhamBenhKhamDoanAsync([FromBody]PhongBenhVienHangDoiKhamBenhViewModel hangDoiViewModel)
        {
            var benhNhanHienTai = await _yeuCauKhamBenhService.GetYeuCauKhamBenhDangKhamTheoHopDongKhamDoanAsync(hangDoiViewModel.Id);
            if (benhNhanHienTai == null)
            {
                throw new ApiException(_localizationService.GetResource("ApiError.ConcurrencyError"));
            }

            if (!benhNhanHienTai.YeuCauTiepNhan.YeuCauKhamBenhs
                .Any(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.DaKham
                        && x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham))
            {
                throw new ApiException(_localizationService.GetResource("KhamBenh.YeuCauKhamBenh.DaHoanThanhKham"));
            }

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

            hangDoiViewModel.ToEntity(benhNhanHienTai);

            var yeuCauKhamChinhChange = hangDoiViewModel.YeuCauKhamBenh.TemplateDichVuKhamSucKhoes.FirstOrDefault(x => x.YeuCauKhamBenhId == benhNhanHienTai.YeuCauKhamBenhId.Value);
            if (yeuCauKhamChinhChange != null)
            {
                ChangeDataKhamBenh(benhNhanHienTai.YeuCauKhamBenh, yeuCauKhamChinhChange, benhNhanHienTai.YeuCauKhamBenh.TrieuChungTiepNhan, hangDoiViewModel.YeuCauKhamBenh.IsHoanThanhKham);
            }


            foreach (var yeuCauKham in benhNhanHienTai.YeuCauTiepNhan.YeuCauKhamBenhs.Where(x => x.Id != benhNhanHienTai.YeuCauKhamBenhId.Value
                                                                                                 && x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.DaKham
                                                                                                 && x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham))
            {
                var yeuCauKhamChange = hangDoiViewModel.YeuCauKhamBenh.TemplateDichVuKhamSucKhoes.FirstOrDefault(x => x.YeuCauKhamBenhId == yeuCauKham.Id);
                if (yeuCauKhamChange != null)
                {
                    ChangeDataKhamBenh(yeuCauKham, yeuCauKhamChange, benhNhanHienTai.YeuCauKhamBenh.TrieuChungTiepNhan, hangDoiViewModel.YeuCauKhamBenh.IsHoanThanhKham);
                }
            }

            benhNhanHienTai.YeuCauTiepNhan.KSKNhanVienDanhGiaCanLamSangId = _userAgentHelper.GetCurrentUserId();
            //if (hangDoiViewModel.YeuCauKhamBenh.IsHoanThanhKham == true)
            //{
            //    benhNhanHienTai.YeuCauTiepNhan.KSKNhanVienKetLuanId = _userAgentHelper.GetCurrentUserId();
            //    benhNhanHienTai.YeuCauTiepNhan.KSKThoiDiemKetLuan = DateTime.Now;
            //    benhNhanHienTai.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan = Enums.EnumTrangThaiYeuCauTiepNhan.DaHoanTat;
            //    benhNhanHienTai.WillDelete = true;
            //}

            if (hangDoiViewModel.YeuCauTiepNhan.IsDuChuyenKhoaKhamSucKhoeChinh != true)
            {
                benhNhanHienTai.YeuCauTiepNhan.KSKKetLuanPhanLoaiSucKhoe = null;
                //BVHD-3349
                //benhNhanHienTai.YeuCauTiepNhan.KSKKetLuanGhiChu = null;
                //benhNhanHienTai.YeuCauTiepNhan.KSKKetLuanCacBenhTat = null;
            }

            await _phongBenhVienHangDoiService.UpdateAsync(benhNhanHienTai);

            #region Xử lý trường hợp hoàn thành khám, dv nào ko check sẽ hủy
            if (hangDoiViewModel.YeuCauKhamBenh.IsHoanThanhKham == true)
            {
                var yeuCauTiepNhan = benhNhanHienTai.YeuCauTiepNhan;
                yeuCauTiepNhan.KSKNhanVienKetLuanId = yeuCauTiepNhan.KSKNhanVienKetLuanId ??  _userAgentHelper.GetCurrentUserId();
                yeuCauTiepNhan.KSKThoiDiemKetLuan = yeuCauTiepNhan.KSKThoiDiemKetLuan ?? DateTime.Now;
                yeuCauTiepNhan.TrangThaiYeuCauTiepNhan = Enums.EnumTrangThaiYeuCauTiepNhan.DaHoanTat;

                foreach (var yeuCauKham in yeuCauTiepNhan.YeuCauKhamBenhs.Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.DaKham
                                                                                                         && x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham))
                {
                    var yeuCauKhamChange = hangDoiViewModel.YeuCauKhamBenh.TemplateDichVuKhamSucKhoes.FirstOrDefault(x => x.YeuCauKhamBenhId == yeuCauKham.Id);
                    //if (yeuCauKhamChange != null)
                    //{
                    // cập nhật trạng thái 
                    //if (!yeuCauKhamChange.IsCheckedDichVu)
                    //{
                    //    yeuCauKham.WillDelete = true;
                    //}
                    // cập nhật trạng thái 
                    if (yeuCauKhamChange != null && yeuCauKhamChange.IsCheckedDichVu)
                    {
                        yeuCauKham.TrangThai = Enums.EnumTrangThaiYeuCauKhamBenh.DaKham;
                        yeuCauKham.ThoiDiemHoanThanh = yeuCauKham.ThoiDiemHoanThanh ?? DateTime.Now;

                        // lưu lịch sử
                        var lichSuNew = new YeuCauKhamBenhLichSuTrangThai();
                        lichSuNew.TrangThaiYeuCauKhamBenh = yeuCauKham.TrangThai;
                        lichSuNew.MoTa = lichSuNew.TrangThaiYeuCauKhamBenh.GetDescription();
                        yeuCauKham.YeuCauKhamBenhLichSuTrangThais.Add(lichSuNew);
                    }
                    else
                    {
                        yeuCauKham.WillDelete = true;
                    }

                    // xóa hàng đợi
                    if (yeuCauKham.PhongBenhVienHangDois.Any())
                        yeuCauKham.PhongBenhVienHangDois.Last().WillDelete = true;
                    //}
                }

                foreach (var yeuCauDichVuKyThuat in yeuCauTiepNhan.YeuCauDichVuKyThuats.Where(x => x.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien))
                {
                    if (yeuCauTiepNhan.YeuCauDuocPhamBenhViens.Any(x => x.YeuCauDichVuKyThuatId == yeuCauDichVuKyThuat.Id && x.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy)
                        || yeuCauTiepNhan.YeuCauVatTuBenhViens.Any(x => x.YeuCauDichVuKyThuatId == yeuCauDichVuKyThuat.Id && x.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy))
                    {
                        throw new ApiException(_localizationService.GetResource("ChiDinh.HuyDichDichVu.DaCoGhiNhanVTTHThuoc"));
                    }

                    foreach (var hangDoi in yeuCauDichVuKyThuat.PhongBenhVienHangDois)
                    {
                        hangDoi.WillDelete = true;
                    }
                    yeuCauDichVuKyThuat.WillDelete = true;
                }

                await _yeuCauTiepNhanService.PrepareForDeleteDichVuAndUpdateAsync(yeuCauTiepNhan); //PrepareForEditYeuCauTiepNhanAndUpdateAsync
            }
            #endregion

            var yeuCauKhamBenh = benhNhanHienTai.ToModel<KhamBenhPhongBenhVienHangDoiViewModel>();
            return yeuCauKhamBenh;
        }

        private void ChangeDataKhamBenh(YeuCauKhamBenh yeuCauKhamBenh, KhamBenhTemplateDichVuKhamSucKhoeViewModel dataChange, string trieuChungTiepNhan, bool? hoanThanhKham = false)
        {
            // kiểm tra trạng thái yêu cầu khám bệnh dang khám
            if (dataChange.IsCheckedDichVu)
            {
                //BVHD-3706
                yeuCauKhamBenh.TrieuChungTiepNhan = yeuCauKhamBenh.TrieuChungTiepNhan ?? trieuChungTiepNhan;

                if (yeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham)
                {
                    yeuCauKhamBenh.TrangThai = Enums.EnumTrangThaiYeuCauKhamBenh.DangKham;
                    //yeuCauKhamBenh.NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
                    yeuCauKhamBenh.BacSiThucHienId = _userAgentHelper.GetCurrentUserId();
                    yeuCauKhamBenh.ThoiDiemThucHien = DateTime.Now;
                    yeuCauKhamBenh.BacSiKetLuanId = _userAgentHelper.GetCurrentUserId();
                    //yeuCauKhamBenh.NoiKetLuanId = _userAgentHelper.GetCurrentNoiLLamViecId();

                    // mặc định là nơi đăng ký, hám đoàn tất cả chỉ là nhập liệu thông tin
                    yeuCauKhamBenh.NoiThucHienId = yeuCauKhamBenh.NoiKetLuanId = yeuCauKhamBenh.NoiDangKyId;


                    YeuCauKhamBenhLichSuTrangThai trangThaiMoi = new YeuCauKhamBenhLichSuTrangThai
                    {
                        TrangThaiYeuCauKhamBenh = yeuCauKhamBenh.TrangThai,
                        MoTa = yeuCauKhamBenh.TrangThai.GetDescription()
                    };
                    yeuCauKhamBenh.YeuCauKhamBenhLichSuTrangThais.Add(trangThaiMoi);
                }
            }
            else if (dataChange.IsChangeData)
            {
                yeuCauKhamBenh.TrangThai = Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham;
                yeuCauKhamBenh.NoiThucHienId = null;
                yeuCauKhamBenh.BacSiThucHienId = null;
                yeuCauKhamBenh.ThoiDiemThucHien = null;
                yeuCauKhamBenh.BacSiKetLuanId = null;
                yeuCauKhamBenh.NoiKetLuanId = null;

                YeuCauKhamBenhLichSuTrangThai trangThaiMoi = new YeuCauKhamBenhLichSuTrangThai
                {
                    TrangThaiYeuCauKhamBenh = yeuCauKhamBenh.TrangThai,
                    MoTa = "Bỏ khám dịch vụ trong khám đoàn tất cả"
                };
                yeuCauKhamBenh.YeuCauKhamBenhLichSuTrangThais.Add(trangThaiMoi);
            }

            if (dataChange != null)
            {
                yeuCauKhamBenh.ThongTinKhamTheoDichVuData = dataChange.ThongTinKhamTheoDichVuData;
                yeuCauKhamBenh.ThongTinKhamTheoDichVuTemplate = dataChange.ThongTinKhamTheoDichVuTemplate;
            }

            //if (hoanThanhKham == true)
            //{
            //    // cập nhật trạng thái 
            //    if (dataChange.IsCheckedDichVu)
            //    {
            //        yeuCauKhamBenh.TrangThai = Enums.EnumTrangThaiYeuCauKhamBenh.DaKham;
            //        yeuCauKhamBenh.ThoiDiemHoanThanh = DateTime.Now;
            //    }
            //    else
            //    {
            //        yeuCauKhamBenh.WillDelete = true;
            //    }

            //    // xóa hàng đợi
            //    if (yeuCauKhamBenh.PhongBenhVienHangDois.Any())
            //        yeuCauKhamBenh.PhongBenhVienHangDois.Last().WillDelete = true;

            //    // lưu lịch sử
            //    var lichSuNew = new YeuCauKhamBenhLichSuTrangThai();
            //    lichSuNew.TrangThaiYeuCauKhamBenh = yeuCauKhamBenh.TrangThai;
            //    lichSuNew.MoTa = lichSuNew.TrangThaiYeuCauKhamBenh.GetDescription();
            //    yeuCauKhamBenh.YeuCauKhamBenhLichSuTrangThais.Add(lichSuNew);
            //}

            //if (hoanThanhKham == true)
            //{
            //    // cập nhật trạng thái 
            //    if (dataChange.IsCheckedDichVu)
            //    {
            //        yeuCauKhamBenh.TrangThai = Enums.EnumTrangThaiYeuCauKhamBenh.DaKham;
            //        yeuCauKhamBenh.ThoiDiemHoanThanh = DateTime.Now;

            //        // xóa hàng đợi
            //        if (yeuCauKhamBenh.PhongBenhVienHangDois.Any())
            //            yeuCauKhamBenh.PhongBenhVienHangDois.Last().WillDelete = true;

            //        // lưu lịch sử
            //        var lichSuNew = new YeuCauKhamBenhLichSuTrangThai();
            //        lichSuNew.TrangThaiYeuCauKhamBenh = yeuCauKhamBenh.TrangThai;
            //        lichSuNew.MoTa = lichSuNew.TrangThaiYeuCauKhamBenh.GetDescription();
            //        yeuCauKhamBenh.YeuCauKhamBenhLichSuTrangThais.Add(lichSuNew);
            //    }
            //}
        }

        [HttpPost("GetKetQuaMauDichVuKhamBenh")]
        public async Task<ActionResult> GetKetQuaMauDichVuKhamBenhAsync(KhamDoanTatCaPhongKetQuaMauVo ketQuaMuaVo)
        {
            var entity = _khamDoanService.GetYeuCauTiepNhan(ketQuaMuaVo.YeuCauTiepNhanId, ketQuaMuaVo.HopDongKhamSucKhoeNhanVienId);
            var result = _khamDoanService.GetKetQuaMau(entity);
            return Ok(result.Result);
        }

        [HttpPost("GetKetQuaMauDichVuKyThuat")]
        public async Task<ActionResult<List<KetQuaMauDichVuKyThuatDataVo>>> GetKetQuaMauDichVuKyThuatAsync(KhamDoanTatCaPhongKetQuaMauVo ketQuaMuaVo)
        {
            var result = await _yeuCauKhamBenhService.GetKetQuaMauDichVuKyThuatAsync(ketQuaMuaVo);
            return Ok(result);
        }

        [HttpPut("KiemTraDichVuKhamDoanChuaThucHien")]
        public async Task<ActionResult<KhamDoanTatCaPhongDichVuChuaThucHienVo>> KiemTraDichVuKhamDoanChuaThucHienAsync(KhamDoanTatCaPhongKiemTraDichVuChuaThucHienVo kiemTraVo)
        {
            var result = await _yeuCauKhamBenhService.KiemTraDichVuKhamDoanChuaThucHienAsync(kiemTraVo);
            return result;
        }
        #endregion

        #region cập nhật quay lại chưa khám
        [HttpPut("XuLyQuayLaiChuaKham")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamBenhDangKham)]
        public async Task<ActionResult> XuLyQuayLaiChuaKhamAsync(long hangDoiId)
        {
            // xử lý quay lại chưa khám
            await _yeuCauKhamBenhService.XuLyQuayLaiChuaKhamAsync(hangDoiId);
            return Ok();
        }

        [HttpPut("XuLyQuayLaiChuaKhamKhamDoanTheoPhong")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.KhamDoanKhamBenh)]
        public async Task<ActionResult> XuLyQuayLaiChuaKhamKhamDoanTheoPhongAsync(long hangDoiId)
        {
            var hangDoi = _phongBenhVienHangDoiService.GetById(hangDoiId, 
                              a => a.Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.YeuCauKhamBenhs)
                .Include(x => x.YeuCauKhamBenh).ThenInclude(x => x.YeuCauDichVuKyThuats)
                .Include(x => x.YeuCauKhamBenh).ThenInclude(x => x.YeuCauDuocPhamBenhViens)
                .Include(x => x.YeuCauKhamBenh).ThenInclude(x => x.YeuCauVatTuBenhViens)
                .Include(x => x.YeuCauKhamBenh).ThenInclude(x => x.YeuCauKhamBenhLichSuTrangThais)
                .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.YeuCauKhamBenhs)
                .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.TuVanThuocKhamSucKhoes));

            var thongTinKhamData = string.Empty;
            if (hangDoi.YeuCauKhamBenh?.ChuyenKhoaKhamSucKhoe != null)
            {
                thongTinKhamData = _khamDoanService.GetDataDefaultDichVuKhamSucKhoe(hangDoi.YeuCauKhamBenh.ChuyenKhoaKhamSucKhoe);
            }

            // xử lý quay lại chưa khám
            await _yeuCauKhamBenhService.XuLyQuayLaiChuaKhamKhamDoanTheoPhongAsync(hangDoi, thongTinKhamData);
            return Ok();
        }
        #endregion


        #region BVHD-3574
        [HttpGet("KiemTraKhoaHienTaiCoNhieuNguoiBenh")]
        public async Task<bool> KiemTraKhoaHienTaiCoNhieuNguoiBenhAsync()
        {
            var result = await _yeuCauKhamBenhService.KiemTraKhoaHienTaiCoNhieuNguoiBenhAsync();
            return result;
        }


        #endregion

        #region BVHD-3797
        [HttpGet("KiemTraCoDichVuChuaThucHien")]
        public async Task<List<string>> KiemTraCoDichVuChuaThucHienAsync(long yeuCauKhamBenhId)
        {
            var result = await _yeuCauKhamBenhService.KiemTraCoDichVuChuaThucHienAsync(yeuCauKhamBenhId);
            return result;
        }

        #endregion
    }
}
