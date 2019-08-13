

namespace Lumos
{
    public class PageEntity
    {
        public string Name { get; set; }

        public int Total { get; set; }

        public int PageSize { get; set; }

        public object Items { get; set; }

        public object Status { get; set; }
    }
}