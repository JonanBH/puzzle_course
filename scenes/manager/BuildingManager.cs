using Game.Resources.Building;
using Game.UI;
using Godot;

namespace Game.Manager;

public partial class BuildingManager : Node
{

	[Export]
	private GridManager gridManager;
	[Export]
	private Node2D ySortRoot;
	[Export]
	private Node2D cursor;
	[Export]
	private GameUI gameUI;

	private int startingResourceCount = 4;
	private int currentResourceCount = 0;
	private int currentlyUsedResourceCount = 0;
	private int AvailableResourceCount => startingResourceCount + currentResourceCount - currentlyUsedResourceCount;

	private BuildingResource toPlaceBuildingResource;
	private Vector2I? hoveredGridCell;

	public override void _Ready()
	{
		gridManager.ResourceTilesUpdated += OnResourceTilesUpdated;

		gameUI.BuildingResourceSelected += OnBuldingResourceSelected;
	}

	public override void _UnhandledInput(InputEvent evt)
	{
		if (evt.IsActionPressed("left_click") && hoveredGridCell.HasValue &&
			gridManager.IsTilePositionBuildable(hoveredGridCell.Value) &&
			toPlaceBuildingResource != null &&
			AvailableResourceCount >= toPlaceBuildingResource.ResourceCost)
		{
			PlaceBuildingAtHoveredCellPosition();

			cursor.Visible = false;
		}
	}


	public override void _Process(double delta)
	{
		var gridPosition = gridManager.GetMouseGridCellPosition();
		cursor.GlobalPosition = gridPosition * 64;

		if (cursor.Visible && (!hoveredGridCell.HasValue || hoveredGridCell.Value != gridPosition) &&
			toPlaceBuildingResource != null)
		{
			hoveredGridCell = gridPosition;
			gridManager.ClearHighlightedTiles();
			gridManager.HighlightExpandedBuildableTiles(hoveredGridCell.Value, toPlaceBuildingResource.BuildableRadius);
			gridManager.HighlightResourceTiles(hoveredGridCell.Value, toPlaceBuildingResource.ResourceRadius);
		}
	}

	private void PlaceBuildingAtHoveredCellPosition()
	{
		if (!hoveredGridCell.HasValue)
		{
			return;
		}

		var building = toPlaceBuildingResource.BuildingScene.Instantiate<Node2D>();
		building.GlobalPosition = hoveredGridCell.Value * 64;

		ySortRoot.AddChild(building);

		hoveredGridCell = null;

		gridManager.ClearHighlightedTiles();

		currentlyUsedResourceCount += toPlaceBuildingResource.ResourceCost;
		GD.Print($"Remaining available resource: {AvailableResourceCount}");
	}

	private void OnResourceTilesUpdated(int resourceCount)
	{
		currentResourceCount = resourceCount;
	}
	
	private void OnBuldingResourceSelected(BuildingResource buildingResource)
	{
		toPlaceBuildingResource = buildingResource;
		cursor.Visible = true;
		gridManager.HighlightBuildableTiles();
	}
}
