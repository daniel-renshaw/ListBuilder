using System.Text.Json.Serialization;

namespace ListBuilder.Model;

public partial class Item : ObservableObject
{
	// Unique reference
	// Required
	[JsonPropertyName("name")]
	public string? Name { get; set; }

	// Text to display in app
	// Optional
	[JsonPropertyName("display")]
	public string? Display { get; set; }

	// Some value that can be referenced
	// Optional
	[JsonPropertyName("value")]
	public string? Value { get; set; }

	// Inherits properties from the Item name in this property
	// Value of this Item will overwrite inherited Value
	// HasType/Has will be in addition to inherited (so technically the Is Item could have a radio button list while this Item has a checkbox list)
	// Optional
	[JsonPropertyName("is")]
	public string? Is { get; set; }

	// Determines how the Items in Has will be handled
	// Optional
	// "all" -> this Item has all of these Items
	// "one" -> this Item has one of these Items (picker)
	// "any" -> this Item has any combination of these Items (select from list to add)
	// "unique" -> same as any but each item can only be used once (checkbox list)
	[JsonPropertyName("hasType")]
	public string? HasType { get; set; }

	// Any Items that this Item is direct parent to
	// Optional
	[JsonPropertyName("has")]
	public ObservableCollection<Item>? Has { get; set; }

	/*
	 * Non Json Members (internal stuff)
	 */
	[ObservableProperty]
	public bool? isExpanded = false;
}

public class ExampleUsage
{
	/* Hash table containing all Items referenced in a data set
	 *	[
	 *		{
	 *			"name": "unit",
	 *			"display": "Unit Name {statA}{statB}",
	 *			"hasType": "all",
	 *			"has": [
	 *				{
	 *					"name": "weapon",
	 *					"display": "Weapon Name {statA}{statB}",
	 *					"has": [
	 *						{
	 *							"name": "statA",
	 *							"display": "{value}",
	 *							"value": "2.5",
	 *						},
	 *						{
	 *							"name": "statB",
	 *							"display": "{value}%",
	 *							"value": "5",
	 *						},
	 *					],
	 *				},
	 *			],
	 *		},
	 *	]
	 *
	 * Items dictionary would contain 4 Items with keys: unit, weapon, statA, statB
	 */
	public Dictionary<string, Item> Items { get; set; } = new();
}
