using System.ComponentModel.DataAnnotations;


namespace Global_Logistics_Management_System.Models
{
    public class Client
    {
        [Key]
        public int ClientId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        // Navigation property
        public ICollection<Contract> Contracts { get; set; } = new List<Contract>();
    }
}
