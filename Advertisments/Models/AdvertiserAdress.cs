using Advertisments.Enums;

namespace Advertisments.Models
{
    public class AdvertiserAdress
    {
        public int Id { get; set; }
        public int AdvertiserId { get; set; }
        public AddressType Type { get; set; }
        public string Address { get; set; }
        public int PostNumber { get; set; }
        public string City { get; set; }
    }
}
