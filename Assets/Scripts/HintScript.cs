using UnityEngine;

public class HintScript : BasicTickable
{
    [SerializeField]
    private string text;
    private bool _isTriggered;
    
    public override void OnEndTick()
    {
        var colliding = Physics2D.OverlapPoint(transform.position, LayerMask.GetMask("Collision"));
        if (colliding && colliding.CompareTag("Player") && !_isTriggered)
        {
            GameLogicUI.Instance.ShowHint(text);
            _isTriggered = true;
        }
    }
}