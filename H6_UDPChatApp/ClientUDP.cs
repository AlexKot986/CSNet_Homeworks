
using H6_UDPChatApp.Abstraction;
using H6_UDPChatApp.Model;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace H6_UDPChatApp
{
    public class ClientUDP
    {
        private readonly string name;
        private readonly IMessageSource messageSourse;
        private readonly IPEndPoint remoteEndPoint;
        public ClientUDP(string name, IMessageSource messageSourse, IPEndPoint endPoint)
        {
            this.name = name;
            this.messageSourse = messageSourse;
            remoteEndPoint = endPoint;
        }
        private void Registred()
        {
            MessageUDP message = new MessageUDP { FromName = name, ToName = string.Empty, Text = string.Empty, Command = Command.Register};
            messageSourse.SendMessage(message, remoteEndPoint);
        }
        public void ClientSendler()
        {
            Registred();

            while (true)
            {
                Console.Write("Имя получателя: ");
                string msgToName = Console.ReadLine();
                Console.Write("Текст сообщения: ");
                string msgText = Console.ReadLine() ?? string.Empty;

                if (!string.IsNullOrEmpty(msgToName))
                {
                    MessageUDP message = new MessageUDP { FromName = name, ToName = msgToName, Text = msgText, Command = Command.Message};
                    messageSourse.SendMessage(message, remoteEndPoint);
                }              
            }          
        }
        public void ClientListener()
        {
            Registred();
            IPEndPoint remoteEP = remoteEndPoint;

            try
            {
                while (true)
                {
                    MessageUDP message = messageSourse.ReceiveMessage(ref remoteEP);
                    Console.WriteLine(message);

                    message.Command = Command.Confirmation;
                    messageSourse.SendMessage(message, remoteEP);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
