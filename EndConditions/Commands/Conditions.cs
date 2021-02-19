namespace EndConditions.Commands
{
    using CommandSystem;
    using NorthwoodLib.Pools;
    using System;
    using System.Text;

    [CommandHandler(typeof(GameConsoleCommandHandler))]
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Conditions : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
            foreach (Condition condition in EventHandlers.Conditions)
            {
                stringBuilder.Append(Environment.NewLine).AppendLine(condition.Name).AppendLine($"Leading Team: {condition.LeadingTeam}")
                    .AppendLine($"Roles: {string.Join(", ", condition.RoleConditions)}");
            }

            response = stringBuilder.ToString();
            StringBuilderPool.Shared.Return(stringBuilder);
            return true;
        }

        public string Command { get; } = "conditions";
        public string[] Aliases { get; } = Array.Empty<string>();
        public string Description { get; } = "Lists all current conditions.";
    }
}