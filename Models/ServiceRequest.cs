using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Global_Logistics_Management_System.Models
{
    public class ServiceRequest
    {
        [Key]
        public int RequestId { get; set; }

        [Required]
        public string JobDescription { get; set; }

        public DateTime RequestDate { get; set; } = DateTime.Now;

        // Foreign Key
        public int ContractId { get; set; }
        public virtual Contract? Contract { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Cost { get; set; }
    }
}
