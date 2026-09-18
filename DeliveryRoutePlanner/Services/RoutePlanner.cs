using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeliveryRoutePlanner.Models;

namespace DeliveryRoutePlanner.Services
{
    public class RoutePlanner
    {
        private const decimal VehicleCapacity = 10m;

        public PlanningResult CreateTrips(List<Delivery> deliveries)
        {
            var result = new PlanningResult();
            var duplicateIds = deliveries
                .GroupBy(d => d.Id)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToHashSet();


            var validDeliveries = new List<Delivery>();

            foreach (var delivery in deliveries)
            {
                if (duplicateIds.Contains(delivery.Id))
                {
                    result.InvalidDeliveries.Add(new InvalidDelivery
                    {
                        Id = delivery.Id,
                        Area = delivery.Area,
                        Priority = delivery.Priority,
                        Weight = delivery.Weight,
                        Reason = "Duplicate delivery ID."
                    });

                    continue;
                }

                if (delivery.Weight <= 0)
                {
                    result.InvalidDeliveries.Add(new InvalidDelivery
                    {
                        Id = delivery.Id,
                        Area = delivery.Area,
                        Priority = delivery.Priority,
                        Weight = delivery.Weight,
                        Reason = "Weight must be greater than zero."
                    });

                    continue;
                }

                if (delivery.Weight > VehicleCapacity)
                {
                    result.InvalidDeliveries.Add(new InvalidDelivery
                    {
                        Id = delivery.Id,
                        Area = delivery.Area,
                        Priority = delivery.Priority,
                        Weight = delivery.Weight,
                        Reason = "Weight exceeds the vehicle capacity of 10 kg."
                    });

                    continue;
                }

                validDeliveries.Add(delivery);
            }

            var sortedDeliveries = validDeliveries
                .OrderBy(d => d.Priority)
                .ThenBy(d => d.Area)
                .ThenBy(d => d.Id)
                .ToList();

            foreach (var delivery in sortedDeliveries)
            {
                var trip = FindBestSameAreaTrip(
                    result.Trips,
                    delivery);

                if (trip == null)
                {
                    trip = FindBestAvailableTrip(
                        result.Trips,
                        delivery);
                }

                if (trip == null)
                {
                    trip = new Trip
                    {
                        TripNumber = result.Trips.Count + 1
                    };

                    result.Trips.Add(trip);
                }

                trip.AddDelivery(delivery);
            }

            return result;
        }

        private Trip? FindBestSameAreaTrip(
            List<Trip> trips,
            Delivery delivery)
        {
            return trips
                .Where(t =>
                    t.ContainsArea(delivery.Area) &&
                    t.CanFit(delivery))
                .OrderBy(t => t.RemainingCapacity - delivery.Weight)
                .FirstOrDefault();
        }

        private Trip? FindBestAvailableTrip(
            List<Trip> trips,
            Delivery delivery)
        {
            return trips
                .Where(t => t.CanFit(delivery))
                .OrderBy(t => t.RemainingCapacity - delivery.Weight)
                .FirstOrDefault();
        }

        public List<Trip> SearchTrips(
            List<Trip> trips,
            string? area = null,
            int? tripNumber = null)
        {
            IEnumerable<Trip> query = trips;

            if (tripNumber.HasValue)
            {
                query = query.Where(t =>
                    t.TripNumber == tripNumber.Value);
            }

            if (!string.IsNullOrWhiteSpace(area))
            {
                query = query.Where(t =>
                    t.Deliveries.Any(d =>
                        d.Area.Equals(
                            area,
                            StringComparison.OrdinalIgnoreCase)));
            }

            return query
                .OrderBy(t => t.TripNumber)
                .ToList();
        }


    }
}
