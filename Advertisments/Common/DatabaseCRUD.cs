using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Globalization;
using Microsoft.Data.SqlClient;
using Advertisments.Models;

namespace Advertisments.Common;

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
    /// <summary>
    /// id must be spelled like "@id" in sql-string.
    /// </summary>
    /// <param name="connectionString"></param>
    /// <param name="sqlString"></param>
    /// <param name="id"></param>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static int ExecuteById(string connectionString, string sqlString, int id, out string errorMsg)
    {
        SqlConnection dbConnection = new();
        dbConnection.ConnectionString = connectionString;
        SqlCommand dbCommand = new SqlCommand(sqlString, dbConnection);
        dbCommand.Parameters.Add("id", SqlDbType.Int).Value = id;
        return ExecuteCommandToDatabase(dbCommand, out errorMsg);
    }
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
                errorMsg = "Error (at ExecuteByObject command) performing operation on the database";
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
        List<T> items = new List<T>();

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

                items.Add(propertyClass);
            }
            reader.Close();
            return items;
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


