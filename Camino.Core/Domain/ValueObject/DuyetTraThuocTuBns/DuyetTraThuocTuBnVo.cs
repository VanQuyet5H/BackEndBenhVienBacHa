using System;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.DuyetTraThuocTuBns
{
    public class DuyetTraThuocTuBnVo : GridItem
    {
        public string SoPhieu { get; set; }
        
        public long? KhoaHoanTraId { get; set; }
        
        public string KhoaHoanTraDisplay { get; set; }
        
        public long? HoanTraVeKhoId { get; set; }
        
        public string HoanTraVeKhoDisplay { get; set; }
        
        public long? NguoiYeuCauId { get; set; }
        
        public string NguoiYeuCauDisplay { get; set; }

        public long? NguoiDuyetId { get; set; }

        public string NguoiDuyetDisplay { get; set; }
        
        public DateTime? NgayYeuCau { get; set; }
        
        public string NgayYeuCauDisplay => NgayYeuCau?.ApplyFormatDateTime();

        public DateTime? NgayDuyet { get; set; }
        
        public string NgayDuyetDisplay => NgayDuyet?.ApplyFormatDateTime();

        public string GhiChu { get; set; }
        
        public bool? TinhTrang { get; set; }
        
        public string TinhTrangDisplay => TinhTrang == true ? "<span class='green-txt'>Đã duyệt</span>" : "<span class='orange-txt'>Chờ duyệt</span>";
    }
}
