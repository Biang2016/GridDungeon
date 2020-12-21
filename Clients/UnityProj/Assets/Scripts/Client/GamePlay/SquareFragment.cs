using BiangLibrary.GameDataFormat.Grid;
using BiangLibrary.ObjectPool;
using Sirenix.OdinInspector;
using UnityEngine;

public class SquareFragment : PoolObject
{
    public override void OnRecycled()
    {
        base.OnRecycled();
        Anim.ResetTrigger("TurnToFront");
        Anim.ResetTrigger("TurnToBack");
        Anim.ResetTrigger("ResetToFront");
        Anim.ResetTrigger("ResetToBack");
        FragmentCollider.OnRecycled();
    }

    public FragmentCollider FragmentCollider;

    public Animator Anim;

    public GridPos GridPos;

    [ShowInInspector]
    private bool front;

    [ShowInInspector]
    public bool Front
    {
        get { return front; }
        set
        {
            if (front != value)
            {
                front = value;
                LevelManager.Instance.FragmentFrontMatrix[GridPos.z, GridPos.x] = value;
                Anim.SetTrigger(front ? "TurnToFront" : "TurnToBack");
                if (front) LevelManager.Instance.CurrentFrontCount++;
                else LevelManager.Instance.CurrentFrontCount--;
            }
        }
    }

    public virtual void FlipFragment()
    {
        Front = !Front;
    }

    public void Initialize(GridPos gp, bool front)
    {
        GridPos = gp;
        this.front = front;
        Anim.SetTrigger(front ? "ResetToFront" : "ResetToBack");
    }
}