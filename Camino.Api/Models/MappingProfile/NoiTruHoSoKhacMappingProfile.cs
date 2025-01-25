using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.BangKiemAnToanNBPT;
using Camino.Api.Models.BangKiemAnToanPhauThuatTuPhongDieuTri;
using Camino.Api.Models.BienBanHoiChanSuDungThuocCoDau;
using Camino.Api.Models.DieuTriNoiTru;
using Camino.Api.Models.GiayCamKetSuDungThuocNgoaiBH;
using Camino.Api.Models.GiayCamKetTuNguyenSuDungThuocDichVuNgoaiBaoHiemYTe;
using Camino.Api.Models.GiayChungNhanNghiViecHuongBHXH;
using Camino.Api.Models.GiayChungNhanPhauThuat;
using Camino.Api.Models.GiayChungSinhMangThaiHo;
using Camino.Api.Models.GiayNghiDuongThaiNoiTru;
using Camino.Api.Models.PhieuCongKhaiDichVuKyThuat;
using Camino.Api.Models.PhieuCongKhaiThuoc;
using Camino.Api.Models.PhieuCongKhaiVatTu;
using Camino.Api.Models.PhieuDeNghiTestTruocKhiDungThuoc;
using Camino.Api.Models.PhieuKhaiThacTienSuBenh;
using Camino.Api.Models.PhieuSoKet15NgayDieuTri;
using Camino.Api.Models.PhieuTheoDoiTruyenDich;
using Camino.Api.Models.PhieuTheoDoiTruyenMau;
using Camino.Api.Models.TrichBienBanHoiChan;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.MappingProfile
{
    public class NoiTruHoSoKhacMappingProfile : Profile
    {
        public NoiTruHoSoKhacMappingProfile()
        {
            #region Trích biên bản hội chẩn
            CreateMap<Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhac, TrichBienBanHoiChanViewModel>();

            CreateMap<TrichBienBanHoiChanViewModel, Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhac>();
            #endregion
            #region biên bản hội chẩn phẩu thuật
            CreateMap<Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhac, BienBanHoiChanPhauThuatModel>();

            CreateMap<BienBanHoiChanPhauThuatModel, Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhac>();
            #endregion
            #region Phiếu sơ kết 15 ngày
            //CreateMap<Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhac, PhieuSoKet15NgayDieuTriViewModel>();
            //CreateMap<PhieuSoKet15NgayDieuTriViewModel, Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhac>();
            #endregion
            #region map FileChuKyViewModel 
            //CreateMap<Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhacFileDinhKem, FileChuKyViewModel>();

            //CreateMap<FileChuKyViewModel, Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhacFileDinhKem>();
            #endregion
            #region map phieu khai thac tien su benh
            CreateMap<Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhac, PhieuKhaiThacTienSuBenhViewModel>();

            CreateMap<PhieuKhaiThacTienSuBenhViewModel, Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhac>();
            #region map FileChuKyViewModel 
            CreateMap<Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhacFileDinhKem, FileChuKyPhieuKhaiThacTienSuBenhViewModel>();

            CreateMap<FileChuKyPhieuKhaiThacTienSuBenhViewModel, Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhacFileDinhKem>();
            #endregion
            #endregion
            #region bang kiem an toan toan phau thuat
            CreateMap<Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhac, BangKiemAnToanNBPTViewModel>();

            CreateMap<BangKiemAnToanNBPTViewModel, Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhac>();
            #endregion
            #region Bảng kiểm điểm an toàn người bệnh pt từ phòng điều trị
            CreateMap<Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhac, BangKiemAnToanPhauThuatTuPhongDieuTriViewModel>();

            CreateMap<BangKiemAnToanPhauThuatTuPhongDieuTriViewModel, Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhac>();
            #endregion
            #region Map phiếu sàng lọc dinh dưỡng
            CreateMap<NoiTruHoSoKhac, DieuTriNoiTru.HoSoKhacPhieuSangLocDinhDuongViewModel>().IgnoreAllNonExisting()
                .ForMember(p => p.NhanVienThucHienDisplay, o => o.MapFrom(p2 => p2.NhanVienThucHien.User.HoTen))
                .ForMember(p => p.NoiThucHienDisplay, o => o.MapFrom(p2 => p2.NoiThucHien.Ten))
                .AfterMap((s, d) => ViewModelMappingPhieuSangLocDinhDuong(s, d));
            CreateMap<DieuTriNoiTru.HoSoKhacPhieuSangLocDinhDuongViewModel, NoiTruHoSoKhac>().IgnoreAllNonExisting()
                .AfterMap((s, d) => AddOrUpdatePhieuSangLocDinhDuong(s, d));

            CreateMap<NoiTruHoSoKhacFileDinhKem, DieuTriNoiTru.FileChuKyPhieuSangLocDinhDuongViewModel>().IgnoreAllNonExisting();
            CreateMap<DieuTriNoiTru.FileChuKyPhieuSangLocDinhDuongViewModel, NoiTruHoSoKhacFileDinhKem>().IgnoreAllNonExisting();
            #endregion
            #region Map phiếu theo dõi chức năng sống
            CreateMap<NoiTruHoSoKhac, DieuTriNoiTru.HoSoKhacPhieuTheoDoiChucNangSongViewModel>().IgnoreAllNonExisting()
                .ForMember(p => p.NhanVienThucHienDisplay, o => o.MapFrom(p2 => p2.NhanVienThucHien.User.HoTen))
                .ForMember(p => p.NoiThucHienDisplay, o => o.MapFrom(p2 => p2.NoiThucHien.Ten))
                .AfterMap((s, d) => ViewModelMappingPhieuTheoDoiChucNangSong(s, d));
            CreateMap<DieuTriNoiTru.HoSoKhacPhieuTheoDoiChucNangSongViewModel, NoiTruHoSoKhac>().IgnoreAllNonExisting()
                .AfterMap((s, d) => AddOrUpdatePhieuTheoDoiChucNangSong(s, d));

            CreateMap<NoiTruHoSoKhacFileDinhKem, DieuTriNoiTru.FileChuKyPhieuTheoDoiChucNangSongViewModel>().IgnoreAllNonExisting();
            CreateMap<DieuTriNoiTru.FileChuKyPhieuTheoDoiChucNangSongViewModel, NoiTruHoSoKhacFileDinhKem>().IgnoreAllNonExisting();
            #endregion
            #region Map giấy tự nguyện triệt sản
            CreateMap<NoiTruHoSoKhac, DieuTriNoiTru.HoSoKhacGiayTuNguyenTrietSanViewModel>().IgnoreAllNonExisting()
                .ForMember(p => p.NhanVienThucHienDisplay, o => o.MapFrom(p2 => p2.NhanVienThucHien.User.HoTen))
                .ForMember(p => p.NoiThucHienDisplay, o => o.MapFrom(p2 => p2.NoiThucHien.Ten))
                .AfterMap((s, d) => ViewModelMappingGiayTuNguyenTrietSan(s, d));
            CreateMap<DieuTriNoiTru.HoSoKhacGiayTuNguyenTrietSanViewModel, NoiTruHoSoKhac>().IgnoreAllNonExisting()
                .AfterMap((s, d) => AddOrUpdateGiayTuNguyenTrietSan(s, d));

            CreateMap<NoiTruHoSoKhacFileDinhKem, DieuTriNoiTru.FileChuKyGiayTuNguyenTrietSanViewModel>().IgnoreAllNonExisting();
            CreateMap<DieuTriNoiTru.FileChuKyGiayTuNguyenTrietSanViewModel, NoiTruHoSoKhacFileDinhKem>().IgnoreAllNonExisting();
            #endregion
            #region Map biên bản cam kết gây tê giảm đau trong đẻ - sau mổ
            CreateMap<NoiTruHoSoKhac, DieuTriNoiTru.HoSoKhacBienBanCamKetGayTeGiamDauTrongDeSauMoViewModel>().IgnoreAllNonExisting()
                .ForMember(p => p.NhanVienThucHienDisplay, o => o.MapFrom(p2 => p2.NhanVienThucHien.User.HoTen))
                .ForMember(p => p.NoiThucHienDisplay, o => o.MapFrom(p2 => p2.NoiThucHien.Ten))
                .AfterMap((s, d) => ViewModelMappingBienBanCamKetGayTeGiamDauTrongDeSauMo(s, d));
            CreateMap<DieuTriNoiTru.HoSoKhacBienBanCamKetGayTeGiamDauTrongDeSauMoViewModel, NoiTruHoSoKhac>().IgnoreAllNonExisting()
                .AfterMap((s, d) => AddOrUpdateBienBanCamKetGayTeGiamDauTrongDeSauMo(s, d));

            CreateMap<NoiTruHoSoKhacFileDinhKem, DieuTriNoiTru.FileChuKyBienBanCamKetGayTeGiamDauTrongDeSauMoViewModel>().IgnoreAllNonExisting();
            CreateMap<DieuTriNoiTru.FileChuKyBienBanCamKetGayTeGiamDauTrongDeSauMoViewModel, NoiTruHoSoKhacFileDinhKem>().IgnoreAllNonExisting();
            #endregion
            #region Map bản theo dõi gây mê hồi sức
            CreateMap<NoiTruHoSoKhac, DieuTriNoiTru.HoSoKhacBangTheoDoiGayMeHoiSucViewModel>().IgnoreAllNonExisting()
                .ForMember(p => p.NhanVienThucHienDisplay, o => o.MapFrom(p2 => p2.NhanVienThucHien.User.HoTen))
                .ForMember(p => p.NoiThucHienDisplay, o => o.MapFrom(p2 => p2.NoiThucHien.Ten))
                .AfterMap((s, d) => ViewModelMappingBangTheoDoiGayMeHoiSuc(s, d));
            CreateMap<DieuTriNoiTru.HoSoKhacBangTheoDoiGayMeHoiSucViewModel, NoiTruHoSoKhac>().IgnoreAllNonExisting()
                .AfterMap((s, d) => AddOrUpdateBangTheoDoiGayMeHoiSuc(s, d));

            CreateMap<NoiTruHoSoKhacFileDinhKem, DieuTriNoiTru.FileChuKyBangTheoDoiGayMeHoiSucViewModel>().IgnoreAllNonExisting();
            CreateMap<DieuTriNoiTru.FileChuKyBangTheoDoiGayMeHoiSucViewModel, NoiTruHoSoKhacFileDinhKem>().IgnoreAllNonExisting();
            #endregion
            #region Map phiếu công khai thuốc
            CreateMap<Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhac, PhieuCongKhaiThuocViewModel>();

            CreateMap<PhieuCongKhaiThuocViewModel, Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhac>();
            #endregion
            #region Map phiếu công khai vật tư
            CreateMap<Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhac, PhieuCongKhaiVatTuViewModel>();

            CreateMap<PhieuCongKhaiVatTuViewModel, Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhac>();
            #endregion

            #region Map phiếu công khai vật tư

            CreateMap<Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhac, PhieuCongKhaiDichVuKyThuatViewModel>();
            CreateMap<PhieuCongKhaiDichVuKyThuatViewModel, Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhac>();

            #endregion


            #region Map phiếu công khai thuốc vật tư

            CreateMap<Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhac, PhieuCongKhaiThuocVatTuViewModel>();
            CreateMap<PhieuCongKhaiThuocVatTuViewModel, Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhac>();

            #endregion

            #region Map phiếu đề nghị test trước khi dùng
            CreateMap<Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhac, PhieuDeNghiTestTruocKhiDungThuocViewModel>();

            CreateMap<PhieuDeNghiTestTruocKhiDungThuocViewModel, Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhac>();
            #endregion
            #region Map giấy chuyển tuyến
            CreateMap<NoiTruHoSoKhac, DieuTriNoiTru.HoSoKhacGiayChuyenTuyenViewModel>().IgnoreAllNonExisting()
                .ForMember(p => p.NhanVienThucHienDisplay, o => o.MapFrom(p2 => p2.NhanVienThucHien.User.HoTen))
                .ForMember(p => p.NoiThucHienDisplay, o => o.MapFrom(p2 => p2.NoiThucHien.Ten));
            CreateMap<DieuTriNoiTru.HoSoKhacGiayChuyenTuyenViewModel, NoiTruHoSoKhac>().IgnoreAllNonExisting()
                .AfterMap((s, d) => AddOrUpdateGiayChuyenTuyen(s, d));
            #endregion
            #region Map phiếu theo dõi truyền dịch
            CreateMap<Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhac, PhieuTheoDoiTruyenDichViewModel>();

            CreateMap<PhieuTheoDoiTruyenDichViewModel, Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhac>();
            #endregion
            #region Map phiếu theo dõi truyền máu
            CreateMap<Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhac, PhieuTheoDoiTruyenMauViewModel>();

            CreateMap<PhieuTheoDoiTruyenMauViewModel, Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhac>();
            #endregion
            #region Map phiếu giấy cam kết tự nguyện sử dụng thuốc
            CreateMap<Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhac, GiayCamKetTuNguyenSuDungThuocDichVuNgoaiBaoHiemYTeViewModel>();

            CreateMap<GiayCamKetTuNguyenSuDungThuocDichVuNgoaiBaoHiemYTeViewModel, Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhac>();
            #endregion
            #region Map phiếu giấy chứng nhận nghỉ việc hưởng bhxh
            CreateMap<Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhac, GiayChungNhanNghiViecHuongBHXHViewModel>();

            CreateMap<GiayChungNhanNghiViecHuongBHXHViewModel, Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhac>();
            #endregion
            #region Map phiếu giấy chứng sinh mang thai hộ
            CreateMap<Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhac, GiayChungSinhMangThaiHoViewModel>();

            CreateMap<GiayChungSinhMangThaiHoViewModel, Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhac>();
            #endregion
            #region Map biểu đồ chuyển dạ
            CreateMap<NoiTruHoSoKhac, DieuTriNoiTru.HoSoKhacBieuDoChuyenDaViewModel>().IgnoreAllNonExisting()
                .ForMember(p => p.NhanVienThucHienDisplay, o => o.MapFrom(p2 => p2.NhanVienThucHien.User.HoTen))
                .ForMember(p => p.NoiThucHienDisplay, o => o.MapFrom(p2 => p2.NoiThucHien.Ten))
                .AfterMap((s, d) => ViewModelMappingBieuDoChuyenDa(s, d));
            CreateMap<DieuTriNoiTru.HoSoKhacBieuDoChuyenDaViewModel, NoiTruHoSoKhac>().IgnoreAllNonExisting()
                .AfterMap((s, d) => AddOrUpdateBieuDoChuyenDa(s, d));

            CreateMap<NoiTruHoSoKhacFileDinhKem, DieuTriNoiTru.FileChuKyBieuDoChuyenDaViewModel>().IgnoreAllNonExisting();
            CreateMap<DieuTriNoiTru.FileChuKyBieuDoChuyenDaViewModel, NoiTruHoSoKhacFileDinhKem>().IgnoreAllNonExisting();
            #endregion
            #region Biên bản hội chẩn sử dụng thuốc có dấu
            CreateMap<Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhac, BienBanHoiChanSuDungThuocCoDauViewModel>();

            CreateMap<BienBanHoiChanSuDungThuocCoDauViewModel, Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhac>();
            #endregion
            #region hồ sơ chăm sóc điều dưỡng hộ sinh
            CreateMap<Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhac, HoSoChamSocDieuDuongHoSinhViewModel>();
            CreateMap<HoSoChamSocDieuDuongHoSinhViewModel, Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhac>();
            #endregion
            #region Map phiếu giấy nghỉ dưỡng thai
            CreateMap<Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhac, GiayChungNhanNghiDuongThaiViewModel>();

            CreateMap<GiayChungNhanNghiDuongThaiViewModel, Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhac>();
            #endregion
            #region giấy chứng nhận phẩu thuật 
            CreateMap<Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhac, GiayChungNhanPhauThuatViewModel>();

            CreateMap<GiayChungNhanPhauThuatViewModel, Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhac>();
            #endregion
            #region Giấy cam kết sử dụng thuốc ngoài bảo hiểm
            CreateMap<Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhac, GiayCamKetSuDungThuocNgoaiBHViewModel>();

            CreateMap<GiayCamKetSuDungThuocNgoaiBHViewModel, Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhac>();
            #endregion
            #region giấy cam kết gây tê giảm đau trong đẻ - sau phẫu thuật
            CreateMap<Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhac, GiayCamKetGayMeGiamDauTrongDeSauPhauThuatViewModel>();

            CreateMap<GiayCamKetGayMeGiamDauTrongDeSauPhauThuatViewModel, Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhac>();
            #endregion
        }

        #region Phiếu sàng lọc dinh dưỡng
        private void ViewModelMappingPhieuSangLocDinhDuong(NoiTruHoSoKhac s, DieuTriNoiTru.HoSoKhacPhieuSangLocDinhDuongViewModel d)
        {
            foreach(var item in s.NoiTruHoSoKhacFileDinhKems)
            {
                var chuKy = item.ToModel<DieuTriNoiTru.FileChuKyPhieuSangLocDinhDuongViewModel>();
                d.FilesChuKy.Add(chuKy);
            }
        }

        private void AddOrUpdatePhieuSangLocDinhDuong(DieuTriNoiTru.HoSoKhacPhieuSangLocDinhDuongViewModel s, NoiTruHoSoKhac d)
        {
            d.LoaiHoSoDieuTriNoiTru = LoaiHoSoDieuTriNoiTru.PhieuSangLocDinhDuong;

            foreach (var item in d.NoiTruHoSoKhacFileDinhKems)
            {
                if (s.FilesChuKy == null || !s.FilesChuKy.Any(p => p.TenGuid == item.TenGuid))
                {
                    item.WillDelete = true;
                }
            }

            if (s.FilesChuKy != null)
            {
                foreach (var item in s.FilesChuKy)
                {
                    if (!d.NoiTruHoSoKhacFileDinhKems.Any(p => p.TenGuid == item.TenGuid))
                    {
                        var chuKy = item.ToEntity<NoiTruHoSoKhacFileDinhKem>();
                        d.NoiTruHoSoKhacFileDinhKems.Add(chuKy);
                    }
                }
            }
        }
        #endregion
        #region Phiếu theo dõi chức năng sống
        private void ViewModelMappingPhieuTheoDoiChucNangSong(NoiTruHoSoKhac s, DieuTriNoiTru.HoSoKhacPhieuTheoDoiChucNangSongViewModel d)
        {
            foreach (var item in s.NoiTruHoSoKhacFileDinhKems)
            {
                var chuKy = item.ToModel<DieuTriNoiTru.FileChuKyPhieuTheoDoiChucNangSongViewModel>();
                d.FilesChuKy.Add(chuKy);
            }
        }

        private void AddOrUpdatePhieuTheoDoiChucNangSong(DieuTriNoiTru.HoSoKhacPhieuTheoDoiChucNangSongViewModel s, NoiTruHoSoKhac d)
        {
            d.LoaiHoSoDieuTriNoiTru = LoaiHoSoDieuTriNoiTru.PhieuTheoDoiChucNangSong;

            foreach (var item in d.NoiTruHoSoKhacFileDinhKems)
            {
                if (s.FilesChuKy == null || !s.FilesChuKy.Any(p => p.TenGuid == item.TenGuid))
                {
                    item.WillDelete = true;
                }
            }

            if(s.FilesChuKy != null)
            {
                foreach (var item in s.FilesChuKy)
                {
                    if (!d.NoiTruHoSoKhacFileDinhKems.Any(p => p.TenGuid == item.TenGuid))
                    {
                        var chuKy = item.ToEntity<NoiTruHoSoKhacFileDinhKem>();
                        d.NoiTruHoSoKhacFileDinhKems.Add(chuKy);
                    }
                }
            }
        }
        #endregion
        #region Giấy tự nguyện triệt sản
        private void ViewModelMappingGiayTuNguyenTrietSan(NoiTruHoSoKhac s, DieuTriNoiTru.HoSoKhacGiayTuNguyenTrietSanViewModel d)
        {
            foreach (var item in s.NoiTruHoSoKhacFileDinhKems)
            {
                var chuKy = item.ToModel<DieuTriNoiTru.FileChuKyGiayTuNguyenTrietSanViewModel>();
                d.FilesChuKy.Add(chuKy);
            }
        }

        private void AddOrUpdateGiayTuNguyenTrietSan(DieuTriNoiTru.HoSoKhacGiayTuNguyenTrietSanViewModel s, NoiTruHoSoKhac d)
        {
            d.LoaiHoSoDieuTriNoiTru = LoaiHoSoDieuTriNoiTru.GiayTuNguyenTrietSan;

            foreach (var item in d.NoiTruHoSoKhacFileDinhKems)
            {
                if (s.FilesChuKy == null || !s.FilesChuKy.Any(p => p.TenGuid == item.TenGuid))
                {
                    item.WillDelete = true;
                }
            }

            if (s.FilesChuKy != null)
            {
                foreach (var item in s.FilesChuKy)
                {
                    if(!d.NoiTruHoSoKhacFileDinhKems.Any(p => p.TenGuid == item.TenGuid))
                    {
                        var chuKy = item.ToEntity<NoiTruHoSoKhacFileDinhKem>();
                        d.NoiTruHoSoKhacFileDinhKems.Add(chuKy);
                    }
                }
            }
        }
        #endregion
        #region Biên bản cam kết gây tê giảm đau trong đẻ - sau mổ
        private void ViewModelMappingBienBanCamKetGayTeGiamDauTrongDeSauMo(NoiTruHoSoKhac s, DieuTriNoiTru.HoSoKhacBienBanCamKetGayTeGiamDauTrongDeSauMoViewModel d)
        {
            foreach (var item in s.NoiTruHoSoKhacFileDinhKems)
            {
                var chuKy = item.ToModel<DieuTriNoiTru.FileChuKyBienBanCamKetGayTeGiamDauTrongDeSauMoViewModel>();
                d.FilesChuKy.Add(chuKy);
            }
        }

        private void AddOrUpdateBienBanCamKetGayTeGiamDauTrongDeSauMo(DieuTriNoiTru.HoSoKhacBienBanCamKetGayTeGiamDauTrongDeSauMoViewModel s, NoiTruHoSoKhac d)
        {
            d.LoaiHoSoDieuTriNoiTru = LoaiHoSoDieuTriNoiTru.BienBanCamKetGayTeGiamDauTrongDeSauMo;

            foreach (var item in d.NoiTruHoSoKhacFileDinhKems)
            {
                if (s.FilesChuKy == null || !s.FilesChuKy.Any(p => p.TenGuid == item.TenGuid))
                {
                    item.WillDelete = true;
                }
            }

            if (s.FilesChuKy != null)
            {
                foreach (var item in s.FilesChuKy)
                {
                    if (!d.NoiTruHoSoKhacFileDinhKems.Any(p => p.TenGuid == item.TenGuid))
                    {
                        var chuKy = item.ToEntity<NoiTruHoSoKhacFileDinhKem>();
                        d.NoiTruHoSoKhacFileDinhKems.Add(chuKy);
                    }
                }
            }
        }
        #endregion
        #region Bảng theo dõi gây mê hồi sức
        private void ViewModelMappingBangTheoDoiGayMeHoiSuc(NoiTruHoSoKhac s, DieuTriNoiTru.HoSoKhacBangTheoDoiGayMeHoiSucViewModel d)
        {
            foreach (var item in s.NoiTruHoSoKhacFileDinhKems)
            {
                var chuKy = item.ToModel<DieuTriNoiTru.FileChuKyBangTheoDoiGayMeHoiSucViewModel>();
                d.FilesChuKy.Add(chuKy);
            }
        }

        private void AddOrUpdateBangTheoDoiGayMeHoiSuc(DieuTriNoiTru.HoSoKhacBangTheoDoiGayMeHoiSucViewModel s, NoiTruHoSoKhac d)
        {
            d.LoaiHoSoDieuTriNoiTru = LoaiHoSoDieuTriNoiTru.BangTheoDoiGayMeHoiSuc;

            foreach (var item in d.NoiTruHoSoKhacFileDinhKems)
            {
                if (s.FilesChuKy == null || !s.FilesChuKy.Any(p => p.TenGuid == item.TenGuid))
                {
                    item.WillDelete = true;
                }
            }

            if (s.FilesChuKy != null)
            {
                foreach (var item in s.FilesChuKy)
                {
                    if (!d.NoiTruHoSoKhacFileDinhKems.Any(p => p.TenGuid == item.TenGuid))
                    {
                        var chuKy = item.ToEntity<NoiTruHoSoKhacFileDinhKem>();
                        d.NoiTruHoSoKhacFileDinhKems.Add(chuKy);
                    }
                }
            }
        }
        #endregion
        #region Giấy chuyển tuyến
        private void AddOrUpdateGiayChuyenTuyen(DieuTriNoiTru.HoSoKhacGiayChuyenTuyenViewModel s, NoiTruHoSoKhac d)
        {
            d.LoaiHoSoDieuTriNoiTru = LoaiHoSoDieuTriNoiTru.GiayChuyenTuyen;
        }
        #endregion
        #region Biểu đồ chuyển dạ
        private void ViewModelMappingBieuDoChuyenDa(NoiTruHoSoKhac s, DieuTriNoiTru.HoSoKhacBieuDoChuyenDaViewModel d)
        {
            foreach (var item in s.NoiTruHoSoKhacFileDinhKems)
            {
                var chuKy = item.ToModel<DieuTriNoiTru.FileChuKyBieuDoChuyenDaViewModel>();
                d.FilesChuKy.Add(chuKy);
            }
        }

        private void AddOrUpdateBieuDoChuyenDa(DieuTriNoiTru.HoSoKhacBieuDoChuyenDaViewModel s, NoiTruHoSoKhac d)
        {
            d.LoaiHoSoDieuTriNoiTru = LoaiHoSoDieuTriNoiTru.BieuDoChuyenDa;

            foreach (var item in d.NoiTruHoSoKhacFileDinhKems)
            {
                if (s.FilesChuKy == null || !s.FilesChuKy.Any(p => p.TenGuid == item.TenGuid))
                {
                    item.WillDelete = true;
                }
            }

            if (s.FilesChuKy != null)
            {
                foreach (var item in s.FilesChuKy)
                {
                    if (!d.NoiTruHoSoKhacFileDinhKems.Any(p => p.TenGuid == item.TenGuid))
                    {
                        var chuKy = item.ToEntity<NoiTruHoSoKhacFileDinhKem>();
                        d.NoiTruHoSoKhacFileDinhKems.Add(chuKy);
                    }
                }
            }
        }
        #endregion
    }
}