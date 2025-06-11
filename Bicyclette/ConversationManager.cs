using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Bicyclette
{
    public static class ConversationManager
    {
        public static string FilePath = "conversations.json";
        public static List<Conversation> Conversations = new();

        public static void Load()
        {
            if (File.Exists(FilePath))
            {
                var json = File.ReadAllText(FilePath);
                Conversations = JsonSerializer.Deserialize<List<Conversation>>(json) ?? new();
            }
        }

        public static void Save()
        {
            var json = JsonSerializer.Serialize(Conversations, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }
    }
}
