﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Dapper.Example.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuperHeroController : ControllerBase
    {
        private readonly IConfiguration _config;
        public SuperHeroController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public async Task<ActionResult<List<SuperHero>>> GetSuperHero()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<SuperHero> heroes = await SelectAllHeroes(connection);
            return Ok(heroes);
        }

     

        [HttpGet("{heroId}")]
        public async Task<ActionResult<List<SuperHero>>> GetHero(int heroId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var hero = await connection.QueryFirstAsync<SuperHero>("select * from SuperHeroes where Id = @Id",
                new {Id=heroId});
            return Ok(hero);
        }


        [HttpPost]
        public async Task<ActionResult<List<SuperHero>>> CreateSuperHero(SuperHero hero)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("insert into SuperHeroes(name,firstname,lastname,place) values (@Name,@FirstName,@LastName,@Place)",hero);
            return Ok(await SelectAllHeroes(connection));
        }

        [HttpPut]
        public async Task<ActionResult<List<SuperHero>>> UpdateSuperHero(SuperHero hero)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("update SuperHeroes set name=@Name,firstname=@FirstName,lastname=@LastName,place=@Place where Id=@Id", hero);
            return Ok(await SelectAllHeroes(connection));
        }



        [HttpDelete("{heroId}")]
        public async Task<ActionResult<List<SuperHero>>> DeleteSuperHero(int heroId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("delete from SuperHeroes where Id =@Id",new {Id=heroId });
            return Ok(await SelectAllHeroes(connection));
        }





        private static async Task<IEnumerable<SuperHero>> SelectAllHeroes(SqlConnection connection)
        {
            return await connection.QueryAsync<SuperHero>("select * from SuperHeroes");
        }
    }
}