using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using System.Xml.Linq;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.DuocPhamBenhVienPhanNhoms;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DuocPhamBenhViens;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.Thuoc;
using Camino.Core.Helpers;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.DuocPhamBenhVien
{
    [ScopedDependency(ServiceType = typeof(IDuocPhamBenhVienService))]
    public class DuocPhamBenhVienService : MasterFileService<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien>, IDuocPhamBenhVienService
    {
        IRepository<Core.Domain.Entities.Thuocs.DuocPham> _repositoryDuocPham;
        IRepository<DuocPhamBenhVienPhanNhom> _duocPhamBenhVienPhanNhomRepository;
        IRepository<DuocPhamBenhVienPhanNhom> _repositoryDuocPhamBenhVienPhanNhom;
        IRepository<Core.Domain.Entities.CauHinhs.CauHinh> _cauHinhRepository;
        IRepository<Core.Domain.Entities.MayXetNghiems.MayXetNghiem> _mayXetNghiemRepository;

        public DuocPhamBenhVienService(IRepository<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> repository
            , IRepository<Core.Domain.Entities.Thuocs.DuocPham> repositoryDuocPham,
            IRepository<DuocPhamBenhVienPhanNhom> duocPhamBenhVienPhanNhomRepository,
            IRepository<Core.Domain.Entities.CauHinhs.CauHinh> cauHinhRepository,
            IRepository<Core.Domain.Entities.MayXetNghiems.MayXetNghiem> mayXetNghiemRepository
             , IRepository<Core.Domain.Entities.DuocPhamBenhVienPhanNhoms.DuocPhamBenhVienPhanNhom> repositoryDuocPhamBenhVienPhanNhom)
            : base(repository)
        {
            _repositoryDuocPham = repositoryDuocPham;
            _repositoryDuocPhamBenhVienPhanNhom = repositoryDuocPhamBenhVienPhanNhom;
            _duocPhamBenhVienPhanNhomRepository = duocPhamBenhVienPhanNhomRepository;
            _cauHinhRepository = cauHinhRepository;
            _mayXetNghiemRepository = mayXetNghiemRepository;
        }

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool isPrint)
        {
            //
            var phanNhoms = _duocPhamBenhVienPhanNhomRepository.TableNoTracking.ToList();
            BuildDefaultSortExpression(queryInfo);
            var query = BaseRepository.TableNoTracking
                .Select(s => new DuocPhamBenhVienGridVo()
                {
                    Id = s.Id,
                    Ten = s.DuocPham.Ten,
                    SoDangKy = s.DuocPham.SoDangKy,
                    MaHoatChat = s.DuocPham.MaHoatChat,
                    Ma = s.MaDuocPhamBenhVien,
                    HoatChat = s.DuocPham.HoatChat,
                    QuyCach = s.DuocPham.QuyCach,
                    HamLuong = s.DuocPham.HamLuong,
                    TenDonViTinh = s.DuocPham.DonViTinh.Ten,
                    TenDuongDung = s.DuocPham.DuongDung.Ten,
                    TenLoaiThuocHoacHoatChat = s.DuocPham.LoaiThuocHoacHoatChat.GetDescription(),
                    PhanNhom = s.DuocPhamBenhVienPhanNhom != null ? PhanNhomChaCuaDuocPham(phanNhoms, s.DuocPhamBenhVienPhanNhom) : ""
                }).ApplyLike(queryInfo.SearchTerms, g => g.Ten, g => g.HamLuong, g => g.HoatChat, g => g.MaHoatChat, g => g.Ma
                , g => g.QuyCach, g => g.SoDangKy, g => g.TenDonViTinh, g => g.TenDuongDung, g => g.PhanNhom);
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = isPrint == true ? query.OrderBy(queryInfo.SortString).ToArrayAsync() : query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };

        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var phanNhoms = _duocPhamBenhVienPhanNhomRepository.TableNoTracking.ToList();
            var query = BaseRepository
                .TableNoTracking
                .Select(s => new DuocPhamBenhVienGridVo()
                {
                    Id = s.Id,
                    Ten = s.DuocPham.Ten,
                    SoDangKy = s.DuocPham.SoDangKy,
                    MaHoatChat = s.DuocPham.MaHoatChat,
                    Ma = s.MaDuocPhamBenhVien,
                    HoatChat = s.DuocPham.HoatChat,
                    QuyCach = s.DuocPham.QuyCach,
                    HamLuong = s.DuocPham.HamLuong,
                    TenDonViTinh = s.DuocPham.DonViTinh.Ten,
                    TenDuongDung = s.DuocPham.DuongDung.Ten,
                    TenLoaiThuocHoacHoatChat = s.DuocPham.LoaiThuocHoacHoatChat.GetDescription(),
                    PhanNhom = s.DuocPhamBenhVienPhanNhom != null ? PhanNhomChaCuaDuocPham(phanNhoms, s.DuocPhamBenhVienPhanNhom) : ""
                }).ApplyLike(queryInfo.SearchTerms, g => g.Ten, g => g.HamLuong, g => g.HoatChat, g => g.MaHoatChat, g => g.Ma
                    , g => g.QuyCach, g => g.SoDangKy, g => g.TenDonViTinh, g => g.TenDuongDung, g => g.PhanNhom); ;
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        public async Task<bool> IsIdExists(long id)
        {
            var result = true;
            var resultDT = await BaseRepository.TableNoTracking.Where(p => p.Id == id).ToListAsync();
            if (resultDT.Count() > 0)
            {
                result = false;
            }
            return result;
        }


        private void RenameSortForFormatColumn(QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.SortString) && queryInfo.SortString.Contains("Format"))
            {
                queryInfo.SortStringFormat = queryInfo.SortString?.Replace("Format", "") ?? "";
            }
        }
        public async Task<List<DuocPhamTemplate>> GetListDuocPham(DropDownListRequestModel model)
        {
            return await _repositoryDuocPham
                .TableNoTracking.Where(o => BaseRepository.TableNoTracking.All(p => p.Id != o.Id)).Include(x => x.DonViTinh)
                .Include(g => g.DuongDung).ThenInclude(k => k.DuocPhams).Where(q => q.IsDisabled != true).Select(s => new DuocPhamTemplate()
                {

                    DisplayName = s.Ten + " - " + s.HoatChat,
                    KeyId = s.Id,
                    Ten = s.Ten,
                    HoatChat = s.HoatChat,
                    SoDangKy = s.SoDangKy
                }).ApplyLike(model.Query, o => o.DisplayName)
                .Take(model.Take).ToListAsync();
        }
        public async Task<List<NhomDichVuBenhVienPhanNhomTreeViewVo>> GetListDichVuBenhVienPhanNhomAsync(DropDownListRequestModel model)
        {
            var lstNhomDichVu = await _repositoryDuocPhamBenhVienPhanNhom.TableNoTracking
                .Select(item => new NhomDichVuBenhVienPhanNhomTreeViewVo
                {
                    KeyId = item.Id,
                    DisplayName = item.Ten,//item.Ma + " - " + item.Ten,
                    ParentId = item.NhomChaId
                })
                .ToListAsync();

            var query = lstNhomDichVu.Select(item => new NhomDichVuBenhVienPhanNhomTreeViewVo
            {
                KeyId = item.KeyId,
                DisplayName = item.DisplayName,
                ParentId = item.ParentId,
                Items = GetChildrenTree(lstNhomDichVu, item.KeyId, model.Query.RemoveVietnameseDiacritics(), item.DisplayName.RemoveVietnameseDiacritics())
            })
            .Where(x =>
                x.ParentId == null && (string.IsNullOrEmpty(model.Query) ||
                                       (!string.IsNullOrEmpty(model.Query) && (x.Items.Any() || x.DisplayName.RemoveVietnameseDiacritics().Trim().ToLower().Contains(model.Query.RemoveVietnameseDiacritics().Trim().ToLower())))))
            .Take(model.Take).ToList();
            return query;
        }

        public static List<NhomDichVuBenhVienPhanNhomTreeViewVo> GetChildrenTree(List<NhomDichVuBenhVienPhanNhomTreeViewVo> comments, long Id, string queryString, string parentDisplay)
        {
            var query = comments
                .Where(c => c.ParentId != null && c.ParentId == Id)
                .Select(c => new NhomDichVuBenhVienPhanNhomTreeViewVo
                {
                    KeyId = c.KeyId,
                    DisplayName = c.DisplayName,
                    Level = c.Level,
                    ParentId = Id,
                    Items = GetChildrenTree(comments, c.KeyId, queryString, c.DisplayName)
                })
                .Where(c => string.IsNullOrEmpty(queryString)
                            || (!string.IsNullOrEmpty(queryString) && (parentDisplay.Trim().ToLower().Contains(queryString.Trim().ToLower()) || c.DisplayName.RemoveVietnameseDiacritics().Trim().ToLower().Contains(queryString.Trim().ToLower()) || c.Items.Any())))
                .ToList();
            return query;
        }

        private string PhanNhomChaCuaDuocPham(List<DuocPhamBenhVienPhanNhom> phanNhoms, DuocPhamBenhVienPhanNhom duocPhamBenhVienPhanNhom)
        {

            if (duocPhamBenhVienPhanNhom.CapNhom == 1)
            {
                return null;
            }
            else
            {
                if (duocPhamBenhVienPhanNhom.CapNhom == 2)
                {
                    return duocPhamBenhVienPhanNhom.Ten;
                }
                else
                {
                    if (duocPhamBenhVienPhanNhom.NhomCha != null)
                        return PhanNhomChaCuaDuocPham(phanNhoms, duocPhamBenhVienPhanNhom.NhomCha);
                    else
                    {
                        if (duocPhamBenhVienPhanNhom.NhomChaId != null)
                        {
                            var item = phanNhoms.FirstOrDefault(o => o.Id == (long)duocPhamBenhVienPhanNhom.NhomChaId);
                            if (item != null)
                            {
                                return PhanNhomChaCuaDuocPham(phanNhoms, item);
                            }
                            else
                            {
                                return null;
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            //if (duocPhamBenhVienPhanNhom.NhomCha != null)
            //    return PhanNhomChaCuaDuocPham(phanNhoms, duocPhamBenhVienPhanNhom.NhomCha);
            //else
            //{
            //    if (duocPhamBenhVienPhanNhom.NhomChaId != null)
            //    {
            //        var item = phanNhoms.FirstOrDefault(o => o.Id == (long)duocPhamBenhVienPhanNhom.NhomChaId);
            //        if (item != null)
            //        {
            //            if (item.NhomCha != null)
            //            {
            //                return PhanNhomChaCuaDuocPham(phanNhoms, item.NhomCha);
            //            }
            //            else
            //            {
            //                return item.Ten;
            //            }
            //        }
            //        else
            //        {
            //            return duocPhamBenhVienPhanNhom.Ten;
            //        }
            //    }
            //    else
            //    {
            //        return duocPhamBenhVienPhanNhom.Ten;
            //    }
            //}
        }

        public async Task<List<LookupItemVo>> DuocPhamBenhVienChaPhanNhoms(DropDownListRequestModel queryInfo)
        {
            var duocPhamBenhVienChaPhanNhoms = _repositoryDuocPhamBenhVienPhanNhom.TableNoTracking
                .Where(z => z.NhomChaId == null)
                .Select(item => new LookupItemVo
                {
                    KeyId = item.Id,
                    DisplayName = item.Ten
                }).ApplyLike(queryInfo.Query, o => o.DisplayName)
                .Take(queryInfo.Take);
            return await duocPhamBenhVienChaPhanNhoms.ToListAsync();
        }

        public async Task<List<NhomDichVuBenhVienPhanNhomTreeViewVo>> DichVuBenhVienPhanNhomsLv2VaLv3(DropDownListRequestModel model)
        {
            var duocPhamBenhVienPhanNhomChaId = CommonHelper.GetIdFromRequestDropDownList(model);
            var duocPhamBenhVienPhanNhoms = await _repositoryDuocPhamBenhVienPhanNhom.TableNoTracking
                .Select(item => new NhomDichVuBenhVienPhanNhomTreeViewVo
                {
                    KeyId = item.Id,
                    DisplayName = item.Ten,//item.Ma + " - " + item.Ten,
                    ParentId = item.NhomChaId
                })
                .ToListAsync();

            var query = duocPhamBenhVienPhanNhoms
                .Select(item => new NhomDichVuBenhVienPhanNhomTreeViewVo
                {
                    KeyId = item.KeyId,
                    DisplayName = item.DisplayName,
                    ParentId = item.ParentId,
                    Items = GetChildrenTree(duocPhamBenhVienPhanNhoms, item.KeyId, model.Query.RemoveVietnameseDiacritics(), item.DisplayName.RemoveVietnameseDiacritics())
                })
            .Where(x =>
                x.ParentId == duocPhamBenhVienPhanNhomChaId && (string.IsNullOrEmpty(model.Query) ||
                                       (!string.IsNullOrEmpty(model.Query) && (x.Items.Any() || x.DisplayName.RemoveVietnameseDiacritics().Trim().ToLower().Contains(model.Query.RemoveVietnameseDiacritics().Trim().ToLower())))))
            .ToList(); //.Take(model.Take) khách hang muốn xem hết
            return query;
        }

        public async Task<List<LookupItemVo>> PhanLoaiThuocTheoQuanLys(DropDownListRequestModel queryInfo)
        {
            var listPhanLoaiThuocTheoQuanLy = EnumHelper.GetListEnum<LoaiThuocTheoQuanLy>();
            var lookupItem = listPhanLoaiThuocTheoQuanLy
                .Select(item => new LookupItemVo
                {
                    KeyId = (long)item,
                    DisplayName = item.GetDescription()
                });
            if (!string.IsNullOrEmpty(queryInfo.Query))
            {
                lookupItem = lookupItem.Where(p => p.DisplayName.RemoveVietnameseDiacritics().ToLower().Contains(queryInfo.Query.RemoveVietnameseDiacritics().ToLower())).ToList();
            }
            return lookupItem.ToList();
        }



        public List<LookupItemVo> GetAllMayXetNghiem(DropDownListRequestModel queryInfo)
        {
            var getAllMayXetNghiems = _mayXetNghiemRepository.TableNoTracking.Where(z=> z.HieuLuc).Select(item => new LookupItemVo
            {
                KeyId = item.Id,
                DisplayName = item.Ma + "-" + item.Ten
            }).ApplyLike(queryInfo.Query, o => o.DisplayName).Take(queryInfo.Take);

            return getAllMayXetNghiems.ToList();
        }

        public async Task<List<DuocPhamBenhVienPhanNhom>> DuocPhamBenhVienPhanNhoms()
        {
            return await _duocPhamBenhVienPhanNhomRepository.TableNoTracking.ToListAsync();
        }
        public async Task<string> GetTenDuocPhamBenhVienPhanNhom(long duocPhamBenhVienPhanNhomId)
        {
            return await _duocPhamBenhVienPhanNhomRepository.TableNoTracking.Where(p => p.Id == duocPhamBenhVienPhanNhomId).Select(z => z.Ten).FirstAsync();
        }

        public async Task<bool> LaDuocPhamBenhVienPhanNhomCon(long duocPhamBenhVienPhanNhomId)
        {
            var nhomChaId = await _duocPhamBenhVienPhanNhomRepository.TableNoTracking.Where(p => p.Id == duocPhamBenhVienPhanNhomId).Select(z => z.NhomChaId).FirstAsync();
            if (nhomChaId == null)
            {
                return false;
            }
            return true;
        }

        #region //BVHD-345
        public async Task<string> GetMaTaoMoiDuocPhamAsync(MaDuocPhamTaoMoiInfoVo model)
        {
            // cấu trúc mã dược phẩm mới
            // 3 ký tự đầu tên dược phẩm
            // 1 ký tự phân nhóm dược phẩm
            // 3 số đánh thứ tự tên dược phẩm từ 001 -> 999
            // tổng ký tự mã là 7

            var maDuocPhamNew = string.Empty;

            var tenDuocPhamXoaDau = model.TenDuocPham.RemoveVietnameseDiacritics().Replace(" ", string.Empty);
            var baKyTuDauDuocPham = string.Empty; // tenDuocPhamXoaDau.Substring(0, 3); => chi lay alphabet
            var arrTenDuocPhamChar = tenDuocPhamXoaDau.ToCharArray();
            foreach (var tenChar in arrTenDuocPhamChar)
            {
                if (Char.IsLetter(tenChar))
                {
                    baKyTuDauDuocPham += tenChar;
                    if (baKyTuDauDuocPham.Length == 3)
                    {
                        break;
                    }
                }
            }

            var lstPhanNhom = await _duocPhamBenhVienPhanNhomRepository.TableNoTracking.ToListAsync();
            var kyTuPhanNhomDuocPham = GetKyHieuPhanNhom(lstPhanNhom, model.PhanNhomId);

            maDuocPhamNew = baKyTuDauDuocPham + kyTuPhanNhomDuocPham;
            var soThuTuDuocPham = 1;

            var @maDuocPhamTemp = maDuocPhamNew;
            var lstMaDuocPhamCungTen = await BaseRepository.TableNoTracking
                    .Where(x => x.MaDuocPhamBenhVien.Substring(0, 4).Contains(@maDuocPhamTemp)
                                && (model.DuocPhamNhatMaCuoiCungId == null || x.Id < model.DuocPhamNhatMaCuoiCungId))
                    .Select(x => x.MaDuocPhamBenhVien)
                    .ToListAsync();

            // dùng để check những mã đang được gán tạm trên FE
            lstMaDuocPhamCungTen
                .AddRange(
                    model.MaDuocPhamTemps
                        .Where(x => x.Substring(0, 4).Contains(@maDuocPhamTemp))
                        .Select(x => x)
                        .ToList()
                    );

            if (!lstMaDuocPhamCungTen.Any())
            {
                maDuocPhamNew += soThuTuDuocPham.ToString("000");
            }
            else
            {
                var lstSoThuTu = new List<int>();
                foreach (var maDuocPham in lstMaDuocPhamCungTen)
                {
                    var strSoThuTu = maDuocPham.Substring(4);
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
                    maDuocPhamNew += thuTuNews.First().ToString("000");
                }
            }

            return maDuocPhamNew.ToUpper();
        }

        private string GetKyHieuPhanNhom(List<DuocPhamBenhVienPhanNhom> lstDuocPhamBenhVienPhanNhoms, long? phanNhomId)
        {
            if (phanNhomId != null)
            {
                var phanNhom = lstDuocPhamBenhVienPhanNhoms.Where(x => x.Id == phanNhomId).FirstOrDefault();
                if (phanNhom != null)
                {
                    if (!string.IsNullOrEmpty(phanNhom.KyHieuPhanNhom))
                    {
                        return phanNhom.KyHieuPhanNhom;
                    }
                    else
                    {
                        return GetKyHieuPhanNhom(lstDuocPhamBenhVienPhanNhoms, phanNhom.NhomChaId ?? 0);
                    }
                }
            }

            return "K"; // mắc định nếu ko có ký hiệu phân nhóm là chữ K
        }

        public async Task<bool> KiemTraTrungDuocPhamBenhVienAsync(DuocPham duocPham)
        {
            var kiemTraTrung = await BaseRepository.TableNoTracking
                .AnyAsync(x => x.DuocPham.Ten.Contains(duocPham.Ten)
                               && ((string.IsNullOrEmpty(x.DuocPham.HamLuong) && string.IsNullOrEmpty(duocPham.HamLuong)) || x.DuocPham.HamLuong.Contains(duocPham.HamLuong))
                               && x.DuocPham.DonViTinhId == duocPham.DonViTinhId
                               && x.DuocPham.DuongDungId == duocPham.DuongDungId
                               && ((string.IsNullOrEmpty(x.DuocPham.HoatChat) && string.IsNullOrEmpty(duocPham.HoatChat)) || x.DuocPham.HoatChat.Contains(duocPham.HoatChat))
                               && ((string.IsNullOrEmpty(x.DuocPham.MaHoatChat) && string.IsNullOrEmpty(duocPham.MaHoatChat)) || x.DuocPham.MaHoatChat.Contains(duocPham.MaHoatChat))
                               && ((x.DuocPham.TheTich == null && duocPham.TheTich == null) || x.DuocPham.TheTich == duocPham.TheTich)
                               && ((string.IsNullOrEmpty(x.DuocPham.SoDangKy) && string.IsNullOrEmpty(duocPham.SoDangKy)) || x.DuocPham.SoDangKy.Contains(duocPham.SoDangKy))
                               && ((string.IsNullOrEmpty(x.DuocPham.NhaSanXuat) && string.IsNullOrEmpty(duocPham.NhaSanXuat)) || x.DuocPham.NhaSanXuat.Contains(duocPham.NhaSanXuat))
                               && ((string.IsNullOrEmpty(x.DuocPham.NuocSanXuat) && string.IsNullOrEmpty(duocPham.NuocSanXuat)) || x.DuocPham.NuocSanXuat.Contains(duocPham.NuocSanXuat)));
            return kiemTraTrung;
        }

        public async Task<bool> KiemTraTrungMaDuocPhamBenhVienAsync(long duocPhamBenhVienId, string maDuocPham, List<string> maDuocPhamTemps = null)
        {
            if (string.IsNullOrEmpty(maDuocPham) || maDuocPham.Length < 7)
            {
                return false;
            }

            var kiemTra = await BaseRepository.TableNoTracking
                .AnyAsync(x => (duocPhamBenhVienId == 0 || x.Id != duocPhamBenhVienId)
                               && x.MaDuocPhamBenhVien.Contains(maDuocPham));
            return kiemTra || (maDuocPhamTemps != null && maDuocPhamTemps.Contains(maDuocPham));
        }

        public void XuLyCapNhatMaDuocPhamBenhVien()
        {
            long duocPhamBenhVienIdPrev = 0;
            int take = 30;
            XDocument data = null;
            XElement duocPhamBenhVienElement = null;
            var path = @"Resource\\DuocPhamBenhVienDaCapNhatMaCuoiCung.xml";
            if (File.Exists(path))
            {
                data = XDocument.Load(path);
                XNamespace root = data.Root.GetDefaultNamespace();
                duocPhamBenhVienElement = data.Descendants(root + "DuocPhamBenhVien").FirstOrDefault();
                duocPhamBenhVienIdPrev = (long)duocPhamBenhVienElement.Element(root + "DuocPhamBenhVienId");
            }
            else
            {
                data =
                    new XDocument(
                        new XElement("DuocPhamBenhVien",
                            new XElement("DuocPhamBenhVienId", duocPhamBenhVienIdPrev.ToString())
                        )
                    );
                XNamespace root = data.Root.GetDefaultNamespace();
                duocPhamBenhVienElement = data.Descendants(root + "DuocPhamBenhVien").FirstOrDefault();
            }

            if (duocPhamBenhVienIdPrev != -1)
            {
                var duocPhamBenhVienIds = BaseRepository.TableNoTracking
                    .Where(x => x.Id > duocPhamBenhVienIdPrev)
                    .Select(x => x.Id)
                    .Take(take)
                    .ToList();
                if (duocPhamBenhVienIds.Any())
                {
                    foreach (var duocPhamBenhVienId in duocPhamBenhVienIds)
                    {
                        var duocPhamBenhVien = BaseRepository.Table
                            .Include(x => x.DuocPham)
                            .FirstOrDefault(x => x.Id == duocPhamBenhVienId
                                                 && x.DuocPham != null);
                        if (duocPhamBenhVien != null)
                        {
                            var obj = new MaDuocPhamTaoMoiInfoVo()
                            {
                                TenDuocPham = duocPhamBenhVien.DuocPham.Ten,
                                PhanNhomId = duocPhamBenhVien.DuocPhamBenhVienPhanNhomId,
                                DuocPhamNhatMaCuoiCungId = duocPhamBenhVienId
                            };
                            duocPhamBenhVien.MaDuocPhamBenhVien = GetMaTaoMoiDuocPhamAsync(obj).Result;
                            BaseRepository.Context.SaveChanges();
                        }
                    }

                    // cập nhật id cuối cùng
                    // trường hợp đã chạy hết dược phẩm bệnh viện thì bật cờ ko chạy job này nữa
                    if (duocPhamBenhVienIds.Count < take)
                    {
                        duocPhamBenhVienIdPrev = -1;
                    }
                    else
                    {
                        duocPhamBenhVienIdPrev = duocPhamBenhVienIds.OrderByDescending(x => x).First();
                    }
                    duocPhamBenhVienElement.Element("DuocPhamBenhVienId").Value = duocPhamBenhVienIdPrev.ToString();
                    data.Save(path);
                }
                else
                {
                    // trường hợp đã chạy hết dược phẩm bệnh viện thì bật cờ ko chạy job này nữa
                    duocPhamBenhVienIdPrev = -1;
                    duocPhamBenhVienElement.Element("DuocPhamBenhVienId").Value = duocPhamBenhVienIdPrev.ToString();
                    data.Save(path);
                }
            }
        }
        #endregion


        #region BVHD-3911

        public bool KiemTraNhomDuocPhamBatBuocNhapSoDangKy(string soDangKy, long? duocPhamBenhVienPhanNhomId)
        {
            if (duocPhamBenhVienPhanNhomId == null || duocPhamBenhVienPhanNhomId == 0)
            {
                return true;
            }

            var laNhomDuocPhamYeuCauNhapSoDangKy = duocPhamBenhVienPhanNhomId == (long)EnumDuocPhamBenhVienPhanNhom.TanDuoc
                                                   || duocPhamBenhVienPhanNhomId == (long)EnumDuocPhamBenhVienPhanNhom.SinhPham
                                                   || duocPhamBenhVienPhanNhomId == (long)EnumDuocPhamBenhVienPhanNhom.Vacxin
                                                   || duocPhamBenhVienPhanNhomId == (long)EnumDuocPhamBenhVienPhanNhom.MyPham
                                                   || duocPhamBenhVienPhanNhomId == (long)EnumDuocPhamBenhVienPhanNhom.ThucPhamChucNang
                                                   || duocPhamBenhVienPhanNhomId == (long)EnumDuocPhamBenhVienPhanNhom.ThietBiYTe
                                                   || duocPhamBenhVienPhanNhomId == (long)EnumDuocPhamBenhVienPhanNhom.SinhPhamChanDoan;

            // nếu nhóm thuốc các nhóm ở dưới, thì số đăng ký bắt buộc nhập
            return !laNhomDuocPhamYeuCauNhapSoDangKy || !string.IsNullOrEmpty(soDangKy);
        }

        #endregion
    }
}
