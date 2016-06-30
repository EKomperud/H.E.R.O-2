using UnityEngine;
using System.Collections;

/// <summary>
/// This is a list of helper functions that wrap Input methods in a way that lets you easily add prefixes or suffixes to input names.
/// Instead of having to copy player controllers across four separate classes that only change input name strings,
/// Just initialize your player controllers with appropriate prefix/suffix strings and replace Input method calls with MultiInput
/// 
/// These are the most common input methods, if you need more you can add them.
/// </summary>
public static class MultiInput {

    public const int PREFIX = 0;
    public const int SUFFIX = 1;

    // Does Input.GetAxis and adds a prefix to axisName
    // For example, if you do MultiInput.GetAxis("Horizontal", "P1_"), this will be equivalent to Input.GetAxis("P1_Horizontal");
    public static float GetAxis(string axisName, string prefix)
    {
        return Input.GetAxis(prefix + axisName);
    }

    public static bool GetButton(string buttonName, string prefix)
    {
        return Input.GetButton(prefix + buttonName);
    }

    public static bool GetButtonDown(string buttonName, string prefix)
    {
        return Input.GetButtonDown(prefix + buttonName);
    }

    public static bool GetButtonUp(string buttonName, string prefix)
    {
        return Input.GetButtonUp(prefix + buttonName);
    }


    // Use these instead if you'd rather have suffixes instead of prefixes in your input names
    // For example, MultiInput.GetAxis("Horizontal", "", "_P2") is equivalent to Input.GetAxis("Horizontal_P2");
    public static float GetAxis(string axisName, string prefix, string suffix)
    {
        return Input.GetAxis(prefix + axisName + suffix);
    }

    public static bool GetButton(string buttonName, string prefix, string suffix)
    {
        return Input.GetButton(prefix + buttonName + suffix);
    }

    public static bool GetButtonDown(string buttonName, string prefix, string suffix)
    {
        return Input.GetButtonDown(prefix + buttonName + suffix);
    }

    public static bool GetButtonUp(string buttonName, string prefix, string suffix)
    {
        return Input.GetButtonUp(prefix + buttonName + suffix);
    }



    // Another option, for flag use MultiInput.PREFIX or MultiInput.SUFFIX
    // idk how your input is set up just use whatever's most convenient
    public static float GetAxis(string axisName, string prefixOrSuffix, int flag)
    {
        if (flag == MultiInput.SUFFIX)
            return Input.GetAxis(axisName + prefixOrSuffix);
        else 
            return Input.GetAxis(prefixOrSuffix + axisName);
    }

    public static bool GetButton(string buttonName, string prefixOrSuffix, int flag)
    {
        if (flag == MultiInput.SUFFIX)
            return Input.GetButton(buttonName + prefixOrSuffix);
        else
            return Input.GetButton(prefixOrSuffix + buttonName);
    }

    public static bool GetButtonDown(string buttonName, string prefixOrSuffix, int flag)
    {
        if (flag == MultiInput.SUFFIX)
            return Input.GetButtonDown(buttonName + prefixOrSuffix);
        else
            return Input.GetButtonDown(prefixOrSuffix + buttonName);
    }

    public static bool GetButtonUp(string buttonName, string prefixOrSuffix, int flag)
    {
        if (flag == MultiInput.SUFFIX)
            return Input.GetButtonUp(buttonName + prefixOrSuffix);
        else
            return Input.GetButtonUp(prefixOrSuffix + buttonName);
    }
}
