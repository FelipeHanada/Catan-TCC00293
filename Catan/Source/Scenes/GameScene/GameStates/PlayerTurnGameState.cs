using Catan.Source.Game.Player;

namespace Catan.Source.Scenes.Game
{
    public class PlayerTurnGameState : GameState
    {
        public Player Player { get; }
        public PlayerHud Hud { get; private set; }

        public PlayerTurnGameState(GameScene gameScene, Player player)
            : base(gameScene)
        {
            Player = player;
            Hud = new PlayerHud(gameScene.Atlas, Player);
            AddChild(Hud);
        }
    }
}
