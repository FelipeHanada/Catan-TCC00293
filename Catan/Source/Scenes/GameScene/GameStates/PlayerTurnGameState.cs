using Catan.Source.Content;
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
        }
        public override void LoadContent()
        {
            base.LoadContent();
            _gameScene.Subscribe(Hud);
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            _gameScene.Unsubscribe(Hud);           
        }
    }
}
