namespace ProjetoPaises.Servicos
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Modelos;
    
    public class ApiService
    {
        public async Task<Response> GetCountries(string urlBase, string controller) // tarefa de devolver uma Response atraves do base adress e do controller da Api
        { // Faz com uma task para ser criar um thread e nao bloquear a aplicação enquanto carrega os países 
            try
            {
                var client = new HttpClient(); // conexão via Http
                
                client.BaseAddress = new Uri(urlBase); // endereço base da API

                var response = await client.GetAsync(controller); // vai buscar o controlador de forma assíncrona e guarda na variável response

                var result = await response.Content.ReadAsStringAsync(); // carrega os resultados no formato de uma string para o objeto result

                if (!response.IsSuccessStatusCode) // verificar se algo correu mal
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = result, // json vai devolver caso corra mal
                    };
                }

                var countries = JsonConvert.DeserializeObject<List<Country>>(result); // recebo o json e coloco numa lista do tipo Country

                return new Response
                {
                    IsSuccess = true,
                    Result = countries // vai receber um objecto que na prática é a lista dos países
                };
            }
            catch (Exception ex)
            {
                return new Response // resposta personalizada
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<Response> GetWeather(string urlBase, string controller) // WEATHER API
        { // Faz com uma task para ser criar um thread e nao bloquear a aplicação enquanto carrega os países 
            try
            {
                var client = new HttpClient(); // conexão via Http
                
                client.BaseAddress = new Uri(urlBase); // endereço base da API

                var response = await client.GetAsync(controller); // vai buscar o controlador de forma assíncrona e guarda na variável response

                var result = await response.Content.ReadAsStringAsync(); // carrega os resultados no formato de uma string para o objeto result

                if (!response.IsSuccessStatusCode) // verificar se algo correu mal
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = result, // json vai devolver caso corra mal
                    };
                }

                var weathers = JsonConvert.DeserializeObject<List<Weather>>(result); // recebo o json e coloco numa lista do tipo Country

                return new Response
                {
                    IsSuccess = true,
                    Result = weathers // vai receber um objecto que na prática é a lista dos países
                };
            }
            catch (Exception ex)
            {
                return new Response // resposta personalizada
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
    }
}
