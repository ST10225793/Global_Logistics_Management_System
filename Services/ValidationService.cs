using Microsoft.EntityFrameworkCore;
using Global_Logistics_Management_System.Models;

namespace Global_Logistics_Management_System.Services
{
    public class ValidationService
    {
        public bool CanCreateRequest(Contract contract)
        {
            // Business Rule: Contract must be active
            return contract != null && contract.IsActive;
        }
    }
}
