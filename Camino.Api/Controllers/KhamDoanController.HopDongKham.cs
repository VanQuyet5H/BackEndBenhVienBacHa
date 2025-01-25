using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.Error;
using Camino.Api.Models.KhamDoan;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.KhamDoans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using static Camino.Core.Domain.Enums;
using Camino.Core.Domain.ValueObject.KhamDoan;
using System.Globalization;
using Camino.Services.Helpers;
using Camino.Core.Domain.Entities.PhongBenhViens;
using System.Text;

namespace Camino.Api.Controllers
{
    public partial class KhamDoanController
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataDanhSachHopDongKhamForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanHopDongKham)]
        public async Task<ActionResult<GridDataSource>> GetDataDanhSachHopDongKhamForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _khamDoanService.GetDSHopDongKhamForGrid(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDanhSachHopDongKhamPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanHopDongKham)]
        public async Task<ActionResult<GridDataSource>> GetTotalDanhSachHopDongKhamPageForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _khamDoanService.GetTotalPagesDSHopDongKhamForGrid(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataDanhSachNhanVienCTyForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanHopDongKham)]
        public async Task<ActionResult<GridDataSource>> GetDataDanhSachNhanVienCTyForGridAsync([FromBody] QueryInfo queryInfo)
        {
            long hopDongKhamId = int.Parse(queryInfo.AdditionalSearchString);
            var gridData = await _khamDoanService.GetDSHopDongKhamSucKhoeNhanVienForGrid(queryInfo, hopDongKhamId);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDanhSachNhanVienCTyPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanHopDongKham)]
        public async Task<ActionResult<GridDataSource>> GetTotalDanhSachNhanVienCTyPageForGridAsync([FromBody] QueryInfo queryInfo)
        {
            long hopDongKhamId = int.Parse(queryInfo.AdditionalSearchString);
            var gridData = await _khamDoanService.GetTotalPagesDSHopDongKhamSucKhoeNhanVienForGrid(queryInfo, hopDongKhamId);
            return Ok(gridData);
        }

        [HttpPost("DownloadTemplateGoiKham")]
        public ActionResult DownloadGoiKham()
        {
            var path = @"Resource\\TemplateGoiKham.xlsx";
            System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(fs);

            long byteLength = new System.IO.FileInfo(path).Length;
            var fileContent = binaryReader.ReadBytes((Int32)byteLength);

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=TemplateGoiKham.xlsx");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(fileContent, "application/vnd.ms-excel");
        }

        [HttpPost("DownloadTemplateNhanVienKSK")]
        public ActionResult DownloadTemplateNhanVienKSK()
        {
            var path = @"Resource\\NhanVienKhamSucKhoe.xlsx";
            System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(fs);

            long byteLength = new System.IO.FileInfo(path).Length;
            var fileContent = binaryReader.ReadBytes((Int32)byteLength);

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=NhanVienKhamSucKhoe.xlsx");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(fileContent, "application/vnd.ms-excel");
        }

        [HttpGet("ImportGoiKham")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.KhamDoanHopDongKham)]
        public ActionResult<bool> ImportGoiKham(string tenGuid, string duongDan, long hopDongKhamSucKhoeId)
        {
            var dataHopDongKham = _khamDoanService.GetThongTinHopDongKham(hopDongKhamSucKhoeId);

            List<GoiKhamSucKhoeViewModel> listGoiKhamSucKhoe = new List<GoiKhamSucKhoeViewModel>();

            GoiKhamSucKhoeViewModel goiKhamSucKhoeViewModel = new GoiKhamSucKhoeViewModel();
            List<DanhSachNhapExcelError> listError = new List<DanhSachNhapExcelError>();

            var path = _taiLieuDinhKemService.GetObjectStream(duongDan, tenGuid);

            using (ExcelPackage package = new ExcelPackage(path))
            {
                ExcelWorksheet workSheet = package.Workbook.Worksheets["Gói khám"];
                if (workSheet == null)
                {
                    throw new ApiException("Thông tin file gói khám không đúng");
                }
                int totalRows = workSheet.Dimension.Rows;
                if (totalRows >= 2)
                {
                    for (int i = 2; i <= totalRows; i++)
                    {
                        //Khách hàng chắc chắn sẽ nhâp value cột 1
                        if (workSheet.Cells[i, 1].Value != null)
                        {
                            DanhSachNhapExcelError dataError = new DanhSachNhapExcelError();
                            var goiKhamDichVuKhamSucKhoeDoanViewModel = new GoiKhamDichVuKhamSucKhoeDoanViewModel();

                            if (workSheet.Cells[i, 1].Value == null || workSheet.Cells[i, 2].Value == null || workSheet.Cells[i, 4].Value == null || workSheet.Cells[i, 5].Value == null
                                || workSheet.Cells[i, 5].Value == null || workSheet.Cells[i, 7].Value == null || workSheet.Cells[i, 8].Value == null || workSheet.Cells[i, 9].Value == null
                                || workSheet.Cells[i, 10].Value == null || workSheet.Cells[i, 11].Value == null || workSheet.Cells[i, 12].Value == null
                                || workSheet.Cells[i, 13].Value == null)
                            {
                                var inforMaGoiKham = workSheet.Cells[i, 1].Value == null ? _localizationService.GetResource("KhamDoan.DichVuKhamSucKhoeDoans.MaGoiKham.Required") + "|" : "";
                                var inforTenGoiKham = workSheet.Cells[i, 2].Value == null ? _localizationService.GetResource("KhamDoan.DichVuKhamSucKhoeDoans.TenGoiKham.Required") + "|" : "";
                                var inLaDichVuKham = workSheet.Cells[i, 3].Value == null ? _localizationService.GetResource("KhamDoan.DichVuKhamSucKhoeDoans.LaDichVuKham.Required") + "|" : "";
                                var infoNhom = workSheet.Cells[i, 4].Value == null ? _localizationService.GetResource("KhamDoan.DichVuKhamSucKhoeDoans.NhomDichVu.Required") + "|" : "";
                                var inforChuyenKhoaKhamSucKhoem = workSheet.Cells[i, 5].Value == null ? _localizationService.GetResource("KhamDoan.DichVuKhamSucKhoeDoans.ChuyenKhoaKhamSucKhoe.Required") + "|" : "";
                                var inforDichVuKyThuatBenhVienId = workSheet.Cells[i, 6].Value == null ? _localizationService.GetResource("KhamDoan.DichVuKhamSucKhoeDoans.DichVuKyThuatBenhVienId.Required") + "|" : "";

                                var inforLoaiGia = workSheet.Cells[i, 9].Value == null ? _localizationService.GetResource("KhamDoan.DichVuKhamSucKhoeDoans.LoaiGia.Required") + "|" : "";
                                var inforDonGiaBenhVien = workSheet.Cells[i, 10].Value == null ? _localizationService.GetResource("KhamDoan.DichVuKhamSucKhoeDoans.DonGiaBenhVien.Required") + "|" : "";
                                var inforDonGiaUuDai = workSheet.Cells[i, 11].Value == null ? _localizationService.GetResource("KhamDoan.DichVuKhamSucKhoeDoans.DonGiaUuDai.Required") + "|" : "";
                                var inforDonGiaChuaUuDai = workSheet.Cells[i, 12].Value == null ? _localizationService.GetResource("KhamDoan.DichVuKhamSucKhoeDoans.DonGiaChuaUuDai.Required") + "|" : "";

                                var inforHinhThucKhamBenh = workSheet.Cells[i, 13].Value == null ? _localizationService.GetResource("KhamDoan.DichVuKhamSucKhoeDoans.HinhThucKhamBenh.Required") + "|" : "";
                                var inforNoiThucHien = workSheet.Cells[i, 8].Value == null ? _localizationService.GetResource("KhamDoan.DichVuKhamSucKhoeDoans.NoiThucHien.Required") + "|" : "";

                                listError.Add(new DanhSachNhapExcelError
                                {
                                    MaGoi = (workSheet.Cells[i, 1].Value != null ? workSheet.Cells[i, 1].Value.ToString() : "") + "-"
                                            + (workSheet.Cells[i, 2].Value != null ? workSheet.Cells[i, 2].Value.ToString() : ""),
                                    Error = inforMaGoiKham + inforTenGoiKham + inLaDichVuKham + infoNhom + inforChuyenKhoaKhamSucKhoem + inforDichVuKyThuatBenhVienId + inforLoaiGia
                                            + inforDonGiaBenhVien + inforDonGiaUuDai + inforDonGiaChuaUuDai + inforHinhThucKhamBenh + inforNoiThucHien
                                });
                                continue;
                            }

                            goiKhamDichVuKhamSucKhoeDoanViewModel.Ma = workSheet.Cells[i, 1].Value != null ? workSheet.Cells[i, 1].Value.ToString() : null;
                            goiKhamDichVuKhamSucKhoeDoanViewModel.Ten = workSheet.Cells[i, 2].Value != null ? workSheet.Cells[i, 2].Value.ToString() : null;
                            var laDVKham = (workSheet.Cells[i, 3].Value.ToString().Trim().TrimStart().TrimEnd().ToUpper() == ("Có").ToUpper() ? true : false);

                            goiKhamDichVuKhamSucKhoeDoanViewModel.LaDichVuKham = laDVKham;
                            goiKhamDichVuKhamSucKhoeDoanViewModel.Nhom = GetNhomDichVuChiDinhKhamSucKhoe(workSheet.Cells[i, 4].Value.ToString());
                            goiKhamDichVuKhamSucKhoeDoanViewModel.ChuyenKhoaKhamSucKhoe = GetChuyenKhoaKhamSucKhoe(workSheet.Cells[i, 5].Value.ToString());

                            goiKhamDichVuKhamSucKhoeDoanViewModel.NhomGiaDichVuKyThuatBenhVienId = GetNhomGiaDichVuKyThuatBenhVien(laDVKham, workSheet.Cells[i, 9].Value.ToString());

                            goiKhamDichVuKhamSucKhoeDoanViewModel.DichVuKyThuatBenhVienId = DichVuKhamBenhBVHoacDVKTBenhVien(workSheet.Cells[i, 6].Value.ToString());

                            var kiemTraGoiSucKhoeKham = GoiKhamSucKhoeNoiThucHienViewModel(workSheet.Cells[i, 8].Value.ToString(),
                                                                                                                                 laDVKham,
                                                                                                                                 DichVuKhamBenhBVHoacDVKTBenhVien(workSheet.Cells[i, 6].Value.ToString()));

                            if (kiemTraGoiSucKhoeKham.Any(c => c.PhongBenhVienId == 0))
                            {
                                listError.Add(new DanhSachNhapExcelError
                                {
                                    MaGoi = (workSheet.Cells[i, 1].Value != null ? workSheet.Cells[i, 1].Value.ToString() : "") + "-"
                                             + (workSheet.Cells[i, 2].Value != null ? workSheet.Cells[i, 2].Value.ToString() : ""),
                                    Error = "Nơi thực hiện " + workSheet.Cells[i, 8].Value.ToString() + " Không có trong hệ thống."
                                });
                                continue;
                            }


                            goiKhamDichVuKhamSucKhoeDoanViewModel.GoiKhamSucKhoeNoiThucHiens = kiemTraGoiSucKhoeKham;
                            goiKhamDichVuKhamSucKhoeDoanViewModel.SoLan = int.Parse(workSheet.Cells[i, 7].Value.ToString());
                            goiKhamDichVuKhamSucKhoeDoanViewModel.NoiThucHienString = workSheet.Cells[i, 8].Value.ToString();
                            goiKhamDichVuKhamSucKhoeDoanViewModel.LoaiGia = workSheet.Cells[i, 9].Value != null ? workSheet.Cells[i, 9].Value.ToString() : null;
                            goiKhamDichVuKhamSucKhoeDoanViewModel.DonGiaBenhVien = decimal.Parse(workSheet.Cells[i, 10].Value.ToString());
                            goiKhamDichVuKhamSucKhoeDoanViewModel.DonGiaUuDai = decimal.Parse(workSheet.Cells[i, 11].Value.ToString());
                            goiKhamDichVuKhamSucKhoeDoanViewModel.DonGiaChuaUuDai = decimal.Parse(workSheet.Cells[i, 12].Value.ToString());

                            goiKhamDichVuKhamSucKhoeDoanViewModel.GioiTinhNam = workSheet.Cells[i, 14].Value != null ? KiemTraGioiTinhNam(workSheet.Cells[i, 14].Value.ToString()) : null;
                            goiKhamDichVuKhamSucKhoeDoanViewModel.GioiTinhNu = workSheet.Cells[i, 14].Value != null ? KiemTraGioiTinhNu(workSheet.Cells[i, 14].Value.ToString()) : null;

                            goiKhamDichVuKhamSucKhoeDoanViewModel.CoMangThai = workSheet.Cells[i, 15].Value != null ? KiemTraCoThai(workSheet.Cells[i, 15].Value.ToString()) : null;
                            goiKhamDichVuKhamSucKhoeDoanViewModel.KhongMangThai = workSheet.Cells[i, 15].Value != null ? KiemTraKhongThai(workSheet.Cells[i, 15].Value.ToString()) : null;

                            goiKhamDichVuKhamSucKhoeDoanViewModel.DaLapGiaDinh = workSheet.Cells[i, 16].Value != null ? KiemTraCoThai(workSheet.Cells[i, 15].Value.ToString()) : null;
                            goiKhamDichVuKhamSucKhoeDoanViewModel.ChuaLapGiaDinh = workSheet.Cells[i, 16].Value != null ? KiemTraChuaLapGiaDinh(workSheet.Cells[i, 15].Value.ToString()) : null;

                            goiKhamSucKhoeViewModel.DichVuKhamSucKhoeDoans.Add(goiKhamDichVuKhamSucKhoeDoanViewModel);
                        }
                    }
                }
                else
                {
                    throw new ApiException(_localizationService.GetResource("KhamDoan.DichVuKhamSucKhoeDoans.Required"));
                }
            }

            var dataItemGoiKhams = goiKhamSucKhoeViewModel.DichVuKhamSucKhoeDoans.GroupBy(vv => vv.Ma).Select(c => c.ToList());

            if (dataItemGoiKhams.Any())
            {
                var kiemTraGoiKham = goiKhamSucKhoeViewModel.DichVuKhamSucKhoeDoans.GroupBy(vv => vv.Ma);
                foreach (var maGoiKham in kiemTraGoiKham)
                {
                    var ktgoikham = KiemTraMaGoiKham(maGoiKham.Key, hopDongKhamSucKhoeId);
                    if (ktgoikham)
                    {
                        listError.Add(new DanhSachNhapExcelError
                        {
                            MaGoi = maGoiKham.Key,
                            Error = "Tên gói khám " + maGoiKham.Key + " tồn tại trong hệ thống."
                        });
                    }
                }
            }

            if (dataItemGoiKhams.Any())
            {
                foreach (var dataItemGoiKham in dataItemGoiKhams)
                {
                    var nhanVienKhamSucKhoes = _khamDoanService.GetGoiKhamTheoMaHDKvaMa(hopDongKhamSucKhoeId, dataItemGoiKham.Select(c => c.Ma).FirstOrDefault());
                    if (!nhanVienKhamSucKhoes.Any())
                    {
                        goiKhamSucKhoeViewModel.CongTyKhamSucKhoeId = dataHopDongKham.CongTyKhamSucKhoeId;
                        goiKhamSucKhoeViewModel.TenCongTy = dataHopDongKham.CongTyKhamSucKhoe.Ten;
                        goiKhamSucKhoeViewModel.SoHopDong = dataHopDongKham.SoHopDong;
                        goiKhamSucKhoeViewModel.NgayHieuLuc = dataHopDongKham.NgayHieuLuc;
                        goiKhamSucKhoeViewModel.NgayKetThuc = dataHopDongKham.NgayKetThuc;
                        goiKhamSucKhoeViewModel.Ma = dataItemGoiKham.Select(c => c.Ma).FirstOrDefault();
                        goiKhamSucKhoeViewModel.Ten = dataItemGoiKham.Select(c => c.Ten).FirstOrDefault();
                        goiKhamSucKhoeViewModel.DichVuKhamSucKhoeDoans = dataItemGoiKham.Select(c => c).ToList();
                        goiKhamSucKhoeViewModel.HopDongKhamSucKhoeId = hopDongKhamSucKhoeId;

                        listGoiKhamSucKhoe.Add(goiKhamSucKhoeViewModel);
                    }
                }

            }

            if (listGoiKhamSucKhoe.Any())
            {
                foreach (var viewModel in listGoiKhamSucKhoe)
                {
                    var goiKhamSucKhoe = viewModel.ToEntity<GoiKhamSucKhoe>();
                    foreach (var item in viewModel.DichVuKhamSucKhoeDoans.Where(p => p.LaDichVuKham))
                    {
                        var goiKhamKhamBenh = new GoiKhamSucKhoeDichVuKhamBenh
                        {
                            DichVuKhamBenhBenhVienId = item.DichVuKyThuatBenhVienId.Value,
                            ChuyenKhoaKhamSucKhoe = item.ChuyenKhoaKhamSucKhoe.Value,
                            NhomGiaDichVuKhamBenhBenhVienId = item.NhomGiaDichVuKyThuatBenhVienId,
                            DonGiaBenhVien = item.DonGiaBenhVien.Value,
                            DonGiaUuDai = item.DonGiaUuDai.Value,
                            DonGiaChuaUuDai = item.DonGiaChuaUuDai.Value,
                            GioiTinhNam = item.GioiTinhNam.GetValueOrDefault(),
                            GioiTinhNu = item.GioiTinhNu.GetValueOrDefault(),
                            CoMangThai = item.CoMangThai.GetValueOrDefault(),
                            KhongMangThai = item.KhongMangThai.GetValueOrDefault(),
                            DaLapGiaDinh = item.DaLapGiaDinh.GetValueOrDefault(),
                            ChuaLapGiaDinh = item.ChuaLapGiaDinh.GetValueOrDefault(),
                            SoTuoiTu = item.SoTuoiTu,
                            SoTuoiDen = item.SoTuoiDen,
                        };
                        foreach (var noiThucHien in item.GoiKhamSucKhoeNoiThucHiens)
                        {
                            var goiKhamNoiThucHien = new GoiKhamSucKhoeNoiThucHien
                            {
                                PhongBenhVienId = noiThucHien.PhongBenhVienId
                            };
                            goiKhamKhamBenh.GoiKhamSucKhoeNoiThucHiens.Add(goiKhamNoiThucHien);
                        }
                        goiKhamSucKhoe.GoiKhamSucKhoeDichVuKhamBenhs.Add(goiKhamKhamBenh);
                    }

                    foreach (var item in viewModel.DichVuKhamSucKhoeDoans.Where(p => !p.LaDichVuKham))
                    {
                        var goiKhamKyThuat = new GoiKhamSucKhoeDichVuDichVuKyThuat
                        {
                            DichVuKyThuatBenhVienId = item.DichVuKyThuatBenhVienId.Value,
                            NhomGiaDichVuKyThuatBenhVienId = item.NhomGiaDichVuKyThuatBenhVienId,
                            SoLan = item.SoLan.Value,
                            DonGiaBenhVien = item.DonGiaBenhVien.Value,
                            DonGiaUuDai = item.DonGiaUuDai.Value,
                            DonGiaChuaUuDai = item.DonGiaChuaUuDai.Value,
                            GioiTinhNam = item.GioiTinhNam.GetValueOrDefault(),
                            GioiTinhNu = item.GioiTinhNu.GetValueOrDefault(),
                            CoMangThai = item.CoMangThai.GetValueOrDefault(),
                            KhongMangThai = item.KhongMangThai.GetValueOrDefault(),
                            DaLapGiaDinh = item.DaLapGiaDinh.GetValueOrDefault(),
                            ChuaLapGiaDinh = item.ChuaLapGiaDinh.GetValueOrDefault(),
                            SoTuoiTu = item.SoTuoiTu,
                            SoTuoiDen = item.SoTuoiDen
                        };
                        foreach (var noiThucHien in item.GoiKhamSucKhoeNoiThucHiens)
                        {
                            var goiKhamNoiThucHien = new GoiKhamSucKhoeNoiThucHien
                            {
                                PhongBenhVienId = noiThucHien.PhongBenhVienId
                            };
                            goiKhamKyThuat.GoiKhamSucKhoeNoiThucHiens.Add(goiKhamNoiThucHien);
                        }
                        goiKhamSucKhoe.GoiKhamSucKhoeDichVuDichVuKyThuats.Add(goiKhamKyThuat);
                    }
                    _goiKhamSucKhoeService.Add(goiKhamSucKhoe);
                }
            }


            if (listError.Any())
                listError[0].TotalThanhCong = listGoiKhamSucKhoe.Count;
            else
                listError.Add(new DanhSachNhapExcelError { TotalThanhCong = listGoiKhamSucKhoe.Count });

            return Ok(listError);
        }

        [HttpGet("ImportNhanVien")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.KhamDoanHopDongKham)]
        public async Task<ActionResult<List<DanhSachNhapExcelError>>> ImportNhanVien(string tenGuid, string duongDan, long hopDongKhamSucKhoeId)
        {
            List<HopDongKhamSucKhoeNhanVienViewModel> hopDongSucKhoeNhanViens = new List<HopDongKhamSucKhoeNhanVienViewModel>();
            List<DanhSachNhapExcelError> listNhanVienError = new List<DanhSachNhapExcelError>();

            var path = _taiLieuDinhKemService.GetObjectStream(duongDan, tenGuid);
            using (ExcelPackage package = new ExcelPackage(path))
            {
                ExcelWorksheet workSheet = package.Workbook.Worksheets["Nhân viên"];
                if (workSheet == null)
                {
                    throw new ApiException("Thông tin file nhân viên không đúng");
                }

                int totalRows = workSheet.Dimension.Rows;
                if (totalRows >= 2)
                {
                    for (int i = 2; i <= totalRows; i++)
                    {
                        var dataNhanNhanVien = new HopDongKhamSucKhoeNhanVienViewModel();

                        #region Thông tin bắt buôc phải có khi nhập nhân viên

                        dataNhanNhanVien.HopDongKhamSucKhoeId = hopDongKhamSucKhoeId;

                        DateTime? ktngaySinh = DateTime.MinValue;

                        if (!string.IsNullOrEmpty(workSheet.Cells[i, 6].Text))
                        {
                            DateTime.TryParseExact(workSheet.Cells[i, 6].Text.ToString().Trim(), "dd/MM/yyyy", null, DateTimeStyles.None, out var doB);
                            DateTime dtNgaySinh = doB;

                            if (dtNgaySinh != DateTime.MinValue)
                            {
                                var ngaySinh = dtNgaySinh;
                                dataNhanNhanVien.NgaySinh = ngaySinh.Day;
                                dataNhanNhanVien.ThangSinh = ngaySinh.Month;
                                dataNhanNhanVien.NamSinh = ngaySinh.Year;
                            }
                            else
                            {
                                listNhanVienError.Add(new DanhSachNhapExcelError
                                {
                                    MaNV = workSheet.Cells[i, 2].Text.ToString(),
                                    TenNhanVien = workSheet.Cells[i, 3].Text.ToString().ToUpper(),
                                    Error = "Vui lòng nhập ngày sinh đúng định dạng: Ngày/Tháng/Năm"
                                });
                            }
                        }
                        else
                        {
                            ktngaySinh = null;
                        }

                        if (string.IsNullOrEmpty(workSheet.Cells[i, 3].Text)) { continue; }
                        if (ktngaySinh == null || string.IsNullOrEmpty(workSheet.Cells[i, 3].Text) || string.IsNullOrEmpty(workSheet.Cells[i, 6].Text) || string.IsNullOrEmpty(workSheet.Cells[i, 4].Text))
                        {
                            var inforNgaySinh = ktngaySinh == null ? _localizationService.GetResource("KhamDoan.DichVuKhamSucKhoeDoans.NgaySinhNhanVien.Required") : "";
                            var inforHoTen = string.IsNullOrEmpty(workSheet.Cells[i, 3].Text) ? _localizationService.GetResource("KhamDoan.DichVuKhamSucKhoeDoans.HoTenNhanVien.Required") : "";
                            var inforGioiTinh = string.IsNullOrEmpty(workSheet.Cells[i, 4].Text) ? _localizationService.GetResource("KhamDoan.DichVuKhamSucKhoeDoans.GioiTinhNhanVien.Required") : "";
                            var inforCMND = string.IsNullOrEmpty(workSheet.Cells[i, 5].Text) ? _localizationService.GetResource("KhamDoan.DichVuKhamSucKhoeDoans.CMNDNHanVen.Required") : "";
                            var inforgoiKham = string.IsNullOrEmpty(workSheet.Cells[i, 7].Text) ? _localizationService.GetResource("KhamDoan.DichVuKhamSucKhoeDoans.KiemTraGoiKhamNV.Required") : "";

                            listNhanVienError.Add(new DanhSachNhapExcelError
                            {
                                MaNV = workSheet.Cells[i, 2].Text.ToString(),
                                TenNhanVien = !string.IsNullOrEmpty(workSheet.Cells[i, 3].Text) ? workSheet.Cells[i, 4].Text.ToString().ToUpper() : "Chưa xác nhận",
                                Error = inforNgaySinh + "|" + inforHoTen + "|" + inforGioiTinh + "|" + inforCMND + "|" + inforgoiKham
                            });

                            continue;
                        }
                        else
                        {
                            var ktGoiKham = GetGoiKham(workSheet.Cells[i, 7].Text.ToString().TrimStart().TrimEnd(), hopDongKhamSucKhoeId);
                            if (ktGoiKham == null)
                            {
                                listNhanVienError.Add(new DanhSachNhapExcelError
                                {
                                    MaNV = workSheet.Cells[i, 2].Text.ToString(),
                                    TenNhanVien = !string.IsNullOrEmpty(workSheet.Cells[i, 3].Text) ? workSheet.Cells[i, 3].Text.ToString().ToUpper() : "Chưa xác nhận",
                                    Error = _localizationService.GetResource("KhamDoan.DichVuKhamSucKhoeDoans.KiemTraGoiKhamNV.Required")
                                });
                                continue;
                            }

                            //kiểm tra gói kham dung chung ko 
                            var kiemtraGoiKhamChung = KiemTraGoiKhamDungChung((long)ktGoiKham);
                            if (kiemtraGoiKhamChung)
                            {
                                listNhanVienError.Add(new DanhSachNhapExcelError
                                {
                                    MaNV = workSheet.Cells[i, 2].Text.ToString(),
                                    TenNhanVien = !string.IsNullOrEmpty(workSheet.Cells[i, 3].Text) ? workSheet.Cells[i, 3].Text.ToString().ToUpper() : "Chưa xác nhận",
                                    Error = "Tên gói khám " + workSheet.Cells[i, 7].Text.ToString() + " là gói dùng chung."
                                });
                                continue;
                            }

                            dataNhanNhanVien.HoTen = workSheet.Cells[i, 3].Text.ToString().ToUpper();
                            dataNhanNhanVien.GioiTinh = workSheet.Cells[i, 4].Text.ToString().Trim().TrimStart().TrimEnd().ToUpper() == Enums.LoaiGioiTinh.GioiTinhNam.GetDescription().ToUpper()
                                                                                          ? Enums.LoaiGioiTinh.GioiTinhNam : Enums.LoaiGioiTinh.GioiTinhNu;
                            dataNhanNhanVien.SoChungMinhThu = workSheet.Cells[i, 5].Text.ToString();
                            dataNhanNhanVien.GoiKhamSucKhoeId = ktGoiKham;
                        }


                        #endregion



                        DateTime? ngayCapCmt = DateTime.MinValue;
                        if (!string.IsNullOrEmpty(workSheet.Cells[i, 13].Text))
                        {
                            DateTime.TryParseExact(workSheet.Cells[i, 6].Text.ToString().Trim(), "dd/MM/yyyy", null, DateTimeStyles.None, out var ngayCapCmtFormat);

                            if (ngayCapCmtFormat != DateTime.MinValue)
                            {
                                ngayCapCmt = ngayCapCmtFormat;
                            }
                            else
                            {
                                listNhanVienError.Add(new DanhSachNhapExcelError
                                {
                                    MaNV = workSheet.Cells[i, 2].Text.ToString(),
                                    TenNhanVien = workSheet.Cells[i, 3].Text.ToString(),
                                    Error = "Vui lòng nhập chứng minh nhân dân đúng định dạng: Ngày/Tháng/Năm"
                                });
                            }
                        }
                        else
                        {
                            ngayCapCmt = null;
                        }

                        DateTime? ngayBatDauLamViec = DateTime.MinValue;
                        if (!string.IsNullOrEmpty(workSheet.Cells[i, 24].Text))
                        {

                            DateTime.TryParseExact(workSheet.Cells[i, 24].Text.ToString(), "dd/MM/yyyy", null, DateTimeStyles.None, out var ngayLamViecFormat);
                            if (ngayLamViecFormat != DateTime.MinValue)
                            {
                                ngayBatDauLamViec = ngayLamViecFormat;
                            }
                            else
                            {
                                listNhanVienError.Add(new DanhSachNhapExcelError
                                {
                                    MaNV = workSheet.Cells[i, 2].Text.ToString(),
                                    TenNhanVien = workSheet.Cells[i, 3].Text.ToString(),
                                    Error = "Vui lòng nhập ngày làm việc đúng định dạng: Ngày/Tháng/Năm"
                                });
                            }

                        }
                        else
                        {
                            ngayBatDauLamViec = null;
                        }





                        #region Không cần validate nhân viên

                        dataNhanNhanVien.STTNhanVien = !string.IsNullOrEmpty(workSheet.Cells[i, 1].Text) ? Int16.Parse(workSheet.Cells[i, 1].Text.ToString()) : (int?)null;

                        dataNhanNhanVien.MaNhanVien = !string.IsNullOrEmpty(workSheet.Cells[i, 2].Text) ? workSheet.Cells[i, 2].Text.ToString() : null;
                        dataNhanNhanVien.HoTenKhac = !string.IsNullOrEmpty(workSheet.Cells[i, 8].Text) ? workSheet.Cells[i, 7].Text.ToString() : null;
                        dataNhanNhanVien.SoDienThoai = !string.IsNullOrEmpty(workSheet.Cells[i, 9].Text) ? workSheet.Cells[i, 8].Text.ToString() : null;

                        dataNhanNhanVien.NgheNghiepId = !string.IsNullOrEmpty(workSheet.Cells[i, 10].Text) ? GetThongTinNgheNghiep(workSheet.Cells[i, 10].Text.ToString()) : null;
                        dataNhanNhanVien.QuocTichId = !string.IsNullOrEmpty(workSheet.Cells[i, 11].Text) ? GetQuocTich(workSheet.Cells[i, 11].Text.ToString()) : null;
                        dataNhanNhanVien.DanTocId = !string.IsNullOrEmpty(workSheet.Cells[i, 12].Text) ? GetDanToc(workSheet.Cells[i, 12].Text.ToString()) : null;

                        dataNhanNhanVien.NgayCapChungMinhThu = ngayCapCmt;
                        dataNhanNhanVien.NoiCapChungMinhThu = !string.IsNullOrEmpty(workSheet.Cells[i, 14].Text) ? workSheet.Cells[i, 14].Text.ToString() : null;
                        dataNhanNhanVien.TinhThanhId = !string.IsNullOrEmpty(workSheet.Cells[i, 15].Text) ? GetTinhThanh(workSheet.Cells[i, 15].Text.ToString()) : null;
                        dataNhanNhanVien.QuanHuyenId = !string.IsNullOrEmpty(workSheet.Cells[i, 16].Text) ? GetQuanHuyen(workSheet.Cells[i, 16].Text.ToString()) : null;
                        dataNhanNhanVien.PhuongXaId = !string.IsNullOrEmpty(workSheet.Cells[i, 17].Text) ? GetPhuongXa(workSheet.Cells[i, 17].Text.ToString()) : null;
                        dataNhanNhanVien.DiaChi = !string.IsNullOrEmpty(workSheet.Cells[i, 18].Text) ? workSheet.Cells[i, 18].Text.ToString() : null;

                        dataNhanNhanVien.HoKhauTinhThanhId = !string.IsNullOrEmpty(workSheet.Cells[i, 19].Text) ? GetTinhThanh(workSheet.Cells[i, 19].Text.ToString()) : null;
                        dataNhanNhanVien.HoKhauQuanHuyenId = !string.IsNullOrEmpty(workSheet.Cells[i, 20].Text) ? GetQuanHuyen(workSheet.Cells[i, 20].Text.ToString()) : null;
                        dataNhanNhanVien.HoKhauPhuongXaId = !string.IsNullOrEmpty(workSheet.Cells[i, 21].Text) ? GetPhuongXa(workSheet.Cells[i, 21].Text.ToString()) : null;
                        dataNhanNhanVien.HoKhauDiaChi = !string.IsNullOrEmpty(workSheet.Cells[i, 22].Text) ? workSheet.Cells[i, 22].Text.ToString() : null;

                        dataNhanNhanVien.TenDonViHoacBoPhan = !string.IsNullOrEmpty(workSheet.Cells[i, 23].Text) ? workSheet.Cells[i, 23].Text.ToString() : null;
                        dataNhanNhanVien.NgayBatDauLamViec = ngayBatDauLamViec;

                        dataNhanNhanVien.NhomMau = !string.IsNullOrEmpty(workSheet.Cells[i, 25].Text) ? GetEnumNhomMau(workSheet.Cells[i, 25].Text.ToString()) : null;
                        dataNhanNhanVien.YeuToRh = !string.IsNullOrEmpty(workSheet.Cells[i, 26].Text) ? GetEnumYeuToRh(workSheet.Cells[i, 26].Text.ToString()) : null;
                        dataNhanNhanVien.TinhTrangHonNhan = !string.IsNullOrEmpty(workSheet.Cells[i, 27].Text) ? GetTinhTrangHonNhan(workSheet.Cells[i, 27].Text.ToString()) : null;

                        dataNhanNhanVien.CoMangThai = !string.IsNullOrEmpty(workSheet.Cells[i, 28].Text) ? TinhTrangCoThai(workSheet.Cells[i, 28].Text.ToString()) : null;
                        dataNhanNhanVien.NhomDoiTuongKhamSucKhoe = !string.IsNullOrEmpty(workSheet.Cells[i, 29].Text) ? workSheet.Cells[i, 29].Text.ToString() : null;
                        dataNhanNhanVien.Email = !string.IsNullOrEmpty(workSheet.Cells[i, 30].Text) ? workSheet.Cells[i, 30].Text.ToString() : null;

                        dataNhanNhanVien.NgheCongViecTruocDays = !string.IsNullOrEmpty(workSheet.Cells[i, 31].Text) ? CongViecTruocDay(workSheet.Cells[i, 31].Text.ToString()) : null;
                        dataNhanNhanVien.TienSuBenhs = !string.IsNullOrEmpty(workSheet.Cells[i, 32].Text) ? TienSuBenh(workSheet.Cells[i, 32].Text.ToString()) : null;
                        dataNhanNhanVien.GhiChuDiUngThuoc = !string.IsNullOrEmpty(workSheet.Cells[i, 33].Text) ? workSheet.Cells[i, 33].Text.ToString() : null;

                        #endregion
                        hopDongSucKhoeNhanViens.Add(dataNhanNhanVien);
                    }

                }
                else
                {
                    throw new ApiException(_localizationService.GetResource("KhamDoan.HopDongKhamSucKhoeNhanVien.Required"));
                }
            }

            var nhanVienLoi = listNhanVienError.Select(c => c.MaNV.TrimStart().TrimEnd()).ToList();
            hopDongSucKhoeNhanViens = hopDongSucKhoeNhanViens.Where(c => !nhanVienLoi.Contains(c.MaNhanVien.TrimStart().TrimEnd())).ToList();

            if (hopDongSucKhoeNhanViens.Any())
            {
                //Kiểm tra nhân viên đã thêm trước đó hay chưa hợp đồng chứng minh nhân dân.
                var nhanVienKhamSucKhoes = _khamDoanService.NhanVienKhamSucKhoeTheoHDs(hopDongKhamSucKhoeId).Select(cc => cc.SoChungMinhThu).ToList();
                if (nhanVienKhamSucKhoes.Any())
                {
                    hopDongSucKhoeNhanViens = hopDongSucKhoeNhanViens.Where(c => !nhanVienKhamSucKhoes.Contains(c.SoChungMinhThu)).ToList();
                    if (hopDongSucKhoeNhanViens.Count() == 0)
                    {
                        listNhanVienError.Add(new DanhSachNhapExcelError
                        {
                            TenNhanVien = string.Empty,
                            Error = "Vui lòng kiểm tra số chứng minh thư"
                        });
                    }
                }


                foreach (var viewModel in hopDongSucKhoeNhanViens)
                {
                    viewModel.NgheCongViecTruocDay = viewModel.NgheCongViecTruocDays != null ? JsonConvert.SerializeObject(viewModel.NgheCongViecTruocDays) : null;
                    viewModel.GhiChuTienSuBenh = viewModel.TienSuBenhs != null ? JsonConvert.SerializeObject(viewModel.TienSuBenhs) : null;

                    var entityHopDongKhamSucKhoeNhanVien = viewModel.ToEntity<HopDongKhamSucKhoeNhanVien>();
                    entityHopDongKhamSucKhoeNhanVien.DaLapGiaDinh = viewModel.TinhTrangHonNhan == TinhTrangHonNhan.CoGiaDinh ? true : false;

                    await _khamDoanService.ThemHoacCapNhatHopDongKhamSucKhoeNhanVien(entityHopDongKhamSucKhoeNhanVien);
                }
            }

            if (listNhanVienError.Any())
                listNhanVienError[0].TotalThanhCong = hopDongSucKhoeNhanViens.Count;
            else
                listNhanVienError.Add(new DanhSachNhapExcelError { TotalThanhCong = hopDongSucKhoeNhanViens.Count });

            return Ok(listNhanVienError);
        }

        #region CRUD     

        //Cập nhật số lượng nhân viên khám hợp đồng
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("CapNhatSoLuongNhanVienKhamTrongHopDong")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanHopDongKham)]
        public ActionResult<bool> CapNhatSoLuongNhanVienKhamTrongHopDong(long hopDongKhamId)
        {
            return Ok(_khamDoanService.CapNhatSoLuongNhanVienKhamTrongHopDong(hopDongKhamId));
        }

        [HttpGet("GetThongTinHopDongKham")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanHopDongKham)]
        public ActionResult<KhamDoanHopDongKhamViewModel> GetThongTinHopDongKham(long hopDongKhamId)
        {
            var dataHopDongKham = _khamDoanService.GetThongTinHopDongKham(hopDongKhamId);
            var khamDoanHopDongKhamViewModel = dataHopDongKham.ToModel<KhamDoanHopDongKhamViewModel>();

            khamDoanHopDongKhamViewModel.TenCongTy = dataHopDongKham.CongTyKhamSucKhoe.Ten;
            khamDoanHopDongKhamViewModel.TenLoaiHopDong = LoaiHopDong.KhamSucKhoeCongTy.GetDescription();

            khamDoanHopDongKhamViewModel.TrangThaiHopDongKham = dataHopDongKham.DaKetThuc ? TrangThaiHopDongKham.DaKetThucHD : TrangThaiHopDongKham.DangThucHienHD;
            khamDoanHopDongKhamViewModel.TenTrangThaiHopDongKham = dataHopDongKham.DaKetThuc ? TrangThaiHopDongKham.DaKetThucHD.GetDescription() : TrangThaiHopDongKham.DangThucHienHD.GetDescription();
            
            // Nếu mà kết thúc ngoài gói BVHD-3944
            if (khamDoanHopDongKhamViewModel.TrangThaiHopDongKham == TrangThaiHopDongKham.DaKetThucHD)
            {
                //var nhanVienTheoHopDongs = dataHopDongKham.HopDongKhamSucKhoeNhanViens;
                //var yeuCauTiepNhanChoHDs = nhanVienTheoHopDongs.SelectMany(c => c.YeuCauTiepNhans);
                //var yeuCauKhamBenhs = yeuCauTiepNhanChoHDs.Where(cc => cc.YeuCauKhamBenhs.Any(z => z.GoiKhamSucKhoeId != null && z.TrangThai != EnumTrangThaiYeuCauKhamBenh.ChuaKham && z.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham)).SelectMany(c => c.YeuCauKhamBenhs).ToList();
                //var yeuCauKyThuats = yeuCauTiepNhanChoHDs.Where(cc => cc.YeuCauDichVuKyThuats.Any(z => z.GoiKhamSucKhoeId != null && z.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien && z.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)).SelectMany(c => c.YeuCauDichVuKyThuats).ToList();                
                khamDoanHopDongKhamViewModel.GiaTriHopDong = ((double)_khamDoanService.GetGiaTriHopDong(hopDongKhamId));
            }
            return Ok(khamDoanHopDongKhamViewModel);
        }

        [HttpPost("ThemHoacCapNhatHopDongKham")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.KhamDoanHopDongKham)]
        public async Task<ActionResult<long>> ThemHoacCapNhatHopDongKham(KhamDoanHopDongKhamViewModel viewModel)
        {
            if (!viewModel.HopDongKhamSucKhoeDiaDiems.Any())
            {
                throw new ApiException(_localizationService.GetResource("KhamDoanHopDongKham.ThongTinDiaDiemKhams.Required"));
            }

            //==========================Kiểm tra chức danh tồn tại hay không=====================
            DropDownListRequestModel model = new DropDownListRequestModel();
            model.Query = viewModel.ChucDanhNguoiKy;
            ResourceHelper.CreateChucDanh(model);

            if (viewModel.Id.Equals(0))
            {
                var entityHopDongKham = viewModel.ToEntity<HopDongKhamSucKhoe>();
                await _khamDoanService.ThemHoacCapNhatHopDongKham(entityHopDongKham);

                return Ok(entityHopDongKham.Id);
            }
            else
            {
                var entityHopDongKham = _khamDoanService.GetThongTinHopDongKham(viewModel.Id);
                viewModel.ToEntity(entityHopDongKham);
                await _khamDoanService.ThemHoacCapNhatHopDongKham(entityHopDongKham);

                return Ok(entityHopDongKham.Id);
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpDelete("XuLyXoaHopDongKham")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.KhamDoanHopDongKham)]
        public ActionResult<(long, string)> XuLyXoaHopDongKham(long id)
        {
            return Ok(_khamDoanService.XoaHopDongKham(id));
        }

        [HttpGet("GetThongTinHopDongKhamSucKhoeNhanVien")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanHopDongKham)]
        public ActionResult<HopDongKhamSucKhoeNhanVienViewModel> GetThongTinHopDongKhamSucKhoeNhanVien(long hopDongKhamSucKhoeNhanVienId)
        {
            var dataHopDongKham = _khamDoanService.GetThongTinHopDongKhamSucKhoeNhanVien(hopDongKhamSucKhoeNhanVienId);
            var hopDongKhamSucKhoeNhanVienViewModel = dataHopDongKham.ToModel<HopDongKhamSucKhoeNhanVienViewModel>();

            hopDongKhamSucKhoeNhanVienViewModel.MaBN = dataHopDongKham.BenhNhan?.MaBN;
            hopDongKhamSucKhoeNhanVienViewModel.TinhTrangHonNhan = dataHopDongKham.DaLapGiaDinh ? TinhTrangHonNhan.CoGiaDinh : TinhTrangHonNhan.ChuaCoGiaDinh;

            hopDongKhamSucKhoeNhanVienViewModel.TenGoiKhamSucKhoe = dataHopDongKham.GoiKhamSucKhoe.Ten;

            if (hopDongKhamSucKhoeNhanVienViewModel.ThangSinh != 0 && hopDongKhamSucKhoeNhanVienViewModel.ThangSinh != null && hopDongKhamSucKhoeNhanVienViewModel.NamSinh != null)
            {
                hopDongKhamSucKhoeNhanVienViewModel.NgayThangNamSinh = new DateTime(hopDongKhamSucKhoeNhanVienViewModel.NamSinh ?? 0, hopDongKhamSucKhoeNhanVienViewModel.ThangSinh ?? 0, hopDongKhamSucKhoeNhanVienViewModel.NgaySinh ?? 0);
            }

            hopDongKhamSucKhoeNhanVienViewModel.NgheCongViecTruocDays = hopDongKhamSucKhoeNhanVienViewModel.NgheCongViecTruocDay != null ?
                JsonConvert.DeserializeObject<List<NgheCongViecTruocDay>>(hopDongKhamSucKhoeNhanVienViewModel.NgheCongViecTruocDay) :
                new List<NgheCongViecTruocDay>();

            hopDongKhamSucKhoeNhanVienViewModel.TienSuBenhs = hopDongKhamSucKhoeNhanVienViewModel.GhiChuTienSuBenh != null ?
            JsonConvert.DeserializeObject<List<TienSuBenh>>(hopDongKhamSucKhoeNhanVienViewModel.GhiChuTienSuBenh) : new List<TienSuBenh>();


            hopDongKhamSucKhoeNhanVienViewModel.TinhTrangKham = dataHopDongKham.YeuCauTiepNhans.Any();

            return Ok(hopDongKhamSucKhoeNhanVienViewModel);
        }

        [HttpPost("ThemHoacCapNhatHopDongKhamSucKhoeNhanVien")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.KhamDoanHopDongKham)]
        public async Task<ActionResult<long>> ThemHoacCapNhatHopDongKhamSucKhoeNhanVien(HopDongKhamSucKhoeNhanVienViewModel viewModel)
        {
            viewModel.NgheCongViecTruocDay = viewModel.NgheCongViecTruocDays != null && viewModel.NgheCongViecTruocDays.Any() ? JsonConvert.SerializeObject(viewModel.NgheCongViecTruocDays) : null;
            viewModel.GhiChuTienSuBenh = viewModel.TienSuBenhs != null && viewModel.TienSuBenhs.Any() ? JsonConvert.SerializeObject(viewModel.TienSuBenhs) : null;

            if (viewModel != null)
            {
                if (viewModel.Id.Equals(0))
                {
                    var entityHopDongKhamSucKhoeNhanVien = viewModel.ToEntity<HopDongKhamSucKhoeNhanVien>();
                    entityHopDongKhamSucKhoeNhanVien.DaLapGiaDinh = viewModel.TinhTrangHonNhan == TinhTrangHonNhan.CoGiaDinh;

                    await _khamDoanService.ThemHoacCapNhatHopDongKhamSucKhoeNhanVien(entityHopDongKhamSucKhoeNhanVien);
                    return Ok(entityHopDongKhamSucKhoeNhanVien.Id);
                }
                else
                {
                    var entityHopDongKhamSucKhoeNhanVien = _khamDoanService.GetThongTinHopDongKhamSucKhoeNhanVien(viewModel.Id);
                    viewModel.ToEntity(entityHopDongKhamSucKhoeNhanVien);
                    entityHopDongKhamSucKhoeNhanVien.DaLapGiaDinh = viewModel.TinhTrangHonNhan == TinhTrangHonNhan.CoGiaDinh;

                    if (viewModel.NgayThangNamSinh == null)
                    {
                        entityHopDongKhamSucKhoeNhanVien.ThangSinh = null;
                        entityHopDongKhamSucKhoeNhanVien.NamSinh = null;
                        entityHopDongKhamSucKhoeNhanVien.NamSinh = viewModel.NamSinh;
                    }

                    await _khamDoanService.ThemHoacCapNhatHopDongKhamSucKhoeNhanVien(entityHopDongKhamSucKhoeNhanVien);
                    return Ok(entityHopDongKhamSucKhoeNhanVien.Id);
                }
            }
            return Ok();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("XoaHopDongKhamSucKhoeNhanVien")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanHopDongKham)]
        public ActionResult<bool> XoaHopDongKhamSucKhoeNhanVien(long nhanVienHDKhamId)
        {
            return Ok(_khamDoanService.XoaHopDongKhamSucKhoeNhanVien(nhanVienHDKhamId));
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataDanhSachPhongKhamForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanHopDongKham)]
        public async Task<ActionResult<GridDataSource>> GetDataDanhSachPhongKhamForGridAsync([FromBody] QueryInfo queryInfo)
        {
            long hopDongKhamId = int.Parse(queryInfo.AdditionalSearchString);
            var gridData = await _khamDoanService.GetDanhSachPhongBenhVienGrid(queryInfo, hopDongKhamId);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDanhSachPhongKhamPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanHopDongKham)]
        public async Task<ActionResult<GridDataSource>> GetTotalDanhSachPhongKhamPageForGridAsync([FromBody] QueryInfo queryInfo)
        {
            long hopDongKhamId = int.Parse(queryInfo.AdditionalSearchString);
            var gridData = await _khamDoanService.GetTotalDanhSachPhongBenhVienGrid(queryInfo, hopDongKhamId);
            return Ok(gridData);
        }

        [HttpPost("ThemHoacSuaDanhSachPhongKhamTaiCongTy")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.KhamDoanHopDongKham)]
        public async Task<ActionResult<long>> ThemHoacSuaDanhSachPhongKhamTaiCongTy(DanhSachPhongKhamTaiCongTyViewModel viewModel)
        {
            if (viewModel != null)
            {
                if (viewModel.Id.Equals(0))
                {
                    var entityPhongBenhVien = viewModel.ToEntity<PhongBenhVien>();
                    await _khamDoanService.ThemHoacCapNhatDanhSachPhongKhamCongTy(entityPhongBenhVien, viewModel.DanhSachNhanSu);
                    return Ok(entityPhongBenhVien.Id);
                }
                else
                {
                    var entityPhongBenhVien = _khamDoanService.GetPhongBenhVien(viewModel.Id);
                    viewModel.ToEntity(entityPhongBenhVien);

                    await _khamDoanService.ThemHoacCapNhatDanhSachPhongKhamCongTy(entityPhongBenhVien, viewModel.DanhSachNhanSu);
                    return Ok(entityPhongBenhVien.Id);
                }
            }
            return Ok();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("XoaPhongKhamTaiCongTy")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanHopDongKham)]
        public ActionResult<bool> XoaPhongKhamTaiCongTy(long id)
        {
            return Ok(_khamDoanService.XoaPhongKhamTaiCongTy(id));
        }

        [HttpPost("GetListDanhSachNhanSuMultiSelect")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanHopDongKham)]
        public ActionResult<LookupItemVo> GetListDanhSachNhanSuMultiSelect([FromBody] DropDownListRequestModel queryInfo, long hopDongKhamSucKhoeId)
        {
            return Ok(_khamDoanService.GetListDanhSachNhanSuMultiSelect(queryInfo, hopDongKhamSucKhoeId));
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("KiemTraHopDongNhanVienChuaKham")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanHopDongKham)]
        public ActionResult KetThucHopDong(long hopDongKhamId)
        {
            var danhSachBenhNhanChuaKhamXong = _khamDoanService.KiemTraHopDongNhanVienChuaKham(hopDongKhamId);
            return Ok(danhSachBenhNhanChuaKhamXong);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("KetThucHopDongKham")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanHopDongKham)]
        public ActionResult KetThucHopDongKham(long hopDongKhamId)
        {
            _khamDoanService.KetThucHopDongKham(hopDongKhamId);
            return Ok();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("MoLaiHopDongKham")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanHopDongKham)]
        public ActionResult MoLaiHopDongKham(MoHopDongKhamViewModel moHopDongKhamViewModel)
        {
            _khamDoanService.MoLaiHopDongKhamSucKhoe(moHopDongKhamViewModel);
            return Ok();
        }

        [HttpPost("XoaAllNhanVienChuaKham")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.KhamDoanHopDongKham)]
        public async Task<ActionResult<bool>> XoaAllNhanVienChuaKham(long hopDongKhamSucKhoeId)
        {
            return Ok(await _khamDoanService.XoaTatCaNhanVienChuaKham(hopDongKhamSucKhoeId));
        }

        #endregion

        #region Thông tin Import/Export Excel

        #region Thông tin Export

        [HttpPost("ExportDanhSachHopDongKham")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.KhamDoanHopDongKham)]
        public async Task<ActionResult> ExportDanhSachHopDongKham(QueryInfo queryInfo)
        {
            var gridData = await _khamDoanService.GetDSHopDongKhamForGrid(queryInfo, true);

            var DanhSachKhamSucKhoeDoanData = gridData.Data.Select(p => (KhamDoanHopDongKhamGridVo)p).ToList();
            var excelData = DanhSachKhamSucKhoeDoanData.Map<List<HopDongKhamExportExcel>>();

            var lstValueObject = new List<(string, string)>
            {
                (nameof(HopDongKhamExportExcel.SoHopDong), "SHĐ"),
                (nameof(HopDongKhamExportExcel.TenCongTy), "Tên công ty"),
                (nameof(HopDongKhamExportExcel.NgayHopDongDisplay), "Ngày hợp đồng"),
                (nameof(HopDongKhamExportExcel.DiaChiKham), "Địa điểm khám"),
                (nameof(HopDongKhamExportExcel.LoaiHopDongDisplay), "Loại hợp đồng"),
                (nameof(HopDongKhamExportExcel.NgayKham), "Ngày khám"),
                (nameof(HopDongKhamExportExcel.TenTrangThai), "Trạng thái")
            };
            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "DS Hợp Đồng Khám", 2, "DS Hợp Đồng Khám");
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DSHopDongKham" + DateTime.Now.Year + ".xlsx");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        #endregion

        #region Thông tin Import

        private bool KiemTraGoiKhamDungChung(long goiKhamId)
        {
            return _khamDoanService.KiemTraGoiKhamDungDungGoiChung(goiKhamId);
        }

        private NhomDichVuChiDinhKhamSucKhoe GetNhomDichVuChiDinhKhamSucKhoe(string nhomDichVuChiDinhKhamSucKhoe)
        {
            if (nhomDichVuChiDinhKhamSucKhoe == Enums.NhomDichVuChiDinhKhamSucKhoe.ChuanDoanHinhAnh.GetDescription())
                return Enums.NhomDichVuChiDinhKhamSucKhoe.ChuanDoanHinhAnh;
            else if (nhomDichVuChiDinhKhamSucKhoe == Enums.NhomDichVuChiDinhKhamSucKhoe.ThamDoChucNang.GetDescription())
                return Enums.NhomDichVuChiDinhKhamSucKhoe.ThamDoChucNang;
            else if (nhomDichVuChiDinhKhamSucKhoe == Enums.NhomDichVuChiDinhKhamSucKhoe.KhamBenh.GetDescription())
                return Enums.NhomDichVuChiDinhKhamSucKhoe.KhamBenh;
            else if (nhomDichVuChiDinhKhamSucKhoe == Enums.NhomDichVuChiDinhKhamSucKhoe.XetNghiem.GetDescription())
                return Enums.NhomDichVuChiDinhKhamSucKhoe.XetNghiem;
            else
                return Enums.NhomDichVuChiDinhKhamSucKhoe.XetNghiem;
        }

        private ChuyenKhoaKhamSucKhoe GetChuyenKhoaKhamSucKhoe(string chuyenKhoaKhamSucKhoe)
        {
            if (chuyenKhoaKhamSucKhoe == Enums.ChuyenKhoaKhamSucKhoe.DaLieu.GetDescription())
                return Enums.ChuyenKhoaKhamSucKhoe.DaLieu;
            else if (chuyenKhoaKhamSucKhoe == Enums.ChuyenKhoaKhamSucKhoe.Mat.GetDescription())
                return Enums.ChuyenKhoaKhamSucKhoe.Mat;
            else if (chuyenKhoaKhamSucKhoe == Enums.ChuyenKhoaKhamSucKhoe.NgoaiKhoa.GetDescription())
                return Enums.ChuyenKhoaKhamSucKhoe.NgoaiKhoa;
            else if (chuyenKhoaKhamSucKhoe == Enums.ChuyenKhoaKhamSucKhoe.NoiKhoa.GetDescription())
                return Enums.ChuyenKhoaKhamSucKhoe.NoiKhoa;
            else if (chuyenKhoaKhamSucKhoe == Enums.ChuyenKhoaKhamSucKhoe.RangHamMat.GetDescription())
                return Enums.ChuyenKhoaKhamSucKhoe.RangHamMat;
            else if (chuyenKhoaKhamSucKhoe == Enums.ChuyenKhoaKhamSucKhoe.SanPhuKhoa.GetDescription())
                return Enums.ChuyenKhoaKhamSucKhoe.SanPhuKhoa;
            else if (chuyenKhoaKhamSucKhoe == Enums.ChuyenKhoaKhamSucKhoe.TaiMuiHong.GetDescription())
                return Enums.ChuyenKhoaKhamSucKhoe.TaiMuiHong;
            else
                return Enums.ChuyenKhoaKhamSucKhoe.DaLieu;
        }

        private HinhThucKhamBenh GetHinhThucKhamBenh(string hinhThucKhamBenh)
        {
            if (MaskHelper.RemoveVietnameseDiacritics(hinhThucKhamBenh).Trim() == MaskHelper.RemoveVietnameseDiacritics(Enums.HinhThucKhamBenh.NoiVien.GetDescription()).Trim())
                return Enums.HinhThucKhamBenh.NoiVien;
            else if (MaskHelper.RemoveVietnameseDiacritics(hinhThucKhamBenh).Trim() == MaskHelper.RemoveVietnameseDiacritics(Enums.HinhThucKhamBenh.KhamDoanNgoaiVien.GetDescription()).Trim())
                return Enums.HinhThucKhamBenh.KhamDoanNgoaiVien;
            else
                return Enums.HinhThucKhamBenh.NoiVien;
        }

        private EnumNhomMau? GetEnumNhomMau(string nhomMau)
        {
            if (nhomMau == Enums.EnumNhomMau.A.GetDescription())
                return Enums.EnumNhomMau.A;
            else if (nhomMau == Enums.EnumNhomMau.B.GetDescription())
                return Enums.EnumNhomMau.B;
            else if (nhomMau == Enums.EnumNhomMau.AB.GetDescription())
                return Enums.EnumNhomMau.AB;
            else if (nhomMau == Enums.EnumNhomMau.O.GetDescription())
                return Enums.EnumNhomMau.O;
            else
                return null;
        }

        private EnumYeuToRh? GetEnumYeuToRh(string yeuToRH)
        {
            if (MaskHelper.RemoveVietnameseDiacritics(yeuToRH.ToUpper()).Trim() == MaskHelper.RemoveVietnameseDiacritics(Enums.EnumYeuToRh.Amtinh.GetDescription().ToUpper()).Trim())
                return Enums.EnumYeuToRh.Amtinh;
            else
            if (MaskHelper.RemoveVietnameseDiacritics(yeuToRH.ToUpper()).Trim() == MaskHelper.RemoveVietnameseDiacritics(Enums.EnumYeuToRh.DuongTinh.GetDescription().ToUpper()).Trim())
                return Enums.EnumYeuToRh.DuongTinh;
            else
                return null;
        }

        private TinhTrangHonNhan? GetTinhTrangHonNhan(string tinhTrangHonNhan)
        {
            if (MaskHelper.RemoveVietnameseDiacritics(tinhTrangHonNhan.ToUpper()).Trim() == MaskHelper.RemoveVietnameseDiacritics(Enums.TinhTrangHonNhan.CoGiaDinh.GetDescription().ToUpper()).Trim())
                return Enums.TinhTrangHonNhan.CoGiaDinh;
            else if (MaskHelper.RemoveVietnameseDiacritics(tinhTrangHonNhan.ToUpper()).Trim() == MaskHelper.RemoveVietnameseDiacritics(Enums.TinhTrangHonNhan.ChuaCoGiaDinh.GetDescription().ToUpper()).Trim())
                return Enums.TinhTrangHonNhan.ChuaCoGiaDinh;
            else
                return null;
        }

        private long GetNhomGiaDichVuKyThuatBenhVien(bool laDichVuKham, string tenLoaiGia)
        {
            DropDownListRequestModel requestModel = new DropDownListRequestModel();
            requestModel.Query = tenLoaiGia;
            requestModel.Take = 10;

            if (laDichVuKham)
            {
                return _tiepNhanBenhNhanService.GetLoaiGiaKhamBenh(requestModel).Result.FirstOrDefault().KeyId;
            }
            else
            {
                return _khamDoanService.GetLoaiGiaDichVuKyThuat(requestModel).Result.FirstOrDefault().KeyId;
            }
        }

        private long DichVuKhamBenhBVHoacDVKTBenhVien(string tenLoaiGia)
        {
            DropDownListRequestModel requestModel = new DropDownListRequestModel();
            requestModel.Query = tenLoaiGia;
            requestModel.Take = 10;
            var model = _khamDoanService.GetDVKBVaDVKT(requestModel).Result;

            return model;
        }

        private List<GoiKhamSucKhoeNoiThucHienViewModel> GoiKhamSucKhoeNoiThucHienViewModel(string NoiThucHienString, bool LaDichVuKham, long DichVuKyThuatBenhVienId)
        {
            var phongBenhViens = NoiThucHienString.Split(';');
            var res = new List<GoiKhamSucKhoeNoiThucHienViewModel>();
            if (phongBenhViens.Any())
            {
                foreach (var phongBenhVien in phongBenhViens)
                {
                    var goiKhamSucKhoeNoiThucHienViewModel = new GoiKhamSucKhoeNoiThucHienViewModel();

                    DropDownListRequestModel requestModel = new DropDownListRequestModel();
                    requestModel.Query = phongBenhVien;
                    requestModel.Take = 10;

                    goiKhamSucKhoeNoiThucHienViewModel.PhongBenhVienId = _khamDoanService.GetPhongBenhVienTheoTen(requestModel).Any() ? _khamDoanService.GetPhongBenhVienTheoTen(requestModel).FirstOrDefault().KeyId : 0;

                    if (LaDichVuKham)
                    {
                        goiKhamSucKhoeNoiThucHienViewModel.GoiKhamSucKhoeDichVuKhamBenhId = DichVuKyThuatBenhVienId;
                    }
                    else
                    {
                        goiKhamSucKhoeNoiThucHienViewModel.GoiKhamSucKhoeDichVuDichVuKyThuatId = DichVuKyThuatBenhVienId;
                    }
                    res.Add(goiKhamSucKhoeNoiThucHienViewModel);
                }
            }
            return res;
        }

        private bool? KiemTraGioiTinhNam(string gioiTinh)
        {
            return gioiTinh.ToUpper().Trim() == LoaiGioiTinh.GioiTinhNam.GetDescription().ToUpper();
        }

        private bool? KiemTraGioiTinhNu(string gioiTinh)
        {
            return gioiTinh.ToUpper().Trim() == LoaiGioiTinh.GioiTinhNu.GetDescription().ToUpper();
        }

        private bool? KiemTraCoThai(string thai)
        {
            return thai == "Có";
        }

        private bool? KiemTraKhongThai(string thai)
        {
            return thai == "Không";
        }

        private bool? KiemTraDaLapGD(string gd)
        {
            return gd == "Đã lập gia đình";
        }

        private bool? KiemTraChuaLapGiaDinh(string gd)
        {
            return gd == "Chưa lập gia đình";
        }

        private long? GetThongTinNgheNghiep(string tenNgheNghiep)
        {
            DropDownListRequestModel requestModel = new DropDownListRequestModel();
            requestModel.Query = tenNgheNghiep;
            requestModel.Take = 10;
            var res = _tiepNhanBenhNhanService.GetNgheNghiep(requestModel).Result;
            if (res.Any())
                return res.FirstOrDefault().KeyId;
            else
                return null;
        }

        private long? GetQuocTich(string tenQuocTich)
        {
            DropDownListRequestModel requestModel = new DropDownListRequestModel();
            requestModel.Query = tenQuocTich;
            requestModel.Take = 10;

            var res = _tiepNhanBenhNhanService.GetQuocTich(requestModel).Result;
            if (res.Any())
                return res.FirstOrDefault().KeyId;
            else
                return null;
        }

        private long? GetDanToc(string tenDanToc)
        {
            DropDownListRequestModel requestModel = new DropDownListRequestModel();
            requestModel.Query = tenDanToc;
            requestModel.Take = 10;

            var res = _tiepNhanBenhNhanService.GetDanToc(requestModel).Result;
            if (res.Any())
                return res.FirstOrDefault().KeyId;
            else
                return null;
        }

        private long? GetTinhThanh(string tenTinhThanh)
        {
            DropDownListRequestModel requestModel = new DropDownListRequestModel();
            requestModel.Query = tenTinhThanh;
            requestModel.Take = 10;

            var res = _tiepNhanBenhNhanService.GetTinhThanh(requestModel, 0).Result;
            if (res.Any())
                return res.FirstOrDefault().KeyId;
            else
                return null;
        }

        private long? GetQuanHuyen(string tenQuanHuyen)
        {
            DropDownListRequestModel requestModel = new DropDownListRequestModel();
            requestModel.Query = tenQuanHuyen;
            requestModel.Take = 10;

            var res = _tiepNhanBenhNhanService.GetQuanHuyen(requestModel, 0).Result;
            if (res.Any())
                return res.FirstOrDefault().KeyId;
            else
                return null;
        }

        private long? GetPhuongXa(string tenPhuongXa)
        {
            DropDownListRequestModel requestModel = new DropDownListRequestModel();
            requestModel.Query = tenPhuongXa;
            requestModel.Take = 10;

            var res = _tiepNhanBenhNhanService.GetPhuongXa(requestModel).Result;
            if (res.Any())
                return res.FirstOrDefault().KeyId;
            else
                return null;
        }

        private long? GetGoiKham(string tenGoiKham, long hopDongGoiKhamId)
        {
            var goiKham = _khamDoanService.GetGoiKhamTheoTenVaHopDong(tenGoiKham, hopDongGoiKhamId).Result;
            if (goiKham != null)
                return goiKham.Id;
            else
                return null;
        }

        private bool KiemTraMaGoiKham(string maGoiKham, long hopDongGoiKhamId)
        {
            return _khamDoanService.KiemTraMaGoiKham(maGoiKham, hopDongGoiKhamId);
        }

        private List<NgheCongViecTruocDay> CongViecTruocDay(string congViecTruocDay)
        {
            var cvs = congViecTruocDay.Split(',');
            var res = new List<NgheCongViecTruocDay>();
            if (cvs.Any())
            {
                foreach (var cv in cvs)
                {
                    var items = cv.Split('|');
                    if (items.Any())
                    {
                        var data = new NgheCongViecTruocDay();
                        data.CongViec = items[0];

                        DateTime? tuNgay = DateTime.MinValue;
                        if (!String.IsNullOrEmpty(items[1]))
                        {
                            tuNgay = DateTime.ParseExact(items[1], "dd/MM/yyyy", null);
                        }
                        else
                        {
                            tuNgay = null;
                        }

                        data.TuNgay = tuNgay;

                        DateTime? denNgay = DateTime.MinValue;
                        if (!String.IsNullOrEmpty(items[2]))
                        {
                            denNgay = DateTime.ParseExact(items[1], "dd/MM/yyyy", null);
                        }
                        else
                        {
                            denNgay = null;
                        }

                        data.DenNgay = denNgay;
                        res.Add(data);
                    }
                }
            }
            return res;
        }

        private List<TienSuBenh> TienSuBenh(string tienSuBenh)
        {
            var cvs = tienSuBenh.Split(',');
            var res = new List<TienSuBenh>();
            if (cvs.Any())
            {
                foreach (var cv in cvs)
                {
                    var items = cv.Split('|');
                    if (items.Any())
                    {
                        var data = new TienSuBenh();
                        data.LoaiTienSuId = EnumLoaiTienSuBenh.BanThan.GetDescription() == (items[0]).ToString() ? (int)EnumLoaiTienSuBenh.BanThan : (int)EnumLoaiTienSuBenh.GiaDinh;
                        data.LoaiTienSu = EnumLoaiTienSuBenh.BanThan.GetDescription() == (items[0]).ToString() ? EnumLoaiTienSuBenh.BanThan.GetDescription() : EnumLoaiTienSuBenh.GiaDinh.GetDescription();
                        data.TenBenh = items[1];

                        DateTime? phatHienNam = DateTime.MinValue;
                        if (!String.IsNullOrEmpty(items[2]))
                        {
                            phatHienNam = DateTime.ParseExact(items[1], "dd/MM/yyyy", null);
                        }
                        else
                        {
                            phatHienNam = null;
                        }

                        data.PhatHienNam = phatHienNam;

                        res.Add(data);
                    }
                }
            }
            return res;
        }

        private bool? TinhTrangCoThai(string tinhTrangHonNhan)
        {
            return tinhTrangHonNhan.ToUpper() == ("Mang thai").ToUpper() ? true : false;
        }

        #endregion

        #endregion

        #region Thống kê
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsyncBangKeDichVuKhamDoan")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.KhamDoanHopDongKham)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncBangKeDichVuKhamDoan([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _khamDoanService.GetDataForGridAsyncBangKeDichVuKhamDoan(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsyncBangKeDichVuKhamDoan")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.KhamDoanHopDongKham)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncBangKeDichVuKhamDoan([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _khamDoanService.GetTotalPageForGridAsyncBangKeDichVuKhamDoan(queryInfo);
            return Ok(gridData);
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsyncBangKeDichVuKhamDoanChiTiet")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.KhamDoanHopDongKham)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncBangKeDichVuKhamDoanChiTiet([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _khamDoanService.GetDataForGridAsyncBangKeDichVuKhamDoanChiTiet(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsyncBangKeDichVuKhamDoanChiTiet")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.KhamDoanHopDongKham)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncBangKeDichVuKhamDoanChiTiet([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _khamDoanService.GetTotalPageForGridAsyncBangKeDichVuKhamDoanChiTiet(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetTotalDanhSachNhanVienCongTyTheoHopDongKham")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanHopDongKham)]
        public ActionResult<int> GetTotalDanhSachNhanVienCongTyTheoHopDongKham(long hopDongKhamId)
        {
            var totalNhanVien = _khamDoanService.GetTotalDanhSachNhanVienCongTyTheoHopDongKham(hopDongKhamId);
            return Ok(totalNhanVien);
        }


        #region Export excel
        [HttpPost("XuatFileExcelTrongGoi")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.KhamDoanHopDongKham)]
        public async Task<ActionResult> XuatFileExcelTrongGoi(XuatFileExcelTrongGoi xuatFileExcelTrongGoi)
        {
            if (xuatFileExcelTrongGoi.LaDichVuTrongGoi)
            {
                var tenCongTy = await _khamDoanService.GetTenCongTyTheoHopDong(xuatFileExcelTrongGoi.HopDongKhamSucKhoeId);
                var dichVuKhamDoanChiTiets = _khamDoanService.ExportExcelNhanVienDichVuTrongGois(xuatFileExcelTrongGoi.HopDongKhamSucKhoeId);
                var bytes = _khamDoanService.ExportDichVuKhamDoanChiTiets(dichVuKhamDoanChiTiets, tenCongTy);
                HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=TongHopChiPhiKetQuaKSKTheoHopDong" + DateTime.Now.Year + ".xls");
                Response.ContentType = "application/vnd.ms-excel";
                return new FileContentResult(bytes, "application/vnd.ms-excel");
            }
            else
            {
                var tenCongTy = await _khamDoanService.GetTenCongTyTheoHopDong(xuatFileExcelTrongGoi.HopDongKhamSucKhoeId);
                var dichVuKhamDoanChiTiets = _khamDoanService.ExportExcelNhanVienDichVuNgoaiGois(xuatFileExcelTrongGoi.HopDongKhamSucKhoeId);
                var bytes = _khamDoanService.ExportDichVuKhamDoanChiTiets(dichVuKhamDoanChiTiets, tenCongTy);
                HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=TongHopChiPhiKetQuaKSKTheoHopDong" + DateTime.Now.Year + ".xls");
                Response.ContentType = "application/vnd.ms-excel";
                return new FileContentResult(bytes, "application/vnd.ms-excel");
            }
        }
        #endregion
        #endregion
    }
}
