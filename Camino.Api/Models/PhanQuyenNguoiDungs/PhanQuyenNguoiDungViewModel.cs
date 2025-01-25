using System.Collections.Generic;
using Camino.Core.Domain;

namespace Camino.Api.Models.PhanQuyenNguoiDungs
{
    public class PhanQuyenNguoiDungViewModel : BaseViewModel
    {
        public PhanQuyenNguoiDungViewModel()
        {
            DocumentTypes = new List<DocumentTypeViewModel>();
        }

        public bool? IsDefault { get; set; }

        public string Name { get; set; }

        public Enums.UserType? UserType { get; set; }

        public string UserTypeDisplay { get; set; }

        public List<DocumentTypeViewModel> DocumentTypes { get; set; }
    }
}
