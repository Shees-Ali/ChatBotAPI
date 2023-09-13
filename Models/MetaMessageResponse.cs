namespace ChatBotAPI.Models
{
    public class Entry
    {
        public string id { get; set; }
        public long time { get; set; }
        public List<Messaging> messaging { get; set; }
    }

    public class Message
    {
        public string mid { get; set; }
        public string text { get; set; }
        public Tags tags { get; set; }
    }

    public class Messaging
    {
        public Sender sender { get; set; }
        public Recipient recipient { get; set; }
        public long timestamp { get; set; }
        public Message message { get; set; }
    }

    public class Recipient
    {
        public string id { get; set; }
    }

    public class MetaMessageResponse
    {
        public string @object { get; set; }
        public List<Entry> entry { get; set; }
    }

    public class Sender
    {
        public string id { get; set; }
    }

    public class Tags
    {
        public string source { get; set; }
    }
}
