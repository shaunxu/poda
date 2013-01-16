using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Poda.VersionProviders;
using System.Security.Cryptography;

namespace Poda.ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            (new PodaSearch()).ShowDialog();

            //RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            //// STEP 0: Initialize the PODA cofiguration
            //Poda.Factory.Config(
            //    new Poda.Configuration.ConfigurationFileProvider.ConfigurationFileProvider(),
            //    connectionString => new SqlConnection(connectionString));
            //var ver = Poda.Factory.CurrentVersion;

            //// Insert members and questions (2 questions per member)
            //using (var poda = Poda.Factory.Create())
            //{
            //    for (int i = 0; i < 10000; i++)
            //    {
            //        var memberId = Guid.NewGuid();
            //        var memberKey = memberId.GetKeyWithVersion(ver);
            //        var memberName = string.Format("Member:{0}", i);
            //        poda.Execute()
            //            .ForPlainSQL("INSERT INTO Members VALUES (@ID, @Name)")
            //            .With("ID", memberKey)
            //            .With("Name", memberName)
            //            .FederationOn("Member", "ID", memberKey)
            //            .AsNothing();
            //        for (int j = 0; j < 2; j++)
            //        {
            //            var questionId = Guid.NewGuid();
            //            var questionMemberId = memberKey;
            //            var keyBytes = new byte[32];
            //            rng.GetBytes(keyBytes);
            //            var question = Convert.ToBase64String(keyBytes);
            //            poda.Execute()
            //                .ForPlainSQL("INSERT INTO MemberQuestions VALUES (@ID, @MemberID, @Question)")
            //                .With("ID", questionId)
            //                .With("MemberID", memberKey)
            //                .With("Question", question)
            //                .FederationOn("MemberQuestions", "MemberID", memberKey)
            //                .AsNothing();
            //        }
            //    }
            //    poda.Commit();
            //}

            Console.WriteLine("Done!");
            Console.ReadKey();
        }
    }
}
