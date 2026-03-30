using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace P_001
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void masterDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmMasterDetails frmMasterDetails = new frmMasterDetails();
            frmMasterDetails.Show();
            
        }

        private void sPCrudToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmPatient_SP es = new frmPatient_SP();
            es.Show();
           
        }

        private void showDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmShowData frmShowData = new frmShowData();
            frmShowData.Show();

        }

        private void designationEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmDesignationEntry frmDesignationEntry = new frmDesignationEntry();
            frmDesignationEntry.Show();
        }

        private void doctorsInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmDoctorBasicInformationReport frm= new frmDoctorBasicInformationReport();
            frm.Show();
        }

        private void designationWiseDoctorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmDesignationWiseDoctorDatail frmDesignationWiseDoctorDatail=new frmDesignationWiseDoctorDatail();
            frmDesignationWiseDoctorDatail.Show();
        }
    }
}
