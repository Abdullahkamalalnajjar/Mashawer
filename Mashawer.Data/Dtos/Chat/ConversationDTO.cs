namespace Mashawer.Data.Dtos.Chat
{
    public class ConversationDTO
    {
        public int Id { get; set; }
        public string SenderId { get; set; } // معرف المرسل
        public string ReceiverId { get; set; } // معرف المستقبل
        public string UserName { get; set; } // اسم المرسل
        public string UserImage { get; set; } // صورة المرسل
        public string LastMessageContent { get; set; } // محتوى الرسالة الأخيرة
        public int UnreadMessagesCount { get; set; } // محتوى الرسالة الأخيرة
        public DateTime? LastMessageTimestamp { get; set; } // توقيت الرسالة الأخيرة

    }
    public class ConversationForAdminDTO
    {
        public int Id { get; set; }
        public string SenderId { get; set; } // معرف المرسل
        public string ReceiverId { get; set; } // معرف المستقبل
        public string UserName { get; set; } // اسم المرسل
        public string UserImage { get; set; } // صورة المرسل
        public string UserName2 { get; set; } // اسم المرسل
        public string UserImage2 { get; set; } // صورة المرسل
        public string LastMessageContent { get; set; } // محتوى الرسالة الأخيرة
        public DateTime? LastMessageTimestamp { get; set; } // توقيت الرسالة الأخيرة
        public List<MessageDto> Messages { get; set; } = new();

    }


}
