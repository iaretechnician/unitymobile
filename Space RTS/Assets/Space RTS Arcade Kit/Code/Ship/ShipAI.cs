using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

/// <summary>
/// Class specifically to deal with input.
/// </summary>
public class ShipAI : MonoBehaviour
{
    public Order CurrentOrder;

    // Variables used by AI orders
    [HideInInspector] public List<Transform> wayPointList;
    [HideInInspector] public int nextWayPoint;
    [HideInInspector] public Vector3 tempDest;
    [HideInInspector] public float throttle;

    // Used by Steering Action
    [HideInInspector] public Vector3 angularTorque;
    [HideInInspector] public PIDController pid_angle, pid_velocity;
    [HideInInspector] public float pid_P = 10, pid_I = 0.5f, pid_D = 0.5f;

    [HideInInspector] public Rigidbody rBody;
    [HideInInspector] public Ship ship;

    [HideInInspector] public static event EventHandler OnOrderFinished;

    // Collision avoidance overrides any order execution
    private bool avoidCollision = false;
    // Temp destination will be overriden if avoiding a collision, so remember the original
    private Vector3 savedTempDest = Vector3.zero;
    private const float COLLISION_RAYCAST_DIST = 50f;

    private void Awake()
    {
        ship = GetComponent<Ship>();
        rBody = GetComponent<Rigidbody>();

        pid_angle = new PIDController(pid_P, pid_I, pid_D);
        pid_velocity = new PIDController(pid_P, pid_I, pid_D);

        wayPointList = new List<Transform>();
    }

    private void Update()
    {
        if (ship.IsPlayerControlled)
            return;

        // If an order is present, perform it
        if (CurrentOrder != null)
        {
            
            CheckForwardCollisions();
            

            if (avoidCollision)
            {
                ShipSteering.SteerTowardsTarget(this);
                if (Vector3.Angle(transform.forward, tempDest - transform.position) < 10)
                    throttle = 0.5f;
                else
                    throttle = 0f;
                // If the avoidance destination was reached, resume flight towards original destination
                if (Vector3.Distance(transform.position, tempDest) < 10)
                {
                    tempDest = savedTempDest;
                    avoidCollision = false;
                }
            }
            else
            {
                if (CurrentOrder != null)
                {
                    // Update the order
                    CurrentOrder.UpdateState(this);
                }
                else
                {
                    throttle = 0f;
                }
            }
        }
    }

    private void OnDestroy()
    {
        if(OnOrderFinished != null)
            OnOrderFinished(ship, EventArgs.Empty);   // Notify listeners
    }

    /// <summary>
    /// Finish current ship order and clean up.
    /// </summary>
    public void FinishOrder()
    {
        if (CurrentOrder == null)
            return;

        OnOrderFinished(ship, EventArgs.Empty);   // Notify listeners

        CurrentOrder.Destroy();
        CurrentOrder = null;
        tempDest = Vector3.zero;
        avoidCollision = false;

        wayPointList.Clear();
        ConsoleOutput.PostMessage(name + " has completed the order.");

        ship.PlayerInput.throttle = 0f;

        if (ship == Ship.PlayerShip)
            ship.IsPlayerControlled = true;
    }

    #region collision avoidance
    private Vector3 upOffset = new Vector3(0, 3, 0),
        downOffset = new Vector3(0, -3, 0),
        leftOffset = new Vector3(-3, 0, 0),
        rightOffset = new Vector3(3, 0, 0);
    
    private void CheckForwardCollisions()
    {
        if (throttle < 0.05f)
            return;

        // Shoot 4 raycasts from each tip of the ship

        RaycastHit hit;
        float minDistance = COLLISION_RAYCAST_DIST*2;
        Vector3 avoidancePosition = Vector3.zero;

        Debug.DrawRay(transform.position + upOffset, transform.forward);
        if (Physics.Raycast(transform.position+upOffset, transform.forward, out hit, COLLISION_RAYCAST_DIST))
        {
            if(hit.transform.tag == "Asteroid" || hit.transform.tag == "Station")
                if(hit.distance < minDistance)
                {
                    savedTempDest = tempDest;
                    minDistance = hit.distance;
                    avoidancePosition = transform.position + transform.up * minDistance - transform.right * minDistance;
                }
        }
        Debug.DrawRay(transform.position + downOffset, transform.forward);
        if (Physics.Raycast(transform.position + downOffset, transform.forward, out hit, COLLISION_RAYCAST_DIST))
        {
            if (hit.transform.tag == "Asteroid" || hit.transform.tag == "Station")
                if (hit.distance < minDistance)
                {
                    savedTempDest = tempDest;
                    minDistance = hit.distance;
                    avoidancePosition = transform.position + transform.up * minDistance - transform.right * minDistance;
                }
        }
        Debug.DrawRay(transform.position + rightOffset, transform.forward);
        if (Physics.Raycast(transform.position + rightOffset, transform.forward, out hit, COLLISION_RAYCAST_DIST))
        {
            if (hit.transform.tag == "Asteroid" || hit.transform.tag == "Station")
                if (hit.distance < minDistance)
                {
                    savedTempDest = tempDest;
                    minDistance = hit.distance;
                    avoidancePosition = transform.position + transform.up * minDistance - transform.right * minDistance;
                }
        }
        Debug.DrawRay(transform.position + leftOffset, transform.forward);
        if (Physics.Raycast(transform.position + leftOffset, transform.forward, out hit, COLLISION_RAYCAST_DIST))
        {
            if (hit.transform.tag == "Asteroid" || hit.transform.tag == "Station")
                if (hit.distance < minDistance)
                {
                    savedTempDest = tempDest;
                    minDistance = hit.distance;
                    avoidancePosition = transform.position + transform.up * minDistance - transform.right * minDistance;
                }
        }

        if (minDistance != COLLISION_RAYCAST_DIST*2)
        {
            tempDest = avoidancePosition;
            avoidCollision = true;
        }
    }
    #endregion collision avoidance

    // Autopilot commands
    #region commands
    /// <summary>
    /// Commands the ship to move to a given object.
    /// </summary>
    /// <param name="destination"></param>
    public void MoveTo(Transform destination)
    {
        FinishOrder();
        ship.IsPlayerControlled = false;
        if (destination != null)
        {
            wayPointList.Clear();
            wayPointList.Add(destination);
            nextWayPoint = 0;

            CurrentOrder = new OrderMove();
        }        
    }

    public void MoveWaypoints(List<Vector3> selectedWaypoints)
    {
        FinishOrder();
        ship.IsPlayerControlled = false;
        if (selectedWaypoints != null)
        {
            CurrentOrder = new OrderMoveWaypoints(selectedWaypoints);
        }
    }

    /// <summary>
    /// Commands the ship to move to a specified position.
    /// </summary>
    /// <param name="position">world position of destination</param>
    public void MoveTo(Vector3 position)
    {
        FinishOrder();
        tempDest = position;

        if (position == Vector3.zero)
            return;
        ship.IsPlayerControlled = false;

        CurrentOrder = new OrderMove();
    }

    /// <summary>
    /// Commands the ship to move through the given waypoints. Once the last one is reached,
    /// the route is restarted from the first waypoint.
    /// </summary>
    /// <param name="waypoints"></param>
    public void PatrolPath(Transform[] waypoints)
    {
        FinishOrder();
        ship.IsPlayerControlled = false;
        CurrentOrder = new OrderPatrol();

        wayPointList.Clear();

    }

    /// <summary>
    /// Commands the ship to move randomly at low speed, roughly in the same area.
    /// </summary>
    public void Idle()
    {
        FinishOrder();
        ship.IsPlayerControlled = false;
        CurrentOrder = new OrderIdle();
        tempDest = transform.position;

    }

    /// <summary>
    /// Commands the ship to follow player ship
    /// </summary>
    public void FollowMe()
    {
        FinishOrder();
        // Cant chase its own tail
        if (ship == Ship.PlayerShip)
            return;

        ship.IsPlayerControlled = false;
        CurrentOrder = new OrderFollow(this, Ship.PlayerShip);

        wayPointList.Clear();
        wayPointList.Add(Ship.PlayerShip.transform);
        nextWayPoint = 0;

    }

    /// <summary>
    /// Commands the ship to follow a target
    /// </summary>
    public void Follow(Transform target)
    {
        FinishOrder();
        ship.IsPlayerControlled = false;
        if (target != null)
        {
            wayPointList.Clear();
            wayPointList.Add(target);
            nextWayPoint = 0;

            CurrentOrder = new OrderFollow(this, target.GetComponent<Ship>());
        }
       
    }

    /// <summary>
    /// Commands the ship to attack an object
    /// </summary>
    public void Attack(GameObject target)
    {
        FinishOrder();
        CurrentOrder = new OrderAttack();
        ship.IsPlayerControlled = false;

        wayPointList.Clear();
        wayPointList.Add(target.transform);
        nextWayPoint = 0;

    }

    /// <summary>
    /// Commands the ship to attack all enemies in the area
    /// </summary>
    public void AttackAll()
    {
        FinishOrder();
        CurrentOrder = new OrderAttackAll();
        ship.IsPlayerControlled = false;

        wayPointList.Clear();
        nextWayPoint = 0;
    }

    #endregion commands
}