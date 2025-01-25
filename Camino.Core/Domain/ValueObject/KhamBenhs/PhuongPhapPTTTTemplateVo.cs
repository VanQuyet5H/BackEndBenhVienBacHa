using System.Collections.Generic;

namespace Camino.Core.Domain.ValueObject.KhamBenhs
{
    public class PhuongPhapPTTTTemplateVo
    {
        public string KeyId { get; set; }

        public string DisplayName => Ten;

        public string Ten { get; set; }

        public string Ma { get; set; }
    }
    public class VoMuTiSelect
    {
        public string PhuongThucPhauThuats { get; set; }

    }
}
