using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary.Data_Access
{
    public class TextConnector : IDataConnection
    {
        /// <summary>
        /// Saves a new prize to a text file
        /// </summary>
        /// <param name="model">The prize information</param>
        /// <returns>The prize information plus the unique identifier.</returns>
        public PrizeModel CreatePrize(PrizeModel model)
        {
            model.Id = 1;

            return model;
        }
    }
}
