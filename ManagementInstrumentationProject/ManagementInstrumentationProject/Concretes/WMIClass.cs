using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementInstrumentationProject.Concretes
{
    public class WMIClass
    {
        public string Name { get; set; }
        public List<WMIClassElement> elements { get; set; }
    }
}
