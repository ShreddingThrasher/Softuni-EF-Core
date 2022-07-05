using System;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace _05.ChangeTownNamesCasing
{
    class StartUp
    {
        static void Main(string[] args)
        {
            const string _connectionString =
                @"Server=.;Database=MinionsDB;Integrated Security=True;";

            string country = Console.ReadLine();

            using SqlConnection sqlConnection =
                new SqlConnection(_connectionString);
            sqlConnection.Open();

            int changedCount = UpdateTownNamesToUpperCase(sqlConnection, country);
            string[] changedNames = GetTownNames(sqlConnection, country);

            if(changedCount > 0)
            {
                Console.WriteLine($"{changedCount} town names were affected.");
                Console.WriteLine($"[{string.Join(", ", changedNames)}]");
            }
            else
            {
                Console.WriteLine("No town names were affected.");
            }

            sqlConnection.Close();
        }

        /// <summary>
        /// Using an open Sql Connection updates town names by a given country to upper case.
        /// Returns the number of changed towns.
        /// </summary>
        /// <param name="sqlConnection">Open Sql Connection</param>
        /// <param name="country">Country</param>
        /// <returns></returns>
        static int UpdateTownNamesToUpperCase(SqlConnection sqlConnection, string country)
        {
            string updateQuery = @"UPDATE Towns
                                   SET Name = UPPER(Name)
                                   WHERE CountryCode = (SELECT c.Id FROM Countries AS c WHERE c.Name = @countryName)";

            SqlCommand updateCommand = new SqlCommand(updateQuery, sqlConnection);
            updateCommand.Parameters.AddWithValue("@countryName", country);

            return updateCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Using an open Sql Connection returns all Town Names by a given country.
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <param name="country"></param>
        /// <returns></returns>
        private static string[] GetTownNames(SqlConnection sqlConnection, string country)
        {
            List<string> towns = new List<string>();

            string query = @"SELECT t.Name 
                             FROM Towns as t
                             JOIN Countries AS c ON c.Id = t.CountryCode
                             WHERE c.Name = @countryName";

            SqlCommand command = new SqlCommand(query, sqlConnection);
            command.Parameters.AddWithValue("@countryName", country);

            using SqlDataReader townsReader = command.ExecuteReader();

            while (townsReader.Read())
            {
                towns.Add((string)townsReader["Name"]);
            }

            return towns.ToArray();
        }
    }
}
