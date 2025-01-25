namespace Camino.Core.Domain.ValueObject.DichVuKhamBenh
{
    public class DichVuKhamBenhTemplateVo
    {
        public long KeyId { get; set; }

        public string DisplayName { get; set; }

        public string Ma { get; set; }

        public string Ten { get; set; }
    }
    public class DichVuKhamBenhBenhVienTemplateVo
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public string DichVu { get; set; }
        public string Ma { get; set; }
        public string MaDichVuTT37 { get; set; }
        public decimal? Gia { get; set; }
        public long? NhomGiaDichVuKhamBenhBenhVienId { get; set; }
    }
}
