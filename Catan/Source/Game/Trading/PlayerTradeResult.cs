namespace Catan.Source.Game.Trading
{
    public class PlayerTradeResult
    {
        public bool Success { get; }
        public string Message { get; }

        private PlayerTradeResult(bool success, string message)
        {
            Success = success;
            Message = message; // depois da pra mudar esse resultado, talvez colocar explicabilade aqui, mas acredito que não vai dar tempo pra isso
        }

        public static PlayerTradeResult Ok(string message)
        {
            return new PlayerTradeResult(true, message);
        }

        public static PlayerTradeResult Fail(string message)
        {
            return new PlayerTradeResult(false, message);
        }
    }
}
