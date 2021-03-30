namespace EndConditions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Newtonsoft.Json.Linq;
    using Properties;
    using YamlDotNet.Serialization;
    using ServerHandlers = Exiled.Events.Handlers.Server;

    /// <summary>
    /// The main plugin class.
    /// </summary>
    public class EndConditions : Plugin<Config>
    {
        private static readonly string ConfigsDirectory = Path.Combine(Paths.Configs, "EndConditions");

        private static readonly string FileDirectory = Path.Combine(ConfigsDirectory, "config.yml");

        private static readonly EndConditions InstanceValue = new EndConditions();

        private EndConditions()
        {
        }

        /// <summary>
        /// Gets a static instance of the <see cref="EndConditions"/> class.
        /// </summary>
        public static EndConditions Instance { get; } = InstanceValue;

        /// <inheritdoc/>
        public override string Author { get; } = "Build";

        /// <inheritdoc/>
        public override string Name { get; } = "EndConditions";

        /// <inheritdoc/>
        public override Version RequiredExiledVersion { get; } = new Version(2, 9, 4);

        /// <inheritdoc/>
        public override Version Version { get; } = new Version(3, 1, 3);

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            ServerHandlers.EndingRound += EventHandlers.OnEndingRound;
            ServerHandlers.ReloadedConfigs += OnReloadedConfigs;
            LoadConditions();
            base.OnEnabled();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            EventHandlers.Conditions.Clear();
            ServerHandlers.EndingRound -= EventHandlers.OnEndingRound;
            ServerHandlers.ReloadedConfigs -= OnReloadedConfigs;
            base.OnDisabled();
        }

        private void OnReloadedConfigs()
        {
            bool isLocked = Round.IsLocked;
            Round.IsLocked = true;

            EventHandlers.Conditions.Clear();
            LoadConditions();

            Round.IsLocked = isLocked;
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

                        if (!Enum.TryParse(group.Name, true, out LeadingTeam leadingTeam))
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

                        EventHandlers.Conditions.Add(new Condition
                        {
                            EscapeConditions = escapeConditions,
                            LeadingTeam = leadingTeam,
                            Name = string.Join(" ", splitName).Trim(),
                            RoleConditions = hold,
                        });
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error($"Error loading win conditions for EndConditions: {e}");
                OnDisabled();
            }
        }
    }
}