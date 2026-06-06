using Catan.Source.Game;
using Catan.Source.Scenes;
using Microsoft.Xna.Framework;
using Xunit;

namespace Catan.Tests.Source.Game
{
    public class GameObjectTests
    {
        [Fact]
        public void Constructor_SetsCoordinatesAndStartsWithoutChildren()
        {
            GameObject gameObject = new(12.5f, 34.5f);

            Assert.Equal(12.5f, gameObject.X);
            Assert.Equal(34.5f, gameObject.Y);
            Assert.Empty(gameObject.Children);
        }

        [Fact]
        public void DefaultConstructor_UsesOriginAndNoOpLifecycleMethodsDoNotThrow()
        {
            GameObject gameObject = new();

            Assert.Equal(0, gameObject.X);
            Assert.Equal(0, gameObject.Y);
            gameObject.Update(new GameTime());
            gameObject.Draw(new GameTime(), null);
        }

        [Fact]
        public void AddChild_AppendsChildToChildren()
        {
            GameObject parent = new();
            GameObject child = new();

            parent.AddChild(child);

            Assert.Single(parent.Children);
            Assert.Same(child, parent.Children[0]);
        }

        [Fact]
        public void SceneSubscription_SubscribesAndUnsubscribesChildren()
        {
            TestScene scene = new();
            TrackingGameObject parent = new();
            TrackingGameObject child = new();
            parent.AddChild(child);

            scene.Subscribe(parent);
            scene.Update(new GameTime());

            Assert.Equal(1, parent.SubscribeCount);
            Assert.Equal(1, child.SubscribeCount);

            scene.Unsubscribe(parent);
            scene.Update(new GameTime());

            Assert.Equal(1, parent.UnsubscribeCount);
            Assert.Equal(1, child.UnsubscribeCount);
        }

        private class TestScene : Scene
        {
        }

        private class TrackingGameObject : GameObject
        {
            public int SubscribeCount { get; private set; }
            public int UnsubscribeCount { get; private set; }

            public override void OnSubscribe(Scene scene)
            {
                SubscribeCount++;
                base.OnSubscribe(scene);
            }

            public override void OnUnsubscribe(Scene scene)
            {
                UnsubscribeCount++;
                base.OnUnsubscribe(scene);
            }
        }
    }
}
