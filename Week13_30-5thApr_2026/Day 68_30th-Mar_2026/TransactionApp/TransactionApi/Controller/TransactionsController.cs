using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TransactionApi.Data;
using TransactionApi.DTO;
using TransactionApi.Model;

namespace TransactionApi.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public TransactionsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetTransactions()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var transactions = _context.Transactions
                .Where(t => t.UserId == userId)
                .ToList();

            var result = _mapper.Map<List<TransactionDTO>>(transactions);

            return Ok(result);
        }
    }
}
