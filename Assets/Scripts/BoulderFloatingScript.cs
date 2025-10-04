public class BoulderFloatingScript : BasicTickable
{
    // For z index
    
    public override void PostEndTick()
    {
        base.PostEndTick();
        SpriteRenderer.sortingOrder--;
    }
}