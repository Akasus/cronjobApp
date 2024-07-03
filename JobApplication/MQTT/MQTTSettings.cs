using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplication.MQTT
{
    public class MQTTSettings
    {
        public string Broker { get; set; }
        public string ClientId { get; set; }
        public string Topic { get; set; }
    }
}
}
