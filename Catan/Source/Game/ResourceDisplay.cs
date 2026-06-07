using Catan.Source.Content;
using Catan.Source.Game.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Catan.Source.Game
{
    public class ResourceDisplay : GameObject
    {
        private Dictionary<ResourceId, int> _resourceCounts = new()
        {
            [ResourceId.Wood] = 0,
            [ResourceId.Wool] = 0,
            [ResourceId.Brick] = 0,
            [ResourceId.Ore] = 0,
            [ResourceId.Wheat] = 0,
        };
        private readonly List<ButtonAction> _selectionButtons = new();
        private SpriteFont _font;
        private Atlas _atlas;
        public ResourceDisplay(float x, float y, Atlas atlas) : base(x, y)
        {
            _font ??= Game1.ContentManager.Load<SpriteFont>("bigFont");
            _atlas = atlas;

            //add buttons
            for (int i = 0; i < ResourceUtils.ResourceIds.Length; i++)
            {
                float buttonX = this.X + 100 * i + 30;
                ResourceId resourceId = ResourceUtils.ResourceIds[i];
                Action incrementAction = () => { IncrementResource(resourceId); };
                Action decrementAction = () => { DecrementResource(resourceId); };
                ButtonAction incrementButton = new ButtonAction(buttonX + 30, this.Y+90, atlas, incrementAction, "+");
                incrementButton.SetFontScale(0.08f);
                ButtonAction decrementButton = new ButtonAction(buttonX, this.Y+90, atlas, decrementAction, "-");
                decrementButton.SetFontScale(0.08f);

                _selectionButtons.Add(incrementButton);
                _selectionButtons.Add(decrementButton);
                AddChild(incrementButton);
                AddChild(decrementButton);
            }
        }

        public void DecrementResource(ResourceId resourceId)
        {
            if (_resourceCounts[resourceId] == 0) return;
            _resourceCounts[resourceId]--;
        }
        public void IncrementResource(ResourceId resourceId)
        {
            _resourceCounts[resourceId]++;
        }

        public Dictionary<ResourceId, int> GetSelectedResources()
        {
            Dictionary<ResourceId, int> selectedResources = new();

            foreach (KeyValuePair<ResourceId, int> resource in _resourceCounts)
            {
                if (resource.Value > 0)
                {
                    selectedResources[resource.Key] = resource.Value;
                }
            }

            return selectedResources;
        }

        public void SetSelectedResources(IReadOnlyDictionary<ResourceId, int> resources)
        {
            Clear();

            foreach (KeyValuePair<ResourceId, int> resource in resources)
            {
                if (_resourceCounts.ContainsKey(resource.Key))
                {
                    _resourceCounts[resource.Key] = Math.Max(0, resource.Value);
                }
            }
        }

        public void SetSelectionEnabled(bool isEnabled)
        {
            foreach (ButtonAction button in _selectionButtons)
            {
                button.SetEnabled(isEnabled);
            }
        }

        public void Clear()
        {
            foreach (ResourceId resource in ResourceUtils.ResourceIds)
            {
                _resourceCounts[resource] = 0;
            }
        }



        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
            for (int i = 0; i < ResourceUtils.ResourceIds.Length; i++)
            {
                float resourceX = this.X + 100 * i + 30;
                ResourceId resourceId = ResourceUtils.ResourceIds[i];
                spriteBatch.DrawString(_font, _resourceCounts[resourceId].ToString() + "X", new Vector2(resourceX - 25, this.Y + 30), Color.White, 0.0f, new Vector2(0, 0), 0.125f, SpriteEffects.None, 0.0f);

                if (resourceId == ResourceId.Wool)
                {
                    spriteBatch.Draw(_atlas.Texture, new Vector2(resourceX, this.Y), Atlas.GetRectangle(AtlasSpriteId.WoolResource), Color.White);
                }
                else if (resourceId == ResourceId.Wheat)
                {
                    spriteBatch.Draw(_atlas.Texture, new Vector2(resourceX, this.Y), Atlas.GetRectangle(AtlasSpriteId.WheatResource), Color.White);
                }
                else if (resourceId == ResourceId.Ore)
                {
                    spriteBatch.Draw(_atlas.Texture, new Vector2(resourceX, this.Y), Atlas.GetRectangle(AtlasSpriteId.OreResource), Color.White);
                }
                else if (resourceId == ResourceId.Brick)
                {
                    spriteBatch.Draw(_atlas.Texture, new Vector2(resourceX, this.Y), Atlas.GetRectangle(AtlasSpriteId.BrickResource), Color.White);
                }
                else if (resourceId == ResourceId.Wood)
                {
                    spriteBatch.Draw(_atlas.Texture, new Vector2(resourceX, this.Y), Atlas.GetRectangle(AtlasSpriteId.WoodResource), Color.White);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

        }
    }
}
