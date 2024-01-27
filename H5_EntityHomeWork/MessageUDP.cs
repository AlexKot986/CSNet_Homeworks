
using H5_EntityHomeWork.Model;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Text.Json;

namespace H5_EntityHomeWork
{
    public class MessageUDP
    {
        public Command Command { get; set; }
        public int? Id { get; set; }
        public string FromName { get; set; }
        public string ToName { get; set; }
        public string Text { get; set; }

        // Метод для сериализации в JSON
        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }

        // Статический метод для десериализации JSON в объект MyMessage
        public static MessageUDP FromJson(string json)
        {
            return JsonSerializer.Deserialize<MessageUDP>(json);
        }
        public override string ToString()
        {
            return $"Сообщение от '{FromName}': {Text}";
        }
    }
}
