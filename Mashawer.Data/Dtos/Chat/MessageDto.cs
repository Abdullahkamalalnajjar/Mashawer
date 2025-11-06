namespace Mashawer.Data.Dtos.Chat
{
    public class MessageDto
    {
        public int Id { get; set; }
        public int ConversationId { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string? Content { get; set; }
        public string SenderName { get; set; } // اسم المرسل
                                               //    public string SenderProfilePicture { get; set; } // صورة المرسل
        public string RecipientName { get; set; } // اسم المستقبل
                                                  //   public string RecipientProfilePicture { get; set; } // صورة المستقبل
        public bool IsRead { get; set; }  // حقل جديد لتخزين حالة القراءة


        public string? FileBase64 { get; set; } // حقل جديد لتخزين الصورة


        public DateTime SentAt { get; set; }

    }

}
