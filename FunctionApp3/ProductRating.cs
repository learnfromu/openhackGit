using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionApp3
{
    public class ProductRating
    {
        public string ProductId { get; set; }
        public string UserId { get; set; }
        public string LocationName { get; set; }
        public int Rating { get; set; }
        public string UserNotes { get; set; }
        public string id { get; set; }
        public DateTime Timestamp { get; set; }

        public void PopulateIdandTime()
        {
            id = Guid.NewGuid().ToString();
            Timestamp = DateTime.UtcNow;
        }
    }
}
