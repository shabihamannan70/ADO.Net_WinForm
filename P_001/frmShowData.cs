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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace P_001
{
    public partial class frmShowData : Form
    {
        BindingSource bsS = new BindingSource();
        BindingSource bsC = new BindingSource();
        DataSet ds;
        public frmShowData()
        {
            InitializeComponent();
        }

        private void frmShowData_Load(object sender, EventArgs e)
        {
            LoadDataBindingSource();
        }

        public void LoadDataBindingSource()
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter("SELECT d.*,s.designationName FROM Doctors d INNER JOIN designation s ON d.designationId=s.Id", con))
                {
                    ds = new DataSet();
                    sda.Fill(ds, "Doctors");
                    sda.SelectCommand.CommandText = "SELECT doctorId,specialities,description,fee FROM Specialities";
                    sda.Fill(ds, "Specialities");

                    ds.Tables["Doctors"].Columns.Add(new DataColumn("image", typeof(byte[])));
                    for (int i = 0; i < ds.Tables["Doctors"].Rows.Count; i++)
                    {
                        ds.Tables["Doctors"].Rows[i]["image"] = File.ReadAllBytes($@"..\..\Images\{ds.Tables["Doctors"].Rows[i]["picture"]}");
                    }
                    DataRelation rel = new DataRelation("FK_S_S", ds.Tables["Doctors"].Columns["doctorId"], ds.Tables["Specialities"].Columns["doctorId"]);
                    ds.Relations.Add(rel);
                    bsS.DataSource = ds;
                    bsS.DataMember = "Doctors";

                    bsC.DataSource = bsS;
                    bsC.DataMember = "FK_S_S";
                    dataGridView1.DataSource = bsC;
                    AddDataBindings();
                }
            }
        }

        private void AddDataBindings()
        {
            lblDoctorId.DataBindings.Clear();
            lblDoctorId.DataBindings.Add("Text", bsS, "doctorId");

            lblDoctorsName.DataBindings.Clear();
            lblDoctorsName.DataBindings.Add("Text", bsS, "doctorName");

            lblEmail.DataBindings.Clear();
            lblEmail.DataBindings.Add("Text", bsS, "email");

            lblMobile.DataBindings.Clear();
            lblMobile.DataBindings.Add("Text", bsS, "contactNumber");

            lblDesignation.DataBindings.Clear();
            lblDesignation.DataBindings.Add("Text", bsS, "designationName");

            pictureBox1.DataBindings.Clear();
            pictureBox1.DataBindings.Add(new Binding("Image", bsS, "image", true));

            chkInDhaka.DataBindings.Clear();
            chkInDhaka.DataBindings.Add("Checked", bsS, "chamberInDhaka", true);

           
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            int v = int.Parse((bsS.Current as DataRowView).Row[0].ToString());
            new frmEdit { TheForm = this, IdToEdit = v }.ShowDialog();
        }

        private void btnFirst_Click(object sender, EventArgs e)
        {
            bsS.MoveFirst();
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (bsS.Position > 0)
            {
                bsS.MovePrevious();
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (bsS.Position < bsS.Count - 1)
            {
                bsS.MoveNext();
            }
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            bsS.MoveLast();
        }
    }
}
