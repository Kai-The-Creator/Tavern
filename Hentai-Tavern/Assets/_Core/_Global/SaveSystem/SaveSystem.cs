using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveSystem : Singleton<SaveSystem>
{
    public const string dataArray = "SavesArray";
    public const string dataAutoSave = "AutoSaveSlot";
    public const string GALLERYSAVE = "GallerySaveData";
    public const string SettingsSave = "GameSettingsSaveData";

    public List<string> data = new List<string>();

    private void Start()
    {
        DontDestroyOnLoad(this);

        Init();
    }

    public void Init()
    {
        Debug.Log(GetFilePath(dataArray));

        LoadArray();
    }

    public SaveData GetAutoSave()
    {
        string path = GetFilePath(dataAutoSave);
        SaveData dataSave = null;
        if (File.Exists(path))
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string json = reader.ReadToEnd();
                dataSave = new SaveData();

                dataSave = JsonUtility.FromJson<SaveData>(json);

            }
        }
        return dataSave;
    }

    public void RemoveAutoSlot()
    {
        string path = GetFilePath(dataAutoSave);
        File.Delete(path);
    }

    public List<SaveData> DataList()
    {
        List<SaveData> list = new();

        for (int i = 0; i < this.data.Count; i++)
        {
            int id = i;
            string path = GetFilePath(data[id]);

            if (File.Exists(path))
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    string json = reader.ReadToEnd();
                    SaveData dataSave = new SaveData();

                    dataSave = JsonUtility.FromJson<SaveData>(json);

                    list.Add(dataSave);
                }
            }
        }

        return list;
    }

    /*#region SaveLoadGallery
    public void SaveGallery()
    {
        GallerySaveData dataSave = new GallerySaveData();

        dataSave.CardList = GalleryProvider.instance.CardList;
        dataSave.SceneList = GalleryProvider.instance.SceneList;
        dataSave.ArtList = GalleryProvider.instance.ArtList;

        string json = JsonUtility.ToJson(dataSave);

        string path = GetFilePath(GALLERYSAVE);

        FileStream stream = Stream(path);

        using (StreamWriter writer = new StreamWriter(stream))
        {
            writer.Write(json);
        };
    }

    public void LoadGallery()
    {
        string path = GetFilePath(GALLERYSAVE);

        if (File.Exists(path))
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string json = reader.ReadToEnd();
                GallerySaveData dataSave = new GallerySaveData();

                dataSave = JsonUtility.FromJson<GallerySaveData>(json);

                GalleryProvider.instance.Load(dataSave.CardList, dataSave.SceneList, dataSave.ArtList);
            }
        }
    }
    #endregion*/

    #region SaveSettings

    public void SaveSettings()
    {
        SettingsData dataSave = new SettingsData();

        //dataSave.resolutionID = GlobalClientController.instance.resID;
        dataSave.language = LocalizationManager.Language;
        dataSave.soundsVolume = AudioSystem.instance.GetSoundVolume();
        dataSave.musicVolume = AudioSystem.instance.GetMusicVolume();
        dataSave.fullScr = Screen.fullScreen;

        string json = JsonUtility.ToJson(dataSave);

        string path = GetFilePath(SettingsSave);

        FileStream stream = Stream(path);

        using (StreamWriter writer = new StreamWriter(stream))
        {
            writer.Write(json);
        };

        Debug.Log("Save Settings");
    }

    public void LoadSettings()
    {
        string path = GetFilePath(SettingsSave);

        if (File.Exists(path))
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string json = reader.ReadToEnd();
                SettingsData dataSave = new SettingsData();

                dataSave = JsonUtility.FromJson<SettingsData>(json);

                //GlobalClientController.instance.SetResolution(dataSave.resolutionID);
                LocalizationManager.instance.SetLang(dataSave.language);
                AudioSystem.instance.SetSoundVolume(dataSave.soundsVolume);
                AudioSystem.instance.SetMusicVolume(dataSave.musicVolume);
                Screen.fullScreen = dataSave.fullScr;
            }
        }

        Debug.Log("Load Settings");
    }

    #endregion

    #region Save
    public void SaveGame(int id, bool isAuto = false)
    {
        SaveData dataSave = new SaveData();

        ClearSaveSlot(id, isAuto);

        


        string json = JsonUtility.ToJson(dataSave);

        string path = GetFilePath(!isAuto ? data[id] : dataAutoSave);

        FileStream stream = Stream(path);

        using (StreamWriter writer = new StreamWriter(stream))
        {
            writer.Write(json);
        };
    }

    public void SetAutoSave()
    {
        StartCoroutine(WaitAutoSave());
    }

    private IEnumerator WaitAutoSave()
    {
        yield return new WaitForSeconds(5f);
        SaveGame(0, true);
    }

    

    #endregion

    #region Load
    public void LoadGame(int id, bool isAuto = false)
    {
        string path = GetFilePath(!isAuto ? data[id] : dataAutoSave);

        if (File.Exists(path))
        {
            using (StreamReader reader = new StreamReader(path))
            {
                

                string json = reader.ReadToEnd();
                SaveData dataSave = new SaveData();

                dataSave = JsonUtility.FromJson<SaveData>(json);

                
                if(SceneManager.GetActiveScene().name == GameScenes.Menu.ToString())
                {
                    StartCoroutine(waitLoad(dataSave));
                }
                
            }
        }
    }

    private IEnumerator waitLoad(SaveData dataSave)
    {
        yield return new WaitForSeconds(3f);
    }

   
    #endregion

    private void ClearSaveSlot(int id, bool isAuto)
    {

        string path = GetFilePath(!isAuto ? data[id] : dataAutoSave);
        FileStream stream = Stream(path);

        using (StreamWriter writer = new StreamWriter(stream))
        {
            writer.Write("");
        };
    }

    public void AddNewSlotToArray(bool isAuto = false)
    {
        int id = data.Count;
        string key = $"Slot#{id}";

        if(!isAuto)
            data.Add(key);

        SaveGame(id, isAuto);
        SaveArray();
    }

    public void RemoveSlotFromArray(int id)
    {
        string path = GetFilePath(data[id]);
        data.RemoveAt(id);
        File.Delete(path);
        SaveArray();
    }

    private string GetFilePath(string fileName)
    {
        return Application.persistentDataPath + "/" + fileName;
    }

    private FileStream Stream(string path)
    {
        return new FileStream(path, FileMode.Create);
    }

    public void SaveArray()
    {
        SaveArrayData array = new SaveArrayData();

        array.data = this.data;

        string json = JsonUtility.ToJson(array);

        string path = GetFilePath(dataArray);

        FileStream stream = Stream(path);

        using (StreamWriter writer = new StreamWriter(stream))
        {
            writer.Write(json);
        };
    }

    public void LoadArray()
    {
        string path = GetFilePath(dataArray);

        if (File.Exists(path))
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string json = reader.ReadToEnd();
                SaveArrayData array = new SaveArrayData();

                array = JsonUtility.FromJson<SaveArrayData>(json);

                this.data = array.data;
            }
        }
    }
}
