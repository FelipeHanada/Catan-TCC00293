using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Catan.Source.Content
{
    public enum AtlasSpriteId
    {
        TileForest, TileSheeps, TileBricks, TileMountains, TileDesert, TileFarms,

        SettlementPlayer1, CityPlayer1, Road1Player1, Road2Player1, Road3Player1,
        SettlementPlayer2, CityPlayer2, Road1Player2, Road2Player2, Road3Player2,
        SettlementPlayer3, CityPlayer3, Road1Player3, Road2Player3, Road3Player3,
        SettlementPlayer4, CityPlayer4, Road1Player4, Road2Player4, Road3Player4,

        DevCardInvention, DevCardRoad, DevCardMonopoly, DevCardKnight, DevCardVP,
    }

    public enum AtlasPlayerSprite
    {
        Settlement, City, Road1, Road2, Road3,
    }

    public class Atlas
    {
        private Texture2D _texture;
        private static readonly Dictionary<AtlasSpriteId, Rectangle> _spriteRectangles = new Dictionary<AtlasSpriteId, Rectangle>
        {
            [AtlasSpriteId.TileForest] = new Rectangle(0, 0, 128, 128),
            [AtlasSpriteId.TileSheeps] = new Rectangle(64, 0, 64, 64),
            [AtlasSpriteId.TileBricks] = new Rectangle(128, 0, 64, 64),
            [AtlasSpriteId.TileMountains] = new Rectangle(192, 0, 64, 64),
            [AtlasSpriteId.TileDesert] = new Rectangle(256, 0, 64, 64),
            [AtlasSpriteId.TileFarms] = new Rectangle(320, 0, 64, 64),
            [AtlasSpriteId.SettlementPlayer1] = new Rectangle(0, 64, 32, 32),
            [AtlasSpriteId.CityPlayer1] = new Rectangle(32, 64, 32, 32),
            [AtlasSpriteId.Road1Player1] = new Rectangle(64, 64, 48, 16),
            [AtlasSpriteId.Road2Player1] = new Rectangle(64, 80, 48, 16),
            [AtlasSpriteId.Road3Player1] = new Rectangle(64, 96, 48, 16),
            [AtlasSpriteId.SettlementPlayer2] = new Rectangle(0, 128, 32, 32),
            [AtlasSpriteId.CityPlayer2] = new Rectangle(32, 128, 32, 32),
            [AtlasSpriteId.Road1Player2] = new Rectangle(64, 128, 48, 16),
            [AtlasSpriteId.Road2Player2] = new Rectangle(64, 144, 48, 16),
            [AtlasSpriteId.Road3Player2] = new Rectangle(64, 160, 48, 16),
            [AtlasSpriteId.SettlementPlayer3] = new Rectangle(0, 192, 32, 32),
            [AtlasSpriteId.CityPlayer3] = new Rectangle(32, 192, 32, 32),
            [AtlasSpriteId.Road1Player3] = new Rectangle(64, 192, 48, 16),
            [AtlasSpriteId.Road2Player3] = new Rectangle(64, 208, 48, 16),
            [AtlasSpriteId.Road3Player3] = new Rectangle(64, 224, 48, 16),
            [AtlasSpriteId.SettlementPlayer4] = new Rectangle(0, 256, 32, 32),
            [AtlasSpriteId.CityPlayer4] = new Rectangle(32, 256, 32, 32),
            [AtlasSpriteId.Road1Player4] = new Rectangle(64, 256, 48, 16),
            [AtlasSpriteId.Road2Player4] = new Rectangle(64, 272, 48, 16),
            [AtlasSpriteId.Road3Player4] = new Rectangle(64, 288, 48, 16),
            [AtlasSpriteId.DevCardInvention] = new Rectangle(0, 320, 48, 64),
            [AtlasSpriteId.DevCardRoad] = new Rectangle(48, 320, 48, 64),
            [AtlasSpriteId.DevCardMonopoly] = new Rectangle(96, 320, 48, 64),
            [AtlasSpriteId.DevCardKnight] = new Rectangle(144, 320, 48, 64),
            [AtlasSpriteId.DevCardVP] = new Rectangle(192, 320, 48, 64),
        };
        private static readonly Dictionary<int, Dictionary<AtlasPlayerSprite, AtlasSpriteId>> _playerSpriteMappings = new Dictionary<int, Dictionary<AtlasPlayerSprite, AtlasSpriteId>>
        {
            [1] = new Dictionary<AtlasPlayerSprite, AtlasSpriteId>
            {
                [AtlasPlayerSprite.Settlement] = AtlasSpriteId.SettlementPlayer1,
                [AtlasPlayerSprite.City] = AtlasSpriteId.CityPlayer1,
                [AtlasPlayerSprite.Road1] = AtlasSpriteId.Road1Player1,
                [AtlasPlayerSprite.Road2] = AtlasSpriteId.Road2Player1,
                [AtlasPlayerSprite.Road3] = AtlasSpriteId.Road3Player1,
            },
            [2] = new Dictionary<AtlasPlayerSprite, AtlasSpriteId>
            {
                [AtlasPlayerSprite.Settlement] = AtlasSpriteId.SettlementPlayer2,
                [AtlasPlayerSprite.City] = AtlasSpriteId.CityPlayer2,
                [AtlasPlayerSprite.Road1] = AtlasSpriteId.Road1Player2,
                [AtlasPlayerSprite.Road2] = AtlasSpriteId.Road2Player2,
                [AtlasPlayerSprite.Road3] = AtlasSpriteId.Road3Player2,
            },
            [3] = new Dictionary<AtlasPlayerSprite, AtlasSpriteId>
            {
                [AtlasPlayerSprite.Settlement] = AtlasSpriteId.SettlementPlayer3,
                [AtlasPlayerSprite.City] = AtlasSpriteId.CityPlayer3,
                [AtlasPlayerSprite.Road1] = AtlasSpriteId.Road1Player3,
                [AtlasPlayerSprite.Road2] = AtlasSpriteId.Road2Player3,
                [AtlasPlayerSprite.Road3] = AtlasSpriteId.Road3Player3,
            },
            [4] = new Dictionary<AtlasPlayerSprite, AtlasSpriteId>
            {
                [AtlasPlayerSprite.Settlement] = AtlasSpriteId.SettlementPlayer4,
                [AtlasPlayerSprite.City] = AtlasSpriteId.CityPlayer4,
                [AtlasPlayerSprite.Road1] = AtlasSpriteId.Road1Player4,
                [AtlasPlayerSprite.Road2] = AtlasSpriteId.Road2Player4,
                [AtlasPlayerSprite.Road3] = AtlasSpriteId.Road3Player4,
            },
        };

        public Texture2D Texture
        {
            get
            {
                if (_texture == null)
                {
                    throw new InvalidOperationException("Atlas não foi carregado. Chame Atlas.Instance.Load(content) antes de acessar a textura.");
                }
                return _texture;
            }
        }

        public Atlas(ContentManager content)
        {
            _texture = content.Load<Texture2D>("catan-atlas");
        }

        public static Rectangle GetRectangle(AtlasSpriteId sprite)
        {
            if (_spriteRectangles.TryGetValue(sprite, out Rectangle rect))
            {
                return rect;
            }

            throw new ArgumentException($"Sprite {sprite} não encontrado no atlas.");
        }

        public static Rectangle GetRectangle(AtlasPlayerSprite sprite, int player)
        {
            if (!_playerSpriteMappings.TryGetValue(player, out var playerSprites))
            {
                throw new ArgumentOutOfRangeException(nameof(player), "Player deve ser entre 1 e 4.");
            }

            if (!playerSprites.TryGetValue(sprite, out var spriteId))
            {
                throw new ArgumentException($"Sprite de jogador {sprite} não suportado.");
            }

            return GetRectangle(spriteId);
        }
    }
}