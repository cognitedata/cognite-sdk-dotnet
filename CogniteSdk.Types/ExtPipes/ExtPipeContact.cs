using System;
using System.Collections.Generic;
using System.Text;

namespace CogniteSdk
{
    /// <summary>
    /// Contact for an extraction pipeline
    /// </summary>
    public class ExtPipeContact
    {
        /// <summary>
        /// Contact name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Contact email, up to 254 characters
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Contact role
        /// </summary>
        public string Role { get; set; }
        /// <summary>
        /// True, if contact should receive email notifications
        /// </summary>
        public bool SendNotification { get; set; }
    }
}
