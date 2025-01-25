using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.Users
{
    public class RoleGridVo : GridItem
    {
        public string Name { get; set; }
        public bool? IsDefault { get; set; }
    }
}