using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace ProcessMonitoring
{
    public partial class FrmMain : Form
    {
        DataTable dtProcess = new DataTable();
        DataTable dtThreads = new DataTable();
        public FrmMain()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            comboBoxProcessIdType.SelectedIndex = 0;
            dtProcess.Columns.Add("Process ID (Hex)", typeof(string));
            dtProcess.Columns.Add("Process ID (Dec)", typeof(string));
            dtProcess.Columns.Add("Process Name", typeof(string));
            dtProcess.Columns.Add("Action", typeof(string));
            dtProcess.Columns.Add("File Name", typeof(string));

            dgvProcess.DataSource = new BindingSource(dtProcess, string.Empty);
            dgvProcess.AllowUserToAddRows = false;
            dgvProcess.AllowUserToDeleteRows = false;
            dgvProcess.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProcess.ReadOnly = true;
            dgvProcess.Columns[0].Width = 150;
            dgvProcess.Columns[1].Width = 150;
            dgvProcess.Columns[2].Width = 150;
            dgvProcess.Columns[3].Width = 100;
            dgvProcess.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            dtThreads.Columns.Add("Thread ID (Hex)", typeof(string));
            dtThreads.Columns.Add("Thread ID (Dec)", typeof(string));
            dtThreads.Columns.Add("State", typeof(string));

            dgvThreads.DataSource = new BindingSource(dtThreads, string.Empty);
            dgvThreads.AllowUserToAddRows = false;
            dgvThreads.AllowUserToDeleteRows = false;
            dgvThreads.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvThreads.ReadOnly = true;

            dgvProcess.Columns[0].Width = 150;
            dgvProcess.Columns[1].Width = 150;
            dgvProcess.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void btnMonitoring_Click(object sender, EventArgs e)
        {

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                // 通过进程名查找
                var processList = Process.GetProcessesByName(txtSearchKey.Text);
                dtProcess.Clear();
                foreach (var process in processList)
                {
                    var dr = dtProcess.NewRow();
                    dr[0] = Convert.ToString(process.Id, 16).PadLeft(8, '0').ToUpper();
                    dr[1] = Convert.ToString(process.Id, 10);
                    dr[2] = process.ProcessName;
                    dr[3] = string.Empty;
                    dr[4] = process.MainModule.FileName;
                    dtProcess.Rows.Add(dr);
                }
                dtProcess.AcceptChanges();
            }
            else if (comboBox1.SelectedIndex == 1)
            {
                int processId = 0;

                // 通过进程ID查找
                if (comboBoxProcessIdType.SelectedIndex == 0)
                {
                    // 十进制
                    processId = Convert.ToInt32(txtSearchKey.Text);
                }
                else if (comboBoxProcessIdType.SelectedIndex == 1)
                {
                    // 十六进制
                    processId = Convert.ToInt32(txtSearchKey.Text, 16);
                }

                var process = Process.GetProcessById(processId);
                dtProcess.Clear();

                var dr = dtProcess.NewRow();
                dr[0] = Convert.ToString(process.Id, 16).PadLeft(8, '0').ToUpper();
                dr[1] = Convert.ToString(process.Id, 10);
                dr[2] = process.ProcessName;
                dr[3] = string.Empty;
                dr[4] = process.MainModule.FileName;
                dtProcess.Rows.Add(dr);

                dtProcess.AcceptChanges();
            }
            else
            {
                // 未定义
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                comboBoxProcessIdType.Visible = false;
            }
            else if (comboBox1.SelectedIndex == 1)
            {
                comboBoxProcessIdType.Visible = true;
            }
        }

        private void dgvProcess_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvProcess.CurrentCell == null)
            {
                dtThreads.Clear();
                dtThreads.AcceptChanges();
            }
            else
            {
                var process = Process.GetProcessById(Convert.ToInt32(dgvProcess.CurrentCell.OwningRow.Cells[1].Value));
                dtThreads.Clear();

                foreach (ProcessThread thread in process.Threads)
                {
                    var dr = dtThreads.NewRow();
                    dr[0] = Convert.ToString(thread.Id, 16).PadLeft(8, '0').ToUpper();
                    dr[1] = Convert.ToString(thread.Id, 10);
                    dr[2] = thread.ThreadState;
                    dtThreads.Rows.Add(dr);
                }
                dtThreads.AcceptChanges();
            }
        }
    }
}
