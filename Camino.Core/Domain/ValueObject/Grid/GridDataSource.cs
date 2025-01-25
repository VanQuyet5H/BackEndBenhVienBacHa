using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.Grid
{
    public class GridDataSource
    {
        public ICollection<GridItem> Data { get; set; }

        public int TotalRowCount { get; set; }
    }
}
