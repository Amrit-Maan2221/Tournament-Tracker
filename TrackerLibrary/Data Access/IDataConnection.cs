using TrackerLibrary.Models;

namespace TrackerLibrary.Data_Access
{
    public interface IDataConnection
    {
        PrizeModel CreatePrize(PrizeModel model);
        PersonModel CreatePerson(PersonModel model);
    }
}