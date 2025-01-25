using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System.Collections.Generic;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.DonViHanhChinh
{
    public class DonViHanhChinhGridVo : GridItem
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string TenVietTat { get; set; }
        public CapHanhChinh CapHanhChinhId { get; set; }
        public string CapHanhChinh
        {
            get
            {
                return CapHanhChinhId.GetDescription();
            }
        }
        public long? TrucThuocDonViHanhChinhId { get; set; }
        public string TrucThuocDonViHanhChinh { get; set; }
    }
    public class DonViHanhChinhQueryInfo : QueryInfo
    {
        public CapHanhChinh CapHanhChinhId { get; set; }
        public long? TrucThuocDonViHanhChinhId { get; set; }
        public long? TinhThanhId { get; set; }
        public long? QuanHuyenId { get; set; }
        public long? PhuongXaId { get; set; }
        public long? KhomApId { get; set; }
        public string Ma { get; set; }
        public string TenVietTat { get; set; }
        public string Ten { get; set; }
    }
    public class DonViHanhChinhInfo 
    {
        public CapHanhChinh CapHanhChinhId { get; set; }
        public long? TrucThuocDonViHanhChinhId { get; set; }
        public long? TinhThanhId { get; set; }
        public long? QuanHuyenId { get; set; }
        public long? PhuongXaId { get; set; }
        public long? KhomApId { get; set; }
        public string Ma { get; set; }
        public string TenVietTat { get; set; }
        public string Ten { get; set; }
    }
    public class DonViHanhChinhLookupQueryInfo : LookupQueryInfo
    {
        public long? TinhThanhId { get; set; }
        public long? QuanHuyenId { get; set; }
        public long? PhuongXaId { get; set; }
    }
    public class DonViHanhChinhExcel : GridItem
    {
        public DonViHanhChinhExcel()
        {
            DonViHanhChinhs = new List<DonViHanhChinhExcel>();
        }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string TenVietTat { get; set; }
        public CapHanhChinh CapHanhChinhId { get; set; }
        public string CapHanhChinh
        {
            get
            {
                return CapHanhChinhId.GetDescription();
            }
        }
        public long? TrucThuocDonViHanhChinhId { get; set; }
        public string TrucThuocDonViHanhChinh { get; set; }
        public List<DonViHanhChinhExcel> DonViHanhChinhs { get; set; }
    }
}
