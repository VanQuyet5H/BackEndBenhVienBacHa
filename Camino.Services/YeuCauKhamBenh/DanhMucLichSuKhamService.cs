using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Data;
using System.Linq.Dynamic.Core;
using Newtonsoft.Json;
using System;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using static Camino.Core.Domain.Enums;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;

namespace Camino.Services.YeuCauKhamBenh
{
    public partial class YeuCauKhamBenhService
    {
        public async Task<GridDataSource> GetDataForGridAsyncDanhMucLichSuKhamBenh(QueryInfo queryInfo, bool exportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);
            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }
            var query = BaseRepository.TableNoTracking
                 .Where(yckb => yckb.YeuCauTiepNhan.LoaiYeuCauTiepNhan != EnumLoaiYeuCauTiepNhan.KhamSucKhoe
                 && (yckb.TrangThai == EnumTrangThaiYeuCauKhamBenh.DaKham || yckb.TrangThai == EnumTrangThaiYeuCauKhamBenh.HuyKham))
                .Select(s => new DanhSachChoKhamGridVo
                {
                    Id = s.Id,
                    YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                    MaYeuCauTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    BenhNhanId = s.YeuCauTiepNhan.BenhNhanId,
                    MaBenhNhan = s.YeuCauTiepNhan.BenhNhan.MaBN,
                    HoTen = s.YeuCauTiepNhan.HoTen,
                    NamSinh = s.YeuCauTiepNhan.NamSinh,
                    DiaChi = s.YeuCauTiepNhan.DiaChiDayDu,
                    ThoiDiemTiepNhanDisplay = s.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                    ThoiDiemTiepNhan = s.ThoiDiemChiDinh,
                    TrieuChungTiepNhan = s.YeuCauTiepNhan.TrieuChungTiepNhan,
                    BHYTMaSoThe = s.YeuCauTiepNhan.BHYTMaSoThe,
                    DoiTuong = s.YeuCauTiepNhan.CoBHYT != true ? "Viện phí" : "BHYT (" + s.YeuCauTiepNhan.BHYTMucHuong.ToString() + "%)",
                    TrangThaiYeuCauKhamBenh = s.TrangThai,
                    TrangThaiYeuCauKhamBenhSearch = s.TrangThai == EnumTrangThaiYeuCauKhamBenh.DaKham ? "Đã khám" : "Hủy khám",
                    TenDichVu = s.TenDichVu,
                    CoDonThuocBHYT = s.YeuCauKhamBenhDonThuocs.Any(p => p.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocBHYT && p.YeuCauKhamBenhDonThuocChiTiets.Any()),
                    CoDonThuocKhongBHYT = s.YeuCauKhamBenhDonThuocs.Any(p => p.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT && p.YeuCauKhamBenhDonThuocChiTiets.Any()),
                    CoDonVatTu = s.YeuCauKhamBenhDonVTYTs.Any(),
                    TenNhanVienTiepNhan = s.YeuCauTiepNhan.NhanVienTiepNhan.User.HoTen,
                    CachGiaiQuyet = s.CachGiaiQuyet,
                    ChuanDoan = s.GhiChuICDChinh,
                    BSKham = s.BacSiKetLuanId != null ? s.BacSiKetLuan.User.HoTen : null,
                    ThoiDiemThucHien = s.ThoiDiemThucHien,
                    TenNhanVienChiDinh = s.NhanVienChiDinh.User.HoTen,
                    CoNhapVien = s.CoNhapVien

                });
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {

                var queryString = JsonConvert.DeserializeObject<DanhSachChoKhamGridVo>(queryInfo.AdditionalSearchString);

                if (!string.IsNullOrEmpty(queryString.FromDate) || !string.IsNullOrEmpty(queryString.ToDate))
                {
                    DateTime denNgay;
                    queryString.FromDate.TryParseExactCustom(out var tuNgay);
                    //DateTime.TryParseExact(queryString.FromDate, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None,
                    //    out var tuNgay);
                    if (string.IsNullOrEmpty(queryString.ToDate))
                    {
                        denNgay = DateTime.Now;
                    }
                    else
                    {
                        queryString.ToDate.TryParseExactCustom(out denNgay);
                        //DateTime.TryParseExact(queryString.ToDate, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out denNgay);
                    }
                    denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                    query = query.Where(p => p.ThoiDiemTiepNhan >= tuNgay && p.ThoiDiemTiepNhan <= denNgay);
                }
                if (!string.IsNullOrEmpty(queryString.SearchString))
                {
                    var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
                    query = query.ApplyLike(searchTerms,
                        g => g.HoTen,
                        g => g.MaYeuCauTiepNhan,
                        g => g.NamSinh.ToString(),
                        g => g.DiaChi,
                        g => g.MaBenhNhan,
                        g => g.TrieuChungTiepNhan,
                        g => g.TenDichVu,
                        g => g.TenNhanVienTiepNhan,
                        g => g.BSKham,
                        g => g.ChuanDoan,
                        g => g.CachGiaiQuyet,
                        g => g.TenNhanVienChiDinh

                   );
                }
            }

            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                query = query.ApplyLike(queryInfo.SearchTerms,
                    g => g.HoTen,
                    g => g.MaYeuCauTiepNhan,
                    g => g.NamSinh.ToString(),
                    g => g.DiaChi,
                    g => g.MaBenhNhan,
                    g => g.TrieuChungTiepNhan,
                    g => g.TenDichVu,
                        g => g.TenNhanVienTiepNhan, g => g.BSKham,
                      g => g.ChuanDoan,
                      g => g.CachGiaiQuyet,
                      g => g.TenNhanVienChiDinh
                    );
            }
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsyncDanhMucLichSuKhamBenh(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking
                 .Where(yckb => yckb.YeuCauTiepNhan.LoaiYeuCauTiepNhan != EnumLoaiYeuCauTiepNhan.KhamSucKhoe
                 && (yckb.TrangThai == EnumTrangThaiYeuCauKhamBenh.DaKham || yckb.TrangThai == EnumTrangThaiYeuCauKhamBenh.HuyKham))
                .Select(s => new DanhSachChoKhamGridVo
                {
                    Id = s.Id,
                    YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                    MaYeuCauTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    BenhNhanId = s.YeuCauTiepNhan.BenhNhanId,
                    MaBenhNhan = s.YeuCauTiepNhan.BenhNhan.MaBN,
                    HoTen = s.YeuCauTiepNhan.HoTen,
                    NamSinh = s.YeuCauTiepNhan.NamSinh,
                    DiaChi = s.YeuCauTiepNhan.DiaChiDayDu,
                    ThoiDiemTiepNhanDisplay = s.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                    ThoiDiemTiepNhan = s.ThoiDiemChiDinh,
                    TrieuChungTiepNhan = s.YeuCauTiepNhan.TrieuChungTiepNhan,
                    BHYTMaSoThe = s.YeuCauTiepNhan.BHYTMaSoThe,
                    DoiTuong = s.YeuCauTiepNhan.CoBHYT != true ? "Viện phí" : "BHYT (" + s.YeuCauTiepNhan.BHYTMucHuong.ToString() + "%)",
                    TrangThaiYeuCauKhamBenh = s.TrangThai,
                    TrangThaiYeuCauKhamBenhSearch = s.TrangThai == EnumTrangThaiYeuCauKhamBenh.DaKham ? "Đã khám" : "Hủy khám",
                    TenDichVu = s.TenDichVu,
                    CoDonThuocBHYT = s.YeuCauKhamBenhDonThuocs.Any(p => p.YeuCauKhamBenhDonThuocChiTiets.Any(c => c.LaDuocPhamBenhVien == true)),
                    CoDonThuocKhongBHYT = s.YeuCauKhamBenhDonThuocs.Any(p => p.YeuCauKhamBenhDonThuocChiTiets.Any(c => c.LaDuocPhamBenhVien == false)),
                    CoDonVatTu = s.YeuCauKhamBenhDonVTYTs.Any(),
                    TenNhanVienTiepNhan = s.YeuCauTiepNhan.NhanVienTiepNhan.User.HoTen,
                    CachGiaiQuyet = s.CachGiaiQuyet,
                    ChuanDoan = s.Icdchinh.TenTiengViet,
                    BSKham = s.BacSiKetLuanId != null ? s.BacSiKetLuan.User.HoTen : null,
                    ThoiDiemThucHien = s.ThoiDiemThucHien,
                    TenNhanVienChiDinh = s.NhanVienChiDinh.User.HoTen
                });
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {

                var queryString = JsonConvert.DeserializeObject<DanhSachChoKhamGridVo>(queryInfo.AdditionalSearchString);

                if (!string.IsNullOrEmpty(queryString.FromDate) || !string.IsNullOrEmpty(queryString.ToDate))
                {
                    DateTime denNgay;
                    queryString.FromDate.TryParseExactCustom(out var tuNgay);
                    //DateTime.TryParseExact(queryString.FromDate, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None,
                    //    out var tuNgay);
                    if (string.IsNullOrEmpty(queryString.ToDate))
                    {
                        denNgay = DateTime.Now;
                    }
                    else
                    {
                        queryString.ToDate.TryParseExactCustom(out denNgay);
                        //DateTime.TryParseExact(queryString.ToDate, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out denNgay);
                    }
                    denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                    query = query.Where(p => p.ThoiDiemTiepNhan >= tuNgay && p.ThoiDiemTiepNhan <= denNgay);
                }
                if (!string.IsNullOrEmpty(queryString.SearchString))
                {
                    var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
                    query = query.ApplyLike(searchTerms,
                      g => g.HoTen,
                      g => g.MaYeuCauTiepNhan,
                      g => g.NamSinh.ToString(),
                      g => g.DiaChi,
                      g => g.MaBenhNhan,
                      g => g.TrieuChungTiepNhan,
                      g => g.TenDichVu,
                      g => g.TenNhanVienTiepNhan,
                      g => g.BSKham,
                      g => g.ChuanDoan,
                      g => g.CachGiaiQuyet,
                      g => g.TenNhanVienChiDinh
                   );

                }
            }

            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                query = query.ApplyLike(queryInfo.SearchTerms,
                      g => g.HoTen,
                      g => g.MaYeuCauTiepNhan,
                      g => g.NamSinh.ToString(),
                      g => g.DiaChi,
                      g => g.MaBenhNhan,
                      g => g.TrieuChungTiepNhan,
                      g => g.TenDichVu,
                        g => g.TenNhanVienTiepNhan,
                        g => g.BSKham,
                      g => g.ChuanDoan,
                      g => g.CachGiaiQuyet,
                      g => g.TenNhanVienChiDinh

                      );
            }
            //query = query.Where(o =>o.ThoiDiemTiepNhan.Value.Date == DateTime.Now.Date);
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }


        public async Task<ThongTinYeuCauKhamVo> GetThongTinYeuCauKham(long yeuCauKhamBenhId)
        {
            var result = await BaseRepository.TableNoTracking
                           .Include(o => o.YeuCauTiepNhan)
                           .Where(p => p.Id == yeuCauKhamBenhId)
                           .Select(s => new ThongTinYeuCauKhamVo
                           {
                               YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                               YeuCauKhamBenhTruocId = s.YeuCauKhamBenhTruocId,
                               BenhNhanId = s.YeuCauTiepNhan.BenhNhanId
                           }).FirstOrDefaultAsync();
            return result;
        }

        public async Task<ThongTinBenhNhanVo> GetThongTinBenhNhan(long yeuCauKhamBenhId)
        {

            var result = await BaseRepository.TableNoTracking
                           .Where(p => p.Id == yeuCauKhamBenhId)
                           .Select(s => new ThongTinBenhNhanVo
                           {
                               YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                               YeuCauKhamBenhTruocId = s.YeuCauKhamBenhTruocId,
                               MaYeuCauTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                               BenhNhanId = s.YeuCauTiepNhan.BenhNhanId,
                               MaBN = s.YeuCauTiepNhan.BenhNhan.MaBN,
                               HoTen = s.YeuCauTiepNhan.HoTen,
                               TenGioiTinh = s.YeuCauTiepNhan.GioiTinh == null ? null : s.YeuCauTiepNhan.GioiTinh.GetDescription(),
                               Tuoi = s.YeuCauTiepNhan.NamSinh == null ? 0 : (DateTime.Now.Year - s.YeuCauTiepNhan.NamSinh.Value),
                               SoDienThoai = s.YeuCauTiepNhan.SoDienThoai.ApplyFormatPhone(),
                               DanToc = s.YeuCauTiepNhan.DanToc.Ten ?? null,
                               BHYTMucHuong = s.YeuCauTiepNhan.BHYTMucHuong == null ? null : s.YeuCauTiepNhan.BHYTMucHuong,
                               NgheNghiep = s.YeuCauTiepNhan.NgheNghiep.Ten ?? null,
                               DiaChi = s.YeuCauTiepNhan.DiaChiDayDu,
                               TuyenKham = s.YeuCauTiepNhan.LyDoVaoVien == null ? null : s.YeuCauTiepNhan.LyDoVaoVien.GetDescription(),
                               TenLyDoTiepNhan = s.YeuCauTiepNhan.LyDoTiepNhan.Ten,
                               TenLyDoKhamBenh = s.YeuCauTiepNhan.TrieuChungTiepNhan,
                               ThoiDiemTiepNhanDisplay = s.YeuCauTiepNhan.ThoiDiemTiepNhan.ApplyFormatDateTimeSACH(),
                               NamSinh = s.YeuCauTiepNhan.NamSinh,
                               ThangSinh = s.YeuCauTiepNhan.ThangSinh,
                               NgaySinh = s.YeuCauTiepNhan.NgaySinh,
                               TuoiThoiDiemHienTai = "",
                               SoBHYT = s.YeuCauTiepNhan.BHYTMaSoThe,
                               CoBHYT = s.YeuCauTiepNhan.CoBHYT,
                               BHYTNgayHetHan = s.YeuCauTiepNhan.BHYTNgayHetHan,
                               BHYTNgayHieuLuc = s.YeuCauTiepNhan.BHYTNgayHieuLuc,
                               LoaiYeuCauTiepNhan = s.YeuCauTiepNhan.LoaiYeuCauTiepNhan,
                               TenCongTy = s.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoe.Ten,

                               //BVHD-3800
                               LaCapCuu = s.YeuCauTiepNhan.LaCapCuu,

                               //BVHD-3941
                               CoBaoHiemTuNhan = s.YeuCauTiepNhan.CoBHTN
                           }).FirstOrDefaultAsync();
            if (result != null)
            {
                var tuoiThoiDiemHienTai = 0;
                if (result.NamSinh != null)
                {
                    tuoiThoiDiemHienTai = DateTime.Now.Year - result.NamSinh.Value;
                }
                var dobConvert = DateHelper.ConvertDOBToTimeJson(result.NgaySinh, result.ThangSinh, result.NamSinh);
                var jsonConvertString = new NgayThangNamSinhVo();

                if (!string.IsNullOrEmpty(dobConvert) && tuoiThoiDiemHienTai < 6)
                {
                    jsonConvertString = JsonConvert.DeserializeObject<NgayThangNamSinhVo>(dobConvert);
                }

                var tuoiBenhNhan = result.NamSinh != null ?
                                (tuoiThoiDiemHienTai < 6 ? jsonConvertString.Years + " Tuổi " + jsonConvertString.Months + " Tháng " + jsonConvertString.Days + " Ngày" : tuoiThoiDiemHienTai.ToString()) : tuoiThoiDiemHienTai.ToString();
                result.TuoiThoiDiemHienTai = tuoiBenhNhan;
            }
            return result;
        }

        public string InToaThuocKhamBenhDanhSach(InToaThuocKhamBenhDanhSach inToaThuoc)
        {
            var infoBN = ThongTinBenhNhanPhieuThuoc(inToaThuoc.YeuCauTiepNhanId, inToaThuoc.YeuCauKhamBenhId);

            var content = string.Empty;
            var contentThuocBHYT = string.Empty;
            var contentThuocTrongBenhVien = string.Empty;
            var contentThuocNgoaiBenhVien = string.Empty;

            var contentVatTu = string.Empty;
            var resultThuocBHYT = string.Empty;

            var resultThuocTrongBenhVien = string.Empty;
            var resultThuocNgoaiBenhVien = string.Empty;
            var resultVatTu = string.Empty;
            var resultThuocThucPhamChucNang = string.Empty;
            var contentThucPhamChucNang = string.Empty;

            var templateDonThuocBHYT = infoBN.LaTreEm == true ? _templateRepository.TableNoTracking.Where(x => x.Name.Equals("DonThuocBHYTTreEm")).First() : _templateRepository.TableNoTracking.Where(x => x.Name.Equals("DonThuocBHYT")).First();
            var templateDonThuocTrongBenhVien = infoBN.LaTreEm == true ? _templateRepository.TableNoTracking.Where(x => x.Name.Equals("DonThuocKhongBHYTTreEm")).First() : _templateRepository.TableNoTracking.Where(x => x.Name.Equals("DonThuocKhongBHYT")).First();
            var templateDonThuocThucPhamChucNang = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("DonThuocThucPhamChucNang")).FirstOrDefault();
            var templateDonThuocNgoaiBenhVien = infoBN.LaTreEm == true ? _templateRepository.TableNoTracking.Where(x => x.Name.Equals("DonThuocNgoaiBVTreEm")).First() : _templateRepository.TableNoTracking.Where(x => x.Name.Equals("DonThuocNgoaiBV")).First();

            var templateVatTuYT = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("KhamBenhVatTuYTe")).First();

            //Thuốc trong BV
            var donThuocBHYTChiTiets = _yeuCauKhamBenhDonThuocChiTietRepository.TableNoTracking
                                .Where(z => z.YeuCauKhamBenhDonThuoc.YeuCauKhamBenhId == inToaThuoc.YeuCauKhamBenhId && z.YeuCauKhamBenhDonThuoc.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT)
                                .Select(s => new InNoiTruDonThuocChiTietVo
                                {
                                    STT = s.SoThuTu,
                                    Id = s.Id,
                                    Ten = s.Ten,
                                    MaHoatChat = s.MaHoatChat,
                                    HoatChat = s.HoatChat,
                                    HamLuong = s.HamLuong,
                                    TenDuongDung = s.DuongDung.Ten,
                                    DVT = s.DonViTinh.Ten,
                                    SoLuong = s.SoLuong,
                                    SoNgayDung = s.SoNgayDung,
                                    ThoiGianDungSang = s.ThoiGianDungSang,
                                    ThoiGianDungTrua = s.ThoiGianDungTrua,
                                    ThoiGianDungChieu = s.ThoiGianDungChieu,
                                    ThoiGianDungToi = s.ThoiGianDungToi,
                                    // thuốc gây nghiện,hướng thần thì cách dùng con số chuyển thành text , ngược lại nếu thuống thường kiểm tra sl kê nhỏ hơn 10 thì thêm 0 phía trước , còn lại bình thường
                                    DungSang = s.DungSang,
                                    DungTrua = s.DungTrua,
                                    DungChieu = s.DungChieu,
                                    DungToi = s.DungToi,
                                    ThoiDiemKeDon = s.YeuCauKhamBenhDonThuoc.ThoiDiemKeDon,
                                    GhiChu = s.YeuCauKhamBenhDonThuoc.GhiChu,
                                    CachDung = s.GhiChu,
                                    LaDuocPhamBenhVien = s.LaDuocPhamBenhVien,
                                    TenBacSiKeDon = s.YeuCauKhamBenhDonThuoc.BacSiKeDon.User.HoTen,
                                    BacSiKeDonId = s.YeuCauKhamBenhDonThuoc.BacSiKeDonId,
                                    LoaiThuocTheoQuanLy = s.DuocPham.DuocPhamBenhVien.LoaiThuocTheoQuanLy,
                                    DuocPhamBenhVienPhanNhomId = s.DuocPham.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId,
                                    DuongDungId = s.DuongDungId
                                }).OrderBy(p => p.STT ?? 0).ToList();

            //var donThuocBHYTsDoc = donThuocBHYTChiTiets.Where(z =>
            //                                                                                      z.DuongDungId != Constants.DuongDungIdSapXep.Tiem
            //                                                                                   && z.DuongDungId != Constants.DuongDungIdSapXep.Uong
            //                                                                                   && z.DuongDungId != Constants.DuongDungIdSapXep.Dat
            //                                                                                   && z.DuongDungId != Constants.DuongDungIdSapXep.DungNgoai
            //                                                                                   && z.LaDuocPhamBenhVien
            //                                                                                   && z.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.ThuocDoc).ToList();
            //var donThuocBHYTsTiem = donThuocBHYTChiTiets.Where(z => z.DuongDungId == Constants.DuongDungIdSapXep.Tiem
            //                                                             && z.LaDuocPhamBenhVien
            //                                                             && z.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.ThuocDoc).ToList();
            //var donThuocBHYTsUong = donThuocBHYTChiTiets.Where(z => z.DuongDungId == Constants.DuongDungIdSapXep.Uong
            //                                                             && z.LaDuocPhamBenhVien
            //                                                              && z.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.ThuocDoc).ToList();
            //var donThuocBHYTsDat = donThuocBHYTChiTiets.Where(z => z.DuongDungId == Constants.DuongDungIdSapXep.Dat
            //                                                            && z.LaDuocPhamBenhVien
            //                                                             && z.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.ThuocDoc).ToList();
            //var donThuocBHYTsDungNgoai = donThuocBHYTChiTiets.Where(z => z.DuongDungId == Constants.DuongDungIdSapXep.DungNgoai
            //                                                                  && z.LaDuocPhamBenhVien
            //                                                                   && z.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.ThuocDoc).ToList();

            //var donThuocBHYTsKhac = donThuocBHYTChiTiets.Where(z => z.DuongDungId != Constants.DuongDungIdSapXep.Tiem
            //                                                                                   && z.DuongDungId != Constants.DuongDungIdSapXep.Uong
            //                                                                                   && z.DuongDungId != Constants.DuongDungIdSapXep.Dat
            //                                                                                   && z.DuongDungId != Constants.DuongDungIdSapXep.DungNgoai
            //                                                                                   && z.LaDuocPhamBenhVien
            //                                                                                 && z.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.ThuocDoc
            //                                                                                 ).ToList();
            //var donThuocBHYTs = donThuocBHYTsDoc
            //               .Concat(donThuocBHYTsTiem)
            //               .Concat(donThuocBHYTsUong)
            //               .Concat(donThuocBHYTsDat)
            //               .Concat(donThuocBHYTsDungNgoai)
            //               .Concat(donThuocBHYTsKhac);
            var donThuocBHYTs = donThuocBHYTChiTiets;

            var donThuocKhongBHYTChiTiets = _yeuCauKhamBenhDonThuocChiTietRepository.TableNoTracking
                                .Where(z => z.YeuCauKhamBenhDonThuoc.YeuCauKhamBenhId == inToaThuoc.YeuCauKhamBenhId && z.YeuCauKhamBenhDonThuoc.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocKhongBHYT)
                                .Select(s => new InNoiTruDonThuocChiTietVo
                                {
                                    STT = s.SoThuTu,
                                    Id = s.Id,
                                    Ten = s.Ten,
                                    MaHoatChat = s.MaHoatChat,
                                    HoatChat = s.HoatChat,
                                    HamLuong = s.HamLuong,
                                    TenDuongDung = s.DuongDung.Ten,
                                    DVT = s.DonViTinh.Ten,
                                    SoLuong = s.SoLuong,
                                    SoNgayDung = s.SoNgayDung,
                                    ThoiGianDungSang = s.ThoiGianDungSang,
                                    ThoiGianDungTrua = s.ThoiGianDungTrua,
                                    ThoiGianDungChieu = s.ThoiGianDungChieu,
                                    ThoiGianDungToi = s.ThoiGianDungToi,
                                    DungSang = s.DungSang,
                                    DungTrua = s.DungTrua,
                                    DungChieu = s.DungChieu,
                                    DungToi = s.DungToi,
                                    ThoiDiemKeDon = s.YeuCauKhamBenhDonThuoc.ThoiDiemKeDon,
                                    GhiChu = s.YeuCauKhamBenhDonThuoc.GhiChu,
                                    CachDung = s.GhiChu,
                                    LaDuocPhamBenhVien = s.LaDuocPhamBenhVien,
                                    TenBacSiKeDon = s.YeuCauKhamBenhDonThuoc.BacSiKeDon.User.HoTen,
                                    BacSiKeDonId = s.YeuCauKhamBenhDonThuoc.BacSiKeDonId,
                                    LoaiThuocTheoQuanLy = s.DuocPham.DuocPhamBenhVien.LoaiThuocTheoQuanLy,
                                    DuocPhamBenhVienPhanNhomId = s.DuocPham.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId,
                                    DuongDungId = s.DuongDungId
                                }).ToList();
            var duocPhamBenhVienPhanNhoms = _duocPhamBenhVienPhanNhomRepository.TableNoTracking.ToList();

            foreach (var thuoc in donThuocKhongBHYTChiTiets)
            {
                thuoc.DuocPhamBenhVienPhanNhomChaId = CalculateHelper.GetDuocPhamBenhVienPhanNhomCha(thuoc.DuocPhamBenhVienPhanNhomId.GetValueOrDefault(), duocPhamBenhVienPhanNhoms);
            }

            var userCurrentId = donThuocBHYTChiTiets.Any() ? donThuocBHYTChiTiets.First().BacSiKeDonId : (donThuocKhongBHYTChiTiets.Any() ? donThuocKhongBHYTChiTiets.First().BacSiKeDonId : 0);

            var tenBacSiKeDon = _userRepository.TableNoTracking
                             .Where(u => u.Id == userCurrentId).Select(u =>
                             (u.NhanVien.HocHamHocVi != null ? u.NhanVien.HocHamHocVi.Ma + " " : "")
                           //+ (u.NhanVien.ChucDanh != null ? u.NhanVien.ChucDanh.NhomChucDanh.Ma + "." : "")
                           + u.HoTen).FirstOrDefault();
            //var donThuocTrongBVKhongBHYTsDoc = donThuocKhongBHYTChiTiets.Where(z =>
            //                                                                                     z.DuongDungId != Constants.DuongDungIdSapXep.Tiem
            //                                                                                  && z.DuongDungId != Constants.DuongDungIdSapXep.Uong
            //                                                                                  && z.DuongDungId != Constants.DuongDungIdSapXep.Dat
            //                                                                                  && z.DuongDungId != Constants.DuongDungIdSapXep.DungNgoai
            //                                                                                  && z.LaDuocPhamBenhVien
            //                                                                                  && z.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.ThuocDoc).ToList();
            //var donThuocTrongBVKhongBHYTsTiem = donThuocKhongBHYTChiTiets.Where(z => z.DuongDungId == Constants.DuongDungIdSapXep.Tiem
            //                                                              && z.LaDuocPhamBenhVien
            //                                                              && z.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.ThuocDoc).ToList();
            //var donThuocTrongBVKhongBHYTsUong = donThuocKhongBHYTChiTiets.Where(z => z.DuongDungId == Constants.DuongDungIdSapXep.Uong
            //                                                             && z.LaDuocPhamBenhVien
            //                                                             && z.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.ThuocDoc).ToList();
            //var donThuocTrongBVKhongBHYTsDat = donThuocKhongBHYTChiTiets.Where(z => z.DuongDungId == Constants.DuongDungIdSapXep.Dat
            //                                                            && z.LaDuocPhamBenhVien
            //                                                            && z.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.ThuocDoc).ToList();
            //var donThuocTrongBVKhongBHYTsDungNgoai = donThuocKhongBHYTChiTiets.Where(z => z.DuongDungId == Constants.DuongDungIdSapXep.DungNgoai
            //                                                                  && z.LaDuocPhamBenhVien
            //                                                                  && z.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.ThuocDoc).ToList();

            //var donThuocTrongBVKhongBHYTsKhac = donThuocKhongBHYTChiTiets.Where(z => z.DuongDungId != Constants.DuongDungIdSapXep.Tiem
            //                                                                                   && z.DuongDungId != Constants.DuongDungIdSapXep.Uong
            //                                                                                   && z.DuongDungId != Constants.DuongDungIdSapXep.Dat
            //                                                                                   && z.DuongDungId != Constants.DuongDungIdSapXep.DungNgoai
            //                                                                                   && z.LaDuocPhamBenhVien
            //                                                                                 && z.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.ThuocDoc
            //                                                                                 ).ToList();
            //var donThuocTrongBVs = donThuocTrongBVKhongBHYTsDoc
            //               .Concat(donThuocTrongBVKhongBHYTsTiem)
            //               .Concat(donThuocTrongBVKhongBHYTsUong)
            //               .Concat(donThuocTrongBVKhongBHYTsDat)
            //               .Concat(donThuocTrongBVKhongBHYTsDungNgoai)
            //               .Concat(donThuocTrongBVKhongBHYTsKhac);
            var donThuocTrongBVs = donThuocKhongBHYTChiTiets.Where(z => z.LaDuocPhamBenhVien).OrderBy(z => z.STT).ToList();

            //var donThuocNgoaiBVsDoc = donThuocKhongBHYTChiTiets.Where(z =>
            //                                                                                     z.DuongDungId != Constants.DuongDungIdSapXep.Tiem
            //                                                                                  && z.DuongDungId != Constants.DuongDungIdSapXep.Uong
            //                                                                                  && z.DuongDungId != Constants.DuongDungIdSapXep.Dat
            //                                                                                  && z.DuongDungId != Constants.DuongDungIdSapXep.DungNgoai
            //                                                                                  && z.LaDuocPhamBenhVien
            //                                                                                  && z.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.ThuocDoc).ToList();
            //var donThuocNgoaiBVsTiem = donThuocKhongBHYTChiTiets.Where(z => z.DuongDungId == Constants.DuongDungIdSapXep.Tiem
            //                                                              && !z.LaDuocPhamBenhVien
            //                                                              && z.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.ThuocDoc).ToList();
            //var donThuocNgoaiBVsUong = donThuocKhongBHYTChiTiets.Where(z => z.DuongDungId == Constants.DuongDungIdSapXep.Uong
            //                                                             && !z.LaDuocPhamBenhVien
            //                                                             && z.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.ThuocDoc).ToList();
            //var donThuocNgoaiBVsDat = donThuocKhongBHYTChiTiets.Where(z => z.DuongDungId == Constants.DuongDungIdSapXep.Dat
            //                                                            && !z.LaDuocPhamBenhVien
            //                                                            && z.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.ThuocDoc).ToList();
            //var donThuocNgoaiBVsDungNgoai = donThuocKhongBHYTChiTiets.Where(z => z.DuongDungId == Constants.DuongDungIdSapXep.DungNgoai
            //                                                                  && !z.LaDuocPhamBenhVien
            //                                                                  && z.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.ThuocDoc).ToList();

            //var donThuocNgoaiBVsKhac = donThuocKhongBHYTChiTiets.Where(z => z.DuongDungId != Constants.DuongDungIdSapXep.Tiem
            //                                                                                   && z.DuongDungId != Constants.DuongDungIdSapXep.Uong
            //                                                                                   && z.DuongDungId != Constants.DuongDungIdSapXep.Dat
            //                                                                                   && z.DuongDungId != Constants.DuongDungIdSapXep.DungNgoai
            //                                                                                   && !z.LaDuocPhamBenhVien
            //                                                                                 && z.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.ThuocDoc
            //                                                                                 ).ToList();
            //var donThuocNgoaiBVs = donThuocNgoaiBVsDoc
            //               .Concat(donThuocNgoaiBVsTiem)
            //               .Concat(donThuocNgoaiBVsUong)
            //               .Concat(donThuocNgoaiBVsDat)
            //               .Concat(donThuocNgoaiBVsDungNgoai)
            //               .Concat(donThuocNgoaiBVsKhac);
            var donThuocNgoaiBVs = donThuocKhongBHYTChiTiets.Where(z => !z.LaDuocPhamBenhVien).OrderBy(z => z.STT).ToList();


            var headerBHYT = string.Empty;
            var headerKhongBHYT = string.Empty;
            var headerThuocNgoaiBV = string.Empty;
            var headerThucPhamChucNang = string.Empty;

            //if (inToaThuoc.Header)
            //{
            //    headerBHYT = "<p  style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
            //                  "<th>TOA THUỐC BẢO HIỂM Y TẾ</th>" +
            //             "</p>";
            //    headerKhongBHYT = "<p  style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
            //                  "<th>TOA THUỐC KHÔNG BẢO HIỂM Y TẾ</th>" +
            //             "</p>";

            //    headerThuocNgoaiBV = "<p  style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
            //                 "<th>TOA THUỐC NGOÀI BỆNH VIỆN</ th>" +
            //                 "</p>";

            //    headerThucPhamChucNang = "<p  style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
            //                     "<th>ĐƠN TƯ VẤN</ th>" +
            //                     "</p>";
            //}
            var sttBHYT = 0;

            var sttKhongBHYTTrongBV = 0;
            var sttKhongBHYTNgoaiBV = 0;
            var sttTPCN = 0;
            if (donThuocBHYTs.Any())
            {
                foreach (var donThuocBHYTChiTiet in donThuocBHYTs)
                {
                    var cd =
                             (!string.IsNullOrEmpty(donThuocBHYTChiTiet.DungSangDisplay)
                                 ? "Sáng " + donThuocBHYTChiTiet.DungSang
                                 +
                                   (!string.IsNullOrEmpty(donThuocBHYTChiTiet.ThoiGianDungSangDisplay)
                                       ? " " + donThuocBHYTChiTiet.ThoiGianDungSangDisplay
                                       : "") + " " + donThuocBHYTChiTiet.DVT + ","
                                 : "") +
                             (!string.IsNullOrEmpty(donThuocBHYTChiTiet.DungTruaDisplay)
                                 ? "Trưa " + donThuocBHYTChiTiet.DungTrua +
                                   (!string.IsNullOrEmpty(donThuocBHYTChiTiet.ThoiGianDungTruaDisplay)
                                       ? " " + donThuocBHYTChiTiet.ThoiGianDungTruaDisplay
                                       : "") + " " + donThuocBHYTChiTiet.DVT + ","
                                 : "") +
                             (!string.IsNullOrEmpty(donThuocBHYTChiTiet.DungChieuDisplay)
                                 ? "Chiều " + donThuocBHYTChiTiet.DungChieu +
                                   (!string.IsNullOrEmpty(donThuocBHYTChiTiet.ThoiGianDungChieuDisplay)
                                       ? " " + donThuocBHYTChiTiet.ThoiGianDungChieuDisplay
                                       : "") + " " + donThuocBHYTChiTiet.DVT + ","
                                 : "") +
                            (!string.IsNullOrEmpty(donThuocBHYTChiTiet.DungToiDisplay)
                                 ? "Tối " + donThuocBHYTChiTiet.DungToi +
                                   (!string.IsNullOrEmpty(donThuocBHYTChiTiet.ThoiGianDungToiDisplay)
                                       ? " " + donThuocBHYTChiTiet.ThoiGianDungToiDisplay
                                       : "") + " " + donThuocBHYTChiTiet.DVT + ","
                                 : "");

                    var cachDung = (!string.IsNullOrEmpty(cd) ? "<i>" + cd.Trim().Remove(cd.Trim().Length - 1) + "<i></br>" : "")
                                 + (!string.IsNullOrEmpty(donThuocBHYTChiTiet.CachDung) ? "<p style='margin:0'><i>" + donThuocBHYTChiTiet.CachDung + " </i></p>" : "");
                    sttBHYT++;
                    resultThuocBHYT += "<tr>";
                    resultThuocBHYT += "<td style='vertical-align: top; text-align: center' >" + sttBHYT + "</td>";
                    resultThuocBHYT += "<td >" + FormatTenDuocPham(donThuocBHYTChiTiet.Ten, donThuocBHYTChiTiet.HoatChat, donThuocBHYTChiTiet.HamLuong, donThuocBHYTChiTiet.DuocPhamBenhVienPhanNhomId)
                        + (!string.IsNullOrEmpty(cachDung) ? "</br> " + cachDung : "")
                        + "</td>";

                    resultThuocBHYT += "<td  style='vertical-align: top;text-align: center' >" + FormatSoLuong(donThuocBHYTChiTiet.SoLuong, donThuocBHYTChiTiet.LoaiThuocTheoQuanLy) + " " + donThuocBHYTChiTiet.DVT + "</td>";
                    resultThuocBHYT += "</tr>";
                }

                if (!string.IsNullOrEmpty(resultThuocBHYT))
                {
                    resultThuocBHYT = "<style>.thuoc-table{border-top: 1px solid #000;border-right: 1px solid #000;border-spacing: 0;}.thuoc-table td,.thuoc-table th{border-left: 1px solid #000;border-bottom: 1px solid #000;padding: 5px;}</style><table width='100%' class='thuoc-table'><thead><tr><th>STT</th><th>Tên thuốc – Hàm lượng - Liều dùng</th><th>Số lượng</th></tr></thead><tbody>" + resultThuocBHYT + "</tbody></table>";
                    var data = new DataYCKBDonThuoc
                    {
                        Header = headerBHYT,
                        TemplateDonThuoc = resultThuocBHYT,
                        LogoUrl = inToaThuoc.HostingName + "/assets/img/logo-bacha-full.png",
                        BarCodeImgBase64 = !string.IsNullOrEmpty(infoBN.MaTN) ? BarcodeHelper.GenerateBarCode(infoBN.MaTN) : "",
                        MaTN = "<b>Mã TN: </b>" + "<b>" + infoBN.MaTN + "</b>",
                        HoTen = infoBN.HoTen,
                        Tuoi = infoBN.Tuoi,
                        NamSinhDayDu = infoBN.NamSinhDayDu,
                        CanNang = infoBN.CanNang,
                        GioiTinh = infoBN?.GioiTinh,
                        DiaChi = infoBN?.DiaChi,
                        CMND = infoBN?.CMND,
                        SoTheBHYT = infoBN.BHYTMaSoThe,
                        NgayHieuLuc = infoBN.BHYTNgayHieuLuc == null ? "" : (infoBN.BHYTNgayHieuLuc.Value).ApplyFormatDate(),
                        NgayHetHan = infoBN.BHYTNgayHetHan == null ? "" : (infoBN.BHYTNgayHetHan.Value).ApplyFormatDate(),
                        ChuanDoan = infoBN?.ChuanDoan,
                        ThoiDiemKeDon = donThuocBHYTChiTiets.Any() ? donThuocBHYTChiTiets.Select(z => z.ThoiDiemKeDon).First() :
                                (donThuocKhongBHYTChiTiets.Any() ? donThuocKhongBHYTChiTiets.Select(z => z.ThoiDiemKeDon).First() : (DateTime?)null),
                        BacSiKham = tenBacSiKeDon,
                        LoiDan = infoBN.LoiDan,
                        MaBN = infoBN.MaBN,
                        SoDienThoai = infoBN.SoDienThoai,
                        SoThang = infoBN.SoThang,
                        CongKhoan = sttBHYT,
                        //KhoaPhong = khoaPhong
                    };
                    contentThuocBHYT = TemplateHelpper.FormatTemplateWithContentTemplate(templateDonThuocBHYT.Body, data);
                }

            }

            if (donThuocKhongBHYTChiTiets.Any())
            {
                if (donThuocTrongBVs.Any())
                {
                    foreach (var donThuocTrongBV in donThuocTrongBVs)
                    {
                        var cd =
                             (!string.IsNullOrEmpty(donThuocTrongBV.DungSangDisplay)

                                     ? "Sáng " + donThuocTrongBV.DungSang +
                                       (!string.IsNullOrEmpty(donThuocTrongBV.ThoiGianDungSangDisplay)
                                           ? " " + donThuocTrongBV.ThoiGianDungSangDisplay
                                           : "") + " " + donThuocTrongBV.DVT + ","
                                     : "") +
                             (!string.IsNullOrEmpty(donThuocTrongBV.DungTruaDisplay)

                                     ? "Trưa " + donThuocTrongBV.DungTrua +
                                       (!string.IsNullOrEmpty(donThuocTrongBV.ThoiGianDungTruaDisplay)
                                           ? " " + donThuocTrongBV.ThoiGianDungTruaDisplay
                                           : "") + " " + donThuocTrongBV.DVT + ","
                                     : "") +
                             (!string.IsNullOrEmpty(donThuocTrongBV.DungChieuDisplay)

                                     ? "Chiều " + donThuocTrongBV.DungChieu +
                                       (!string.IsNullOrEmpty(donThuocTrongBV.ThoiGianDungChieuDisplay)
                                           ? " " + donThuocTrongBV.ThoiGianDungChieuDisplay
                                           : "") + " " + donThuocTrongBV.DVT + ","
                                     : "") +
                             (!string.IsNullOrEmpty(donThuocTrongBV.DungToiDisplay)

                                     ? "Tối " + donThuocTrongBV.DungToi +
                                       (!string.IsNullOrEmpty(donThuocTrongBV.ThoiGianDungToiDisplay)
                                           ? " " + donThuocTrongBV.ThoiGianDungToiDisplay
                                           : "") + " " + donThuocTrongBV.DVT + ","
                                     : "");

                        var cachDung = (!string.IsNullOrEmpty(cd) ? "<i>" + cd.Trim().Remove(cd.Trim().Length - 1) + "<i></br>" : "")
                               + (!string.IsNullOrEmpty(donThuocTrongBV.CachDung) ? "<p style='margin:0'><i>" + donThuocTrongBV.CachDung + " </i></p>" : "");
                        if (donThuocTrongBV.DuocPhamBenhVienPhanNhomChaId == (long)Enums.EnumDuocPhamBenhVienPhanNhom.ThucPhamChucNang
                             || donThuocTrongBV.DuocPhamBenhVienPhanNhomChaId == (long)Enums.EnumDuocPhamBenhVienPhanNhom.MyPham
                             || donThuocTrongBV.DuocPhamBenhVienPhanNhomChaId == (long)Enums.EnumDuocPhamBenhVienPhanNhom.VatTuYTe
                             || donThuocTrongBV.DuocPhamBenhVienPhanNhomChaId == (long)Enums.EnumDuocPhamBenhVienPhanNhom.ThietBiYTe
                             || donThuocTrongBV.DuocPhamBenhVienPhanNhomChaId == (long)Enums.EnumDuocPhamBenhVienPhanNhom.ThuocTuDuocLieu)
                        {
                            sttTPCN++;
                            resultThuocThucPhamChucNang += "<tr>";
                            resultThuocThucPhamChucNang += "<td style='vertical-align: top;text-align: center' >" + sttTPCN + "</td>";
                            resultThuocThucPhamChucNang += "<td >" + "<b>" + donThuocTrongBV.Ten + "</b>"
                             + (!string.IsNullOrEmpty(cachDung) ? "</br> " + cachDung : "")
                                + "</td>";
                            resultThuocThucPhamChucNang += "<td style='vertical-align: top;text-align: center' >" + FormatSoLuong(donThuocTrongBV.SoLuong, donThuocTrongBV.LoaiThuocTheoQuanLy) + " " + donThuocTrongBV.DVT + "</td>";
                            resultThuocThucPhamChucNang += "</tr>";


                        }
                        else
                        {
                            sttKhongBHYTTrongBV++;
                            resultThuocTrongBenhVien += "<tr>";
                            resultThuocTrongBenhVien += "<td style='vertical-align: top;text-align: center' >" + sttKhongBHYTTrongBV + "</td>";
                            resultThuocTrongBenhVien += "<td >" + FormatTenDuocPham(donThuocTrongBV.Ten, donThuocTrongBV.HoatChat, donThuocTrongBV.HamLuong, donThuocTrongBV.DuocPhamBenhVienPhanNhomId)
                                 + (!string.IsNullOrEmpty(cachDung) ? "</br> " + cachDung : "")
                                + "</td>";
                            resultThuocTrongBenhVien += "<td style='vertical-align: top;text-align: center'  >" + FormatSoLuong(donThuocTrongBV.SoLuong, donThuocTrongBV.LoaiThuocTheoQuanLy) + " " + donThuocTrongBV.DVT + "</td>";
                            resultThuocTrongBenhVien += "</tr>";
                        }
                    }
                    if (!string.IsNullOrEmpty(resultThuocTrongBenhVien))
                    {
                        resultThuocTrongBenhVien = "<style>.thuoc-table{border-top: 1px solid #000;border-right: 1px solid #000;border-spacing: 0;}.thuoc-table td,.thuoc-table th{border-left: 1px solid #000;border-bottom: 1px solid #000;padding: 5px;}</style><table width='100%' class='thuoc-table'><thead><tr><th>STT</th><th>Tên thuốc – Hàm lượng - Liều dùng</th><th>Số lượng</th></tr></thead><tbody>" + resultThuocTrongBenhVien + "</tbody></table>";
                        var data = new DataYCKBDonThuoc
                        {
                            Header = headerKhongBHYT,
                            TemplateDonThuoc = resultThuocTrongBenhVien,
                            LogoUrl = inToaThuoc.HostingName + "/assets/img/logo-bacha-full.png",
                            BarCodeImgBase64 = !string.IsNullOrEmpty(infoBN.MaTN) ? BarcodeHelper.GenerateBarCode(infoBN.MaTN) : "",
                            MaTN = "<b>Mã TN: </b>" + "<b>" + infoBN.MaTN + "</b>",
                            HoTen = infoBN.HoTen,
                            NamSinhDayDu = infoBN.NamSinhDayDu,

                            Tuoi = infoBN.Tuoi,
                            CMND = infoBN?.CMND,
                            CanNang = infoBN.CanNang,
                            GioiTinh = infoBN?.GioiTinh,
                            DiaChi = infoBN?.DiaChi,
                            SoTheBHYT = infoBN.BHYTMaSoThe,
                            NgayHieuLuc = infoBN.BHYTNgayHieuLuc == null ? "" : (infoBN.BHYTNgayHieuLuc.Value).ApplyFormatDate(),
                            NgayHetHan = infoBN.BHYTNgayHetHan == null ? "" : (infoBN.BHYTNgayHetHan.Value).ApplyFormatDate(),
                            ChuanDoan = infoBN?.ChuanDoan,
                            BacSiKham = tenBacSiKeDon,
                            LoiDan = infoBN.LoiDan,
                            NguoiGiamHo = infoBN?.NguoiGiamHo,
                            MaBN = infoBN.MaBN,
                            SoDienThoai = infoBN.SoDienThoai,
                            SoThang = infoBN.SoThang,
                            CongKhoan = sttKhongBHYTTrongBV,
                            //KhoaPhong = khoaPhong,
                            ThoiDiemKeDon = donThuocBHYTChiTiets.Any() ? donThuocBHYTChiTiets.Select(z => z.ThoiDiemKeDon).First() :
                                (donThuocKhongBHYTChiTiets.Any() ? donThuocKhongBHYTChiTiets.Select(z => z.ThoiDiemKeDon).First() : (DateTime?)null),

                        };
                        contentThuocTrongBenhVien = TemplateHelpper.FormatTemplateWithContentTemplate(templateDonThuocTrongBenhVien.Body, data);
                    }

                }
                if (donThuocNgoaiBVs.Any())
                {
                    foreach (var donThuocNgoaiBV in donThuocNgoaiBVs)
                    {
                        var cd =
                             (!string.IsNullOrEmpty(donThuocNgoaiBV.DungSangDisplay)

                                     ? "Sáng " + donThuocNgoaiBV.DungSang +
                                       (!string.IsNullOrEmpty(donThuocNgoaiBV.ThoiGianDungSangDisplay)
                                           ? " " + donThuocNgoaiBV.ThoiGianDungSangDisplay
                                           : "") + " " + donThuocNgoaiBV.DVT + ","
                                     : "") +
                             (!string.IsNullOrEmpty(donThuocNgoaiBV.DungTruaDisplay)

                                     ? "Trưa " + donThuocNgoaiBV.DungTrua +
                                       (!string.IsNullOrEmpty(donThuocNgoaiBV.ThoiGianDungTruaDisplay)
                                           ? " " + donThuocNgoaiBV.ThoiGianDungTruaDisplay
                                           : "") + " " + donThuocNgoaiBV.DVT + ","
                                     : "") +
                             (!string.IsNullOrEmpty(donThuocNgoaiBV.DungChieuDisplay)

                                     ? "Chiều " + donThuocNgoaiBV.DungChieu +
                                       (!string.IsNullOrEmpty(donThuocNgoaiBV.ThoiGianDungChieuDisplay)
                                           ? " " + donThuocNgoaiBV.ThoiGianDungChieuDisplay
                                           : "") + " " + donThuocNgoaiBV.DVT + ","
                                     : "") +
                             (!string.IsNullOrEmpty(donThuocNgoaiBV.DungToiDisplay)
                                     ? "Tối " + donThuocNgoaiBV.DungToi +
                                       (!string.IsNullOrEmpty(donThuocNgoaiBV.ThoiGianDungToiDisplay)
                                           ? " " + donThuocNgoaiBV.ThoiGianDungToiDisplay
                                           : "") + " " + donThuocNgoaiBV.DVT + ","
                                     : "");

                        var cachDung = (!string.IsNullOrEmpty(cd) ? "<i>" + cd.Trim().Remove(cd.Trim().Length - 1) + "<i></br>" : "")
                              + (!string.IsNullOrEmpty(donThuocNgoaiBV.CachDung) ? "<p style='margin:0'><i>" + donThuocNgoaiBV.CachDung + " </i></p>" : "");
                        if (donThuocNgoaiBV.DuocPhamBenhVienPhanNhomChaId == (long)Enums.EnumDuocPhamBenhVienPhanNhom.ThucPhamChucNang
                        || donThuocNgoaiBV.DuocPhamBenhVienPhanNhomChaId == (long)Enums.EnumDuocPhamBenhVienPhanNhom.MyPham
                        || donThuocNgoaiBV.DuocPhamBenhVienPhanNhomChaId == (long)Enums.EnumDuocPhamBenhVienPhanNhom.VatTuYTe
                        || donThuocNgoaiBV.DuocPhamBenhVienPhanNhomChaId == (long)Enums.EnumDuocPhamBenhVienPhanNhom.ThietBiYTe
                        || donThuocNgoaiBV.DuocPhamBenhVienPhanNhomChaId == (long)Enums.EnumDuocPhamBenhVienPhanNhom.ThuocTuDuocLieu)
                        {
                            sttTPCN++;
                            resultThuocThucPhamChucNang += "<tr>";
                            resultThuocThucPhamChucNang += "<td style='vertical-align: top;text-align: center' >" + sttTPCN + "</td>";
                            resultThuocThucPhamChucNang += "<td >" + "<b>" + donThuocNgoaiBV.Ten + "</b>"
                                + (!string.IsNullOrEmpty(cachDung) ? "</br> " + cachDung : "")

                                + "</td>";
                            resultThuocThucPhamChucNang += "<td style='vertical-align: top;text-align: center' >" + FormatSoLuong(donThuocNgoaiBV.SoLuong, donThuocNgoaiBV.LoaiThuocTheoQuanLy) + " " + donThuocNgoaiBV.DVT + "</td>";
                            resultThuocThucPhamChucNang += "</tr>";
                        }
                        else
                        {
                            sttKhongBHYTNgoaiBV++;
                            resultThuocNgoaiBenhVien += "<tr>";
                            resultThuocNgoaiBenhVien += "<td style='vertical-align: top;text-align: center' >" + sttKhongBHYTNgoaiBV + "</td>";
                            resultThuocNgoaiBenhVien += "<td >" + FormatTenDuocPham(donThuocNgoaiBV.Ten, donThuocNgoaiBV.HoatChat, donThuocNgoaiBV.HamLuong, donThuocNgoaiBV.DuocPhamBenhVienPhanNhomId)
                                + (!string.IsNullOrEmpty(cachDung) ? "</br> " + cachDung : "")
                                + "</td>";
                            resultThuocNgoaiBenhVien += "<td style='vertical-align: top;text-align: center' >" + FormatSoLuong(donThuocNgoaiBV.SoLuong, donThuocNgoaiBV.LoaiThuocTheoQuanLy) + " " + donThuocNgoaiBV.DVT + "</td>";
                            resultThuocNgoaiBenhVien += "</tr>";
                        }
                    }
                    if (!string.IsNullOrEmpty(resultThuocNgoaiBenhVien))
                    {
                        resultThuocNgoaiBenhVien = "<style>.thuoc-table{border-top: 1px solid #000;border-right: 1px solid #000;border-spacing: 0;}.thuoc-table td,.thuoc-table th{border-left: 1px solid #000;border-bottom: 1px solid #000;padding: 5px;}</style><table width='100%' class='thuoc-table'><thead><tr><th>STT</th><th>Tên thuốc – Hàm lượng - Liều dùng</th><th>Số lượng</th></tr></thead><tbody>" + resultThuocNgoaiBenhVien + "</tbody></table>";
                        var data = new DataYCKBDonThuoc
                        {
                            Header = headerKhongBHYT,
                            TemplateDonThuoc = resultThuocNgoaiBenhVien,
                            LogoUrl = inToaThuoc.HostingName + "/assets/img/logo-bacha-full.png",
                            BarCodeImgBase64 = !string.IsNullOrEmpty(infoBN.MaTN) ? BarcodeHelper.GenerateBarCode(infoBN.MaTN) : "",
                            MaTN = "<b>Mã TN: </b>" + "<b>" + infoBN.MaTN + "</b>",
                            HoTen = infoBN.HoTen,
                            NamSinhDayDu = infoBN.NamSinhDayDu,
                            Tuoi = infoBN.Tuoi,
                            CMND = infoBN?.CMND,
                            CanNang = infoBN.CanNang,
                            GioiTinh = infoBN?.GioiTinh,
                            DiaChi = infoBN?.DiaChi,
                            SoTheBHYT = infoBN.BHYTMaSoThe,
                            NgayHieuLuc = infoBN.BHYTNgayHieuLuc == null ? "" : (infoBN.BHYTNgayHieuLuc.Value).ApplyFormatDate(),
                            NgayHetHan = infoBN.BHYTNgayHetHan == null ? "" : (infoBN.BHYTNgayHetHan.Value).ApplyFormatDate(),
                            ChuanDoan = infoBN?.ChuanDoan,
                            BacSiKham = tenBacSiKeDon,
                            LoiDan = infoBN.LoiDan,
                            NguoiGiamHo = infoBN?.NguoiGiamHo,
                            MaBN = infoBN.MaBN,
                            SoDienThoai = infoBN.SoDienThoai,
                            SoThang = infoBN.SoThang,
                            CongKhoan = sttKhongBHYTNgoaiBV,
                            //KhoaPhong = khoaPhong,
                            ThoiDiemKeDon = donThuocBHYTChiTiets.Any() ? donThuocBHYTChiTiets.Select(z => z.ThoiDiemKeDon).First() :
                                (donThuocKhongBHYTChiTiets.Any() ? donThuocKhongBHYTChiTiets.Select(z => z.ThoiDiemKeDon).First() : (DateTime?)null),

                        };
                        contentThuocNgoaiBenhVien = TemplateHelpper.FormatTemplateWithContentTemplate(templateDonThuocNgoaiBenhVien.Body, data);
                    }
                }
            }
            if (!string.IsNullOrEmpty(resultThuocThucPhamChucNang))
            {
                resultThuocThucPhamChucNang = "<style>.thuoc-table{border-top: 1px solid #000;border-right: 1px solid #000;border-spacing: 0;}.thuoc-table td,.thuoc-table th{border-left: 1px solid #000;border-bottom: 1px solid #000;padding: 5px;}</style><table width='100%' class='thuoc-table'><thead><tr><th>STT</th><th>Tên sản phẩm – Cách dùng</th><th>Số lượng</th></tr></thead><tbody>" + resultThuocThucPhamChucNang + "</tbody></table>";
                var data = new DataYCKBDonThuoc
                {
                    Header = headerThucPhamChucNang,
                    TemplateDonThuoc = resultThuocThucPhamChucNang,
                    LogoUrl = inToaThuoc.HostingName + "/assets/img/logo-bacha-full.png",
                    BarCodeImgBase64 = !string.IsNullOrEmpty(infoBN.MaTN) ? BarcodeHelper.GenerateBarCode(infoBN.MaTN) : "",
                    MaTN = "<b>Mã TN: </b>" + "<b>" + infoBN.MaTN + "</b>",
                    HoTen = infoBN.HoTen,
                    NamSinhDayDu = infoBN.NamSinhDayDu,
                    Tuoi = infoBN.Tuoi,
                    CanNang = infoBN.CanNang,
                    GioiTinh = infoBN?.GioiTinh,
                    DiaChi = infoBN?.DiaChi,
                    SoTheBHYT = infoBN.BHYTMaSoThe,
                    ChuanDoan = infoBN?.ChuanDoan,
                    BacSiKham = tenBacSiKeDon,
                    LoiDan = infoBN.LoiDan,
                    MaBN = infoBN.MaBN,
                    SoDienThoai = infoBN.SoDienThoai,
                    SoThang = infoBN.SoThang,
                    CongKhoan = sttTPCN,
                    //KhoaPhong = khoaPhong,
                    ThoiDiemKeDon = donThuocBHYTChiTiets.Any() ? donThuocBHYTChiTiets.Select(z => z.ThoiDiemKeDon).First() :
                                (donThuocKhongBHYTChiTiets.Any() ? donThuocKhongBHYTChiTiets.Select(z => z.ThoiDiemKeDon).First() : (DateTime?)null),

                };
                contentThucPhamChucNang = TemplateHelpper.FormatTemplateWithContentTemplate(templateDonThuocThucPhamChucNang.Body, data);
            }

            if (inToaThuoc.VatTu)
            {
                var headerVatTu = "<p style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
                                 "<th>VẬT TƯ Y TẾ</th>" + "</p>";
                var vTYTChiTiets = BaseRepository.TableNoTracking
                       .Include(yckb => yckb.YeuCauTiepNhan)
                       .Include(yckb => yckb.YeuCauKhamBenhDonVTYTs).ThenInclude(ycdt => ycdt.YeuCauKhamBenhDonVTYTChiTiets).ThenInclude(dtct => dtct.NhomVatTu)
                       .Include(yckb => yckb.YeuCauKhamBenhDonVTYTs).ThenInclude(ycdt => ycdt.YeuCauKhamBenhDonVTYTChiTiets).ThenInclude(dtct => dtct.VatTuBenhVien)
                       .SelectMany(yckb => yckb.YeuCauKhamBenhDonVTYTs)
                               .Select(vt => vt)
                               .Where(vt => vt.YeuCauKhamBenhId == inToaThuoc.YeuCauKhamBenhId)
                       .SelectMany(ycvt => ycvt.YeuCauKhamBenhDonVTYTChiTiets).Include(s => s.YeuCauKhamBenhDonVTYT)
                       .Select(vtct => vtct).Include(dtct => dtct.NhomVatTu).Include(dtct => dtct.VatTuBenhVien)
                       .ToList();

                var STT = 0;
                foreach (var item in vTYTChiTiets)
                {
                    STT++;
                    resultVatTu += "<tr>";
                    resultVatTu += "<td style='vertical-align: top;text-align: center'>" + STT + "</td>";
                    resultVatTu += "<td>" + item.Ten
                                + (!string.IsNullOrEmpty(item.GhiChu) ? "</br> <i>" + item.GhiChu + "</i>" : "")
                        + "</td>";
                    resultVatTu += "<td style='vertical-align: top;text-align: center'>" + item.SoLuong + " " + item.DonViTinh + "</td>";
                    //resultVatTu += "<td><i>" + (!string.IsNullOrEmpty(item.GhiChu) ? item.GhiChu : "&nbsp;") + "</i></td>";
                    resultVatTu += "</tr>";
                }
                resultVatTu = "<style>.thuoc-table{border-top: 1px solid #000;border-right: 1px solid #000;border-spacing: 0;}.thuoc-table td,.thuoc-table th{border-left: 1px solid #000;border-bottom: 1px solid #000;padding: 5px;}</style><table width='100%' class='thuoc-table'><thead><tr><th>STT</th><th>TÊN VTYT</th><th>SỐ LƯỢNG</th></tr></thead><tbody>" + resultVatTu + "</tbody></table>";
                if (vTYTChiTiets.Any())
                {
                    var data = new DataYCKBVatTu
                    {
                        MaTN = "<b>Mã TN: </b>" + "<b>" + infoBN.MaTN + "</b>",
                        HoTen = infoBN.HoTen,
                        NamSinhDayDu = infoBN.NamSinhDayDu,
                        Tuoi = infoBN.Tuoi,
                        CanNang = infoBN.CanNang,
                        GioiTinh = infoBN?.GioiTinh,
                        DiaChi = infoBN?.DiaChi,
                        SoTheBHYT = infoBN.BHYTMaSoThe,
                        ChuanDoan = infoBN?.ChuanDoan,
                        BacSiKham = tenBacSiKeDon,
                        MaBN = infoBN.MaBN,
                        SoDienThoai = infoBN.SoDienThoai,
                        CongKhoan = STT,
                        Header = headerVatTu,
                        TemplateVatTu = resultVatTu,
                        LogoUrl = inToaThuoc.HostingName + "/assets/img/logo-bacha-full.png",
                        BarCodeImgBase64 = !string.IsNullOrEmpty(infoBN.MaTN) ? BarcodeHelper.GenerateBarCode(infoBN.MaTN) : "",
                        CMND = infoBN?.CMND,
                        NguoiGiamHo = infoBN?.NguoiGiamHo,
                        ThoiDiemKeDon = vTYTChiTiets.Any() ? vTYTChiTiets.Select(z => z.YeuCauKhamBenhDonVTYT.ThoiDiemKeDon).First() : (DateTime?)null

                    };
                    contentVatTu = TemplateHelpper.FormatTemplateWithContentTemplate(templateVatTuYT.Body, data);
                }
            }
            if (contentThuocBHYT != "")
            {
                contentThuocBHYT = contentThuocBHYT + "<div class=\"pagebreak\"> </div>";
            }
            if (contentThuocTrongBenhVien != "")
            {
                contentThuocTrongBenhVien = contentThuocTrongBenhVien + "<div class=\"pagebreak\"> </div>";
            }
            if (contentThuocNgoaiBenhVien != "")
            {
                contentThuocNgoaiBenhVien = contentThuocNgoaiBenhVien + "<div class=\"pagebreak\"> </div>";
            }
            if (contentThucPhamChucNang != "")
            {
                contentThucPhamChucNang = contentThucPhamChucNang + "<div class=\"pagebreak\"> </div>";
            }
            if (contentVatTu != "")
            {
                contentVatTu = contentVatTu + "<div class=\"pagebreak\"> </div>";
            }
     
            content = contentThuocBHYT + contentThuocTrongBenhVien + contentThuocNgoaiBenhVien + contentThucPhamChucNang + contentVatTu;
            return content;
        }
        #region tách in thuốc BHYT ds lịch sử khám bệnh (BHYT Và Không BHYT)
        public string InToaThuocBHYTVaKhongBHYTDanhSachKhamBenh(InToaThuocKhamBenhDanhSach inToaThuoc)
        {
            var infoBN = ThongTinBenhNhanPhieuThuoc(inToaThuoc.YeuCauTiepNhanId, inToaThuoc.YeuCauKhamBenhId);

            var content = string.Empty;
            var contentThuocBHYT = string.Empty;
            var contentThuocTrongBenhVien = string.Empty;
            var contentThuocNgoaiBenhVien = string.Empty;

            var contentVatTu = string.Empty;
            var resultThuocBHYT = string.Empty;

            var resultThuocTrongBenhVien = string.Empty;
            var resultThuocNgoaiBenhVien = string.Empty;
            var resultVatTu = string.Empty;
            var resultThuocThucPhamChucNang = string.Empty;
            var contentThucPhamChucNang = string.Empty;

            var templateDonThuocBHYT = infoBN.LaTreEm == true ? _templateRepository.TableNoTracking.Where(x => x.Name.Equals("DonThuocBHYTTreEm")).First() : _templateRepository.TableNoTracking.Where(x => x.Name.Equals("DonThuocBHYT")).First();
            var templateDonThuocTrongBenhVien = infoBN.LaTreEm == true ? _templateRepository.TableNoTracking.Where(x => x.Name.Equals("DonThuocKhongBHYTTreEm")).First() : _templateRepository.TableNoTracking.Where(x => x.Name.Equals("DonThuocKhongBHYT")).First();
            var templateDonThuocThucPhamChucNang = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("DonThuocThucPhamChucNang")).FirstOrDefault();
            var templateDonThuocNgoaiBenhVien = infoBN.LaTreEm == true ? _templateRepository.TableNoTracking.Where(x => x.Name.Equals("DonThuocNgoaiBVTreEm")).First() : _templateRepository.TableNoTracking.Where(x => x.Name.Equals("DonThuocNgoaiBV")).First();

            var templateVatTuYT = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("KhamBenhVatTuYTe")).First();

            //Thuốc trong BV
            var donThuocBHYTChiTiets = _yeuCauKhamBenhDonThuocChiTietRepository.TableNoTracking
                                .Where(z => z.YeuCauKhamBenhDonThuoc.YeuCauKhamBenhId == inToaThuoc.YeuCauKhamBenhId && z.YeuCauKhamBenhDonThuoc.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT)
                                .Select(s => new InNoiTruDonThuocChiTietVo
                                {
                                    STT = s.SoThuTu,
                                    Id = s.Id,
                                    Ten = s.Ten,
                                    MaHoatChat = s.MaHoatChat,
                                    HoatChat = s.HoatChat,
                                    HamLuong = s.HamLuong,
                                    TenDuongDung = s.DuongDung.Ten,
                                    DVT = s.DonViTinh.Ten,
                                    SoLuong = s.SoLuong,
                                    SoNgayDung = s.SoNgayDung,
                                    ThoiGianDungSang = s.ThoiGianDungSang,
                                    ThoiGianDungTrua = s.ThoiGianDungTrua,
                                    ThoiGianDungChieu = s.ThoiGianDungChieu,
                                    ThoiGianDungToi = s.ThoiGianDungToi,
                                    // thuốc gây nghiện,hướng thần thì cách dùng con số chuyển thành text , ngược lại nếu thuống thường kiểm tra sl kê nhỏ hơn 10 thì thêm 0 phía trước , còn lại bình thường
                                    DungSang = s.DungSang,
                                    DungTrua = s.DungTrua,
                                    DungChieu = s.DungChieu,
                                    DungToi = s.DungToi,
                                    ThoiDiemKeDon = s.YeuCauKhamBenhDonThuoc.ThoiDiemKeDon,
                                    GhiChu = s.YeuCauKhamBenhDonThuoc.GhiChu,
                                    CachDung = s.GhiChu,
                                    LaDuocPhamBenhVien = s.LaDuocPhamBenhVien,
                                    TenBacSiKeDon = s.YeuCauKhamBenhDonThuoc.BacSiKeDon.User.HoTen,
                                    BacSiKeDonId = s.YeuCauKhamBenhDonThuoc.BacSiKeDonId,
                                    LoaiThuocTheoQuanLy = s.DuocPham.DuocPhamBenhVien.LoaiThuocTheoQuanLy,
                                    DuocPhamBenhVienPhanNhomId = s.DuocPham.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId,
                                    DuongDungId = s.DuongDungId
                                }).OrderBy(p => p.STT ?? 0).ToList();

            //var donThuocBHYTsDoc = donThuocBHYTChiTiets.Where(z =>
            //                                                                                      z.DuongDungId != Constants.DuongDungIdSapXep.Tiem
            //                                                                                   && z.DuongDungId != Constants.DuongDungIdSapXep.Uong
            //                                                                                   && z.DuongDungId != Constants.DuongDungIdSapXep.Dat
            //                                                                                   && z.DuongDungId != Constants.DuongDungIdSapXep.DungNgoai
            //                                                                                   && z.LaDuocPhamBenhVien
            //                                                                                   && z.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.ThuocDoc).ToList();
            //var donThuocBHYTsTiem = donThuocBHYTChiTiets.Where(z => z.DuongDungId == Constants.DuongDungIdSapXep.Tiem
            //                                                             && z.LaDuocPhamBenhVien
            //                                                             && z.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.ThuocDoc).ToList();
            //var donThuocBHYTsUong = donThuocBHYTChiTiets.Where(z => z.DuongDungId == Constants.DuongDungIdSapXep.Uong
            //                                                             && z.LaDuocPhamBenhVien
            //                                                              && z.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.ThuocDoc).ToList();
            //var donThuocBHYTsDat = donThuocBHYTChiTiets.Where(z => z.DuongDungId == Constants.DuongDungIdSapXep.Dat
            //                                                            && z.LaDuocPhamBenhVien
            //                                                             && z.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.ThuocDoc).ToList();
            //var donThuocBHYTsDungNgoai = donThuocBHYTChiTiets.Where(z => z.DuongDungId == Constants.DuongDungIdSapXep.DungNgoai
            //                                                                  && z.LaDuocPhamBenhVien
            //                                                                   && z.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.ThuocDoc).ToList();

            //var donThuocBHYTsKhac = donThuocBHYTChiTiets.Where(z => z.DuongDungId != Constants.DuongDungIdSapXep.Tiem
            //                                                                                   && z.DuongDungId != Constants.DuongDungIdSapXep.Uong
            //                                                                                   && z.DuongDungId != Constants.DuongDungIdSapXep.Dat
            //                                                                                   && z.DuongDungId != Constants.DuongDungIdSapXep.DungNgoai
            //                                                                                   && z.LaDuocPhamBenhVien
            //                                                                                 && z.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.ThuocDoc
            //                                                                                 ).ToList();
            //var donThuocBHYTs = donThuocBHYTsDoc
            //               .Concat(donThuocBHYTsTiem)
            //               .Concat(donThuocBHYTsUong)
            //               .Concat(donThuocBHYTsDat)
            //               .Concat(donThuocBHYTsDungNgoai)
            //               .Concat(donThuocBHYTsKhac);
            var donThuocBHYTs = donThuocBHYTChiTiets;

            var donThuocKhongBHYTChiTiets = _yeuCauKhamBenhDonThuocChiTietRepository.TableNoTracking
                                .Where(z => z.YeuCauKhamBenhDonThuoc.YeuCauKhamBenhId == inToaThuoc.YeuCauKhamBenhId && z.YeuCauKhamBenhDonThuoc.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocKhongBHYT)
                                .Select(s => new InNoiTruDonThuocChiTietVo
                                {
                                    STT = s.SoThuTu,
                                    Id = s.Id,
                                    Ten = s.Ten,
                                    MaHoatChat = s.MaHoatChat,
                                    HoatChat = s.HoatChat,
                                    HamLuong = s.HamLuong,
                                    TenDuongDung = s.DuongDung.Ten,
                                    DVT = s.DonViTinh.Ten,
                                    SoLuong = s.SoLuong,
                                    SoNgayDung = s.SoNgayDung,
                                    ThoiGianDungSang = s.ThoiGianDungSang,
                                    ThoiGianDungTrua = s.ThoiGianDungTrua,
                                    ThoiGianDungChieu = s.ThoiGianDungChieu,
                                    ThoiGianDungToi = s.ThoiGianDungToi,
                                    DungSang = s.DungSang,
                                    DungTrua = s.DungTrua,
                                    DungChieu = s.DungChieu,
                                    DungToi = s.DungToi,
                                    ThoiDiemKeDon = s.YeuCauKhamBenhDonThuoc.ThoiDiemKeDon,
                                    GhiChu = s.YeuCauKhamBenhDonThuoc.GhiChu,
                                    CachDung = s.GhiChu,
                                    LaDuocPhamBenhVien = s.LaDuocPhamBenhVien,
                                    TenBacSiKeDon = s.YeuCauKhamBenhDonThuoc.BacSiKeDon.User.HoTen,
                                    BacSiKeDonId = s.YeuCauKhamBenhDonThuoc.BacSiKeDonId,
                                    LoaiThuocTheoQuanLy = s.DuocPham.DuocPhamBenhVien.LoaiThuocTheoQuanLy,
                                    DuocPhamBenhVienPhanNhomId = s.DuocPham.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId,
                                    DuongDungId = s.DuongDungId
                                }).ToList();
            var duocPhamBenhVienPhanNhoms = _duocPhamBenhVienPhanNhomRepository.TableNoTracking.ToList();

            foreach (var thuoc in donThuocKhongBHYTChiTiets)
            {
                thuoc.DuocPhamBenhVienPhanNhomChaId = CalculateHelper.GetDuocPhamBenhVienPhanNhomCha(thuoc.DuocPhamBenhVienPhanNhomId.GetValueOrDefault(), duocPhamBenhVienPhanNhoms);
            }

            var userCurrentId = donThuocBHYTChiTiets.Any() ? donThuocBHYTChiTiets.First().BacSiKeDonId : (donThuocKhongBHYTChiTiets.Any() ? donThuocKhongBHYTChiTiets.First().BacSiKeDonId : 0);

            var tenBacSiKeDon = _userRepository.TableNoTracking
                             .Where(u => u.Id == userCurrentId).Select(u =>
                             (u.NhanVien.HocHamHocVi != null ? u.NhanVien.HocHamHocVi.Ma + " " : "")
                           //+ (u.NhanVien.ChucDanh != null ? u.NhanVien.ChucDanh.NhomChucDanh.Ma + "." : "")
                           + u.HoTen).FirstOrDefault();
            //var donThuocTrongBVKhongBHYTsDoc = donThuocKhongBHYTChiTiets.Where(z =>
            //                                                                                     z.DuongDungId != Constants.DuongDungIdSapXep.Tiem
            //                                                                                  && z.DuongDungId != Constants.DuongDungIdSapXep.Uong
            //                                                                                  && z.DuongDungId != Constants.DuongDungIdSapXep.Dat
            //                                                                                  && z.DuongDungId != Constants.DuongDungIdSapXep.DungNgoai
            //                                                                                  && z.LaDuocPhamBenhVien
            //                                                                                  && z.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.ThuocDoc).ToList();
            //var donThuocTrongBVKhongBHYTsTiem = donThuocKhongBHYTChiTiets.Where(z => z.DuongDungId == Constants.DuongDungIdSapXep.Tiem
            //                                                              && z.LaDuocPhamBenhVien
            //                                                              && z.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.ThuocDoc).ToList();
            //var donThuocTrongBVKhongBHYTsUong = donThuocKhongBHYTChiTiets.Where(z => z.DuongDungId == Constants.DuongDungIdSapXep.Uong
            //                                                             && z.LaDuocPhamBenhVien
            //                                                             && z.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.ThuocDoc).ToList();
            //var donThuocTrongBVKhongBHYTsDat = donThuocKhongBHYTChiTiets.Where(z => z.DuongDungId == Constants.DuongDungIdSapXep.Dat
            //                                                            && z.LaDuocPhamBenhVien
            //                                                            && z.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.ThuocDoc).ToList();
            //var donThuocTrongBVKhongBHYTsDungNgoai = donThuocKhongBHYTChiTiets.Where(z => z.DuongDungId == Constants.DuongDungIdSapXep.DungNgoai
            //                                                                  && z.LaDuocPhamBenhVien
            //                                                                  && z.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.ThuocDoc).ToList();

            //var donThuocTrongBVKhongBHYTsKhac = donThuocKhongBHYTChiTiets.Where(z => z.DuongDungId != Constants.DuongDungIdSapXep.Tiem
            //                                                                                   && z.DuongDungId != Constants.DuongDungIdSapXep.Uong
            //                                                                                   && z.DuongDungId != Constants.DuongDungIdSapXep.Dat
            //                                                                                   && z.DuongDungId != Constants.DuongDungIdSapXep.DungNgoai
            //                                                                                   && z.LaDuocPhamBenhVien
            //                                                                                 && z.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.ThuocDoc
            //                                                                                 ).ToList();
            //var donThuocTrongBVs = donThuocTrongBVKhongBHYTsDoc
            //               .Concat(donThuocTrongBVKhongBHYTsTiem)
            //               .Concat(donThuocTrongBVKhongBHYTsUong)
            //               .Concat(donThuocTrongBVKhongBHYTsDat)
            //               .Concat(donThuocTrongBVKhongBHYTsDungNgoai)
            //               .Concat(donThuocTrongBVKhongBHYTsKhac);
            var donThuocTrongBVs = donThuocKhongBHYTChiTiets.Where(z => z.LaDuocPhamBenhVien).OrderBy(z => z.STT).ToList();

            //var donThuocNgoaiBVsDoc = donThuocKhongBHYTChiTiets.Where(z =>
            //                                                                                     z.DuongDungId != Constants.DuongDungIdSapXep.Tiem
            //                                                                                  && z.DuongDungId != Constants.DuongDungIdSapXep.Uong
            //                                                                                  && z.DuongDungId != Constants.DuongDungIdSapXep.Dat
            //                                                                                  && z.DuongDungId != Constants.DuongDungIdSapXep.DungNgoai
            //                                                                                  && z.LaDuocPhamBenhVien
            //                                                                                  && z.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.ThuocDoc).ToList();
            //var donThuocNgoaiBVsTiem = donThuocKhongBHYTChiTiets.Where(z => z.DuongDungId == Constants.DuongDungIdSapXep.Tiem
            //                                                              && !z.LaDuocPhamBenhVien
            //                                                              && z.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.ThuocDoc).ToList();
            //var donThuocNgoaiBVsUong = donThuocKhongBHYTChiTiets.Where(z => z.DuongDungId == Constants.DuongDungIdSapXep.Uong
            //                                                             && !z.LaDuocPhamBenhVien
            //                                                             && z.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.ThuocDoc).ToList();
            //var donThuocNgoaiBVsDat = donThuocKhongBHYTChiTiets.Where(z => z.DuongDungId == Constants.DuongDungIdSapXep.Dat
            //                                                            && !z.LaDuocPhamBenhVien
            //                                                            && z.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.ThuocDoc).ToList();
            //var donThuocNgoaiBVsDungNgoai = donThuocKhongBHYTChiTiets.Where(z => z.DuongDungId == Constants.DuongDungIdSapXep.DungNgoai
            //                                                                  && !z.LaDuocPhamBenhVien
            //                                                                  && z.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.ThuocDoc).ToList();

            //var donThuocNgoaiBVsKhac = donThuocKhongBHYTChiTiets.Where(z => z.DuongDungId != Constants.DuongDungIdSapXep.Tiem
            //                                                                                   && z.DuongDungId != Constants.DuongDungIdSapXep.Uong
            //                                                                                   && z.DuongDungId != Constants.DuongDungIdSapXep.Dat
            //                                                                                   && z.DuongDungId != Constants.DuongDungIdSapXep.DungNgoai
            //                                                                                   && !z.LaDuocPhamBenhVien
            //                                                                                 && z.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.ThuocDoc
            //                                                                                 ).ToList();
            //var donThuocNgoaiBVs = donThuocNgoaiBVsDoc
            //               .Concat(donThuocNgoaiBVsTiem)
            //               .Concat(donThuocNgoaiBVsUong)
            //               .Concat(donThuocNgoaiBVsDat)
            //               .Concat(donThuocNgoaiBVsDungNgoai)
            //               .Concat(donThuocNgoaiBVsKhac);
            var donThuocNgoaiBVs = donThuocKhongBHYTChiTiets.Where(z => !z.LaDuocPhamBenhVien).OrderBy(z => z.STT).ToList();


            var headerBHYT = string.Empty;
            var headerKhongBHYT = string.Empty;
            var headerThuocNgoaiBV = string.Empty;
            var headerThucPhamChucNang = string.Empty;

            //if (inToaThuoc.Header)
            //{
            //    headerBHYT = "<p  style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
            //                  "<th>TOA THUỐC BẢO HIỂM Y TẾ</th>" +
            //             "</p>";
            //    headerKhongBHYT = "<p  style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
            //                  "<th>TOA THUỐC KHÔNG BẢO HIỂM Y TẾ</th>" +
            //             "</p>";

            //    headerThuocNgoaiBV = "<p  style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
            //                 "<th>TOA THUỐC NGOÀI BỆNH VIỆN</ th>" +
            //                 "</p>";

            //    headerThucPhamChucNang = "<p  style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
            //                     "<th>ĐƠN TƯ VẤN</ th>" +
            //                     "</p>";
            //}
            var sttBHYT = 0;

            var sttKhongBHYTTrongBV = 0;
            var sttKhongBHYTNgoaiBV = 0;
            var sttTPCN = 0;
            if (donThuocBHYTs.Any())
            {
                foreach (var donThuocBHYTChiTiet in donThuocBHYTs)
                {
                    var cd =
                             (!string.IsNullOrEmpty(donThuocBHYTChiTiet.DungSangDisplay)
                                 ? "Sáng " + donThuocBHYTChiTiet.DungSang
                                 +
                                   (!string.IsNullOrEmpty(donThuocBHYTChiTiet.ThoiGianDungSangDisplay)
                                       ? " " + donThuocBHYTChiTiet.ThoiGianDungSangDisplay
                                       : "") + " " + donThuocBHYTChiTiet.DVT + ","
                                 : "") +
                             (!string.IsNullOrEmpty(donThuocBHYTChiTiet.DungTruaDisplay)
                                 ? "Trưa " + donThuocBHYTChiTiet.DungTrua +
                                   (!string.IsNullOrEmpty(donThuocBHYTChiTiet.ThoiGianDungTruaDisplay)
                                       ? " " + donThuocBHYTChiTiet.ThoiGianDungTruaDisplay
                                       : "") + " " + donThuocBHYTChiTiet.DVT + ","
                                 : "") +
                             (!string.IsNullOrEmpty(donThuocBHYTChiTiet.DungChieuDisplay)
                                 ? "Chiều " + donThuocBHYTChiTiet.DungChieu +
                                   (!string.IsNullOrEmpty(donThuocBHYTChiTiet.ThoiGianDungChieuDisplay)
                                       ? " " + donThuocBHYTChiTiet.ThoiGianDungChieuDisplay
                                       : "") + " " + donThuocBHYTChiTiet.DVT + ","
                                 : "") +
                            (!string.IsNullOrEmpty(donThuocBHYTChiTiet.DungToiDisplay)
                                 ? "Tối " + donThuocBHYTChiTiet.DungToi +
                                   (!string.IsNullOrEmpty(donThuocBHYTChiTiet.ThoiGianDungToiDisplay)
                                       ? " " + donThuocBHYTChiTiet.ThoiGianDungToiDisplay
                                       : "") + " " + donThuocBHYTChiTiet.DVT + ","
                                 : "");

                    var cachDung = (!string.IsNullOrEmpty(cd) ? "<i>" + cd.Trim().Remove(cd.Trim().Length - 1) + "<i></br>" : "")
                                 + (!string.IsNullOrEmpty(donThuocBHYTChiTiet.CachDung) ? "<p style='margin:0'><i>" + donThuocBHYTChiTiet.CachDung + " </i></p>" : "");
                    sttBHYT++;
                    resultThuocBHYT += "<tr>";
                    resultThuocBHYT += "<td style='vertical-align: top; text-align: center' >" + sttBHYT + "</td>";
                    resultThuocBHYT += "<td >" + FormatTenDuocPham(donThuocBHYTChiTiet.Ten, donThuocBHYTChiTiet.HoatChat, donThuocBHYTChiTiet.HamLuong, donThuocBHYTChiTiet.DuocPhamBenhVienPhanNhomId)
                        + (!string.IsNullOrEmpty(cachDung) ? "</br> " + cachDung : "")
                        + "</td>";

                    resultThuocBHYT += "<td  style='vertical-align: top;text-align: center' >" + FormatSoLuong(donThuocBHYTChiTiet.SoLuong, donThuocBHYTChiTiet.LoaiThuocTheoQuanLy) + " " + donThuocBHYTChiTiet.DVT + "</td>";
                    resultThuocBHYT += "</tr>";
                }

                if (!string.IsNullOrEmpty(resultThuocBHYT))
                {
                    resultThuocBHYT = "<style>.thuoc-table{border-top: 1px solid #000;border-right: 1px solid #000;border-spacing: 0;}.thuoc-table td,.thuoc-table th{border-left: 1px solid #000;border-bottom: 1px solid #000;padding: 5px;}</style><table width='100%' class='thuoc-table'><thead><tr><th>STT</th><th>Tên thuốc – Hàm lượng - Liều dùng</th><th>Số lượng</th></tr></thead><tbody>" + resultThuocBHYT + "</tbody></table>";
                    var data = new DataYCKBDonThuoc
                    {
                        Header = headerBHYT,
                        TemplateDonThuoc = resultThuocBHYT,
                        LogoUrl = inToaThuoc.HostingName + "/assets/img/logo-bacha-full.png",
                        BarCodeImgBase64 = !string.IsNullOrEmpty(infoBN.MaTN) ? BarcodeHelper.GenerateBarCode(infoBN.MaTN) : "",
                        MaTN = "<b>Mã TN: </b>" + "<b>" + infoBN.MaTN + "</b>",
                        HoTen = infoBN.HoTen,
                        Tuoi = infoBN.Tuoi,
                        NamSinhDayDu = infoBN.NamSinhDayDu,
                        CanNang = infoBN.CanNang,
                        GioiTinh = infoBN?.GioiTinh,
                        DiaChi = infoBN?.DiaChi,
                        CMND = infoBN?.CMND,
                        SoTheBHYT = infoBN.BHYTMaSoThe,
                        NgayHieuLuc = infoBN.BHYTNgayHieuLuc == null ? "" : (infoBN.BHYTNgayHieuLuc.Value).ApplyFormatDate(),
                        NgayHetHan = infoBN.BHYTNgayHetHan == null ? "" : (infoBN.BHYTNgayHetHan.Value).ApplyFormatDate(),
                        ChuanDoan = infoBN?.ChuanDoan,
                        ThoiDiemKeDon = donThuocBHYTChiTiets.Any() ? donThuocBHYTChiTiets.Select(z => z.ThoiDiemKeDon).First() :
                                (donThuocKhongBHYTChiTiets.Any() ? donThuocKhongBHYTChiTiets.Select(z => z.ThoiDiemKeDon).First() : (DateTime?)null),
                        BacSiKham = tenBacSiKeDon,
                        LoiDan = infoBN.LoiDan,
                        MaBN = infoBN.MaBN,
                        SoDienThoai = infoBN.SoDienThoai,
                        SoThang = infoBN.SoThang,
                        CongKhoan = sttBHYT,
                        //KhoaPhong = khoaPhong
                    };
                    contentThuocBHYT = TemplateHelpper.FormatTemplateWithContentTemplate(templateDonThuocBHYT.Body, data);
                }

            }

            if (donThuocKhongBHYTChiTiets.Any())
            {
                if (donThuocTrongBVs.Any())
                {
                    foreach (var donThuocTrongBV in donThuocTrongBVs)
                    {
                        var cd =
                             (!string.IsNullOrEmpty(donThuocTrongBV.DungSangDisplay)

                                     ? "Sáng " + donThuocTrongBV.DungSang +
                                       (!string.IsNullOrEmpty(donThuocTrongBV.ThoiGianDungSangDisplay)
                                           ? " " + donThuocTrongBV.ThoiGianDungSangDisplay
                                           : "") + " " + donThuocTrongBV.DVT + ","
                                     : "") +
                             (!string.IsNullOrEmpty(donThuocTrongBV.DungTruaDisplay)

                                     ? "Trưa " + donThuocTrongBV.DungTrua +
                                       (!string.IsNullOrEmpty(donThuocTrongBV.ThoiGianDungTruaDisplay)
                                           ? " " + donThuocTrongBV.ThoiGianDungTruaDisplay
                                           : "") + " " + donThuocTrongBV.DVT + ","
                                     : "") +
                             (!string.IsNullOrEmpty(donThuocTrongBV.DungChieuDisplay)

                                     ? "Chiều " + donThuocTrongBV.DungChieu +
                                       (!string.IsNullOrEmpty(donThuocTrongBV.ThoiGianDungChieuDisplay)
                                           ? " " + donThuocTrongBV.ThoiGianDungChieuDisplay
                                           : "") + " " + donThuocTrongBV.DVT + ","
                                     : "") +
                             (!string.IsNullOrEmpty(donThuocTrongBV.DungToiDisplay)

                                     ? "Tối " + donThuocTrongBV.DungToi +
                                       (!string.IsNullOrEmpty(donThuocTrongBV.ThoiGianDungToiDisplay)
                                           ? " " + donThuocTrongBV.ThoiGianDungToiDisplay
                                           : "") + " " + donThuocTrongBV.DVT + ","
                                     : "");

                        var cachDung = (!string.IsNullOrEmpty(cd) ? "<i>" + cd.Trim().Remove(cd.Trim().Length - 1) + "<i></br>" : "")
                               + (!string.IsNullOrEmpty(donThuocTrongBV.CachDung) ? "<p style='margin:0'><i>" + donThuocTrongBV.CachDung + " </i></p>" : "");
                        if (donThuocTrongBV.DuocPhamBenhVienPhanNhomChaId == (long)Enums.EnumDuocPhamBenhVienPhanNhom.ThucPhamChucNang
                             || donThuocTrongBV.DuocPhamBenhVienPhanNhomChaId == (long)Enums.EnumDuocPhamBenhVienPhanNhom.MyPham
                             || donThuocTrongBV.DuocPhamBenhVienPhanNhomChaId == (long)Enums.EnumDuocPhamBenhVienPhanNhom.VatTuYTe
                             || donThuocTrongBV.DuocPhamBenhVienPhanNhomChaId == (long)Enums.EnumDuocPhamBenhVienPhanNhom.ThietBiYTe
                             || donThuocTrongBV.DuocPhamBenhVienPhanNhomChaId == (long)Enums.EnumDuocPhamBenhVienPhanNhom.ThuocTuDuocLieu)
                        {
                            sttTPCN++;
                            resultThuocThucPhamChucNang += "<tr>";
                            resultThuocThucPhamChucNang += "<td style='vertical-align: top;text-align: center' >" + sttTPCN + "</td>";
                            resultThuocThucPhamChucNang += "<td >" + "<b>" + donThuocTrongBV.Ten + "</b>"
                             + (!string.IsNullOrEmpty(cachDung) ? "</br> " + cachDung : "")
                                + "</td>";
                            resultThuocThucPhamChucNang += "<td style='vertical-align: top;text-align: center' >" + FormatSoLuong(donThuocTrongBV.SoLuong, donThuocTrongBV.LoaiThuocTheoQuanLy) + " " + donThuocTrongBV.DVT + "</td>";
                            resultThuocThucPhamChucNang += "</tr>";


                        }
                        else
                        {
                            sttKhongBHYTTrongBV++;
                            resultThuocTrongBenhVien += "<tr>";
                            resultThuocTrongBenhVien += "<td style='vertical-align: top;text-align: center' >" + sttKhongBHYTTrongBV + "</td>";
                            resultThuocTrongBenhVien += "<td >" + FormatTenDuocPham(donThuocTrongBV.Ten, donThuocTrongBV.HoatChat, donThuocTrongBV.HamLuong, donThuocTrongBV.DuocPhamBenhVienPhanNhomId)
                                 + (!string.IsNullOrEmpty(cachDung) ? "</br> " + cachDung : "")
                                + "</td>";
                            resultThuocTrongBenhVien += "<td style='vertical-align: top;text-align: center'  >" + FormatSoLuong(donThuocTrongBV.SoLuong, donThuocTrongBV.LoaiThuocTheoQuanLy) + " " + donThuocTrongBV.DVT + "</td>";
                            resultThuocTrongBenhVien += "</tr>";
                        }
                    }
                    if (!string.IsNullOrEmpty(resultThuocTrongBenhVien))
                    {
                        resultThuocTrongBenhVien = "<style>.thuoc-table{border-top: 1px solid #000;border-right: 1px solid #000;border-spacing: 0;}.thuoc-table td,.thuoc-table th{border-left: 1px solid #000;border-bottom: 1px solid #000;padding: 5px;}</style><table width='100%' class='thuoc-table'><thead><tr><th>STT</th><th>Tên thuốc – Hàm lượng - Liều dùng</th><th>Số lượng</th></tr></thead><tbody>" + resultThuocTrongBenhVien + "</tbody></table>";
                        var data = new DataYCKBDonThuoc
                        {
                            Header = headerKhongBHYT,
                            TemplateDonThuoc = resultThuocTrongBenhVien,
                            LogoUrl = inToaThuoc.HostingName + "/assets/img/logo-bacha-full.png",
                            BarCodeImgBase64 = !string.IsNullOrEmpty(infoBN.MaTN) ? BarcodeHelper.GenerateBarCode(infoBN.MaTN) : "",
                            MaTN = "<b>Mã TN: </b>" + "<b>" + infoBN.MaTN + "</b>",
                            HoTen = infoBN.HoTen,
                            NamSinhDayDu = infoBN.NamSinhDayDu,

                            Tuoi = infoBN.Tuoi,
                            CMND = infoBN?.CMND,
                            CanNang = infoBN.CanNang,
                            GioiTinh = infoBN?.GioiTinh,
                            DiaChi = infoBN?.DiaChi,
                            SoTheBHYT = infoBN.BHYTMaSoThe,
                            NgayHieuLuc = infoBN.BHYTNgayHieuLuc == null ? "" : (infoBN.BHYTNgayHieuLuc.Value).ApplyFormatDate(),
                            NgayHetHan = infoBN.BHYTNgayHetHan == null ? "" : (infoBN.BHYTNgayHetHan.Value).ApplyFormatDate(),
                            ChuanDoan = infoBN?.ChuanDoan,
                            BacSiKham = tenBacSiKeDon,
                            LoiDan = infoBN.LoiDan,
                            NguoiGiamHo = infoBN?.NguoiGiamHo,
                            MaBN = infoBN.MaBN,
                            SoDienThoai = infoBN.SoDienThoai,
                            SoThang = infoBN.SoThang,
                            CongKhoan = sttKhongBHYTTrongBV,
                            //KhoaPhong = khoaPhong,
                            ThoiDiemKeDon = donThuocBHYTChiTiets.Any() ? donThuocBHYTChiTiets.Select(z => z.ThoiDiemKeDon).First() :
                                (donThuocKhongBHYTChiTiets.Any() ? donThuocKhongBHYTChiTiets.Select(z => z.ThoiDiemKeDon).First() : (DateTime?)null),

                        };
                        contentThuocTrongBenhVien = TemplateHelpper.FormatTemplateWithContentTemplate(templateDonThuocTrongBenhVien.Body, data);
                    }

                }
                if (donThuocNgoaiBVs.Any())
                {
                    foreach (var donThuocNgoaiBV in donThuocNgoaiBVs)
                    {
                        var cd =
                             (!string.IsNullOrEmpty(donThuocNgoaiBV.DungSangDisplay)

                                     ? "Sáng " + donThuocNgoaiBV.DungSang +
                                       (!string.IsNullOrEmpty(donThuocNgoaiBV.ThoiGianDungSangDisplay)
                                           ? " " + donThuocNgoaiBV.ThoiGianDungSangDisplay
                                           : "") + " " + donThuocNgoaiBV.DVT + ","
                                     : "") +
                             (!string.IsNullOrEmpty(donThuocNgoaiBV.DungTruaDisplay)

                                     ? "Trưa " + donThuocNgoaiBV.DungTrua +
                                       (!string.IsNullOrEmpty(donThuocNgoaiBV.ThoiGianDungTruaDisplay)
                                           ? " " + donThuocNgoaiBV.ThoiGianDungTruaDisplay
                                           : "") + " " + donThuocNgoaiBV.DVT + ","
                                     : "") +
                             (!string.IsNullOrEmpty(donThuocNgoaiBV.DungChieuDisplay)

                                     ? "Chiều " + donThuocNgoaiBV.DungChieu +
                                       (!string.IsNullOrEmpty(donThuocNgoaiBV.ThoiGianDungChieuDisplay)
                                           ? " " + donThuocNgoaiBV.ThoiGianDungChieuDisplay
                                           : "") + " " + donThuocNgoaiBV.DVT + ","
                                     : "") +
                             (!string.IsNullOrEmpty(donThuocNgoaiBV.DungToiDisplay)
                                     ? "Tối " + donThuocNgoaiBV.DungToi +
                                       (!string.IsNullOrEmpty(donThuocNgoaiBV.ThoiGianDungToiDisplay)
                                           ? " " + donThuocNgoaiBV.ThoiGianDungToiDisplay
                                           : "") + " " + donThuocNgoaiBV.DVT + ","
                                     : "");

                        var cachDung = (!string.IsNullOrEmpty(cd) ? "<i>" + cd.Trim().Remove(cd.Trim().Length - 1) + "<i></br>" : "")
                              + (!string.IsNullOrEmpty(donThuocNgoaiBV.CachDung) ? "<p style='margin:0'><i>" + donThuocNgoaiBV.CachDung + " </i></p>" : "");
                        if (donThuocNgoaiBV.DuocPhamBenhVienPhanNhomChaId == (long)Enums.EnumDuocPhamBenhVienPhanNhom.ThucPhamChucNang
                        || donThuocNgoaiBV.DuocPhamBenhVienPhanNhomChaId == (long)Enums.EnumDuocPhamBenhVienPhanNhom.MyPham
                        || donThuocNgoaiBV.DuocPhamBenhVienPhanNhomChaId == (long)Enums.EnumDuocPhamBenhVienPhanNhom.VatTuYTe
                        || donThuocNgoaiBV.DuocPhamBenhVienPhanNhomChaId == (long)Enums.EnumDuocPhamBenhVienPhanNhom.ThietBiYTe
                        || donThuocNgoaiBV.DuocPhamBenhVienPhanNhomChaId == (long)Enums.EnumDuocPhamBenhVienPhanNhom.ThuocTuDuocLieu)
                        {
                            sttTPCN++;
                            resultThuocThucPhamChucNang += "<tr>";
                            resultThuocThucPhamChucNang += "<td style='vertical-align: top;text-align: center' >" + sttTPCN + "</td>";
                            resultThuocThucPhamChucNang += "<td >" + "<b>" + donThuocNgoaiBV.Ten + "</b>"
                                + (!string.IsNullOrEmpty(cachDung) ? "</br> " + cachDung : "")

                                + "</td>";
                            resultThuocThucPhamChucNang += "<td style='vertical-align: top;text-align: center' >" + FormatSoLuong(donThuocNgoaiBV.SoLuong, donThuocNgoaiBV.LoaiThuocTheoQuanLy) + " " + donThuocNgoaiBV.DVT + "</td>";
                            resultThuocThucPhamChucNang += "</tr>";
                        }
                        else
                        {
                            sttKhongBHYTNgoaiBV++;
                            resultThuocNgoaiBenhVien += "<tr>";
                            resultThuocNgoaiBenhVien += "<td style='vertical-align: top;text-align: center' >" + sttKhongBHYTNgoaiBV + "</td>";
                            resultThuocNgoaiBenhVien += "<td >" + FormatTenDuocPham(donThuocNgoaiBV.Ten, donThuocNgoaiBV.HoatChat, donThuocNgoaiBV.HamLuong, donThuocNgoaiBV.DuocPhamBenhVienPhanNhomId)
                                + (!string.IsNullOrEmpty(cachDung) ? "</br> " + cachDung : "")
                                + "</td>";
                            resultThuocNgoaiBenhVien += "<td style='vertical-align: top;text-align: center' >" + FormatSoLuong(donThuocNgoaiBV.SoLuong, donThuocNgoaiBV.LoaiThuocTheoQuanLy) + " " + donThuocNgoaiBV.DVT + "</td>";
                            resultThuocNgoaiBenhVien += "</tr>";
                        }
                    }
                    if (!string.IsNullOrEmpty(resultThuocNgoaiBenhVien))
                    {
                        resultThuocNgoaiBenhVien = "<style>.thuoc-table{border-top: 1px solid #000;border-right: 1px solid #000;border-spacing: 0;}.thuoc-table td,.thuoc-table th{border-left: 1px solid #000;border-bottom: 1px solid #000;padding: 5px;}</style><table width='100%' class='thuoc-table'><thead><tr><th>STT</th><th>Tên thuốc – Hàm lượng - Liều dùng</th><th>Số lượng</th></tr></thead><tbody>" + resultThuocNgoaiBenhVien + "</tbody></table>";
                        var data = new DataYCKBDonThuoc
                        {
                            Header = headerKhongBHYT,
                            TemplateDonThuoc = resultThuocNgoaiBenhVien,
                            LogoUrl = inToaThuoc.HostingName + "/assets/img/logo-bacha-full.png",
                            BarCodeImgBase64 = !string.IsNullOrEmpty(infoBN.MaTN) ? BarcodeHelper.GenerateBarCode(infoBN.MaTN) : "",
                            MaTN = "<b>Mã TN: </b>" + "<b>" + infoBN.MaTN + "</b>",
                            HoTen = infoBN.HoTen,
                            NamSinhDayDu = infoBN.NamSinhDayDu,
                            Tuoi = infoBN.Tuoi,
                            CMND = infoBN?.CMND,
                            CanNang = infoBN.CanNang,
                            GioiTinh = infoBN?.GioiTinh,
                            DiaChi = infoBN?.DiaChi,
                            SoTheBHYT = infoBN.BHYTMaSoThe,
                            NgayHieuLuc = infoBN.BHYTNgayHieuLuc == null ? "" : (infoBN.BHYTNgayHieuLuc.Value).ApplyFormatDate(),
                            NgayHetHan = infoBN.BHYTNgayHetHan == null ? "" : (infoBN.BHYTNgayHetHan.Value).ApplyFormatDate(),
                            ChuanDoan = infoBN?.ChuanDoan,
                            BacSiKham = tenBacSiKeDon,
                            LoiDan = infoBN.LoiDan,
                            NguoiGiamHo = infoBN?.NguoiGiamHo,
                            MaBN = infoBN.MaBN,
                            SoDienThoai = infoBN.SoDienThoai,
                            SoThang = infoBN.SoThang,
                            CongKhoan = sttKhongBHYTNgoaiBV,
                            //KhoaPhong = khoaPhong,
                            ThoiDiemKeDon = donThuocBHYTChiTiets.Any() ? donThuocBHYTChiTiets.Select(z => z.ThoiDiemKeDon).First() :
                                (donThuocKhongBHYTChiTiets.Any() ? donThuocKhongBHYTChiTiets.Select(z => z.ThoiDiemKeDon).First() : (DateTime?)null),

                        };
                        contentThuocNgoaiBenhVien = TemplateHelpper.FormatTemplateWithContentTemplate(templateDonThuocNgoaiBenhVien.Body, data);
                    }
                }
            }
            if (!string.IsNullOrEmpty(resultThuocThucPhamChucNang))
            {
                resultThuocThucPhamChucNang = "<style>.thuoc-table{border-top: 1px solid #000;border-right: 1px solid #000;border-spacing: 0;}.thuoc-table td,.thuoc-table th{border-left: 1px solid #000;border-bottom: 1px solid #000;padding: 5px;}</style><table width='100%' class='thuoc-table'><thead><tr><th>STT</th><th>Tên sản phẩm – Cách dùng</th><th>Số lượng</th></tr></thead><tbody>" + resultThuocThucPhamChucNang + "</tbody></table>";
                var data = new DataYCKBDonThuoc
                {
                    Header = headerThucPhamChucNang,
                    TemplateDonThuoc = resultThuocThucPhamChucNang,
                    LogoUrl = inToaThuoc.HostingName + "/assets/img/logo-bacha-full.png",
                    BarCodeImgBase64 = !string.IsNullOrEmpty(infoBN.MaTN) ? BarcodeHelper.GenerateBarCode(infoBN.MaTN) : "",
                    MaTN = "<b>Mã TN: </b>" + "<b>" + infoBN.MaTN + "</b>",
                    HoTen = infoBN.HoTen,
                    NamSinhDayDu = infoBN.NamSinhDayDu,
                    Tuoi = infoBN.Tuoi,
                    CanNang = infoBN.CanNang,
                    GioiTinh = infoBN?.GioiTinh,
                    DiaChi = infoBN?.DiaChi,
                    SoTheBHYT = infoBN.BHYTMaSoThe,
                    ChuanDoan = infoBN?.ChuanDoan,
                    BacSiKham = tenBacSiKeDon,
                    LoiDan = infoBN.LoiDan,
                    MaBN = infoBN.MaBN,
                    SoDienThoai = infoBN.SoDienThoai,
                    SoThang = infoBN.SoThang,
                    CongKhoan = sttTPCN,
                    //KhoaPhong = khoaPhong,
                    ThoiDiemKeDon = donThuocBHYTChiTiets.Any() ? donThuocBHYTChiTiets.Select(z => z.ThoiDiemKeDon).First() :
                                (donThuocKhongBHYTChiTiets.Any() ? donThuocKhongBHYTChiTiets.Select(z => z.ThoiDiemKeDon).First() : (DateTime?)null),

                };
                contentThucPhamChucNang = TemplateHelpper.FormatTemplateWithContentTemplate(templateDonThuocThucPhamChucNang.Body, data);
            }

            if (inToaThuoc.VatTu)
            {
                var headerVatTu = "<p style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
                                 "<th>VẬT TƯ Y TẾ</th>" + "</p>";
                var vTYTChiTiets = BaseRepository.TableNoTracking
                       .Include(yckb => yckb.YeuCauTiepNhan)
                       .Include(yckb => yckb.YeuCauKhamBenhDonVTYTs).ThenInclude(ycdt => ycdt.YeuCauKhamBenhDonVTYTChiTiets).ThenInclude(dtct => dtct.NhomVatTu)
                       .Include(yckb => yckb.YeuCauKhamBenhDonVTYTs).ThenInclude(ycdt => ycdt.YeuCauKhamBenhDonVTYTChiTiets).ThenInclude(dtct => dtct.VatTuBenhVien)
                       .SelectMany(yckb => yckb.YeuCauKhamBenhDonVTYTs)
                               .Select(vt => vt)
                               .Where(vt => vt.YeuCauKhamBenhId == inToaThuoc.YeuCauKhamBenhId)
                       .SelectMany(ycvt => ycvt.YeuCauKhamBenhDonVTYTChiTiets).Include(s => s.YeuCauKhamBenhDonVTYT)
                       .Select(vtct => vtct).Include(dtct => dtct.NhomVatTu).Include(dtct => dtct.VatTuBenhVien)
                       .ToList();

                var STT = 0;
                foreach (var item in vTYTChiTiets)
                {
                    STT++;
                    resultVatTu += "<tr>";
                    resultVatTu += "<td style='vertical-align: top;text-align: center'>" + STT + "</td>";
                    resultVatTu += "<td>" + item.Ten
                                + (!string.IsNullOrEmpty(item.GhiChu) ? "</br> <i>" + item.GhiChu + "</i>" : "")
                        + "</td>";
                    resultVatTu += "<td style='vertical-align: top;text-align: center'>" + item.SoLuong + " " + item.DonViTinh + "</td>";
                    //resultVatTu += "<td><i>" + (!string.IsNullOrEmpty(item.GhiChu) ? item.GhiChu : "&nbsp;") + "</i></td>";
                    resultVatTu += "</tr>";
                }
                resultVatTu = "<style>.thuoc-table{border-top: 1px solid #000;border-right: 1px solid #000;border-spacing: 0;}.thuoc-table td,.thuoc-table th{border-left: 1px solid #000;border-bottom: 1px solid #000;padding: 5px;}</style><table width='100%' class='thuoc-table'><thead><tr><th>STT</th><th>TÊN VTYT</th><th>SỐ LƯỢNG</th></tr></thead><tbody>" + resultVatTu + "</tbody></table>";
                if (vTYTChiTiets.Any())
                {
                    var data = new DataYCKBVatTu
                    {
                        MaTN = "<b>Mã TN: </b>" + "<b>" + infoBN.MaTN + "</b>",
                        HoTen = infoBN.HoTen,
                        NamSinhDayDu = infoBN.NamSinhDayDu,
                        Tuoi = infoBN.Tuoi,
                        CanNang = infoBN.CanNang,
                        GioiTinh = infoBN?.GioiTinh,
                        DiaChi = infoBN?.DiaChi,
                        SoTheBHYT = infoBN.BHYTMaSoThe,
                        ChuanDoan = infoBN?.ChuanDoan,
                        BacSiKham = tenBacSiKeDon,
                        MaBN = infoBN.MaBN,
                        SoDienThoai = infoBN.SoDienThoai,
                        CongKhoan = STT,
                        Header = headerVatTu,
                        TemplateVatTu = resultVatTu,
                        LogoUrl = inToaThuoc.HostingName + "/assets/img/logo-bacha-full.png",
                        BarCodeImgBase64 = !string.IsNullOrEmpty(infoBN.MaTN) ? BarcodeHelper.GenerateBarCode(infoBN.MaTN) : "",
                        CMND = infoBN?.CMND,
                        NguoiGiamHo = infoBN?.NguoiGiamHo,
                        ThoiDiemKeDon = vTYTChiTiets.Any() ? vTYTChiTiets.Select(z => z.YeuCauKhamBenhDonVTYT.ThoiDiemKeDon).First() : (DateTime?)null

                    };
                    contentVatTu = TemplateHelpper.FormatTemplateWithContentTemplate(templateVatTuYT.Body, data);
                }
            }
            if (contentThuocBHYT != "")
            {
                contentThuocBHYT = contentThuocBHYT + "<div class=\"pagebreak\"> </div>";
            }
            if (contentThuocTrongBenhVien != "")
            {
                contentThuocTrongBenhVien = contentThuocTrongBenhVien + "<div class=\"pagebreak\"> </div>";
            }
            if (contentThuocNgoaiBenhVien != "")
            {
                contentThuocNgoaiBenhVien = contentThuocNgoaiBenhVien + "<div class=\"pagebreak\"> </div>";
            }
            if (contentThucPhamChucNang != "")
            {
                contentThucPhamChucNang = contentThucPhamChucNang + "<div class=\"pagebreak\"> </div>";
            }
            if (contentVatTu != "")
            {
                contentVatTu = contentVatTu + "<div class=\"pagebreak\"> </div>";
            }
            if(inToaThuoc.ThuocBHYT == true)
            {
                content += contentThuocBHYT;
            }
            if (inToaThuoc.ThuocKhongBHYT == true)
            {
                content += contentThuocTrongBenhVien + contentThuocNgoaiBenhVien + contentThucPhamChucNang + contentVatTu;
            }
            return content;
        }
        #endregion
    }
}
