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
    private string owmlat;
    private string owmlon;
    private string owmAPI = "eff7c51aa691219e52705263d0428f2b";
    
    private Root root;
    private Root2 root2;
    private Main main;

    public Text textArea;

    public void callApi(UnityEngine.UI.InputField query)
    {
        //geoNamesURL = "https://www.geonames.org/search.html?q=" + query + "&country=";
        geoNamesURL = "http://api.geonames.org/geoCodeAddressJSON?q=" + query + "&username=" + geoNamesUser;
        //Debug.Log(geoNamesURL);

        //owmURL = "https://api.openweathermap.org/data/2.5/weather?lat=" + owmlat + "&lon=" + owmlon + "&appid=" + owmAPI;
        //Debug.Log(owmURL);
        
        StartCoroutine(SendRequest(geoNamesURL));
     
        //StartCoroutine(SendRequest(owmURL)); //was trying to do it in 2 parts but i think i will make it 1
    }

    IEnumerator SendRequest(string gURL)
    {
        UnityWebRequest request = UnityWebRequest.Get(gURL);
        
        yield return request.SendWebRequest();
        
        if (request.result != UnityWebRequest.Result.Success) UnityEngine.Debug.Log("Erro de comunicação: "+ request.error);
        else
        {
            root = new Root();
            root = JsonUtility.FromJson<Root>(request.downloadHandler.text);
            owmlat = root.address.lat;
            owmlon = root.address.lng;
            
            owmURL = "https://api.openweathermap.org/data/2.5/weather?lat=" + owmlat + "&lon=" + owmlon + "&appid=" + owmAPI; 
            //go to this address and then replace lat and log, by the geonames fetch
            //https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={API key} 
            //normalized for the example -> https://api.openweathermap.org/data/2.5/weather?lat=38.61313&lon=-78.79882&appid=eff7c51aa691219e52705263d0428f2b
            //last step is to show the weather of that place on screen :)

            UnityWebRequest request2 = UnityWebRequest.Get(owmURL);
                
            yield return request2.SendWebRequest();

            if (request2.result != UnityWebRequest.Result.Success) UnityEngine.Debug.Log("Erro de comunicação: "+ request2.error);
            else{
                root2 = new Root2();
                root2 = JsonUtility.FromJson<Root2>(request2.downloadHandler.text);
                main = new Main();
                main = JsonUtility.FromJson<Main>(request2.downloadHandler.text);
                textArea.text = "Current Weather for " + root2.name + " is:/n minimum of: " + main.temp_min + " and a maximum of: " + main.temp_max;
            }
        }
    }
}