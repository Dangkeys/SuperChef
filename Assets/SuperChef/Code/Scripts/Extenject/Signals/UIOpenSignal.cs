public class UIOpenSignal
{
    public UIType Type;
    public bool IsOpen;

    public UIOpenSignal(UIType type, bool isOpen=false)
    {
        Type = type;
        IsOpen = isOpen;
    }
}

// Optional enum to describe which UI opened
public enum UIType
{
    Inventory,
    Map,
    Settings,
    Shop,
    Dialogue,
    // add more as needed
}
