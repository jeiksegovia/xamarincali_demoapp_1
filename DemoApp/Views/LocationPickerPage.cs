using System;
using Xamarin.Forms;
using System.Threading.Tasks;

using System.Collections.Generic;
using System.Diagnostics;

namespace DemoApp
{
	public class LocationPickerPage : ContentPage
	{
		
		public TaskCompletionSource<Place> locationData;

		//Views
		RelativeLayout _layout;
		StackLayout content;
		SearchBar search;
		ListView list;

		ActivityIndicator popImaAct;
		Image myLocation;

		public LocationPickerPage()
		{
			_layout = new RelativeLayout();
			Content = _layout;
			locationData = new TaskCompletionSource<Place>();
			this.Title = "Location Picker";

			search = new SearchBar
			{
				Placeholder = "Search places",
			};

			search.SearchButtonPressed += getPlaces;

			var listTitle = new Label
			{
				FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
				Text = "Select the location",
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				HeightRequest = 17,
				HorizontalTextAlignment = TextAlignment.Start,
               	TextColor = Color.Gray,
			};

			//list
			list = new ListView();
			//cellTemplate
			var cellTemplate = new DataTemplate(typeof(PlaceCell));
			cellTemplate.SetBinding(PlaceCell.TextProperty, ".name");
			list.ItemTemplate = cellTemplate;

			// Using ItemTapped
			list.ItemTapped += async (sender, e) =>
			{
				var answer = await DisplayAlert("Selected location", "\"" + ((Place)e.Item).name + "\"", "ok", "Cancel");
				if (answer)
					returnLocationData(((Place)e.Item));
			};

			content = new StackLayout
			{
				Spacing = 0,
				Padding = new Thickness(0, 0, 0, 0),
				Children = { search, listTitle, list, },
			};
			popImaAct = new ActivityIndicator
			{
				IsRunning = true,
				Color = Color.Gray,
				IsVisible = false,
			};

			myLocation = new Image { Source = "ico-mylocation", BackgroundColor= Color.White };

			//using gesture recognizer
			TapGestureRecognizer tapLocation = new TapGestureRecognizer();
			tapLocation.Tapped += myLocationTap;
			myLocation.GestureRecognizers.Add(tapLocation);

			_layout.Children.Add(content,
				Constraint.RelativeToParent((p) =>
				{
					return 0;
				}),
				Constraint.RelativeToParent((p) =>
				{
					return 0;
				}),
				Constraint.RelativeToParent((p) =>
				{
					return p.Width;
				}),
				Constraint.RelativeToParent((p) =>
				{
					return p.Height;
				})
			);

			_layout.Children.Add(myLocation,
				Constraint.RelativeToParent((p) =>
				{
					return 0;
				}),
				Constraint.RelativeToParent((p) =>
				{
					return p.Height-40;
				}),
				Constraint.RelativeToParent((p) =>
				{
					return p.Width;
				}),
				Constraint.RelativeToParent((p) =>
				{
					return 40;
				})
			);

			_layout.Children.Add(popImaAct,
								Constraint.RelativeToParent((p) => { return p.Width / 5 * 2; }),
			                     Constraint.RelativeToParent((p) => { return p.Height / 5 * 2; }),
			                     Constraint.RelativeToParent((p) => { return p.Width / 5; }),
			                     Constraint.RelativeToParent((p) => { return p.Height / 5; })
			);
			
		}

		async void getPlaces(object o, EventArgs e)
		{
			if (search.Text != null)
			{
				Debug.WriteLine("Loc: " + search.Text.ToString());
				popImaAct.IsVisible = true;
				//var remoteClient = new FlyerzClient();
				List<Place> places = await APIServices.getPlaces(search.Text.ToString().Trim());
				//list.ItemsSource = places;

				list.ItemsSource = places;
				popImaAct.IsVisible = false;
			}
		}

		public async Task<Place> getLocation()
		{
			Debug.WriteLine("Waiting for location data");
			var locationResult = await locationData.Task;
			return locationResult;
		}

		async void myLocationTap(object o, EventArgs e)
		{
			//simple animation
			await ((View)o).ScaleTo(0.96, 50, Easing.CubicOut);
			await ((View)o).ScaleTo(1.04, 90, Easing.CubicIn);
			await ((View)o).ScaleTo(1, 60, Easing.CubicOut);
			popImaAct.IsVisible = true;
			Debug.WriteLine("Getting my location:");
			double[] coor = null;//Get the location
			if (coor != null)
			{
				try
				{
					//var remoteClient = new FlyerzClient();
					//List<PlaceDTO> places = await remoteClient.getAddress(coor[0], coor[1]);
					//list.ItemsSource = places;
					popImaAct.IsVisible = false;
				}
				catch (Exception ee)
				{
					Debug.WriteLine("Error: " + ee.Message);
					await DisplayAlert("Error", "", "ok");
					popImaAct.IsVisible = false;
				};
			}else{
				popImaAct.IsVisible = false;
				await DisplayAlert("Location unavailable", "", "ok");
			}

		}

		async void returnLocationData(Place place)
		{
			locationData.TrySetResult(place);
			await Navigation.PopAsync();
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();
		}
	}

	public class PlaceCell : ImageCell
	{
		public PlaceCell()
		{
			this.ImageSource = "ico-pin";
		}	
	}
}


