using Camino.Core.Data;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.BaoCao
{
    public partial interface IBaoCaoService
    {
        Task<List<LookupItemVo>> GetKhoVatTuTraNCCLookupAsync(LookupQueryInfo queryInfo);
        Task<GridDataSource> GetDataBaoCaoTinhHinhTraVTYTNCCForGridAsync(BaoCaoTinhHinhTraVTYTNCCQueryInfo queryInfo);
        byte[] ExportBaoCaoTinhHinhTraVTYTNCC(GridDataSource gridDataSource, BaoCaoTinhHinhTraVTYTNCCQueryInfo query);
    }
}
