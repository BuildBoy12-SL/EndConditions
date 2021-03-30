namespace EndConditions.Commands
{
    using System;
    using CommandSystem;
    using Exiled.API.Enums;
    using Exiled.Events.EventArgs;
    using Exiled.Permissions.Extensions;

    [CommandHandler(typeof(GameConsoleCommandHandler))]
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class TestConditions : ICommand
    {
        private readonly EndingRoundEventArgs testEvent = new EndingRoundEventArgs(LeadingTeam.Draw, default, false);

        /// <inheritdoc/>
        public string Command { get; } = "testconditions";

        /// <inheritdoc/>
        public string[] Aliases { get; } = Array.Empty<string>();

        /// <inheritdoc/>
        public string Description { get; } = "Fires the EndingRound method once. Used for debugging.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("ec.test"))
            {
                response = "Insufficient permission. Required: ec.test";
                return false;
            }

            EventHandlers.OnEndingRound(testEvent);
            response = "Fired event.";
            return true;
        }
    }
}