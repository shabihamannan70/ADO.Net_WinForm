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
using static P_001.frmMasterDetails;

namespace P_001
{
    public partial class frmEdit : Form
    {
        List<SpecialityDetails> details = new List<SpecialityDetails>();
        string currentFile = "";
        string oldFile = "";
        public frmEdit()
        {
            InitializeComponent();
        }
        public frmShowData TheForm { get; set; }
        public int IdToEdit { get; set; }

        private void frmEdit_Load(object sender, EventArgs e)
        {
            dataGridView1.AutoGenerateColumns = false;
            LoadCombo();
            LoadInForm();
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

        private void LoadInForm()
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Doctors WHERE doctorId=@i", con))
                {
                    cmd.Parameters.AddWithValue("@i", IdToEdit);
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        txtDoctorName.Text = dr.GetString(1);
                        txtEmail.Text = dr.GetString(2);
                        txtPhone.Text = dr.GetString(3);
                        cmbDesignation.SelectedValue = dr.GetInt32(4);
                        chkInDhaka.Checked = dr.GetBoolean(5);
                        pictureBox1.Image = Image.FromFile(@"..\..\Images\" + dr.GetString(6));
                        oldFile = dr.GetString(6);
                        

                    }
                    dr.Close();
                    cmd.CommandText = "SELECT * FROM Specialities WHERE doctorId=@i";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@i", IdToEdit);
                    SqlDataReader dr2 = cmd.ExecuteReader();
                    while (dr2.Read())
                    {
                        details.Add(new SpecialityDetails
                        {
                            specialities = dr2.GetString(2),
                            description = dr2.GetString(3),
                            fee = dr2.GetDecimal(4)
                        });
                    }
                    SetDataSource();
                    con.Close();
                }
            }
        }
        private void SetDataSource()
        {
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = details;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.pictureBox1.Image = Image.FromFile(openFileDialog1.FileName);
                currentFile = openFileDialog1.FileName;
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 3)
            {
                details.RemoveAt(e.RowIndex);
                dataGridView1.DataSource = null;
                dataGridView1.DataSource = details;
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
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
                        string f=oldFile;
                        if(currentFile != "")
                        {
                            string ext = Path.GetExtension(currentFile);
                            f = Path.GetFileNameWithoutExtension(Guid.NewGuid().ToString()) + ext;
                            string savePath = @"..\..\Images\" + f;
                            MemoryStream ms = new MemoryStream(File.ReadAllBytes(currentFile));
                            byte[] bytes = ms.ToArray();
                            FileStream fs = new FileStream(savePath, FileMode.Create);
                            fs.Write(bytes, 0, bytes.Length);
                            fs.Close();
                        }

                        cmd.CommandText = "UPDATE Doctors SET doctorName=@doctorName,email=@email,contactNumber=@contactNumber,designationId=@designationId,chamberInDhaka=@chamberInDhaka,picture=@picture,photo=@photo WHERE doctorId=@id";

                        cmd.Parameters.AddWithValue("@id", IdToEdit);
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
                            cmd.ExecuteNonQuery();
                            cmd.CommandText = "DELETE FROM Specialities WHERE doctorId=@id";
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@id", IdToEdit);
                            cmd.ExecuteNonQuery();
                            
                            foreach (var d in details)
                            {
                                cmd.CommandText = "INSERT INTO Specialities(doctorId,specialities,description,fee) VALUES(@doctorId,@specialities,@description,@fee)";
                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("@doctorId", IdToEdit);
                                cmd.Parameters.AddWithValue("@specialities", d.specialities);
                                cmd.Parameters.AddWithValue("@description", d.description);
                                cmd.Parameters.AddWithValue("@fee", d.fee);
                                cmd.ExecuteNonQuery();
                            }
                            trx.Commit();
                            MessageBox.Show("Data updated successfully", "Success");
                            TheForm.LoadDataBindingSource();

                        }
                        catch (Exception ex)
                        {
                            trx.Rollback();
                            TheForm.LoadDataBindingSource();
                            MessageBox.Show("" + ex);
                        }
                    }
                }
                con.Close();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            details.Add(new SpecialityDetails
            {
                specialities = txtSpecialities.Text,
                description = txtDescription.Text,
                fee = Convert.ToDecimal(txtfee.Text)
            });
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = details;
        }

        private void btnDeleteAll_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
            {
                con.Open();
                using (SqlTransaction trx = con.BeginTransaction())
                {

                    string sql = "DELETE FROM Specialities WHERE doctorId=@id";
                    using (SqlCommand cmd = new SqlCommand(sql, con, trx))
                    {
                        cmd.Parameters.AddWithValue("@id", IdToEdit);
                        try
                        {
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                            cmd.CommandText = "DELETE FROM Doctors WHERE doctorId=@id";
                            cmd.Parameters.AddWithValue("@id", IdToEdit);
                            cmd.ExecuteNonQuery();
                            trx.Commit();
                            MessageBox.Show("Data deleted successfully!!!", "Success");
                            TheForm.LoadDataBindingSource();
                            this.Close();
                        }
                        catch (Exception)
                        {
                            trx.Rollback();
                            MessageBox.Show("Failed to delete", "Error");
                        }
                    }
                    con.Close();
                }
            }
        }
    }
}

