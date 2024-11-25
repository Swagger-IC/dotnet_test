using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Web;
using Rise.Client;
using Rise.Client.Products.Services;
using Rise.Shared.Products;
using Rise.Shared.Users;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Client.Auth;
using Blazored.LocalStorage;
using Blazored.Modal;
using Blazored.Toast;
using Rise.Client.Orders;
using Rise.Shared.Roles;
using Rise.Client.Users.Services;
using Rise.Shared.Leveranciers;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredModal();

builder.Services.AddScoped<ProductenlijstStatus>();
builder.Services.AddScoped<Winkelmand>();

builder.Services.AddBlazoredToast();

builder.Services.AddHttpClient<IProductService, ProductService>(client =>
{
    client.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}api/");
}).AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();;

builder.Services.AddHttpClient<ILeverancierService, LeverancierService>(client =>
{
    client.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}api/");
}).AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>(); ;

builder.Services.AddHttpClient<IUserService, UserService>(client =>
{
    client.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}");
}).AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();;

builder.Services.AddHttpClient<IRolService, RolService>(client =>
{
    client.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}api/");
}).AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();;

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("Auth0", options.ProviderOptions);
    options.ProviderOptions.ResponseType = "code";
    options.ProviderOptions.PostLogoutRedirectUri = builder.HostEnvironment.BaseAddress;
    options.ProviderOptions.AdditionalProviderParameters.Add("audience", builder.Configuration["Auth0:Audience"]!);
}).AddAccountClaimsPrincipalFactory<ArrayClaimsPrincipalFactory<RemoteUserAccount>>();

await builder.Build().RunAsync();
