using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Catan.Source.Content;


namespace Catan.Source.Scenes
{
    internal enum ConfigState //para tela de seleceao e confirmacao
    {
        Selecting,
        Confirming,
    }

    internal class MatchPlayer // representa o jogador (placeholder)
    {
        public string Name { get; set; } = string.Empty;
        public bool IsAi { get; set; }
    }

    internal class MatchSettings //configuracoes basicas da partida a serem passadas a diante
    {
        public List<MatchPlayer> Players { get; set; } = new();
        public int TargetScore { get; set; }
    }

    internal class ConfigScene : Scene
    {
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;
        private Texture2D _pixel;

        private MouseState _previousMouseState;
        private ConfigState _state;

        private int _humanPlayers;
        private int _targetScore;

        private Rectangle _humanMinusButton;
        private Rectangle _humanPlusButton;
        private Rectangle _scoreMinusButton;
        private Rectangle _scorePlusButton;
        private Rectangle _nextButton;
        private Rectangle _backButton;
        private Rectangle _confirmButton;

        private MatchSettings? _createdSettings;

        public ConfigScene()
        {
            _spriteBatch = null!;
            _font = null!;
            _pixel = null!;
            _state = ConfigState.Selecting;
            _humanPlayers = 1;
            _targetScore = 10;
        }

        public override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(Game1.GraphicsDeviceInstance);
            _font = Game1.ContentManager.Load<SpriteFont>("defaultFont");

            _pixel = new Texture2D(Game1.GraphicsDeviceInstance, 1, 1);
            _pixel.SetData(new[] { Color.White });

            _humanMinusButton = new Rectangle(420, 150, 40, 40);
            _humanPlusButton = new Rectangle(620, 150, 40, 40);
            _scoreMinusButton = new Rectangle(420, 230, 40, 40);
            _scorePlusButton = new Rectangle(620, 230, 40, 40);
            _nextButton = new Rectangle(420, 330, 240, 50);
            _backButton = new Rectangle(420, 330, 110, 50);
            _confirmButton = new Rectangle(550, 330, 110, 50);

            base.LoadContent();
        }

        public override void UnloadContent()
        {
            _pixel?.Dispose();
            _pixel = null!;

            _spriteBatch?.Dispose();
            _spriteBatch = null!;

            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            var mouse = Mouse.GetState();

            if (IsLeftMouseJustClicked(mouse, _previousMouseState))
            {
                HandleClick(mouse.Position);
            }

            _previousMouseState = mouse;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            if (_state == ConfigState.Selecting)
            {
                DrawSelecting();
            }
            else
            {
                DrawConfirming();
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void HandleClick(Point mousePosition)
        {
            if (_state == ConfigState.Selecting)
            {
                if (_humanMinusButton.Contains(mousePosition))
                {
                    _humanPlayers = System.Math.Max(0, _humanPlayers - 1);
                }
                else if (_humanPlusButton.Contains(mousePosition))
                {
                    _humanPlayers = System.Math.Min(4, _humanPlayers + 1);
                }
                else if (_scoreMinusButton.Contains(mousePosition))
                {
                    _targetScore = System.Math.Max(1, _targetScore - 1);
                }
                else if (_scorePlusButton.Contains(mousePosition))
                {
                    _targetScore = System.Math.Min(50, _targetScore + 1);
                }
                else if (_nextButton.Contains(mousePosition))
                {
                    _createdSettings = BuildMatchSettings();
                    _state = ConfigState.Confirming;
                }

                return;
            }

            if (_backButton.Contains(mousePosition))
            {
                _state = ConfigState.Selecting;
                return;
            }

            if (_confirmButton.Contains(mousePosition))
            {
                _createdSettings ??= BuildMatchSettings();
                Game1.ChangeScene(new GameScene());
            }

        }

        private void DrawSelecting()
        {
            _spriteBatch.DrawString(_font, "Configuração de Partida", new Vector2(40, 40), Color.White);

            _spriteBatch.DrawString(_font, "Jogadores reais (0 a 4):", new Vector2(40, 155), Color.White);
            _spriteBatch.DrawString(_font, _humanPlayers.ToString(), new Vector2(530, 155), Color.White);
            DrawButton(_humanMinusButton, "-");
            DrawButton(_humanPlusButton, "+");

            _spriteBatch.DrawString(_font, "Pontuação alvo:", new Vector2(40, 235), Color.White);
            _spriteBatch.DrawString(_font, _targetScore.ToString(), new Vector2(530, 235), Color.White);
            DrawButton(_scoreMinusButton, "-");
            DrawButton(_scorePlusButton, "+");

            DrawButton(_nextButton, "Next");
        }

        private void DrawConfirming()
        {
            _createdSettings ??= BuildMatchSettings();

            _spriteBatch.DrawString(_font, "Confirmar Configuração", new Vector2(40, 40), Color.White);
            _spriteBatch.DrawString(
                _font,
                $"Pontuação alvo: {_createdSettings.TargetScore}",
                new Vector2(40, 110),
                Color.White
            );

            _spriteBatch.DrawString(_font, "Participantes:", new Vector2(40, 160), Color.White);

            for (var i = 0; i < _createdSettings.Players.Count; i++)
            {
                var player = _createdSettings.Players[i];
                var kind = player.IsAi ? "IA" : "Humano";
                _spriteBatch.DrawString(
                    _font,
                    $"{i + 1}. {player.Name} ({kind})",
                    new Vector2(60, 200 + (i * 35)),
                    Color.White
                );
            }

            DrawButton(_backButton, "Voltar");
            DrawButton(_confirmButton, "Confirmar");
        }

        private MatchSettings BuildMatchSettings()
        {
            // preenhce o resto com IA até completar 4
            var settings = new MatchSettings
            {
                TargetScore = _targetScore,
            };

            for (var i = 0; i < 4; i++)
            {
                var isHuman = i < _humanPlayers;
                settings.Players.Add(new MatchPlayer
                {
                    Name = isHuman ? $"Jogador {i + 1}" : "IA",
                    IsAi = !isHuman,
                });
            }

            return settings;
        }

        private void DrawButton(Rectangle rectangle, string text)
        {
            _spriteBatch.Draw(_pixel, rectangle, Color.DimGray);

            var textSize = _font.MeasureString(text);
            var textPosition = new Vector2(
                rectangle.X + (rectangle.Width - textSize.X) / 2f,
                rectangle.Y + (rectangle.Height - textSize.Y) / 2f
            );

            _spriteBatch.DrawString(_font, text, textPosition, Color.White);
        }

        private static bool IsLeftMouseJustClicked(MouseState current, MouseState previous)
        {
            return current.LeftButton == ButtonState.Pressed && previous.LeftButton == ButtonState.Released;
        }
    }
}
