using System.Collections.Generic;
using System.IO;
using UnityEngine;


//really just a glorified text parser/replacer
public class JumperStoryFramework : MonoBehaviour
{
    #region variables

    [SerializeField]
    [Tooltip("Requires full path e.g. Assets/Resources/UI/my_text_file.txt")]
    protected string UITextsName = "Assets/JumperResources/UI/JumperUILines.txt";

    protected HashSet<string> axisNameList;
    //first entry is the text to display, second is the axis the jumpermanagerui is looking for
    protected Queue<System.Tuple<string, string>> tutorialStrings;
    protected Queue<System.Tuple<string, string>> endgameStrings;
    protected Dictionary<string, System.Tuple<string, string>> inputReplacers;
    protected char axisMarker = '<';
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
        PopulateAxisSet(jm.player.GetComponent<JumperPlayerController>());
        PopulateInputReplacers(out inputReplacers);
        //want to populate even if we dont use the tutorial
        ReadInTexts(out tutorialStrings, out endgameStrings);
    }

    private void PopulateAxisSet(JumperPlayerController jp)
    {
        axisNameList = new HashSet<string> { jp.GetHorizontalAxis(), jp.GetJumpAxis(), jp.GetPickupAxis(), jp.GetThrowAxis() };
    }

    private void PopulateInputReplacers(out Dictionary<string, System.Tuple<string, string>> inputReplacers)
    {
        inputReplacers = new Dictionary<string, System.Tuple<string, string>>();
        //want to always have 4 buttons, labeled statically, if "" no button defined for that action
        //maps to 0-3 ----> positive, negative, alt positive, alt negative
        //if buttons are more than 4, multiple axes with same name, use modulo 4 for repeats of pos/neg/altpos/altneg
        //use modulo 2 to get all repeat options for positive and negatives
        Dictionary<string, string[]> buttons;

        GetKeysFromAxis(axisNameList, out buttons);
        foreach (string key in buttons.Keys)
        {
            PullPositiveNegativeButtons(buttons[key], out string positive, out string negative);
            //Debug.Log(positive);
            //Debug.Log(negative);
            inputReplacers.Add("<" + key + ">", new System.Tuple<string, string>(positive, negative));
        }
    }

    //puts all positive options into a string, each separated by " or ", same with negative options 
    private void PullPositiveNegativeButtons(string[] v, out string positive, out string negative)
    {
        positive = "";
        negative = "";
        string button = v[0];
        if (button != "")
        {
            positive += PullPositiveOrNegativeHelper(button);
        }
        button = v[1];
        if (button != "")
        {
            negative += PullPositiveOrNegativeHelper(button);
        }
        for (int i = 2; i < v.Length; i++)
        {
            button = v[i];
            if (button == "")
                continue;
            if (i % 2 == 1)
            {
                negative += " or " + PullPositiveOrNegativeHelper(button);
            }
            else
            {
                positive += " or " + PullPositiveOrNegativeHelper(button);
            }
        }
    }

    //optional --> puts quotes around single letter input button descriptions
    private string PullPositiveOrNegativeHelper(string button)
    {
        if (button.Length == 1)
        {
            return "\"" + button + "\"";
        }
        else
        {
            return button;
        }
    }

    //credit : https://stackoverflow.com/questions/40231499/how-do-i-get-the-keycode-currently-assigned-to-the-input-manager
    //order of buttons is ---> positive, negative, alt positive, alt negative
    private void GetKeysFromAxis(HashSet<string> axisName, out Dictionary<string, string[]> buttons)
    {
        var inputManager = UnityEditor.AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0];
        UnityEditor.SerializedObject obj = new UnityEditor.SerializedObject(inputManager);
        UnityEditor.SerializedProperty axisArray = obj.FindProperty("m_Axes");
        buttons = new Dictionary<string, string[]>();
        if (axisArray.arraySize < axisName.Count)
            Debug.LogWarning("Not enough axes");

        for (int i = 0; i < axisArray.arraySize; ++i)
        {
            string[] buttonCandidates = new string[4];
            int offset = 0;
            var axis = axisArray.GetArrayElementAtIndex(i);
            var name = axis.displayName;      //axis.displayName  "Horizontal"  string
            if (!axisName.Contains(name)) //only looking for specific axes
            {
                continue;
            }
            if (buttons.ContainsKey(name)) //only want one entry per key
            {
                offset += buttons[name].Length;
                AssignToArray(buttons[name], 4, out buttonCandidates);
            }
            axis.Next(true);   //axis.displayName      "Name"  string
            axis.Next(false);      //axis.displayName  "Descriptive Name"  string
            axis.Next(false);      //axis.displayName  "Descriptive Negative Name" string
            axis.Next(false);      //axis.displayName  "Negative Button"   string
            buttonCandidates[1 + offset] = axis.stringValue;
            axis.Next(false);      //axis.displayName  "Positive Button"   string
            buttonCandidates[0 + offset] = axis.stringValue;
            axis.Next(false);      //axis.displayName  "Positive Button"   string
            buttonCandidates[3 + offset] = axis.stringValue;
            axis.Next(false);      //axis.displayName  "Positive Button"   string
            buttonCandidates[2 + offset] = axis.stringValue;
            // Debug.Log(name);
            // Debug.Log(buttonCandidates[0] + " " + buttonCandidates[1] + " " + buttonCandidates[2] + " " + buttonCandidates[3]);
            if (offset != 0)
            {
                buttons[name] = buttonCandidates;
                //Debug.Log("added new stuff: " + buttonCandidates[4] + " " + buttonCandidates[5] + " " + buttonCandidates[6] + " " + buttonCandidates[7]);
            }
            else
            {
                buttons.Add(name, buttonCandidates);
            }
        }
    }

    private void AssignToArray(string[] v, int extraSize, out string[] buttonCandidates)
    {
        buttonCandidates = new string[v.Length + extraSize];
        for (int i = 0; i < v.Length; i++)
        {
            buttonCandidates[i] = v[i];
        }
    }

    private void ReadInTexts(out Queue<System.Tuple<string, string>> tutorial, out Queue<System.Tuple<string, string>> ending)
    {
        StreamReader sr = File.OpenText(UITextsName);
        string line;
        int sectionCount = 3;
        //all these definers need to be 2+ characters
        string commentDefiner = "//";
        string tutorialDefiner = "TUTORIAL";
        string endDefiner = "END";
        string replacePosDefine = "<REPLACE_POSITIVE>";
        string replaceNegDefine = "<REPLACE_NEGATIVE>";
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
            //dispose of lines without all the sections defined
            if (sections.Length < sectionCount)
            {
                continue;
            }
            //no empty bs
            if (sections[0].Length < 2 || sections[1] == "" || sections[2] == "")
            {
                continue;
            }
            //dispose of commented lines
            if (Equals(sections[0].Substring(0, 2), commentDefiner))
            {
                continue;
            }
            //TODO need to parse the damn thing
            if (sections[0] == tutorialDefiner)
            {
                tutorial.Enqueue(StaticVariableReplacements(sections, replacePosDefine, replaceNegDefine));
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

        Debug.LogWarning(displayText);
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

    // Update is called once per frame
    void Update()
    {
        
    }

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
            axis = axis.Substring(1, axis.Length - 2);          
        }
        return axis;
        /*
        if (axisNameList.Contains(axis))
        {
            return axis;
        }
        Debug.Log("Axis " + axis + " not in axis list, hopefully a float");
        return axis;
        */
    }

    //should attempt to use as a timer if not an axis
    public bool IsAnAxis(string axis)
    {
        return axisNameList.Contains(axis);
    }
}
