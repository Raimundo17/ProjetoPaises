namespace ProjetoPaises.Servicos
{
    using System.Net;
    using Modelos;
    
    public class NetworkService // este serviço vai disponibilizar uma ligação á internet
    {
        public Response CheckConnection()
        {
            var client = new WebClient();

            try
            {
                using (client.OpenRead("http://clients3.google.com/generate_204")) // verificar se existe ligacao á internet através de um ping ao servidor da google
                {
                    return new Response
                    {
                        IsSuccess = true, // caso haja conexao devolve um objeto do tipo response 
                    };
                }
            }
            catch 
            {
                return new Response // caso nao haja conexao devolve um objeto do tipo response / Sempre baseado com o que foi definido na classe Response
                {
                    IsSuccess = false,
                    Message = "Check your Internet Connection..." 
                };
            }
        }
    }
}
