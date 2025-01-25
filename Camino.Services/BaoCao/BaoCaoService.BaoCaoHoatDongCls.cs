using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.BaoCao;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Helpers;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        public async Task<GridDataSource> GetDataBaoCaoHoatDongCLSMauThucTeForGridAsync(BaoCaoHoatDongClsQueryInfo queryInfo)
        {
            var cauHinhBaoCao = _cauHinhService.LoadSetting<CauHinhBaoCao>();
            var nhomDichVuBenhViens = _nhomDichVuBenhVienRepository.TableNoTracking.ToList();
            var dataHoatDongClsAll = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(o => o.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy &&
                ((o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem && o.PhienXetNghiemChiTiets.Any(ct => ct.ThoiDiemCoKetQua != null && ct.ThoiDiemCoKetQua >= queryInfo.FromDate && ct.ThoiDiemCoKetQua <= queryInfo.ToDate)) ||
                    ((o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh || o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang) && o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien &&
                        ((o.ThoiDiemHoanThanh != null && o.ThoiDiemHoanThanh >= queryInfo.FromDate && o.ThoiDiemHoanThanh <= queryInfo.ToDate) || (o.ThoiDiemHoanThanh == null && o.ThoiDiemThucHien != null && o.ThoiDiemThucHien >= queryInfo.FromDate && o.ThoiDiemThucHien <= queryInfo.ToDate)))
                ))
                .Select(o => new DataHoatDongCls()
                {
                    Id = o.Id,
                    TenDichVu = o.TenDichVu,
                    LoaiDichVuKyThuat = o.LoaiDichVuKyThuat,
                    SoLanYeuCau = o.SoLan,
                    SoLanThucHienXetNghiem = o.DichVuKyThuatBenhVien.SoLanThucHienXetNghiem,
                    NhomDichVuBenhVienId = o.NhomDichVuBenhVienId,
                    MaYeuCauTiepNhan = o.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    LoaiYeuCauTiepNhan = o.YeuCauTiepNhan.LoaiYeuCauTiepNhan,
                    PhienXetNghiemChiTietIds = o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem ? o.PhienXetNghiemChiTiets.Select(c=>c.Id).ToList() : new List<long>()
                }).ToList();

            var phienXetNghiemChiTietIds = dataHoatDongClsAll.SelectMany(o => o.PhienXetNghiemChiTietIds).ToList();

            //var phienXetNghiemChiTietIdCoKqs = _phienXetNghiemChiTietRepository.TableNoTracking
            //    .Where(o => phienXetNghiemChiTietIds.Contains(o.Id) && o.KetQuaXetNghiemChiTiets.Any(k => (k.GiaTriTuMay != null && k.GiaTriTuMay != "") || (k.GiaTriNhapTay != null && k.GiaTriNhapTay != "")))
            //    .Select(o => o.Id).ToList();

            var maxTake = 18000;

            List<long> phienXetNghiemChiTietIdCoKqs = new List<long>();
            for (int i = 0; i < phienXetNghiemChiTietIds.Count; i = i + maxTake)
            {
                var takeXuatKhoChiTietIds = phienXetNghiemChiTietIds.Skip(i).Take(maxTake).ToList();

                var ids = _phienXetNghiemChiTietRepository.TableNoTracking
                    .Where(o => takeXuatKhoChiTietIds.Contains(o.Id) && o.KetQuaXetNghiemChiTiets.Any(k => (k.GiaTriTuMay != null && k.GiaTriTuMay != "") || (k.GiaTriNhapTay != null && k.GiaTriNhapTay != "")))
                    .Select(o => o.Id).ToList();
                phienXetNghiemChiTietIdCoKqs.AddRange(ids);
            }

            //var ketQuaXetNghiemChiTiets = _ketQuaXetNghiemChiTietRepository.TableNoTracking
            //    .Where(o => phienXetNghiemChiTietIds.Contains(o.PhienXetNghiemChiTietId))
            //    .Select(o => new { o.PhienXetNghiemChiTietId, CoKq = (o.GiaTriTuMay != null && o.GiaTriTuMay != "") || (o.GiaTriNhapTay != null && o.GiaTriNhapTay != "") })
            //    .GroupBy(o => o.PhienXetNghiemChiTietId
            //        , o => o,
            //        (k, v) => new
            //        {
            //            PhienXetNghiemChiTietId = k,
            //            CoKq = v.Any(kq => kq.CoKq == true)
            //        }).ToList();

            //var phienXetNghiemChiTietIdCoKqs1 = ketQuaXetNghiemChiTiets.Where(o => o.CoKq).Select(o => o.PhienXetNghiemChiTietId).ToList();

            var dataHoatDongCls = dataHoatDongClsAll
                .Where(o => o.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.XetNghiem || 
                            (o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem && phienXetNghiemChiTietIdCoKqs.Intersect(o.PhienXetNghiemChiTietIds).Any()))
                .ToList();

            var dataNhomXNHuyetHoc = dataHoatDongCls.Where(o => o.NhomDichVuBenhVienId == cauHinhBaoCao.NhomXNHuyetHoc).ToList();
            var dataNhomXNHoaSinh = dataHoatDongCls.Where(o => o.NhomDichVuBenhVienId == cauHinhBaoCao.NhomXNHoaSinh).ToList();
            var dataNhomXNViSinhTruHiv = dataHoatDongCls.Where(o => o.NhomDichVuBenhVienId == cauHinhBaoCao.NhomXNViSinh && !o.TenDichVu.ToLower().Contains("hiv")).ToList();
            var dataNhomXNHiv = dataHoatDongCls.Where(o => o.NhomDichVuBenhVienId == cauHinhBaoCao.NhomXNViSinh && o.TenDichVu.ToLower().Contains("hiv")).ToList();
            var dataNhomXNNuocTieu = dataHoatDongCls.Where(o => o.NhomDichVuBenhVienId == cauHinhBaoCao.NhomXNNuocTieu).ToList();
            var dataNhomXNSinhHocPhanTu = dataHoatDongCls.Where(o => o.NhomDichVuBenhVienId == cauHinhBaoCao.NhomXNSinhHocPhanTu).ToList();
            var dataNhomXNTeBaoGiaiPhauBenh = dataHoatDongCls.Where(o => o.NhomDichVuBenhVienId == cauHinhBaoCao.NhomXNTeBaoGiaiPhauBenh).ToList();


            //var dataNhomChanDoanHinhAnh = dataHoatDongCls.Where(o => o.NhomDichVuBenhVienId == cauHinhBaoCao.NhomChanDoanHinhAnh).ToList();
            var dataNhomXQuang = dataHoatDongCls.Where(o => o.NhomDichVuBenhVienId == cauHinhBaoCao.NhomXQuangThuong || o.NhomDichVuBenhVienId == cauHinhBaoCao.NhomXQuangSoHoa).ToList();
            var dataNhomXQuangTruPanorama = dataHoatDongCls.Where(o => (o.NhomDichVuBenhVienId == cauHinhBaoCao.NhomXQuangThuong || o.NhomDichVuBenhVienId == cauHinhBaoCao.NhomXQuangSoHoa) && !o.TenDichVu.ToLower().Contains(cauHinhBaoCao.TenDichVuXQuangPanorama.ToLower())).ToList();
            var dataNhomXQuangPanorama = dataHoatDongCls.Where(o => (o.NhomDichVuBenhVienId == cauHinhBaoCao.NhomXQuangThuong || o.NhomDichVuBenhVienId == cauHinhBaoCao.NhomXQuangSoHoa) && o.TenDichVu.ToLower().Contains(cauHinhBaoCao.TenDichVuXQuangPanorama.ToLower())).ToList();
            var dataNhomSieuAm = dataHoatDongCls.Where(o => o.NhomDichVuBenhVienId == cauHinhBaoCao.NhomSieuAm).ToList();
            var dataNhomCTScannerTruConebeam = dataHoatDongCls.Where(o => o.NhomDichVuBenhVienId == cauHinhBaoCao.NhomCTScanner && !o.TenDichVu.ToLower().Contains(cauHinhBaoCao.TenDichVuCTConebeam.ToLower())).ToList();
            var dataNhomCTScannerConebeam = dataHoatDongCls.Where(o => o.NhomDichVuBenhVienId == cauHinhBaoCao.NhomCTScanner && o.TenDichVu.ToLower().Contains(cauHinhBaoCao.TenDichVuCTConebeam.ToLower())).ToList();
            var dataNhomMRI = dataHoatDongCls.Where(o => o.NhomDichVuBenhVienId == cauHinhBaoCao.NhomMRI).ToList();
            var dataNhomDoLoangXuong = dataHoatDongCls.Where(o => o.NhomDichVuBenhVienId == cauHinhBaoCao.NhomDoLoangXuong).ToList();

            var nhomChanDoanHinhAnhKhacIds = new List<long> { cauHinhBaoCao.NhomChanDoanHinhAnh };
            foreach(var nhomDichVu in nhomDichVuBenhViens.Where(o=>o.NhomDichVuBenhVienChaId == cauHinhBaoCao.NhomChanDoanHinhAnh))
            {
                if (nhomDichVu.Id != cauHinhBaoCao.NhomXQuangThuong && nhomDichVu.Id != cauHinhBaoCao.NhomXQuangSoHoa
                    && nhomDichVu.Id != cauHinhBaoCao.NhomSieuAm && nhomDichVu.Id != cauHinhBaoCao.NhomCTScanner
                    && nhomDichVu.Id != cauHinhBaoCao.NhomMRI && nhomDichVu.Id != cauHinhBaoCao.NhomDoLoangXuong
                    && nhomDichVu.Id != cauHinhBaoCao.NhomDienTim && nhomDichVu.Id != cauHinhBaoCao.NhomDienNao
                    && nhomDichVu.Id != cauHinhBaoCao.NhomNoiSoi && nhomDichVu.Id != cauHinhBaoCao.NhomNoiSoiTMH
                    && nhomDichVu.Id != cauHinhBaoCao.NhomDoHoHap)
                {
                    nhomChanDoanHinhAnhKhacIds.Add(nhomDichVu.Id);
                }
            }
            var dataNhomChanDoanHinhAnhKhac = dataHoatDongCls.Where(o => nhomChanDoanHinhAnhKhacIds.Contains(o.NhomDichVuBenhVienId)).ToList();

            //var dataNhomThamDoChucNang = dataHoatDongCls.Where(o => o.NhomDichVuBenhVienId == cauHinhBaoCao.NhomThamDoChucNang).ToList();
            var dataNhomDienTim = dataHoatDongCls.Where(o => o.NhomDichVuBenhVienId == cauHinhBaoCao.NhomDienTim).ToList();
            var dataNhomDienNao = dataHoatDongCls.Where(o => o.NhomDichVuBenhVienId == cauHinhBaoCao.NhomDienNao).ToList();
            var dataNhomNoiSoiTieuHoa = dataHoatDongCls.Where(o => o.NhomDichVuBenhVienId == cauHinhBaoCao.NhomNoiSoi).ToList();
            var dataNhomNoiSoiTMH = dataHoatDongCls.Where(o => o.NhomDichVuBenhVienId == cauHinhBaoCao.NhomNoiSoiTMH).ToList();
            var dataNhomDoHoHap = dataHoatDongCls.Where(o => o.NhomDichVuBenhVienId == cauHinhBaoCao.NhomDoHoHap).ToList();

            var nhomThamDoChucNangKhacIds = new List<long> { cauHinhBaoCao.NhomThamDoChucNang };
            foreach (var nhomDichVu in nhomDichVuBenhViens.Where(o => o.NhomDichVuBenhVienChaId == cauHinhBaoCao.NhomThamDoChucNang))
            {
                if (nhomDichVu.Id != cauHinhBaoCao.NhomXQuangThuong && nhomDichVu.Id != cauHinhBaoCao.NhomXQuangSoHoa
                    && nhomDichVu.Id != cauHinhBaoCao.NhomSieuAm && nhomDichVu.Id != cauHinhBaoCao.NhomCTScanner
                    && nhomDichVu.Id != cauHinhBaoCao.NhomMRI && nhomDichVu.Id != cauHinhBaoCao.NhomDoLoangXuong
                    && nhomDichVu.Id != cauHinhBaoCao.NhomDienTim && nhomDichVu.Id != cauHinhBaoCao.NhomDienNao
                    && nhomDichVu.Id != cauHinhBaoCao.NhomNoiSoi && nhomDichVu.Id != cauHinhBaoCao.NhomNoiSoiTMH
                    && nhomDichVu.Id != cauHinhBaoCao.NhomDoHoHap)
                {
                    nhomThamDoChucNangKhacIds.Add(nhomDichVu.Id);
                }
            }

            var dataNhomThamDoChucNangKhac = dataHoatDongCls.Where(o => nhomThamDoChucNangKhacIds.Contains(o.NhomDichVuBenhVienId)).ToList();

            var gridItemNhomXNHuyetHoc = new BaoCaoHoatDongClsGridVo
            {
                TenDichVu = "- Huyết học",
                DonVi = "Tiêu bản",
                NgoaiTru = dataNhomXNHuyetHoc.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                NoiTru = dataNhomXNHuyetHoc.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                SucKhoeKhac = dataNhomXNHuyetHoc.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
            };
            var gridItemNhomXNHoaSinh = new BaoCaoHoatDongClsGridVo
            {
                TenDichVu = "- Hóa sinh",
                DonVi = "Tiêu bản",
                NgoaiTru = dataNhomXNHoaSinh.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                NoiTru = dataNhomXNHoaSinh.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                SucKhoeKhac = dataNhomXNHoaSinh.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
            };
            var gridItemNhomXNViSinhTruHiv = new BaoCaoHoatDongClsGridVo
            {
                TenDichVu = "- Vi sinh",
                DonVi = "Tiêu bản",
                NgoaiTru = dataNhomXNViSinhTruHiv.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                NoiTru = dataNhomXNViSinhTruHiv.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                SucKhoeKhac = dataNhomXNViSinhTruHiv.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
            };
            var gridItemNhomXNHiv = new BaoCaoHoatDongClsGridVo
            {
                TenDichVu = "- HIV",
                DonVi = "Tiêu bản",
                NgoaiTru = dataNhomXNHiv.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                NoiTru = dataNhomXNHiv.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                SucKhoeKhac = dataNhomXNHiv.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
            };
            var gridItemViKhuan = new BaoCaoHoatDongClsGridVo
            {
                TenDichVu = "- Vi khuẩn",
                DonVi = "Tiêu bản",
                NgoaiTru = 0,
                NoiTru = 0,
                SucKhoeKhac = 0,
            };
            var gridItemNhomXNSinhHocPhanTu = new BaoCaoHoatDongClsGridVo
            {
                TenDichVu = "- Sinh học phân tử",
                DonVi = "Tiêu bản",
                NgoaiTru = dataNhomXNSinhHocPhanTu.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                NoiTru = dataNhomXNSinhHocPhanTu.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                SucKhoeKhac = dataNhomXNSinhHocPhanTu.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
            };
            var gridItemNhomXNNuocTieu = new BaoCaoHoatDongClsGridVo
            {
                TenDichVu = "- XN Nước tiểu",
                DonVi = "Tiêu bản",
                NgoaiTru = dataNhomXNNuocTieu.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                NoiTru = dataNhomXNNuocTieu.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                SucKhoeKhac = dataNhomXNNuocTieu.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
            };
            var gridItemNhomXNTeBaoGiaiPhauBenh = new BaoCaoHoatDongClsGridVo
            {
                TenDichVu = "- XN Tế bào + Giải phẫu bệnh",
                DonVi = "Tiêu bản",
                NgoaiTru = dataNhomXNTeBaoGiaiPhauBenh.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                NoiTru = dataNhomXNTeBaoGiaiPhauBenh.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                SucKhoeKhac = dataNhomXNTeBaoGiaiPhauBenh.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
            };
            var gridItemNhomXNKhac = new BaoCaoHoatDongClsGridVo
            {
                TenDichVu = "- Khác",
                DonVi = "Tiêu bản",
                NgoaiTru = 0,
                NoiTru = 0,
                SucKhoeKhac = 0,
            };

            var dataReturn = new List<BaoCaoHoatDongClsGridVo>();
            dataReturn.Add(new BaoCaoHoatDongClsGridVo
            {
                STT = "I",
                TenDichVu = "Các xét nghiệm:",
                DonVi = "",
                NgoaiTru = gridItemNhomXNHuyetHoc.NgoaiTru + gridItemNhomXNHoaSinh.NgoaiTru + gridItemNhomXNViSinhTruHiv.NgoaiTru + gridItemNhomXNHiv.NgoaiTru + gridItemNhomXNSinhHocPhanTu.NgoaiTru + gridItemNhomXNNuocTieu.NgoaiTru + gridItemNhomXNTeBaoGiaiPhauBenh.NgoaiTru,
                NoiTru = gridItemNhomXNHuyetHoc.NoiTru + gridItemNhomXNHoaSinh.NoiTru + gridItemNhomXNViSinhTruHiv.NoiTru + gridItemNhomXNHiv.NoiTru + gridItemNhomXNSinhHocPhanTu.NoiTru + gridItemNhomXNNuocTieu.NoiTru + gridItemNhomXNTeBaoGiaiPhauBenh.NoiTru,
                SucKhoeKhac = gridItemNhomXNHuyetHoc.SucKhoeKhac + gridItemNhomXNHoaSinh.SucKhoeKhac + gridItemNhomXNViSinhTruHiv.SucKhoeKhac + gridItemNhomXNHiv.SucKhoeKhac + gridItemNhomXNSinhHocPhanTu.SucKhoeKhac + gridItemNhomXNNuocTieu.SucKhoeKhac + gridItemNhomXNTeBaoGiaiPhauBenh.SucKhoeKhac,
                ToDam = true
            });
            dataReturn.Add(gridItemNhomXNHuyetHoc);
            dataReturn.Add(gridItemNhomXNHoaSinh);
            dataReturn.Add(gridItemNhomXNViSinhTruHiv);
            dataReturn.Add(gridItemNhomXNHiv);
            dataReturn.Add(gridItemViKhuan);
            dataReturn.Add(gridItemNhomXNSinhHocPhanTu);
            dataReturn.Add(gridItemNhomXNNuocTieu);
            dataReturn.Add(gridItemNhomXNTeBaoGiaiPhauBenh);
            dataReturn.Add(gridItemNhomXNKhac);

            var gridItemNhomXQuangTruPanorama = new BaoCaoHoatDongClsGridVo
            {
                STT = "",
                TenDichVu = "Xquang thường quy + C-arm",
                DonVi = "Lần",
                NgoaiTru = dataNhomXQuangTruPanorama.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                NoiTru = dataNhomXQuangTruPanorama.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                SucKhoeKhac = dataNhomXQuangTruPanorama.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
            };
            var gridItemNhomXQuangPanorama = new BaoCaoHoatDongClsGridVo
            {
                STT = "",
                TenDichVu = "X-quang (Panorama)",
                DonVi = "Lần",
                NgoaiTru = dataNhomXQuangPanorama.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                NoiTru = dataNhomXQuangPanorama.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                SucKhoeKhac = dataNhomXQuangPanorama.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
            };
            var gridItemNhomXQuangKhac = new BaoCaoHoatDongClsGridVo
            {
                STT = "",
                TenDichVu = "Khác",
                DonVi = "Lần",
                NgoaiTru = 0,
                NoiTru = 0,
                SucKhoeKhac = 0,
            };
            var gridItemNhomXQuang = new BaoCaoHoatDongClsGridVo
            {
                STT = "1",
                TenDichVu = "Số lần chụp XQ",
                DonVi = "Lần",
                NgoaiTru = gridItemNhomXQuangTruPanorama.NgoaiTru + gridItemNhomXQuangPanorama.NgoaiTru,
                NoiTru = gridItemNhomXQuangTruPanorama.NoiTru + gridItemNhomXQuangPanorama.NoiTru,
                SucKhoeKhac = gridItemNhomXQuangTruPanorama.SucKhoeKhac + gridItemNhomXQuangPanorama.SucKhoeKhac,
            };
            var gridItemNhomSieuAm = new BaoCaoHoatDongClsGridVo
            {
                STT = "2",
                TenDichVu = "TS lần siêu âm",
                DonVi = "Lần",
                NgoaiTru = dataNhomSieuAm.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                NoiTru = dataNhomSieuAm.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                SucKhoeKhac = dataNhomSieuAm.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
            };
            var gridItemNhomCTScannerTruConebeam = new BaoCaoHoatDongClsGridVo
            {
                STT = "",
                TenDichVu = "CLVT",
                DonVi = "Lần",
                NgoaiTru = dataNhomCTScannerTruConebeam.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                NoiTru = dataNhomCTScannerTruConebeam.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                SucKhoeKhac = dataNhomCTScannerTruConebeam.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
            };
            var gridItemNhomCTScannerConebeam = new BaoCaoHoatDongClsGridVo
            {
                STT = "",
                TenDichVu = "CT Cone bean",
                DonVi = "Lần",
                NgoaiTru = dataNhomCTScannerConebeam.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                NoiTru = dataNhomCTScannerConebeam.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                SucKhoeKhac = dataNhomCTScannerConebeam.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
            };
            var gridItemNhomCTScanner = new BaoCaoHoatDongClsGridVo
            {
                STT = "3",
                TenDichVu = "CT - Scanner",
                DonVi = "Lần",
                NgoaiTru = gridItemNhomCTScannerTruConebeam.NgoaiTru + gridItemNhomCTScannerConebeam.NgoaiTru,
                NoiTru = gridItemNhomCTScannerTruConebeam.NoiTru + gridItemNhomCTScannerConebeam.NoiTru,
                SucKhoeKhac = gridItemNhomCTScannerTruConebeam.SucKhoeKhac + gridItemNhomCTScannerConebeam.SucKhoeKhac,
            };
            var gridItemNhomMRI = new BaoCaoHoatDongClsGridVo
            {
                STT = "4",
                TenDichVu = "Cộng hưởng từ",
                DonVi = "Lần",
                NgoaiTru = dataNhomMRI.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                NoiTru = dataNhomMRI.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                SucKhoeKhac = dataNhomMRI.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
            };
            var gridItemNhomDoLoangXuong = new BaoCaoHoatDongClsGridVo
            {
                STT = "5",
                TenDichVu = "Đo loãng xương",
                DonVi = "Lần",
                NgoaiTru = dataNhomDoLoangXuong.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                NoiTru = dataNhomDoLoangXuong.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                SucKhoeKhac = dataNhomDoLoangXuong.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
            };
            var gridItemHAKhac = new BaoCaoHoatDongClsGridVo
            {
                STT = "6",
                TenDichVu = "Khác",
                DonVi = "Lần",
                NgoaiTru = dataNhomChanDoanHinhAnhKhac.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                NoiTru = dataNhomChanDoanHinhAnhKhac.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                SucKhoeKhac = dataNhomChanDoanHinhAnhKhac.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
            };


            dataReturn.Add(new BaoCaoHoatDongClsGridVo
            {
                STT = "II",
                TenDichVu = "Chẩn đoán hình ảnh",
                DonVi = "",
                NgoaiTru = gridItemNhomXQuang.NgoaiTru + gridItemNhomSieuAm.NgoaiTru + gridItemNhomCTScanner.NgoaiTru + gridItemNhomMRI.NgoaiTru + gridItemNhomDoLoangXuong.NgoaiTru + gridItemHAKhac.NgoaiTru,
                NoiTru = gridItemNhomXQuang.NoiTru + gridItemNhomSieuAm.NoiTru + gridItemNhomCTScanner.NoiTru + gridItemNhomMRI.NoiTru + gridItemNhomDoLoangXuong.NoiTru + gridItemHAKhac.NoiTru,
                SucKhoeKhac = gridItemNhomXQuang.SucKhoeKhac + gridItemNhomSieuAm.SucKhoeKhac + gridItemNhomCTScanner.SucKhoeKhac + gridItemNhomMRI.SucKhoeKhac + gridItemNhomDoLoangXuong.SucKhoeKhac + gridItemHAKhac.SucKhoeKhac,
                ToDam = true
            });
            var groupNguoiChupXquang = dataNhomXQuang.GroupBy(o => o.MaYeuCauTiepNhan).ToList();
            dataReturn.Add(new BaoCaoHoatDongClsGridVo
            {
                STT = "",
                TenDichVu = "Số người chụp XQ",
                DonVi = "Người",
                NgoaiTru = groupNguoiChupXquang.Count(o => o.Any(x => x.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru)),
                NoiTru = groupNguoiChupXquang.Count(o => o.All(x => x.LoaiYeuCauTiepNhan != Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru) && o.Any(x => x.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru)),
                SucKhoeKhac = groupNguoiChupXquang.Count(o => o.Any(x => x.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe)),
            });
            
            dataReturn.Add(gridItemNhomXQuang);
            dataReturn.Add(gridItemNhomXQuangTruPanorama);
            dataReturn.Add(gridItemNhomXQuangPanorama);
            dataReturn.Add(gridItemNhomXQuangKhac);
            dataReturn.Add(gridItemNhomSieuAm);
            dataReturn.Add(gridItemNhomCTScanner);
            dataReturn.Add(gridItemNhomCTScannerTruConebeam);
            dataReturn.Add(gridItemNhomCTScannerConebeam);
            dataReturn.Add(gridItemNhomMRI);
            dataReturn.Add(gridItemNhomDoLoangXuong);
            dataReturn.Add(gridItemHAKhac);

            var gridItemNhomDienTim = new BaoCaoHoatDongClsGridVo
            {
                TenDichVu = "- Điện tim",
                DonVi = "Lần",
                NgoaiTru = dataNhomDienTim.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                NoiTru = dataNhomDienTim.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                SucKhoeKhac = dataNhomDienTim.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
            };
            var gridItemNhomDienNao = new BaoCaoHoatDongClsGridVo
            {
                TenDichVu = "- Điện não",
                DonVi = "Lần",
                NgoaiTru = dataNhomDienNao.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                NoiTru = dataNhomDienNao.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                SucKhoeKhac = dataNhomDienNao.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
            };
            var gridItemNhomNoiSoiTieuHoa = new BaoCaoHoatDongClsGridVo
            {
                TenDichVu = "T.đó: Tiêu hóa",
                DonVi = "Lần",
                NgoaiTru = dataNhomNoiSoiTieuHoa.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                NoiTru = dataNhomNoiSoiTieuHoa.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                SucKhoeKhac = dataNhomNoiSoiTieuHoa.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
            };
            var gridItemNhomNoiSoiTMH = new BaoCaoHoatDongClsGridVo
            {
                TenDichVu = "      TMH",
                DonVi = "Lần",
                NgoaiTru = dataNhomNoiSoiTMH.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                NoiTru = dataNhomNoiSoiTMH.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                SucKhoeKhac = dataNhomNoiSoiTMH.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
            };
            var gridItemNhomNoiSoiSan = new BaoCaoHoatDongClsGridVo
            {
                TenDichVu = "      Sản ( soi CTC)",
                DonVi = "Lần",
                NgoaiTru = 0,
                NoiTru = 0,
                SucKhoeKhac = 0,
            };
            var gridItemNhomNoiSoi = new BaoCaoHoatDongClsGridVo
            {
                TenDichVu = "- Nội soi - T.số:",
                DonVi = "Lần",
                NgoaiTru = gridItemNhomNoiSoiTieuHoa.NgoaiTru + gridItemNhomNoiSoiTMH.NgoaiTru,
                NoiTru = gridItemNhomNoiSoiTieuHoa.NoiTru + gridItemNhomNoiSoiTMH.NoiTru,
                SucKhoeKhac = gridItemNhomNoiSoiTieuHoa.SucKhoeKhac + gridItemNhomNoiSoiTMH.SucKhoeKhac,
            };
            var gridItemNhomDoHoHap = new BaoCaoHoatDongClsGridVo
            {
                TenDichVu = "- Chức năng hô hấp",
                DonVi = "Lần",
                NgoaiTru = dataNhomDoHoHap.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                NoiTru = dataNhomDoHoHap.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                SucKhoeKhac = dataNhomDoHoHap.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
            };
            var gridItemKhac = new BaoCaoHoatDongClsGridVo
            {
                TenDichVu = "- Khác",
                DonVi = "Lần",
                NgoaiTru = dataNhomThamDoChucNangKhac.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                NoiTru = dataNhomThamDoChucNangKhac.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                SucKhoeKhac = dataNhomThamDoChucNangKhac.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
            };

            dataReturn.Add(new BaoCaoHoatDongClsGridVo
            {
                STT = "III",
                TenDichVu = "Thăm dò chức năng",
                DonVi = "",
                NgoaiTru = gridItemNhomDienTim.NgoaiTru + gridItemNhomDienNao.NgoaiTru + gridItemNhomNoiSoi.NgoaiTru + gridItemNhomDoHoHap.NgoaiTru + gridItemKhac.NgoaiTru,
                NoiTru = gridItemNhomDienTim.NoiTru + gridItemNhomDienNao.NoiTru + gridItemNhomNoiSoi.NoiTru + gridItemNhomDoHoHap.NoiTru + gridItemKhac.NoiTru,
                SucKhoeKhac = gridItemNhomDienTim.SucKhoeKhac + gridItemNhomDienNao.SucKhoeKhac + gridItemNhomNoiSoi.SucKhoeKhac + gridItemNhomDoHoHap.SucKhoeKhac + gridItemKhac.SucKhoeKhac,
                ToDam = true
            });
            dataReturn.Add(gridItemNhomDienTim);
            dataReturn.Add(gridItemNhomDienNao);
            dataReturn.Add(gridItemNhomNoiSoi);
            dataReturn.Add(gridItemNhomNoiSoiTieuHoa);
            dataReturn.Add(gridItemNhomNoiSoiSan);
            dataReturn.Add(gridItemNhomNoiSoiTMH);
            dataReturn.Add(gridItemNhomDoHoHap);
            dataReturn.Add(gridItemKhac);

            var slMau = _yeuCauTruyenMauRepository.TableNoTracking
                .Where(o => o.XuatKhoMauChiTietId != null && o.XuatKhoMauChiTiet.XuatKhoMau.NgayXuat >= queryInfo.FromDate && o.XuatKhoMauChiTiet.XuatKhoMau.NgayXuat < queryInfo.ToDate)
                .Select(o => o.XuatKhoMauChiTiet.NhapKhoMauChiTiet.TheTich).DefaultIfEmpty(0).Sum();

            dataReturn.Add(new BaoCaoHoatDongClsGridVo
            {
                STT = "IV",
                TenDichVu = "TS lượng máu SD tại BV (ml)",
                DonVi = "",
                NgoaiTru = 0,
                NoiTru = (int)slMau,
                SucKhoeKhac = 0,
                ToDam = true
            });

            dataReturn.Add(new BaoCaoHoatDongClsGridVo
            {
                STT = "",
                TenDichVu = "SL máu từ TT huyết học",
                DonVi = "ml",
                NgoaiTru = 0,
                NoiTru = (int)slMau,
                SucKhoeKhac = 0,
            });
            return new GridDataSource
            {
                Data = dataReturn.ToArray(),
                TotalRowCount = dataReturn.Count()
            };
        }
       
        public async Task<GridDataSource> GetDataBaoCaoHoatDongCLSMauCucQuanLyForGridAsync(BaoCaoHoatDongClsQueryInfo queryInfo)
        {
            var cauHinhBaoCao = _cauHinhService.LoadSetting<CauHinhBaoCao>();
            var nhomDichVuBenhViens = _nhomDichVuBenhVienRepository.TableNoTracking.ToList();
            var dataHoatDongClsAll = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(o => o.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy &&
                ((o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem && o.PhienXetNghiemChiTiets.Any(ct => ct.ThoiDiemCoKetQua != null && ct.ThoiDiemCoKetQua >= queryInfo.FromDate && ct.ThoiDiemCoKetQua <= queryInfo.ToDate)) ||
                    ((o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh || o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang) && o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien &&
                        ((o.ThoiDiemHoanThanh != null && o.ThoiDiemHoanThanh >= queryInfo.FromDate && o.ThoiDiemHoanThanh <= queryInfo.ToDate) || (o.ThoiDiemHoanThanh == null && o.ThoiDiemThucHien != null && o.ThoiDiemThucHien >= queryInfo.FromDate && o.ThoiDiemThucHien <= queryInfo.ToDate)))
                ))
                .Select(o => new DataHoatDongCls()
                {
                    Id = o.Id,
                    TenDichVu = o.TenDichVu,
                    LoaiDichVuKyThuat = o.LoaiDichVuKyThuat,
                    SoLanYeuCau = o.SoLan,
                    SoLanThucHienXetNghiem = o.DichVuKyThuatBenhVien.SoLanThucHienXetNghiem,
                    NhomDichVuBenhVienId = o.NhomDichVuBenhVienId,
                    MaYeuCauTiepNhan = o.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    LoaiYeuCauTiepNhan = o.YeuCauTiepNhan.LoaiYeuCauTiepNhan,
                    PhienXetNghiemChiTietIds = o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem ? o.PhienXetNghiemChiTiets.Select(c => c.Id).ToList() : new List<long>()
                }).ToList();

            var phienXetNghiemChiTietIds = dataHoatDongClsAll.SelectMany(o => o.PhienXetNghiemChiTietIds).ToList();

            //var phienXetNghiemChiTietIdCoKqs = _phienXetNghiemChiTietRepository.TableNoTracking
            //    .Where(o => phienXetNghiemChiTietIds.Contains(o.Id) && o.KetQuaXetNghiemChiTiets.Any(k => (k.GiaTriTuMay != null && k.GiaTriTuMay != "") || (k.GiaTriNhapTay != null && k.GiaTriNhapTay != "")))
            //    .Select(o => o.Id).ToList();

            var maxTake = 18000;

            List<long> phienXetNghiemChiTietIdCoKqs = new List<long>();
            for (int i = 0; i < phienXetNghiemChiTietIds.Count; i = i + maxTake)
            {
                var takeXuatKhoChiTietIds = phienXetNghiemChiTietIds.Skip(i).Take(maxTake).ToList();

                var ids = _phienXetNghiemChiTietRepository.TableNoTracking
                    .Where(o => takeXuatKhoChiTietIds.Contains(o.Id) && o.KetQuaXetNghiemChiTiets.Any(k => (k.GiaTriTuMay != null && k.GiaTriTuMay != "") || (k.GiaTriNhapTay != null && k.GiaTriNhapTay != "")))
                    .Select(o => o.Id).ToList();
                phienXetNghiemChiTietIdCoKqs.AddRange(ids);
            }

            var dataHoatDongCls = dataHoatDongClsAll
                .Where(o => o.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.XetNghiem ||
                            (o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem && phienXetNghiemChiTietIdCoKqs.Intersect(o.PhienXetNghiemChiTietIds).Any()))
                .ToList();

            var dataNhomXNHuyetHoc = dataHoatDongCls.Where(o => o.NhomDichVuBenhVienId == cauHinhBaoCao.NhomXNHuyetHoc).ToList();
            var dataNhomXNHoaSinhNuocTieu = dataHoatDongCls.Where(o => o.NhomDichVuBenhVienId == cauHinhBaoCao.NhomXNHoaSinh || o.NhomDichVuBenhVienId == cauHinhBaoCao.NhomXNNuocTieu).ToList();
            var dataNhomXNViSinhTruHiv = dataHoatDongCls.Where(o => o.NhomDichVuBenhVienId == cauHinhBaoCao.NhomXNViSinh && !o.TenDichVu.ToLower().Contains("hiv")).ToList();
            var dataNhomXNHiv = dataHoatDongCls.Where(o => o.NhomDichVuBenhVienId == cauHinhBaoCao.NhomXNViSinh && o.TenDichVu.ToLower().Contains("hiv")).ToList();
            //var dataNhomXNNuocTieu = dataHoatDongCls.Where(o => o.NhomDichVuBenhVienId == cauHinhBaoCao.NhomXNNuocTieu).ToList();
            var dataNhomXNKhac = dataHoatDongCls.Where(o => o.NhomDichVuBenhVienId == cauHinhBaoCao.NhomXNSinhHocPhanTu || o.NhomDichVuBenhVienId == cauHinhBaoCao.NhomXNTeBaoGiaiPhauBenh).ToList();
            //var dataNhomXNTeBaoGiaiPhauBenh = dataHoatDongCls.Where(o => o.NhomDichVuBenhVienId == cauHinhBaoCao.NhomXNTeBaoGiaiPhauBenh).ToList();

            //var dataNhomChanDoanHinhAnh = dataHoatDongCls.Where(o => o.NhomDichVuBenhVienId == cauHinhBaoCao.NhomChanDoanHinhAnh).ToList();
            var dataNhomXQuang = dataHoatDongCls.Where(o => o.NhomDichVuBenhVienId == cauHinhBaoCao.NhomXQuangThuong || o.NhomDichVuBenhVienId == cauHinhBaoCao.NhomXQuangSoHoa).ToList();
            var dataNhomSieuAm = dataHoatDongCls.Where(o => o.NhomDichVuBenhVienId == cauHinhBaoCao.NhomSieuAm).ToList();
            //var dataNhomXQuangSoHoa = dataHoatDongCls.Where(o => o.NhomDichVuBenhVienId == cauHinhBaoCao.NhomXQuangSoHoa).ToList();
            var dataNhomCTScanner = dataHoatDongCls.Where(o => o.NhomDichVuBenhVienId == cauHinhBaoCao.NhomCTScanner).ToList();
            var dataNhomMRI = dataHoatDongCls.Where(o => o.NhomDichVuBenhVienId == cauHinhBaoCao.NhomMRI).ToList();
            var dataNhomDoLoangXuong = dataHoatDongCls.Where(o => o.NhomDichVuBenhVienId == cauHinhBaoCao.NhomDoLoangXuong).ToList();

            var nhomChanDoanHinhAnhKhacIds = new List<long> { cauHinhBaoCao.NhomChanDoanHinhAnh };
            foreach (var nhomDichVu in nhomDichVuBenhViens.Where(o => o.NhomDichVuBenhVienChaId == cauHinhBaoCao.NhomChanDoanHinhAnh))
            {
                if (nhomDichVu.Id != cauHinhBaoCao.NhomXQuangThuong && nhomDichVu.Id != cauHinhBaoCao.NhomXQuangSoHoa
                    && nhomDichVu.Id != cauHinhBaoCao.NhomSieuAm && nhomDichVu.Id != cauHinhBaoCao.NhomCTScanner
                    && nhomDichVu.Id != cauHinhBaoCao.NhomMRI && nhomDichVu.Id != cauHinhBaoCao.NhomDoLoangXuong
                    && nhomDichVu.Id != cauHinhBaoCao.NhomDienTim && nhomDichVu.Id != cauHinhBaoCao.NhomDienNao
                    && nhomDichVu.Id != cauHinhBaoCao.NhomNoiSoi && nhomDichVu.Id != cauHinhBaoCao.NhomNoiSoiTMH
                    && nhomDichVu.Id != cauHinhBaoCao.NhomDoHoHap)
                {
                    nhomChanDoanHinhAnhKhacIds.Add(nhomDichVu.Id);
                }
            }
            var dataNhomChanDoanHinhAnhKhac = dataHoatDongCls.Where(o => nhomChanDoanHinhAnhKhacIds.Contains(o.NhomDichVuBenhVienId)).ToList();

            //var dataNhomThamDoChucNang = dataHoatDongCls.Where(o => o.NhomDichVuBenhVienId == cauHinhBaoCao.NhomThamDoChucNang).ToList();
            var dataNhomDienTim = dataHoatDongCls.Where(o => o.NhomDichVuBenhVienId == cauHinhBaoCao.NhomDienTim).ToList();
            var dataNhomDienNao = dataHoatDongCls.Where(o => o.NhomDichVuBenhVienId == cauHinhBaoCao.NhomDienNao).ToList();
            var dataNhomNoiSoi = dataHoatDongCls.Where(o => o.NhomDichVuBenhVienId == cauHinhBaoCao.NhomNoiSoi || o.NhomDichVuBenhVienId == cauHinhBaoCao.NhomNoiSoiTMH).ToList();
            //var dataNhomNoiSoiTMH = dataHoatDongCls.Where(o => o.NhomDichVuBenhVienId == cauHinhBaoCao.NhomNoiSoiTMH).ToList();
            var dataNhomDoHoHap = dataHoatDongCls.Where(o => o.NhomDichVuBenhVienId == cauHinhBaoCao.NhomDoHoHap).ToList();

            var nhomThamDoChucNangKhacIds = new List<long> { cauHinhBaoCao.NhomThamDoChucNang };
            foreach (var nhomDichVu in nhomDichVuBenhViens.Where(o => o.NhomDichVuBenhVienChaId == cauHinhBaoCao.NhomThamDoChucNang))
            {
                if (nhomDichVu.Id != cauHinhBaoCao.NhomXQuangThuong && nhomDichVu.Id != cauHinhBaoCao.NhomXQuangSoHoa
                    && nhomDichVu.Id != cauHinhBaoCao.NhomSieuAm && nhomDichVu.Id != cauHinhBaoCao.NhomCTScanner
                    && nhomDichVu.Id != cauHinhBaoCao.NhomMRI && nhomDichVu.Id != cauHinhBaoCao.NhomDoLoangXuong
                    && nhomDichVu.Id != cauHinhBaoCao.NhomDienTim && nhomDichVu.Id != cauHinhBaoCao.NhomDienNao
                    && nhomDichVu.Id != cauHinhBaoCao.NhomNoiSoi && nhomDichVu.Id != cauHinhBaoCao.NhomNoiSoiTMH
                    && nhomDichVu.Id != cauHinhBaoCao.NhomDoHoHap)
                {
                    nhomThamDoChucNangKhacIds.Add(nhomDichVu.Id);
                }
            }

            var dataNhomThamDoChucNangKhac = dataHoatDongCls.Where(o => nhomThamDoChucNangKhacIds.Contains(o.NhomDichVuBenhVienId)).ToList();

            var gridItemNhomXNHuyetHoc = new BaoCaoHoatDongClsGridVo
            {
                TenDichVu = "- Huyết học",
                DonVi = "Tiêu bản",
                NgoaiTru = dataNhomXNHuyetHoc.Where(o=>o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru).Select(o=>o.SoLan).DefaultIfEmpty().Sum(),
                NoiTru = dataNhomXNHuyetHoc.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                SucKhoeKhac = dataNhomXNHuyetHoc.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
            };
            var gridItemNhomXNHoaSinhNuocTieu = new BaoCaoHoatDongClsGridVo
            {
                TenDichVu = "- Hóa sinh",
                DonVi = "Tiêu bản",
                NgoaiTru = dataNhomXNHoaSinhNuocTieu.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                NoiTru = dataNhomXNHoaSinhNuocTieu.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                SucKhoeKhac = dataNhomXNHoaSinhNuocTieu.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
            };
            var gridItemNhomXNViSinhTruHiv = new BaoCaoHoatDongClsGridVo
            {
                TenDichVu = "- Vi sinh",
                DonVi = "Tiêu bản",
                NgoaiTru = dataNhomXNViSinhTruHiv.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                NoiTru = dataNhomXNViSinhTruHiv.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                SucKhoeKhac = dataNhomXNViSinhTruHiv.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
            };
            var gridItemNhomXNHiv = new BaoCaoHoatDongClsGridVo
            {
                TenDichVu = "- HIV",
                DonVi = "Tiêu bản",
                NgoaiTru = dataNhomXNHiv.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                NoiTru = dataNhomXNHiv.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                SucKhoeKhac = dataNhomXNHiv.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
            };
            var gridNhomXNKhac = new BaoCaoHoatDongClsGridVo
            {
                TenDichVu = "- Khác",
                DonVi = "Tiêu bản",
                NgoaiTru = dataNhomXNKhac.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                NoiTru = dataNhomXNKhac.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                SucKhoeKhac = dataNhomXNKhac.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
            };
            var gridItemViKhuan = new BaoCaoHoatDongClsGridVo
            {
                TenDichVu = "- Vi khuẩn",
                DonVi = "Tiêu bản",
                NgoaiTru = 0,
                NoiTru = 0,
                SucKhoeKhac = 0,
            };

            var dataReturn = new List<BaoCaoHoatDongClsGridVo>();
            dataReturn.Add(new BaoCaoHoatDongClsGridVo
            {
                STT = "I",
                TenDichVu = "Các xét nghiệm:",
                DonVi = "",
                NgoaiTru = gridItemNhomXNHuyetHoc.NgoaiTru + gridItemNhomXNHoaSinhNuocTieu.NgoaiTru + gridItemNhomXNViSinhTruHiv.NgoaiTru + gridItemNhomXNHiv.NgoaiTru + gridNhomXNKhac.NgoaiTru + gridItemViKhuan.NgoaiTru,
                NoiTru = gridItemNhomXNHuyetHoc.NoiTru + gridItemNhomXNHoaSinhNuocTieu.NoiTru + gridItemNhomXNViSinhTruHiv.NoiTru + gridItemNhomXNHiv.NoiTru + gridNhomXNKhac.NoiTru + gridItemViKhuan.NoiTru,
                SucKhoeKhac = gridItemNhomXNHuyetHoc.SucKhoeKhac + gridItemNhomXNHoaSinhNuocTieu.SucKhoeKhac + gridItemNhomXNViSinhTruHiv.SucKhoeKhac + gridItemNhomXNHiv.SucKhoeKhac + gridNhomXNKhac.SucKhoeKhac + gridItemViKhuan.SucKhoeKhac,
                ToDam = true
            });
            dataReturn.Add(gridItemNhomXNHuyetHoc);
            dataReturn.Add(gridItemNhomXNHoaSinhNuocTieu);
            dataReturn.Add(gridItemNhomXNViSinhTruHiv);
            dataReturn.Add(gridItemNhomXNHiv);
            dataReturn.Add(gridNhomXNKhac);
            dataReturn.Add(gridItemViKhuan);

            var gridItemNhomXQuang = new BaoCaoHoatDongClsGridVo
            {
                STT = "1",
                TenDichVu = "Số lần chụp XQ",
                DonVi = "Lần",
                NgoaiTru = dataNhomXQuang.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                NoiTru = dataNhomXQuang.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                SucKhoeKhac = dataNhomXQuang.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
            };
            var gridItemNhomSieuAm = new BaoCaoHoatDongClsGridVo
            {
                STT = "2",
                TenDichVu = "Số lần siêu âm",
                DonVi = "Lần",
                NgoaiTru = dataNhomSieuAm.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                NoiTru = dataNhomSieuAm.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                SucKhoeKhac = dataNhomSieuAm.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
            };
            var gridItemNhomCTScanner = new BaoCaoHoatDongClsGridVo
            {
                STT = "3",
                TenDichVu = "CT - Scanner",
                DonVi = "Lần",
                NgoaiTru = dataNhomCTScanner.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                NoiTru = dataNhomCTScanner.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                SucKhoeKhac = dataNhomCTScanner.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
            };
            var gridItemNhomMRI = new BaoCaoHoatDongClsGridVo
            {
                STT = "4",
                TenDichVu = "Cộng hưởng từ",
                DonVi = "Lần",
                NgoaiTru = dataNhomMRI.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                NoiTru = dataNhomMRI.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                SucKhoeKhac = dataNhomMRI.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
            };
            var gridItemNhomDoLoangXuong = new BaoCaoHoatDongClsGridVo
            {
                STT = "5",
                TenDichVu = "Đo loãng xương",
                DonVi = "Lần",
                NgoaiTru = dataNhomDoLoangXuong.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                NoiTru = dataNhomDoLoangXuong.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                SucKhoeKhac = dataNhomDoLoangXuong.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
            };
            var gridItemHAKhac = new BaoCaoHoatDongClsGridVo
            {
                STT = "6",
                TenDichVu = "Khác",
                DonVi = "Lần",
                NgoaiTru = dataNhomChanDoanHinhAnhKhac.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                NoiTru = dataNhomChanDoanHinhAnhKhac.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                SucKhoeKhac = dataNhomChanDoanHinhAnhKhac.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
            };

            
            dataReturn.Add(new BaoCaoHoatDongClsGridVo
            {
                STT = "II",
                TenDichVu = "Chẩn đoán hình ảnh",
                DonVi = "",
                NgoaiTru = gridItemNhomXQuang.NgoaiTru + gridItemNhomSieuAm.NgoaiTru + gridItemNhomCTScanner.NgoaiTru + gridItemNhomMRI.NgoaiTru + gridItemNhomDoLoangXuong.NgoaiTru + gridItemHAKhac.NgoaiTru,
                NoiTru = gridItemNhomXQuang.NoiTru + gridItemNhomSieuAm.NoiTru + gridItemNhomCTScanner.NoiTru + gridItemNhomMRI.NoiTru + gridItemNhomDoLoangXuong.NoiTru + gridItemHAKhac.NoiTru,
                SucKhoeKhac = gridItemNhomXQuang.SucKhoeKhac + gridItemNhomSieuAm.SucKhoeKhac + gridItemNhomCTScanner.SucKhoeKhac + gridItemNhomMRI.SucKhoeKhac + gridItemNhomDoLoangXuong.SucKhoeKhac + gridItemHAKhac.SucKhoeKhac,
                ToDam = true
            });
            var groupNguoiChupXquang = dataNhomXQuang.GroupBy(o => o.MaYeuCauTiepNhan).ToList();
            dataReturn.Add(new BaoCaoHoatDongClsGridVo
            {
                STT = "",
                TenDichVu = "Số người chụp XQ",
                DonVi = "Người",
                NgoaiTru = groupNguoiChupXquang.Count(o=> o.Any(x=>x.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru)),
                NoiTru = groupNguoiChupXquang.Count(o => o.All(x => x.LoaiYeuCauTiepNhan != Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru) && o.Any(x => x.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru)),
                SucKhoeKhac = groupNguoiChupXquang.Count(o => o.Any(x => x.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe)),
            });
            dataReturn.Add(gridItemNhomXQuang);
            dataReturn.Add(gridItemNhomSieuAm);
            dataReturn.Add(gridItemNhomCTScanner);
            dataReturn.Add(gridItemNhomMRI);
            dataReturn.Add(gridItemNhomDoLoangXuong);
            dataReturn.Add(gridItemHAKhac);

            var gridItemNhomDienTim = new BaoCaoHoatDongClsGridVo
            {
                TenDichVu = "- Điện tim",
                DonVi = "Lần",
                NgoaiTru = dataNhomDienTim.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                NoiTru = dataNhomDienTim.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                SucKhoeKhac = dataNhomDienTim.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
            };
            var gridItemNhomDienNao = new BaoCaoHoatDongClsGridVo
            {
                TenDichVu = "- Điện não",
                DonVi = "Lần",
                NgoaiTru = dataNhomDienNao.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                NoiTru = dataNhomDienNao.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                SucKhoeKhac = dataNhomDienNao.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
            };
            var gridItemNhomNoiSoi = new BaoCaoHoatDongClsGridVo
            {
                TenDichVu = "- Nội soi",
                DonVi = "Lần",
                NgoaiTru = dataNhomNoiSoi.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                NoiTru = dataNhomNoiSoi.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                SucKhoeKhac = dataNhomNoiSoi.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
            };
            var gridItemNhomDoHoHap = new BaoCaoHoatDongClsGridVo
            {
                TenDichVu = "- Chức năng hô hấp",
                DonVi = "Lần",
                NgoaiTru = dataNhomDoHoHap.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                NoiTru = dataNhomDoHoHap.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                SucKhoeKhac = dataNhomDoHoHap.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
            };
            var gridItemKhac = new BaoCaoHoatDongClsGridVo
            {
                TenDichVu = "- Khác",
                DonVi = "Lần",
                NgoaiTru = dataNhomThamDoChucNangKhac.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                NoiTru = dataNhomThamDoChucNangKhac.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
                SucKhoeKhac = dataNhomThamDoChucNangKhac.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe).Select(o => o.SoLan).DefaultIfEmpty().Sum(),
            };

            dataReturn.Add(new BaoCaoHoatDongClsGridVo
            {
                STT = "III",
                TenDichVu = "Thăm dò chức năng",
                DonVi = "",
                NgoaiTru = gridItemNhomDienTim.NgoaiTru + gridItemNhomDienNao.NgoaiTru + gridItemNhomNoiSoi.NgoaiTru + gridItemNhomDoHoHap.NgoaiTru + gridItemKhac.NgoaiTru,
                NoiTru = gridItemNhomDienTim.NoiTru + gridItemNhomDienNao.NoiTru + gridItemNhomNoiSoi.NoiTru + gridItemNhomDoHoHap.NoiTru + gridItemKhac.NoiTru,
                SucKhoeKhac = gridItemNhomDienTim.SucKhoeKhac + gridItemNhomDienNao.SucKhoeKhac + gridItemNhomNoiSoi.SucKhoeKhac + gridItemNhomDoHoHap.SucKhoeKhac + gridItemKhac.SucKhoeKhac,
                ToDam = true
            });
            dataReturn.Add(gridItemNhomDienTim);
            dataReturn.Add(gridItemNhomDienNao);
            dataReturn.Add(gridItemNhomNoiSoi);
            dataReturn.Add(gridItemNhomDoHoHap);
            dataReturn.Add(gridItemKhac);

            var slMau = _yeuCauTruyenMauRepository.TableNoTracking
                .Where(o=>o.XuatKhoMauChiTietId != null && o.XuatKhoMauChiTiet.XuatKhoMau.NgayXuat >= queryInfo.FromDate && o.XuatKhoMauChiTiet.XuatKhoMau.NgayXuat < queryInfo.ToDate)
                .Select(o => o.XuatKhoMauChiTiet.NhapKhoMauChiTiet.TheTich).DefaultIfEmpty(0).Sum();

            dataReturn.Add(new BaoCaoHoatDongClsGridVo
            {
                STT = "IV",
                TenDichVu = "TS lượng máu SD tại BV (ml)",
                DonVi = "",
                NgoaiTru = 0,
                NoiTru = (int)slMau,
                SucKhoeKhac = 0,
                ToDam = true
            });

            dataReturn.Add(new BaoCaoHoatDongClsGridVo
            {
                STT = "",
                TenDichVu = "SL máu từ TT huyết học",
                DonVi = "ml",
                NgoaiTru = 0,
                NoiTru = (int)slMau,
                SucKhoeKhac = 0,
            });
            return new GridDataSource
            {
                Data = dataReturn.ToArray(),
                TotalRowCount = dataReturn.Count()
            };
        }


        public virtual byte[] ExportBaoCaoHoatDongCLSMauThucTe(GridDataSource dataSource,  BaoCaoHoatDongClsQueryInfo query)
        {
            var datas = (ICollection<BaoCaoHoatDongClsGridVo>)dataSource.Data;
            var listDanhMucCha = datas.GroupBy(s => s.DanhMucCha).Select(s => s.First().DanhMucCha).ToList();

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BÁO CÁO HOẠT ĐỘNG CẬN LÂM SÀNG");
                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 10;
                    worksheet.Column(2).Width = 40;
                    worksheet.Column(3).Width = 15;
                    worksheet.Column(4).Width = 15;
                    worksheet.Column(5).Width = 15;
                    worksheet.Column(6).Width = 15;
                    worksheet.Column(7).Width = 15;
                    worksheet.Column(8).Width = 15;
                    worksheet.Column(9).Width = 10;
                    worksheet.Column(10).Width = 40;
                    worksheet.Column(11).Width = 15;
                    worksheet.Column(12).Width = 15;
                    worksheet.Column(13).Width = 15;
                    worksheet.Column(14).Width = 15;
                    worksheet.Column(15).Width = 15;
                    worksheet.DefaultColWidth = 7;
                    worksheet.Row(6).Height = 33;

                    using (var range = worksheet.Cells["A1:B1"])
                    {
                        range.Worksheet.Cells["A1:B1"].Merge = true;
                        range.Worksheet.Cells["A1:B1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                        range.Worksheet.Cells["A1:B1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A1:B1"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A1:B1"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A1:B1"].Style.Font.Color.SetColor(Color.Black);
                    }

                    

                    using (var range = worksheet.Cells["A2:B2"])
                    {
                        range.Worksheet.Cells["A2:B2"].Merge = true;
                        range.Worksheet.Cells["A2:B2"].Value = "Khoa: Phòng Kế Hoạch Tổng Hợp";
                        range.Worksheet.Cells["A2:B2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A2:B2"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A2:B2"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A2:B2"].Style.Font.Color.SetColor(Color.Black);
                    }


                   

                    using (var range = worksheet.Cells["A3:G3"])
                    {
                        range.Worksheet.Cells["A3:G3"].Merge = true;
                        range.Worksheet.Cells["A3:G3"].Value =  $"Từ ngày: " + query.FromDate.FormatNgayGioTimKiemTrenBaoCao()
                                                                  + " - đến ngày: " + query.ToDate.FormatNgayGioTimKiemTrenBaoCao();;

                        range.Worksheet.Cells["A3:G3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:G3"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A3:G3"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A3:G3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:G3"].Style.Font.Bold = true;
                    }

                  

                    using (var range = worksheet.Cells["A4:G4"])
                    {
                        range.Worksheet.Cells["A4:G4"].Merge = true;
                        range.Worksheet.Cells["A4:G4"].Value = "( Thực tế)";
                        range.Worksheet.Cells["A4:G4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:G4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A4:G4"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A4:G4"].Style.Font.Color.SetColor(Color.Black);
                    }

                   

                    using (var range = worksheet.Cells["A6:G6"])
                    {
                        range.Worksheet.Cells["A6:G6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A6:G6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A6:G6"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A6:G6"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A6:G6"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A6:G6"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A6:G6"].Style.WrapText = true;

                        range.Worksheet.Cells["A6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A6"].Value = "Số TT";

                        range.Worksheet.Cells["B6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B6"].Value = "Danh mục";

                        range.Worksheet.Cells["C6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C6"].Value = "Đơn vị";

                        range.Worksheet.Cells["D6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D6"].Value = "Tổng số";

                        range.Worksheet.Cells["E6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E6"].Value = "Nội trú";

                        range.Worksheet.Cells["F6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F6"].Value = "Ngoại trú";

                        range.Worksheet.Cells["G6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G6"].Value = "Sức khỏe + Khác";


                    }

                   

                    var stt = 1;
                    var index = 7;
                    var sott = 1;

                    if (listDanhMucCha.Any())
                    {
                        foreach (var item in datas)
                        {
                            if (item.ToDam)
                            {
                                using (var range = worksheet.Cells["A" + index + ":G" + index])
                                {
                                    range.Worksheet.Cells["A" + index + ":G" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                    range.Worksheet.Cells["A" + index + ":G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                    range.Worksheet.Cells["A" + index + ":G" + index].Style.Font.Color.SetColor(Color.Black);
                                    range.Worksheet.Cells["A" + index + ":G" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["A" + index + ":G" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["A" + index + ":G" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["A" + index + ":G" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["A" + index + ":G" + index].Style.Font.Bold = true;

                                    range.Worksheet.Cells["A" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["A" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["A" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["A" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["A" + index].Value = item.STT;
                                    range.Worksheet.Cells["A" + index].Style.Font.UnderLine = true;
                                    range.Worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                    range.Worksheet.Cells["B" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["B" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["B" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["B" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["B" + index].Value = item.TenDichVu;
                                    range.Worksheet.Cells["B" + index].Style.Font.UnderLine = true;
                                    range.Worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                    range.Worksheet.Cells["C" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["C" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["C" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["C" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                    range.Worksheet.Cells["D" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["D" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["D" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["D" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["D" + index].Value = item.TongSo;
                                    range.Worksheet.Cells["D" + index].Style.Font.UnderLine = true;
                                    range.Worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                    range.Worksheet.Cells["E" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["E" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["E" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["E" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["E" + index].Value = item.NoiTru;
                                    range.Worksheet.Cells["E" + index].Style.Font.UnderLine = true;
                                    range.Worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                    range.Worksheet.Cells["F" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["F" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["F" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["F" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["F" + index].Value = item.NgoaiTru;
                                    range.Worksheet.Cells["F" + index].Style.Font.UnderLine = true;
                                    range.Worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                    range.Worksheet.Cells["G" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["G" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["G" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["G" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["G" + index].Value = item.SucKhoeKhac;
                                    range.Worksheet.Cells["G" + index].Style.Font.UnderLine = true;
                                    range.Worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    index++;
                                }
                            }

                            if (!item.ToDam)
                            {
                                worksheet.Cells["A" + index + ":O" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                worksheet.Cells["A" + index + ":O" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                worksheet.Cells["A" + index + ":O" + index].Style.Font.Color.SetColor(Color.Black);


                                worksheet.Cells["A" + index + ":G" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["A" + index + ":G" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["A" + index + ":G" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["A" + index + ":G" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                worksheet.Cells["A" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["A" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["A" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["A" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["A" + index].Value = item.STT;

                                worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["B" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["B" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["B" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["B" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["B" + index].Value = item.TenDichVu;

                                worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["C" + index].Value = item.DonVi;
                                worksheet.Cells["C" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["C" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["C" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["C" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["D" + index].Value = item.TongSo;
                                worksheet.Cells["D" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["D" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["D" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["D" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["E" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["E" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["E" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["E" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["E" + index].Value = item.NoiTru;

                                worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["F" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["F" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["F" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["F" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["F" + index].Value = item.NgoaiTru;

                                worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["G" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["G" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["G" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["G" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["G" + index].Value = item.SucKhoeKhac;

                                index++;
                            }
                        }
                    }

                    worksheet.Cells["A" + index + ":G" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["A" + index + ":G" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["A" + index + ":G" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    index = index + 4;

                    worksheet.Cells["B" + index + ":G" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                    worksheet.Cells["B" + index + ":G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["B" + index + ":G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                    worksheet.Cells["B" + index + ":G" + index].Style.Font.Bold = true;

                    worksheet.Cells["B" + index + ":G" + index].Value = "Trưởng khoa";
                    worksheet.Cells["B" + index + ":G" + index].Merge = true;

                    index = index + 4;

                    worksheet.Cells["B" + index + ":G" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                    worksheet.Cells["B" + index + ":G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["B" + index + ":G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                    worksheet.Cells["B" + index + ":G" + index].Style.Font.Bold = true;

                    worksheet.Cells["B" + index + ":G" + index].Value = "BS.CKII Chử Thị Anh Thơ";
                    worksheet.Cells["B" + index + ":G" + index].Merge = true;
                    

                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }


        public virtual byte[] ExportBaoCaoHoatDongCLSMauCucQuanLy(GridDataSource dataSource, BaoCaoHoatDongClsQueryInfo query)
        {
            var datas = (ICollection<BaoCaoHoatDongClsGridVo>)dataSource.Data;         
            long userId = _userAgentHelper.GetCurrentUserId();
            string nguoiLogin = _nhanVienRepository.TableNoTracking.Where(x => x.Id == userId).Select(s => s.User.HoTen).FirstOrDefault();

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BÁO CÁO HOẠT ĐỘNG CẬN LÂM SÀNG");
                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 10;
                    worksheet.Column(2).Width = 40;
                    worksheet.Column(3).Width = 15;
                    worksheet.Column(4).Width = 15;
                    worksheet.Column(5).Width = 15;
                    worksheet.Column(6).Width = 15;
                    worksheet.Column(7).Width = 15;
                    worksheet.Column(8).Width = 15;
                    worksheet.Column(9).Width = 10;
                    worksheet.Column(10).Width = 40;
                    worksheet.Column(11).Width = 15;
                    worksheet.Column(12).Width = 15;
                    worksheet.Column(13).Width = 15;
                    worksheet.Column(14).Width = 15;
                    worksheet.Column(15).Width = 15;
                    worksheet.DefaultColWidth = 7;
                    worksheet.Row(6).Height = 33;

                    using (var range = worksheet.Cells["A1:B1"])
                    {
                        range.Worksheet.Cells["A1:B1"].Merge = true;
                        range.Worksheet.Cells["A1:B1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                        range.Worksheet.Cells["A1:B1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A1:B1"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A1:B1"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A1:B1"].Style.Font.Color.SetColor(Color.Black);
                    }



                    using (var range = worksheet.Cells["A2:B2"])
                    {
                        range.Worksheet.Cells["A2:B2"].Merge = true;
                        range.Worksheet.Cells["A2:B2"].Value = "Khoa: Phòng Kế Hoạch Tổng Hợp";
                        range.Worksheet.Cells["A2:B2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A2:B2"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A2:B2"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A2:B2"].Style.Font.Color.SetColor(Color.Black);
                    }



                    using (var range = worksheet.Cells["A3:G3"])
                    {
                        range.Worksheet.Cells["A3:G3"].Merge = true;
                        range.Worksheet.Cells["A3:G3"].Value = "BÁO CÁO HOẠT ĐỘNG CẬN LÂM SÀNG";
                        range.Worksheet.Cells["A3:G3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:G3"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A3:G3"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A3:G3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:G3"].Style.Font.Bold = true;
                    }



                    using (var range = worksheet.Cells["A4:G4"])
                    {
                        range.Worksheet.Cells["A4:G4"].Merge = true;
                        range.Worksheet.Cells["A4:G4"].Value = "( Mẫu Cục Quản lý Khám, chữa bệnh - Bộ Y Tế)";
                        range.Worksheet.Cells["A4:G4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:G4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A4:G4"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A4:G4"].Style.Font.Color.SetColor(Color.Black);
                    }



                    using (var range = worksheet.Cells["A6:G6"])
                    {
                        range.Worksheet.Cells["A6:G6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A6:G6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A6:G6"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A6:G6"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A6:G6"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A6:G6"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A6:G6"].Style.WrapText = true;

                        range.Worksheet.Cells["A6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A6"].Value = "Số TT";

                        range.Worksheet.Cells["B6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //range.Worksheet.Cells["B6"].Value = $"Tháng {query.MonthYear.Month}/{query.MonthYear.Year}";

                        range.Worksheet.Cells["C6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C6"].Value = "Đơn vị";

                        range.Worksheet.Cells["D6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D6"].Value = "Tổng số";

                        range.Worksheet.Cells["E6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E6"].Value = "Nội trú";

                        range.Worksheet.Cells["F6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F6"].Value = "Ngoại trú";

                        range.Worksheet.Cells["G6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G6"].Value = "Sức khỏe + Khác";


                    }

                   
                    var index = 7;
                  
                    if (datas.Any())
                    {
                        foreach (var item in datas)
                        {
                            if (item.ToDam)
                            {
                                using (var range = worksheet.Cells["A" + index + ":G" + index])
                                {
                                    range.Worksheet.Cells["A" + index + ":G" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                    range.Worksheet.Cells["A" + index + ":G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                    range.Worksheet.Cells["A" + index + ":G" + index].Style.Font.Color.SetColor(Color.Black);
                                    range.Worksheet.Cells["A" + index + ":G" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["A" + index + ":G" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["A" + index + ":G" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["A" + index + ":G" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["A" + index + ":G" + index].Style.Font.Bold = true;

                                    range.Worksheet.Cells["A" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["A" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["A" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["A" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["A" + index].Value = item.STT;
                                    range.Worksheet.Cells["A" + index].Style.Font.UnderLine = true;
                                    range.Worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                    range.Worksheet.Cells["B" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["B" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["B" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["B" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["B" + index].Value = item.TenDichVu;
                                    range.Worksheet.Cells["B" + index].Style.Font.UnderLine = true;
                                    range.Worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                    range.Worksheet.Cells["C" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["C" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["C" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["C" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                    range.Worksheet.Cells["D" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["D" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["D" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["D" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["D" + index].Value = item.TongSo;
                                    range.Worksheet.Cells["D" + index].Style.Font.UnderLine = true;
                                    range.Worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                    range.Worksheet.Cells["E" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["E" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["E" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["E" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["E" + index].Value = item.NoiTru;
                                    range.Worksheet.Cells["E" + index].Style.Font.UnderLine = true;
                                    range.Worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                    range.Worksheet.Cells["F" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["F" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["F" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["F" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["F" + index].Value = item.NgoaiTru;
                                    range.Worksheet.Cells["F" + index].Style.Font.UnderLine = true;
                                    range.Worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                    range.Worksheet.Cells["G" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["G" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["G" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["G" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    range.Worksheet.Cells["G" + index].Value = item.SucKhoeKhac;
                                    range.Worksheet.Cells["G" + index].Style.Font.UnderLine = true;
                                    range.Worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    index++;
                                }
                            }                    

                            if (!item.ToDam)
                            {
                                worksheet.Cells["A" + index + ":O" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                worksheet.Cells["A" + index + ":O" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                worksheet.Cells["A" + index + ":O" + index].Style.Font.Color.SetColor(Color.Black);


                                worksheet.Cells["A" + index + ":G" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["A" + index + ":G" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["A" + index + ":G" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["A" + index + ":G" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                worksheet.Cells["A" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["A" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["A" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["A" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["A" + index].Value = item.STT;

                                worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["B" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["B" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["B" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["B" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["B" + index].Value = item.TenDichVu;

                                worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["C" + index].Value = item.DonVi;
                                worksheet.Cells["C" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["C" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["C" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["C" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["D" + index].Value = item.TongSo;
                                worksheet.Cells["D" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["D" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["D" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["D" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["E" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["E" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["E" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["E" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["E" + index].Value = item.NoiTru;

                                worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["F" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["F" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["F" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["F" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["F" + index].Value = item.NgoaiTru;

                                worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["G" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["G" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["G" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["G" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells["G" + index].Value = item.SucKhoeKhac;

                                index++;
                            }
                        }
                    }


                    worksheet.Cells["A" + index + ":G" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["A" + index + ":G" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["A" + index + ":G" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    index = index + 2 ;
                    worksheet.Cells["B" + index + ":G" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                    worksheet.Cells["B" + index + ":G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["B" + index + ":G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                    worksheet.Cells["B" + index + ":G" + index].Style.Font.Italic = true;

                    worksheet.Cells["B" + index + ":G" + index].Value = $"Hà Nội, ngày {DateTime.Now.Day} tháng {DateTime.Now.Month} năm {DateTime.Now.Year}";
                    worksheet.Cells["B" + index + ":G" + index].Merge = true;

                    index = index + 2;

                    worksheet.Cells["B" + index + ":G" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                    worksheet.Cells["B" + index + ":G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["B" + index + ":G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                    worksheet.Cells["B" + index + ":G" + index].Style.Font.Bold = true;

                    worksheet.Cells["B" + index + ":G" + index].Value = "Người tổng hợp";
                    worksheet.Cells["B" + index + ":G" + index].Merge = true;

                    index = index + 4;

                    worksheet.Cells["B" + index + ":G" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                    worksheet.Cells["B" + index + ":G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["B" + index + ":G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));

                    worksheet.Cells["B" + index + ":G" + index].Value = nguoiLogin;
                    worksheet.Cells["B" + index + ":G" + index].Merge = true;

                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }

    }
}
