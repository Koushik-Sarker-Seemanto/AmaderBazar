using System.ComponentModel.DataAnnotations;

namespace Models.AdminModels
{
    public class RegisterViewModel
    {
        [Required]
        public string AuthorSecret { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}