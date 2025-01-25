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

namespace Camino.Services.NoiDungMauLoiDanBacSi
{
    [ScopedDependency(ServiceType = typeof(INoiDungMauLoiDanBacSiService))]
    public class NoiDungMauLoiDanBacSiService : MasterFileService<Core.Domain.Entities.NoiDungMauLoiDanBacSi.NoiDungMauLoiDanBacSi>, INoiDungMauLoiDanBacSiService
    {
        public NoiDungMauLoiDanBacSiService(IRepository<Core.Domain.Entities.NoiDungMauLoiDanBacSi.NoiDungMauLoiDanBacSi> repository) : base(repository)
        {

        }

        public async Task<bool> KiemTraTrungMa(long id, string ma, long loaiBenhAn)
        {
            if (string.IsNullOrEmpty(ma))
            {
                return false;
            }

            var kiemTra = await BaseRepository.TableNoTracking.AnyAsync(x => (id == 0 || x.Id != id) && x.LoaiBenhAn == loaiBenhAn
                                                                             && x.Ma.ToLower().Trim() == ma.ToLower().Trim());
            return kiemTra;
        }

        public async Task<List<NoiDungLoiDanBacSiLookupItemVo>> GetListNoiDungMauAsync(DropDownListRequestModel model, long LoaiBenhAn)
        {
            var lst = await
                BaseRepository.TableNoTracking.Where(x => x.LoaiBenhAn == LoaiBenhAn)
                    .Select(item => new NoiDungLoiDanBacSiLookupItemVo()
                    {
                        DisplayName = item.Ten,
                        Ma = item.Ma,
                        Ten = item.Ten,
                        HuongXuLyLoiDanBacSi = item.HuongXuLyLoiDanBacSi,
                        KeyId = item.Id
                    })
                    .ApplyLike(model.Query, x => x.DisplayName)
                    .OrderByDescending(x => x.KeyId == model.Id).ThenBy(x => x.DisplayName)
                    .Take(model.Take)
                    .ToListAsync();
            return lst;
        }
    }
}
