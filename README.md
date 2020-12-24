# Goals
The overall goal of this project is to establish a simple proof-of-concept for a larger planned project. A centralized identity provider (IdP) will serve to log users in and issue JWTs with the OpenIdConnect protocol. Clients will use this token to call a web API with the main data of the application. A permissions server will handle deciding whether users have authorization to perform something in real-time. Clients will be able to receive and cache permissions information so they know how to display their dynamic forms and controls. Once the API call is made, the web API will verify that the user has access and, if they do, do what they need to do.

The multitenancy aspect is handled almost entirely by the clients. They implement their own portal where a user selects from their lists of available tenants. Handling this in the client as opposed to setting this on the identity side means that it is easier and faster for the user to work with multiple tenants, accessing data in one and then quickly moving to another. The client will use the selected tenant's GUID when making calls to the API.
## The main problem - permissions
Permissions is really the largest problem here. When it boils down to it, there are really three possible ways to handle updating of permissions.
#### 1 - Eventual update via token refresh
The easiest of the 3 methods, the basis of this is to put permissions information (or, at least, roles) into the JWT and have a very short refresh timer (< 10 minutes) so that when a user's access is changed, it will be updated *eventually*. However, when a user consumes an invitation and is granted access to a new tenant, this needs to be updated immediately. This is really the only caveat that makes this method not possible. In addition to this, storing roles and permissions in the JWT is a controversial topic and isn't supposed to be done. It adds overhead to each request and clashes with the fact that JWT has issues with stale data.
#### 2 - Force a token refresh
This seems to be how Auth0 does it at a glance. They allow users to force a user's token to be re-issued with fresh data. I have heard of others who have implemented custom stores to handle versioning of tokens so that a user knows when its token needs to be refreshed. Obviously, this stateful usage of JWT largely defeats the purpose of using them but it is an interesting idea nonetheless. Essentially it is the same as tracking JWTs for recovation, however as far as I know there is no way to force a user to update its JWT. Revocation requires a new log in, which is against the point of refreshing stale data as the user should not have to log in again. So not only would this require custom code server-side and client-side, it's forcing a square block into a round hole. Also, tenant selection would have to take place identity-side because each JWT would be issued with only the permissions wrt a single tenant. Handling the tenant identity-side isn't as flexible.
#### 3 - Permissions server (policy server)
The logical conclusion is to server permissions via a separate server which determines whether a user has access to the API action. However, this requires a bit of infrastructure and adds a heavy overhead to each request from a client. Firstly, a custom claims mapper must be created (and permissions cached most likely). Secondly, some infrastructure needs to be setup so that a web API (on a separate server) is able to call the permissions server and essentially ask (can user x do operation y), which obviously adds an overhead of one call per request, at least as far as endpoints that require permission. Furthermore, the client(s) must also know this data, however refreshing every single call would double the overhead, so a simple cache that updates periodically or when a user does something they don't have access to would be smarter. Policy Server does something similar although it is monolithic and all done via ASP.NET Core MVC. The hardest part will probably be having the web API communicate and with permissions endpoint and ask in some typesafe way if a user can do an operation.
# The policy server 
In this case, the policy server is coupled with the Identity server for ease of accessing the underlying database containing users, tenants, etc.
#### Communication
Communication is done via gRPC calls from the API server to the Identity server. authorization.proto contains the details of the request/reply.
https://chromium.googlesource.com/external/github.com/grpc/grpc/+/HEAD/src/csharp/BUILD-INTEGRATION.md
# Setup
#### Email Configuration
Email configuration is stored in user secrets and injected via the IOptions<TOptions> interface.
To set the email configuration, sign up for a free account with SendGrid and enter your credentials into user secrets:
```
cd CoreMultiTenancy.Identity
dotnet user-secrets set "Email:SendGridUser" "<your_username>"
dotnet user-secrets set "Email:SendGridKey" "<your_key>"
```