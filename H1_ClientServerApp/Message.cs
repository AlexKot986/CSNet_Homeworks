using System.Text.Json;

namespace H1_ClientServerApp
{
    internal class Message
    {
        public string Name { get; set; }
        public string? Text { get; set; }
        public DateTime Time { get; set; }
        public Message(string name, string text) 
        { 
            Name = name;
            Text = text;
            Time = DateTime.Now;
        }


        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }
        public static Message? FromJson(string json)
        {
            return JsonSerializer.Deserialize<Message>(json);
        }
        public override string ToString()
        {
            return $"Получено сообщение от {Name} ({Time.ToShortTimeString()}):\n {Text}";
        }
    }
}
