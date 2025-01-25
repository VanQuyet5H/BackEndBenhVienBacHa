using System;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.VatTuBenhViens;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.VatTuBenhViens;
using Camino.Data;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Helpers;

namespace Camino.Services.VatTuBenhViens
{
    [ScopedDependency(ServiceType = typeof(IVatTuBenhVienService))]
    public class VatTuBenhVienService : MasterFileService<VatTuBenhVien>, IVatTuBenhVienService
    {
        IRepository<Core.Domain.Entities.VatTus.VatTu> _vatTurepository;
        public VatTuBenhVienService(IRepository<VatTuBenhVien> repository, IRepository<Core.Domain.Entities.VatTus.VatTu> vatTurepository) : base(repository)
        {
            _vatTurepository = vatTurepository;
        }
        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel)
        {
            BuildDefaultSortExpression(queryInfo);

            if(exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var query = BaseRepository.TableNoTracking.Include(g => g.VatTus).ThenInclude(a => a.DonViTinh).Include(x => x.VatTus).ThenInclude(x => x.NhomVatTu).Select(s => new VatTuBenhVienGridVo
            {
                Id = s.Id,
                Ten = s.VatTus.Ten,
                Ma = s.MaVatTuBenhVien,
                NuocSanXuat = s.VatTus.NuocSanXuat,
                NhaSanXuat = s.VatTus.NhaSanXuat,
                QuyCach = s.VatTus.QuyCach,
                TenDonViTinh = s.VatTus.DonViTinh,
                TenNhomVatTu = s.VatTus.NhomVatTu.Ten,
                NhomVatTuId = s.VatTus.NhomVatTuId,
                IsDisabled = s.VatTus.IsDisabled
            });
            query = query.ApplyLike(queryInfo.SearchTerms, g => g.Ten, g => g.Ma, g => g.TenDonViTinh, g => g.NhaSanXuat, g => g.NuocSanXuat, g => g.TenNhomVatTu);
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var query = BaseRepository.TableNoTracking.Include(g => g.VatTus).ThenInclude(a => a.DonViTinh).Include(x => x.VatTus).ThenInclude(x => x.NhomVatTu).Select(s => new VatTuBenhVienGridVo
            {
                Id = s.Id,
                Ten = s.VatTus.Ten,
                Ma = s.MaVatTuBenhVien,
                NuocSanXuat = s.VatTus.NuocSanXuat,
                NhaSanXuat = s.VatTus.NhaSanXuat,
                QuyCach = s.VatTus.QuyCach,
                TenDonViTinh = s.VatTus.DonViTinh,
                TenNhomVatTu = s.VatTus.NhomVatTu.Ten,
                NhomVatTuId = s.VatTus.NhomVatTuId
            });
            query = query.ApplyLike(queryInfo.SearchTerms, g => g.Ten, g => g.Ma, g => g.TenDonViTinh, g => g.NhaSanXuat, g => g.NuocSanXuat, g => g.TenNhomVatTu);
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        public async Task<List<VatTuYTeDropdownTemplateVo>> GetVatTuYTeBenhVienKhamBenh(DropDownListRequestModel model)
        {
            return await _vatTurepository.TableNoTracking.Where(o=>BaseRepository.TableNoTracking.All(p=>p.Id!=o.Id)).Include(x => x.NhomVatTu).Where(x => x.IsDisabled == false)
                .Select(item => new VatTuYTeDropdownTemplateVo
                {
                    DisplayName = item.Ma + " - " + item.Ten,
                    KeyId = item.Id,
                    DonVi = item.DonViTinh,
                    Ten = item.Ten,
                    Nhom = item.NhomVatTu.Ten,
                    Ma = item.Ma,
                }).ApplyLike(model.Query, o => o.DisplayName).Take(model.Take)
                .ToListAsync();
        }
        public async Task<List<VatTuYTeDropdownTemplateVo>> GetVatTuYTeBenhVienKhamBenhUpdate(DropDownListRequestModel model, long id)
        {
            var lst = await _vatTurepository.TableNoTracking.Include(x => x.NhomVatTu).Where(x => x.IsDisabled == false)
                .ApplyLike(model.Query, g => g.Ma, g => g.Ten, g => g.NhomVatTu.Ten, g => g.DonViTinh)
               .ToListAsync();
            var vtBenhVien = await BaseRepository.TableNoTracking
                .ToListAsync();
            var result = lst.Where(p => !vtBenhVien.Any(p2 => p2.Id == p.Id)).ToList();
            var query = result.Select(item => new VatTuYTeDropdownTemplateVo
            {
                DisplayName = item.Ten,
                KeyId = item.Id,
                DonVi = item.DonViTinh,
                Ten = item.Ten,
                Nhom = item.NhomVatTu.Ten,
                Ma = item.Ma,
            }).ToList();
            if (id != 0)
            {
                var vtBenhVienUp = await _vatTurepository.TableNoTracking.Where(x => x.Id.Equals(id)).Select(item => new VatTuYTeDropdownTemplateVo
                {
                    DisplayName = item.Ten,
                    KeyId = item.Id,
                    DonVi = item.DonViTinh,
                    Ten = item.Ten,
                    Nhom = item.NhomVatTu.Ten,
                    Ma = item.Ma,
                }).ToListAsync();

                query.AddRange(vtBenhVienUp);
            }
            return query;
        }

        public List<LookupItemVo> GetLoaiSuDung(LookupQueryInfo queryInfo)
        {
            var listLoaiPttt = EnumHelper.GetListEnum<Enums.LoaiSuDung>()
                .Select(item => new LookupItemVo
                {
                    DisplayName = item.GetDescription(),
                    KeyId = Convert.ToInt32(item)
                }).ToList();

            if (!string.IsNullOrEmpty(queryInfo.Query))
            {
                listLoaiPttt = listLoaiPttt.Where(p => p.DisplayName != null && p.DisplayName.ToLower()
                                                           .Contains(queryInfo.Query.ToLower().Trim())).ToList();
            }

            return listLoaiPttt;
        }

        #region //BVHD-345
        public async Task<string> GetMaTaoMoiVatTuAsync(MaVatTuTaoMoiInfoVo model)
        {
            // cấu trúc mã vật tư mới
            // 2 ký tự đầu chữ đầu trong tên vật tư
            // 1 ký tự đầu trong chữ thứ 2 tên vật tư
            // 1 ký tư mặc định là V
            // 3 số đánh thứ tự tên vật tư từ 001 -> 999
            // tổng ký tự mã là 7

            var maVatTuNew = string.Empty;
            var kyTuPhanNhomVatTu = "V";

            var tenVatTuXoaDau = model.TenVatTu.RemoveVietnameseDiacritics();
            var baKyTuDauVatTu = string.Empty;
            var arrTenVatTu = tenVatTuXoaDau.Split(" ");

            // 2 ký tự đầu trong chữ đầu tiên
            var arrChuDauChar = arrTenVatTu[0].ToCharArray();
            foreach (var tenChar in arrChuDauChar)
            {
                if (Char.IsLetter(tenChar))
                {
                    baKyTuDauVatTu += tenChar;
                    if (baKyTuDauVatTu.Length == 2)
                    {
                        break;
                    }
                }
            }

            // 1 ký tự đầu trong chữ thứ 2
            if (arrTenVatTu.Length > 1)
            {
                for (int i = 1; i < arrTenVatTu.Length; i++)
                {
                    var arrChuThuHaiChar = arrTenVatTu[i].ToCharArray();
                    foreach (var tenChar in arrChuThuHaiChar)
                    {
                        if (Char.IsLetter(tenChar))
                        {
                            baKyTuDauVatTu += tenChar;
                            break;
                        }
                    }

                    if (baKyTuDauVatTu.Length == 3)
                    {
                        break;
                    }
                } 
            }

            maVatTuNew = baKyTuDauVatTu + kyTuPhanNhomVatTu;
            var soThuTuVatTu = 1;

            var @maVatTuTemp = maVatTuNew;
            var lenKyTuChuMaVatTu = @maVatTuTemp.Length;
            var lstMaVatTuCungTen = await BaseRepository.TableNoTracking
                    .Where(x => x.MaVatTuBenhVien.Substring(0, lenKyTuChuMaVatTu).Contains(@maVatTuTemp)
                                && (model.VatTuMaCuoiCungId == null || x.Id < model.VatTuMaCuoiCungId))
                    .Select(x => x.MaVatTuBenhVien)
                    .ToListAsync();

            // dùng để check những mã đang được gán tạm trên FE
            lstMaVatTuCungTen
                .AddRange(
                    model.MaVatTuTemps
                        .Where(x => x.Substring(0, lenKyTuChuMaVatTu).ToLower().Contains(maVatTuNew.ToLower()))
                        .Select(x => x)
                        .ToList()
                    );

            if (!lstMaVatTuCungTen.Any())
            {
                maVatTuNew += soThuTuVatTu.ToString("000");
            }
            else
            {
                var lstSoThuTu = new List<int>();
                foreach (var maVatTu in lstMaVatTuCungTen)
                {
                    var strSoThuTu = maVatTu.Substring(lenKyTuChuMaVatTu);
                    int numericValue;
                    bool isNumber = int.TryParse(strSoThuTu, out numericValue);
                    if (isNumber && numericValue <= 999 && numericValue >= 1)
                    {
                        lstSoThuTu.Add(numericValue);
                    }
                    else
                    {
                        var arrChar = strSoThuTu.ToCharArray();
                        var strNumberNew = string.Empty;
                        for (int i = arrChar.Length - 1; i >= 0; i--)
                        {
                            int valueByIndex;
                            bool isNumberByIndex = int.TryParse(arrChar[i].ToString(), out valueByIndex);
                            if (isNumberByIndex)
                            {
                                strNumberNew = arrChar[i] + strNumberNew;
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (!string.IsNullOrEmpty(strNumberNew))
                        {
                            int numericValueTemp;
                            bool isNumberTemp = int.TryParse(strNumberNew, out numericValueTemp);
                            lstSoThuTu.Add(numericValueTemp);
                        }
                    }
                }
                var soThuTuNews = Enumerable.Range(1, 999).Except(lstSoThuTu);
                var thuTuNews = soThuTuNews as int[] ?? soThuTuNews.ToArray();
                if (thuTuNews.Any())
                {
                    maVatTuNew += thuTuNews.First().ToString("000");
                }
            }

            return maVatTuNew.ToUpper();
        }

        public async Task<bool> KiemTraTrungVatTuBenhVienAsync(Core.Domain.Entities.VatTus.VatTu vatTu)
        {
            var kiemTraTrung = await BaseRepository.TableNoTracking
                .AnyAsync(x => x.VatTus.Ten.Contains(vatTu.Ten)
                               && ((string.IsNullOrEmpty(x.VatTus.DonViTinh) && string.IsNullOrEmpty(vatTu.DonViTinh)) || x.VatTus.DonViTinh.Contains(vatTu.DonViTinh))
                               && ((string.IsNullOrEmpty(x.VatTus.NhaSanXuat) && string.IsNullOrEmpty(vatTu.NhaSanXuat)) || x.VatTus.DonViTinh.Contains(vatTu.NhaSanXuat))
                               && ((string.IsNullOrEmpty(x.VatTus.NuocSanXuat) && string.IsNullOrEmpty(vatTu.NuocSanXuat)) || x.VatTus.DonViTinh.Contains(vatTu.NuocSanXuat)));
            return kiemTraTrung;
        }

        public async Task<bool> KiemTraTrungMaVatTuBenhVienAsync(long vatTuBenhVienId, string maVatTu, List<string> maVatTuTemps = null)
        {
            if (string.IsNullOrEmpty(maVatTu) || maVatTu.Length < 7)
            {
                return false;
            }

            var kiemTra = await BaseRepository.TableNoTracking
                .AnyAsync(x => (vatTuBenhVienId == 0 || x.Id != vatTuBenhVienId)
                               && x.MaVatTuBenhVien.Contains(maVatTu));
            return kiemTra || (maVatTuTemps != null && maVatTuTemps.Contains(maVatTu));
        }

        public void XuLyCapNhatMaVatTuBenhVien()
        {
            long vatTuBenhVienIdPrev = 0;
            int take = 20;
            XDocument data = null;
            XElement vatTuBenhVienElement = null;
            var path = @"Resource\\VatTuBenhVienDaCapNhatMaCuoiCung.xml";
            if (File.Exists(path))
            {
                data = XDocument.Load(path);
                XNamespace root = data.Root.GetDefaultNamespace();
                vatTuBenhVienElement = data.Descendants(root + "VatTuBenhVien").FirstOrDefault();
                vatTuBenhVienIdPrev = (long)vatTuBenhVienElement.Element(root + "VatTuBenhVienId");
            }
            else
            {
                data =
                    new XDocument(
                        new XElement("VatTuBenhVien",
                            new XElement("VatTuBenhVienId", vatTuBenhVienIdPrev.ToString())
                        )
                    );
                XNamespace root = data.Root.GetDefaultNamespace();
                vatTuBenhVienElement = data.Descendants(root + "VatTuBenhVien").FirstOrDefault();
            }

            if (vatTuBenhVienIdPrev != -1)
            {
                var vatTuBenhVienIds = BaseRepository.TableNoTracking
                    .Where(x => x.Id > vatTuBenhVienIdPrev)
                    .Select(x => x.Id)
                    .Take(take)
                    .ToList();
                if (vatTuBenhVienIds.Any())
                {
                    foreach (var vatTuBenhVienId in vatTuBenhVienIds)
                    {
                        var vatTuBenhVien = BaseRepository.Table
                            .Include(x => x.VatTus)
                            .Include(x => x.YeuCauVatTuBenhViens)
                            .Include(x => x.YeuCauKhamBenhDonVTYTChiTiets)
                            .Include(x => x.DonVTYTThanhToanChiTiets)
                            .Include(x => x.DonVTYTThanhToanChiTietTheoPhieuThus)
                            .FirstOrDefault(x => x.Id == vatTuBenhVienId
                                                 && x.VatTus != null);
                        if (vatTuBenhVien != null)
                        {
                            var obj = new MaVatTuTaoMoiInfoVo()
                            {
                                TenVatTu = vatTuBenhVien.VatTus.Ten,
                                VatTuMaCuoiCungId = vatTuBenhVienId
                            };
                            var maMoi = GetMaTaoMoiVatTuAsync(obj).Result;
                            vatTuBenhVien.MaVatTuBenhVien = maMoi;

                            foreach (var yeuCau in vatTuBenhVien.YeuCauVatTuBenhViens)
                            {
                                yeuCau.Ma = maMoi;
                            }

                            foreach (var yeuCau in vatTuBenhVien.YeuCauKhamBenhDonVTYTChiTiets)
                            {
                                yeuCau.Ma = maMoi;
                            }

                            foreach (var donChiTiet in vatTuBenhVien.DonVTYTThanhToanChiTiets)
                            {
                                donChiTiet.Ma = maMoi;
                            }

                            foreach (var donChiTiet in vatTuBenhVien.DonVTYTThanhToanChiTietTheoPhieuThus)
                            {
                                donChiTiet.Ma = maMoi;
                            }

                            BaseRepository.Context.SaveChanges();
                        }
                    }

                    // cập nhật id cuối cùng
                    // trường hợp đã chạy hết vật tư bệnh viện thì bật cờ ko chạy job này nữa
                    if (vatTuBenhVienIds.Count < take)
                    {
                        vatTuBenhVienIdPrev = -1;
                    }
                    else
                    {
                        vatTuBenhVienIdPrev = vatTuBenhVienIds.OrderByDescending(x => x).First();
                    }
                    vatTuBenhVienElement.Element("VatTuBenhVienId").Value = vatTuBenhVienIdPrev.ToString();
                    data.Save(path);
                }
                else
                {
                    // trường hợp đã chạy hết vật tư bệnh viện thì bật cờ ko chạy job này nữa
                    vatTuBenhVienIdPrev = -1;
                    vatTuBenhVienElement.Element("VatTuBenhVienId").Value = vatTuBenhVienIdPrev.ToString();
                    data.Save(path);
                }
            }
        }
        #endregion
    }

}

