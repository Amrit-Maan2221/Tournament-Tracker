namespace TrackerLibrary.Models
{
    /// <summary>
    /// Represents one person
    /// </summary>
    public class PersonModel
    {
        /// <summary>
        /// The unique identifier for the person
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The first name of the person
        /// </summary>
        public string FirstName { get; set; } = string.Empty;
        /// <summary>
        /// the Last Name of the person
        /// </summary>
        public string Lastname { get; set; } = string.Empty;
        /// <summary>
        /// The primary email address of the person
        /// </summary>
        public string EmailAddress { get; set; } = string.Empty; 
        /// <summary>
        /// The primary cell phone number of the person
        /// </summary>
        public string CellPhoneNumber { get; set; } = string.Empty; 

        public string FullName
        {
            get
            {
                return $"{FirstName} {Lastname}";
            }
        }
    }
}