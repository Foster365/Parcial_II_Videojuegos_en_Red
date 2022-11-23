using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Voice.PUN;

public class VoiceUI : MonoBehaviourPun
{
    MicUI _micUI;
    void Start()
    {
        if (photonView.IsMine)
        {
            _micUI = FindObjectOfType<MicUI>();
        }
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            var v = PunVoiceClient.Instance.PrimaryRecorder.TransmitEnabled;
            _micUI.Show(v);
        }
    }
}
