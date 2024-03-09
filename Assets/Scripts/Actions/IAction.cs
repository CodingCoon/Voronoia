using System.Collections;

public interface IAction 
{
    string Name { get; }

    IEnumerator Execute();

    float GetPrice();

    string GetDetailedInfos();

}
