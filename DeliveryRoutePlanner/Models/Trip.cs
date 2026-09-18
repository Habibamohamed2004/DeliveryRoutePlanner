using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryRoutePlanner.Models
{
    public class Trip
    {
        public int TripNumber { get; set; }

        public List<Delivery> Deliveries { get; set; } = new();

        public decimal TotalWeight =>
            Deliveries.Sum(d => d.Weight);

        public decimal RemainingCapacity =>
            10m - TotalWeight;

        public decimal UtilizationPercentage =>
            TotalWeight / 10m * 100m;

        public bool ContainsArea(string area)
        {
            return Deliveries.Any(d =>
                d.Area.Equals(area, StringComparison.OrdinalIgnoreCase));
        }

        public bool CanFit(Delivery delivery)
        {
            return TotalWeight + delivery.Weight <= 10m;
        }

        public void AddDelivery(Delivery delivery)
        {
            Deliveries.Add(delivery);
        }
    }
}
