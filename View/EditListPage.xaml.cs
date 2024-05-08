namespace ListBuilder.View;

public partial class EditListPage : ContentPage
{
	public EditListPage(EditListViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}