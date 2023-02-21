using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using TrackerLibrary.Models;

namespace TrackerLibrary.Data_Access
{
    internal class SqlConnector : IDataConnection
    {
        private const string DATABASE_NAME = "Tournaments";

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
                        PersonModel person = new PersonModel();
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
        /// Returns a list of all teams from the database
        /// </summary>
        /// <returns>List of teams information</returns>
        public List<TeamModel> GetTeam_All()
        {
            List<TeamModel> output = new List<TeamModel>();

            using (SqlConnection connection = new SqlConnection(GlobalConfig.GetConnectionString(DATABASE_NAME)))
            {
                SqlCommand command = new SqlCommand("[dbo].[spTeams_GetAll]", connection);
                command.CommandType = CommandType.StoredProcedure;
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        TeamModel team = new TeamModel();
                        team.TeamName = reader["TeamName"].ToString();
                        bool placeNumberValidNumber = int.TryParse(reader["Id"].ToString(), out int id);
                        if (placeNumberValidNumber)
                        {
                            team.Id = id;
                        }
                        output.Add(team);
                    }
                }

                // Retrive team members
                command = new SqlCommand("[dbo].[spTeamMembers_GetByTeam]", connection);
                command.CommandType = CommandType.StoredProcedure;
                foreach (TeamModel team in output)
                {
                    command.Parameters.AddWithValue("@TeamId", team.Id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PersonModel person = new PersonModel();
                            person.FirstName = reader["FirstName"].ToString();
                            person.LastName = reader["LastName"].ToString();
                            person.CellPhoneNumber = reader["CellphoneNumber"].ToString();
                            person.EmailAddress = reader["EmailAddress"].ToString();
                            bool placeNumberValidNumber = int.TryParse(reader["Id"].ToString(), out int id);
                            if (placeNumberValidNumber)
                            {
                                person.Id = id;
                            }
                            team.TeamMembers.Add(person);
                        }
                    }
                    // Clear the parameters
                    command.Parameters.Clear();
                }
            }

            return output;
        }

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


        /// <summary>
        /// Saves a new Tournament to the database
        /// </summary>
        /// <param name="model">The Tournament information<</param>
        public void CreateTournament(TournamentModel model)
        {
            using (SqlConnection connection = new SqlConnection(GlobalConfig.GetConnectionString(DATABASE_NAME)))
            {
                connection.Open();
                SaveTournament(connection, model);

                SaveTournamentPrizes(connection, model);

                SaveTournamentEntries(connection, model);

                SaveTournamentRounds(connection, model);
            }
        }



        private void SaveTournament(SqlConnection connection, TournamentModel model)
        {
            SqlCommand insertTournamentCommand = new SqlCommand("[dbo].[spTournaments_Insert]", connection);
            insertTournamentCommand.CommandType = CommandType.StoredProcedure;
            insertTournamentCommand.Parameters.AddWithValue("@TournamentName", model.TournamnetName);
            insertTournamentCommand.Parameters.AddWithValue("@EntryFee", model.EntryFee);

            SqlParameter tournamentId = new SqlParameter("@Id", SqlDbType.Int);
            tournamentId.Direction = ParameterDirection.Output;
            insertTournamentCommand.Parameters.Add(tournamentId);
            insertTournamentCommand.ExecuteNonQuery();

            bool ValidNumber = int.TryParse(tournamentId.Value.ToString(), out int id);
            if (ValidNumber)
            {
                model.Id = id;
            }
        }

        private void SaveTournamentPrizes(SqlConnection connection, TournamentModel model)
        {
            foreach (PrizeModel pz in model.Prizes)
            {
                SqlCommand insertTournamentPrizesCommand = new SqlCommand("[dbo].[spTournamentPrizes_Insert]", connection);
                insertTournamentPrizesCommand.CommandType = CommandType.StoredProcedure;
                insertTournamentPrizesCommand.Parameters.AddWithValue("@TournamentId", model.Id);
                insertTournamentPrizesCommand.Parameters.AddWithValue("@PrizeId", pz.Id);

                SqlParameter tournamentPrizeId = new SqlParameter("@Id", SqlDbType.Int);
                tournamentPrizeId.Direction = ParameterDirection.Output;
                insertTournamentPrizesCommand.Parameters.Add(tournamentPrizeId);
                insertTournamentPrizesCommand.ExecuteNonQuery();
            }
        }

        private void SaveTournamentEntries(SqlConnection connection, TournamentModel model)
        {
            foreach (TeamModel team in model.EnteredTeams)
            {
                SqlCommand insertTournamentEnteriesCommand = new SqlCommand("[dbo].[spTournamentEntries_Insert]", connection);
                insertTournamentEnteriesCommand.CommandType = CommandType.StoredProcedure;
                insertTournamentEnteriesCommand.Parameters.AddWithValue("@TournamentId", model.Id);
                insertTournamentEnteriesCommand.Parameters.AddWithValue("@TeamId", team.Id);

                SqlParameter tournamentEntriesId = new SqlParameter("@Id", SqlDbType.Int);
                tournamentEntriesId.Direction = ParameterDirection.Output;
                insertTournamentEnteriesCommand.Parameters.Add(tournamentEntriesId);
                insertTournamentEnteriesCommand.ExecuteNonQuery();
            }
        }

        private void SaveTournamentRounds(SqlConnection connection, TournamentModel model)
        {
            foreach (List<MatchupModel> round in model.Rounds)
            {
                foreach (MatchupModel matchup in round)
                {
                    SqlCommand insertMatchupsCommand = new SqlCommand("[dbo].[spMatchups_Insert]", connection);
                    insertMatchupsCommand.CommandType = CommandType.StoredProcedure;
                    insertMatchupsCommand.Parameters.AddWithValue("@TournamentId", model.Id);
                    insertMatchupsCommand.Parameters.AddWithValue("@MatchupRound", matchup.MatchupRound);

                    SqlParameter matchupId = new SqlParameter("@Id", SqlDbType.Int);
                    matchupId.Direction = ParameterDirection.Output;
                    insertMatchupsCommand.Parameters.Add(matchupId);
                    insertMatchupsCommand.ExecuteNonQuery();

                    bool ValidNumber = int.TryParse(matchupId.Value.ToString(), out int id);
                    if (ValidNumber)
                    {
                        matchup.Id = id;
                    }

                    foreach (MatchupEntryModel entry in matchup.Entries)
                    {
                        SqlCommand insertMatchupsEntryCommand = new SqlCommand("[dbo].[spMatchupEntries_Insert]", connection);
                        insertMatchupsEntryCommand.CommandType = CommandType.StoredProcedure;
                        insertMatchupsEntryCommand.Parameters.AddWithValue("@MatchupId", matchup.Id);
                        
                        if (entry.ParentMatchup == null)
                        {
                            insertMatchupsEntryCommand.Parameters.AddWithValue("@ParentMatchupId", DBNull.Value);
                        }
                        else
                        {
                            insertMatchupsEntryCommand.Parameters.AddWithValue("@ParentMatchupId", entry.ParentMatchup.Id);
                        }

                        if (entry.TeamCompeting == null)
                        {
                            insertMatchupsEntryCommand.Parameters.AddWithValue("@TeamCompetingId", DBNull.Value);
                        }
                        else
                        {
                            insertMatchupsEntryCommand.Parameters.AddWithValue("@TeamCompetingId", entry.TeamCompeting.Id);
                        }
                        SqlParameter matchupEntryId = new SqlParameter("@Id", SqlDbType.Int);
                        matchupEntryId.Direction = ParameterDirection.Output;
                        insertMatchupsEntryCommand.Parameters.Add(matchupEntryId);
                        insertMatchupsEntryCommand.ExecuteNonQuery();
                    }
                }
            }
        }
    }

}