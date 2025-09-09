using Game.Manager;
using Game.Resources.Building;
using Godot;

namespace Game;

public partial class Main : Node
{

	private Sprite2D cursor;
	private BuildingResource towerResource;
	private BuildingResource villageResource;
	private Button placeTowerButton;
	private Button placeVillageButton;
	private GridManager gridManager;
	private Node2D ySortRoot;

	private Vector2I? hoveredGridCell;
	private BuildingResource toPlaceBuildingResource;

	public override void _Ready()
	{
		gridManager = GetNode<GridManager>("GridManager");
		towerResource = GD.Load<BuildingResource>("res://resources/building/Tower.tres");
		villageResource = GD.Load<BuildingResource>("res://resources/building/Village.tres");
		placeTowerButton = GetNode<Button>("PlaceTowerButton");
		placeVillageButton = GetNode<Button>("PlaceVillageButton");
		ySortRoot = GetNode<Node2D>("YSortRoot");
		cursor = GetNode<Sprite2D>("Cursor");

		cursor.Visible = false;

		placeTowerButton.Pressed += OnPlaceTowerButtonPressed;
		placeVillageButton.Pressed += OnPlaceVillageButtonPressed;
		gridManager.ResourceTilesUpdated += OnResourceTilesUpdated;

	}

	public override void _UnhandledInput(InputEvent evt)
	{
		if (evt.IsActionPressed("left_click") && hoveredGridCell.HasValue &&
			gridManager.IsTilePositionBuildable(hoveredGridCell.Value))
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
	}

	private void OnPlaceTowerButtonPressed()
	{
		cursor.Visible = true;
		gridManager.HighlightBuildableTiles();
		toPlaceBuildingResource = towerResource;
	}

	private void OnPlaceVillageButtonPressed()
	{
		cursor.Visible = true;
		gridManager.HighlightBuildableTiles();
		toPlaceBuildingResource = villageResource;
	}

	private void OnResourceTilesUpdated(int resourceCount)
	{
		GD.Print(resourceCount);
	}
}
