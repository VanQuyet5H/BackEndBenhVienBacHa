using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BenhNhans;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Camino.Core.Data;
using Camino.Core.Helpers;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using System.Collections.Generic;
using Camino.Core.Domain.Entities.CongTyBaoHiemTuNhans;
using System;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using System.Drawing;

namespace Camino.Services.BenhNhans
{
    [ScopedDependency(ServiceType = typeof(IBenhNhanService))]
    public class BenhNhanService : MasterFileService<BenhNhan>, IBenhNhanService
    {
        private readonly IRepository<CongTyBaoHiemTuNhan> _congTyBaoHiemTuNhanRepository;
        private readonly IRepository<DuocPham> _duocPhamRepository;
        private readonly IRepository<ThuocHoacHoatChat> _thuocHoacHoatChatRepository;
        private readonly IRepository<Template> _templateRepository;


        public BenhNhanService(IRepository<BenhNhan> repository,
            IRepository<DuocPham> duocPhamRepository,
            IRepository<ThuocHoacHoatChat> thuocHoacHoatChatRepository,
            IRepository<Template> templateRepository,
            IRepository<CongTyBaoHiemTuNhan> congTyBaoHiemTuNhanRepository) : base(repository)
        {
            _congTyBaoHiemTuNhanRepository = congTyBaoHiemTuNhanRepository;
            _duocPhamRepository = duocPhamRepository;
            _thuocHoacHoatChatRepository = thuocHoacHoatChatRepository;
            _templateRepository = templateRepository;
        }

        public async Task<BenhNhan> GetBenhNhanByMaBHYT(string maBHYT, long? benhNhanId)
        {
            if (benhNhanId == null || benhNhanId<=0)
            {
                var entity = await BaseRepository.TableNoTracking.FirstOrDefaultAsync(p => p.BHYTMaSoThe.Equals(maBHYT));
                return entity;
            }
            else
            {
                var entity = await BaseRepository.TableNoTracking.FirstOrDefaultAsync(p => p.BHYTMaSoThe.Equals(maBHYT) && p.Id == benhNhanId);
                return entity;
            }
        }

        public async Task<List<BenhNhan>> GetBenhNhanByTiepNhanBenhNhanTimKiem(TimKiemBenhNhanGridVo model, TimKiemBenhNhanSearch searchPopup)
        {
            var entity = BaseRepository.TableNoTracking.Include(p => p.TinhThanh).Include(p => p.QuanHuyen).Include(p => p.PhuongXa).AsQueryable();
            //
            if (searchPopup == null)
            {
                if (!string.IsNullOrEmpty(model.MaBN))
                {
                    entity = entity.Where(p => p.MaBN.Contains(model.MaBN));
                }
                if (!string.IsNullOrEmpty(model.MaBHYT))
                {
                    entity = entity.Where(p => p.BHYTMaSoThe.Contains(model.MaBHYT));
                }
                if (!string.IsNullOrEmpty(model.HoTen))
                {
                    entity = entity.Where(p => p.HoTen.Contains(model.HoTen));
                }
                if (!string.IsNullOrEmpty(model.SoChungMinhThu))
                {
                    entity = entity.Where(p => p.SoChungMinhThu.Contains(model.SoChungMinhThu));
                }
                if (!string.IsNullOrEmpty(model.SoDienThoai))
                {
                    entity = entity.Where(p => p.SoDienThoai.Contains(model.SoDienThoai));
                }
                //if (!string.IsNullOrEmpty(model.DiaChi))
                //{

                //    entity = entity.Where(p => p.DiaChi.Contains(model.DiaChi) 
                //    || (p.TinhThanh != null && (", " + p.TinhThanh).Contains(model.DiaChi) )
                //    || (p.TinhThanh != null && (", " + p.TinhThanh).Contains(model.DiaChi))
                //    );
                //}
                //if (model.GioiTinh != null)
                //{
                //    entity = entity.Where(p => p.GioiTinh == model.GioiTinh);
                //}
                if (model.NgaySinh != null)
                {
                    var ngaySinh = model.NgaySinh.GetValueOrDefault().Day;
                    var thangSinh = model.NgaySinh.GetValueOrDefault().Month;
                    var namSinh = model.NgaySinh.GetValueOrDefault().Year;
                    entity = entity.Where(p => p.NgaySinh == ngaySinh && p.ThangSinh == thangSinh && p.NamSinh == namSinh);
                }

                if (model.NamSinh != null)
                {
                    entity = entity.Where(p => p.NamSinh == model.NamSinh);
                }
            }


            if (searchPopup != null)
            {
                //entity = entity.Where(p => p.MaBN.Contains(searchPopup.MaBenhNhan ?? "") && p.BHYTMaSoThe.Contains(searchPopup.MaBHYT ?? "")
                //        && p.HoTen.Contains(searchPopup.HoTen ?? "") && p.SoChungMinhThu.Contains(searchPopup.CMND ?? "")
                //        && p.SoDienThoai.Contains(searchPopup.DienThoai ?? "") && p.DiaChi.Contains(searchPopup.DiaChi ?? ""));
                if (!string.IsNullOrEmpty(searchPopup.MaBenhNhan))
                {
                    entity = entity.Where(p => p.MaBN.Contains(searchPopup.MaBenhNhan));
                }
                if (!string.IsNullOrEmpty(searchPopup.MaBHYT))
                {
                    entity = entity.Where(p => p.BHYTMaSoThe.Contains(searchPopup.MaBHYT));
                }
                if (!string.IsNullOrEmpty(searchPopup.HoTen))
                {
                    //entity = entity.Where(p => p.HoTen.Contains(searchPopup.HoTen));
                    entity = entity.Where(p => p.HoTen.Contains(searchPopup.HoTen));
                }
                if (!string.IsNullOrEmpty(searchPopup.CMND))
                {
                    entity = entity.Where(p => p.SoChungMinhThu.Contains(searchPopup.CMND));
                }
                if (!string.IsNullOrEmpty(searchPopup.DienThoai))
                {
                    entity = entity.Where(p => p.SoDienThoai.Contains(searchPopup.DienThoai));
                }
                //if (!string.IsNullOrEmpty(searchPopup.DiaChi))
                //{
                //    entity = entity.Where(p => p.DiaChi.Contains(searchPopup.DiaChi));
                //}
                if (searchPopup.NgaySinhFormat != null)
                {
                    var ngaySinh = searchPopup.NgaySinhFormat.GetValueOrDefault().Day;
                    var thangSinh = searchPopup.NgaySinhFormat.GetValueOrDefault().Month;
                    var namSinh = searchPopup.NgaySinhFormat.GetValueOrDefault().Year;
                    entity = entity.Where(p => p.NgaySinh == ngaySinh && p.ThangSinh == thangSinh && p.NamSinh == namSinh);
                }

                if (searchPopup.NamSinh != null)
                {
                    entity = entity.Where(p => p.NamSinh == searchPopup.NamSinh);
                }
            }

            var result = await entity.ToListAsync();
            foreach (var item in result)
            {
                if (item.PhuongXa != null && item.QuanHuyen != null && item.TinhThanh != null)
                {
                    item.DiaChi = item.DiaChi + ", " + item.PhuongXa.Ten + ", " + item.QuanHuyen.Ten + " ," + item.TinhThanh.Ten;
                }
                else if (item.PhuongXa != null && item.QuanHuyen != null)
                {
                    item.DiaChi = item.DiaChi + ", " + item.PhuongXa.Ten + ", " + item.QuanHuyen.Ten;
                }
                else if (item.QuanHuyen != null && item.TinhThanh != null)
                {
                    item.DiaChi = item.DiaChi + ", " + item.QuanHuyen.Ten + " ," + item.TinhThanh.Ten;
                }
                else if (item.PhuongXa != null && item.TinhThanh != null)
                {
                    item.DiaChi = item.DiaChi + ", " + item.PhuongXa.Ten + " ," + item.TinhThanh.Ten;
                }
                else if (item.PhuongXa != null)
                {
                    item.DiaChi = item.DiaChi + ", " + item.PhuongXa.Ten;
                }
                else if (item.QuanHuyen != null)
                {
                    item.DiaChi = item.DiaChi + ", " + item.QuanHuyen.Ten;
                }
                else if (item.TinhThanh != null)
                {
                    item.DiaChi = item.DiaChi + ", " + item.TinhThanh.Ten;
                }
            }

            if (!string.IsNullOrEmpty(model.DiaChi))
            {
                result = result.Where(p => !string.IsNullOrEmpty(p.DiaChi) && p.DiaChi.ToLower().ConvertUnicodeString().ConvertToUnSign().Contains(model.DiaChi.ToLower().ConvertUnicodeString().ConvertToUnSign())
                ).ToList();
            }

            if (searchPopup != null)
            {
                if (!string.IsNullOrEmpty(searchPopup.DiaChi))
                {
                    result = result.Where(p => !string.IsNullOrEmpty(p.DiaChi) && p.DiaChi.ToLower().ConvertUnicodeString().ConvertToUnSign().Contains(searchPopup.DiaChi.ToLower().ConvertUnicodeString().ConvertToUnSign())
                ).ToList();
                }
            }

            return result;
        }

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel)
        {
            BuildDefaultSortExpression(queryInfo);

            if (forExportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var query = BaseRepository.TableNoTracking.Select(s => new BenhNhanGridVo
            {
                Id = s.Id,
                DiaChi = s.DiaChi,
                Email = s.Email,
                GioiTinh = s.GioiTinh != null ? s.GioiTinh.GetDescription() : "",
                SoChungMinhThu = s.SoChungMinhThu,
                HoTen = s.HoTen,
                NamSinh = s.NamSinh
            }).ApplyLike(queryInfo.SearchTerms,
                g => g.DiaChi,
                g => g.NamSinh.ToString(),
                g => g.HoTen,
                g => g.Email,
                g => g.SoChungMinhThu);
            var countTask = queryInfo.LazyLoadPage == true ?
                Task.FromResult(0) :
                query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource
            {
                Data = queryTask.Result,
                TotalRowCount = countTask.Result
            };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var result = BaseRepository.TableNoTracking.Select(s => new BenhNhanGridVo
            {
                Id = s.Id,
                DiaChi = s.DiaChi,
                Email = s.Email,
                GioiTinh = s.GioiTinh != null ? s.GioiTinh.GetDescription() : "",
                SoChungMinhThu = s.SoChungMinhThu,
                HoTen = s.HoTen,
                NamSinh = s.NamSinh
            }).ApplyLike(queryInfo.SearchTerms,
                g => g.DiaChi,
                g => g.NamSinh.ToString(),
                g => g.HoTen,
                g => g.Email,
                g => g.SoChungMinhThu);

            var countTask = result.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<ThemBHTNGridVo> ThemBHTN(ThemBHTN model)
        {
            var result = new ThemBHTNGridVo
            {
                BenhNhanId = model.BenhNhanId,
                CongTyBaoHiemTuNhanId = model.CongTyBaoHiemTuNhanId,
                CongTyDisplay = (await _congTyBaoHiemTuNhanRepository
                                .TableNoTracking.FirstOrDefaultAsync(p => p.Id == (model.CongTyBaoHiemTuNhanId ?? 0)))?.Ten,
                MaSoThe = model.MaSoThe,
                NgayHieuLuc = model.NgayHieuLuc,
                NgayHieuLucDisplay = model.NgayHieuLuc != null ? (model.NgayHieuLuc ?? DateTime.Now).ApplyFormatDate() : null,
                NgayHetHan = model.NgayHetHan,
                NgayHetHanDisplay = model.NgayHetHan != null ? (model.NgayHetHan ?? DateTime.Now).ApplyFormatDate() : null,
                SoDienThoai = model.SoDienThoai,
                DiaChi = model.DiaChi
            };
            return result;
        }

        public async Task<TienSuBenhGridVo> ThemTienSuBenh(ThemTienSuBenh model)
        {
            var tienSuBenh = new TienSuBenhGridVo
            {
                BenhNhanId = model.BenhNhanId,
                BenhNhanTienSuBenhId = model.BenhNhanTienSuBenhId,
                TenBenh = model.TenBenh,
                LoaiTienSuBenh = model.LoaiTienSuBenh,
                TenLoaiTienSuBenh = model.LoaiTienSuBenh.GetDescription()
            };
            return tienSuBenh;
        }

        public async Task<DiUngThuocGridVo> ThemDiUngThuoc(ThemDiUngThuoc model)
        {
            var diUngThuoc = new DiUngThuocGridVo
            {
                BenhNhanId = model.BenhNhanId,
                BenhNhanDiUngId = model.BenhNhanDiUngId,
                TenDiUng = model.TenDiUng,
                LoaiDiUng = model.LoaiDiUng,
                LoaiDiUngDisplay = model.LoaiDiUng.GetDescription(),
                BieuHienDiUng = model.BieuHienDiUng,
                ThuocId = model.ThuocId,
                TenThuoc = model.TenThuoc,
                MucDo = model.MucDo,
                MucDoDisplay = model.MucDo.GetDescription()
            };
            if (model.ThuocId != null && model.ThuocId != 0)
            {
                diUngThuoc.TenDiUng = (await _thuocHoacHoatChatRepository.TableNoTracking.FirstOrDefaultAsync(p => p.Id == (model.ThuocId ?? 0)))?.Ten;
            }

            return diUngThuoc;
        }
        public List<LookupItemVo> GetLoaiTienSuBenhs()
        {
            var list = Enum.GetValues(typeof(Enums.EnumLoaiTienSuBenh)).Cast<Enum>();
            var result = list.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            }).ToList();
            return result;
        }

        public string TenThuocDiUng(long thuocId)
        {
            var tenThuoc = _thuocHoacHoatChatRepository.TableNoTracking
                            .Where(dp => dp.Id == thuocId).Select(dp => dp.Ten).FirstOrDefault();
            return tenThuoc;
        }

        private ThongTinTheBenhNhan ThongTinhTheBenhNhan(long benhNhanId)
        {
            var thongTinBN = BaseRepository.TableNoTracking
               .Where(bn => bn.Id == benhNhanId)
               .Select(s => new ThongTinTheBenhNhan
               {
                   Id = s.Id,
                   MaBN = s.MaBN,
                   HoTenBenhNhan = s.HoTen,
                   NgaySinh = s.NgaySinh,
                   ThangSinh = s.ThangSinh,
                   NamSinh = s.NamSinh,
                   GioiTinh = s.GioiTinh,
                   GioiTinhDisplay = s.GioiTinh != null ? s.GioiTinh.GetDescription() : null,
                   SoDienThoai = s.SoDienThoai.ApplyFormatPhone(),
                   NgheNghiep = s.NgheNghiep != null ? s.NgheNghiep.Ten : null,
                   QuocTich = s.QuocTich != null ? s.QuocTich.Ten : null,
                   Tinh = s.TinhThanh != null ? s.TinhThanh.Ten : null,
                   Phuong = s.PhuongXa != null ? s.PhuongXa.Ten : null,
                   Huyen = s.QuanHuyen != null ? s.QuanHuyen.Ten : null,
                   SoNha = s.DiaChi,
                   SoCMND = s.SoChungMinhThu,
                   Email = s.Email,
                   DanToc = s.DanToc != null ? s.DanToc.Ten : null
               })
               .FirstOrDefault();
            return thongTinBN;
        }

        public string InTheBenhNhanBenhNhan(long benhNhanId, string hostingName)
        {
            var thongTinBN = ThongTinhTheBenhNhan(benhNhanId);
            var content = string.Empty;

            var barCode = BarcodeHelper.EncodeStringsToContentBarcode(true, thongTinBN?.HoTenBenhNhan,
              DateHelper.DOBFormat(thongTinBN?.NgaySinh, thongTinBN?.ThangSinh, thongTinBN?.NamSinh),
              thongTinBN?.NamSinh.ToString(),
              thongTinBN?.GioiTinh.ToString(),
              thongTinBN?.SoDienThoai,
              thongTinBN?.NgheNghiep,
              thongTinBN?.QuocTich,
              thongTinBN?.Tinh,
              thongTinBN?.Phuong,
              thongTinBN?.SoNha,
              thongTinBN?.SoCMND,
              thongTinBN?.Email,
              thongTinBN?.DanToc);

            if (thongTinBN != null)
            {
                barCode = thongTinBN.MaBN + "|" + barCode;
            }
            var templateTheBenhNhan = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("TheBenhNhan")).FirstOrDefault();
            if (templateTheBenhNhan != null)
            {
                var data = new TheBenhNhan
                {
                    HostingName = hostingName,
                    BarCodeImgBase64 = BarcodeHelper.GenerateQrCode(barCode, Color.FromArgb(38, 42, 54), Color.White),
                    MaBN = thongTinBN?.MaBN,
                    HoTenBenhNhan = thongTinBN?.HoTenBenhNhan.ToUpper()
                };
                content = TemplateHelpper.FormatTemplateWithContentTemplate(templateTheBenhNhan.Body, data);
            }
            return content;
        }

        public async Task<bool> CheckCongTyBHTNExists(long? congTyBHTNId, List<long> congTyBHTNIds)
        {
            if (congTyBHTNId == null)
            {
                return true;
            }
            if (congTyBHTNIds != null)
            {
                if (congTyBHTNIds.Any(p => p == congTyBHTNId))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return true;
        }

        public async Task<bool> CheckTienSuBenhExists(Enums.EnumLoaiTienSuBenh? loaiTienSuBenh, string tenBenh, List<BenhNhanTienSuBenhChiTiet> tenBenhs)
        {
            //if (congTyBHTNId == null)
            //{
            //    return true;
            //}
            if (tenBenhs != null)
            {
                foreach (var item in tenBenhs)
                {
                    if (tenBenhs.Any(p => p.LoaiTienSuBenh == loaiTienSuBenh && p.TenBenh == tenBenh))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            return true;
        }

        public async Task<bool> CheckDiUngThuocExists(Enums.LoaiDiUng? loaiDiUng, long? thuocId, string tenDiUng, List<BenhNhanDiUngThuocChiTiet> tenDiUngs)
        {
            //if (congTyBHTNId == null)
            //{
            //    return true;
            //}
            if (tenDiUngs != null)
            {
                foreach (var item in tenDiUngs)
                {
                    if (tenDiUngs.Any(p => p.LoaiDiUng == loaiDiUng && p.TenDiUng == tenDiUng) || tenDiUngs.Any(p => p.LoaiDiUng == loaiDiUng && p.ThuocId == thuocId))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            return true;
        }
    }
}