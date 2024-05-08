namespace ListBuilder.View;

public partial class EditorPage : ContentPage
{
	public EditorPage(EditorViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}