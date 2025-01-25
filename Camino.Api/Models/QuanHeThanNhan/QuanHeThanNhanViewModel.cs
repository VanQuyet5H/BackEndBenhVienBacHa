namespace Camino.Api.Models.QuanHeThanNhan
{
    public class QuanHeThanNhanViewModel : BaseViewModel
    {
        public string Ten { get; set; }

        public string TenVietTat { get; set; }

        public string MoTa { get; set; }

        public bool? IsDisabled { get; set; }
    }
}
