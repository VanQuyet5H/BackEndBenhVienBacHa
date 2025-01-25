using Camino.Core.Domain;

namespace Camino.Api.Models.PhanQuyenNguoiDungs
{
    public class DocumentTypeViewModel : BaseViewModel
    {
        public long? IdParent { get; set; }

        public bool? IsDelete { get; set; }

        public bool? IsInsert { get; set; }

        public bool? IsProcess { get; set; }

        public bool? IsUpdate { get; set; }

        public bool? IsView { get; set; }

        public bool? IsChange { get; set; }

        public Enums.DocumentType? DocumentType { get; set; }
    }
}
