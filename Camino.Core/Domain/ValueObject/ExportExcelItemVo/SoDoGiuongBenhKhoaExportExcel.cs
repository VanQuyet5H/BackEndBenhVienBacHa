namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class SoDoGiuongBenhKhoaExportExcel
    {
        //public int STT { get; set; }
        [Width(50)]
        public string Ten { get; set; }
        [Width(20)]
        public int GiuongTrong { get; set; }
        [Width(25)]
        public int GiuongCoBenhNhan { get; set; }
        [Width(30)]
        public int TongGiuongBenhCuaKhoa { get; set; }
    }
}
