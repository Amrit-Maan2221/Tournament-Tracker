using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary.Data_Access
{
    public class SqlConnector : IDataConnection
    {
        /// <summary>
        /// Saves a new prize to the database
        /// </summary>
        /// <param name="model">The prize information</param>
        /// <returns>The prize information plus the unique identifier.</returns>
        public PrizeModel CreatePrize(PrizeModel model)
        {
            using (SqlConnection connection = new SqlConnection(GlobalConfig.GetConnectionString("Tournaments")))
            {
                SqlCommand command = new SqlCommand("[dbo].[spPrizes_Insert]", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@PlaceNumber", model.PlaceNumber);
                command.Parameters.AddWithValue("@PlaceName", model.PlaceName);
                command.Parameters.AddWithValue("@PrizeAmount", model.PrizeAmount);
                command.Parameters.AddWithValue("@PrizePercentage", model.PrizePercentage);

                SqlParameter modelId = new SqlParameter("@Id", SqlDbType.Int);
                modelId.Direction = ParameterDirection.Output;
                command.Parameters.Add(modelId);
                connection.Open();
                command.ExecuteNonQuery();

                bool placeNumberValidNumber = int.TryParse(modelId.Value.ToString(), out int id);
                if (placeNumberValidNumber)
                {
                    model.Id = id;
                }
            }

            return model;
        }


        /// <summary>
        /// Saves a new person to the database
        /// </summary>
        /// <param name="model">The person information</param>
        /// <returns>The person information plus the unique identifier.</returns>
        public PersonModel CreatePerson(PersonModel model)
        {
            using (SqlConnection connection = new SqlConnection(GlobalConfig.GetConnectionString("Tournaments")))
            {
                SqlCommand command = new SqlCommand("[dbo].[spPeople_Insert]", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@FirstName", model.FirstName);
                command.Parameters.AddWithValue("@LastName", model.LastName);
                command.Parameters.AddWithValue("@EmailAddress", model.EmailAddress);
                command.Parameters.AddWithValue("@CellPhoneNumber", model.CellPhoneNumber);

                SqlParameter modelId = new SqlParameter("@Id", SqlDbType.Int);
                modelId.Direction = ParameterDirection.Output;
                command.Parameters.Add(modelId);
                connection.Open();
                command.ExecuteNonQuery();

                bool placeNumberValidNumber = int.TryParse(modelId.Value.ToString(), out int id);
                if (placeNumberValidNumber)
                {
                    model.Id = id;
                }

            }

            return model;
        }
    }

}