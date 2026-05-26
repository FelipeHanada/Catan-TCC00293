using System;
using Catan.Source.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GamePlayer = Catan.Source.Game.Player.Player;
using Catan.Source.Scenes;
using Catan.Source.Scenes.Game;

namespace Catan.Source.Game.Board
{
    public class TileVertex : GameObject
    {
        private const int CircleDiameter = 8;
        private const float Radius = CircleDiameter / 2f;

        private static Texture2D? _circleTexture;
        private static readonly GamePlayer _placeholderPlayer = new(0);
        private MouseState _previousMouseState;

        private readonly Atlas atlas;
        private readonly GameScene gameScene;
        public Building Building { get; private set; }
        public bool HasBuilding => Building != null;

        public TileVertex(float x, float y, Atlas atlas, GameScene gameScene) 
            : base(x, y)
        {
            this.atlas = atlas;
            this.gameScene = gameScene;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            EnsureTextureGenerated();

            var position = new Vector2(X, Y);
            var origin = new Vector2(Radius);
            
            spriteBatch.Draw(_circleTexture, position, null, Color.White, 0f, origin, 1f, SpriteEffects.None, 0f);

            if (Building != null)
            {
                AtlasPlayerSprite sprite = Building.Type == BuildingType.City
                    ? AtlasPlayerSprite.City
                    : AtlasPlayerSprite.Settlement;

                Rectangle rectangle = Atlas.GetRectangle(sprite, Building.Owner.PlayerNumber);
                spriteBatch.Draw(
                    atlas.Texture,
                    position - new Vector2(rectangle.Width / 2, rectangle.Height / 2),
                    rectangle,
                    Color.White
                );
            }
        }

        public override void Update(GameTime gameTime)
        {
            MouseState currentMouseState = Mouse.GetState();
            
            if (gameScene.GetCurrentStateGame() is PositionSettlementGameState gameState)
            {
                if (IsHovering(currentMouseState) && (currentMouseState.LeftButton == ButtonState.Pressed && 
                    _previousMouseState.LeftButton == ButtonState.Released))
                {
                    if (gameState.CanPlaceBuilding(this))
                    {
                        PlaceBuilding(new Building(gameState.Player, gameState.BuildingType));
                        gameScene.ExitState();

                        SoundManager.Instance.Play(SfxId.ConstrucaoCasa);
                    } else
                    {
                        SoundManager.Instance.Play(SfxId.TijoloCaindo);
                    }
                }
            }

            _previousMouseState = currentMouseState;
        }

        public void PlaceBuilding(Building building)
        {
            Building = building;
        }

        private bool IsHovering(MouseState mouseState)
        {
            float deltaX = mouseState.X - X;
            float deltaY = mouseState.Y - Y;
            float distanceSquared = (deltaX * deltaX) + (deltaY * deltaY);
            return distanceSquared <= (Radius * Radius * 16);
        }

        private static void EnsureTextureGenerated()
        {
            if (_circleTexture != null) return;

            _circleTexture = new Texture2D(Game1.GraphicsDeviceInstance, CircleDiameter, CircleDiameter);
            
            var pixels = new Color[CircleDiameter * CircleDiameter];
            var radiusSq = Radius * Radius;
            var center = new Vector2(Radius, Radius);

            for (int y = 0; y < CircleDiameter; y++)
            {
                for (int x = 0; x < CircleDiameter; x++)
                {
                    var offset = new Vector2(x, y) - center;
                    
                    pixels[y * CircleDiameter + x] = offset.LengthSquared() <= radiusSq
                        ? Color.Red
                        : Color.Transparent;
                }
            }

            _circleTexture.SetData(pixels);
        }
    }
}
