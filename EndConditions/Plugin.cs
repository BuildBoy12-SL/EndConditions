namespace EndConditions
{
	using System;
	using System.Collections.Generic;
	using System.IO;
    using System.Linq;
    using System.Text;
    using EndConditions.Properties;
    using Exiled.API.Features;
    using Newtonsoft.Json.Linq;
    using YamlDotNet.Serialization;
    
    public class Plugin : Plugin<Config>
	{
		public static string PluginDirectory = Path.Combine(Paths.Plugins, "EndConditions");
		public static string FileDirectory = Path.Combine(PluginDirectory, "config.yml");
		private static readonly Lazy<Plugin> LazyInstance = new Lazy<Plugin>(() => new Plugin());
		private Plugin() { }
		public static Plugin ConfigRef => LazyInstance.Value;
		private Handler handler;

		public override void OnEnabled() 
		{
			base.OnEnabled();
			LoadConditions();
			handler = new Handler(this);
			Exiled.Events.Handlers.Server.EndingRound += handler.OnCheckRoundEnd;
			Log.Info("EndConditions Loaded.");
		}

		public override void OnDisabled() 
		{
			Exiled.Events.Handlers.Server.EndingRound -= handler.OnCheckRoundEnd;
			handler = null;
		}

		public void LoadConditions()
		{
			try
			{
				string path = Config.UsesGlobalConfig ? FileDirectory : Path.Combine(PluginDirectory, ServerConsole.Port.ToString(), "config.yml");
				if (!Directory.Exists(PluginDirectory))
					Directory.CreateDirectory(PluginDirectory);
				//If it doesn't exist, make it so it does
				if (!Config.UsesGlobalConfig)
				{
					if (!Directory.Exists(Path.Combine(PluginDirectory, ServerConsole.Port.ToString())))
						Directory.CreateDirectory(Path.Combine(PluginDirectory, ServerConsole.Port.ToString()));
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
						List<string> hold = new List<string>();
						JProperty minibundle = bundle.Properties().First();
						foreach (string classes in minibundle.Value as JArray)
						{
							hold.Add(classes.ToLower());
						}
						Handler.escapeConditions.Add($"{group.Name.ToLower()}-{minibundle.Name.ToLower()}", hold);
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