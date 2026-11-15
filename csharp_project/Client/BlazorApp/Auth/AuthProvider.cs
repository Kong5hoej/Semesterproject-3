using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using ApiContracts;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace BlazorApp.Auth;

public class AuthProvider : AuthenticationStateProvider{
    private HttpClient client;
    private IJSRuntime jsRuntime;

    public AuthProvider(HttpClient client, IJSRuntime jsRuntime){
        this.client = client;
        this.jsRuntime = jsRuntime;

        AuthenticationStateChanged += async authTask => {
            string jwt = await GetJwt();
            AuthenticationHeaderValue authenticationHeaderValue = null;
            if(jwt != "")
                authenticationHeaderValue = new AuthenticationHeaderValue("Bearer", jwt);
            client.DefaultRequestHeaders.Authorization = authenticationHeaderValue;
        };
        NotifyAuthChange();
    }
    
    public override async Task<AuthenticationState> GetAuthenticationStateAsync(){
        string jwt = await GetJwt();
        if (string.IsNullOrEmpty(jwt)){
            return new AuthenticationState(new ClaimsPrincipal());
        }

        var claims = ParseClaimsFromJwt(jwt);
        ClaimsIdentity identity = new ClaimsIdentity(claims, authenticationType: "jwt", nameType: ClaimTypes.Name, roleType: "role");
        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);

        return new AuthenticationState(claimsPrincipal);
    }

    public async Task LoginAsync(UserAccessDTO userAccessDto){
        HttpResponseMessage response = await client.PostAsJsonAsync(
            "auth/login",
            userAccessDto
        );
        string jwt = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode){
            throw new Exception(jwt);
        }
        
        var claims = ParseClaimsFromJwt(jwt);
        ClaimsIdentity identity = new ClaimsIdentity(claims, authenticationType: "jwt", nameType: "name", roleType: "role");
        ClaimsPrincipal user = new ClaimsPrincipal(identity);

        await jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "SEP3G5JWT", jwt);
        NotifyAuthChange();
    }

    public async Task Logout(){
        await jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "SEP3G5JWT", "");
        NotifyAuthChange();
    }

    private void NotifyAuthChange(){
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    private async Task<string> GetJwt(){
        string jwt = "";
        try{
            jwt = await jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "SEP3G5JWT");
        } catch (InvalidOperationException e){ }
        return jwt;
    }
    
    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt){
        JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
        JwtSecurityToken token = handler.ReadJwtToken(jwt);
        return token.Claims;
    }
}