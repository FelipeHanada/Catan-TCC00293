using Microsoft.Xna.Framework;
using Catan;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Catan.Source.Scenes
{
    internal class GameScene : Scene
    {
        private Texture2D _atlas;
        private SpriteBatch _spriteBatch;

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
            _atlas = Game1.ContentManager.Load<Texture2D>("catan-atlas");

            base.LoadContent();
        }

        public override void UnloadContent()
        {
            _spriteBatch?.Dispose();
            _spriteBatch = null;
            _atlas = null;

            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(_atlas, Vector2.Zero, Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
