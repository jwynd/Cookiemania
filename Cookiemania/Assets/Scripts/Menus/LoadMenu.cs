using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static PlayerDataStatics;

public class LoadMenu : MonoBehaviour
{
    [SerializeField]
    private bool dontDestroy = false;
    [SerializeField]
    private List<Button> saveSlots = null;
    [SerializeField]
    private TMPro.TextMeshProUGUI noSaves = null;
    [SerializeField]
    private Button loadButton = null;
    public List<GameObject> activateOnDestroy;
    public List<MonoBehaviour> enableOnDestroy;

    private string loadSlot = "";
    private bool NoReactivate = false;

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
        // button failed
        if (loadSlot == "")
        {
            return;
        }
        EnableLoadButton(true);
    }

    private void EnableLoadButton(bool v)
    {
        loadButton.gameObject.SetActive(v);
    }

    public void Return()
    {
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
        saveSlots[0].gameObject.SetActive(PlayerPrefs.GetString(P_PREFS_SLOT_1, "") != "");
        saveSlots[0].GetComponent<TMPro.TextMeshProUGUI>().text = 
            PlayerPrefs.GetString(P_PREFS_SLOT_1_NAME, "") + ": $" + 
            PlayerPrefs.GetString(P_PREFS_SLOT_1_MONEY, "");
        saveSlots[1].gameObject.SetActive(PlayerPrefs.GetString(P_PREFS_SLOT_2, "") != "");
        saveSlots[1].GetComponent<TMPro.TextMeshProUGUI>().text =
            PlayerPrefs.GetString(P_PREFS_SLOT_2_NAME, "") + ": $" +
            PlayerPrefs.GetString(P_PREFS_SLOT_2_MONEY, "");
        saveSlots[2].gameObject.SetActive(PlayerPrefs.GetString(P_PREFS_SLOT_3, "") != "");
        saveSlots[2].GetComponent<TMPro.TextMeshProUGUI>().text =
            PlayerPrefs.GetString(P_PREFS_SLOT_3_NAME, "") + ": $" +
            PlayerPrefs.GetString(P_PREFS_SLOT_3_MONEY, "");
        // if the slots all got disabled, there are no loadable games can swap with text
        noSaves.gameObject.SetActive(saveSlots.All(button => !button.gameObject.activeSelf));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Return();
        }
    }


    public void Load()
    {
        // shouldn't be called when loadslot is empty
        if (loadSlot == "")
        {
            EnableLoadButton(false);
            return;
        }
        PlayerPrefs.SetString(P_PREFS_LOAD, loadSlot);
        PlayerPrefs.Save();
        NoReactivate = true;
        SceneManager.LoadScene("Desktop", LoadSceneMode.Single);
    }

    void OnDestroy()
    {
        if (enabled)
            ReactivateObjects();
    }

    private void ReactivateObjects()
    {
        if (NoReactivate) return;
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
