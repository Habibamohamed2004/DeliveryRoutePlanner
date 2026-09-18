using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryRoutePlanner.Models
{
    public class PlanningResult
    {
        public List<Trip> Trips { get; set; } = new();

        public List<InvalidDelivery> InvalidDeliveries { get; set; } = new();
    }
}
