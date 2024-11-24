using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class EnemyControl : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private List<Transform> patrolPoints = new List<Transform>();
    [SerializeField] private float speedPatrol;
    [SerializeField] private float speedChase;

    [Header("Chase Settings")]
    [SerializeField] private Transform player;

    [Header("Post-Processing")]
    [SerializeField] private Volume postProcessVolume; 
    private Vignette vignette; 
    [SerializeField] private Color vignetteColorChase = Color.red;
    [SerializeField] private float vignetteIntensityChase = 20f;
    [SerializeField] private float vignetteIntensityNormal = 0.0f;

    private NavMeshAgent agent;
    private int currentPatrolIndex = 0;
    private bool isChasing = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        if (postProcessVolume != null && postProcessVolume.profile != null)
        {
            List<VolumeComponent> components = postProcessVolume.profile.components;
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i] is Vignette)
                {
                    vignette = (Vignette)components[i];
                    break;
                }
            }
        }
    }
    private void Start()
    {
        agent.speed = speedPatrol;
        postProcessVolume.enabled = false;

        if (patrolPoints.Count > 0)
        {
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
        if (vignette != null)
        {
            vignette.intensity.value = vignetteIntensityNormal;
            vignette.color.value = Color.black; 
        }
    }
    void Update()
    {
        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            EnemyPatrol();
        }
    }
    private void ChasePlayer()
    {
        if (!agent.pathPending)
        {
            agent.SetDestination(player.position);
        }
    }
    private void EnemyPatrol()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            isChasing = true;
            agent.speed = speedChase;
            postProcessVolume.enabled = true;

            if (vignette != null)
            {
                vignette.color.value = vignetteColorChase;
                vignette.intensity.value = vignetteIntensityChase;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            isChasing = false; 
            agent.speed = speedPatrol;
            postProcessVolume.enabled = false;

            if (vignette != null)
            {
                vignette.color.value = Color.black;
                vignette.intensity.value = vignetteIntensityNormal;
            }

            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }
}
