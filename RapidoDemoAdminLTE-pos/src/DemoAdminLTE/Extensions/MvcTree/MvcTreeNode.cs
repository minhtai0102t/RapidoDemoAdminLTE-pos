using System;
using System.Collections.Generic;

namespace DemoAdminLTE.Extensions
{
    public class MvcTreeNode
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public List<MvcTreeNode> Children { get; set; }

        public MvcTreeNode(int? id, string title)
        {
            Id = id;
            Title = title;
            Children = new List<MvcTreeNode>();
        }
        public MvcTreeNode(string title)
            : this(null, title)
        {
        }
    }
}
