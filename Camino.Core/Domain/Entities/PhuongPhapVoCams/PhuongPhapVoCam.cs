using System.Collections.Generic;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;

namespace Camino.Core.Domain.Entities.PhuongPhapVoCams
{
    public class PhuongPhapVoCam : BaseEntity
    {
        public string Ma { get; set; }

        public string Ten { get; set; }

        public string MoTa { get; set; }

        private ICollection<YeuCauDichVuKyThuatTuongTrinhPTTT> _yeuCauDichVuKyThuatTuongTrinhPTTTs;
        public virtual ICollection<YeuCauDichVuKyThuatTuongTrinhPTTT> YeuCauDichVuKyThuatTuongTrinhPTTTs
        {
            get => _yeuCauDichVuKyThuatTuongTrinhPTTTs ?? (_yeuCauDichVuKyThuatTuongTrinhPTTTs = new List<YeuCauDichVuKyThuatTuongTrinhPTTT>());
            protected set => _yeuCauDichVuKyThuatTuongTrinhPTTTs = value;
        }
    }
}
