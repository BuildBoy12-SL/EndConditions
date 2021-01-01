namespace EndConditions
{
    using Exiled.API.Features;
    using Newtonsoft.Json.Linq;
    using Properties;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using YamlDotNet.Serialization;
    using ServerEvents = Exiled.Events.Handlers.Server;

    public class EndConditions : Plugin<Config>
    {
        internal static EndConditions Instance;
        private static readonly string ConfigsDirectory = Path.Combine(Paths.Configs, "EndConditions");
        private static readonly string FileDirectory = Path.Combine(ConfigsDirectory, "config.yml");
        private readonly Handler _handler = new();

        public override void OnEnabled()
        {
            LoadConditions();
            Instance = this;
            ServerEvents.RoundStarted += _handler.OnRoundStart;
            ServerEvents.EndingRound += _handler.OnCheckRoundEnd;
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Handler.EndConditions.Clear();
            ServerEvents.RoundStarted -= _handler.OnRoundStart;
            ServerEvents.EndingRound -= _handler.OnCheckRoundEnd;
            Instance = null;
        }

        public override string Author => "Build";
        public override string Name => "EndConditions";
        public override Version RequiredExiledVersion => new(2, 1, 22);
        public override Version Version => new(3, 0, 2);

        private void LoadConditions()
        {
            try
            {
                string path = Config.UsesGlobalConfig
                    ? FileDirectory
                    : Path.Combine(ConfigsDirectory, ServerConsole.Port.ToString(), "config.yml");
                if (!Directory.Exists(ConfigsDirectory))
                    Directory.CreateDirectory(ConfigsDirectory);
                
                if (!Config.UsesGlobalConfig)
                {
                    if (!Directory.Exists(Path.Combine(ConfigsDirectory, ServerConsole.Port.ToString())))
                        Directory.CreateDirectory(Path.Combine(ConfigsDirectory, ServerConsole.Port.ToString()));
                }

                if (!File.Exists(path))
                    File.WriteAllText(path, Encoding.UTF8.GetString(Resources.config));

                FileStream stream = File.OpenRead(path);
                IDeserializer deserializer = new DeserializerBuilder().Build();
                object yamlObject = deserializer.Deserialize(new StreamReader(stream));
                ISerializer serializer = new SerializerBuilder().JsonCompatible().Build();
                string jsonString = serializer.Serialize(yamlObject);
                JObject json = JObject.Parse(jsonString);

                JObject configs = json.SelectToken("endconditions").Value<JObject>();
                JProperty[] groups = configs.Properties().ToArray();
                foreach (JProperty group in groups)
                {
                    foreach (JObject bundle in group.Value.Children())
                    {
                        JProperty minibundle = bundle.Properties().First();
                        List<string> hold = new List<string>();
                        foreach (string classes in minibundle.Value as JArray)
                        {
                            hold.Add(classes.ToLower());
                        }

                        Handler.EndConditions.Add($"{group.Name.ToLower()}-{minibundle.Name.ToLower()}", hold);
                    }
                }

                Log.Info("EndConditions Configs loaded.");
            }
            catch (Exception e)
            {
                Log.Error($"Error loading win conditions for EndConditions: {e}");
            }
        }
    }
}