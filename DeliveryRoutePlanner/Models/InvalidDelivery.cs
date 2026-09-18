using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryRoutePlanner.Models
{
    public class InvalidDelivery
    {
        public int Id { get; set; }

        public string Area { get; set; } = string.Empty;

        public int Priority { get; set; }

        public decimal Weight { get; set; }

        public string Reason { get; set; } = string.Empty;
    }
}
