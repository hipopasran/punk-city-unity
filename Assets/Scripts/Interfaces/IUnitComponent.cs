public interface IUnitComponent
{
    public Unit Unit { get; }
    public void InitializeOn(Unit unit);
}
