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
    private Main main;
    private Weather weather;
    private Wind wind;

    public Text PlaceName;
    public Text WeatherDesc;
    public Text Min;
    public Text Max;
    public Text Pressure;
    public Text Wspeed;

    public void callApi(InputField query)
    {
        //geoNamesURL = "https://www.geonames.org/search.html?q=" + query + "&country=";
        geoNamesURL = "http://api.geonames.org/geoCodeAddressJSON?q=" + query.text + "&username=" + geoNamesUser;
        //Debug.Log(geoNamesURL);

        //owmURL = "https://api.openweathermap.org/data/2.5/weather?lat=" + owmlat + "&lon=" + owmlon + "&appid=" + owmAPI;
        //Debug.Log(owmURL);
        
        StartCoroutine(SendRequest(geoNamesURL));
     
        //StartCoroutine(SendRequest(owmURL)); //was trying to do it in 2 parts but i think i will make it 1
    }

    IEnumerator SendRequest(string URL)
    {
        UnityWebRequest request = UnityWebRequest.Get(URL);
        
        yield return request.SendWebRequest();
        
        if (request.result != UnityWebRequest.Result.Success) UnityEngine.Debug.Log("Erro de comunicação: "+ request.error);
        else
        {
            root = new Root();
            root = JsonUtility.FromJson<Root>(request.downloadHandler.text);
            UnityEngine.Debug.Log(root.address.lat);
            owmURL = "https://api.openweathermap.org/data/2.5/weather?lat=" + root.address.lat + "&lon=" + root.address.lng + "&units=imperial" + "&appid=" + owmAPI; 
            //go to this address and then replace lat and log, by the geonames fetch
            //https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={API key} 
            //normalized for the example -> https://api.openweathermap.org/data/2.5/weather?lat=38.61313&lon=-78.79882&appid=eff7c51aa691219e52705263d0428f2b
            //last step is to show the weather of that place on screen :)

            request = UnityWebRequest.Get(owmURL);
                
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success) UnityEngine.Debug.Log("Erro de comunicação: "+ request.error);
            else{
                root2 = new Root2();
                root2 = JsonUtility.FromJson<Root2>(request.downloadHandler.text);

                //i tought i needed to do this manually for each class, but i guess i dont have to
                //main = new Main();
                //main = JsonUtility.FromJson<Main>(request2.downloadHandler.text);

                //weather = new Weather();
                //weather = JsonUtility.FromJson<Weather>(request2.downloadHandler.text);

                //wind = new Wind();
                //wind = JsonUtility.FromJson<Wind>(request2.downloadHandler.text);

                PlaceName.text = "The current weather for " + root2.name + " is:";
                WeatherDesc.text = "Description: " + root2.weather[0].main + ", " + root2.weather[0].description;
                Min.text = "Minimum: " + root2.main.temp_min.ToString() + "°F";
                Max.text = "Maximum: " + root2.main.temp_max.ToString() + "°F";
                Pressure.text = "Pressure: " + root2.main.pressure.ToString() + "h Pa.";
                //UnityEngine.Debug.Log(main.pressure);
                Wspeed.text = "Wind Speed: " + root2.wind.speed.ToString() + " " + root2.wind.deg.ToString() + " degrees.";
            }
        }
    }
}