using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xxl_conf_core.domain
{
    public class BeanField
    {
        public String BeanName { get; set; }
        public String Property { get; set; }

        public BeanField()
        {
        }

        public BeanField(String beanName, String property)
        {
            this.BeanName = beanName;
            this.Property = property;
        }


    }
}
