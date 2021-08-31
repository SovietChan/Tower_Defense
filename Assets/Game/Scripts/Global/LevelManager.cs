using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    private static LevelManager _instance = null;
    public static LevelManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<LevelManager>();
            }
            return _instance;
        }
    }

    [SerializeField] private Transform _towerUIParent;
    [SerializeField] private GameObject _towerUIPrefab;

    [SerializeField] private TowerController[] _towerPrefabs;
    [SerializeField] private EnemyController[] _enemyPrefabs;

    [SerializeField] private Transform[] _enemyPaths;

    [SerializeField] private float _spawnDelay = 5f;

    private List<TowerController> _spawnedTowers = new List<TowerController>();
    private List<EnemyController> _spawnedEnemies = new List<EnemyController>();
    private List<BulletController> _spawnedBullets = new List<BulletController>();

    private float _runningSpawnDelay;

    public bool IsOver { get; private set; }

    [SerializeField] private int _maxLives = 3;
    [SerializeField] private int _totalEnemy = 15;

    [SerializeField] private GameObject _panel;
    [SerializeField] private Text _statusInfo;
    [SerializeField] private Text _livesInfo;
    [SerializeField] private Text _totalEnemyInfo;

    private int _currentLives;
    private int _enemyCounter;
    // Start is called before the first frame update
    void Start()
    {
        InstantiateAllTowerUI();
        SetCurrentLives(_maxLives);
        SetTotalEnemy(_totalEnemy);
    }

    // Update is called once per frame
    private void Update()
    {
        _runningSpawnDelay -= Time.unscaledDeltaTime;
        if (_runningSpawnDelay <= 0f)
        {
            SpawnEnemy();
            _runningSpawnDelay = _spawnDelay;
        }

        foreach (TowerController tower in _spawnedTowers)
        {
            tower.CheckNearestEnemy(_spawnedEnemies);
            tower.SeekTarget();
            tower.ShootTarget();
        }

        foreach (EnemyController enemyController in _spawnedEnemies)
        {
            if (!enemyController.gameObject.activeSelf)
            {
                continue;
            }
            if (Vector2.Distance(enemyController.transform.position, enemyController.TargetPosition) < 0.1f)
            {
                enemyController.SetCurrentPathIndex(enemyController.CurrentPathIndex + 1);
                if (enemyController.CurrentPathIndex < _enemyPaths.Length)
                {
                    enemyController.SetTargetPosition(_enemyPaths[enemyController.CurrentPathIndex].position);
                }

                else
                {
                    ReduceLives(1);
                    enemyController.gameObject.SetActive(false);
                }
            }
            else
            {
                enemyController.MoveToTarget();
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (IsOver)
        {
            return;
        }
    }

    private void InstantiateAllTowerUI()
    {
        foreach (TowerController towerController in _towerPrefabs)
        {
            GameObject newTowerViewObj = Instantiate(_towerUIPrefab.gameObject, _towerUIParent);
            TowerView newTowerView = newTowerViewObj.GetComponent<TowerView>();
            newTowerView.SetTowerPrefab(towerController);
            newTowerView.transform.name = towerController.name;
        }

    }

    public void RegisterSpawnedTower(TowerController towerController)
    {
        _spawnedTowers.Add(towerController);
    }

    private void SpawnEnemy()
    {
        SetTotalEnemy(--_enemyCounter);
        if (_enemyCounter < 0)
        {
            bool isAllEnemyDestroyed = _spawnedEnemies.Find(e => e.gameObject.activeSelf) == null;
            if (isAllEnemyDestroyed)
            {
                SetGameOver(true);
            }
            return;
        }
        int randomIndex = Random.Range(0, _enemyPrefabs.Length);
        string enemyIndexString = (randomIndex + 1).ToString();

        GameObject newEnemyControllerObj = _spawnedEnemies.Find(
            e => !e.gameObject.activeSelf && e.name.Contains(enemyIndexString)
        )?.gameObject;

        if (newEnemyControllerObj == null)
        {
            newEnemyControllerObj = Instantiate(_enemyPrefabs[randomIndex].gameObject);
        }

        EnemyController newEnemyController = newEnemyControllerObj.GetComponent<EnemyController>();

        if (!_spawnedEnemies.Contains(newEnemyController))
        {
            _spawnedEnemies.Add(newEnemyController);
        }

        newEnemyController.transform.position = _enemyPaths[0].position;
        newEnemyController.SetTargetPosition(_enemyPaths[1].position);
        newEnemyController.SetCurrentPathIndex(1);
        newEnemyController.gameObject.SetActive(true);

    }
    public BulletController GetBulletFromPool(BulletController prefab)
    {
        GameObject newBulletControllerObj = _spawnedBullets.Find(
            b => !b.gameObject.activeSelf && b.name.Contains(prefab.name)
        )?.gameObject;

        if (newBulletControllerObj == null)
        {
            newBulletControllerObj = Instantiate(prefab.gameObject);
        }

        BulletController newBulletController = newBulletControllerObj.GetComponent<BulletController>();

        if (!_spawnedBullets.Contains(newBulletController))
        {
            _spawnedBullets.Add(newBulletController);
        }
        return newBulletController;
    }



    public void ExplodeAt(Vector2 point, float radius, int damage)
    {
        foreach (EnemyController enemyController in _spawnedEnemies)
        {
            if (enemyController.gameObject.activeSelf)
            {
                if (Vector2.Distance(enemyController.transform.position, point) <= radius)
                {
                    enemyController.ReduceEnemyHealth(damage);
                }
            }
        }
    }

    public void ReduceLives(int value)
    {
        SetCurrentLives(_currentLives - value);
        if (_currentLives <= 0)
        {
            SetGameOver(false);
        }
    }

    public void SetCurrentLives(int currentLives)
    {
        _currentLives = Mathf.Max(currentLives, 0);
        _livesInfo.text = $"Lives: {_currentLives}";
    }

    public void SetTotalEnemy(int totalEnemy)
    {
        _enemyCounter = totalEnemy;
        _totalEnemyInfo.text = $"Total Enemy: {Mathf.Max(_enemyCounter, 0)}";
    }

    public void SetGameOver(bool isWin)
    {
        IsOver = true;
        _statusInfo.text = isWin ? "You Win!" : "You Lose!";
        _panel.gameObject.SetActive(true);
    }
    private void OnDrawGizmos()
    {
        for (int i = 0; i < _enemyPaths.Length - 1; i++)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(_enemyPaths[i].position, _enemyPaths[i + 1].position);
        }
    }
}
