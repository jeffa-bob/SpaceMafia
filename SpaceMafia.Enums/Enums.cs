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

    public enum RpcCalls
    {
        PlayAnimation,
        CompleteTask,
        SyncSettings,
        SetInfected,
        Exiled,
        CheckName,
        SetName,
        CheckColor,
        SetColor,
        SetHat,
        SetSkin,
        ReportDeadBody,
        MurderPlayer,
        SendChat,
        StartMeeting,
        SetScanner,
        SendChatNote,
        SetPet,
        SetStartCounter,
        EnterVent,
        ExitVent,
        SnapTo,
        Close,
        VotingComplete,
        CastVote,
        ClearVote,
        AddVote,
        CloseDoorsOfType,
        RepairSystem,
        SetTasks,
        UpdateGameData
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
}
