using System.Collections.Generic;

namespace SSLCommerz.Models
{
    public class SSLCommerzInitResponse
    {
        public string status { get; set; }
        public string failedreason { get; set; }
        public string sessionkey { get; set; }
        public SSLCommerzGw gw { get; set; }
        public string redirectGatewayURL { get; set; }
        public string redirectGatewayURLFailed { get; set; }
        public string GatewayPageURL { get; set; }
        public string storeBanner { get; set; }
        public string storeLogo { get; set; }
        public List<SSLCommerzDesc> desc { get; set; }
        public string is_direct_pay_enable { get; set; }
    }
}