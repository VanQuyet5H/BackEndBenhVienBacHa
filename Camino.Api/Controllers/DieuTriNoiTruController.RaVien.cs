
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.DieuTriNoiTru;
using Camino.Api.Models.Error;
using Camino.Api.Models.RaVien;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.NoiTruBenhAn;
using Camino.Core.Domain.ValueObject.RaVienNoiTru;
using Camino.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public partial class DieuTriNoiTruController
    {
        #region Thông tin chung ra viện

        private void PrepareTongKetBenhAn(RaVien model, long yeuCauTiepNhanId)
        {
            var entity = _dieuTriNoiTruService.GetById(yeuCauTiepNhanId
               , s => s.Include(u => u.NoiTruBenhAn).ThenInclude(p => p.NoiTruPhieuDieuTris).ThenInclude(p => p.ChanDoanChinhICD)
               .Include(u => u.BenhNhan).ThenInclude(p => p.NgheNghiep)

               .Include(u => u.NgheNghiep)
               .Include(u => u.DanToc)
               .Include(u => u.QuocTich)
               .Include(u => u.PhuongXa)
               .Include(u => u.QuanHuyen)
               .Include(u => u.TinhThanh)
               .Include(u => u.HinhThucDen)

               .Include(u => u.YeuCauNhapVien)

               .Include(u => u.NguoiLienHePhuongXa)
               .Include(u => u.NguoiLienHeQuanHuyen)
               .Include(u => u.NguoiLienHeTinhThanh)

               .Include(u => u.YeuCauDichVuGiuongBenhViens).ThenInclude(p => p.GiuongBenh)

               .Include(u => u.NoiTruBenhAn).ThenInclude(p => p.ChuyenDenBenhVien)

               .Include(u => u.NoiTruBenhAn).ThenInclude(p => p.NoiTruKhoaPhongDieuTris).ThenInclude(p => p.KhoaPhongChuyenDen)
               .Include(u => u.NoiTruBenhAn).ThenInclude(p => p.NhanVienTaoBenhAn).ThenInclude(p => p.User)
           );

            if (entity.NoiTruBenhAn.ThongTinTongKetBenhAn != null)
            {
                var tongKetBenhAn = entity.NoiTruBenhAn.ThongTinTongKetBenhAn != null ? JsonConvert.DeserializeObject<DieuTriNoiTruTongKetBenhAnViewModel>(entity.NoiTruBenhAn.ThongTinTongKetBenhAn)
                                                                                         : new DieuTriNoiTruTongKetBenhAnViewModel();
                if (tongKetBenhAn.DacDiemTreSoSinhs != null)
                {
                    if (tongKetBenhAn.DacDiemTreSoSinhs.Count == 1)
                    {
                        model.ChonThai = true; // true là đơn thai

                        model.ChonTraiGai = tongKetBenhAn.DacDiemTreSoSinhs[0].GioiTinhId == EnumGioiTinh.Trai;
                        model.ChonSongChet = tongKetBenhAn.DacDiemTreSoSinhs[0].TinhTrangId == EnumTrangThaiSong.Song;
                    }
                    else
                    {
                        model.ChonThai = false; // false là đa thai

                        model.ChonTrai = tongKetBenhAn.DacDiemTreSoSinhs.Any(c => c.GioiTinhId == EnumGioiTinh.Trai);
                        model.SoLuongTrai = tongKetBenhAn.DacDiemTreSoSinhs.Count(c => c.GioiTinhId == EnumGioiTinh.Trai);


                        model.ChonTraiSong = tongKetBenhAn.DacDiemTreSoSinhs.Where(c => c.GioiTinhId == EnumGioiTinh.Trai).Any(c => c.TinhTrangId == EnumTrangThaiSong.Song);
                        model.SoLuongTraiSong = tongKetBenhAn.DacDiemTreSoSinhs.Where(c => c.GioiTinhId == EnumGioiTinh.Trai).Count(c => c.TinhTrangId == EnumTrangThaiSong.Song);

                        model.ChonTraiChet = tongKetBenhAn.DacDiemTreSoSinhs.Where(c => c.GioiTinhId == EnumGioiTinh.Trai).Any(c => c.TinhTrangId == EnumTrangThaiSong.Chet);
                        model.SoLuongTraiChet = tongKetBenhAn.DacDiemTreSoSinhs.Where(c => c.GioiTinhId == EnumGioiTinh.Trai).Count(c => c.TinhTrangId == EnumTrangThaiSong.Chet);


                        model.ChonGai = tongKetBenhAn.DacDiemTreSoSinhs.Any(c => c.GioiTinhId == EnumGioiTinh.Gai);
                        model.SoLuongGai = tongKetBenhAn.DacDiemTreSoSinhs.Count(c => c.GioiTinhId == EnumGioiTinh.Gai);

                        model.ChonGaiSong = tongKetBenhAn.DacDiemTreSoSinhs.Where(c => c.GioiTinhId == EnumGioiTinh.Gai).Any(c => c.TinhTrangId == EnumTrangThaiSong.Song);
                        model.SoLuongGaiSong = tongKetBenhAn.DacDiemTreSoSinhs.Where(c => c.GioiTinhId == EnumGioiTinh.Gai).Count(c => c.TinhTrangId == EnumTrangThaiSong.Song);

                        model.ChonGaiChet = tongKetBenhAn.DacDiemTreSoSinhs.Where(c => c.GioiTinhId == EnumGioiTinh.Gai).Any(c => c.TinhTrangId == EnumTrangThaiSong.Chet);
                        model.SoLuongGaiChet = tongKetBenhAn.DacDiemTreSoSinhs.Where(c => c.GioiTinhId == EnumGioiTinh.Gai).Count(c => c.TinhTrangId == EnumTrangThaiSong.Chet);


                    }
                }
            }
        }

        #endregion

        #region Lấy thông tin ra viện nội khoa và khoa nhi

        [HttpGet("GetThongTinRaVienNoiKhoaNhiKhoa/{yeuCauTiepNhanId}")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public ActionResult<RaVien> GetThongTinRaVienNoiKhoaNhiKhoa(long yeuCauTiepNhanId)
        {
            var model = _dieuTriNoiTruService.GetRaVienNoiTruKhoaNoiKhoaNhi(yeuCauTiepNhanId);
            return Ok(model);
        }

        [HttpPost("LuuHoacCapNhatThongTinRaVienNoiKhoaNhiKhoa")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> LuuHoacCapNhatThongTinRaVienNoiKhoaNhiKhoa(RaVien model)
        {
            KiemTraChuanDoanKemTheo(model);

            await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(model.YeuCauTiepNhanId);

            _dieuTriNoiTruService.LuuHoacCapNhatRaVienNoiTruKhoaNoiKhoaNhi(model);
            return Ok();
        }

        [HttpPost("KetThucThongTinBenhAnNoiKhoa")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.DanhSachDieuTriNoiTru)]
        public ActionResult KetThucThongTinBenhAnNoiKhoa(RaVienVM model)
        {  
            KetThucBenhAnVo ketThucBenhAnVo = new KetThucBenhAnVo();
            ketThucBenhAnVo.GhiChuTaiKham = model.GhiChuTaiKham;
            ketThucBenhAnVo.HinhThucRaVien = (Enums.EnumHinhThucRaVien)model.HinhThucRaVienId;
            ketThucBenhAnVo.KetQuaDieuTri = (Enums.EnumKetQuaDieuTri)model.KetQuaDieuTriId;
            ketThucBenhAnVo.NgayHenTaiKham = model.NgayHienTaiKham;
            ketThucBenhAnVo.ThoiDiemRaVien = (DateTime)model.ThoiGianRaVien;
            ketThucBenhAnVo.YeuCauTiepNhanId = model.YeuCauTiepNhanId;

            _dieuTriNoiTruService.KetThucBenhAn(ketThucBenhAnVo);

            return Ok();
        }

        [HttpPost("MoThongTinBenhAnNoiKhoa")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.MoLaiBenhAn)]
        public ActionResult MoThongTinBenhAnNoiKhoa(RaVienNoiTruKhoaNoiKhoaNhi model)
        {
            _dieuTriNoiTruService.MoLaiBenhAn(model.YeuCauTiepNhanId);
            return Ok();
        }

        [HttpGet("GetThongTinThoiGianDieuTriBenhAnSoSinh")]
        public ActionResult<ThoiGianDieuTriSoSinhViewModel> GetThongTinThoiGianDieuTriBenhAnSoSinh(long noiTruBenhAnId)
        {
            var thongTinBenhAns = _dieuTriNoiTruService.GetById(noiTruBenhAnId, s => s.Include(i => i.NoiTruBenhAn).ThenInclude(c => c.NoiTruThoiGianDieuTriBenhAnSoSinhs));
            var thongTinThoiGianDTBAs = thongTinBenhAns.NoiTruBenhAn.NoiTruThoiGianDieuTriBenhAnSoSinhs.Select(c => new ThoiGianDieuTriSoSinhViewModel
            {
                Id = c.Id,
                GioBatDau = c.GioBatDau,
                GioKetThuc = c.GioKetThuc,
                GhiChuDieuTri = c.GhiChuDieuTri,
                NgayDieuTri = c.NgayDieuTri,
                NoiTruBenhAnId = c.NoiTruBenhAnId,
                NoiTruPhieuDieuTriId = c.NoiTruPhieuDieuTriId
            });
            return Ok(thongTinThoiGianDTBAs);
        }

        [HttpPost("UpdateThoiGianSoSinhRaVien")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> UpdateThoiGianSoSinhRaVien([FromBody] UpdateDieuTriSoSinhRaVienViewModel viewModel)
        {
            if (viewModel.ThoiGianDieuTriSoSinhRaVienViewModel.Any())
            {
                foreach (var item in viewModel.ThoiGianDieuTriSoSinhRaVienViewModel)
                {
                    var entity = await _dieuTriNoiTruService.GetYeuCauTiepNhanWithIncludeUpdate((long)item.NoiTruBenhAnId);

                    foreach (var itemThoiGianDieuTriSoSinhViewModel in item.ThoiGianDieuTriSoSinhViewModels)
                    {
                        KiemTraThoiGianDieuTriSoSinh(itemThoiGianDieuTriSoSinhViewModel, item.ThoiGianDieuTriSoSinhViewModels);
                    }

                    var phieuDieuTriUpdate = entity.NoiTruBenhAn.NoiTruPhieuDieuTris.First(p => p.Id == item.NoiTruPhieuDieuTriId);
                    item.ToEntity(phieuDieuTriUpdate);

                    _dieuTriNoiTruService.Update(entity);
                }
            }
            return Ok();
        }


        #endregion

        #region Lấy thông tin ra viện ngoại khoa thẩm mỹ

        [HttpGet("GetRaVienNoiTruNgoaiKhoaThamMy/{yeuCauTiepNhanId}")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public ActionResult<RaVien> GetRaVienNoiTruNgoaiKhoaThamMy(long yeuCauTiepNhanId)
        {
            var model = _dieuTriNoiTruService.GetRaVienNoiTruNgoaiKhoaThamMy(yeuCauTiepNhanId);
            return Ok(model);
        }

        [HttpPost("LuuHoacCapNhatRaVienNoiKhoaThamMy")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> LuuHoacCapNhatRaVienNoiKhoaThamMy(RaVien model)
        {
            KiemTraChuanDoanKemTheo(model);

            await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(model.YeuCauTiepNhanId);

            _dieuTriNoiTruService.LuuHoacCapNhatRaVienNoiKhoaThamMy(model);
            return Ok();
        }

        [HttpPost("KetThucThongTinBenhAnNgoaiKhoaThamMy")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.DanhSachDieuTriNoiTru)]
        public ActionResult KetThucThongTinBenhAnNgoaiKhoaThamMy(RaVienVM model)
        {          
            KetThucBenhAnVo ketThucBenhAnVo = new KetThucBenhAnVo();
            ketThucBenhAnVo.GhiChuTaiKham = model.GhiChuTaiKham;
            ketThucBenhAnVo.HinhThucRaVien = (Enums.EnumHinhThucRaVien)model.HinhThucRaVienId;
            ketThucBenhAnVo.KetQuaDieuTri = (Enums.EnumKetQuaDieuTri)model.KetQuaDieuTriId;
            ketThucBenhAnVo.NgayHenTaiKham = model.NgayHienTaiKham;
            ketThucBenhAnVo.ThoiDiemRaVien = (DateTime)model.ThoiGianRaVien;
            ketThucBenhAnVo.YeuCauTiepNhanId = model.YeuCauTiepNhanId;

            _dieuTriNoiTruService.KetThucBenhAn(ketThucBenhAnVo);

            return Ok();
        }

        [HttpPost("MoThongTinBenhAnNgoaiKhoaThamMy")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.MoLaiBenhAn)]
        public ActionResult MoThongTinBenhAnNgoaiKhoaThamMy(RaVienNoiTruKhoaNoiKhoaNhi model)
        {
            _dieuTriNoiTruService.MoLaiBenhAn(model.YeuCauTiepNhanId);
            return Ok();
        }

        #endregion

        #region Lấy thông tin ra viện phụ khoa

        [HttpGet("GetRaVienNoiTruPhuKhoa/{yeuCauTiepNhanId}")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public ActionResult<RaVien> GetRaVienNoiTruPhuKhoa(long yeuCauTiepNhanId)
        {
            var model = _dieuTriNoiTruService.GetRaVienNoiTruPhuKhoa(yeuCauTiepNhanId);
            return Ok(model);
        }

        [HttpPost("LuuHoacCapNhatRaVienNoiTruPhuKhoa")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> LuuHoacCapNhatRaVienNoiTruPhuKhoa(RaVien model)
        {
            KiemTraChuanDoanKemTheo(model);

            await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(model.YeuCauTiepNhanId);

            _dieuTriNoiTruService.LuuHoacCapNhatRaVienNoiTruPhuKhoa(model);
            return Ok();
        }

        [HttpPost("KiemTraThongTinKetThucBenhAn")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.DanhSachDieuTriNoiTru)]
        public ActionResult KiemTraThongTinKetThucBenhAn(RaVienVM model)
        {
            //BVHD-3781 Khoa không cho ra viện  
            var khoaKhongChoRaVienStr = _dieuTriNoiTruService.KiemTraKhoaKhongChoRaVien(model.YeuCauTiepNhanId);
            if (!string.IsNullOrEmpty(khoaKhongChoRaVienStr))
            {
                throw new ApiException(khoaKhongChoRaVienStr);
            }

            var errors = _dieuTriNoiTruService.KiemTraThongTinKetThucBenhAn(model.YeuCauTiepNhanId);
            return Ok(errors);
        }


        [HttpPost("KetThucThongTinBenhAnPhuKhoa")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.DanhSachDieuTriNoiTru)]
        public ActionResult KetThucThongTinBenhAnPhuKhoa(RaVienVM model)
        {  
            KetThucBenhAnVo ketThucBenhAnVo = new KetThucBenhAnVo();
            ketThucBenhAnVo.GhiChuTaiKham = model.GhiChuTaiKham;
            ketThucBenhAnVo.HinhThucRaVien = (Enums.EnumHinhThucRaVien)model.HinhThucRaVienId;
            ketThucBenhAnVo.KetQuaDieuTri = (Enums.EnumKetQuaDieuTri)model.KetQuaDieuTriId;
            ketThucBenhAnVo.NgayHenTaiKham = model.NgayHienTaiKham;
            ketThucBenhAnVo.ThoiDiemRaVien = (DateTime)model.ThoiGianRaVien;
            ketThucBenhAnVo.YeuCauTiepNhanId = model.YeuCauTiepNhanId;

            _dieuTriNoiTruService.KetThucBenhAn(ketThucBenhAnVo);

            return Ok();
        }

        [HttpPost("MoThongTinBenhAnPhuKhoa")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.MoLaiBenhAn)]
        public ActionResult MoThongTinBenhAnPhuKhoa(RaVienNoiTruKhoaNoiKhoaNhi model)
        {
            _dieuTriNoiTruService.MoLaiBenhAn(model.YeuCauTiepNhanId);
            return Ok();
        }

        #endregion

        #region Lấy thông tin ra viện sản khoa mổ

        [HttpGet("GetRaVienNoiTruSanKhoaMo/{yeuCauTiepNhanId}")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public ActionResult<RaVien> GetRaVienNoiTruSanKhoaMo(long yeuCauTiepNhanId)
        {
            var model = _dieuTriNoiTruService.GetRaVienNoiTruSanKhoaMo(yeuCauTiepNhanId);
            PrepareTongKetBenhAn(model, yeuCauTiepNhanId);
            return Ok(model);
        }

        [HttpPost("LuuHoacCapNhatRaVienNoiTruSanKhoaMo")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> LuuHoacCapNhatRaVienNoiTruSanKhoaMo(RaVien model)
        {
            KiemTraChuanDoanKemTheo(model);

            await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(model.YeuCauTiepNhanId);

            _dieuTriNoiTruService.LuuHoacCapNhatRaVienNoiTruSanKhoaMo(model);
            return Ok();
        }

        [HttpPost("KetThucThongTinBenhAnSanKhoaMo")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.DanhSachDieuTriNoiTru)]
        public ActionResult KetThucThongTinBenhAnSanKhoaMo(RaVienVM model)
        {
            KetThucBenhAnVo ketThucBenhAnVo = new KetThucBenhAnVo();
            ketThucBenhAnVo.GhiChuTaiKham = model.GhiChuTaiKham;
            ketThucBenhAnVo.HinhThucRaVien = (Enums.EnumHinhThucRaVien)model.HinhThucRaVienId;
            ketThucBenhAnVo.KetQuaDieuTri = (Enums.EnumKetQuaDieuTri)model.KetQuaDieuTriId;
            ketThucBenhAnVo.NgayHenTaiKham = model.NgayHienTaiKham;
            ketThucBenhAnVo.ThoiDiemRaVien = (DateTime)model.ThoiGianRaVien;
            ketThucBenhAnVo.YeuCauTiepNhanId = model.YeuCauTiepNhanId;

            _dieuTriNoiTruService.KetThucBenhAn(ketThucBenhAnVo);

            return Ok();
        }

        [HttpPost("MoThongTinBenhAnSanKhoaMo")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.MoLaiBenhAn)]
        public ActionResult MoThongTinBenhAnSanKhoaMo(RaVienNoiTruKhoaNoiKhoaNhi model)

        {
            _dieuTriNoiTruService.MoLaiBenhAn(model.YeuCauTiepNhanId);
            return Ok();
        }

        #endregion

        #region Lấy thông tin ra viện sản khoa thường

        [HttpPost("MoThongTinBenhAnSanKhoaThuong")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.MoLaiBenhAn)]
        public ActionResult MoThongTinBenhAnSanKhoaThuong(RaVienNoiTruKhoaNoiKhoaNhi model)
        {
            _dieuTriNoiTruService.MoLaiBenhAn(model.YeuCauTiepNhanId);
            return Ok();
        }

        [HttpGet("GetRaVienNoiTruSanKhoaThuong/{yeuCauTiepNhanId}")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public ActionResult<RaVien> GetRaVienNoiTruSanKhoaThuong(long yeuCauTiepNhanId)
        {
            var model = _dieuTriNoiTruService.GetRaVienNoiTruSanKhoaThuong(yeuCauTiepNhanId);
            PrepareTongKetBenhAn(model, yeuCauTiepNhanId);
            return Ok(model);
        }

        [HttpPost("LuuHoacCapNhatRaVienNoiTruSanKhoaThuong")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> LuuHoacCapNhatRaVienNoiTruSanKhoaThuong(RaVien raVienNoiTruSanKhoaThuong)
        {
            KiemTraChuanDoanKemTheo(raVienNoiTruSanKhoaThuong);

            await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(raVienNoiTruSanKhoaThuong.YeuCauTiepNhanId);

            _dieuTriNoiTruService.LuuHoacCapNhatRaVienNoiTruSanKhoaThuong(raVienNoiTruSanKhoaThuong);
            return Ok();
        }

        [HttpPost("KetThucThongTinBenhAnSanKhoaThuong")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.DanhSachDieuTriNoiTru)]
        public ActionResult KetThucThongTinBenhAnSanKhoaThuong(RaVienVM model)
        {         

            KetThucBenhAnVo ketThucBenhAnVo = new KetThucBenhAnVo();
            ketThucBenhAnVo.GhiChuTaiKham = model.GhiChuTaiKham;
            ketThucBenhAnVo.HinhThucRaVien = (Enums.EnumHinhThucRaVien)model.HinhThucRaVienId;
            ketThucBenhAnVo.KetQuaDieuTri = (Enums.EnumKetQuaDieuTri)model.KetQuaDieuTriId;
            ketThucBenhAnVo.NgayHenTaiKham = model.NgayHienTaiKham;
            ketThucBenhAnVo.ThoiDiemRaVien = (DateTime)model.ThoiGianRaVien;
            ketThucBenhAnVo.YeuCauTiepNhanId = model.YeuCauTiepNhanId;

            _dieuTriNoiTruService.KetThucBenhAn(ketThucBenhAnVo);

            return Ok();
        }

        #endregion Lấy thông tin ra viện sản khoa thường

        #region export

        [HttpPost("InPhieuRaVien")]
        [ClaimRequirement(SecurityOperation.Process, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> InPhieuRaVien(long yeuCauTiepNhanId, string hostingName)
        {
            var content = "";
            var dataBenhAnRaVien = new PhieuRaVienViewModel();

            var entity = await _dieuTriNoiTruService.GetByIdAsync(yeuCauTiepNhanId
                                                                    , s => s.Include(u => u.NoiTruBenhAn).ThenInclude(p => p.NoiTruPhieuDieuTris)
                                                                    .ThenInclude(p => p.ChanDoanChinhICD)
                                                                    .Include(u => u.BenhNhan).ThenInclude(p => p.NgheNghiep)
                                                                    .Include(u => u.NgheNghiep)
                                                                    .Include(u => u.NgheNghiepCuaMe)
                                                                    .Include(u => u.NgheNghiepCuaBo)
                                                                    .Include(u => u.DanToc)
                                                                    .Include(u => u.QuocTich)
                                                                    .Include(u => u.PhuongXa)
                                                                    .Include(u => u.QuanHuyen)
                                                                    .Include(u => u.TinhThanh)
                                                                    .Include(u => u.HinhThucDen)
                                                                    .Include(u => u.YeuCauNhapVien).ThenInclude(cc => cc.YeuCauTiepNhanMe).ThenInclude(cc => cc.NoiTruBenhAn)
                                                                    .Include(u => u.YeuCauNhapVien).ThenInclude(cc => cc.YeuCauTiepNhanMe).ThenInclude(cc => cc.NgheNghiep)
                                                                    .Include(u => u.NguoiLienHePhuongXa)
                                                                    .Include(u => u.NguoiLienHeQuanHuyen)
                                                                    .Include(u => u.NguoiLienHeTinhThanh)
                                                                    .Include(u => u.YeuCauDichVuGiuongBenhViens).ThenInclude(p => p.GiuongBenh)
                                                                    .Include(u => u.NoiTruBenhAn).ThenInclude(p => p.ChuyenDenBenhVien)
                                                                    .Include(u => u.NoiTruBenhAn).ThenInclude(p => p.ChanDoanChinhRaVienICD)
                                                                    .Include(u => u.NoiTruBenhAn).ThenInclude(p => p.NoiTruKhoaPhongDieuTris).ThenInclude(p => p.KhoaPhongChuyenDen)
                                                                    .Include(u => u.NoiTruBenhAn).ThenInclude(p => p.NhanVienTaoBenhAn).ThenInclude(p => p.User));

            if (entity == null)
            {
                throw new ApiException(_localizationService.GetResource("DieuTriNoiTru.PhieuRaVien.IsExists"));
            }

            LuuBenhAnTruocKhiNhapThongTin(entity.NoiTruBenhAn.LoaiBenhAn, yeuCauTiepNhanId, entity.NoiTruBenhAn, entity);

            #region Lưu thông tin tao phiếu in (Thông tin bệnh án , tổng kết , ra viện)

            #endregion

            #region THÔNG TIN NỘI TRÚ BỆNH ÁN (Thông tin bệnh án , thông tin tổng kết bệnh án, Ra viện)

            var thongTinBenhAn = entity.NoiTruBenhAn.ThongTinBenhAn != null ? JsonConvert.DeserializeObject<ThongTinBenhAn>(entity.NoiTruBenhAn.ThongTinBenhAn) : new ThongTinBenhAn();
            var tongKetBenhAn = entity.NoiTruBenhAn.ThongTinTongKetBenhAn != null ? JsonConvert.DeserializeObject<DieuTriNoiTruTongKetBenhAnViewModel>(entity.NoiTruBenhAn.ThongTinTongKetBenhAn) : new DieuTriNoiTruTongKetBenhAnViewModel();
            var thongTinRaVien = entity.NoiTruBenhAn.ThongTinRaVien != null ? JsonConvert.DeserializeObject<RaVien>(entity.NoiTruBenhAn.ThongTinRaVien) : new RaVien();

            #endregion

            #region TEMPLATE TỪNG BỆNH ÁN

            var html = _danhSachChoKhamService.GetBodyByName("BenhAnNoiKhoa");

            if (entity.NoiTruBenhAn.LoaiBenhAn == LoaiBenhAn.PhuKhoa)
            {
                html = _danhSachChoKhamService.GetBodyByName("BenhAnPhuKhoa");
            }
            else if (entity.NoiTruBenhAn.LoaiBenhAn == LoaiBenhAn.NgoaiKhoa || entity.NoiTruBenhAn.LoaiBenhAn == LoaiBenhAn.ThamMy)
            {
                html = _danhSachChoKhamService.GetBodyByName("BenhAnNgoaiKhoa");
            }
            else if (entity.NoiTruBenhAn.LoaiBenhAn == LoaiBenhAn.SanKhoaMo || entity.NoiTruBenhAn.LoaiBenhAn == LoaiBenhAn.SanKhoaThuong)
            {
                html = _danhSachChoKhamService.GetBodyByName("BenhAnSanKhoa");
            }
            else if (entity.NoiTruBenhAn.LoaiBenhAn == LoaiBenhAn.NhiKhoa)
            {
                html = _danhSachChoKhamService.GetBodyByName("BenhAnNhiKhoa");
            }
            else if (entity.NoiTruBenhAn.LoaiBenhAn == LoaiBenhAn.TreSoSinh)
            {
                html = _danhSachChoKhamService.GetBodyByName("BenhAnTreSoSinh");
            }

            #endregion

            #region XỬ LÝ THÔNG TIN

            PrepareThongTinHanhChinh(dataBenhAnRaVien, entity, hostingName);

            PrepareThongTinQuanLyNguoiBenh(dataBenhAnRaVien, entity, thongTinRaVien);

            PrepareThongTinChuanDoanNguoiBenh(dataBenhAnRaVien, entity, thongTinRaVien);

            PrepareThongTinhTrangRaVien(dataBenhAnRaVien, entity, thongTinRaVien);

            PrepareThongTinDungChung(dataBenhAnRaVien, entity, thongTinRaVien, thongTinBenhAn, tongKetBenhAn);

            #endregion

            //cập nhật data muốn xuống hàng theo y khách hàng
            var dataJson = Newtonsoft.Json.JsonConvert.SerializeObject(dataBenhAnRaVien);
            var replaceData = dataJson.Replace("\\n", "<br/>");
            var newData = JsonConvert.DeserializeObject<PhieuRaVienViewModel>(replaceData);
            //cập nhật data muốn xuống hàng theo y khách hàng

            content = TemplateHelpper.FormatTemplateWithContentTemplate(html, newData);


            return Ok(content);
        }

        private PhieuRaVienViewModel PrepareThongTinHanhChinh(PhieuRaVienViewModel phieuRaVienViewModel, YeuCauTiepNhan yeuCauTiepNhan, string hostingName)
        {
            phieuRaVienViewModel.HoTen = yeuCauTiepNhan.HoTen;
            phieuRaVienViewModel.LogoUrl = hostingName + "/assets/img/logo-bacha-full.png";
            phieuRaVienViewModel.MaVaoVien = yeuCauTiepNhan.NoiTruBenhAn.SoBenhAn;
            phieuRaVienViewModel.SoLuuTru = yeuCauTiepNhan.NoiTruBenhAn.SoLuuTru;
            phieuRaVienViewModel.HoTen = yeuCauTiepNhan.HoTen;
            phieuRaVienViewModel.NamSinh = yeuCauTiepNhan.NamSinh + "";
            phieuRaVienViewModel.Nam = yeuCauTiepNhan.GioiTinh == LoaiGioiTinh.GioiTinhNam ? ChonThongTin : null;
            phieuRaVienViewModel.Nu = yeuCauTiepNhan.GioiTinh == LoaiGioiTinh.GioiTinhNu ? ChonThongTin : null;
            phieuRaVienViewModel.DiaChi = yeuCauTiepNhan.DiaChiDayDu;
            phieuRaVienViewModel.NgheNghiep = yeuCauTiepNhan.NgheNghiep?.Ten;

            phieuRaVienViewModel.KhoaPhong = yeuCauTiepNhan.NoiTruBenhAn.NoiTruKhoaPhongDieuTris != null ? yeuCauTiepNhan.NoiTruBenhAn.NoiTruKhoaPhongDieuTris
                                                                                                                         .Last().KhoaPhongChuyenDen.Ten
                                                                                                                         .Replace("Khoa", string.Empty).ToLower() : KhongCoDuLieu;
            phieuRaVienViewModel.Giuong = yeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Any(p => p.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan) ? yeuCauTiepNhan
                                                        .YeuCauDichVuGiuongBenhViens.Last(p => p.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan)
                                                        .GiuongBenh.Ten.ToLower() : KhongCoDuLieu;
            phieuRaVienViewModel.HoTenBenhNhan = yeuCauTiepNhan.HoTen.ToUpper();

            phieuRaVienViewModel.SNg1 = yeuCauTiepNhan.NgaySinh != 0 ?
                                       (yeuCauTiepNhan.NgaySinh + "").Select(cc => cc.ToString()).ToArray().Length == 1 ? "0" :
                                       (yeuCauTiepNhan.NgaySinh + "").Select(cc => cc.ToString()).ToArray()[0] : KhongCoDuLieu;

            phieuRaVienViewModel.SNg2 = yeuCauTiepNhan.NgaySinh != 0 ? (yeuCauTiepNhan.NgaySinh + "").Select(cc => cc.ToString()).ToArray().Length == 1 ?
                                                                       (yeuCauTiepNhan.NgaySinh + "").Select(cc => cc.ToString()).ToArray()[0] :
                                                                       (yeuCauTiepNhan.NgaySinh + "").Select(cc => cc.ToString()).ToArray().Length != 1 ?
                                                                       (yeuCauTiepNhan.NgaySinh + "").Select(cc => cc.ToString()).ToArray()[1] : KhongCoDuLieu : KhongCoDuLieu;

            phieuRaVienViewModel.STh1 = yeuCauTiepNhan.ThangSinh != 0 ? (yeuCauTiepNhan.ThangSinh + "").Select(cc => cc.ToString()).ToArray().Length == 1 ? "0" :
                                                                        (yeuCauTiepNhan.ThangSinh + "").Select(cc => cc.ToString()).ToArray()[0] : KhongCoDuLieu;

            phieuRaVienViewModel.STh2 = yeuCauTiepNhan.ThangSinh != 0 ? (yeuCauTiepNhan.ThangSinh + "").Select(cc => cc.ToString()).ToArray().Length == 1 ?
                                                                        (yeuCauTiepNhan.ThangSinh + "").Select(cc => cc.ToString()).ToArray()[0] :
                                                                        (yeuCauTiepNhan.ThangSinh + "").Select(cc => cc.ToString()).ToArray().Length != 1 ?
                                                                        (yeuCauTiepNhan.ThangSinh + "").Select(cc => cc.ToString()).ToArray()[1] : KhongCoDuLieu : KhongCoDuLieu;


            phieuRaVienViewModel.Sn1 = yeuCauTiepNhan.NamSinh != null ? (yeuCauTiepNhan.NamSinh + "").Select(cc => cc.ToString()).ToArray()[0] : KhongCoDuLieu;
            phieuRaVienViewModel.Sn2 = yeuCauTiepNhan.NamSinh != null ? (yeuCauTiepNhan.NamSinh + "").Select(cc => cc.ToString()).ToArray()[1] : KhongCoDuLieu;
            phieuRaVienViewModel.Sn3 = yeuCauTiepNhan.NamSinh != null ? (yeuCauTiepNhan.NamSinh + "").Select(cc => cc.ToString()).ToArray()[2] : KhongCoDuLieu;
            phieuRaVienViewModel.Sn4 = yeuCauTiepNhan.NamSinh != null ? (yeuCauTiepNhan.NamSinh + "").Select(cc => cc.ToString()).ToArray()[3] : KhongCoDuLieu;

            phieuRaVienViewModel.T1 = ((DateTime.Now.Year - yeuCauTiepNhan.NamSinh) + "").Select(cc => cc.ToString()).ToArray()[0];
            phieuRaVienViewModel.T2 = ((DateTime.Now.Year - yeuCauTiepNhan.NamSinh) + "").Select(cc => cc.ToString()).ToArray().Length > 1 ?
                                       ((DateTime.Now.Year - yeuCauTiepNhan.NamSinh) + "").Select(cc => cc.ToString()).ToArray()[1] : KhongCoDuLieu;

            phieuRaVienViewModel.Nam1 = yeuCauTiepNhan.GioiTinh == LoaiGioiTinh.GioiTinhNam ? ChonThongTin : null;
            phieuRaVienViewModel.Nu2 = yeuCauTiepNhan.GioiTinh == LoaiGioiTinh.GioiTinhNu ? ChonThongTin : null;
            phieuRaVienViewModel.DanToc = yeuCauTiepNhan.DanToc != null ? yeuCauTiepNhan.DanToc.Ten : KhongCoDuLieu;
            phieuRaVienViewModel.QuocTich = yeuCauTiepNhan.QuocTich != null ? yeuCauTiepNhan.QuocTich.Ten : KhongCoDuLieu;
            phieuRaVienViewModel.SoNha = yeuCauTiepNhan.DiaChi != null ? yeuCauTiepNhan.DiaChi : KhongCoDuLieu;
            phieuRaVienViewModel.XaPhuong = yeuCauTiepNhan.PhuongXa != null ? yeuCauTiepNhan.PhuongXa.Ten : KhongCoDuLieu;
            phieuRaVienViewModel.Huyen = yeuCauTiepNhan.QuanHuyen != null ? yeuCauTiepNhan.QuanHuyen.Ten : KhongCoDuLieu;
            phieuRaVienViewModel.TinhThanhPho = yeuCauTiepNhan.TinhThanh != null ? yeuCauTiepNhan.TinhThanh.Ten : KhongCoDuLieu;

            phieuRaVienViewModel.NoiLamViec = yeuCauTiepNhan.NoiLamViec;
            phieuRaVienViewModel.HCBHYT = yeuCauTiepNhan.CoBHYT == true ? ChonThongTin : null;
            phieuRaVienViewModel.HCThuPhi = yeuCauTiepNhan.CoBHYT != true ? ChonThongTin : null;

            phieuRaVienViewModel.HCMien = null;
            phieuRaVienViewModel.HCKhac = null;

            phieuRaVienViewModel.BHYTNgayHetHan = yeuCauTiepNhan.BHYTNgayHetHan != null ? (yeuCauTiepNhan.BHYTNgayHetHan ?? DateTime.Now).ApplyFormatDate() : KhongCoDuLieu;

            phieuRaVienViewModel.BHYT1 = yeuCauTiepNhan.BHYTMaSoThe != null ?
                (yeuCauTiepNhan.BHYTMaSoThe.Select(cc => cc.ToString()).ToArray().Length > 1 ? yeuCauTiepNhan.BHYTMaSoThe.Select(cc => cc.ToString()).ToArray()[0] : KhongCoDuLieu)
                + (yeuCauTiepNhan.BHYTMaSoThe.Select(cc => cc.ToString()).ToArray()[1].Length > 2 ? yeuCauTiepNhan.BHYTMaSoThe.Select(cc => cc.ToString()).ToArray()[1] : KhongCoDuLieu) : KhongCoDuLieu;

            phieuRaVienViewModel.BHYT2 = yeuCauTiepNhan.BHYTMaSoThe != null && yeuCauTiepNhan.BHYTMaSoThe.Select(cc => cc.ToString()).ToArray().Length > 3
                                                                             ? yeuCauTiepNhan.BHYTMaSoThe.Select(cc => cc.ToString()).ToArray()[2] : KhongCoDuLieu;

            phieuRaVienViewModel.BHYT3 = yeuCauTiepNhan.BHYTMaSoThe != null ? (yeuCauTiepNhan.BHYTMaSoThe.Select(cc => cc.ToString()).ToArray().Length > 4 ?
                                                                               yeuCauTiepNhan.BHYTMaSoThe.Select(cc => cc.ToString()).ToArray()[3] : KhongCoDuLieu)
                                                                            + (yeuCauTiepNhan.BHYTMaSoThe.Select(cc => cc.ToString()).ToArray().Length > 5 ?
                                                                               yeuCauTiepNhan.BHYTMaSoThe.Select(cc => cc.ToString()).ToArray()[4] : KhongCoDuLieu) : KhongCoDuLieu;

            phieuRaVienViewModel.BHYT4 = yeuCauTiepNhan.BHYTMaSoThe != null ? (yeuCauTiepNhan.BHYTMaSoThe.Select(cc => cc.ToString()).ToArray().Length > 6 ?
                                                                               yeuCauTiepNhan.BHYTMaSoThe.Select(cc => cc.ToString()).ToArray()[5] : KhongCoDuLieu)
                                        + (yeuCauTiepNhan.BHYTMaSoThe.Select(cc => cc.ToString()).ToArray().Length > 7 ? yeuCauTiepNhan.BHYTMaSoThe.Select(cc => cc.ToString()).ToArray()[6] : KhongCoDuLieu)
                                        + (yeuCauTiepNhan.BHYTMaSoThe.Select(cc => cc.ToString()).ToArray().Length > 8 ? yeuCauTiepNhan.BHYTMaSoThe.Select(cc => cc.ToString()).ToArray()[7] : KhongCoDuLieu) : KhongCoDuLieu;

            phieuRaVienViewModel.BHYT5 = yeuCauTiepNhan.BHYTMaSoThe != null
                    ? (yeuCauTiepNhan.BHYTMaSoThe.Select(cc => cc.ToString()).ToArray().Length > 9 ? yeuCauTiepNhan.BHYTMaSoThe.Select(cc => cc.ToString()).ToArray()[8] : KhongCoDuLieu)
                    + (yeuCauTiepNhan.BHYTMaSoThe.Select(cc => cc.ToString()).ToArray().Length > 10 ? yeuCauTiepNhan.BHYTMaSoThe.Select(cc => cc.ToString()).ToArray()[9] : KhongCoDuLieu)
                    + (yeuCauTiepNhan.BHYTMaSoThe.Select(cc => cc.ToString()).ToArray().Length > 11 ? yeuCauTiepNhan.BHYTMaSoThe.Select(cc => cc.ToString()).ToArray()[10] : KhongCoDuLieu)
                    + (yeuCauTiepNhan.BHYTMaSoThe.Select(cc => cc.ToString()).ToArray().Length > 12 ? yeuCauTiepNhan.BHYTMaSoThe.Select(cc => cc.ToString()).ToArray()[11] : KhongCoDuLieu)
                    + (yeuCauTiepNhan.BHYTMaSoThe.Select(cc => cc.ToString()).ToArray().Length > 13 ? yeuCauTiepNhan.BHYTMaSoThe.Select(cc => cc.ToString()).ToArray()[12] : KhongCoDuLieu)
                    + (yeuCauTiepNhan.BHYTMaSoThe.Select(cc => cc.ToString()).ToArray().Length > 14 ? yeuCauTiepNhan.BHYTMaSoThe.Select(cc => cc.ToString()).ToArray()[13] : KhongCoDuLieu)
                    + (yeuCauTiepNhan.BHYTMaSoThe.Select(cc => cc.ToString()).ToArray().Length > 15 ? yeuCauTiepNhan.BHYTMaSoThe.Select(cc => cc.ToString()).ToArray()[14] : KhongCoDuLieu)
                    : KhongCoDuLieu;

            phieuRaVienViewModel.NguoiLienHeQuanHeThanNhan = yeuCauTiepNhan.NguoiLienHeHoTen
                    + (yeuCauTiepNhan.NguoiLienHeDiaChi != null ? ", " + yeuCauTiepNhan.NguoiLienHeDiaChi : KhongCoDuLieu)
                    + (yeuCauTiepNhan.NguoiLienHeTinhThanh != null ? ", " + yeuCauTiepNhan.NguoiLienHeTinhThanh.Ten : KhongCoDuLieu)
                    + (yeuCauTiepNhan.NguoiLienHeQuanHuyen != null ? ", " + yeuCauTiepNhan.NguoiLienHeQuanHuyen.Ten : KhongCoDuLieu)
                    + (yeuCauTiepNhan.NguoiLienHePhuongXa != null ? ", " + yeuCauTiepNhan.NguoiLienHePhuongXa.Ten : KhongCoDuLieu);

            phieuRaVienViewModel.SoDienThoai = yeuCauTiepNhan.NguoiLienHeSoDienThoai;

            var lstMaNgheNghiep = yeuCauTiepNhan.NgheNghiep != null
               ? yeuCauTiepNhan.NgheNghiep.TenVietTat.Select(cc => cc.ToString()).ToArray().ToList() : new List<string>();

            var nn = string.Empty;
            foreach (var item in lstMaNgheNghiep)
            {
                nn += "<span class=\"square\">" + item + "</span>";
            }

            phieuRaVienViewModel.NN = nn;

            var lstMaDanToc = yeuCauTiepNhan.DanToc != null ? yeuCauTiepNhan.DanToc.Ma.Select(cc => cc.ToString()).ToArray().ToList()
                                                            : new List<string>();

            var dt = string.Empty;
            foreach (var item in lstMaDanToc)
            {
                dt += "<span class=\"square\">" + item + "</span>";
            }

            phieuRaVienViewModel.DT = dt;

            var lstMaQuocTich = yeuCauTiepNhan.QuocTich != null
                ? yeuCauTiepNhan.QuocTich.Ma.Select(cc => cc.ToString()).ToArray().ToList() : new List<string>();
            var nk = string.Empty;
            foreach (var item in lstMaQuocTich)
            {
                nk += "<span class=\"square\">" + item + "</span>";
            }
            phieuRaVienViewModel.NK = nk;

            var lstMaQuanHuyen = yeuCauTiepNhan.QuanHuyen != null
                ? yeuCauTiepNhan.QuanHuyen.Ma.Select(cc => cc.ToString()).ToArray().ToList() : new List<string>();
            var h = string.Empty;
            foreach (var item in lstMaQuanHuyen)
            {
                h += "<span class=\"square\">" + item + "</span>";
            }
            phieuRaVienViewModel.H = h;


            var lstMaTinhThanhPho = yeuCauTiepNhan.TinhThanh != null ?
                                    yeuCauTiepNhan.TinhThanh.Ma.Select(cc => cc.ToString()).ToArray().ToList() :
                                    new List<string>();
            var ttp = string.Empty;
            foreach (var item in lstMaTinhThanhPho)
            {
                ttp += "<span class=\"square\">" + item + "</span>";
            }
            phieuRaVienViewModel.TTP = ttp;



            //yêu cầu tiếp nhận của mẹ
            var yeuCauTiepNhanMe = yeuCauTiepNhan?.YeuCauNhapVien.YeuCauTiepNhanMe;
            if (yeuCauTiepNhanMe != null)
            {
                phieuRaVienViewModel.HoTenMe = yeuCauTiepNhanMe.HoTen;
                phieuRaVienViewModel.NgheNghiepMe = yeuCauTiepNhanMe?.NgheNghiep?.Ten;
                var lstMaNgheNghiepMe = yeuCauTiepNhanMe.NgheNghiep != null
                   ? yeuCauTiepNhanMe.NgheNghiep.TenVietTat.Select(cc => cc.ToString()).ToArray().ToList() : new List<string>();

                var nnMe = string.Empty;
                foreach (var item in lstMaNgheNghiep)
                {
                    nnMe += "<span class=\"square\">" + item + "</span>";
                }

                phieuRaVienViewModel.NNMe = nnMe;

                phieuRaVienViewModel.MeSNg1 = yeuCauTiepNhanMe.NgaySinh != 0 ?
                                     (yeuCauTiepNhanMe.NgaySinh + "").Select(cc => cc.ToString()).ToArray().Length == 1 ? "0" :
                                     (yeuCauTiepNhanMe.NgaySinh + "").Select(cc => cc.ToString()).ToArray()[0] : KhongCoDuLieu;

                phieuRaVienViewModel.MeSNg2 = yeuCauTiepNhanMe.NgaySinh != 0 ? (yeuCauTiepNhanMe.NgaySinh + "").Select(cc => cc.ToString()).ToArray().Length == 1 ?
                                                                           (yeuCauTiepNhanMe.NgaySinh + "").Select(cc => cc.ToString()).ToArray()[0] :
                                                                           (yeuCauTiepNhanMe.NgaySinh + "").Select(cc => cc.ToString()).ToArray().Length != 1 ?
                                                                           (yeuCauTiepNhanMe.NgaySinh + "").Select(cc => cc.ToString()).ToArray()[1] : KhongCoDuLieu : KhongCoDuLieu;

                phieuRaVienViewModel.MeSTh1 = yeuCauTiepNhanMe.ThangSinh != 0 ? (yeuCauTiepNhanMe.ThangSinh + "").Select(cc => cc.ToString()).ToArray().Length == 1 ? "0" :
                                                                            (yeuCauTiepNhanMe.ThangSinh + "").Select(cc => cc.ToString()).ToArray()[0] : KhongCoDuLieu;

                phieuRaVienViewModel.MeSTh2 = yeuCauTiepNhanMe.ThangSinh != 0 ? (yeuCauTiepNhanMe.ThangSinh + "").Select(cc => cc.ToString()).ToArray().Length == 1 ?
                                                                            (yeuCauTiepNhanMe.ThangSinh + "").Select(cc => cc.ToString()).ToArray()[0] :
                                                                            (yeuCauTiepNhanMe.ThangSinh + "").Select(cc => cc.ToString()).ToArray().Length != 1 ?
                                                                            (yeuCauTiepNhanMe.ThangSinh + "").Select(cc => cc.ToString()).ToArray()[1] : KhongCoDuLieu : KhongCoDuLieu;


                phieuRaVienViewModel.MeSn1 = yeuCauTiepNhanMe.NamSinh != null ? (yeuCauTiepNhanMe.NamSinh + "").Select(cc => cc.ToString()).ToArray()[0] : KhongCoDuLieu;
                phieuRaVienViewModel.MeSn2 = yeuCauTiepNhanMe.NamSinh != null ? (yeuCauTiepNhanMe.NamSinh + "").Select(cc => cc.ToString()).ToArray()[1] : KhongCoDuLieu;
                phieuRaVienViewModel.MeSn3 = yeuCauTiepNhanMe.NamSinh != null ? (yeuCauTiepNhanMe.NamSinh + "").Select(cc => cc.ToString()).ToArray()[2] : KhongCoDuLieu;
                phieuRaVienViewModel.MeSn4 = yeuCauTiepNhanMe.NamSinh != null ? (yeuCauTiepNhanMe.NamSinh + "").Select(cc => cc.ToString()).ToArray()[3] : KhongCoDuLieu;

                phieuRaVienViewModel.MeT1 = ((DateTime.Now.Year - yeuCauTiepNhanMe.NamSinh) + "").Select(cc => cc.ToString()).ToArray()[0];
                phieuRaVienViewModel.MeT2 = ((DateTime.Now.Year - yeuCauTiepNhanMe.NamSinh) + "").Select(cc => cc.ToString()).ToArray().Length > 1 ?
                                           ((DateTime.Now.Year - yeuCauTiepNhanMe.NamSinh) + "").Select(cc => cc.ToString()).ToArray()[1] : KhongCoDuLieu;
            }


            phieuRaVienViewModel.HoTenBo = yeuCauTiepNhan.HoTenBo;
            phieuRaVienViewModel.NgheNghiepBo = yeuCauTiepNhan?.NgheNghiepCuaBo?.Ten;
            var lstMaNgheNghiepBo = yeuCauTiepNhan.NgheNghiepCuaBo != null
               ? yeuCauTiepNhan.NgheNghiepCuaBo.TenVietTat.Select(cc => cc.ToString()).ToArray().ToList() : new List<string>();



            var nnBo = string.Empty;
            foreach (var item in lstMaNgheNghiepBo)
            {
                nnBo += "<span class=\"square\">" + item + "</span>";
            }

            phieuRaVienViewModel.NNMe = nnBo;

            phieuRaVienViewModel.NhomMauMe = yeuCauTiepNhanMe?.NhomMau.GetDescription();

            phieuRaVienViewModel.TTVHBo = yeuCauTiepNhan.TrinhDoVanHoaCuaBo;
            phieuRaVienViewModel.TTVHMe = yeuCauTiepNhan.TrinhDoVanHoaCuaMe;


            phieuRaVienViewModel.TTVHBo = yeuCauTiepNhan.TrinhDoVanHoaCuaBo;
            phieuRaVienViewModel.TTVHMe = yeuCauTiepNhan.TrinhDoVanHoaCuaMe;

            return phieuRaVienViewModel;
        }
        private PhieuRaVienViewModel PrepareThongTinQuanLyNguoiBenh(PhieuRaVienViewModel phieuRaVienViewModel, YeuCauTiepNhan yeuCauTiepNhan, RaVien thongTinRaVien)
        {
            var vaoKhoaFirst = yeuCauTiepNhan.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.OrderBy(cc => cc.ThoiDiemVaoKhoa).FirstOrDefault();

            phieuRaVienViewModel.Gio = yeuCauTiepNhan.NoiTruBenhAn.ThoiDiemNhapVien.Hour + KhongCoDuLieu;
            phieuRaVienViewModel.Phut = yeuCauTiepNhan.NoiTruBenhAn.ThoiDiemNhapVien.Minute + KhongCoDuLieu;
            phieuRaVienViewModel.Ngay = yeuCauTiepNhan.NoiTruBenhAn.ThoiDiemNhapVien.ApplyFormatDate();
            phieuRaVienViewModel.CQYT = yeuCauTiepNhan.HinhThucDen?.Ten == "Chuyển tuyến" ? ChonThongTin : null;
            phieuRaVienViewModel.TD = yeuCauTiepNhan.HinhThucDen?.Ten == "Tự đến" ? ChonThongTin : null;
            phieuRaVienViewModel.QLK = yeuCauTiepNhan.HinhThucDen?.Ten != "Tự đến" && yeuCauTiepNhan.HinhThucDen?.Ten != "Chuyển tuyến" ? ChonThongTin : null;

            phieuRaVienViewModel.GioRV = thongTinRaVien.ThoiGianRaVien != null ? thongTinRaVien.ThoiGianRaVien.GetValueOrDefault().Hour + "" : KhongCoDuLieu;
            phieuRaVienViewModel.PhutRV = thongTinRaVien.ThoiGianRaVien != null ? thongTinRaVien.ThoiGianRaVien.GetValueOrDefault().Minute + "" : KhongCoDuLieu;
            phieuRaVienViewModel.NgayRV = thongTinRaVien.ThoiGianRaVien != null ? thongTinRaVien.ThoiGianRaVien.GetValueOrDefault().ApplyFormatDate() + "" : KhongCoDuLieu;

            phieuRaVienViewModel.RaVien = thongTinRaVien.HinhThucRaVienId == EnumHinhThucRaVien.RaVien
                || thongTinRaVien.HinhThucRaVienId == EnumHinhThucRaVien.CapCuuRaVienTrongNgay ? ChonThongTin : null;
            phieuRaVienViewModel.XinVe = thongTinRaVien.HinhThucRaVienId == EnumHinhThucRaVien.XinVe
                || thongTinRaVien.HinhThucRaVienId == EnumHinhThucRaVien.NangXinVe ? ChonThongTin : null;
            phieuRaVienViewModel.BoVe = thongTinRaVien.HinhThucRaVienId == EnumHinhThucRaVien.BoVe ? ChonThongTin : null;
            phieuRaVienViewModel.DuaVe = thongTinRaVien.HinhThucRaVienId == EnumHinhThucRaVien.TuVong
                || thongTinRaVien.HinhThucRaVienId == EnumHinhThucRaVien.TuVongTruoc24H ? ChonThongTin : null;

            phieuRaVienViewModel.TongSoNgayDieuTri = null;
            phieuRaVienViewModel.LanThu = yeuCauTiepNhan.NoiTruBenhAn.SoLanVaoVienDoBenhNay;


            var soNgayDieuTriBanDau = ((vaoKhoaFirst.ThoiDiemRaKhoa ?? DateTime.Now) - vaoKhoaFirst.ThoiDiemVaoKhoa).Days + 1 + "";
            if (soNgayDieuTriBanDau.Length > 1)
            {
                var soNgay = soNgayDieuTriBanDau.Select(c => c).ToArray();
                phieuRaVienViewModel.SoNgayDieuTriBanDau1 = soNgay[0].ToString();
                phieuRaVienViewModel.SoNgayDieuTriBanDau2 = soNgay[1].ToString();
            }
            else
            {
                phieuRaVienViewModel.SoNgayDieuTriBanDau1 = "0";
                phieuRaVienViewModel.SoNgayDieuTriBanDau2 = soNgayDieuTriBanDau;
            }

            var chuyenKhoas = yeuCauTiepNhan.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.Where(cc => cc.Id != vaoKhoaFirst.Id)
                                                         .OrderBy(c => c.ThoiDiemVaoKhoa).ToList();

            var htmlChuyenKhoa = string.Empty;
            foreach (var item in chuyenKhoas)
            {
                var soNgay = ((item.ThoiDiemRaKhoa ?? DateTime.Now) - item.ThoiDiemVaoKhoa).Days + 1 + "";
                var itemNgay1 = string.Empty;
                var itemNgay2 = string.Empty;


                if (soNgayDieuTriBanDau.Length > 1)
                {
                    var sNgay = soNgay.Select(c => c).ToArray();
                    itemNgay1 = sNgay[0].ToString();
                    itemNgay2 = sNgay[1].ToString();
                }
                else
                {
                    itemNgay1 = "0";
                    itemNgay2 = soNgay;
                }

                htmlChuyenKhoa += " <i><span class='squareKhoa'> " + item.KhoaPhongChuyenDen.Ma + " </span></i><i>" + item.ThoiDiemVaoKhoa.Hour.ToString() + " giờ " + item.ThoiDiemVaoKhoa.Minute.ToString() + "phút," + item.ThoiDiemVaoKhoa.Date.ApplyFormatDate() + "</i>" +
                      "<span class='square2'> " + itemNgay1 + " </span> <span class='square2'>" + itemNgay2 + " </span><br/>";
            }

            phieuRaVienViewModel.TuyenTren = thongTinRaVien.HinhThucRaVienId == EnumHinhThucRaVien.ChuyenVien ?
                                             thongTinRaVien.TuyenChuyenId == LoaiChuyenTuyen.TuyenDuoiChuyenLen ||
                                             thongTinRaVien.TuyenChuyenId == LoaiChuyenTuyen.TuTuyenDuoiLenTrenLienKe ||
                                             thongTinRaVien.TuyenChuyenId == LoaiChuyenTuyen.TuTuyenDuoiLenTrenKhongLienKe
                                            ? ChonThongTin : string.Empty : string.Empty;

            phieuRaVienViewModel.TuyenDuoi = thongTinRaVien.HinhThucRaVienId == EnumHinhThucRaVien.ChuyenVien ?
                                               thongTinRaVien.TuyenChuyenId == LoaiChuyenTuyen.TuTuyenTrenVe
                                            || thongTinRaVien.TuyenChuyenId == LoaiChuyenTuyen.TuyenDuoiLienKe
                                            || thongTinRaVien.TuyenChuyenId == LoaiChuyenTuyen.TuyenDuoiKhongLienKe
                                            || thongTinRaVien.TuyenChuyenId == LoaiChuyenTuyen.TuTuyenTrenVeTuyenDuoi
                                            || thongTinRaVien.TuyenChuyenId == LoaiChuyenTuyen.TuyenTrenChuyenVeCoSoDaGoi
                                            ? ChonThongTin : string.Empty : string.Empty;

            phieuRaVienViewModel.CK = thongTinRaVien.HinhThucRaVienId == EnumHinhThucRaVien.ChuyenVien ?
                                               thongTinRaVien.TuyenChuyenId == LoaiChuyenTuyen.CungTuyenHang1
                                            || thongTinRaVien.TuyenChuyenId == LoaiChuyenTuyen.CungTuyenHang2
                                            || thongTinRaVien.TuyenChuyenId == LoaiChuyenTuyen.NoiKhacChuyenDen
                                            || thongTinRaVien.TuyenChuyenId == LoaiChuyenTuyen.ChuyenGiuaCacCoSoCungTuyen
                                            ? ChonThongTin : string.Empty : string.Empty;

            phieuRaVienViewModel.ChuyenDen = thongTinRaVien.HinhThucRaVienId == EnumHinhThucRaVien.ChuyenVien ?
                                             thongTinRaVien.TenBenhVien : string.Empty;


            phieuRaVienViewModel.ChuyenKhoa = htmlChuyenKhoa;
            phieuRaVienViewModel.VaoKhoa = vaoKhoaFirst.KhoaPhongChuyenDen.Ma;
            phieuRaVienViewModel.GioVK = vaoKhoaFirst.ThoiDiemVaoKhoa.Hour.ToString();
            phieuRaVienViewModel.NgayVK = vaoKhoaFirst.ThoiDiemVaoKhoa.Date.ApplyFormatDate();
            phieuRaVienViewModel.PhutVK = vaoKhoaFirst.ThoiDiemVaoKhoa.Minute.ToString();

            var noiTruFirst = yeuCauTiepNhan.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.FirstOrDefault();
            if (noiTruFirst != null && noiTruFirst.ThoiDiemRaKhoa != null)
            {
                var ngayDieuTri = ((noiTruFirst.ThoiDiemRaKhoa ?? DateTime.Now) - noiTruFirst.ThoiDiemVaoKhoa).Days + 1 + "";
                var lstNgayDieuTri = ngayDieuTri.Select(cc => cc.ToString()).ToArray();
                var ndt = string.Empty;
                foreach (var item in lstNgayDieuTri)
                {
                    ndt += "<span class=\"square\">" + item + "</span>";
                }
                phieuRaVienViewModel.DT15 = ndt;
            }

            if (thongTinRaVien.ThoiGianRaVien != null)
            {
                var tongSoNgayDieuTri = ((thongTinRaVien.ThoiGianRaVien ?? DateTime.Now) - yeuCauTiepNhan.NoiTruBenhAn.ThoiDiemNhapVien).Days + 1 + "";
                var lstTongSoNgayDieuTri = tongSoNgayDieuTri.Select(cc => cc.ToString()).ToArray();

                if (lstTongSoNgayDieuTri.Length == 1)
                {
                    phieuRaVienViewModel.TSDT1 = "0";
                    phieuRaVienViewModel.TSDT2 = "0";
                    phieuRaVienViewModel.TSDT3 = lstTongSoNgayDieuTri[0];
                }
                else if (lstTongSoNgayDieuTri.Length == 2)
                {
                    phieuRaVienViewModel.TSDT1 = "0";
                    phieuRaVienViewModel.TSDT2 = lstTongSoNgayDieuTri[0];
                    phieuRaVienViewModel.TSDT3 = lstTongSoNgayDieuTri[1];

                }
                else
                {
                    phieuRaVienViewModel.TSDT1 = lstTongSoNgayDieuTri[0];
                    phieuRaVienViewModel.TSDT2 = lstTongSoNgayDieuTri[1];
                    phieuRaVienViewModel.TSDT3 = tongSoNgayDieuTri.Substring(2);
                }
            }

            return phieuRaVienViewModel;
        }
        private PhieuRaVienViewModel PrepareThongTinChuanDoanNguoiBenh(PhieuRaVienViewModel phieuRaVienViewModel, YeuCauTiepNhan yeuCauTiepNhan, RaVien thongTinRaVien)
        {
            #region KHÁM CHUẨN ĐOÁN NỘI KHOA

            phieuRaVienViewModel.NoiChuyenDen = thongTinRaVien.GhiChuChuanDoanNoiChuyenDen;
            phieuRaVienViewModel.KKBCapCuu = thongTinRaVien.GhiChuChuanDoanKKBCapCuu;
            phieuRaVienViewModel.KhiVaoKhoaDT = thongTinRaVien.GhiChuNoiChuanDoanKhiVaoKhoaDieuTri;

            phieuRaVienViewModel.BenhChinh = thongTinRaVien.GhiChuChuanDoanRaVien;

            phieuRaVienViewModel.ThuThuat = thongTinRaVien.DoThuThuat == true ? ChonThongTin : null;
            phieuRaVienViewModel.PhauThuat = thongTinRaVien.DoPhauThuat == true ? ChonThongTin : null;
            phieuRaVienViewModel.TaiBien = thongTinRaVien.TrieuChung == true ? ChonThongTin : null;
            phieuRaVienViewModel.BienChung = thongTinRaVien.TrieuChung == false ? ChonThongTin : null;

            if (thongTinRaVien.ChuanDoanKemTheos != null && thongTinRaVien.ChuanDoanKemTheos.Any())
            {
                var benhKemTheo = string.Empty;
                foreach (var item in thongTinRaVien.ChuanDoanKemTheos)
                {
                    benhKemTheo += "<div class='margin_top1'><span class='value' style='display: block !important;'>";
                    benhKemTheo += "<i>" + item.ChuanDoan + "</i>";
                    benhKemTheo += "<div style='float: right !important'> ";

                    if (item.ICD != 0)
                    {
                        var ma = _icdService.GetMaICD(item.ICD).Result;
                        var fillSquareModel = FillSquareModel(ma);

                        benhKemTheo += "<span class=\"square2\">" + fillSquareModel.Square1 + "</span>";
                        benhKemTheo += "<span class=\"square2\">" + fillSquareModel.Square2 + "</span>";
                        benhKemTheo += "<span class=\"square2\">" + fillSquareModel.Square3 + "</span>";
                        benhKemTheo += "<span class=\"square2\">" + fillSquareModel.Square4 + "</span>";
                        benhKemTheo += "</div></span></div>";

                    }

                }

                phieuRaVienViewModel.BenhKemTheo = benhKemTheo;
            }

            if (thongTinRaVien.ChuanDoanNoiChuyenDenId != null)
            {
                string ma = _icdService.GetMaICD(thongTinRaVien.ChuanDoanNoiChuyenDenId ?? 0).Result;
                var fillSquareCDNoiChuyenDen = FillSquareModel(ma);

                phieuRaVienViewModel.MaCD1 = fillSquareCDNoiChuyenDen.Square1;
                phieuRaVienViewModel.MaCD2 = fillSquareCDNoiChuyenDen.Square2;
                phieuRaVienViewModel.MaCD3 = fillSquareCDNoiChuyenDen.Square3;
                phieuRaVienViewModel.MaCD4 = fillSquareCDNoiChuyenDen.Square4;
            }

            if (thongTinRaVien.ChuanDoanKKBCapCuuId != null)
            {
                var ma = _icdService.GetMaICD(thongTinRaVien.ChuanDoanKKBCapCuuId ?? 0).Result;
                var fillSquareCDKKCapCuu = FillSquareModel(ma);
                phieuRaVienViewModel.MaKKBCapCuu1 = fillSquareCDKKCapCuu.Square1;
                phieuRaVienViewModel.MaKKBCapCuu2 = fillSquareCDKKCapCuu.Square2;
                phieuRaVienViewModel.MaKKBCapCuu3 = fillSquareCDKKCapCuu.Square3;
                phieuRaVienViewModel.MaKKBCapCuu4 = fillSquareCDKKCapCuu.Square4;

            }

            if (thongTinRaVien.NoiChuanDoanKhiVaoKhoaDieuTriId != null)
            {
                var ma = _icdService.GetMaICD(thongTinRaVien.NoiChuanDoanKhiVaoKhoaDieuTriId ?? 0).Result;
                var fillSquareCDVaoKhoaDieu = FillSquareModel(ma);
                phieuRaVienViewModel.MaVaoKhoaDT1 = fillSquareCDVaoKhoaDieu.Square1;
                phieuRaVienViewModel.MaVaoKhoaDT2 = fillSquareCDVaoKhoaDieu.Square2;
                phieuRaVienViewModel.MaVaoKhoaDT3 = fillSquareCDVaoKhoaDieu.Square3;
                phieuRaVienViewModel.MaVaoKhoaDT4 = fillSquareCDVaoKhoaDieu.Square4;

            }

            if (thongTinRaVien.ChuanDoanRaVienId != null)
            {
                var ma = _icdService.GetMaICD(thongTinRaVien.ChuanDoanRaVienId ?? 0).Result;
                var fillSquareChuanDoanRaVien = FillSquareModel(ma);

                phieuRaVienViewModel.MaBC1 = fillSquareChuanDoanRaVien.Square1;
                phieuRaVienViewModel.MaBC2 = fillSquareChuanDoanRaVien.Square2;
                phieuRaVienViewModel.MaBC3 = fillSquareChuanDoanRaVien.Square3;
                phieuRaVienViewModel.MaBC4 = fillSquareChuanDoanRaVien.Square4;
            }

            #endregion

            phieuRaVienViewModel.CC = yeuCauTiepNhan.NoiTruBenhAn.LaCapCuu == true ? ChonThongTin : "";
            phieuRaVienViewModel.KKB = yeuCauTiepNhan.NoiTruBenhAn.LaCapCuu == false ? ChonThongTin : "";

            phieuRaVienViewModel.Khoi = thongTinRaVien.KetQuaDieuTriId == EnumKetQuaDieuTri.Khoi ? ChonThongTin : null;
            phieuRaVienViewModel.NangHon = thongTinRaVien.KetQuaDieuTriId == EnumKetQuaDieuTri.NangHon ? ChonThongTin : null;
            phieuRaVienViewModel.DoGiam = thongTinRaVien.KetQuaDieuTriId == EnumKetQuaDieuTri.Do ? ChonThongTin : null;
            phieuRaVienViewModel.TuVong = thongTinRaVien.KetQuaDieuTriId == EnumKetQuaDieuTri.TuVong ? ChonThongTin : null;
            phieuRaVienViewModel.KhongThayDoi = thongTinRaVien.KetQuaDieuTriId == EnumKetQuaDieuTri.KhongThayDoi ? ChonThongTin : null;

            phieuRaVienViewModel.DoBenh = thongTinRaVien.LyDoTuVongId == LyDoTuVong.DoBenh ? ChonThongTin : null;
            phieuRaVienViewModel.DoTaiBienDT = thongTinRaVien.LyDoTuVongId == LyDoTuVong.DoTaiBienDieuTri ? ChonThongTin : null;
            phieuRaVienViewModel.KhacDT = thongTinRaVien.LyDoTuVongId == LyDoTuVong.DoBenh ? ChonThongTin : null;



            return phieuRaVienViewModel;
        }
        private PhieuRaVienViewModel PrepareThongTinhTrangRaVien(PhieuRaVienViewModel phieuRaVienViewModel, YeuCauTiepNhan yeuCauTiepNhan, RaVien thongTinRaVien)
        {

            phieuRaVienViewModel.GioTV = thongTinRaVien.HinhThucRaVienId == EnumHinhThucRaVien.TuVong
                || thongTinRaVien.HinhThucRaVienId == EnumHinhThucRaVien.TuVongTruoc24H ? thongTinRaVien.ThoiGianTuVong?.Hour + KhongCoDuLieu : KhongCoDuLieu;
            phieuRaVienViewModel.PhutTV = thongTinRaVien.HinhThucRaVienId == EnumHinhThucRaVien.TuVong
                || thongTinRaVien.HinhThucRaVienId == EnumHinhThucRaVien.TuVongTruoc24H ? thongTinRaVien.ThoiGianTuVong?.Minute + KhongCoDuLieu : KhongCoDuLieu;
            phieuRaVienViewModel.NgayTV = thongTinRaVien.HinhThucRaVienId == EnumHinhThucRaVien.TuVong
                || thongTinRaVien.HinhThucRaVienId == EnumHinhThucRaVien.TuVongTruoc24H ? thongTinRaVien.ThoiGianTuVong?.Day.ToString() : KhongCoDuLieu;
            phieuRaVienViewModel.ThangTV = thongTinRaVien.HinhThucRaVienId == EnumHinhThucRaVien.TuVong
                || thongTinRaVien.HinhThucRaVienId == EnumHinhThucRaVien.TuVongTruoc24H ? thongTinRaVien.ThoiGianTuVong?.Month.ToString() : KhongCoDuLieu;
            phieuRaVienViewModel.NamTV = thongTinRaVien.HinhThucRaVienId == EnumHinhThucRaVien.TuVong
                || thongTinRaVien.HinhThucRaVienId == EnumHinhThucRaVien.TuVongTruoc24H ? thongTinRaVien.ThoiGianTuVong?.Year.ToString() : KhongCoDuLieu;

            phieuRaVienViewModel.Khoi = thongTinRaVien.KetQuaDieuTriId == EnumKetQuaDieuTri.Khoi ? ChonThongTin : null;
            phieuRaVienViewModel.NangHon = thongTinRaVien.KetQuaDieuTriId == EnumKetQuaDieuTri.NangHon ? ChonThongTin : null;
            phieuRaVienViewModel.DoGiam = thongTinRaVien.KetQuaDieuTriId == EnumKetQuaDieuTri.Do ? ChonThongTin : null;
            phieuRaVienViewModel.TuVong = thongTinRaVien.KetQuaDieuTriId == EnumKetQuaDieuTri.TuVong ? ChonThongTin : null;
            phieuRaVienViewModel.KhongThayDoi = thongTinRaVien.KetQuaDieuTriId == EnumKetQuaDieuTri.KhongThayDoi ? ChonThongTin : null;

            phieuRaVienViewModel.DoBenh = thongTinRaVien.LyDoTuVongId == LyDoTuVong.DoBenh ||
                                          thongTinRaVien.LyDoTuVongId == LyDoTuVong.DoUngThu ? ChonThongTin : null;

            phieuRaVienViewModel.DoTaiBienDT = thongTinRaVien.LyDoTuVongId == LyDoTuVong.DoTaiBienDieuTri ? ChonThongTin : null;
            phieuRaVienViewModel.KhacDT = thongTinRaVien.LyDoTuVongId == LyDoTuVong.KhongRo ||
                                          thongTinRaVien.LyDoTuVongId == LyDoTuVong.DoNguyenNhanKhac ? ChonThongTin : null;

            phieuRaVienViewModel.NguyenNhanChinhTuVong = thongTinRaVien.ChiTietChuanDoanTuVong;
            phieuRaVienViewModel.KhamNghiemTuThi = thongTinRaVien.KhamNghiemTuThi == true ? ChonThongTin : null;

            phieuRaVienViewModel.LanhTinh = thongTinRaVien.GiaPhauThuatId == LoaiGiaPhauThuat.LanhTinh ? ChonThongTin : null;
            phieuRaVienViewModel.NghiNgo = thongTinRaVien.GiaPhauThuatId == LoaiGiaPhauThuat.NghiNgo ? ChonThongTin : null;
            phieuRaVienViewModel.AcTinh = thongTinRaVien.GiaPhauThuatId == LoaiGiaPhauThuat.AcTinh ? ChonThongTin : null;

            phieuRaVienViewModel.ChanDoanGiaiPhauTT = thongTinRaVien.ChiTietChuanDoanTuThi;

            if (thongTinRaVien.HinhThucRaVienId == EnumHinhThucRaVien.TuVong || thongTinRaVien.HinhThucRaVienId == EnumHinhThucRaVien.TuVongTruoc24H)
            {
                //
                if (thongTinRaVien.ThoiGianTuVong != null && yeuCauTiepNhan.ThoiDiemTiepNhan != null)
                {
                    var dayTuVong = ((thongTinRaVien.ThoiGianTuVong ?? DateTime.Now) - yeuCauTiepNhan.ThoiDiemTiepNhan).Days + 1;
                    if (dayTuVong > 1)
                    {
                        phieuRaVienViewModel.Sau24hVaoVien = ChonThongTin;
                    }
                    else
                    {
                        phieuRaVienViewModel.Trong24hVaoVien = ChonThongTin;
                    }
                }

                var chuanDoanNguyenNhanTV = _icdService.GetMaICD(thongTinRaVien.NguyenNhanChinhTuVongId ?? 0).Result;
                if (!string.IsNullOrEmpty(chuanDoanNguyenNhanTV))
                {
                    var fillSquareCDNguyenNhanTV = FillSquareModel(chuanDoanNguyenNhanTV);

                    phieuRaVienViewModel.MaNNCTuVong1 = fillSquareCDNguyenNhanTV.Square1;
                    phieuRaVienViewModel.MaNNCTuVong2 = fillSquareCDNguyenNhanTV.Square2;
                    phieuRaVienViewModel.MaNNCTuVong3 = fillSquareCDNguyenNhanTV.Square3;
                    phieuRaVienViewModel.MaNNCTuVong4 = fillSquareCDNguyenNhanTV.Square4;
                }


                var chuanDoanTuThi = _icdService.GetMaICD(thongTinRaVien.ChuanDoanTuThiId ?? 0).Result;
                if (!string.IsNullOrEmpty(chuanDoanTuThi))
                {
                    var fillSquareChuanDoanTuThi = FillSquareModel(chuanDoanTuThi);

                    phieuRaVienViewModel.MaCDGiaiPhauTT1 = fillSquareChuanDoanTuThi.Square1;
                    phieuRaVienViewModel.MaCDGiaiPhauTT2 = fillSquareChuanDoanTuThi.Square2;
                    phieuRaVienViewModel.MaCDGiaiPhauTT3 = fillSquareChuanDoanTuThi.Square3;
                    phieuRaVienViewModel.MaCDGiaiPhauTT4 = fillSquareChuanDoanTuThi.Square4;
                }
            }

            return phieuRaVienViewModel;
        }

        private PhieuRaVienViewModel PrepareThongTinDungChung(PhieuRaVienViewModel phieuRaVienViewModel, YeuCauTiepNhan yeuCauTiepNhan,
                                                              RaVien thongTinRaVien, ThongTinBenhAn thongTinBenhAn, DieuTriNoiTruTongKetBenhAnViewModel tongKetBenhAn)
        {
            #region THÔNG TIN CHUNG CỦA PHIẾU

            phieuRaVienViewModel.CC = yeuCauTiepNhan.NoiTruBenhAn.LaCapCuu == true ? ChonThongTin : "";
            phieuRaVienViewModel.KKB = yeuCauTiepNhan.NoiTruBenhAn.LaCapCuu == false ? ChonThongTin : "";

            phieuRaVienViewModel.NoiChuyenDen = thongTinRaVien.GhiChuChuanDoanNoiChuyenDen;
            phieuRaVienViewModel.KKBCapCuu = thongTinRaVien.GhiChuChuanDoanKKBCapCuu;
            phieuRaVienViewModel.KhiVaoKhoaDT = thongTinRaVien.GhiChuNoiChuanDoanKhiVaoKhoaDieuTri;
            phieuRaVienViewModel.BenhChinh = thongTinRaVien.GhiChuChuanDoanRaVien;

            phieuRaVienViewModel.ThuThuat = thongTinRaVien.DoThuThuat == true ? ChonThongTin : null;
            phieuRaVienViewModel.PhauThuat = thongTinRaVien.DoPhauThuat == true ? ChonThongTin : null;
            phieuRaVienViewModel.TaiBien = thongTinRaVien.TrieuChung == true ? ChonThongTin : null;
            phieuRaVienViewModel.BienChung = thongTinRaVien.TrieuChung == false ? ChonThongTin : null;

            phieuRaVienViewModel.Khoi = thongTinRaVien.KetQuaDieuTriId == EnumKetQuaDieuTri.Khoi ? ChonThongTin : null;
            phieuRaVienViewModel.NangHon = thongTinRaVien.KetQuaDieuTriId == EnumKetQuaDieuTri.NangHon ? ChonThongTin : null;
            phieuRaVienViewModel.DoGiam = thongTinRaVien.KetQuaDieuTriId == EnumKetQuaDieuTri.Do ? ChonThongTin : null;
            phieuRaVienViewModel.TuVong = thongTinRaVien.KetQuaDieuTriId == EnumKetQuaDieuTri.TuVong ? ChonThongTin : null;
            phieuRaVienViewModel.KhongThayDoi = thongTinRaVien.KetQuaDieuTriId == EnumKetQuaDieuTri.KhongThayDoi ? ChonThongTin : null;

            phieuRaVienViewModel.NguyenNhanChinhTuVong = thongTinRaVien.ChiTietChuanDoanTuVong;
            phieuRaVienViewModel.KhamNghiemTuThi = thongTinRaVien.KhamNghiemTuThi == true ? ChonThongTin : null;

            phieuRaVienViewModel.LanhTinh = thongTinRaVien.GiaPhauThuatId == LoaiGiaPhauThuat.LanhTinh ? ChonThongTin : null;
            phieuRaVienViewModel.NghiNgo = thongTinRaVien.GiaPhauThuatId == LoaiGiaPhauThuat.NghiNgo ? ChonThongTin : null;
            phieuRaVienViewModel.AcTinh = thongTinRaVien.GiaPhauThuatId == LoaiGiaPhauThuat.AcTinh ? ChonThongTin : null;

            phieuRaVienViewModel.ChanDoanGiaiPhauTT = thongTinRaVien.ChiTietChuanDoanTuThi;

            phieuRaVienViewModel.NgayHienTai = DateTime.Now.Day + "";
            phieuRaVienViewModel.ThangHienTai = DateTime.Now.Month + "";
            phieuRaVienViewModel.NamHienTai = DateTime.Now.Year + "";

            phieuRaVienViewModel.LyDoVaoVien = thongTinBenhAn.LyDoVaoVien;
            phieuRaVienViewModel.QuaTrinhBenhLy = thongTinBenhAn.QuaTrinhHoiBenh;
            phieuRaVienViewModel.TSBBanThan = thongTinBenhAn.TienSuBenhBanThan;

            phieuRaVienViewModel.DiUng = thongTinBenhAn.ThoiGianDiUng != null ? ChonThongTin : null;
            phieuRaVienViewModel.ThoiGianDiUng = thongTinBenhAn.ThoiGianDiUng + "";
            phieuRaVienViewModel.ThuocLa = thongTinBenhAn.ThoiGianThuocLa != null ? ChonThongTin : null;
            phieuRaVienViewModel.ThoiGianThuocLa = thongTinBenhAn.ThoiGianThuocLa + "";
            phieuRaVienViewModel.MaTuy = thongTinBenhAn.ThoiGianMaTuy != null ? ChonThongTin : null;
            phieuRaVienViewModel.ThoiGianMaTuy = thongTinBenhAn.ThoiGianMaTuy + "";
            phieuRaVienViewModel.ThuocLao = thongTinBenhAn.ThoiGianThuocLao != null ? ChonThongTin : null;
            phieuRaVienViewModel.ThoiGianThuocLao = thongTinBenhAn.ThoiGianThuocLao + "";
            phieuRaVienViewModel.RuouBia = thongTinBenhAn.ThoiGianRuouBia != null ? ChonThongTin : null;
            phieuRaVienViewModel.ThoiGianRuouBia = thongTinBenhAn.ThoiGianRuouBia + "";
            phieuRaVienViewModel.DDKhac = thongTinBenhAn.ThoiGianKhac != null ? ChonThongTin : null;
            phieuRaVienViewModel.ThoiGianDDKhac = thongTinBenhAn.ThoiGianKhac + "";

            phieuRaVienViewModel.TSBGiaDinh = thongTinBenhAn.TienSuBenhGiaDinh;

            phieuRaVienViewModel.ToanThan = thongTinBenhAn.KhamBenhToanThan;

            phieuRaVienViewModel.TuanHoan = thongTinBenhAn.TuanHoan;
            phieuRaVienViewModel.HoHap = thongTinBenhAn.HoHap;
            phieuRaVienViewModel.TieuHoa = thongTinBenhAn.TieuHoa;
            phieuRaVienViewModel.ThanTietNieuSinhDuc = thongTinBenhAn.ThanTietNieu;
            phieuRaVienViewModel.ThanKinh = thongTinBenhAn.ThanKinh;
            phieuRaVienViewModel.CoXuongKhop = thongTinBenhAn.CoXuongKhop;
            phieuRaVienViewModel.TaiMuiHong = thongTinBenhAn.TaiMuiHong;
            phieuRaVienViewModel.RangHamMat = thongTinBenhAn.RangHamMat;
            phieuRaVienViewModel.Mat = thongTinBenhAn.Mat;
            phieuRaVienViewModel.NoiTietDinhDuongVaCacBenhKhac = thongTinBenhAn.NoiTiet;
            phieuRaVienViewModel.CacCLS = thongTinBenhAn.CacXetNghiemCanLam;
            phieuRaVienViewModel.TomTatBenhAn = thongTinBenhAn.TomTatBenhAn;

            phieuRaVienViewModel.ChanDoanBenhChinh = thongTinBenhAn.ChuanDoan;
            phieuRaVienViewModel.ChanDoanBenhKemTheo = thongTinBenhAn.ChuanDoanKemTheos != null ? thongTinBenhAn.ChuanDoanKemTheos.Select(p => p.ChuanDoan).Join("; ") : "";
            phieuRaVienViewModel.ChanDoanPhanBiet = thongTinBenhAn.ChuanDoanPhanBiets != null ? thongTinBenhAn.ChuanDoanPhanBiets.Select(p => p.ChuanDoan).Join("; ") : "";

            phieuRaVienViewModel.TienLuong = thongTinBenhAn.TienLuong;
            phieuRaVienViewModel.HuongDieuTri = thongTinBenhAn.HuongDanDieuTri;

            phieuRaVienViewModel.NgayLamBA = DateTime.Now.Day + "";
            phieuRaVienViewModel.ThangLamBA = DateTime.Now.Month + "";
            phieuRaVienViewModel.NamLamBA = DateTime.Now.Year + "";

            phieuRaVienViewModel.TKNam = thongTinBenhAn.BatDauThayKinhNam + "";
            phieuRaVienViewModel.TuoiTK = thongTinBenhAn.TuoiCoKinh + "";
            phieuRaVienViewModel.TCKNguyet = thongTinBenhAn.TinhChatKinhNghiem + "";
            phieuRaVienViewModel.NgayCKy = thongTinBenhAn.ChuKy + "";
            phieuRaVienViewModel.SoNgayTK = thongTinBenhAn.SoNgayThayKinh + "";
            phieuRaVienViewModel.LKinh = thongTinBenhAn.LuongKinh + "";
            phieuRaVienViewModel.KNguyetLCNgay = thongTinBenhAn?.KinhCuoiNgay?.ApplyFormatDate();
            phieuRaVienViewModel.DauBung = thongTinBenhAn.DauBung == true ? ChonThongTin : null;
            phieuRaVienViewModel.KTruoc = thongTinBenhAn.ThoiGianTruoc == true ? ChonThongTin : null;
            phieuRaVienViewModel.KTrong = thongTinBenhAn.ThoiGianTrong == true ? ChonThongTin : null;
            phieuRaVienViewModel.KSau = thongTinBenhAn.ThoiGianSau == true ? ChonThongTin : null;
            phieuRaVienViewModel.LCNam = thongTinBenhAn.LayChongNam + "";
            phieuRaVienViewModel.LCTuoi = thongTinBenhAn.TuoiLayChong + "";
            phieuRaVienViewModel.HKinhNam = thongTinBenhAn.NamHetKinh + "";
            phieuRaVienViewModel.HKinhTuoi = thongTinBenhAn.TuoiHetKinh + "";
            phieuRaVienViewModel.NhungBenhPKDaDT = thongTinBenhAn.NhungBenhPhuKhoaDaDieuTri + thongTinBenhAn.BenhPhuKhoaDieuTri;



            phieuRaVienViewModel.S1 = thongTinBenhAn.TienThaiPara1 != null ? thongTinBenhAn.TienThaiPara1?.ToString() : null;
            phieuRaVienViewModel.S2 = thongTinBenhAn.TienThaiPara2 != null ? thongTinBenhAn.TienThaiPara2?.ToString() : null;
            phieuRaVienViewModel.S3 = thongTinBenhAn.TienThaiPara3 != null ? thongTinBenhAn.TienThaiPara3?.ToString() : null;
            phieuRaVienViewModel.S4 = thongTinBenhAn.TienThaiPara4 != null ? thongTinBenhAn.TienThaiPara4?.ToString() : null;

            phieuRaVienViewModel.SPhuKhoa1 = thongTinBenhAn.TienThaiPara == TienThaiPara.SinhDuThang ? ChonThongTin : null;
            phieuRaVienViewModel.SPhuKhoa2 = thongTinBenhAn.TienThaiPara == TienThaiPara.Som ? ChonThongTin : null;
            phieuRaVienViewModel.SPhuKhoa3 = thongTinBenhAn.TienThaiPara == TienThaiPara.Say ? ChonThongTin : null;
            phieuRaVienViewModel.SPhuKhoa4 = thongTinBenhAn.TienThaiPara == TienThaiPara.Song ? ChonThongTin : null;


            var yeuCauTiepNhanMe = yeuCauTiepNhan?.YeuCauNhapVien.YeuCauTiepNhanMe;
            if (yeuCauTiepNhanMe != null && !string.IsNullOrEmpty(yeuCauTiepNhanMe.NoiTruBenhAn.ThongTinBenhAn))
            {
                var thongTinBenhAnMe = JsonConvert.DeserializeObject<ThongTinBenhAn>(yeuCauTiepNhanMe.NoiTruBenhAn.ThongTinBenhAn);
                var tongTinTongKetBenhAnMe = JsonConvert.DeserializeObject<DieuTriNoiTruTongKetBenhAnViewModel>(yeuCauTiepNhanMe.NoiTruBenhAn.ThongTinTongKetBenhAn);

                phieuRaVienViewModel.ConThuMay = thongTinBenhAnMe.TienSuSanKhoas != null ? thongTinBenhAnMe.TienSuSanKhoas.Select(c => c.SoLanCoThai).LastOrDefault()?.ToString() : string.Empty;

                //phieuRaVienViewModel.SS1 = thongTinBenhAn.TienThaiPara == TienThaiPara.SinhDuThang ? ChonThongTin : null;
                //phieuRaVienViewModel.SS2 = thongTinBenhAn.TienThaiPara == TienThaiPara.Som ? ChonThongTin : null;
                //phieuRaVienViewModel.SS3 = thongTinBenhAn.TienThaiPara == TienThaiPara.Say ? ChonThongTin : null;
                //phieuRaVienViewModel.SS4 = thongTinBenhAn.TienThaiPara == TienThaiPara.Song ? ChonThongTin : null;
            }
            else
            {
                phieuRaVienViewModel.ConThuMay = thongTinBenhAn.ConThuMay?.ToString();
            }




            phieuRaVienViewModel.VongDauSS = thongTinBenhAn?.VongDauSoSinh.ToString();
            phieuRaVienViewModel.HoHapNhipTho = thongTinBenhAn.HoHapNhiTho?.ToString();

            if (thongTinBenhAn.ChiSoSinhTons != null && thongTinBenhAn.ChiSoSinhTons.Any())
            {
                phieuRaVienViewModel.NhietDoSS = thongTinBenhAn.ChiSoSinhTons.Last().ThanNhiet + "";
                phieuRaVienViewModel.NhiThoSS = thongTinBenhAn.ChiSoSinhTons.Last().NhipTho + "";
                phieuRaVienViewModel.CanNangSS = thongTinBenhAn.ChiSoSinhTons.Last().CanNang + "";
                phieuRaVienViewModel.ChieuCaoSS = thongTinBenhAn.ChiSoSinhTons.Last().ChieuCao + "";
            }

            phieuRaVienViewModel.Hach = thongTinBenhAn.Hach;
            phieuRaVienViewModel.Vu = thongTinBenhAn.Vu;
            phieuRaVienViewModel.ThanTietNieu = thongTinBenhAn.ThanTietNieu;
            phieuRaVienViewModel.CoQuanKhac = thongTinBenhAn.CacBoPhanKhac;

            phieuRaVienViewModel.CacDauHieuSinhDuc = thongTinBenhAn.CacDauHieuSinhDucThuPhat;
            phieuRaVienViewModel.MoiLon = thongTinBenhAn.MoiLon;
            phieuRaVienViewModel.MoiBe = thongTinBenhAn.MoiBe;
            phieuRaVienViewModel.AmVat = thongTinBenhAn.AmVat;
            phieuRaVienViewModel.AmHo = thongTinBenhAn.AmHo;
            phieuRaVienViewModel.MangTrinh = thongTinBenhAn.MangTrinh;
            phieuRaVienViewModel.TangSinhMon = thongTinBenhAn.TangSinhMon;

            phieuRaVienViewModel.AmDao = thongTinBenhAn.AmDao;
            phieuRaVienViewModel.CoTuCung = thongTinBenhAn.CoTuCung;
            phieuRaVienViewModel.ThanTuCung = thongTinBenhAn.ThanTuCung;
            phieuRaVienViewModel.PhanPhu = thongTinBenhAn.PhanPhu;
            phieuRaVienViewModel.CacTuiCung = thongTinBenhAn.CacTuiCung;

            phieuRaVienViewModel.BSLamBA = yeuCauTiepNhan.NoiTruBenhAn.NhanVienTaoBenhAn?.User?.HoTen;

            phieuRaVienViewModel.QuaTrinhBLLS = tongKetBenhAn.QuaTrinhBenhLy;
            phieuRaVienViewModel.PhuongPhapDT = tongKetBenhAn.PhuongPhapDieuTri;
            phieuRaVienViewModel.PPDTPhauThuat = tongKetBenhAn.PhauThuatThuThuat == 1 ? ChonThongTin : null;
            phieuRaVienViewModel.PPDTThuThuat = tongKetBenhAn.PhauThuatThuThuat == 2 ? ChonThongTin : null;

            phieuRaVienViewModel.TTNguoiBenhRaVien = tongKetBenhAn.TinhTrangNguoiBenhKhiRaVien;
            phieuRaVienViewModel.HDTVaCacCheDoTT = tongKetBenhAn.HuongDieuTri;
            phieuRaVienViewModel.SoToXQuang = tongKetBenhAn.SoToXQuang + "";
            phieuRaVienViewModel.SoToScanner = tongKetBenhAn.SoToCTScanner + "";
            phieuRaVienViewModel.SoToSieuAm = tongKetBenhAn.SoToSieuAm + "";
            phieuRaVienViewModel.SoToXetNghiem = tongKetBenhAn.SoToXetNghiem + "";
            phieuRaVienViewModel.SoToKhacCDTT = tongKetBenhAn.SoToKhac + "";
            phieuRaVienViewModel.SoToToanHoSo = tongKetBenhAn.SoToXQuang + tongKetBenhAn.SoToCTScanner
              + tongKetBenhAn.SoToSieuAm + tongKetBenhAn.SoToXetNghiem + tongKetBenhAn.SoToKhac + "";


            phieuRaVienViewModel.NgayCD = DateTime.Now.Day + "";
            phieuRaVienViewModel.ThangCD = DateTime.Now.Month + "";
            phieuRaVienViewModel.NamCD = DateTime.Now.Year + "";

            phieuRaVienViewModel.NguyenNhan = thongTinRaVien.NguyenNhan;
            phieuRaVienViewModel.ChanDoanTruocPT = thongTinRaVien.GhiChuChuanDoanTruocPhauThuat;
            phieuRaVienViewModel.ChanDoanSauPT = thongTinRaVien.GhiChuChuanDoanSauPhauThuat;
            phieuRaVienViewModel.CDDoPT = thongTinRaVien.DoPhauThuat == true ? ChonThongTin : null;
            phieuRaVienViewModel.CDDoGM = thongTinRaVien.DoGayMe == true ? ChonThongTin : null;
            phieuRaVienViewModel.CDDoNK = thongTinRaVien.DoNhiemKhau == true ? ChonThongTin : null;
            phieuRaVienViewModel.CDKhac = thongTinRaVien.Khac == true ? ChonThongTin : null;
            phieuRaVienViewModel.TongSoNgayDTPT = thongTinRaVien.TongSoNgayDieuTriSauPT + "";
            phieuRaVienViewModel.TongSoLanDTPT = thongTinRaVien.TongSoLanPT + "";


            phieuRaVienViewModel.CapCuu = thongTinRaVien.TinhTrangCapCuuChuDong == true ? ChonThongTin : null;
            phieuRaVienViewModel.ChuDong = thongTinRaVien.TinhTrangCapCuuChuDong == false ? ChonThongTin : null;

            phieuRaVienViewModel.CDTPT = thongTinRaVien.GhiChuChuanDoanTruocPhauThuat;
            phieuRaVienViewModel.CDSPT = thongTinRaVien.GhiChuChuanDoanSauPhauThuat;

            phieuRaVienViewModel.LVDe = thongTinRaVien.GhiChuNoiChuanDoanLucVaoDe;
            phieuRaVienViewModel.PPPhauThuat = thongTinRaVien.PhuongPhapPhauThuat;
            phieuRaVienViewModel.NgayDe = thongTinRaVien.NgayDe != null ? (thongTinRaVien.NgayDe ?? DateTime.Now).ApplyFormatDate() : "";


            phieuRaVienViewModel.KhocNgay = thongTinBenhAn?.TinhTrangSoSinh == TinhTrangSoSinh.KhocNgay ? ChonThongTin : "";
            phieuRaVienViewModel.Ngat = thongTinBenhAn?.TinhTrangSoSinh == TinhTrangSoSinh.Ngat ? ChonThongTin : "";
            phieuRaVienViewModel.Khac = thongTinBenhAn?.TinhTrangSoSinh == TinhTrangSoSinh.Khac ? ChonThongTin : "";

            phieuRaVienViewModel.ChucdanhBacSiDoDe = thongTinBenhAn.TenNhanVien + " " + thongTinBenhAn.TenChucDanh;

            phieuRaVienViewModel.MotPhut = thongTinBenhAn.Apgar1p;
            phieuRaVienViewModel.NamPhut = thongTinBenhAn.Apgar5p;
            phieuRaVienViewModel.MuoiPhut = thongTinBenhAn.Apgar10p;

            phieuRaVienViewModel.ChucdanhBacSiChuyenSoSinh = thongTinBenhAn.TenNhanVienChuyenSoSinh + " " + thongTinBenhAn.TenNhanVienChucDan;
            phieuRaVienViewModel.TinhTrangDinhDuongSauSinh = thongTinBenhAn.TinhTrangDinhDuongSauSinh;



            phieuRaVienViewModel.HutDich = thongTinBenhAn.HutDich == true ? ChonThongTin : "";
            phieuRaVienViewModel.DatNoiKhiQuan = thongTinBenhAn.DatNoiKhiQuan == true ? ChonThongTin : "";
            phieuRaVienViewModel.XoaBopTim = thongTinBenhAn.XoaBopTim == true ? ChonThongTin : "";
            phieuRaVienViewModel.BopBongO2 = thongTinBenhAn.BopBongO2 == true ? ChonThongTin : "";
            phieuRaVienViewModel.ThoO2 = thongTinBenhAn.ThoO2 == true ? ChonThongTin : "";
            phieuRaVienViewModel.ThoO2 = thongTinBenhAn.ThoO2 == true ? ChonThongTin : "";


            phieuRaVienViewModel.DiTatBamSinh = thongTinBenhAn.DiTatBamSinh == true ? ChonThongTin : "";
            phieuRaVienViewModel.CoHauMon = thongTinBenhAn.CoHauMon == true ? ChonThongTin : "";
            phieuRaVienViewModel.CuTheDiTat = thongTinBenhAn.ChuThichDiTatBamSinh;
            phieuRaVienViewModel.LyDoCanThiepSS = thongTinBenhAn.LyDoCanThiep;
            phieuRaVienViewModel.CachThucDe = thongTinRaVien.CachThucDe;
            phieuRaVienViewModel.NgoiThai = thongTinRaVien.NgoiThai;
            phieuRaVienViewModel.KSTuCung = thongTinRaVien.KiemXoatTuCung;

            phieuRaVienViewModel.DiTat = thongTinRaVien.DiTatThai;

            phieuRaVienViewModel.TuoiThai = thongTinBenhAn.TuoiThai + "";
            phieuRaVienViewModel.KhamThaiTai = thongTinBenhAn.KhamLaiThai;
            phieuRaVienViewModel.TiemPhongUV = thongTinBenhAn.TiemChungUongVan == true ? ChonThongTin : null;
            phieuRaVienViewModel.DuocTiem = thongTinBenhAn.DuocTiem + "";

            phieuRaVienViewModel.GioCDa = thongTinBenhAn.BatDauChuyenDa != null ? thongTinBenhAn.BatDauChuyenDa.Value.Hour + "" : "";
            phieuRaVienViewModel.PhutCDa = thongTinBenhAn.BatDauChuyenDa != null ? thongTinBenhAn.BatDauChuyenDa.Value.Minute + "" : "";
            phieuRaVienViewModel.NgayCDa = thongTinBenhAn.BatDauChuyenDa != null ? thongTinBenhAn.BatDauChuyenDa.Value.ApplyFormatDate() + "" : "";

            phieuRaVienViewModel.DHLucDau = thongTinBenhAn.DauHieuBanDau;
            phieuRaVienViewModel.BienChuyen = thongTinBenhAn.ChuyenBien;

            phieuRaVienViewModel.ToanTrang = thongTinBenhAn.ToanTrang;
            phieuRaVienViewModel.Phu = thongTinBenhAn.Phu == true ? ChonThongTin : null;
            phieuRaVienViewModel.TietNieu = thongTinBenhAn.ThanTieuNieu;
            phieuRaVienViewModel.CacBoPhanKhac = thongTinBenhAn.CacBoPhanKhac;

            phieuRaVienViewModel.BungCoSeo = thongTinBenhAn.BungCoSoCu == true ? ChonThongTin : null;
            phieuRaVienViewModel.HinhDangTC = thongTinBenhAn.HinhDangTuCung;
            phieuRaVienViewModel.TuThe = thongTinBenhAn.TuThe;
            phieuRaVienViewModel.ChieuCaoTC = thongTinBenhAn.ChieuCaoTuCung + "";
            phieuRaVienViewModel.VongBung = thongTinBenhAn.VongBung + "";
            phieuRaVienViewModel.ConCoTC = thongTinBenhAn.ConCoTC;
            phieuRaVienViewModel.TimThai = thongTinBenhAn.TimThai + "";
            phieuRaVienViewModel.ChiSo = thongTinBenhAn.ChiSoBiShop + "";
            phieuRaVienViewModel.OiPhong = thongTinBenhAn.TinhTrangVoOiId == TinhTrangVoOi.OiPhong ? ChonThongTin : "";
            phieuRaVienViewModel.OiDet = thongTinBenhAn.TinhTrangVoOiId == TinhTrangVoOi.OiDep ? ChonThongTin : "";
            phieuRaVienViewModel.OiQL = thongTinBenhAn.TinhTrangVoOiId == TinhTrangVoOi.OiQuaLe ? ChonThongTin : "";

            phieuRaVienViewModel.GioVoOi = thongTinBenhAn.NgayVoNuocOi != null ? thongTinBenhAn.NgayVoNuocOi.Value.Hour + "" : "";
            phieuRaVienViewModel.PhutVoOi = thongTinBenhAn.NgayVoNuocOi != null ? thongTinBenhAn.NgayVoNuocOi.Value.Minute + "" : "";
            phieuRaVienViewModel.NgayVoOi = thongTinBenhAn.NgayVoNuocOi != null ? (thongTinBenhAn.NgayVoNuocOi ?? DateTime.Now).ApplyFormatDate() : "";

            phieuRaVienViewModel.TuNhien = thongTinBenhAn.VoOiId == VoOi.TuNhien ? ChonThongTin : "";
            phieuRaVienViewModel.BamOi = thongTinBenhAn.VoOiId == VoOi.BamOi ? ChonThongTin : "";

            phieuRaVienViewModel.MauSacNuocOi = thongTinBenhAn.MauSacNuocOi;
            phieuRaVienViewModel.MauSacNuocOiSS = thongTinBenhAn.MauSac;

            phieuRaVienViewModel.NuocOiNhieuIt = thongTinBenhAn.NuocOiNhieuHayIt;
            phieuRaVienViewModel.Ngoi = thongTinBenhAn.Ngoi;
            phieuRaVienViewModel.The = thongTinBenhAn.The;
            phieuRaVienViewModel.KieuThe = thongTinBenhAn.KieuThe;

            phieuRaVienViewModel.Cao = thongTinBenhAn.DoLotId == DoLot.Cao ? ChonThongTin : "";
            phieuRaVienViewModel.Chuc = thongTinBenhAn.DoLotId == DoLot.Chuc ? ChonThongTin : "";
            phieuRaVienViewModel.Chat = thongTinBenhAn.DoLotId == DoLot.Chặt ? ChonThongTin : "";
            phieuRaVienViewModel.Lot = thongTinBenhAn.DoLotId == DoLot.Lọt ? ChonThongTin : "";

            phieuRaVienViewModel.DKHaVe = thongTinBenhAn.DuongKinhNhoHaVe;
            phieuRaVienViewModel.KhiVaoKhoa = thongTinBenhAn.TinhHinhSoSinhKhiVaoKhoa;

            phieuRaVienViewModel.PhanBiet = thongTinBenhAn.ChuanDoanPhanBiets != null ? thongTinBenhAn.ChuanDoanPhanBiets.Select(p => p.TenICD).Join("; ") : "";


            phieuRaVienViewModel.GioVaoBuongDe = tongKetBenhAn.VaoBuongDeLuc != null ? tongKetBenhAn.VaoBuongDeLuc.Value.Hour + "" : "";
            phieuRaVienViewModel.PhutVaoBuongDe = tongKetBenhAn.VaoBuongDeLuc != null ? tongKetBenhAn.VaoBuongDeLuc.Value.Minute + "" : "";
            phieuRaVienViewModel.NgayVaoBuongDe = tongKetBenhAn.VaoBuongDeLuc != null ? (tongKetBenhAn.VaoBuongDeLuc ?? DateTime.Now).ApplyFormatDate() : "";

            phieuRaVienViewModel.TenNguoiTD = thongTinBenhAn.TenNhanVienTheoDoi;
            phieuRaVienViewModel.ChucDanh = thongTinBenhAn.TenChucDanh;

            phieuRaVienViewModel.TenNguoiTDSanKhoa = tongKetBenhAn.TenNhanVienTheoDoi;
            phieuRaVienViewModel.ChucDanhSanKhoa = tongKetBenhAn.TenChucDanh;

            phieuRaVienViewModel.GioDe = thongTinBenhAn.DeLuc != null ? thongTinBenhAn.DeLuc.Value.Hour + "" : "";
            phieuRaVienViewModel.PhutDe = thongTinBenhAn.DeLuc != null ? thongTinBenhAn.DeLuc.Value.Minute + "" : "";
            phieuRaVienViewModel.NgayDeSoSinh = thongTinBenhAn.DeLuc != null ? (thongTinBenhAn.DeLuc ?? DateTime.Now).ApplyFormatDate() : "";


            phieuRaVienViewModel.Boc = tongKetBenhAn.Boc == true ? ChonThongTin : null;
            phieuRaVienViewModel.So = tongKetBenhAn.So == true ? ChonThongTin : null;

            phieuRaVienViewModel.GioSoRau = tongKetBenhAn.RauSoLuc != null ? tongKetBenhAn.RauSoLuc.Value.Hour + "" : "";
            phieuRaVienViewModel.PhutSoRau = tongKetBenhAn.RauSoLuc != null ? tongKetBenhAn.RauSoLuc.Value.Minute + "" : "";
            phieuRaVienViewModel.NgaySoRau = tongKetBenhAn.RauSoLuc != null ? (tongKetBenhAn.RauSoLuc ?? DateTime.Now).ApplyFormatDate() : "";

            phieuRaVienViewModel.CachSoRau = tongKetBenhAn.CachSoRau;
            phieuRaVienViewModel.MatMang = tongKetBenhAn.MatMang;
            phieuRaVienViewModel.MatMui = tongKetBenhAn.MatMui;
            phieuRaVienViewModel.BanhRau = tongKetBenhAn.BanhRau;
            phieuRaVienViewModel.CanNangSoRau = tongKetBenhAn.CanNang + "";
            phieuRaVienViewModel.RauCC = tongKetBenhAn.RauCuonCo == true ? ChonThongTin : "";
            phieuRaVienViewModel.CuongRD = tongKetBenhAn.CuonRauDai;
            phieuRaVienViewModel.CoChayMauSC = tongKetBenhAn.CoChayMauSauSo == true ? ChonThongTin : "";
            phieuRaVienViewModel.LuongMauMat = tongKetBenhAn.LuongMauMat + "";
            phieuRaVienViewModel.TuCungSoRau = tongKetBenhAn.KiemSoatTuCung == true ? ChonThongTin : "";
            phieuRaVienViewModel.XuLyVaKQSoRau = tongKetBenhAn.XuLyKetQuaSoRau;

            phieuRaVienViewModel.DaNiemMac = tongKetBenhAn.DaMienMac;


            if (yeuCauTiepNhan.NoiTruBenhAn.LoaiBenhAn == LoaiBenhAn.NhiKhoa)
            {
                phieuRaVienViewModel.DeThuong = thongTinBenhAn.TinhTrangKhiSinh == TinhTrangKhiSinh.DeThuong ? ChonThongTin : "";
                phieuRaVienViewModel.Forceps = thongTinBenhAn.TinhTrangKhiSinh == TinhTrangKhiSinh.Forces ? ChonThongTin : "";
                phieuRaVienViewModel.GiacHut = thongTinBenhAn.TinhTrangKhiSinh == TinhTrangKhiSinh.GiacHut ? ChonThongTin : "";
                phieuRaVienViewModel.PT = thongTinBenhAn.TinhTrangKhiSinh == TinhTrangKhiSinh.DePhauThuat ? ChonThongTin : "";
                phieuRaVienViewModel.DeCH = thongTinBenhAn.TinhTrangKhiSinh == TinhTrangKhiSinh.DeChiHuy ? ChonThongTin : "";
                phieuRaVienViewModel.KhacSP = thongTinBenhAn.TinhTrangKhiSinh == TinhTrangKhiSinh.Khac ? ChonThongTin : "";
            }
            else
            {
                phieuRaVienViewModel.DeThuong = tongKetBenhAn.PhuongPhapDeId == PhuongPhapDe.DeThuong ? ChonThongTin : "";
                phieuRaVienViewModel.Forceps = tongKetBenhAn.PhuongPhapDeId == PhuongPhapDe.Forceps ? ChonThongTin : "";
                phieuRaVienViewModel.GiacHut = tongKetBenhAn.PhuongPhapDeId == PhuongPhapDe.GiacHut ? ChonThongTin : "";
                phieuRaVienViewModel.PT = tongKetBenhAn.PhuongPhapDeId == PhuongPhapDe.PT ? ChonThongTin : "";
                phieuRaVienViewModel.DeCH = tongKetBenhAn.PhuongPhapDeId == PhuongPhapDe.DeChiHuy ? ChonThongTin : "";
                phieuRaVienViewModel.KhacSP = tongKetBenhAn.PhuongPhapDeId == PhuongPhapDe.Khac ? ChonThongTin : "";
            }


            phieuRaVienViewModel.DeThuongNhi = thongTinBenhAn.CachDe == true ? ChonThongTin : "";
            phieuRaVienViewModel.DeMoNhi = thongTinBenhAn.CachDe == false ? ChonThongTin : "";


            phieuRaVienViewModel.LDCanThiep = tongKetBenhAn.LyDoCanThiep;

            phieuRaVienViewModel.KhongRach = tongKetBenhAn.TangSinhMonId == TangSinhMon.KhongRach ? ChonThongTin : "";
            phieuRaVienViewModel.Rach = tongKetBenhAn.TangSinhMonId == TangSinhMon.Rach ? ChonThongTin : "";
            phieuRaVienViewModel.Cat = tongKetBenhAn.TangSinhMonId == TangSinhMon.Cat ? ChonThongTin : "";

            phieuRaVienViewModel.PPLoaiChi = tongKetBenhAn.PhuongPhapKhauVaLoaiChi;
            phieuRaVienViewModel.SMK = tongKetBenhAn.SoMuiKhau + "";

            phieuRaVienViewModel.CTCRach = tongKetBenhAn.CoTuCungId == CoTuCung.Rach ? ChonThongTin : "";
            phieuRaVienViewModel.CTCKRach = tongKetBenhAn.CoTuCungId == CoTuCung.KhongRach ? ChonThongTin : "";

            phieuRaVienViewModel.CDTPhauThuat = tongKetBenhAn.ChanDoanTruocPhauThuat;
            phieuRaVienViewModel.CDSPhauThuat = tongKetBenhAn.ChanDoanSauPhauThuat;

            phieuRaVienViewModel.TaiBienPT = tongKetBenhAn.TaiBien == true ? ChonThongTin : "";
            phieuRaVienViewModel.BienChungPT = tongKetBenhAn.BienChung == true ? ChonThongTin : "";
            phieuRaVienViewModel.DoPT = tongKetBenhAn.DoPhauThuat == true ? ChonThongTin : "";
            phieuRaVienViewModel.DoGayMe = tongKetBenhAn.DoGayMe == true ? ChonThongTin : "";
            phieuRaVienViewModel.DoNK = tongKetBenhAn.DoViKhuan == true ? ChonThongTin : "";
            phieuRaVienViewModel.KhacPT = tongKetBenhAn.DoKhac == true ? ChonThongTin : "";

            phieuRaVienViewModel.TinhHinhToanThan = thongTinBenhAn.TinhHinhToanThan;

            phieuRaVienViewModel.Kg = thongTinBenhAn.CanNang + " " + thongTinBenhAn.CanNangLucSinh + " ";
            phieuRaVienViewModel.DiTatBS = thongTinBenhAn.DiTatBamSinh == true ? ChonThongTin : "";
            phieuRaVienViewModel.CuTheTatBS = thongTinBenhAn.ChuThichDiTatBamSinh + thongTinBenhAn.ThongTinDiTatBamSinh;
            phieuRaVienViewModel.PTTinhThan = thongTinBenhAn.PhatTrienVeTinhThan;
            phieuRaVienViewModel.PTVanDong = thongTinBenhAn.PhatTrienVeVanDong;
            phieuRaVienViewModel.CacBenhLyKhac = thongTinBenhAn.CacBenhLyKhac;

            phieuRaVienViewModel.SuaMe = thongTinBenhAn.NuoiDuong == 1 ? ChonThongTin : null;
            phieuRaVienViewModel.NuoiNT = thongTinBenhAn.NuoiDuong == 2 ? ChonThongTin : null;
            phieuRaVienViewModel.HonHop = thongTinBenhAn.NuoiDuong == 3 ? ChonThongTin : null;

            phieuRaVienViewModel.ThangCaiSua = thongTinBenhAn.CaiSuaThangThu + "";

            phieuRaVienViewModel.TaiVT = thongTinBenhAn.ChamSoc == ChamSoc.TaiVuonTre ? ChonThongTin : null;
            phieuRaVienViewModel.TaiNha = thongTinBenhAn.ChamSoc == ChamSoc.TaiNha ? ChonThongTin : null;

            phieuRaVienViewModel.TCLao = thongTinBenhAn.Lao == true ? ChonThongTin : null;
            phieuRaVienViewModel.TCBaiLiet = thongTinBenhAn.BaiLiet == true ? ChonThongTin : null;
            phieuRaVienViewModel.TCSoi = thongTinBenhAn.Soi == true ? ChonThongTin : null;
            phieuRaVienViewModel.TCHoGa = thongTinBenhAn.HoGa == true ? ChonThongTin : null;
            phieuRaVienViewModel.TCUonVan = thongTinBenhAn.UonVang == true ? ChonThongTin : null;
            phieuRaVienViewModel.TCBachHau = thongTinBenhAn.BachHau == true ? ChonThongTin : null;
            phieuRaVienViewModel.TCKhac = (thongTinBenhAn.Lao != true && thongTinBenhAn.BaiLiet != true
                  && thongTinBenhAn.Soi != true && thongTinBenhAn.HoGa != true && thongTinBenhAn.UonVang != true && thongTinBenhAn.BachHau != true) ? ChonThongTin : null;


            phieuRaVienViewModel.HongHao = thongTinBenhAn.TrangThaiMauSacDa == MauSacDa.HongHao ? ChonThongTin : null;
            phieuRaVienViewModel.TaiXanh = thongTinBenhAn.TrangThaiMauSacDa == MauSacDa.XanhTai ? ChonThongTin : null;
            phieuRaVienViewModel.Vang = thongTinBenhAn.TrangThaiMauSacDa == MauSacDa.Vang ? ChonThongTin : null;
            phieuRaVienViewModel.Tim = thongTinBenhAn.TrangThaiMauSacDa == MauSacDa.Tim ? ChonThongTin : null;
            phieuRaVienViewModel.KhacSoSinh = thongTinBenhAn.TrangThaiMauSacDa == MauSacDa.Khac ? ChonThongTin : null;
            phieuRaVienViewModel.NgheTho = thongTinBenhAn.HoHapNgheTho;
            phieuRaVienViewModel.Sliverman = thongTinBenhAn.HoHapChiSoSilverman;
            phieuRaVienViewModel.TimMach = thongTinBenhAn.TimMachNhipTim;
            phieuRaVienViewModel.Bung = thongTinBenhAn.Bung;


            phieuRaVienViewModel.NhungBenhCuTheTC = thongTinBenhAn.NoiDungTiemChungKhac;

            phieuRaVienViewModel.VongDau = thongTinBenhAn.VongDau + "";
            phieuRaVienViewModel.VongNguc = thongTinBenhAn.VongNguc + "";
            phieuRaVienViewModel.TaiMuiHongRHMMatDD = thongTinBenhAn.TaiMuiHong;
            phieuRaVienViewModel.NgayThu = thongTinBenhAn.ThuMayVaoVien;

            #endregion

            phieuRaVienViewModel.CanNangRaVien = thongTinRaVien.CanNang?.ToString();

            #region CHỈ SỐ SINH TỒN 

            if (thongTinBenhAn.ChiSoSinhTons != null && thongTinBenhAn.ChiSoSinhTons.Any())
            {
                phieuRaVienViewModel.Mach = thongTinBenhAn.ChiSoSinhTons.Last().NhipTim + "";
                phieuRaVienViewModel.NhietDo = thongTinBenhAn.ChiSoSinhTons.Last().ThanNhiet + "";
                phieuRaVienViewModel.HuyetAp = thongTinBenhAn.ChiSoSinhTons.Last().HuyetApTamThu + "/" + thongTinBenhAn.ChiSoSinhTons.Last().HuyetApTamTruong;
                phieuRaVienViewModel.NhipTho = thongTinBenhAn.ChiSoSinhTons.Last().NhipTho + "";
                phieuRaVienViewModel.CanNang = thongTinBenhAn.ChiSoSinhTons.Last().CanNang + "";
                phieuRaVienViewModel.ChieuCao = thongTinBenhAn.ChiSoSinhTons.Last().ChieuCao + "";
                phieuRaVienViewModel.MachSP = thongTinBenhAn.ChiSoSinhTons.Last().NhipTim + "";
                phieuRaVienViewModel.NhietDoSP = thongTinBenhAn.ChiSoSinhTons.Last().ThanNhiet + "";
                phieuRaVienViewModel.HuyetApSP = thongTinBenhAn.ChiSoSinhTons.Last().HuyetAp + "";
                phieuRaVienViewModel.NhipThoSP = thongTinBenhAn.ChiSoSinhTons.Last().NhipTho + "";
            }

            #endregion

            #region THÔNG TIN PHỤ KHOA

            var lstTomTat = new List<string>();
            lstTomTat.Add(tongKetBenhAn.XNMau);
            lstTomTat.Add(tongKetBenhAn.XQuang);
            lstTomTat.Add(tongKetBenhAn.XNTeBao);
            lstTomTat.Add(tongKetBenhAn.SieuAm);
            lstTomTat.Add(tongKetBenhAn.XNBLGP);
            lstTomTat.Add(tongKetBenhAn.XNKhac);

            if (lstTomTat.Any())
            {
                phieuRaVienViewModel.TomTatKQCLSCoGiaTriChanDoan = lstTomTat.Join(" ;");
            }

            #endregion

            #region PHẨU THUẬT THỦ THUẬT , LẦN PHẨU THUẬT

            if (tongKetBenhAn.gridPhauThuatThuThuat != null)
            {
                var contentGrid = string.Empty;
                foreach (var item in tongKetBenhAn.gridPhauThuatThuThuat)
                {
                    var gioPT = item.PTTTNgayGio != null ? (item.PTTTNgayGio ?? DateTime.Now).ApplyFormatDate() : "";
                    contentGrid += "<tr>";
                    contentGrid += "<td>" + gioPT + "</td>";
                    contentGrid += "<td>" + item.PTTTPhuongPhap + "</td>";
                    contentGrid += "<td>" + _nhanVienService.GetNameNhanVienWithNhanVienId(item.PTTTPhauThuatVien).Result + "</td>";
                    contentGrid += "<td>" + _nhanVienService.GetNameNhanVienWithNhanVienId(item.PTTTBacSyGayMe).Result + "</td>";
                    contentGrid += "</tr>";
                }
                phieuRaVienViewModel.LanPhauThuatThuThuat = contentGrid;
            }

            if (tongKetBenhAn.LanPhauThuats != null)
            {
                var contentGrid = string.Empty;
                foreach (var item in tongKetBenhAn.LanPhauThuats)
                {
                    var gioPT = item.PTTTNgayGio != null ? (item.PTTTNgayGio ?? DateTime.Now).ApplyFormatDate() : "";
                    contentGrid += "<tr>";
                    contentGrid += "<td>" + gioPT + "</td>";
                    contentGrid += "<td>" + item.PTTT + "</td>";
                    contentGrid += "<td>" + _nhanVienService.GetNameNhanVienWithNhanVienId(item.PTTTPhauThuatVien).Result + "</td>";
                    contentGrid += "<td>" + _nhanVienService.GetNameNhanVienWithNhanVienId(item.PTTTBacSyGayMe).Result + "</td>";
                    contentGrid += "</tr>";
                }
                phieuRaVienViewModel.TinhHinhPhauThuat = contentGrid;
            }

            #endregion

            #region HÌNH ẢNH KHÁM BỆNH KÈM THEO

            var hinhAnh = string.Empty;
            if (thongTinBenhAn.BoPhanTonThuongs != null)
            {
                foreach (var item in thongTinBenhAn.BoPhanTonThuongs)
                {
                    hinhAnh += "<tr> <th  width=\"25 % \">";
                    hinhAnh += "<img style=\"width:50% \" src=\" " + item.HinhAnh + " \" class=\"mx - auto\" />";
                    hinhAnh += "</th> <td  style=\"vertical-align: top;\" width=\"25 % \"> <div class=\"container\">";
                    hinhAnh += "<div  class=\"value breakword\">" + item.MoTa + "</div>";
                    hinhAnh += "</div> </td> </tr>";
                }
                phieuRaVienViewModel.HinhAnh = hinhAnh;
            }

            #endregion

            #region TỔNG SỐ NGÀY ĐIỀU TRỊ SAU PHẨU THUẬT , TỔNG LẦN PHẨU THUẬT

            var tongNgayDieuTriSauPT = thongTinRaVien.TongSoNgayDieuTriSauPT;

            if (tongNgayDieuTriSauPT > 0)
            {
                var itemSoPTs = tongNgayDieuTriSauPT.ToString().Select(cc => cc.ToString()).ToArray();
                for (int i = 0; i < itemSoPTs.Length; i++)
                {
                    if (i == 0)
                    {
                        phieuRaVienViewModel.MaTNDT1 = itemSoPTs[i];
                    }
                    if (i == 1)
                    {
                        phieuRaVienViewModel.MaTNDT2 = itemSoPTs[i];
                    }
                    if (i == 2)
                    {
                        phieuRaVienViewModel.MaTNDT3 = itemSoPTs[i];
                    }
                }
            }

            var tongLanPT = thongTinRaVien.TongSoLanPT;
            if (tongLanPT > 0)
            {
                var itemSoPTs = tongLanPT.ToString().Select(cc => cc.ToString()).ToArray();
                for (int i = 0; i < itemSoPTs.Length; i++)
                {
                    if (i == 0)
                    {
                        phieuRaVienViewModel.MaTLDT1 = itemSoPTs[i];
                    }
                    if (i == 1)
                    {
                        phieuRaVienViewModel.MaTLDT2 = itemSoPTs[i];
                    }
                }
            }

            #endregion


            var maICDTruocPT = _icdService.GetMaICD(thongTinRaVien.ChuanDoanTruocPhauThuatId ?? 0).Result;
            if (!string.IsNullOrEmpty(maICDTruocPT))
            {
                var fillMAICDTruocPT = FillSquareModel(maICDTruocPT);

                phieuRaVienViewModel.MaCDTPT1 = fillMAICDTruocPT.Square1;
                phieuRaVienViewModel.MaCDTPT2 = fillMAICDTruocPT.Square2;
                phieuRaVienViewModel.MaCDTPT3 = fillMAICDTruocPT.Square3;
                phieuRaVienViewModel.MaCDTPT4 = fillMAICDTruocPT.Square4;

                phieuRaVienViewModel.CDTPT1 = fillMAICDTruocPT.Square1;
                phieuRaVienViewModel.CDTPT2 = fillMAICDTruocPT.Square2;
                phieuRaVienViewModel.CDTPT3 = fillMAICDTruocPT.Square3;
                phieuRaVienViewModel.CDTPT4 = fillMAICDTruocPT.Square4;
            }

            var maICDSauPT = _icdService.GetMaICD(thongTinRaVien.ChuanDoanSauPhauThuatId ?? 0).Result;
            if (!string.IsNullOrEmpty(maICDSauPT))
            {
                var fillMaICDSauPT = FillSquareModel(maICDSauPT);

                phieuRaVienViewModel.MaCDSPT1 = fillMaICDSauPT.Square1;
                phieuRaVienViewModel.MaCDSPT2 = fillMaICDSauPT.Square2;
                phieuRaVienViewModel.MaCDSPT3 = fillMaICDSauPT.Square3;
                phieuRaVienViewModel.MaCDSPT4 = fillMaICDSauPT.Square4;
            }

            //var maICD = _icdService.GetMaICD(thongTinRaVien.ChuanDoanRaVienId ?? 0).Result;
            //if (!string.IsNullOrEmpty(maICD))
            //{
            //    var fillMaICD = FillSquareModel(maICD);

            //    phieuRaVienViewModel.MaCDSPT1 = fillMaICD.Square1;
            //    phieuRaVienViewModel.MaCDSPT2 = fillMaICD.Square2;
            //    phieuRaVienViewModel.MaCDSPT3 = fillMaICD.Square3;
            //    phieuRaVienViewModel.MaCDSPT4 = fillMaICD.Square4;
            //}

            var maLVD = _icdService.GetMaICD(thongTinRaVien.NoiChuanDoanLucVaoDeId ?? 0).Result;
            if (!string.IsNullOrEmpty(maLVD))
            {
                var fillMaLVD = FillSquareModel(maLVD);
                phieuRaVienViewModel.LVDe1 = fillMaLVD.Square1;
                phieuRaVienViewModel.LVDe2 = fillMaLVD.Square2;
                phieuRaVienViewModel.LVDe3 = fillMaLVD.Square3;
                phieuRaVienViewModel.LVDe4 = fillMaLVD.Square4;
            }



            if (thongTinRaVien.TongSoNgayDieuTriSauPT != null)
            {
                var lstStr = thongTinRaVien.TongSoNgayDieuTriSauPT.ToString().ToArray();
                if (lstStr.Length == 1)
                {
                    phieuRaVienViewModel.TSSPT1 = thongTinRaVien.TongSoNgayDieuTriSauPT.ToString();
                }
                else
                {
                    phieuRaVienViewModel.TSSPT1 = lstStr[0] + "";
                    phieuRaVienViewModel.TSSPT2 = thongTinRaVien.TongSoNgayDieuTriSauPT.ToString().Substring(1);
                }
            }

            if (thongTinRaVien.TongSoLanPT != null)
            {
                var lstStr = thongTinRaVien.TongSoLanPT.ToString().ToArray();
                if (lstStr.Length == 1)
                {
                    phieuRaVienViewModel.TSLPT1 = thongTinRaVien.TongSoLanPT.ToString();
                }
                else
                {
                    phieuRaVienViewModel.TSLPT1 = lstStr[0] + "";
                    phieuRaVienViewModel.TSLPT2 = thongTinRaVien.TongSoLanPT.ToString().Substring(1);
                }
            }

            if (thongTinBenhAn.TuNgayDenNgay != null)
            {
                phieuRaVienViewModel.KCTuNgay = thongTinBenhAn.TuNgayDenNgay.startDate != null ? (thongTinBenhAn.TuNgayDenNgay.startDate ?? DateTime.Now).ApplyFormatDate() : "";
                phieuRaVienViewModel.KCDenNgay = thongTinBenhAn.TuNgayDenNgay.endDate != null ? (thongTinBenhAn.TuNgayDenNgay.endDate ?? DateTime.Now).ApplyFormatDate() : "";
            }

            if (thongTinBenhAn.TienSuSanKhoas != null)
            {
                var tienSuSanKhoa = string.Empty;

                foreach (var item in thongTinBenhAn.TienSuSanKhoas)
                {
                    tienSuSanKhoa += "<tr>";

                    tienSuSanKhoa += "<td style=\"text-align:center \">" + item.SoLanCoThai + "</td>";
                    tienSuSanKhoa += "<td style=\"text-align:center \">" + item.Nam + "</td>";
                    tienSuSanKhoa += "<td style=\"text-align:center \"> <span class=\"square2\">" + (item.DeDuThang == true ? ChonThongTin : null) + "</span> </td>";
                    tienSuSanKhoa += "<td style=\"text-align:center \"> <span class=\"square2\" style=\"text-align:center \">" + (item.DeThieuThang == true ? ChonThongTin : null) + "</span> </td>";
                    tienSuSanKhoa += "<td style=\"text-align:center \"> <span class=\"square2\" style=\"text-align:center \">" + (item.Say == true ? ChonThongTin : null) + "</span> </td>";
                    tienSuSanKhoa += "<td style=\"text-align:center \"> <span class=\"square2\" style=\"text-align:center \">" + (item.Hut == true ? ChonThongTin : null) + "</span> </td>";
                    tienSuSanKhoa += "<td style=\"text-align:center \"> <span class=\"square2\" style=\"text-align:center \">" + (item.Nao == true ? ChonThongTin : null) + "</span> </td>";
                    tienSuSanKhoa += "<td style=\"text-align:center \"> <span class=\"square2\" style=\"text-align:center \">" + (item.CoCVat == true ? ChonThongTin : null) + "</span> </td>";
                    tienSuSanKhoa += "<td style=\"text-align:center \"> <span class=\"square2\" style=\"text-align:center \">" + (item.ChuaNgoaiTC == true ? ChonThongTin : null) + "</span> </td>";
                    tienSuSanKhoa += "<td style=\"text-align:center \"> <span class=\"square2\" style=\"text-align:center \">" + (item.ChuaTrung == true ? ChonThongTin : null) + "</span> </td>";
                    tienSuSanKhoa += "<td style=\"text-align:center \"> <span class=\"square2\" style=\"text-align:center \">" + (item.ThaiChetLuu == true ? ChonThongTin : null) + "</span> </td>";
                    tienSuSanKhoa += "<td style=\"text-align:center \"> <span class=\"square2\" style=\"text-align:center \">" + (item.ConHienSong == true ? ChonThongTin : null) + "</span> </td>";

                    tienSuSanKhoa += "<td style=\"text-align:center \">" + item.CanNang + "</td>";
                    tienSuSanKhoa += "<td style=\"text-align:center \">" + item.PhuongPhapDe + "</td>";
                    tienSuSanKhoa += "<td style=\"text-align:center \"> <span class=\"square2\" style=\"text-align:center \">" + (item.TaiBien == true ? ChonThongTin : null) + "</span> </td>";

                    tienSuSanKhoa += "</tr>";
                }

                phieuRaVienViewModel.TienSuSanKhoa = tienSuSanKhoa;
            }

            if (thongTinBenhAn.DacDiemTreSoSinhSauSinhs != null)
            {
                var dacDiemSoSinh = string.Empty;

                foreach (var item in thongTinBenhAn.DacDiemTreSoSinhSauSinhs)
                {
                    dacDiemSoSinh += "<tr>";

                    dacDiemSoSinh += "<td style=\"text-align:center \">" + (item.Diem) + "</td>";
                    dacDiemSoSinh += "<td style=\"text-align:center  \"> " + (item.SuGianNoLongNguc) + " </td>";
                    dacDiemSoSinh += "<td style=\"text-align:center  \"> " + (item.CoKeoCoLienSuon) + "</td>";
                    dacDiemSoSinh += "<td style=\"text-align:center  \"> " + (item.CoKeoMuiUc) + " </td>";
                    dacDiemSoSinh += "<td style=\"text-align:center  \"> " + (item.DapCanhMui) + " </td>";
                    dacDiemSoSinh += "<td style=\"text-align:center \">" + (item.Reni) + "</td>";
                    dacDiemSoSinh += "</tr>";
                }

                phieuRaVienViewModel.DacDiemTreSoSinhSauSinhs = dacDiemSoSinh;

            }

            if (thongTinRaVien.ChonThai != null)
            {
                if (thongTinRaVien.ChonThai == true)
                {
                    phieuRaVienViewModel.DonThai = ChonThongTin;

                    if (thongTinRaVien.ChonTrai == true)
                    {
                        phieuRaVienViewModel.Trai = ChonThongTin;
                    }
                    else
                    {
                        phieuRaVienViewModel.Gai = ChonThongTin;
                    }

                    if (thongTinRaVien.ChonSongChet == true)
                    {
                        phieuRaVienViewModel.Song = ChonThongTin;
                    }
                    else
                    {
                        phieuRaVienViewModel.Chet = ChonThongTin;
                    }
                }
                else
                {
                    phieuRaVienViewModel.DaThai = ChonThongTin;

                    phieuRaVienViewModel.Trai = thongTinRaVien.SoLuongTrai + "";
                    phieuRaVienViewModel.Gai = thongTinRaVien.SoLuongGai + "";
                    phieuRaVienViewModel.Song = thongTinRaVien.SoLuongTraiSong + thongTinRaVien.SoLuongGaiSong + "";
                    phieuRaVienViewModel.Chet = thongTinRaVien.SoLuongTraiChet + thongTinRaVien.SoLuongGaiChet + "";
                }
            }

            phieuRaVienViewModel.XuongKhop = thongTinBenhAn.XuongKhop;
            phieuRaVienViewModel.ThanKinhPhanXa = thongTinBenhAn.ThanKinhPhanXa;
            phieuRaVienViewModel.ThanKinhTruongLucHoc = thongTinBenhAn.ThanKinhTruongLucCo;
            phieuRaVienViewModel.CoQuanSinhDucNgoai = thongTinBenhAn.CacCoQuanSinhDucNgoai;
            phieuRaVienViewModel.CacXNCLSCanLam = thongTinBenhAn.CacXetNghiemCanLam;
            phieuRaVienViewModel.ChiDinhTheoDoi = thongTinBenhAn.ChiDinhTheoDoi;

            if (tongKetBenhAn.DacDiemTreSoSinhs != null)
            {
                var dataDacDiemTre = string.Empty;

                var donThai = tongKetBenhAn.DacDiemTreSoSinhs.Count == 1;

                int countItem = 1;
                foreach (var item in tongKetBenhAn.DacDiemTreSoSinhs)
                {
                    if (countItem != 1)
                    {
                        dataDacDiemTre += "<tr style=\"height: 15px\"> <td> </td> </tr>";
                    }

                    dataDacDiemTre += "<tr><td colspan=\"2\"><div class='container'><div>- Đẻ lúc:<span class='valuedownline'> " + (item.DeLuc != null ? item.DeLuc.Value.Hour + "" : "") + "</span> giờ<span class='valuedownline'> " + (item.DeLuc != null ? item.DeLuc.Value.Minute + "" : "") + "</span> phút, ngày<span class='valuedownline'> " + (item.DeLuc != null ? (item.DeLuc ?? DateTime.Now).ApplyFormatDate() : "") + "</span>"
                        + " -ApGar:  1 phút <span class='valuedownline'>" + (item.ApGar == 1 ? item.ChiSoApGar?.ToString() + "" : "...") + "</span> &nbsp;điểm 5 phút <span class='valuedownline'>" + (item.ApGar5 == 2 ? item.ChiSoApGar5?.ToString() + "" : "...") + "</span> &nbsp;điểm 10 phút <span class='valuedownline'>" + (item.ApGar10 == 3 ? item.ChiSoApGar10?.ToString() + "" : "...") + "</span> &nbsp;điểm</div></div><td>";
                    dataDacDiemTre += "<td><div class='container'><div></div></div><td></tr>";

                    ////new tr tag
                    dataDacDiemTre += "<tr> <td> <div class=\"containerGD\"> <div class=\"label\">- Cân nặng: </div>";
                    dataDacDiemTre += "<div class=\"value\" style=\"width: 20 % \">" + item.CanNang + "</div>";
                    dataDacDiemTre += "<div class=\"label\"> &nbsp; gram &nbsp;&nbsp; Cao: </div>";
                    dataDacDiemTre += "<div class=\"value\" style=\"width: 20 % \">" + item.Cao + "</div>";
                    dataDacDiemTre += "<div class=\"label\"> &nbsp; cm,  &nbsp;&nbsp; Vòng đầu: </div>";
                    dataDacDiemTre += "<div class=\"value\" style=\"width: 38 % \">" + item.VongDau + " </div> &nbsp;cm </div> </div> </td> </tr>";

                    ////new tr tag
                    dataDacDiemTre += "<tr> <td colspan=\"2\"> <div class=\"containergd\"> <div class=\"label\">- Con: - đơn thai";
                    dataDacDiemTre += "<i> 1.trai </i> <span class=\"square3\"> " + ((donThai && item.GioiTinhId == EnumGioiTinh.Trai) ? ChonThongTin : null) + " </span>";
                    dataDacDiemTre += "<i> 2.gái </i> <span class=\"square3\"> " + ((donThai && item.GioiTinhId != EnumGioiTinh.Trai) ? ChonThongTin : null) + " </span> - đa thai:";
                    dataDacDiemTre += "<i> 1.trai </i> <span class=\"square3\"> " + ((!donThai && item.GioiTinhId == EnumGioiTinh.Trai) ? ChonThongTin : null) + " </span>";
                    dataDacDiemTre += "<i> 2.gái </i> <span class=\"square3\"> " + ((!donThai && item.GioiTinhId != EnumGioiTinh.Trai) ? ChonThongTin : null) + " </span>";
                    dataDacDiemTre += "<i>- tật bẩm sinh:</i> <span class=\"square3\"> " + ((item.DiTat != null) ? ChonThongTin : null) + " </span>";
                    dataDacDiemTre += "<i>- có hậu môn:</i> <span class=\"square3\"> " + ((item.CoHauMon.ToString() == "True") ? ChonThongTin : null) + " </span> </div> </div> </td> <td> <div class=\"containergd\"> <div class=\"label\"> </div> </td> </tr>";

                    //new tr tag
                    dataDacDiemTre += "<tr> <td colspan=\"3\"> <div class=\"container\"> - Cụ thể bẩm sinh:";
                    dataDacDiemTre += "<div class=\"valuedownline\">" + item.DiTat + " </div> </div> </td> </tr>";

                    //new tr tag
                    dataDacDiemTre += "<tr> <td colspan=\"3\"> <div class=\"container\"> - Tình trạng thể sơ sinh sau khi đẻ:";
                    dataDacDiemTre += "<div class=\"valuedownline\">" + item.TinhTrang + "</div> </div> </td> </tr>";

                    //new tr tag
                    dataDacDiemTre += "<tr> <td colspan=\"3\"> <div class=\"container\"> - Xử lý và kết quả:";
                    dataDacDiemTre += "<div class=\"valuedownline\">" + item.KetQuaXuLy + "</div> </div> </td> </tr>";

                    countItem++;
                }

                phieuRaVienViewModel.DacDiemTreSoSinhList = dataDacDiemTre;
            }

            return phieuRaVienViewModel;
        }

        private FillSquareModel FillSquareModel(string fillData)
        {
            var fillSquareModel = new FillSquareModel();
            var dataFillText = fillData.Select(cc => cc.ToString()).ToArray();

            if (dataFillText.Length == 1)
            {
                fillSquareModel.Square1 = dataFillText[0];
            }
            else if (dataFillText.Length == 2)
            {
                fillSquareModel.Square1 = dataFillText[0];
                fillSquareModel.Square2 = dataFillText[1];
            }
            else if (dataFillText.Length == 3)
            {
                fillSquareModel.Square1 = dataFillText[0];
                fillSquareModel.Square2 = dataFillText[1];
                fillSquareModel.Square3 = dataFillText[2];

            }
            else
            {
                fillSquareModel.Square1 = dataFillText[0];
                fillSquareModel.Square2 = dataFillText[1];
                fillSquareModel.Square3 = dataFillText[2];
                fillSquareModel.Square4 = fillData.Substring(3);
            }
            return fillSquareModel;
        }

        private void LuuBenhAnTruocKhiNhapThongTin
            (LoaiBenhAn loaiBenhAn, long yeuCauTiepNhanId, NoiTruBenhAn noiTruBenhAn, YeuCauTiepNhan entity)
        {
            #region Thông tin bệnh án 

            if (noiTruBenhAn != null && noiTruBenhAn.ThongTinBenhAn == null)
            {
                //Ta sẽ lưu thông tin bệnh án nếu chưa tạo chức năng lưu lại trước những thông tin lấy bên bệnh viện
                switch (loaiBenhAn)
                {
                    case LoaiBenhAn.NhiKhoa:
                        _dieuTriNoiTruService.GetThongTinBenhAnNoiKhoaNhi(yeuCauTiepNhanId);
                        break;
                    case LoaiBenhAn.NoiKhoa:
                        _dieuTriNoiTruService.GetThongTinBenhAnNoiKhoaNhi(yeuCauTiepNhanId);
                        break;
                    case LoaiBenhAn.PhuKhoa:
                        _dieuTriNoiTruService.GetThongTinBenhAnPhuKhoa(yeuCauTiepNhanId);
                        break;
                    case LoaiBenhAn.NgoaiKhoa:
                        _dieuTriNoiTruService.GetThongTinBenhAnNgoaiKhoa(yeuCauTiepNhanId);
                        break;
                    case LoaiBenhAn.ThamMy:
                        _dieuTriNoiTruService.GetThongTinBenhAnNgoaiKhoa(yeuCauTiepNhanId);
                        break;
                    case LoaiBenhAn.SanKhoaMo:
                        _dieuTriNoiTruService.GetThongTinBenhAnSK(yeuCauTiepNhanId);
                        break;
                    case LoaiBenhAn.SanKhoaThuong:
                        _dieuTriNoiTruService.GetThongTinBenhAnSK(yeuCauTiepNhanId);
                        break;
                    case LoaiBenhAn.TreSoSinh:
                        _dieuTriNoiTruService.GetThongTinBenhAnTreSoSinh(yeuCauTiepNhanId);
                        break;
                }

            }

            #endregion

            #region Tổng kết bệnh án

            if (noiTruBenhAn != null && noiTruBenhAn.ThongTinTongKetBenhAn == null)
            {
                var result = new DieuTriNoiTruTongKetBenhAnViewModel();

                result.gridPhauThuatThuThuat = GetDanhSachPTTT(yeuCauTiepNhanId);
                result.LanPhauThuats = GetDanhSachPTTT(yeuCauTiepNhanId);

                noiTruBenhAn.ThongTinTongKetBenhAn = JsonConvert.SerializeObject(result);
                _dieuTriNoiTruService.Update(entity);

            }

            #endregion
        }


        private void KiemTraChuanDoanKemTheo(RaVien raVien)
        {
            //kiểm tra chẩn đoán phân biệt
            if (raVien.ChuanDoanKemTheos != null)
            {
                var lstChanDoanPhanBietId = raVien.ChuanDoanKemTheos.Where(x => x.Id == 0).Select(x => x.ICD).ToList();
                if (lstChanDoanPhanBietId.Count() != lstChanDoanPhanBietId.Distinct().Count())
                {
                    throw new ApiException("Chuẩn đoán kèm theo không được trùng");
                }
            }
        }

        private static readonly string KhongCoDuLieu = string.Empty;
        private static readonly string ChonThongTin = "X";

        #endregion export
        [HttpPost("IsCheckThongTinBenhAnDaKetThuc")]
        public ActionResult IsCheckThongTinBenhAnDaKetThuc(long id)
        {
            var result = _dieuTriNoiTruService.IsCheckThongTinBenhAnDaKetThuc(id);
            return Ok(result);
        }
    }
}
