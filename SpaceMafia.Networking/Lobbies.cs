﻿using System;
using System.Collections.Generic;
using System.Net;

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
        byte Maxplayers = 0x0a;
        uint Language = 256;
        byte MapType;
        float PlayerSpeedModifier = 10.0f;
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
        bool ComfirmEjects = true;
        bool VisualTasks = true;
        bool AnonymousVoting = false;
        byte TaskBarUpdates = 0x00;
    }
}
