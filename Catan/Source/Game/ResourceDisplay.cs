using Catan.Source.Content;
using Catan.Source.Game.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catan.Source.Game
{
    public class ResourceDisplay : GameObject
    {
        private Dictionary<ResourceId, int> ResourceCounts = new()
        {
            [ResourceId.Wood] = 0,
            [ResourceId.Wool] = 0,
            [ResourceId.Brick] = 0,
            [ResourceId.Ore] = 0,
            [ResourceId.Wheat] = 0,
        };
        private SpriteFont Font;
        private Atlas atlas;
        public ResourceDisplay(float x, float y, Atlas atlas) : base(x, y)
        {
            Font ??= Game1.ContentManager.Load<SpriteFont>("bigFont");
            this.atlas = atlas;
        }

        public void decrementResource(ResourceId resourceId)
        {
            if (ResourceCounts[resourceId] == 0) return;
            ResourceCounts[resourceId]--;
        }
        public void incrementResource(ResourceId resourceId)
        {
            ResourceCounts[resourceId]++;
        }

        private bool drawHelper(SpriteBatch spriteBatch, ResourceId resourceId, int deltaX)
        {
            if (ResourceCounts[resourceId] == 0) return false;
            float resourceX = this.X + 100 * deltaX + 30;
            spriteBatch.DrawString(Font, ResourceCounts[resourceId].ToString() + "X", new Vector2(resourceX - 25, this.Y + 30), Color.White, 0.0f, new Vector2(0, 0), 0.125f, SpriteEffects.None, 0.0f);

            if (resourceId == ResourceId.Wool)
            {
                spriteBatch.Draw(atlas.Texture, new Vector2(resourceX, this.Y), Atlas.GetRectangle(AtlasSpriteId.WoolResource), Color.White);
            }
            else if (resourceId == ResourceId.Wheat)
            {
                spriteBatch.Draw(atlas.Texture, new Vector2(resourceX, this.Y), Atlas.GetRectangle(AtlasSpriteId.WheatResource), Color.White);
            }
            else if (resourceId == ResourceId.Ore)
            {
                spriteBatch.Draw(atlas.Texture, new Vector2(resourceX, this.Y), Atlas.GetRectangle(AtlasSpriteId.OreResource), Color.White);
            }
            else if (resourceId == ResourceId.Brick)
            {
                spriteBatch.Draw(atlas.Texture, new Vector2(resourceX, this.Y), Atlas.GetRectangle(AtlasSpriteId.BrickResource), Color.White);
            }
            else if (resourceId == ResourceId.Wood)
            {
                spriteBatch.Draw(atlas.Texture, new Vector2(resourceX, this.Y), Atlas.GetRectangle(AtlasSpriteId.WoodResource), Color.White);
            }

            return true;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
            int delta = 0;
            if (drawHelper(spriteBatch, ResourceId.Wool, delta)) delta += 1;
            if (drawHelper(spriteBatch, ResourceId.Wheat, delta)) delta += 1;
            if (drawHelper(spriteBatch, ResourceId.Ore, delta)) delta += 1;
            if (drawHelper(spriteBatch, ResourceId.Brick, delta)) delta += 1;
            if (drawHelper(spriteBatch, ResourceId.Wood, delta)) delta += 1;
        }
    }
}
