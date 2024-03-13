using Advertisments.Common;
using Advertisments.Enums;
using Advertisments.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Net;

namespace Advertisments.ModelMethods
{
    public static class AdvertiserAddressMethods
    {
        private const string CONNECTIONSSTRING = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Advertisements_AUFI_Labb2;Integrated Security=True";
        //---------------------------------------------------------------
        public static List<AdvertiserAdress> GetAddresses(out string errorMsg)
        {
            string sqlString = "Select * from tbl_address";
            SqlCommand dbCommand = DatabaseCRUD.SetCommand(CONNECTIONSSTRING, sqlString);
            return DatabaseCRUD.ReadAll<AdvertiserAdress>(dbCommand, ReadAddress, out errorMsg);
        }
        public static AdvertiserAdress GetAddressById(int id, out string errorMsg)
        {
            string sqlString = "Select * from tbl_address where ad_id=@id";
            SqlCommand dbCommand = DatabaseCRUD.SetCommand(CONNECTIONSSTRING, sqlString);
            dbCommand.Parameters.Add("id", SqlDbType.Int).Value = id;
            return DatabaseCRUD.ReadSingle<AdvertiserAdress>(dbCommand, ReadAddress, out errorMsg);
        }
        public static int DeleteAddress(int id, out string errorMsg)
        {
            string sqlString = "Delete from tbl_advertiser where ad_id=@id";
            //return ExecuteById(sqlString, id, out errorMsg);
            return DatabaseCRUD.ExecuteById(CONNECTIONSSTRING, sqlString, id, out errorMsg);
        }
        public static int InsertAddress(AdvertiserAdress address, out string errorMsg)
        {
            string sqlString = "Insert into tbl_address " +
                "(ad_advertiser_id, ad_type, ad_address, ad_post_num, ad_city) " +
                "values (@advertiserId, @type, @address, @postMr, @city)";
            return ExecuteByObject(sqlString, address, out errorMsg);
        }
        public static int UpdateAddess(AdvertiserAdress address, out string errorMsg)
        {
            string sqlString = "Update tbl_advertiser " +
                "set ad_id=@id, ad_advertiser_id=@advertiserId, ad_type=@type, ad_address=@address, ad_post_num=@postNr, ad_city=@city " +
                "where ad_id=@id";
            return ExecuteByObject(sqlString, address, out errorMsg);
        }
        //---------------------------------------------------------------
        private static int ExecuteByObject(string sqlString, AdvertiserAdress address, out string errorMsg)
        {
            SqlCommand cmd = DatabaseCRUD.SetCommand(CONNECTIONSSTRING, sqlString);
            AddParameters(cmd, address);
            return DatabaseCRUD.ExecuteCommandToDatabase(cmd, out errorMsg);
        }
        /// <summary>
        /// Ser likadan ut som i AdvertiserMethods!!! Kanske kan generalisera!! :D
        /// </summary>
        /// <param name="sqlString"></param>
        /// <param name="id"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        private static int ExecuteById(string sqlString, int id, out string errorMsg)
        {
            SqlCommand cmd = DatabaseCRUD.SetCommand(CONNECTIONSSTRING, sqlString);
            cmd.Parameters.Add("id", SqlDbType.Int).Value = id;
            return DatabaseCRUD.ExecuteCommandToDatabase(cmd, out errorMsg);
        }
        //---------------------------------------------------------------
        private static AdvertiserAdress ReadAddress(SqlDataReader reader)
        {
            AdvertiserAdress address = new();
            address.Id = Convert.ToInt32(reader["ad_id"]);
            address.AdvertiserId = Convert.ToInt32(reader["ad_advertiser_id"]);
            if(!Enum.TryParse(reader["ad_type"].ToString(), out AddressType type))
            {
                type = AddressType.Delivery;
            }
            address.Type = type;
            address.Address = reader["ad_address"].ToString();
            address.PostNumber = Convert.ToInt32(reader["ad_post_num"]);
            address.City = reader["ad_city"].ToString();

            return address;
        }
        private static void AddParameters(SqlCommand cmd, AdvertiserAdress address)
        {
            cmd.Parameters.Add("id", SqlDbType.Int).Value = address.Id;
            cmd.Parameters.Add("advertiserId", SqlDbType.Int).Value = address.AdvertiserId;
            cmd.Parameters.Add("type", SqlDbType.VarChar, 20).Value = address.Type;
            cmd.Parameters.Add("address", SqlDbType.VarChar, 30).Value = address.Address;
            cmd.Parameters.Add("postNr", SqlDbType.Int).Value = address.PostNumber;
            cmd.Parameters.Add("city", SqlDbType.VarChar, 20).Value = address.City;
        }
        //---------------------------------------------------------------
        //---------------------------------------------------------------
    }
}
