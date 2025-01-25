using Camino.Core.Data;
using Camino.Core.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Services.DieuTriNoiTru
{
    public partial class DieuTriNoiTruService
    {
        public async Task<List<string>> GetListNhanVienAutoComplete(DropDownListRequestModel queryInfo)
        {
            var lstNhanVien = _nhanVienRepository.TableNoTracking.Select(p => p.User.HoTen)
                                                                 .ApplyLike(queryInfo.Query, o => o)
                                                                 .Take(queryInfo.Take);

            return await lstNhanVien.ToListAsync();
        }

        public async Task<List<string>> GetListChucDanhAutoComplete(DropDownListRequestModel queryInfo)
        {
            var lstChucDanh = _chucDanhRepository.TableNoTracking.Select(p => p.Ten)
                                                                 .ApplyLike(queryInfo.Query, o => o)
                                                                 .Take(queryInfo.Take);

            return await lstChucDanh.ToListAsync();
        }

        public async Task<List<string>> GetListVanBangChuyenMonAutoComplete(DropDownListRequestModel queryInfo)
        {
            var lstVanBang = _vanBangChuyenMonRepository.TableNoTracking.Select(p => p.Ten)
                                                                        .ApplyLike(queryInfo.Query, o => o)
                                                                        .Take(queryInfo.Take);

            return await lstVanBang.ToListAsync();
        }
    }
}
