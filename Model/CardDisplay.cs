namespace ListBuilder.Model;

public partial class CardDisplay : ObservableObject
{
	public CardDisplay(string head, string desc)
	{
		heading = head;
		description = desc;
		isExpanded = false;
	}

	[ObservableProperty]
	public string heading;
	[ObservableProperty]
	public string description;
	[ObservableProperty]
	public bool isExpanded;
}

public partial class BundleCardDisplay : CardDisplay
{
	public BundleCardDisplay(string h1, string h2, string bundleName, string latestVer) : base(h1, h2)
	{
		BundleName = bundleName;
		latestVersion = latestVer;
	}

	public string BundleName;
	[ObservableProperty]
	public string latestVersion;

	public bool NeedsUpdate => (LatestVersion != "" && !Description.Equals(LatestVersion));
	public bool DoesntNeedUpdate => !NeedsUpdate;
}
