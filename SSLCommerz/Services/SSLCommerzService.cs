﻿using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
 using SSLCommerz.Contracts;
 using SSLCommerz.Models;

 namespace SSLCommerz.Services
{
    public class SSLCommerzService: ISSLCommerzService
    {
        private readonly ILogger<SSLCommerzService> logger;
        private readonly ISSLCommerzConfig config;
        private readonly string SSLCommerzBaseUrl;

        public SSLCommerzService(ISSLCommerzConfig config, ILogger<SSLCommerzService> logger)
        {
            this.logger = logger;
            this.config = config;

            this.SSLCommerzBaseUrl = config.IsDevelopmentMode ? config.SSLCommerzSandboxBaseUrl : config.SSLCommerzBaseUrl;
        }

        public SSLCommerzInitResponse InitiateTransaction(NameValueCollection postData)
        {
            if(postData == null)
            {
                return null;
            }

            postData.Add("store_id", config.StoreID);
            postData.Add("store_passwd", config.StorePass);

            try
            {
                //string response = this.GetPostResponse(config.SubmitUri, postData);

                byte[] response = null;
                string requestUrl = $"{SSLCommerzBaseUrl}{config.SubmitUri}";
                logger.LogInformation("Requestttttttttttt: "+requestUrl);

                using (WebClient client = new WebClient())
                {
                    response = client.UploadValues(requestUrl, postData);
                }

                var result = System.Text.Encoding.UTF8.GetString(response);
                SSLCommerzInitResponse resp = JsonConvert.DeserializeObject<SSLCommerzInitResponse>(result);

                if (resp.status != "SUCCESS")
                {
                    logger.LogError("Unable to get data from SSLCommerz. Please contact your manager!");
                }
                
                return resp;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"InitiateTransaction got exception {ex.Message}");
            }

            return null;
        }

        public bool OrderValidate(NameValueCollection requestParams, string valId, string merchantTrxID, string merchantTrxAmount, string merchantTrxCurrency)
        {
            SslCommerzUtility utility = new SslCommerzUtility();

            bool hashVerified = utility.IPNHashVerify(requestParams, config.StorePass);

            if (hashVerified)
            {
                string encodedValID = System.Web.HttpUtility.UrlEncode(valId);
                string encodedStoreID = System.Web.HttpUtility.UrlEncode(config.StoreID);
                string encodedStorePassword = System.Web.HttpUtility.UrlEncode(config.StorePass);

                string validateUrl = SSLCommerzBaseUrl + config.ValidationUri + "?val_id=" + encodedValID;
                validateUrl += "&store_id=" + encodedStoreID + "&store_passwd=" + encodedStorePassword;
                validateUrl += "&v=1&format=json";

                string json = string.Empty;
                HttpWebRequest request = (HttpWebRequest) WebRequest.Create(validateUrl);
                HttpWebResponse response = (HttpWebResponse) request.GetResponse();

                Stream resStream = response.GetResponseStream();                
                using (StreamReader reader = new StreamReader(resStream))
                {
                    json = reader.ReadToEnd();
                }
                return ReadSSLCommerzValidatorResponse(json, merchantTrxID, merchantTrxAmount, merchantTrxCurrency);
            }
            else
            {
                string error = "Unable to verify hash";
                logger.LogError(error);
                return false;
            }
        }

        private bool ReadSSLCommerzValidatorResponse(string json, string merchantTrxID, string merchantTrxAmount, string merchantTrxCurrency)
        {
            try
            {
                if (json != "")
                {
                    SSLCommerzValidatorResponse resp = JsonConvert.DeserializeObject<SSLCommerzValidatorResponse>(json);

                    if (resp != null && (resp.status == "VALID" || resp.status == "VALIDATED"))
                    {
                        if (merchantTrxCurrency == "BDT")
                        {
                            if (merchantTrxID == resp.tran_id && (Math.Abs(Convert.ToDecimal(merchantTrxAmount) - Convert.ToDecimal(resp.amount)) < 1) && merchantTrxCurrency == "BDT")
                            {
                                return true;
                            }
                            else
                            {
                                string error = "Amount not matching";
                                logger.LogError(error);
                                return false;
                            }
                        }
                        else
                        {
                            if (merchantTrxID == resp.tran_id && (Math.Abs(Convert.ToDecimal(merchantTrxAmount) - Convert.ToDecimal(resp.currency_amount)) < 1) && merchantTrxCurrency == resp.currency_type
                            )
                            {
                                return true;
                            }
                            else
                            {
                                string error = "Currency Amount not matching";
                                logger.LogError(error);
                                return false;
                            }

                        }
                    }
                    else
                    {
                        string error = "This transaction is either expired or fails";
                        logger.LogError(error);
                        return false;
                    }
                }
                else
                {
                    string error = "Unable to get Transaction JSON status";
                    logger.LogError(error);
                    return false;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"ReadSSLCommerzValidatorResponse {ex.Message}");
            }
            
            return false;
        }

        //private string GetPostResponse(string uri, NameValueCollection postData)
        //{
        //    try
        //    {
        //        byte[] response = null;

        //        string requestUrl = string.Empty;

        //        if (config.IsDevelopmentMode)
        //        {
        //            requestUrl = $"{config.SSLCommerzSandboxBaseUrl}{uri}";
        //        }
        //        else
        //        {
        //            requestUrl = $"{config.SSLCommerzBaseUrl}{uri}";
        //        }

        //        using (WebClient client = new WebClient())
        //        {
        //            response = client.UploadValues(requestUrl, postData);
        //        }

        //        return System.Text.Encoding.UTF8.GetString(response);
        //    }
        //    catch(Exception ex)
        //    {
        //        logger.LogError(ex, $"{uri} got exception {ex.Message}");
        //    }

        //    return null;
        //}

    }
}