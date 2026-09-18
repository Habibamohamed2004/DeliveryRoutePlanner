using DeliveryRoutePlanner.Models;
using DeliveryRoutePlanner.Services;

Console.WriteLine("==========================================");
Console.WriteLine("       DELIVERY ROUTE PLANNER");
Console.WriteLine("==========================================");
Console.WriteLine();

string inputPath = Path.Combine(
    AppContext.BaseDirectory,
    "Input",
    "deliveries.csv");

try
{
    var reader = new DeliveryReader();

    List<Delivery> deliveries = reader.ReadFromCsv(inputPath);

    if (deliveries.Count == 0)
    {
        Console.WriteLine("No deliveries found.");
        return;
    }

    var planner = new RoutePlanner();

    PlanningResult result = planner.CreateTrips(deliveries);

    bool running = true;

    while (running)
    {
        Console.Clear();

        Console.WriteLine("==========================================");
        Console.WriteLine("       DELIVERY ROUTE PLANNER");
        Console.WriteLine("==========================================");
        Console.WriteLine();

        Console.WriteLine($"Current Trips: {result.Trips.Count}");
        Console.WriteLine();

        Console.WriteLine("1. Display All Trips");
        Console.WriteLine("2. Search / Filter Trips");
        Console.WriteLine("3. Delivery Analytics");
        Console.WriteLine("4. Display Summary");
        Console.WriteLine("5. Display Invalid Deliveries");
        Console.WriteLine("6. Export Planned Trips");
        Console.WriteLine("7. Exit");
        Console.WriteLine();

        Console.Write("Enter your choice: ");

        string? choice = Console.ReadLine();

        Console.Clear();

        switch (choice)
        {
            case "1":
                DisplayTrips(result.Trips);
                Pause();
                break;

            case "2":
                SearchTrips(planner, result.Trips);
                Pause();
                break;

            case "3":
                DisplayAnalytics(
                    deliveries,
                    result.Trips,
                    result.InvalidDeliveries);

                Pause();
                break;

            case "4":
                DisplaySummary(
                    deliveries,
                    result.Trips,
                    result.InvalidDeliveries);

                Pause();
                break;

            case "5":
                DisplayInvalidDeliveries(
                    result.InvalidDeliveries);

                Pause();
                break;

            case "6":
                ExportTripsToCsv(result.Trips);
                Pause();
                break;

            case "7":
                running = false;
                Console.WriteLine("Goodbye!");
                break;

            default:
                Console.WriteLine("Invalid choice.");
                Pause();
                break;
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}


// ==========================================================
// SEARCH / FILTER TRIPS
// ==========================================================

static void SearchTrips(
    RoutePlanner planner,
    List<Trip> trips)
{
    Console.WriteLine("==========================================");
    Console.WriteLine("          SEARCH / FILTER TRIPS");
    Console.WriteLine("==========================================");
    Console.WriteLine();

    Console.WriteLine("1. Search by Trip Number");
    Console.WriteLine("2. Search by Area");
    Console.WriteLine("3. Display All Trips");
    Console.WriteLine();

    Console.Write("Enter your choice: ");

    string? choice = Console.ReadLine();

    List<Trip> results;

    switch (choice)
    {
        case "1":

            Console.Write("Enter Trip Number: ");

            if (!int.TryParse(
                    Console.ReadLine(),
                    out int tripNumber))
            {
                Console.WriteLine("Invalid trip number.");
                return;
            }

            results = planner.SearchTrips(
                trips,
                tripNumber: tripNumber);

            break;

        case "2":

            Console.Write("Enter Area: ");

            string? area = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(area))
            {
                Console.WriteLine("Area cannot be empty.");
                return;
            }

            results = planner.SearchTrips(
                trips,
                area: area);

            break;

        case "3":

            results = trips;

            break;

        default:

            Console.WriteLine("Invalid choice.");
            return;
    }

    Console.WriteLine();

    if (results.Count == 0)
    {
        Console.WriteLine("No matching trips found.");
        return;
    }

    Console.WriteLine(
        $"Found {results.Count} trip(s).");

    Console.WriteLine();

    DisplayTrips(results);
}


// ==========================================================
// DELIVERY ANALYTICS
// ==========================================================

static void DisplayAnalytics(
    List<Delivery> allDeliveries,
    List<Trip> trips,
    List<InvalidDelivery> invalidDeliveries)
{
    Console.Clear();
    Console.WriteLine("==========================================");
    Console.WriteLine("          DELIVERY ANALYTICS");
    Console.WriteLine("==========================================");
    Console.WriteLine();

    if (trips.Count == 0)
    {
        Console.WriteLine("No trips available for analysis.");
        return;
    }

    // ------------------------------------------------------
    // AREA STATISTICS
    // ------------------------------------------------------

    Console.WriteLine("AREA STATISTICS");
    Console.WriteLine("------------------------------------------");

    var areaStatistics = trips
        .SelectMany(t => t.Deliveries)
        .GroupBy(d => d.Area)
        .OrderBy(g => g.Key);

    foreach (var area in areaStatistics)
    {
        int deliveryCount = area.Count();

        decimal totalWeight = area.Sum(d => d.Weight);

        Console.WriteLine(
            $"{area.Key}");

        Console.WriteLine(
            $"  Deliveries  : {deliveryCount}");

        Console.WriteLine(
            $"  Total Weight: {totalWeight:F1} kg");

        Console.WriteLine();
    }


    // ------------------------------------------------------
    // PRIORITY STATISTICS
    // ------------------------------------------------------

    Console.WriteLine("PRIORITY STATISTICS");
    Console.WriteLine("------------------------------------------");

    var priorityStatistics = trips
        .SelectMany(t => t.Deliveries)
        .GroupBy(d => d.Priority)
        .OrderBy(g => g.Key);

    foreach (var priority in priorityStatistics)
    {
        int deliveryCount = priority.Count();

        decimal totalWeight = priority.Sum(d => d.Weight);

        Console.WriteLine(
            $"Priority {priority.Key}");

        Console.WriteLine(
            $"  Deliveries  : {deliveryCount}");

        Console.WriteLine(
            $"  Total Weight: {totalWeight:F1} kg");

        Console.WriteLine();
    }


    // ------------------------------------------------------
    // PACKAGE STATISTICS
    // ------------------------------------------------------

    var validDeliveries = trips
        .SelectMany(t => t.Deliveries)
        .ToList();

    if (validDeliveries.Count > 0)
    {
        Console.WriteLine("PACKAGE STATISTICS");
        Console.WriteLine("------------------------------------------");

        decimal averageWeight =
            validDeliveries.Average(d => d.Weight);

        Delivery heaviest =
            validDeliveries
                .OrderByDescending(d => d.Weight)
                .First();

        Delivery lightest =
            validDeliveries
                .OrderBy(d => d.Weight)
                .First();

        Console.WriteLine(
            $"Average Package Weight : {averageWeight:F1} kg");

        Console.WriteLine(
            $"Heaviest Package       : {heaviest.Id} ({heaviest.Weight:F1} kg)");

        Console.WriteLine(
            $"Lightest Package       : {lightest.Id} ({lightest.Weight:F1} kg)");

        Console.WriteLine();
    }


    // ------------------------------------------------------
    // TRIP STATISTICS
    // ------------------------------------------------------

    Console.WriteLine("TRIP STATISTICS");
    Console.WriteLine("------------------------------------------");

    Trip heaviestTrip =
        trips
            .OrderByDescending(t => t.TotalWeight)
            .First();

    Trip lightestTrip =
        trips
            .OrderBy(t => t.TotalWeight)
            .First();

    decimal averageTripWeight =
        trips.Average(t => t.TotalWeight);

    decimal averageUtilization =
        trips.Average(t => t.UtilizationPercentage);

    Console.WriteLine(
        $"Heaviest Trip       : Trip {heaviestTrip.TripNumber} " +
        $"({heaviestTrip.TotalWeight:F1} kg)");

    Console.WriteLine(
        $"Lightest Trip       : Trip {lightestTrip.TripNumber} " +
        $"({lightestTrip.TotalWeight:F1} kg)");

    Console.WriteLine(
        $"Average Trip Weight : {averageTripWeight:F1} kg");

    Console.WriteLine(
        $"Average Utilization : {averageUtilization:F1}%");

    Console.WriteLine();

    // ------------------------------------------------------
    // UTILIZATION REPORT
    // ------------------------------------------------------

    Console.WriteLine("CAPACITY UTILIZATION");
    Console.WriteLine("------------------------------------------");

    foreach (var trip in trips)
    {
        Console.WriteLine(
            $"Trip {trip.TripNumber} : " +
            $"{trip.TotalWeight:F1} / 10 kg " +
            $"({trip.UtilizationPercentage:F1}%)");
    }

    Console.WriteLine();

    int highlyUtilizedTrips = trips.Count(
        t => t.UtilizationPercentage >= 80);

    int underUtilizedTrips = trips.Count(
        t => t.UtilizationPercentage < 50);

    Console.WriteLine(
        $"Highly Utilized Trips (>= 80%): {highlyUtilizedTrips}");

    Console.WriteLine(
        $"Underutilized Trips (< 50%)   : {underUtilizedTrips}");
}


// ==========================================================
// EXPORT TRIPS TO CSV
// ==========================================================

static void ExportTripsToCsv(List<Trip> trips)
{
    Console.Clear();
    Console.WriteLine("==========================================");
    Console.WriteLine("          EXPORT PLANNED TRIPS");
    Console.WriteLine("==========================================");
    Console.WriteLine();

    if (trips.Count == 0)
    {
        Console.WriteLine("There are no trips to export.");
        return;
    }

    string outputDirectory = Path.Combine(
    AppContext.BaseDirectory,
    "Output");

    Directory.CreateDirectory(outputDirectory);

    string outputPath = Path.Combine(
        outputDirectory,
        "planned_trips.csv");

    using StreamWriter writer = new StreamWriter(outputPath);

    writer.WriteLine(
        "TripNumber,DeliveryId,Area,Priority,Weight");

    foreach (var trip in trips)
    {
        foreach (var delivery in trip.Deliveries)
        {
            writer.WriteLine(
                $"{trip.TripNumber}," +
                $"{delivery.Id}," +
                $"{delivery.Area}," +
                $"{delivery.Priority}," +
                $"{delivery.Weight:F1}");
        }
    }

    Console.WriteLine(
        "Trips exported successfully.");

    Console.WriteLine();

    Console.WriteLine(
        $"File: {outputPath}");

    Console.WriteLine();

    Console.WriteLine(
        $"Exported {trips.Count} trips.");
}


// ==========================================================
// DISPLAY TRIPS
// ==========================================================

static void DisplayTrips(List<Trip> trips)
{
    Console.Clear();
    Console.WriteLine("TRIPS");
    Console.WriteLine();

    if (trips.Count == 0)
    {
        Console.WriteLine("No trips available.");
        return;
    }

    foreach (var trip in trips)
    {
        Console.WriteLine($"Trip {trip.TripNumber}");
        Console.WriteLine("------------------------------------------");

        foreach (var delivery in trip.Deliveries)
        {
            Console.WriteLine(
                $"Delivery ID : {delivery.Id}");

            Console.WriteLine(
                $"Area        : {delivery.Area}");

            Console.WriteLine(
                $"Priority    : {delivery.Priority}");

            Console.WriteLine(
                $"Weight      : {delivery.Weight:F1} kg");

            Console.WriteLine();
        }

        Console.WriteLine(
            $"Total Weight       : {trip.TotalWeight:F1} kg");

        Console.WriteLine(
            $"Remaining Capacity : {trip.RemainingCapacity:F1} kg");

        Console.WriteLine(
            $"Utilization        : {trip.UtilizationPercentage:F1}%");

        Console.WriteLine();
        Console.WriteLine("------------------------------------------");
        Console.WriteLine();
    }
}


// ==========================================================
// DISPLAY INVALID DELIVERIES
// ==========================================================

static void DisplayInvalidDeliveries(
    List<InvalidDelivery> invalidDeliveries)
{
    Console.Clear();
    if (invalidDeliveries.Count == 0)
    {
        Console.WriteLine("No invalid deliveries found.");
        return;
    }

    Console.WriteLine("INVALID DELIVERIES");
    Console.WriteLine("------------------------------------------");

    foreach (var delivery in invalidDeliveries)
    {
        Console.WriteLine(
            $"ID       : {delivery.Id}");

        Console.WriteLine(
            $"Area     : {delivery.Area}");

        Console.WriteLine(
            $"Priority : {delivery.Priority}");

        Console.WriteLine(
            $"Weight   : {delivery.Weight:F1} kg");

        Console.WriteLine(
            $"Reason   : {delivery.Reason}");

        Console.WriteLine();
    }
}


// ==========================================================
// DISPLAY SUMMARY
// ==========================================================

static void DisplaySummary(
    List<Delivery> allDeliveries,
    List<Trip> trips,
    List<InvalidDelivery> invalidDeliveries)
{
    decimal totalWeight = trips
        .SelectMany(t => t.Deliveries)
        .Sum(d => d.Weight);

    decimal averageTripWeight = trips.Count > 0
        ? totalWeight / trips.Count
        : 0;

    decimal overallUtilization = trips.Count > 0
        ? totalWeight / (trips.Count * 10m) * 100m
        : 0;
    Console.Clear();
    Console.WriteLine("==========================================");
    Console.WriteLine("                 SUMMARY");
    Console.WriteLine("==========================================");

    Console.WriteLine(
        $"Total Deliveries    : {allDeliveries.Count}");

    Console.WriteLine(
        $"Valid Deliveries    : {allDeliveries.Count - invalidDeliveries.Count}");

    Console.WriteLine(
        $"Invalid Deliveries  : {invalidDeliveries.Count}");

    Console.WriteLine(
        $"Total Trips         : {trips.Count}");

    Console.WriteLine(
        $"Total Weight        : {totalWeight:F1} kg");

    Console.WriteLine(
        $"Average Trip Weight : {averageTripWeight:F1} kg");

    Console.WriteLine(
        $"Overall Utilization : {overallUtilization:F1}%");

    Console.WriteLine("==========================================");
}


// ==========================================================
// PAUSE
// ==========================================================

static void Pause()
{
    Console.WriteLine();
    Console.WriteLine("Press Enter to continue...");
    Console.ReadLine();
}