namespace Camino.Api.Models.Khoa
{
    public class KhoaViewModel : BaseViewModel
    {
        public string Ten { get; set; }

        public string Ma { get; set; }

        public string MoTa { get; set; }

        public bool? IsDisabled { get; set; }
    }
}
