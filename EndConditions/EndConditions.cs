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
        private static readonly string ConfigDirectory = Path.Combine(Paths.Configs, "EndConditions");
        private static readonly string FileDirectory = Path.Combine(ConfigDirectory, "config.yml");
        private readonly Handler _handler = new Handler();

        public override void OnEnabled()
        {
            LoadConditions();
            Instance = this;
            ServerEvents.EndingRound += _handler.OnCheckRoundEnd;
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Handler.EndConditions.Clear();
            ServerEvents.EndingRound -= _handler.OnCheckRoundEnd;
            Instance = null;
        }

        public override string Author => "Build";
        public override string Name => "EndConditions";
        public override Version RequiredExiledVersion => new Version(2, 1, 16);
        public override Version Version => new Version(3, 0, 0);

        private void LoadConditions()
        {
            try
            {
                string path = Config.UsesGlobalConfig
                    ? FileDirectory
                    : Path.Combine(ConfigDirectory, ServerConsole.Port.ToString(), "config.yml");
                if (!Directory.Exists(ConfigDirectory))
                    Directory.CreateDirectory(ConfigDirectory);
                //If it doesn't exist, make it so it does
                if (!Config.UsesGlobalConfig)
                {
                    if (!Directory.Exists(Path.Combine(ConfigDirectory, ServerConsole.Port.ToString())))
                        Directory.CreateDirectory(Path.Combine(ConfigDirectory, ServerConsole.Port.ToString()));
                }

                if (!File.Exists(path))
                    File.WriteAllText(path, Encoding.UTF8.GetString(Resources.config));

                //Read the file
                FileStream stream = File.OpenRead(path);
                IDeserializer deserializer = new DeserializerBuilder().Build();
                object yamlObject = deserializer.Deserialize(new StreamReader(stream));
                ISerializer serializer = new SerializerBuilder().JsonCompatible().Build();
                string jsonString = serializer.Serialize(yamlObject);
                JObject json = JObject.Parse(jsonString);

                //Get the EndConditions
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
