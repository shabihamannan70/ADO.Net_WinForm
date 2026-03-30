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

namespace P_001
{
    public partial class frmPatient_SP : Form
    {
        string conStr = "Data Source=.;Initial Catalog=projectdb;Integrated Security=true;MultipleActiveResultSets=true; TrustServerCertificate=True;";

        SqlConnection sqlCon;
        SqlCommand sqlCmd;
        string patientId = "";

        public frmPatient_SP()
        {
            InitializeComponent();
            sqlCon = new SqlConnection(conStr);
            sqlCon.Open();
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPatientName.Text))
            {
                MessageBox.Show("Enter emp name!!!");
                txtPatientName.Select();
            }
            else if (cmbCity.SelectedIndex <= -1)
            {
                MessageBox.Show("Select city!!!");
                cmbCity.Select();
            }
            else if (string.IsNullOrWhiteSpace(txtContact.Text))
            {
                MessageBox.Show("Enter emp name!!!");
                txtContact.Select();
            }
            else if (cmbGender.SelectedIndex <= -1)
            {
                MessageBox.Show("Select Gender!!!");
                cmbGender.Select();
            }
            else
            {
                try
                {
                    if (sqlCon.State == ConnectionState.Closed)
                    {
                        sqlCon.Open();
                    }
                    DataTable dt = new DataTable();
                    sqlCmd = new SqlCommand("spPatient", sqlCon);
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("@actionType", "SaveData");
                    sqlCmd.Parameters.AddWithValue("@patientId", patientId);
                    sqlCmd.Parameters.AddWithValue("@name", txtPatientName.Text);
                    sqlCmd.Parameters.AddWithValue("@cityId", cmbCity.SelectedValue);
                    sqlCmd.Parameters.AddWithValue("@contact", txtContact.Text);
                    sqlCmd.Parameters.AddWithValue("@gender", cmbGender.Text);
                    int numRes = sqlCmd.ExecuteNonQuery();
                    if (numRes > 0)
                    {
                        MessageBox.Show("Data saved successfully!!!", "Success");
                        dataGridView1.DataSource = FetchPatientDetails();
                        AllClear();

                    }
                    else
                    {
                        MessageBox.Show("Please try again later!!!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error : " + ex.Message);
                }
            }
        }

        private void frmPatient_SP_Load(object sender, EventArgs e)
        {
            LoadCityCombo();
            dataGridView1.DataSource = FetchPatientDetails();
        }
        private void LoadCityCombo()
        {
            SqlDataAdapter sda = new SqlDataAdapter("SELECT id,cityName FROM cities", sqlCon);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            cmbCity.DataSource = dt;
            cmbCity.ValueMember = "id";
            cmbCity.DisplayMember = "cityName";


        }
        private DataTable FetchPatientDetails()
        {
            if (sqlCon.State == ConnectionState.Closed)
            {
                sqlCon.Open();
            }
            DataTable dt = new DataTable();
            sqlCmd = new SqlCommand("spPatient", sqlCon);
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.Parameters.AddWithValue("@actionType", "FetchData");
            SqlDataAdapter sda = new SqlDataAdapter(sqlCmd);
            sda.Fill(dt);
            return dt;
        }

        private DataTable FetchEmpRecord(string employeeId)
        {
            if (sqlCon.State == ConnectionState.Closed)
            {
                sqlCon.Open();
            }
            DataTable dt = new DataTable();
            sqlCmd = new SqlCommand("spPatient", sqlCon);
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.Parameters.AddWithValue("@actionType", "FetchRecord");
            sqlCmd.Parameters.AddWithValue("@patientId", patientId);
            SqlDataAdapter sda = new SqlDataAdapter(sqlCmd);
            sda.Fill(dt);
            return dt;

        }
        private void AllClear()
        {
            txtPatientName.Clear();
            cmbCity.Text = "";
            txtContact.Text = "";
            cmbGender.SelectedIndex = -1;
            patientId = "";
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = FetchPatientDetails();
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            AllClear();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                btnSave.Text = "Update";
                patientId = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                DataTable dt = FetchEmpRecord(patientId);
                if (dt.Rows.Count > 0)
                {
                    patientId = dt.Rows[0][0].ToString();
                    txtPatientName.Text = dt.Rows[0][1].ToString();
                    cmbCity.Text = dt.Rows[0][3].ToString();
                    txtContact.Text = dt.Rows[0][4].ToString();
                    cmbGender.Text = dt.Rows[0][2].ToString();
                }
                else
                {
                    AllClear();
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(patientId))
            {
                try
                {
                    if (sqlCon.State == ConnectionState.Closed)
                    {
                        sqlCon.Open();
                    }
                    DataTable dt = new DataTable();
                    sqlCmd = new SqlCommand("spPatient", sqlCon);
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("@actionType", "DeleteData");
                    sqlCmd.Parameters.AddWithValue("@patientId", patientId);
                    int numRes = sqlCmd.ExecuteNonQuery();
                    if (numRes > 0)
                    {
                        MessageBox.Show("Data delete successfully!!!", "Success");
                        dataGridView1.DataSource = FetchPatientDetails();
                        AllClear();
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }
    }
}
