public class IncomePosition 
{
    public string Name { get; private set; }
    public float Value { get; private set; }

    public IncomePosition(string name, float value)
    {
        this.Name = name;
        this.Value = value;
    }

    public override string ToString()
    {
        return Name + " " + Value;
    }
}