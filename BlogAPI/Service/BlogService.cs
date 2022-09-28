using BlogAPI.Model;
using BlogAPI.ModelDTO;
using BlogAPI.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Service;

public class BlogService : IBlogService
{
    private readonly DataContext _dataContext;
    DateTime now = DateTime.Now;
    public BlogService(DataContext dataContext)
    {
        _dataContext = dataContext;
    }


    public async Task<List<BlogPost>> GetAllPost()
    {
        return await _dataContext.BlogPosts.Include(x => x.Author)
            .Include(x=>x.Category)
            .ToListAsync();
    }

    public async Task<BlogPost?> GetPostById(int id)
    {
        var post = await _dataContext.BlogPosts.Where(x => x.PostId == id)
            .Include(x => x.Author)
            .Include(x=>x.Category)
            .FirstOrDefaultAsync();
        if (post == null)
        {
            return null;
        }
        return post;
    }

    public async Task<List<Author>> GetPostByAuthor(int id)
    {
        var author = await _dataContext.Authors.Where(x => x.AuthorId == id)
            .Include(x => x.BlogPosts)
            .ToListAsync();
        return author;
    }

    public async Task<BlogPost> AddPost(NewPostDTO newPost)
    {
        var author = await _dataContext.Authors.Where(x => x.AuthorId == newPost.AuthorId)
            .Include(x => x.BlogPosts)
            .FirstOrDefaultAsync();
        
        var category = await _dataContext.Categories
            .Where(x => x.CategoryId == newPost.CategoryId)
            .FirstOrDefaultAsync();
        
        var post = new BlogPost
        {
            Title = newPost.Title,
            Summary = newPost.Summary,
            Body = newPost.Body,
            Author = author,
            Tags = newPost.Tags,
            AuthorId = newPost.AuthorId,
            Category = category,
            CategoryId = newPost.CategoryId,
            Updated = newPost.Updated,
            Created=DateTime.UtcNow

        };
        _dataContext.BlogPosts.Add(post);
        await _dataContext.SaveChangesAsync();
        return post;
    }

    public async Task<BlogPost> UpdatePost(NewPostDTO updatePost)
    {
        var author = await _dataContext.Authors.Where(x => x.AuthorId == updatePost.AuthorId)
            .Include(x => x.BlogPosts)
            .FirstOrDefaultAsync();
        var category = await _dataContext.Categories.Where(x => x.CategoryId == updatePost.CategoryId)
            .FirstOrDefaultAsync();
        if (author == null)
        {
            return null;
        }
        var post = await _dataContext.BlogPosts.FindAsync(updatePost.Id);
        post.PostId = updatePost.Id;
        post.Title = updatePost.Title;
        post.Summary = updatePost.Summary;
        post.Body = updatePost.Body;
        post.Tags = updatePost.Tags;
        post.Category = category;
        post.Author = author;
        post.Created = post.Created;
        post.Updated = DateTime.UtcNow;

        _dataContext.BlogPosts.Update(post);
        await _dataContext.SaveChangesAsync();
        return post;

    }

    public async Task<Author> AddAuthor(NewAuthorDTO newAuthor)
    {
        var post = await _dataContext.BlogPosts.Where(x => x.AuthorId == newAuthor.Id)
            .Include(x => x.Author)
            .ToListAsync();
        var author = new Author
        {
            AuthorId = newAuthor.Id,
            Name = newAuthor.Name,
            Description = newAuthor.Description,
            BlogPosts = post
        };
        _dataContext.Authors.Add(author);
        await _dataContext.SaveChangesAsync();
        return author;
    }

    public async Task<Category> AddCategory(NewCategoryDTO newCategory)
    {
        var post = await _dataContext.BlogPosts.Where(x => x.PostId == newCategory.PostId)
            .Include(x => x.Author)
            .Include(x => x.Category).ToListAsync();
        var category = new Category
        {
            CategoryId = newCategory.CategoryId,
            CategoryName = newCategory.CategoryName,
            BlogPosts = post
        };
        _dataContext.Categories.Add(category);
        await _dataContext.SaveChangesAsync();
        return category;
    }

    public async Task DeletePost(int id)
    {
        var post = await _dataContext.BlogPosts.FindAsync(id);
        _dataContext.BlogPosts.Remove(post);
        await _dataContext.SaveChangesAsync();
    }
}