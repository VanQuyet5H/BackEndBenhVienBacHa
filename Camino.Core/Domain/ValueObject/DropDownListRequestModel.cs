namespace Camino.Core.Domain.ValueObject
{
    public class DropDownListRequestModel
    {
        public string ParameterDependencies { get; set; }
        public long Id { get; set; }
        public string Query { get; set; }
        public int Take { get; set; }  
    }
}