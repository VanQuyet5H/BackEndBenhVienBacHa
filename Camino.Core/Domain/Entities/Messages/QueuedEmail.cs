using System;

namespace Camino.Core.Domain.Entities.Messages
{
    public class QueuedEmail : BaseEntity
    {
        /// <summary>
        /// Gets or sets the To property (email address)
        /// </summary>
        public string To { get; set; }

        /// <summary>
        /// Gets or sets the subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the body
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the attachment file 
        /// </summary>
        public string AttachmentFile { get; set; }

        /// <summary>
        /// Gets or sets the sent date and time
        /// </summary>
        public DateTime? DontSendBeforeDate { get; set; }

        /// <summary>
        /// Gets or sets the send tries
        /// </summary>
        public int SentTries { get; set; }

        /// <summary>
        /// Gets or sets the sent date and time
        /// </summary>
        public DateTime? SentOn { get; set; }
    }
}
