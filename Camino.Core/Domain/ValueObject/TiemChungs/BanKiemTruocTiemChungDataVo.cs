using System.Collections.Generic;

namespace Camino.Core.Domain.ValueObject.TiemChungs
{
    public class BanKiemTruocTiemChungDataVo
    {
        public BanKiemTruocTiemChungDataVo()
        {
            DataKhamTheoTemplate = new List<BanKiemTruocTiemChungDataChiTietVo>();
        }

        //public bool IsTrongBenhVien { get; set; }
        public List<BanKiemTruocTiemChungDataChiTietVo> DataKhamTheoTemplate { get; set; }
    }

    public class BanKiemTruocTiemChungDataChiTietVo
    {
        public string Id { get; set; }
        public string Value { get; set; }
    }

    public class PhanLoaiBanKiemTruocTiemChungDataVo
    {
        public Enums.NhomKhamSangLoc NhomKhamSangLoc { get; set; }
    }
}