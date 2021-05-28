using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static PlayerDataStatics;

public class SaveMenu : MonoBehaviour
{
    [SerializeField]
    private bool dontDestroy = false;
    [SerializeField]
    private List<Button> saveSlots = null;
    [SerializeField]
    private Button loadButton = null;
    public List<GameObject> activateOnDestroy;
    public List<MonoBehaviour> enableOnDestroy;

    private string loadSlot = "";
    private bool overwriting = false;
    private int slotNumber = 0;

    // readies a file to load and opens the confirm menu
    public void ReadyOpen(int i)
    {
        switch (i)
        {
            case 1:
                loadSlot = PlayerPrefs.GetString(P_PREFS_SLOT_1, "");
                break;
            case 2:
                loadSlot = PlayerPrefs.GetString(P_PREFS_SLOT_2, "");
                break;
            case 3:
                loadSlot = PlayerPrefs.GetString(P_PREFS_SLOT_3, "");
                break;
            default:
                break;
        }
        slotNumber = i;
        overwriting = loadSlot != "";
        if (loadSlot == "")
        {
            loadSlot = "Save" + i;
        }
        EnableLoadButton(true);
    }

    private void EnableLoadButton(bool v)
    {
        loadButton.gameObject.SetActive(v);
        if (v)
            loadButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text =
                overwriting ? "OVERWRITE" : "SAVE";
    }

    public void Return()
    {
        if (!enabled) return;
        loadSlot = "";
        overwriting = false;
        slotNumber = 0;
        if (dontDestroy)
        {
            enabled = false;
            gameObject.SetActive(false);
            return;
        }
        Destroy(gameObject);
    }

    private void Awake()
    {
        EnableLoadButton(false);
        SetSaveData();
    }

    private void SetSaveData()
    {
        if (PlayerPrefs.GetString(P_PREFS_SLOT_1, "") != "")
            saveSlots[0].GetComponent<TMPro.TextMeshProUGUI>().text =
                PlayerPrefs.GetString(P_PREFS_SLOT_1_NAME, "") + ": $" +
                PlayerPrefs.GetString(P_PREFS_SLOT_1_MONEY, "");
        if (PlayerPrefs.GetString(P_PREFS_SLOT_2, "") != "")
            saveSlots[1].GetComponent<TMPro.TextMeshProUGUI>().text =
                PlayerPrefs.GetString(P_PREFS_SLOT_2_NAME, "") + ": $" +
                PlayerPrefs.GetString(P_PREFS_SLOT_2_MONEY, "");
        if (PlayerPrefs.GetString(P_PREFS_SLOT_3, "") != "")
            saveSlots[2].GetComponent<TMPro.TextMeshProUGUI>().text =
                PlayerPrefs.GetString(P_PREFS_SLOT_3_NAME, "") + ": $" +
                PlayerPrefs.GetString(P_PREFS_SLOT_3_MONEY, "");
    }

    private void Start()
    {
        InputAxes.Instance.Escape.started += delegate { Return(); };
    }

    public void Save()
    {
        // shouldn't be called when loadslot is empty
        if (loadSlot == "")
        {
            EnableLoadButton(false);
            return;
        }
        // show that game is saving with some kinda overview thingy?
        SaveSystem.Save(loadSlot, slotNumber);
        EnableLoadButton(false);
        SetSaveData();
        Return();
    }

    void OnDestroy()
    {
        if (enabled)
            ReactivateObjects();
    }

    private void ReactivateObjects()
    {
        foreach (GameObject g in activateOnDestroy)
        {
            g.SetActive(true);
        }
        foreach (MonoBehaviour g in enableOnDestroy)
        {
            g.enabled = true;
        }
    }

    private void OnDisable()
    {
        ReactivateObjects();
    }
}
