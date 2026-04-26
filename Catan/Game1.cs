using Catan.Source.Scenes;
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
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _currentScene = new ConfigScene(); //mudei aqui para testar a cena de configuração
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
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
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
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _currentScene.Draw(gameTime);

            base.Draw(gameTime);
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
            }
        }
    }
}
