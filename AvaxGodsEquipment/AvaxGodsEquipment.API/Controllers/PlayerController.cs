using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

[ApiController]
[Route("api/[controller]")]
public class PlayerController : ControllerBase
{
    private readonly IPlayerService _playerService;

    public PlayerController(IPlayerService playerService)
    {
        _playerService = playerService;
    }

    [HttpGet("{walletAddress}")]
    public async Task<ActionResult<Player>> GetPlayer(string walletAddress)
    {
        var player = await _playerService.GetPlayer(walletAddress);
        return Ok(player);
    }

    [HttpGet("{walletAddress}/balance")]
    public async Task<ActionResult<decimal>> GetBalance(string walletAddress)
    {
        var balance = await _playerService.GetBalance(walletAddress);
        return Ok(balance);
    }
}