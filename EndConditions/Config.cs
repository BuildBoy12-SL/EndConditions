namespace EndConditions
{
    using System.ComponentModel;
    using Exiled.API.Enums;
    using Exiled.API.Interfaces;

    /// <inheritdoc cref="IConfig"/>
    public sealed class Config : IConfig
    {
        /// <inheritdoc/>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the server will use the global EndConditions config.
        /// </summary>
        [Description("Whether the server will use the global EndConditions config.")]
        public bool UsesGlobalConfig { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether debug messages will be displayed.
        /// </summary>
        [Description("Whether debug messages will be displayed.")]
        public bool AllowDebug { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the plugin should allow the games default win conditions to also be read.
        /// </summary>
        [Description("Whether the plugin should allow the games default win conditions to also be read.")]
        public bool AllowDefaultEndConditions { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="DetonationWinner"/> will be used.
        /// </summary>
        [Description("Whether Detonation Winner will be used..")]
        public bool EndOnDetonation { get; set; } = false;

        /// <summary>
        /// Gets or sets the team that will win when the warhead detonates.
        /// </summary>
        [Description("The team that will win when the warhead detonates.")]
        public LeadingTeam DetonationWinner { get; set; } = LeadingTeam.FacilityForces;

        /// <summary>
        /// Gets or sets a value indicating whether tutorials will be ignored while checking if the round should end.
        /// </summary>
        [Description("Whether tutorials will be ignored while checking if the round should end.")]
        public bool IgnoreTutorials { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether friendly fire will be enabled when the round ends.
        /// </summary>
        [Description("Whether friendly fire will be enabled when the round ends.")]
        public bool RoundEndFf { get; set; } = false;
    }
}