using System.Text.Json;

namespace H1_ClientServerApp
{
    internal class Message : ICloneable
    {
        public string FromName {  get; set; }
        public string ToName { get; set; }
        public string Text { get; set; }
        public DateTime Time { get; set; }
        public Message(string fromName, string toName, string text) 
        { 
            FromName = fromName;
            ToName = toName;
            Text = text;
            Time = DateTime.Now;
        }
        private Message(string fromName, string toName)
        {
            FromName = fromName;
            ToName = toName;
            Text = string.Empty;
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
            return $"Сообщение от {FromName} ({Time.ToShortTimeString()}):\n {Text}";
        }

        public object Clone()
        {
            Message answerMsg = new("Server", FromName);
            return answerMsg;
        }
    }
}
