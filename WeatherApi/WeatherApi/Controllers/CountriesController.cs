using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WeatherApi.Models;

namespace WeatherApi.Controllers
{
    public class CountriesController : ApiController
    {
        DataClasses1DataContext dc = new DataClasses1DataContext();

        // GET: api/Countries
        public List<Country> Get()
        {
            var list = from Country in dc.Countries select Country;

            return list.ToList();
        }

        // GET: api/Countries/5
        /// <summary>
        /// Weather Information from a country
        /// </summary>
        /// <param name="id"></param>
        /// <returns>return countries</returns>
        public IHttpActionResult Get(int id) // retorna um resultado da resposta da API
        {
            var countries = dc.Countries.SingleOrDefault(c => c.Id == id); // vai ao weather procurar o país que quero, o primeiro que encontrar e se a condicao se verificar retorno o país

            if (countries != null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, countries));
            }

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Country dont exist"));
        }

        // POST: api/Countries // inserir países na tabela
        /// <summary>
        /// Regist of a new country weather information
        /// </summary>
        /// <param name="newCountry"></param>
        /// <returns>Country</returns>
        public IHttpActionResult Post([FromBody] Country newCountry) // recebe um objeto do tipo Country 
        {
            Country country = dc.Countries.FirstOrDefault(c => c.Id == newCountry.Id);

            if (country != null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict,
                    "There is already a country registed with that Id"));
            }

            dc.Countries.InsertOnSubmit(newCountry); //vou á lista adicionar o país que recebo

            try
            {
                dc.SubmitChanges();
            }
            catch (Exception e)
            {

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.ServiceUnavailable, e));
            }

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK));
        }

        // PUT: api/Countries/5
        /// <summary>
        /// Change the weather information of a Country
        /// </summary>
        /// <param name="changedCountry"></param>
        /// <returns>Country</returns>
        public IHttpActionResult Put([FromBody] Country changedCountry)
        {
            Country country = dc.Countries.FirstOrDefault(c => c.Id == changedCountry.Id); // verificar se o país com aquele id existe ou não

            if (country == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, 
                    "There is no country with that id to be changed"));
            }

            country.Name = changedCountry.Name;
            country.Temperature = changedCountry.Temperature;
            country.Wind = changedCountry.Wind;
            country.Humidity = changedCountry.Humidity;
            country.Precipitation = changedCountry.Precipitation;
            
            try
            {
                dc.SubmitChanges();
            }
            catch (Exception e)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.ServiceUnavailable, e));
            }

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK));
        }

        // DELETE: api/Countries/5
        public IHttpActionResult Delete(int id)
        {
            Country country = dc.Countries.FirstOrDefault(c => c.Id == id);

            if (country != null)
            {
                dc.Countries.DeleteOnSubmit(country);

                try
                {
                    dc.SubmitChanges();
                }
                catch (Exception e)
                {

                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.ServiceUnavailable, e));
                }

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK));

            }

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "There is no country with that Id to be deleted"));
        }
    }
}
