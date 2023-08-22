using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    #region Monobehavior Methods
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Screen.SetResolution(1920, 1080, false);
        LoadGameData();
    }

    string GameDataFileName = "GameData.json";
    public GameData gameData = new GameData();

    #endregion
    public void LoadGameData()
    {
        string filePath = Application.dataPath + "/" + GameDataFileName;

        // 저장된 게임이 있다면
        if (File.Exists(filePath))
        {
            // 저장된 파일 읽어오고 Json을 클래스 형식으로 전환해서 할당
            string FromJsonData = File.ReadAllText(filePath);
            gameData = JsonUtility.FromJson<GameData>(FromJsonData);

            //데이터 적용
            //if (gameData.jellyColoring == Coloring.Black) gameData.jellyColoring = Coloring.Red;
            //if (gameData.mainColoring == Coloring.Black) gameData.mainColoring = Coloring.Red;
            if (gameData.sceneIndex != SceneManager.GetActiveScene().buildIndex)
            {
                if (FindObjectOfType<PlayerController>() != null) 
                {
                    SaveGameData(FindObjectOfType<PlayerController>().transform.position);
                }
                return;
            }

            JellyShooter _js = FindObjectOfType<JellyShooter>();
            //플레이어 이동
            _js.transform.position = gameData.respawnPoint;
            //젤리 색 변경
            _js.SetJellyColoring(gameData.jellyColoring);
            //deathCount 적용
            UIManager.deathCount = gameData.deathCount;
            //haveFoundTheKing 적용
            PlayerController.haveFoundTheKing = gameData.haveFoundTheKing;
            //배경 색 변경
            ColorManager.instance.SwitchMainColoring(gameData.mainColoring);
            //isEyeBall 적용, 지나친 스타 전부 끄기
            List<ColoredObject> _allColoredObjects = new List<ColoredObject>();
            _allColoredObjects.AddRange(FindObjectsOfType<ColoredObject>());
            for (int i = 0; i < _allColoredObjects.Count; i++)
            {
                if (_allColoredObjects[i].transform.position.x <= gameData.respawnPoint.x)
                {
                    _allColoredObjects[i].startAsEyeball = false;
                    _allColoredObjects[i].InitEyeball();
                    if (_allColoredObjects[i].tag == "Star") _allColoredObjects[i].gameObject.SetActive(false);
                }
            }

        }
        // 저장된 게임이 없다면 새로 데이터 지정
        else
        {
            ResetJson();
        }
    }

    public void SaveGameData(Vector2 respawnPoint)
    {
        //데이터 저장
        //sceneIndex 저장
        gameData.sceneIndex = SceneManager.GetActiveScene().buildIndex;

        //respawnPoint 저장
        gameData.respawnPoint = respawnPoint;

        //deathCount 저장
        gameData.deathCount = UIManager.deathCount;

        //haveFoundTheKing 저장
        gameData.haveFoundTheKing = PlayerController.haveFoundTheKing;

        //jellyColoring 저장
        JellyShooter _js = FindObjectOfType<JellyShooter>();
        Coloring _coloring = _js.jellyColoring;
        if (_js.jelliedObject != null) _coloring = _js.jelliedObject.objectColoring;
        gameData.jellyColoring = _coloring;

        //mainColoring 저장
        gameData.mainColoring = ColorManager.instance.mainColoring;

        // 클래스를 Json 형식으로 전환
        string ToJsonData = JsonUtility.ToJson(gameData, true);
        string filePath = Application.dataPath + "/" + GameDataFileName;


        // 이미 저장된 파일이 있다면 덮어쓰고, 없다면 새로 만들어서 저장
        File.WriteAllText(filePath, ToJsonData);
        //Debug.Log("save: " + filePath);
    }

    public void ResetJson()
    {
        //sceneIndex 저장
        gameData.sceneIndex = -1;

        //respawnPoint 저장
        gameData.respawnPoint = Vector3.zero;

        //jellyColoring 저장
        JellyShooter _js = FindObjectOfType<JellyShooter>();
        Coloring _coloring = Coloring.Red;
        gameData.jellyColoring = _coloring;

        //mainColoring 저장
        gameData.mainColoring = Coloring.Red;

        //isEyeBall 리스트 저장
        gameData.isEyeBallList.Clear();

        gameData.deathCount = 0;

        gameData.haveFoundTheKing = false;

        // 클래스를 Json 형식으로 전환
        string ToJsonData = JsonUtility.ToJson(gameData, true);
        string filePath = Application.dataPath + "/" + GameDataFileName;


        // 이미 저장된 파일이 있다면 덮어쓰고, 없다면 새로 만들어서 저장
        File.WriteAllText(filePath, ToJsonData);
        //Debug.Log("save: " + filePath);
    }

    public void IncrementDeathCount()
    {
        gameData.deathCount++;

        // 클래스를 Json 형식으로 전환
        string ToJsonData = JsonUtility.ToJson(gameData, true);
        string filePath = Application.dataPath + "/" + GameDataFileName;


        // 이미 저장된 파일이 있다면 덮어쓰고, 없다면 새로 만들어서 저장
        File.WriteAllText(filePath, ToJsonData);
        //Debug.Log("save: " + filePath);
    }
}
