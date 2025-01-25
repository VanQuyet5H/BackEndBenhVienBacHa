using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Text;

namespace Camino.Core.Domain.ValueObject
{
    public class CaminoPermission
    {
        public Enums.DocumentType DocumentType { get; set; }
        public Enums.SecurityOperation SecurityOperation { get; set; }
    }
}
