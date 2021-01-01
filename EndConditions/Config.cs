namespace EndConditions
{
    using Exiled.API.Interfaces;
    using System.ComponentModel;

    public sealed class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;

        [Description("Enable if the server should use the global EndConditions config.")]
        public bool UsesGlobalConfig { get; set; } = true;

        [Description("Enable if the plugin should send more information messages.")]
        public bool AllowVerbose { get; set; } = false;

        [Description("Enable if the plugin should send debug messages.")]
        public bool AllowDebug { get; set; } = false;

        [Description(
            "Enable if the plugin should allow the games default win conditions as well as the configured ones.")]
        public bool AllowDefaultEndConditions { get; set; } = false;

        [Description(
            "Who should win if the warhead detonates. Set to \"none\" to disable this, otherwise use leading team names from the #resources channel.")]
        public string DetonationWinner { get; set; } = "none";

        [Description(
            "If tutorials should be ignored as normal, disable if you have plugins like SerpentsHand and set conditions accordingly.")]
        public bool IgnoreTutorials { get; set; } = true;

        [Description("Enables FriendlyFire when the round ends when enabled.")]
        public bool RoundEndFf { get; set; } = false;
    }
}