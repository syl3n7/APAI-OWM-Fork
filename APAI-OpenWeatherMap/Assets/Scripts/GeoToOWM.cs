using System;
using System.Diagnostics;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GeoToOWM : MonoBehaviour
{
    private string geoNamesUser = "example";
    private string owmAPI = "OpenWeatherMapApiKey";
    private Root georoot;
    private Root2 owmroot;
    public Text Error;
    public Text PlaceName;
    public Text WeatherDesc;
    public RawImage WeatherDescImg;
    public Text MinMax;
    public Text Pressure;
    public Text Winddsg;
    public Text Humidity;
    public Text FeelsLike;
    public Text CloudinessVisibility;
    public Text SunriseSunset;

    private void Start()
    {
        Application.targetFrameRate = 60;
        WeatherDescImg.enabled = false;
    }
    public void callApi(InputField query)
    {
        Error.text = "";
        string geoNamesURL = "http://api.geonames.org/geoCodeAddressJSON?q=" + query.text + "&username=" + geoNamesUser;
        StartCoroutine(GeoRequest(geoNamesURL));
    }

    IEnumerator GeoRequest(string URL) //get data from geonames, from placename to lat and lng.
    {
        UnityWebRequest geo = UnityWebRequest.Get(URL);
        yield return geo.SendWebRequest();
        if (geo.result != UnityWebRequest.Result.Success) Error.text = "Communication Error: "+ geo.error;
        else
        {
            georoot = new Root();
            georoot = JsonUtility.FromJson<Root>(geo.downloadHandler.text);
            string OWMURL = "https://api.openweathermap.org/data/2.5/weather?lat=" + georoot.address.lat + "&lon=" + georoot.address.lng + "&units=imperial" + "&appid=" + owmAPI;
            StartCoroutine(GetOWM(OWMURL));
        }
    }
    IEnumerator GetOWM(string owmURL) //get data from owm using lat, lon, units, apikey and display data on objects.
    {
        UnityWebRequest owm = UnityWebRequest.Get(owmURL);
        yield return owm.SendWebRequest();
        if (owm.result != UnityWebRequest.Result.Success) Error.text = "Communication Error: " + owm.error;
        else
        {
            owmroot = new Root2();
            owmroot = JsonUtility.FromJson<Root2>(owm.downloadHandler.text);
            PlaceName.text = "Weather for: " + owmroot.name + ", " + owmroot.sys.country;
            FeelsLike.text = "Feels Like: " + owmroot.main.feels_like.ToString() + "°F";
            WeatherDesc.text = "Conditions: " + owmroot.main.temp.ToString() + "°F  " + owmroot.weather[0].main + ", " + owmroot.weather[0].description;
            string txtr = "https://openweathermap.org/img/w/" + owmroot.weather[0].icon + ".png";
            StartCoroutine(GetTexture(txtr));
            //yield return new WaitForSecondsRealtime(3);
            MinMax.text = "Minimum: " + owmroot.main.temp_min.ToString() + "°F  " + " Maximum: " + owmroot.main.temp_max.ToString() + "°F";
            Pressure.text = "Pressure: " + owmroot.main.pressure.ToString() + "hPa";
            Humidity.text = "Humidity: " + owmroot.main.humidity.ToString() + "%";
            Winddsg.text = "Wind Speed: " + owmroot.wind.speed.ToString() + " mph\n\nDirection: " + owmroot.wind.deg.ToString() + " degrees\n\nGust: " + owmroot.wind.gust.ToString() + " mph";
            CloudinessVisibility.text = "Cloudiness: " + owmroot.clouds.all.ToString() + "%  " + "Visibility: " + owmroot.visibility.ToString() + " meters";
            SunriseSunset.text = "Sunrise: " + ConvertUnixToDateTime(owmroot.sys.sunrise).ToString() + " UTC\n\n" + "Sunset: " + ConvertUnixToDateTime(owmroot.sys.sunset).ToString() + " UTC";
        }
    }

    private static DateTime ConvertUnixToDateTime(long unixtime) //Converts UnixTime into DateTime :)
    {
        DateTime sTime = new DateTime(1970, 1, 1, 0, 0, 0).ToLocalTime();
        return sTime.AddSeconds(unixtime);
    }

    IEnumerator GetTexture(string txtrURL) //get data from the url of the texture and set it to the object.
    {
        UnityWebRequest txtr = UnityWebRequestTexture.GetTexture(txtrURL);
        yield return txtr.SendWebRequest();

        if (txtr.result != UnityWebRequest.Result.Success)
        {
            Error.text = txtr.error;
        }
        else
        {
            WeatherDescImg.texture = ((DownloadHandlerTexture)txtr.downloadHandler).texture;
            WeatherDescImg.enabled = true;
        }
    }
}
