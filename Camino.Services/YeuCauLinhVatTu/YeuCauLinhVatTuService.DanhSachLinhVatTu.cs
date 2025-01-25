using Camino.Core.Data;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.LinhVatTu;
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

namespace Camino.Services.YeuCauLinhVatTu
{
    public partial class YeuCauLinhVatTuService
    {
        #region  Ds linh duoc phẩm
        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool print)
        {

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                BuildDefaultSortExpression(queryInfo);
                var queryString = JsonConvert.DeserializeObject<SeachNgay>(queryInfo.AdditionalSearchString);

                var queryDangChoGoi = DangChoGoi(queryInfo);
                var queryDangChoDuyet = DangChoDuyet(queryInfo);
                var queryTuChoiDuyet = TuChoiDuyet(queryInfo);
                var queryDaDuyet = DaDuyet(queryInfo);

                var query = new List<DSLinhVatTuGridVo>();

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
                    query = new List<DSLinhVatTuGridVo>();
                    query = queryDangChoGoi.Union(queryDangChoDuyet).Union(queryTuChoiDuyet).Union(queryDaDuyet).ToList();
                }

                var dataOrderBy = query.AsQueryable();
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
                var queryDangChoGui = BaseRepository.TableNoTracking.Where(x => x.DuocDuyet == null && x.DaGui != true).Select(s => new DSLinhVatTuGridVo()
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
                var queryDangChoDuyet = BaseRepository.TableNoTracking.Where(x => x.DuocDuyet == null).Select(s => new DSLinhVatTuGridVo()
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
                var queryTuChoiDuyet = BaseRepository.TableNoTracking.Where(x => x.DuocDuyet == false).Select(s => new DSLinhVatTuGridVo()
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
                var queryDaDuyet = BaseRepository.TableNoTracking.Where(x => x.DuocDuyet == true).Select(s => new DSLinhVatTuGridVo()
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
                var query = queryDangChoGui.Union(queryDangChoDuyet).Union(queryTuChoiDuyet).Union(queryDaDuyet);
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

                var queryDangChoGoi = DangChoGoi(queryInfo);
                var queryDangChoDuyet = DangChoDuyet(queryInfo);
                var queryTuChoiDuyet = TuChoiDuyet(queryInfo);
                var queryDaDuyet = DaDuyet(queryInfo);

                var query = new List<DSLinhVatTuGridVo>();

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
                    query = new List<DSLinhVatTuGridVo>();
                    query = queryDangChoGoi.Union(queryDangChoDuyet).Union(queryTuChoiDuyet).Union(queryDaDuyet).ToList();
                }
                var dataOrderBy = query.AsQueryable();
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
        public async Task<string> InLinhVatTu(XacNhanInLinhVatTu xacNhanInLinhDuocPham)
        {
            var content = "";
            var ThuocHoacVatTu = " ";
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
               .FirstOrDefault(x => x.Name.Equals("PhieuLinhVatTuTrucTiep"));

            var kiemTraPhieuLinhTinhTrangDuyet = _yeuCauLinhVatTuRepository.TableNoTracking.Where(d => d.Id == xacNhanInLinhDuocPham.YeuCauLinhVatTuId).Select(d => d.DuocDuyet).First();

            var yeuCauLinhVatTu = BaseRepository.GetByIdAsync(xacNhanInLinhDuocPham.YeuCauLinhVatTuId,
                                                                s => s.Include(z => z.KhoNhap)
                                                                     .Include(z => z.KhoXuat)
                                                                     .Include(z => z.NhanVienYeuCau)
                                                                     .Include(z => z.NhanVienDuyet)
                                                                     .Include(z=>z.YeuCauLinhVatTuChiTiets).ThenInclude(k => k.VatTuBenhVien).ThenInclude(w => w.VatTus)
                                                                     .Include(z => z.YeuCauVatTuBenhViens).ThenInclude(k=>k.VatTuBenhVien).ThenInclude(w=>w.VatTus)
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
                    if (yeuCauLinhVatTu.Result != null)
                    {
                        var vt = yeuCauLinhVatTu.Result.YeuCauLinhVatTuChiTiets
                                                                .Select(o => new VatTuGridVo
                                                                {
                                                                    MaVatTu = o.VatTuBenhVien.Ma,
                                                                    TenVatTu = o.VatTuBenhVien.VatTus.Ten + (o.VatTuBenhVien.VatTus.NhaSanXuat != null && o.VatTuBenhVien.VatTus.NhaSanXuat != "" ? "; " + o.VatTuBenhVien.VatTus.NhaSanXuat : "") +
                                                                                                             (o.VatTuBenhVien.VatTus.NuocSanXuat != null && o.VatTuBenhVien.VatTus.NuocSanXuat != "" ? "; " + o.VatTuBenhVien.VatTus.NuocSanXuat : ""),
                                                                    SoLuong = o.SoLuong,
                                                                    DonViTinh = o.VatTuBenhVien.VatTus.DonViTinh,
                                                                    SoLuongCoTheXuat = o.SoLuongCoTheXuat,
                                                                    LaVatTuBHYT = o.LaVatTuBHYT
                                                                })
                                                               .GroupBy(xy => new { xy.TenVatTu, xy.MaVatTu, xy.DonViTinh })
                                                               .Select(o => new VatTuGridVo
                                                               {
                                                                   MaVatTu = o.First().MaVatTu,
                                                                   TenVatTu = o.First().TenVatTu,
                                                                   SoLuong = o.Sum(s => s.SoLuong),
                                                                   DonViTinh = o.First().DonViTinh,
                                                                   SoLuongCoTheXuat = o.Sum(s => s.SoLuongCoTheXuat)
                                                               }).OrderBy(d=>d.TenVatTu);
                        var objData = GetHTMLLinhBenhNhanTuChoi(vt.OrderBy(d=>d.LaVatTuBHYT).ThenBy(d=>d.TenVatTu).ToList());
                        ThuocHoacVatTu = objData.html;
                        index = objData.Index;
                    }
                    else
                    {
                        var yeucau = 0; // to do
                        var thucChat = ""; // to do
                        var tenLoaiLinh = "";
                        var donViTinh = "";
                        var maHoatChat = "";
                        if (yeuCauLinhVatTu.Result.YeuCauVatTuBenhViens.Where(s => s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan).Any())
                        {
                            var objData = GetHTMLLinhBenhNhanDataoDaDuyet(yeuCauLinhVatTu.Result.YeuCauVatTuBenhViens.Where(s => s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan).ToList());
                            ThuocHoacVatTu = objData.html;
                            index = objData.Index;
                            ThuocHoacVatTu = objData.html;
                        }
                        
                    }






                    var maVachPhieuLinh = yeuCauLinhVatTu.Result.SoPhieu.ToString();
                    var data = new
                    {
                        LogoUrl = xacNhanInLinhDuocPham.Hosting + "/assets/img/logo-bacha-full.png",
                        BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauLinhVatTu.Result.SoPhieu.ToString()) ? BarcodeHelper.GenerateBarCode(maVachPhieuLinh) : "",
                        MaVachPhieuLinh = maVachPhieuLinh,
                        NoiGiao = yeuCauLinhVatTu?.Result?.KhoXuat?.Ten,
                        DienGiai = yeuCauLinhVatTu?.Result?.GhiChu,
                        TruongKhoaDuocVTYT = "",
                        NguoiGiao = yeuCauLinhVatTu?.Result?.NhanVienDuyet?.User?.HoTen,
                        NguoiNhan = yeuCauLinhVatTu.Result?.NhanVienYeuCau?.User?.HoTen,
                        TuNgay = yeuCauLinhVatTu?.Result?.ThoiDiemLinhTongHopTuNgay?.ApplyFormatDateTimeSACH(),
                        DenNgay = yeuCauLinhVatTu?.Result?.ThoiDiemLinhTongHopDenNgay?.ApplyFormatDateTimeSACH(),
                        TruongPhongKhoaPhong = "",
                        CongKhoan = index - 1,
                        NgayThangNam = DateTime.Now.ApplyFormatDateTimeSACH(),
                        //HeaderPhieuLinhThuoc = tenLoaiLinh,
                        ThuocHoacVatTu = ThuocHoacVatTu,
                        KhoaPhong = yeuCauLinhVatTu?.Result?.KhoNhap?.Ten,
                        Ngay = DateTime.Now.Day,
                        Thang = DateTime.Now.Month,
                        Nam = DateTime.Now.Year,
                        Gio = DateTime.Now.ApplyFormatTime(),
                        NoiNhan=TenNoiNhanPhieuLinhTrucTiep((long)yeuCauLinhVatTu?.Result?.NoiYeuCauId)
                    };
                    content = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data);
                    return content;
                }
                else
                {
                    string yeuCau = "";
                    if(kiemTraPhieuLinhTinhTrangDuyet == true)
                    {
                        if (yeuCauLinhVatTu.Result != null)
                        {
                            if (yeuCauLinhVatTu.Result.YeuCauLinhVatTuChiTiets.Any())
                            {

                                //var vtBHYT = yeuCauLinhVatTu.Result.YeuCauLinhVatTuChiTiets
                                //                              .Select(o => new VatTuDaTaoGridVo
                                //                              {
                                //                                  MaVatTu = o.VatTuBenhVien.Ma,
                                //                                  TenVatTu = o.VatTuBenhVien.VatTus.Ten + (o.VatTuBenhVien.VatTus.NhaSanXuat != null && o.VatTuBenhVien.VatTus.NhaSanXuat != "" ? "; " + o.VatTuBenhVien.VatTus.NhaSanXuat : "") +
                                //                                                                               (o.VatTuBenhVien.VatTus.NuocSanXuat != null && o.VatTuBenhVien.VatTus.NuocSanXuat != "" ? "; " + o.VatTuBenhVien.VatTus.NuocSanXuat : ""),
                                //                                  DuocDuyet = o.YeuCauLinhVatTu.DuocDuyet,
                                //                                  SoLuong = o.SoLuong,
                                //                                  TenLoaiLinh = o.YeuCauLinhVatTu.LoaiPhieuLinh.GetDescription(),
                                //                                  DonViTinh = o.VatTuBenhVien.VatTus.DonViTinh,
                                //                                  GhiChu = o.YeuCauLinhVatTu.GhiChu,
                                //                                  YeuCau = o.SoLuong,
                                //                                  LaVatTuBHYT = o.LaVatTuBHYT,
                                //                                  MaTN = o.YeuCauVatTuBenhVien.YeuCauTiepNhanId != null ? o.YeuCauVatTuBenhVien.YeuCauTiepNhan.MaYeuCauTiepNhan :""
                                //                              })
                                //                              .GroupBy(xy => new { xy.MaVatTu, xy.TenVatTu, xy.DuocDuyet, xy.DonViTinh })
                                //                              .Select(o => new VatTuDaTaoGridVo
                                //                              {
                                //                                  MaVatTu = o.First().MaVatTu,
                                //                                  TenVatTu = o.First().TenVatTu,
                                //                                  DuocDuyet = o.First().DuocDuyet,
                                //                                  SoLuong = o.Sum(s => s.SoLuong),
                                //                                  TenLoaiLinh = o.First().TenLoaiLinh,
                                //                                  DonViTinh = o.First().DonViTinh,
                                //                                  GhiChu = o.First().GhiChu,
                                //                                  YeuCau = o.Sum(s => s.YeuCau),
                                //                                  LaVatTuBHYT =o.First().LaVatTuBHYT,
                                //                                  MaTN = o.First().MaTN,
                                //                              });
                                var objData = GetHTMLLinhBenhNhanDataoDaDuyet(yeuCauLinhVatTu.Result.YeuCauVatTuBenhViens.Where(s => s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan).ToList());
                                ThuocHoacVatTu = objData.html;
                                index = objData.Index;
                                ThuocHoacVatTu = objData.html;
                            }
                        }
                    }
                    else
                    {
                        if (yeuCauLinhVatTu.Result != null)
                        {
                            if (yeuCauLinhVatTu.Result.YeuCauVatTuBenhViens.Any(s=> s.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy))
                            {

                                //var vtBHYT = yeuCauLinhVatTu.Result.YeuCauVatTuBenhViens
                                //                              .Select(o => new VatTuDaTaoGridVo
                                //                              {
                                //                                  MaVatTu = o.VatTuBenhVien.Ma,
                                //                                  TenVatTu = o.VatTuBenhVien.VatTus.Ten + (o.VatTuBenhVien.VatTus.NhaSanXuat != null && o.VatTuBenhVien.VatTus.NhaSanXuat != "" ? "; " + o.VatTuBenhVien.VatTus.NhaSanXuat : "") +
                                //                                                                               (o.VatTuBenhVien.VatTus.NuocSanXuat != null && o.VatTuBenhVien.VatTus.NuocSanXuat != "" ? "; " + o.VatTuBenhVien.VatTus.NuocSanXuat : ""),
                                //                                  DuocDuyet = o.YeuCauLinhVatTu.DuocDuyet,
                                //                                  SoLuong = o.SoLuong,
                                //                                  TenLoaiLinh = o.LoaiPhieuLinh.GetDescription(),
                                //                                  DonViTinh = o.VatTuBenhVien.VatTus.DonViTinh,
                                //                                  GhiChu = o.GhiChu,
                                //                                  YeuCau = o.SoLuong,
                                //                              })
                                //                              .GroupBy(xy => new { xy.MaVatTu, xy.TenVatTu, xy.DuocDuyet, xy.DonViTinh })
                                //                              .Select(o => new VatTuDaTaoGridVo
                                //                              {
                                //                                  MaVatTu = o.First().MaVatTu,
                                //                                  TenVatTu = o.First().TenVatTu,
                                //                                  DuocDuyet = o.First().DuocDuyet,
                                //                                  SoLuong = o.Sum(s => s.SoLuong),
                                //                                  TenLoaiLinh = o.First().TenLoaiLinh,
                                //                                  DonViTinh = o.First().DonViTinh,
                                //                                  GhiChu = o.First().GhiChu,
                                //                                  YeuCau = o.Sum(s => s.YeuCau)
                                //                              });
                                var objData = GetHTMLLinhBenhNhanDataoDaDuyet(yeuCauLinhVatTu.Result.YeuCauVatTuBenhViens.Where(s => s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan && s.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy).ToList());
                                ThuocHoacVatTu = objData.html;
                                index = objData.Index;
                                ThuocHoacVatTu = objData.html;
                            }
                        }
                    }
                    

                    var maVachPhieuLinh = yeuCauLinhVatTu.Result.SoPhieu.ToString();
                    var data = new
                    {
                        LogoUrl = xacNhanInLinhDuocPham.Hosting + "/assets/img/logo-bacha-full.png",
                        BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauLinhVatTu.Result.SoPhieu.ToString()) ? BarcodeHelper.GenerateBarCode(maVachPhieuLinh) : "",
                        MaVachPhieuLinh = maVachPhieuLinh,
                        NoiGiao = yeuCauLinhVatTu?.Result?.KhoXuat?.Ten,
                        DienGiai = yeuCauLinhVatTu?.Result?.GhiChu,
                        TruongKhoaDuocVTYT = "",
                        NguoiGiao = yeuCauLinhVatTu?.Result?.NhanVienDuyet?.User?.HoTen,
                        NguoiNhan = yeuCauLinhVatTu.Result?.NhanVienYeuCau?.User?.HoTen,
                        TuNgay = yeuCauLinhVatTu?.Result?.ThoiDiemLinhTongHopTuNgay?.ApplyFormatDateTimeSACH(),
                        DenNgay = yeuCauLinhVatTu?.Result?.ThoiDiemLinhTongHopDenNgay?.ApplyFormatDateTimeSACH(),
                        TruongPhongKhoaPhong = "",
                        CongKhoan = index - 1,
                        NgayThangNam = DateTime.Now.ApplyFormatDateTimeSACH(),
                        //HeaderPhieuLinhThuoc = tenLoaiLinh,
                        ThuocHoacVatTu = ThuocHoacVatTu,
                        KhoaPhong = yeuCauLinhVatTu?.Result?.KhoNhap?.Ten,
                        Ngay = DateTime.Now.Day,
                        Thang = DateTime.Now.Month,
                        Nam = DateTime.Now.Year,
                        //Gio = DateTime.Now.ApplyFormatTime()
                        NoiNhan = TenNoiNhanPhieuLinhTrucTiep((long)yeuCauLinhVatTu?.Result?.NoiYeuCauId)
                    };
                    content = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data);
                    return content;
                }

            }
            else // lĩnh thường
            {
                if (yeuCauLinhVatTu.Result != null)
                {
                    var yeucau = 0; // to do
                    var thucChat = 0; // to do
                    var tenLoaiLinh = "";
                    var donViTinh = "";
                    var maHoatChat = "";
                    var ten = "";
                    var ghichu = "";
                    if (yeuCauLinhVatTu.Result.YeuCauLinhVatTuChiTiets.Where(x => x.LaVatTuBHYT == true).Any())
                    {
                        foreach (var itemx in yeuCauLinhVatTu.Result.YeuCauLinhVatTuChiTiets.Where(x => x.LaVatTuBHYT == true))
                        {
                            donViTinh = _vatTuBenhVienRepository.TableNoTracking.Where(z => z.Id == itemx.VatTuBenhVienId).Select(x => x.VatTus.DonViTinh).FirstOrDefault();
                            ten = _vatTuBenhVienRepository.TableNoTracking.Where(z => z.Id == itemx.VatTuBenhVienId)
                                .Select(s => (s.VatTus.Ten + (s.VatTus.NhaSanXuat != null && s.VatTus.NhaSanXuat != "" ? "; " + s.VatTus.NhaSanXuat : "") +
                                              (s.VatTus.NuocSanXuat != null && s.VatTus.NuocSanXuat != "" ? "; " + s.VatTus.NuocSanXuat : ""))).FirstOrDefault();

                            ThuocHoacVatTu = ThuocHoacVatTu + headerBHYT + "<tr style='border: 1px solid #020000;'>"
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
                    if (yeuCauLinhVatTu.Result.YeuCauLinhVatTuChiTiets.Where(x => x.LaVatTuBHYT == false).Any())
                    {
                        foreach (var itemx in yeuCauLinhVatTu.Result.YeuCauLinhVatTuChiTiets.Where(x => x.LaVatTuBHYT == false))
                        {
                            donViTinh = _vatTuBenhVienRepository.TableNoTracking.Where(z => z.Id == itemx.VatTuBenhVienId).Select(x => x.VatTus.DonViTinh).FirstOrDefault();
                            ten = _vatTuBenhVienRepository.TableNoTracking.Where(z => z.Id == itemx.VatTuBenhVienId)
                               .Select(s => (s.VatTus.Ten + (s.VatTus.NhaSanXuat != null && s.VatTus.NhaSanXuat != "" ? "; " + s.VatTus.NhaSanXuat : "") +
                                             (s.VatTus.NuocSanXuat != null && s.VatTus.NuocSanXuat != "" ? "; " + s.VatTus.NuocSanXuat : ""))).FirstOrDefault();

                            ThuocHoacVatTu = ThuocHoacVatTu + headerKhongBHYT + "<tr style='border: 1px solid #020000;'>"
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
                    var maVachPhieuLinh = yeuCauLinhVatTu.Result.SoPhieu.ToString();
                    var data = new
                    {
                        LogoUrl = xacNhanInLinhDuocPham.Hosting + "/assets/img/logo-bacha-full.png",
                        BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauLinhVatTu.Result.SoPhieu.ToString()) ? BarcodeHelper.GenerateBarCode(maVachPhieuLinh) : "",
                        MaVachPhieuLinh = maVachPhieuLinh,
                        NoiGiao = yeuCauLinhVatTu?.Result?.KhoXuat?.Ten,
                        DienGiai = yeuCauLinhVatTu?.Result?.GhiChu,
                        TruongKhoaDuocVTYT = "",
                        NguoiGiao = yeuCauLinhVatTu?.Result?.NhanVienDuyet?.User?.HoTen,
                        NguoiNhan = yeuCauLinhVatTu.Result?.NhanVienYeuCau?.User?.HoTen,
                        TuNgay = yeuCauLinhVatTu?.Result?.NgayYeuCau.ApplyFormatDateTimeSACH(),
                        DenNgay = yeuCauLinhVatTu?.Result?.NgayDuyet == null ? "" : yeuCauLinhVatTu?.Result?.NgayDuyet.Value.ApplyFormatDateTimeSACH(),
                        TruongPhongKhoaPhong = "",
                        CongKhoan = index - 1,
                        NgayThangNam = DateTime.Now.ApplyFormatDateTimeSACH(),
                        HeaderPhieuLinhThuoc = tenLoaiLinh,
                        ThuocHoacVatTu = ThuocHoacVatTu,
                        KhoaPhong = yeuCauLinhVatTu?.Result?.KhoNhap?.Ten,
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
        public async Task<GridDataSource> GetDataDSDuyetVatTuForGridAsync(QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                BuildDefaultSortExpression(queryInfo);
                var queryString = JsonConvert.DeserializeObject<SeachNgay>(queryInfo.AdditionalSearchString);

                //var queryDangChoGoi = DangChoGoi(queryInfo);
                var queryDangChoDuyet = DangChoDuyet(queryInfo, true);
                var queryTuChoiDuyet = TuChoiDuyet(queryInfo, true);
                var queryDaDuyet = DaDuyet(queryInfo, true);

                var query = new List<DSLinhVatTuGridVo>();

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
                    query = new List<DSLinhVatTuGridVo>();
                    query = queryDangChoDuyet.Union(queryTuChoiDuyet).Union(queryDaDuyet).ToList();
                }

                var dataOrderBy = query.AsQueryable();
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
                //var queryDangChoGui = BaseRepository.TableNoTracking.Where(x => x.DuocDuyet == null && x.DaGui != true).Select(s => new DSLinhVatTuGridVo()
                //{
                //    Id = s.Id,
                //    MaPL = s.SoPhieu,
                //    Loai = s.LoaiPhieuLinh.GetDescription(),
                //    LoaiPhieuLinh = s.LoaiPhieuLinh,
                //    NguoiYeuCau = s.NhanVienYeuCau.User.HoTen,
                //    LinhTuKho = s.KhoXuat.Ten,
                //    LinhVeKhoId = s.KhoXuatId,
                //    LinhVeKho = s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan ? "" : s.KhoNhap.Ten,
                //    NgayYeuCau = s.NgayYeuCau,
                //    TinhTrang = "Đang chờ duyệt",
                //    Nguoiduyet = s.NhanVienDuyet.User.HoTen,
                //    NgayDuyet = s.NgayDuyet != null ? s.NgayDuyet : (DateTime?)null,
                //    NgayDuyetHienThi = s.NgayDuyet != null ? Convert.ToDateTime(s.NgayDuyet).ApplyFormatDateTimeSACH() : "",
                //    DuocDuyet = s.DuocDuyet
                //});
                var queryDangChoDuyet = BaseRepository.TableNoTracking.Where(x => x.DuocDuyet == null).Select(s => new DSLinhVatTuGridVo()
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
                var queryTuChoiDuyet = BaseRepository.TableNoTracking.Where(x => x.DuocDuyet == false).Select(s => new DSLinhVatTuGridVo()
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
                var queryDaDuyet = BaseRepository.TableNoTracking.Where(x => x.DuocDuyet == true).Select(s => new DSLinhVatTuGridVo()
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
                var query = queryDangChoDuyet.Union(queryTuChoiDuyet).Union(queryDaDuyet);
                var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
                var queryTask = query.Skip(queryInfo.Skip)
                    .Take(queryInfo.Take).ToArrayAsync();

                await Task.WhenAll(countTask, queryTask);
                return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
            }

            return null;
        }

        public async Task<GridDataSource> GetDSDuyetVatTuTotalPageForGridAsync(QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<SeachNgay>(queryInfo.AdditionalSearchString);

                //var queryDangChoGoi = DangChoGoi(queryInfo);
                var queryDangChoDuyet = DangChoDuyet(queryInfo, true);
                var queryTuChoiDuyet = TuChoiDuyet(queryInfo, true);
                var queryDaDuyet = DaDuyet(queryInfo, true);

                var query = new List<DSLinhVatTuGridVo>();

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
                    query = new List<DSLinhVatTuGridVo>();
                    query = queryDangChoDuyet.Union(queryTuChoiDuyet).Union(queryDaDuyet).ToList();
                }
                var dataOrderBy = query.AsQueryable();
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
        #region Ds duyệt vatTu child
        public async Task<GridDataSource> GetDataDSDuyetLinhVatTuChildForGridAsync(QueryInfo queryInfo)
        {
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            int loaiPhieulinh = int.Parse(queryString[1]);
            EnumLoaiPhieuLinh loaiPhieu;
            if (loaiPhieulinh == 1)
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhDuTru;
            }
            else if (loaiPhieulinh == 2)
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhBu;
            }
            else
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhChoBenhNhan;
            }
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhBu)
            {
                BuildDefaultSortExpression(queryInfo);
                var trangThaiBu = _yeuCauLinhVatTuRepository.TableNoTracking.Where(d => d.Id == long.Parse(queryString[0])).Select(d => d.DuocDuyet).FirstOrDefault();
                var query = _yeuCauLinhVatTuChiTietRepository.TableNoTracking
                            .Where(p => p.YeuCauLinhVatTuId == long.Parse(queryString[0])
                                //&& p.YeuCauVatTuBenhVien.TrangThai == EnumYeuCauVatTuBenhVien.DaThucHien
                                && (p.YeuCauLinhVatTu.DuocDuyet == false ? p.YeuCauVatTuBenhVien.KhongLinhBu != false : p.YeuCauVatTuBenhVien.KhongLinhBu != true)
                            )  
                            .Select(s => new YeuCauLinhVatTuBuGridVo
                            {
                                Id = s.Id,
                                YeuCauLinhVatTuId = s.YeuCauLinhVatTuId,
                                VatTuBenhVienId = s.VatTuBenhVienId,
                                TenVatTu = s.VatTuBenhVien.VatTus.Ten,
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
                            .GroupBy(x => new { x.YeuCauLinhVatTuId, x.VatTuBenhVienId, x.LaBHYT, x.Nhom, x.DonViTinh, x.HangSanXuat, x.NuocSanXuat, x.SoLuongTon })
                              .Select(item => new YeuCauLinhVatTuBuGridVo()
                              {
                                  Id= item.FirstOrDefault().Id,
                                  VatTuBenhVienId = item.First().VatTuBenhVienId,
                                  LaBHYT = item.First().LaBHYT,
                                  TenVatTu = item.First().TenVatTu,
                                  Nhom = item.First().Nhom,
                                  DonViTinh = item.First().DonViTinh,
                                  HangSanXuat = item.First().HangSanXuat,
                                  NuocSanXuat = item.First().NuocSanXuat,
                                  SoLuongCanBu = item.Sum(x => x.SoLuongCanBu).MathRoundNumber(2),
                                  LinhveKhoId = long.Parse(queryString[2]),
                                  LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhBu,
                                  SLDaLinh = item.Sum(x => x.SLDaLinh),
                                  SoLuongYeuCauDaDuyet = item.Sum(s=>s.SoLuongYeuCauDaDuyet),
                                  DaDuyet = item.First().DaDuyet
                              })
                              .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu).Distinct();
                var VatTuLinhBuGridVos = query.ToList();

                if(trangThaiBu == null)
                {
                    var lstVatTuBenhVien = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(x => x.NhapKhoVatTu.KhoId == long.Parse(queryString[2]) &&
                     x.SoLuongDaXuat < x.SoLuongNhap).ToList();

                    var result = VatTuLinhBuGridVos.Where(p => lstVatTuBenhVien.Any(o => o.VatTuBenhVienId == p.VatTuBenhVienId && o.LaVatTuBHYT == p.LaBHYT));

                    result = result.Select(o =>
                    {
                        o.SoLuongTon = lstVatTuBenhVien.Where(t => t.VatTuBenhVienId == o.VatTuBenhVienId && t.LaVatTuBHYT == o.LaBHYT).Sum(t => t.SoLuongNhap - t.SoLuongDaXuat);
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
                        TenVatTu = s.VatTuBenhVien.VatTus.Ten,
                        DonViTinh = s.VatTuBenhVien.VatTus.DonViTinh,
                        SoLuongYc = s.SoLuong,
                        HangSanXuat = s.VatTuBenhVien.VatTus.NhaSanXuat,
                        NuocSanXuat = s.VatTuBenhVien.VatTus.NuocSanXuat,
                        BSKeToa = s.YeuCauVatTuBenhVien.NhanVienChiDinh.User.HoTen,
                        Nhom = s.LaVatTuBHYT == true ? "Vật Tư BHYT" : "Vật Tư Không BHYT",
                        SLTon = s.VatTuBenhVien.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == s.YeuCauLinhVatTu.KhoXuatId && nkct.LaVatTuBHYT == s.LaVatTuBHYT).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat).MathRoundNumber(1),
                    });
                var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
                var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                    .Take(queryInfo.Take).ToArrayAsync();

                await Task.WhenAll(countTask, queryTask);
               
                return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
            }
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhChoBenhNhan)
            {
                int trangThai = 0;
                if (queryString[3] == "true" || string.IsNullOrEmpty(queryString[1]))
                {
                    trangThai = 0;
                }// 3 Duoc duyet : dang duyet , đa duyet
                if (queryString[3] == "false" || queryString[3] == "False")
                {
                    trangThai = 1;
                } // tu choi duyet
                if (trangThai == 1)
                {
                    IQueryable<DSLinhVatChildTuGridVo> queryable = null;
                    var yeuCauLinhVatTu =
                    await _yeuCauLinhVatTuRepository.TableNoTracking.FirstAsync(x => x.Id == long.Parse(queryString[0]));
                    queryable = _yeuCauLinhVatTuChiTietRepository.TableNoTracking
                        .Where(x => x.YeuCauLinhVatTuId == long.Parse(queryString[0]))
                        .Select(item => new DSLinhVatChildTuGridVo()
                        {
                            YeuCauLinhVatTuId = long.Parse(queryString[0]),
                            VatTuBenhVienId = item.VatTuBenhVienId,
                            LaBHYT = item.LaVatTuBHYT,
                            TenVatTu = item.VatTuBenhVien.VatTus.Ten,
                            DonViTinh = item.VatTuBenhVien.VatTus.DonViTinh,
                            HangSanXuat = item.VatTuBenhVien.VatTus.NhaSanXuat,
                            NuocSanXuat = item.VatTuBenhVien.VatTus.NuocSanXuat,
                            SoLuongYeuCau = item.SoLuong,
                            Nhom = item.LaVatTuBHYT == true ? "Vật Tư BHYT" : "Vật Tư Không BHYT",
                            DichVuKham = item.YeuCauVatTuBenhVien.YeuCauKhamBenh != null ? item.YeuCauVatTuBenhVien.YeuCauKhamBenh.TenDichVu : (item.YeuCauVatTuBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh != null ? item.YeuCauVatTuBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu : "Ghi nhận trong PTTT"),
                            BacSiKeToa = item.YeuCauVatTuBenhVien.YeuCauKhamBenh != null ? item.YeuCauVatTuBenhVien.YeuCauKhamBenh.NhanVienChiDinh.User.HoTen : "",
                            BacSyKeToa = item.YeuCauVatTuBenhVien.YeuCauKhamBenh != null ? item.YeuCauVatTuBenhVien.YeuCauKhamBenh.NhanVienChiDinh.User.HoTen : "",
                            NgayDieuTri =item.YeuCauVatTuBenhVien != null ? (item.YeuCauVatTuBenhVien.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && item.YeuCauVatTuBenhVien.NoiTruPhieuDieuTriId != null) ? item.YeuCauVatTuBenhVien.NoiTruPhieuDieuTri.NgayDieuTri : (DateTime?)item.YeuCauVatTuBenhVien.ThoiDiemChiDinh : null,
                            NgayKetString = item.YeuCauVatTuBenhVien != null ? item.YeuCauVatTuBenhVien.ThoiDiemChiDinh.ApplyFormatDateTimeSACH() : "",
                            DuocDuyet = item.YeuCauLinhVatTu != null ? item.YeuCauLinhVatTu.DuocDuyet :null
                        })
                        .GroupBy(x => new
                        {
                            x.YeuCauLinhVatTuId,
                            x.VatTuBenhVienId,
                            x.LaBHYT,
                            x.Nhom,
                            x.DonViTinh,
                            x.HangSanXuat,
                            x.NuocSanXuat,
                            x.SoLuongYeuCau
                        })
                        .Select(item => new DSLinhVatChildTuGridVo()
                        {
                            YeuCauLinhVatTuId = long.Parse(queryString[0]),
                            VatTuBenhVienId = item.First().VatTuBenhVienId,
                            LaBHYT = item.First().LaBHYT,
                            TenVatTu = item.First().TenVatTu,
                            DonViTinh = item.First().DonViTinh,
                            HangSanXuat = item.First().HangSanXuat,
                            NuocSanXuat = item.First().NuocSanXuat,
                            SoLuongYeuCau = item.First().SoLuongYeuCau,
                            SoLuongTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                .Where(x => x.VatTuBenhVienId == item.First().VatTuBenhVienId
                                            && x.NhapKhoVatTu.KhoId == yeuCauLinhVatTu.KhoXuatId
                                            && x.NhapKhoVatTu.DaHet != true
                                            && x.LaVatTuBHYT == item.First().LaBHYT
                                            && x.SoLuongDaXuat < x.SoLuongNhap && x.HanSuDung >= DateTime.Now).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                            Nhom = item.First().Nhom,
                            DichVuKham = item.First().DichVuKham,
                            BacSiKeToa = item.First().BacSiKeToa,
                            BacSyKeToa = item.First().BacSyKeToa,
                            NgayDieuTri = item.First().NgayDieuTri,
                            NgayKetString = item.First().NgayKetString,
                            DuocDuyet = item.First().DuocDuyet
                        })
                        .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu).Distinct();
                    var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : queryable.CountAsync();
                    var queryTask = queryable.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                        .Take(queryInfo.Take).ToArrayAsync();
                    await Task.WhenAll(countTask, queryTask);
                    return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
                }
                else
                {
                    BuildDefaultSortExpression(queryInfo);
                    var yeuCauLinhId = long.Parse(queryString[0]);

                    // cập nhật 29/10/2021: trường hợp đã duyệt thì lấy thông tin từ YeCauLinhDuocPhamCHiTiet
                    var yeuCauLinhVatTu =
                        await _yeuCauLinhVatTuRepository.TableNoTracking.FirstAsync(x => x.Id == yeuCauLinhId);
                    IQueryable<DSLinhVatChildTuGridVo> query = null;

                    if (yeuCauLinhVatTu.DuocDuyet != true)
                    {
                        query = _yeuCauVatTuBenhVienRepository.TableNoTracking
                        .Where(o => o.YeuCauLinhVatTuId == yeuCauLinhId
                                    && o.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan
                                    && o.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy
                                    && o.YeuCauTiepNhan.BenhNhanId != null)
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
                            KhoLinhId = s.KhoLinhId,
                            
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
                            Id = yeuCauLinhId,
                            DuocDuyet = s.First().DuocDuyet,
                            YeuCauTiepNhanId = s.First().YeuCauTiepNhanId,
                            NgayDieuTri = s.First().NgayDieuTri,
                            KhoLinhId = s.First().KhoLinhId
                        });
                    }
                    else
                    {
                        query = _yeuCauLinhVatTuChiTietRepository.TableNoTracking
                            .Where(x => x.YeuCauLinhVatTuId == yeuCauLinhId
                                        && x.YeuCauVatTuBenhVien.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan
                                        && x.YeuCauVatTuBenhVien.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy
                                        && x.YeuCauVatTuBenhVien.YeuCauTiepNhan.BenhNhanId != null)
                            .OrderBy(x => x.YeuCauVatTuBenhVien.ThoiDiemChiDinh)
                            .Select(s => new DSLinhVatChildTuGridVo
                            {
                                BenhNhanId = s.YeuCauVatTuBenhVien.YeuCauTiepNhan.BenhNhanId.Value,
                                MaYeuCauTiepNhan = s.YeuCauVatTuBenhVien.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                MaBenhNhan = s.YeuCauVatTuBenhVien.YeuCauTiepNhan.BenhNhan.MaBN,
                                HoTen = s.YeuCauVatTuBenhVien.YeuCauTiepNhan.HoTen,
                                SoLuong = s.SoLuong,
                                DichVuKham = s.YeuCauVatTuBenhVien.YeuCauKhamBenh != null 
                                    ? s.YeuCauVatTuBenhVien.YeuCauKhamBenh.TenDichVu 
                                    : (s.YeuCauVatTuBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh != null ? s.YeuCauVatTuBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu : ""),
                                BacSiKeToa = s.YeuCauVatTuBenhVien.NhanVienChiDinh.User.HoTen,
                                NgayKe = s.YeuCauVatTuBenhVien.ThoiDiemChiDinh,
                                DuocDuyet = s.YeuCauLinhVatTu.DuocDuyet,
                                YeuCauTiepNhanId = s.YeuCauVatTuBenhVien.YeuCauTiepNhanId,
                                NgayDieuTri = (s.YeuCauVatTuBenhVien.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && s.YeuCauVatTuBenhVien.NoiTruPhieuDieuTriId != null)
                                    ? s.YeuCauVatTuBenhVien.NoiTruPhieuDieuTri.NgayDieuTri : s.YeuCauVatTuBenhVien.ThoiDiemChiDinh,
                                KhoLinhId = s.YeuCauVatTuBenhVien.KhoLinhId
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
                                Id = yeuCauLinhId,
                                DuocDuyet = s.First().DuocDuyet,
                                YeuCauTiepNhanId = s.First().YeuCauTiepNhanId,
                                NgayDieuTri = s.First().NgayDieuTri,
                                KhoLinhId = s.First().KhoLinhId
                            });
                    }

                    var daGuis = _yeuCauLinhVatTuRepository.TableNoTracking.Where(d => d.Id == yeuCauLinhId).Select(d => d.DaGui);
                    if (daGuis.Any())
                    {
                        if(daGuis.LastOrDefault() == false)
                        {   
                            if(query.Any())
                            {
                                var dataGoi = DataChoGoi((long)query.Select(d => d.KhoLinhId).First()).AsQueryable();
                                query = query.Union(dataGoi);
                            }
                        }
                    }
                    var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
                    var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                        .Take(queryInfo.Take).ToArrayAsync();
                    await Task.WhenAll(countTask, queryTask);
                    return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
                }
            }
            return null;
        }
        public async Task<GridDataSource> GetDataDSDuyetLinhVatTuChildChildForGridAsync(QueryInfo queryInfo)
        {
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            int loaiPhieulinh = int.Parse(queryString[1]);
            EnumLoaiPhieuLinh loaiPhieu;
            if (loaiPhieulinh == 1)
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhDuTru;
            }
            else if (loaiPhieulinh == 2)
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhBu;
            }
            else
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhChoBenhNhan;
            }
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhBu)
            {
                BuildDefaultSortExpression(queryInfo);
                //var query = _yeuCauVatTuBenhVienRepository.TableNoTracking
                //.Where(o => o.VatTuBenhVienId == long.Parse(queryString[2])
                //            && o.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu
                //            //&& o.LaVatTuBHYT == bool.Parse(queryString[3])
                //            //&& o.KhoLinhId == long.Parse(queryString[4])
                //            && o.KhongLinhBu != true
                //            && o.YeuCauLinhVatTuId == null
                //            && ( o.SoLuongDaLinhBu == null || o.SoLuongDaLinhBu < o.SoLuong))
                var trangThai = _yeuCauLinhVatTuChiTietRepository.TableNoTracking.Where(x => x.Id == long.Parse(queryString[0])).Select(s => s.YeuCauLinhVatTu.DuocDuyet).FirstOrDefault();
                var yeuCauLinhVatTuId = _yeuCauLinhVatTuChiTietRepository.TableNoTracking.Where(x => x.Id == long.Parse(queryString[0])).Select(s => s.YeuCauLinhVatTuId).FirstOrDefault();
                var query = _yeuCauLinhVatTuChiTietRepository.TableNoTracking
                         .Where(p => p.YeuCauLinhVatTuId == yeuCauLinhVatTuId
                            && p.YeuCauLinhVatTu.DuocDuyet == trangThai
                             && p.VatTuBenhVienId == long.Parse(queryString[2])
                             && p.LaVatTuBHYT == bool.Parse(queryString[3])
                             && p.YeuCauVatTuBenhVien.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhBu
                         //&& (p.YeuCauLinhVatTu.DuocDuyet == false ? p.YeuCauVatTuBenhVien.KhongLinhBu != false : p.YeuCauVatTuBenhVien.KhongLinhBu != true)
                         )
             .OrderBy(x => x.YeuCauVatTuBenhVien.ThoiDiemChiDinh)
                .Select(s => new VatTuLinhBuCuaBNGridVos
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
                        TenVatTu = s.VatTuBenhVien.VatTus.Ten,
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
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhChoBenhNhan)
            {
                BuildDefaultSortExpression(queryInfo);
                int kieuIn = 0;
                if (queryString[6] == "" || queryString[6] == "true")
                {
                    kieuIn = 0;
                }
                if (queryString[6] == "false" || queryString[6] == "False")
                {
                    kieuIn = 1;
                }
                List<DSLinhVatChildTuGridVo> queryable = new List<DSLinhVatChildTuGridVo>();
                var yeuCau =
                 _yeuCauLinhVatTuRepository.TableNoTracking.Any(x => x.Id == long.Parse(queryString[0]));
              
                if (kieuIn == 0)
                {
                    if(yeuCau == true)
                    {
                        var yeuCauLinhVatTu =
                        await _yeuCauLinhVatTuRepository.TableNoTracking.FirstAsync(x => x.Id == long.Parse(queryString[0]));

                        // cập nhật 08/11/2021: trường hợp đã duyệt thì lấy thông tin từ YeCauLinhVatTuCHiTiet
                        if (yeuCauLinhVatTu.DuocDuyet != true)
                        {
                            queryable = _yeuCauVatTuBenhVienRepository.TableNoTracking
                           .Where(x => x.YeuCauLinhVatTuId == long.Parse(queryString[0])
                                       && x.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan
                                       && x.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy
                                       && x.YeuCauTiepNhanId == long.Parse(queryString[5]))
                           .Select(item => new DSLinhVatChildTuGridVo()
                           {
                               YeuCauLinhVatTuId = long.Parse(queryString[0]),
                               VatTuBenhVienId = item.VatTuBenhVienId,
                               LaBHYT = item.LaVatTuBHYT,
                               TenVatTu = item.Ten,
                               DonViTinh = item.VatTuBenhVien.VatTus.DonViTinh,
                               HangSanXuat = item.VatTuBenhVien.VatTus.NhaSanXuat,
                               NuocSanXuat = item.VatTuBenhVien.VatTus.NuocSanXuat,
                               SoLuongYeuCau = item.SoLuong,

                               DichVuKham = item.YeuCauKhamBenh != null ? item.YeuCauKhamBenh.TenDichVu : (item.YeuCauDichVuKyThuat.YeuCauKhamBenh != null ? item.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu : "Ghi nhận trong PTTT"),
                               BacSiKeToa = item.NhanVienChiDinh.User.HoTen,
                               NgayKe = item.ThoiDiemChiDinh,
                               Nhom = item.LaVatTuBHYT == true ? "Vật Tư BHYT" : "Vật Tư Không BHYT",
                               NgayDieuTri = item.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && item.NoiTruPhieuDieuTriId != null ? item.NoiTruPhieuDieuTri.NgayDieuTri : item.ThoiDiemChiDinh
                           })
                           .GroupBy(x => new
                           {
                               x.YeuCauLinhVatTuId,
                               x.VatTuBenhVienId,
                               x.LaBHYT,
                               x.Nhom,
                               x.DonViTinh,
                               x.HangSanXuat,
                               x.NuocSanXuat,
                               x.DichVuKham,
                               x.BacSiKeToa,
                               x.NgayKe
                           })
                           .Select(item => new DSLinhVatChildTuGridVo()
                           {
                               YeuCauLinhVatTuId = long.Parse(queryString[0]),
                               VatTuBenhVienId = item.First().VatTuBenhVienId,
                               LaBHYT = item.First().LaBHYT,
                               TenVatTu = item.First().TenVatTu,
                               DonViTinh = item.First().DonViTinh,
                               HangSanXuat = item.First().HangSanXuat,
                               NuocSanXuat = item.First().NuocSanXuat,
                               SoLuongYeuCau = item.Sum(x => x.SoLuongYeuCau),
                               SoLuongTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                   .Where(x => x.VatTuBenhVienId == item.First().VatTuBenhVienId
                                               && x.NhapKhoVatTu.KhoId == yeuCauLinhVatTu.KhoXuatId
                                               && x.LaVatTuBHYT == item.First().LaBHYT
                                               && x.NhapKhoVatTu.DaHet != true
                                               && x.HanSuDung >= DateTime.Now).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                               DichVuKham = item.First().DichVuKham,
                               BacSiKeToa = item.First().BacSiKeToa,
                               NgayKe = item.First().NgayKe,
                               Nhom = item.First().Nhom,
                               NgayDieuTri = item.First().NgayDieuTri
                           })
                           .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu).Distinct().ToList();
                        }
                        else
                        {
                            queryable = _yeuCauLinhVatTuChiTietRepository.TableNoTracking
                                .Where(x => x.YeuCauLinhVatTuId == long.Parse(queryString[0])
                                            && x.YeuCauVatTuBenhVien.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan
                                            && x.YeuCauVatTuBenhVien.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy
                                            && x.YeuCauVatTuBenhVien.YeuCauTiepNhanId == long.Parse(queryString[5]))
                                .Select(item => new DSLinhVatChildTuGridVo()
                                {
                                    YeuCauLinhVatTuId = long.Parse(queryString[0]),
                                    VatTuBenhVienId = item.VatTuBenhVienId,
                                    LaBHYT = item.LaVatTuBHYT,
                                    TenVatTu = item.YeuCauVatTuBenhVien.Ten,
                                    DonViTinh = item.VatTuBenhVien.VatTus.DonViTinh,
                                    HangSanXuat = item.VatTuBenhVien.VatTus.NhaSanXuat,
                                    NuocSanXuat = item.VatTuBenhVien.VatTus.NuocSanXuat,
                                    SoLuongYeuCau = item.SoLuong,

                                    DichVuKham = item.YeuCauVatTuBenhVien.YeuCauKhamBenh != null 
                                        ? item.YeuCauVatTuBenhVien.YeuCauKhamBenh.TenDichVu 
                                        : (item.YeuCauVatTuBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh != null ? item.YeuCauVatTuBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu : "Ghi nhận trong PTTT"),
                                    BacSiKeToa = item.YeuCauVatTuBenhVien.NhanVienChiDinh.User.HoTen,
                                    NgayKe = item.YeuCauVatTuBenhVien.ThoiDiemChiDinh,
                                    Nhom = item.LaVatTuBHYT == true ? "Vật Tư BHYT" : "Vật Tư Không BHYT",
                                    NgayDieuTri = (item.YeuCauVatTuBenhVien.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru  && item.YeuCauVatTuBenhVien.NoiTruPhieuDieuTriId != null)
                                        ? item.YeuCauVatTuBenhVien.NoiTruPhieuDieuTri.NgayDieuTri : item.YeuCauVatTuBenhVien.ThoiDiemChiDinh
                                })
                           .GroupBy(x => new
                           {
                               x.YeuCauLinhVatTuId,
                               x.VatTuBenhVienId,
                               x.LaBHYT,
                               x.Nhom,
                               x.DonViTinh,
                               x.HangSanXuat,
                               x.NuocSanXuat,
                               x.DichVuKham,
                               x.BacSiKeToa,
                               x.NgayKe
                           })
                           .Select(item => new DSLinhVatChildTuGridVo()
                           {
                               YeuCauLinhVatTuId = long.Parse(queryString[0]),
                               VatTuBenhVienId = item.First().VatTuBenhVienId,
                               LaBHYT = item.First().LaBHYT,
                               TenVatTu = item.First().TenVatTu,
                               DonViTinh = item.First().DonViTinh,
                               HangSanXuat = item.First().HangSanXuat,
                               NuocSanXuat = item.First().NuocSanXuat,
                               SoLuongYeuCau = item.Sum(x => x.SoLuongYeuCau),
                               SoLuongTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                   .Where(x => x.VatTuBenhVienId == item.First().VatTuBenhVienId
                                               && x.NhapKhoVatTu.KhoId == yeuCauLinhVatTu.KhoXuatId
                                               && x.LaVatTuBHYT == item.First().LaBHYT
                                               && x.NhapKhoVatTu.DaHet != true
                                               && x.HanSuDung >= DateTime.Now).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                               DichVuKham = item.First().DichVuKham,
                               BacSiKeToa = item.First().BacSiKeToa,
                               NgayKe = item.First().NgayKe,
                               Nhom = item.First().Nhom,
                               NgayDieuTri = item.First().NgayDieuTri
                           })
                           .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu).Distinct().ToList();
                        }
                    }
                    else
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

                        var yeuCauVatTu = _yeuCauVatTuBenhVienRepository.TableNoTracking
                                                 .Where(x => x.KhoLinhId == long.Parse(queryString[7]) &&
                                                  x.YeuCauLinhVatTuId == null &&
                                                  x.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 &&
                                                  phongLinhVes.Any(o => o.KeyId == x.NoiChiDinhId) &&
                                                  x.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy
                                                  && x.YeuCauTiepNhanId == long.Parse(queryString[5]))
                       .Select(item => new DSLinhVatChildTuGridVo()
                       {
                           YeuCauLinhVatTuId = long.Parse(queryString[0]),
                           VatTuBenhVienId = item.VatTuBenhVienId,
                           LaBHYT = item.LaVatTuBHYT,
                           TenVatTu = item.Ten,
                           DonViTinh = item.VatTuBenhVien.VatTus.DonViTinh,
                           HangSanXuat = item.VatTuBenhVien.VatTus.NhaSanXuat,
                           NuocSanXuat = item.VatTuBenhVien.VatTus.NuocSanXuat,
                           SoLuongYeuCau = item.SoLuong,

                           DichVuKham = item.YeuCauKhamBenh != null ? item.YeuCauKhamBenh.TenDichVu : (item.YeuCauDichVuKyThuat.YeuCauKhamBenh != null ? item.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu : ""),
                           BacSiKeToa = item.NhanVienChiDinh.User.HoTen,
                           NgayKe = item.ThoiDiemChiDinh,
                           Nhom = item.LaVatTuBHYT == true ? "Vật Tư BHYT" : "Vật Tư Không BHYT",
                           NgayDieuTri = (item.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru  && item.NoiTruPhieuDieuTriId != null)? item.NoiTruPhieuDieuTri.NgayDieuTri : item.ThoiDiemChiDinh,
                           SoLuongTon = item.VatTuBenhVien.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == item.KhoLinhId && nkct.LaVatTuBHYT == item.LaVatTuBHYT && nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat),
                       })
                       .GroupBy(x => new
                       {
                           x.YeuCauLinhVatTuId,
                           x.VatTuBenhVienId,
                           x.LaBHYT,
                           x.Nhom,
                           x.DonViTinh,
                           x.HangSanXuat,
                           x.NuocSanXuat,
                           x.DichVuKham,
                           x.BacSiKeToa,
                           x.NgayKe
                       })
                       .Select(item => new DSLinhVatChildTuGridVo()
                       {
                           YeuCauLinhVatTuId = long.Parse(queryString[0]),
                           VatTuBenhVienId = item.First().VatTuBenhVienId,
                           LaBHYT = item.First().LaBHYT,
                           TenVatTu = item.First().TenVatTu,
                           DonViTinh = item.First().DonViTinh,
                           HangSanXuat = item.First().HangSanXuat,
                           NuocSanXuat = item.First().NuocSanXuat,
                           SoLuongYeuCau = item.Sum(x => x.SoLuongYeuCau),
                           SoLuongTon = item.First().SoLuongTon,
                           DichVuKham = item.First().DichVuKham,
                           BacSiKeToa = item.First().BacSiKeToa,
                           NgayKe = item.First().NgayKe,
                           Nhom = item.First().Nhom,
                           NgayDieuTri = item.First().NgayDieuTri
                       })
                       .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu).Distinct().ToList();
                        if(yeuCauVatTu.Any())
                        {
                            queryable = queryable.Union(yeuCauVatTu).ToList();
                        }
                    }
                }
                else
                {
                    var yeuCauLinhVatTu =
                        await _yeuCauLinhVatTuRepository.TableNoTracking.FirstAsync(x => x.Id == long.Parse(queryString[0]));
                    queryable = _yeuCauVatTuBenhVienRepository.TableNoTracking
                        .Where(x => x.YeuCauLinhVatTuId == long.Parse(queryString[0]))
                        .Select(item => new DSLinhVatChildTuGridVo()
                        {
                            YeuCauLinhVatTuId = long.Parse(queryString[0]),
                            VatTuBenhVienId = item.VatTuBenhVienId,
                            LaBHYT = item.LaVatTuBHYT,
                            TenVatTu = item.VatTuBenhVien.VatTus.Ten,
                            DonViTinh = item.VatTuBenhVien.VatTus.DonViTinh,
                            HangSanXuat = item.VatTuBenhVien.VatTus.NhaSanXuat,
                            NuocSanXuat = item.VatTuBenhVien.VatTus.NuocSanXuat,
                            SoLuongYeuCau = item.SoLuong,
                            Nhom = item.LaVatTuBHYT == true ? "Vật Tư BHYT" : "Vật Tư Không BHYT",
                        })
                        .GroupBy(x => new
                        {
                            x.YeuCauLinhVatTuId,
                            x.VatTuBenhVienId,
                            x.LaBHYT,
                            x.Nhom,
                            x.DonViTinh,
                            x.HangSanXuat,
                            x.NuocSanXuat,
                            x.SoLuongYeuCau
                        })
                        .Select(item => new DSLinhVatChildTuGridVo()
                        {
                            YeuCauLinhVatTuId = long.Parse(queryString[0]),
                            VatTuBenhVienId = item.First().VatTuBenhVienId,
                            LaBHYT = item.First().LaBHYT,
                            TenVatTu = item.First().TenVatTu,
                            DonViTinh = item.First().DonViTinh,
                            HangSanXuat = item.First().HangSanXuat,
                            NuocSanXuat = item.First().NuocSanXuat,
                            SoLuongYeuCau = item.First().SoLuongYeuCau,
                            SoLuongTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                .Where(x => x.VatTuBenhVienId == item.First().VatTuBenhVienId
                                            && x.NhapKhoVatTu.KhoId == yeuCauLinhVatTu.KhoXuatId
                                            && x.NhapKhoVatTu.DaHet != true
                                            && x.LaVatTuBHYT == item.First().LaBHYT
                                            && x.HanSuDung >= DateTime.Now).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                            Nhom = item.First().Nhom
                        })
                        .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu).Distinct().ToList();
                }
                var dataOrderBy = queryable.AsQueryable().OrderBy(queryInfo.SortString);
                var data = dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
                var countTask = dataOrderBy.Count();

                return new GridDataSource { Data = data, TotalRowCount = countTask };
            }
            return null;
        }
        public async Task<GridDataSource> GetTotalPageFDSLinhVatTuChildForGridAsync(QueryInfo queryInfo)
        {
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            int loaiPhieulinh = int.Parse(queryString[1]);
            EnumLoaiPhieuLinh loaiPhieu;
            if (loaiPhieulinh == 1)
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhDuTru;
            }
            else if (loaiPhieulinh == 2)
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhBu;
            }
            else
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhChoBenhNhan;
            }
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhBu)
            {
                var trangThaiBu = _yeuCauLinhVatTuRepository.TableNoTracking.Where(d => d.Id == long.Parse(queryString[0])).Select(d => d.DuocDuyet).FirstOrDefault();
                var query = _yeuCauLinhVatTuChiTietRepository.TableNoTracking
                          .Where(p => p.YeuCauLinhVatTuId == long.Parse(queryString[0])
                              //&& p.YeuCauVatTuBenhVien.TrangThai == EnumYeuCauVatTuBenhVien.DaThucHien
                              && (p.YeuCauLinhVatTu.DuocDuyet == false ? p.YeuCauVatTuBenhVien.KhongLinhBu != false : p.YeuCauVatTuBenhVien.KhongLinhBu != true)
                          )
                          .Select(s => new YeuCauLinhVatTuBuGridVo
                          {
                              Id = s.Id,
                              YeuCauLinhVatTuId = s.YeuCauLinhVatTuId,
                              VatTuBenhVienId = s.VatTuBenhVienId,
                              TenVatTu = s.VatTuBenhVien.VatTus.Ten,
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
                          .GroupBy(x => new { x.YeuCauLinhVatTuId, x.VatTuBenhVienId, x.LaBHYT, x.Nhom, x.DonViTinh, x.HangSanXuat, x.NuocSanXuat, x.SoLuongTon })
                            .Select(item => new YeuCauLinhVatTuBuGridVo()
                            {
                                Id = item.FirstOrDefault().Id,
                                VatTuBenhVienId = item.First().VatTuBenhVienId,
                                LaBHYT = item.First().LaBHYT,
                                TenVatTu = item.First().TenVatTu,
                                Nhom = item.First().Nhom,
                                DonViTinh = item.First().DonViTinh,
                                HangSanXuat = item.First().HangSanXuat,
                                NuocSanXuat = item.First().NuocSanXuat,
                                SoLuongCanBu = item.Sum(x => x.SoLuongCanBu).MathRoundNumber(2),
                                LinhveKhoId = long.Parse(queryString[2]),
                                LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhBu,
                                SLDaLinh = item.Sum(x => x.SLDaLinh),
                                SoLuongYeuCauDaDuyet = item.Sum(s => s.SoLuongYeuCauDaDuyet),
                                DaDuyet = item.First().DaDuyet
                            })
                            .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu).Distinct();
                var VatTuLinhBuGridVos = query.ToList();
                if(trangThaiBu == null)
                {
                    var lstVatTuBenhVien = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(x => x.NhapKhoVatTu.KhoId == long.Parse(queryString[2])
                       && x.SoLuongDaXuat < x.SoLuongNhap).ToList();

                    var result = VatTuLinhBuGridVos.Where(p => lstVatTuBenhVien.Any(o => o.VatTuBenhVienId == p.VatTuBenhVienId && o.LaVatTuBHYT == p.LaBHYT));

                    result = result.Select(o =>
                    {
                        o.SoLuongTon = lstVatTuBenhVien.Where(t => t.VatTuBenhVienId == o.VatTuBenhVienId && t.LaVatTuBHYT == o.LaBHYT).Sum(t => t.SoLuongNhap - t.SoLuongDaXuat);
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
                        TenVatTu = s.VatTuBenhVien.VatTus.Ten,
                        DonViTinh = s.VatTuBenhVien.VatTus.DonViTinh,
                        SoLuongYc = s.SoLuong,
                        HangSanXuat = s.VatTuBenhVien.VatTus.NhaSanXuat,
                        NuocSanXuat = s.VatTuBenhVien.VatTus.NuocSanXuat,
                        BSKeToa = s.YeuCauVatTuBenhVien.NhanVienChiDinh.User.HoTen,
                        Nhom = s.LaVatTuBHYT == true ? "BHYT" : "Không BHYT",
                        SLTon = s.VatTuBenhVien.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == s.YeuCauLinhVatTu.KhoXuatId && nkct.LaVatTuBHYT == s.LaVatTuBHYT).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat).MathRoundNumber(1),
                    });
                var countTask = query.CountAsync();
                await Task.WhenAll(countTask);

                return new GridDataSource { TotalRowCount = countTask.Result };
            }
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhChoBenhNhan)
            {
                int trangThai = 0;
                if (queryString[3] == "true" || string.IsNullOrEmpty(queryString[1]))
                {
                    trangThai = 0;
                }// 3 Duoc duyet : dang duyet , đa duyet
                if (queryString[3] == "false" || queryString[3] == "False")
                {
                    trangThai = 1;
                } // tu choi duyet
                if (trangThai == 1)
                {
                    IQueryable<DSLinhVatChildTuGridVo> queryable = null;
                    var yeuCauLinhVatTu =
                    await _yeuCauLinhVatTuRepository.TableNoTracking.FirstAsync(x => x.Id == long.Parse(queryString[0]));
                    queryable = _yeuCauLinhVatTuChiTietRepository.TableNoTracking
                        .Where(x => x.YeuCauLinhVatTuId == long.Parse(queryString[0]))
                        .Select(item => new DSLinhVatChildTuGridVo()
                        {
                            YeuCauLinhVatTuId = long.Parse(queryString[0]),
                            VatTuBenhVienId = item.VatTuBenhVienId,
                            LaBHYT = item.LaVatTuBHYT,
                            TenVatTu = item.VatTuBenhVien.VatTus.Ten,
                            DonViTinh = item.VatTuBenhVien.VatTus.DonViTinh,
                            HangSanXuat = item.VatTuBenhVien.VatTus.NhaSanXuat,
                            NuocSanXuat = item.VatTuBenhVien.VatTus.NuocSanXuat,
                            SoLuongYeuCau = item.SoLuong,
                            Nhom = item.LaVatTuBHYT == true ? "Vật Tư BHYT" : "Vật Tư Không BHYT",
                            DichVuKham = item.YeuCauVatTuBenhVien.YeuCauKhamBenh != null ? item.YeuCauVatTuBenhVien.YeuCauKhamBenh.TenDichVu : (item.YeuCauVatTuBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh != null ? item.YeuCauVatTuBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu : "Ghi nhận trong PTTT"),
                            BacSiKeToa = item.YeuCauVatTuBenhVien.YeuCauKhamBenh != null ? item.YeuCauVatTuBenhVien.YeuCauKhamBenh.NhanVienChiDinh.User.HoTen : "",
                            BacSyKeToa = item.YeuCauVatTuBenhVien.YeuCauKhamBenh != null ? item.YeuCauVatTuBenhVien.YeuCauKhamBenh.NhanVienChiDinh.User.HoTen : ""
                        })
                        .GroupBy(x => new
                        {
                            x.YeuCauLinhVatTuId,
                            x.VatTuBenhVienId,
                            x.LaBHYT,
                            x.Nhom,
                            x.DonViTinh,
                            x.HangSanXuat,
                            x.NuocSanXuat,
                            x.SoLuongYeuCau
                        })
                        .Select(item => new DSLinhVatChildTuGridVo()
                        {
                            YeuCauLinhVatTuId = long.Parse(queryString[0]),
                            VatTuBenhVienId = item.First().VatTuBenhVienId,
                            LaBHYT = item.First().LaBHYT,
                            TenVatTu = item.First().TenVatTu,
                            DonViTinh = item.First().DonViTinh,
                            HangSanXuat = item.First().HangSanXuat,
                            NuocSanXuat = item.First().NuocSanXuat,
                            SoLuongYeuCau = item.First().SoLuongYeuCau,
                            SoLuongTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                .Where(x => x.VatTuBenhVienId == item.First().VatTuBenhVienId
                                            && x.NhapKhoVatTu.KhoId == yeuCauLinhVatTu.KhoXuatId
                                            && x.NhapKhoVatTu.DaHet != true
                                            && x.LaVatTuBHYT == item.First().LaBHYT
                                            && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                            Nhom = item.First().Nhom,
                            DichVuKham = item.First().DichVuKham,
                            BacSiKeToa = item.First().BacSiKeToa,
                            BacSyKeToa = item.First().BacSyKeToa
                        })
                        .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu).Distinct();
                    var countTask = queryable.CountAsync();
                    await Task.WhenAll(countTask);

                    return new GridDataSource { TotalRowCount = countTask.Result };
                }
                else
                {
                    var yeuCauLinhId = long.Parse(queryString[0]);

                    var query = _yeuCauVatTuBenhVienRepository.TableNoTracking
                    .Where(o => o.YeuCauLinhVatTuId == yeuCauLinhId
                                && o.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan
                                && o.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy
                                && o.YeuCauTiepNhan.BenhNhanId != null)
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
                        NgayDieuTri = (s.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && s.NoiTruPhieuDieuTriId != null)? s.NoiTruPhieuDieuTri.NgayDieuTri : s.ThoiDiemChiDinh,
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
                        Id = yeuCauLinhId,
                        DuocDuyet = s.First().DuocDuyet,
                        YeuCauTiepNhanId = s.First().YeuCauTiepNhanId,
                        NgayDieuTri = s.First().NgayDieuTri,
                        KhoLinhId = s.First().KhoLinhId
                    });
                    var daGuis = _yeuCauLinhVatTuRepository.TableNoTracking.Where(d => d.Id == yeuCauLinhId).Select(d => d.DaGui);
                    if (daGuis.Any())
                    {
                        if (daGuis.LastOrDefault() == false)
                        {
                            if (query.Any())
                            {
                                var dataGoi = DataChoGoi((long)query.Select(d => d.KhoLinhId).First()).AsQueryable();
                                query = query.Union(dataGoi);
                            }
                        }
                    }
                    var countTask = query.CountAsync();
                    await Task.WhenAll(countTask);

                    return new GridDataSource { TotalRowCount = countTask.Result };
                }
            }
            return null;
        }
        public async Task<GridDataSource> GetTotalPageFDSLinhVatTuChildChildForGridAsync(QueryInfo queryInfo)
        {
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            int loaiPhieulinh = int.Parse(queryString[1]);
            EnumLoaiPhieuLinh loaiPhieu;
            if (loaiPhieulinh == 1)
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhDuTru;
            }
            else if (loaiPhieulinh == 2)
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhBu;
            }
            else
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhChoBenhNhan;
            }
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhBu)
            {
                var trangThai = _yeuCauLinhVatTuChiTietRepository.TableNoTracking.Where(x => x.Id == long.Parse(queryString[0])).Select(s => s.YeuCauLinhVatTu.DuocDuyet).FirstOrDefault();
                var yeuCauLinhVatTuId = _yeuCauLinhVatTuChiTietRepository.TableNoTracking.Where(x => x.Id == long.Parse(queryString[0])).Select(s => s.YeuCauLinhVatTuId).FirstOrDefault();
                var query = _yeuCauLinhVatTuChiTietRepository.TableNoTracking
                         .Where(p => p.YeuCauLinhVatTuId == yeuCauLinhVatTuId
                            && p.YeuCauLinhVatTu.DuocDuyet == trangThai
                             && p.VatTuBenhVienId == long.Parse(queryString[2])
                             && p.LaVatTuBHYT == bool.Parse(queryString[3])
                             && p.YeuCauVatTuBenhVien.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhBu
                         //&& (p.YeuCauLinhVatTu.DuocDuyet == false ? p.YeuCauVatTuBenhVien.KhongLinhBu != false : p.YeuCauVatTuBenhVien.KhongLinhBu != true)
                         )
             .OrderBy(x => x.YeuCauVatTuBenhVien.ThoiDiemChiDinh)
              .Select(s => new VatTuLinhBuCuaBNGridVos
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
                        TenVatTu = s.VatTuBenhVien.VatTus.Ten,
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
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhChoBenhNhan)
            {
                int kieuIn = 0;
                if (queryString[6] == "" || queryString[6] == "true")
                {
                    kieuIn = 0;
                }
                if (queryString[6] == "false" || queryString[6] == "False")
                {
                    kieuIn = 1;
                }
                List<DSLinhVatChildTuGridVo> queryable = new List<DSLinhVatChildTuGridVo>();
                var yeuCau =
                 _yeuCauLinhVatTuRepository.TableNoTracking.Any(x => x.Id == long.Parse(queryString[0]));

                if (kieuIn == 0)
                {
                    if (yeuCau == true)
                    {
                        var yeuCauLinhVatTu =
                        await _yeuCauLinhVatTuRepository.TableNoTracking.FirstAsync(x => x.Id == long.Parse(queryString[0]));
                        queryable = _yeuCauVatTuBenhVienRepository.TableNoTracking
                       .Where(x => x.YeuCauLinhVatTuId == long.Parse(queryString[0])
                                   && x.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan
                                   && x.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy
                                   && x.YeuCauTiepNhanId == long.Parse(queryString[5]))
                       .Select(item => new DSLinhVatChildTuGridVo()
                       {
                           YeuCauLinhVatTuId = long.Parse(queryString[0]),
                           VatTuBenhVienId = item.VatTuBenhVienId,
                           LaBHYT = item.LaVatTuBHYT,
                           TenVatTu = item.Ten,
                           DonViTinh = item.VatTuBenhVien.VatTus.DonViTinh,
                           HangSanXuat = item.VatTuBenhVien.VatTus.NhaSanXuat,
                           NuocSanXuat = item.VatTuBenhVien.VatTus.NuocSanXuat,
                           SoLuongYeuCau = item.SoLuong,

                           DichVuKham = item.YeuCauKhamBenh != null ? item.YeuCauKhamBenh.TenDichVu : (item.YeuCauDichVuKyThuat.YeuCauKhamBenh != null ? item.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu : ""),
                           BacSiKeToa = item.NhanVienChiDinh.User.HoTen,
                           NgayKe = item.ThoiDiemChiDinh,
                           Nhom = item.LaVatTuBHYT == true ? "Vật Tư BHYT" : "Vật Tư Không BHYT",
                           NgayDieuTri = (item.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && item.NoiTruPhieuDieuTriId != null) ? item.NoiTruPhieuDieuTri.NgayDieuTri : item.ThoiDiemChiDinh
                       })
                       .GroupBy(x => new
                       {
                           x.YeuCauLinhVatTuId,
                           x.VatTuBenhVienId,
                           x.LaBHYT,
                           x.Nhom,
                           x.DonViTinh,
                           x.HangSanXuat,
                           x.NuocSanXuat,
                           x.DichVuKham,
                           x.BacSiKeToa,
                           x.NgayKe
                       })
                       .Select(item => new DSLinhVatChildTuGridVo()
                       {
                           YeuCauLinhVatTuId = long.Parse(queryString[0]),
                           VatTuBenhVienId = item.First().VatTuBenhVienId,
                           LaBHYT = item.First().LaBHYT,
                           TenVatTu = item.First().TenVatTu,
                           DonViTinh = item.First().DonViTinh,
                           HangSanXuat = item.First().HangSanXuat,
                           NuocSanXuat = item.First().NuocSanXuat,
                           SoLuongYeuCau = item.Sum(x => x.SoLuongYeuCau),
                           SoLuongTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                               .Where(x => x.VatTuBenhVienId == item.First().VatTuBenhVienId
                                           && x.NhapKhoVatTu.KhoId == yeuCauLinhVatTu.KhoXuatId
                                           && x.LaVatTuBHYT == item.First().LaBHYT
                                           && x.NhapKhoVatTu.DaHet != true
                                           && x.SoLuongDaXuat < x.SoLuongNhap && x.HanSuDung >= DateTime.Now).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                           DichVuKham = item.First().DichVuKham,
                           BacSiKeToa = item.First().BacSiKeToa,
                           NgayKe = item.First().NgayKe,
                           Nhom = item.First().Nhom,
                           NgayDieuTri = item.First().NgayDieuTri
                       })
                       .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu).Distinct().ToList();
                    }
                    else
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

                        var yeuCauVatTu = _yeuCauVatTuBenhVienRepository.TableNoTracking
                                                 .Where(x => x.KhoLinhId == long.Parse(queryString[7]) &&
                                                  x.YeuCauLinhVatTuId == null &&
                                                  x.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 &&
                                                  phongLinhVes.Any(o => o.KeyId == x.NoiChiDinhId) &&
                                                  x.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy
                                                  && x.YeuCauTiepNhanId == long.Parse(queryString[5]))
                       .Select(item => new DSLinhVatChildTuGridVo()
                       {
                           YeuCauLinhVatTuId = long.Parse(queryString[0]),
                           VatTuBenhVienId = item.VatTuBenhVienId,
                           LaBHYT = item.LaVatTuBHYT,
                           TenVatTu = item.Ten,
                           DonViTinh = item.VatTuBenhVien.VatTus.DonViTinh,
                           HangSanXuat = item.VatTuBenhVien.VatTus.NhaSanXuat,
                           NuocSanXuat = item.VatTuBenhVien.VatTus.NuocSanXuat,
                           SoLuongYeuCau = item.SoLuong,

                           DichVuKham = item.YeuCauKhamBenh != null ? item.YeuCauKhamBenh.TenDichVu : (item.YeuCauDichVuKyThuat.YeuCauKhamBenh != null ? item.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu : ""),
                           BacSiKeToa = item.NhanVienChiDinh.User.HoTen,
                           NgayKe = item.ThoiDiemChiDinh,
                           Nhom = item.LaVatTuBHYT == true ? "Vật Tư BHYT" : "Vật Tư Không BHYT",
                           NgayDieuTri = (item.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && item.NoiTruPhieuDieuTriId != null) ? item.NoiTruPhieuDieuTri.NgayDieuTri : item.ThoiDiemChiDinh,
                           SoLuongTon = item.VatTuBenhVien.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == item.KhoLinhId && nkct.LaVatTuBHYT == item.LaVatTuBHYT && nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat),
                       })
                       .GroupBy(x => new
                       {
                           x.YeuCauLinhVatTuId,
                           x.VatTuBenhVienId,
                           x.LaBHYT,
                           x.Nhom,
                           x.DonViTinh,
                           x.HangSanXuat,
                           x.NuocSanXuat,
                           x.DichVuKham,
                           x.BacSiKeToa,
                           x.NgayKe
                       })
                       .Select(item => new DSLinhVatChildTuGridVo()
                       {
                           YeuCauLinhVatTuId = long.Parse(queryString[0]),
                           VatTuBenhVienId = item.First().VatTuBenhVienId,
                           LaBHYT = item.First().LaBHYT,
                           TenVatTu = item.First().TenVatTu,
                           DonViTinh = item.First().DonViTinh,
                           HangSanXuat = item.First().HangSanXuat,
                           NuocSanXuat = item.First().NuocSanXuat,
                           SoLuongYeuCau = item.Sum(x => x.SoLuongYeuCau),
                           SoLuongTon = item.First().SoLuongTon,
                           DichVuKham = item.First().DichVuKham,
                           BacSiKeToa = item.First().BacSiKeToa,
                           NgayKe = item.First().NgayKe,
                           Nhom = item.First().Nhom,
                           NgayDieuTri = item.First().NgayDieuTri
                       })
                       .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu).Distinct().ToList();
                        if (yeuCauVatTu.Any())
                        {
                            queryable = queryable.Union(yeuCauVatTu).ToList();
                        }
                    }
                }
                else
                {
                    var yeuCauLinhVatTu =
                        await _yeuCauLinhVatTuRepository.TableNoTracking.FirstAsync(x => x.Id == long.Parse(queryString[0]));
                    queryable = _yeuCauVatTuBenhVienRepository.TableNoTracking
                        .Where(x => x.YeuCauLinhVatTuId == long.Parse(queryString[0]))
                        .Select(item => new DSLinhVatChildTuGridVo()
                        {
                            YeuCauLinhVatTuId = long.Parse(queryString[0]),
                            VatTuBenhVienId = item.VatTuBenhVienId,
                            LaBHYT = item.LaVatTuBHYT,
                            TenVatTu = item.VatTuBenhVien.VatTus.Ten,
                            DonViTinh = item.VatTuBenhVien.VatTus.DonViTinh,
                            HangSanXuat = item.VatTuBenhVien.VatTus.NhaSanXuat,
                            NuocSanXuat = item.VatTuBenhVien.VatTus.NuocSanXuat,
                            SoLuongYeuCau = item.SoLuong,
                            Nhom = item.LaVatTuBHYT == true ? "Vật Tư BHYT" : "Vật Tư Không BHYT",
                        })
                        .GroupBy(x => new
                        {
                            x.YeuCauLinhVatTuId,
                            x.VatTuBenhVienId,
                            x.LaBHYT,
                            x.Nhom,
                            x.DonViTinh,
                            x.HangSanXuat,
                            x.NuocSanXuat,
                            x.SoLuongYeuCau
                        })
                        .Select(item => new DSLinhVatChildTuGridVo()
                        {
                            YeuCauLinhVatTuId = long.Parse(queryString[0]),
                            VatTuBenhVienId = item.First().VatTuBenhVienId,
                            LaBHYT = item.First().LaBHYT,
                            TenVatTu = item.First().TenVatTu,
                            DonViTinh = item.First().DonViTinh,
                            HangSanXuat = item.First().HangSanXuat,
                            NuocSanXuat = item.First().NuocSanXuat,
                            SoLuongYeuCau = item.First().SoLuongYeuCau,
                            SoLuongTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                .Where(x => x.VatTuBenhVienId == item.First().VatTuBenhVienId
                                            && x.NhapKhoVatTu.KhoId == yeuCauLinhVatTu.KhoXuatId
                                            && x.NhapKhoVatTu.DaHet != true
                                            && x.LaVatTuBHYT == item.First().LaBHYT
                                            && x.SoLuongDaXuat < x.SoLuongNhap && x.HanSuDung >= DateTime.Now).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                            Nhom = item.First().Nhom
                        })
                        .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu).Distinct().ToList();
                }

                var countTask = queryable.AsQueryable().CountAsync();
                await Task.WhenAll(countTask);

                return new GridDataSource { TotalRowCount = countTask.Result };
            }
            return null;
        }
        #endregion
        private List<DSLinhVatTuGridVo> DangChoGoi(QueryInfo queryInfo, bool manHinhDuyet = false)
        {
            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = _phongBenhVienRepository.GetById(noiLamViecCurrentId);
            var queryString = JsonConvert.DeserializeObject<SeachNgay>(queryInfo.AdditionalSearchString);
            var queryDangChoDuyet = BaseRepository.TableNoTracking.Where(x => x.DuocDuyet == null && x.DaGui != true && ((manHinhDuyet == true && x.KhoXuat.LaKhoKSNK != true) || (manHinhDuyet == false && x.KhoNhap.LaKhoKSNK != true && x.NoiYeuCau != null && phongBenhVien != null && x.NoiYeuCau.KhoaPhongId == phongBenhVien.KhoaPhongId))).Select(s => new DSLinhVatTuGridVo()
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
        private List<DSLinhVatTuGridVo> DangChoDuyet(QueryInfo queryInfo, bool manHinhDuyet = false)
        {
            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = _phongBenhVienRepository.GetById(noiLamViecCurrentId);
            var queryString = JsonConvert.DeserializeObject<SeachNgay>(queryInfo.AdditionalSearchString);
            var queryDangChoDuyet = BaseRepository.TableNoTracking.Where(x => x.DuocDuyet == null && x.DaGui == true && ((manHinhDuyet == true && x.KhoXuat.LaKhoKSNK != true) || (manHinhDuyet == false && x.KhoNhap.LaKhoKSNK != true && x.NoiYeuCau != null && phongBenhVien != null && x.NoiYeuCau.KhoaPhongId == phongBenhVien.KhoaPhongId))).Select(s => new DSLinhVatTuGridVo()
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
            return queryDangChoDuyet.OrderByDescending(d=>d.NgayYeuCau).ToList();
        }
        private List<DSLinhVatTuGridVo> TuChoiDuyet(QueryInfo queryInfo, bool manHinhDuyet = false)
        {
            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = _phongBenhVienRepository.GetById(noiLamViecCurrentId);
            var queryString = JsonConvert.DeserializeObject<SeachNgay>(queryInfo.AdditionalSearchString);
            var TCD = BaseRepository.TableNoTracking.Where(x => x.DuocDuyet == false && x.DaGui == true && ((manHinhDuyet == true && x.KhoXuat.LaKhoKSNK != true) || (manHinhDuyet == false && x.KhoNhap.LaKhoKSNK != true && x.NoiYeuCau != null && phongBenhVien != null && x.NoiYeuCau.KhoaPhongId == phongBenhVien.KhoaPhongId))).Select(s => new DSLinhVatTuGridVo()
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
                if (queryStrings.NgayYeuCauRangDateStartDate == null && queryStrings.NgayYeuCauRangDateStartEnd == null)
                {
                    DateTime TuNgayYCPart = DateTime.Now;
                    DateTime DenNgayYCsPart = DateTime.Now;
                    var tuNgay = new DateTime(TuNgayYCPart.Year, TuNgayYCPart.Month, TuNgayYCPart.Day, 0, 0, 0);
                    var denNgay = new DateTime(DenNgayYCsPart.Year, DenNgayYCsPart.Month, DenNgayYCsPart.Day, 23, 59, 59);
                    TCD = TCD.Where(p => p.NgayYeuCau >= tuNgay && p.NgayYeuCau <= denNgay);
                }
            }
            return TCD.OrderByDescending(d => d.NgayYeuCau).ToList();
        }
        private List<DSLinhVatTuGridVo> DaDuyet(QueryInfo queryInfo, bool manHinhDuyet = false)
        {
            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = _phongBenhVienRepository.GetById(noiLamViecCurrentId);
            var queryString = JsonConvert.DeserializeObject<SeachNgay>(queryInfo.AdditionalSearchString);
            var queryDaDuyet = BaseRepository.TableNoTracking.Where(x => x.DuocDuyet == true && x.DaGui == true && ((manHinhDuyet == true && x.KhoXuat.LaKhoKSNK != true) || (manHinhDuyet == false && x.KhoNhap.LaKhoKSNK != true && x.NoiYeuCau != null && phongBenhVien != null && x.NoiYeuCau.KhoaPhongId == phongBenhVien.KhoaPhongId))).Select(s => new DSLinhVatTuGridVo()
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
                if (queryStrings.NgayYeuCauRangDateStartDate == null && queryStrings.NgayYeuCauRangDateStartEnd == null)
                {
                    DateTime TuNgayYCPart = DateTime.Now;
                    DateTime DenNgayYCsPart = DateTime.Now;
                    var tuNgay = new DateTime(TuNgayYCPart.Year, TuNgayYCPart.Month, TuNgayYCPart.Day, 0, 0, 0);
                    var denNgay = new DateTime(DenNgayYCsPart.Year, DenNgayYCsPart.Month, DenNgayYCsPart.Day, 23, 59, 59);
                    queryDaDuyet = queryDaDuyet.Where(p => p.NgayYeuCau >= tuNgay && p.NgayYeuCau <= denNgay);
                }
            }
            return queryDaDuyet.OrderByDescending(d => d.NgayYeuCau).ToList();
        }
        public virtual byte[] ExportDanhSachLayDuTruLinh(ICollection<DSLinhVatTuGridVo> datalinhs)
        {
            var queryInfo = new DSLinhVatTuGridVo();
            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<DSLinhVatTuGridVo>("STT", p => ind++)
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
                    string[] SetColumnItems = { "A", "B", "C", "D", "E", "F", "G", "H", "A", "I", "J", "K", "L", "M","N","O","P" };
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

                    var manager = new PropertyManager<DSLinhVatTuGridVo>(requestProperties);
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
                        // lĩnh trực tiếp
                        if (yeuCauLinhDuocPham.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan)
                        {
                            if(yeuCauLinhDuocPham.DuocDuyet != false)
                            {
                                using (var range = worksheet.Cells["B" + index + ":F" + index])
                                {
                                    range.Worksheet.Cells["B" + index + ":F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    range.Worksheet.Cells["B" + index + ":F" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    range.Worksheet.Cells["B" + index + ":F" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                    range.Worksheet.Cells["B" + index + ":F" + index].Style.Font.Color.SetColor(Color.Black);
                                    range.Worksheet.Cells["B" + index + ":F" + index].Style.Font.Bold = true;
                                    range.Worksheet.Cells["B" + index + ":F" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    range.Worksheet.Cells["B" + index + ":F" + index].Style.Fill.BackgroundColor.SetColor(Color.LightSkyBlue);


                                    string[,] SetColumnLinhBenhNhans = { { "B", "#" }, { "C", "Mã TN" }, { "D", "Mã BN" }, { "E", "Họ Tên" }, { "F", "SL" } };

                                    for (int i = 0; i < SetColumnLinhBenhNhans.Length / 2; i++)
                                    {
                                        var setColumn = ((SetColumnLinhBenhNhans[i, 0]).ToString() + index + ":" + (SetColumnLinhBenhNhans[i, 0]).ToString() + index).ToString();
                                        range.Worksheet.Cells[setColumn].Merge = true;
                                        range.Worksheet.Cells[setColumn].Value = SetColumnLinhBenhNhans[i, 1];
                                    }
                                    index++;
                                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                }
                                foreach (var nhom in yeuCauLinhDuocPham.ListChildLinhBenhNhan)
                                {
                                    worksheet.Cells["B" + index].Value = sttItems++;
                                    worksheet.Cells["C" + index].Value = nhom.MaYeuCauTiepNhan;
                                    worksheet.Cells["D" + index].Value = nhom.MaBenhNhan;
                                    worksheet.Cells["E" + index].Value = nhom.HoTen;
                                    worksheet.Cells["F" + index].Value = nhom.SoLuong + "";

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
                                        range.Worksheet.Cells["C" + index + ":M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        range.Worksheet.Cells["C" + index + ":M" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                        range.Worksheet.Cells["C" + index + ":M" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                        range.Worksheet.Cells["C" + index + ":M" + index].Style.Font.Color.SetColor(Color.Black);
                                        range.Worksheet.Cells["C" + index + ":M" + index].Style.Font.Bold = true;
                                        range.Worksheet.Cells["C" + index + ":M" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        range.Worksheet.Cells["C" + index + ":M" + index].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

                                        //Set column A to K
                                        string[,] SetColumnDichVus = { { "C" , "#" },{ "D" , "Tên Vật Tư" },
                                    { "E", "ĐVT" }, { "F", "Hãng SX" }, { "G", "Nước SX" },{ "H", "DV Khám" },{ "I", "BS Kê Toa" }
                                    ,{ "J", "Ngày Điều trị" },{ "K", "Ngày  Kê" },{ "L", "SL Tồn" },{ "M", "SL Yêu Cầu" } };

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
                                    var vatTu = nhom.ListChildChildLinhBenhNhan
                                                                                        .Select(s => new DSLinhVatChildTuGridVo
                                                                                        {
                                                                                            TenVatTu = s.TenVatTu,
                                                                                            NongDoHamLuong = s.NongDoHamLuong,
                                                                                            HoatChat = s.HoatChat,
                                                                                            DuongDung = s.DuongDung,
                                                                                            DonViTinh = s.DonViTinh,
                                                                                            HangSanXuat = s.HangSanXuat,
                                                                                            NuocSanXuat = s.NuocSanXuat,
                                                                                            DichVuKham = s.DichVuKham,
                                                                                            BacSiKeToa = s.BacSiKeToa,
                                                                                            NgayKe = s.NgayKe,
                                                                                            SoLuongTon = s.SoLuongTon,
                                                                                            SoLuongYeuCau = s.SoLuongYeuCau,
                                                                                            NgayDieuTri = s.NgayDieuTri
                                                                                        }).ToList();
                                    if (vatTu.Count() > 0)
                                    {
                                        foreach (var vt in vatTu) // bhyt
                                        {
                                            worksheet.Cells["C" + index].Value = sttDichVu++;
                                            worksheet.Cells["D" + index].Value = vt.TenVatTu;
                                            worksheet.Cells["E" + index].Value = vt.DonViTinh;
                                            worksheet.Cells["F" + index].Value = vt.HangSanXuat;
                                            worksheet.Cells["G" + index].Value = vt.NuocSanXuat;
                                            worksheet.Cells["H" + index].Value = vt.DichVuKham;
                                            worksheet.Cells["I" + index].Value = vt.BacSiKeToa;
                                            worksheet.Cells["J" + index].Value = vt.NgayDieuTriString;
                                            worksheet.Cells["K" + index].Value = vt.NgayKetString;
                                            worksheet.Cells["L" + index].Value = vt.SoLuongTon + "";
                                            worksheet.Cells["M" + index].Value = vt.SoLuongYeuCau + "";

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
                            else
                            {
                                var vatTu = yeuCauLinhDuocPham.ListChildLinhBenhNhan
                                                                                        .Select(s => new DSLinhVatChildTuGridVo
                                                                                        {
                                                                                            TenVatTu = s.TenVatTu,
                                                                                            NongDoHamLuong = s.NongDoHamLuong,
                                                                                            HoatChat = s.HoatChat,
                                                                                            DuongDung = s.DuongDung,
                                                                                            DonViTinh = s.DonViTinh,
                                                                                            HangSanXuat = s.HangSanXuat,
                                                                                            NuocSanXuat = s.NuocSanXuat,
                                                                                            DichVuKham = s.DichVuKham,
                                                                                            BacSiKeToa = s.BacSiKeToa,
                                                                                            NgayKe = s.NgayKe,
                                                                                            SoLuongTon = s.SoLuongTon,
                                                                                            SoLuongYeuCau = s.SoLuongYeuCau,
                                                                                            NgayDieuTri = s.NgayDieuTri
                                                                                        }).ToList();
                                using (var range = worksheet.Cells["B" + index + ":L" + index])
                                {
                                    range.Worksheet.Cells["B" + index + ":J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    range.Worksheet.Cells["B" + index + ":J" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    range.Worksheet.Cells["B" + index + ":J" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                    range.Worksheet.Cells["B" + index + ":J" + index].Style.Font.Color.SetColor(Color.Black);
                                    range.Worksheet.Cells["B" + index + ":J" + index].Style.Font.Bold = true;
                                    range.Worksheet.Cells["B" + index + ":J" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    range.Worksheet.Cells["B" + index + ":J" + index].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

                                    //Set column C to M
                                    string[,] SetColumnDichVus = { { "B" , "#" },{ "C" , "Tên Vật Tư" },
                                    { "D", "ĐVT" }, { "E", "Hãng SX" }, { "F", "Nước SX" },{ "G", "DV Khám" },{ "H", "BS Kê Toa" }
                                    ,{ "I", "Ngày Điều trị" },{ "J", "Ngày  Kê" },{ "K", "SL Tồn" },{ "L", "SL Yêu Cầu" } };
                                  

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
                                if (vatTu.Count() > 0)
                                {
                                    foreach (var vt in vatTu) // bhyt
                                    {
                                        worksheet.Cells["B" + index].Value = sttDichVu++;
                                        worksheet.Cells["C" + index].Value = vt.TenVatTu;
                                        worksheet.Cells["D" + index].Value = vt.DonViTinh;
                                        worksheet.Cells["E" + index].Value = vt.HangSanXuat;
                                        worksheet.Cells["F" + index].Value = vt.NuocSanXuat;
                                        worksheet.Cells["G" + index].Value = vt.DichVuKham;
                                        worksheet.Cells["H" + index].Value = vt.BacSiKeToa;
                                        worksheet.Cells["I" + index].Value = vt.NgayDieuTriString;
                                        worksheet.Cells["J" + index].Value = vt.NgayKetString;
                                        worksheet.Cells["K" + index].Value = vt.SoLuongTon + "";
                                        worksheet.Cells["L" + index].Value = vt.SoLuongYeuCau +"";



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
                            var vatTu = yeuCauLinhDuocPham.ListChildLinhBu
                                .Select(s => new YeuCauLinhVatTuBuGridVo
                                {
                                    TenVatTu = s.TenVatTu,
                                    DonViTinh = s.DonViTinh,
                                    HangSanXuat = s.HangSanXuat,
                                    NuocSanXuat = s.NuocSanXuat,
                                    SoLuongTon = s.SoLuongTon,
                                    SLDaLinh = s.SLDaLinh,
                                    SoLuongCanBu = s.SoLuongCanBu,
                                    SoLuongYeuCau = s.SoLuongYeuCau,
                                    ListChildChildLinhBu = s.ListChildChildLinhBu
                                }).ToList();
                            if (vatTu.Count() > 0)
                            {
                                foreach (var vt in vatTu)
                                {
                                    worksheet.Cells["B" + index].Value = sttItems++;
                                    worksheet.Cells["C" + index].Value = vt.TenVatTu;  // to do
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
                                   TenVatTu = k.TenVatTu,
                                   Nhom = k.Nhom,
                                   DonViTinh = k.DonViTinh,
                                   HangSanXuat = k.HangSanXuat,
                                   NuocSanXuat = k.NuocSanXuat,
                                   SoLuongYeuCau = k.SoLuongYc
                               }).ToList();
                            var dutruthuongKhongBHYT = yeuCauLinhDuocPham.ListChildLinhDuTru.Where(s => s.LaBHYT == false)
                               .Select(k => new DSLinhVatChildTuGridVo()
                               {
                                   TenVatTu = k.TenVatTu,
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
                                    worksheet.Cells["C" + index].Value = nhom.TenVatTu;  // to do
                                    worksheet.Cells["D" + index].Value = nhom.DonViTinh;
                                    worksheet.Cells["E" + index].Value = nhom.HangSanXuat;
                                    worksheet.Cells["F" + index].Value = nhom.NuocSanXuat;
                                    worksheet.Cells["G" + index].Value = nhom.SoLuongYeuCau +"";

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
                                    worksheet.Cells["C" + index].Value = nhom.TenVatTu;  // to do
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
        public virtual byte[] ExportDanhSachDuyetLayDuTruLinh(ICollection<DSLinhVatTuGridVo> datalinhs)
        {
            var queryInfo = new DSLinhVatTuGridVo();
            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<DSLinhVatTuGridVo>("STT", p => ind++)
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

                    var manager = new PropertyManager<DSLinhVatTuGridVo>(requestProperties);
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
                        // lĩnh trực tiếp
                        if (yeuCauLinhDuocPham.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan)
                        {
                            if (yeuCauLinhDuocPham.DuocDuyet != false)
                            {
                                using (var range = worksheet.Cells["B" + index + ":F" + index])
                                {
                                    range.Worksheet.Cells["B" + index + ":F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    range.Worksheet.Cells["B" + index + ":F" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    range.Worksheet.Cells["B" + index + ":F" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                    range.Worksheet.Cells["B" + index + ":F" + index].Style.Font.Color.SetColor(Color.Black);
                                    range.Worksheet.Cells["B" + index + ":F" + index].Style.Font.Bold = true;
                                    range.Worksheet.Cells["B" + index + ":F" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    range.Worksheet.Cells["B" + index + ":F" + index].Style.Fill.BackgroundColor.SetColor(Color.LightSkyBlue);


                                    string[,] SetColumnLinhBenhNhans = { { "B", "#" }, { "C", "Mã TN" }, { "D", "Mã BN" }, { "E", "Họ Tên" }, { "F", "SL" } };

                                    for (int i = 0; i < SetColumnLinhBenhNhans.Length / 2; i++)
                                    {
                                        var setColumn = ((SetColumnLinhBenhNhans[i, 0]).ToString() + index + ":" + (SetColumnLinhBenhNhans[i, 0]).ToString() + index).ToString();
                                        range.Worksheet.Cells[setColumn].Merge = true;
                                        range.Worksheet.Cells[setColumn].Value = SetColumnLinhBenhNhans[i, 1];
                                    }
                                    index++;
                                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                }
                                foreach (var nhom in yeuCauLinhDuocPham.ListChildLinhBenhNhan)
                                {
                                    worksheet.Cells["B" + index].Value = sttItems++;
                                    worksheet.Cells["C" + index].Value = nhom.MaYeuCauTiepNhan;
                                    worksheet.Cells["D" + index].Value = nhom.MaBenhNhan;
                                    worksheet.Cells["E" + index].Value = nhom.HoTen;
                                    worksheet.Cells["F" + index].Value = nhom.SoLuong + "";

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
                                        range.Worksheet.Cells["C" + index + ":M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        range.Worksheet.Cells["C" + index + ":M" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                        range.Worksheet.Cells["C" + index + ":M" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                        range.Worksheet.Cells["C" + index + ":M" + index].Style.Font.Color.SetColor(Color.Black);
                                        range.Worksheet.Cells["C" + index + ":M" + index].Style.Font.Bold = true;
                                        range.Worksheet.Cells["C" + index + ":M" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        range.Worksheet.Cells["C" + index + ":M" + index].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

                                        //Set column A to K
                                        string[,] SetColumnDichVus = { { "C" , "#" },{ "D" , "Tên Vật Tư" },
                                    { "E", "ĐVT" }, { "F", "Hãng SX" }, { "G", "Nước SX" },{ "H", "DV Khám" },{ "I", "BS Kê Toa" }
                                    ,{ "J", "Ngày Điều trị" },{ "K", "Ngày  Kê" },{ "L", "SL Tồn" },{ "M", "SL Yêu Cầu" } };

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
                                    var duocPham = nhom.ListChildChildLinhBenhNhan
                                                                                        .Select(s => new DSLinhVatChildTuGridVo
                                                                                        {
                                                                                            TenVatTu = s.TenVatTu,
                                                                                            NongDoHamLuong = s.NongDoHamLuong,
                                                                                            HoatChat = s.HoatChat,
                                                                                            DuongDung = s.DuongDung,
                                                                                            DonViTinh = s.DonViTinh,
                                                                                            HangSanXuat = s.HangSanXuat,
                                                                                            NuocSanXuat = s.NuocSanXuat,
                                                                                            DichVuKham = s.DichVuKham,
                                                                                            BacSiKeToa = s.BacSiKeToa,
                                                                                            NgayKe = s.NgayKe,
                                                                                            SoLuongTon = s.SoLuongTon,
                                                                                            SoLuongYeuCau = s.SoLuongYeuCau,
                                                                                            NgayDieuTri = s.NgayDieuTri
                                                                                        }).ToList();
                                  
                                    if (duocPham.Count() > 0)
                                    {
                                        foreach (var vt in duocPham) // bhyt
                                        {
                                            worksheet.Cells["C" + index].Value = sttDichVu++;
                                            worksheet.Cells["D" + index].Value = vt.TenVatTu;
                                            worksheet.Cells["E" + index].Value = vt.DonViTinh;
                                            worksheet.Cells["F" + index].Value = vt.HangSanXuat;
                                            worksheet.Cells["G" + index].Value = vt.NuocSanXuat;
                                            worksheet.Cells["H" + index].Value = vt.DichVuKham;
                                            worksheet.Cells["I" + index].Value = vt.BacSiKeToa;
                                            worksheet.Cells["J" + index].Value = vt.NgayDieuTriString;
                                            worksheet.Cells["K" + index].Value = vt.NgayKetString;
                                            worksheet.Cells["L" + index].Value = vt.SoLuongTon + "";
                                            worksheet.Cells["M" + index].Value = vt.SoLuongYeuCau + "";

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
                            else
                            {
                                var vatTu = yeuCauLinhDuocPham.ListChildLinhBenhNhan
                                                                                        .Select(s => new DSLinhVatChildTuGridVo
                                                                                        {
                                                                                            TenVatTu = s.TenVatTu,
                                                                                            NongDoHamLuong = s.NongDoHamLuong,
                                                                                            HoatChat = s.HoatChat,
                                                                                            DuongDung = s.DuongDung,
                                                                                            DonViTinh = s.DonViTinh,
                                                                                            HangSanXuat = s.HangSanXuat,
                                                                                            NuocSanXuat = s.NuocSanXuat,
                                                                                            DichVuKham = s.DichVuKham,
                                                                                            BacSiKeToa = s.BacSiKeToa,
                                                                                            NgayKe = s.NgayKe,
                                                                                            SoLuongTon = s.SoLuongTon,
                                                                                            SoLuongYeuCau = s.SoLuongYeuCau,
                                                                                            NgayDieuTri = s.NgayDieuTri
                                                                                        }).ToList();
                           
                                using (var range = worksheet.Cells["B" + index + ":L" + index])
                                {
                                    range.Worksheet.Cells["B" + index + ":J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    range.Worksheet.Cells["B" + index + ":J" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    range.Worksheet.Cells["B" + index + ":J" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                    range.Worksheet.Cells["B" + index + ":J" + index].Style.Font.Color.SetColor(Color.Black);
                                    range.Worksheet.Cells["B" + index + ":J" + index].Style.Font.Bold = true;
                                    range.Worksheet.Cells["B" + index + ":J" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    range.Worksheet.Cells["B" + index + ":J" + index].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

                                    //Set column C to M
                                    string[,] SetColumnDichVus = { { "B" , "#" },{ "C" , "Tên Vật Tư" },
                                    { "D", "ĐVT" }, { "E", "Hãng SX" }, { "F", "Nước SX" },{ "G", "DV Khám" },{ "H", "BS Kê Toa" }
                                    ,{ "I", "Ngày Điều trị" },{ "J", "Ngày  Kê" },{ "K", "SL Tồn" },{ "L", "SL Yêu Cầu" } };


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
                                if (vatTu.Count() > 0)
                                {
                                    foreach (var vt in vatTu) // bhyt
                                    {
                                        worksheet.Cells["B" + index].Value = sttDichVu++;
                                        worksheet.Cells["C" + index].Value = vt.TenVatTu;
                                        worksheet.Cells["D" + index].Value = vt.DonViTinh;
                                        worksheet.Cells["E" + index].Value = vt.HangSanXuat;
                                        worksheet.Cells["F" + index].Value = vt.NuocSanXuat;
                                        worksheet.Cells["G" + index].Value = vt.DichVuKham;
                                        worksheet.Cells["H" + index].Value = vt.BacSiKeToa;
                                        worksheet.Cells["I" + index].Value = vt.NgayDieuTriString;
                                        worksheet.Cells["J" + index].Value = vt.NgayKetString;
                                        worksheet.Cells["K" + index].Value = vt.SoLuongTon + "";
                                        worksheet.Cells["L" + index].Value = vt.SoLuongYeuCau + "";



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
                            var vatTu = yeuCauLinhDuocPham.ListChildLinhBu
                                .Select(s => new YeuCauLinhVatTuBuGridVo
                                {
                                    TenVatTu = s.TenVatTu,
                                    DonViTinh = s.DonViTinh,
                                    HangSanXuat = s.HangSanXuat,
                                    NuocSanXuat = s.NuocSanXuat,
                                    SoLuongTon = s.SoLuongTon,
                                    SLDaLinh = s.SLDaLinh,
                                    SoLuongCanBu = s.SoLuongCanBu,
                                    SoLuongYeuCau = s.SoLuongYeuCau,
                                    ListChildChildLinhBu = s.ListChildChildLinhBu,
                                }).ToList();
                            if (vatTu.Count() > 0)
                            {
                                foreach (var nhom in vatTu)
                                {
                                    worksheet.Cells["B" + index].Value = sttItems++;
                                    worksheet.Cells["C" + index].Value = nhom.TenVatTu;  // to do
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
                                   TenVatTu = k.TenVatTu,
                                   Nhom = k.Nhom,
                                   DonViTinh = k.DonViTinh,
                                   HangSanXuat = k.HangSanXuat,
                                   NuocSanXuat = k.NuocSanXuat,
                                   SoLuongYeuCau = k.SoLuongYc
                               }).ToList();
                            var dutruthuongKhongBHYT = yeuCauLinhDuocPham.ListChildLinhDuTru.Where(s => s.LaBHYT == false)
                               .Select(k => new DSLinhVatChildTuGridVo()
                               {
                                   TenVatTu = k.TenVatTu,
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
                                    worksheet.Cells["C" + index].Value = nhom.TenVatTu;  // to do
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
                                    worksheet.Cells["C" + index].Value = nhom.TenVatTu;  // to do
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
        public async Task<GridDataSource> GetDataDSYeuCauLinhVatTuChildChildForGridAsync(QueryInfo queryInfo)
        {
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            int loaiPhieulinh = int.Parse(queryString[1]);
            EnumLoaiPhieuLinh loaiPhieu;
            if (loaiPhieulinh == 1)
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhDuTru;
            }
            else if (loaiPhieulinh == 2)
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhBu;
            }
            else
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhChoBenhNhan;
            }
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhBu)
            {
                BuildDefaultSortExpression(queryInfo);
                //var query = _yeuCauVatTuBenhVienRepository.TableNoTracking
                //.Where(o => o.VatTuBenhVienId == long.Parse(queryString[2])
                //            && o.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu
                //            //&& o.LaVatTuBHYT == bool.Parse(queryString[3])
                //            //&& o.KhoLinhId == long.Parse(queryString[4])
                //            && o.KhongLinhBu != true
                //            && o.YeuCauLinhVatTuId == null
                //            && ( o.SoLuongDaLinhBu == null || o.SoLuongDaLinhBu < o.SoLuong))
                var trangThai = _yeuCauLinhVatTuChiTietRepository.TableNoTracking.Where(x => x.Id == long.Parse(queryString[0])).Select(s => s.YeuCauLinhVatTu.DuocDuyet).FirstOrDefault();
                var yeuCauLinhVatTuId = _yeuCauLinhVatTuChiTietRepository.TableNoTracking.Where(x => x.Id == long.Parse(queryString[0])).Select(s => s.YeuCauLinhVatTuId).FirstOrDefault();
                var query = _yeuCauLinhVatTuChiTietRepository.TableNoTracking
                         .Where(p => p.YeuCauLinhVatTuId == yeuCauLinhVatTuId
                            && p.YeuCauLinhVatTu.DuocDuyet == trangThai
                             && p.VatTuBenhVienId == long.Parse(queryString[2])
                             && p.LaVatTuBHYT == bool.Parse(queryString[3])
                             && p.YeuCauVatTuBenhVien.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhBu
                         //&& (p.YeuCauLinhVatTu.DuocDuyet == false ? p.YeuCauVatTuBenhVien.KhongLinhBu != false : p.YeuCauVatTuBenhVien.KhongLinhBu != true)
                         )
             .OrderBy(x => x.YeuCauVatTuBenhVien.ThoiDiemChiDinh)
                .Select(s => new VatTuLinhBuCuaBNGridVos
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
                        TenVatTu = s.VatTuBenhVien.VatTus.Ten,
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
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhChoBenhNhan)
            {
                BuildDefaultSortExpression(queryInfo);
                int kieuIn = 0;
                if (queryString[6] == "" || queryString[6] == "true")
                {
                    kieuIn = 0;
                }
                if (queryString[6] == "false" || queryString[6] == "False")
                {
                    kieuIn = 1;
                }
                List<DSLinhVatChildTuGridVo> queryable = new List<DSLinhVatChildTuGridVo>();
                var yeuCau =
                 _yeuCauLinhVatTuRepository.TableNoTracking.Any(x => x.Id == long.Parse(queryString[0]));

                if (kieuIn == 0)
                {
                    if (yeuCau == true)
                    {
                        var yeuCauLinhVatTu =
                        await _yeuCauLinhVatTuRepository.TableNoTracking.FirstAsync(x => x.Id == long.Parse(queryString[0]));
                        queryable = _yeuCauVatTuBenhVienRepository.TableNoTracking
                       .Where(x => x.YeuCauLinhVatTuId == long.Parse(queryString[0])
                                   && x.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan
                                   && x.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy
                                   && x.YeuCauTiepNhanId == long.Parse(queryString[5]))
                       .Select(item => new DSLinhVatChildTuGridVo()
                       {
                           YeuCauLinhVatTuId = long.Parse(queryString[0]),
                           VatTuBenhVienId = item.VatTuBenhVienId,
                           LaBHYT = item.LaVatTuBHYT,
                           TenVatTu = item.Ten,
                           DonViTinh = item.VatTuBenhVien.VatTus.DonViTinh,
                           HangSanXuat = item.VatTuBenhVien.VatTus.NhaSanXuat,
                           NuocSanXuat = item.VatTuBenhVien.VatTus.NuocSanXuat,
                           SoLuongYeuCau = item.SoLuong,

                           DichVuKham = item.YeuCauKhamBenh != null ? item.YeuCauKhamBenh.TenDichVu : (item.YeuCauDichVuKyThuat.YeuCauKhamBenh != null ? item.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu : "Ghi nhận trong PTTT"),
                           BacSiKeToa = item.NhanVienChiDinh.User.HoTen,
                           NgayKe = item.ThoiDiemChiDinh,
                           NgayKetString = item.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                           Nhom = item.LaVatTuBHYT == true ? "Vật Tư BHYT" : "Vật Tư Không BHYT",
                           NgayDieuTri = (item.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && item.NoiTruPhieuDieuTriId != null) ? item.NoiTruPhieuDieuTri.NgayDieuTri : item.ThoiDiemChiDinh,
                           DuocDuyet = item.YeuCauLinhVatTu.DuocDuyet
                       })
                       .GroupBy(x => new
                       {
                           x.YeuCauLinhVatTuId,
                           x.VatTuBenhVienId,
                           x.LaBHYT,
                           x.Nhom,
                           x.DonViTinh,
                           x.HangSanXuat,
                           x.NuocSanXuat,
                           x.DichVuKham,
                           x.BacSiKeToa,
                           x.NgayKe
                       })
                       .Select(item => new DSLinhVatChildTuGridVo()
                       {
                           YeuCauLinhVatTuId = long.Parse(queryString[0]),
                           VatTuBenhVienId = item.First().VatTuBenhVienId,
                           LaBHYT = item.First().LaBHYT,
                           TenVatTu = item.First().TenVatTu,
                           DonViTinh = item.First().DonViTinh,
                           HangSanXuat = item.First().HangSanXuat,
                           NuocSanXuat = item.First().NuocSanXuat,
                           SoLuongYeuCau = item.Sum(x => x.SoLuongYeuCau),
                           SoLuongTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                               .Where(x => x.VatTuBenhVienId == item.First().VatTuBenhVienId
                                           && x.NhapKhoVatTu.KhoId == yeuCauLinhVatTu.KhoXuatId
                                           && x.LaVatTuBHYT == item.First().LaBHYT
                                           && x.NhapKhoVatTu.DaHet != true
                                           && x.HanSuDung >= DateTime.Now).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                           DichVuKham = item.First().DichVuKham,
                           BacSiKeToa = item.First().BacSiKeToa,
                           NgayKe = item.First().NgayKe,
                           Nhom = item.First().Nhom,
                           NgayDieuTri = item.First().NgayDieuTri,
                           NgayKetString = item.First().NgayKetString,
                           DuocDuyet = item.First().DuocDuyet
                       })
                       .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu).Distinct().ToList();
                    }
                    else
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

                        var yeuCauVatTu = _yeuCauVatTuBenhVienRepository.TableNoTracking
                                                 .Where(x => x.KhoLinhId == long.Parse(queryString[7]) &&
                                                  x.YeuCauLinhVatTuId == null &&
                                                  x.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 &&
                                                  phongLinhVes.Any(o => o.KeyId == x.NoiChiDinhId) &&
                                                  x.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy
                                                  && x.YeuCauTiepNhanId == long.Parse(queryString[5]))
                       .Select(item => new DSLinhVatChildTuGridVo()
                       {
                           YeuCauLinhVatTuId = long.Parse(queryString[0]),
                           VatTuBenhVienId = item.VatTuBenhVienId,
                           LaBHYT = item.LaVatTuBHYT,
                           TenVatTu = item.Ten,
                           DonViTinh = item.VatTuBenhVien.VatTus.DonViTinh,
                           HangSanXuat = item.VatTuBenhVien.VatTus.NhaSanXuat,
                           NuocSanXuat = item.VatTuBenhVien.VatTus.NuocSanXuat,
                           SoLuongYeuCau = item.SoLuong,

                           DichVuKham = item.YeuCauKhamBenh != null ? item.YeuCauKhamBenh.TenDichVu : (item.YeuCauDichVuKyThuat.YeuCauKhamBenh != null ? item.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu : ""),
                           BacSiKeToa = item.NhanVienChiDinh.User.HoTen,
                           NgayKe = item.ThoiDiemChiDinh,
                           NgayKetString = item.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                           Nhom = item.LaVatTuBHYT == true ? "Vật Tư BHYT" : "Vật Tư Không BHYT",
                           NgayDieuTri = (item.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru  && item.NoiTruPhieuDieuTriId != null)? item.NoiTruPhieuDieuTri.NgayDieuTri : item.ThoiDiemChiDinh,
                           SoLuongTon = item.VatTuBenhVien.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == item.KhoLinhId && nkct.LaVatTuBHYT == item.LaVatTuBHYT && nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat),

                       })
                       .GroupBy(x => new
                       {
                           x.YeuCauLinhVatTuId,
                           x.VatTuBenhVienId,
                           x.LaBHYT,
                           x.Nhom,
                           x.DonViTinh,
                           x.HangSanXuat,
                           x.NuocSanXuat,
                           x.DichVuKham,
                           x.BacSiKeToa,
                           x.NgayKe
                       })
                       .Select(item => new DSLinhVatChildTuGridVo()
                       {
                           YeuCauLinhVatTuId = long.Parse(queryString[0]),
                           VatTuBenhVienId = item.First().VatTuBenhVienId,
                           LaBHYT = item.First().LaBHYT,
                           TenVatTu = item.First().TenVatTu,
                           DonViTinh = item.First().DonViTinh,
                           HangSanXuat = item.First().HangSanXuat,
                           NuocSanXuat = item.First().NuocSanXuat,
                           SoLuongYeuCau = item.Sum(x => x.SoLuongYeuCau),
                           SoLuongTon = item.First().SoLuongTon,
                           DichVuKham = item.First().DichVuKham,
                           BacSiKeToa = item.First().BacSiKeToa,
                           NgayKe = item.First().NgayKe,
                           Nhom = item.First().Nhom,
                           NgayDieuTri = item.First().NgayDieuTri,
                           NgayKetString = item.First().NgayKetString
                       })
                       .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu).Distinct().ToList();
                        if (yeuCauVatTu.Any())
                        {
                            queryable = queryable.Union(yeuCauVatTu).ToList();
                        }
                    }
                }
                else
                {
                    var yeuCauLinhVatTu =
                        await _yeuCauLinhVatTuRepository.TableNoTracking.FirstAsync(x => x.Id == long.Parse(queryString[0]));
                    queryable = _yeuCauVatTuBenhVienRepository.TableNoTracking
                        .Where(x => x.YeuCauLinhVatTuId == long.Parse(queryString[0]))
                        .Select(item => new DSLinhVatChildTuGridVo()
                        {
                            YeuCauLinhVatTuId = long.Parse(queryString[0]),
                            VatTuBenhVienId = item.VatTuBenhVienId,
                            LaBHYT = item.LaVatTuBHYT,
                            TenVatTu = item.VatTuBenhVien.VatTus.Ten,
                            DonViTinh = item.VatTuBenhVien.VatTus.DonViTinh,
                            HangSanXuat = item.VatTuBenhVien.VatTus.NhaSanXuat,
                            NuocSanXuat = item.VatTuBenhVien.VatTus.NuocSanXuat,
                            SoLuongYeuCau = item.SoLuong,
                            Nhom = item.LaVatTuBHYT == true ? "Vật Tư BHYT" : "Vật Tư Không BHYT",
                        })
                        .GroupBy(x => new
                        {
                            x.YeuCauLinhVatTuId,
                            x.VatTuBenhVienId,
                            x.LaBHYT,
                            x.Nhom,
                            x.DonViTinh,
                            x.HangSanXuat,
                            x.NuocSanXuat,
                            x.SoLuongYeuCau
                        })
                        .Select(item => new DSLinhVatChildTuGridVo()
                        {
                            YeuCauLinhVatTuId = long.Parse(queryString[0]),
                            VatTuBenhVienId = item.First().VatTuBenhVienId,
                            LaBHYT = item.First().LaBHYT,
                            TenVatTu = item.First().TenVatTu,
                            DonViTinh = item.First().DonViTinh,
                            HangSanXuat = item.First().HangSanXuat,
                            NuocSanXuat = item.First().NuocSanXuat,
                            SoLuongYeuCau = item.First().SoLuongYeuCau,
                            SoLuongTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                .Where(x => x.VatTuBenhVienId == item.First().VatTuBenhVienId
                                            && x.NhapKhoVatTu.KhoId == yeuCauLinhVatTu.KhoXuatId
                                            && x.NhapKhoVatTu.DaHet != true
                                            && x.LaVatTuBHYT == item.First().LaBHYT
                                            && x.SoLuongDaXuat < x.SoLuongNhap && x.HanSuDung >= DateTime.Now).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                            Nhom = item.First().Nhom
                        })
                        .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu).Distinct().ToList();
                }

                //var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : queryable.CountAsync();
                //var queryTask = queryable.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                //    .Take(queryInfo.Take).ToArrayAsync();
                //await Task.WhenAll(countTask, queryTask);
                //return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
                var dataOrderBy = queryable.AsQueryable().OrderBy(queryInfo.SortString);
                var data = dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
                var countTask = dataOrderBy.Count();

                return new GridDataSource { Data = data, TotalRowCount = countTask };
            }
            return null;
        }
        public async Task<GridDataSource> GetTotalPageFDSYeuCauLinhVatTuChildChildForGridAsync(QueryInfo queryInfo)
        {
            var queryString = queryInfo.AdditionalSearchString.Split('-');
            int loaiPhieulinh = int.Parse(queryString[1]);
            EnumLoaiPhieuLinh loaiPhieu;
            if (loaiPhieulinh == 1)
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhDuTru;
            }
            else if (loaiPhieulinh == 2)
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhBu;
            }
            else
            {
                loaiPhieu = EnumLoaiPhieuLinh.LinhChoBenhNhan;
            }
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhBu)
            {
                var yeuCauLinhVatTuId = _yeuCauLinhVatTuChiTietRepository.TableNoTracking.Where(x => x.Id == long.Parse(queryString[0])).Select(s => s.YeuCauLinhVatTuId).FirstOrDefault();
                var query = _yeuCauLinhVatTuChiTietRepository.TableNoTracking
                       .Where(p => p.YeuCauLinhVatTuId == yeuCauLinhVatTuId
                            //&& p.YeuCauVatTuBenhVien.TrangThai == EnumYeuCauVatTuBenhVien.DaThucHien
                            && (p.YeuCauLinhVatTu.DuocDuyet == false ? p.YeuCauVatTuBenhVien.KhongLinhBu != false : p.YeuCauVatTuBenhVien.KhongLinhBu != true)
                       )
           .OrderBy(x => x.YeuCauVatTuBenhVien.ThoiDiemChiDinh)
              .Select(s => new VatTuLinhBuCuaBNGridVos
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
                        TenVatTu = s.VatTuBenhVien.VatTus.Ten,
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
            if (loaiPhieu == EnumLoaiPhieuLinh.LinhChoBenhNhan)
            {
                int kieuIn = 0;
                if (queryString[6] == "" || queryString[6] == "true")
                {
                    kieuIn = 0;
                }
                if (queryString[6] == "false" || queryString[6] == "False")
                {
                    kieuIn = 1;
                }
                List<DSLinhVatChildTuGridVo> queryable = new List<DSLinhVatChildTuGridVo>();
                var yeuCau =
                 _yeuCauLinhVatTuRepository.TableNoTracking.Any(x => x.Id == long.Parse(queryString[0]));

                if (kieuIn == 0)
                {
                    if (yeuCau == true)
                    {
                        var yeuCauLinhVatTu =
                        await _yeuCauLinhVatTuRepository.TableNoTracking.FirstAsync(x => x.Id == long.Parse(queryString[0]));
                        queryable = _yeuCauVatTuBenhVienRepository.TableNoTracking
                       .Where(x => x.YeuCauLinhVatTuId == long.Parse(queryString[0])
                                   && x.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan
                                   && x.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy
                                   && x.YeuCauTiepNhanId == long.Parse(queryString[5]))
                       .Select(item => new DSLinhVatChildTuGridVo()
                       {
                           YeuCauLinhVatTuId = long.Parse(queryString[0]),
                           VatTuBenhVienId = item.VatTuBenhVienId,
                           LaBHYT = item.LaVatTuBHYT,
                           TenVatTu = item.Ten,
                           DonViTinh = item.VatTuBenhVien.VatTus.DonViTinh,
                           HangSanXuat = item.VatTuBenhVien.VatTus.NhaSanXuat,
                           NuocSanXuat = item.VatTuBenhVien.VatTus.NuocSanXuat,
                           SoLuongYeuCau = item.SoLuong,

                           DichVuKham = item.YeuCauKhamBenh != null ? item.YeuCauKhamBenh.TenDichVu : (item.YeuCauDichVuKyThuat.YeuCauKhamBenh != null ? item.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu : "Ghi nhận trong PTTT"),
                           BacSiKeToa = item.NhanVienChiDinh.User.HoTen,
                           NgayKe = item.ThoiDiemChiDinh,
                           NgayKetString =item.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                           Nhom = item.LaVatTuBHYT == true ? "Vật Tư BHYT" : "Vật Tư Không BHYT",
                           NgayDieuTri = (item.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && item.NoiTruPhieuDieuTriId != null) ? item.NoiTruPhieuDieuTri.NgayDieuTri : item.ThoiDiemChiDinh
                       })
                       .GroupBy(x => new
                       {
                           x.YeuCauLinhVatTuId,
                           x.VatTuBenhVienId,
                           x.LaBHYT,
                           x.Nhom,
                           x.DonViTinh,
                           x.HangSanXuat,
                           x.NuocSanXuat,
                           x.DichVuKham,
                           x.BacSiKeToa,
                           x.NgayKe
                       })
                       .Select(item => new DSLinhVatChildTuGridVo()
                       {
                           YeuCauLinhVatTuId = long.Parse(queryString[0]),
                           VatTuBenhVienId = item.First().VatTuBenhVienId,
                           LaBHYT = item.First().LaBHYT,
                           TenVatTu = item.First().TenVatTu,
                           DonViTinh = item.First().DonViTinh,
                           HangSanXuat = item.First().HangSanXuat,
                           NuocSanXuat = item.First().NuocSanXuat,
                           SoLuongYeuCau = item.Sum(x => x.SoLuongYeuCau),
                           SoLuongTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                               .Where(x => x.VatTuBenhVienId == item.First().VatTuBenhVienId
                                           && x.NhapKhoVatTu.KhoId == yeuCauLinhVatTu.KhoXuatId
                                           && x.LaVatTuBHYT == item.First().LaBHYT
                                           && x.NhapKhoVatTu.DaHet != true
                                           && x.SoLuongDaXuat < x.SoLuongNhap && x.HanSuDung >= DateTime.Now).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                           DichVuKham = item.First().DichVuKham,
                           BacSiKeToa = item.First().BacSiKeToa,
                           NgayKe = item.First().NgayKe,
                           Nhom = item.First().Nhom,
                           NgayDieuTri = item.First().NgayDieuTri,
                           NgayKetString = item.First().NgayKetString
                       })
                       .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu).Distinct().ToList();
                    }
                    else
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

                        var yeuCauVatTu = _yeuCauVatTuBenhVienRepository.TableNoTracking
                                                 .Where(x => x.KhoLinhId == long.Parse(queryString[7]) &&
                                                  x.YeuCauLinhVatTuId == null &&
                                                  x.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 &&
                                                  phongLinhVes.Any(o => o.KeyId == x.NoiChiDinhId) &&
                                                  x.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy
                                                  && x.YeuCauTiepNhanId == long.Parse(queryString[5]))
                       .Select(item => new DSLinhVatChildTuGridVo()
                       {
                           YeuCauLinhVatTuId = long.Parse(queryString[0]),
                           VatTuBenhVienId = item.VatTuBenhVienId,
                           LaBHYT = item.LaVatTuBHYT,
                           TenVatTu = item.Ten,
                           DonViTinh = item.VatTuBenhVien.VatTus.DonViTinh,
                           HangSanXuat = item.VatTuBenhVien.VatTus.NhaSanXuat,
                           NuocSanXuat = item.VatTuBenhVien.VatTus.NuocSanXuat,
                           SoLuongYeuCau = item.SoLuong,

                           DichVuKham = item.YeuCauKhamBenh != null ? item.YeuCauKhamBenh.TenDichVu : (item.YeuCauDichVuKyThuat.YeuCauKhamBenh != null ? item.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu : "Ghi nhận trong PTTT"),
                           BacSiKeToa = item.NhanVienChiDinh.User.HoTen,
                           NgayKe = item.ThoiDiemChiDinh,
                           Nhom = item.LaVatTuBHYT == true ? "Vật Tư BHYT" : "Vật Tư Không BHYT",
                           NgayDieuTri = (item.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && item.NoiTruPhieuDieuTriId != null) ? item.NoiTruPhieuDieuTri.NgayDieuTri : item.ThoiDiemChiDinh,
                           SoLuongTon = item.VatTuBenhVien.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == item.KhoLinhId && nkct.LaVatTuBHYT == item.LaVatTuBHYT && nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat),
                       })
                       .GroupBy(x => new
                       {
                           x.YeuCauLinhVatTuId,
                           x.VatTuBenhVienId,
                           x.LaBHYT,
                           x.Nhom,
                           x.DonViTinh,
                           x.HangSanXuat,
                           x.NuocSanXuat,
                           x.DichVuKham,
                           x.BacSiKeToa,
                           x.NgayKe
                       })
                       .Select(item => new DSLinhVatChildTuGridVo()
                       {
                           YeuCauLinhVatTuId = long.Parse(queryString[0]),
                           VatTuBenhVienId = item.First().VatTuBenhVienId,
                           LaBHYT = item.First().LaBHYT,
                           TenVatTu = item.First().TenVatTu,
                           DonViTinh = item.First().DonViTinh,
                           HangSanXuat = item.First().HangSanXuat,
                           NuocSanXuat = item.First().NuocSanXuat,
                           SoLuongYeuCau = item.Sum(x => x.SoLuongYeuCau),
                           SoLuongTon = item.First().SoLuongTon,
                           DichVuKham = item.First().DichVuKham,
                           BacSiKeToa = item.First().BacSiKeToa,
                           NgayKe = item.First().NgayKe,
                           Nhom = item.First().Nhom,
                           NgayDieuTri = item.First().NgayDieuTri
                       })
                       .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu).Distinct().ToList();
                        if (yeuCauVatTu.Any())
                        {
                            queryable = queryable.Union(yeuCauVatTu).ToList();
                        }
                    }
                }
                else
                {
                    var yeuCauLinhVatTu =
                        await _yeuCauLinhVatTuRepository.TableNoTracking.FirstAsync(x => x.Id == long.Parse(queryString[0]));
                    queryable = _yeuCauVatTuBenhVienRepository.TableNoTracking
                        .Where(x => x.YeuCauLinhVatTuId == long.Parse(queryString[0]))
                        .Select(item => new DSLinhVatChildTuGridVo()
                        {
                            YeuCauLinhVatTuId = long.Parse(queryString[0]),
                            VatTuBenhVienId = item.VatTuBenhVienId,
                            LaBHYT = item.LaVatTuBHYT,
                            TenVatTu = item.VatTuBenhVien.VatTus.Ten,
                            DonViTinh = item.VatTuBenhVien.VatTus.DonViTinh,
                            HangSanXuat = item.VatTuBenhVien.VatTus.NhaSanXuat,
                            NuocSanXuat = item.VatTuBenhVien.VatTus.NuocSanXuat,
                            SoLuongYeuCau = item.SoLuong,
                            Nhom = item.LaVatTuBHYT == true ? "Vật Tư BHYT" : "Vật Tư Không BHYT",
                        })
                        .GroupBy(x => new
                        {
                            x.YeuCauLinhVatTuId,
                            x.VatTuBenhVienId,
                            x.LaBHYT,
                            x.Nhom,
                            x.DonViTinh,
                            x.HangSanXuat,
                            x.NuocSanXuat,
                            x.SoLuongYeuCau
                        })
                        .Select(item => new DSLinhVatChildTuGridVo()
                        {
                            YeuCauLinhVatTuId = long.Parse(queryString[0]),
                            VatTuBenhVienId = item.First().VatTuBenhVienId,
                            LaBHYT = item.First().LaBHYT,
                            TenVatTu = item.First().TenVatTu,
                            DonViTinh = item.First().DonViTinh,
                            HangSanXuat = item.First().HangSanXuat,
                            NuocSanXuat = item.First().NuocSanXuat,
                            SoLuongYeuCau = item.First().SoLuongYeuCau,
                            SoLuongTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                .Where(x => x.VatTuBenhVienId == item.First().VatTuBenhVienId
                                            && x.NhapKhoVatTu.KhoId == yeuCauLinhVatTu.KhoXuatId
                                            && x.NhapKhoVatTu.DaHet != true
                                            && x.LaVatTuBHYT == item.First().LaBHYT
                                            && x.SoLuongDaXuat < x.SoLuongNhap && x.HanSuDung >= DateTime.Now).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                            Nhom = item.First().Nhom
                        })
                        .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu).Distinct().ToList();
                }

                var countTask = queryable.AsQueryable().CountAsync();
                await Task.WhenAll(countTask);

                return new GridDataSource { TotalRowCount = countTask.Result };
            }
            return null;
        }
        #endregion
        private OBJListVatTu GetHTMLLinhBenhNhanTuChoi(List<VatTuGridVo> gridvos)
        {
            string sluongDaXuat = "";
            string ghiChu = "";
            string vatTu = "";
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
                vatTu = vatTu +
                                         "<tr style='border: 1px solid #020000;'>" +

                                        "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                        index++
                                        + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                       itemx.MaVatTu
                                        + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                        itemx.TenVatTu
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
            var data = new OBJListVatTu
            {
                Index = index,
                html = vatTu
            };
            return data;
        }
        private OBJListVatTu GetHTMLLinhBenhNhanDataoDaDuyet(List<YeuCauVatTuBenhVien> gridvos)
        {
            var yeucau = 0; // to do
            var thucChat = ""; // to do
            var tenLoaiLinh = "";
            var donViTinh = "";
            var maHoatChat = "";
            var vt = "";
            int index = 1;
            var infovatTus = gridvos.GroupBy(d => new { d.VatTuBenhVienId, d.DonViTinh })
                .Select(d => new {
                    Ma = d.First().Ma,
                    TenVatTu =d.First().Ten,
                    DonViTinh = d.First().DonViTinh,
                    SoLuong = d.Sum(f=>f.SoLuong),
                    TenLoaiLinh =d.First().LoaiPhieuLinh.GetDescription(),
                }).OrderBy(d=>d.TenVatTu).ToList();
            foreach (var itemx in infovatTus.ToList())
            {
                vt = vt +
                                          "<tr style='border: 1px solid #020000;'>" +
                                         "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                        index++
                                        + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                        itemx.Ma
                                        + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                        itemx.TenVatTu
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
            var data = new OBJListVatTu
            {
                Index = index,
                html = vt
            };
            return data;
        }
        private OBJListVatTu GetHTMLDuyet(List<VatTuDaTaoGridVo> gridVos)
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
                                       itemx.MaVatTu
                                        + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                        itemx.TenVatTu
                                        + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                       itemx.DonViTinh
                                        + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                        itemx.SoLuong
                                         + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                        yeuCau
                                        + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                        "&nbsp;";
            }
            var data = new OBJListVatTu
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

            var yeuCauVatTu = _yeuCauVatTuBenhVienRepository.TableNoTracking
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
            return yeuCauVatTu.ToList();
        }
        private double GetSoLuongTonVatTu(long vatTuBenhVienId, long khoXuatId, bool laVatTuBHYT)
        {
            return _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(o => o.VatTuBenhVienId == vatTuBenhVienId && o.LaVatTuBHYT == laVatTuBHYT && o.NhapKhoVatTu.KhoId == khoXuatId).Sum(o => o.SoLuongNhap - o.SoLuongDaXuat).MathRoundNumber();
        }
    }
}

