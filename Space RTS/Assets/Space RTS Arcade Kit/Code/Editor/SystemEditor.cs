using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEditor.SceneManagement;

public class SystemEditor : EditorWindow {

    private const int UNIVERSE_EDITOR_TAB = 0;
    private const int SECTOR_EDITOR_TAB = 1;

    private int currentTab = 0;
    private int previousTab = -1;
    private int selectedIndex = -1;

    [Range(0, 100)]
    private int sectorDifficulty;
    private bool isTakenByPlayer;
    [Range(-50, 50)]
    private int xMapPosition;
    [Range(-50, 50)]
    private int yMapPosition;
    private bool displayExportableObjects = false;

    private string sectorName;

    [MenuItem("Window/Sector Editor")]
    public static void ShowWindow()
    {
        GetWindow(typeof(SystemEditor));
        EditorSceneManager.OpenScene("Assets/Space RTS Arcade Kit/Content/Scenes/SectorEditor.unity");
    }

    void OnGUI()
    {
        currentTab = GUILayout.Toolbar(currentTab, new string[] { "Universe Editor", "Sector Editor" });
        switch (currentTab)
        {
            case UNIVERSE_EDITOR_TAB:
                if (previousTab != currentTab)
                {
                    previousTab = currentTab;
                    UniverseMap.Sectors = SectorLoader.LoadUniverse();
                    EditorSceneManager.OpenScene("Assets/Space RTS Arcade Kit/Content/Scenes/Universe.unity");
                }
                RenderUniverseData();
                break;
            case SECTOR_EDITOR_TAB:
                if (previousTab != currentTab)
                {
                    previousTab = currentTab;
                    EditorSceneManager.OpenScene("Assets/Space RTS Arcade Kit/Content/Scenes/SectorEditor.unity");
                    ClearSector();
                    UpdateSectorFields();
                    SectorLoader.LoadSectorData(sectorName);
                }
                RenderSectorData();
                break;
        }
    }

    private void RenderSectorData()
    {
        /*
        * Find and display export objects
        */
        if (GUILayout.Button("Find objects for export"))
        {
            FindObjectsForExport();
            displayExportableObjects = true;
        }
        if (displayExportableObjects)
        {
            GUILayout.Label("Sector objects found: ", EditorStyles.boldLabel);
            GUILayout.Label("Sun flare: " + sun.name);
            GUILayout.Label("Skybox: " + skybox.name);

            GUILayout.Space(10);

            foreach (var station in stations)
            {
                if (station != null)
                    GUILayout.Label("Station: " + station.name);
            }
            foreach (var gate in jumpgates)
            {
                if (gate != null)
                    GUILayout.Label("Jumpgate: " + gate.name);
            }
            foreach (var field in fields)
            {
                if (field != null)
                    GUILayout.Label("Asteroid Field: " + field.name);
            }
        }

        /*
        * Export and clear
        */
        GUILayout.Space(50);
        GUILayout.Label("EXPORT AND CLEAR SECTOR DATA", EditorStyles.boldLabel);
        if (GUILayout.Button("Clear sector"))
        {
            ClearSector();
        }
        if (GUILayout.Button("EXPORT TO FILE"))
        {
            if (FindObjectsForExport())
            {
                SectorSaver.SaveSectorToFile(stations, jumpgates, fields, GenerateRandomSector.SectorSize, sectorName);
                Debug.Log("Successfully exported sector " + sectorName+" to file");
            }
            else
            {
                Debug.LogError("Cannot export sector until all data is filled in!");
            }
            
            if (sectorName != null)
            {
                UpdateUniverse();
                
            }
        }

        /*
        * Sector data editing
        */
        GUILayout.Space(50);
        if (GUILayout.Button("Generate random sector content"))
        {
            GenerateRandomSector.Randomize();
        }
    }

    private void RenderUniverseData()
    {
        /*
         * Sector list and add sector data
         */
        Color color_default = GUI.backgroundColor;
        Color color_selected = Color.gray;

        GUIStyle itemStyle = new GUIStyle(GUI.skin.button);  // make a new GUIStyle

        itemStyle.alignment = TextAnchor.MiddleLeft; // align text to the left
        itemStyle.active.background = itemStyle.normal.background;  // gets rid of button click background style.
        itemStyle.margin = new RectOffset(0, 0, 0, 0); //removes the space between items (previously there was a small gap between GUI which made it harder to select a desired item)

        List<string> sectors = new List<string>(UniverseMap.Sectors.Keys);
        sectors.Add("Add New Sector");
        for (int i = 0; i < sectors.Count; i++)
        {
            GUI.backgroundColor = (selectedIndex == i) ? color_selected : Color.clear;

            //show a button using the new GUIStyle
            if (GUILayout.Button(sectors[i], itemStyle))
            {
                selectedIndex = i;
                sectorName = sectors[i];
                UpdateSectorFields();

                continue;
            }

            GUI.backgroundColor = color_default; //this is to avoid affecting other GUIs outside of the list
        }

        /*
         * Sector data
         */
        if (selectedIndex != -1)
        {
            GUILayout.Space(50);
            GUILayout.Label("SECTOR OPTIONS", EditorStyles.boldLabel);
            sectorName = EditorGUILayout.TextField("Sector name:", sectorName);
            sectorDifficulty = EditorGUILayout.IntField("Sector difficulty e[0, 100]:", sectorDifficulty);
            xMapPosition = EditorGUILayout.IntField("Sector X position on map e[-50, 50]", xMapPosition);
            yMapPosition = EditorGUILayout.IntField("Sector Y position on map e[-50, 50]", yMapPosition);

            if (selectedIndex < sectors.Count - 1)
            {
                var selectedSector = UniverseMap.Sectors[sectors[selectedIndex]];

                selectedSector.Name = sectorName;
                selectedSector.Difficulty = sectorDifficulty;
                selectedSector.SectorPosition.x = xMapPosition;
                selectedSector.SectorPosition.y = yMapPosition;
            }

            if (GUILayout.Button("Continue"))
            {
                previousTab = -1;
                currentTab = SECTOR_EDITOR_TAB;
            }
            GUILayout.Space(100);
            if (GUILayout.Button("Delete sector"))
            {
                File.Delete(SectorLoader.SECTORS_ROOT + sectorName);
                Debug.Log("Sector " + sectorName + " deleted successfully");
            }
        }
    }

    private Flare sun = null;
    private Material skybox = null;
    private GameObject[] stations, jumpgates, fields;

    private bool FindObjectsForExport()
    {
        // Validate fields
        if (sectorName == "" || sectorDifficulty < 0 || (xMapPosition == 0 & yMapPosition == 0))
            return false;

        // Get sun flare
        sun = GameObject.FindGameObjectWithTag("Sun").GetComponent<Light>().flare;
        // Get skybox
        skybox = RenderSettings.skybox;

        // Get stations
        stations = GameObject.FindGameObjectsWithTag("Station");
        // Get jumpgates
        jumpgates = GameObject.FindGameObjectsWithTag("Jumpgate");
        // Get asteroid fields
        fields = GameObject.FindGameObjectsWithTag("AsteroidField");

        return true;
    }

    private void ClearSector()
    {
        // Clear sector
        GameObject[] objects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        for (int i=0; i<objects.Length; i++)
        {
            if (objects[i].tag != "Sun" && objects[i].tag != "MainCamera") { 
                GameObject.DestroyImmediate(objects[i]);
            }
        }
    }

    private void UpdateUniverse()
    {
        // Load sectors
        Dictionary<string, SerializableUniverseSector> universeSectors = UniverseMap.Sectors = SectorLoader.LoadUniverse();
       
        // Update or create
        if (universeSectors.ContainsKey(sectorName))
        {
            // Update
            var updatedSector = universeSectors[sectorName];
            updatedSector.Difficulty = sectorDifficulty;
            updatedSector.SectorPosition = new SerializableVector2(xMapPosition, yMapPosition);
        }
        else
        {
            // Create
            var createdSector = new SerializableUniverseSector(sectorName, xMapPosition, yMapPosition, sectorDifficulty);
            universeSectors[sectorName] = createdSector;
        }

        // Find existing sector files
        List<string> sectorNames = new List<string>(Directory.GetFiles(SectorLoader.SECTORS_ROOT));
        for(int i=0; i<sectorNames.Count; i++)
        {
            sectorNames[i] = Path.GetFileName(sectorNames[i]);
        }

        // Delete non existing sectors
        List<string> universeSectorNames = new List<string>(universeSectors.Keys);
        foreach(string sector in universeSectorNames)
        {
            if (!sectorNames.Contains(sector))
            {
                universeSectors.Remove(sector); // Because there is no sector file for this descriptor
            }
        }

        // Save sectors
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(SaveGame.PERSISTANCE_PATH + "Data/Universe", FileMode.OpenOrCreate);
        formatter.Serialize(stream, universeSectors);
        stream.Close();
    }

    private void UpdateSectorFields()
    {
        if(UniverseMap.Sectors.ContainsKey(sectorName))
        {
            SerializableUniverseSector sector = UniverseMap.Sectors[sectorName];
            sectorName = sector.Name;
            sectorDifficulty = sector.Difficulty;
            xMapPosition = (int)sector.SectorPosition.x;
            yMapPosition = (int)sector.SectorPosition.y;
        }
    }

}
