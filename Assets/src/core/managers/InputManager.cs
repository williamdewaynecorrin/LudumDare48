using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    //singleton instance
    public static InputManager instance = null;
    public static float DEADZONETHRESHOLD = 0.095f;
    public static float AXISINPUTTHRESHOLD = 0.75f;
    public static bool RADIALMENUOPEN = false;

    //instance variables
    public List<XboxController> activeGamepads = new List<XboxController>();
    int checkDeviceLoop = 60 * 3; //every 3 seconds
    int checkDeviceLoopReset;
    public int peakControllerConnected = 0;

    // Use this for initialization
    void Awake ()
    {
        if (instance == null)
        {
            instance = this;
            Setup();
            GameObject.DontDestroyOnLoad(this.gameObject);
        }
        else if(instance != this)
        {
            GameObject.Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Adds an xbox controller for active input.
    /// </summary>
    /// <param name="playerNum">the 1-based index of the player</param>
    /// <param name="rawName">the raw name of the device driver</param>
    public XboxController AddController(int playerNum, string rawName)
    {
        XboxController newController = new XboxController(playerNum, rawName);
        activeGamepads.Add(newController);
        return newController;
    }

    public XboxController GetController(int playerNum)
    {
        foreach(var x in activeGamepads)
        {
            if (x.PlayerNumber == playerNum)
                return x;
        }

        return null;
    }

    void Setup()
    {
        checkDeviceLoopReset = checkDeviceLoop;
        instance.CheckInputDeviceArrays();
    }

    void CheckInputDeviceArrays()
    {
        string[] activeDevices = Input.GetJoystickNames();//this length could be 2 even though only 1 controller is disconnected.
        string[] rawNames = GetActiveRawGamepadNames(); //this will start at 0 at beginning of the game.

        //this is annoying - if they dont EXACTLY match up, it means some shit has been disconnected.
        //example string ->   1. Xbox360 Controller 
        //                    2. 
        //                    
        //COMPARED TO:        1.Xbox360 Controller
        //                    2.Xbox One Controller For PC

        //if we have less gamepads stored than devices active
        if (rawNames.Length < activeDevices.Length)
        {
            //we probably just started the game. just set all of the rawnames equal to the activedevices.
            for (int i = 0; i < activeDevices.Length; ++i)
            {
                //make a new controller if a controller with the same index doesnt exist already
                if (!InputManager.ControllerExists(i + 1))
                {
                    XboxController newC = InputManager.instance.AddController(i + 1, activeDevices[i]);

                    //is the controller actually connected? we can tell by looking at the string.
                    newC.IsConnected = !activeDevices[i].Equals("");
                }
            }
        }
        else
        {
            //note that some of these can be empty.
            for (int i = 0; i < activeDevices.Length; ++i)
            {
                if (activeDevices[i].Equals(""))
                {
                    //this one is disconnected. make sure it exists and set its status
                    if(InputManager.ControllerExists(i + 1))
                    {
                        InputManager.instance.GetController(i + 1).IsConnected = false;
                    }
                    else //throw an exception, this should not happen
                    {
                        Debug.LogError("Input error: a controller should have been added before it reaches this loop.");
                    }
                }
                else
                {
                    //this one is connected. make sure it exists and set its status
                    if (InputManager.ControllerExists(i + 1))
                    {
                        InputManager.instance.GetController(i + 1).IsConnected = true;
                    }
                    else //throw an exception, this should not happen
                    {
                        Debug.LogError("Input error: a controller should have been added before it reaches this loop.");
                    }
                }
            }
        }
    }

    void FixedUpdate ()
    {
        checkDeviceLoop--;

        if(checkDeviceLoop == 0)
        {
            //check for device connections and disconnections
            checkDeviceLoop = checkDeviceLoopReset;
            CheckInputDeviceArrays();
        }
	}

    private void LateUpdate()
    {
        foreach (var x in instance.activeGamepads)
        {
            x.UpdatePreviousStates();
        }
    }

    public static void Reset()
    {
        //nothing to do here yet
    }

    public static bool ControllerExists(int index)
    {
        foreach (var x in instance.activeGamepads)
        {
            if (x.PlayerNumber == index)
                return true;
        }

        return false;
    }

    public static bool AnyControllerConnected()
    {
        foreach(var x in instance.activeGamepads)
        {
            if(x.IsConnected)
            {
                return true;
            }
        }

        return false;
    }

    public static bool GamepadsConnected()
    {
        return Input.GetJoystickNames().Length > 0;
    }

    public static string GetGamepadInformationString()
    {
        string info = "";

        string[] pads = Input.GetJoystickNames();

        for(int i = 0; i < pads.Length; ++i)
        {
            if (!pads[i].Equals(""))
                info += (i + 1).ToString() + ". " + pads[i] + "\n";
            else info += (i + 1).ToString() + ". " + "DISCONNECTED.\n";
        }

        return info;
    }

    public static string GetControllerConnectionsString()
    {
        string info = "";

        foreach(XboxController x in instance.activeGamepads)
        {
            info += x.PlayerNumber.ToString() + ". Connected: " + x.IsConnected + " " + x.rawInputName + "\n";
        }
   
        return info;
    }

    public static string[] GetActiveRawGamepadNames()
    {
        string[] rawNames = new string[instance.activeGamepads.Count];

        for(int i = 0; i < rawNames.Length; ++i)
        {
            rawNames[i] = instance.activeGamepads[i].rawInputName;
        }

        return rawNames;
    }
}
