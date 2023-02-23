using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
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


        public List<TournamentModel> GetTournament_All()
        {
            List<TournamentModel> tournamentList = new List<TournamentModel>();
            using (SqlConnection connection = new SqlConnection(GlobalConfig.GetConnectionString(DATABASE_NAME)))
            {
                connection.Open();
                SqlCommand getAllTournamentsCommand = new SqlCommand("[dbo].[spTournaments_GetAll]", connection);
                getAllTournamentsCommand.CommandType = CommandType.StoredProcedure;
                using (SqlDataReader reader = getAllTournamentsCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //TournamentModel:
                        //Id: int (got it)
                        //TournamnetName: string (got it)
                        //EntryFee: decimal (got it)
                        //EnteredTeams: List<TeamModel> (Got it)
                        //Prizes: List<PrizeModel> (got it)
                        //Rounds: List<List<MatchupModel>> (got it)
                        TournamentModel tournament = new TournamentModel();
                        bool placeTournamentId = int.TryParse(reader["Id"].ToString(), out int tournamentId);
                        if (placeTournamentId)
                        {
                            tournament.Id = tournamentId;
                        }
                        tournament.TournamentName = reader["TournamentName"].ToString();
                        tournament.EntryFee = decimal.Parse(reader["EntryFee"].ToString());


                        tournamentList.Add(tournament);
                    }
                }

                foreach (TournamentModel tournament in tournamentList)
                {
                    // Populate Prizes
                    tournament.Prizes = GetPrizesByTournament(tournament.Id, connection);

                    // Get Teams
                    tournament.EnteredTeams = GetTeamsByTournament(tournament.Id, connection);

                    // Get Rounds
                    tournament.Rounds = GetRoundsByTournament(tournament.Id, connection);
                }
            }

            return tournamentList;
        }

        private List<List<MatchupModel>> GetRoundsByTournament(int tournamentId, SqlConnection connection)
        {
            List<TeamModel> allTeams = GetTeam_All();

            // MatchupModel:
            // Id:              int (got it)
            // WinnerId:        int (got it)
            // Winner:          TeamModel (got it)
            // MatchupRound:    int (got it)
            // Entries:         List<MatchupEntryModel> (got it)

            List<MatchupModel> matchups = new List<MatchupModel>();
            SqlCommand getTournamentMatchUpsCommand = new SqlCommand("[dbo].[spMatchups_GetByTournament]", connection);
            getTournamentMatchUpsCommand.CommandType = CommandType.StoredProcedure;
            getTournamentMatchUpsCommand.Parameters.AddWithValue("@TournamentId", tournamentId);
            using (SqlDataReader matchupsReader = getTournamentMatchUpsCommand.ExecuteReader())
            {
                while (matchupsReader.Read())
                {
                    MatchupModel matchup = new MatchupModel();
                    matchup.Id = int.Parse(matchupsReader["Id"].ToString());
                    bool validWinnerId = int.TryParse(matchupsReader["WinnerId"].ToString(), out int winnerId);
                    if (validWinnerId)
                    {
                        matchup.WinnerId = winnerId;
                        matchup.Winner = allTeams.Where(x => x.Id == matchup.WinnerId).First();
                    }
                    matchup.MatchupRound = int.Parse(matchupsReader["MatchupRound"].ToString());
                    matchups.Add(matchup);
                }
            }


            // get match entries
            foreach (MatchupModel matchup in matchups)
            {
                SqlCommand getMatchUpEntriesCommand = new SqlCommand("[dbo].[spMatchupEntries_GetByMatchup]", connection);
                getMatchUpEntriesCommand.CommandType = CommandType.StoredProcedure;
                getMatchUpEntriesCommand.Parameters.AddWithValue("@MatchupId", matchup.Id);
                using (SqlDataReader matchupEntryReader = getMatchUpEntriesCommand.ExecuteReader())
                {
                    while (matchupEntryReader.Read())
                    {
                        // MatchupEntryModel:
                        // Id (got it),
                        // TeamCompetingId (got it)
                        // TeamCompeting (got it)
                        // Score (got it)
                        MatchupEntryModel matchupEntry = new MatchupEntryModel();
                        matchupEntry.Id = int.Parse(matchupEntryReader["Id"].ToString());
                        bool validTeamCompetingId = int.TryParse(matchupEntryReader["TeamCompetingId"].ToString(), out int teamCompetingId);
                        if (validTeamCompetingId)
                        {
                            matchupEntry.TeamCompetingId = teamCompetingId;
                            matchupEntry.TeamCompeting = allTeams.Where(x => x.Id == matchupEntry.TeamCompetingId).First();
                        }

                        bool validParentMatchId = int.TryParse(matchupEntryReader["ParentMatchupId"].ToString(), out int parentMatchId);
                        if (validParentMatchId)
                        {
                            matchupEntry.ParentMatchupId = parentMatchId;
                            matchupEntry.ParentMatchup = matchups.Where(x => x.Id == matchupEntry.ParentMatchupId).First();
                        }

                        bool validScoreData = double.TryParse(matchupEntryReader["Score"].ToString(), out double scoreData);
                        if (validScoreData)
                        {
                            matchupEntry.Score = scoreData;
                        }

                        matchup.Entries.Add(matchupEntry);
                    }
                }
            }


            List<List<MatchupModel>> Rounds = new List<List<MatchupModel>>();
            // List<List<MatchupModel>>
            List<MatchupModel> currRow = new List<MatchupModel>();
            int currRound = 1;

            foreach (MatchupModel m in matchups)
            {
                if (m.MatchupRound > currRound)
                {
                    Rounds.Add(currRow);
                    currRow = new List<MatchupModel>();
                    currRound += 1;
                }

                currRow.Add(m);
            }

            Rounds.Add(currRow);


            return Rounds;
        }

        private List<TeamModel> GetTeamsByTournament(int tournamentId, SqlConnection connection)
        {
            List<TeamModel> tournamentTeams = new List<TeamModel>();
            SqlCommand getTeamsByTournamentCommand = new SqlCommand("[dbo].[spTeam_getByTournament]", connection);
            getTeamsByTournamentCommand.CommandType = CommandType.StoredProcedure;
            getTeamsByTournamentCommand.Parameters.AddWithValue("@TournamentId", tournamentId);
            using (SqlDataReader teamsReader = getTeamsByTournamentCommand.ExecuteReader())
            {
                while (teamsReader.Read())
                {
                    TeamModel team = new TeamModel();
                    bool validPrizeId = int.TryParse(teamsReader["Id"].ToString(), out int teamId);
                    if (validPrizeId)
                    {
                        team.Id = teamId;
                    }

                    team.TeamName = teamsReader["TeamName"].ToString();





                    tournamentTeams.Add(team);
                }
            }

            foreach (TeamModel team in tournamentTeams)
            {
                SqlCommand getTeamMembersCommand = new SqlCommand("[dbo].[spTeamMembers_GetByTeam]", connection);
                getTeamMembersCommand.CommandType = CommandType.StoredProcedure;
                getTeamMembersCommand.Parameters.AddWithValue("@TeamId", team.Id);
                using (SqlDataReader teamMembersReader = getTeamMembersCommand.ExecuteReader())
                {
                    while (teamMembersReader.Read())
                    {
                        PersonModel person = new PersonModel();
                        person.FirstName = teamMembersReader["FirstName"].ToString();
                        person.LastName = teamMembersReader["LastName"].ToString();
                        person.CellPhoneNumber = teamMembersReader["CellphoneNumber"].ToString();
                        person.EmailAddress = teamMembersReader["EmailAddress"].ToString();
                        bool memberValid = int.TryParse(teamMembersReader["Id"].ToString(), out int memberId);
                        if (memberValid)
                        {
                            person.Id = memberId;
                        }
                        team.TeamMembers.Add(person);
                    }
                }
            }

            return tournamentTeams;
        }

        private List<PrizeModel> GetPrizesByTournament(int tournamentId, SqlConnection connection)
        {
            List<PrizeModel> tournamentPrizes = new List<PrizeModel>();
            SqlCommand getPrizesByTournamentsCommand = new SqlCommand("[dbo].[spPrizes_GetByTournament]", connection);
            getPrizesByTournamentsCommand.CommandType = CommandType.StoredProcedure;
            getPrizesByTournamentsCommand.Parameters.AddWithValue("@TournamentId", tournamentId);
            using (SqlDataReader reader = getPrizesByTournamentsCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    PrizeModel prize = new PrizeModel();
                    bool validPrizeId = int.TryParse(reader["Id"].ToString(), out int prizeId);
                    if (validPrizeId)
                    {
                        prize.Id = prizeId;
                    }

                    prize.PlaceName = reader["PlaceName"].ToString();
                    prize.PlaceNumber = int.Parse(reader["PlaceNumber"].ToString());
                    prize.PrizePercentage = double.Parse(reader["PrizePercentage"].ToString());
                    prize.PrizeAmount = decimal.Parse(reader["PrizeAmount"].ToString());
                    tournamentPrizes.Add(prize);
                }
            }

            return tournamentPrizes;
        }

        /// <summary>
        /// Saves a new prize to the database
        /// </summary>
        /// <param name="model">The prize information</param>
        /// <returns>The prize information plus the unique identifier.</returns>
        public void CreatePrize(PrizeModel model)
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
        }


        /// <summary>
        /// Saves a new person to the database
        /// </summary>
        /// <param name="model">The person information</param>
        /// <returns>The person information plus the unique identifier.</returns>
        public void CreatePerson(PersonModel model)
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
        }




        /// <summary>
        /// Saves a new team to the database
        /// </summary>
        /// <param name="model">The team information<</param>
        /// <returns>The team information plus the unique identifier.</returns>
        public void CreateTeam(TeamModel teamModel)
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
            insertTournamentCommand.Parameters.AddWithValue("@TournamentName", model.TournamentName);
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

        public void UpdateMatchup(MatchupModel model)
        {
            using (SqlConnection connection = new SqlConnection(GlobalConfig.GetConnectionString(DATABASE_NAME)))
            {
                connection.Open();
                if (model.Winner != null)
                {
                    SqlCommand updateMatchCommand = new SqlCommand("[dbo].[spMatchups_Update]", connection);
                    updateMatchCommand.CommandType = CommandType.StoredProcedure;
                    updateMatchCommand.Parameters.AddWithValue("@Id", model.Id);
                    updateMatchCommand.Parameters.AddWithValue("@WinnerId", model.Winner.Id);
                    updateMatchCommand.ExecuteNonQuery();
                }

                foreach (MatchupEntryModel matchEntry in model.Entries)
                {
                    if (matchEntry.TeamCompeting != null)
                    {
                        SqlCommand updateMatchEntryCommand = new SqlCommand("[dbo].[spMatchupEntries_Update]", connection);
                        updateMatchEntryCommand.CommandType = CommandType.StoredProcedure;
                        updateMatchEntryCommand.Parameters.AddWithValue("@Id", matchEntry.Id);
                        updateMatchEntryCommand.Parameters.AddWithValue("@TeamCompetingId", matchEntry.TeamCompeting.Id);
                        updateMatchEntryCommand.Parameters.AddWithValue("@Score", matchEntry.Score);
                        updateMatchEntryCommand.ExecuteNonQuery();
                    }
                }
            }
        }
    }

}