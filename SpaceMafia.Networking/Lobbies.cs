﻿using Hazel;
using Hazel.Udp;
using SpaceMafia.Client;
using SpaceMafia.Enums;
using SpaceMafia.Exceptions;
using SpaceMafia.Extras;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace SpaceMafia.Networking
{
    public static class Lobbies
    {
        static byte[] V2Map = new byte[] { 0x19, 0x15, 0x13, 0x0A, 0x08, 0x0B, 0x0C, 0x0D, 0x16, 0x0F, 0x10, 0x06, 0x18, 0x17, 0x12, 0x07, 0x00, 0x03, 0x09, 0x04, 0x0E, 0x14, 0x01, 0x02, 0x05, 0x11 };
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

    struct LobbySettings
    {
        string JoinCode;
        byte MaxPlayers;// = 0x0a;
        uint Language;// = 0;
        byte MapType;
        float PlayerSpeedModifier;// = 1.0f;
        float CrewLightModifier;// = 1.0f;
        float ImpostorLightModifier;// = 1.5f;
        float KillCooldown;// = 15;
        byte CommonTasks;// = 1;
        byte LongTasks;// = 1;
        byte ShortTasks;// = 1;
        int Emergencies;// = 2;
        byte ImpostorCount;
        byte KillDistance;// = 1;
        int DiscussionTime;// = 15;
        int VotingTime;// = 120;
        bool IsDefault;// = true;
        byte EmeergencyCooldown;// = 0x0f;
        bool ConfirmEjects;// = true;
        bool VisualTasks;// = true;
        bool AnonymousVoting;// = false;
        byte TaskBarUpdates;// = 0x00;
    }

    public class AmongUsClient
    {
        private static readonly byte[] HANDSHAKE =
            {0x4A, 0xE2, 0x02, 0x03, 0x08, 0x43, 0x4f, 0x4d, 0x4d, 0x55, 0x4e, 0x53, 0x54};

        LobbySettings settings;

        private const ushort MATCHMAKER_PORT = 22023;

        private uint _hostId; // the mm ID of the host of the current lobby
        private uint _clientId; // the mm ID of ourselves
        private bool _hasPlayerData = false; // our first connect we briefly do a spawn to gather user data
        private bool _hasReconnectedAfterPlayerData = false;

        private List<PlayerData> _playerData = new List<PlayerData>();
        private Dictionary<int, int> _playerControlNetIdToPlayerId = new Dictionary<int, int>();

        private IPAddress _address; // the address of the server we're connected to
        private ushort _port; // the port we're connected to
        private string _lobbyName; // the name of the lobby we're connected to
        private int _lobbyCode; // the code of the lobby we're connected to

        private UdpClientConnection _connection;

        /// <summary>
        /// Fired when the client connects to the server. Note that this may be fired
        /// multiple times
        /// </summary>
        public event Action OnConnect;

        /// <summary>
        /// Fired when the client disconnects from the server outside of the first
        /// connection attempt, such as when everyone has left the lobby.
        /// </summary>
        public event Action OnDisconnect;

        /// <summary>
        /// Fired in moments when talking should be allowed, such as when voting starts.
        /// Note: does not fire when a game ends, you need OnGameEnd for that.
        /// </summary>
        public event Action OnTalkingStart;

        /// <summary>
        /// Fired when the game transitions from a state where talking is allowed to one
        /// where talking is not allowed, such as on game start or after a vote.
        /// </summary>
        public event Action OnTalkingEnd;

        /// <summary>
        /// Fired after a single game has ended and the bot is back in the lobby. This is
        /// a separate event from talking start.
        /// </summary>
        public event Action OnGameEnd;

        /// <summary>
        /// Invoked when the player data is updated for one or more players. Also invoked
        /// at the start with the list of all players that were already in the lobby.
        /// </summary>
        public event Action<List<PlayerData>> OnPlayerDataUpdate;

        /// <summary>
        /// Initializes this client by connecting to the specified host and attempting
        /// to join the specified lobby code. Will throw if connection fails, else will
        /// start servicing messages in the background. The caller is responsible for
        /// ensuring that the application stays running as long as the client is active.
        /// </summary>
        public async Task Connect(IPAddress address, string lobbyName, ushort port = MATCHMAKER_PORT)
        {
            _address = address;
            _port = port;
            _lobbyName = lobbyName;
            _lobbyCode = Lobbies.GameNameToIntV2(lobbyName);

            var (connection, response) = await ConnectToMMAndSend(address, port, JoinGame);

            _connection = connection;
            _connection.DataReceived += OnMessageReceived;
            _connection.Disconnected += (sender, args) => { OnDisconnect?.Invoke(); };

            HandleJoinGameResult(response);

            if (!_hasPlayerData)
            {
                // If we don't have user data, send a SceneChange so that we receive a spawn.
                _connection.SendReliableMessage(writer =>
                {
                    writer.StartMessage((byte)MMTags.GameData);
                    writer.Write(_lobbyCode);
                    writer.StartMessage((byte)GameDataTags.SceneChange);
                    writer.WritePacked(_clientId); // note: must be _clientId since localplayer is not set yet
                    writer.Write("OnlineGame");
                    writer.EndMessage();
                    writer.EndMessage();
                });
            }
            else
            {
                // We have user data, invoke listeners.
                _hasReconnectedAfterPlayerData = true;

                OnConnect?.Invoke();
                OnPlayerDataUpdate?.Invoke(_playerData);
            }
        }

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

        private void JoinGame(MessageWriter writer)
        {
            writer.StartMessage((byte)MMTags.JoinGame);
            writer.Write(_lobbyCode);
            writer.WritePacked(0x7); // map ownership flags
            writer.EndMessage();
        }

        public void DisconnectAndExit()
        {
            _connection?.Disconnect("Bye bye!");
        }
        private void HandleJoinGameResult(MessageReader response)
        {
            // If the response isn't joined game, we have a problem.
            if (response.Tag != (byte)MMTags.JoinedGame)
            {
                // If it isn't JoinGame (which has a disconnect reason) just disconnect with unknown.
                if (response.Tag != (byte)MMTags.JoinGame)
                    throw new AUException("Connecting to the Among Us servers failed with an unknown error.");

                var reason = (DisconnectReasons)response.ReadInt32();

                if (reason == DisconnectReasons.GameNotFound)
                {
                    throw new AUException(
                        "Could not join the lobby because the game was not found. Please double-check the lobby code and region.");
                }

                if (reason == DisconnectReasons.GameFull)
                {
                    throw new AUException(
                        "Could not join the lobby because it was full. Remember that the bot also takes up a space!");
                }

                if (reason == DisconnectReasons.GameStarted)
                {
                    throw new AUException(
                        "Could not join the lobby because the game is already in progress. Please try again after this round is finished.");
                }

                if (reason == DisconnectReasons.Custom)
                {
                    var text = response.ReadString();
                    throw new AUException($"Could not join the lobby: '{text}'");
                }

                throw new AUException($"Could not join the lobby due to an unknown error: {reason}");
            }

            // We're fine!
            response.ReadInt32(); // game id
            _clientId = response.ReadUInt32(); // local client id
            _hostId = response.ReadUInt32(); // host id
        }
        private void OnMessageReceived(DataReceivedEventArgs args)
        {
            try
            {
                foreach (var message in args.Message.Messages())
                {
                    HandleMessage(message);
                }
            }
            finally
            {
                args.Message.Recycle();
            }
        }
        private void HandleMessage(MessageReader message)
        {
            if (message.Tag == (byte)MMTags.GameData || message.Tag == (byte)MMTags.GameDataTo)
            {
                message.ReadInt32(); // game id

                // Parse targetId field if GameDataTo.
                if (message.Tag == (byte)MMTags.GameDataTo && message.ReadPackedInt32() != _clientId)
                    return;

                foreach (var gameDataMessage in message.Messages())
                {
                    HandleGameData(gameDataMessage);
                }
            }
            else if (message.Tag == (byte)MMTags.StartGame)
            {
                HandleStartGame(message);
            }
            else if (message.Tag == (byte)MMTags.EndGame)
            {
                HandleEndGame(message);
            }
            else if (message.Tag == (byte)MMTags.RemovePlayer)
            {
                HandleRemovePlayer(message);
            }
            //else if (message.Tag == (byte)MMTags.WaitForHost)
            //{
            //   PingPacket(message);
            //}
        }

        /// <summary>
        /// Invoked for each data packet that contains game data. Checks if this is an RPC or spawn,
        /// and if yes acts accordingly.
        /// </summary>
        private void HandleGameData(MessageReader reader)
        {
            if (reader.Tag == (byte)GameDataTags.Rpc)
            {
                HandleRPC(reader);
            }
            else if (reader.Tag == (byte)GameDataTags.Spawn)
            {
                HandleSpawn(reader);
            }
        }
        /// <summary>
        /// Handles an RPC game packet, dispatching the results when appropriate.
        /// </summary>
        private void HandleRPC(MessageReader reader)
        {
            reader.ReadPackedInt32(); // rpc target
            var action = (RPCCalls)reader.ReadByte();

            if (action == RPCCalls.Close)
            {
                OnTalkingEnd?.Invoke();
            }
            else if (action == RPCCalls.StartMeeting)
            {
                OnTalkingStart?.Invoke();
            }
            else if (action == RPCCalls.UpdateGameData)
            {
                if (!_hasReconnectedAfterPlayerData) return; // don't handle UpdateGameData earlier.

                foreach (var dataEntry in reader.Messages())
                {
                    UpdateOrCreatePlayerData(dataEntry, dataEntry.Tag);
                }

                OnPlayerDataUpdate?.Invoke(_playerData);
            }
            else if (action == RPCCalls.MurderPlayer)
            {
                var victim = reader.ReadPackedInt32();
                var victimPlayerId = _playerControlNetIdToPlayerId[victim];

                _playerData.Find(x => x.id == victimPlayerId).statusBitField |= 4; // dead
                OnPlayerDataUpdate?.Invoke(_playerData);
            }
            else if (action == RPCCalls.VotingComplete)
            {
                reader.ReadBytesAndSize(); // voting data
                var victim = reader.ReadByte();
                if (victim != 0xFF)
                {
                    _playerData.Find(x => x.id == victim).statusBitField |= 4; // dead
                    OnPlayerDataUpdate?.Invoke(_playerData);
                }
            }
        }

        private void HandleSpawn(MessageReader reader)
        {
            var spawnId = (SpawnableObjects)reader.ReadPackedUInt32();
            var owner = reader.ReadPackedUInt32();
            reader.ReadByte(); // flags
            reader.ReadPackedInt32(); // component length

            if (spawnId == SpawnableObjects.GameData)
            {
                reader.ReadPackedInt32(); // game data net id
                var gameData = reader.ReadMessage();
                var numPlayers = gameData.ReadPackedInt32();

                for (var i = 0; i < numPlayers; i++)
                {
                    var playerId = gameData.ReadByte();
                    UpdateOrCreatePlayerData(gameData, playerId);
                }
            }
            else if (spawnId == SpawnableObjects.PlayerControl)
            {
                var netId = reader.ReadPackedInt32(); // player control net id
                var controlData = reader.ReadMessage();
                controlData.ReadByte(); // unk, seems to be 1 if us, else 0
                var playerId = controlData.ReadByte(); // this is us, we got to ignore us

                _playerControlNetIdToPlayerId[netId] = playerId;

                // If this is us (only for the brief initial connect), ignore it.
                if (owner == _clientId) return;

                // Either join an existing entry or create a new one.
                var existing = _playerData.Find(x => x.id == playerId);
                if (existing != null)
                {
                    existing.clientId = owner;
                }
                else
                {
                    _playerData.Add(new PlayerData(owner, playerId));
                }

                // Update if this is not the initial data gathering state.
                if (_hasPlayerData)
                {
                    OnPlayerDataUpdate?.Invoke(_playerData);
                }

                // Check if we have the data on everyone, and if yes disconnect and reconnect.
                if (!_hasPlayerData && _playerData.All(x => x.clientId != 0))
                {
                    _hasPlayerData = true;
                    DisconnectAndReconnect();
                }
            }
        }

        /// <summary>
        /// Helper function that parses a PlayerData instance from the specified
        /// reader and updates the local copy of the player data with the info.
        /// </summary>
        private void UpdateOrCreatePlayerData(MessageReader reader, byte playerId)
        {
            var player = _playerData.Find(x => x.id == playerId);
            if (player == null)
            {
                _playerData.Add(player = new PlayerData(0, playerId));
            }

            player.Deserialize(reader);
        }

        /// <summary>
        /// Invoked when the game has been started.
        /// </summary>
        private void HandleStartGame(MessageReader message)
        {
            OnTalkingEnd?.Invoke();

            _connection.SendReliableMessage(writer =>
            {
                writer.StartMessage((byte)MMTags.GameData);
                writer.Write(_lobbyCode);

                writer.StartMessage((byte)GameDataTags.Ready);
                writer.WritePacked(_clientId);
                writer.EndMessage();

                writer.EndMessage();


            });
        }

        /// <summary>
        /// Invoked when the game has ended. Attempts to rejoin the same lobby.
        /// </summary>
        private void HandleEndGame(MessageReader message)
        {
            _playerData.ForEach(x => x.tasks.Clear()); // nobody has tasks any more
            OnPlayerDataUpdate?.Invoke(_playerData);
            OnGameEnd?.Invoke();

            // Simply rejoin the same lobby.
            _connection.SendReliableMessage(JoinGame);
        }

        /// <summary>
        /// Invoked when a player left the lobby. Handles situations where we
        /// end up becoming the host.
        /// </summary>
        private async void HandleRemovePlayer(MessageReader reader)
        {
            reader.ReadInt32(); // room code
            var idThatLeft = reader.ReadInt32(); // id that left
            var newHost = reader.ReadUInt32();
            reader.ReadByte(); // disconnect reason

            // Update the game data by removing the player that left.
            _hostId = newHost;
            _playerData = _playerData.Where(x => x.clientId != idThatLeft).ToList();
            OnPlayerDataUpdate?.Invoke(_playerData);

            // If we're the host now, leave and attempt to rejoin to make someone else host.
            if (newHost == _clientId)
            {
                await DisconnectAndReconnect();
            }
        }

        /// <summary>
        /// Helper that disconnects and reconnects to the lobby. Used after a game and
        /// when we have gathered all info we needed from spawning ourselves. Will not
        /// invoke the disconnect handler, unless the connecting fails.
        /// </summary>
        private async Task DisconnectAndReconnect()
        {
            _connection.RemoveDisconnectListeners();
            _connection.Disconnect("Bye bye!");

            try
            {
                await Connect(_address, _lobbyName, _port);
            }
            catch
            {
                OnDisconnect?.Invoke();
            }
        }

        /// <summary>
        /// This is sent from client and server as a keep alive, and it must be ACKed.
        /// </summary>
        public void PingPacket(MessageReader reader)
        {
            //var num = reader.ReadUInt16();
            _connection.SendReliableMessage(writer =>
            {
                writer.StartMessage((byte)MMTags.WaitForHost);
                writer.Write(166);

                writer.EndMessage();

            });
        }

        public void SendMovement(ushort xpos, ushort ypos, ushort xvel, ushort yvel)
        {
            MessageWriter writer = MessageWriter.Get(SendOption.None);
            writer.StartMessage((byte)MMTags.GameData);
            writer.Write(_lobbyCode);
            writer.Write((ushort)5);
            writer.Write((byte)GameDataTags.Data);
            writer.WritePacked(_clientId);
            writer.Write(xpos);
            writer.Write(ypos);
            writer.Write(xvel);
            writer.Write(yvel);

            _connection.Send(writer);
        }

        public void SendChat(string chat)
        {
            //var num = reader.ReadUInt16();
            _connection.SendReliableMessage(writer =>
            {
                writer.StartMessage((byte)RPCCalls.SendChat);
                writer.Write(chat);

                writer.EndMessage();

            });
        }
    }
}
