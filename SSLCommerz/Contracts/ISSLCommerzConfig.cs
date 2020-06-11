namespace SSLCommerz.Contracts
{
    public interface ISSLCommerzConfig
    {
        string StoreID { get;}
        string StorePass { get;}
        bool IsDevelopmentMode { get;}

        string SSLCommerzBaseUrl { get;}
        string SSLCommerzSandboxBaseUrl { get;}
        string SubmitUri { get;}
        string ValidationUri { get;}
        string CheckingUri { get;}

        string SuccessUrl { get; }
        string FailUrl { get; }
        string CancelUrl { get; }
    }
}