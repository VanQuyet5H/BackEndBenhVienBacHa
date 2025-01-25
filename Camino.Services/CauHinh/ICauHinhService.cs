using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.CauHinh
{
    public interface ICauHinhService : IMasterFileService<Core.Domain.Entities.CauHinhs.CauHinh>
    {
        GridDataSource GetDataForGridAsync(QueryInfo queryInfo);
        GridDataSource GetTotalPageForGridAsync(QueryInfo queryInfo);
        Core.Domain.Entities.CauHinhs.CauHinh GetSetting(string key);
        T GetSettingByKey<T>(string key, T defaultValue = default(T));
        T LoadSetting<T>() where T : ISettings, new();
        ISettings LoadSetting(Type type);
        void SaveSetting<T>(T settings) where T : ISettings, new();
        void SetSetting<T>(string key, T value);
        List<LookupItemVo> getListLoaiCauHinh();
        Task<bool> IsNameExists(string name, long id = 0);
        Task<bool> IsValueExists(string value, long id = 0);
        Task<decimal> SoTienBHYTSeThanhToanToanBo();
        Task<List<double>> GetTyLeHuongBaoHiem5LanKhamDichVuBHYT();

        Task<bool> IsTenCauHinhExists(string tenCauHinh = null, long cauHinhId = 0, int loaiCauHinh = 0);
        List<int> GetTiLeHuongBaoHiemDichVuPTTT();
        int GetTiLeTheoThapGia(Enums.LoaiThapGia loaiThapGia, decimal giaNhap, int vat = 0, long? khoNhapSauKhiDuyetId = null);
        Core.Domain.Entities.CauHinhs.CauHinh GetByName(string name);

        Task<decimal> GetDonGiaThuePhongAsync(ThuePhong thuePhong);
    }
}
