using UnityEngine;
using UnityEngine.AI;

// Helper so every state can safely touch the agent without repeating the guard.
// If the agent is missing or inactive (e.g. Threnos), the call is a silent no-op.
internal static class AgentUtil
{
    public static void Stop(EnemyBase e)
    {
        if (e.agent != null && e.agent.isActiveAndEnabled)
            e.agent.isStopped = true;
    }

    public static void Resume(EnemyBase e)
    {
        if (e.agent != null && e.agent.isActiveAndEnabled)
            e.agent.isStopped = false;
    }

    public static void Disable(EnemyBase e)
    {
        if (e.agent != null && e.agent.enabled)
            e.agent.enabled = false;
    }

    public static void SetDestination(EnemyBase e, Vector3 dest)
    {
        if (e.agent != null && e.agent.isActiveAndEnabled)
            e.agent.SetDestination(dest);
    }

    public static void ResetPath(EnemyBase e)
    {
        if (e.agent != null && e.agent.isActiveAndEnabled)
            e.agent.ResetPath();
    }
}

// ── IDLE ──────────────────────────────────────────────────────────────
public class EnemyIdleState : IEnemyState
{
    private float idleTimer;
    private const float IDLE_DURATION = 0.5f;

    public void Enter(EnemyBase e)
    {
        idleTimer = IDLE_DURATION;
        AgentUtil.Stop(e);
        e.SetAnimMove(true, false, false);
    }

    public void Tick(EnemyBase e)
    {
        idleTimer -= Time.deltaTime;

        if (e.distanceToPlayer <= e.chaseRange || idleTimer <= 0f)
            e.TransitionTo(e.ChaseState);
    }

    public void FixedTick(EnemyBase e) { }

    public void Exit(EnemyBase e)
    {
        AgentUtil.Resume(e);
    }
}

// ── CHASE ─────────────────────────────────────────────────────────────
public class EnemyChaseState : IEnemyState
{
    private float repathTimer;
    private const float REPATH_INTERVAL = 0.15f;

    private Vector3 arrivalOffset;

    public void Enter(EnemyBase e)
    {
        repathTimer = 0f;
        arrivalOffset = PickArrivalOffset(e);
        AgentUtil.Resume(e);
        e.SetAnimMove(false, false, true);
    }

    public void Tick(EnemyBase e)
    {
        repathTimer -= Time.deltaTime;

        if (repathTimer <= 0f)
        {
            repathTimer = REPATH_INTERVAL;
            AgentUtil.SetDestination(e, e.player.position + arrivalOffset);
        }

        if (e.distanceToPlayer <= e.attackRange)
        {
            e.TransitionTo(e.AttackState);
            return;
        }

        if (e.distanceToPlayer > e.chaseRange + 3f)
            e.TransitionTo(e.IdleState);
    }

    public void FixedTick(EnemyBase e) { }

    public void Exit(EnemyBase e)
    {
        AgentUtil.ResetPath(e);
    }

    private Vector3 PickArrivalOffset(EnemyBase e)
    {
        float angle = (e.GetInstanceID() % 360) * Mathf.Deg2Rad;
        float radius = e.attackRange * 0.6f;
        return new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * radius;
    }
}

// ── ATTACK ────────────────────────────────────────────────────────────
public class EnemyAttackState : IEnemyState
{
    public void Enter(EnemyBase e)
    {
        AgentUtil.Stop(e);
        e.SetAnimMove(false, false, false);
    }

    public void Tick(EnemyBase e)
    {
        FacePlayer(e);

        if (e.distanceToPlayer > e.attackRange + 1.2f && !e.isDealtDamageWindowOpen)
        {
            e.CloseDamageWindow();
            e.TransitionTo(e.ChaseState);
            return;
        }

        TryAttack(e);
    }

    public void FixedTick(EnemyBase e) { }

    public void Exit(EnemyBase e)
    {
        e.CloseDamageWindow();
        AgentUtil.Resume(e);
    }

    private void TryAttack(EnemyBase e)
    {
        EnemyAttackSO data = e.GetCurrentAttackData();
        if (data == null) return;

        bool cooldownReady = Time.time - e.lastAttackTime >= data.attackCooldown;
        if (!cooldownReady || e.isDealtDamageWindowOpen) return;

        if (e.animator.runtimeAnimatorController != data.animatorOV)
            e.animator.runtimeAnimatorController = data.animatorOV;

        e.animator.Play("Attack_1", 0, 0f);
        e.lastAttackTime = Time.time;
        e.AdvanceCombo();
    }

    private void FacePlayer(EnemyBase e)
    {
        Vector3 dir = e.player.position - e.transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.001f) return;

        Quaternion target = Quaternion.LookRotation(dir);
        e.transform.rotation = Quaternion.Slerp(
            e.transform.rotation, target, Time.deltaTime * 10f
        );
    }
}

// ── STUN ──────────────────────────────────────────────────────────────
public class EnemyStunState : IEnemyState
{
    public void Enter(EnemyBase e)
    {
        AgentUtil.Stop(e);
        AgentUtil.Disable(e);           // safe no-op if agent is already gone
        e.CloseDamageWindow();
        e.SetAnimMove(false, false, false);

        e.animator.ResetTrigger("stun");
        e.animator.ResetTrigger("Jump");
        e.animator.SetTrigger("death");

        if (e.healthBarRoot != null)
            e.healthBarRoot.SetActive(false);
    }

    public void Tick(EnemyBase e) { }
    public void FixedTick(EnemyBase e) { }

    public void Exit(EnemyBase e)
    {
        AgentUtil.Resume(e);
    }
}

// ── DEATH ─────────────────────────────────────────────────────────────
public class EnemyDeathState : IEnemyState
{
    public void Enter(EnemyBase e)
    {
        AgentUtil.Stop(e);
        AgentUtil.Disable(e);           // safe no-op if agent is already gone
        e.CloseDamageWindow();
        e.SetAnimMove(false, false, false);
        e.animator.SetTrigger("death");

        if (e.healthBarRoot != null)
            e.healthBarRoot.SetActive(false);
    }

    public void Tick(EnemyBase e) { }
    public void FixedTick(EnemyBase e) { }
    public void Exit(EnemyBase e) { }
}