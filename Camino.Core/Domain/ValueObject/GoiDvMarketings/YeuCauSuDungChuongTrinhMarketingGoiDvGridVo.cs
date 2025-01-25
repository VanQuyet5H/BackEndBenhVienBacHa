using System;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.GoiDvMarketings
{
    public class YeuCauSuDungChuongTrinhMarketingGoiDvGridVo : GridItem
    {
        public string MaBn { get; set; }

        public string TenBn { get; set; }

        public string DiaChi { get; set; }

        public DateTime NgayDangKy { get; set; }

        public string NgayDangKyDisplay => NgayDangKy.ApplyFormatDate();
    }
}
