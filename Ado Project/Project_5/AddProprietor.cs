using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project_5
{
    public partial class AddProprietor : Form
    {
        List<Proprietor> Proprietor = new List<Proprietor>();
        public AddProprietor()
        {
            InitializeComponent();
        }
        public ICrossDataSyncM FormReloading { get; set; }
        public ICrossDataSyncM FormToSysnc { get; set; }

        private void AddProprietor_Load(object sender, EventArgs e)
        {
            this.textBox1.Text = this.GetNewProprietorId().ToString();
        }

        private object GetNewProprietorId()
        {
            using (SqlConnection con = new SqlConnection(ConnectionHP.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(ProprietorId), 0) FROM Proprietor", con))
                {
                    con.Open();
                    int id = (int)cmd.ExecuteScalar();
                    con.Close();
                    return id + 1;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(ConnectionHP.ConnectionString))
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {

                    using (SqlCommand cmd = new SqlCommand(@"INSERT INTO Proprietor 
                                            (ProprietorId, ProprietorName, Phone,GroceryId) VALUES
                                            (@pi, @pn, @ph, @gi)", con, tran))
                    {
                        cmd.Parameters.AddWithValue("@pi", int.Parse(textBox1.Text));
                        cmd.Parameters.AddWithValue("@pn", textBox2.Text);
                        cmd.Parameters.AddWithValue("@ph", textBox3.Text);
                        cmd.Parameters.AddWithValue("@gi", int.Parse(textBox4.Text));
                        
                        try
                        {
                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Data Saved", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                Proprietor.Add(new Proprietor
                                {
                                    ProprietorId = int.Parse(textBox1.Text),
                                    ProprietorName = textBox2.Text,
                                    Phone = textBox3.Text,
                                    GroceryId = int.Parse(textBox4.Text),
                                }); ;
                                tran.Commit();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error: {ex.Message}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            tran.Rollback();
                        }
                        finally
                        {
                            if (con.State == ConnectionState.Open)
                            {
                                con.Close();
                            }
                        }

                    }

                }

            }
        }

        private void AddProprietor_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.FormToSysnc.ReloadProprietor(this.Proprietor);
        }


    }
}
