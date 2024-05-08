namespace ListBuilder.View;

public partial class NewListPage : ContentPage
{
	public NewListPage(NewListViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}