using Camino.Core.Data;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Newtonsoft.Json;
using System.Globalization;
using static Camino.Core.Domain.Enums;
using System.Collections.Generic;
using Camino.Core.Domain;
using Camino.Services.ExportImport.Help;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject.YeuCauKSNK;

namespace Camino.Services.YeuCauLinhKSNK
{
    public partial class YeuCauLinhKSNKService
    {
        #region  Ds linh duoc phẩm, vật tư
        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool print)
        {

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                BuildDefaultSortExpression(queryInfo);
                var queryString = JsonConvert.DeserializeObject<SeachNgay>(queryInfo.AdditionalSearchString);

                var queryDangChoGoi = DangChoGoi(queryInfo).Union(DangChoGoiDP(queryInfo));
                var queryDangChoDuyet = DangChoDuyet(queryInfo).Union(DangChoDuyetDP(queryInfo));
                var queryTuChoiDuyet = TuChoiDuyet(queryInfo).Union(TuChoiDuyetDP(queryInfo));
                var queryDaDuyet = DaDuyet(queryInfo).Union(DaDuyetDP(queryInfo));

                var query = new List<DSLinhKSNKGridVo>();

                if (queryString.DangChoGoi == true)
                {
                    query = queryDangChoGoi.ToList();
                }
                if (queryString.DangChoDuyet == true)
                {
                    query = query.Union(queryDangChoDuyet).ToList();
                }
                if (queryString.TuChoiDuyet == true)
                {
                    query = query.Union(queryTuChoiDuyet).ToList();
                }
                if (queryString.DaDuyet == true)
                {
                    query = query.Union(queryDaDuyet).ToList();
                }
                if (queryString.DangChoGoi == false && queryString.DangChoDuyet == false && queryString.TuChoiDuyet == true && queryString.DaDuyet == false)
                {
                    query = new List<DSLinhKSNKGridVo>();
                    query = queryDangChoGoi.Union(queryDangChoDuyet).Union(queryTuChoiDuyet).Union(queryDaDuyet).ToList();
                }

                var dataOrderBy = query.Where(d=>d.LoaiPhieuLinh != EnumLoaiPhieuLinh.LinhChoBenhNhan).AsQueryable();
                if (queryInfo.Sort.Count == 1 && string.IsNullOrEmpty(queryInfo.Sort[0].Dir))
                {
                    queryInfo.Sort[0].Dir = "asc";
                    queryInfo.Sort[0].Field = "DuocDuyet";
                }
                var quaythuoc = dataOrderBy.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
                var countTask = dataOrderBy.Count();

                return new GridDataSource { Data = quaythuoc, TotalRowCount = countTask };

            }
            else
            {
                BuildDefaultSortExpression(queryInfo);
                var queryDangChoGui = BaseRepository.TableNoTracking.Where(x => x.DuocDuyet == null && x.DaGui != true).Select(s => new DSLinhKSNKGridVo()
                {
                    Id = s.Id,
                    MaPL = s.SoPhieu,
                    Loai = s.LoaiPhieuLinh.GetDescription(),
                    LoaiPhieuLinh = s.LoaiPhieuLinh,
                    NguoiYeuCau = s.NhanVienYeuCau.User.HoTen,
                    LinhTuKho = s.KhoXuat.Ten,
                    LinhVeKhoId = s.KhoXuatId,
                    LinhVeKho = s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan ? "" : s.KhoNhap.Ten,
                    NgayYeuCau = s.NgayYeuCau,
                    TinhTrang = "Đang chờ duyệt",
                    Nguoiduyet = s.NhanVienDuyet.User.HoTen,
                    NgayDuyet = s.NgayDuyet != null ? s.NgayDuyet : (DateTime?)null,
                    NgayDuyetHienThi = s.NgayDuyet != null ? Convert.ToDateTime(s.NgayDuyet).ApplyFormatDateTimeSACH() : "",
                    DuocDuyet = s.DuocDuyet
                });
                var queryDangChoDuyet = BaseRepository.TableNoTracking.Where(x => x.DuocDuyet == null).Select(s => new DSLinhKSNKGridVo()
                {
                    Id = s.Id,
                    MaPL = s.SoPhieu,
                    Loai = s.LoaiPhieuLinh.GetDescription(),
                    LoaiPhieuLinh = s.LoaiPhieuLinh,
                    NguoiYeuCau = s.NhanVienYeuCau.User.HoTen,
                    LinhTuKho = s.KhoXuat.Ten,
                    LinhVeKhoId = s.KhoXuatId,
                    LinhVeKho = s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan ? "" : s.KhoNhap.Ten,
                    NgayYeuCau = s.NgayYeuCau,
                    TinhTrang = "Đang chờ duyệt",
                    Nguoiduyet = s.NhanVienDuyet.User.HoTen,
                    NgayDuyet = s.NgayDuyet != null ? s.NgayDuyet : (DateTime?)null,
                    NgayDuyetHienThi = s.NgayDuyet != null ? Convert.ToDateTime(s.NgayDuyet).ApplyFormatDateTimeSACH() : "",
                    DuocDuyet = s.DuocDuyet
                });
                var queryTuChoiDuyet = BaseRepository.TableNoTracking.Where(x => x.DuocDuyet == false).Select(s => new DSLinhKSNKGridVo()
                {
                    Id = s.Id,
                    MaPL = s.SoPhieu,
                    Loai = s.LoaiPhieuLinh.GetDescription(),
                    LoaiPhieuLinh = s.LoaiPhieuLinh,
                    NguoiYeuCau = s.NhanVienYeuCau.User.HoTen,
                    LinhTuKho = s.KhoXuat.Ten,
                    LinhVeKhoId = s.KhoXuatId,
                    LinhVeKho = s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan ? "" : s.KhoNhap.Ten,
                    NgayYeuCau = s.NgayYeuCau,
                    TinhTrang = "Từ Chối duyệt",
                    Nguoiduyet = s.NhanVienDuyet.User.HoTen,
                    NgayDuyet = s.NgayDuyet != null ? s.NgayDuyet : (DateTime?)null,
                    NgayDuyetHienThi = s.NgayDuyet != null ? Convert.ToDateTime(s.NgayDuyet).ApplyFormatDateTimeSACH() : "",
                    DuocDuyet = s.DuocDuyet
                });
                var queryDaDuyet = BaseRepository.TableNoTracking.Where(x => x.DuocDuyet == true).Select(s => new DSLinhKSNKGridVo()
                {
                    Id = s.Id,
                    MaPL = s.SoPhieu,
                    Loai = s.LoaiPhieuLinh.GetDescription(),
                    LoaiPhieuLinh = s.LoaiPhieuLinh,
                    NguoiYeuCau = s.NhanVienYeuCau.User.HoTen,
                    LinhTuKho = s.KhoXuat.Ten,
                    LinhVeKhoId = s.KhoXuatId,
                    LinhVeKho = s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan ? "" : s.KhoNhap.Ten,
                    NgayYeuCau = s.NgayYeuCau,
                    TinhTrang = "Đã duyệt",
                    Nguoiduyet = s.NhanVienDuyet.User.HoTen,
                    NgayDuyet = s.NgayDuyet != null ? s.NgayDuyet : (DateTime?)null,
                    NgayDuyetHienThi = s.NgayDuyet != null ? Convert.ToDateTime(s.NgayDuyet).ApplyFormatDateTimeSACH() : "",
                    DuocDuyet = s.DuocDuyet
                });
                var query = queryDangChoGui.Union(queryDangChoDuyet).Union(queryTuChoiDuyet).Union(queryDaDuyet).Where(d => d.LoaiPhieuLinh != EnumLoaiPhieuLinh.LinhChoBenhNhan);
                var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
                var queryTask = query.Skip(queryInfo.Skip)
                    .Take(queryInfo.Take).ToArrayAsync();

                await Task.WhenAll(countTask, queryTask);
                return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
            }

            return null;
        }
        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<SeachNgay>(queryInfo.AdditionalSearchString);

                //var queryDangChoGoi = DangChoGoi(queryInfo);
                //var queryDangChoDuyet = DangChoDuyet(queryInfo);
                //var queryTuChoiDuyet = TuChoiDuyet(queryInfo);
                //var queryDaDuyet = DaDuyet(queryInfo);

                var queryDangChoGoi = DangChoGoi(queryInfo).Union(DangChoGoiDP(queryInfo));
                var queryDangChoDuyet = DangChoDuyet(queryInfo).Union(DangChoDuyetDP(queryInfo));
                var queryTuChoiDuyet = TuChoiDuyet(queryInfo).Union(TuChoiDuyetDP(queryInfo));
                var queryDaDuyet = DaDuyet(queryInfo).Union(DaDuyetDP(queryInfo));

                var query = new List<DSLinhKSNKGridVo>();

                if (queryString.DangChoGoi == true)
                {
                    query = queryDangChoGoi.ToList();
                }
                if (queryString.DangChoDuyet == true)
                {
                    query = query.Union(queryDangChoDuyet).ToList();
                }
                if (queryString.TuChoiDuyet == true)
                {
                    query = query.Union(queryTuChoiDuyet).ToList();
                }
                if (queryString.DaDuyet == true)
                {
                    query = query.Union(queryDaDuyet).ToList();
                }
                if (queryString.DangChoGoi == false && queryString.DangChoDuyet == false && queryString.TuChoiDuyet == true && queryString.DaDuyet == false)
                {
                    query = new List<DSLinhKSNKGridVo>();
                    query = queryDangChoGoi.Union(queryDangChoDuyet).Union(queryTuChoiDuyet).Union(queryDaDuyet).ToList();
                }
                var dataOrderBy = query.Where(d => d.LoaiPhieuLinh != EnumLoaiPhieuLinh.LinhChoBenhNhan).AsQueryable();
                if (queryInfo.Sort.Count == 1 && string.IsNullOrEmpty(queryInfo.Sort[0].Dir))
                {
                    queryInfo.Sort[0].Dir = "asc";
                    queryInfo.Sort[0].Field = "DuocDuyet";
                }
                var countTask = dataOrderBy.OrderBy(queryInfo.SortString).Count();

                return new GridDataSource { TotalRowCount = countTask };
            }
            return null;
        }
        #endregion
        #region In lĩnh dược phẩm
        public async Task<string> InLinhKSNK(XacNhanInLinhKSNK xacNhanInLinhDuocPham)
        {
            var content = "";
            var ThuocHoacKSNK = " ";
            var index = 1;
            var thuocBHYT = "";
            var thuocKBHYT = "";
            var groupThuocBHYT = "Vật Tư BHYT";
            var groupThuocKhongBHYT = "Vật Tư Không BHYT";

            var headerBHYT = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                        + "<td style='border: 1px solid #020000;text-align: left;' colspan='7'><b>" + groupThuocBHYT.ToUpper()
                                        + "</b></tr>";
            var headerKhongBHYT = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                        + "<td style='border: 1px solid #020000;text-align: left;' colspan='7'><b>" + groupThuocKhongBHYT.ToUpper()
                                        + "</b></tr>";
            var result = _templateRepository.TableNoTracking
               .FirstOrDefault(x => x.Name.Equals("PhieuLinhKSNKTrucTiep"));

            var kiemTraPhieuLinhTinhTrangDuyet = _yeuCauLinhVatTuRepository.TableNoTracking.Where(d => d.Id == xacNhanInLinhDuocPham.YeuCauLinhKSNKId).Select(d => d.DuocDuyet).First();

            var yeuCauLinhKSNK = BaseRepository.GetByIdAsync(xacNhanInLinhDuocPham.YeuCauLinhKSNKId,
                                                                s => s.Include(z => z.KhoNhap)
                                                                     .Include(z => z.KhoXuat)
                                                                     .Include(z => z.NhanVienYeuCau)
                                                                     .Include(z => z.NhanVienDuyet)
                                                                     .Include(z => z.YeuCauLinhVatTuChiTiets).ThenInclude(k => k.VatTuBenhVien).ThenInclude(w => w.VatTus)
                                                                     .Include(z => z.YeuCauVatTuBenhViens).ThenInclude(k => k.VatTuBenhVien).ThenInclude(w => w.VatTus)
                                                                     .Include(z => z.YeuCauVatTuBenhViens).ThenInclude(k => k.YeuCauTiepNhan)
                                                                     .Include(z => z.YeuCauLinhVatTuChiTiets));

            if (xacNhanInLinhDuocPham.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu)  // lĩnh bù
            {

                return content;
            }
            else if (xacNhanInLinhDuocPham.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan)  // lĩnh tt
            {
                if (xacNhanInLinhDuocPham.TrangThaiIn == false) // k duyệt
                {
                    if (yeuCauLinhKSNK.Result != null)
                    {
                        var vt = yeuCauLinhKSNK.Result.YeuCauLinhVatTuChiTiets
                                                                .Select(o => new KSNKGridVo
                                                                {
                                                                    MaKSNK = o.VatTuBenhVien.Ma,
                                                                    TenKSNK = o.VatTuBenhVien.VatTus.Ten + (o.VatTuBenhVien.VatTus.NhaSanXuat != null && o.VatTuBenhVien.VatTus.NhaSanXuat != "" ? "; " + o.VatTuBenhVien.VatTus.NhaSanXuat : "") +
                                                                                                             (o.VatTuBenhVien.VatTus.NuocSanXuat != null && o.VatTuBenhVien.VatTus.NuocSanXuat != "" ? "; " + o.VatTuBenhVien.VatTus.NuocSanXuat : ""),
                                                                    SoLuong = o.SoLuong,
                                                                    DonViTinh = o.VatTuBenhVien.VatTus.DonViTinh,
                                                                    SoLuongCoTheXuat = o.SoLuongCoTheXuat,
                                                                    LaKSNKBHYT = o.LaVatTuBHYT
                                                                })
                                                               .GroupBy(xy => new { xy.TenKSNK, xy.MaKSNK, xy.DonViTinh })
                                                               .Select(o => new KSNKGridVo
                                                               {
                                                                   MaKSNK = o.First().MaKSNK,
                                                                   TenKSNK = o.First().TenKSNK,
                                                                   SoLuong = o.Sum(s => s.SoLuong),
                                                                   DonViTinh = o.First().DonViTinh,
                                                                   SoLuongCoTheXuat = o.Sum(s => s.SoLuongCoTheXuat)
                                                               }).OrderBy(d => d.TenKSNK);
                        var objData = GetHTMLLinhBenhNhanTuChoi(vt.OrderBy(d => d.LaKSNKBHYT).ThenBy(d => d.TenKSNK).ToList());
                        ThuocHoacKSNK = objData.html;
                        index = objData.Index;
                    }
                    else
                    {
                        var yeucau = 0; // to do
                        var thucChat = ""; // to do
                        var tenLoaiLinh = "";
                        var donViTinh = "";
                        var maHoatChat = "";
                        if (yeuCauLinhKSNK.Result.YeuCauVatTuBenhViens.Where(s => s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan).Any())
                        {
                            var objData = GetHTMLLinhBenhNhanDataoDaDuyet(yeuCauLinhKSNK.Result.YeuCauVatTuBenhViens.Where(s => s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan).ToList());
                            ThuocHoacKSNK = objData.html;
                            index = objData.Index;
                            ThuocHoacKSNK = objData.html;
                        }

                    }






                    var maVachPhieuLinh = yeuCauLinhKSNK.Result.SoPhieu.ToString();
                    var data = new
                    {
                        LogoUrl = xacNhanInLinhDuocPham.Hosting + "/assets/img/logo-bacha-full.png",
                        BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauLinhKSNK.Result.SoPhieu.ToString()) ? BarcodeHelper.GenerateBarCode(maVachPhieuLinh) : "",
                        MaVachPhieuLinh = maVachPhieuLinh,
                        NoiGiao = yeuCauLinhKSNK?.Result?.KhoXuat?.Ten,
                        DienGiai = yeuCauLinhKSNK?.Result?.GhiChu,
                        TruongKhoaDuocVTYT = "",
                        NguoiGiao = yeuCauLinhKSNK?.Result?.NhanVienDuyet?.User?.HoTen,
                        NguoiNhan = yeuCauLinhKSNK.Result?.NhanVienYeuCau?.User?.HoTen,
                        TuNgay = yeuCauLinhKSNK?.Result?.ThoiDiemLinhTongHopTuNgay?.ApplyFormatDateTimeSACH(),
                        DenNgay = yeuCauLinhKSNK?.Result?.ThoiDiemLinhTongHopDenNgay?.ApplyFormatDateTimeSACH(),
                        TruongPhongKhoaPhong = "",
                        CongKhoan = index - 1,
                        NgayThangNam = DateTime.Now.ApplyFormatDateTimeSACH(),
                        //HeaderPhieuLinhThuoc = tenLoaiLinh,
                        ThuocHoacKSNK = ThuocHoacKSNK,
                        KhoaPhong = yeuCauLinhKSNK?.Result?.KhoNhap?.Ten,
                        Ngay = DateTime.Now.Day,
                        Thang = DateTime.Now.Month,
                        Nam = DateTime.Now.Year,
                        Gio = DateTime.Now.ApplyFormatTime(),
                        NoiNhan = TenNoiNhanPhieuLinhTrucTiep((long)yeuCauLinhKSNK?.Result?.NoiYeuCauId)
                    };
                    content = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data);
                    return content;
                }
                else
                {
                    string yeuCau = "";
                    if (kiemTraPhieuLinhTinhTrangDuyet == true)
                    {
                        if (yeuCauLinhKSNK.Result != null)
                        {
                            if (yeuCauLinhKSNK.Result.YeuCauLinhVatTuChiTiets.Any())
                            {

                             
                                var objData = GetHTMLLinhBenhNhanDataoDaDuyet(yeuCauLinhKSNK.Result.YeuCauVatTuBenhViens.Where(s => s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan).ToList());
                                ThuocHoacKSNK = objData.html;
                                index = objData.Index;
                                ThuocHoacKSNK = objData.html;
                            }
                        }
                    }
                    else
                    {
                        if (yeuCauLinhKSNK.Result != null)
                        {
                            if (yeuCauLinhKSNK.Result.YeuCauVatTuBenhViens.Any())
                            {

                                var objData = GetHTMLLinhBenhNhanDataoDaDuyet(yeuCauLinhKSNK.Result.YeuCauVatTuBenhViens.Where(s => s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan).ToList());
                                ThuocHoacKSNK = objData.html;
                                index = objData.Index;
                                ThuocHoacKSNK = objData.html;
                            }
                        }
                    }


                    var maVachPhieuLinh = yeuCauLinhKSNK.Result.SoPhieu.ToString();
                    var data = new
                    {
                        LogoUrl = xacNhanInLinhDuocPham.Hosting + "/assets/img/logo-bacha-full.png",
                        BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauLinhKSNK.Result.SoPhieu.ToString()) ? BarcodeHelper.GenerateBarCode(maVachPhieuLinh) : "",
                        MaVachPhieuLinh = maVachPhieuLinh,
                        NoiGiao = yeuCauLinhKSNK?.Result?.KhoXuat?.Ten,
                        DienGiai = yeuCauLinhKSNK?.Result?.GhiChu,
                        TruongKhoaDuocVTYT = "",
                        NguoiGiao = yeuCauLinhKSNK?.Result?.NhanVienDuyet?.User?.HoTen,
                        NguoiNhan = yeuCauLinhKSNK.Result?.NhanVienYeuCau?.User?.HoTen,
                        TuNgay = yeuCauLinhKSNK?.Result?.ThoiDiemLinhTongHopTuNgay?.ApplyFormatDateTimeSACH(),
                        DenNgay = yeuCauLinhKSNK?.Result?.ThoiDiemLinhTongHopDenNgay?.ApplyFormatDateTimeSACH(),
                        TruongPhongKhoaPhong = "",
                        CongKhoan = index - 1,
                        NgayThangNam = DateTime.Now.ApplyFormatDateTimeSACH(),
                        //HeaderPhieuLinhThuoc = tenLoaiLinh,
                        ThuocHoacKSNK = ThuocHoacKSNK,
                        KhoaPhong = yeuCauLinhKSNK?.Result?.KhoNhap?.Ten,
                        Ngay = DateTime.Now.Day,
                        Thang = DateTime.Now.Month,
                        Nam = DateTime.Now.Year,
                        //Gio = DateTime.Now.ApplyFormatTime()
                        NoiNhan = TenNoiNhanPhieuLinhTrucTiep((long)yeuCauLinhKSNK?.Result?.NoiYeuCauId)
                    };
                    content = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data);
                    return content;
                }

            }
            else // lĩnh thường
            {
                if (yeuCauLinhKSNK.Result != null)
                {
                    var yeucau = 0; // to do
                    var thucChat = 0; // to do
                    var tenLoaiLinh = "";
                    var donViTinh = "";
                    var maHoatChat = "";
                    var ten = "";
                    var ghichu = "";
                    if (yeuCauLinhKSNK.Result.YeuCauLinhVatTuChiTiets.Where(x => x.LaVatTuBHYT == true).Any())
                    {
                        var inFoDuocPhams = _vatTuBenhVienRepository.TableNoTracking.
                                   Select(s => new {
                                       Id = s.Id,
                                       DonViTinh = s.VatTus.DonViTinh ,
                                       Ten = (s.VatTus.Ten + (s.VatTus.NhaSanXuat != null && s.VatTus.NhaSanXuat != "" ? "; " + s.VatTus.NhaSanXuat : "") +
                                          (s.VatTus.NuocSanXuat != null && s.VatTus.NuocSanXuat != "" ? "; " + s.VatTus.NuocSanXuat : ""))
                                   }).ToList();

                       
                        foreach (var itemx in yeuCauLinhKSNK.Result.YeuCauLinhVatTuChiTiets.Where(x => x.LaVatTuBHYT == true))
                        {
                            donViTinh = inFoDuocPhams.Where(d => d.Id == itemx.VatTuBenhVienId).Select(d => d.DonViTinh).FirstOrDefault();

                            ten = inFoDuocPhams.Where(z => z.Id == itemx.VatTuBenhVienId)
                                    .Select(s => s.Ten).FirstOrDefault();

                            ThuocHoacKSNK = ThuocHoacKSNK + headerBHYT + "<tr style='border: 1px solid #020000;'>"
                                                    + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                                    index++
                                                    + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                                   maHoatChat
                                                    + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                                    ten
                                                    + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                                   donViTinh
                                                    + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                                    yeucau
                                                     + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                                    thucChat
                                                    + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                                    ghichu;
                            tenLoaiLinh = EnumLoaiPhieuLinh.LinhDuTru.GetDescription();
                            donViTinh = "";
                            ten = "";
                        }
                    }
                    if (yeuCauLinhKSNK.Result.YeuCauLinhVatTuChiTiets.Where(x => x.LaVatTuBHYT == false).Any())
                    {
                        var inFoDuocPhams = _vatTuBenhVienRepository.TableNoTracking.
                                  Select(s => new {
                                      Id = s.Id,
                                      DonViTinh = s.VatTus.DonViTinh,
                                      Ten = (s.VatTus.Ten + (s.VatTus.NhaSanXuat != null && s.VatTus.NhaSanXuat != "" ? "; " + s.VatTus.NhaSanXuat : "") +
                                         (s.VatTus.NuocSanXuat != null && s.VatTus.NuocSanXuat != "" ? "; " + s.VatTus.NuocSanXuat : ""))
                                  }).ToList();
                        foreach (var itemx in yeuCauLinhKSNK.Result.YeuCauLinhVatTuChiTiets.Where(x => x.LaVatTuBHYT == false))
                        {
                            donViTinh = inFoDuocPhams.Where(d => d.Id == itemx.VatTuBenhVienId).Select(d => d.DonViTinh).FirstOrDefault();

                            ten = inFoDuocPhams.Where(z => z.Id == itemx.VatTuBenhVienId)
                                    .Select(s => s.Ten).FirstOrDefault();

                            ThuocHoacKSNK = ThuocHoacKSNK + headerKhongBHYT + "<tr style='border: 1px solid #020000;'>"
                                                  + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                                    index++
                                                    + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                                   maHoatChat
                                                    + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                                    ten
                                                    + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                                   donViTinh
                                                    + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                                    yeucau
                                                     + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                                    thucChat
                                                    + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                                    ghichu;
                            tenLoaiLinh = EnumLoaiPhieuLinh.LinhDuTru.GetDescription();
                            donViTinh = "";
                            ten = "";
                        }
                    }
                    var maVachPhieuLinh = yeuCauLinhKSNK.Result.SoPhieu.ToString();
                    var data = new
                    {
                        LogoUrl = xacNhanInLinhDuocPham.Hosting + "/assets/img/logo-bacha-full.png",
                        BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauLinhKSNK.Result.SoPhieu.ToString()) ? BarcodeHelper.GenerateBarCode(maVachPhieuLinh) : "",
                        MaVachPhieuLinh = maVachPhieuLinh,
                        NoiGiao = yeuCauLinhKSNK?.Result?.KhoXuat?.Ten,
                        DienGiai = yeuCauLinhKSNK?.Result?.GhiChu,
                        TruongKhoaDuocVTYT = "",
                        NguoiGiao = yeuCauLinhKSNK?.Result?.NhanVienDuyet?.User?.HoTen,
                        NguoiNhan = yeuCauLinhKSNK.Result?.NhanVienYeuCau?.User?.HoTen,
                        TuNgay = yeuCauLinhKSNK?.Result?.NgayYeuCau.ApplyFormatDateTimeSACH(),
                        DenNgay = yeuCauLinhKSNK?.Result?.NgayDuyet == null ? "" : yeuCauLinhKSNK?.Result?.NgayDuyet.Value.ApplyFormatDateTimeSACH(),
                        TruongPhongKhoaPhong = "",
                        CongKhoan = index - 1,
                        NgayThangNam = DateTime.Now.ApplyFormatDateTimeSACH(),
                        HeaderPhieuLinhThuoc = tenLoaiLinh,
                        ThuocHoacKSNK = ThuocHoacKSNK,
                        KhoaPhong = yeuCauLinhKSNK?.Result?.KhoNhap?.Ten,
                        Ngay = DateTime.Now.Day,
                        Thang = DateTime.Now.Month,
                        Nam = DateTime.Now.Year,
                        Gio = DateTime.Now.ApplyFormatTime()
                    };
                    content = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data);
                    return content;
                }
                return content;
            }
        }
        #endregion
        #region Ds duyệt dược phẩm
        public async Task<GridDataSource> GetDataDSDuyetKSNKForGridAsync(QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                BuildDefaultSortExpression(queryInfo);
                var queryString = JsonConvert.DeserializeObject<SeachNgay>(queryInfo.AdditionalSearchString);

                var queryDangChoDuyet = DangChoDuyet(queryInfo, true).Union(DangChoDuyetDP(queryInfo,true));
                var queryTuChoiDuyet = TuChoiDuyet(queryInfo, true).Union(TuChoiDuyetDP(queryInfo, true));
                var queryDaDuyet = DaDuyet(queryInfo, true).Union(DaDuyetDP(queryInfo, true));

                var query = new List<DSLinhKSNKGridVo>();

                //if (queryString.DangChoGoi == true)
                //{
                //    query = queryDangChoGoi.ToList();
                //}
                if (queryString.DangChoDuyet == true)
                {
                    query = query.Union(queryDangChoDuyet).ToList();
                }
                if (queryString.TuChoiDuyet == true)
                {
                    query = query.Union(queryTuChoiDuyet).ToList();
                }
                if (queryString.DaDuyet == true)
                {
                    query = query.Union(queryDaDuyet).ToList();
                }
                if (queryString.DangChoGoi == false && queryString.DangChoDuyet == false && queryString.TuChoiDuyet == true && queryString.DaDuyet == false)
                {
                    query = new List<DSLinhKSNKGridVo>();
                    query = queryDangChoDuyet.Union(queryTuChoiDuyet).Union(queryDaDuyet).ToList();
                }

                var dataOrderBy = query.Where(d=>d.LoaiPhieuLinh != EnumLoaiPhieuLinh.LinhChoBenhNhan).AsQueryable();
                if (queryInfo.Sort.Count == 1 && string.IsNullOrEmpty(queryInfo.Sort[0].Dir))
                {
                    queryInfo.Sort[0].Dir = "asc";
                    queryInfo.Sort[0].Field = "DuocDuyet";
                }
                var quaythuoc = dataOrderBy.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
                var countTask = dataOrderBy.Count();

                return new GridDataSource { Data = quaythuoc, TotalRowCount = countTask };

            }
            else
            {
                BuildDefaultSortExpression(queryInfo);
                var queryDangChoDuyet = BaseRepository.TableNoTracking.Where(x => x.DuocDuyet == null).Select(s => new DSLinhKSNKGridVo()
                {
                    Id = s.Id,
                    MaPL = s.SoPhieu,
                    Loai = s.LoaiPhieuLinh.GetDescription(),
                    LoaiPhieuLinh = s.LoaiPhieuLinh,
                    NguoiYeuCau = s.NhanVienYeuCau.User.HoTen,
                    LinhTuKho = s.KhoXuat.Ten,
                    LinhVeKhoId = s.KhoXuatId,
                    LinhVeKho = s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan ? "" : s.KhoNhap.Ten,
                    NgayYeuCau = s.NgayYeuCau,
                    TinhTrang = "Đang chờ duyệt",
                    Nguoiduyet = s.NhanVienDuyet.User.HoTen,
                    NgayDuyet = s.NgayDuyet != null ? s.NgayDuyet : (DateTime?)null,
                    NgayDuyetHienThi = s.NgayDuyet != null ? Convert.ToDateTime(s.NgayDuyet).ApplyFormatDateTimeSACH() : "",
                    DuocDuyet = s.DuocDuyet
                });
                var queryTuChoiDuyet = BaseRepository.TableNoTracking.Where(x => x.DuocDuyet == false).Select(s => new DSLinhKSNKGridVo()
                {
                    Id = s.Id,
                    MaPL = s.SoPhieu,
                    Loai = s.LoaiPhieuLinh.GetDescription(),
                    LoaiPhieuLinh = s.LoaiPhieuLinh,
                    NguoiYeuCau = s.NhanVienYeuCau.User.HoTen,
                    LinhTuKho = s.KhoXuat.Ten,
                    LinhVeKhoId = s.KhoXuatId,
                    LinhVeKho = s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan ? "" : s.KhoNhap.Ten,
                    NgayYeuCau = s.NgayYeuCau,
                    TinhTrang = "Từ Chối duyệt",
                    Nguoiduyet = s.NhanVienDuyet.User.HoTen,
                    NgayDuyet = s.NgayDuyet != null ? s.NgayDuyet : (DateTime?)null,
                    NgayDuyetHienThi = s.NgayDuyet != null ? Convert.ToDateTime(s.NgayDuyet).ApplyFormatDateTimeSACH() : "",
                    DuocDuyet = s.DuocDuyet
                });
                var queryDaDuyet = BaseRepository.TableNoTracking.Where(x => x.DuocDuyet == true).Select(s => new DSLinhKSNKGridVo()
                {
                    Id = s.Id,
                    MaPL = s.SoPhieu,
                    Loai = s.LoaiPhieuLinh.GetDescription(),
                    LoaiPhieuLinh = s.LoaiPhieuLinh,
                    NguoiYeuCau = s.NhanVienYeuCau.User.HoTen,
                    LinhTuKho = s.KhoXuat.Ten,
                    LinhVeKhoId = s.KhoXuatId,
                    LinhVeKho = s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan ? "" : s.KhoNhap.Ten,
                    NgayYeuCau = s.NgayYeuCau,
                    TinhTrang = "Đã duyệt",
                    Nguoiduyet = s.NhanVienDuyet.User.HoTen,
                    NgayDuyet = s.NgayDuyet != null ? s.NgayDuyet : (DateTime?)null,
                    NgayDuyetHienThi = s.NgayDuyet != null ? Convert.ToDateTime(s.NgayDuyet).ApplyFormatDateTimeSACH() : "",
                    DuocDuyet = s.DuocDuyet
                });
                var query = queryDangChoDuyet.Union(queryTuChoiDuyet).Union(queryDaDuyet).Where(d => d.LoaiPhieuLinh != EnumLoaiPhieuLinh.LinhChoBenhNhan);
                var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
                var queryTask = query.Skip(queryInfo.Skip)
                    .Take(queryInfo.Take).ToArrayAsync();

                await Task.WhenAll(countTask, queryTask);
                return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
            }

            return null;
        }

        public async Task<GridDataSource> GetDSDuyetKSNKTotalPageForGridAsync(QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<SeachNgay>(queryInfo.AdditionalSearchString);

                var queryDangChoDuyet = DangChoDuyet(queryInfo, true).Union(DangChoDuyetDP(queryInfo, true));
                var queryTuChoiDuyet = TuChoiDuyet(queryInfo, true).Union(TuChoiDuyetDP(queryInfo, true));
                var queryDaDuyet = DaDuyet(queryInfo, true).Union(DaDuyetDP(queryInfo, true));

                var query = new List<DSLinhKSNKGridVo>();
               
                if (queryString.DangChoDuyet == true)
                {
                    query = query.Union(queryDangChoDuyet).ToList();
                }
                if (queryString.TuChoiDuyet == true)
                {
                    query = query.Union(queryTuChoiDuyet).ToList();
                }
                if (queryString.DaDuyet == true)
                {
                    query = query.Union(queryDaDuyet).ToList();
                }
                if (queryString.DangChoGoi == false && queryString.DangChoDuyet == false && queryString.TuChoiDuyet == true && queryString.DaDuyet == false)
                {
                    query = new List<DSLinhKSNKGridVo>();
                    query = queryDangChoDuyet.Union(queryTuChoiDuyet).Union(queryDaDuyet).ToList();
                }
                var dataOrderBy = query.Where(d => d.LoaiPhieuLinh != EnumLoaiPhieuLinh.LinhChoBenhNhan).AsQueryable();
                if (queryInfo.Sort.Count == 1 && string.IsNullOrEmpty(queryInfo.Sort[0].Dir))
                {
                    queryInfo.Sort[0].Dir = "asc";
                    queryInfo.Sort[0].Field = "DuocDuyet";
                }
                var countTask = dataOrderBy.OrderBy(queryInfo.SortString).Count();

                return new GridDataSource { TotalRowCount = countTask };
            }
            return null;
        }
        #endregion
        #region Ds duyệt KSNK child
        public async Task<GridDataSource> GetDataDSDuyetLinhKSNKChildForGridAsync(QueryInfo queryInfo)
        {
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            int loaiPhieulinh = int.Parse(queryString[1]);

           

            EnumLoaiPhieuLinh loaiPhieu;
            if (loaiPhieulinh == 1)
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhDuTru;
            }
            else 
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhBu;
            }
           
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhBu)
            {
                bool loaiDuocPhamHayVatTu = bool.Parse(queryString[3]);
                if (loaiDuocPhamHayVatTu == true)
                {
                    BuildDefaultSortExpression(queryInfo);
                    var trangThaiLinhBu = _yeuCauLinhDuocPhamRepository.TableNoTracking.Where(d => d.Id == long.Parse(queryString[0])).Select(d => d.DuocDuyet).FirstOrDefault();

                    var query = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                                .Where(p => p.YeuCauLinhDuocPhamId == long.Parse(queryString[0])
                                    //&& p.YeuCauDuocPhamBenhVien.TrangThai == EnumYeuCauDuocPhamBenhVien.DaThucHien
                                    && (p.YeuCauLinhDuocPham.DuocDuyet == false ? p.YeuCauDuocPhamBenhVien.KhongLinhBu != false : p.YeuCauDuocPhamBenhVien.KhongLinhBu != true)
                                )
                                .Select(s => new YeuCauLinhKSNKBuGridVo
                                {
                                    Id = s.Id,
                                    YeuCauLinhKSNKId = s.YeuCauLinhDuocPhamId,
                                    KSNKBenhVienId = s.DuocPhamBenhVienId,
                                    TenKSNK = s.DuocPhamBenhVien.DuocPham.Ten,
                                    DonViTinh = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                                    HangSanXuat = s.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                                    NuocSanXuat = s.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                                    LaBHYT = s.LaDuocPhamBHYT,
                                    Nhom = s.LaDuocPhamBHYT == true ? "Dược Phẩm BHYT" : "Dược Phẩm Không BHYT",
                                    SoLuongCanBu = s.SoLuongCanBu != null ? (double)s.SoLuongCanBu.MathRoundNumber(2) : 0,
                                    SLDaLinh = s.YeuCauLinhDuocPhamId != null ? (double)s.YeuCauDuocPhamBenhVien.SoLuongDaLinhBu != null ? (double)s.YeuCauDuocPhamBenhVien.SoLuongDaLinhBu : 0 : 0,
                                    LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhBu,
                                    LinhVeKhoId = long.Parse(queryString[2]),
                                    NongDoHamLuong = s.DuocPhamBenhVien.DuocPham.HamLuong,
                                    HoatChat = s.DuocPhamBenhVien.DuocPham.HoatChat,
                                    DuongDung = s.DuocPhamBenhVien.DuocPham.DuongDung.Ten,
                                    DaDuyet = s.YeuCauLinhDuocPhamId != null ? s.YeuCauLinhDuocPham.DuocDuyet : null,
                                    SoLuongYeuCauDaDuyet = s.SoLuong // trường hợp cho đã duyệt

                            })
                                .GroupBy(x => new { x.YeuCauLinhKSNKId, x.KSNKBenhVienId, x.LaBHYT, x.Nhom, x.DonViTinh, x.HangSanXuat, x.NuocSanXuat, x.SoLuongTon })
                                  .Select(item => new YeuCauLinhKSNKBuGridVo()
                                  {
                                      Id = item.First().Id,
                                      KSNKBenhVienId = item.First().KSNKBenhVienId,
                                      LaBHYT = item.First().LaBHYT,
                                      TenKSNK = item.First().TenKSNK,
                                      Nhom = item.First().Nhom,
                                      DonViTinh = item.First().DonViTinh,
                                      HangSanXuat = item.First().HangSanXuat,
                                      NuocSanXuat = item.First().NuocSanXuat,
                                      SoLuongCanBu = item.Sum(x => x.SoLuongCanBu.MathRoundNumber(2)),
                                      LinhVeKhoId = long.Parse(queryString[2]),
                                      LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhBu,
                                      NongDoHamLuong = item.FirstOrDefault().NongDoHamLuong,
                                      SLDaLinh = item.Sum(x => x.SLDaLinh),
                                      HoatChat = item.First().HoatChat,
                                      DuongDung = item.First().DuongDung,
                                      DaDuyet = item.First().DaDuyet,
                                      SoLuongYeuCauDaDuyet = item.Sum(s => s.SoLuongYeuCauDaDuyet),
                                      LoaiDuocPhamHayVatTu = true
                                  })
                                  ;
                    var DuocPhamLinhBuGridVos = query.ToList();
                    if (trangThaiLinhBu == null)
                    {
                        var lstDuocPhamBenhVien = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(x => x.NhapKhoDuocPhams.KhoId == long.Parse(queryString[2])
                          ).ToList();

                        var result = DuocPhamLinhBuGridVos.Where(p => lstDuocPhamBenhVien.Any(o => o.DuocPhamBenhVienId == p.KSNKBenhVienId && o.LaDuocPhamBHYT == p.LaBHYT));

                        result = result.Select(o =>
                        {
                            o.SoLuongTon = lstDuocPhamBenhVien.Where(t => t.DuocPhamBenhVienId == o.KSNKBenhVienId && t.LaDuocPhamBHYT == o.LaBHYT).Sum(t => t.SoLuongNhap - t.SoLuongDaXuat);
                            o.SoLuongYeuCau = (o.SLDaLinh == 0 || o.SLDaLinh == null) ? (o.SoLuongTon < o.SoLuongCanBu ? o.SoLuongTon.MathRoundNumber(2) : o.SoLuongCanBu.MathRoundNumber(2))
                                                                 : (o.SoLuongTon < (o.SoLuongCanBu - o.SLDaLinh) ? o.SoLuongTon.MathRoundNumber(2) : (o.SoLuongCanBu - o.SLDaLinh).MathRoundNumber(2));
                            return o;
                        });
                        result = result.OrderBy(x => x.LaBHYT).ThenBy(x => x.TenKSNK).Distinct();
                        var countTask = queryInfo.LazyLoadPage == true ? 0 : result.Count();
                        var queryTask = result.ToArray();
                        return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
                    }
                    else
                    {
                        query = query.OrderBy(x => x.LaBHYT).ThenBy(x => x.TenKSNK).Distinct();
                        var countTask = queryInfo.LazyLoadPage == true ? 0 : query.Count();
                        var queryTask = query.ToArray();
                        return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
                    }
                }
                else
                {
                    BuildDefaultSortExpression(queryInfo);
                    var trangThaiBu = _yeuCauLinhVatTuRepository.TableNoTracking.Where(d => d.Id == long.Parse(queryString[0])).Select(d => d.DuocDuyet).FirstOrDefault();
                    var query = _yeuCauLinhVatTuChiTietRepository.TableNoTracking
                                .Where(p => p.YeuCauLinhVatTuId == long.Parse(queryString[0])
                                    //&& p.YeuCauVatTuBenhVien.TrangThai == EnumYeuCauVatTuBenhVien.DaThucHien
                                    && (p.YeuCauLinhVatTu.DuocDuyet == false ? p.YeuCauVatTuBenhVien.KhongLinhBu != false : p.YeuCauVatTuBenhVien.KhongLinhBu != true)
                                )
                                .Select(s => new YeuCauLinhKSNKBuGridVo
                                {
                                    Id = s.Id,
                                    YeuCauLinhKSNKId = s.YeuCauLinhVatTuId,
                                    KSNKBenhVienId = s.VatTuBenhVienId,
                                    TenKSNK = s.VatTuBenhVien.VatTus.Ten,
                                    DonViTinh = s.VatTuBenhVien.VatTus.DonViTinh,
                                    HangSanXuat = s.VatTuBenhVien.VatTus.NhaSanXuat,
                                    NuocSanXuat = s.VatTuBenhVien.VatTus.NuocSanXuat,
                                    LaBHYT = s.LaVatTuBHYT,
                                    Nhom = s.LaVatTuBHYT == true ? "Vật Tư BHYT" : "Vật Tư Không BHYT",
                                    SoLuongCanBu = (double)s.SoLuongCanBu,
                                    SLDaLinh = s.YeuCauVatTuBenhVienId != null ? (double)s.YeuCauVatTuBenhVien.SoLuongDaLinhBu != null ? (double)s.YeuCauVatTuBenhVien.SoLuongDaLinhBu : 0 : 0,
                                    LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhBu,
                                    LinhveKhoId = long.Parse(queryString[2]),
                                    SoLuongYeuCauDaDuyet = s.SoLuong,
                                    DaDuyet = s.YeuCauLinhVatTuId != null ? s.YeuCauLinhVatTu.DuocDuyet : null

                                })
                                .GroupBy(x => new { x.YeuCauLinhKSNKId, x.KSNKBenhVienId, x.LaBHYT, x.Nhom, x.DonViTinh, x.HangSanXuat, x.NuocSanXuat, x.SoLuongTon })
                                  .Select(item => new YeuCauLinhKSNKBuGridVo()
                                  {
                                      Id = item.FirstOrDefault().Id,
                                      KSNKBenhVienId = item.First().KSNKBenhVienId,
                                      LaBHYT = item.First().LaBHYT,
                                      TenKSNK = item.First().TenKSNK,
                                      Nhom = item.First().Nhom,
                                      DonViTinh = item.First().DonViTinh,
                                      HangSanXuat = item.First().HangSanXuat,
                                      NuocSanXuat = item.First().NuocSanXuat,
                                      SoLuongCanBu = item.Sum(x => x.SoLuongCanBu).MathRoundNumber(2),
                                      LinhveKhoId = long.Parse(queryString[2]),
                                      LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhBu,
                                      SLDaLinh = item.Sum(x => x.SLDaLinh),
                                      SoLuongYeuCauDaDuyet = item.Sum(s => s.SoLuongYeuCauDaDuyet),
                                      DaDuyet = item.First().DaDuyet,
                                      LoaiDuocPhamHayVatTu = false
                                  })
                                  .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenKSNK).Distinct();
                    var KSNKLinhBuGridVos = query.ToList();

                    if (trangThaiBu == null)
                    {
                        var lstVatTuBenhVien = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(x => x.NhapKhoVatTu.KhoId == long.Parse(queryString[2]) &&
                         x.SoLuongDaXuat < x.SoLuongNhap).ToList();

                        var result = KSNKLinhBuGridVos.Where(p => lstVatTuBenhVien.Any(o => o.VatTuBenhVienId == p.KSNKBenhVienId && o.LaVatTuBHYT == p.LaBHYT));

                        result = result.Select(o =>
                        {
                            o.SoLuongTon = lstVatTuBenhVien.Where(t => t.VatTuBenhVienId == o.KSNKBenhVienId && t.LaVatTuBHYT == o.LaBHYT).Sum(t => t.SoLuongNhap - t.SoLuongDaXuat);
                            o.SoLuongYeuCau = (o.SLDaLinh == 0 || o.SLDaLinh == null) ? (o.SoLuongTon < o.SoLuongCanBu ? o.SoLuongTon.MathRoundNumber(2) : o.SoLuongCanBu.MathRoundNumber(2))
                                                                 : (o.SoLuongTon < (o.SoLuongCanBu - o.SLDaLinh) ? o.SoLuongTon.MathRoundNumber(2) : (o.SoLuongCanBu - o.SLDaLinh).MathRoundNumber(2));
                            return o;
                        });
                        var countTask = queryInfo.LazyLoadPage == true ? 0 : result.Count();
                        var queryTask = result.Skip(queryInfo.Skip)
                            .Take(queryInfo.Take).ToArray();
                        return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
                    }
                    else
                    {
                        var countTask = queryInfo.LazyLoadPage == true ? 0 : query.Count();
                        var queryTask = query.Skip(queryInfo.Skip)
                            .Take(queryInfo.Take).ToArray();
                        return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
                    }

                }

            }
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhDuTru)
            {
                BuildDefaultSortExpression(queryInfo);
                var query = _yeuCauLinhVatTuChiTietRepository.TableNoTracking
                     .Where(o =>
                             o.YeuCauLinhVatTu.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhDuTru &&
                             o.YeuCauLinhVatTuId == long.Parse(queryString[0])
                            )
                    .Select(s => new DSLinhVatChildTuGridVo
                    {
                        Id = long.Parse(queryString[0]),
                        LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhDuTru,
                        Ma = s.VatTuBenhVien.VatTus.Ma,
                        TenKSNK = s.VatTuBenhVien.VatTus.Ten,
                        DonViTinh = s.VatTuBenhVien.VatTus.DonViTinh,
                        SoLuongYc = s.SoLuong,
                        HangSanXuat = s.VatTuBenhVien.VatTus.NhaSanXuat,
                        NuocSanXuat = s.VatTuBenhVien.VatTus.NuocSanXuat,
                        BSKeToa = s.YeuCauVatTuBenhVien.NhanVienChiDinh.User.HoTen,
                        Nhom = s.LaVatTuBHYT == true ? "BHYT" : "Không BHYT",
                        SLTon = s.VatTuBenhVien.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == s.YeuCauLinhVatTu.KhoXuatId && nkct.LaVatTuBHYT == s.LaVatTuBHYT).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat).MathRoundNumber(1),
                        TenKhoLinh = s.YeuCauLinhVatTu.KhoXuat.Ten
                    });
                var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
                var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                    .Take(queryInfo.Take).ToArrayAsync();

                await Task.WhenAll(countTask, queryTask);

                return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
            }
            
            return null;
        }
        public async Task<GridDataSource> GetDataDSDuyetLinhKSNKChildChildForGridAsync(QueryInfo queryInfo)
        {
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            int loaiPhieulinh = int.Parse(queryString[1]);

           
            EnumLoaiPhieuLinh loaiPhieu;
            if (loaiPhieulinh == 1)
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhDuTru;
            }
            else 
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhBu;
            }
            
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhBu)
            {
                bool loaiDuocPhamHayVatTu = bool.Parse(queryString[5]);
                if (loaiDuocPhamHayVatTu == true)
                {
                    var trangThaiDuyet = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking.Where(x => x.Id == long.Parse(queryString[0])).Select(s => s.YeuCauLinhDuocPham.DuocDuyet).FirstOrDefault();
                    var yeuCauLinhDuocPhamId = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking.Where(x => x.Id == long.Parse(queryString[0])).Select(s => s.YeuCauLinhDuocPhamId).FirstOrDefault();
                    var query = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                                .Where(p => p.YeuCauLinhDuocPhamId == yeuCauLinhDuocPhamId
                                 && p.DuocPhamBenhVienId == long.Parse(queryString[2])
                                 && p.LaDuocPhamBHYT == bool.Parse(queryString[3])
                                 && p.YeuCauDuocPhamBenhVien.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhBu
                                 && p.YeuCauLinhDuocPham.DuocDuyet == trangThaiDuyet
                                //&& (p.YeuCauLinhDuocPham.DuocDuyet == false ? p.YeuCauDuocPhamBenhVien.KhongLinhBu != false : p.YeuCauDuocPhamBenhVien.KhongLinhBu != true)
                                )
                    .OrderBy(x => x.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh)
                    .Select(s => new KSNKLinhBuCuaBNGridVos
                    {
                        Id = s.Id,
                        MaTN = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.MaYeuCauTiepNhan,
                        MaBN = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.BenhNhan.MaBN,
                        HoTen = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.HoTen,
                        SL = trangThaiDuyet == true ? s.SoLuong : s.SoLuongCanBu,
                        DVKham = s.YeuCauDuocPhamBenhVien.YeuCauKhamBenh.TenDichVu ?? s.YeuCauDuocPhamBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu ?? null,
                        BSKeToa = s.YeuCauDuocPhamBenhVien.NhanVienChiDinh.User.HoTen,
                        NgayKe = s.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                        SLDaLinh = s.SoLuongDaLinhBu != null ? s.SoLuongDaLinhBu : 0,
                        SLDanhSachDuyet = s.SoLuong.MathRoundNumber(2),
                        SLCanBu = s.SoLuongCanBu,
                        NgayDieuTri = s.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri != null ? s.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri.NgayDieuTri : s.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh
                    });
                    var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
                    var queryTask = query.Skip(queryInfo.Skip)
                        .Take(queryInfo.Take).ToArrayAsync();
                    await Task.WhenAll(countTask, queryTask);
                    return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
                }
                else
                {
                    BuildDefaultSortExpression(queryInfo);

                    var trangThai = _yeuCauLinhVatTuChiTietRepository.TableNoTracking.Where(x => x.Id == long.Parse(queryString[0])).Select(s => s.YeuCauLinhVatTu.DuocDuyet).FirstOrDefault();
                    var yeuCauLinhKSNKId = _yeuCauLinhVatTuChiTietRepository.TableNoTracking.Where(x => x.Id == long.Parse(queryString[0])).Select(s => s.YeuCauLinhVatTuId).FirstOrDefault();
                    var query = _yeuCauLinhVatTuChiTietRepository.TableNoTracking
                             .Where(p => p.YeuCauLinhVatTuId == yeuCauLinhKSNKId
                                && p.YeuCauLinhVatTu.DuocDuyet == trangThai
                                 && p.VatTuBenhVienId == long.Parse(queryString[2])
                                 && p.LaVatTuBHYT == bool.Parse(queryString[3])
                                 && p.YeuCauVatTuBenhVien.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhBu
                             //&& (p.YeuCauLinhKSNK.DuocDuyet == false ? p.YeuCauVatTuBenhVien.KhongLinhBu != false : p.YeuCauVatTuBenhVien.KhongLinhBu != true)
                             )
                 .OrderBy(x => x.YeuCauVatTuBenhVien.ThoiDiemChiDinh)
                    .Select(s => new KSNKLinhBuCuaBNGridVos
                    {
                        Id = s.Id,
                        MaTN = s.YeuCauVatTuBenhVien.YeuCauTiepNhan.MaYeuCauTiepNhan,
                        MaBN = s.YeuCauVatTuBenhVien.YeuCauTiepNhan.BenhNhan.MaBN,
                        HoTen = s.YeuCauVatTuBenhVien.YeuCauTiepNhan.HoTen,
                        SL = trangThai == true ? s.SoLuong : s.SoLuongCanBu,
                        DVKham = s.YeuCauVatTuBenhVien.YeuCauKhamBenh.TenDichVu ?? s.YeuCauVatTuBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu ?? null,
                        BSKeToa = s.YeuCauVatTuBenhVien.NhanVienChiDinh.User.HoTen,
                        NgayKe = s.YeuCauVatTuBenhVien.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                        SLDaLinh = s.SoLuongDaLinhBu != null ? s.SoLuongDaLinhBu : 0,
                        SLDanhSachDuyet = s.SoLuong.MathRoundNumber(2),
                        SLCanBu = s.SoLuongCanBu,
                        NgayDieuTri = s.YeuCauVatTuBenhVien.NoiTruPhieuDieuTri != null ? s.YeuCauVatTuBenhVien.NoiTruPhieuDieuTri.NgayDieuTri : s.YeuCauVatTuBenhVien.ThoiDiemChiDinh
                    });
                    var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
                    var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                        .Take(queryInfo.Take).ToArrayAsync();
                    await Task.WhenAll(countTask, queryTask);
                    return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
                }
                
            }
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhDuTru)
            {
                BuildDefaultSortExpression(queryInfo);
                var query = _yeuCauLinhVatTuChiTietRepository.TableNoTracking
                     .Where(o =>
                             o.YeuCauLinhVatTu.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhDuTru &&
                             o.YeuCauLinhVatTuId == long.Parse(queryString[0])
                            )
                    .Select(s => new DSLinhVatChildTuGridVo
                    {
                        Id = s.Id,
                        Ma = s.VatTuBenhVien.VatTus.Ma,
                        TenKSNK = s.VatTuBenhVien.VatTus.Ten,
                        DonViTinh = s.VatTuBenhVien.VatTus.DonViTinh,
                        SoLuongYc = s.SoLuong,
                        HangSanXuat = s.VatTuBenhVien.VatTus.NhaSanXuat,
                        NuocSanXuat = s.VatTuBenhVien.VatTus.NuocSanXuat,
                        BSKeToa = s.YeuCauVatTuBenhVien.NhanVienChiDinh.User.HoTen,
                        Nhom = s.LaVatTuBHYT == true ? "BHYT" : "Không BHYT",
                        TenKhoLinh = s.YeuCauLinhVatTu.KhoXuat.Ten
                    });
                var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
                var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                    .Take(queryInfo.Take).ToArrayAsync();

                await Task.WhenAll(countTask, queryTask);
                return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
            }
            
            return null;
        }
        public async Task<GridDataSource> GetTotalPageFDSLinhKSNKChildForGridAsync(QueryInfo queryInfo)
        {
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            int loaiPhieulinh = int.Parse(queryString[1]);

            

            EnumLoaiPhieuLinh loaiPhieu;
            if (loaiPhieulinh == 1)
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhDuTru;
            }
            else 
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhBu;
            }
          
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhBu)
            {
                bool loaiDuocPhamHayVatTu = bool.Parse(queryString[3]);
                if (loaiDuocPhamHayVatTu == true) // DP
                {
                    var query = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                           .Where(p => p.YeuCauLinhDuocPhamId == long.Parse(queryString[0])
                             //&& p.YeuCauDuocPhamBenhVien.TrangThai == EnumYeuCauDuocPhamBenhVien.ChuaThucHien
                             && (p.YeuCauLinhDuocPham.DuocDuyet == false ? p.YeuCauDuocPhamBenhVien.KhongLinhBu != false : p.YeuCauDuocPhamBenhVien.KhongLinhBu != true)
                           //&& (p.YeuCauDuocPhamBenhVien.SoLuongDaLinhBu == null || p.YeuCauDuocPhamBenhVien.SoLuongDaLinhBu < p.YeuCauDuocPhamBenhVien.SoLuong)
                           )
                           .Select(s => new YeuCauLinhKSNKBuGridVo
                           {
                               Id = s.Id,
                               YeuCauLinhKSNKId = s.YeuCauLinhDuocPhamId,
                               KSNKBenhVienId = s.DuocPhamBenhVienId,
                               TenKSNK = s.DuocPhamBenhVien.DuocPham.Ten,
                               DonViTinh = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                               HangSanXuat = s.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                               NuocSanXuat = s.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                               LaBHYT = s.LaDuocPhamBHYT,
                               Nhom = s.LaDuocPhamBHYT == true ? "Dược Phẩm BHYT" : "Dược Phẩm Không BHYT",
                               SoLuongCanBu = s.SoLuongCanBu != null ? (double)s.SoLuongCanBu : 0,
                               SLDaLinh = s.YeuCauDuocPhamBenhVien != null ? (double)s.YeuCauDuocPhamBenhVien.SoLuongDaLinhBu != null ? (double)s.YeuCauDuocPhamBenhVien.SoLuongDaLinhBu : 0 : 0,
                               LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhBu,
                               LinhVeKhoId = long.Parse(queryString[2])
                           })
                           .GroupBy(x => new { x.YeuCauLinhKSNKId, x.KSNKBenhVienId, x.LaBHYT, x.Nhom, x.DonViTinh, x.HangSanXuat, x.NuocSanXuat, x.SoLuongTon })
                             .Select(item => new YeuCauLinhKSNKBuGridVo()
                             {
                                 KSNKBenhVienId = item.First().KSNKBenhVienId,
                                 LaBHYT = item.First().LaBHYT,
                                 TenKSNK = item.First().TenKSNK,
                                 Nhom = item.First().Nhom,
                                 DonViTinh = item.First().DonViTinh,
                                 HangSanXuat = item.First().HangSanXuat,
                                 NuocSanXuat = item.First().NuocSanXuat,
                                 SoLuongCanBu = item.Sum(x => x.SoLuongCanBu),
                                 LinhVeKhoId = long.Parse(queryString[2]),
                                 LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhBu,
                                 SLDaLinh = item.Sum(x => x.SLDaLinh),
                                 LoaiDuocPhamHayVatTu = true
                             })
                             .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenKSNK).Distinct();
                    var DuocPhamLinhBuGridVos = query.ToList();

                    var lstDuocPhamBenhVien = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(x => x.NhapKhoDuocPhams.KhoId == long.Parse(queryString[2])
                           ).ToList();

                    var result = DuocPhamLinhBuGridVos.Where(p => lstDuocPhamBenhVien.Any(o => o.DuocPhamBenhVienId == p.KSNKBenhVienId && o.LaDuocPhamBHYT == p.LaBHYT));

                    result = result.Select(o =>
                    {
                        //o.SoLuongTon = lstVatTuBenhVien.Where(t => t.DuocPhamBenhVienId == o.DuocPhamBenhVienId && t.LaDuocPhamBHYT == o.LaBHYT).Sum(t => t.SoLuongNhap - t.SoLuongDaXuat);
                        //o.SoLuongYeuCau = o.SoLuongTon < o.SoLuongCanBu ? o.SoLuongTon : o.SoLuongCanBu;

                        o.SoLuongTon = lstDuocPhamBenhVien.Where(t => t.DuocPhamBenhVienId == o.KSNKBenhVienId && t.LaDuocPhamBHYT == o.LaBHYT).Sum(t => t.SoLuongNhap - t.SoLuongDaXuat);
                        o.SoLuongYeuCau = (o.SLDaLinh == 0 || o.SLDaLinh == null) ? (o.SoLuongTon < o.SoLuongCanBu ? o.SoLuongTon.MathRoundNumber(2) : o.SoLuongCanBu.MathRoundNumber(2))
                                                             : (o.SoLuongTon < (o.SoLuongCanBu - o.SLDaLinh) ? o.SoLuongTon.MathRoundNumber(2) : (o.SoLuongCanBu - o.SLDaLinh).MathRoundNumber(2));
                        return o;
                    });
                    var dataOrderBy = result.AsQueryable().OrderBy(queryInfo.SortString);
                    var countTask = dataOrderBy.Count();

                    return new GridDataSource { TotalRowCount = countTask };
                }
                else
                { 
                    // VT
                    var trangThaiBu = _yeuCauLinhVatTuRepository.TableNoTracking.Where(d => d.Id == long.Parse(queryString[0])).Select(d => d.DuocDuyet).FirstOrDefault();
                    var query = _yeuCauLinhVatTuChiTietRepository.TableNoTracking
                              .Where(p => p.YeuCauLinhVatTuId == long.Parse(queryString[0])
                                  //&& p.YeuCauVatTuBenhVien.TrangThai == EnumYeuCauVatTuBenhVien.DaThucHien
                                  && (p.YeuCauLinhVatTu.DuocDuyet == false ? p.YeuCauVatTuBenhVien.KhongLinhBu != false : p.YeuCauVatTuBenhVien.KhongLinhBu != true)
                              )
                              .Select(s => new YeuCauLinhKSNKBuGridVo
                              {
                                  Id = s.Id,
                                  YeuCauLinhKSNKId = s.YeuCauLinhVatTuId,
                                  KSNKBenhVienId = s.VatTuBenhVienId,
                                  TenKSNK = s.VatTuBenhVien.VatTus.Ten,
                                  DonViTinh = s.VatTuBenhVien.VatTus.DonViTinh,
                                  HangSanXuat = s.VatTuBenhVien.VatTus.NhaSanXuat,
                                  NuocSanXuat = s.VatTuBenhVien.VatTus.NuocSanXuat,
                                  LaBHYT = s.LaVatTuBHYT,
                                  Nhom = s.LaVatTuBHYT == true ? "Vật Tư BHYT" : "Vật Tư Không BHYT",
                                  SoLuongCanBu = (double)s.SoLuongCanBu,
                                  SLDaLinh = s.YeuCauVatTuBenhVienId != null ? (double)s.YeuCauVatTuBenhVien.SoLuongDaLinhBu != null ? (double)s.YeuCauVatTuBenhVien.SoLuongDaLinhBu : 0 : 0,
                                  LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhBu,
                                  LinhveKhoId = long.Parse(queryString[2]),
                                  SoLuongYeuCauDaDuyet = s.SoLuong,
                                  DaDuyet = s.YeuCauLinhVatTuId != null ? s.YeuCauLinhVatTu.DuocDuyet : null

                              })
                              .GroupBy(x => new { x.YeuCauLinhKSNKId, x.KSNKBenhVienId, x.LaBHYT, x.Nhom, x.DonViTinh, x.HangSanXuat, x.NuocSanXuat, x.SoLuongTon })
                                .Select(item => new YeuCauLinhKSNKBuGridVo()
                                {
                                    Id = item.FirstOrDefault().Id,
                                    KSNKBenhVienId = item.First().KSNKBenhVienId,
                                    LaBHYT = item.First().LaBHYT,
                                    TenKSNK = item.First().TenKSNK,
                                    Nhom = item.First().Nhom,
                                    DonViTinh = item.First().DonViTinh,
                                    HangSanXuat = item.First().HangSanXuat,
                                    NuocSanXuat = item.First().NuocSanXuat,
                                    SoLuongCanBu = item.Sum(x => x.SoLuongCanBu).MathRoundNumber(2),
                                    LinhveKhoId = long.Parse(queryString[2]),
                                    LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhBu,
                                    SLDaLinh = item.Sum(x => x.SLDaLinh),
                                    SoLuongYeuCauDaDuyet = item.Sum(s => s.SoLuongYeuCauDaDuyet),
                                    DaDuyet = item.First().DaDuyet,
                                    LoaiDuocPhamHayVatTu = false
                                })
                                .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenKSNK).Distinct();
                    var KSNKLinhBuGridVos = query.ToList();
                    if (trangThaiBu == null)
                    {
                        var lstVatTuBenhVien = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(x => x.NhapKhoVatTu.KhoId == long.Parse(queryString[2])
                           && x.SoLuongDaXuat < x.SoLuongNhap).ToList();

                        var result = KSNKLinhBuGridVos.Where(p => lstVatTuBenhVien.Any(o => o.VatTuBenhVienId == p.KSNKBenhVienId && o.LaVatTuBHYT == p.LaBHYT));

                        result = result.Select(o =>
                        {
                            o.SoLuongTon = lstVatTuBenhVien.Where(t => t.VatTuBenhVienId == o.KSNKBenhVienId && t.LaVatTuBHYT == o.LaBHYT).Sum(t => t.SoLuongNhap - t.SoLuongDaXuat);
                            o.SoLuongYeuCau = (o.SLDaLinh == 0 || o.SLDaLinh == null) ? (o.SoLuongTon < o.SoLuongCanBu ? o.SoLuongTon.MathRoundNumber(2) : o.SoLuongCanBu.MathRoundNumber(2))
                                                                 : (o.SoLuongTon < (o.SoLuongCanBu - o.SLDaLinh) ? o.SoLuongTon.MathRoundNumber(2) : (o.SoLuongCanBu - o.SLDaLinh).MathRoundNumber(2));
                            return o;
                        });
                        var dataOrderBy = result.AsQueryable();
                        var countTask = dataOrderBy.Count();

                        return new GridDataSource { TotalRowCount = countTask };
                    }
                    else
                    {
                        var dataOrderBy = query.AsQueryable();
                        var countTask = dataOrderBy.Count();

                        return new GridDataSource { TotalRowCount = countTask };
                    }
                }
            }
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhDuTru)
            {
                var query = _yeuCauLinhVatTuChiTietRepository.TableNoTracking
                     .Where(o =>
                             o.YeuCauLinhVatTu.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhDuTru &&
                             o.YeuCauLinhVatTuId == long.Parse(queryString[0])
                            )
                    .Select(s => new DSLinhVatChildTuGridVo
                    {
                        Id = long.Parse(queryString[0]),
                        LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhDuTru,
                        Ma = s.VatTuBenhVien.VatTus.Ma,
                        TenKSNK = s.VatTuBenhVien.VatTus.Ten,
                        DonViTinh = s.VatTuBenhVien.VatTus.DonViTinh,
                        SoLuongYc = s.SoLuong,
                        HangSanXuat = s.VatTuBenhVien.VatTus.NhaSanXuat,
                        NuocSanXuat = s.VatTuBenhVien.VatTus.NuocSanXuat,
                        BSKeToa = s.YeuCauVatTuBenhVien.NhanVienChiDinh.User.HoTen,
                        Nhom = s.LaVatTuBHYT == true ? "BHYT" : "Không BHYT",
                        SLTon = s.VatTuBenhVien.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == s.YeuCauLinhVatTu.KhoXuatId && nkct.LaVatTuBHYT == s.LaVatTuBHYT).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat).MathRoundNumber(1),
                        TenKhoLinh = s.YeuCauLinhVatTu.KhoXuat.Ten
                    });
                var countTask = query.CountAsync();
                await Task.WhenAll(countTask);

                return new GridDataSource { TotalRowCount = countTask.Result };
            }
            return null;
        }
        public async Task<GridDataSource> GetTotalPageFDSLinhKSNKChildChildForGridAsync(QueryInfo queryInfo)
        {
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            int loaiPhieulinh = int.Parse(queryString[1]);

            

            EnumLoaiPhieuLinh loaiPhieu;
            if (loaiPhieulinh == 1)
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhDuTru;
            }
            else 
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhBu;
            }
           
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhBu)
            {
                bool loaiDuocPhamHayVatTu = bool.Parse(queryString[3]);
                if (loaiDuocPhamHayVatTu == true)
                {
                    var trangThaiDuyet = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking.Where(x => x.Id == long.Parse(queryString[0])).Select(s => s.YeuCauLinhDuocPham.DuocDuyet).FirstOrDefault();
                    var yeuCauLinhDuocPhamId = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking.Where(x => x.Id == long.Parse(queryString[0])).Select(s => s.YeuCauLinhDuocPhamId).FirstOrDefault();
                    var query = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                                .Where(p => p.YeuCauLinhDuocPhamId == yeuCauLinhDuocPhamId
                                 && p.DuocPhamBenhVienId == long.Parse(queryString[2])
                                 && p.LaDuocPhamBHYT == bool.Parse(queryString[3])
                                 && p.YeuCauDuocPhamBenhVien.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhBu
                                 && p.YeuCauLinhDuocPham.DuocDuyet == trangThaiDuyet
                                //&& (p.YeuCauLinhDuocPham.DuocDuyet == false ? p.YeuCauDuocPhamBenhVien.KhongLinhBu != false : p.YeuCauDuocPhamBenhVien.KhongLinhBu != true)
                                )
                    .OrderBy(x => x.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh)
                    .Select(s => new KSNKLinhBuCuaBNGridVos
                    {
                        Id = s.Id,
                        MaTN = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.MaYeuCauTiepNhan,
                        MaBN = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.BenhNhan.MaBN,
                        HoTen = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.HoTen,
                        SL = trangThaiDuyet == true ? s.SoLuong : s.SoLuongCanBu,
                        DVKham = s.YeuCauDuocPhamBenhVien.YeuCauKhamBenh.TenDichVu ?? s.YeuCauDuocPhamBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu ?? null,
                        BSKeToa = s.YeuCauDuocPhamBenhVien.NhanVienChiDinh.User.HoTen,
                        NgayKe = s.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                        SLDaLinh = s.SoLuongDaLinhBu != null ? s.SoLuongDaLinhBu : 0,
                        SLDanhSachDuyet = s.SoLuong.MathRoundNumber(2),
                        SLCanBu = s.SoLuongCanBu,
                        NgayDieuTri = s.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri != null ? s.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri.NgayDieuTri : s.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh
                    });
                    var countTask = query.Skip(queryInfo.Skip)
                       .Take(queryInfo.Take).CountAsync();
                    await Task.WhenAll(countTask);

                    return new GridDataSource { TotalRowCount = countTask.Result };
                }
                else
                {
                    var trangThai = _yeuCauLinhVatTuChiTietRepository.TableNoTracking.Where(x => x.Id == long.Parse(queryString[0])).Select(s => s.YeuCauLinhVatTu.DuocDuyet).FirstOrDefault();
                    var yeuCauLinhKSNKId = _yeuCauLinhVatTuChiTietRepository.TableNoTracking.Where(x => x.Id == long.Parse(queryString[0])).Select(s => s.YeuCauLinhVatTuId).FirstOrDefault();
                    var query = _yeuCauLinhVatTuChiTietRepository.TableNoTracking
                             .Where(p => p.YeuCauLinhVatTuId == yeuCauLinhKSNKId
                                && p.YeuCauLinhVatTu.DuocDuyet == trangThai
                                 && p.VatTuBenhVienId == long.Parse(queryString[2])
                                 && p.LaVatTuBHYT == bool.Parse(queryString[3])
                                 && p.YeuCauVatTuBenhVien.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhBu
                             //&& (p.YeuCauLinhKSNK.DuocDuyet == false ? p.YeuCauVatTuBenhVien.KhongLinhBu != false : p.YeuCauVatTuBenhVien.KhongLinhBu != true)
                             && p.YeuCauVatTuBenhVien.KhoLinh.LaKhoKSNK == true)
                 .OrderBy(x => x.YeuCauVatTuBenhVien.ThoiDiemChiDinh)
                  .Select(s => new KSNKLinhBuCuaBNGridVos
                  {
                      Id = s.Id,
                      MaTN = s.YeuCauVatTuBenhVien.YeuCauTiepNhan.MaYeuCauTiepNhan,
                      MaBN = s.YeuCauVatTuBenhVien.YeuCauTiepNhan.BenhNhan.MaBN,
                      HoTen = s.YeuCauVatTuBenhVien.YeuCauTiepNhan.HoTen,
                      SL = s.SoLuong,
                      DVKham = s.YeuCauVatTuBenhVien.YeuCauKhamBenh.TenDichVu ?? s.YeuCauVatTuBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu ?? null,
                      BSKeToa = s.YeuCauVatTuBenhVien.NhanVienChiDinh.User.HoTen,
                      NgayKe = s.YeuCauVatTuBenhVien.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                      SLDaLinh = s.SoLuongDaLinhBu != null ? s.SoLuongDaLinhBu : 0
                  });
                    var countTask = query.Skip(queryInfo.Skip)
                        .Take(queryInfo.Take).CountAsync();
                    await Task.WhenAll(countTask);

                    return new GridDataSource { TotalRowCount = countTask.Result };
                }
               
            }
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhDuTru)
            {
                var query = _yeuCauLinhVatTuChiTietRepository.TableNoTracking
                     .Where(o =>
                             o.YeuCauLinhVatTu.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhDuTru &&
                             o.YeuCauLinhVatTuId == long.Parse(queryString[0])
                            )
                    .Select(s => new DSLinhVatChildTuGridVo
                    {
                        Id = s.Id,
                        Ma = s.VatTuBenhVien.VatTus.Ma,
                        TenKSNK = s.VatTuBenhVien.VatTus.Ten,
                        DonViTinh = s.VatTuBenhVien.VatTus.DonViTinh,
                        SoLuongYc = s.SoLuong,
                        HangSanXuat = s.VatTuBenhVien.VatTus.NhaSanXuat,
                        NuocSanXuat = s.VatTuBenhVien.VatTus.NuocSanXuat,
                        BSKeToa = s.YeuCauVatTuBenhVien.NhanVienChiDinh.User.HoTen,
                        Nhom = s.LaVatTuBHYT == true ? "BHYT" : "Không BHYT",
                        TenKhoLinh = s.YeuCauLinhVatTu.KhoXuat.Ten
                    });
                var countTask = query.CountAsync();
                await Task.WhenAll(countTask);

                return new GridDataSource { TotalRowCount = countTask.Result };
            }
            return null;
        }
        #endregion
       
        public virtual byte[] ExportDanhSachLayDuTruLinh(ICollection<DSLinhKSNKGridVo> datalinhs)
        {
            var queryInfo = new DSLinhKSNKGridVo();
            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<DSLinhKSNKGridVo>("STT", p => ind++)
            };
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("DANH SÁCH YÊU CẦU LĨNH VẬT TƯ");

                    // set row

                    worksheet.Row(9).Height = 24.5;
                    worksheet.DefaultRowHeight = 25;
                    worksheet.Column(1).Width = 30;
                    worksheet.Column(2).Width = 30;
                    worksheet.Column(3).Width = 30;
                    worksheet.Column(4).Width = 30;
                    worksheet.Column(5).Width = 30;
                    worksheet.Column(6).Width = 30;
                    worksheet.Column(7).Width = 30;
                    worksheet.Column(8).Width = 30;
                    worksheet.Column(9).Width = 30;
                    worksheet.Column(10).Width = 30;
                    worksheet.Column(11).Width = 30;
                    worksheet.Column(12).Width = 30;
                    worksheet.Column(13).Width = 30;
                    worksheet.Column(14).Width = 30;
                    worksheet.Column(15).Width = 30;

                    worksheet.DefaultColWidth = 7;

                    //set column 
                    string[] SetColumnItems = { "A", "B", "C", "D", "E", "F", "G", "H", "A", "I", "J", "K", "L", "M", "N", "O", "P" };
                    var worksheetTitle = SetColumnItems[0] + 1 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 1;
                    var worksheetTitleStatus = SetColumnItems[0] + 2 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 2;
                    var worksheetTitleNgay = SetColumnItems[0] + 3 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 3;
                    var worksheetTitleHeader = SetColumnItems[0] + 3 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 6;

                    using (var range = worksheet.Cells[worksheetTitle])
                    {
                        range.Worksheet.Cells[worksheetTitle].Merge = true;
                        range.Worksheet.Cells[worksheetTitle].Value = "DANH SÁCH YÊU CẦU LĨNH VẬT TƯ".ToUpper();
                        range.Worksheet.Cells[worksheetTitle].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitle].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitle].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                        range.Worksheet.Cells[worksheetTitle].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitle].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells[worksheetTitleStatus])
                    {
                        range.Worksheet.Cells[worksheetTitleStatus].Merge = true;
                        //range.Worksheet.Cells[worksheetTitleStatus].Value = "Trạng thái: ";/*+ string.Join(", ", arrTrangThai);*/
                        range.Worksheet.Cells[worksheetTitleStatus].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleStatus].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleStatus].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                        range.Worksheet.Cells[worksheetTitleStatus].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleStatus].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells[worksheetTitleNgay])
                    {
                        range.Worksheet.Cells[worksheetTitleNgay].Merge = true;
                        //range.Worksheet.Cells[worksheetTitleNgay].Value = "Từ ngày: " + tuNgay?.ApplyFormatDate() + " - đến ngày: " + denNgay?.ApplyFormatDate();
                        range.Worksheet.Cells[worksheetTitleNgay].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleNgay].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Bold = true;
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Italic = true;
                    }

                    using (var range = worksheet.Cells[worksheetTitleHeader])
                    {
                        range.Worksheet.Cells[worksheetTitleHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.Bold = true;

                        string[,] SetColumns ={ { "A" , "Mã PL" }, { "B", "Loại" }, { "C", "Lĩnh từ kho" } , { "D", "Lĩnh về kho" },
                                    { "E", "Người yêu càu" }, { "F", "Ngày yêu cầu" },{ "G", "Tình Trạng" },{ "H", "Người duyệt" },{ "I", "Ngày duyệt" }};

                        for (int i = 0; i < SetColumns.Length / 2; i++)
                        {
                            var setColumn = ((SetColumns[i, 0]).ToString() + 4 + ":" + (SetColumns[i, 0]).ToString() + 6).ToString();
                            range.Worksheet.Cells[setColumn].Merge = true;
                            range.Worksheet.Cells[setColumn].Value = SetColumns[i, 1];
                        }

                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    var manager = new PropertyManager<DSLinhKSNKGridVo>(requestProperties);
                    int index = 7;
                    var worksheetFirstLast = SetColumnItems[0] + index + ":" + SetColumnItems[SetColumnItems.Length - 1] + index;

                    int sttYCTN = 1;
                    foreach (var yeuCauLinhDuocPham in datalinhs)
                    {
                        manager.CurrentObject = yeuCauLinhDuocPham;
                        manager.WriteToXlsx(worksheet, index);

                        worksheet.Cells["A" + index].Value = yeuCauLinhDuocPham.MaPL;
                        worksheet.Cells["B" + index].Value = yeuCauLinhDuocPham.Loai;
                        worksheet.Cells["C" + index].Value = yeuCauLinhDuocPham.LinhTuKho;
                        worksheet.Cells["D" + index].Value = yeuCauLinhDuocPham.LinhVeKho;
                        worksheet.Cells["E" + index].Value = yeuCauLinhDuocPham.NguoiYeuCau;
                        worksheet.Cells["F" + index].Value = yeuCauLinhDuocPham.NgayYeuCau.ApplyFormatDateTimeSACH();
                        worksheet.Cells["G" + index].Value = yeuCauLinhDuocPham.TinhTrang;
                        worksheet.Cells["H" + index].Value = yeuCauLinhDuocPham.Nguoiduyet;
                        worksheet.Cells["I" + index].Value = yeuCauLinhDuocPham.NgayDuyet != null ? yeuCauLinhDuocPham.NgayDuyet.Value.ApplyFormatDateTimeSACH() : "";

                        for (int ii = 0; ii < SetColumnItems.Length; ii++)
                        {
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Color.SetColor(Color.Black);
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Bold = true;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Fill.BackgroundColor.SetColor(Color.LightSteelBlue);
                        }

                        worksheet.Row(index).Height = 20.5;

                        var indexMain = index;
                        index++;
                        int sttItems = 1;
                        // Lĩnh bù
                        if (yeuCauLinhDuocPham.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu)
                        {
                            using (var range = worksheet.Cells["B" + index + ":J" + index])
                            {
                                range.Worksheet.Cells["B" + index + ":J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                range.Worksheet.Cells["B" + index + ":J" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                range.Worksheet.Cells["B" + index + ":J" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                range.Worksheet.Cells["B" + index + ":J" + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells["B" + index + ":J" + index].Style.Font.Bold = true;
                                range.Worksheet.Cells["B" + index + ":J" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                range.Worksheet.Cells["B" + index + ":J" + index].Style.Fill.BackgroundColor.SetColor(Color.LightSkyBlue);

                                string[,] SetColumnLinhBus = {  { "B", "#" },{ "C", "Tên Vật Tư" }, { "D", "Đơn Vị Tính" }, { "E", "Hãng SX" }, { "F", "Nước SX" } ,
                                    { "G", "SL Tồn" },{ "H", "SL Đã Bù" },{ "I", "SL Cần Bù"},{ "J", "SL Yêu Cầu" }};

                                for (int i = 0; i < SetColumnLinhBus.Length / 2; i++)
                                {
                                    var setColumn = ((SetColumnLinhBus[i, 0]).ToString() + index + ":" + (SetColumnLinhBus[i, 0]).ToString() + index).ToString();
                                    range.Worksheet.Cells[setColumn].Merge = true;
                                    range.Worksheet.Cells[setColumn].Value = SetColumnLinhBus[i, 1];
                                }
                                index++;
                                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            }
                            var KSNK = yeuCauLinhDuocPham.ListChildLinhBu
                                .Select(s => new YeuCauLinhKSNKBuGridVo
                                {
                                    TenKSNK = s.TenKSNK,
                                    DonViTinh = s.DonViTinh,
                                    HangSanXuat = s.HangSanXuat,
                                    NuocSanXuat = s.NuocSanXuat,
                                    SoLuongTon = s.SoLuongTon,
                                    SLDaLinh = s.SLDaLinh,
                                    SoLuongCanBu = s.SoLuongCanBu,
                                    SoLuongYeuCau = s.SoLuongYeuCau,
                                    ListChildChildLinhBu = s.ListChildChildLinhBu
                                }).ToList();
                            if (KSNK.Count() > 0)
                            {
                                foreach (var vt in KSNK)
                                {
                                    worksheet.Cells["B" + index].Value = sttItems++;
                                    worksheet.Cells["C" + index].Value = vt.TenKSNK;  // to do
                                    worksheet.Cells["D" + index].Value = vt.DonViTinh;
                                    worksheet.Cells["E" + index].Value = vt.HangSanXuat;
                                    worksheet.Cells["F" + index].Value = vt.NuocSanXuat;
                                    worksheet.Cells["G" + index].Value = vt.SoLuongTon + "";
                                    worksheet.Cells["H" + index].Value = vt.SLDaLinh + "";
                                    worksheet.Cells["I" + index].Value = vt.SoLuongCanBu + "";
                                    worksheet.Cells["J" + index].Value = vt.SoLuongYeuCau + "";


                                    for (int ii = 0; ii < SetColumnItems.Length; ii++)
                                    {
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    }
                                    index++;
                                    using (var range = worksheet.Cells["C" + index + ":M" + index])
                                    {
                                        range.Worksheet.Cells["C" + index + ":J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        range.Worksheet.Cells["C" + index + ":J" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                        range.Worksheet.Cells["C" + index + ":J" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                        range.Worksheet.Cells["C" + index + ":J" + index].Style.Font.Color.SetColor(Color.Black);
                                        range.Worksheet.Cells["C" + index + ":J" + index].Style.Font.Bold = true;
                                        range.Worksheet.Cells["C" + index + ":J" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        range.Worksheet.Cells["C" + index + ":J" + index].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

                                        //Set column 
                                        string[,] SetColumnDichVus = { { "C" , "#" },{ "D" , "Mã Tiếp Nhận" }, { "E", "Mã Người Bệnh" }, { "F", "Họ Tên" } , { "G", "Dịch Vụ Khám" },
                                    { "H", "Bác Sỹ Kê Vật Tư" },{ "I", "Ngày Điều Trị" } ,{ "J", "Ngày Kê" } ,{ "K", "Số Lượng Đã Bù" },{ "L", "Số Lượng Cần Bù" },{ "M", "SL Yêu Cầu" } };

                                        for (int i = 0; i < SetColumnDichVus.Length / 2; i++)
                                        {
                                            var setColumn = ((SetColumnDichVus[i, 0]).ToString() + index + ":" + (SetColumnDichVus[i, 0]).ToString() + index).ToString();
                                            range.Worksheet.Cells[setColumn].Merge = true;
                                            range.Worksheet.Cells[setColumn].Value = SetColumnDichVus[i, 1];
                                        }

                                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    }
                                    index++;

                                    var sttDichVu = 1;
                                    foreach (var dichVu in vt.ListChildChildLinhBu)
                                    {
                                        worksheet.Cells["C" + index].Value = sttDichVu++;
                                        worksheet.Cells["D" + index].Value = dichVu.MaTN;
                                        worksheet.Cells["E" + index].Value = dichVu.MaBN;
                                        worksheet.Cells["F" + index].Value = dichVu.HoTen;
                                        worksheet.Cells["G" + index].Value = dichVu.DVKham;
                                        worksheet.Cells["H" + index].Value = dichVu.BSKeToa;
                                        worksheet.Cells["I" + index].Value = dichVu.NgayDieuTriString;
                                        worksheet.Cells["J" + index].Value = dichVu.NgayKe;
                                        worksheet.Cells["K" + index].Value = dichVu.SLDaLinh + "";
                                        worksheet.Cells["L" + index].Value = dichVu.SL + "";
                                        worksheet.Cells["M" + index].Value = dichVu.SLDanhSachDuyet + "";

                                        for (int ii = 0; ii < SetColumnItems.Length; ii++)
                                        {
                                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                        }

                                        index++;
                                    }
                                }
                            }
                        }
                        // lĩnh thường
                        if (yeuCauLinhDuocPham.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhDuTru)
                        {

                            using (var range = worksheet.Cells["B" + index + ":G" + index])
                            {
                                range.Worksheet.Cells["B" + index + ":G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                range.Worksheet.Cells["B" + index + ":G" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                range.Worksheet.Cells["B" + index + ":G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                range.Worksheet.Cells["B" + index + ":G" + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells["B" + index + ":G" + index].Style.Font.Bold = true;
                                range.Worksheet.Cells["B" + index + ":G" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                range.Worksheet.Cells["B" + index + ":G" + index].Style.Fill.BackgroundColor.SetColor(Color.LightSkyBlue);

                                string[,] SetColumnLinhDuTrus = { { "B", "#" },{ "C", "Tên Vật Tư" }, { "d", "Đơn Vị Tính" }, { "E", "Hãng SX" } ,
                                { "F", "Nước SX" },{ "G", "Số Lượng Yêu Cầu" }};

                                for (int i = 0; i < SetColumnLinhDuTrus.Length / 2; i++)
                                {
                                    var setColumn = ((SetColumnLinhDuTrus[i, 0]).ToString() + index + ":" + (SetColumnLinhDuTrus[i, 0]).ToString() + index).ToString();
                                    range.Worksheet.Cells[setColumn].Merge = true;
                                    range.Worksheet.Cells[setColumn].Value = SetColumnLinhDuTrus[i, 1];
                                }
                                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            }
                            index++;
                            var dutruthuongBHYT = yeuCauLinhDuocPham.ListChildLinhDuTru.Where(s => s.LaBHYT == true)
                               .Select(k => new DSLinhVatChildTuGridVo()
                               {
                                   TenKSNK = k.TenKSNK,
                                   Nhom = k.Nhom,
                                   DonViTinh = k.DonViTinh,
                                   HangSanXuat = k.HangSanXuat,
                                   NuocSanXuat = k.NuocSanXuat,
                                   SoLuongYeuCau = k.SoLuongYc
                               }).ToList();
                            var dutruthuongKhongBHYT = yeuCauLinhDuocPham.ListChildLinhDuTru.Where(s => s.LaBHYT == false)
                               .Select(k => new DSLinhVatChildTuGridVo()
                               {
                                   TenKSNK = k.TenKSNK,
                                   Nhom = k.Nhom,
                                   DonViTinh = k.DonViTinh,
                                   HangSanXuat = k.HangSanXuat,
                                   NuocSanXuat = k.NuocSanXuat,
                                   SoLuongYeuCau = k.SoLuongYc
                               }).ToList();
                            if (dutruthuongBHYT.Count() > 0)
                            {
                                using (var range = worksheet.Cells["B" + index + ":G" + index])
                                {
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.Color.SetColor(Color.Black);
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.Bold = true;
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Fill.BackgroundColor.SetColor(Color.RosyBrown);
                                    //Set column A to K
                                    string[,] SetColumnLoaiDuocPham = { { "B", "BHYT" } };

                                    for (int i = 0; i < SetColumnLoaiDuocPham.Length / 2; i++)
                                    {
                                        var setColumn = ((SetColumnLoaiDuocPham[i, 0]).ToString() + index + ":" + (SetColumnLoaiDuocPham[i, 0]).ToString() + index).ToString();
                                        range.Worksheet.Cells[setColumn].Merge = true;
                                        range.Worksheet.Cells[setColumn].Value = SetColumnLoaiDuocPham[i, 1];
                                    }

                                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                }
                                index++;
                                foreach (var nhom in dutruthuongBHYT)
                                {
                                    worksheet.Cells["B" + index].Value = sttItems++;
                                    worksheet.Cells["C" + index].Value = nhom.TenKSNK;  // to do
                                    worksheet.Cells["D" + index].Value = nhom.DonViTinh;
                                    worksheet.Cells["E" + index].Value = nhom.HangSanXuat;
                                    worksheet.Cells["F" + index].Value = nhom.NuocSanXuat;
                                    worksheet.Cells["G" + index].Value = nhom.SoLuongYeuCau + "";

                                    for (int ii = 0; ii < SetColumnItems.Length; ii++)
                                    {
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    }
                                    index++;
                                }
                            }
                            if (dutruthuongKhongBHYT.Count() > 0)
                            {
                                using (var range = worksheet.Cells["B" + index + ":G" + index])
                                {
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.Color.SetColor(Color.Black);
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.Bold = true;
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Fill.BackgroundColor.SetColor(Color.RosyBrown);
                                    //Set column A to K
                                    string[,] SetColumnLoaiDuocPham = { { "B", "Không BHYT" } };

                                    for (int i = 0; i < SetColumnLoaiDuocPham.Length / 2; i++)
                                    {
                                        var setColumn = ((SetColumnLoaiDuocPham[i, 0]).ToString() + index + ":" + (SetColumnLoaiDuocPham[i, 0]).ToString() + index).ToString();
                                        range.Worksheet.Cells[setColumn].Merge = true;
                                        range.Worksheet.Cells[setColumn].Value = SetColumnLoaiDuocPham[i, 1];
                                    }

                                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                }
                                index++;
                                foreach (var nhom in dutruthuongKhongBHYT)
                                {
                                    worksheet.Cells["B" + index].Value = sttItems++;
                                    worksheet.Cells["C" + index].Value = nhom.TenKSNK;  // to do
                                    worksheet.Cells["D" + index].Value = nhom.DonViTinh;
                                    worksheet.Cells["E" + index].Value = nhom.HangSanXuat;
                                    worksheet.Cells["F" + index].Value = nhom.NuocSanXuat;
                                    worksheet.Cells["G" + index].Value = nhom.SoLuongYeuCau + "";

                                    for (int ii = 0; ii < SetColumnItems.Length; ii++)
                                    {
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    }
                                    index++;
                                }
                            }

                        }

                        for (int inde = indexMain + 1; inde <= index - 1; inde++)
                        {
                            worksheet.Row(inde).OutlineLevel = 1;
                        }

                        index++;
                    }

                    xlPackage.Save();
                }

                return stream.ToArray();
            }
        }
        public virtual byte[] ExportDanhSachDuyetLayDuTruLinh(ICollection<DSLinhKSNKGridVo> datalinhs)
        {
            var queryInfo = new DSLinhKSNKGridVo();
            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<DSLinhKSNKGridVo>("STT", p => ind++)
            };
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("DANH SÁCH DUYỆT LĨNH VẬT TƯ");

                    // set row

                    worksheet.Row(9).Height = 24.5;
                    worksheet.DefaultRowHeight = 25;
                    worksheet.Column(1).Width = 30;
                    worksheet.Column(2).Width = 30;
                    worksheet.Column(3).Width = 30;
                    worksheet.Column(4).Width = 30;
                    worksheet.Column(5).Width = 30;
                    worksheet.Column(6).Width = 30;
                    worksheet.Column(7).Width = 30;
                    worksheet.Column(8).Width = 30;
                    worksheet.Column(9).Width = 30;
                    worksheet.Column(10).Width = 30;
                    worksheet.Column(11).Width = 30;
                    worksheet.Column(12).Width = 30;
                    worksheet.Column(13).Width = 30;
                    worksheet.Column(14).Width = 30;
                    worksheet.Column(15).Width = 30;

                    worksheet.DefaultColWidth = 7;

                    //set column 
                    string[] SetColumnItems = { "A", "B", "C", "D", "E", "F", "G", "H", "A", "I", "J", "K", "L", "M", "N", "O" };
                    var worksheetTitle = SetColumnItems[0] + 1 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 1;
                    var worksheetTitleStatus = SetColumnItems[0] + 2 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 2;
                    var worksheetTitleNgay = SetColumnItems[0] + 3 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 3;
                    var worksheetTitleHeader = SetColumnItems[0] + 3 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 6;

                    using (var range = worksheet.Cells[worksheetTitle])
                    {
                        range.Worksheet.Cells[worksheetTitle].Merge = true;
                        range.Worksheet.Cells[worksheetTitle].Value = "DANH SÁCH DUYỆT LĨNH VẬT TƯ".ToUpper();
                        range.Worksheet.Cells[worksheetTitle].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitle].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitle].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                        range.Worksheet.Cells[worksheetTitle].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitle].Style.Font.Bold = true;
                    }

                    //using (var range = worksheet.Cells[worksheetTitleStatus])
                    //{
                    //    range.Worksheet.Cells[worksheetTitleStatus].Merge = true;
                    //    //range.Worksheet.Cells[worksheetTitleStatus].Value = "Trạng thái: ";/*+ string.Join(", ", arrTrangThai);*/
                    //    range.Worksheet.Cells[worksheetTitleStatus].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    //    range.Worksheet.Cells[worksheetTitleStatus].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    //    range.Worksheet.Cells[worksheetTitleStatus].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                    //    range.Worksheet.Cells[worksheetTitleStatus].Style.Font.Color.SetColor(Color.Black);
                    //    range.Worksheet.Cells[worksheetTitleStatus].Style.Font.Bold = true;
                    //}

                    //using (var range = worksheet.Cells[worksheetTitleNgay])
                    //{
                    //    range.Worksheet.Cells[worksheetTitleNgay].Merge = true;
                    //    //range.Worksheet.Cells[worksheetTitleNgay].Value = "Từ ngày: " + tuNgay?.ApplyFormatDate() + " - đến ngày: " + denNgay?.ApplyFormatDate();
                    //    range.Worksheet.Cells[worksheetTitleNgay].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    //    range.Worksheet.Cells[worksheetTitleNgay].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                    //    range.Worksheet.Cells[worksheetTitleNgay].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                    //    range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Color.SetColor(Color.Black);
                    //    range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Bold = true;
                    //    range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Italic = true;
                    //}

                    using (var range = worksheet.Cells[worksheetTitleHeader])
                    {
                        range.Worksheet.Cells[worksheetTitleHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.Bold = true;

                        string[,] SetColumns ={ { "A" , "Mã PL" }, { "B", "Loại" }, { "C", "Lĩnh từ kho" } , { "D", "Lĩnh về kho" },
                                    { "E", "Người yêu càu" }, { "F", "Ngày yêu cầu" },{ "G", "Tình Trạng" },{ "H", "Người duyệt" },{ "I", "Ngày duyệt" }};

                        for (int i = 0; i < SetColumns.Length / 2; i++)
                        {
                            var setColumn = ((SetColumns[i, 0]).ToString() + 4 + ":" + (SetColumns[i, 0]).ToString() + 6).ToString();
                            range.Worksheet.Cells[setColumn].Merge = true;
                            range.Worksheet.Cells[setColumn].Value = SetColumns[i, 1];
                        }

                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    var manager = new PropertyManager<DSLinhKSNKGridVo>(requestProperties);
                    int index = 7;
                    var worksheetFirstLast = SetColumnItems[0] + index + ":" + SetColumnItems[SetColumnItems.Length - 1] + index;

                    int sttYCTN = 1;
                    foreach (var yeuCauLinhDuocPham in datalinhs)
                    {
                        manager.CurrentObject = yeuCauLinhDuocPham;
                        manager.WriteToXlsx(worksheet, index);

                        worksheet.Cells["A" + index].Value = yeuCauLinhDuocPham.MaPL;
                        worksheet.Cells["B" + index].Value = yeuCauLinhDuocPham.Loai;
                        worksheet.Cells["C" + index].Value = yeuCauLinhDuocPham.LinhTuKho;
                        worksheet.Cells["D" + index].Value = yeuCauLinhDuocPham.LinhVeKho;
                        worksheet.Cells["E" + index].Value = yeuCauLinhDuocPham.NguoiYeuCau;
                        worksheet.Cells["F" + index].Value = yeuCauLinhDuocPham.NgayYeuCau.ApplyFormatDateTimeSACH();
                        worksheet.Cells["G" + index].Value = yeuCauLinhDuocPham.TinhTrang;
                        worksheet.Cells["H" + index].Value = yeuCauLinhDuocPham.Nguoiduyet;
                        worksheet.Cells["I" + index].Value = yeuCauLinhDuocPham.NgayDuyet != null ? yeuCauLinhDuocPham.NgayDuyet.Value.ApplyFormatDateTimeSACH() : "";

                        for (int ii = 0; ii < SetColumnItems.Length; ii++)
                        {
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Color.SetColor(Color.Black);
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Bold = true;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Fill.BackgroundColor.SetColor(Color.LightSteelBlue);
                        }

                        worksheet.Row(index).Height = 20.5;

                        var indexMain = index;
                        index++;
                        int sttItems = 1;
                        // Lĩnh bù
                        if (yeuCauLinhDuocPham.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu)
                        {
                            using (var range = worksheet.Cells["B" + index + ":J" + index])
                            {
                                range.Worksheet.Cells["B" + index + ":J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                range.Worksheet.Cells["B" + index + ":J" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                range.Worksheet.Cells["B" + index + ":J" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                range.Worksheet.Cells["B" + index + ":J" + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells["B" + index + ":J" + index].Style.Font.Bold = true;
                                range.Worksheet.Cells["B" + index + ":J" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                range.Worksheet.Cells["B" + index + ":J" + index].Style.Fill.BackgroundColor.SetColor(Color.LightSkyBlue);

                                string[,] SetColumnLinhBus = {  { "B", "#" },{ "C", "Tên Vật Tư" }, { "D", "Đơn Vị Tính" }, { "E", "Hãng SX" }, { "F", "Nước SX" } ,
                                    { "G", "SL Tồn" },{ "H", "SL Đã Bù" },{ "I", "SL Cần Bù"},{ "J", "SL Yêu Cầu" }};

                                for (int i = 0; i < SetColumnLinhBus.Length / 2; i++)
                                {
                                    var setColumn = ((SetColumnLinhBus[i, 0]).ToString() + index + ":" + (SetColumnLinhBus[i, 0]).ToString() + index).ToString();
                                    range.Worksheet.Cells[setColumn].Merge = true;
                                    range.Worksheet.Cells[setColumn].Value = SetColumnLinhBus[i, 1];
                                }
                                index++;
                                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            }
                            var KSNK = yeuCauLinhDuocPham.ListChildLinhBu
                                .Select(s => new YeuCauLinhKSNKBuGridVo
                                {
                                    TenKSNK = s.TenKSNK,
                                    DonViTinh = s.DonViTinh,
                                    HangSanXuat = s.HangSanXuat,
                                    NuocSanXuat = s.NuocSanXuat,
                                    SoLuongTon = s.SoLuongTon,
                                    SLDaLinh = s.SLDaLinh,
                                    SoLuongCanBu = s.SoLuongCanBu,
                                    SoLuongYeuCau = s.SoLuongYeuCau,
                                    ListChildChildLinhBu = s.ListChildChildLinhBu,
                                }).ToList();
                            if (KSNK.Count() > 0)
                            {
                                foreach (var nhom in KSNK)
                                {
                                    worksheet.Cells["B" + index].Value = sttItems++;
                                    worksheet.Cells["C" + index].Value = nhom.TenKSNK;  // to do
                                    worksheet.Cells["D" + index].Value = nhom.DonViTinh;
                                    worksheet.Cells["E" + index].Value = nhom.HangSanXuat;
                                    worksheet.Cells["F" + index].Value = nhom.NuocSanXuat;
                                    worksheet.Cells["G" + index].Value = nhom.SoLuongTon + "";
                                    worksheet.Cells["H" + index].Value = nhom.SLDaLinh + "";
                                    worksheet.Cells["I" + index].Value = nhom.SoLuongCanBu + "";
                                    worksheet.Cells["J" + index].Value = nhom.SoLuongYeuCau + "";


                                    for (int ii = 0; ii < SetColumnItems.Length; ii++)
                                    {
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    }
                                    index++;
                                    using (var range = worksheet.Cells["C" + index + ":M" + index])
                                    {
                                        range.Worksheet.Cells["C" + index + ":J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        range.Worksheet.Cells["C" + index + ":J" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                        range.Worksheet.Cells["C" + index + ":J" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                        range.Worksheet.Cells["C" + index + ":J" + index].Style.Font.Color.SetColor(Color.Black);
                                        range.Worksheet.Cells["C" + index + ":J" + index].Style.Font.Bold = true;
                                        range.Worksheet.Cells["C" + index + ":J" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        range.Worksheet.Cells["C" + index + ":J" + index].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

                                        //Set column 
                                        string[,] SetColumnDichVus = { { "C" , "#" },{ "D" , "Mã Tiếp Nhận" }, { "E", "Mã Người Bệnh" }, { "F", "Họ Tên" } , { "G", "Dịch Vụ Khám" },
                                    { "H", "Bác Sỹ Kê Toa Vật Tư" },{ "I", "Ngày Điều Trị" }, { "J", "Ngày Kê" },{ "K", "Số Lượng Cần Bù" },{ "L", "Số Lượng Đã Bù" },{ "M", "SL Được Duyệt" }};

                                        for (int i = 0; i < SetColumnDichVus.Length / 2; i++)
                                        {
                                            var setColumn = ((SetColumnDichVus[i, 0]).ToString() + index + ":" + (SetColumnDichVus[i, 0]).ToString() + index).ToString();
                                            range.Worksheet.Cells[setColumn].Merge = true;
                                            range.Worksheet.Cells[setColumn].Value = SetColumnDichVus[i, 1];
                                        }

                                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    }
                                    index++;

                                    var sttDichVu = 1;
                                    foreach (var dichVu in nhom.ListChildChildLinhBu)
                                    {
                                        worksheet.Cells["C" + index].Value = sttDichVu++;
                                        worksheet.Cells["D" + index].Value = dichVu.MaTN;
                                        worksheet.Cells["E" + index].Value = dichVu.MaBN;
                                        worksheet.Cells["F" + index].Value = dichVu.HoTen;
                                        worksheet.Cells["G" + index].Value = dichVu.DVKham;
                                        worksheet.Cells["H" + index].Value = dichVu.BSKeToa;
                                        worksheet.Cells["I" + index].Value = dichVu.NgayDieuTriString;
                                        worksheet.Cells["J" + index].Value = dichVu.NgayKe;
                                        worksheet.Cells["K" + index].Value = dichVu.SLDaLinh + "";
                                        worksheet.Cells["L" + index].Value = dichVu.SL + "";
                                        worksheet.Cells["M" + index].Value = dichVu.SLDanhSachDuyet + "";

                                        for (int ii = 0; ii < SetColumnItems.Length; ii++)
                                        {
                                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                        }

                                        index++;
                                    }
                                }
                            }
                        }
                        // lĩnh thường
                        if (yeuCauLinhDuocPham.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhDuTru)
                        {

                            using (var range = worksheet.Cells["B" + index + ":G" + index])
                            {
                                range.Worksheet.Cells["B" + index + ":G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                range.Worksheet.Cells["B" + index + ":G" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                range.Worksheet.Cells["B" + index + ":G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                range.Worksheet.Cells["B" + index + ":G" + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells["B" + index + ":G" + index].Style.Font.Bold = true;
                                range.Worksheet.Cells["B" + index + ":G" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                range.Worksheet.Cells["B" + index + ":G" + index].Style.Fill.BackgroundColor.SetColor(Color.LightSkyBlue);

                                string[,] SetColumnLinhDuTrus = { { "B", "#" },{ "C", "Tên Vật Tư" }, { "d", "Đơn Vị Tính" }, { "E", "Hãng SX" } ,
                                { "F", "Nước SX" },{ "G", "Số Lượng Yêu Cầu" }};

                                for (int i = 0; i < SetColumnLinhDuTrus.Length / 2; i++)
                                {
                                    var setColumn = ((SetColumnLinhDuTrus[i, 0]).ToString() + index + ":" + (SetColumnLinhDuTrus[i, 0]).ToString() + index).ToString();
                                    range.Worksheet.Cells[setColumn].Merge = true;
                                    range.Worksheet.Cells[setColumn].Value = SetColumnLinhDuTrus[i, 1];
                                }
                                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            }
                            index++;
                            var dutruthuongBHYT = yeuCauLinhDuocPham.ListChildLinhDuTru.Where(s => s.LaBHYT == true)
                               .Select(k => new DSLinhVatChildTuGridVo()
                               {
                                   TenKSNK = k.TenKSNK,
                                   Nhom = k.Nhom,
                                   DonViTinh = k.DonViTinh,
                                   HangSanXuat = k.HangSanXuat,
                                   NuocSanXuat = k.NuocSanXuat,
                                   SoLuongYeuCau = k.SoLuongYc
                               }).ToList();
                            var dutruthuongKhongBHYT = yeuCauLinhDuocPham.ListChildLinhDuTru.Where(s => s.LaBHYT == false)
                               .Select(k => new DSLinhVatChildTuGridVo()
                               {
                                   TenKSNK = k.TenKSNK,
                                   Nhom = k.Nhom,
                                   DonViTinh = k.DonViTinh,
                                   HangSanXuat = k.HangSanXuat,
                                   NuocSanXuat = k.NuocSanXuat,
                                   SoLuongYeuCau = k.SoLuongYc
                               }).ToList();
                            if (dutruthuongBHYT.Count() > 0)
                            {
                                using (var range = worksheet.Cells["B" + index + ":G" + index])
                                {
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.Color.SetColor(Color.Black);
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.Bold = true;
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Fill.BackgroundColor.SetColor(Color.RosyBrown);
                                    //Set column A to K
                                    string[,] SetColumnLoaiDuocPham = { { "B", "BHYT" } };

                                    for (int i = 0; i < SetColumnLoaiDuocPham.Length / 2; i++)
                                    {
                                        var setColumn = ((SetColumnLoaiDuocPham[i, 0]).ToString() + index + ":" + (SetColumnLoaiDuocPham[i, 0]).ToString() + index).ToString();
                                        range.Worksheet.Cells[setColumn].Merge = true;
                                        range.Worksheet.Cells[setColumn].Value = SetColumnLoaiDuocPham[i, 1];
                                    }

                                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                }
                                index++;
                                foreach (var nhom in dutruthuongBHYT)
                                {
                                    worksheet.Cells["B" + index].Value = sttItems++;
                                    worksheet.Cells["C" + index].Value = nhom.TenKSNK;  // to do
                                    worksheet.Cells["D" + index].Value = nhom.DonViTinh;
                                    worksheet.Cells["E" + index].Value = nhom.HangSanXuat;
                                    worksheet.Cells["F" + index].Value = nhom.NuocSanXuat;
                                    worksheet.Cells["G" + index].Value = nhom.SoLuongYeuCau + "";

                                    for (int ii = 0; ii < SetColumnItems.Length; ii++)
                                    {
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    }
                                    index++;
                                }
                            }
                            if (dutruthuongKhongBHYT.Count() > 0)
                            {
                                using (var range = worksheet.Cells["B" + index + ":G" + index])
                                {
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.Color.SetColor(Color.Black);
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.Bold = true;
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    range.Worksheet.Cells["B" + index + ":B" + index].Style.Fill.BackgroundColor.SetColor(Color.RosyBrown);
                                    //Set column A to K
                                    string[,] SetColumnLoaiDuocPham = { { "B", "Không BHYT" } };

                                    for (int i = 0; i < SetColumnLoaiDuocPham.Length / 2; i++)
                                    {
                                        var setColumn = ((SetColumnLoaiDuocPham[i, 0]).ToString() + index + ":" + (SetColumnLoaiDuocPham[i, 0]).ToString() + index).ToString();
                                        range.Worksheet.Cells[setColumn].Merge = true;
                                        range.Worksheet.Cells[setColumn].Value = SetColumnLoaiDuocPham[i, 1];
                                    }

                                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                }
                                index++;
                                foreach (var nhom in dutruthuongKhongBHYT)
                                {
                                    worksheet.Cells["B" + index].Value = sttItems++;
                                    worksheet.Cells["C" + index].Value = nhom.TenKSNK;  // to do
                                    worksheet.Cells["D" + index].Value = nhom.DonViTinh;
                                    worksheet.Cells["E" + index].Value = nhom.HangSanXuat;
                                    worksheet.Cells["F" + index].Value = nhom.NuocSanXuat;
                                    worksheet.Cells["G" + index].Value = nhom.SoLuongYeuCau + "";

                                    for (int ii = 0; ii < SetColumnItems.Length; ii++)
                                    {
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    }
                                    index++;
                                }
                            }

                        }

                        for (int inde = indexMain + 1; inde <= index - 1; inde++)
                        {
                            worksheet.Row(inde).OutlineLevel = 1;
                        }

                        index++;
                    }

                    xlPackage.Save();
                }

                return stream.ToArray();
            }
        }

        #region ds yeu cau
        public async Task<GridDataSource> GetDataDSYeuCauLinhKSNKChildChildForGridAsync(QueryInfo queryInfo)
        {
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            int loaiPhieulinh = int.Parse(queryString[1]);
            

            EnumLoaiPhieuLinh loaiPhieu;
            if (loaiPhieulinh == 1)
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhDuTru;
            }
            else 
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhBu;
            }
            
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhBu)
            {
                bool loaiDuocPhamHayVatTu = bool.Parse(queryString[3]);
                if (loaiDuocPhamHayVatTu == true)
                {
                    BuildDefaultSortExpression(queryInfo);
                    var yeuCauLinhDuocPhamId = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking.Where(x => x.Id == long.Parse(queryString[0])).Select(s => s.YeuCauLinhDuocPhamId).FirstOrDefault();
                    var query = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                                .Where(p => p.YeuCauLinhDuocPhamId == yeuCauLinhDuocPhamId
                               && p.YeuCauDuocPhamBenhVien.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                                && (p.YeuCauLinhDuocPham.DuocDuyet == false ? p.YeuCauDuocPhamBenhVien.KhongLinhBu != false : p.YeuCauDuocPhamBenhVien.KhongLinhBu != true)
                                && p.YeuCauDuocPhamBenhVien.KhoLinh.LaKhoKSNK == true)
                    .OrderBy(x => x.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh)
                    .Select(s => new KSNKLinhBuCuaBNGridVos
                    {
                        Id = s.Id,
                        MaTN = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.MaYeuCauTiepNhan,
                        MaBN = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.BenhNhan.MaBN,
                        HoTen = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.HoTen,
                        SL = s.SoLuongCanBu,
                        DVKham = s.YeuCauDuocPhamBenhVien.YeuCauKhamBenh.TenDichVu ?? s.YeuCauDuocPhamBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu ?? null,
                        BSKeToa = s.YeuCauDuocPhamBenhVien.NhanVienChiDinh.User.HoTen,
                        NgayKe = s.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                        SLDaLinh = s.SoLuongDaLinhBu != null ? s.SoLuongDaLinhBu : 0
                    });
                    var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
                    var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                        .Take(queryInfo.Take).ToArrayAsync();
                    await Task.WhenAll(countTask, queryTask);
                    return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
                }
                else
                {
                    BuildDefaultSortExpression(queryInfo);
                    var trangThai = _yeuCauLinhVatTuChiTietRepository.TableNoTracking.Where(x => x.Id == long.Parse(queryString[0])).Select(s => s.YeuCauLinhVatTu.DuocDuyet).FirstOrDefault();
                    var yeuCauLinhKSNKId = _yeuCauLinhVatTuChiTietRepository.TableNoTracking.Where(x => x.Id == long.Parse(queryString[0])).Select(s => s.YeuCauLinhVatTuId).FirstOrDefault();
                    var query = _yeuCauLinhVatTuChiTietRepository.TableNoTracking
                             .Where(p => p.YeuCauLinhVatTuId == yeuCauLinhKSNKId
                                && p.YeuCauLinhVatTu.DuocDuyet == trangThai
                                 && p.VatTuBenhVienId == long.Parse(queryString[2])
                                 && p.LaVatTuBHYT == bool.Parse(queryString[3])
                                 && p.YeuCauVatTuBenhVien.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhBu
                             //&& (p.YeuCauLinhKSNK.DuocDuyet == false ? p.YeuCauVatTuBenhVien.KhongLinhBu != false : p.YeuCauVatTuBenhVien.KhongLinhBu != true)
                             && p.YeuCauVatTuBenhVien.KhoLinh.LaKhoKSNK == true)
                 .OrderBy(x => x.YeuCauVatTuBenhVien.ThoiDiemChiDinh)
                    .Select(s => new KSNKLinhBuCuaBNGridVos
                    {
                        Id = s.Id,
                        MaTN = s.YeuCauVatTuBenhVien.YeuCauTiepNhan.MaYeuCauTiepNhan,
                        MaBN = s.YeuCauVatTuBenhVien.YeuCauTiepNhan.BenhNhan.MaBN,
                        HoTen = s.YeuCauVatTuBenhVien.YeuCauTiepNhan.HoTen,
                        SL = trangThai == true ? s.SoLuong : s.SoLuongCanBu,
                        DVKham = s.YeuCauVatTuBenhVien.YeuCauKhamBenh.TenDichVu ?? s.YeuCauVatTuBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu ?? null,
                        BSKeToa = s.YeuCauVatTuBenhVien.NhanVienChiDinh.User.HoTen,
                        NgayKe = s.YeuCauVatTuBenhVien.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                        SLDaLinh = s.SoLuongDaLinhBu != null ? s.SoLuongDaLinhBu : 0,
                        SLDanhSachDuyet = s.SoLuong.MathRoundNumber(2),
                        SLCanBu = s.SoLuongCanBu,
                        NgayDieuTri = s.YeuCauVatTuBenhVien.NoiTruPhieuDieuTri != null ? s.YeuCauVatTuBenhVien.NoiTruPhieuDieuTri.NgayDieuTri : s.YeuCauVatTuBenhVien.ThoiDiemChiDinh
                    });
                    var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
                    var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                        .Take(queryInfo.Take).ToArrayAsync();
                    await Task.WhenAll(countTask, queryTask);
                    return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
                }
                
            }
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhDuTru)
            {
                BuildDefaultSortExpression(queryInfo);
                var query = _yeuCauLinhVatTuChiTietRepository.TableNoTracking
                     .Where(o =>
                             o.YeuCauLinhVatTu.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhDuTru &&
                             o.YeuCauLinhVatTuId == long.Parse(queryString[0])
                            )
                    .Select(s => new DSLinhVatChildTuGridVo
                    {
                        Id = s.Id,
                        Ma = s.VatTuBenhVien.VatTus.Ma,
                        TenKSNK = s.VatTuBenhVien.VatTus.Ten,
                        DonViTinh = s.VatTuBenhVien.VatTus.DonViTinh,
                        SoLuongYc = s.SoLuong,
                        HangSanXuat = s.VatTuBenhVien.VatTus.NhaSanXuat,
                        NuocSanXuat = s.VatTuBenhVien.VatTus.NuocSanXuat,
                        BSKeToa = s.YeuCauVatTuBenhVien.NhanVienChiDinh.User.HoTen,
                        Nhom = s.LaVatTuBHYT == true ? "BHYT" : "Không BHYT"
                    });
                var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
                var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                    .Take(queryInfo.Take).ToArrayAsync();

                await Task.WhenAll(countTask, queryTask);
                return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
            }
            return null;
        }
        public async Task<GridDataSource> GetTotalPageFDSYeuCauLinhKSNKChildChildForGridAsync(QueryInfo queryInfo)
        {
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            int loaiPhieulinh = int.Parse(queryString[1]);
           

            EnumLoaiPhieuLinh loaiPhieu;
            if (loaiPhieulinh == 1)
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhDuTru;
            }
            else 
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhBu;
            }
            
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhBu)
            {
                bool loaiDuocPhamHayVatTu = bool.Parse(queryString[3]);
                if (loaiDuocPhamHayVatTu == true)
                {
                    var yeuCauLinhDuocPhamId = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking.Where(x => x.Id == long.Parse(queryString[0])).Select(s => s.YeuCauLinhDuocPhamId).FirstOrDefault();
                    var query = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                                .Where(p => p.YeuCauLinhDuocPhamId == yeuCauLinhDuocPhamId
                               && p.YeuCauDuocPhamBenhVien.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                                && (p.YeuCauLinhDuocPham.DuocDuyet == false ? p.YeuCauDuocPhamBenhVien.KhongLinhBu != false : p.YeuCauDuocPhamBenhVien.KhongLinhBu != true)
                                && p.YeuCauDuocPhamBenhVien.KhoLinh.LaKhoKSNK == true)
                    .OrderBy(x => x.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh)
                    .Select(s => new KSNKLinhBuCuaBNGridVos
                    {
                        Id = s.Id,
                        MaTN = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.MaYeuCauTiepNhan,
                        MaBN = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.BenhNhan.MaBN,
                        HoTen = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.HoTen,
                        SL = s.SoLuongCanBu,
                        DVKham = s.YeuCauDuocPhamBenhVien.YeuCauKhamBenh.TenDichVu ?? s.YeuCauDuocPhamBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu ?? null,
                        BSKeToa = s.YeuCauDuocPhamBenhVien.NhanVienChiDinh.User.HoTen,
                        NgayKe = s.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                        SLDaLinh = s.SoLuongDaLinhBu != null ? s.SoLuongDaLinhBu : 0
                    });
                    var countTask = query.Skip(queryInfo.Skip)
                        .Take(queryInfo.Take).CountAsync();
                    await Task.WhenAll(countTask);

                    return new GridDataSource { TotalRowCount = countTask.Result };
                }
                else
                {
                    var yeuCauLinhKSNKId = _yeuCauLinhVatTuChiTietRepository.TableNoTracking.Where(x => x.Id == long.Parse(queryString[0])).Select(s => s.YeuCauLinhVatTuId).FirstOrDefault();
                    var query = _yeuCauLinhVatTuChiTietRepository.TableNoTracking
                           .Where(p => p.YeuCauLinhVatTuId == yeuCauLinhKSNKId
                                //&& p.YeuCauVatTuBenhVien.TrangThai == EnumYeuCauVatTuBenhVien.DaThucHien
                                && (p.YeuCauLinhVatTu.DuocDuyet == false ? p.YeuCauVatTuBenhVien.KhongLinhBu != false : p.YeuCauVatTuBenhVien.KhongLinhBu != true)
                           && p.YeuCauVatTuBenhVien.KhoLinh.LaKhoKSNK == true)
               .OrderBy(x => x.YeuCauVatTuBenhVien.ThoiDiemChiDinh)
                  .Select(s => new KSNKLinhBuCuaBNGridVos
                  {
                      Id = s.Id,
                      MaTN = s.YeuCauVatTuBenhVien.YeuCauTiepNhan.MaYeuCauTiepNhan,
                      MaBN = s.YeuCauVatTuBenhVien.YeuCauTiepNhan.BenhNhan.MaBN,
                      HoTen = s.YeuCauVatTuBenhVien.YeuCauTiepNhan.HoTen,
                      SL = s.SoLuong,
                      DVKham = s.YeuCauVatTuBenhVien.YeuCauKhamBenh.TenDichVu ?? s.YeuCauVatTuBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu ?? null,
                      BSKeToa = s.YeuCauVatTuBenhVien.NhanVienChiDinh.User.HoTen,
                      NgayKe = s.YeuCauVatTuBenhVien.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                      SLDaLinh = s.SoLuongDaLinhBu != null ? s.SoLuongDaLinhBu : 0
                  });
                    var countTask = query.Skip(queryInfo.Skip)
                        .Take(queryInfo.Take).CountAsync();
                    await Task.WhenAll(countTask);

                    return new GridDataSource { TotalRowCount = countTask.Result };
                }
                
            }
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhDuTru)
            {
                var query = _yeuCauLinhVatTuChiTietRepository.TableNoTracking
                     .Where(o =>
                             o.YeuCauLinhVatTu.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhDuTru &&
                             o.YeuCauLinhVatTuId == long.Parse(queryString[0])
                            )
                    .Select(s => new DSLinhVatChildTuGridVo
                    {
                        Id = s.Id,
                        Ma = s.VatTuBenhVien.VatTus.Ma,
                        TenKSNK = s.VatTuBenhVien.VatTus.Ten,
                        DonViTinh = s.VatTuBenhVien.VatTus.DonViTinh,
                        SoLuongYc = s.SoLuong,
                        HangSanXuat = s.VatTuBenhVien.VatTus.NhaSanXuat,
                        NuocSanXuat = s.VatTuBenhVien.VatTus.NuocSanXuat,
                        BSKeToa = s.YeuCauVatTuBenhVien.NhanVienChiDinh.User.HoTen,
                        Nhom = s.LaVatTuBHYT == true ? "BHYT" : "Không BHYT"
                    });
                var countTask = query.CountAsync();
                await Task.WhenAll(countTask);

                return new GridDataSource { TotalRowCount = countTask.Result };
            }
            return null;
        }
        #endregion

        private OBJListKSNK GetHTMLLinhBenhNhanTuChoi(List<KSNKGridVo> gridvos)
        {
            string sluongDaXuat = "";
            string ghiChu = "";
            string KSNK = "";
            int index = 1;
            foreach (var itemx in gridvos)
            {
                if (itemx.SoLuongCoTheXuat == null)
                {
                    sluongDaXuat = "";
                }
                else
                {
                    sluongDaXuat = Convert.ToString(itemx.SoLuongCoTheXuat);
                }
                KSNK = KSNK +
                                         "<tr style='border: 1px solid #020000;'>" +

                                        "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                        index++
                                        + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                       itemx.MaKSNK
                                        + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                        itemx.TenKSNK
                                        + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                       itemx.DonViTinh
                                        + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                        itemx.SoLuong
                                         + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                        sluongDaXuat
                                        + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                        "&nbsp;";
                //tenLoaiLinh = itemx.LoaiPhieuLinh.GetDescription();
                sluongDaXuat = "";
            }
            var data = new OBJListKSNK
            {
                Index = index,
                html = KSNK
            };
            return data;
        }
        private OBJListKSNK GetHTMLLinhBenhNhanDataoDaDuyet(List<YeuCauVatTuBenhVien> gridvos)
        {
            var yeucau = 0; // to do
            var thucChat = ""; // to do
            var tenLoaiLinh = "";
            var donViTinh = "";
            var maHoatChat = "";
            var vt = "";
            int index = 1;
            var infoKSNKs = gridvos.GroupBy(d => new { d.VatTuBenhVienId, d.DonViTinh })
                .Select(d => new {
                    Ma = d.First().Ma,
                    TenKSNK = d.First().Ten,
                    DonViTinh = d.First().DonViTinh,
                    SoLuong = d.Sum(f => f.SoLuong),
                    TenLoaiLinh = d.First().LoaiPhieuLinh.GetDescription(),
                }).OrderBy(d => d.TenKSNK).ToList();
            foreach (var itemx in infoKSNKs.ToList())
            {
                vt = vt +
                                          "<tr style='border: 1px solid #020000;'>" +
                                         "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                        index++
                                        + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                        itemx.Ma
                                        + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                        itemx.TenKSNK
                                        + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                       itemx.DonViTinh
                                        + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                        itemx.SoLuong
                                         + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                        thucChat
                                        + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                        "&nbsp;";
                tenLoaiLinh = itemx.TenLoaiLinh;
                donViTinh = "";
            }
            var data = new OBJListKSNK
            {
                Index = index,
                html = vt
            };
            return data;
        }
        private OBJListKSNK GetHTMLDuyet(List<KSNKDaTaoGridVo> gridVos)
        {
            var yeuCau = "";
            var thucChat = "";
            var tenLoaiLinh = "";
            var donViTinh = "";
            var maHoatChat = "";
            var vt = "";
            int index = 1;

            foreach (var itemx in gridVos)
            {
                if (itemx.DuocDuyet == true)
                {
                    yeuCau = itemx.YeuCau.ToString();
                }
                else
                {
                    yeuCau = "";
                }
                vt = vt + "<tr style='border: 1px solid #020000;'>"

                                        + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                        index++
                                        + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                       itemx.MaKSNK
                                        + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                        itemx.TenKSNK
                                        + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                       itemx.DonViTinh
                                        + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                        itemx.SoLuong
                                         + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                        yeuCau
                                        + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                        "&nbsp;";
            }
            var data = new OBJListKSNK
            {
                Index = index,
                html = vt
            };
            return data;
        }
        private List<DSLinhVatChildTuGridVo> DataChoGoi(long idKhoLinh)
        {
            long khoaId = 0;
            var phongBenhVien = _phongBenhVienRepository.Table.FirstOrDefault(o => o.Id == _userAgentHelper.GetCurrentNoiLLamViecId());
            if (phongBenhVien != null)
            {
                khoaId = phongBenhVien.KhoaPhongId;
            }

            //Lấy ds phòng mà nhân viên login thuộc (bao gồm các phòng mà nhân viên chỉ thuộc khoa)
            var phongLinhVes = _phongBenhVienRepository.Table.Where(o => o.KhoaPhongId == khoaId && o.IsDisabled != true).Select(o => new LookupItemVo
            {
                KeyId = (long)o.Id,
                DisplayName = o.Ten
            }).OrderBy(o => o.DisplayName).ToList();

            var yeuCauKSNK = _yeuCauVatTuBenhVienRepository.TableNoTracking
                                     .Where(x => x.KhoLinhId == idKhoLinh &&
                                      x.YeuCauLinhVatTuId == null &&
                                      x.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 &&
                                      phongLinhVes.Any(o => o.KeyId == x.NoiChiDinhId) &&
                                      x.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy)
                        .OrderBy(x => x.ThoiDiemChiDinh)
                      .Select(s => new DSLinhVatChildTuGridVo
                      {
                          BenhNhanId = s.YeuCauTiepNhan.BenhNhanId.Value,
                          MaYeuCauTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                          MaBenhNhan = s.YeuCauTiepNhan.BenhNhan.MaBN,
                          HoTen = s.YeuCauTiepNhan.HoTen,
                          SoLuong = s.SoLuong,
                          DichVuKham = s.YeuCauKhamBenh != null ? s.YeuCauKhamBenh.TenDichVu : (s.YeuCauDichVuKyThuat.YeuCauKhamBenh != null ? s.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu : ""),
                          BacSiKeToa = s.NhanVienChiDinh.User.HoTen,
                          NgayKe = s.ThoiDiemChiDinh,
                          DuocDuyet = s.YeuCauLinhVatTu.DuocDuyet,
                          YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                          NgayDieuTri = (s.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && s.NoiTruPhieuDieuTriId != null) ? s.NoiTruPhieuDieuTri.NgayDieuTri : s.ThoiDiemChiDinh,
                          KhoLinhId = s.KhoLinhId
                      }).GroupBy(x => new
                      {
                          x.BenhNhanId,
                          x.MaYeuCauTiepNhan,
                          x.HoTen
                      }).Select(s => new DSLinhVatChildTuGridVo
                      {
                          BenhNhanId = s.First().BenhNhanId,
                          MaYeuCauTiepNhan = s.First().MaYeuCauTiepNhan,
                          MaBenhNhan = s.First().MaBenhNhan,
                          HoTen = s.First().HoTen,
                          SoLuong = s.Sum(a => a.SoLuong),
                          DichVuKham = s.First().DichVuKham,
                          BacSiKeToa = s.First().BacSiKeToa,
                          NgayKe = s.First().NgayKe,
                          LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhChoBenhNhan,
                          //Id = yeuCauLinhId,
                          DuocDuyet = s.First().DuocDuyet,
                          YeuCauTiepNhanId = s.First().YeuCauTiepNhanId,
                          NgayDieuTri = s.First().NgayDieuTri,
                          KhoLinhId = s.First().KhoLinhId
                      });
            return yeuCauKSNK.ToList();
        }
        private double GetSoLuongTonKSNK(long VatTuBenhVienId, long khoXuatId, bool LaVatTuBHYT)
        {
            return _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(o => o.VatTuBenhVienId == VatTuBenhVienId && o.LaVatTuBHYT == LaVatTuBHYT && o.NhapKhoVatTu.KhoId == khoXuatId).Sum(o => o.SoLuongNhap - o.SoLuongDaXuat).MathRoundNumber();
        }
        public string TenNoiNhanPhieuLinhTrucTiep(long noiYeuCauId)
        {
            long khoaId = 0;
            var phongBenhVien = _phongBenhVienRepository.Table.FirstOrDefault(o => o.Id == noiYeuCauId);
            if (phongBenhVien != null)
            {
                khoaId = phongBenhVien.KhoaPhongId;
            }
            var tenKhoa = _khoaPhongRepository.TableNoTracking.Where(s => s.Id == khoaId).Select(c => c.Ten).FirstOrDefault();
            return tenKhoa;
        }



        #region  dược phẩm 


        private List<DSLinhKSNKGridVo> DangChoGoi(QueryInfo queryInfo, bool manHinhDuyet = false)
        {
            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = _phongBenhVienRepository.GetById(noiLamViecCurrentId);
            var queryString = JsonConvert.DeserializeObject<SeachNgay>(queryInfo.AdditionalSearchString);
            var queryDangChoDuyet = BaseRepository.TableNoTracking
                .Where(x => x.KhoNhap.LaKhoKSNK == true && x.DuocDuyet == null && x.DaGui != true && ((manHinhDuyet == true && x.KhoXuat.KhoaPhongId == phongBenhVien.KhoaPhongId) || (manHinhDuyet == false && x.NoiYeuCau != null && phongBenhVien != null && x.NoiYeuCau.KhoaPhongId == phongBenhVien.KhoaPhongId))).Select(s => new DSLinhKSNKGridVo()
            {
                Id = s.Id,
                MaPL = s.SoPhieu,
                Loai = s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan ? s.NoiYeuCau.KhoaPhong.Ten + " " + (s.LoaiPhieuLinh.GetDescription()).ToLower() : s.LoaiPhieuLinh.GetDescription(),
                LoaiPhieuLinh = s.LoaiPhieuLinh,
                LinhTuKhoId = s.KhoNhapId,
                LinhVeKhoId = s.KhoXuatId,
                NguoiYeuCau = s.NhanVienYeuCau.User.HoTen,
                LinhTuKho = s.KhoXuat.Ten,
                LinhVeKho = s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan ? "" : s.KhoNhap.Ten,
                NgayYeuCau = s.NgayYeuCau,
                TinhTrang = "Đang chờ gởi",
                Nguoiduyet = s.NhanVienDuyet.User.HoTen,
                NgayDuyet = s.NgayDuyet != null ? s.NgayDuyet : (DateTime?)null,
                NgayDuyetHienThi = s.NgayDuyet != null ? Convert.ToDateTime(s.NgayDuyet).ApplyFormatDateTimeSACH() : "",
                DuocDuyet = s.DuocDuyet,
                DaGui = s.DaGui
            });
            if (queryString.Searching != null)
            {
                var searchTamp = queryString.Searching.Trim();
                queryDangChoDuyet = queryDangChoDuyet.ApplyLike(searchTamp, g => g.NguoiYeuCau, g => g.Nguoiduyet, g => g.LinhTuKho, g => g.LinhVeKho, g => g.MaPL);
            }


            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryStrings = JsonConvert.DeserializeObject<SeachNgay>(queryInfo.AdditionalSearchString);
                if (queryStrings.NgayDuyetRangDateStartDate != null && queryStrings.NgayDuyetRangDateStartEnd != null)
                {
                    DateTime TuNgayDuyetPart = DateTime.Now;
                    DateTime DenNgayDuyetsPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayDuyetRangDateStartDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayDuyetPart);
                    DateTime.TryParseExact(queryStrings.NgayDuyetRangDateStartEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgayDuyetsPart);
                    var tuNgay = new DateTime(TuNgayDuyetPart.Year, TuNgayDuyetPart.Month, TuNgayDuyetPart.Day, 0, 0, 0);
                    var denNgay = new DateTime(DenNgayDuyetsPart.Year, DenNgayDuyetsPart.Month, DenNgayDuyetsPart.Day, 23, 59, 59);
                    queryDangChoDuyet = queryDangChoDuyet.Where(p => p.NgayDuyet >= tuNgay && p.NgayDuyet <= denNgay);
                }
                if (queryStrings.NgayDuyetRangDateStartDate != null && queryStrings.NgayDuyetRangDateStartEnd == null)
                {
                    DateTime TuNgayDuyetPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayDuyetRangDateStartDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayDuyetPart);
                    var tuNgay = new DateTime(TuNgayDuyetPart.Year, TuNgayDuyetPart.Month, TuNgayDuyetPart.Day, 0, 0, 0);
                    queryDangChoDuyet = queryDangChoDuyet.Where(p => p.NgayDuyet >= tuNgay);
                }
                if (queryStrings.NgayDuyetRangDateStartDate == null && queryStrings.NgayDuyetRangDateStartEnd != null)
                {
                    DateTime DenNgayDuyetsPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayDuyetRangDateStartEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgayDuyetsPart);
                    var denNgay = new DateTime(DenNgayDuyetsPart.Year, DenNgayDuyetsPart.Month, DenNgayDuyetsPart.Day, 23, 59, 59);
                    queryDangChoDuyet = queryDangChoDuyet.Where(p => p.NgayDuyet <= denNgay);
                }
                if (queryStrings.NgayYeuCauRangDateStartDate != null && queryStrings.NgayYeuCauRangDateStartEnd != null)
                {
                    DateTime TuNgayYCPart = DateTime.Now;
                    DateTime DenNgayYCsPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayYeuCauRangDateStartDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayYCPart);
                    DateTime.TryParseExact(queryStrings.NgayYeuCauRangDateStartEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgayYCsPart);
                    var tuNgay = new DateTime(TuNgayYCPart.Year, TuNgayYCPart.Month, TuNgayYCPart.Day, 0, 0, 0);
                    var denNgay = new DateTime(DenNgayYCsPart.Year, DenNgayYCsPart.Month, DenNgayYCsPart.Day, 23, 59, 59);
                    queryDangChoDuyet = queryDangChoDuyet.Where(p => p.NgayYeuCau >= tuNgay && p.NgayYeuCau <= denNgay);
                }
                if (queryStrings.NgayYeuCauRangDateStartDate != null && queryStrings.NgayYeuCauRangDateStartEnd == null)
                {
                    DateTime TuNgayYCPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayYeuCauRangDateStartDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayYCPart);
                    var tuNgay = new DateTime(TuNgayYCPart.Year, TuNgayYCPart.Month, TuNgayYCPart.Day, 0, 0, 0);
                    queryDangChoDuyet = queryDangChoDuyet.Where(p => p.NgayYeuCau >= tuNgay);
                }
                if (queryStrings.NgayYeuCauRangDateStartDate == null && queryStrings.NgayYeuCauRangDateStartEnd != null)
                {
                    DateTime DenNgayYCsPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayYeuCauRangDateStartEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgayYCsPart);
                    var denNgay = new DateTime(DenNgayYCsPart.Year, DenNgayYCsPart.Month, DenNgayYCsPart.Day, 23, 59, 59);
                    queryDangChoDuyet = queryDangChoDuyet.Where(p => p.NgayYeuCau <= denNgay);
                }
                if (queryStrings.NgayYeuCauRangDateStartDate == null && queryStrings.NgayYeuCauRangDateStartEnd == null)
                {
                    DateTime TuNgayYCPart = DateTime.Now;
                    DateTime DenNgayYCsPart = DateTime.Now;
                    var tuNgay = new DateTime(TuNgayYCPart.Year, TuNgayYCPart.Month, TuNgayYCPart.Day, 0, 0, 0);
                    var denNgay = new DateTime(DenNgayYCsPart.Year, DenNgayYCsPart.Month, DenNgayYCsPart.Day, 23, 59, 59);
                    queryDangChoDuyet = queryDangChoDuyet.Where(p => p.NgayYeuCau >= tuNgay && p.NgayYeuCau <= denNgay);
                }
            }
            return queryDangChoDuyet.OrderByDescending(d => d.NgayYeuCau).ToList();
        }
        private List<DSLinhKSNKGridVo> DangChoDuyet(QueryInfo queryInfo, bool manHinhDuyet = false)
        {
            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = _phongBenhVienRepository.GetById(noiLamViecCurrentId);
            var queryString = JsonConvert.DeserializeObject<SeachNgay>(queryInfo.AdditionalSearchString);
            var queryDangChoDuyet = BaseRepository.TableNoTracking.Where(x => x.KhoNhap.LaKhoKSNK == true && x.DuocDuyet == null && x.DaGui == true && ((manHinhDuyet == true && x.KhoXuat.KhoaPhongId == phongBenhVien.KhoaPhongId) || (manHinhDuyet == false && x.NoiYeuCau != null && phongBenhVien != null && x.NoiYeuCau.KhoaPhongId == phongBenhVien.KhoaPhongId))).Select(s => new DSLinhKSNKGridVo()
            {
                Id = s.Id,
                MaPL = s.SoPhieu,
                LoaiPhieuLinh = s.LoaiPhieuLinh,
                Loai = s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan ? s.NoiYeuCau.KhoaPhong.Ten + " " + (s.LoaiPhieuLinh.GetDescription()).ToLower() : s.LoaiPhieuLinh.GetDescription(),
                LinhTuKhoId = s.KhoNhapId,
                LinhVeKhoId = s.KhoXuatId,
                NguoiYeuCau = s.NhanVienYeuCau.User.HoTen,
                LinhTuKho = s.KhoXuat.Ten,
                LinhVeKho = s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan ? "" : s.KhoNhap.Ten,
                NgayYeuCau = s.NgayYeuCau,
                TinhTrang = "Đang chờ duyệt",
                Nguoiduyet = s.NhanVienDuyet.User.HoTen,
                NgayDuyet = s.NgayDuyet != null ? s.NgayDuyet : (DateTime?)null,
                NgayDuyetHienThi = s.NgayDuyet != null ? Convert.ToDateTime(s.NgayDuyet).ApplyFormatDateTimeSACH() : "",
                DuocDuyet = s.DuocDuyet,
                DaGui = s.DaGui
            });
            if (queryString.Searching != null)
            {
                var searchTamp = queryString.Searching.Trim();
                queryDangChoDuyet = queryDangChoDuyet.ApplyLike(searchTamp, g => g.NguoiYeuCau, g => g.Nguoiduyet, g => g.LinhTuKho, g => g.LinhVeKho, g => g.MaPL);
            }


            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryStrings = JsonConvert.DeserializeObject<SeachNgay>(queryInfo.AdditionalSearchString);
                if (queryStrings.NgayDuyetRangDateStartDate != null && queryStrings.NgayDuyetRangDateStartEnd != null)
                {
                    DateTime TuNgayDuyetPart = DateTime.Now;
                    DateTime DenNgayDuyetsPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayDuyetRangDateStartDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayDuyetPart);
                    DateTime.TryParseExact(queryStrings.NgayDuyetRangDateStartEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgayDuyetsPart);
                    var tuNgay = new DateTime(TuNgayDuyetPart.Year, TuNgayDuyetPart.Month, TuNgayDuyetPart.Day, 0, 0, 0);
                    var denNgay = new DateTime(DenNgayDuyetsPart.Year, DenNgayDuyetsPart.Month, DenNgayDuyetsPart.Day, 23, 59, 59);
                    queryDangChoDuyet = queryDangChoDuyet.Where(p => p.NgayDuyet >= tuNgay && p.NgayDuyet <= denNgay);
                }
                if (queryStrings.NgayDuyetRangDateStartDate != null && queryStrings.NgayDuyetRangDateStartEnd == null)
                {
                    DateTime TuNgayDuyetPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayDuyetRangDateStartDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayDuyetPart);
                    var tuNgay = new DateTime(TuNgayDuyetPart.Year, TuNgayDuyetPart.Month, TuNgayDuyetPart.Day, 0, 0, 0);
                    queryDangChoDuyet = queryDangChoDuyet.Where(p => p.NgayDuyet >= tuNgay);
                }
                if (queryStrings.NgayDuyetRangDateStartDate == null && queryStrings.NgayDuyetRangDateStartEnd != null)
                {
                    DateTime DenNgayDuyetsPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayDuyetRangDateStartEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgayDuyetsPart);
                    var denNgay = new DateTime(DenNgayDuyetsPart.Year, DenNgayDuyetsPart.Month, DenNgayDuyetsPart.Day, 23, 59, 59);
                    queryDangChoDuyet = queryDangChoDuyet.Where(p => p.NgayDuyet <= denNgay);
                }
                if (queryStrings.NgayYeuCauRangDateStartDate != null && queryStrings.NgayYeuCauRangDateStartEnd != null)
                {
                    DateTime TuNgayYCPart = DateTime.Now;
                    DateTime DenNgayYCsPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayYeuCauRangDateStartDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayYCPart);
                    DateTime.TryParseExact(queryStrings.NgayYeuCauRangDateStartEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgayYCsPart);
                    var tuNgay = new DateTime(TuNgayYCPart.Year, TuNgayYCPart.Month, TuNgayYCPart.Day, 0, 0, 0);
                    var denNgay = new DateTime(DenNgayYCsPart.Year, DenNgayYCsPart.Month, DenNgayYCsPart.Day, 23, 59, 59);
                    queryDangChoDuyet = queryDangChoDuyet.Where(p => p.NgayYeuCau >= tuNgay && p.NgayYeuCau <= denNgay);
                }
                if (queryStrings.NgayYeuCauRangDateStartDate != null && queryStrings.NgayYeuCauRangDateStartEnd == null)
                {
                    DateTime TuNgayYCPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayYeuCauRangDateStartDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayYCPart);
                    var tuNgay = new DateTime(TuNgayYCPart.Year, TuNgayYCPart.Month, TuNgayYCPart.Day, 0, 0, 0);
                    queryDangChoDuyet = queryDangChoDuyet.Where(p => p.NgayYeuCau >= tuNgay);
                }
            }
            return queryDangChoDuyet.OrderByDescending(d => d.NgayYeuCau).ToList();
        }
        private List<DSLinhKSNKGridVo> TuChoiDuyet(QueryInfo queryInfo, bool manHinhDuyet = false)
        {
            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = _phongBenhVienRepository.GetById(noiLamViecCurrentId);
            var queryString = JsonConvert.DeserializeObject<SeachNgay>(queryInfo.AdditionalSearchString);
            var TCD = BaseRepository.TableNoTracking.Where(x => x.KhoNhap.LaKhoKSNK == true &&  x.DuocDuyet == false && x.DaGui == true && ((manHinhDuyet == true && x.KhoXuat.KhoaPhongId == phongBenhVien.KhoaPhongId) || (manHinhDuyet == false && x.NoiYeuCau != null && phongBenhVien != null && x.NoiYeuCau.KhoaPhongId == phongBenhVien.KhoaPhongId))).Select(s => new DSLinhKSNKGridVo()
            {
                Id = s.Id,
                LinhTuKhoId = s.KhoNhapId,
                LinhVeKhoId = s.KhoXuatId,
                MaPL = s.SoPhieu,
                LoaiPhieuLinh = s.LoaiPhieuLinh,
                Loai = s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan ? s.NoiYeuCau.KhoaPhong.Ten + " " + (s.LoaiPhieuLinh.GetDescription()).ToLower() : s.LoaiPhieuLinh.GetDescription(),
                NguoiYeuCau = s.NhanVienYeuCau.User.HoTen,
                LinhTuKho = s.KhoXuat.Ten,
                LinhVeKho = s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan ? "" : s.KhoNhap.Ten,
                NgayYeuCau = s.NgayYeuCau,
                TinhTrang = "Từ chối duyệt",
                Nguoiduyet = s.NhanVienDuyet.User.HoTen,
                NgayDuyet = s.NgayDuyet != null ? s.NgayDuyet : (DateTime?)null,
                NgayDuyetHienThi = s.NgayDuyet != null ? Convert.ToDateTime(s.NgayDuyet).ApplyFormatDateTimeSACH() : "",
                DuocDuyet = s.DuocDuyet,
                DaGui = s.DaGui
            });
            if (queryString.Searching != null)
            {
                var searchTamp = queryString.Searching.Trim();
                TCD = TCD.ApplyLike(searchTamp, g => g.NguoiYeuCau, g => g.Nguoiduyet, g => g.LinhTuKho, g => g.LinhVeKho, g => g.MaPL);
            }

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryStrings = JsonConvert.DeserializeObject<SeachNgay>(queryInfo.AdditionalSearchString);
                if (queryStrings.NgayDuyetRangDateStartDate != null && queryStrings.NgayDuyetRangDateStartEnd != null)
                {
                    DateTime TuNgayDuyetPart = DateTime.Now;
                    DateTime DenNgayDuyetsPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayDuyetRangDateStartDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayDuyetPart);
                    DateTime.TryParseExact(queryStrings.NgayDuyetRangDateStartEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgayDuyetsPart);
                    var tuNgay = new DateTime(TuNgayDuyetPart.Year, TuNgayDuyetPart.Month, TuNgayDuyetPart.Day, 0, 0, 0);
                    var denNgay = new DateTime(DenNgayDuyetsPart.Year, DenNgayDuyetsPart.Month, DenNgayDuyetsPart.Day, 23, 59, 59);
                    TCD = TCD.Where(p => p.NgayDuyet >= tuNgay && p.NgayDuyet <= denNgay);
                }
                if (queryStrings.NgayDuyetRangDateStartDate != null && queryStrings.NgayDuyetRangDateStartEnd == null)
                {
                    DateTime TuNgayDuyetPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayDuyetRangDateStartDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayDuyetPart);
                    var tuNgay = new DateTime(TuNgayDuyetPart.Year, TuNgayDuyetPart.Month, TuNgayDuyetPart.Day, 0, 0, 0);
                    TCD = TCD.Where(p => p.NgayDuyet >= tuNgay);
                }
                if (queryStrings.NgayDuyetRangDateStartDate == null && queryStrings.NgayDuyetRangDateStartEnd != null)
                {
                    DateTime DenNgayDuyetsPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayDuyetRangDateStartEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgayDuyetsPart);
                    var denNgay = new DateTime(DenNgayDuyetsPart.Year, DenNgayDuyetsPart.Month, DenNgayDuyetsPart.Day, 23, 59, 59);
                    TCD = TCD.Where(p => p.NgayDuyet <= denNgay);
                }
                if (queryStrings.NgayYeuCauRangDateStartDate != null && queryStrings.NgayYeuCauRangDateStartEnd != null)
                {
                    DateTime TuNgayYCPart = DateTime.Now;
                    DateTime DenNgayYCsPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayYeuCauRangDateStartDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayYCPart);
                    DateTime.TryParseExact(queryStrings.NgayYeuCauRangDateStartEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgayYCsPart);
                    var tuNgay = new DateTime(TuNgayYCPart.Year, TuNgayYCPart.Month, TuNgayYCPart.Day, 0, 0, 0);
                    var denNgay = new DateTime(DenNgayYCsPart.Year, DenNgayYCsPart.Month, DenNgayYCsPart.Day, 23, 59, 59);
                    TCD = TCD.Where(p => p.NgayYeuCau >= tuNgay && p.NgayYeuCau <= denNgay);
                }
                if (queryStrings.NgayYeuCauRangDateStartDate != null && queryStrings.NgayYeuCauRangDateStartEnd == null)
                {
                    DateTime TuNgayYCPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayYeuCauRangDateStartDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayYCPart);
                    var tuNgay = new DateTime(TuNgayYCPart.Year, TuNgayYCPart.Month, TuNgayYCPart.Day, 0, 0, 0);
                    TCD = TCD.Where(p => p.NgayYeuCau >= tuNgay);
                }
                if (queryStrings.NgayYeuCauRangDateStartDate == null && queryStrings.NgayYeuCauRangDateStartEnd != null)
                {
                    DateTime DenNgayYCsPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayYeuCauRangDateStartEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgayYCsPart);
                    var denNgay = new DateTime(DenNgayYCsPart.Year, DenNgayYCsPart.Month, DenNgayYCsPart.Day, 23, 59, 59);
                    TCD = TCD.Where(p => p.NgayYeuCau <= denNgay);
                }
               
            }
            return TCD.OrderByDescending(d => d.NgayYeuCau).ToList();
        }
        private List<DSLinhKSNKGridVo> DaDuyet(QueryInfo queryInfo, bool manHinhDuyet = false)
        {
            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = _phongBenhVienRepository.GetById(noiLamViecCurrentId);
            var queryString = JsonConvert.DeserializeObject<SeachNgay>(queryInfo.AdditionalSearchString);
            var queryDaDuyet = BaseRepository.TableNoTracking.Where(x => x.KhoNhap.LaKhoKSNK == true && x.DuocDuyet == true && x.DaGui == true && ((manHinhDuyet == true && x.KhoXuat.KhoaPhongId == phongBenhVien.KhoaPhongId) || (manHinhDuyet == false && x.NoiYeuCau != null && phongBenhVien != null && x.NoiYeuCau.KhoaPhongId == phongBenhVien.KhoaPhongId))).Select(s => new DSLinhKSNKGridVo()
            {
                Id = s.Id,
                LinhTuKhoId = s.KhoNhapId,
                LinhVeKhoId = s.KhoXuatId,
                MaPL = s.SoPhieu,
                LoaiPhieuLinh = s.LoaiPhieuLinh,
                Loai = s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan ? s.NoiYeuCau.KhoaPhong.Ten + " " + (s.LoaiPhieuLinh.GetDescription()).ToLower() : s.LoaiPhieuLinh.GetDescription(),
                NguoiYeuCau = s.NhanVienYeuCau.User.HoTen,
                LinhTuKho = s.KhoXuat.Ten,
                LinhVeKho = s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan ? "" : s.KhoNhap.Ten,
                NgayYeuCau = s.NgayYeuCau,
                TinhTrang = "Đã duyệt",
                Nguoiduyet = s.NhanVienDuyet.User.HoTen,
                NgayDuyet = s.NgayDuyet != null ? s.NgayDuyet : (DateTime?)null,
                NgayDuyetHienThi = s.NgayDuyet != null ? Convert.ToDateTime(s.NgayDuyet).ApplyFormatDateTimeSACH() : "",
                DuocDuyet = s.DuocDuyet,
                DaGui = s.DaGui
            });
            if (queryString.Searching != null)
            {
                var searchTamp = queryString.Searching.Trim();
                queryDaDuyet = queryDaDuyet.ApplyLike(searchTamp, g => g.NguoiYeuCau, g => g.Nguoiduyet, g => g.LinhTuKho, g => g.LinhVeKho, g => g.MaPL);
            }
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryStrings = JsonConvert.DeserializeObject<SeachNgay>(queryInfo.AdditionalSearchString);
                if (queryStrings.NgayDuyetRangDateStartDate != null && queryStrings.NgayDuyetRangDateStartEnd != null)
                {
                    DateTime TuNgayDuyetPart = DateTime.Now;
                    DateTime DenNgayDuyetsPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayDuyetRangDateStartDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayDuyetPart);
                    DateTime.TryParseExact(queryStrings.NgayDuyetRangDateStartEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgayDuyetsPart);
                    var tuNgay = new DateTime(TuNgayDuyetPart.Year, TuNgayDuyetPart.Month, TuNgayDuyetPart.Day, 0, 0, 0);
                    var denNgay = new DateTime(DenNgayDuyetsPart.Year, DenNgayDuyetsPart.Month, DenNgayDuyetsPart.Day, 23, 59, 59);
                    queryDaDuyet = queryDaDuyet.Where(p => p.NgayDuyet >= tuNgay && p.NgayDuyet <= denNgay);
                }
                if (queryStrings.NgayDuyetRangDateStartDate != null && queryStrings.NgayDuyetRangDateStartEnd == null)
                {
                    DateTime TuNgayDuyetPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayDuyetRangDateStartDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayDuyetPart);
                    var tuNgay = new DateTime(TuNgayDuyetPart.Year, TuNgayDuyetPart.Month, TuNgayDuyetPart.Day, 0, 0, 0);
                    queryDaDuyet = queryDaDuyet.Where(p => p.NgayDuyet >= tuNgay);
                }
                if (queryStrings.NgayDuyetRangDateStartDate == null && queryStrings.NgayDuyetRangDateStartEnd != null)
                {
                    DateTime DenNgayDuyetsPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayDuyetRangDateStartEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgayDuyetsPart);
                    var denNgay = new DateTime(DenNgayDuyetsPart.Year, DenNgayDuyetsPart.Month, DenNgayDuyetsPart.Day, 23, 59, 59);
                    queryDaDuyet = queryDaDuyet.Where(p => p.NgayDuyet <= denNgay);
                }
                if (queryStrings.NgayYeuCauRangDateStartDate != null && queryStrings.NgayYeuCauRangDateStartEnd != null)
                {
                    DateTime TuNgayYCPart = DateTime.Now;
                    DateTime DenNgayYCsPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayYeuCauRangDateStartDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayYCPart);
                    DateTime.TryParseExact(queryStrings.NgayYeuCauRangDateStartEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgayYCsPart);
                    var tuNgay = new DateTime(TuNgayYCPart.Year, TuNgayYCPart.Month, TuNgayYCPart.Day, 0, 0, 0);
                    var denNgay = new DateTime(DenNgayYCsPart.Year, DenNgayYCsPart.Month, DenNgayYCsPart.Day, 23, 59, 59);
                    queryDaDuyet = queryDaDuyet.Where(p => p.NgayYeuCau >= tuNgay && p.NgayYeuCau <= denNgay);
                }
                if (queryStrings.NgayYeuCauRangDateStartDate != null && queryStrings.NgayYeuCauRangDateStartEnd == null)
                {
                    DateTime TuNgayYCPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayYeuCauRangDateStartDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayYCPart);
                    var tuNgay = new DateTime(TuNgayYCPart.Year, TuNgayYCPart.Month, TuNgayYCPart.Day, 0, 0, 0);
                    queryDaDuyet = queryDaDuyet.Where(p => p.NgayYeuCau >= tuNgay);
                }
                if (queryStrings.NgayYeuCauRangDateStartDate == null && queryStrings.NgayYeuCauRangDateStartEnd != null)
                {
                    DateTime DenNgayYCsPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayYeuCauRangDateStartEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgayYCsPart);
                    var denNgay = new DateTime(DenNgayYCsPart.Year, DenNgayYCsPart.Month, DenNgayYCsPart.Day, 23, 59, 59);
                    queryDaDuyet = queryDaDuyet.Where(p => p.NgayYeuCau <= denNgay);
                }
            }
            return queryDaDuyet.OrderByDescending(d => d.NgayYeuCau).ToList();
        }





        private List<DSLinhKSNKGridVo> DangChoGoiDP(QueryInfo queryInfo, bool manHinhDuyet = false)
        {
            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = _phongBenhVienRepository.GetById(noiLamViecCurrentId);
            var queryString = JsonConvert.DeserializeObject<SeachNgay>(queryInfo.AdditionalSearchString);
            var queryDangChoDuyet = _yeuCauLinhDuocPhamRepository.TableNoTracking.Where(x => x.KhoNhap.LaKhoKSNK == true &&  x.DuocDuyet == null && x.DaGui != true && ((manHinhDuyet == true && x.KhoXuat.KhoaPhongId == phongBenhVien.KhoaPhongId) || (manHinhDuyet == false && x.NoiYeuCau != null && phongBenhVien != null && x.NoiYeuCau.KhoaPhongId == phongBenhVien.KhoaPhongId))).Select(s => new DSLinhKSNKGridVo()
            {
                Id = s.Id,
                MaPL = s.SoPhieu,
                Loai = s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan ? s.NoiYeuCau.KhoaPhong.Ten + " " + (s.LoaiPhieuLinh.GetDescription()).ToLower() : s.LoaiPhieuLinh.GetDescription(),
                LoaiPhieuLinh = s.LoaiPhieuLinh,
                LinhTuKhoId = s.KhoNhapId,
                LinhVeKhoId = s.KhoXuatId,
                NguoiYeuCau = s.NhanVienYeuCau.User.HoTen,
                LinhTuKho = s.KhoXuat.Ten,
                LinhVeKho = s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan ? "" : s.KhoNhap.Ten,
                NgayYeuCau = s.NgayYeuCau,
                TinhTrang = "Đang chờ gởi",
                Nguoiduyet = s.NhanVienDuyet.User.HoTen,
                NgayDuyet = s.NgayDuyet != null ? s.NgayDuyet : (DateTime?)null,
                NgayDuyetHienThi = s.NgayDuyet != null ? Convert.ToDateTime(s.NgayDuyet).ApplyFormatDateTimeSACH() : "",
                DuocDuyet = s.DuocDuyet,
                DaGui = s.DaGui,
                LoaiDuocPhamHayVatTu = true
            });
            if (queryString.Searching != null)
            {
                var searchTamp = queryString.Searching.Trim();
                queryDangChoDuyet = queryDangChoDuyet.ApplyLike(searchTamp, g => g.NguoiYeuCau, g => g.Nguoiduyet, g => g.LinhTuKho, g => g.LinhVeKho, g => g.MaPL);
            }


            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryStrings = JsonConvert.DeserializeObject<SeachNgay>(queryInfo.AdditionalSearchString);
                if (queryStrings.NgayDuyetRangDateStartDate != null && queryStrings.NgayDuyetRangDateStartEnd != null)
                {
                    DateTime TuNgayDuyetPart = DateTime.Now;
                    DateTime DenNgayDuyetsPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayDuyetRangDateStartDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayDuyetPart);
                    DateTime.TryParseExact(queryStrings.NgayDuyetRangDateStartEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgayDuyetsPart);
                    var tuNgay = new DateTime(TuNgayDuyetPart.Year, TuNgayDuyetPart.Month, TuNgayDuyetPart.Day, 0, 0, 0);
                    var denNgay = new DateTime(DenNgayDuyetsPart.Year, DenNgayDuyetsPart.Month, DenNgayDuyetsPart.Day, 23, 59, 59);
                    queryDangChoDuyet = queryDangChoDuyet.Where(p => p.NgayDuyet >= tuNgay && p.NgayDuyet <= denNgay);
                }
                if (queryStrings.NgayDuyetRangDateStartDate != null && queryStrings.NgayDuyetRangDateStartEnd == null)
                {
                    DateTime TuNgayDuyetPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayDuyetRangDateStartDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayDuyetPart);
                    var tuNgay = new DateTime(TuNgayDuyetPart.Year, TuNgayDuyetPart.Month, TuNgayDuyetPart.Day, 0, 0, 0);
                    queryDangChoDuyet = queryDangChoDuyet.Where(p => p.NgayDuyet >= tuNgay);
                }
                if (queryStrings.NgayDuyetRangDateStartDate == null && queryStrings.NgayDuyetRangDateStartEnd != null)
                {
                    DateTime DenNgayDuyetsPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayDuyetRangDateStartEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgayDuyetsPart);
                    var denNgay = new DateTime(DenNgayDuyetsPart.Year, DenNgayDuyetsPart.Month, DenNgayDuyetsPart.Day, 23, 59, 59);
                    queryDangChoDuyet = queryDangChoDuyet.Where(p => p.NgayDuyet <= denNgay);
                }
                if (queryStrings.NgayYeuCauRangDateStartDate != null && queryStrings.NgayYeuCauRangDateStartEnd != null)
                {
                    DateTime TuNgayYCPart = DateTime.Now;
                    DateTime DenNgayYCsPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayYeuCauRangDateStartDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayYCPart);
                    DateTime.TryParseExact(queryStrings.NgayYeuCauRangDateStartEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgayYCsPart);
                    var tuNgay = new DateTime(TuNgayYCPart.Year, TuNgayYCPart.Month, TuNgayYCPart.Day, 0, 0, 0);
                    var denNgay = new DateTime(DenNgayYCsPart.Year, DenNgayYCsPart.Month, DenNgayYCsPart.Day, 23, 59, 59);
                    queryDangChoDuyet = queryDangChoDuyet.Where(p => p.NgayYeuCau >= tuNgay && p.NgayYeuCau <= denNgay);
                }
                if (queryStrings.NgayYeuCauRangDateStartDate != null && queryStrings.NgayYeuCauRangDateStartEnd == null)
                {
                    DateTime TuNgayYCPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayYeuCauRangDateStartDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayYCPart);
                    var tuNgay = new DateTime(TuNgayYCPart.Year, TuNgayYCPart.Month, TuNgayYCPart.Day, 0, 0, 0);
                    queryDangChoDuyet = queryDangChoDuyet.Where(p => p.NgayYeuCau >= tuNgay);
                }
            }
            return queryDangChoDuyet.OrderByDescending(d => d.NgayYeuCau).ToList();
        }
        private List<DSLinhKSNKGridVo> DangChoDuyetDP(QueryInfo queryInfo, bool manHinhDuyet = false)
        {
            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = _phongBenhVienRepository.GetById(noiLamViecCurrentId);
            var queryString = JsonConvert.DeserializeObject<SeachNgay>(queryInfo.AdditionalSearchString);
            var queryDangChoDuyet = _yeuCauLinhDuocPhamRepository.TableNoTracking.Where(x => x.KhoNhap.LaKhoKSNK == true &&  x.DuocDuyet == null && x.DaGui == true && ((manHinhDuyet == true && x.KhoXuat.KhoaPhongId == phongBenhVien.KhoaPhongId) || (manHinhDuyet == false && x.NoiYeuCau != null && phongBenhVien != null && x.NoiYeuCau.KhoaPhongId == phongBenhVien.KhoaPhongId))).Select(s => new DSLinhKSNKGridVo()
            {
                Id = s.Id,
                MaPL = s.SoPhieu,
                LoaiPhieuLinh = s.LoaiPhieuLinh,
                Loai = s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan ? s.NoiYeuCau.KhoaPhong.Ten + " " + (s.LoaiPhieuLinh.GetDescription()).ToLower() : s.LoaiPhieuLinh.GetDescription(),
                LinhTuKhoId = s.KhoNhapId,
                LinhVeKhoId = s.KhoXuatId,
                NguoiYeuCau = s.NhanVienYeuCau.User.HoTen,
                LinhTuKho = s.KhoXuat.Ten,
                LinhVeKho = s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan ? "" : s.KhoNhap.Ten,
                NgayYeuCau = s.NgayYeuCau,
                TinhTrang = "Đang chờ duyệt",
                Nguoiduyet = s.NhanVienDuyet.User.HoTen,
                NgayDuyet = s.NgayDuyet != null ? s.NgayDuyet : (DateTime?)null,
                NgayDuyetHienThi = s.NgayDuyet != null ? Convert.ToDateTime(s.NgayDuyet).ApplyFormatDateTimeSACH() : "",
                DuocDuyet = s.DuocDuyet,
                DaGui = s.DaGui,
                LoaiDuocPhamHayVatTu = true
            });
            if (queryString.Searching != null)
            {
                var searchTamp = queryString.Searching.Trim();
                queryDangChoDuyet = queryDangChoDuyet.ApplyLike(searchTamp, g => g.NguoiYeuCau, g => g.Nguoiduyet, g => g.LinhTuKho, g => g.LinhVeKho, g => g.MaPL);
            }


            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryStrings = JsonConvert.DeserializeObject<SeachNgay>(queryInfo.AdditionalSearchString);
                if (queryStrings.NgayDuyetRangDateStartDate != null && queryStrings.NgayDuyetRangDateStartEnd != null)
                {
                    DateTime TuNgayDuyetPart = DateTime.Now;
                    DateTime DenNgayDuyetsPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayDuyetRangDateStartDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayDuyetPart);
                    DateTime.TryParseExact(queryStrings.NgayDuyetRangDateStartEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgayDuyetsPart);
                    var tuNgay = new DateTime(TuNgayDuyetPart.Year, TuNgayDuyetPart.Month, TuNgayDuyetPart.Day, 0, 0, 0);
                    var denNgay = new DateTime(DenNgayDuyetsPart.Year, DenNgayDuyetsPart.Month, DenNgayDuyetsPart.Day, 23, 59, 59);
                    queryDangChoDuyet = queryDangChoDuyet.Where(p => p.NgayDuyet >= tuNgay && p.NgayDuyet <= denNgay);
                }
                if (queryStrings.NgayDuyetRangDateStartDate != null && queryStrings.NgayDuyetRangDateStartEnd == null)
                {
                    DateTime TuNgayDuyetPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayDuyetRangDateStartDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayDuyetPart);
                    var tuNgay = new DateTime(TuNgayDuyetPart.Year, TuNgayDuyetPart.Month, TuNgayDuyetPart.Day, 0, 0, 0);
                    queryDangChoDuyet = queryDangChoDuyet.Where(p => p.NgayDuyet >= tuNgay);
                }
                if (queryStrings.NgayDuyetRangDateStartDate == null && queryStrings.NgayDuyetRangDateStartEnd != null)
                {
                    DateTime DenNgayDuyetsPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayDuyetRangDateStartEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgayDuyetsPart);
                    var denNgay = new DateTime(DenNgayDuyetsPart.Year, DenNgayDuyetsPart.Month, DenNgayDuyetsPart.Day, 23, 59, 59);
                    queryDangChoDuyet = queryDangChoDuyet.Where(p => p.NgayDuyet <= denNgay);
                }
                if (queryStrings.NgayYeuCauRangDateStartDate != null && queryStrings.NgayYeuCauRangDateStartEnd != null)
                {
                    DateTime TuNgayYCPart = DateTime.Now;
                    DateTime DenNgayYCsPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayYeuCauRangDateStartDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayYCPart);
                    DateTime.TryParseExact(queryStrings.NgayYeuCauRangDateStartEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgayYCsPart);
                    var tuNgay = new DateTime(TuNgayYCPart.Year, TuNgayYCPart.Month, TuNgayYCPart.Day, 0, 0, 0);
                    var denNgay = new DateTime(DenNgayYCsPart.Year, DenNgayYCsPart.Month, DenNgayYCsPart.Day, 23, 59, 59);
                    queryDangChoDuyet = queryDangChoDuyet.Where(p => p.NgayYeuCau >= tuNgay && p.NgayYeuCau <= denNgay);
                }
                if (queryStrings.NgayYeuCauRangDateStartDate != null && queryStrings.NgayYeuCauRangDateStartEnd == null)
                {
                    DateTime TuNgayYCPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayYeuCauRangDateStartDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayYCPart);
                    var tuNgay = new DateTime(TuNgayYCPart.Year, TuNgayYCPart.Month, TuNgayYCPart.Day, 0, 0, 0);
                    queryDangChoDuyet = queryDangChoDuyet.Where(p => p.NgayYeuCau >= tuNgay);
                }
                if (queryStrings.NgayYeuCauRangDateStartDate == null && queryStrings.NgayYeuCauRangDateStartEnd != null)
                {
                    DateTime DenNgayYCsPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayYeuCauRangDateStartEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgayYCsPart);
                    var denNgay = new DateTime(DenNgayYCsPart.Year, DenNgayYCsPart.Month, DenNgayYCsPart.Day, 23, 59, 59);
                    queryDangChoDuyet = queryDangChoDuyet.Where(p => p.NgayYeuCau <= denNgay);
                }
            }
            return queryDangChoDuyet.OrderByDescending(d => d.NgayYeuCau).ToList();
        }
        private List<DSLinhKSNKGridVo> TuChoiDuyetDP(QueryInfo queryInfo, bool manHinhDuyet = false)
        {
            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = _phongBenhVienRepository.GetById(noiLamViecCurrentId);
            var queryString = JsonConvert.DeserializeObject<SeachNgay>(queryInfo.AdditionalSearchString);
            var TCD = _yeuCauLinhDuocPhamRepository.TableNoTracking.Where(x => x.KhoNhap.LaKhoKSNK == true && x.DuocDuyet == false && x.DaGui == true && ((manHinhDuyet == true && x.KhoXuat.KhoaPhongId == phongBenhVien.KhoaPhongId) || (manHinhDuyet == false && x.NoiYeuCau != null && phongBenhVien != null && x.NoiYeuCau.KhoaPhongId == phongBenhVien.KhoaPhongId))).Select(s => new DSLinhKSNKGridVo()
            {
                Id = s.Id,
                LinhTuKhoId = s.KhoNhapId,
                LinhVeKhoId = s.KhoXuatId,
                MaPL = s.SoPhieu,
                LoaiPhieuLinh = s.LoaiPhieuLinh,
                Loai = s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan ? s.NoiYeuCau.KhoaPhong.Ten + " " + (s.LoaiPhieuLinh.GetDescription()).ToLower() : s.LoaiPhieuLinh.GetDescription(),
                NguoiYeuCau = s.NhanVienYeuCau.User.HoTen,
                LinhTuKho = s.KhoXuat.Ten,
                LinhVeKho = s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan ? "" : s.KhoNhap.Ten,
                NgayYeuCau = s.NgayYeuCau,
                TinhTrang = "Từ chối duyệt",
                Nguoiduyet = s.NhanVienDuyet.User.HoTen,
                NgayDuyet = s.NgayDuyet != null ? s.NgayDuyet : (DateTime?)null,
                NgayDuyetHienThi = s.NgayDuyet != null ? Convert.ToDateTime(s.NgayDuyet).ApplyFormatDateTimeSACH() : "",
                DuocDuyet = s.DuocDuyet,
                DaGui = s.DaGui,
                LoaiDuocPhamHayVatTu = true
            });
            if (queryString.Searching != null)
            {
                var searchTamp = queryString.Searching.Trim();
                TCD = TCD.ApplyLike(searchTamp, g => g.NguoiYeuCau, g => g.Nguoiduyet, g => g.LinhTuKho, g => g.LinhVeKho, g => g.MaPL);
            }

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryStrings = JsonConvert.DeserializeObject<SeachNgay>(queryInfo.AdditionalSearchString);
                if (queryStrings.NgayDuyetRangDateStartDate != null && queryStrings.NgayDuyetRangDateStartEnd != null)
                {
                    DateTime TuNgayDuyetPart = DateTime.Now;
                    DateTime DenNgayDuyetsPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayDuyetRangDateStartDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayDuyetPart);
                    DateTime.TryParseExact(queryStrings.NgayDuyetRangDateStartEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgayDuyetsPart);
                    var tuNgay = new DateTime(TuNgayDuyetPart.Year, TuNgayDuyetPart.Month, TuNgayDuyetPart.Day, 0, 0, 0);
                    var denNgay = new DateTime(DenNgayDuyetsPart.Year, DenNgayDuyetsPart.Month, DenNgayDuyetsPart.Day, 23, 59, 59);
                    TCD = TCD.Where(p => p.NgayDuyet >= tuNgay && p.NgayDuyet <= denNgay);
                }
                if (queryStrings.NgayDuyetRangDateStartDate != null && queryStrings.NgayDuyetRangDateStartEnd == null)
                {
                    DateTime TuNgayDuyetPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayDuyetRangDateStartDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayDuyetPart);
                    var tuNgay = new DateTime(TuNgayDuyetPart.Year, TuNgayDuyetPart.Month, TuNgayDuyetPart.Day, 0, 0, 0);
                    TCD = TCD.Where(p => p.NgayDuyet >= tuNgay);
                }
                if (queryStrings.NgayDuyetRangDateStartDate == null && queryStrings.NgayDuyetRangDateStartEnd != null)
                {
                    DateTime DenNgayDuyetsPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayDuyetRangDateStartEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgayDuyetsPart);
                    var denNgay = new DateTime(DenNgayDuyetsPart.Year, DenNgayDuyetsPart.Month, DenNgayDuyetsPart.Day, 23, 59, 59);
                    TCD = TCD.Where(p => p.NgayDuyet <= denNgay);
                }
                if (queryStrings.NgayYeuCauRangDateStartDate != null && queryStrings.NgayYeuCauRangDateStartEnd != null)
                {
                    DateTime TuNgayYCPart = DateTime.Now;
                    DateTime DenNgayYCsPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayYeuCauRangDateStartDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayYCPart);
                    DateTime.TryParseExact(queryStrings.NgayYeuCauRangDateStartEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgayYCsPart);
                    var tuNgay = new DateTime(TuNgayYCPart.Year, TuNgayYCPart.Month, TuNgayYCPart.Day, 0, 0, 0);
                    var denNgay = new DateTime(DenNgayYCsPart.Year, DenNgayYCsPart.Month, DenNgayYCsPart.Day, 23, 59, 59);
                    TCD = TCD.Where(p => p.NgayYeuCau >= tuNgay && p.NgayYeuCau <= denNgay);
                }
                if (queryStrings.NgayYeuCauRangDateStartDate != null && queryStrings.NgayYeuCauRangDateStartEnd == null)
                {
                    DateTime TuNgayYCPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayYeuCauRangDateStartDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayYCPart);
                    var tuNgay = new DateTime(TuNgayYCPart.Year, TuNgayYCPart.Month, TuNgayYCPart.Day, 0, 0, 0);
                    TCD = TCD.Where(p => p.NgayYeuCau >= tuNgay);
                }
                if (queryStrings.NgayYeuCauRangDateStartDate == null && queryStrings.NgayYeuCauRangDateStartEnd != null)
                {
                    DateTime DenNgayYCsPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayYeuCauRangDateStartEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgayYCsPart);
                    var denNgay = new DateTime(DenNgayYCsPart.Year, DenNgayYCsPart.Month, DenNgayYCsPart.Day, 23, 59, 59);
                    TCD = TCD.Where(p => p.NgayYeuCau <= denNgay);
                }
            }
            return TCD.OrderByDescending(d => d.NgayYeuCau).ToList();
        }
        private List<DSLinhKSNKGridVo> DaDuyetDP(QueryInfo queryInfo, bool manHinhDuyet = false)
        {
            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = _phongBenhVienRepository.GetById(noiLamViecCurrentId);
            var queryString = JsonConvert.DeserializeObject<SeachNgay>(queryInfo.AdditionalSearchString);
            var queryDaDuyet = _yeuCauLinhDuocPhamRepository.TableNoTracking.Where(x => x.KhoNhap.LaKhoKSNK == true && x.DuocDuyet == true && x.DaGui == true && ((manHinhDuyet == true && x.KhoXuat.KhoaPhongId == phongBenhVien.KhoaPhongId) || (manHinhDuyet == false && x.NoiYeuCau != null && phongBenhVien != null && x.NoiYeuCau.KhoaPhongId == phongBenhVien.KhoaPhongId))).Select(s => new DSLinhKSNKGridVo()
            {
                Id = s.Id,
                LinhTuKhoId = s.KhoNhapId,
                LinhVeKhoId = s.KhoXuatId,
                MaPL = s.SoPhieu,
                LoaiPhieuLinh = s.LoaiPhieuLinh,
                Loai = s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan ? s.NoiYeuCau.KhoaPhong.Ten + " " + (s.LoaiPhieuLinh.GetDescription()).ToLower() : s.LoaiPhieuLinh.GetDescription(),
                NguoiYeuCau = s.NhanVienYeuCau.User.HoTen,
                LinhTuKho = s.KhoXuat.Ten,
                LinhVeKho = s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan ? "" : s.KhoNhap.Ten,
                NgayYeuCau = s.NgayYeuCau,
                TinhTrang = "Đã duyệt",
                Nguoiduyet = s.NhanVienDuyet.User.HoTen,
                NgayDuyet = s.NgayDuyet != null ? s.NgayDuyet : (DateTime?)null,
                NgayDuyetHienThi = s.NgayDuyet != null ? Convert.ToDateTime(s.NgayDuyet).ApplyFormatDateTimeSACH() : "",
                DuocDuyet = s.DuocDuyet,
                DaGui = s.DaGui,
                LoaiDuocPhamHayVatTu = true
            });
            if (queryString.Searching != null)
            {
                var searchTamp = queryString.Searching.Trim();
                queryDaDuyet = queryDaDuyet.ApplyLike(searchTamp, g => g.NguoiYeuCau, g => g.Nguoiduyet, g => g.LinhTuKho, g => g.LinhVeKho, g => g.MaPL);
            }
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryStrings = JsonConvert.DeserializeObject<SeachNgay>(queryInfo.AdditionalSearchString);
                if (queryStrings.NgayDuyetRangDateStartDate != null && queryStrings.NgayDuyetRangDateStartEnd != null)
                {
                    DateTime TuNgayDuyetPart = DateTime.Now;
                    DateTime DenNgayDuyetsPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayDuyetRangDateStartDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayDuyetPart);
                    DateTime.TryParseExact(queryStrings.NgayDuyetRangDateStartEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgayDuyetsPart);
                    var tuNgay = new DateTime(TuNgayDuyetPart.Year, TuNgayDuyetPart.Month, TuNgayDuyetPart.Day, 0, 0, 0);
                    var denNgay = new DateTime(DenNgayDuyetsPart.Year, DenNgayDuyetsPart.Month, DenNgayDuyetsPart.Day, 23, 59, 59);
                    queryDaDuyet = queryDaDuyet.Where(p => p.NgayDuyet >= tuNgay && p.NgayDuyet <= denNgay);
                }
                if (queryStrings.NgayDuyetRangDateStartDate != null && queryStrings.NgayDuyetRangDateStartEnd == null)
                {
                    DateTime TuNgayDuyetPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayDuyetRangDateStartDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayDuyetPart);
                    var tuNgay = new DateTime(TuNgayDuyetPart.Year, TuNgayDuyetPart.Month, TuNgayDuyetPart.Day, 0, 0, 0);
                    queryDaDuyet = queryDaDuyet.Where(p => p.NgayDuyet >= tuNgay);
                }
                if (queryStrings.NgayDuyetRangDateStartDate == null && queryStrings.NgayDuyetRangDateStartEnd != null)
                {
                    DateTime DenNgayDuyetsPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayDuyetRangDateStartEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgayDuyetsPart);
                    var denNgay = new DateTime(DenNgayDuyetsPart.Year, DenNgayDuyetsPart.Month, DenNgayDuyetsPart.Day, 23, 59, 59);
                    queryDaDuyet = queryDaDuyet.Where(p => p.NgayDuyet <= denNgay);
                }
                if (queryStrings.NgayYeuCauRangDateStartDate != null && queryStrings.NgayYeuCauRangDateStartEnd != null)
                {
                    DateTime TuNgayYCPart = DateTime.Now;
                    DateTime DenNgayYCsPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayYeuCauRangDateStartDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayYCPart);
                    DateTime.TryParseExact(queryStrings.NgayYeuCauRangDateStartEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgayYCsPart);
                    var tuNgay = new DateTime(TuNgayYCPart.Year, TuNgayYCPart.Month, TuNgayYCPart.Day, 0, 0, 0);
                    var denNgay = new DateTime(DenNgayYCsPart.Year, DenNgayYCsPart.Month, DenNgayYCsPart.Day, 23, 59, 59);
                    queryDaDuyet = queryDaDuyet.Where(p => p.NgayYeuCau >= tuNgay && p.NgayYeuCau <= denNgay);
                }
                if (queryStrings.NgayYeuCauRangDateStartDate != null && queryStrings.NgayYeuCauRangDateStartEnd == null)
                {
                    DateTime TuNgayYCPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayYeuCauRangDateStartDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayYCPart);
                    var tuNgay = new DateTime(TuNgayYCPart.Year, TuNgayYCPart.Month, TuNgayYCPart.Day, 0, 0, 0);
                    queryDaDuyet = queryDaDuyet.Where(p => p.NgayYeuCau >= tuNgay);
                }
                if (queryStrings.NgayYeuCauRangDateStartDate == null && queryStrings.NgayYeuCauRangDateStartEnd != null)
                {
                    DateTime DenNgayYCsPart = DateTime.Now;
                    DateTime.TryParseExact(queryStrings.NgayYeuCauRangDateStartEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgayYCsPart);
                    var denNgay = new DateTime(DenNgayYCsPart.Year, DenNgayYCsPart.Month, DenNgayYCsPart.Day, 23, 59, 59);
                    queryDaDuyet = queryDaDuyet.Where(p => p.NgayYeuCau <= denNgay);
                }
            }
            return queryDaDuyet.OrderByDescending(d => d.NgayYeuCau).ToList();
        }

        #endregion
        #region child dp
         #region child lĩnh dược phẩm con 
        #region Ds yêu cầu DuocPham child
        public async Task<GridDataSource> GetDataDSYeuCauLinhDuocPhamKSNKChildForGridAsync(QueryInfo queryInfo)
        {
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            int loaiPhieulinh = int.Parse(queryString[1]); // 1 loai phieu linh
            int trangThai = 0;

            EnumLoaiPhieuLinh loaiPhieu;
            if (loaiPhieulinh == 1)
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhDuTru;
            }
            else 
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhBu;
            }
           
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhBu)
            {
                BuildDefaultSortExpression(queryInfo);
                var trangThaiLinhBu = _yeuCauLinhDuocPhamRepository.TableNoTracking.Where(d => d.Id == long.Parse(queryString[0])).Select(d => d.DuocDuyet).FirstOrDefault();

                var query = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                            .Where(p => p.YeuCauLinhDuocPhamId == long.Parse(queryString[0])
                                //&& p.YeuCauDuocPhamBenhVien.TrangThai == EnumYeuCauDuocPhamBenhVien.DaThucHien
                                && (p.YeuCauLinhDuocPham.DuocDuyet == false ? p.YeuCauDuocPhamBenhVien.KhongLinhBu != false : p.YeuCauDuocPhamBenhVien.KhongLinhBu != true)
                            )
                            .Select(s => new YeuCauLinhKSNKBuGridVo
                            {
                                Id = s.Id,
                                YeuCauLinhKSNKId = s.YeuCauLinhDuocPhamId,
                                KSNKBenhVienId = s.DuocPhamBenhVienId,
                                TenKSNK = s.DuocPhamBenhVien.DuocPham.Ten,
                                DonViTinh = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                                HangSanXuat = s.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                                NuocSanXuat = s.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                                LaBHYT = s.LaDuocPhamBHYT,
                                Nhom = s.LaDuocPhamBHYT == true ? "Dược Phẩm BHYT" : "Dược Phẩm Không BHYT",
                                SoLuongCanBu = s.SoLuongCanBu != null ? (double)s.SoLuongCanBu.MathRoundNumber(2) : 0,
                                SLDaLinh = s.YeuCauLinhDuocPhamId != null ? (double)s.YeuCauDuocPhamBenhVien.SoLuongDaLinhBu != null ? (double)s.YeuCauDuocPhamBenhVien.SoLuongDaLinhBu : 0 : 0,
                                LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhBu,
                                LinhVeKhoId = long.Parse(queryString[2]),
                                NongDoHamLuong = s.DuocPhamBenhVien.DuocPham.HamLuong,
                                HoatChat = s.DuocPhamBenhVien.DuocPham.HoatChat,
                                DuongDung = s.DuocPhamBenhVien.DuocPham.DuongDung.Ten,
                                DaDuyet = s.YeuCauLinhDuocPhamId != null ? s.YeuCauLinhDuocPham.DuocDuyet : null,
                                SoLuongYeuCauDaDuyet = s.SoLuong // trường hợp cho đã duyệt

                            })
                            .GroupBy(x => new { x.YeuCauLinhKSNKId, x.KSNKBenhVienId, x.LaBHYT, x.Nhom, x.DonViTinh, x.HangSanXuat, x.NuocSanXuat, x.SoLuongTon })
                              .Select(item => new YeuCauLinhKSNKBuGridVo()
                              {
                                  Id = item.First().Id,
                                  KSNKBenhVienId = item.First().KSNKBenhVienId,
                                  LaBHYT = item.First().LaBHYT,
                                  TenKSNK = item.First().TenKSNK,
                                  Nhom = item.First().Nhom,
                                  DonViTinh = item.First().DonViTinh,
                                  HangSanXuat = item.First().HangSanXuat,
                                  NuocSanXuat = item.First().NuocSanXuat,
                                  SoLuongCanBu = item.Sum(x => x.SoLuongCanBu.MathRoundNumber(2)),
                                  LinhVeKhoId = long.Parse(queryString[2]),
                                  LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhBu,
                                  NongDoHamLuong = item.FirstOrDefault().NongDoHamLuong,
                                  SLDaLinh = item.Sum(x => x.SLDaLinh),
                                  HoatChat = item.First().HoatChat,
                                  DuongDung = item.First().DuongDung,
                                  DaDuyet = item.First().DaDuyet,
                                  SoLuongYeuCauDaDuyet = item.Sum(s => s.SoLuongYeuCauDaDuyet)
                              })
                              ;
                var DuocPhamLinhBuGridVos = query.ToList();
                if(trangThaiLinhBu == null)
                {
                    var lstDuocPhamBenhVien = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(x => x.NhapKhoDuocPhams.KhoId == long.Parse(queryString[2])
                      ).ToList();

                    var result = DuocPhamLinhBuGridVos.Where(p => lstDuocPhamBenhVien.Any(o => o.DuocPhamBenhVienId == p.KSNKBenhVienId && o.LaDuocPhamBHYT == p.LaBHYT));

                    result = result.Select(o =>
                    {
                        o.SoLuongTon = lstDuocPhamBenhVien.Where(t => t.DuocPhamBenhVienId == o.KSNKBenhVienId && t.LaDuocPhamBHYT == o.LaBHYT).Sum(t => t.SoLuongNhap - t.SoLuongDaXuat);
                        o.SoLuongYeuCau = (o.SLDaLinh == 0 || o.SLDaLinh == null) ? (o.SoLuongTon < o.SoLuongCanBu ? o.SoLuongTon.MathRoundNumber(2) : o.SoLuongCanBu.MathRoundNumber(2))
                                                             : (o.SoLuongTon < (o.SoLuongCanBu - o.SLDaLinh) ? o.SoLuongTon.MathRoundNumber(2) : (o.SoLuongCanBu - o.SLDaLinh).MathRoundNumber(2));
                        return o;
                    });
                    result = result.OrderBy(x => x.LaBHYT).ThenBy(x => x.TenKSNK).Distinct();
                    var countTask = queryInfo.LazyLoadPage == true ? 0 : result.Count();
                    var queryTask = result.ToArray();
                    return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
                }
                else
                {
                    query = query.OrderBy(x => x.LaBHYT).ThenBy(x => x.TenKSNK).Distinct();
                    var countTask = queryInfo.LazyLoadPage == true ? 0 : query.Count();
                    var queryTask = query.ToArray();
                    return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
                }
               
            }
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhDuTru) 
            {
                BuildDefaultSortExpression(queryInfo);
                var query = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                     .Where(o =>
                             o.YeuCauLinhDuocPham.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhDuTru &&
                             o.YeuCauLinhDuocPhamId == long.Parse(queryString[0])
                            )
                    .Select(s => new DSLinhVatChildTuGridVo
                    {
                        Id = long.Parse(queryString[0]),
                        LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhDuTru,
                        Ma = s.DuocPhamBenhVien.DuocPham.MaHoatChat,
                        TenKSNK = s.DuocPhamBenhVien.DuocPham.Ten,
                        DonViTinh = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                        SoLuongYc = s.SoLuong,
                        HangSanXuat = s.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                        NuocSanXuat = s.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                        BSKeToa = s.YeuCauDuocPhamBenhVien.NhanVienChiDinh.User.HoTen,
                        Nhom = s.LaDuocPhamBHYT == true ? "BHYT":"Không BHYT",
                        HoatChat = s.DuocPhamBenhVien.DuocPham.HoatChat,
                        NongDoHamLuong = s.DuocPhamBenhVien.DuocPham.HamLuong,
                        DuongDung = s.DuocPhamBenhVien.DuocPham.DuongDung.Ten,
                        SLTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(o => o.DuocPhamBenhVienId == s.DuocPhamBenhVienId && o.LaDuocPhamBHYT == s.LaDuocPhamBHYT && o.NhapKhoDuocPhams.KhoId == s.YeuCauLinhDuocPham.KhoXuatId ).Sum(o => o.SoLuongNhap - o.SoLuongDaXuat).MathRoundNumber(1),
                        DuocDuyet = s.YeuCauLinhDuocPham != null ? s.YeuCauLinhDuocPham.DuocDuyet :null,
                        LaBHYT = s.LaDuocPhamBHYT,
                        TenKhoLinh = s.YeuCauLinhDuocPham.KhoXuat.Ten
                    });
                var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
                var queryTask = query.OrderBy(d=>d.LaBHYT).ThenBy(d=>d.TenKSNK).Skip(queryInfo.Skip)
                    .Take(queryInfo.Take).ToArrayAsync();

                await Task.WhenAll(countTask, queryTask);
                return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
            }
            return null;
        }
        public async Task<GridDataSource> GetDataDSYeuCauLinhDuocPhamKSNKChildChildForGridAsync(QueryInfo queryInfo)
        {
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            int loaiPhieulinh = int.Parse(queryString[1]);
            EnumLoaiPhieuLinh loaiPhieu;
            if (loaiPhieulinh == 1)
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhDuTru;
            }
            else 
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhBu;
            }
           
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhBu)
            {

                //var query = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                //.Where(o => o.DuocPhamBenhVienId == long.Parse(queryString[2])
                //            && o.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu
                //            && o.KhongLinhBu != true
                //            && o.YeuCauLinhDuocPhamId != null
                //            && (o.SoLuongDaLinhBu == null || o.SoLuongDaLinhBu < o.SoLuong)
                //           )
                var trangThaiDuyet = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking.Where(x => x.Id == long.Parse(queryString[0])).Select(s => s.YeuCauLinhDuocPham.DuocDuyet).FirstOrDefault();
                var yeuCauLinhDuocPhamId = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking.Where(x => x.Id == long.Parse(queryString[0])).Select(s => s.YeuCauLinhDuocPhamId).FirstOrDefault();
                var query = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                //    .Where(o => o.YeuCauLinhDuocPhamId == yeuCauLinhId
                //            && o.DuocPhamBenhVienId == duocPhamBenhVienId
                //            && o.YeuCauDuocPhamBenhVien.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhBu
                //            && o.LaDuocPhamBHYT == laBHYT
                //            && o.YeuCauDuocPhamBenhVien.TrangThai == Enums.EnumYeuCauDuocPhamBenhVien.DaThucHien)
                //.OrderBy(x => x.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh)
                            .Where(p => p.YeuCauLinhDuocPhamId == yeuCauLinhDuocPhamId
                             && p.DuocPhamBenhVienId == long.Parse(queryString[2])
                             && p.LaDuocPhamBHYT == bool.Parse(queryString[3])
                             && p.YeuCauDuocPhamBenhVien.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhBu
                             && p.YeuCauLinhDuocPham.DuocDuyet == trangThaiDuyet
                            //&& (p.YeuCauLinhDuocPham.DuocDuyet == false ? p.YeuCauDuocPhamBenhVien.KhongLinhBu != false : p.YeuCauDuocPhamBenhVien.KhongLinhBu != true)
                            )
                .OrderBy(x => x.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh)
                .Select(s => new KSNKLinhBuCuaBNGridVos
                {
                    Id = s.Id,
                    MaTN = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    MaBN = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.BenhNhan.MaBN,
                    HoTen = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.HoTen,
                    SL = trangThaiDuyet == true ? s.SoLuong : s.SoLuongCanBu,
                    DVKham = s.YeuCauDuocPhamBenhVien.YeuCauKhamBenh.TenDichVu ?? s.YeuCauDuocPhamBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu ?? null,
                    BSKeToa = s.YeuCauDuocPhamBenhVien.NhanVienChiDinh.User.HoTen,
                    NgayKe = s.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                    SLDaLinh = s.SoLuongDaLinhBu != null ? s.SoLuongDaLinhBu : 0,
                    SLDanhSachDuyet = s.SoLuong.MathRoundNumber(2),
                    SLCanBu = s.SoLuongCanBu,
                    NgayDieuTri = s.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri != null ? s.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri.NgayDieuTri : s.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh
                });
                var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
                var queryTask = query.Skip(queryInfo.Skip)
                    .Take(queryInfo.Take).ToArrayAsync();
                await Task.WhenAll(countTask, queryTask);
                return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
            }
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhDuTru)
            {
                BuildDefaultSortExpression(queryInfo);
                var query = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                     .Where(o =>
                             o.YeuCauLinhDuocPham.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhDuTru &&
                             o.YeuCauLinhDuocPhamId == long.Parse(queryString[0])
                            )
                    .Select(s => new DSLinhVatChildTuGridVo
                    {
                        Id = s.Id,
                        Ma = s.DuocPhamBenhVien.DuocPham.MaHoatChat,
                        TenKSNK = s.DuocPhamBenhVien.DuocPham.Ten,
                        DonViTinh = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                        SoLuongYc = s.SoLuong,
                        HangSanXuat = s.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                        NuocSanXuat = s.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                        BSKeToa = s.YeuCauDuocPhamBenhVien.NhanVienChiDinh.User.HoTen,
                        Nhom = s.LaDuocPhamBHYT == true ? "BHYT" : "Không BHYT",
                        TenKhoLinh = s.YeuCauLinhDuocPham.KhoXuat.Ten
                    });
                var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
                var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                    .Take(queryInfo.Take).ToArrayAsync();

                await Task.WhenAll(countTask, queryTask);
                return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
            }
            return null;
        }
        public async Task<GridDataSource> GetTotalPageFDSYeuCauLinhDuocPhamKSNKChildForGridAsync(QueryInfo queryInfo)
        {
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            int loaiPhieulinh = int.Parse(queryString[1]);
            EnumLoaiPhieuLinh loaiPhieu;

            if (loaiPhieulinh == 1)
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhDuTru;
            }
            else 
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhBu;
            }
            
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhBu)
            {

                BuildDefaultSortExpression(queryInfo);
                var query = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                            .Where(p => p.YeuCauLinhDuocPhamId == long.Parse(queryString[0])
                              //&& p.YeuCauDuocPhamBenhVien.TrangThai == EnumYeuCauDuocPhamBenhVien.ChuaThucHien
                              && (p.YeuCauLinhDuocPham.DuocDuyet == false ? p.YeuCauDuocPhamBenhVien.KhongLinhBu != false : p.YeuCauDuocPhamBenhVien.KhongLinhBu != true)
                            //&& (p.YeuCauDuocPhamBenhVien.SoLuongDaLinhBu == null || p.YeuCauDuocPhamBenhVien.SoLuongDaLinhBu < p.YeuCauDuocPhamBenhVien.SoLuong)
                            )
                            .Select(s => new YeuCauLinhKSNKBuGridVo
                            {
                                Id = s.Id,
                                YeuCauLinhKSNKId = s.YeuCauLinhDuocPhamId,
                                KSNKBenhVienId = s.DuocPhamBenhVienId,
                                TenKSNK = s.DuocPhamBenhVien.DuocPham.Ten,
                                DonViTinh = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                                HangSanXuat = s.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                                NuocSanXuat = s.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                                LaBHYT = s.LaDuocPhamBHYT,
                                Nhom = s.LaDuocPhamBHYT == true ? "Dược Phẩm BHYT" : "Dược Phẩm Không BHYT",
                                SoLuongCanBu = s.SoLuongCanBu != null ? (double)s.SoLuongCanBu : 0,
                                SLDaLinh = s.YeuCauDuocPhamBenhVien != null ? (double)s.YeuCauDuocPhamBenhVien.SoLuongDaLinhBu != null ? (double)s.YeuCauDuocPhamBenhVien.SoLuongDaLinhBu : 0 : 0,
                                LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhBu,
                                LinhVeKhoId = long.Parse(queryString[2])
                            })
                            .GroupBy(x => new { x.YeuCauLinhKSNKId, x.KSNKBenhVienId, x.LaBHYT, x.Nhom, x.DonViTinh, x.HangSanXuat, x.NuocSanXuat, x.SoLuongTon })
                              .Select(item => new YeuCauLinhKSNKBuGridVo()
                              {
                                  KSNKBenhVienId = item.First().KSNKBenhVienId,
                                  LaBHYT = item.First().LaBHYT,
                                  TenKSNK = item.First().TenKSNK,
                                  Nhom = item.First().Nhom,
                                  DonViTinh = item.First().DonViTinh,
                                  HangSanXuat = item.First().HangSanXuat,
                                  NuocSanXuat = item.First().NuocSanXuat,
                                  SoLuongCanBu = item.Sum(x => x.SoLuongCanBu),
                                  LinhVeKhoId = long.Parse(queryString[2]),
                                  LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhBu,
                                  SLDaLinh = item.Sum(x => x.SLDaLinh)
                              })
                              .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenKSNK).Distinct();
                var DuocPhamLinhBuGridVos = query.ToList();

                var lstDuocPhamBenhVien = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(x => x.NhapKhoDuocPhams.KhoId == long.Parse(queryString[2])
                       ).ToList();

                var result = DuocPhamLinhBuGridVos.Where(p => lstDuocPhamBenhVien.Any(o => o.DuocPhamBenhVienId == p.KSNKBenhVienId && o.LaDuocPhamBHYT == p.LaBHYT));

                result = result.Select(o =>
                {
                    //o.SoLuongTon = lstVatTuBenhVien.Where(t => t.DuocPhamBenhVienId == o.DuocPhamBenhVienId && t.LaDuocPhamBHYT == o.LaBHYT).Sum(t => t.SoLuongNhap - t.SoLuongDaXuat);
                    //o.SoLuongYeuCau = o.SoLuongTon < o.SoLuongCanBu ? o.SoLuongTon : o.SoLuongCanBu;

                    o.SoLuongTon = lstDuocPhamBenhVien.Where(t => t.DuocPhamBenhVienId == o.KSNKBenhVienId && t.LaDuocPhamBHYT == o.LaBHYT).Sum(t => t.SoLuongNhap - t.SoLuongDaXuat);
                    o.SoLuongYeuCau = (o.SLDaLinh == 0 || o.SLDaLinh == null) ? (o.SoLuongTon < o.SoLuongCanBu ? o.SoLuongTon.MathRoundNumber(2) : o.SoLuongCanBu.MathRoundNumber(2))
                                                         : (o.SoLuongTon < (o.SoLuongCanBu - o.SLDaLinh) ? o.SoLuongTon.MathRoundNumber(2) : (o.SoLuongCanBu - o.SLDaLinh).MathRoundNumber(2));
                    return o;
                });
                var dataOrderBy = result.AsQueryable().OrderBy(queryInfo.SortString);
                var countTask = dataOrderBy.Count();

                return new GridDataSource { TotalRowCount = countTask };
            }
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhDuTru)
            {
                BuildDefaultSortExpression(queryInfo);
                var query = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                     .Where(o =>
                             o.YeuCauLinhDuocPham.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhDuTru &&
                             o.YeuCauLinhDuocPhamId == long.Parse(queryString[0])
                            )
                    .Select(s => new DSLinhVatChildTuGridVo
                    {
                        Id = long.Parse(queryString[0]),
                        LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhDuTru,
                        Ma = s.DuocPhamBenhVien.DuocPham.MaHoatChat,
                        TenKSNK = s.DuocPhamBenhVien.DuocPham.Ten,
                        DonViTinh = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                        SoLuongYc = s.SoLuong,
                        HangSanXuat = s.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                        NuocSanXuat = s.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                        BSKeToa = s.YeuCauDuocPhamBenhVien.NhanVienChiDinh.User.HoTen,
                        Nhom = s.LaDuocPhamBHYT == true ? "BHYT" : "Không BHYT"
                    });
                var countTask = query.CountAsync();
                await Task.WhenAll(countTask);

                return new GridDataSource { TotalRowCount = countTask.Result };
            }
            return null;
        }
        public async Task<GridDataSource> GetTotalPageFDSYeuCauLinhDuocPhamKSNKChildChildForGridAsync(QueryInfo queryInfo)
        {
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            int loaiPhieulinh = int.Parse(queryString[1]);
            EnumLoaiPhieuLinh loaiPhieu;
            if (loaiPhieulinh == 1)
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhDuTru;
            }
            else 
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhBu;
            }
            
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhBu)
            {
                var yeuCauLinhDuocPhamId = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking.Where(x => x.Id == long.Parse(queryString[0])).Select(s => s.YeuCauLinhDuocPhamId).FirstOrDefault();
                var query = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                            .Where(p => p.YeuCauLinhDuocPhamId == yeuCauLinhDuocPhamId
                           && p.YeuCauDuocPhamBenhVien.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                            && (p.YeuCauLinhDuocPham.DuocDuyet == false ? p.YeuCauDuocPhamBenhVien.KhongLinhBu != false : p.YeuCauDuocPhamBenhVien.KhongLinhBu != true)
                            )
                .OrderBy(x => x.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh)
                .Select(s => new KSNKLinhBuCuaBNGridVos
                {
                    Id = s.Id,
                    MaTN = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    MaBN = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.BenhNhan.MaBN,
                    HoTen = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.HoTen,
                    SL = s.SoLuongCanBu,
                    DVKham = s.YeuCauDuocPhamBenhVien.YeuCauKhamBenh.TenDichVu ?? s.YeuCauDuocPhamBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu ?? null,
                    BSKeToa = s.YeuCauDuocPhamBenhVien.NhanVienChiDinh.User.HoTen,
                    NgayKe = s.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                    SLDaLinh = s.SoLuongDaLinhBu != null ? s.SoLuongDaLinhBu : 0
                });
                var countTask = query.CountAsync();
                await Task.WhenAll(countTask);

                return new GridDataSource { TotalRowCount = countTask.Result };
            }
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhDuTru)
            {
                BuildDefaultSortExpression(queryInfo);
                var query = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                     .Where(o =>
                             o.YeuCauLinhDuocPham.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhDuTru &&
                             o.YeuCauLinhDuocPhamId == long.Parse(queryString[0])
                            )
                    .Select(s => new DSLinhVatChildTuGridVo
                    {
                        Id = s.Id,
                        Ma = s.DuocPhamBenhVien.DuocPham.MaHoatChat,
                        TenKSNK = s.DuocPhamBenhVien.DuocPham.Ten,
                        DonViTinh = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                        SoLuongYc = s.SoLuong,
                        HangSanXuat = s.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                        NuocSanXuat = s.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                        BSKeToa = s.YeuCauDuocPhamBenhVien.NhanVienChiDinh.User.HoTen,
                        Nhom = s.LaDuocPhamBHYT == true ? "BHYT" : "Không BHYT",
                        TenKhoLinh = s.YeuCauLinhDuocPham.KhoXuat.Ten
                    });
                var countTask = query.CountAsync();
                await Task.WhenAll(countTask);

                return new GridDataSource { TotalRowCount = countTask.Result };
            }
            return null;
        }
        #endregion

        #endregion
        #endregion
    }
}
