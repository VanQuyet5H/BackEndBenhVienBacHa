using Camino.Core.Domain.ValueObject.Grid;
using System.Collections.Generic;

namespace Camino.Core.Domain.ValueObject
{
    public class QuocGiaGridVo : GridItem
    {
        public string Ma { get; set; }

        public string Ten { get; set; }

        public string TenVietTat { get; set; }

        public string QuocTich { get; set; }

        public string MaDienThoaiQuocTe { get; set; }

        public string ThuDo { get; set; }

        public string ChauLuc { get; set; }
        //public Enums.EnumChauLuc ChauLuc { get; set; }

        public bool? IsDisabled { get; set; }

        //public virtual NhaSanXuatTheoQuocGias.NhaSanXuatTheoQuocGiasGridVo NhaSanXuatTheoQuocGia { get; set; }
    }
}
