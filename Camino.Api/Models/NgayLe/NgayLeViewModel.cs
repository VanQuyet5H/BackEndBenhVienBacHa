namespace Camino.Api.Models.DanToc
{
    public class NgayLeViewModel : BaseViewModel
    {
        public NgayLeViewModel() { }

        public string Ten { get; set; }
        public int? Ngay { get; set; }
        public int? Thang { get; set; }
        public int? Nam { get; set; }
        public bool? LeHangNam { get; set; }
    }
}
