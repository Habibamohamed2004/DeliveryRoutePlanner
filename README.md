Delivery Route Planner

A simple C# console application that organizes delivery requests into vehicle trips while respecting a maximum vehicle capacity of 10 kg per trip.

Features:
- Read delivery requests from a CSV file.
- Validate delivery data.
- Reject duplicate delivery IDs.
- Reject deliveries with zero or negative weight.
- Reject deliveries heavier than the vehicle's 10 kg capacity.
- Prioritize deliveries using priority numbers, where lower numbers are more urgent.
- Group deliveries from the same area where reasonably possible.
- Ensure that no trip exceeds 10 kg.
- Display all planned trips.
- Search and filter trips by trip number or area.
- Display delivery and trip statistics.
- Export planned trips to a CSV file.
- Display invalid deliveries and the reason they were rejected.

Input Format:
The program uses CSV as the input format.
The first line must contain the column headers:
Id,Area,Priority,Weight

Example:

Id,Area,Priority,Weight
1,Nasr City,2,4.5
2,Maadi,1,2.0
3,Nasr City,3,1.2
4,Zamalek,1,7.0
5,Maadi,2,3.5

Columns:
Id - Unique delivery ID.
Area - Delivery destination area.
Priority - Lower numbers represent higher priority.
Weight - Package weight in kilograms.

The sample input file is included in the Input folder.

How to Run
Requirements
.NET SDK installed.
Visual Studio, Visual Studio Code, or another C# IDE.
Steps
Clone or download the repository.
Open the project in Visual Studio or your preferred IDE.
Make sure the sample file exists at:
Input/deliveries.csv
Build the project.
Run the application.

The program will read the CSV file automatically and create the delivery trips.

Solution Approach:
I divided the solution into three main parts:

1. Reading the input

DeliveryReader reads the CSV file and converts each row into a Delivery object.
The input is validated while it is being read. Invalid file formats or invalid values generate clear error messages.

2. Validating deliveries

Before creating trips, the program checks each delivery.

A delivery is considered invalid when:
Its ID is duplicated.
Its weight is zero or negative.
Its weight is greater than 10 kg.

Invalid deliveries are not added to any trip. Instead, they are stored in InvalidDeliveries together with the reason they were rejected.
This makes it clear which deliveries were not planned and why.

3. Creating trips

Valid deliveries are first sorted by:

Priority
Area
Delivery ID

Since lower priority numbers represent more urgent deliveries, sorting by priority ensures that urgent deliveries are handled first.
For each delivery, the program first tries to find an existing trip that:
Already contains deliveries from the same area.
Has enough remaining capacity.

If no suitable same-area trip exists, the program looks for any existing trip where the delivery fits.
Among the possible trips, the program chooses the trip that will have the smallest remaining capacity after adding the delivery.
If no existing trip can fit the delivery, a new trip is created.

This approach tries to balance the two main requirements:
Handle higher-priority deliveries first.
Group deliveries from the same area where reasonably possible.

Handling Edge Cases:

No deliveries
If the input file contains no deliveries, the program displays: No deliveries found.
and stops without creating any trips.
Package heavier than 10 kg
A package heavier than the vehicle capacity cannot be placed into any trip.

It is marked as invalid with the reason: Weight exceeds the vehicle capacity of 10 kg.
Multiple deliveries with the same priority

When deliveries have the same priority, the program uses the area as the next sorting criterion and then the delivery ID.
This provides deterministic behavior instead of relying on the input order.
Adding a package would exceed capacity
The program checks the trip capacity before adding every delivery.

A delivery is only added when:
Current Trip Weight + Delivery Weight <= 10 kg

Therefore, no trip can exceed the 10 kg vehicle capacity.

Extensions:

1. Delivery Statistics & Analytics

I added a statistics section to provide more information about the planned deliveries.

It displays:

Statistics by area.
Statistics by priority.
Average package weight.
Heaviest package.
Lightest package.
Heaviest trip.
Lightest trip.
Average trip weight.
Average capacity utilization.
Capacity utilization for each trip.
Number of highly utilized trips.
Number of underutilized trips.

I chose this feature because it gives a quick overview of how efficiently the delivery requests are being organized.

2. Search / Filter Trips
The program allows the user to search for planned trips by:
Trip number.
Delivery area.
All trips.

For example, the user can search for Maadi and see only trips that contain deliveries going to Maadi.

I chose this feature because it makes the program easier to use when the number of trips becomes larger.

3. Export Planned Routes to CSV
The program can export the generated trips to:
Output/planned_trips.csv

The exported file contains:
TripNumber,DeliveryId,Area,Priority,Weight

This allows the planned routes to be reused outside the application, for example in Excel or another system.

Limitations of the Algorithm
The algorithm is a greedy approach, so it does not guarantee the mathematically optimal grouping of deliveries.

For example, choosing a trip for an earlier delivery may prevent later deliveries from being grouped together in the most efficient way.

The algorithm prioritizes:
1.Delivery priority.
2.Same-area grouping.
3.Efficient use of remaining capacity.

This is a reasonable trade-off for this assignment because the goal is to create a simple, understandable solution rather than implement an optimization algorithm.

Reasoning Questions

1. Explain your solution approach in your own words.

I first read the deliveries from a CSV file and validate them.
Invalid deliveries are separated from valid deliveries so that they cannot accidentally be included in a trip.
For valid deliveries, I sort them by priority so that more urgent deliveries are processed first.
For each delivery, I first try to place it into a trip that already contains deliveries from the same area. If that is not possible, I try any trip with enough remaining capacity.
When multiple trips are possible, I choose the one that will have the smallest remaining capacity after adding the delivery. This helps reduce unused vehicle capacity.
If no existing trip can fit the delivery, I create a new trip.

2. What was the most difficult part of the assignment?

The most difficult part was deciding how to balance priority, area grouping, and vehicle capacity.
These requirements can conflict with each other.
For example, a delivery may have a high priority but there may not be enough space in a trip that already contains deliveries from the same area.
I decided to process deliveries by priority first, then prefer same-area trips, while always making sure the 10 kg capacity is respected.

3. Are there situations where your algorithm may not produce the best possible grouping?

Yes.
The algorithm is greedy, so it makes the best decision based on the trips currently available.
A decision that looks good for one delivery may make it harder to create a better combination for later deliveries.
For example, choosing to place a delivery into an existing trip because it fits may leave a small amount of unused capacity that cannot be used effectively later.
Finding the mathematically optimal grouping would require a more advanced optimization approach.

For this assignment, I preferred a simpler algorithm that is predictable, readable, and easy to maintain.

4. If the input contained 1,000,000 delivery requests, what part of your solution might become slow or memory-intensive?

The file-reading part is now more memory-efficient because I use File.ReadLines(), which processes the file one line at a time instead of loading the entire file into memory.
However, the program still stores all valid deliveries in a List<Delivery>, so 1,000,000 delivery objects would require a significant amount of memory.
The route-planning algorithm could also become slower with a very large number of deliveries. For each delivery, it may search through the existing trips to find a suitable trip, especially when there are many trips.
Sorting all deliveries by priority, area, and ID would also require additional processing time.

5. What would you improve if you had another day to work on the solution?

I would improve the input processing first by reading the CSV file as a stream instead of loading the entire file into memory.
I would also improve the trip-selection algorithm so that searching for suitable trips becomes more efficient for large datasets.
Other possible improvements would include:

More robust CSV parsing for areas containing commas.
Better validation of priority values.
More automated tests for edge cases.
A more detailed export format.
Allowing the user to specify the input file.
Adding configuration for vehicle capacity instead of hardcoding 10 kg.
Improving the console interface.
Complexity

Let n be the number of valid deliveries and t be the number of trips.
Sorting the deliveries takes approximately:
O(n log n)
For each delivery, the program may search through the existing trips to find a suitable trip.
Therefore, the trip-planning part can approach:
O(n × t)
in the worst case.
Memory usage is mainly affected by the number of deliveries and trips stored in memory.
Example Result

For the sample input, the program creates trips while keeping every trip at or below 10 kg.

The user can then:

1. Display All Trips
2. Search / Filter Trips
3. Delivery Analytics
4. Display Summary
5. Display Invalid Deliveries
6. Export Planned Trips
7. Exit

Design Decision
I intentionally kept the application as a simple C# console application without using a complicated framework or architecture.

The goal was to keep the code readable and easy to explain while still separating responsibilities between:

Models
Input reading
Route planning
Console interaction

This makes the solution easier to understand, test, and modify.
