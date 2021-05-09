using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XboxController
{
    private int _playerNumber = 0;
    private bool _isConnected = false;
    public string rawInputName = null;
    public KeyboardToXBoxMap keyboardmap;

    float lastDpadLeft = 0;
    float lastDpadRight = 0;
    float lastDpadDown = 0;
    float lastDpadUp = 0;

    float lastLHoriz = 0;
    float lastLVert = 0;
    float lastRHoriz = 0;
    float lastRVert = 0;

    float lastRTrigger = 0;
    float lastLTrigger = 0;

    Vector3 lastMousePos = Vector3.one * float.MaxValue;
    Vector3 lastMouseDelta;

    string[] valid_circles = 
    {
        "12345678", "23456781", "34567812", "45678123", "56781234", "67812345", "78123456", "81234567",
        "87654321", "76543218", "65432187", "54321876", "43218765", "32187654", "21876543", "18765432"
    };

    public int PlayerNumber
    {
        get { return _playerNumber; }
        set
        {
            _playerNumber = value;
        }
    }

    public bool IsConnected
    {
        get { return _isConnected; }
        set
        {
            _isConnected = value;
        }
    }

    public XboxController(int playerNum, string rawName)
    {
        this.PlayerNumber = playerNum;
        this.rawInputName = rawName;
        this.keyboardmap = new KeyboardToXBoxMap(playerNum);
    }

    public void UpdatePreviousStates()
    {
        lastLHoriz = GetHorizontalAxis();
        lastLVert = GetVerticalAxis();
        lastRHoriz = GetHorizontalAxisRightStick();
        lastRVert = GetVerticalAxisRightStick();

        lastRTrigger = GetRightTrigger();
        lastLTrigger = GetLeftTrigger();
        lastMouseDelta = Input.mousePosition - lastMousePos;
        if (lastMousePos == Vector3.one * float.MaxValue)
            lastMouseDelta = Vector3.zero;

        lastMousePos = Input.mousePosition;
    }

    //POLLING SYSTEM SETUP
    // 6 7 8
    //  \|/
    // 5-0-1
    //  /|\
    // 4 3 2

    string leftStickPoll = "";
    float pollTime = 1.15f;
    float dTime = 0.0f;

    void InnerPollLeftStick(char add)
    {
        int r = leftStickPoll.Length - 1;

        if (r == -1)
            leftStickPoll += add;

        else if (!leftStickPoll[r].Equals(add))
            leftStickPoll += add;

    }

    public void PollLeftStick(float horizontal, float vertical)
    {
        if (dTime >= pollTime)
        {
            leftStickPoll = "";
            dTime = 0;
        }

        if (Mathf.Abs(horizontal) < InputManager.DEADZONETHRESHOLD && Mathf.Abs(vertical) < InputManager.DEADZONETHRESHOLD)
            InnerPollLeftStick('0');
        else if (Mathf.Abs(vertical) < InputManager.DEADZONETHRESHOLD && horizontal >= InputManager.DEADZONETHRESHOLD)
            InnerPollLeftStick('1');
        else if (horizontal >= InputManager.DEADZONETHRESHOLD && vertical <= -InputManager.DEADZONETHRESHOLD)
            InnerPollLeftStick('2');
        else if (Mathf.Abs(horizontal) < InputManager.DEADZONETHRESHOLD && vertical <= -InputManager.DEADZONETHRESHOLD)
            InnerPollLeftStick('3');
        else if (horizontal <= -InputManager.DEADZONETHRESHOLD && vertical <= -InputManager.DEADZONETHRESHOLD)
            InnerPollLeftStick('4');

        else if (Mathf.Abs(vertical) < InputManager.DEADZONETHRESHOLD && horizontal <= -InputManager.DEADZONETHRESHOLD)
            InnerPollLeftStick('5');
        else if (horizontal <= -InputManager.DEADZONETHRESHOLD && vertical >= InputManager.DEADZONETHRESHOLD)
            InnerPollLeftStick('6');
        else if (Mathf.Abs(horizontal) < InputManager.DEADZONETHRESHOLD && vertical >= InputManager.DEADZONETHRESHOLD)
            InnerPollLeftStick('7');
        else if (horizontal >= InputManager.DEADZONETHRESHOLD && vertical >= InputManager.DEADZONETHRESHOLD)
            InnerPollLeftStick('8');

        dTime += Time.fixedDeltaTime;
    }

    public string GetLeftStickPoll()
    {
        return leftStickPoll;
    }

    public float GetHorizontalAxis()
    {
        float l = Input.GetKey(keyboardmap.Map(XBoxButton.LeftStickLeft)) ? -1f : 0f;
        float r = Input.GetKey(keyboardmap.Map(XBoxButton.LeftStickRight)) ? 1f : 0f;
        return Input.GetAxis("Horizontal" + PlayerNumber) + l + r;  
    } 

    public bool GetHorizontalAxisDown(AxisInputDir dir)
    {
        float horiz = GetHorizontalAxis();
        if(dir == AxisInputDir.Neutral)
        {
            if (horiz < InputManager.DEADZONETHRESHOLD && horiz > -InputManager.DEADZONETHRESHOLD
                && (lastLHoriz >= InputManager.AXISINPUTTHRESHOLD || lastLHoriz <= -InputManager.AXISINPUTTHRESHOLD))
                return true;
        }

        bool currentlydown = dir == AxisInputDir.Positive ? (horiz > InputManager.AXISINPUTTHRESHOLD) : (horiz < -InputManager.AXISINPUTTHRESHOLD);
        bool istriggered = dir == AxisInputDir.Positive ? (lastLHoriz < InputManager.AXISINPUTTHRESHOLD) : (lastLHoriz > -InputManager.AXISINPUTTHRESHOLD);
        istriggered &= currentlydown;

        return istriggered;
    }

    public bool GetVerticalAxisDown(AxisInputDir dir)
    {
        float vert = GetVerticalAxis();
        if (dir == AxisInputDir.Neutral)
        {
            if (vert < InputManager.DEADZONETHRESHOLD && vert > -InputManager.DEADZONETHRESHOLD
                && (lastLVert >= InputManager.AXISINPUTTHRESHOLD || lastLVert <= -InputManager.AXISINPUTTHRESHOLD))
                return true;
        }

        bool currentlydown = dir == AxisInputDir.Positive ? (vert > InputManager.AXISINPUTTHRESHOLD) : (vert < -InputManager.AXISINPUTTHRESHOLD);
        bool istriggered = dir == AxisInputDir.Positive ? (lastLVert < InputManager.AXISINPUTTHRESHOLD) : (lastLVert > -InputManager.AXISINPUTTHRESHOLD);
        istriggered &= currentlydown;

        return istriggered;
    }

    public bool GetLeftStickRotatingInCircles()
    {
        for(int i = 0; i < valid_circles.Length; ++i)
        {
            if (leftStickPoll.Contains(valid_circles[i]))
                return true;
        }

        return false;
        //return CalcLevenshteinDistance( leftStickPoll, "12345678") < 4 || CalcLevenshteinDistance(leftStickPoll, "87654321") < 4;
    }

    public float GetVerticalAxis()
    {
        float u = Input.GetKey(keyboardmap.Map(XBoxButton.LeftStickUp)) ? 1f : 0f;
        float d = Input.GetKey(keyboardmap.Map(XBoxButton.LeftStickDown)) ? -1f : 0f;
        return Input.GetAxis("Vertical" + PlayerNumber) + u + d;
    } 

    public float GetHorizontalAxisRightStick()
    {
        float kbrl = lastMouseDelta.x;
        return Input.GetAxis("HorizontalRightStick" + PlayerNumber) + kbrl;
    } 

    public float GetVerticalAxisRightStick()
    {
        float kbud = lastMouseDelta.y;
        return Input.GetAxis("VerticalRightStick" + PlayerNumber) + kbud;
    } 

    public float GetDPadHorizontal()
    {
        return Input.GetAxis("DPadHorizontal" + PlayerNumber);
    } 

    public float GetDPadVertical()
    {
        return Input.GetAxis("DPadVertical" + PlayerNumber);
    } 

    public bool GetDPadLeft()
    {
        float dpadLeftAxis = GetDPadHorizontal();
        bool currentlyDown = dpadLeftAxis < -0.75f;

        bool isDown = false;

        if(currentlyDown && lastDpadLeft > dpadLeftAxis)
        {
            isDown = true;
        }
        lastDpadLeft = dpadLeftAxis;
        return isDown;
    }

    public bool GetDPadRight()
    {
        float dpadRightAxis = GetDPadHorizontal();
        bool currentlyDown = dpadRightAxis > 0.75f;

        bool isDown = false;

        if (currentlyDown && lastDpadRight < dpadRightAxis)
        {
            isDown = true;
        }
        lastDpadRight = dpadRightAxis;
        return isDown;
    }

    public bool GetDPadDown()
    {
        float dpadUpAxis = GetDPadVertical();
        bool currentlyDown = dpadUpAxis < -0.85f;

        bool isDown = false;

        if (currentlyDown && lastDpadDown > dpadUpAxis)
        {
            isDown = true;
        }
        lastDpadDown = dpadUpAxis;
        return isDown;
    }

    public bool GetDPadUp()
    {
        float dpadUpAxis = GetDPadVertical();
        bool currentlyDown = dpadUpAxis > 0.75f;

        bool isDown = false;

        if (currentlyDown && lastDpadUp < dpadUpAxis)
        {
            isDown = true;
        }
        lastDpadUp = dpadUpAxis;
        return isDown;
    }

    public float GetRightTrigger()
    {
        return Input.GetAxis("RightTrigger" + PlayerNumber);
    }

    public bool GetRightTrigger(ButtonQuery q)
    {
        string buttonstring = "RightTrigger";
        float cur = Input.GetAxis(buttonstring + PlayerNumber);

        switch (q)
        {
            case ButtonQuery.Pressed:
                return cur > InputManager.DEADZONETHRESHOLD;
            case ButtonQuery.Down:
                return cur > lastRTrigger;
            case ButtonQuery.Up:
                return cur < lastRTrigger;
        }

        return false;
    }

    public float GetLeftTrigger()
    {
        return Input.GetAxis("LeftTrigger" + PlayerNumber);
    }

    public bool GetLeftTrigger(ButtonQuery q)
    {
        string buttonstring = "LeftTrigger";
        float cur = Input.GetAxis(buttonstring + PlayerNumber);

        switch (q)
        {
            case ButtonQuery.Pressed:
                return cur > InputManager.DEADZONETHRESHOLD;
            case ButtonQuery.Down:
                return cur > lastLTrigger;
            case ButtonQuery.Up:
                return cur < lastLTrigger;
        }

        return false;
    }

    public bool GetAButton(ButtonQuery q)
    {
        string buttonstring = "A";

        switch (q)
        {
            case ButtonQuery.Pressed:
                return Input.GetButton(buttonstring + PlayerNumber) || Input.GetKey(keyboardmap.Map(XBoxButton.A));
            case ButtonQuery.Down:
                return Input.GetButtonDown(buttonstring + PlayerNumber) || Input.GetKeyDown(keyboardmap.Map(XBoxButton.A));
            case ButtonQuery.Up:
                return Input.GetButtonUp(buttonstring + PlayerNumber) || Input.GetKeyUp(keyboardmap.Map(XBoxButton.A));
        }

        //this should never be reached
        Debug.LogError("Unexpected return of function: GetAButton(ButtonQuery q)");
        return false;
    } 

    public bool GetBButton(ButtonQuery q)
    {
        string buttonstring = "B";

        switch (q)
        {
            case ButtonQuery.Pressed:
                return Input.GetButton(buttonstring + PlayerNumber) || Input.GetKey(keyboardmap.Map(XBoxButton.B));
            case ButtonQuery.Down:
                return Input.GetButtonDown(buttonstring + PlayerNumber) || Input.GetKeyDown(keyboardmap.Map(XBoxButton.B));
            case ButtonQuery.Up:
                return Input.GetButtonUp(buttonstring + PlayerNumber) || Input.GetKeyUp(keyboardmap.Map(XBoxButton.B));
        }

        //this should never be reached
        Debug.LogError("Unexpected return of function: GetBButton(ButtonQuery q)");
        return false;
    } 

    public bool GetXButton(ButtonQuery q)
    {
        string buttonstring = "X";

        switch (q)
        {
            case ButtonQuery.Pressed:
                return Input.GetButton(buttonstring + PlayerNumber) || Input.GetKey(keyboardmap.Map(XBoxButton.X));
            case ButtonQuery.Down:
                return Input.GetButtonDown(buttonstring + PlayerNumber) || Input.GetKeyDown(keyboardmap.Map(XBoxButton.X));
            case ButtonQuery.Up:
                return Input.GetButtonUp(buttonstring + PlayerNumber) || Input.GetKeyUp(keyboardmap.Map(XBoxButton.X));
        }

        //this should never be reached 
        Debug.LogError("Unexpected return of function: GetXButton(ButtonQuery q)");
        return false;
    } 

    public bool GetYButton(ButtonQuery q)
    {
        string buttonstring = "Y";

        switch (q)
        {
            case ButtonQuery.Pressed:
                return Input.GetButton(buttonstring + PlayerNumber) || Input.GetKey(keyboardmap.Map(XBoxButton.Y));
            case ButtonQuery.Down:
                return Input.GetButtonDown(buttonstring + PlayerNumber) || Input.GetKeyDown(keyboardmap.Map(XBoxButton.Y));
            case ButtonQuery.Up:
                return Input.GetButtonUp(buttonstring + PlayerNumber) || Input.GetKeyUp(keyboardmap.Map(XBoxButton.Y));
        }

        //this should never be reached
        Debug.LogError("Unexpected return of function: GetYButton(ButtonQuery q)");
        return false;
    } 

    public bool GetRightBumper(ButtonQuery q)
    {
        string buttonstring = "RightBumper";

        switch (q)
        {
            case ButtonQuery.Pressed:
                return Input.GetButton(buttonstring + PlayerNumber);
            case ButtonQuery.Down:
                return Input.GetButtonDown(buttonstring + PlayerNumber);
            case ButtonQuery.Up:
                return Input.GetButtonUp(buttonstring + PlayerNumber);
        }

        //this should never be reached
        Debug.LogError("Unexpected return of function: GetRightBumper(ButtonQuery q)");
        return false;
    } 

    public bool GetLeftBumper(ButtonQuery q)
    {
        string buttonstring = "LeftBumper";

        switch (q)
        {
            case ButtonQuery.Pressed:
                return Input.GetButton(buttonstring + PlayerNumber);
            case ButtonQuery.Down:
                return Input.GetButtonDown(buttonstring + PlayerNumber);
            case ButtonQuery.Up:
                return Input.GetButtonUp(buttonstring + PlayerNumber);
        }

        //this should never be reached
        Debug.LogError("Unexpected return of function: GetLeftBumper(ButtonQuery q)");
        return false;
    } 

    public bool GetStartButton(ButtonQuery q)
    {
        string buttonstring = "Start";

        switch (q)
        {
            case ButtonQuery.Pressed:
                return Input.GetButton(buttonstring + PlayerNumber);
            case ButtonQuery.Down:
                return Input.GetButtonDown(buttonstring + PlayerNumber);
            case ButtonQuery.Up:
                return Input.GetButtonUp(buttonstring + PlayerNumber);
        }

        //this should never be reached
        Debug.LogError("Unexpected return of function: GetStartButton(ButtonQuery q)");
        return false;
    } 

    public bool GetSelectButton(ButtonQuery q)
    {
        string buttonstring = "Select";

        switch (q)
        {
            case ButtonQuery.Pressed:
                return Input.GetButton(buttonstring + PlayerNumber);
            case ButtonQuery.Down:
                return Input.GetButtonDown(buttonstring + PlayerNumber);
            case ButtonQuery.Up:
                return Input.GetButtonUp(buttonstring + PlayerNumber);
        }

        //this should never be reached
        Debug.LogError("Unexpected return of function: GetSelectButton(ButtonQuery q)");
        return false;
    } 

    public bool GetLeftStick(ButtonQuery q)
    {
        string buttonstring = "LeftStick";

        switch (q)
        {
            case ButtonQuery.Pressed:
                return Input.GetButton(buttonstring + PlayerNumber);
            case ButtonQuery.Down:
                return Input.GetButtonDown(buttonstring + PlayerNumber);
            case ButtonQuery.Up:
                return Input.GetButtonUp(buttonstring + PlayerNumber);
        }

        //this should never be reached
        Debug.LogError("Unexpected return of function: GetLeftStick(ButtonQuery q)");
        return false;
    }

    public bool GetRightStick(ButtonQuery q)
    {
        string buttonstring = "RightStick";

        switch (q)
        {
            case ButtonQuery.Pressed:
                return Input.GetButton(buttonstring + PlayerNumber);
            case ButtonQuery.Down:
                return Input.GetButtonDown(buttonstring + PlayerNumber);
            case ButtonQuery.Up:
                return Input.GetButtonUp(buttonstring + PlayerNumber);
        }

        //this should never be reached
        Debug.LogError("Unexpected return of function: GetRightStick(ButtonQuery q)");
        return false;
    }

    //public bool GetMiddleButton(ButtonQuery q)
    //{
    //    string buttonstring = "MiddleButton";

    //    switch (q)
    //    {
    //        case ButtonQuery.Pressed:
    //            return Input.GetButton(buttonstring + PlayerNumber);
    //        case ButtonQuery.Down:
    //            return Input.GetButtonDown(buttonstring + PlayerNumber);
    //        case ButtonQuery.Up:
    //            return Input.GetButtonUp(buttonstring + PlayerNumber);
    //    }

    //    //this should never be reached
    //    Debug.LogError("Unexpected return of function: GetMiddleButton(ButtonQuery q)");
    //    return false;
    //}

    public bool AnyButton()
    {
        return GetHorizontalAxis() != 0f || GetVerticalAxis() != 0f ||
            GetRightTrigger() != 0f || GetLeftTrigger() != 0f ||
            GetHorizontalAxisRightStick() != 0f || GetVerticalAxisRightStick() != 0f ||
            GetAButton(ButtonQuery.Pressed) || GetBButton(ButtonQuery.Pressed) ||
            GetXButton(ButtonQuery.Pressed) || GetYButton(ButtonQuery.Pressed) ||
            GetRightBumper(ButtonQuery.Pressed) || GetLeftBumper(ButtonQuery.Pressed) ||
            GetStartButton(ButtonQuery.Pressed) || GetSelectButton(ButtonQuery.Pressed) || GetLeftStick(ButtonQuery.Pressed) ||
            GetRightStick(ButtonQuery.Pressed);// || GetMiddleButton(ButtonQuery.Pressed);
    }

    private static int CalcLevenshteinDistance(string a, string b)
    {
        if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b)) return 0;

        int lengthA = a.Length;
        int lengthB = b.Length;
        var distances = new int[lengthA + 1, lengthB + 1];
        for (int i = 0; i <= lengthA; distances[i, 0] = i++) ;
        for (int j = 0; j <= lengthB; distances[0, j] = j++) ;

        for (int i = 1; i <= lengthA; i++)
            for (int j = 1; j <= lengthB; j++)
            {
                int cost = b[j - 1] == a[i - 1] ? 0 : 1;
                distances[i, j] = Mathf.Min
                    (
                    Mathf.Min(distances[i - 1, j] + 1, distances[i, j - 1] + 1),
                    distances[i - 1, j - 1] + cost
                    );
            }
        return distances[lengthA, lengthB];
    }
}

public enum ButtonQuery
{
    Pressed,
    Down,
    Up
}

public enum AxisInputDir
{
    Positive = 0,
    Negative = 1,
    Neutral = 2
}

public enum XBoxButton
{
    A = 0,
    B = 1,
    Y = 2,
    X = 3,
    LeftStickRight = 4,
    LeftStickLeft = 5,
    LeftStickUp = 6,
    LeftStickDown = 7,
}

public class KeyboardToXBoxMap
{
    private Dictionary<XBoxButton, KeyCode> map;
    public int keyboardindex;
    public KeyboardToXBoxMap(int idx)
    {
        keyboardindex = idx;
        map = new Dictionary<XBoxButton, KeyCode>();

        switch(idx)
        {
            case 1:
                map[XBoxButton.A] = KeyCode.Space;
                map[XBoxButton.B] = KeyCode.LeftShift;
                map[XBoxButton.Y] = KeyCode.X;
                map[XBoxButton.X] = KeyCode.Z;

                map[XBoxButton.LeftStickRight] = KeyCode.D;
                map[XBoxButton.LeftStickLeft] = KeyCode.A;
                map[XBoxButton.LeftStickUp] = KeyCode.W;
                map[XBoxButton.LeftStickDown] = KeyCode.S;
                break;
            case 2:
                map[XBoxButton.A] = KeyCode.RightShift;
                map[XBoxButton.B] = KeyCode.Return;
                map[XBoxButton.Y] = KeyCode.Delete;
                map[XBoxButton.X] = KeyCode.RightControl;

                map[XBoxButton.LeftStickRight] = KeyCode.RightArrow;
                map[XBoxButton.LeftStickLeft] = KeyCode.LeftArrow;
                map[XBoxButton.LeftStickUp] = KeyCode.UpArrow;
                map[XBoxButton.LeftStickDown] = KeyCode.DownArrow;
                break;
        }
    }

    public KeyCode Map(XBoxButton button)
    {
        return map[button];
    }

    //Input.GetKey(keyboardmap.Map(XboxButton.A))
}
