using ListBuilder.View;

namespace ListBuilder;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(DataManagementPage), typeof(DataManagementPage));
		Routing.RegisterRoute(nameof(AddDataPage), typeof(AddDataPage));
		Routing.RegisterRoute(nameof(NewListPage), typeof(NewListPage));
		Routing.RegisterRoute(nameof(EditListPage), typeof(EditListPage));
		Routing.RegisterRoute(nameof(EditorPage), typeof(EditorPage));
	}
}
