using Camino.Core.Domain.ValueObject.BaoCao;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Services.ExportImport.Help;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Services.Helpers;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;


namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        public async Task<GridDataSource> GetBaoCaoSoPhucTrinhPhauThuatThuThuatForGridAsync(QueryInfo queryInfo)
        {
            var thongTinPhucTrinhs = new List<BaoCaoSoPhucTrinhPhauThuatThuThuatGridVo>();
            var timKiemNangCaoObj = new BaoCaoSoPhucTrinhPhauThuatThuTHuatQueryInfo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoSoPhucTrinhPhauThuatThuTHuatQueryInfo>(queryInfo.AdditionalSearchString);
            }

            DateTime? tuNgay = null;
            DateTime? denNgay = null;
            //kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.FromDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.FromDate)) //) || !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.FromDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
                tuNgay = tuNgayTemp;
            }

            if (timKiemNangCaoObj.ToDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.ToDate))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.ToDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                denNgay = denNgayTemp;
            }

            #region BVHD-3636
            //xử lý  khoa phòng
            var lstKhoaId = new List<long>();
            var lstPhongId = new List<long>();

            if (timKiemNangCaoObj.NoiThucHienIds.Any())
            {
                foreach (var item in timKiemNangCaoObj.NoiThucHienIds)
                {
                    var noiThucHien = JsonConvert.DeserializeObject<ItemNoiThucHienBaoCaoPTTTVo>(item);

                    if (noiThucHien.PhongId == null) // nơi thực hiện là khoa
                    {
                        lstKhoaId.Add(noiThucHien.KhoaId);
                    }
                    else
                    {
                        lstPhongId.Add(noiThucHien.PhongId.Value);
                    }
                }
            }
            else
            {
                lstKhoaId.AddRange(timKiemNangCaoObj.KhoaPhongIds.Select(x => long.Parse(x)).ToList());
            }


            //xử lý loại dịch vụ
            var layTatCaDichVuPTTT = !((timKiemNangCaoObj.LaPhauThuat == true && timKiemNangCaoObj.LaThuThuat != true) || (timKiemNangCaoObj.LaPhauThuat != true && timKiemNangCaoObj.LaThuThuat == true));
            #endregion

            if (tuNgay != null && denNgay != null)
            {
                var nhomThuThuatIds = new List<long>();
                var cauHinhNhomThuThuat = _cauHinhService.GetSetting("CauHinhBaoCao.NhomThuThuat");
                long.TryParse(cauHinhNhomThuThuat?.Value, out long nhomThuThuatId);
                var lstNhomDichVu = await _nhomDichVuBenhVienRepository.TableNoTracking
                    .Select(item => new NhomDichVuBenhVienTreeViewVo
                    {
                        KeyId = item.Id,
                        DisplayName = item.Ten,//item.Ma + " - " + item.Ten,
                        ParentId = item.NhomDichVuBenhVienChaId,
                        Ma = item.Ma,
                        IsDefault = item.IsDefault
                    })
                    .ToListAsync();
                GetFullNhomThuThuat(lstNhomDichVu, nhomThuThuatId, nhomThuThuatIds);

                thongTinPhucTrinhs = _yeuCauDichVuKyThuatRepository.TableNoTracking
                    .Where(x => x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat
                                && x.YeuCauDichVuKyThuatTuongTrinhPTTT != null
                                //&& x.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemPhauThuat != null
                                //&& x.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemPhauThuat >= tuNgay
                                //&& x.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemPhauThuat <= denNgay
                                //&& (timKiemNangCaoObj.KhoaId == null || x.NoiThucHien.KhoaPhongId == timKiemNangCaoObj.KhoaId))

                                //BVHD-3636
                                && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                && x.ThoiDiemThucHien != null
                                && x.ThoiDiemThucHien >= tuNgay
                                && x.ThoiDiemThucHien <= denNgay

                                && ((!lstKhoaId.Any() && !lstPhongId.Any())
                                    || (lstKhoaId.Any() && lstKhoaId.Contains(x.NoiThucHien.KhoaPhongId)) 
                                    || (lstPhongId.Any() && lstPhongId.Contains(x.NoiThucHienId.Value)))
                                
                                && (layTatCaDichVuPTTT 
                                    || (timKiemNangCaoObj.LaThuThuat == true && nhomThuThuatIds.Contains(x.NhomDichVuBenhVienId))
                                    || (timKiemNangCaoObj.LaPhauThuat == true && !nhomThuThuatIds.Contains(x.NhomDichVuBenhVienId))))
                    .Select(item => new BaoCaoSoPhucTrinhPhauThuatThuThuatGridVo
                    {
                        Id = item.Id,
                        YeuCauDichVuKyThuatTuongTrinhPTTTId = item.YeuCauDichVuKyThuatTuongTrinhPTTT.Id,
                        MaTN = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                        TenBenhNhan = item.YeuCauTiepNhan.HoTen,
                        NgaySinh = item.YeuCauTiepNhan.NgaySinh,
                        ThangSinh = item.YeuCauTiepNhan.ThangSinh,
                        NamSinh = item.YeuCauTiepNhan.NamSinh,
                        GioiTinh = item.YeuCauTiepNhan.GioiTinh.GetDescription(),
                        DiaChi = item.YeuCauTiepNhan.DiaChiDayDu,
                        BHYT = item.YeuCauTiepNhan.BHYTMaSoThe,
                        ChuanDoanTruocPt = item.YeuCauDichVuKyThuatTuongTrinhPTTT.ICDTruocPhauThuat != null 
                            ? $"{item.YeuCauDichVuKyThuatTuongTrinhPTTT.ICDTruocPhauThuat.Ma} - {item.YeuCauDichVuKyThuatTuongTrinhPTTT.ICDTruocPhauThuat.TenTiengViet}" : string.Empty,
                        ChuanDoanSauPt = item.YeuCauDichVuKyThuatTuongTrinhPTTT.ICDSauPhauThuat != null
                            ? $"{item.YeuCauDichVuKyThuatTuongTrinhPTTT.ICDSauPhauThuat.Ma} - {item.YeuCauDichVuKyThuatTuongTrinhPTTT.ICDSauPhauThuat.TenTiengViet}" : string.Empty,
                        PhuongPhapPhauThuat = item.TenDichVu,
                        //!string.IsNullOrEmpty(item.YeuCauDichVuKyThuatTuongTrinhPTTT.TenPhuongPhapPTTT) 
                        //    ? item.YeuCauDichVuKyThuatTuongTrinhPTTT.TenPhuongPhapPTTT.Replace("|", "; ") : string.Empty,

                        //BVHD-3636
                        ThuThuatChuyenKhoa = item.NhomDichVuBenhVien.Ten,
                        SoThuTuThongTu50 = item.DichVuKyThuatBenhVien.ThongTu,

                        PhuongPhapVoCam = item.YeuCauDichVuKyThuatTuongTrinhPTTT.PhuongPhapVoCam != null 
                            ? item.YeuCauDichVuKyThuatTuongTrinhPTTT.PhuongPhapVoCam.Ten : string.Empty,
                        NhomDichVu = nhomThuThuatIds.Contains(item.NhomDichVuBenhVienId) ? "Thủ thuật" : "Phẫu thuật",
                        SoLuong = item.SoLan,
                        DonGia = item.Gia,
                        ThoiGianBatDauPhauThuat = item.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemPhauThuat,
                        ThoiGianKetThucPhauThuat = item.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemKetThucPhauThuat,
                        ThoiGianBatDauGayMe = item.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiGianBatDauGayMe,
                        LoaiPTTT = item.DichVuKyThuatBenhVien.LoaiPhauThuatThuThuat, // item.YeuCauDichVuKyThuatTuongTrinhPTTT.LoaiPhauThuatThuThuat.GetDescription(),
                        NoiChiDinh = item.NoiChiDinh.Ten,
                        NoiThucHien = item.NoiThucHien.Ten,
                    })
                    .OrderBy(x => x.ThoiGianBatDauPhauThuat)
                    .Skip(queryInfo.Skip).Take(queryInfo.Take)
                    .ToList();

                var lstTuongTrinhId = thongTinPhucTrinhs.Select(x => x.YeuCauDichVuKyThuatTuongTrinhPTTTId).ToList();
                var lstEkipBacSi = _phauThuatThuThuatEkipBacSiRepository.TableNoTracking
                    .Where(x => lstTuongTrinhId.Contains(x.YeuCauDichVuKyThuatTuongTrinhPTTTId) 
                                && x.VaiTroBacSi != null)
                    .Select(item => new BaoCaoSoPhucTrinhPhauThuatThuThuatEkipVo()
                    {
                        YeuCauDichVuKyThuatTuongTrinhPTTTId = item.YeuCauDichVuKyThuatTuongTrinhPTTTId,
                        HoTen = item.NhanVien.User.HoTen,
                        VaiTroBacSi = item.VaiTroBacSi
                    }).ToList();
                var lstEkipDieuDuong = _phauThuatThuThuatEkipDieuDuongRepository.TableNoTracking
                    .Where(x => lstTuongTrinhId.Contains(x.YeuCauDichVuKyThuatTuongTrinhPTTTId)
                                && x.VaiTroDieuDuong != null)
                    .Select(item => new BaoCaoSoPhucTrinhPhauThuatThuThuatEkipVo()
                    {
                        YeuCauDichVuKyThuatTuongTrinhPTTTId = item.YeuCauDichVuKyThuatTuongTrinhPTTTId,
                        HoTen = item.NhanVien.User.HoTen,
                        VaiTroDieuDuong = item.VaiTroDieuDuong
                    }).ToList();


                foreach (var phucTrinh in thongTinPhucTrinhs)
                {
                    var lstEkipBacSiTheoPhucTrinh = lstEkipBacSi.Where(x => x.YeuCauDichVuKyThuatTuongTrinhPTTTId == phucTrinh.YeuCauDichVuKyThuatTuongTrinhPTTTId).ToList();
                    var lstEkipDieuDuongTheoPhucTrinh = lstEkipDieuDuong.Where(x => x.YeuCauDichVuKyThuatTuongTrinhPTTTId == phucTrinh.YeuCauDichVuKyThuatTuongTrinhPTTTId).ToList();

                    phucTrinh.PTVChinh = string.Join(", ", lstEkipBacSiTheoPhucTrinh.Where(x => x.VaiTroBacSi == Enums.EnumVaiTroBacSi.PhauThuatVienChinh).Select(x => x.HoTen));
                    phucTrinh.NguoiGayMeChinh = string.Join(", ", lstEkipBacSiTheoPhucTrinh.Where(x => x.VaiTroBacSi == Enums.EnumVaiTroBacSi.GayMeTeChinh).Select(x => x.HoTen));
                    phucTrinh.NguoiGayMePhu = string.Join(", ", lstEkipBacSiTheoPhucTrinh.Where(x => x.VaiTroBacSi == Enums.EnumVaiTroBacSi.GayMeTePhu).Select(x => x.HoTen));
                    phucTrinh.PTVPhu = string.Join(", ", lstEkipDieuDuongTheoPhucTrinh.Where(x => x.VaiTroDieuDuong == Enums.EnumVaiTroDieuDuong.PhuPhauThuat).Select(x => x.HoTen));
                    phucTrinh.DungCuVongTrong = string.Join(", ", lstEkipDieuDuongTheoPhucTrinh.Where(x => x.VaiTroDieuDuong == Enums.EnumVaiTroDieuDuong.DungCuVongTrong).Select(x => x.HoTen));
                    phucTrinh.DungCuVongNgoai = string.Join(", ", lstEkipDieuDuongTheoPhucTrinh.Where(x => x.VaiTroDieuDuong == Enums.EnumVaiTroDieuDuong.DungCuVongNgoai).Select(x => x.HoTen));
                    phucTrinh.NguoiGayMeGayTePhu = string.Join(", ", lstEkipDieuDuongTheoPhucTrinh.Where(x => x.VaiTroDieuDuong == Enums.EnumVaiTroDieuDuong.PhuMePhuTe).Select(x => x.HoTen));
                    phucTrinh.ChayNgoai = string.Join(", ", lstEkipDieuDuongTheoPhucTrinh.Where(x => x.VaiTroDieuDuong == Enums.EnumVaiTroDieuDuong.ChayNgoai).Select(x => x.HoTen));
                    phucTrinh.Phu1 = string.Join(", ", lstEkipDieuDuongTheoPhucTrinh.Where(x => x.VaiTroDieuDuong == Enums.EnumVaiTroDieuDuong.Phu1).Select(x => x.HoTen));
                    phucTrinh.Phu2 = string.Join(", ", lstEkipDieuDuongTheoPhucTrinh.Where(x => x.VaiTroDieuDuong == Enums.EnumVaiTroDieuDuong.Phu2).Select(x => x.HoTen));
                    phucTrinh.Phu3 = string.Join(", ", lstEkipDieuDuongTheoPhucTrinh.Where(x => x.VaiTroDieuDuong == Enums.EnumVaiTroDieuDuong.Phu3).Select(x => x.HoTen));
                }
            }

            return new GridDataSource
            {
                Data = thongTinPhucTrinhs.ToArray(),
                TotalRowCount = thongTinPhucTrinhs.Count()
            };
        }

        public async Task<GridDataSource> GetTotalBaoCaoSoPhucTrinhPhauThuatThuThuatForGridAsync(QueryInfo queryInfo)
        {
            var timKiemNangCaoObj = new BaoCaoSoPhucTrinhPhauThuatThuTHuatQueryInfo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoSoPhucTrinhPhauThuatThuTHuatQueryInfo>(queryInfo.AdditionalSearchString);
            }

            DateTime? tuNgay = null;
            DateTime? denNgay = null;
            //kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.FromDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.FromDate)) //) || !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.FromDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
                tuNgay = tuNgayTemp;
            }

            if (timKiemNangCaoObj.ToDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.ToDate))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.ToDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                denNgay = denNgayTemp;
            }

            #region BVHD-3636
            //xử lý  khoa phòng
            var lstKhoaId = new List<long>();
            var lstPhongId = new List<long>();

            if (timKiemNangCaoObj.NoiThucHienIds.Any())
            {
                foreach (var item in timKiemNangCaoObj.NoiThucHienIds)
                {
                    var noiThucHien = JsonConvert.DeserializeObject<ItemNoiThucHienBaoCaoPTTTVo>(item);

                    if (noiThucHien.PhongId == null) // nơi thực hiện là khoa
                    {
                        lstKhoaId.Add(noiThucHien.KhoaId);
                    }
                    else
                    {
                        lstPhongId.Add(noiThucHien.PhongId.Value);
                    }
                }
            }
            else
            {
                lstKhoaId.AddRange(timKiemNangCaoObj.KhoaPhongIds.Select(x => long.Parse(x)).ToList());
            }


            //xử lý loại dịch vụ
            var layTatCaDichVuPTTT = !((timKiemNangCaoObj.LaPhauThuat == true && timKiemNangCaoObj.LaThuThuat != true) || (timKiemNangCaoObj.LaPhauThuat != true && timKiemNangCaoObj.LaThuThuat == true));
            #endregion

            if (tuNgay != null && denNgay != null)
            {
                var nhomThuThuatIds = new List<long>();
                var cauHinhNhomThuThuat = _cauHinhService.GetSetting("CauHinhBaoCao.NhomThuThuat");
                long.TryParse(cauHinhNhomThuThuat?.Value, out long nhomThuThuatId);
                var lstNhomDichVu = await _nhomDichVuBenhVienRepository.TableNoTracking
                    .Select(item => new NhomDichVuBenhVienTreeViewVo
                    {
                        KeyId = item.Id,
                        DisplayName = item.Ten,//item.Ma + " - " + item.Ten,
                        ParentId = item.NhomDichVuBenhVienChaId,
                        Ma = item.Ma,
                        IsDefault = item.IsDefault
                    })
                    .ToListAsync();
                GetFullNhomThuThuat(lstNhomDichVu, nhomThuThuatId, nhomThuThuatIds);

                var thongTinPhucTrinhs = _yeuCauDichVuKyThuatRepository.TableNoTracking
                    .Where(x => x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat
                                && x.YeuCauDichVuKyThuatTuongTrinhPTTT != null
                                //&& x.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemPhauThuat != null
                                //&& x.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemPhauThuat >= tuNgay
                                //&& x.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemPhauThuat <= denNgay
                                //&& (timKiemNangCaoObj.KhoaId == null || x.NoiThucHien.KhoaPhongId == timKiemNangCaoObj.KhoaId))

                                //BVHD-3636
                                && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                && x.ThoiDiemThucHien != null
                                && x.ThoiDiemThucHien >= tuNgay
                                && x.ThoiDiemThucHien <= denNgay

                                && ((!lstKhoaId.Any() && !lstPhongId.Any())
                                    || (lstKhoaId.Any() && lstKhoaId.Contains(x.NoiThucHien.KhoaPhongId))
                                    || (lstPhongId.Any() && lstPhongId.Contains(x.NoiThucHienId.Value)))

                                && (layTatCaDichVuPTTT
                                    || (timKiemNangCaoObj.LaThuThuat == true && nhomThuThuatIds.Contains(x.NhomDichVuBenhVienId))
                                    || (timKiemNangCaoObj.LaPhauThuat == true && !nhomThuThuatIds.Contains(x.NhomDichVuBenhVienId))))
                    .Select(item => new BaoCaoSoPhucTrinhPhauThuatThuThuatGridVo
                    {
                        Id = item.Id
                    });
                var countTask = thongTinPhucTrinhs.CountAsync();
                await Task.WhenAll(countTask);

                return new GridDataSource { TotalRowCount = countTask.Result };
            }

            return new GridDataSource { TotalRowCount = 0 };
        }

        public virtual byte[] ExportBaoCaoSoPhucTrinhPhauThuatThuThuatGridVo(GridDataSource gridDataSource, QueryInfo query)
        {
            var timKiemNangCaoObj = new BaoCaoSoPhucTrinhPhauThuatThuTHuatQueryInfo();
            if (!string.IsNullOrEmpty(query.AdditionalSearchString) && query.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoSoPhucTrinhPhauThuatThuTHuatQueryInfo>(query.AdditionalSearchString);
            }

            DateTime? tuNgay = null;
            DateTime? denNgay = null;
            //kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.FromDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.FromDate)) //) || !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.FromDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
                tuNgay = tuNgayTemp;
            }

            if (timKiemNangCaoObj.ToDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.ToDate))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.ToDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                denNgay = denNgayTemp;
            }

            var tenKhoa = "Tất cả";
            //if (timKiemNangCaoObj.KhoaId != null)
            //{
            //    var khoa = _KhoaPhongRepository.TableNoTracking.FirstOrDefault(x => x.Id == timKiemNangCaoObj.KhoaId);
            //    if (khoa != null)
            //    {
            //        tenKhoa = khoa?.Ten;
            //    }
            //}
            #region BVHD-3636
            //xử lý  khoa phòng
            var lstKhoaId = new List<long>();
            if (timKiemNangCaoObj.NoiThucHienIds.Any())
            {
                foreach (var item in timKiemNangCaoObj.NoiThucHienIds)
                {
                    var noiThucHien = JsonConvert.DeserializeObject<ItemNoiThucHienBaoCaoPTTTVo>(item);
                    lstKhoaId.Add(noiThucHien.KhoaId);
                }
            }
            else
            {
                lstKhoaId.AddRange(timKiemNangCaoObj.KhoaPhongIds.Select(x => long.Parse(x)).ToList());
            }

            lstKhoaId = lstKhoaId.Distinct().ToList();
            if (lstKhoaId.Any())
            {
                var lstTenKhoa = _KhoaPhongRepository.TableNoTracking
                    .Where(x => lstKhoaId.Contains(x.Id))
                    .Select(x => x.Ten)
                    .Distinct().ToList();
                tenKhoa = string.Join(", ", lstTenKhoa);
            }

            #endregion

            var datas = (ICollection<BaoCaoSoPhucTrinhPhauThuatThuThuatGridVo>)gridDataSource.Data;
            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<BaoCaoSoPhucTrinhPhauThuatThuThuatGridVo>("STT", p => ind++)
            };
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BÁO CÁO SỔ PHÚC TRÌNH PHẪU THUẬT/ THỦ THUẬT");

                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 15;
                    worksheet.Column(3).Width = 30;
                    worksheet.Column(4).Width = 15;
                    worksheet.Column(5).Width = 10;
                    worksheet.Column(6).Width = 40;
                    worksheet.Column(7).Width = 20;
                    worksheet.Column(8).Width = 30;
                    worksheet.Column(9).Width = 30;
                    worksheet.Column(10).Width = 30;
                    worksheet.Column(11).Width = 30;
                    worksheet.Column(12).Width = 30;

                    worksheet.Column(13).Width = 30;
                    worksheet.Column(14).Width = 20;
                    worksheet.Column(15).Width = 5;
                    worksheet.Column(16).Width = 15;
                    worksheet.Column(17).Width = 20;
                    worksheet.Column(18).Width = 20;
                    worksheet.Column(19).Width = 20;
                    worksheet.Column(20).Width = 25;
                    worksheet.Column(21).Width = 25;
                    worksheet.Column(22).Width = 25;
                    worksheet.Column(23).Width = 25;
                    worksheet.Column(24).Width = 25;
                    worksheet.Column(25).Width = 25;
                    worksheet.Column(26).Width = 25;
                    worksheet.Column(27).Width = 25;
                    worksheet.Column(28).Width = 25;
                    worksheet.Column(29).Width = 25;
                    worksheet.Column(30).Width = 25;
                    worksheet.Column(31).Width = 25;
                    worksheet.Column(32).Width = 25;
                    worksheet.Column(33).Width = 25;
                    worksheet.DefaultColWidth = 7;


                    #region // default range column bác sĩ, điều dưỡng
                    var arrColumnDefault = new string[] {"W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF", "AG" };
                    var lstColumnUse = new List<ExcelBaoCaoSoPhucTrinhPhauThuatThuThuatEkipVo>();

                    #endregion

                    //SET title BV
                    using (var range = worksheet.Cells["A1:E1"])
                    {
                        range.Worksheet.Cells["A1:E1"].Merge = true;
                        range.Worksheet.Cells["A1:E1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                        range.Worksheet.Cells["A1:E1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A1:E1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1:E1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:E1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:E1"].Style.Font.Bold = true;
                    }

                    // SET title head cho bảng excel
                    using (var range = worksheet.Cells["A3:L3"])
                    {
                        range.Worksheet.Cells["A3:L3"].Merge = true;
                        range.Worksheet.Cells["A3:L3"].Value = "BÁO CÁO SỔ PHÚC TRÌNH PHẪU THUẬT/ THỦ THUẬT";
                        range.Worksheet.Cells["A3:L3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:L3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:L3"].Style.Font.SetFromFont(new Font("Times New Roman", 16));
                        range.Worksheet.Cells["A3:L3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:L3"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A4:L4"])
                    {
                        range.Worksheet.Cells["A4:L4"].Merge = true;
                        range.Worksheet.Cells["A4:L4"].Value = $"Khoa: {tenKhoa}";
                        range.Worksheet.Cells["A4:L4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:L4"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A4:L4"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A4:L4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:L4"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A5:L5"])
                    {
                        range.Worksheet.Cells["A5:L5"].Merge = true;
                        range.Worksheet.Cells["A5:L5"].Value = "Từ ngày: " + tuNgay?.FormatNgayGioTimKiemTrenBaoCao()
                                                     + " - đến ngày: " + denNgay?.FormatNgayGioTimKiemTrenBaoCao();
                        range.Worksheet.Cells["A5:L5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A5:L5"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A5:L5"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A5:L5"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A5:L5"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A7:V7"])
                    {
                        range.Worksheet.Cells["A7:V7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A7:V7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A7:V7"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A7:V7"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A7:V7"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A7:V7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A8:V8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A8:V8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A8:V8"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A8:V8"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A8:V8"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A8:V8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A7:A8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A7:A8"].Merge = true;
                        range.Worksheet.Cells["A7:A8"].Value = "STT";

                        range.Worksheet.Cells["B7:B8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B7:B8"].Merge = true;
                        range.Worksheet.Cells["B7:B8"].Value = "Mã TN";

                        range.Worksheet.Cells["C7:C8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C7:C8"].Merge = true;
                        range.Worksheet.Cells["C7:C8"].Value = "Họ tên NB";

                        range.Worksheet.Cells["D7:D8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D7:D8"].Merge = true;
                        range.Worksheet.Cells["D7:D8"].Value = "Ngày sinh";

                        range.Worksheet.Cells["E7:E8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E7:E8"].Merge = true;
                        range.Worksheet.Cells["E7:E8"].Value = "Giới tính";

                        range.Worksheet.Cells["F7:F8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F7:F8"].Merge = true;
                        range.Worksheet.Cells["F7:F8"].Value = "Địa chỉ";

                        range.Worksheet.Cells["G7:G8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G7:G8"].Merge = true;
                        range.Worksheet.Cells["G7:G8"].Value = "BHYT";

                        range.Worksheet.Cells["H7:I7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H7:I7"].Merge = true;
                        range.Worksheet.Cells["H7:I7"].Value = "Chuẩn đoán";
                        range.Worksheet.Cells["H8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["H8"].Value = "Trước PT";
                        range.Worksheet.Cells["I8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["I8"].Value = "Sau PT";

                        range.Worksheet.Cells["J7:J8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["J7:J8"].Merge = true;
                        range.Worksheet.Cells["J7:J8"].Value = "Phương pháp phẫu thuật";

                        range.Worksheet.Cells["K7:K8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["K7:K8"].Merge = true;
                        range.Worksheet.Cells["K7:K8"].Value = "Thủ thuật chuyên khoa";

                        range.Worksheet.Cells["L7:L8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["L7:L8"].Merge = true;
                        range.Worksheet.Cells["L7:L8"].Value = "STT Theo thông tư 50";

                        range.Worksheet.Cells["M7:M8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["M7:M8"].Merge = true;
                        range.Worksheet.Cells["M7:M8"].Value = "Phương pháp vô cảm";

                        range.Worksheet.Cells["N7:N8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["N7:N8"].Merge = true;
                        range.Worksheet.Cells["N7:N8"].Value = "Nhóm DV";

                        range.Worksheet.Cells["O7:O8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["O7:O8"].Merge = true;
                        range.Worksheet.Cells["O7:O8"].Value = "SL";

                        range.Worksheet.Cells["P7:P8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["P7:P8"].Merge = true;
                        range.Worksheet.Cells["P7:P8"].Value = "Đơn giá";

                        range.Worksheet.Cells["Q7:R7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["Q7:R7"].Merge = true;
                        range.Worksheet.Cells["Q7:R7"].Value = "Thời gian phẫu thuật";
                        range.Worksheet.Cells["Q8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["Q8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["Q8"].Value = "Bắt đầu";
                        range.Worksheet.Cells["R8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["R8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["R8"].Value = "Kết thúc";

                        range.Worksheet.Cells["S7:S8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["S7:S8"].Merge = true;
                        range.Worksheet.Cells["S7:S8"].Value = "Thời gian khởi mê";

                        range.Worksheet.Cells["T7:T8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["T7:T8"].Merge = true;
                        range.Worksheet.Cells["T7:T8"].Value = "Loại PTTT";

                        range.Worksheet.Cells["U7:U8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["U7:U8"].Merge = true;
                        range.Worksheet.Cells["U7:U8"].Value = "Nơi chỉ đinh";

                        range.Worksheet.Cells["V7:V8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["V7:V8"].Merge = true;
                        range.Worksheet.Cells["V7:V8"].Value = "Nơi thực hiện";

                        #region tự động hiển thị column theo data
                        if (datas.Any(x => !string.IsNullOrEmpty(x.PTVChinh)))
                        {
                            var nameCell = arrColumnDefault[0];
                            lstColumnUse.Add(new ExcelBaoCaoSoPhucTrinhPhauThuatThuThuatEkipVo()
                            {
                                CellName = nameCell,
                                VaiTroBacSi = Enums.EnumVaiTroBacSi.PhauThuatVienChinh
                            });
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.Font.Bold = true;
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Merge = true;
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Value = "PTV chính";
                            arrColumnDefault = arrColumnDefault.Where(x => x != nameCell).ToArray();
                        }

                        if (datas.Any(x => !string.IsNullOrEmpty(x.NguoiGayMeChinh)))
                        {
                            var nameCell = arrColumnDefault[0];
                            lstColumnUse.Add(new ExcelBaoCaoSoPhucTrinhPhauThuatThuThuatEkipVo()
                            {
                                CellName = nameCell,
                                VaiTroBacSi = Enums.EnumVaiTroBacSi.GayMeTeChinh
                            });
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.Font.Bold = true;
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Merge = true;
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Value = "Gây mê/ tê chính";
                            arrColumnDefault = arrColumnDefault.Where(x => x != nameCell).ToArray();
                        }

                        if (datas.Any(x => !string.IsNullOrEmpty(x.NguoiGayMePhu)))
                        {
                            var nameCell = arrColumnDefault[0];
                            lstColumnUse.Add(new ExcelBaoCaoSoPhucTrinhPhauThuatThuThuatEkipVo()
                            {
                                CellName = nameCell,
                                VaiTroBacSi = Enums.EnumVaiTroBacSi.GayMeTePhu
                            });
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.Font.Bold = true;
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Merge = true;
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Value = "Gây mê/ tê phụ";
                            arrColumnDefault = arrColumnDefault.Where(x => x != nameCell).ToArray();
                        }

                        if (datas.Any(x => !string.IsNullOrEmpty(x.PTVPhu)))
                        {
                            var nameCell = arrColumnDefault[0];
                            lstColumnUse.Add(new ExcelBaoCaoSoPhucTrinhPhauThuatThuThuatEkipVo()
                            {
                                CellName = nameCell,
                                VaiTroDieuDuong = Enums.EnumVaiTroDieuDuong.PhuPhauThuat
                            });
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.Font.Bold = true;
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Merge = true;
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Value = "Phụ phậu thuật";
                            arrColumnDefault = arrColumnDefault.Where(x => x != nameCell).ToArray();
                        }

                        if (datas.Any(x => !string.IsNullOrEmpty(x.DungCuVongTrong)))
                        {
                            var nameCell = arrColumnDefault[0];
                            lstColumnUse.Add(new ExcelBaoCaoSoPhucTrinhPhauThuatThuThuatEkipVo()
                            {
                                CellName = nameCell,
                                VaiTroDieuDuong = Enums.EnumVaiTroDieuDuong.DungCuVongTrong
                            });
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.Font.Bold = true;
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Merge = true;
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Value = "Dụng cụ vòng trong";
                            arrColumnDefault = arrColumnDefault.Where(x => x != nameCell).ToArray();
                        }

                        if (datas.Any(x => !string.IsNullOrEmpty(x.DungCuVongNgoai)))
                        {
                            var nameCell = arrColumnDefault[0];
                            lstColumnUse.Add(new ExcelBaoCaoSoPhucTrinhPhauThuatThuThuatEkipVo()
                            {
                                CellName = nameCell,
                                VaiTroDieuDuong = Enums.EnumVaiTroDieuDuong.DungCuVongNgoai
                            });
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.Font.Bold = true;
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Merge = true;
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Value = "Dụng cụ vòng ngoài";
                            arrColumnDefault = arrColumnDefault.Where(x => x != nameCell).ToArray();
                        }

                        if (datas.Any(x => !string.IsNullOrEmpty(x.NguoiGayMeGayTePhu)))
                        {
                            var nameCell = arrColumnDefault[0];
                            lstColumnUse.Add(new ExcelBaoCaoSoPhucTrinhPhauThuatThuThuatEkipVo()
                            {
                                CellName = nameCell,
                                VaiTroDieuDuong = Enums.EnumVaiTroDieuDuong.PhuMePhuTe
                            });
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.Font.Bold = true;
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Merge = true;
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Value = "Phụ mê/ phụ tê";
                            arrColumnDefault = arrColumnDefault.Where(x => x != nameCell).ToArray();
                        }

                        if (datas.Any(x => !string.IsNullOrEmpty(x.ChayNgoai)))
                        {
                            var nameCell = arrColumnDefault[0];
                            lstColumnUse.Add(new ExcelBaoCaoSoPhucTrinhPhauThuatThuThuatEkipVo()
                            {
                                CellName = nameCell,
                                VaiTroDieuDuong = Enums.EnumVaiTroDieuDuong.ChayNgoai
                            });
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.Font.Bold = true;
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Merge = true;
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Value = "Chạy ngoài";
                            arrColumnDefault = arrColumnDefault.Where(x => x != nameCell).ToArray();
                        }

                        if (datas.Any(x => !string.IsNullOrEmpty(x.Phu1)))
                        {
                            var nameCell = arrColumnDefault[0];
                            lstColumnUse.Add(new ExcelBaoCaoSoPhucTrinhPhauThuatThuThuatEkipVo()
                            {
                                CellName = nameCell,
                                VaiTroDieuDuong = Enums.EnumVaiTroDieuDuong.Phu1
                            });
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.Font.Bold = true;
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Merge = true;
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Value = "Phụ 1";
                            arrColumnDefault = arrColumnDefault.Where(x => x != nameCell).ToArray();
                        }

                        if (datas.Any(x => !string.IsNullOrEmpty(x.Phu2)))
                        {
                            var nameCell = arrColumnDefault[0];
                            lstColumnUse.Add(new ExcelBaoCaoSoPhucTrinhPhauThuatThuThuatEkipVo()
                            {
                                CellName = nameCell,
                                VaiTroDieuDuong = Enums.EnumVaiTroDieuDuong.Phu2
                            });
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.Font.Bold = true;
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Merge = true;
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Value = "Phụ 2";
                            arrColumnDefault = arrColumnDefault.Where(x => x != nameCell).ToArray();
                        }

                        if (datas.Any(x => !string.IsNullOrEmpty(x.Phu3)))
                        {
                            var nameCell = arrColumnDefault[0];
                            lstColumnUse.Add(new ExcelBaoCaoSoPhucTrinhPhauThuatThuThuatEkipVo()
                            {
                                CellName = nameCell,
                                VaiTroDieuDuong = Enums.EnumVaiTroDieuDuong.Phu3
                            });
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.Font.Bold = true;
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Merge = true;
                            range.Worksheet.Cells[$"{nameCell}7:{nameCell}8"].Value = "Phụ 3";
                            arrColumnDefault = arrColumnDefault.Where(x => x != nameCell).ToArray();
                        }
                        #endregion

                        //range.Worksheet.Cells["U7:U8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //range.Worksheet.Cells["U7:U8"].Merge = true;
                        //range.Worksheet.Cells["U7:U8"].Value = "PTV chính";

                        //range.Worksheet.Cells["V7:V8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //range.Worksheet.Cells["V7:V8"].Merge = true;
                        //range.Worksheet.Cells["V7:V8"].Value = "Gây mê/ tê chính";

                        //range.Worksheet.Cells["W7:W8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //range.Worksheet.Cells["W7:W8"].Merge = true;
                        //range.Worksheet.Cells["W7:W8"].Value = "Gây mê/ tê phụ";

                        //range.Worksheet.Cells["X7:X8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //range.Worksheet.Cells["X7:X8"].Merge = true;
                        //range.Worksheet.Cells["X7:X8"].Value = "Phụ phậu thuật";

                        //range.Worksheet.Cells["Y7:Y8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //range.Worksheet.Cells["Y7:Y8"].Merge = true;
                        //range.Worksheet.Cells["Y7:Y8"].Value = "Dụng cụ vòng trong";

                        //range.Worksheet.Cells["Z7:Z8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //range.Worksheet.Cells["Z7:Z8"].Merge = true;
                        //range.Worksheet.Cells["Z7:Z8"].Value = "Dụng cụ vòng ngoài";

                        //range.Worksheet.Cells["AA7:AA8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //range.Worksheet.Cells["AA7:AA8"].Merge = true;
                        //range.Worksheet.Cells["AA7:AA8"].Value = "Phụ mê/ phụ tê";

                        //range.Worksheet.Cells["AB7:AB8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //range.Worksheet.Cells["AB7:AB8"].Merge = true;
                        //range.Worksheet.Cells["AB7:AB8"].Value = "Chạy ngoài";

                        //range.Worksheet.Cells["AC7:AC8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //range.Worksheet.Cells["AC7:AC8"].Merge = true;
                        //range.Worksheet.Cells["AC7:AC8"].Value = "Phụ 1";

                        //range.Worksheet.Cells["AD7:AD8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //range.Worksheet.Cells["AD7:AD8"].Merge = true;
                        //range.Worksheet.Cells["AD7:AD8"].Value = "Phụ 2";

                        //range.Worksheet.Cells["AE7:AE8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //range.Worksheet.Cells["AE7:AE8"].Merge = true;
                        //range.Worksheet.Cells["AE7:AE8"].Value = "Phụ 3";
                    }

                    int index = 9;
                    var stt = 1;
                    var numberFormat = "#,##0.00";
                    if (datas.Any())
                    {
                        foreach (var item in datas)
                        {
                            using (var range = worksheet.Cells["A" + index + ":H" + index])
                            {
                                range.Worksheet.Cells["A" + index + ":H" + index].Style.VerticalAlignment =
                                    ExcelVerticalAlignment.Bottom;
                                range.Worksheet.Cells["A" + index + ":H" + index].Style.Font
                                    .SetFromFont(new Font("Times New Roman", 10));
                                range.Worksheet.Cells["A" + index + ":H" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["A" + index + ":H" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["A" + index + ":H" + index].Style.Font.Color
                                    .SetColor(Color.Black);

                                worksheet.Cells["A" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index].Value = stt;

                                worksheet.Cells["B" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["B" + index].Value = item.MaTN;

                                worksheet.Cells["C" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["C" + index].Value = item.TenBenhNhan;

                                worksheet.Cells["D" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["D" + index].Value = item.NgaySinhDisplay;

                                worksheet.Cells["E" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["E" + index].Value = item.GioiTinh;

                                worksheet.Cells["F" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["F" + index].Value = item.DiaChi;

                                worksheet.Cells["G" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["G" + index].Value = item.BHYT;

                                worksheet.Cells["H" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["H" + index].Value = item.ChuanDoanTruocPt;

                                worksheet.Cells["I" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["I" + index].Value = item.ChuanDoanSauPt;

                                worksheet.Cells["J" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["J" + index].Value = item.PhuongPhapPhauThuat;

                                worksheet.Cells["K" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["K" + index].Value = item.ThuThuatChuyenKhoa;

                                worksheet.Cells["L" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["L" + index].Value = item.SoThuTuThongTu50;

                                worksheet.Cells["M" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["M" + index].Value = item.PhuongPhapVoCam;

                                worksheet.Cells["N" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["N" + index].Value = item.NhomDichVu;

                                worksheet.Cells["O" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["O" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["O" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                worksheet.Cells["O" + index].Value = item.SoLuong;

                                worksheet.Cells["P" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["P" + index].Value = item.DonGia;
                                worksheet.Cells["P" + index].Style.Numberformat.Format = numberFormat;

                                worksheet.Cells["Q" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["Q" + index].Value = item.ThoiGianPhauThuatStart;

                                worksheet.Cells["R" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["R" + index].Value = item.ThoiGianPhauThuatEnd;

                                worksheet.Cells["S" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["S" + index].Value = item.ThoiGianKhoiMe;

                                worksheet.Cells["T" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["T" + index].Value = item.LoaiPTTT;

                                worksheet.Cells["U" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["U" + index].Value = item.NoiChiDinh;

                                worksheet.Cells["V" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["V" + index].Value = item.NoiThucHien;

                                foreach (var columnDetail in lstColumnUse)
                                {
                                    worksheet.Cells[columnDetail.CellName + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    if (columnDetail.VaiTroBacSi != null)
                                    {
                                        switch (columnDetail.VaiTroBacSi)
                                        {
                                            case Enums.EnumVaiTroBacSi.PhauThuatVienChinh:
                                                worksheet.Cells[columnDetail.CellName + index].Value = item.PTVChinh;
                                                break;
                                            case Enums.EnumVaiTroBacSi.GayMeTeChinh:
                                                worksheet.Cells[columnDetail.CellName + index].Value = item.NguoiGayMeChinh;
                                                break;
                                            case Enums.EnumVaiTroBacSi.GayMeTePhu:
                                                worksheet.Cells[columnDetail.CellName + index].Value = item.NguoiGayMePhu;
                                                break;
                                        }
                                    }
                                    else if (columnDetail.VaiTroDieuDuong != null)
                                    {
                                        switch (columnDetail.VaiTroDieuDuong)
                                        {
                                            case Enums.EnumVaiTroDieuDuong.PhuPhauThuat:
                                                worksheet.Cells[columnDetail.CellName + index].Value = item.PTVPhu;
                                                break;
                                            case Enums.EnumVaiTroDieuDuong.DungCuVongTrong:
                                                worksheet.Cells[columnDetail.CellName + index].Value = item.DungCuVongTrong;
                                                break;
                                            case Enums.EnumVaiTroDieuDuong.DungCuVongNgoai:
                                                worksheet.Cells[columnDetail.CellName + index].Value = item.DungCuVongNgoai;
                                                break;
                                            case Enums.EnumVaiTroDieuDuong.PhuMePhuTe:
                                                worksheet.Cells[columnDetail.CellName + index].Value = item.NguoiGayMeGayTePhu;
                                                break;
                                            case Enums.EnumVaiTroDieuDuong.ChayNgoai:
                                                worksheet.Cells[columnDetail.CellName + index].Value = item.ChayNgoai;
                                                break;
                                            case Enums.EnumVaiTroDieuDuong.Phu1:
                                                worksheet.Cells[columnDetail.CellName + index].Value = item.Phu1;
                                                break;
                                            case Enums.EnumVaiTroDieuDuong.Phu2:
                                                worksheet.Cells[columnDetail.CellName + index].Value = item.Phu2;
                                                break;
                                            case Enums.EnumVaiTroDieuDuong.Phu3:
                                                worksheet.Cells[columnDetail.CellName + index].Value = item.Phu3;
                                                break;
                                        }
                                    }
                                }

                                //worksheet.Cells["U" + index].Style.Border
                                //    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                //worksheet.Cells["U" + index].Value = item.PTVChinh;

                                //worksheet.Cells["V" + index].Style.Border
                                //    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                //worksheet.Cells["V" + index].Value = item.NguoiGayMeChinh;

                                //worksheet.Cells["W" + index].Style.Border
                                //    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                //worksheet.Cells["W" + index].Value = item.NguoiGayMePhu;

                                //worksheet.Cells["X" + index].Style.Border
                                //   .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                //worksheet.Cells["X" + index].Value = item.PTVPhu;

                                //worksheet.Cells["Y" + index].Style.Border
                                //   .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                //worksheet.Cells["Y" + index].Value = item.DungCuVongTrong;

                                //worksheet.Cells["Z" + index].Style.Border
                                //   .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                //worksheet.Cells["Z" + index].Value = item.DungCuVongNgoai;

                                //worksheet.Cells["AA" + index].Style.Border
                                //   .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                //worksheet.Cells["AA" + index].Value = item.NguoiGayMeGayTePhu;

                                //worksheet.Cells["AB" + index].Style.Border
                                //   .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                //worksheet.Cells["AB" + index].Value = item.ChayNgoai;

                                //worksheet.Cells["AC" + index].Style.Border
                                //   .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                //worksheet.Cells["AC" + index].Value = item.Phu1;

                                //worksheet.Cells["AD" + index].Style.Border
                                //   .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                //worksheet.Cells["AD" + index].Value = item.Phu2;

                                //worksheet.Cells["AE" + index].Style.Border
                                //   .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                //worksheet.Cells["AE" + index].Value = item.Phu3;
                                index++;
                            }
                            stt++;
                        }
                    }
                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }
    }
}