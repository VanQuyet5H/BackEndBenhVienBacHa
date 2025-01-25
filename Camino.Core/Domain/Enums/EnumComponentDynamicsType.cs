using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum ComponentDynamicsType
        {
            [Description("Label")]
            Label = 1,
            [Description("TextBox")]
            TextBox = 2,
            [Description("TextArea")]
            TextArea = 3,
            [Description("FieldSet")]
            FieldSet = 4,
            [Description("Combobox")]
            Combobox = 5,
            [Description("DatePicker")]
            DatePicker = 7
        }
    }
}
