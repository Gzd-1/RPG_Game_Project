using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum EnemyState
    {
        NormalState,
        FightingState,
        MovingState,
        RestingState
    }

    private EnemyState state = EnemyState.NormalState;
    private EnemyState childState = EnemyState.RestingState;
    private NavMeshAgent enemyAgent;

    public float restTime = 2f;
    private float restTimer = 0f;

    public int HP = 100;
    public int exp = 20;

    void Start()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (state == EnemyState.NormalState)
        {
            if (childState == EnemyState.RestingState)
            {
                restTimer += Time.deltaTime;
                if (restTimer > restTime)
                {
                    Vector3 randomPosition = FindRandomPosition();
                    enemyAgent.SetDestination(randomPosition);
                    childState = EnemyState.MovingState;
                }
            }
            else if (childState == EnemyState.MovingState)
            {
                if (enemyAgent.remainingDistance <= 0.1f) // 使用阈值避免浮点误差
                {
                    restTimer = 0f;
                    childState = EnemyState.RestingState;
                }
            }
        }
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    TakeDamage(30);
        //}
    }

    Vector3 FindRandomPosition()
    {
        Vector3 randomDir = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
        return transform.position + randomDir.normalized * Random.Range(2f, 5f);
    }

    public void TakeDamage(int damage)
    {
        HP -= damage;
        if (HP <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        GetComponent<Collider>().enabled = false;
            int count = Random.Range(1,5);
            for (int i = 0; i < count; i++)
            {
                SpawnPickableItem();
            }
            EventCenter.EnemyDied(this);
            Destroy(gameObject);
    }
    private void SpawnPickableItem()
    {
        ItemSO item = ItemDBManager.Instance.GetRandomItem();
        //print(transform.position);
        GameObject go =GameObject.Instantiate(item.prefab, transform.position, Quaternion.identity);
        go.tag= Tag.INTERACTABLE;
        Animator anim = go.GetComponent<Animator>();
        if (anim != null)
        {
           anim.enabled = false;
        }
        PickableObject po= go.AddComponent<PickableObject>();
        po.itemSO = item;

        Collider collider = go.GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = true;
            collider.isTrigger = false;
        }
        Rigidbody rgd= go.GetComponent<Rigidbody>();
        if (rgd != null)
        {
            rgd.isKinematic = false;
            rgd.useGravity= true;
        }
    }
}