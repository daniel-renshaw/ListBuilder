namespace ListBuilder.ViewModel;

public partial class AddDataViewModel : BaseViewModel
{
	public AddDataViewModel(DataService dataService)
	{
		Title = "Add Data";
		this.dataService = dataService;
		numberOfItemsSelected = 0;
		Task.Run(GetGalleryAsync);
	}

	[RelayCommand]
	async Task GetGalleryAsync()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;
			var gallery = await dataService.GetGallery();

			if (AllRepos.Count != 0)
				AllRepos.Clear();

			foreach (var repo in gallery.Repositories)
			{
				if (dataService.DataBundles.ContainsKey(repo.Name!))
					continue;
				AllRepos.Add(new RepoSelectableCard(repo, Color.FromArgb("#212121")));
			}

			DisplayedRepos.ReplaceRange(AllRepos);
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Unable to get gallery: {ex.Message}");
			await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
		}
		finally
		{
			IsBusy = false;
		}
	}

	[RelayCommand]
	public void CardTapped(SelectableCard card)
	{
		if (card is null)
			return;

		card.IsSelected = !card.IsSelected;

		if (card.IsSelected)
		{
			card.BgColor = Colors.Green;
			++numberOfItemsSelected;
			HasSelectedSomething = true;
		}
		else
		{
			card.BgColor = Color.FromArgb("#212121");
			--numberOfItemsSelected;
			if (numberOfItemsSelected == 0)
				HasSelectedSomething = false;
		}
	}

	[RelayCommand]
	public async Task DownloadData()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			foreach (var card in AllRepos)
			{
				if (!card.IsSelected)
					continue;

				card.Repo = await dataService.DownloadRepository(card.Repo, false);
				dataService.AddDataBundle(card.Repo);
			}

			await dataService.SaveDataBundleInformation();
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Unable to download data: {ex.Message}");
			await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
		}
		finally
		{
			IsBusy = false;

			await Shell.Current.GoToAsync("..");
		}
	}

	partial void OnSearchStringChanged(string? value)
	{
		if (SearchString is null)
			return;

		var repos = AllRepos.Where(x => x.Heading.Contains(SearchString, StringComparison.OrdinalIgnoreCase));
		if (repos.Count() != DisplayedRepos.Count)
			DisplayedRepos.Clear();

		foreach (var repo in repos)
			DisplayedRepos.Add(repo);
	}

	DataService dataService;
	uint numberOfItemsSelected;
	Collection<RepoSelectableCard> AllRepos { get; } = new();

	public ObservableRangeCollection<RepoSelectableCard> DisplayedRepos { get; set; } = new();

	[ObservableProperty]
	public string? searchString;

	[ObservableProperty]
	public bool hasSelectedSomething;
}
