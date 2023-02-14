﻿namespace TrackerLibrary.Models
{
    /// <summary>
    /// Represente what the prize is for the given place
    /// </summary>
    public class PrizeModel
    {
        /// <summary>
        /// The unique identifier for the prize
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The numeric identifier fOR the place (2 for second place, etc)
        /// </summary>
        public int PlaceNumber { get; set; }
        /// <summary>
        /// The friendly name for the place (second place, first runner up, etc.)
        /// The fixed amount this place earns 
        /// </summary>
        public string PlaceName { get; set; } 
        /// <summary>
        /// the fixed amount this place earns or zero if it is not used.
        /// </summary>
        public decimal PrizeAmount { get; set; }
        /// <summary>
        /// the number that represents the percentage of the overal take or 
        /// zero if it is not used. The percentage is a fraction of 1 (so   0.5  for 
        /// 50%).
        /// / </summary>
        public double PrizePercentage { get; set; }

        public PrizeModel()
        { 
            PlaceName= string.Empty;
        }

        public PrizeModel(string placeName, string placeNumber, string prizeAmount, string prizePercentage)
        {
            PlaceName = placeName;

            int placeNumberValue = 0;
            int.TryParse(placeNumber, out placeNumberValue);
            PlaceNumber = placeNumberValue;

            decimal prizeAmountValue = 0;
            decimal.TryParse(prizeAmount, out prizeAmountValue);
            PrizeAmount = prizeAmountValue;

            double prizePercentageValue = 0;
            double.TryParse(prizePercentage, out prizePercentageValue);
            PrizePercentage = prizePercentageValue;
        }
    }
}