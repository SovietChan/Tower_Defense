using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private int _bulletPower;
    private float _bulletSpeed;
    private float _bulletSplashRadius;

    private EnemyController _targetEnemy;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (_targetEnemy != null)
        {
            if (!_targetEnemy.gameObject.activeSelf)
            {
                gameObject.SetActive(false);
                _targetEnemy = null;
                return;
            }

            transform.position = Vector3.MoveTowards(transform.position, _targetEnemy.transform.position, _bulletSpeed * Time.fixedDeltaTime);
            Vector3 direction = _targetEnemy.transform.position - transform.position;
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, targetAngle - 90f));

        }
        if (LevelManager.Instance.IsOver)
        {
            return;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_targetEnemy == null)
        {
            return;
        }
        if (collision.gameObject.Equals(_targetEnemy.gameObject))
        {
            gameObject.SetActive(false);
            if (_bulletSplashRadius > 0f)
            {
                LevelManager.Instance.ExplodeAt(transform.position, _bulletSplashRadius, _bulletPower);
            }
            else
            {
                _targetEnemy.ReduceEnemyHealth(_bulletPower);
            }
            _targetEnemy = null;
        }
    }

    public void SetProperties(int bulletPower, float bulletSpeed, float bulletSplashRadius)
    {
        _bulletPower = bulletPower;
        _bulletSpeed = bulletSpeed;
        _bulletSplashRadius = bulletSplashRadius;
    }

    public void SetTargetEnemy(EnemyController enemyController)
    {
        _targetEnemy = enemyController;
    }
}
