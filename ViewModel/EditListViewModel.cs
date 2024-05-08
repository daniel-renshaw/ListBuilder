namespace ListBuilder.ViewModel;

[QueryProperty(nameof(GameList), "GameList")]
public partial class EditListViewModel : BaseViewModel
{
	public EditListViewModel(DataService dataService)
	{
		Title = "Edit List";
		DisplayText = "";
		this.dataService = dataService;
	}

	[RelayCommand]
	public async Task LoadAsync()
	{
		try
		{
			IsBusy = true;

			if (GameList == null)
				throw new NullReferenceException("GameList is null");

			await dataService.LoadGameSystem(GameList.GameSystem);

			foreach (var cat in GameList.Files)
				await dataService.LoadCatalogue(GameList.GameSystem, cat);

			Title = $"Editing {GameList.Name}";

			if (GameList.List.Name is null)
			{
				// new list
				// load from dataService.ListDesc
			}
			else
			{
				// editing existing list
				// load from GameList.List
			}

			Collection<string> forceEntries = dataService.GetItemsInheritedFrom("force");
			List<string> temp = new();
			foreach (var force in forceEntries)
			{
				if (dataService.LoadedData.TryGetValue(force, out Item result))
					temp.Add(result.Display ?? "Error: No display set!");
			}

			temp.Sort(StringComparer.CurrentCulture);
			Forces.ReplaceRange(temp);
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Unable to load game list: {ex.Message}");
			await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
		}
		finally
		{
			IsBusy = false;
		}
	}

	partial void OnSelectedForceChanged(string? value)
	{
		// do stuffs
	}

	DataService dataService;

	[ObservableProperty]
	GameList? gameList;

	[ObservableProperty]
	public string? selectedForce;

	[ObservableProperty]
	string? displayText;

	public ObservableRangeCollection<string> Forces { get; set; } = new();
}
