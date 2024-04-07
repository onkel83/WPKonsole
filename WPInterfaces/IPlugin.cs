namespace WPInterfaces
{
    public interface IPlugin
    {
        string Name { get; }
        void Execute(string[] args);
    }
}

