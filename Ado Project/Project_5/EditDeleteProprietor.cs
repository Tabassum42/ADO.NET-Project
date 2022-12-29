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
    public partial class EditDeleteProprietor : Form
    {
        string action = "Edit";
        Proprietor proprietor;
        public EditDeleteProprietor()
        {
            InitializeComponent();
        }
        public int EditDeleteproprietor { get; set; }
        public ICrossDataSyncM FormToReloaded { get; set; }
        private void EditDeleteProprietor_Load(object sender, EventArgs e)
        {
            ShowData();
        }
        private void ShowData()
        {
            using (SqlConnection con = new SqlConnection(ConnectionHP.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Proprietor WHERE Proprietorid =@i", con))
                {
                    cmd.Parameters.AddWithValue("@i", this.EditDeleteproprietor);
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        textBox1.Text = dr.GetInt32(0).ToString();
                        textBox2.Text = dr.GetString(1);
                        textBox3.Text = dr.GetString(2);
                        textBox4.Text = dr.GetInt32(3).ToString();
                    }
                    con.Close();
                }
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.action = "Edit";
            using (SqlConnection con = new SqlConnection(ConnectionHP.ConnectionString))
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {

                    using (SqlCommand cmd = new SqlCommand(@"UPDATE  Proprietor  
                                            SET  ProprietorName=@n, Phone=@p, GroceryId=@gi 
                                            WHERE ProprietorId=@i", con, tran))
                    {
                        cmd.Parameters.AddWithValue("@i", int.Parse(textBox1.Text));
                        cmd.Parameters.AddWithValue("@n", textBox2.Text);
                        cmd.Parameters.AddWithValue("@p", textBox3.Text);
                        cmd.Parameters.AddWithValue("@gi", int.Parse(textBox4.Text));
                        try
                        {
                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Data Saved", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                proprietor = new Proprietor
                                {
                                    ProprietorId = int.Parse(textBox1.Text),
                                    ProprietorName = textBox2.Text,
                                    Phone = textBox3.Text,
                                    GroceryId = int.Parse(textBox4.Text),
                                };
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

        private void button3_Click_1(object sender, EventArgs e)
        {
            ShowData();
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            this.action = "Delete";
            using (SqlConnection con = new SqlConnection(ConnectionHP.ConnectionString))
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {

                    using (SqlCommand cmd = new SqlCommand(@"DELETE  Proprietor  
                                            WHERE ProprietorId=@i", con, tran))
                    {
                        cmd.Parameters.AddWithValue("@i", int.Parse(textBox1.Text));
                        try
                        {
                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Data Saved", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

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

        private void EditDeleteProprietor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.action == "edit")
                this.FormToReloaded.UpdateProprietor(proprietor);
            else
                this.FormToReloaded.RemoveProprietor(Int32.Parse(this.textBox1.Text));
        }
    }

}

    

