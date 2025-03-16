using PSHome_Surface_Support_Frontend.Infastructure.Data.Models;
using PSHome_Surface_Support_Frontend.Pages.SCE.TSS;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace PSHome_Surface_Support_Frontend.Data.Services
{
    public class XmlConfigService
    {
        private readonly string _basePath;

        public XmlConfigService(IWebHostEnvironment env)
        {
            string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "tss");
            _basePath = directoryPath;
        }

        public Configuration LoadConfiguration(XmlConfigService _xmlService)
        {
            ConfigEditor configEditor = new ConfigEditor(_xmlService);

            XDocument doc = new XDocument();


            var files = Directory.GetFiles(_basePath, "*.xml", SearchOption.AllDirectories);

            foreach (var region in configEditor.AvailableRegions)
            {
                string filePath = Path.Combine(_basePath, $"template0001_{region}.xml");

                foreach (var file in files) {

                    if (file.Split("/").Last().Contains(region))
                    {
                        doc = XDocument.Load(filePath);
                        return ParseConfiguration(doc);
                    }
                    else
                    {
                        Console.WriteLine($"Region not found: {region}");
                    }
                }
            }

            return null;
        }

        public Task<List<string>> SaveConfiguration(Configuration config, 
            List<string> regions, 
            bool isRetail, 
            bool includeEnvironmentClosed, 
            bool disableBarSupport,
            bool enableHttpGZipCompression,
            bool enableHttpDeflateCompression,
            bool enableSecureCommercePoints,
            bool useRegionalServiceIds,
            string maxServiceIds,
            string AdminObjectId,
            HTTPCompressionSubsystems httpCompressionSubsystems,
            bool override182Sharc)
        {

            Task<List<string>> result = null;
            foreach (var region in regions)
            {
                string filePath = Path.Combine(_basePath, isRetail ? $"coreHztFmpQrx0002_{region}.xml" : $"clientconfig0001_{region}.xml");
                result = SaveToXml(config, filePath, region, includeEnvironmentClosed, 
                    disableBarSupport, enableHttpGZipCompression, enableHttpDeflateCompression,
                    enableSecureCommercePoints, useRegionalServiceIds, maxServiceIds,
                    AdminObjectId, httpCompressionSubsystems, override182Sharc);
            }
            return result;
        }

        private Configuration ParseConfiguration(XDocument doc)
        {
            var config = new Configuration
            {
                Version = DateTime.Parse(doc.Root.Element("VERSION")?.Value),
                SecureContentRoot = doc.Root.Element("SecureContentRoot")?.Value,
                ScreenContentRoot = doc.Root.Element("ScreenContentRoot")?.Value,
                SecureLuaObjectResourcesRoot = doc.Root.Element("secure_lua_object_resources_root")?.Value,
                SecureLuaSceneResourcesRoot = doc.Root.Element("secure_lua_scene_resources_root")?.Value
            };

            //DataCapture Service
            // Parse Region Info
            var datacapture = doc.Root.Element("datacapture");
            if(datacapture != null)
            {
                config.DataCaptureService = new DataCapture
                {
                    Url = datacapture.Element("url")?.Value,
                    modeToEdit = int.Parse(datacapture.Element("url")?.Attribute("mode")?.Value),
                };
            }
               
            // Parse SHA1 files
            config.Sha1Files = doc.Root.Elements("SHA1")
                .Select(x => new Sha1File
                {
                    File = x.Attribute("file")?.Value,
                    Digest = x.Attribute("digest")?.Value
                }).ToList();

            // Parse Scene Redirects
            config.SceneRedirects = doc.Root.Elements("SceneRedirect")
                .Select(x => new SceneRedirect
                {
                    Dest = x.Attribute("dest")?.Value,
                    Region = x.Attribute("region")?.Value,
                    Src = x.Attribute("src")?.Value
                }).ToList();

            var connection = doc.Root.Element("connection");
            if (connection != null)
            {
                config.Connection = new Connection
                {
                    muis = connection.Element("muis")?.Value,
                    contentServer = connection.Element("contentserver")?.Value,
                    key = connection.Element("contentserver").Attribute("key")?.Value,
                };
            }

            var ssfw = doc.Root.Element("ssfw");
            if (ssfw != null)
            {
                config.SSFWConnection = new SSFWConnection
                {
                    ttl = int.Parse(ssfw.Element("identity").Attribute("ttl")?.Value),
                    secret = ssfw.Element("identity").Attribute("secret")?.Value,
                    identityService = ssfw.Element("identity")?.Value,
                    rewardsService = ssfw.Element("rewards")?.Value,
                    clanService = ssfw.Element("clan")?.Value,
                    saveDataService = ssfw.Element("savedata")?.Value,
                    avatarService = ssfw.Element("avatar")?.Value,
                    layoutService = ssfw.Element("layout")?.Value,
                    trunksService = ssfw.Element("trunks")?.Value,
                    avatarLayoutService = ssfw.Element("avatarlayout")?.Value,
                    structuredSaveDataService = ssfw.Element("structured")?.Value,
                };
            }

            // Parse Global Modes
            var globalElement = doc.Root.Element("global");
            if (globalElement != null)
            {
                config.Global = new GlobalConfig
                {
                    Modes = globalElement.Elements("mode")
                        .Select(m => new Mode
                        {
                            SCEA = m.Attribute("SCEA")?.Value,
                            SCEJ = m.Attribute("SCEJ")?.Value,
                            SCEE = m.Attribute("SCEE")?.Value,
                            SCEAsia = m.Attribute("SCEAsia")?.Value,
                            Value = (GlobalType)int.Parse(m.Value)
                        }).ToList()
                };
            }
            else
            {
                config.Global = new GlobalConfig(); // Ensure Global is never null
            }


            var regionInfo = doc.Root.Element("REGIONINFO");
            if (regionInfo != null)
            {
                config.RegionInfo = new RegionInfo
                {
                    RegionTypes = regionInfo.Element("REGION_TYPES")?.Elements("TYPE")
                        .Select(x => new RegionType
                        {
                            Name = x.Attribute("name")?.Value,
                            Territory = x.Attribute("territory")?.Value,
                            Instance = x.Attribute("instance")?.Value,
                            Value = x.Value
                        }).ToList() ?? new List<RegionType>(),

                    RegionMaps = regionInfo.Element("REGION_MAP")?.Elements("MAP")
                        .Select(x => new RegionMap
                        {
                            Code = x.Attribute("code")?.Value,
                            Loc = x.Attribute("loc")?.Value,
                            Value = x.Value
                        }).ToList() ?? new List<RegionMap>(),

                    RegionLocalisations = regionInfo.Element("LOCALISATIONS")?.Elements("REF")
                        .Select(x => new RegionLocalisations
                        {
                            language = x.Attribute("language")?.Value,
                            Value = x.Value
                        }).ToList() ?? new List<RegionLocalisations>()
                };
            }

            return config;
        }

        private Task<List<string>> SaveToXml(Configuration config, 
            string filePath, string region, 
            bool includeEnvironmentClosed, 
            bool disableBarSupport,
            bool enableHttpGZipCompression,
            bool enableHttpDeflateompression,
            bool enableSecureCommercePoints,
            bool useRegionalServiceIds,
            string maxServiceIds,
            string AdminObjectId,
            HTTPCompressionSubsystems httpCompressionSubsystems,
            bool override182Sharc)
        {
            List<string> codes = new List<string>();

            var root = new XElement("XML",
                new XElement("VERSION", DateTime.UtcNow.ToString("yyyy/mm/dd hh:mm:ss")),
                new XElement("SecureContentRoot", config.SecureContentRoot),
                new XElement("ScreenContentRoot", config.ScreenContentRoot)
            );

            if (includeEnvironmentClosed)
            {
                //root.Add(new XComment("Enable for Server Maintenance"));
                root.Add(new XElement("ENVIRONMENT_CLOSED"));
            }


            if (config.SecureLuaObjectResourcesRoot != null)
            {
                root.Add(new XElement("secure_lua_object_resources_root", config.SecureLuaObjectResourcesRoot));
            }
            if (config.SecureLuaSceneResourcesRoot != null)
            {
                root.Add(new XElement("secure_lua_scene_resources_root", config.SecureLuaSceneResourcesRoot));
            }

            root.Add(new XElement("objects", new XElement("prepared_database")),
                        new XElement("datacapture",
                            new XElement("url",
                                    new XAttribute("mode", config.DataCaptureService.modeToEdit),
                                    config.DataCaptureService.Url
                            )
                        ),
                        config.Sha1Files.Select(f => new XElement("SHA1",
                            new XAttribute("file", f.File),
                            new XAttribute("digest", f.Digest)
                            )
                        ),
                        new XElement("profanityfilter", 
                            new XAttribute("apikey", "6b77c0b1-4636-4942-b08c-c4ee126b82ae"),
                            new XAttribute("forceOffline", "true"),
                            new XAttribute("privateKey", "NVluu9dWima10JIUKhCVvg=="),
                            new XAttribute("updaterOverrideUrl", "update-prod.pfs.online.scee.com")
                            ),
                        config.SceneRedirects.Select(r => new XElement("SceneRedirect",
                            new XAttribute("dest", r.Dest),
                            new XAttribute("src", r.Src),
                            new XAttribute("region", r.Region ?? "SCEA")
                            )
                        ),
                        new XComment("DNS Overrides.  Maximum 30 entries"),
                        new XElement("DNSOverride", 
                            new XAttribute("action", "allow"), 
                            new XAttribute("report", "on"),
                            new XAttribute("clearcache", "off"),
                            "0.0.0.0/0"
                        )
            );

            if (enableHttpGZipCompression || enableHttpDeflateompression)
            {
                var http_compression_node = new XElement("http_compression");

                string encoding = string.Empty;

                bool error = false;
                if (enableHttpGZipCompression == true 
                    && enableHttpDeflateompression == false) {
                    encoding = "gzip";
                } else if (enableHttpGZipCompression == false 
                    && enableHttpDeflateompression == true) {
                    encoding = "deflate";
                } else {
                    codes.Add("ERROR: ATTEMPTED TO ENABLE MULTIPLE ENCODINGS NOT SUPPORTED)");
                    error = true;
                }

                if(error == false)
                {
                    http_compression_node.Add(new XAttribute("encodings", encoding));
                    if (httpCompressionSubsystems.object_catalogue)
                        http_compression_node.Add(new XElement("object_catalogue"));
                    if (httpCompressionSubsystems.service_ids_list)
                        http_compression_node.Add(new XElement("service_ids_list"));
                    if (httpCompressionSubsystems.permitted_hosts_hdk)
                        http_compression_node.Add(new XElement("permitted_hosts_hdk"));
                    if (httpCompressionSubsystems.config_bundle)
                        http_compression_node.Add(new XElement("config_bundle"));
                    if (httpCompressionSubsystems.scene_list)
                        http_compression_node.Add(new XElement("scene_list"));
                    if (httpCompressionSubsystems.navigator_files)
                        http_compression_node.Add(new XElement("navigator_files"));
                    if (httpCompressionSubsystems.navigator_avatar_image)
                        http_compression_node.Add(new XElement("navigator_avatar_image"));
                    if (httpCompressionSubsystems.ssfw_get_inventory)
                        http_compression_node.Add(new XElement("ssfw_get_inventory"));
                    if (httpCompressionSubsystems.ssfw_get_selectables_trunks)
                        http_compression_node.Add(new XElement("ssfw_get_selectables_trunks"));
                    if (httpCompressionSubsystems.ssfw_get_save_data)
                        http_compression_node.Add(new XElement("ssfw_get_save_data"));
                    if (httpCompressionSubsystems.ssfw_get_scripted_save_data)
                        http_compression_node.Add(new XElement("ssfw_get_scripted_save_data"));
                    if (httpCompressionSubsystems.ssfw_get_apartment_layout)
                        http_compression_node.Add(new XElement("ssfw_get_apartment_layout"));
                    if (httpCompressionSubsystems.ssfw_get_club_details)
                        http_compression_node.Add(new XElement("ssfw_get_club_details"));
                    if (httpCompressionSubsystems.ssfw_get_layout)
                        http_compression_node.Add(new XElement("ssfw_get_layout"));
                    if (httpCompressionSubsystems.scene_xml)
                        http_compression_node.Add(new XElement("scene_xml"));
                    if (httpCompressionSubsystems.scene_description)
                        http_compression_node.Add(new XElement("scene_description"));
                    if (httpCompressionSubsystems.scene_archive)
                        http_compression_node.Add(new XElement("scene_archive"));
                    if (httpCompressionSubsystems.object_description)
                        http_compression_node.Add(new XElement("object_description"));
                    if (httpCompressionSubsystems.object_archive)
                        http_compression_node.Add(new XElement("object_archive"));
                    if (httpCompressionSubsystems.commerce_point_xml)
                        http_compression_node.Add(new XElement("commerce_point_xml"));
                    if (httpCompressionSubsystems.screen_file)
                        http_compression_node.Add(new XElement("screen_file"));
                    if (httpCompressionSubsystems.screen_content_audio)
                        http_compression_node.Add(new XElement("screen_content_audio"));
                    if (httpCompressionSubsystems.screen_content_image)
                        http_compression_node.Add(new XElement("screen_content_image"));
                    if (httpCompressionSubsystems.screen_content_video)
                        http_compression_node.Add(new XElement("screen_content_video"));
                    if (httpCompressionSubsystems.menuscreen_help)
                        http_compression_node.Add(new XElement("menuscreen_help"));
                    if (httpCompressionSubsystems.http_image)
                        http_compression_node.Add(new XElement("http_image"));

                    root.Add(http_compression_node);
                }
            }

            #region AdminObjectId
            if(AdminObjectId != null)
            {
                if(Regex.IsMatch(AdminObjectId, @"^[0-9A-Fa-f]{8}-[0-9A-Fa-f]{8}-[0-9A-Fa-f]{8}-[0-9A-Fa-f]{8}$")) {
                    root.Add(new XElement("AdminObjectId", AdminObjectId));
                } else {
                    codes.Add("ERROR: Invalid Object ID");
                }
            } else
            {
                codes.Add("WARN: No admin object ID specified, admin functionality unavailable");
            }
            #endregion

            #region useregionalserviceids
            if (useRegionalServiceIds)
            {
                root.Add(new XElement("useregionalserviceids"));
            }
            #endregion

            #region maxServiceIds
            root.Add(new XElement("maxserviceids", maxServiceIds ?? "65"));
            #endregion

            #region secure_commerce_points
            if (enableSecureCommercePoints) {

                root.Add(
                    new XElement("commerce",
                    new XElement("secure_commerce_points")
                    )
                );
            }
            #endregion

            #region Connection
            string contentServerKey = string.Empty;
            if(override182Sharc)
            {
                //Server Connection 1.82
                contentServerKey = "8243a3b10f1f1660a7fc934aac263c9c5161092dc25=";
            } else
            {
                //Server Connection 1.83+
                contentServerKey = "8b9qT7u6XQ7Sf0GKSIivMEeG7NROLTZGgNtN8iI6n1Y=";

            }
            root.Add(
                            new XElement("connection",
                                new XElement("muis", config.Connection.muis),
                                new XElement("contentserver",
                                    new XAttribute("key", contentServerKey),
                                    config.Connection.contentServer
                                )
                            )
                );
            #endregion

            #region Knight Mode 
            if (disableBarSupport)
            {
                root.Add(
                        new XElement("disablebar")
                    );
            }
            #endregion

            root.Add(
                        
                        new XComment(@"<messageQueue>
    <connect address=""prod.homemq.online.scee.com"" port=""10086"" login=""prod"" password=""monkeyface"" vhost=""cprod"" isCritical=""false"" />
    <client>/exchange/exchange.client/pshome.client.$(user)</client>
    <client>/exchange/exchange.client/pshome.client.command</client>
    <subscribe>/exchange/exchange.platform/pshome.platform.$(user)</subscribe>
    <post>/exchange/exchange.event/pshome.client.event.#</post>
    <post>/exchange/exchange.score/pshome.content.score.#</post>
    <post>/exchange/exchange.message/pshome.content.message.#</post>
    <events>
      <enabled>true</enabled>
      <destination>
        <default>/exchange/exchange.event/pshome.client.event</default>
      </destination>
    </events>
    <content>
      <message>/exchange/exchange.message/pshome.content.message</message>
      <score>/exchange/exchange.score/pshome.content.score</score>
    </content>
  </messageQueue>"),
                        new XElement("ssfw",
                            new XComment("<!--ssl accelerated services - homerewards.online.scee.com-->"),
                            new XElement("identity",
                                new XAttribute("ttl", config.SSFWConnection.ttl),
                                new XAttribute("secret", config.SSFWConnection.secret), config.SSFWConnection.identityService),
                            new XComment("<!--host ssl services - homeserverservices.online.scee.com-->"),
                            new XElement("rewards", config.SSFWConnection.rewardsService),
                            new XElement("clan", config.SSFWConnection.clanService),
                            new XElement("savedata", config.SSFWConnection.saveDataService),
                            new XElement("avatar", config.SSFWConnection.avatarService),
                            new XElement("layout", config.SSFWConnection.layoutService),
                            new XElement("trunks", config.SSFWConnection.trunksService),
                            new XElement("avatarlayout", config.SSFWConnection.avatarLayoutService),
                            new XElement("structured", config.SSFWConnection.rewardsService)
                        ),
                        new XElement("global",
                            config.Global.Modes.Select(m => new XElement("mode",
                                new XAttribute("SCEA", m.SCEA ?? ""),
                                new XAttribute("SCEJ", m.SCEJ ?? ""),
                                new XAttribute("SCEE", m.SCEE ?? ""),
                                new XAttribute("SCEAsia", m.SCEAsia ?? ""),
                                (int)m.Value))),
                        new XElement("agerestrictions",
                            new XElement("age", new XAttribute("region", "sceasia"), 0),
                            new XElement("age", new XAttribute("region", "scej"), 0),
                            new XElement("age", new XAttribute("region", "scee"), 0),
                            new XElement("age", new XAttribute("region", "scea"), 0),
                            new XElement("age", new XAttribute("region", "scek"), 0)
                        ),
                        new XElement("REGIONINFO",
                            new XComment("these define the instances into which users are divided"),
                            new XComment("this list is currently limited to 32 entries"),
                            new XElement("INSTANCE_TYPES",
                                new XElement("TYPE", new XAttribute("name", "EU")),
                                new XElement("TYPE", new XAttribute("name", "US")),
                                new XElement("TYPE", new XAttribute("name", "Japan")),
                                new XElement("TYPE", new XAttribute("name", "Asia"))
                            ),
                            new XComment("region name, instance type and region tag (which also serves as default localisation)"),
                            new XElement("REGION_TYPES",
                                new XComment("32 max entries"),
                                config.RegionInfo.RegionTypes.Select(t => new XElement("TYPE",
                                    new XAttribute("name", t.Name),
                                    new XAttribute("territory", t.Territory),
                                    new XAttribute("instance", t.Instance),
                                    t.Value)
                                )
                            ),
                            new XComment("map user into a REGION_TYPE based on their language-country code"),
                            new XElement("REGION_MAP",
                                new XComment("64 max entries"),
                                config.RegionInfo.RegionMaps.Select(m => new XElement("MAP",
                                    new XAttribute("code", m.Code),
                                    new XAttribute("loc", m.Loc),
                                    m.Value)
                                )
                            ),
                            new XComment("localisation mappings: matches against language attribute (incl wildcards) from top to bottom"),
                            new XElement("LOCALISATIONS",
                                new XComment("32 max entries"),
                                config.RegionInfo.RegionLocalisations.Select(t => new XElement("REF",
                                    new XAttribute("language", t.language),
                                    t.Value)
                                )
                            )

                        )
            );

            var doc = new XDocument(
                root
            );
;
            var docFormatted = XDocument.Parse(doc.ToString());
            docFormatted.Save(filePath, SaveOptions.None);
            return Task.FromResult(codes);
        }

    }
}
