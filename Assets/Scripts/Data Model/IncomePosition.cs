public class IncomePosition
{
    public string Name { get; private set; }
    public float Value { get; private set; }
    public IncomePosition(string name, float value) { Name = name; Value = value; }
    public override string ToString() => Name + " " + Value;
}