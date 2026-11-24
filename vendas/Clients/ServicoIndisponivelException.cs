namespace Vendas.Api.Clients
{
    // Lançada quando outro serviço não responde (fora do ar, timeout, erro 5xx)    
    public class ServicoIndisponivelException : Exception
    {
        public ServicoIndisponivelException(string servico, Exception? causa = null)
        : base($"O serviço de {servico} está indisponível no momento.", causa) { }
    }
}
