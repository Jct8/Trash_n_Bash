using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SheetCodes;

public class Barricade : MonoBehaviour/*, IDragHandler , IDropHandler */, IDraggableItem
{
    [SerializeField] private float _Defence = 10.0f;
    [SerializeField] public float health;
    [SerializeField] private float _PercentFromTower = 5.0f;
    [SerializeField] private float repairTime = 3.0f;
    [SerializeField] private float repairRange = 1.0f;

    private bool _CanBePickedUp = true;
    private float _MaxHealth = 0.0f;
    public float _barricadeBuildTime = 1.0f;
    public Image healthBar;
    public GameObject healthBarGO;
    public bool inRangeRepair = false;
    public bool isRepairing = false;
    public bool isAlive = true;
    public bool isPlaced = false;
    public GameObject destroyParticlePrefab;

    ICharacterSound characterSound;

    public void PickUp(GameObject playerGO)
    {
        Tower tower = ServiceLocator.Get<LevelManager>().towerInstance.GetComponent<Tower>();
        health = tower.MaxHealth * _PercentFromTower * 0.01f;
        _MaxHealth = health;
        if (_MaxHealth != 0.0f)
            healthBar.fillAmount = health / _MaxHealth;
        isAlive = true;
        //tower.TakeDamage(_Health);

        GetComponent<NavMeshObstacle>().enabled = false;
        float height = transform.position.y;

        transform.parent = playerGO.transform;
        transform.rotation = playerGO.GetComponent<PlayerController>().GetController().transform.rotation;
        transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f), 90);

        transform.position = playerGO.transform.position;
        transform.position += playerGO.GetComponent<PlayerController>().GetController().transform.forward;
        transform.position += new Vector3(0.0f, height, 0.0f);
    }

    public bool CanBePickedUp()
    {
        return _CanBePickedUp;
    }

    private void Awake()
    {
        _MaxHealth = health;
        characterSound = GetComponent<ICharacterSound>();
    }

    private void Start()
    {
        characterSound = GetComponent<ICharacterSound>();
        VariableLoader variableLoader = ServiceLocator.Get<VariableLoader>();
        if (variableLoader.useGoogleSheets)
        {
            health = variableLoader.BarriacdeStats["Health"];
            _MaxHealth = variableLoader.BarriacdeStats["Health"];
            // _barricadeBuildTime = variableLoader.BarriacdeStats["BuildTime"];
        }
        ///////////  Upgrades - Improved Barricades  ///////////
        int level = ServiceLocator.Get<GameManager>().upgradeLevelsDictionary[UpgradeMenu.Upgrade.ImprovedBarricades];
        UpgradesIdentifier upgradesIdentifier = ModelManager.UpgradesModel.GetUpgradeEnum(UpgradeMenu.Upgrade.ImprovedBarricades, level);
        if (level >= 1 && ServiceLocator.Get<GameManager>().upgradeEnabled[UpgradeMenu.Upgrade.ImprovedBarricades])
            health += ModelManager.UpgradesModel.GetRecord(upgradesIdentifier).ModifierValue;

        healthBarGO.transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up);
    }

    private void Update()
    {
    }

    public void PlaceBarricade()
    {
        _CanBePickedUp = false;
        transform.parent = null;
        isPlaced = true;
        GetComponent<NavMeshObstacle>().enabled = true;
        GetComponent<NavMeshObstacle>().carving = true;
        StartCoroutine(characterSound.BarricadeSound(0));
    }

    public bool CheckRepairValid(Transform playerTransform)
    {
        if (Vector2.Distance(transform.position, playerTransform.position) < repairRange & isRepairing)
        {
            return true;
        }
        StopCoroutine(Repair());
        isRepairing = false;
        return false;
    }

    public IEnumerator Repair()
    {
        isRepairing = true;
        StartCoroutine(characterSound.BarricadeSound(3));
        yield return new WaitForSeconds(repairTime);
        if (isRepairing)
        {
            health = _MaxHealth;
            if (_MaxHealth != 0.0f)
                healthBar.fillAmount = health / _MaxHealth;
        }
        isRepairing = false;
        isAlive = true;

        if (ServiceLocator.Get<LevelManager>().isTutorial == true)
        {
            TutorialManager tutorialManager = GameObject.FindGameObjectWithTag("TutorialManager").GetComponent<TutorialManager>();
            tutorialManager.EndTutorial();
        }
    }

    public void TakeDamage(float dmg)
    {
        health -= dmg;
        StartCoroutine(characterSound.BarricadeSound(2));
        if (_MaxHealth != 0.0f)
            healthBar.fillAmount = health / _MaxHealth;
        if (health <= 0.0f)
        {
            isAlive = false;
            if (destroyParticlePrefab)
            {
                GameObject go = Instantiate(destroyParticlePrefab);
                go.transform.position = transform.position;
                go.transform.rotation = transform.rotation;
            }
            Destroy(gameObject);

        }
    }

    public void TakeFullDamage()
    {
        health -= _MaxHealth;
        if (_MaxHealth != 0.0f)
            healthBar.fillAmount = health / _MaxHealth;
        if (health <= 0.0f)
        {
            isAlive = false;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!isPlaced)
        {
            PlaceBarricade();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isPlaced)
        {
            Plane plane = new Plane(Vector3.up, transform.position);
            Ray ray = eventData.pressEventCamera.ScreenPointToRay(eventData.position);
            float distance;
            if (plane.Raycast(ray, out distance))
            {
                transform.position = ray.origin + ray.direction * distance;
            }

            StartCoroutine(characterSound.BarricadeSound(1));
        }

    }

    public bool DropItem()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.gameObject.CompareTag("Ground"))
            {
                transform.position = hit.point;
                gameObject.SetActive(true);
                PlaceBarricade();
                return true;
            }
            else
            {
                ServiceLocator.Get<GameManager>().barricadeSpawner.ResetBarricade();
                Destroy(gameObject);
                return false;
            }
        }
        return false;
    }
}
