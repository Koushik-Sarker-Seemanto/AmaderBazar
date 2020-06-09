namespace SSLCommerz.Models
{
    public class SSLCommerzValidatorResponse
    {
        public string status { get; set; }
        public string tran_date { get; set; }
        public string tran_id { get; set; }
        public string val_id { get; set; }
        public string amount { get; set; }
        public string store_amount { get; set; }
        public string currency { get; set; }
        public string bank_tran_id { get; set; }
        public string card_type { get; set; }
        public string card_no { get; set; }
        public string card_issuer { get; set; }
        public string card_brand { get; set; }
        public string card_issuer_country { get; set; }
        public string card_issuer_country_code { get; set; }
        public string currency_type { get; set; }
        public string currency_amount { get; set; }
        public string currency_rate { get; set; }
        public string base_fair { get; set; }
        public string value_a { get; set; }
        public string value_b { get; set; }
        public string value_c { get; set; }
        public string value_d { get; set; }
        public string emi_instalment { get; set; }
        public string emi_amount { get; set; }
        public string emi_description { get; set; }
        public string emi_issuer { get; set; }
        public string account_details { get; set; }
        public string risk_title { get; set; }
        public string risk_level { get; set; }
        public string APIConnect { get; set; }
        public string validated_on { get; set; }
        public string gw_version { get; set; }
        public string token_key { get; set; }
    }
}