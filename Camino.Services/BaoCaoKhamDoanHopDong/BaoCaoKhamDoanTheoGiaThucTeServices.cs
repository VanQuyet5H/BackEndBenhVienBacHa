using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using System;
using Newtonsoft.Json;
using Camino.Core.Domain.Entities.KhamDoans;
using Camino.Core.Domain.ValueObject.BaoCaoKhamDoanHopDong;
using System.Globalization;
using static Camino.Core.Domain.Enums;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using System.Collections.Generic;
using Camino.Core.Helpers;
using OfficeOpenXml.Style;
using System.Drawing;
using System.IO;
using OfficeOpenXml;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.BaoCao.BaoCaoTongHopKetQuaKhamDoan;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.KhamDoan;
using Camino.Services.ExportImport.Help;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using System.Text.RegularExpressions;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Services.KhamDoan;
using Microsoft.EntityFrameworkCore.Internal;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;
using Camino.Core.Domain.Entities.DichVuKyThuats;
using Camino.Core.Domain.ValueObject.BaoCao.BaoCaoDuTruSoLuongNguoiThucHienDvLSCLS;
using Camino.Core.Domain.ValueObject.DSDichVuNgoaiGoiKeToan;
using Camino.Core.Domain.Entities.GoiKhamSucKhoeChungDichVuKhamBenhNhanViens;
using Camino.Core.Domain.Entities.GoiKhamSucKhoeChungDichVuKyThuatNhanViens;
using Camino.Core.Domain.ValueObject.BaoCao.BaoCaoDoanhThuKhamDoanTheoNhomDichVu;
using Camino.Services.CauHinh;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Domain.Entities.XetNghiems;

namespace Camino.Services.BaoCaoKhamDoanHopDong
{
    [ScopedDependency(ServiceType = typeof(IBaoCaoKhamDoanTheoGiaThucTeServices))]
    public class BaoCaoKhamDoanTheoGiaThucTeServices : MasterFileService<HopDongKhamSucKhoeNhanVien>, IBaoCaoKhamDoanTheoGiaThucTeServices
    {
        private readonly IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> _yeuCauKhamBenhRepository;
        private readonly IRepository<YeuCauDichVuKyThuat> _yeuCauDichVuKyThuatRepository;
        private readonly IRepository<YeuCauTiepNhan> _yeuCauTiepNhanRepository;
        private readonly IRepository<GoiKhamSucKhoe> _goiKhamSucKhoeRepository;
        private readonly IRepository<GoiKhamSucKhoeDichVuKhamBenh> _goiKhamSucKhoeDichVuKhamBenhRepository;
        private readonly IRepository<GoiKhamSucKhoeDichVuDichVuKyThuat> _goiKhamSucKhoeDichVuKyThuatRepository;
        private readonly ICauHinhService _cauHinhService;

        public BaoCaoKhamDoanTheoGiaThucTeServices(
            IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> yeuCauKhamBenhRepository,
            IRepository<HopDongKhamSucKhoeNhanVien> repository,
            IRepository<YeuCauDichVuKyThuat> yeuCauDichVuKyThuatRepository,
            IRepository<YeuCauTiepNhan> yeuCauTiepNhanRepository,
            IRepository<GoiKhamSucKhoe> goiKhamSucKhoeRepository,
            IRepository<GoiKhamSucKhoeDichVuKhamBenh> goiKhamSucKhoeDichVuKhamBenhRepository,
            IRepository<GoiKhamSucKhoeDichVuDichVuKyThuat> goiKhamSucKhoeDichVuKyThuatRepository,
            ICauHinhService cauHinhService
            ) : base(repository)
        {
            _yeuCauKhamBenhRepository = yeuCauKhamBenhRepository;
            _yeuCauDichVuKyThuatRepository = yeuCauDichVuKyThuatRepository;
            _yeuCauTiepNhanRepository = yeuCauTiepNhanRepository;
            _goiKhamSucKhoeRepository = goiKhamSucKhoeRepository;
            _goiKhamSucKhoeDichVuKhamBenhRepository = goiKhamSucKhoeDichVuKhamBenhRepository;
            _goiKhamSucKhoeDichVuKyThuatRepository = goiKhamSucKhoeDichVuKyThuatRepository;
            _cauHinhService = cauHinhService;
        }

        public ICauHinhService _CauHinhService { get; }

        public GridDataSource BaoCaoDoanhThuKhamDoanTheoNhomDichVuThucTe(BaoCaoDoanhThuKhamDoanTheoNhomDichVuThucTeQueryInfo queryInfo)
        {
            var lstDoanhThuDichVuKhamQuery = _yeuCauKhamBenhRepository.TableNoTracking
                            .Where(x => x.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham
                                        && ((x.ThoiDiemHoanThanh != null && x.ThoiDiemHoanThanh >= queryInfo.FromDate && x.ThoiDiemHoanThanh < queryInfo.ToDate) || (x.ThoiDiemHoanThanh == null && x.ThoiDiemThucHien != null && x.ThoiDiemThucHien >= queryInfo.FromDate && x.ThoiDiemThucHien < queryInfo.ToDate))
                                        && x.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe && x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId != null);
            var lstDoanhThuDichVuKyThuatQuery = _yeuCauDichVuKyThuatRepository.TableNoTracking
                            .Where(x => (x.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien || (x.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem && x.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien))
                                        && ((x.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem && x.ThoiDiemThucHien != null && x.PhienXetNghiemChiTiets.Any(p => p.ThoiDiemNhanMau != null && p.ThoiDiemNhanMau >= queryInfo.FromDate && p.ThoiDiemNhanMau < queryInfo.ToDate))
                                            || (x.LoaiDichVuKyThuat != LoaiDichVuKyThuat.XetNghiem && ((x.ThoiDiemHoanThanh != null && x.ThoiDiemHoanThanh >= queryInfo.FromDate && x.ThoiDiemHoanThanh < queryInfo.ToDate) || (x.ThoiDiemHoanThanh == null && x.ThoiDiemThucHien != null && x.ThoiDiemThucHien >= queryInfo.FromDate && x.ThoiDiemThucHien < queryInfo.ToDate))))
                                        && x.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe && x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId != null);
            if (queryInfo.CongTyId != 0)
            {
                if (queryInfo.HopDongId != 0)
                {
                    lstDoanhThuDichVuKhamQuery = lstDoanhThuDichVuKhamQuery.Where(o => o.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId == queryInfo.HopDongId);
                    lstDoanhThuDichVuKyThuatQuery = lstDoanhThuDichVuKyThuatQuery.Where(o => o.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId == queryInfo.HopDongId);
                }
                else
                {
                    lstDoanhThuDichVuKhamQuery = lstDoanhThuDichVuKhamQuery.Where(o => o.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoeId == queryInfo.CongTyId);
                }
            }

            var lstDoanhThuDichVuKham = lstDoanhThuDichVuKhamQuery
                            .Select(item => new BaoCaoDoanhThuKhamDoanTheoNhomDichVuThucTeDataVo()
                            {
                                YeucauTiepNhanId = item.YeuCauTiepNhanId,
                                YeucauKhamBenhId = item.Id,
                                TenDichVu = item.TenDichVu,
                                GoiKhamSucKhoeId = item.GoiKhamSucKhoeId,
                                DichVuKhamBenhBenhVienId = item.DichVuKhamBenhBenhVienId,
                                NhomGiaDichVuKhamBenhBenhVienId = item.NhomGiaDichVuKhamBenhBenhVienId,
                                Gia = item.Gia,
                                SoLan = 1,
                                //DonGiaUuDai = item.DonGiaUuDai,
                                SoTienMienGiam = item.SoTienMienGiam,
                            })
                            .ToList();
            var lstDoanhThuDichVuKyThuat = lstDoanhThuDichVuKyThuatQuery
                            .Select(item => new BaoCaoDoanhThuKhamDoanTheoNhomDichVuThucTeDataVo()
                            {
                                YeucauTiepNhanId = item.YeuCauTiepNhanId,
                                YeucauDichVuKyThuatId = item.Id,
                                NhomDichVuBenhVienId = item.NhomDichVuBenhVienId,
                                LoaiDichVuKyThuat = item.LoaiDichVuKyThuat,
                                //BaoCaoDoanhThuKhamDoanTheoNhomDichVuDataXetNghiemVos = item.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem ?
                                //    item.PhienXetNghiemChiTiets.Select(p => new BaoCaoDoanhThuKhamDoanTheoNhomDichVuDataXetNghiemVo { Id = p.Id, ThoiDiemNhanMau = p.ThoiDiemNhanMau}).ToList() :
                                //    new List<BaoCaoDoanhThuKhamDoanTheoNhomDichVuDataXetNghiemVo>(),
                                TenDichVu = item.TenDichVu,
                                GoiKhamSucKhoeId = item.GoiKhamSucKhoeId,
                                DichVuKyThuatBenhVienId = item.DichVuKyThuatBenhVienId,
                                NhomGiaDichVuKyThuatBenhVienId = item.NhomGiaDichVuKyThuatBenhVienId,
                                Gia = item.Gia,
                                SoLan = item.SoLan,
                                //DonGiaUuDai = item.DonGiaUuDai,
                                SoTienMienGiam = item.SoTienMienGiam,
                            })
                            .ToList();
            //var lstDoanhThuDichVuKyThuat = lstDoanhThuDichVuKyThuatAll.Where(o =>
            //    o.LoaiDichVuKyThuat != LoaiDichVuKyThuat.XetNghiem ||
            //    (o.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem &&
            //     o.BaoCaoDoanhThuKhamDoanTheoNhomDichVuDataXetNghiemVos.Any(x=>x.ThoiDiemNhanMau != null && x.ThoiDiemNhanMau >= queryInfo.FromDate && x.ThoiDiemNhanMau < queryInfo.ToDate))).ToList();
            var allDichVu = lstDoanhThuDichVuKham.Concat(lstDoanhThuDichVuKyThuat).ToList();

            var goiKhamSucKhoeIds = allDichVu.Select(o => o.GoiKhamSucKhoeId.GetValueOrDefault()).Distinct().ToList();
            var goiKhamSucKhoeDichVuKhamBenhs = _goiKhamSucKhoeDichVuKhamBenhRepository.TableNoTracking
                .Where(o => goiKhamSucKhoeIds.Contains(o.GoiKhamSucKhoeId))
                .Select(o => new
                {
                    o.GoiKhamSucKhoeId,
                    o.DichVuKhamBenhBenhVienId,
                    o.NhomGiaDichVuKhamBenhBenhVienId,
                    o.DonGiaThucTe
                }).ToList();
            var goiKhamSucKhoeDichVuKyThuats = _goiKhamSucKhoeDichVuKyThuatRepository.TableNoTracking
                .Where(o => goiKhamSucKhoeIds.Contains(o.GoiKhamSucKhoeId))
                .Select(o => new
                {
                    o.GoiKhamSucKhoeId,
                    o.DichVuKyThuatBenhVienId,
                    o.NhomGiaDichVuKyThuatBenhVienId,
                    o.DonGiaThucTe
                }).ToList();

            var yctnIds = allDichVu.Select(o => o.YeucauTiepNhanId).Distinct().ToList();
            var yctnDatas = _yeuCauTiepNhanRepository.TableNoTracking.Where(o => yctnIds.Contains(o.Id))
                .Select(o => new BaoCaoDoanhThuKhamDoanTheoNhomDichVuThucTeYCTNDataVo
                {
                    YeucauTiepNhanId = o.Id,
                    HopDongId = o.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId,
                    CongTyId = o.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoeId,
                    MaNB = o.BenhNhan.MaBN,
                    MaTN = o.MaYeuCauTiepNhan,
                    HoVaTen = o.HoTen,
                    NamSinh = o.NamSinh,
                    GioiTinh = o.GioiTinh,
                    TenCongTy = o.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoe.Ten,
                    SoHopDong = o.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.SoHopDong
                }).ToList();
            var cauHinhBaoCao = _cauHinhService.LoadSetting<CauHinhBaoCao>();

            var gridData = new List<BaoCaoDoanhThuKhamDoanTheoNhomDichVuThucTeGridVo>();
            foreach (var dv in allDichVu)
            {
                var yctn = yctnDatas.FirstOrDefault(o => o.YeucauTiepNhanId == dv.YeucauTiepNhanId);
                var gridItem = new BaoCaoDoanhThuKhamDoanTheoNhomDichVuThucTeGridVo
                {
                    MaTN = yctn.MaTN,
                    MaNB = yctn.MaNB,
                    HopDongId = yctn.HopDongId,
                    CongTyId = yctn.CongTyId,
                    HoVaTen = yctn.HoVaTen,
                    NamSinh = yctn.NamSinh?.ToString(),
                    GioiTinh = yctn.GioiTinh?.GetDescription(),
                    TenCongTy = $"{yctn.TenCongTy} - {yctn.SoHopDong}"
                };

                if (dv.GoiKhamSucKhoeId != null)
                {
                    
                    if (dv.YeucauKhamBenhId != null)
                    {
                        var goiKhamSucKhoeDichVuKhamBenh = goiKhamSucKhoeDichVuKhamBenhs
                            .FirstOrDefault(o => o.GoiKhamSucKhoeId == dv.GoiKhamSucKhoeId && o.DichVuKhamBenhBenhVienId == dv.DichVuKhamBenhBenhVienId && o.NhomGiaDichVuKhamBenhBenhVienId == dv.NhomGiaDichVuKhamBenhBenhVienId);
                        var doanhThu = (goiKhamSucKhoeDichVuKhamBenh?.DonGiaThucTe ?? 0) * dv.SoLan;
                        gridItem.SoTienDVKhamBenh = doanhThu;
                        gridItem.TongCong = doanhThu;
                    }
                    else
                    {
                        var goiKhamSucKhoeDichVuKyThuat = goiKhamSucKhoeDichVuKyThuats
                            .FirstOrDefault(o => o.GoiKhamSucKhoeId == dv.GoiKhamSucKhoeId && o.DichVuKyThuatBenhVienId == dv.DichVuKyThuatBenhVienId && o.NhomGiaDichVuKyThuatBenhVienId == dv.NhomGiaDichVuKyThuatBenhVienId);
                        var doanhThu = (goiKhamSucKhoeDichVuKyThuat?.DonGiaThucTe ?? 0) * dv.SoLan;
                        if (dv.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem)
                        {
                            gridItem.SoTienDVXetNghiem = doanhThu;
                        }
                        else
                        {
                            if (dv.NhomDichVuBenhVienId == cauHinhBaoCao.NhomNoiSoi)
                            {
                                gridItem.SoTienDVThamDoChucNangNoiSoi = doanhThu;
                            }
                            else if (dv.NhomDichVuBenhVienId == cauHinhBaoCao.NhomNoiSoiTMH)
                            {
                                gridItem.SoTienDVTDCNNoiSoiTMH = doanhThu;
                            }
                            else if (dv.NhomDichVuBenhVienId == cauHinhBaoCao.NhomSieuAm)
                            {
                                gridItem.SoTienDVCDHASieuAm = doanhThu;
                            }
                            else if (dv.NhomDichVuBenhVienId == cauHinhBaoCao.NhomXQuangThuong || dv.NhomDichVuBenhVienId == cauHinhBaoCao.NhomXQuangSoHoa)
                            {
                                gridItem.SoTienDVCDHAXQuangThuongXQuangSoHoa = doanhThu;
                            }
                            else if (dv.NhomDichVuBenhVienId == cauHinhBaoCao.NhomCTScanner)
                            {
                                gridItem.SoTienDVCTScan = doanhThu;
                            }
                            else if (dv.NhomDichVuBenhVienId == cauHinhBaoCao.NhomMRI)
                            {
                                gridItem.SoTienDVMRI = doanhThu;
                            }
                            else if (dv.NhomDichVuBenhVienId == cauHinhBaoCao.NhomDienTim || dv.NhomDichVuBenhVienId == cauHinhBaoCao.NhomDienNao)
                            {
                                gridItem.SoTienDVDienTimDienNao = doanhThu;
                            }
                            else if (dv.NhomDichVuBenhVienId == cauHinhBaoCao.NhomDoLoangXuong || dv.NhomDichVuBenhVienId == cauHinhBaoCao.NhomDoHoHap)
                            {
                                gridItem.SoTienDVTDCNDoLoangXuong = doanhThu;
                            }
                            else
                            {
                                gridItem.SoTienDVKhac = doanhThu;
                            }
                        }
                        gridItem.TongCong = doanhThu;
                    }                    
                }
                else
                {
                    var doanhThu = dv.Gia * dv.SoLan - dv.SoTienMienGiam.GetValueOrDefault();
                    if (dv.YeucauKhamBenhId != null)
                    {
                        gridItem.SoTienDVKhamBenhNG = doanhThu;
                    }
                    else
                    {
                        if (dv.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem)
                        {
                            gridItem.SoTienDVXetNghiemNG = doanhThu;
                        }
                        if (dv.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat)
                        {
                            if (dv.TenDichVu != null &&
                                dv.TenDichVu.ToLower().Contains("phẫu thuật"))
                            {
                                gridItem.SoTienDVPhauThuatNG = doanhThu;
                            }
                            else
                            {
                                gridItem.SoTienDVThuThuatNG = doanhThu;
                            }
                        }
                        else
                        {
                            if (dv.NhomDichVuBenhVienId == cauHinhBaoCao.NhomNoiSoi)
                            {
                                gridItem.SoTienDVThamDoChucNangNoiSoiNG = doanhThu;
                            }
                            else if (dv.NhomDichVuBenhVienId == cauHinhBaoCao.NhomNoiSoiTMH)
                            {
                                gridItem.SoTienDVTDCNNoiSoiTMHNG = doanhThu;
                            }
                            else if (dv.NhomDichVuBenhVienId == cauHinhBaoCao.NhomSieuAm)
                            {
                                gridItem.SoTienDVCDHASieuAmNG = doanhThu;
                            }
                            else if (dv.NhomDichVuBenhVienId == cauHinhBaoCao.NhomXQuangThuong || dv.NhomDichVuBenhVienId == cauHinhBaoCao.NhomXQuangSoHoa)
                            {
                                gridItem.SoTienDVCDHAXQuangThuongXQuangSoHoaNG = doanhThu;
                            }
                            else if (dv.NhomDichVuBenhVienId == cauHinhBaoCao.NhomCTScanner)
                            {
                                gridItem.SoTienDVCTScanNG = doanhThu;
                            }
                            else if (dv.NhomDichVuBenhVienId == cauHinhBaoCao.NhomMRI)
                            {
                                gridItem.SoTienDVMRING = doanhThu;
                            }
                            else if (dv.NhomDichVuBenhVienId == cauHinhBaoCao.NhomDienTim || dv.NhomDichVuBenhVienId == cauHinhBaoCao.NhomDienNao)
                            {
                                gridItem.SoTienDVDienTimDienNaoNG = doanhThu;
                            }
                            else if (dv.NhomDichVuBenhVienId == cauHinhBaoCao.NhomDoLoangXuong || dv.NhomDichVuBenhVienId == cauHinhBaoCao.NhomDoHoHap)
                            {
                                gridItem.SoTienDVTDCNDoLoangXuongNG = doanhThu;
                            }
                            else
                            {
                                gridItem.SoTienDVKhacNG = doanhThu;
                            }
                        }
                    }
                    gridItem.TongCongNG = doanhThu;
                }
                gridData.Add(gridItem);
            }

            var returnData = new List<BaoCaoDoanhThuKhamDoanTheoNhomDichVuThucTeGridVo>();
            foreach (var yctn in gridData.GroupBy(o => o.MaTN))
            {
                var returnItem = new BaoCaoDoanhThuKhamDoanTheoNhomDichVuThucTeGridVo
                {
                    MaTN = yctn.Key,
                    MaNB = yctn.First().MaNB,
                    HopDongId = yctn.First().HopDongId,
                    CongTyId = yctn.First().CongTyId,
                    HoVaTen = yctn.First().HoVaTen,
                    NamSinh = yctn.First().NamSinh,
                    GioiTinh = yctn.First().GioiTinh,
                    TenCongTy = yctn.First().TenCongTy
                };
                returnItem.SoTienDVKhamBenh = yctn.All(o => o.SoTienDVKhamBenh == null) ? (decimal?)null : yctn.Select(o => o.SoTienDVKhamBenh.GetValueOrDefault()).Sum();

                returnItem.SoTienDVXetNghiem = yctn.All(o => o.SoTienDVXetNghiem == null) ? (decimal?)null : yctn.Select(o => o.SoTienDVXetNghiem.GetValueOrDefault()).Sum();

                returnItem.SoTienDVThamDoChucNangNoiSoi = yctn.All(o => o.SoTienDVThamDoChucNangNoiSoi == null) ? (decimal?)null : yctn.Select(o => o.SoTienDVThamDoChucNangNoiSoi.GetValueOrDefault()).Sum();

                returnItem.SoTienDVTDCNNoiSoiTMH = yctn.All(o => o.SoTienDVTDCNNoiSoiTMH == null) ? (decimal?)null : yctn.Select(o => o.SoTienDVTDCNNoiSoiTMH.GetValueOrDefault()).Sum();

                returnItem.SoTienDVCDHASieuAm = yctn.All(o => o.SoTienDVCDHASieuAm == null) ? (decimal?)null : yctn.Select(o => o.SoTienDVCDHASieuAm.GetValueOrDefault()).Sum();

                returnItem.SoTienDVCDHAXQuangThuongXQuangSoHoa = yctn.All(o => o.SoTienDVCDHAXQuangThuongXQuangSoHoa == null) ? (decimal?)null : yctn.Select(o => o.SoTienDVCDHAXQuangThuongXQuangSoHoa.GetValueOrDefault()).Sum();

                returnItem.SoTienDVCTScan = yctn.All(o => o.SoTienDVCTScan == null) ? (decimal?)null : yctn.Select(o => o.SoTienDVCTScan.GetValueOrDefault()).Sum();

                returnItem.SoTienDVMRI = yctn.All(o => o.SoTienDVMRI == null) ? (decimal?)null : yctn.Select(o => o.SoTienDVMRI.GetValueOrDefault()).Sum();

                returnItem.SoTienDVDienTimDienNao = yctn.All(o => o.SoTienDVDienTimDienNao == null) ? (decimal?)null : yctn.Select(o => o.SoTienDVDienTimDienNao.GetValueOrDefault()).Sum();

                returnItem.SoTienDVTDCNDoLoangXuong = yctn.All(o => o.SoTienDVTDCNDoLoangXuong == null) ? (decimal?)null : yctn.Select(o => o.SoTienDVTDCNDoLoangXuong.GetValueOrDefault()).Sum();

                returnItem.SoTienDVKhac = yctn.All(o => o.SoTienDVKhac == null) ? (decimal?)null : yctn.Select(o => o.SoTienDVKhac.GetValueOrDefault()).Sum();

                returnItem.TongCong = yctn.All(o => o.TongCong == null) ? (decimal?)null : yctn.Select(o => o.TongCong.GetValueOrDefault()).Sum();

                returnItem.SoTienDVKhamBenhNG = yctn.All(o => o.SoTienDVKhamBenhNG == null) ? (decimal?)null : yctn.Select(o => o.SoTienDVKhamBenhNG.GetValueOrDefault()).Sum();

                returnItem.SoTienDVXetNghiemNG = yctn.All(o => o.SoTienDVXetNghiemNG == null) ? (decimal?)null : yctn.Select(o => o.SoTienDVXetNghiemNG.GetValueOrDefault()).Sum();

                returnItem.SoTienDVPhauThuatNG = yctn.All(o => o.SoTienDVPhauThuatNG == null) ? (decimal?)null : yctn.Select(o => o.SoTienDVPhauThuatNG.GetValueOrDefault()).Sum();

                returnItem.SoTienDVThuThuatNG = yctn.All(o => o.SoTienDVThuThuatNG == null) ? (decimal?)null : yctn.Select(o => o.SoTienDVThuThuatNG.GetValueOrDefault()).Sum();

                returnItem.SoTienDVThamDoChucNangNoiSoiNG = yctn.All(o => o.SoTienDVThamDoChucNangNoiSoiNG == null) ? (decimal?)null : yctn.Select(o => o.SoTienDVThamDoChucNangNoiSoiNG.GetValueOrDefault()).Sum();

                returnItem.SoTienDVTDCNNoiSoiTMHNG = yctn.All(o => o.SoTienDVTDCNNoiSoiTMHNG == null) ? (decimal?)null : yctn.Select(o => o.SoTienDVTDCNNoiSoiTMHNG.GetValueOrDefault()).Sum();

                returnItem.SoTienDVCDHASieuAmNG = yctn.All(o => o.SoTienDVCDHASieuAmNG == null) ? (decimal?)null : yctn.Select(o => o.SoTienDVCDHASieuAmNG.GetValueOrDefault()).Sum();

                returnItem.SoTienDVCDHAXQuangThuongXQuangSoHoaNG = yctn.All(o => o.SoTienDVCDHAXQuangThuongXQuangSoHoaNG == null) ? (decimal?)null : yctn.Select(o => o.SoTienDVCDHAXQuangThuongXQuangSoHoaNG.GetValueOrDefault()).Sum();

                returnItem.SoTienDVCTScanNG = yctn.All(o => o.SoTienDVCTScanNG == null) ? (decimal?)null : yctn.Select(o => o.SoTienDVCTScanNG.GetValueOrDefault()).Sum();

                returnItem.SoTienDVMRING = yctn.All(o => o.SoTienDVMRING == null) ? (decimal?)null : yctn.Select(o => o.SoTienDVMRING.GetValueOrDefault()).Sum();

                returnItem.SoTienDVDienTimDienNaoNG = yctn.All(o => o.SoTienDVDienTimDienNaoNG == null) ? (decimal?)null : yctn.Select(o => o.SoTienDVDienTimDienNaoNG.GetValueOrDefault()).Sum();

                returnItem.SoTienDVTDCNDoLoangXuongNG = yctn.All(o => o.SoTienDVTDCNDoLoangXuongNG == null) ? (decimal?)null : yctn.Select(o => o.SoTienDVTDCNDoLoangXuongNG.GetValueOrDefault()).Sum();

                returnItem.SoTienDVKhacNG = yctn.All(o => o.SoTienDVKhacNG == null) ? (decimal?)null : yctn.Select(o => o.SoTienDVKhacNG.GetValueOrDefault()).Sum();

                returnItem.TongCongNG = yctn.All(o => o.TongCongNG == null) ? (decimal?)null : yctn.Select(o => o.TongCongNG.GetValueOrDefault()).Sum();

                returnData.Add(returnItem);
            }

            return new GridDataSource { Data = returnData.OrderBy(o => o.MaTN).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray(), TotalRowCount = returnData.Count };
        }

        public virtual byte[] ExportBaoCaoDoanhThuKhamDoanTheoNhomDichVuThucTe(IList<BaoCaoDoanhThuKhamDoanTheoNhomDichVuThucTeGridVo> bc,
                                                          BaoCaoDoanhThuKhamDoanTheoNhomDichVuThucTeQueryInfo queryInfo)
        {
            var tuNgay = queryInfo.ToDate;
            var denNgay = queryInfo.FromDate;

            var dataBaoCaos = bc.ToList();

            int ind = 1;

            var requestProperties = new[]
            {
                new PropertyByName<BaoCaoDoanhThuKhamDoanTheoNhomDichVuThucTeGridVo>("STT",p => ind++),
            };

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BÁO CÁO DOANH THU KHÁM ĐOÀN THEO NHÓM DỊCH VỤ THỰC TẾ");

                    // set row
                    worksheet.Row(9).Height = 24.5;
                    worksheet.DefaultRowHeight = 16;

                    // set column
                    worksheet.Column(2).Width = 30;
                    worksheet.Column(3).Width = 30;
                    worksheet.Column(4).Width = 20;
                    worksheet.Column(5).Width = 20;
                    worksheet.Column(6).Width = 20;
                    worksheet.Column(7).Width = 20;
                    worksheet.Column(8).Width = 20;
                    worksheet.Column(9).Width = 15;
                    worksheet.Column(10).Width = 15;
                    worksheet.Column(11).Width = 15;
                    worksheet.Column(12).Width = 15;
                    worksheet.Column(13).Width = 15;
                    worksheet.Column(14).Width = 15;
                    worksheet.Column(15).Width = 15;
                    worksheet.Column(16).Width = 15;

                    worksheet.Column(17).Width = 15;
                    worksheet.Column(18).Width = 15;
                    worksheet.Column(19).Width = 15;
                    worksheet.Column(20).Width = 15;
                    worksheet.Column(21).Width = 15;
                    worksheet.Column(22).Width = 15;
                    worksheet.Column(23).Width = 15;
                    worksheet.Column(24).Width = 15;
                    worksheet.Column(25).Width = 15;
                    worksheet.Column(26).Width = 15;
                    worksheet.Column(27).Width = 15;
                    worksheet.Column(28).Width = 15;
                    worksheet.Column(29).Width = 15;
                    worksheet.Column(30).Width = 15;
                    worksheet.Column(31).Width = 15;
                    worksheet.Column(32).Width = 15;

                    worksheet.DefaultColWidth = 7;

                    //set column 
                    string[] SetColumnItems = { "A", "B", "C", "D", "E", "F", "G", "H", "A", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF" };
                    var worksheetTitleThuPhi = SetColumnItems[0] + 3 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 4;
                    var worksheetTitleNgay = SetColumnItems[0] + 5 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 5;

                    using (var range = worksheet.Cells[worksheetTitleThuPhi])
                    {
                        range.Worksheet.Cells[worksheetTitleThuPhi].Merge = true;
                        range.Worksheet.Cells[worksheetTitleThuPhi].Value = "BÁO CÁO DOANH THU KHÁM ĐOÀN THEO NHÓM DỊCH VỤ".ToUpper();
                        range.Worksheet.Cells[worksheetTitleThuPhi].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleThuPhi].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleThuPhi].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells[worksheetTitleThuPhi].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleThuPhi].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells[worksheetTitleNgay])
                    {
                        range.Worksheet.Cells[worksheetTitleNgay].Merge = true;
                        range.Worksheet.Cells[worksheetTitleNgay].Value = "Từ ngày: " + queryInfo.FromDate.ApplyFormatDate() + " đến ngày: " + queryInfo.ToDate.ApplyFormatDate();
                        range.Worksheet.Cells[worksheetTitleNgay].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleNgay].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Bold = true;
                        //range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Italic = true;
                    }


                    using (var range = worksheet.Cells["A6:AF8"])
                    {
                        range.Worksheet.Cells["A6:AF8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A6:AF8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A6:AF8"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A6:AF8"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A6:AF8"].Style.Font.Bold = true;



                        //Set column A to A
                        range.Worksheet.Cells["A6:A8"].Merge = true;
                        range.Worksheet.Cells["A6:A8"].Value = "STT";
                        //Set column B to B
                        range.Worksheet.Cells["B6:B8"].Merge = true;
                        range.Worksheet.Cells["B6:B8"].Value = "Mã NB";
                        //Set column C to C
                        range.Worksheet.Cells["C6:C8"].Merge = true;
                        range.Worksheet.Cells["B6:C8"].Value = "Mã TN";
                        //Set column D to D
                        range.Worksheet.Cells["D6:D8"].Merge = true;
                        range.Worksheet.Cells["D6:D8"].Value = "Họ và Tên";
                        //Set column E to E
                        range.Worksheet.Cells["E6:E8"].Merge = true;
                        range.Worksheet.Cells["E6:E8"].Value = "Năm sinh";
                        //Set column F to F
                        range.Worksheet.Cells["F6:F8"].Merge = true;
                        range.Worksheet.Cells["F6:F8"].Value = "Giới tính";



                        #region DOANH THU DỊCH VỤ TRONG GÓI
                        //Set column F to Q
                        range.Worksheet.Cells["G6:R7"].Merge = true;
                        range.Worksheet.Cells["G6:R7"].Value = "DOANH THU DỊCH VỤ TRONG GÓI";

                        range.Worksheet.Cells["G8:G8"].Merge = true;
                        range.Worksheet.Cells["G8:G8"].Value = "Khám bệnh";



                        range.Worksheet.Cells["H8:H8"].Merge = true;
                        range.Worksheet.Cells["H8:H8"].Value = "Xét nghiệm";

                        range.Worksheet.Cells["I8:I8"].Merge = true;
                        range.Worksheet.Cells["I8:I8"].Value = "Nội soi";

                        range.Worksheet.Cells["J8:J8"].Merge = true;
                        range.Worksheet.Cells["J8:J8"].Value = "Nội soi TMH";

                        range.Worksheet.Cells["K8:K8"].Merge = true;
                        range.Worksheet.Cells["K8:K8"].Value = "Siêu âm";

                        range.Worksheet.Cells["L8:L8"].Merge = true;
                        range.Worksheet.Cells["L8:L8"].Value = "X-Quang";


                        range.Worksheet.Cells["M8:M8"].Merge = true;
                        range.Worksheet.Cells["M8:M8"].Value = "CT Scan";

                        range.Worksheet.Cells["N8:N8"].Merge = true;
                        range.Worksheet.Cells["N8:N8"].Value = "MRI";

                        range.Worksheet.Cells["O8:O8"].Merge = true;
                        range.Worksheet.Cells["O8:O8"].Value = "ĐiệnTim + Điện Não";

                        range.Worksheet.Cells["P8:P8"].Merge = true;
                        range.Worksheet.Cells["P8:P8"].Value = "TDCN + Đo loãng xương";

                        range.Worksheet.Cells["Q8:Q8"].Merge = true;
                        range.Worksheet.Cells["Q8:Q8"].Value = "DV khác";

                        range.Worksheet.Cells["R8:R8"].Merge = true;
                        range.Worksheet.Cells["R8:R8"].Value = "Tổng cộng";
                        #endregion
                        #region DOANH THU DỊCH VỤ PHÁT SINH NGOÀI GÓI
                        //Set column R to AE
                        range.Worksheet.Cells["S6:AF7"].Merge = true;
                        range.Worksheet.Cells["S6:AF7"].Value = "DOANH THU DỊCH VỤ PHÁT SINH NGOÀI GÓI";

                        range.Worksheet.Cells["S8:S8"].Merge = true;
                        range.Worksheet.Cells["S8:S8"].Value = "Khám bệnh";

                        range.Worksheet.Cells["T8:T8"].Merge = true;
                        range.Worksheet.Cells["T8:T8"].Value = "Xét nghiệm";

                        range.Worksheet.Cells["U8:U8"].Merge = true;
                        range.Worksheet.Cells["U8:U8"].Value = "Nội soi";

                        range.Worksheet.Cells["V8:V8"].Merge = true;
                        range.Worksheet.Cells["V8:V8"].Value = "Nội soi TMH";

                        range.Worksheet.Cells["W8:W8"].Merge = true;
                        range.Worksheet.Cells["W8:W8"].Value = "Siêu âm";

                        range.Worksheet.Cells["X8:X8"].Merge = true;
                        range.Worksheet.Cells["X8:X8"].Value = "X-Quang";


                        range.Worksheet.Cells["Y8:Y8"].Merge = true;
                        range.Worksheet.Cells["Y8:Y8"].Value = "CT Scan";

                        range.Worksheet.Cells["Z8:Z8"].Merge = true;
                        range.Worksheet.Cells["Z8:Z8"].Value = "MRI";

                        range.Worksheet.Cells["AA8:AA8"].Merge = true;
                        range.Worksheet.Cells["AA8:AA8"].Value = "ĐiệnTim + Điện Não";

                        range.Worksheet.Cells["AB8:AB8"].Merge = true;
                        range.Worksheet.Cells["AB8:AB8"].Value = "TDCN + Đo loãng xương";

                        range.Worksheet.Cells["AC8:AC8"].Merge = true;
                        range.Worksheet.Cells["AC8:AC8"].Value = "Thủ thuật";

                        range.Worksheet.Cells["AD8:AD8"].Merge = true;
                        range.Worksheet.Cells["AD8:AD8"].Value = "Phẩu thuật";

                        range.Worksheet.Cells["AE8:AE8"].Merge = true;
                        range.Worksheet.Cells["AE8:AE8"].Value = "DV khác";

                        range.Worksheet.Cells["AF8:AF8"].Merge = true;
                        range.Worksheet.Cells["AF8:AF8"].Value = "Tổng cộng";
                        #endregion

                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    var manager = new PropertyManager<BaoCaoDoanhThuKhamDoanTheoNhomDichVuThucTeGridVo>(requestProperties);


                    var dataBind = dataBaoCaos.GroupBy(d => d.TenCongTy).ToArray();
                    var dataTotal = dataBaoCaos.GroupBy(d => d.TenCongTy)
                        .Select(d => new
                        {
                            // trong gói
                            SumKhamBenh = d.Sum(f => f.SoTienDVKhamBenh),
                            SumXetNghiem = d.Sum(f => f.SoTienDVXetNghiem),
                            SumNoiSoi = d.Sum(f => f.SoTienDVThamDoChucNangNoiSoi),
                            SumNoiSoiTMH = d.Sum(f => f.SoTienDVTDCNNoiSoiTMH),
                            SumSieuAm = d.Sum(f => f.SoTienDVCDHASieuAm),
                            SumXQuang = d.Sum(f => f.SoTienDVCDHAXQuangThuongXQuangSoHoa),
                            SumCTScan = d.Sum(f => f.SoTienDVCTScan),
                            SumMRI = d.Sum(f => f.SoTienDVMRI),
                            SumDienTimDienNao = d.Sum(f => f.SoTienDVDienTimDienNao),
                            SumTDCNDoLoangXuong = d.Sum(f => f.SoTienDVTDCNDoLoangXuong),
                            SumDVKhac = d.Sum(f => f.SoTienDVKhac),
                            SumTongCong = d.Sum(f => f.TongCong),
                            // ngoài gói
                            SumKhamBenhNG = d.Sum(f => f.SoTienDVKhamBenhNG),
                            SumXetNghiemNG = d.Sum(f => f.SoTienDVXetNghiemNG),
                            SumNoiSoiNG = d.Sum(f => f.SoTienDVThamDoChucNangNoiSoiNG),
                            SumNoiSoiTMHNG = d.Sum(f => f.SoTienDVTDCNNoiSoiTMHNG),
                            SumSieuAmNG = d.Sum(f => f.SoTienDVCDHASieuAmNG),
                            SumXQuangNG = d.Sum(f => f.SoTienDVCDHAXQuangThuongXQuangSoHoaNG),
                            SumCTScanNG = d.Sum(f => f.SoTienDVCTScanNG),
                            SumMRING = d.Sum(f => f.SoTienDVMRING),
                            SumDienTimDienNaoNG = d.Sum(f => f.SoTienDVDienTimDienNaoNG),
                            SumTDCNDoLoangXuongNG = d.Sum(f => f.SoTienDVTDCNDoLoangXuongNG),
                            SumDVKhacNG = d.Sum(f => f.SoTienDVKhacNG),
                            SumTongCongNG = d.Sum(f => f.TongCongNG),
                            TenCongTy = d.First().TenCongTy,
                            SumThuThuat = d.Sum(f => f.SoTienDVThuThuatNG),
                            SumPhauThuat = d.Sum(f => f.SoTienDVPhauThuatNG)
                        }).ToArray();


                    int index = 9;
                    int lengthIndexArray = 0;
                    foreach (var itemgroup in dataBind)
                    {
                        worksheet.Cells["A" + index + ":" + "F" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["A" + index + ":" + "F" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["A" + index + ":" + "F" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["A" + index + ":" + "F" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["A" + index + ":" + "F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells["A" + index + ":" + "F" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));

                        worksheet.Cells["A" + index + ":" + "F" + index].Style.Font.Color.SetColor(Color.Black);
                        worksheet.Cells["A" + index + ":" + "F" + index].Style.Font.Bold = true;
                        worksheet.Cells["A" + index + ":" + "F" + index].Merge = true;
                        worksheet.Cells["A" + index + ":" + "F" + index].Value = dataTotal[lengthIndexArray].TenCongTy;

                        for (int ii = 0; ii < SetColumnItems.Length; ii++)
                        {
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Color.SetColor(Color.Black);
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Bold = true;
                        }

                        #region dịch vụ trong gói
                        worksheet.Cells["G" + index + ":" + "G" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["G" + index + ":" + "G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        worksheet.Cells["H" + index + ":" + "H" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["H" + index + ":" + "H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        worksheet.Cells["I" + index + ":" + "I" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["I" + index + ":" + "I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        worksheet.Cells["J" + index + ":" + "J" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["J" + index + ":" + "J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        worksheet.Cells["K" + index + ":" + "K" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["K" + index + ":" + "K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        worksheet.Cells["L" + index + ":" + "L" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["L" + index + ":" + "L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        worksheet.Cells["M" + index + ":" + "M" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["M" + index + ":" + "M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        worksheet.Cells["N" + index + ":" + "N" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["N" + index + ":" + "N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        worksheet.Cells["O" + index + ":" + "O" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["O" + index + ":" + "O" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        worksheet.Cells["P" + index + ":" + "P" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["P" + index + ":" + "P" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        worksheet.Cells["Q" + index + ":" + "Q" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["Q" + index + ":" + "Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        worksheet.Cells["R" + index + ":" + "R" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["R" + index + ":" + "R" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;


                        worksheet.Cells["G" + index + ":" + "G" + index].Value = dataTotal[lengthIndexArray].SumKhamBenh;
                        worksheet.Cells["H" + index + ":" + "H" + index].Value = dataTotal[lengthIndexArray].SumXetNghiem;
                        worksheet.Cells["I" + index + ":" + "I" + index].Value = dataTotal[lengthIndexArray].SumNoiSoi;
                        worksheet.Cells["J" + index + ":" + "J" + index].Value = dataTotal[lengthIndexArray].SumNoiSoiTMH;
                        worksheet.Cells["K" + index + ":" + "K" + index].Value = dataTotal[lengthIndexArray].SumSieuAm;
                        worksheet.Cells["L" + index + ":" + "L" + index].Value = dataTotal[lengthIndexArray].SumXQuang;
                        worksheet.Cells["M" + index + ":" + "M" + index].Value = dataTotal[lengthIndexArray].SumCTScan;
                        worksheet.Cells["N" + index + ":" + "N" + index].Value = dataTotal[lengthIndexArray].SumMRI;
                        worksheet.Cells["O" + index + ":" + "O" + index].Value = dataTotal[lengthIndexArray].SumDienTimDienNao;
                        worksheet.Cells["P" + index + ":" + "P" + index].Value = dataTotal[lengthIndexArray].SumTDCNDoLoangXuong;
                        worksheet.Cells["Q" + index + ":" + "Q" + index].Value = dataTotal[lengthIndexArray].SumDVKhac;
                        worksheet.Cells["R" + index + ":" + "R" + index].Value = dataTotal[lengthIndexArray].SumTongCong;
                        #endregion

                        #region DỊCH VỤ NGOÀI GÓI
                        worksheet.Cells["S" + index + ":" + "S" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["T" + index + ":" + "T" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["U" + index + ":" + "U" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["V" + index + ":" + "V" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["W" + index + ":" + "W" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["X" + index + ":" + "X" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["Y" + index + ":" + "Y" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["Z" + index + ":" + "Z" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["AA" + index + ":" + "AA" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["AB" + index + ":" + "AB" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["AC" + index + ":" + "AC" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["AD" + index + ":" + "AD" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["AE" + index + ":" + "AE" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["AF" + index + ":" + "AF" + index].Style.Numberformat.Format = "#,##0.00";



                        worksheet.Cells["S" + index + ":" + "S" + index].Value = dataTotal[lengthIndexArray].SumKhamBenhNG;
                        worksheet.Cells["T" + index + ":" + "T" + index].Value = dataTotal[lengthIndexArray].SumXetNghiemNG;
                        worksheet.Cells["U" + index + ":" + "U" + index].Value = dataTotal[lengthIndexArray].SumNoiSoiNG;
                        worksheet.Cells["V" + index + ":" + "V" + index].Value = dataTotal[lengthIndexArray].SumNoiSoiTMHNG;
                        worksheet.Cells["W" + index + ":" + "W" + index].Value = dataTotal[lengthIndexArray].SumSieuAmNG;
                        worksheet.Cells["X" + index + ":" + "X" + index].Value = dataTotal[lengthIndexArray].SumXQuangNG;
                        worksheet.Cells["Y" + index + ":" + "Y" + index].Value = dataTotal[lengthIndexArray].SumCTScanNG;
                        worksheet.Cells["Z" + index + ":" + "Z" + index].Value = dataTotal[lengthIndexArray].SumMRING;
                        worksheet.Cells["AA" + index + ":" + "AA" + index].Value = dataTotal[lengthIndexArray].SumDienTimDienNaoNG;
                        worksheet.Cells["AB" + index + ":" + "AB" + index].Value = dataTotal[lengthIndexArray].SumTDCNDoLoangXuongNG;
                        worksheet.Cells["AC" + index + ":" + "AC" + index].Value = dataTotal[lengthIndexArray].SumThuThuat;
                        worksheet.Cells["AD" + index + ":" + "AD" + index].Value = dataTotal[lengthIndexArray].SumPhauThuat;
                        worksheet.Cells["AE" + index + ":" + "AE" + index].Value = dataTotal[lengthIndexArray].SumDVKhacNG;
                        worksheet.Cells["AF" + index + ":" + "AF" + index].Value = dataTotal[lengthIndexArray].SumTongCongNG;
                        #endregion
                        index++;
                        int Stt = 1;
                        foreach (var item in itemgroup.ToList())
                        {
                            worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["A" + index].Value = Stt;

                            worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["B" + index].Value = item.MaNB;

                            worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["C" + index].Value = item.MaTN;

                            worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["D" + index].Value = item.HoVaTen;

                            worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["E" + index].Value = item.NamSinh;

                            worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["F" + index].Value = item.GioiTinh;

                            worksheet.Cells["A" + index + ":" + "D" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                            for (int ii = 0; ii < SetColumnItems.Length; ii++)
                            {
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Color.SetColor(Color.Black);
                                //worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Bold = true;
                            }
                            #region DỊCH VỤ TRONG GÓI
                            #region css
                            worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["O" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["P" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["R" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            worksheet.Cells["G" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["I" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["O" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["P" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["Q" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["R" + index].Style.Numberformat.Format = "#,##0.00";
                            #endregion css
                            #region bind item
                            worksheet.Cells["G" + index].Value = SoTien(item.SoTienDVKhamBenh);
                            worksheet.Cells["H" + index].Value = SoTien(item.SoTienDVXetNghiem);
                            worksheet.Cells["I" + index].Value = SoTien(item.SoTienDVThamDoChucNangNoiSoi);
                            worksheet.Cells["J" + index].Value = SoTien(item.SoTienDVTDCNNoiSoiTMH);
                            worksheet.Cells["K" + index].Value = SoTien(item.SoTienDVCDHASieuAm);
                            worksheet.Cells["L" + index].Value = SoTien(item.SoTienDVCDHAXQuangThuongXQuangSoHoa);
                            worksheet.Cells["M" + index].Value = SoTien(item.SoTienDVCTScan);
                            worksheet.Cells["N" + index].Value = SoTien(item.SoTienDVMRI);
                            worksheet.Cells["O" + index].Value = SoTien(item.SoTienDVDienTimDienNao);
                            worksheet.Cells["P" + index].Value = SoTien(item.SoTienDVTDCNDoLoangXuong);
                            worksheet.Cells["Q" + index].Value = SoTien(item.SoTienDVKhac);
                            worksheet.Cells["R" + index].Value = SoTien(item.TongCong);
                            #endregion bind item
                            #endregion
                            #region DỊCH VỤ ngoài GÓI
                            #region css
                            worksheet.Cells["S" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["T" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["U" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["V" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["W" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["X" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["Y" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["Z" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["AA" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["AB" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["AC" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["AD" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["AE" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["AF" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            worksheet.Cells["S" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["T" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["U" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["V" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["W" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["X" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["Y" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["Z" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["AA" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["AB" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["AC" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["AD" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["AE" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["AF" + index].Style.Numberformat.Format = "#,##0.00";
                            #endregion  css
                            #region bind item 
                            worksheet.Cells["S" + index].Value = SoTien(item.SoTienDVKhamBenhNG);
                            worksheet.Cells["T" + index].Value = SoTien(item.SoTienDVXetNghiemNG);
                            worksheet.Cells["U" + index].Value = SoTien(item.SoTienDVThamDoChucNangNoiSoiNG);
                            worksheet.Cells["V" + index].Value = SoTien(item.SoTienDVTDCNNoiSoiTMHNG);
                            worksheet.Cells["W" + index].Value = SoTien(item.SoTienDVCDHASieuAmNG);
                            worksheet.Cells["X" + index].Value = SoTien(item.SoTienDVCDHAXQuangThuongXQuangSoHoaNG);
                            worksheet.Cells["Y" + index].Value = SoTien(item.SoTienDVCTScanNG);
                            worksheet.Cells["Z" + index].Value = SoTien(item.SoTienDVMRING);
                            worksheet.Cells["AA" + index].Value = SoTien(item.SoTienDVDienTimDienNaoNG);
                            worksheet.Cells["AB" + index].Value = SoTien(item.SoTienDVTDCNDoLoangXuongNG);
                            worksheet.Cells["AC" + index].Value = SoTien(item.SoTienDVThuThuatNG);
                            worksheet.Cells["AD" + index].Value = SoTien(item.SoTienDVPhauThuatNG);
                            worksheet.Cells["AE" + index].Value = SoTien(item.SoTienDVKhacNG);
                            worksheet.Cells["AF" + index].Value = SoTien(item.TongCongNG);
                            #endregion item
                            #endregion
                            index++;
                            Stt++;

                        }
                        lengthIndexArray++;

                    }

                    var worksheetFirstLast = SetColumnItems[0] + index + ":" + SetColumnItems[SetColumnItems.Length - 1] + index;

                    worksheet.Cells[worksheetFirstLast].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                    worksheet.Cells[worksheetFirstLast].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells[worksheetFirstLast].Style.Font.Bold = true;
                    worksheet.Cells[worksheetFirstLast].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                    worksheet.Cells[worksheetFirstLast].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    for (int ii = 0; ii < SetColumnItems.Length; ii++)
                    {
                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Color.SetColor(Color.Black);
                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Bold = true;
                    }

                    // total grid
                    #region dịch vụ trong gói
                    var totalSoTienDVKhamBenh = bc.Sum(d => d.SoTienDVKhamBenh);
                    var totalSoTienDVXetNghiem = bc.Sum(d => d.SoTienDVXetNghiem);
                    var totalSoTienDVThamDoChucNangNoiSoi = bc.Sum(d => d.SoTienDVThamDoChucNangNoiSoi);
                    var totalSoTienDVTDCNNoiSoiTMH = bc.Sum(d => d.SoTienDVTDCNNoiSoiTMH);
                    var totalSoTienDVCDHASieuAm = bc.Sum(d => d.SoTienDVCDHASieuAm);
                    var totalSoTienDVCDHAXQuangThuongXQuangSoHoa = bc.Sum(d => d.SoTienDVCDHAXQuangThuongXQuangSoHoa);
                    var totalSoTienDVCTScan = bc.Sum(d => d.SoTienDVCTScan);
                    var totalSoTienDVMRI = bc.Sum(d => d.SoTienDVMRI);
                    var totalSoTienDVDienTimDienNao = bc.Sum(d => d.SoTienDVDienTimDienNao);
                    var totalSoTienDVTDCNDoLoangXuong = bc.Sum(d => d.SoTienDVTDCNDoLoangXuong);
                    var totalSoTienDVKhac = bc.Sum(d => d.SoTienDVKhac);
                    var totalTongCong = bc.Sum(d => d.TongCong);
                    #endregion
                    #region ngoài gói
                    var totalSoTienDVKhamBenhNG = bc.Sum(d => d.SoTienDVKhamBenhNG);
                    var totalSoTienDVXetNghiemNG = bc.Sum(d => d.SoTienDVXetNghiemNG);
                    var totalSoTienDVThamDoChucNangNoiSoiNG = bc.Sum(d => d.SoTienDVThamDoChucNangNoiSoiNG);
                    var totalSoTienDVTDCNNoiSoiTMHNG = bc.Sum(d => d.SoTienDVTDCNNoiSoiTMHNG);
                    var totalSoTienDVCDHASieuAmNG = bc.Sum(d => d.SoTienDVCDHASieuAmNG);
                    var totalSoTienDVCDHAXQuangThuongXQuangSoHoaNG = bc.Sum(d => d.SoTienDVCDHAXQuangThuongXQuangSoHoaNG);
                    var totalSoTienDVCTScanNG = bc.Sum(d => d.SoTienDVCTScanNG);
                    var totalSoTienDVMRING = bc.Sum(d => d.SoTienDVMRING);
                    var totalSoTienDVDienTimDienNaoNG = bc.Sum(d => d.SoTienDVDienTimDienNaoNG);
                    var totalSoTienDVTDCNDoLoangXuongNG = bc.Sum(d => d.SoTienDVTDCNDoLoangXuongNG);
                    var totalSoTienDVThuThuatNG = bc.Sum(d => d.SoTienDVThuThuatNG);
                    var totalSoTienDVPhauThuatNG = bc.Sum(d => d.SoTienDVPhauThuatNG);
                    var totalSoTienDVKhacNG = bc.Sum(d => d.SoTienDVKhacNG);
                    var totalTongCongNG = bc.Sum(d => d.TongCongNG);
                    #endregion

                    using (var range = worksheet.Cells["A" + index + ":F" + index])
                    {
                        range.Worksheet.Cells["A" + index + ":F" + index].Merge = true;
                        range.Worksheet.Cells["A" + index + ":F" + index].Value = "Tổng cộng";
                    }
                    // STYLE 
                    // FOR MAT 
                    #region trong gói
                    worksheet.Cells["G" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["I" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["O" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["P" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["Q" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["R" + index].Style.Numberformat.Format = "#,##0.00";
                    #endregion
                    #region ngoài gói
                    worksheet.Cells["S" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["T" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["U" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["V" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["W" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["X" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["Y" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["Z" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["AA" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["AB" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["AC" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["AD" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["AE" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["AF" + index].Style.Numberformat.Format = "#,##0.00";
                    #endregion
                    //
                    worksheet.Cells["G" + index].Value = totalSoTienDVKhamBenh;
                    worksheet.Cells["H" + index].Value = totalSoTienDVXetNghiem;
                    worksheet.Cells["I" + index].Value = totalSoTienDVThamDoChucNangNoiSoi;
                    worksheet.Cells["J" + index].Value = totalSoTienDVTDCNNoiSoiTMH;
                    worksheet.Cells["K" + index].Value = totalSoTienDVCDHASieuAm;
                    worksheet.Cells["L" + index].Value = totalSoTienDVCDHAXQuangThuongXQuangSoHoa;
                    worksheet.Cells["M" + index].Value = totalSoTienDVCTScan;
                    worksheet.Cells["N" + index].Value = totalSoTienDVMRI;
                    worksheet.Cells["O" + index].Value = totalSoTienDVDienTimDienNao;
                    worksheet.Cells["P" + index].Value = totalSoTienDVTDCNDoLoangXuong;
                    worksheet.Cells["Q" + index].Value = totalSoTienDVKhac;
                    worksheet.Cells["R" + index].Value = totalTongCong;

                    // ngoài gói
                    worksheet.Cells["S" + index].Value = totalSoTienDVKhamBenhNG;
                    worksheet.Cells["T" + index].Value = totalSoTienDVXetNghiemNG;
                    worksheet.Cells["U" + index].Value = totalSoTienDVThamDoChucNangNoiSoiNG;
                    worksheet.Cells["V" + index].Value = totalSoTienDVTDCNNoiSoiTMHNG;
                    worksheet.Cells["W" + index].Value = totalSoTienDVCDHASieuAmNG;
                    worksheet.Cells["X" + index].Value = totalSoTienDVCDHAXQuangThuongXQuangSoHoaNG;
                    worksheet.Cells["Y" + index].Value = totalSoTienDVCTScanNG;
                    worksheet.Cells["Z" + index].Value = totalSoTienDVMRING;
                    worksheet.Cells["AA" + index].Value = totalSoTienDVDienTimDienNaoNG;
                    worksheet.Cells["AB" + index].Value = totalSoTienDVTDCNDoLoangXuongNG;
                    worksheet.Cells["AC" + index].Value = totalSoTienDVThuThuatNG;
                    worksheet.Cells["AD" + index].Value = totalSoTienDVPhauThuatNG;
                    worksheet.Cells["AE" + index].Value = totalSoTienDVKhacNG;
                    worksheet.Cells["AF" + index].Value = totalTongCongNG;
                    xlPackage.Save();
                }

                return stream.ToArray();
            }

        }

        private decimal? SoTien(decimal? soTien)
        {
            return soTien;
        }

    }
}
