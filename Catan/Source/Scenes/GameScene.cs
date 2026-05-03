using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Catan.Source.Content;
using Catan.Source.Game.Board;


namespace Catan.Source.Scenes
{
    internal class GameScene : Scene
    {
        private SpriteBatch _spriteBatch;
        private Atlas _atlas;

        private Board _board;

        public GameScene()
        {
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(Game1.GraphicsDeviceInstance);
            _atlas = new Atlas(Game1.ContentManager);
            _board = new(0, 0, _atlas);

            base.LoadContent();
        }

        public override void UnloadContent()
        {
            _spriteBatch?.Dispose();
            _spriteBatch = null;

            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.F1))//atalho temporario para tela de fim
            {
                Game1.ChangeScene(new EndGameScene());
                return;
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            _board.Draw(gameTime, _spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
