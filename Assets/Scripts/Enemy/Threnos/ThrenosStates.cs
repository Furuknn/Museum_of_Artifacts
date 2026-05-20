public class ThrenosChaseState : IEnemyState
{
    public void Enter(EnemyBase e)
    {
        var t = e as ThrenosController;
        if (t == null) return;
        t.canMove = true;
        e.SetAnimMove(false, false, true);
    }

    public void Tick(EnemyBase e)
    {
        var t = e as ThrenosController;
        if (t == null) return;

        // Only handle movement — attack decisions belong to EvaluateActionTick in Update()
        if (!t.isActing)
            t.MoveTowardsPlayer();
    }

    public void FixedTick(EnemyBase e) { }
    public void Exit(EnemyBase e) { }
}

public class ThrenosIdleState : IEnemyState
{
    public void Enter(EnemyBase e)
    {
        var t = e as ThrenosController;
        t?.TransitionTo(t.TChaseState);
    }

    public void Tick(EnemyBase e) { }
    public void FixedTick(EnemyBase e) { }
    public void Exit(EnemyBase e) { }
}

public class ThrenosCombatState : IEnemyState
{
    public void Enter(EnemyBase e) { }

    public void Tick(EnemyBase e)
    {
        var t = e as ThrenosController;
        if (t == null || t.isActing) return;

        float currentMeleeRange = t.isPhase2 ? t.katanaMeleeRange : t.meleeRange;

        if (t.distanceToPlayer > currentMeleeRange)
        {
            t.MoveTowardsPlayer();
            e.SetAnimMove(false, false, true);
        }
        else
        {
            e.SetAnimMove(true, false, false);
        }
        // No attack calls here — EvaluateActionTick owns that
    }

    public void FixedTick(EnemyBase e) { }
    public void Exit(EnemyBase e) { }
}