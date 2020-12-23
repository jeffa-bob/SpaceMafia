using Hazel;
using Hazel.Udp;
using SpaceMafia.Enums;
using SpaceMafia.Exceptions;
using SpaceMafia.Extras;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace SpaceMafia.Networking
{
    public static class Lobbies
    {
        public static byte[] V2Map = new byte[] { 0x19, 0x15, 0x13, 0x0A, 0x08, 0x0B, 0x0C, 0x0D, 0x16, 0x0F, 0x10, 0x06, 0x18, 0x17, 0x12, 0x07, 0x00, 0x03, 0x09, 0x04, 0x0E, 0x14, 0x01, 0x02, 0x05, 0x11 };
        public static int GameNameToIntV2(string code)
        {
            var a = V2Map[code[0] - 65];
            var b = V2Map[code[1] - 65];
            var c = V2Map[code[2] - 65];
            var d = V2Map[code[3] - 65];
            var e = V2Map[code[4] - 65];
            var f = V2Map[code[5] - 65];

            var one = (a + 26 * b) & 0x3FF;
            var two = (c + 26 * (d + 26 * (e + 26 * f)));

            return (int)(one | ((two << 10) & 0x3FFFFC00) | 0x80000000);
        }
    }

    public class Lobby
    {
        private static readonly byte[] HANDSHAKE =
            {0x4A, 0xE2, 0x02, 0x03, 0x08, 0x49, 0x6D, 0x70, 0x6F, 0x73, 0x74, 0x6F, 0x72};

        string JoinCode;
        byte MaxPlayers = 0x0a;
        uint Language = 0;
        byte MapType;
        float PlayerSpeedModifier = 1.0f;
        float CrewLightModifier = 1.0f;
        float ImpostorLightModifier = 1.5f;
        float KillCooldown = 15;
        byte CommonTasks = 1;
        byte LongTasks = 1;
        byte ShortTasks = 1;
        int Emergencies = 2;
        byte ImpostorCount;
        byte KillDistance = 1;
        int DiscussionTime = 15;
        int VotingTime = 120;
        bool IsDefault = true;
        byte EmeergencyCooldown = 0x0f;
        bool ConfirmEjects = true;
        bool VisualTasks = true;
        bool AnonymousVoting = false;
        byte TaskBarUpdates = 0x00;

        private static async Task<(UdpClientConnection, MessageReader)> ConnectToMMAndSend(IPAddress address,
           ushort port, Action<MessageWriter> writeMessage)
        {
            var firstMessageTask = new TaskCompletionSource<MessageReader>();

            var connection = new UdpClientConnection(new IPEndPoint(address, port));
            connection.KeepAliveInterval = 1000;
            connection.DisconnectTimeout = 10000;
            connection.ResendPingMultiplier = 1.2f;

            // Set up an event handler to resolve the task on first non-reselect message received.
            Action<DataReceivedEventArgs> onDataReceived = null;
            onDataReceived = args =>
            {
                try
                {
                    var msg = args.Message.ReadMessage();
                    if (msg.Tag == (byte)MMTags.ReselectServer) return; // not interested

                    firstMessageTask.TrySetResult(msg);
                    connection.DataReceived -= onDataReceived;
                }
                finally
                {
                    args.Message.Recycle();
                }
            };
            connection.DataReceived += onDataReceived;

            // Set up an event handler to set an exception for the task on early disconnect.
            connection.Disconnected += (sender, args) =>
            {
                connection.Dispose();
                firstMessageTask.TrySetException(new AUException("Connection to matchmaker prematurely exited"));
            };

            // Connect to the endpoint.
            connection.ConnectAsync(HANDSHAKE);
            await connection.ConnectWaitLock.AsTask();

            // Send the contents.
            connection.SendReliableMessage(writeMessage);

            // Wait for the response to arrive.
            var response = await firstMessageTask.Task;

            // If this is not a redirect, return the result.
            if (response.Tag != (byte)MMTags.Redirect)
            {
                return (connection, response);
            }

            // This is a redirect, so do this again but with the new data.
            var newIp = response.ReadUInt32();
            var newPort = response.ReadUInt16();

            // Reconnect to new host.
            return await ConnectToMMAndSend(new IPAddress(newIp), newPort, writeMessage);
        }
    }
}
