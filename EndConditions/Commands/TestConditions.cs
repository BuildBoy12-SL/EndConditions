namespace EndConditions.Commands
{
    using CommandSystem;
    using Exiled.API.Enums;
    using Exiled.Events.EventArgs;
    using Exiled.Permissions.Extensions;
    using System;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class TestConditions : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("ec.test"))
            {
                response = "Insufficient permission. Required: ec.test";
                return false;
            }
            
            EndConditions.EventHandlers.OnCheckRoundEnd(new EndingRoundEventArgs(LeadingTeam.Draw, default, false));
            response = "Fired event.";
            return true;
        }

        public string Command { get; } = "testconditions";
        public string[] Aliases { get; } = Array.Empty<string>();
        public string Description { get; } = "Fires the EndingRound method once. Used for debugging.";
    }
}