using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Poda.Shared;
using System.Data;

namespace Nanook.Entities
{
    public class Member
    {
        public Guid ID { get; set; }
        public string Email { get; set; }
    }

    internal class MemberEntityConverter : IEntityConverter<Member>
    {
        public Member Convert(IDataRecord reader)
        {
            return new Member()
            {
                ID = (Guid)reader["ID"],
                Email = (string)reader["Email"]
            };
        }
    }
}