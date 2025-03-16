namespace PSHome_Surface_Support_Frontend.Infastructure.Data.Models
{
    public class HomeTSSFormat
    {


    }

    public class Configuration
    {
        public DateTime Version { get; set; } = new DateTime();
        public string SecureContentRoot { get; set; }
        public string ScreenContentRoot { get; set; }
        public string SecureLuaObjectResourcesRoot { get; set; }
        public string SecureLuaSceneResourcesRoot { get; set; }
        public DataCapture DataCaptureService { get; set; }
        public List<Sha1File> Sha1Files { get; set; } = new List<Sha1File>();
        public List<SceneRedirect> SceneRedirects { get; set; } = new List<SceneRedirect>();
        public HTTPCompressionSubsystems hTTPCompressionSubsystems { get; set; }
        public Connection Connection { get; set; }
        public SSFWConnection SSFWConnection { get; set; }
        public GlobalConfig Global { get; set; }
        //public Dictionary<GlobalTypes, GlobalRegionList> globalRegionValueList = new Dictionary<GlobalTypes, GlobalRegionList>();
        public RegionInfo RegionInfo { get; set; }
    }

    public class DataCapture
    {
        public string Url { get; set; }
        public int modeToEdit { get; set; }
    }
    public class HTTPCompressionSubsystems
    {
        public bool object_catalogue { get; set; }
        public bool service_ids_list { get; set; }
        public bool permitted_hosts_hdk { get; set; }
        public bool config_bundle { get; set; }
        public bool scene_list { get; set; }
        public bool navigator_files { get; set; }
        public bool navigator_avatar_image { get; set; }
        public bool ssfw_get_inventory { get; set; }
        public bool ssfw_get_selectables_trunks { get; set; }
        public bool ssfw_get_save_data { get; set; }
        public bool ssfw_get_scripted_save_data { get; set; }
        public bool ssfw_get_apartment_layout { get; set; }
        public bool ssfw_get_club_details { get; set; }
        public bool ssfw_get_layout { get; set; }
        public bool scene_xml { get; set; }
        public bool scene_description { get; set; }
        public bool scene_archive { get; set; }
        public bool object_description { get; set; }
        public bool object_archive { get; set; }
        public bool commerce_point_xml { get; set; }
        public bool screen_file { get; set; }
        public bool screen_content_audio { get; set; }
        public bool screen_content_image { get; set; }
        public bool screen_content_video { get; set; }
        public bool menuscreen_help { get; set; }
        public bool http_image { get; set; }
    }

    public class Connection
    {
        public string muis { get; set; }
        public string key { get; set; }
        public string contentServer { get; set; }
    }

    public class SSFWConnection
    {
        public int ttl { get; set; }
        public string secret { get; set; }
        public string identityService { get; set; }
        public string rewardsService { get; set; }
        public string clanService { get; set; }
        public string saveDataService { get; set; }
        public string avatarService { get; set; }
        public string layoutService { get; set; }
        public string trunksService { get; set; }
        public string avatarLayoutService { get; set; }
        public string structuredSaveDataService { get; set; }
    }

    public class GlobalConfig
    {
        public List<Mode> Modes { get; set; } = new List<Mode>();
    }

    public class Mode
    {
        public string SCEA { get; set; }
        public string SCEJ { get; set; }
        public string SCEE { get; set; }
        public string SCEAsia { get; set; }
        public GlobalType Value { get; set; }
    }


    public class Sha1File
    {
        public string File { get; set; }
        public string Digest { get; set; }
    }

    public class SceneRedirect
    {
        public string Dest { get; set; }
        public string Region { get; set; }
        public string Src { get; set; }
    }

    public class RegionInfo
    {
        public List<RegionType> RegionTypes { get; set; } = new List<RegionType>();
        public List<RegionMap> RegionMaps { get; set; } = new List<RegionMap>();
        public List<RegionLocalisations> RegionLocalisations { get; set; } = new List<RegionLocalisations>();
    }

    public class RegionType
    {
        public string Name { get; set; }
        public string Territory { get; set; }
        public string Instance { get; set; }
        public string Value { get; set; }
    }

    public class RegionMap
    {
        public string Code { get; set; }
        public string Loc { get; set; }
        public string Value { get; set; }
    }

    public class RegionLocalisations
    {
        public string language { get; set; }
        public string Value { get; set; }
    }

    public enum GlobalType
    {
        VoiceCommsMode = 0,
        PictureFrameFileTransfers = 1,
        TUSWriteDisable = 2,
        TUSReadDisable = 3,
        TrophyAPIDisable = 4,
        ClansAPIDisable = 5,
        ScoreRankingAPIDisable = 6,
        LoadBeforeSpawnEnable = 7,
        ClientCameraItemEnable = 8,
        PSPLusExtrasAvailable = 9,
        GhostOnMergeGlitch = 10,
        PublicInstanceMethod = 11,
        PlayersMetListPrivateTextSend = 12,
        PlayersMetListPrivateTextReceive = 13,
        PlayersMetListPrivateVoiceAccept = 14,
        PlayersMetListGroup = 15,
        PlayersMetListGameLaunch = 16,
        PlayersMetListAvatarInteraction = 17,
        PlayersMetListPortableInteraction = 18,
        PlayersMetListChatLog = 19,
        PlayersMetListDeveloper = 20,
        PresenceInfo = 21,
        UseOldApartmentMechanism = 22,
        DisableAllClubFeatures = 23,
        DisableCrossRegionGroupFeatures = 24,
        ProtocolOverride = 25
    }

}
