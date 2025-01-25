using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        string InGiayChungSinh(long hoSoKhacId,string hosting);
        string InGiayChungSinhTatCa(long hoSoKhacId, string hosting);
        Task<ThongTinNhanVienDangNhap> ThongTinNhanVienDangNhapIdTreSoSinh(long id);
        bool CheckNgayCapGiayChungSinh(long yeuCauTiepNhan, DateTime? ngayCapChungSinh);
        bool kiemTrungSoChungSinh(string so, long yctn ,long? noiTruHoSoKhacGiayChungSinhId);
        Task<List<InfoBAConGridVo>> GetDataInfoBACon(DropDownListRequestModel model, long yeuCauTiepNhanMeId);
        string GetNameBacSi(long id);
    }
}
