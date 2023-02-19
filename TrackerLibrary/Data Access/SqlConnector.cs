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
        private const string DATABASE_NAME = "Tournaments";


        /// <summary>
        /// Saves a new prize to the database
        /// </summary>
        /// <param name="model">The prize information</param>
        /// <returns>The prize information plus the unique identifier.</returns>
        public PrizeModel CreatePrize(PrizeModel model)
        {
            using (SqlConnection connection = new SqlConnection(GlobalConfig.GetConnectionString(DATABASE_NAME)))
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
            using (SqlConnection connection = new SqlConnection(GlobalConfig.GetConnectionString(DATABASE_NAME)))
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


        /// <summary>
        /// Returns a list of all people from the database
        /// </summary>
        /// <returns>List of person information</returns>
        public List<PersonModel> GetPerson_All()
        {
            List<PersonModel> output = new List<PersonModel>();

            using (SqlConnection connection = new SqlConnection(GlobalConfig.GetConnectionString(DATABASE_NAME)))
            {
                SqlCommand command = new SqlCommand("[dbo].[spPeople_GetAll]", connection);
                command.CommandType = CommandType.StoredProcedure;

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        PersonModel person= new PersonModel();
                        person.FirstName = reader["FirstName"].ToString();
                        person.LastName = reader["LastName"].ToString();
                        person.CellPhoneNumber = reader["CellphoneNumber"].ToString();
                        person.EmailAddress = reader["EmailAddress"].ToString();
                        bool placeNumberValidNumber = int.TryParse(reader["Id"].ToString(), out int id);
                        if (placeNumberValidNumber)
                        {
                            person.Id = id;
                        }
                        output.Add(person);
                    }
                }

            }

            return output;
        }

        /// <summary>
        /// Saves a new team to the database
        /// </summary>
        /// <param name="model">The team information<</param>
        /// <returns>The team information plus the unique identifier.</returns>
        public TeamModel CreateTeam(TeamModel teamModel)
        {
            using (SqlConnection connection = new SqlConnection(GlobalConfig.GetConnectionString(DATABASE_NAME)))
            {
                SqlCommand insertTeamCommand = new SqlCommand("[dbo].[spTeams_Insert]", connection);
                insertTeamCommand.CommandType = CommandType.StoredProcedure;
                insertTeamCommand.Parameters.AddWithValue("@TeamName", teamModel.TeamName);

                SqlParameter teamId = new SqlParameter("@Id", SqlDbType.Int);
                teamId.Direction = ParameterDirection.Output;
                insertTeamCommand.Parameters.Add(teamId);
                connection.Open();
                insertTeamCommand.ExecuteNonQuery();

                bool placeNumberValidNumber = int.TryParse(teamId.Value.ToString(), out int id);
                if (placeNumberValidNumber)
                {
                    teamModel.Id = id;
                }

                foreach (PersonModel teamMember in teamModel.TeamMembers)
                {
                    SqlCommand insertTeamMemberCommand = new SqlCommand("[dbo].[spTeamMembers_Insert]", connection);
                    insertTeamMemberCommand.CommandType = CommandType.StoredProcedure;
                    insertTeamMemberCommand.Parameters.AddWithValue("@TeamId", teamModel.Id);
                    insertTeamMemberCommand.Parameters.AddWithValue("@PersonId", teamMember.Id);

                    insertTeamMemberCommand.ExecuteNonQuery();
                }

                return teamModel;
            }
        }
    }

}