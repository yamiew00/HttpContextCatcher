using System.Collections.Generic;

namespace HttpContextCatcher
{
    /// <summary>
    /// catch all items of httpcontext.Items dictionary.
    /// </summary>
    public class ItemCatcher
    {
        public Dictionary<string, object> Items { get; set; }
    }
}
