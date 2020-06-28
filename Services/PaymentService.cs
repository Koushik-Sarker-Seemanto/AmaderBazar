using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models.Entities;
using Models.OrderModels;
using Models.Validation_and_Enums;
using Newtonsoft.Json;
using PdfSharpCore;
using PdfSharpCore.Pdf;
using Repositories;
using Services.Contracts;
using SSLCommerz.Contracts;
using TheArtOfDev.HtmlRenderer.PdfSharp;

namespace Services
{
    public class PaymentService : IPaymentService
    {
        private IMongoRepository _repository;
        private ILogger<PaymentService> _logger;
        private ISSLCommerzService _sslCommerzService;
        private readonly ITransactionService _transactionService;
        private readonly IAdminPanelServices _adminPanelServices;

        public PaymentService(IMongoRepository repository, IAdminPanelServices adminPanelService,
            ITransactionService transactionService, ILogger<PaymentService> logger,
            ISSLCommerzService sslCommerzService)
        {
            _repository = repository;
            _logger = logger;
            _sslCommerzService = sslCommerzService;
            _adminPanelServices = adminPanelService;
            _transactionService = transactionService;
        }

        public async Task<bool> ValidatePaymentRequest(IFormCollection request)
        {
            _logger.LogInformation($"Request.Form: {JsonConvert.SerializeObject(request)}");
            string trxId = ""+request["tran_id"];
            string valId = "" + request["val_id"];
            string _amount = "" + request["amount"].ToString();
            double Amount = ConvertToDouble(_amount);
            string orderId = "" + request["value_a"];
            
            string  Name = "" + request["cus_name"];

            // AMOUNT and Currency FROM DB FOR THIS TRANSACTION
            string amount = "";
            if (!string.IsNullOrEmpty(trxId))
            {
                if (!string.IsNullOrEmpty(orderId))
                {
                    var orderData = await _repository.GetItemAsync<Order>(e => e.Id == orderId);
                    if (orderData == null)
                    {
                        await _transactionService.AddTransection(trxId, Name, Amount, StatusEnum.Failure, null, null);
                        _logger.LogInformation("OderData is null for this Trxid");
                        return false;
                    }

                    var animal = await _repository.GetItemAsync<LiveAnimal>(e => e.Id == orderData.LiveAnimalId);
                    if (animal == null)
                    {
                        await _transactionService.AddTransection(trxId, Name, Amount, StatusEnum.Failure, null, orderData);
                        _logger.LogInformation("Live Animal is null for this Trxid");
                        return false;
                    }

                    amount = animal.Price.ToString();

                    NameValueCollection collection = new NameValueCollection();
                    foreach (var pair in request)
                    {
                        collection.Add(pair.Key, pair.Value);
                    }


                    _logger.LogInformation($"NameValueCollection: {JsonConvert.SerializeObject(collection)}");

                    var validate = _sslCommerzService.OrderValidate(collection, valId, trxId, amount, "BDT");
                    if (validate)
                    {
                        var res = await _adminPanelServices.SellAnimal(animal.Id);
                        if (res)
                        {

                            var result = await _transactionService.AddTransection(trxId, Name, Amount, StatusEnum.Success, animal, orderData);
                            return result;
                        }
                        var resultF = await _transactionService.AddTransection(trxId, Name, Amount, StatusEnum.Success, animal, orderData);
                        return res;
                    }
                    else
                    {
                        _logger.LogInformation($"validate = _sslCommerzService.OrderValidate: false");
                        await _transactionService.AddTransection(trxId, Name, Amount, StatusEnum.Failure, animal, orderData);
                        return false;
                    }
                }
            }
            
            return false;
        }
        private double ConvertToDouble(string s)
        {
            char systemSeparator = Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator[0];
            double result = 0;
            try
            {
                if (s != null)
                    if (!s.Contains(","))
                        result = double.Parse(s, CultureInfo.InvariantCulture);
                    else
                        result = Convert.ToDouble(s.Replace(".", systemSeparator.ToString()).Replace(",", systemSeparator.ToString()));
            }
            catch (Exception e)
            {
                try
                {
                    result = Convert.ToDouble(s);
                }
                catch
                {
                    try
                    {
                        result = Convert.ToDouble(s.Replace(",", ";").Replace(".", ",").Replace(";", "."));
                    }
                    catch
                    {
                        throw new Exception("Wrong string-to-double format");
                    }
                }
            }
            return result;
        }

        public FileStreamResult CreateReciept(OrderViewModel model,Transaction transaction)
        {
            try
            {
                var customerName = model.Order?.Name;
                var customerPhone = model.Order?.PhoneNumber;
                var customerAddress = model.Order?.Address;
                var orderId = model.Order.Id;
                var deliveryDAte = model.Order?.DeliveryDate.ToString("yyyy MMMM dd");
                string html =
                    @"<!DOCTYPE html>
<html lang='en'>

<head>
    <meta charset='utf-8'>
    <title>Invoice</title>
    <style>
        .clearfix:after {
            content: '';
            display: table;
            clear: both;
        }

        a {
            color: #5D6975;
            text-decoration: underline;
        }

        body {
            position: relative;
            margin: 0 auto;
            color: #001028;
            background: #FFFFFF;
            font-size: 12px;
        }

        header {
            padding: 10px 0;
        }
        h1 {
            border-top: 1px solid #73c067;;
            border-bottom: 1px solid #73c067;;
            color: #73c067;
            font-size: 2.4em;
            line-height: 1.4em;
            font-weight: normal;
            text-align: center;
            margin: 0 0 20px 0;
            background: url(dimension.png);
        }
        #project {
            float: left;
        }
        #project span {
            color: #5D6975;
            text-align: right;
            width: 52px;
            margin-right: 10px;
            display: inline-block;
            font-size: 1em;
        }
        #project div{
            white-space: nowrap;
        }
        
        table, td, th {  
            border: 1px solid #ddd;
            text-align: center;
        }
        td, th {
            padding: 2px;
        }
        tr:nth-child(even) {background-color: #a3d39c;}
        table {
            border-collapse: collapse;
            width: 100%;
        }
    </style>
</head>

<body>
    <header class='clearfix'>
        <h1 style='margin-top: 10px;'>PAYMENT SLIP </h1>
        <h3 style='color: #73c067'>Order Id: <a style='cursor: none;color:red;text-decoration: none;'>" + transaction.TranxId + @"</a></h3>
        <div id='project'>
            <div><span>Customer</span> " + customerName + @"</div>
            <div><span>Phone No</span> " + customerPhone + @"</div>
            <div><span>Address</span> " + customerAddress + @"</div>
            
            <div><span>Delivery DATE</span> " + deliveryDAte + @" </div>
        </div>
    </header>
    <div>

        
        <h3 style='color: #73c067;'>Product Details</h3>
        <table style='margin: auto; width: 60%;'>
            <tbody>
                <tr>
                    <td>Title</td>
                    <td>" + model.LiveAnimal.Title + @"</td>
                </tr>
                <tr>
                    <td>Category</td>
                    <td>" + model.LiveAnimal.Category + @"</td>
                </tr>
                <tr>
                    <td>Height</td>
                    <td>" + model.LiveAnimal.Height + @"</td>
                </tr>
                <tr>
                    <td>Weight</td>
                    <td>" + model.LiveAnimal.Weight + @"</td>
                </tr>
                <tr>
                    <td>Teeth</td>
                    <td>" + model.LiveAnimal.Teeth + @"</td>
                </tr>
                <tr>
                    <td>Origin</td>
                    <td>" + model.LiveAnimal.Origin + @"</td>
                </tr>
                <tr>
                    <td>Location</td>
                    <td>" + model.LiveAnimal.Location + @"</td>
                </tr>
                <tr>
                    <td>Color</td>
                    <td>" + model.LiveAnimal.Color + @"</td>
                </tr>
            </tbody>
        </table>
        <h3 style='color: #73c067'>Payment</h3>

        <table>
            <thead>
                <tr>
                    <th class='service'>Title</th>
                    <th>PRICE</th>
                    <th>QUANTITY</th>
                    <th>TOTAL (PAID)</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td class='service'>" + model.LiveAnimal.Title + @"</td>
                    <td class='unit'>" + model.LiveAnimal.Price + @"</td>
                    <td class='qty'>1</td>
                    <td class='total'>" + model.LiveAnimal.Price + @"</td>
                </tr>
                
                </tr>
                <tr>
                    <td style='text-align: right;' colspan='3'>SUBTOTAL</td>
                    <td class='total'>" + model.LiveAnimal.Price + @"</td>
                </tr>
                <tr>
                    <td style='text-align: right;' colspan='3'>TAX 0%</td>
                    <td class='total'>0</td>
                </tr>
                <tr>
                    <td style='text-align: right;' colspan='3' class='grand total'>GRAND TOTAL (PAID) </td>
                    <td class='grand total'>" + model.LiveAnimal.Price + @"</td>
                </tr>
            </tbody>
        </table>
        <div>
            <h3 style='color: red'>Notice:</h3>
            <ul>
                <li>Thank you For Payment.</li>
                <li>Keep the invoice safe Untill Product Delivery.</li>
                <li>FOR ANY QUERY GO TO OUR WEBSITE CONTACT US SECTION OR <a href='https://www.farmhut.com.bd/home/AboutUS" + orderId + @"'> Click Here</a></li>
            </ul>
        </div>
    </div>
</body>

</html>";
                MemoryStream stream = new MemoryStream();
                PdfDocument pdf = PdfGenerator.GeneratePdf(html, PageSize.A5);
                pdf.Save(stream);
                FileStreamResult fileStreamResult = new FileStreamResult(stream, "application/pdf");

                fileStreamResult.FileDownloadName = "Booking Recipet.pdf";
                return fileStreamResult;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Voucher generate Failed: {e.Message}");
                return null;
            }

            /*PdfDocument pdf = PdfGenerator.GeneratePdf("<p><h1>Hello World</h1>This is html rendered text</p>", PageSize.A4);
            pdf.Save("document.pdf");*/
        }


    }
}