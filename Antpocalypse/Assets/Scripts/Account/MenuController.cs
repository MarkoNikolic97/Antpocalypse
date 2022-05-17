using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject mainScreenObject, optionsScreenObject, classSelectScreenObject, mapScreenObject, accountScreenObject;

    public static string ChosenClass;
    public static string ChosenMap; 
    public static int MapSize;

    MapData[] maps;
    // Start is called before the first frame update
    void Start()
    {
        ChosenClass = "invalid";
        MapSize = 1;

        MapData map1 = new MapData(); map1.Name = "Grayscale"; map1.Description = "Simple Gray Map";
        MapData map2 = new MapData(); map2.Name = "..Soon.."; map2.Description = "Coming Soon.....";
        maps = new MapData[2];
        maps[0] = map1;
        maps[1] = map2;

        mapIndex = 0;
        UpdateMapInfoToUI(maps[mapIndex]);



        accountScreenObject.SetActive(true);
        mainScreenObject.SetActive(false);
        classSelectScreenObject.SetActive(false);
        mapScreenObject.SetActive(false);
        optionsScreenObject.SetActive(false);
        


    }
    public void ClearMainMenu()
    {
        mainScreenObject.SetActive(false);
        classSelectScreenObject.SetActive(false);
        mapScreenObject.SetActive(false);
        optionsScreenObject.SetActive(false);

    }

    public void LoadMainMenu()
    {
        mainScreenObject.SetActive(true);
        classSelectScreenObject.SetActive(false);
        mapScreenObject.SetActive(false);
        optionsScreenObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Button startGameButton = mapScreenObject.transform.GetChild(3).GetComponent<Button>();
        if (mapIndex > 0)
            startGameButton.interactable = false;
        else
            startGameButton.interactable = true;

        Button classSelectButton = classSelectScreenObject.transform.GetChild(3).GetComponent<Button>();
        if (ChosenClass.Equals("invalid"))
            classSelectButton.interactable = false;
        else
            classSelectButton.interactable = true;
    }

    

    #region MainMenu

    public void MainMenu_Start()
    {
        mainScreenObject.SetActive(false);
        classSelectScreenObject.SetActive(true);

    }

    public void MainMenu_Options()
    {
        Debug.Log("Not Implemented");
    }

    public void MainMenu_Shop()
    {
        Debug.Log("Not Implemented");
    }

    #endregion


    #region ClassSelect

    public void ClassSelect_SelectClass(string className) // Class Select OnClick
    {
        Class chosenclass = null;
        Debug.Log("Class: " + className);

        if (className.Equals("Elementalist"))
        {
            chosenclass = new ElementalistClass();
        }
        else if (className.Equals("Commando"))
        {
            chosenclass = new CommandoClass();
        }
        

        TextMeshProUGUI nameField = classSelectScreenObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI descriptionField = classSelectScreenObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        if (chosenclass == null)
        {
            nameField.text = "Coming Soon...";
            descriptionField.text = "";
            ChosenClass = "invalid";

            return;
        }

        nameField.text = chosenclass.Name;
        descriptionField.text = chosenclass.Description;

        ChosenClass = className;
    }

    public void ClassSelect_Select() // SelectButton
    {
        if (ChosenClass.Equals(""))
        {
            return;
        }

        classSelectScreenObject.SetActive(false);
        mapScreenObject.SetActive(true);

    }

    public void ClassSelect_Back()
    {
        mainScreenObject.SetActive(true);
        classSelectScreenObject.SetActive(false);
    }



    #endregion




    #region MapSelect
    public struct MapData
    {
        public string Name, Description;
        public string mapIconPath;
    }

    int mapIndex = 0;

    string[] mapSizes = { "Small", "Medium", "Large" };
    int sizeIndex = 1;

    public void MapSelect_NextMap()
    {
        mapIndex = Mathf.Clamp(mapIndex + 1, 0, 1);
        UpdateMapInfoToUI(maps[mapIndex]);

        

    }

    public void MapSelect_PreviousMap()
    {
        mapIndex = Mathf.Clamp(mapIndex - 1, 0, 1);
        UpdateMapInfoToUI(maps[mapIndex]);
    }

    public void UpdateMapInfoToUI(MapData data)
    {
        TextMeshProUGUI mapName = mapScreenObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI mapDescription = mapScreenObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        Image mapImageIcon = mapScreenObject.transform.GetChild(1).GetComponent<Image>();

        mapName.text = data.Name;
        mapDescription.text = data.Description;

        ChosenMap = data.Name;
        /// SET MAP IMAGE TO THE ACCORDING ICON

    }

    public void MapSelect_Play()
    {

        SceneManager.LoadScene("MainScene");
        Debug.Log("Start Game");
    }

    public void MapSelect_Back()
    {
        classSelectScreenObject.SetActive(true);
        mapScreenObject.SetActive(false);
    }

    public void LargerMap()
    {
        TextMeshProUGUI mapSizeField = mapScreenObject.transform.GetChild(6).GetComponent<TextMeshProUGUI>();
        sizeIndex = Mathf.Clamp(sizeIndex + 1, 0, 2);

        mapSizeField.text = mapSizes[sizeIndex];
        MapSize = sizeIndex;
    }

    public void SmallerMap()
    {
        TextMeshProUGUI mapSizeField = mapScreenObject.transform.GetChild(6).GetComponent<TextMeshProUGUI>();
        sizeIndex = Mathf.Clamp(sizeIndex - 1, 0, 2);

        mapSizeField.text = mapSizes[sizeIndex];
        MapSize = sizeIndex;
    }

    #endregion

}
