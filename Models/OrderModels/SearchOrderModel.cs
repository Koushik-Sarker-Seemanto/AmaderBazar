using System.ComponentModel.DataAnnotations;

namespace Models.OrderModels
{
    public class SearchOrderModel
    {
        [Required]
        public string Id { get; set; }
    }
}