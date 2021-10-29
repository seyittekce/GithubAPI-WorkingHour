using Core;
using Core.Abstracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Octokit;
using System;
using System.Threading.Tasks;

namespace GithubAPI_WorkingHour.Controllers
{
    public class AccountController : Controller
    {
        private readonly IOptions<GithubApiKeys> keys;
        private readonly IGitHubClient client;
        private readonly IPassword password;

        public AccountController(IOptions<GithubApiKeys> config, IGitHubClient client, IPassword password)
        {
            keys = config;
            this.client = client;
            this.password = password;
        }

        public async Task<IActionResult> IndexAsync()
        {
            string accessToken = HttpContext.Session.GetString("OAuthToken");
            if (accessToken != null)
            {
                client.Connection.Credentials = new Credentials(HttpContext.Session.GetString("OAuthToken"));
                User user = await client.User.Current();
                HttpContext.Session.SetString("avatar", user.AvatarUrl);
                HttpContext.Session.SetString("name", user.Name);
                HttpContext.Session.SetString("githubUrl", user.HtmlUrl);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return Redirect(GetOauthLoginUrl());
            }
        }

        public async Task<IActionResult> Authorize(string code, string state)
        {
            if (!string.IsNullOrEmpty(code))
            {
                string expectedState = HttpContext.Session.GetString("CSRF:State");
                if (state != expectedState)
                {
                    throw new InvalidOperationException("SECURITY FAIL!");
                }

                HttpContext.Session.Remove("CSRF:State");
                OauthToken token = await client.Oauth.CreateAccessToken(
                    new OauthTokenRequest(keys.Value.ClientId, keys.Value.ClientSecret, code));
                HttpContext.Session.SetString("OAuthToken", token.AccessToken);
            }
            return RedirectToAction("Index");
        }

        private string GetOauthLoginUrl()
        {
            string csrf = password.Generate(24, 1);
            HttpContext.Session.SetString("CSRF:State", csrf);
            OauthLoginRequest request = new OauthLoginRequest(keys.Value.ClientId)
            {
                Scopes = { "user", "notifications" },
                State = csrf
            };
            Uri oauthLoginUrl = client.Oauth.GetGitHubLoginUrl(request);
            return oauthLoginUrl.ToString();
        }
    }
}