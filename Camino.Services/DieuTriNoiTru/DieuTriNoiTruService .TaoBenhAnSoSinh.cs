using Camino.Core.Data;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Services.DieuTriNoiTru
{
    public partial class DieuTriNoiTruService
    {
        public List<ThongTinBenhAnMe> ChonBenhAnMe(DropDownListRequestModel model)
        {
            var query = BaseRepository.TableNoTracking.Where(o => o.NoiTruBenhAn.ThoiDiemRaVien == null && (
                      o.NoiTruBenhAn.LoaiBenhAn == Core.Domain.Enums.LoaiBenhAn.PhuKhoa
                   || o.NoiTruBenhAn.LoaiBenhAn == Core.Domain.Enums.LoaiBenhAn.SanKhoaMo
                   || o.NoiTruBenhAn.LoaiBenhAn == Core.Domain.Enums.LoaiBenhAn.SanKhoaThuong))
                   .ApplyLike(model.Query, p => p.NoiTruBenhAn.SoBenhAn, p => p.BenhNhan.MaBN,
                              p => p.BenhNhan.HoTen, p => p.MaYeuCauTiepNhan)
                .Select(s => new ThongTinBenhAnMe
                {

                    KeyId = s.Id,
                    DisplayName = s.NoiTruBenhAn.SoBenhAn,
                    Id = s.Id,
                    YeuCauTiepNhanMeId = s.Id,
                    MaBA = s.NoiTruBenhAn.SoBenhAn,
                    HoTen = s.HoTen,
                    MaBN = s.BenhNhan.MaBN,
                    MaTN = s.MaYeuCauTiepNhan,
                    NamSinh = s.NamSinh.ToString()
                });

            return query.ToList();
        }

        public List<LookupItemVo> KhoaChuyenBenhAnSoSinhVe(DropDownListRequestModel model)
        {
            var khoaSan = _cauHinhService.GetSetting("CauHinhNoiTru.KhoaPhuSan");
            long.TryParse(khoaSan?.Value, out long khoaSanId);
            var khoaNhi = _cauHinhService.GetSetting("CauHinhNoiTru.KhoaNhi");
            long.TryParse(khoaNhi?.Value, out long khoaNhiId);

            var LookupItemVos = new List<LookupItemVo>()
            {
                new LookupItemVo { KeyId = khoaNhiId, DisplayName = khoaNhi.Description },
                new LookupItemVo { KeyId = khoaSanId, DisplayName = khoaSan.Description }
            };

            return LookupItemVos;
        }
    }
}
