using ListBuilder.View;

namespace ListBuilder.ViewModel;

public partial class DataManagementViewModel : BaseViewModel
{
	public DataManagementViewModel(DataService dataService)
	{
		Title = "Data Manager";
		this.dataService = dataService;
	}

	[RelayCommand]
	public async Task LoadCardsAsync()
	{
		IsBusy = true;

		if (Cards.Count != dataService.DataBundles.Count)
		{
			Collection<BundleCardDisplay> temp = new();
			foreach (var bundle in dataService.DataBundles)
			{
				BSDRepositoryInfo repo = null!;

				try
				{
					repo = await dataService.GetRepository(bundle.Value.URL);
				}
				catch (Exception ex)
				{
					Debug.WriteLine($"Unable to get bundle repo info: {ex.Message}");
					await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
				}

				temp.Add(new BundleCardDisplay(bundle.Value.DisplayInfo, bundle.Value.Version, bundle.Value.BundleName, repo?.Version ?? ""));
			}

			Cards.ReplaceRange(temp);
		}

		IsBusy = false;
	}

	[RelayCommand]
	public void CardTapped(BundleCardDisplay card)
	{
		if (card is null)
			return;

		foreach (BundleCardDisplay cd in Cards)
		{
			if (cd != card && cd.IsExpanded)
				cd.IsExpanded = false;
		}
	}

	[RelayCommand]
	public async Task DeleteData(BundleCardDisplay card)
	{
		try
		{
			IsBusy = true;

			bool answer = await Shell.Current.DisplayAlert("Warning!", "Are you sure you want to delete this data? This action cannot be undone.", "Yes", "No");
			if (answer)
			{
				dataService.DeleteDataBundle(card.BundleName);
				await dataService.SaveDataBundleInformation();
				Cards.Remove(card);
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Failed to delete data: {ex.Message}");
			await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
		}
		finally
		{
			IsBusy = false;
		}
	}

	[RelayCommand]
	public async Task UpdateData(BundleCardDisplay card)
	{
		try
		{
			IsBusy = true;

			var repo = await dataService.DownloadRepository(card.BundleName, true);
			if (repo != null)
			{
				card.Description = repo.Version ?? "1.0";
				card.LatestVersion = repo.Version ?? "1.0";
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Failed to update data: {ex.Message}");
			await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
		}
		finally
		{
			IsBusy = false;
		}
	}

	[RelayCommand]
	async Task GoToAddData()
	{
		await Shell.Current.GoToAsync($"{nameof(AddDataPage)}");
	}

	DataService dataService;
	public ObservableRangeCollection<BundleCardDisplay> Cards { get; set; } = new();
}
