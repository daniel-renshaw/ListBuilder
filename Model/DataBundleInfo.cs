using System.Text.Json.Serialization;

namespace ListBuilder.Model;

public class DataBundleInfo
{
	[JsonConstructor]
	public DataBundleInfo()
	{
		BundleName = "";
		DisplayInfo = "";
		Version = "";
		URL = "";
	}

	public DataBundleInfo(string name, string display, string ver, string url)
	{
		BundleName = name;
		DisplayInfo = display;
		Version = ver;
		URL = url;
	}

	[JsonPropertyName("bundleName")]
	public string BundleName { get; set; }
	[JsonPropertyName("displayInfo")]
	public string DisplayInfo { get; set; }
	[JsonPropertyName("version")]
	public string Version { get; set;}
	[JsonPropertyName("url")]
	public string URL { get; set; }
}
