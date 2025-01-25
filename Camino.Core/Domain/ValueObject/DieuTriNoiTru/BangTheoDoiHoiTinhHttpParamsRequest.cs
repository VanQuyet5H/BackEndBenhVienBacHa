namespace Camino.Core.Domain.ValueObject.DieuTriNoiTru
{
    public class BangTheoDoiHoiTinhHttpParamsRequest
    {
        public long YeuCauTiepNhanId { get; set; }

        public long? IdNoiTruHoSo { get; set; }

        public string HostingName { get; set; }

        public bool? Header { get; set; }
    }
}
