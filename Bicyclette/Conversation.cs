using System.Collections.Generic;

namespace Bicyclette
{
    public class Conversation
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public List<string> Messages { get; set; } = new();
    }
}

