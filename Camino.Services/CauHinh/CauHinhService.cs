using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.CauHinhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Camino.Services.CauHinh
{
    [ScopedDependency(ServiceType = typeof(ICauHinhService))]
    public class CauHinhService : MasterFileService<Core.Domain.Entities.CauHinhs.CauHinh>, ICauHinhService
    {
        private readonly IRepository<CauHinhTheoThoiGian> _cauHinhTheoThoiGianRepository;
        private readonly IRepository<CauHinhThapGia> _cauHinhThapGiaRepository;
        private readonly IRepository<CauHinhTheoThoiGianChiTiet> _cauHinhTheoThoiGianChiTietRepository;
        private readonly IRepository<Core.Domain.Entities.CauHinhs.NgayLeTet> _ngayLeTetRepository;

        public CauHinhService(
            IRepository<Core.Domain.Entities.CauHinhs.CauHinh> repository,
            IRepository<CauHinhThapGia> cauHinhThapGiaRepository,
            IRepository<CauHinhTheoThoiGian> cauHinhTheoThoiGianRepository,
            IRepository<Core.Domain.Entities.CauHinhs.NgayLeTet> ngayLeTetRepository,
            IRepository<CauHinhTheoThoiGianChiTiet> cauHinhTheoThoiGianChiTietRepository) : base(repository)
        {
            _cauHinhTheoThoiGianRepository = cauHinhTheoThoiGianRepository;
            _cauHinhThapGiaRepository = cauHinhThapGiaRepository;
            _cauHinhTheoThoiGianChiTietRepository = cauHinhTheoThoiGianChiTietRepository;
            _ngayLeTetRepository = ngayLeTetRepository;
        }

        public GridDataSource GetDataForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var resultQuery = new List<CauHinhGrid>();

            var queryCauHinh = BaseRepository.TableNoTracking
                .Where(p =>
                EF.Functions.Like(p.Name, "%.%"));

            var queryCauHinhTheoThoiGian = _cauHinhTheoThoiGianRepository.TableNoTracking
                .Where(p =>
                    EF.Functions.Like(p.Name, "%.%"));

            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryCauHinh = queryCauHinh.ApplyLike(queryInfo.SearchTerms, g => g.Description);
                queryCauHinhTheoThoiGian = queryCauHinhTheoThoiGian.ApplyLike(queryInfo.SearchTerms, g => g.Description);
            }
            else
            {
                var queryString = JsonConvert.DeserializeObject<CauHinhGrid>(queryInfo.AdditionalSearchString);
                queryString.Description = queryString.Description?.TrimStart().TrimEnd();

                if (queryString.LoaiCauHinh != 0 && string.IsNullOrEmpty(queryString.Description))
                {
                    queryCauHinh = queryCauHinh.Where(x => x.Name.Contains(queryString.LoaiCauHinh.ToString()));
                    queryCauHinhTheoThoiGian = queryCauHinhTheoThoiGian.Where(x => x.Name.Contains(queryString.LoaiCauHinh.ToString()));
                }
                else if (!string.IsNullOrEmpty(queryString.Description))
                {
                    queryCauHinh = queryCauHinh.Where(p => EF.Functions.Like(p.Description, $"%{queryString.Description}%"));
                    queryCauHinhTheoThoiGian = queryCauHinhTheoThoiGian.Where(p => EF.Functions.Like(p.Description, $"%{queryString.Description}%"));
                }
            }

            resultQuery.AddRange(queryCauHinh.ToList().Select(p
                => new CauHinhGrid
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Value = FormatValue(p.DataType, p.Value),
                    IsCauHinh = true
                }));

            resultQuery.AddRange(queryCauHinhTheoThoiGian.ToList().Select(p
                => new CauHinhGrid
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Value = null,
                    IsCauHinh = false
                }));

            var queryIqueryable = resultQuery.AsQueryable();

            var countTask = queryIqueryable.Count();
            var queryTask = queryIqueryable.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArray();

            return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
        }

        private string FormatValue(Enums.DataType type, string value)
        {
            switch (type)
            {
                case Enums.DataType.Date:
                    return Convert.ToDateTime(value).ApplyFormatDate();
                case Enums.DataType.Time:
                    return Convert.ToInt32(value).ConvertIntSecondsToTime();
                case Enums.DataType.Datetime:
                    return Convert.ToDateTime(value).ApplyFormatDateTimeSACH();
                case Enums.DataType.Phone:
                    return value.ApplyFormatPhone();
            }
            return value;
        }

        public GridDataSource GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var resultQuery = new List<CauHinhGrid>();

            var queryCauHinh = BaseRepository.TableNoTracking
                .Where(p =>
                EF.Functions.Like(p.Name, "%.%"));

            var queryCauHinhTheoThoiGian = _cauHinhTheoThoiGianRepository.TableNoTracking
                .Where(p =>
                    EF.Functions.Like(p.Name, "%.%"));

            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryCauHinh = queryCauHinh.ApplyLike(queryInfo.SearchTerms, g => g.Description);
                queryCauHinhTheoThoiGian = queryCauHinhTheoThoiGian.ApplyLike(queryInfo.SearchTerms, g => g.Description);
            }
            else
            {
                var queryString = JsonConvert.DeserializeObject<CauHinhGrid>(queryInfo.AdditionalSearchString);


                if (queryString.LoaiCauHinh != 0 && string.IsNullOrEmpty(queryString.Description))
                {
                    queryCauHinh = queryCauHinh.Where(x => x.Name.Contains(queryString.LoaiCauHinh.ToString()));
                    queryCauHinhTheoThoiGian = queryCauHinhTheoThoiGian.Where(x => x.Name.Contains(queryString.LoaiCauHinh.ToString()));
                }
                else if (!string.IsNullOrEmpty(queryString.Description))
                {
                    queryCauHinh = queryCauHinh.Where(p => EF.Functions.Like(p.Description, $"%{queryString.Description}%"));
                    queryCauHinhTheoThoiGian = queryCauHinhTheoThoiGian.Where(p => EF.Functions.Like(p.Description, $"%{queryString.Description}%"));
                }
            }

            resultQuery.AddRange(queryCauHinh.ToList().Select(p
                => new CauHinhGrid
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Value = FormatValue(p.DataType, p.Value),
                    IsCauHinh = true
                }));

            resultQuery.AddRange(queryCauHinhTheoThoiGian.ToList().Select(p
                => new CauHinhGrid
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Value = null,
                    IsCauHinh = false
                }));

            var queryIqueryable = resultQuery.AsQueryable();

            var countTask = queryIqueryable.Count();

            return new GridDataSource { TotalRowCount = countTask };
        }

        #region Setting

        public Core.Domain.Entities.CauHinhs.CauHinh GetSetting(string key)
        {
            if (string.IsNullOrEmpty(key))
                return null;
            key = key.Trim().ToLowerInvariant();
            return BaseRepository.Table.FirstOrDefault(o => o.Name == key);
        }

        public T GetSettingByKey<T>(string key, T defaultValue = default(T))
        {
            if (string.IsNullOrEmpty(key))
                return defaultValue;

            key = key.Trim().ToLowerInvariant();
            var setting = BaseRepository.TableNoTracking.FirstOrDefault(o => o.Name == key);
            if (setting != null)
                return CommonHelper.To<T>(setting.Value);
            return defaultValue;
        }

        public T LoadSetting<T>() where T : ISettings, new()
        {
            return (T)LoadSetting(typeof(T));
        }

        public ISettings LoadSetting(Type type)
        {
            var settings = Activator.CreateInstance(type);

            foreach (var prop in type.GetProperties())
            {
                // get properties we can read and write to
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                var key = type.Name + "." + prop.Name;
                var setting = GetSettingByKey<string>(key);
                if (setting == null)
                    continue;

                if (!TypeDescriptor.GetConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                    continue;

                if (!TypeDescriptor.GetConverter(prop.PropertyType).IsValid(setting))
                    continue;

                var value = TypeDescriptor.GetConverter(prop.PropertyType).ConvertFromInvariantString(setting);

                //set property
                prop.SetValue(settings, value, null);
            }

            return settings as ISettings;
        }

        public void SaveSetting<T>(T settings) where T : ISettings, new()
        {
            foreach (var prop in typeof(T).GetProperties())
            {
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                if (!TypeDescriptor.GetConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                    continue;

                var key = typeof(T).Name + "." + prop.Name;
                dynamic value = prop.GetValue(settings, null);
                if (value != null)
                    SetSetting(key, value);
                else
                    SetSetting(key, "");
            }
        }

        public void SetSetting<T>(string key, T value)
        {
            if (key == null)
                throw new ArgumentNullException();
            key = key.Trim().ToLowerInvariant();
            var valueStr = TypeDescriptor.GetConverter(typeof(T)).ConvertToInvariantString(value);

            var setting = GetSetting(key);
            if (setting != null)
            {
                setting.Value = valueStr;
                Update(setting);
            }
            else
            {
                setting = new Core.Domain.Entities.CauHinhs.CauHinh
                {
                    Name = key,
                    Value = valueStr,
                    DataType = Enums.DataType.String
                };
                Add(setting);
            }
        }

        #endregion

        public async Task<bool> IsNameExists(string name, long id = 0)
        {
            var result = false;
            if (id == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Name.Equals(name));

            }
            else
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Name.Equals(name) && p.Id != id);
            }
            if (result)
                return false;
            return true;
        }
        public async Task<bool> IsValueExists(string value, long id = 0)
        {
            var result = false;
            if (id == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Value.Equals(value));

            }
            else
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Value.Equals(value) && p.Id != id);
            }
            if (result)
                return false;
            return true;
        }
        public List<LookupItemVo> getListLoaiCauHinh()
        {
            var listEnum = EnumHelper.GetListEnum<Enums.LoaiCauHinh>().Select(item => new LookupItemVo()
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item)
            }).ToList();
            return listEnum;
        }

        public Task<List<double>> GetTyLeHuongBaoHiem5LanKhamDichVuBHYT()
        {
            var result = new List<double>();
            result.Add(100);
            result.Add(30);
            result.Add(30);
            result.Add(30);
            result.Add(10);
            return Task.FromResult(result);
        }

        public Task<decimal> SoTienBHYTSeThanhToanToanBo()
        {
            decimal result = 223500;
            //decimal result = 100;
            return Task.FromResult(result);
        }

        public async Task<bool> IsTenCauHinhExists(string tenCauHinh = null, long cauHinhId = 0, int loaiCauHinh = 0)
        {
            bool result = false;

            if (loaiCauHinh == 1) {
                result = await BaseRepository.TableNoTracking.AnyAsync(p =>
                    p.Description.Equals(tenCauHinh) && p.Id != cauHinhId);
                result = await _cauHinhTheoThoiGianRepository.TableNoTracking.AnyAsync(p =>
                    p.Description.Equals(tenCauHinh) && p.Id != cauHinhId);
            }

            if (loaiCauHinh == 2)
            {
                result = await _cauHinhTheoThoiGianRepository.TableNoTracking.AnyAsync(p =>
                    p.Description.Equals(tenCauHinh) && p.Id != cauHinhId);
                result = await BaseRepository.TableNoTracking.AnyAsync(p =>
                    p.Description.Equals(tenCauHinh) && p.Id != cauHinhId);
            }

            return result;
        }

        public List<int> GetTiLeHuongBaoHiemDichVuPTTT()
        {
            return new List<int>{100,80,50};
        }
        public int GetTiLeTheoThapGia(Enums.LoaiThapGia loaiThapGia, decimal giaNhap,int vat = 0, long? khoNhapSauKhiDuyetId = null)
        {
            //update BACHA-427
            //Giá tồn tại Nhà thuốc = Đơn giá nhập chưa VAT
            //Gía tồn tại các kho thuốc = Đơn gía nhập chưa VAT +giá VAT
            //Căn cứ vào giá trị tồn kho thì sẽ tính được Hệ số bán ra theo Tháp giá
            if (loaiThapGia == Enums.LoaiThapGia.ThuocKhongBaoHiem)
            {
                var giaTon = khoNhapSauKhiDuyetId == (long) Enums.EnumKhoDuocPham.KhoNhaThuoc ? giaNhap : Math.Round((giaNhap + (giaNhap*vat/100)),2);
                return _cauHinhThapGiaRepository.TableNoTracking.FirstOrDefault(o => o.LoaiThapGia == loaiThapGia && (o.GiaTu == null || o.GiaTu < giaTon) && (o.GiaDen == null || giaTon <= o.GiaDen))?.TiLeTheoThapGia ?? 0;
            }
            return _cauHinhThapGiaRepository.TableNoTracking.FirstOrDefault(o =>o.LoaiThapGia == loaiThapGia && (o.GiaTu == null || o.GiaTu < giaNhap) && (o.GiaDen == null || giaNhap <= o.GiaDen))?.TiLeTheoThapGia ?? 0;
        }
        public Core.Domain.Entities.CauHinhs.CauHinh GetByName(string name) {
            return BaseRepository.TableNoTracking.FirstOrDefault(o=>o.Name==name);
        }

        public async Task<decimal> GetDonGiaThuePhongAsync(ThuePhong thuePhong)
        {
            var cauHinhChung = LoadSetting<CauHinhChung>();
            var ngayLeTets = _ngayLeTetRepository.TableNoTracking.ToList();
            List<DayOfWeek> ngayNghis = new List<DayOfWeek>();
            if (!string.IsNullOrEmpty(cauHinhChung.NgayLamViec))
            {
                for (int i = 0; i < cauHinhChung.NgayLamViec.Length; i++)
                {
                    if (cauHinhChung.NgayLamViec[i] == '0')
                    {
                        switch (i)
                        {
                            case 0:
                                ngayNghis.Add(DayOfWeek.Monday);
                                break;
                            case 1:
                                ngayNghis.Add(DayOfWeek.Tuesday);
                                break;
                            case 2:
                                ngayNghis.Add(DayOfWeek.Wednesday);
                                break;
                            case 3:
                                ngayNghis.Add(DayOfWeek.Thursday);
                                break;
                            case 4:
                                ngayNghis.Add(DayOfWeek.Friday);
                                break;
                            case 5:
                                ngayNghis.Add(DayOfWeek.Saturday);
                                break;
                            case 6:
                                ngayNghis.Add(DayOfWeek.Sunday);
                                break;
                        }
                    }
                }
            }            

            if (thuePhong.ThoiDiemBatDau >= thuePhong.ThoiDiemKetThuc)
            {
                return 0;
            }
            int soPhutTrongGio = 0;
            int soPhutNgoaiGio = 0;
            int soPhutNgayLeTet = 0;
            var tick = thuePhong.ThoiDiemBatDau;
            while(tick < thuePhong.ThoiDiemKetThuc)
            {
                if(ngayLeTets.Any(o=>o.Ngay == tick.Day && o.Thang == tick.Month && (o.LeHangNam == true || o.Nam == tick.Year)))
                {
                    soPhutNgayLeTet += 1;
                }
                else
                {
                    if (ngayNghis.Contains(tick.DayOfWeek))
                    {
                        soPhutNgoaiGio += 1;
                    }
                    else
                    {
                        var gioBatDauLam = tick.Date.AddSeconds(cauHinhChung.GioBatDauLamViec);
                        if(tick < gioBatDauLam)
                        {
                            soPhutNgoaiGio += 1;
                        }
                        else
                        {
                            var gioKetThucLam = tick.Date.AddSeconds(cauHinhChung.GioKetThucLamViec);
                            if(tick >= gioKetThucLam)
                            {
                                soPhutNgoaiGio += 1;
                            }
                            else
                            {
                                soPhutTrongGio += 1;
                            }
                        }
                    }
                }
                tick = tick.AddMinutes(1);
            }
            var tongSoPhut = soPhutTrongGio + soPhutNgoaiGio + soPhutNgayLeTet;
            if (soPhutTrongGio * 2 >= tongSoPhut)
            {
                //tinh trong gio
                return TinhGiaThuePhong(tongSoPhut, thuePhong.BlockThoiGianTheoPhut, thuePhong.GiaThue, thuePhong.GiaThuePhatSinh);                
            }
            else
            {
                if(soPhutNgayLeTet * 2 > tongSoPhut)
                {
                    //tinh theo le tet
                    return TinhGiaThuePhong(tongSoPhut, thuePhong.BlockThoiGianTheoPhut, thuePhong.GiaThue, thuePhong.GiaThuePhatSinh, thuePhong.PhanTramLeTet, thuePhong.PhanTramPhatSinhLeTet);
                }
                else
                {
                    //tinh ngoai gio
                    return TinhGiaThuePhong(tongSoPhut, thuePhong.BlockThoiGianTheoPhut, thuePhong.GiaThue, thuePhong.GiaThuePhatSinh, thuePhong.PhanTramNgoaiGio, thuePhong.PhanTramPhatSinhNgoaiGio);
                }
            }
        }
        private decimal TinhGiaThuePhong(int tongSoPhut, int blockThoiGianTheoPhut, decimal giaThue, decimal giaThuePhatSinh, int phanTram = 0, int phanTramPhatSinh = 0)
        {
            if (tongSoPhut <= blockThoiGianTheoPhut)
            {
                return Math.Round(giaThue + (giaThue * phanTram / 100));
            }
            else
            {
                var soPhutPhatSinh = tongSoPhut - blockThoiGianTheoPhut;
                return Math.Round(giaThue + (giaThue * phanTram / 100) + ((giaThuePhatSinh + (giaThuePhatSinh * phanTramPhatSinh / 100)) / 60 * soPhutPhatSinh));
            }
        }
    }
}
