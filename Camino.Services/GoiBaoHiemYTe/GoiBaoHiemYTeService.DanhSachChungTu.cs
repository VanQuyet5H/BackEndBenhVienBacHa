using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.BHYT;
using Camino.Core.Domain.Entities.HamGuiHoSoWatchings;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BHYT;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.ExcelChungTu;
using Camino.Core.Domain.ValueObject.GiayChungNhanNghiViecHuongBHXH;
using Camino.Core.Domain.ValueObject.GoiBaoHiemYTe;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.HamGuiHoSoWatchings;
using Camino.Core.Helpers;
using Camino.Data;
using Camino.Services.CauHinh;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestSharp;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.GoiBaoHiemYTe
{
    [ScopedDependency(ServiceType = typeof(IGoiBaoHiemYTeService))]
    public partial class GoiBaoHiemYTeService : MasterFileService<YeuCauTiepNhan>, IGoiBaoHiemYTeService
    {
        public GridDataSource GetDataDanhSachXuatChungTuExcelForGrid(QueryInfo queryChungTuExcelInfo)
        {
            var queryInfo = JsonConvert.DeserializeObject<TimKiemThongTinBenhNhanXuatExcelChungTuQueryInfo>(queryChungTuExcelInfo.AdditionalSearchString);
            if (!string.IsNullOrEmpty(queryInfo.FromDate) || !string.IsNullOrEmpty(queryInfo.ToDate))
            {
                DateTime.TryParseExact(queryInfo.FromDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
                DateTime.TryParseExact(queryInfo.ToDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);
                queryInfo.TuNgay = tuNgay;
                queryInfo.DenNgay = denNgay;
            }

            queryInfo.LoaiChungTu = queryInfo.LoaiChungTu == null ? LoaiChungTuXuatExcel.GiayRaVien : queryInfo.LoaiChungTu;

            var queryNgoaiTru = BaseRepository.TableNoTracking
               .Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru &&
                           o.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                           o.CoBHYT == true &&
                          ((queryInfo.TuNgay == null || o.YeuCauKhamBenhs.Any(c => c.DuongThaiNgayIn >= queryInfo.TuNgay)) && (queryInfo.DenNgay == null || o.YeuCauKhamBenhs.Any(c => c.DuongThaiNgayIn < queryInfo.DenNgay)) ||
                           (queryInfo.TuNgay == null || o.YeuCauKhamBenhs.Any(c => c.NghiHuongBHXHNgayIn >= queryInfo.TuNgay)) && (queryInfo.DenNgay == null || o.YeuCauKhamBenhs.Any(c => c.NghiHuongBHXHNgayIn < queryInfo.DenNgay))));

            if (!string.IsNullOrEmpty(queryInfo.SearchString))
            {
                queryNgoaiTru = queryNgoaiTru.ApplyLike(queryInfo.SearchString, x => x.MaYeuCauTiepNhan, x => x.HoTen, x => x.BenhNhan.MaBN);
            }

            var ngoaiTruData = queryNgoaiTru.Select(o => new DanhSachChungTuXuatExcelData
            {
                Id = o.Id,
                MaNB = o.BenhNhan.MaBN,
                HoTen = o.HoTen,
                NgaySinh = o.NgaySinh,
                ThangSinh = o.ThangSinh,
                NamSinh = o.NamSinh,
                //NgayThangNam = DateHelper.DOBFormat(o.NgaySinh, o.ThangSinh, o.NamSinh),
                GioiTinh = o.GioiTinh,
                MaYeuCauTiepNhan = o.MaYeuCauTiepNhan,
                QuyetToanTheoNoiTru = o.QuyetToanTheoNoiTru,
                YeuCauKhamBenhThongTinDuongThais = o.YeuCauKhamBenhs
                                                    .Where(k => k.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham && k.BaoHiemChiTra != null && k.BaoHiemChiTra == true)
                                                    .Select(k => new YeuCauKhamBenhThongTinDuongThai
                                                    {
                                                        DuongThaiTuNgay = k.DuongThaiTuNgay,
                                                        DuongThaiDenNgay = k.DuongThaiDenNgay,
                                                        GhiChuICDChinh = k.GhiChuICDChinh,
                                                        DuongThaiNgayIn = k.DuongThaiNgayIn
                                                    }).ToList(),
                YeuCauKhamBenhThongTinNghiHuongBHXHs = o.YeuCauKhamBenhs
                                                    .Where(k => k.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham && k.BaoHiemChiTra != null && k.BaoHiemChiTra == true)
                                                    .Select(k => new YeuCauKhamBenhThongTinNghiHuongBHXH
                                                    {
                                                        NghiTuNgay = k.NghiHuongBHXHTuNgay,
                                                        NghiDenNgay = k.NghiHuongBHXHDenNgay,
                                                        GhiChuICDChinh = k.GhiChuICDChinh,
                                                        NghiHuongBHXHNgayIn = k.NghiHuongBHXHNgayIn,
                                                        PhuongPhapDieuTri = k.PhuongPhapDieuTriNghiHuongBHYT
                                                    }).ToList(),
                //ChanDoanGhiChu = o.YeuCauKhamBenhs.Where(yckb => yckb.DuongThaiTuNgay != null && yckb.DuongThaiDenNgay != null).Select(c => c.GhiChuICDChinh).FirstOrDefault(),
                //YeuCauKhamBenhIdCoDuongThais = o.YeuCauKhamBenhs.Where(yckb => yckb.DuongThaiTuNgay != null && yckb.DuongThaiDenNgay != null).Select(c => c.Id).ToList()
            }).ToList();

            var ngoaiTruIds = ngoaiTruData.Where(o => o.QuyetToanTheoNoiTru == true).Select(o => o.Id).ToList();


            var noiTruHoSoKhacQuery = _noiTruHoSoKhacRepository.TableNoTracking
                .Where(c => (queryInfo.TuNgay == null || c.ThoiDiemThucHien >= queryInfo.TuNgay) && (queryInfo.DenNgay == null || c.ThoiDiemThucHien < queryInfo.DenNgay));
            
            switch (queryInfo.LoaiChungTu)
            {
                case LoaiChungTuXuatExcel.GiayNghiHuongBHXH:
                    noiTruHoSoKhacQuery = noiTruHoSoKhacQuery.Where(c => c.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayChungNhanNghiViecHuongBHXH);
                    break;
                case LoaiChungTuXuatExcel.GiayRaVien:
                    noiTruHoSoKhacQuery = noiTruHoSoKhacQuery.Where(c => c.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayRaVien);
                    break;
                case LoaiChungTuXuatExcel.GiayNghiDuongThai:
                    noiTruHoSoKhacQuery = noiTruHoSoKhacQuery.Where(c => c.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayNghiDuongThai);
                    break;
                case LoaiChungTuXuatExcel.TomTatBenhAn:
                    noiTruHoSoKhacQuery = noiTruHoSoKhacQuery.Where(c => c.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.TomTatHoSoBenhAn);
                    break;
                case LoaiChungTuXuatExcel.GiayChungSinh:
                    noiTruHoSoKhacQuery = noiTruHoSoKhacQuery.Where(c => c.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayChungSinh);
                    break;
                default:
                    noiTruHoSoKhacQuery = noiTruHoSoKhacQuery.Where(c => c.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayRaVien);
                    break;
            }
            var noiTruHoSoKhacData = noiTruHoSoKhacQuery.Select(o => new
            {
                o.Id,
                o.YeuCauTiepNhanId,
                o.ThongTinHoSo,
                o.ThoiDiemThucHien
            }).ToList();

            var yeuCauTiepNhanNoiTruIds = noiTruHoSoKhacData.Select(o=>o.YeuCauTiepNhanId).Distinct().ToList();

            var queryNoiTru = BaseRepository.TableNoTracking
                .Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && yeuCauTiepNhanNoiTruIds.Contains(o.Id) &&
                            o.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy && o.NoiTruBenhAn.ThoiDiemRaVien != null &&
                             (o.CoBHYT == true) && o.NoiTruBenhAn != null); // && (ngoaiTruIds.Contains(o.YeuCauTiepNhanNgoaiTruCanQuyetToanId.GetValueOrDefault())) do giờ tìm theo chứng từ

            if (!string.IsNullOrEmpty(queryInfo.SearchString))
            {
                queryNoiTru = queryNoiTru.ApplyLike(queryInfo.SearchString, x => x.MaYeuCauTiepNhan, x => x.HoTen, x => x.BenhNhan.MaBN);
            }


            //switch (queryInfo.LoaiChungTu)
            //{
            //    case LoaiChungTuXuatExcel.GiayNghiHuongBHXH:
            //        queryNoiTru = queryNoiTru.Where(o => o.NoiTruHoSoKhacs.Any(c => c.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayChungNhanNghiViecHuongBHXH));
            //        break;
            //    case LoaiChungTuXuatExcel.GiayRaVien:
            //        queryNoiTru = queryNoiTru.Where(o => o.NoiTruHoSoKhacs.Any(c => c.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayRaVien));
            //        break;
            //    case LoaiChungTuXuatExcel.GiayNghiDuongThai:
            //        queryNoiTru = queryNoiTru.Where(o => o.NoiTruHoSoKhacs.Any(c => c.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayNghiDuongThai));
            //        break;
            //    case LoaiChungTuXuatExcel.TomTatBenhAn:
            //        queryNoiTru = queryNoiTru.Where(o => o.NoiTruHoSoKhacs.Any(c => c.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.TomTatHoSoBenhAn));
            //        break;
            //    case LoaiChungTuXuatExcel.GiayChungSinh:
            //        queryNoiTru = queryNoiTru.Where(o => o.NoiTruHoSoKhacs.Any(c => c.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayChungSinh));
            //        break;
            //    default:
            //        queryNoiTru = queryNoiTru.Where(o => o.NoiTruHoSoKhacs.Any(c => c.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayRaVien));
            //        break;
            //}

            var noiTruData = queryNoiTru.Select(o => new DanhSachChungTuXuatExcelData
            {
                Id = o.Id,
                MaNB = o.BenhNhan.MaBN,
                MaTN = o.MaYeuCauTiepNhan,
                HoTen = o.BenhNhan.HoTen,
                NamSinh = o.NamSinh,
                GioiTinh = o.GioiTinh,
                YeuCauTiepNhanNgoaiTruCanQuyetToanId = o.YeuCauTiepNhanNgoaiTruCanQuyetToanId,

                ThoiGianNhapVien = o.NoiTruBenhAn.ThoiDiemNhapVien,
                ThoiGianRaVien = o.NoiTruBenhAn.ThoiDiemRaVien,

                ChanDoanGhiChu = o.NoiTruBenhAn.ChanDoanChinhRaVienGhiChu,
                LoaiBenhAn = o.NoiTruBenhAn.LoaiBenhAn,
                ThongTinNoiTruPhieuDieuTris = o.NoiTruBenhAn.NoiTruPhieuDieuTris.Select(n => new ThongTinNoiTruPhieuDieuTri { NgayDieuTri = n.NgayDieuTri, ChanDoanChinhGhiChu = n.ChanDoanChinhGhiChu }).ToList(),
                ThongTinTongKetBenhAn = o.NoiTruBenhAn.ThongTinTongKetBenhAn,

                //GiayRaVien = queryInfo.LoaiChungTu == LoaiChungTuXuatExcel.GiayRaVien ? o.NoiTruHoSoKhacs.Where(c => c.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayRaVien).Select(c => c.ThongTinHoSo).FirstOrDefault() : string.Empty,
                //GiayChungNhanNghiViecHuongBHXH = queryInfo.LoaiChungTu == LoaiChungTuXuatExcel.GiayNghiHuongBHXH ? o.NoiTruHoSoKhacs.Where(c => c.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayChungNhanNghiViecHuongBHXH).Select(c => c.ThongTinHoSo).FirstOrDefault() : string.Empty,
                //GiayNghiDuongThai = queryInfo.LoaiChungTu == LoaiChungTuXuatExcel.GiayNghiDuongThai ? o.NoiTruHoSoKhacs.Where(c => c.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayNghiDuongThai).Select(c => c.ThongTinHoSo).FirstOrDefault() : string.Empty,
                //TomTatHoSoBenhAn = queryInfo.LoaiChungTu == LoaiChungTuXuatExcel.TomTatBenhAn ? o.NoiTruHoSoKhacs.Where(c => c.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.TomTatHoSoBenhAn).Select(c => c.ThongTinHoSo).FirstOrDefault() : string.Empty,
                //GiayChungSinh = queryInfo.LoaiChungTu == LoaiChungTuXuatExcel.GiayChungSinh ? o.NoiTruHoSoKhacs.Where(c => c.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayChungSinh).Select(c => c.ThongTinHoSo).FirstOrDefault() : string.Empty,


                //NgayTaoChungTu = o.NoiTruHoSoKhacs.Any() && queryInfo.LoaiChungTu == LoaiChungTuXuatExcel.GiayRaVien ?
                //                o.NoiTruHoSoKhacs.Where(c => c.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayRaVien).Select(c => c.ThoiDiemThucHien).FirstOrDefault() :
                //                queryInfo.LoaiChungTu == LoaiChungTuXuatExcel.GiayNghiHuongBHXH ?
                //                o.NoiTruHoSoKhacs.Where(c => c.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayChungNhanNghiViecHuongBHXH).Select(c => c.ThoiDiemThucHien).FirstOrDefault() :
                //                queryInfo.LoaiChungTu == LoaiChungTuXuatExcel.GiayNghiDuongThai ?
                //                o.NoiTruHoSoKhacs.Where(c => c.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayNghiDuongThai).Select(c => c.ThoiDiemThucHien).FirstOrDefault() :
                //                queryInfo.LoaiChungTu == LoaiChungTuXuatExcel.TomTatBenhAn ?
                //                o.NoiTruHoSoKhacs.Where(c => c.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.TomTatHoSoBenhAn).Select(c => c.ThoiDiemThucHien).FirstOrDefault() :
                //                queryInfo.LoaiChungTu == LoaiChungTuXuatExcel.GiayChungSinh ?
                //                o.NoiTruHoSoKhacs.Where(c => c.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayChungSinh).Select(c => c.ThoiDiemThucHien).FirstOrDefault() :
                //                (DateTime?)null

            }).ToList();

            foreach (var noiTru in noiTruData)
            {
                var noiTruHoSoKhac = noiTruHoSoKhacData.Where(o => o.YeuCauTiepNhanId == noiTru.Id).FirstOrDefault();
                noiTru.NgayTaoChungTu = noiTruHoSoKhac?.ThoiDiemThucHien;
                noiTru.GiayRaVien = String.Empty;
                noiTru.GiayChungNhanNghiViecHuongBHXH = String.Empty;
                noiTru.GiayNghiDuongThai = String.Empty;
                noiTru.TomTatHoSoBenhAn = String.Empty;
                noiTru.GiayChungSinh = String.Empty;
                switch (queryInfo.LoaiChungTu)
                {
                    case LoaiChungTuXuatExcel.GiayNghiHuongBHXH:
                        noiTru.GiayChungNhanNghiViecHuongBHXH = noiTruHoSoKhac?.ThongTinHoSo;
                        break;
                    case LoaiChungTuXuatExcel.GiayRaVien:
                        noiTru.GiayRaVien = noiTruHoSoKhac?.ThongTinHoSo;
                        break;
                    case LoaiChungTuXuatExcel.GiayNghiDuongThai:
                        noiTru.GiayNghiDuongThai = noiTruHoSoKhac?.ThongTinHoSo;
                        break;
                    case LoaiChungTuXuatExcel.TomTatBenhAn:
                        noiTru.TomTatHoSoBenhAn = noiTruHoSoKhac?.ThongTinHoSo;
                        break;
                    case LoaiChungTuXuatExcel.GiayChungSinh:
                        noiTru.GiayChungSinh = noiTruHoSoKhac?.ThongTinHoSo;
                        break;
                    default:
                        noiTru.GiayRaVien = noiTruHoSoKhac?.ThongTinHoSo;
                        break;
                }

                if (noiTru.LoaiBenhAn == LoaiBenhAn.SanKhoaMo || noiTru.LoaiBenhAn == LoaiBenhAn.SanKhoaThuong)
                {
                    if (noiTru.ThongTinNoiTruPhieuDieuTris != null && noiTru.ThongTinNoiTruPhieuDieuTris.Count > 0)
                    {
                        noiTru.ChanDoanGhiChu = noiTru.ThongTinNoiTruPhieuDieuTris.OrderByDescending(c => c.NgayDieuTri).FirstOrDefault().ChanDoanChinhGhiChu;
                    }
                }

                if (!string.IsNullOrEmpty(noiTru.ThongTinTongKetBenhAn))
                {
                    var thongTinTongKetBAJSON = JsonConvert.DeserializeObject<PhuongPhapDieuTriJSON>(noiTru.ThongTinTongKetBenhAn);
                    noiTru.PhuongPhapDieuTri = thongTinTongKetBAJSON.PhuongPhapDieuTri;
                }

                if (!string.IsNullOrEmpty(noiTru.GiayRaVien))
                {
                    var giayRaVienJson = JsonConvert.DeserializeObject<PhuongPhapDieuTriJSON>(noiTru.GiayRaVien);

                    noiTru.PhuongPhapDieuTri = giayRaVienJson.PhuongPhapDieuTri;
                    noiTru.ChanDoanGhiChu = giayRaVienJson.ChanDoan;
                }

                if (!string.IsNullOrEmpty(noiTru.GiayChungNhanNghiViecHuongBHXH))
                {
                    var giayChungNhanNghiViecHuongBHXH = JsonConvert.DeserializeObject<InPhieuGiayChungNhanNghiViecHuongBHXH>(noiTru.GiayChungNhanNghiViecHuongBHXH);
                    noiTru.PhuongPhapDieuTri = giayChungNhanNghiViecHuongBHXH.ChanDoanVaPhuongPhapDieuTri;
                }

                if (!string.IsNullOrEmpty(noiTru.TomTatHoSoBenhAn))
                {
                    var tomTatHoSoBenhAn = JsonConvert.DeserializeObject<TomTatHoSoBenhAnVo>(noiTru.TomTatHoSoBenhAn);
                    noiTru.PhuongPhapDieuTri = tomTatHoSoBenhAn.PpDieuTri;
                }
            }

            var returnData = new List<DanhSachChungTuXuatExcel>();
            var ngoaiTruVaNoiTrus = new List<long>();

            foreach (var yctn in ngoaiTruData)
            {
                var noiTru = noiTruData.FirstOrDefault(o => o.YeuCauTiepNhanNgoaiTruCanQuyetToanId == yctn.Id);
                if (queryInfo.LoaiChungTu == LoaiChungTuXuatExcel.GiayNghiDuongThai && noiTru == null)
                {
                    if (yctn.YeuCauKhamBenhThongTinDuongThais.Any(o => o.DuongThaiTuNgay != null && o.DuongThaiDenNgay != null))
                    {
                        var itemData = new DanhSachChungTuXuatExcel()
                        {
                            Id = yctn.Id,
                            MaYeuCauTiepNhan = yctn.MaYeuCauTiepNhan,
                            YeuCauTiepNhanNgoaiTruId = yctn.Id,
                            MaNB = yctn.MaNB,
                            ThoiGianTiepNhan = yctn.ThoiGianTiepNhan,
                            HoTen = yctn.HoTen,
                            NgayThangNam = yctn.NgayThangNam?.ToString(),
                            GioiTinh = yctn.GioiTinh,
                            ChanDoan = yctn.YeuCauKhamBenhThongTinDuongThais.First(o => o.DuongThaiTuNgay != null && o.DuongThaiDenNgay != null).GhiChuICDChinh,
                            NgayTaoChungTu = yctn.YeuCauKhamBenhThongTinDuongThais.First(o => o.DuongThaiTuNgay != null && o.DuongThaiDenNgay != null).DuongThaiNgayIn,
                        };
                        returnData.Add(itemData);
                    }
                }
                if (queryInfo.LoaiChungTu == LoaiChungTuXuatExcel.GiayNghiHuongBHXH && noiTru == null)
                {
                    if (yctn.YeuCauKhamBenhThongTinNghiHuongBHXHs.Any(o => o.NghiTuNgay != null && o.NghiDenNgay != null))
                    {
                        var itemData = new DanhSachChungTuXuatExcel()
                        {
                            Id = yctn.Id,
                            MaYeuCauTiepNhan = yctn.MaYeuCauTiepNhan,
                            YeuCauTiepNhanNgoaiTruId = yctn.Id,
                            MaNB = yctn.MaNB,
                            ThoiGianTiepNhan = yctn.ThoiGianTiepNhan,
                            HoTen = yctn.HoTen,
                            NgayThangNam = yctn.NgayThangNam?.ToString(),
                            GioiTinh = yctn.GioiTinh,
                            ChanDoan = yctn.YeuCauKhamBenhThongTinNghiHuongBHXHs.First(o => o.NghiTuNgay != null && o.NghiDenNgay != null).GhiChuICDChinh,
                            NgayTaoChungTu = yctn.YeuCauKhamBenhThongTinNghiHuongBHXHs.First(o => o.NghiTuNgay != null && o.NghiDenNgay != null).NghiHuongBHXHNgayIn,
                            PhuongPhapDieuTri = yctn.YeuCauKhamBenhThongTinNghiHuongBHXHs.First(o => o.NghiTuNgay != null && o.NghiDenNgay != null).PhuongPhapDieuTri,
                        };
                        returnData.Add(itemData);
                    }
                }
                else
                {

                    if (queryInfo.LoaiChungTu == LoaiChungTuXuatExcel.GiayRaVien && (noiTru == null || string.IsNullOrEmpty(noiTru.GiayRaVien)))
                        continue;
                    if (queryInfo.LoaiChungTu == LoaiChungTuXuatExcel.GiayNghiHuongBHXH && (noiTru == null || string.IsNullOrEmpty(noiTru.GiayChungNhanNghiViecHuongBHXH)))
                        continue;
                    if (queryInfo.LoaiChungTu == LoaiChungTuXuatExcel.TomTatBenhAn && (noiTru == null || string.IsNullOrEmpty(noiTru.TomTatHoSoBenhAn)))
                        continue;
                    if (queryInfo.LoaiChungTu == LoaiChungTuXuatExcel.GiayChungSinh && (noiTru == null || string.IsNullOrEmpty(noiTru.GiayChungSinh)))
                        continue;
                    if (queryInfo.LoaiChungTu == LoaiChungTuXuatExcel.GiayNghiDuongThai && (noiTru == null || string.IsNullOrEmpty(noiTru.GiayNghiDuongThai)))
                        continue;

                    if (noiTru != null)
                    {
                        ngoaiTruVaNoiTrus.Add(noiTru.Id);

                        yctn.ThoiGianNhapVien = noiTru.ThoiGianNhapVien;
                        yctn.ThoiGianRaVien = noiTru.ThoiGianRaVien;
                        yctn.PhuongPhapDieuTri = noiTru.PhuongPhapDieuTri;
                        yctn.ChanDoanGhiChu = noiTru.ChanDoanGhiChu;
                        yctn.Id = noiTru.Id;

                        yctn.YeuCauTiepNhanNoiTruId = noiTru.Id;
                        yctn.YeuCauTiepNhanNgoaiTruId = noiTru.YeuCauTiepNhanNgoaiTruCanQuyetToanId;
                        yctn.NgayTaoChungTu = noiTru.NgayTaoChungTu;

                        var itemData = new DanhSachChungTuXuatExcel()
                        {
                            Id = yctn.Id,

                            MaYeuCauTiepNhan = yctn.MaYeuCauTiepNhan,
                            YeuCauTiepNhanNoiTruId = yctn.YeuCauTiepNhanNoiTruId,
                            YeuCauTiepNhanNgoaiTruId = yctn.YeuCauTiepNhanNgoaiTruId,
                            NgayTaoChungTu = yctn.NgayTaoChungTu,
                            MaNB = yctn.MaNB,
                            ThoiGianTiepNhan = yctn.ThoiGianTiepNhan,

                            HoTen = yctn.HoTen,
                            NgayThangNam = yctn.NgayThangNam?.ToString(),
                            GioiTinh = yctn.GioiTinh,
                            ChanDoan = yctn.ChanDoanGhiChu,
                            PhuongPhapDieuTri = yctn.PhuongPhapDieuTri,
                            TuNgay = yctn.ThoiGianNhapVien,
                            DenNgay = yctn.ThoiGianRaVien,
                        };

                        returnData.Add(itemData);
                    }
                }
            }
            var dsNoiTruKhongCoNgoaiTrus = noiTruData.Where(nt => !ngoaiTruVaNoiTrus.Contains(nt.Id));
            foreach (var yctn in dsNoiTruKhongCoNgoaiTrus)
            {
                var noiTru = noiTruData.FirstOrDefault(o => o.Id == yctn.Id);
                if (queryInfo.LoaiChungTu == LoaiChungTuXuatExcel.GiayRaVien && (noiTru == null || string.IsNullOrEmpty(noiTru.GiayRaVien)))
                    continue;
                if (queryInfo.LoaiChungTu == LoaiChungTuXuatExcel.GiayNghiHuongBHXH && (noiTru == null || string.IsNullOrEmpty(noiTru.GiayChungNhanNghiViecHuongBHXH)))
                    continue;
                if (queryInfo.LoaiChungTu == LoaiChungTuXuatExcel.TomTatBenhAn && (noiTru == null || string.IsNullOrEmpty(noiTru.TomTatHoSoBenhAn)))
                    continue;
                if (queryInfo.LoaiChungTu == LoaiChungTuXuatExcel.GiayChungSinh && (noiTru == null || string.IsNullOrEmpty(noiTru.GiayChungSinh)))
                    continue;
                if (queryInfo.LoaiChungTu == LoaiChungTuXuatExcel.GiayNghiDuongThai && (noiTru == null || string.IsNullOrEmpty(noiTru.GiayNghiDuongThai)))
                    continue;

                if (noiTru != null)
                {
                    yctn.ThoiGianNhapVien = noiTru.ThoiGianNhapVien;
                    yctn.ThoiGianRaVien = noiTru.ThoiGianRaVien;
                    yctn.PhuongPhapDieuTri = noiTru.PhuongPhapDieuTri;
                    yctn.ChanDoanGhiChu = noiTru.ChanDoanGhiChu;
                    yctn.Id = noiTru.Id;
                    yctn.YeuCauTiepNhanNoiTruId = noiTru.Id;
                    yctn.YeuCauTiepNhanNgoaiTruId = noiTru.YeuCauTiepNhanNgoaiTruCanQuyetToanId;
                    yctn.NgayTaoChungTu = noiTru.NgayTaoChungTu;
                    var itemData = new DanhSachChungTuXuatExcel()
                    {
                        Id = yctn.Id,

                        MaYeuCauTiepNhan = yctn.MaYeuCauTiepNhan,

                        YeuCauTiepNhanNoiTruId = yctn.YeuCauTiepNhanNoiTruId,
                        YeuCauTiepNhanNgoaiTruId = yctn.YeuCauTiepNhanNgoaiTruId,

                        NgayTaoChungTu = yctn.NgayTaoChungTu,

                        MaNB = yctn.MaNB,
                        ThoiGianTiepNhan = yctn.ThoiGianTiepNhan,

                        HoTen = yctn.HoTen,
                        NgayThangNam = yctn.NgayThangNam?.ToString(),
                        GioiTinh = yctn.GioiTinh,
                        ChanDoan = yctn.ChanDoanGhiChu,
                        PhuongPhapDieuTri = yctn.PhuongPhapDieuTri,
                        TuNgay = yctn.ThoiGianNhapVien,
                        DenNgay = yctn.ThoiGianRaVien,
                    };
                    returnData.Add(itemData);
                }
            }

            returnData = returnData.Where(c => (queryInfo.TuNgay == null || c.NgayTaoChungTu >= queryInfo.TuNgay) && (queryInfo.DenNgay == null || c.NgayTaoChungTu < queryInfo.DenNgay)).ToList();

            return new GridDataSource
            {
                Data = returnData.OrderBy(o => o.NgayTaoChungTu).Skip(queryChungTuExcelInfo.Skip).Take(queryChungTuExcelInfo.Take).ToArray(),
                TotalRowCount = returnData.Count()
            };
        }
        public GridDataSource GetDataDanhSachXuatChungTuExcelForGridOld(QueryInfo queryChungTuExcelInfo)
        {
            var queryInfo = JsonConvert.DeserializeObject<TimKiemThongTinBenhNhanXuatExcelChungTuQueryInfo>(queryChungTuExcelInfo.AdditionalSearchString);
            if (!string.IsNullOrEmpty(queryInfo.FromDate) || !string.IsNullOrEmpty(queryInfo.ToDate))
            {
                DateTime.TryParseExact(queryInfo.FromDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
                DateTime.TryParseExact(queryInfo.ToDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);
                queryInfo.TuNgay = tuNgay;
                queryInfo.DenNgay = denNgay;
            }

            queryInfo.LoaiChungTu = queryInfo.LoaiChungTu == null ? LoaiChungTuXuatExcel.GiayRaVien : queryInfo.LoaiChungTu;

            var queryNgoaiTru = BaseRepository.TableNoTracking
               .Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru &&
                           o.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                           o.CoBHYT == true &&
                          ((queryInfo.TuNgay == null || o.YeuCauKhamBenhs.Any(c => c.DuongThaiNgayIn >= queryInfo.TuNgay)) && (queryInfo.DenNgay == null || o.YeuCauKhamBenhs.Any(c => c.DuongThaiNgayIn < queryInfo.DenNgay)) ||
                           (queryInfo.TuNgay == null || o.YeuCauKhamBenhs.Any(c => c.NghiHuongBHXHNgayIn >= queryInfo.TuNgay)) && (queryInfo.DenNgay == null || o.YeuCauKhamBenhs.Any(c => c.NghiHuongBHXHNgayIn < queryInfo.DenNgay))));

            if (!string.IsNullOrEmpty(queryInfo.SearchString))
            {
                queryNgoaiTru = queryNgoaiTru.ApplyLike(queryInfo.SearchString, x => x.MaYeuCauTiepNhan, x => x.HoTen, x => x.BenhNhan.MaBN);
            }

            var ngoaiTruData = queryNgoaiTru.Select(o => new DanhSachChungTuXuatExcelData
            {
                Id = o.Id,
                MaNB = o.BenhNhan.MaBN,
                HoTen = o.HoTen,
                NgaySinh = o.NgaySinh,
                ThangSinh = o.ThangSinh,
                NamSinh = o.NamSinh,
                //NgayThangNam = DateHelper.DOBFormat(o.NgaySinh, o.ThangSinh, o.NamSinh),
                GioiTinh = o.GioiTinh,
                MaYeuCauTiepNhan = o.MaYeuCauTiepNhan,
                QuyetToanTheoNoiTru = o.QuyetToanTheoNoiTru,
                YeuCauKhamBenhThongTinDuongThais = o.YeuCauKhamBenhs
                                                    .Where(k => k.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham && k.BaoHiemChiTra != null && k.BaoHiemChiTra == true)
                                                    .Select(k => new YeuCauKhamBenhThongTinDuongThai
                                                    {
                                                        DuongThaiTuNgay = k.DuongThaiTuNgay,
                                                        DuongThaiDenNgay = k.DuongThaiDenNgay,
                                                        GhiChuICDChinh = k.GhiChuICDChinh,
                                                        DuongThaiNgayIn = k.DuongThaiNgayIn
                                                    }).ToList(),
                YeuCauKhamBenhThongTinNghiHuongBHXHs = o.YeuCauKhamBenhs
                                                    .Where(k => k.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham && k.BaoHiemChiTra != null && k.BaoHiemChiTra == true)
                                                    .Select(k => new YeuCauKhamBenhThongTinNghiHuongBHXH
                                                    {
                                                        NghiTuNgay = k.NghiHuongBHXHTuNgay,
                                                        NghiDenNgay = k.NghiHuongBHXHDenNgay,
                                                        GhiChuICDChinh = k.GhiChuICDChinh,
                                                        NghiHuongBHXHNgayIn = k.NghiHuongBHXHNgayIn,
                                                        PhuongPhapDieuTri = k.PhuongPhapDieuTriNghiHuongBHYT
                                                    }).ToList(),
                //ChanDoanGhiChu = o.YeuCauKhamBenhs.Where(yckb => yckb.DuongThaiTuNgay != null && yckb.DuongThaiDenNgay != null).Select(c => c.GhiChuICDChinh).FirstOrDefault(),
                //YeuCauKhamBenhIdCoDuongThais = o.YeuCauKhamBenhs.Where(yckb => yckb.DuongThaiTuNgay != null && yckb.DuongThaiDenNgay != null).Select(c => c.Id).ToList()
            }).ToList();

            var ngoaiTruIds = ngoaiTruData.Where(o => o.QuyetToanTheoNoiTru == true).Select(o => o.Id).ToList();
            var queryNoiTru = BaseRepository.TableNoTracking
                .Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru &&
                            o.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy && o.NoiTruBenhAn.ThoiDiemRaVien != null &&
                             (o.CoBHYT == true) && o.NoiTruBenhAn != null); // && (ngoaiTruIds.Contains(o.YeuCauTiepNhanNgoaiTruCanQuyetToanId.GetValueOrDefault())) do giờ tìm theo chứng từ

            if (!string.IsNullOrEmpty(queryInfo.SearchString))
            {
                queryNoiTru = queryNoiTru.ApplyLike(queryInfo.SearchString, x => x.MaYeuCauTiepNhan, x => x.HoTen, x => x.BenhNhan.MaBN);
            }


            switch (queryInfo.LoaiChungTu)
            {
                case LoaiChungTuXuatExcel.GiayNghiHuongBHXH:
                    queryNoiTru = queryNoiTru.Where(o => o.NoiTruHoSoKhacs.Any(c => c.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayChungNhanNghiViecHuongBHXH));
                    break;
                case LoaiChungTuXuatExcel.GiayRaVien:
                    queryNoiTru = queryNoiTru.Where(o => o.NoiTruHoSoKhacs.Any(c => c.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayRaVien));
                    break;
                case LoaiChungTuXuatExcel.GiayNghiDuongThai:
                    queryNoiTru = queryNoiTru.Where(o => o.NoiTruHoSoKhacs.Any(c => c.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayNghiDuongThai));
                    break;
                case LoaiChungTuXuatExcel.TomTatBenhAn:
                    queryNoiTru = queryNoiTru.Where(o => o.NoiTruHoSoKhacs.Any(c => c.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.TomTatHoSoBenhAn));
                    break;
                case LoaiChungTuXuatExcel.GiayChungSinh:
                    queryNoiTru = queryNoiTru.Where(o => o.NoiTruHoSoKhacs.Any(c => c.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayChungSinh));
                    break;
                default:
                    queryNoiTru = queryNoiTru.Where(o => o.NoiTruHoSoKhacs.Any(c => c.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayRaVien));
                    break;
            }           
           
            var noiTruData = queryNoiTru.Select(o => new DanhSachChungTuXuatExcelData
            {
                Id = o.Id,
                MaNB = o.BenhNhan.MaBN,
                MaTN = o.MaYeuCauTiepNhan,
                HoTen = o.BenhNhan.HoTen,
                NamSinh = o.NamSinh,
                GioiTinh = o.GioiTinh,
                YeuCauTiepNhanNgoaiTruCanQuyetToanId = o.YeuCauTiepNhanNgoaiTruCanQuyetToanId,

                ThoiGianNhapVien = o.NoiTruBenhAn.ThoiDiemNhapVien,
                ThoiGianRaVien = o.NoiTruBenhAn.ThoiDiemRaVien,

                ChanDoanGhiChu = o.NoiTruBenhAn.ChanDoanChinhRaVienGhiChu,
                LoaiBenhAn = o.NoiTruBenhAn.LoaiBenhAn,
                ThongTinNoiTruPhieuDieuTris = o.NoiTruBenhAn.NoiTruPhieuDieuTris.Select(n => new ThongTinNoiTruPhieuDieuTri { NgayDieuTri = n.NgayDieuTri, ChanDoanChinhGhiChu = n.ChanDoanChinhGhiChu }).ToList(),
                ThongTinTongKetBenhAn = o.NoiTruBenhAn.ThongTinTongKetBenhAn,

                GiayRaVien = queryInfo.LoaiChungTu == LoaiChungTuXuatExcel.GiayRaVien ? o.NoiTruHoSoKhacs.Where(c => c.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayRaVien).Select(c => c.ThongTinHoSo).FirstOrDefault() : string.Empty,
                GiayChungNhanNghiViecHuongBHXH = queryInfo.LoaiChungTu == LoaiChungTuXuatExcel.GiayNghiHuongBHXH ? o.NoiTruHoSoKhacs.Where(c => c.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayChungNhanNghiViecHuongBHXH).Select(c => c.ThongTinHoSo).FirstOrDefault() : string.Empty,
                GiayNghiDuongThai = queryInfo.LoaiChungTu == LoaiChungTuXuatExcel.GiayNghiDuongThai ? o.NoiTruHoSoKhacs.Where(c => c.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayNghiDuongThai).Select(c => c.ThongTinHoSo).FirstOrDefault() : string.Empty,
                TomTatHoSoBenhAn = queryInfo.LoaiChungTu == LoaiChungTuXuatExcel.TomTatBenhAn ? o.NoiTruHoSoKhacs.Where(c => c.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.TomTatHoSoBenhAn).Select(c => c.ThongTinHoSo).FirstOrDefault() : string.Empty,
                GiayChungSinh = queryInfo.LoaiChungTu == LoaiChungTuXuatExcel.GiayChungSinh ? o.NoiTruHoSoKhacs.Where(c => c.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayChungSinh).Select(c => c.ThongTinHoSo).FirstOrDefault() : string.Empty,


                NgayTaoChungTu = o.NoiTruHoSoKhacs.Any() && queryInfo.LoaiChungTu == LoaiChungTuXuatExcel.GiayRaVien ?
                                o.NoiTruHoSoKhacs.Where(c => c.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayRaVien).Select(c => c.ThoiDiemThucHien).FirstOrDefault() :
                                queryInfo.LoaiChungTu == LoaiChungTuXuatExcel.GiayNghiHuongBHXH ?
                                o.NoiTruHoSoKhacs.Where(c => c.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayChungNhanNghiViecHuongBHXH).Select(c => c.ThoiDiemThucHien).FirstOrDefault() :
                                queryInfo.LoaiChungTu == LoaiChungTuXuatExcel.GiayNghiDuongThai ?
                                o.NoiTruHoSoKhacs.Where(c => c.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayNghiDuongThai).Select(c => c.ThoiDiemThucHien).FirstOrDefault() :
                                queryInfo.LoaiChungTu == LoaiChungTuXuatExcel.TomTatBenhAn ?
                                o.NoiTruHoSoKhacs.Where(c => c.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.TomTatHoSoBenhAn).Select(c => c.ThoiDiemThucHien).FirstOrDefault() :
                                queryInfo.LoaiChungTu == LoaiChungTuXuatExcel.GiayChungSinh ?
                                o.NoiTruHoSoKhacs.Where(c => c.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayChungSinh).Select(c => c.ThoiDiemThucHien).FirstOrDefault() :
                                (DateTime?)null

            }).ToList();

            foreach (var noiTru in noiTruData)
            {
                if (noiTru.LoaiBenhAn == LoaiBenhAn.SanKhoaMo || noiTru.LoaiBenhAn == LoaiBenhAn.SanKhoaThuong)
                {
                    if (noiTru.ThongTinNoiTruPhieuDieuTris != null && noiTru.ThongTinNoiTruPhieuDieuTris.Count > 0)
                    {
                        noiTru.ChanDoanGhiChu = noiTru.ThongTinNoiTruPhieuDieuTris.OrderByDescending(c => c.NgayDieuTri).FirstOrDefault().ChanDoanChinhGhiChu;
                    }
                }

                if (!string.IsNullOrEmpty(noiTru.ThongTinTongKetBenhAn))
                {
                    var thongTinTongKetBAJSON = JsonConvert.DeserializeObject<PhuongPhapDieuTriJSON>(noiTru.ThongTinTongKetBenhAn);
                    noiTru.PhuongPhapDieuTri = thongTinTongKetBAJSON.PhuongPhapDieuTri;
                }

                if (!string.IsNullOrEmpty(noiTru.GiayRaVien))
                {
                    var giayRaVienJson = JsonConvert.DeserializeObject<PhuongPhapDieuTriJSON>(noiTru.GiayRaVien);

                    noiTru.PhuongPhapDieuTri = giayRaVienJson.PhuongPhapDieuTri;
                    noiTru.ChanDoanGhiChu = giayRaVienJson.ChanDoan;
                }

                if (!string.IsNullOrEmpty(noiTru.GiayChungNhanNghiViecHuongBHXH))
                {
                    var giayChungNhanNghiViecHuongBHXH = JsonConvert.DeserializeObject<InPhieuGiayChungNhanNghiViecHuongBHXH>(noiTru.GiayChungNhanNghiViecHuongBHXH);
                    noiTru.PhuongPhapDieuTri = giayChungNhanNghiViecHuongBHXH.ChanDoanVaPhuongPhapDieuTri;
                }

                if (!string.IsNullOrEmpty(noiTru.TomTatHoSoBenhAn))
                {
                    var tomTatHoSoBenhAn = JsonConvert.DeserializeObject<TomTatHoSoBenhAnVo>(noiTru.TomTatHoSoBenhAn);
                    noiTru.PhuongPhapDieuTri = tomTatHoSoBenhAn.PpDieuTri;
                }
            }

            var returnData = new List<DanhSachChungTuXuatExcel>();
            var ngoaiTruVaNoiTrus = new List<long>();

            foreach (var yctn in ngoaiTruData)
            {
                var noiTru = noiTruData.FirstOrDefault(o => o.YeuCauTiepNhanNgoaiTruCanQuyetToanId == yctn.Id);
                if (queryInfo.LoaiChungTu == LoaiChungTuXuatExcel.GiayNghiDuongThai && noiTru == null)
                {
                    if (yctn.YeuCauKhamBenhThongTinDuongThais.Any(o => o.DuongThaiTuNgay != null && o.DuongThaiDenNgay != null))
                    {
                        var itemData = new DanhSachChungTuXuatExcel()
                        {
                            Id = yctn.Id,
                            MaYeuCauTiepNhan = yctn.MaYeuCauTiepNhan,
                            YeuCauTiepNhanNgoaiTruId = yctn.Id,
                            MaNB = yctn.MaNB,
                            ThoiGianTiepNhan = yctn.ThoiGianTiepNhan,
                            HoTen = yctn.HoTen,
                            NgayThangNam = yctn.NgayThangNam?.ToString(),
                            GioiTinh = yctn.GioiTinh,
                            ChanDoan = yctn.YeuCauKhamBenhThongTinDuongThais.First(o => o.DuongThaiTuNgay != null && o.DuongThaiDenNgay != null).GhiChuICDChinh,
                            NgayTaoChungTu = yctn.YeuCauKhamBenhThongTinDuongThais.First(o => o.DuongThaiTuNgay != null && o.DuongThaiDenNgay != null).DuongThaiNgayIn,
                        };
                        returnData.Add(itemData);
                    }
                }
                if (queryInfo.LoaiChungTu == LoaiChungTuXuatExcel.GiayNghiHuongBHXH && noiTru == null)
                {
                    if (yctn.YeuCauKhamBenhThongTinNghiHuongBHXHs.Any(o => o.NghiTuNgay != null && o.NghiDenNgay != null))
                    {
                        var itemData = new DanhSachChungTuXuatExcel()
                        {
                            Id = yctn.Id,
                            MaYeuCauTiepNhan = yctn.MaYeuCauTiepNhan,
                            YeuCauTiepNhanNgoaiTruId = yctn.Id,
                            MaNB = yctn.MaNB,
                            ThoiGianTiepNhan = yctn.ThoiGianTiepNhan,
                            HoTen = yctn.HoTen,
                            NgayThangNam = yctn.NgayThangNam?.ToString(),
                            GioiTinh = yctn.GioiTinh,
                            ChanDoan = yctn.YeuCauKhamBenhThongTinNghiHuongBHXHs.First(o => o.NghiTuNgay != null && o.NghiDenNgay != null).GhiChuICDChinh,
                            NgayTaoChungTu = yctn.YeuCauKhamBenhThongTinNghiHuongBHXHs.First(o => o.NghiTuNgay != null && o.NghiDenNgay != null).NghiHuongBHXHNgayIn,
                            PhuongPhapDieuTri = yctn.YeuCauKhamBenhThongTinNghiHuongBHXHs.First(o => o.NghiTuNgay != null && o.NghiDenNgay != null).PhuongPhapDieuTri,
                        };
                        returnData.Add(itemData);
                    }
                }
                else
                {

                    if (queryInfo.LoaiChungTu == LoaiChungTuXuatExcel.GiayRaVien && (noiTru == null || string.IsNullOrEmpty(noiTru.GiayRaVien)))
                        continue;
                    if (queryInfo.LoaiChungTu == LoaiChungTuXuatExcel.GiayNghiHuongBHXH && (noiTru == null || string.IsNullOrEmpty(noiTru.GiayChungNhanNghiViecHuongBHXH)))
                        continue;
                    if (queryInfo.LoaiChungTu == LoaiChungTuXuatExcel.TomTatBenhAn && (noiTru == null || string.IsNullOrEmpty(noiTru.TomTatHoSoBenhAn)))
                        continue;
                    if (queryInfo.LoaiChungTu == LoaiChungTuXuatExcel.GiayChungSinh && (noiTru == null || string.IsNullOrEmpty(noiTru.GiayChungSinh)))
                        continue;
                    if (queryInfo.LoaiChungTu == LoaiChungTuXuatExcel.GiayNghiDuongThai && (noiTru == null || string.IsNullOrEmpty(noiTru.GiayNghiDuongThai)))
                        continue;

                    if (noiTru != null)
                    {
                        ngoaiTruVaNoiTrus.Add(noiTru.Id);

                        yctn.ThoiGianNhapVien = noiTru.ThoiGianNhapVien;
                        yctn.ThoiGianRaVien = noiTru.ThoiGianRaVien;
                        yctn.PhuongPhapDieuTri = noiTru.PhuongPhapDieuTri;
                        yctn.ChanDoanGhiChu = noiTru.ChanDoanGhiChu;
                        yctn.Id = noiTru.Id;

                        yctn.YeuCauTiepNhanNoiTruId = noiTru.Id;
                        yctn.YeuCauTiepNhanNgoaiTruId = noiTru.YeuCauTiepNhanNgoaiTruCanQuyetToanId;
                        yctn.NgayTaoChungTu = noiTru.NgayTaoChungTu;

                        var itemData = new DanhSachChungTuXuatExcel()
                        {
                            Id = yctn.Id,

                            MaYeuCauTiepNhan = yctn.MaYeuCauTiepNhan,
                            YeuCauTiepNhanNoiTruId = yctn.YeuCauTiepNhanNoiTruId,
                            YeuCauTiepNhanNgoaiTruId = yctn.YeuCauTiepNhanNgoaiTruId,
                            NgayTaoChungTu = yctn.NgayTaoChungTu,
                            MaNB = yctn.MaNB,
                            ThoiGianTiepNhan = yctn.ThoiGianTiepNhan,

                            HoTen = yctn.HoTen,
                            NgayThangNam = yctn.NgayThangNam?.ToString(),
                            GioiTinh = yctn.GioiTinh,
                            ChanDoan = yctn.ChanDoanGhiChu,
                            PhuongPhapDieuTri = yctn.PhuongPhapDieuTri,
                            TuNgay = yctn.ThoiGianNhapVien,
                            DenNgay = yctn.ThoiGianRaVien,
                        };

                        returnData.Add(itemData);
                    }
                }
            }           
            var dsNoiTruKhongCoNgoaiTrus = noiTruData.Where(nt => !ngoaiTruVaNoiTrus.Contains(nt.Id));
            foreach (var yctn in dsNoiTruKhongCoNgoaiTrus)
            {
                var noiTru = noiTruData.FirstOrDefault(o => o.Id == yctn.Id);
                if (queryInfo.LoaiChungTu == LoaiChungTuXuatExcel.GiayRaVien && (noiTru == null || string.IsNullOrEmpty(noiTru.GiayRaVien)))
                    continue;
                if (queryInfo.LoaiChungTu == LoaiChungTuXuatExcel.GiayNghiHuongBHXH && (noiTru == null || string.IsNullOrEmpty(noiTru.GiayChungNhanNghiViecHuongBHXH)))
                    continue;
                if (queryInfo.LoaiChungTu == LoaiChungTuXuatExcel.TomTatBenhAn && (noiTru == null || string.IsNullOrEmpty(noiTru.TomTatHoSoBenhAn)))
                    continue;
                if (queryInfo.LoaiChungTu == LoaiChungTuXuatExcel.GiayChungSinh && (noiTru == null || string.IsNullOrEmpty(noiTru.GiayChungSinh)))
                    continue;
                if (queryInfo.LoaiChungTu == LoaiChungTuXuatExcel.GiayNghiDuongThai && (noiTru == null || string.IsNullOrEmpty(noiTru.GiayNghiDuongThai)))
                    continue;

                if (noiTru != null)
                {
                    yctn.ThoiGianNhapVien = noiTru.ThoiGianNhapVien;
                    yctn.ThoiGianRaVien = noiTru.ThoiGianRaVien;
                    yctn.PhuongPhapDieuTri = noiTru.PhuongPhapDieuTri;
                    yctn.ChanDoanGhiChu = noiTru.ChanDoanGhiChu;
                    yctn.Id = noiTru.Id;
                    yctn.YeuCauTiepNhanNoiTruId = noiTru.Id;
                    yctn.YeuCauTiepNhanNgoaiTruId = noiTru.YeuCauTiepNhanNgoaiTruCanQuyetToanId;
                    yctn.NgayTaoChungTu = noiTru.NgayTaoChungTu;
                    var itemData = new DanhSachChungTuXuatExcel()
                    {
                        Id = yctn.Id,

                        MaYeuCauTiepNhan = yctn.MaYeuCauTiepNhan,

                        YeuCauTiepNhanNoiTruId = yctn.YeuCauTiepNhanNoiTruId,
                        YeuCauTiepNhanNgoaiTruId = yctn.YeuCauTiepNhanNgoaiTruId,

                        NgayTaoChungTu = yctn.NgayTaoChungTu,

                        MaNB = yctn.MaNB,
                        ThoiGianTiepNhan = yctn.ThoiGianTiepNhan,

                        HoTen = yctn.HoTen,
                        NgayThangNam = yctn.NgayThangNam?.ToString(),
                        GioiTinh = yctn.GioiTinh,
                        ChanDoan = yctn.ChanDoanGhiChu,
                        PhuongPhapDieuTri = yctn.PhuongPhapDieuTri,
                        TuNgay = yctn.ThoiGianNhapVien,
                        DenNgay = yctn.ThoiGianRaVien,
                    };
                    returnData.Add(itemData);
                }
            }

            returnData = returnData.Where(c => (queryInfo.TuNgay == null || c.NgayTaoChungTu >= queryInfo.TuNgay) && (queryInfo.DenNgay == null || c.NgayTaoChungTu < queryInfo.DenNgay)).ToList();

            return new GridDataSource
            {
                Data = returnData.OrderBy(o => o.NgayTaoChungTu).Skip(queryChungTuExcelInfo.Skip).Take(queryChungTuExcelInfo.Take).ToArray(),
                TotalRowCount = returnData.Count()
            };
        }

        public long GetIdPhieuNoiTruHoSoKhac(long id, Enums.LoaiHoSoDieuTriNoiTru loai)
        {
            var query = _noiTruHoSoKhacRepository.TableNoTracking.Where(s => s.YeuCauTiepNhanId == id && s.LoaiHoSoDieuTriNoiTru == loai).Select(d => d.Id); ;
            return query.FirstOrDefault();
        }

    }
}
