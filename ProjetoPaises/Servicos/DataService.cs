namespace ProjetoPaises.Servicos
{
    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.IO;
    using ProjetoPaises.Modelos;
    
    public class DataService
    {
        private SQLiteConnection connection; // atributo para fazer a ligação á base de dados

        private SQLiteCommand command;

        private DialogService dialogService; // para comunicar com o utilizador caso algo aconteca

        public DataService()
        {
            // vou criar na raiz uma pasta chamada data e vai ser nessa pasta que vou colocar a base de dados; se a pasta nao existir ele vai criar
            dialogService = new DialogService();

            if (!Directory.Exists("Data")) //se a pasta "Data" nao estiver criada
            {
                Directory.CreateDirectory("Data"); // cria a pasta "Data"
            }

            var path = @"Data\Countries.sqlite"; // caminho para a base de dados

            try
            {
                connection = new SQLiteConnection("DataSource=" + path); // Connection String á base de dados
                
                connection.Open(); // abre a conecção ou cria a base de dados caso ela nao exista

                string sqlcommand =
                    "create table if not exists countries(Name varchar (50), Capital varchar (50), Region varchar (50), Subregion varchar (50), Population int, Gini varchar (50), Demonym varchar (50), NativeName varchar (50))"; // comando em sql para criar as tabelas

                command = new SQLiteCommand(sqlcommand, connection);

                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error", e.Message);
            }
        }

        //quando ele vai á api coloca os dados na base de dados
        public void SaveData(List<Country> Countries)
        {
            try
            {
                foreach (var country in Countries)
                {
                    string sql =
                        string.Format("insert into Countries (Name, Capital, Region, Subregion, Population, Gini, Demonym, NativeName) values('{0}', '{1}', '{2}', '{3}', {4}, '{5}', '{6}', '{7}')",
                        country.Name.Replace("'","''"), country.Capital.Replace("'", "''"), country.Region.Replace("'", "''"), country.Subregion.Replace("'", "''"), country.Population, country.Gini, country.Demonym.Replace("'", "''"), country.NativeName.Replace("'", "''"));

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

        public List<Country> GetData()
        {
            List<Country> countries = new List<Country>();

            try
            {
                string sql = "select Name, Capital, Region, Subregion, Population, Gini, Demonym, NativeName from Countries";

                command = new SQLiteCommand(sql, connection);

                //Lê cada registo
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    countries.Add(new Country
                    {
                        Name = (string)reader["Name"],
                        Capital = (string)reader["Capital"],
                        Region = (string)reader["Region"],
                        Subregion = (string)reader["Subregion"],
                        Population = (int)reader["Population"],
                        Gini = (string)reader["Gini"],
                        Demonym = (string)reader["Demonym"],
                        NativeName = (string)reader["NativeName"]
                    });
                }
                connection.Close();
                return countries;
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
                string sql = "Delete from Countries";

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
