namespace ProjetoPaises.Modelos
{
    public class Response // modelo de dados que vai tratar da resposta
    {
        public bool IsSuccess { get; set; } //verifica se existe internet ; se a API carregou; se os dados foram carrgados para a base de dados

        public string Message { get; set; } // caso as coisas corram mal o que é que se passou, guardo a mensagem vinda da api e faco uma personalizada 

        public object Result { get; set; } // caso corra tudo bem guardo um objeto chamado result (é object porque assim funciona para qualquer tipo de objeto que receba)
    }
}
