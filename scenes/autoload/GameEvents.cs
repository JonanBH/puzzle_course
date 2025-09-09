using Game.Component;
using Godot;

namespace Game.Autoload;

public partial class GameEvents : Node
{
	[Signal]
	public delegate void BuildingPlacedEventHandler(BuildingComponent buildingComponent);

	public static GameEvents Instance { get; private set; }

	public override void _Notification(int what)
	{
		if (what == NotificationSceneInstantiated)
		{
			Instance = this;
		}
	}

	public static void EmitBuildingPlaced(BuildingComponent buildingComponent)
	{
		Instance.EmitSignal(SignalName.BuildingPlaced, buildingComponent);
	}
}
