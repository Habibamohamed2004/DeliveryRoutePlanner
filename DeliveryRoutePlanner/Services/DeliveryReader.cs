using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeliveryRoutePlanner.Models;

namespace DeliveryRoutePlanner.Services
{
    public class DeliveryReader
    {
        public List<Delivery> ReadFromCsv(string filePath)
        {
            var deliveries = new List<Delivery>();

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(
                    $"Input file was not found: {filePath}");
            }

            bool isFirstLine = true;
            int lineNumber = 0;

            foreach (var line in File.ReadLines(filePath))
            {
                lineNumber++;
                if (isFirstLine)
                {
                    isFirstLine = false;
                    continue;
                }

                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var columns = line.Split(',');

                if (columns.Length != 4)
                {
                    throw new FormatException(
                        $"Invalid format on line {lineNumber}. Expected 4 columns.");
                }

                if (!int.TryParse(columns[0].Trim(), out int id))
                {
                    throw new FormatException(
                        $"Invalid delivery ID on line {lineNumber}.");
                }

                string area = columns[1].Trim();

                if (string.IsNullOrWhiteSpace(area))
                {
                    throw new FormatException(
                        $"Area cannot be empty on line {lineNumber}.");
                }

                if (!int.TryParse(columns[2].Trim(), out int priority))
                {
                    throw new FormatException(
                        $"Invalid priority on line {lineNumber}.");
                }

                if (!decimal.TryParse(
                        columns[3].Trim(),
                        out decimal weight))
                {
                    throw new FormatException(
                        $"Invalid weight on line {lineNumber}.");
                }

                deliveries.Add(new Delivery
                {
                    Id = id,
                    Area = area,
                    Priority = priority,
                    Weight = weight
                });
            }

            return deliveries;
        }
    }
}
