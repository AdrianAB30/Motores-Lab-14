using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.AI;
using System;

public class CoinControl : MonoBehaviour
{
    [SerializeField] private float timeEase;          
    [SerializeField] private Ease myEase;             
    [SerializeField] private float timeToReachTarget;
    private AudioSource coinCollected;
    private NavMeshAgent navAgent;
    private Transform coin;
    private Vector3 randomTarget;

    public static event Action<int> OnCoinCollected;

    private void Awake()
    {
        coinCollected = GetComponent<AudioSource>();
        navAgent = GetComponent<NavMeshAgent>();
        coin = GetComponent<Transform>();
    }
    private void Start()
    {
        RotateCoin();
        MoveCoinToRandomPosition();
    }
    private void RotateCoin()
    {
        if (coin != null)
        {
            coin.DORotate(new Vector3(0, 360, 0), timeEase, RotateMode.LocalAxisAdd).SetEase(myEase).SetLoops(-1, LoopType.Incremental);
        }
    }
    private void MoveCoinToRandomPosition()
    {
        randomTarget = GetRandomNavMeshPosition();

        navAgent.SetDestination(randomTarget);

        StartCoroutine(ChangeDestination());
    }
    private IEnumerator ChangeDestination()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeToReachTarget);

            randomTarget = GetRandomNavMeshPosition();

            navAgent.SetDestination(randomTarget);
        }
    }
    private Vector3 GetRandomNavMeshPosition()
    {
        Vector3 randomPos = new Vector3(UnityEngine.Random.Range(-10f, 27f), 0f, UnityEngine.Random.Range(-25f, 1f)); 
        NavMeshHit hit;

        if (NavMesh.SamplePosition(randomPos, out hit, 3f, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return randomPos;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            coinCollected.Play();
            OnCoinCollected?.Invoke(1);
            StartCoroutine(DestroyCoinAfterSound());
        }
    }
    private IEnumerator DestroyCoinAfterSound()
    {
        yield return new WaitForSeconds(coinCollected.clip.length);
        Destroy(gameObject);
    }
}
