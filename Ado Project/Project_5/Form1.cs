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
    public partial class Form1 : Form,ICrossDataSyncM
    {
        DataSet ds;
        BindingSource gsGrocery = new BindingSource();
        BindingSource gsProprietor = new BindingSource();
        public Form1()
        {
            InitializeComponent();
        }
        public void LoadData()
        {
            ds = new DataSet();
            using (SqlConnection con = new SqlConnection(ConnectionHP.ConnectionString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Grocery", con))
                {
                    da.Fill(ds, "Grocery");
                    ds.Tables["Grocery"].Columns.Add(new DataColumn("image", typeof(System.Byte[])));
                    for (var i = 0; i < ds.Tables["Grocery"].Rows.Count; i++)
                    {
                        ds.Tables["Grocery"].Rows[i]["image"] = File.ReadAllBytes(Path.Combine(Path.GetFullPath(@"..\..\Pictures"), ds.Tables["Grocery"].Rows[i]["Picture"].ToString()));
                    }
                    da.SelectCommand.CommandText = "SELECT * FROM Proprietor";
                    da.Fill(ds, "Proprietor");
                    DataRelation rel = new DataRelation("FK_BOOK_GROCERY",
                        ds.Tables["Grocery"].Columns["GroceryId"],
                        ds.Tables["Proprietor"].Columns["GroceryId"]);
                    ds.Relations.Add(rel);
                    ds.AcceptChanges();
                }
            }
        }
        private void BindData()
        {
            gsGrocery.DataSource = ds;
            gsGrocery.DataMember = "Grocery";
            gsProprietor.DataSource = gsGrocery;
            gsProprietor.DataMember = "FK_BOOK_GROCERY";
            this.dataGridView1.DataSource = gsProprietor;
            lbl1.DataBindings.Add(new Binding("Text", gsGrocery, "ProductName"));
            lbl2.DataBindings.Add(new Binding("Text", gsGrocery, "Quantity"));
            lbl3.DataBindings.Add(new Binding("Text", gsGrocery, "ManufactureDate"));
            lbl4.DataBindings.Add(new Binding("Text", gsGrocery, "Price"));
            checkBox1.DataBindings.Add(new Binding("Checked", gsGrocery, "Available"));
            pictureBox1.DataBindings.Add(new Binding("Image", gsGrocery, "image", true));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadData();
            BindData();
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AddGrocery() { FormReloading = this }.ShowDialog();
        }

        public void ReloadData(List<Grocery> grocerys)
        {
            foreach (var g in grocerys)
            {
                DataRow dr = ds.Tables["Grocery"].NewRow();
                dr[0] = g.GroceryId;
                dr["ProductName"] = g.ProductName;
                dr["Quantity"] = g.Quantity;
                dr["ManufactureDate"] = g.ManufactureDate;
                dr["Price"] = g.Price;
                dr["available"] = g.Available;
                dr["Picture"] = g.Picture;
                dr["image"] = File.ReadAllBytes(Path.Combine(Path.GetFullPath(@"..\..\Pictures"), g.Picture));
                ds.Tables["Grocery"].Rows.Add(dr);

            }
            ds.AcceptChanges();
            gsGrocery.MoveLast();
        }

        public void UpdateGrocery(Grocery g)
        {
            for (var i = 0; i < ds.Tables["Grocery"].Rows.Count; i++)
            {
                if ((int)ds.Tables["Grocery"].Rows[i]["GroceryId"] == g.GroceryId)
                {
                    ds.Tables["Grocery"].Rows[i]["ProductName"] = g.ProductName;
                    ds.Tables["Grocery"].Rows[i]["Quantity"] = g.Quantity;
                    ds.Tables["Grocery"].Rows[i]["ManufactureDate"] = g.ManufactureDate;
                    ds.Tables["Grocery"].Rows[i]["Price"] = g.Price;
                    ds.Tables["Grocery"].Rows[i]["Available"] = g.Available;
                    ds.Tables["Grocery"].Rows[i]["image"] = File.ReadAllBytes(Path.Combine(Path.GetFullPath(@"..\..\Pictures"), g.Picture));
                    break;
                }
            }
            ds.AcceptChanges();
        }

        public void RemoveGrocery(int id)
        {
            for (var i = 0; i < ds.Tables["Grocery"].Rows.Count; i++)
            {
                if ((int)ds.Tables["Grocery"].Rows[i]["GroceryId"] == id)
                {
                    ds.Tables["Grocery"].Rows.RemoveAt(i);
                    break;
                }
            }
            ds.AcceptChanges();
        }

        private void editDeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int id = (int)(this.gsGrocery.Current as DataRowView).Row[0];
            new EditForm { GroceryToEditDelete = id, FormReload = this }.ShowDialog();
        }




        private void groceryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new GroceryFormRpt().ShowDialog();
        }

       
        private void addToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            new AddProprietor() { FormReloading = this }.ShowDialog();
        }

        public void ReloadProprietor(List<Proprietor> Proprietor)
        {
            foreach (var p in Proprietor)
            {
                DataRow dr = ds.Tables["Proprietor"].NewRow();
                dr[0] = p.ProprietorId;
                dr["ProprietorName"] = p.ProprietorName;
                dr["Phone"] = p.Phone;
                dr["GroceryId"] = p.GroceryId;
                ds.Tables["proprietor"].Rows.Add(dr);

            }
            ds.AcceptChanges();
            gsGrocery.MoveLast();
        }

        public void UpdateProprietor(Proprietor p)
        {
            for (var n = 0; n < ds.Tables["proprietor"].Rows.Count; n++)
            {
                if ((int)ds.Tables["proprietor"].Rows[n]["ProprietorId"] == p.ProprietorId)
                {
                    ds.Tables["Grocery"].Rows[n]["ProprietorName"] = p.ProprietorName;
                    ds.Tables["Grocery"].Rows[n]["Phone"] = p.Phone;
                    ds.Tables["Grocery"].Rows[n]["GroceryId"] = p.GroceryId;
                    break;
                }
            }
            ds.AcceptChanges();
        }

        public void RemoveProprietor(int id)
        {
            for (var n = 0; n < ds.Tables["proprietor"].Rows.Count; n++)
            {
                if ((int)ds.Tables["proprietor"].Rows[n]["ProprietorId"] == id)
                {
                    ds.Tables["proprietor"].Rows.RemoveAt(n);
                    break;
                }
            }
            ds.AcceptChanges();
        }

        private void editDeteteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int id = (int)(this.gsProprietor.Current as DataRowView).Row[0];
            new EditDeleteProprietor { EditDeleteproprietor = id, FormToReloaded = this }.ShowDialog();
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            gsGrocery.MoveFirst();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            gsGrocery.MovePrevious();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            gsGrocery.MoveNext();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            gsGrocery.MoveLast();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            new AddGrocery() { FormReloading = this }.ShowDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void proprietorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new GroceryGroupRpt().ShowDialog();
        }
    }
}
