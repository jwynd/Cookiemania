using System.Collections.Generic;
using System.IO;
using UnityEngine;


//really just a glorified text parser/replacer
public class JumperStoryFramework : MonoBehaviour
{
    #region variables

    //all these definers need to be 2+ characters
    const string commentDefiner = "//";
    const string tutorialDefiner = "TUTORIAL";
    const string endDefiner = "END";
    const string replacePosDefiner = "<REPLACE_POSITIVE>";
    const string replaceNegDefiner = "<REPLACE_NEGATIVE>";
    const int minimumLineLength = 3;
    const int sectionCount = 3;
    //if theres an axis marker, theres also an ending axis marker, which gets disposed of
    const char axisMarker = '<';

    [SerializeField]
    [Tooltip("Requires full path e.g. Assets/Resources/UI/my_text_file.txt")]
    protected string UITextsName = "Assets/JumperResources/UI/JumperUILines.txt";

    //first entry is the text to display, second is the axis the jumpermanagerui is looking for
    protected Queue<System.Tuple<string, string>> tutorialStrings;
    protected Queue<System.Tuple<string, string>> endgameStrings;
    protected Dictionary<string, System.Tuple<string, string>> inputReplacers;

    protected JumperManagerGame jm;


    public static JumperStoryFramework Instance { get; private set; }

    #endregion

    #region startup

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        jm = JumperManagerGame.Instance;
        PopulateInputReplacers(out inputReplacers);
        //want to populate even if we dont use the tutorial
        ReadInTexts(out tutorialStrings, out endgameStrings);
        //once all the texts are finished, disable
        enabled = false;
    }

    private void PopulateInputReplacers(out Dictionary<string, System.Tuple<string, string>> inputReplacers)
    {
        inputReplacers = new Dictionary<string, System.Tuple<string, string>>();
        GetKeysFromInputController(out Dictionary<string, System.Tuple<List<string>, List<string>>> temp);
        foreach (string key in temp.Keys)
        {
            AddOrs(temp[key], out string positive, out string negative);
            //Debug.Log(positive);
            //Debug.Log(negative);
            inputReplacers.Add("<" + key + ">", new System.Tuple<string, string>(positive, negative));
        }
    }

    //puts all positive options into a string, each separated by " or ", same with negative options 
    private void AddOrs(System.Tuple<List<string>, List<string>> keys, out string positive, out string negative)
    {
        OrHelper(keys, out positive);
        OrHelper(keys, out negative);
    }

    private static void OrHelper(System.Tuple<List<string>, List<string>> keys, out string ordKeys)
    {
        string init = " or ";
        ordKeys = "";
        foreach (var key in keys.Item1)
        {
            ordKeys += init;
            ordKeys += key;
        }
        ordKeys = ordKeys.Remove(0, init.Length);
    }

    private void GetKeysFromInputController(out Dictionary<string, System.Tuple<List<string>, List<string>>> buttons)
    {
        buttons = jm.Player.Input.AxesToKeys;
    }


    private void ReadInTexts(out Queue<System.Tuple<string, string>> tutorial, out Queue<System.Tuple<string, string>> ending)
    {
        StreamReader sr = File.OpenText(UITextsName);
        string line;
        //second string is the axis game should look for, for this object OR a string timer (need to convert to float)
        tutorial = new Queue<System.Tuple<string, string>>();
        ending = new Queue<System.Tuple<string, string>>();
        while ((line = sr.ReadLine()) != null)
        {
            string[] sections = line.Split(':');
            foreach (string section in sections)
            {
                section.Trim();
            }
            //dispose of commented lines
            if (Equals(sections[0].Substring(0, commentDefiner.Length), commentDefiner))
            {
                continue;
            }
            //dispose of lines without exact number of sections
            if (sections.Length != sectionCount)
            {
                continue;
            }
            //no empty bs
            if (sections[0].Length < (minimumLineLength - 1) || sections[1] == "" || sections[2] == "")
            {
                continue;
            }
            if (sections[0] == tutorialDefiner)
            {
                tutorial.Enqueue(StaticVariableReplacements(sections, replacePosDefiner, replaceNegDefiner));
            }
            else if (sections[0] == endDefiner)
            {
                //end strings need to be re-evaluated during runtime
                ending.Enqueue(new System.Tuple<string, string>(sections[2], sections[1]));
            }
        }
    }

    private System.Tuple<string, string> StaticVariableReplacements(string[] sections, string posKeyword, string negKeyword)
    {
        System.Tuple<string, string> returnList;
        string axis, displayText;
        //sections required to have min count 3
        axis = sections[1];
        if (axis[0] == axisMarker)
        {
            displayText = ReplaceWords(sections[2], posKeyword, negKeyword, axis);
        }
        else
        {
            displayText = sections[2];
        }
        //need to look through input replacers and replace the appropriate stuff in the
        returnList = new System.Tuple<string, string>(displayText, axis);
        return returnList;
    }

    private string ReplaceWords(string line, string posKeyword, string negKeyword, string axis)
    {
        string[] words = line.Split(' ');
        string returnString = "";
        for (int i = 0; i < words.Length; i++)
        {
            if (Equals(posKeyword, words[i]))
            {
                words[i] = inputReplacers[axis].Item1;
            }
            else if (Equals(negKeyword, words[i]))
            {
                words[i] = inputReplacers[axis].Item2;
            }
        }

        foreach (var word in words)
        {
            returnString += word + " ";
        }
        returnString.Trim();
        return returnString;
    }

    

    #endregion

    //offloads line to whatever calls it, does not keep the line after this

    //order is, first string is the display string, second is the axis to listen to
    public System.Tuple<string, string> GetNextTutorialLine()
    {
        if (tutorialStrings.Count == 0) return null;
        System.Tuple<string, string> temp = tutorialStrings.Dequeue();
        return new System.Tuple<string, string>(temp.Item1, GetAxisName(temp.Item2));
    }

    //actual axis to listen to
    protected string GetAxisName(string axis)
    {
        if (axis == "")
        {
            Debug.LogError("blank axis requested, need to pass an actual axis");
            return "";
        }
        if (axis[0] == axisMarker)
        {
            return axis.Substring(1, axis.Length - 2);          
        }
        return axis;
    }

    //should attempt to use as a timer if not an axis
    public bool IsAnAxis(string axis)
    {
        return jm.Player.Input.Axes.Contains(axis);
    }
}
