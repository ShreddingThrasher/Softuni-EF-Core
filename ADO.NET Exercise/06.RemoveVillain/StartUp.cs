using System;
using System.Data.SqlClient;

namespace _06.RemoveVillain
{
    class StartUp
    {
        static void Main(string[] args)
        {
            const string _connectionString =
                @"Server=.;Database=MinionsDB;Integrated Security=True;";

            int villainId = int.Parse(Console.ReadLine());

            using SqlConnection sqlConnection =
                new SqlConnection(_connectionString);
            sqlConnection.Open();

            string villainName = CheckVillain(sqlConnection, villainId);


            if(villainName == null)
            {
                Console.WriteLine("No such villain was found.");
                return;
            }

            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();

            try
            {
                int releasedMinions = DeleteFromMinionsVillains(sqlConnection, sqlTransaction, villainId);
                DeleteVillain(sqlConnection, sqlTransaction, villainId);

                sqlTransaction.Commit();

                Console.WriteLine($"{villainName} was deleted.");
                Console.WriteLine($"{releasedMinions} minions were released.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                sqlTransaction.Rollback();
            }

            sqlConnection.Close();
        }

        /// <summary>
        /// Using an Open Sql Connection gets a villain name by Id from the database.
        /// </summary>
        /// <param name="sqlConnection">Open Sql Connecation</param>
        /// <param name="villainId">Villain Id</param>
        /// <returns></returns>
        static string CheckVillain(SqlConnection sqlConnection, int villainId)
        {
            string query = @"SELECT Name FROM Villains WHERE Id = @villainId";

            SqlCommand command = new SqlCommand(query, sqlConnection);
            command.Parameters.AddWithValue("@villainId", villainId);

            return (string)command.ExecuteScalar();
        }

        /// <summary>
        /// Using an open Sql Connection deletes records in MinionsVillains table by a given villain Id.
        /// Returns the count of deleted records (released minions).
        /// </summary>
        /// <param name="sqlConnection">Open Sql Connection</param>
        /// <param name="villainId">Villain Id</param>
        /// <returns></returns>
        static int DeleteFromMinionsVillains(SqlConnection sqlConnection, SqlTransaction sqlTransaction, int villainId)
        {
            string query = @"DELETE FROM MinionsVillains 
                             WHERE VillainId = @villainId";

            SqlCommand command = new SqlCommand(query, sqlConnection);
            command.Parameters.AddWithValue("@villainId", villainId);

            command.Transaction = sqlTransaction;

            return command.ExecuteNonQuery();
        }

        /// <summary>
        /// Using an open Sql Connection deletes a villain from the database.
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <param name="villainId"></param>
        static void DeleteVillain(SqlConnection sqlConnection, SqlTransaction sqlTransaction, int villainId)
        {
            string query = @"DELETE FROM Villains
                             WHERE Id = @villainId";

            SqlCommand command = new SqlCommand(query, sqlConnection);
            command.Parameters.AddWithValue("@villainId", villainId);
            command.Transaction = sqlTransaction;

            command.ExecuteNonQuery();
        }
    }
}
