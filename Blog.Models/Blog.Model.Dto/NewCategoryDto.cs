using System.Text.Json.Serialization;

namespace Blog.Api.Blog.Model.Dto;

public class NewCategoryDto
{
    public string CategoryName { get; set; }
    [JsonIgnore]
    public int PostId { get; set; }
}