using System.IO.Compression;
using System.Net.Http.Json;
using System.Text.Json;
using System.Xml.Serialization;

namespace ListBuilder.Services;

public class DataService
{
	HttpClient httpClient;
	bool hasDoneInitialLoad;
	Dictionary<string, Collection<string>> inheritanceDictionary;
	Collection<string> loadedCatalogues;

	// We want semi parsed data on all of these so that we don't have to parse so many files every time
	// DataBundleInfo.json
	public Dictionary<string, DataBundleInfo> DataBundles { get; set; } = new();
	// GameListInfo.json
	public Collection<GameListInfo> GameLists { get; set; } = new();
	// GameSystemInfo.json
	public Dictionary<string, GameSystemInfo> GameSystems { get; set; } = new();

	//public GameSystem LoadedGameSystem { get; set; } = new();

	public Dictionary<string, Item> LoadedData { get; set; } = new();
	public Item ListDesc { get; set; } = new();

	public DataService()
	{
		httpClient = new HttpClient();
		hasDoneInitialLoad = false;
		inheritanceDictionary = new Dictionary<string, Collection<string>>();
		loadedCatalogues = new Collection<string>();
	}

	public string GetDataPath()
	{
		return Path.Combine(FileSystem.Current.AppDataDirectory, "data");
	}

	public string GetDataPath(string folder)
	{
		return Path.Combine(FileSystem.Current.AppDataDirectory, "data", folder);
	}

	public string GetDataPath(string folder, string file)
	{
		return Path.Combine(FileSystem.Current.AppDataDirectory, "data", folder, file);
	}

	public async Task HandleInitialLoad()
	{
		if (hasDoneInitialLoad)
			return;

		await Task.Run(() =>
		{
			LoadDataBundleInformation();
			LoadGameListInformation();
			LoadGameSystemInformation();
			hasDoneInitialLoad = true;
		});
	}

	public async Task<BSDGalleryInfo> GetGallery()
	{
		var res = await httpClient.GetFromJsonAsync<BSDGalleryInfo>("insertrepourlhere");
		return res!;
	}

	public async Task<BSDRepositoryInfo> GetRepository(string url)
	{
		var res = await httpClient.GetFromJsonAsync<BSDRepositoryInfo>(url);
		return res!;
	}

	public async Task<BSDRepositoryInfo> DownloadRepository(BSDRepositoryInfo repo, bool updateBundles = true)
	{
		if (repo.Name is null || repo.RepositoryUrl is null)
			return repo;

		// repo only has partial info from galley, get full info from actual url
		return await DownloadRepository(repo.Name, repo.RepositoryUrl, updateBundles);
	}

	public async Task<BSDRepositoryInfo> DownloadRepository(string name, string url, bool updateBundles = true)
	{
		var repoInfo = await GetRepository(url);
		foreach (var file in repoInfo.RepositoryFiles)
			await DownloadFile(name, file);

		var dir = new DirectoryInfo(GetDataPath(name));
		LoadGameSystemInformationForDirectory(dir);

		if (updateBundles)
		{
			AddDataBundle(repoInfo);
			await SaveDataBundleInformation();
		}

		return repoInfo;
	}

	public async Task<BSDRepositoryInfo> DownloadRepository(string name, bool updateBundles = true)
	{
		DataBundleInfo dbi;
		if (DataBundles.TryGetValue(name, out dbi!))
			return await DownloadRepository(dbi.BundleName, dbi.URL, updateBundles);
		return null!;
	}

	private async Task DownloadFile(string folder, BSDFileInfo file)
	{
		if (file.Name is null || file.FileUrl is null)
			return;

		var stream = await httpClient.GetStreamAsync(file.FileUrl);
		using var archive = await Task.Run(() => new ZipArchive(stream, ZipArchiveMode.Read));
		if (archive.Entries.Count != 1)
			throw new InvalidOperationException("Invalid file has more than 1 entry.");

		using var entry = archive.Entries[0].Open();
		string fileName = archive.Entries[0].Name;
		string folderPath = Path.Combine(GetDataPath(folder));
		string path = Path.Combine(folderPath, fileName);

		if (!Directory.Exists(folderPath))
			Directory.CreateDirectory(folderPath);

		using FileStream output = File.Open(path, FileMode.Create);
		await entry.CopyToAsync(output);
	}

	private void LoadDataBundleInformation()
	{
		string filePath = Path.Combine(FileSystem.Current.AppDataDirectory, "DataBundleInfo.json");
		if (File.Exists(filePath))
		{
			using var stream = File.OpenRead(filePath);
			JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };
			DataBundles = JsonSerializer.Deserialize<Dictionary<string, DataBundleInfo>>(stream) ?? new();
		}
	}

	public void AddDataBundle(BSDRepositoryInfo repo)
	{
		DataBundles[repo.Name!] = new DataBundleInfo(repo.Name!, repo.Description ?? repo.Name!, repo.Version ?? "1.0", repo.RepositoryUrl!);
	}

	public async Task SaveDataBundleInformation()
	{
		string filePath = Path.Combine(FileSystem.Current.AppDataDirectory, "DataBundleInfo.json");
		using var stream = File.Open(filePath, FileMode.Create);
		JsonSerializerOptions options = new() { WriteIndented = true };
		await JsonSerializer.SerializeAsync(stream, DataBundles, options);
	}

	public void DeleteDataBundle(string bundleName)
	{
		DataBundleInfo db = DataBundles[bundleName];
		if (db is null)
			return;

		string path = Path.Combine(FileSystem.Current.AppDataDirectory, bundleName);
		DirectoryInfo di = new DirectoryInfo(path);
		if (di.Exists)
			di.Delete(true);

		DataBundles.Remove(bundleName);
	}

	private void LoadGameListInformation()
	{
		throw new Exception("DataService::LoadGameListInformation needs implementing");
	}

	private void LoadGameSystemInformationForDirectory(DirectoryInfo dir)
	{
		if (!dir.Exists)
			return;

		var files = dir.GetFiles("*.gst");
		if (files.Length > 0)
		{
			foreach (var file in files)
			{
				using XmlReader reader = XmlReader.Create(file.FullName);
				if (reader.MoveToContent() == XmlNodeType.Element)
				{
					if (reader.MoveToAttribute("id"))
					{
						var id = reader.Value;
						if (reader.MoveToAttribute("name"))
						{
							var name = reader.Value;
							if (reader.MoveToAttribute("revision"))
							{
								var ver = "v" + reader.Value;
								GameSystems[id] = new GameSystemInfo(name, dir.Name + "/" + file.Name, ver);
							}
						}
					}
				}
			}
		}

		var cats = dir.GetFiles("*.cat");
		if (cats.Length > 0)
		{
			foreach (var file in cats)
			{
				using XmlReader reader = XmlReader.Create(file.FullName);
				if (reader.MoveToContent() == XmlNodeType.Element)
				{
					if (reader.MoveToAttribute("name"))
					{
						var name = reader.Value;
						if (reader.MoveToAttribute("revision"))
						{
							var ver = "v" + reader.Value;
							if (reader.MoveToAttribute("library"))
							{
								var library = reader.Value;
								if (library == "true")
									continue;

								if (reader.MoveToAttribute("gameSystemId"))
								{
									var gsi = reader.Value;
									GameSystems[gsi].Catalogues.Add(Tuple.Create(file.Name, name + " " + ver));
								}
							}
						}
					}
				}
			}
		}
	}

	private void LoadGameSystemInformation()
	{
		DirectoryInfo di = new DirectoryInfo(GetDataPath());
		if (!di.Exists)
			return;

		foreach (DirectoryInfo dir in di.EnumerateDirectories())
		{
			LoadGameSystemInformationForDirectory(dir);
		}
	}

	public async Task LoadGameSystem(string gameSystem)
	{
		throw new Exception("DataService::LoadGameSystem needs implementing");
	}

	public async Task LoadCatalogue(string gameSystem, string catalogue)
	{
		throw new Exception("DataService::LoadCatalogue needs implementing");
	}

	public Collection<string> GetItemsInheritedFrom(string name)
	{
		Collection<string> list;
		if (inheritanceDictionary.TryGetValue(name, out list!))
			return list;
		return new();
	}
}
