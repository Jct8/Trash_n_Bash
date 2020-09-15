using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameState _GameState { get; set; }

    public int videoNumbertoPlay = 1;
    public int currentlevel = 1;
    public int highScore;
    private float coolTime = 10.0f;

    public float _houseHP = 50.0f;
    public float _racoonHP;
    public string sceneToLoad = "MainMenu";

    public Dictionary<UpgradeMenu.Upgrade, int> upgradeLevelsDictionary = new Dictionary<UpgradeMenu.Upgrade, int>();
    public Dictionary<UpgradeMenu.Upgrade, bool> upgradeEnabled = new Dictionary<UpgradeMenu.Upgrade, bool>();
    public List<string> specialTargets = new List<string>();
    public string choosenTarget = "No Target";
    public BarricadeSpawner barricadeSpawner;

    private Camera _camera;
    private Vector3 _camPos;
    private float getDeltaTime { set; get; }

    public bool _isCameraUsing = false;

    private bool isVaribableLoaded = false;
    public enum GameState
    {
        Loader,
        MainMenu,
        GamePlay,
        Tutorial,
        GameWin,
        GameLose
    }

    private void Awake()
    {
        _GameState = GameState.Loader;
        specialTargets.Add("No Target");
        foreach (UpgradeMenu.Upgrade item in System.Enum.GetValues(typeof(UpgradeMenu.Upgrade)))
        {
            upgradeLevelsDictionary.Add(item, 0);
            upgradeEnabled.Add(item, false);
        }
    }

    public GameManager Initialize()
    {
        currentlevel = 0;
        return this;
    }

    private void Update()
    {
        switch (_GameState)
        {
            case GameState.Loader:
                ServiceLocator.Get<AudioManager>().gameObject.SetActive(true);
                break;
            case GameState.MainMenu:
                break;
            case GameState.GamePlay:
                if (ServiceLocator.Get<LevelManager>().CheckLoseCondition())
                {
                    _GameState = GameState.GameLose;
                    StartCoroutine(ZoomInAndOut(1.0f, true));
                    StartCoroutine(DelayEnding(3.0f));
                }
                else if (ServiceLocator.Get<LevelManager>().CheckWinCondition())
                {
                    _GameState = GameState.GameWin;
                    StartCoroutine(ZoomInAndOut(1.0f, false));
                    StartCoroutine(DelayEnding(3.0f));
                }
                break;
            case GameState.Tutorial:
                if (ServiceLocator.Get<LevelManager>().CheckLoseCondition())
                {
                    _GameState = GameState.GameLose;
                    StartCoroutine(ZoomInAndOut(1.0f, true));
                    StartCoroutine(DelayEnding(3.0f));
                }
                else if (ServiceLocator.Get<LevelManager>().CheckWinCondition())
                {
                    _GameState = GameState.GameWin;
                    StartCoroutine(ZoomInAndOut(1.0f, false));
                    StartCoroutine(DelayEnding(3.0f));
                }
                break;
        }
        getDeltaTime = Time.deltaTime;
    }

    public IEnumerator ZoomInAndOut(float delay, bool isLose)
    {
        if (_isCameraUsing)
            yield return null;
        else
        {
            _isCameraUsing = true;
            _camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            _camPos = _camera.gameObject.transform.localPosition;
            Quaternion _camRot = _camera.transform.rotation;
            GameObject playerInstance = ServiceLocator.Get<LevelManager>().playerInstance;
            GameObject towerInstance = ServiceLocator.Get<LevelManager>().towerInstance;
            if (isLose)
            {
                if (!playerInstance.GetComponent<Player>().isAlive)
                {
                    _camera.transform.LookAt(playerInstance.transform);
                    while (_camera.fieldOfView > 20.0f)
                    {
                        _camera.fieldOfView -= 60.0f * Time.deltaTime;
                        _camera.transform.LookAt(playerInstance.transform);
                        yield return new WaitForSeconds(0.01f);
                    }
                }
                else if (towerInstance.GetComponent<Tower>().fullHealth <= 0.0f)
                {
                    _camera.transform.LookAt(towerInstance.transform);
                    while (_camera.fieldOfView > 20.0f)
                    {
                        _camera.fieldOfView -= 60.0f * Time.deltaTime;
                        _camera.transform.LookAt(towerInstance.transform);
                        yield return new WaitForSeconds(0.01f);
                    }

                }
            }
            else
            {
                _camera.transform.LookAt(playerInstance.transform);
                while (_camera.fieldOfView > 20.0f)
                {
                    _camera.fieldOfView -= 60.0f * Time.deltaTime;
                    _camera.transform.LookAt(playerInstance.transform);
                    yield return new WaitForSeconds(0.01f);
                }

            }

            yield return new WaitForSeconds(delay);

            _camera.gameObject.transform.localPosition = Vector3.MoveTowards
            (_camera.gameObject.transform.localPosition,
            new Vector3(_camPos.x, _camPos.y,
            _camPos.z), 100.0f);
            while (_camera.fieldOfView < 59.0f)
            {
                if (_camera)
                {
                    _camera.gameObject.transform.rotation = Quaternion.Slerp(_camera.transform.rotation, _camRot, 10.0f * Time.deltaTime);
                    _camera.fieldOfView += 60.0f * Time.deltaTime;
                    yield return new WaitForSeconds(0.01f);
                }
            }
            _camera.gameObject.transform.rotation = _camRot;
            _camera.fieldOfView = 60.0f;
            _camera.gameObject.transform.localPosition = _camPos;
            _isCameraUsing = false;
            yield return null;
        }
       
    }

    public IEnumerator ShakeCamera(float duration, float shakeAmount, float decreaseFactor)
    {
        if (_isCameraUsing)
            yield return null;
        else
        {
            _isCameraUsing = true;
            _camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            Vector3 originPos = _camera.transform.localPosition;
            float current = 0.0f;
            while (current < duration)
            {
                _camera.transform.localPosition = originPos + Random.insideUnitSphere * shakeAmount;
                current += Time.deltaTime * decreaseFactor;
                yield return new WaitForSeconds(0.01f);
            }

            _camera.transform.localPosition = originPos;
            _isCameraUsing = false;
            yield return null;
        }

    }

    IEnumerator DelayEnding(float delay)
    {
        yield return new WaitForSeconds(delay);
        yield return new WaitUntil(() => { return !_isCameraUsing; });
        StartCoroutine(ServiceLocator.Get<UIManager>().Reset());
        Time.timeScale = 0.0f;
        ServiceLocator.Get<UIManager>().endPanel.gameObject.SetActive(true);
        yield return null;
    }
    int GetLevelNumber()
    {
        string current = SceneManager.GetActiveScene().name;
        switch (current)
        {
            case "Level1":
                return 1;
            case "Level2":
                return 2;
            case "Level3":
                return 3;
            case "Level4":
                return 4;
            case "Level5":
                return 5;
            default:
                return 1;
        }
    }

    public void SaveScore()
    {
        var mongoInstance = FindObjectOfType<PlayerInfoMenuMongo>()?.GetComponent<PlayerInfoMenuMongo>();
        var mySqlInstance = FindObjectOfType<PlayerInfoMenuSQL>()?.GetComponent<PlayerInfoMenuSQL>();
        var levelManager = ServiceLocator.Get<LevelManager>();
        if (mongoInstance)
        {
            if (mongoInstance.currentPlayer == null)
                return;
            MatchMongo match = new MatchMongo(GetLevelNumber(), levelManager.score,System.DateTime.Now);
            string jsonData = JsonConvert.SerializeObject(match);
            DatabaseConnection.Instance.CreateMatch(jsonData, mongoInstance.currentPlayer.Id);
        }
        else if(mySqlInstance)
        {
            if (mySqlInstance.currentPlayer == null)
                return;
            MatchSQL match = new MatchSQL(1,mySqlInstance.currentPlayer.player_id,GetLevelNumber(), levelManager.score, System.DateTime.Now);
            string jsonData = JsonConvert.SerializeObject(match);
            DatabaseConnection.Instance.CreateMatch(jsonData, "");
        }
    }

    public void SetGameOver()
    {
        SaveScore();
        Time.timeScale = 1.0f;
        ServiceLocator.Get<UIManager>().endPanel.gameObject.SetActive(false);
        _GameState = GameState.MainMenu;
        SceneManager.LoadScene("GameOver");
    }

    public void SetGameWin()
    {
        SaveScore();
        Time.timeScale = 1.0f;
        ServiceLocator.Get<UIManager>().endPanel.gameObject.SetActive(false);

        Player player = ServiceLocator.Get<LevelManager>().playerInstance.GetComponent<Player>();
        Tower tower = ServiceLocator.Get<LevelManager>().towerInstance.GetComponent<Tower>();

        _racoonHP = player.health;
        _houseHP = tower.fullHealth;

        _GameState = GameState.MainMenu;
        string current = SceneManager.GetActiveScene().name;
        switch(current)
        {
            case "Level1":
                currentlevel = 1;
                break;
            case "Level2":
                currentlevel = 2;
                break;
            case "Level3":
                currentlevel = 3;
                break;
            case "Level4":
                currentlevel = 4;
                break;
            case "Level5":
                currentlevel = 5;
                break;
            default:
                currentlevel++;
                break;
        }

        if (SceneManager.GetActiveScene().name == "Level5")
        {
            sceneToLoad = "MainMenu";
            videoNumbertoPlay = 2;
            SceneManager.LoadScene("CutScene");
        }
        else
        {
            SceneManager.LoadScene("UpgradeMenu");
        }
    }

    public void changeGameState(GameState state)
    {
        _GameState = state;
        if (_GameState == GameState.GamePlay || _GameState == GameState.Tutorial)
        {
            ServiceLocator.Get<UIManager>().gameObject.SetActive(true);
            ServiceLocator.Get<UIManager>().StartCoroutine(ServiceLocator.Get<UIManager>().Reset());
            ServiceLocator.Get<LevelManager>().gameObject.SetActive(true);
        }
    }

    public void LoadTowerHP()
    {
        if (isVaribableLoaded)
            return;
        VariableLoader variableLoader = ServiceLocator.Get<VariableLoader>();
        if (variableLoader.useGoogleSheets)
        {
            _houseHP = variableLoader.TowerStats["Health"];
        }
        isVaribableLoaded = true;
    }
}
