using System.Text.Json.Serialization;

namespace ListBuilder.Model;

public class GameSystemInfo
{
	[JsonConstructor]
	public GameSystemInfo()
	{
		Name = "";
		File = "";
		Version = "";
	}

	public GameSystemInfo(string name, string file, string version)
	{
		Name = name;
		File = file;
		Version = version;
	}

	public string NameAndVersion { get {  return Name + " " + Version; } }

	[JsonPropertyName("name")]
	public string Name { get; set; }
	[JsonPropertyName("file")]
	public string File { get; set; }
	[JsonPropertyName("version")]
	public string Version { get; set; }
	[JsonPropertyName("catalogues")]
	public Collection<Tuple<string, string>> Catalogues { get; set; } = new();
}

/*public class GameSystem
{
	public GameSystem()
	{
		Name = "";
	}

	public GameSystem(string name)
	{
		Name = name;
	}

	public string Name { get; set; }

	public BSGamesystemXml? Xml { get; set; }
	public Dictionary<string, BSCatalogueXml> CatalogueXmls { get; set; } = new();
}*/

