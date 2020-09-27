namespace EndConditions
{
	using Exiled.API.Interfaces;
	using System.ComponentModel;
	
    public sealed class Config : IConfig
    {
        [Description("Enable if EndConditions should be loaded.")]
        public bool IsEnabled { get; set; } = true;

		/// <summary>
		/// If the current server should use the global EndConditions config, or create a local one.
		/// </summary>
		[Description("Enable if the server should use the global EndConditions config.")]
		public bool UsesGlobalConfig { get; set; } = true;

		/// <summary>
		/// If various extra information messages should be sent to console. eg. Team wins due to X condition standard.
		/// </summary>
		[Description("Enable if the plugin should send more information messages.")]
		public bool AllowVerbose { get; set; } = false;

		/// <summary>
		/// If the debug messages should be sent. Disabled by default due to spam resulting in low visibility of other plugin debug messages.
		/// </summary>
		[Description("Enable if the plugin should send debug messages.")]
		public bool AllowDebug { get; set; } = false;

		/// <summary>
		/// If the plugin should still allow the game to have its win conditions as well. Disabled by default to preserve consistency.
		/// </summary>
		[Description("Enable if the plugin should allow the games default win conditions as well as the configured ones.")]
		public bool AllowDefaultEndConditions { get; set; } = false;

		/// <summary>
		/// Once the warhead detonates, the round will automatically end if a winner is configured. If set to none, this will do nothing.
		/// </summary>
		[Description("Who should win if the warhead detonates. Set to \"none\" to disable this, otherwise use leading team names from the #resources channel.")]
		public string DetonationWinner { get; set; } = "none";

		/// <summary>
		/// If the plugin will register tutorials as it would other classes.
		/// </summary>
		[Description("If tutorials should be ignored as normal, disable if you have plugins like SerpentsHand and set conditions accordingly.")]
		public bool IgnoreTutorials { get; set; } = true;		
	}
}
