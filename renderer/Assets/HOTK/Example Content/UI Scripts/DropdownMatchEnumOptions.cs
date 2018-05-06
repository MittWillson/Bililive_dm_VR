﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

[RequireComponent(typeof(Dropdown))]
public class DropdownMatchEnumOptions : MonoBehaviour
{
    public HOTK_Overlay Overlay;

    public EnumSelection EnumOptions;

    public Dropdown Dropdown
    {
        get { return _dropdown ?? (_dropdown = GetComponent<Dropdown>()); }
    }

    private Dropdown _dropdown;

    public void OnEnable()
    {
        Dropdown.ClearOptions();
        var strings = new List<string>();
        switch (EnumOptions)
        {
            case EnumSelection.AttachmentDevice:
                UpdateDeviceDropdown();
                HOTK_TrackedDeviceManager.OnControllerIndicesUpdated += UpdateDeviceDropdown;
                break;
            case EnumSelection.AttachmentPoint:
                strings.AddRange(from object e in Enum.GetValues(typeof(MountLocation)) select e.ToString());
                Dropdown.AddOptions(strings);
                Dropdown.value = strings.IndexOf(Overlay.AnchorPoint.ToString());
                break;
            case EnumSelection.AnimationType:
                strings.AddRange(from object e in Enum.GetValues(typeof(AnimationType)) select e.ToString());
                Dropdown.AddOptions(strings);
                Dropdown.value = strings.IndexOf(Overlay.AnimateOnGaze.ToString());
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private bool SetToRightController = false;
    private bool SetToLeftController = false;

    public void SwapControllers()
    {
        if (HOTK_TrackedDeviceManager.Instance.LeftIndex != OpenVR.k_unTrackedDeviceIndexInvalid && HOTK_TrackedDeviceManager.Instance.RightIndex != OpenVR.k_unTrackedDeviceIndexInvalid) return; // If both controllers are found, don't swap selected name
        if (Dropdown.options[Dropdown.value].text == MountDevice.LeftController.ToString()) SetToRightController = true;
        else if (Dropdown.options[Dropdown.value].text == MountDevice.RightController.ToString()) SetToLeftController = true;
    }

    private void UpdateDeviceDropdown()
    {
        var strings = new List<string>
        {
            MountDevice.World.ToString(), MountDevice.Screen.ToString()
        };
        if (HOTK_TrackedDeviceManager.Instance.LeftIndex != OpenVR.k_unTrackedDeviceIndexInvalid) strings.Add(MountDevice.LeftController.ToString());
        if (HOTK_TrackedDeviceManager.Instance.RightIndex != OpenVR.k_unTrackedDeviceIndexInvalid) strings.Add(MountDevice.RightController.ToString());
        Dropdown.ClearOptions();
        Dropdown.AddOptions(strings);
        if (SetToRightController) Dropdown.value = strings.IndexOf(MountDevice.RightController.ToString());
        else if (SetToLeftController) Dropdown.value = strings.IndexOf(MountDevice.LeftController.ToString());
        else Dropdown.value = strings.IndexOf(Overlay.AnchorDevice.ToString());
        SetToRightController = false;
        SetToLeftController = false;
    }

    public void SetDropdownState(string val)
    {
        switch (EnumOptions)
        {
            case EnumSelection.AttachmentDevice:
                Overlay.AnchorDevice = (MountDevice) Enum.Parse(typeof (MountDevice), Dropdown.options[Dropdown.value].text);
                break;
            case EnumSelection.AttachmentPoint:
                Overlay.AnchorPoint = (MountLocation) Enum.Parse(typeof (MountLocation), Dropdown.options[Dropdown.value].text);
                break;
            case EnumSelection.AnimationType:
                Overlay.AnimateOnGaze = (AnimationType) Enum.Parse(typeof (AnimationType), Dropdown.options[Dropdown.value].text);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void SetToOption(string text)
    {
        for (var i = 0; i < Dropdown.options.Count; i ++)
        {
            if (Dropdown.options[i].text != text) continue;
            Dropdown.value = i;
            break;
        }
    }

    public enum EnumSelection
    {
        AttachmentDevice,
        AttachmentPoint,
        AnimationType
    }
}
