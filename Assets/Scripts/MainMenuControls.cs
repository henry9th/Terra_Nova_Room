using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuControls : MonoBehaviour
{
    public GameObject HomeMenuCanvas; 
    public GameObject MySpaceShipsCanvas;
    public GameObject MotherShipCanvas;

    void Awake() {
        HomeMenuCanvas = GameObject.Find("HomeMenu");
        MySpaceShipsCanvas = GameObject.Find("MySpaceShipsMenu");
        MotherShipCanvas = GameObject.Find("MotherShipMenu");
    }

    // Start is called before the first frame update
    void Start()
    {
        HomeHandler();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HomeHandler()
    {
        //Fetch the score from the PlayerPrefs (set these Playerprefs in another script). If no Int of this name exists, the default is 0.
        //playerLevel_UNLOCKED = PlayerPrefs.GetInt("Score", 0);
        Debug.Log("Home Button hit");
        HomeMenuCanvas.SetActive(true);
        MySpaceShipsCanvas.SetActive(false);
        MotherShipCanvas.SetActive(false);
    }

    public void ConnectWalletHandler()
    {
        //Fetch the score from the PlayerPrefs (set these Playerprefs in another script). If no Int of this name exists, the default is 0.
        //playerLevel_UNLOCKED = PlayerPrefs.GetInt("Score", 0);
        Debug.Log("Connect Wallet Button hit");
    }

    public void MySpaceShipsHandler() { 
        Debug.Log("MySpaceShips Button hit");
        HomeMenuCanvas.SetActive(false);
        MotherShipCanvas.SetActive(false);
        MySpaceShipsCanvas.SetActive(true);
    }

    public void MotherShipHandler() { 
        Debug.Log("Mother Ship Button hit");
        HomeMenuCanvas.SetActive(false);
        MotherShipCanvas.SetActive(true);
        MySpaceShipsCanvas.SetActive(false);
    }

    public void AIAdminHandler() { 
        Debug.Log("AI Admin Button hit");

    }
}
