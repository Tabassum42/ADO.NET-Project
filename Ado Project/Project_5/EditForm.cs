using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project_5
{
    public partial class EditForm : Form
    {
        string filePath, oldFile, fileName;
        string action = "Edit";
        Grocery grocery;
        public EditForm()
        {
            InitializeComponent();
        }
        public int GroceryToEditDelete { get; set; }
        public ICrossDataSyncM FormReload { get; set; }

        private void button1_Click(object sender, EventArgs e)
        {
            this.action = "Edit";
            using (SqlConnection con = new SqlConnection(ConnectionHP.ConnectionString))
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {

                    using (SqlCommand cmd = new SqlCommand(@"UPDATE  Grocery  
                                            SET  ProductName=@n, Quantity=@q, ManufactureDate= @m, Picture=@pi, Price=@p,Available=@a
                                            WHERE GroceryId=@i", con, tran))
                    {
                        cmd.Parameters.AddWithValue("@i", int.Parse(textBox1.Text));
                        cmd.Parameters.AddWithValue("@n", textBox2.Text);
                        cmd.Parameters.AddWithValue("@q", textBox3.Text);
                        cmd.Parameters.AddWithValue("@m", dateTimePicker1.Value);
                        cmd.Parameters.AddWithValue("@a", checkBox1.Checked);
                        cmd.Parameters.AddWithValue("@p", decimal.Parse(textBox4.Text));
                        if (!string.IsNullOrEmpty(this.filePath))
                        {
                            string ext = Path.GetExtension(this.filePath);
                            fileName = $"{Guid.NewGuid()}{ext}";
                            string savePath = Path.Combine(Path.GetFullPath(@"..\..\Pictures"), fileName);
                            File.Copy(filePath, savePath, true);
                            cmd.Parameters.AddWithValue("@pi", fileName);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@pi", oldFile);
                        }


                        try
                        {
                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Data Saved", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                grocery = new Grocery
                                {
                                    GroceryId = int.Parse(textBox1.Text),
                                    ProductName = textBox2.Text,
                                    Quantity = textBox3.Text,
                                    ManufactureDate = dateTimePicker1.Value,
                                    Price= decimal.Parse(textBox4.Text),
                                    Available = checkBox1.Checked,
                                    Picture = filePath == "" ? oldFile : fileName
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

        private void button4_Click(object sender, EventArgs e)
        {
            this.action = "Delete";
            using (SqlConnection con = new SqlConnection(ConnectionHP.ConnectionString))
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {

                    using (SqlCommand cmd = new SqlCommand(@"DELETE  Grocery  
                                            WHERE GroceryId=@i", con, tran))
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

        private void button3_Click(object sender, EventArgs e)
        {
            ShowData();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.filePath = this.openFileDialog1.FileName;
                this.label7.Text = Path.GetFileName(this.filePath);
                this.pictureBox1.Image = Image.FromFile(this.filePath);
            }
        }

        private void EditForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.action == "edit")
                this.FormReload.UpdateGrocery(grocery);
            else
                this.FormReload.RemoveGrocery(Int32.Parse(this.textBox1.Text));
        }

        private void EditForm_Load(object sender, EventArgs e)
        {
            ShowData();
        }
        private void ShowData()
        {
            using (SqlConnection con = new SqlConnection(ConnectionHP.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Grocery WHERE GroceryId =@i", con))
                {
                    cmd.Parameters.AddWithValue("@i", this.GroceryToEditDelete);
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        textBox1.Text = dr.GetInt32(0).ToString();
                        textBox2.Text = dr.GetString(1);
                        textBox3.Text = dr.GetString(2);
                        dateTimePicker1.Value = dr.GetDateTime(3);
                        textBox4.Text = dr.GetDecimal(5).ToString("0.00");
                        checkBox1.Checked = dr.GetBoolean(6);

                        oldFile = dr.GetString(4).ToString();
                        pictureBox1.Image = Image.FromFile(Path.Combine(@"..\..\Pictures", dr.GetString(4).ToString()));
                    }
                    con.Close();
                }
            }

        }
    }
}
