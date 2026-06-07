namespace Catan.Source.Game.AI
{
    public enum AiTurnMode
    {
        Auto,
        Manual,
    }

    public static class AiTurnModeExtensions
    {
        public static bool ShouldAutoEndTurn(this AiTurnMode mode)
        {
            return mode == AiTurnMode.Auto;
        }
    }

    public static class AiTurnInputPolicy
    {
        public static bool CanUseManualActionButtons(bool isAiPlayerActionsState)
        {
            return !isAiPlayerActionsState;
        }

        public static bool CanEndTurn(bool isAiPlayerActionsState, AiTurnMode mode)
        {
            return !isAiPlayerActionsState || mode == AiTurnMode.Manual;
        }

        public static bool CanOpenDevelopmentCards(bool isAiPlayerActionsState, AiTurnMode mode)
        {
            return !isAiPlayerActionsState || mode == AiTurnMode.Manual;
        }
    }
}
