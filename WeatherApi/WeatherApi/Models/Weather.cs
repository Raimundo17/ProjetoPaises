using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WeatherApi.Controllers;

namespace WeatherApi.Models
{
    public class Weather
    {

        private static List<Country> countries;

        public static List<Country> Countries 
        {
            get 
            {
                if(countries == null)
                {
                    CreateCountries(); // gerar paises para dentro da lista
                }

                return countries;
            }
            set {countries =  countries = value;}
            } 
        
        private static void CreateCountries()
        {
            countries = new List<Country>();

            countries.Add(new Country
            {
                Id = 1,
                Name = "Canada",
                Temperature = 15,
                Wind = 18,
                Humidity = 12,
                Precipitation = 50
            });

            countries.Add(new Country
            {
                Id = 2,
                Name = "Japan",
                Temperature = 15,
                Wind = 18,
                Humidity = 12,
                Precipitation = 50
            });

            countries.Add(new Country
            {
                Id = 3,
                Name = "Angola",
                Temperature = 15,
                Wind = 18,
                Humidity = 12,
                Precipitation = 50
            });
        }
    }
}