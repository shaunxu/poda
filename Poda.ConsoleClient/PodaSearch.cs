using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Poda.ConsoleClient
{
    public partial class PodaSearch : Form
    {
        public PodaSearch()
        {
            InitializeComponent();

            dataGridView1.AutoGenerateColumns = true;
            Poda.Factory.Config(
                new Poda.Configuration.ConfigurationFileProvider.ConfigurationFileProvider(),
                connectionString => new SqlConnection(connectionString));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var memberKey = textBox1.Text.Trim();
            if (string.IsNullOrWhiteSpace(memberKey))
            {
                // fan-out query
                var ds = new DataSet();

                var sw = new Stopwatch();
                sw.Start();
                using (var poda = Poda.Factory.Create())
                {
                    ds = poda.Execute()
                        .ForPlainSQL("SELECT * FROM Members LEFT JOIN MemberQuestions ON Members.ID = MemberQuestions.MemberID")
                        .FederationOnAll()
                        .AsDataSet();
                }
                sw.Stop();

                toolStripStatusLabel1.Text = string.Format("Fan-out Query: {0}. Count: {1}", sw.Elapsed.TotalMilliseconds / 1000, ds.Tables[0].Rows.Count);
                dataGridView1.DataSource = ds.Tables[0];
            }
            else
            {
                // sharding-specified query
                // fan-out query
                var ds = new DataSet();

                var sw = new Stopwatch();
                sw.Start();
                using (var poda = Poda.Factory.Create())
                {
                    ds = poda.Execute()
                        .ForPlainSQL("SELECT * FROM Members LEFT JOIN MemberQuestions ON Members.ID = MemberQuestions.MemberID WHERE Members.ID = @ID")
                        .With("ID", memberKey)
                        .FederationOn("Members", "ID", memberKey)
                        .AsDataSet();
                }
                sw.Stop();

                toolStripStatusLabel1.Text = string.Format("Specified Query: {0}.", sw.Elapsed.TotalMilliseconds / 1000);
                dataGridView1.DataSource = ds.Tables[0];
            }
        }
    }
}
