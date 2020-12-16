using UnityEngine;

public class FragmentCollider : MonoBehaviour
{
    public BoxCollider BoxCollider;

    public SquareFragment SquireFragment;

    public void OnRecycled()
    {
    }

    void FixedUpdate()
    {
        if (GameStateManager.Instance.GetState() != GameState.Playing) return;
    }
}