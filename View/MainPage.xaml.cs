namespace ListBuilder.View;

public partial class MainPage : ContentPage
{
	public MainPage(HomeViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}

