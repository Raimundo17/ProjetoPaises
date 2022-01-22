namespace ProjetoPaises.Modelos
{
    public class Weather
    {
        public int Id { get; set; } // A informação é distinguida por id porque podemos ter informações diferentes no mesmo país

        public string Name { get; set; }

        public int Temperature { get; set; }

        public int Wind { get; set; }

        public int Humidity { get; set; }

        public int Precipitation { get; set; }

        public string DisplayName
        {
            get
            {
                return $"{Name}";
            }
        }
    }
}
