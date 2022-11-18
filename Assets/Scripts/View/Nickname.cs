using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class Nickname : MonoBehaviour
{
    public TextMeshProUGUI nickNameUI;
    public Vector3 offset;
    Transform target;
    Camera nickCamera;
    private void Awake()
    {
        nickCamera = Camera.main;
    }
    public void SetTarget(Transform target)
    {
        this.target = target;
    }
    public void SetName(string nick)
    {
        nickNameUI.text = nick;
    }

    private void Update()
    {
        if (target != null && nickCamera != null)
        {
            var pos = nickCamera.WorldToScreenPoint(target.position + offset);
            transform.position = pos;
        }
    }
}
