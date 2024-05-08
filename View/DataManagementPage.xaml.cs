namespace ListBuilder.View;

public partial class DataManagementPage : ContentPage
{
	public DataManagementPage(DataManagementViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}