using System;
using Microsoft.Extensions.Configuration;
using SSLCommerz.Contracts;

namespace SSLCommerz
{
    public class SSLCommerzConfig : ISSLCommerzConfig
    {
        public string StoreID { get; set; }

        public string StorePass { get; set; }

        public bool IsDevelopmentMode { get; set; }

        public string SSLCommerzBaseUrl { get; set; }

        public string SSLCommerzSandboxBaseUrl { get; set; }

        public string SubmitUri { get; set; }

        public string ValidationUri { get; set; }

        public string CheckingUri { get; set; }

        public string SuccessUrl { get; set; }
        public string FailUrl { get; set; }
        public string CancelUrl { get; set; }

        public SSLCommerzConfig()
        {
            
        }

        public SSLCommerzConfig(IConfiguration configuration)
        {
            this.StoreID = configuration.GetSection("SSLCommerz:StoreID").Value;
            this.StorePass = configuration.GetSection("SSLCommerz:StorePass").Value;

            var isDevelopmentMode = configuration.GetSection("SSLCommerz:IsDevelopmentMode").Value;
            if (!string.IsNullOrEmpty(isDevelopmentMode))
            {
                this.IsDevelopmentMode = isDevelopmentMode.ToLower() == "true" ? true : false;
            }
            
            this.SSLCommerzBaseUrl = configuration.GetSection("SSLCommerz:SSLCommerzBaseUrl").Value;
            this.SSLCommerzSandboxBaseUrl = configuration.GetSection("SSLCommerz:SSLCommerzSandboxBaseUrl").Value;

            this.SubmitUri = configuration.GetSection("SSLCommerz:SubmitUri").Value;
            this.ValidationUri = configuration.GetSection("SSLCommerz:ValidationUri").Value;
            this.CheckingUri = configuration.GetSection("SSLCommerz:CheckingUri").Value;

            this.SuccessUrl = configuration.GetSection("SSLCommerz:SuccessUrl").Value;
            this.FailUrl = configuration.GetSection("SSLCommerz:FailUrl").Value;
            this.CancelUrl = configuration.GetSection("SSLCommerz:CancelUrl").Value;
        }
    }
}