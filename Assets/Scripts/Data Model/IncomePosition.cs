using Microsoft.Unity.VisualStudio.Editor;

public class IncomePosition 
{
    public Image Image { get; private set; }
    public string Name { get; private set; }
    public float Value { get; private set; }

    public IncomePosition(string name, float value)
    {
        this.Name = name;
        this.Value = value;
//        this.Image = Image; 
    }

    public override string ToString()
    {
        return Name + " " + Value;
    }
}