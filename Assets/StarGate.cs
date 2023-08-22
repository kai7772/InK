using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarGate : MonoBehaviour
{
    private void Start()
    {
        SetAlpha(1.0f);
        GetComponent<Collider2D>().isTrigger = false;
    }

    private void OnEnable()
    {
        FindObjectOfType<PlayerController>().onStarCollect += CheckStarRemaining;
    }

    private void OnDisable()
    {
        if (FindObjectOfType<PlayerController>() != null)
        {
            FindObjectOfType<PlayerController>().onStarCollect -= CheckStarRemaining;
        }
    }

    private void CheckStarRemaining()
    {
        int _starLeftCount = 0;
        ColoredObject[] _allColoredObjects = FindObjectsOfType<ColoredObject>();
        for (int i = 0; i < _allColoredObjects.Length; i++)
        {
            var _obj = _allColoredObjects[i];
            if (_obj.CompareTag("Star"))
            {
                if (_obj.transform.position.x <= transform.position.x)
                {
                    _starLeftCount++;
                }
            }
        }

        if (_starLeftCount <= 0)
        {
            SetAlpha(0.3f);
            GetComponent<Collider2D>().isTrigger = true;
        }
    }

    private void SetAlpha(float alpha)
    {
        List<SpriteRenderer> _renderers = new List<SpriteRenderer>();
        _renderers.Clear();
        _renderers.Add(GetComponent<SpriteRenderer>());
        _renderers.AddRange(GetComponentsInChildren<SpriteRenderer>());

        for (int i = 0; i < _renderers.Count; i++)
        {
            Color _color = _renderers[i].color;
            _renderers[i].color = new Color(_color.r, _color.g, _color.b, alpha);
        }

    }
}
