using Camino.Core.Domain.ValueObject.Grid;
namespace Camino.Core.Domain.ValueObject.Thuoc
{
    public class DuongDungGridVo : GridItem
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string MoTa { get; set; }
        public bool? IsDisabled { get; set; }
    }
}
