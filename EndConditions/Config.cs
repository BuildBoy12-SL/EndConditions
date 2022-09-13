// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace EndConditions
{
#pragma warning disable SA1401 // Fields should be private
    using System.ComponentModel;
    using System.IO;
    using EndConditions.Configs;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Interfaces;
    using Exiled.Loader;

    /// <inheritdoc />
    public class Config : IConfig
    {
        /// <summary>
        /// The config containing the win conditions.
        /// </summary>
        public WinConditionsConfig WinConditions;

        /// <summary>
        /// Initializes a new instance of the <see cref="Config"/> class.
        /// </summary>
        public Config() => LoadConditions();

        /// <inheritdoc/>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the name of the file to store the win conditions config in.
        /// </summary>
        [Description("The name of the file to store the win conditions config in.")]
        public string FileName { get; set; } = "global.yml";

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="DetonationWinner"/> will be used.
        /// </summary>
        [Description("Whether Detonation Winner will be used.")]
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

        /// <summary>
        /// Loads the win condition config.
        /// </summary>
        public void LoadConditions()
        {
            string folderPath = Path.Combine(Paths.Configs, "EndConditions");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string filePath = Path.Combine(folderPath, FileName);
            WinConditions = !File.Exists(filePath)
                ? new WinConditionsConfig()
                : Loader.Deserializer.Deserialize<WinConditionsConfig>(File.ReadAllText(filePath));

            File.WriteAllText(filePath, Loader.Serializer.Serialize(WinConditions));
        }
    }
}