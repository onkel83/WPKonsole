using WPInterfaces;

namespace UserPlugin
{
    public class User : IData
    {
        private string? iD;
        private string? name;
        private string? text;

        public string ID { get => iD??"0"; set => iD = value; }
        public string Name { get => name??"TestUser"; set => name = value; }
        public string Text { get => text??"Hier könnte ihre Werbung stehen !"; set => text = value; }

    }
}
