using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Data;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.KetQuaVaKetLuanMau
{
    [ScopedDependency(ServiceType = typeof(IKetQuaVaKetLuanMauService))]
    public class KetQuaVaKetLuanMauService : MasterFileService<Core.Domain.Entities.KetQuaVaKetLuanMaus.KetQuaVaKetLuanMau>, IKetQuaVaKetLuanMauService
    {
        public KetQuaVaKetLuanMauService(IRepository<Core.Domain.Entities.KetQuaVaKetLuanMaus.KetQuaVaKetLuanMau> repository) : base(repository)
        {

        }

        public async Task<bool> KiemTraTrungMa(long id, string ma)
        {
            if (string.IsNullOrEmpty(ma))
            {
                return false;
            }

            var kiemTra = await BaseRepository.TableNoTracking.AnyAsync(x => (id == 0 || x.Id != id) 
                                                                             //&& x.LoaiKetQuaVaKetLuanMau == loaiMau
                                                                             && x.Ma.ToLower().Trim() == ma.ToLower().Trim());
            return kiemTra;
        }

        #region lookup
        public async Task<List<NoiDungMauLookupItemVo>> GetListNoiDungMauAsync(DropDownListRequestModel model)
        {
            var lst = await
                BaseRepository.TableNoTracking
                    //.Where(x => (isKetQuaMau && x.LoaiKetQuaVaKetLuanMau == Enums.LoaiKetQuaVaKetLuanMau.KetQuaMau) || (!isKetQuaMau && x.LoaiKetQuaVaKetLuanMau == Enums.LoaiKetQuaVaKetLuanMau.KetLuanMau))
                    .Select(item => new NoiDungMauLookupItemVo()
                    {
                        DisplayName = item.Ten,
                        Ma = item.Ma,
                        Ten = item.Ten,
                        KetQuaMau = item.KetQuaMau,
                        KetLuanMau = item.KetLuanMau,
                        KeyId = item.Id
                    })
                    .ApplyLike(model.Query, x => x.DisplayName)
                    .OrderByDescending(x => x.KeyId == model.Id).ThenBy(x => x.DisplayName)
                    .Take(model.Take)
                    .ToListAsync();
            return lst;
        }


        #endregion
    }
}
