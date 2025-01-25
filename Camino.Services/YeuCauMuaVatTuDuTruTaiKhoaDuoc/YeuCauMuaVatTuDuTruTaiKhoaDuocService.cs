using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.DuTruVatTus;
using Camino.Core.Domain.ValueObject.DuTruMuaVatTuTaiKhoaDuoc;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.YeuCauMuaVatTuDuTruTaiKhoaDuoc
{
    [ScopedDependency(ServiceType = typeof(IYeuCauMuaVatTuDuTruTaiKhoaDuocService))]
    public class YeuCauMuaVatTuDuTruTaiKhoaDuocService : MasterFileService<DuTruMuaVatTuKhoDuoc>, IYeuCauMuaVatTuDuTruTaiKhoaDuocService
    {
        private readonly IRepository<DuTruMuaVatTuTheoKhoa> _duTruMuaVatTuTheoKhoaRepo;
        private readonly IRepository<DuTruMuaVatTu> _duTruMuaVatTuRepo;

        public YeuCauMuaVatTuDuTruTaiKhoaDuocService(IRepository<DuTruMuaVatTuKhoDuoc> repository, IRepository<DuTruMuaVatTuTheoKhoa> duTruMuaVatTuTheoKhoaRepo,
            IRepository<DuTruMuaVatTu> duTruMuaVatTuRepo)
            : base(repository)
        {
            _duTruMuaVatTuTheoKhoaRepo = duTruMuaVatTuTheoKhoaRepo;
            _duTruMuaVatTuRepo = duTruMuaVatTuRepo;
        }
        public async Task<long> GuiDuTruMuaVatTuTaiKhoaDuoc(DuTruMuaVatTuChiTietGoiViewGridVo duTruMuaVatTuTaiKhoaDuoc)
        {
            DuTruMuaVatTuKhoDuoc duTruMuaVatTuKhoDuoc = new DuTruMuaVatTuKhoDuoc
            {
                NhanVienYeuCauId = duTruMuaVatTuTaiKhoaDuoc.NguoiYeuCauId,
                GhiChu = duTruMuaVatTuTaiKhoaDuoc.GhiChu,
                KyDuTruMuaDuocPhamVatTuId= duTruMuaVatTuTaiKhoaDuoc.KyDuTruId,
                NgayYeuCau = duTruMuaVatTuTaiKhoaDuoc.NgayYeuCau,
                SoPhieu = GetSoPhieuDuTruTheKhoaDuoc(),
                DenNgay = duTruMuaVatTuTaiKhoaDuoc.DenNgay,
                TuNgay = duTruMuaVatTuTaiKhoaDuoc.TuNgay

            };
            var listDuTruTheoKhoa = _duTruMuaVatTuTheoKhoaRepo.Table
                .Where(o => duTruMuaVatTuTaiKhoaDuoc.thongTinChiTietTongHopDuTruTuaTaiKhoaDuocGoiList.SelectMany(x => x.thongTinChiTietTongHopDuTruTuaTaiKhoaDuocListChild).Select(y => y.DuTruMuaVatTuTheoKhoaId).Contains(o.Id))
                .Include(o => o.DuTruMuaVatTuTheoKhoaChiTiets).ToList();
            var listDuTru = _duTruMuaVatTuRepo.Table
                .Where(o => duTruMuaVatTuTaiKhoaDuoc.thongTinChiTietTongHopDuTruTuaTaiKhoaDuocGoiList.SelectMany(x => x.thongTinChiTietTongHopDuTruTuaTaiKhoaDuocListChild).Where(y => y.DuTruMuaVatTuTheoKhoaId == 0).Select(y => y.DuTruMuaVatTuId).Contains(o.Id))
                .Include(o => o.DuTruMuaVatTuChiTiets).ToList();
            foreach (var duTruTheoKhoa in listDuTruTheoKhoa)
            {
                duTruMuaVatTuKhoDuoc.DuTruMuaVatTuTheoKhoas.Add(duTruTheoKhoa);
            }
            foreach (var duTru in listDuTru)
            {
                duTruMuaVatTuKhoDuoc.DuTruMuaVatTus.Add(duTru);
            }

            foreach (var chiTietVo in duTruMuaVatTuTaiKhoaDuoc.thongTinChiTietTongHopDuTruTuaTaiKhoaDuocGoiList)
            {
                DuTruMuaVatTuKhoDuocChiTiet duTruMuaVatTuKhoDuocChiTiet = new DuTruMuaVatTuKhoDuocChiTiet
                {
                    SoLuongDuTru = chiTietVo.SLDuTru,
                    LaVatTuBHYT = chiTietVo.Loai,
                    VatTuId = chiTietVo.VatTuId,
                    SoLuongDuKienSuDung = chiTietVo.SLDuKienSuDungTrongKho,
                    SoLuongDuTruKhoDuocDuyet = chiTietVo.SLDuTruKhoDuocDuyet,
                    SoLuongDuTruTruongKhoaDuyet = chiTietVo.SLDuTruTKhoDuyet,

                };
                foreach (var chiTietTheoKhoa in chiTietVo.thongTinChiTietTongHopDuTruTuaTaiKhoaDuocListChild)
                {
                    if (chiTietTheoKhoa.DuTruMuaVatTuTheoKhoaChiTietId != 0)
                    {
                        var aa = listDuTruTheoKhoa.SelectMany(o => o.DuTruMuaVatTuTheoKhoaChiTiets).First(o => o.Id == chiTietTheoKhoa.DuTruMuaVatTuTheoKhoaChiTietId);
                        duTruMuaVatTuKhoDuocChiTiet.DuTruMuaVatTuTheoKhoaChiTiets.Add(listDuTruTheoKhoa.SelectMany(o => o.DuTruMuaVatTuTheoKhoaChiTiets).First(o => o.Id == chiTietTheoKhoa.DuTruMuaVatTuTheoKhoaChiTietId));
                    }
                    else
                    {
                        duTruMuaVatTuKhoDuocChiTiet.DuTruMuaVatTuChiTiets.Add(listDuTru.SelectMany(o => o.DuTruMuaVatTuChiTiets).First(o => o.Id == chiTietTheoKhoa.DuTruMuaVatTuChiTietId));
                    }
                }
                duTruMuaVatTuKhoDuoc.DuTruMuaVatTuKhoDuocChiTiets.Add(duTruMuaVatTuKhoDuocChiTiet);
            }

            BaseRepository.Add(duTruMuaVatTuKhoDuoc);
            return duTruMuaVatTuKhoDuoc.Id;

        }
        private string GetSoPhieuDuTruTheKhoaDuoc()
        {
            var result = string.Empty;
            var soPhieu = "KDTHDT";
            var lastYearMonthCurrent = DateTime.Now.ToString("yyMM");
            var lastSoPhieuStr = BaseRepository.TableNoTracking.Select(p => p.SoPhieu).LastOrDefault();

            if (lastSoPhieuStr != null)
            {
                var lastSoPhieu = int.Parse(lastSoPhieuStr.Substring(lastSoPhieuStr.Length - 4));
                if (lastSoPhieu < 10)
                {
                    var lastDuTruId = "000" + (lastSoPhieu + 1).ToString();
                    result = soPhieu + lastYearMonthCurrent + lastDuTruId;
                }
                else if (lastSoPhieu < 100)
                {
                    var lastDuTruId = "00" + (lastSoPhieu + 1).ToString();
                    result = soPhieu + lastYearMonthCurrent + lastDuTruId;
                }
                else
                {
                    var lastDuTruId = "0" + (lastSoPhieu + 1).ToString();
                    result = soPhieu + lastYearMonthCurrent + lastDuTruId;
                }
            }
            else
            {
                result = soPhieu + lastYearMonthCurrent + "0001";
            }
            return result;
        }
    }
}
