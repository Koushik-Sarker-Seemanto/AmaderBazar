using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
namespace SSLCommerz.Services
{
    internal class SslCommerzUtility
    {
        private readonly string VerifyKeyParamName = "verify_key";
        private readonly string VerifySignParamName = "verify_sign";
        /// <summary>
        /// SSLCommerz IPN Hash Verify method
        /// </summary>
        /// <param name="requestParams"></param>
        /// <param name="storePassword"></param>
        /// <returns>Boolean - True or False</returns>
        public bool IPNHashVerify(NameValueCollection requestParams, string storePassword)
        {
            List<string> keyList = new List<string>();
            string verifyKey = string.Empty;
            string verifySign = string.Empty;

            if(requestParams != null && requestParams.Get(VerifyKeyParamName) != null)
            {
                verifyKey = requestParams[VerifyKeyParamName];
            }

            if (requestParams != null && requestParams.Get(VerifySignParamName) != null)
            {
                verifySign = requestParams[VerifySignParamName];
            }

            // Check For verify_sign and verify_key parameters
            if (!string.IsNullOrEmpty(verifyKey) && !string.IsNullOrEmpty(verifySign))
            {
                // Split key string by comma to make a list array
                keyList = verifyKey.Split(',').ToList<string>();

                // Initiate a key value pair list array
                List<KeyValuePair<string, string>> dataArray = new List<KeyValuePair<string, string>>();

                // Store key and value of post in a list
                foreach (string key in keyList)
                {
                    if (requestParams.Get(key) != null)
                    {
                        string value = requestParams[key];

                        dataArray.Add(new KeyValuePair<string, string>(key, value));
                    }
                    else
                    {
                        dataArray.Add(new KeyValuePair<string, string>(key, null));
                    }
                }

                // Store Hashed Password in list
                if (!string.IsNullOrEmpty(storePassword))
                {
                    string hashedPass = this.MD5(storePassword);
                    dataArray.Add(new KeyValuePair<string, string>("store_passwd", hashedPass));

                    // Sort Array
                    dataArray.Sort(
                        delegate (KeyValuePair<string, string> pairOne,
                        KeyValuePair<string, string> pairTwo)
                        {
                            return pairOne.Key.CompareTo(pairTwo.Key);
                        }
                    );

                    // Concat and make String from array
                    string hashString = "";
                    foreach (var kv in dataArray)
                    {
                        hashString += kv.Key + "=" + kv.Value + "&";
                    }
                    // Trim & from end of this string
                    hashString = hashString.TrimEnd('&');

                    // Make hash by hash_string and store
                    string generatedHash = this.MD5(hashString);

                    // Check if generated hash and verify_sign match or not
                    if (generatedHash.Equals(verifySign))
                    {
                        return true; // Matched
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Make PHP like MD5 Hashing
        /// </summary>
        /// <param name="hashValue"></param>
        /// <returns>md5 Hashed String</returns>
        public string MD5(string hashValue)
        {
            byte[] asciiBytes = ASCIIEncoding.ASCII.GetBytes(hashValue);
            byte[] hashedBytes = System.Security.Cryptography.MD5CryptoServiceProvider.Create().ComputeHash(asciiBytes);
            string hashedString = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            return hashedString;
        }
    }
}