using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player Data")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float groundDistance;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask clickableLayer;
    private Vector2 movement;
    private Rigidbody myRBD;
    public bool isJump = false;
    public bool isGrounded = true;
    private NavMeshAgent navMeshAgent;
    public bool isUsingNavMesh = false;

    private void Awake()
    {
        myRBD = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.enabled = true;
    }
    private void FixedUpdate()
    {
        CheckGround();

        if (isJump)
        {
            Jump(); 
        }
        if (!isUsingNavMesh)
        {
            ApplyPhysics();
        }
        else
        {
            CheckNavMeshMovement();
        }
    }
    public void OnMovement(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();

        if (movement != Vector2.zero)
        {
            isUsingNavMesh = false;
            navMeshAgent.enabled = false;
        }
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            isJump = true;
        }
    }
    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, clickableLayer))
            {
                navMeshAgent.enabled = true;
                navMeshAgent.SetDestination(hit.point); 
                navMeshAgent.isStopped = false;
                isUsingNavMesh = true;
            }
        }
    }
    private void ApplyPhysics()
    {
        myRBD.velocity = new Vector3(movement.x * speed, myRBD.velocity.y, movement.y * speed);
    }
    private void Jump()
    {
        if (isGrounded)
        {
            myRBD.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJump = false;
        }
    }
    private void CheckGround()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundDistance, groundLayer);
    }
    private void CheckNavMeshMovement()
    {
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            isUsingNavMesh = false;
            navMeshAgent.isStopped = true; 
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundDistance);
    }
}
