using Advertisments.Models;
using System.ComponentModel.DataAnnotations;

namespace Advertisments.ViewModels
{
    public class AdvertiserViewModel
    {
        // Advertiser
        [Required]
        public int Id { get; set; }
        [Required]
        public string Company { get; set; }
        [Required]
        public string OrganizationNumber { get; set; }
        public string Phone { get; set; }
        // Addresses
        public AdvertiserAdress DeliveryAddress { get; set; }
        public AdvertiserAdress? BillingAddress { get; set; }
    }
}
