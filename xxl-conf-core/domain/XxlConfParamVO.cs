using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xxl_conf_core.domain
{
    public class XxlConfParamVO
    {
        public string AccessToken { get; set; }
        public string Env { get; set; }
        public List<string> Keys { get; set; }
    }
}
