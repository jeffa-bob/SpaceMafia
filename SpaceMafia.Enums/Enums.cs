using System;

namespace SpaceMafia.Enums
{

    // from https://wiki.weewoo.net/wiki/Enums

    public enum Color
    {
        Red,
        Blue,
        Green,
        Pink,
        Orange,
        Yellow,
        Black,
        White,
        Purple,
        Brown,
        Cyan,
        Lime,
        Fortegreen
    }

    public enum DeathReason
    {
        Exile,
        Kill,
        Disconnect
    }

    public enum FreeWeekendState
    {
        NotFree,
        FreeMIRA,
        FreePolus
    }

    public enum GameStates
    {
        NotStarted,
        Started,
        Ended,
        Destroyed
    }

    public enum Hat
    {
        NoHat,
        Astronaut,
        BaseballCap,
        BrainSlug,
        BushHat,
        CaptainsHat,
        DoubleTopHat,
        Flowerpot,
        Goggles,
        HardHat,
        Military,
        PaperHat,
        PartyHat,
        Police,
        Stethescope,
        TopHat,
        TowelWizard,
        Ushanka,
        Viking,
        WallCap,
        Snowman,
        Reindeer,
        Lights,
        Santa,
        Tree,
        Present,
        Candycanes,
        ElfHat,
        NewYears2018,
        WhiteHat,
        Crown,
        Eyebrows,
        HaloHat,
        HeroCap,
        PipCap,
        PlungerHat,
        ScubaHat,
        StickminHat,
        StrawHat,
        TenGallonHat,
        ThirdEyeHat,
        ToiletPaperHat,
        Toppat,
        Fedora,
        Goggles_2,
        Headphones,
        MaskHat,
        PaperMask,
        Security,
        StrapHat,
        Banana,
        Beanie,
        Bear,
        Cheese,
        Cherry,
        Egg,
        Fedora_2,
        Flamingo,
        FlowerPin,
        Helmet,
        Plant,
        BatEyes,
        BatWings,
        Horns,
        Mohawk,
        Pumpkin,
        ScaryBag,
        Witch,
        Wolf,
        Pirate,
        Plague,
        Machete,
        Fred,
        MinerCap,
        WinterHat,
        Archae,
        Antenna,
        Balloon,
        BirdNest,
        BlackBelt,
        Caution,
        Chef,
        CopHat,
        DoRag,
        DumSticker,
        Fez,
        GeneralHat,
        GreyThing,
        HunterCap,
        JungleHat,
        MiniCrewmate,
        NinjaMask,
        RamHorns,
        Snowman_2
    }

    public enum KillAnimType
    {
        Stab,
        Tongue,
        Shoot,
        Neck,
        None
    }

    public enum Language
    {
        English,
        Spanish,
        Portuguese,
        Korean,
        Russian
    }

    public enum LimboStates
    {
        PreSpawn,
        NotLimbo,
        WaitingForHost
    }
    public enum Map
    {
        Skeld,
        MiraHQ,
        Polus
    }
    public enum Pet
    {
        EmptyPet,
        Alien,
        Crewmate,
        Doggy,
        Stickmin,
        Hamster,
        Robot,
        UFO,
        Ellie,
        Squig,
        Bedcrab,
        Glitch
    }

    public enum Skin
    {
        None,
        Astro,
        Capt,
        Mech,
        Military,
        Police,
        Science,
        SuitB,
        SuitW,
        Wall,
        Hazmat,
        Security,
        Tarmac,
        Miner,
        Winter,
        Archae
    }

    public enum SpawnFlags
    {
        None,
        IsClientCharacter
    }

    public enum SpawnableObjectIds
    {
        ShipStatus,
        MeetingHud,
        LobbyBehavior,
        GameData,
        VoteBanSystem,
        Player,
        CustomNetworkTransform,
        HeadQuarters,
        PlanetMap,
        AprilShipStatus
    }

    public enum SystemTypes
    {
        Hallway,
        Storage,
        Cafeteria,
        Reactor,
        UpperEngine,
        Nav,
        Admin,
        Electrical,
        LifeSupp,
        Shields,
        MedBay,
        Security,
        Weapons,
        LowerEngine,
        Comms,
        ShipTasks,
        Doors,
        Sabotage,
        Decontamination,
        Launchpad,
        LockerRoom,
        Laboratory,
        Balcony,
        Office,
        Greenhouse,
        Dropship,
        Decontamination2,
        Outside,
        Specimens,
        Boiler_Room
    }
    public enum TaskTypes
    {
        SubmitScan,
        PrimeShields,
        FuelEngines,
        ChartCourse,
        StartReactor,
        SwipeCard,
        ClearAsteroids,
        UploadData,
        InspectSample,
        EmptyChute,
        EmptyGarbage,
        AlignEngineOutput,
        FixWiring,
        CalibrateDistributor,
        DivertPower,
        UnlockManifolds,
        ResetReactor,
        FixLights,
        Filter,
        FixComms,
        RestoreOxy,
        StabilizeSteering,
        AssembleArtifact,
        SortSamples,
        MeasureWeather,
        EnterIdCode,
        BuyBeverage,
        ProcessData,
        RunDiagnostics,
        WaterPlants,
        MonitorOxygen,
        StoreArtifact,
        FillCanisters,
        ActivateWeatherNodes,
        InsertKeys,
        ResetSeismic,
        ScanBoardingPass,
        OpenWaterways,
        ReplaceWaterJug,
        RepairDrill,
        AlignTelescope,
        RecordTemperature,
        RebootWifi,
    }

    public enum MMTags : byte
    {
        HostGame = 0,
        JoinGame = 1,
        StartGame = 2,
        RemoveGame = 3,
        RemovePlayer = 4,
        GameData = 5,
        GameDataTo = 6,
        JoinedGame = 7,
        EndGame = 8,
        AlterGame = 10,
        KickPlayer = 11,
        WaitForHost = 12,
        Redirect = 13,
        ReselectServer = 14,
        GetGameList = 9,
        GetGameListV2 = 16,
    }

    /**
     * Represents the tags of messages sent by/to us through MM GameData packets.
     */
    public enum GameDataTags : byte
    {
        Data = 1,
        Rpc = 2,
        Spawn = 4,
        Despawn = 5,
        SceneChange = 6,
        Ready = 7,
        ChangeSettings = 8,
    }

    /**
     * Represents the set of possible RCP actions that can be invoked by a client.
     */
    public enum RPCCalls : byte
    {
        PlayAnimation = 0,
        CompleteTask = 1,
        SyncSettings = 2,
        SetInfected = 3,
        Exiled = 4,
        CheckName = 5,
        SetName = 6,
        CheckColor = 7,
        SetColor = 8,
        SetHat = 9,
        SetSkin = 10,
        ReportDeadBody = 11,
        MurderPlayer = 12,
        SendChat = 13,
        StartMeeting = 14,
        SetScanner = 15,
        SendChatNote = 16,
        SetPet = 17,
        SetStartCounter = 18,
        EnterVent = 19,
        ExitVent = 20,
        SnapTo = 21,
        Close = 22,
        VotingComplete = 23,
        CastVote = 24,
        ClearVote = 25,
        AddVote = 26,
        CloseDoorsOfType = 27,
        RepairSystem = 28,
        SetTasks = 29,
        UpdateGameData = 30
    }

    /**
     * Represents a reason for being disconnected, as reported by the server.
     */
    public enum DisconnectReasons
    {
        ExitGame = 0,
        GameFull = 1,
        GameStarted = 2,
        GameNotFound = 3,
        IncorrectVersion = 5,
        Banned = 6,
        Kicked = 7,
        Custom = 8,
        InvalidName = 9,
        Hacking = 10,
        Destroy = 16,
        Error = 17,
        IncorrectGame = 18,
        ServerRequest = 19,
        ServerFull = 20,
        FocusLostBackground = 207,
        IntentionalLeaving = 208,
        FocusLost = 209,
        NewConnection = 210
    }

    public enum SpawnableObjects : byte
    {
        ShipStatus0 = 0,
        MeetingHud = 1,
        LobbyBehavior = 2,
        GameData = 3,
        PlayerControl = 4,
        ShipStatus1 = 5,
        ShipStatus2 = 6,
        ShipStatus3 = 7
    }
}
