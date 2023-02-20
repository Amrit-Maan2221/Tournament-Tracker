using System.Collections.Generic;
using TrackerLibrary.Models;

namespace TrackerLibrary.Data_Access
{
    public interface IDataConnection
    {
        PrizeModel CreatePrize(PrizeModel model);
        PersonModel CreatePerson(PersonModel model);
        TeamModel CreateTeam(TeamModel model);
        List<PersonModel> GetPerson_All();
        List<TeamModel> GetTeam_All();
    }
}