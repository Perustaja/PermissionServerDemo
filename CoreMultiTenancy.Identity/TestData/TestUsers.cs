using System;
using System.Collections.Generic;
using System.Security.Claims;
using IdentityServer4.Test;

namespace CoreMultiTenancy.Identity.TestData
{
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
                  new Claim("tid", "org1"),
              }
          },
          new TestUser
          {
              SubjectId = Guid.NewGuid().ToString(),
              Username = "TestUser2",
              Password = "password",
              Claims = new List<Claim>
              {
                  new Claim("tid", "org2")
              }
          }
        };
    }
}