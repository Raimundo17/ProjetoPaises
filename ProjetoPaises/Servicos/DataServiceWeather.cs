namespace ProjetoPaises.Servicos
{
    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.IO;
    using ProjetoPaises.Modelos;

    public class DataServiceWeather
    {
        private SQLiteConnection connection; // atributo para fazer a ligação á base de dados

        private SQLiteCommand command;

        private DialogService dialogService; // para comunicar com o utilizador caso algo aconteca

        public DataServiceWeather()
        {
            // vou criar na raiz uma pasta chamada data e vai ser nessa pasta que vou colocar a base de dados; se a pasta nao existir ele vai criar
            dialogService = new DialogService();

            if (!Directory.Exists("DataWeather")) //se a pasta "Data" nao estiver criada
            {
                Directory.CreateDirectory("DataWeather"); // cria a pasta "Data"
            }

            var path = @"DataWeather\weathers.sqlite"; // caminho para a base de dados

            try
            {
                connection = new SQLiteConnection("DataSource=" + path); // Connection String á base de dados
                connection.Open(); // abre a conecção ou cria a base de dados caso ela nao exista

                string sqlcommand =
                    "create table if not exists weathers(Id int, Name varchar (50), Temperature int, Wind int, Humidity int, Precipitation int)"; // comando em sql para criar as tabelas

                command = new SQLiteCommand(sqlcommand, connection);

                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error", e.Message);
            }
        }

        //quando ele vai a api coloca os dados na base de dados
        public void SaveData(List<Weather> Weathers)
        {
            try
            {
                foreach (var weather in Weathers)
                {
                    string sql =
                        string.Format("insert into Weathers (Id, Name, Temperature, Wind, Humidity, Precipitation) values({0}, '{1}', {2}, {3}, {4}, '{5}')",
                        weather.Id, weather.Name, weather.Temperature, weather.Wind, weather.Humidity, weather.Precipitation);

                    command = new SQLiteCommand(sql, connection);

                    command.ExecuteNonQuery(); // comando que vai executar a string
                }
                connection.Close();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error", e.Message);
            }
        }

        public List<Weather> GetData()
        {
            List<Weather> weathers = new List<Weather>();

            try
            {
                string sql = "select Id, Name, Temperature, Wind, Humidity, Precipitation from Weathers";

                command = new SQLiteCommand(sql, connection);

                //Lê cada registo
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    weathers.Add(new Weather
                    {
                        Id = (int)reader["Id"],
                        Name = (string)reader["Name"],
                        Temperature = (int)reader["Temperature"],
                        Wind = (int)reader["Wind"],
                        Humidity = (int)reader["Humidity"],
                        Precipitation = (int)reader["Precipitation"],
                    });
                }
                connection.Close();
                return weathers;
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error", e.Message);
                return null;
            }
        }

        public void DeleteData()
        {
            try
            {
                string sql = "Delete from Weathers";

                command = new SQLiteCommand(sql, connection);

                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error", e.Message);
            }
        }
    }
}
