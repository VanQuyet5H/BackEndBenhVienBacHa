using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;

namespace Camino.Core.Domain.Entities.TemplateKhamBenhTheoDichVus
{
    public class TemplateKhamBenhTheoDichVu : BaseEntity
    {
        public long DichVuKhamBenhBenhVienId { get; set; }

        public string Ten { get; set; }

        public string TieuDe { get; set; }

        public string ComponentDynamics { get; set; }

        public virtual DichVuKhamBenhBenhVien DichVuKhamBenhBenhVien { get; set; }
    }
}
