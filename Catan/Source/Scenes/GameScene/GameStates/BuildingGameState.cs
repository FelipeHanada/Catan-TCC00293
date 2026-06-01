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
		UISlate buildSlate = new UISlate(600, 0, atlas, Color.Brown, 200, 200, "Contruir");
		buildSlate.setEnabled(false);

		return buildSlate;
	}

	public UISlate UISlate { get; private set; }
	public BuildingGameState(GameScene gameScene, Player player) : base(gameScene, player)
	{
		UISlate = BuildUISlate(gameScene.Atlas);
		AddChild(UISlate);
	}
}
