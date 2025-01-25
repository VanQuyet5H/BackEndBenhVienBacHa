using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.KhamBenh;
using Camino.Api.Models.TiemChung;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.TiemChungs;
using Camino.Core.Helpers;
using Camino.Services.BenhNhans;
using Camino.Services.DieuTriNoiTru;
using Camino.Services.Helpers;
using Camino.Services.KhamBenhs;
using Camino.Services.Localization;
using Camino.Services.PhauThuatThuThuat;
using Camino.Services.PhongBenhVien;
using Camino.Services.TiemChung;
using Camino.Services.TiepNhanBenhNhan;
using Camino.Services.YeuCauKhamBenh;
using Camino.Services.YeuCauLinhDuocPham;
using Camino.Services.YeuCauTiepNhans;
using Microsoft.AspNetCore.Mvc;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public partial class TiemChungController : CaminoBaseController
    {
        private readonly ITiemChungService _tiemChungService;
        private readonly ILocalizationService _localizationService;
        private readonly IYeuCauDichVuKyThuatService _yeuCauDichVuKyThuatService;
        private readonly ITaiKhoanBenhNhanService _taiKhoanBenhNhanService;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly IPhongBenhVienHangDoiService _phongBenhVienHangDoiService;
        private readonly IKhamBenhService _khamBenhService;
        private readonly IYeuCauKhamBenhService _yeuCauKhamBenhService;
        private readonly IPhauThuatThuThuatService _phauThuatThuThuatService;
        private readonly IYeuCauLinhDuocPhamService _yeuCauLinhDuocPhamService;
        private readonly IPhongBenhVienService _phongBenhVienService;
        private readonly IThuNganNoiTruService _thuNganNoiTruService;
        private readonly IYeuCauGoiDichVuService _yeuCauGoiDichVuService;
        private readonly IDieuTriNoiTruService _dieuTriNoiTruService;
        private readonly ITiepNhanBenhNhanService _tiepNhanBenhNhanService;

        public TiemChungController(
            ITiemChungService tiemChungService,
            ILocalizationService localizationService,
            IYeuCauDichVuKyThuatService yeuCauDichVuKyThuatService,
            ITaiKhoanBenhNhanService taiKhoanBenhNhanService,
            IUserAgentHelper userAgentHelper,
            IPhongBenhVienHangDoiService phongBenhVienHangDoiService,
            IKhamBenhService khamBenhService,
            IYeuCauKhamBenhService yeuCauKhamBenhService,
            IPhauThuatThuThuatService phauThuatThuThuatService,
            IYeuCauLinhDuocPhamService yeuCauLinhDuocPhamService,
            IPhongBenhVienService phongBenhVienService,
            IThuNganNoiTruService thuNganNoiTruService,
            IYeuCauGoiDichVuService yeuCauGoiDichVuService,
            IDieuTriNoiTruService dieuTriNoiTruService,
            ITiepNhanBenhNhanService tiepNhanBenhNhanService
        )
        {
            _tiemChungService = tiemChungService;
            _localizationService = localizationService;
            _yeuCauDichVuKyThuatService = yeuCauDichVuKyThuatService;
            _taiKhoanBenhNhanService = taiKhoanBenhNhanService;
            _userAgentHelper = userAgentHelper;
            _phongBenhVienHangDoiService = phongBenhVienHangDoiService;
            _khamBenhService = khamBenhService;
            _yeuCauKhamBenhService = yeuCauKhamBenhService;
            _phauThuatThuThuatService = phauThuatThuThuatService;
            _yeuCauLinhDuocPhamService = yeuCauLinhDuocPhamService;
            _phongBenhVienService = phongBenhVienService;
            _thuNganNoiTruService = thuNganNoiTruService;
            _yeuCauGoiDichVuService = yeuCauGoiDichVuService;
            _dieuTriNoiTruService = dieuTriNoiTruService;
            _tiepNhanBenhNhanService = tiepNhanBenhNhanService;
        }

        #region Get data
        [HttpPost("GetDanhSachChoKhamHienTai")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TiemChungKhamSangLoc, Enums.DocumentType.TiemChungThucHienTiem)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachChoKhamHienTaiAsync(HangDoiTiemChungQuyeryInfo quyeryInfo)
        {
            var ds = await _tiemChungService.GetDanhSachChoKhamHienTaiAsyncVer3(quyeryInfo);
            return Ok(ds);
        }

        [HttpPost("GetYeuCauKhamTiemChungDangKhamTheoPhongKham")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TiemChungKhamSangLoc, Enums.DocumentType.TiemChungThucHienTiem)]
        public async Task<YeuCauKhamTiemChungViewModel> GetYeuCauKhamTiemChungDangKhamTheoPhongKhamAsync(HangDoiTiemChungDangKhamQuyeryInfo queryInfo)
        {
            //var result = await _tiemChungService.GetYeuCauKhamTiemChungDangKhamTheoPhongKhamAsync(queryInfo);
            //var result = await _tiemChungService.GetYeuCauKhamTiemChungDangKhamTheoPhongKhamAsyncVer2(queryInfo);
            var result = await _tiemChungService.GetThongTinYeuCauKhamTiemChungDangKhamTheoPhongKhamAsyncVer2(queryInfo);
            if (result == null)
            {
                return null;
            }

            //BVHD-3800
            //var laCapCuu = result.YeuCauTiepNhan.LaCapCuu ?? result.YeuCauTiepNhan.YeuCauNhapVien?.YeuCauKhamBenh?.YeuCauTiepNhan?.LaCapCuu;

            #region Cập nhật 4/4/2022
            bool? laCapCuuTiepNhanNoiTru = false;
            if (result.YeuCauTiepNhan.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru)
            {
                laCapCuuTiepNhanNoiTru = result.YeuCauTiepNhan.LaCapCuu;
            }
            else
            {
                laCapCuuTiepNhanNoiTru = await _tiemChungService.KiemTraLanTiepNhanLaCapCuuAsync(result.YeuCauTiepNhanId);
            }
            var laCapCuu = result.YeuCauTiepNhan.LaCapCuu ?? laCapCuuTiepNhanNoiTru;
            #endregion

            var resultViewModel = result.ToModel<YeuCauKhamTiemChungViewModel>();

            //BVHD-3800
            resultViewModel.YeuCauTiepNhan.LaCapCuu = laCapCuu;

            var soThangTuoi = CalculateHelper.TinhTongSoThangCuaTuoi(resultViewModel.YeuCauTiepNhan.NgaySinh, resultViewModel.YeuCauTiepNhan.ThangSinh, resultViewModel.YeuCauTiepNhan.NamSinh);
            resultViewModel.SoThangTuoi = soThangTuoi;

            // trường hợp chưa khám thì tạo mới
            if (resultViewModel.KhamSangLocTiemChung == null)
            {
                resultViewModel.KhamSangLocTiemChung = new TiemChungYeuCauDichVuKyThuatKhamSangLocViewModel();
            }
            else
            {
                resultViewModel.CoVacxinChuaTiem = resultViewModel.KhamSangLocTiemChung.YeuCauDichVuKyThuats.Any(x => x.TiemChung.TrangThaiTiemChung == Enums.TrangThaiTiemChung.ChuaTiemChung);

                // hiện tại ko cần check trạng thái thanh thanh toán, cứ có vacxin đã chỉ định mà chưa tiêm là vẫn hiện nút lưu chứ ko hiện nút lưu hoàn thanh tiêm
                //&& (x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan 
                //    || x.TrangThaiThanhToan == TrangThaiThanhToan.BaoLanhThanhToan
                //    || x.TrangThaiThanhToan == TrangThaiThanhToan.CapNhatThanhToan));

                foreach (var vacxin in resultViewModel.KhamSangLocTiemChung.YeuCauDichVuKyThuats.Where(x => x.TiemChung.TrangThaiTiemChung == TrangThaiTiemChung.DaTiemChung))
                {
                    vacxin.IsDaTiem = true;
                }

                var lstYeuCauGoiDichVuId = resultViewModel.KhamSangLocTiemChung.YeuCauDichVuKyThuats
                    .Where(x => x.YeuCauGoiDichVuId != null).Select(x => x.YeuCauGoiDichVuId.Value).Distinct().ToList();
                var lstTenGoiDichVu = await _tiemChungService.GetListTenGoiDichVu(lstYeuCauGoiDichVuId);

                foreach (var vacxin in resultViewModel.KhamSangLocTiemChung.YeuCauDichVuKyThuats.Where(x => x.YeuCauGoiDichVuId != null))
                {
                    vacxin.TenGoiDichVu = lstTenGoiDichVu
                        .Where(x => x.KeyId == vacxin.YeuCauGoiDichVuId.Value).Select(x => x.DisplayName)
                        .FirstOrDefault();
                }
            }

            /* Template khám sàng lọc */
            if (string.IsNullOrEmpty(resultViewModel.KhamSangLocTiemChung.ThongTinKhamSangLocTiemChungTemplate))
            {
                if (soThangTuoi >= 1)
                {
                    resultViewModel.KhamSangLocTiemChung.ThongTinKhamSangLocTiemChungTemplate = await _tiemChungService.GetTemplateKhamSangLocTiemChungAsync("KhamSangLocDoiTuongTrenMotThangTuoiNgoaiBenhVien");
                }
                else
                {
                    resultViewModel.KhamSangLocTiemChung.ThongTinKhamSangLocTiemChungTemplate = await _tiemChungService.GetTemplateKhamSangLocTiemChungAsync("KhamSangLocChungTreSoSinhNgoaiBenhVien");
                }
            }
            /* */

            ////dich vu khuyen mai
            //if ((result.YeuCauTiepNhan.BenhNhan.YeuCauGoiDichVus.Any() && result.YeuCauTiepNhan.BenhNhan.YeuCauGoiDichVus.Any(z => z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs.Any() ||
            //                                                                                                                       z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats.Any() ||
            //                                                                                                                       z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs.Any()))
            //    || (result.YeuCauTiepNhan.BenhNhan.YeuCauGoiDichVuSoSinhs.Any() && result.YeuCauTiepNhan.BenhNhan.YeuCauGoiDichVuSoSinhs.Any(z => z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs.Any() ||
            //                                                                                                                                      z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats.Any() ||
            //                                                                                                                                      z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs.Any())))
            //{
            //    resultViewModel.CoDichVuKhuyenMai = true;
            //}

            #region Cập nhật 4/4/2022
            if (result.YeuCauTiepNhan.BenhNhanId != null)
            {
                resultViewModel.CoDichVuKhuyenMai = await _tiemChungService.KiemTraCoDichVuKhuyenMaiAsync(result.YeuCauTiepNhan.BenhNhanId.Value);
            }

            if (resultViewModel.KhamSangLocTiemChung != null)
            {
                var lstYCDVKT = resultViewModel.KhamSangLocTiemChung.YeuCauDichVuKyThuats
                    .Where(x => x.TiemChung != null
                                && x.TiemChung.XuatKhoDuocPhamChiTietId != null
                                && string.IsNullOrEmpty(x.TiemChung.SoLoVacXinDisplay))
                    .Distinct().ToList();

                var lstXuatChiTietId = lstYCDVKT
                    .Select(x => x.TiemChung.XuatKhoDuocPhamChiTietId.Value)
                    .Distinct().ToList();

                if (lstXuatChiTietId.Any())
                {
                    var lstThongTinSoLo = await _tiemChungService.GetThongTinSoLoAsync(lstXuatChiTietId);
                    if (lstThongTinSoLo.Any())
                    {
                        foreach (var yckt in lstYCDVKT)
                        {
                            var lstSoLo = lstThongTinSoLo.Where(x => x.XuatKhoChiTietId == yckt.TiemChung.XuatKhoDuocPhamChiTietId).ToList();
                            yckt.TiemChung.SoLoVacXinDisplay = string.Join(", ", lstSoLo.Select(a => a.SoLo?.Trim()).Where(a => !string.IsNullOrEmpty(a)).Distinct().ToList());
                        }
                    }
                }
            }
            #endregion

            #region BVHD-3941
            if (resultViewModel.YeuCauTiepNhan.CoBaoHiemTuNhan == true)
            {
                resultViewModel.YeuCauTiepNhan.TenCongTyBaoHiemTuNhan = await _tiepNhanBenhNhanService.GetThongTinBaoHiemTuNhanAsync(result.YeuCauTiepNhanId);
            }
            #endregion

            /* Số dư tài khoản*/
            if (resultViewModel.NoiTruPhieuDieuTriId != null)
            {
                decimal soDuTk = await _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(resultViewModel.YeuCauTiepNhanId);
                //decimal chiPhiKhamChuaBenh = _thuNganNoiTruService.GetDanhSachChiPhiKhamChuaBenhChuaThu(resultViewModel.YeuCauTiepNhanId).Result.Select(o => o.BNConPhaiThanhToan).DefaultIfEmpty(0).Sum();
                decimal chiPhiKhamChuaBenh = _thuNganNoiTruService.GetSoTienBNConPhaiThanhToan(resultViewModel.YeuCauTiepNhanId).Result;

                resultViewModel.YeuCauTiepNhan.SoDuTaiKhoan = soDuTk;
                resultViewModel.YeuCauTiepNhan.SoDuTaiKhoanConLai = soDuTk - chiPhiKhamChuaBenh;
            }
            else
            {
                decimal soDuTk = await _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(resultViewModel.YeuCauTiepNhanId);
                decimal soDuUocLuongConLai = await _taiKhoanBenhNhanService.GetSoTienUocLuongConLai(resultViewModel.YeuCauTiepNhanId);

                resultViewModel.YeuCauTiepNhan.SoDuTaiKhoan = soDuTk;
                resultViewModel.YeuCauTiepNhan.SoDuTaiKhoanConLai = soDuUocLuongConLai;
            }
            /* */


            ////dich vu khuyen mai
            //if ((result.YeuCauTiepNhan.BenhNhan.YeuCauGoiDichVus.Any() && result.YeuCauTiepNhan.BenhNhan.YeuCauGoiDichVus.Any(z => z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs.Any() ||
            //                                                                          z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats.Any() ||
            //                                                                          z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs.Any()))
            // || (result.YeuCauTiepNhan.BenhNhan.YeuCauGoiDichVuSoSinhs.Any() && result.YeuCauTiepNhan.BenhNhan.YeuCauGoiDichVuSoSinhs.Any(z => z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs.Any() ||
            //                                                                           z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats.Any() ||
            //                                                                           z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs.Any())))
            //{
            //    resultViewModel.CoDichVuKhuyenMai = true;
            //}
            //var yeuCauKTNew = new YeuCauDichVuKyThuatViewModel();
            //foreach (var item in resultViewModel.YeuCauKhamBenh.YeuCauDichVuKyThuats.Where(p => p.DieuTriNgoaiTru == true && p.TrangThaiThanhToan != Enums.TrangThaiThanhToan.HuyThanhToan))
            //{
            //    yeuCauKTNew = new YeuCauDichVuKyThuatViewModel
            //    {
            //        Id = item.Id,
            //        TenDichVu = item.TenDichVu,
            //        MaDichVu = item.MaDichVu,
            //        MaGiaDichVu = item.MaGiaDichVu,
            //        NhomGiaDichVuKyThuatBenhVienId = item.NhomGiaDichVuKyThuatBenhVienId,
            //        Gia = item.Gia,
            //        NhomChiPhi = item.NhomChiPhi,
            //        LoaiDichVuKyThuat = item.LoaiDichVuKyThuat,
            //        NhomDichVuBenhVienId = item.NhomDichVuBenhVienId,
            //        DieuTriNgoaiTru = item.DieuTriNgoaiTru,
            //        DuocHuongBaoHiem = item.DuocHuongBaoHiem,
            //        DichVuKyThuatBenhVienId = item.DichVuKyThuatBenhVienId,
            //        SoLan = item.SoLan,
            //        ThoiDiemBatDauDieuTri = item.ThoiDiemBatDauDieuTri,
            //        TenDichVuHienThi = item.MaDichVu + " - " + item.TenDichVu,
            //        YeuCauKhamBenhId = item.YeuCauKhamBenhId,
            //        YeuCauTiepNhanId = item.YeuCauTiepNhanId,
            //        TenNhomDichVu = item.TenNhomDichVu,
            //        TrangThai = item.TrangThai,
            //        TrangThaiThanhToan = item.TrangThaiThanhToan,
            //        ThoiDiemChiDinh = item.ThoiDiemChiDinh,
            //        ThoiDiemDangKy = item.ThoiDiemDangKy,
            //        NhanVienChiDinhId = item.NhanVienChiDinhId
            //    };
            //}
            //resultViewModel.YeuCauKhamBenh.YeuCauDichVuKyThuat = yeuCauKTNew;
            //if (resultViewModel.YeuCauKhamBenh.YeuCauDichVuKyThuat.Id == 0)
            //{
            //    resultViewModel.YeuCauKhamBenh.CoDieuTriNgoaiTru = null;
            //}
            //var benhVienHienTai = await _yeuCauKhamBenhService.BenhVienHienTai();
            //if (benhVienHienTai != null)
            //{
            //    resultViewModel.YeuCauTiepNhan.BenhVienHienTai = benhVienHienTai.ToModel<BienVienViewModel>();
            //}
            //// get template khám theo dịch vụ khám
            //if (string.IsNullOrEmpty(resultViewModel.YeuCauKhamBenh.ThongTinKhamTheoDichVuTemplate))
            //{
            //    resultViewModel.YeuCauKhamBenh.ThongTinKhamTheoDichVuTemplate = await _khamBenhService.GetTemplateKhamBenhTheoDichVuKham(result.YeuCauKhamBenh.DichVuKhamBenhBenhVienId);
            //}

            //if (resultViewModel.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe)
            //{
            //    var lstDichVuKhamSucKhoe = await _yeuCauKhamBenhService.GetTemplateCacDichVuKhamSucKhoeAsync(resultViewModel.YeuCauTiepNhanId, resultViewModel.YeuCauKhamBenh.Id);
            //    resultViewModel.YeuCauKhamBenh.TemplateDichVuKhamSucKhoes.Add(new KhamBenhTemplateDichVuKhamSucKhoeViewModel()
            //    {
            //        YeuCauKhamBenhId = resultViewModel.YeuCauKhamBenhId.Value,
            //        ChuyenKhoaKhamSucKhoe = resultViewModel.YeuCauKhamBenh.ChuyenKhoaKhamSucKhoe,
            //        //TenChuyenKhoa = resultViewModel.YeuCauKhamBenh.ChuyenKhoaKhamSucKhoe.GetDescription(),
            //        ThongTinKhamTheoDichVuTemplate = resultViewModel.YeuCauKhamBenh.ThongTinKhamTheoDichVuTemplate,
            //        ThongTinKhamTheoDichVuData = resultViewModel.YeuCauKhamBenh.ThongTinKhamTheoDichVuData,
            //        TrangThai = resultViewModel.YeuCauKhamBenh.TrangThai,
            //        //IsDaKham = resultViewModel.YeuCauKhamBenh.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham && resultViewModel.YeuCauKhamBenh.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham,
            //        IsDungChuyenKhoaLogin = true
            //    });
            //    resultViewModel.YeuCauKhamBenh.TemplateDichVuKhamSucKhoes.AddRange(lstDichVuKhamSucKhoe.Select(item => new KhamBenhTemplateDichVuKhamSucKhoeViewModel()
            //    {
            //        YeuCauKhamBenhId = item.Id,
            //        ChuyenKhoaKhamSucKhoe = item.ChuyenKhoaKhamSucKhoe,
            //        //TenChuyenKhoa = item.ChuyenKhoaKhamSucKhoe.GetDescription(),
            //        ThongTinKhamTheoDichVuTemplate = item.ThongTinKhamTheoDichVuTemplate,
            //        ThongTinKhamTheoDichVuData = item.ThongTinKhamTheoDichVuData,
            //        TrangThai = item.TrangThai,
            //        IsDisabled = true
            //        //IsDaKham = item.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham && item.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham
            //    }));
            //    resultViewModel.YeuCauKhamBenh.TemplateDichVuKhamSucKhoes = resultViewModel.YeuCauKhamBenh.TemplateDichVuKhamSucKhoes.OrderBy(x => x.TenChuyenKhoa).ToList();
            //}


            ////get thông tin gói marketing của người bệnh
            //if (result.YeuCauTiepNhan.BenhNhan.YeuCauGoiDichVus.Any() || result.YeuCauTiepNhan.BenhNhan.YeuCauGoiDichVuSoSinhs.Any())
            //{
            //    var gridData = await _khamBenhService.GetGoiDichVuCuaBenhNhanDataForGridAsync(new QueryInfo
            //    {
            //        AdditionalSearchString = $"{result.YeuCauTiepNhan.BenhNhanId} ; false",
            //        Take = Int32.MaxValue
            //    });

            //    var lstGoi = gridData.Data.Select(p => (GoiDichVuTheoBenhNhanGridVo)p).ToList();
            //    if (lstGoi.Any())
            //    {
            //        resultViewModel.GoiDichVus = lstGoi;
            //    }
            //}


            //// get số dư toàn khoản
            //resultViewModel.YeuCauKhamBenh.SoDuTaiKhoan = await _taikhoanBenhNhanService.GetSoTienDaTamUngAsync(resultViewModel.YeuCauTiepNhanId);
            //resultViewModel.YeuCauKhamBenh.SoDuTaiKhoanConLai = await _taikhoanBenhNhanService.GetSoTienUocLuongConLai(resultViewModel.YeuCauTiepNhanId);
            //resultViewModel.YeuCauKhamBenh.MucTranChiPhi = _khamBenhService.GetMucTranChiPhi();
            return resultViewModel;
        }

        [HttpPost("GetYeuCauKhamTiemChungTiepTheo")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TiemChungKhamSangLoc, Enums.DocumentType.TiemChungThucHienTiem)]
        public async Task<YeuCauKhamTiemChungViewModel> GetYeuCauKhamTiemChungTiepTheo(HangDoiTiemChungQuyeryInfo queryInfo)
        {
            var result = await _tiemChungService.GetYeuCauKhamTiemChungTiepTheoAsyncVer3(queryInfo);

            if(result != null)
            {
                return result.ToModel<YeuCauKhamTiemChungViewModel>();
            }

            return null;
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
        #endregion

        #region Get list lookup
        [HttpPost("GetListNhanVien")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListNhanVienAsync([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _yeuCauLinhDuocPhamService.GetListNhanVienAsync(model);
            return Ok(lookup);
        }

        [HttpPost("GetListTrangThaiTiemVacxin")]
        public ActionResult<ICollection<Enums.LoaiDiUng>> GetListTrangThaiTiemVacxin()
        {
            var listEnum = _tiemChungService.GetListTrangThaiTiemVacxin();
            return Ok(listEnum);
        }

        [HttpPost("GetListNoiTheoDoiSauTiem")]
        public async Task<ActionResult<ICollection<LookupItemTemplateVo>>> GetListNoiTheoDoiSauTiemAsync([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _tiemChungService.GetListPhongBenhVienAsync(model);
            return Ok(lookup);
        }

        [HttpPost("GetListVacXin")]
        public async Task<ActionResult<ICollection<LookupItemTemplateVo>>> GetListVacXinAsync([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _tiemChungService.GetListVacXinAsync(model);
            return Ok(lookup);
        }

        [HttpGet("GetTemplateKhamSangLoc")]
        public async Task<ActionResult<string>> GetTemplateKhamSangLoc(long yeuCauTiepNhanId, NhomKhamSangLoc nhomKhamSangLoc)
        {
            var yeuCauTiepNhan = _tiemChungService.GetById(yeuCauTiepNhanId);

            var soThangTuoi = CalculateHelper.TinhTongSoThangCuaTuoi(yeuCauTiepNhan.NgaySinh, yeuCauTiepNhan.ThangSinh, yeuCauTiepNhan.NamSinh);

            var template = string.Empty;

            /* Template khám sàng lọc */
            if (soThangTuoi >= 1)
            {
                if(nhomKhamSangLoc == NhomKhamSangLoc.TrongBenhVien)
                {
                    template = await _tiemChungService.GetTemplateKhamSangLocTiemChungAsync("KhamSangLocDoiTuongTrenMotThangTuoiTrongBenhVien");
                }
                else if(nhomKhamSangLoc == NhomKhamSangLoc.NgoaiBenhVien)
                {
                    template = await _tiemChungService.GetTemplateKhamSangLocTiemChungAsync("KhamSangLocDoiTuongTrenMotThangTuoiNgoaiBenhVien");
                }
                else
                {
                    template = await _tiemChungService.GetTemplateKhamSangLocTiemChungAsync("KhamSangLocDoiTuongCovid");
                }
            }
            else
            {
                if (nhomKhamSangLoc == NhomKhamSangLoc.TrongBenhVien)
                {
                    template = await _tiemChungService.GetTemplateKhamSangLocTiemChungAsync("KhamSangLocChungTreSoSinhTrongBenhVien");
                }
                else if (nhomKhamSangLoc == NhomKhamSangLoc.NgoaiBenhVien)
                {
                    template = await _tiemChungService.GetTemplateKhamSangLocTiemChungAsync("KhamSangLocChungTreSoSinhNgoaiBenhVien");
                }
                else
                {
                    template = await _tiemChungService.GetTemplateKhamSangLocTiemChungAsync("KhamSangLocDoiTuongCovid");
                }
            }

            return Ok(template);
        }
        #endregion
    }
}
