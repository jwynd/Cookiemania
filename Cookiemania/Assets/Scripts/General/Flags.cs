using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flags : MonoBehaviour
{
    public static Flags Instance;
    private Dictionary<string, int> flags;

    void Awake(){
        if(Instance != null && Instance != this){
            Debug.LogError("Two or more instances of General_Flags detected");
            Destroy(this);
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        flags = new Dictionary<string, int>();
    }
    
    // returns the value of the flag, or -1 if the flag does not exist
    public int getFlag(string flag){
        int val = 0;
        if(flags.TryGetValue(flag, out val)){
            return val;
        } else {
            Debug.LogWarning("Attempted to access non existant flag: \""+flag+"\"");
            return -1;
        }
    }

    // Sets the value of an existing flag, if the final argument is set to true, non existant flags will be created
    // returns true for success, and false for failure
    public bool setFlag(string flag, int val, bool create = false){
        try{
            int temp = flags[flag]; // force it to throw a KeyNotFoundException if the flag does not exist
            flags[flag] = val;
            return flags[flag]==val;
        } catch  (KeyNotFoundException){
            if(create){
                Debug.Log("Flag \""+flag+"\" does not exist, creating...");
                createFlag(flag, val);
                return flags[flag]==val;
            } else {
                Debug.LogError("Attempted to set non existant flag \""+flag+"\" to create flag call createFlag(flag, val), or setFlag(flag, val, true)");
                return false;
            }
        }
    }

    // creates a new flag with an initial value, returns ture for success, and false for failure
    public bool createFlag(string flag, int val){
        try{
            flags.Add(flag, val);
            return flags[flag]==val;
        } catch (System.ArgumentException) {
            Debug.LogError("The flag \""+flag+"\" already exists when createFlag is called");
            return false;
        }
    }
}
