using Advertisments.Common;
using Advertisments.Models;
using Advertisments.Enums;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Data.SqlTypes;
using Advertisments.ViewModels;

namespace Advertisments.ModelMethods
{
    public static class AdvertiserMethods
    {
        private const string CONNECTIONSSTRING = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Advertisements_AUFI_Labb2;Integrated Security=True";
        //---------------------------------------------------------------
        #region CRUD
        public static List<Advertiser> GetAdvertisers(out string errorMsg)
        {
            string sqlString = "Select * from tbl_advertiser";
            SqlCommand dbCommand = DatabaseCRUD.SetCommand(CONNECTIONSSTRING, sqlString);

            return DatabaseCRUD.ReadAll<Advertiser>(dbCommand, ReadAdvertiser, out errorMsg);
        }
        public static Advertiser GetAdvertiserById(int id, out string errorMsg)
        {
            string sqlString = "Select * from tbl_advertiser where ad_id=@companyId";
            SqlCommand dbCommand = DatabaseCRUD.SetCommand(CONNECTIONSSTRING, sqlString);
            dbCommand.Parameters.Add("companyId", SqlDbType.Int).Value = id;
            return DatabaseCRUD.ReadSingle<Advertiser>(dbCommand, ReadAdvertiser, out errorMsg);
        }  
        public static int DeleteAdvertiser(int id, out string errorMsg)
        {
            string sqlString = "Delete from tbl_advertiser where ad_id=@companyId";
            return ExecuteById(sqlString, id, out errorMsg);
        }
        public static int InsertAdvertiser(Advertiser advertiser, out string errorMsg)
        {
            string sqlString = "Insert into tbl_advertiser " +
                "(ad_company_name, ad_org_number, ad_phone) " +
                "values (@companyId, @company, @orgNr, @phone)";            
            return ExecuteByObject(sqlString, advertiser, out errorMsg);
        }
        public static int UpdateAdvertiser(Advertiser advertiser, out string errorMsg)
        {
            string sqlString = "Update tbl_advertiser " +
                "set ad_id=@companyId, ad_company_name=@company, ad_org_number=@orgNr, ad_phone=@phone " +
                "where ad_id=@companyId";
            return ExecuteByObject(sqlString, advertiser, out errorMsg);
        }
        #endregion CRUD
        //---------------------------------------------------------------
        #region Other
        private static int ExecuteByObject(string sqlString, Advertiser advertiser, out string errorMsg)
        {
            SqlCommand dbCommand = DatabaseCRUD.SetCommand(CONNECTIONSSTRING, sqlString);
            AddParameters(dbCommand, advertiser);
            return DatabaseCRUD.ExecuteCommandToDatabase(dbCommand, out errorMsg);
        }
        private static int ExecuteById(string sqlString, int id, out string errorMsg)
        {
            SqlCommand dbCommand = DatabaseCRUD.SetCommand(CONNECTIONSSTRING, sqlString);
            dbCommand.Parameters.Add("companyId", SqlDbType.Int).Value = id;
            return DatabaseCRUD.ExecuteCommandToDatabase(dbCommand, out errorMsg);
        }
        //---------------------------------------------------------------
        private static Advertiser ReadAdvertiser(SqlDataReader reader)
        {
            Advertiser advertiser = new Advertiser();
            advertiser.Id = Convert.ToInt32(reader["ad_id"]);
            advertiser.Company = reader["ad_company_name"].ToString();
            advertiser.OrganizationNumber = reader["ad_org_number"].ToString();
            advertiser.Phone = reader["ad_phone"].ToString();

            return advertiser;
        }
        private static void AddParameters(SqlCommand cmd, Advertiser advertiser)
        {
            cmd.Parameters.Add("companyId", SqlDbType.Int).Value = advertiser.Id;
            cmd.Parameters.Add("company", SqlDbType.VarChar, 30).Value = advertiser.Company;
            cmd.Parameters.Add("orgNr", SqlDbType.VarChar, 11).Value = advertiser.OrganizationNumber;
            cmd.Parameters.Add("phone", SqlDbType.VarChar, 10).Value = advertiser.Phone;
        }
        //---------------------------------------------------------------
        public static Advertiser MapVmTOModel(AdvertiserViewModel vmAdvertiser)
        {
            Advertiser advertiser = new()
            {
                Id = vmAdvertiser.Id,
                Company = vmAdvertiser.Company,
                OrganizationNumber = vmAdvertiser.OrganizationNumber,
                Phone = vmAdvertiser.Phone
            };
            return advertiser;
        }
        public static AdvertiserViewModel MapModelToVm(Advertiser advertiser)       // Testing 7/12 -23
        {
            IEnumerable<AdvertiserAdress> addressList = AdvertiserAddressMethods.GetAddresses(out _);
            //var add = addressList.SingleOrDefault(p => ((p.AdvertiserId == advertiser.Id) && (p.Type == AddressType.Delivery)));
            AdvertiserAdress add1 = new();//(AdvertiserAdress)addressList.Where<AdvertiserAdress>(p => p.AdvertiserId == advertiser.Id && p.Type == AddressType.Delivery);
            //AdvertiserAdress add2 = (AdvertiserAdress)addressList.Where<AdvertiserAdress>(p => p.AdvertiserId == advertiser.Id && p.Type == AddressType.Billing); 
            var add3 = addressList.Select(p => p.AdvertiserId == 1);// && p.Type == AddressType.Delivery);
            var add5 = addressList.Where(p => p.AdvertiserId == 1);
            var add6 = addressList.Where(p => p.Type == AddressType.Delivery);
            var add4 = addressList.Where(p => p.AdvertiserId == 1).Select(p => p.AdvertiserId);
            var add7 = addressList.Where(p => p.AdvertiserId == 1 && p.Type == AddressType.Delivery); //Ger mig rätt object
            add1 = (AdvertiserAdress)add7;                                                            // Kan INTE sätta =-tecken!! Måste Mappa!!
            AdvertiserViewModel vmAdvertiser = new()
            {
                Id = advertiser.Id,
                Company = advertiser.Company,
                OrganizationNumber = advertiser.OrganizationNumber,
                Phone = advertiser.Phone,
                DeliveryAddress = (AdvertiserAdress)addressList.Where<AdvertiserAdress>(p => p.AdvertiserId == advertiser.Id && p.Type == AddressType.Delivery),
                BillingAddress = (AdvertiserAdress)addressList.Where<AdvertiserAdress>(p => p.AdvertiserId == advertiser.Id && p.Type == AddressType.Billing)
            };
            return vmAdvertiser;
        }
        public static int GetLastCreatedCompany(out string errorMsg)
        {            
            return GetAdvertisers(out errorMsg).LastOrDefault().Id;
        }
        #endregion Other
        //---------------------------------------------------------------
        public static int RegisterCompany(AdvertiserViewModel newCompany, bool useBilling, out string errorMsg)
        {
            SqlConnection dbConnection = new()
            {
                ConnectionString = CONNECTIONSSTRING
            };
            SqlCommand cmd1 = new SqlCommand("", dbConnection);
            SqlCommand cmd2 = new SqlCommand("", dbConnection);
            // Call Procedure 2
            cmd2.CommandType = CommandType.StoredProcedure;
            cmd2.CommandText = "spNewAddress";

            // Transaction
            SqlTransaction transaction;
            int rowsAffected = 0;
            errorMsg = "";
            //return 2;                                       /// test , stoppar
            List<SqlParameter> sqlParameters = new();
            sqlParameters = AddParameters(cmd2);

            dbConnection.Open();
            transaction = dbConnection.BeginTransaction();
            cmd1.Transaction = transaction;
            cmd2.Transaction = transaction;
            try
            {
                cmd1 = CallNewCompany(cmd1, newCompany, out int companyId);
                rowsAffected = cmd1.ExecuteNonQuery();                
                companyId = Convert.ToInt32(cmd1.Parameters["@fk_Advertiser_Id"].Value);
                if (rowsAffected != 1) throw new Exception("Error trying to Register a company to database, procedure 1");

                sqlParameters = SetParameterValues(sqlParameters, newCompany.DeliveryAddress, companyId);
                rowsAffected += cmd2.ExecuteNonQuery();
                if (rowsAffected != 2) throw new Exception("Error trying to Register a company-address to database, procedure 2");
                if(useBilling)
                {
                    sqlParameters = SetParameterValues(sqlParameters, newCompany.BillingAddress, companyId);
                    rowsAffected += cmd2.ExecuteNonQuery();
                    if (rowsAffected != 3) throw new Exception("Error trying to Register a company-address to database, procedure 3");
                }
                //throw new Exception("df");
                transaction.Commit();
            }
            catch (SqlException slqEx)
            {
                errorMsg = $"SQL-Error: {slqEx.Message}";
                transaction.Rollback();
            }
            catch(Exception ex)
            {
                errorMsg = $"Error: {ex.Message}";
                transaction.Rollback();
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }
            
            return rowsAffected;

        }
        /// <summary>
        /// Only works if you add the parameters once.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="newCompany"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        private static SqlCommand CallNewCompany(SqlCommand cmd, AdvertiserViewModel newCompany, out int companyId)
        {
            companyId = -1;
            // Call Procedure
            cmd.CommandText = "spNewCompany";
            cmd.CommandType = CommandType.StoredProcedure;
            // Input
            cmd.Parameters.Add("@name", SqlDbType.VarChar, 30).Value = newCompany.Company;
            cmd.Parameters.Add("@orgNr", SqlDbType.VarChar, 11).Value = newCompany.OrganizationNumber;
            cmd.Parameters.Add("@phone", SqlDbType.VarChar, 10).Value = newCompany.Phone;
            // Output
            cmd.Parameters.Add("@fk_Advertiser_Id", SqlDbType.Int);
            cmd.Parameters["@fk_Advertiser_Id"].Direction = ParameterDirection.Output;
            
            return cmd;
        }
        private static SqlCommand CallAddresses(SqlCommand cmd, AdvertiserAdress address, int companyId) //Only works if you add the parameters once
        {
            // Call Procedure
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spNewAddress";
            // Input
            cmd.Parameters.Add("@fk_Advertiser_Id", SqlDbType.Int).Value = companyId;
            cmd.Parameters.Add("@type", SqlDbType.VarChar, 20).Value = address.Type.ToString();
            cmd.Parameters.Add("@address", SqlDbType.VarChar, 30).Value = address.Address;
            cmd.Parameters.Add("@postNr", SqlDbType.Int).Value = address.PostNumber;
            cmd.Parameters.Add("@city", SqlDbType.VarChar, 20).Value = address.City;

            return cmd;
        }
        //---------------------------------------------------------------
        private static List<SqlParameter> AddParameters(SqlCommand cmd)
        {
            List<SqlParameter> sqlParameters = new();

            sqlParameters.Add(cmd.Parameters.Add("@fk_Advertiser_Id", SqlDbType.Int));
            sqlParameters.Add(cmd.Parameters.Add("@type", SqlDbType.VarChar, 20));
            sqlParameters.Add(cmd.Parameters.Add("@address", SqlDbType.VarChar, 30));
            sqlParameters.Add(cmd.Parameters.Add("@postNr", SqlDbType.Int));
            sqlParameters.Add(cmd.Parameters.Add("@city", SqlDbType.VarChar, 20));
            return sqlParameters;
        }
        /// <summary>
        /// Can add value to paramters multiple times.
        /// </summary>
        /// <param name="sqlParameters"></param>
        /// <param name="address"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        private static List<SqlParameter> SetParameterValues(List<SqlParameter> sqlParameters, AdvertiserAdress address, int companyId)
        {
            // Set values
            sqlParameters[0].Value = companyId;
            sqlParameters[1].Value = address.Type;
            sqlParameters[2].Value = address.Address;
            sqlParameters[3].Value = address.PostNumber;
            sqlParameters[4].Value = address.City;
            return sqlParameters;
        }
    }
}
