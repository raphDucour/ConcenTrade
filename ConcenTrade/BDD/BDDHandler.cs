using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Concentrade.BDD
{
    internal class BDDHandler
    {
        private readonly string connectionString;

        public BDDHandler()
        {
            // ⚠️ Change les paramètres selon ta config
            connectionString = "server=localhost;user=root;password=volcy2004;database=concenTradeDataBase;port=3306;";
        }

        // Méthode générique pour exécuter une requête INSERT, UPDATE ou DELETE
        public int ExecuteNonQuery(string query, params MySqlParameter[] parameters)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddRange(parameters);
            return cmd.ExecuteNonQuery(); // Renvoie le nombre de lignes affectées
        }

        // Méthode générique pour exécuter une requête SELECT (une valeur)
        public object ExecuteScalar(string query, params MySqlParameter[] parameters)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddRange(parameters);
            return cmd.ExecuteScalar(); // Renvoie une seule valeur
        }

        // Méthode générique pour récupérer un tableau de résultats
        public DataTable ExecuteQuery(string query, params MySqlParameter[] parameters)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddRange(parameters);
            using var adapter = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }
    }
}
