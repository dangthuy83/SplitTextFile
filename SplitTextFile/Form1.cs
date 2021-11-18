using System.ComponentModel;
using System.IO;

namespace SplitTextFile
{
    public partial class Form1 : Form
    {
        BackgroundWorker _backgroundWorker = new BackgroundWorker();        
        public Form1()
        {
            InitializeComponent();
            _backgroundWorker.DoWork += BackgroundWorker_DoWork;
            _backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
        }

        private void BackgroundWorker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            _ = MessageBox.Show("TẠO FILE THÀNH CÔNG");
        }

        private void BackgroundWorker_DoWork(object? sender, DoWorkEventArgs e)
        {
            try
            {
                int l_counter = 0;
                int spl_counter = 1;
                string line;
                string saveFile = Application.StartupPath + @"/File Split";
                if (!Directory.Exists(saveFile))
                {
                    _ = Directory.CreateDirectory(saveFile);
                }
                StreamWriter? swWriter = new(saveFile + @"/ouput.txt");
                StreamReader swReader = new(TxtPath.Text);
                while ((line = swReader.ReadLine()) != null)
                {
                    swWriter.WriteLine(line);
                    l_counter++;

                    if (l_counter >= int.Parse(TxtSoLuong.Text))
                    {
                        swWriter.Close();
                        swWriter = null;
                        swWriter = new StreamWriter(saveFile + @"/output_" + spl_counter + ".txt");
                        spl_counter++;
                        l_counter = 0;
                    }
                }

                // Close the stream reader / writer
                swReader.Close();
                swWriter.Close();

            }
            catch (Exception ex)
            {
                _ = MessageBox.Show("KHÔNG TẠO ĐƯỢC FILE" + Environment.NewLine + ex);
            }
        }

        private void BtnBrowser_Click(object sender, EventArgs e)
        {
            Openfile();
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            if(!_backgroundWorker.IsBusy)
            {
                _backgroundWorker.RunWorkerAsync();
            }
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {

        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void Openfile()
        {
            using OpenFileDialog openFileDialog = new() { Filter = "Text File(*.txt)|*.txt|All File(*.*)|*.*", Multiselect = false };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                TxtPath.Text = openFileDialog.FileName;
            }
        }
    }
}