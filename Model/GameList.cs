using System.Text.Json.Serialization;

namespace ListBuilder.Model;

public class GameList
{
	[JsonConstructor]
	public GameList()
	{
		Name = "";
		GameSystem = "";
	}

	[JsonPropertyName("name")]
	public string Name { get; set; }
	[JsonPropertyName("gameSystem")]
	public string GameSystem { get; set; }
	[JsonPropertyName("files")]
	public Collection<string> Files { get; set; } = new();
	[JsonPropertyName("list")]
	public Item List { get; set; } = new();
}

public class GameListInfo
{
	[JsonConstructor]
	public GameListInfo()
	{
		Name = "";
		Game = "";
	}

	public GameListInfo(string name, string game)
	{
		Name = name;
		Game = game;
	}

	[JsonPropertyName("name")]
	public string Name { get; set; }
	[JsonPropertyName("game")]
	public string Game { get; set; }
}
