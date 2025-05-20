using EasyBlog.Shared.Enums;

namespace EasyBlog.Shared.Classes;

public class EasyBlogTreeNode
{
    public string Text { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public ArticleDataTypes Type { get; set; } = ArticleDataTypes.Years;
    public bool HasChildren { get; set; } = false;
    public bool IsExpanded { get; set; } = false;
    public List<EasyBlogTreeNode> Children { get; set; } = new List<EasyBlogTreeNode>();
}