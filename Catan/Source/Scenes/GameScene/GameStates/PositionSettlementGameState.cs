using Catan.Source.Game.Board;
using Catan.Source.Game.Player;
using Microsoft.Xna.Framework;

namespace Catan.Source.Scenes.Game
{
    public class PositionSettlementGameState : GameState
    {
        public Player Player { get; private set; }
        public BuildingType BuildingType { get; private set; }
        public PositionSettlementGameState(GameScene gameScene, Player player, BuildingType buildingType)
            : base(gameScene)
        {
            Player = player;
            BuildingType = buildingType;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
