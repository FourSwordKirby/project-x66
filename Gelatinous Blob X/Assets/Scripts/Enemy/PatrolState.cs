using UnityEngine;
using System.Collections.Generic;

public class PatrolState : State<Enemy> {

    public struct Options
    {
        public float pauseTime, rotationSpeed, rotationFov;

        public Options(float pTime, float rSpeed, float rFov)
        {
            pauseTime = pTime;
            rotationSpeed = rSpeed;
            rotationFov = rFov;
        }
    }
    public Options CreateOptions()
    {
        return new Options(DEFAULT_PAUSE_TIME, DEFAULT_ROTATION_SPEED, DEFAULT_FOV);
    }

    private const float DEFAULT_PAUSE_TIME = 0.0f;
    private const float DEFAULT_ROTATION_SPEED = 20.0f;
    private const float DEFAULT_FOV = 45.0f;

    private PatrolPath path;
    private int currentTargetIndex;
    private SeekTargetWithPathState[] seekStates;

    private float pauseTime;
    private float patrolPauseTimer;

    private float rotationSpeed;
    private float rotationFov;
    private bool stopped;
    private Vector3 targetDirection;

    public PatrolState(Enemy owner, StateMachine<Enemy> fsm, PatrolPath patrolPath, float pauseTime = 0.0f)
        : base(owner, fsm)
    {
        this.path = patrolPath;
        this.seekStates = null;
        this.currentTargetIndex = -1;
        this.pauseTime = pauseTime;
        this.patrolPauseTimer = pauseTime;

        this.rotationSpeed = DEFAULT_ROTATION_SPEED;
        this.rotationFov = DEFAULT_FOV;
        this.stopped = false;
        this.targetDirection = Owner.transform.forward;
    }

    private SeekTargetWithPathState getSeekState()
    {
        if(seekStates == null || seekStates.Length != path.Path.Count)
        {
            seekStates = new SeekTargetWithPathState[path.Path.Count];
        }
        if(seekStates[currentTargetIndex] == null)
        {
            seekStates[currentTargetIndex] = new SeekTargetWithPathState(Owner, Fsm, path.Path[currentTargetIndex].gameObject, 0.5f);
        }
        return seekStates[currentTargetIndex];
    }

    private int getClosestPatrolNodeIndex()
    {
        int closestIndex = 0;
        float closestDistance = 100000000.0f;
        for (int i = 0; i < path.Path.Count; ++i)
        {
            PatrolNode node = path.Path[i];
            float distance = (node.transform.position - Owner.transform.position).sqrMagnitude;
            if (distance < closestDistance)
            {
                closestIndex = i;
                closestDistance = distance;
            }
        }
        return closestIndex;
    }

    public override void Enter()
    {
        if(path == null)
        {
            throw new MissingReferenceException(Owner.name + " does not have a Patrol Path object.");
        }
        Debug.Log("State enter");
        currentTargetIndex = getClosestPatrolNodeIndex();
    }

    public override void Execute()
    {
        SeekTargetWithPathState state = getSeekState();
        state.Execute();
        if(state.IsAtTarget())
        {
            if (!stopped)
            {
                stopped = true;
                targetDirection = Quaternion.AngleAxis(rotationFov, Owner.transform.up) * Owner.transform.forward;
            }
            if(Vector3.Angle(Owner.transform.forward, targetDirection) < 10.0f)
            {
                rotationFov *= -1.0f;
                targetDirection = Quaternion.AngleAxis(2.0f * rotationFov, Owner.transform.up) * targetDirection;
            }
            rotationSpeed = Mathf.Abs(rotationSpeed) * Mathf.Sign(rotationFov);

            patrolPauseTimer -= Time.deltaTime;
            if(patrolPauseTimer < 0.0f)
            {
                currentTargetIndex = (currentTargetIndex + 1) % path.Path.Count;
                patrolPauseTimer = pauseTime;
                stopped = false;
            }
        }
    }

    public override void FixedExecute()
    {
        SeekTargetWithPathState state = getSeekState();
        state.FixedExecute();
        if (stopped)
        {
            Owner.transform.rotation = Quaternion.LookRotation(Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, Owner.transform.up) * Owner.transform.forward, Owner.transform.up);
        }
    }

    public override void Exit()
    {
        // Do nothing
    }
}
