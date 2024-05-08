using ListBuilder.View;

namespace ListBuilder.ViewModel;

public partial class HomeViewModel : BaseViewModel
{
	public HomeViewModel(DataService dataService)
	{
		Title = "Lists";
		this.dataService = dataService;
	}

	[RelayCommand]
	public async Task LoadCards()
	{
		try
		{
			IsBusy = true;

			await dataService.HandleInitialLoad();

			if (AllCards.Count != dataService.GameLists.Count)
			{
				foreach (var list in dataService.GameLists)
					AllCards.Add(new CardDisplay(list.Name, list.Game));

				DisplayedCards.ReplaceRange(AllCards);
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Failed to load list: {ex.Message}");
			await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
		}
		finally
		{
			IsBusy = false;
		}
	}

	[RelayCommand]
	public async Task DeleteListAsync(CardDisplay card)
	{
		try
		{
			IsBusy = true;

			bool answer = await Shell.Current.DisplayAlert("Warning!", "Are you sure you want to delete this list? This action cannot be undone.", "Yes", "No");
			if (answer)
			{
				//dataService.DeleteDataBundle(card.BundleName);
				//await dataService.SaveDataBundleInformation();
				//Cards.Remove(card);
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Failed to delete list: {ex.Message}");
			await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
		}
		finally
		{
			IsBusy = false;
		}
	}

	[RelayCommand]
	public async Task EditListAsync(CardDisplay card)
	{
		try
		{
			IsBusy = true;
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Failed to edit list: {ex.Message}");
			await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
		}
		finally
		{
			IsBusy = false;
		}
	}

	[RelayCommand]
	public void CardTapped(CardDisplay card)
	{
		if (card is null)
			return;

		foreach (CardDisplay cd in AllCards)
		{
			if (cd != card && cd.IsExpanded)
				cd.IsExpanded = false;
		}
	}

	[RelayCommand]
	public async Task GoToNewListAsync()
	{
		await Shell.Current.GoToAsync($"{nameof(NewListPage)}");
	}

	partial void OnSearchStringChanged(string? value)
	{
		if (SearchString is null)
			return;

		var cards = AllCards.Where(x => (x.Heading.Contains(SearchString, StringComparison.OrdinalIgnoreCase) || x.Description.Contains(SearchString, StringComparison.OrdinalIgnoreCase)));
		if (cards.Count() != DisplayedCards.Count)
			DisplayedCards.Clear();

		foreach (var card in cards)
			DisplayedCards.Add(card);
	}

	DataService dataService;
	Collection<CardDisplay> AllCards { get; } = new();
	public ObservableRangeCollection<CardDisplay> DisplayedCards { get; set; } = new();

	[ObservableProperty]
	public string? searchString;
}
