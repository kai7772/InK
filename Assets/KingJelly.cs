using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KingJelly : MonoBehaviour
{
    public Transform returnPoint;
    public Image fadeImage;
    public GameObject easterEggBlockObject;

    private Coroutine _fadeCoroutine;

    private void Start()
    {
        if (PlayerController.haveFoundTheKing) easterEggBlockObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (_fadeCoroutine == null)
            {
                _fadeCoroutine = StartCoroutine(FoundTheKing());
            }
        }
    }

    IEnumerator FoundTheKing()
    {
        float _fadeTime = 3f;
        float _timer = 0f;
        Color _trans = new Color(Color.white.r, Color.white.g, Color.white.b, 0f);
        //ȭ�� ���̵� �ƿ�
        _timer = _fadeTime;
        while (_timer > 0f)
        {
            fadeImage.color = Color.Lerp(Color.white, _trans, _timer / _fadeTime);
            _timer -= Time.deltaTime;
            yield return null;
        }
        //�÷��̾� �ű�� ��� �ѱ�
        FindObjectOfType<PlayerController>().transform.position = returnPoint.position;
        PlayerController.haveFoundTheKing = true;
        FindObjectOfType<PlayerController>().UpdateKingJelly();
        easterEggBlockObject.SetActive(true);
        //�״�� ����
        DataManager.instance.SaveGameData(returnPoint.position);
        //ȭ�� ���̵� ��
        _timer = _fadeTime;
        while (_timer > 0f)
        {
            fadeImage.color = Color.Lerp(_trans, Color.white, _timer / _fadeTime);
            _timer -= Time.deltaTime;
            yield return null;
        }
    }
}
