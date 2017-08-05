using PSNLib.Models;
using TheHttp;

namespace PSNLib
{
    public class Community
    {
        private readonly string _accessToken;
        internal Community(PSAPI context)
        {
            var param = new
            {
                npsso = context._npsso,
                device_profile = "mobile",
                grant_type = "sso_cookie",
                scope = "psn:sceapp,user:account.get,user:account.settings.privacy.get,user:account.settings.privacy.update,user:account.realName.get,user:account.realName.update,kamaji:get_account_hash,kamaji:communities_mobileapp,kamaji:satchel,kamaji:game_list,capone:report_submission",
                service_entity = "urn:service-entity:psn",
                ui = "pr",
                duid = "0000000d000400808F4B3AA3301B4945B2E3636E38C0DDFC",
                client_id = "58f7f128-5325-41f1-bcff-7b590b7200cd",
                client_secret = "BFogDNpBInrYB8s0"
            };
            var res = Http.Post(Consts.APIEndpoints.OAUTH_URL, param);
            if (res.HasError)
                throw new PSNException(res.ErrorMessage.AsJson()["error_description"]);

            _accessToken = res.AsJson()["access_token"];

        }

        public Models.Community[] GetMyCommunities(int limit = 50)
        {
            var res = Http.Get(Consts.APIEndpoints.COMMUNITIES_URL +
                               "?fields=backgroundImage%2CtilebackgroundImage%2Cdescription%2Cid%2Cmembers%2Cname%2CprofileImage%2Crole%2CunreadMessageCount%2Csessions%2Ctype%2Clanguage%2Ctimezone%2CtitleName%2C%20titleId%2CnameLastModifiedBy%2CdescriptionLastModifiedBy%2CgriefReportableItems%2CgameSessions%2Cparties%26includeFields%3DgameSessions%2Cparties&limit=" +
                               limit, null, null, new { Authorization = "Bearer " + _accessToken });

            if (res == null || res.HasError)
                return new Models.Community[0];
            int len = res.AsJson()["total"];
            var allCommunites = res.AsJson()["communities"];
            var communites = new Models.Community[len];
            for (int i = 0; i < len; i++)
            {
                communites[i] = new Models.Community();
                communites[i].Name = allCommunites[i]["name"];
                communites[i].Id = allCommunites[i]["id"];
                communites[i].MembersCount = allCommunites[i]["members"]["size"];
                communites[i].Role = allCommunites[i]["role"];
                communites[i].IsOpen = allCommunites[i]["type"] == "open";
            }
            return communites;
        }

        public bool Leave(string communityId)
        {
            var res = Http.Delete(Consts.APIEndpoints.COMMUNITIES_URL + communityId + "/members", new {}, null, new
            {
                Authorization = "Bearer " + _accessToken,
                ContentType = "application/json"
            });
            return !res.HasError;
        }
    }
}
