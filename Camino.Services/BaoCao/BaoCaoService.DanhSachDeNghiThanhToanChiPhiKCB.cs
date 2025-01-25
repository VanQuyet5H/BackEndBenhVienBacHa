using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Camino.Services.ExportImport.Help;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using Camino.Core.Domain.ValueObject.DanhSachDeNghiThanhToanChiPhiKCB;
using static Camino.Core.Domain.Enums;
using System;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Domain.ValueObject.ExcelChungTu;
using Newtonsoft.Json;
using Camino.Core.Domain.ValueObject.BHYT;
using Camino.Services.Helpers;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        public async Task<GridDataSource> GetDanhSachDeNghiThanhToanChiPhiKCBNgoaiTruForGrid(DanhSachDeNghiThanhToanChiPhiKCBQueryInfoVo queryInfo)
        {
            var hinhThucKCB = EnumMaHoaHinhThucKCB.KhamBenh;
            var allYeuCauTiepNhanIds = _yeuCauTiepNhanDuLieuGuiCongBHYT.TableNoTracking
                .Where(o => (o.HinhThucKCB == null || o.HinhThucKCB == hinhThucKCB)
                            && (o.NgayRa == null || (queryInfo.TuNgay <= o.NgayRa && o.NgayRa < queryInfo.DenNgay)))
                .Select(o => o.YeuCauTiepNhanId)
                .Distinct().ToList();

            var allDuLieuGuiCongBHYTs = _yeuCauTiepNhanDuLieuGuiCongBHYT.TableNoTracking
                .Where(o => allYeuCauTiepNhanIds.Contains(o.YeuCauTiepNhanId))
                .Select(o => new { o.Id, o.YeuCauTiepNhanId, o.CoGuiCong, o.Version, o.HinhThucKCB, o.NgayRa })
                .ToList();

            var groupDuLieuGuiCongBHYTs = allDuLieuGuiCongBHYTs.GroupBy(o => o.YeuCauTiepNhanId);

            List<long> duLieuGuiCongBHYTIds = new List<long>();
            foreach (var g in groupDuLieuGuiCongBHYTs)
            {
                var last = g.Where(o => o.CoGuiCong == true).OrderBy(o => o.Version).ThenBy(o => o.Id).LastOrDefault();
                if (last != null && (last.HinhThucKCB == null || last.HinhThucKCB == hinhThucKCB)
                            && (last.NgayRa == null || (queryInfo.TuNgay <= last.NgayRa && last.NgayRa < queryInfo.DenNgay)))
                {
                    duLieuGuiCongBHYTIds.Add(last.Id);
                }
            }

            var duLieuGuiCongBHYTChiTiets = _yeuCauTiepNhanDuLieuGuiCongBHYT.TableNoTracking
                .Where(o => duLieuGuiCongBHYTIds.Contains(o.Id))
                .Select(o => new { o.Id, o.YeuCauTiepNhanId, o.DuLieu })
                .OrderBy(o => o.Id).ToList();

            var returnData = new List<ExcelFile7980A>();
            foreach (var duLieuGuiCongBHYTChiTiet in duLieuGuiCongBHYTChiTiets)
            {
                if (!string.IsNullOrEmpty(duLieuGuiCongBHYTChiTiet.DuLieu))
                {
                    var thongTinBenhNhanGoiBHYT = JsonConvert.DeserializeObject<ThongTinBenhNhan>(duLieuGuiCongBHYTChiTiet.DuLieu);
                    if (thongTinBenhNhanGoiBHYT != null && thongTinBenhNhanGoiBHYT.MaLoaiKCB == hinhThucKCB
                            && queryInfo.TuNgay <= thongTinBenhNhanGoiBHYT.NgayRa && thongTinBenhNhanGoiBHYT.NgayRa < queryInfo.DenNgay)
                    {
                        returnData.Add(BHYTHelper.MapThongTinBenhNhanToExcelFile7980A(thongTinBenhNhanGoiBHYT));
                    }
                }
            }

            var cauHinhBaoHiemYTe = _cauHinhService.LoadSetting<BaoHiemYTe>();

            var dsTiepNhanKhamChuaBenhBanDaus = returnData
                .Where(o => o.MaDKBD == cauHinhBaoHiemYTe.BenhVienTiepNhan)
                .ToList();
            var dsTiepNhanNoiTinhs = returnData
                .Where(o => o.MaDKBD != cauHinhBaoHiemYTe.BenhVienTiepNhan && o.MaDKBD != null && o.MaDKBD.StartsWith("01"))
                .ToList();
            var dsTiepNhanNgoaiTinhs = returnData
                .Where(o => o.MaDKBD != cauHinhBaoHiemYTe.BenhVienTiepNhan && (o.MaDKBD == null || !o.MaDKBD.StartsWith("01")))
                .ToList();

            var dsNgoaiTrus = new List<DanhSachDeNghiThanhToanChiPhiKCBNgoaiTru>();

            var dsTiepNhanKhamChuaBenhBanDauDungTuyens = dsTiepNhanKhamChuaBenhBanDaus.Where(o => o.MaLyDoVaoVien == (int)EnumLyDoVaoVien.DungTuyen).ToList();
            dsNgoaiTrus.Add(TinhTongChiPhiKhamBenh(dsTiepNhanKhamChuaBenhBanDauDungTuyens, NoiDangKyKCBBanDau.BenhNhanNoiTinhKhamChuaBenhBanDau, EnumLyDoVaoVien.DungTuyen));

            var dsTiepNhanKhamChuaBenhBanDauTraiTuyens = dsTiepNhanKhamChuaBenhBanDaus.Where(o => o.MaLyDoVaoVien == (int)EnumLyDoVaoVien.TraiTuyen).ToList();
            if (dsTiepNhanKhamChuaBenhBanDauTraiTuyens.Any())
                dsNgoaiTrus.Add(TinhTongChiPhiKhamBenh(dsTiepNhanKhamChuaBenhBanDauTraiTuyens, NoiDangKyKCBBanDau.BenhNhanNoiTinhKhamChuaBenhBanDau, EnumLyDoVaoVien.TraiTuyen));

            var dsTiepNhanKhamChuaBenhBanDauThongTuyens = dsTiepNhanKhamChuaBenhBanDaus.Where(o => o.MaLyDoVaoVien == (int)EnumLyDoVaoVien.ThongTuyen).ToList();
            if (dsTiepNhanKhamChuaBenhBanDauThongTuyens.Any())
                dsNgoaiTrus.Add(TinhTongChiPhiKhamBenh(dsTiepNhanKhamChuaBenhBanDauThongTuyens, NoiDangKyKCBBanDau.BenhNhanNoiTinhKhamChuaBenhBanDau, EnumLyDoVaoVien.ThongTuyen));

            var dsTiepNhanNoiTinhDungTuyens = dsTiepNhanNoiTinhs.Where(o => o.MaLyDoVaoVien == (int)EnumLyDoVaoVien.DungTuyen).ToList();
            dsNgoaiTrus.Add(TinhTongChiPhiKhamBenh(dsTiepNhanNoiTinhDungTuyens, NoiDangKyKCBBanDau.BenhNhanNoiTinhDen, EnumLyDoVaoVien.DungTuyen));

            var dsTiepNhanNoiTinhTraiTuyens = dsTiepNhanNoiTinhs.Where(o => o.MaLyDoVaoVien == (int)EnumLyDoVaoVien.TraiTuyen).ToList();
            if (dsTiepNhanNoiTinhTraiTuyens.Any())
                dsNgoaiTrus.Add(TinhTongChiPhiKhamBenh(dsTiepNhanNoiTinhTraiTuyens, NoiDangKyKCBBanDau.BenhNhanNoiTinhDen, EnumLyDoVaoVien.TraiTuyen));

            var dsTiepNhanNoiTinhThongTuyens = dsTiepNhanNoiTinhs.Where(o => o.MaLyDoVaoVien == (int)EnumLyDoVaoVien.ThongTuyen).ToList();
            if (dsTiepNhanNoiTinhThongTuyens.Any())
                dsNgoaiTrus.Add(TinhTongChiPhiKhamBenh(dsTiepNhanNoiTinhThongTuyens, NoiDangKyKCBBanDau.BenhNhanNoiTinhDen, EnumLyDoVaoVien.ThongTuyen));

            var dsTiepNhanNgoaiTinhDungTuyens = dsTiepNhanNgoaiTinhs.Where(o => o.MaLyDoVaoVien == (int)EnumLyDoVaoVien.DungTuyen).ToList();
            dsNgoaiTrus.Add(TinhTongChiPhiKhamBenh(dsTiepNhanNgoaiTinhDungTuyens, NoiDangKyKCBBanDau.BenhNhanNgoaiTinhDen, EnumLyDoVaoVien.DungTuyen));

            var dsTiepNhanNgoaiTinhTraiTuyens = dsTiepNhanNgoaiTinhs.Where(o => o.MaLyDoVaoVien == (int)EnumLyDoVaoVien.TraiTuyen).ToList();
            if (dsTiepNhanNgoaiTinhTraiTuyens.Any())
                dsNgoaiTrus.Add(TinhTongChiPhiKhamBenh(dsTiepNhanNgoaiTinhTraiTuyens, NoiDangKyKCBBanDau.BenhNhanNgoaiTinhDen, EnumLyDoVaoVien.TraiTuyen));

            var dsTiepNhanNgoaiTinhThongTuyens = dsTiepNhanNgoaiTinhs.Where(o => o.MaLyDoVaoVien == (int)EnumLyDoVaoVien.ThongTuyen).ToList();
            if (dsTiepNhanNgoaiTinhThongTuyens.Any())
                dsNgoaiTrus.Add(TinhTongChiPhiKhamBenh(dsTiepNhanNgoaiTinhThongTuyens, NoiDangKyKCBBanDau.BenhNhanNgoaiTinhDen, EnumLyDoVaoVien.ThongTuyen));
                       
            return new GridDataSource
            {
                Data = dsNgoaiTrus.ToArray(),
                TotalRowCount = dsNgoaiTrus.Count()
            };
        }

        public async Task<GridDataSource> GetDanhSachDeNghiThanhToanChiPhiKCBNoiTruForGrid(DanhSachDeNghiThanhToanChiPhiKCBQueryInfoVo queryInfo)
        {
            var hinhThucKCB = EnumMaHoaHinhThucKCB.DieuTriNoiTru;
            var allYeuCauTiepNhanIds = _yeuCauTiepNhanDuLieuGuiCongBHYT.TableNoTracking
                .Where(o => (o.HinhThucKCB == null || o.HinhThucKCB == hinhThucKCB)
                            && (o.NgayRa == null || (queryInfo.TuNgay <= o.NgayRa && o.NgayRa < queryInfo.DenNgay)))
                .Select(o => o.YeuCauTiepNhanId)
                .Distinct().ToList();

            var allDuLieuGuiCongBHYTs = _yeuCauTiepNhanDuLieuGuiCongBHYT.TableNoTracking
                .Where(o => allYeuCauTiepNhanIds.Contains(o.YeuCauTiepNhanId))
                .Select(o => new { o.Id, o.YeuCauTiepNhanId, o.CoGuiCong, o.Version, o.HinhThucKCB, o.NgayRa })
                .ToList();

            var groupDuLieuGuiCongBHYTs = allDuLieuGuiCongBHYTs.GroupBy(o => o.YeuCauTiepNhanId);

            List<long> duLieuGuiCongBHYTIds = new List<long>();
            foreach (var g in groupDuLieuGuiCongBHYTs)
            {
                var last = g.Where(o => o.CoGuiCong == true).OrderBy(o => o.Version).ThenBy(o => o.Id).LastOrDefault();
                if (last != null && (last.HinhThucKCB == null || last.HinhThucKCB == hinhThucKCB)
                            && (last.NgayRa == null || (queryInfo.TuNgay <= last.NgayRa && last.NgayRa < queryInfo.DenNgay)))
                {
                    duLieuGuiCongBHYTIds.Add(last.Id);
                }
            }

            var duLieuGuiCongBHYTChiTiets = _yeuCauTiepNhanDuLieuGuiCongBHYT.TableNoTracking
                .Where(o => duLieuGuiCongBHYTIds.Contains(o.Id))
                .Select(o => new { o.Id, o.YeuCauTiepNhanId, o.DuLieu })
                .OrderBy(o => o.Id).ToList();

            var returnData = new List<ExcelFile7980A>();
            foreach (var duLieuGuiCongBHYTChiTiet in duLieuGuiCongBHYTChiTiets)
            {
                if (!string.IsNullOrEmpty(duLieuGuiCongBHYTChiTiet.DuLieu))
                {
                    var thongTinBenhNhanGoiBHYT = JsonConvert.DeserializeObject<ThongTinBenhNhan>(duLieuGuiCongBHYTChiTiet.DuLieu);
                    if (thongTinBenhNhanGoiBHYT != null && thongTinBenhNhanGoiBHYT.MaLoaiKCB == hinhThucKCB
                            && queryInfo.TuNgay <= thongTinBenhNhanGoiBHYT.NgayRa && thongTinBenhNhanGoiBHYT.NgayRa < queryInfo.DenNgay)
                    {
                        returnData.Add(BHYTHelper.MapThongTinBenhNhanToExcelFile7980A(thongTinBenhNhanGoiBHYT));
                    }
                }
            }

            var cauHinhBaoHiemYTe = _cauHinhService.LoadSetting<BaoHiemYTe>();

            var dsTiepNhanKhamChuaBenhBanDaus = returnData
                .Where(o => o.MaDKBD == cauHinhBaoHiemYTe.BenhVienTiepNhan)
                .ToList();
            var dsTiepNhanNoiTinhs = returnData
                .Where(o => o.MaDKBD != cauHinhBaoHiemYTe.BenhVienTiepNhan && o.MaDKBD != null && o.MaDKBD.StartsWith("01"))
                .ToList();
            var dsTiepNhanNgoaiTinhs = returnData
                .Where(o => o.MaDKBD != cauHinhBaoHiemYTe.BenhVienTiepNhan && (o.MaDKBD == null || !o.MaDKBD.StartsWith("01")))
                .ToList();

            var dsChiPhiKCBNoiTrus = new List<DanhSachDeNghiThanhToanChiPhiKCBNoiTru>();

            var dsTiepNhanKhamChuaBenhBanDauDungTuyens = dsTiepNhanKhamChuaBenhBanDaus.Where(o => o.MaLyDoVaoVien == (int)EnumLyDoVaoVien.DungTuyen).ToList();
            dsChiPhiKCBNoiTrus.Add(TinhTongChiPhiKCBNoiTru(dsTiepNhanKhamChuaBenhBanDauDungTuyens, NoiDangKyKCBBanDau.BenhNhanNoiTinhKhamChuaBenhBanDau, EnumLyDoVaoVien.DungTuyen));

            var dsTiepNhanKhamChuaBenhBanDauTraiTuyens = dsTiepNhanKhamChuaBenhBanDaus.Where(o => o.MaLyDoVaoVien == (int)EnumLyDoVaoVien.TraiTuyen).ToList();
            if(dsTiepNhanKhamChuaBenhBanDauTraiTuyens.Any())
                dsChiPhiKCBNoiTrus.Add(TinhTongChiPhiKCBNoiTru(dsTiepNhanKhamChuaBenhBanDauTraiTuyens, NoiDangKyKCBBanDau.BenhNhanNoiTinhKhamChuaBenhBanDau, EnumLyDoVaoVien.TraiTuyen));

            var dsTiepNhanKhamChuaBenhBanDauThongTuyens = dsTiepNhanKhamChuaBenhBanDaus.Where(o => o.MaLyDoVaoVien == (int)EnumLyDoVaoVien.ThongTuyen).ToList();
            if (dsTiepNhanKhamChuaBenhBanDauThongTuyens.Any())
                dsChiPhiKCBNoiTrus.Add(TinhTongChiPhiKCBNoiTru(dsTiepNhanKhamChuaBenhBanDauThongTuyens, NoiDangKyKCBBanDau.BenhNhanNoiTinhKhamChuaBenhBanDau, EnumLyDoVaoVien.ThongTuyen));

            var dsTiepNhanNoiTinhDungTuyens = dsTiepNhanNoiTinhs.Where(o => o.MaLyDoVaoVien == (int)EnumLyDoVaoVien.DungTuyen).ToList();
            dsChiPhiKCBNoiTrus.Add(TinhTongChiPhiKCBNoiTru(dsTiepNhanNoiTinhDungTuyens, NoiDangKyKCBBanDau.BenhNhanNoiTinhDen, EnumLyDoVaoVien.DungTuyen));

            var dsTiepNhanNoiTinhTraiTuyens = dsTiepNhanNoiTinhs.Where(o => o.MaLyDoVaoVien == (int)EnumLyDoVaoVien.TraiTuyen).ToList();
            if (dsTiepNhanNoiTinhTraiTuyens.Any())
                dsChiPhiKCBNoiTrus.Add(TinhTongChiPhiKCBNoiTru(dsTiepNhanNoiTinhTraiTuyens, NoiDangKyKCBBanDau.BenhNhanNoiTinhDen, EnumLyDoVaoVien.TraiTuyen));

            var dsTiepNhanNoiTinhThongTuyens = dsTiepNhanNoiTinhs.Where(o => o.MaLyDoVaoVien == (int)EnumLyDoVaoVien.ThongTuyen).ToList();
            if (dsTiepNhanNoiTinhThongTuyens.Any())
                dsChiPhiKCBNoiTrus.Add(TinhTongChiPhiKCBNoiTru(dsTiepNhanNoiTinhThongTuyens, NoiDangKyKCBBanDau.BenhNhanNoiTinhDen, EnumLyDoVaoVien.ThongTuyen));

            var dsTiepNhanNgoaiTinhDungTuyens = dsTiepNhanNgoaiTinhs.Where(o => o.MaLyDoVaoVien == (int)EnumLyDoVaoVien.DungTuyen).ToList();
            dsChiPhiKCBNoiTrus.Add(TinhTongChiPhiKCBNoiTru(dsTiepNhanNgoaiTinhDungTuyens, NoiDangKyKCBBanDau.BenhNhanNgoaiTinhDen, EnumLyDoVaoVien.DungTuyen));

            var dsTiepNhanNgoaiTinhTraiTuyens = dsTiepNhanNgoaiTinhs.Where(o => o.MaLyDoVaoVien == (int)EnumLyDoVaoVien.TraiTuyen).ToList();
            if (dsTiepNhanNgoaiTinhTraiTuyens.Any())
                dsChiPhiKCBNoiTrus.Add(TinhTongChiPhiKCBNoiTru(dsTiepNhanNgoaiTinhTraiTuyens, NoiDangKyKCBBanDau.BenhNhanNgoaiTinhDen, EnumLyDoVaoVien.TraiTuyen));

            var dsTiepNhanNgoaiTinhThongTuyens = dsTiepNhanNgoaiTinhs.Where(o => o.MaLyDoVaoVien == (int)EnumLyDoVaoVien.ThongTuyen).ToList();
            if (dsTiepNhanNgoaiTinhThongTuyens.Any())
                dsChiPhiKCBNoiTrus.Add(TinhTongChiPhiKCBNoiTru(dsTiepNhanNgoaiTinhThongTuyens, NoiDangKyKCBBanDau.BenhNhanNgoaiTinhDen, EnumLyDoVaoVien.ThongTuyen));

            return new GridDataSource
            {
                Data = dsChiPhiKCBNoiTrus.ToArray(),
                TotalRowCount = dsChiPhiKCBNoiTrus.Count()
            };
        }
        private DanhSachDeNghiThanhToanChiPhiKCBNgoaiTru TinhTongChiPhiKhamBenh(List<ExcelFile7980A> dsTiepNhans, NoiDangKyKCBBanDau noiDangKyKCBBanDau, EnumLyDoVaoVien lyDoVaoVien)
        {
            return new DanhSachDeNghiThanhToanChiPhiKCBNgoaiTru
            {
                NoiDangKyKCBBanDau = noiDangKyKCBBanDau,
                KhamChuaBenh = lyDoVaoVien,
                SoLanDenKham = dsTiepNhans.Count,
                ChiPhiXetNghiemKhongApDungTLTT = dsTiepNhans.Sum(o => o.TienXetNghiem),
                ChiPhiCDHATDCNKhongApDungTLTT = dsTiepNhans.Sum(o => o.TienCDHA),
                ChiPhiThuocKhongApDungTLTT = dsTiepNhans.Sum(o => o.TienThuoc),
                ChiPhiMauKhongApDungTLTT = dsTiepNhans.Sum(o => o.TienMau),
                ChiPhiTTPTKhongApDungTLTT = dsTiepNhans.Sum(o => o.TienPTTT),
                ChiPhiVTYTKhongApDungTLTT = dsTiepNhans.Sum(o => o.TienVTYT),

                ChiPhiDVKTThanhToanTiLe = dsTiepNhans.Sum(o => o.TienDVKTTyLe),
                ChiPhiThuocThanhToanTiLe = dsTiepNhans.Sum(o => o.TienThuocTyLe),
                ChiPhiVTYTThanhToanTiLe = dsTiepNhans.Sum(o => o.TienVTYTTyLe),

                TienKham = dsTiepNhans.Sum(o => o.TienKham),
                VanChuyen = dsTiepNhans.Sum(o => o.TienVanChuyen),
                TongCongChiPhiKCB = dsTiepNhans.Sum(o => o.TienTongChi),
                TongCongChiPhiBHXHThanhToan = dsTiepNhans.Sum(o => o.TienBHTuTra),
                ChiPhiNgoaiQuyDinhSuat = dsTiepNhans.Sum(o => o.TienNgoaiDanhSach)
            };
        }
        private DanhSachDeNghiThanhToanChiPhiKCBNoiTru TinhTongChiPhiKCBNoiTru(List<ExcelFile7980A> dsTiepNhans, NoiDangKyKCBBanDau noiDangKyKCBBanDau, EnumLyDoVaoVien lyDoVaoVien)
        {
            return new DanhSachDeNghiThanhToanChiPhiKCBNoiTru
            {
                NoiDangKyKCBBanDau = noiDangKyKCBBanDau,
                KhamChuaBenh = lyDoVaoVien,
                SoLanDenKham = dsTiepNhans.Count,
                TongSoNgayDieuTri = dsTiepNhans.Sum(o => o.SoNgayDieuTri),
                ChiPhiXetNghiemKhongApDungTLTT = dsTiepNhans.Sum(o => o.TienXetNghiem),
                ChiPhiCDHATDCNKhongApDungTLTT = dsTiepNhans.Sum(o => o.TienCDHA),
                ChiPhiThuocKhongApDungTLTT = dsTiepNhans.Sum(o => o.TienThuoc),
                ChiPhiMauKhongApDungTLTT = dsTiepNhans.Sum(o => o.TienMau),
                ChiPhiTTPTKhongApDungTLTT = dsTiepNhans.Sum(o => o.TienPTTT),
                ChiPhiVTYTKhongApDungTLTT = dsTiepNhans.Sum(o => o.TienVTYT),

                ChiPhiDVKTThanhToanTiLe = dsTiepNhans.Sum(o => o.TienDVKTTyLe),
                ChiPhiThuocThanhToanTiLe = dsTiepNhans.Sum(o => o.TienThuocTyLe),
                ChiPhiVTYTThanhToanTiLe = dsTiepNhans.Sum(o => o.TienVTYTTyLe),

                TienKham = dsTiepNhans.Sum(o => o.TienKham),
                VanChuyen = dsTiepNhans.Sum(o => o.TienVanChuyen),
                TienGiuong = dsTiepNhans.Sum(o => o.TienGiuong),
                TongCongChiPhiKCB = dsTiepNhans.Sum(o => o.TienTongChi),
                TongCongChiPhiBHXHThanhToan = dsTiepNhans.Sum(o => o.TienBHTuTra),
                ChiPhiNgoaiQuyDinhSuat = dsTiepNhans.Sum(o => o.TienNgoaiDanhSach)
            };
        }

        public virtual byte[] ExportDanhSachDeNghiThanhToanChiPhiKCBNgoaiTru(GridDataSource gridDataSource, DanhSachDeNghiThanhToanChiPhiKCBQueryInfoVo query)
        {
            var datas = (ICollection<DanhSachDeNghiThanhToanChiPhiKCBNgoaiTru>)gridDataSource.Data;

            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<DanhSachDeNghiThanhToanChiPhiKCBNgoaiTru>("STT", p => ind++)
            };

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("[Đẩy cổng BHYT] Báo cáo 79a");
                    worksheet.DefaultRowHeight = 16;
                    worksheet.Column(1).Width = 10;
                    worksheet.Column(2).Width = 20;
                    worksheet.Column(3).Width = 35;
                    worksheet.Column(4).Width = 20;
                    worksheet.Column(5).Width = 20;
                    worksheet.Column(6).Width = 20;
                    worksheet.Column(7).Width = 30;
                    worksheet.Column(8).Width = 20;
                    worksheet.Column(9).Width = 20;
                    worksheet.Column(10).Width = 35;
                    worksheet.Column(11).Width = 20;
                    worksheet.Column(12).Width = 20;
                    worksheet.Column(13).Width = 30;
                    worksheet.Column(14).Width = 40;
                    worksheet.Column(15).Width = 25;
                    worksheet.Column(16).Width = 20;
                    worksheet.Column(17).Width = 25;
                    worksheet.Column(18).Width = 35;

                    worksheet.DefaultColWidth = 7;
                    worksheet.Row(8).Height = 24;

                    using (var range = worksheet.Cells["A1:M1"])
                    {
                        range.Worksheet.Cells["A1:M1"].Merge = true;
                        range.Worksheet.Cells["A1:M1"].Value = "Tên CSKCB: Bệnh viện đa khoa quốc tế Bắc Hà";
                        range.Worksheet.Cells["A1:M1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A1:M1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1:M1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:M1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:M1"].Style.Font.Bold = true;

                    }

                    using (var range = worksheet.Cells["A2:M2"])
                    {
                        range.Worksheet.Cells["A2:M2"].Merge = true;
                        range.Worksheet.Cells["A2:M2"].Value = $"Mã số: 01249";
                        range.Worksheet.Cells["A2:M2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A2:M2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A2:M2"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A2:M2"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A2:M2"].Style.Font.Bold = true;

                    }

                    using (var range = worksheet.Cells["O2:R2"])
                    {
                        range.Worksheet.Cells["O2:R2"].Merge = true;
                        range.Worksheet.Cells["O2:R2"].Value = $"Mẫu số: 79a-HD";
                        range.Worksheet.Cells["O2:R2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["O2:R2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["O2:R2"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["O2:R2"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["O2:R2"].Style.Font.Bold = true;

                    }

                    using (var range = worksheet.Cells["O3:R3"])
                    {
                        range.Worksheet.Cells["O3:R3"].Merge = true;
                        range.Worksheet.Cells["O3:R3"].Value = $"(Ban hành theo Thông tư số 178/2012/TT-BTC " +
                                                               $"ngày 23/10/2012 của Bộ Tài Chính)";
                        range.Worksheet.Cells["O3:R3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["O3:R3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["O3:R3"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["O3:R3"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["A5:R5"])
                    {
                        range.Worksheet.Cells["A5:R5"].Merge = true;
                        range.Worksheet.Cells["A5:R5"].Value = "DANH SÁCH ĐỀ NGHỊ THANH TOÁN CHI PHÍ KCB NGOẠI TRÚ";
                        range.Worksheet.Cells["A5:R5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A5:R5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A5:R5"].Style.Font.SetFromFont(new Font("Times New Roman", 16));
                        range.Worksheet.Cells["A5:R5"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A5:R5"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A6:R6"])
                    {
                        range.Worksheet.Cells["A6:R6"].Merge = true;
                        range.Worksheet.Cells["A6:R6"].Value = "Từ ngày: " + query.TuNgay.FormatNgayGioTimKiemTrenBaoCao()
                                                            + " - đến ngày: " + query.DenNgay.FormatNgayGioTimKiemTrenBaoCao();

                        range.Worksheet.Cells["A6:R6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A6:R6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A6:R6"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A6:R6"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A6:R6"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["Q7:R7"])
                    {
                        range.Worksheet.Cells["Q7:R7"].Merge = true;
                        range.Worksheet.Cells["Q7:R7"].Value = "Đơn vị: đồng";
                        range.Worksheet.Cells["Q7:R7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        range.Worksheet.Cells["Q7:R7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["Q7:R7"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["Q7:R7"].Style.Font.Color.SetColor(Color.Black);

                    }

                    var indexHeader = 9;
                    var lastHeader = 11;

                    using (var range = worksheet.Cells["A" + indexHeader + ":R" + lastHeader])
                    {
                        range.Worksheet.Cells["A" + indexHeader + ":R" + lastHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A" + indexHeader + ":R" + lastHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A" + indexHeader + ":R" + lastHeader].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + indexHeader + ":R" + lastHeader].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A" + indexHeader + ":R" + lastHeader].Style.Font.Bold = true;
                        range.Worksheet.Cells["A" + indexHeader + ":R" + lastHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A" + indexHeader + ":A" + lastHeader].Merge = true;
                        range.Worksheet.Cells["A" + indexHeader + ":A" + lastHeader].Value = "STT";
                        range.Worksheet.Cells["A" + indexHeader + ":A" + lastHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A" + indexHeader + ":A" + lastHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["B" + indexHeader + ":B" + lastHeader].Merge = true;
                        range.Worksheet.Cells["B" + indexHeader + ":B" + lastHeader].Value = "Khám chữa bệnh";
                        range.Worksheet.Cells["B" + indexHeader + ":B" + lastHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["B" + indexHeader + ":B" + lastHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["C" + indexHeader + ":C" + lastHeader].Merge = true;
                        range.Worksheet.Cells["C" + indexHeader + ":C" + lastHeader].Value = "Số lượt(Số lần đến khám)";
                        range.Worksheet.Cells["C" + indexHeader + ":C" + lastHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["C" + indexHeader + ":C" + lastHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["D" + indexHeader + ":O" + indexHeader].Merge = true;
                        range.Worksheet.Cells["D" + indexHeader + ":O" + indexHeader].Value = "TỔNG CHI PHÍ KHÁM CHỮA BỆNH";
                        range.Worksheet.Cells["D" + indexHeader + ":O" + indexHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["D" + indexHeader + ":O" + indexHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["D" + (indexHeader + 1) + ":D" + (indexHeader + 2)].Merge = true;
                        range.Worksheet.Cells["D" + (indexHeader + 1) + ":D" + (indexHeader + 2)].Value = "Tổng cộng";
                        range.Worksheet.Cells["D" + (indexHeader + 1) + ":D" + (indexHeader + 2)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["D" + (indexHeader + 1) + ":D" + (indexHeader + 2)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        #region Không áp dụng tỷ lệ thanh toán

                        range.Worksheet.Cells["E" + (indexHeader + 1) + ":J" + (indexHeader + 1)].Merge = true;
                        range.Worksheet.Cells["E" + (indexHeader + 1) + ":J" + (indexHeader + 1)].Value = "Không áp dụng tỷ lệ thanh toán";
                        range.Worksheet.Cells["E" + (indexHeader + 1) + ":J" + (indexHeader + 1)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["E" + (indexHeader + 1) + ":J" + (indexHeader + 1)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["E" + (indexHeader + 2) + ":E" + (indexHeader + 2)].Merge = true;
                        range.Worksheet.Cells["E" + (indexHeader + 2) + ":E" + (indexHeader + 2)].Value = "Xét nghiệm";
                        range.Worksheet.Cells["E" + (indexHeader + 2) + ":E" + (indexHeader + 2)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["E" + (indexHeader + 2) + ":E" + (indexHeader + 2)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["F" + (indexHeader + 2) + ":F" + (indexHeader + 2)].Merge = true;
                        range.Worksheet.Cells["F" + (indexHeader + 2) + ":F" + (indexHeader + 2)].Value = "CĐHA TDCN";
                        range.Worksheet.Cells["F" + (indexHeader + 2) + ":F" + (indexHeader + 2)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["F" + (indexHeader + 2) + ":F" + (indexHeader + 2)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["G" + (indexHeader + 2) + ":G" + (indexHeader + 2)].Merge = true;
                        range.Worksheet.Cells["G" + (indexHeader + 2) + ":G" + (indexHeader + 2)].Value = "Thuốc";
                        range.Worksheet.Cells["G" + (indexHeader + 2) + ":G" + (indexHeader + 2)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G" + (indexHeader + 2) + ":G" + (indexHeader + 2)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["H" + (indexHeader + 2) + ":H" + (indexHeader + 2)].Merge = true;
                        range.Worksheet.Cells["H" + (indexHeader + 2) + ":H" + (indexHeader + 2)].Value = "Máu";
                        range.Worksheet.Cells["H" + (indexHeader + 2) + ":H" + (indexHeader + 2)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["H" + (indexHeader + 2) + ":H" + (indexHeader + 2)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["I" + (indexHeader + 2) + ":I" + (indexHeader + 2)].Merge = true;
                        range.Worksheet.Cells["I" + (indexHeader + 2) + ":I" + (indexHeader + 2)].Value = "TTPT";
                        range.Worksheet.Cells["I" + (indexHeader + 2) + ":I" + (indexHeader + 2)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["I" + (indexHeader + 2) + ":I" + (indexHeader + 2)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["J" + (indexHeader + 2) + ":J" + (indexHeader + 2)].Merge = true;
                        range.Worksheet.Cells["J" + (indexHeader + 2) + ":J" + (indexHeader + 2)].Value = "VTYT";
                        range.Worksheet.Cells["J" + (indexHeader + 2) + ":J" + (indexHeader + 2)].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["J" + (indexHeader + 2) + ":J" + (indexHeader + 2)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        #endregion

                        #region Thanh toán theo tỷ lệ

                        range.Worksheet.Cells["K" + (indexHeader + 1) + ":M" + (indexHeader + 1)].Merge = true;
                        range.Worksheet.Cells["K" + (indexHeader + 1) + ":M" + (indexHeader + 1)].Value = "Thanh toán theo tỷ lệ";
                        range.Worksheet.Cells["K" + (indexHeader + 1) + ":M" + (indexHeader + 1)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["K" + (indexHeader + 1) + ":M" + (indexHeader + 1)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["K" + (indexHeader + 2) + ":K" + (indexHeader + 2)].Merge = true;
                        range.Worksheet.Cells["K" + (indexHeader + 2) + ":K" + (indexHeader + 2)].Value = "DVKT";
                        range.Worksheet.Cells["K" + (indexHeader + 2) + ":K" + (indexHeader + 2)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["K" + (indexHeader + 2) + ":K" + (indexHeader + 2)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["L" + (indexHeader + 2) + ":L" + (indexHeader + 2)].Merge = true;
                        range.Worksheet.Cells["L" + (indexHeader + 2) + ":L" + (indexHeader + 2)].Value = "Thuốc";
                        range.Worksheet.Cells["L" + (indexHeader + 2) + ":L" + (indexHeader + 2)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["L" + (indexHeader + 2) + ":L" + (indexHeader + 2)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["M" + (indexHeader + 2) + ":M" + (indexHeader + 2)].Merge = true;
                        range.Worksheet.Cells["M" + (indexHeader + 2) + ":M" + (indexHeader + 2)].Value = "VTYT";
                        range.Worksheet.Cells["M" + (indexHeader + 2) + ":M" + (indexHeader + 2)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["M" + (indexHeader + 2) + ":M" + (indexHeader + 2)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        #endregion

                        range.Worksheet.Cells["N" + (indexHeader + 1) + ":N" + (indexHeader + 2)].Merge = true;
                        range.Worksheet.Cells["N" + (indexHeader + 1) + ":N" + (indexHeader + 2)].Value = "Tiền khám";
                        range.Worksheet.Cells["N" + (indexHeader + 1) + ":N" + (indexHeader + 2)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["N" + (indexHeader + 1) + ":N" + (indexHeader + 2)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["O" + (indexHeader + 1) + ":O" + (indexHeader + 2)].Merge = true;
                        range.Worksheet.Cells["O" + (indexHeader + 1) + ":O" + (indexHeader + 2)].Value = "Vận chuyển";
                        range.Worksheet.Cells["O" + (indexHeader + 1) + ":O" + (indexHeader + 2)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["O" + (indexHeader + 1) + ":O" + (indexHeader + 2)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["P" + (indexHeader) + ":P" + (indexHeader + 2)].Merge = true;
                        range.Worksheet.Cells["P" + (indexHeader) + ":P" + (indexHeader + 2)].Value = "Người bệnh chi trả";
                        range.Worksheet.Cells["P" + (indexHeader) + ":P" + (indexHeader + 2)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["P" + (indexHeader) + ":P" + (indexHeader + 2)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        #region Chi phí đề nghị BHXH thanh toán

                        range.Worksheet.Cells["Q" + (indexHeader) + ":R" + (indexHeader)].Merge = true;
                        range.Worksheet.Cells["Q" + (indexHeader) + ":R" + (indexHeader)].Value = "Chi phí đề nghị BHXH thanh toán";
                        range.Worksheet.Cells["Q" + (indexHeader) + ":R" + (indexHeader)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["Q" + (indexHeader) + ":R" + (indexHeader)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["Q" + (indexHeader + 1) + ":Q" + (indexHeader + 2)].Merge = true;
                        range.Worksheet.Cells["Q" + (indexHeader + 1) + ":Q" + (indexHeader + 2)].Value = "Tổng cộng";
                        range.Worksheet.Cells["Q" + (indexHeader + 1) + ":Q" + (indexHeader + 2)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["Q" + (indexHeader + 1) + ":Q" + (indexHeader + 2)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["R" + (indexHeader + 1) + ":R" + (indexHeader + 2)].Merge = true;
                        range.Worksheet.Cells["R" + (indexHeader + 1) + ":R" + (indexHeader + 2)].Value = "Trong đó chi phí ngoài quỹ định suất";
                        range.Worksheet.Cells["R" + (indexHeader + 1) + ":R" + (indexHeader + 2)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["R" + (indexHeader + 1) + ":R" + (indexHeader + 2)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        #endregion

                    }

                    //số thứ tự (A - 15)
                    var sttHeader = lastHeader + 1;
                    using (var range = worksheet.Cells["A" + sttHeader + ":R" + sttHeader])
                    {
                        range.Worksheet.Cells["A" + sttHeader + ":R" + sttHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A" + sttHeader + ":R" + sttHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A" + sttHeader + ":R" + sttHeader].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + sttHeader + ":R" + sttHeader].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A" + sttHeader + ":R" + sttHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A" + sttHeader + ":A" + sttHeader].Merge = true;
                        range.Worksheet.Cells["A" + sttHeader + ":A" + sttHeader].Value = "A";
                        range.Worksheet.Cells["A" + sttHeader + ":A" + sttHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A" + sttHeader + ":A" + sttHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["B" + sttHeader + ":B" + sttHeader].Merge = true;
                        range.Worksheet.Cells["B" + sttHeader + ":B" + sttHeader].Value = "B";
                        range.Worksheet.Cells["B" + sttHeader + ":B" + sttHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["B" + sttHeader + ":B" + sttHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["C" + sttHeader + ":C" + sttHeader].Merge = true;
                        range.Worksheet.Cells["C" + sttHeader + ":C" + sttHeader].Value = "C";
                        range.Worksheet.Cells["C" + sttHeader + ":C" + sttHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["C" + sttHeader + ":C" + sttHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["D" + sttHeader + ":D" + sttHeader].Merge = true;
                        range.Worksheet.Cells["D" + sttHeader + ":D" + sttHeader].Value = "(1)";
                        range.Worksheet.Cells["D" + sttHeader + ":D" + sttHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["D" + sttHeader + ":D" + sttHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["E" + sttHeader + ":E" + sttHeader].Merge = true;
                        range.Worksheet.Cells["E" + sttHeader + ":E" + sttHeader].Value = "(2)";
                        range.Worksheet.Cells["E" + sttHeader + ":E" + sttHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["E" + sttHeader + ":E" + sttHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["F" + sttHeader + ":F" + sttHeader].Merge = true;
                        range.Worksheet.Cells["F" + sttHeader + ":F" + sttHeader].Value = "(3)";
                        range.Worksheet.Cells["F" + sttHeader + ":F" + sttHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["F" + sttHeader + ":F" + sttHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["G" + sttHeader + ":G" + sttHeader].Merge = true;
                        range.Worksheet.Cells["G" + sttHeader + ":G" + sttHeader].Value = "(4)";
                        range.Worksheet.Cells["G" + sttHeader + ":G" + sttHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G" + sttHeader + ":G" + sttHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["H" + sttHeader + ":H" + sttHeader].Merge = true;
                        range.Worksheet.Cells["H" + sttHeader + ":H" + sttHeader].Value = "(5)";
                        range.Worksheet.Cells["H" + sttHeader + ":H" + sttHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["H" + sttHeader + ":H" + sttHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["I" + sttHeader + ":I" + sttHeader].Merge = true;
                        range.Worksheet.Cells["I" + sttHeader + ":I" + sttHeader].Value = "(6)";
                        range.Worksheet.Cells["I" + sttHeader + ":I" + sttHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["I" + sttHeader + ":I" + sttHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["J" + sttHeader + ":J" + sttHeader].Merge = true;
                        range.Worksheet.Cells["J" + sttHeader + ":J" + sttHeader].Value = "(7)";
                        range.Worksheet.Cells["J" + sttHeader + ":J" + sttHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["J" + sttHeader + ":J" + sttHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["K" + sttHeader + ":K" + sttHeader].Merge = true;
                        range.Worksheet.Cells["K" + sttHeader + ":K" + sttHeader].Value = "(8)";
                        range.Worksheet.Cells["K" + sttHeader + ":K" + sttHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["K" + sttHeader + ":K" + sttHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["L" + sttHeader + ":L" + sttHeader].Merge = true;
                        range.Worksheet.Cells["L" + sttHeader + ":L" + sttHeader].Value = "(9)";
                        range.Worksheet.Cells["L" + sttHeader + ":L" + sttHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["L" + sttHeader + ":L" + sttHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["M" + sttHeader + ":M" + sttHeader].Merge = true;
                        range.Worksheet.Cells["M" + sttHeader + ":M" + sttHeader].Value = "(10)";
                        range.Worksheet.Cells["M" + sttHeader + ":M" + sttHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["M" + sttHeader + ":M" + sttHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["N" + sttHeader + ":N" + sttHeader].Merge = true;
                        range.Worksheet.Cells["N" + sttHeader + ":N" + sttHeader].Value = "(11)";
                        range.Worksheet.Cells["N" + sttHeader + ":N" + sttHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["N" + sttHeader + ":N" + sttHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["O" + sttHeader + ":O" + sttHeader].Merge = true;
                        range.Worksheet.Cells["O" + sttHeader + ":O" + sttHeader].Value = "(12)";
                        range.Worksheet.Cells["O" + sttHeader + ":O" + sttHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["O" + sttHeader + ":O" + sttHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["P" + sttHeader + ":P" + sttHeader].Merge = true;
                        range.Worksheet.Cells["P" + sttHeader + ":P" + sttHeader].Value = "(13)";
                        range.Worksheet.Cells["P" + sttHeader + ":P" + sttHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["P" + sttHeader + ":P" + sttHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["Q" + sttHeader + ":Q" + sttHeader].Merge = true;
                        range.Worksheet.Cells["Q" + sttHeader + ":Q" + sttHeader].Value = "(14)";
                        range.Worksheet.Cells["Q" + sttHeader + ":Q" + sttHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["Q" + sttHeader + ":Q" + sttHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["R" + sttHeader + ":R" + sttHeader].Merge = true;
                        range.Worksheet.Cells["R" + sttHeader + ":R" + sttHeader].Value = "(15)";
                        range.Worksheet.Cells["R" + sttHeader + ":R" + sttHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["R" + sttHeader + ":R" + sttHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    }


                    var manager = new PropertyManager<DanhSachDeNghiThanhToanChiPhiKCBNgoaiTru>(requestProperties);

                    int index = sttHeader + 1;

                    var stt = 1;

                    var lstNoiDangKyKCBBanDau = datas.GroupBy(x => new { x.NoiDangKyKCBBanDau }).Select(item => new
                    {
                        item.First().NoiDangKyKCBBanDauDisplay

                    }).ToList();

                    if (lstNoiDangKyKCBBanDau.Any())
                    {
                        foreach (var item in lstNoiDangKyKCBBanDau)
                        {
                            var dataTheoGroups = datas.Where(o => o.NoiDangKyKCBBanDauDisplay == item.NoiDangKyKCBBanDauDisplay).ToList();

                            if (dataTheoGroups.Any())
                            {
                                using (var range = worksheet.Cells["A" + index + ":R" + index])
                                {
                                    range.Worksheet.Cells["A" + index + ":R" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                    range.Worksheet.Cells["A" + index + ":R" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                    range.Worksheet.Cells["A" + index + ":R" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["A" + index + ":R" + index].Style.Font.Color.SetColor(Color.Black);
                                    range.Worksheet.Cells["A" + index + ":R" + index].Value = item.NoiDangKyKCBBanDauDisplay;
                                    range.Worksheet.Cells["A" + index + ":R" + index].Merge = true;
                                }

                                index++;
                                foreach (var dataItem in dataTheoGroups)
                                {
                                    if (dataTheoGroups.Any())
                                    {
                                        using (var range = worksheet.Cells["B" + index + ":R" + index])
                                        {
                                            range.Worksheet.Cells["B" + index + ":R" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                            range.Worksheet.Cells["B" + index + ":R" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells["B" + index + ":R" + index].Style.Font.Color.SetColor(Color.Black);


                                            worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                            worksheet.Cells["A" + index].Value = stt;

                                            worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["B" + index].Value = dataItem.KhamChuaBenhDisplay;

                                            worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            worksheet.Cells["C" + index].Value = dataItem.SoLanDenKham;

                                            worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            worksheet.Cells["D" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["D" + index].Value = dataItem.TongCongChiPhiKCB;

                                            worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            worksheet.Cells["E" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["E" + index].Value = dataItem.ChiPhiXetNghiemKhongApDungTLTT;

                                            worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            worksheet.Cells["F" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["F" + index].Value = dataItem.ChiPhiCDHATDCNKhongApDungTLTT;

                                            worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            worksheet.Cells["G" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["G" + index].Value = dataItem.ChiPhiThuocKhongApDungTLTT;

                                            worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["H" + index].Value = dataItem.ChiPhiMauKhongApDungTLTT;

                                            worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            worksheet.Cells["I" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["I" + index].Value = dataItem.ChiPhiTTPTKhongApDungTLTT;

                                            worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["J" + index].Value = dataItem.ChiPhiVTYTKhongApDungTLTT;

                                            worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["K" + index].Value = dataItem.ChiPhiDVKTThanhToanTiLe;

                                            worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["L" + index].Value = dataItem.ChiPhiThuocThanhToanTiLe;

                                            worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["M" + index].Value = dataItem.ChiPhiVTYTThanhToanTiLe;

                                            worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["N" + index].Value = dataItem.TienKham;

                                            worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["O" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            worksheet.Cells["O" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["O" + index].Value = dataItem.VanChuyen;

                                            worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["P" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            worksheet.Cells["P" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["P" + index].Value = dataItem.NguoiBenhChiTra;

                                            worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            worksheet.Cells["Q" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["Q" + index].Value = dataItem.TongCongChiPhiBHXHThanhToan;

                                            worksheet.Cells["R" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["R" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            worksheet.Cells["R" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["R" + index].Value = dataItem.ChiPhiNgoaiQuyDinhSuat;
                                        }

                                        index++;
                                    }
                                }

                                using (var range = worksheet.Cells["B" + index + ":R" + index])
                                {
                                    range.Worksheet.Cells["B" + index + ":R" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                    range.Worksheet.Cells["B" + index + ":R" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                    range.Worksheet.Cells["B" + index + ":R" + index].Style.Font.Color.SetColor(Color.Black);


                                    worksheet.Cells["A" + index + ":B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["A" + index + ":B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["A" + index + ":B" + index].Merge = true;
                                    worksheet.Cells["A" + index + ":B" + index].Value = "Cộng:";



                                    worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["C" + index].Value = dataTheoGroups.Sum(s => s.SoLanDenKham);

                                    worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["D" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["D" + index].Value = dataTheoGroups.Sum(s => s.TongCongChiPhiKCB);

                                    worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["E" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["E" + index].Value = dataTheoGroups.Sum(s => s.ChiPhiXetNghiemKhongApDungTLTT);

                                    worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["F" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["F" + index].Value = dataTheoGroups.Sum(s => s.ChiPhiCDHATDCNKhongApDungTLTT);

                                    worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["G" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["G" + index].Value = dataTheoGroups.Sum(s => s.ChiPhiThuocKhongApDungTLTT);

                                    worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["H" + index].Value = dataTheoGroups.Sum(s => s.ChiPhiMauKhongApDungTLTT);

                                    worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["I" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["I" + index].Value = dataTheoGroups.Sum(s => s.ChiPhiTTPTKhongApDungTLTT);

                                    worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["J" + index].Value = dataTheoGroups.Sum(s => s.ChiPhiVTYTKhongApDungTLTT);

                                    worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["K" + index].Value = dataTheoGroups.Sum(s => s.ChiPhiDVKTThanhToanTiLe);

                                    worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["L" + index].Value = dataTheoGroups.Sum(s => s.ChiPhiThuocThanhToanTiLe);

                                    worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["M" + index].Value = dataTheoGroups.Sum(s => s.ChiPhiVTYTThanhToanTiLe);

                                    worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["N" + index].Value = dataTheoGroups.Sum(s => s.TienKham);

                                    worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["O" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["O" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["O" + index].Value = dataTheoGroups.Sum(s => s.VanChuyen);

                                    worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["P" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["P" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["P" + index].Value = dataTheoGroups.Sum(s => s.NguoiBenhChiTra);

                                    worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["Q" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["Q" + index].Value = dataTheoGroups.Sum(s => s.TongCongChiPhiBHXHThanhToan);

                                    worksheet.Cells["R" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["R" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["R" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["R" + index].Value = dataTheoGroups.Sum(s => s.ChiPhiNgoaiQuyDinhSuat);
                                }
                                index++;
                            }
                        }

                    }

                    using (var range = worksheet.Cells["B" + index + ":R" + index])
                    {
                        range.Worksheet.Cells["B" + index + ":R" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["B" + index + ":R" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["B" + index + ":R" + index].Style.Font.Color.SetColor(Color.Black);

                        worksheet.Cells["A" + index + ":B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["A" + index + ":B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells["A" + index + ":B" + index].Merge = true;
                        worksheet.Cells["A" + index + ":B" + index].Value = "Tổng cộng A + B + C:";



                        worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["C" + index].Value = datas.Sum(s => s.SoLanDenKham);

                        worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["D" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["D" + index].Value = datas.Sum(s => s.TongCongChiPhiKCB);

                        worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["E" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["E" + index].Value = datas.Sum(s => s.ChiPhiXetNghiemKhongApDungTLTT);

                        worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["F" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["F" + index].Value = datas.Sum(s => s.ChiPhiCDHATDCNKhongApDungTLTT);

                        worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["G" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["G" + index].Value = datas.Sum(s => s.ChiPhiThuocKhongApDungTLTT);

                        worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["H" + index].Value = datas.Sum(s => s.ChiPhiMauKhongApDungTLTT);

                        worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["I" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["I" + index].Value = datas.Sum(s => s.ChiPhiTTPTKhongApDungTLTT);

                        worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["J" + index].Value = datas.Sum(s => s.ChiPhiVTYTKhongApDungTLTT);

                        worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["K" + index].Value = datas.Sum(s => s.ChiPhiDVKTThanhToanTiLe);

                        worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["L" + index].Value = datas.Sum(s => s.ChiPhiThuocThanhToanTiLe);

                        worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["M" + index].Value = datas.Sum(s => s.ChiPhiVTYTThanhToanTiLe);

                        worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["N" + index].Value = datas.Sum(s => s.TienKham);

                        worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["O" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["O" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["O" + index].Value = datas.Sum(s => s.VanChuyen);

                        worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["P" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["P" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["P" + index].Value = datas.Sum(s => s.NguoiBenhChiTra);

                        worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["Q" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["Q" + index].Value = datas.Sum(s => s.TongCongChiPhiBHXHThanhToan);

                        worksheet.Cells["R" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["R" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["R" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["R" + index].Value = datas.Sum(s => s.ChiPhiNgoaiQuyDinhSuat);
                    }


                    index = index + 2;

                    using (var range = worksheet.Cells["B" + index + ":R" + index])
                    {
                        range.Worksheet.Cells["B" + index + ":R" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["B" + index + ":R" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["B" + index + ":R" + index].Style.Font.Color.SetColor(Color.Black);

                        worksheet.Cells["A" + index + ":R" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells["A" + index + ":R" + index].Merge = true;
                        worksheet.Cells["A" + index + ":R" + index].Value = $"Tổng số tiền đề nghị thanh toán (viết bằng chữ):" +
                            $" {NumberHelper.ChuyenSoRaText(Convert.ToDouble(datas.Sum(o => o.TongCongChiPhiBHXHThanhToan)))}";
                    }

                    index = index + 2;

                    using (var range = worksheet.Cells["N" + index + ":R" + index])
                    {
                        range.Worksheet.Cells["N" + index + ":R" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["N" + index + ":R" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["N" + index + ":R" + index].Style.Font.Color.SetColor(Color.Black);


                        worksheet.Cells["N" + index + ":R" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["N" + index + ":R" + index].Merge = true;
                        worksheet.Cells["N" + index + ":R" + index].Value = $"Ngày ...... tháng ...... năm ............:";
                    }

                    index++;
                    using (var range = worksheet.Cells["A" + index + ":R" + index])
                    {
                        range.Worksheet.Cells["A" + index + ":R" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A" + index + ":R" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + index + ":R" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A" + index + ":R" + index].Style.Font.Bold = true;

                        worksheet.Cells["A" + index + ":D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["A" + index + ":D" + index].Merge = true;
                        worksheet.Cells["A" + index + ":D" + index].Value = $"Người lập biểu";

                        worksheet.Cells["E" + index + ":I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["E" + index + ":I" + index].Merge = true;
                        worksheet.Cells["E" + index + ":I" + index].Value = $"Trưởng phòng KHTH";

                        worksheet.Cells["J" + index + ":M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["J" + index + ":M" + index].Merge = true;
                        worksheet.Cells["J" + index + ":M" + index].Value = $"Kế toán trưởng";

                        worksheet.Cells["N" + index + ":R" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["N" + index + ":R" + index].Merge = true;
                        worksheet.Cells["N" + index + ":R" + index].Value = $"Thủ trưởng đơn vị";
                    }

                    index++;
                    using (var range = worksheet.Cells["A" + index + ":R" + index])
                    {
                        range.Worksheet.Cells["A" + index + ":R" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A" + index + ":R" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + index + ":R" + index].Style.Font.Color.SetColor(Color.Black);


                        worksheet.Cells["A" + index + ":D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["A" + index + ":D" + index].Merge = true;
                        worksheet.Cells["A" + index + ":D" + index].Value = $"(Ký, họ tên)";

                        worksheet.Cells["E" + index + ":I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["E" + index + ":I" + index].Merge = true;
                        worksheet.Cells["E" + index + ":I" + index].Value = $"(Ký, họ tên)";

                        worksheet.Cells["J" + index + ":M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["J" + index + ":M" + index].Merge = true;
                        worksheet.Cells["J" + index + ":M" + index].Value = $"(Ký, họ tên)";

                        worksheet.Cells["N" + index + ":R" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["N" + index + ":R" + index].Merge = true;
                        worksheet.Cells["N" + index + ":R" + index].Value = $"(Ký, họ tên, đóng dấu)";
                    }
                    xlPackage.Save();
                }
                return stream.ToArray();

            }
        }

        public virtual byte[] ExportDanhSachDeNghiThanhToanChiPhiKCBNoiTru(GridDataSource gridDataSource, DanhSachDeNghiThanhToanChiPhiKCBQueryInfoVo query)
        {
            var datas = (ICollection<DanhSachDeNghiThanhToanChiPhiKCBNoiTru>)gridDataSource.Data;

            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<DanhSachDeNghiThanhToanChiPhiKCBNoiTru>("STT", p => ind++)
            };

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("[Đẩy cổng BHYT] Báo cáo 80a");
                    worksheet.DefaultRowHeight = 16;
                    worksheet.Column(1).Width = 10;
                    worksheet.Column(2).Width = 20;
                    worksheet.Column(3).Width = 20;
                    worksheet.Column(4).Width = 40;
                    worksheet.Column(5).Width = 20;
                    worksheet.Column(6).Width = 20;
                    worksheet.Column(7).Width = 30;
                    worksheet.Column(8).Width = 20;
                    worksheet.Column(9).Width = 20;
                    worksheet.Column(10).Width = 35;
                    worksheet.Column(11).Width = 20;
                    worksheet.Column(12).Width = 20;
                    worksheet.Column(13).Width = 30;
                    worksheet.Column(14).Width = 40;
                    worksheet.Column(15).Width = 25;
                    worksheet.Column(16).Width = 20;
                    worksheet.Column(17).Width = 25;
                    worksheet.Column(18).Width = 35;
                    worksheet.Column(19).Width = 25;
                    worksheet.Column(20).Width = 40;

                    worksheet.DefaultColWidth = 7;
                    worksheet.Row(8).Height = 24;

                    using (var range = worksheet.Cells["A1:M1"])
                    {
                        range.Worksheet.Cells["A1:M1"].Merge = true;
                        range.Worksheet.Cells["A1:M1"].Value = "Tên CSKCB: Bệnh viện đa khoa quốc tế Bắc Hà";
                        range.Worksheet.Cells["A1:M1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A1:M1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1:M1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:M1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:M1"].Style.Font.Bold = true;

                    }

                    using (var range = worksheet.Cells["A2:M2"])
                    {
                        range.Worksheet.Cells["A2:M2"].Merge = true;
                        range.Worksheet.Cells["A2:M2"].Value = $"Mã số: 01249";
                        range.Worksheet.Cells["A2:M2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A2:M2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A2:M2"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A2:M2"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A2:M2"].Style.Font.Bold = true;

                    }

                    using (var range = worksheet.Cells["Q2:T2"])
                    {
                        range.Worksheet.Cells["Q2:T2"].Merge = true;
                        range.Worksheet.Cells["Q2:T2"].Value = $"Mẫu số: 80a-HD";
                        range.Worksheet.Cells["Q2:T2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["Q2:T2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["Q2:T2"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["Q2:T2"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["Q2:T2"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["Q3:T3"])
                    {
                        range.Worksheet.Cells["Q3:T3"].Merge = true;
                        range.Worksheet.Cells["Q3:T3"].Value = $"(Ban hành theo Thông tư số 178/2012/TT-BTC " +
                                                               $"ngày 23/10/2012 của Bộ Tài Chính)";
                        range.Worksheet.Cells["Q3:T3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["Q3:T3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["Q3:T3"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["Q3:T3"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["A5:T5"])
                    {
                        range.Worksheet.Cells["A5:T5"].Merge = true;
                        range.Worksheet.Cells["A5:T5"].Value = "DANH SÁCH ĐỀ NGHỊ THANH TOÁN CHI PHÍ KCB NỘI TRÚ";
                        range.Worksheet.Cells["A5:T5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A5:T5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A5:T5"].Style.Font.SetFromFont(new Font("Times New Roman", 16));
                        range.Worksheet.Cells["A5:T5"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A5:T5"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A6:T6"])
                    {
                        range.Worksheet.Cells["A6:T6"].Merge = true;
                        range.Worksheet.Cells["A6:T6"].Value = "Từ ngày: " + query.TuNgay.FormatNgayGioTimKiemTrenBaoCao()
                                                            + " - đến ngày: " + query.DenNgay.FormatNgayGioTimKiemTrenBaoCao();

                        range.Worksheet.Cells["A6:T6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A6:T6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A6:T6"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A6:T6"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A6:T6"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["Q7:T7"])
                    {
                        range.Worksheet.Cells["Q7:T7"].Merge = true;
                        range.Worksheet.Cells["Q7:T7"].Value = "Đơn vị: đồng";
                        range.Worksheet.Cells["Q7:T7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        range.Worksheet.Cells["Q7:T7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["Q7:T7"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["Q7:T7"].Style.Font.Color.SetColor(Color.Black);

                    }

                    var indexHeader = 9;
                    var lastHeader = 11;

                    using (var range = worksheet.Cells["A" + indexHeader + ":T" + lastHeader])
                    {
                        range.Worksheet.Cells["A" + indexHeader + ":T" + lastHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A" + indexHeader + ":T" + lastHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A" + indexHeader + ":T" + lastHeader].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + indexHeader + ":T" + lastHeader].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A" + indexHeader + ":T" + lastHeader].Style.Font.Bold = true;
                        range.Worksheet.Cells["A" + indexHeader + ":T" + lastHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A" + indexHeader + ":A" + lastHeader].Merge = true;
                        range.Worksheet.Cells["A" + indexHeader + ":A" + lastHeader].Value = "STT";
                        range.Worksheet.Cells["A" + indexHeader + ":A" + lastHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A" + indexHeader + ":A" + lastHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["B" + indexHeader + ":B" + lastHeader].Merge = true;
                        range.Worksheet.Cells["B" + indexHeader + ":B" + lastHeader].Value = "Khám chữa bệnh";
                        range.Worksheet.Cells["B" + indexHeader + ":B" + lastHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["B" + indexHeader + ":B" + lastHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["C" + indexHeader + ":C" + lastHeader].Merge = true;
                        range.Worksheet.Cells["C" + indexHeader + ":C" + lastHeader].Value = "Số lượt(Số lần đến khám)";
                        range.Worksheet.Cells["C" + indexHeader + ":C" + lastHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["C" + indexHeader + ":C" + lastHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["D" + indexHeader + ":D" + lastHeader].Merge = true;
                        range.Worksheet.Cells["D" + indexHeader + ":D" + lastHeader].Value = "Số ngày điều trị (Tổng số ngày điều trị)";
                        range.Worksheet.Cells["D" + indexHeader + ":D" + lastHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["D" + indexHeader + ":D" + lastHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["E" + indexHeader + ":Q" + indexHeader].Merge = true;
                        range.Worksheet.Cells["E" + indexHeader + ":Q" + indexHeader].Value = "TỔNG CHI PHÍ KHÁM CHỮA BỆNH";
                        range.Worksheet.Cells["E" + indexHeader + ":Q" + indexHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["E" + indexHeader + ":Q" + indexHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["E" + (indexHeader + 1) + ":E" + (indexHeader + 2)].Merge = true;
                        range.Worksheet.Cells["E" + (indexHeader + 1) + ":E" + (indexHeader + 2)].Value = "Tổng cộng";
                        range.Worksheet.Cells["E" + (indexHeader + 1) + ":E" + (indexHeader + 2)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["E" + (indexHeader + 1) + ":E" + (indexHeader + 2)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        #region Không áp dụng tỷ lệ thanh toán

                        range.Worksheet.Cells["F" + (indexHeader + 1) + ":K" + (indexHeader + 1)].Merge = true;
                        range.Worksheet.Cells["F" + (indexHeader + 1) + ":K" + (indexHeader + 1)].Value = "Không áp dụng tỷ lệ thanh toán";
                        range.Worksheet.Cells["F" + (indexHeader + 1) + ":K" + (indexHeader + 1)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["F" + (indexHeader + 1) + ":K" + (indexHeader + 1)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["F" + (indexHeader + 2) + ":F" + (indexHeader + 2)].Merge = true;
                        range.Worksheet.Cells["F" + (indexHeader + 2) + ":F" + (indexHeader + 2)].Value = "Xét nghiệm";
                        range.Worksheet.Cells["F" + (indexHeader + 2) + ":F" + (indexHeader + 2)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["F" + (indexHeader + 2) + ":F" + (indexHeader + 2)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["G" + (indexHeader + 2) + ":G" + (indexHeader + 2)].Merge = true;
                        range.Worksheet.Cells["G" + (indexHeader + 2) + ":G" + (indexHeader + 2)].Value = "CĐHA TDCN";
                        range.Worksheet.Cells["G" + (indexHeader + 2) + ":G" + (indexHeader + 2)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G" + (indexHeader + 2) + ":G" + (indexHeader + 2)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["H" + (indexHeader + 2) + ":H" + (indexHeader + 2)].Merge = true;
                        range.Worksheet.Cells["H" + (indexHeader + 2) + ":H" + (indexHeader + 2)].Value = "Thuốc";
                        range.Worksheet.Cells["H" + (indexHeader + 2) + ":H" + (indexHeader + 2)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["H" + (indexHeader + 2) + ":H" + (indexHeader + 2)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["I" + (indexHeader + 2) + ":I" + (indexHeader + 2)].Merge = true;
                        range.Worksheet.Cells["I" + (indexHeader + 2) + ":I" + (indexHeader + 2)].Value = "Máu";
                        range.Worksheet.Cells["I" + (indexHeader + 2) + ":I" + (indexHeader + 2)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["I" + (indexHeader + 2) + ":I" + (indexHeader + 2)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["J" + (indexHeader + 2) + ":J" + (indexHeader + 2)].Merge = true;
                        range.Worksheet.Cells["J" + (indexHeader + 2) + ":J" + (indexHeader + 2)].Value = "TTPT";
                        range.Worksheet.Cells["J" + (indexHeader + 2) + ":J" + (indexHeader + 2)].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["J" + (indexHeader + 2) + ":J" + (indexHeader + 2)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["K" + (indexHeader + 2) + ":K" + (indexHeader + 2)].Merge = true;
                        range.Worksheet.Cells["K" + (indexHeader + 2) + ":K" + (indexHeader + 2)].Value = "VTYT";
                        range.Worksheet.Cells["K" + (indexHeader + 2) + ":K" + (indexHeader + 2)].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["K" + (indexHeader + 2) + ":K" + (indexHeader + 2)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        #endregion

                        #region Thanh toán theo tỷ lệ

                        range.Worksheet.Cells["L" + (indexHeader + 1) + ":N" + (indexHeader + 1)].Merge = true;
                        range.Worksheet.Cells["L" + (indexHeader + 1) + ":N" + (indexHeader + 1)].Value = "Thanh toán theo tỷ lệ";
                        range.Worksheet.Cells["L" + (indexHeader + 1) + ":N" + (indexHeader + 1)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["L" + (indexHeader + 1) + ":N" + (indexHeader + 1)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["L" + (indexHeader + 2) + ":L" + (indexHeader + 2)].Merge = true;
                        range.Worksheet.Cells["L" + (indexHeader + 2) + ":L" + (indexHeader + 2)].Value = "DVKT";
                        range.Worksheet.Cells["L" + (indexHeader + 2) + ":L" + (indexHeader + 2)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["L" + (indexHeader + 2) + ":L" + (indexHeader + 2)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["M" + (indexHeader + 2) + ":M" + (indexHeader + 2)].Merge = true;
                        range.Worksheet.Cells["M" + (indexHeader + 2) + ":M" + (indexHeader + 2)].Value = "Thuốc";
                        range.Worksheet.Cells["M" + (indexHeader + 2) + ":M" + (indexHeader + 2)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["M" + (indexHeader + 2) + ":M" + (indexHeader + 2)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["N" + (indexHeader + 2) + ":N" + (indexHeader + 2)].Merge = true;
                        range.Worksheet.Cells["N" + (indexHeader + 2) + ":N" + (indexHeader + 2)].Value = "VTYT";
                        range.Worksheet.Cells["N" + (indexHeader + 2) + ":N" + (indexHeader + 2)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["N" + (indexHeader + 2) + ":N" + (indexHeader + 2)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        #endregion


                        range.Worksheet.Cells["O" + (indexHeader + 1) + ":O" + (indexHeader + 2)].Merge = true;
                        range.Worksheet.Cells["O" + (indexHeader + 1) + ":O" + (indexHeader + 2)].Value = "Tiền khám";
                        range.Worksheet.Cells["O" + (indexHeader + 1) + ":O" + (indexHeader + 2)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["O" + (indexHeader + 1) + ":O" + (indexHeader + 2)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["P" + (indexHeader + 1) + ":P" + (indexHeader + 2)].Merge = true;
                        range.Worksheet.Cells["P" + (indexHeader + 1) + ":P" + (indexHeader + 2)].Value = "Tiền giường";
                        range.Worksheet.Cells["P" + (indexHeader + 1) + ":P" + (indexHeader + 2)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["P" + (indexHeader + 1) + ":P" + (indexHeader + 2)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["Q" + (indexHeader + 1) + ":Q" + (indexHeader + 2)].Merge = true;
                        range.Worksheet.Cells["Q" + (indexHeader + 1) + ":Q" + (indexHeader + 2)].Value = "Vận chuyển";
                        range.Worksheet.Cells["Q" + (indexHeader + 1) + ":Q" + (indexHeader + 2)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["Q" + (indexHeader + 1) + ":Q" + (indexHeader + 2)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        #region Chi phí đề nghị BHXH thanh toán

                        range.Worksheet.Cells["R" + indexHeader + ":R" + lastHeader].Merge = true;
                        range.Worksheet.Cells["R" + indexHeader + ":R" + lastHeader].Value = "Người bệnh chi trả";
                        range.Worksheet.Cells["R" + indexHeader + ":R" + lastHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["R" + indexHeader + ":R" + lastHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["S" + (indexHeader) + ":T" + (indexHeader)].Merge = true;
                        range.Worksheet.Cells["S" + (indexHeader) + ":T" + (indexHeader)].Value = "Chi phí đề nghị BHXH thanh toán";
                        range.Worksheet.Cells["S" + (indexHeader) + ":T" + (indexHeader)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["S" + (indexHeader) + ":T" + (indexHeader)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["S" + (indexHeader + 1) + ":S" + (indexHeader + 2)].Merge = true;
                        range.Worksheet.Cells["S" + (indexHeader + 1) + ":S" + (indexHeader + 2)].Value = "Tổng cộng";
                        range.Worksheet.Cells["S" + (indexHeader + 1) + ":S" + (indexHeader + 2)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["S" + (indexHeader + 1) + ":S" + (indexHeader + 2)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["T" + (indexHeader + 1) + ":T" + (indexHeader + 2)].Merge = true;
                        range.Worksheet.Cells["T" + (indexHeader + 1) + ":T" + (indexHeader + 2)].Value = "Trong đó chi phí ngoài quỹ định suất";
                        range.Worksheet.Cells["T" + (indexHeader + 1) + ":T" + (indexHeader + 2)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["T" + (indexHeader + 1) + ":T" + (indexHeader + 2)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        #endregion

                    }

                    //số thứ tự (A - 15)
                    var sttHeader = lastHeader + 1;
                    using (var range = worksheet.Cells["A" + sttHeader + ":T" + sttHeader])
                    {
                        range.Worksheet.Cells["A" + sttHeader + ":T" + sttHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A" + sttHeader + ":T" + sttHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A" + sttHeader + ":T" + sttHeader].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + sttHeader + ":T" + sttHeader].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A" + sttHeader + ":T" + sttHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A" + sttHeader + ":A" + sttHeader].Merge = true;
                        range.Worksheet.Cells["A" + sttHeader + ":A" + sttHeader].Value = "A";
                        range.Worksheet.Cells["A" + sttHeader + ":A" + sttHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A" + sttHeader + ":A" + sttHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["B" + sttHeader + ":B" + sttHeader].Merge = true;
                        range.Worksheet.Cells["B" + sttHeader + ":B" + sttHeader].Value = "B";
                        range.Worksheet.Cells["B" + sttHeader + ":B" + sttHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["B" + sttHeader + ":B" + sttHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["C" + sttHeader + ":C" + sttHeader].Merge = true;
                        range.Worksheet.Cells["C" + sttHeader + ":C" + sttHeader].Value = "C";
                        range.Worksheet.Cells["C" + sttHeader + ":C" + sttHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["C" + sttHeader + ":C" + sttHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["D" + sttHeader + ":D" + sttHeader].Merge = true;
                        range.Worksheet.Cells["D" + sttHeader + ":D" + sttHeader].Value = "D";
                        range.Worksheet.Cells["D" + sttHeader + ":D" + sttHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["D" + sttHeader + ":D" + sttHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["E" + sttHeader + ":E" + sttHeader].Merge = true;
                        range.Worksheet.Cells["E" + sttHeader + ":E" + sttHeader].Value = "(1)";
                        range.Worksheet.Cells["E" + sttHeader + ":E" + sttHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["E" + sttHeader + ":E" + sttHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["F" + sttHeader + ":F" + sttHeader].Merge = true;
                        range.Worksheet.Cells["F" + sttHeader + ":F" + sttHeader].Value = "(2)";
                        range.Worksheet.Cells["F" + sttHeader + ":F" + sttHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["F" + sttHeader + ":F" + sttHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["G" + sttHeader + ":G" + sttHeader].Merge = true;
                        range.Worksheet.Cells["G" + sttHeader + ":G" + sttHeader].Value = "(3)";
                        range.Worksheet.Cells["G" + sttHeader + ":G" + sttHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G" + sttHeader + ":G" + sttHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["H" + sttHeader + ":H" + sttHeader].Merge = true;
                        range.Worksheet.Cells["H" + sttHeader + ":H" + sttHeader].Value = "(4)";
                        range.Worksheet.Cells["H" + sttHeader + ":H" + sttHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["H" + sttHeader + ":H" + sttHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["I" + sttHeader + ":I" + sttHeader].Merge = true;
                        range.Worksheet.Cells["I" + sttHeader + ":I" + sttHeader].Value = "(5)";
                        range.Worksheet.Cells["I" + sttHeader + ":I" + sttHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["I" + sttHeader + ":I" + sttHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["J" + sttHeader + ":J" + sttHeader].Merge = true;
                        range.Worksheet.Cells["J" + sttHeader + ":J" + sttHeader].Value = "(6)";
                        range.Worksheet.Cells["J" + sttHeader + ":J" + sttHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["J" + sttHeader + ":J" + sttHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["K" + sttHeader + ":K" + sttHeader].Merge = true;
                        range.Worksheet.Cells["K" + sttHeader + ":K" + sttHeader].Value = "(7)";
                        range.Worksheet.Cells["K" + sttHeader + ":K" + sttHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["K" + sttHeader + ":K" + sttHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["L" + sttHeader + ":L" + sttHeader].Merge = true;
                        range.Worksheet.Cells["L" + sttHeader + ":L" + sttHeader].Value = "(8)";
                        range.Worksheet.Cells["L" + sttHeader + ":L" + sttHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["L" + sttHeader + ":L" + sttHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["M" + sttHeader + ":M" + sttHeader].Merge = true;
                        range.Worksheet.Cells["M" + sttHeader + ":M" + sttHeader].Value = "(9)";
                        range.Worksheet.Cells["M" + sttHeader + ":M" + sttHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["M" + sttHeader + ":M" + sttHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["N" + sttHeader + ":N" + sttHeader].Merge = true;
                        range.Worksheet.Cells["N" + sttHeader + ":N" + sttHeader].Value = "(10)";
                        range.Worksheet.Cells["N" + sttHeader + ":N" + sttHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["N" + sttHeader + ":N" + sttHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["O" + sttHeader + ":O" + sttHeader].Merge = true;
                        range.Worksheet.Cells["O" + sttHeader + ":O" + sttHeader].Value = "(11)";
                        range.Worksheet.Cells["O" + sttHeader + ":O" + sttHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["O" + sttHeader + ":O" + sttHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["P" + sttHeader + ":P" + sttHeader].Merge = true;
                        range.Worksheet.Cells["P" + sttHeader + ":P" + sttHeader].Value = "(12)";
                        range.Worksheet.Cells["P" + sttHeader + ":P" + sttHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["P" + sttHeader + ":P" + sttHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["Q" + sttHeader + ":Q" + sttHeader].Merge = true;
                        range.Worksheet.Cells["Q" + sttHeader + ":Q" + sttHeader].Value = "(13)";
                        range.Worksheet.Cells["Q" + sttHeader + ":Q" + sttHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["Q" + sttHeader + ":Q" + sttHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["R" + sttHeader + ":R" + sttHeader].Merge = true;
                        range.Worksheet.Cells["R" + sttHeader + ":R" + sttHeader].Value = "(14)";
                        range.Worksheet.Cells["R" + sttHeader + ":R" + sttHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["R" + sttHeader + ":R" + sttHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["S" + sttHeader + ":S" + sttHeader].Merge = true;
                        range.Worksheet.Cells["S" + sttHeader + ":S" + sttHeader].Value = "(15)";
                        range.Worksheet.Cells["S" + sttHeader + ":S" + sttHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["S" + sttHeader + ":S" + sttHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["T" + sttHeader + ":T" + sttHeader].Merge = true;
                        range.Worksheet.Cells["T" + sttHeader + ":T" + sttHeader].Value = "(16)";
                        range.Worksheet.Cells["T" + sttHeader + ":T" + sttHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["T" + sttHeader + ":T" + sttHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    }


                    var manager = new PropertyManager<DanhSachDeNghiThanhToanChiPhiKCBNoiTru>(requestProperties);

                    int index = sttHeader + 1;

                    var stt = 1;

                    var lstNoiDangKyKCBBanDau = datas.GroupBy(x => new { x.NoiDangKyKCBBanDau }).Select(item => new
                    {
                        item.First().NoiDangKyKCBBanDauDisplay

                    }).ToList();

                    if (lstNoiDangKyKCBBanDau.Any())
                    {
                        foreach (var item in lstNoiDangKyKCBBanDau)
                        {
                            var dataTheoGroups = datas.Where(o => o.NoiDangKyKCBBanDauDisplay == item.NoiDangKyKCBBanDauDisplay).ToList();

                            if (dataTheoGroups.Any())
                            {
                                using (var range = worksheet.Cells["A" + index + ":T" + index])
                                {
                                    range.Worksheet.Cells["A" + index + ":T" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                    range.Worksheet.Cells["A" + index + ":T" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                    range.Worksheet.Cells["A" + index + ":T" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["A" + index + ":T" + index].Style.Font.Color.SetColor(Color.Black);
                                    range.Worksheet.Cells["A" + index + ":T" + index].Value = item.NoiDangKyKCBBanDauDisplay;
                                    range.Worksheet.Cells["A" + index + ":T" + index].Merge = true;
                                }

                                index++;
                                foreach (var dataItem in dataTheoGroups)
                                {
                                    if (dataTheoGroups.Any())
                                    {
                                        using (var range = worksheet.Cells["B" + index + ":T" + index])
                                        {
                                            range.Worksheet.Cells["B" + index + ":T" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                            range.Worksheet.Cells["B" + index + ":T" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells["B" + index + ":T" + index].Style.Font.Color.SetColor(Color.Black);


                                            worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                            worksheet.Cells["A" + index].Value = stt;

                                            worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["B" + index].Value = dataItem.KhamChuaBenhDisplay;

                                            worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            worksheet.Cells["C" + index].Value = dataItem.SoLanDenKham;

                                            worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            worksheet.Cells["D" + index].Value = dataItem.TongSoNgayDieuTri;

                                            worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            worksheet.Cells["E" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["E" + index].Value = dataItem.TongCongChiPhiKCB;

                                            worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            worksheet.Cells["F" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["F" + index].Value = dataItem.ChiPhiXetNghiemKhongApDungTLTT;

                                            worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            worksheet.Cells["G" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["G" + index].Value = dataItem.ChiPhiCDHATDCNKhongApDungTLTT;

                                            worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["H" + index].Value = dataItem.ChiPhiThuocKhongApDungTLTT;

                                            worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            worksheet.Cells["I" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["I" + index].Value = dataItem.ChiPhiMauKhongApDungTLTT;

                                            worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["J" + index].Value = dataItem.ChiPhiTTPTKhongApDungTLTT;

                                            worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["K" + index].Value = dataItem.ChiPhiVTYTKhongApDungTLTT;

                                            worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["L" + index].Value = dataItem.ChiPhiDVKTThanhToanTiLe;

                                            worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["M" + index].Value = dataItem.ChiPhiThuocThanhToanTiLe;

                                            worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["N" + index].Value = dataItem.ChiPhiVTYTThanhToanTiLe;

                                            worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["O" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            worksheet.Cells["O" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["O" + index].Value = dataItem.TienKham;

                                            worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["P" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            worksheet.Cells["P" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["P" + index].Value = dataItem.TienGiuong;

                                            worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            worksheet.Cells["Q" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["Q" + index].Value = dataItem.VanChuyen;

                                            worksheet.Cells["R" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["R" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            worksheet.Cells["R" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["R" + index].Value = dataItem.NguoiBenhChiTra;

                                            worksheet.Cells["S" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["S" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            worksheet.Cells["S" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["S" + index].Value = dataItem.TongCongChiPhiBHXHThanhToan;

                                            worksheet.Cells["T" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["T" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            worksheet.Cells["T" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["T" + index].Value = dataItem.ChiPhiNgoaiQuyDinhSuat;
                                        }

                                        index++;
                                    }
                                }

                                using (var range = worksheet.Cells["B" + index + ":R" + index])
                                {
                                    range.Worksheet.Cells["B" + index + ":R" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                    range.Worksheet.Cells["B" + index + ":R" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                    range.Worksheet.Cells["B" + index + ":R" + index].Style.Font.Color.SetColor(Color.Black);


                                    worksheet.Cells["A" + index + ":B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["A" + index + ":B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["A" + index + ":B" + index].Merge = true;
                                    worksheet.Cells["A" + index + ":B" + index].Value = "Cộng:";



                                    worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["C" + index].Value = dataTheoGroups.Sum(s => s.SoLanDenKham);

                                    worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;           
                                    worksheet.Cells["D" + index].Value = dataTheoGroups.Sum(s => s.TongSoNgayDieuTri);

                                    worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["E" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["E" + index].Value = dataTheoGroups.Sum(s => s.TongCongChiPhiKCB);

                                    worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["F" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["F" + index].Value = dataTheoGroups.Sum(s => s.ChiPhiXetNghiemKhongApDungTLTT);

                                    worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["G" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["G" + index].Value = dataTheoGroups.Sum(s => s.ChiPhiCDHATDCNKhongApDungTLTT);

                                    worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["H" + index].Value = dataTheoGroups.Sum(s => s.ChiPhiThuocKhongApDungTLTT);

                                    worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["I" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["I" + index].Value = dataTheoGroups.Sum(s => s.ChiPhiMauKhongApDungTLTT);

                                    worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["J" + index].Value = dataTheoGroups.Sum(s => s.ChiPhiTTPTKhongApDungTLTT);

                                    worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["K" + index].Value = dataTheoGroups.Sum(s => s.ChiPhiVTYTKhongApDungTLTT);

                                    worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["L" + index].Value = dataTheoGroups.Sum(s => s.ChiPhiDVKTThanhToanTiLe);

                                    worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["M" + index].Value = dataTheoGroups.Sum(s => s.ChiPhiThuocThanhToanTiLe);

                                    worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["N" + index].Value = dataTheoGroups.Sum(s => s.ChiPhiVTYTThanhToanTiLe);

                                    worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["O" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["O" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["O" + index].Value = dataTheoGroups.Sum(s => s.TienKham);

                                    worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["P" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["P" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["P" + index].Value = dataTheoGroups.Sum(s => s.TienGiuong);

                                    worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["Q" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["Q" + index].Value = dataTheoGroups.Sum(s => s.VanChuyen);

                                    worksheet.Cells["R" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["R" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["R" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["R" + index].Value = dataTheoGroups.Sum(s => s.NguoiBenhChiTra);

                                    worksheet.Cells["S" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["S" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["S" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["S" + index].Value = dataTheoGroups.Sum(s => s.TongCongChiPhiBHXHThanhToan);

                                    worksheet.Cells["T" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["T" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["T" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["T" + index].Value = dataTheoGroups.Sum(s => s.ChiPhiNgoaiQuyDinhSuat);
                                }
                                index++;
                            }
                        }

                    }

                    using (var range = worksheet.Cells["B" + index + ":T" + index])
                    {
                        range.Worksheet.Cells["B" + index + ":T" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["B" + index + ":T" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["B" + index + ":T" + index].Style.Font.Color.SetColor(Color.Black);

                        worksheet.Cells["A" + index + ":B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["A" + index + ":B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells["A" + index + ":B" + index].Merge = true;
                        worksheet.Cells["A" + index + ":B" + index].Value = "Tổng cộng A + B + C + D:";



                        worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["C" + index].Value = datas.Sum(s => s.SoLanDenKham);

                        worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["D" + index].Value = datas.Sum(s => s.TongSoNgayDieuTri);

                        worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["E" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["E" + index].Value = datas.Sum(s => s.TongCongChiPhiKCB);

                        worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["F" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["F" + index].Value = datas.Sum(s => s.ChiPhiXetNghiemKhongApDungTLTT);

                        worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["G" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["G" + index].Value = datas.Sum(s => s.ChiPhiCDHATDCNKhongApDungTLTT);

                        worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["H" + index].Value = datas.Sum(s => s.ChiPhiThuocKhongApDungTLTT);

                        worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["I" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["I" + index].Value = datas.Sum(s => s.ChiPhiMauKhongApDungTLTT);

                        worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["J" + index].Value = datas.Sum(s => s.ChiPhiTTPTKhongApDungTLTT);

                        worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["K" + index].Value = datas.Sum(s => s.ChiPhiVTYTKhongApDungTLTT);

                        worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["L" + index].Value = datas.Sum(s => s.ChiPhiDVKTThanhToanTiLe);

                        worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["M" + index].Value = datas.Sum(s => s.ChiPhiThuocThanhToanTiLe);

                        worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["N" + index].Value = datas.Sum(s => s.ChiPhiVTYTThanhToanTiLe);

                        worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["O" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["O" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["O" + index].Value = datas.Sum(s => s.TienKham);

                        worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["P" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["P" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["P" + index].Value = datas.Sum(s => s.TienGiuong);

                        worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["Q" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["Q" + index].Value = datas.Sum(s => s.VanChuyen);

                        worksheet.Cells["R" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["R" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["R" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["R" + index].Value = datas.Sum(s => s.NguoiBenhChiTra);

                        worksheet.Cells["S" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["S" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["S" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["S" + index].Value = datas.Sum(s => s.TongCongChiPhiBHXHThanhToan);

                        worksheet.Cells["T" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["T" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["T" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["T" + index].Value = datas.Sum(s => s.ChiPhiNgoaiQuyDinhSuat);
                    }

                    index = index + 2;

                    using (var range = worksheet.Cells["B" + index + ":T" + index])
                    {
                        range.Worksheet.Cells["B" + index + ":T" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["B" + index + ":T" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["B" + index + ":T" + index].Style.Font.Color.SetColor(Color.Black);

                        worksheet.Cells["A" + index + ":T" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells["A" + index + ":T" + index].Merge = true;
                        worksheet.Cells["A" + index + ":T" + index].Value = $"Tổng số tiền đề nghị thanh toán (viết bằng chữ):" +
                            $" {NumberHelper.ChuyenSoRaText(Convert.ToDouble(datas.Sum(o => o.TongCongChiPhiBHXHThanhToan)))}";
                    }

                    index = index + 2;

                    using (var range = worksheet.Cells["N" + index + ":T" + index])
                    {
                        range.Worksheet.Cells["N" + index + ":T" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["N" + index + ":T" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["N" + index + ":T" + index].Style.Font.Color.SetColor(Color.Black);

                        worksheet.Cells["N" + index + ":T" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["N" + index + ":T" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["N" + index + ":T" + index].Merge = true;
                        worksheet.Cells["N" + index + ":T" + index].Value = $"Ngày ...... tháng ...... năm ............:";
                    }

                    index++;
                    using (var range = worksheet.Cells["A" + index + ":T" + index])
                    {
                        range.Worksheet.Cells["A" + index + ":T" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A" + index + ":T" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + index + ":T" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A" + index + ":T" + index].Style.Font.Bold = true;

                        worksheet.Cells["A" + index + ":D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["A" + index + ":D" + index].Merge = true;
                        worksheet.Cells["A" + index + ":D" + index].Value = $"Người lập biểu";

                        worksheet.Cells["E" + index + ":I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["E" + index + ":I" + index].Merge = true;
                        worksheet.Cells["E" + index + ":I" + index].Value = $"Trưởng phòng KHTH";

                        worksheet.Cells["J" + index + ":M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["J" + index + ":M" + index].Merge = true;
                        worksheet.Cells["J" + index + ":M" + index].Value = $"Kế toán trưởng";

                        worksheet.Cells["N" + index + ":T" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["N" + index + ":T" + index].Merge = true;
                        worksheet.Cells["N" + index + ":T" + index].Value = $"Thủ trưởng đơn vị";
                    }

                    index++;
                    using (var range = worksheet.Cells["A" + index + ":T" + index])
                    {
                        range.Worksheet.Cells["A" + index + ":T" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A" + index + ":T" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + index + ":T" + index].Style.Font.Color.SetColor(Color.Black);


                        worksheet.Cells["A" + index + ":D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["A" + index + ":D" + index].Merge = true;
                        worksheet.Cells["A" + index + ":D" + index].Value = $"(Ký, họ tên)";

                        worksheet.Cells["E" + index + ":I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["E" + index + ":I" + index].Merge = true;
                        worksheet.Cells["E" + index + ":I" + index].Value = $"(Ký, họ tên)";

                        worksheet.Cells["J" + index + ":M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["J" + index + ":M" + index].Merge = true;
                        worksheet.Cells["J" + index + ":M" + index].Value = $"(Ký, họ tên)";

                        worksheet.Cells["N" + index + ":T" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["N" + index + ":T" + index].Merge = true;
                        worksheet.Cells["N" + index + ":T" + index].Value = $"(Ký, họ tên, đóng dấu)";
                    }
                    xlPackage.Save();
                }
                return stream.ToArray();

            }
        }

        private async Task<GridDataSource> GetDanhSachDeNghiThanhToanChiPhiKCBNgoaiTruForGrid_v1(DanhSachDeNghiThanhToanChiPhiKCBQueryInfoVo queryInfo)
        {
            var allYeuCauTiepNhanNgoaiTrus = _yeuCauTiepNhanRepository.TableNoTracking
                .Where(o => o.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru && o.CoBHYT == true
                            && o.DuyetBaoHiems.Any(d => d.ThoiDiemDuyetBaoHiem >= queryInfo.TuNgay && d.ThoiDiemDuyetBaoHiem < queryInfo.DenNgay)
                            && o.QuyetToanTheoNoiTru != true)
                .Select(o => new
                {
                    Id = o.Id,
                    ThoiDiemDuyetBaoHiems = o.DuyetBaoHiems.Select(d => d.ThoiDiemDuyetBaoHiem).ToList()
                }).ToList();

            var yeuCauTiepNhanNgoaiTruIds = new List<long>();

            foreach (var yeuCauTiepNhanNgoaiTru in allYeuCauTiepNhanNgoaiTrus)
            {
                var thoiDiemDuyetBaoHiemCuoi = yeuCauTiepNhanNgoaiTru.ThoiDiemDuyetBaoHiems.OrderBy(t => t).Last();
                if (thoiDiemDuyetBaoHiemCuoi >= queryInfo.TuNgay && thoiDiemDuyetBaoHiemCuoi < queryInfo.DenNgay)
                {
                    yeuCauTiepNhanNgoaiTruIds.Add(yeuCauTiepNhanNgoaiTru.Id);
                }
            }

            var thongTinYeuCauTiepNhans = _yeuCauTiepNhanRepository.TableNoTracking
                .Where(o => yeuCauTiepNhanNgoaiTruIds.Contains(o.Id))
                .Select(o => new
                {
                    o.Id,
                    o.BHYTMaDKBD
                }).ToList();

            var thongTinYeuCauKhamBenhs = _yeuCauKhamBenhRepository.TableNoTracking
                .Where(o => yeuCauTiepNhanNgoaiTruIds.Contains(o.YeuCauTiepNhanId) && o.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham && o.DuocHuongBaoHiem && o.BaoHiemChiTra == true)
                .Select(o => new
                {
                    o.YeuCauTiepNhanId,
                    DonGiaBaoHiem = o.DonGiaBaoHiem.GetValueOrDefault(),
                    TiLeBaoHiemThanhToan = o.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    MucHuongBaoHiem = o.MucHuongBaoHiem.GetValueOrDefault()
                }).ToList();

            var thongTinYeuCauDichVuKyThuats = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(o => yeuCauTiepNhanNgoaiTruIds.Contains(o.YeuCauTiepNhanId) && o.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && o.DuocHuongBaoHiem && o.BaoHiemChiTra == true)
                .Select(o => new
                {
                    o.YeuCauTiepNhanId,
                    o.NhomChiPhi,
                    o.SoLan,
                    DonGiaBaoHiem = o.DonGiaBaoHiem.GetValueOrDefault(),
                    TiLeBaoHiemThanhToan = o.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    MucHuongBaoHiem = o.MucHuongBaoHiem.GetValueOrDefault()
                }).ToList();

            var thongTinYeuCauDuocPhams = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                .Where(o => yeuCauTiepNhanNgoaiTruIds.Contains(o.YeuCauTiepNhanId) && o.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy && o.DuocHuongBaoHiem && o.BaoHiemChiTra == true)
                .Select(o => new
                {
                    o.YeuCauTiepNhanId,
                    o.SoLuong,
                    DonGiaBaoHiem = o.DonGiaBaoHiem.GetValueOrDefault(),
                    TiLeBaoHiemThanhToan = o.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    MucHuongBaoHiem = o.MucHuongBaoHiem.GetValueOrDefault()
                }).ToList();

            var thongTinYeuCauVatTus = _yeuCauVatTuBenhVienRepository.TableNoTracking
                .Where(o => yeuCauTiepNhanNgoaiTruIds.Contains(o.YeuCauTiepNhanId) && o.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy && o.DuocHuongBaoHiem && o.BaoHiemChiTra == true)
                .Select(o => new
                {
                    o.YeuCauTiepNhanId,
                    o.SoLuong,
                    DonGiaBaoHiem = o.DonGiaBaoHiem.GetValueOrDefault(),
                    TiLeBaoHiemThanhToan = o.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    MucHuongBaoHiem = o.MucHuongBaoHiem.GetValueOrDefault()
                }).ToList();

            var thongTinDonThuocs = _donThuocThanhToanRepository.TableNoTracking
                .Where(o => yeuCauTiepNhanNgoaiTruIds.Contains(o.YeuCauTiepNhanId.GetValueOrDefault()) && o.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocBHYT && o.TrangThai != TrangThaiDonThuocThanhToan.DaHuy)
                .SelectMany(o => o.DonThuocThanhToanChiTiets).Where(ct => ct.DuocHuongBaoHiem && ct.BaoHiemChiTra == true)
                .Select(o => new
                {
                    YeuCauTiepNhanId = o.DonThuocThanhToan.YeuCauTiepNhanId,
                    SoLuong = o.SoLuong,
                    DonGiaBaoHiem = o.DonGiaBaoHiem.GetValueOrDefault(),
                    TiLeBaoHiemThanhToan = o.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    MucHuongBaoHiem = o.MucHuongBaoHiem.GetValueOrDefault()
                }).ToList();

            var cauHinhBaoHiemYTe = _cauHinhService.LoadSetting<BaoHiemYTe>();

            var dsTiepNhanKhamChuaBenhBanDauIds = thongTinYeuCauTiepNhans
                .Where(o => o.BHYTMaDKBD == cauHinhBaoHiemYTe.BenhVienTiepNhan)
                .Select(o => o.Id)
                .ToList();
            var dsTiepNhanNoiTinhIds = thongTinYeuCauTiepNhans
                .Where(o => o.BHYTMaDKBD != cauHinhBaoHiemYTe.BenhVienTiepNhan && o.BHYTMaDKBD != null && o.BHYTMaDKBD.StartsWith("01"))
                .Select(o => o.Id)
                .ToList();
            var dsTiepNhanNgoaiTinhIds = thongTinYeuCauTiepNhans
                .Where(o => o.BHYTMaDKBD != cauHinhBaoHiemYTe.BenhVienTiepNhan && (o.BHYTMaDKBD == null || !o.BHYTMaDKBD.StartsWith("01")))
                .Select(o => o.Id)
                .ToList();

            var chiPhiKhamChuaBenhBanDau = new DanhSachDeNghiThanhToanChiPhiKCBNgoaiTru
            {
                NoiDangKyKCBBanDau = NoiDangKyKCBBanDau.BenhNhanNoiTinhKhamChuaBenhBanDau,
                KhamChuaBenh = EnumLyDoVaoVien.DungTuyen,
                SoLanDenKham = dsTiepNhanKhamChuaBenhBanDauIds.Count(),
                ChiPhiXetNghiemKhongApDungTLTT = 0,
                ChiPhiCDHATDCNKhongApDungTLTT = 0,
                ChiPhiThuocKhongApDungTLTT = 0,
                ChiPhiMauKhongApDungTLTT = 0,
                ChiPhiTTPTKhongApDungTLTT = 0,
                ChiPhiVTYTKhongApDungTLTT = 0,
                ChiPhiDVKTThanhToanTiLe = 0,
                ChiPhiThuocThanhToanTiLe = 0,
                ChiPhiVTYTThanhToanTiLe = 0,
                TienKham = 0,
                VanChuyen = 0,
                TongCongChiPhiBHXHThanhToan = 0,
                ChiPhiNgoaiQuyDinhSuat = 0
            };
            var chiPhiKhamChuaBenhNoiTinh = new DanhSachDeNghiThanhToanChiPhiKCBNgoaiTru
            {
                NoiDangKyKCBBanDau = NoiDangKyKCBBanDau.BenhNhanNoiTinhDen,
                KhamChuaBenh = EnumLyDoVaoVien.DungTuyen,
                SoLanDenKham = dsTiepNhanNoiTinhIds.Count(),
                ChiPhiXetNghiemKhongApDungTLTT = 0,
                ChiPhiCDHATDCNKhongApDungTLTT = 0,
                ChiPhiThuocKhongApDungTLTT = 0,
                ChiPhiMauKhongApDungTLTT = 0,
                ChiPhiTTPTKhongApDungTLTT = 0,
                ChiPhiVTYTKhongApDungTLTT = 0,
                ChiPhiDVKTThanhToanTiLe = 0,
                ChiPhiThuocThanhToanTiLe = 0,
                ChiPhiVTYTThanhToanTiLe = 0,
                TienKham = 0,
                VanChuyen = 0,
                TongCongChiPhiBHXHThanhToan = 0,
                ChiPhiNgoaiQuyDinhSuat = 0
            };
            var chiPhiKhamChuaBenhNgoaiTinh = new DanhSachDeNghiThanhToanChiPhiKCBNgoaiTru
            {
                NoiDangKyKCBBanDau = NoiDangKyKCBBanDau.BenhNhanNgoaiTinhDen,
                KhamChuaBenh = EnumLyDoVaoVien.DungTuyen,
                SoLanDenKham = dsTiepNhanNgoaiTinhIds.Count(),
                ChiPhiXetNghiemKhongApDungTLTT = 0,
                ChiPhiCDHATDCNKhongApDungTLTT = 0,
                ChiPhiThuocKhongApDungTLTT = 0,
                ChiPhiMauKhongApDungTLTT = 0,
                ChiPhiTTPTKhongApDungTLTT = 0,
                ChiPhiVTYTKhongApDungTLTT = 0,
                ChiPhiDVKTThanhToanTiLe = 0,
                ChiPhiThuocThanhToanTiLe = 0,
                ChiPhiVTYTThanhToanTiLe = 0,
                TienKham = 0,
                VanChuyen = 0,
                TongCongChiPhiBHXHThanhToan = 0,
                ChiPhiNgoaiQuyDinhSuat = 0
            };

            var dsNgoaiTrus = new List<DanhSachDeNghiThanhToanChiPhiKCBNgoaiTru> { chiPhiKhamChuaBenhBanDau, chiPhiKhamChuaBenhNoiTinh, chiPhiKhamChuaBenhNgoaiTinh };

            foreach (var thongTinYeuCauKhamBenh in thongTinYeuCauKhamBenhs)
            {
                var thanhTienBHYT = Math.Round(thongTinYeuCauKhamBenh.DonGiaBaoHiem * thongTinYeuCauKhamBenh.TiLeBaoHiemThanhToan / 100, 2);
                var tienBHYTThanhToan = Math.Round(thanhTienBHYT * thongTinYeuCauKhamBenh.MucHuongBaoHiem / 100, 2);

                if (dsTiepNhanKhamChuaBenhBanDauIds.Contains(thongTinYeuCauKhamBenh.YeuCauTiepNhanId))
                {
                    chiPhiKhamChuaBenhBanDau.TienKham += thanhTienBHYT;
                    chiPhiKhamChuaBenhBanDau.TongCongChiPhiBHXHThanhToan += tienBHYTThanhToan;
                }
                else if (dsTiepNhanNoiTinhIds.Contains(thongTinYeuCauKhamBenh.YeuCauTiepNhanId))
                {
                    chiPhiKhamChuaBenhNoiTinh.TienKham += thanhTienBHYT;
                    chiPhiKhamChuaBenhNoiTinh.TongCongChiPhiBHXHThanhToan += tienBHYTThanhToan;
                }
                else if (dsTiepNhanNgoaiTinhIds.Contains(thongTinYeuCauKhamBenh.YeuCauTiepNhanId))
                {
                    chiPhiKhamChuaBenhNgoaiTinh.TienKham += thanhTienBHYT;
                    chiPhiKhamChuaBenhNgoaiTinh.TongCongChiPhiBHXHThanhToan += tienBHYTThanhToan;
                }
            }
            foreach (var thongTinYeuCauDichVuKyThuat in thongTinYeuCauDichVuKyThuats)
            {
                var thanhTienBHYT = Math.Round(thongTinYeuCauDichVuKyThuat.DonGiaBaoHiem * thongTinYeuCauDichVuKyThuat.SoLan * thongTinYeuCauDichVuKyThuat.TiLeBaoHiemThanhToan / 100, 2);
                var tienBHYTThanhToan = Math.Round(thanhTienBHYT * thongTinYeuCauDichVuKyThuat.MucHuongBaoHiem / 100, 2);

                if (thongTinYeuCauDichVuKyThuat.NhomChiPhi == EnumDanhMucNhomTheoChiPhi.XetNghiem)
                {
                    if (dsTiepNhanKhamChuaBenhBanDauIds.Contains(thongTinYeuCauDichVuKyThuat.YeuCauTiepNhanId))
                    {
                        chiPhiKhamChuaBenhBanDau.ChiPhiXetNghiemKhongApDungTLTT += thanhTienBHYT;
                        chiPhiKhamChuaBenhBanDau.TongCongChiPhiBHXHThanhToan += tienBHYTThanhToan;
                    }
                    else if (dsTiepNhanNoiTinhIds.Contains(thongTinYeuCauDichVuKyThuat.YeuCauTiepNhanId))
                    {
                        chiPhiKhamChuaBenhNoiTinh.ChiPhiXetNghiemKhongApDungTLTT += thanhTienBHYT;
                        chiPhiKhamChuaBenhNoiTinh.TongCongChiPhiBHXHThanhToan += tienBHYTThanhToan;
                    }
                    else if (dsTiepNhanNgoaiTinhIds.Contains(thongTinYeuCauDichVuKyThuat.YeuCauTiepNhanId))
                    {
                        chiPhiKhamChuaBenhNgoaiTinh.ChiPhiXetNghiemKhongApDungTLTT += thanhTienBHYT;
                        chiPhiKhamChuaBenhNgoaiTinh.TongCongChiPhiBHXHThanhToan += tienBHYTThanhToan;
                    }
                }
                else if (thongTinYeuCauDichVuKyThuat.NhomChiPhi == EnumDanhMucNhomTheoChiPhi.ChuanDoanHinhAnh || thongTinYeuCauDichVuKyThuat.NhomChiPhi == EnumDanhMucNhomTheoChiPhi.ThamDoChucNang)
                {
                    if (dsTiepNhanKhamChuaBenhBanDauIds.Contains(thongTinYeuCauDichVuKyThuat.YeuCauTiepNhanId))
                    {
                        chiPhiKhamChuaBenhBanDau.ChiPhiCDHATDCNKhongApDungTLTT += thanhTienBHYT;
                        chiPhiKhamChuaBenhBanDau.TongCongChiPhiBHXHThanhToan += tienBHYTThanhToan;
                    }
                    else if (dsTiepNhanNoiTinhIds.Contains(thongTinYeuCauDichVuKyThuat.YeuCauTiepNhanId))
                    {
                        chiPhiKhamChuaBenhNoiTinh.ChiPhiCDHATDCNKhongApDungTLTT += thanhTienBHYT;
                        chiPhiKhamChuaBenhNoiTinh.TongCongChiPhiBHXHThanhToan += tienBHYTThanhToan;
                    }
                    else if (dsTiepNhanNgoaiTinhIds.Contains(thongTinYeuCauDichVuKyThuat.YeuCauTiepNhanId))
                    {
                        chiPhiKhamChuaBenhNgoaiTinh.ChiPhiCDHATDCNKhongApDungTLTT += thanhTienBHYT;
                        chiPhiKhamChuaBenhNgoaiTinh.TongCongChiPhiBHXHThanhToan += tienBHYTThanhToan;
                    }
                }
                else if (thongTinYeuCauDichVuKyThuat.NhomChiPhi == EnumDanhMucNhomTheoChiPhi.ThuThuatPhauThuat || thongTinYeuCauDichVuKyThuat.NhomChiPhi == EnumDanhMucNhomTheoChiPhi.ThuThuat)
                {
                    if (dsTiepNhanKhamChuaBenhBanDauIds.Contains(thongTinYeuCauDichVuKyThuat.YeuCauTiepNhanId))
                    {
                        chiPhiKhamChuaBenhBanDau.ChiPhiTTPTKhongApDungTLTT += thanhTienBHYT;
                        chiPhiKhamChuaBenhBanDau.TongCongChiPhiBHXHThanhToan += tienBHYTThanhToan;
                    }
                    else if (dsTiepNhanNoiTinhIds.Contains(thongTinYeuCauDichVuKyThuat.YeuCauTiepNhanId))
                    {
                        chiPhiKhamChuaBenhNoiTinh.ChiPhiTTPTKhongApDungTLTT += thanhTienBHYT;
                        chiPhiKhamChuaBenhNoiTinh.TongCongChiPhiBHXHThanhToan += tienBHYTThanhToan;
                    }
                    else if (dsTiepNhanNgoaiTinhIds.Contains(thongTinYeuCauDichVuKyThuat.YeuCauTiepNhanId))
                    {
                        chiPhiKhamChuaBenhNgoaiTinh.ChiPhiTTPTKhongApDungTLTT += thanhTienBHYT;
                        chiPhiKhamChuaBenhNgoaiTinh.TongCongChiPhiBHXHThanhToan += tienBHYTThanhToan;
                    }
                }
            }

            foreach (var thongTinYeuCauDuocPham in thongTinYeuCauDuocPhams)
            {
                var thanhTienBHYT = Math.Round(thongTinYeuCauDuocPham.DonGiaBaoHiem * (decimal)thongTinYeuCauDuocPham.SoLuong * thongTinYeuCauDuocPham.TiLeBaoHiemThanhToan / 100, 2);
                var tienBHYTThanhToan = Math.Round(thanhTienBHYT * thongTinYeuCauDuocPham.MucHuongBaoHiem / 100, 2);

                if (dsTiepNhanKhamChuaBenhBanDauIds.Contains(thongTinYeuCauDuocPham.YeuCauTiepNhanId))
                {
                    chiPhiKhamChuaBenhBanDau.ChiPhiThuocKhongApDungTLTT += thanhTienBHYT;
                    chiPhiKhamChuaBenhBanDau.TongCongChiPhiBHXHThanhToan += tienBHYTThanhToan;
                }
                else if (dsTiepNhanNoiTinhIds.Contains(thongTinYeuCauDuocPham.YeuCauTiepNhanId))
                {
                    chiPhiKhamChuaBenhNoiTinh.ChiPhiThuocKhongApDungTLTT += thanhTienBHYT;
                    chiPhiKhamChuaBenhNoiTinh.TongCongChiPhiBHXHThanhToan += tienBHYTThanhToan;
                }
                else if (dsTiepNhanNgoaiTinhIds.Contains(thongTinYeuCauDuocPham.YeuCauTiepNhanId))
                {
                    chiPhiKhamChuaBenhNgoaiTinh.ChiPhiThuocKhongApDungTLTT += thanhTienBHYT;
                    chiPhiKhamChuaBenhNgoaiTinh.TongCongChiPhiBHXHThanhToan += tienBHYTThanhToan;
                }
            }

            foreach (var thongTinYeuCauVatTu in thongTinYeuCauVatTus)
            {
                var thanhTienBHYT = Math.Round(thongTinYeuCauVatTu.DonGiaBaoHiem * (decimal)thongTinYeuCauVatTu.SoLuong * thongTinYeuCauVatTu.TiLeBaoHiemThanhToan / 100, 2);
                var tienBHYTThanhToan = Math.Round(thanhTienBHYT * thongTinYeuCauVatTu.MucHuongBaoHiem / 100, 2);

                if (dsTiepNhanKhamChuaBenhBanDauIds.Contains(thongTinYeuCauVatTu.YeuCauTiepNhanId))
                {
                    chiPhiKhamChuaBenhBanDau.ChiPhiVTYTKhongApDungTLTT += thanhTienBHYT;
                    chiPhiKhamChuaBenhBanDau.TongCongChiPhiBHXHThanhToan += tienBHYTThanhToan;
                }
                else if (dsTiepNhanNoiTinhIds.Contains(thongTinYeuCauVatTu.YeuCauTiepNhanId))
                {
                    chiPhiKhamChuaBenhNoiTinh.ChiPhiVTYTKhongApDungTLTT += thanhTienBHYT;
                    chiPhiKhamChuaBenhNoiTinh.TongCongChiPhiBHXHThanhToan += tienBHYTThanhToan;
                }
                else if (dsTiepNhanNgoaiTinhIds.Contains(thongTinYeuCauVatTu.YeuCauTiepNhanId))
                {
                    chiPhiKhamChuaBenhNgoaiTinh.ChiPhiVTYTKhongApDungTLTT += thanhTienBHYT;
                    chiPhiKhamChuaBenhNgoaiTinh.TongCongChiPhiBHXHThanhToan += tienBHYTThanhToan;
                }
            }

            foreach (var thongTinDonThuoc in thongTinDonThuocs)
            {
                var thanhTienBHYT = Math.Round(thongTinDonThuoc.DonGiaBaoHiem * (decimal)thongTinDonThuoc.SoLuong * thongTinDonThuoc.TiLeBaoHiemThanhToan / 100, 2);
                var tienBHYTThanhToan = Math.Round(thanhTienBHYT * thongTinDonThuoc.MucHuongBaoHiem / 100, 2);

                if (dsTiepNhanKhamChuaBenhBanDauIds.Contains(thongTinDonThuoc.YeuCauTiepNhanId.GetValueOrDefault()))
                {
                    chiPhiKhamChuaBenhBanDau.ChiPhiThuocKhongApDungTLTT += thanhTienBHYT;
                    chiPhiKhamChuaBenhBanDau.TongCongChiPhiBHXHThanhToan += tienBHYTThanhToan;
                }
                else if (dsTiepNhanNoiTinhIds.Contains(thongTinDonThuoc.YeuCauTiepNhanId.GetValueOrDefault()))
                {
                    chiPhiKhamChuaBenhNoiTinh.ChiPhiThuocKhongApDungTLTT += thanhTienBHYT;
                    chiPhiKhamChuaBenhNoiTinh.TongCongChiPhiBHXHThanhToan += tienBHYTThanhToan;
                }
                else if (dsTiepNhanNgoaiTinhIds.Contains(thongTinDonThuoc.YeuCauTiepNhanId.GetValueOrDefault()))
                {
                    chiPhiKhamChuaBenhNgoaiTinh.ChiPhiThuocKhongApDungTLTT += thanhTienBHYT;
                    chiPhiKhamChuaBenhNgoaiTinh.TongCongChiPhiBHXHThanhToan += tienBHYTThanhToan;
                }
            }

            return new GridDataSource
            {
                Data = dsNgoaiTrus.ToArray(),
                TotalRowCount = dsNgoaiTrus.Count()
            };
        }

        /*
        private List<DanhSachDeNghiThanhToanChiPhiKCBNgoaiTru> DanhSachDeNghiThanhToanChiPhiKCBNgoaiTru()
        {
            return new List<DanhSachDeNghiThanhToanChiPhiKCBNgoaiTru>
            {
                new DanhSachDeNghiThanhToanChiPhiKCBNgoaiTru {
                    NoiDangKyKCBBanDau = NoiDangKyKCBBanDau.BenhNhanNoiTinhKhamChuaBenhBanDau,
                    KhamChuaBenh = EnumLyDoVaoVien.DungTuyen,
                    SoLanDenKham = 5,
                    ChiPhiXetNghiemKhongApDungTLTT = 153800,
                    ChiPhiCDHATDCNKhongApDungTLTT = 174700,
                    ChiPhiThuocKhongApDungTLTT = 207600,
                    ChiPhiMauKhongApDungTLTT = 0,
                    ChiPhiTTPTKhongApDungTLTT = 0,

                    ChiPhiVTYTKhongApDungTLTT = 0,
                    ChiPhiDVKTThanhToanTiLe = 0,
                    ChiPhiThuocThanhToanTiLe = 0,
                    ChiPhiVTYTThanhToanTiLe = 0,

                    TienKham = 152500,
                    VanChuyen = 0,

                    TongCongChiPhiBHXHThanhToan = 0,
                    ChiPhiNgoaiQuyDinhSuat = 0
                },
                new DanhSachDeNghiThanhToanChiPhiKCBNgoaiTru {
                    NoiDangKyKCBBanDau = NoiDangKyKCBBanDau.BenhNhanNoiTinhDen,
                    KhamChuaBenh = EnumLyDoVaoVien.DungTuyen,
                    SoLanDenKham = 12,
                    ChiPhiXetNghiemKhongApDungTLTT = 634100,
                    ChiPhiCDHATDCNKhongApDungTLTT = 1748200,
                    ChiPhiThuocKhongApDungTLTT = 631560,
                    ChiPhiMauKhongApDungTLTT = 0,
                    ChiPhiTTPTKhongApDungTLTT = 0,

                    ChiPhiVTYTKhongApDungTLTT = 0,
                    ChiPhiDVKTThanhToanTiLe = 0,
                    ChiPhiThuocThanhToanTiLe = 0,
                    ChiPhiVTYTThanhToanTiLe = 0,

                    TienKham = 375150,
                    VanChuyen = 0,
                    TongCongChiPhiBHXHThanhToan = 138152,
                    ChiPhiNgoaiQuyDinhSuat = 0
                },
                new DanhSachDeNghiThanhToanChiPhiKCBNgoaiTru {
                    NoiDangKyKCBBanDau = NoiDangKyKCBBanDau.BenhNhanNgoaiTinhDen,
                    KhamChuaBenh = EnumLyDoVaoVien.DungTuyen,
                    SoLanDenKham = 4,
                    ChiPhiXetNghiemKhongApDungTLTT = 170400,
                    ChiPhiCDHATDCNKhongApDungTLTT = 197100,
                    ChiPhiThuocKhongApDungTLTT = 309400,

                    ChiPhiMauKhongApDungTLTT = 0,
                    ChiPhiTTPTKhongApDungTLTT = 0,

                    ChiPhiVTYTKhongApDungTLTT = 0,
                    ChiPhiDVKTThanhToanTiLe = 0,
                    ChiPhiThuocThanhToanTiLe = 0,
                    ChiPhiVTYTThanhToanTiLe = 0,

                    TienKham = 122000,
                    VanChuyen = 0,
                    TongCongChiPhiBHXHThanhToan = 72015,
                    ChiPhiNgoaiQuyDinhSuat = 0
                },
            };
        }
        private List<DanhSachDeNghiThanhToanChiPhiKCBNoiTru> DanhSachDeNghiThanhToanChiPhiKCBNoiTru()
        {
            return new List<DanhSachDeNghiThanhToanChiPhiKCBNoiTru>
            {
                new DanhSachDeNghiThanhToanChiPhiKCBNoiTru {
                    NoiDangKyKCBBanDau = NoiDangKyKCBBanDau.BenhNhanNoiTinhKhamChuaBenhBanDau,
                    KhamChuaBenh = EnumLyDoVaoVien.DungTuyen,

                    SoLanDenKham = 5,
                    TongSoNgayDieuTri = 10,

                    ChiPhiXetNghiemKhongApDungTLTT = 153800,
                    ChiPhiCDHATDCNKhongApDungTLTT = 174700,
                    ChiPhiThuocKhongApDungTLTT = 207600,
                    ChiPhiMauKhongApDungTLTT = 0,
                    ChiPhiTTPTKhongApDungTLTT = 0,

                    ChiPhiVTYTKhongApDungTLTT = 0,
                    ChiPhiDVKTThanhToanTiLe = 0,
                    ChiPhiThuocThanhToanTiLe = 0,
                    ChiPhiVTYTThanhToanTiLe = 0,

                    TienKham = 152500,
                    VanChuyen = 0,
                    TienGiuong = 495500,

                    //NguoiBenhChiTra = 0,
                    ChiPhiNgoaiQuyDinhSuat = 0
                },
                new DanhSachDeNghiThanhToanChiPhiKCBNoiTru {
                    NoiDangKyKCBBanDau = NoiDangKyKCBBanDau.BenhNhanNoiTinhDen,
                    KhamChuaBenh = EnumLyDoVaoVien.DungTuyen,

                    SoLanDenKham = 12,
                    TongSoNgayDieuTri = 10,

                    ChiPhiXetNghiemKhongApDungTLTT = 634100,
                    ChiPhiCDHATDCNKhongApDungTLTT = 1748200,
                    ChiPhiThuocKhongApDungTLTT = 631560,
                    ChiPhiMauKhongApDungTLTT = 0,
                    ChiPhiTTPTKhongApDungTLTT = 0,

                    ChiPhiVTYTKhongApDungTLTT = 0,
                    ChiPhiDVKTThanhToanTiLe = 0,
                    ChiPhiThuocThanhToanTiLe = 0,
                    ChiPhiVTYTThanhToanTiLe = 0,

                    TienKham = 375150,
                    VanChuyen = 0,
                      TienGiuong = 495500,

                    //NguoiBenhChiTra = 138152,
                    ChiPhiNgoaiQuyDinhSuat = 0
                },
                new DanhSachDeNghiThanhToanChiPhiKCBNoiTru {
                    NoiDangKyKCBBanDau = NoiDangKyKCBBanDau.BenhNhanNgoaiTinhDen,
                    KhamChuaBenh = EnumLyDoVaoVien.DungTuyen,

                    SoLanDenKham = 12,
                    TongSoNgayDieuTri = 10,

                    ChiPhiXetNghiemKhongApDungTLTT = 170400,
                    ChiPhiCDHATDCNKhongApDungTLTT = 197100,
                    ChiPhiThuocKhongApDungTLTT = 309400,

                    ChiPhiMauKhongApDungTLTT = 0,
                    ChiPhiTTPTKhongApDungTLTT = 0,

                    ChiPhiVTYTKhongApDungTLTT = 0,
                    ChiPhiDVKTThanhToanTiLe = 0,
                    ChiPhiThuocThanhToanTiLe = 0,
                    ChiPhiVTYTThanhToanTiLe = 0,

                    TienKham = 122000,
                    VanChuyen = 0,
                      TienGiuong = 495500,

                    //NguoiBenhChiTra = 72015,
                    ChiPhiNgoaiQuyDinhSuat = 0
                },
            };
        }
        */
    }
}
