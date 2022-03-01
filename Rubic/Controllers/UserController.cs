﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rubic.DbContext;
using Rubic.Models;
using Rubic.Models.Dto;

namespace Rubic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public MoneyBotContext _context;
        public IMapper _mapper;

        public UserController(MoneyBotContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register (UserIdentity userIdentity)
        {
            User user = new User() {
                PhoneNumberPrefix = userIdentity.PhoneNumberPrefix,
                PhoneNumber = userIdentity.PhoneNumber,
                Password = userIdentity.Password
            };
            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("Auth")]
        public async Task<ActionResult> Auth(UserIdentity userIdentity)
        {
            User user = await _context.Users.FirstOrDefaultAsync(
                u => u.PhoneNumberPrefix == userIdentity.PhoneNumberPrefix && 
                u.PhoneNumber == userIdentity.PhoneNumber && 
                u.Password == userIdentity.Password
                );

            if (user == null) return NotFound("Неверный логин или пароль");

            return Ok("Успешная авторизация");
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserInformationDto>> Get(int userId)
        {
            User user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return NotFound();

            UserInformationDto userInformationDto = _mapper.Map<UserInformationDto>(user);

            return userInformationDto;
        }
    }
}
