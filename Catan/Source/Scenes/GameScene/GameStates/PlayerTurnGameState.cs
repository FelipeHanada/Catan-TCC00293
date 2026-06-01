using Catan.Source.Content;
using Catan.Source.Game;
using Catan.Source.Game.Player;
using System;

namespace Catan.Source.Scenes.Game
{
    public class PlayerTurnGameState : GameState
    {
        public Player Player { get; }
        public PlayerHud Hud { get; private set; }
        public Button BuildButton { get; private set; }
        public Button TradeButton { get; private set; }

        public PlayerTurnGameState(GameScene gameScene, Player player)
            : base(gameScene)
        {
            Player = player;
            Hud = new PlayerHud(gameScene.Atlas, Player);

            Action EnableBuildSlate = () => {

            };

            Action EnableTradeSlate = () => {

            };

            BuildButton = new ButtonAction(700, 650, gameScene.Atlas, EnableBuildSlate, "Construir");
            TradeButton = new ButtonAction(600, 650, gameScene.Atlas, EnableTradeSlate, "Trocar");
        }
        public override void LoadContent()
        {
            base.LoadContent();
            _gameScene.Subscribe(Hud);
            _gameScene.Subscribe(BuildButton);
            _gameScene.Subscribe(TradeButton);
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            _gameScene.Unsubscribe(Hud);           
        }
    }
}
