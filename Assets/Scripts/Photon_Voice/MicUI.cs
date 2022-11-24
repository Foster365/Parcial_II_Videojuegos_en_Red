using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MicUI : MonoBehaviour
{
    public Image image;
    public void Show(bool v)
    {
        image.enabled = v;
    }
}
