namespace Camino.Core.Domain.ValueObject.Thuoc
{
    public class DuongDungTemplateVo
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public string Ten { get; set; }
        public string Ma { get; set; }
    }
    public class MaHoatChatHoatChatDuongDungTemplateVo
    {
        public string MaHoatChat { get; set; }
        public string HoatChat { get; set; }
        public string DuongDung { get; set; }
    }
}
