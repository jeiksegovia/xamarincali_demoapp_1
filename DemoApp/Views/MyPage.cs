using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace DemoApp
{
	public class MyPage : ContentPage
	{
		StackLayout _layout;
		TableView table;
		public MyPage()
		{
			Content = _layout = new StackLayout();
			table = new TableView();
			table.Intent = TableIntent.Settings;
			table.HasUnevenRows = true;

			var locationCell = new TextCell {
				Text = "Location",
				TextColor = Color.FromHex(Values.textColor),
			};
			locationCell.Tapped+=async(object sender, EventArgs e) => { 
				LocationPickerPage locationPage = new LocationPickerPage();
				await Navigation.PushAsync(locationPage);
				Debug.WriteLine("asking for location: ");
				var location= await locationPage.getLocation();
				if(location!=null&&location.name!=""){
					locationCell.Text=location.name;
				}
			};

			var locationSection = new TableSection() {
							locationCell,
						};

			table.Root = new TableRoot()
			{
				locationSection
			};

			_layout.Children.Add(table);

		}
	}
}

