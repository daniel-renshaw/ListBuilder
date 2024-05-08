using ListBuilder.Services;
using ListBuilder.View;
using Microsoft.Extensions.Logging;

namespace ListBuilder;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		// Shared
		builder.Services.AddSingleton<DataService>();

		// Viewer
		builder.Services.AddSingleton<HomeViewModel>();
		builder.Services.AddSingleton<DataManagementViewModel>();
		builder.Services.AddSingleton<EditorViewModel>();
		builder.Services.AddTransient<AddDataViewModel>();
		builder.Services.AddTransient<NewListViewModel>();
		builder.Services.AddTransient<EditListViewModel>();

		builder.Services.AddSingleton<MainPage>();
		builder.Services.AddSingleton<DataManagementPage>();
		builder.Services.AddSingleton<EditorPage>();
		builder.Services.AddTransient<AddDataPage>();
		builder.Services.AddTransient<NewListPage>();
		builder.Services.AddTransient<EditListPage>();

		// Editor

		return builder.Build();
	}
}
