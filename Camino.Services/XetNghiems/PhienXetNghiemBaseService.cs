using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.DichVuXetNghiems;
using Camino.Core.Domain.Entities.XetNghiems;
using Camino.Core.Domain.ValueObject.XetNghiems;
using Camino.Data;
using Camino.Services.CauHinh;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Microsoft.EntityFrameworkCore;
namespace Camino.Services.XetNghiems
{
    public class PhienXetNghiemBaseService : MasterFileService<PhienXetNghiem>, IPhienXetNghiemBaseService
    {
        protected readonly IUserAgentHelper _userAgentHelper;
        protected readonly ICauHinhService _cauHinhService;
        protected readonly ILocalizationService _localizationService;
        private IRepository<PhienXetNghiem> repository;

        public PhienXetNghiemBaseService(IRepository<PhienXetNghiem> phienXetNghiemRepository, IUserAgentHelper userAgentHelper, ICauHinhService cauHinhService, ILocalizationService localizationService) 
            : base(phienXetNghiemRepository)
        {
            _userAgentHelper = userAgentHelper;
            _cauHinhService = cauHinhService;
            _localizationService = localizationService;
        }

        public async Task DuyetPhieuGuiMauXetNghiem(long phieuGuiMauXetNghiemId, long nhanVienNhanMauId)
        {
            var phieuGuiMauXetNghiem = BaseRepository.Context.Set<PhieuGoiMauXetNghiem>()
                .Include(o=>o.MauXetNghiems).ThenInclude(o=>o.PhienXetNghiem).ThenInclude(o=>o.PhienXetNghiemChiTiets).ThenInclude(o => o.KetQuaXetNghiemChiTiets)
                .Include(o => o.MauXetNghiems).ThenInclude(o => o.PhienXetNghiem).ThenInclude(o => o.PhienXetNghiemChiTiets).ThenInclude(o => o.DichVuKyThuatBenhVien)
                .Include(o => o.MauXetNghiems).ThenInclude(o => o.PhienXetNghiem).ThenInclude(o => o.BenhNhan)
                .FirstOrDefault(o => o.Id == phieuGuiMauXetNghiemId && o.DaNhanMau != true);
            if (phieuGuiMauXetNghiem != null)
            {
                var dichVuXetNghiems =await BaseRepository.Context.Set<Core.Domain.Entities.DichVuXetNghiems.DichVuXetNghiem>().AsNoTracking().ToListAsync();
                var dichVuXetNghiemKetNoiChiSos = await BaseRepository.Context.Set<DichVuXetNghiemKetNoiChiSo>().AsNoTracking().ToListAsync();
                phieuGuiMauXetNghiem.NhanVienNhanMauId = nhanVienNhanMauId;
                phieuGuiMauXetNghiem.ThoiDiemNhanMau = DateTime.Now;
                phieuGuiMauXetNghiem.DaNhanMau = true;

                var ketQuaXetNghiemTruocs = GetKetQuaXetNghiemTruocCuaBenhNhan(phieuGuiMauXetNghiem.MauXetNghiems.FirstOrDefault()?.PhienXetNghiem.BenhNhanId ?? 0);

                foreach (var groupPhienXetNghiem in phieuGuiMauXetNghiem.MauXetNghiems.GroupBy(o=>o.PhienXetNghiemId))
                {
                    var phienXetNghiem = groupPhienXetNghiem.First().PhienXetNghiem;
                    foreach (var groupNhomDichVu in groupPhienXetNghiem.GroupBy(o=>o.NhomDichVuBenhVienId))
                    {
                        foreach (var phienXetNghiemChiTiet in phienXetNghiem.PhienXetNghiemChiTiets)
                        {
                            if (phienXetNghiemChiTiet.NhomDichVuBenhVienId == groupNhomDichVu.Key && !phienXetNghiemChiTiet.KetQuaXetNghiemChiTiets.Any() && phienXetNghiemChiTiet.DichVuKyThuatBenhVien.DichVuXetNghiemId != null && phienXetNghiemChiTiet.ThoiDiemNhanMau != null)
                            {
                                AddKetQuaXetNghiemChiTiet(phienXetNghiemChiTiet,
                                    dichVuXetNghiems.First(o => o.Id == phienXetNghiemChiTiet.DichVuKyThuatBenhVien.DichVuXetNghiemId),
                                    phienXetNghiem.BenhNhan, dichVuXetNghiems, dichVuXetNghiemKetNoiChiSos, ketQuaXetNghiemTruocs);
                                //update 02/11 xử lý ds phiên XN
                                if (phienXetNghiem.ChoKetQua == null)
                                {
                                    phienXetNghiem.ChoKetQua = true;
                                }
                            }
                        }
                    }
                }
            }
            await BaseRepository.Context.SaveChangesAsync();
        }

        public async Task DuyetYeuCauChayLaiXetNghiem(long yeuCauChayLaiXetNghiemId, long nhanVienDuyetId)
        {
            var yeuCauChayLaiXetNghiem = BaseRepository.Context.Set<YeuCauChayLaiXetNghiem>()
                .Include(o => o.PhienXetNghiem).ThenInclude(o => o.PhienXetNghiemChiTiets).ThenInclude(o => o.KetQuaXetNghiemChiTiets)
                .Include(o => o.PhienXetNghiem).ThenInclude(o => o.PhienXetNghiemChiTiets).ThenInclude(o => o.DichVuKyThuatBenhVien)
                .Include(o => o.PhienXetNghiem).ThenInclude(o => o.BenhNhan)
                .FirstOrDefault(o => o.Id == yeuCauChayLaiXetNghiemId && o.DuocDuyet == null);
            if (yeuCauChayLaiXetNghiem != null)
            {
                var dichVuXetNghiems = await BaseRepository.Context.Set<Core.Domain.Entities.DichVuXetNghiems.DichVuXetNghiem>().AsNoTracking().ToListAsync();
                var dichVuXetNghiemKetNoiChiSos = await BaseRepository.Context.Set<DichVuXetNghiemKetNoiChiSo>().AsNoTracking().ToListAsync();

                yeuCauChayLaiXetNghiem.NhanVienDuyetId = nhanVienDuyetId;
                yeuCauChayLaiXetNghiem.NgayDuyet = DateTime.Now;
                yeuCauChayLaiXetNghiem.DuocDuyet = true;

                var phienXetNghiemChiTiets = yeuCauChayLaiXetNghiem.PhienXetNghiem.PhienXetNghiemChiTiets.Where(o =>o.NhomDichVuBenhVienId == yeuCauChayLaiXetNghiem.NhomDichVuBenhVienId);
                var lanXetNghiemTruoc = phienXetNghiemChiTiets.GroupBy(o => o.LanThucHien).OrderBy(o => o.Key).Last();
                foreach (var phienXetNghiemChiTiet in lanXetNghiemTruoc)
                {
                    var phienXetNghiemChiTietMoi = new PhienXetNghiemChiTiet
                    {
                        NhomDichVuBenhVienId = phienXetNghiemChiTiet.NhomDichVuBenhVienId,
                        YeuCauDichVuKyThuatId = phienXetNghiemChiTiet.YeuCauDichVuKyThuatId,
                        DichVuKyThuatBenhVienId = phienXetNghiemChiTiet.DichVuKyThuatBenhVienId,
                        NhanVienLayMauId = phienXetNghiemChiTiet.NhanVienLayMauId,
                        PhongLayMauId = phienXetNghiemChiTiet.PhongLayMauId,
                        ThoiDiemLayMau = phienXetNghiemChiTiet.ThoiDiemLayMau,
                        NhanVienNhanMauId = phienXetNghiemChiTiet.NhanVienNhanMauId,
                        PhongNhanMauId = phienXetNghiemChiTiet.PhongNhanMauId,
                        ThoiDiemNhanMau = phienXetNghiemChiTiet.ThoiDiemNhanMau,
                        LanThucHien = phienXetNghiemChiTiet.LanThucHien + 1,
                        ChayLaiKetQua = true,
                        YeuCauChayLaiXetNghiemId = yeuCauChayLaiXetNghiemId,
                        PhienXetNghiem = phienXetNghiemChiTiet.PhienXetNghiem
                    };
                    if (phienXetNghiemChiTiet.DichVuKyThuatBenhVien.DichVuXetNghiemId != null && phienXetNghiemChiTiet.ThoiDiemNhanMau != null)
                    {
                        var ketQuaXetNghiemTruocVos = phienXetNghiemChiTiet.KetQuaXetNghiemChiTiets
                            .Select(o => new KetQuaXetNghiemTruocVo
                            {
                                ThoiDiemKetLuan = o.PhienXetNghiemChiTiet.ThoiDiemKetLuan,
                                DichVuXetNghiemId = o.DichVuXetNghiemId,
                                GiaTriTuMay = o.GiaTriTuMay,
                                GiaTriNhapTay = o.GiaTriNhapTay,
                                GiaTriDuyet = o.GiaTriDuyet
                            }).ToList();

                        AddKetQuaXetNghiemChiTiet(phienXetNghiemChiTietMoi,
                            dichVuXetNghiems.First(o => o.Id == phienXetNghiemChiTiet.DichVuKyThuatBenhVien.DichVuXetNghiemId),
                            yeuCauChayLaiXetNghiem.PhienXetNghiem.BenhNhan, dichVuXetNghiems, dichVuXetNghiemKetNoiChiSos, ketQuaXetNghiemTruocVos);
                        //update 02/11 xử lý ds phiên XN
                        if (yeuCauChayLaiXetNghiem.PhienXetNghiem.ChoKetQua == null)
                        {
                            yeuCauChayLaiXetNghiem.PhienXetNghiem.ChoKetQua = true;
                        }
                    }
                    yeuCauChayLaiXetNghiem.PhienXetNghiem.PhienXetNghiemChiTiets.Add(phienXetNghiemChiTietMoi);
                }
                yeuCauChayLaiXetNghiem.PhienXetNghiem.ThoiDiemKetLuan = null;
                yeuCauChayLaiXetNghiem.PhienXetNghiem.NhanVienKetLuanId = null;
            }
            await BaseRepository.Context.SaveChangesAsync();
        }

        public List<KetQuaXetNghiemTruocVo> GetKetQuaXetNghiemTruocCuaBenhNhan(long benhNhanId)
        {
            return BaseRepository.TableNoTracking.Where(o => o.BenhNhanId == benhNhanId)
                .SelectMany(o => o.PhienXetNghiemChiTiets).Where(o => o.ThoiDiemKetLuan != null)
                .SelectMany(o => o.KetQuaXetNghiemChiTiets)
                .Select(o => new KetQuaXetNghiemTruocVo
                {
                    ThoiDiemKetLuan = o.PhienXetNghiemChiTiet.ThoiDiemKetLuan,
                    DichVuXetNghiemId = o.DichVuXetNghiemId,
                    GiaTriTuMay = o.GiaTriTuMay,
                    GiaTriNhapTay = o.GiaTriNhapTay,
                    GiaTriDuyet = o.GiaTriDuyet
                }).ToList();
        }

        public void AddKetQuaXetNghiemChiTiet(PhienXetNghiemChiTiet phienXetNghiemChiTiet, Core.Domain.Entities.DichVuXetNghiems.DichVuXetNghiem dichVuXetNghiem, BenhNhan benhNhan, List<Core.Domain.Entities.DichVuXetNghiems.DichVuXetNghiem> dichVuXetNghiems, List<DichVuXetNghiemKetNoiChiSo> dichVuXetNghiemKetNoiChiSos, List<KetQuaXetNghiemTruocVo> ketQuaXetNghiemLanTruocs = null)
        {
            var ketQuaXetNghiemChiTiet = new KetQuaXetNghiemChiTiet
            {
                BarCodeID = phienXetNghiemChiTiet.PhienXetNghiem.BarCodeId,
                BarCodeNumber = phienXetNghiemChiTiet.PhienXetNghiem.BarCodeNumber,
                YeuCauDichVuKyThuatId = phienXetNghiemChiTiet.YeuCauDichVuKyThuatId,
                DichVuKyThuatBenhVienId = phienXetNghiemChiTiet.DichVuKyThuatBenhVienId,
                NhomDichVuBenhVienId = phienXetNghiemChiTiet.NhomDichVuBenhVienId,
                LanThucHien = phienXetNghiemChiTiet.LanThucHien,
                DichVuXetNghiemId = dichVuXetNghiem.Id,
                DichVuXetNghiemChaId = dichVuXetNghiem.DichVuXetNghiemChaId,
                DichVuXetNghiemMa = dichVuXetNghiem.Ma,
                DichVuXetNghiemTen = dichVuXetNghiem.Ten,
                CapDichVu = dichVuXetNghiem.CapDichVu,
                DonVi = dichVuXetNghiem.DonVi,
                SoThuTu = dichVuXetNghiem.SoThuTu,
                ThoiDiemGuiYeuCau = DateTime.Now,
            };

            var ketQuaCu = ketQuaXetNghiemLanTruocs
                ?.Where(o => o.DichVuXetNghiemId == ketQuaXetNghiemChiTiet.DichVuXetNghiemId)
                .OrderBy(o => o.ThoiDiemKetLuan).LastOrDefault();
            if (ketQuaCu != null)
            {
                ketQuaXetNghiemChiTiet.GiaTriCu = string.IsNullOrEmpty(ketQuaCu.GiaTriDuyet)
                    ? (string.IsNullOrEmpty(ketQuaCu.GiaTriNhapTay) ? ketQuaCu.GiaTriTuMay : ketQuaCu.GiaTriNhapTay)
                    : ketQuaCu.GiaTriDuyet;
            }

            var dichVuXetNghiemKetNoiChiSo = dichVuXetNghiemKetNoiChiSos
                .Where(o => o.DichVuXetNghiemId == dichVuXetNghiem.Id && o.HieuLuc).OrderBy(o => o.Id).LastOrDefault();
            if (dichVuXetNghiemKetNoiChiSo != null)
            {
                ketQuaXetNghiemChiTiet.DichVuXetNghiemKetNoiChiSoId = dichVuXetNghiemKetNoiChiSo.Id;
                ketQuaXetNghiemChiTiet.MaChiSo = dichVuXetNghiemKetNoiChiSo.MaChiSo;
                ketQuaXetNghiemChiTiet.TiLe = dichVuXetNghiemKetNoiChiSo.TiLe;
                ketQuaXetNghiemChiTiet.MauMayXetNghiemId = dichVuXetNghiemKetNoiChiSo.MauMayXetNghiemId;
            }

            if (benhNhan.NamSinh != null)
            {
                var ngaySinh = new DateTime(benhNhan.NamSinh.Value, benhNhan.ThangSinh == null || benhNhan.ThangSinh == 0 ? 1 : benhNhan.ThangSinh.Value, benhNhan.NgaySinh == null || benhNhan.NgaySinh == 0 ? 1 : benhNhan.NgaySinh.Value);
                int tuoi = DateTime.Now.Year - ngaySinh.Year;
                if (ngaySinh > DateTime.Now.AddYears(-tuoi)) tuoi--;

                if (tuoi < 6 && (!string.IsNullOrEmpty(dichVuXetNghiem.TreEm6Min) || !string.IsNullOrEmpty(dichVuXetNghiem.TreEm6Max)))
                {
                    ketQuaXetNghiemChiTiet.GiaTriMin = dichVuXetNghiem.TreEm6Min;
                    ketQuaXetNghiemChiTiet.GiaTriMax = dichVuXetNghiem.TreEm6Max;
                }
                else if (tuoi >= 6 && tuoi < 12 && (!string.IsNullOrEmpty(dichVuXetNghiem.TreEm612Min) || !string.IsNullOrEmpty(dichVuXetNghiem.TreEm612Max)))
                {
                    ketQuaXetNghiemChiTiet.GiaTriMin = dichVuXetNghiem.TreEm612Min;
                    ketQuaXetNghiemChiTiet.GiaTriMax = dichVuXetNghiem.TreEm612Max;
                }
                else if (tuoi >= 12 && tuoi < 18 && (!string.IsNullOrEmpty(dichVuXetNghiem.TreEm1218Min) || !string.IsNullOrEmpty(dichVuXetNghiem.TreEm1218Max)))
                {
                    ketQuaXetNghiemChiTiet.GiaTriMin = dichVuXetNghiem.TreEm1218Min;
                    ketQuaXetNghiemChiTiet.GiaTriMax = dichVuXetNghiem.TreEm1218Max;
                }
                else if (tuoi < 18 && (!string.IsNullOrEmpty(dichVuXetNghiem.TreEmMin) || !string.IsNullOrEmpty(dichVuXetNghiem.TreEmMax)))
                {
                    ketQuaXetNghiemChiTiet.GiaTriMin = dichVuXetNghiem.TreEmMin;
                    ketQuaXetNghiemChiTiet.GiaTriMax = dichVuXetNghiem.TreEmMax;
                }
                else
                {
                    if (benhNhan.GioiTinh == Enums.LoaiGioiTinh.GioiTinhNam)
                    {
                        ketQuaXetNghiemChiTiet.GiaTriMin = dichVuXetNghiem.NamMin;
                        ketQuaXetNghiemChiTiet.GiaTriMax = dichVuXetNghiem.NamMax;
                    }
                    else if (benhNhan.GioiTinh == Enums.LoaiGioiTinh.GioiTinhNu)
                    {
                        ketQuaXetNghiemChiTiet.GiaTriMin = dichVuXetNghiem.NuMin;
                        ketQuaXetNghiemChiTiet.GiaTriMax = dichVuXetNghiem.NuMax;
                    }
                }
            }
            ketQuaXetNghiemChiTiet.GiaTriNguyHiemMin = dichVuXetNghiem.NguyHiemMin;
            ketQuaXetNghiemChiTiet.GiaTriNguyHiemMax = dichVuXetNghiem.NguyHiemMax;

            phienXetNghiemChiTiet.KetQuaXetNghiemChiTiets.Add(ketQuaXetNghiemChiTiet);
            //update 07/15/2021: tự động gửi duyệt
            phienXetNghiemChiTiet.DaGoiDuyet = true;
            foreach (var dichVuXetNghiemCon in dichVuXetNghiems.Where(o=>o.DichVuXetNghiemChaId == dichVuXetNghiem.Id))
            {
                AddKetQuaXetNghiemChiTiet(phienXetNghiemChiTiet,dichVuXetNghiemCon,benhNhan,dichVuXetNghiems,dichVuXetNghiemKetNoiChiSos,ketQuaXetNghiemLanTruocs);
            }
        }
    }
}
