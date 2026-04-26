using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Catan.Source.Content;


namespace Catan.Source.Scenes
{
    internal class GameScene : Scene
    {
        private SpriteBatch _spriteBatch;
        private Atlas _atlas;

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
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(_atlas.Texture, Vector2.Zero, Atlas.GetRectangle(AtlasSpriteId.TileForest), Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
