using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Windows;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace Plugin.UI
{
	public class PluginButton : IExtensionApplication
	{

		public RibbonControl Ribbon;
		public bool IsButtonAddedToRibbon = false;

		public void Initialize()
		{
			Application.Idle += OnApplicationIdle;
		}

		private void OnApplicationIdle(object sender, EventArgs e)
		{
			if (IsButtonAddedToRibbon == true)
			{
				Application.Idle -= OnApplicationIdle;
				return;
			}
			Ribbon = ComponentManager.Ribbon;
			if (Ribbon != null)
			{
				AddRibbonTab();
			}
			IsButtonAddedToRibbon = true;
		}

		public void Terminate()
		{

		}

		private void AddRibbonTab()
		{
			Ribbon = ComponentManager.Ribbon;

			RibbonTab existingTab = Ribbon.Tabs.FirstOrDefault(tab => tab.Title.Equals("Add-Ins", StringComparison.OrdinalIgnoreCase));

			if (existingTab == null)
			{
				existingTab = new RibbonTab
				{
					Title = "Add-Ins",
					Id = "DATA_EXTRACTOR_TAB"
				};
				Ribbon.Tabs.Add(existingTab);
			}

			// Create a new Ribbon panel
			RibbonPanel existingPanel = existingTab.Panels.FirstOrDefault(panel => panel.Source.Title.Equals("Extract", StringComparison.OrdinalIgnoreCase));

			if (existingPanel == null)
			{
				// Create a new Ribbon panel
				RibbonPanelSource panelSource = new RibbonPanelSource
				{
					Title = "Extract"
				};
				RibbonPanel newPanel = new RibbonPanel
				{
					Source = panelSource
				};
				existingTab.Panels.Add(newPanel);

				//For running in application
				string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
				//System.Windows.MessageBox.Show(currentDirectory);

				string exchangeImagePath = System.IO.Path.Combine(currentDirectory, "..\\..\\Resources\\logo2025.png");

				//For running in debug mode
				//string exchangeImagePath = System.IO.Path.GetFullPath("..\\..\\..\\resources\\logo2025.png");

				// Create a new button
				RibbonButton dataExtractorButton = new RibbonButton
				{
					Text = "Data Extractor",
					ShowText = true,
					ShowImage = true,
					Size = RibbonItemSize.Large,
					Orientation = System.Windows.Controls.Orientation.Vertical, // Vertical orientation to place text below the image
				};

				BitmapImage exchangeButtonImage = new BitmapImage(new Uri(exchangeImagePath));
				dataExtractorButton.LargeImage = exchangeButtonImage;
				dataExtractorButton.CommandHandler = new MyButtonCommandHandler();
				panelSource.Items.Add(dataExtractorButton);
			}

		}

	}

	public class MyButtonCommandHandler : System.Windows.Input.ICommand
	{
		public bool IsFirstClick { get; set; } = true;

		DataExtractor DataExtractorWindow;
		public bool CanExecute(object parameter)
		{
			return true;
		}

		public void Execute(object parameter)
		{
			if (!IsFirstClick)
			{
				if (DataExtractorWindow.IsCloseButtonClicked)
				{
					DataExtractorWindow = null;
				}
				else
				{
					return; // Close Button is Not Clicked, Previous window is still open.
				}

			}
			DataExtractorWindow = new DataExtractor();
			DataExtractorWindow.Show();
			IsFirstClick = false;
		}

		public event EventHandler CanExecuteChanged;
	}
}
