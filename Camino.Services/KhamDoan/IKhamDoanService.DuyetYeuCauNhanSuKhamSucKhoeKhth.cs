using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.KhamDoan;

namespace Camino.Services.KhamDoan
{
    public partial interface IKhamDoanService
    {
        Task TuChoiDuyetKhthAsync(TuChoiDuyetKhthRequest tuChoiDuyet);

        Task DuyetKhthAsync(long id);
    }
}
