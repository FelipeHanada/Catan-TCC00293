using Catan.Source.Game.Inventory;

namespace Catan.Source.Game.DevelopmentCards
{
    public class DevelopmentCardPurchaseResult
    {
        public bool Success { get; }
        public string Message { get; }
        public DevelopmentCard Card { get; }

        private DevelopmentCardPurchaseResult(bool success, string message, DevelopmentCard card)
        {
            Success = success;
            Message = message;
            Card = card;
        }

        public static DevelopmentCardPurchaseResult Ok(string message, DevelopmentCard card = null)
        {
            return new DevelopmentCardPurchaseResult(true, message, card);
        }

        public static DevelopmentCardPurchaseResult Fail(string message)
        {
            return new DevelopmentCardPurchaseResult(false, message, null);
        }
    }
}
