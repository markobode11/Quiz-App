using System.Collections.Generic;

namespace PublicApi.DTO.v1.Account
{
    public class Message
    {
        public Message()
        {
        }

        public Message(params string[] messages)
        {
            Messages = messages;
        }

        public IList<string> Messages { get; set; } = new List<string>();
    }
}