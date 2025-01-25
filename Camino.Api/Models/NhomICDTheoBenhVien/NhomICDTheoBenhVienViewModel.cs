using Camino.Core.Domain;
using System.Collections.Generic;

namespace Camino.Api.Models.QuocGia
{
    public class NhomICDTheoBenhVienViewModel : BaseViewModel
    {
        public string Stt { get; set; }
        public string Ma { get; set; }
        public string TenTiengViet { get; set; }
        public string TenTiengAnh { get; set; }
        public string MoTa { get; set; }
        public bool? HieuLuc { get; set; }

        public long? ChuongICDId { get; set; }
        public string TextChuongICD { get; set; }
        
        public List<NhomICDLienKetICDTheoBenhVienViewModel> NhomICDLienKetICDTheoBenhViens { get; set; }
        public List<string> MaICDs { get; set; }
    }   
    public class NhomICDLienKetICDTheoBenhVienViewModel : BaseViewModel
    {
        public long NhomICDTheoBenhVienId { get; set; }        
        public long? ICDId { get; set; }
    }
}
