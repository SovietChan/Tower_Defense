using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    // Start is called before the first frame update
    void Start()
    {
        InstantiateAllTowerUI();
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
