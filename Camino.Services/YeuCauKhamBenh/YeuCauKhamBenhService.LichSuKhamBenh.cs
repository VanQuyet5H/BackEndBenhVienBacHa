using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Camino.Core.Helpers;
using Camino.Core.Data;
using Camino.Core.Domain.Entities.KetQuaSinhHieus;
using Camino.Core.Domain.ValueObject.BenhNhans;
using System.Collections.Generic;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Core.Domain;
using System;
using static Camino.Core.Domain.Enums;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.KetQuaSinhHieu;
using Camino.Core.Helpers;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Newtonsoft.Json;

namespace Camino.Services.YeuCauKhamBenh
{
    public partial class YeuCauKhamBenhService
    {

        #region GetDataForGridAsyncLichSuKhamBenh
        public async Task<GridDataSource> GetDataForGridAsyncLichSuKhamBenh(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            long.TryParse(queryInfo.AdditionalSearchString, out var BenhNhanId);
            var query = BaseRepository.TableNoTracking
                .Include(o => o.YeuCauTiepNhan).ThenInclude(o => o.NoiTiepNhan).ThenInclude(o => o.KhoaPhong)
                .Include(o => o.Icdchinh)
                .Include(o => o.YeuCauTiepNhan).ThenInclude(o => o.NhanVienTiepNhan).ThenInclude(o => o.User)
                //.Include(o => o.ChuanDoan)
                .Include(o => o.YeuCauTiepNhan).ThenInclude(o => o.KetQuaSinhHieus)
                .Include(o => o.YeuCauTiepNhan).ThenInclude(o => o.BenhNhan)
                .Where(o => o.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham && o.YeuCauTiepNhan.BenhNhanId == BenhNhanId)
                .Select(s => new DanhSachDaKhamGridVo
                {
                    Id = s.Id,
                    YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                    MaBN = queryInfo.AdditionalSearchString,
                    Phong = s.NoiKetLuan.Ten, //phong
                    BaSiKham = s.BacSiThucHien != null ? s.BacSiThucHien.User.HoTen : null,
                    BacSiKetLuan = s.BacSiKetLuanId != null ? s.BacSiKetLuan.User.HoTen : null,
                    HoTen = s.YeuCauTiepNhan.HoTen,
                    LyDoKham = s.YeuCauTiepNhan.TrieuChungTiepNhan,
                    ThoiDiemDangKy = s.ThoiDiemHoanThanh,
                    ThoiDiemDangKyDisplay = s.ThoiDiemDangKy.ApplyFormatDate(),
                    CachGiaiQuyet = s.CachGiaiQuyet,
                    TrieuChungLamSang = s.YeuCauKhamBenhTrieuChungs.Where(x => x.YeuCauKhamBenhId == s.Id).FirstOrDefault().TrieuChung.Ten,
                    ChuanDoanICD = s.GhiChuICDChinh,
                    TenDichVuKham = s.TenDichVu
                }).OrderByDescending(p => p.ThoiDiemDangKy).ApplyLike(queryInfo.SearchTerms, g => g.BaSiKham, g => g.BaSiKhamRemoveDiacritics, g => g.HoTen, g => g.HoTenRemoveDiacritics, g => g.Phong, g => g.PhongRemoveDiacritics, g => g.LyDoKham, g => g.LyDoKhamRemoveDiacritics, g => g.TrieuChungLamSang, g => g.TrieuChungLamSangDiacritics, g => g.ChuanDoanICD, g => g.ChuanDoanICDDiacritics, g=>g.CachGiaiQuyetDiacritics,g=>g.TenDichVuKham);

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            //var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
            //   .Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }
        public async Task<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> GetByIdHaveInclude1(long benhNhanId)
        {
            var result = await BaseRepository.GetByIdAsync(benhNhanId, s => s.Include(o => o.YeuCauTiepNhan).ThenInclude(o => o.NoiTiepNhan).ThenInclude(o => o.KhoaPhong)
                .Include(o => o.Icdchinh)
                .Include(o => o.YeuCauTiepNhan).ThenInclude(o => o.NhanVienTiepNhan).ThenInclude(o => o.User)
                //.Include(o => o.ChuanDoan)
                .Include(o => o.YeuCauTiepNhan).ThenInclude(o => o.KetQuaSinhHieus)
                .Include(o => o.YeuCauTiepNhan).ThenInclude(o => o.BenhNhan)
                 .Include(o => o.YeuCauTiepNhan).ThenInclude(o => o.YeuCauTiepNhanLichSuKhamBHYT)
            );
            return result;
        }
        public async Task<GridDataSource> GetTotalPageForGridAsyncLichSuKhamBenh(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking
                .Include(o => o.YeuCauTiepNhan).ThenInclude(o => o.NoiTiepNhan).ThenInclude(o => o.KhoaPhong)
                .Include(o => o.Icdchinh)
                .Include(o => o.YeuCauTiepNhan).ThenInclude(o => o.NhanVienTiepNhan).ThenInclude(o => o.User)
                //.Include(o => o.ChuanDoan)
                .Include(o => o.YeuCauTiepNhan).ThenInclude(o => o.KetQuaSinhHieus)
                .Include(o => o.YeuCauTiepNhan).ThenInclude(o => o.BenhNhan)
                .Where(o => o.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham && o.YeuCauTiepNhan.BenhNhanId == long.Parse(queryInfo.AdditionalSearchString))
                .Select(s => new DanhSachDaKhamGridVo
                {
                    Id = s.Id,
                    YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                    MaBN = queryInfo.AdditionalSearchString,
                    Phong = s.NoiKetLuan.Ten, //phong
                    BaSiKham = s.BacSiKetLuanId != null ? s.BacSiKetLuan.User.HoTen : null,
                    HoTen = s.YeuCauTiepNhan.HoTen,
                    LyDoKham = s.YeuCauTiepNhan.TrieuChungTiepNhan,
                    ThoiDiemDangKy = s.ThoiDiemHoanThanh,
                    ThoiDiemDangKyDisplay = s.ThoiDiemDangKy.ApplyFormatDate(),
                    TrieuChungLamSang = s.YeuCauKhamBenhTrieuChungs.Where(x => x.YeuCauKhamBenhId == s.Id).FirstOrDefault().TrieuChung.Ten,
                    ChuanDoanICD = s.Icdchinh.TenTiengViet,
                    CachGiaiQuyet = s.CachGiaiQuyet,
                    TenDichVuKham = s.TenDichVu
                }).OrderByDescending(p => p.ThoiDiemDangKy).ApplyLike(queryInfo.SearchTerms, g => g.BaSiKham, g => g.BaSiKhamRemoveDiacritics, g => g.HoTen, g => g.HoTenRemoveDiacritics, g => g.Phong, g => g.PhongRemoveDiacritics, g => g.LyDoKham, g => g.LyDoKhamRemoveDiacritics, g => g.TrieuChungLamSang, g => g.TrieuChungLamSangDiacritics, g => g.ChuanDoanICD, g => g.ChuanDoanICDDiacritics, g => g.CachGiaiQuyetDiacritics, g => g.TenDichVuKham);

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        #endregion
        #region GetDataForGridAsyncLichSuKhamBenh BHYT 

        public async Task<GridDataSource> GetDataForGridAsyncLichSuKhamBenhBHYT(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            long.TryParse(queryInfo.AdditionalSearchString, out var yeuCauTiepNhanId);
            var query = _yeuCauTiepNhanLichSuKhamBHYTRepository.TableNoTracking
                .Include(p => p.YeuCauTiepNhan).ThenInclude(p => p.BenhNhan)
                .Where(o => o.YeuCauTiepNhanId == yeuCauTiepNhanId)
            .Select(s => new GridLichSuKCB
            {
                Id = s.Id,

                KetQuaDieuTriNumber = s.KetQuaDieuTri,
                KetQuaDieuTri = s.KetQuaDieuTri != null ? s.KetQuaDieuTri.GetDescription() : "",

                LyDoVaoVienNumber = s.LyDoVaoVien,
                LyDoVaoVien = s.LyDoVaoVien != null ? s.LyDoVaoVien.GetDescription() : "",

                MaCoSoKCB = s.MaCSKCB,
                MaTheBHYT = s.MaCSKCB != null ? s.MaTheBHYT : "",

                NgayRaDateTime = s.NgayRa,
                NgayRaVien = s.NgayRa != null ? s.NgayRa.Value.ApplyFormatDateTime() : "",

                NgayVaoDateTime = s.NgayVao,
                NgayVaoVien = s.NgayVao != null ? s.NgayVao.Value.ApplyFormatDateTime() : "",
                TinhTrangRaVienNumber = s.TinhTrangRaVien,
                TinhTrangRaVien = s.TinhTrangRaVien != null ? s.TinhTrangRaVien.GetDescription() : "",

                HoVaTen = s.YeuCauTiepNhan.BenhNhan.HoTen,

            }).OrderByDescending(p => p.NgayRaDateTime).ApplyLike(queryInfo.SearchTerms, g => g.KetQuaDieuTri, g => g.LyDoVaoVien, g => g.NgayRaVien, g => g.NgayVaoVien, g => g.TinhTrangRaVien, g => g.HoVaTen, g => g.MaTheBHYT, g => g.MaCoSoKCB).ToList();

            var allData = new List<GridLichSuKCB>();
            foreach (var item in query)
            {
                item.STT = item.STT + 1;
            }
            var thuNgan = query.ToArray();
            var countTask = thuNgan.Length;
            return new GridDataSource { Data = thuNgan, TotalRowCount = countTask };
        }
        public async Task<GridDataSource> GetTotalPageForGridAsyncLichSuKhamBenhBHYT(QueryInfo queryInfo)
        {
            long.TryParse(queryInfo.AdditionalSearchString, out var yeuCauTiepNhanId);
            var query = _yeuCauTiepNhanLichSuKhamBHYTRepository.TableNoTracking
               .Include(p => p.YeuCauTiepNhan).ThenInclude(p => p.BenhNhan)
               .Where(o => o.YeuCauTiepNhanId == yeuCauTiepNhanId)
           .Select(s => new GridLichSuKCB
           {
               Id = s.Id,

               KetQuaDieuTriNumber = s.KetQuaDieuTri,
               KetQuaDieuTri = s.KetQuaDieuTri != null ? s.KetQuaDieuTri.GetDescription() : "",

               LyDoVaoVienNumber = s.LyDoVaoVien,
               LyDoVaoVien = s.LyDoVaoVien != null ? s.LyDoVaoVien.GetDescription() : "",

               MaCoSoKCB = s.MaCSKCB,
               MaTheBHYT = s.MaCSKCB != null ? s.MaTheBHYT : "",

               NgayRaDateTime = s.NgayRa,
               NgayRaVien = s.NgayRa != null ? s.NgayRa.Value.ApplyFormatDateTime() : "",

               NgayVaoDateTime = s.NgayVao,
               NgayVaoVien = s.NgayVao != null ? s.NgayVao.Value.ApplyFormatDateTime() : "",
               TinhTrangRaVienNumber = s.TinhTrangRaVien,
               TinhTrangRaVien = s.TinhTrangRaVien != null ? s.TinhTrangRaVien.GetDescription() : "",

               HoVaTen = s.YeuCauTiepNhan.BenhNhan.HoTen,

           }).OrderByDescending(p => p.NgayRaDateTime).ApplyLike(queryInfo.SearchTerms, g => g.KetQuaDieuTri, g => g.LyDoVaoVien, g => g.NgayRaVien, g => g.NgayVaoVien, g => g.TinhTrangRaVien, g => g.HoVaTen, g => g.MaTheBHYT, g => g.MaCoSoKCB).ToList();
            foreach (var item in query)
            {
                item.STT = item.STT + 1;
            }
            var thuNgan = query.ToArray();
            var countTask = thuNgan.Length;
            return new GridDataSource { TotalRowCount = countTask };
        }
        public Task<List<TrieuChungBenhSu>> GetDataForGridAsyncTrieuChungBenhSu(long ycKhamBenhId)
        {
            //var querys = BaseRepository.TableNoTracking
            //    .Where(o => o.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham && o.Id == ycKhamBenhId).Select(s => s.YeuCauTiepNhan.BenhNhanId).FirstOrDefault();

            //BVHD-3895
            var lstDichVuKhamVietTatId = GetichVuKhamIdHienThiTenVietTatAsync();

            var query = BaseRepository.TableNoTracking
                .Where(o => o.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham && o.Id == ycKhamBenhId)
                .Select(s => new TrieuChungBenhSu
                {
                    TrieuChungTiepNhan = s.TrieuChungTiepNhan,
                    BenhSu = s.BenhSu,
                    KhamToanThan = s.KhamToanThan,
                    TenDichVu = s.TenDichVu,
                    ThongTinKhamTheoDichVuTemplate = s.ThongTinKhamTheoDichVuTemplate,
                    ThongTinKhamTheoDichVuData = s.ThongTinKhamTheoDichVuData,
                    ChanDoanSoBoGhiChu = s.ChanDoanSoBoGhiChu,
                    ChanDoanSoBoICDId = s.ChanDoanSoBoICD.TenTiengViet,

                    NoiGioiThieu = s.ChanDoanCuaNoiGioiThieu,
                    IsKhamDoan = s.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe  ? true : false,
                    KSKPhanLoaiTheLuc = s.YeuCauTiepNhan.KSKPhanLoaiTheLuc,

                    //BVHD-3895
                    //LaDichVuKhamVietTat = s.YeuCauTiepNhan.LoaiYeuCauTiepNhan != EnumLoaiYeuCauTiepNhan.KhamSucKhoe && lstDichVuKhamVietTatId.Contains(s.DichVuKhamBenhBenhVienId)
                });


            return query.ToListAsync();
        }
        public ListAll GetDataKhamCoQuanKhacTatCaChuyenKhoaTheoBenhNhan(long yeuCauTiepNhanId)
        {
            var queryDataListChuyenKhoa = BaseRepository.TableNoTracking
                .Where(o => o.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham && o.YeuCauTiepNhanId == yeuCauTiepNhanId)
                .Select(s => new ChuyenKhoaBenhNhan
                {
                    ThongTinKhamTheoDichVuTemplate = s.ThongTinKhamTheoDichVuTemplate,
                    ThongTinKhamTheoDichVuData = s.ThongTinKhamTheoDichVuData,
                }).ToList();
            List<ListAllChuyenKhoaBenhNhanNoiKhoaNgoaiKhoaSanPhuKhoaDaLieu> listChuyenKhoaNoi = new List<ListAllChuyenKhoaBenhNhanNoiKhoaNgoaiKhoaSanPhuKhoaDaLieu>();
            List<ListAllChuyenKhoaBenhNhanNoiKhoaNgoaiKhoaSanPhuKhoaDaLieu> listChuyenKhoaNgoai = new List<ListAllChuyenKhoaBenhNhanNoiKhoaNgoaiKhoaSanPhuKhoaDaLieu>();
            List<ListAllChuyenKhoaBenhNhanNoiKhoaNgoaiKhoaSanPhuKhoaDaLieu> listChuyenKhoaSanPhuKhoa = new List<ListAllChuyenKhoaBenhNhanNoiKhoaNgoaiKhoaSanPhuKhoaDaLieu>();
            List<ListAllChuyenKhoaBenhNhanNoiKhoaNgoaiKhoaSanPhuKhoaDaLieu> listChuyenKhoaDaLieu = new List<ListAllChuyenKhoaBenhNhanNoiKhoaNgoaiKhoaSanPhuKhoaDaLieu>();
            MatOBJ objMat = new MatOBJ();
            TaiMuiHongOBJ objTaiMuiHong = new TaiMuiHongOBJ();
            RangHamMatOBJ objRangHamMat = new RangHamMatOBJ();
            foreach (var itemChuyenKhoa in queryDataListChuyenKhoa)
            {
                var jsonOjbectTemplate = JsonConvert.DeserializeObject<ThongTinBenhNhanKhamKhacTemplateList>(itemChuyenKhoa.ThongTinKhamTheoDichVuTemplate);
                var jsonOjbectData = JsonConvert.DeserializeObject<ThongTinBenhNhanKhamKhacList>(itemChuyenKhoa.ThongTinKhamTheoDichVuData);

                foreach (var itemx in jsonOjbectTemplate.ComponentDynamics)
                {
                    var kiemTra = jsonOjbectData.DataKhamTheoTemplate.Where(s => s.Id == itemx.Id);
                    if (kiemTra.Any())
                    {
                        ListAllChuyenKhoaBenhNhanNoiKhoaNgoaiKhoaSanPhuKhoaDaLieu chuyenKhoaObj = new ListAllChuyenKhoaBenhNhanNoiKhoaNgoaiKhoaSanPhuKhoaDaLieu();
                        switch (itemx.Id)
                        {
                            case "TuanHoan":
                                chuyenKhoaObj.Id = ChuyenKhoaKhamSucKhoe.NoiKhoa;
                                chuyenKhoaObj.TenTemplate = "Tuần hoàn:";
                                chuyenKhoaObj.Ten = kiemTra.First().Value;
                                listChuyenKhoaNoi.Add(chuyenKhoaObj);
                                break;

                            case "TuanHoanPhanLoai":
                                chuyenKhoaObj.Id = ChuyenKhoaKhamSucKhoe.NoiKhoa;
                                chuyenKhoaObj.TenTemplate = "Phân loại:";
                                chuyenKhoaObj.Ten = !string.IsNullOrEmpty(kiemTra.First().Value) ? ((Enums.PhanLoaiSucKhoe)(int.Parse(kiemTra.First().Value))).GetDescription() : "";
                                listChuyenKhoaNoi.Add(chuyenKhoaObj);
                                break;

                            case "HoHap":
                                chuyenKhoaObj.Id = ChuyenKhoaKhamSucKhoe.NoiKhoa;
                                chuyenKhoaObj.TenTemplate = "Hô hấp:";
                                chuyenKhoaObj.Ten = kiemTra.First().Value;
                                listChuyenKhoaNoi.Add(chuyenKhoaObj);
                                break;
                            case "HoHapPhanLoai":
                                chuyenKhoaObj.Id = ChuyenKhoaKhamSucKhoe.NoiKhoa;
                                chuyenKhoaObj.TenTemplate = "Phân loại:";
                                chuyenKhoaObj.Ten = !string.IsNullOrEmpty(kiemTra.First().Value) ? ((Enums.PhanLoaiSucKhoe)(int.Parse(kiemTra.First().Value))).GetDescription() : "";
                                listChuyenKhoaNoi.Add(chuyenKhoaObj);
                                break;

                            case "TieuHoa":
                                chuyenKhoaObj.Id = ChuyenKhoaKhamSucKhoe.NoiKhoa;
                                chuyenKhoaObj.TenTemplate = "Tiêu hóa:";
                                chuyenKhoaObj.Ten = kiemTra.First().Value;
                                listChuyenKhoaNoi.Add(chuyenKhoaObj);
                                break;

                            case "TieuHoaPhanLoai":
                                chuyenKhoaObj.Id = ChuyenKhoaKhamSucKhoe.NoiKhoa;
                                chuyenKhoaObj.TenTemplate = "Phân loại:";
                                chuyenKhoaObj.Ten = !string.IsNullOrEmpty(kiemTra.First().Value) ? ((Enums.PhanLoaiSucKhoe)(int.Parse(kiemTra.First().Value))).GetDescription() : "";
                                listChuyenKhoaNoi.Add(chuyenKhoaObj);
                                break;

                            case "ThanTietNieu":
                                chuyenKhoaObj.Id = ChuyenKhoaKhamSucKhoe.NoiKhoa;
                                chuyenKhoaObj.TenTemplate = "Thận tiết niệu:";
                                chuyenKhoaObj.Ten = kiemTra.First().Value;
                                listChuyenKhoaNoi.Add(chuyenKhoaObj);
                                break;

                            case "ThanTietNieuPhanLoai":
                                chuyenKhoaObj.Id = ChuyenKhoaKhamSucKhoe.NoiKhoa;
                                chuyenKhoaObj.TenTemplate = "Phân loại:";
                                chuyenKhoaObj.Ten = !string.IsNullOrEmpty(kiemTra.First().Value) ? ((Enums.PhanLoaiSucKhoe)(int.Parse(kiemTra.First().Value))).GetDescription() : "";
                                listChuyenKhoaNoi.Add(chuyenKhoaObj);
                                break;

                            case "NoiTiet":
                                chuyenKhoaObj.Id = ChuyenKhoaKhamSucKhoe.NoiKhoa;
                                chuyenKhoaObj.TenTemplate = "Nội tiết:";
                                chuyenKhoaObj.Ten = kiemTra.First().Value;
                                listChuyenKhoaNoi.Add(chuyenKhoaObj);
                                break;

                            case "NoiTietPhanLoai":
                                chuyenKhoaObj.Id = ChuyenKhoaKhamSucKhoe.NoiKhoa;
                                chuyenKhoaObj.TenTemplate = "Phân loại:";
                                chuyenKhoaObj.Ten = !string.IsNullOrEmpty(kiemTra.First().Value) ? ((Enums.PhanLoaiSucKhoe)(int.Parse(kiemTra.First().Value))).GetDescription() : "";
                                listChuyenKhoaNoi.Add(chuyenKhoaObj);
                                break;

                            case "CoXuongKhop":
                                chuyenKhoaObj.Id = ChuyenKhoaKhamSucKhoe.NoiKhoa;
                                chuyenKhoaObj.TenTemplate = "Cơ xương khớp:";
                                chuyenKhoaObj.Ten = kiemTra.First().Value;
                                listChuyenKhoaNoi.Add(chuyenKhoaObj);
                                break;

                            case "CoXuongKhopPhanLoai":
                                chuyenKhoaObj.Id = ChuyenKhoaKhamSucKhoe.NoiKhoa;
                                chuyenKhoaObj.TenTemplate = "Phân loại:";
                                chuyenKhoaObj.Ten = !string.IsNullOrEmpty(kiemTra.First().Value) ? ((Enums.PhanLoaiSucKhoe)(int.Parse(kiemTra.First().Value))).GetDescription() : "";
                                listChuyenKhoaNoi.Add(chuyenKhoaObj);
                                break;

                            case "ThanKinh":
                                chuyenKhoaObj.Id = ChuyenKhoaKhamSucKhoe.NoiKhoa;
                                chuyenKhoaObj.TenTemplate = "Thần kinh:";
                                chuyenKhoaObj.Ten = kiemTra.First().Value;
                                listChuyenKhoaNoi.Add(chuyenKhoaObj);
                                break;

                            case "ThanKinhPhanLoai":
                                chuyenKhoaObj.Id = ChuyenKhoaKhamSucKhoe.NoiKhoa;
                                chuyenKhoaObj.TenTemplate = "Phân loại:";
                                chuyenKhoaObj.Ten = !string.IsNullOrEmpty(kiemTra.First().Value) ? ((Enums.PhanLoaiSucKhoe)(int.Parse(kiemTra.First().Value))).GetDescription() : "";
                                listChuyenKhoaNoi.Add(chuyenKhoaObj);
                                break;

                            case "TamThan":
                                chuyenKhoaObj.Id = ChuyenKhoaKhamSucKhoe.NoiKhoa;
                                chuyenKhoaObj.TenTemplate = "Tâm thần:";
                                chuyenKhoaObj.Ten = kiemTra.First().Value;
                                listChuyenKhoaNoi.Add(chuyenKhoaObj);
                                break;

                            case "TamThanPhanLoai":
                                chuyenKhoaObj.Id = ChuyenKhoaKhamSucKhoe.NoiKhoa;
                                chuyenKhoaObj.TenTemplate = "Phân loại:";
                                chuyenKhoaObj.Ten = !string.IsNullOrEmpty(kiemTra.First().Value) ? ((Enums.PhanLoaiSucKhoe)(int.Parse(kiemTra.First().Value))).GetDescription() : "";
                                listChuyenKhoaNoi.Add(chuyenKhoaObj);
                                break;
                            case "NgoaiKhoa":
                                chuyenKhoaObj.Id = ChuyenKhoaKhamSucKhoe.NgoaiKhoa;
                                chuyenKhoaObj.TenTemplate = "Ngoại khoa:";
                                chuyenKhoaObj.Ten = kiemTra.First().Value;
                                listChuyenKhoaNgoai.Add(chuyenKhoaObj);
                                break;

                            case "NgoaiKhoaPhanLoai":
                                chuyenKhoaObj.Id = ChuyenKhoaKhamSucKhoe.NgoaiKhoa;
                                chuyenKhoaObj.TenTemplate = "Phân loại:";
                                chuyenKhoaObj.Ten = !string.IsNullOrEmpty(kiemTra.First().Value) ? ((Enums.PhanLoaiSucKhoe)(int.Parse(kiemTra.First().Value))).GetDescription() : "";
                                listChuyenKhoaNgoai.Add(chuyenKhoaObj);
                                break;

                            case "SanPhuKhoa":
                                chuyenKhoaObj.Id = ChuyenKhoaKhamSucKhoe.SanPhuKhoa;
                                chuyenKhoaObj.TenTemplate = "Sản phụ khoa:";
                                chuyenKhoaObj.Ten = kiemTra.First().Value;
                                listChuyenKhoaSanPhuKhoa.Add(chuyenKhoaObj);
                                break;

                            case "SanPhuKhoaPhanLoai":
                                chuyenKhoaObj.Id = ChuyenKhoaKhamSucKhoe.SanPhuKhoa;
                                chuyenKhoaObj.TenTemplate = "Phân loại:";
                                chuyenKhoaObj.Ten = !string.IsNullOrEmpty(kiemTra.First().Value) ? ((Enums.PhanLoaiSucKhoe)(int.Parse(kiemTra.First().Value))).GetDescription() : "";
                                listChuyenKhoaSanPhuKhoa.Add(chuyenKhoaObj);
                                break;

                            case "DaLieu":
                                chuyenKhoaObj.Id = ChuyenKhoaKhamSucKhoe.DaLieu;
                                chuyenKhoaObj.TenTemplate = "Da liễu:";
                                chuyenKhoaObj.Ten = kiemTra.First().Value;
                                listChuyenKhoaDaLieu.Add(chuyenKhoaObj);
                                break;
                            case "DaLieuPhanLoai":
                                chuyenKhoaObj.Id = ChuyenKhoaKhamSucKhoe.DaLieu;
                                chuyenKhoaObj.TenTemplate = "Phân loại:";
                                chuyenKhoaObj.Ten = !string.IsNullOrEmpty(kiemTra.First().Value) ? ((Enums.PhanLoaiSucKhoe)(int.Parse(kiemTra.First().Value))).GetDescription() : "";
                                listChuyenKhoaDaLieu.Add(chuyenKhoaObj);
                                break;
                            case "CacBenhVeMat":
                               objMat.CacBenhVeMat = kiemTra.FirstOrDefault().Value;
                                break;
                            case "MatPhanLoai":
                                objMat.PhanLoai = !string.IsNullOrEmpty(kiemTra.First().Value) ? ((Enums.PhanLoaiSucKhoe)(int.Parse(kiemTra.First().Value))).GetDescription() : "";
                                break;
                            case "CacBenhTaiMuiHong":
                                objTaiMuiHong.CacBenhTaiMuiHong = kiemTra.FirstOrDefault().Value;
                                break;
                            case "TaiMuiHongPhanLoai":
                                objTaiMuiHong.PhanLoai = !string.IsNullOrEmpty(kiemTra.First().Value) ? ((Enums.PhanLoaiSucKhoe)(int.Parse(kiemTra.First().Value))).GetDescription() : "";
                                break;
                            case "HamTren":
                                objRangHamMat.HamTren = kiemTra.FirstOrDefault().Value;
                                break;
                            case "HamDuoi":
                                objRangHamMat.HamDuoi = kiemTra.FirstOrDefault().Value;
                                break;
                            case "CacBenhRangHamMat":
                                objRangHamMat.CacBenhRangHamMat = kiemTra.FirstOrDefault().Value;
                                break;
                            case "RangHamMatPhanLoai":
                                objRangHamMat.PhanLoai = !string.IsNullOrEmpty(kiemTra.First().Value) ? ((Enums.PhanLoaiSucKhoe)(int.Parse(kiemTra.First().Value))).GetDescription() : "";
                                break;
                            default:
                                //do a different thing
                                break;
                        }
                    }


                }
                var kiemTraGroupItem = jsonOjbectTemplate.ComponentDynamics.Where(s => s.Id == "group");
                foreach (var itemx in kiemTraGroupItem)
                {
                    if (itemx.groupItems != null)
                    {
                        
                        foreach (var itemxx in itemx.groupItems)
                        {   
                            var kiemTra = jsonOjbectData.DataKhamTheoTemplate.Where(s => s.Id == itemxx.Id);
                            
                            if (kiemTra.Any())
                            {
                                switch (itemxx.Id)
                                {

                                    case "KhongKinhMatPhai":
                                        objMat.KhongKinhMatPhai = kiemTra.FirstOrDefault().Value;
                                        break;
                                    case "KhongKinhMatTrai":
                                        objMat.KhongKinhMatTrai = kiemTra.FirstOrDefault().Value;
                                        break;
                                    case "CoKinhMatPhai":
                                        objMat.CoKinhMatPhai = kiemTra.FirstOrDefault().Value;
                                        break;
                                    case "CoKinhMatTrai":
                                        objMat.CoKinhMatTrai = kiemTra.FirstOrDefault().Value;
                                        break;
                                    case "CacBenhVeMat":
                                        objMat.CacBenhVeMat = kiemTra.FirstOrDefault().Value;
                                        break;
                                    case "MatPhanLoai":
                                        objMat.PhanLoai = kiemTra.FirstOrDefault().Value;
                                        break;
                                    case "TaiTraiNoiThuong":
                                        objTaiMuiHong.TaiTraiNoiThuong = kiemTra.FirstOrDefault().Value;
                                        break;
                                    case "TaiTraiNoiTham":
                                        objTaiMuiHong.TaiTraiNoiTham = kiemTra.FirstOrDefault().Value;
                                        break;
                                    case "TaiPhaiNoiThuong":
                                        objTaiMuiHong.TaiPhaiNoiThuong = kiemTra.FirstOrDefault().Value;
                                        break;
                                    case "TaiPhaiNoiTham":
                                        objTaiMuiHong.TaiPhaiNoiTham = kiemTra.FirstOrDefault().Value;
                                        break;
                                    default:
                                        //do a different thing
                                        break;
                                }
                            }
                        }
                    }
                }
            }


            ////// OBJ ALL tất cả chuyên khoa 

            ListAll obj = new ListAll();

            obj.ListNoiKhoa = listChuyenKhoaNoi.OrderBy(s=>s.Id).ToList();
            obj.ListNgoaiKhoa = listChuyenKhoaNgoai.OrderBy(s => s.Id).ToList();

            obj.ListSanPhuKhoa = listChuyenKhoaSanPhuKhoa.OrderBy(s => s.Id).ToList();
            obj.ListDaLieu = listChuyenKhoaDaLieu.OrderBy(s => s.Id).ToList();
            obj.Mat = objMat;
            obj.TaiMuiHong = objTaiMuiHong;
            obj.RangHamMat = objRangHamMat;



            return obj;
        }
        #endregion
        #region GetTotalPageForGridAsyncLichSuKhamBenhChild
        public async Task<GridDataSource> GetDataForGridAsyncLichSuKhamBenhChild(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var query = _yeuCauKhamBenhDonThuocChiTietRepository.TableNoTracking
                .Include(o => o.YeuCauKhamBenhDonThuoc).ThenInclude(o => o.YeuCauKhamBenh)
                .Include(o => o.DuocPham).ThenInclude(o => o.DonViTinh)
                .Include(o => o.DuongDung)
                .Where(o => o.YeuCauKhamBenhDonThuoc.YeuCauKhamBenhId == long.Parse(queryInfo.SearchTerms))
                .Select(s => new DanhSachDaKhamGridVo
                {
                    Id = s.Id,
                    Duoc = s.DuocPham.Ten,
                    HoatChat = s.DuocPham.HoatChat,
                    DonViTinh = s.DuocPham.DonViTinh.Ten,
                    Sang = s.DungSang,
                    Trua = s.DungTrua,
                    Toi = s.DungToi,
                    SoNgay = s.SoNgayDung,
                    SoLuong = s.SoLuong,
                    DuongDung = s.DuongDung.Ten,
                    GhiChu = s.GhiChu
                });
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetTotalPageForGridAsyncLichSuKhamBenhChild(QueryInfo queryInfo)
        {
            var query = _yeuCauKhamBenhDonThuocChiTietRepository.TableNoTracking
               .Include(o => o.YeuCauKhamBenhDonThuoc).ThenInclude(o => o.YeuCauKhamBenh)
               .Include(o => o.DuocPham).ThenInclude(o => o.DonViTinh)
               .Include(o => o.DuongDung)
               .Where(o => o.YeuCauKhamBenhDonThuoc.YeuCauKhamBenhId == long.Parse(queryInfo.SearchTerms))
               .Select(s => new DanhSachDaKhamGridVo
               {
                   Id = s.Id,
                   Duoc = s.DuocPham.Ten,
                   HoatChat = s.DuocPham.HoatChat,
                   DonViTinh = s.DuocPham.DonViTinh.Ten,
                   Sang = s.DungSang,
                   Trua = s.DungTrua,
                   Toi = s.DungToi,
                   SoNgay = s.SoNgayDung,
                   SoLuong = s.SoLuong,
                   DuongDung = s.DuongDung.Ten,
                   GhiChu = s.GhiChu
               });
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        #endregion
        #region Chỉ số sinh hiệu
        public async Task<GridDataSource> GetDataForGridAsyncChiSoSinhHieu(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            //long.TryParse(queryInfo.AdditionalSearchString, out var yeuCauTiepNhanId);
            var yeuCauTiepNhanId = long.Parse(queryInfo.AdditionalSearchString);
            var query = _ketQuaSinhHieuRepository.TableNoTracking
               .Include(p => p.NhanVienThucHien).ThenInclude(p => p.User)
               .Where(p => p.YeuCauTiepNhanId == yeuCauTiepNhanId)
               .Select(source => new KetQuaSinhHieuGridVo
               {
                   Id = source.Id,
                   NhanVienThucHien = source.NhanVienThucHien.User.HoTen,
                   Bmi = source.Bmi,
                   CanNang = source.CanNang,
                   ChieuCao = source.ChieuCao,
                   HuyetAp = source.HuyetApTamThu + "/" + source.HuyetApTamTruong,
                   NgayThucHien = source.CreatedOn.Value.ApplyFormatDateTimeSACH(),
                   NhipTho = source.NhipTho,
                   NhipTim = source.NhipTim,
                   ThanNhiet = source.ThanNhiet,
                   Glassgow = source.Glassgow,
                   SpO2 = source.SpO2
               });
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        #endregion
        #region Dị ứng thuốc
        public async Task<GridDataSource> GetDataForGridAsyncDiUngThuoc(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            //long.TryParse(queryInfo.AdditionalSearchString, out var benhNhanId);
            var benhNhanId = long.Parse(queryInfo.AdditionalSearchString);
            var query = _benhNhanDiUngThuocRepository.TableNoTracking
                               .Where(r => r.BenhNhanId == benhNhanId)
                               .Select(source => new BenhNhanDiUngThuocKhamBenhGridVo()
                               {
                                   //Todo: Nam Update
                                   BieuHienDiUng = source.BieuHienDiUng,
                                   LoaiDiUng = source.LoaiDiUng,
                                   TenLoaiDiUng = source.LoaiDiUng.GetDescription(),
                                   TenDiUng = source.TenDiUng,
                                   TenMucDo = source.MucDo.GetDescription()
                               });
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }
        #endregion
        #region Tiền sử bệnh
        public async Task<GridDataSource> GetDataForGridAsyncTienSuBenh(QueryInfo queryInfo)
        {
            //TODO: Nam Update
            //long.TryParse(queryInfo.AdditionalSearchString, out var yeuCauTiepNhanId);
            //var query = _benhNhanTienSuBenhRepository.TableNoTracking
            //            //.Include(p => p.Icd)
            //            .Where(p => p.BenhNhanId == yeuCauTiepNhanId)
            //            .Select(source => new BenhNhanTienSuKhamBenhGridVo
            //            {
            //                Id = source.Id,
            //                LoaiTienSuBenh = source.LoaiTienSuBenh.GetDescription(),
            //                TenBenh = source.TenBenh,
            //            });
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var benhNhanId = long.Parse(queryInfo.AdditionalSearchString);
            var query = _benhNhanTienSuBenhRepository.TableNoTracking
                       .Where(p => p.BenhNhanId == benhNhanId)
                       .Select(source => new BenhNhanTienSuKhamBenhGridVo
                       {
                           Id = source.Id,
                           LoaiTienSuBenh = source.LoaiTienSuBenh.GetDescription(),
                           TenBenh = source.TenBenh,
                       });
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }
        #endregion
        #region Khám bệnh 
        public async Task<List<DanhSachDaKhamKhamBenhGridVo>> GetDataForGridAsyncKhamBenh(long yeuCauKhamBenhId)
        {
            var query = await BaseRepository.TableNoTracking
                        .Include(o => o.YeuCauTiepNhan)
                        .Include(o => o.YeuCauKhamBenhChuanDoans)
                        .Where(r => r.Id == yeuCauKhamBenhId)
                        .Select(s => new DanhSachDaKhamKhamBenhGridVo()
                        {
                            Id = s.Id,
                            TrieuChungLamSang = s.YeuCauKhamBenhTrieuChungs.Where(x => x.YeuCauKhamBenhId == yeuCauKhamBenhId).ToList(),
                            LyDoKham = s.YeuCauTiepNhan.TrieuChungTiepNhan,
                            GhiChuTrieuChungLamSang = s.GhiChuTrieuChungLamSang,
                            ChuanDoanBanDau = s.YeuCauKhamBenhChuanDoans.Where(x => x.YeuCauKhamBenhId == yeuCauKhamBenhId).ToList(),
                            ThongTinKhamTheoKhoa = s.ThongTinKhamTheoKhoa
                        }).ToListAsync();
            return query;
        }
        public async Task<string> GetTrieuChungLamSang(long TrieuChungId)
        {
            //lay ds duoc pham co trong kho ngoai
            var query = _trieuChungRepository.TableNoTracking.Where(x => x.Id == TrieuChungId).FirstOrDefault();
            if (query != null)
            {
                return query.Ten;
            }
            return null;
        }
        public async Task<string> GetChuanDoanBanDau(long chuanDoanId)
        {
            //lay ds duoc pham co trong kho ngoai
            var query = _chuanDoanRepository.TableNoTracking.Where(x => x.Id == chuanDoanId).FirstOrDefault();
            if (query != null)
            {
                var data = query.Ma + "-" + query.TenTiengViet;
                return data;
            }
            return null;
        }
        public async Task<List<DanhSachDaKhamGridVo>> GetDataForGridAsyncKhamBenhRangHamMat(long yeuCauKhamBenhId)
        {
            var query = await BaseRepository.TableNoTracking
                        .Where(r => r.Id == yeuCauKhamBenhId)
                        .Select(s => new DanhSachDaKhamGridVo()
                        {
                            ThongTinKhamTheoKhoa = s.DichVuKhamBenhBenhVien.TemplateKhamBenhTheoDichVus.Where(p => p.DichVuKhamBenhBenhVienId == s.DichVuKhamBenhBenhVienId).FirstOrDefault().ComponentDynamics
                        }).ToListAsync();
            return query;
        }
        #endregion
        #region Chỉ Dịnh 
        private decimal ToPercentage(int percent)
        {
            return (decimal)(percent) / 100;
        }
 
        public bool KiemTraCoChiecKhauKCK(long yeuCauKhamBenh)
        {
            //TODO: need update goi dv
            //var listyeuCaugoidichvu = _yeuCauGoiDichVuRepository.TableNoTracking
            //                         .Where(o => o.YeuCauKhamBenhId == yeuCauKhamBenh && o.CoChietKhau == false).ToList();
            //if (listyeuCaugoidichvu.Count() > 0)
            //{
            //    return true;
            //}
            return false;
        }

        public List<LichSuKhamBenhGridVo> GetDataForGridAsyncChiDinDichVuKhac(long yeuCauKhamBenhId, long yeuCauTiepNhanId)
        {
            var goiDichVuKhamBenh = new List<LichSuKhamBenhGridVo>();

            var lstYeuCauKhamBenhChiDinh = BaseRepository.TableNoTracking
                .Include(o => o.BacSiDangKy)
                .Include(o => o.NhomGiaDichVuKhamBenhBenhVien)
                .Include(o => o.NoiDangKy)
                .Include(o => o.BacSiDangKy).ThenInclude(o=>o.User)
                .Include(o => o.NhanVienChiDinh).ThenInclude(o => o.User)
                .Include(o => o.YeuCauGoiDichVu).ThenInclude(o => o.ChuongTrinhGoiDichVu)
                .Where(o => o.YeuCauTiepNhanId == yeuCauTiepNhanId && o.YeuCauKhamBenhTruocId == yeuCauKhamBenhId && o.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham)
                .OrderBy(o => o.CreatedOn).ToList();
            goiDichVuKhamBenh.AddRange(lstYeuCauKhamBenhChiDinh.Select(p => new LichSuKhamBenhGridVo
            {
                Id = p.Id,
                Nhom = EnumNhomGoiDichVu.DichVuKhamBenh.GetDescription(),
                NhomId = (int)EnumNhomGoiDichVu.DichVuKhamBenh,
                LoaiYeuCauDichVuId = p.DichVuKhamBenhBenhVienId,
                NhomGiaDichVuBenhVienId = p.NhomGiaDichVuKhamBenhBenhVienId,
                Ma = p.MaDichVu,
                TenDichVu = p.TenDichVu,
                LoaiGia = p.NhomGiaDichVuKhamBenhBenhVien?.Ten,
                DonGia = p.Gia,
                ThanhTien = 0,
                BHYTThanhToan = 0,
                BNThanhToan = 0,
                NoiThucHien = p.NoiDangKy?.Ten,
                NoiThucHienId = p.NoiDangKyId ?? 0,
                SoLuong = 1,
                TrangThaiDichVu = p.TrangThai.GetDescription(),
                TrangThaiDichVuId = (int)p.TrangThai,
                KiemTraBHYTXacNhan = p.BaoHiemChiTra != null,
                TenNguoiThucHien = p.BacSiDangKy?.User?.HoTen,
                NguoiThucHienId = p.BacSiDangKyId,
                KhongTinhPhi = p.KhongTinhPhi,
                DonGiaBaoHiem = p.DonGiaBaoHiem,
                DuocHuongBaoHiem = p.DuocHuongBaoHiem,
                YeuCauGoiDichVuId = p.YeuCauGoiDichVuId,
                ThoiGianChiDinh = p.ThoiDiemChiDinh,
                NguoiChiDinhDisplay = p.NhanVienChiDinh?.User?.HoTen,
                TenGoiDichVu = p.YeuCauGoiDichVu != null ? "Dịch vụ chọn từ gói: " + (p.YeuCauGoiDichVu.ChuongTrinhGoiDichVu.Ten + " - " + p.YeuCauGoiDichVu.ChuongTrinhGoiDichVu.TenGoiDichVu).ToUpper() : null
            }));

            var lstYeuCauDichVuKyThuats = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Include(o => o.NhomDichVuBenhVien).ThenInclude(o => o.NhomDichVuBenhVienCha)
                .Include(o => o.DichVuKyThuatBenhVien).ThenInclude(o => o.DichVuKyThuat)
                .Include(o => o.DichVuKyThuatBenhVien).ThenInclude(o => o.DichVuKyThuatBenhVienGiaBaoHiems)
                .Include(o => o.NhomGiaDichVuKyThuatBenhVien)
                .Include(o => o.NoiThucHien)
                .Include(o => o.NhanVienThucHien).ThenInclude(o => o.User)
                .Include(o => o.NhanVienChiDinh).ThenInclude(o => o.User)
                .Include(o => o.YeuCauGoiDichVu).ThenInclude(o => o.ChuongTrinhGoiDichVu)
                .Where(o => o.YeuCauKhamBenhId == yeuCauKhamBenhId && o.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                .OrderBy(o => o.CreatedOn).ToList();

            goiDichVuKhamBenh.AddRange(lstYeuCauDichVuKyThuats.Select(p => new LichSuKhamBenhGridVo
            {
                Id = p.Id,
                Nhom = (string.IsNullOrEmpty(p.NhomDichVuBenhVien.NhomDichVuBenhVienCha?.Ten) ? "" : p.NhomDichVuBenhVien.NhomDichVuBenhVienCha?.Ten + " - ") + p.NhomDichVuBenhVien.Ten,
                NhomId = (int)EnumNhomGoiDichVu.DichVuKyThuat,
                LoaiYeuCauDichVuId = p.DichVuKyThuatBenhVien?.Id,
                NhomGiaDichVuBenhVienId = p.NhomGiaDichVuKyThuatBenhVien?.Id ?? 0,
                Ma = p.DichVuKyThuatBenhVien?.Ma,
                MaGiaDichVu = p.DichVuKyThuatBenhVien?.DichVuKyThuat?.MaGia,
                MaTT37 = p.DichVuKyThuatBenhVien?.DichVuKyThuat?.Ma4350,
                TenDichVu = p.DichVuKyThuatBenhVien.Ten,
                LoaiGia = p.NhomGiaDichVuKyThuatBenhVien?.Ten,
                DonGia = p.Gia,
                TLBaoHiemThanhToan = p.DichVuKyThuatBenhVien?.DichVuKyThuatBenhVienGiaBaoHiems?.Where(o => o.TuNgay <= DateTime.Now && (o.DenNgay == null || DateTime.Now <= o.DenNgay)).FirstOrDefault()?.TiLeBaoHiemThanhToan ?? 0,
                ThanhTien = 0,
                BHYTThanhToan = 0,
                TLMG = p.TiLeUuDai ?? 0,
                SoTienMG = 0,
                BNThanhToan = 0,
                NoiThucHien = p.NoiThucHien?.Ten,
                NoiThucHienId = p.NoiThucHienId ?? 0,
                SoLuong = p.SoLan,
                TrangThaiDichVu = p.TrangThai.GetDescription(),
                TrangThaiDichVuId = (int)p.TrangThai,
                KiemTraBHYTXacNhan = p.BaoHiemChiTra != null,
                TenNguoiThucHien = p.NhanVienThucHien?.User.HoTen,
                NguoiThucHienId = p.NhanVienThucHienId,
                DonGiaBaoHiem = p.DonGiaBaoHiem,
                DuocHuongBaoHiem = p.DuocHuongBaoHiem,
                BenhPhamXetNghiem = p.BenhPhamXetNghiem,
                YeuCauGoiDichVuId = p.YeuCauGoiDichVuId,
                ThoiGianChiDinh = p.ThoiDiemChiDinh,
                NguoiChiDinhDisplay = p.NhanVienChiDinh?.User?.HoTen,
                TenGoiDichVu = p.YeuCauGoiDichVu != null ? "Dịch vụ chọn từ gói: " + (p.YeuCauGoiDichVu.ChuongTrinhGoiDichVu.Ten + " - " + p.YeuCauGoiDichVu.ChuongTrinhGoiDichVu.TenGoiDichVu).ToUpper() : null
            }));

            foreach (var itemx in goiDichVuKhamBenh)
            {
                decimal? thanhtien = itemx.DonGia * (decimal)itemx.SoLuong ?? 0;
                decimal? thanhTienBHTT = itemx.GiaBaoHiemThanhToan * (decimal)itemx.SoLuong ?? 0;

                itemx.ThanhTien = thanhtien;
                itemx.BHYTThanhToan = thanhTienBHTT;
                itemx.BNThanhToan = itemx.KhongTinhPhi != true ? (thanhtien - thanhTienBHTT) : 0; // to do nam ho KhongTinhPhi
            }
            return goiDichVuKhamBenh;
        }

        public List<LichSuKhamBenhGridVo> GetDataForGridAsyncChiDinDichVuKhacOld(long yeuCauKhamBenhId, long yeuCauTiepNhanId)
        {
            var goiDichVuKhamBenh = new List<LichSuKhamBenhGridVo>();
            var yeuCauTiepNhan = _yeuCauTiepNhanRepository.TableNoTracking
                       .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien)?.ThenInclude(p => p.DichVuKhamBenhBenhVienGiaBenhViens)
                       .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien)?.ThenInclude(p => p.DichVuKhamBenhBenhVienGiaBaoHiems)
                       .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien)?.ThenInclude(p => p.DoiTuongUuDaiDichVuKhamBenhBenhViens)
                       .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien)?.ThenInclude(p => p.DichVuKhamBenh)
                       .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien)
                       .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.NhomGiaDichVuKhamBenhBenhVien)
                       .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.NoiThucHien)
                       .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.NoiDangKy)
                       .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.BacSiThucHien)?.ThenInclude(p => p.User)
                       .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.BacSiDangKy)?.ThenInclude(p => p.User)

                       .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatVuBenhVienGiaBenhViens)
                       .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatBenhVienGiaBaoHiems)
                       .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DoiTuongUuDaiDichVuKyThuatBenhViens)
                       .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuat)
                       .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)
                       .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)
                       .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiThucHien)
                       .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomDichVuBenhVien).ThenInclude(p=>p.NhomDichVuBenhVienCha)
                       .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienThucHien)?.ThenInclude(p => p.User)
                       .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)

                       .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)?.ThenInclude(p => p.DichVuGiuongBenhVienGiaBenhViens)
                       .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)?.ThenInclude(p => p.DichVuGiuongBenhVienGiaBaoHiems)
                       .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)?.ThenInclude(p => p.DichVuGiuong)
                       .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)
                       .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.NhomGiaDichVuGiuongBenhVien)
                       .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.NoiThucHien)
                       .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
                       .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.DuocPhamBenhVien)?.ThenInclude(p => p.DuocPham)
                       .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.NoiChiDinh)
                       .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
                       .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.NoiCapThuoc)
                       .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.NhanVienCapThuoc)?.ThenInclude(p => p.User)

                       .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.VatTuBenhVien)?.ThenInclude(p => p.VatTus)
                       .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.NoiChiDinh)
                       .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
                       .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.NoiCapVatTu)
                       .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.NhanVienCapVatTu)?.ThenInclude(p => p.User)
                       .Where(p => p.Id == yeuCauTiepNhanId && p.YeuCauKhamBenhs.Any(c=>c.Id == yeuCauKhamBenhId));

            if (yeuCauTiepNhan.Any())
            {
                long userId = _userAgentHelper.GetCurrentUserId();
                int? nhomId = null;
                var lstYeuCauKhamBenhChiDinh = yeuCauTiepNhan.First().YeuCauKhamBenhs.Where(x => x.YeuCauKhamBenhTruocId == yeuCauKhamBenhId && (x.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham) // to do nam ho
                                                                                        ).OrderBy(x => x.CreatedOn);
                //// yêu cầu khám hiện tại
                var yeuCauKhamBenhHienTai = yeuCauTiepNhan.First().YeuCauKhamBenhs.First(x => x.Id == yeuCauKhamBenhId);



                goiDichVuKhamBenh.AddRange(lstYeuCauKhamBenhChiDinh.Select(p => new LichSuKhamBenhGridVo
                {
                    Id = p.Id,
                    Nhom = EnumNhomGoiDichVu.DichVuKhamBenh.GetDescription(),
                    NhomId = (int)EnumNhomGoiDichVu.DichVuKhamBenh,
                    LoaiYeuCauDichVuId = p.DichVuKhamBenhBenhVienId,
                    NhomGiaDichVuBenhVienId = p.NhomGiaDichVuKhamBenhBenhVienId,
                    Ma = p.MaDichVu,
                    TenDichVu = p.TenDichVu,
                    //TenLoaiGia = p.NhomGiaDichVuKhamBenhBenhVien?.Ten,
                    LoaiGia = p.NhomGiaDichVuKhamBenhBenhVien?.Ten,
                    DonGia = p.Gia,
                    //GiaBaoHiemThanhToan = p.GiaBaoHiemThanhToan ?? 0,
                    ThanhTien = 0,
                    BHYTThanhToan = 0,
                    BNThanhToan = 0,
                    NoiThucHien =  p.NoiDangKy?.Ten,
                    NoiThucHienId = p.NoiDangKyId ?? 0,
                    //TenNguoiThucHien = p.BacSiDangKy?.User?.HoTen,
                    //NguoiThucHienId = p.BacSiDangKyId,
                    SoLuong = 1,
                    TrangThaiDichVu = p.TrangThai.GetDescription(),
                    TrangThaiDichVuId = (int)p.TrangThai,
                    KiemTraBHYTXacNhan = p.BaoHiemChiTra != null,
                    TenNguoiThucHien = p.BacSiDangKy?.User?.HoTen,
                    NguoiThucHienId = p.BacSiDangKyId,
                    KhongTinhPhi = p.KhongTinhPhi,
                    DonGiaBaoHiem = p.DonGiaBaoHiem,
                    DuocHuongBaoHiem = p.DuocHuongBaoHiem,
                    YeuCauGoiDichVuId = p.YeuCauGoiDichVuId,
                    ThoiGianChiDinh = p.ThoiDiemChiDinh,
                    NguoiChiDinhDisplay = p.NhanVienChiDinh?.User?.HoTen,
                    TenGoiDichVu = p.YeuCauGoiDichVu != null ? "Dịch vụ chọn từ gói: " + (p.YeuCauGoiDichVu.ChuongTrinhGoiDichVu.Ten + " - " + p.YeuCauGoiDichVu.ChuongTrinhGoiDichVu.TenGoiDichVu).ToUpper() : null
                }));


                goiDichVuKhamBenh.AddRange(yeuCauKhamBenhHienTai.YeuCauDichVuKyThuats.Where(c => c.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).Select(p => new LichSuKhamBenhGridVo // to do nam ho
                {
                    Id = p.Id,
                    //Nhom = EnumNhomGoiDichVu.DichVuKyThuat.GetDescription(),
                    Nhom = (string.IsNullOrEmpty(p.NhomDichVuBenhVien.NhomDichVuBenhVienCha?.Ten) ? "" : p.NhomDichVuBenhVien.NhomDichVuBenhVienCha?.Ten + " - ") + p.NhomDichVuBenhVien.Ten,
                    NhomId = (int)EnumNhomGoiDichVu.DichVuKyThuat,
                    LoaiYeuCauDichVuId = p.DichVuKyThuatBenhVien?.Id,
                    NhomGiaDichVuBenhVienId = p.NhomGiaDichVuKyThuatBenhVien?.Id ?? 0,
                    Ma = p.DichVuKyThuatBenhVien?.Ma,
                    MaGiaDichVu = p.DichVuKyThuatBenhVien?.DichVuKyThuat?.MaGia,
                    MaTT37 = p.DichVuKyThuatBenhVien?.DichVuKyThuat?.Ma4350,
                    TenDichVu = p.DichVuKyThuatBenhVien.Ten,
                    LoaiGia = p.NhomGiaDichVuKyThuatBenhVien?.Ten,
                    DonGia = p.Gia,
                    //GiaBaoHiemThanhToan = p.GiaBaoHiemThanhToan ?? 0,
                    TLBaoHiemThanhToan = p.DichVuKyThuatBenhVien?.DichVuKyThuatBenhVienGiaBaoHiems?.Where(o => o.TuNgay <= DateTime.Now && (o.DenNgay == null || DateTime.Now <= o.DenNgay)).FirstOrDefault()?.TiLeBaoHiemThanhToan ?? 0,
                    ThanhTien = 0,
                    BHYTThanhToan = 0,
                    TLMG = p.TiLeUuDai ?? 0,
                    SoTienMG = 0,
                    BNThanhToan = 0,
                    NoiThucHien =  p.NoiThucHien?.Ten,
                    NoiThucHienId = p.NoiThucHienId ?? 0,
                    //NoiThucHienIdStr = String.Format("{0},{1}", p.NoiThucHienId, p.NhanVienThucHien?.User?.Id??0),
                    SoLuong = p.SoLan,
                    TrangThaiDichVu = p.TrangThai.GetDescription(),
                    TrangThaiDichVuId = (int)p.TrangThai,
                    KiemTraBHYTXacNhan = p.BaoHiemChiTra != null,
                    TenNguoiThucHien = p.NhanVienThucHien?.User.HoTen,
                    NguoiThucHienId = p.NhanVienThucHienId,
                    DonGiaBaoHiem = p.DonGiaBaoHiem,
                    DuocHuongBaoHiem = p.DuocHuongBaoHiem,
                    BenhPhamXetNghiem = p.BenhPhamXetNghiem,
                    YeuCauGoiDichVuId = p.YeuCauGoiDichVuId,
                    ThoiGianChiDinh = p.ThoiDiemChiDinh,
                    NguoiChiDinhDisplay = p.NhanVienChiDinh?.User?.HoTen,
                    TenGoiDichVu = p.YeuCauGoiDichVu != null ? "Dịch vụ chọn từ gói: " + (p.YeuCauGoiDichVu.ChuongTrinhGoiDichVu.Ten + " - " + p.YeuCauGoiDichVu.ChuongTrinhGoiDichVu.TenGoiDichVu).ToUpper() : null
                }));


                //goiDichVuKhamBenh.AddRange(yeuCauKhamBenhHienTai.YeuCauDichVuGiuongBenhViens.Where(c => (c.TrangThai != EnumTrangThaiGiuongBenh.DaHuy)).Select(p => new LichSuKhamBenhGridVo
                //{
                //    Id = p.Id,
                //    Nhom = EnumNhomGoiDichVu.DichVuGiuongBenh.GetDescription(),
                //    NhomId = (int)EnumNhomGoiDichVu.DichVuGiuongBenh,
                //    LoaiYeuCauDichVuId = p.DichVuGiuongBenhVien?.Id ?? 0,
                //    NhomGiaDichVuBenhVienId = p.NhomGiaDichVuGiuongBenhVien?.Id ?? 0,
                //    Ma = p.DichVuGiuongBenhVien?.Ma,
                //    TenDichVu = p.DichVuGiuongBenhVien?.Ten,
                //    LoaiGia = p.NhomGiaDichVuGiuongBenhVien?.Ten,
                //    DonGia = p.Gia,
                //    //GiaBaoHiemThanhToan = p.GiaBaoHiemThanhToan ?? 0,
                //    // update nè
                //    TLBaoHiemThanhToan = p.DichVuGiuongBenhVien?.DichVuGiuongBenhVienGiaBaoHiems?.Where(o => o.TuNgay <= DateTime.Now && (o.DenNgay == null || DateTime.Now <= o.DenNgay)).FirstOrDefault()?.TiLeBaoHiemThanhToan ?? 0,
                //    NoiThucHien = String.Format("{0} - {1}", p.NoiThucHien?.Ma ?? "", p.NoiThucHien?.Ten),
                //    NoiThucHienId = p.NoiThucHienId ?? 0,
                //    // NoiThucHienIdStr = String.Format("{0},{1}", p.NoiThucHien?.Id??0, p.NoiThucHien?.Ma),// NhanVienThucHien ko co 
                //    TrangThaiDichVu = p.TrangThai.GetDescription(),
                //    TrangThaiDichVuId = (int)p.TrangThai,
                //    // update nè
                //    ThanhTien = 0,
                //    BHYTThanhToan = 0,
                //    TLMG = 0, //update
                //    SoTienMG = 0, //update
                //    BNThanhToan = 0, //update
                //    SoLuong = 1, //update
                //    KiemTraBHYTXacNhan = p.BaoHiemChiTra != null,
                //    KhongTinhPhi = p.KhongTinhPhi,
                //    TenGiuongBenh = p.GiuongBenh != null ? p.GiuongBenh.Ten : "",
                //    DonGiaBaoHiem = p.DonGiaBaoHiem,
                //    DuocHuongBaoHiem = p.DuocHuongBaoHiem
                //}));


                //goiDichVuKhamBenh.AddRange(yeuCauKhamBenhHienTai.YeuCauDuocPhamBenhViens.Where(c => (c.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy)).Select(p => new LichSuKhamBenhGridVo
                //{
                //    Id = p.Id,
                //    Nhom = EnumNhomGoiDichVu.DuocPham.GetDescription(),
                //    NhomId = (int)EnumNhomGoiDichVu.DuocPham,
                //    LoaiYeuCauDichVuId = p.DuocPhamBenhVien.Id,
                //    Ma = p.DuocPhamBenhVien.Ma,
                //    TenDichVu = p.DuocPhamBenhVien.DuocPham?.Ten,
                //    LoaiGia = "Bệnh Viện", //doi update
                //    DonGia = p.DonGiaBan,
                //    //GiaBaoHiemThanhToan = p.GiaBaoHiemThanhToan ?? 0,
                //    TLBaoHiemThanhToan = p.TiLeBaoHiemThanhToan,
                //    ThanhTien = 0,
                //    BHYTThanhToan = 0, //DB update 
                //    TLMG = 0,    // update 
                //    SoTienMG = 0,  //update
                //    BNThanhToan = 0,  //update 
                //                      //KhoaPhongId = p.NoiCapThuocId,
                //    NhomChiPhiId = 1,// update 
                //    NoiThucHien = String.Format("{0}-{1}", p.NoiCapThuoc?.Ma ?? "", p.NhanVienCapThuoc?.User?.HoTen ?? ""),

                //    SoLuong = Convert.ToInt32(p.SoLuong),
                //    TrangThaiDichVu = p.TrangThai.GetDescription(),
                //    KiemTraBHYTXacNhan = p.BaoHiemChiTra != null,
                //    KhongTinhPhi = p.KhongTinhPhi,
                //    DonGiaBaoHiem = p.DonGiaBaoHiem,
                //    DuocHuongBaoHiem = p.DuocHuongBaoHiem
                //}));


                //goiDichVuKhamBenh.AddRange(yeuCauKhamBenhHienTai.YeuCauVatTuBenhViens.Where(c => (c.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy)).Select(p => new LichSuKhamBenhGridVo
                //{
                //    Id = p.Id,
                //    Nhom = EnumNhomGoiDichVu.VatTuTieuHao.GetDescription(),
                //    NhomId = (int)EnumNhomGoiDichVu.VatTuTieuHao,
                //    LoaiYeuCauDichVuId = p.VatTuBenhVien.Id,
                //    NhomGiaDichVuBenhVienId = p.VatTuBenhVien.VatTus.NhomVatTuId,
                //    Ma = p.VatTuBenhVien.VatTus?.Ma,
                //    TenDichVu = p.VatTuBenhVien.VatTus?.Ten,
                //    LoaiGia = "Bệnh Viện", //doi DB update 
                //    DonGia = p.DonGiaBan,
                //    ThanhTien = p.GiaBan,
                //    BHYTThanhToan = 0,  //doi DB update 
                //    TLMG = 0,   //doi DB update 
                //    SoTienMG = 0,  //doi DB update 
                //    BNThanhToan = 0,  //doi DB update 
                //    NoiThucHien = String.Format("{0}-{1}", p.NoiCapVatTu?.Ma ?? "", p.NhanVienCapVatTu?.User?.HoTen ?? ""),
                //    SoLuong = Convert.ToInt32(p.SoLuong),
                //    TrangThaiDichVu = p.TrangThai.GetDescription(),
                //    KiemTraBHYTXacNhan = p.BaoHiemChiTra != null,
                //    KhongTinhPhi = p.KhongTinhPhi,
                //    DonGiaBaoHiem = p.DonGiaBaoHiem,
                //    DuocHuongBaoHiem = p.DuocHuongBaoHiem
                //}));




            }
            foreach (var itemx in goiDichVuKhamBenh)
            {
                decimal? thanhtien = itemx.DonGia * (decimal)itemx.SoLuong ?? 0;
                decimal? thanhTienBHTT = itemx.GiaBaoHiemThanhToan * (decimal)itemx.SoLuong ?? 0;

                itemx.ThanhTien = thanhtien;
                itemx.BHYTThanhToan = thanhTienBHTT;
                itemx.BNThanhToan = itemx.KhongTinhPhi != true ? (thanhtien - thanhTienBHTT) : 0; // to do nam ho KhongTinhPhi
            }
            return goiDichVuKhamBenh;
        }

        private static decimal ToPercentages(int percent)
        {
            return (decimal)(percent) / 100;
        }
        #endregion
        #region KetLuan
        public async Task<List<KetLuanGridVo>> GetDataForGridAsyncKetLuan(long yeuCauKhamBenhId)
        {
            var queryKhamChuyenKhoa = BaseRepository.TableNoTracking
                .Include(o => o.YeuCauTiepNhan).ThenInclude(o => o.NoiTiepNhan).ThenInclude(o => o.KhoaPhong)
                .Include(o => o.Icdchinh).Include(o => o.YeuCauKhamBenhICDKhacs)
                .Include(o => o.YeuCauTiepNhan).ThenInclude(o => o.NhanVienTiepNhan).ThenInclude(o => o.User)
                .Include(o => o.IcdchinhId)
                .Include(o => o.YeuCauTiepNhan)
                .Where(o => o.YeuCauKhamBenhTruocId == yeuCauKhamBenhId)
                .Select(k => new KetLuanGridVo
                {
                    TenDichVu = k.TenDichVu,
                    LoaiGia = k.NhomGiaDichVuKhamBenhBenhVien.Ten,
                    DuocHuongBaoHiem = k.DuocHuongBaoHiem == true ? "Có" : "Không",
                    NoiThucHien = k.NoiChiDinhId != null ? k.NoiChiDinh.Ten : "",
                    SoLuong = 1,
                    DonGia = k.Gia,
                    ThanhTien = k.Gia,
                }).FirstOrDefault();

            var queryData = await BaseRepository.TableNoTracking
                  .Include(o => o.YeuCauTiepNhan).ThenInclude(o => o.NoiTiepNhan).ThenInclude(o => o.KhoaPhong)
                  .Include(o => o.Icdchinh).Include(o => o.YeuCauKhamBenhICDKhacs)
                  .Include(o => o.YeuCauDichVuKyThuats)
                  .Include(o => o.YeuCauTiepNhan).ThenInclude(o => o.NhanVienTiepNhan).ThenInclude(o => o.User)
                  .Include(o => o.YeuCauTiepNhan)
                  .Where(o => o.Id == yeuCauKhamBenhId)
                  .Select(s => new KetLuanGridVo
                  {
                      Id = s.Id,
                      ChuanDoanICDChinh = s.Icdchinh.TenTiengViet,
                      ChuanDoanICDPhu = s.YeuCauKhamBenhICDKhacs != null ? s.YeuCauKhamBenhICDKhacs.Where(k => k.YeuCauKhamBenhId == s.Id).ToList() : null, // xem lại
                      HuongDieuTri = s.HuongDieuTri,
                      KetQuaDieuTri = s.KetQuaDieuTri.GetDescription(),
                      GhiChu = s.GhiChuTaiKham,
                      TaiKham = s.CoTaiKham,
                      NgayTaiKham = s.NgayTaiKham,
                      NgayHenKhamDisplay = s.NgayTaiKham != null ? s.NgayTaiKham.Value.ApplyFormatDate() : null,
                      KhamChuyenKhoaTiepTheo = s.CoKhamChuyenKhoaTiepTheo,
                      TenDichVu = queryKhamChuyenKhoa != null ? queryKhamChuyenKhoa.TenDichVu : null,

                      LoaiGia = queryKhamChuyenKhoa != null ? queryKhamChuyenKhoa.LoaiGia : null,
                      DuocHuongBaoHiem = queryKhamChuyenKhoa != null ? queryKhamChuyenKhoa.DuocHuongBaoHiem : null,
                      NoiThucHien = queryKhamChuyenKhoa != null ? queryKhamChuyenKhoa.NoiThucHien : null,
                      TinhTrangRaVien = s.TinhTrangRaVien.GetDescription(),
                      SoLuong = queryKhamChuyenKhoa != null ? queryKhamChuyenKhoa.SoLuong : null,
                      DonGia = queryKhamChuyenKhoa != null ? queryKhamChuyenKhoa.DonGia : null,
                      ThanhTien = queryKhamChuyenKhoa != null ? queryKhamChuyenKhoa.ThanhTien : null,

                      CoKeToa = s.CoKeToa,
                      // update 11/6/2020
                      TomTatKetQuaCLS = s.TomTatKetQuaCLS,
                      ChuThichChanDoanICDChinh = s.GhiChuICDChinh,
                      CachGiaiQuyet = s.CachGiaiQuyet,
                      NhapVien = s.CoNhapVien,
                      Khoa = s.KhoaPhongNhapVien != null ? s.KhoaPhongNhapVien.Ten : null,
                      LyDoNhapVien = s.LyDoNhapVien,
                      ChuyenVien = s.CoChuyenVien,
                      BenhVienChuyenDen = s.BenhVienChuyenVienId != null ? s.BenhVienChuyenVien.Ten : null,
                      LyDoChuyenVien = s.LyDoChuyenVien,
                      CoTuVong = s.CoTuVong,
                      // update ngày 25/6/2020
                      //TODO: Nam check lai.
                      DieuTriNgoaiTru = s.CoDieuTriNgoaiTru,
                      TenDichVuKyThuat = s.YeuCauDichVuKyThuats != null ? s.YeuCauDichVuKyThuats.Where(p => p.DieuTriNgoaiTru == true).Select(p => p.TenDichVu).FirstOrDefault() : null,
                      SoLanDieuTri = s.YeuCauDichVuKyThuats != null ? s.YeuCauDichVuKyThuats.Where(p => p.DieuTriNgoaiTru == true).Select(p => p.SoLan).FirstOrDefault() : 0,
                      ThoiGianBatDauDieuTri = s.YeuCauDichVuKyThuats != null ? s.YeuCauDichVuKyThuats.Where(p => p.DieuTriNgoaiTru == true)
                                                                    .Select(p => p.ThoiDiemBatDauDieuTri.Value.ApplyFormatDate()).FirstOrDefault() : null,
                      //SoLanDieuTri = s.YeuCauDichVuKyThuats.Any(p => p.YeuCauKhamBenh.CoDieuTriNgoaiTru == true) ? s.YeuCauDichVuKyThuats.Where(p => p.YeuCauKhamBenh.CoDieuTriNgoaiTru == true).FirstOrDefault().SoLan != 0 ? s.YeuCauDichVuKyThuats.Where(p => p.YeuCauKhamBenh.CoDieuTriNgoaiTru == true).FirstOrDefault().SoLan : 0 : 0,
                      //ThoiGianBatDauDieuTri = s.YeuCauDichVuKyThuats.Any(p => p.YeuCauKhamBenh.CoDieuTriNgoaiTru == true) ? s.YeuCauDichVuKyThuats.Where(p => p.YeuCauKhamBenh.CoDieuTriNgoaiTru == true).FirstOrDefault().ThoiDiemBatDauDieuTri.Value.ApplyFormatDate() ?? "" : ""
                      // update 27/8/2020
                      TinhTrangChuyenVien = s.TinhTrangBenhNhanChuyenVien != null ? s.TinhTrangBenhNhanChuyenVien : "",
                      ThoiDiemChuyenVien = s.ThoiDiemChuyenVien != null ? s.ThoiDiemChuyenVien.Value.ApplyFormatDateTimeSACH() : "",
                      NhanVienHoTong = s.NhanVienHoTongChuyenVienId != null ? s.NhanVienHoTongChuyenVien.User.HoTen : "",
                      PhuongTienDiChuyen = s.PhuongTienChuyenVien != null ? s.PhuongTienChuyenVien : "",
                  }).ToListAsync();
            return queryData;
        }
        public async Task<List<KetLuanGridVo>> GetChuanDoanICDPhu(long yeuCauKhamBenhId)
        {
            var queryData = await _yeuCauKhamBenhICDKhacRepository.TableNoTracking
                            .Include(yckb => yckb.ICDId)
                            .Where(o => o.YeuCauKhamBenhId == yeuCauKhamBenhId)
                            .Select(s => new KetLuanGridVo
                            {
                                Id = s.Id,
                                ICDPhu = s.ICD.TenTiengViet,
                                ChuThichChanDoanICDPhu = s.GhiChu
                            }).ToListAsync();
            return queryData;
        }
        public GridDataSource GetDataForGridAsyncToaMau(QueryInfo queryInfo)
        {
            var query = _yeuCauKhamBenhDonThuocRepository.TableNoTracking
                .Include(o => o.YeuCauKhamBenh).ThenInclude(o => o.Icdchinh)
                .Include(o => o.YeuCauKhamBenh).ThenInclude(o => o.YeuCauTiepNhan).ThenInclude(o => o.NhanVienTiepNhan).ThenInclude(o => o.User)
                .Include(o => o.YeuCauKhamBenhDonThuocChiTiets)
                //.Where(o => o.YeuCauKhamBenhId == long.Parse(queryInfo.AdditionalSearchString))
                .Select(s => new KetLuanGridVo
                {
                    Id = s.Id,
                    ChuanDoanICDChinh = s.YeuCauKhamBenh.Icdchinh.TenTiengViet, // xem lại
                    BacSyKeToa = s.YeuCauKhamBenh.YeuCauTiepNhan.NhanVienTiepNhan.User.HoTen, // xem lại
                    SuDung = s.YeuCauKhamBenhDonThuocChiTiets.Where(x => x.YeuCauKhamBenhDonThuocId == s.Id).FirstOrDefault().LieuLuongCachDung  // xem lại

                });

            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
               .Take(queryInfo.Take).ToArray();
            var countTask = query.Count();

            return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
        }
        public GridDataSource GetDataForGridAsyncChildToaMau(QueryInfo queryInfo)
        {
            long.TryParse(queryInfo.AdditionalSearchString, out var YeuCauKhamBenhDonThuocId);
            BuildDefaultSortExpression(queryInfo);
            var query = _yeuCauKhamBenhDonThuocChiTietRepository.TableNoTracking
                .Include(o => o.YeuCauKhamBenhDonThuoc).ThenInclude(o => o.YeuCauKhamBenh)
                .Include(o => o.DuocPham).ThenInclude(o => o.DonViTinh)
                .Include(o => o.DuongDung)
                .Where(o => o.YeuCauKhamBenhDonThuocId == YeuCauKhamBenhDonThuocId)
                .Select(s => new DanhSachDaKhamGridVo
                {
                    Id = s.Id,
                    Duoc = s.DuocPham.Ten,
                    HoatChat = s.DuocPham.HoatChat,
                    DonViTinh = s.DuocPham.DonViTinh.Ten,
                    Sang = s.DungSang,
                    Trua = s.DungTrua,
                    Toi = s.DungToi,
                    SoNgay = s.SoNgayDung,
                    SoLuong = s.SoLuong,
                    DuongDung = s.DuongDung.Ten,
                    GhiChu = s.GhiChu,
                });
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
               .Take(queryInfo.Take).ToArray();
            var countTask = query.Count();

            return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
        }

        public GridDataSource GetDataForGridAsyncChildToaThuoc(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            var lstDuocPham = _yeuCauKhamBenhDonThuocChiTietRepository.TableNoTracking
                             .Include(o => o.DuocPham)
                             .Select(p => p.DuocPham.MaHoatChat).ToList();
            var lstADR = _aDRRepository.TableNoTracking
                           .Include(o => o.ThuocHoacHoatChat1)
                           .Include(o => o.ThuocHoacHoatChat2)
                           .Where(o => o.ThuocHoacHoatChat1Id == o.ThuocHoacHoatChat1.Id
                                        && o.ThuocHoacHoatChat2Id == o.ThuocHoacHoatChat2.Id)
                           .Select(s => new MaHoatChatGridVo
                           {
                               Ten1 = s.ThuocHoacHoatChat1.Ten,
                               Ten2 = s.ThuocHoacHoatChat2.Ten,
                               MaHoatChat1 = s.ThuocHoacHoatChat1.Ma,
                               MaHoatChat2 = s.ThuocHoacHoatChat2.Ma
                           }).ToList();
            long.TryParse(queryInfo.AdditionalSearchString, out var YeuCauKhamBenhId);
            var query = _yeuCauKhamBenhDonThuocChiTietRepository.TableNoTracking
                .Include(o => o.YeuCauKhamBenhDonThuoc).ThenInclude(o => o.YeuCauKhamBenh)
                .Include(o => o.DuocPham).ThenInclude(o => o.DonViTinh)
                .Include(o => o.DuongDung)
                .Where(o => o.YeuCauKhamBenhDonThuoc.YeuCauKhamBenhId == YeuCauKhamBenhId)
                .Select(s => new ToaThuocGridVo
                {
                    Id = s.Id,
                    Duoc = s.DuocPham.Ten,
                    HoatChat = s.DuocPham.HoatChat,
                    DonViTinh = s.DuocPham.DonViTinh.Ten,
                    SangDisplay = s.DungSang == null ? null : s.DungSang.FloatToStringFraction(),
                    TruaDisplay = s.DungTrua == null ? null : s.DungTrua.FloatToStringFraction(),
                    ChieuDisplay = s.DungChieu == null ? null : s.DungChieu.FloatToStringFraction(),
                    ToiDisplay = s.DungToi == null ? null : s.DungToi.FloatToStringFraction(),
                    ThoiGianDungSangDisplay = s.ThoiGianDungSang == null ? null : "(" + s.ThoiGianDungSang.Value.ConvertIntSecondsToTime12h() + ")",
                    ThoiGianDungTruaDisplay = s.ThoiGianDungTrua == null ? null : "(" + s.ThoiGianDungTrua.Value.ConvertIntSecondsToTime12h() + ")",
                    ThoiGianDungChieuDisplay = s.ThoiGianDungChieu == null ? null : "(" + s.ThoiGianDungChieu.Value.ConvertIntSecondsToTime12h() + ")",
                    ThoiGianDungToiDisplay = s.ThoiGianDungToi == null ? null : "(" + s.ThoiGianDungToi.Value.ConvertIntSecondsToTime12h() + ")",
                    SoNgay = s.SoNgayDung,
                    SoLuong = s.SoLuong,
                    DuongDung = s.DuongDung.Ten,
                    GhiChu = s.GhiChu,
                    DonGiaNhap = s.LaDuocPhamBenhVien && s.DonThuocThanhToanChiTiets.FirstOrDefault() != null ? s.DonThuocThanhToanChiTiets.First().DonGiaNhap : 0,
                    DonGia = s.LaDuocPhamBenhVien && s.DonThuocThanhToanChiTiets.FirstOrDefault() != null ? s.DonThuocThanhToanChiTiets.First().DonGiaBan : 0,
                    TiLeTheoThapGia = s.LaDuocPhamBenhVien && s.DonThuocThanhToanChiTiets.FirstOrDefault() != null ? s.DonThuocThanhToanChiTiets.First().TiLeTheoThapGia : 0,
                    VAT = s.LaDuocPhamBenhVien && s.DonThuocThanhToanChiTiets.FirstOrDefault() != null ? s.DonThuocThanhToanChiTiets.First().VAT : 0,
                    ThuocBHYT = s.YeuCauKhamBenhDonThuoc.YeuCauKhamBenh.DuocHuongBaoHiem == true ? "Có" : "Không",
                    TuongTacThuoc = GetTuongTac(s.DuocPham.MaHoatChat, lstDuocPham, lstADR),
                    TenLoaiThuoc = s.YeuCauKhamBenhDonThuoc.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT ? "Thuốc BHYT" : "Thuốc Không BHYT",
                    LoaiThuocId =(int)s.YeuCauKhamBenhDonThuoc.LoaiDonThuoc,
                    DiUngThuocDisplay = s.YeuCauKhamBenhDonThuoc.YeuCauKhamBenh.YeuCauTiepNhan.BenhNhan.BenhNhanDiUngThuocs.Any(diung => diung.TenDiUng == s.DuocPham.MaHoatChat && diung.LoaiDiUng == Enums.LoaiDiUng.Thuoc) ? "Có" : "Không",
                });
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
               .Take(queryInfo.Take).ToArray();
            var countTask = query.Count();

            return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
        }
        public List<ToaThuocGridVo> GetDataForGridAsyncChildToaThuocList(long yeuCauKhamBenhId)
        {
            var lstDuocPham = _yeuCauKhamBenhDonThuocChiTietRepository.TableNoTracking
                             .Include(o => o.DuocPham)
                             .Select(p => p.DuocPham.MaHoatChat).ToList();

            var lstADR = _aDRRepository.TableNoTracking
                           .Include(o => o.ThuocHoacHoatChat1)
                           .Include(o => o.ThuocHoacHoatChat2)
                           .Where(o => o.ThuocHoacHoatChat1Id == o.ThuocHoacHoatChat1.Id
                                        && o.ThuocHoacHoatChat2Id == o.ThuocHoacHoatChat2.Id)
                           .Select(s => new MaHoatChatGridVo
                           {
                               Ten1 = s.ThuocHoacHoatChat1.Ten,
                               Ten2 = s.ThuocHoacHoatChat2.Ten,
                               MaHoatChat1 = s.ThuocHoacHoatChat1.Ma,
                               MaHoatChat2 = s.ThuocHoacHoatChat2.Ma
                           }).ToList();
            var query = _yeuCauKhamBenhDonThuocChiTietRepository.TableNoTracking
                .Include(o => o.YeuCauKhamBenhDonThuoc).ThenInclude(o => o.YeuCauKhamBenh)
                .Include(o => o.DuocPham).ThenInclude(o => o.DonViTinh)
                .Include(o => o.DuongDung)
                .Where(o => o.YeuCauKhamBenhDonThuoc.YeuCauKhamBenhId == yeuCauKhamBenhId)
                .Select(s => new ToaThuocGridVo
                {
                    Id = s.Id,
                    Duoc = s.DuocPham.Ten,
                    HoatChat = s.DuocPham.HoatChat,
                    DonViTinh = s.DuocPham.DonViTinh.Ten,
                    SangDisplay = s.DungSang == null ? null : s.DungSang.FloatToStringFraction(),
                    TruaDisplay = s.DungTrua == null ? null : s.DungTrua.FloatToStringFraction(),
                    ChieuDisplay = s.DungChieu == null ? null : s.DungChieu.FloatToStringFraction(),
                    ToiDisplay = s.DungToi == null ? null : s.DungToi.FloatToStringFraction(),
                    ThoiGianDungSangDisplay = s.ThoiGianDungSang == null ? null : "(" + s.ThoiGianDungSang.Value.ConvertIntSecondsToTime12h() + ")",
                    ThoiGianDungTruaDisplay = s.ThoiGianDungTrua == null ? null : "(" + s.ThoiGianDungTrua.Value.ConvertIntSecondsToTime12h() + ")",
                    ThoiGianDungChieuDisplay = s.ThoiGianDungChieu == null ? null : "(" + s.ThoiGianDungChieu.Value.ConvertIntSecondsToTime12h() + ")",
                    ThoiGianDungToiDisplay = s.ThoiGianDungToi == null ? null : "(" + s.ThoiGianDungToi.Value.ConvertIntSecondsToTime12h() + ")",
                    SoNgay = s.SoNgayDung,
                    SoLuong = s.SoLuong,
                    DuongDung = s.DuongDung.Ten,
                    GhiChu = s.GhiChu,
                    DonGiaNhap = s.LaDuocPhamBenhVien && s.DonThuocThanhToanChiTiets.FirstOrDefault() != null ? s.DonThuocThanhToanChiTiets.First().DonGiaNhap : 0,
                    DonGia = s.LaDuocPhamBenhVien && s.DonThuocThanhToanChiTiets.FirstOrDefault() != null ? s.DonThuocThanhToanChiTiets.First().DonGiaBan : 0,
                    TiLeTheoThapGia = s.LaDuocPhamBenhVien && s.DonThuocThanhToanChiTiets.FirstOrDefault() != null ? s.DonThuocThanhToanChiTiets.First().TiLeTheoThapGia : 0,
                    VAT = s.LaDuocPhamBenhVien && s.DonThuocThanhToanChiTiets.FirstOrDefault() != null ? s.DonThuocThanhToanChiTiets.First().VAT : 0,
                    //BaoHiemTra = s.YeuCauKhamBenhDonThuoc.YeuCauKhamBenh.BaoHiemChiTra == true ? "Có" : "Không",
                    //BHChiTra = s.YeuCauKhamBenhDonThuoc.YeuCauKhamBenh.BaoHiemChiTra == true ? s.YeuCauKhamBenhDonThuoc.YeuCauKhamBenh.GiaBaoHiemThanhToan : 0,
                    //BNChiTra = ((s.LaDuocPhamBenhVien ? s.DonThuocThanhToanChiTiets.First().Gia : null) * Convert.ToDecimal(s.SoLuong)) - (s.YeuCauKhamBenhDonThuoc.YeuCauKhamBenh.GiaBaoHiemThanhToan * s.YeuCauKhamBenhDonThuoc.YeuCauKhamBenh.TiLeUuDai),
                    ThuocBHYT = s.YeuCauKhamBenhDonThuoc.YeuCauKhamBenh.DuocHuongBaoHiem == true ? "Có" : "Không",
                    TuongTacThuoc = GetTuongTac(s.DuocPham.MaHoatChat, lstDuocPham, lstADR),
                    DiUngThuocDisplay = s.YeuCauKhamBenhDonThuoc.YeuCauKhamBenh.YeuCauTiepNhan.BenhNhan.BenhNhanDiUngThuocs.Any(diung => diung.TenDiUng == s.DuocPham.MaHoatChat && diung.LoaiDiUng == Enums.LoaiDiUng.Thuoc) ? "Có" : "Không",
                    LoiDan = s.YeuCauKhamBenhDonThuoc.GhiChu
                });

            return query.ToList();
        }

        #endregion
        #region PTTT
        public async Task<ThongTinPhauThuat> GetLichSuThongTinTuongTrinh(long yeuCauKhamBenhId)
        {
            var thongTinPhauThuat = await _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Include(w => w.YeuCauKhamBenh).ThenInclude(w => w.NoiThucHien).ThenInclude(w => w.KhoaPhong)
                .Where(p => p.YeuCauKhamBenhId == yeuCauKhamBenhId)
                .Select(p => new ThongTinPhauThuat
                {
                    TenKhoaPhong = p.YeuCauKhamBenh.NoiThucHien.KhoaPhong.Ten,
                    ChanDoanVaoKhoa = p.YeuCauKhamBenh.Icdchinh != null ? p.YeuCauKhamBenh.Icdchinh.TenTiengViet : null,
                    MoTa = p.YeuCauKhamBenh.GhiChuICDChinh
                }).FirstOrDefaultAsync();

            return thongTinPhauThuat;
        }
        public async Task<List<KhamBenhPhauThuatThuThuatGridVo>> GetLichSuListPhauThuatThuThuat(long yeuCauKhamBenhId)
        {
            var phauThuatThuThuats = await _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Include(x => x.YeuCauDichVuKyThuatTuongTrinhPTTT).ThenInclude(x => x.ICDTruocPhauThuat)
                .Include(x => x.YeuCauDichVuKyThuatTuongTrinhPTTT).ThenInclude(x => x.ICDSauPhauThuat)
                .Include(x => x.YeuCauDichVuKyThuatTuongTrinhPTTT).ThenInclude(x => x.PhuongPhapVoCam)
                .Where(p => p.YeuCauKhamBenhId == yeuCauKhamBenhId && p.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat && p.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                .Select(o =>
                    new KhamBenhPhauThuatThuThuatGridVo
                    {
                        Ten = o.TenDichVu,
                        Id = o.Id,
                        ICDTruocPhauThuatId = o.YeuCauDichVuKyThuatTuongTrinhPTTT != null ? o.YeuCauDichVuKyThuatTuongTrinhPTTT.ICDTruocPhauThuatId : null,
                        ICDTruocPhauThuatDisplay = o.YeuCauDichVuKyThuatTuongTrinhPTTT.ICDTruocPhauThuat != null ? o.YeuCauDichVuKyThuatTuongTrinhPTTT.ICDTruocPhauThuat.Ma + " - " + o.YeuCauDichVuKyThuatTuongTrinhPTTT.ICDTruocPhauThuat.TenTiengViet : null,
                        ICDSauPhauThuatId = o.YeuCauDichVuKyThuatTuongTrinhPTTT != null ? o.YeuCauDichVuKyThuatTuongTrinhPTTT.ICDSauPhauThuatId : null,
                        ICDSauPhauThuatDisplay = o.YeuCauDichVuKyThuatTuongTrinhPTTT.ICDSauPhauThuat != null ? o.YeuCauDichVuKyThuatTuongTrinhPTTT.ICDSauPhauThuat.Ma + " - " + o.YeuCauDichVuKyThuatTuongTrinhPTTT.ICDSauPhauThuat.TenTiengViet : null,
                        GhiChuICDTruocPhauThuat = o.YeuCauDichVuKyThuatTuongTrinhPTTT.GhiChuICDTruocPhauThuat,
                        GhiChuICDSauPhauThuat = o.YeuCauDichVuKyThuatTuongTrinhPTTT.GhiChuICDSauPhauThuat,
                        PhuongPhapPhauThuatThuThuatKey = o.YeuCauDichVuKyThuatTuongTrinhPTTT.MaPhuongPhapPTTT,
                        PhuongPhapPhauThuatThuThuatDisplay = o.YeuCauDichVuKyThuatTuongTrinhPTTT.MaPhuongPhapPTTT + " - " + o.YeuCauDichVuKyThuatTuongTrinhPTTT.TenPhuongPhapPTTT,
                        LoaiPTTTEnum = o.YeuCauDichVuKyThuatTuongTrinhPTTT.LoaiPhauThuatThuThuat,
                        LoaiPTTTDisplay = o.YeuCauDichVuKyThuatTuongTrinhPTTT.LoaiPhauThuatThuThuat != null ? o.YeuCauDichVuKyThuatTuongTrinhPTTT.LoaiPhauThuatThuThuat.GetDescription() : string.Empty,
                        PhuongPhapVoCamId = o.YeuCauDichVuKyThuatTuongTrinhPTTT.PhuongPhapVoCam != null ? o.YeuCauDichVuKyThuatTuongTrinhPTTT.PhuongPhapVoCamId : null,
                        PhuongPhapVoCamDisplay = o.YeuCauDichVuKyThuatTuongTrinhPTTT.PhuongPhapVoCam != null ? o.YeuCauDichVuKyThuatTuongTrinhPTTT.PhuongPhapVoCam.Ma + " - " + o.YeuCauDichVuKyThuatTuongTrinhPTTT.PhuongPhapVoCam.Ten : string.Empty,
                        TinhHinhPttt = o.YeuCauDichVuKyThuatTuongTrinhPTTT.TinhHinhPTTT,
                        TinhHinhPtttDisplay = o.YeuCauDichVuKyThuatTuongTrinhPTTT.TinhHinhPTTT != null ? o.YeuCauDichVuKyThuatTuongTrinhPTTT.TinhHinhPTTT.GetDescription() : string.Empty,
                        TaiBienPttt = o.YeuCauDichVuKyThuatTuongTrinhPTTT.TaiBienPTTT,
                        TaiBienPtttDisplay = o.YeuCauDichVuKyThuatTuongTrinhPTTT.TaiBienPTTT != null ? o.YeuCauDichVuKyThuatTuongTrinhPTTT.TaiBienPTTT.GetDescription() : string.Empty,
                        TuVongPttt = o.YeuCauDichVuKyThuatTuongTrinhPTTT.TuVongTrongPTTT,
                        TuVongPtttDisplay = o.YeuCauDichVuKyThuatTuongTrinhPTTT.TuVongTrongPTTT != null ? o.YeuCauDichVuKyThuatTuongTrinhPTTT.TuVongTrongPTTT.GetDescription() : string.Empty,
                        TrinhTuPttt = o.YeuCauDichVuKyThuatTuongTrinhPTTT.TrinhTuPhauThuat,
                        NhanVienThucHienDisplay = o.NhanVienThucHien != null ? o.NhanVienThucHien.User.HoTen : string.Empty,
                        ListLuocDoPhauThuatThuThuatResultVo = o.YeuCauDichVuKyThuatTuongTrinhPTTT.YeuCauDichVuKyThuatLuocDoPhauThuats.Any() ? o.YeuCauDichVuKyThuatTuongTrinhPTTT.YeuCauDichVuKyThuatLuocDoPhauThuats.ToList() : null
                    }).ToListAsync();

            return phauThuatThuThuats;
        }
        #endregion

    }
}