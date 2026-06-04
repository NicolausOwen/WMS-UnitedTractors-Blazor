using System;
using MySqlConnector;

class Program
{
    static void Main()
    {
        string connStr = "Server=localhost;Port=3306;Database=ut_wms_db;User=root;Password=;";
        try {
            using var conn = new MySqlConnection(connStr);
            conn.Open();
            using var cmd = new MySqlCommand("SHOW TABLES", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine(reader.GetString(0));
            }
        } catch (Exception e) {
            Console.WriteLine(e.Message);
        }
    }
}
