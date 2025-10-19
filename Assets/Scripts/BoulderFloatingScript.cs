public class BoulderFloatingScript : BasicTickable
{
    // For z index
    
    public override void OnPostEndTick()
    {
        base.OnPostEndTick();
        SpriteRenderer.sortingOrder--;
    }
}