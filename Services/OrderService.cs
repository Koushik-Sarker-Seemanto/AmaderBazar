using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models.AdminModels;
using Models.Entities;
using Models.LiveAnimalModels;
using Models.OrderModels;
using Repositories;
using Services.Contracts;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;

namespace Services
{
    public class OrderService : IOrderService
    {
        private readonly IMongoRepository _repository;
        private readonly ILogger<OrderService> _logger;
        private readonly ILiveAnimalService _liveAnimalService;
        public OrderService(ILogger<OrderService> logger,IMongoRepository mongoRepository,ILiveAnimalService liveAnimalService)
        {
            _repository = mongoRepository;
            _logger = logger;
            _liveAnimalService = liveAnimalService;
        }
        public async Task<bool> AddOrder(Order order)
        {
            try
            {
                await _repository.SaveAsync<Order>(order);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"AddOrder Failed: {e.Message}");
                return false;
            }
        }

        public async Task<Order> FindOrderById(string id)
        {
            try
            {
                var order = await _repository.GetItemAsync<Order>(d=>d.Id == id);
                return order;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"GetOrder Failed: {e.Message}");
                return null;
            }
        }

        public async Task<List<OrderViewModel>> PlacedOrders()
        {
            try
            {
                var orders = await _repository.GetItemsAsync<Order>();
                var list = orders?.ToList();
                list.Reverse();
                List<OrderViewModel> orderViewModels = new List<OrderViewModel>();
                foreach (var order in orders)
                {
                    var orderView = await BuildOrderViewModel(order);
                    orderViewModels.Add(orderView);
                }

                return orderViewModels;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"GetOrderList Failed: {e.Message}");
                return null;
            }
        }

        public async Task<bool> ContactClient(string OrderId)
        {
            try
            {
                var order = await _repository.GetItemAsync<Order>(e => e.Id == OrderId);
                if (order == null)
                {
                    return false;
                }
                if (order.Contacted)
                {
                    _logger.LogInformation($"ContactClient: Already Contacted.");
                    return false;
                }
                order.Contacted = true;
                await _repository.UpdateAsync<Order>(e => e.Id == order.Id, order);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"GetAnimalDetails Failed: {e.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteOrder(string OrderId)
        {
            try
            {
                await _repository.DeleteAsync<Order>(e => e.Id == OrderId);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Delete Animal Failed: {e.Message}");
                return false;
            }
        }

        public  FileStreamResult CreateReciept(OrderViewModel model)
        {
            try
            {

                PdfDocument document = new PdfDocument();
                //Adds page settings
                document.PageSettings.Orientation = PdfPageOrientation.Landscape;
                document.PageSettings.Margins.All = 50;
                //Adds a page to the document
                PdfPage page = document.Pages.Add();
                PdfGraphics graphics = page.Graphics;
                FileStream imageStream = new FileStream("wwwroot/images/04d50343-f1c3-4198-abd6-28b0db0ed0fc.jpg", FileMode.Open, FileAccess.Read);
                RectangleF bounds = new RectangleF(176, 0, 300, 100);
                PdfImage image = PdfImage.FromStream(imageStream);
                //Draws the image to the PDF page
                page.Graphics.DrawImage(image, bounds);
                PdfBrush solidBrush = new PdfSolidBrush(new PdfColor(126, 151, 173));
                bounds = new RectangleF(0, bounds.Bottom + 90, graphics.ClientSize.Width, 30);
                //Draws a rectangle to place the heading in that region.
                graphics.DrawRectangle(solidBrush, bounds);
                //Creates a font for adding the heading in the page
                PdfFont subHeadingFont = new PdfStandardFont(PdfFontFamily.TimesRoman, 14);
                //Creates a text element to add the invoice number

                PdfTextElement element = new PdfTextElement("INVOICE " + model.Order.Id, subHeadingFont);
                element.Brush = PdfBrushes.White;

                //Draws the heading on the page
                PdfLayoutResult result = element.Draw(page, new PointF(10, bounds.Top + 8));
                string currentDate = "DATE " + DateTime.Now.ToString("MM/dd/yyyy");
                //Measures the width of the text to place it in the correct location
                SizeF textSize = subHeadingFont.MeasureString(currentDate);
                PointF textPosition = new PointF(graphics.ClientSize.Width - textSize.Width - 10, result.Bounds.Y);
                //Draws the date by using DrawString method
                graphics.DrawString(currentDate, subHeadingFont, element.Brush, textPosition);
                PdfFont timesRoman = new PdfStandardFont(PdfFontFamily.TimesRoman, 10);
                //Creates text elements to add the address and draw it to the page.
                element = new PdfTextElement("BILL TO ", timesRoman);
                element.Brush = new PdfSolidBrush(new PdfColor(126, 155, 203));
                result = element.Draw(page, new PointF(10, result.Bounds.Bottom + 15));
                // Address
                element = new PdfTextElement(model.Order.Name, timesRoman);
                element.Brush = new PdfSolidBrush(new PdfColor(Color.Black));
                result = element.Draw(page, new PointF(10, result.Bounds.Bottom + 3));
                //
                element = new PdfTextElement(model.Order.PhoneNumber, timesRoman);
                element.Brush = new PdfSolidBrush(new PdfColor(Color.Black));
                result = element.Draw(page, new PointF(10, result.Bounds.Bottom + 3));
                //
                element = new PdfTextElement(model.Order.Address, timesRoman);
                element.Brush = new PdfSolidBrush(new PdfColor(Color.Black));
                result = element.Draw(page, new PointF(10, result.Bounds.Bottom + 3));


                PdfPen linePen = new PdfPen(new PdfColor(126, 151, 173), 0.70f);
                PointF startPoint = new PointF(0, result.Bounds.Bottom + 3);
                PointF endPoint = new PointF(graphics.ClientSize.Width, result.Bounds.Bottom + 6);
                //Draws a line at the bottom of the address
                graphics.DrawLine(linePen, startPoint, endPoint);

                //Data Source
                List<object> data = new List<object>();
                Object row1 = new
                {
                    Title = model.LiveAnimal.Title,
                    Category = model.LiveAnimal.Category,
                    Origin = model.LiveAnimal.Origin,
                    Color = model.LiveAnimal.Color,
                    Location = model.LiveAnimal.Location,
                    Price = "   " + model.LiveAnimal.Price
                };

                data.Add(row1);

                //Creates a PDF grid
                PdfGrid grid = new PdfGrid();
                //Adds the data source
                grid.DataSource = data;
                //Creates the grid cell styles
                PdfGridCellStyle cellStyle = new PdfGridCellStyle();
                cellStyle.Borders.All = PdfPens.White;
                PdfGridRow header = grid.Headers[0];
                //Creates the header style
                PdfGridCellStyle headerStyle = new PdfGridCellStyle();
                headerStyle.Borders.All = new PdfPen(new PdfColor(126, 151, 173));
                headerStyle.BackgroundBrush = new PdfSolidBrush(new PdfColor(126, 151, 173));
                headerStyle.TextBrush = PdfBrushes.White;
                headerStyle.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 14f, PdfFontStyle.Regular);

                //Adds cell customizations
                for (int i = 0; i < header.Cells.Count; i++)
                {
                    if (i == 0 || i == 1)
                        header.Cells[i].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                    else
                        header.Cells[i].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                }

                //Applies the header style
                header.ApplyStyle(headerStyle);
                cellStyle.Borders.Bottom = new PdfPen(new PdfColor(217, 217, 217), 1.00f);
                cellStyle.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 13f);
                cellStyle.TextBrush = new PdfSolidBrush(new PdfColor(131, 130, 136));

                //Creates the layout format for grid
                PdfGridLayoutFormat layoutFormat = new PdfGridLayoutFormat();
                // Creates layout format settings to allow the table pagination
                layoutFormat.Layout = PdfLayoutType.Paginate;
                //Draws the grid to the PDF page.
                PdfGridLayoutResult gridResult = grid.Draw(page, new RectangleF(new PointF(0, result.Bounds.Bottom + 40), new SizeF(graphics.ClientSize.Width, graphics.ClientSize.Height - 100)), layoutFormat);
                MemoryStream stream = new MemoryStream();

                document.Save(stream);

                //Set the position as '0'.
                stream.Position = 0;

                //Download the PDF document in the browser
                FileStreamResult fileStreamResult = new FileStreamResult(stream, "application/pdf");

                fileStreamResult.FileDownloadName = "Booking Recipet.pdf";
                return fileStreamResult;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Voucher generate Failed: {e.Message}");
                return null;
            }
        }

        private async Task<OrderViewModel> BuildOrderViewModel(Order order)
        {
            var liveAnimal = await _liveAnimalService.GetLiveAnimalById(order.LiveAnimalId);
            OrderViewModel orderView = new OrderViewModel
            {
                Order = order,
                LiveAnimal = liveAnimal,

            };
            return orderView;
        }
    }
}
