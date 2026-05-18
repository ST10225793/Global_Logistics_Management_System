using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Global_Logistics_Management_System.Models
{
    public enum ContractStatus { Draft, Active, Expired, OnHold }
    public class Contract
    {
        [Key]
        public int ContractId { get; set; }

        [Required]
        [Display(Name = "Contract Name")] 
        public string ContractName { get; set; }

        [Required]
        public string Description { get; set; }

        public ContractStatus Status { get; set; } = ContractStatus.Draft;

        [Display(Name = "Service Level")]
        public string ServiceLevel { get; set; } // e.g., Gold, Silver, Bronze

        [Required]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal CostUSD { get; set; }

        [Display(Name = "Signed Contract")] 
        public string? SignedAgreementPath { get; set; }
        // Calculated property for status
        public bool IsActive => DateTime.Now >= StartDate && DateTime.Now <= EndDate;

        // Foreign Key
        public int ClientId { get; set; }
        public virtual Client? Client { get; set; }

        public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();

    }
}
