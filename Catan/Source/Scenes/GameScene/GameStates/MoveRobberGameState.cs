using DiagnosticsDebug = System.Diagnostics.Debug;
using Catan.Source.Game.Board;
using Microsoft.Xna.Framework;

namespace Catan.Source.Scenes.Game
{
    public class MoveRobberGameState : GameState
    {
        public MoveRobberGameState(GameScene gameScene)
            : base(gameScene)
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Tile destinationTile = GetPlaceholderDestinationTile();
            if (destinationTile != null && _gameScene.Board.MoveRobberTo(destinationTile))
            {
                #if DEBUG
                DiagnosticsDebug.WriteLine("Ladrão: movido para um novo tile válido.");
                #endif
            }

            _gameScene.ExitState();
        }

        private Tile GetPlaceholderDestinationTile()
        {
            // Depois o jogador vai escolher para qual tile mover o ladrão, por enquanto vai para o primeiro tile válido encontrado
            foreach (Tile tile in _gameScene.Board.Tiles)
            {
                if (_gameScene.Board.CanMoveRobberTo(tile))
                {
                    return tile;
                }
            }

            return null;
        }
    }
}
