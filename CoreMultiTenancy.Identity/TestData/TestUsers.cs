using System;
using System.Collections.Generic;
using System.Security.Claims;
using IdentityServer4.Test;

namespace CoreMultiTenancy.Identity.TestData
{
    // Should be removed to an external testing project eventually
    public class TestUsers
    {
        public static List<TestUser> Users = new List<TestUser>()
        {
          new TestUser
          {
              SubjectId = Guid.NewGuid().ToString(),
              Username = "TestUser1",
              Password = "password",
              Claims = new List<Claim>
              {

              }
          },
          new TestUser
          {
              SubjectId = Guid.NewGuid().ToString(),
              Username = "TestUser2",
              Password = "password",
              Claims = new List<Claim>
              {
                  
              }
          }
        };
    }
}