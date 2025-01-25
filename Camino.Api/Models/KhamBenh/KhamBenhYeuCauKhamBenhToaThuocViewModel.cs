
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using System.Collections.Generic;

namespace Camino.Api.Models.KhamBenh
{
    public class KhamBenhYeuCauKhamBenhToaThuocViewModel
    {
        public long YeuCauKhamBenhId { get; set; }
        public long YeuCauKhamBenhTruocId { get; set; }
        public List<ApDungToaThuocChiTietVo> ToaThuocList { get; set; }
    }
}
