namespace UnitConverter;

public partial class App : Application
{
	public App()
	{
        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(Secrets.SfLicenseKey);

        InitializeComponent();

		MainPage = new AppShell();
	}
}
