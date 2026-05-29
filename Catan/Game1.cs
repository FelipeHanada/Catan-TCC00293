using Catan.Source.Scenes;
using Catan.Source.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Catan
{
    public class Game1 : Game
    {
        private static Game1 _instance;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public static GraphicsDevice GraphicsDeviceInstance => Instance().GraphicsDevice;
        public static ContentManager ContentManager => Instance().Content;

        private Scene _currentScene;
        private Scene? _nextScene;

        private Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _currentScene = new MainMenuScene();
            _nextScene = null;
        }

        public static Game1 Instance()
        {
            if (_instance == null)
            {
                _instance = new Game1();
            }
            return _instance;
        }

        protected override void Initialize()
        {
            base.Initialize();
            _currentScene.Initialize();
            MusicManager.Instance.Play(_currentScene.Music);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            SoundManager.Instance.LoadContent(Content);
            MusicManager.Instance.LoadContent(Content);
            MusicManager.Instance.Play(_currentScene.Music);
        }


        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _currentScene.Update(gameTime);
            TransitionScene();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, null);

            GraphicsDevice.Clear(Color.CornflowerBlue);

            _currentScene.Draw(gameTime, _spriteBatch);

            base.Draw(gameTime);

            _spriteBatch.End();
        }

        public static void ChangeScene(Scene newScene)
        {
            var instance = Instance();
            instance._nextScene = newScene;
        }

        public static void TransitionScene()
        {
            var instance = Instance();
            if (instance._nextScene != null)
            {
                instance._currentScene.Dispose();
                instance._currentScene = instance._nextScene;
                instance._nextScene = null;
                instance._currentScene.Initialize();
                MusicManager.Instance.Play(instance._currentScene.Music);
            }
        }
    }
}
