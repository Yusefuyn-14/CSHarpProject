using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Management;
using System.Management.Instrumentation;

namespace ManagementInstrumentationProject.Concretes
{
    public class WMIClassElement
    {
        public CimType Type { get; set; }
        public string Name { get; set; }
        public object Value { get; set; }
    }
}
