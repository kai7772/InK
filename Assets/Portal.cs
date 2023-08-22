using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Portal : MonoBehaviour
{
    public bool isEndingGoal = false;
    public Image fadeImage;

    private Coroutine _endingCoroutine;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            print("Touched");
            if (isEndingGoal == false) SceneController.instance.NextLevel();
            else
            {
                if (_endingCoroutine == null) _endingCoroutine = StartCoroutine(EndingFadeOut());
            }
        }
    }

    IEnumerator EndingFadeOut()
    {
        float _fadeTime = 3f;
        float _timer = 0f;
        Color _trans = new Color(Color.white.r, Color.white.g, Color.white.b, 0f);
        //화면 페이드 아웃
        _timer = _fadeTime;
        while (_timer > 0f)
        {
            fadeImage.color = Color.Lerp(Color.white, _trans, _timer / _fadeTime);
            _timer -= Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(1);

        SceneController.instance.NextLevel();
    }

}
