
using static Camino.Core.Domain.Enums;
namespace Camino.Api.Models.InputStringStoreds
{
    public class InputStringStoredViewModel : BaseViewModel
    {
        public InputStringStoredKey? Set { get; set; }
        public string Value { get; set; }
    }
}
