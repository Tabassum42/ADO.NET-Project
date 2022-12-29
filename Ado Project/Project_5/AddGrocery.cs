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
    public partial class AddGrocery : Form
    {
        string filePath = "";
        List<Grocery> grocery = new List<Grocery>();
        public AddGrocery()
        {
            InitializeComponent();
        }
        public ICrossDataSyncM FormReloading { get; set; }
        private void AddGrocery_Load(object sender, EventArgs e)
        {
            this.textBox1.Text = this.GetNewGroceryId().ToString();
        }
        private int GetNewGroceryId()
        {
            using (SqlConnection con = new SqlConnection(ConnectionHP.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(GroceryId), 0) FROM Grocery", con))
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

                    using (SqlCommand cmd = new SqlCommand(@"INSERT INTO Grocery 
                                            (GroceryId, ProductName, Quantity, ManufactureDate, Picture, Price,Available) VALUES
                                            (@i, @n, @q, @m, @pi, @p,@a)", con, tran))
                    {
                        cmd.Parameters.AddWithValue("@i", int.Parse(textBox1.Text));
                        cmd.Parameters.AddWithValue("@n", textBox2.Text);
                        cmd.Parameters.AddWithValue("@q", textBox3.Text);
                        cmd.Parameters.AddWithValue("@m", dateTimePicker1.Value);

                        cmd.Parameters.AddWithValue("@p", decimal.Parse(textBox4.Text));

                        cmd.Parameters.AddWithValue("@a", checkBox1.Checked);
                        string ext = Path.GetExtension(this.filePath);
                        string fileName = $"{Guid.NewGuid()}{ext}";
                        string savePath = Path.Combine(Path.GetFullPath(@"..\..\Pictures"), fileName);
                        File.Copy(filePath, savePath, true);
                        cmd.Parameters.AddWithValue("@pi", fileName);

                        try
                        {
                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Data Saved", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                grocery.Add(new Grocery
                                {
                                    GroceryId = int.Parse(textBox1.Text),
                                    ProductName = textBox2.Text,
                                    Quantity = textBox3.Text,
                                    ManufactureDate = dateTimePicker1.Value,
                                    Price = decimal.Parse(textBox4.Text),
                                    Available = checkBox1.Checked,
                                    Picture = fileName
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

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.filePath = this.openFileDialog1.FileName;
                this.label7.Text = Path.GetFileName(this.filePath);
                this.pictureBox1.Image = Image.FromFile(this.filePath);
            }
        }

        private void AddGrocery_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.FormReloading.ReloadData(this.grocery);
        }
    }
    
}
