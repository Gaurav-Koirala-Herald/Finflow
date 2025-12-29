using FinFlowAPI.Data;
using FinFlowAPI.DTO;
using FinFlowAPI.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace FinFlowAPI.Services.Comments;

public class CommentService : ICommentService
{
    private readonly ApplicationDbContext _context;
    public CommentService(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<CommentDTO> CreateCommentAsync(CommentDTO dto)
    {
        var mappedRequest = dto.Adapt<Comment>();
        await _context.Comments.AddAsync(mappedRequest);
        await _context.SaveChangesAsync();
        return dto;
    }

    public async Task<bool> DeleteCommentAsync(int id)
    {
        var existingComment = await _context.Comments.SingleOrDefaultAsync(x => x.Id == id);
        if (existingComment == null) return false;
        _context.Comments.Remove(existingComment);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<List<CommentDTO>> GetAllCommentsAsync(int postId)
    {
        var comments = await _context.Comments.Where(x => x.PostId == postId).ToListAsync();
        var mappedResponse = comments.Adapt<List<CommentDTO>>();
        return mappedResponse;
    }
}