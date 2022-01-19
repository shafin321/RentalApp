using System;

namespace TruckRentalApp
{
    class Program
    {
        static void Main(string[] args)
        {   //(year, month, day, hour, mint, second)
            DateTime start = new DateTime(2022, 1, 5, 12, 0, 0);
            DateTime end = new DateTime(2022, 1, 6, 11, 0, 0);

            Console.WriteLine(CalculateAmountOwed(start,end));
        }  

        public static class Pricing
        {
            // Simulates going out to the database, loading in these values, and then using
            // them for the lifespan of this application instance.
            public static int discountedStartMinutes = 20;
            public static decimal discountedStartPrice = 5M;
            public static decimal hourlyRate = 25M;
            public static decimal dailyWeekdayRate = 400M;
            public static decimal dailyWeekendRate = 200M;
        }

        private static decimal CalculateAmountOwed(DateTime startDate, DateTime endDate)
        {
            decimal output = 0;
            TimeSpan rentalTime = endDate.Subtract(startDate);

            if (rentalTime.TotalMinutes <= Pricing.discountedStartMinutes)
            {
                // Just the first cheaper period
                output = Pricing.discountedStartPrice;
            }
            else
            {
                // Hourly rate
                decimal calculatedTotalCostByHourly = Pricing.discountedStartPrice +
                    ((int)Math.Ceiling((rentalTime.TotalMinutes - Pricing.discountedStartMinutes) / 60.0) * Pricing.hourlyRate);

                if (calculatedTotalCostByHourly <= IdentifyDailyRate(startDate))
                {
                    output = calculatedTotalCostByHourly;
                }
                else
                {
                    // identify the number of days
                    // noon is the cutoff (assess extra day at or after the 12th hour)
                    TimeSpan rentalDays = endDate.Date.Subtract(startDate.Date);
                    int totalRentalDays = rentalDays.Days;

                    if (endDate.Hour >= 12)
                    {
                        totalRentalDays += 1;
                    }

                    decimal calculatedDailyPrice = 0;

                    for (int i = 0; i < totalRentalDays; i++)
                    {
                        calculatedDailyPrice += IdentifyDailyRate(startDate.AddDays(i));
                    }

                    output = calculatedDailyPrice;
                }
            }

            return output;
        }

        private static decimal IdentifyDailyRate(DateTime day)
        {
            decimal output = Pricing.dailyWeekdayRate;
            if(day.DayOfWeek==DayOfWeek.Saturday || day.DayOfWeek==DayOfWeek.Sunday)
            {
                output = Pricing.dailyWeekendRate;
            }

            return output;
            
        }
    }
}
