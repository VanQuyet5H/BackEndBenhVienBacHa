using System.Collections.Generic;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.Users;

namespace Camino.Api.Models.Users
{
    public class RoleViewModel : BaseViewModel
    {
        public RoleViewModel()
        {
            RoleFunctions = new List<RoleFunctionViewModel>();
            RoleFunctionGrids = new List<RoleFunctionGrids>();
        }
        public string Name { get; set; }
        public Enums.UserType UserType { get; set; }
        public bool IsDefault { get; set; }
        public bool IsCheckAll { get; set; }
        public List<RoleFunctionViewModel> RoleFunctions { get; set; }
        public List<RoleFunctionGrids> RoleFunctionGrids { get; set; }
    }
}