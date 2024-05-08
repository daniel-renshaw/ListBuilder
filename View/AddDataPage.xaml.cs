namespace ListBuilder.View;

public partial class AddDataPage : ContentPage
{
	public AddDataPage(AddDataViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}