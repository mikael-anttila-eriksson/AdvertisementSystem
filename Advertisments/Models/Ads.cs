using System.ComponentModel.DataAnnotations;

namespace Advertisments.Models
{
    public class Ads
    {
        public int Id { get; set; }
        public int? AdvertiserId { get; set; }
        public int? SubscriberId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = true)]
        public double PriceProduct { get; set; }
        public double PriceAd { get; set; }
        /// <summary>
        /// My ugly solution to the double-number problem
        /// </summary>
        public string Price_Product_String
        { get { return PriceProduct.ToString(); } 
          set { PriceProduct = Convert.ToDouble(value); }
        }
    }
}
