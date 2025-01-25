using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.DuTruMuaDuocPhamKhoDuocs;
using Camino.Core.Domain.Entities.DuTruMuaDuocPhams;
using Camino.Core.Domain.Entities.DuTruMuaDuocPhamTheoKhoas;
using Camino.Core.Domain.ValueObject.DuTruMuaDuocPhamTaiKhoaDuoc;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.YeuCauMuaDuocPhamDuTruTaiKhoaDuoc
{
    [ScopedDependency(ServiceType = typeof(IYeuCauMuaDuocPhamDuTruTaiKhoaDuocService))]
   public class YeuCauMuaDuocPhamDuTruTaiKhoaDuocService : MasterFileService<DuTruMuaDuocPhamKhoDuoc>, IYeuCauMuaDuocPhamDuTruTaiKhoaDuocService
    {
        private readonly IRepository<DuTruMuaDuocPhamTheoKhoa> _duTruMuaDuocPhamTheoKhoaRepo;
        private readonly IRepository<DuTruMuaDuocPham> _duTruMuaDuocPhamRepo;

        public YeuCauMuaDuocPhamDuTruTaiKhoaDuocService(IRepository<DuTruMuaDuocPhamKhoDuoc> repository, IRepository<DuTruMuaDuocPhamTheoKhoa> duTruMuaDuocPhamTheoKhoaRepo,
            IRepository<DuTruMuaDuocPham> duTruMuaDuocPhamRepo)
            : base(repository)
        {
            _duTruMuaDuocPhamTheoKhoaRepo = duTruMuaDuocPhamTheoKhoaRepo;
            _duTruMuaDuocPhamRepo = duTruMuaDuocPhamRepo;
        }
        public async Task<long> GuiDuTruMuaDuocPhamTaiKhoaDuoc (DuTruMuaDuocPhamChiTietGoiViewGridVo duTruMuaDuocPhamTaiKhoaDuoc)
        {
            DuTruMuaDuocPhamKhoDuoc duTruMuaDuocPhamKhoDuoc = new DuTruMuaDuocPhamKhoDuoc
            {
                NhanVienYeuCauId = duTruMuaDuocPhamTaiKhoaDuoc.NguoiYeuCauId,
                GhiChu = duTruMuaDuocPhamTaiKhoaDuoc.GhiChu,
                KyDuTruMuaDuocPhamVatTuId = duTruMuaDuocPhamTaiKhoaDuoc.KyDuTruId,
                NgayYeuCau = duTruMuaDuocPhamTaiKhoaDuoc.NgayYeuCau,
                SoPhieu = GetSoPhieuDuTruTheKhoaDuoc(),
                DenNgay = duTruMuaDuocPhamTaiKhoaDuoc.DenNgay,
                TuNgay = duTruMuaDuocPhamTaiKhoaDuoc.TuNgay

            };
            var listDuTruTheoKhoa = _duTruMuaDuocPhamTheoKhoaRepo.Table
                .Where(o => duTruMuaDuocPhamTaiKhoaDuoc.thongTinChiTietTongHopDuTruTuaTaiKhoaDuocGoiList.SelectMany(x => x.thongTinChiTietTongHopDuTruTuaTaiKhoaDuocListChild).Select(y => y.DuTruMuaDuocPhamTheoKhoaId).Contains(o.Id))
                .Include(o=>o.DuTruMuaDuocPhamTheoKhoaChiTiets).ToList();
            var listDuTru = _duTruMuaDuocPhamRepo.Table
                .Where(o => duTruMuaDuocPhamTaiKhoaDuoc.thongTinChiTietTongHopDuTruTuaTaiKhoaDuocGoiList.SelectMany(x => x.thongTinChiTietTongHopDuTruTuaTaiKhoaDuocListChild).Where(y=>y.DuTruMuaDuocPhamTheoKhoaId == 0).Select(y => y.DuTruMuaDuocPhamId).Contains(o.Id))
                .Include(o => o.DuTruMuaDuocPhamChiTiets).ToList();
            foreach(var duTruTheoKhoa in listDuTruTheoKhoa)
            {
                duTruMuaDuocPhamKhoDuoc.DuTruMuaDuocPhamTheoKhoas.Add(duTruTheoKhoa);
            }
            foreach (var duTru in listDuTru)
            {
                duTruMuaDuocPhamKhoDuoc.DuTruMuaDuocPhams.Add(duTru);
            }

            foreach(var chiTietVo in duTruMuaDuocPhamTaiKhoaDuoc.thongTinChiTietTongHopDuTruTuaTaiKhoaDuocGoiList)
            {
                DuTruMuaDuocPhamKhoDuocChiTiet duTruMuaDuocPhamKhoDuocChiTiet = new DuTruMuaDuocPhamKhoDuocChiTiet
                {
                    SoLuongDuTru = chiTietVo.SLDuTru,
                    LaDuocPhamBHYT = chiTietVo.Loai,
                    DuocPhamId = chiTietVo.DuocPhamId,
                    SoLuongDuKienSuDung = chiTietVo.SLDuKienSuDungTrongKho,
                    SoLuongDuTruKhoDuocDuyet = (int)chiTietVo.SLDuTruKhoDuocDuyet,
                    SoLuongDuTruTruongKhoaDuyet = (int)chiTietVo.SLDuTruTKhoDuyet,
                    
                };
                foreach(var chiTietTheoKhoa in chiTietVo.thongTinChiTietTongHopDuTruTuaTaiKhoaDuocListChild)
                {
                    if(chiTietTheoKhoa.DuTruMuaDuocPhamTheoKhoaChiTietId != 0)
                    {
                        var aa = listDuTruTheoKhoa.SelectMany(o => o.DuTruMuaDuocPhamTheoKhoaChiTiets).First(o => o.Id == chiTietTheoKhoa.DuTruMuaDuocPhamTheoKhoaChiTietId);
                        duTruMuaDuocPhamKhoDuocChiTiet.DuTruMuaDuocPhamTheoKhoaChiTiets.Add(listDuTruTheoKhoa.SelectMany(o => o.DuTruMuaDuocPhamTheoKhoaChiTiets).First(o => o.Id == chiTietTheoKhoa.DuTruMuaDuocPhamTheoKhoaChiTietId));
                    }
                    else
                    {
                        duTruMuaDuocPhamKhoDuocChiTiet.DuTruMuaDuocPhamChiTiets.Add(listDuTru.SelectMany(o => o.DuTruMuaDuocPhamChiTiets).First(o => o.Id == chiTietTheoKhoa.DuTruMuaDuocPhamChiTietId));
                    }
                }
                duTruMuaDuocPhamKhoDuoc.DuTruMuaDuocPhamKhoDuocChiTiets.Add(duTruMuaDuocPhamKhoDuocChiTiet);
            }

            BaseRepository.Add(duTruMuaDuocPhamKhoDuoc);
            return duTruMuaDuocPhamKhoDuoc.Id ;
          
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
