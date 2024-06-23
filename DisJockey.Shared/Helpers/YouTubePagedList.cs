using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisJockey.Shared.Helpers {
    public class YouTubePagedList<T> : List<T> {
        public string CurrentPageToken { get; set; }
        public string NextPageToken { get; set; }
        public string PreviousPageToken { get; set; }

        public YouTubePagedList(IEnumerable<T> items, string currentPageToken, string nextPageToken, string previousPageToken) {
            CurrentPageToken = currentPageToken;
            NextPageToken = nextPageToken;
            PreviousPageToken = previousPageToken;
            AddRange(items);
        }

        public static YouTubePagedList<T> Empty() => new YouTubePagedList<T>(Enumerable.Empty<T>(), string.Empty, string.Empty, string.Empty);
    }
}
