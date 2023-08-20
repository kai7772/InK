using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextTriggerObject : MonoBehaviour
{
    public TextMeshProUGUI text;

    private void Start()
    {
        if (text != null) text.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && text != null)
        {
            text.gameObject.SetActive(true);
        }
    }
}
