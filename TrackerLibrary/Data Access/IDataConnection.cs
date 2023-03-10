using System.Collections.Generic;
using TrackerLibrary.Models;

namespace TrackerLibrary.Data_Access
{
    public interface IDataConnection
    {
        void CreatePrize(PrizeModel model);
        void CreatePerson(PersonModel model);
        void CreateTeam(TeamModel model);
        void CreateTournament(TournamentModel model);
        List<PersonModel> GetPerson_All();
        List<TeamModel> GetTeam_All();
        List<TournamentModel> GetTournament_All();

        void UpdateMatchup(MatchupModel model);

        void CompleteTournament(TournamentModel model);
    }
}