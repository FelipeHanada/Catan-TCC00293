using Catan.Source.Game.Player;
using Microsoft.Xna.Framework;

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
            Hud = new PlayerHud(gameScene, player, 1280 - 210, 10, Color.White, 200, 170);
            AddChild(Hud);
        }
    }
}
