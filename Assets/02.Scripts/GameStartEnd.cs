using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameStartEnd : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public GameObject InKing;
    
    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Ending")
        {
            UpdateEndingScoreText();
            InKing.SetActive(PlayerController.haveFoundTheKing);
        }
    }

    IEnumerator Ending()
    {
        yield return new WaitForSeconds(5.0f);
        SceneManager.LoadScene(0);
    }

    public void StartNewGame()
    {
        DataManager.instance.ResetJson();
        SceneManager.LoadScene(1);
    }

    public void LoadGame()
    {
        if (DataManager.instance.gameData.sceneIndex <= 0) return;

        SceneManager.LoadScene(DataManager.instance.gameData.sceneIndex);
    }

    public void SkipFirstStage()
    {
        DataManager.instance.ResetJson();
        SceneManager.LoadScene(2);
    }

    void UpdateEndingScoreText()
    {
        string _scoreText = "���� �ִ� ����: ��";
        for (int i = 1; i < UIManager.deathCount; i++)
        {
            _scoreText += "��";
        }
        if (UIManager.deathCount <= 0 && PlayerController.haveFoundTheKing == false) _scoreText = "�ѹ��� ���� �ʴٴ�, �׾߸��� �פ��� ��! �פ��� ��!";
        if (UIManager.deathCount <= 0 && PlayerController.haveFoundTheKing == true) _scoreText = "�뵥�� ��ŷ�� ������. �� ������ ������ ���� ���̾�. ����� ���Դϴ�!";
            scoreText.text = _scoreText;
    }

    // Update is called once per frame
    public void gameQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif       
    }
}