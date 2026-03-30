using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace P_001
{
    public partial class frmMasterDetails : Form
    {
        List<SpecialityDetails> details = new List<SpecialityDetails>();
        string currentFile = "";
        public Form1 TheForm { get; set; }
        public frmMasterDetails()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            details.Add(new SpecialityDetails
            {
                specialities = txtSpecialities.Text,
                description = txtDescription.Text,
                fee= Convert.ToDecimal(txtfee.Text)
            });
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = details;

        }

        private void frmMasterDetails_Load(object sender, EventArgs e)
        {
            dataGridView1.AutoGenerateColumns = false;
            LoadCombo();
        }

        private void LoadCombo()
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
            {
                SqlDataAdapter sda = new SqlDataAdapter("SELECT Id,designationName FROM designation ", con);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                cmbDesignation.DataSource = dt;
                cmbDesignation.ValueMember = "Id";
                cmbDesignation.DisplayMember = "designationName";
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
           
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
            {
                con.Open();
                using (SqlTransaction trx = con.BeginTransaction())
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.Transaction = trx;
                        string ext = Path.GetExtension(currentFile);
                        string f = Path.GetFileNameWithoutExtension(Guid.NewGuid().ToString()) + ext;
                        string savePath = @"..\..\Images\" + f;
                        MemoryStream ms = new MemoryStream(File.ReadAllBytes(currentFile));
                        byte[] bytes = ms.ToArray();
                        FileStream fs = new FileStream(savePath, FileMode.Create);
                        fs.Write(bytes, 0, bytes.Length);
                        fs.Close();

                        cmd.CommandText = "INSERT INTO Doctors(doctorName,email,contactNumber,designationId,chamberInDhaka,picture,photo) VALUES(@doctorName,@email,@contactNumber,@designationId,@chamberInDhaka,@picture,@photo); SELECT SCOPE_IDENTITY();";
                        cmd.Parameters.AddWithValue("@doctorName", txtDoctorName.Text);
                        cmd.Parameters.AddWithValue("@email", txtEmail.Text);
                        cmd.Parameters.AddWithValue("@contactNumber", txtPhone.Text);
                        cmd.Parameters.AddWithValue("@designationId", cmbDesignation.SelectedValue);
                        cmd.Parameters.AddWithValue("@chamberInDhaka", chkInDhaka.Checked);
                        cmd.Parameters.AddWithValue("@picture", f);
                        MemoryStream ms2 = new MemoryStream();
                        pictureBox1.Image.Save(ms2, pictureBox1.Image.RawFormat);
                        cmd.Parameters.AddWithValue("@photo", ms2.ToArray());

                        try
                        {
                            var dId = cmd.ExecuteScalar();
                            foreach (var d in details)
                            {
                                cmd.CommandText = "INSERT INTO Specialities(doctorId,specialities,description,fee) VALUES(@doctorId,@specialities,@description,@fee)";
                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("@doctorId", dId);
                                cmd.Parameters.AddWithValue("@specialities", d.specialities);
                                cmd.Parameters.AddWithValue("@description", d.description);
                                cmd.Parameters.AddWithValue("@fee", d.fee);
                                cmd.ExecuteNonQuery();
                            }
                            trx.Commit();
                            MessageBox.Show("Data saved successfully", "Success");

                        }
                        catch (Exception ex)
                        {
                            trx.Rollback();
                            MessageBox.Show("" + ex);
                        }
                    }
                }
                con.Close();
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        public class SpecialityDetails
        {
            public string specialities { get; set; }
            public string description { get; set; }
            public decimal fee { get; set; }
        }

        private void btnBrowse_Click_1(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.pictureBox1.Image = Image.FromFile(openFileDialog1.FileName);
                currentFile = openFileDialog1.FileName;
            }
        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 3)
            {
                details.RemoveAt(e.RowIndex);
                dataGridView1.DataSource = null;
                dataGridView1.DataSource = details;
            }
        }
    }
}
