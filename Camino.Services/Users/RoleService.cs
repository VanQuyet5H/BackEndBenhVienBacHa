using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Caching;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.Users;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.Users;
using Camino.Data;
using Camino.Services.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Camino.Services.Users
{
    [ScopedDependencyAttribute(ServiceType = typeof(IRoleService))]
    public class RoleService : MasterFileService<Role>, IRoleService
    {
        public static readonly string ROLES_PATTERN_KEY = "Camino.roles.";
        private readonly IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien> _dichVuKyThuatBenhVienRepository;

        private readonly IRepository<RoleFunction> _roleFunctionRepository;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly IStaticCacheManager _cacheManager;
        public RoleService(IRepository<Role> repository, IRepository<RoleFunction> roleFunctionRepository, IStaticCacheManager cacheManager
            , IUserAgentHelper userAgentHelper, IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien> dichVuKyThuatBenhVienRepository) : base(repository)
        {
            _roleFunctionRepository = roleFunctionRepository;
            _userAgentHelper = userAgentHelper;
            _dichVuKyThuatBenhVienRepository = dichVuKyThuatBenhVienRepository;
            _cacheManager = cacheManager;
        }

        public string Test()
        {
            //var sql = "SELECT * FROM [DichVuKyThuatBenhVien] inner join FREETEXTTABLE([DichVuKyThuatBenhVien], ([Ten], [Ma]),'xquang') as key_a on[BvBacHa10072020].[dbo].[DichVuKyThuatBenhVien].ID = key_a.[KEY] Order By key_a.RANK DESC";
            var lstColumnNameSearch = new List<string>();
            lstColumnNameSearch.Add("Ten");
            lstColumnNameSearch.Add("Ma");

            var result = _dichVuKyThuatBenhVienRepository.ApplyFulltext("xquang", "DichVuKyThuatBenhVien", lstColumnNameSearch);

            return JsonConvert.SerializeObject(result.ToList());
        }

        public bool VerifyAccess(long[] roleIds, Enums.DocumentType[] documentTypes, Enums.SecurityOperation securityOperation)
        {
            if (securityOperation == Enums.SecurityOperation.None || documentTypes.Contains(Enums.DocumentType.None))
                return true;
            if (roleIds == null || roleIds.Length == 0)
            {
                return false;
            }

            var roles = _cacheManager.Get(ROLES_PATTERN_KEY, () =>
            {
                return BaseRepository.TableNoTracking.Include(o => o.RoleFunctions).ToList();
            });

            //var roles = BaseRepository.TableNoTracking.Include(o => o.RoleFunctions).ToList();

            var userRoles = roles.Where(o => roleIds.Contains(o.Id));

            return userRoles.Any(o => o.RoleFunctions.Any(r => r.SecurityOperation == securityOperation && documentTypes.Contains(r.DocumentType)));
        }

        private bool CheckMenuCanView(ICollection<RoleFunction> roleFunctions, Enums.DocumentType documentType, Enums.SecurityOperation securityOperation)
        {
            return roleFunctions.Any(r =>
                r.SecurityOperation == securityOperation && r.DocumentType == documentType);
        }

        public MenuInfo GetMenuInfo(long[] roleIds)
        {
            var roles = BaseRepository.TableNoTracking.Include(o => o.RoleFunctions).ToList();
            var userRoles = roles.Where(o => roleIds.Contains(o.Id));
            var roleFunctions = userRoles.SelectMany(o => o.RoleFunctions).ToList();
            var objResult = new MenuInfo
            {
                #region Internal
                CanViewUser = CheckMenuCanView(roleFunctions, Enums.DocumentType.User, Enums.SecurityOperation.View),
                CanViewRole = CheckMenuCanView(roleFunctions, Enums.DocumentType.Role, Enums.SecurityOperation.View),
                CanViewQuanLyNhatKyHeThong = CheckMenuCanView(roleFunctions, Enums.DocumentType.QuanLyNhatKyHeThong, Enums.SecurityOperation.View),
                CanViewQuanLyCacCauHinh = CheckMenuCanView(roleFunctions, Enums.DocumentType.QuanLyCacCauHinh, Enums.SecurityOperation.View),

                CanViewQuanLyLichSuSMS = CheckMenuCanView(roleFunctions, Enums.DocumentType.QuanLyLichSuSMS, Enums.SecurityOperation.View),
                CanViewQuanLyLichSuThongBao = CheckMenuCanView(roleFunctions, Enums.DocumentType.QuanLyLichSuThongBao, Enums.SecurityOperation.View),
                CanViewQuanLyLichSuEmail = CheckMenuCanView(roleFunctions, Enums.DocumentType.QuanLyLichSuEmail, Enums.SecurityOperation.View),

                CanViewQuanLyCacMessagingTemplate = CheckMenuCanView(roleFunctions, Enums.DocumentType.QuanLyCacMessagingTemplate, Enums.SecurityOperation.View),
                CanViewQuanLyNoiDungMauXuatRaPdf = CheckMenuCanView(roleFunctions, Enums.DocumentType.QuanLyNoiDungMauXuatRaPdf, Enums.SecurityOperation.View),

                CanViewDanhMucGiuongBenh = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucGiuongBenh, Enums.SecurityOperation.View),
                CanViewTinhTrangGiuongBenh = CheckMenuCanView(roleFunctions, Enums.DocumentType.TinhTrangGiuongBenh, Enums.SecurityOperation.View),
                #region DanhMuc
                #region NhomHeThong
                CanViewDanhMucNgheNghiep = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucNgheNghiep, Enums.SecurityOperation.View),
                CanViewDanhMucChucVu = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucChucVu, Enums.SecurityOperation.View),
                CanViewDanhMucChucDanh = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucChucDanh, Enums.SecurityOperation.View),
                CanViewDanhMucVanBangChuyenMon = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucVanBangChuyenMon, Enums.SecurityOperation.View),
                CanViewDanhMucLoaiBenhVien = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucLoaiBenhVien, Enums.SecurityOperation.View),
                CanViewDanhMucCapQuanLyBenhVien = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucCapQuanLyBenhVien, Enums.SecurityOperation.View),
                CanViewDanhMucBenhVien = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucBenhVien, Enums.SecurityOperation.View),
                CanViewDanhMucLoaiPhongBenh = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucLoaiPhongBenh, Enums.SecurityOperation.View),
                //CanViewDanhMucLyDoKhamBenh = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucLyDoKhamBenh, Enums.SecurityOperation.View),
                CanViewDanhMucQuanHeThanNhan = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucQuanHeThanNhan, Enums.SecurityOperation.View),
                CanViewDanhMucNhomDichVuBenhVien = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucNhomDichVuBenhVien, Enums.SecurityOperation.View),
                CanViewDanhMucDichVuKhamBenh = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucDichVuKhamBenh, Enums.SecurityOperation.View),
                CanViewDanhMucChuyenKhoa = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucChuyenKhoa, Enums.SecurityOperation.View),
                //CanViewDanhMucDichVuCanLamSang = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucDichVuCanLamSang, Enums.SecurityOperation.View),
                CanViewDanhMucVatTuYTe = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucVatTuYTe, Enums.SecurityOperation.View),
                CanViewDanhMucDichVuKyThuat = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucDichVuKyThuat, Enums.SecurityOperation.View),
                CanViewDanhMucPhamViHanhNghe = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucPhamViHanhNghe, Enums.SecurityOperation.View),
                CanViewDanhMucHocViHocHam = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucHocViHocHam, Enums.SecurityOperation.View),
                CanViewDanhMucNhomChucDanh = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucNhomChucDanh, Enums.SecurityOperation.View),
                CanViewDanhMucMauVaChePham = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucMauVaChePham, Enums.SecurityOperation.View),
                CanViewDanhMucDonViTinh = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucDonViTinh, Enums.SecurityOperation.View),
                CanViewDanhMucNhaSanXuat = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucNhaSanXuat, Enums.SecurityOperation.View),
                CanViewDanhMucDuocPham = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucDuocPham, Enums.SecurityOperation.View),
                CanViewDanhMucDuongDung = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucDuongDung, Enums.SecurityOperation.View),
                CanViewDanhMucThuocHoacHoatChat = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucThuocHoacHoatChat, Enums.SecurityOperation.View),
                CanViewDanhMucNhomThuoc = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucNhomThuoc, Enums.SecurityOperation.View),
                CanViewDanhMucAdrPhanUngCoHaiCuaThuoc = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucAdrPhanUngCoHaiCuaThuoc, Enums.SecurityOperation.View),
                CanViewDanhMucDichVuGiuong = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucDichVuGiuong, Enums.SecurityOperation.View),
                CanViewDanhMucDichVuGiuongTaiBenhVien = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucDichVuGiuongTaiBenhVien, Enums.SecurityOperation.View),
                CanViewDanhMucDichVuKhamBenhTaiBenhVien = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucDichVuKhamBenhTaiBenhVien, Enums.SecurityOperation.View),
                CanViewDanhMucDichVuKyThuatTaiBenhVien = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucDichVuKyThuatTaiBenhVien, Enums.SecurityOperation.View),
                CanViewDanhMucNhomDichVuKyThuat = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucNhomDichVuKyThuat, Enums.SecurityOperation.View),
                CanViewDanhMucNhaThau = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucNhaThau, Enums.SecurityOperation.View),
                CanViewDanhMucKhoDuocPham = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucKhoDuocPham, Enums.SecurityOperation.View),
                CanViewDanhMucHopDongThauDuocPham = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucHopDongThauDuocPham, Enums.SecurityOperation.View),
                CanViewDanhMucKhoDuocPhamViTri = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucKhoDuocPhamViTri, Enums.SecurityOperation.View),
                CanViewDanhMucDuocPhamBenhVien = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucDuocPhamBenhVien, Enums.SecurityOperation.View),
                CanViewDanhMucDinhMucDuocPhamTonKho = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucDinhMucDuocPhamTonKho, Enums.SecurityOperation.View),
                CanViewDanhMucNhomVatTuYTe = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucNhomVatTuYTe, Enums.SecurityOperation.View),
                CanViewDanhMucVatTuYTeTaiBenhVien = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucVatTuYTeTaiBenhVien, Enums.SecurityOperation.View),
                CanViewDanhMucChiSoXetNghiem = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucChiSoXetNghiem, Enums.SecurityOperation.View),
                CanViewDanhMucChuanDoanHinhAnh = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucChuanDoanHinhAnh, Enums.SecurityOperation.View),
                CanViewDanhMucTrieuChung = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucTrieuChung, Enums.SecurityOperation.View),
                CanViewDanhMucChuanDoan = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucChuanDoan, Enums.SecurityOperation.View),
                CanViewDanhMucNhomChanDoan = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucNhomChanDoan, Enums.SecurityOperation.View),
                CanViewDanhMucLyDoTiepNhan = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucLyDoTiepNhan, Enums.SecurityOperation.View),
                //CanViewDanhMucNguoiGioiThieu = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucNguoiGioiThieu, Enums.SecurityOperation.View),
                CanViewDanhMucNoiGioiThieu = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucNoiGioiThieu, Enums.SecurityOperation.View),
                CanViewDanhMucPhuongPhapVoCam = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucPhuongPhapVoCam, Enums.SecurityOperation.View),
                CanViewCanLamSang = CheckMenuCanView(roleFunctions, Enums.DocumentType.CanLamSang, Enums.SecurityOperation.View),
                CanViewDanToc = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanToc, Enums.SecurityOperation.View),
                CanViewLichSuCanLamSang = CheckMenuCanView(roleFunctions, Enums.DocumentType.LichSuCanLamSang, Enums.SecurityOperation.View),
                CanViewLichSuGuiBHYT = CheckMenuCanView(roleFunctions, Enums.DocumentType.LichSuGuiBHYT, Enums.SecurityOperation.View),
                CanViewQuanLyICD = CheckMenuCanView(roleFunctions, Enums.DocumentType.QuanLyICD, Enums.SecurityOperation.View),

                #endregion NhomHeThong
                #region NhomKhoaPhong
                CanViewDanhMucKhoaPhong = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucKhoaPhong, Enums.SecurityOperation.View),
                CanViewDanhMucKhoaPhongNhanVien = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucKhoaPhongNhanVien, Enums.SecurityOperation.View),
                CanViewDanhMucKhoaPhongPhongKham = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucKhoaPhongPhongKham, Enums.SecurityOperation.View),
                CanViewXacNhanBhytDaHoanThanh = CheckMenuCanView(roleFunctions, Enums.DocumentType.XacNhanBhytDaHoanThanh, Enums.SecurityOperation.View),
                #endregion
                #region LichPhanCong
                CanViewDanhMucLichPhanCongNgoaiTru = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucLichPhanCongNgoaiTru, Enums.SecurityOperation.View),
                #endregion
                #region NhomNhanVien
                CanViewDanhMucNhanVien = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucNhanVien, Enums.SecurityOperation.View),
                #endregion
                #region NhomPhongBan
                #endregion NhomPhongBan
                #endregion DanhMuc
                //CanViewYeuCauKhamBenh = CheckMenuCanView(roleFunctions, Enums.DocumentType.YeuCauKhamBenh, Enums.SecurityOperation.View),
                CanViewDanhSachChoKham = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhSachChoKham, Enums.SecurityOperation.View),
                CanViewGoiDichVuMarketing = CheckMenuCanView(roleFunctions, Enums.DocumentType.GoiDichVuMarketing, Enums.SecurityOperation.View),
                CanViewGoiDvChuongTrinhMarketing = CheckMenuCanView(roleFunctions, Enums.DocumentType.GoiDvChuongTrinhMarketing, Enums.SecurityOperation.View),
                CanViewNhapKhoDuocPham = CheckMenuCanView(roleFunctions, Enums.DocumentType.NhapKhoDuocPham, Enums.SecurityOperation.View),
                CanViewXuatKhoDuocPham = CheckMenuCanView(roleFunctions, Enums.DocumentType.XuatKhoDuocPham, Enums.SecurityOperation.View),
                CanViewDuocPhamTonKho = CheckMenuCanView(roleFunctions, Enums.DocumentType.DuocPhamTonKho, Enums.SecurityOperation.View),
                CanViewDuocPhamSapHetHan = CheckMenuCanView(roleFunctions, Enums.DocumentType.DuocPhamSapHetHan, Enums.SecurityOperation.View),
                CanViewDuocPhamDaHetHan = CheckMenuCanView(roleFunctions, Enums.DocumentType.DuocPhamDaHetHan, Enums.SecurityOperation.View),
                CanViewKhamBenh = CheckMenuCanView(roleFunctions, Enums.DocumentType.KhamBenh, Enums.SecurityOperation.View),
                CanViewDoiTuongUuDaiDichVuKyThuat = CheckMenuCanView(roleFunctions, Enums.DocumentType.DoiTuongUuDaiDichVuKyThuat, Enums.SecurityOperation.View),
                CanViewDoiTuongUuDaiDichVuKhamBenh = CheckMenuCanView(roleFunctions, Enums.DocumentType.DoiTuongUuDaiDichVuKhamBenh, Enums.SecurityOperation.View),
                CanViewXacNhanBHYT = CheckMenuCanView(roleFunctions, Enums.DocumentType.XacNhanBHYT, Enums.SecurityOperation.View),
                CanViewThuNgan = CheckMenuCanView(roleFunctions, Enums.DocumentType.ThuNgan, Enums.SecurityOperation.View),
                CanViewQuayThuoc = CheckMenuCanView(roleFunctions, Enums.DocumentType.QuayThuoc, Enums.SecurityOperation.View),
                CanViewLoiDan = CheckMenuCanView(roleFunctions, Enums.DocumentType.LoiDan, Enums.SecurityOperation.View),
                CanViewToaThuocMau = CheckMenuCanView(roleFunctions, Enums.DocumentType.ToaThuocMau, Enums.SecurityOperation.View),
                CanViewBaoCaoChiTietDoanhThuTheoBacSi = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoChiTietDoanhThuTheoBacSi, Enums.SecurityOperation.View),
                CanViewBaoCaoChiTietDoanhThuTheoKhoaPhong = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoChiTietDoanhThuTheoKhoaPhong, Enums.SecurityOperation.View),
                CanViewBaoCaoThuVienPhiBenhNhan = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoThuVienPhiBenhNhan, Enums.SecurityOperation.View),
                CanViewDanhMucQuocGia = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucQuocGia, Enums.SecurityOperation.View),

                //CanViewBaoCaoNoiTruNgoaiTru = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoNoiTruNgoaiTru, Enums.SecurityOperation.View),
                CanViewBaoCaoTongHopDoanhThuTheoBacSi = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoTongHopDoanhThuTheoBacSi, Enums.SecurityOperation.View),
                CanViewBaoCaoTongHopDoanhThuTheoKhoaPhong = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoTongHopDoanhThuTheoKhoaPhong, Enums.SecurityOperation.View),
                //CanViewTheoDoiTinhHinhThanhToanVienPhi = CheckMenuCanView(roleFunctions, Enums.DocumentType.TheoDoiTinhHinhThanhToanVienPhi, Enums.SecurityOperation.View),
                CanViewBaoCaoDanhSachThuTienVienPhi = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoDanhSachThuTienVienPhi, Enums.SecurityOperation.View),
                CanViewBenhNhan = CheckMenuCanView(roleFunctions, Enums.DocumentType.BenhNhan, Enums.SecurityOperation.View),
                CanViewGuiBaoHiemYTe = CheckMenuCanView(roleFunctions, Enums.DocumentType.GuiBaoHiemYTe, Enums.SecurityOperation.View),
                CanViewLichSuTiepNhan = CheckMenuCanView(roleFunctions, Enums.DocumentType.LichSuTiepNhan, Enums.SecurityOperation.View),
                CanViewLichSuKhamBenh = CheckMenuCanView(roleFunctions, Enums.DocumentType.LichSuKhamBenh, Enums.SecurityOperation.View),
                CanViewLichSuXacNhanBHYT = CheckMenuCanView(roleFunctions, Enums.DocumentType.LichSuXacNhanBHYT, Enums.SecurityOperation.View),
                CanViewLichSuThuNgan = CheckMenuCanView(roleFunctions, Enums.DocumentType.LichSuThuNgan, Enums.SecurityOperation.View),
                CanViewLichSuQuayThuoc = CheckMenuCanView(roleFunctions, Enums.DocumentType.LichSuQuayThuoc, Enums.SecurityOperation.View),
                CanViewYeuCauTiepNhan = CheckMenuCanView(roleFunctions, Enums.DocumentType.YeuCauTiepNhan, Enums.SecurityOperation.View),
                //CanViewXacNhanThuNganDaHoanThanh = CheckMenuCanView(roleFunctions, Enums.DocumentType.XacNhanThuNganDaHoanThanh, Enums.SecurityOperation.View),
                CanViewPhauThuatThuThuatTheoNgay = CheckMenuCanView(roleFunctions, Enums.DocumentType.PhauThuatThuThuatTheoNgay, Enums.SecurityOperation.View),
                CanViewLichSuPhauThuatThuThuat = CheckMenuCanView(roleFunctions, Enums.DocumentType.LichSuPhauThuatThuThuat, Enums.SecurityOperation.View),
                //CanViewDuyetNhapKho = CheckMenuCanView(roleFunctions, Enums.DocumentType.DuyetNhapKho, Enums.SecurityOperation.View),
                CanViewDuyetNhapKhoVatTu = CheckMenuCanView(roleFunctions, Enums.DocumentType.DuyetNhapKhoVatTu, Enums.SecurityOperation.View),
                CanViewNhapKhoVatTu = CheckMenuCanView(roleFunctions, Enums.DocumentType.NhapKhoVatTu, Enums.SecurityOperation.View),
                //CanViewDanhSachLichSuBanThuoc = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhSachLichSuBanThuoc, Enums.SecurityOperation.View),
                CanViewLinhTrucTiepDuocPham = CheckMenuCanView(roleFunctions, Enums.DocumentType.DuyetYeuCauLinhDuocPham, Enums.SecurityOperation.View),
                CanViewDanhMucHopDongThauVatTu = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucHopDongThauVatTu, Enums.SecurityOperation.View),
                CanViewDanhMucDinhMucVatTuTonKho = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucDinhMucVatTuTonKho, Enums.SecurityOperation.View),
                CanViewDanhMucDuocPhamBenhVienPhanNhom = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucDuocPhamBenhVienPhanNhom, Enums.SecurityOperation.View),
                CanViewDanhMucCongTyBhtn = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucCongTyBhtn, Enums.SecurityOperation.View),
                CanViewDanhMucCongTyUuDai = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucCongTyUuDai, Enums.SecurityOperation.View),
                //CanViewDanhSachLinhVatTu = CheckMenuCanView(roleFunctions, Enums.DocumentType.YeuCauLinhVatTu, Enums.SecurityOperation.View),
                //CanViewDanhSachLinhDuyetVatTu = CheckMenuCanView(roleFunctions, Enums.DocumentType.YeuCauLinhDuyetVatTu, Enums.SecurityOperation.View),
                //CanViewDanhSachLinhDuocPham = CheckMenuCanView(roleFunctions, Enums.DocumentType.YeuCauLinhDuocPham, Enums.SecurityOperation.View),
                //CanViewDanhSachDuyetLinhDuocPham = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhSachDuyetLinhDuocPham, Enums.SecurityOperation.View),
                //CanViewTaoLinhDuocPhamTrucTiep = CheckMenuCanView(roleFunctions, Enums.DocumentType.TaoLinhDuocPhamTrucTiep, Enums.SecurityOperation.View),
                //                CanViewDanhSachCanLinhTrucTiepDuocPham = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhSachCanLinhTrucTiepDuocPham, Enums.SecurityOperation.View),
                //                CanViewDanhSachCanLinhTrucTiepVatTu = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhSachCanLinhTrucTiepVatTu, Enums.SecurityOperation.View),
                //                CanViewDanhSachCanLinhBuDuocPham = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhSachCanLinhBuDuocPham, Enums.SecurityOperation.View),
                //                CanViewDanhSachCanLinhBuVatTu = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhSachCanLinhBuVatTu, Enums.SecurityOperation.View),
                CanViewDuyetNhapKhoDuocPham = CheckMenuCanView(roleFunctions, Enums.DocumentType.DuyetNhapKhoDuocPham, Enums.SecurityOperation.View),
                CanViewDanhSachYeuCauLinhDuocPham = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhSachYeuCauLinhDuocPham, Enums.SecurityOperation.View),
                CanViewTaoYeuCauLinhThuongDuocPham = CheckMenuCanView(roleFunctions, Enums.DocumentType.TaoYeuCauLinhThuongDuocPham, Enums.SecurityOperation.View),
                CanViewTaoYeuCauLinhBuDuocPham = CheckMenuCanView(roleFunctions, Enums.DocumentType.TaoYeuCauLinhBuDuocPham, Enums.SecurityOperation.View),
                CanViewTaoYeuCauLinhTrucTiepDuocPham = CheckMenuCanView(roleFunctions, Enums.DocumentType.TaoYeuCauLinhTrucTiepDuocPham, Enums.SecurityOperation.View),
                CanViewDuyetYeuCauLinhDuocPham = CheckMenuCanView(roleFunctions, Enums.DocumentType.DuyetYeuCauLinhDuocPham, Enums.SecurityOperation.View),
                CanViewDanhSachYeuCauLinhVatTu = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhSachYeuCauLinhVatTu, Enums.SecurityOperation.View),
                CanViewTaoYeuCauLinhThuongVatTu = CheckMenuCanView(roleFunctions, Enums.DocumentType.TaoYeuCauLinhThuongVatTu, Enums.SecurityOperation.View),
                CanViewTaoYeuCauLinhBuVatTu = CheckMenuCanView(roleFunctions, Enums.DocumentType.TaoYeuCauLinhBuVatTu, Enums.SecurityOperation.View),
                CanViewTaoYeuCauLinhTrucTiepVatTu = CheckMenuCanView(roleFunctions, Enums.DocumentType.TaoYeuCauLinhTrucTiepVatTu, Enums.SecurityOperation.View),
                CanViewDuyetYeuCauLinhVatTu = CheckMenuCanView(roleFunctions, Enums.DocumentType.DuyetYeuCauLinhVatTu, Enums.SecurityOperation.View),

                CanViewXuatKhoVatTu = CheckMenuCanView(roleFunctions, Enums.DocumentType.XuatKhoVatTu, Enums.SecurityOperation.View),
                CanViewYeuCauHoanTraDuocPham = CheckMenuCanView(roleFunctions, Enums.DocumentType.YeuCauHoanTraDuocPham, Enums.SecurityOperation.View),
                CanViewYeuCauHoanTraVatTu = CheckMenuCanView(roleFunctions, Enums.DocumentType.YeuCauHoanTraVatTu, Enums.SecurityOperation.View),
                //CanViewDanhSachDuyetYeuCauHoanTraDuocPham = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhSachDuyetYeuCauHoanTraDuocPham, Enums.SecurityOperation.View),
                //CanViewDanhSachDuyetYeuCauHoanTraVatTu = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhSachDuyetYeuCauHoanTraVatTu, Enums.SecurityOperation.View),
                CanViewDuyetYeuCauHoanTraDuocPham = CheckMenuCanView(roleFunctions, Enums.DocumentType.DuyetYeuCauHoanTraDuocPham, Enums.SecurityOperation.View),
                CanViewDuyetYeuCauHoanTraVatTu = CheckMenuCanView(roleFunctions, Enums.DocumentType.DuyetYeuCauHoanTraVatTu, Enums.SecurityOperation.View),
                CanViewKhamBenhDangKham = CheckMenuCanView(roleFunctions, Enums.DocumentType.KhamBenhDangKham, Enums.SecurityOperation.View),

                CanViewXuatKhoDuocPhamKhac = CheckMenuCanView(roleFunctions, Enums.DocumentType.XuatKhoDuocPhamKhac, Enums.SecurityOperation.View),
                CanViewXuatKhoVatTuKhac = CheckMenuCanView(roleFunctions, Enums.DocumentType.XuatKhoVatTuKhac, Enums.SecurityOperation.View),

                CanViewVatTuTonKho = CheckMenuCanView(roleFunctions, Enums.DocumentType.VatTuTonKho, Enums.SecurityOperation.View),
                CanViewVatTuSapHetHan = CheckMenuCanView(roleFunctions, Enums.DocumentType.VatTuSapHetHan, Enums.SecurityOperation.View),
                CanViewVatTuDaHetHan = CheckMenuCanView(roleFunctions, Enums.DocumentType.VatTuDaHetHan, Enums.SecurityOperation.View),
                CanViewNhapKhoMarketing = CheckMenuCanView(roleFunctions, Enums.DocumentType.NhapKhoMarketing, Enums.SecurityOperation.View),
                CanViewXuatKhoMarketing = CheckMenuCanView(roleFunctions, Enums.DocumentType.XuatKhoMarketing, Enums.SecurityOperation.View),
                CanViewQuaTangMarketing = CheckMenuCanView(roleFunctions, Enums.DocumentType.QuaTangMarketing, Enums.SecurityOperation.View),
                CanViewGoiDichVuNhomThuongDung = CheckMenuCanView(roleFunctions, Enums.DocumentType.GoiDichVuNhomThuongDung, Enums.SecurityOperation.View),
                CanViewVoucherMarketing = CheckMenuCanView(roleFunctions, Enums.DocumentType.VoucherMarketing, Enums.SecurityOperation.View),

                CanViewDanhSachMarketing = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhSachMarketing, Enums.SecurityOperation.View),
                CanViewCongNoBhtn = CheckMenuCanView(roleFunctions, Enums.DocumentType.CongNoBhtn, Enums.SecurityOperation.View),
                CanViewBaoCaoCongNoCongTyBhtn = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoCongNoCongTyBhtn, Enums.SecurityOperation.View),

                CanViewDanhSachYeuCauDuTruMuaDuocPham = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhSachYeuCauDuTruMuaDuocPham, Enums.SecurityOperation.View),
                CanViewDanhSachTongHopDuTruMuaDuocPhamTaiKhoa = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoa, Enums.SecurityOperation.View),
                CanViewDanhSachTongHopDuTruMuaDuocPhamTaiKhoaDuoc = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoaDuoc, Enums.SecurityOperation.View),
                CanViewDanhSachTongHopDuTruMuaDuocPhamTaiGiamDoc = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiGiamDoc, Enums.SecurityOperation.View),
                CanViewKyDuTru = CheckMenuCanView(roleFunctions, Enums.DocumentType.KyDuTru, Enums.SecurityOperation.View),

                CanViewDanhSachYeuCauDuTruMuaVatTu = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhSachYeuCauDuTruMuaVatTu, Enums.SecurityOperation.View),
                CanViewDanhSachTongHopDuTruMuaVatTuTaiKhoa = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoa, Enums.SecurityOperation.View),
                CanViewDanhSachTongHopDuTruMuaVatTuTaiKhoaDuoc = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoaDuoc, Enums.SecurityOperation.View),
                CanViewDanhSachTongHopDuTruMuaVatTuTaiGiamDoc = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiGiamDoc, Enums.SecurityOperation.View),
                CanViewDuyetKetQuaXetNghiem = CheckMenuCanView(roleFunctions, Enums.DocumentType.DuyetKetQuaXetNghiem, Enums.SecurityOperation.View),
                CanViewDuyetYeuCauChayLaiXetNghiem = CheckMenuCanView(roleFunctions, Enums.DocumentType.DuyetYeuCauChayLaiXetNghiem, Enums.SecurityOperation.View),

                CanViewKetQuaXetNghiem = CheckMenuCanView(roleFunctions, Enums.DocumentType.KetQuaXetNghiem, Enums.SecurityOperation.View),
                CanViewChiSoXetNghiem = CheckMenuCanView(roleFunctions, Enums.DocumentType.ChiSoXetNghiem, Enums.SecurityOperation.View),
                CanViewThietBiXetNghiem = CheckMenuCanView(roleFunctions, Enums.DocumentType.ThietBiXetNghiem, Enums.SecurityOperation.View),
                CanViewGoiMauXetNghiem = CheckMenuCanView(roleFunctions, Enums.DocumentType.GoiMauXetNghiem, Enums.SecurityOperation.View),
                CanViewNhanMauXetNghiem = CheckMenuCanView(roleFunctions, Enums.DocumentType.NhanMauXetNghiem, Enums.SecurityOperation.View),
                CanViewLayMauXetNghiem = CheckMenuCanView(roleFunctions, Enums.DocumentType.LayMauXetNghiem, Enums.SecurityOperation.View),
                CanViewDanhSachDieuTriNoiTru = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhSachDieuTriNoiTru, Enums.SecurityOperation.View),
                CanViewTiepNhanNoiTru = CheckMenuCanView(roleFunctions, Enums.DocumentType.TiepNhanNoiTru, Enums.SecurityOperation.View),
                CanViewTongHopYLenh = CheckMenuCanView(roleFunctions, Enums.DocumentType.TongHopYLenh, Enums.SecurityOperation.View),

                //                CanViewKetQuaSieuAm = CheckMenuCanView(roleFunctions, Enums.DocumentType.KetQuaSieuAm, Enums.SecurityOperation.View),
                //                CanViewKetQuaXQuang = CheckMenuCanView(roleFunctions, Enums.DocumentType.KetQuaXQuang, Enums.SecurityOperation.View),
                //                CanViewKetQuaNoiSoi = CheckMenuCanView(roleFunctions, Enums.DocumentType.KetQuaNoiSoi, Enums.SecurityOperation.View),
                //                CanViewKetQuaDienTim = CheckMenuCanView(roleFunctions, Enums.DocumentType.KetQuaDienTim, Enums.SecurityOperation.View),
                CanViewNhapKhoMau = CheckMenuCanView(roleFunctions, Enums.DocumentType.NhapKhoMau, Enums.SecurityOperation.View),
                CanViewDuyetNhapKhoMau = CheckMenuCanView(roleFunctions, Enums.DocumentType.DuyetNhapKhoMau, Enums.SecurityOperation.View),
                CanViewLuuTruHoSo = CheckMenuCanView(roleFunctions, Enums.DocumentType.LuuTruHoSo, Enums.SecurityOperation.View),
                CanViewTraThuocTuBenhNhan = CheckMenuCanView(roleFunctions, Enums.DocumentType.TraThuocTuBenhNhan, Enums.SecurityOperation.View),
                CanViewDuyetTraThuocTuBenhNhan = CheckMenuCanView(roleFunctions, Enums.DocumentType.DuyetTraThuocTuBenhNhan, Enums.SecurityOperation.View),
                CanViewXacNhanBhytNoiTru = CheckMenuCanView(roleFunctions, Enums.DocumentType.XacNhanBhytNoiTru, Enums.SecurityOperation.View),
                CanViewDuyetTraVatTuTuBenhNhan = CheckMenuCanView(roleFunctions, Enums.DocumentType.DuyetTraVatTuTuBenhNhan, Enums.SecurityOperation.View),
                CanViewTraVatTuTuBenhNhan = CheckMenuCanView(roleFunctions, Enums.DocumentType.TraVatTuTuBenhNhan, Enums.SecurityOperation.View),
                CanViewCongNoBenhNhan = CheckMenuCanView(roleFunctions, Enums.DocumentType.CongNoBenhNhan, Enums.SecurityOperation.View),
                CanViewBaoCaoLuuTruHoSoBenhAn = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoLuuTruHoSoBenhAn, Enums.SecurityOperation.View),
                CanViewBaoCaoBSDanhSachKhamNgoaiTru = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoBSDanhSachKhamNgoaiTru, Enums.SecurityOperation.View),
                CanViewBaoCaoXuatNhapTon = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoXuatNhapTon, Enums.SecurityOperation.View),
                CanViewBaoCaoTiepNhanBenhNhanKham = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoTiepNhanBenhNhanKham, Enums.SecurityOperation.View),

                CanViewBaoCaoKetQuaKhamChuaBenh = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoKetQuaKhamChuaBenh, Enums.SecurityOperation.View),
                CanViewBaoCaoVienPhiThuTien = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoVienPhiThuTien, Enums.SecurityOperation.View),
                CanViewBaoCaoThongKeDonThuoc = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoThongKeDonThuoc, Enums.SecurityOperation.View),
                CanViewKhamDoanLichSuTiepNhanKhamSucKhoe = CheckMenuCanView(roleFunctions, Enums.DocumentType.KhamDoanLichSuTiepNhanKhamSucKhoe, Enums.SecurityOperation.View),
                CanViewKhamDoanCongTy = CheckMenuCanView(roleFunctions, Enums.DocumentType.KhamDoanCongTy, Enums.SecurityOperation.View),
                CanViewKhamDoanChiSoSinhTon = CheckMenuCanView(roleFunctions, Enums.DocumentType.KhamDoanChiSoSinhTon, Enums.SecurityOperation.View),
                CanViewKhamDoanHopDongKham = CheckMenuCanView(roleFunctions, Enums.DocumentType.KhamDoanHopDongKham, Enums.SecurityOperation.View),
                CanViewKhamDoanKetLuanCanLamSangKhamSucKhoeDoan = CheckMenuCanView(roleFunctions, Enums.DocumentType.KhamDoanKetLuanCanLamSangKhamSucKhoeDoan, Enums.SecurityOperation.View),
                CanViewKhamDoanTiepNhan = CheckMenuCanView(roleFunctions, Enums.DocumentType.KhamDoanTiepNhan, Enums.SecurityOperation.View),
                CanViewKhamDoanGoiKhamSucKhoe = CheckMenuCanView(roleFunctions, Enums.DocumentType.KhamDoanGoiKhamSucKhoe, Enums.SecurityOperation.View),

                CanViewKhamDoanYeuCauNhanSuKhamSucKhoe = CheckMenuCanView(roleFunctions, Enums.DocumentType.KhamDoanYeuCauNhanSuKhamSucKhoe, Enums.SecurityOperation.View),

                CanViewKhamDoanKetLuanKhamSucKhoeDoan = CheckMenuCanView(roleFunctions, Enums.DocumentType.KhamDoanKetLuanKhamSucKhoeDoan, Enums.SecurityOperation.View),
                CanViewKhamDoanDuyetYeuCauNhanSuKhamSucKhoeKhth = CheckMenuCanView(roleFunctions, Enums.DocumentType.KhamDoanDuyetYeuCauNhanSuKhamSucKhoeKhth, Enums.SecurityOperation.View),
                CanViewKhamDoanDuyetYeuCauNhanSuKhamSucKhoePhongNhanSu = CheckMenuCanView(roleFunctions, Enums.DocumentType.KhamDoanDuyetYeuCauNhanSuKhamSucKhoePhongNhanSu, Enums.SecurityOperation.View),
                CanViewDanhMucCheDoAn = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucCheDoAn, Enums.SecurityOperation.View),
                CanViewBaoCaoDoanhThuNhaThuoc = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoDoanhThuNhaThuoc, Enums.SecurityOperation.View),
                CanViewBaoCaoHoatDongKhoaKhamBenh = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoHoatDongKhoaKhamBenh, Enums.SecurityOperation.View),
                CanViewBaoCaoThucHienCls = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoThucHienCls, Enums.SecurityOperation.View),
                CanViewDanhSachBenhNhanPhauThuat = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhSachBenhNhanPhauThuat, Enums.SecurityOperation.View),
                CanViewKhamDoanDuyetYeuCauNhanSuKhamSucKhoeGiamDoc = CheckMenuCanView(roleFunctions, Enums.DocumentType.KhamDoanDuyetYeuCauNhanSuKhamSucKhoeGiamDoc, Enums.SecurityOperation.View),
                CanViewKhamDoanKhamBenh = CheckMenuCanView(roleFunctions, Enums.DocumentType.KhamDoanKhamBenh, Enums.SecurityOperation.View),
                CanViewKhamDoanKhamBenhTatCaPhong = CheckMenuCanView(roleFunctions, Enums.DocumentType.KhamDoanKhamBenhTatCaPhong, Enums.SecurityOperation.View),
                CanViewCapNhatDuocPhamTonKho = CheckMenuCanView(roleFunctions, Enums.DocumentType.CapNhatDuocPhamTonKho, Enums.SecurityOperation.View),
                CanViewCapNhatVatTuTonKho = CheckMenuCanView(roleFunctions, Enums.DocumentType.CapNhatVatTuTonKho, Enums.SecurityOperation.View),
                CanViewBaoCaoBangKeChiTietTTCN = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoBangKeChiTietTTCN, Enums.SecurityOperation.View),
                CanViewBaoCaoTheKho = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoTheKho, Enums.SecurityOperation.View),
                CanViewBaoCaoTonKho = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoTonKho, Enums.SecurityOperation.View),
                CanViewBaoCaoDoanhThuTheoNhomDichVu = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoDoanhThuTheoNhomDichVu, Enums.SecurityOperation.View),
                CanViewBaoCaoLuuKetQuaXetNghiemHangNgay = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoLuuKetQuaXetNghiemHangNgay, Enums.SecurityOperation.View),
                CanViewBaoCaoXNTVatTu = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoXNTVatTu, Enums.SecurityOperation.View),
                CanViewBaoCaoSoXetNghiemSangLocHiv = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoSoXetNghiemSangLocHiv, Enums.SecurityOperation.View),
                CanViewBaoCaoTongHopSoLuongXetNghiem = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoTongHopSoLuongXetNghiemTheo, Enums.SecurityOperation.View),
                CanViewBaoCaoBenhNhanLamXetNghiem = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoBenhNhanLamXetNghiem, Enums.SecurityOperation.View),
                CanViewDanhSachDieuChuyenNoiBoDuocPham = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhSachDieuChuyenNoiBoDuocPham, Enums.SecurityOperation.View),
                CanViewDanhSachDuyetDieuChuyenNoiBoDuocPham = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhSachDuyetDieuChuyenNoiBoDuocPham, Enums.SecurityOperation.View),
                CanViewBaoCaoBenhNhanKhamNgoaiTru = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoBenhNhanKhamNgoaiTru, Enums.SecurityOperation.View),
                CanViewBaoCaoSoLuongThuThuat = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoSoLuongThuThuat, Enums.SecurityOperation.View),
                CanViewBaoCaoSoPhucTrinhPhauThuatThuThuat = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoSoPhucTrinhPhauThuatThuThuat, Enums.SecurityOperation.View),
                CanViewBaoCaoHoatDongClsTheoKhoa = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoHoatDongClsTheoKhoa, Enums.SecurityOperation.View),
                CanViewBaoCaoSoThongKeCls = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoSoThongKeCls, Enums.SecurityOperation.View),
                CanViewTaoBenhAnSoSinh = CheckMenuCanView(roleFunctions, Enums.DocumentType.TaoBenhAnSoSinh, Enums.SecurityOperation.View),
                CanViewTuDienDichVuKyThuat = CheckMenuCanView(roleFunctions, Enums.DocumentType.TuDienDichVuKyThuat, Enums.SecurityOperation.View),
                CanViewTiemChungKhamSangLoc = CheckMenuCanView(roleFunctions, Enums.DocumentType.TiemChungKhamSangLoc, Enums.SecurityOperation.View),
                CanViewTiemChungThucHienTiem = CheckMenuCanView(roleFunctions, Enums.DocumentType.TiemChungThucHienTiem, Enums.SecurityOperation.View),
                CanViewTiemChungLichSuTiem = CheckMenuCanView(roleFunctions, Enums.DocumentType.TiemChungLichSuTiem, Enums.SecurityOperation.View),
                CanViewBaoCaoDichVuTrongGoiKhamDoan = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoDichVuTrongGoiKhamDoan, Enums.SecurityOperation.View),
                CanViewBaoCaoDichVuNgoaiGoiKhamDoan = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoDichVuNgoaiGoiKhamDoan, Enums.SecurityOperation.View),
                CanViewBaoCaoHieuQuaCongViec = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoHieuQuaCongViec, Enums.SecurityOperation.View),
                CanViewBaoCaoTiepNhanBenhPham = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoTiepNhanBenhPham, Enums.SecurityOperation.View),
                CanViewBaoCaoTonKhoXN = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoTonKhoXN, Enums.SecurityOperation.View),
                CanViewBaoCaoTonKhoKT = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoTonKhoKT, Enums.SecurityOperation.View),
                CanViewBaoCaoTinhHinhTraNCC = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoTinhHinhTraNCC, Enums.SecurityOperation.View),
                CanViewBaoCaoTinhHinhNhapNCCChiTiet = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoTinhHinhNhapNCCChiTiet, Enums.SecurityOperation.View),

                CanViewBaoCaoDuocChiTietXuatNoiBo = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoDuocChiTietXuatNoiBo, Enums.SecurityOperation.View),
                CanViewBaoCaoChiTietMienPhiTronVien = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoChiTietMienPhiTronVien, Enums.SecurityOperation.View),
                CanViewBaoCaoBienBanKiemKeKT = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoBienBanKiemKeKT, Enums.SecurityOperation.View),
                CanViewBaoCaoBangKePhieuXuatKho = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoBangKePhieuXuatKho, Enums.SecurityOperation.View),
                CanViewBaoCaoTinhHinhNhapTuNhaCungCap = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoTinhHinhNhapTuNhaCungCap, Enums.SecurityOperation.View),
                CanViewBaoCaoTongHopDoanhThuThaiSanDaSinh = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoTongHopDoanhThuThaiSanDaSinh, Enums.SecurityOperation.View),
                CanViewBaoCaoTongHopDangKyGoiDichVu = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoTongHopDangKyGoiDichVu, Enums.SecurityOperation.View),
                CanViewBaoCaoSoChiTietVatTuHangHoa = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoSoChiTietVatTuHangHoa, Enums.SecurityOperation.View),
                CanViewBaoCaoDoanhThuKhamDoanTheoNhomDV = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoDoanhThuKhamDoanTheoNhomDV, Enums.SecurityOperation.View),
                CanViewBaoCaoBangKeXuatThuocTheoBenhNhan = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoBangKeXuatThuocTheoBenhNhan, Enums.SecurityOperation.View),
                CanViewBaoCaoHoatDongCls = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoHoatDongCls, Enums.SecurityOperation.View),
                CanViewBaoCaoThuocSapHetHanDung = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoThuocSapHetHanDung, Enums.SecurityOperation.View),
                CanViewBaoCaoDuocTinhHinhXuatNoiBo = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoDuocTinhHinhXuatNoiBo, Enums.SecurityOperation.View),
                CanViewBaoCaoKTNhapXuatTonChiTiet = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoKTNhapXuatTonChiTiet, Enums.SecurityOperation.View),
                CanViewBaoCaoSoLieuThoiGianSuDungDV = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoSoLieuThoiGianSuDungDV, Enums.SecurityOperation.View),
                CanViewBaoCaoDoanhThuKhamDoanTheoKP = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoDoanhThuKhamDoanTheoKP, Enums.SecurityOperation.View),
                CanViewBaoCaoChiTietHoaHongCuaNguoiGT = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoChiTietHoaHongCuaNguoiGT, Enums.SecurityOperation.View),
                CanViewBaoCaoCamKetSuDungThuocNgoaiBHYT = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoCamKetSuDungThuocNgoaiBHYT, Enums.SecurityOperation.View),
                CanViewBaoCaoBangKeGiaoHoaDonSangPKT = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoBangKeGiaoHoaDonSangPKT, Enums.SecurityOperation.View),
                CanViewBaoCaoHoatDongKhamDoan = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoHoatDongKhamDoan, Enums.SecurityOperation.View),
                CanViewBaoCaoTongHopKetQuaKSK = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoTongHopKetQuaKSK, Enums.SecurityOperation.View),
                CanViewBaoCaoHoatDongNoiTru = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoHoatDongNoiTru, Enums.SecurityOperation.View),
                CanViewBaoCaoBienBanKiemKeDuocVT = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoBienBanKiemKeDuocVT, Enums.SecurityOperation.View),
                CanViewBaoCaoThongKeKSK = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoThongKeKSK, Enums.SecurityOperation.View),


                #region lịch sử khám chữa bệnh
                CanViewLichSuKhamChuaBenh = CheckMenuCanView(roleFunctions, Enums.DocumentType.LichSuKhamChuaBenh, Enums.SecurityOperation.View),
                #endregion

                CanViewBaoCaoNguoiBenhDenKham = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoNguoiBenhDenKham, Enums.SecurityOperation.View),
                CanViewBaoCaoNguoiBenhDenLamDVKT = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoNguoiBenhDenLamDVKT, Enums.SecurityOperation.View),
                CanViewBaoCaoTraCuuDuLieu = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoTraCuuDuLieu, Enums.SecurityOperation.View),
                CanViewBaoCaoTongHopCongNoChuaThanhToan = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoTongHopCongNoChuaThanhToan, Enums.SecurityOperation.View),
                CanViewBaoCaoKSKChuyenKhoa = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoKSKChuyenKhoa, Enums.SecurityOperation.View),
                CanViewBaoCaoHoatDongKhamBenhTheoDichVu = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoHoatDongKhamBenhTheoDichVu, Enums.SecurityOperation.View),
                CanViewBaoCaoHoatDongKhamBenhTheoKhoaPhong = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoHoatDongKhamBenhTheoKhoaPhong, Enums.SecurityOperation.View),
                CanViewBaoCaoThuVienPhiChuaHoan = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoThuVienPhiChuaHoan, Enums.SecurityOperation.View),
                CanViewBaoCaoBangKeChiTietTheoNguoiBenh = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoBangKeChiTietTheoNguoiBenh, Enums.SecurityOperation.View),
                CanViewBangKeThuocVatTuPhauThuat = CheckMenuCanView(roleFunctions, Enums.DocumentType.BangKeThuocVatTuPhauThuat, Enums.SecurityOperation.View),
                CanViewBaoCaoTonKhoVatTuYTe = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoTonKhoVatTuYTe, Enums.SecurityOperation.View),
                CanViewBaoCaoTheKhoVatTuYTe = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoTheKhoVatTuYTe, Enums.SecurityOperation.View),
                CanViewKhamDoanThongKeSoNguoiKhamSucKhoeLSCLS = CheckMenuCanView(roleFunctions, Enums.DocumentType.KhamDoanThongKeSoNguoiKhamSucKhoeLSCLS, Enums.SecurityOperation.View),
                CanViewBaoCaoDichVuPhatSinhNgoaiGoiCuaKeToan = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoDichVuPhatSinhNgoaiGoiCuaKeToan, Enums.SecurityOperation.View),
                CanViewBangThongKeTiepNhanNoiTruVaNgoaiTru = CheckMenuCanView(roleFunctions, Enums.DocumentType.BangThongKeTiepNhanNoiTruVaNgoaiTru, Enums.SecurityOperation.View),
                CanViewBaoCaoDichVuTiaPlasMaHoTroDieuTriVetThuong = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoDichVuTiaPlasMaHoTroDieuTriVetThuong, Enums.SecurityOperation.View),
                CanViewBaoCaoDoanhThuKhamDoanTheoNhomDichVu = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoDoanhThuKhamDoanTheoNhomDichVu, Enums.SecurityOperation.View),
                CanViewBaoCaoBenhNhanRaVienNoiTru = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoBenhNhanRaVienNoiTru, Enums.SecurityOperation.View),

                CanViewThongKeThuocTheoBacSi = CheckMenuCanView(roleFunctions, Enums.DocumentType.ThongKeThuocTheoBacSi, Enums.SecurityOperation.View),
                CanViewThongKeBSKeDonTheoThuoc = CheckMenuCanView(roleFunctions, Enums.DocumentType.ThongKeBSKeDonTheoThuoc, Enums.SecurityOperation.View),
                CanViewThongKeCacDichVuChuaLayLenBienLaiThuTien = CheckMenuCanView(roleFunctions, Enums.DocumentType.ThongKeCacDichVuChuaLayLenBienLaiThuTien, Enums.SecurityOperation.View),
                CanViewDanhSachBARaVienChuaXacNhanHoanTatChiPhi = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhSachBARaVienChuaXacNhanHoanTatChiPhi, Enums.SecurityOperation.View),
                CanViewDanhMucDichVuKyThuatBenhVien = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucDichVuKyThuatBenhVien, Enums.SecurityOperation.View),
                CanViewDanhMucDichVuKhamBenhBenhVien = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucDichVuKhamBenhBenhVien, Enums.SecurityOperation.View),
                CanViewDanhSachThuVienPhiNoiTruVaNgoaiTruChuaHoan = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhSachThuVienPhiNoiTruVaNgoaiTruChuaHoan, Enums.SecurityOperation.View),
                CanViewDanhSachXuatChungTuExcel = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhSachXuatChungTuExcel, Enums.SecurityOperation.View),
                CanViewDanhSachLichSuHuyBanThuoc = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhSachLichSuHuyBanThuoc, Enums.SecurityOperation.View),
                CanViewBaoCaoKetQuaKhamChuaBenhKT = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoKetQuaKhamChuaBenhKT, Enums.SecurityOperation.View),
                CanViewBaoCaoHoatDongNoiTruChiTiet = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoHoatDongNoiTruChiTiet, Enums.SecurityOperation.View),
                CanViewBaoCaoTinhHinhBenhTatTuVong = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoTinhHinhBenhTatTuVong, Enums.SecurityOperation.View),
                CanViewDanhMucBenhVaNhomBenh = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucBenhVaNhomBenh, Enums.SecurityOperation.View),
                //CanViewDanhSachGayBenhAn = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhSachGayBenhAn, Enums.SecurityOperation.View),


                #region Bệnh án điện tử
                CanViewDanhMucGayBenhAn = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucGayBenhAn, Enums.SecurityOperation.View),
                CanViewBenhAnDienTu = CheckMenuCanView(roleFunctions, Enums.DocumentType.BenhAnDienTu, Enums.SecurityOperation.View),

                #endregion

                CanViewDSXNNgoaiTruVaNoiTruBHYT = CheckMenuCanView(roleFunctions, Enums.DocumentType.DSXNNgoaiTruVaNoiTruBHYT, Enums.SecurityOperation.View),
                CanViewBaoCaoNhapXuatTon = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoNhapXuatTon, Enums.SecurityOperation.View),

                CanViewDanhSachDonThuocChoCapThuocBHYT = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhSachDonThuocChoCapThuocBHYT, Enums.SecurityOperation.View),
                CanViewLichSuXuatThuocCapThuocBHYT = CheckMenuCanView(roleFunctions, Enums.DocumentType.LichSuXuatThuocCapThuocBHYT, Enums.SecurityOperation.View),
                CanViewMoLaiBenhAn = CheckMenuCanView(roleFunctions, Enums.DocumentType.MoLaiBenhAn, Enums.SecurityOperation.View),

                CanViewNhapVatTuThuocNhomKSNK = CheckMenuCanView(roleFunctions, Enums.DocumentType.NhapVatTuThuocNhomKSNK, Enums.SecurityOperation.View),
                CanViewXuatKhoVatTuThuocNhomKSNK = CheckMenuCanView(roleFunctions, Enums.DocumentType.XuatKhoVatTuThuocNhomKSNK, Enums.SecurityOperation.View),
                CanViewXuatKhoKhacVatTuThuocNhomKSNK = CheckMenuCanView(roleFunctions, Enums.DocumentType.XuatKhoKhacVatTuThuocNhomKSNK, Enums.SecurityOperation.View),

                CanViewYeuCauDuTruMuaNhomKSNK = CheckMenuCanView(roleFunctions, Enums.DocumentType.YeuCauDuTruMuaNhomKSNK, Enums.SecurityOperation.View),
                CanViewTHDTMuaTaiKSNK = CheckMenuCanView(roleFunctions, Enums.DocumentType.THDTMuaTaiKSNK, Enums.SecurityOperation.View),
                CanViewTHDTMuaTaiHanhChinh = CheckMenuCanView(roleFunctions, Enums.DocumentType.THDTMuaTaiHanhChinh, Enums.SecurityOperation.View),
                CanViewTHDTMuaTaiGiamDoc = CheckMenuCanView(roleFunctions, Enums.DocumentType.THDTMuaTaiGiamDoc, Enums.SecurityOperation.View),


                CanViewYeuCauHoanTraKSNK = CheckMenuCanView(roleFunctions, Enums.DocumentType.YeuCauHoanTraKSNK, Enums.SecurityOperation.View),
                CanViewDuyetYeuCauHoanTraKSNK = CheckMenuCanView(roleFunctions, Enums.DocumentType.DuyetYeuCauHoanTraKSNK, Enums.SecurityOperation.View),

                CanViewDanhSachYeuCauLinhKSNK = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhSachYeuCauLinhKSNK, Enums.SecurityOperation.View),
                CanViewTaoYeuCauLinhThuongKSNK = CheckMenuCanView(roleFunctions, Enums.DocumentType.TaoYeuCauLinhThuongKSNK, Enums.SecurityOperation.View),
                CanViewTaoYeuCauLinhBuKSNK = CheckMenuCanView(roleFunctions, Enums.DocumentType.TaoYeuCauLinhBuKSNK, Enums.SecurityOperation.View),
                CanViewDuyetYeuCauLinhKSNK = CheckMenuCanView(roleFunctions, Enums.DocumentType.DuyetYeuCauLinhKSNK, Enums.SecurityOperation.View),

                CanViewBaoCaoXNXuatNhapTonKhoXetNghiem = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoXNXuatNhapTonKhoXetNghiem, Enums.SecurityOperation.View),
                CanViewBaoCaoXNPhieuNhapHoaChat = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoXNPhieuNhapHoaChat, Enums.SecurityOperation.View),
                CanViewBaoCaoXNPhieuXuatHoaChat = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoXNPhieuXuatHoaChat, Enums.SecurityOperation.View),

                CanViewDSThanhToanChiPhiKCBNgoaiTru = CheckMenuCanView(roleFunctions, Enums.DocumentType.DSThanhToanChiPhiKCBNgoaiTru, Enums.SecurityOperation.View),
                CanViewDSThanhToanChiPhiKCBNoiTru = CheckMenuCanView(roleFunctions, Enums.DocumentType.DSThanhToanChiPhiKCBNoiTru, Enums.SecurityOperation.View),

                CanViewQuanLyNgayLe = CheckMenuCanView(roleFunctions, Enums.DocumentType.QuanLyNgayLe, Enums.SecurityOperation.View),
                CanViewQuanLyLichLamViec = CheckMenuCanView(roleFunctions, Enums.DocumentType.QuanLyLichLamViec, Enums.SecurityOperation.View),

                CanViewThongKeDichVuKhamSucKhoe = CheckMenuCanView(roleFunctions, Enums.DocumentType.ThongKeDichVuKhamSucKhoe, Enums.SecurityOperation.View),
                CanViewDanhMucQuanLyHDPP = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucQuanLyHDPP, Enums.SecurityOperation.View),

                CanViewDanhMucCauHinhThuePhong = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucCauHinhThuePhong, Enums.SecurityOperation.View),
                CanViewDanhMucLichSuThuePhong = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucLichSuThuePhong, Enums.SecurityOperation.View),

                CanViewDayLenCongGiamDinh7980a = CheckMenuCanView(roleFunctions, Enums.DocumentType.DayLenCongGiamDinh7980a, Enums.SecurityOperation.View),
                CanViewGiamDinhBHYT7980aXuatChoKeToan = CheckMenuCanView(roleFunctions, Enums.DocumentType.GiamDinhBHYT7980aXuatChoKeToan, Enums.SecurityOperation.View),
                CanViewCauHinhNguoiDuyetTheoNhomDichVu = CheckMenuCanView(roleFunctions, Enums.DocumentType.CauHinhNguoiDuyetTheoNhomDichVu, Enums.SecurityOperation.View),


                CanViewBaoCaoTinhHinhNhapNhaCungCapChiTiet = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoTinhHinhNhapNhaCungCapChiTiet, Enums.SecurityOperation.View),
                CanViewBaoCaoTinhHinhTraNhaCungCapChiTiet = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoTinhHinhTraNhaCungCapChiTiet, Enums.SecurityOperation.View),

                CanViewVTYTTinhHinhTraNCC = CheckMenuCanView(roleFunctions, Enums.DocumentType.VTYTTinhHinhTraNCC, Enums.SecurityOperation.View),
                CanViewBaoCaoKeToanBangKeChiTietNguoiBenh = CheckMenuCanView(roleFunctions, Enums.DocumentType.BaoCaoKeToanBangKeChiTietNguoiBenh, Enums.SecurityOperation.View),
                CanViewVTYTBaoCaoChiTietXuatNoiBo = CheckMenuCanView(roleFunctions, Enums.DocumentType.VTYTBaoCaoChiTietXuatNoiBo, Enums.SecurityOperation.View),
                CanViewKHTHBaoCaoThongKeSLThuThuat = CheckMenuCanView(roleFunctions, Enums.DocumentType.KHTHBaoCaoThongKeSLThuThuat, Enums.SecurityOperation.View),
                CanViewVTYTBaoCaoChiTietHoanTraNoiBo = CheckMenuCanView(roleFunctions, Enums.DocumentType.VTYTBaoCaoChiTietHoanTraNoiBo, Enums.SecurityOperation.View),
                CanViewBCDTKhamDoanTheoNhomDVDGThucTe = CheckMenuCanView(roleFunctions, Enums.DocumentType.BCDTKhamDoanTheoNhomDVDGThucTe, Enums.SecurityOperation.View),
                CanViewBCDTKhamDoanTheoKhoaPhongDGThucTe = CheckMenuCanView(roleFunctions, Enums.DocumentType.BCDTKhamDoanTheoKhoaPhongDGThucTe, Enums.SecurityOperation.View),

                CanViewDanhMucLoaiGiaDichVu = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucLoaiGiaDichVu, Enums.SecurityOperation.View),
                CanViewBCDTChiaTheoPhong = CheckMenuCanView(roleFunctions, Enums.DocumentType.BCDTChiaTheoPhong, Enums.SecurityOperation.View),
                CanViewBCDTTongHopDoanhThuTheoNguonBenhNhan = CheckMenuCanView(roleFunctions, Enums.DocumentType.BCDTTongHopDoanhThuTheoNguonBenhNhan, Enums.SecurityOperation.View),
                CanViewDanhMucCauHinhHeSoTheoNoiGioiThieuHoaHong = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucCauHinhHeSoTheoNoiGioiThieuHoaHong, Enums.SecurityOperation.View),
                CanViewDanhMucDonViHanhChinh = CheckMenuCanView(roleFunctions, Enums.DocumentType.DanhMucDonViHanhChinh, Enums.SecurityOperation.View),
                #endregion Internal

            };
            return objResult;
        }

        public PortalMenuInfo GetPortalMenuInfo(long[] roleIds)
        {
            var roles = BaseRepository.TableNoTracking.Include(o => o.RoleFunctions).ToList();
            var userRoles = roles.Where(o => roleIds.Contains(o.Id));
            var roleFunctions = userRoles.SelectMany(o => o.RoleFunctions).ToList();
            var objResult = new PortalMenuInfo
            {
            };
            return objResult;
        }
        public ICollection<CaminoPermission> GetPermissions(long[] roleIds)
        {
            var roles = BaseRepository.TableNoTracking.Include(o => o.RoleFunctions).ToList();
            var userRoles = roles.Where(o => roleIds.Contains(o.Id));
            return userRoles.SelectMany(o => o.RoleFunctions.Select(r => new CaminoPermission { DocumentType = r.DocumentType, SecurityOperation = r.SecurityOperation })).ToList();
        }
        public async Task<ICollection<LookupItemVo>> GetLookupAsync()
        {
            return await BaseRepository.TableNoTracking.Select(o => new LookupItemVo { DisplayName = o.Name, KeyId = o.Id }).ToListAsync();
        }


        #region CRUD

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var query = BaseRepository.TableNoTracking.Where(o => o.UserType == Enums.UserType.NhanVien).Select(s => new RoleGridVo
            {
                Id = s.Id,
                Name = s.Name,
                IsDefault = s.IsDefault
            }).ApplyLike(queryInfo.SearchTerms, x => x.Name);

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking.Where(o => o.UserType == Enums.UserType.NhanVien).Select(s => new RoleGridVo
            {
                Id = s.Id,
                Name = s.Name,
                IsDefault = s.IsDefault
            }).ApplyLike(queryInfo.SearchTerms, o => o.Name);

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task UpdateRoleFunctionForRole(List<RoleFunction> roleFunctions, long roleId)
        {
            await RemoveAllRoleFuntionOld(roleId);
            //Add new role function for role
            await AddPermission(roleFunctions, roleId);
        }

        public async Task AddPermissionForRole(List<RoleFunction> roleFunctions, long roleId)
        {
            await AddPermission(roleFunctions, roleId);
        }

        private async Task AddPermission(List<RoleFunction> roleFunctions, long roleId)
        {
            var lstRoleFunctionsNew = roleFunctions.Select(roleFunction => new RoleFunction
            {
                RoleId = roleId,
                SecurityOperation = roleFunction.SecurityOperation,
                DocumentType = roleFunction.DocumentType
            })
                .ToList();
            await _roleFunctionRepository.AddRangeAsync(lstRoleFunctionsNew);
            _cacheManager.RemoveByPattern(ROLES_PATTERN_KEY);
        }

        private async Task RemoveAllRoleFuntionOld(long roleId)
        {
            var roleFunctions = _roleFunctionRepository.Table.Where(p => p.RoleId == roleId).ToList();
            await _roleFunctionRepository.DeleteAsync(roleFunctions);
        }
        #endregion CRUD

        public async Task<ICollection<LookupItemVo>> GetRoleTypeNhanVienNoiBoAsync()
        {
            return await BaseRepository.TableNoTracking.Where(o => o.UserType == Enums.UserType.NhanVien).Select(o => new LookupItemVo { DisplayName = o.Name, KeyId = o.Id }).ToListAsync();
        }
        public long GetRoleTypeKhachVanLaiBoAsync()
        {
            return BaseRepository.TableNoTracking.FirstOrDefault(o => o.UserType == Enums.UserType.KhachVangLai).Id;
        }

        public Task<Role> GetRoleWithUserType(Enums.UserType userType)
        {
            return Task.FromResult(BaseRepository.TableNoTracking.First(p => p.UserType == userType));
        }

        public async Task<ICollection<LookupItemTextVo>> GetRoleQuyenHanNhanVienAsync()
        {
            return await BaseRepository.TableNoTracking.Where(o => o.UserType == Enums.UserType.NhanVien).Select(o => new LookupItemTextVo { DisplayName = o.Name, KeyId = o.Name }).ToListAsync();
        }

        public bool IsHavePermissionForUpdateInformationTNBN()
        {
            var lstRoleId = _userAgentHelper.GetListCurrentUserRoleId();

            var result = false;

            //foreach (var roleId in lstRoleId)
            //{
            //    var roleFunction = _roleFunctionRepository.TableNoTracking.Where(p => p.RoleId == roleId);
            //    if (roleFunction.Any(p => p.DocumentType == Enums.DocumentType.YeuCauTiepNhanChinhSuaThongTinHanhChinh))
            //    {
            //        result = true;
            //    }
            //}

            if (lstRoleId.Any())
            {
                result = _roleFunctionRepository.TableNoTracking
                    .Any(p => p.DocumentType == Enums.DocumentType.YeuCauTiepNhanChinhSuaThongTinHanhChinh
                                && lstRoleId.Contains(p.RoleId));
            }

            return result;
        }
    }
}
