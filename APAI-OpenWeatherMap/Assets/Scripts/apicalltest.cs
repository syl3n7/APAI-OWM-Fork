using System;
using System.Diagnostics;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class apicalltest : MonoBehaviour
{
    private string endereco;
    private string geoNamesURL;
    private string geoNamesUser = "syl3n7";
    private string owmURL;
    private string owmAPI = "eff7c51aa691219e52705263d0428f2b";
    private Root root;
    private Root2 root2;
    public Text PlaceName;
    public Text WeatherDesc;
    public Text Min;
    public Text Max;
    public Text Pressure;
    public Text Wspeed;
    public Text Error;

    public void callApi(InputField query)
    {
        geoNamesURL = "http://api.geonames.org/geoCodeAddressJSON?q=" + query.text + "&username=" + geoNamesUser;
        StartCoroutine(SendRequest(geoNamesURL));
    }

    IEnumerator SendRequest(string URL)
    {
        UnityWebRequest request = UnityWebRequest.Get(URL);
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success) PlaceName.text = "Communication Error: "+ request.error;
        else
        {
            root = new Root();
            root = JsonUtility.FromJson<Root>(request.downloadHandler.text);
            owmURL = "https://api.openweathermap.org/data/2.5/weather?lat=" + root.address.lat + "&lon=" + root.address.lng + "&units=imperial" + "&appid=" + owmAPI;
            request = UnityWebRequest.Get(owmURL);
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success) PlaceName.text = "Communication Error: "+ request.error;
            else{
                root2 = new Root2();
                root2 = JsonUtility.FromJson<Root2>(request.downloadHandler.text);
                PlaceName.text = "The current weather for " + root2.name + " is:";
                WeatherDesc.text = "Description: " + root2.weather[0].main + ", " + root2.weather[0].description;
                Min.text = "Minimum: " + root2.main.temp_min.ToString() + "°F";
                Max.text = "Maximum: " + root2.main.temp_max.ToString() + "°F";
                Pressure.text = "Pressure: " + root2.main.pressure.ToString() + "h Pa.";
                Wspeed.text = "Wind Speed: " + root2.wind.speed.ToString() + " mph, " + root2.wind.deg.ToString() + " degrees.";
            }
        }
    }
}