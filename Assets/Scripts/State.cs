using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class State 
{

    public enum STATE
    {
        IDLE, PATROL, PURSUE, ATTACK, SLEEP
    };
    public enum EVENT
    {
        ENTER, UPDATE, EXIT
    };
    public STATE name;
    protected EVENT stage;
    protected GameObject npc;
    protected Animator anim;
    protected Transform player;
    protected State nextState;
    protected NavMeshAgent agent;

    float visDist = 10.0f;
    float visAngle = 30.0f;
    float shootDist = 1.0f;

    public State(GameObject _npc, Animator _anim, NavMeshAgent _agent, Transform _player)
    {
        npc = _npc;
        agent = _agent;
        anim = _anim;
        player = _player;
        stage = EVENT.ENTER;
    }

    public virtual void Enter()
    {
        stage = EVENT.UPDATE;
    }

    public virtual void Update()
    {
        stage = EVENT.UPDATE;
    }

    public virtual void Exit()
    {
        stage = EVENT.EXIT;
    }
    public State Process()
    {
        if (stage == EVENT.ENTER) Enter();
        if (stage == EVENT.UPDATE) Update();
        if (stage == EVENT.EXIT)
        {
            Exit();
            return nextState;
        }
        return this;
    }

    //Used to check if the player is in enemy's visible range or not.
    public bool CanSeePlayer()
    {
        Vector3 direction = player.position - npc.transform.position;
        float angle = Vector3.Angle(direction, npc.transform.forward);
        if(direction.magnitude<=visDist && angle<visAngle)
        {
            return true;
        }
        return false;
    }

    //Used to check if the player can be attacked or not.
    public bool CanAttackPlayer()
    {
        Vector3 direction = player.position - npc.transform.position;
        float angle = Vector3.Angle(direction, npc.transform.forward);
        if(direction.magnitude<=shootDist)
        {
            return true;
        }
        return false;
    }


}

public class Idle : State
{
    public Idle(GameObject _npc, Animator _anim, NavMeshAgent _agent, Transform _player)
        : base(_npc, _anim, _agent, _player)
    {
        name = STATE.IDLE;
    }

    public override void Enter()
    {
        //anim.SetTrigger("isIdle");
        anim.SetFloat("Blend", 0f);

        base.Enter();
        Debug.Log("Idle State Entered");
    }

    public override void Update()
    {
        if(CanSeePlayer())
        {
            Debug.Log("Entering Pursue State");
            nextState = new Pursue(npc, anim, agent, player);
            stage = EVENT.EXIT;
        }

        else if (Random.Range(0, 100) < 10)
        {
            nextState = new Patrol(npc, anim, agent, player);
            stage = EVENT.EXIT;
        }
        
    }

    public override void Exit()
    {
        //anim.ResetTrigger("isIdle");
        base.Exit();
    }
};

public class Patrol : State
{
    int currentIndex = -1;
    public Patrol(GameObject _npc, Animator _anim, NavMeshAgent _agent,  Transform _player)
        : base(_npc, _anim, _agent, _player)
    {
        name = STATE.PATROL;
        agent.speed = 2;
        agent.isStopped = false;
    }

    public override void Enter()
    {
        Debug.Log("Walking State Entered");
        currentIndex = 0;
        //anim.SetTrigger("isWalking");
        anim.SetFloat("Blend", 2.0f);
        base.Enter();
    }
    public override void Update()
    {
        if (CanSeePlayer())
        {
            Debug.Log("Entering Pursue State");
            nextState = new Pursue(npc, anim, agent, player);
            stage = EVENT.EXIT;
        }

        if (agent.remainingDistance < 1)
        {
            if (currentIndex >= GameEnvironment.Singleton.Checkpoints.Count - 1)
            {
                currentIndex = 0;
            }
            else
            {
                currentIndex++;
            }
            agent.SetDestination(GameEnvironment.Singleton.Checkpoints[currentIndex].transform.position);
        }
    }

    public override void Exit()
    {
        //anim.ResetTrigger("isWalking");
        base.Exit();
    }
};

public class Pursue : State
{
    public Pursue(GameObject _npc, Animator _anim, NavMeshAgent _agent, Transform _player)
        : base(_npc, _anim, _agent, _player)
    {
        name = STATE.PURSUE;
        agent.speed = 5;
        agent.isStopped = false;
    }

    public override void Enter()
    {
        anim.SetFloat("Blend", 6.0f);
        Debug.Log("Pursue State Entered");
        base.Enter();
    }

    public override void Update()
    {
        agent.SetDestination(player.position);
        if(agent.hasPath)
        {
            Debug.Log("Path Found");
            if(CanAttackPlayer())
            {
                Debug.Log("Entering Attack State");
                nextState = new Attack(npc, anim, agent, player);
                stage = EVENT.EXIT;
            }
            else if(!CanSeePlayer())
            {
                nextState = new Patrol(npc, anim, agent, player);
                stage = EVENT.EXIT;
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}

public class Attack : State
{
    float rotationSpeed = 2.0f;

    public Attack(GameObject _npc, Animator _anim, NavMeshAgent _agent, Transform _player)
        : base(_npc, _anim, _agent, _player)
    {
        name = STATE.ATTACK;

    }

    public override void Enter()
    {
        Debug.Log("Attack State Entered");
        anim.SetTrigger("IsAttacking");
        agent.isStopped = true;
        base.Enter();
    }

    public override void Update()
    {
        Vector3 direction = player.position - npc.transform.position;
        float angle = Vector3.Angle(direction, npc.transform.forward);
        direction.y = 0; //To prevent tilting of the character around x or y axis.

        npc.transform.rotation = Quaternion.Slerp(npc.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotationSpeed);
        if(!CanAttackPlayer())
        {
            Debug.Log("Entering Idle State");
            nextState = new Idle(npc, anim, agent, player);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}