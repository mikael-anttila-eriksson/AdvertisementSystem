using System.ComponentModel.DataAnnotations;

namespace Advertisments.Models
{
    public class Subscriber
    {
        public int SubscriberId { get; set; }
        [MinLength(13, ErrorMessage = "Too short, should be 12 numbers + dash(-)")]
        [MaxLength(13, ErrorMessage = "Msg dont show, instead impossible to write more than 13 characters!")]
        public string PersonNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public int PostNumber { get; set; }
        public string City { get; set; }
    }
}
