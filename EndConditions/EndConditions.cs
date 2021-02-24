namespace EndConditions
{
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Newtonsoft.Json.Linq;
    using Properties;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using YamlDotNet.Serialization;
    using ServerHandlers = Exiled.Events.Handlers.Server;

    public class EndConditions : Plugin<Config>
    {
        private static readonly string ConfigsDirectory = Path.Combine(Paths.Configs, "EndConditions");
        private static readonly string FileDirectory = Path.Combine(ConfigsDirectory, "config.yml");
        internal static EventHandlers EventHandlers;

        public override void OnEnabled()
        {
            EventHandlers = new EventHandlers(Config);
            ServerHandlers.EndingRound += EventHandlers.OnCheckRoundEnd;
            ServerHandlers.ReloadedConfigs += OnReloadedConfigs;
            ServerHandlers.RoundStarted += EventHandlers.OnRoundStart;
            LoadConditions();
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            EventHandlers.Conditions.Clear();
            ServerHandlers.EndingRound -= EventHandlers.OnCheckRoundEnd;
            ServerHandlers.ReloadedConfigs -= OnReloadedConfigs;
            ServerHandlers.RoundStarted -= EventHandlers.OnRoundStart;
            EventHandlers = null;
            base.OnDisabled();
        }

        public override string Author => "Build";
        public override string Name => "EndConditions";
        public override Version RequiredExiledVersion => new(2, 3, 3);
        public override Version Version => new(3, 1, 2);

        private void OnReloadedConfigs()
        {
            EventHandlers.Conditions.Clear();
            LoadConditions();
        }
        
        private void LoadConditions()
        {
            try
            {
                string path = Config.UsesGlobalConfig ? FileDirectory : Path.Combine(ConfigsDirectory, Server.Port.ToString(), "config.yml");
                
                if (!Directory.Exists(ConfigsDirectory))
                    Directory.CreateDirectory(ConfigsDirectory);

                if (!Config.UsesGlobalConfig)
                {
                    if (!Directory.Exists(Path.Combine(ConfigsDirectory, Server.Port.ToString())))
                        Directory.CreateDirectory(Path.Combine(ConfigsDirectory, Server.Port.ToString()));
                }

                if (!File.Exists(path))
                    File.WriteAllText(path, Encoding.UTF8.GetString(Resources.config));

                FileStream stream = File.OpenRead(path);
                IDeserializer deserializer = new DeserializerBuilder().Build();
                object yamlObject = deserializer.Deserialize(new StreamReader(stream));
                if (yamlObject == null)
                {
                    Log.Error("Unable to deserialize EndConditions win conditions!");
                    OnDisabled();
                    return;
                }

                ISerializer serializer = new SerializerBuilder().JsonCompatible().Build();
                string jsonString = serializer.Serialize(yamlObject);
                JObject json = JObject.Parse(jsonString);

                JObject configs = json.SelectToken("endconditions")?.Value<JObject>();
                if (configs == null)
                {
                    Log.Error("Could not read the EndConditions config! Disabling.");
                    OnDisabled();
                    return;
                }

                List<JProperty> groups = configs.Properties().ToList();
                foreach (JProperty group in groups)
                {
                    foreach (JObject bundle in group.Value.Children())
                    {
                        JProperty miniBundle = bundle.Properties().First();
                        var array = miniBundle.Value as JArray;
                        if (array == null)
                        {
                            Log.Error($"Unable to parse a class bundle under the condition {group.Name}.");
                            continue;
                        }

                        if (!Enum.TryParse(group.Name, out LeadingTeam leadingTeam))
                        {
                            Log.Error($"Unable to parse {group.Name} into a leading team. Skipping registration of condition.");
                            continue;
                        }

                        List<string> hold = new List<string>();
                        foreach (string item in array)
                        {
                            hold.Add(item.ToLower());
                        }

                        List<string> splitName = miniBundle.Name.Split(' ').ToList();
                        List<string> escapeConditions = splitName.Where(item => EventHandlers.EscapeTracking.ContainsKey(item)).ToList();
                        escapeConditions.ForEach(item => splitName.Remove(item));

                        EventHandlers.Conditions.Add(new Condition(escapeConditions, leadingTeam, string.Join(" ", splitName).Trim(), hold));
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error($"Error loading win conditions for EndConditions: {e.Message}\n{e.StackTrace}");
                OnDisabled();
            }
        }
    }
}