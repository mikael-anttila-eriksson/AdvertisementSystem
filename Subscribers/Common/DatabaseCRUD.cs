using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Globalization;
using Microsoft.Data.SqlClient;
using Subscriber_API.Models;

namespace Subscriber_API.Commonn;

public static class DatabaseCRUD
{

    
    //---------------------------------------------------------------
    public static SqlCommand SetCommand(string connectionString, string sqlString)
    {
        SqlConnection dbConnection = new();
        dbConnection.ConnectionString = connectionString;
        SqlCommand dbCommand = new SqlCommand(sqlString, dbConnection);

        // if Loop new parameters
        //List<SqlParameter> parameters = new List<SqlParameter>();
        //parameters = AddParameters(dbCommand);
        //parameters = SetParameterValues()

        return dbCommand;
    }
    // Set Parameters
    private static void SetParameters(SqlCommand cmd) // som input - Hard to use them here, need to be set at specific class, I think...
    {
        //cmd.Parameters.Add("fornamn", SqlDbType.VarChar, 30).Value = pd.Fornamn;
        //cmd.Parameters.Add("efternamn", SqlDbType.VarChar, 30).Value = pd.Efternamn;
        //cmd.Parameters.Add("fodelsear", SqlDbType.Int).Value = pd.Fodelsear;
        //cmd.Parameters.Add("bor", SqlDbType.Int).Value = pd.Bor;
        //cmd.Parameters.Add("epost", SqlDbType.VarChar, 50).Value = pd.Epost;
    }
    //---------------------------------------------------------------
    #region Add/Set Parameters -  Easier to set at specific class, I think...
    private static List<SqlParameter> AddParameters(SqlCommand cmd)
    {
        List<SqlParameter> sqlParameter = new();

        // Set data
        sqlParameter.Add(cmd.Parameters.Add("@stock", SqlDbType.VarChar, 8));
        sqlParameter.Add(cmd.Parameters.Add("@date", SqlDbType.Date));
        sqlParameter.Add(cmd.Parameters.Add("@open", SqlDbType.Decimal, 10));
        sqlParameter.Add(cmd.Parameters.Add("@close", SqlDbType.Decimal, 10));
        sqlParameter.Add(cmd.Parameters.Add("@high", SqlDbType.Decimal, 10));
        sqlParameter.Add(cmd.Parameters.Add("@low", SqlDbType.Decimal, 10));
        sqlParameter.Add(cmd.Parameters.Add("@volume", SqlDbType.Int));

        return sqlParameter;
    }
    private static List<SqlParameter> SetParameterValues(List<SqlParameter> list, string[] row, string stock)
    {
        // Configure conversion
        NumberFormatInfo provider = new();
        provider.CurrencyDecimalSeparator = "."; // To be able to convert the strings to double.
                                                 // Set values
        list[0].Value = stock;                              // [Stock]
        list[1].Value = Convert.ToDateTime(row[0]);         // [Date]
        list[2].Value = Convert.ToDouble(row[1], provider); // [Open]
        list[3].Value = Convert.ToDouble(row[4], provider); // [Close]
        list[4].Value = Convert.ToDouble(row[2], provider); // [High]
        list[5].Value = Convert.ToDouble(row[3], provider); // [Low]
        list[6].Value = Convert.ToInt32(row[6]);            // [Volume]
                                                            //row[5] "ADJ Close" is NOT USED!

        return list;
    }
    #endregion Add/Set Parameters -  Easier to set at specific class, I think...
    //---------------------------------------------------------------    
    #region Database Calls
    /// <summary>
    /// For INSERT, UPDATE, DELETE - Operations.
    /// </summary>
    /// <param name="dbCommand"></param>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static int ExecuteCommandToDatabase(SqlCommand dbCommand, out string errorMsg)
    {
        // Insert into Database
        try
        {
            // open
            dbCommand.Connection.Open();
            int rowsAffected = 0;
            rowsAffected = dbCommand.ExecuteNonQuery();
            if (rowsAffected == 1)
            {
                // No Error
                errorMsg = "";
            }
            else
            {
                // Error
                errorMsg = "Error (at Execute command) performing operation on the database";
            }
            return rowsAffected;
        }
        catch (Exception ex)
        {
            errorMsg = ex.Message;
            return 0; // no row affected
        }
        finally
        {
            dbCommand.Connection.Close();
        }
    }
    
    public static List<T> ReadAll<T>(SqlCommand dbCommand, Func<SqlDataReader, T> ReadT, out string errorMsg) where T : new()
    {
        // Read data
        SqlDataReader reader = null;
        errorMsg = "";
        List<T> accounts = new List<T>();

        try
        {
            // Open
            dbCommand.Connection.Open();
            reader = dbCommand.ExecuteReader();

            // Start read
            while (reader.Read())
            {
                List<T> accRow = new();
                T propertyClass = ReadT(reader);

                accounts.Add(propertyClass);
            }
            reader.Close();
            return accounts;
        }
        catch (Exception ex)
        {
            errorMsg = ex.Message;
            return null;
        }
        finally
        {
            dbCommand.Connection.Close();
        }
    }
    public static T ReadSingle<T>(SqlCommand sql, Func<SqlDataReader, T> ReadT, out string errorMsg) where T : new()
    {
        errorMsg = "";
        SqlDataReader reader = null;
        try
        {
            sql.Connection.Open();
            reader = sql.ExecuteReader();

            reader.Read();
            T propertyClass = ReadT(reader);
            reader.Close();
            return propertyClass;

        }
        catch (Exception ex)
        {
            errorMsg = ex.Message;
            return default(T);
        }
        finally
        {
            sql.Connection.Close();
        }
    }
    #endregion Database Calls
}


