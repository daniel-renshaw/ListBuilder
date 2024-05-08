namespace ListBuilder.ViewModel;

public partial class NewListViewModel : BaseViewModel
{
	public NewListViewModel(DataService dataService)
	{
		Title = "New List";
		this.dataService = dataService;
	}

	[RelayCommand]
	public async Task LoadGames()
	{
		try
		{
			IsBusy = true;

			if (dataService.GameSystems.Count == 0)
			{
				bool answer = await Shell.Current.DisplayAlert("Oops!", "You have no game data! Would you like to go download some?", "Yes", "No");
				if (answer)
					await Shell.Current.GoToAsync($"{nameof(DataManagementPage)}");
				else
					await Shell.Current.GoToAsync("..");
			}
			else
			{
				Collection<GameSystemInfo> temp = new();
				foreach (KeyValuePair<string, GameSystemInfo> pair in dataService.GameSystems)
					temp.Add(pair.Value);
				Games.ReplaceRange(temp);
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Failed in loading games: {ex.Message}");
			await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
			await Shell.Current.GoToAsync("..");
		}
		finally
		{
			IsBusy = false;
		}
	}

	[RelayCommand]
	public async Task CompleteListCreation()
	{
		if (!HasFilledOutForm)
			return;

		IsBusy = true;

		GameList list = new()
		{
			Name = ListNameString!,
			GameSystem = SelectedGame!.File
		};

		foreach (SelectableCard card in Files)
		{
			if (card.IsSelected)
				list.Files.Add(card.Description);
		}

		// TODO save list here

		await Shell.Current.GoToAsync($"../{nameof(EditListPage)}", true, new Dictionary<string, object>
		{
			{ "GameList", list }
		});

		IsBusy = false;
	}

	partial void OnSelectedGameChanged(GameSystemInfo? value)
	{
		if (value == null)
		{
			Files.Clear();
			return;
		}

		Collection<SelectableCard> temp = new();
		foreach (Tuple<string, string> pair in value.Catalogues)
			temp.Add(new SelectableCard(pair.Item2, pair.Item1, Color.FromArgb("#212121")));
		Files.ReplaceRange(temp);
	}

	[RelayCommand]
	public void CardTapped(SelectableCard card)
	{
		if (card is null)
			return;

		card.IsSelected = !card.IsSelected;

		if (card.IsSelected)
			card.BgColor = Colors.Green;
		else
			card.BgColor = Color.FromArgb("#212121");
	}

	DataService dataService;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(HasFilledOutForm))]
	public string? listNameString;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(HasFilledOutForm))]
	[NotifyPropertyChangedFor(nameof(HasSelectedGame))]
	public GameSystemInfo? selectedGame;

	public bool HasSelectedGame { get { return SelectedGame != null; } }
	public bool HasFilledOutForm { get { return ListNameString?.Length > 0 && HasSelectedGame; } }

	public ObservableRangeCollection<GameSystemInfo> Games { get; set; } = new();
	public ObservableRangeCollection<SelectableCard> Files { get; set; } = new();
}
