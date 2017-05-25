using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;

namespace DemoApp
{
	public static class APIServices
	{
		static string baseUrl = "https://maps.googleapis.com/maps/api/";
		static string placesApiKey = "AIzaSyC9klL_XkoCiJJqgk3qU9i4pZ0jl4b9GC8";

		static HttpClient createClient() {
			HttpClient httpClient = new HttpClient()
			{
				BaseAddress = new Uri(baseUrl),
				Timeout = new TimeSpan(0,0,0,30),
			};
			httpClient.DefaultRequestHeaders.Accept.Clear();
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			Debug.WriteLine("->httpClient created!!");
			return httpClient;
		}
		public static async Task<List<Place>> getPlaces(string query)
		{
			using (var httpClient = createClient())
			{
				//string url= "geocode/json?latlng=" + lat.ToString().Replace(",", ".") + "," + lng.ToString().Replace(",", ".") + "&key=" +placesApiKey;
				string url = "place/textsearch/json?&key=" + placesApiKey + "&query=" + System.Net.WebUtility.UrlEncode(query);

				var response = await httpClient.GetAsync(url).ConfigureAwait(false);
				if (response.IsSuccessStatusCode)
				{
					var rawJson = await response.Content.ReadAsStringAsync();
					var jsonObj = JsonConvert.DeserializeObject<JObject>(rawJson);
					JToken results;
					if (jsonObj.TryGetValue("results", out results))
					{
						List<Place> placesList = new List<Place>();
							foreach (var place in results)
							{
								try
								{
									placesList.Add(new Place()
									{
										address = (string)place["formatted_address"],
										name = (string)place["name"] + " - " + (string)place["formatted_address"],
										latitude = (double)place["geometry"]["location"]["lat"],
										longitude = (double)place["geometry"]["location"]["lng"],
									});
								}
								catch (Exception e)
								{
									Debug.WriteLine("mapping error:" + e.Message);
									Debug.WriteLine(" -" + place.ToString());
								}
							};
						
						return placesList;
					};
				}
				else
				{
					Debug.WriteLine("Error status code: " + response.StatusCode);
					Debug.WriteLine("http content: " + response.Content.ToString());
				}
			}
			return null;
			}	
	}
}
