namespace TrackerLibrary.Models
{
    namespace TrackerLibrary.Models
    {
        public class TeamModel
        {
            public int Id { get; set; }
            public string TeamName { get; set; } = string.Empty;
            public List<PersonModel> TeamMembers { get; set; } = new List<PersonModel>();
        }
    }
}
