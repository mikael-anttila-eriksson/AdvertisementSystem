using Advertisments.Common;
using Advertisments.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Advertisments.ModelMethods
{
    public static class AdsMethods
    {
        private const string CONNECTIONSSTRING = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Advertisements_AUFI_Labb2;Integrated Security=True";
        //---------------------------------------------------------------
        public static List<Ads> GetAds(out string errorMsg)
        {
            string sqlString = "Select * from tbl_ads";
            SqlCommand dbCommand = DatabaseCRUD.SetCommand(CONNECTIONSSTRING, sqlString);
            return DatabaseCRUD.ReadAll<Ads>(dbCommand, ReadAds, out errorMsg);
        }
        public static Ads GetAdById(int id, out string errorMsg)
        {
            string sqlString = "Select * from tbl_ads where ad_id=@id";
            SqlCommand dbCommand = DatabaseCRUD.SetCommand(CONNECTIONSSTRING, sqlString);
            dbCommand.Parameters.Add("id", SqlDbType.Int).Value = id;
            return DatabaseCRUD.ReadSingle<Ads>(dbCommand, ReadAds, out errorMsg);
        }
        public static int DeleteAd(int id, out string errorMsg)
        {
            string sqlString = "Delete from tbl_ads where ad_id=@id";
            return DatabaseCRUD.ExecuteById(CONNECTIONSSTRING, sqlString, id, out errorMsg);
        }
        public static int InsertAd(Ads ad, out string errorMsg)
        {
            string sqlString = "Insert into tbl_ads " +
                "(ad_advertiser_id, ad_subscriber_id, ad_title, ad_content, ad_price_product, ad_price_ad) " +
                "values (@advertiser_id, @subscriber_id, @title, @content, @price_product, @price_ad)";
            return ExecuteByObject(sqlString, ad, out errorMsg);
        }
        public static int UpdateAd(Ads ad, out string errorMsg)
        {
            string sqlString = "Update tbl_ads " +
                "set ad_advertiser_id=@advertiser_id, ad_subscriber_id=@subscriber_id, ad_title=@title, ad_content=@content, ad_price_product=@price_product, ad_price_ad=@price_ad " +
                "where ad_id=@id";
            return ExecuteByObject(sqlString, ad, out errorMsg);
        }
        //---------------------------------------------------------------
        private static int ExecuteByObject(string sqlString, Ads ad, out string errorMsg)
        {
            SqlCommand cmd = DatabaseCRUD.SetCommand(CONNECTIONSSTRING, sqlString);
            AddParameters(cmd, ad);
            return DatabaseCRUD.ExecuteCommandToDatabase(cmd, out errorMsg);
        }
        //---------------------------------------------------------------
        private static Ads ReadAds(SqlDataReader reader)
        {
            Ads ad = new();
            ad.Id = Convert.ToInt32(reader["ad_id"]);
            // This is because these Fk-Id:s can be null in database. When read they become DBNull.Value
            // which Convert.ToInt32 does not like!!
            // Works!!
            ad.AdvertiserId = reader["ad_advertiser_id"] == DBNull.Value ? null : Convert.ToInt32(reader["ad_advertiser_id"]);            
            ad.SubscriberId = reader["ad_subscriber_id"] == DBNull.Value ? null : Convert.ToInt32(reader["ad_subscriber_id"]);            
            ad.Title = reader["ad_title"].ToString();
            ad.Content = reader["ad_content"].ToString();
            ad.PriceProduct = Convert.ToDouble(reader["ad_price_product"]);
            ad.PriceAd = Convert.ToDouble(reader["ad_price_ad"]);
            return ad;
        }
        private static void AddParameters(SqlCommand cmd, Ads ad)
        {
            cmd.Parameters.Add("id", SqlDbType.Int).Value = ad.Id;
            cmd.Parameters.Add("advertiser_id", SqlDbType.Int).Value = ReturnNullOrInt(ad.AdvertiserId);
            cmd.Parameters.Add("subscriber_id", SqlDbType.Int).Value = ReturnNullOrInt(ad.SubscriberId);
            cmd.Parameters.Add("title", SqlDbType.VarChar, 20).Value = ad.Title;
            cmd.Parameters.Add("content", SqlDbType.VarChar, 200).Value = ad.Content;
            cmd.Parameters.Add("price_product", SqlDbType.SmallMoney).Value = ad.PriceProduct;
            cmd.Parameters.Add("price_ad", SqlDbType.SmallMoney).Value = ad.PriceAd;

        }
        //---------------------------------------------------------------
        private static object ReturnNullOrInt(int? id)
        {
            if (id <= 0 || id == null) return DBNull.Value;
            return id;
        }
        
    }
}
