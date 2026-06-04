namespace Catan.Source.Game.DevelopmentCards
{
    public class DevelopmentCardActivationResult
    {
        public bool Success { get; }
        public string Message { get; }

        private DevelopmentCardActivationResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public static DevelopmentCardActivationResult Ok(string message)
        {
            return new DevelopmentCardActivationResult(true, message);
        }

        public static DevelopmentCardActivationResult Fail(string message)
        {
            return new DevelopmentCardActivationResult(false, message);
        }
    }
}
