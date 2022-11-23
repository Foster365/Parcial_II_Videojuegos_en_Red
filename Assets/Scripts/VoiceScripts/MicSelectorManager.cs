using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Voice;
using Photon.Voice.Unity;

public class MicSelectorManager : MonoBehaviour
{
    public Recorder rec;
    public TMP_Dropdown dropdown;
    private void Awake()
    {
        var list = new List<string>(Microphone.devices);
        dropdown.AddOptions(list);
    }

    public void SetMic(int i)
    {
        string mic = Microphone.devices[i];
        rec.MicrophoneDevice = new DeviceInfo(mic);
    }
}
