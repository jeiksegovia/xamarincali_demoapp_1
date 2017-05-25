using System;

using Xamarin.Forms;

namespace DemoApp
{
	public class LocationPickerPage : ContentPage
	{
		public LocationPickerPage()
		{
			Content = new StackLayout
			{
				Children = {
					new Label { Text = "Hello ContentPage" }
				}
			};
		}
	}
}

