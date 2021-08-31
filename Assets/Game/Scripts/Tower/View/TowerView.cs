using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerView : MonoBehaviour
{
    [SerializeField] private Image _towerIcon;
    private TowerController _towerPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetTowerPrefab(TowerController towerController)
    {
        _towerPrefab = towerController;
        _towerIcon.sprite = towerController.GetTowerHeadIcon();
    }
}
