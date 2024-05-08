namespace ListBuilder.ViewModel;

public partial class EditorViewModel : BaseViewModel
{
	public EditorViewModel(DataService dataService)
	{
		Title = "Editor";
		this.dataService = dataService;
	}

	[RelayCommand]
	public async Task LoadItems()
	{
		try
		{
			IsBusy = true;

			Collection<Item> temp = new();
			Item item1 = new Item{ Display = "Item 1", Has = new ObservableCollection<Item>() };
			Item item2 = new Item { Display = "Item 2", Has = new ObservableCollection<Item>() };
			Item item3 = new Item { Display = "Item 3" };
			Item item4 = new Item { Display = "Item 4" };
			Item item5 = new Item { Display = "Item 5" };

			ItemCatalog[item1.Display] = item1;
			ItemCatalog[item2.Display] = item2;
			ItemCatalog[item3.Display] = item3;
			ItemCatalog[item4.Display] = item4;
			ItemCatalog[item5.Display] = item5;

			item1.Has.Add(item2);
			item1.Has.Add(item3);
			item1.Has.Add(item4);
			item2.Has.Add(item5);

			temp.Add(item1);
			Items.ReplaceRange(temp);
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Failed in loading items: {ex.Message}");
			await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
			await Shell.Current.GoToAsync("..");
		}
		finally
		{
			IsBusy = false;
		}
	}

	[RelayCommand]
	public void NodeTapped(Item item)
	{
		if (item is null || item.Display is null)
			return;
	}

	DataService dataService;
	Dictionary<string, Item> ItemCatalog { get; set; } = new();

	public ObservableRangeCollection<Item> Items { get; set; } = new();
}
