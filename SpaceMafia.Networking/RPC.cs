using System;
using System.Net;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Formats.Asn1;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

namespace SpaceMafia.Networking
{

    class RPC
    {
        private Socket sock;
        public RPC()
        {

        }
    }

    public class GameListRequest
    {
        const byte MaxPlayers = 0x0a;
        uint Language = 0;
        byte MapType;
        const float PlayerSpeedModifier = 1.0f;
        const float CrewLightModifier = 1.0f;
        const float ImpostorLightModifier = 1.5f;
        const float KillCooldown = 15;
        const byte CommonTasks = 1;
        const byte LongTasks = 1;
        const byte ShortTasks = 1;
        const int Emergencies = 2;
        byte ImpostorCount;
        const byte KillDistance = 1;
        const int DiscussionTime = 15;
        const int VotingTime = 120;
        const bool IsDefault = true;
        const byte EmeergencyCooldown = 0x0f;
        const bool ConfirmEjects = true;
        const bool VisualTasks = true;
        const bool AnonymousVoting = false;
        const byte TaskBarUpdates = 0x00;
        byte[] request;
        byte[] recieve = new byte[256];

        public GameListRequest(byte Map, byte ImpostorCount, uint Language)
        {
            this.MapType = Map;
            this.Language = Language;
            this.ImpostorCount = ImpostorCount;
        }

        public byte[] GenerateOptionRequest()
        {
            const byte length = 0x2e;
            const byte version = 0x04;
            byte[] packet = new byte[] { length, version, MaxPlayers };
            packet = packet.Concat(BitConverter.GetBytes(Language)).ToArray();
            packet = packet.Append(MapType).ToArray();
            packet = packet.Concat(BitConverter.GetBytes(PlayerSpeedModifier)).ToArray();
            packet = packet.Concat(BitConverter.GetBytes(CrewLightModifier)).ToArray();
            packet = packet.Concat(BitConverter.GetBytes(ImpostorLightModifier)).ToArray();
            packet = packet.Concat(BitConverter.GetBytes(KillCooldown)).ToArray();
            packet = packet.Append(CommonTasks).ToArray();
            packet = packet.Append(LongTasks).ToArray();
            packet = packet.Append(ShortTasks).ToArray();
            packet = packet.Concat(BitConverter.GetBytes(Emergencies)).ToArray();
            packet = packet.Append(ImpostorCount).ToArray();
            packet = packet.Append(KillDistance).ToArray();
            packet = packet.Concat(BitConverter.GetBytes(DiscussionTime)).ToArray();
            packet = packet.Concat(BitConverter.GetBytes(VotingTime)).ToArray();
            packet = packet.Concat(BitConverter.GetBytes(IsDefault)).ToArray();
            packet = packet.Append(EmeergencyCooldown).ToArray();
            packet = packet.Concat(BitConverter.GetBytes(ConfirmEjects)).ToArray();
            packet = packet.Concat(BitConverter.GetBytes(VisualTasks)).ToArray();
            packet = packet.Concat(BitConverter.GetBytes(AnonymousVoting)).ToArray();
            packet = packet.Append(TaskBarUpdates).ToArray();
            request = packet;
            return packet;
        }

        public byte[] SendOptionRequest(IPAddress address)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint endPoint = new IPEndPoint(address, 22023);
            EndPoint senderRemote = (EndPoint)endPoint;
            socket.SendTo(this.request,endPoint);
            Console.WriteLine("waiting");
            var a = socket.ReceiveFrom(this.recieve,SocketFlags.None,ref senderRemote);
            socket.Close();
            Console.WriteLine(a);
            return recieve;
        }
    }
}
