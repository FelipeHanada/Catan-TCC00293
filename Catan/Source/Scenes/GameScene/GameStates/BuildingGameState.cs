using Catan.Source.Game;
using Catan.Source.Game.Player;
using Catan.Source.Scenes;
using Catan.Source.Scenes.Game;
using Catan.Source.Game.Resources;
using Microsoft.Xna.Framework;
using Catan.Source.Content;

public class BuildingGameState : PlayerTurnGameState
{
	static UISlate BuildUISlate(Atlas atlas)
	{
		UISlate tradeSlate = new UISlate(600, 200, atlas, Color.Gray, 600, 300, "Trocar");

		ResourceDisplay resourceDisplay = new ResourceDisplay(600,250, atlas);
		for (int i = 0; i < 10; i++) resourceDisplay.incrementResource(ResourceId.Wool);
		for (int i = 0; i < 21; i++) resourceDisplay.incrementResource(ResourceId.Wood);
		for (int i = 0; i < 3; i++) resourceDisplay.incrementResource(ResourceId.Ore);
		for (int i = 0; i < 4; i++) resourceDisplay.incrementResource(ResourceId.Brick);
		for (int i = 0; i < 5; i++) resourceDisplay.incrementResource(ResourceId.Wheat);
		tradeSlate.AddChild(resourceDisplay);

		return tradeSlate;
	}

	public UISlate UISlate { get; private set; }
	public BuildingGameState(GameScene gameScene, Player player) : base(gameScene, player)
	{
		UISlate = BuildUISlate(gameScene.Atlas);
	}

    public override void LoadContent()
    {
        base.LoadContent();
		_gameScene.Subscribe(UISlate);
    }

    public override void UnloadContent()
    {
        base.UnloadContent();
		_gameScene.Unsubscribe(UISlate);
    }
}
