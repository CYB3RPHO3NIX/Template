namespace Template.Consumer.Utilities
{
    /// <summary>
    /// Utility for generating queue/topic names based on event types.
    /// Ensures consistent naming across all brokers and environments.
    /// </summary>
    public static class QueueNamingUtility
    {
        /// <summary>
        /// Generates a queue name from an event listener class name.
        /// Example: UserCreatedEventListener -> user-created-events
        /// </summary>
        public static string GetQueueNameFromListenerType(Type listenerType)
        {
            var name = listenerType.Name;

            // Remove "EventListener" suffix
            if (name.EndsWith("EventListener"))
                name = name.Substring(0, name.Length - "EventListener".Length);

            // Convert from PascalCase to kebab-case
            return ConvertToKebabCase(name) + "-events";
        }

        /// <summary>
        /// Generates a queue name from an event class name.
        /// Example: UserCreatedEvent -> user-created
        /// </summary>
        public static string GetQueueNameFromEventType(Type eventType)
        {
            var name = eventType.Name;

            // Remove "Event" suffix
            if (name.EndsWith("Event"))
                name = name.Substring(0, name.Length - "Event".Length);

            // Convert from PascalCase to kebab-case
            return ConvertToKebabCase(name);
        }

        /// <summary>
        /// Generates a queue name for a specific feature.
        /// </summary>
        public static string GetQueueNameForFeature(string featureName)
        {
            if (string.IsNullOrWhiteSpace(featureName))
                throw new ArgumentException("Feature name cannot be null or empty", nameof(featureName));

            return ConvertToKebabCase(featureName) + "-events";
        }

        /// <summary>
        /// Converts PascalCase to kebab-case.
        /// Example: UserCreated -> user-created
        /// </summary>
        private static string ConvertToKebabCase(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            var result = new System.Text.StringBuilder();

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];

                if (char.IsUpper(c) && i > 0)
                {
                    result.Append('-');
                    result.Append(char.ToLower(c));
                }
                else
                {
                    result.Append(char.ToLower(c));
                }
            }

            return result.ToString();
        }
    }
}
