using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Drawing.Imaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using System.IO;

namespace WindowsFormsApp5
{
    public partial class Form1 : Form
    {
        const string DefaultAddress = @"d:\Pictures\image";
        private FilterInfoCollection webcam;
        private VideoCaptureDevice cam;
        private int Counter = 0;
        private int SelectionNow = 0;
        private int SelectionBefore = 0;
        public Form1()
        {
            InitializeComponent();
            setupDataGrid();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //WriteVideo();
            webcam = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo VideoCaptureDevice in webcam)
            {
                comboBox1.Items.Add(VideoCaptureDevice.Name);
            }
            comboBox1.SelectedItem = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex >= 0)
            {
                cam = new VideoCaptureDevice(webcam[comboBox1.SelectedIndex].MonikerString);
                cam.NewFrame += Cam_NewFrame;
                cam.Start();
            }
            else
            {
                MessageBox.Show("choose a webcam first");
            }
        }

        private void Cam_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bit = (Bitmap)eventArgs.Frame.Clone();
            pictureBox2.Image = bit;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (cam.IsRunning)
            {
                cam.Stop();
                pictureBox2.Image = Image.FromFile(DefaultAddress + 0 + ".jpg"); ;
                Counter = 0;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (pictureBox2.Image != null)
            {
                string fileName = DefaultAddress + Counter.ToString()+".jpg";
                pictureBox2.Image.Save(fileName);
                Counter += 1;
                addRowToGrid(fileName, Counter);
            }
            else
            {
                MessageBox.Show("Open cam First");
            }
        }
        private void setupDataGrid()
        {
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Navy;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font(dataGridView1.Font, FontStyle.Bold);
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            dataGridView1.GridColor = Color.Black;
            dataGridView1.RowHeadersVisible = true;
            dataGridView1.RowTemplate.Height = 120;
            dataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.ColumnCount = 4;
            dataGridView1.Columns[0].Name = "No.";
            dataGridView1.Columns[0].Width = 40;
            dataGridView1.Columns[1].Name = "time";
            dataGridView1.Columns[1].Width = 100;
            dataGridView1.Columns[2].Name = "guessing";
            dataGridView1.Columns[3].Name = "user log";
            dataGridView1.Columns[3].Width = 280;
            DataGridViewImageColumn imgc = new DataGridViewImageColumn();
            imgc.HeaderText = "Image";
            imgc.Image = null;
            imgc.Width = 180;
            imgc.ImageLayout = DataGridViewImageCellLayout.Zoom;
            imgc.Name = "Image";
            
            dataGridView1.Columns.Insert(2, imgc);
        }
        private void addRowToGrid(string Address, int number)
        {
            string Time = DateTime.Now.ToString("HH:mm:ss tt");
            string Note = "";
            string[] row = { "", "", "", "", "" };
            string Gussing="";
            DataGridViewRow rowTemp = (DataGridViewRow)dataGridView1.Rows[0].Clone();
            if (InputBox("Guessing","your guessing is:",ref Note) == DialogResult.OK){
                Gussing = Note;
            }

            dataGridView1.Rows.Add(rowTemp);
            dataGridView1.Rows[number - 1].Cells[0].Value = number.ToString();
            dataGridView1.Rows[number - 1].Cells[1].Value = Time;
            dataGridView1.Rows[number - 1].Cells[2].Value = Image.FromFile(Address);
            dataGridView1.Rows[number - 1].Cells[3].Value = Gussing;
        }
        public static DialogResult InputBox(string title, string promptText, ref string value)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
        }


        private void button4_Click(object sender, EventArgs e)
        {
            dataGridView1.CurrentRow.Cells[4].Value = textBox1.Text;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //if(comboBox1.SelectedIndex != -1)
            //{ 
                if (cam.IsRunning)
                {
                    MessageBox.Show("close the camera first!");
                }
                else
                {
                    SelectionNow = e.RowIndex;
                    if (SelectionNow != SelectionBefore)
                    {
                        pictureBox2.Image = Image.FromFile(DefaultAddress + SelectionNow + ".jpg");
                        if (dataGridView1.Rows[e.RowIndex].Cells[4].Value != null)
                        {
                            textBox1.Text = dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString();
                        }
                        else
                        {
                        textBox1.Text = null;
                         }
                    }
                }
            //}
            SelectionBefore = SelectionNow;
        }
    }
    
}
