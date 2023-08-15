﻿using EFModels.Models;
using FlexCoreService.UserCtrl.Dtos;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Xml.Linq;
using Newtonsoft.Json;
using System.Text;

namespace FlexCoreService.Controllers
{
    [EnableCors("AllowAny")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UsersController(AppDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// 取得會員資料
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        [HttpGet("{memberId}")]
        [Authorize]
        public async Task<ProfileDto> GetUserProfil(int memberId)
        {
            //StringBuilder sb = new StringBuilder();
            //sb.AppendLine("<ul>");
            //foreach (Claim claim in HttpContext.User.Claims)
            //{
            //    sb.AppendLine($@"<li> claim.Type:{claim.Type} , claim.Value:{claim.Value}</li>");
            //}
            ClaimsPrincipal user = HttpContext.User;

            if (_db.Members == null)
            {
                return null;
            }
            ProfileDto proDto = _db.Members.Include(m => m.fk_Level).Where(m => m.MemberId == memberId).Select(m => new ProfileDto
            {
                MemberId = m.MemberId,
                fk_Level = m.fk_LevelId,
                LevelName = m.fk_Level.LevelName,
                Name = m.Name,
                Email = m.Email,
                Mobile = m.Mobile,
                Gender = m.Gender,
                Birthday = m.Birthday,
                CommonAddress = m.CommonAddress,
                AlternateAddress1 = m.AlternateAddress.AlternateAddress1,
                IsSubscribeNews = m.IsSubscribeNews
            }).First();


            return proDto;
        }

        /// <summary>
        /// 登入中
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto value)
        {
            var userData = (from m in _db.Members
                            where m.Account == value.Account
                            select m).SingleOrDefault();

            var userPassword = string.Empty;
            

            if (userData == null)
            {
                //欄位或帳號驗證失敗
                return Ok(null);
            }
            else
            {                
                //驗證密碼
                userPassword = userData.EncryptedPassword;

                if (userPassword == value.EncryptedPassword)
                {
                    var claims = new List<Claim>
                    {
                    new Claim(ClaimTypes.Name, userData.Account),
                    new Claim("UserPassword", userData.EncryptedPassword),
                    new Claim("FullName", userData.Name),
                    new Claim("MemberId",userData.MemberId.ToString())//MemberId為自訂宣告的Type名稱
                   // new Claim(ClaimTypes.Role, "Administrator")//管理角色
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTime.UtcNow.AddDays(7),
                    };

                    //控制登入狀態
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                    
                    return Ok(JsonConvert.SerializeObject(claims));
                }               
                return Ok(userData.Account);
            }
        }

        /// <summary>
        /// 登出
        /// </summary>
        [HttpDelete("Logout")]
        public string Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return "ok";
        }

        /// <summary>
        /// 測試登入狀態
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public bool IsLogin()
        {
            return User.Identity.IsAuthenticated;
        }

        /// <summary>
        /// 未登入
        /// </summary>
        /// <returns></returns>
        [HttpGet("NoLogin")]
        public string NoLogin()
        {
            return "未登入";
        }


        /// <summary>
        /// 註冊
        /// </summary>
        /// <param name="regdto"></param>
        /// <returns></returns>
        [HttpPost("Register")]
        public async Task<RegisterDto> Register([FromBody] RegisterDto regdto)
        {
            Member member = new Member
            {
                Account = regdto.Account,
                EncryptedPassword = regdto.EncryptedPassword,
                Email = regdto.Email,
                Birthday = regdto.Birthday,
                Mobile = regdto.Mobile,
                Name = regdto.Name,
                fk_LevelId = 1//一般會員
            };
            //todo發送驗證信



            _db.Members.Add(member);
            await _db.SaveChangesAsync();
            return regdto;
        }

        /// <summary>
        /// 編輯會員資料
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        [HttpPut("Id")]
        public async Task<ActionResult<string>> EditUserProfile(int id, ProfileDto prodto)
        {
            
            //檢查帳號是否存在
            Member member = await _db.Members.FindAsync(id); //FindAsync 根據主键查找對應的紀錄

            if (member == null)
            {
                return NotFound("找不到對應的會員資料");
            }

            member.Email = prodto.Email;
            member.Mobile = prodto.Mobile;
            member.Gender = prodto.Gender;
            member.Birthday = prodto.Birthday;
            member.CommonAddress = prodto.CommonAddress;
            member.IsSubscribeNews = prodto.IsSubscribeNews;

            // 更新 AlternateAddress 資料
            if (member.AlternateAddress == null)
            {
                member.AlternateAddress = new AlternateAddress(); // 建立新的 AlternateAddress 物件
            }

            member.AlternateAddress.AlternateAddress1 = prodto.AlternateAddress1;
            member.AlternateAddress.AlternateAddress2 = prodto.AlternateAddress2;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MemberExists(id))
                {
                    return "編輯會員資料失敗!";
                }
                else
                {
                    throw;
                }
            }
            //檢查資料都填寫

            //跳更新成功回到本頁
            return Ok("編輯會員資料成功");
        }

        private bool MemberExists(int id)
        {
            return (_db.Members?.Any(e => e.MemberId == id)).GetValueOrDefault();
        }
    }
}
