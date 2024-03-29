﻿

using System.Collections.Generic;

namespace Lumos
{
    public class PageEntity
    {
        public string Name { get; set; }

        public int Total { get; set; }

        public int PageSize { get; set; }

        public object Items { get; set; }
    }

    public class PageEntity<T>
    {
        public PageEntity()
        {
            this.Items = new List<T>();
        }

        public int Total { get; set; }
        public int PageSize { get; set; }
        public int PageCount { get; set; }
        public int PageIndex { get; set; }
        public List<T> Items { get; set; }
    }
}