using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Camino.Core.Domain.ValueObject
{
    public class LookupItemVo
    {
        public long KeyId { get; set; }

        public string DisplayName { get; set; }
    }
    public class LookupItemTextVo
    {
        public string KeyId { get; set; }
        public string DisplayName { get; set; }
    }
    public class SrcFile
    {
        public string DuongDan { get; set; }
        public string TenGuid { get; set; }
    }

    public class LookupItemTemplateVo
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public string Ten { get; set; }
        public string Ma { get; set; }
    }
    public class LookupItemTemplate
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public int CapNhom { get; set; }
        public long? NhomChaId { get; set; }

    }
    public class LookupItemVoLichSuKham
    {
        public long KeyId { get; set; }

        public long Id { get; set; }
        public bool CoChiecKhau { get; set; }
    }

    public class LookupTreeItemVo : LookupItemVo
    {
        public LookupTreeItemVo()
        {
            Items = new List<LookupTreeItemVo>();
        }

        public bool IsDisabled
        {
            get => Items != null && Items.Any();
        }
        public int Level { get; set; }
        public long? ParentId { get; set; }
        public List<LookupTreeItemVo> Items { get; set; }
    }

    public class LookupCheckItemVo : LookupItemTextVo
    {
        public bool IsCheck { get; set; }
    }

    public class LookupItemCauHinhVo
    {
        public string KeyId { get; set; }
        public string DisplayName { get; set; }
        public string Value { get; set; }
        public int DataType { get; set; }
        public string GhiChu { get; set; }
    }

    public class LookupItemYeuCauTruyenMauVo : LookupItemVo
    {
        public string MaYeuCauTiepNhan { get; set; }
        public string MaBenhNhan { get; set; }
        public string MaBenhAn { get; set; }
        public string HoTen { get; set; }
        public string MaChePhamMau { get; set; }
        public long ChePhamMauId { get; set; }
        public string TenChePhamMau { get; set; }
        public int PhanLoaiMau { get; set; }
        public string NhomMau { get; set; }
        public string TheTichDisplay { get; set; }
        public decimal TheTich { get; set; }
    }
    public class LookupItemBacSiTemplateVo
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public string MaChucDanh { get; set; }
        public string MaNV { get; set; }
    }
}
