using Subscriber_API.Models;
using Subscriber_API.Commonn;
using Microsoft.Data.SqlClient;
using System.Data.SqlTypes;
using System.Data;
using System.Reflection;
using System.Linq;
using System.Collections;
using System.Data.Common;
using System.Net;
using System.Numerics;

namespace Subscriber_API.ModelMethods
{
    public static class SubscriberMethods
    {
        public const string CONNECTIONSTRING = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Subscriber_AUFI_Labb2;Integrated Security=True";

        // Tryíng to make writing database-names easier, this was not as smooth as I hoped, maybe Enum would work better?!? 
        private const string TABLE_SUB = "tbl_subscriber";
        private const string SUBSCRIBERID = "su_id";
        private const string PERSON_NR = "su_person_id";
        private const string FIRSTNAME = "su_firstname";
        private const string LASTNAME = "su_lastname";
        private const string PHONE = "su_phone";
        private const string ADDRESS = "su_address";
        private const string POST_NR = "su_post_num";
        private const string CITY = "su_city";
        //---------------------------------------------------------------
        #region CRUD
        public static List<Subscriber> GetSubscribers(out string errorMsg)
        {
            string sqlString = "Select * from tbl_subscriber";
            SqlCommand dbCommand = DatabaseCRUD.SetCommand(CONNECTIONSTRING, sqlString);

            // Read            
            return DatabaseCRUD.ReadAll<Subscriber>(dbCommand, ReadSubscriber, out errorMsg);            
        }
        public static Subscriber GetById(int id, out string errorMsg)
        {
            string sqlString = "Select * from tbl_subscriber where su_id=@id";
            SqlCommand dbCommand = DatabaseCRUD.SetCommand(CONNECTIONSTRING, sqlString);
            dbCommand.Parameters.Add("id", SqlDbType.Int).Value = id;

            // Read
            return DatabaseCRUD.ReadSingle<Subscriber>(dbCommand, ReadSubscriber, out errorMsg);            
        }
        public static int InsertSubscriber(Subscriber subscriber, out string errorMsg)
        {
            string sqlString = "Insert into tbl_subscriber " +
                $"({PERSON_NR}, {FIRSTNAME}, {LASTNAME}, {PHONE}, {ADDRESS}, {POST_NR}, {CITY}) " +
                "values(@personNr, @firstname, @lastname, @phone, @address, @postNr, @city)";

            SqlCommand dbCommand = DatabaseCRUD.SetCommand(CONNECTIONSTRING, sqlString);
            AddParameters(dbCommand, subscriber);

            // Insert
            return DatabaseCRUD.ExecuteCommandToDatabase(dbCommand, out errorMsg);            
        }
        public static int DeleteSubscriber(int id, out string errorMsg)         // Gör dessa klart xxxxxxxxxxxxxxxxxxxxxxx
        {
            string sqlString = $"Delete from {TABLE_SUB} where {SUBSCRIBERID}=@id";

            SqlCommand dbCommand = DatabaseCRUD.SetCommand(CONNECTIONSTRING, sqlString);
            dbCommand.Parameters.Add("id", SqlDbType.Int).Value = id;
            // Delete
            return DatabaseCRUD.ExecuteCommandToDatabase(dbCommand, out errorMsg);            
        }
        public static int UpdateSubscriber(Subscriber subscriber, out string errorMsg)
        {
            string sqlString = $"Update {TABLE_SUB} " +
                $"set {PERSON_NR}=@personNr, {FIRSTNAME}=@firstname, {LASTNAME}=@lastname, " +
                $"{PHONE}=@phone, {ADDRESS}=@address, {POST_NR}=@postNr, {CITY}=@city " +
                $"where {SUBSCRIBERID}=@id";

            SqlCommand dbCommand = DatabaseCRUD.SetCommand(CONNECTIONSTRING, sqlString);
            AddParameters(dbCommand, subscriber);
            // Update
            return DatabaseCRUD.ExecuteCommandToDatabase(dbCommand, out errorMsg);
        }
        #endregion CRUD
        //---------------------------------------------------------------
        #region Connect Models to Database
             
        private static Subscriber ReadSubscriber(SqlDataReader reader)
        {
            
            Subscriber subscriber = new();
            subscriber.SubscriberId = Convert.ToInt32(reader[SUBSCRIBERID]);
            subscriber.PersonNumber = reader[PERSON_NR].ToString();
            subscriber.FirstName = reader[FIRSTNAME].ToString();
            subscriber.LastName = reader[LASTNAME].ToString();
            subscriber.Phone = reader[PHONE].ToString();
            subscriber.Address = reader[ADDRESS].ToString();
            subscriber.PostNumber = Convert.ToInt32(reader[POST_NR]);
            subscriber.City = reader[CITY].ToString();

            return subscriber;
        }
        private static void AddParameters(SqlCommand cmd, Subscriber subscriber)
        {
            // Add
            cmd.Parameters.Add("id", SqlDbType.Int).Value = subscriber.SubscriberId; 
            cmd.Parameters.Add("personNr", SqlDbType.VarChar, 13).Value = subscriber.PersonNumber;
            cmd.Parameters.Add("firstname", SqlDbType.VarChar, 30).Value = subscriber.FirstName;
            cmd.Parameters.Add("lastname", SqlDbType.VarChar, 30).Value = subscriber.LastName;
            cmd.Parameters.Add("phone", SqlDbType.VarChar, 10).Value = subscriber.Phone;
            cmd.Parameters.Add("address", SqlDbType.VarChar, 30).Value = subscriber.Address;
            cmd.Parameters.Add("postNr", SqlDbType.Int).Value = subscriber.PostNumber;
            cmd.Parameters.Add("city", SqlDbType.VarChar, 20).Value = subscriber.City;
        }
        #endregion Connect Models to Database
    }
}
