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
    public bool _enemySkillActived { get; set; }

    public Dictionary<UpgradeMenu.Upgrade, int> upgradeLevelsDictionary = new Dictionary<UpgradeMenu.Upgrade, int>();
    public Dictionary<UpgradeMenu.Upgrade, bool> upgradeEnabled = new Dictionary<UpgradeMenu.Upgrade, bool>();
    public List<string> specialTargets = new List<string>();
    public string choosenTarget = "No Target";
    public BarricadeSpawner barricadeSpawner;

    private Camera _camera;
    private Vector3 _camPos;
    private float getDeltaTime { set; get; }

    [Header("Shaking camera")]
    private Vector3 originPos;
    public float shakeAmount = 0.7f;
    public float decrFactor = 1.0f;

    private bool _isCameraUsing = false;

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
                    StartCoroutine(ZoomInAndOut(0.5f, true));
                    StartCoroutine(DelayEnding(3.0f));
                }
                else if (ServiceLocator.Get<LevelManager>().CheckWinCondition())
                {
                    _GameState = GameState.GameWin;
                    StartCoroutine(ZoomInAndOut(0.5f, false));
                    StartCoroutine(DelayEnding(3.0f));
                }
                break;
            case GameState.Tutorial:
                if (ServiceLocator.Get<LevelManager>().CheckLoseCondition())
                {
                    _GameState = GameState.GameLose;
                    StartCoroutine(DelayEnding(3.0f));
                }
                else if (ServiceLocator.Get<LevelManager>().CheckWinCondition())
                {
                    _GameState = GameState.GameWin;
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

    public IEnumerator ShakeCamera(float duration)
    {
        if (_isCameraUsing)
            yield return null;
        else
        {
            _isCameraUsing = true;
            _camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            originPos = _camera.transform.localPosition;
            float current = 0.0f;
            while (current < duration)
            {
                _camera.transform.localPosition = originPos + Random.insideUnitSphere * shakeAmount;
                current += Time.deltaTime * decrFactor;
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
        StartCoroutine(ServiceLocator.Get<UIManager>().Reset());
        Time.timeScale = 0.0f;
        ServiceLocator.Get<UIManager>().endPanel.gameObject.SetActive(true);
        yield return null;
    }

    public void SetGameOver()
    {
        Time.timeScale = 1.0f;
        ServiceLocator.Get<UIManager>().endPanel.gameObject.SetActive(false);
        _GameState = GameState.MainMenu;
        SceneManager.LoadScene("GameOver");
    }

    public void SetGameWin()
    {
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
            default:
                currentlevel++;
                break;
        }

        if (SceneManager.GetActiveScene().name == "Level4")
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

    public void enemySkillActived()
    {
        Debug.Log("Act!");
        _enemySkillActived = true;
        if(_enemySkillActived)
        {
            if (coolTime < Time.time)
            {
                coolTime = 10.0f + Time.time;
                _enemySkillActived = false;
                Debug.Log("Stop!");
            }
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
