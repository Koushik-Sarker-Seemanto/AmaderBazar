using System.Collections.Specialized;
using SSLCommerz.Models;

namespace SSLCommerz.Contracts
{
    public interface ISSLCommerzService
    {
        SSLCommerzInitResponse InitiateTransaction(NameValueCollection postData);

        bool OrderValidate(NameValueCollection requestParams, string valId, string merchantTrxID, string merchantTrxAmount, string merchantTrxCurrency);
    }
}