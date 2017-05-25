using System;
using CoreLocation;
using UIKit;
using Xamarin.Forms;
using Flyerz.iOS;
using Foundation;
using System.Threading.Tasks;


[assembly: Dependency (typeof (LocationManagerIOS))]
namespace Flyerz.iOS
{
	public class LocationManagerIOS//:ILocationManager
	{
		CLLocationManager iPhoneLocationManager=null;
		double Latitude;
		double Longitude;
		bool updated=false;
		bool denied=false;
		string status="";
		public void init(){
			// initialize our location manager and callback handler
			iPhoneLocationManager = new CLLocationManager ();
			//Console.WriteLine ("iPhoneLocationManager created");
			// uncomment this if you want to use the delegate pattern:
			//locationDelegate = new LocationDelegate (mainScreen);
			//iPhoneLocationManager.Delegate = locationDelegate;

			// you can set the update threshold and accuracy if you want:
			//iPhoneLocationManager.DistanceFilter = 10; // move ten meters before updating
			//iPhoneLocationManager.HeadingFilter = 3; // move 3 degrees before updating

			// you can also set the desired accuracy:
			//iPhoneLocationManager.DesiredAccuracy = 1000; // 1000 meters
			// you can also use presets, which simply evalute to a double value:
			iPhoneLocationManager.DesiredAccuracy = CLLocation.AccuracyKilometer;

			//iPhoneLocationManager.PausesLocationUpdatesAutomatically = true;

			// handle the updated location method and update the UI
			if (UIDevice.CurrentDevice.CheckSystemVersion (6,0)) {
				iPhoneLocationManager.LocationsUpdated += (object sender, CLLocationsUpdatedEventArgs e) => {
					UpdateLocation ( e.Locations [e.Locations.Length - 1]);
				};
				//Console.WriteLine ("updated location set ios 6 ");
			} else {
				#pragma warning disable 618
				// this won't be called on iOS 6 (deprecated)
				iPhoneLocationManager.UpdatedLocation += (object sender, CLLocationUpdatedEventArgs e) => {
					UpdateLocation ( e.NewLocation);
				};
				//Console.WriteLine ("updated location set ios 7+");
				#pragma warning restore 618
			}

			iPhoneLocationManager.AuthorizationChanged+= (object sender,CLAuthorizationChangedEventArgs e) => {AuthorizationUpdated(e);};

				
			// start updating our location.
			if (CLLocationManager.LocationServicesEnabled)iPhoneLocationManager.StartUpdatingLocation ();
//			if (CLLocationManager.HeadingAvailable)iPhoneLocationManager.StartUpdatingHeading ();
		}

		void stopUpdating(){
			if(iPhoneLocationManager!=null)
			iPhoneLocationManager.StopUpdatingLocation ();
			//iPhoneLocationManager.StopUpdatingHeading ();
		}

		public void UpdateLocation (CLLocation newLocation)
		{
			Latitude = newLocation.Coordinate.Latitude;
			Longitude =newLocation.Coordinate.Longitude;
			updated = true;
			//newLocation.Altitude;
			//newLocation.Course;
			//newLocation.Speed;
			// get the distance from here to paris
			//(newLocation.DistanceFrom(new CLLocation(48.857, 2.351)) / 1000);
		}

		public async Task<double[]> getLocation(){
			if (iPhoneLocationManager == null)init ();
			while ((updated == false)&& denied==false) {
				await Task.Delay (5);
			}
			if (CLLocationManager.LocationServicesEnabled) {

				if (updated == false) { Console.WriteLine ("location not updated");}

				if (iPhoneLocationManager.Location != null && denied==false) {
					double[] coor = new double[2];
					coor [0] = iPhoneLocationManager.Location.Coordinate.Latitude;
					coor [1] = iPhoneLocationManager.Location.Coordinate.Longitude;
					return coor;
				} else {
					Console.WriteLine ("Location empty");
					return null;
				}
			} else {
				Console.WriteLine ("Location Disable");
			}
			return null;
		}

		private void AuthorizationUpdated(CLAuthorizationChangedEventArgs e){
			Console.WriteLine("Loc Autorization change: "+e.Status);
			status = e.Status.ToString ();
			if (e.Status.ToString() == "Denied") {
				
				UIAlertView alert = new UIAlertView () { 
					Title = "Location not available", Message = "Please enable your location from Settings"
				};
				alert.AddButton("ok");
				alert.AddButton("Settings");
				alert.Clicked += async(object sender, UIButtonEventArgs b) => {
					if(b.ButtonIndex==1){
						UIApplication.SharedApplication.OpenUrl(new NSUrl(UIApplication.OpenSettingsUrlString));
						int i=0;
						while(i<10 && updated==false){
							await Task.Delay(1000);
							i++;
						}
						if(status!="AuthorizedWhenInUse"){denied = true;Console.WriteLine("assum denied");}else{return;}
					}
					if(b.ButtonIndex==0){
					denied = true;
					}
				};
				alert.Show ();
			}
			if (e.Status.ToString () == "AuthorizedWhenInUse") {
				denied = false;
				if (CLLocationManager.LocationServicesEnabled&iPhoneLocationManager!=null)iPhoneLocationManager.StartUpdatingLocation ();
			}
			if (e.Status.ToString () == "NotDetermined") {
				//iOS 8 requires you to manually request authorization now - Note the Info.plist file has a new key called requestWhenInUseAuthorization added to.
				if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0)&iPhoneLocationManager!=null)
				{
					iPhoneLocationManager.RequestWhenInUseAuthorization ();
					System.Console.WriteLine ("Ask location Authorization");
				}
			}
		}

		public void dispose(){
			stopUpdating ();
			if (iPhoneLocationManager != null) {
				iPhoneLocationManager.Dispose ();
				iPhoneLocationManager = null;
			}
		}

	}
}

