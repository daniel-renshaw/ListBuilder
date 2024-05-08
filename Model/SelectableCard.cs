namespace ListBuilder.Model;

public partial class SelectableCard : ObservableObject
{
	public SelectableCard(string heading, string desc, Color color)
	{
		Heading = heading;
		Description = desc;
		BgColor = color;
		IsSelected = false;
	}

	public string Heading { get; set; }
	public string Description { get; set; }

	[ObservableProperty]
	public bool isSelected;

	[ObservableProperty]
	public Color? bgColor;
}

public partial class RepoSelectableCard : SelectableCard
{
	public RepoSelectableCard(BSDRepositoryInfo repoInfo, Color color) : base(repoInfo.Description ?? "", repoInfo.GithubUrl ?? "", color)
	{
		Repo = repoInfo;
	}

	public BSDRepositoryInfo Repo { get; set; }
}
